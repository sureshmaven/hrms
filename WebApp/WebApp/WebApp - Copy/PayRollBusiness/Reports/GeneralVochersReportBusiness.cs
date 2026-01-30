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
    public class GeneralVochersReportBusiness : BusinessBase
    {
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        public GeneralVochersReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public async Task<IList<CommonReportModel>> GeneralVochersData(string month, string RegEmp, string SupEmp, string Debit, string Credit, string All)
        {
            int SlNo = 1;
            int RowCnt = 0;
            string DescriptionofGLHead = "";
            string q1 = "";
            string q2 = "";
            string headofcdata = "";
            string branchdata = "";
            string pfloans = "";
            string allloans = "";
            string deductionsdata = "";
            string general = PrConstants.REGULAR;
            string adhoc = PrConstants.ADHOC;
            string encashment = PrConstants.ENCASHMENT;
            DateTime str = Convert.ToDateTime(month);
            string str1 = str.ToString("yyyy-MM-dd");
            string str2 = str.ToString("yyyy-MM-dd");
            string dm = str.ToString("MMM-yyyy");
            string test = "2019-05-01";
            string fm = _LoginCredential.FM.ToString();
            int fy = 0;
            DataTable dt;
            DataTable dt1;
            DataTable dt2;
            DataTable dt3;
            DataTable dt4;
            if (fm == "01" || fm == "02" || fm == "03")
            {
                fy = _LoginCredential.FY;
            }
            else
            {
                fy = _LoginCredential.FY - 1;
            }

            string day = "01";
            string[] fmon = new string[] { fy.ToString(), fm, day };
            string fmm = string.Join("-", fmon);
            string active = "";
            if (str1 == fmm)
            {
                active = " p.active =1";
            }
            else
            {
                active = " p.active =0";
            }
            if (str1 != "")
            {
                DateTime birthDate = Convert.ToDateTime(str1);
                string year = Convert.ToString(birthDate.Year);
                string month1 = Convert.ToString(birthDate.Month - 1);
                string day1 = Convert.ToString(birthDate.Day);
                str2 = year + '-' + month1 + '-' + day1;
                q2 = "  p.fm ='" + str2 + "' ";
            }
            if (str1 != "" && RegEmp != "" && RegEmp != "undefined")
            {
                q1 = "  p.fm ='" + str1 + "' and p.spl_type in('Regular', 'Adhoc')  ";
                // q1 = "  p.fm ='" + str1 + "' and p.spl_type != '" + encashment + "' ";

            }
            if (str1 != "" && SupEmp != "" && RegEmp == "undefined")
            {
                q1 = "  p.fm ='" + str1 + "' and p.spl_type='" + adhoc + "'";
            }
            if (Debit != "" && Debit != "undefined" && Debit != null)
            {

                //headofcdata = "select 'Total Recoveries - Head Office' as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, " +
                //    "sum(p.deductions_amount ) as TotalDeductions from pr_emp_payslip p join pr_interface_vochers_codes b on b.vocher_name = p.branch " +
                //    "where  " + q1 + " and vocher_name in ('OtherBranch', 'HO Bkg-Br')  " +
                //    "group by   fas_gl_code2,fas_gl_code1 order by fas_gl_code1 ";
                headofcdata = "select 'Total Recoveries - Head Office' as DescriptionofGLHead,'' as FASGLCODE,case when sum(TotalDeductions) is null then 0 else sum(TotalDeductions) end  as TotalDeductions from  (select 'Total Recoveries - Head Office' as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as " +
 " FASGLCODE, sum(p.deductions_amount) as TotalDeductions from pr_emp_payslip p join pr_interface_vochers_codes b on " +
 " b.vocher_name = p.branch where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc') and vocher_name in ('OtherBranch', 'HO Bkg-Br')" +
 " group by fas_gl_code2,fas_gl_code1 " +
 " union all " +
 "select 'Total Recoveries - Head Office' as DescriptionofGLHead,'' as FASGLCODE, ((select case when sum(dd_provident_fund) is null then 0 else sum(dd_provident_fund) end as TotalDeductions from pr_emp_payslip o " +
 "join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and spl_type = 'Adhoc' and p.branch in( 43,44) )" +
 "+(select case when sum(NPS) is null then 0 else sum(NPS) end as TotalDeductions from pr_emp_payslip o join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and spl_type = 'Adhoc' and p.branch in(43, 44) )+" +
 "((select case when sum(bank_share) is null then 0 else sum(bank_share) end  from pr_ob_share o " +
 "join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and bank_share!= 0 and p.branch in( 43,44))+" +
 "(select case when sum(nps_bank_share) is null then 0 else sum(nps_bank_share) end  from pr_ob_share o " +
 "join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and nps_bank_share!= 0 and p.branch in( 43,44))+" +
 "( select case when sum(amount) is null then 0 else sum(amount) end from pr_emp_adhoc_deduction_field o join employees p on o.emp_code = p.empid " +
 " where fm = '" + str1 + "' and p.branch in(43, 44) and m_id = 586 ))) as TotalDeductions  " +
 "union all " +
 "select 'Total Recoveries - Head Office' as DescriptionofGLHead,'' as FASGLCODE,case when sum(pension_open) is null then 0 else sum(pension_open) end as TotalDeductions from pr_ob_share o " +
 "join employees p on o.emp_code = p.empid where o.fm = '" + str1 + "' and p.branch in( 43,44) ) as x ";

                //                branchdata = "select DescriptionofGLHead,FASGLCODE,sum(TotalDeductions) as deductions,vocher from (select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead, "+
                // " concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, sum(p.deductions_amount) as TotalDeductions "+
                //" ,b.vocher_type as vocher from pr_emp_payslip p join pr_interface_vochers_codes b on b.vocher_name = p.branch " +
                //"where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc') and b.vocher_type = 'branch' and b.vocher_name != 'HO Bkg-Br' " +
                //"group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                //"union all " +
                //"select concat('Total Recoveries - ', b.name) as DescriptionofGLHead,concat(d.fas_gl_code2, '/', d.fas_gl_code1) as FASGLCODE, " +
                //"(sum(pension_open) + sum(bank_share)) as pension,d.vocher_type as vocher from pr_ob_share o " +
                //"join employees p on o.emp_code = p.empid join branches b on b.id = p.branch join pr_interface_vochers_codes d on d.vocher_name = b.name "+
                //  "where o.fm = '" + str1 + "' and branch!= 43 group by b.name,vocher_name,d.vocher_type, fas_gl_code2,fas_gl_code1 " +
                //  "union all " +
                //  "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE," +
                //  " sum(p.dd_provident_fund) as TotalDeductions,b.vocher_type as vocher from pr_emp_payslip p join pr_interface_vochers_codes b on b.vocher_name = p.branch " +
                //  "where p.fm = '" + str1 + "' and p.spl_type in('Adhoc') and b.vocher_type = 'branch' and b.vocher_name != 'HO Bkg-Br' group by vocher_name,b.vocher_type," +
                //  "fas_gl_code2,fas_gl_code1) as aa group by DescriptionofGLHead,FASGLCODE,vocher";

                branchdata = "select DescriptionofGLHead,FASGLCODE,sum(TotalDeductions) as TotalDeductions,type,FASGORDER from (select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE," +
                    "sum(p.deductions_amount) as TotalDeductions,b.vocher_type as type,b.fas_gl_code1 as FASGORDER from pr_emp_payslip p join pr_interface_vochers_codes b on b.vocher_name = p.branch where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc') and b.vocher_type = 'branch' " +
                    "and b.vocher_name != 'HO Bkg-Br' group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    "union all " +
                    "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE,(sum(NPS)+sum(dd_provident_fund))  as TotalDeductions,b.vocher_type as type ,b.fas_gl_code1 as FASGORDER from pr_emp_payslip o join pr_interface_vochers_codes b on b.vocher_name = o.branch " +
                    "join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and spl_type = 'Adhoc' and p.branch not in( 43,44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    "union all " +
                    "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE,(sum(bank_share)+sum(nps_bank_share)) as TotalDeductions,b.vocher_type as type,b.fas_gl_code1 as FASGORDER from pr_ob_share o " +
                    "join employees p on o.emp_code = p.empid join branches bb on bb.id = p.branch join pr_interface_vochers_codes b on b.vocher_name = bb.name " +
                    " where fm = '" + str1 + "' and bank_share!= 0 and p.branch not in(43,44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    " union all  select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead," +
                    "concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE,(sum(amount))as TotalDeductions," +
                    "b.vocher_type as type ,b.fas_gl_code1 as FASGORDER from pr_emp_adhoc_deduction_field o " +
                    "join employees p on o.emp_code = p.empid " +
                    "join Branches br on br.id = p.Branch join pr_interface_vochers_codes b " +
                    "on b.vocher_name = br.Name where o.fm = '" + str1 + "'   and p.branch not in(43, 44) and m_id = 586  group by vocher_name, b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    //"union all  select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead," +
                    //"concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE,(sum(amount)) as TotalDeductions," +
                    //"b.vocher_type as type ,b.fas_gl_code1 as FASGORDER from pr_emp_adhoc_contribution_field o " +
                    //"join employees p on o.emp_code = p.empid join Branches br on br.id = p.Branch " +
                    //"join pr_interface_vochers_codes b on b.vocher_name = br.Name " +
                    //"where o.fm = '" + str1 + "'   and p.branch not in(43, 44)and m_id = 42 AND active=1 group by vocher_name," +
                    //"b.vocher_type, fas_gl_code2,fas_gl_code1" +
                    " union all " +
                    " select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE," +
                    " sum(pension_open) as TotalDeductions,b.vocher_type as type,b.fas_gl_code1 as FASGORDER from pr_ob_share o join employees p on o.emp_code = p.empid join branches bb on bb.id = p.branch " +
                    " join pr_interface_vochers_codes b on b.vocher_name = bb.name where o.fm = '" + str1 + "'  and p.branch not in( 43,44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    " " +
                    " ) as x group by DescriptionofGLHead,FASGLCODE,type,FASGORDER order by FASGORDER ";

                //branchdata = "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, " +
                //   "sum(p.deductions_amount ) as TotalDeductions,b.vocher_type from pr_emp_payslip p join pr_interface_vochers_codes b " +
                //   "on b.vocher_name = p.branch where  " + q1 + " and b.vocher_type = 'branch' and b.vocher_name !='HO Bkg-Br'  group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 order by fas_gl_code1";

                //string branchdataotherfullclearingloans = "select sum(principal_paid_amount) as principal_paid_amount from pr_emp_adv_loans_bef_monthend l join pr_emp_adv_loans_adjustments_bef_monthend adj on adj.loan_sl_no = l.loan_sl_no  where installments_amount < principal_paid_amount; ";
                //                string branchdataotherfullclearingloans = "select sum(principal_paid_amount) as principal_paid_amount from pr_emp_adv_loans_bef_monthend l join pr_emp_adv_loans_adjustments_bef_monthend adj  on adj.loan_sl_no = l.loan_sl_no  " +
                //                    "where installments_amount < (principal_paid_amount)and loan_type_mid not in (16, 17, 18, 19, 20, 21, 26, 27)  " +
                //                    "and adj.fm = '" + str1 + "' ";

                //                string branchdataotherprincipalinstpaidamt = "select sum(principal_paid_amount)+sum(interest_paid_amount) as pricipl_inst_paid_amount from  " +
                //" pr_emp_adv_loans_bef_monthend l join pr_emp_adv_loans_adjustments_bef_monthend  adj  on adj.loan_sl_no = l.loan_sl_no " +
                // " join pr_Emp_payslip pslip on pslip.emp_code = l.emp_code  and pslip.fm = '" + str1 + "'  join pr_Emp_payslip_deductions ded on ded.payslip_mid = pslip.id and l.loan_type_mid = ded.dd_mid and dd_type = 'Loan' " +
                // " where installments_amount>= (principal_paid_amount + interest_paid_amount) and loan_type_mid in (16, 17, 18, 19, 20, 21, 26, 27) " +
                //" and principal_balance_amount = 0  and adj.fm = '" + str1 + "' group by dd_amount having(sum(principal_paid_amount)+sum(interest_paid_amount))!= dd_amount ";


                //string branchdataotherfullclearingloans = "select SUM(adj.principal_paid_amount+adj.interest_paid_amount) as principal_paid_amount from  pr_emp_adv_loans l " +
                //    "join pr_emp_adv_loans_adjustments adj on adj.emp_adv_loans_mid = l.id join pr_emp_adv_loans_adjustments adj1 on adj1.emp_adv_loans_mid = l.id " +
                //    "where adj.fm = '" + str1 + "'  and adj.principal_balance_amount = 0  and adj1.fm = '" + str2 + "' and adj.principal_paid_amount > adj1.principal_paid_amount"; 

                //string branchdataotherprincipalinstpaidamt = " select SUM(adj.principal_paid_amount+adj.interest_paid_amount) as pricipl_inst_paid_amount  from  pr_emp_adv_loans l " +
                //    "join pr_emp_adv_loans_adjustments adj on adj.emp_adv_loans_mid = l.id join pr_emp_adv_loans_adjustments adj1 on adj1.emp_adv_loans_mid = l.id " +
                //    "where adj.fm = '" + str1 + "'  and adj.principal_balance_amount != 0  and adj1.fm = '" + str2 + "' and adj.principal_paid_amount > adj1.principal_paid_amount " +
                //    "and adj.installments_amount > adj1.installments_amount and completed_installment> 1 AND adj1.installments_amount != 0 ";

                //string totamtOfBranches = "select sum(ammount.TotalDeductions) as TOTAL from (select concat('Total Recoveries - ', b.vocher_name) " +
                //    "as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, " +
                //    "sum(p.deductions_amount ) as TotalDeductions,b.vocher_type from pr_emp_payslip p " +
                //    "join pr_interface_vochers_codes b on b.vocher_name = p.branch where   " + q1 + "  and b.vocher_type = 'branch' and b.vocher_name !='HO Bkg-Br' " +
                //    "group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1) as ammount";
                string totamtOfBranches = "select sum(TOTAL) as TOTAL from (select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE,sum(p.deductions_amount) as TOTAL ,b.vocher_type from pr_emp_payslip p join pr_interface_vochers_codes b " +
                     " on b.vocher_name = p.branch where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc') and b.vocher_type = 'branch' and b.vocher_name != 'HO Bkg-Br' group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                     " union all  " +
                     "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, (sum(NPS)+sum(dd_provident_fund)) as TOTAL,b.vocher_type from pr_emp_payslip o join pr_interface_vochers_codes b on b.vocher_name = o.branch " +
                     "join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and spl_type = 'Adhoc' and p.branch not in( 43,44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                     "union all " +
                     "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, (sum(bank_share)+sum(nps_bank_share)) as TOTAL,b.vocher_type from pr_ob_share o " +
                     "join employees p on o.emp_code = p.empid join branches bb on bb.id = p.branch join pr_interface_vochers_codes b on b.vocher_name = bb.name where fm = '" + str1 + "' and bank_share!= 0 and p.branch not in( 43,44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                     "union all  select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead," +
                     "concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, (sum(amount)) as TOTAL," +
                     "b.vocher_type from pr_emp_adhoc_deduction_field o join employees p on o.emp_code = p.empid " +
                     "join branches bb on bb.id = p.branch join pr_interface_vochers_codes b on " +
                     "b.vocher_name = bb.name where fm = '" + str1 + "' and m_id = 586  and p.branch not " +
                     "in(43, 44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                     //"union all  select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead," +
                     //"concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, (sum(amount)) as TOTAL," +
                     //"b.vocher_type from pr_emp_adhoc_contribution_field o join employees p on o.emp_code = p.empid " +
                     //"join branches bb on bb.id = p.branch join pr_interface_vochers_codes b " +
                     //"on b.vocher_name = bb.name where fm = '" + str1 + "' and m_id = 42 AND active=1 and p.branch not in(43, 44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1" +
                     "  union all select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, " +
                     "sum(pension_open) as TOTAL,b.vocher_type from pr_ob_share o join employees p on o.emp_code = p.empid join branches bb on bb.id = p.branch " +
                     "join pr_interface_vochers_codes b on b.vocher_name = bb.name where o.fm = '" + str1 + "'  and p.branch not in( 43,44) " +
                     "group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                     ") as x ";


                dt = await _sha.Get_Table_FromQry(headofcdata);
                dt1 = await _sha.Get_Table_FromQry(branchdata);
                DataTable dtBrtot = await _sha.Get_Table_FromQry(totamtOfBranches);
                //DataTable dtBrtot1 = await _sha.Get_Table_FromQry(branchdataotherfullclearingloans);
                //DataTable dtBrtot2 = await _sha.Get_Table_FromQry(branchdataotherprincipalinstpaidamt);

                //string total = dtBrtot.Rows[0]["Total"].ToString();
                //string total1 = dtBrtot1.Rows[0]["principal_paid_amount"].ToString();
                double total2 = 0;

                double total1 = 0;
                double totals = 0;

                //string principal_paid_amount = dtBrtot1.Rows[0]["principal_paid_amount"].ToString();
                //string pricipl_inst_paid_amt = dtBrtot2.Rows[0]["pricipl_inst_paid_amount"].ToString();
                string total = dtBrtot.Rows[0]["Total"].ToString();

                if (dtBrtot.Rows.Count > 0 && total != "")
                {
                    totals = Convert.ToDouble(dtBrtot.Rows[0]["Total"].ToString());
                }
                else
                {
                    totals = 0;
                }
                //if (dtBrtot1.Rows.Count > 0 && principal_paid_amount != "")
                //{
                //    total1 = Convert.ToDouble(principal_paid_amount);
                //}
                //else
                //{
                //    total1 = 0;
                //}

                //if (dtBrtot2.Rows.Count > 0  && pricipl_inst_paid_amt != "")
                //{
                //    total2 = Convert.ToDouble(dtBrtot2.Rows[0]["pricipl_inst_paid_amount"].ToString());
                //}
                //else
                //{
                //    total2 = 0;
                //}

                //string total2 = dtBrtot2.Rows[0]["pricipl_inst_paid_amount"].ToString();

                double btotal = (totals + total1 + total2);

                decimal Dcbtotal = Convert.ToDecimal(btotal.ToString()) + 0.00M;
                decimal DPTbtotal = Convert.ToDecimal(String.Format("{0:0.00}", Dcbtotal));

                string Nwbtotal = String.Format("{0:n}", DPTbtotal);

                //double btotal = (Convert.ToDouble(dtBrtot.Rows[0]["Total"]) + Convert.ToDouble(dtBrtot1.Rows[0]["principal_paid_amount"]) + total2);
                double tot = 0;
                if (dt.Rows.Count > 0 || dt1.Rows.Count > 0)
                {
                    if (total != "")
                    {
                        tot = btotal;
                    }
                    else
                    {
                        tot = 0;

                    }

                    double grandtotdebit = Convert.ToDouble(dt.Rows[0]["TotalDeductions"]) + tot;

                    decimal Dcgrandtotdebit = Convert.ToDecimal(grandtotdebit.ToString()) + 0.00M;
                    decimal DPTgrandtotdebit = Convert.ToDecimal(String.Format("{0:0.00}", Dcgrandtotdebit));
                    string Nwgrandtotdebit = String.Format("{0:n}", DPTgrandtotdebit);
                    //HEAD OFFICE
                    foreach (DataRow dr in dt.Rows)
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                             + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                          + ReportColHeaderValueOnly11(0, "PAYROLL SYSTEM ")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColHeaderValueOnly11(0, "DETAILS OF VOCHERS PREPARED IN RESPECT OF SALARIES")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColHeaderValueOnly11(0, "DEDUCTION TYPE : DEBIT")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColFooter(0, "FOR THE MONTH OF ", month)
                        };
                        lst.Add(crm);

                        DescriptionofGLHead = dr["DescriptionofGLHead"].ToString();
                        string FASGLCODE = dr["FASGLCODE"].ToString();
                        string TotalDeductions = dr["TotalDeductions"].ToString();
                        if (TotalDeductions == "")
                        {
                            TotalDeductions = "0";
                        }
                        decimal DTotalDeductions = Convert.ToDecimal(TotalDeductions.ToString()) + 0.00M;
                        decimal DPTTotalDeductions = Convert.ToDecimal(String.Format("{0:0.00}", DTotalDeductions));
                        string NwTotalDeductions = String.Format("{0:n}", DPTTotalDeductions);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            // SlNo= SlNo++.ToString(),
                            SlNo = dr["DescriptionofGLHead"].ToString(),
                            column2 = dr["FASGLCODE"].ToString(),
                            column3 = NwTotalDeductions.ToString(),

                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooterAlign(230, "Total", NwTotalDeductions.ToString())
                        };
                        lst.Add(crm);
                    }

                    //BRANCHES

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                   + ReportColHeaderValueOnly11(0, "PAYROLL SYSTEM  ")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColHeaderValueOnly11(0, "DETAILS OF VOCHERS PREPARED IN RESPECT OF SALARIES")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                    + ReportColHeaderValueOnly11(0, "DEDUCTION TYPE : DEBIT")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                  + ReportColFooter(0, "FOR THE MONTH OF ", month)
                    };
                    lst.Add(crm);
                    foreach (DataRow dr1 in dt1.Rows)
                    {
                        DescriptionofGLHead = dr1["DescriptionofGLHead"].ToString();
                        string FASGLCODE = dr1["FASGLCODE"].ToString();
                        string TotalDeductions = dr1["TotalDeductions"].ToString();
                        if (TotalDeductions == "")
                        {
                            TotalDeductions = "0";
                        }
                        decimal D1TotalDeductions = Convert.ToDecimal(TotalDeductions.ToString()) + 0.00M;
                        decimal DPTTotalDeductions = Convert.ToDecimal(String.Format("{0:0.00}", D1TotalDeductions));
                        string NwD1TotalDeductions = String.Format("{0:n}", DPTTotalDeductions);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            // SlNo= SlNo++.ToString(),
                            SlNo = dr1["DescriptionofGLHead"].ToString(),
                            column2 = dr1["FASGLCODE"].ToString(),
                            column3 = NwD1TotalDeductions,

                        };
                        lst.Add(crm);

                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooterAlign(230, "Total ", Nwbtotal.ToString())
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                   + ReportColFooterAlign(230, "Grand Total ", Nwgrandtotdebit.ToString())
                    };
                    lst.Add(crm);

                }
            }
            else if (Debit == "undefined" && All == "undefined" && Credit != null)
            {
                // - initially in deductionsdata query active status is static value i.e 1 so changed to dynamic active status // chaitanya on 20/03/2020
                deductionsdata = " select concat('Total Recoveries - ', 'FEST ADVANCE') as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else '' end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid=123 and " + q1 + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1 " +
                   " union all select concat('Total Recoveries - ', 'NON PRIORITY PERSONAL LOAN') as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else '' end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid=(select id from pr_deduction_field_master where name ='NON PRIORITY PERSONAL LOAN') and " + q1 + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1  " +
                   "union all select concat('Total Recoveries - Adhoc -', e.name) as DescriptionofGLHead," +
                   "case when concat(b.fas_gl_code2,'/',b.fas_gl_code1)= '/' then null else '' end as  fasglcode, sum(amount) as TotalDeductions " +
                   "from pr_emp_adhoc_deduction_field f join pr_deduction_field_master e on f.m_id = e.id " +
                   "join pr_interface_vochers_codes b on b.vocher_name = e.name where b.vocher_type in " +
                   "('Deductions', 'LoanInt', 'PFLoanInt', 'Loans') and f.m_id != 123 and f.m_id != (select id from pr_deduction_field_master " +
                   "where name = 'NON PRIORITY PERSONAL LOAN') and f.fm = '" + str1 + "'  and e.name " +
                   "in ('Club Subscription', 'TELANGANA OFFICERS ASSN')and f.active = 1 group by name , fas_gl_code2,fas_gl_code1  " +
                    "union all select concat('Total Recoveries - ', d.dd_name) as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else '' end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid!=123 and d.dd_mid!=(select id from pr_deduction_field_master where name ='NON PRIORITY PERSONAL LOAN') and " + q1


+ " and d.dd_name != 'NPS'" + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1 " +
                   "union all select 'Total Recoveries - Income Tax' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2,'/',b.fas_gl_code1)= '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode from pr_interface_vochers_codes b where vocher_name = 'Income Tax') ,sum(dd_income_tax) from pr_emp_payslip p where " + q1 + " " +
                   "union all select 'Total Recoveries - Prof.Tax' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2,'/',b.fas_gl_code1)= '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode from pr_interface_vochers_codes b where vocher_name = 'Prof. Tax'),sum(dd_prof_tax) from pr_emp_payslip p where " + q1 + " " +
                   "  union all " +
  "select 'Total Recoveries - Max Pension' as DescriptionofGLHead,'' as num,((select sum(pension_open) as TotalDeductions from pr_ob_share " +
  "where fm = '" + str1 + "'))  as TotalDeductions " +
  " union all select 'Total Recoveries - PF Contribution' as DescriptionofGLHead,'' as num1, " +
  " ((select case when  sum(dd_provident_fund)   is null then 0 else sum(dd_provident_fund) end as TotalDeductions   from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Adhoc' ) " +
  " + (select sum(bank_share) as TotalDeductions  from pr_ob_share where fm = '" + str1 + "'  and bank_share!= 0) )  as TotalDeductions " +
  "union all select 'Total Recoveries - NPS Contribution' as DescriptionofGLHead,'' as num1," +
  " (( select case when sum(NPS) is null then 0 else sum(NPS) end as TotalDeductions from pr_emp_payslip where fm = '" + str1 + "' " +
  "and spl_type = 'Adhoc' ) + (select sum(NPS_bank_share)+ (select sum(amount) as TotalDeductions from pr_emp_adhoc_contribution_field where fm = '" + str1 + "' and m_id = 42 ) as TotalDeductions from pr_ob_share where fm = '" + str1 + "' " +
  "and NPS_bank_share!= 0) ) as TotalDeductions ";
                //end

                string dedtotal = "  select sum(ammount.TotalDeductions) as Total from (select concat('Total Recoveries - ', 'FEST ADVANCE') as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else '' end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid=123 and " + q1 + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1 " +
                   " union all select concat('Total Recoveries - ', 'NON PRIORITY PERSONAL LOAN') as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else '' end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid=(select id from pr_deduction_field_master where name ='NON PRIORITY PERSONAL LOAN') and " + q1 + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1 " +
                    "union all select concat('Total Recoveries - ', d.dd_name) as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else concat(b.fas_gl_code2,'/',b.fas_gl_code1) end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid!=123 and d.dd_mid!=(select id from pr_deduction_field_master where name ='NON PRIORITY PERSONAL LOAN') and " + q1 + " and d.dd_name != 'NPS'" + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1 " +
                   "union all select 'Total Recoveries - Income Tax' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2,'/',b.fas_gl_code1)= '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode from pr_interface_vochers_codes b where vocher_name = 'Income Tax') ,sum(dd_income_tax) from pr_emp_payslip p where " + q1 + " " +
                   "union all select 'Total Recoveries - Prof.Tax' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2,'/',b.fas_gl_code1)= '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode from pr_interface_vochers_codes b where vocher_name = 'Prof. Tax'),sum(dd_prof_tax) from pr_emp_payslip p where " + q1 + " " +
                   " 	union all " +
  "select 'Total Recoveries -Max Pension' as DescriptionofGLHead,'' as num,((select sum(pension_open) as TotalDeductions from pr_ob_share " +
  " where fm = '" + str1 + "')) " +
  "union all select 'Total Recoveries - PF Contribution' as DescriptionofGLHead,'' as num1, " +
  " ((select case when  sum(dd_provident_fund)   is null then 0 else sum(dd_provident_fund) end as TotalDeductions   from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Adhoc' ) " +
  " + (select sum(bank_share) as TotalDeductions  from pr_ob_share where fm = '" + str1 + "'  and bank_share!= 0) )  as TotalDeductions  " +
  "union all select 'Total Recoveries - NPS Contribution' as DescriptionofGLHead,'' as num1, (( select case when sum(NPS) is null then 0 else sum(NPS) end as " +
  "TotalDeductions from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Adhoc' ) + (select sum(NPS_bank_share) as TotalDeductions " +
  "from pr_ob_share where fm = '" + str1 + "' and NPS_bank_share!= 0) +(select case when sum(amount) " +
  "is null then 0 else sum(amount) end as TotalDeductions from pr_emp_adhoc_deduction_field " +
  "where fm='" + str1 + "' and active=1 and m_id in (85,145))+(select case when sum(amount) is null then 0 " +
  "else sum(amount) end as TotalDeductions from pr_emp_adhoc_contribution_field " +
  "where fm = '" + str1 + "'  and m_id = 42  )))   as ammount ";

                //allloans = "select case when vocher_type='LoanInt' then concat('Interst on - ',vocher_name) else " +
                //   "concat('Principle on - ',vocher_name) end as DescriptionofGLHead,case when vocher_type = 'LoanInt' " +
                //   "then sum(interest_paid_amount) else sum(adj.principal_paid_amount) end as TotalDeductions,b.fas_gl_code2 as FASGLCODE " +
                //   "from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id join pr_emp_adv_loans_adjustments_bef_monthend " +
                //   "adj on adj.loan_sl_no = l.loan_sl_no join pr_interface_vochers_codes b on b.vocher_name = lm.loan_description " +
                //   "where adj.fm = '" + str1 + "' and loan_description not like'%PF%' group by loan_description,fas_gl_code2," +
                //   "fas_gl_code1,vocher_name,vocher_type";

                //string loantot = "select sum(ammount.TotalDeductions) as Total from (select case when vocher_type='LoanInt' then concat('Interst on - ',vocher_name) else " +
                //   "concat('Principle on - ',vocher_name) end as DescriptionofGLHead,case when vocher_type = 'LoanInt' " +
                //   "then sum(interest_paid_amount) else sum(adj.principal_paid_amount) end as TotalDeductions,b.fas_gl_code2 as FASGLCODE " +
                //   "from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id join pr_emp_adv_loans_adjustments_bef_monthend " +
                //   "adj on adj.loan_sl_no = l.loan_sl_no join pr_interface_vochers_codes b on b.vocher_name = lm.loan_description " +
                //   "where adj.fm = '" + str1 + "' and loan_description not like'%PF%' group by loan_description,fas_gl_code2," +
                //   "fas_gl_code1,vocher_name,vocher_type)as ammount";


                pfloans = "" +
                   //   "select 'Max Pension' as DescriptionofGLHead,'' as FASGLCODE,((select sum(dd_provident_fund) as TotalDeductions from pr_emp_payslip where fm = '" + str1 + "' and spl_type in('Regular', 'Adhoc')) - (select sum(bank_share) as TotalDeductions from pr_ob_share where fm = '" + str1 + "' ) ) as TotalDeductions " +
                   //"union all " +
                   "  select 'VPF' as DescriptionofGLHead,'' as FASGLCODE,(select sum(dd_amount) from pr_emp_payslip_deductions " +
                    "where dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip " +
                    "where fm =  '" + str1 + "' and spl_type in('Regular', 'Adhoc') ))as TotalDeductions " +
                    //                    " union all " +
                    //"select 'Total Recoveries - Club Subscription' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2, '/', b.fas_gl_code1) = '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode " +
                    //"from pr_interface_vochers_codes b where vocher_name = 'Club Subscription'),sum(dd_club_subscription) " +
                    //"from pr_emp_payslip p where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc')   " +

                    "union all " +

                 "select 'Provident Fund' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1) " +
                 "from pr_interface_vochers_codes where vocher_name = 'Provident Fund') as FASGLCODE,(select sum(dd_provident_fund) as TotalDeductions " +
                 "from pr_emp_payslip p where p.fm =  '" + str1 + "'  and p.spl_type in('Regular', 'Adhoc'))  " +
                 "union all  select 'NPS' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1) from pr_interface_vochers_codes " +
                 "where vocher_name = 'NPS') as FASGLCODE,sum(NPS)+ (select sum(amount) from pr_emp_adhoc_deduction_field " +
                 "where fm = '" + str1 + "'  and m_id = 586 ) as TotalDeductions from pr_emp_payslip p where p.fm = '" + str1 + "'  " +
                 "and p.spl_type in('Regular', 'Adhoc')";

                //                   "union all " +
                //                   " select  case when vocher_type = 'PFLoanInt' then concat('Interst on - ',vocher_name) else concat('Principle on - ', vocher_name) " +
                //" end as DescriptionofGLHead,'' as FASGLCODE,case when vocher_type = 'PFLoanInt' then sum(interest_paid_amount) else sum(adj.principal_paid_amount) end " +
                // " as TotalDeductions from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id " +
                // " join pr_emp_adv_loans_adjustments_bef_monthend adj on adj.loan_sl_no = l.loan_sl_no join pr_interface_vochers_codes b " +
                //"on b.vocher_name =lm.loan_description where adj.fm = '" + str1 + "'   and lm.id in (16,17,18,19,20,21,26,27) " +
                //"group by loan_description,fas_gl_code2,fas_gl_code1,vocher_name, vocher_type " +


                string pfloanstot = " select sum(ammount.TotalDeductions) as Total from(" +
  //"select 'Max Pension' as DescriptionofGLHead,'' as FASGLCODE,sum(pension_open) as TotalDeductions from pr_ob_share where fm='" + str1 + "' " +
  "  select 'VPF' as DescriptionofGLHead,'' as FASGLCODE,(select sum(dd_amount) from pr_emp_payslip_deductions " +
                    "where dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip " +
                    "where fm =  '" + str1 + "' and spl_type in('Regular', 'Adhoc'))) as TotalDeductions " +

                //                   " union all " +
                //"select 'Total Recoveries - Club Subscription' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2, '/', b.fas_gl_code1) = '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode " +
                //"from pr_interface_vochers_codes b where vocher_name = 'Club Subscription'),sum(dd_club_subscription) " +
                //"from pr_emp_payslip p where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc')   " +

                "union all select 'Provident Fund' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1)  " +
                "from pr_interface_vochers_codes where vocher_name = 'Provident Fund') as FASGLCODE,(select sum(dd_provident_fund) " +
                "as TotalDeductions from pr_emp_payslip p where " + q1 + "  ) union all " +
                "select 'NPS' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1) from pr_interface_vochers_codes " +
                "where vocher_name = 'NPS') as FASGLCODE,sum(NPS)+ (select sum(amount) from pr_emp_adhoc_deduction_field " +
                "where fm = '" + str1 + "'  and m_id = 586 ) as TotalDeductions from pr_emp_payslip p where " +
                "p.fm =  '" + str1 + "'  and p.spl_type in('Regular', 'Adhoc')   ) as ammount ";
                //"union all " +
                //"select case when vocher_type = 'PFLoanInt' then concat('Interst on - ',vocher_name) else concat('Principle on - ', vocher_name) end as DescriptionofGLHead, " +
                //"'' as FASGLCODE,case when vocher_type = 'PFLoanInt' then sum(interest_paid_amount) else sum(adj.principal_paid_amount) end as TotalDeductions " +
                //"from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id " +
                //"join pr_emp_adv_loans_adjustments_bef_monthend adj on adj.loan_sl_no = l.loan_sl_no join pr_interface_vochers_codes b on b.vocher_name = lm.loan_description " +
                //"where adj.fm = '" + str1 + "' and lm.id in (16, 17, 18, 19, 20, 21, 26, 27) group by loan_description,fas_gl_code2,fas_gl_code1,vocher_name, vocher_type)  as ammount ";

                dt2 = await _sha.Get_Table_FromQry(deductionsdata);
                DataTable dtded = await _sha.Get_Table_FromQry(dedtotal);
                //dt3 = await _sha.Get_Table_FromQry(allloans);
                //DataTable dtLoan = await _sha.Get_Table_FromQry(loantot);
                dt4 = await _sha.Get_Table_FromQry(pfloans);
                DataTable dtPfLoan = await _sha.Get_Table_FromQry(pfloanstot);
                string dtotal = dtded.Rows[0]["Total"].ToString();
                if (dtotal == "")
                {
                    dtotal = "0";
                }
                decimal Dcdtotal = Convert.ToDecimal(dtotal.ToString()) + 0.00M;
                decimal DPTdtotal = Convert.ToDecimal(String.Format("{0:0.00}", Dcdtotal));
                string NwDcdtotal = String.Format("{0:n}", DPTdtotal);

                //string Ltotal = dtLoan.Rows[0]["Total"].ToString();
                string Ptotal = dtPfLoan.Rows[0]["Total"].ToString();
                //if (Ptotal == "")
                //{
                //    Ptotal = "0";
                //}
                //decimal DcPtotal = Convert.ToDecimal(Ptotal.ToString()) + 0.00M;
                //decimal DPTPtotal = Convert.ToDecimal(String.Format("{0:0.00}", DcPtotal));
                //string NwPtotal = String.Format("{0:n}", DPTPtotal);
                double Ltot = 0;
                double dtot = 0;
                double Ptot = 0;
                //deductions
                if (dt2.Rows.Count > 0 || dt4.Rows.Count > 0)
                {
                    if (dtotal != "")
                    {
                        dtot = Convert.ToDouble(dtotal);
                    }
                    //if (Ltotal != "")
                    //{
                    //    Ltot = Convert.ToDouble(dtLoan.Rows[0]["Total"]);
                    //}
                    if (Ptotal != "")
                    {
                        Ptot = Convert.ToDouble(dtPfLoan.Rows[0]["Total"]);
                    }
                    double grandtotfcredits = dtot + Ptot + Ltot;

                    decimal Dcgrandtotfcredits = Convert.ToDecimal(grandtotfcredits.ToString()) + 0.00M;
                    decimal DPTgrandtotfcredits = Convert.ToDecimal(String.Format("{0:0.00}", Dcgrandtotfcredits));
                    string Nwgrandtotfcredits = String.Format("{0:n}", DPTgrandtotfcredits);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColHeaderValueOnly11(0, "PAYROLL SYSTEM ")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColHeaderValueOnly11(0, "DETAILS OF VOCHERS PREPARED IN RESPECT OF SALARIES")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                    + ReportColHeaderValueOnly11(0, "DEDUCTION TYPE : CREDIT")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                  + ReportColFooter(0, "FOR THE MONTH OF ", month)
                    };
                    lst.Add(crm);
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        DescriptionofGLHead = dr2["DescriptionofGLHead"].ToString();
                        string FASGLCODE = dr2["FASGLCODE"].ToString();
                        string TotalDeductions = dr2["TotalDeductions"].ToString();
                        if (TotalDeductions == "")
                        {
                            TotalDeductions = "0";
                        }
                        decimal DTotalDeductions = Convert.ToDecimal(TotalDeductions.ToString()) + 0.00M;
                        decimal DPTTotalDeductions = Convert.ToDecimal(String.Format("{0:0.00}", DTotalDeductions));
                        string NwTotalDeductions = String.Format("{0:n}", DPTTotalDeductions);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            // SlNo= SlNo++.ToString(),
                            SlNo = dr2["DescriptionofGLHead"].ToString(),
                            column2 = dr2["FASGLCODE"].ToString(),
                            column3 = NwTotalDeductions.ToString(),

                        };
                        lst.Add(crm);

                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterAlign(230, "Total ", NwDcdtotal)
                    };
                    lst.Add(crm);

                    //all loans
                    //  crm = new CommonReportModel
                    //  {
                    //      RowId = RowCnt++,
                    //      HRF = "F",
                    //      SlNo = "<span style='color:#eef8fd'>^</span>"
                    //          + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                    //  };
                    //  lst.Add(crm);

                    //  crm = new CommonReportModel
                    //  {
                    //      RowId = RowCnt++,
                    //      HRF = "F",
                    //      SlNo = "<span style='color:#eef8fd'>^</span>"
                    //    + ReportColHeaderValueOnly11(0, "PAYROLL SYSTEM  ")
                    //  };
                    //  lst.Add(crm);

                    //  crm = new CommonReportModel
                    //  {
                    //      RowId = RowCnt++,
                    //      HRF = "F",
                    //      SlNo = "<span style='color:#eef8fd'>^</span>"
                    //   + ReportColHeaderValueOnly11(0, "DETAILS OF VOCHERS PREPARED IN RESPECT OF SALARIES")
                    //  };
                    //  lst.Add(crm);

                    //  crm = new CommonReportModel
                    //  {
                    //      RowId = RowCnt++,
                    //      HRF = "F",
                    //      SlNo = "<span style='color:#eef8fd'>^</span>"
                    //  + ReportColHeaderValueOnly11(0, "DEDUCTION TYPE : CREDIT")
                    //  };
                    //  lst.Add(crm);

                    //  crm = new CommonReportModel
                    //  {
                    //      RowId = RowCnt++,
                    //      HRF = "F",
                    //      SlNo = "<span style='color:#eef8fd'>^</span>"
                    //+ ReportColFooter(0, "FOR THE MONTH OF ", month)
                    //  };
                    //  lst.Add(crm);

                    //foreach (DataRow dr3 in dt3.Rows)
                    //{
                    //    DescriptionofGLHead = dr3["DescriptionofGLHead"].ToString();
                    //    string FASGLCODE = dr3["FASGLCODE"].ToString();
                    //    string TotalDeductions = dr3["TotalDeductions"].ToString();
                    //    crm = new CommonReportModel
                    //    {
                    //        RowId = RowCnt++,
                    //        HRF = "R",
                    //        // SlNo= SlNo++.ToString(),
                    //        SlNo = dr3["DescriptionofGLHead"].ToString(),
                    //        column2 = dr3["FASGLCODE"].ToString(),
                    //        column3 = dr3["TotalDeductions"].ToString(),

                    //    };
                    //    lst.Add(crm);

                    //}
                    //crm = new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "F",
                    //    SlNo = "<span style='color:#eef8fd'>^</span>"
                    // + ReportColFooter(230, "Total ", Ltotal)
                    //};
                    //lst.Add(crm);


                    //pf loans
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColHeaderValueOnly11(0, "PAYROLL SYSTEM  ")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColHeaderValueOnly11(0, "DETAILS OF VOCHERS PREPARED IN RESPECT OF SALARIES")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                    + ReportColHeaderValueOnly11(0, "DEDUCTION TYPE : CREDIT")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                  + ReportColFooter(0, "FOR THE MONTH OF ", month)
                    };
                    lst.Add(crm);

                    foreach (DataRow dr4 in dt4.Rows)
                    {
                        DescriptionofGLHead = dr4["DescriptionofGLHead"].ToString();
                        string FASGLCODE = dr4["FASGLCODE"].ToString();
                        string TotalDeductions = dr4["TotalDeductions"].ToString();
                        if (TotalDeductions == "")
                        {
                            TotalDeductions = "0";
                        }
                        decimal DTotalDeductions = Convert.ToDecimal(TotalDeductions.ToString()) + 0.00M;
                        decimal DPTTotalDeductions = Convert.ToDecimal(String.Format("{0:0.00}", DTotalDeductions));
                        string Nw4TotalDeductions = String.Format("{0:n}", DPTTotalDeductions);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            // SlNo= SlNo++.ToString(),
                            SlNo = dr4["DescriptionofGLHead"].ToString(),
                            column2 = dr4["FASGLCODE"].ToString(),
                            column3 = Nw4TotalDeductions.ToString(),

                        };
                        lst.Add(crm);

                    }

                    if (Ptotal == "")
                    {
                        Ptotal = "0";
                    }
                    decimal DcPtotal = Convert.ToDecimal(Ptotal.ToString()) + 0.00M;
                    decimal DPTPtotal = Convert.ToDecimal(String.Format("{0:0.00}", DcPtotal));
                    string NwPtotal = String.Format("{0:n}", DPTPtotal);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColFooterAlign(230, "Total ", NwPtotal)
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                  + ReportColFooterAlign(230, "Grand Total ", Nwgrandtotfcredits.ToString())
                    };
                    lst.Add(crm);
                }
                // return await _sha.Get_Table_FromQry(query);
            }
            else if (All != null)
            {

                //headofcdata = "select 'Total Recoveries - Head Office' as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, " +
                //    "sum(p.deductions_amount ) as TotalDeductions from pr_emp_payslip p join pr_interface_vochers_codes b on b.vocher_name = p.branch " +
                //    "where  " + q1 + " and vocher_name in ('OtherBranch', 'HO Bkg-Br')  " +
                //    "group by   fas_gl_code2,fas_gl_code1 order by fas_gl_code1 ";
                headofcdata = "select 'Total Recoveries - Head Office' as DescriptionofGLHead,'' as FASGLCODE,case when sum(TotalDeductions) is null then 0 else sum(TotalDeductions) end  as TotalDeductions from  (select 'Total Recoveries - Head Office' as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as " +
" FASGLCODE, sum(p.deductions_amount) as TotalDeductions from pr_emp_payslip p join pr_interface_vochers_codes b on " +
" b.vocher_name = p.branch where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc') and vocher_name in ('OtherBranch', 'HO Bkg-Br')" +
" group by fas_gl_code2,fas_gl_code1 " +
" union all " +
"select 'Total Recoveries - Head Office' as DescriptionofGLHead,'' as FASGLCODE, ((select case when sum(dd_provident_fund) is null then 0 else sum(dd_provident_fund) end as TotalDeductions from pr_emp_payslip o " +
"join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and spl_type = 'Adhoc' and p.branch in( 43,44) )" +
"+(select case when sum(NPS) is null then 0 else sum(NPS) end as TotalDeductions from pr_emp_payslip o join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and spl_type = 'Adhoc' and p.branch in(43, 44) )+" +
"((select case when sum(bank_share) is null then 0 else sum(bank_share) end  from pr_ob_share o " +
"join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and bank_share!= 0 and p.branch in( 43,44))+" +
"(select case when sum(nps_bank_share) is null then 0 else sum(nps_bank_share) end  from pr_ob_share o " +
"join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and nps_bank_share!= 0 and p.branch in( 43,44))+" +
"( select case when sum(amount) is null then 0 else sum(amount) end from pr_emp_adhoc_deduction_field o join employees p on o.emp_code = p.empid " +
" where fm = '" + str1 + "' and p.branch in(43, 44) and m_id = 586 ))) as TotalDeductions  " +
"union all " +
"select 'Total Recoveries - Head Office' as DescriptionofGLHead,'' as FASGLCODE,case when sum(pension_open) is null then 0 else sum(pension_open) end as TotalDeductions from pr_ob_share o " +
"join employees p on o.emp_code = p.empid where o.fm = '" + str1 + "' and p.branch in( 43,44) ) as x ";

                //                branchdata = "select DescriptionofGLHead,FASGLCODE,sum(TotalDeductions) as deductions,vocher from (select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead, "+
                // " concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, sum(p.deductions_amount) as TotalDeductions "+
                //" ,b.vocher_type as vocher from pr_emp_payslip p join pr_interface_vochers_codes b on b.vocher_name = p.branch " +
                //"where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc') and b.vocher_type = 'branch' and b.vocher_name != 'HO Bkg-Br' " +
                //"group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                //"union all " +
                //"select concat('Total Recoveries - ', b.name) as DescriptionofGLHead,concat(d.fas_gl_code2, '/', d.fas_gl_code1) as FASGLCODE, " +
                //"(sum(pension_open) + sum(bank_share)) as pension,d.vocher_type as vocher from pr_ob_share o " +
                //"join employees p on o.emp_code = p.empid join branches b on b.id = p.branch join pr_interface_vochers_codes d on d.vocher_name = b.name "+
                //  "where o.fm = '" + str1 + "' and branch!= 43 group by b.name,vocher_name,d.vocher_type, fas_gl_code2,fas_gl_code1 " +
                //  "union all " +
                //  "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE," +
                //  " sum(p.dd_provident_fund) as TotalDeductions,b.vocher_type as vocher from pr_emp_payslip p join pr_interface_vochers_codes b on b.vocher_name = p.branch " +
                //  "where p.fm = '" + str1 + "' and p.spl_type in('Adhoc') and b.vocher_type = 'branch' and b.vocher_name != 'HO Bkg-Br' group by vocher_name,b.vocher_type," +
                //  "fas_gl_code2,fas_gl_code1) as aa group by DescriptionofGLHead,FASGLCODE,vocher";

                branchdata = "select DescriptionofGLHead,FASGLCODE,sum(TotalDeductions) as TotalDeductions,type,FASGORDER from (select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE," +
                    "sum(p.deductions_amount) as TotalDeductions,b.vocher_type as type,b.fas_gl_code1 as FASGORDER from pr_emp_payslip p join pr_interface_vochers_codes b on b.vocher_name = p.branch where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc') and b.vocher_type = 'branch' " +
                    "and b.vocher_name != 'HO Bkg-Br' group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    "union all " +
                    "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE,(sum(NPS)+sum(dd_provident_fund))  as TotalDeductions,b.vocher_type as type ,b.fas_gl_code1 as FASGORDER from pr_emp_payslip o join pr_interface_vochers_codes b on b.vocher_name = o.branch " +
                    "join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and spl_type = 'Adhoc' and p.branch not in( 43,44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    "union all " +
                    "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE,(sum(bank_share)+sum(nps_bank_share)) as TotalDeductions,b.vocher_type as type,b.fas_gl_code1 as FASGORDER from pr_ob_share o " +
                    "join employees p on o.emp_code = p.empid join branches bb on bb.id = p.branch join pr_interface_vochers_codes b on b.vocher_name = bb.name " +
                    " where fm = '" + str1 + "' and bank_share!= 0 and p.branch not in(43,44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    " union all  select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead," +
                    "concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE,(sum(amount))as TotalDeductions," +
                    "b.vocher_type as type ,b.fas_gl_code1 as FASGORDER from pr_emp_adhoc_deduction_field o " +
                    "join employees p on o.emp_code = p.empid " +
                    "join Branches br on br.id = p.Branch join pr_interface_vochers_codes b " +
                    "on b.vocher_name = br.Name where o.fm = '" + str1 + "'   and p.branch not in(43, 44) and m_id = 586  group by vocher_name, b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    //"union all  select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead," +
                    //"concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE,(sum(amount)) as TotalDeductions," +
                    //"b.vocher_type as type ,b.fas_gl_code1 as FASGORDER from pr_emp_adhoc_contribution_field o " +
                    //"join employees p on o.emp_code = p.empid join Branches br on br.id = p.Branch " +
                    //"join pr_interface_vochers_codes b on b.vocher_name = br.Name " +
                    //"where o.fm = '" + str1 + "'   and p.branch not in(43, 44)and m_id = 42 AND active=1 group by vocher_name," +
                    //"b.vocher_type, fas_gl_code2,fas_gl_code1" +
                    " union all " +
                    " select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE," +
                    " sum(pension_open) as TotalDeductions,b.vocher_type as type,b.fas_gl_code1 as FASGORDER from pr_ob_share o join employees p on o.emp_code = p.empid join branches bb on bb.id = p.branch " +
                    " join pr_interface_vochers_codes b on b.vocher_name = bb.name where o.fm = '" + str1 + "'  and p.branch not in( 43,44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    " " +
                    " ) as x group by DescriptionofGLHead,FASGLCODE,type,FASGORDER order by FASGORDER ";

                //branchdata = "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, " +
                //   "sum(p.deductions_amount ) as TotalDeductions,b.vocher_type from pr_emp_payslip p join pr_interface_vochers_codes b " +
                //   "on b.vocher_name = p.branch where  " + q1 + " and b.vocher_type = 'branch' and b.vocher_name !='HO Bkg-Br'  group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 order by fas_gl_code1";

                //string branchdataotherfullclearingloans = "select sum(principal_paid_amount) as principal_paid_amount from pr_emp_adv_loans_bef_monthend l join pr_emp_adv_loans_adjustments_bef_monthend adj on adj.loan_sl_no = l.loan_sl_no  where installments_amount < principal_paid_amount; ";
                //                string branchdataotherfullclearingloans = "select sum(principal_paid_amount) as principal_paid_amount from pr_emp_adv_loans_bef_monthend l join pr_emp_adv_loans_adjustments_bef_monthend adj  on adj.loan_sl_no = l.loan_sl_no  " +
                //                    "where installments_amount < (principal_paid_amount)and loan_type_mid not in (16, 17, 18, 19, 20, 21, 26, 27)  " +
                //                    "and adj.fm = '" + str1 + "' ";

                //                string branchdataotherprincipalinstpaidamt = "select sum(principal_paid_amount)+sum(interest_paid_amount) as pricipl_inst_paid_amount from  " +
                //" pr_emp_adv_loans_bef_monthend l join pr_emp_adv_loans_adjustments_bef_monthend  adj  on adj.loan_sl_no = l.loan_sl_no " +
                // " join pr_Emp_payslip pslip on pslip.emp_code = l.emp_code  and pslip.fm = '" + str1 + "'  join pr_Emp_payslip_deductions ded on ded.payslip_mid = pslip.id and l.loan_type_mid = ded.dd_mid and dd_type = 'Loan' " +
                // " where installments_amount>= (principal_paid_amount + interest_paid_amount) and loan_type_mid in (16, 17, 18, 19, 20, 21, 26, 27) " +
                //" and principal_balance_amount = 0  and adj.fm = '" + str1 + "' group by dd_amount having(sum(principal_paid_amount)+sum(interest_paid_amount))!= dd_amount ";


                //string branchdataotherfullclearingloans = "select SUM(adj.principal_paid_amount+adj.interest_paid_amount) as principal_paid_amount from  pr_emp_adv_loans l " +
                //    "join pr_emp_adv_loans_adjustments adj on adj.emp_adv_loans_mid = l.id join pr_emp_adv_loans_adjustments adj1 on adj1.emp_adv_loans_mid = l.id " +
                //    "where adj.fm = '" + str1 + "'  and adj.principal_balance_amount = 0  and adj1.fm = '" + str2 + "' and adj.principal_paid_amount > adj1.principal_paid_amount"; 

                //string branchdataotherprincipalinstpaidamt = " select SUM(adj.principal_paid_amount+adj.interest_paid_amount) as pricipl_inst_paid_amount  from  pr_emp_adv_loans l " +
                //    "join pr_emp_adv_loans_adjustments adj on adj.emp_adv_loans_mid = l.id join pr_emp_adv_loans_adjustments adj1 on adj1.emp_adv_loans_mid = l.id " +
                //    "where adj.fm = '" + str1 + "'  and adj.principal_balance_amount != 0  and adj1.fm = '" + str2 + "' and adj.principal_paid_amount > adj1.principal_paid_amount " +
                //    "and adj.installments_amount > adj1.installments_amount and completed_installment> 1 AND adj1.installments_amount != 0 ";

                //string totamtOfBranches = "select sum(ammount.TotalDeductions) as TOTAL from (select concat('Total Recoveries - ', b.vocher_name) " +
                //    "as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, " +
                //    "sum(p.deductions_amount ) as TotalDeductions,b.vocher_type from pr_emp_payslip p " +
                //    "join pr_interface_vochers_codes b on b.vocher_name = p.branch where   " + q1 + "  and b.vocher_type = 'branch' and b.vocher_name !='HO Bkg-Br' " +
                //    "group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1) as ammount";
                string totamtOfBranches = "select sum(TOTAL) as TOTAL from (select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE,sum(p.deductions_amount) as TOTAL ,b.vocher_type from pr_emp_payslip p join pr_interface_vochers_codes b " +
                    " on b.vocher_name = p.branch where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc') and b.vocher_type = 'branch' and b.vocher_name != 'HO Bkg-Br' group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    " union all  " +
                    "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, (sum(NPS)+sum(dd_provident_fund)) as TOTAL,b.vocher_type from pr_emp_payslip o join pr_interface_vochers_codes b on b.vocher_name = o.branch " +
                    "join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and spl_type = 'Adhoc' and p.branch not in( 43,44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    "union all " +
                    "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, (sum(bank_share)+sum(nps_bank_share)) as TOTAL,b.vocher_type from pr_ob_share o " +
                    "join employees p on o.emp_code = p.empid join branches bb on bb.id = p.branch join pr_interface_vochers_codes b on b.vocher_name = bb.name where fm = '" + str1 + "' and bank_share!= 0 and p.branch not in( 43,44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    "union all  select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead," +
                    "concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, (sum(amount)) as TOTAL," +
                    "b.vocher_type from pr_emp_adhoc_deduction_field o join employees p on o.emp_code = p.empid " +
                    "join branches bb on bb.id = p.branch join pr_interface_vochers_codes b on " +
                    "b.vocher_name = bb.name where fm = '" + str1 + "' and m_id = 586  and p.branch not " +
                    "in(43, 44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    //"union all  select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead," +
                    //"concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, (sum(amount)) as TOTAL," +
                    //"b.vocher_type from pr_emp_adhoc_contribution_field o join employees p on o.emp_code = p.empid " +
                    //"join branches bb on bb.id = p.branch join pr_interface_vochers_codes b " +
                    //"on b.vocher_name = bb.name where fm = '" + str1 + "' and m_id = 42 AND active=1 and p.branch not in(43, 44) group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1" +
                    "  union all select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,concat(b.fas_gl_code2, '/', b.fas_gl_code1) as FASGLCODE, " +
                    "sum(pension_open) as TOTAL,b.vocher_type from pr_ob_share o join employees p on o.emp_code = p.empid join branches bb on bb.id = p.branch " +
                    "join pr_interface_vochers_codes b on b.vocher_name = bb.name where o.fm = '" + str1 + "'  and p.branch not in( 43,44) " +
                    "group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
                    ") as x ";


                dt = await _sha.Get_Table_FromQry(headofcdata);
                dt1 = await _sha.Get_Table_FromQry(branchdata);
                DataTable dtBrtot = await _sha.Get_Table_FromQry(totamtOfBranches);
                //DataTable dtBrtot1 = await _sha.Get_Table_FromQry(branchdataotherfullclearingloans);
                //DataTable dtBrtot2 = await _sha.Get_Table_FromQry(branchdataotherprincipalinstpaidamt);

                //string total = dtBrtot.Rows[0]["Total"].ToString();
                //string total1 = dtBrtot1.Rows[0]["principal_paid_amount"].ToString();
                double total1 = 0;
                double total = 0;
                double total2 = 0;
                //string principal_paid_amount = dtBrtot1.Rows[0]["principal_paid_amount"].ToString();
                //string pricipl_inst_paid_amount = dtBrtot2.Rows[0]["pricipl_inst_paid_amount"].ToString();
                string Total = dtBrtot.Rows[0]["Total"].ToString();

                if (dtBrtot.Rows.Count > 0 && Total != "")
                {
                    total = Convert.ToDouble(dtBrtot.Rows[0]["Total"].ToString());
                }
                else
                {
                    total = 0;
                }
                //if (dtBrtot1.Rows.Count > 0 && principal_paid_amount != "")
                //{
                //    total1 = Convert.ToDouble(principal_paid_amount);
                //}
                //else
                //{
                //    total1 = 0;
                //}

                //if (dtBrtot2.Rows.Count > 0)
                //{
                //    total2 = Convert.ToDouble(dtBrtot2.Rows[0]["pricipl_inst_paid_amount"].ToString());
                //}
                //else
                //{
                //    total2 = 0;
                //}
                //string total2 = dtBrtot2.Rows[0]["pricipl_inst_paid_amount"].ToString();
                double btotal = (total + total1 + total2);
                decimal Dcbtotal = Convert.ToDecimal(btotal.ToString()) + 0.00M;
                decimal DPTbtotal = Convert.ToDecimal(String.Format("{0:0.00}", Dcbtotal));
                string Nwbtotal = String.Format("{0:n}", DPTbtotal);
                if (dt.Rows.Count > 0 || dt1.Rows.Count > 0)
                {
                    double tot = btotal;
                    double grandtotdebit = Convert.ToDouble(dt.Rows[0]["TotalDeductions"]) + tot;
                    decimal Dcgrandtotdebit = Convert.ToDecimal(grandtotdebit.ToString()) + 0.00M;
                    decimal DCPTgrandtotdebit = Convert.ToDecimal(String.Format("{0:0.00}", Dcgrandtotdebit));
                    string Nwgrandtotdebit = String.Format("{0:n}", DCPTgrandtotdebit);
                    //HEAD OFFICE
                    foreach (DataRow dr in dt.Rows)
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                             + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                          + ReportColHeaderValueOnly11(0, "PAYROLL SYSTEM  ")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColHeaderValueOnly11(0, "DETAILS OF VOCHERS PREPARED IN RESPECT OF SALARIES")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColHeaderValueOnly11(0, "DEDUCTION TYPE : DEBIT")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColFooter(0, "FOR THE MONTH OF ", month)
                        };
                        lst.Add(crm);

                        DescriptionofGLHead = dr["DescriptionofGLHead"].ToString();
                        string FASGLCODE = dr["FASGLCODE"].ToString();
                        // ;
                        string TotalDeductions = dr["TotalDeductions"].ToString();
                        if (TotalDeductions == "")
                        {
                            TotalDeductions = "0";
                        }
                        decimal DTotalDeductions = Convert.ToDecimal(TotalDeductions.ToString()) + 0.00M;
                        decimal DPTTotalDeductions = Convert.ToDecimal(String.Format("{0:0.00}", DTotalDeductions));
                        string NwTotalDeductions = String.Format("{0:n}", DPTTotalDeductions);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            // SlNo= SlNo++.ToString(),
                            SlNo = dr["DescriptionofGLHead"].ToString(),
                            column2 = dr["FASGLCODE"].ToString(),
                            column3 = NwTotalDeductions.ToString(),

                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooterAlign(230, "Total", NwTotalDeductions.ToString())
                        };
                        lst.Add(crm);
                    }

                    //BRANCHES

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                   + ReportColHeaderValueOnly11(0, "PAYROLL SYSTEM  ")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColHeaderValueOnly11(0, "DETAILS OF VOCHERS PREPARED IN RESPECT OF SALARIES")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                    + ReportColHeaderValueOnly11(0, "DEDUCTION TYPE : DEBIT")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                  + ReportColFooter(0, "FOR THE MONTH OF ", month)
                    };
                    lst.Add(crm);
                    foreach (DataRow dr1 in dt1.Rows)
                    {
                        DescriptionofGLHead = dr1["DescriptionofGLHead"].ToString();
                        string FASGLCODE = dr1["FASGLCODE"].ToString();
                        string TotalDeductions = dr1["TotalDeductions"].ToString();
                        if (TotalDeductions == "")
                        {
                            TotalDeductions = "0";
                        }
                        decimal DTotalDeductions = Convert.ToDecimal(TotalDeductions.ToString()) + 0.00M;
                        decimal DPTTotalDeductions = Convert.ToDecimal(String.Format("{0:0.00}", DTotalDeductions));
                        string NwTotalDeductions = String.Format("{0:n}", DPTTotalDeductions);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            // SlNo= SlNo++.ToString(),
                            SlNo = dr1["DescriptionofGLHead"].ToString(),
                            column2 = dr1["FASGLCODE"].ToString(),
                            column3 = NwTotalDeductions.ToString(),

                        };
                        lst.Add(crm);

                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooterAlign(230, "Total ", Nwbtotal.ToString())
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                   + ReportColFooterAlign(230, "Grand Total ", Nwgrandtotdebit.ToString())
                    };
                    lst.Add(crm);

                }
                deductionsdata = " select concat('Total Recoveries - ', 'FEST ADVANCE') as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else '' end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid=123 and " + q1 + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1 " +
                   " union all select concat('Total Recoveries - ', 'NON PRIORITY PERSONAL LOAN') as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else '' end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid=(select id from pr_deduction_field_master where name ='NON PRIORITY PERSONAL LOAN') and " + q1 + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1  " +
                   "union all select concat('Total Recoveries - Adhoc -', e.name) as DescriptionofGLHead," +
                   "case when concat(b.fas_gl_code2,'/',b.fas_gl_code1)= '/' then null else '' end as  fasglcode, sum(amount) as TotalDeductions " +
                   "from pr_emp_adhoc_deduction_field f join pr_deduction_field_master e on f.m_id = e.id " +
                   "join pr_interface_vochers_codes b on b.vocher_name = e.name where b.vocher_type in " +
                   "('Deductions', 'LoanInt', 'PFLoanInt', 'Loans') and f.m_id != 123 and f.m_id != (select id from pr_deduction_field_master " +
                   "where name = 'NON PRIORITY PERSONAL LOAN') and f.fm = '" + str1 + "'  and e.name " +
                   "in ('Club Subscription', 'TELANGANA OFFICERS ASSN')and f.active = 1 group by name , fas_gl_code2,fas_gl_code1  " +
                    "union all select concat('Total Recoveries - ', d.dd_name) as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else '' end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid!=123 and d.dd_mid!=(select id from pr_deduction_field_master where name ='NON PRIORITY PERSONAL LOAN') and " + q1


+ " and d.dd_name != 'NPS'" + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1 " +
                   "union all select 'Total Recoveries - Income Tax' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2,'/',b.fas_gl_code1)= '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode from pr_interface_vochers_codes b where vocher_name = 'Income Tax') ,sum(dd_income_tax) from pr_emp_payslip p where " + q1 + " " +
                   "union all select 'Total Recoveries - Prof.Tax' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2,'/',b.fas_gl_code1)= '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode from pr_interface_vochers_codes b where vocher_name = 'Prof. Tax'),sum(dd_prof_tax) from pr_emp_payslip p where " + q1 + " " +
                   "  union all " +
  "select 'Total Recoveries - Max Pension' as DescriptionofGLHead,'' as num,((select sum(pension_open) as TotalDeductions from pr_ob_share " +
  "where fm = '" + str1 + "'))  as TotalDeductions " +
  " union all select 'Total Recoveries - PF Contribution' as DescriptionofGLHead,'' as num1, " +
  " ((select case when  sum(dd_provident_fund)   is null then 0 else sum(dd_provident_fund) end as TotalDeductions   from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Adhoc' ) " +
  " + (select sum(bank_share) as TotalDeductions  from pr_ob_share where fm = '" + str1 + "'  and bank_share!= 0) )  as TotalDeductions " +
  "union all select 'Total Recoveries - NPS Contribution' as DescriptionofGLHead,'' as num1," +
  " (( select case when sum(NPS) is null then 0 else sum(NPS) end as TotalDeductions from pr_emp_payslip where fm = '" + str1 + "' " +
  "and spl_type = 'Adhoc' ) + (select sum(NPS_bank_share)+ (select sum(amount) as TotalDeductions from pr_emp_adhoc_contribution_field where fm = '" + str1 + "' and m_id = 42 ) as TotalDeductions from pr_ob_share where fm = '" + str1 + "' " +
  "and NPS_bank_share!= 0) ) as TotalDeductions ";


                string dedtotal = "  select sum(ammount.TotalDeductions) as Total from (select concat('Total Recoveries - ', 'FEST ADVANCE') as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else '' end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid=123 and " + q1 + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1 " +
                   " union all select concat('Total Recoveries - ', 'NON PRIORITY PERSONAL LOAN') as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else '' end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid=(select id from pr_deduction_field_master where name ='NON PRIORITY PERSONAL LOAN') and " + q1 + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1 " +
                    "union all select concat('Total Recoveries - ', d.dd_name) as DescriptionofGLHead,case when " +
                   "concat(b.fas_gl_code2,'/',b.fas_gl_code1)='/' then null else concat(b.fas_gl_code2,'/',b.fas_gl_code1) end  as fasglcode,sum(dd_amount) as TotalDeductions " +
                   "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                   "where b.vocher_type in ('Deductions','LoanInt','PFLoanInt','Loans') and d.dd_mid!=123 and d.dd_mid!=(select id from pr_deduction_field_master where name ='NON PRIORITY PERSONAL LOAN') and " + q1 + " and d.dd_name != 'NPS'" + " " +
                   "group by dd_name , fas_gl_code2,fas_gl_code1 " +
                   "union all select 'Total Recoveries - Income Tax' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2,'/',b.fas_gl_code1)= '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode from pr_interface_vochers_codes b where vocher_name = 'Income Tax') ,sum(dd_income_tax) from pr_emp_payslip p where " + q1 + " " +
                   "union all select 'Total Recoveries - Prof.Tax' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2,'/',b.fas_gl_code1)= '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode from pr_interface_vochers_codes b where vocher_name = 'Prof. Tax'),sum(dd_prof_tax) from pr_emp_payslip p where " + q1 + " " +
                   " 	union all " +
  "select 'Total Recoveries -Max Pension' as DescriptionofGLHead,'' as num,((select sum(pension_open) as TotalDeductions from pr_ob_share " +
  " where fm = '" + str1 + "')) " +
  "union all select 'Total Recoveries - PF Contribution' as DescriptionofGLHead,'' as num1, " +
  " ((select case when  sum(dd_provident_fund)   is null then 0 else sum(dd_provident_fund) end as TotalDeductions   from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Adhoc' ) " +
  " + (select sum(bank_share) as TotalDeductions  from pr_ob_share where fm = '" + str1 + "'  and bank_share!= 0) )  as TotalDeductions  " +
  "union all select 'Total Recoveries - NPS Contribution' as DescriptionofGLHead,'' as num1, (( select case when sum(NPS) is null then 0 else sum(NPS) end as " +
  "TotalDeductions from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Adhoc' ) + (select sum(NPS_bank_share) as TotalDeductions " +
  "from pr_ob_share where fm = '" + str1 + "' and NPS_bank_share!= 0) +(select case when sum(amount) " +
  "is null then 0 else sum(amount) end as TotalDeductions from pr_emp_adhoc_deduction_field " +
  "where fm='" + str1 + "' and active=1 and m_id in (85,145))+(select case when sum(amount) is null then 0 " +
  "else sum(amount) end as TotalDeductions from pr_emp_adhoc_contribution_field " +
  "where fm = '" + str1 + "'  and m_id = 42  )))   as ammount ";



                //allloans = "select case when vocher_type='LoanInt' then concat('Interst on - ',vocher_name) else " +
                //   "concat('Principle on - ',vocher_name) end as DescriptionofGLHead,case when vocher_type = 'LoanInt' " +
                //   "then sum(interest_paid_amount) else sum(adj.principal_paid_amount) end as TotalDeductions,b.fas_gl_code2 as FASGLCODE " +
                //   "from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id join pr_emp_adv_loans_adjustments_bef_monthend " +
                //   "adj on adj.loan_sl_no = l.loan_sl_no join pr_interface_vochers_codes b on b.vocher_name = lm.loan_description " +
                //   "where adj.fm = '" + str1 + "' and loan_description not like'%PF%' group by loan_description,fas_gl_code2," +
                //   "fas_gl_code1,vocher_name,vocher_type";
                //string loantot = "select sum(ammount.TotalDeductions) as Total from (select case when vocher_type='LoanInt' then concat('Interst on - ',vocher_name) else " +
                //   "concat('Principle on - ',vocher_name) end as DescriptionofGLHead,case when vocher_type = 'LoanInt' " +
                //   "then sum(interest_paid_amount) else sum(adj.principal_paid_amount) end as TotalDeductions,b.fas_gl_code2 as FASGLCODE " +
                //   "from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id join pr_emp_adv_loans_adjustments_bef_monthend " +
                //   "adj on adj.loan_sl_no = l.loan_sl_no join pr_interface_vochers_codes b on b.vocher_name = lm.loan_description " +
                //   "where adj.fm = '" + str1 + "' and loan_description not like'%PF%' group by loan_description,fas_gl_code2," +
                //   "fas_gl_code1,vocher_name,vocher_type)as ammount";


                //pfloans = "select 'Max Pension' as DescriptionofGLHead,'' as FASGLCODE,sum(pension_open) as TotalDeductions from pr_ob_share where fm='" + str1 + "' " +
                //   "union all " +
                //   "select 'Provident Fund' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1)  " +
                //   "from pr_interface_vochers_codes where vocher_name = 'Provident Fund') as FASGLCODE,sum(dd_provident_fund) " +
                //   "as TotalDeductions from pr_emp_payslip p where " + q1 + " " +
                //   "union all " +
                //   "select 'PF Contribution' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1)  " +
                //   "from pr_interface_vochers_codes where vocher_name = 'PF Contribution') as FASGLCODE,sum(bank_share) as TotalDeductions from pr_ob_share where fm = '" + str1 + "' " +
                pfloans = "" +
                   //   "select 'Max Pension' as DescriptionofGLHead,'' as FASGLCODE,((select sum(dd_provident_fund) as TotalDeductions from pr_emp_payslip where fm = '" + str1 + "' and spl_type in('Regular', 'Adhoc')) - (select sum(bank_share) as TotalDeductions from pr_ob_share where fm = '" + str1 + "' ) ) as TotalDeductions " +
                   //"union all " +
                   "  select 'VPF' as DescriptionofGLHead,'' as FASGLCODE,(select sum(dd_amount) from pr_emp_payslip_deductions " +
                    "where dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip " +
                    "where fm =  '" + str1 + "' and spl_type in('Regular', 'Adhoc') ))as TotalDeductions " +
                    //                    " union all " +
                    //"select 'Total Recoveries - Club Subscription' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2, '/', b.fas_gl_code1) = '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode " +
                    //"from pr_interface_vochers_codes b where vocher_name = 'Club Subscription'),sum(dd_club_subscription) " +
                    //"from pr_emp_payslip p where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc')   " +

                    "union all " +

                 "select 'Provident Fund' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1) " +
                 "from pr_interface_vochers_codes where vocher_name = 'Provident Fund') as FASGLCODE,(select sum(dd_provident_fund) as TotalDeductions " +
                 "from pr_emp_payslip p where p.fm =  '" + str1 + "'  and p.spl_type in('Regular', 'Adhoc'))  " +
                 "union all  select 'NPS' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1) from pr_interface_vochers_codes " +
                 "where vocher_name = 'NPS') as FASGLCODE,sum(NPS)+ (select sum(amount) from pr_emp_adhoc_deduction_field " +
                 "where fm = '" + str1 + "'  and m_id = 586 ) as TotalDeductions from pr_emp_payslip p where p.fm = '" + str1 + "'  " +
                 "and p.spl_type in('Regular', 'Adhoc')";
                //                "union all " +
                //                   " select  case when vocher_type = 'PFLoanInt' then concat('Interst on - ',vocher_name) else concat('Principle on - ', vocher_name) " +
                //" end as DescriptionofGLHead,'' as FASGLCODE,case when vocher_type = 'PFLoanInt' then sum(interest_paid_amount) else sum(adj.principal_paid_amount) end " +
                // " as TotalDeductions from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id " +
                // " join pr_emp_adv_loans_adjustments_bef_monthend adj on adj.loan_sl_no = l.loan_sl_no join pr_interface_vochers_codes b " +
                //"on b.vocher_name =lm.loan_description where adj.fm = '" + str1 + "'   and lm.id in (16,17,18,19,20,21,26,27) " +
                //"group by loan_description,fas_gl_code2,fas_gl_code1,vocher_name, vocher_type ";
                //"union all " +

                //"select  'PF Contribution' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1) " +
                //"from pr_interface_vochers_codes where vocher_name = 'PF Contribution') as FASGLCODE,( (select sum(dd_provident_fund) as TotalDeductions  from pr_emp_payslip " +
                //"where fm =  '" + str1 + "' and spl_type in('Regular', 'Adhoc')) -  (select sum(pension_open) as TotalDeductions   from pr_ob_share where fm =  '" + str1 + "' ) ) as TotalDeductions " +

                //"union all select  'PF Advance Interest' as DescriptionofGLHead, (select concat(fas_gl_code2, '/', fas_gl_code1) as FASGLCODE " +
                //   "from pr_interface_vochers_codes where vocher_name = 'PF Advance Interest'),sum(interest_paid_amount) as TotalDeductions " +
                //   "from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id join pr_emp_adv_loans_adjustments_bef_monthend adj " +
                //   "on adj.loan_sl_no = l.loan_sl_no  where adj.fm = '" + str1 + "' and lm.id in (16, 17, 18, 19, 20, 21, 26, 27) " +
                //   "union all select  'PF Advance Principle' as DescriptionofGLHead, (select concat(fas_gl_code2, '/', fas_gl_code1) as FASGLCODE  " +
                //   "from pr_interface_vochers_codes where vocher_name = 'PF Advance Principle'),sum(principal_paid_amount) as TotalDeductions " +
                //   "from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id join pr_emp_adv_loans_adjustments_bef_monthend adj on adj.loan_sl_no = l.loan_sl_no  where adj.fm = '" + str1 + "' and lm.id in (16, 17, 18, 19, 20, 21, 26, 27); ";

                string pfloanstot = " select sum(ammount.TotalDeductions) as Total from(" +
  //"select 'Max Pension' as DescriptionofGLHead,'' as FASGLCODE,sum(pension_open) as TotalDeductions from pr_ob_share where fm='" + str1 + "' " +
  "  select 'VPF' as DescriptionofGLHead,'' as FASGLCODE,(select sum(dd_amount) from pr_emp_payslip_deductions " +
                    "where dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip " +
                    "where fm =  '" + str1 + "' and spl_type in('Regular', 'Adhoc'))) as TotalDeductions " +

                //                   " union all " +
                //"select 'Total Recoveries - Club Subscription' as DescriptionofGLHead,(select case when concat(b.fas_gl_code2, '/', b.fas_gl_code1) = '/' then null else concat(b.fas_gl_code2, '/', b.fas_gl_code1) end as fasglcode " +
                //"from pr_interface_vochers_codes b where vocher_name = 'Club Subscription'),sum(dd_club_subscription) " +
                //"from pr_emp_payslip p where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc')   " +

                "union all select 'Provident Fund' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1)  " +
                "from pr_interface_vochers_codes where vocher_name = 'Provident Fund') as FASGLCODE,(select sum(dd_provident_fund) " +
                "as TotalDeductions from pr_emp_payslip p where " + q1 + "  ) union all " +
                "select 'NPS' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1) from pr_interface_vochers_codes " +
                "where vocher_name = 'NPS') as FASGLCODE,sum(NPS)+ (select sum(amount) from pr_emp_adhoc_deduction_field " +
                "where fm = '" + str1 + "'  and m_id = 586 ) as TotalDeductions from pr_emp_payslip p where " +
                "p.fm =  '" + str1 + "'  and p.spl_type in('Regular', 'Adhoc')   ) as ammount ";
                //               "union all " +
                //                   " select  case when vocher_type = 'PFLoanInt' then concat('Interst on - ',vocher_name) else concat('Principle on - ', vocher_name) " +
                //" end as DescriptionofGLHead,'' as FASGLCODE,case when vocher_type = 'PFLoanInt' then sum(interest_paid_amount) else sum(adj.principal_paid_amount) end " +
                // " as TotalDeductions from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id " +
                // " join pr_emp_adv_loans_adjustments_bef_monthend adj on adj.loan_sl_no = l.loan_sl_no join pr_interface_vochers_codes b " +
                //"on b.vocher_name =lm.loan_description where adj.fm = '" + str1 + "'   and lm.id in (16,17,18,19,20,21,26,27) " +
                //"group by loan_description,fas_gl_code2,fas_gl_code1,vocher_name, vocher_type ) as ammount ";
                //"union all " +
                //"select 'PF Contribution' as DescriptionofGLHead,(select concat(fas_gl_code2, '/', fas_gl_code1)  " +
                //"from pr_interface_vochers_codes where vocher_name = 'PF Contribution') as FASGLCODE,sum(bank_share) as TotalDeductions from pr_ob_share where fm = '" + str1 + "' " +
                //"union all " +
                //"union all select  'PF Advance Interest' as DescriptionofGLHead, (select concat(fas_gl_code2, '/', fas_gl_code1) as FASGLCODE " +
                //"from pr_interface_vochers_codes where vocher_name = 'PF Advance Interest'),sum(interest_paid_amount) as TotalDeductions " +
                //"from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id join pr_emp_adv_loans_adjustments_bef_monthend adj " +
                //"on adj.loan_sl_no = l.loan_sl_no  where adj.fm = '" + str1 + "' and lm.id in (16, 17, 18, 19, 20, 21, 26, 27) " +
                //"union all select  'PF Advance Principle' as DescriptionofGLHead, (select concat(fas_gl_code2, '/', fas_gl_code1) as FASGLCODE  " +
                //"from pr_interface_vochers_codes where vocher_name = 'PF Advance Principle'),sum(principal_paid_amount) as TotalDeductions " +
                //"from pr_loan_master lm join pr_emp_adv_loans_bef_monthend l on l.loan_type_mid = lm.id join pr_emp_adv_loans_adjustments_bef_monthend adj on adj.loan_sl_no = l.loan_sl_no  where adj.fm = '" + str1 + "' and lm.id in (16, 17, 18, 19, 20, 21, 26, 27)) as ammount; ";

                dt2 = await _sha.Get_Table_FromQry(deductionsdata);
                DataTable dtded = await _sha.Get_Table_FromQry(dedtotal);
                //dt3 = await _sha.Get_Table_FromQry(allloans);
                //DataTable dtLoan = await _sha.Get_Table_FromQry(loantot);
                dt4 = await _sha.Get_Table_FromQry(pfloans);
                DataTable dtPfLoan = await _sha.Get_Table_FromQry(pfloanstot);
                string dtotal = dtded.Rows[0]["Total"].ToString();
                if (dtotal == "")
                {
                    dtotal = "0";
                }
                decimal Dcdtotal = Convert.ToDecimal(dtotal.ToString()) + 0.00M;
                decimal DPTdtotal = Convert.ToDecimal(String.Format("{0:0.00}", Dcdtotal));
                string Nwdtotal = String.Format("{0:n}", DPTdtotal);

                //string Ltotal = dtLoan.Rows[0]["Total"].ToString();
                string Ptotal = dtPfLoan.Rows[0]["Total"].ToString();
                //if (Ptotal == "")
                // {
                // Ptotal = "0";
                //}
                double Ltot = 0;
                double dtot = 0;
                double Ptot = 0;

                //deductions
                if (dtotal != "" || Ptotal != "")
                {
                    //if (Ltotal != "")
                    //{
                    //    Ltot = Convert.ToDouble(dtLoan.Rows[0]["Total"]);
                    //}
                    if (dtotal != "")
                    {
                        dtot = Convert.ToDouble(dtotal);
                    }
                    if (Ptotal != "")
                    {
                        Ptot = Convert.ToDouble(dtPfLoan.Rows[0]["Total"]);
                    }
                    double grandtotfcredits = Convert.ToDouble(dtot + Ptot + Ltot);
                    decimal Dgrandtotfcredits = Convert.ToDecimal(grandtotfcredits.ToString()) + 0.00M;
                    decimal DPTgrandtotfcredits = Convert.ToDecimal(String.Format("{0:0.00}", Dgrandtotfcredits));
                    string Nwgrandtotfcredits = String.Format("{0:n}", DPTgrandtotfcredits);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColHeaderValueOnly11(0, "PAYROLL SYSTEM  ")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColHeaderValueOnly11(0, "DETAILS OF VOCHERS PREPARED IN RESPECT OF SALARIES")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                    + ReportColHeaderValueOnly11(0, "DEDUCTION TYPE : CREDIT")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                  + ReportColFooter(0, "FOR THE MONTH OF ", month)
                    };
                    lst.Add(crm);
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        DescriptionofGLHead = dr2["DescriptionofGLHead"].ToString();
                        string FASGLCODE = dr2["FASGLCODE"].ToString();
                        string TotalDeductions = dr2["TotalDeductions"].ToString();
                        if (TotalDeductions == "")
                        {
                            TotalDeductions = "0";
                        }
                        decimal DTotalDeductions = Convert.ToDecimal(TotalDeductions.ToString()) + 0.00M;
                        decimal DPTTotalDeductions = Convert.ToDecimal(String.Format("{0:0.00}", DTotalDeductions));
                        string NwTotalDeductions = String.Format("{0:n}", DPTTotalDeductions);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            // SlNo= SlNo++.ToString(),
                            SlNo = dr2["DescriptionofGLHead"].ToString(),
                            column2 = dr2["FASGLCODE"].ToString(),
                            column3 = NwTotalDeductions.ToString(),

                        };
                        lst.Add(crm);

                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterAlign(230, "Total ", Nwdtotal)
                    };
                    lst.Add(crm);

                    //all loans
                    //  crm = new CommonReportModel
                    //  {
                    //      RowId = RowCnt++,
                    //      HRF = "F",
                    //      SlNo = "<span style='color:#eef8fd'>^</span>"
                    //          + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                    //  };
                    //  lst.Add(crm);

                    //  crm = new CommonReportModel
                    //  {
                    //      RowId = RowCnt++,
                    //      HRF = "F",
                    //      SlNo = "<span style='color:#eef8fd'>^</span>"
                    //    + ReportColHeaderValueOnly11(0, "PAYROLL SYSTEM  ")
                    //  };
                    //  lst.Add(crm);

                    //  crm = new CommonReportModel
                    //  {
                    //      RowId = RowCnt++,
                    //      HRF = "F",
                    //      SlNo = "<span style='color:#eef8fd'>^</span>"
                    //   + ReportColHeaderValueOnly11(0, "DETAILS OF VOCHERS PREPARED IN RESPECT OF SALARIES")
                    //  };
                    //  lst.Add(crm);

                    //  crm = new CommonReportModel
                    //  {
                    //      RowId = RowCnt++,
                    //      HRF = "F",
                    //      SlNo = "<span style='color:#eef8fd'>^</span>"
                    //  + ReportColHeaderValueOnly11(0, "DEDUCTION TYPE : CREDIT")
                    //  };
                    //  lst.Add(crm);

                    //  crm = new CommonReportModel
                    //  {
                    //      RowId = RowCnt++,
                    //      HRF = "F",
                    //      SlNo = "<span style='color:#eef8fd'>^</span>"
                    //+ ReportColFooter(0, "FOR THE MONTH OF ", month)
                    //  };
                    //  lst.Add(crm);

                    //foreach (DataRow dr3 in dt3.Rows)
                    //{
                    //    DescriptionofGLHead = dr3["DescriptionofGLHead"].ToString();
                    //    string FASGLCODE = dr3["FASGLCODE"].ToString();
                    //    string TotalDeductions = dr3["TotalDeductions"].ToString();
                    //    crm = new CommonReportModel
                    //    {
                    //        RowId = RowCnt++,
                    //        HRF = "R",
                    //        // SlNo= SlNo++.ToString(),
                    //        SlNo = dr3["DescriptionofGLHead"].ToString(),
                    //        column2 = dr3["FASGLCODE"].ToString(),
                    //        column3 = dr3["TotalDeductions"].ToString(),

                    //    };
                    //    lst.Add(crm);

                    //}
                    //crm = new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "F",
                    //    SlNo = "<span style='color:#eef8fd'>^</span>"
                    // + ReportColFooter(230, "Total ", Ltotal)
                    //};
                    //lst.Add(crm);


                    //pf loans
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColHeaderValueOnly11(0, "PAYROLL SYSTEM  ")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColHeaderValueOnly11(0, "DETAILS OF VOCHERS PREPARED IN RESPECT OF SALARIES")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                    + ReportColHeaderValueOnly11(0, "DEDUCTION TYPE : CREDIT")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                  + ReportColFooter(0, "FOR THE MONTH OF ", month)
                    };
                    lst.Add(crm);

                    foreach (DataRow dr4 in dt4.Rows)
                    {
                        DescriptionofGLHead = dr4["DescriptionofGLHead"].ToString();
                        string FASGLCODE = dr4["FASGLCODE"].ToString();
                        string TotalDeductions = dr4["TotalDeductions"].ToString();
                        if (TotalDeductions == "")
                        {
                            TotalDeductions = "0";
                        }
                        decimal DTotalDeductions = Convert.ToDecimal(TotalDeductions.ToString()) + 0.00M;
                        decimal DPTTotalDeductions = Convert.ToDecimal(String.Format("{0:0.00}", DTotalDeductions));
                        string NwTotalDeductions = String.Format("{0:n}", DPTTotalDeductions);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            // SlNo= SlNo++.ToString(),
                            SlNo = dr4["DescriptionofGLHead"].ToString(),
                            column2 = dr4["FASGLCODE"].ToString(),
                            column3 = NwTotalDeductions.ToString(),

                        };
                        lst.Add(crm);

                    }
                    if (Ptotal == "")
                    {
                        Ptotal = "0";
                    }

                    decimal DPtotal = Convert.ToDecimal(Ptotal.ToString()) + 0.00M;
                    decimal DPTPtotal = Convert.ToDecimal(String.Format("{0:0.00}", DPtotal));
                    string NwPtotal = String.Format("{0:n}", DPTPtotal);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColFooterAlign(230, "Total ", NwPtotal)
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                   + ReportColFooterAlign(230, "Grand Total ", Nwgrandtotfcredits.ToString())
                    };
                    lst.Add(crm);
                }
            }
            return lst;
        }

        public string ReportColFooterAlign(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable + ": " + "</span>" + "<span style='float:right'>" + value + "</span>";

            return sRet;
        }

        public string ReportColHeaderValueOnly11(int spaceCount, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_HEADER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp &nbsp ";
            sRet += "</span>";

            sRet += "<span><b>" + value + "</b></span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }
    }
}
