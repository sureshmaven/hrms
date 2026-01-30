using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mavensoft.Common;


namespace PayRollBusiness.PayrollService
{
    public class HourEndProcess : BusinessBase
    {

        log4net.ILog _logger = null;
       
        public HourEndProcess(LoginCredential loginCredential, log4net.ILog logger) : base(loginCredential)
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
        public async Task<DataTable> hourprocess()
        {
           
            try
            {
                string currDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string qryfmfy = "  select run_date_time,CompletedEmpCodes,emp_codes,id  from pr_payroll_service_run where active=1 and CompletedEmpCodes is null and  run_date_time <= '" + currDate + "';";
                return await _sha.Get_Table_FromQry(qryfmfy);
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return null;


        }
    }

}
