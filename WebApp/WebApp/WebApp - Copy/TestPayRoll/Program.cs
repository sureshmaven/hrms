using PayRollBusiness.Process;
using PayrollModels;
using System;

namespace TestPayRoll
{
    class Program
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            LoansPaymentProcess loanProc = new LoansPaymentProcess(getLoginCredential(), _logger);
            
            //test loans payments            
            var flg = loanProc.InstallmentPartPayments(6720, 6000, 'I').GetAwaiter().GetResult();
            var flg1 = loanProc.InstallmentPartPayments(6720, 12000, 'P').GetAwaiter().GetResult();
        }    

    private static LoginCredential getLoginCredential()
        {
            //string qryfm = " select fm from pr_month_details where active=1";

            return new LoginCredential
            {
                AppName = "PR Service",
                AppVersion = "1.0.0",
                AppEnvironment = "Dev",
                FinancialMonthDate = DateTime.Now,
                FY = 2020,
                EmpCode = 0,
                EmpShortName = "Service"
            };
        }
    }
}
