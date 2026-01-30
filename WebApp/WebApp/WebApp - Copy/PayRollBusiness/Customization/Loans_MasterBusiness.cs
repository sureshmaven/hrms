using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.DAL.Business;
using PayrollModels;
using Mavensoft.DAL.Db;
using System.Data;
using Newtonsoft.Json;
using Mavensoft.Common;
namespace PayRollBusiness.Customization
{
    public class Loans_MasterBusiness : BusinessBase
    {
        public Loans_MasterBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        SqlHelperAsync _sha = new SqlHelperAsync();


        public async Task<IList<CommonGetModel>> GetLoans_MastersData()
        {

            string getloansdata = "SELECT id,loan_id,loan_description,interest_rate FROM pr_loan_master WHERE active=1";
            DataTable dt = await _sha.Get_Table_FromQry(getloansdata);
            //var resultJson = JsonConvert.SerializeObject(dt);
            //return resultJson;

            IList<CommonGetModel> lstLoandata = new List<CommonGetModel>();

            foreach (DataRow dr in dt.Rows)
            {
                lstLoandata.Add(new CommonGetModel
                {
                    id = dr["id"].ToString(),
                    loan_id = dr["loan_id"].ToString(),
                    loan_description = dr["loan_description"].ToString(),
                    interest_rate = dr["interest_rate"].ToString(),
                    Action = "Add"
                });
            }

            return lstLoandata;
        }

        public async Task<DataTable> GetPayslipStructure()
        {

            //string getloansdata = "SELECT id,loan_id,loan_description,interest_rate FROM pr_loan_master WHERE active=1";
            //DataTable dt = await _sha.Get_Table_FromQry(getloansdata);
            //var resultJson = JsonConvert.SerializeObject(dt);
            //return resultJson;

            string qryGetPayslip = "select distinct(ef.id) as id, ef.name as name, epf.cust_status as status,ef.type as type, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_earn_field_master ef left outer join  pr_payslip_customization " +
                 "epf on ef.id = epf.m_id and ef.type = epf.field_type and epf.active = 1 WHERE ef.type='pay_fields' " +
                 "union all select distinct(ef.id) as id, ef.name as name, epf.cust_status as status,ef.type as type, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_allowance_field_master ef left outer join  pr_payslip_customization " +
                 "epf on ef.id = epf.m_id and ef.type = epf.field_type and epf.active = 1  WHERE ef.type='EMPA' " +
                 "union all select distinct(ef.id) as id, ef.name as name, epf.cust_status as status,ef.type as type, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_allowance_field_master ef left outer join  pr_payslip_customization " +
                 "epf on ef.id = epf.m_id and ef.type = epf.field_type and epf.active = 1  WHERE ef.type='EMPSA'" +
                 ";";
            DataTable dt = await _sha.Get_Table_FromQry(qryGetPayslip);
            //var empAllowanceSpecialFields = dt;

            //var empjson = JsonConvert.SerializeObject(empAllowanceSpecialFields);


            //empjson = empjson.Replace("null", "''");

            return dt;
        }

        public async Task<string> UpdateLoansData(CommonPostDTO Values)
        {
            var multData = Values.mstrloanobject;
            string qry = "";
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            var loanname = "";
            var loanid = "";
            int NewNumIndex = 0;

            foreach (var loans in multData)
            {
                if (loans.Action == "New")
                {
                    string qryname = "select loan_id,loan_description from pr_loan_master";
                    DataTable dtqryname = await _sha.Get_Table_FromQry(qryname);
                    foreach (DataRow lntypes in dtqryname.Rows)
                    {
                        loanid =lntypes["loan_id"].ToString();
                        loanname = lntypes["loan_description"].ToString();
                        if ( loans.loan_id==loanid || loans.loan_description==loanname)
                        {
                            return "E#Loantype Type#Loan Type Already Exists";
                            // return "I#Employee Personal Deduction#Deduction Type Already Exists";
                        }
                    }
                    NewNumIndex++;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_loan_master", NewNumIndex));
                    qry = "INSERT INTO pr_loan_master(id,loan_id,loan_description,interest_rate,active,trans_id) " +
                        "VALUES(@idnew" + NewNumIndex + ",'" + loans.loan_id +"','"+loans.loan_description+"','"+loans.interest_rate+ "',1,@transidnew)";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_loan_master", "@idnew" + NewNumIndex, ""));
                }

                else if (loans.Action == "Update")
                {
                    //qry = "Update pr_emp_lic_details SET days=" + newVal + ", trans_id=@transidnew where dept_id=" + pkid + " AND Emp_Code=" + EmpCode + " ;";
                    qry = " Update pr_loan_master SET loan_description='" + loans.loan_description+ "',interest_rate=" + loans.interest_rate + " where id=" + loans.id  + " ";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_loan_master", loans.id.ToString(), ""));
                }
                else if (loans.Action == "Deleted")
                {
                    qry = " Update pr_loan_master SET active=0 where id=" + loans.id + " AND loan_id='" + loans.loan_id + "' ;";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_loan_master", loans.id.ToString(), ""));
                }
            }
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#Data Submission#Data Submitted Successfully..!!";
            }
            else
            {
                return "E#Error#Error While Data Submission";
            }
        }

        public async Task<string> updatePayslipStructure(PayslipStructure Values)
        {
            var multData = Values.payslip;
            string qry = "";
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            var loanname = "";
            var loanid = "";
            int NewNumIndex = 0;

            foreach (var payslip in multData)
            {
                if (payslip.action == "Insert")
                {
                    
                    NewNumIndex++;
                    //2. gen new num create table pr_payslip_customization(id int, m_id int,field_type nvarchar,cust_status bit,active int, trans_id int);
                    sbqry.Append(GetNewNumStringArr("pr_payslip_customization", NewNumIndex));
                    
                        qry = "INSERT INTO pr_payslip_customization(id,m_id,field_type,cust_status,active,trans_id) " +
                        "VALUES(@idnew" + NewNumIndex + "," + payslip.id + ",'" + payslip.type + "','" + payslip.status + "',1,@transidnew);";
                    
                    
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_payslip_customization", "@idnew" + NewNumIndex, ""));
                }

                else if (payslip.action == "Update")
                {
                    //qry = "Update pr_emp_lic_details SET days=" + newVal + ", trans_id=@transidnew where dept_id=" + pkid + " AND Emp_Code=" + EmpCode + " ;";
                    
                        qry = " Update pr_payslip_customization SET cust_status='" + payslip.status + "' " +
                            "where m_id=" + payslip.id + " and field_type='" + payslip.type + "';";
                    
                    //qry = " Update pr_payslip_customization SET loan_description='" + loans.loan_description + "',interest_rate=" + loans.interest_rate + " where id=" + loans.id + " ";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_payslip_customization", payslip.id.ToString(), ""));
                }
                
            }
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#Data Submission#Data Submitted Successfully..!!";
            }
            else
            {
                return "E#Error#Error While Data Submission";
            }
        }


        public async Task<string> updateEncashmentStructure(EncashmentStructure Values)
        {
            var multData = Values.enashEarn;
            var multDData = Values.enashDed;
            string qry = "";
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            var loanname = "";
            var loanid = "";
            int NewNumIndex = 0;
            if (multData != null)
            {
                foreach (var payslip in multData)
                {
                    if (payslip.action == "Insert")
                    {

                        NewNumIndex++;
                        //2. gen new num create table pr_payslip_customization(id int, m_id int,field_type nvarchar,cust_status bit,active int, trans_id int);
                        sbqry.Append(GetNewNumStringArr("pr_encashment_earnings_customization", NewNumIndex));

                        qry = "INSERT INTO pr_encashment_earnings_customization(id,m_id,field_type,cust_status,active,trans_id) " +
                        "VALUES(@idnew" + NewNumIndex + "," + payslip.id + ",'" + payslip.type + "','" + payslip.status + "',1,@transidnew);";


                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_encashment_earnings_customization", "@idnew" + NewNumIndex, ""));
                    }

                    else if (payslip.action == "Update")
                    {
                        //qry = "Update pr_emp_lic_details SET days=" + newVal + ", trans_id=@transidnew where dept_id=" + pkid + " AND Emp_Code=" + EmpCode + " ;";

                        qry = " Update pr_encashment_earnings_customization SET cust_status='" + payslip.status + "' " +
                            "where m_id=" + payslip.id + " and field_type='" + payslip.type + "';";

                        //qry = " Update pr_payslip_customization SET loan_description='" + loans.loan_description + "',interest_rate=" + loans.interest_rate + " where id=" + loans.id + " ";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_encashment_earnings_customization", payslip.id.ToString(), ""));
                    }

                }
            }

            if (multDData != null)
            {
                foreach (var payslip in multDData)
                {
                    if (payslip.action == "Insert")
                    {

                        NewNumIndex++;
                        //2. gen new num create table pr_payslip_customization(id int, m_id int,field_type nvarchar,cust_status bit,active int, trans_id int);
                        sbqry.Append(GetNewNumStringArr("pr_encashment_deductions_customization", NewNumIndex));

                        qry = "INSERT INTO pr_encashment_deductions_customization(id,m_id,field_type,cust_status,active,trans_id) " +
                        "VALUES(@idnew" + NewNumIndex + "," + payslip.id + ",'" + payslip.type + "','" + payslip.status + "',1,@transidnew);";


                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_encashment_deductions_customization", "@idnew" + NewNumIndex, ""));
                    }

                    else if (payslip.action == "Update")
                    {
                        //qry = "Update pr_emp_lic_details SET days=" + newVal + ", trans_id=@transidnew where dept_id=" + pkid + " AND Emp_Code=" + EmpCode + " ;";

                        qry = " Update pr_encashment_deductions_customization SET cust_status='" + payslip.status + "' " +
                            "where m_id=" + payslip.id + " and field_type='" + payslip.type + "';";

                        //qry = " Update pr_payslip_customization SET loan_description='" + loans.loan_description + "',interest_rate=" + loans.interest_rate + " where id=" + loans.id + " ";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_encashment_deductions_customization", payslip.id.ToString(), ""));
                    }

                }
            }

            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#Data Submission#Data Submitted Successfully..!!";
            }
            else
            {
                return "E#Error#Error While Data Submission";
            }
        }


        public async Task<string> GetEncashmentStructure()
        {

            //string getloansdata = "SELECT id,loan_id,loan_description,interest_rate FROM pr_loan_master WHERE active=1";
            //DataTable dt = await _sha.Get_Table_FromQry(getloansdata);
            //var resultJson = JsonConvert.SerializeObject(dt);
            //return resultJson;

            string qryGetPayslip = "select distinct(ef.id) as id, ef.name as name, epf.cust_status as status,ef.type as type, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_earn_field_master ef left outer join  pr_encashment_earnings_customization " +
                 "epf on ef.id = epf.m_id and ef.type = epf.field_type and epf.active = 1 WHERE ef.type='pay_fields' " +
                 "union all select distinct(ef.id) as id, ef.name as name, epf.cust_status as status,ef.type as type, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_allowance_field_master ef left outer join  pr_encashment_earnings_customization " +
                 "epf on ef.id = epf.m_id and ef.type = epf.field_type and epf.active = 1  WHERE ef.type='EMPA' " +
                 "union all select distinct(ef.id) as id, ef.name as name, epf.cust_status as status,ef.type as type, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_allowance_field_master ef left outer join  pr_encashment_earnings_customization " +
                 "epf on ef.id = epf.m_id and ef.type = epf.field_type and epf.active = 1  WHERE ef.type='EMPSA'" +
                 ";";

            string gryGetEncashment = "select dfm.id as id, dfm.name, dfc.cust_status as status,dfm.type as type , " +
                                         "case when dfc.m_id is null then 'N' else 'U' end as row_type " +
                                         "from pr_deduction_field_master dfm " +
                                         "left outer join  pr_encashment_deductions_customization dfc " +
                                         "on dfm.id = dfc.m_id and dfm.type=dfc.field_type and dfc.active = 1 " +
                                         "where dfm.type='EPD';";
            DataSet dsGetpayfields = await _sha.Get_MultiTables_FromQry(qryGetPayslip + gryGetEncashment);

            //DataSet dsGetpayfields = await _sha.Get_MultiTables_FromQry(qryGetDFentryfields + qryGetCFentryfields + qryGetDeptEntryfields);

            var dtEffields = dsGetpayfields.Tables[0];
            var dtDffields = dsGetpayfields.Tables[1];



            var efjson = JsonConvert.SerializeObject(dtEffields);
            var dfjson = JsonConvert.SerializeObject(dtDffields);



            efjson = efjson.Replace("null", "''");
            dfjson = dfjson.Replace("null", "''");



            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var EfDetails = javaScriptSerializer.DeserializeObject(efjson);
            var DfDetails = javaScriptSerializer.DeserializeObject(dfjson);



            var resultJson = javaScriptSerializer.Serialize(new { EfDetails = EfDetails, DfDetails = DfDetails });

            return JsonConvert.SerializeObject(resultJson);

            //DataTable dt = await _sha.Get_Table_FromQry(qryGetPayslip);
            ////var empAllowanceSpecialFields = dt;

            ////var empjson = JsonConvert.SerializeObject(empAllowanceSpecialFields);


            ////empjson = empjson.Replace("null", "''");

            //return dt;
        }
    }
}
