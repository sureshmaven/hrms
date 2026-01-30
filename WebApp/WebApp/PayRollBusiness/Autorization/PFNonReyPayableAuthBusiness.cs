using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Masters
{
    public class PFNonReyPayableAuthBusiness : BusinessBase
    {
        public PFNonReyPayableAuthBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();

        public async Task<string> getNonPayableEmpDetails(string empCode)
        {
            string qry = "select id from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and active=1 and authorisation=0 and process=0;";

            DataTable dt = await _sha.Get_Table_FromQry(qry);

            if (dt.Rows.Count > 0)
            {
                IList<PfDetails> lstDept = new List<PfDetails>();
                string genQry = "Select pf.pf_account_no as pf_no,pf.purpose_of_advance,adv.purpose_name,pf.rate_of_basic_da," +
                    "pf.eligibility_amount,pf.own_share,pf.vpf,pf.bank_share,pf.total,adv.month,pf.amount_applied,pf.sanctioned_amount " +
                    "from pr_emp_pf_nonrepayable_loan pf " +
                    "join pr_purpose_of_advance_master adv on pf.purpose_of_advance = adv.id " +
                    "WHERE pf.emp_code = " + Convert.ToInt32(empCode) + " and pf.active = 1 and authorisation=0 and process=0 ;";
                string certQry = "select id,cert_name,status from pr_emp_pf_non_cert_elg where " +
                    "emp_code=" + Convert.ToInt32(empCode) + " and active=1 and authorisation=0 and process=0;";
                DataSet ds = await _sha.Get_MultiTables_FromQry(genQry + certQry);
                DataTable common = ds.Tables[0];
                foreach (DataRow gen in common.Rows)
                {
                    lstDept.Add(new PfDetails
                    {
                        pf_no = gen["pf_no"].ToString(),
                        purpose_name = gen["purpose_name"].ToString(),
                        rate_of_basic_da =Math.Round(Convert.ToDouble( gen["rate_of_basic_da"].ToString())),
                        eligibility_amount = gen["eligibility_amount"].ToString(),
                        own_share = Convert.ToInt32(gen["own_share"]),
                        vpf = Convert.ToInt32(gen["vpf"]),
                        bank_share = Convert.ToInt32(gen["bank_share"]),
                        total = gen["total"].ToString(),
                        amount_applied = gen["amount_applied"].ToString(),
                        purpose_of_advance = gen["purpose_of_advance"].ToString(),
                        month = gen["month"].ToString(),
                        sanction = gen["sanctioned_amount"].ToString(),
                        
                    });
                }
                DataTable cert = ds.Tables[1];
                IList<certDetails> lstCer = new List<certDetails>();
                foreach (DataRow cer in cert.Rows)
                {
                    lstCer.Add(new certDetails
                    {
                        id = cer["id"].ToString(),
                        cert_name = cer["cert_name"].ToString(),
                        status = cer["status"].ToString(),
                    });
                }

                //string selQry = "select o.own_share_total,o.bank_share_total,o.vpf_total,o.da,o.basic,e.pf_no from " +
                //    "pr_ob_share o left outer join pr_emp_general e on o.emp_code = e.emp_code where o.emp_code =" + Convert.ToInt32(empCode) + "and o.active=1";
                string selQry = "select top 1 case when prnon.own_share is null then o.own_share_total else ((o.own_share_total + (case when o.own_share_intrst_total is null then 0 " +
                    "else own_share_intrst_total end))-prnon.own_share) end as own_share_total,case when prnon.vpf is " +
                    "null then (o.vpf_total+(ISNULL(o.vpf_intrst_total, 0 ))) else ((ISNULL(o.vpf_intrst_total, 0 )+o.vpf_total) - " +
                    "(select sum (vpf) from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and process=1))end as vpf_total," +
                    "case when prnon.bank_share is null then o.bank_share_total else ((o.bank_share_total + (case when o.bank_share_intrst_total is null then 0 else o.bank_share_intrst_total  end)) -prnon.bank_share)end as bank_share_total,o.da,o.basic,e.pf_no as pf_no,format(emp.RetirementDate, 'yyyy,MM,dd') as  RetirementDate from " +
                        "pr_ob_share o left join pr_emp_pf_nonrepayable_loan prnon on o.emp_code=prnon.emp_code and o.active=1  and prnon.process=1 left outer join pr_emp_general e on o.emp_code = e.emp_code join Employees emp on e.emp_code=emp.EmpId where o.emp_code =" + Convert.ToInt32(empCode) + " and o.active=1";
                //  string selQry = "select case when prnon.own_share is null then o.own_share_total else ((o.own_share_intrst_total+o.own_share_total)-prnon.own_share) end as own_share_total,case when prnon.vpf is null then o.vpf_total else ((o.vpf_total+o.vpf_intrst_total) - prnon.vpf) end as vpf_total,case when prnon.bank_share is null then o.bank_share_total else ((o.bank_share_total+o.bank_share_intrst_total) - prnon.bank_share) end as bank_share_total,o.da,o.basic,e.pf_no from " +
                //"pr_ob_share o left join pr_emp_pf_nonrepayable_loan prnon on o.emp_code=prnon.emp_code and o.active=1  and prnon.process=1 left outer join pr_emp_general e on o.emp_code = e.emp_code where o.emp_code =" + Convert.ToInt32(empCode) + "and o.active=1";
                DataTable details = await _sha.Get_Table_FromQry(selQry);

                IList<PfDetails> lstDepts = new List<PfDetails>();
                foreach (DataRow dr in details.Rows)
                {
                    lstDepts.Add(new PfDetails
                    {
                        basic = dr["basic"].ToString(),
                        pf_no = dr["pf_no"].ToString(),
                        own_share = Convert.ToInt32(dr["own_share_total"]),
                        bank_share = Convert.ToInt32(dr["bank_share_total"]),
                        da_percent = dr["da"].ToString(),
                        vpf = Convert.ToInt32(dr["vpf_total"]),
                    });
                }

                var loans = JsonConvert.SerializeObject(lstDepts);
                var loan = JsonConvert.SerializeObject(lstDept);
                var certificates = JsonConvert.SerializeObject(lstCer);
                var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var loanData = javaScriptSerializer.DeserializeObject(loan);
                var certData = javaScriptSerializer.DeserializeObject(certificates);
                var loansData = javaScriptSerializer.DeserializeObject(loans);
                var resultJson = javaScriptSerializer.Serialize(new
                {
                    loan = loanData,
                    cert = certData,
                    loans = loansData

                });
                return resultJson;
            }
            else
            {
                return "No Data Found";
            }
        }
        public async Task<string> getAuthorise(int empCode,string loantype)
        {
            int emp_code = _LoginCredential.EmpCode;
            int NewNumIndex = 0;
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            NewNumIndex++;
            sbqry.Append(GetNewNumStringArr("pr_emp_pf_nonrepayable_loan", NewNumIndex));
            string qry = " update pr_emp_pf_nonrepayable_loan set authorisation =1 " +
                " where emp_code=" + Convert.ToInt32(empCode) + " and active=1 and authorisation=0 and process=0;";

            
            sbqry.Append(qry);
            
            string qry1 = " update pr_emp_pf_non_cert_elg set authorisation= 1 " +
                " where emp_code=" + Convert.ToInt32(empCode) + " and active=1;";
            sbqry.Append(qry1);

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_nonrepayable_loan", "@idnew" + NewNumIndex, emp_code.ToString()));
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#PF Non Payable#Authorisation Processed Successfully..!!";
            }
            else
            {
                return "E#PF Non Payable#Error While Authorisation Process";
            }
        }
    }
}
