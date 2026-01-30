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
using System.Configuration;
using PayRollBusiness.Process;
using System.Linq;

namespace PayRollBusiness.Reports
{
   public class Form3ABusiness : BusinessBase
    {
        public Form3ABusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }

        List<CommonReportModel> lst = new List<CommonReportModel>();

        public async Task<IList<CommonReportModel>> form3AData(string empCode, string fYear)
        {
            int RowCnt = 0;
            int Eyear = 0001;
            int Fyear = 0001;
            string newemp = "";
            string oldemp = "";
            double sumownshare = 0.00;
            double pensionopen=0.00;
            if (empCode.Contains("^"))
            {
                empCode = "0";
            }
            if (fYear != null && fYear != "^2")
            {
                Eyear = int.Parse(fYear);
                Fyear = int.Parse(fYear) - 1;
            }

            string Qry = "select  DISTINCT ob.fm, REPLACE(RIGHT(CONVERT(VARCHAR(11),ob.fm, 106), 8), ' ', '-') as fm1,ob.emp_code, e.ShortName,e.FatherName,gen.pf_no,own_share,pension_open,MA.lop_days from pr_ob_share ob" +
                " join pr_emp_general gen on gen.emp_code = ob.emp_code" +
                " join pr_month_attendance MA on MA.emp_code = ob.emp_code" +
                " join Employees e on e.EmpId = ob.emp_code where ob.fm between DATEFROMPARTS("+ Fyear + ", 04, 01) and DATEFROMPARTS("+ Eyear +", 03, 31 ) and gen.active = 1 and MA.active=1 ";

            if (empCode != "All")
            {
                Qry += " and ob.emp_code = '"+empCode+"'";
            }
            Qry += " order by  ob.emp_code,ob.fm";



            DataTable dt = await _sha.Get_Table_FromQry(Qry);
            int count = 0;
            int count1 = dt.Rows.Count;

            foreach (DataRow dr in dt.Rows )
            {
               
                count = count + 1;
                DataTable dt1;
                newemp = dr["emp_code"].ToString();

                //string Qry1 = "select  ob.emp_code,sum(own_share) as own_share , sum(pension_open) as pension_open  from pr_ob_share ob where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS("+ Eyear +", 03, 31 ) and emp_code = '"+ newemp + "' group by ob.emp_code";
                if (oldemp == "")
                {
                    string Qry1 = "select ob.emp_code, sum(own_share) as own_share, sum(pension_open) as pension_open, sum(lop_days) as lop_days from pr_ob_share ob join pr_emp_general gen on gen.emp_code = ob.emp_code join Employees e on e.EmpId = ob.emp_code join pr_month_attendance MA on MA.emp_code = ob.emp_code and MA.active=1 where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and ob.emp_code = " + newemp + " and gen.active=1 group by ob.emp_code";
                    dt1= await _sha.Get_Table_FromQry(Qry1);

                }
                else
                {
                    string Qry1 = "select ob.emp_code, sum(own_share) as own_share, sum(pension_open) as pension_open, sum(lop_days) as lop_days from pr_ob_share ob join pr_emp_general gen on gen.emp_code = ob.emp_code join Employees e on e.EmpId = ob.emp_code join pr_month_attendance MA on MA.emp_code = ob.emp_code and MA.active=1 where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and ob.emp_code = " + oldemp + " and gen.active=1 group by ob.emp_code";
                     dt1 = await _sha.Get_Table_FromQry(Qry1);

                }




                // first employee
                if (newemp != oldemp && oldemp == "")
                {
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style='color:#C8EAFB'>~</span>"
                                            + ReportColHeader(0, "1. Emp Code", dr["emp_code"].ToString()),
                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style ='color:#C8EAFB'> ~</span> "
                                            + ReportColHeader(0, "2. Name/SurName", dr["ShortName"].ToString()),
                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style ='color:#C8EAFB'> ~</span> "
                                            + ReportColHeader(0, "3. P.F Account No", dr["pf_no"].ToString()),
                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style ='color:#C8EAFB'> ~</span> "
                                            + ReportColHeader(0, "4. Name & Address Of The Factory", "TELANGANA STATE CO-OP APEX BANK LTD."),
                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style ='color:#C8EAFB'> ~</span> "
                                            + ReportColHeader(0, "5. Statutory Rate Of Contribution", "")
                                             + ReportColHeader(10, "Code No", ""),
                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style ='color:#C8EAFB'> ~</span> "
                                            + ReportColHeader(0, "6. Voluntary Higher Rate of Employees Contribution If Any", ""),
                    });

                }

                if (newemp == oldemp || oldemp== "")
                {
                    String DRownshare = ((Convert.ToDouble(dr["own_share"])) * 8).ToString();
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = dr["fm1"].ToString(),
                        //column2 = ReportColConvertToDecimalMUL((dr["own_share"]).ToString()),
                         column2 = ReportColConvertToDecimal(DRownshare.ToString()),

                        column3 = ReportColConvertToDecimal(dr["own_share"].ToString()),
                        
                        column4 = ReportColConvertToDecimal((Convert.ToInt32(dr["own_share"]) - Convert.ToInt32(dr["pension_open"])).ToString()),
                     //   column4 = (Convert.ToInt32(dr["own_share"]) - Convert.ToInt32(dr["pension_open"])).ToString(),
                        column5 = ReportColConvertToDecimal(dr["pension_open"].ToString()),
                        column6 = "0",
                        column7 = dr["lop_days"].ToString(),
                        column8 = "",
                    });

                   
                }



                // next employee
                if (newemp != oldemp && oldemp != "")
                {
                    // for total fist employee
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    column1 = "Others",
                    //    column2 = "",
                    //    column3 = "",
                    //    column4 = "",
                    //    column5 = "",
                    //    column6 = " ",
                    //    column7 = "",
                    //    column8 = "",
                    //});
                    String dt1ownshare = ((Convert.ToDouble(dt1.Rows[0]["own_share"])) * 8).ToString();
                    lst.Add(new CommonReportModel
                    {
                    RowId = RowCnt++,
                        HRF = "R",
                        column1 = "Total",
                        column2 = ReportColConvertToDecimal(dt1ownshare.ToString()),
                        
                        column3 = ReportColConvertToDecimal(dt1.Rows[0]["own_share"].ToString()),
                        column4 = ReportColConvertToDecimal((Convert.ToInt32(dt1.Rows[0]["own_share"]) - Convert.ToInt32(dt1.Rows[0]["pension_open"])).ToString()),
                        //  column4 = (Convert.ToInt32(dt1.Rows[0]["own_share"]) - Convert.ToInt32(dt1.Rows[0]["pension_open"])).ToString(),
                        column5 = ReportColConvertToDecimal(dt1.Rows[0]["pension_open"].ToString()),
                        column6 = "0",
                        column7 = dt1.Rows[0]["lop_days"].ToString(),
                        column8 = "",

                    });

                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                               + ReportColFooter1(0, "Certified that  the total amount of  contribution (both shares) indicated in this card i.e.Rs."+(Convert.ToDouble(dt1.Rows[0]["own_share"])+ (Convert.ToInt32(dt1.Rows[0]["own_share"]) - Convert.ToInt32(dt1.Rows[0]["pension_open"])) + Convert.ToDouble(dt1.Rows[0]["pension_open"])) + ".00 has already  been  remmited in full in EPF A/C No.1 and " +
                               "P.F No 10 has vide note below.","")
                               //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                              + ReportColFooter1(0, "Certified  that the  difference   between the  total  of  the  contribution under " +
                              " columns 3, 4a & 4b of the above table and that arrived at on the total wages shown" +
                              " in column 2 at the prescribed rate is solely due to rounding off of contributions to the nearest rupee under the rules.", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                               + ReportColFooter1(0, "Note: 1) In respect of the Form 3 A sent to the regional office during the course " +
                               "of  the  currency   period  for the purpose  of final  settlement of the" +
                               " accounts  of the  member who has left  service  details of date & reason for leaving service should be furnished under column 7(a) & 7(b)", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooter1(0, "Note: 2) In  respect of  those  who  are not  members  of the  pension  fund  the " +
                                "employers share of  contribution to EPF will be 8.13 % or 12 % as the case" +
                                " may be & is to be shown under column4(a)", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooterlesscol(80, "For TELANGANA STATE CO-OP APEX BANK LTD.","")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooter1(0, "Date:", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooter1(0, "Place:", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });





                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style='color:#C8EAFB'>~</span>"
                                            + ReportColHeader(0, "1. Emp Code", dr["emp_code"].ToString()),
                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style ='color:#C8EAFB'> ~</span> "
                                            + ReportColHeader(0, "2. Name/SurName", dr["ShortName"].ToString()),
                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style ='color:#C8EAFB'> ~</span> "
                                            + ReportColHeader(0, "3. P.F Account No", dr["pf_no"].ToString()),
                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style ='color:#C8EAFB'> ~</span> "
                                            + ReportColHeader(0, "4. Name & Address Of The Factory", "TELANGANA STATE CO-OP APEX BANK LTD."),
                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style ='color:#C8EAFB'> ~</span> "
                                            + ReportColHeader(0, "5. Statutory Rate Of Contribution", "")
                                             + ReportColHeader(10, "Code No", ""),
                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style ='color:#C8EAFB'> ~</span> "
                                            + ReportColHeader(0, "6. Voluntary Higher Rate of Employees Contribution If Any", ""),
                    });

                }
                if (newemp != oldemp && oldemp != "")
                {
                    string DRownshare1 = ((Convert.ToDouble(dr["own_share"])) * 8).ToString();
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = dr["fm1"].ToString(),
                        column2 = ReportColConvertToDecimal(DRownshare1.ToString()),
                        // column2 = ((Convert.ToDouble(dr["own_share"])) * 8).ToString(),
                        column3 = ReportColConvertToDecimal(dr["own_share"].ToString()),
                        column4 = ReportColConvertToDecimal((Convert.ToInt32(dr["own_share"]) - Convert.ToInt32(dr["pension_open"])).ToString()),
                   //   column4 = (Convert.ToInt32(dr["own_share"]) - Convert.ToInt32(dr["pension_open"])).ToString(),
                        column5 = ReportColConvertToDecimal(dr["pension_open"].ToString()),
                        column6 = "0",
                        column7 = dr["lop_days"].ToString(),
                        column8 = "",
                    });
                }


                //for footer

                if ( count == count1)
                {

                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    column1 = "Others",
                    //    column2 = "",
                    //    column3 = "",
                    //    column4 = "",
                    //    column5 = "",
                    //    column6 = " ",
                    //    column7 = "",
                    //    column8 = "",
                    //});
                    String dt1ownshare1 = ((Convert.ToDouble(dt1.Rows[0]["own_share"])) * 8).ToString();
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "Total",
                        column2 = ReportColConvertToDecimal(dt1ownshare1.ToString()),
                        column3 = ReportColConvertToDecimal(dt1.Rows[0]["own_share"].ToString()),
                        column4 = ReportColConvertToDecimal((Convert.ToInt32(dt1.Rows[0]["own_share"]) - Convert.ToInt32(dt1.Rows[0]["pension_open"])).ToString()),
                        //  column4 = (Convert.ToInt32(dt1.Rows[0]["own_share"]) - Convert.ToInt32(dt1.Rows[0]["pension_open"])).ToString(),
                        column5 = ReportColConvertToDecimal(dt1.Rows[0]["pension_open"].ToString()),
                        column6 = "0",
                        column7 = dt1.Rows[0]["lop_days"].ToString(),
                        column8 = "",

                    });

                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                               + ReportColFooter1(0, "Certified that  the total amount of  contribution (both shares) indicated in this card i.e.Rs." + (Convert.ToDouble(dt1.Rows[0]["own_share"]) + (Convert.ToInt32(dt1.Rows[0]["own_share"]) - Convert.ToInt32(dt1.Rows[0]["pension_open"])) + Convert.ToDouble(dt1.Rows[0]["pension_open"])) + ".00 has already  been  remmited in full in EPF A/C No.1 and " +
                               "P.F No 10 has vide note below.", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                              + ReportColFooter1(0, "Certified  that the  difference   between the  total  of  the  contribution under " +
                              " columns 3, 4a & 4b of the above table and that arrived at on the total wages shown" +
                              " in column 2 at the prescribed rate is solely due to rounding off of contributions to the nearest rupee under the rules.", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                               + ReportColFooter1(0, "Note: 1) In respect of the Form 3 A sent to the regional office during the course " +
                               "of  the  currency   period  for the purpose  of final  settlement of the" +
                               " accounts  of the  member who has left  service  details of date & reason for leaving service should be furnished under column 7(a) & 7(b)", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooter1(0, "Note: 2) In  respect of  those  who  are not  members  of the  pension  fund  the " +
                                "employers share of  contribution to EPF will be 8.13 % or 12 % as the case" +
                                " may be & is to be shown under column4(a)", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooterlesscol(80, "For TELANGANA STATE CO-OP APEX BANK LTD.", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooter1(0, "Date:", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooter1(0, "Place:", "")
                        //+ ReportColFooter1(30, "Net Amount of Rs.  ", "")

                    });


                }


                oldemp = dr["emp_code"].ToString();

            }

            return lst;
        }


        public string ReportColFooter1(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable + " " + value + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string ReportColFooterlesscol1(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp; &nbsp; &nbsp; ";
            sRet += "</span>";

            sRet += "<span>" + lable + " " + value + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string ReportColConvertToDecimalMUL(string value)
        {

            if (value == "")
            {
                value = "0";
            }
            decimal Drvalue = Convert.ToDecimal(value.ToString()) * 8 + 0.00M;
            decimal DPT = Convert.ToDecimal(String.Format("{0:0.00}", Drvalue));
            string NwDPT = String.Format("{0:n}", DPT);


            return NwDPT;
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
