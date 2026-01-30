using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using System.Web.Mvc;
using System.Data;

namespace PayRollBusiness.Process
{
    public class LoansPaymentProcess : BusinessBase
    {
        StringBuilder sbqry = new StringBuilder();
        log4net.ILog _logger = null;

        public LoansPaymentProcess(LoginCredential loginCredential, log4net.ILog logger) : base(loginCredential)
        {
            _logger = logger;
        }

        //Pf Loan 1
        private async Task<bool> PfLoan1Payments(DataTable dtLoan, DataTable dtFm, int instAmt, char PaymentType, DataTable dtPrevPartPay)
        {
            int intrestAmt = 0;
            int intrestAmt1 = 0;
            bool bRet = false;
            //Declarations
            //string qry = "";
            int Prin_bal_amt = 0;
            int principal_installment = 0;
            int no_of_installments_cover = 0;
            //int Os_Total_Amt = 0;
            int Remaining_installments = 0;
            //1. get data from dtLoan
            //string payment_type = PaymentType.ToString();
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
            }

            if (Os_Intrst_Amt > 0)
            {
                intrestAmt = intrestAmt + Os_Intrst_Amt;
            }

            principal_installment = instAmt - intrestAmt;
            Prin_bal_amt = Os_Principal_Amt - principal_installment;
            Os_Intrst_Amt = 0;

            no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
            Completed_installments = Completed_installments + no_of_installments_cover;
            Remaining_installments = Total_installment - Completed_installments;
            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
            sbqry.Append(GenNewTransactionString());
            //loan Adjust
            sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_balance_amount]," +
                "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                "[installments_amount],[active],[trans_id],[fm],[fy],[interest_accured],[interest_paid_amount],loan_sl_no) values "
               + "(@idnew+1," + LnPkid + "," + ChLnPkid + "," + Os_Principal_Amt + "," + principal_installment + "," + Prin_bal_amt + "," +
               "" + intrestAmt + "," + 0 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
               "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + instAmt + "," +
               "" + no_of_installments_cover + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + intrestAmt + "," + intrestAmt + "," + Loan_Sl_No + ");");

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

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

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", ChLnPkid.ToString(), Install_amt.ToString()));
            sbqry.Append("UPDATE pr_emp_adv_loans_child SET principal_amount_recovered=principal_amount_recovered+" + principal_installment
                + ",total_amount_recovered=total_amount_recovered+" + Install_amt
                + ",interest_amount_recovered=interest_amount_recovered+" + intrestAmt
                + ",interest_accured=interest_accured+" + intrestAmt
                + " , os_principal_amount=" + Prin_bal_amt
                + ", os_total_amount= " + Prin_bal_amt
                + updChild
                + ", os_interest_amount = " + Os_Intrst_Amt
                + " where id=" + ChLnPkid + " AND active=1;");

            sbqry.Append("UPDATE pr_emp_adv_loans SET completed_installment=" + Completed_installments
                + updInstallAmtParent
                + ",remaining_installment=" + Remaining_installments
                + " ,total_recovered_amount=total_recovered_amount+" + Install_amt
                + " where id=" + LnPkid + " ;");

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
            //execute sql statements in one shot
            bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

            return bRet;
        }

        //fest loan
        private async Task<bool> FestLoanPayments(DataTable dtLoan, DataTable dtFm, int instAmt, char PaymentType)
        {
            bool bRet = false;
            //Declarations
            //string qry = "";
            int Prin_bal_amt = 0;
            int no_of_installments_cover = 0;
            int Os_Total_Amt = 0;
            int Remaining_installments = 0;
            //1. get data from dtLoan
            //string payment_type = PaymentType.ToString();
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
            Prin_bal_amt = Os_Principal_Amt - instAmt;
            no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
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

            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
            sbqry.Append(GenNewTransactionString());
            //loan Adjust
            sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
               + "(@idnew+1," + LnPkid + "," + ChLnPkid + "," + Os_Principal_Amt + "," + instAmt + "," + Prin_bal_amt + "," +
               "" + 0 + "," + 0 + "," + 0 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
               "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + instAmt + "," +
               "" + no_of_installments_cover + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
            
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

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", ChLnPkid.ToString(), Install_amt.ToString()));
            sbqry.Append("UPDATE pr_emp_adv_loans_child SET principal_amount_recovered=principal_amount_recovered+" + Install_amt
                + ",total_amount_recovered=total_amount_recovered+" + Install_amt
                + " , os_principal_amount=" + Prin_bal_amt 
                + ", os_total_amount= " + Os_Total_Amt 
                + updChild
                + " where id=" + ChLnPkid + " AND active=1;");

            sbqry.Append("UPDATE pr_emp_adv_loans SET completed_installment=" + Completed_installments
                + updInstallAmtParent
                + ",remaining_installment=" + Remaining_installments 
                + " ,total_recovered_amount=total_recovered_amount+" + Install_amt
                + " where id=" + LnPkid + " ;");

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
            //execute sql statements in one shot
            bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

            return bRet;
        }

        public async Task<bool> InstallmentPartPayments(int loan_id, int instAmt, char PaymentType)
        {
            bool bRet = false;

            //Get Loan Data
            string GetLoans = "SELECT mas.loan_id,mas.id as mastertypeid,chi.priority,chi.Id as childloanid," +
              "chi.emp_adv_loans_mid,adv.method ,chi.principal_amount_recovered,chi.total_amount_recovered," +
              "chi.loan_amount,os_principal_amount,chi.os_interest_amount,chi.principal_recovered_flag,chi.interest_recovered_flag," +
              "chi.loan_sl_no,chi.interest_rate,chi.total_interest_installments,adv.installment_amount,adv.total_installment, " +
              "adv.interest_installment_amount,adv.completed_installment,adv.total_recovered_amount,adv.sanction_date,chi.date_disburse " +
              "FROM pr_emp_adv_loans adv" +
              " JOIN pr_loan_master mas ON adv.loan_type_mid = mas.id " +
              " JOIN pr_emp_adv_loans_child chi ON adv.id = chi.emp_adv_loans_mid " +
              " WHERE adv.active = 1 AND chi.emp_adv_loans_mid =" + loan_id + " " +
              " ORDER BY chi.interest_rate desc;";

            string qryGetfm = "Select fm,fy from pr_month_details where active=1;";

            string qryPartPayments = "SELECT emp_adv_loans_child_mid,cash_paid_on " +
                "from pr_emp_adv_loans_adjustments adj " +
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
                    bRet = await FestLoanPayments(dtLoans, dtFm, instAmt, PaymentType);
                }
                else if (loanType == PrConstants.PF_LOAN1_CODE || loanType == PrConstants.PF_LOAN2_CODE || loanType == PrConstants.PF_LOANST1_CODE || loanType == PrConstants.PF_LOANST2_CODE || loanType == PrConstants.PF_LOANLT1_CODE || loanType == PrConstants.PF_LOANLT2_CODE || loanType == PrConstants.PF_LOANLT3_CODE || loanType == PrConstants.PF_LOANLT4_CODE) //Indraja
                {
                    bRet = await PfLoan1Payments(dtLoans, dtFm, instAmt, PaymentType, dtPartPay);
                }                
                else if (loanType == PrConstants.VEH_LOANLT6_CODE) //Uma
                {
                    bRet = await TwoWheelerLoanPayments(dtLoans, dtFm, instAmt, PaymentType, dtPartPay);
                }
                else if (loanType == PrConstants.HOUSING_ADDL_LOAN_CODE) //Raji
                {
                    bRet = await House2DLoanPayments(dtLoans, dtFm, instAmt, PaymentType);
                }
                else if (loanType == PrConstants.HOUSE_LOAN_MAIN) //Raji
                {
                    bRet = await House2DLoanPayments(dtLoans, dtFm, instAmt, PaymentType);
                }
                else if (loanType == PrConstants.VEH_LOANLT5_CODE) //Raji
                {
                    bRet = await FourWheelerLoanPayments(dtLoans, dtFm, instAmt, PaymentType, dtPartPay);
                }
            }

            return bRet;

        }

        //4w loan
        private async Task<bool> FourWheelerLoanPayments(DataTable dtLoan, DataTable dtFm, int instAmt, char PaymentType, DataTable dtPrevPartPay)
        {
            bool bRet = false;
            int Prin_bal_amt = 0;
            int no_of_installments_cover = 0;
            int Os_Total_Amt = 0;
            int Remaining_installments = 0;
            //1. get data from dtLoan
            //string payment_type = PaymentType.ToString();
            string payment_mode = PaymentType == 'I' ? PrConstants.LOAN_INSTALLMENT : PrConstants.LOAN_PARTPAYMENT;
            string Loan_Code = dtLoan.Rows[0]["loan_id"].ToString();
            int Loan_id = Convert.ToInt32(dtLoan.Rows[0]["mastertypeid"]);
            int Priority = Convert.ToInt32(dtLoan.Rows[0]["priority"]);
            int LnPkid = Convert.ToInt32(dtLoan.Rows[0]["emp_adv_loans_mid"]);
            int ChLnPkid = Convert.ToInt32(dtLoan.Rows[0]["childloanid"]);
            string Method = dtLoan.Rows[0]["method"].ToString();
            int Principal_amt_Recovered = Convert.ToInt32(dtLoan.Rows[0]["principal_amount_recovered"]);
            int Total_amt_Recovered = Convert.ToInt32(dtLoan.Rows[0]["total_amount_recovered"]);
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
            int IntRATE = Convert.ToInt32(dtLoan.Rows[0]["interest_rate"]);
            DateTime sandate = Convert.ToDateTime(dtLoan.Rows[0]["sanction_date"]);
            Prin_bal_amt = Os_Principal_Amt - instAmt;
            no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
            Completed_installments = Completed_installments + no_of_installments_cover;
            Remaining_installments = Total_installment - Completed_installments;
            Principal_amt_Recovered = Principal_amt_Recovered + instAmt;
            Total_amount_recovered = Principal_amt_Recovered;

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
            if (dtPrevPartPay.Rows.Count > 0)
            {
                prePartPayDate = Convert.ToDateTime(dtPrevPartPay.Rows[0]["cash_paid_on"].ToString());
            }

            double intrestAmt = await calcIntrestAmt(Os_Principal_Amt, IntRATE, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate);

            string intrst_accured = "";
            if (intrestAmt > 0)
            {
                intrst_accured = ", interest_accured=" + intrestAmt + ",os_interest_amount=" + Convert.ToInt32(Os_Intrst_Amt + intrestAmt);
            }
            Os_Principal_Amt = Prin_bal_amt;
            Os_Total_Amt = Prin_bal_amt;

            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
            sbqry.Append(GenNewTransactionString());
            //loan Adjust
            sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
               + "(@idnew+1," + LnPkid + "," + ChLnPkid + "," + Os_Principal_Amt + "," + instAmt + "," + Prin_bal_amt + "," + intrestAmt + "," +
               "" + Os_Intrst_Amt + "," + 0 + "," + Convert.ToInt32(Os_Intrst_Amt + intrestAmt) + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
               "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + instAmt + "," +
               "" + no_of_installments_cover + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
            //3. child loan update
            sbqry.Append("UPDATE pr_emp_adv_loans_child SET principal_amount_recovered=" + Principal_amt_Recovered
                + intrst_accured
                + ",total_amount_recovered=" + Total_amount_recovered + " , os_principal_amount=" + Os_Principal_Amt
                + ", os_total_amount= " + Os_Total_Amt + " where id=" + ChLnPkid + " AND active=1;");

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", ChLnPkid.ToString(), Install_amt.ToString()));
            //4. parent loan update
            string updInstallAmt = "";
            if (Os_Principal_Amt > 0 && Os_Principal_Amt < Install_amt)
            {
                updInstallAmt = ", installment_amount=" + Os_Principal_Amt;
                sbqry.Append("UPDATE pr_emp_adv_loans SET completed_installment=" + Completed_installments
               + updInstallAmt
               + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
               + Total_amount_recovered + ",active=1 " + " where id=" + LnPkid + " ;");
            }
            if (Os_Principal_Amt == 0)
            {
                updInstallAmt += "principal_recovered_flag=1";
                sbqry.Append("UPDATE pr_emp_adv_loans_child SET principal_recovered_flag = 1 where emp_adv_loans_mid=" + LnPkid + " ;");
                //intrest should be calculated

            }

            sbqry.Append("UPDATE pr_emp_adv_loans SET completed_installment=" + Completed_installments
                + updInstallAmt
                + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                + Total_amount_recovered + ",active=1 " + " where id=" + LnPkid + " ;");

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
            //execute sql statements in one shot
            bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

            return bRet;
        }


        //Housing Loand 2D
        private async Task<bool> House2DLoanPayments(DataTable dtLoan, DataTable dtFm, int instAmt, char PaymentType)
        {
            bool bRet = false;
            //Declarations
            //string qry = "";
            int Prin_bal_amt1 = 0;
            int no_of_installments_cover1 = 0;
            int Os_Total_Amt1 = 0;
            int Remaining_installments1 = 0;
            int Os_Intrst_Amt1 = 0;
            //1. get data from dtLoan

            string payment_mode = PaymentType == 'I' ? PrConstants.LOAN_INSTALLMENT : PrConstants.LOAN_PARTPAYMENT;
            string Loan_Code = dtLoan.Rows[0]["loan_id"].ToString();
            int Loan_id = Convert.ToInt32(dtLoan.Rows[0]["mastertypeid"]);
            int Priority1 = Convert.ToInt32(dtLoan.Rows[0]["priority"]);
            int LnPkid = Convert.ToInt32(dtLoan.Rows[0]["emp_adv_loans_mid"]);
            int ChLnPkid1 = Convert.ToInt32(dtLoan.Rows[0]["childloanid"]);
            string Method = dtLoan.Rows[0]["method"].ToString();
            int Principal_amt_Recovered1 = Convert.ToInt32(dtLoan.Rows[0]["principal_amount_recovered"]);
            int Total_amt_Recovered1 = Convert.ToInt32(dtLoan.Rows[0]["total_amount_recovered"]);
            int Loan_amt1 = Convert.ToInt32(dtLoan.Rows[0]["loan_amount"]);
            int Os_Principal_Amt1 = Convert.ToInt32(dtLoan.Rows[0]["os_principal_amount"]);
            Os_Intrst_Amt1 = Convert.ToInt32(dtLoan.Rows[0]["os_interest_amount"]);
            bool Principal_recovered_flag1 = bool.Parse(dtLoan.Rows[0]["principal_recovered_flag"].ToString());
            bool Interest_recovered_flag1 = bool.Parse(dtLoan.Rows[0]["interest_recovered_flag"].ToString());
            int Loan_Sl_No = Convert.ToInt32(dtLoan.Rows[0]["loan_sl_no"]);
            int Install_amt1 = Convert.ToInt32(dtLoan.Rows[0]["installment_amount"]);
            int Total_installment = Convert.ToInt32(dtLoan.Rows[0]["total_installment"]);
            int Completed_installments1 = Convert.ToInt32(dtLoan.Rows[0]["completed_installment"]);
            int Total_amount_recovered1 = Convert.ToInt32(dtLoan.Rows[0]["total_recovered_amount"]);
            decimal IntRate = Convert.ToDecimal(dtLoan.Rows[0]["interest_rate"].ToString());
            DateTime sandate = Convert.ToDateTime(dtLoan.Rows[0]["sanction_date"]);
            int loan_adj_amt = 0;
            int loan_adj_amt_for2 = 0;
            int out_pric_amt1 = Os_Principal_Amt1;
            if (Os_Principal_Amt1 < instAmt)
            {
                loan_adj_amt_for2 = Os_Principal_Amt1 - instAmt;
            }
            //if (Os_Principal_Amt1> instAmt)
            //{
            //    loan_adj_amt = Os_Principal_Amt1 - instAmt;
            //}
            //else
            //{
            //     loan_adj_amt = instAmt - Os_Principal_Amt1;
            //}
            loan_adj_amt =Math.Abs(Os_Principal_Amt1 - instAmt);
            if (Os_Principal_Amt1 < instAmt && Principal_recovered_flag1 == false)
            {
                instAmt = Os_Principal_Amt1;
                Prin_bal_amt1 = 0;
                no_of_installments_cover1 = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt1)).ToString());
                Completed_installments1 = Completed_installments1 + no_of_installments_cover1;
                Remaining_installments1 = Total_installment - Completed_installments1;
                Principal_amt_Recovered1 = Principal_amt_Recovered1 + instAmt;
                Total_amount_recovered1 = Principal_amt_Recovered1;
                Os_Principal_Amt1 = Prin_bal_amt1;
                Os_Total_Amt1 = Prin_bal_amt1;

            }
            else if (Os_Principal_Amt1 > instAmt && Principal_recovered_flag1 == false)
            {
                Prin_bal_amt1 = Os_Principal_Amt1 - instAmt;
                no_of_installments_cover1 = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt1)).ToString());
                Completed_installments1 = Completed_installments1 + no_of_installments_cover1;
                Remaining_installments1 = Total_installment - Completed_installments1;
                Principal_amt_Recovered1 = Principal_amt_Recovered1 + instAmt;
                Total_amount_recovered1 = Principal_amt_Recovered1;
                Os_Principal_Amt1 = Prin_bal_amt1;
                Os_Total_Amt1 = Prin_bal_amt1;
            }

            //subloan2
            int ChLnPkid2 = 0;
            int Os_Principal_Amt2 = 0;
            int Prin_bal_amt2 = 0;
            int no_of_installments_cover2 = 0;
            int Os_Total_Amt2 = 0;
            int Remaining_installments = 0;
            int Completed_installments2 = 0;
            int Total_amount_recovered2 = 0;
            int Install_amt2 = 0;
            int Principal_amt_Recovered2 = 0;
            int instAmt2 = 0;
            int Priority2 = 0;
            int Os_Intrst_Amt2 = 0;
            int out_pric_amt2 = 0;
            if (dtLoan.Rows.Count >= 2 && loan_adj_amt > 0)
            {
                loan_adj_amt = loan_adj_amt - Os_Principal_Amt2;
                instAmt2 = instAmt;
                Priority2 = Convert.ToInt32(dtLoan.Rows[1]["priority"]);
                ChLnPkid2 = Convert.ToInt32(dtLoan.Rows[1]["childloanid"]);
                Principal_amt_Recovered2 = Convert.ToInt32(dtLoan.Rows[1]["principal_amount_recovered"]);
                int Total_amt_Recovered2 = Convert.ToInt32(dtLoan.Rows[1]["total_amount_recovered"]);
                int Loan_amt2 = Convert.ToInt32(dtLoan.Rows[1]["loan_amount"]);
                Os_Principal_Amt2 = Convert.ToInt32(dtLoan.Rows[1]["os_principal_amount"]);
                Os_Intrst_Amt2 = Convert.ToInt32(dtLoan.Rows[1]["os_interest_amount"]);
                bool Principal_recovered_flag2 = bool.Parse(dtLoan.Rows[1]["principal_recovered_flag"].ToString());
                bool Interest_recovered_flag2 = bool.Parse(dtLoan.Rows[1]["interest_recovered_flag"].ToString());
                Install_amt2 = Convert.ToInt32(dtLoan.Rows[1]["installment_amount"]);
                Completed_installments2 = Convert.ToInt32(dtLoan.Rows[1]["completed_installment"]);
                Total_amount_recovered2 = Convert.ToInt32(dtLoan.Rows[1]["total_recovered_amount"]);
                out_pric_amt2 = Os_Principal_Amt2;
                Prin_bal_amt2 = Os_Principal_Amt2 - instAmt;// should not do 
                no_of_installments_cover2 = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt2)).ToString());
                Completed_installments2 = Completed_installments2 + no_of_installments_cover2;
                Remaining_installments = Total_installment - Completed_installments2;
                Principal_amt_Recovered2 = Principal_amt_Recovered2 + instAmt2;
                Total_amount_recovered2 = Principal_amt_Recovered2;
                Os_Principal_Amt2 = Prin_bal_amt2;
                Os_Total_Amt2 = Prin_bal_amt2;

            }
            //Payments Dates
            DateTime paidDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
            string installments_paid_date = (paidDate.ToString("yyyy-MM-dd"));
            string Cash_paid_on = installments_paid_date;
            int fy = Convert.ToInt32(dtFm.Rows[0]["fy"].ToString());
            string fm = installments_paid_date;
            int Month = paidDate.Month;
            int Year = paidDate.Year;
            string PrinRecFlag = "";
            string active = "";

            string updInstallAmt = "";
            //if (Os_Principal_Amt1 > 0 && Os_Principal_Amt1 < Install_amt1 && Os_Principal_Amt2 < Install_amt1)

            //else
            //{
            //    updInstallAmt = ", installment_amount=" + Os_Principal_Amt2;
            //}


            if (Os_Principal_Amt2 > 0 && Os_Principal_Amt2 < Install_amt1)
            {
                updInstallAmt = ", installment_amount=" + Os_Principal_Amt2;
            }
            if (Os_Principal_Amt1 == 0 && Principal_recovered_flag1 == false || Os_Principal_Amt2 == 0)
            {
                PrinRecFlag += ", principal_recovered_flag=1 ";
            }

            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
            sbqry.Append(GenNewTransactionString());
            //Subloan1 Installment
            if (Loan_amt1 > Total_amt_Recovered1 && Priority1 == 1 && Principal_recovered_flag1 == false)
            {
                //interest calc
                string qry = "SELECT installments_paid_date from  pr_emp_adv_loans_adjustments " +
                    "where payment_type='Part Pay' and emp_adv_loans_mid=" + Loan_id + " and emp_adv_loans_child_mid=" + ChLnPkid1 + ";";
                DataTable partpaydate = await _sha.Get_Table_FromQry(qry);
                if(partpaydate.Rows.Count>0)
                {
                    DateTime PPayDate = Convert.ToDateTime(partpaydate.Rows[0]["installments_paid_date"]);
                }
                //if(Install_amt1> )
                //var x = await calcIntrestAmt(Os_Principal_Amt1, IntRate, sandate, paidDate, instAmt, PPayDate);
                //sub1
                sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                    "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                    "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                    "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                   + "(@idnew," + LnPkid + "," + ChLnPkid1 + "," + out_pric_amt1 + "," + instAmt + "," + Prin_bal_amt1 + "," +
                   "" + 0 + "," + 0 + "," + 0 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                   "" + no_of_installments_cover1 + ",'" + installments_paid_date + "'," + instAmt + "," +
                   "" + no_of_installments_cover1 + "," + instAmt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

                //sub2
             //   sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
             // "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
             // "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
             // "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
             //+ "(@idnew+1," + LnPkid + "," + ChLnPkid2 + "," + Os_Principal_Amt2 + "," + 0 + "," + Prin_bal_amt2 + "," +
             //"" + 0 + "," + 0 + "," + 0 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
             //"" + no_of_installments_cover2 + ",'" + installments_paid_date + "'," + instAmt2 + "," +
             //"" + no_of_installments_cover2 + "," + Install_amt2 + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
             //   sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew+1", ""));

                //3. child loan update subloan1
                sbqry.Append("UPDATE pr_emp_adv_loans_child SET principal_amount_recovered=" + Principal_amt_Recovered1
                     + PrinRecFlag +
                    ",total_amount_recovered=" + Total_amount_recovered1 + " , os_principal_amount=" + Os_Principal_Amt1 + ", " +
                    "os_total_amount= " + Os_Total_Amt1 + " where id=" + ChLnPkid1 + " AND active=1;");
            }
            else if (loan_adj_amt > instAmt)
            {
                //Subloan2 Adjust 
                sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew," + LnPkid + "," + ChLnPkid2 + "," + out_pric_amt2 + "," + instAmt2 + "," + Prin_bal_amt2 + "," +
                  "" + 0 + "," + 0 + "," + 0 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover2 + ",'" + installments_paid_date + "'," + instAmt2 + "," +
                  "" + no_of_installments_cover2 + "," + Install_amt2 + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

                //child loan update subloan2
                sbqry.Append("UPDATE pr_emp_adv_loans_child SET principal_amount_recovered=" + Principal_amt_Recovered2
                   + PrinRecFlag +
               ",total_amount_recovered=" + Total_amount_recovered2 + " , os_principal_amount=" + Os_Principal_Amt2 + ", " +
               "os_total_amount= " + Os_Total_Amt2 + " where id=" + ChLnPkid2 + " AND active=1;");
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", ChLnPkid1.ToString(), Install_amt1.ToString()));
            }
            else
            {
                //Subloan2 Installment
                sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew," + LnPkid + "," + ChLnPkid2 + "," + Os_Principal_Amt2 + "," + instAmt2 + "," + Prin_bal_amt2 + "," +
                  "" + 0 + "," + 0 + "," + 0 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover2 + ",'" + installments_paid_date + "'," + instAmt2 + "," +
                  "" + no_of_installments_cover2 + "," + Install_amt2 + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

                //child loan update subloan2
                sbqry.Append("UPDATE pr_emp_adv_loans_child SET principal_amount_recovered=" + Principal_amt_Recovered2
                   + PrinRecFlag +
               ",total_amount_recovered=" + Total_amount_recovered2 + " , os_principal_amount=" + Os_Principal_Amt2 + ", " +
               "os_total_amount= " + Os_Total_Amt2 + " where id=" + ChLnPkid2 + " AND active=1;");
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", ChLnPkid1.ToString(), Install_amt1.ToString()));
            }

            //4. parent loan update
            sbqry.Append("UPDATE pr_emp_adv_loans SET completed_installment=" + Completed_installments1
                + updInstallAmt
                + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                + Total_amount_recovered1 + ",active=1 " + " where id=" + LnPkid + " ;");
            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));

             if (loan_adj_amt > instAmt && Os_Principal_Amt1==0)
            {
                if(out_pric_amt2 <= loan_adj_amt)
                {
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew," + LnPkid + "," + ChLnPkid2 + "," + out_pric_amt2 + "," + loan_adj_amt + "," + 0 + "," +
                  "" + 0 + "," + 0 + "," + 0 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover2 + ",'" + installments_paid_date + "'," + loan_adj_amt + "," +
                  "" + no_of_installments_cover2 + "," + loan_adj_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");
                } else
                {
                    sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                   "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                  + "(@idnew," + LnPkid + "," + ChLnPkid2 + "," + out_pric_amt2 + "," + loan_adj_amt + "," + Prin_bal_amt2 + "," +
                  "" + 0 + "," + 0 + "," + 0 + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                  "" + no_of_installments_cover2 + ",'" + installments_paid_date + "'," + loan_adj_amt + "," +
                  "" + no_of_installments_cover2 + "," + loan_adj_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");

                }
                //Subloan2 Adjust 
                
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

                //child loan update subloan2
                sbqry.Append("UPDATE pr_emp_adv_loans_child SET principal_amount_recovered=" + Principal_amt_Recovered2
                   + PrinRecFlag +
               ",total_amount_recovered=" + Total_amount_recovered2 + " , os_principal_amount=" + Os_Principal_Amt2 + ", " +
               "os_total_amount= " + Os_Total_Amt2 + " where id=" + ChLnPkid2 + " AND active=1;");
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", ChLnPkid1.ToString(), Install_amt1.ToString()));
            }
            //execute sql statements in one shot
            bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

            return bRet;

        }
        //2w loan
        private async Task<bool> TwoWheelerLoanPayments(DataTable dtLoan, DataTable dtFm, int instAmt, char PaymentType, DataTable dtPrevPartPay)
        {
            bool bRet = false;
            int Prin_bal_amt = 0;
            int no_of_installments_cover = 0;
            int Os_Total_Amt = 0;
            int Remaining_installments = 0;
            //1. get data from dtLoan
            //string payment_type = PaymentType.ToString();
            string payment_mode = PaymentType == 'I' ? PrConstants.LOAN_INSTALLMENT : PrConstants.LOAN_PARTPAYMENT;
            string Loan_Code = dtLoan.Rows[0]["loan_id"].ToString();
            int Loan_id = Convert.ToInt32(dtLoan.Rows[0]["mastertypeid"]);
            int Priority = Convert.ToInt32(dtLoan.Rows[0]["priority"]);
            int LnPkid = Convert.ToInt32(dtLoan.Rows[0]["emp_adv_loans_mid"]);
            int ChLnPkid = Convert.ToInt32(dtLoan.Rows[0]["childloanid"]);
            string Method = dtLoan.Rows[0]["method"].ToString();
            int Principal_amt_Recovered = Convert.ToInt32(dtLoan.Rows[0]["principal_amount_recovered"]);
            int Total_amt_Recovered = Convert.ToInt32(dtLoan.Rows[0]["total_amount_recovered"]);
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
            int IntRATE = Convert.ToInt32(dtLoan.Rows[0]["interest_rate"]);
            DateTime sandate = Convert.ToDateTime(dtLoan.Rows[0]["sanction_date"]);
            Prin_bal_amt = Os_Principal_Amt - instAmt;
            no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(instAmt / Install_amt)).ToString());
            Completed_installments = Completed_installments + no_of_installments_cover;
            Remaining_installments = Total_installment - Completed_installments;
            Principal_amt_Recovered = Principal_amt_Recovered + instAmt;
            Total_amount_recovered = Principal_amt_Recovered;

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
            if (dtPrevPartPay.Rows.Count > 0)
            {
                prePartPayDate = Convert.ToDateTime(dtPrevPartPay.Rows[0]["cash_paid_on"].ToString());
            }

            double intrestAmt = await calcIntrestAmt(Os_Principal_Amt, IntRATE, sandate, paidDate, new DateTime(1900, 1, 1), prePartPayDate);

            string intrst_accured = "";
            if (intrestAmt > 0)
            {
                intrst_accured = ", interest_accured=" + intrestAmt + ",os_interest_amount=" + Convert.ToInt32(Os_Intrst_Amt + intrestAmt);
            }
            //Os_Principal_Amt = Prin_bal_amt;
            Os_Total_Amt = Prin_bal_amt;

            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
            sbqry.Append(GenNewTransactionString());
            //loan Adjust
            sbqry.Append("INSERT INTO pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                "[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
               + "(@idnew+1," + LnPkid + "," + ChLnPkid + "," + Os_Principal_Amt + "," + instAmt + "," + Prin_bal_amt + "," + intrestAmt + "," +
               "" + Os_Intrst_Amt + "," + 0 + "," + Convert.ToInt32(Os_Intrst_Amt + intrestAmt) + ",'" + payment_mode + "','" + payment_mode + "', '" + installments_paid_date + "'," +
               "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + instAmt + "," +
               "" + no_of_installments_cover + "," + Install_amt + ",1, @transidnew,'" + fm + "'," + fy + "," + Loan_Sl_No + ");");

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
            //3. child loan update
            sbqry.Append("UPDATE pr_emp_adv_loans_child SET principal_amount_recovered=" + Principal_amt_Recovered
                + intrst_accured
                + ",total_amount_recovered=" + Total_amount_recovered + " , os_principal_amount=" + Os_Principal_Amt
                + ", os_total_amount= " + Os_Total_Amt + " where id=" + ChLnPkid + " AND active=1;");

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", ChLnPkid.ToString(), Install_amt.ToString()));
            //4. parent loan update
            string updInstallAmt = "";
            if (Os_Principal_Amt > 0 && Os_Principal_Amt < Install_amt)
            {
                updInstallAmt = ", installment_amount=" + Os_Principal_Amt;
                sbqry.Append("UPDATE pr_emp_adv_loans SET completed_installment=" + Completed_installments
               + updInstallAmt
               + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
               + Total_amount_recovered + ",active=1 " + " where id=" + LnPkid + " ;");
            }
            if (Os_Principal_Amt == 0)
            {
                updInstallAmt += "principal_recovered_flag=1";
                sbqry.Append("UPDATE pr_emp_adv_loans_child SET principal_recovered_flag = 1 where emp_adv_loans_mid=" + LnPkid + " ;");
                //intrest should be calculated
                
            }

            sbqry.Append("UPDATE pr_emp_adv_loans SET completed_installment=" + Completed_installments
                + updInstallAmt
                + ",remaining_installment=" + Remaining_installments + " ,total_recovered_amount="
                + Total_amount_recovered + ",active=1 " + " where id=" + LnPkid + " ;");

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", LnPkid.ToString(), LnPkid.ToString()));
            //execute sql statements in one shot
            bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

            return bRet;
        }

        private async Task<double> calcIntrestAmt(int osPrinc, double iRate, DateTime loanSactDate, DateTime dtFm, DateTime dtPartPay, DateTime dtPrevPartPay)
        {
            int yrDays = 365;
            double retIntAmt = 0;

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

            return retIntAmt;
        }

    }
}
