using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.Common;

namespace PayRollBusiness.Process
{
   public class IncrementReportBusiness : BusinessBase
    {
        public IncrementReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        Helper helper = new Helper();
        // search
        public async Task<DataTable> searchIncrementReport(string SearchDatafrom, string SearchDatato)
        {
            //DateTime Fdate = DateTime.Now, Tdate = DateTime.Now;
            //if (SearchDatafrom != "^1")            
            //{
            //    Fdate = Convert.ToDateTime(SearchDatafrom);
            //    Tdate = Convert.ToDateTime(SearchDatato);
            //}

            if (SearchDatafrom.Contains("^"))
            {
                SearchDatafrom = "01-01-00";
                SearchDatato = "01-01-00";
            }

            DateTime Fdate = Convert.ToDateTime(SearchDatafrom);
            DateTime Tdate = Convert.ToDateTime(SearchDatato);

            //string qrySel = "SELECT emp_code AS Code,e.ShortName AS EName,des.Code AS Designation,CONVERT(VARCHAR, increment_date, 105) AS IncDate " +
            //    "FROM pr_emp_inc_anual_stag inc " +
            //    "JOIN employees e ON e.empid = inc.emp_code "   +
            //    "JOIN Designations des ON e.CurrentDesignation = des.Id AND inc.active = 1 " +
            //    "WHERE increment_date BETWEEN '" + Fdate.ToString("yyyy-MM-dd") + "' AND '"+ Tdate.ToString("yyyy-MM-dd") + "';";

            string qrySel = "select e.empId as Code, e.ShortName as EName,D.name as Designation, CONVERT(VARCHAR,cast(b.revision_of_date_change as date),105) AS IncDate  from pr_emp_biological_field b" +
                " join Employees e On b.emp_code = e.EmpId" +
                " join Designations D on D.id = e.CurrentDesignation " +
                "join pr_emp_inc_anual_stag A on b.emp_code = A.emp_code and b.fm=a.fm " +
                " where b.revision_of_date_change BETWEEN '" + Fdate.ToString("yyyy-MM-dd") + "' AND '" + Tdate.ToString("yyyy-MM-dd") + "'  and a.active=1; ";

            return await _sha.Get_Table_FromQry(qrySel);
           
        }

        public async Task<DataTable> searchIncrementReportForPOC(string SearchDatafrom, string SearchDatato)
        {
            int fy = _LoginCredential.FY;
            DateTime startFy = Convert.ToDateTime(fy - 1 + "-04-01");
            DateTime endFy = Convert.ToDateTime(fy + "-03-30");
            string yearString = "";
            string qrySel = "";
            qrySel = "SELECT emp_code AS Code,e.ShortName AS EName,des.Name AS Designation,CONVERT(VARCHAR, increment_date, 105) AS IncDate " +
                "FROM pr_emp_inc_anual_stag inc " +
                "JOIN employees e ON e.empid = inc.emp_code " +
                "JOIN Designations des ON e.CurrentDesignation = des.Id AND inc.active = 1 " +
                "WHERE increment_date BETWEEN ";
            string actualString = "";

            if (SearchDatafrom.Contains("^"))
            {
                SearchDatafrom = "01-01-00";
                SearchDatato = "01-01-00";
                actualString = "SELECT emp_code AS Code,e.ShortName AS EName,des.Name AS Designation,CONVERT(VARCHAR, increment_date, 105) AS IncDate " +
                "FROM pr_emp_inc_anual_stag inc " +
                "JOIN employees e ON e.empid = inc.emp_code " +
                "JOIN Designations des ON e.CurrentDesignation = des.Id AND inc.active = 1 " +
                "WHERE increment_date BETWEEN '" + SearchDatafrom + "' AND '" + SearchDatato + "';";
            }
            else
            {
                actualString = Helper.getDetesForFromDateToDateWithoutJoin(fy, SearchDatafrom, SearchDatato, qrySel,"pr_emp_inc_anual_stag", "old_pr_emp_inc_anual_stag");
            }
            //if(yearString.Contains("New") && !yearString.Contains("Old")) {
            //    string[] strNew = yearString.Split('#');
            //    string startYear = strNew[1];
            //    string endYear = strNew[2];
            //    qrySel = "SELECT emp_code AS Code,e.ShortName AS EName,des.Name AS Designation,CONVERT(VARCHAR, increment_date, 105) AS IncDate " +
            //    "FROM pr_emp_inc_anual_stag inc " +
            //    "JOIN employees e ON e.empid = inc.emp_code " +
            //    "JOIN Designations des ON e.CurrentDesignation = des.Id AND inc.active = 1 " +
            //    "WHERE increment_date BETWEEN '" + startYear + "' AND '" + endYear + "';";

            //} else if(yearString.Contains("Old") && !yearString.Contains("New"))
            //{
            //    string[] strNew = yearString.Split('#');
            //    string startYear = strNew[1];
            //    string endYear = strNew[2];
            //    qrySel = "SELECT emp_code AS Code,e.ShortName AS EName,des.Name AS Designation,CONVERT(VARCHAR, increment_date, 105) AS IncDate " +
            //    "FROM old_pr_emp_inc_anual_stag inc " +
            //    "JOIN employees e ON e.empid = inc.emp_code " +
            //    "JOIN Designations des ON e.CurrentDesignation = des.Id AND inc.active = 1 " +
            //    "WHERE increment_date BETWEEN '" + startYear + "' AND '" + endYear + "';";
            //} else if(yearString.Contains("Old") && yearString.Contains("New"))
            //{
            //    string[] strNew = yearString.Split('#');
            //    string startYearold = strNew[1];
            //    string endYearold = strNew[2];
            //    string startYearnew = strNew[4];
            //    string endYearnew = strNew[5];
            //    qrySel = "SELECT emp_code AS Code,e.ShortName AS EName,des.Name AS Designation,CONVERT(VARCHAR, increment_date, 105) AS IncDate " +
            //   "FROM old_pr_emp_inc_anual_stag inc " +
            //   "JOIN employees e ON e.empid = inc.emp_code " +
            //   "JOIN Designations des ON e.CurrentDesignation = des.Id AND inc.active = 1 " +
            //   "WHERE increment_date BETWEEN '" + startYearold + "' AND '" + endYearold + "' union all " +
            //   "SELECT emp_code AS Code,e.ShortName AS EName,des.Name AS Designation,CONVERT(VARCHAR, increment_date, 105) AS IncDate " +
            //   "FROM pr_emp_inc_anual_stag inc " +
            //   "JOIN employees e ON e.empid = inc.emp_code " +
            //   "JOIN Designations des ON e.CurrentDesignation = des.Id AND inc.active = 1 " +
            //   "WHERE increment_date BETWEEN '" + startYearnew + "' AND '" + endYearnew + "';";

            //}
            
           
            return await _sha.Get_Table_FromQry(actualString);

        }
        public async Task<DataTable> searchIncrementReportForPOCForJoin(string SearchDatafrom, string SearchDatato)
        {
            int fy = _LoginCredential.FY;
            DateTime startFy = Convert.ToDateTime(fy - 1 + "-04-01");
            DateTime endFy = Convert.ToDateTime(fy + "-03-30");
            string yearString = "";
            string qrySel = "";
            qrySel = "SELECT emp_code AS Code,e.ShortName AS EName,des.Name AS Designation,CONVERT(VARCHAR, increment_date, 105) AS IncDate " +
                "FROM pr_emp_inc_anual_stag inc " +
                "JOIN employees e ON e.empid = inc.emp_code " +
                "JOIN Designations des ON e.CurrentDesignation = des.Id AND inc.active = 1 " +
                "WHERE increment_date BETWEEN ";
            string actualString = "";

            if (SearchDatafrom.Contains("^"))
            {
                SearchDatafrom = "01-01-00";
                SearchDatato = "01-01-00";

                actualString = "SELECT emp_code AS Code,e.ShortName AS EName,des.Name AS Designation,CONVERT(VARCHAR, increment_date, 105) AS IncDate " +
                "FROM pr_emp_inc_anual_stag inc " +
                "JOIN employees e ON e.empid = inc.emp_code " +
                "JOIN Designations des ON e.CurrentDesignation = des.Id AND inc.active = 1 " +
                "WHERE increment_date BETWEEN '" + SearchDatafrom + "' AND '" + SearchDatato + "';";
            }
            else
            {
                string[] strArry = { "pr_emp_inc_anual_stag", "Designations"};

                actualString = Helper.getDetesForFromDateToDateWithJoins(fy, SearchDatafrom, 
                    SearchDatato, qrySel, strArry);
            }
            
            return await _sha.Get_Table_FromQry(actualString);

        }
    }
}
