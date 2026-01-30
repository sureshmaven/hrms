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
using HRMSBusiness.Db;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class LeaveTravelController : Controller
    {
        private ContextBase db = new ContextBase();
        public ActionResult AdminLTCApply(string empcode)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            int lEmpId = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Id).FirstOrDefault();
            int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
            string lcode1 = db.Designations.Where(a => a.Id == designation).Select(a => a.Name).FirstOrDefault();
            string lMessage = string.Empty;
            var items1 = Facade.EntitiesFacade.GetAllblockperiod().Select(x => new BlockPeriod
            {
                Id = x.Id,
                Block_Period = x.Block_Period,
            });
            ViewBag.Blockperiod = new SelectList(items1, "Id", "Block_Period");
          
            var items = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().Where(a => a.Code == "CL" || a.Code == "PL").Select(x => new LeaveTypes
            {
                Id = x.Id,
                Type = x.Type.Trim(),
            });
            ViewBag.LeaveTypes = new SelectList(items, "Id", "Type");
            if (TempData["AlertMessage"] != null)
            {
                lMessage = TempData["AlertMessage"].ToString();
            }
            ViewBag.Message = lCredentials.LoginMode;
            TempData["Loginmode"] = lCredentials.LoginMode;
            var lEmpleaveBalanceList = db.V_EmpLeaveBalance.Where(a => a.EmpId == lEmpId).ToList();
            var lmodel = new ViewModel { Loginmode = lCredentials.LoginMode, EmpId = empcode, };
            V_EmpLeaveBalance lbalance = new V_EmpLeaveBalance();
            lbalance.GetAllLeavesTypes = lEmpleaveBalanceList;
            lmodel.lEmpLeaveBal = lbalance;
            lmodel.designation = lcode1;
            return View(lmodel);
        }
      
        // GET: LeaveTravel
        public ActionResult Index()
        {

            
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
            string lcode1 = db.Designations.Where(a => a.Id == designation).Select(a => a.Name).FirstOrDefault();
            string lMessage = string.Empty;
            var items1 = Facade.EntitiesFacade.GetAllblockperiod().Select(x => new BlockPeriod
            {
                Id = x.Id,
                Block_Period = x.Block_Period,
            });
            ViewBag.Blockperiod = new SelectList(items1, "Id", "Block_Period");
            //if (lcode1 == "Attender" || lcode1 == "Driver" || lcode1 == "SA" || lcode1 == "Attender-Watchman" || lcode1 == "Attender/J.C" || lcode1 == "Watchman" || lcode1 == "JR-SA" || lcode1 == "SA-Assistant Cashier")
            //{
            //    var items = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().Where(a => a.Code == "CL" || a.Code == "PL" || a.Code == "C-OFF").Select(x => new LeaveTypes
            //    {
            //        Id = x.Id,
            //        Type = x.Type.Trim(),
            //    });
            //    ViewBag.LeaveTypes = new SelectList(items, "Id", "Type");

            //}
            //else
            //{
            //    var items = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().Where(a => a.Code == "CL" || a.Code == "PL").Select(x => new LeaveTypes
            //    {
            //        Id = x.Id,
            //        Type = x.Type.Trim(),
            //    });
            //    ViewBag.LeaveTypes = new SelectList(items, "Id", "Type");
            //}
            var items = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().Where(a => a.Code == "CL" || a.Code == "PL").Select(x => new LeaveTypes
            {
                Id = x.Id,
                Type = x.Type.Trim(),
            });
            ViewBag.LeaveTypes = new SelectList(items, "Id", "Type");
            if (TempData["AlertMessage"] != null)
            {
                lMessage = TempData["AlertMessage"].ToString();
            }
            ViewBag.Message = lCredentials.LoginMode;
            TempData["Loginmode"] = lCredentials.LoginMode;
            var lEmpleaveBalanceList = db.V_EmpLeaveBalance.Where(a => a.EmpId == lEmpId).ToList();
            var lmodel = new ViewModel { Loginmode = lCredentials.LoginMode, EmpId = lCredentials.EmpId, };
            V_EmpLeaveBalance lbalance = new V_EmpLeaveBalance();
            lbalance.GetAllLeavesTypes = lEmpleaveBalanceList;
            lmodel.lEmpLeaveBal = lbalance;
            lmodel.designation = lcode1;
            return View(lmodel);
        }
        [HttpGet]
        public JsonResult GetEmployeeData(string empcode)
        {
            int lclbal = 0;
            int lPlbal = 0;
            string branchs = "";
            string totalexperience = "";
            string lresult = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                var employees = db.Employes.ToList();
                int id = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Id).FirstOrDefault();
                var lshortname = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.ShortName).FirstOrDefault();
                var ldesignation = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.CurrentDesignation).FirstOrDefault();
                string desig = db.Designations.Where(a => a.Id == ldesignation).Select(a => a.Name).FirstOrDefault();
                int branch = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Branch).FirstOrDefault();
                string branchss = db.Branches.Where(a => a.Id == branch).Select(a => a.Name).FirstOrDefault();

                int dept = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Department).FirstOrDefault();
                string depts = db.Departments.Where(a => a.Id == dept).Select(a => a.Name).FirstOrDefault();
                 lclbal = db.EmpLeaveBalance.Where(a => a.EmpId== id && a.LeaveTypeId == 1 && a.Year == DateTime.Today.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                lPlbal = db.EmpLeaveBalance.Where(a => a.EmpId== id && a.LeaveTypeId == 3 && a.Year == DateTime.Today.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                if (branch == 43)
                {

                    branchs = depts;

                }
                else
                {
                    branchs = branchss;


                }
                int lUserLoginId = employees.Where(a => a.EmpId == empcode).Select(a => a.Id).FirstOrDefault();

                var lResult = (from userslist in employees
                               where userslist.Id == lUserLoginId
                               select new
                               {
                                   userslist.TotalExperience,
                               });
                var lresponseArray = lResult.ToArray();

                totalexperience = lresponseArray[0].TotalExperience;

                string ltotalexperience = totalexperience;

                Session["ltotalexp"] = totalexperience;




                return Json(new { lshortnmaeAjax = lshortname, ldesigAjax = desig, lTotalexpAjax = totalexperience, lbranchAjax = branchs,lcasual= lclbal,lprivilege= lPlbal }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;
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
                
                requireformate = Department;
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
        public JsonResult checkLeaveEligebleOrNot(int empid,string StartDate, string EndDate, string Ltctype)
        {
            string status = "";

            DateTime star1 = DateTime.Parse(StartDate);
            DateTime end1 = DateTime.Parse(EndDate);
            string lstar = star1.ToString("yyyy-MM-dd");
            string lend = end1.ToString("yyyy-MM-dd");
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            ///int lEmpId = db.Employes.Where(a => a.EmpId == lCredentails.EmpId).Select(a => a.Id).FirstOrDefault();
            int emplevescount = db.Leaves_LTC.Where(a => a.EmpId == empid).Count();
            string Empcode = db.Employes.Where(a => a.EmpId == empid.ToString()).Select(a => a.EmpId).FirstOrDefault();
            string EmpPkId1 = db.Employes.Where(a =>a.EmpId == empid.ToString()).Select(a => a.Id.ToString()).FirstOrDefault();
            string EmpPkId = EmpPkId1.ToString();
            LeavesBusiness Lbus = new LeavesBusiness();
            var dtwd = Lbus.getcheckLTCWDOD(EmpPkId,Empcode, lstar, lend,status);
            if (dtwd != "")
            {
                // status = "false/" + star1.ToShortDateString() + " , " + end1.ToShortDateString() + " " + "Already these dates are applied in " + dtwd;
                status = "false/" + "Please Check the date range already applied in  " + dtwd;
                return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            }
            if (emplevescount > 0)
            {
                string str = "";
                string str1 = "";
                List<Leaves_LTC> lStartEndCount = db.Leaves_LTC.Where(a => a.EmpId == empid).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.LtcType == "Availment").ToList();
                List<Leaves_LTC> lStartEndCount1 = db.Leaves_LTC.Where(a => a.EmpId == empid).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.LtcType == "Encashment").ToList();

                if (Ltctype == "Availment")
                {
                    foreach (Leaves_LTC l in lStartEndCount)
                    {

                        DateTime star = l.StartDate;
                        DateTime end = l.EndDate;
                        DateTime[] dates = GetDatesBetween(star, end).ToArray();
                        for (int i = 0; i < dates.Length; i++)
                        {
                            string d = dates[i].ToShortDateString();
                            if (star1 <= dates[i] && dates[i] <= end1)
                            {
                                //true condition already applied
                                str = str + dates[i].ToShortDateString() + ",";
                            }

                        }
                        if (str != "")
                        {
                            status = "false/" + str + "--Already LTC Availment applied in these dates.";
                        }
                    }

                }
                else if (Ltctype == "Encashment")
                {
                    foreach (Leaves_LTC l in lStartEndCount1)
                    {
                        DateTime star = l.StartDate;
                        DateTime end = l.EndDate;
                        DateTime[] dates = GetDatesBetween(star, end).ToArray();
                        for (int i = 0; i < dates.Length; i++)
                        {
                            string d = dates[i].ToShortDateString();

                            if (star1 <= dates[i] && dates[i] <= end1)
                            {
                                str1 = str1 + dates[i].ToShortDateString() + ",";
                            }
                        }
                        if (str1 != "")
                        {
                            status = "false/" + str1 + "--Already LTC Encashment applied in these dates.";
                        }

                    }
                }

            }
            else
            {

                status = "countzero";
            }
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkBlockPeriodEligebleOrNot1(string EmpCode, string blockperiod)
        {
            string status = "";
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == EmpCode).Select(a => a.Id).FirstOrDefault();
            int emplevescount = db.Leaves_LTC.Where(a => a.EmpId == lEmpId).Count();
            if (emplevescount > 0)
            {

                var lStartEndCount = db.Leaves_LTC.Where(a => a.EmpId == lEmpId).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.LtcType == "Encashment").Where(a => a.Block_Period == blockperiod).Select(a => a.Block_Period).Count();
                if (lStartEndCount > 0)
                {
                    status = "false/" + "--Already LTC Encashment applied in this Block Period.";
                }

            }
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkBlockPeriodEligebleOrNot(string EmpCode, string blockperiod)
        {
            string status = "";
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == EmpCode).Select(a => a.Id).FirstOrDefault();
            int emplevescount = db.Leaves_LTC.Where(a => a.EmpId == lEmpId).Count();
            if (emplevescount > 0)
            {

                var lStartEndCount = db.Leaves_LTC.Where(a => a.EmpId == lEmpId).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.LtcType == "Availment").Where(a => a.Block_Period == blockperiod).Select(a => a.Block_Period).Count();
                if(lStartEndCount > 0)
                {
                    status = "false/" + "--Already LTC Availment applied in this Block Period.";
                }

            }
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkLTCEligebleOrNot(string StartDate, string EndDate, string Ltctype)
        {
            string status = "";
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentails.EmpId).Select(a => a.Id).FirstOrDefault();
            int emplevescount = db.Leaves_LTC.Where(a => a.EmpId == lEmpId).Count();
            DateTime star1 = DateTime.Parse(StartDate);
            DateTime end1 = DateTime.Parse(EndDate);
            string lstar = star1.ToString("yyyy-MM-dd");
            string lend = end1.ToString("yyyy-MM-dd");
            if (emplevescount > 0)
            {
                string str1 = "";
                List<Leaves_LTC> lStartEndCount1 = db.Leaves_LTC.Where(a => a.EmpId == lEmpId).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.LtcType == "Encashment").ToList();
                if (Ltctype == "Encashment")
                {
                    foreach (Leaves_LTC l in lStartEndCount1)
                    {
                        DateTime star = l.StartDate;
                        DateTime end = l.EndDate;
                        DateTime[] dates = GetDatesBetween(star, end).ToArray();
                        for (int i = 0; i < dates.Length; i++)
                        {
                            string d = dates[i].ToShortDateString();

                            if (star1 <= dates[i] && dates[i] <= end1)
                            {
                                str1 = str1 + dates[i].ToShortDateString() + ",";
                            }
                        }
                        if (str1 != "")
                        {
                            status = "false/" + "--Already LTC Encashment applied in this Block Period.";
                        }
                    }
                    return Json(new { message = status }, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }
        public string GetDiffDays(DateTime Startdate, DateTime EndDate)
        {
            string ldurations = "";
            string lzeros = "00";
            DateTime d1 = Startdate;
            DateTime d2 = EndDate;
            string fromtime = d1.ToShortTimeString().ToString();
            string totime = d2.ToShortTimeString().ToString();
            int TotalDays = (EndDate.Date - Startdate.Date).Days;
            TotalDays = TotalDays + 1;
            if (totime == "6:00 PM")
            {
                //  string ltime = (TotalDays * 8).ToString();
                ldurations = TotalDays + " days - " + lzeros.PadRight(2, '0') + ":" + lzeros.PadRight(2, '0') + ":" + lzeros.PadRight(2, '0');
            }
            else
            {
                TotalDays = TotalDays - 1;
                d1 = Convert.ToDateTime(fromtime);
                d2 = Convert.ToDateTime(totime);
                TimeSpan ts = d2.Subtract(d1).Duration();
                double lhours = ts.TotalHours;
                int lonlyhours = (int)lhours;
                double lminutes = ts.Minutes;
                // lhours = lhours + (TotalDays * 8);
                if (TotalDays == 0)
                {
                    ldurations = lonlyhours.ToString().PadLeft(2, '0') + ":" + lminutes.ToString().PadRight(2, '0') + ":" + lzeros.PadRight(2, '0');
                }
                else
                {
                    ldurations = TotalDays + " days - " + lonlyhours + ":" + lminutes.ToString().PadRight(2, '0') + ":" + lzeros.PadRight(2, '0');
                }
            }
            return ldurations;
        }
        [HttpGet]
        public JsonResult GetAuthorityNamess(string Name)
        {
            string lresult = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                var employees = db.Employes.ToList();
                // var dbResult = db.Leaves.ToList();
                int lUserLoginId = employees.Where(a => a.EmpId.ToLower() == lCredentials.EmpId.ToLower()).Select(a => a.Id).FirstOrDefault();
                if (string.IsNullOrEmpty(Name))
                {
                    var lResult = (from userslist in employees
                                   where userslist.Id == lUserLoginId
                                   select new
                                   {
                                       userslist.ControllingAuthority,
                                       userslist.SanctioningAuthority,
                                       userslist.TotalExperience,
                                   });
                    var lresponseArray = lResult.ToArray();
                    string totalexperience = lresponseArray[0].TotalExperience;
                    string lControllingAuthority = lresponseArray[0].ControllingAuthority;
                    string lSanctioningAuthority = lresponseArray[0].SanctioningAuthority;
                    int lcontrol = Convert.ToInt32(lControllingAuthority);
                    int lsancationcontrol = Convert.ToInt32(lSanctioningAuthority);
                    Session["lcontrols"] = lcontrol;
                    Session["lSancation"] = lsancationcontrol;
                    Employees lcontrolling = Facade.EntitiesFacade.GetEmpTabledata.GetById(lcontrol);
                    Employees lsancationing = Facade.EntitiesFacade.GetEmpTabledata.GetById(lsancationcontrol);
                    return Json(new { lControllingAuthorityAjax = lcontrolling.ShortName, lSanctioningAuthorityAjax = lsancationing.ShortName, lTotalexpAjax = totalexperience }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var lResults = (from userslist in employees
                                    where userslist.Id == lUserLoginId
                                    select new
                                    {
                                        userslist.ControllingAuthority,
                                        userslist.SanctioningAuthority,
                                        userslist.TotalExperience,

                                    });
                    var lresponseArray = lResults.Distinct().ToArray();
                    string lControllingAuthority = lresponseArray[0].ControllingAuthority;
                    string lSanctioningAuthority = lresponseArray[0].SanctioningAuthority;
                    string totalexperience = lresponseArray[0].TotalExperience;
                    int lcontrol = Convert.ToInt32(lControllingAuthority);
                    int lsancationcontrol = Convert.ToInt32(lSanctioningAuthority);
                    Employees lcontrolling = Facade.EntitiesFacade.GetEmpTabledata.GetById(lcontrol);
                    Employees lsancationing = Facade.EntitiesFacade.GetEmpTabledata.GetById(lsancationcontrol);
                    return Json(new { lControllingAuthorityAjax = lcontrolling.ShortName, lSanctioningAuthorityAjax = lsancationing.ShortName, lTotalexpAjax = totalexperience }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;

        }
        [HttpGet]
        public JsonResult SelfLTApply(string StartDate)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
            try
            {
                var ldeputation = db.Leaves_LTC.ToList();
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
                                       otherduty.StartDate,
                                       otherduty.EndDate,
                                       otherduty.PlaceOfVisits,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                       otherduty.Status,
                                       otherduty.UpdatedDate,
                                       Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
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
                                       otherduty.StartDate,
                                       otherduty.EndDate,
                                       otherduty.PlaceOfVisits,
                                       otherduty.Status,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                       Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
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
        [HttpPost]

        public JsonResult LTHistorySearchViews(string StartDate, string EndDate)
        {
            string lMessage = string.Empty;
            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentails.EmpId).Select(a => a.Id).FirstOrDefault();

                if (StartDate == "" || EndDate == "")
                {
                    var ldeputation = db.Leaves_LTC.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var lodmaster = db.OD_Master.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from otherduty in ldeputation
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
                                        otherduty.StartDate,
                                        otherduty.EndDate,
                                        otherduty.Status,
                                        otherduty.PlaceOfVisits,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        otherduty.UpdatedDate,
                                        Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                        otherduty.Reason,
                                        otherduty.Subject
                                    }).OrderByDescending(A => A.UpdatedDate);
                    return Json(lResults, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DateTime ToDate = Convert.ToDateTime(EndDate);
                    string strDate = StartDate;
                    string[] sa = strDate.Split('-');
                    string strNew = sa[2] + "-" + sa[1] + "-" + sa[0];

                    string strDate1 = EndDate;
                    string[] sa1 = strDate1.Split('-');
                    string strNew1 = sa1[2] + "-" + sa1[1] + "-" + sa1[0];

                    DateTime FromDate = DateTime.ParseExact(strNew, "yyyy-MM-dd", null);
                    DateTime Todate = DateTime.ParseExact(strNew1, "yyyy-MM-dd", null);
                    var ldeputation = db.Leaves_LTC.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var lodmaster = db.OD_Master.ToList();
                    var ldesignation = db.Designations.ToList();
                    DateTime lStartdate = FromDate;
                    DateTime lEnddate = ToDate;

                    var lResults = (from otherduty in ldeputation
                                    join emp in lemployees on otherduty.EmpId equals emp.Id

                                    join branch in lBranches on otherduty.BranchId equals branch.Id
                                    join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                    join dept in Departments on otherduty.DepartmentId equals dept.Id

                                    where ((lStartdate >= otherduty.StartDate && lStartdate <= otherduty.EndDate)
                                      || (lEnddate >= otherduty.EndDate && lStartdate <= otherduty.EndDate)) && otherduty.EmpId == lEmpId
                                    select new
                                    {

                                        otherduty.Id,
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        otherduty.StartDate,
                                        otherduty.EndDate,
                                        otherduty.Status,
                                        otherduty.PlaceOfVisits,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                        otherduty.UpdatedDate,
                                        otherduty.Reason,
                                        otherduty.Subject
                                    }).OrderByDescending(A => A.UpdatedDate);
                    return Json(lResults, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        //LTC Approval
        [HttpGet]
        public ActionResult Approvals()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            string lMessage = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();

                if (lCredentials.LoginMode == "SuperAdmin")
                {
                    Leaves_LTC lmodel = new Leaves_LTC();
                    lmodel.Loginmode = lCredentials.LoginMode;
                    ViewBag.Message = lCredentials.LoginMode;
                    var items = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().Where(a => a.Code == "CL" || a.Code == "PL").Select(x => new LeaveTypes
                    {
                        Id = x.Id,
                        Type = x.Type.Trim(),
                    });
                    ViewBag.LeaveTypes = new SelectList(items, "Id", "Type");
                    var items1 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.Name != "OtherBranch").Select(x => new Branches
                    {
                        Id = x.Id,
                        Name = x.Name,
                    });
                    ViewBag.Branch = new SelectList(items1, "Id", "Name");
                    TempData["Loginmode"] = lCredentials.LoginMode;
                    return View(lmodel);
                }
                else
                {
                    Leaves_LTC lmodel = new Leaves_LTC();
                    lmodel.Loginmode = lCredentials.LoginMode;
                    ViewBag.Message = lCredentials.LoginMode;
                    var items = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().Where(a => a.Code == "CL" || a.Code == "PL").Select(x => new LeaveTypes
                    {
                        Id = x.Id,
                        Type = x.Type.Trim(),
                    });
                    ViewBag.LeaveTypes = new SelectList(items, "Id", "Type");
                    var items1 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.Name != "OtherBranch").Select(x => new Branches
                    {
                        Id = x.Id,
                        Name = x.Name,
                    });
                    ViewBag.Branch = new SelectList(items1, "Id", "Name");
                    TempData["Loginmode"] = lCredentials.LoginMode;
                    return View(lmodel);
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return View(lMessage);
        }
        public ActionResult LTCApprovalView()
        {
            return View();
        }

        [HttpGet]
        public JsonResult LTCApprovalViews(string status)
        {
            string lMessage = string.Empty;
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            try
            {
                var lEmployees = db.Employes.ToList();
                var lOtherduty = db.Leaves_LTC.ToList();
                var lBranches = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var lodmaster = db.OD_Master.ToList();
                var ldesignation = db.Designations.ToList();
                var lleaveTypes = db.LeaveTypes.ToList();
                var bp = db.BlockPeriod.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lControllingAuthority = db.Leaves_LTC.Where(a => a.ControllingAuthority == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                int lSancationingAuthority = db.Leaves_LTC.Where(a => a.SanctioningAuthority == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                if (lEmpId == lControllingAuthority)
                {
                    var Duration = string.Empty;
                    var lResult = (from otherduty in lOtherduty
                                   join emp in lEmployees on otherduty.EmpId equals emp.Id
                                   // join branch in lBranches on duty.VistorFrom equals branch.Id
                                   join branch in lBranches on otherduty.BranchId equals branch.Id
                                   join dept in ldept on otherduty.DepartmentId equals dept.Id
                                   join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                   join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                   //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                   join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                   where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                   otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                   select new
                                   {
                                       otherduty.Id,
                                       emp.EmpId,
                                       otherduty.LtcType,
                                       EmployeeName = emp.ShortName,
                                       otherduty.StartDate,
                                       otherduty.EndDate,
                                       designation = desig.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                       leavetypes.Code,
                                       otherduty.Status,
                                       otherduty.UpdatedDate,
                                       Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                       otherduty.Reason,
                                       otherduty.Subject,
                                       block.Block_Period,
                                   }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                }
                if (lEmpId == lSancationingAuthority)
                {
                    var lResult = (from otherduty in lOtherduty
                                   join emp in lEmployees on otherduty.EmpId equals emp.Id
                                   // join branch in lBranches on duty.VistorFrom equals branch.Id
                                   join branch in lBranches on otherduty.BranchId equals branch.Id
                                   join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                   join dept in ldept on otherduty.DepartmentId equals dept.Id
                                   join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                   join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                   //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                   where
                                     otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"

                                   select new
                                   {
                                       otherduty.Id,
                                       emp.EmpId,
                                       EmployeeName = emp.ShortName,
                                       otherduty.StartDate,
                                       otherduty.EndDate,
                                       otherduty.LtcType,
                                       designation = desig.Code,
                                       leavetypes.Code,
                                       Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                       otherduty.Status,
                                       otherduty.UpdatedDate,
                                       Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                       otherduty.Reason,
                                       otherduty.Subject,
                                       block.Block_Period,
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
        public JsonResult LTCApprovalViews(string EmployeeCodey, string LeaveIds)
        {
            Timesheet_Request_Form ltform = new Timesheet_Request_Form();
            string lMessage = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var ldbresult = db.Leaves_LTC.ToList();
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
                        string lstauts = db.Leaves_LTC.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                        if (lstauts == "Pending")
                        {
                            int leaverowid = Convert.ToInt32(lIdss);
                            Leaves_LTC lcontrolling = Facade.EntitiesFacade.GetLTCTabledata.GetById(leaverowid);

                            Leaves_LTC lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatep.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                            lupdatep.Status = "Forwarded";
                            db.Entry(lupdatep).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["Forward"] = "LTC Forwarded Successfully";
                            if (lupdatep.EndDate <= DateTime.Now.Date)
                            {
                                int ltype = Convert.ToInt32(lupdatep.LeaveType);
                                string lcode = db.LeaveTypes.Where(a => a.Id == ltype).Select(a => a.Code).FirstOrDefault();
                                int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                ltform.UserId = lId;
                                ltform.BranchId = (int)lupdatep.BranchId;
                                ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                ltform.DesignationId = (int)lupdatep.DesignationId;
                                ltform.Shift_Id = (int)shiftids;
                                ltform.Reason_Type = "LTC-" + lcode;
                                ltform.Reason_Desc = "LTC";
                                ltform.ReqFromDate = lupdatep.StartDate;
                                ltform.ReqToDate = lupdatep.EndDate;
                                ltform.CA = lupdatep.ControllingAuthority;
                                ltform.SA = lupdatep.SanctioningAuthority;
                                ltform.Status = lupdatep.Status;
                                ltform.UpdatedBy = lCredentials.EmpId;
                                ltform.UpdatedDate = DateTime.Now.Date;
                                ltform.Processed = 0;
                                //db.Timesheet_Request_Form.Add(ltform);
                                db.SaveChanges();
                            }
                        }
                        if (lstauts == "Forwarded")
                        {
                            int leaverowid = Convert.ToInt32(lIdss);
                            Leaves_LTC lSancationing = Facade.EntitiesFacade.GetLTCTabledata.GetById(leaverowid);

                            Leaves_LTC lupdatef = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatef.Status = "Approved";
                            lupdatef.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                            db.Entry(lupdatef).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["Approve"] = "LTC Approved Successfully";
                            if (lupdatef.EndDate <= DateTime.Now.Date)
                            {
                                int ltype = Convert.ToInt32(lupdatef.LeaveType);
                                string lcode = db.LeaveTypes.Where(a => a.Id == ltype).Select(a => a.Code).FirstOrDefault();
                                int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                ltform.UserId = lId;
                                ltform.BranchId = (int)lupdatef.BranchId;
                                ltform.DepartmentId = (int)lupdatef.DepartmentId;
                                ltform.DesignationId = (int)lupdatef.DesignationId;
                                ltform.Shift_Id = (int)shiftids;
                                ltform.Reason_Type = "LTC-"+lcode;
                                ltform.Reason_Desc = "LTC";
                                ltform.ReqFromDate = lupdatef.StartDate;
                                ltform.ReqToDate = lupdatef.EndDate;
                                ltform.CA = lupdatef.ControllingAuthority;
                                ltform.SA = lupdatef.SanctioningAuthority;
                                ltform.Status = lupdatef.Status;
                                ltform.UpdatedBy = lCredentials.EmpId;
                                ltform.UpdatedDate = DateTime.Now.Date;
                                ltform.Processed = 0;
                                //db.Timesheet_Request_Form.Add(ltform);
                                db.SaveChanges();
                            }
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
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var lEmpBalance = db.EmpLeaveBalance.Where(a=>a.Year==DateTime.Now.Year).ToList();
                var ldbresult = db.Leaves_LTC.ToList();
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
                            int lleaveTypeIds = db.LeaveTypes.Where(a => a.Code == lType).Select(a => a.Id).FirstOrDefault();
                            int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                            int LeaveId = Convert.ToInt32(lIdss);
                            string lstauts = db.Leaves_LTC.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                            string ltctype = db.Leaves_LTC.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.LtcType).FirstOrDefault();
                            string day = "15";
                            string month = "mar";
                            string year = DateTime.Now.Year.ToString();
                            string careylapse = day + "-" + month + "-" + year;
                            DateTime llapsedate = Convert.ToDateTime(careylapse).Date;

                            Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();

                            int? lcaryleavebal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Count();
                            int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                            int? leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                            int carrylbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                            if (lstauts == "Pending")
                            {
                                Leaves_LTC lcontrolling = Facade.EntitiesFacade.GetLTCTabledata.GetById(LeaveId);
                                string lcontrolstatus = "Cancelled";
                                string lcontrolvalue = "0";
                                Leaves_LTC lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Cancelled";
                                lupdatep.Year = DateTime.Now.Year;
                                lupdatep.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                if (ltctype == "Availment")
                                {
                                    int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year==DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                    DateTime lupade = Convert.ToDateTime(lupdatep.UpdatedDate).Date;
                                    int lLeaveDaystotal = db.Leaves_LTC.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds.ToString()).Where(a => a.Id == LeaveId && a.Year == DateTime.Now.Year).Select(a => a.TotalDays).FirstOrDefault();

                                    int TotalDays = lEmpLeaveBalancetotal + lLeaveDaystotal;
                                    
                                    EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                    lbalance.LeaveTypeId = lleaveTypeIds;
                                    lbalance.EmpId = lId;
                                    lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                    lbalance.LeaveBalance = TotalDays;
                                    db.Entry(lbalance).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                TempData["cancel"] = "LTC Cancelled Successfully";
                            }
                            else if (lstauts == "Forwarded")
                            {
                                Leaves_LTC lSancationing = Facade.EntitiesFacade.GetLTCTabledata.GetById(LeaveId);
                                string llSancationingstatus = "Cancelled";
                                string llSancationingvalue = "1";
                                Leaves_LTC lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Cancelled";
                                lupdatep.Year = DateTime.Now.Year;
                                lupdatep.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                if (ltctype == "Availment")
                                {
                                    int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                    int lLeaveDaystotal = db.Leaves_LTC.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds.ToString()).Where(a => a.Id == LeaveId && a.Year == DateTime.Now.Year).Select(a => a.TotalDays).FirstOrDefault();
                                    int TotalDays = lEmpLeaveBalancetotal + lLeaveDaystotal;
                                    DateTime lupade = Convert.ToDateTime(lupdatep.UpdatedDate).Date;
                                 
                                    EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                    lbalance.LeaveTypeId = lleaveTypeIds;
                                    lbalance.EmpId = lId;
                                    lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                    lbalance.LeaveBalance = TotalDays;
                                    db.Entry(lbalance).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                TempData["cancel"] = "LTC Cancelled Successfully";
                            }
                            k++;
                            break;
                        }
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
        public JsonResult Deny(string EmployeeCodey, string leaveTypes, string LeaveIds)
        {
            string lMessage = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var lEmpBalance = db.EmpLeaveBalance.Where(a=>a.Year==DateTime.Now.Year).ToList();
                var ldbresult = db.Leaves_LTC.ToList();
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
                            int lleaveTypeIds = db.LeaveTypes.Where(a => a.Code == lType).Select(a => a.Id).FirstOrDefault();
                            int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                            int LeaveId = Convert.ToInt32(lIdss);
                            string lstauts = db.Leaves_LTC.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                            string ltctype = db.Leaves_LTC.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.LtcType).FirstOrDefault();
                            string day = "15";
                            string month = "mar";
                            string year = DateTime.Now.Year.ToString();
                            string careylapse = day + "-" + month + "-" + year;
                            DateTime llapsedate = Convert.ToDateTime(careylapse).Date;
                          
                            Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();

                            int? lcaryleavebal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Count();
                            int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                            int? leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                            int carrylbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                            if (lstauts == "Pending")
                            {
                                Leaves_LTC lcontrolling = Facade.EntitiesFacade.GetLTCTabledata.GetById(LeaveId);
                                string lcontrolstatus = "Denied";
                                string lcontrolvalue = "0";
                                Leaves_LTC lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Denied";
                                lupdatep.Year = DateTime.Now.Year;
                                lupdatep.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                if (ltctype == "Availment")
                                {
                                    int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                    int lLeaveDaystotal = db.Leaves_LTC.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds.ToString()).Where(a => a.Id == LeaveId && a.Year == DateTime.Now.Year).Select(a => a.TotalDays).FirstOrDefault();
                                    int TotalDays = lEmpLeaveBalancetotal + lLeaveDaystotal;
                                    // leaves carryforward

                                    DateTime lupade = Convert.ToDateTime(lupdatep.UpdatedDate).Date;
                                    
                                    //empleavebalance
                                    EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                    lbalance.LeaveTypeId = lleaveTypeIds;
                                    lbalance.EmpId = lId;
                                   lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                    lbalance.LeaveBalance = TotalDays;
                                    db.Entry(lbalance).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                TempData["Denied"] = "LTC Denied Successfully";
                            }
                            else if (lstauts == "Forwarded")
                            {
                                Leaves_LTC lSancationing = Facade.EntitiesFacade.GetLTCTabledata.GetById(LeaveId);
                                string llSancationingstatus = "Denied";
                                string llSancationingvalue = "1";
                                Leaves_LTC lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Denied";
                                lupdatep.Year = DateTime.Now.Year;
                                lupdatep.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                if (ltctype == "Availment")
                                {
                                    DateTime lupade = Convert.ToDateTime(lupdatep.UpdatedDate).Date;
                                    int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                    int lLeaveDaystotal = db.Leaves_LTC.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds.ToString()).Where(a => a.Id == LeaveId && a.Year == DateTime.Now.Year).Select(a => a.TotalDays).FirstOrDefault();
                                    int TotalDays = lEmpLeaveBalancetotal + lLeaveDaystotal;
                                   
                                    EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                    lbalance.LeaveTypeId = lleaveTypeIds;
                                    lbalance.EmpId = lId;
                                    lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                    lbalance.LeaveBalance = TotalDays;
                                    db.Entry(lbalance).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                TempData["Denied"] = "LTC Denied Successfully";
                            }
                            k++;
                            break;
                        }
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
        public ActionResult LTCCancelRequest(string LeaveId)
        {
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentails.EmpId).Select(a => a.Id).FirstOrDefault();
            int id = 0;
            if (LeaveId != "")
                id = Convert.ToInt32(LeaveId);
            Leaves_LTC lOdCancel = Facade.EntitiesFacade.GetLTCTabledata.GetById(id);
            lOdCancel.Status = "Cancelled";
            db.Entry(lOdCancel).State = EntityState.Modified;
            db.SaveChanges();
            TempData["status"] = "LTC Cancelled Successfully";
            return RedirectToAction("History", "LeavesTravel");

        }
        [HttpGet]
        public ActionResult History()
        {
            string lMessage = string.Empty;
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            if (TempData["AlertMessage"] != null)
            {
                lMessage = TempData["AlertMessage"].ToString();
            }
            ViewBag.Message = lCredentials.LoginMode;
            TempData["Loginmode"] = lCredentials.LoginMode;

            var lmodel = new Leaves_LTC { Loginmode = lCredentials.LoginMode };
            return View(lmodel);
        }

        [HttpGet]
        public JsonResult LTCHistoryViews(string StartDate)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            try
            {
                var ltype = db.OD_Master.ToList();
                var ldeputation = db.Leaves_LTC.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                var lResult = (from otherduty in ldeputation
                               join emp in lemployees on otherduty.EmpId equals emp.Id
                               //join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                               join branch in lBranches on otherduty.BranchId equals branch.Id
                               join desig in ldesignation on otherduty.DesignationId equals desig.Id
                               join dept in Departments on otherduty.DepartmentId equals dept.Id

                               orderby otherduty.Id descending
                               select new
                               {

                                   otherduty.Id,
                                   emp.EmpId,
                                   EmployeeName = emp.ShortName,
                                   designation = desig.Code,

                                   UpdatedDate = otherduty.UpdatedDate,
                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                   StartDate = GetsDates(otherduty.StartDate),
                                   EndDate = GetsDates(otherduty.EndDate),
                                   Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),

                                   otherduty.Status,
                                   otherduty.Reason,
                                   Action = otherduty.Status == "Cancelled" ? "Cancelled" : "Cancel"
                               }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                return Json(lResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }

        [HttpPost]
        public JsonResult LTCHistoryView(FormCollection form)
        {
            var YourRadioButton = Request.Form["Status"];
            string lMessage = string.Empty;
            try
            {
                var ltype = db.OD_Master.ToList();
                var ldeputation = db.Leaves_LTC.ToList();
                var lleaveHistory = db.V_LeaveHistory.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                DateTime FromDate = Convert.ToDateTime(form["Stdate"]);
                DateTime ToDate = Convert.ToDateTime(form["Endate"]);
                string Applieddate = form["lApplied"];
                string Requestdate = form["lRequest"];
                DateTime lStartdate = Convert.ToDateTime(FromDate.ToString("yyyy-MM-dd 00:00:00.000"));
                DateTime lEnddate = Convert.ToDateTime(ToDate.ToString("yyyy-MM-dd 23:59:59.000"));
                var Departments = db.Departments.ToList();
                var lbranch = db.Branches.ToList();
                // int leavetypeids = 0;
                // int LtypeId = Convert.ToInt32(leaveTypeId);
                // string lType = db.LeaveTypes.Where(a => a.Id == LtypeId).Select(a => a.Type).FirstOrDefault();
                if (Applieddate == "Applied")
                {
                    var lResults = (from deput in ldeputation
                                    join emp in lemployees on deput.EmpId equals emp.Id

                                    join branch in lbranch on deput.BranchId equals branch.Id
                                    join desig in ldesignation on deput.DesignationId equals desig.Id
                                    join dept in Departments on deput.DepartmentId equals dept.Id

                                    where ((deput.UpdatedDate >= lStartdate.Date
                                   && deput.UpdatedDate <= lEnddate.Date) || (deput.UpdatedDate <= lStartdate.Date
                                   && deput.UpdatedDate >= lEnddate.Date))
                                    select new
                                    {
                                        deput.Id,
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,

                                        UpdatedDate = deput.UpdatedDate,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        StartDate = GetsDates(deput.StartDate),
                                        EndDate = GetsDates(deput.EndDate),
                                        Duration = GetDiffDays(deput.StartDate, deput.EndDate),

                                        deput.Status,
                                        deput.Reason,
                                        Action = deput.Status == "Cancelled" ? "Cancelled" : "Cancel"
                                    }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResults, JsonRequestBehavior.AllowGet);
                }

                else if (Applieddate == "Request")
                {

                    var lResults = (from deput in ldeputation
                                    join emp in lemployees on deput.EmpId equals emp.Id
                                    // join visitbran in lbranch on deput.VistorFrom equals visitbran.Id
                                    join branch in lbranch on deput.BranchId equals branch.Id
                                    join desig in ldesignation on deput.DesignationId equals desig.Id
                                    join dept in Departments on deput.DepartmentId equals dept.Id
                                    //join type in ltype on deput.Purpose equals type.Id
                                    where deput.StartDate.Date >= lStartdate.Date && deput.EndDate.Date <= lEnddate.Date
                                    || (deput.StartDate.Date <= lStartdate.Date
                                   && deput.EndDate.Date >= lEnddate.Date)
                                    select new
                                    {

                                        deput.Id,
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,

                                        UpdatedDate = deput.UpdatedDate,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        StartDate = GetsDates(deput.StartDate),
                                        EndDate = GetsDates(deput.EndDate),
                                        Duration = GetDiffDays(deput.StartDate, deput.EndDate),

                                        deput.Status,
                                        deput.Reason,
                                        Action = deput.Status == "Cancelled" ? "Cancelled" : "Cancel"
                                    }).OrderByDescending(A => A.StartDate).ThenByDescending(a => a.Id);
                    return Json(lResults, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }
        [HttpGet]
        public JsonResult TodaysLTC(string StartDate)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
            try
            {
                var ltype = db.OD_Master.ToList();
                var ldeputation = db.Leaves_LTC.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                var lResult = ((from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id

                                join branch in lBranches on otherduty.BranchId equals branch.Id
                                join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                join dept in Departments on otherduty.DepartmentId equals dept.Id

                                where ((lStartDate >= otherduty.StartDate.Date && lStartDate <= otherduty.EndDate.Date)
                                      || (lEndDate >= otherduty.EndDate.Date && lStartDate <= otherduty.EndDate.Date))
                                orderby otherduty.Id descending
                                select new
                                {

                                    otherduty.Id,
                                    emp.EmpId,
                                    EmployeeName = emp.ShortName,

                                    designation = desig.Code,
                                    UpdatedDate = otherduty.UpdatedDate,
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = GetsDates(otherduty.StartDate),
                                    EndDate = GetsDates(otherduty.EndDate),

                                    Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                    otherduty.Status,
                                    otherduty.Reason,
                                    Action = otherduty.Status == "Cancelled" ? "Cancelled" : "Cancel"
                                }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id));
                return Json(lResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }
        [HttpPost]
        public ActionResult LTCPost(ViewModel LTC)
        {
            int leavebalance = 0;
            int lEmpId1 = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.Id).FirstOrDefault();
            leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId== lEmpId1 && a.LeaveTypeId.ToString() ==LTC.lleavetravel.LeaveType && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
            LTC.lleavetravel.Year = DateTime.Now.Year;
            var lEmpLeaveBalance = db.EmpLeaveBalance.ToList();
            var lEmpBalance = db.V_EmpLeaveBalance.ToList();
            LoginCredential lcredentials = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.Id).FirstOrDefault();
            Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId.ToString() == LTC.lleavetravel.LeaveType && a.Year == DateTime.Now.Year).FirstOrDefault();
          
            int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId.ToString() == LTC.lleavetravel.LeaveType && a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();

            int? previousbal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId.ToString() == LTC.lleavetravel.LeaveType && a.Year == DateTime.Now.Year).Where(a => a.Year == DateTime.Now.Year).Select(a => a.PreviousYearCF).FirstOrDefault();
            DateTime? Retirement = db.Employes.Where(a => a.EmpId == lcredentials.EmpId).Select(a => a.RetirementDate).FirstOrDefault();
            try
            {
                if (Retirement >= LTC.lleavetravel.EndDate)
                {
                    var lTypes = db.LeaveTypes.ToList();
                    int lCasualLeave =  db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.Year == DateTime.Now.Year && a.LeaveTypeId.ToString() == LTC.lleavetravel.LeaveType).Select(a => a.LeaveBalance).FirstOrDefault();
                    int lPrivilegeLeave = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.Year == DateTime.Now.Year &&  a.LeaveTypeId.ToString() == LTC.lleavetravel.LeaveType).Select(a=>a.LeaveBalance).FirstOrDefault();
                    int lCompOff = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.CompensatoryOff).FirstOrDefault();
                    string lLeaveCode = lTypes.Where(a => a.Id == Convert.ToInt32(LTC.lleavetravel.LeaveType)).Select(a => a.Code).FirstOrDefault();
                    int Counts = db.HolidayList.Where(a => a.Date >= LTC.lleavetravel.StartDate).Where(k => k.Date <= LTC.lleavetravel.EndDate).Select(k => k.Date).Distinct().Count();
                    if (LTC.lleavetravel.LtcType == "Availment")
                    {
                        FamilyRelations lfamily = new FamilyRelations();
                        Leaves_LTC lt = new Leaves_LTC();
                        //   int lcontrolling = Convert.ToInt32(Session["lcontrols"].ToString());
                        //    int lsanctioning = Convert.ToInt32(Session["lSancation"].ToString());
                        string totalexperience = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.TotalExperience).FirstOrDefault();
                        LTC.lleavetravel.UpdatedBy = Convert.ToInt32(lcredentials.EmpId);
                        LTC.lleavetravel.EmpId = lEmpId1;
                        LTC.lleavetravel.UpdatedDate = GetCurrentTime(DateTime.Now);
                        LTC.lleavetravel.Status = "Approved";
                        LTC.lleavetravel.leave_balance = leavebalance;
                        LTC.lleavetravel.Block_Period = LTC.Block_Period;
                        LTC.lleavetravel.ModeOfTransport = LTC.ModeOfTransport;
                        string lcontrolling = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                        string lsanctioning = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                        LTC.lleavetravel.ControllingAuthority = Convert.ToInt32(lcontrolling);
                        LTC.lleavetravel.SanctioningAuthority = Convert.ToInt32(lsanctioning);
                        LTC.lleavetravel.TotalExperience = totalexperience;
                        int totalleavebalances = (LTC.lleavetravel.EndDate.Date - LTC.lleavetravel.StartDate.Date).Days;
                        int ltotalbalace = totalleavebalances + 1;
                        if (lLeaveCode == "CL")
                        {
                            LTC.lleavetravel.TotalDays = ltotalbalace - Counts;
                        }
                        else
                        {
                            LTC.lleavetravel.TotalDays = ltotalbalace;
                        }

                        int branch = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.Branch).FirstOrDefault();
                        int department = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.Department).FirstOrDefault();
                        int designation = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.CurrentDesignation).FirstOrDefault();

                        LTC.lleavetravel.BranchId = branch;
                        LTC.lleavetravel.DepartmentId = department;
                        LTC.lleavetravel.DesignationId = designation;
                        if (lLeaveCode == "CL")
                        {
                            if (lCasualLeave - (ltotalbalace - Counts) < 0)
                            {
                                TempData["AlertMessage"] = "No Casual Leaves are available to apply LTC";
                                return RedirectToAction("AdminLTCApply");
                            }

                        }
                        if (lLeaveCode == "PL")
                        {
                            if (lPrivilegeLeave - ltotalbalace < 0)
                            {
                                TempData["AlertMessage"] = "No Privilege Leaves are available to apply LTC";
                                return RedirectToAction("AdminLTCApply");
                            }
                        }
                        if (lLeaveCode == "C-OFF")
                        {
                            if (lCompOff - ltotalbalace < 0)
                            {
                                TempData["AlertMessage"] = "No C-OFF  available to apply LTC";
                                return RedirectToAction("AdminLTCApply");
                            }
                        }
                        else
                        {
                            // LTC.lleavetravel.TotalDays = ltotalbalace;
                            db.Leaves_LTC.Add(LTC.lleavetravel);
                            db.SaveChanges();
                            if (lLeaveCode == "CL")
                            {
                                DateTime star1 = LTC.lleavetravel.StartDate.Date;
                                DateTime end1 = LTC.lleavetravel.EndDate.Date;
                                int LeaveType = lTypes.Where(a => a.Id == Convert.ToInt32(LTC.lleavetravel.LeaveType)).Select(a => a.Id).FirstOrDefault();
                                int UiStartenddiff = (end1 - star1).Days;
                                UiStartenddiff = UiStartenddiff + 1;
                                var lHolidays = db.HolidayList.ToList();
                                int Count = lHolidays.Where(a => a.Date >= LTC.lleavetravel.StartDate).Where(k => k.Date <= LTC.lleavetravel.EndDate).Select(k => k.Date).Distinct().Count();
                                int Leavedays = UiStartenddiff - Count;
                                int leavetypes = Convert.ToInt32(LTC.lleavetravel.LeaveType);
                                EmpLeaveBalance NewEmpbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == leavetypes && a.Year == DateTime.Now.Year).FirstOrDefault();


                                NewEmpbalance.EmpId = lEmpId1;
                                NewEmpbalance.LeaveTypeId = Convert.ToInt32(LTC.lleavetravel.LeaveType);
                                int totalleavebalances1 = NewEmpbalance.LeaveBalance - Leavedays;
                                NewEmpbalance.Debits = NewEmpbalance.Debits + Leavedays;
                                NewEmpbalance.LeaveBalance = totalleavebalances1;
                                NewEmpbalance.UpdatedBy = lEmpId.ToString();
                                db.Entry(NewEmpbalance).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                int leavetypes = Convert.ToInt32(LTC.lleavetravel.LeaveType);
                                EmpLeaveBalance NewEmpbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == leavetypes && a.Year == DateTime.Now.Year).FirstOrDefault();

                                NewEmpbalance.EmpId = lEmpId1;
                                NewEmpbalance.LeaveTypeId = Convert.ToInt32(LTC.lleavetravel.LeaveType);
                                int totalleavebalances1 = NewEmpbalance.LeaveBalance - LTC.lleavetravel.TotalDays;
                                NewEmpbalance.LeaveBalance = totalleavebalances1;
                                NewEmpbalance.Debits = NewEmpbalance.Debits + LTC.lleavetravel.TotalDays;
                                NewEmpbalance.UpdatedBy = lEmpId.ToString();
                                db.Entry(NewEmpbalance).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            for (int i = 0; i < LTC.relation.Length; i++)
                            {
                                var lmembers = LTC.relation.GetValue(i).ToString();
                                if (lmembers == "Father" || lmembers == "Mother" || lmembers == "Children")
                                {
                                    lfamily.EmpId = lEmpId;
                                    lfamily.Relation = lmembers;
                                    lfamily.RelationName = LTC.name.GetValue(i).ToString();
                                    lfamily.RelationAge = LTC.RelationAge.GetValue(i).ToString();
                                    lfamily.Occupation = LTC.Occupation.GetValue(i).ToString();
                                    lfamily.DeclarationType = LTC.DeclarationType1;
                                    lfamily.EmpWorking = string.Empty;
                                    lfamily.EmpAddress = string.Empty;
                                    lfamily.IsLiable = LTC.Form1CheckBox;
                                    lfamily.Place = LTC.Form1Place;
                                    lfamily.LTCDate = Convert.ToDateTime(LTC.Form1Date);
                                    lfamily.LTCId = LTC.lleavetravel.Id;
                                    db.FamilyRelations.Add(lfamily);

                                    db.SaveChanges();
                                }
                                else if (lmembers == "Spouse")
                                {
                                    lfamily.EmpId = lEmpId1;
                                    lfamily.Relation = lmembers;
                                    lfamily.RelationName = LTC.name.GetValue(i).ToString();
                                    lfamily.RelationAge = LTC.RelationAge.GetValue(i).ToString();
                                    lfamily.Occupation = LTC.Occupation.GetValue(i).ToString();
                                    lfamily.DeclarationType = LTC.DeclarationType2;
                                    lfamily.EmpWorking = LTC.lrelation.EmpWorking;
                                    lfamily.EmpAddress = LTC.lrelation.EmpAddress;
                                    lfamily.IsLiable = LTC.Form2CheckBox;
                                    lfamily.Place = LTC.Form2Place;
                                    lfamily.LTCDate = Convert.ToDateTime(LTC.Form2Date);
                                    lfamily.LTCId = LTC.lleavetravel.Id;
                                    db.FamilyRelations.Add(lfamily);

                                    db.SaveChanges();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            TempData["AlertMessage"] = "LTC Applied Successfully";
                            return RedirectToAction("AdminLTCApply");
                        }

                    }
                    else
                    {
                        if (LTC.lleavetravel.LtcType == "Encashment")
                        {
                            FamilyRelations lfamily = new FamilyRelations();
                            Leaves_LTC lt = new Leaves_LTC();
                            //   int lcontrolling = Convert.ToInt32(Session["lcontrols"].ToString());
                            //    int lsanctioning = Convert.ToInt32(Session["lSancation"].ToString());
                            string totalexperience = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.TotalExperience).FirstOrDefault();
                            LTC.lleavetravel.UpdatedBy = Convert.ToInt32(lcredentials.EmpId);
                            LTC.lleavetravel.EmpId = lEmpId1;
                            LTC.lleavetravel.UpdatedDate = GetCurrentTime(DateTime.Now);
                            LTC.lleavetravel.Status = "Approved";
                            LTC.lleavetravel.Block_Period = LTC.Block_Period;
                            LTC.lleavetravel.leave_balance = leavebalance;
                            LTC.lleavetravel.ModeOfTransport = LTC.ModeOfTransport;
                            string lcontrolling = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                            string lsanctioning = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                            LTC.lleavetravel.ControllingAuthority = Convert.ToInt32(lcontrolling);
                            LTC.lleavetravel.SanctioningAuthority = Convert.ToInt32(lsanctioning);
                            LTC.lleavetravel.TotalExperience = totalexperience;
                            int totalleavebalances = (LTC.lleavetravel.EndDate.Date - LTC.lleavetravel.StartDate.Date).Days;
                            int ltotalbalace = totalleavebalances + 1;
                            if (lLeaveCode == "CL")
                            {
                                LTC.lleavetravel.TotalDays = ltotalbalace - Counts;
                            }
                            else
                            {
                                LTC.lleavetravel.TotalDays = ltotalbalace;
                            }

                            int branch = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.Branch).FirstOrDefault();
                            int department = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.Department).FirstOrDefault();
                            int designation = db.Employes.Where(a => a.EmpId == LTC.EmpId).Select(a => a.CurrentDesignation).FirstOrDefault();

                            LTC.lleavetravel.BranchId = branch;
                            LTC.lleavetravel.DepartmentId = department;
                            LTC.lleavetravel.DesignationId = designation;
                            if (lLeaveCode == "CL")
                            {
                                if (lCasualLeave - (ltotalbalace - Counts) < 0)
                                {
                                    TempData["AlertMessage"] = "No Casual Leaves are available to apply LTC";
                                    return RedirectToAction("AdminLTCApply");
                                }

                            }
                            if (lLeaveCode == "PL")
                            {
                                if (lPrivilegeLeave - ltotalbalace < 0)
                                {
                                    TempData["AlertMessage"] = "No Privilege Leaves are available to apply LTC";
                                    return RedirectToAction("AdminLTCApply");
                                }
                            }
                            if (lLeaveCode == "C-OFF")
                            {
                                if (lCompOff - ltotalbalace < 0)
                                {
                                    TempData["AlertMessage"] = "No C-OFF  available to apply LTC";
                                    return RedirectToAction("AdminLTCApply");
                                }
                            }
                            else
                            {
                                // LTC.lleavetravel.TotalDays = ltotalbalace;
                                db.Leaves_LTC.Add(LTC.lleavetravel);
                                db.SaveChanges();
                                if (lLeaveCode == "CL")
                                {
                                    DateTime star1 = LTC.lleavetravel.StartDate.Date;
                                    DateTime end1 = LTC.lleavetravel.EndDate.Date;
                                    int LeaveType = lTypes.Where(a => a.Id == Convert.ToInt32(LTC.lleavetravel.LeaveType)).Select(a => a.Id).FirstOrDefault();
                                    int UiStartenddiff = (end1 - star1).Days;
                                    UiStartenddiff = UiStartenddiff + 1;
                                    var lHolidays = db.HolidayList.ToList();
                                    int Count = lHolidays.Where(a => a.Date >= LTC.lleavetravel.StartDate).Where(k => k.Date <= LTC.lleavetravel.EndDate).Select(k => k.Date).Distinct().Count();
                                    int Leavedays = UiStartenddiff - Count;
                                    int leavetypes = Convert.ToInt32(LTC.lleavetravel.LeaveType);
                                    EmpLeaveBalance NewEmpbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == leavetypes && a.Year == DateTime.Now.Year).FirstOrDefault();


                                    NewEmpbalance.EmpId = lEmpId1;
                                    NewEmpbalance.LeaveTypeId = Convert.ToInt32(LTC.lleavetravel.LeaveType);
                                    int totalleavebalances1 = NewEmpbalance.LeaveBalance - Leavedays;
                                    NewEmpbalance.Debits = NewEmpbalance.Debits + Leavedays;
                                    NewEmpbalance.LeaveBalance = totalleavebalances1;
                                    NewEmpbalance.UpdatedBy = lEmpId.ToString();
                                    db.Entry(NewEmpbalance).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    int leavetypes = Convert.ToInt32(LTC.lleavetravel.LeaveType);
                                    EmpLeaveBalance NewEmpbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == leavetypes && a.Year == DateTime.Now.Year).FirstOrDefault();

                                    NewEmpbalance.EmpId = lEmpId1;
                                    NewEmpbalance.LeaveTypeId = Convert.ToInt32(LTC.lleavetravel.LeaveType);
                                    int totalleavebalances1 = NewEmpbalance.LeaveBalance - LTC.lleavetravel.TotalDays;
                                    NewEmpbalance.LeaveBalance = totalleavebalances1;
                                    NewEmpbalance.Debits = NewEmpbalance.Debits + LTC.lleavetravel.TotalDays;
                                    NewEmpbalance.UpdatedBy = lEmpId.ToString();
                                    db.Entry(NewEmpbalance).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                for (int i = 0; i < LTC.relation.Length; i++)
                                {
                                    var lmembers = LTC.relation.GetValue(i).ToString();
                                    if (lmembers == "Father" || lmembers == "Mother" || lmembers == "Children")
                                    {
                                        lfamily.EmpId = lEmpId;
                                        lfamily.Relation = lmembers;
                                        lfamily.RelationName = LTC.name.GetValue(i).ToString();
                                        lfamily.RelationAge = LTC.RelationAge.GetValue(i).ToString();
                                        lfamily.Occupation = LTC.Occupation.GetValue(i).ToString();
                                        lfamily.DeclarationType = LTC.DeclarationType1;
                                        lfamily.EmpWorking = string.Empty;
                                        lfamily.EmpAddress = string.Empty;
                                        lfamily.IsLiable = LTC.Form1CheckBox;
                                        lfamily.Place = LTC.Form1Place;
                                        lfamily.LTCDate = Convert.ToDateTime(LTC.Form1Date);
                                        lfamily.LTCId = LTC.lleavetravel.Id;
                                        db.FamilyRelations.Add(lfamily);

                                        db.SaveChanges();
                                    }
                                    else if (lmembers == "Spouse")
                                    {
                                        lfamily.EmpId = lEmpId1;
                                        lfamily.Relation = lmembers;
                                        lfamily.RelationName = LTC.name.GetValue(i).ToString();
                                        lfamily.RelationAge = LTC.RelationAge.GetValue(i).ToString();
                                        lfamily.Occupation = LTC.Occupation.GetValue(i).ToString();
                                        lfamily.DeclarationType = LTC.DeclarationType2;
                                        lfamily.EmpWorking = LTC.lrelation.EmpWorking;
                                        lfamily.EmpAddress = LTC.lrelation.EmpAddress;
                                        lfamily.IsLiable = LTC.Form2CheckBox;
                                        lfamily.Place = LTC.Form2Place;
                                        lfamily.LTCDate = Convert.ToDateTime(LTC.Form2Date);
                                        lfamily.LTCId = LTC.lleavetravel.Id;
                                        db.FamilyRelations.Add(lfamily);

                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                TempData["AlertMessage"] = "LTC Encashed Successfully";
                                return RedirectToAction("AdminLTCApply");
                            }

                        }
                    }
                    }
                else
                {
                    TempData["AlertMessage"] = "The Selected Dates should be less than or equal to the Retirement Date" + "  " + Retirement.Value.ToShortDateString() + "  " + "Please select other dates.";
                    return RedirectToAction("AdminLTCApply");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return RedirectToAction("AdminLTCApply");
        }
        [HttpGet]
        public JsonResult LTCViewencash(string EmpId)
        {
            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var lResult = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var dResult = db.Designations.ToList();
                var ltcresult = db.Leaves_LTC.ToList();
                var blockperiod = db.BlockPeriod.ToList();
                var leavetype = db.LeaveTypes.ToList();
                if (String.IsNullOrEmpty(EmpId))
                {
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                                join leavetypes in leavetype on employee.LeaveType equals Convert.ToString(leavetypes.Id)
                                where emp.EmpId == lCredentails.EmpId && employee.LtcType == "Encashment"
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.StartDate,
                                    employee.EndDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.TravelAdvance,
                                    employee.ModeOfTransport,
                                    employee.PlaceOfVisits,
                                    employee.LtcType,
                                    employee.Reason,
                                    block.Block_Period,
                                    leavetypes.Type,
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
        [HttpPost]
        public JsonResult LTCViewsencash(string startdate, string enddate)
        {
            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var lResult = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var dResult = db.Designations.ToList();
                var ltcresult = db.Leaves_LTC.ToList();
                var blockperiod = db.BlockPeriod.ToList();
                var leavetype = db.LeaveTypes.ToList();
                if (startdate == "" && enddate == "")
                {
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                                join leavetypes in leavetype on employee.LeaveType equals Convert.ToString(leavetypes.Id)
                                where emp.EmpId == lCredentails.EmpId && employee.LtcType == "Encashment"
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.StartDate,
                                    employee.EndDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.TravelAdvance,
                                    employee.ModeOfTransport,
                                    employee.PlaceOfVisits,
                                    employee.LtcType,
                                    employee.Reason,
                                    block.Block_Period,
                                    leavetypes.Type,
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (startdate != "" && enddate != "")
                {
                    var Sd = Convert.ToDateTime(startdate);
                    var Ed = Convert.ToDateTime(enddate);
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                                join leavetypes in leavetype on employee.LeaveType equals Convert.ToString(leavetypes.Id)
                                where (employee.StartDate >= Sd && employee.EndDate <= Ed) ||
                                (employee.StartDate <= Sd && employee.EndDate >= Ed)
                                where emp.EmpId == lCredentails.EmpId && employee.LtcType == "Encashment"
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.StartDate,
                                    employee.EndDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.TravelAdvance,
                                    employee.ModeOfTransport,
                                    employee.PlaceOfVisits,
                                    employee.LtcType,
                                    employee.Reason,
                                    block.Block_Period,
                                    leavetypes.Type,
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
        public JsonResult LTCView(string EmpId)
        {
            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var lResult = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var dResult = db.Designations.ToList();
                var ltcresult = db.Leaves_LTC.ToList();
                var blockperiod = db.BlockPeriod.ToList();
                var leavetype = db.LeaveTypes.ToList();
                if (String.IsNullOrEmpty(EmpId))
                {
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                                join leavetypes in leavetype on employee.LeaveType equals Convert.ToString(leavetypes.Id)
                                where emp.EmpId == lCredentails.EmpId 
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.StartDate,
                                    employee.EndDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.TravelAdvance,
                                    employee.ModeOfTransport,
                                    employee.PlaceOfVisits,
                                    employee.LtcType,
                                    employee.Reason,
                                    block.Block_Period,
                                    leavetypes.Type,
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
        [HttpPost]
        public JsonResult LTCViews(string startdate, string enddate,string Type)
        {
            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var lResult = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();

                var dResult = db.Designations.ToList();
                var ltcresult = db.Leaves_LTC.ToList();
                var blockperiod = db.BlockPeriod.ToList();
                var leavetype = db.LeaveTypes.ToList();
                if (startdate == "" && enddate == "")
                {
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                                join leavetypes in leavetype on employee.LeaveType equals Convert.ToString(leavetypes.Id)
                                where emp.EmpId == lCredentails.EmpId 
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.StartDate,
                                    employee.EndDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.TravelAdvance,
                                    employee.ModeOfTransport,
                                    employee.PlaceOfVisits,
                                    employee.LtcType,
                                    employee.Reason,
                                    block.Block_Period,
                                    leavetypes.Type,
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (startdate != "" && enddate != "" && Type=="")
                {
                    var Sd = Convert.ToDateTime(startdate);
                    var Ed = Convert.ToDateTime(enddate);
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                                join leavetypes in leavetype on employee.LeaveType equals Convert.ToString(leavetypes.Id)
                                where (employee.StartDate >= Sd && employee.EndDate <= Ed) ||
                                (employee.StartDate <= Sd && employee.EndDate >= Ed)
                                where emp.EmpId == lCredentails.EmpId 
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.StartDate,
                                    employee.EndDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.TravelAdvance,
                                    employee.ModeOfTransport,
                                    employee.PlaceOfVisits,
                                    employee.LtcType,
                                    employee.Reason,
                                    block.Block_Period,
                                    leavetypes.Type,
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (startdate != "" && enddate != "" && Type == "Encashment")
                {
                    var Sd = Convert.ToDateTime(startdate);
                    var Ed = Convert.ToDateTime(enddate);
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                                join leavetypes in leavetype on employee.LeaveType equals Convert.ToString(leavetypes.Id)
                                where (employee.StartDate >= Sd && employee.EndDate <= Ed) ||
                                (employee.StartDate <= Sd && employee.EndDate >= Ed)
                                where emp.EmpId == lCredentails.EmpId && employee.LtcType == "Encashment"
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.StartDate,
                                    employee.EndDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.TravelAdvance,
                                    employee.ModeOfTransport,
                                    employee.PlaceOfVisits,
                                    employee.LtcType,
                                    employee.Reason,
                                    block.Block_Period,
                                    leavetypes.Type,
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (startdate != "" && enddate != "" && Type=="Availment")
                {
                    var Sd = Convert.ToDateTime(startdate);
                    var Ed = Convert.ToDateTime(enddate);
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on employee.DesignationId equals desig.Id
                                join branchs in lbranch on employee.BranchId equals branchs.Id
                                join depart in ldepartments on employee.DepartmentId equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                                join leavetypes in leavetype on employee.LeaveType equals Convert.ToString(leavetypes.Id)
                                where (employee.StartDate >= Sd && employee.EndDate <= Ed) ||
                                (employee.StartDate <= Sd && employee.EndDate >= Ed)
                                where emp.EmpId == lCredentails.EmpId && employee.LtcType == "Availment"
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    employee.UpdatedDate,
                                    employee.StartDate,
                                    employee.EndDate,
                                    employee.Subject,
                                    employee.TotalExperience,
                                    employee.Status,
                                    employee.TravelAdvance,
                                    employee.ModeOfTransport,
                                    employee.PlaceOfVisits,
                                    employee.LtcType,
                                    employee.Reason,
                                    block.Block_Period,
                                    leavetypes.Type,
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
        [HttpPost]
        public ActionResult DeclarationForm(FormCollection form)
        {
            return View();
        }
        public JsonResult GetTotalPLBalance(string Name)
        {

            string lresult = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                //var lleavetype = db.LeaveTypes.ToList();
                var lleaveBalance = db.V_EmpLeaveBalance.ToList();
                //var leaveid = db.LeaveTypes.Where(a => a.Type == "Privilege Leave").Select(a => a.Id).FirstOrDefault();
                int lUserLoginId = db.Employes.Where(a => a.EmpId.ToString() == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                //var employees = db.EmpLeaveBalance.Where(a => a.EmpId == lUserLoginId).Where(a => a.LeaveTypeId == leaveid).ToList();
                if (string.IsNullOrEmpty(Name))
                {
                    var lResult = (from lbalance in lleaveBalance
                                   where lbalance.EmpId == lUserLoginId
                                   select new
                                   {
                                       lbalance.CasualLeave,
                                       lbalance.PrivilegeLeave,
                                       lbalance.CompensatoryOff,


                                   });
                    var lresponseArray = lResult.ToArray();
                    int lcasual = lresponseArray[0].CasualLeave;
                    int lprivilege = lresponseArray[0].PrivilegeLeave;
                    int lcompoff = lresponseArray[0].CompensatoryOff;

                    //int ltotalexperience = Convert.ToInt32(totalexperience);
                    //  Session["lcontrols"] = lcontrol;   
                    return Json(new { lcasualajax = lcasual, lprivilegeajax = lprivilege, lcoffajax = lcompoff }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;
        }
        [HttpGet]
        public JsonResult CheckBlockPeriod(string EmpCode,string Name)
        {
            string status = "";
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == EmpCode).Select(a => a.Id).FirstOrDefault();
            int emplevescount = db.Leaves_LTC.Where(a => a.EmpId == lEmpId).Count();
            if (emplevescount > 0)
            {

                var lStartEndCount = db.Leaves_LTC.Where(a => a.EmpId == lEmpId).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.LtcType == "Encashment").Where(a => a.Block_Period == Name).Select(a => a.Block_Period).Count();
                if (lStartEndCount > 0)
                {
                    status = "false/" + "--Already LTC Encashment applied in this Block Period.";
                }

            }
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public JsonResult LTCTooltip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var lEmployees = db.Employes.ToList();
            var lOtherduty = db.Leaves_LTC.ToList();
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
                               join block in bp on Convert.ToInt32(duty.Block_Period) equals block.Id
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
                                   StartingDate = duty.StartDate.ToShortDateString(),
                                   EndDate = duty.EndDate.ToShortDateString(),
                                   Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                   duty.Status,
                                   block.Block_Period,
                                   duty.LtcType,
                                   leavetypes.Code,
                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = OderrowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                string lVisitingfrom = lresponseArray[0].LtcType;
                string Vistingto = lresponseArray[0].Code;
                string ODStartDate = lresponseArray[0].StartingDate;
                string ODEndDate = lresponseArray[0].EndDate;
                string ODDuration = lresponseArray[0].Block_Period;
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
        public JsonResult LTCNexttip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var lEmployees = db.Employes.ToList();
            var lOtherduty = db.Leaves_LTC.ToList();
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
                               join block in bp on Convert.ToInt32(duty.Block_Period) equals block.Id
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
                                   StartingDate = duty.StartDate.ToShortDateString(),
                                   EndDate = duty.EndDate.ToShortDateString(),
                                   Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                   duty.Status,
                                   block.Block_Period,
                                   duty.LtcType,
                                   leavetypes.Code,
                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = OderrowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                string lVisitingfrom = lresponseArray[0].LtcType;
                string Vistingto = lresponseArray[0].Code;
                string ODStartDate = lresponseArray[0].StartingDate;
                string ODEndDate = lresponseArray[0].EndDate;
                string ODDuration = lresponseArray[0].Duration;
                string ODPurpose = lresponseArray[0].Block_Period;
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
                    lpurpose = ODPurpose,
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
        public JsonResult LTCPrevioustip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var lEmployees = db.Employes.ToList();
            var lOtherduty = db.Leaves_LTC.ToList();
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
                               join block in bp on Convert.ToInt32(duty.Block_Period) equals block.Id
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
                                   StartingDate = duty.StartDate.ToShortDateString(),
                                   EndDate = duty.EndDate.ToShortDateString(),
                                   Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                   duty.Status,
                                   block.Block_Period,
                                   duty.LtcType,
                                   leavetypes.Code,
                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = OderrowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                string lVisitingfrom = lresponseArray[0].LtcType;
                string Vistingto = lresponseArray[0].Code;
                string ODStartDate = lresponseArray[0].StartingDate;
                string ODEndDate = lresponseArray[0].EndDate;
                string ODDuration = lresponseArray[0].Duration;
                string ODPurpose = lresponseArray[0].Block_Period;
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
                    lpurpose = ODPurpose,
                    lstatus = ODStatus
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        [HttpPost]
        public JsonResult LTCApprovalSearch(string EmployeeCode, string Branch, string ADate, string LeaveType)
        {

            string lMessage = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                var lleaveHistory = db.V_LeaveHistory.ToList();
                var lOtherduty = db.Leaves_LTC.ToList();
                var lleaveTypes = db.LeaveTypes.ToList();
                var lEmployees = db.Employes.ToList();
                var lleaves = db.Leaves.ToList();
                var lBranches = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var bp = db.BlockPeriod.ToList();
                var ldesignation = db.Designations.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lControllingAuthority = db.Leaves.Where(a => a.ControllingAuthority == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                int lSancationingAuthority = db.Leaves.Where(a => a.SanctioningAuthority == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                var lleaveBalance = db.V_EmpLeaveBalance.ToList();
                if (lEmpId == lControllingAuthority)
                {
                    if (EmployeeCode == "" && Branch == "" && ADate == "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id                                       
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id                                      
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch == "" && ADate == "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch != "" && ADate == "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch

                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch == "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch == "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate == "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.EmpId == EmployeeCode
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.EmpId == EmployeeCode
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch == "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.EmpId == EmployeeCode
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch == "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.EmpId == EmployeeCode
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch != "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch == "" && ADate != "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch != "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate != "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id                                      
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"                                 
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate == "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch != "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch      
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch != "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    
                }
                else if (lEmpId == lSancationingAuthority)
                {
                    if (EmployeeCode != "" && Branch == "" && ADate == "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch != "" && ADate == "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else   if (EmployeeCode == "" && Branch == "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch == "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate == "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.EmpId == EmployeeCode
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.EmpId == EmployeeCode
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch == "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.EmpId == EmployeeCode
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch == "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.EmpId == EmployeeCode
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch != "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch == "" && ADate != "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch != "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate != "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode != "" && Branch != "" && ADate == "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where emp.EmpId == EmployeeCode
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch != "" && ADate != "" && LeaveType == "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where otherduty.UpdatedDate.Value.Date == Convert.ToDateTime(ADate)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (EmployeeCode == "" && Branch != "" && ADate == "" && LeaveType != "")
                    {
                        var Duration = string.Empty;
                        var lResult = (from otherduty in lOtherduty
                                       join emp in lEmployees on otherduty.EmpId equals emp.Id
                                       // join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branch in lBranches on otherduty.BranchId equals branch.Id
                                       join dept in ldept on otherduty.DepartmentId equals dept.Id
                                       join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                       join block in bp on Convert.ToInt32(otherduty.Block_Period) equals block.Id
                                       //join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       join leavetypes in lleaveTypes on Convert.ToInt32(otherduty.LeaveType) equals leavetypes.Id
                                       where otherduty.ControllingAuthority == lControllingAuthority && otherduty.Status == "Pending" ||
                                       otherduty.SanctioningAuthority == lSancationingAuthority && otherduty.Status == "Forwarded"
                                       where emp.Branch == Convert.ToInt32(Branch) || emp.Branch_Value1 == Branch
                                       where leavetypes.Id == Convert.ToInt32(LeaveType)
                                       select new
                                       {
                                           otherduty.Id,
                                           emp.EmpId,
                                           otherduty.LtcType,
                                           EmployeeName = emp.ShortName,
                                           otherduty.StartDate,
                                           otherduty.EndDate,
                                           designation = desig.Code,
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leavetypes.Code,
                                           otherduty.Status,
                                           otherduty.UpdatedDate,
                                           Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                           otherduty.Reason,
                                           otherduty.Subject,
                                           block.Block_Period,
                                       }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }

        //HolidayCount cl,ml,pl combinations
        public JsonResult CLMLPLHolidayLeave(int empid, string appStDate, string appEndDate, string leavetype)
        {
            SqlHelper sh = new SqlHelper();
            DateTime appStDate1 = Convert.ToDateTime(appStDate);
            DateTime appEndDate1 = Convert.ToDateTime(appEndDate);
            string qry1 = "";
            string qry2 = "";
            string qry3 = "";
            string qry4 = "";
            if (leavetype == "1")
            {
                //Scenario 1  - cl, H1, H2, (apply pl, ml)
                 qry1 = "Select top 1 DATEDIFF(Day, EndDate+1, '" + appStDate1 + "') as LastLeaveDayDiff , EndDate LastLeaveDate,LeaveType from Leaves_LTC where LeaveType in (3) and Status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited') and EndDate<'" + appStDate1 + "' and empid=(select id from Employees where empid= '" + empid + "') order by EndDate desc; ";
                 qry2 = " select count(*) as HolidaysCount from(select distinct[Date] from HolidayList where Date > (select top 1 EndDate from Leaves_LTC where LeaveType in (3)  and EndDate < '" + appStDate1 + "' and empid = (select id from Employees where empid='" + empid + "') order by EndDate desc) and Date< '" + appStDate1 + "') as x; ";

                //scenario 2 - (apply pl, ml), H1, H2, CL
                 qry3 = "select top 1 DATEDIFF(Day, '" + appEndDate1 + "',StartDate-1) as LatestLeaveDayDiff, StartDate LatestLeaveDate,LeaveType from Leaves_LTC where LeaveType in (3) and Status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited') and StartDate>'" + appEndDate1 + "' and empid=(select id from Employees where empid='" + empid + "') order by StartDate asc; ";
                 qry4 = "select count(*) as HolidaysCount from(select distinct[Date] from HolidayList where Date > '" + appEndDate1 + "' and Date < (select top 1 StartDate from Leaves_LTC where LeaveType  in (3) and StartDate > '" + appEndDate1 + "'and empid = (select id from Employees where empid='" + empid + "') order by StartDate asc)) as x; ";
            }
            if (leavetype == "3")
            {
                //Scenario 1  - cl, H1, H2, (apply pl, ml)
                qry1 = "Select top 1 DATEDIFF(Day, EndDate+1, '" + appStDate1 + "') as LastLeaveDayDiff , EndDate LastLeaveDate,LeaveType from Leaves_LTC where LeaveType in (1) and Status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited') and EndDate<'" + appStDate1 + "' and empid= (select id from Employees where empid='" + empid + "') order by EndDate desc; ";
                qry2 = " select count(*) as HolidaysCount from(select distinct[Date] from HolidayList where Date > (select top 1 EndDate from Leaves_LTC where LeaveType in (1)  and EndDate < '" + appStDate1 + "' and empid = (select id from Employees where empid='" + empid + "') order by EndDate desc) and Date< '" + appStDate1 + "') as x; ";

                //scenario 2 - (apply pl, ml), H1, H2, CL
                qry3 = "select top 1 DATEDIFF(Day, '" + appEndDate1 + "',StartDate-1) as LatestLeaveDayDiff, StartDate LatestLeaveDate,LeaveType from Leaves_LTC where LeaveType in (1) and Status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited') and StartDate>'" + appEndDate1 + "' and empid= (select id from Employees where empid='" + empid + "') order by StartDate asc; ";
                qry4 = "select count(*) as HolidaysCount from(select distinct[Date] from HolidayList where Date > '" + appEndDate1 + "' and Date < (select top 1 StartDate from Leaves_LTC where LeaveType  in (1) and StartDate > '" + appEndDate1 + "'and empid = (select id from Employees where empid='" + empid + "') order by StartDate asc)) as x; ";
            }
            DataSet ds1 = sh.Get_MultiTables_FromQry(qry1 + qry2 + qry3 + qry4);
            DataTable dt1 = ds1.Tables[0];
            DataTable dt2 = ds1.Tables[1];
            DataTable dt3 = ds1.Tables[2];
            DataTable dt4 = ds1.Tables[3];
            string status = "";

            //Scenario 1  - cl, H1, H2, (apply pl, ml)
            if (dt1.Rows.Count > 0 && dt2.Rows.Count > 0)
            {
                int lastLeaveDayDiff = int.Parse(dt1.Rows[0]["LastLeaveDayDiff"].ToString());
                int holidaysCount = int.Parse(dt2.Rows[0]["HolidaysCount"].ToString());
                if (lastLeaveDayDiff == holidaysCount)
                {
                    string cd = "CL";
                    if (dt1.Rows[0]["LeaveType"].ToString() == "2")
                    {
                        cd = "ML";
                    }
                    else if (dt1.Rows[0]["LeaveType"].ToString() == "3")
                    {
                        cd = "PL";
                    }
                    //return "E#There is " + cd + " applied on " + Convert.ToDateTime(dt1.Rows[0]["LastLeaveDate"]).ToString("dd-MM-yyyy");
                    status= "false/" + "--There is " + cd + " applied on " + Convert.ToDateTime(dt1.Rows[0]["LastLeaveDate"]).ToString("dd-MM-yyyy")+ " Consecutive Leaves cannot be applied"; 
                }
            }

            //scenario 2 - (apply pl, ml), H1, H2, CL
            if (dt3.Rows.Count > 0 && dt4.Rows.Count > 0)
            {
                int latestLeaveDayDiff = int.Parse(dt3.Rows[0]["LatestLeaveDayDiff"].ToString());
                int holidaysCount = int.Parse(dt4.Rows[0]["HolidaysCount"].ToString());
                if (latestLeaveDayDiff == holidaysCount)
                {
                    string cd = "CL";
                    if (dt3.Rows[0]["LeaveType"].ToString() == "2")
                    {
                        cd = "ML";
                    }
                    else if (dt3.Rows[0]["LeaveType"].ToString() == "3")
                    {
                        cd = "PL";
                    }
                    status = "false/" + "--There is " + cd + " applied on " + Convert.ToDateTime(dt3.Rows[0]["LatestLeaveDate"]).ToString("dd-MM-yyyy") + " Consecutive Leaves cannot be applied";
                }
            }
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);

        }
    }
}