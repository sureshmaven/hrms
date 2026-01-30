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
    public class SingleRateLoanOutStandingBus : BusinessBase
    {
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        public SingleRateLoanOutStandingBus(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        int RowCnt = 0;
        int SNo = 1;
        string loandes = "";
        string oldloandes = "";
        string q1 = "";
        string q2 = "";
        string foldloandes = "";
        int count = 0;
        public async Task<IList<CommonReportModel>> GetSingleRateLoanOutStandingData(string loancode, string mnth)
        {
            string qry = "";
            //var empid = "0";
            string loancodes = loancode;

            DateTime str = Convert.ToDateTime(mnth);
            string str1 = str.ToString("yyyy-MM-dd");
            string dm = str.ToString("MMM-yyyy");
            string oldmonth = "";
            if (loancode.Contains("^") || loancode == "")
            {
                loancodes = "0";
                mnth = "01-01-01";
            }
            if (loancode != "All")
            {
                q1 = " where l.loan_type_mid in (" + loancodes + ") and month(a.fm)=month('" + str1 + "') and year(a.fm)=year('" + str1 + "') and a.active=1 order by  lm.loan_description";
                q2 = " where l.loan_type_mid in (" + loancodes + ") and month(a.fm)=month('" + str1 + "') and year(a.fm)=year('" + str1 + "') and a.active=1 group by  lm.loan_description";
            }
            qry = "select lm.loan_description,l.emp_code,e.shortname as Employee_Name,d.Name as Desig, cl.interest_rate," +
                "(a.principal_open_amount) as Principal_Opening,(a.principal_paid_amount  +  a.interest_paid_amount ) as Curr_Month_recieved,a.principal_paid_amount as PrincipalPaid, " +
                "a.principal_balance_amount as Loan_Closing, a.interest_open_amount as Interest_Opening," +
                "a.interest_accured as Interest_Accured, a.interest_paid_amount as Interest_Repaid," +
                "a.interest_balance_amount as Interest_Closing,(cl.loan_amount)as total_amount,(cl.principal_amount_recovered) as total_recovered_amount from pr_emp_adv_loans_adjustments a " +
                "join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid " +
                "join pr_loan_master lm on lm.id=l.loan_type_mid " +
                "join pr_emp_adv_loans_child  cl on l.id = cl.emp_adv_loans_mid and cl.id=a.emp_adv_loans_child_mid " +
                "join Employees e on e.EmpId = l.emp_code " +
                "join Designations d on d.Id = e.CurrentDesignation " + q1 + ";";


            string sumquery = " select lm.loan_description,sum(cl.interest_rate)as interest_rate,sum(a.principal_open_amount) as Principal_Opening," +
                "sum(a.principal_paid_amount  +  a.interest_paid_amount ) as Curr_Month_recieved,sum(a.principal_paid_amount) as PrincipalPaid,sum(a.principal_balance_amount) as Loan_Closing," +
                "sum(a.interest_open_amount) as Interest_Opening,sum(a.interest_accured) as Interest_Accured," +
                "sum(a.interest_paid_amount) as Interest_Repaid,sum(a.interest_balance_amount) as Interest_Closing," +
                "sum(cl.loan_amount)as total_amount,sum(cl. principal_amount_recovered)as total_recovered_amount " +
                "from pr_emp_adv_loans_adjustments a join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid " +
                "join pr_loan_master lm on lm.id = l.loan_type_mid " +
                "join pr_emp_adv_loans_child  cl on l.id = cl.emp_adv_loans_mid and cl.id=a.emp_adv_loans_child_mid " +
                "join Employees e on e.EmpId = l.emp_code " +
                "join Designations d on d.Id = e.CurrentDesignation " + q2 + ";";

            DataSet ds = await _sha.Get_MultiTables_FromQry(qry + sumquery);
            DataTable dt = ds.Tables[0];
            DataTable dt1 = ds.Tables[1];


            foreach (DataRow dr in dt.Rows)
            {
                count++;

                //loandes = dr1["loan_description"].ToString();
                loandes = dr["loan_description"].ToString();
                if (oldloandes != loandes)
                {
                    //if (oldmonth != dm)
                    //{
                    //    crm = new CommonReportModel
                    //    {
                    //        RowId = RowCnt++,
                    //        HRF = "H",
                    //        SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //         + ReportColHeaderValueOnly(0, dm)

                    //    };
                    //    lst.Add(crm);
                    //}
                    oldmonth = dm;
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        SlNo = "<span style='color:#C8EAFB'>~</span>"
                        + ReportColHeader(0, "Loan Description ", dr["loan_description"].ToString()),
                        column2 = "`",
                        column3 = "`",
                        column4 = "`",
                        column5 = "`",
                        column6 = "`",
                        column7 = "`",
                        column8 = "`",
                        column9 = "`",
                        column10 = "`",
                        column11 = "`",
                        column12 = "`",
                        column13 = "`",
                        column14 = "`",

                    };
                    lst.Add(crm);
                }




              //  string sinterest_rate = dr["interest_rate"].ToString();
              //decimal DTtlinterestrate = Convert.ToDecimal(sinterest_rate) + 0.00M;
              //  string NTtlinterestrate = String.Format("{0:n}", DTtlinterestrate);

                
               
                string sPrincipal_Opening = dr["Principal_Opening"].ToString();
                decimal DTtlPrincipalOpening = Convert.ToDecimal(sPrincipal_Opening) + 0.00M;
                string NTtlPrincipalOpening = String.Format("{0:n}", DTtlPrincipalOpening);

                //column6 = dr["Curr_Month_recieved"].ToString(),



                string sCurr_Month_recieved = dr["Curr_Month_recieved"].ToString();
                decimal DTtlCurrMonthrecieved = Convert.ToDecimal(sCurr_Month_recieved) + 0.00M;
                string NTtlCurrMonthrecieved = String.Format("{0:n}", DTtlCurrMonthrecieved);


                //column7 = dr["Loan_Closing"].ToString(),


                string sLoan_Closing = dr["Loan_Closing"].ToString();
                decimal DTtlLoanClosing = Convert.ToDecimal(sLoan_Closing) + 0.00M;
                string NTtlLoanClosing = String.Format("{0:n}", DTtlLoanClosing);


                //column8 = dr["Interest_Opening"].ToString(),


                string sInterest_Opening = dr["Interest_Opening"].ToString();
                decimal DTtlInterestOpening = Convert.ToDecimal(sInterest_Opening) + 0.00M;
                string NTtlInterestOpening = String.Format("{0:n}", DTtlInterestOpening);



                ////column9 = dr["Interest_Accured"].ToString(),


                //string sInterest_Accured = dr["Interest_Accured"].ToString();
                //decimal DTtlInterestAccured = Convert.ToDecimal(sInterest_Accured) + 0.00M;
                //string NTtlInterestAccured = String.Format("{0:n}", DTtlInterestAccured);


                //column11 = dr["Interest_Closing"].ToString(),


                string sInterest_Closing = dr["Interest_Closing"].ToString();
                decimal DTtlInterestClosing = Convert.ToDecimal(sInterest_Closing) + 0.00M;
                string NTtlInterestClosing = String.Format("{0:n}", DTtlInterestClosing);


                //column12 = dr["PrincipalPaid"].ToString(),


                string sPrincipalPaid = dr["PrincipalPaid"].ToString();
                decimal DTtlPrincipalPaid = Convert.ToDecimal(sPrincipalPaid) + 0.00M;
                string NTtlPrincipalPaid = String.Format("{0:n}", DTtlPrincipalPaid);

                //column10 = dr["Interest_Repaid"].ToString(),

                string sInterest_Repaid = dr["Interest_Repaid"].ToString();
                decimal DTtlInterestRepaid = Convert.ToDecimal(sInterest_Repaid) + 0.00M;
                string NTtlInterestRepaid = String.Format("{0:n}", DTtlInterestRepaid);

                
                crm = new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "R",
                    // SlNo= SNo++.ToString(),
                    SlNo = dr["emp_code"].ToString(),
                    column2 = dr["Employee_Name"].ToString(),
                    column3 = dr["Desig"].ToString(),
                    column4 = ReportColConvertToDecimal(dr["interest_rate"].ToString()),
                    //column4 = NTtlinterestrate.ToString(),
                    column5 = NTtlPrincipalOpening.ToString(),
                    column6 = NTtlCurrMonthrecieved.ToString(),
                    column7 = NTtlLoanClosing.ToString(),
                    column8 = NTtlInterestOpening.ToString(),
                    //column9 = NTtlInterestAccured.ToString(),
                    ////column4 = dr["interest_rate"].ToString(),

                    //column5 = dr["Principal_Opening"].ToString(),
                    //column6 = dr["Curr_Month_recieved"].ToString(),
                    //column7 = dr["Loan_Closing"].ToString(),
                   // column8 = dr["Interest_Opening"].ToString(),
                    column9 = ReportColConvertToDecimal(dr["Interest_Accured"].ToString()),
                    // column10 = dr["Interest_Repaid"].ToString(),
                    column10 = NTtlInterestRepaid.ToString(),
                    //column11 = dr["Interest_Closing"].ToString(),
                    //column12 = dr["PrincipalPaid"].ToString(),
                    column11 = NTtlInterestClosing.ToString(),
                    column12 = NTtlPrincipalPaid.ToString(),
                    column13 = ReportColConvertToDecimal(dr["total_amount"].ToString()),
                    column14 = ReportColConvertToDecimal(dr["total_recovered_amount"].ToString()),
                };
                lst.Add(crm);


                foreach (DataRow dr1 in dt1.Rows)
                {
                    string floandes = dr1["loan_description"].ToString();
                    int count1 = dt.Select().Where(s => s["loan_description"].ToString() == floandes).Count();
                    if (floandes == loandes)
                    {
                        if (count == count1)
                        {
                            count = 0;
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",
                                SlNo = "Total",
                                column4 = ReportColConvertToDecimal(dr1["interest_rate"].ToString()),
                                column5 = ReportColConvertToDecimal(dr1["Principal_Opening"].ToString()),
                                column6 = ReportColConvertToDecimal(dr1["Curr_Month_recieved"].ToString()),
                                column7 = ReportColConvertToDecimal(dr1["Loan_Closing"].ToString()),
                                column8 = ReportColConvertToDecimal(dr1["Interest_Opening"].ToString()),
                                column9 = ReportColConvertToDecimal(dr1["Interest_Accured"].ToString()),
                                column10 = ReportColConvertToDecimal(dr1["Interest_Repaid"].ToString()),
                                column11 = ReportColConvertToDecimal(dr1["Interest_Closing"].ToString()),
                                column12 = ReportColConvertToDecimal(dr1["PrincipalPaid"].ToString()),
                                column13 = ReportColConvertToDecimal(dr["total_amount"].ToString()),
                                column14 = ReportColConvertToDecimal(dr["total_recovered_amount"].ToString()),
                                //column4 = dr1["interest_rate"].ToString(),
                                //column5 = dr1["Principal_Opening"].ToString(),
                                //column6 = dr1["Curr_Month_recieved"].ToString(),
                                //column7 = dr1["Loan_Closing"].ToString(),
                                //column8 = dr1["Interest_Opening"].ToString(),
                                //column9 = dr1["Interest_Accured"].ToString(),
                                //column10 = dr1["Interest_Repaid"].ToString(),
                                //column11 = dr1["Interest_Closing"].ToString(),
                                //column12 = dr1["PrincipalPaid"].ToString(),
                                //column13 = dr["total_amount"].ToString(),
                                //column14 = dr["total_recovered_amount"].ToString(),
                                //  SlNo = "<span style='color:#eef8fd'>^</span>"
                                //+ ReportColFooterValueOnly(0, "Total")
                                //+ ReportColFooterValueOnly(73, dr1["interest_rate"].ToString())
                                //+ ReportColFooterValueOnly(17, dr1["Principal_Opening"].ToString())
                                //+ ReportColFooterValueOnly(20, dr1["Curr_Month_recieved"].ToString())
                                // + ReportColFooterValueOnly(20, dr1["Loan_Closing"].ToString())
                                //+ ReportColFooterValueOnly(15, dr1["Interest_Opening"].ToString())
                                //+ ReportColFooterValueOnly(30, dr1["Interest_Accured"].ToString())
                                //+ ReportColFooterValueOnly(25, dr1["Interest_Repaid"].ToString())
                                //+ ReportColFooterValueOnly(10, dr1["Interest_Closing"].ToString())

                            };
                            lst.Add(crm);
                        }
                    }

                }
                oldloandes = dr["loan_description"].ToString();
            }
            return lst;
        }
        public async Task<IList<CommonReportModel>> GetSingleRateLoanOutStandingDatahrms(string empcode, string mnth)
        {
            string qry = "";
            //var empid = "0";
            string loancodes = empcode;
            if (mnth.Contains("^"))
            {
                loancodes = "0";
                mnth = "01-01-01";
            }
            DateTime str = Convert.ToDateTime(mnth);
            string str1 = str.ToString("yyyy-MM-dd");
            string dm = str.ToString("MMM-yyyy");
            string oldmonth = "";

            if (empcode != "All")
            {
                q1 = " where l.emp_code in (" + empcode + ") and month(a.fm)=month('" + str1 + "') and year(a.fm)=year('" + str1 + "') and a.active=1 order by  lm.loan_description";
                q2 = " where l.emp_code in (" + empcode + ") and month(a.fm)=month('" + str1 + "') and year(a.fm)=year('" + str1 + "') and a.active=1 group by  lm.loan_description";
            }
            qry = "select lm.loan_description,l.emp_code,e.shortname as Employee_Name,d.Name as Desig, cl.interest_rate," +
                "(a.principal_open_amount) as Principal_Opening,(a.principal_paid_amount  +  a.interest_paid_amount ) as Curr_Month_recieved,a.principal_paid_amount as PrincipalPaid, " +
                "a.principal_balance_amount as Loan_Closing, a.interest_open_amount as Interest_Opening," +
                "a.interest_accured as Interest_Accured, a.interest_paid_amount as Interest_Repaid," +
                "a.interest_balance_amount as Interest_Closing,(cl.loan_amount)as total_amount,(cl.principal_amount_recovered) as total_recovered_amount from pr_emp_adv_loans_adjustments a " +
                "join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid " +
                "join pr_loan_master lm on lm.id=l.loan_type_mid " +
                "join pr_emp_adv_loans_child  cl on l.id = cl.emp_adv_loans_mid and cl.id=a.emp_adv_loans_child_mid " +
                "join Employees e on e.EmpId = l.emp_code " +
                "join Designations d on d.Id = e.CurrentDesignation " + q1 + ";";


            string sumquery = " select lm.loan_description,sum(cl.interest_rate)as interest_rate,sum(a.principal_open_amount) as Principal_Opening," +
                "sum(a.principal_paid_amount  +  a.interest_paid_amount ) as Curr_Month_recieved,sum(a.principal_paid_amount) as PrincipalPaid,sum(a.principal_balance_amount) as Loan_Closing," +
                "sum(a.interest_open_amount) as Interest_Opening,sum(a.interest_accured) as Interest_Accured," +
                "sum(a.interest_paid_amount) as Interest_Repaid,sum(a.interest_balance_amount) as Interest_Closing," +
                "sum(cl.loan_amount)as total_amount,sum(cl. principal_amount_recovered)as total_recovered_amount " +
                "from pr_emp_adv_loans_adjustments a join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid " +
                "join pr_loan_master lm on lm.id = l.loan_type_mid " +
                "join pr_emp_adv_loans_child  cl on l.id = cl.emp_adv_loans_mid and cl.id=a.emp_adv_loans_child_mid " +
                "join Employees e on e.EmpId = l.emp_code " +
                "join Designations d on d.Id = e.CurrentDesignation " + q2 + ";";

            DataSet ds = await _sha.Get_MultiTables_FromQry(qry + sumquery);
            DataTable dt = ds.Tables[0];
            DataTable dt1 = ds.Tables[1];


            foreach (DataRow dr in dt.Rows)
            {
                count++;

                //loandes = dr1["loan_description"].ToString();
                loandes = dr["loan_description"].ToString();
                if (oldloandes != loandes)
                {
                    //if (oldmonth != dm)
                    //{
                    //    crm = new CommonReportModel
                    //    {
                    //        RowId = RowCnt++,
                    //        HRF = "H",
                    //        SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //         + ReportColHeaderValueOnly(0, dm)

                    //    };
                    //    lst.Add(crm);
                    //}
                    oldmonth = dm;
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        SlNo = "<span style='color:#C8EAFB'>~</span>"
                        + ReportColHeader(0, "Loan Description ", dr["loan_description"].ToString()),
                        column2 = "`",
                        column3 = "`",
                        column4 = "`",
                        column5 = "`",
                        column6 = "`",
                        column7 = "`",
                        column8 = "`",
                        column9 = "`",
                        column10 = "`",
                        column11 = "`",
                        column12 = "`",
                        column13 = "`",
                        column14 = "`",

                    };
                    lst.Add(crm);
                }


                crm = new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "R",
                    // SlNo= SNo++.ToString(),
                    SlNo = dr["emp_code"].ToString(),
                    column2 = dr["Employee_Name"].ToString(),
                    column3 = dr["Desig"].ToString(),
                    column4 = dr["interest_rate"].ToString(),
                    column5 = dr["Principal_Opening"].ToString(),
                    column6 = dr["Curr_Month_recieved"].ToString(),
                    column7 = dr["Loan_Closing"].ToString(),
                    column8 = dr["Interest_Opening"].ToString(),
                    column9 = dr["Interest_Accured"].ToString(),
                    column10 = dr["Interest_Repaid"].ToString(),
                    column11 = dr["Interest_Closing"].ToString(),
                    column12 = dr["PrincipalPaid"].ToString(),
                    column13 = dr["total_amount"].ToString(),
                    column14 = dr["total_recovered_amount"].ToString(),
                };
                lst.Add(crm);


                foreach (DataRow dr1 in dt1.Rows)
                {
                    string floandes = dr1["loan_description"].ToString();
                    int count1 = dt.Select().Where(s => s["loan_description"].ToString() == floandes).Count();
                    if (floandes == loandes)
                    {
                        if (count == count1)
                        {
                            count = 0;
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",
                                SlNo = "Total",
                                column4 = dr1["interest_rate"].ToString(),
                                column5 = dr1["Principal_Opening"].ToString(),
                                column6 = dr1["Curr_Month_recieved"].ToString(),
                                column7 = dr1["Loan_Closing"].ToString(),
                                column8 = dr1["Interest_Opening"].ToString(),
                                column9 = dr1["Interest_Accured"].ToString(),
                                column10 = dr1["Interest_Repaid"].ToString(),
                                column11 = dr1["Interest_Closing"].ToString(),
                                column12 = dr1["PrincipalPaid"].ToString(),
                                column13 = dr["total_amount"].ToString(),
                                column14 = dr["total_recovered_amount"].ToString(),
                                //  SlNo = "<span style='color:#eef8fd'>^</span>"
                                //+ ReportColFooterValueOnly(0, "Total")
                                //+ ReportColFooterValueOnly(73, dr1["interest_rate"].ToString())
                                //+ ReportColFooterValueOnly(17, dr1["Principal_Opening"].ToString())
                                //+ ReportColFooterValueOnly(20, dr1["Curr_Month_recieved"].ToString())
                                // + ReportColFooterValueOnly(20, dr1["Loan_Closing"].ToString())
                                //+ ReportColFooterValueOnly(15, dr1["Interest_Opening"].ToString())
                                //+ ReportColFooterValueOnly(30, dr1["Interest_Accured"].ToString())
                                //+ ReportColFooterValueOnly(25, dr1["Interest_Repaid"].ToString())
                                //+ ReportColFooterValueOnly(10, dr1["Interest_Closing"].ToString())

                            };
                            lst.Add(crm);
                        }
                    }

                }
                oldloandes = dr["loan_description"].ToString();
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

