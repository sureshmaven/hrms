using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Entities;
using Repository;
using HRMSApplication.Models;
using HRMSApplication.Helpers;
using HRMSApplication.Filters;
using HRMSBusiness.Business;
using Mavensoft.Common;
using HRMSBusiness.Db;
using System.Configuration;
using iTextSharp.text;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class PLE_TypeController : Controller
    {
        LeavesBusiness lbus = new LeavesBusiness();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        private ContextBase db = new ContextBase();
     public ActionResult AdminPLApply()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            string year = DateTime.Now.Year.ToString();
            int yearcurrent = Convert.ToInt32(year);
            int previousyear = yearcurrent - 1;
            string prevyear = previousyear + "-" + year;
            List<string> listyear = new List<string>();
            listyear.Add("Select");
            listyear.Add(prevyear);
            listyear.Add(year);
            List<string> listyear1 = new List<string>();
            listyear1.Add("Select");
            listyear1.Add("15");
            listyear1.Add("30");
            ViewBag.plyear = listyear;
            ViewBag.pllist = listyear1;
            return View();
        }

        public ActionResult Index(string empcode)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            var items = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().Where(a => a.Type != "ALL").Select(x => new LeaveTypes
            {
                Id = x.Id,
                Type = x.Type.Trim(),
            });
            ViewBag.LeaveTypes = new SelectList(items, "Id", "Type");
            string shortname = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.ShortName).FirstOrDefault();

            int designation = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.CurrentDesignation).FirstOrDefault();
            string desig = db.Designations.Where(a => a.Id == designation).Select(a => a.Name).FirstOrDefault();
            int branch = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Branch).FirstOrDefault();
            string branchs = db.Branches.Where(a => a.Id == branch).Select(a => a.Name).FirstOrDefault();

            int dept = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Department).FirstOrDefault();
            string depts = db.Departments.Where(a => a.Id == dept).Select(a => a.Name).FirstOrDefault();

            if (branch == 43)
            {

                ViewBag.branchname = depts;

            }
            else
            {
                ViewBag.branchname = branchs;


            }
            if (TempData["AlertMessage"] != null)
            {
                lMessage = TempData["AlertMessage"].ToString();
            }
            ViewBag.Message = lCredentials.LoginMode;
            TempData["Loginmode"] = lCredentials.LoginMode;
            var lmodel = new ViewModel { Loginmode = lCredentials.LoginMode };
            return View(lmodel);
        }
        public static DateTime GetCurrentTime(DateTime ldate)
        {
            DateTime serverTime = DateTime.Now;
            DateTime utcTime = serverTime.ToUniversalTime();
            // convert it to Utc using timezone setting of server computer
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }
        public string GetFirstandLastName(string FirstName, string LastName)
        {
            string lfirstname = "";
            lfirstname = FirstName.Length.ToString();
            int lfirst = Convert.ToInt32(lfirstname);
            if (lfirst >= 3)
            {
                lfirstname = FirstName.Substring(0, 1);
            }
            lfirstname = lfirstname + " " + LastName;
            return lfirstname;
        }
        public string GetBranchDepartmentConcat(string branch, string Department)
        {
            string requireformate = "";
            if (branch != "OtherBranch" && branch != "TBD")
            {
                requireformate = branch;
            }
            if (Department != "OtherDepartment" && Department != "TBD")
            {
                requireformate = Department;
            }

            if (branch != null && Department != null)
            {

            }
            if (branch == "HeadOffice")
            {
                requireformate = requireformate + " " + Department;
            }
            else
            {
                requireformate = requireformate.Replace('/', ' ');
            }
            return requireformate;
        }
        public string GetsDates(DateTime ldate)
        {
            string ldates = "";
            DateTime d1 = ldate;
            ldates = d1.ToShortDateString().ToString() + " - " + d1.ToShortTimeString().ToString();
            return ldates;
        }
        public DateTime[] GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date.Date);
            return allDates.ToArray();
        }
        [HttpGet]
        public JsonResult GetEmployeeData(string empcode)
        {
            string year = DateTime.Now.Year.ToString();
            int yearcurrent = Convert.ToInt32(year);
            int previousyear = yearcurrent - 1;
            string prevyear = previousyear + "-" + year;
            List<string> listyear = new List<string>();
            listyear.Add("Select");
            listyear.Add(prevyear);
            listyear.Add(year);
            List<string> listyear1 = new List<string>();
            listyear1.Add("Select");
            listyear1.Add("15");
            listyear1.Add("30");
            ViewBag.plyear = listyear;
            ViewBag.pllist = listyear1;
            string branchs = "";
            string totalexperience = "";
            string lresult = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                // var employees = db.Employes.ToList();
                int count = db.Employes.Where(a => a.EmpId == empcode).Count();
                var lshortname = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.ShortName).FirstOrDefault();
                var ldesignation = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.CurrentDesignation).FirstOrDefault();
                string desig = db.Designations.Where(a => a.Id == ldesignation).Select(a => a.Name).FirstOrDefault();
                int branch = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Branch).FirstOrDefault();
                string branchss = db.Branches.Where(a => a.Id == branch).Select(a => a.Name).FirstOrDefault();

                int dept = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Department).FirstOrDefault();
                string depts = db.Departments.Where(a => a.Id == dept).Select(a => a.Name).FirstOrDefault();

                if (branch == 43)
                {

                   branchs = depts;

                }
                else if(branch != 43 && branch!=0)
                {
                    branchs = branchss;


                }
                
                    int lUserLoginId = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Id).FirstOrDefault();
                int lLeaveBalance = 0;
                string lleavetypes = "";
                if (count != 0)
                {
                    var lResult = (from userslist in db.Employes
                                   where userslist.Id == lUserLoginId
                                   select new
                                   {
                                       userslist.TotalExperience,
                                   });
                    var lresponseArray = lResult.ToArray();

                    totalexperience = lresponseArray[0].TotalExperience;


                    string ltotalexperience = totalexperience;

                    Session["ltotalexp"] = totalexperience;

                    var leaveid = db.LeaveTypes.Where(a => a.Type == "Privilege Leave").Select(a => a.Id).FirstOrDefault();
                    // int lUserLoginId = db.Employes.Where(a => a.EmpId.ToString() == empcode).Select(a => a.Id).FirstOrDefault();
                    var employees = db.EmpLeaveBalance.Where(a => a.EmpId == lUserLoginId).Where(a => a.LeaveTypeId == leaveid && a.Year == DateTime.Now.Year).ToList();
                    var lResults = (from userslist in employees
                                    join lleave in db.LeaveTypes on userslist.LeaveTypeId equals lleave.Id
                                    select new
                                    {
                                        userslist.LeaveBalance,
                                        leavetype = lleave.Type,


                                    });
                    var lresponseArrays = lResults.ToArray();
                     lLeaveBalance = lresponseArrays[0].LeaveBalance;
                     lleavetypes = lresponseArrays[0].leavetype;
                }
                string RetirementDate = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.RetirementDate.ToString()).FirstOrDefault();
                DateTime lrdatee = Convert.ToDateTime(RetirementDate).Date;
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                string lstatus1 = "";
                if (count == 0)
                {
                    lstatus1 = "Notfound";
                }
                else
                if (lrdatee < lStartDate)
                {
                    lstatus1 = "AlreadyRetired";
                }

                return Json(new { lshortnmaeAjax = lshortname, ldesigAjax = desig, lTotalexpAjax = totalexperience, lbranchAjax = branchs, lbalanceajax = lLeaveBalance, lbalancetypeajax = lleavetypes,Status = lstatus1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;
        }
        [HttpGet]
        public JsonResult GetAuthorityNamess(string Name)
        {
            string plcontroll = System.Configuration.ConfigurationManager.AppSettings["PlControlling"];
            string plsanction = System.Configuration.ConfigurationManager.AppSettings["PlSanctioning"];
            string lresult = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                var employees = db.Employes.ToList();
                var lcontrolname = db.Employes.Where(a => a.EmpId == plcontroll).Select(a => a.ShortName).FirstOrDefault();
                var lsanctionname = db.Employes.Where(a => a.EmpId == plsanction).Select(a => a.ShortName).FirstOrDefault();
                int lUserLoginId = employees.Where(a => a.EmpId.ToLower() == lCredentials.EmpId.ToLower()).Select(a => a.Id).FirstOrDefault();
                if (string.IsNullOrEmpty(Name))
                {
                    var lResult = (from userslist in employees
                                   where userslist.Id == lUserLoginId
                                   select new
                                   {
                                       userslist.TotalExperience,
                                   });
                    var lresponseArray = lResult.ToArray();
                    //string lControllingAuthority = lresponseArray[0].ControllingAuthority;
                    //string lSanctioningAuthority = lresponseArray[0].SanctioningAuthority;
                    string totalexperience = lresponseArray[0].TotalExperience;
                    //int lcontrol = Convert.ToInt32(lcontrolname);
                    //int lsancationcontrol = Convert.ToInt32(lsanctionname);
                    string ltotalexperience = totalexperience;
                    //Session["lcontrols"] = lcontrol;
                    //Session["lSancation"] = lsancationcontrol;
                    Session["ltotalexp"] = totalexperience;
                    //Employees lcontrolling = Facade.EntitiesFacade.GetEmpTabledata.GetById(lcontrol);
                    //Employees lsancationing = Facade.EntitiesFacade.GetEmpTabledata.GetById(lsancationcontrol);
                    // Employees ltotal = Facade.EntitiesFacade.GetEmpTabledata.GetById(totalexperience);
                    return Json(new { lControllingAuthorityAjax = lcontrolname, lSanctioningAuthorityAjax = lsanctionname, lTotalexpAjax = totalexperience }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;
        }
        [HttpGet]
        public JsonResult SelfPLApply(string StartDate)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
            try
            {
                var ldeputation = db.PLE_Type.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                if (lCredentials.LoginMode == "Employee")
                {
                    var lResult = (from otherduty in ldeputation
                                   join emp in lemployees on otherduty.EmpId equals emp.Id
                                   join branch in lBranches on otherduty.BranchId equals branch.Id
                                   join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                   join dept in Departments on otherduty.DepartmentId equals dept.Id
                                   where otherduty.EmpId == lEmpId
                                   select new
                                   {
                                       otherduty.Id,
                                       emp.EmpId,
                                       EmployeeName = emp.ShortName,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                       otherduty.Status,
                                       otherduty.UpdatedDate,
                                       otherduty.Reason,
                                       otherduty.Subject
                                   }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResult, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var lResult = (from otherduty in ldeputation
                                   join emp in lemployees on otherduty.EmpId equals emp.Id

                                   join branch in lBranches on otherduty.BranchId equals branch.Id
                                   join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                   join dept in Departments on otherduty.DepartmentId equals dept.Id

                                   where otherduty.EmpId == lEmpId
                                   select new
                                   {
                                       otherduty.Id,
                                       emp.EmpId,
                                       EmployeeName = emp.ShortName,
                                       otherduty.Status,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                       otherduty.UpdatedDate,
                                       otherduty.Reason,
                                       otherduty.Subject
                                   }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResult, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        public JsonResult checkCurrentyear(string EmpCode,string CurrYear)
        {
            string alowrnotyear1 = "";
            string alowrnotyear2= "";
            if (CurrYear.Contains('-'))
            {
                string[] arr = CurrYear.Split('-');
                alowrnotyear1 = arr[0];
                alowrnotyear2 = arr[1];
            }
            else
            {
                alowrnotyear1 = (Convert.ToInt32(CurrYear) - 1)  + "-" + CurrYear;
            }
            
            int count = 0;
            string year = DateTime.Now.Year.ToString();
            int yearcurrent = Convert.ToInt32(year);
            int previousyear = yearcurrent - 1;
            string prevyear = previousyear + "-" + year;
            int lEmpIds = db.Employes.Where(a => a.EmpId == EmpCode).Select(a => a.Id).FirstOrDefault();
            int plcountsyear1 = 0;
            int plcountsyear2 = 0;
            int plcounts = 0;
            if (alowrnotyear1 != "" && alowrnotyear2 != "")
            {
                plcountsyear1 = db.PLE_Type.Where(a => a.EmpId == lEmpIds && a.CurrentYear == alowrnotyear1).OrderByDescending(a => a.Id).Select(a => a.CurrentYear).Count();
                plcountsyear2 = db.PLE_Type.Where(a => a.EmpId == lEmpIds && a.CurrentYear == alowrnotyear2).OrderByDescending(a => a.Id).Select(a => a.CurrentYear).Count();
            }
            else
            {
                plcountsyear2 = db.PLE_Type.Where(a => a.EmpId == lEmpIds && a.CurrentYear == alowrnotyear1).OrderByDescending(a => a.Id).Select(a => a.CurrentYear).Count();
            }
            plcounts= db.PLE_Type.Where(a => a.EmpId == lEmpIds && a.CurrentYear == CurrYear).OrderByDescending(a => a.Id).Select(a => a.CurrentYear).Count();
            if (plcountsyear1 != 0)
            {
                plcounts = plcountsyear1;
            }
            if(plcountsyear2 !=0)
            {
                plcounts = plcountsyear2;
            }

            if (plcounts >0)
            {
                count = 1;
            }
           else if (CurrYear == prevyear)
            {
                List<string> lyear = new List<string>();
                lyear = CurrYear.Split(new char[] { '-' }).ToList();
                //string result = CurrYear.Substring(CurrYear.Length - 4);
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == EmpCode).Select(a => a.Id).FirstOrDefault();
                var empids = from u in db.PLE_Type
                             where u.CurrentYear.Equals(CurrYear) && u.EmpId == lEmpId
                             where u.Status != "Cancelled"
                             where (u.Status != "Denied")
                             select u;
              
                count = empids.Count();
            }
            else
            {
                List<string> lyear = new List<string>();
                lyear = CurrYear.Split(new char[] { '-' }).ToList();
                //string result = CurrYear.Substring(CurrYear.Length - 4);
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == EmpCode).Select(a => a.Id).FirstOrDefault();
                string plcount = db.PLE_Type.Where(a=>a.EmpId==lEmpId).OrderByDescending(a=>a.Id).Select(a => a.CurrentYear).FirstOrDefault();
                string plyear = "";
                if (prevyear == CurrYear)
                {
                    if (plcount != "")
                    {
                        char[] spearator = { '-', ' ' };
                        Int32 countc = 2;

                        String[] strlist = plcount.Split(spearator,
                           countc, StringSplitOptions.None);
                        plyear = strlist[1];
                    }
                }
                var empids = from u in db.PLE_Type
                                 where u.CurrentYear.Equals(CurrYear) && u.EmpId == lEmpId
                                 where u.Status != "Cancelled"
                                 where (u.Status != "Denied")
                                 select u;
                if (plyear == CurrYear)
                {
                    count = 1;
                }
                else if (plcounts>0)
                {
                    count = 1;
                }
                else
                {
                    count = empids.Count();
                }
                
            }
           //if(Convert.ToUInt32(CurrYear)<DateTime.Now.Year)
           // {
           //     return Json(new { message = "pused" }, JsonRequestBehavior.AllowGet);
           // }
            if (count == 0)
            {


                return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);


            }
            else
            {

                return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult checkCurrentencash(string EmpCode, string CurrYear, string encashpl)
        {

            string status = "";
            status = lbus.AddPlbalanceDetails(EmpCode,CurrYear,encashpl);
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == EmpCode).Select(a => a.Id).FirstOrDefault();
          

                return Json(new { message = status}, JsonRequestBehavior.AllowGet);
       
        }


        [HttpPost]
        public JsonResult Getencashdata(string yearcurr)
        {
            var pldata = new List<SelectListItem>();
            string year = DateTime.Now.Year.ToString();
            int yearcurrent = Convert.ToInt32(year);
            
            if (yearcurr == DateTime.Now.Year.ToString() && yearcurrent == Convert.ToInt32(yearcurr))
            {
                var list = new List<SelectListItem>();
                for (var i = 1; i < 16; i++)
                    list.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                pldata = list;
               
            }
            else
            {
                var list = new List<SelectListItem>();
                for (var i = 1; i < 31; i++)
                    list.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
               pldata = list;
            }
           
            return Json(pldata, JsonRequestBehavior.AllowGet);

        }

        //}
        [HttpPost]
        public ActionResult PLPost(ViewModel PL)
        {
            string[] payrollaccess = ConfigurationManager.AppSettings["PayrollAccessId"].Split(',');
            string fms="" ;
            int fys = 0001;
          SqlHelper sh = new SqlHelper();
            string query = " select fm, fy from pr_month_details where active = 1";
           DataTable dt= sh.Get_Table_FromQry(query);
            foreach(DataRow dr in dt.Rows)
            {
                 fms = Convert.ToDateTime(dr["fm"]).ToString("yyyy-MM-dd");
                 fys = Convert.ToInt32(dr["fy"]);
            }
            int leavebalance = 0;
            int lEmpId1 = db.Employes.Where(a => a.EmpId == PL.EmpId).Select(a => a.Id).FirstOrDefault();
            leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId1 && a.LeaveTypeId== 3 && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
            int plencah = Convert.ToInt32(PL.lpl.PLEncash);
           // var lcarryforward = db.Leaves_CarryForward.ToList();
            LoginCredential lcredentials = LoginHelper.GetCurrentUser();
         
            int lEmpId = db.Employes.Where(a => a.EmpId == lcredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            var lEmpLeaveBalance = db.EmpLeaveBalance.Where(a=>a.Year==DateTime.Now.Year).ToList();
            //int lcontrolling = Convert.ToInt32(Session["lcontrols"].ToString());
            //int lsanctioning = Convert.ToInt32(Session["lSancation"].ToString());
            DateTime? Retirement = db.Employes.Where(a => a.EmpId == PL.EmpId).Select(a => a.RetirementDate).FirstOrDefault();
            int Retirementyear = Retirement.Value.Year;
            string year = "";
            if (PL.lpl.CurrentYear == DateTime.Now.Year.ToString())
            {
                year = PL.lpl.CurrentYear;
            }
            else
            {
                string yearc = PL.lpl.CurrentYear;
                char[] spearator = { '-', ' ' };
                Int32 count = 2;
             
                String[] strlist = yearc.Split(spearator,
                   count, StringSplitOptions.None);
                year=strlist[1];
            }
                string prevyear=db.PLE_Type.Where(a => a.EmpId == lEmpId1).Select(a => a.CurrentYear).FirstOrDefault();
            int pyear = 0;
            int ayear = 0;
            string pryear = "";
            string afyear = "";
            if (prevyear != "")
            {
                 pyear = Convert.ToInt32(prevyear) +1;
                ayear = Convert.ToInt32(prevyear) - 1;
          
             pryear = pyear.ToString();
             afyear = ayear.ToString();
            }
            if (Retirementyear >= Convert.ToInt32(year))
            {
                if (PL.lpl.TotalPL == "0")
                {
                    TempData["AlertMessage"] = "NO PL available to apply";
                }
               else  if (PL.lpl.PLEncash == "0")
                {
                    TempData["AlertMessage"] = "Please enter Atleast 1 PL in PLEncash";
                }
                else if (Convert.ToInt32(PL.lpl.TotalPL) < Convert.ToInt32(PL.lpl.PLEncash))
                {
                    TempData["AlertMessage"] = "Please enter available PL";
                }
                else if (pryear == PL.lpl.CurrentYear.TrimStart() && PL.lpl.PLEncash == "30")
                {
                    TempData["AlertMessage"] = " Only 15 are allowed to encash this year";
                }
                else if (afyear == PL.lpl.CurrentYear.TrimStart() && PL.lpl.PLEncash == "30")
                {
                    TempData["AlertMessage"] = " Only 15 are allowed to encash this year";
                }
                //else if (prevyear != "" && PL.lpl.PLEncash == "30")
                //{
                //    TempData["AlertMessage"] = " Only 15 are allowed to encash this year";
                //}
                else
                {
                    //checking pl.lpl.fm is null or not
                    string strDate = "";
                    if (PL.lpl.fm is null)
                    {
                        strDate = fms;
                    }
                    else
                    {
                        strDate = PL.lpl.fm.ToString();
                    }
                    
                    string[] sa = strDate.Split('-');
                    string s1 = sa[0];
                    if (s1.StartsWith("0"))
                    {
                        s1 = "01";
                    }

                    string s2 = sa[1].ToString();
                    string s3 = sa[2].ToString();
                    string datefm = s1 + s2 + s3;
                   
                  //commented below fm and fy are using any where.
                    //string fm = DateTime.Parse(datefm).ToString("yyyy/MM/dd");
                    // int fy = Helper.getFinancialYear(Convert.ToDateTime(datefm));
                    
                    int lLeaveId = db.LeaveTypes.Where(a => a.Type == PL.lpl.LeaveType).Select(a => a.Id).FirstOrDefault();
                    string totalecperience = db.Employes.Where(a => a.Id == lEmpId1).Select(a => a.TotalExperience).FirstOrDefault();
                    PL.lpl.UpdatedBy = Convert.ToInt32(lcredentials.EmpId);
                    PL.lpl.EmpId = lEmpId1;
                    PL.lpl.UpdatedDate = GetCurrentTime(DateTime.Now);
                    PL.lpl.Status = "Approved";
                    PL.lpl.LeaveType = lLeaveId.ToString();
                    PL.lpl.CurrentYear = PL.lpl.CurrentYear;
                    string lcontrolid = db.Employes.Where(a => a.EmpId == PL.EmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                    string lsanctionid = db.Employes.Where(a => a.EmpId == PL.EmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                    PL.lpl.ControllingAuthority =Convert.ToInt32(lcontrolid);
                    PL.lpl.SanctioningAuthority = Convert.ToInt32(lsanctionid);
                    PL.lpl.TotalExperience = PL.TotalExperience;
                    PL.lpl.authorisation = 1;
                    PL.lpl.PLType = "Encashment";
                    PL.lpl.Reason = "PL Encashment";
                    PL.lpl.process = 0;
                    PL.lpl.fm = Convert.ToDateTime(fms);
                    PL.lpl.fy = fys;
                    PL.lpl.leave_balance = leavebalance;

                     int BranchId = db.Employes.Where(a => a.EmpId == PL.EmpId).Select(a => a.Branch).FirstOrDefault();
                    int DepartmentId = db.Employes.Where(a => a.EmpId == PL.EmpId).Select(a => a.Department).FirstOrDefault();
                    int DesignationId = db.Employes.Where(a => a.EmpId == PL.EmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
                    PL.lpl.BranchId = BranchId;
                    PL.lpl.DepartmentId = DepartmentId;
                    PL.lpl.DesignationId = DesignationId;
                    PL.lpl.PLEncash = plencah.ToString();
                    //if(PL.lpl.PLEncash.StartsWith("0"))
                    //{
                    //    PL.lpl.PLEncash.Replace("0","");
                    //}
                    //PL.lpl.PLEncash.TrimStart(new Char[] { '0' });
                    db.PLE_Type.Add(PL.lpl);
                    db.SaveChanges();
                    Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId1).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();

                    int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId1).Where(a => a.LeaveTypeId == lLeaveId).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();

                    int? previousbal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == lLeaveId).Where(a => a.Year == DateTime.Now.Year).Select(a => a.PreviousYearCF).FirstOrDefault();
                  
                    EmpLeaveBalance NewEmpbalance = lEmpLeaveBalance.Where(a => a.EmpId == lEmpId1).Where(a => a.LeaveTypeId == lLeaveId && a.Year == DateTime.Now.Year).FirstOrDefault();
                    NewEmpbalance.EmpId = lEmpId1;
                    NewEmpbalance.LeaveTypeId = lLeaveId;
                    int totalleavebalances = Convert.ToInt32(PL.lpl.TotalPL) - Convert.ToInt32(PL.lpl.PLEncash);
                    NewEmpbalance.LeaveBalance = totalleavebalances;
                    NewEmpbalance.Debits = NewEmpbalance.Debits + Convert.ToInt32(PL.lpl.PLEncash);
                    NewEmpbalance.UpdatedBy = lEmpId.ToString();
                    db.Entry(NewEmpbalance).State = EntityState.Modified;
                    db.SaveChanges();
                  
                    TempData["AlertMessage"] = "PL Applied Successfully";
                    string empstring = "";
                    TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                    string tem = TempData["RolePages"].ToString();
                    Mavensoft.DAL.Db.SqlHelper sha = new Mavensoft.DAL.Db.SqlHelper();
                    string qry = "select constant from all_constants where functionality='LoginAccess' and active=1";
                    DataTable constat = sha.Get_Table_FromQry(qry);
                    foreach (DataRow dr in constat.Rows)
                    {
                        empstring = dr["constant"].ToString();
                    }
                    if (empstring.Split(',').Contains(lcredentials.EmpId) && tem.Contains("RPYLB"))
                    {
                        return RedirectToAction("AdminPLApply");
                    }
                    else if (empstring.Split(',').Contains(lcredentials.EmpId))
                    {
                        return RedirectToAction("AdminPLApply/PayrollEncash");
                    }
                    else
                    {
                        return RedirectToAction("AdminPLApply");
                    }
                }
                return RedirectToAction("AdminPLApply");
            }
            else
            {
                TempData["AlertMessage"] = "The Selected Year should be less than or equal to the Retirement Year" + "  " + Retirement.Value.Year + "  " + "Please select other year.";
                string empstring = "";
                Mavensoft.DAL.Db.SqlHelper sha = new Mavensoft.DAL.Db.SqlHelper();
                string qry = "select constant from all_constants where functionality='LoginAccess' and active=1";
                DataTable constat = sha.Get_Table_FromQry(qry);
                foreach (DataRow dr in constat.Rows)
                {
                    empstring = dr["constant"].ToString();
                }
                if (empstring.Split(',').Contains(lcredentials.EmpId))
                {
                    return RedirectToAction("AdminPLApply/PayrollEncash");
                }
                else
                {
                    return RedirectToAction("AdminPLApply");
                }
            }
        }
        [HttpGet]
        public JsonResult PLView(string EmpId)
        {
            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var lResult = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var dResult = db.Designations.ToList();
                var plresult = db.PLE_Type.ToList();
                if (String.IsNullOrEmpty(EmpId))
                {
                    var data = (from employee in plresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                where emp.EmpId == lCredentails.EmpId
                                select new
                                {
                                    employee.CurrentYear,
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.PLType,
                                    employee.LeaveType,
                                    employee.TotalPL,
                                    employee.PLEncash,
                                }).OrderByDescending(a=>a.UpdatedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        [HttpPost]
        public JsonResult PLViews(string startdate, string enddate)
        {
            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var lResult = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var dResult = db.Designations.ToList();
                var plresult = db.PLE_Type.ToList();
                if (startdate == "" && enddate == "")
                {
                    var data = (from employee in plresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                where emp.EmpId == lCredentails.EmpId
                                select new
                                {
                                    employee.CurrentYear,
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.PLType,
                                    employee.LeaveType,
                                    employee.TotalPL,
                                    employee.PLEncash,
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (startdate != "" && enddate != "")
                {
                    var Sd = Convert.ToDateTime(startdate);
                    var Ed = Convert.ToDateTime(enddate);
                    var data = (from employee in plresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                where emp.EmpId == lCredentails.EmpId
                                select new
                                {
                                    employee.CurrentYear,
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.PLType,
                                    employee.LeaveType,
                                    employee.TotalPL,
                                    employee.PLEncash,
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        [HttpGet]
        public ActionResult Approvals()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            string lMessage = string.Empty;
            try
            {
                if (lCredentials.LoginMode == "SuperAdmin")
                {
                    PLE_Type lmodel = new PLE_Type();
                    lmodel.Loginmode = lCredentials.LoginMode;
                    ViewBag.Message = lCredentials.LoginMode;
                    TempData["Loginmode"] = lCredentials.LoginMode;
                    return View("PLApprovals");
                }
                else
                {
                    PLE_Type lmodel = new PLE_Type();
                    lmodel.Loginmode = lCredentials.LoginMode;
                    ViewBag.Message = lCredentials.LoginMode;
                    TempData["Loginmode"] = lCredentials.LoginMode;
                    return View("PLApprovals");
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return View(lMessage);
        }
        [HttpGet]
        public JsonResult PLApprovalViews(string status)
        {
            string lMessage = string.Empty;
            try
            {
                string plcontroll = System.Configuration.ConfigurationManager.AppSettings["PlControlling"];
                string plsanction = System.Configuration.ConfigurationManager.AppSettings["PlSanctioning"];
                var lEmployees = db.Employes.ToList();
                var lple = db.PLE_Type.ToList();
                var lBranches = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                var lleavetypes = db.LeaveTypes.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lplcontrol = Convert.ToInt32(plcontroll);
                int lplsanction = Convert.ToInt32(plsanction);
                int lplcid = db.Employes.Where(a => a.EmpId == plcontroll).Select(a => a.Id).FirstOrDefault();
                int lplsid = db.Employes.Where(a => a.EmpId == plsanction).Select(a => a.Id).FirstOrDefault();
                // int lControllingAuthority = db.PLE_Type.Where(a => a.ControllingAuthority == lplcontrol).Select(a => a.ControllingAuthority).FirstOrDefault();
                // int lSancationingAuthority = db.PLE_Type.Where(a => a.SanctioningAuthority == lplsanction).Select(a => a.SanctioningAuthority).FirstOrDefault();
                if (lEmpId == lplcid)
                {
                    var Duration = string.Empty;
                    var lResult = (from plencash in lple
                                   join emp in lEmployees on plencash.EmpId equals emp.Id
                                   join branch in lBranches on plencash.BranchId equals branch.Id
                                   join dept in ldept on plencash.DepartmentId equals dept.Id
                                   join desig in ldesignation on plencash.DesignationId equals desig.Id
                                   where lEmpId == lplcid && plencash.Status == "Pending" ||
                                  lEmpId == lplsid && plencash.Status == "Forwarded"
                                   //  where plencash.Status == "Pending"

                                   select new
                                   {
                                       plencash.Id,
                                       emp.EmpId,
                                       EmployeeName = emp.ShortName,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                       plencash.Status,
                                       plencash.UpdatedDate,
                                       plencash.PLEncash,
                                       plencash.TotalPL,
                                       plencash.Subject
                                   }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                }
                if (lEmpId == lplsid)
                {
                    var lResult = (from plencash in lple
                                   join emp in lEmployees on plencash.EmpId equals emp.Id
                                   join branch in lBranches on plencash.BranchId equals branch.Id
                                   join dept in ldept on plencash.DepartmentId equals dept.Id
                                   join desig in ldesignation on plencash.DesignationId equals desig.Id
                                   where lEmpId == lplsid && plencash.Status == "Forwarded"

                                   select new
                                   {
                                       plencash.Id,
                                       emp.EmpId,
                                       EmployeeName = emp.ShortName,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                       plencash.Status,
                                       plencash.UpdatedDate,
                                       plencash.PLEncash,
                                       plencash.TotalPL,
                                       plencash.Subject
                                   }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        [HttpPost]
        public JsonResult PLApprovalViews(string EmployeeCodey, string LeaveIds)
        {
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var ldbresult = db.PLE_Type.ToList();
                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeCodey))
                {
                    lCode = EmployeeCodey.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                List<string> lLeaveIds = new List<string>();
                if (!string.IsNullOrEmpty(LeaveIds))
                {
                    lLeaveIds = LeaveIds.Split(new char[] { ',' }).ToList();
                    lLeaveIds.Remove("");
                }
                int k = 0;
                for (int i = 0; i < lCode.Count; i++)
                {
                    for (; k < lLeaveIds.Count;)
                    {
                        string lECode = lCode[i];
                        string lIdss = lLeaveIds[k];
                        int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                        int LeaveId = Convert.ToInt32(lIdss);
                        string lstauts = db.PLE_Type.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                        if (lstauts == "Pending")
                        {
                            int leaverowid = Convert.ToInt32(lIdss);
                            PLE_Type lcontrolling = Facade.EntitiesFacade.GetPLTabledata.GetById(leaverowid);

                            PLE_Type lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatep.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                            lupdatep.Status = "Forwarded";
                            db.Entry(lupdatep).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["Forward"] = "PL Forwarded Successfully";
                        }
                        if (lstauts == "Forwarded")
                        {
                            int leaverowid = Convert.ToInt32(lIdss);
                            PLE_Type lSancationing = Facade.EntitiesFacade.GetPLTabledata.GetById(leaverowid);

                            PLE_Type lupdatef = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatef.Status = "Approved";
                            lupdatef.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                            db.Entry(lupdatef).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["Approve"] = "PL Approved Successfully";
                        }
                        k++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            string id = "";
            return Json(id = "jjj", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Cancel(string EmployeeCodey, string leaveTypes, string LeaveIds)
        {
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var lEmpBalance = db.EmpLeaveBalance.Where(a=>a.Year==DateTime.Now.Year).ToList();
                var ldbresult = db.PLE_Type.ToList();
                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeCodey))
                {
                    lCode = EmployeeCodey.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                List<string> lTypes = new List<string>();
                if (!string.IsNullOrEmpty(leaveTypes))
                {
                    lTypes = leaveTypes.Split(new char[] { ',' }).ToList();
                    lTypes.Remove("");
                }
                List<string> lLeaveIds = new List<string>();
                if (!string.IsNullOrEmpty(LeaveIds))
                {
                    lLeaveIds = LeaveIds.Split(new char[] { ',' }).ToList();
                    lLeaveIds.Remove("");
                }
                int j = 0;
                int k = 0;
                for (int i = 0; i < lCode.Count; i++)
                {
                    for (; j < lTypes.Count; j++)
                    {
                        for (; k < lLeaveIds.Count;)
                        {
                            string lType = lTypes[j];
                            string lECode = lCode[i];
                            string lIdss = lLeaveIds[k];
                            int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                            int LeaveId = Convert.ToInt32(lIdss);
                            int lleaveTypeIds = db.LeaveTypes.Where(a => a.Type == lType).Select(a => a.Id).FirstOrDefault();
                            string lstauts = db.PLE_Type.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                            string lleaveTypecode = db.LeaveTypes.Where(a => a.Code == lType).Select(a => a.Code).FirstOrDefault();
                            if (lstauts == "Pending")

                            {
                                PLE_Type lcontrolling = Facade.EntitiesFacade.GetPLTabledata.GetById(LeaveId);
                                string lcontrolstatus = "Cancelled";
                                string lcontrolvalue = "0";
                                PLE_Type lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Cancelled";
                                lupdatep.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                var leaveid = db.LeaveTypes.Where(a => a.Type == "Privilege Leave").Select(a => a.Id).FirstOrDefault();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == leaveid && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                string lLeaveDaystotal = db.PLE_Type.Where(a => a.EmpId == lId).Where(a => a.LeaveType == leaveid.ToString()).Where(a => a.Id == LeaveId).Select(a => a.PLEncash).FirstOrDefault();

                                int TotalDays = lEmpLeaveBalancetotal + Convert.ToInt32(lLeaveDaystotal);

                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == leaveid).FirstOrDefault();
                                lbalance.LeaveTypeId = leaveid;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - Convert.ToInt32(lLeaveDaystotal);
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["cancel"] = "PL Cancelled Successfully";
                            }
                            else if (lstauts == "Forwarded")
                            {
                                PLE_Type lSancationing = Facade.EntitiesFacade.GetPLTabledata.GetById(LeaveId);
                                string llSancationingstatus = "Cancelled";
                                string llSancationingvalue = "1";
                                PLE_Type lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Cancelled";
                                lupdatep.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                var leaveid = db.LeaveTypes.Where(a => a.Type == "Privilege Leave").Select(a => a.Id).FirstOrDefault();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == leaveid && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                string lLeaveDaystotal = db.PLE_Type.Where(a => a.EmpId == lId).Where(a => a.LeaveType == leaveid.ToString()).Where(a => a.Id == LeaveId).Select(a => a.PLEncash).FirstOrDefault();

                                int TotalDays = lEmpLeaveBalancetotal + Convert.ToInt32(lLeaveDaystotal);
                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == leaveid && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = leaveid;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - Convert.ToInt32(lLeaveDaystotal);
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["cancel"] = "PL Cancelled Successfully";
                            }
                            k++;
                            j++;
                            break;
                        }

                        break;

                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            string id = "";
            return Json(id = "jjj", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTotalPLBalance(string empcode)
        {

            string lresult = string.Empty;
            try
            {
                var lleavetype = db.LeaveTypes.ToList();
                var leaveid = db.LeaveTypes.Where(a => a.Type == "Privilege Leave").Select(a => a.Id).FirstOrDefault();
                int lUserLoginId = db.Employes.Where(a => a.EmpId.ToString() == empcode).Select(a => a.Id).FirstOrDefault();
                var employees = db.EmpLeaveBalance.Where(a => a.EmpId == lUserLoginId).Where(a => a.LeaveTypeId == leaveid && a.Year == DateTime.Now.Year).ToList();
                 var lResult = (from userslist in employees
                                   join lleave in lleavetype on userslist.LeaveTypeId equals lleave.Id
                                   select new
                                   {
                                       userslist.LeaveBalance,
                                       leavetype = lleave.Type,


                                   });
                    var lresponseArray = lResult.ToArray();
                    int lLeaveBalance = lresponseArray[0].LeaveBalance;
                    string lleavetypes = lresponseArray[0].leavetype;

                    //int ltotalexperience = Convert.ToInt32(totalexperience);
                    //  Session["lcontrols"] = lcontrol;   
                    return Json(new { lbalanceajax = lLeaveBalance, lbalancetypeajax = lleavetypes }, JsonRequestBehavior.AllowGet);

                
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;
        }
        public JsonResult GetTotalPLBalance1(string Name)
        {

            string lresult = string.Empty;
            try
            {
                var lleavetype = db.LeaveTypes.ToList();
                var leaveid = db.LeaveTypes.Where(a => a.Type == "Privilege Leave").Select(a => a.Id).FirstOrDefault();
                int lUserLoginId = db.Employes.Where(a => a.EmpId.ToString() == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var employees = db.EmpLeaveBalance.Where(a => a.EmpId == lUserLoginId).Where(a => a.LeaveTypeId == leaveid && a.Year == DateTime.Now.Year).ToList();
                var lResult = (from userslist in employees
                               join lleave in lleavetype on userslist.LeaveTypeId equals lleave.Id
                               select new
                               {
                                   
                                   leavetype = lleave.Type,


                               });
                var lresponseArray = lResult.ToArray();
            
                string lleavetypes = lresponseArray[0].leavetype;

                //int ltotalexperience = Convert.ToInt32(totalexperience);
                //  Session["lcontrols"] = lcontrol;   
                return Json(new {lbalancetypeajax = lleavetypes }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;
        }
        [HttpPost]
        public JsonResult Deny(string EmployeeCodey, string leaveTypes, string LeaveIds)
        {
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var lEmpBalance = db.EmpLeaveBalance.Where(a=>a.Year==DateTime.Now.Year).ToList();
                var ldbresult = db.PLE_Type.ToList();
                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeCodey))
                {
                    lCode = EmployeeCodey.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                List<string> lTypes = new List<string>();
                if (!string.IsNullOrEmpty(leaveTypes))
                {
                    lTypes = leaveTypes.Split(new char[] { ',' }).ToList();
                    lTypes.Remove("");
                }
                List<string> lLeaveIds = new List<string>();
                if (!string.IsNullOrEmpty(LeaveIds))
                {
                    lLeaveIds = LeaveIds.Split(new char[] { ',' }).ToList();
                    lLeaveIds.Remove("");
                }
                int j = 0;
                int k = 0;
                for (int i = 0; i < lCode.Count; i++)
                {
                    for (; j < lTypes.Count; j++)
                    {
                        for (; k < lLeaveIds.Count;)
                        {

                            string lECode = lCode[i];
                            string lType = lTypes[j];
                            string lIdss = lLeaveIds[k];
                            int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                            int LeaveId = Convert.ToInt32(lIdss);
                            int lleaveTypeIds = db.LeaveTypes.Where(a => a.Type == lType).Select(a => a.Id).FirstOrDefault();
                            string lstauts = db.PLE_Type.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                            if (lstauts == "Pending")
                            {
                                PLE_Type lcontrolling = Facade.EntitiesFacade.GetPLTabledata.GetById(LeaveId);
                                string lcontrolstatus = "Denied";
                                string lcontrolvalue = "0";
                                PLE_Type lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Denied";
                                lupdatep.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                var leaveid = db.LeaveTypes.Where(a => a.Type == "Privilege Leave").Select(a => a.Id).FirstOrDefault();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == leaveid && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                string lLeaveDaystotal = db.PLE_Type.Where(a => a.EmpId == lId).Where(a => a.LeaveType == leaveid.ToString()).Where(a => a.Id == LeaveId ).Select(a => a.PLEncash).FirstOrDefault();
                                int TotalDays = lEmpLeaveBalancetotal + Convert.ToInt32(lLeaveDaystotal);
                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == leaveid && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = leaveid;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - Convert.ToInt32(lLeaveDaystotal);
                                // lbalance.Credits = lbalance.Credits + Convert.ToInt32(lLeaveDaystotal);
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["Denied"] = "Leave Denied Successfully";
                            }
                            else if (lstauts == "Forwarded")
                            {
                                PLE_Type lSancationing = Facade.EntitiesFacade.GetPLTabledata.GetById(LeaveId);
                                string llSancationingstatus = "Denied";
                                string llSancationingvalue = "1";
                                PLE_Type lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Denied";
                                lupdatep.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                var leaveid = db.LeaveTypes.Where(a => a.Type == "Privilege Leave").Select(a => a.Id).FirstOrDefault();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == leaveid && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                string lLeaveDaystotal = db.PLE_Type.Where(a => a.EmpId == lId).Where(a => a.LeaveType == leaveid.ToString()).Where(a => a.Id == LeaveId).Select(a => a.PLEncash).FirstOrDefault();

                                int TotalDays = lEmpLeaveBalancetotal + Convert.ToInt32(lLeaveDaystotal);
                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == leaveid && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = leaveid;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - Convert.ToInt32(lLeaveDaystotal);
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["Denied"] = "PL Denied Successfully";
                            }
                            k++;
                            j++;
                            break;
                        }

                        break;

                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            string id = "";
            return Json(id = "jjj", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PLTooltip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var lEmployees = db.Employes.ToList();
            var lOtherduty = db.PLE_Type.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var lodmaster = db.OD_Master.ToList();
            var ldesignation = db.Designations.ToList();
            var lleaveTypes = db.LeaveTypes.ToList();
            var bp = db.BlockPeriod.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int OderrowId = Convert.ToInt32(EmployeeCodey);
            try
            {
                var lResult = (from duty in lOtherduty
                               join employee in lEmployees on duty.EmpId equals employee.Id

                               join branches in lBranches on duty.BranchId equals branches.Id
                               join dept in ldept on duty.DepartmentId equals dept.Id
                               join desig in ldesignation on duty.DesignationId equals desig.Id
                               join leavetypes in lleaveTypes on Convert.ToInt32(duty.LeaveType) equals leavetypes.Id
                               where duty.Id == OderrowId
                               select new
                               {
                                   duty.Id,
                                   employee.EmpId,
                                   EmployeeName = employee.ShortName,
                                   employee.ShortName,
                                   designation = desig.Code,
                                   AppliedDate = duty.UpdatedDate.Value.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                   duty.Status,
                                   duty.TotalExperience,
                                   duty.TotalPL,
                                   duty.PLEncash,
                                   duty.CurrentYear,
                                   leavetypes.Code,
                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = OderrowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                string lVisitingfrom = lresponseArray[0].TotalExperience;
                string Vistingto = lresponseArray[0].Code;
                string ODStartDate = lresponseArray[0].TotalPL;
                string ODEndDate = lresponseArray[0].PLEncash;
                string ODDuration = lresponseArray[0].CurrentYear;
                //string ODPurpose = lresponseArray[0].Purpose;
                string ODStatus = lresponseArray[0].Status;
                return Json(new
                {
                    lEmployeeId = employeeId,
                    lEmployeeName = employeeName,
                    ldeptbranch = Deptbranchs,
                    ldesig = ldesig12,
                    lvisitfrom = lVisitingfrom,
                    lvisitto = Vistingto,
                    lstartdate = ODStartDate,
                    lenddate = ODEndDate,
                    lduration = ODDuration,
                    //lpurpose = ODPurpose,
                    lstatus = ODStatus
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }
        [HttpGet]
        public JsonResult PLNexttip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var lEmployees = db.Employes.ToList();
            var lOtherduty = db.PLE_Type.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var lodmaster = db.OD_Master.ToList();
            var ldesignation = db.Designations.ToList();
            var lleaveTypes = db.LeaveTypes.ToList();
            var bp = db.BlockPeriod.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int OderrowId = Convert.ToInt32(EmployeeCodey);
            try
            {
                var lResult = (from duty in lOtherduty
                               join employee in lEmployees on duty.EmpId equals employee.Id
                               join branches in lBranches on duty.BranchId equals branches.Id
                               join dept in ldept on duty.DepartmentId equals dept.Id
                               join desig in ldesignation on duty.DesignationId equals desig.Id
                               join leavetypes in lleaveTypes on Convert.ToInt32(duty.LeaveType) equals leavetypes.Id
                               where duty.Id == OderrowId
                               select new
                               {
                                   duty.Id,
                                   employee.EmpId,
                                   EmployeeName = employee.ShortName,
                                   employee.ShortName,
                                   designation = desig.Code,
                                   AppliedDate = duty.UpdatedDate.Value.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                   duty.Status,
                                   duty.TotalExperience,
                                   duty.TotalPL,
                                   duty.PLEncash,
                                   duty.CurrentYear,
                                   leavetypes.Code,
                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = OderrowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                string lVisitingfrom = lresponseArray[0].TotalExperience;
                string Vistingto = lresponseArray[0].Code;
                string ODStartDate = lresponseArray[0].TotalPL;
                string ODEndDate = lresponseArray[0].PLEncash;
                string ODDuration = lresponseArray[0].CurrentYear;
                //string ODPurpose = lresponseArray[0].Purpose;
                string ODStatus = lresponseArray[0].Status;
                return Json(new
                {
                    lEmployeeId = employeeId,
                    lEmployeeName = employeeName,
                    ldeptbranch = Deptbranchs,
                    ldesig = ldesig12,
                    lvisitfrom = lVisitingfrom,
                    lvisitto = Vistingto,
                    lstartdate = ODStartDate,
                    lenddate = ODEndDate,
                    lduration = ODDuration,
                    //lpurpose = ODPurpose,
                    lstatus = ODStatus
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }
        [HttpGet]
        public JsonResult PLPrevioustip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var lEmployees = db.Employes.ToList();
            var lOtherduty = db.PLE_Type.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var lodmaster = db.OD_Master.ToList();
            var ldesignation = db.Designations.ToList();
            var lleaveTypes = db.LeaveTypes.ToList();
            var bp = db.BlockPeriod.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int OderrowId = Convert.ToInt32(EmployeeCodey);
            try
            {
                var lResult = (from duty in lOtherduty
                               join employee in lEmployees on duty.EmpId equals employee.Id
                               join branches in lBranches on duty.BranchId equals branches.Id
                               join dept in ldept on duty.DepartmentId equals dept.Id
                               join desig in ldesignation on duty.DesignationId equals desig.Id

                               join leavetypes in lleaveTypes on Convert.ToInt32(duty.LeaveType) equals leavetypes.Id
                               where duty.Id == OderrowId
                               select new
                               {
                                   duty.Id,
                                   employee.EmpId,
                                   EmployeeName = employee.ShortName,
                                   employee.ShortName,
                                   designation = desig.Code,
                                   AppliedDate = duty.UpdatedDate.Value.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                   duty.Status,
                                   duty.TotalExperience,
                                   duty.TotalPL,
                                   duty.PLEncash,
                                   duty.CurrentYear,
                                   leavetypes.Code,
                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = OderrowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                string lVisitingfrom = lresponseArray[0].TotalExperience;
                string Vistingto = lresponseArray[0].Code;
                string ODStartDate = lresponseArray[0].TotalPL;
                string ODEndDate = lresponseArray[0].PLEncash;
                string ODDuration = lresponseArray[0].CurrentYear;
                //string ODPurpose = lresponseArray[0].Purpose;
                string ODStatus = lresponseArray[0].Status;
                return Json(new
                {
                    lEmployeeId = employeeId,
                    lEmployeeName = employeeName,
                    ldeptbranch = Deptbranchs,
                    ldesig = ldesig12,
                    lvisitfrom = lVisitingfrom,
                    lvisitto = Vistingto,
                    lstartdate = ODStartDate,
                    lenddate = ODEndDate,
                    lduration = ODDuration,
                    //lpurpose = ODPurpose,
                    lstatus = ODStatus
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }
        public JsonResult Plapprovalsearch(string branch, string EmpId)
        {
            try
            {
                string lMessage = string.Empty;
                if (EmpId == "" && branch == "")
                {
                    var lEmployees = db.Employes.ToList();
                    var lple = db.PLE_Type.ToList();
                    var lBranches = db.Branches.ToList();
                    var ldept = db.Departments.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lleavetypes = db.LeaveTypes.ToList();
                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                    string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                    int lControllingAuthority = db.PLE_Type.Where(a => a.ControllingAuthority == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                    int lSancationingAuthority = db.PLE_Type.Where(a => a.SanctioningAuthority == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                    var Duration = string.Empty;
                    var lResult = (from plencash in lple
                                   join emp in lEmployees on plencash.EmpId equals emp.Id
                                   join branches in lBranches on plencash.BranchId equals branches.Id
                                   join dept in ldept on plencash.DepartmentId equals dept.Id
                                   join desig in ldesignation on plencash.DesignationId equals desig.Id
                                   where plencash.ControllingAuthority == lControllingAuthority && plencash.Status == "Pending" ||
                                   plencash.SanctioningAuthority == lSancationingAuthority && plencash.Status == "Forwarded"
                                   select new
                                   {
                                       plencash.Id,
                                       emp.EmpId,
                                       EmployeeName = emp.ShortName,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       plencash.Status,
                                       plencash.UpdatedDate,
                                       plencash.PLEncash,
                                       plencash.TotalPL,
                                       plencash.Subject
                                   }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                }
                else if (EmpId != "" && branch == "")
                {
                    var lEmployees = db.Employes.ToList();
                    var lple = db.PLE_Type.ToList();
                    var lBranches = db.Branches.ToList();
                    var ldept = db.Departments.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lleavetypes = db.LeaveTypes.ToList();
                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                    string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                    int lControllingAuthority = db.PLE_Type.Where(a => a.ControllingAuthority == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                    int lSancationingAuthority = db.PLE_Type.Where(a => a.SanctioningAuthority == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                    var Duration = string.Empty;
                    var lResult = (from plencash in lple
                                   join emp in lEmployees on plencash.EmpId equals emp.Id
                                   join branches in lBranches on plencash.BranchId equals branches.Id
                                   join dept in ldept on plencash.DepartmentId equals dept.Id
                                   join desig in ldesignation on plencash.DesignationId equals desig.Id

                                   where plencash.ControllingAuthority == lControllingAuthority && plencash.Status == "Pending" ||
                                   plencash.SanctioningAuthority == lSancationingAuthority && plencash.Status == "Forwarded"
                                   where emp.EmpId == EmpId
                                   select new
                                   {
                                       plencash.Id,
                                       emp.EmpId,
                                       EmployeeName = emp.ShortName,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       plencash.Status,
                                       plencash.UpdatedDate,
                                       plencash.PLEncash,
                                       plencash.TotalPL,
                                       plencash.Subject
                                   }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                }
                else if (EmpId == "" && branch != "")
                {
                    DateTime lStartDate = Convert.ToDateTime(branch);
                    var lEmployees = db.Employes.ToList();
                    var lple = db.PLE_Type.ToList();
                    var lBranches = db.Branches.ToList();
                    var ldept = db.Departments.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lleavetypes = db.LeaveTypes.ToList();
                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                    string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                    int lControllingAuthority = db.PLE_Type.Where(a => a.ControllingAuthority == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                    int lSancationingAuthority = db.PLE_Type.Where(a => a.SanctioningAuthority == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                    var Duration = string.Empty;
                    var lResult = (from plencash in lple
                                   join emp in lEmployees on plencash.EmpId equals emp.Id
                                   join branches in lBranches on plencash.BranchId equals branches.Id
                                   join dept in ldept on plencash.DepartmentId equals dept.Id
                                   join desig in ldesignation on plencash.DesignationId equals desig.Id
                                   where plencash.ControllingAuthority == lControllingAuthority && plencash.Status == "Pending" ||
                                   plencash.SanctioningAuthority == lSancationingAuthority && plencash.Status == "Forwarded"
                                   where plencash.UpdatedDate.Value.Date == lStartDate
                                   select new
                                   {
                                       plencash.Id,
                                       emp.EmpId,
                                       EmployeeName = emp.ShortName,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       plencash.Status,
                                       plencash.UpdatedDate,
                                       plencash.PLEncash,
                                       plencash.TotalPL,
                                       plencash.Subject
                                   }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                }
                else if (EmpId != "" && branch != "")
                {
                    DateTime lStartDate = Convert.ToDateTime(branch);
                    var lEmployees = db.Employes.ToList();
                    var lple = db.PLE_Type.ToList();
                    var lBranches = db.Branches.ToList();
                    var ldept = db.Departments.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lleavetypes = db.LeaveTypes.ToList();
                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                    string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                    int lControllingAuthority = db.PLE_Type.Where(a => a.ControllingAuthority == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                    int lSancationingAuthority = db.PLE_Type.Where(a => a.SanctioningAuthority == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                    var Duration = string.Empty;
                    var lResult = (from plencash in lple
                                   join emp in lEmployees on plencash.EmpId equals emp.Id
                                   join branches in lBranches on plencash.BranchId equals branches.Id
                                   join dept in ldept on plencash.DepartmentId equals dept.Id
                                   join desig in ldesignation on plencash.DesignationId equals desig.Id
                                   where plencash.ControllingAuthority == lControllingAuthority && plencash.Status == "Pending" ||
                                   plencash.SanctioningAuthority == lSancationingAuthority && plencash.Status == "Forwarded"
                                   where plencash.UpdatedDate.Value.Date == lStartDate
                                   where emp.EmpId == EmpId
                                   select new
                                   {
                                       plencash.Id,
                                       emp.EmpId,
                                       EmployeeName = emp.ShortName,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       plencash.Status,
                                       plencash.UpdatedDate,
                                       plencash.PLEncash,
                                       plencash.TotalPL,
                                       plencash.Subject
                                   }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        public JsonResult Plempsearch(string applieddate, string year)
        {
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var lResult = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var dResult = db.Designations.ToList();
            var plresult = db.PLE_Type.ToList();
            if (applieddate == "" && year == "")
            {
                var data = (from employee in plresult
                            join emp in lResult on employee.EmpId equals emp.Id
                            join desig in dResult on employee.DesignationId equals desig.Id
                            join branchs in lbranch on employee.BranchId equals branchs.Id
                            join depart in ldepartments on employee.DepartmentId equals depart.Id
                            where emp.EmpId == lCredentails.EmpId
                            select new
                            {
                                employee.CurrentYear,
                                emp.EmpId,
                                emp.ShortName,
                                desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                employee.UpdatedDate,
                                employee.Subject,
                                employee.TotalExperience,
                                employee.Status,
                                employee.PLType,
                                employee.LeaveType,
                                employee.TotalPL,
                                employee.PLEncash,
                            }).OrderByDescending(a=>a.UpdatedDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (applieddate != "" && year == "")
            {
                var data = (from employee in plresult
                            join emp in lResult on employee.EmpId equals emp.Id
                            join desig in dResult on employee.DesignationId equals desig.Id
                            join branchs in lbranch on employee.BranchId equals branchs.Id
                            join depart in ldepartments on employee.DepartmentId equals depart.Id
                            where emp.EmpId == lCredentails.EmpId
                            where employee.UpdatedDate.Value.Date == Convert.ToDateTime(applieddate)
                            select new
                            {
                                employee.CurrentYear,
                                emp.EmpId,
                                emp.ShortName,
                                desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                employee.UpdatedDate,
                                employee.Subject,
                                employee.TotalExperience,
                                employee.Status,
                                employee.PLType,
                                employee.LeaveType,
                                employee.TotalPL,
                                employee.PLEncash,
                            }).OrderByDescending(a => a.UpdatedDate); 
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (applieddate == "" && year != "")
            {
                var data = (from employee in plresult
                            join emp in lResult on employee.EmpId equals emp.Id
                            join desig in dResult on employee.DesignationId equals desig.Id
                            join branchs in lbranch on employee.BranchId equals branchs.Id
                            join depart in ldepartments on employee.DepartmentId equals depart.Id
                            where emp.EmpId == lCredentails.EmpId
                            where employee.CurrentYear.Contains(year)
                            select new
                            {
                                employee.CurrentYear,
                                emp.EmpId,
                                emp.ShortName,
                                desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                employee.UpdatedDate,
                                employee.Subject,
                                employee.TotalExperience,
                                employee.Status,
                                employee.PLType,
                                employee.LeaveType,
                                employee.TotalPL,
                                employee.PLEncash,
                            }).OrderByDescending(a => a.UpdatedDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (applieddate != "" && year != "")
            {
                var data = (from employee in plresult
                            join emp in lResult on employee.EmpId equals emp.Id
                            join desig in dResult on employee.DesignationId equals desig.Id
                            join branchs in lbranch on employee.BranchId equals branchs.Id
                            join depart in ldepartments on employee.DepartmentId equals depart.Id
                            where emp.EmpId == lCredentails.EmpId
                            where employee.UpdatedDate.Value.Date == Convert.ToDateTime(applieddate)
                            where employee.CurrentYear.Contains(year)
                            select new
                            {
                                employee.CurrentYear,
                                emp.EmpId,
                                emp.ShortName,
                                desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                employee.UpdatedDate,
                                employee.Subject,
                                employee.TotalExperience,
                                employee.Status,
                                employee.PLType,
                                employee.LeaveType,
                                employee.TotalPL,
                                employee.PLEncash,
                            }).OrderByDescending(a => a.UpdatedDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
    }
}

