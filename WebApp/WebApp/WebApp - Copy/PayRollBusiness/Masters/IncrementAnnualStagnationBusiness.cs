using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Masters
{
   public class IncrementAnnualStagnationBusiness : BusinessBase
    {
        public IncrementAnnualStagnationBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        public async Task<IList<GetIncrementModel>> getIncAnnualStag()
        {

            string qrySel = "SELECT emp_code,e.ShortName,des.Name as desi_mid,e.DOJ as DOJ,basic_amount,increment_amount,increment_type ,increment_date ,inc.stages " +
                "FROM pr_emp_inc_anual_stag inc join employees e on e.empid = inc.emp_code " +
                "JOIN Designations des on e.CurrentDesignation = des.Id AND inc.active = 1 AND inc.process = 0 and inc.authorisation = 0 " +
                "WHERE(SELECT month(fm) FROM pr_month_details WHERE active = 1) = month(inc.increment_date) " +
                " AND e.RetirementDate>=(select fm from  pr_month_details where active=1);";
                
                 //"select emp_code,e.ShortName,des.Name as desi_mid,basic_amount,increment_amount,increment_type ,increment_date ,inc.stages  " +
                //"from pr_emp_inc_anual_stag inc join employees e on e.empid = inc.emp_code  " +
                //"join Designations des on e.CurrentDesignation = des.Id and inc.active = 1 and inc.process = 0 and inc.authorisation=0 " +
                ////"and inc.authorisation=0 " + 
                //"where month(increment_date)= MONTH(getdate()) ";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<GetIncrementModel> lstDept = new List<GetIncrementModel>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new GetIncrementModel
                    {
                        Id = dr["emp_code"].ToString(),
                        Name = dr["ShortName"].ToString(),
                        Basic = dr["basic_amount"].ToString().ToString(),
                        Increment = dr["increment_amount"].ToString(),
                        increment_type = dr["increment_type"].ToString(),
                        increment_date = Convert.ToDateTime(dr["increment_date"]).ToString("dd/MM/yyyy"),
                         desi_mid = dr["desi_mid"].ToString(),
                        DOJ = Convert.ToDateTime(dr["DOJ"]).ToString("dd/MM/yyyy")

                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }
        
        public async Task<string> UpdateIncAnnualStag(List<string> Values)
        {
            string qry="";
            string retMessage = "";
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            try
            {
                foreach (string Id in Values)
                {
                    qry = "update pr_emp_inc_anual_stag set process = 1 where emp_code = '" + Id + "' and active = 1 and process = 0";
                    sbqry.Append(qry);

                    qry = "update pr_emp_inc_date_change set process = 1 where emp_code = '" + Id + "' and active = 1 and process = 0";
                    sbqry.Append(qry);

                    qry = "update pr_emp_inc_date_change set authorisation = 1 where emp_code = '" + Id + "' and active = 1 ";
                    sbqry.Append(qry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_inc_anual_stag", Id.ToString(), ""));

                }
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                  retMessage = "I#IncrementAnnualStagnation # Data Updated Successfully ..!!";
                }
               
            }
            catch(Exception e)
            {
                string msg = e.Message;
                return "E#Error:#" + msg;
            }
            return retMessage;
        }
    }
}
