using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PayRollBusiness.PayrollService
{
    class LoansBeforeMonthEndProcess: BusinessBase
    {
        log4net.ILog _logger = null;
        public LoansBeforeMonthEndProcess(LoginCredential loginCredential, log4net.ILog logger) : base(loginCredential)
        {
            _logger = logger;
        }
        StringBuilder sbqry = new StringBuilder();
        public async Task<bool> InstallmentPartPaymentsBeforeMonthend(int loan_id, double instAmt, char PaymentType)
        {
            bool bRet = false;
            try
            {
                //Get Loan Data
                string GetLoans = "SELECT mas.loan_id,mas.id as mastertypeid,chi.priority,chi.Id as childloanid," +
                  "chi.emp_adv_loans_mid,adv.method ,chi.principal_amount_recovered,chi.total_amount_recovered," +
                  "chi.loan_amount,os_principal_amount,chi.os_interest_amount,chi.principal_recovered_flag,chi.interest_recovered_flag," +
                  "chi.loan_sl_no,chi.interest_rate,chi.total_interest_installments,adv.installment_amount,adv.total_installment, " +
                  "adv.interest_installment_amount,adv.completed_installment,adv.total_recovered_amount,adv.sanction_date,chi.date_disburse,chi.interest_amount_recovered,adv.interest_installment " +
                  "FROM pr_emp_adv_loans_bef_monthend adv" +
                  " JOIN pr_loan_master mas ON adv.loan_type_mid = mas.id " +
                  " JOIN pr_emp_adv_loans_child_bef_monthend chi ON adv.id = chi.emp_adv_loans_mid " +
                  " WHERE adv.active = 1 AND chi.emp_adv_loans_mid =" + loan_id + " " +
                  " ORDER BY chi.interest_rate desc;";

                string qryGetfm = "Select fm,fy from pr_month_details where active=1;";

                string qryPartPayments = "SELECT emp_adv_loans_child_mid,cash_paid_on " +
                    "from pr_emp_adv_loans_adjustments_bef_monthend adj " +
                    "join pr_month_details mn on month(adj.cash_paid_on) = month(mn.fm) and year(adj.cash_paid_on) = year(mn.fm) and mn.active = 1 " +
                    "where payment_type = 'Part Payment' and emp_adv_loans_mid = " + loan_id;

                DataSet dtLoansAndMonth = await _sha.Get_MultiTables_FromQry(GetLoans + qryGetfm + qryPartPayments);

                DataTable dtLoans = dtLoansAndMonth.Tables[0];
                DataTable dtFm = dtLoansAndMonth.Tables[1];
                DataTable dtPartPay = dtLoansAndMonth.Tables[2];

                 if (dtLoans.Rows.Count > 0)
                {
                    string loanType = dtLoans.Rows[0]["loan_id"].ToString().ToUpper();

                    //festival loans
                    if (loanType == PrConstants.FESTIVAL_LOAN_CODE)//Raji
                    {
                        bRet = await FestLoanPaymentsBeforeMonthend(dtLoans, dtFm, instAmt, PaymentType);
                    }
                    else if (loanType == PrConstants.PF_LOAN1_CODE || loanType == PrConstants.PF_LOAN2_CODE || loanType == PrConstants.PF_LOANST1_CODE || loanType == PrConstants.PF_LOANST2_CODE || loanType == PrConstants.PF_LOANLT1_CODE || loanType == PrConstants.PF_LOANLT2_CODE || loanType == PrConstants.PF_LOANLT3_CODE || loanType == PrConstants.PF_LOANLT4_CODE) //Indraja
                    {
                        bRet = await PfLoan1PaymentsBeforeMonthend(dtLoans, dtFm, instAmt, PaymentType, dtPartPay);
                    }
                    else if (loanType == PrConstants.VEH_LOANLT6_CODE && (dtLoans.Rows.Count == 1) ||
                        loanType == PrConstants.HOUSING_LOAN_CODE && (dtLoans.Rows.Count == 1)
                        || loanType == PrConstants.HOUSING_2A_LOAN_CODE && (dtLoans.Rows.Count == 1)
                        || loanType == PrConstants.HOUSING_2B_2C_LOAN_CODE && (dtLoans.Rows.Count == 1)
                        || loanType == PrConstants.HOUSING_ADDL_LOAN_CODE && (dtLoans.Rows.Count == 1)
                        || loanType == PrConstants.HOUSING_COMMERCIAL_LOAN_CODE && (dtLoans.Rows.Count == 1)
                        || loanType == PrConstants.HOUSE_LOAN_MAIN && (dtLoans.Rows.Count == 1))
                    {
                        bRet = await TwoWheelerLoanPaymentsBeforeMonthend(dtLoans, dtFm, instAmt, PaymentType, dtPartPay);
                    }
                    else if (loanType == PrConstants.HOUSING_LOAN_CODE && (dtLoans.Rows.Count > 1)
                        || loanType == PrConstants.HOUSING_2A_LOAN_CODE && (dtLoans.Rows.Count > 1)
                        || loanType == PrConstants.HOUSING_2B_2C_LOAN_CODE && (dtLoans.Rows.Count > 1)
                        || loanType == PrConstants.HOUSING_ADDL_LOAN_CODE && (dtLoans.Rows.Count > 1)
                        || loanType == PrConstants.HOUSING_COMMERCIAL_LOAN_CODE && (dtLoans.Rows.Count > 1)
                        || loanType == PrConstants.HOUSE_LOAN_MAIN && (dtLoans.Rows.Count > 1)
                        || loanType == PrConstants.VEH_LOANLT6_CODE && (dtLoans.Rows.Count > 1)
                        || loanType == PrConstants.VEH_LOANLT5_CODE && (dtLoans.Rows.Count > 1)) //Raji
                    {
                        bRet = await House2DLoanPaymentsBeforeMonthend(dtLoans, dtFm, instAmt, PaymentType, dtPartPay);
                    }
                    else if (loanType == PrConstants.VEH_LOANLT5_CODE && (dtLoans.Rows.Count == 1)) //Raji
                    {
                        bRet = await FourWheelerLoanPaymentsBeforeMonthend(dtLoans, dtFm, instAmt, PaymentType, dtPartPay);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            return bRet;

        }

        private async Task<bool> PfLoan1PaymentsBeforeMonthend(DataTable dtLoan, DataTable dtFm, double instAmt, char PaymentType, DataTable dtPrevPartPay)
        {
            double intrestAmt = 0;
            int intrestAmt1 = 0;
            int NewNumIndex = 0;
            bool bRet = false;
            //Declarations
            //string qry = "";
            double Prin_bal_amt = 0;
            double interest_paid_amount = 0;
            double principal_installment = 0;
            double interest_accured = 0;
            int no_of_installments_cover = 0;
            //int Os_Total_Amt = 0;
            int Remaining_installments = 0;
            double interest_bal_amt = 0;
            //1. get data from dtLoan
            //string payment_type = PaymentType.ToString();
            try
            {
                string payment_mode = PaymentType == 'I' ? PrConstants.LOAN_INSTALLMENT : PrConstants.LOAN_PARTPAYMENT;
                string Loan_Code = dtLoan.Rows[0]["loan_id"].ToString();
                int Loan_id = Convert.ToInt32(dtLoan.Rows[0]["mastertypeid"]);
                int Priority = Convert.ToInt32(dtLoan.Rows[0]["priority"]);
                int LnPkid = Convert.ToInt32(dtLoan.Rows[0]["emp_adv_loans_mid"]);
                int ChLnPkid = Convert.ToInt32(dtLoan.Rows[0]["childloanid"]);
                string Method = dtLoan.Rows[0]["method"].ToString();
                //int Principal_amt_Recovered = Convert.ToInt32(dtLoan.Rows[0]["principal_amount_recovered"]);
                //int Total_amt_Recovered = Convert.ToInt32(dtLoan.Rows[0]["total_amount_recovered"]);
                int Loan_amt = Convert.ToInt32(dtLoan.Rows[0]["loan_amount"]);
                int Os_Principal_Amt = Convert.ToInt32(dtLoan.Rows[0]["os_principal_amount"]);
                int Os_Intrst_Amt = Convert.ToInt32(dtLoan.Rows[0]["os_interest_amount"]);
                bool Principal_recovered_flag = bool.Parse(dtLoan.Rows[0]["principal_recovered_flag"].ToString());
                bool Interest_recovered_flag = bool.Parse(dtLoan.Rows[0]["interest_recovered_flag"].ToString());
                int Loan_Sl_No = Convert.ToInt32(dtLoan.Rows[0]["loan_sl_no"]);
                int Install_amt = Convert.ToInt32(dtLoan.Rows[0]["installment_amount"]);
                int Total_installment = Convert.ToInt32(dtLoan.Rows[0]["total_installment"]);
                int Completed_installments = Convert.ToInt32(dtLoan.Rows[0]["completed_installment"]);
                int Total_amount_recovered = Convert.ToInt32(dtLoan.Rows[0]["total_recovered_amount"]);

                float IntRATE = float.Parse(dtLoan.Rows[0]["interest_rate"].ToString());
                DateTime sandate = Convert.ToDateTime(dtLoan.Rows[0]["sanction_date"]);

                //Payments Dates
                DateTime paidDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                string installments_paid_date = (paidDate.ToString("yyyy-MM-dd"));
                string Cash_paid_on = installments_paid_date;
                int fy = Convert.ToInt32(dtFm.Rows[0]["fy"].ToString());
                string fm = installments_paid_date;
                int Month = paidDate.Month;
                int Year = paidDate.Year;
                DataTable getidfromadj = await _sha.Get_Table_FromQry("select * from pr_emp_adv_loans_adjustments_bef_monthend where  emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_mid=" + LnPkid + " and fm='" + fm + "'");
                if (getidfromadj.Rows.Count > 0)
                {
                    sbqry.Append("delete from  pr_emp_adv_loans_adjustments_bef_monthend where emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_mid=" + LnPkid + " and fm='" + fm + "' ");
                }

                DateTime prePartPayDate = new DateTime(1900, 01, 01);
                if (dtPrevPartPay.Rows.Count > 0)
                {
                    prePartPayDate = Convert.ToDateTime(dtPrevPartPay.Rows[0]["cash_paid_on"].ToString());
                }
                Prin_bal_amt = Os_Principal_Amt;
                //find intrest
                if (Os_Principal_Amt != Install_amt)
                {
                    intrestAmt = Convert.ToInt32(await calcIntrestAmt(Os_Principal_Amt, IntRATE, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate));
                    interest_accured = intrestAmt;
                }

                if (Os_Intrst_Amt > 0)
                {
                    intrestAmt = intrestAmt + Os_Intrst_Amt;
                    interest_paid_amount = intrestAmt;
                }
                if (instAmt != 0)
                {
                    principal_installment = instAmt - intrestAmt;
                    Prin_bal_amt = Os_Principal_Amt - principal_installment;
                    Os_Intrst_Amt = 0;
                }
                else
                {
                    principal_installment = instAmt;
                    Prin_bal_amt = Os_Principal_Amt - principal_installment;
                    Os_Intrst_Amt = 0;
                    interest_bal_amt = intrestAmt;
                }

                //Newly added on 25/05/2020
                if(instAmt!=0 && Install_amt!=0 &&(Install_amt> instAmt|| Install_amt== instAmt))
                {
                    no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString()); 
                }
                else
                {
                    no_of_installments_cover = 0;
                }
                //end

                //no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString()); //commented on 25/05/2020
                Completed_installments = Completed_installments + no_of_installments_cover;
                Remaining_installments = Total_installment - Completed_installments;
                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                sbqry.Append(GenNewTransactionString());
                //loan Adjust
                sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                    "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_balance_amount]," +
                    "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                    "[installments_amount],[active],[trans_id],[fm],[fy],[interest_accured],[interest_paid_amount],loan_sl_no) values "
                   + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + Os_Principal_Amt + "," + principal_installment + "," + Prin_bal_amt + "," +
                   "" + intrestAmt + "," + interest_bal_amt + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                   "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + instAmt + "," +
                   "" + no_of_installments_cover + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + interest_accured + "," + interest_paid_amount + "," + Loan_Sl_No + ");");

               // sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));

                string updInstallAmtParent = "";
                string updChild = "";
                if (Prin_bal_amt > 0 && Prin_bal_amt < Install_amt)
                {
                    //Os_Intrst_Amt = Convert.ToInt32(dtLoan.Rows[0]["os_interest_amount"]);
                    intrestAmt1 = Convert.ToInt32(await calcIntrestAmt(Prin_bal_amt, IntRATE, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate));
                    Prin_bal_amt = Prin_bal_amt + intrestAmt1 + Os_Intrst_Amt;
                    updInstallAmtParent = ", installment_amount=" + Prin_bal_amt;
                }

                if (Prin_bal_amt == 0)
                {
                    updInstallAmtParent += ", Active=0";
                    updChild = ",principal_recovered_flag = 1,interest_recovered_flag=1,active = 0";
                }

                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child_bef_monthend", ChLnPkid.ToString(), Install_amt.ToString()));
                sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET principal_amount_recovered=principal_amount_recovered+" + principal_installment
                    + ",total_amount_recovered=total_amount_recovered+" + Install_amt
                    + ",interest_amount_recovered=interest_amount_recovered+" + intrestAmt
                    + ",interest_accured=interest_accured+" + intrestAmt
                    + " , os_principal_amount=" + Prin_bal_amt
                    + ", os_total_amount= " + Prin_bal_amt
                    + updChild
                    + ", os_interest_amount = " + Os_Intrst_Amt
                    + " where id=" + ChLnPkid + " AND active=1;");

                //sbqry.Append("UPDATE pr_emp_adv_loans SET completed_installment=" + Completed_installments
                //    + updInstallAmtParent
                //    + ",remaining_installment=" + Remaining_installments
                //    + " ,total_recovered_amount=total_recovered_amount+" + Install_amt
                //    + " where id=" + LnPkid + " ;");

                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
                //execute sql statements in one shot
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            return bRet;
        }
        //fest loan
        private async Task<bool> FestLoanPaymentsBeforeMonthend(DataTable dtLoan, DataTable dtFm, double instAmt, char PaymentType)
        {
            bool bRet = false;
            //Declarations
            int NewNumIndex = 0;
            double Prin_bal_amt = 0;
            int no_of_installments_cover = 0;
            double Os_Total_Amt = 0;
            int Remaining_installments = 0;
            //1. get data from dtLoan
            //string payment_type = PaymentType.ToString();
            try
            {
                string payment_mode = PaymentType == 'I' ? PrConstants.LOAN_INSTALLMENT : PrConstants.LOAN_PARTPAYMENT;
                string Loan_Code = dtLoan.Rows[0]["loan_id"].ToString();
                int Loan_id = Convert.ToInt32(dtLoan.Rows[0]["mastertypeid"]);
                int Priority = Convert.ToInt32(dtLoan.Rows[0]["priority"]);
                int LnPkid = Convert.ToInt32(dtLoan.Rows[0]["emp_adv_loans_mid"]);
                int ChLnPkid = Convert.ToInt32(dtLoan.Rows[0]["childloanid"]);
                string Method = dtLoan.Rows[0]["method"].ToString();
                //int Principal_amt_Recovered = Convert.ToInt32(dtLoan.Rows[0]["principal_amount_recovered"]);
                //int Total_amt_Recovered = Convert.ToInt32(dtLoan.Rows[0]["total_amount_recovered"]);
                int Loan_amt = Convert.ToInt32(dtLoan.Rows[0]["loan_amount"]);
                double Os_Principal_Amt = Convert.ToInt32(dtLoan.Rows[0]["os_principal_amount"]);
                int Os_Intrst_Amt = Convert.ToInt32(dtLoan.Rows[0]["os_interest_amount"]);
                bool Principal_recovered_flag = bool.Parse(dtLoan.Rows[0]["principal_recovered_flag"].ToString());
                bool Interest_recovered_flag = bool.Parse(dtLoan.Rows[0]["interest_recovered_flag"].ToString());
                int Loan_Sl_No = Convert.ToInt32(dtLoan.Rows[0]["loan_sl_no"]);
                int Install_amt = Convert.ToInt32(dtLoan.Rows[0]["installment_amount"]);
                int Total_installment = Convert.ToInt32(dtLoan.Rows[0]["total_installment"]);
                int Completed_installments = Convert.ToInt32(dtLoan.Rows[0]["completed_installment"]);
                int Total_amount_recovered = Convert.ToInt32(dtLoan.Rows[0]["total_recovered_amount"]);
                Prin_bal_amt = Os_Principal_Amt - instAmt;

                //Newly added on 25/05/2020
                if(instAmt!=0 && Install_amt!=0 && (Install_amt>instAmt|| Install_amt==instAmt ))
                {
                    no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
                }
                else
                {
                    no_of_installments_cover = 0;
                }
                //End

                //no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString()); //Commented on 25/05/2020
                Completed_installments = Completed_installments + no_of_installments_cover;
                Remaining_installments = Total_installment - Completed_installments;
                //Principal_amt_Recovered = Principal_amt_Recovered + instAmt;
                //Total_amount_recovered = Principal_amt_Recovered;
                Os_Total_Amt = Prin_bal_amt;
                //Payments Dates
                DateTime paidDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                string installments_paid_date = (paidDate.ToString("yyyy-MM-dd"));
                string Cash_paid_on = installments_paid_date;
                int fy = Convert.ToInt32(dtFm.Rows[0]["fy"].ToString());
                string fm = installments_paid_date;
                int Month = paidDate.Month;
                int Year = paidDate.Year;
                DataTable getidfromadj = await _sha.Get_Table_FromQry("select * from pr_emp_adv_loans_adjustments_bef_monthend where  emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_mid="+LnPkid+" and fm='"+ fm + "'");
                if (getidfromadj.Rows.Count>0)
                {
                    sbqry.Append("delete from  pr_emp_adv_loans_adjustments_bef_monthend where emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_mid=" + LnPkid + " and fm='" + fm + "' ");
                }
                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                sbqry.Append(GenNewTransactionString());
                //loan Adjust
                sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                    "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                    "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                    "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                   + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + Os_Principal_Amt + "," + instAmt + "," + Prin_bal_amt + "," +
                   "" + 0 + "," + 0 + "," + 0 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                   "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + instAmt + "," +
                   "" + no_of_installments_cover + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");

                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));

                //festival
                string updInstallAmtParent = "";
                string updChild = "";
                if (Prin_bal_amt > 0 && Prin_bal_amt < Install_amt)
                {
                    updInstallAmtParent = ", installment_amount=" + Prin_bal_amt;
                }
                if (Prin_bal_amt == 0)
                {
                    updInstallAmtParent += ", Active=0";
                    updChild = ",principal_recovered_flag = 1,active = 0";
                }

                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child_bef_monthend", ChLnPkid.ToString(), Install_amt.ToString()));
                //sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET principal_amount_recovered=principal_amount_recovered+" + Install_amt
                //    + ",total_amount_recovered=total_amount_recovered+" + Install_amt
                //    + " , os_principal_amount=" + Prin_bal_amt
                //    + ", os_total_amount= " + Os_Total_Amt
                //    + updChild
                //    + " where id=" + ChLnPkid + " AND active=1;");

                //sbqry.Append("UPDATE pr_emp_adv_loans SET completed_installment=" + Completed_installments
                //    + updInstallAmtParent
                //    + ",remaining_installment=" + Remaining_installments
                //    + " ,total_recovered_amount=total_recovered_amount+" + Install_amt
                //    + " where id=" + LnPkid + " ;");

                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
                //execute sql statements in one shot
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            return bRet;
        }
        //4w loan
        private async Task<bool> FourWheelerLoanPaymentsBeforeMonthend(DataTable dtLoan, DataTable dtFm, double instAmt, char PaymentType, DataTable dtPrevPartPay)
        {
            bool bRet = false;
            double Prin_bal_amt = 0;
            double Prin_Paid_amt = 0;
            double Prin_Open_amt = 0;
            int no_of_installments_cover = 0;
            double Os_Total_Amt = 0;
            int Remaining_installments = 0;
            double intrestAmt = 0;
            double Interest_Paid_amt = 0;
            double Interest_Open_amt = 0;
            double Interest_Bal_amt = 0;
            string intrst_accured = "";
            int NewNumIndex = 0;
            try
            {
                //1. get data from dtLoan
                //string payment_type = PaymentType.ToString();
                string payment_mode = PaymentType == 'I' ? PrConstants.LOAN_INSTALLMENT : PrConstants.LOAN_PARTPAYMENT;
                string Loan_Code = dtLoan.Rows[0]["loan_id"].ToString();
                int Loan_id = Convert.ToInt32(dtLoan.Rows[0]["mastertypeid"]);
                int Priority = Convert.ToInt32(dtLoan.Rows[0]["priority"]);
                int LnPkid = Convert.ToInt32(dtLoan.Rows[0]["emp_adv_loans_mid"]);
                int ChLnPkid = Convert.ToInt32(dtLoan.Rows[0]["childloanid"]);
                string Method = dtLoan.Rows[0]["method"].ToString();
                double Principal_amt_Recovered = Convert.ToDouble(dtLoan.Rows[0]["principal_amount_recovered"]);
                double Interest_amount_recovered = Convert.ToDouble(dtLoan.Rows[0]["interest_amount_recovered"]);
                double Total_amt_Recovered = Convert.ToDouble(dtLoan.Rows[0]["total_amount_recovered"]);
                int Loan_amt = Convert.ToInt32(dtLoan.Rows[0]["loan_amount"]);
                double Os_Principal_Amt = Convert.ToDouble(dtLoan.Rows[0]["os_principal_amount"]);
                double Os_Intrst_Amt = Convert.ToDouble(dtLoan.Rows[0]["os_interest_amount"]);
                bool Principal_recovered_flag = bool.Parse(dtLoan.Rows[0]["principal_recovered_flag"].ToString());
                bool Interest_recovered_flag = bool.Parse(dtLoan.Rows[0]["interest_recovered_flag"].ToString());
                int Loan_Sl_No = Convert.ToInt32(dtLoan.Rows[0]["loan_sl_no"]);
                double Install_amt = Convert.ToDouble(dtLoan.Rows[0]["installment_amount"]);
                double Intrest_Install_amt = Convert.ToDouble(dtLoan.Rows[0]["interest_installment_amount"]);
                int Total_installment = Convert.ToInt32(dtLoan.Rows[0]["total_installment"]);
                int Completed_installments = Convert.ToInt32(dtLoan.Rows[0]["completed_installment"]);
                double Total_amount_recovered = Convert.ToDouble(dtLoan.Rows[0]["total_recovered_amount"]);
                float IntRATE = float.Parse(dtLoan.Rows[0]["interest_rate"].ToString());
                DateTime sandate = Convert.ToDateTime(dtLoan.Rows[0]["sanction_date"]);
                int Interest_installments = Convert.ToInt32(dtLoan.Rows[0]["interest_installment"]);
                double Interest_installment_amt = Convert.ToDouble(dtLoan.Rows[0]["interest_installment_amount"]);
                //DataTable getidfromadj = await _sha.Get_Table_FromQry("select emp_adv_loans_child_mid from pr_emp_adv_loans_adjustments_bef_monthend where  active=1 and emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_mid=" + LnPkid + "");
                //if (Convert.ToInt32(getidfromadj.Rows[0]["emp_adv_loans_child_mid"]) == ChLnPkid)
                //{
                //    sbqry.Append("update  pr_emp_adv_loans_adjustments_bef_monthend set active=0 where  active=1 and emp_adv_loans_child_mid=" + ChLnPkid + "");
                //}
                
                DateTime prePartPayDate = new DateTime(1900, 01, 01);
                //Payments Dates
                DateTime paidDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                DateTime pPaydate = Convert.ToDateTime(dtLoan.Rows[0]["date_disburse"].ToString());
                string installments_paid_date = (paidDate.ToString("yyyy-MM-dd"));
                string Cash_paid_on = installments_paid_date;
                int fy = Convert.ToInt32(dtFm.Rows[0]["fy"].ToString());
                string fm = installments_paid_date;
                int Month = paidDate.Month;
                int Year = paidDate.Year;
                DataTable getidfromadj = await _sha.Get_Table_FromQry("select * from pr_emp_adv_loans_adjustments_bef_monthend where  emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_mid=" + LnPkid + " and fm='" + fm + "'");
                if (getidfromadj.Rows.Count > 0)
                {
                    sbqry.Append("delete from  pr_emp_adv_loans_adjustments_bef_monthend where  emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_mid=" + LnPkid + " and fm='" + fm + "' ");
                }
                //when part pay interest calculation
                if (dtPrevPartPay.Rows.Count > 0)
                {
                    prePartPayDate = Convert.ToDateTime(dtPrevPartPay.Rows[0]["cash_paid_on"].ToString());
                }
                //when monthend interest calculation
                if (Os_Principal_Amt != 0)
                {
                    intrestAmt = await calcIntrestAmt(Os_Principal_Amt, IntRATE, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate);
                }

                //when loan adjustment
                if (Os_Principal_Amt < instAmt && Os_Principal_Amt > 0)
                {
                    Prin_Open_amt = Os_Principal_Amt;
                    Principal_amt_Recovered = Principal_amt_Recovered + Os_Principal_Amt;
                    Total_amount_recovered = Principal_amt_Recovered;
                    double loan_adj_amt = instAmt - Os_Principal_Amt;
                    Prin_Paid_amt = Os_Principal_Amt;
                    Os_Principal_Amt = 0;
                    Prin_bal_amt = 0;
                    instAmt = instAmt - loan_adj_amt;
                    if (loan_adj_amt <= Os_Intrst_Amt)
                    {
                        Interest_Open_amt = Os_Intrst_Amt;
                        Os_Intrst_Amt = Os_Intrst_Amt - loan_adj_amt;
                        intrestAmt = Os_Intrst_Amt;
                        Interest_Paid_amt = loan_adj_amt;
                        Interest_amount_recovered = Interest_amount_recovered + Interest_Paid_amt;
                        Interest_Bal_amt = Os_Intrst_Amt;
                    }
                }
                //When Interest PAy
                else if (Os_Principal_Amt == 0 && Os_Intrst_Amt > 0)
                {
                    Interest_Open_amt = Os_Intrst_Amt;
                    Interest_Bal_amt = Interest_Open_amt - instAmt;
                    Interest_Paid_amt = instAmt;
                    Interest_amount_recovered = Interest_amount_recovered + Interest_Paid_amt;
                    Os_Intrst_Amt = Os_Intrst_Amt - Interest_Paid_amt;
                    if (Os_Intrst_Amt <= 0 || Os_Intrst_Amt < 1)
                    {
                        Os_Intrst_Amt = 0;
                        Interest_Bal_amt = 0;
                    }

                    //Newly added on 25/05/2020
                    if (instAmt != 0 && Intrest_Install_amt != 0 && (Intrest_Install_amt > instAmt || Intrest_Install_amt == instAmt))
                    {
                        no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Intrest_Install_amt)).ToString());
                    }
                    else
                    {
                        no_of_installments_cover = 0;
                    }
                    //End

                    //no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Intrest_Install_amt)).ToString()); //Commented on 25/05/2020
                    Completed_installments = Completed_installments + no_of_installments_cover;
                    Remaining_installments = Total_installment - Completed_installments;
                    Prin_Paid_amt = 0;
                    Install_amt = 0;

                }
                //When Principal PAy
                else
                {
                    Prin_Open_amt = Os_Principal_Amt;
                    Interest_Open_amt = Os_Intrst_Amt;
                    Interest_Bal_amt = Convert.ToDouble(intrestAmt + Os_Intrst_Amt);
                    Prin_Paid_amt = instAmt;
                    Prin_bal_amt = Os_Principal_Amt - Prin_Paid_amt;

                    //Newly Added on 25/05/2020
                    if(instAmt!= 0 && Install_amt!=0 && (Install_amt> instAmt|| Install_amt== instAmt))
                    {
                        no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
                    }
                    else
                    {
                        no_of_installments_cover = 0;
                    }
                    //End

                    //no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString()); //commented on 25/05/2020
                    Completed_installments = Completed_installments + no_of_installments_cover;
                    Remaining_installments = Total_installment - Completed_installments;
                    Principal_amt_Recovered = Principal_amt_Recovered + instAmt;
                    Total_amount_recovered = Principal_amt_Recovered;
                    // Os_Principal_Amt = Prin_bal_amt;
                }


                if (intrestAmt > 0 && Principal_recovered_flag == false)
                {
                    //Interest_Bal_amt = Convert.ToDouble(Interest_Bal_amt + intrestAmt);
                    intrst_accured = ", interest_accured=" + intrestAmt + ",os_interest_amount=" + Convert.ToDouble(Os_Intrst_Amt + intrestAmt);
                }
                else
                {
                    intrst_accured = ", interest_accured=" + intrestAmt + ",os_interest_amount=" + Convert.ToDouble(Os_Intrst_Amt + intrestAmt);

                }


                //First Installment
                if (Os_Principal_Amt == Loan_amt)
                {
                    //int Os_Principal_Amt_update = Prin_bal_amt+Os_Principal_Amt;
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + Prin_Open_amt + "," + Prin_Paid_amt + "," + Prin_bal_amt + "," + intrestAmt + "," +
                  "" + Os_Intrst_Amt + "," + 0 + "," + Interest_Bal_amt + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + instAmt + "," +
                  "" + no_of_installments_cover + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));
                }
                //Other than First Installment
                else
                {

                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                        "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                        "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                        "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                       + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + Prin_Open_amt + "," + Prin_Paid_amt + "," + Prin_bal_amt + "," + intrestAmt + "," +
                       "" + Interest_Open_amt + "," + Interest_Paid_amt + "," + Interest_Bal_amt + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                       "" + Completed_installments + ",'" + installments_paid_date + "'," + instAmt + "," +
                       "" + Completed_installments + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));
                }
                Os_Principal_Amt = Prin_bal_amt;
                Os_Total_Amt = Convert.ToDouble(Prin_bal_amt + Os_Intrst_Amt + intrestAmt);
                //3. child loan update
                //sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET principal_amount_recovered=" + Principal_amt_Recovered
                //    + intrst_accured + ",interest_amount_recovered=" + Interest_amount_recovered + ""
                //    + ",total_amount_recovered=" + Total_amount_recovered + " , os_principal_amount=" + Os_Principal_Amt
                //    + ", os_total_amount= " + Os_Total_Amt + " where id=" + ChLnPkid + " AND active=1;");

                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child_bef_monthend", ChLnPkid.ToString(), Install_amt.ToString()));
                //4. parent loan update
                string updInstallAmt = "";
                //update instalamt when Os_Principal_Amt Lessthan installmentAmt 
                //if (Os_Principal_Amt > 0 && Os_Principal_Amt < Install_amt && Os_Total_Amt < instAmt)
                //{
                //    updInstallAmt = ", installment_amount=" + Os_Principal_Amt;
                //    sbqry.Append("UPDATE pr_emp_adv_loans_bef_monthend SET completed_installment=" + Completed_installments
                //   + updInstallAmt
                //   + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                //   + Total_amount_recovered + ",active=1 " + " where id=" + LnPkid + " ;");
                //}
                // principal_recovered_flag = 1 
                //if (Os_Principal_Amt == 0 && Principal_recovered_flag == false)
                //{
                //    sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET principal_recovered_flag = 1 where emp_adv_loans_mid=" + LnPkid + " ;");
                //    Interest_installment_amt = Interest_Bal_amt / Interest_installments;
                //}
                //loan close
                //if (Os_Intrst_Amt == 0 && intrestAmt == 0 && Os_Intrst_Amt == 0)
                //{
                //    sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET Interest_recovered_flag = 1,active=0 where emp_adv_loans_mid=" + LnPkid + " ;");

                //    sbqry.Append("UPDATE pr_emp_adv_loans_bef_monthend SET completed_installment=" + Completed_installments
                //     + updInstallAmt
                //     + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                //     + Total_amount_recovered + ",active=0 " + " where id=" + LnPkid + " ;");

                //    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
                //}
                //when installment processing
                //else
                //{
                //    if (Os_Intrst_Amt < Interest_installment_amt)
                //    {
                //        Interest_installment_amt = Os_Intrst_Amt;
                //    }
                //    sbqry.Append("UPDATE pr_emp_adv_loans_bef_monthend SET completed_installment=" + Completed_installments
                //        + updInstallAmt
                //        + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                //        + Total_amount_recovered + ",interest_installment_amount=" + Interest_installment_amt + ",active=1 " + " where id=" + LnPkid + " ;");

                //    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
                //}
                //execute sql statements in one shot
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            return bRet;
        }
        //2w loan
        private async Task<bool> TwoWheelerLoanPaymentsBeforeMonthend(DataTable dtLoan, DataTable dtFm, double instAmt, char PaymentType, DataTable dtPrevPartPay)
        {
            bool bRet = false;
            double Prin_Open_amt = 0;
            double Prin_bal_amt = 0;
            double Prin_Paid_amt = 0;
            int no_of_installments_cover = 0;
            double Os_Total_Amt = 0;
            int Remaining_installments = 0;
            double intrestAmt = 0;
            double Interest_Paid_amt = 0;
            double Interest_Open_amt = 0;
            double Interest_Bal_amt = 0;
            string intrst_accured = "";
            int NewNumIndex = 0;
            try
            {
                //1. get data from dtLoan
                //string payment_type = PaymentType.ToString();
                string payment_mode = PaymentType == 'I' ? PrConstants.LOAN_INSTALLMENT : PrConstants.LOAN_PARTPAYMENT;
                string Loan_Code = dtLoan.Rows[0]["loan_id"].ToString();
                int Loan_id = Convert.ToInt32(dtLoan.Rows[0]["mastertypeid"]);
                int Priority = Convert.ToInt32(dtLoan.Rows[0]["priority"]);
                int LnPkid = Convert.ToInt32(dtLoan.Rows[0]["emp_adv_loans_mid"]);
                int ChLnPkid = Convert.ToInt32(dtLoan.Rows[0]["childloanid"]);
                string Method = dtLoan.Rows[0]["method"].ToString();
                double Principal_amt_Recovered = Convert.ToDouble(dtLoan.Rows[0]["principal_amount_recovered"]);
                double Interest_amount_recovered = Convert.ToDouble(dtLoan.Rows[0]["interest_amount_recovered"]);
                double Total_amt_Recovered = Convert.ToDouble(dtLoan.Rows[0]["total_amount_recovered"]);
                int Loan_amt = Convert.ToInt32(dtLoan.Rows[0]["loan_amount"]);
                double Os_Principal_Amt = Convert.ToDouble(dtLoan.Rows[0]["os_principal_amount"]);
                double Os_Intrst_Amt = Convert.ToDouble(dtLoan.Rows[0]["os_interest_amount"]);
                bool Principal_recovered_flag = bool.Parse(dtLoan.Rows[0]["principal_recovered_flag"].ToString());
                bool Interest_recovered_flag = bool.Parse(dtLoan.Rows[0]["interest_recovered_flag"].ToString());
                int Loan_Sl_No = Convert.ToInt32(dtLoan.Rows[0]["loan_sl_no"]);
                double Install_amt = Convert.ToDouble(dtLoan.Rows[0]["installment_amount"]);
                double Intrest_Install_amt = Convert.ToDouble(dtLoan.Rows[0]["interest_installment_amount"]);
                int Total_installment = Convert.ToInt32(dtLoan.Rows[0]["total_installment"]);
                int Completed_installments = Convert.ToInt32(dtLoan.Rows[0]["completed_installment"]);
                double Total_amount_recovered = Convert.ToDouble(dtLoan.Rows[0]["total_recovered_amount"]);
                float IntRATE = float.Parse(dtLoan.Rows[0]["interest_rate"].ToString());
                DateTime sandate = Convert.ToDateTime(dtLoan.Rows[0]["sanction_date"]);
                int Interest_installments = Convert.ToInt32(dtLoan.Rows[0]["interest_installment"]);
                double Interest_installment_amt = Convert.ToDouble(dtLoan.Rows[0]["interest_installment_amount"]);

                DateTime prePartPayDate = new DateTime(1900, 01, 01);
                //Payments Dates
                DateTime paidDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                DateTime pPaydate = Convert.ToDateTime(dtLoan.Rows[0]["date_disburse"].ToString());
                string installments_paid_date = (paidDate.ToString("yyyy-MM-dd"));
                string Cash_paid_on = installments_paid_date;
                int fy = Convert.ToInt32(dtFm.Rows[0]["fy"].ToString());
                string fm = installments_paid_date;
                int Month = paidDate.Month;
                int Year = paidDate.Year;
                //DataTable getidfromadj = await _sha.Get_Table_FromQry("select emp_adv_loans_child_mid from pr_emp_adv_loans_adjustments_bef_monthend where  active=1 and emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_mid=" + LnPkid + "");
                //if (Convert.ToInt32(getidfromadj.Rows[0]["emp_adv_loans_child_mid"]) == ChLnPkid)
                //{
                //    sbqry.Append("update  pr_emp_adv_loans_adjustments_bef_monthend set active=0 where  active=1 and emp_adv_loans_child_mid=" + ChLnPkid + "");
                //}
                DataTable getidfromadj = await _sha.Get_Table_FromQry("select * from pr_emp_adv_loans_adjustments_bef_monthend where  emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_mid=" + LnPkid + " and fm='" + fm + "'");
                if (getidfromadj.Rows.Count > 0)
                {
                    sbqry.Append("delete from  pr_emp_adv_loans_adjustments_bef_monthend where emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_mid=" + LnPkid + " and fm='" + fm + "' ");
                }


                //when part pay interest calculation
                if (dtPrevPartPay.Rows.Count > 0)
                {
                    prePartPayDate = Convert.ToDateTime(dtPrevPartPay.Rows[0]["cash_paid_on"].ToString());
                }
                //when monthend interest calculation
                if (Os_Principal_Amt != 0)
                {
                    intrestAmt = await calcIntrestAmt(Os_Principal_Amt, IntRATE, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate);
                }
                //when loan adjustment
                if (Os_Principal_Amt < instAmt && Os_Principal_Amt > 0)
                {
                    Prin_Open_amt = Os_Principal_Amt;
                    Principal_amt_Recovered = Principal_amt_Recovered + Os_Principal_Amt;
                    Total_amount_recovered = Principal_amt_Recovered;
                    double loan_adj_amt = instAmt - Os_Principal_Amt;
                    Prin_Paid_amt = Os_Principal_Amt;
                    Os_Principal_Amt = 0;
                    Prin_bal_amt = 0;
                    instAmt = instAmt - loan_adj_amt;
                    if (loan_adj_amt <= Os_Intrst_Amt)
                    {
                        Interest_Open_amt = Os_Intrst_Amt;
                        Os_Intrst_Amt = Os_Intrst_Amt - loan_adj_amt;
                        intrestAmt = Os_Intrst_Amt;
                        Interest_Paid_amt = loan_adj_amt;
                        Interest_amount_recovered = Interest_amount_recovered + Interest_Paid_amt;
                        Interest_Bal_amt = Os_Intrst_Amt;
                    }
                }
                //When Interest PAy
                else if (Os_Principal_Amt == 0 && Os_Intrst_Amt > 0)
                {
                    Interest_Open_amt = Os_Intrst_Amt;
                    Interest_Bal_amt = Interest_Open_amt - instAmt;
                    Interest_Paid_amt = instAmt;
                    Interest_amount_recovered = Interest_amount_recovered + Interest_Paid_amt;
                    Os_Intrst_Amt = Os_Intrst_Amt - Interest_Paid_amt;
                    if (Os_Intrst_Amt <= 0 || Os_Intrst_Amt < 1)
                    {
                        Os_Intrst_Amt = 0;
                        Interest_Bal_amt = 0;
                    }
                    //newly added on 25/05/2020
                    if (instAmt != 0 && Intrest_Install_amt != 0 && ((Intrest_Install_amt > instAmt) || (Intrest_Install_amt == instAmt)))
                    {
                        no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Intrest_Install_amt)).ToString());
                    }
                    else
                    {
                        no_of_installments_cover = 0;
                    }
                    //end

                    //no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Intrest_Install_amt)).ToString());
                    Completed_installments = Completed_installments + no_of_installments_cover;
                    Remaining_installments = Total_installment - Completed_installments;
                    Prin_Paid_amt = 0;
                    Install_amt = 0;
                }
                //When Principal PAy
                else
                {
                    Prin_Open_amt = Os_Principal_Amt;
                    Interest_Open_amt = Os_Intrst_Amt;
                    Interest_Bal_amt = Convert.ToDouble(intrestAmt + Os_Intrst_Amt);
                    Prin_bal_amt = Os_Principal_Amt - instAmt;
                    //newly added on 25/05/2020
                    if(instAmt!=0 && Install_amt!=0 && ((Install_amt> instAmt)||(Install_amt== instAmt)))
                    {
                        no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
                    }
                    else
                    {
                        no_of_installments_cover = 0;
                    }
                    //end

                    //no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
                    Completed_installments = Completed_installments + no_of_installments_cover;
                    Remaining_installments = Total_installment - Completed_installments;
                    Principal_amt_Recovered = Principal_amt_Recovered + instAmt;
                    Total_amount_recovered = Principal_amt_Recovered;
                    Prin_Paid_amt = instAmt;
                    // Os_Principal_Amt = Prin_bal_amt;
                }


                if (intrestAmt > 0 && Principal_recovered_flag == false && Os_Principal_Amt != 0)
                {
                    //Interest_Bal_amt = Convert.ToDouble(Interest_Bal_amt + intrestAmt);
                    intrst_accured = ", interest_accured=" + intrestAmt + ",os_interest_amount=" + Convert.ToDouble(Os_Intrst_Amt + intrestAmt);
                }
                else
                {
                    intrst_accured = ", interest_accured=" + intrestAmt + ",os_interest_amount=" + Convert.ToDouble(Os_Intrst_Amt);

                }


                //First Installment
                if (Os_Principal_Amt == Loan_amt)
                {
                    
                    //int Os_Principal_Amt_update = Prin_bal_amt+Os_Principal_Amt;
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + Os_Principal_Amt + "," + instAmt + "," + Prin_bal_amt + "," + intrestAmt + "," +
                  "" + Os_Intrst_Amt + "," + 0 + "," + Convert.ToInt32(Os_Intrst_Amt + intrestAmt) + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + instAmt + "," +
                  "" + no_of_installments_cover + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));
                }
                //Other than First Installment
                else
                {

                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                        "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                        "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                        "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                       + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + Prin_Open_amt + "," + Prin_Paid_amt + "," + Prin_bal_amt + "," + intrestAmt + "," +
                       "" + Interest_Open_amt + "," + Interest_Paid_amt + "," + Interest_Bal_amt + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                       "" + Completed_installments + ",'" + installments_paid_date + "'," + instAmt + "," +
                       "" + Completed_installments + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));
                }
                Os_Principal_Amt = Prin_bal_amt;
                Os_Total_Amt = Convert.ToDouble(Prin_bal_amt + Os_Intrst_Amt + intrestAmt);
                //3. child loan update
                sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET principal_amount_recovered=" + Principal_amt_Recovered
                    + intrst_accured
                    + ",total_amount_recovered=" + Total_amount_recovered + " , os_principal_amount=" + Os_Principal_Amt
                    + ", os_total_amount= " + Os_Total_Amt + ",interest_amount_recovered=" + Interest_amount_recovered + " where id=" + ChLnPkid + " AND active=1;");

                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child_bef_monthend", ChLnPkid.ToString(), Install_amt.ToString()));
                //4. parent loan update
                string updInstallAmt = "";
                //update instalamt when Os_Principal_Amt Lessthan installmentAmt 
                //if (Os_Principal_Amt > 0 && Os_Principal_Amt < Install_amt && Os_Total_Amt < instAmt)
                //{
                //    updInstallAmt = ", installment_amount=" + Os_Total_Amt;
                //    sbqry.Append("UPDATE pr_emp_adv_loans_bef_monthend SET completed_installment=" + Completed_installments
                //   + updInstallAmt
                //   + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                //   + Total_amount_recovered + ",active=1 " + " where id=" + LnPkid + " ;");
                //}
                //// principal_recovered_flag = 1 
                //if (Os_Principal_Amt == 0 && Principal_recovered_flag == false)
                //{
                //    sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET principal_recovered_flag = 1 where emp_adv_loans_mid=" + LnPkid + " ;");
                //    Interest_installment_amt = Math.Round(Convert.ToDouble(Interest_Bal_amt / Interest_installments), 2);
                //}
                ////loan close
                //if (Os_Intrst_Amt == 0 && intrestAmt == 0 && Os_Intrst_Amt == 0)
                //{
                //    sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET Interest_recovered_flag = 1,active=0 where emp_adv_loans_mid=" + LnPkid + " ;");

                //    sbqry.Append("UPDATE pr_emp_adv_loans_bef_monthend SET completed_installment=" + Completed_installments
                //     + updInstallAmt
                //     + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                //     + Total_amount_recovered + ",active=0 " + " where id=" + LnPkid + " ;");

                //    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_bef_monthend", LnPkid.ToString(), LnPkid.ToString()));
                //}
                ////when installment processing
                //else
                //{
                //    if (Os_Intrst_Amt < Interest_installment_amt)
                //    {
                //        Interest_installment_amt = Os_Intrst_Amt;
                //    }
                //    sbqry.Append("UPDATE pr_emp_adv_loans_bef_monthend SET completed_installment=" + Completed_installments
                //        + updInstallAmt
                //        + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                //        + Total_amount_recovered + ",interest_installment_amount=" + Interest_installment_amt + ",active=1 " + " where id=" + LnPkid + " ;");

                //    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_bef_monthend", LnPkid.ToString(), LnPkid.ToString()));
                //}
                //execute sql statements in one shot
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            return bRet;
        }
        //Housing Loand 2D
        private async Task<bool> House2DLoanPaymentsBeforeMonthend(DataTable dtLoan, DataTable dtFm, double instAmt, char PaymentType, DataTable dtPrevPartPay)
        {
            double os_int_totamt_two_sub_lns = 0;
            bool bRet = false;
            double Prin_bal_amt = 0;
            double Prin_Paid_amt = 0;
            int no_of_installments_cover = 0;
            int Os_Total_Amt = 0;
            int Remaining_installments = 0;
            double intrestAmt = 0;
            double Interest_Paid_amt = 0;
            double Interest_Open_amt = 0;
            double Interest_Bal_amt = 0;
            string intrst_accured = "";
            double loan_adj_amt = 0;
            double Prin_bal_amt1 = 0;
            double Prin_Paid_amt1 = 0;
            int no_of_installments_cover1 = 0;
            int Os_Total_Amt1 = 0;
            double intrestAmt1 = 0;
            double Interest_Paid_amt1 = 0;
            double Interest_Open_amt1 = 0;
            double Interest_Bal_amt1 = 0;
            string intrst_accured1 = "";
            double Total_amount_recovered1 = 0;
            double Install_amt1 = 0;
            double Amount_Paid = 0;
            double prin_open_amt = 0;
            double prin_open_amt1 = 0;
            int NewNumIndex = 0;
            //1. get data from dtLoan
            //string payment_type = PaymentType.ToString();
            try
            {
                string payment_mode = PaymentType == 'I' ? PrConstants.LOAN_INSTALLMENT : PrConstants.LOAN_PARTPAYMENT;
                string Loan_Code = dtLoan.Rows[0]["loan_id"].ToString();
                int Loan_id = Convert.ToInt32(dtLoan.Rows[0]["mastertypeid"]);
                int Priority = Convert.ToInt32(dtLoan.Rows[0]["priority"]);
                int Priority1 = Convert.ToInt32(dtLoan.Rows[1]["priority"]);
                int LnPkid = Convert.ToInt32(dtLoan.Rows[0]["emp_adv_loans_mid"]);
                int ChLnPkid = Convert.ToInt32(dtLoan.Rows[0]["childloanid"]);
                int ChLnPkid1 = Convert.ToInt32(dtLoan.Rows[1]["childloanid"]);
                string Method = dtLoan.Rows[0]["method"].ToString();
                string Method1 = dtLoan.Rows[1]["method"].ToString();
                double Principal_amt_Recovered = Convert.ToDouble(dtLoan.Rows[0]["principal_amount_recovered"]);
                double Principal_amt_Recovered1 = Convert.ToDouble(dtLoan.Rows[1]["principal_amount_recovered"]);
                double Interest_amount_recovered = Convert.ToDouble(dtLoan.Rows[0]["interest_amount_recovered"]);
                double Interest_amount_recovered1 = Convert.ToDouble(dtLoan.Rows[1]["interest_amount_recovered"]);
                double Total_amt_Recovered = Convert.ToDouble(dtLoan.Rows[0]["total_amount_recovered"]);
                double Total_amt_Recovered1 = Convert.ToDouble(dtLoan.Rows[1]["total_amount_recovered"]);
                int Loan_amt = Convert.ToInt32(dtLoan.Rows[0]["loan_amount"]);
                int Loan_amt1 = Convert.ToInt32(dtLoan.Rows[1]["loan_amount"]);
                double Os_Principal_Amt = Convert.ToDouble(dtLoan.Rows[0]["os_principal_amount"]);
                double Os_Principal_Amt1 = Convert.ToDouble(dtLoan.Rows[1]["os_principal_amount"]);
                double Os_Intrst_Amt = Convert.ToDouble(dtLoan.Rows[0]["os_interest_amount"]);
                double Os_Intrst_Amt1 = Convert.ToDouble(dtLoan.Rows[1]["os_interest_amount"]);
                bool Principal_recovered_flag = bool.Parse(dtLoan.Rows[0]["principal_recovered_flag"].ToString());
                bool Principal_recovered_flag1 = bool.Parse(dtLoan.Rows[1]["principal_recovered_flag"].ToString());
                bool Interest_recovered_flag = bool.Parse(dtLoan.Rows[0]["interest_recovered_flag"].ToString());
                bool Interest_recovered_flag1 = bool.Parse(dtLoan.Rows[1]["interest_recovered_flag"].ToString());
                int Loan_Sl_No = Convert.ToInt32(dtLoan.Rows[0]["loan_sl_no"]);
                double Install_amt = Convert.ToDouble(dtLoan.Rows[0]["installment_amount"]);
                int Total_installment = Convert.ToInt32(dtLoan.Rows[0]["total_installment"]);
                int Completed_installments = Convert.ToInt32(dtLoan.Rows[0]["completed_installment"]);
                double Total_amount_recovered = Convert.ToDouble(dtLoan.Rows[0]["total_recovered_amount"]);
                float IntRATE = float.Parse(dtLoan.Rows[0]["interest_rate"].ToString());
                float IntRATE1 = float.Parse(dtLoan.Rows[1]["interest_rate"].ToString());
                DateTime sandate = Convert.ToDateTime(dtLoan.Rows[0]["sanction_date"]);
                int Interest_installments = Convert.ToInt32(dtLoan.Rows[0]["interest_installment"]);
                decimal Interest_installment_amt = Convert.ToDecimal(dtLoan.Rows[0]["interest_installment_amount"]);

                DateTime prePartPayDate = new DateTime(1900, 01, 01);
                //Payments Dates
                DateTime paidDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                DateTime pPaydate = Convert.ToDateTime(dtLoan.Rows[0]["date_disburse"].ToString());
                string installments_paid_date = (paidDate.ToString("yyyy-MM-dd"));
                string Cash_paid_on = installments_paid_date;
                int fy = Convert.ToInt32(dtFm.Rows[0]["fy"].ToString());
                string fm = installments_paid_date;
                int Month = paidDate.Month;
                int Year = paidDate.Year;
                bool adjust = false;

                DataTable getidfromadj = await _sha.Get_Table_FromQry("select * from pr_emp_adv_loans_adjustments_bef_monthend where emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_child_mid="+ ChLnPkid1 + " and emp_adv_loans_mid=" + LnPkid + " and fm='"+fm+"'");
                if (getidfromadj.Rows.Count>0)
                {
                    sbqry.Append("delete from  pr_emp_adv_loans_adjustments_bef_monthend  where emp_adv_loans_child_mid=" + ChLnPkid + " and emp_adv_loans_child_mid=" + ChLnPkid1 + " and emp_adv_loans_mid=" + LnPkid + " and fm=" + fm + "");
                }
                if (dtPrevPartPay.Rows.Count > 0)
                {
                    prePartPayDate = Convert.ToDateTime(dtPrevPartPay.Rows[0]["cash_paid_on"].ToString());
                }

                if (Os_Principal_Amt != 0)
                {
                    intrestAmt = await calcIntrestAmt(Os_Principal_Amt, IntRATE, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate);
                }
                if (Os_Principal_Amt1 != 0)
                {
                    intrestAmt1 = await calcIntrestAmt(Os_Principal_Amt1, IntRATE1, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate);
                }
                //when loan adjustment
                if (Os_Principal_Amt < instAmt && Os_Principal_Amt > 0)
                {
                    prin_open_amt = Os_Principal_Amt;
                    Principal_amt_Recovered = Principal_amt_Recovered + Os_Principal_Amt;
                    Total_amount_recovered = Principal_amt_Recovered;
                    loan_adj_amt = instAmt - Os_Principal_Amt;
                    Prin_Paid_amt = Os_Principal_Amt;
                    Os_Principal_Amt = 0;
                    Prin_bal_amt = 0;
                    instAmt = instAmt - loan_adj_amt;
                    Interest_Open_amt = Os_Intrst_Amt;
                    Os_Intrst_Amt = Convert.ToInt32(Os_Intrst_Amt + intrestAmt);
                    Interest_Bal_amt = Os_Intrst_Amt;
                    adjust = true;
                    if (loan_adj_amt <= Os_Principal_Amt1)
                    {
                        //sub2 Os_Principal_Amt1 pay
                        prin_open_amt1 = Os_Principal_Amt1;
                        Os_Principal_Amt1 = Os_Principal_Amt1 - loan_adj_amt;
                        Principal_amt_Recovered1 = loan_adj_amt;
                        Prin_bal_amt1 = Os_Principal_Amt1;
                        Total_amount_recovered1 = Principal_amt_Recovered1;
                        Prin_Paid_amt1 = loan_adj_amt;
                        Interest_Bal_amt1 = Convert.ToInt32(Os_Intrst_Amt1 + intrestAmt1);

                    }
                    else
                    {
                        if (Os_Principal_Amt1 != 0)
                        {
                            intrestAmt1 = await calcIntrestAmt(Os_Principal_Amt1, IntRATE1, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate);
                        }
                        //sub2 Os_Principal_Amt1 =0
                        //Os_Principal_Amt1 = Os_Principal_Amt1 - loan_adj_amt;
                        Principal_amt_Recovered1 = loan_adj_amt;
                        Prin_bal_amt1 = 0;
                        Total_amount_recovered1 = Principal_amt_Recovered1;
                        loan_adj_amt = loan_adj_amt - Os_Principal_Amt1;
                        Os_Principal_Amt1 = 0;

                        if (loan_adj_amt > 0)
                        {
                            //sub1 interest covering
                            if (Os_Intrst_Amt > loan_adj_amt)
                            {
                                //subtract sub1 Os_Intrst_Amt
                                Os_Intrst_Amt = Os_Intrst_Amt - loan_adj_amt;
                                Total_amount_recovered = Principal_amt_Recovered + loan_adj_amt;
                                Interest_Paid_amt = loan_adj_amt;
                                Interest_Bal_amt = Os_Intrst_Amt;

                            }
                            else
                            {

                                Total_amount_recovered = Principal_amt_Recovered + loan_adj_amt;
                                Interest_Paid_amt = loan_adj_amt;
                                Interest_Bal_amt = 0;
                                loan_adj_amt = loan_adj_amt - Os_Intrst_Amt;
                                Os_Intrst_Amt = 0;

                                //Os_Intrst_Amt =0
                                if (loan_adj_amt > 0)
                                {
                                    if (Os_Intrst_Amt1 > loan_adj_amt)
                                    {
                                        //subtract sub2 Os_Intrst_Amt1

                                        Os_Intrst_Amt1 = Os_Intrst_Amt1 - loan_adj_amt;
                                        Total_amount_recovered1 = Principal_amt_Recovered1 + loan_adj_amt;
                                        Interest_Paid_amt1 = loan_adj_amt;
                                        Interest_Bal_amt1 = Os_Intrst_Amt1;
                                    }
                                    else
                                    {
                                        Total_amount_recovered1 = Principal_amt_Recovered1 + loan_adj_amt;
                                        Interest_Paid_amt1 = loan_adj_amt;
                                        Interest_Bal_amt1 = 0;
                                        loan_adj_amt = loan_adj_amt - Os_Intrst_Amt1;
                                        Os_Intrst_Amt = 0;
                                        //Os_Intrst_Amt1 =0

                                    }
                                }
                            }

                        }

                    }
                }
                else if (Os_Principal_Amt1 < instAmt && Os_Principal_Amt1 > 0)
                {
                    //sub2 prin clear
                    prin_open_amt1 = Os_Principal_Amt1;
                    loan_adj_amt = instAmt - Os_Principal_Amt1;
                    Prin_Paid_amt1 = Os_Principal_Amt1;
                    Os_Principal_Amt1 = 0;
                    Prin_bal_amt1 = Os_Principal_Amt1;


                    if (loan_adj_amt > 0)
                    {
                        //sub1 interest covering
                        if (Os_Intrst_Amt > loan_adj_amt)
                        {
                            //subtract sub1 Os_Intrst_Amt
                            Interest_Open_amt = Os_Intrst_Amt;
                            Os_Intrst_Amt = Os_Intrst_Amt - loan_adj_amt;
                            Total_amount_recovered = Principal_amt_Recovered + loan_adj_amt;
                            Interest_Paid_amt = loan_adj_amt;
                            Interest_Bal_amt = Os_Intrst_Amt;

                        }
                        //sub1 interest clear
                        else
                        {
                            Interest_Open_amt = Os_Intrst_Amt;
                            loan_adj_amt = loan_adj_amt - Os_Intrst_Amt;
                            Interest_Paid_amt = Os_Intrst_Amt;
                            Os_Intrst_Amt = 0;
                            Interest_Bal_amt = Os_Intrst_Amt;
                        }
                        //sub2 interest covering
                        if (loan_adj_amt > Os_Intrst_Amt1)
                        {
                            Os_Intrst_Amt1 = loan_adj_amt - Os_Intrst_Amt1;
                            Total_amount_recovered = Principal_amt_Recovered1 + loan_adj_amt;
                            Interest_Paid_amt1 = Os_Intrst_Amt1;
                            Interest_Bal_amt1 = Os_Intrst_Amt1;
                            Interest_installment_amt = Convert.ToDecimal(Interest_Bal_amt1 + Os_Intrst_Amt) / Interest_installments;
                        }
                        else
                        {
                            Interest_Open_amt1 = Os_Intrst_Amt1;
                            Os_Intrst_Amt1 = Os_Intrst_Amt1 - loan_adj_amt;
                            Interest_Bal_amt1 = Os_Intrst_Amt1;
                            Interest_Paid_amt1 = loan_adj_amt;
                        }

                    }

                }
                //When Interest PAy
                else if (Os_Principal_Amt == 0 && Os_Intrst_Amt > 0 && Principal_recovered_flag == true && Os_Principal_Amt1 == 0)
                {
                    Interest_Open_amt = Os_Intrst_Amt;
                    Interest_Open_amt1 = Os_Intrst_Amt1;
                    //Interest_Bal_amt = Interest_Open_amt - instAmt;
                    //adjustment
                    if (Os_Intrst_Amt < instAmt)
                    {
                        //sub1 Os_Intrst_Amt = 0;

                        //Interest_recovered_flag = true;

                        Amount_Paid = Os_Intrst_Amt;
                        Interest_Paid_amt = Interest_Paid_amt + Os_Intrst_Amt;
                        Interest_amount_recovered = Interest_amount_recovered + Interest_Paid_amt;
                        loan_adj_amt = instAmt - Os_Intrst_Amt;
                        Os_Intrst_Amt = 0;
                        if (loan_adj_amt > 0)
                        {
                            //sub2 Os_Intrst_Amt1
                            if (Os_Intrst_Amt1 < loan_adj_amt)
                            {
                                Amount_Paid = Os_Intrst_Amt1;
                                Interest_amount_recovered1 = Interest_amount_recovered1 + Amount_Paid;
                                Interest_Open_amt1 = Os_Intrst_Amt1;
                                Interest_Paid_amt1 = Interest_Paid_amt1 + Os_Intrst_Amt1;
                                //Interest_recovered_flag1 = true;
                                Os_Intrst_Amt1 = 0;
                                Install_amt1 = loan_adj_amt;
                            }
                            else
                            {
                                Amount_Paid = loan_adj_amt;
                                Interest_amount_recovered1 = Interest_amount_recovered1 + Amount_Paid;
                                Interest_Open_amt1 = Os_Intrst_Amt1;

                                Os_Intrst_Amt1 = Os_Intrst_Amt1 - loan_adj_amt;
                                Install_amt1 = loan_adj_amt;

                                Interest_Paid_amt1 = Interest_Paid_amt1 + loan_adj_amt;
                                Interest_Bal_amt1 = Os_Intrst_Amt1;
                            }


                        }


                    }
                    else
                    {
                        Interest_Bal_amt = Interest_Open_amt - instAmt;
                        Interest_Paid_amt = instAmt;
                        
                        //Newly added on 25/05/2020
                        if(instAmt!=0 && Install_amt!=0 && (Install_amt>instAmt|| Install_amt== instAmt))
                        {
                            no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
                        }
                        else
                        {
                            no_of_installments_cover = 0;
                        }
                        //End

                        //no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString()); //commented on 25/05/2020
                        Completed_installments = Completed_installments + no_of_installments_cover;
                        Remaining_installments = Total_installment - Completed_installments;
                        Os_Intrst_Amt = Os_Intrst_Amt - Interest_Paid_amt;
                        Prin_Paid_amt = 0;
                        Install_amt = 0;
                        Interest_amount_recovered = Interest_amount_recovered + Interest_Paid_amt;

                        Interest_Bal_amt1 = Interest_Open_amt1;
                        Os_Intrst_Amt1 = Os_Intrst_Amt1 - Interest_Paid_amt1;
                        Prin_Paid_amt1 = 0;
                        Install_amt1 = 0;
                        Interest_amount_recovered1 = 0;

                    }

                }
                //When subloan2 Interest  PAy
                else if (Os_Principal_Amt1 == 0 && Os_Intrst_Amt1 > 0 && Interest_recovered_flag1 == false && Principal_recovered_flag1 == true && Os_Principal_Amt1 == 0)
                {
                    Interest_Open_amt1 = Os_Intrst_Amt1;
                    Interest_Bal_amt1 = Interest_Open_amt1 - instAmt;
                    if (Interest_Bal_amt1 < 0 || Interest_Bal_amt1 < 1)
                    {
                        Interest_Bal_amt1 = 0;
                    }
                    Interest_Paid_amt1 = instAmt;
                    no_of_installments_cover1 = Convert.ToInt32(Interest_Paid_amt1 / instAmt);
                    Completed_installments = Completed_installments + no_of_installments_cover1;
                    Remaining_installments = Total_installment - Completed_installments;
                    Interest_amount_recovered1 = Interest_amount_recovered1 + Interest_Paid_amt1;
                    Os_Intrst_Amt1 = Os_Intrst_Amt1 - Interest_Paid_amt1;
                    if (Os_Intrst_Amt1 < 0 || Os_Intrst_Amt1 < 1)
                    {
                        Os_Intrst_Amt1 = 0;
                    }
                    Prin_Paid_amt = 0;
                    Install_amt = 0;
                }
                //When subloan1 Principal PAy
                else if (Os_Principal_Amt > 0 && Principal_recovered_flag == false)
                {
                    prin_open_amt = Os_Principal_Amt;
                    Interest_Open_amt = Os_Intrst_Amt;
                    Interest_Bal_amt = Convert.ToInt32(intrestAmt + Os_Intrst_Amt);
                    Prin_bal_amt = Os_Principal_Amt - instAmt;
                    
                    //Newly added on 25/05/2020
                    if(instAmt!=0&& Install_amt!=0 &&(Install_amt> instAmt || Install_amt== instAmt))
                    {
                        no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
                    }
                    else
                    {
                        no_of_installments_cover = 0;
                    }
                    //End

                    //no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString()); commented on 25/05/2020
                    Completed_installments = Completed_installments + no_of_installments_cover;
                    Remaining_installments = Total_installment - Completed_installments;
                    Principal_amt_Recovered = Principal_amt_Recovered + instAmt;
                    Total_amount_recovered = Principal_amt_Recovered;
                    Interest_Bal_amt1 = Convert.ToInt32(intrestAmt1 + Os_Intrst_Amt1);
                    Total_amount_recovered1 = 0;
                    Prin_Paid_amt = instAmt;
                    Prin_Paid_amt1 = 0;
                    Prin_bal_amt1 = Os_Principal_Amt1;
                    Install_amt1 = 0;
                    Principal_amt_Recovered1 = 0;
                    prin_open_amt1 = Os_Principal_Amt1;
                    // Os_Principal_Amt = Prin_bal_amt;
                }
                else if (Os_Principal_Amt == 0 && Principal_recovered_flag == true)
                {
                    prin_open_amt1 = Os_Principal_Amt1;
                    Interest_Open_amt1 = Os_Intrst_Amt1;
                    Interest_Bal_amt1 = Convert.ToInt32(intrestAmt1 + Os_Intrst_Amt1);
                    Prin_bal_amt1 = Os_Principal_Amt1 - instAmt;
                    no_of_installments_cover1 = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
                    Completed_installments = Completed_installments + no_of_installments_cover1;
                    Remaining_installments = Total_installment - Completed_installments;
                    Principal_amt_Recovered1 = Principal_amt_Recovered1 + instAmt;
                    Total_amount_recovered = Principal_amt_Recovered1;
                    Prin_Paid_amt1 = instAmt;
                    Total_amount_recovered1 = Principal_amt_Recovered1;
                    Interest_Open_amt = Os_Intrst_Amt;

                    Interest_Bal_amt = Os_Intrst_Amt;
                }


                //if (dtPrevPartPay.Rows.Count > 0)
                //{
                //    prePartPayDate = Convert.ToDateTime(dtPrevPartPay.Rows[0]["cash_paid_on"].ToString());
                //}

                //if (Os_Principal_Amt != 0)
                //{
                //    intrestAmt = await calcIntrestAmt(Os_Principal_Amt, IntRATE, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate);
                //}
                //if (Os_Principal_Amt1 != 0)
                //{
                //    intrestAmt1 = await calcIntrestAmt(Os_Principal_Amt1, IntRATE, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate);
                //}

                if (intrestAmt > 0 && Principal_recovered_flag == false && Os_Principal_Amt != 0 && adjust == false)
                {
                    //Interest_Bal_amt = Convert.ToInt32(Interest_Bal_amt + intrestAmt);
                    intrst_accured = ", interest_accured=" + intrestAmt + ",os_interest_amount=" + Convert.ToInt32(Os_Intrst_Amt + intrestAmt);
                }
                else
                {
                    intrst_accured = ", interest_accured=" + intrestAmt + ",os_interest_amount=" + Os_Intrst_Amt;

                }

                if (intrestAmt1 > 0 && Principal_recovered_flag1 == false)
                {
                    //Interest_Bal_amt1 = Convert.ToInt32(Interest_Bal_amt1 + intrestAmt1);
                    intrst_accured1 = ", interest_accured=" + intrestAmt1 + ",os_interest_amount=" + Convert.ToInt32(Os_Intrst_Amt1 + intrestAmt1);
                }
                else
                {
                    intrst_accured1 = ", interest_accured=" + intrestAmt1 + ",os_interest_amount=" + Convert.ToInt32(Os_Intrst_Amt1 + intrestAmt1);

                }

                //Interest_Bal_amt = Os_Intrst_Amt + Interest_Bal_amt1;
                //First Installment
                if (Os_Principal_Amt == Loan_amt)
                {
                    //int Os_Principal_Amt_update = Prin_bal_amt+Os_Principal_Amt;
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + Os_Principal_Amt + "," + Prin_Paid_amt + "," + Prin_bal_amt + "," + intrestAmt + "," +
                  "" + Os_Intrst_Amt + "," + 0 + "," + Convert.ToInt32(Os_Intrst_Amt + intrestAmt) + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + instAmt + "," +
                  "" + no_of_installments_cover + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));

                    // int Os_Principal_Amt_update = Prin_bal_amt + Os_Principal_Amt;
                    //sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                    //sbqry.Append(GenNewTransactionString());
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid1 + "," + Os_Principal_Amt1 + "," + Prin_Paid_amt1 + "," + Prin_bal_amt1 + "," + intrestAmt1 + "," +
                  "" + Os_Intrst_Amt1 + "," + 0 + "," + Convert.ToInt32(Os_Intrst_Amt1 + intrestAmt1) + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover1 + ",'" + installments_paid_date + "'," + instAmt + "," +
                  "" + no_of_installments_cover1 + "," + Install_amt + ",1, @transidnew+1,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));
                }
                //when subloan1 Principal paid calculate sub2 interest
                else if (Os_Principal_Amt >= 0 && Principal_recovered_flag == false && Os_Principal_Amt1 > 0)
                {

                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                        "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                        "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                        "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                       + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + prin_open_amt + "," + Prin_Paid_amt + "," + Prin_bal_amt + "," + intrestAmt + "," +
                       "" + Interest_Open_amt + "," + Interest_Paid_amt + "," + Interest_Bal_amt + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                       "" + Completed_installments + ",'" + installments_paid_date + "'," + instAmt + "," +
                       "" + Completed_installments + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));

                    // int Os_Principal_Amt_update = Prin_bal_amt + Os_Principal_Amt;
                    //sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                    //sbqry.Append(GenNewTransactionString());
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew" + NewNumIndex + ", " + LnPkid + "," + ChLnPkid1 + "," + prin_open_amt1 + "," + Prin_Paid_amt1 + "," + Prin_bal_amt1 + "," + intrestAmt1 + "," +
                  "" + Os_Intrst_Amt1 + "," + 0 + "," + Interest_Bal_amt1 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover1 + ",'" + installments_paid_date + "'," + instAmt + "," +
                  "" + no_of_installments_cover1 + "," + Install_amt + ",1, @transidnew+1,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));
                }
                //only sub2 principal recover
                else if (Os_Principal_Amt == 0 && Principal_recovered_flag == true && Os_Principal_Amt1 > 0)
                {
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                        "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                        "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                        "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                       + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + prin_open_amt + "," + Prin_Paid_amt + "," + Prin_bal_amt + "," + intrestAmt + "," +
                       "" + Interest_Open_amt + "," + Interest_Paid_amt + "," + Interest_Bal_amt + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                       "" + Completed_installments + ",'" + installments_paid_date + "'," + instAmt + "," +
                       "" + Completed_installments + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));

                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid1 + "," + Os_Principal_Amt1 + "," + instAmt + "," + Prin_bal_amt1 + "," + intrestAmt1 + "," +
                  "" + Os_Intrst_Amt1 + "," + 0 + "," + Interest_Bal_amt1 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover1 + ",'" + installments_paid_date + "'," + instAmt + "," +
                  "" + no_of_installments_cover1 + "," + Install_amt + ",1, @transidnew+1,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));
                }
                // if both principals are done and have to adjust interest amount in both sub loans
                else if (Principal_recovered_flag == true && Principal_recovered_flag1 == true && Interest_recovered_flag == false && Interest_recovered_flag1 == false && loan_adj_amt != 0)
                {
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                        "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                        "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                        "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                       + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + Os_Principal_Amt + "," + Prin_Paid_amt + "," + Prin_bal_amt + "," + intrestAmt + "," +
                       "" + Interest_Open_amt + "," + Interest_Paid_amt + "," + Interest_Bal_amt + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                       "" + Completed_installments + ",'" + installments_paid_date + "'," + instAmt + "," +
                       "" + Completed_installments + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));

                    // int Os_Principal_Amt_update = Prin_bal_amt + Os_Principal_Amt;
                    //sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                    //sbqry.Append(GenNewTransactionString());
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid1 + "," + Os_Principal_Amt1 + "," + Prin_Paid_amt1 + "," + Prin_bal_amt1 + "," + intrestAmt1 + "," +
                  "" + Interest_Open_amt1 + "," + Interest_Paid_amt1 + "," + Interest_Bal_amt1 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover1 + ",'" + installments_paid_date + "'," + Amount_Paid + "," +
                  "" + no_of_installments_cover1 + "," + Amount_Paid + ",1, @transidnew+1,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));
                }
                //Prin1,int1 done and int2 adjust
                else if (Principal_recovered_flag == true && Principal_recovered_flag1 == false && Interest_recovered_flag == false && Interest_recovered_flag1 == false)
                {
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                        "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                        "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                        "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                       + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + prin_open_amt + "," + Prin_Paid_amt + "," + Prin_bal_amt + "," + intrestAmt + "," +
                       "" + Interest_Open_amt + "," + Interest_Paid_amt + "," + Interest_Bal_amt + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                       "" + Completed_installments + ",'" + installments_paid_date + "'," + instAmt + "," +
                       "" + Completed_installments + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));

                    // int Os_Principal_Amt_update = Prin_bal_amt + Os_Principal_Amt;
                    //sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                    //sbqry.Append(GenNewTransactionString());
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid1 + "," + prin_open_amt1 + "," + Prin_Paid_amt1 + "," + Prin_bal_amt1 + "," + intrestAmt1 + "," +
                  "" + Interest_Open_amt1 + "," + Interest_Paid_amt1 + "," + Interest_Bal_amt1 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover1 + ",'" + installments_paid_date + "'," + Amount_Paid + "," +
                  "" + no_of_installments_cover1 + "," + Amount_Paid + ",1, @transidnew+1,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));
                }
                // sub 1 interest recover
                else if (Os_Principal_Amt == 0 && Principal_recovered_flag == true && Os_Principal_Amt1 == 0 && Principal_recovered_flag1 == true && Os_Intrst_Amt > 0)
                {
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                        "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                        "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                        "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                       + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid + "," + Os_Principal_Amt + "," + Prin_Paid_amt + "," + Prin_bal_amt + "," + intrestAmt + "," +
                       "" + Interest_Open_amt + "," + Interest_Paid_amt + "," + Interest_Bal_amt + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                       "" + Completed_installments + ",'" + installments_paid_date + "'," + instAmt + "," +
                       "" + Completed_installments + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));

                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid1 + "," + prin_open_amt1 + "," + Prin_Paid_amt1 + "," + Prin_bal_amt1 + "," + intrestAmt1 + "," +
                  "" + Interest_Open_amt1 + "," + Interest_Paid_amt1 + "," + Interest_Bal_amt1 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover1 + ",'" + installments_paid_date + "'," + Amount_Paid + "," +
                  "" + no_of_installments_cover1 + "," + Amount_Paid + ",1, @transidnew+1,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));
                }
                // sub2 interest recover
                else if (Os_Principal_Amt == 0 && Principal_recovered_flag == true && Os_Principal_Amt1 == 0 && Principal_recovered_flag1 == true && Interest_recovered_flag == true)
                {
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments_bef_monthend", NewNumIndex));
                    sbqry.Append(GenNewTransactionString());
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                        "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                        "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                        "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                       + "(@idnew" + NewNumIndex + "," + LnPkid + "," + ChLnPkid1 + "," + Os_Principal_Amt1 + "," + Prin_Paid_amt1 + "," + Prin_bal_amt1 + "," + intrestAmt1 + "," +
                       "" + Interest_Open_amt1 + "," + Interest_Paid_amt1 + "," + Interest_Bal_amt1 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                       "" + Completed_installments + ",'" + installments_paid_date + "'," + instAmt + "," +
                       "" + Completed_installments + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments_bef_monthend", "@idnew" + NewNumIndex + "", ""));


                }

                Os_Principal_Amt = Prin_bal_amt;
                Os_Total_Amt = Convert.ToInt32(Prin_bal_amt + Os_Intrst_Amt + intrestAmt);
                Os_Principal_Amt1 = Prin_bal_amt1;
                Os_Total_Amt1 = Convert.ToInt32(Prin_bal_amt1 + Os_Intrst_Amt1 + intrestAmt1);
                //3. child loan update
                //sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET principal_amount_recovered=" + Principal_amt_Recovered
                //    + intrst_accured
                //    + ",total_amount_recovered=" + Total_amount_recovered + " , os_principal_amount=" + Os_Principal_Amt
                //    + ", os_total_amount= " + Os_Total_Amt + ",interest_amount_recovered=" + Interest_amount_recovered + " where id=" + ChLnPkid + " AND active=1;");

                //sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET principal_amount_recovered=" + Principal_amt_Recovered1
                //  + intrst_accured1
                //  + ",total_amount_recovered=" + Total_amount_recovered1 + " , os_principal_amount=" + Os_Principal_Amt1
                //  + ", os_total_amount= " + Os_Total_Amt1 + ",interest_amount_recovered=" + Interest_amount_recovered1 + " where id=" + ChLnPkid1 + " AND active=1;");
                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child_bef_monthend", ChLnPkid1.ToString(), instAmt.ToString()));
                //4. parent loan update
                string updInstallAmt = "";
                //update instalamt when Os_Principal_Amt Lessthan installmentAmt 
                //if (Os_Principal_Amt1 > 0 && Os_Principal_Amt1 < Install_amt)
                //{
                //    updInstallAmt = ", installment_amount=" + Os_Principal_Amt1;
                //    sbqry.Append("UPDATE pr_emp_adv_loans_bef_monthend SET completed_installment=" + Completed_installments
                //   + updInstallAmt
                //   + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                //   + Total_amount_recovered + ",active=1 " + " where id=" + LnPkid + " ;");
                //}
                //// principal_recovered_flag = 1 
                //if (Os_Principal_Amt == 0 && Principal_recovered_flag == false)
                //{
                //    sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET principal_recovered_flag = 1 where id=" + ChLnPkid + " ;");
                //    //Interest_installment_amt = Interest_Bal_amt / Interest_installments;
                //}
                //if (Os_Principal_Amt1 == 0 && Principal_recovered_flag1 == false && Principal_recovered_flag == true && Os_Principal_Amt == 0)
                //{
                //    sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET principal_recovered_flag = 1 where id=" + ChLnPkid1 + " ;");
                //    Interest_installment_amt = decimal.Round(Convert.ToDecimal((Os_Intrst_Amt + Interest_Bal_amt1) / Interest_installments), 2);
                //}
                ////to make sub1 intewrest recovered flag =1
                //if (Os_Principal_Amt == 0 && Os_Principal_Amt1 == 0 && Os_Intrst_Amt == 0 && Os_Intrst_Amt1 != 0 && Principal_recovered_flag == true && Interest_recovered_flag == false)
                //{
                //    sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET Interest_recovered_flag = 1,active=0 where id=" + ChLnPkid + " ;");

                //    sbqry.Append("UPDATE pr_emp_adv_loans_bef_monthend SET completed_installment=" + Completed_installments
                //     + updInstallAmt
                //     + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                //     + Total_amount_recovered + "" + " where id=" + LnPkid + " ;");

                //    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
                //}
                ////loan close
                //else if (Os_Principal_Amt == 0 && Os_Principal_Amt1 == 0 && Os_Intrst_Amt == 0 && Os_Intrst_Amt1 == 0)
                //{
                //    sbqry.Append("UPDATE pr_emp_adv_loans_child_bef_monthend SET Interest_recovered_flag = 1,active=0 where id=" + ChLnPkid1 + " ;");

                //    sbqry.Append("UPDATE pr_emp_adv_loans_bef_monthend SET completed_installment=" + Completed_installments
                //     + updInstallAmt
                //     + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                //     + Total_amount_recovered + ",active=0 " + " where id=" + LnPkid + " ;");

                //    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
                //}
                ////when installment processing
                //else
                //{
                //    os_int_totamt_two_sub_lns = Os_Intrst_Amt + Os_Intrst_Amt1;
                //    if (os_int_totamt_two_sub_lns < instAmt)
                //    {
                //        Interest_installment_amt = Convert.ToDecimal(os_int_totamt_two_sub_lns);
                //    }

                //    sbqry.Append("UPDATE pr_emp_adv_loans_bef_monthend SET completed_installment=" + Completed_installments
                //        + updInstallAmt
                //        + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                //        + Total_amount_recovered + ",interest_installment_amount=" + Interest_installment_amt + ",active=1 " + " where id=" + LnPkid + " ;");

                //    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
                //}
                //execute sql statements in one shot
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            return bRet;
        }
        private async Task<double> calcIntrestAmt(double osPrinc, double iRate, DateTime loanSactDate, DateTime dtFm, DateTime dtPartPay, DateTime dtPrevPartPay)
        {
            int yrDays = 365;
            double retIntAmt = 0;
            try
            {
                //calc days
                int days = 0;
                if (loanSactDate.Month == dtFm.Month && loanSactDate.Year == dtFm.Year) //in same month
                    days = Helper.findLastDayOfMonth(dtFm) - loanSactDate.Day;
                else
                    days = Helper.findLastDayOfMonth(dtFm);


                if (dtPartPay.Year == 1900) //Mn end installment
                {
                    if (dtPrevPartPay.Year != 1900) //if any prev. partpay in the same month remove those days
                    {
                        days -= dtPrevPartPay.Day;
                    }
                    retIntAmt = Math.Round(((osPrinc * (iRate / 100)) / yrDays) * days);
                }
                //else //any part payments
                //{
                //    days = dtPartPay.Day;
                //    retIntAmt = Math.Round(((osPrinc * (iRate / 100)) / yrDays) * days);
                //}
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            return retIntAmt;
        }
    }
}
