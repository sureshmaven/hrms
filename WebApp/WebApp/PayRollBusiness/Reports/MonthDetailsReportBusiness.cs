using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.Common;
using Mavensoft.DAL.Business;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.Configuration;
using PayRollBusiness;
using PayrollModels;
using System.Data;
namespace PayRollBusiness.Reports
{
   public class MonthDetailsReportBusiness: BusinessBase
    {
        public MonthDetailsReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        Helper helper = new Helper();

        public async Task<DataTable> MonthDetailsReportData(string month)
        {

            int Eyears = 0001;
            int Fyears = 0001;
           if (month.Contains("^"))
            {
               
                Fyears = 0001;
                Eyears = 0001;
            }
            else
            {
                Eyears = Int32.Parse(month);
                Fyears = Int32.Parse(month) - 1;
            }
            string qrySel = "select fy,fm as fmonth,format(fm,'MMMM-yyyy') as fm,da_slabs,da_points,da_percent," +
                "is_interest_calculated,interest_percent from pr_month_details " +
                "where fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) order by fmonth desc ";

            return await _sha.Get_Table_FromQry(qrySel);
        }


        public async Task<IList<LICReport>> getFyforMonthdetailsReport()
        {
            string qryfy = "select year(fm) as fm_fy from pr_month_details where active=1;";
            int fm_fy = await _sha.Run_INS_ExecuteScalar(qryfy);

            IList<LICReport> fyear = new List<LICReport>();

            int fy = fm_fy + 1;
            int Id = 0;

            fyear.Add(new LICReport
            {
                Id = Id.ToString(),
                fY = "Select",

            });

            Id++;
            fyear.Add(new LICReport
            {

                Id = Id.ToString(),
                fY = (fm_fy + "-" + (fy)).ToString(),

            });

            for (int i = 1; i < 10; i++)
            {
                Id++;
                fm_fy--;
                fy--;
                fyear.Add(new LICReport
                {
                    Id = Id.ToString(),
                    fY = (fm_fy + "-" + (fy)).ToString(),

                });
            }

            return fyear;

        }
    }
}
