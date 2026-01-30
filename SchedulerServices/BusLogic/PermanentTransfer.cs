using BusLogic.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusLogic
{
   public class PermanentTransfer : CommonService
    {
        private string _dbConnStr;
        public PermanentTransfer(string dbConnStr) : base(dbConnStr)
        {
            _dbConnStr = dbConnStr;
        }
        public void PermanentTransfers()
        {
            try
            {
                string empid = getAllEmpIdsFrompermTransfers();
                string empids = empid.TrimEnd(',');
                if (empids != "")
                {
                    //List<string> lCode = new List<string>();
                    var arrEmps = empids.Split(',');

                    string updQry = "";
                    string updQry1 = "";
                    foreach (var emp in arrEmps)
                    {
                        var arremp = emp.Split('#');
                        var dept= arremp[1];
                        var Branches =arremp[2];
                        var desig= arremp[3];
                        updQry += "UPDATE Employees SET Department =" + dept + ",Branch=" + Branches + ",CurrentDesignation=" + desig + ",PerBranch=" + Branches + ",PerDepartment=" + dept + " WHERE Id=" + arremp[0] + "; ";
                        updQry1 += "UPDATE Employees SET Branch_Value1 =case when Employees.Department=46 then 43 else 42 end WHERE Id=" + arremp[0] + "; ";
                    }

                        string qryLog = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action],[Key1],[Val1]) "
                         + "VALUES(getdate(),'PermanentTransfer','Update','Emp Ids','" + empids + "' )";

                        SqlHelper sh = new SqlHelper(_dbConnStr);
                        sh.Run_UPDDEL_ExecuteNonQuery(updQry+ updQry1 + qryLog);
                    

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }
        private string getAllEmpIdsFrompermTransfers()
        {
            string retStr = "";

            string currDate = DateTime.Now.ToString("yyyy-MM-dd");

            string selQry = "Select  EmpId,NewDepartment,NewBranch,NewDesignation from Employee_Transfer where EffectiveFrom='" + currDate + "' AND [Type]='PermanentTransfer' order by UpdatedDate desc  ";
             

            SqlHelper sh = new SqlHelper(_dbConnStr);
            DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);

            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["EmpId"].ToString() + "#" + dr["NewDepartment"].ToString() + "#" + dr["NewBranch"].ToString() + "#" + dr["NewDesignation"].ToString() + ",";

            }
            return retStr;
        }
    }
}
