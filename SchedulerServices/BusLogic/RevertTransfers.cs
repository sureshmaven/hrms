using BusLogic.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusLogic
{
    public class RevertTransfers : CommonService
    {
        private string _dbConnStr;
        public RevertTransfers(string dbConnStr) : base(dbConnStr)
        {
            _dbConnStr = dbConnStr;
        }
     

        public void RevertTempTransfers()
        {
            try
            {
                string empids = getAllEmpIdsFromTempTransfers();
                if (empids != "")
                {
                    List<string> lCode = new List<string>();
                    if (!string.IsNullOrEmpty(empids))
                    {
                        lCode = empids.Split(new char[] { ',' }).ToList();
                        lCode.Remove("");

                        string updQry = "";
                        string qryIns3 = "";
                        for (int i = 0; i < lCode.Count; i++)
                        {
                            updQry += "UPDATE Employees SET Branch =PerBranch,Department=PerDepartment  WHERE Id in (" + lCode[i] + ");";
                            qryIns3 += "UPDATE Employees SET Branch_Value1 =case when Employees.Department = 46 then 43 else 42 end " +
                                                   " WHERE  Employees.id =" + lCode[i] + ";";
                        }

                        string qryLog = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action],[Key1],[Val1]) "
                         + "VALUES(getdate(),'TemporaryTransfer','Rollback','Emp Ids','" + empids + "' )";

                        SqlHelper sh = new SqlHelper(_dbConnStr);
                        sh.Run_UPDDEL_ExecuteNonQuery(updQry+ qryIns3 + qryLog);
                    }

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }

    


        private string getAllEmpIdsFromTempTransfers()
        {
            string retStr = "";

            string currDate = DateTime.Now.ToString("yyyy-MM-dd");

            string selQry = "Select  EmpId from Employee_Transfer where EffectiveTo='" + currDate + "' AND [Type]='TemporaryTransfer' "
                    + "AND EmpId not in (Select EmpId from Employee_Transfer where[Type] = 'TemporaryTransfer' AND EffectiveFrom = dateadd(day, 1, '" + currDate + "')) order by UpdatedDate desc ";

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