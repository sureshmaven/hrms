using BusLogic.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusLogic
{
    public class TodayFutureTransfers : CommonService
    {
        private string _dbConnStr;
        public TodayFutureTransfers(string dbConnStr) : base(dbConnStr)
        {
            _dbConnStr = dbConnStr;
        }


        public void TodayTempTransfers()
        {
            string empids = getAllEmpIdsFromTempTransfers();
            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            if (empids != "")
            {
                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(empids))
                {
                    lCode = empids.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                string qryIns2 = "";
                for (int i = 0; i < lCode.Count; i++)
                {
                    qryIns2 += "UPDATE  e SET e.branch = l.NewBranch, e.Department = l.NewDepartment,e.Branch_Value1 =case when l.NewDepartment=46 then 43 else 42 end FROM employees AS e  JOIN  Employee_Transfer as l on e.id=l.empid " +
                        " WHERE  e.id = " + lCode[i] + "and EffectiveFrom='" + currDate + "' and l.[Type]='TemporaryTransfer';";

                }
                string qryLog = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action],[Key1],[Val1]) "
                       + "VALUES(getdate(),'TemporaryTransfer','Updated','Emp Ids','" + empids + "' )";

                SqlHelper sh = new SqlHelper(_dbConnStr);
                sh.Run_UPDDEL_ExecuteNonQuery(qryIns2 + qryLog);
            }

        }
        private string getAllEmpIdsFromTempTransfers()
        {
            string retStr = "";

            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            string selQry = "Select EmpId from Employee_Transfer where EffectiveFrom='" + currDate + "' AND [Type]='TemporaryTransfer' ";

            SqlHelper sh = new SqlHelper(_dbConnStr);
            DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);
            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["EmpId"].ToString() + ",";
            }
            return retStr;
        }
    }
}
