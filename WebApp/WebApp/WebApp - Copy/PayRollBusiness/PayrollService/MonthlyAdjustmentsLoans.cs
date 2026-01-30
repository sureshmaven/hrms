using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.PayrollService
{
    public class MonthlyAdjustmentsLoans : BusinessBase
    {
        log4net.ILog _logger = null;

        //*** start of common code ***
        public MonthlyAdjustmentsLoans(LoginCredential loginCredential, log4net.ILog logger) : base(loginCredential)
        {
            _logger = logger;
        }

        public async Task ServiceStarting(string servicename)
        {
            string qryIns = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action]) "
                + "VALUES(getdate(),'" + servicename + "','Start');";
            qryIns += "SELECT CAST(SCOPE_IDENTITY() as int);";
            await _sha.Run_INS_ExecuteScalar(qryIns);
        }
        public async Task ServiceStoping(string servicename)
        {
            string qryIns = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action]) "
                + "VALUES(getdate(),'" + servicename + "','Stop');";
            qryIns += "SELECT CAST(SCOPE_IDENTITY() as int);";

            await _sha.Run_INS_ExecuteScalar(qryIns);
        }

        //*** End of common code ***

        public async Task<bool> UpdateMonthlyLoanInstallments()
        {
            bool bRet = false;
           
            //qry for all emps        
            string getAllEmployee = "select distinct el.emp_code from Employees e "+
                                   " join pr_emp_adv_loans el on e.EmpId = el.emp_code "+
                                   " where el.active = 1";

            string qryfm = " select fm from pr_month_details where active=1";

            DataSet dtEmployees = await _sha.Get_MultiTables_FromQry(getAllEmployee + qryfm);

            DataTable dtEmployeesdata = dtEmployees.Tables[0];
            DataTable dtFm = dtEmployees.Tables[1];

            foreach (DataRow drEmp in dtEmployeesdata.Rows)
            {
                int Empcode = 452;// Convert.ToInt32(drEmp["emp_code"].ToString());

                string getLoans = " select mas.loan_id,mas.id as mastertypeid,chi.priority,chi.Id as childloanid,chi.emp_adv_loans_mid from pr_emp_adv_loans adv" +
                                " join pr_loan_master mas on adv.loan_type_mid = mas.id " +
                                " join pr_emp_adv_loans_child chi on adv.id = chi.emp_adv_loans_mid where adv.active = 1 AND adv.emp_code=" + Empcode + " order by loan_id";


                DataTable dtLoans = await _sha.Get_Table_FromQry(getLoans);

                if (dtLoans.Rows.Count > 0)
                {
                    foreach (DataRow drinstallment in dtLoans.Rows)
                    {
                        string loanType = drinstallment["loan_id"].ToString().ToUpper();
                        int loanid = Convert.ToInt32(drinstallment["mastertypeid"].ToString());
                        int priority = Convert.ToInt32(drinstallment["priority"]);
                        int emp_adv_loans_child_mid = Convert.ToInt32(drinstallment["childloanid"].ToString());
                        int emp_adv_loans_mid = Convert.ToInt32(drinstallment["emp_adv_loans_mid"].ToString());

                        //Fiance Month
                        DateTime installmentDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                        int Month = installmentDate.Month;
                        int Year = installmentDate.Year;

                        //1. loan type FEST
                        if (loanType == PrConstants.FESTIVAL_LOAN_CODE && priority == 1)
                        {

                            bRet = UpdateInstallmentAmountForFest(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, loanid, Month, Year).GetAwaiter().GetResult();

                        }

                        //2. loan Type 
                        else if (loanType != PrConstants.PF_LOAN1_CODE && loanType != PrConstants.PF_LOAN2_CODE && loanType != PrConstants.FESTIVAL_LOAN_CODE)
                        {
                            bRet = UpadateinstallmentsForHL2(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, loanid,Month, Year).GetAwaiter().GetResult();
                        }


                    }
                }
                return bRet;

            }
            return bRet;

        }

       
        //For Fest Loans
        private async Task<bool> UpdateInstallmentAmountForFest(int EmpCode, int emp_adv_loans_mid, int emp_adv_loans_child_mid, int loanid, int month, int year)
        {
            StringBuilder sbqry = null;

            bool bRet = false;

            string qry = "";

            string getLoans = "select chi.loan_amount,chi.os_principal_amount,chi.os_this_month_interest,chi.os_interest_amount,chi.principal_amount_recovered, chi.principal_recovered_flag,chi.interest_accured,chi.priority,chi.interest_amount_recovered,chi.interest_rate,chi.total_amount_recovered,chi.os_total_amount,adv.installment_amount,adv.completed_installment,adv.remaining_installment,adv.total_installment,adv.installment_start_date from pr_emp_adv_loans adv" +
                              " join pr_emp_adv_loans_child chi on adv.id =" + emp_adv_loans_mid +
                              " where adv.active = 1 AND adv.emp_code =" + EmpCode + " AND chi.id =" + emp_adv_loans_child_mid; //+ " AND adv.installment_start_date <= '" + installment_Date + "'";

            string qrygetinstallment_amount = "select dd_amount from pr_emp_payslip psl " +
                                             " join pr_emp_payslip_deductions ded on psl.id = ded.payslip_mid " +
                                             " where psl.emp_code=" + EmpCode + " AND ded.dd_type='loan'" + " AND ded.dd_mid=" + loanid + " AND month(psl.fm)=" + month + " AND year(psl.fm)=" + year;

            string qryfm = "select fm from pr_month_details where active=1";


            DataSet dsfestlonas = await _sha.Get_MultiTables_FromQry(getLoans + qrygetinstallment_amount + qryfm);

            DataTable dtloansdata = dsfestlonas.Tables[0];
            DataTable dtddamount = dsfestlonas.Tables[1];
            DataTable dtfince = dsfestlonas.Tables[2];

            if (dtloansdata.Rows.Count > 0 && dtddamount.Rows.Count > 0)
            {
                foreach (DataRow drFest in dtloansdata.Rows)
               {
                    sbqry = new StringBuilder();


                    //installments calculation
                    int installmentamount = Convert.ToInt32(dtddamount.Rows[0]["dd_amount"].ToString());
                    int completed_installments = Convert.ToInt32(drFest["completed_installment"].ToString());
                    int remaining_installments = Convert.ToInt32(drFest["remaining_installment"].ToString());
                    int no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(installmentamount / installmentamount)).ToString());
                    int total_installments = Convert.ToInt32(drFest["total_installment"].ToString());
                    completed_installments = completed_installments + no_of_installments_cover;
                    remaining_installments = total_installments - completed_installments;

                    int os_principal_amount = Convert.ToInt32(drFest["os_principal_amount"].ToString());
                    int os_total_amount = Convert.ToInt32(drFest["os_total_amount"].ToString());

                    int loan_amount= Convert.ToInt32(drFest["loan_amount"].ToString());
                    //int loan_amount = Convert.ToInt32(drFest["loan_amount"].ToString());


                    int principal_amount_recovered = Convert.ToInt32(drFest["principal_amount_recovered"].ToString());
                    int principal_open_amount = os_principal_amount;
                    int principal_balance_amount = principal_open_amount - installmentamount;
                    int interest_accured = Convert.ToInt32(drFest["interest_accured"].ToString());

                    os_principal_amount = principal_balance_amount;
                    os_total_amount = principal_balance_amount;

                    bool principal_recovered_flag = Convert.ToBoolean(drFest["principal_recovered_flag"].ToString());
                

                    //Payments Dates
                    DateTime paidDate = Convert.ToDateTime(dtfince.Rows[0]["fm"].ToString()); 
                    string installments_paid_date =(paidDate.ToString("yyyy-MM-dd"));
                    string Cash_paid_on = installments_paid_date;

                    //PaymentMode
                    string payment_type = "Salary";
                    string payment_mode = "Salary";

                    //trans_id
                    sbqry.Append(GenNewTransactionString());

                    if (!principal_recovered_flag)
                    {
                        //1. qry---insert into pr_emp_adv_loans_adjustments
                        sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                        qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid],[installments_amount],[active],[trans_id]) values "
                           + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + installmentamount + "," + principal_balance_amount + "," + 0 + "," + 0 + "," + 0 + ",'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," + no_of_installments_cover + ",'"+ installments_paid_date + "',"+ installmentamount +"," + completed_installments+","+ installmentamount + ",1, @transidnew);";
                        sbqry.Append(qry);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));


                        //2. qry update pr_emp_adv_loans_child table
                        int interest_amount_recovered = Convert.ToInt32(drFest["interest_amount_recovered"].ToString());
                        double total_amount_recovered = Convert.ToDouble(drFest["total_amount_recovered"].ToString());

                        principal_amount_recovered = principal_amount_recovered + installmentamount;
                        total_amount_recovered = interest_amount_recovered + principal_amount_recovered;

                        double total_recovered_amount = installmentamount * completed_installments;


                        if (total_recovered_amount != loan_amount)
                        {
                            qry = "update pr_emp_adv_loans_child SET principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + " where id=" + emp_adv_loans_child_mid + " AND priority=1 AND active=1;";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));
                        }
                        else
                            qry = "update pr_emp_adv_loans_child SET principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + ",active=0 " + ",principal_recovered_flag=1" + ",priority=0" + " where id=" + emp_adv_loans_child_mid + " AND priority=1;";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));
                        

                        //3.qry update pr_emp_adv_loans table
                        if (total_recovered_amount != loan_amount)
                        {
                            qry = "update pr_emp_adv_loans SET completed_installment=" + completed_installments + ",remaining_installment=" + remaining_installments + " ,total_recovered_amount=" + total_recovered_amount + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), EmpCode.ToString()));
                        }
                        else
                            qry = "update pr_emp_adv_loans SET completed_installment=" + completed_installments + ",remaining_installment=" + remaining_installments + " ,total_recovered_amount=" + total_recovered_amount + ",active=0 " + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), EmpCode.ToString()));
                        
                            

                    }
                   
                    try
                    {
                      bRet=  await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

                    }
                    catch (Exception ex)
                    {
                        _logger.Info(sbqry.ToString());
                        _logger.Error(ex.Message);
                        _logger.Error(ex.StackTrace);

                        bRet = false;
                    }


                }


            }
            return bRet;

        }


        private async Task<bool> UpadateinstallmentsForHL2(int EmpCode,int emp_adv_loans_mid, int emp_adv_loans_child_mid, int loanid, int Month, int Year )
        {
            StringBuilder sbqry = null;
            bool bRet = false;

            string qry = "";

            string qrygetLoans = "select chi.principal_recovered_flag,chi.loan_amount,chi.os_principal_amount,chi.os_this_month_interest,chi.os_interest_amount,chi.date_disburse,chi.os_interest_amount,chi.principal_amount_recovered,chi.interest_accured,chi.interest_rate,chi.priority,chi.interest_amount_recovered,chi.interest_rate,chi.total_amount_recovered,chi.os_total_amount,adv.installment_amount,adv.completed_installment,adv.remaining_installment,adv.installment_start_date,adv.principal_installment,adv.interest_installment from pr_emp_adv_loans adv" +
                             " join pr_emp_adv_loans_child chi on adv.id =" + emp_adv_loans_mid +
                             " where adv.active = 1 AND adv.emp_code =" + EmpCode + " AND chi.id =" + emp_adv_loans_child_mid;

           string qrygetinstallment_amount = "select dd_amount from pr_emp_payslip psl " +
                                             " join pr_emp_payslip_deductions ded on psl.id = ded.payslip_mid " +
                                             " where psl.emp_code=" + EmpCode + " AND ded.dd_type='loan'" + " AND ded.dd_mid=" + loanid + " AND month(psl.fm)=" + Month + " AND year(psl.fm)=" + Year;

            string qryfm = "select fm from pr_month_details where active=1";

            DataSet dtloans = await _sha.Get_MultiTables_FromQry(qrygetLoans + qrygetinstallment_amount + qryfm);

            DataTable dtloansdata = dtloans.Tables[0];
            DataTable dtdd_amount = dtloans.Tables[1];
            DataTable dtfm = dtloans.Tables[3];

            if (dtloansdata.Rows.Count > 0 && dtdd_amount.Rows.Count > 0)
            {
                foreach(DataRow drinstall in dtloansdata.Rows)
                {
                    sbqry = new StringBuilder();


                    //installments calculation
                    int installmentamount = Convert.ToInt32(dtdd_amount.Rows[0]["dd_amount"].ToString());
                    int completed_installments = Convert.ToInt32(drinstall["completed_installment"].ToString());
                    int remaining_installments = Convert.ToInt32(drinstall["remaining_installment"].ToString());
                    int no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(installmentamount / installmentamount)).ToString());
                    int total_installments = Convert.ToInt32(drinstall["total_installment"].ToString());
                    completed_installments = completed_installments + no_of_installments_cover;
                    remaining_installments = total_installments - completed_installments;

                    int os_principal_amount = Convert.ToInt32(drinstall["os_principal_amount"].ToString());
                    double os_total_amount = Convert.ToDouble(drinstall["os_total_amount"].ToString());
                    int loan_amount = Convert.ToInt32(drinstall["loan_amount"].ToString());

                    //principal amount
                    int principal_amount_recovered = Convert.ToInt32(drinstall["principal_amount_recovered"].ToString());
                    int principal_open_amount = os_principal_amount;
                    int principal_balance_amount = principal_open_amount - installmentamount;

                    int principal_installment = Convert.ToInt32(drinstall["principal_installment"].ToString());

                    //interest amount
                    int interest_installment = Convert.ToInt32(drinstall["interest_installment"].ToString());
                    int interest_accured = Convert.ToInt32(drinstall["interest_accured"].ToString());
                    double interest_rate2 = Convert.ToDouble(drinstall["interest_rate"].ToString());
                    int os_interest_amount = Convert.ToInt32(drinstall["os_interest_amount"].ToString());

                    double interest_open_amount = Convert.ToDouble(loan_amount) * interest_rate2;
                    int interest_paid_amount = os_interest_amount;
                    double interest_balance_amount = interest_open_amount - interest_installment;

                    int div_fm_interest = 100 * 12;
                   

                    double os_this_month_interest = interest_open_amount / div_fm_interest;
                     os_total_amount = Convert.ToDouble(loan_amount) + Convert.ToDouble(os_interest_amount);
                    os_principal_amount = principal_balance_amount;


                   int divintr_install = Convert.ToInt32(interest_installment) / 4; //2nd priority interest installments 48/4=12
                   int Fpriority_intinstall = divintr_install * 3; //1st priority interest installments 12*3=36
                   int Spriority_intinstall = Convert.ToInt32(principal_installment) + Convert.ToInt32(Fpriority_intinstall - 1); // 


                    bool principal_recovered_flag = Convert.ToBoolean(drinstall["principal_recovered_flag"].ToString());

                    //Payments Dates
                    DateTime paidDate = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());
                    DateTime dtAmountDisburse = Convert.ToDateTime(drinstall["date_disburse"].ToString());
                    string installments_paid_date = paidDate.ToString("yyy-MM-dd");
                    string Cash_paid_on = paidDate.ToString("yyy-MM-dd");
                    string date_disburse = dtAmountDisburse.ToString("yyyy-MM-dd");

                    //interest calculation
                    int days= calDaysForAmount(dtAmountDisburse);

                  
                    //PaymentMode
                    string payment_type = "Salary";
                    string payment_mode = "Salary";

                    //trans_id
                    sbqry.Append(GenNewTransactionString());

                    

                    //1. qry---insert into pr_emp_adv_loans_adjustments
                    if (!principal_recovered_flag)
                    {

                        sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                        qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id]) values "
                           + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + installmentamount + "," + principal_balance_amount + "," + interest_open_amount + "," + interest_paid_amount + "," + interest_balance_amount + ",'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," + completed_installments + "," + installmentamount + ",1, @transidnew);";
                        sbqry.Append(qry);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

                      
                        //2. qry update pr_emp_adv_loans_child table
                        int interest_amount_recovered = Convert.ToInt32(drinstall["interest_amount_recovered"].ToString());
                        double total_amount_recovered = Convert.ToDouble(drinstall["total_amount_recovered"].ToString());

                        principal_amount_recovered = principal_amount_recovered + installmentamount;
                        total_amount_recovered = interest_amount_recovered + principal_amount_recovered;

                        double total_recovered_amount = installmentamount * completed_installments;


                        if (total_recovered_amount != loan_amount)
                        {
                            qry = "update pr_emp_adv_loans_child SET principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + " where id=" + emp_adv_loans_child_mid + " AND priority=1 AND active=1;";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));
                        }
                        else
                        qry = "update pr_emp_adv_loans_child SET principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount+ ",principal_recovered_flag=1" + " where id=" + emp_adv_loans_child_mid + " AND priority=1;";
                        sbqry.Append(qry);

                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));


                          //3.qry update pr_emp_adv_loans table
                        
                           qry = "update pr_emp_adv_loans SET completed_installment=" + completed_installments + ",remaining_installment=" + remaining_installments + " ,total_recovered_amount=" + total_recovered_amount + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                           sbqry.Append(qry);

                           sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), EmpCode.ToString()));
                                                
                    }

                    else
                    {
                        //1. insert into pr_emp_adv_loans_adjustments table
                        sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                        qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id]) values "
                           + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + installmentamount + "," + principal_balance_amount + "," + interest_open_amount + "," + interest_paid_amount + "," + interest_balance_amount + ",'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," + completed_installments + "," + installmentamount + ",1, @transidnew);";
                        sbqry.Append(qry);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));


                        //2. qry update pr_emp_adv_loans_child table
                        int interest_amount_recovered = Convert.ToInt32(drinstall["interest_amount_recovered"].ToString());
                        double total_amount_recovered = Convert.ToDouble(drinstall["total_amount_recovered"].ToString());

                        principal_amount_recovered = principal_amount_recovered + installmentamount;
                        total_amount_recovered = interest_amount_recovered + principal_amount_recovered;

                        double total_recovered_amount = installmentamount * completed_installments;




                    }
                    
                }
            }

            return bRet;
        }

        private int calDaysForAmount(DateTime startMonth)
        {
            DateTime lastDayDate = new DateTime(startMonth.Year, startMonth.Month, 1).AddMonths(1).AddDays(-1);
            return (int)(lastDayDate.Date - startMonth.Date).TotalDays + 1;
        }

    }
}

