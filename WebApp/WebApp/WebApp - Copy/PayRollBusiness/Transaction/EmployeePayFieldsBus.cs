using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PayRollBusiness.Transaction
{
    public class EmployeePayFieldsBus : BusinessBase
    {
        public EmployeePayFieldsBus(LoginCredential loginCredential) : base(loginCredential)
        {
            
        }
        
        public async Task<string> GetEmpPayDetails(int EmpId)
        {
            string qryGetEFpayfields = "select ef.id as Id, ef.name as Name, epf.amount as Value, case when epf.m_id is null then 'N' else 'U' end as row_type " +
                "from pr_earn_field_master ef left outer join  pr_emp_epf_earn_field epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_id = " + EmpId + " WHERE ef.type='EPF';";
            string qryGetDFpayfields = "select df.id as Id, df.name as Name, dpf.amount as Value, case when dpf.m_id is null then 'N' else 'U' end as row_type " +
                "from pr_deduction_field_master df left outer join  pr_emp_epf_deduction_field dpf on df.id = dpf.m_id and dpf.active = 1 and dpf.emp_id = " + EmpId + " WHERE df.type='EPF';";
            DataSet dsGetpayfields = await _sha.Get_MultiTables_FromQry(qryGetEFpayfields + qryGetDFpayfields);

            var dtEFfields = dsGetpayfields.Tables[0];
            var dtDFfields = dsGetpayfields.Tables[1];

            var dfjson = JsonConvert.SerializeObject(dtDFfields);
            var efjson = JsonConvert.SerializeObject(dtEFfields);

            dfjson = dfjson.Replace("null", "''");
            efjson = efjson.Replace("null", "''");

            var javaScriptSerializer = new JavaScriptSerializer();
            var dfDetails = javaScriptSerializer.DeserializeObject(dfjson);
            var efDetails = javaScriptSerializer.DeserializeObject(efjson);

            var resultJson = javaScriptSerializer.Serialize(new { DfDetails = dfDetails, EfDetails = efDetails });

            return JsonConvert.SerializeObject(resultJson);
        }

        public async Task<string> AddPayDetails(CommonPostDTO Values)
        {
            int FY = DateTime.Now.Year + 1;
            string FM = DateTime.Now.ToString("MM-dd-yyyy");
            int EfEmpId = Values.EntityId;

            // N^1=356^~U^3=456^400~U^4=^111;

            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());

            int NewNumIndex = 0;
            
           
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
                        qry = "Insert into pr_emp_epf_earn_field ([id],[fy],[fm],[emp_id],[m_id],[m_type],[amount],[active],[trans_id]) values "
                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "'," + Values.EntityId + ", " + pkid + ",'EF', " + newVal + ",1, @transidnew);";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_epf_earn_field", "@idnew" + NewNumIndex, ""));
                    }
                    else if (type == "U" && newVal != "") //update
                    {
                        qry = "Update pr_emp_epf_earn_field SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_id=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_epf_earn_field", pkid.ToString(), oldVal));
                    }
                    else if (type == "U" && newVal == "") //delete
                    {
                        qry = "Update pr_emp_epf_earn_field SET active = 0, trans_id=@transidnew where m_id=" + pkid + " AND emp_id=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_epf_earn_field", pkid.ToString(), ""));
                    }
                }
               

            }

            if (Values.StringData2 != null)
            {
                string[] DfarrRows = Values.StringData2.Split('~');

                foreach (string rdata in DfarrRows)
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
                        sbqry.Append(GetNewNumStringArr("pr_emp_epf_deduction_field", NewNumIndex));

                        //3. qry
                        qry = "Insert into pr_emp_epf_deduction_field ([id],[fy],[fm],[emp_id],[m_id],[m_type],[amount],[active],[trans_id]) values "
                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "'," + Values.EntityId + ", " + pkid + ",'DF', " + newVal + ",1, @transidnew);";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_epf_deduction_field", "@idnew" + NewNumIndex, ""));
                    }
                    else if (type == "U" && newVal != "") //update
                    {
                        qry = "Update pr_emp_epf_deduction_field SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_id=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_epf_deduction_field", pkid.ToString(), oldVal));
                    }
                    else if (type == "U" && newVal == "") //delete
                    {
                        qry = "Update pr_emp_epf_deduction_field SET active = 0, trans_id=@transidnew where m_id=" + pkid + " AND emp_id=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_epf_deduction_field", pkid.ToString(), ""));
                    }
                }
            }

            //sbqry.Remove(sbqry.Length - 1, 1);
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#Employee Payfield#Data Added Successfully";
            }
            else
            {
                return "E#Error 123#Error 456";
            }
            
        }

    }
}
