using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using PayrollModels.Masters;
using System.Configuration;
using System.Data;
namespace PayRollBusiness.Reports
{
  public class EncashmentSummaryBus:BusinessBase
    {
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        public EncashmentSummaryBus(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        public async Task<IList<CommonReportModel>> EncashSummaryData(string fromDate, string toDate, string RegEmp, string SupEmp)
        {
            int SlNo = 1;
            int RowCnt = 0;
            string branch = "";
            string q1 = "";
            string efdate = "";
            string etdate = "";
            string general = PrConstants.REGULAR;
            string adhoc = PrConstants.ADHOC;
            string Fdate = fromDate;
            string Tdate = toDate;
            string encashdate = "";
            string oldencashdate = "";

            int iFY = _LoginCredential.FY;

            int dtFM = _LoginCredential.FM;

            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");

            if (fromDate == "^1")
            {
                Fdate = "01-01-01";
                Tdate = "01-01-01";
            }

            DateTime fdate = Convert.ToDateTime(Fdate);
            DateTime tdate = Convert.ToDateTime(Tdate);
            Fdate = fdate.ToString("yyyy-MM-dd");
            Tdate = tdate.ToString("yyyy-MM-dd");
            efdate= fdate.ToString("dd-MM-yyyy");
            etdate = tdate.ToString("dd-MM-yyyy");
            //StringBuilder sb = new System.Text.StringBuilder();
            //sb.Append(efdate);
            //sb.Append("".PadLeft(20, ' ').Replace(" ", " "));
            //sb.Append(etdate);
            //if (RegEmp != "" && RegEmp != "undefined")
            //{
            //    q1 = "  and e.RetirementDate >="+ FM;

            //}
            //if (SupEmp != "" && RegEmp == "undefined")
            //{
            //    q1 = " and e.RetirementDate <"+ FM;
            //}

            //string query1 = "select sum(r.gross_amount) as GrossSal,sum(r.dd_provident_fund) as PF," +
            //    "sum(r.dd_income_tax) as IT,sum(r.deductions_amount) as Totdedamt," +
            //    "sum(r.net_amount) as netamount,Sum(r.vpf) as VPF from(select gross_amount, dd_provident_fund, dd_income_tax, deductions_amount, " +
            //    "0 as vpf, net_amount, emp_code, emp_id from pr_emp_payslip a  " +
            //    "where a.id not in (select payslip_mid from pr_emp_payslip_deductions) and a.spl_type = 'Encashment' union all " +
            //    "select gross_amount, dd_provident_fund, dd_income_tax, deductions_amount, dd_amount as vpf," +
            //    "net_amount,a.emp_code,a.emp_id from pr_emp_payslip a " +
            //    "join pr_emp_payslip_deductions pd on a.id = pd.payslip_mid where a.spl_type = 'Encashment') as r " +
            //    "join ple_type ple on ple.empid = r.emp_id join employees e on e.EmpId = r.emp_code " +
            //    "where ple.fm BETWEEN '"+Fdate+"' and '"+Tdate+"' "+q1+"";
            //string query1 = "select case when sum(r.gross_amount) is NULL then 0 else sum(r.gross_amount) end as GrossSal," +
            //    "case when sum(r.dd_provident_fund) is NULL then 0 else sum(r.dd_provident_fund)end as PF," +
            //    "case when sum(r.dd_income_tax) is NULL then 0 else sum(r.dd_income_tax)end as IT," +
            //    "case when sum(r.deductions_amount) is NULL then 0 else sum(r.deductions_amount) end as Totdedamt," +
            //    "case when sum(r.net_amount) is NULL then 0 else sum(r.net_amount)end as netamount," +
            //    "case when Sum(r.vpf) is NULL then 0 else Sum(r.vpf) end as VPF " +
            //    "from(select gross_amount, dd_provident_fund, dd_income_tax, deductions_amount, 0 as vpf, " +
            //    " net_amount, emp_code, emp_id from pr_emp_payslip a where a.id " +
            //    "not in (select payslip_mid from pr_emp_payslip_deductions)and a.spl_type = 'Encashment' " +
            //    "union all select gross_amount, dd_provident_fund, dd_income_tax,deductions_amount, " +
            //    "dd_amount as vpf,net_amount,a.emp_code,a.emp_id from pr_emp_payslip a " +
            //    "join pr_emp_payslip_deductions pd on a.id = pd.payslip_mid " +
            //    "where a.spl_type = 'Encashment') as r join ple_type " +
            //    "ple on ple.empid = r.emp_id join employees e on e.EmpId = r.emp_code where ple.fm " +
            //    "BETWEEN '"+ Fdate + "' and '"+ Tdate + "'";

            string query1 = "select case when sum(r.gross_amount) is NULL then 0 else sum(r.gross_amount) end as GrossSal,case when sum(r.dd_provident_fund) is NULL then 0 else sum(r.dd_provident_fund)end as PF,case when sum(r.dd_income_tax)" +
                " is NULL then 0 else sum(r.dd_income_tax)end as IT,case when sum(r.deductions_amount) is NULL then 0 else sum(r.deductions_amount) " +
                "end as Totdedamt,case when sum(r.net_amount) is NULL then 0 else sum(r.net_amount)end as netamount , 0 as VPF from pr_emp_payslip r " +
                " where r.spl_type = 'Encashment' and r.fm >= '"+Fdate+"' and r.fm <= '"+ Tdate + "' union all" +
                " select 0 as GrossSal, 0 as PF, 0 as IT, 0 as Totdedamt, 0 as netamount,case when Sum(d.dd_amount) is NULL " +
                "then 0 else Sum(d.dd_amount) end as VPF from pr_emp_payslip_deductions d join pr_emp_payslip p on p.id = d.payslip_mid where p.spl_type = 'Encashment' and p.fm >= '"+ Fdate + "' and p.fm <= '"+ Tdate + "';";
            DataTable dt = await _sha.Get_Table_FromQry(query1);

           
                //encashdate = Convert.ToDateTime(dr["fm"]).ToString("MMM-yyyy");
                string GrossSalary = dt.Rows[0]["GrossSal"].ToString();
                string ProvidentFund = dt.Rows[0]["PF"].ToString();
                string VPF = dt.Rows[1]["Vpf"].ToString();
                string IT = dt.Rows[0]["IT"].ToString();
                string TotalDeductionamt = dt.Rows[0]["Totdedamt"].ToString();
                string NET = dt.Rows[0]["netamount"].ToString();
                //if (GrossSalary != "" && ProvidentFund != "" && VPF != "" && TotalDeductionamt != "" && NET!="")
                //{
                //if (oldencashdate != encashdate)
                //{
                //    //crm = new CommonReportModel
                //    //{
                //    //    RowId = RowCnt++,
                //    //    HRF = "H",
                //    //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                //    // + ReportColHeader(0, "FOR THE MONTH OF", encashdate)
                //    //};
                //    //lst.Add(crm);
                //}
                // oldencashdate = encashdate;
                if (GrossSalary != "" && ProvidentFund != "" && VPF != "" && TotalDeductionamt != "" && NET != "")
                { 
                    crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                        // SlNo= SlNo++.ToString(),
                            SlNo = ReportColConvertToDecimal(GrossSalary.ToString()),
                            column2 = ReportColConvertToDecimal(ProvidentFund.ToString()),
                        column3 = ReportColConvertToDecimal(VPF.ToString()),
                        column4 = ReportColConvertToDecimal(IT.ToString()),
                        column5 = ReportColConvertToDecimal(TotalDeductionamt.ToString()),
                        column6 = ReportColConvertToDecimal(NET.ToString()),

                    };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "PASSED FOR PAYMENT OF GROSS AMOUNT OF    Rs  ", ReportColConvertToDecimal(GrossSalary))
                        };
                        lst.Add(crm);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooter(0, "FOR PAYMENT OF NET   AMOUNT OF   Rs  ", ReportColConvertToDecimal(NET))
                        };
                        lst.Add(crm);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "PASSED FOR PAYMENT OF BANK CONTRIBUTION TOWARDS PF  ", ReportColConvertToDecimal(ProvidentFund))
                        };
                        lst.Add(crm);
                        //CommonReportModel tot = getTotal(branch, dt);
                        //tot.RowId = RowCnt++;
                        //lst.Add(tot);          
                }

          
            return lst;
            // return await _sha.Get_Table_FromQry(query);
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
