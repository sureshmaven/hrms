using System;
using System.Collections.Generic;
using Mavensoft.DAL.Business;
using System.Text;
using System.Threading.Tasks;
using PayrollModels;
using System.Data;
using System.Linq;

namespace PayRollBusiness.Reports
{
    public class 
        YearlyPayBusiness : BusinessBase
    {
        public YearlyPayBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        //drop down financial year eg:2019-2020
        public async Task<IList<LICReport>> getFy()
        {
            string qryfy = "select year(fm) as fm_fy from pr_month_details where active=1;";
            int fm_fy = await _sha.Run_INS_ExecuteScalar(qryfy);

            IList<LICReport> fyear = new List<LICReport>();

            int fy = fm_fy + 1;
            int Id = 0;

            fyear.Add(new LICReport
            {
                Id = Id.ToString(),
                fY = "Select",

            });

            Id++;
            fyear.Add(new LICReport
            {

                Id = Id.ToString(),
                fY = (fm_fy + "-" + (fy)).ToString(),

            });

            for (int i = 1; i < 6; i++)
            {
                Id++;
                fm_fy--;
                fy--;
                fyear.Add(new LICReport
                {
                    Id = Id.ToString(),
                    fY = (fm_fy + "-" + (fy)).ToString(),

                });
            }

            return fyear;

        }
        IList<CommonReportModel> lst = new List<CommonReportModel>();

        public async Task<IList<CommonReportModel>> GetYearlypayData(string empcode, string fYear)
        {
            int Eyear = 0001;
            int Fyear = 0001;
            if (fYear != null && fYear != "^2")
            {
                Eyear = int.Parse(fYear);
                Fyear = int.Parse(fYear) - 1;
            }
            if (empcode.Contains("^"))
            {
                empcode = "0";
                fYear = "1900";
            }
            string Qry1 = "";
            string Qry2 = "";
            string Qry3 = "";
            int RowCnt = 0;
            double basicALLow = 0;
            double allDeduct = 0;
            string sampleEmp = "";
            string sname = "";
            string sdept = "";
            string oldempid = "";
            string smnth = "";
            string oldmnth1 = "";
            string newmnth1 = "";
            string oldemp_code = "";
            string newemp_code = "";
            double basic1 = 0, basic2 = 0, basic3 = 0, basic4 = 0, basic5 = 0, basic6 = 0, basic7 = 0, basic8 = 0, basic9 = 0, basic10 = 0, basic11 = 0, basic12 = 0, basic13 = 0;
            double deduc1 = 0, deduc2 = 0, deduc3 = 0, deduc4 = 0, deduc5 = 0, deduc6 = 0, deduc7 = 0, deduc8 = 0, deduc9 = 0, deduc10 = 0, deduc11 = 0, deduc12 = 0, deduc13 = 0, tot_deduct = 0;
            double basicG1 = 0, basicG2 = 0, basicG3 = 0, basicG4 = 0, basicG5 = 0, basicG6 = 0, basicG7 = 0, basicG8 = 0, basicG9 = 0, basicG10 = 0, basicG11 = 0, basicG12 = 0, basicG13 = 0;
            double Allow1 = 0, Allow2 = 0, Allow3 = 0, Allow4 = 0, Allow5 = 0, Allow6 = 0, Allow7 = 0, Allow8 = 0, Allow9 = 0, Allow10 = 0, Allow11 = 0, Allow12 = 0, Allow13 = 0, tot_allow = 0;
            double basicB1 = 0, basicB2 = 0, basicB3 = 0, basicB4 = 0, basicB5 = 0, basicB6 = 0, basicB7 = 0, basicB8 = 0, basicB9 = 0, basicB10 = 0, basicB11 = 0, basicB12 = 0, basicB13 = 0;
            double basicD1 = 0, basicD2 = 0, basicD3 = 0, basicD4 = 0, basicD5 = 0, basicD6 = 0, basicD7 = 0, basicD8 = 0, basicD9 = 0, basicD10 = 0, basicD11 = 0, basicD12 = 0, basicD13 = 0;
            double basicC1 = 0, basicC2 = 0, basicC3 = 0, basicC4 = 0, basicC5 = 0, basicC6 = 0, basicC7 = 0, basicC8 = 0, basicC9 = 0, basicC10 = 0, basicC11 = 0, basicC12 = 0, basicC13 = 0;
            double basicH1 = 0, basicH2 = 0, basicH3 = 0, basicH4 = 0, basicH5 = 0, basicH6 = 0, basicH7 = 0, basicH8 = 0, basicH9 = 0, basicH10 = 0, basicH11 = 0, basicH12 = 0, basicH13 = 0;
            double basicI1 = 0, basicI2 = 0, basicI3 = 0, basicI4 = 0, basicI5 = 0, basicI6 = 0, basicI7 = 0, basicI8 = 0, basicI9 = 0, basicI10 = 0, basicI11 = 0, basicI12 = 0, basicI13 = 0;
            double basicT1 = 0, basicT2 = 0, basicT3 = 0, basicT4 = 0, basicT5 = 0, basicT6 = 0, basicT7 = 0, basicT8 = 0, basicT9 = 0, basicT10 = 0, basicT11 = 0, basicT12 = 0, basicT13 = 0;
            double basicSDA1 = 0, basicSDA2 = 0, basicSDA3 = 0, basicSDA4 = 0, basicSDA5 = 0, basicSDA6 = 0, basicSDA7 = 0, basicSDA8 = 0, basicSDA9 = 0, basicSDA10 = 0, basicSDA11 = 0, basicSDA12 = 0, basicSDA13 = 0;
            double basicSAlw1 = 0, basicSAlw2 = 0, basicSAlw3 = 0, basicSAlw4 = 0, basicSAlw5 = 0, basicSAlw6 = 0, basicSAlw7 = 0, basicSAlw8 = 0, basicSAlw9 = 0, basicSAlw10 = 0, basicSAlw11 = 0, basicSAlw12 = 0, basicSAlw13 = 0;
            string newbasic = "";
            string oldbasic = "";
            



            string cond1 = " order by  P.emp_code, P.fm,E.ShortName,D.Name ";
            string cond2 = " group by P.emp_code, P.fm,E.ShortName,D.Name, all_name, all_amount ";
            string cond3 = " order by P.emp_code, P.fm,E.ShortName,D.Name ";
            //D.Name as Department,            
            Qry1 = " select P.emp_code, E.ShortName,case D.Name when 'OtherDepartment' then b.Name else d.Name end  as Department,P.fm,P.er_basic as Basic,P.er_da as DA,P.er_cca as CCA,P.er_hra as HRA,P.er_interim_relief as INTERIM_RELIEF,P.er_telangana_inc as Telangana_Increment,P.spl_da as spl_da ,P.spl_allw as spl_allw ," +
                "P.gross_amount as gross_amount,P.net_amount as net_amount,P.dd_provident_fund as dd_provident_fund,P.dd_income_tax as dd_income_tax,P.dd_prof_tax as dd_prof_tax ,P.dd_club_subscription as dd_club_subscription," +
                "P.dd_telangana_officers_assn as dd_telangana_officers_assn, P.deductions_amount as deductions_amount from pr_emp_payslip P" +
                " join Employees E on P.emp_code = E.EmpId join Departments D ON D.id = E.Department" +
                "  join Branches  b on b.Id=e.Branch " +
                " where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 )";

            string allallowQry2 = " select distinct name from ( select distinct name as name from pr_allowance_field_master  union all select distinct name from pr_earn_field_master) as x ";
            Qry2 = " select P.emp_code, E.ShortName as ShortName,D.Name as Department,P.fm,all_name, all_amount, sum(all_amount) as all_tot from pr_emp_payslip_allowance PA join pr_emp_payslip P on pa.payslip_mid=p.id join Employees E on P.emp_code= E.EmpId join Departments D ON D.id= E.Department where P.fm between DATEFROMPARTS (" + Fyear + ", 04, 01 ) and DATEFROMPARTS (" + Eyear + ", 03, 31 ) and all_amount is not null ";
            string alldeducQry3 = " select distinct name from ( select distinct loan_description as name from pr_loan_master union all select distinct name from pr_deduction_field_master ) as x ";
            Qry3 = " select P.emp_code, E.ShortName,D.Name as Department,P.fm,dd_name,dd_amount from pr_emp_payslip_deductions PA join pr_emp_payslip P on PA.payslip_mid = P.id join Employees E on P.emp_code= E.EmpId join Departments D ON D.id= E.Department where P.fm between DATEFROMPARTS (" + Fyear + ", 04, 01 ) and DATEFROMPARTS (" + Eyear + ", 03, 31 ) and dd_amount is not null ";


            if (empcode != "All")
            {
                Qry1 += " AND P.emp_code in ( " + empcode + ")";
                Qry2 += " AND P.emp_code in ( " + empcode + ")";
                Qry3 += " AND P.emp_code in ( " + empcode + ")";
                //earnTotalQr += " where P.emp_code in(" + empcode + ")";
            }
            Qry1 += cond1;
            Qry2 += cond2;
            Qry3 += cond3;


            DataSet ds = await _sha.Get_MultiTables_FromQry(Qry1 + Qry2 + Qry3 + allallowQry2 + alldeducQry3);
            DataTable dtBasic = ds.Tables[0];
            DataTable dtAllow = ds.Tables[1];
            DataTable dtDeducts = ds.Tables[2];
            DataTable dtAllAllow = ds.Tables[3];
            DataTable dtAllDeduct = ds.Tables[4];


            var lstEmps = dtBasic.Rows.Cast<DataRow>().Select(x => new { empid = x["emp_code"].ToString(), name = x["ShortName"].ToString(), Department = x["Department"].ToString()}).Distinct();


            try
            {
                //loop each employess in basic table(since all employees have basics, da, cca.. data)
                foreach (var e in lstEmps)
                {
                    sampleEmp = e.empid;
                    sname = e.name;
                    sdept = e.Department;
                    //smnth = e.mnth;
                    if (oldempid != sampleEmp)
                    {
                        //if new emp - grouging row
                        lst.Add(new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            column1 = "<span style='color:#C8EAFB'>~</span>"
                            + ReportColHeader(0, "Emp Code", sampleEmp)
                            + ReportColHeader(10, "Emp Name", sname)
                            + ReportColHeader(10, "Department", sdept)

                        });
                        //Data header
                        lst.Add(new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            column1 = "Particulars ",
                            column2 = "APR " + Fyear,
                            column3 = "MAY " + Fyear,
                            column4 = "JUN " + Fyear,
                            column5 = "JUL " + Fyear,
                            column6 = "AUG " + Fyear,
                            column7 = "SEP " + Fyear,
                            column8 = "OCT " + Fyear,
                            column9 = "NOV " + Fyear,
                            column10 = "DEC " + Fyear,
                            column11 = "JAN " + Eyear,
                            column12 = "FEB " + Eyear,
                            column13 = "MAR " + Eyear,
                            column14 = "Total "
                        });
                    }

                    #region Basic, da, cca....etc

                    //get basics, da, cca.. data
                    var empBrows = dtBasic.Rows.Cast<DataRow>().Where(x => x["emp_code"].ToString() == sampleEmp );

                   

                    //1.basic
                    bool flgbasic = false;
                    var basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "Basic",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        basic13 = 0;
                        if ((newbasic == oldbasic )&& (newmnth1 == oldmnth1 ))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgbasic = true;
                                    if (mn == 1) { basicB1 = basicB1 + Convert.ToDouble(empBrow["Basic"]); basicItem.column11 = ReportColConvertToDecimal(basicB1.ToString()); }
                                    if (mn == 2) { basicB2 = basic2 + Convert.ToDouble(empBrow["Basic"]); basicItem.column12 = ReportColConvertToDecimal(basicB2.ToString()); }
                                    if (mn == 3) { basicB3 = basicB3 + Convert.ToDouble(empBrow["Basic"]); basicItem.column13 = ReportColConvertToDecimal(basicB3.ToString()); }
                                    if (mn == 4) { basicB4 = basicB4 + Convert.ToDouble(empBrow["Basic"]); basicItem.column2 = ReportColConvertToDecimal(basicB4.ToString()); }
                                    if (mn == 5) { basicB5 = basicB5 + Convert.ToDouble(empBrow["Basic"]); basicItem.column3 = ReportColConvertToDecimal(basicB5.ToString()); }
                                    if (mn == 6) { basicB6 = basicB6 + Convert.ToDouble(empBrow["Basic"]); basicItem.column4 = ReportColConvertToDecimal(basicB6.ToString()); }
                                    if (mn == 7) { basicB7 = basicB7 + Convert.ToDouble(empBrow["Basic"]); basicItem.column5 = ReportColConvertToDecimal(basicB7.ToString()); }
                                    if (mn == 8) { basicB8 = basicB8 + Convert.ToDouble(empBrow["Basic"]); basicItem.column6 = ReportColConvertToDecimal(basicB8.ToString()); }
                                    if (mn == 9) { basicB9 = basicB9 + Convert.ToDouble(empBrow["Basic"]); basicItem.column7 = ReportColConvertToDecimal(basicB9.ToString()); }
                                    if (mn == 10) { basicB10 = basicB10 + Convert.ToDouble(empBrow["Basic"]); basicItem.column8 = ReportColConvertToDecimal(basicB10.ToString()); }
                                    if (mn == 11) { basicB11 = basicB11 + Convert.ToDouble(empBrow["Basic"]); basicItem.column9 = ReportColConvertToDecimal(basicB11.ToString()); }
                                    if (mn == 12) { basicB12 = basicB12 + Convert.ToDouble(empBrow["Basic"]); basicItem.column10 =ReportColConvertToDecimal(basicB12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgbasic = true;
                                    if (mn == 1) { basicB1 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column11 = ReportColConvertToDecimal(basicB1.ToString()); }
                                    if (mn == 2) { basicB2 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column12 = ReportColConvertToDecimal(basicB2.ToString()); }
                                    if (mn == 3) { basicB3 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column13 = ReportColConvertToDecimal(basicB3.ToString()); }
                                    if (mn == 4) { basicB4 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column2 = ReportColConvertToDecimal(basicB4.ToString()); }
                                    if (mn == 5) { basicB5 = Convert.ToDouble(empBrow["Basic"]); basicItem.column3 = ReportColConvertToDecimal(basicB5.ToString()); }
                                    if (mn == 6) { basicB6 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column4 = ReportColConvertToDecimal(basicB6.ToString()); }
                                    if (mn == 7) { basicB7 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column5 = ReportColConvertToDecimal(basicB7.ToString()); }
                                    if (mn == 8) { basicB8 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column6 = ReportColConvertToDecimal(basicB8.ToString()); }
                                    if (mn == 9) { basicB9 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column7 = ReportColConvertToDecimal(basicB9.ToString()); }
                                    if (mn == 10) { basicB10 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column8 = ReportColConvertToDecimal(basicB10.ToString()); }
                                    if (mn == 11) { basicB11 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column9 = ReportColConvertToDecimal(basicB11.ToString()); }
                                    if (mn == 12) { basicB12 =  Convert.ToDouble(empBrow["Basic"]); basicItem.column10 = ReportColConvertToDecimal(basicB12.ToString()); }
                                }
                            }
                        }

                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicB13 = (basicB1 + basicB2 + basicB3 + basicB4 + basicB5 + basicB6 + basicB7 + basicB8 + basicB9 + basicB10 + basicB11 + basicB12);
                        basicItem.column14 = ReportColConvertToDecimal(basicB13.ToString());

                    }
                    if (flgbasic) { lst.Add(basicItem); }

                    //2.DA
                    bool flgDa = true;
                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "DA",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        basic13 = 0;
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgDa = true;
                                    if (mn == 1) { basicD1 = basicD1 + Convert.ToDouble(empBrow["DA"]); basicItem.column11 = ReportColConvertToDecimal(basicD1.ToString()); }
                                    if (mn == 2) { basicD2 = basicD2 + Convert.ToDouble(empBrow["DA"]); basicItem.column12 = ReportColConvertToDecimal(basicD2.ToString()); }
                                    if (mn == 3) { basicD3 = basicD3 + Convert.ToDouble(empBrow["DA"]); basicItem.column13 = ReportColConvertToDecimal(basicD3.ToString()); }
                                    if (mn == 4) { basicD4 = basicD4 + Convert.ToDouble(empBrow["DA"]); basicItem.column2 = ReportColConvertToDecimal(basicD4.ToString()); }
                                    if (mn == 5) { basicD5 = basicD5 + Convert.ToDouble(empBrow["DA"]); basicItem.column3 = ReportColConvertToDecimal(basicD5.ToString()); }
                                    if (mn == 6) { basicD6 = basicD6 + Convert.ToDouble(empBrow["DA"]); basicItem.column4 = ReportColConvertToDecimal(basicD6.ToString()); }
                                    if (mn == 7) { basicD7 = basicD7 + Convert.ToDouble(empBrow["DA"]); basicItem.column5 = ReportColConvertToDecimal(basicD7.ToString()); }
                                    if (mn == 8) { basicD8 = basicD8 + Convert.ToDouble(empBrow["DA"]); basicItem.column6 = ReportColConvertToDecimal(basicD8.ToString()); }
                                    if (mn == 9) { basicD9 = basicD9 + Convert.ToDouble(empBrow["DA"]); basicItem.column7 = ReportColConvertToDecimal(basicD9.ToString()); }
                                    if (mn == 10) { basicD10 = basicD10 + Convert.ToDouble(empBrow["DA"]); basicItem.column8 = ReportColConvertToDecimal(basicD10.ToString()); }
                                    if (mn == 11) { basicD11 = basicD11 + Convert.ToDouble(empBrow["DA"]); basicItem.column9 = ReportColConvertToDecimal(basicD11.ToString()); }
                                    if (mn == 12) { basicD12 = basicD12 + Convert.ToDouble(empBrow["DA"]); basicItem.column10 = ReportColConvertToDecimal(basicD12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgDa = true;
                                    if (mn == 1) { basicD1 = Convert.ToDouble(empBrow["DA"]); basicItem.column11 = ReportColConvertToDecimal(basicD1.ToString()); }
                                    if (mn == 2) { basicD2 =  Convert.ToDouble(empBrow["DA"]); basicItem.column12 = ReportColConvertToDecimal(basicD2.ToString()); }
                                    if (mn == 3) { basicD3 = Convert.ToDouble(empBrow["DA"]); basicItem.column13 = ReportColConvertToDecimal(basicD3.ToString()); }
                                    if (mn == 4) { basicD4 =  Convert.ToDouble(empBrow["DA"]); basicItem.column2 = ReportColConvertToDecimal(basicD4.ToString()); }
                                    if (mn == 5) { basicD5 = Convert.ToDouble(empBrow["DA"]); basicItem.column3 = ReportColConvertToDecimal(basicD5.ToString()); }
                                    if (mn == 6) { basicD6 =  Convert.ToDouble(empBrow["DA"]); basicItem.column4 = ReportColConvertToDecimal(basicD6.ToString()); }
                                    if (mn == 7) { basicD7 =  Convert.ToDouble(empBrow["DA"]); basicItem.column5 = ReportColConvertToDecimal(basicD7.ToString()); }
                                    if (mn == 8) { basicD8 = Convert.ToDouble(empBrow["DA"]); basicItem.column6 = ReportColConvertToDecimal(basicD8.ToString()); }
                                    if (mn == 9) { basicD9 =  Convert.ToDouble(empBrow["DA"]); basicItem.column7 = ReportColConvertToDecimal(basicD9.ToString()); }
                                    if (mn == 10) { basicD10 =  Convert.ToDouble(empBrow["DA"]); basicItem.column8 = ReportColConvertToDecimal(basicD10.ToString()); }
                                    if (mn == 11) { basicD11 =  Convert.ToDouble(empBrow["DA"]); basicItem.column9 = ReportColConvertToDecimal(basicD11.ToString()); }
                                    if (mn == 12) { basicD12 =  Convert.ToDouble(empBrow["DA"]); basicItem.column10 = ReportColConvertToDecimal(basicD12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicD13 = (basicD1 + basicD2 + basicD3 + basicD4 + basicD5 + basicD6 + basicD7 + basicD8 + basicD9 + basicD10 + basicD11 + basicD12);
                        basicItem.column14 = ReportColConvertToDecimal(basicD13.ToString());

                    }
                    if (flgDa) { lst.Add(basicItem); }
                   
                    //CCA
                    bool flgCCA = false;

                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "CCA",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        basic13 = 0;
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgCCA = true;
                                    if (mn == 1) { basicC1 = basicC1 + Convert.ToDouble(empBrow["CCA"]); basicItem.column11 = ReportColConvertToDecimal(basicC1.ToString()); }
                                    if (mn == 2) { basicC2 = basicC2 + Convert.ToDouble(empBrow["CCA"]); basicItem.column12 = ReportColConvertToDecimal(basicC2.ToString()); }
                                    if (mn == 3) { basicC3 = basicC3 + Convert.ToDouble(empBrow["CCA"]); basicItem.column13 = ReportColConvertToDecimal(basicC3.ToString()); }
                                    if (mn == 4) { basicC4 = basicC4 + Convert.ToDouble(empBrow["CCA"]); basicItem.column2 = ReportColConvertToDecimal(basicC4.ToString()); }
                                    if (mn == 5) { basicC5 = basicC5 + Convert.ToDouble(empBrow["CCA"]); basicItem.column3 = ReportColConvertToDecimal(basicC5.ToString()); }
                                    if (mn == 6) { basicC6 = basicC6 + Convert.ToDouble(empBrow["CCA"]); basicItem.column4 = ReportColConvertToDecimal(basicC6.ToString()); }
                                    if (mn == 7) { basicC7 = basicC7 + Convert.ToDouble(empBrow["CCA"]); basicItem.column5 = ReportColConvertToDecimal(basicC7.ToString()); }
                                    if (mn == 8) { basicC8 = basicC8 + Convert.ToDouble(empBrow["CCA"]); basicItem.column6 = ReportColConvertToDecimal(basicC8.ToString()); }
                                    if (mn == 9) { basicC9 = basicC9 + Convert.ToDouble(empBrow["CCA"]); basicItem.column7 = ReportColConvertToDecimal(basicC9.ToString()); }
                                    if (mn == 10) { basicC10 = basicC10 + Convert.ToDouble(empBrow["CCA"]); basicItem.column8 = ReportColConvertToDecimal(basicC10.ToString()); }
                                    if (mn == 11) { basicC11 = basicC11 + Convert.ToDouble(empBrow["CCA"]); basicItem.column9 = ReportColConvertToDecimal(basicC11.ToString()); }
                                    if (mn == 12) { basicC12 = basicC12 + Convert.ToDouble(empBrow["CCA"]); basicItem.column10 = ReportColConvertToDecimal(basicC12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgCCA = true;
                                    if (mn == 1) { basicC1 = Convert.ToDouble(empBrow["CCA"]); basicItem.column11 = ReportColConvertToDecimal(basicC1.ToString()); }
                                    if (mn == 2) { basicC2 =  Convert.ToDouble(empBrow["CCA"]); basicItem.column12 = ReportColConvertToDecimal(basicC2.ToString()); }
                                    if (mn == 3) { basicC3 =  Convert.ToDouble(empBrow["CCA"]); basicItem.column13 = ReportColConvertToDecimal(basicC3.ToString()); }
                                    if (mn == 4) { basicC4 =  Convert.ToDouble(empBrow["CCA"]); basicItem.column2 = ReportColConvertToDecimal(basicC4.ToString()); }
                                    if (mn == 5) { basicC5 = Convert.ToDouble(empBrow["CCA"]); basicItem.column3 = ReportColConvertToDecimal(basicC5.ToString()); }
                                    if (mn == 6) { basicC6 = Convert.ToDouble(empBrow["CCA"]); basicItem.column4 = ReportColConvertToDecimal(basicC6.ToString()); }
                                    if (mn == 7) { basicC7 = Convert.ToDouble(empBrow["CCA"]); basicItem.column5 = ReportColConvertToDecimal(basicC7.ToString()); }
                                    if (mn == 8) { basicC8 = Convert.ToDouble(empBrow["CCA"]); basicItem.column6 = ReportColConvertToDecimal(basicC8.ToString()); }
                                    if (mn == 9) { basicC9 = Convert.ToDouble(empBrow["CCA"]); basicItem.column7 = ReportColConvertToDecimal(basicC9.ToString()); }
                                    if (mn == 10) { basicC10 = Convert.ToDouble(empBrow["CCA"]); basicItem.column8 = ReportColConvertToDecimal(basicC10.ToString()); }
                                    if (mn == 11) { basicC11 = Convert.ToDouble(empBrow["CCA"]); basicItem.column9 = ReportColConvertToDecimal(basicC11.ToString()); }
                                    if (mn == 12) { basicC12 = Convert.ToDouble(empBrow["CCA"]); basicItem.column10 = ReportColConvertToDecimal(basicC12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicC13 = (basicC1 + basicC2 + basicC3 + basicC4 + basicC5 + basicC6 + basicC7 + basicC8 + basicC9 + basicC10 + basicC11 + basicC12);
                        basicItem.column14 = ReportColConvertToDecimal(basicC13.ToString());
                    }
                    if (flgCCA) { lst.Add(basicItem); }

                    //HRA
                    bool flgHRA = false;

                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "HRA",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                        //Annual Increment
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        basicH13 = 0;
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgHRA = true;
                                    if (mn == 1) { basicH1 = basicH1 + Convert.ToDouble(empBrow["HRA"]); basicItem.column11 = ReportColConvertToDecimal(basicH1.ToString()); }
                                    if (mn == 2) { basicH2 = basicH2 + Convert.ToDouble(empBrow["HRA"]); basicItem.column12 = ReportColConvertToDecimal(basicH2.ToString()); }
                                    if (mn == 3) { basicH3 = basicH3 + Convert.ToDouble(empBrow["HRA"]); basicItem.column13 = ReportColConvertToDecimal(basicH3.ToString()); }
                                    if (mn == 4) { basicH4 = basicH4 + Convert.ToDouble(empBrow["HRA"]); basicItem.column2 = ReportColConvertToDecimal(basicH4.ToString()); }
                                    if (mn == 5) { basicH5 = basicH5 + Convert.ToDouble(empBrow["HRA"]); basicItem.column3 = ReportColConvertToDecimal(basicH5.ToString()); }
                                    if (mn == 6) { basicH6 = basicH6 + Convert.ToDouble(empBrow["HRA"]); basicItem.column4 = ReportColConvertToDecimal(basicH6.ToString()); }
                                    if (mn == 7) { basicH7 = basicH7 + Convert.ToDouble(empBrow["HRA"]); basicItem.column5 = ReportColConvertToDecimal(basicH7.ToString()); }
                                    if (mn == 8) { basicH8 = basicH8 + Convert.ToDouble(empBrow["HRA"]); basicItem.column6 = ReportColConvertToDecimal(basicH8.ToString()); }
                                    if (mn == 9) { basicH9 = basicH9 + Convert.ToDouble(empBrow["HRA"]); basicItem.column7 = ReportColConvertToDecimal(basicH9.ToString()); }
                                    if (mn == 10) { basicH10 = basicH10 + Convert.ToDouble(empBrow["HRA"]); basicItem.column8 = ReportColConvertToDecimal(basicH10.ToString()); }
                                    if (mn == 11) { basicH11 = basicH11 + Convert.ToDouble(empBrow["HRA"]); basicItem.column9 = ReportColConvertToDecimal(basicH11.ToString()); }
                                    if (mn == 12) { basicH12 = basicH12 + Convert.ToDouble(empBrow["HRA"]); basicItem.column10 = ReportColConvertToDecimal(basicH12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgHRA = true;
                                    if (mn == 1) { basicH1 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column11 = ReportColConvertToDecimal(basicH1.ToString()); }
                                    if (mn == 2) { basicH2 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column12 = ReportColConvertToDecimal(basicH2.ToString()); }
                                    if (mn == 3) { basicH3 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column13 = ReportColConvertToDecimal(basicH3.ToString()); }
                                    if (mn == 4) { basicH4 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column2 = ReportColConvertToDecimal(basicH4.ToString()); }
                                    if (mn == 5) { basicH5 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column3 = ReportColConvertToDecimal(basicH5.ToString()); }
                                    if (mn == 6) { basicH6 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column4 = ReportColConvertToDecimal(basicH6.ToString()); }
                                    if (mn == 7) { basicH7 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column5 = ReportColConvertToDecimal(basicH7.ToString()); }
                                    if (mn == 8) { basicH8 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column6 = ReportColConvertToDecimal(basicH8.ToString()); }
                                    if (mn == 9) { basicH9 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column7 = ReportColConvertToDecimal(basicH9.ToString()); }
                                    if (mn == 10) { basicH10 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column8 = ReportColConvertToDecimal(basicH10.ToString()); }
                                    if (mn == 11) { basicH11 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column9 = ReportColConvertToDecimal(basicH11.ToString()); }
                                    if (mn == 12) { basicH12 =  Convert.ToDouble(empBrow["HRA"]); basicItem.column10 = ReportColConvertToDecimal(basicH12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicH13 = (basicH1 + basicH2 + basicH3 + basicH4 + basicH5 + basicH6 + basicH7 + basicH8 + basicH9 + basicH10 + basicH11 + basicH12);
                        basicItem.column14 = ReportColConvertToDecimal(basicH13.ToString());
                    }
                    if (flgHRA) { lst.Add(basicItem); }

                    //IT
                    bool flgIT = false;
                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";

                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "INTERIM_RELIEF",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        basicI13 = 0;
                        if ((newbasic == oldbasic ) && (newmnth1 == oldmnth1 ))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgIT = true;
                                    if (mn == 1) { basicI1 = basicI1 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column11 = ReportColConvertToDecimal(basicI1.ToString()); }
                                    if (mn == 2) { basicI2 = basicI2 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column12 = ReportColConvertToDecimal(basicI2.ToString()); }
                                    if (mn == 3) { basicI3 = basicI3 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column13 = ReportColConvertToDecimal(basicI3.ToString()); }
                                    if (mn == 4) { basicI4 = basicI4 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column2 = ReportColConvertToDecimal(basicI4.ToString()); }
                                    if (mn == 5) { basicI5 = basicI5 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column3 = ReportColConvertToDecimal(basicI5.ToString()); }
                                    if (mn == 6) { basicI6 = basicI6 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column4 = ReportColConvertToDecimal(basicI6.ToString()); }
                                    if (mn == 7) { basicI7 = basicI7 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column5 = ReportColConvertToDecimal(basicI7.ToString()); }
                                    if (mn == 8) { basicI8 = basicI8 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column6 = ReportColConvertToDecimal(basicI8.ToString()); }
                                    if (mn == 9) { basicI9 = basicI9 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column7 = ReportColConvertToDecimal(basicI9.ToString()); }
                                    if (mn == 10) { basicI10 = basicI10 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column8 = ReportColConvertToDecimal(basicI10.ToString()); }
                                    if (mn == 11) { basicI11 = basicI11 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column9 = ReportColConvertToDecimal(basicI11.ToString()); }
                                    if (mn == 12) { basicI12 = basicI12 + Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column10 = ReportColConvertToDecimal(basicI12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgIT = true;
                                    if (mn == 1) { basicI1 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column11 = ReportColConvertToDecimal(basicI1.ToString()); }
                                    if (mn == 2) { basicI2 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column12 = ReportColConvertToDecimal(basicI2.ToString()); }
                                    if (mn == 3) { basicI3 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column13 = ReportColConvertToDecimal(basicI3.ToString()); }
                                    if (mn == 4) { basicI4 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column2 = ReportColConvertToDecimal(basicI4.ToString()); }
                                    if (mn == 5) { basicI5 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column3 = ReportColConvertToDecimal(basicI5.ToString()); }
                                    if (mn == 6) { basicI6 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column4 = ReportColConvertToDecimal(basicI6.ToString()); }
                                    if (mn == 7) { basicI7 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column5 = ReportColConvertToDecimal(basicI7.ToString()); }
                                    if (mn == 8) { basicI8 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column6 = ReportColConvertToDecimal(basicI8.ToString()); }
                                    if (mn == 9) { basicI9 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column7 = ReportColConvertToDecimal(basicI9.ToString()); }
                                    if (mn == 10) { basicI10 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column8 = ReportColConvertToDecimal(basicI10.ToString()); }
                                    if (mn == 11) { basicI11 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column9 = ReportColConvertToDecimal(basicI11.ToString()); }
                                    if (mn == 12) { basicI12 = Convert.ToDouble(empBrow["INTERIM_RELIEF"]); basicItem.column10 = ReportColConvertToDecimal(basicI12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicI13 = (basicI1 + basicI2 + basicI3 + basicI4 + basicI5 + basicI6 + basicI7 + basicI8 + basicI9 + basicI10 + basicI11 + basicI12);
                        basicItem.column14 = ReportColConvertToDecimal(basicI13.ToString());

                    }
                    if (flgIT) { lst.Add(basicItem); }

                    //Telangana incr
                    bool flgTelI = false;

                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "Telangana_Increment",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        basicT13 = 0;
                        if ((newbasic == oldbasic ) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgTelI = true;
                                    if (mn == 1) { basicT1 = basicT1 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column11 = ReportColConvertToDecimal(basicT1.ToString()); }
                                    if (mn == 2) { basicT2 = basicT2 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column12 = ReportColConvertToDecimal(basicT2.ToString()); }
                                    if (mn == 3) { basicT3 = basicT3 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column13 = ReportColConvertToDecimal(basicT3.ToString()); }
                                    if (mn == 4) { basicT4 = basicT4 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column2 = ReportColConvertToDecimal(basicT4.ToString()); }
                                    if (mn == 5) { basicT5 = basicT5 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column3 = ReportColConvertToDecimal(basicT5.ToString()); }
                                    if (mn == 6) { basicT6 = basicT6 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column4 = ReportColConvertToDecimal(basicT6.ToString()); }
                                    if (mn == 7) { basicT7 = basicT7 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column5 = ReportColConvertToDecimal(basicT7.ToString()); }
                                    if (mn == 8) { basicT8 = basicT8 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column6 = ReportColConvertToDecimal(basicT8.ToString()); }
                                    if (mn == 9) { basicT9 = basicT9 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column7 = ReportColConvertToDecimal(basicT9.ToString()); }
                                    if (mn == 10) { basicT10 = basicT10 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column8 = ReportColConvertToDecimal(basicT10.ToString()); }
                                    if (mn == 11) { basicT11 = basicT11 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column9 = ReportColConvertToDecimal(basicT11.ToString()); }
                                    if (mn == 12) { basicT12 = basicT12 + Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column10 = ReportColConvertToDecimal(basicT12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgTelI = true;
                                    if (mn == 1) { basicT1 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column11 = ReportColConvertToDecimal(basicT1.ToString()); }
                                    if (mn == 2) { basicT2 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column12 = ReportColConvertToDecimal(basicT2.ToString()); }
                                    if (mn == 3) { basicT3 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column13 = ReportColConvertToDecimal(basicT3.ToString()); }
                                    if (mn == 4) { basicT4 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column2 = ReportColConvertToDecimal(basicT4.ToString()); }
                                    if (mn == 5) { basicT5 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column3 = ReportColConvertToDecimal(basicT5.ToString()); }
                                    if (mn == 6) { basicT6 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column4 = ReportColConvertToDecimal(basicT6.ToString()); }
                                    if (mn == 7) { basicT7 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column5 = ReportColConvertToDecimal(basicT7.ToString()); }
                                    if (mn == 8) { basicT8 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column6 = ReportColConvertToDecimal(basicT8.ToString()); }
                                    if (mn == 9) { basicT9 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column7 = ReportColConvertToDecimal(basicT9.ToString()); }
                                    if (mn == 10) { basicT10 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column8 = ReportColConvertToDecimal(basicT10.ToString()); }
                                    if (mn == 11) { basicT11 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column9 = ReportColConvertToDecimal(basicT11.ToString()); }
                                    if (mn == 12) { basicT12 = Convert.ToDouble(empBrow["Telangana_Increment"]); basicItem.column10 = ReportColConvertToDecimal(basicT12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicT13 = (basicT1 + basicT2 + basicT3 + basicT4 + basicT5 + basicT6 + basicT7 + basicT8 + basicT9 + basicT10 + basicT11 + basicT12);
                        basicItem.column14 = ReportColConvertToDecimal(basicT13.ToString());
                    }

                    if (flgTelI) { lst.Add(basicItem); }
                    //Spl-DA
                    bool flgspl_da = false;

                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "spl_da",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        basicSDA13 = 0;
                        if ((newbasic == oldbasic ) && (newmnth1 == oldmnth1 ))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgspl_da = true;
                                    if (mn == 1) { basicSDA1 = basicSDA1 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column11 = ReportColConvertToDecimal(basicSDA1.ToString()); }
                                    if (mn == 2) { basicSDA2 = basicSDA2 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column12 = ReportColConvertToDecimal(basicSDA2.ToString()); }
                                    if (mn == 3) { basicSDA3 = basicSDA3 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column13 = ReportColConvertToDecimal(basicSDA3.ToString()); }
                                    if (mn == 4) { basicSDA4 = basicSDA4 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column2 = ReportColConvertToDecimal(basicSDA4.ToString()); }
                                    if (mn == 5) { basicSDA5 = basicSDA5 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column3 = ReportColConvertToDecimal(basicSDA5.ToString()); }
                                    if (mn == 6) { basicSDA6 = basicSDA6 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column4 = ReportColConvertToDecimal(basicSDA6.ToString()); }
                                    if (mn == 7) { basicSDA7 = basicSDA7 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column5 = ReportColConvertToDecimal(basicSDA7.ToString()); }
                                    if (mn == 8) { basicSDA8 = basicSDA8 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column6 = ReportColConvertToDecimal(basicSDA8.ToString()); }
                                    if (mn == 9) { basicSDA9 = basicSDA9 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column7 = ReportColConvertToDecimal(basicSDA9.ToString()); }
                                    if (mn == 10) { basicSDA10 = basicSDA10 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column8 = ReportColConvertToDecimal(basicSDA10.ToString()); }
                                    if (mn == 11) { basicSDA11 = basicSDA11 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column9 = ReportColConvertToDecimal(basicSDA11.ToString()); }
                                    if (mn == 12) { basicSDA12 = basicSDA12 + Convert.ToDouble(empBrow["spl_da"]); basicItem.column10 = ReportColConvertToDecimal(basicSDA12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgspl_da = true;
                                    if (mn == 1) { basicSDA1 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column11 = ReportColConvertToDecimal(basicSDA1.ToString()); }
                                    if (mn == 2) { basicSDA2 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column12 = ReportColConvertToDecimal(basicSDA2.ToString()); }
                                    if (mn == 3) { basicSDA3 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column13 = ReportColConvertToDecimal(basicSDA3.ToString()); }
                                    if (mn == 4) { basicSDA4 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column2 = ReportColConvertToDecimal(basicSDA4.ToString()); }
                                    if (mn == 5) { basicSDA5 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column3 = ReportColConvertToDecimal(basicSDA5.ToString()); }
                                    if (mn == 6) { basicSDA6 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column4 = ReportColConvertToDecimal(basicSDA6.ToString()); }
                                    if (mn == 7) { basicSDA7 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column5 = ReportColConvertToDecimal(basicSDA7.ToString()); }
                                    if (mn == 8) { basicSDA8 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column6 = ReportColConvertToDecimal(basicSDA8.ToString()); }
                                    if (mn == 9) { basicSDA9 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column7 = ReportColConvertToDecimal(basicSDA9.ToString()); }
                                    if (mn == 10) { basicSDA10 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column8 = ReportColConvertToDecimal(basicSDA10.ToString()); }
                                    if (mn == 11) { basicSDA11 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column9 = ReportColConvertToDecimal(basicSDA11.ToString()); }
                                    if (mn == 12) { basicSDA12 = Convert.ToDouble(empBrow["spl_da"]); basicItem.column10 = ReportColConvertToDecimal(basicSDA12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicSDA13 = (basicSDA1 + basicSDA2 + basicSDA3 + basicSDA4 + basicSDA5 + basicSDA6 + basicSDA7 + basicSDA8 + basicSDA9 + basicSDA10 + basicSDA11 + basicSDA12);
                        basicItem.column14 = ReportColConvertToDecimal(basicSDA13.ToString());
                    }
                    if (flgspl_da) { lst.Add(basicItem); }

                    // spl_allw

                    bool flgspl_allw = false;

                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "spl_allw",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        basicSAlw13 = 0;
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgspl_allw = true;
                                    if (mn == 1) { basicSAlw1 = basicT1 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = basicT2 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = basicT3 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = basicT4 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = basicT5 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = basicT6 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = basicT7 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = basicT8 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = basicT9 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = basicT10 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = basicT11 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = basicT12 + Convert.ToDouble(empBrow["spl_allw"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgspl_allw = true;
                                    if (mn == 1) { basicSAlw1 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = Convert.ToDouble(empBrow["spl_allw"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicSAlw13 = (basicSAlw1 + basicSAlw2 + basicSAlw3 + basicSAlw4 + basicSAlw5 + basicSAlw6 + basicSAlw7 + basicSAlw8 + basicSAlw9 + basicSAlw10 + basicSAlw11 + basicSAlw12);
                        basicItem.column14 = ReportColConvertToDecimal(basicSAlw13.ToString());
                    }
                    if (flgspl_allw) { lst.Add(basicItem); }
                    #endregion

                    #region Allowences


                    foreach (DataRow allwMast in dtAllAllow.Rows)
                    {
                        bool flgAllow = false;
                        basic1 = 0; basic2 = 0; basic3 = 0; basic4 = 0; basic5 = 0; basic6 = 0; basic7 = 0; basic8 = 0; basic9 = 0; basic10 = 0; basic11 = 0; basic12 = 0; basic13 = 0;
                        string allowanace_name = allwMast["name"].ToString();
                        basicItem = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            column1 = allowanace_name,
                            column2 = ReportColConvertToDecimal("0"),
                            column3 = ReportColConvertToDecimal("0"),
                            column4 = ReportColConvertToDecimal("0"),
                            column5 = ReportColConvertToDecimal("0"),
                            column6 = ReportColConvertToDecimal("0"),
                            column7 = ReportColConvertToDecimal("0"),
                            column8 = ReportColConvertToDecimal("0"),
                            column9 = ReportColConvertToDecimal("0"),
                            column10 = ReportColConvertToDecimal("0"),
                            column11 = ReportColConvertToDecimal("0"),
                            column12 = ReportColConvertToDecimal("0"),
                            column13 = ReportColConvertToDecimal("0"),
                            column14 = ReportColConvertToDecimal("0")

                        };

                        var empallrows = dtAllow.Rows.Cast<DataRow>().Where(x => x["emp_code"].ToString() == sampleEmp
                        &&  string.Equals(x["all_name"].ToString(), allowanace_name, StringComparison.OrdinalIgnoreCase));
                        foreach (DataRow empallowrow in empallrows)
                        {
                            basic13 = 0;
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empallowrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgAllow = true;
                                    if (mn == 1) { basic1 = basic1 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column11 = ReportColConvertToDecimal(basic1.ToString()); }
                                    if (mn == 2) { basic2 = basic2 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column12 = ReportColConvertToDecimal(basic2.ToString()); }
                                    if (mn == 3) { basic3 = basic3 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column13 = ReportColConvertToDecimal(basic3.ToString()); }
                                    if (mn == 4) { basic4 = basic4 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column2 = ReportColConvertToDecimal(basic4.ToString()); }
                                    if (mn == 5) { basic5 = basic5 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column3 = ReportColConvertToDecimal(basic5.ToString()); }
                                    if (mn == 6) { basic6 = basic6 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column4 = ReportColConvertToDecimal(basic6.ToString()); }
                                    if (mn == 7) { basic7 = basic7 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column5 = ReportColConvertToDecimal(basic7.ToString()); }
                                    if (mn == 8) { basic8 = basic8 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column6 = ReportColConvertToDecimal(basic8.ToString()); }
                                    if (mn == 9) { basic9 = basic9 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column7 = ReportColConvertToDecimal(basic9.ToString()); }
                                    if (mn == 10) { basic10 = basic10 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column8 = ReportColConvertToDecimal(basic10.ToString()); }
                                    if (mn == 11) { basic11 = basic11 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column9 = ReportColConvertToDecimal(basic11.ToString()); }
                                    if (mn == 12) { basic12 = basic12 + Convert.ToDouble(empallowrow["all_amount"]); basicItem.column10 = ReportColConvertToDecimal(basic12.ToString()); }
                                }
                            }
                            basic13 = (basic1 + basic2 + basic3 + basic4 + basic5 + basic6 + basic7 + basic8 + basic9 + basic10 + basic11 + basic12);
                            basicItem.column14 = ReportColConvertToDecimal(basic13.ToString());

                        }
                        if (flgAllow) { lst.Add(basicItem); }

                    }
                    #endregion

                    #region Gross Pay
                    //"Gross Pay"

                    bool flggross = false;
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "<span style='font-weight: bold'; 'font-size: 15px';>Gross Pay</span>",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")

                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        Allow13 = 0;
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1 ))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flggross = true;
                                    if (mn == 1) { Allow1 = Allow1 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column11 = ReportColConvertToDecimal(Allow1.ToString()); }
                                    if (mn == 2) { Allow2 = Allow2 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column12 = ReportColConvertToDecimal(Allow2.ToString()); }
                                    if (mn == 3) { Allow3 = Allow3 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column13 = ReportColConvertToDecimal(Allow3.ToString()); }
                                    if (mn == 4) { Allow4 = Allow4 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column2 = ReportColConvertToDecimal(Allow4.ToString()); }
                                    if (mn == 5) { Allow5 = Allow5 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column3 = ReportColConvertToDecimal(Allow5.ToString()); }
                                    if (mn == 6) { Allow6 = Allow6 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column4 = ReportColConvertToDecimal(Allow6.ToString()); }
                                    if (mn == 7) { Allow7 = Allow7 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column5 = ReportColConvertToDecimal(Allow7.ToString()); }
                                    if (mn == 8) { Allow8 = Allow8 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column6 = ReportColConvertToDecimal(Allow8.ToString()); }
                                    if (mn == 9) { Allow9 = Allow9 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column7 = ReportColConvertToDecimal(Allow9.ToString()); }
                                    if (mn == 10) { Allow10 = Allow10 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column8 = ReportColConvertToDecimal(Allow10.ToString()); }
                                    if (mn == 11) { Allow11 = Allow11 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column9 = ReportColConvertToDecimal(Allow11.ToString()); }
                                    if (mn == 12) { Allow12 = Allow12 + Convert.ToDouble(empBrow["gross_amount"]); basicItem.column10 = ReportColConvertToDecimal(Allow12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flggross = true;
                                    if (mn == 1) { Allow1 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column11 = ReportColConvertToDecimal(Allow1.ToString()); }
                                    if (mn == 2) { Allow2 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column12 = ReportColConvertToDecimal(Allow2.ToString()); }
                                    if (mn == 3) { Allow3 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column13 = ReportColConvertToDecimal(Allow3.ToString()); }
                                    if (mn == 4) { Allow4 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column2 = ReportColConvertToDecimal(Allow4.ToString()); }
                                    if (mn == 5) { Allow5 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column3 = ReportColConvertToDecimal(Allow5.ToString()); }
                                    if (mn == 6) { Allow6 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column4 = ReportColConvertToDecimal(Allow6.ToString()); }
                                    if (mn == 7) { Allow7 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column5 = ReportColConvertToDecimal(Allow7.ToString()); }
                                    if (mn == 8) { Allow8 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column6 = ReportColConvertToDecimal(Allow8.ToString()); }
                                    if (mn == 9) { Allow9 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column7 = ReportColConvertToDecimal(Allow9.ToString()); }
                                    if (mn == 10) { Allow10 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column8 = ReportColConvertToDecimal(Allow10.ToString()); }
                                    if (mn == 11) { Allow11 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column9 = ReportColConvertToDecimal(Allow11.ToString()); }
                                    if (mn == 12) { Allow12 = Convert.ToDouble(empBrow["gross_amount"]); basicItem.column10 = ReportColConvertToDecimal(Allow12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        Allow13 = (Allow1 + Allow2 + Allow3 + Allow4 + Allow5 + Allow6 + Allow7 + Allow8 + Allow9 + Allow10 + Allow11 + Allow12);
                        basicItem.column14 = ReportColConvertToDecimal(Allow13.ToString());
                    }
                    if (flggross) { lst.Add(basicItem); }
                    //if (flgGrossPay) { lst.Add(basicItem); }
                    



                    #endregion
                    int count = 0;
                    #region Deductions

                    // provident fund
                    bool flgPro_fund = false;
                    basicSAlw1 = 0; basicSAlw2 = 0; basicSAlw3 = 0; basicSAlw4 = 0; basicSAlw5 = 0; basicSAlw6 = 0; basicSAlw7 = 0;
                    basicSAlw8 = 0; basicSAlw9 = 0; basicSAlw10 = 0; basicSAlw11 = 0; basicSAlw12 = 0; basicSAlw13 = 0;
                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "Provident Fund",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        basicSAlw13 = 0;
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {

                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgPro_fund = true;
                                    if (mn == 1) { basicSAlw1 = basicSAlw1 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = basicSAlw2 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = basicSAlw3 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = basicSAlw4 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = basicSAlw5 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = basicSAlw6 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = basicSAlw7 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = basicSAlw8 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = basicSAlw9 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = basicSAlw10 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = basicSAlw11 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = basicSAlw12 + Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgPro_fund = true;
                                    if (mn == 1) { basicSAlw1 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = Convert.ToDouble(empBrow["dd_provident_fund"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicSAlw13 = (basicSAlw1 + basicSAlw2 + basicSAlw3 + basicSAlw4 + basicSAlw5 + basicSAlw6 + basicSAlw7 + basicSAlw8 + basicSAlw9 + basicSAlw10 + basicSAlw11 + basicSAlw12);
                        basicItem.column14 = ReportColConvertToDecimal(basicSAlw13.ToString());
                    }
                    if (flgPro_fund) { lst.Add(basicItem); }

                    // Income tax
                    bool flgIncometax = false;
                    basicSAlw1 = 0; basicSAlw2 = 0; basicSAlw3 = 0; basicSAlw4 = 0; basicSAlw5 = 0; basicSAlw6 = 0; basicSAlw7 = 0;
                    basicSAlw8 = 0; basicSAlw9 = 0; basicSAlw10 = 0; basicSAlw11 = 0; basicSAlw12 = 0; basicSAlw13 = 0;
                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "Income Tax",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgIncometax = true;
                                    if (mn == 1) { basicSAlw1 = basicSAlw1 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = basicSAlw2 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = basicSAlw3 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = basicSAlw4 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = basicSAlw5 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = basicSAlw6 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = basicSAlw7 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = basicSAlw8 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = basicSAlw9 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = basicSAlw10 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = basicSAlw11 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = basicSAlw12 + Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgIncometax = true;
                                    if (mn == 1) { basicSAlw1 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = Convert.ToDouble(empBrow["dd_income_tax"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicSAlw13 = (basicSAlw1 + basicSAlw2 + basicSAlw3 + basicSAlw4 + basicSAlw5 + basicSAlw6 + basicSAlw7 + basicSAlw8 + basicSAlw9 + basicSAlw10 + basicSAlw11 + basicSAlw12);
                        basicItem.column14 = ReportColConvertToDecimal(basicSAlw13.ToString());
                    }
                    if (flgIncometax) { lst.Add(basicItem); }

                    //Prof Tax

                    bool flgproftax = false;
                    basicSAlw1 = 0; basicSAlw2 = 0; basicSAlw3 = 0; basicSAlw4 = 0; basicSAlw5 = 0; basicSAlw6 = 0; basicSAlw7 = 0;
                    basicSAlw8 = 0; basicSAlw9 = 0; basicSAlw10 = 0; basicSAlw11 = 0; basicSAlw12 = 0; basicSAlw13 = 0;
                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "Professional Tax",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgproftax = true;
                                    if (mn == 1) { basicSAlw1 = basicSAlw1 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = basicSAlw2 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = basicSAlw3 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = basicSAlw4 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = basicSAlw5 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = basicSAlw6 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = basicSAlw7 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = basicSAlw8 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = basicSAlw9 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = basicSAlw10 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = basicSAlw11 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = basicSAlw12 + Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgproftax = true;
                                    if (mn == 1) { basicSAlw1 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = Convert.ToDouble(empBrow["dd_prof_tax"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicSAlw13 = (basicSAlw1 + basicSAlw2 + basicSAlw3 + basicSAlw4 + basicSAlw5 + basicSAlw6 + basicSAlw7 + basicSAlw8 + basicSAlw9 + basicSAlw10 + basicSAlw11 + basicSAlw12);
                        basicItem.column14 = ReportColConvertToDecimal(basicSAlw13.ToString());
                    }
                    if (flgproftax) { lst.Add(basicItem); }

                    //club subscription

                    bool flgclub = false;
                    basicSAlw1 = 0; basicSAlw2 = 0; basicSAlw3 = 0; basicSAlw4 = 0; basicSAlw5 = 0; basicSAlw6 = 0; basicSAlw7 = 0;
                    basicSAlw8 = 0; basicSAlw9 = 0; basicSAlw10 = 0; basicSAlw11 = 0; basicSAlw12 = 0; basicSAlw13 = 0;
                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "Club Subscription",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgclub = true;
                                    if (mn == 1) { basicSAlw1 = basicSAlw1 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = basicSAlw2 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = basicSAlw3 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = basicSAlw4 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = basicSAlw5 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = basicSAlw6 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = basicSAlw7 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = basicSAlw8 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = basicSAlw9 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = basicSAlw10 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = basicSAlw11 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = basicSAlw12 + Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgclub = true;
                                    if (mn == 1) { basicSAlw1 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = Convert.ToDouble(empBrow["dd_club_subscription"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicSAlw13 = (basicSAlw1 + basicSAlw2 + basicSAlw3 + basicSAlw4 + basicSAlw5 + basicSAlw6 + basicSAlw7 + basicSAlw8 + basicSAlw9 + basicSAlw10 + basicSAlw11 + basicSAlw12);
                        basicItem.column14 = ReportColConvertToDecimal(basicSAlw13.ToString());
                    }
                    if (flgclub) { lst.Add(basicItem); }

                    //telangana assn

                    bool flgTelassn = false;
                    basicSAlw1 = 0; basicSAlw2 = 0; basicSAlw3 = 0; basicSAlw4 = 0; basicSAlw5 = 0; basicSAlw6 = 0; basicSAlw7 = 0;
                    basicSAlw8 = 0; basicSAlw9 = 0; basicSAlw10 = 0; basicSAlw11 = 0; basicSAlw12 = 0; basicSAlw13 = 0;
                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "Telangana officers assn",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgTelassn = true;
                                    if (mn == 1) { basicSAlw1 = basicSAlw1 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = basicSAlw2 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = basicSAlw3 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = basicSAlw4 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = basicSAlw5 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = basicSAlw6 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = basicSAlw7 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = basicSAlw8 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = basicSAlw9 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = basicSAlw10 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = basicSAlw11 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = basicSAlw12 + Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgTelassn = true;
                                    if (mn == 1) { basicSAlw1 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = Convert.ToDouble(empBrow["dd_telangana_officers_assn"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicSAlw13 = (basicSAlw1 + basicSAlw2 + basicSAlw3 + basicSAlw4 + basicSAlw5 + basicSAlw6 + basicSAlw7 + basicSAlw8 + basicSAlw9 + basicSAlw10 + basicSAlw11 + basicSAlw12);
                        basicItem.column14 = ReportColConvertToDecimal(basicSAlw13.ToString());
                    }
                    if (flgTelassn) { lst.Add(basicItem); }


                    foreach (DataRow deduMast in dtAllDeduct.Rows)
                    {
                        count = count + 1;
                        Console.Write(count);
                        
                        bool flgDedu = false;
                        deduc1 = 0; deduc2 = 0; deduc3 = 0; deduc4 = 0; deduc5 = 0; deduc6 = 0; deduc7 = 0; deduc8 = 0; deduc9 = 0; deduc10 = 0; deduc11 = 0; deduc12 = 0; deduc13 = 0;
                        string deduction_name = deduMast["name"].ToString();
                        basicItem = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            column1 = deduction_name,
                            column2 = ReportColConvertToDecimal("0"),
                            column3 = ReportColConvertToDecimal("0"),
                            column4 = ReportColConvertToDecimal("0"),
                            column5 = ReportColConvertToDecimal("0"),
                            column6 = ReportColConvertToDecimal("0"),
                            column7 = ReportColConvertToDecimal("0"),
                            column8 = ReportColConvertToDecimal("0"),
                            column9 = ReportColConvertToDecimal("0"),
                            column10 = ReportColConvertToDecimal("0"),
                            column11 = ReportColConvertToDecimal("0"),
                            column12 = ReportColConvertToDecimal("0"),
                            column13 = ReportColConvertToDecimal("0"),
                            column14 = ReportColConvertToDecimal("0")

                        };
                        

                        var empDedurows = dtDeducts.Rows.Cast<DataRow>().Where(x => x["emp_code"].ToString() == sampleEmp
                        && string.Equals(x["dd_name"].ToString(), deduction_name, StringComparison.OrdinalIgnoreCase));

                        foreach (DataRow empdeductrow in empDedurows)
                        {
                            newmnth1 = empdeductrow["fm"].ToString();
                            newemp_code = empdeductrow["emp_code"].ToString();
                            basic13 = 0;
                            //if((oldmnth1 != newmnth1 || oldmnth1 == "") && (oldemp_code) == newemp_code) { 
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empdeductrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgDedu = true;
                                    if (mn == 1) { deduc1 = deduc1 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column11 = ReportColConvertToDecimal(deduc1.ToString()); }
                                    if (mn == 2) { deduc2 = deduc2 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column12 = ReportColConvertToDecimal(deduc2.ToString()); }
                                    if (mn == 3) { deduc3 = deduc3 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column13 = ReportColConvertToDecimal(deduc3.ToString()); }
                                    if (mn == 4) { deduc4 = deduc4 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column2 = ReportColConvertToDecimal(deduc4.ToString()); }
                                    if (mn == 5) { deduc5 = deduc5 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column3 = ReportColConvertToDecimal(deduc5.ToString()); }
                                    if (mn == 6) { deduc6 = deduc6 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column4 = ReportColConvertToDecimal(deduc6.ToString()); }
                                    if (mn == 7) { deduc7 = deduc7 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column5 = ReportColConvertToDecimal(deduc7.ToString()); }
                                    if (mn == 8) { deduc8 = deduc8 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column6 = ReportColConvertToDecimal(deduc8.ToString()); }
                                    if (mn == 9) { deduc9 = deduc9 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column7 = ReportColConvertToDecimal(deduc9.ToString()); }
                                    if (mn == 10) { deduc10 = deduc10 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column8 = ReportColConvertToDecimal(deduc10.ToString()); }
                                    if (mn == 11) { deduc11 = deduc11 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column9 = ReportColConvertToDecimal(deduc11.ToString()); }
                                    if (mn == 12) { deduc12 = deduc12 + Convert.ToDouble(empdeductrow["dd_amount"]); basicItem.column10 = ReportColConvertToDecimal(deduc12.ToString()); }
                                }
                            }

                            deduc13 = (deduc1 + deduc2 + deduc3 + deduc4 + deduc5 + deduc6 + deduc7 + deduc8 + deduc9 + deduc10 + deduc11 + deduc12);
                            basicItem.column14 = ReportColConvertToDecimal(basic13.ToString());
                            //}
                            oldmnth1 = empdeductrow["fm"].ToString();
                            oldemp_code = empdeductrow["emp_code"].ToString();
                        }



                        if (flgDedu)
                        {
                            lst.Add(basicItem);
                        }


                    }

                    string abc = "";
                    #endregion

                    #region TotalDeduction
                    //TotalDeduction  deductions_amount
                
                    string oldmnth = "";
                    string newmnth = "";

                    bool flgdeduc = false;
                    basicSAlw1 = 0; basicSAlw2 = 0; basicSAlw3 = 0; basicSAlw4 = 0; basicSAlw5 = 0; basicSAlw6 = 0; basicSAlw7 = 0;
                    basicSAlw8 = 0; basicSAlw9 = 0; basicSAlw10 = 0; basicSAlw11 = 0; basicSAlw12 = 0; basicSAlw13 = 0;
                    newbasic = "";
                    oldbasic = "";
                    newmnth1 = "";
                    oldmnth1 = "";
                    basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "<span style='font-weight: bold';font-size:18px;>Total Deduction</span>",
                        column2 = ReportColConvertToDecimal("0"),
                        column3 = ReportColConvertToDecimal("0"),
                        column4 = ReportColConvertToDecimal("0"),
                        column5 = ReportColConvertToDecimal("0"),
                        column6 = ReportColConvertToDecimal("0"),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = ReportColConvertToDecimal("0"),
                        column10 = ReportColConvertToDecimal("0"),
                        column11 = ReportColConvertToDecimal("0"),
                        column12 = ReportColConvertToDecimal("0"),
                        column13 = ReportColConvertToDecimal("0"),
                        column14 = ReportColConvertToDecimal("0")
                    };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgdeduc = true;
                                    if (mn == 1) { basicSAlw1 = basicSAlw1 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = basicSAlw2 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = basicSAlw3 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = basicSAlw4 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = basicSAlw5 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = basicSAlw6 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = basicSAlw7 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = basicSAlw8 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = basicSAlw9 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = basicSAlw10 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = basicSAlw11 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = basicSAlw12 + Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgdeduc = true;
                                    if (mn == 1) { basicSAlw1 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column11 = ReportColConvertToDecimal(basicSAlw1.ToString()); }
                                    if (mn == 2) { basicSAlw2 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column12 = ReportColConvertToDecimal(basicSAlw2.ToString()); }
                                    if (mn == 3) { basicSAlw3 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column13 = ReportColConvertToDecimal(basicSAlw3.ToString()); }
                                    if (mn == 4) { basicSAlw4 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column2 = ReportColConvertToDecimal(basicSAlw4.ToString()); }
                                    if (mn == 5) { basicSAlw5 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column3 = ReportColConvertToDecimal(basicSAlw5.ToString()); }
                                    if (mn == 6) { basicSAlw6 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column4 = ReportColConvertToDecimal(basicSAlw6.ToString()); }
                                    if (mn == 7) { basicSAlw7 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column5 = ReportColConvertToDecimal(basicSAlw7.ToString()); }
                                    if (mn == 8) { basicSAlw8 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column6 = ReportColConvertToDecimal(basicSAlw8.ToString()); }
                                    if (mn == 9) { basicSAlw9 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column7 = ReportColConvertToDecimal(basicSAlw9.ToString()); }
                                    if (mn == 10) { basicSAlw10 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column8 = ReportColConvertToDecimal(basicSAlw10.ToString()); }
                                    if (mn == 11) { basicSAlw11 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column9 = ReportColConvertToDecimal(basicSAlw11.ToString()); }
                                    if (mn == 12) { basicSAlw12 = Convert.ToDouble(empBrow["deductions_amount"]); basicItem.column10 = ReportColConvertToDecimal(basicSAlw12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        basicSAlw13 = (basicSAlw1 + basicSAlw2 + basicSAlw3 + basicSAlw4 + basicSAlw5 + basicSAlw6 + basicSAlw7 + basicSAlw8 + basicSAlw9 + basicSAlw10 + basicSAlw11 + basicSAlw12);
                        basicItem.column14 = ReportColConvertToDecimal(basicSAlw13.ToString());
                    }
                    if (flgdeduc) { lst.Add(basicItem); }


                    #endregion


                    #region NetPay
                    //NetPay
                    bool flgnet = false;
                    Allow1 = 0; Allow2 = 0; Allow3 = 0; Allow4 = 0; Allow5 = 0; Allow6 = 0; Allow7 = 0; Allow8 = 0; Allow9 = 0; Allow10 = 0; Allow11 = 0; Allow12 = 0; Allow13 = 0;
                            basicItem = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "<span style='font-weight: bold';font-size:18px;>Net Pay</span>",
                                column2 = ReportColConvertToDecimal("0"),
                                column3 = ReportColConvertToDecimal("0"),
                                column4 = ReportColConvertToDecimal("0"),
                                column5 = ReportColConvertToDecimal("0"),
                                column6 = ReportColConvertToDecimal("0"),
                                column7 = ReportColConvertToDecimal("0"),
                                column8 = ReportColConvertToDecimal("0"),
                                column9 = ReportColConvertToDecimal("0"),
                                column10 = ReportColConvertToDecimal("0"),
                                column11 = ReportColConvertToDecimal("0"),
                                column12 = ReportColConvertToDecimal("0"),
                                column13 = ReportColConvertToDecimal("0"),
                                column14 = ReportColConvertToDecimal("0")

                            };
                    foreach (var empBrow in empBrows)
                    {
                        newbasic = empBrow["emp_code"].ToString();
                        newmnth1 = empBrow["fm"].ToString();
                        Allow13 = 0;
                        if ((newbasic == oldbasic) && (newmnth1 == oldmnth1))
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgnet = true;
                                    if (mn == 1) { Allow1 = Allow1 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column11 = ReportColConvertToDecimal(Allow1.ToString()); }
                                    if (mn == 2) { Allow2 = Allow2 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column12 = ReportColConvertToDecimal(Allow2.ToString()); }
                                    if (mn == 3) { Allow3 = Allow3 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column13 = ReportColConvertToDecimal(Allow3.ToString()); }
                                    if (mn == 4) { Allow4 = Allow4 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column2 = ReportColConvertToDecimal(Allow4.ToString()); }
                                    if (mn == 5) { Allow5 = Allow5 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column3 = ReportColConvertToDecimal(Allow5.ToString()); }
                                    if (mn == 6) { Allow6 = Allow6 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column4 = ReportColConvertToDecimal(Allow6.ToString()); }
                                    if (mn == 7) { Allow7 = Allow7 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column5 = ReportColConvertToDecimal(Allow7.ToString()); }
                                    if (mn == 8) { Allow8 = Allow8 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column6 = ReportColConvertToDecimal(Allow8.ToString()); }
                                    if (mn == 9) { Allow9 = Allow9 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column7 = ReportColConvertToDecimal(Allow9.ToString()); }
                                    if (mn == 10) { Allow10 = Allow10 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column8 = ReportColConvertToDecimal(Allow10.ToString()); }
                                    if (mn == 11) { Allow11 = Allow11 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column9 = ReportColConvertToDecimal(Allow11.ToString()); }
                                    if (mn == 12) { Allow12 = Allow12 + Convert.ToDouble(empBrow["net_amount"]); basicItem.column10 = ReportColConvertToDecimal(Allow12.ToString()); }
                                }
                            }
                        }
                        else
                        {
                            for (int mn = 1; mn <= 12; mn++)
                            {
                                DateTime FM = Convert.ToDateTime(empBrow["fm"].ToString());
                                if (FM.Month == mn)
                                {
                                    flgnet = true;
                                    if (mn == 1) { Allow1 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column11 = ReportColConvertToDecimal(Allow1.ToString()); }
                                    if (mn == 2) { Allow2 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column12 = ReportColConvertToDecimal(Allow2.ToString()); }
                                    if (mn == 3) { Allow3 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column13 = ReportColConvertToDecimal(Allow3.ToString()); }
                                    if (mn == 4) { Allow4 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column2 = ReportColConvertToDecimal(Allow4.ToString()); }
                                    if (mn == 5) { Allow5 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column3 = ReportColConvertToDecimal(Allow5.ToString()); }
                                    if (mn == 6) { Allow6 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column4 = ReportColConvertToDecimal(Allow6.ToString()); }
                                    if (mn == 7) { Allow7 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column5 = ReportColConvertToDecimal(Allow7.ToString()); }
                                    if (mn == 8) { Allow8 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column6 = ReportColConvertToDecimal(Allow8.ToString()); }
                                    if (mn == 9) { Allow9 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column7 = ReportColConvertToDecimal(Allow9.ToString()); }
                                    if (mn == 10) { Allow10 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column8 = ReportColConvertToDecimal(Allow10.ToString()); }
                                    if (mn == 11) { Allow11 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column9 = ReportColConvertToDecimal(Allow11.ToString()); }
                                    if (mn == 12) { Allow12 = Convert.ToDouble(empBrow["net_amount"]); basicItem.column10 = ReportColConvertToDecimal(Allow12.ToString()); }
                                }
                            }
                        }
                        oldbasic = empBrow["emp_code"].ToString();
                        oldmnth1 = empBrow["fm"].ToString();
                        Allow13 = (Allow1 + Allow2 + Allow3 + Allow4 + Allow5 + Allow6 + Allow7 + Allow8 + Allow9 + Allow10 + Allow11 + Allow12);
                        basicItem.column14 = ReportColConvertToDecimal(Allow13.ToString());
                    }
                    if (flgnet) { lst.Add(basicItem); }
                    #endregion



                }

            }
            catch (Exception e)
            {

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