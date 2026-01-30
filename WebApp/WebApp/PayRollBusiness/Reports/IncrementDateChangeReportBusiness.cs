using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.DAL.Business;
using PayrollModels;
using System.Data;
namespace PayRollBusiness.Reports
{
    public class IncrementDateChangeReportBusiness:BusinessBase
    {
        public IncrementDateChangeReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
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

            //string qrySel = "select ias.emp_code,e.shortname,d.Code as Desg," +
            //    "ias.basic_amount,convert(varchar,idc.inc_date,105)as increment_date," +
            //    "ias.increment_amount, convert(varchar,ias.increment_date,105)as inc_date_change  " +
            //    "from pr_emp_inc_anual_stag ias " +
            //    "join Employees e on e.EmpId = ias.emp_code " +
            //    "join Designations d on d.id=e.CurrentDesignation  " +
            //    "join pr_emp_inc_date_change idc on idc.emp_code=ias.emp_code " +
            //    "WHERE increment_date BETWEEN '" + Fdate.ToString("yyyy-MM-dd") + "'" +
            //    " AND '" + Tdate.ToString("yyyy-MM-dd") + "' and idc.authorisation=1 and idc.active=1;";

            string qrySel = "select ias.emp_code,e.shortname,d.Code as Desg," +
              "concat(cast(ias.basic_amount as nvarchar), '.00') as basic_amount," +
              "convert(varchar,idc.inc_date,105)as increment_date," +
              "concat(cast(ias.increment_amount as nvarchar), '.00') as increment_amount" +
              ", convert(varchar,ias.increment_date,105)as inc_date_change  " +
              "from pr_emp_inc_anual_stag ias " +
              "join Employees e on e.EmpId = ias.emp_code " +
              "join Designations d on d.id=e.CurrentDesignation  " +
              "join pr_emp_inc_date_change idc on idc.emp_code=ias.emp_code " +
              "WHERE increment_date BETWEEN '" + Fdate.ToString("yyyy-MM-dd") + "'" +
              " AND '" + Tdate.ToString("yyyy-MM-dd") + "' and idc.authorisation=1 and idc.active=1 and ias.active=1;";


            return await _sha.Get_Table_FromQry(qrySel);

        }


    
    }
}
