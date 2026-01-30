using Mavensoft.DAL.Db;
using Newtonsoft.Json;
using PayrollModels;
using System;
using System.Collections.Generic;
using Mavensoft.DAL.Business;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web.Script.Serialization;
namespace PayRollBusiness
{
    public class CommonBusiness : BusinessBase
    {
        SqlHelperAsync _Sha = new SqlHelperAsync();
        LoginCredential _loginCredential = null;
     
        public CommonBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
            _loginCredential = loginCredential;
        }

        public string Env_Fm_Fy()
        {
            return _loginCredential.AppEnvironment + " , " +
                _loginCredential.FinancialMonthDate.ToString("MMM")
                + " (" + (_loginCredential.FY - 1) + " - " + (_loginCredential.FY) + ")";
        }
        //newly added by chaitanya on 24/04/2020
        public async Task<string> PFintyr()
        {
            string query = "select fy, interest from pr_pf_year_rate";
            DataTable dt = await _sha.Get_Table_FromQry(query);
            return JsonConvert.SerializeObject(dt);
        }
        //end
        public async Task<string> RunorRevertbuttonenabledisable()
        {
            string Ret = "";
            string query = "select * from pr_emp_adv_loans_adjustments where  payment_type not in ('Full Clearing','Part Payment') and fm='" + _loginCredential.FinancialMonthDate.ToString("yyyy-MM-dd") + "'";
            DataTable dt = await _sha.Get_Table_FromQry(query);
            if(dt.Rows.Count>0)
            {
                return Ret = "Disable";
            }
            else
            {
                return Ret = "Enable";
            }
        }

        public string Fm_AdjustLoanDatePicker()
        {
            return
                _loginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
        }

        //public string Env_Fm_Fy1()
        //{
        //    return _loginCredential.CurrFinancialMonth;
        //}
        public async Task<string> Bg_Load_InitData()
        {

            string strQry = "Select fy, fm from pr_month_details where active = 1";
            DataTable dt = await _Sha.Get_Table_FromQry(strQry);
            return dt.Rows[0]["fy"].ToString() + "*" +
                dt.Rows[0]["fm"].ToString() ;
        }
        public async Task<string> Bg_Load_InitDataCount()
        {

            string strQry = " select pslipcount,pfauth,incre,j, p,pf,authrep,pfrep,plencash,increauth,incredatecha " +
" from"+
" (select count(*) as pfauth from pr_emp_pf_nonrepayable_loan where authorisation = 0 and active=1 and process=0) as t," +

" (select count(*) as incre from  pr_emp_inc_anual_stag where authorisation=1 and process=1 and active=1) as t0," +
  " (select COUNT(*) as j from pr_emp_jaib_caib_general where authorisation = 0) as t1," +
 " (select count(*) as p from Employee_Transfer  where authorisation = 0 and active=1) as t2,"+
  " (select count(*) as pf from pr_emp_pf_nonrepayable_loan where process = 0 and Authorisation=1) as t3 ," +

  " (select (isnull(sum(total_count),0)-isnull(sum(process_count),0)) as pslipcount from pr_payroll_service_run where status=0 ) as pslip,"+
  " (select count(*) as authrep from pr_emp_pf_repayable_loan where authorisation = 0 and active=1 and process=0) as trep," +
   " (select count(*) as pfrep from pr_emp_pf_repayable_loan where process = 0 and Authorisation=1) as t3rep ," +
   " (select count(*) as plencash from PLE_Type where authorisation = 1 and process = 0) as encash," +
   " (select count(*) as increauth from  pr_emp_inc_anual_stag where authorisation=0 and process=1 and active=1) as instagauth,"+
  " (select  count(*) as incredatecha from pr_emp_inc_date_change where active = 1 and process=1 and authorisation = 0) as incdate";
         
            DataTable dt = await _Sha.Get_Table_FromQry(strQry);
            return dt.Rows[0]["pfauth"].ToString() + "*" + dt.Rows[0]["incre"].ToString() + "*" +
                 dt.Rows[0]["j"].ToString() + "*" + dt.Rows[0]["p"].ToString() + "*" + dt.Rows[0]["pf"].ToString()+ "*" + dt.Rows[0]["pslipcount"].ToString()+ "*" + dt.Rows[0]["authrep"].ToString()+ "*" + dt.Rows[0]["pfrep"].ToString() + "*" + dt.Rows[0]["plencash"].ToString() + "*" + dt.Rows[0]["increauth"].ToString() + "*" + dt.Rows[0]["incredatecha"].ToString();
        }
        public DateTime GetCurrentTime(DateTime ldate)
        {
            DateTime serverTime = DateTime.Now;
            DateTime utcTime = serverTime.ToUniversalTime();
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }

        public async Task<string> SearchEmployee(string EmpCode)
        {
            List<EmployeeSearchResult> retList = new List<EmployeeSearchResult>();


            string strQry = "select e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
            + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
            + " join Designations d on e.CurrentDesignation = d.Id"
            + " join Branches b on e.Branch = b.Id"
            + " join Departments dep on e.Department = dep.Id where empid =" + EmpCode + "";

            DataTable dtEmpDetails = null;
            try
            {
                dtEmpDetails = await _Sha.Get_Table_FromQry(strQry);
            }
            catch
            {
            }

            if (dtEmpDetails == null || dtEmpDetails.Rows.Count == 0)
            {
                strQry = "select e.empid, e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
                + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
                + " join Designations d on e.CurrentDesignation = d.Id"
                + " join Branches b on e.Branch = b.Id join Departments dep on e.Department = dep.Id";

                int ecode = 0;
                if (int.TryParse(EmpCode, out ecode))
                {
                    strQry += " where  e.RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) and empid LIKE '" + EmpCode + "%'; ";
                }
                else
                {
                    strQry += " where  e.RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) and  FirstName LIKE '%" + EmpCode + "%' OR LastName LIKE '%" + EmpCode + "%'; ";
                }

                dtEmpDetails = await _Sha.Get_Table_FromQry(strQry);
            }
            DateTime curdate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime lastday = new DateTime(_LoginCredential.FinancialMonthDate.Year, _LoginCredential.FinancialMonthDate.Month, 1).AddMonths(1).AddDays(-1);
            DateTime nextmonthfirstday = new DateTime(_LoginCredential.FinancialMonthDate.Year, _LoginCredential.FinancialMonthDate.Month, 1).AddMonths(1);
            //int curFY = _LoginCredential.FY;
            string[] arrFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd").Split('-');
            int curFY =Convert.ToInt32(arrFm[0]);
            int curFM = Convert.ToInt32(arrFm[1]);
            string[] arrFm1 = nextmonthfirstday.ToString("yyyy-MM-dd").Split('-');
            int curFY1 = Convert.ToInt32(arrFm1[0]);
            int curFM1 = Convert.ToInt32(arrFm1[1]);
            int curFd1 = Convert.ToInt32(arrFm1[2]);


            //int i_fy = Convert.ToInt32(FM);


            foreach (DataRow drEDet in dtEmpDetails.Rows)
            {
                DateTime retdate = Convert.ToDateTime(drEDet["RetirementDate"]);
                int reFm = retdate.Month;
                int retFy = retdate.Year;
                int retFd = retdate.Day;

                //if ((curFY > retFy && (curFM> reFm || curFM <= reFm))||(curFY==retFy && curFM >= reFm))
                if (retdate< lastday)
                {

                    retList.Add(new EmployeeSearchResult
                    {
                        ECode = "",
                        EName = "",
                        EDesignation = "",
                        EBranch = "",
                        EDoj = "",
                        EDRetire = "",
                        EDoc = "",
                    });
                }

                else
                {
                    retList.Add(new EmployeeSearchResult
                    {
                        ECode = drEDet["empid"].ToString(),
                        EName = drEDet["Name"].ToString(),
                        EDesignation = drEDet["Designation"].ToString(),
                        EBranch = drEDet["deptbranch"].ToString(),
                        EDoj = drEDet["DOJ"].ToString(),
                        EDRetire = drEDet["RetirementDate"].ToString(),
                        EDoc = Convert.ToDateTime(drEDet["DOJ"].ToString()).AddMonths(12).ToString("dd-MM-yyyy"),
                    });
                }


            }

            return JsonConvert.SerializeObject(retList);

        }

        public async Task<string> SearchEmployeeDateOfLeaving(string EmpCode)
        {
            List<EmployeeSearchResult> retList = new List<EmployeeSearchResult>();


            string strQry = "select e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
            + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
            + " join Designations d on e.CurrentDesignation = d.Id"
            + " join Branches b on e.Branch = b.Id"
            + " join Departments dep on e.Department = dep.Id where empid =" + EmpCode + "";

            DataTable dtEmpDetails = null;
            try
            {
                dtEmpDetails = await _Sha.Get_Table_FromQry(strQry);
            }
            catch
            {
            }

            if (dtEmpDetails == null || dtEmpDetails.Rows.Count == 0)
            {
                strQry = "select e.empid, e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
                + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
                + " join Designations d on e.CurrentDesignation = d.Id"
                + " join Branches b on e.Branch = b.Id join Departments dep on e.Department = dep.Id";

                int ecode = 0;
                if (int.TryParse(EmpCode, out ecode))
                {
                    strQry += " where  e.RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) and empid LIKE '" + EmpCode + "%'; ";
                }
                else
                {
                    strQry += " where  e.RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) and  FirstName LIKE '%" + EmpCode + "%' OR LastName LIKE '%" + EmpCode + "%'; ";
                }

                dtEmpDetails = await _Sha.Get_Table_FromQry(strQry);
            }
            DateTime curdate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            //int curFY = _LoginCredential.FY;
            string[] arrFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM").Split('-');
            int curFY = Convert.ToInt32(arrFm[0]);
            int curFM = Convert.ToInt32(arrFm[1]);


            //int i_fy = Convert.ToInt32(FM);


            foreach (DataRow drEDet in dtEmpDetails.Rows)
            {
                DateTime retdate = Convert.ToDateTime(drEDet["RetirementDate"]);
                int reFm = retdate.Month;
                int retFy = retdate.Year;



                retList.Add(new EmployeeSearchResult
                {
                    ECode = drEDet["empid"].ToString(),
                    EName = drEDet["Name"].ToString(),
                    EDesignation = drEDet["Designation"].ToString(),
                    EBranch = drEDet["deptbranch"].ToString(),
                    EDoj = drEDet["DOJ"].ToString(),
                    EDRetire = drEDet["RetirementDate"].ToString(),
                    EDoc = Convert.ToDateTime(drEDet["DOJ"].ToString()).AddMonths(12).ToString("dd-MM-yyyy"),
                });



            }

            return JsonConvert.SerializeObject(retList);

        }


        public async Task<string> SearchEmployeeWithChkIDs(string EmpCode)
        {
            var empcode = "0";
            if (EmpCode == "")
            {
                empcode = "0";
            }
            DateTime Fm = _LoginCredential.FinancialMonthDate;
            DateTime Fmenddate = _LoginCredential.FinancialMonthDate.AddMonths(1).AddDays(-1);
            List<EmployeeSearchResult> retList = new List<EmployeeSearchResult>();

            //string strQry = "select e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
            //+ " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
            //+ " join Designations d on e.CurrentDesignation = d.Id"
            //+ " join Branches b on e.Branch = b.Id"
            //+ " join Departments dep on e.Department = dep.Id where e.RetirementDate>=getdate() order by e.EmpId; ";
            string strQry = "select e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
            + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
            + " join Designations d on e.CurrentDesignation = d.Id"
            + " join Branches b on e.Branch = b.Id"
            + " join Departments dep on e.Department = dep.Id where e.RetirementDate>=CAST('"+Fmenddate+"' as DATETIME) order by e.EmpId; ";

            DataTable dtEmpDetails = null;
            try
            {
                dtEmpDetails = await _Sha.Get_Table_FromQry(strQry);
            }
            catch
            {
            }

            if (dtEmpDetails == null || dtEmpDetails.Rows.Count == 0)
            {
                strQry = "select e.empid, e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
                + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
                + " join Designations d on e.CurrentDesignation = d.Id"
                + " join Branches b on e.Branch = b.Id join Departments dep on e.Department = dep.Id";

                int ecode = 0;
                if (int.TryParse(EmpCode, out ecode))
                {
                    strQry += " where  e.RetirementDate>=getdate() and empid LIKE '" + EmpCode + "%' order by e.EmpId; ";
                }
                else
                {
                    strQry += " where  e.RetirementDate>=getdate() and  FirstName LIKE '%" + EmpCode + "%' OR LastName LIKE '%" + EmpCode + "%' order by e.EmpId; ";
                }

                dtEmpDetails = await _Sha.Get_Table_FromQry(strQry);
            }

            foreach (DataRow drEDet in dtEmpDetails.Rows)
            {
                retList.Add(new EmployeeSearchResult
                {
                    ECode = drEDet["empid"].ToString(),
                    EName = drEDet["Name"].ToString(),
                    EDesignation = drEDet["Designation"].ToString(),
                    EBranch = drEDet["deptbranch"].ToString(),
                    EDoj = drEDet["DOJ"].ToString(),
                    EDRetire = drEDet["RetirementDate"].ToString(),
                    EDoc = Convert.ToDateTime(drEDet["DOJ"].ToString()).AddMonths(12).ToString("dd.MM.yyyy"),
                });
            }

            return JsonConvert.SerializeObject(retList);

        }

        public async Task<string> GetScheduleTypeDetails()
        {
            try
            {
                var qryGetLoanType = "select * from All_Masters where active=1 and description='Loan Vendor Name'";

                DataSet dsGetLfields = await _Sha.Get_MultiTables_FromQry(qryGetLoanType);

                var dtLTfields = dsGetLfields.Tables[0];
                //var dtALfileds = dsGetLfields.Tables[1];
                var ltjson = JsonConvert.SerializeObject(dtLTfields);
                // var aljson = JsonConvert.SerializeObject(dtALfileds);
                ltjson = ltjson.Replace("null", "''");
                // aljson = aljson.Replace("null", "''");
                var javaScriptSerializer = new JavaScriptSerializer();
                var ltDetails = javaScriptSerializer.DeserializeObject(ltjson);
                var resultJson = javaScriptSerializer.Serialize(new { LTDetails = ltDetails });

                return JsonConvert.SerializeObject(resultJson);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "E#Error:#" + msg;
            }
        }

        public async Task<string> GetEmpData()
        {
            try
            {
                var qryGetLoanType = " select  CONCAT(e.FirstName,' ',e.LastName) as name ,convert(varchar,p.fm,105)as fm,d.name as designation " +
                    "from  pr_month_attendance p  join Employees e on e.empid=p.emp_code " +
                    "join Designations d on d.id = e.currentdesignation where p.status = 'StopSalary' and p.active=1; ";

                DataTable dsGetLfields = await _Sha.Get_Table_FromQry(qryGetLoanType);

                return JsonConvert.SerializeObject(dsGetLfields);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "E#Error:#" + msg;
            }
        }

        //NR Loan Emp search with out reteirment date
        public async Task<string> SearchEmployeeNRL(string EmpCode)
        {
            List<EmployeeSearchResult> retList = new List<EmployeeSearchResult>();


            string strQry = "select e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
            + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
            + " join Designations d on e.CurrentDesignation = d.Id"
            + " join Branches b on e.Branch = b.Id"
            + " join Departments dep on e.Department = dep.Id where empid =" + EmpCode + "";

            DataTable dtEmpDetails = null;
            try
            {
                dtEmpDetails = await _Sha.Get_Table_FromQry(strQry);
            }
            catch
            {
            }

            if (dtEmpDetails == null || dtEmpDetails.Rows.Count == 0)
            {
                strQry = "select e.empid, e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
                + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
                + " join Designations d on e.CurrentDesignation = d.Id"
                + " join Branches b on e.Branch = b.Id join Departments dep on e.Department = dep.Id";

                int ecode = 0;
                if (int.TryParse(EmpCode, out ecode))
                {
                    strQry += " where  e.RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) and empid LIKE '" + EmpCode + "%'; ";
                }
                else
                {
                    strQry += " where  e.RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) and  FirstName LIKE '%" + EmpCode + "%' OR LastName LIKE '%" + EmpCode + "%'; ";
                }

                dtEmpDetails = await _Sha.Get_Table_FromQry(strQry);
            }
            DateTime curdate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime lastday = new DateTime(_LoginCredential.FinancialMonthDate.Year, _LoginCredential.FinancialMonthDate.Month, 1).AddMonths(1).AddDays(-1);
            DateTime nextmonthfirstday = new DateTime(_LoginCredential.FinancialMonthDate.Year, _LoginCredential.FinancialMonthDate.Month, 1).AddMonths(1);
            //int curFY = _LoginCredential.FY;
            string[] arrFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd").Split('-');
            int curFY = Convert.ToInt32(arrFm[0]);
            int curFM = Convert.ToInt32(arrFm[1]);
            string[] arrFm1 = nextmonthfirstday.ToString("yyyy-MM-dd").Split('-');
            int curFY1 = Convert.ToInt32(arrFm1[0]);
            int curFM1 = Convert.ToInt32(arrFm1[1]);
            int curFd1 = Convert.ToInt32(arrFm1[2]);


            //int i_fy = Convert.ToInt32(FM);


            foreach (DataRow drEDet in dtEmpDetails.Rows)
            {
                DateTime retdate = Convert.ToDateTime(drEDet["RetirementDate"]);
                int reFm = retdate.Month;
                int retFy = retdate.Year;
                int retFd = retdate.Day;

                //if ((curFY > retFy && (curFM> reFm || curFM <= reFm))||(curFY==retFy && curFM >= reFm))
                //if (retdate < lastday)
                //{

                //    retList.Add(new EmployeeSearchResult
                //    {
                //        ECode = "",
                //        EName = "",
                //        EDesignation = "",
                //        EBranch = "",
                //        EDoj = "",
                //        EDRetire = "",
                //        EDoc = "",
                //    });
                //}

                //else
                //{
                    retList.Add(new EmployeeSearchResult
                    {
                        ECode = drEDet["empid"].ToString(),
                        EName = drEDet["Name"].ToString(),
                        EDesignation = drEDet["Designation"].ToString(),
                        EBranch = drEDet["deptbranch"].ToString(),
                        EDoj = drEDet["DOJ"].ToString(),
                        EDRetire = drEDet["RetirementDate"].ToString(),
                        EDoc = Convert.ToDateTime(drEDet["DOJ"].ToString()).AddMonths(12).ToString("dd-MM-yyyy"),
                    });
                //}


            }

            return JsonConvert.SerializeObject(retList);

        }
    }
}
