using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Mavensoft.DAL.Db;
using PayRollBusiness.PayrollService;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Process
{
    public class PayslipBusiness : BusinessBase
    {
        public PayslipBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        string Qry_GetActive_Employee_GeneralAdhocEncashment = "";
        string amount = "";
        string status = "";
        string lop_days = "";
        string absent_days = "";
        string working_days = "";
        string getallrecords1 = "";
        string getallrecords12 = "";
        string getallrecords = "";
        string sNoAttNoBasic = "";
        string NetAmountlessthanzero = "";
        //this method is used to check the final process
        public async Task<string> getFinalProcess(string EmpId, string Pay_Type)
        {
            string final_process = "";
            try
            {
                string qrySel = "";

                if (EmpId != "")
                {
                    int iFY = _LoginCredential.FY;
                    int dtFM = _LoginCredential.FM;
                    DateTime Financial_md = (_LoginCredential.FinancialMonthDate);

                    //string emp_codes = "";
                    //foreach (string dr in EmpId.Split(','))
                    //{
                    //    emp_codes += "'" + dr + "'" + ",";
                    //}
                    //emp_codes = emp_codes.Remove(emp_codes.Length - 1, 1);
                    if (Pay_Type.Contains("Regular") && Pay_Type.Contains("Adhoc") && Pay_Type.Contains("Encashment"))
                    {
                        Pay_Type = "'Regular','Encashment','Adhoc'";
                    }
                    else if (Pay_Type.Contains("Suspended") && Pay_Type.Contains("Adhoc") && Pay_Type.Contains("Encashment"))
                    {
                        Pay_Type = "'Suspended','Encashment','Adhoc'";
                    }
                    else if (Pay_Type.Contains("Regular") && Pay_Type.Contains("Encashment"))
                    {
                        Pay_Type = "'Regular','Encashment'";
                    }
                    else if (Pay_Type.Contains("Suspended") && Pay_Type.Contains("Encashment"))
                    {
                        Pay_Type = "'Suspended','Encashment'";
                    }
                    else if (Pay_Type.Contains("Regular") && Pay_Type.Contains("Adhoc"))
                    {
                        Pay_Type = "'Regular','Adhoc'";
                    }
                    else if (Pay_Type.Contains("Suspended") && Pay_Type.Contains("Adhoc"))
                    {
                        Pay_Type = "'Suspended','Adhoc'";
                    }
                    else if (Pay_Type.Contains("Encashment") && Pay_Type.Contains("Adhoc"))
                    {
                        Pay_Type = "'Encashment','Adhoc'";
                    }
                    else if (Pay_Type.Contains("Regular"))
                    {
                        Pay_Type = "'Regular'";
                    }
                    else if (Pay_Type.Contains("Suspended"))
                    {
                        Pay_Type = "'Suspended'";
                    }
                    else if (Pay_Type.Contains("Adhoc"))
                    {
                        Pay_Type = "'Adhoc'";
                    }
                    else if (Pay_Type.Contains("Encashment"))
                    {
                        Pay_Type = "'Encashment'";
                    }
                    else if (Pay_Type.Contains("stopsalary"))
                    {
                        Pay_Type = "'StopSalary'";
                    }
                    foreach (var id in EmpId.Split(','))
                    {
                        DataTable getidfrompayslip = await _sha.Get_Table_FromQry("select id,final_process from pr_emp_payslip where emp_code=" + id + " and spl_type in (" + Pay_Type + ") and month(fm)=" + dtFM + " and year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and active=1;");

                        if (getidfrompayslip.Rows.Count > 0)
                        {
                            DataRow pid = getidfrompayslip.Rows[0];

                            final_process = pid["final_process"].ToString();
                        }
                    }
                    if (final_process != "True")
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return "E#Error:#" + msg;
            }
            return final_process;
        }
        //this method is use to show the payslip ui,grid data
        public async Task<DataTable> getPayslipservice()
        {
            string qrySel = "select id,payslip_type,username, convert(varchar(20),run_date_time,105)+' '+convert(varchar(20),convert(time,run_date_time),100) as run_date_time,status,emp_codes,CompletedEmpCodes,concat(total_count,'/',process_count) as process_count ,err_desc from pr_payroll_service_run  order By id desc, run_date_time DESC ";
            return await _sha.Get_Table_FromQry(qrySel);
        }
        public async Task<DataTable> empcodeslist()
        {
            string SQry = "SELECT EmpId , ma.status FROM Employees e join pr_month_attendance ma on ma.emp_code=e.EmpId " +
                    "WHERE ma.status = 'StopSalary' AND ma.active=1 AND RetirementDate >= GETDATE(); ";
            return await _sha.Get_Table_FromQry(SQry);
        }
        public async Task<DataTable> empcodeslistGS(string p_type)
        {
            int iFY = _LoginCredential.FY;
            int dtFM = _LoginCredential.FM;
            DateTime Financial_md = (_LoginCredential.FinancialMonthDate);
            string SQry = "";
            if (p_type.Contains("Regular") && p_type.Contains("Encashment") && p_type.Contains("Adhoc"))
            {
                SQry = "select distinct * from (select emp_code from pr_month_attendance att " +
                    "join Employees e on e.empid=att.emp_code where active = 1  and status in ('Regular') " +
                    "and e.retirementdate>=(select fm from pr_month_details where active=1) " +
                    "union all select emp_code from pr_emp_adhoc_det_field where active = 1 " +
                    "union all select em.EmpId as emp_code from PLE_Type en " +
                    "join Employees em on en.empid = em.id where  en.authorisation = 1 and en.process = 1 and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + ") as p_type order by p_type.emp_code; ";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Encashment") && p_type.Contains("Adhoc"))
            {
                SQry = "select distinct * from (select emp_code from pr_month_attendance att " +
                    "join Employees e on e.empid=att.emp_code   where active = 1  and status in ('Suspended')  " +
                    "and e.retirementdate>=(select fm from pr_month_details where active=1) " +
                    "union all select emp_code from pr_emp_adhoc_det_field where active = 1 " +
                    "union all select em.EmpId as emp_code from PLE_Type en " +
                    "join Employees em on en.empid = em.id where  en.authorisation = 1 and en.process = 1 and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + ") as p_type order by p_type.emp_code; ";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc"))
            {
                SQry = "select distinct * from (select distinct emp_code from pr_month_attendance att " +
                    "join Employees e on e.empid = att.emp_code where active = 1 " +
                    " and status in ('Suspended') and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " " +
                    "and fy=" + iFY + "  and e.retirementdate>=(select fm from pr_month_details where active=1) " +
                    "union all select emp_code from pr_emp_adhoc_det_field where active = 1 and Month(fm)=" + Financial_md.Month + " " +
                    "and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + ") as p_type order by p_type.emp_code  ;";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Adhoc"))
            {
                SQry = "select distinct * from (select distinct emp_code from pr_month_attendance att join Employees e on e.empid = att.emp_code  where active = 1  " +
                    "and status in ('Regular') and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + "  " +
                    "and e.retirementdate>=(select fm from pr_month_details where active=1) and emp_code not in (381,686) " +
                    "union all select emp_code from pr_emp_adhoc_det_field where active = 1 and Month(fm)=" + Financial_md.Month + " " +
                    "and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + ") as p_type " +
                    "order by p_type.emp_code  ;";
            }
            else if (p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                SQry = "select distinct * from ( select emp_code from pr_emp_adhoc_det_field where active = 1 " +
                    "union all select em.EmpId as emp_code from PLE_Type en join Employees em on en.empid = em.id  " +
                    "where en.authorisation = 1 and en.process = 1 and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + ") as p_type order by p_type.emp_code; ";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Encashment"))
            {
                SQry = " select distinct * from (select emp_code from pr_month_attendance att join " +
                    "Employees e on e.empid=att.emp_code  where active = 1  and status in ('Regular')  " +
                    "and e.retirementdate>=(select fm from pr_month_details where active=1) and emp_code not in (381,686) " +
                    "union all select em.EmpId as emp_code from PLE_Type en join Employees em on en.empid = em.id " +
                    "where en.authorisation = 1 and en.process = 1 and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + ") as p_type order by p_type.emp_code ; ";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Encashment"))
            {
                SQry = " select distinct * from (select emp_code from pr_month_attendance " +
                    " att join Employees e on e.empid=att.emp_code " +
                    "where active = 1  and status in ('Suspended')  and e.retirementdate>=(select fm from pr_month_details where active=1) " +
                    "union all select em.EmpId as emp_code from PLE_Type en join Employees em on en.empid = em.id " +
                    "where en.authorisation = 1 and en.process = 1 and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + ") as p_type order by p_type.emp_code ; ";
            }
            else if (p_type.Contains("Regular"))
            {
                //General,Suspended
                SQry = " select distinct emp_code from pr_month_attendance att join Employees e on e.empid=att.emp_code  where active = 1  and status in ('Regular') " +
                    "and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " " +
                    " and e.retirementdate>=(select fm from pr_month_details where active=1) and emp_code not in (381,686) order by emp_code ;";
            } 
            else if (p_type.Contains("Suspended"))
            {
                //General,Suspended
                SQry = " select distinct emp_code from pr_month_attendance  where active = 1  and status in ('Suspended') and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " order by emp_code ;";
            }
            //Adhoc
            else if (p_type.Contains("Adhoc"))
            {
                SQry = " select distinct pa.emp_code from pr_emp_adhoc_det_field pa join Employees e on pa.emp_code=e.empid where active = 1 and Month(fm)=" + Financial_md.Month + " " +
                    "and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " " +
                    " and e.retirementdate>=(select fm from pr_month_details where active=1) and pa.emp_code not in (381,686) order by emp_code; ";
            }
            //Encash
            else if (p_type.Contains("Encashment"))
            {
                //SQry = "  select em.EmpId as emp_code from PLE_Type en join Employees em on en.empid = em.id where en.authorisation = 1 and en.process = 1  order by emp_code; ";
                SQry = "select distinct em.EmpId as emp_code from PLE_Type en join Employees em on en.empid=em.id " +
                    "where Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and em.retirementdate>=(select fm from pr_month_details where active=1) and em.empid not in(381,686) " +
                    "and  en.authorisation=1 and en.process=1  ";
            }

            return await _sha.Get_Table_FromQry(SQry);
        }
        public async Task<DataTable> empcodes(string EmpCode)
        {
            DateTime Financial_md = (_LoginCredential.FinancialMonthDate);
            string fm = Financial_md.ToString("yyyy-MM-dd");
            int dtFM = _LoginCredential.FM;
            string emp_codes = "";
            string SQry = "";
            foreach (string dr in EmpCode.Split(','))
            {
                emp_codes += "'" + dr + "'" + "," + " ";
            }
            emp_codes = emp_codes.Remove(emp_codes.Length - 2, 1);

            SQry = "SELECT distinct EmpId FROM Employees WHERE EmpId in (" + emp_codes + ") and dateadd(month,1,RetirementDate) >=('" + fm + "');";
            return await _sha.Get_Table_FromQry(SQry);
        }
        //this method is used to get the employee codes having attendance
        private string GetActiveEmpCodes_GeneralAdhocEncashment(string[] result, string p_type)
        {
            string[] test = result; // Alternative array creation syntax 
            string empcodes = String.Join(" ", test);


            DateTime Financial_md = (_LoginCredential.FinancialMonthDate);
            int iFY = _LoginCredential.FY;
            int dtFM = _LoginCredential.FM;

            if (p_type.Contains("Regular") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords = "select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Regular')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                    " union all select distinct ded.emp_code, ded.fm, ded.fy, 'Adhoc' as status, atted.status as stpstatus from pr_emp_adhoc_det_field ded " +
                    " join pr_month_attendance atted on atted.emp_code = ded.emp_code " +
                    " where ded.emp_code in (" + empcodes + ") and ded.active = 1 and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + " " +
                    " union all" +
                    " select distinct em.EmpId as emp_code, en.fm, en.fy, 'Encashment' as status, atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance " +
                    " atted on atted.emp_code = em.EmpId  where em.empid in (" + empcodes + ") and   " +
                    "   en.authorisation = 1 and en.PLEncash > 0 and en.process = 1  and Year(atted.fm)= " + Financial_md.Year + " and Year(en.fm)= " + Financial_md.Year + " " +
                    " and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended')) as x order by x.emp_code;";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords = "select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Suspended')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                    " union all select distinct ded.emp_code, ded.fm, ded.fy, 'Adhoc' as status, atted.status as stpstatus from pr_emp_adhoc_det_field ded " +
                    " join pr_month_attendance atted on atted.emp_code = ded.emp_code " +
                    " where ded.emp_code in (" + empcodes + ") and ded.active = 1 and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + " " +
                    " union all" +
                    " select distinct em.EmpId as emp_code, en.fm, en.fy, 'Encashment' as status, atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance " +
                    " atted on atted.emp_code = em.EmpId  where em.empid in (" + empcodes + ") and   " +
                    "   en.authorisation = 1 and en.PLEncash > 0 and en.process = 1 and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and Year(en.fm)= " + Financial_md.Year + " " +
                    " and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended')) as x order by x.emp_code;";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Adhoc"))
            {
                getallrecords = "select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Regular')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                    " union all select distinct ded.emp_code, ded.fm, ded.fy, 'Adhoc' as status, atted.status as stpstatus from pr_emp_adhoc_det_field ded " +
                    " join pr_month_attendance atted on atted.emp_code = ded.emp_code " +
                    " where ded.emp_code in (" + empcodes + ") and ded.active = 1 and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + ") as x order by x.emp_code ; ";

            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc"))
            {
                getallrecords = "select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Suspended')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                    " union all select distinct ded.emp_code, ded.fm, ded.fy, 'Adhoc' as status, atted.status as stpstatus from pr_emp_adhoc_det_field ded " +
                    " join pr_month_attendance atted on atted.emp_code = ded.emp_code " +
                    " where ded.emp_code in (" + empcodes + ") and ded.active = 1 and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + ") as x order by x.emp_code ; ";

            }
            else if (p_type.Contains("Regular") && p_type.Contains("Encashment"))
            {
                getallrecords = " select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Regular')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                   " union all" +
                    " select distinct em.EmpId as emp_code, en.fm, en.fy, 'Encashment' as status, atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance " +
                    " atted on atted.emp_code = em.EmpId  where em.empid in (" + empcodes + ") and   " +
                    "   en.authorisation = 1 and en.PLEncash > 0 and en.process = 1 and Year(en.fm)= " + Financial_md.Year + " " +
                    " and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended')) as x order by x.emp_code;";

            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Encashment"))
            {
                getallrecords = " select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Suspended')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                   " union all" +
                    " select distinct em.EmpId as emp_code, en.fm, en.fy, 'Encashment' as status, atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance " +
                    " atted on atted.emp_code = em.EmpId  where em.empid in (" + empcodes + ") and   " +
                    "   en.authorisation = 1 and en.PLEncash > 0 and en.process = 1 and Year(en.fm)= " + Financial_md.Year + "  " +
                    " and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended')) as x order by x.emp_code;";

            }
            else if (p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords = "select * from (select distinct ded.emp_code, ded.fm, ded.fy, 'Adhoc' as status, atted.status as stpstatus from pr_emp_adhoc_det_field ded " +
                    " join pr_month_attendance atted on atted.emp_code = ded.emp_code " +
                    " where ded.emp_code in (" + empcodes + ") and ded.active = 1 and atted.active = 1 and atted.status in ('Regular', 'StopSalary') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + " " +
                    " union all" +
                    " select distinct em.EmpId as emp_code, en.fm, en.fy, 'Encashment' as status, atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance " +
                    " atted on atted.emp_code = em.EmpId  where em.empid in (" + empcodes + ") and   " +
                    "  en.authorisation = 1 and en.PLEncash > 0 and en.process = 1  and Year(en.fm)= " + Financial_md.Year + " " +
                    " and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended')) as x order by x.emp_code;";
            }
            else if (p_type.Contains("Regular"))
            {
                getallrecords = "select distinct atted.emp_code,atted.fm,atted.fy,status,status as stpstatus from pr_month_attendance atted join pr_emp_pay_field c on atted.emp_code = c.emp_code JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id " +
            " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and working_days> 0 and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and " +
             " status in ('Regular')and c.emp_code in  (" + empcodes + ") and pc.field_type = 'pay_fields' and " +
             " m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount> 0 and m.name = 'Basic' order by emp_code; ";
            }
            else if (p_type.Contains("stopsalary"))
            {
                getallrecords = "select distinct atted.emp_code,atted.fm,atted.fy,status,status as stpstatus from pr_month_attendance atted join pr_emp_pay_field c on atted.emp_code = c.emp_code JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id " +
            " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and working_days> 0 and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and " +
             " status in ('stopsalary')and c.emp_code in  (" + empcodes + ") and pc.field_type = 'pay_fields' and " +
             " m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount> 0 and m.name = 'Basic' order by emp_code; ";
            }
            else if (p_type.Contains("Adhoc"))
            {
                getallrecords = "select distinct ded.emp_code,atted.status as stpstatus ,ded.fm, ded.fy, 'Adhoc' as status from pr_emp_adhoc_det_field ded join pr_month_attendance atted on  atted.emp_code=ded.emp_code " +
                "where ded.emp_code in (" + empcodes + ") and ded.active=1 and atted.active=1 and atted.status in ('Regular','StopSalary','Suspended') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Encashment"))
            {
                getallrecords = "select distinct em.EmpId as emp_code,en.fm, en.fy, 'Encashment' as status," +
                    " atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance atted on atted.emp_code = em.EmpId " +
                    " where em.empid in (" + empcodes + ") and " +
                    "   en.authorisation=1 and en.PLEncash>0 and en.process=1" +
                    " and Year(en.fm)=" + Financial_md.Year + "" +
                    " and atted.active=1 and atted.status in ('Regular','StopSalary','Suspended');";
            }
            else if (p_type.Contains("Suspended"))
            {
                getallrecords = "select distinct atted.emp_code,atted.fm,atted.fy,status,status as stpstatus from pr_month_attendance atted join pr_emp_pay_field c on atted.emp_code = c.emp_code JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id " +
            " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and working_days> 0 and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and " +
             " status in ('Suspended')and c.emp_code in  (" + empcodes + ") and pc.field_type = 'pay_fields' and " +
             " m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount> 0 and m.name = 'Basic' order by emp_code; ";
            }
            return getallrecords;
        }


        private async Task<int> getNewRunId(string date, string emp_codes, int count, string pay_type, string finalProcess)
        {
            string EmployeeName = _LoginCredential.EmpShortName;
            int iRet = 0;

            StringBuilder sbqry = new StringBuilder();

            string insertQry = "";
            bool updateQry = false;
            //StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());

            //2. gen new num
            sbqry.Append(GetNewNumString("pr_payroll_service_run"));
            if (date == "Now")
            {
                updateQry = await _sha.Run_UPDDEL_ExecuteNonQuery("UPDATE pr_payroll_service_run set active=0;");
                insertQry = "INSERT into pr_payroll_service_run(id,run_date_time,status,emp_codes,process_count,total_count,active,trans_id,payslip_type,final_process,username)" +
                "values(@idnew,GETDATE(), 0 ,'" + emp_codes + "','0'," + count + ",1,@transidnew,'" + pay_type + "','" + finalProcess + "' ,'" + EmployeeName + "'  );";
            }
            else
            {
                updateQry = await _sha.Run_UPDDEL_ExecuteNonQuery("UPDATE pr_payroll_service_run set active=0;");
                insertQry = "INSERT into pr_payroll_service_run(id,run_date_time,status,emp_codes,process_count,total_count,active,trans_id ,payslip_type,final_process,username)" +
                    "values(@idnew, CONVERT(datetime,'" + date + "', 105), 0 ,'" + emp_codes + "','0'," + count + ",1,@transidnew ,'" + pay_type + "' ,'" + finalProcess + "','" + EmployeeName + "'  );";
            }
            sbqry.Append(insertQry);

            //4. transaction touch
            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_payroll_service_run", "@idnew", ""));
            sbqry.Append("SELECT @idnew;");

            iRet = await _sha.Run_INS_ExecuteScalar(sbqry.ToString());

            return iRet;

        }


        //this method is use to process the payslip data-from this method we are navigating to the payslip
        public async Task<string> InsertPayslip(string date, string[] emp_id, string Pay_Type, string Finalprocess)
        {
            string Stopsalary_emp_code = " ";
            DataTable dtnetamount = null;
            DateTime Financial_md = (_LoginCredential.FinancialMonthDate);
            int iFY = _LoginCredential.FY;
            int dtFM = _LoginCredential.FM;
            string Allids;
            Allids = emp_id[0];
            string emp_codes = emp_id[0];
            string dtwd = await empidsamount(Allids);
            StringBuilder ATypes = new StringBuilder();
            string[] Types = emp_codes.Split(',');
            string payslip_status = "";
            string queryStopSalary = "";
            string payslip_status_stop = "";
            //string qrys = "";
            //string qrys1 = "";
            //string qrys2 = "";
            string sGenPayslipError = "";
            int gNewRunId = 0;

            //string myTableRow = " ";

            //string myTableRows = " ";
            //string founderMinus1 = "";
            //string myTableRow1 = " ";
            //string founderMinus11 = "";


            //string myTableRows1 = " ";

            //string NonbaEmp = "";

            foreach (string word in Types)
            {
                ATypes.Append(word);
                ATypes.Append("," + " ");
            }
            emp_codes = ATypes.ToString(0, ATypes.Length - 2);

            if (Finalprocess == "")
            {
                Finalprocess = "0";
            }
            //else if(sendmail == "")
            //{
            //    sendmail = "0";
            //}
            //string s = emp_codes;
            string[] arrEmps = emp_codes.Split(',');
            int empsCount = arrEmps.Length;

            //for (int i = 0; i < count.Length; i++)
            //{
            //    count[i] = count[i].Trim();
            //}
            try
            {
                //** get new run id initially ***
                gNewRunId = await getNewRunId(date, emp_codes, empsCount, Pay_Type, Finalprocess);

                string Qry_GetActive_Employee_GeneralAdhocEncashment = GetActiveEmpCodes_GeneralAdhocEncashment(emp_id, Pay_Type);
                //string nonEmpID = GetQryForNoAttNoBasic(emp_id, Pay_Type);

                DataTable dtGet_Employee_PSType_Fm = new DataTable();
                DataTable dt_stop_salary = new DataTable();
                //DataTable nonBaAtt = new DataTable();

                string[] test = emp_id; // Alternative array creation syntax 
                string empcodes = String.Join(" ", test);

                //if (Qry_GetActive_Employee_GeneralAdhocEncashment != "")
                //{
                DataTable dtNoAttNoBasic = await _sha.Get_Table_FromQry(GetQryForNoAttNoBasic(emp_id, Pay_Type));

                //}
                if (Qry_GetActive_Employee_GeneralAdhocEncashment != "")
                {
                    dtGet_Employee_PSType_Fm = await _sha.Get_Table_FromQry(Qry_GetActive_Employee_GeneralAdhocEncashment);

                }
                //** check emps attendance, basic and preparing an error statement. **
                if (dtNoAttNoBasic.Rows.Count > 0)
                {
                    //foreach (DataRow dtr in dtNoAttNoBasic.Rows)
                    //{
                    //    sNoAttNoBasic += dtr["empid"].ToString() + " - no " + dtr["type"].ToString() + ",";
                    //    //myTableRows = myTableRows + myTableRow + "," + " ";
                    //    //founderMinus1 = myTableRows.Remove(myTableRows.Length - 2, 1);
                    //    //NonAttEmp = ""; //founderMinus1;
                    //}
                    //sNoAttNoBasic = sNoAttNoBasic.Remove(sNoAttNoBasic.Length - 1, 1);

                    sNoAttNoBasic = string.Join("," + Environment.NewLine, dtNoAttNoBasic.Rows.Cast<DataRow>()
                                                .Select(x => x["empid"].ToString() + " - " + x["type"].ToString()));

                }
                if (dtGet_Employee_PSType_Fm.Rows.Count > 0)
                {
                    foreach (DataRow dtr in dtGet_Employee_PSType_Fm.Rows)
                    {
                        payslip_status = dtr["status"].ToString();
                        payslip_status_stop = dtr["stpstatus"].ToString();
                        Stopsalary_emp_code = dtr["emp_code"].ToString();
                        if (payslip_status_stop == "StopSalary" && payslip_status == "Adhoc" || payslip_status == "Encashment")
                        {
                            queryStopSalary = "select emp_code from pr_emp_payslip where emp_code in (" + emp_codes + ") " +
                                " and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and " +
                                " active=1 and spl_type='stopsalary';";
                            dt_stop_salary = await _sha.Get_Table_FromQry(queryStopSalary);
                        }

                    }
                }
                if (dt_stop_salary.Rows.Count > 0)
                {
                    try
                    {
                        //StringBuilder sbqry = new StringBuilder();

                        //string insertQry = "";
                        ////StringBuilder sbqry = new StringBuilder();
                        ////1. trans_id
                        //sbqry.Append(GenNewTransactionString());

                        ////2. gen new num
                        //sbqry.Append(GetNewNumString("pr_payroll_service_run"));
                        //if (date == "Now")
                        //{
                        //    insertQry = "INSERT into pr_payroll_service_run(id,run_date_time,status,emp_codes,process_count,total_count,active,trans_id,payslip_type,final_process)" +
                        //    "values(@idnew,GETDATE(), 0 ,'" + emp_codes + "','0'," + count.Length + ",1,@transidnew,'" + Pay_Type + "','" + Finalprocess + "'  );";
                        //}
                        //else
                        //{
                        //    insertQry = "INSERT into pr_payroll_service_run(id,run_date_time,status,emp_codes,process_count,total_count,active,trans_id ,payslip_type,final_process)" +
                        //        "values(@idnew, CONVERT(datetime,'" + date + "', 105), 0 ,'" + emp_codes + "','0'," + count.Length + ",1,@transidnew ,'" + Pay_Type + "' ,'" + Finalprocess + "' );";
                        //}
                        //sbqry.Append(insertQry);

                        ////4. transaction touch
                        //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_payroll_service_run", "@idnew", ""));
                        //sbqry.Append("SELECT @idnew;");
                        if (date == "Now")
                        {
                            //int newid = await _sha.Run_INS_ExecuteScalar(sbqry.ToString());
                            //PaySlipSer payslipSer = new PaySlipSer(_LoginCredential, null);
                            //newRunId = newid;
                            sGenPayslipError = await new PaySlipSer(_LoginCredential, null).Gen_PaySlip(gNewRunId);
                            dtnetamount = await _sha.Get_Table_FromQry(netamount(emp_id, Pay_Type));

                            if (dtnetamount.Rows.Count > 0)
                            {
                                NetAmountlessthanzero = string.Join("," + Environment.NewLine, dtnetamount.Rows.Cast<DataRow>()
                                                                               .Select(x => x["emp_code"].ToString() + " - " + x["type"].ToString()));
                            }
                            if (dtNoAttNoBasic.Rows.Count > 0)
                            {
                                //await EmpcodesfornonAttendence(gNewRunId, Pay_Type);
                            }
                            else
                            {
                                if (Pay_Type == "Encashment" || Pay_Type == "Encashment" && Pay_Type == "Adhoc" || Pay_Type == "Encashment" && Pay_Type == "Regular" || Pay_Type == "Encashment" && Pay_Type == "Adhoc" && Pay_Type == "Regular")
                                {
                                    string emp_code = "";
                                    foreach (DataRow dr in dtGet_Employee_PSType_Fm.Rows)
                                    {
                                        emp_code = dr[0].ToString();
                                        string Qry = "update PLE_Type set payslip_mid = (select top 1 id from pr_emp_payslip where emp_code in ( '" + emp_code + "') and active = 1 order by id Desc) " +
                                         "where EmpId = (select id from Employees where EmpId in ('" + emp_code + "'))";
                                        await _sha.Run_UPDDEL_ExecuteNonQuery(Qry.ToString());
                                    }
                                }
                                return "I#Payslip Services# Processsed Successfully.";
                            }
                        }
                        else
                        {
                            //await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                            return "I#Payslip Services# Processsed Successfully.";
                        }

                    }

                    catch (Exception e)
                    {
                        string msg = e.Message;
                        return "E#Error:#" + msg;
                    }
                }
                else if (dtGet_Employee_PSType_Fm.Rows.Count > 0 && payslip_status_stop != "StopSalary" || (payslip_status_stop == "StopSalary" && payslip_status == "StopSalary"))
                {
                    try
                    {
                        //StringBuilder sbqry = new StringBuilder();

                        //string insertQry = "";
                        ////StringBuilder sbqry = new StringBuilder();
                        ////1. trans_id
                        //sbqry.Append(GenNewTransactionString());

                        ////2. gen new num
                        //sbqry.Append(GetNewNumString("pr_payroll_service_run"));
                        //if (date == "Now")
                        //{
                        //    insertQry = "INSERT into pr_payroll_service_run(id,run_date_time,status,emp_codes,process_count,total_count,active,trans_id,payslip_type,final_process)" +
                        //    "values(@idnew,GETDATE(), 0 ,'" + emp_codes + "','0'," + count.Length + ",1,@transidnew,'" + Pay_Type + "','" + Finalprocess + "'  );";
                        //}
                        //else
                        //{
                        //    insertQry = "INSERT into pr_payroll_service_run(id,run_date_time,status,emp_codes,process_count,total_count,active,trans_id ,payslip_type,final_process)" +
                        //        "values(@idnew, CONVERT(datetime,'" + date + "', 105), 0 ,'" + emp_codes + "','0'," + count.Length + ",1,@transidnew ,'" + Pay_Type + "' ,'" + Finalprocess + "' );";
                        //}
                        //sbqry.Append(insertQry);

                        ////4. transaction touch
                        //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_payroll_service_run", "@idnew", ""));
                        //sbqry.Append("SELECT @idnew;");
                        if (date == "Now")
                        {
                            //int newid = await _sha.Run_INS_ExecuteScalar(sbqry.ToString());
                            //newRunId = newid;
                            //PaySlipSer payslipSer = new PaySlipSer(_LoginCredential, null);
                            //todo 2
                            sGenPayslipError = await new PaySlipSer(_LoginCredential, null).Gen_PaySlip(gNewRunId);
                            dtnetamount = await _sha.Get_Table_FromQry(netamount(emp_id, Pay_Type));

                            if (dtnetamount.Rows.Count > 0)
                            {
                                NetAmountlessthanzero = string.Join("," + Environment.NewLine, dtnetamount.Rows.Cast<DataRow>()
                                                                               .Select(x => x["emp_code"].ToString() + " - " + x["type"].ToString()));
                            }
                            if (dtNoAttNoBasic.Rows.Count > 0 || dtnetamount.Rows.Count > 0)
                            {
                                string qry = "update pr_payroll_service_run set  status=1 ";

                                if (sNoAttNoBasic != "" && sGenPayslipError != "" && NetAmountlessthanzero != "")
                                    qry += ",err_desc='" + (sNoAttNoBasic != "" ? (sNoAttNoBasic) : "") + (sGenPayslipError != "" ? ("," + sGenPayslipError) : "") + (NetAmountlessthanzero != "" ? ("," + NetAmountlessthanzero) : "") + "' ";

                                else if (sNoAttNoBasic != "" && sGenPayslipError != "")
                                    qry += ",err_desc='" + (sNoAttNoBasic != "" ? (sNoAttNoBasic) : "") + (sGenPayslipError != "" ? ("," + sGenPayslipError) : "") + "' ";

                                else if (sNoAttNoBasic != "" && NetAmountlessthanzero != "")
                                    qry += ",err_desc='" + (sNoAttNoBasic != "" ? (sNoAttNoBasic) : "") + (NetAmountlessthanzero != "" ? ("," + NetAmountlessthanzero) : "") + "' ";

                                else if (sGenPayslipError != "" && NetAmountlessthanzero != "")
                                    qry += ",err_desc='" + (sGenPayslipError != "" ? (sGenPayslipError) : "") + (NetAmountlessthanzero != "" ? ("," + NetAmountlessthanzero) : "") + "' ";

                                else if (NetAmountlessthanzero != "")
                                    qry += ",err_desc='" + (NetAmountlessthanzero != "" ? (NetAmountlessthanzero) : "") + "' ";

                                else if (sGenPayslipError != "")
                                    qry += ",err_desc='" + (sGenPayslipError != "" ? (sGenPayslipError) : "") + "' ";

                                else if (sNoAttNoBasic != "")
                                    qry += ",err_desc='" + (sNoAttNoBasic != "" ? (sNoAttNoBasic) : "") + "' ";

                                qry += " Where id=" + gNewRunId;
                                await _sha.Run_UPDDEL_ExecuteNonQuery(qry);

                                return "E#Payslip Error #There are some errors, please check process status.";
                            }
                            else
                            {
                                if (Pay_Type == "Encashment" || Pay_Type == "Encashment" && Pay_Type == "Adhoc" || Pay_Type == "Encashment" && Pay_Type == "Regular" || Pay_Type == "Encashment" && Pay_Type == "Adhoc" && Pay_Type == "Regular")
                                {
                                    string emp_code = "";
                                    foreach (DataRow dr in dtGet_Employee_PSType_Fm.Rows)
                                    {
                                        emp_code = dr[0].ToString();
                                        string Qry = "update PLE_Type set payslip_mid = (select top 1 id from pr_emp_payslip where emp_code in ( '" + emp_code + "') and active = 1 order by id Desc) " +
                                         "where EmpId = (select id from Employees where EmpId in ('" + emp_code + "'))";
                                        await _sha.Run_UPDDEL_ExecuteNonQuery(Qry.ToString());
                                    }
                                }
                                bool obshare = await UpdateOB_Share(empcodes);
                                return "I#Payslip Services# Processsed Successfully.";
                            }
                        }
                        else
                        {
                            //await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                            return "I#Payslip Services# Processsed Successfully.";
                        }
                       
                    }

                    catch (Exception e)
                    {
                        string msg = e.Message;
                        return "E#Error:#" + msg;
                    }
                }
                //else if (payslip_status_stop == "StopSalary" && (payslip_status == "Adhoc" || payslip_status == "Encashment"))
                //{
                //    if (dt_stop_salary.Rows.Count == 0)
                //    {
                //        string qry = "update pr_payroll_service_run set  status=1 ";
                //        //await _sha.Run_UPDDEL_ExecuteNonQuery(ATypes.ToString());
                //        qry += ",err_desc= '" + Stopsalary_emp_code + "   -Salary is Stopped" + (sNoAttNoBasic != "" ? ("," + sNoAttNoBasic) : "") + (sGenPayslipError != "" ? ("," + sGenPayslipError) : "") + "' ";
                //        qry += " Where id=" + gNewRunId;
                //        await _sha.Run_UPDDEL_ExecuteNonQuery(qry);
                //        return "E#Payslip Error #There are some errors, please check process status.";
                //    }
                //}
                //else if (Pay_Type == "Encashment")
                //{
                //    //await _sha.Run_UPDDEL_ExecuteNonQuery(ATypes.ToString());
                //    return "E#Payslip Error # Please give Attendance,Basic and PLEncash amount for the selected employee.";
                //}
                //else if (Pay_Type == "stopsalary")
                //{
                //    //await _sha.Run_UPDDEL_ExecuteNonQuery(ATypes.ToString());
                //    return "E#Payslip Error # The Salary for selected employee is Not stopped Yet.";
                //}
                //else if (Pay_Type == "Adhoc")
                //{
                //    //await _sha.Run_UPDDEL_ExecuteNonQuery(ATypes.ToString());
                //    return "E#Payslip Error # Adhoc Payment Data is not Available for the selected employee.";
                //}
                //else if (Pay_Type == "Suspended")
                //{
                //    //await _sha.Run_UPDDEL_ExecuteNonQuery(ATypes.ToString());
                //    return "E#Payslip Error # Selected Employee is Not Suspended Yet. ";
                //}
                //else 
                //{
                dtnetamount = await _sha.Get_Table_FromQry(netamount(emp_id, Pay_Type));
                if (dtNoAttNoBasic.Rows.Count > 0 || dtnetamount.Rows.Count > 0)
                {
                    string qry = "update pr_payroll_service_run set  status=1 ";

                    if (sNoAttNoBasic != "" && sGenPayslipError != "" && NetAmountlessthanzero != "")
                        qry += ",err_desc='" + (sNoAttNoBasic != "" ? (sNoAttNoBasic) : "") + (sGenPayslipError != "" ? ("," + sGenPayslipError) : "") + (NetAmountlessthanzero != "" ? ("," + NetAmountlessthanzero) : "") + "' ";

                    else if (sNoAttNoBasic != "" && sGenPayslipError != "")
                        qry += ",err_desc='" + (sNoAttNoBasic != "" ? (sNoAttNoBasic) : "") + (sGenPayslipError != "" ? ("," + sGenPayslipError) : "") + "' ";

                    else if (sNoAttNoBasic != "" && NetAmountlessthanzero != "")
                        qry += ",err_desc='" + (sNoAttNoBasic != "" ? (sNoAttNoBasic) : "") + (NetAmountlessthanzero != "" ? ("," + NetAmountlessthanzero) : "") + "' ";

                    else if (sGenPayslipError != "" && NetAmountlessthanzero != "")
                        qry += ",err_desc='" + (sGenPayslipError != "" ? (sGenPayslipError) : "") + (NetAmountlessthanzero != "" ? ("," + NetAmountlessthanzero) : "") + "' ";

                    else if (NetAmountlessthanzero != "")
                        qry += ",err_desc='" + (NetAmountlessthanzero != "" ? (NetAmountlessthanzero) : "") + "' ";

                    else if (sGenPayslipError != "")
                        qry += ",err_desc='" + (sGenPayslipError != "" ? (sGenPayslipError) : "") + "' ";

                    else if (sNoAttNoBasic != "")
                        qry += ",err_desc='" + (sNoAttNoBasic != "" ? (sNoAttNoBasic) : "") + "' ";

                    qry += " Where id=" + gNewRunId;
                    await _sha.Run_UPDDEL_ExecuteNonQuery(qry);

                    return "E#Payslip Error #There are some errors, please check process status.";
                }
            }
            catch (Exception e)
            {
                //string msg = e.Message;
                //return "E#Error:#" + msg;
                return "E#Payslip Error :#" + e.Message;
            }

            return null;
        }
        //this method is use to delete the payslip record from the grid
        public async Task<string> DeletePayslip(string emp_codes)
        {
            string qry = "";
            string retMessage = "";
            string emp_id = "";
            StringBuilder sbqry = new StringBuilder();
            //string dtwd = await SelectedIds(emp_codes);
            //string LeaveBalance = dtwd.TrimEnd(',');
            //var LeaveBalances = LeaveBalance.Split(',');
            //foreach (var leavebal in LeaveBalances)
            //{
            //    var arremp2 = leavebal.Split('#');
            //    emp_id = arremp2[1];
            //}
            //string leaves_available = dtwd.TrimEnd(',');
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            try
            {
                //foreach (string Id in Values)
                //{
                // qry = "update pr_payroll_service_run set active = 0 where id ='" + emp_codes + "'  and active = 1 ";
                qry = "delete from pr_payroll_service_run where id ='" + emp_codes + "' ";
                sbqry.Append(qry);

                //4. transaction touch
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_payroll_service_run", emp_codes.ToString(), ""));

                //}
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    retMessage = "I#Payslip Services# Deleted Successfully";
                }

            }
            catch (Exception e)
            {
                string msg = e.Message;
                return "E#Error:#" + msg;
            }
            return retMessage;
        }
        // select id based on emp_codes
        public async Task<string> SelectedIds(string emp_codes)
        {
            var qry = "";
            string retStr = "";
            qry = "select id from pr_payroll_service_run  where CONVERT(nVARCHAR(4000), emp_codes)='" + emp_codes + "' AND active=1 ";
            DataTable dt = await _sha.Get_Table_FromQry(qry);
            foreach (DataRow dr in dt.Rows)
            {
                retStr = retStr + "id" + "#" + dr["id"].ToString() + ",";
            }
            return retStr;
        }

        public async Task<string> empidsamount(string emp_codes)
        {
            var qry = "";
            if (emp_codes != "")
            {
                qry = "select distinct p.amount,p.emp_code,m.status,m.lop_days,m.absent_days,m.working_days " +
                                "from pr_emp_pay_field p join pr_earn_field_master e on p.m_id = e.id and e.name='Basic' " +
                                "join pr_month_attendance m on p.emp_code = m.emp_code " +
                                "where p.active = 1 and e.active = 1 and m.active = 1 and m.status is not null and m.lop_days is not null " +
                                "and m.absent_days is not null and m.working_days is not null and p.amount !=0 and p.emp_code in (" + emp_codes + ")";
                DataTable dt = await _sha.Get_Table_FromQry(qry);

                foreach (DataRow dr in dt.Rows)
                {
                    amount = dr["amount"].ToString();
                    status = dr["status"].ToString();
                    lop_days = dr["lop_days"].ToString();
                    absent_days = dr["absent_days"].ToString();
                    working_days = dr["working_days"].ToString();
                }
            }
            else
            {
                return null;
            }
            return amount;
        }
        public string netamount(string[] result, string p_type)
        {
            //qry = "select net_amount from pr_emp_payslip where where emp_code in (" + empcodes + ") and status in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 ";
            string[] test = result; // Alternative array creation syntax 
            string empcodes = String.Join(" ", test);


            DateTime Financial_md = (_LoginCredential.FinancialMonthDate);
            int iFY = _LoginCredential.FY;
            int dtFM = _LoginCredential.FM;
            if (p_type.Contains("Regular") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Regular','Adhoc','Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Suspended','Adhoc','Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Adhoc"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Regular','Adhoc')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";

            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Suspended','Adhoc')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";

            }
            else if (p_type.Contains("Regular") && p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Net Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Regular','Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Suspended','Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";

            }
            else if (p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Adhoc','Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Regular"))

            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary  where emp_code in (" + empcodes + ") and spl_type in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";

            }
            else if (p_type.Contains("Suspended"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Suspended')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("stopsalary"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Adhoc"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Adhoc')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'No Sufficient Salary' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            return getallrecords12;
        }
        //this method is used to get the employee who are not having attendance
        private string GetQryForNoAttNoBasic(string[] result, string p_type)
        {
            string[] test = result; // Alternative array creation syntax 
            string empcodes = String.Join(" ", test);


            DateTime Financial_md = (_LoginCredential.FinancialMonthDate);
            int iFY = _LoginCredential.FY;
            int dtFM = _LoginCredential.FM;
            if (p_type.Contains("Regular") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Regular' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance-Regular' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No-Basic' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic' and amount>0" +
                                   " union all " +
                                   " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   " union all " +
                                   " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ")  " +
                                   " except " +
                                   " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                   " join Employees em on en.empid = em.id " +
                                   " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                   " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and active=1 and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   ") as x order by empid";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Suspended' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                    " select distinct emp_code, 'No Attendance-Suspended' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                    " union all " +
                                    " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                   " select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                    " union all " +
                                    " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                    " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                    " union all " +
                                    " select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                    " select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                    " union all " +
                                    " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ")  " +
                                    " except " +
                                    " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                    " join Employees em on en.empid = em.id " +
                                    " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                    " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and active=1 and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                    ") as x order by empid";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Adhoc"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Regular' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance-Regular' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                  " select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                   " union all " +
                                   " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and active=1 and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   ") as x order by empid";

            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Suspended' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance-Suspended' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                   " union all " +
                                   " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and active=1 and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   ") as x order by empid";

            }
            else if (p_type.Contains("Regular") && p_type.Contains("Encashment"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Regular' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance-Regular' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                   " union all " +
                                   " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                    " join Employees em on en.empid = em.id " +
                                    " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                   " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and active=1 and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                    ") as x order by empid";

            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Encashment"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Suspended' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance-Suspended' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                   " union all " +
                                   " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                    " join Employees em on en.empid = em.id " +
                                    " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                   " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and active=1 and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                    ") as x order by empid";

            }
            else if (p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords1 = "select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                     " except " +
                                     " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                     " union all " +
                                     " select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                     " except " +
                                     " select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                     " union all " +
                                     " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                     " except " +
                                    " select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                     " union all " +
                                     " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ")  " +
                                     " except " +
                                     " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                     " join Employees em on en.empid = em.id " +
                                     " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                     " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and active=1 and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped'  " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                     " ) as x order by empid";
            }
            else if (p_type.Contains("Regular"))

            {
                getallrecords1 = "select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                    "except " +
                                    "select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                    "union all " +
                                    "select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                    "except " +
                                    "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                    ") as x order by empid";

            }
            else if (p_type.Contains("Suspended"))
            {
                getallrecords1 = "select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                     "union all " +
                                     "select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic' and amount!=0 and amount>0" +
                                     ") as x order by empid";
            }
            else if (p_type.Contains("stopsalary"))
            {
                getallrecords1 = "select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                     "union all " +
                                     "select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic' and amount!=0 and amount>0" +
                                     ") as x order by empid";
            }
            else if (p_type.Contains("Adhoc"))
            {
                getallrecords1 = "select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                     "union all " +
                                     "select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                     " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and active=1 and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                     ") as x order by empid";
            }
            else if (p_type.Contains("Encashment"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                    " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                    " union all " +
                                    " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                    "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic' and amount!=0 and amount>0" +
                                    " union all " +
                                    " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ")  " +
                                    " except " +
                                    " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                    " join Employees em on en.empid = em.id " +
                                    " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                    " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and active=1 and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                    ") as x order by empid";
            }
            return getallrecords1;
        }
        public async Task<bool> UpdateOB_Share(string emp_codes)
        {
            bool bRet = false;
            string inQry = "";
            string qryUpdate = "";
            string qryvpfdd_amount = ""; DataTable vpf_dd_amount;
            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());


            //string qrygetVpfdata1 = "select epay.emp_code,epay.emp_id,epay.dd_provident_fund, epay.gross_amount,epay.id as payslip_mid from pr_emp_payslip epay " +
            //                     " WHERE (Month(epay.fm) =(select Month(fm) from pr_month_details where Active=1))  AND (Year(epay.fm)=(select Year(fm) from pr_month_details where Active=1))";
            string qrygetVpfdata = "select epay.emp_code,epay.id as payslip_mid,epay.emp_id,count(epay.emp_code),sum(distinct epay.dd_provident_fund) as dd_provident_fund ,sum( distinct epay.gross_amount) as gross_amount ,case  when sum(paydedu.dd_amount) > 0.00 then " +
                "sum(paydedu.dd_amount)  else 0.00 end as dd_amount,sum(distinct epay.NPS)as NPS " +
                " from pr_emp_payslip epay left join pr_emp_payslip_deductions paydedu on epay.emp_code = paydedu.emp_code " +
                " WHERE(Month(epay.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(epay.fm) = (select Year(fm) from pr_month_details where Active = 1)) " +
                " and epay.spl_type='Regular' and epay.emp_code in("+ emp_codes + ") group by epay.emp_code,epay.emp_id ,epay.id ";//added n
                                                                                            //new added "and epay.spl_type='Regular'" in the above line //chaitanya on 4/5/2020
            string qrygetVpfdataencashdata = "select epay.emp_code,epay.id as payslip_mid,epay.emp_id,count(epay.emp_code),sum(distinct epay.dd_provident_fund) as dd_provident_fund ,sum( distinct epay.gross_amount) as gross_amount ,case  when sum(paydedu.dd_amount) > 0.00 then sum(paydedu.dd_amount)  else 0.00 end as dd_amount " +
                           " from pr_emp_payslip epay  Inner join pr_emp_payslip_deductions paydedu on epay.emp_code = paydedu.emp_code " +
                           " WHERE(Month(epay.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(epay.fm) = (select Year(fm) from pr_month_details where Active = 1)) " +
                           " AND paydedu.dd_name = 'VPF Deduction' and epay.spl_type='Encashment' and epay.emp_code in("+ emp_codes + ") group by epay.emp_code,epay.emp_id ,epay.id ";//added n

            string qrygetVpfdataAdhocdata = "select epay.emp_code,epay.id as payslip_mid,epay.emp_id,count(epay.emp_code),sum(distinct epay.dd_provident_fund) as dd_provident_fund ,sum( distinct epay.gross_amount) as gross_amount ,case  when sum(paydedu.dd_amount) > 0.00 then sum(paydedu.dd_amount)  else 0.00 end as dd_amount " +
               " from pr_emp_payslip epay  Inner join pr_emp_payslip_deductions paydedu on epay.emp_code = paydedu.emp_code " +
               " WHERE(Month(epay.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(epay.fm) = (select Year(fm) from pr_month_details where Active = 1)) " +
               " AND paydedu.dd_name = 'VPF Deduction' and epay.spl_type='Adhoc' and epay.emp_code in("+ emp_codes + ") group by epay.emp_code,epay.emp_id ,epay.id ";//added n

            string qrygetfm = "select fm,fy from pr_month_details where Active=1";

            DataSet dtPF = await _sha.Get_MultiTables_FromQry(qrygetVpfdata + qrygetfm + qrygetVpfdataencashdata + qrygetVpfdataAdhocdata);


            DataTable dtPFData = dtPF.Tables[0];
            DataTable dtfm = dtPF.Tables[1];
            DataTable dtPFEncashData = dtPF.Tables[2];
            DataTable dtPFAdhocData = dtPF.Tables[3];
            DateTime fmdate = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());
            int FY = Convert.ToInt32(dtfm.Rows[0]["fy"].ToString());
            string FM = fmdate.ToString("yyyy-MM-dd");



            // Last Month
            DateTime fmlastMonth = fmdate.AddMonths(-1);

            string FMLastMonth = fmlastMonth.ToString("yyyy-MM-dd");

            string[] sa1 = FMLastMonth.Split('-');
            int month = Convert.ToInt32(sa1[1].ToString());

            //Last Year
            int fy = Convert.ToInt32(sa1[0].ToString());

            int NewNumIndex = 0;

            if (dtPFData.Rows.Count > 0)
            {
                foreach (DataRow drFP in dtPFData.Rows)
                {
                    int Emp_code = Convert.ToInt32(drFP["emp_code"].ToString());
                    //int Emp_code = 836;
                    int Emp_id = Convert.ToInt32(drFP["emp_id"].ToString());
                    int payslip_mid = int.Parse(drFP["payslip_mid"].ToString());

                    //newly added on 16/05/2020
                    qryvpfdd_amount = "select dd_amount from pr_emp_payslip_deductions where payslip_mid=(select id from pr_emp_payslip where emp_code=" + Emp_code + " and year(fm)=(select Year(fm) from pr_month_details where Active = 1) and month(fm)=(select Month(fm) from pr_month_details where Active = 1) and spl_type='Regular' ) AND dd_name = 'VPF Deduction'";
                    vpf_dd_amount = await _sha.Get_Table_FromQry(qryvpfdd_amount);
                    //end

                    string VPFdata = "SELECT paydedu.dd_amount from pr_emp_payslip_deductions paydedu where paydedu.payslip_mid =" + payslip_mid + " AND paydedu.dd_name = 'VPF Deduction'";

                    //string GetLastmonthDetailes = " SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount,obshare.bank_share_intrst_open,obshare.own_share_intrst_open,obshare.vpf_intrst_open,obshare.own_share_intrst_total,obshare.bank_share_intrst_total,obshare.vpf_intrst_total from pr_ob_share obshare  " +
                    //    " WHERE active=1 AND obshare.emp_code = " + Emp_code;
                    string GetLastmonthDetailes = "SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.vpf_intrst_total,obshare.own_share_intrst_total,obshare.bank_share_intrst_total from pr_ob_share obshare  WHERE (Month(obshare.fm) =" + month + ")  AND (Year(obshare.fm)=" + fy + ") AND obshare.emp_code=" + Emp_code;

                    string exsitedemp = "SELECT obshare.id,obshare.emp_code,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount From pr_ob_share obshare  WHERE(Month(obshare.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(obshare.fm) = (select Year(fm) from pr_month_details where Active = 1)) and obshare.emp_code =" + Emp_code;

                    string qrybasicda = "select er_basic as basic,er_da as da from pr_emp_payslip where fm=(select fm from pr_month_details where active=1) and spl_type='Regular' and emp_code=" + Emp_code + ";"; ;

                    DataSet dtVPF_lastmonth = await _sha.Get_MultiTables_FromQry(GetLastmonthDetailes + exsitedemp + qrybasicda);
                    DataTable dtbasicda = dtVPF_lastmonth.Tables[2];
                    double vpf_amount = 0;
                    double vpf_amount_intrest_amount = 0;

                    double c_own_share_open = 0;
                    double c_own_share_total = 0;
                    double c_bank_share_open = 0;
                    double c_bank_share_total = 0;
                    double c_vpf_open = 0;
                    double c_vpf_total = 0;

                    // TOTAL INTEREST CALCULATIONS RELATED NAMES ARE COMMENTED 18/19/19
                    // Interest 
                    //double own_share_intrst_amount = 0;
                    //double own_sahre_intrst_open = 0;
                    double own_share_intrst_total = 0;
                    //double vpf_intrst_open = 0;
                    double vpf_intrst_total = 0;
                    //double bank_share_intrst_amount = 0;
                    //double bank_share_intrst_open = 0;
                    double bank_share_intrst_total = 0;
                    //double c_own_share_intrst_amount = 0;
                    //double c_own_sahre_intrst_open = 0;
                    //double c_own_share_intrst_total = 0;
                    //double c_vpf_amount_intrest_amount = 0;
                    //double c_vpf_intrst_open = 0;
                    //double c_vpf_intrst_total = 0;
                    //double c_bank_share_intrst_amount = 0;
                    //double c_bank_share_intrst_open = 0;
                    //double c_bank_share_intrst_total = 0;


                    double own_share_open = 0;
                    double own_share_total = 0;
                    double bank_share_open = 0;
                    double bank_share_total = 0;
                    double vpf_open = 0;
                    double vpf_total = 0;

                    //newly added on 16/05/2020
                    string str_age;
                    DataTable dt_age;
                    //end

                    //  DataTable dtVPF = dtVPF_lastmonth.Tables[0];
                    DataTable dtVPF = dtPF.Tables[0];
                    DataTable dtLastmonthDetailes = dtVPF_lastmonth.Tables[0];
                    DataTable dtemp = dtVPF_lastmonth.Tables[1];
                    //DataTable dtemp = dtPF.Tables[0];
                    if (dtLastmonthDetailes.Rows.Count > 0)
                    {
                        try
                        {
                            //own_share_amounts
                            own_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_open"].ToString());
                            own_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_total"].ToString());

                            //bank_share_amounts
                            bank_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_open"].ToString());
                            bank_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_total"].ToString());


                            //vpf_amounts
                            vpf_open = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_open"].ToString());
                            vpf_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_total"].ToString());

                            //bank_share_interest
                            //bank_share_intrst_amount  = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_intrst_amount"].ToString());
                            //17/09/19 commented 
                            //if(!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["bank_share_intrst_open"].ToString()))
                            //{
                            //    bank_share_intrst_open = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_intrst_open"].ToString());
                            //}

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString()))
                            {
                                bank_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString());
                            }

                            //own_share_interest 
                            //own_share_intrst_amount  = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_intrst_amount"].ToString());

                            //17/09/19 commented 
                            //if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["own_share_intrst_open"].ToString()))
                            //{
                            //    own_sahre_intrst_open = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_intrst_open"].ToString());
                            //}
                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString()))
                            {
                                own_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString());
                            }

                            //    //vpf_interest
                            //    vpf_amount_intrest_amount = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_intrst_amount"].ToString());

                            //17/09/19 commented 
                            //if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["vpf_intrst_open"].ToString()))
                            //{
                            //    vpf_intrst_open = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_intrst_open"].ToString());
                            //}
                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString()))
                            {
                                vpf_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString());
                            }

                        }
                        catch (Exception e)
                        {

                        }
                    }

                    if (dtVPF.Rows.Count > 0)
                    {
                        //vpf_amount = double.Parse(dtVPF.Rows[0]["dd_amount"].ToString()); // this will take 1st row dd_amount value for all the employees
                        //vpf_amount = double.Parse(drFP["dd_amount"].ToString()); // newly added on 4/5/2020 by chaitanya 
                        if (vpf_dd_amount.Rows.Count > 0)
                        {
                            vpf_amount = double.Parse(vpf_dd_amount.Rows[0]["dd_amount"].ToString()); // newly added on 22/5/2020 by chaitanya 
                            vpf_amount_intrest_amount = (vpf_amount * 8.5) / 100;
                        }
                        else
                        {
                            // do nothing
                        }
                        //vpf_amount = double.Parse(vpf_dd_amount.Rows[0]["dd_amount"].ToString()); // newly added on 22/5/2020 by chaitanya 
                        //vpf_amount_intrest_amount = (vpf_amount * 8.5) / 100;

                        //vpf_open and total amounts
                        c_vpf_open = vpf_total;

                        c_vpf_total = (c_vpf_open + vpf_amount);

                    }

                    double own_share_amount = Convert.ToDouble(drFP["dd_provident_fund"].ToString());
                    //double nps_own_share_amount = Convert.ToDouble(drFP["NPS"].ToString());
                    double nps_amount = Convert.ToDouble(drFP["NPS"].ToString());
                    double gross_amount = Convert.ToDouble(drFP["gross_amount"].ToString());

                    double pension_amount = Math.Round((gross_amount * 8.33) / 100);
                    double pension_open = 0;
                    double pension_total = 0;
                    double basic = Convert.ToDouble(dtbasicda.Rows[0]["basic"]);
                    double da = Convert.ToDouble(dtbasicda.Rows[0]["da"]);
                    double nps_own_share_amount = 0;
                    double basicdapercent = Math.Round(((basic + da) * 8.33) / 100, MidpointRounding.AwayFromZero);
                    //newly added on 16/05/2020
                    //str_age = "SELECT EmpId,DOB,DOJ,RetirementDate,CASE WHEN dateadd(year, datediff (year, DOB, getdate()), DOB) > getdate()" +
                    //            "THEN datediff(year, DOB, getdate()) -1 " +
                    //            "ELSE datediff(year, DOB, getdate()) END as Age FROM Employees where Empid=" + Emp_code;
                    str_age = "select Empid,DOB,datediff(month,DOB,(select fm from pr_month_details where active=1)) as Age from Employees where Empid = " + Emp_code;
                    dt_age = await _sha.Get_Table_FromQry(str_age);
                    //age calculating in months
                    if (Convert.ToInt32(dt_age.Rows[0]["Age"]) <= 696)
                    {
                        if ((basic + da) <= 15000)
                        {
                            pension_amount = ((basic + da) * 8.33) / 100;
                            pension_open = pension_amount;
                            pension_open = Math.Round(pension_open, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            pension_open = 1250;
                        }
                    }
                    else
                    {
                        pension_open = 0;
                    }
                    //end

                    //existing condition
                    //if (pension_amount <= 1250)
                    //{
                    //    pension_open = pension_amount;
                    //}
                    //else
                    //{
                    //    pension_open = 1250;

                    //}
                    //end

                    pension_total += pension_open;

                    double bank_share_amount = (own_share_amount - pension_open);
                    //double nps_bank_share_amount = (nps_own_share_amount - pension_open);
                    double nps_bank_share_amount = 0;
                    if (nps_amount != 0 && nps_amount != null)
                    {
                        //nps_bank_share_amount = Math.Round(((basic+da)*0.015),MidpointRounding.AwayFromZero);
                        nps_own_share_amount = nps_amount;
                        nps_bank_share_amount = nps_own_share_amount;
                    }
                    else
                    {
                        nps_own_share_amount = 0;
                        nps_bank_share_amount = 0;
                    }
                    //1.Intrest amount
                    // double own_share_intrest_amount = (own_share_amount * 8.5) / 100;

                    //double bank_share_intrest_amount = (bank_share_amount * 8.5) / 100;

                    //own_share_open and Total _amounts
                    c_own_share_open = own_share_total;
                    c_own_share_total = (c_own_share_open + own_share_amount);

                    //// ownshare Interest

                    //    if (!string.IsNullOrWhiteSpace(dtemp.Rows[0]["own_share_intrst_amount"].ToString()))
                    //    {
                    //        c_own_share_intrst_amount = double.Parse(dtemp.Rows[0]["own_share_intrst_amount"].ToString());
                    //    }

                    //    //Bank_share Interest
                    //    if (!string.IsNullOrWhiteSpace(dtemp.Rows[0]["bank_share_intrst_amount"].ToString()))
                    //    {
                    //        c_bank_share_intrst_amount = double.Parse(dtemp.Rows[0]["bank_share_intrst_amount"].ToString());
                    //    }
                    //    //vpf Interest vpf_amount_intrest_amount
                    //    if (!string.IsNullOrWhiteSpace(dtemp.Rows[0]["vpf_intrst_amount"].ToString()))
                    //    {
                    //        c_vpf_amount_intrest_amount = double.Parse(dtemp.Rows[0]["vpf_intrst_amount"].ToString());
                    //    }



                    //c_own_sahre_intrst_open = own_share_intrst_total;

                    //c_own_share_intrst_total = (c_own_sahre_intrst_open + c_own_share_intrst_amount);


                    //c_bank_share_intrst_open = bank_share_intrst_total;
                    //c_bank_share_intrst_total = (c_bank_share_intrst_open + c_bank_share_intrst_amount);

                    //c_vpf_intrst_open = vpf_intrst_total;
                    //c_vpf_intrst_total = (c_vpf_intrst_open + c_vpf_amount_intrest_amount);
                    //Bank_Share_Open And Total _Amounts
                    c_bank_share_open = bank_share_total;
                    c_bank_share_total = (c_bank_share_open + bank_share_amount);

                    //2. gen new num
                    NewNumIndex++;

                    sbqry.Append(GetNewNumStringArr("pr_ob_share", NewNumIndex));
                    //query

                    //for update records
                    qryUpdate = "update pr_ob_share set active = 0 where emp_code=" + Emp_code + ";";
                    sbqry.Append(qryUpdate);

                    if (dtemp.Rows.Count > 0)
                    {
                        int c_id = int.Parse(dtemp.Rows[0]["id"].ToString());

                        //   inQry = "Update pr_ob_share set own_share=" + own_share_amount + ",own_share_intrst_amount=" + own_share_intrest_amount + " ,vpf =" + vpf_amount + " ,vpf_intrst_amount =" + vpf_amount_intrest_amount + " ,bank_share=" + bank_share_amount + ",bank_share_intrst_amount=" + bank_share_intrest_amount + " ,own_share_open =" + c_own_share_open + " ,own_share_total=" + c_own_share_total + " ,vpf_open=" + c_vpf_open + " ,vpf_total=" + c_vpf_total + " ,bank_share_open=" + c_bank_share_open + " ,bank_share_total=" + c_bank_share_total + ",pension_open="+ pension_open+ ",pension_total="+ pension_total + ",own_share_intrst_open="+ c_own_sahre_intrst_open + ",bank_share_intrst_open="+ c_bank_share_intrst_open + ", vpf_intrst_open="+ c_vpf_intrst_open + ",own_share_intrst_total="+ c_own_share_intrst_total + ",bank_share_intrst_total="+ c_bank_share_intrst_total + ",vpf_intrst_total="+ c_vpf_intrst_total + "  WHERE id=" + c_id + " AND emp_code=" + Emp_code + ";";

                        inQry = "Update pr_ob_share set own_share=" + own_share_amount + ",vpf =" + vpf_amount + " ," +
                            "bank_share=" + bank_share_amount + ",own_share_open =" + c_own_share_open + " ," +
                            "own_share_total=" + c_own_share_total + " ,vpf_open=" + c_vpf_open + " ,vpf_total=" + c_vpf_total + " ," +
                            "bank_share_open=" + c_bank_share_open + " ,bank_share_total=" + c_bank_share_total + "," +
                            "pension_open=" + pension_open + ",pension_total=" + pension_total + ",  " +
                            " own_share_intrst_total=" + own_share_intrst_total + ",bank_share_intrst_total=" + bank_share_intrst_total + "," +
                            "vpf_intrst_total=" + vpf_intrst_total + ",NPS_own_share=" + nps_own_share_amount + ", " +
                            "NPS_bank_share=" + nps_bank_share_amount + ",NPS_pension_open=" + pension_open + "  WHERE id=" + c_id + " AND emp_code=" + Emp_code + ";";
                    }
                    else
                    {
                        inQry = "Insert into pr_ob_share(id,fy,fm,emp_id,emp_code," +
                        "own_share,vpf,bank_share," +
                        "active,[trans_id],[own_share_open]," +
                        "[own_share_total],[vpf_open],[vpf_total]," +
                        "[bank_share_open]," + "[bank_share_total],[pension_open],[pension_total],[own_share_intrst_total],[bank_share_intrst_total],[vpf_intrst_total],[NPS_own_share],[NPS_bank_share],[NPS_pension_open])" +
                        "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + Emp_id + "," + Emp_code + "," +
                        "" + own_share_amount + "," + vpf_amount + "," + bank_share_amount + "," +
                        "1, @transidnew " + "," + "" + c_own_share_open + "," +
                        "" + c_own_share_total + "," + c_vpf_open + " , " + c_vpf_total + " ," +
                        " " + c_bank_share_open + " , " + c_bank_share_total + "," + pension_open + "," + pension_total + "   ," +
                        "" + own_share_intrst_total + "," + bank_share_intrst_total + "," + vpf_intrst_total + "," + nps_own_share_amount + "," +
                        "" + nps_bank_share_amount + "," + pension_open + ");";

                    }
                    sbqry.Append(inQry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_attendance", "@idnew" + NewNumIndex, ""));
                }
            }
            if (dtPFEncashData.Rows.Count > 0)
            {
                foreach (DataRow drFP in dtPFEncashData.Rows)
                {
                    int Emp_code = Convert.ToInt32(drFP["emp_code"].ToString());
                    //int Emp_code = 836;
                    int Emp_id = Convert.ToInt32(drFP["emp_id"].ToString());
                    int payslip_mid = int.Parse(drFP["payslip_mid"].ToString());

                    //newly added on 16/05/2020
                    qryvpfdd_amount = "select dd_amount from pr_emp_payslip_deductions where payslip_mid=(select id from pr_emp_payslip where emp_code=" + Emp_code + " and year(fm)=(select Year(fm) from pr_month_details where Active = 1) and month(fm)=(select Month(fm) from pr_month_details where Active = 1) and spl_type='Encashment' ) AND dd_name = 'VPF Deduction'";
                    vpf_dd_amount = await _sha.Get_Table_FromQry(qryvpfdd_amount);
                    //end

                    string VPFdata = "SELECT paydedu.dd_amount from pr_emp_payslip_deductions paydedu where paydedu.payslip_mid =" + payslip_mid + " AND paydedu.dd_name = 'VPF Deduction'";

                    //string GetLastmonthDetailes = " SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount,obshare.bank_share_intrst_open,obshare.own_share_intrst_open,obshare.vpf_intrst_open,obshare.own_share_intrst_total,obshare.bank_share_intrst_total,obshare.vpf_intrst_total from pr_ob_share_encashment obshare  " +
                    //    " WHERE active=1 AND obshare.emp_code = " + Emp_code;
                    string GetLastmonthDetailes = "SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.vpf_intrst_total,obshare.own_share_intrst_total,obshare.bank_share_intrst_total from pr_ob_share_encashment obshare  WHERE (Month(obshare.fm) =" + month + ")  AND (Year(obshare.fm)=" + fy + ") AND obshare.emp_code=" + Emp_code;

                    string exsitedemp = "SELECT obshare.id,obshare.emp_code,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount From pr_ob_share_encashment obshare  WHERE(Month(obshare.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(obshare.fm) = (select Year(fm) from pr_month_details where Active = 1)) and obshare.emp_code =" + Emp_code;

                    DataSet dtVPF_lastmonth = await _sha.Get_MultiTables_FromQry(GetLastmonthDetailes + exsitedemp);

                    double vpf_amount = 0;
                    double vpf_amount_intrest_amount = 0;

                    double c_own_share_open = 0;
                    double c_own_share_total = 0;
                    double c_bank_share_open = 0;
                    double c_bank_share_total = 0;
                    double c_vpf_open = 0;
                    double c_vpf_total = 0;

                    // TOTAL INTEREST CALCULATIONS RELATED NAMES ARE COMMENTED 18/19/19
                    // Interest 
                    double own_share_intrst_total = 0;
                    double vpf_intrst_total = 0;
                    double bank_share_intrst_total = 0;
                    double own_share_open = 0;
                    double own_share_total = 0;
                    double bank_share_open = 0;
                    double bank_share_total = 0;
                    double vpf_open = 0;
                    double vpf_total = 0;

                    //newly added on 16/05/2020
                    string str_age;
                    DataTable dt_age;
                    //end

                    //  DataTable dtVPF = dtVPF_lastmonth.Tables[0];
                    DataTable dtVPF = dtPF.Tables[0];
                    DataTable dtLastmonthDetailes = dtVPF_lastmonth.Tables[0];
                    DataTable dtemp = dtVPF_lastmonth.Tables[1];
                    //DataTable dtemp = dtPF.Tables[0];
                    if (dtLastmonthDetailes.Rows.Count > 0)
                    {
                        try
                        {
                            //own_share_amounts
                            own_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_open"].ToString());
                            own_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_total"].ToString());

                            //bank_share_amounts
                            bank_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_open"].ToString());
                            bank_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_total"].ToString());


                            //vpf_amounts
                            vpf_open = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_open"].ToString());
                            vpf_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_total"].ToString());

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString()))
                            {
                                bank_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString());
                            }

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString()))
                            {
                                own_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString());
                            }

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString()))
                            {
                                vpf_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString());
                            }

                        }
                        catch (Exception e)
                        {

                        }
                    }

                    if (dtVPF.Rows.Count > 0)
                    {

                        if (vpf_dd_amount.Rows.Count > 0)
                        {
                            vpf_amount = double.Parse(vpf_dd_amount.Rows[0]["dd_amount"].ToString()); // newly added on 22/5/2020 by chaitanya 
                            vpf_amount_intrest_amount = (vpf_amount * 8.5) / 100;
                        }
                        else
                        {
                            // do nothing
                        }

                        c_vpf_open = vpf_total;

                        c_vpf_total = (c_vpf_open + vpf_amount);

                    }

                    double own_share_amount = Convert.ToDouble(drFP["dd_provident_fund"].ToString());
                    double gross_amount = Convert.ToDouble(drFP["gross_amount"].ToString());

                    double pension_amount = Math.Round((gross_amount * 8.33) / 100);
                    double pension_open = 0;
                    double pension_total = 0;

                    //newly added on 16/05/2020
                    str_age = "SELECT EmpId,DOB,DOJ,RetirementDate,CASE WHEN dateadd(year, datediff (year, DOB, getdate()), DOB) > getdate()" +
                                "THEN datediff(year, DOB, getdate()) -1 " +
                                "ELSE datediff(year, DOB, getdate()) END as Age FROM Employees where Empid=" + Emp_code;
                    dt_age = await _sha.Get_Table_FromQry(str_age);
                    if (Convert.ToInt32(dt_age.Rows[0]["Age"]) <= 58)
                    {
                        if (pension_amount <= 1250)
                        {
                            pension_open = pension_amount;
                        }
                        else
                        {
                            pension_open = 1250;
                        }
                    }
                    else
                    {
                        pension_open = 0;
                    }

                    pension_total += pension_open;

                    double bank_share_amount = (own_share_amount - pension_open);
                    c_own_share_open = own_share_total;
                    c_own_share_total = (c_own_share_open + own_share_amount);
                    c_bank_share_open = bank_share_total;
                    c_bank_share_total = (c_bank_share_open + bank_share_amount);

                    //2. gen new num
                    NewNumIndex++;

                    sbqry.Append(GetNewNumStringArr("pr_ob_share_encashment", NewNumIndex));
                    //query

                    //for update records
                    qryUpdate = "update pr_ob_share_encashment set active = 0 where emp_code=" + Emp_code + ";";
                    sbqry.Append(qryUpdate);

                    if (dtemp.Rows.Count > 0)
                    {
                        int c_id = int.Parse(dtemp.Rows[0]["id"].ToString());
                        inQry = "Update pr_ob_share_encashment set own_share=" + own_share_amount + ",vpf =" + vpf_amount + " ,bank_share=" + bank_share_amount + ",own_share_open =" + c_own_share_open + " ,own_share_total=" + c_own_share_total + " ,vpf_open=" + c_vpf_open + " ,vpf_total=" + c_vpf_total + " ,bank_share_open=" + c_bank_share_open + " ,bank_share_total=" + c_bank_share_total + ",pension_open=" + pension_open + ",pension_total=" + pension_total + ",   own_share_intrst_total=" + own_share_intrst_total + ",bank_share_intrst_total=" + bank_share_intrst_total + ",vpf_intrst_total=" + vpf_intrst_total + "  WHERE id=" + c_id + " AND emp_code=" + Emp_code + ";";
                    }
                    else
                    {
                        inQry = "Insert into pr_ob_share_encashment(id,fy,fm,emp_id,emp_code," +
                        "own_share,vpf,bank_share," +
                        "active,[trans_id],[own_share_open]," +
                        "[own_share_total],[vpf_open],[vpf_total]," +
                        "[bank_share_open]," + "[bank_share_total],[pension_open],[pension_total],[own_share_intrst_total]," +
                        "[bank_share_intrst_total],[vpf_intrst_total],[is_interest_caculated],[pension_intrest_amount])" +
                        "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + Emp_id + "," + Emp_code + "," +
                        "" + own_share_amount + "," + vpf_amount + "," + bank_share_amount + "," +
                        "1, @transidnew " + "," + "" + c_own_share_open + "," +
                        "" + c_own_share_total + "," + c_vpf_open + " , " + c_vpf_total + " ," +
                        " " + c_bank_share_open + " , " + c_bank_share_total + "," + pension_open + "," + pension_total + "   ," + own_share_intrst_total + "," + bank_share_intrst_total + "," + vpf_intrst_total + ",0,0);";

                    }
                    sbqry.Append(inQry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_attendance", "@idnew" + NewNumIndex, ""));
                }
            }
            if (dtPFAdhocData.Rows.Count > 0)
            {
                foreach (DataRow drFP in dtPFAdhocData.Rows)
                {
                    int Emp_code = Convert.ToInt32(drFP["emp_code"].ToString());
                    //int Emp_code = 836;
                    int Emp_id = Convert.ToInt32(drFP["emp_id"].ToString());
                    int payslip_mid = int.Parse(drFP["payslip_mid"].ToString());

                    //newly added on 16/05/2020
                    qryvpfdd_amount = "select dd_amount from pr_emp_payslip_deductions where payslip_mid=(select id from pr_emp_payslip where emp_code=" + Emp_code + " and year(fm)=(select Year(fm) from pr_month_details where Active = 1) and month(fm)=(select Month(fm) from pr_month_details where Active = 1) and spl_type='Adhoc' ) AND dd_name = 'VPF Deduction'";
                    vpf_dd_amount = await _sha.Get_Table_FromQry(qryvpfdd_amount);
                    //end

                    string VPFdata = "SELECT paydedu.dd_amount from pr_emp_payslip_deductions paydedu where paydedu.payslip_mid =" + payslip_mid + " AND paydedu.dd_name = 'VPF Deduction'";

                    //string GetLastmonthDetailes = " SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount,obshare.bank_share_intrst_open,obshare.own_share_intrst_open,obshare.vpf_intrst_open,obshare.own_share_intrst_total,obshare.bank_share_intrst_total,obshare.vpf_intrst_total from pr_ob_share_adhoc obshare  " +
                    //    " WHERE active=1 AND obshare.emp_code = " + Emp_code;
                    string GetLastmonthDetailes = "SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.vpf_intrst_total,obshare.own_share_intrst_total,obshare.bank_share_intrst_total from pr_ob_share_adhoc obshare  WHERE (Month(obshare.fm) =" + month + ")  AND (Year(obshare.fm)=" + fy + ") AND obshare.emp_code=" + Emp_code;

                    string exsitedemp = "SELECT obshare.id,obshare.emp_code,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount From pr_ob_share_adhoc obshare  WHERE(Month(obshare.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(obshare.fm) = (select Year(fm) from pr_month_details where Active = 1)) and obshare.emp_code =" + Emp_code;

                    DataSet dtVPF_lastmonth = await _sha.Get_MultiTables_FromQry(GetLastmonthDetailes + exsitedemp);

                    double vpf_amount = 0;
                    double vpf_amount_intrest_amount = 0;

                    double c_own_share_open = 0;
                    double c_own_share_total = 0;
                    double c_bank_share_open = 0;
                    double c_bank_share_total = 0;
                    double c_vpf_open = 0;
                    double c_vpf_total = 0;

                    // TOTAL INTEREST CALCULATIONS RELATED NAMES ARE COMMENTED 18/19/19
                    // Interest 
                    double own_share_intrst_total = 0;
                    double vpf_intrst_total = 0;
                    double bank_share_intrst_total = 0;
                    double own_share_open = 0;
                    double own_share_total = 0;
                    double bank_share_open = 0;
                    double bank_share_total = 0;
                    double vpf_open = 0;
                    double vpf_total = 0;

                    //newly added on 16/05/2020
                    string str_age;
                    DataTable dt_age;
                    //end

                    //  DataTable dtVPF = dtVPF_lastmonth.Tables[0];
                    DataTable dtVPF = dtPF.Tables[0];
                    DataTable dtLastmonthDetailes = dtVPF_lastmonth.Tables[0];
                    DataTable dtemp = dtVPF_lastmonth.Tables[1];
                    //DataTable dtemp = dtPF.Tables[0];
                    if (dtLastmonthDetailes.Rows.Count > 0)
                    {
                        try
                        {
                            //own_share_amounts
                            own_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_open"].ToString());
                            own_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_total"].ToString());

                            //bank_share_amounts
                            bank_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_open"].ToString());
                            bank_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_total"].ToString());


                            //vpf_amounts
                            vpf_open = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_open"].ToString());
                            vpf_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_total"].ToString());

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString()))
                            {
                                bank_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString());
                            }

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString()))
                            {
                                own_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString());
                            }

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString()))
                            {
                                vpf_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString());
                            }

                        }
                        catch (Exception e)
                        {
                            
                        }
                    }

                    if (dtVPF.Rows.Count > 0)
                    {

                        if (vpf_dd_amount.Rows.Count > 0)
                        {
                            vpf_amount = double.Parse(vpf_dd_amount.Rows[0]["dd_amount"].ToString()); // newly added on 22/5/2020 by chaitanya 
                            vpf_amount_intrest_amount = (vpf_amount * 8.5) / 100;
                        }
                        else
                        {
                            // do nothing
                        }

                        c_vpf_open = vpf_total;

                        c_vpf_total = (c_vpf_open + vpf_amount);

                    }

                    double own_share_amount = Convert.ToDouble(drFP["dd_provident_fund"].ToString());
                    double gross_amount = Convert.ToDouble(drFP["gross_amount"].ToString());

                    double pension_amount = Math.Round((gross_amount * 8.33) / 100);
                    double pension_open = 0;
                    double pension_total = 0;

                    //newly added on 16/05/2020
                    str_age = "SELECT EmpId,DOB,DOJ,RetirementDate,CASE WHEN dateadd(year, datediff (year, DOB, getdate()), DOB) > getdate()" +
                                "THEN datediff(year, DOB, getdate()) -1 " +
                                "ELSE datediff(year, DOB, getdate()) END as Age FROM Employees where Empid=" + Emp_code;
                    dt_age = await _sha.Get_Table_FromQry(str_age);
                    if (Convert.ToInt32(dt_age.Rows[0]["Age"]) <= 58)
                    {
                        if (pension_amount <= 1250)
                        {
                            pension_open = pension_amount;
                        }
                        else
                        {
                            pension_open = 1250;
                        }
                    }
                    else
                    {
                        pension_open = 0;
                    }

                    pension_total += pension_open;

                    double bank_share_amount = (own_share_amount - pension_open);
                    c_own_share_open = own_share_total;
                    c_own_share_total = (c_own_share_open + own_share_amount);
                    c_bank_share_open = bank_share_total;
                    c_bank_share_total = (c_bank_share_open + bank_share_amount);

                    //2. gen new num
                    NewNumIndex++;

                    sbqry.Append(GetNewNumStringArr("pr_ob_share_adhoc", NewNumIndex));
                    //query

                    //for update records
                    qryUpdate = "update pr_ob_share_adhoc set active = 0 where emp_code=" + Emp_code + ";";
                    sbqry.Append(qryUpdate);

                    if (dtemp.Rows.Count > 0)
                    {
                        int c_id = int.Parse(dtemp.Rows[0]["id"].ToString());
                        inQry = "Update pr_ob_share_adhoc set own_share=" + own_share_amount + ",vpf =" + vpf_amount + " ,bank_share=" + bank_share_amount + ",own_share_open =" + c_own_share_open + " ,own_share_total=" + c_own_share_total + " ,vpf_open=" + c_vpf_open + " ,vpf_total=" + c_vpf_total + " ,bank_share_open=" + c_bank_share_open + " ,bank_share_total=" + c_bank_share_total + ",pension_open=" + pension_open + ",pension_total=" + pension_total + ",   own_share_intrst_total=" + own_share_intrst_total + ",bank_share_intrst_total=" + bank_share_intrst_total + ",vpf_intrst_total=" + vpf_intrst_total + "  WHERE id=" + c_id + " AND emp_code=" + Emp_code + ";";
                    }
                    else
                    {
                        inQry = "Insert into pr_ob_share_adhoc(id,fy,fm,emp_id,emp_code," +
                        "own_share,vpf,bank_share," +
                        "active,[trans_id],[own_share_open]," +
                        "[own_share_total],[vpf_open],[vpf_total]," +
                        "[bank_share_open]," + "[bank_share_total],[pension_open],[pension_total],[own_share_intrst_total]," +
                        "[bank_share_intrst_total],[vpf_intrst_total],[is_interest_caculated],[pension_intrest_amount])" +
                        "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + Emp_id + "," + Emp_code + "," +
                        "" + own_share_amount + "," + vpf_amount + "," + bank_share_amount + "," +
                        "1, @transidnew " + "," + "" + c_own_share_open + "," +
                        "" + c_own_share_total + "," + c_vpf_open + " , " + c_vpf_total + " ," +
                        " " + c_bank_share_open + " , " + c_bank_share_total + "," + pension_open + "," + pension_total + "   ," + own_share_intrst_total + "," + bank_share_intrst_total + "," + vpf_intrst_total + ",0,0);";
                    }
                    sbqry.Append(inQry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_attendance", "@idnew" + NewNumIndex, ""));
                }
            }
            try
            {
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
            }
            catch (Exception ex)
            {
                bRet = false;
            }

            return bRet;
        }


    }
}

