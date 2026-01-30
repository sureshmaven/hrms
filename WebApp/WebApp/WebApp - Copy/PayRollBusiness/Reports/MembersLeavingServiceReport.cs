using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using Mavensoft.DAL.Business;
using System.Data;
using PayrollModels;
using System.Threading.Tasks;

namespace PayRollBusiness.Reports
{
   public class MembersLeavingServiceReport : BusinessBase
    {
        public MembersLeavingServiceReport(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public async Task<DataTable> getLeavingEmployees(string month)
        {
            DateTime str = Convert.ToDateTime(month);
            string str1 = str.ToString("yyyy-MM-dd");
            //string qrysql = "SELECT '' as Remarks, (case when e.RelievingReason IS NULL then 'Retiring' else e.RelievingReason end) AS RelievingReason,g.emp_code AS Emp_Code, g.uan_no AS Account_No,e.ShortName AS Name_of_Member,e.FatherName AS Father_or_Husband_Name,format(e.RetirementDate,'dd-MM-yyyy') AS Date_of_Leaving  FROM pr_emp_general AS g JOIN Employees AS e ON g.emp_code = e.EmpId AND Month(e.RetirementDate)=Month('" + str1 + "') AND  year(e.RetirementDate)=year('" + str1 + "')";
            string qrysql = "SELECT e.EmpId AS Emp_Code, e.ShortName AS Name_of_Member,e.FatherName AS Father_or_Husband_Name,pr.uan_no AS Account_No,format(e.RetirementDate, 'dd-MM-yyyy') AS Date_of_Leaving,(case when e.RelievingReason IS NULL then 'Retiring' else e.RelievingReason end) AS RelievingReason,'' as Remarks from employees e left join pr_emp_general pr on e.empid = pr.emp_code  and pr.active=1  Where Month(e.RetirementDate) = Month('" + str1 + "') AND year(e.RetirementDate)= year('" + str1 + "')"; 
            if (month =="01-01-01") {
                
               // return await _sha.Get_Table_FromQry(qrysql);
            }
            
                       return await _sha.Get_Table_FromQry(qrysql);
            
        }

    }
}
