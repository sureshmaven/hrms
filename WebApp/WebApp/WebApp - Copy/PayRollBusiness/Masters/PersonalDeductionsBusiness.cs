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
    public class PersonalDeductionsBusiness : BusinessBase
    {
        public PersonalDeductionsBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public async Task<string> GetEmpDeductionDetails(int EmpId,string Field)
        {
            string qryGetDFpayfields = "";
            //string qryGetDFpayfields = "select ef.id as Id, ef.name as Name, epf.amount as Value,epf.section as Section,epf.m_id as mid, case when epf.m_id is null then 'N' else 'U' end as row_type " +
            //      "from pr_deduction_field_master ef left outer join  pr_emp_perdeductions epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + EmpId + " WHERE ef.type='per_ded';";


            //string qryGetDFpayfields = "select ef.id as Id, ef.name as Name, case when epf.amount is null then 0 else epf.amount end as Value,epf.section as Section,epf.m_id as mid, case when epf.m_id is null then 'N' else 'U' end as row_type " +
            //    "from pr_deduction_field_master ef left outer join  pr_emp_perdeductions epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + EmpId + " WHERE ef.type='per_ded';";
            if (Field != "Display")
            {
                 qryGetDFpayfields = "select ef.id as Id, ef.name as Name, epf.amount as Value,epf.section as Section,epf.m_id as mid, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type from pr_deduction_field_master ef left outer join  pr_emp_perdeductions epf " +
                "on ef.id = epf.m_id WHERE epf.emp_code = " + EmpId + " and ef.type='per_ded' and epf.amount!=0 and ef.name not in('HFC', 'HFC INT', 'PPF', 'INTEREST ON EDUCATION LOAN', 'TAX SAVER FD', 'INFRASTRUCTURE BONDS', 'Interest On NSC (Deduction)','LIC') and epf.active=1" +
                " union all" +
                " select ef.id as Id, ef.name as Name, case when epf.amount is null then 0 else epf.amount end as Value,epf.section as Section," +
                " epf.m_id as mid, case when epf.m_id is null then 'N' else 'U' end as row_type from pr_deduction_field_master ef left outer join  " +
                " pr_emp_perdeductions epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + EmpId + " WHERE ef.type = 'per_ded' and (ef.name in('HFC', 'HFC INT', 'PPF', 'INTEREST ON EDUCATION LOAN', 'TAX SAVER FD', 'INFRASTRUCTURE BONDS', 'Interest On NSC (Deduction)', 'LIC') or ef.name like '%"+ Field + "%'); ";
            }
            else
            {
                 qryGetDFpayfields = "select ef.id as Id, ef.name as Name, epf.amount as Value,epf.section as Section,epf.m_id as mid, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type from pr_deduction_field_master ef left outer join  pr_emp_perdeductions epf " +
                "on ef.id = epf.m_id WHERE epf.emp_code = " + EmpId + " and ef.type='per_ded' and epf.amount!=0 and ef.name not in('HFC', 'HFC INT', 'PPF', 'INTEREST ON EDUCATION LOAN', 'TAX SAVER FD', 'INFRASTRUCTURE BONDS', 'Interest On NSC (Deduction)','LIC') and epf.active=1" +
                " union all" +
                " select ef.id as Id, ef.name as Name, case when epf.amount is null then 0 else epf.amount end as Value,epf.section as Section," +
                " epf.m_id as mid, case when epf.m_id is null then 'N' else 'U' end as row_type from pr_deduction_field_master ef left outer join  " +
                " pr_emp_perdeductions epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + EmpId + " WHERE ef.type = 'per_ded' and ef.name in('HFC', 'HFC INT', 'PPF', 'INTEREST ON EDUCATION LOAN', 'TAX SAVER FD', 'INFRASTRUCTURE BONDS', 'Interest On NSC (Deduction)', 'LIC'); ";
            }
            

            //string qryGetDFpayfields = "select ef.id as Id, ef.name as Name, epf.amount as Value,epf.section as Section,epf.m_id as mid, " +
            //    "case when epf.m_id is null then 'N' else 'U' end as row_type from pr_deduction_field_master ef left outer join  pr_emp_perdeductions epf " +
            //    "on ef.id = epf.m_id WHERE epf.emp_code = " + EmpId + " and ef.type='per_ded' and epf.amount!=0 and ef.name not in('HFC', 'HFC INT', 'PPF', 'INTEREST ON EDUCATION LOAN', 'TAX SAVER FD', 'INFRASTRUCTURE BONDS', 'Interest On NSC (Deduction)') and epf.active=1 " +
            //    " union all "+
            //    "select ef.id as Id, ef.name as Name, epf.amount as Value,epf.section as Section,epf.m_id as mid, case when epf.m_id is null then 'N' else 'U' end as row_type " +
            //    "from pr_deduction_field_master ef left outer join  pr_emp_perdeductions epf on ef.id = epf.m_id WHERE ef.name in('HFC', 'HFC INT', 'PPF', 'INTEREST ON EDUCATION LOAN', 'TAX SAVER FD', 'INFRASTRUCTURE BONDS', 'Interest On NSC (Deduction)') " +
            //    " and ef.type = 'per_ded' and epf.emp_code = "+ EmpId + " and epf.active=1;";
            DataTable dt = await _sha.Get_Table_FromQry(qryGetDFpayfields);
            var empdfFields = dt;

            var dsGetpayfields = JsonConvert.SerializeObject(empdfFields);

            dsGetpayfields = dsGetpayfields.Replace("null", "''");
         
            var javaScriptSerializer = new JavaScriptSerializer();

            var dfDetails = javaScriptSerializer.DeserializeObject(dsGetpayfields);
            string query1 = "select id,constant from all_constants where constant not in('793,929,6305,6336,6326,123456') order by constant";
            DataTable dt1 = await _sha.Get_Table_FromQry(query1);
            var sectionfields = dt1;
            var sfields = JsonConvert.SerializeObject(sectionfields);
            var sectiondetails = javaScriptSerializer.DeserializeObject(sfields);
            var resultJson = new { DfDetails = dfDetails,sdetails= sectiondetails };

            return JsonConvert.SerializeObject(resultJson);
        }

        public async Task<string> PerDeductionsDetails(CommonPostDTO Values)
        {
            try
            {
                int FY = _LoginCredential.FY;
                string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
                int EfEmpId = Values.EntityId;
                //int fy = _LoginCredential.FY;
                //string sFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
                // N^1=356^~U^3=456^400~U^4=^111;

                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());

                int NewNumIndex = 0;
                var deductions = Values.objpd;
                var deductionsnew = Values.objperdednew;
                int m_id = 0;
                string qrydp = "";
                var sec = "";
                var amt = "";
                var name = "";
                if (deductions != null)
                {

                    foreach (var item in deductions)
                    {
                        sec = item.section;
                        amt = item.amount;
                        string qry = "";
                       
                            if (item.action == "new")
                            {
                                NewNumIndex++;
                                //2. gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_epf_earn_field", NewNumIndex));

                                //3. qry
                                qry = "Insert into pr_emp_perdeductions ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[section],[active],[trans_id]) values "
                                    + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + ")," + Values.EntityId + ", " + item.id + ",'per_ded', " + item.amount + ",'" + item.section + "',1, @transidnew);";
                                sbqry.Append(qry);

                                //4. transaction touch
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_perdeductions", "@idnew" + NewNumIndex, ""));
                            }
                            else if (item.action == "update") //update
                            {
                            if (item.amount == null && item.section=="Select")
                            {
                                qry = "Update pr_emp_perdeductions SET amount=0, section='" + item.section + "' ,  trans_id=@transidnew where m_id=" + item.id + " AND emp_code=" + Values.EntityId + " ;";
                                sbqry.Append(qry);
                            }
                            else {
                                string m_iddata = "select * from pr_emp_perdeductions where m_id=" + item.id + " AND emp_code=" + Values.EntityId + " ";
                                DataTable m_id_dtqryname = await _sha.Get_Table_FromQry(m_iddata);
                                if (m_id_dtqryname.Rows.Count <= 0)
                                {
                                    NewNumIndex++;
                                    //2. gen new num
                                    sbqry.Append(GetNewNumStringArr("pr_emp_perdeductions", NewNumIndex));
                                    qry = "Insert into pr_emp_perdeductions ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[section],[active],[trans_id]) values "
                                    + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + ")," + Values.EntityId + ", " + item.id + ",'per_ded', " + item.amount + ",'" + item.section + "',1, @transidnew);";
                                    sbqry.Append(qry);
                                }
                                else
                                {
                                    qry = "Update pr_emp_perdeductions SET amount=" + item.amount + ", section='" + item.section + "',  trans_id=@transidnew where m_id=" + item.id + " AND emp_code=" + Values.EntityId + " and active=1 ;";
                                    sbqry.Append(qry);
                                }
                                 }
                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_perdeductions", item.id.ToString(), item.amount));
                            }
                            else if (item.action == "update") //delete
                            {
                                qry = "Update pr_emp_perdeductions SET active = 0, trans_id=@transidnew where m_id=" + item.id + " AND emp_code=" + Values.EntityId + " ;";
                                sbqry.Append(qry);

                                //4. transaction touch
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_perdeductions", item.id.ToString(), ""));
                            }
                    }
                     
                }
               
                if (deductionsnew != null)
                {
                    foreach (var prData in deductionsnew)
                    {
                        sec = prData.section;
                        amt = prData.amount;
                        name = prData.name;
                        var deductiontype = "";
                        string qryname = "select name from pr_deduction_field_master";
                        DataTable dtqryname = await _sha.Get_Table_FromQry(qryname);
                       foreach(DataRow item in dtqryname.Rows)
                        {
                            deductiontype = item["name"].ToString();
                            if(name==deductiontype)
                            {
                                return "E#Deduction Type#Deduction Type Already Exists";
                                // return "I#Employee Personal Deduction#Deduction Type Already Exists";
                            }
                        }
                        if (prData.action == "New")
                            {
                            
                                StringBuilder sbqry1 = new StringBuilder();
                                //1. trans_id
                                sbqry1.Append(GenNewTransactionString());
                                NewNumIndex++;
                                //2. gen new num
                                sbqry1.Append(GetNewNumStringArr("pr_deduction_field_master", NewNumIndex));

                                //3. qry
                                qrydp = "Insert into pr_deduction_field_master " +
                                    "([id],[name],[type],[active],[trans_id]) values "
                                    + "(@idnew" + NewNumIndex + ",'" + prData.name + "','per_ded',1, @transidnew);";
                                sbqry1.Append(qrydp);

                                //4. transaction touch
                                sbqry1.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_deduction_field_master", "@idnew" + NewNumIndex, ""));
                                await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry1.ToString());

                                string qrySelect = "select id from  pr_deduction_field_master where name='" + prData.name + "' and active=1";
                                DataTable dt = await _sha.Get_Table_FromQry(qrySelect);
                                if (dt.Rows.Count > 0)
                                {
                                decimal? amount = 0;
                                foreach (DataRow dr in dt.Rows)
                                    {
                                        m_id = Convert.ToInt32(dr["id"]);
                                    amount = Convert.ToDecimal(prData.amount);
                                    NewNumIndex++;
                                        //2. gen new num
                                        sbqry.Append(GetNewNumStringArr("pr_emp_perearning", NewNumIndex));

                                        //3. qry
                                        string qry = "Insert into pr_emp_perdeductions ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[section],[active],[trans_id]) values "
                                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + ")," + Values.EntityId + ", " + m_id + ",'per_ded', " + amount + ",'" + prData.section + "',1, @transidnew);";
                                        sbqry.Append(qry);

                                        //4. transaction touch
                                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_perdeductions", "@idnew" + NewNumIndex, ""));
                                    }
                                }
                            }
                       
                       
                    }
                }
                //sbqry.Remove(sbqry.Length - 1, 1);
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#Employee Personal Deduction#Data Added Successfully";
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
