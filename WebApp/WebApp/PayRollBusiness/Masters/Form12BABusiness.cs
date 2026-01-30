using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayRollBusiness.PayrollService;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Masters
{
    public class Form12BABusiness : BusinessBase
    {
        public Form12BABusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();

        public async Task<string> GetForm12BAData(string empCode)
        {
            string selQuery = "select fm12.id as Id,fm12.nature_of_perq as name, sf.perq_amt as preq,sf.rec_amt as rec,sf.tax_amt as tax " +
                " from pr_emp_incometax_12ba_master fm12 left outer join pr_emp_incometax_12ba sf on  " +
                " fm12.id=sf.m_id and sf.active = 1 and sf.emp_code =" + Convert.ToInt32(empCode) + " where fm12.type='12ba';";
            DataTable dt = await _sha.Get_Table_FromQry(selQuery);

            return JsonConvert.SerializeObject(dt);
           
        }
        public async Task<string> GetForm12Data(string empCode)
        {

            string date = "select fm as year FROM pr_month_details WHERE active = 1";
            int End = 0;
            int start = 0;
            string fm = "";
            DataTable dtaedt = await _sha.Get_Table_FromQry(date);
            foreach (DataRow dr in dtaedt.Rows)
            {
                DateTime dateTime = Convert.ToDateTime(dr["year"]);
                fm = dateTime.ToString("yyyy-MM-dd");
                string[] arr = fm.Split('-');
                string arr1 = arr[0];

                string finaciadate = "" + Convert.ToInt32(arr1) +"-03-31";


                if (DateTime.Parse(fm) <= DateTime.Parse(finaciadate))
                {
                    End = Convert.ToInt32(arr1);
                    start = Convert.ToInt32(arr1) - 1;

                }
                else if (DateTime.Parse(fm) >= DateTime.Parse(finaciadate))
                {
                    End = Convert.ToInt32(arr1) + 1;
                    start = Convert.ToInt32(arr1);
                }
            }

            string qryGetEFpayfields = "select ef.id as Id, ef.nature_of_perq as Name," +
                "case when ef.nature_of_perq='PAN no' then((SELECT PanCardNo FROM Employees where empid = " + Convert.ToInt32(empCode) + ")) " +
                "else epf.value end as Value, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_emp_incometax_12ba_master ef left outer join  pr_emp_incometax_12b " +
                 "epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = "  +Convert.ToInt32(empCode)  + " WHERE ef.type='12b' " +
                 " and  (select doj FROM employees WHERE empid = " + Convert.ToInt32(empCode) + ") between DATEFROMPARTS("+start+",04,01) and DATEFROMPARTS("+End+",03,01);";
            string str = "0";
            DataTable dt = await _sha.Get_Table_FromQry(qryGetEFpayfields);
            return JsonConvert.SerializeObject(dt);

        }
        public async Task<string> saveForm12BADetails(Form12BA Values)
        {
            try
            {
                int FY = _LoginCredential.FY;
                string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
                int EfEmpId = Values.EntityId;
                

                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());

                int NewNumIndex = 0;
                var form12 = Values.form12ba;
                int? id;
                int? preq;
                int? rec;
                int? tax;
                int? paid;
                if (form12 != null)
                {

                    foreach (var item in form12)
                    {
                        id = Convert.ToInt32(item.id);
                        preq = Convert.ToInt32(item.preq);
                        rec = Convert.ToInt32(item.rec);
                        tax = Convert.ToInt32(item.tax);
                        paid = preq + rec + tax;
                        string qry = "";

                        if (item.action == "new")
                        {
                            NewNumIndex++;
                            //2. gen new num
                            sbqry.Append(GetNewNumStringArr("pr_emp_incometax_12ba", NewNumIndex));

                            //3. qry
                            qry = "Insert into pr_emp_incometax_12ba ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[perq_amt],[rec_amt],[tax_amt],[tax_paid],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + ")," + Values.EntityId + ", " + id + "," + preq + "," + rec + "," + tax + "," + paid + ",1, @transidnew);";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_incometax_12ba", "@idnew" + NewNumIndex, ""));
                        }
                        else if (item.action == "update") //update
                        {
                            NewNumIndex++;
                            qry = "Update pr_emp_incometax_12ba SET perq_amt=" + preq + ",rec_amt=" + rec + ",tax_amt=" + tax + ",tax_paid=" + paid + ",  trans_id=@transidnew where m_id=" + item.id + " AND emp_code=" + Values.EntityId + " ;";
                                sbqry.Append(qry);
                           
                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_incometax_12ba", item.id.ToString(), item.preq));
                        }
                        
                    }

                }
                //sbqry.Remove(sbqry.Length - 1, 1);
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#Form 12BA#Data Added Successfully";
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

        public async Task<string> saveForm12Data(CommonPostDTO Values)
        {

            DateTime gen_date = DateTime.Now;
            int FY = _LoginCredential.FY;
            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
           
            int EfEmpId = Values.EntityId;

            // N^1=356^~U^3=456^400~U^4=^111;

            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());

            int NewNumIndex = 0;
            string date = "";
            string new_date = "";

            //Insertining details table
            string qry = "";



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

                    //string qry = "";
                    if (type == "N")
                    {
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_incometax_12b", NewNumIndex));

                        //3. qry
                        qry = "Insert into pr_emp_incometax_12b ([id],[fy],[fm],[emp_id],[emp_code]," +
                            "[m_id],[value],[active],[trans_id]) values "
                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid=" + EfEmpId + ")," +
                            "" + EfEmpId + "," + pkid + ", '" + newVal + "',1, @transidnew);";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_incometax_12b", "@idnew" + NewNumIndex, ""));
                    }
                    else if (type == "U" && newVal != "") //update
                    {
                        qry = "Update pr_emp_incometax_12b SET value='" + newVal + "', trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_incometax_12b", pkid.ToString(), oldVal));
                    }
                    else if (type == "U" && newVal == "") //delete
                    {
                        qry = "Update pr_emp_incometax_12b SET value='" + newVal + "', trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_incometax_12b", pkid.ToString(), oldVal));
                    }
                    //qry = "Update pr_emp_pay_field SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                    //sbqry.Append(qry);

                    ////4. transaction touch
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", pkid.ToString(), oldVal));
                }
            }






            // sbqry.Remove(sbqry.Length - 1, 1);
            

            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#Form 12B#Data Added Successfully";
            }
            else
            {
                return "E#Error#Error While data Form 12B data Submission";
            }

        }
    }
}
