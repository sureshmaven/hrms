using BusLogic.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusLogic
{
    public class EmpTotalExp: CommonService
    {
        private string _dbConnStr;
        public EmpTotalExp(string dbConnStr) : base(dbConnStr)
        {
            _dbConnStr = dbConnStr;
        }
        public void Totalexperiences()
        {
            try
            {
                string empidDoj = getAllEmpIdsDOJsFromEmployees();
                string empidDojs = empidDoj.TrimEnd(',');
                if (empidDojs != "")
                {
                    string updQry = "";
                    var dor = DateTime.Now.Date;

                    var arrEmps = empidDojs.Split(',');
                    foreach (var emp in arrEmps)
                    {
                        if (emp != "")
                        {
                            //381#10/10/1997
                            var arremp = emp.Split('#');

                            var doj = DateTime.Parse(arremp[1]);
                            int totalmonths = (dor.Year - doj.Year) * 12 + dor.Month - doj.Month;
                            totalmonths += dor.Day < doj.Day ? -1 : 0;

                            int years = totalmonths / 12;
                            int months = totalmonths % 12;
                            int days = dor.Subtract(doj.AddMonths(totalmonths)).Days ;
                            string TotalExp = years + " Years " + months + " Months " + days + " Days";
                          
                            updQry += "UPDATE Employees SET TotalExperience ='" + TotalExp + "' WHERE EmpId=" + arremp[0] + "; ";
                        }
                    }

                    string qryLog = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action],[Key1],[Val1]) "
                      + "VALUES(getdate(),'Totalexperience','Update','Emp Ids','All Emps' )";

                    SqlHelper sh = new SqlHelper(_dbConnStr);
                    sh.Run_UPDDEL_ExecuteNonQuery(updQry + qryLog);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }
        private string getAllEmpIdsDOJsFromEmployees()
        {
            string retStr = "";

            string currDate = DateTime.Now.ToString("yyyy-MM-dd");

            string selQry = "Select EmpId,DOJ from Employees where RetirementDate >= convert(Date, GETDATE())"; //todo - condition

            SqlHelper sh = new SqlHelper(_dbConnStr);
            DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);

            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["EmpId"].ToString() + "#" + dr["DOJ"].ToString() + ",";
            }
            return retStr;
        }
    }
}
