using Mavensoft.DAL.Business;
using Mavensoft.DAL.Db;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PayRollBusiness.Customization
{
    public class LoanMISReport : BusinessBase
    {
        public LoanMISReport(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        IList<CommonReportModel> lst = new List<CommonReportModel>();

        public async Task<IList<CommonReportModel>> GetLoanMIS_Data(string emp_code, string mnth, string payslip)
        {
            if (emp_code.Contains("^"))
            {
                emp_code = "0";
                mnth = "01-01-01";
                payslip = "";
            }
            DateTime str = Convert.ToDateTime(mnth);
            string str1 = str.ToString("yyyy-MM-dd");
            string[] datearr = str1.Split('-');
            if (payslip != "")
            {
                payslip = payslip.Replace(",", "','");
            }

            string qry= "";
            DataTable Loans = new DataTable();
            string qrysel1 = "";
            DataTable pftotal = new DataTable();
            int slno = 0;
            if (emp_code != "All")
            {
                qry = "select l.emp_code,e.LastName,lm.loan_id,l.adv_total_amount,l.adv_total_installment,l.adv_completed_installment," +
                    "l.adv_remaining_installment,l.adv_installment_amount,l.os_principal_amount,l.child_os_interest_amount," +
                    "l.adjust_fm from dim_emp e join Loansfact l on e.EmpId = l.emp_code join pr_loan_master lm on lm.id = l.loan_type_mid " +
                    "where l.emp_code in ("+ emp_code + ") and l.adjust_fm='"+ str1 + "'";
                Loans = await _sha.Get_Table_FromQry(qry);
             
            }
            else
            {
                qry = "select l.emp_code,e.LastName,lm.loan_id,l.adv_total_amount,l.adv_total_installment,l.adv_completed_installment," +
                    "l.adv_remaining_installment,l.adv_installment_amount,l.os_principal_amount,l.child_os_interest_amount," +
                    "l.adjust_fm from dim_emp e join Loansfact l on e.EmpId = l.emp_code join pr_loan_master lm on lm.id = l.loan_type_mid " +
                    "where  l.adjust_fm='" + str1 + "'";
                Loans = await _sha.Get_Table_FromQry(qry);

            }
            try
            {
                if (emp_code != "0")
                {
                    foreach (DataRow dr in Loans.Rows)
                    {
                   
                        lst.Add(new CommonReportModel
                        {
                            RowId = slno++,
                            column1 = dr["emp_code"].ToString(),
                            column2 = dr["LastName"].ToString(),
                            column3 = dr["loan_id"].ToString(),
                            column4 = dr["adv_total_amount"].ToString(),
                            column5 = dr["adv_total_installment"].ToString(),
                            column6 = dr["adv_completed_installment"].ToString(),
                            column7 = dr["adv_remaining_installment"].ToString(),
                            column8 = dr["adv_installment_amount"].ToString(),
                            column9 = dr["os_principal_amount"].ToString(),
                            column10= dr["child_os_interest_amount"].ToString(),
                            column11 = dr["adjust_fm"].ToString(),
                        });
                    }
                    //foreach (DataRow dr1 in pftotal.Rows)
                    //{
                    //    lst.Add(new CommonReportModel
                    //    {
                    //        RowId = slno++,
                    //        column1 = "Grand Total",
                    //        column2 = "",
                    //        column3 = dr1["own_share"].ToString(),
                    //        column4 = dr1["bank_share"].ToString(),
                    //        column5 = dr1["bank_share"].ToString(),
                    //        column6 = dr1["pension_open"].ToString(),
                    //    });
                    //}
                }
            }
            catch (Exception ex)
            {
            }
            return lst;
        }
    }

}
