using System;
using System.Collections.Generic;
using Mavensoft.DAL.Business;
using System.Text;
using System.Threading.Tasks;
using PayrollModels;
using System.Data;
using System.Linq;

namespace PayRollBusiness.Reports
{
    public class Form16ReportBusiness : BusinessBase
    {

        public Form16ReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        //drop down financial year eg:2019-2020
        public async Task<IList<LICReport>> getFy()
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

            for (int i = 1; i < 6; i++)
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
