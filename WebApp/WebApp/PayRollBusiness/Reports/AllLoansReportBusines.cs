using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Reports
{
    public class AllLoansReportBusiness : BusinessBase
    {
        public AllLoansReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        string qrySel;
        string qrysel1;
        string qrysel2;
        string oldtype = "";
        string oldcode = "";
        string type = "";
        string code = "";
        int RowCnt = 0;
        int SlNo = 1;
        int SlNoS = 1;

        IList<CommonReportModel1> lst = new List<CommonReportModel1>();


        public async Task<IList<CommonReportModel>> GetLoanReport(string Month, string RegEmp, string SupEmp)

        {
            string qry = "";
            string newmonth = "";
            string principle = "";
            string intrest = "";
            string total = "";
            string loanType="";
            string EmpTypes = "";
            string str2;
            string stre3;
            string loandes = "";
            string oldloandes = "";
            string oldloandes1 = "";

            string q1 = "";
            string general = PrConstants.REGULAR;
            string adhoc = PrConstants.ADHOC;
            if (Month.Contains("^"))
            {

                Month = "2019-01-12";
            }
            if (RegEmp.Contains("^"))
            {
                RegEmp = "0";

                Month = "2019-01-12";
            }

            DateTime str = Convert.ToDateTime(Month);
            string str1 = str.ToString("yyyy-MM-dd");
            string empid = RegEmp;
            if (RegEmp != "" && RegEmp != "undefined")
            {
                q1 = "  pay.spl_type='" + general + "'";

            }
            if ( SupEmp != "" && RegEmp == "undefined")
            {
                q1 = " pay.spl_type='" + adhoc + "'";
            }
            qry = " select Convert(date,ad.fm,102) as fm,lm.loan_description,sum(ad.principal_paid_amount) as principal_open_amount,sum(ad.interest_paid_amount) as interest_accured,sum( case when ad.principal_paid_amount is null then 0 else ad.principal_paid_amount end +  case when ad.interest_paid_amount is null then 0 else ad.interest_paid_amount end)" +
                  " as Total  from pr_emp_adv_loans_adjustments ad " +
                  " join pr_emp_adv_loans l on l.id = ad.emp_adv_loans_mid " +
                  " join pr_loan_master lm on lm.id = l.loan_type_mid " +
                "  join pr_emp_payslip pay on pay.emp_code = l.emp_code  " +
                  " where " + q1 + " and  month(Convert(date,ad.fm,102)) =month('" + str1 + "') and year(Convert(date,ad.fm,102)) =year('" + str1 + "')  " +
                  "group by lm.loan_description,ad.fm  ";

            qrysel1 = " select  Convert(date, ad.fm,102) as fm,sum(ad.principal_paid_amount) as principal_open_amount,sum(ad.interest_paid_amount) as interest_accured, " +
                      " sum( case when ad.principal_paid_amount is null then 0 else ad.principal_paid_amount end +  case when ad.interest_paid_amount is null then 0 else ad.interest_paid_amount end) " +
                      " as Total  from pr_emp_adv_loans_adjustments ad " +
                      " join pr_emp_adv_loans l on l.id = ad.emp_adv_loans_mid" +
                      " join pr_loan_master lm on lm.id = l.loan_type_mid " +
                    "  join pr_emp_payslip pay on pay.emp_code = l.emp_code  " +
                      " where " + q1 + " and  month(Convert(date,ad.fm,102)) = month('" + str1 + "') and year(Convert(date,ad.fm,102)) =year('" + str1 + "') " +
                      "group by ad.fm ";

            qrysel2 = " select lm.loan_description,ad.fm as fm,l.emp_code as emp_code,e.shortname as ShortName, " +
                      " d.Description as Designation, ad.principal_paid_amount as principal_open_amount, " +
                      " ad.interest_paid_amount as interest_accured ," +
                      " sum( case when ad.principal_paid_amount is null then 0 else ad.principal_paid_amount end +  case when ad.interest_paid_amount is null then 0 else ad.interest_paid_amount end) " +
                      " as Total  from pr_emp_adv_loans_adjustments ad " +
                      " join pr_emp_adv_loans l on l.id = ad.emp_adv_loans_mid join pr_loan_master lm on lm.id = l.loan_type_mid join Employees e on e.EmpId = l.emp_code " +
                      " join Designations d on d.Id = e.CurrentDesignation  join pr_emp_payslip pay on pay.emp_code = l.emp_code   where " + q1 + " and  " +
                       "  month(Convert(date,ad.fm,102)) =month('" + str1 + "') and year(Convert(date,ad.fm,102)) =year('" + str1 + "') " +
                      " group by lm.loan_description , l.emp_code, e.shortname,d.Description, ad.principal_paid_amount, ad.interest_paid_amount,ad.fm ";


            DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qrysel1 + qrysel2);
            DataTable dtALL = ds.Tables[0];
            DataTable dtTot = ds.Tables[1];
            DataTable dtHouse = ds.Tables[2];
            int rowid = 0;

            IList<CommonReportModel> lst = new List<CommonReportModel>();
            //adding summary grid data
            if (ds.Tables[0].Rows.Count > 0)
            {
              
               

                    lst.Add(new CommonReportModel
                    {
                        RowId = 0,
                        HRF = "H",
                        column1 = "<span style='color:#C8EAFB'>~</span>"
                   + ReportColHeader(0, "LOAN SUMMARY REPORT", ""),
                        column2 = "`",
                        column3 = "`",
                        column4 = "`",
                        column5 = "`",
                        column6 = "`",
                        column7 = "`"
                    });
             


            lst.Add(new CommonReportModel
            {
                RowId = 0,
                HRF = "R",

                column1 = "S.No.",
                column2 = "Loan Description",
                column3 = "Principle(1)",
                column4 = "Interest(2)",
                column5 = "Total (1) + (2)"
            });
            foreach (DataRow dr in dtALL.Rows)
                { 
                    lst.Add(new CommonReportModel
                   {
                    RowId = rowid++,
                    HRF = "R",
                    column1 = SlNo++.ToString(),
                    column2 = dr["loan_description"].ToString(),
                    column3 = ReportColConvertToDecimalAndAlign(dr["principal_open_amount"].ToString()),
                    column4 = ReportColConvertToDecimalAndAlign(dr["interest_accured"].ToString()),
                    column5 = ReportColConvertToDecimalAndAlign(dr["Total"].ToString()),
                  });
                newmonth = dr["fm"].ToString();
                    oldloandes = dr["loan_description"].ToString();

                }

                foreach (DataRow dr in dtTot.Rows)
                {
                    principle = ReportColConvertToDecimalAndAlign(dr["principal_open_amount"].ToString());
                    intrest = ReportColConvertToDecimalAndAlign(dr["interest_accured"].ToString());
                    total = ReportColConvertToDecimalAndAlign(dr["Total"].ToString());
                }
                lst.Add(new CommonReportModel
                {
                    RowId = rowid++,
                    HRF = "F",
                    //column1 = "<span style='color:" + Mavensoft.Common.PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>^</span>"
                    //+ ReportColFooter(42, "(P)", principle)
                    //    + ReportColFooter(21, "(I)", intrest)
                    //    + ReportColFooter(44, "(T)", total)

                    column3="(P)"+principle,
                    column4 = "(I)"+intrest,
                    column5="(T)"+total
                });
            }
            //adding Detail grid data
            if (ds.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow dr in dtHouse.Rows)
                {
                    loandes = dr["loan_description"].ToString();
                    
                    if (oldloandes1 != loandes)
                    {

                        lst.Add(new CommonReportModel
                        {
                            RowId = rowid++,
                            HRF = "H",
                            column1 = "<span style='color:#C8EAFB'>~</span>"
                         + ReportColHeader(0, "LOAN SUMMARY REPORT of", dr["loan_description"].ToString()),
                            //column1 = "<span style='color:#C8EAFB'></b></span><span style='margin-left: 30px;'> Detailed Loan REPORT Of " + loanType + " </b></span>"
                            column2 = "`",
                            column3 = "`",
                            column4 = "`",
                            column5 = "`",
                            column6 = "`",
                            column7 = "`"

                        });

                        oldloandes = dr["loan_description"].ToString();
                        lst.Add(new CommonReportModel
                        {
                            RowId = rowid++,
                            HRF = "R",
                            SNo = 1,
                            column1 = "S.No.",
                            column2 = "Emp Code",
                            column3 = " Emp Name",
                            column4 = "Designation",

                            column5 = "Principal",
                            column6 = "Interest",
                            column7 = "Total",



                        });
                       
                    }
                    oldloandes1 = dr["loan_description"].ToString(); 
                    lst.Add(new CommonReportModel
                    {
                        RowId = rowid++,
                        HRF = "R",
                       column1 = SlNoS++.ToString(),
                        column2 = dr["emp_code"].ToString(),
                        column3 = dr["ShortName"].ToString(),
                        column4 = dr["Designation"].ToString(),
                        column5 = ReportColConvertToDecimal(dr["principal_open_amount"].ToString()),
                        column6 = ReportColConvertToDecimal(dr["interest_accured"].ToString()),
                        column7 = ReportColConvertToDecimal(dr["Total"].ToString()),

                    });


                    var EmpType = dr["emp_code"].ToString();
                    var prinicipal = ReportColConvertToDecimal(dr["principal_open_amount"].ToString());
                    if (EmpTypes!= EmpType )
                    {
                        //lst.Add(new CommonReportModel
                        //{
                        //    RowId = rowid++,
                        //    HRF = "F",
                        // column1 = "<span style='color:" + Mavensoft.Common.PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>^</span>"
                        //+ ReportColFooter(138, "(P)", principle)
                        //+ ReportColFooter(10, "(I)", intrest)
                        //+ ReportColFooter(15, "(T)", total)

                        //});
                    }
                    
                    EmpTypes = dr["emp_code"].ToString();
                    principle = ReportColConvertToDecimal(dr["principal_open_amount"].ToString());
                }

            }

            return lst;
        }
      
        private CommonReportModel getTotal(string Month, DataTable dt)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["fm"].ToString() == Month)
                .Select(x => new { tot = x["principal_open_amount"].ToString() + "~" + x["interest_accured"].ToString() + "~" + x["Total"].ToString() }).FirstOrDefault();

            var arrTots = val.tot.ToString().Split('~');


            var tot = new CommonReportModel
            {
                RowId = 0,
                HRF = "F",
                column1 = "<span style='color:#eef8fd'>^</span>"
                + ReportColFooter(40, "", arrTots[0])
                + ReportColFooter(40, "", arrTots[1])
                + ReportColFooter(40, "", arrTots[2])
            };

            return tot;
        }
        public class CommonReportModel1
        {
            public int RowId { get; set; }
            public string HRF { get; set; }
            public string Col1 { get; set; }
            public string Col2 { get; set; }
            public string Col3 { get; set; }
            public string Col4 { get; set; }
            public string Col5 { get; set; }
            public string Col6 { get; set; }
            public string Col7 { get; set; }
            public string Col8 { get; set; }
            public string Col9 { get; set; }
            public string Col10 { get; set; }
            public string Col11 { get; set; }
            public string Col12 { get; set; }
            public string Col13 { get; set; }
            public string Col14 { get; set; }
            public string Col15 { get; set; }
            public string Col16 { get; set; }
            public string Col17 { get; set; }
            public string Col18 { get; set; }
            public string Col19 { get; set; }
            public string Col20 { get; set; }
            public string Col21 { get; set; }
            public string grpclmn { get; set; }
            public string footer { get; set; }
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

        public string ReportColConvertToDecimalAndAlign(string value)
        {
            string sRet = "";
            if (value == "")
            {
                value = "0";
            }
            decimal Drvalue = Convert.ToDecimal(value.ToString()) + 0.00M;
            decimal DPT = Convert.ToDecimal(String.Format("{0:0.00}", Drvalue));
            string NwDPT = String.Format("{0:n}", DPT);
            sRet = "<span style=Float:right>" + NwDPT + "</span>";

            return sRet;
        }



    }

}
