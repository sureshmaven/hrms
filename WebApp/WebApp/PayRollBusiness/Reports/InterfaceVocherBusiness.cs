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
    public class InterfaceVocherBusiness:BusinessBase
    {
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        public InterfaceVocherBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public async Task<IList<CommonReportModel>> InterfaceVochersData(string month, string RegEmp, string SupEmp, string Debit, string Credit,string All)
        {
            int SlNo = 1;
            int RowCnt = 0;
            string DescriptionofGLHead = "";
            DateTime today1 = DateTime.Today;
            string  today= today1.ToString("dd-MM-yyyy");
            string q1 = "";
            string q2 = "";
            string general = PrConstants.REGULAR;
            string adhoc = PrConstants.ADHOC;
            string encashment = PrConstants.ENCASHMENT;
            DateTime str = Convert.ToDateTime(month);
            string str1 = str.ToString("yyyy-MM-dd");
            string dm = str.ToString("MMM-yyyy");
            string debitadvice = PrConstants.DEBIT_ADVICE;
            string debitvocher = PrConstants.DEBIT_VOCHER;
            string creditadvice = PrConstants.CREDIT_ADVICE;
            string creditvocher = PrConstants.CREDIT_VOCHER;
            int debitdebitadvicecount = 0;
            string fm = _LoginCredential.FM.ToString();
            int fy = 0;
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
                active = " and p.active =1";
            }
            else
            {
                active = " and p.active =0";
            }

            if (str1 != "" && RegEmp != "" && RegEmp != "undefined")
            {
                q1 = " p.fm ='" + str1 + "' and p.spl_type != '" + encashment + "' ";

            }
            if (str1 != "" && SupEmp != "" && RegEmp == "undefined")
            {
                q1 = " p.fm ='" + str1 + "' and p.spl_type='" + adhoc + "'";
            }
            
            
        string headofcdata = "select 'Total Recoveries - Head Office' as DescriptionofGLHead, '' as GLAcCode, '' as GLAcNO,sum(TotalDeductions) as TotalDeductions from(select 'Total Recoveries - Head Office' as DescriptionofGLHead, " +
                " b.fas_gl_code2 as GLAcCode,b.fas_gl_code1 as GLAcNO, " +
                "sum(p.deductions_amount) as TotalDeductions from pr_emp_payslip p join pr_interface_vochers_codes b on " +
" b.vocher_name = p.branch where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc') and vocher_name in ('OtherBranch', 'HO Bkg-Br')" +
" group by fas_gl_code2,fas_gl_code1 " +
" union all " +
"select 'Total Recoveries - Head Office' as DescriptionofGLHead, '' as GLAcCode, '' as GLAcNO, ((select sum(dd_provident_fund+NPS) as TotalDeductions from pr_emp_payslip o " +
"join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and spl_type = 'Adhoc' and p.branch = 43 )+ (select sum(bank_share) from pr_ob_share o " +
"join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and bank_share!= 0 and p.branch = 43)) as TotalDeductions " +
"union all " +
"select 'Total Recoveries - Head Office' as DescriptionofGLHead, '' as GLAcCode, '' as GLAcNO,sum(pension_open) as TotalDeductions from pr_ob_share o " +
"join employees p on o.emp_code = p.empid where o.fm = '" + str1 + "' and p.branch = 43 ) as x ";

            //string branchdata = "select DescriptionofGLHead,GLAcNO,GLAcCode,sum(TotalDeductions) as TotalDeductions,type from (select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead," +
            //    " b.fas_gl_code2 as GLAcCode,b.fas_gl_code1 as GLAcNO," +
            //        "sum(p.deductions_amount) as TotalDeductions,b.vocher_type as type from pr_emp_payslip p join pr_interface_vochers_codes b on b.vocher_name = p.branch where p.fm = '" + str1 + "' and p.spl_type in('Regular', 'Adhoc') and b.vocher_type = 'branch' " +
            //        "and b.vocher_name != 'HO Bkg-Br' group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
            //        "union all " +
            //        "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,b.fas_gl_code2 as GLAcCode,b.fas_gl_code1 as GLAcNO,sum(dd_provident_fund) as TotalDeductions,b.vocher_type as type from pr_emp_payslip o join pr_interface_vochers_codes b on b.vocher_name = o.branch " +
            //        "join employees p on o.emp_code = p.empid where fm = '" + str1 + "' and spl_type = 'Adhoc' and p.branch != 43 group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
            //        "union all " +
            //        "select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,b.fas_gl_code2 as GLAcCode,b.fas_gl_code1 as GLAcNO,sum(bank_share) as TotalDeductions,b.vocher_type as type from pr_ob_share o " +
            //        "join employees p on o.emp_code = p.empid join branches bb on bb.id = p.branch join pr_interface_vochers_codes b on b.vocher_name = bb.name " +
            //        " where fm = '" + str1 + "' and bank_share!= 0 and p.branch != 43 group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
            //        " union all " +
            //        " select concat('Total Recoveries - ', b.vocher_name) as DescriptionofGLHead,b.fas_gl_code2 as GLAcCode,b.fas_gl_code1 as GLAcNO," +
            //        " sum(pension_open) as TotalDeductions,b.vocher_type as type from pr_ob_share o join employees p on o.emp_code = p.empid join branches bb on bb.id = p.branch " +
            //        " join pr_interface_vochers_codes b on b.vocher_name = bb.name where o.fm = '" + str1 + "'  and p.branch != 43 group by vocher_name,b.vocher_type, fas_gl_code2,fas_gl_code1 " +
            //        " ) as x group by DescriptionofGLHead,GLAcNO,GLAcCode,type order by DescriptionofGLHead ";

            string deductionsdata = " select concat('Total Recoveries - ', d.dd_name) as DescriptionofGLHead, b.fas_gl_code2 as GLAcCode,b.fas_gl_code1 as GLAcNO , sum(dd_amount) as TotalDeductions " +
                "from pr_emp_payslip_deductions d join pr_emp_payslip p on d.payslip_mid = p.id join pr_interface_vochers_codes b on b.vocher_name = d.dd_name " +
                "where b.vocher_type in ('Deductions', 'LoanInt', 'PFLoanInt', 'Loans') and p.fm = '" + str1 + "' and p.spl_type != 'Encashment'  group by dd_name , fas_gl_code2,fas_gl_code1 " +
                "union all select 'Total Recoveries - Income Tax' as DescriptionofGLHead,(select fas_gl_code2 from pr_interface_vochers_codes b where vocher_name = 'Income Tax') as GLAcCode ," +
                "(select fas_gl_code1 from pr_interface_vochers_codes b where vocher_name = 'Income Tax') as GLAcNO , sum(dd_income_tax) from pr_emp_payslip p where p.fm = '" + str1 + "' and p.spl_type != 'Encashment' " +
                "union all select 'Total Recoveries - Prof.Tax' as DescriptionofGLHead,(select fas_gl_code2 from pr_interface_vochers_codes b where vocher_name = 'Prof. Tax')as GLAcCode, " +
                "(select fas_gl_code2 from pr_interface_vochers_codes b where vocher_name = 'Prof. Tax') as GLAcNO ,sum(dd_prof_tax) from pr_emp_payslip p where p.fm = '" + str1 + "' and p.spl_type != 'Encashment'" +
                " union all select 'Total Recoveries - Max Pension' as DescriptionofGLHead,'' as num,'' as num,((select sum(pension_open) as TotalDeductions " +
                "from pr_ob_share where fm = '" + str1 + "'))  as TotalDeductions  union all select 'Total Recoveries - PF Contribution' as DescriptionofGLHead, '' as num1,'' as num1,  ((select ISNULL(sum(dd_provident_fund+NPS),0) as TotalDeductions  from pr_emp_payslip " +
                " where fm = '" + str1 + "'  and spl_type = 'Adhoc')  +(select ISNULL(sum(bank_share),0) as TotalDeductions  from pr_ob_share where fm = '" + str1 + "' and bank_share!= 0) )  as TotalDeductions ";


            string pfloans = "" +

                   "  select 'VPF' as DescriptionofGLHead,'' as GLAcCode,'' as GLAcNO ,(select sum(dd_amount) from pr_emp_payslip_deductions " +
                    "where dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip " +
                    "where fm =  '" + str1 + "' and spl_type in('Regular', 'Adhoc') ))as TotalDeductions " +
                    "union all " +
                 "select 'Provident Fund' as DescriptionofGLHead," +
                 "(select fas_gl_code2 from pr_interface_vochers_codes where vocher_name = 'Provident Fund') as GLAcCode," +
                 "(select  fas_gl_code1 from pr_interface_vochers_codes where vocher_name = 'Provident Fund') as GLAcNO,sum(dd_provident_fund) as TotalDeductions " +
                 "from pr_emp_payslip p where p.fm =  '" + str1 + "'  and p.spl_type in('Regular', 'Adhoc') union all select 'NPS' as DescriptionofGLHead," +
                 "(select fas_gl_code2 from pr_interface_vochers_codes  where vocher_name = 'NPS') as GLAcCode," +
                 "(select  fas_gl_code1 from pr_interface_vochers_codes where vocher_name = 'NPS') as GLAcNO,sum(NPS) as TotalDeductions " +
                 "from pr_emp_payslip p where p.fm = '" + str1 + "'   and p.spl_type in('Regular', 'Adhoc') ";

            DataTable dt = await _sha.Get_Table_FromQry(headofcdata);
            //DataTable dt1 = await _sha.Get_Table_FromQry(branchdata);
           DataTable dt2= await _sha.Get_Table_FromQry(deductionsdata);
           //DataTable dt3 = await _sha.Get_Table_FromQry(allloans);
            DataTable dt4 = await _sha.Get_Table_FromQry(pfloans);
            //HEAD OFFICE
            if (Debit != "" && Debit != "undefined" && Debit != null)
            {
                if (debitdebitadvicecount == 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        debitdebitadvicecount++;
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "D",
                            SlNo = "<span style='color:#d5e6c2'>%</span>"
                        + ReportColHead(130, debitadvice),
                        };
                        lst.Add(crm);
                        
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                             + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :" ,today)

                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                          + ReportColFooters(0, "GL A/c Code ", dr["GLAcCode"].ToString() , 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c NO", dr["GLAcNO"].ToString() , 175, "  A/C NAME", "HEAD OFFICE")
                        };
                        lst.Add(crm);
                        //crm = new CommonReportModel
                        //{
                        //    RowId = RowCnt++,
                        //    HRF = "F",
                        //    SlNo = "<span style='color:#eef8fd'>^</span>"
                        // + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                        //};
                        //lst.Add(crm);
                      //  crm = new CommonReportModel
                      //  {
                      //      RowId = RowCnt++,
                      //      HRF = "F",
                      //      SlNo = "<span style='color:#eef8fd'>^</span>"
                      //+ ReportColFooter(0, "  A/C NAME", "HEAD OFFICE")
                      //  };
                      //  lst.Add(crm);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "Particulars", dr["DescriptionofGLHead"].ToString())
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                        };
                        lst.Add(crm);
                        
                        DescriptionofGLHead = dr["DescriptionofGLHead"].ToString();
                        string TotalDeductions = dr["TotalDeductions"].ToString();
                        string TotalDeductions_amt = dr["TotalDeductions"].ToString();
                        if (TotalDeductions_amt == "")
                        {
                            TotalDeductions_amt = "0";
                        }
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            //+ ReportColFooter(210, "Amount Rs. ", dr["TotalDeductions"].ToString())
                            + ReportColFooter(210, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt))
                        };
                        lst.Add(crm);
                        // amount in words
                        string netSalWords = "Net salary in words";
                        
                        double Amount = Convert.ToDouble(TotalDeductions_amt);
                        int netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                        netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                        netSalWords += " Rupees Only";
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "In Words   ", netSalWords)
                        };
                        lst.Add(crm);
                        
                        
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                        };
                        lst.Add(crm);
                    }
                }
                if (debitdebitadvicecount == 1)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "D",
                            SlNo = "<span style='color:#eef8fd'>%</span>"
                            + ReportColHead(130, debitvocher)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                             + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c Code ", dr["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c NO", dr["GLAcNO"].ToString(), 175, "  A/C NAME", "HEAD OFFICE")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "Particulars", dr["DescriptionofGLHead"].ToString())
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                        };
                        lst.Add(crm);


                      //  crm = new CommonReportModel
                      //  {
                      //      RowId = RowCnt++,
                      //      HRF = "F",
                      //      SlNo = "<span style='color:#eef8fd'>^</span>"
                      //    + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                      //  };
                      //  lst.Add(crm);

                      //  crm = new CommonReportModel
                      //  {
                      //      RowId = RowCnt++,
                      //      HRF = "F",
                      //      SlNo = "<span style='color:#eef8fd'>^</span>"
                      //+ ReportColFooter(0, "  A/C NAME", "HEAD OFFICE")
                      //  };
                      //  lst.Add(crm);

                        DescriptionofGLHead = dr["DescriptionofGLHead"].ToString();
                        string TotalDeductions = dr["TotalDeductions"].ToString();
                        string TotalDeductions_amt = dr["TotalDeductions"].ToString();
                        if (TotalDeductions_amt == "")
                        {
                            TotalDeductions_amt = "0";
                        }
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(200, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt))
                        };
                        lst.Add(crm);
                        //amount in words
                        string netSalWords = "Net salary in words";
                       
                        double Amount = Convert.ToDouble(TotalDeductions_amt);
                        int netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                        netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                        netSalWords += " Rupees Only";
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "In Words   ", netSalWords)
                        };
                        lst.Add(crm);
                        
                        
                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                         "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                    };
                    lst.Add(crm);

                }
                ////branches
                //foreach (DataRow dr1 in dt1.Rows)
                //{
                //    DescriptionofGLHead = dr1["DescriptionofGLHead"].ToString();
                //    string TotalDeductions = dr1["TotalDeductions"].ToString();
                //    debitdebitadvicecount++;
                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "D",
                //        SlNo = "<span style='color:#d5e6c2'>%</span>"
                //        + ReportColHead(130, debitadvice),
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooterValueOnly(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "GL A/c Code ", dr1["GLAcCode"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //     + ReportColFooter(0, "GL A/c NO", dr1["GLAcNO"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //    + ReportColFooter(0, "Particulars", dr1["DescriptionofGLHead"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //   + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                //    };
                //    lst.Add(crm);


                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //  + ReportColFooter(0, "  A/C NAME", "HEAD OFFICE")
                //    };
                //    lst.Add(crm);

                //    DescriptionofGLHead = dr1["DescriptionofGLHead"].ToString();
                //    TotalDeductions = dr1["TotalDeductions"].ToString();
                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //        + ReportColFooter(200, "Amount Rs. ", dr1["TotalDeductions"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //         + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                //         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                //         "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //         "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                //    };
                //    lst.Add(crm);
                //    string oldDescriptionofGLHead = DescriptionofGLHead;

                //    if (oldDescriptionofGLHead == DescriptionofGLHead)
                //    {
                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "D",
                //            SlNo = "<span style='color:#d5e6c2'>%</span>"
                //      + ReportColHead(130, debitvocher)
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //             + ReportColFooterValueOnly(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooter(0, "GL A/c Code ", dr1["GLAcCode"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //         + ReportColFooter(0, "GL A/c NO", dr1["GLAcNO"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //        + ReportColFooter(0, "Particulars", dr1["DescriptionofGLHead"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                //        };
                //        lst.Add(crm);


                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "  A/C NAME", "HEAD OFFICE")
                //        };
                //        lst.Add(crm);

                //        DescriptionofGLHead = dr1["DescriptionofGLHead"].ToString();
                //        TotalDeductions = dr1["TotalDeductions"].ToString();
                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //            + ReportColFooter(200, "Amount Rs. ", dr1["TotalDeductions"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //            + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //            "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                //            "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //            "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //            "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                //            "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //            "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                //        };
                //        lst.Add(crm);
                //    }
                //    //Deductions 
                //}
            }
            //credits
            else if (Debit == "undefined" && All == "undefined" && Credit != null)
            {//deductions
                foreach (DataRow dr2 in dt2.Rows)
                {
                    DescriptionofGLHead = dr2["DescriptionofGLHead"].ToString();
                    string TotalDeductions = dr2["TotalDeductions"].ToString();
                    debitdebitadvicecount++;
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "D",
                        SlNo = "<span style='color:#d5e6c2'>%</span>"
                        + ReportColHead(130, creditadvice),
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c Code ", dr2["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColFooters(0, "GL A/c NO", dr2["GLAcNO"].ToString(), 175, "  A/C NAME", " ")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                    + ReportColFooter(0, "Particulars", dr2["DescriptionofGLHead"].ToString())
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                   + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                    };
                    lst.Add(crm);


                  //  crm = new CommonReportModel
                  //  {
                  //      RowId = RowCnt++,
                  //      HRF = "F",
                  //      SlNo = "<span style='color:#eef8fd'>^</span>"
                  //    + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                  //  };
                  //  lst.Add(crm);

                  //  crm = new CommonReportModel
                  //  {
                  //      RowId = RowCnt++,
                  //      HRF = "F",
                  //      SlNo = "<span style='color:#eef8fd'>^</span>"
                  //+ ReportColFooter(0, "  A/C NAME", "")
                  //  };
                  //  lst.Add(crm);

                    DescriptionofGLHead = dr2["DescriptionofGLHead"].ToString();
                    TotalDeductions = dr2["TotalDeductions"].ToString();
                    string TotalDeductions_amt = dr2["TotalDeductions"].ToString();
                    if (TotalDeductions_amt == "")
                    {
                        TotalDeductions_amt = "0";
                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(200, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt))
                    };
                    lst.Add(crm);

                    //amount in words
                    string netSalWords = "Net salary in words";
                   
                    double Amount = Convert.ToDouble(TotalDeductions_amt);
                    int netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                    netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                    netSalWords += " Rupees Only";
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "In Words   ", netSalWords)
                    };
                    lst.Add(crm);
                    
                   

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                    
                    };
                    lst.Add(crm);
                    string oldDescriptionofGLHead = DescriptionofGLHead;

                    if (oldDescriptionofGLHead == DescriptionofGLHead)
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "D",
                            SlNo = "<span style='color:#d5e6c2'>%</span>"
                     + ReportColHead(130, creditvocher)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                              + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                          + ReportColFooters(0, "GL A/c Code ", dr2["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c NO", dr2["GLAcNO"].ToString(), 175, "  A/C NAME", " ")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "Particulars", dr2["DescriptionofGLHead"].ToString())
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                        };
                        lst.Add(crm);


                       // crm = new CommonReportModel
                       // {
                       //     RowId = RowCnt++,
                       //     HRF = "F",
                       //     SlNo = "<span style='color:#eef8fd'>^</span>"
                       //   + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                       // };
                       // lst.Add(crm);

                       // crm = new CommonReportModel
                       // {
                       //     RowId = RowCnt++,
                       //     HRF = "F",
                       //     SlNo = "<span style='color:#eef8fd'>^</span>"
                       //+ ReportColFooter(0, "  A/C NAME", "")
                       // };
                       // lst.Add(crm);

                        DescriptionofGLHead = dr2["DescriptionofGLHead"].ToString();
                        TotalDeductions = dr2["TotalDeductions"].ToString();
                        string TotalDeductions_amt1 = dr2["TotalDeductions"].ToString();
                        if (TotalDeductions_amt1 == "")
                        {
                            TotalDeductions_amt1 = "0";
                        }
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(210, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt1))
                        };
                        lst.Add(crm);
                        //amount in words
                        netSalWords = "Net salary in words";
                        
                        Amount = Convert.ToDouble(TotalDeductions_amt1);
                        netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                        netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                        netSalWords += " Rupees Only";
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "In Words   ", netSalWords)
                        };
                        lst.Add(crm);
                         
                        

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                        };
                        lst.Add(crm);
                    }
                    //Deductions 
                }
                ////all loans
                //foreach (DataRow dr3 in dt3.Rows)
                //{
                //    DescriptionofGLHead = dr3["DescriptionofGLHead"].ToString();
                //    string TotalDeductions = dr3["TotalDeductions"].ToString();
                //    debitdebitadvicecount++;
                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "D",
                //        SlNo = "<span style='color:#d5e6c2'>%</span>"
                //        + ReportColHead(130, creditadvice),
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooterValueOnly(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "GL A/c Code ", dr3["GLAcCode"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //     + ReportColFooter(0, "GL A/c NO", dr3["GLAcNO"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //    + ReportColFooter(0, "Particulars", dr3["DescriptionofGLHead"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //   + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                //    };
                //    lst.Add(crm);


                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //   + ReportColFooter(0, "  A/C NAME", "")
                //    };
                //    lst.Add(crm);

                //    DescriptionofGLHead = dr3["DescriptionofGLHead"].ToString();
                //    TotalDeductions = dr3["TotalDeductions"].ToString();
                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //        + ReportColFooter(200, "Amount Rs. ", dr3["TotalDeductions"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                //    };
                //    lst.Add(crm);
                //    string oldDescriptionofGLHead = DescriptionofGLHead;

                //    if (oldDescriptionofGLHead == DescriptionofGLHead)
                //    {
                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "D",
                //            SlNo = "<span style='color:#d5e6c2'>%</span>"
                //      + ReportColHead(130, creditvocher)
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //              + ReportColFooterValueOnly(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooter(0, "GL A/c Code ", dr3["GLAcCode"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //         + ReportColFooter(0, "GL A/c NO", dr3["GLAcNO"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //        + ReportColFooter(0, "Particulars", dr3["DescriptionofGLHead"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                //        };
                //        lst.Add(crm);


                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //       + ReportColFooter(0, "  A/C NAME", "")
                //        };
                //        lst.Add(crm);

                //        DescriptionofGLHead = dr3["DescriptionofGLHead"].ToString();
                //        TotalDeductions = dr3["TotalDeductions"].ToString();
                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //            + ReportColFooter(200, "Amount Rs. ", dr3["TotalDeductions"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                //        };
                //        lst.Add(crm);

                //    }

                    
                //}
                //Pf Loans
                foreach (DataRow dr4 in dt4.Rows)
                {
                    DescriptionofGLHead = dr4["DescriptionofGLHead"].ToString();
                    string TotalDeductions = dr4["TotalDeductions"].ToString();
                    debitdebitadvicecount++;
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "D",
                        SlNo = "<span style='color:#d5e6c2'>%</span>"
                        + ReportColHead(130, creditadvice),
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c Code ", dr4["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColFooters(0, "GL A/c NO", dr4["GLAcNO"].ToString(), 175, "  A/C NAME", " ")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                    + ReportColFooter(0, "Particulars", dr4["DescriptionofGLHead"].ToString())
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                  + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                    };
                    lst.Add(crm);


                  //  crm = new CommonReportModel
                  //  {
                  //      RowId = RowCnt++,
                  //      HRF = "F",
                  //      SlNo = "<span style='color:#eef8fd'>^</span>"
                  //    + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                  //  };
                  //  lst.Add(crm);

                  //  crm = new CommonReportModel
                  //  {
                  //      RowId = RowCnt++,
                  //      HRF = "F",
                  //      SlNo = "<span style='color:#eef8fd'>^</span>"
                  //+ ReportColFooter(0, "  A/C NAME", "")
                  //  };
                  //  lst.Add(crm);

                    DescriptionofGLHead = dr4["DescriptionofGLHead"].ToString();
                    TotalDeductions = dr4["TotalDeductions"].ToString();
                    string TotalDeductions_amt = dr4["TotalDeductions"].ToString();
                    if (TotalDeductions_amt == "")
                    {
                        TotalDeductions_amt = "0";
                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(200, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt))
                    };
                    lst.Add(crm);

                    //amount in words
                    string netSalWords = "Net salary in words";
                   
                    double Amount = Convert.ToDouble(TotalDeductions_amt);
                    int netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                    netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                    netSalWords += " Rupees Only";
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "In Words   ", netSalWords)
                    };
                    lst.Add(crm);                                     

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                    };
                    lst.Add(crm);
                    string oldDescriptionofGLHead = DescriptionofGLHead;

                    if (oldDescriptionofGLHead == DescriptionofGLHead)
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "D",
                            SlNo = "<span style='color:#d5e6c2'>%</span>"
                     + ReportColHead(130, creditvocher)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                             + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c Code ", dr4["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c NO", dr4["GLAcNO"].ToString(), 175, "  A/C NAME", " ")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "Particulars", dr4["DescriptionofGLHead"].ToString())
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                        };
                        lst.Add(crm);


                       // crm = new CommonReportModel
                       // {
                       //     RowId = RowCnt++,
                       //     HRF = "F",
                       //     SlNo = "<span style='color:#eef8fd'>^</span>"
                       //   + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                       // };
                       // lst.Add(crm);

                       // crm = new CommonReportModel
                       // {
                       //     RowId = RowCnt++,
                       //     HRF = "F",
                       //     SlNo = "<span style='color:#eef8fd'>^</span>"
                       //+ ReportColFooter(0, "  A/C NAME", "")
                       // };
                       // lst.Add(crm);

                        DescriptionofGLHead = dr4["DescriptionofGLHead"].ToString();
                        TotalDeductions = dr4["TotalDeductions"].ToString();
                        string TotalDeductions_amt1 = dr4["TotalDeductions"].ToString();
                        if (TotalDeductions_amt1 == "")
                        {
                            TotalDeductions_amt1 = "0";
                        }
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(200, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt1))
                        };
                        lst.Add(crm);

                        //amount in words
                        netSalWords = "Net salary in words";
                       
                        Amount = Convert.ToDouble(TotalDeductions_amt1);
                        netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                        netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                        netSalWords += " Rupees Only";
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "In Words   ", netSalWords)
                        };
                        lst.Add(crm);
                        
                        

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                        };
                        lst.Add(crm);
                    }
          
                }
            }
            else if(All != null)
            {
                if (debitdebitadvicecount == 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        debitdebitadvicecount++;
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "D",
                            SlNo = "<span style='color:#d5e6c2'>%</span>"
                        + ReportColHead(130, debitadvice),
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                             + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)

                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c Code ", dr["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c NO", dr["GLAcNO"].ToString(), 175, "  A/C NAME", "HEAD OFFICE")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "Particulars", dr["DescriptionofGLHead"].ToString())
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                        };
                        lst.Add(crm);


                      //  crm = new CommonReportModel
                      //  {
                      //      RowId = RowCnt++,
                      //      HRF = "F",
                      //      SlNo = "<span style='color:#eef8fd'>^</span>"
                      //    + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                      //  };
                      //  lst.Add(crm);

                      //  crm = new CommonReportModel
                      //  {
                      //      RowId = RowCnt++,
                      //      HRF = "F",
                      //      SlNo = "<span style='color:#eef8fd'>^</span>"
                      //+ ReportColFooter(0, "  A/C NAME", "HEAD OFFICE")
                      //  };
                      //  lst.Add(crm);

                        DescriptionofGLHead = dr["DescriptionofGLHead"].ToString();
                        string TotalDeductions = dr["TotalDeductions"].ToString();
                        string TotalDeductions_amt = dr["TotalDeductions"].ToString();
                        if (TotalDeductions_amt == "")
                        {
                            TotalDeductions_amt = "0";
                        }
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(200, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt))
                        };
                        lst.Add(crm);

                        //amount in words
                        string netSalWords = "Net salary in words";
                        //
                       

                        double Amount = Convert.ToDouble(TotalDeductions_amt);
                        int netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                        netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                        netSalWords += " Rupees Only";
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "In Words   ", netSalWords)
                        };
                        lst.Add(crm);
                        
                       

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                        };
                        lst.Add(crm);
                    }
                }
                if (debitdebitadvicecount == 1)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "D",
                            SlNo = "<span style='color:#d5e6c2'>%</span>"
                            + ReportColHead(130, debitvocher)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                             + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c Code ", dr["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c NO", dr["GLAcNO"].ToString(), 175, "  A/C NAME", "HEAD OFFICE")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "Particulars", dr["DescriptionofGLHead"].ToString())
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                        };
                        lst.Add(crm);


                      //  crm = new CommonReportModel
                      //  {
                      //      RowId = RowCnt++,
                      //      HRF = "F",
                      //      SlNo = "<span style='color:#eef8fd'>^</span>"
                      //    + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                      //  };
                      //  lst.Add(crm);

                      //  crm = new CommonReportModel
                      //  {
                      //      RowId = RowCnt++,
                      //      HRF = "F",
                      //      SlNo = "<span style='color:#eef8fd'>^</span>"
                      //+ ReportColFooter(0, "  A/C NAME", "HEAD OFFICE")
                      //  };
                      //  lst.Add(crm);

                        DescriptionofGLHead = dr["DescriptionofGLHead"].ToString();
                        string TotalDeductions = dr["TotalDeductions"].ToString();
                        string TotalDeductions_amt = dr["TotalDeductions"].ToString();
                        if (TotalDeductions_amt == "")
                        {
                            TotalDeductions_amt = "0";
                        }

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(200, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt))
                        };
                        lst.Add(crm);

                        //amount in words
                        string netSalWords = "Net salary in words";
                        //
                      
                        double Amount = Convert.ToDouble(TotalDeductions_amt);
                        int netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                        netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                        netSalWords += " Rupees Only";
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "In Words   ", netSalWords)
                        };
                        lst.Add(crm);
                        
                        

                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                    };
                    lst.Add(crm);

                }
                ////branches
                //foreach (DataRow dr1 in dt1.Rows)
                //{
                //    DescriptionofGLHead = dr1["DescriptionofGLHead"].ToString();
                //    string TotalDeductions = dr1["TotalDeductions"].ToString();
                //    debitdebitadvicecount++;
                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "D",
                //        SlNo = "<span style='color:#d5e6c2'>%</span>"
                //        + ReportColHead(130, debitadvice),
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooterValueOnly(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "GL A/c Code ", dr1["GLAcCode"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //     + ReportColFooter(0, "GL A/c NO", dr1["GLAcNO"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //    + ReportColFooter(0, "Particulars", dr1["DescriptionofGLHead"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //   + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                //    };
                //    lst.Add(crm);


                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //  + ReportColFooter(0, "  A/C NAME", "HEAD OFFICE")
                //    };
                //    lst.Add(crm);

                //    DescriptionofGLHead = dr1["DescriptionofGLHead"].ToString();
                //    TotalDeductions = dr1["TotalDeductions"].ToString();
                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //        + ReportColFooter(200, "Amount Rs. ", dr1["TotalDeductions"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                //    };
                //    lst.Add(crm);
                //    string oldDescriptionofGLHead = DescriptionofGLHead;

                //    if (oldDescriptionofGLHead == DescriptionofGLHead)
                //    {
                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "D",
                //            SlNo = "<span style='color:#d5e6c2'>%</span>"
                //      + ReportColHead(130, debitvocher)
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //             + ReportColFooterValueOnly(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooter(0, "GL A/c Code ", dr1["GLAcCode"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //         + ReportColFooter(0, "GL A/c NO", dr1["GLAcNO"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //        + ReportColFooter(0, "Particulars", dr1["DescriptionofGLHead"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                //        };
                //        lst.Add(crm);


                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "  A/C NAME", "HEAD OFFICE")
                //        };
                //        lst.Add(crm);

                //        DescriptionofGLHead = dr1["DescriptionofGLHead"].ToString();
                //        TotalDeductions = dr1["TotalDeductions"].ToString();
                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //            + ReportColFooter(200, "Amount Rs. ", dr1["TotalDeductions"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                //        };
                //        lst.Add(crm);
                //    }
                //    //Deductions 
                //}

                //deductions
                foreach (DataRow dr2 in dt2.Rows)
                {
                    DescriptionofGLHead = dr2["DescriptionofGLHead"].ToString();
                    string TotalDeductions = dr2["TotalDeductions"].ToString();
                    debitdebitadvicecount++;
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "D",
                        SlNo = "<span style='color:#d5e6c2'>%</span>"
                        + ReportColHead(130, creditadvice),
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c Code ", dr2["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColFooters(0, "GL A/c NO", dr2["GLAcNO"].ToString(), 175, "  A/C NAME", " ")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                    + ReportColFooter(0, "Particulars", dr2["DescriptionofGLHead"].ToString())
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                   + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                    };
                    lst.Add(crm);


                  //  crm = new CommonReportModel
                  //  {
                  //      RowId = RowCnt++,
                  //      HRF = "F",
                  //      SlNo = "<span style='color:#eef8fd'>^</span>"
                  //    + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                  //  };
                  //  lst.Add(crm);

                  //  crm = new CommonReportModel
                  //  {
                  //      RowId = RowCnt++,
                  //      HRF = "F",
                  //      SlNo = "<span style='color:#eef8fd'>^</span>"
                  //+ ReportColFooter(0, "  A/C NAME", "")
                  //  };
                  //  lst.Add(crm);

                    DescriptionofGLHead = dr2["DescriptionofGLHead"].ToString();
                    TotalDeductions = dr2["TotalDeductions"].ToString();
                    string TotalDeductions_amt = dr2["TotalDeductions"].ToString();
                    if (TotalDeductions_amt == "")
                    {
                        TotalDeductions_amt = "0";
                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(200, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt))
                    };
                    lst.Add(crm);

                    //amount in words
                    string netSalWords = "Net salary in words";
                    //                   
                    double Amount = Convert.ToDouble(TotalDeductions_amt);
                    int netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                    netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                    netSalWords += " Rupees Only";
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "In Words   ", netSalWords)
                    };
                    lst.Add(crm);
                    
                    

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                    };
                    lst.Add(crm);
                    string oldDescriptionofGLHead = DescriptionofGLHead;

                    if (oldDescriptionofGLHead == DescriptionofGLHead)
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "D",
                            SlNo = "<span style='color:#d5e6c2'>%</span>"
                     + ReportColHead(130, creditvocher)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                              + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c Code ", dr2["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c NO", dr2["GLAcNO"].ToString(), 175, "  A/C NAME", " ")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "Particulars", dr2["DescriptionofGLHead"].ToString())
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                        };
                        lst.Add(crm);


                       // crm = new CommonReportModel
                       // {
                       //     RowId = RowCnt++,
                       //     HRF = "F",
                       //     SlNo = "<span style='color:#eef8fd'>^</span>"
                       //   + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                       // };
                       // lst.Add(crm);

                       // crm = new CommonReportModel
                       // {
                       //     RowId = RowCnt++,
                       //     HRF = "F",
                       //     SlNo = "<span style='color:#eef8fd'>^</span>"
                       //+ ReportColFooter(0, "  A/C NAME", "")
                       // };
                       // lst.Add(crm);

                        DescriptionofGLHead = dr2["DescriptionofGLHead"].ToString();
                        TotalDeductions = dr2["TotalDeductions"].ToString();
                        string TotalDeductions_amt1 = dr2["TotalDeductions"].ToString();
                        if (TotalDeductions_amt1 == "")
                        {
                            TotalDeductions_amt1 = "0";
                        }
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(200, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt1))
                        };
                        lst.Add(crm);

                        //amount in words
                        netSalWords = "Net salary in words";
                        
                        Amount = Convert.ToDouble(TotalDeductions_amt1);
                        netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                        netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                        netSalWords += " Rupees Only";
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "In Words   ", netSalWords)
                        };
                        lst.Add(crm);
                         
                        

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                        };
                        lst.Add(crm);
                    }
                    //Deductions 
                }
                ////all loans
                //foreach (DataRow dr3 in dt3.Rows)
                //{
                //    DescriptionofGLHead = dr3["DescriptionofGLHead"].ToString();
                //    string TotalDeductions = dr3["TotalDeductions"].ToString();
                //    debitdebitadvicecount++;
                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "D",
                //        SlNo = "<span style='color:#d5e6c2'>%</span>"
                //        + ReportColHead(130, creditadvice),
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooterValueOnly(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "GL A/c Code ", dr3["GLAcCode"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //     + ReportColFooter(0, "GL A/c NO", dr3["GLAcNO"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //    + ReportColFooter(0, "Particulars", dr3["DescriptionofGLHead"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //   + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                //    };
                //    lst.Add(crm);


                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //   + ReportColFooter(0, "  A/C NAME", "")
                //    };
                //    lst.Add(crm);

                //    DescriptionofGLHead = dr3["DescriptionofGLHead"].ToString();
                //    TotalDeductions = dr3["TotalDeductions"].ToString();
                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //        + ReportColFooter(200, "Amount Rs. ", dr3["TotalDeductions"].ToString())
                //    };
                //    lst.Add(crm);

                //    crm = new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        HRF = "F",
                //        SlNo = "<span style='color:#eef8fd'>^</span>"
                //       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                //    };
                //    lst.Add(crm);
                //    string oldDescriptionofGLHead = DescriptionofGLHead;

                //    if (oldDescriptionofGLHead == DescriptionofGLHead)
                //    {
                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "D",
                //            SlNo = "<span style='color:#d5e6c2'>%</span>"
                //      + ReportColHead(130, creditvocher)
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //              + ReportColFooterValueOnly(100, "TELANGANA STATE CO-OP APEX BANK LTD")
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooter(0, "GL A/c Code ", dr3["GLAcCode"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //         + ReportColFooter(0, "GL A/c NO", dr3["GLAcNO"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //        + ReportColFooter(0, "Particulars", dr3["DescriptionofGLHead"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //      + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                //        };
                //        lst.Add(crm);


                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //          + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //       + ReportColFooter(0, "  A/C NAME", "")
                //        };
                //        lst.Add(crm);

                //        DescriptionofGLHead = dr3["DescriptionofGLHead"].ToString();
                //        TotalDeductions = dr3["TotalDeductions"].ToString();
                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //            + ReportColFooter(200, "Amount Rs. ", dr3["TotalDeductions"].ToString())
                //        };
                //        lst.Add(crm);

                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            HRF = "F",
                //            SlNo = "<span style='color:#eef8fd'>^</span>"
                //       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                //       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                //        };
                //        lst.Add(crm);

                //    }


                //}
                //Pf Loans
                foreach (DataRow dr4 in dt4.Rows)
                {
                    DescriptionofGLHead = dr4["DescriptionofGLHead"].ToString();
                    string TotalDeductions = dr4["TotalDeductions"].ToString();
                    debitdebitadvicecount++;
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "D",
                        SlNo = "<span style='color:#d5e6c2'>%</span>"
                        + ReportColHead(130, creditadvice),
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                          + ReportColFooters(0, "GL A/c Code ", dr4["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                     + ReportColFooters(0, "GL A/c NO", dr4["GLAcNO"].ToString(), 175, "  A/C NAME", " ")
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                    + ReportColFooter(0, "Particulars", dr4["DescriptionofGLHead"].ToString())
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                  + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                    };
                    lst.Add(crm);


                  //  crm = new CommonReportModel
                  //  {
                  //      RowId = RowCnt++,
                  //      HRF = "F",
                  //      SlNo = "<span style='color:#eef8fd'>^</span>"
                  //    + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                  //  };
                  //  lst.Add(crm);

                  //  crm = new CommonReportModel
                  //  {
                  //      RowId = RowCnt++,
                  //      HRF = "F",
                  //      SlNo = "<span style='color:#eef8fd'>^</span>"
                  //+ ReportColFooter(0, "  A/C NAME", "")
                  //  };
                  //  lst.Add(crm);

                    DescriptionofGLHead = dr4["DescriptionofGLHead"].ToString();
                    TotalDeductions = dr4["TotalDeductions"].ToString();
                    string TotalDeductions_amt = dr4["TotalDeductions"].ToString();
                    if (TotalDeductions_amt == "")
                    {
                        TotalDeductions_amt = "0";
                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(200, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt))
                    };
                    lst.Add(crm);

                    //amount in words
                    string netSalWords = "Net salary in words";
                   
                    double Amount = Convert.ToDouble(TotalDeductions_amt);
                    int netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                    netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                    netSalWords += " Rupees Only";
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "In Words   ", netSalWords)
                    };
                    lst.Add(crm);
                    
                    

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                    };
                    lst.Add(crm);
                    string oldDescriptionofGLHead = DescriptionofGLHead;

                    if (oldDescriptionofGLHead == DescriptionofGLHead)
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "D",
                            SlNo = "<span style='color:#d5e6c2'>%</span>"
                     + ReportColHead(130, creditvocher)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                             + ReportColHeaderValueOnly11(100, "TELANGANA STATE CO-OP APEX BANK LTD", "Date :", today)
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c Code ", dr4["GLAcCode"].ToString(), 170, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                         + ReportColFooters(0, "GL A/c NO", dr4["GLAcNO"].ToString(), 175, "  A/C NAME", " ")
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                        + ReportColFooter(0, "Particulars", dr4["DescriptionofGLHead"].ToString())
                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                      + ReportColFooter(0, "Being The Amount Of Salary Recoveries Debited For The Month Of ", month)
                        };
                        lst.Add(crm);


                       // crm = new CommonReportModel
                       // {
                       //     RowId = RowCnt++,
                       //     HRF = "F",
                       //     SlNo = "<span style='color:#eef8fd'>^</span>"
                       //   + ReportColFooter(0, "  GL HEAD  ", "BRANCHES ACCOUNTS")
                       // };
                       // lst.Add(crm);

                       // crm = new CommonReportModel
                       // {
                       //     RowId = RowCnt++,
                       //     HRF = "F",
                       //     SlNo = "<span style='color:#eef8fd'>^</span>"
                       //+ ReportColFooter(0, "  A/C NAME", "")
                       // };
                       // lst.Add(crm);

                        DescriptionofGLHead = dr4["DescriptionofGLHead"].ToString();
                        TotalDeductions = dr4["TotalDeductions"].ToString();
                        string TotalDeductions_amt1 = dr4["TotalDeductions"].ToString();
                        if (TotalDeductions_amt1 == "")
                        {
                            TotalDeductions_amt1 = "0";
                        }
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(200, "Amount Rs. ", ReportColConvertToDecimal(TotalDeductions_amt1))
                        };
                        lst.Add(crm);

                        //amount in words
                        netSalWords = "Net salary in words";
                      
                        Amount = Convert.ToDouble(TotalDeductions_amt1);
                        netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(Amount)));
                        netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                        netSalWords += " Rupees Only";
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "In Words   ", netSalWords)
                        };
                        lst.Add(crm);
                         
                        

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            SlNo = "<span style='color:#eef8fd'>^</span>"
                       + ReportColFooterThreestrings(0, "Staff Asst" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp</span>", "Manager" + "<span>&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                       "&nbsp;&nbsp;&nbsp;&nbsp</span>", "Asst.Gen Mngr")
                        };
                        lst.Add(crm);
                    }

                }
            }
            return lst;
        }
        public string ReportColFooters(int spaceCount, string lable, string value, int spaceCount1, string lable1, string value1)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable + ": " + value + " </span>";
            for (int i = 1; i <= spaceCount1; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable1 + ": " + value1 + " </span>";
            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string ReportColHeaderValueOnly11(int spaceCount, string value, string value1,string Date)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_HEADER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp &nbsp ";
            sRet += "</span>";

            sRet += "<span><b>" + value + "</b></span>";
            sRet += "&nbsp &nbsp ";
            sRet += "&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp  ";
            sRet += "<span><b>" + value1 + "</b></span>";
            sRet += "<span><b>" + Date + "</b></span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
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
