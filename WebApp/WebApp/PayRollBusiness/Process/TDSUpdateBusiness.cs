using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayRollBusiness.PayrollService;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Process
{
    public class TDSUpdateBusiness : BusinessBase
    {
        public TDSUpdateBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();

        public async Task<IList<getUpdate>> getDataToUpdate(string empCode)
        {
            int iFY = _LoginCredential.FY;
            int dtFM = _LoginCredential.FM;
            dtFM = dtFM - 1;
            string query = "select format(p.fm,  'dd-MM-yyyy') as fm,empcode as Id,(CASE WHEN ps.dd_income_tax >0 THEN ps.dd_income_tax ELSE 0 END) as tax_deducted_at_source,tds_per_month," +
                " CONCAT(e.FirstName,' ',e.LastName) as Name,(CASE WHEN " +
                " ABS(ps.dd_income_tax - p.tds_per_month) > 0 THEN ABS(ps.dd_income_tax-p.tds_per_month) ELSE 0 END) " +
                " as diff from pr_emp_tds_process p left outer join Employees e on p.empcode=e.EmpId left outer " +
                " join pr_emp_payslip ps on ps.emp_code=p.empcode and ps.spl_type in ('Regular', 'StopSalary','Suspended') and ps.fy=" + iFY + " and Month(ps.fm)=" + dtFM + "" +
                " where p.active =1 and p.tds_update=0 and p.sal_basic>0";



            DataTable dt = await _sha.Get_Table_FromQry(query);

            IList<getUpdate> lstDept = new List<getUpdate>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new getUpdate
                    {
                        Id = dr["Id"].ToString(),
                        Name = dr["Name"].ToString(),
                        DOL = dr["fm"].ToString().ToString(),
                        tax_deducted_at_source = dr["tax_deducted_at_source"].ToString(),
                        tds_per_month = dr["tds_per_month"].ToString(),
                        diff = dr["diff"].ToString()

                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;

        }

        
        public async Task<string> updateTDs(TDSUpdate values)
        {
            var update = values.update;
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            int NewNumIndex = 0;
            foreach (var emp in update)
            {
                    NewNumIndex++;
                    string tdsUpdate = "update pr_emp_tds_process set tds_update=1  where  empcode=" + Convert.ToInt32(emp.id) + " ;";
                    sbqry.Append(tdsUpdate);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_tds_process", emp.id.ToString() + NewNumIndex, ""));
                    NewNumIndex++;
                    string empUpdate = "update pr_emp_pay_field set amount=" + emp.amount + " where emp_code=" + Convert.ToInt32(emp.id) + " and m_id=(select id from pr_earn_field_master where name='Employee Tds' and type='pay_fields')";
                    sbqry.Append(empUpdate);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", emp.id.ToString() + NewNumIndex, ""));
            }
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#TDS Update#TDS Updated Successfully..!!";
            }
            else
            {
                return "E#TDS Update#Error While TDS Updation..!!";
            }
            
        }
    }
}
