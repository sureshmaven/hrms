using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.DAL.Business;
using PayrollModels;
using PayrollModels.Masters;
using Newtonsoft.Json;
using System.Data;

namespace PayRollBusiness.Reports
{
    public class LoanScheduleReportBusiness : BusinessBase
    {
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        public LoanScheduleReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        int RowCnt = 0;
        int SNo = 1;
        string loandes = "";
        string oldloandes = "";
        public async Task<IList<CommonReportModel>> GetLoanScheduleReports(string loancode, string schedulecode,string mnth)
        {
            string qry = "";
            //var empid = "0";
            string loancodes = loancode;
            string schedulecodes = schedulecode;
            DateTime str = Convert.ToDateTime(mnth);
            string str1 = str.ToString("yyyy-MM-dd");
            if (loancode != "" && schedulecode.Contains("^"))
            { 
            if (loancode.Contains("^") || loancode=="")
            {
                loancodes = "0";
                 mnth = "01-01-01";
                }
           
            qry = "select distinct lm.loan_description as grpcol,e.EmpId,d.Code as Description,e.ShortName as EmployeeName ,l.total_amount as amount," +
                    "l.completed_installment as Inst,l.total_installment as NoOfInstallments from pr_emp_adv_loans l " +
                    "inner join pr_loan_master lm on lm.id = l.loan_type_mid inner join Employees e on e.empid = l.emp_code " +
                    "inner join Designations d on d.id = e.currentdesignation";

              if (loancode != "All")
                {
                    qry += " where lm.id in (" + loancodes + ") and month(l.sanction_date)=month('"+str1+ "') and year(l.sanction_date)=year('"+str1+ "') order by grpcol";

              }
            }

            else
            {
                if (schedulecode.Contains("^") || schedulecode == "")
                {
                    loancodes = "0";

                }
                qry = " select distinct am.name as grpcol,e.EmpId,d.Code as Description,e.ShortName as EmployeeName,l.total_amount as amount," +
                    "l.completed_installment as Inst,l.total_installment as NoOfInstallments from pr_emp_adv_loans l " +
                    "inner join All_Masters am on am.id = l.code_master inner join Employees e on e.empid = l.emp_code" +
                    " inner join Designations d on d.id = e.currentdesignation";

                if (schedulecode != "All")
                {
                    qry += " where am.id in (" + schedulecodes + ") and month(l.sanction_date)=month('" + str1 + "') and year(l.sanction_date)=year('" + str1 + "') order by grpcol";

                }
            }

            //}
            DataTable dt = await _sha.Get_Table_FromQry(qry);
            foreach (DataRow dr in dt.Rows)
            {

                loandes = dr["grpcol"].ToString();
                if (oldloandes != loandes)
                {
                  
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>~</span>"
                        + ReportColHeaderValueOnly(0,dr["grpcol"].ToString()),
                        column2 = "`",
                        column3 = "`",
                        column4 = "`",
                        column5 = "`",
                        column6 = "`",

                    };
                    lst.Add(crm);

                    //crm = new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",

                    //     grpclmn = "<span style='color:#C8EAFB'>~</span>"
                    //     + ReportColHeaderValueOnly(0, "Emp Code")
                    //    + ReportColHeaderValueOnly(33, "Emp Name")
                    //    + ReportColHeaderValueOnly(40, "Desig")
                    //    + ReportColHeaderValueOnly(32, "Amount")
                    //    + ReportColHeaderValueOnly(18, "Inst Paid")
                    //     + ReportColHeaderValueOnly(42, "No.Of Inst")
                    //};
                    //lst.Add(crm);
                }

                oldloandes = dr["grpcol"].ToString();
               
                crm = new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "R",
                    // grpclmn = SNo++.ToString(),
                    grpclmn = dr["EmpId"].ToString(),
                    column2 = dr["EmployeeName"].ToString(),
                    column3 = dr["Description"].ToString(),
                    column4 = ReportColConvertToDecimal(dr["amount"].ToString()),
                    column5 = dr["Inst"].ToString(),
                    column6 = dr["NoOfInstallments"].ToString(),

                };
                lst.Add(crm);
            }
            return lst;
        }

        public string ReportColConvertToDecimal(string value)
        {

            if (value == "")
            {
                value = "0";
            }
            decimal Drvalue = Convert.ToDecimal(value.ToString()) + 0.00M;
            decimal DPT = Convert.ToDecimal(String.Format("{0:0.00}", Drvalue));
            string NwDPT = String.Format("{0:n}", DPT);


            return NwDPT;
        }

    }
}
