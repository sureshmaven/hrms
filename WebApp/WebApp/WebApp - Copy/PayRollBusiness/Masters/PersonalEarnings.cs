using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PayRollBusiness.Masters
{
    public class PersonalEarnings : BusinessBase
    {
        public PersonalEarnings(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public async Task<string> GetEmpPayDetails(int EmpId)
        {
            string qryGetEFpayfields = "select ef.id as Id,epf.Id as c_id ,case when epf.section is null then ef.name else CONCAT(ef.name, ' ( ', epf.section, ' )') end as Name, epf.amount as Value, case when epf.m_id is null then 'N' else 'U' end as row_type " +
                "from pr_earn_field_master ef left outer join  pr_emp_perearning epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + EmpId + " WHERE ef.type='per_earn';";

            DataSet dsGetpayfields = await _sha.Get_MultiTables_FromQry(qryGetEFpayfields);

            var dtEFfields = dsGetpayfields.Tables[0];



            var efjson = JsonConvert.SerializeObject(dtEFfields);


            efjson = efjson.Replace("null", "''");

            var javaScriptSerializer = new JavaScriptSerializer();

            var efDetails = javaScriptSerializer.DeserializeObject(efjson);

            var resultJson = new { EfDetails = efDetails };

            return JsonConvert.SerializeObject(resultJson);
        }

        public async Task<string> PerEarDetails(CommonPostDTO Values)
        {
            try
            {
                string qryEp = "";
                int m_id = 0;
                var personal = Values.objPE;
                //int FY = DateTime.Now.Year + 1;
                //string FM = DateTime.Now.ToString("MM-dd-yyyy");

                int FY = _LoginCredential.FY;
                string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
                int EfEmpId = Values.EntityId;

                // N^1=356^~U^3=456^400~U^4=^111;

                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());

                int NewNumIndex = 0;
                int NewNumIndex1 = 0;



                if (Values.StringData != null)
                {
                    string[] EfarrRows = Values.StringData.Split('~');

                    foreach (string rdata in EfarrRows)
                    {
                        var arrData = rdata.Split('=');

                        var arrTypId = arrData[0].Split('^'); //N^1
                        var type = arrTypId[0];
                        var pkid = arrTypId[1];

                        var arrVals = arrData[1].Split('^'); //456^400
                        var newVal = arrVals[0];
                        var oldVal = arrVals[1];

                        string qry = "";
                        if (type == "N")
                        {
                            NewNumIndex++;
                            //2. gen new num
                            sbqry.Append(GetNewNumStringArr("pr_emp_epf_earn_field", NewNumIndex));

                            //3. qry
                            qry = "Insert into pr_emp_perearning ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + ")," + Values.EntityId + ", " + pkid + ",'per_earn', " + newVal + ",1, @transidnew);";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_perearning", "@idnew" + NewNumIndex, ""));
                        }
                        else if (type == "U" && newVal != "") //update
                        {
                            qry = "Update pr_emp_perearning SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " and active=1 AND emp_code=" + Values.EntityId + " ;";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_perearning", pkid.ToString(), newVal));
                        }
                        else if (type == "U" && newVal == "") //delete
                        {
                            qry = "Update pr_emp_perearning SET active = 0, trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_perearning", pkid.ToString(), oldVal));
                        }
                    }


                }
                if (personal != null)
                {
                    foreach (var prData in personal)
                    {
                        if (prData.Action == "New")
                        {
                            StringBuilder sbqry1 = new StringBuilder();
                            //1. trans_id
                            sbqry1.Append(GenNewTransactionString());
                            NewNumIndex1++;
                            //2. gen new num
                            sbqry1.Append(GetNewNumStringArr("pr_earn_field_master", NewNumIndex1));

                            //3. qry
                            qryEp = "Insert into pr_earn_field_master " +
                                "([id],[name],[type],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex1 + ",'" + prData.Name + "','per_earn',1, @transidnew);";
                            sbqry1.Append(qryEp);

                            //4. transaction touch
                            sbqry1.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_earn_field_master", "@idnew" + NewNumIndex1, ""));
                            await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry1.ToString());

                            string qrySelect = "select id from  pr_earn_field_master where name='" + prData.Name + "' and active=1";
                            DataTable dt = await _sha.Get_Table_FromQry(qrySelect);
                            if (dt.Rows.Count > 0)
                            {
                                decimal? amount = 0;
                                foreach (DataRow dr in dt.Rows)
                                {
                                    m_id = Convert.ToInt32(dr["id"]);
                                    amount = Convert.ToDecimal(prData.Value);
                                    NewNumIndex++;
                                    //2. gen new num
                                    sbqry.Append(GetNewNumStringArr("pr_emp_perearning", NewNumIndex));

                                    //3. qry
                                    string qry = "Insert into pr_emp_perearning ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[active],[trans_id]) values "
                                        + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + ")," +
                                        "" + Values.EntityId + ", " + m_id + ",'per_earn', " + amount + ",1, @transidnew);";
                                    sbqry.Append(qry);

                                    //4. transaction touch
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_perearning", "@idnew" + NewNumIndex, ""));
                                }
                            }
                        }
                    }
                }

                //return "E#Error 123#Error 456";
                //sbqry.Remove(sbqry.Length - 1, 1);
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#Employee Pesonal Earning#Data Added Successfully";
                }
                else
                {
                    return "E#Error 123#Error 456";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "E#Error:#" + msg;
            }
        }
    }
}
