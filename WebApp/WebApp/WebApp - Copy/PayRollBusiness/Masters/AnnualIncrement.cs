using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
namespace PayRollBusiness.Masters
{
    public class AnnualIncrement : BusinessBase
    {
        public AnnualIncrement(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        public async Task<IList<GetIncrementModel>> getDecCategories()
        {
            string query = "select id,designation from pr_emp_designation where active = 1";

            DataTable dt = await _sha.Get_Table_FromQry(query);
            IList<GetIncrementModel> lstDept = new List<GetIncrementModel>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new GetIncrementModel
                    {
                        Id = dr["id"].ToString(),
                        category = dr["designation"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }
        public async Task<IList<GetIncrementTable>> GetAnullaDataByDes(string DesId, string empAnual)
        {
           string query = "select basic,increment, stages From pr_basic_anual_incr_master where designation_id " +
                "=" + Convert.ToInt32(DesId) + " and pay_period ='" + Convert.ToString(empAnual) +"';";

            DataTable dt = await _sha.Get_Table_FromQry(query);
            IList<GetIncrementTable> lstDept = new List<GetIncrementTable>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new GetIncrementTable
                    {
                        Basic = dr["basic"].ToString(),
                       Increment = dr["increment"].ToString(),
                        Stages = dr["stages"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }
      
        public async Task<IList<payPeriod>> GetPayPeriodForAnnualIncr()
        {
            string qrySel = "select distinct Pay_period from  pr_basic_anual_incr_master";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);

            IList<payPeriod> lstDept = new List<payPeriod>();
            foreach (DataRow dr in dt.Rows)
            {
                //string date =Convert.ToDateTime(dr["Pay_period"]).ToString("yyyy-MM-dd");
                lstDept.Add(new payPeriod
                {
               
                    date = Convert.ToDateTime(dr["Pay_period"]).ToString("yyyy-MM-dd")

            });
            }
            return lstDept;
        }

        public async Task<IList<payPeriod>> GetPayPeriodForStagnationIncr()
        {
            string qrySel = "select  distinct Pay_period from  pr_basic_stag_incr_master";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);

            IList<payPeriod> lstDept = new List<payPeriod>();
            foreach (DataRow dr in dt.Rows)
            {
                //string date =Convert.ToDateTime(dr["Pay_period"]).ToString("yyyy-MM-dd");
                lstDept.Add(new payPeriod
                {

                    date = Convert.ToDateTime(dr["Pay_period"]).ToString("yyyy-MM-dd")

                });
            }
            return lstDept;
        }
        public async Task<IList<GetStagIncrementTable>> GetStagnationDataByDes(string DesId, string empAnual)
        {
            string query = "select basic,increment, stages,incr_type From pr_basic_stag_incr_master where designation_id " +
                 "=" + Convert.ToInt32(DesId) + " and pay_period ='" + Convert.ToString(empAnual) + "';";

            DataTable dt = await _sha.Get_Table_FromQry(query);
            IList<GetStagIncrementTable> lstDept = new List<GetStagIncrementTable>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new GetStagIncrementTable
                    {
                        Basic = dr["basic"].ToString(),
                        Increment = dr["increment"].ToString(),
                        Stages = dr["stages"].ToString(),
                        Type = dr["incr_type"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }

    }
}
