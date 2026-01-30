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
    public class EmpAllowences : BusinessBase
    {
        //*** start of common code ***
        public EmpAllowences(LoginCredential loginCredential) : base(loginCredential)
        { }
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

        public async Task EmpAllowenceCalculation()
        {
            DataTable dt = await _sha.Get_Table_FromQry("Select id from employees where empid=371");
            string val = dt.Rows[0][0].ToString();
        }
    }
}
