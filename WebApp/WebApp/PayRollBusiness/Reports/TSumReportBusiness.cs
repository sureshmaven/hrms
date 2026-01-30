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
    public class TSumReportBusiness : BusinessBase
    {
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        public TSumReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public async Task<IList<CommonReportModel>> TSumReportData(string month, string RegEmp, string SupEmp)
        {
            int SlNo = 1;
            int RowCnt = 0;
            string branch = "";
            string q1 = "";
            string q2 = "";
            string general = PrConstants.REGULAR;
            string adhoc = PrConstants.ADHOC;
            string encashment = PrConstants.ENCASHMENT;
            DateTime str = Convert.ToDateTime(month);
            string str1 = str.ToString("yyyy-MM-dd");
            string dm = str.ToString("MMM-yyyy");
            string test = "2019-05-01";
            string fm = _LoginCredential.FM.ToString();
            int fy = 0;
            if (fm=="01" || fm == "02" || fm == "03")
            {
                fy = _LoginCredential.FY ;
            } else
            {
                fy = _LoginCredential.FY - 1;
            }
            
            string day = "01";
            string[] fmon = new string[] { fy.ToString(), fm, day };
            string fmm = string.Join("-", fmon);
            string active = "";
            if(str1 == fmm)
            {
                active = " and p.active =1";
            }
            else
            {
                active = " and p.active =0";
            }

            if (str1 != "" && RegEmp != "" && RegEmp != "undefined")
            {
                q1= " and p.fm ='" + str1 + "' and spl_type in( '" + general + "' , '" + adhoc + "') ";

            }
            if (str1 != "" && SupEmp != "" && RegEmp == "undefined")
            {
               q1= " and p.fm ='" + str1 + "' and p.spl_type='" + adhoc + "'";
            }
            //string query = "select* from(select 'HEADOFFICE' as Summary_Details,sum(p.gross_amount) as GrossSalary," +
            //  "sum(p.dd_provident_fund) as ProvidentFund,sum(pd.dd_amount) as VPF,sum(p.deductions_amount) as TotalDeductions," +
            //  "sum(p.net_amount) as NetSalary from pr_emp_payslip p join pr_emp_payslip_deductions pd on p.id = pd.payslip_mid " +
            //  "where p.branch = 'OtherBranch' " + q1 + ") as x union all(select 'BRANCHES' as Summary_Details,sum(p.gross_amount) " +
            //  "as GrossSalary,sum(p.dd_provident_fund) as ProvidentFund,sum(pd.dd_amount) as VPF," +
            //  "sum(p.deductions_amount) as TotalDeductions,sum(p.net_amount) as NetSalary " +
            //  "from pr_emp_payslip p join pr_emp_payslip_deductions pd on p.id = pd.payslip_mid where p.branch != 'OtherBranch' " + q1 + " )";

            //string query1 = "select* from(select 'HEADOFFICE' as Summary_Details,sum(p.gross_amount) as GrossSalary," +
            // "sum(p.dd_provident_fund) as ProvidentFund,sum(pd.dd_amount) as VPF,sum(p.deductions_amount) as TotalDeductions," +
            // "sum(p.net_amount) as NetSalary from pr_emp_payslip p join pr_emp_payslip_deductions pd on p.id = pd.payslip_mid and pd.dd_name='VPF Deduction'" +
            // "where p.branch = 'OtherBranch' " + q1 + ") as x union all(select 'BRANCHES' as Summary_Details,sum(p.gross_amount) " +
            // "as GrossSalary,sum(p.dd_provident_fund) as ProvidentFund,sum(pd.dd_amount) as VPF," +
            // "sum(p.deductions_amount) as TotalDeductions,sum(p.net_amount) as NetSalary " +
            // "from pr_emp_payslip p join pr_emp_payslip_deductions pd on p.id = pd.payslip_mid and pd.dd_name='VPF Deduction' where p.branch != 'OtherBranch' " + q1 + " )";
//add nps condition for adhoc amount
          
            string query1 = "select * from(select 'HEADOFFICE' as Summary_Details,case when sum(p.gross_amount) > 0 then sum(p.gross_amount) else 0 end as GrossSalary," +
                " case when sum(p.dd_provident_fund)> 0 then sum(p.dd_provident_fund) else 0 end as ProvidentFund," +
                "case when sum(p.deductions_amount )> 0 then sum(p.deductions_amount) else 0 end as TotalDeductions," +
                " case when sum(p.net_amount)> 0 then sum(p.net_amount) else 0 end as NetSalary," +
                "case when sum(p.NPS)" +

                " +(select case when sum(amount) >0 then sum(amount) else 0 end as NPS from pr_emp_adhoc_deduction_field a  join pr_emp_payslip p on p.emp_code = a.emp_code" +
                " where a.fm='" + str1 + "' and  m_id in (586) and p.branch  in ('OtherBranch', 'HO Bkg-Br')  " +
                " and p.fm='" + str1 + "'  and spl_type in('Adhoc') )" +

                "> 0 then sum(p.NPS) " +
                " +(select case when sum(amount) >0 then sum(amount) else 0 end as NPS from pr_emp_adhoc_deduction_field a  join pr_emp_payslip p on p.emp_code = a.emp_code" +
                " where a.fm = '" + str1 + "' and  m_id in (586) and p.branch  in ('OtherBranch', 'HO Bkg-Br')    " +
                "and  p.fm='" + str1 + "'  and spl_type in('Adhoc') )" +
                " else 0 end as NPS from pr_emp_payslip p " +
                " where p.branch in ('OtherBranch','HO Bkg-Br')  " + q1+ " ) as x " +
                "union all" +
                "(select 'BRANCHES' as Summary_Details," +
                " case when sum(p.gross_amount) > 0 then sum(p.gross_amount) else 0 end as GrossSalary,case when sum(p.dd_provident_fund) > 0 then sum(p.dd_provident_fund) else 0 end as ProvidentFund," +
                "case when sum(p.deductions_amount) > 0 then sum(p.deductions_amount) else 0 end as TotalDeductions," +
                " case when sum(p.net_amount) > 0 then sum(p.net_amount) else 0 end as NetSalary," +
                "case when sum(p.NPS)" +
                " +(select case when sum(amount) >0 then sum(amount) else 0 end as NPS from pr_emp_adhoc_deduction_field a  join pr_emp_payslip p on p.emp_code = a.emp_code" +
                " where    a.fm='" + str1 + "' and m_id in (586) and p.branch not  in ('OtherBranch', 'HO Bkg-Br')  " +
                " and p.fm='" + str1 + "'  and spl_type in('Adhoc') )" +

                "> 0 then sum(p.NPS)" +
                " +(select case when sum(amount) >0 then sum(amount) else 0 end as NPS from pr_emp_adhoc_deduction_field a  join pr_emp_payslip p on p.emp_code = a.emp_code" +
                " where    a.fm='" + str1 + "' and m_id in (586) and p.branch not  in ('OtherBranch', 'HO Bkg-Br')  " +
                " and p.fm='" + str1 + "'  and spl_type in('Adhoc') )" +
                " else 0 end as NPS from pr_emp_payslip p  where p.branch not in ('OtherBranch','HO Bkg-Br')  " + q1+ " )";

            string vpfhead = "select  'HEADOFFICE' as Summary_Details,case when sum(pd.dd_amount)> 0 then sum(pd.dd_amount) " +
                "else 0 end as VPF from pr_emp_payslip p join pr_emp_payslip_deductions pd on p.id = pd.payslip_mid and pd.dd_name = 'VPF Deduction' where p.fm = '" + str1 + "' " +
                "and p.branch  in ('OtherBranch', 'HO Bkg-Br') and spl_type in('Regular', 'Adhoc') ";            string vpfbranch= "select 'BRANCHES' as Summary_Details,case when sum(pd.dd_amount)> 0 then sum(pd.dd_amount) else 0 end as VPF " +
                "from pr_emp_payslip p join pr_emp_payslip_deductions pd on p.id = pd.payslip_mid and pd.dd_name = 'VPF Deduction' " +
                "where p.fm = '" + str1 + "'  and p.branch not in ('OtherBranch', 'HO Bkg-Br') and spl_type in('Regular', 'Adhoc') ";


            DataTable dt = await _sha.Get_Table_FromQry(query1);
            DataTable dtvpfhead = await _sha.Get_Table_FromQry(vpfhead);
            DataTable dtvpfbranch = await _sha.Get_Table_FromQry(vpfbranch);
            foreach (DataRow dr in dt.Rows)
                {
                    branch = dr["Summary_Details"].ToString();
                   string GrossSalary = dr["GrossSalary"].ToString();
                    string ProvidentFund = dr["ProvidentFund"].ToString();
                  string NPS = dr["NPS"].ToString();
                    //string VPF = dr["VPF"].ToString();
                    string TotalDeductions = dr["TotalDeductions"].ToString();
                    if (GrossSalary != "" && ProvidentFund != "" && TotalDeductions != "" && branch != "")
                    {
                        if (branch == "HEADOFFICE")
                        {
                        //crm = new CommonReportModel
                        //{
                        //    RowId = RowCnt++,
                        //    HRF = "H",
                        //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                        // + ReportColHeaderValueOnly(0, dm)
                        //};
                        //lst.Add(crm);
                        string DrGrosssalary = dr["GrossSalary"].ToString();
                        if(DrGrosssalary=="")
                        {
                            DrGrosssalary = "0";
                        }
                        string DrProvidentFund = dr["ProvidentFund"].ToString();
                        if (DrProvidentFund == "")
                        {
                            DrProvidentFund = "0";
                        }
                        string DrNPS = dr["NPS"].ToString();
                        if (DrNPS == "")
                        {
                            DrNPS = "0";
                        }

                        string Drvpfhead = dtvpfhead.Rows[0]["VPF"].ToString();
                        if (Drvpfhead == "")
                        {
                            Drvpfhead = "0";
                        }
                        string DrTotalDeductions = dr["TotalDeductions"].ToString();
                        if (DrTotalDeductions == "")
                        {
                            DrTotalDeductions = "0";
                        }
                        string DrNetSalary = dr["NetSalary"].ToString();
                        if (DrNetSalary == "")
                        {
                            DrNetSalary = "0";
                        }

                        crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                // SlNo= SlNo++.ToString(),
                                SlNo = dr["Summary_Details"].ToString(),
                            // column2 =Math.Round(Convert.ToDecimal(dr["GrossSalary"]),2).ToString(),
                            column2 = ReportColConvertToDecimal(DrGrosssalary),
                            column3 = ReportColConvertToDecimal(DrProvidentFund),
                                column7 = ReportColConvertToDecimal(DrNPS),
                                column4 = ReportColConvertToDecimal(Drvpfhead),
                                column5 = ReportColConvertToDecimal(DrTotalDeductions),
                                column6 = ReportColConvertToDecimal(DrNetSalary),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",
                                SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooterAlign(0, "PASSED FOR PAYMENT OF GROSS AMOUNT OF    Rs  ", ReportColConvertToDecimal(DrGrosssalary))
                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",
                                SlNo = "<span style='color:#eef8fd'>^</span>"
                             + ReportColFooterAlign(0, "FOR PAYMENT OF NET   AMOUNT OF    Rs  ", ReportColConvertToDecimal(DrNetSalary))
                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",
                                SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooterAlign(0, "PASSED FOR PAYMENT OF BANK CONTRIBUTION TOWARDS PF  ", ReportColConvertToDecimal(DrProvidentFund))
                            };
                            lst.Add(crm);
                            //CommonReportModel tot = getTotal(branch, dt);
                            //tot.RowId = RowCnt++;
                            //lst.Add(tot);


                        }

                        //lst.Add(crm);

                        if (branch == "BRANCHES")
                        {
                        //crm = new CommonReportModel
                        //{
                        //    RowId = RowCnt++,
                        //    HRF = "H",
                        //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                        //   + ReportColHeaderValueOnly(0,dm)
                        //};
                        //lst.Add(crm);
                        string DrGrosssalary = dr["GrossSalary"].ToString();
                        if (DrGrosssalary == "")
                        {
                            DrGrosssalary = "0";
                        }
                        string DrProvidentFund = dr["ProvidentFund"].ToString();
                        if (DrProvidentFund == "")
                        {
                            DrProvidentFund = "0";
                        }
                        string DrNPS = dr["NPS"].ToString();
                        if (DrNPS == "")
                        {
                            DrNPS = "0";
                        }

                        string Drvpfbranch = dtvpfbranch.Rows[0]["VPF"].ToString();
                        if (Drvpfbranch == "")
                        {
                            Drvpfbranch = "0";
                        }
                        string DrTotalDeductions = dr["TotalDeductions"].ToString();
                        if (DrTotalDeductions == "")
                        {
                            DrTotalDeductions = "0";
                        }
                        string DrNetSalary = dr["NetSalary"].ToString();
                        if (DrNetSalary == "")
                        {
                            DrNetSalary = "0";
                        }
                        crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                //SlNo = SlNo++.ToString(),
                                HRF = "R",
                                SlNo = dr["Summary_Details"].ToString(),
                            // column2 = Math.Round(Convert.ToDecimal(dr["GrossSalary"]), 2).ToString(),
                            column2 = ReportColConvertToDecimal(DrGrosssalary),
                                column3 = ReportColConvertToDecimal(DrProvidentFund),
                                column7 = ReportColConvertToDecimal(DrNPS),
                                column4 = ReportColConvertToDecimal(Drvpfbranch),
                                column5 = ReportColConvertToDecimal(DrTotalDeductions),
                                column6 = ReportColConvertToDecimal(DrNetSalary),

                                //column2 = dr["GrossSalary"].ToString(),
                                //column3 = dr["ProvidentFund"].ToString(),
                                //column4 = dr["VPF"].ToString(),
                                //column5 = dr["TotalDeductions"].ToString(),
                                //column6 = dr["NetSalary"].ToString(),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",
                                SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooterAlign(0, "PASSED FOR PAYMENT OF GROSS AMOUNT OF    Rs  ", ReportColConvertToDecimal(DrGrosssalary))
                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",
                                SlNo = "<span style='color:#eef8fd'>^</span>"
                             + ReportColFooterAlign(0, "FOR PAYMENT OF NET   AMOUNT OF    Rs  ", ReportColConvertToDecimal(DrNetSalary))
                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",
                                SlNo = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooterAlign(0, "PASSED FOR PAYMENT OF BANK CONTRIBUTION  TOWARDS PF  ", ReportColConvertToDecimal(DrProvidentFund))
                            };
                            //CommonReportModel tot = getTotal(branch, dt);
                            //tot.RowId = RowCnt++;
                            //lst.Add(tot);

                            lst.Add(crm);
                        }
                    }
                 
                }
           
           return lst;
           // return await _sha.Get_Table_FromQry(query);
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
        public string ReportColConvertToDecimal( string value)
        {
           

            decimal Drvalue = Convert.ToDecimal(value.ToString()) + 0.00M;
            decimal DPT = Convert.ToDecimal(String.Format("{0:0.00}", Drvalue));
            string NwDPT = String.Format("{0:n}", DPT);


            return NwDPT;
        }


    }
}
