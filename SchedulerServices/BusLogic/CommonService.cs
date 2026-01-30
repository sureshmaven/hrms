using BusLogic.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusLogic
{
    public class CommonService
    {
        private string _dbConnStr;
        public CommonService(string dbConnStr)
        {
            _dbConnStr = dbConnStr;
        }

        public void ServiceStarting(string servicename)
        {
            SqlHelper sh = new SqlHelper(_dbConnStr);

            string qryIns = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action]) "
                + "VALUES(getdate(),'"+ servicename +"','Start');";
            qryIns += "SELECT CAST(SCOPE_IDENTITY() as int);";
            sh.Run_INS_ExecuteScalar(qryIns);
        }

        public void ServiceStoping(string servicename)
        {
            SqlHelper sh = new SqlHelper(_dbConnStr);
            string qryIns = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action]) "
                + "VALUES(getdate(),'"+ servicename +"','Stop');";
            qryIns += "SELECT CAST(SCOPE_IDENTITY() as int);";
            sh.Run_INS_ExecuteScalar(qryIns);
        }

    }
}
