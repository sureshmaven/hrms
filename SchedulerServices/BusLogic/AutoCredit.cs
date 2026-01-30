using BusLogic.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusLogic
{
    public class AutoCredit : CommonService
    {
        private string _dbConnStr;
        public AutoCredit(string dbConnStr) : base(dbConnStr)
        {
            _dbConnStr = dbConnStr;
        }

        public void AutoCredits()
        {
            try
            {
                string qry1 = "";

                string newempid = getAllEmpIdsForemployeedata();
                string newempids = newempid.TrimEnd(',');
                var arrnewEmps = newempids.Split(',');
                string empidautocredit = getAllEmpIdsACBsFromEmployees();
                string leaveempid = getAllEmpIdsLeavesFromEmployees();
                string leavesempid = leaveempid.TrimEnd(',');
                string empids = empidautocredit.TrimEnd(',');
                string empbalce = getAllEmpIdsLeaveBalanceEmployees();
                string empbalceempid = empbalce.TrimEnd(',');
                string currDate = DateTime.Now.ToString("yyyy-MM-dd");
                if (newempid != "")
                {
                    foreach (var emp in arrnewEmps)
                    {
                        var arremp = emp.Split('#');
                        var dept = arremp[1];
                        var Branches = arremp[2];
                        var desig = arremp[3];
                        qry1 += " Insert into Leaves_CreditDebit(EmpId, LeaveTypeId, CreditLeave, DebitLeave, UpdatedBy, UpdatedDate, LeaveBalance, Comments, EmpName, Department, Branch, CurrentDesignation,[Type], Head_Branch_Value, TotalBalance,Year)values('" + arremp[0] + "', 1, 1, 0, 6372, GETDATE(),0, 'AutoCreditCL', '" + arremp[2] + "' ," + arremp[3] + ", " + arremp[4] + "," + arremp[6] + ",'Credit', " + arremp[5] + ",1,Year(GETDATE()))";
                    }
                }



                string qryIns2 = "";
                // string qry2 = "";
                string query = "";
                var dor = DateTime.Now.Date;

                var arrEmps = empids.Split(',');

                foreach (var emp in arrEmps)
                {
                    if (emp != "")
                    {

                        var arremp = emp.Split('#');
                        string empidss = arremp[0];
                        string leavebalance = arremp[2];
                        int leavenewbal = Convert.ToInt32(leavebalance) + 1;
                        string leavebal = leavenewbal.ToString();
                        string totalbalance = arremp[3];
                        int totalbalancenew = Convert.ToInt32(totalbalance) + 1;
                        string totalbal = totalbalancenew.ToString();
                        query += " Insert into Leaves_CreditDebit(EmpId, LeaveTypeId, CreditLeave, DebitLeave, UpdatedBy, UpdatedDate, LeaveBalance, Comments, EmpName, Department, Branch, CurrentDesignation,[Type], Head_Branch_Value, TotalBalance,Year)values('" + arremp[0] + "', 1, 1, 0, 6372, GETDATE()," + leavebal + " , 'AutoCreditCL', '" + arremp[1] + "' ," + arremp[4] + ", " + arremp[5] + "," + arremp[6] + ",'Credit', '" + arremp[7] + "',  " + totalbal + ",Year(GETDATE()));";


                    }
                }
                var leaveaeeEmps = leavesempid.Split(',');

                var empbalance = empbalceempid.Split(',');
                foreach (var emps in empbalance)
                {

                    if (emps != "")
                    {


                        var leavebalance = emps.Split('#');
                        string empidsbal = leavebalance[0];
                        int leaveyear = DateTime.Now.Year;
                        string leavebal = leavebalance[1];
                        int clbalan = Convert.ToInt32(leavebal);
                        int clbalance = clbalan + 1;
                        qryIns2 += " UPDATE EmpLeaveBalance SET LeaveBalance='" + clbalance + "' where EmpId=" + empidsbal + " and LeaveTypeId=1 and Year =Year(GETDATE())";

                    }
                }
                string qryLog = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action],[Key1],[Val1]) "
                  + "VALUES(getdate(),'AutoCredit','Insert/Update','Emp Ids','All Emps' )";

                SqlHelper sh = new SqlHelper(_dbConnStr);
                sh.Run_UPDDEL_ExecuteNonQuery(qry1 + query + qryIns2 + qryLog);
                // sh.Run_UPDDEL_ExecuteNonQuery(qryIns2 + qryLog);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }
        private string Empids()
        {
            string retStr = "";

            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            string selqry1 = " Select EmpId from autoleavecredit where CreditDate='" + currDate + "'";
            SqlHelper sh = new SqlHelper(_dbConnStr);
            DataTable dtEmpIds = sh.Get_Table_FromQry(selqry1);

            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["EmpId"].ToString() + ",";
            }
            return retStr;
        }
        private string getAllEmpIdsForemployeedata()
        {
            string retStr = "";

            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            string empids = Empids();
            string empid = empids.TrimEnd(',');
            string creditdebittop1 = getAllEmpIdsACBsFromEmployees();
            if (empid != "" && creditdebittop1 == "")
            {
                string selQry = " Select Id,EmpId,Department,Branch,CurrentDesignation,Branch_Value1,ShortName from Employees where id in('" + empid + "')";


                SqlHelper sh = new SqlHelper(_dbConnStr);
                DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);

                foreach (DataRow dr in dtEmpIds.Rows)
                {
                    retStr = retStr + dr["Id"].ToString() + "#" + dr["EmpId"].ToString() + "#" + dr["ShortName"].ToString() + "#" + dr["Department"].ToString() + "#" + dr["Branch"].ToString() + "#" + dr["Branch_Value1"].ToString() + "#" + dr["CurrentDesignation"].ToString() + ",";

                }
            }
            return retStr;
        }
        private string getAllEmpIdsACBsFromEmployees()
        {
            try
            {
                string retStr = "";
                string empidcredit = Empids();
                // string empids = empidcredit.TrimEnd(',');
                // var empidss = empidcredit.Split(',');
                List<string> empidss = new List<string>();
                if (!string.IsNullOrEmpty(empidcredit))
                {
                    empidss = empidcredit.Split(new char[] { ',' }).ToList();
                    empidss.Remove("");
                }
                string currDate = DateTime.Now.ToString("yyyy-MM-dd");
                for (int i = 0; i < empidss.Count; i++)
                {
                    string selQry = "";
                    selQry += " Select  top 1 EmpId,EmpName,LeaveBalance,TotalBalance,Department,Branch,CurrentDesignation,Head_Branch_value from Leaves_CreditDebit  WHERE EmpId in (" + empidss[i] + ") order by UpdatedDate desc; ";
                    //autoleavecredit}
                    SqlHelper sh = new SqlHelper(_dbConnStr);
                    DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);

                    foreach (DataRow dr in dtEmpIds.Rows)
                    {
                        retStr = retStr + dr["EmpId"].ToString() + "#" + dr["EmpName"].ToString() + "#" + dr["LeaveBalance"].ToString() + "#" + dr["TotalBalance"].ToString() + "#" + dr["Department"].ToString() + "#" + dr["Branch"].ToString() + "#" + dr["CurrentDesignation"].ToString() + "#" + dr["Head_Branch_value"].ToString() + ",";
                    }
                }
                return retStr;
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        private string getAllEmpIdsLeavesFromEmployees()
        {
            string retStr = "";
            string empidcredit = Empids();
            // string empids = empidcredit.TrimEnd(',');
            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            List<string> empidss = new List<string>();
            if (!string.IsNullOrEmpty(empidcredit))
            {
                empidss = empidcredit.Split(new char[] { ',' }).ToList();
                empidss.Remove("");
            }
            for (int i = 0; i < empidss.Count; i++)
            {
                string selQry = "";

                selQry += " Select distinct top 1 EmpId,ControllingAuthority,SanctioningAuthority,BranchId,DepartmentId,DesignationId from Leaves WHERE EmpId in (" + empidss[i] + ");";

                SqlHelper sh = new SqlHelper(_dbConnStr);
                DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);

                foreach (DataRow dr in dtEmpIds.Rows)
                {
                    retStr = retStr + dr["EmpId"].ToString() + "#" + dr["ControllingAuthority"].ToString() + "#" + dr["SanctioningAuthority"].ToString() + "#" + dr["BranchId"].ToString() + "#" + dr["DepartmentId"].ToString() + "#" + dr["DesignationId"].ToString() + ",";
                }
            }

            return retStr;
        }
        private string getAllEmpIdsLeaveBalanceEmployees()
        {
            string retStr = "";
            string empidcredit = Empids();
            //string empids = empidcredit.TrimEnd(',');
            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            // string selqry1 = " Select EmpId from autoleavecredit where CreditDate='" + currDate + "'";
            List<string> empidss = new List<string>();
            if (!string.IsNullOrEmpty(empidcredit))
            {
                empidss = empidcredit.Split(new char[] { ',' }).ToList();
                empidss.Remove("");
            }
            for (int i = 0; i < empidss.Count; i++)
            {
                string selQry = "";
                selQry += " Select EmpId,LeaveBalance from EmpLeaveBalance  WHERE EmpId in (" + empidss[i] + ") and LeaveTypeId=1 and year=Year(GETDATE());";
                //autoleavecredit
                SqlHelper sh = new SqlHelper(_dbConnStr);
                DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);

                foreach (DataRow dr in dtEmpIds.Rows)
                {
                    retStr = retStr + dr["EmpId"].ToString() + "#" + dr["LeaveBalance"].ToString() + ",";
                }
            }
            return retStr;
        }
    }
}
