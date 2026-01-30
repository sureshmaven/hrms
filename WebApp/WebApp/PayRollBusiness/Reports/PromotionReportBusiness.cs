using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Reports
{
    public class PromotionReportBusiness : BusinessBase
    {
        public PromotionReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        public async Task<DataTable> PromotionReportData(string SearchDatafrom, string SearchDatato)
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

            string qrySel = " select distinct pro.fm,pro.emp_id,pro.emp_code, e.ShortName,d.name as old_desig,pro.category new_Desig, pyf.amount , " +
                "pro.basic_pay_fixed, CONVERT(VARCHAR, pro.promotion_date, 105) AS promotion_date, CONVERT(VARCHAR, pro.incre_due_date, 105) AS Effective_date" +
                " from pr_emp_promotion pro join Employees e on e.empid = pro.emp_code " +
                "join Designations d on d.id = e.CurrentDesignation join pr_emp_pay_field pyf on pyf.emp_code = e.empid " +
                "where pyf.active = 1 AND PRO.authorisation=1 AND pro.promotion_date BETWEEN '" + Fdate.ToString("yyyy-MM-dd") + "' AND '" + Tdate.ToString("yyyy-MM-dd") + "';";

            return await _sha.Get_Table_FromQry(qrySel);
        }
    }
}
