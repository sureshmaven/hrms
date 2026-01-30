using Entities;
using HRMSApplication.Filters;
using HRMSApplication.Helpers;
using HRMSApplication.Models;
using HRMSBusiness.Business;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Facade.EntitiesFacade;
using HRMSBusiness.Comm;
using Newtonsoft.Json;
using HRMSBusiness.Reports;
using System.Data;
using Mavensoft.DAL;
using HRMSBusiness.Db;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class LeaveApplyController : Controller
    {
        private ContextBase db = new ContextBase();
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LeaveApplyController));
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        ReportBusiness Rbus = new ReportBusiness();
        // GET: LeaveApply
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       

        [NoDirectAccess]
        [SessionTimeoutAttribute]

        public ActionResult LeaveRequest()
        {
            string lMessage = string.Empty;
            try
            {
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                LeavesBusiness lbus = new LeavesBusiness();
                int lEmpId = Convert.ToInt32(lCredentials.EmpPkId);
                if (TempData["status"] != null)
                {
                    lMessage = TempData["status"].ToString();
                }
                int designations = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
                string lcodes = db.Designations.Where(a => a.Id == designations).Select(a => a.Code).FirstOrDefault();
                ViewBag.LeaveTypes = new SelectList(lbus.getLeaveTypesSearch(lcodes), "Id", "Type");
                int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
                string lcode = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
                ViewBag.LeaveTypes1 = new SelectList(lbus.getAllLeaveTypes(lcode), "Id", "Type");

                int lPaternityValue = db.LeaveTypes.Where(a => a.Code == "PTL").Select(a => a.Id).FirstOrDefault();
                int lMaternityValue = db.LeaveTypes.Where(a => a.Code == "MTL").Select(a => a.Id).FirstOrDefault();
                ViewBag.PaternityId = lPaternityValue;
                ViewBag.MaternityId = lMaternityValue;
                var lEmpleaveBalanceList = db.V_EmpLeaveBalance.Where(a => a.EmpId == lEmpId).ToList();
                LeaveViewModel lmodel = new LeaveViewModel();
                lmodel.Loginmode = lCredentials.LoginMode;
                int designation1 = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
                string lcode1 = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
                lmodel.designation = lcode1;
                Leaves lLeaves = new Leaves();
                V_EmpLeaveBalance lbalance = new V_EmpLeaveBalance();
                lbalance.GetAllLeavesTypes = lEmpleaveBalanceList;
                lmodel.lEmpLeaveBal = lbalance;

                return View(lmodel);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return View(lMessage);
        }
        //public ActionResult LeaveRequest()
        //{
        //    string lMessage = string.Empty;
        //    try
        //    {
        //        TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
        //        LeavesBusiness lbus = new LeavesBusiness();
        //        int lEmpId = Convert.ToInt32(lCredentials.EmpPkId);
        //        if (TempData["status"] != null)
        //        {
        //            lMessage = TempData["status"].ToString();
        //        }
        //        int designations = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
        //        string lcodes = db.Designations.Where(a => a.Id == designations).Select(a => a.Code).FirstOrDefault();
        //        ViewBag.LeaveTypes = new SelectList(lbus.getLeaveTypesSearch(lcodes), "Id", "Type");
        //        int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
        //        string lcode = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
        //        ViewBag.LeaveTypes1 = new SelectList(lbus.getAllLeaveTypes(lcode), "Id", "Type");

        //        int lPaternityValue = db.LeaveTypes.Where(a => a.Code == "PTL").Select(a => a.Id).FirstOrDefault();
        //        int lMaternityValue = db.LeaveTypes.Where(a => a.Code == "MTL").Select(a => a.Id).FirstOrDefault();
        //        ViewBag.PaternityId = lPaternityValue;
        //        ViewBag.MaternityId = lMaternityValue;
        //        var lEmpleaveBalanceList = db.V_EmpLeaveBalance.Where(a => a.EmpId == lEmpId).ToList();
        //        LeaveViewModel lmodel = new LeaveViewModel();
        //        lmodel.Loginmode = lCredentials.LoginMode;
        //        int designation1 = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
        //        string lcode1 = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
        //        lmodel.designation = lcode1;
        //        Leaves lLeaves = new Leaves();
        //        V_EmpLeaveBalance lbalance = new V_EmpLeaveBalance();
        //        lbalance.GetAllLeavesTypes = lEmpleaveBalanceList;
        //        lmodel.lEmpLeaveBal = lbalance;

        //        return View(lmodel);
        //    }
        //    catch (Exception ex)
        //    {
        //        lMessage = ex.Message;
        //    }
        //    return View(lMessage);
        //}

        [HttpGet]
        public ActionResult Leaveforemployee()
        {
            string lMessage = string.Empty;
            try
            {
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                LeavesBusiness lbus = new LeavesBusiness();
                int lEmpId = Convert.ToInt32(lCredentials.EmpPkId);
                if (TempData["status"] != null)
                {
                    lMessage = TempData["status"].ToString();
                }
                int designations = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
                string lcodes = db.Designations.Where(a => a.Id == designations).Select(a => a.Code).FirstOrDefault();
                ViewBag.LeaveTypes = new SelectList(lbus.getLeaveTypesSearch(lcodes), "Id", "Type");
                int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
                string lcode = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
                ViewBag.LeaveTypes1 = new SelectList(lbus.getAllLeaveTypes(lcode), "Id", "Type");

                int lPaternityValue = db.LeaveTypes.Where(a => a.Code == "PTL").Select(a => a.Id).FirstOrDefault();
                int lMaternityValue = db.LeaveTypes.Where(a => a.Code == "MTL").Select(a => a.Id).FirstOrDefault();
                ViewBag.PaternityId = lPaternityValue;
                ViewBag.MaternityId = lMaternityValue;
                var lEmpleaveBalanceList = db.V_EmpLeaveBalance.Where(a => a.EmpId == lEmpId).ToList();
                LeaveViewModel lmodel = new LeaveViewModel();
                lmodel.Loginmode = lCredentials.LoginMode;
                int designation1 = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
                string lcode1 = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
                lmodel.designation = lcode1;
                Leaves lLeaves = new Leaves();
                V_EmpLeaveBalance lbalance = new V_EmpLeaveBalance();
                lbalance.GetAllLeavesTypes = lEmpleaveBalanceList;
                lmodel.lEmpLeaveBal = lbalance;

                return View(lmodel);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return View(lMessage);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Leaveforemployee(LeaveViewModel leaves, FormCollection form)
        {

            LogInformation.Info("Leaveapply Code started ");
            ContextBase db = new ContextBase();
            MobileApplyLeaveDTO leavedata = new MobileApplyLeaveDTO();

            Leaves leave = new Leaves();

            int lEmpId = db.Employes.Where(a => a.EmpId == leaves.EmpId).Select(a => a.Id).FirstOrDefault();
            DateTime lStartDate = leaves.lleaves.StartDate;
            DateTime lEndDate = leaves.lleaves.EndDate;
            string empcode = db.Employes.Where(a => a.EmpId == leaves.EmpId).Select(a => a.EmpId).FirstOrDefault();
            //string empid = lEmpId.ToString();
            int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
            int lcode = db.Designations.Where(a => a.Id == designation).Select(a => a.Id).FirstOrDefault();
            int branch = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.Branch).FirstOrDefault();
            int dept = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.Department).FirstOrDefault();
            int lType = leaves.lleaves.Id;
            int leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == lType && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
            string startdt = lStartDate.ToString("yyyy-MM-dd");
            string enddt = lEndDate.ToString("yyyy-MM-dd");
            string statuss = leaves.lleaves.Status;
            //LeavesBusiness Lbus = new LeavesBusiness();
            //var dtwd = Lbus.getcheckLTCWDOD(lCredentials.EmpPkId, empcode, startdt, enddt, statuss);
            //if (dtwd != "" && dtwd != "WD") // if already work diary appplied again leave is applying on that day need to allow
            //{
            //    TempData["status"] = "Please Check the date range already applied in  " + dtwd;
            //}

            string lControlling = db.Employes.Where(a => a.EmpId == leaves.EmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
            string lSacantioning = db.Employes.Where(a => a.EmpId == leaves.EmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
            string ename = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.ShortName).FirstOrDefault();
            LeavesBusiness Lbus = new LeavesBusiness();
            var dtwd = Lbus.getcheckLTCWDOD(lEmpId.ToString(), empcode, startdt, enddt, statuss);
            if (dtwd != "" && dtwd != "WD") // if already work diary appplied again leave is applying on that day need to allow
            {
                TempData["AlertMessage"] = "Please Check the date range already applied in  " + dtwd;
            }
            //leave.MatrenityType = form["MaternityType"];
            //leave.DeliveryDate = form["lDelivery.DeliveryDate"];
            else
            {


                leave.EmpId = lEmpId;
                leave.LeaveType = lType;
                leave.ControllingAuthority = Convert.ToInt32(lControlling);
                leave.SanctioningAuthority = Convert.ToInt32(lSacantioning);
                leave.StartDate = lStartDate;
                leave.EndDate = lEndDate;
                int TotalDays = (lEndDate - lStartDate).Days;
                TotalDays = TotalDays + 1;
                leave.LeaveDays = TotalDays;
                leave.TotalDays = TotalDays;
                leave.Subject = "Applied By" + ename;
                leave.Reason = leaves.lleaves.Reason;
                leave.LeavesYear = DateTime.Now.Year;
                leave.UpdatedBy = lCredentials.EmpId;
                leave.UpdatedDate = DateTime.Now;
                leave.LeaveTimeStamp = DateTime.Now;
                leave.BranchId = branch;
                leave.DesignationId = designation;
                leave.DepartmentId = dept;
                leave.Status = "Approved";
                leave.leave_balance = leavebalance;
                db.Leaves.Add(leave);
                db.SaveChanges();
                EmpLeaveBalance lbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == lType && a.Year == DateTime.Now.Year).FirstOrDefault();
                lbalance.LeaveTypeId = lType;
                lbalance.EmpId = lEmpId;
                if(lType ==12 || lType == 16)
                {
                    lbalance.LeaveBalance = leavebalance + TotalDays;
                }
                else
                {
                    lbalance.LeaveBalance = leavebalance - TotalDays;
                }
                
                lbalance.Year = DateTime.Now.Year;
                db.Entry(lbalance).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Leave Applied Successfully";
                return RedirectToAction("Leaveforemployee");
            }



            return RedirectToAction("Leaveforemployee");
        }


        public ActionResult GetAuthorityNames()
        {
            return View();
        }
        public DateTime[] GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates.ToArray();
        }
        public DateTime[] GetDatesBetweenCL(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates.ToArray();
        }

        [HttpGet]
        public JsonResult GetAuthorityNamess(string Name)
        {
            string lresult = string.Empty;
            try
            {
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

                                   });
                    var lresponseArray = lResult.ToArray();

                    string lControllingAuthority = lresponseArray[0].ControllingAuthority;
                    string lSanctioningAuthority = lresponseArray[0].SanctioningAuthority;
                    int lcontrol = Convert.ToInt32(lControllingAuthority);
                    int lsancationcontrol = Convert.ToInt32(lSanctioningAuthority);
                    Session["lcontrols"] = lcontrol;
                    Session["lSancation"] = lsancationcontrol;
                    Employees lcontrolling = Facade.EntitiesFacade.GetEmpTabledata.GetById(lcontrol);
                    Employees lsancationing = Facade.EntitiesFacade.GetEmpTabledata.GetById(lsancationcontrol);
                    return Json(new { lControllingAuthorityAjax = lcontrolling.ShortName, lSanctioningAuthorityAjax = lsancationing.ShortName }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var lResults = (from userslist in employees
                                    where userslist.Id == lUserLoginId
                                    select new
                                    {
                                        userslist.ControllingAuthority,
                                        userslist.SanctioningAuthority,

                                    });
                    var lresponseArray = lResults.Distinct().ToArray();

                    string lControllingAuthority = lresponseArray[0].ControllingAuthority;
                    string lSanctioningAuthority = lresponseArray[0].SanctioningAuthority;

                    return Json(new { lControllingAuthorityAjax = lControllingAuthority, lSanctioningAuthorityAjax = lSanctioningAuthority }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;

        }
        [NoDirectAccess]
        public ActionResult LeaveHistoryView()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }

        public DateTime getFormatDate(DateTime dt)
        {

            DateTime theDate = dt;
            string myTime = theDate.ToString("MM/dd/yyyy");
            return Convert.ToDateTime(myTime);
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
            else
            {
                requireformate = requireformate.Replace('/', ' ');
            }
            return requireformate;
        }
        public string LeaveHistoryViews()
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string EmpIds = lCredentials.EmpId;
            LeavesBusiness lbus = new LeavesBusiness();
            var dt = lbus.getLeaveHistory(lCredentials.EmpPkId);

            return JsonConvert.SerializeObject(Rbus.LeaveApplyController(EmpIds, lEmpId));
            // return JsonConvert.SerializeObject(dt);

        }


        //   public JsonResult LeaveHistorySearchView(string StartDate, string EndDate, string leaveTypeId)
        [HttpPost]
        public string LeaveHistorySearchView(string StartDate, string EndDate, string LeaveType)

        {
            Session["lStartDate"] = StartDate;
            Session["lEndDate"] = EndDate;
            Session["lleaveid"] = LeaveType;
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string EmpIds = lCredentials.EmpId;
                if (StartDate == "" || EndDate == "" || LeaveType == "")
                {


                    StartDate = "-1";
                    EndDate = "-2";
                    LeaveType = "-3";
                    return JsonConvert.SerializeObject(Rbus.SearchLeaveApplyController(EmpIds, lEmpId, StartDate, EndDate, LeaveType));

                }

                else
                {

                    return JsonConvert.SerializeObject(Rbus.SearchLeaveApplyController(EmpIds, lEmpId, StartDate, EndDate, LeaveType));

                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        public int GetdiffbetweendatesLeaves(int Leavedays)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int leaveid = db.Leaves.Where(a => a.EmpId == lEmpId).Select(a => a.LeaveType).FirstOrDefault();
            int leavedays = db.Leaves.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveType == leaveid).Select(a => a.LeaveDays).FirstOrDefault();

            //DateTime date1 = Sd.Date;
            //DateTime date2 = Ed.Date;
            //double NoOfDays = (date2 - date1).TotalDays;
            //double Leavedays = NoOfDays + 1;
            return Leavedays;
        }
        public JsonResult displayjoiningdate(string StartDate, string EndDate, string leaveTypeId)
        {
            string status = "";
            DateTime star1 = DateTime.Parse(StartDate);
            DateTime end1 = DateTime.Parse(EndDate);
            var lHolidays = db.HolidayList.ToList();
            int Leavetypeid = Convert.ToInt32(leaveTypeId);
            string leavetype = db.LeaveTypes.Where(a => a.Id == Leavetypeid).Select(a => a.Code).FirstOrDefault();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int Count = lHolidays.Where(a => a.Date >= end1).Where(k => k.Date <= end1).Count();
            int hdays = 0;
            DateTime? nextHoliday12 = db.HolidayList.Where(a => a.Date > end1).Min(a => (DateTime?)a.Date);
            DateTime nextHoliday1 = Convert.ToDateTime(nextHoliday12);
            if (nextHoliday1 != null && (nextHoliday1 - end1).TotalDays == 1)
            {
                hdays = 1;
                DateTime nextHoliday2 = db.HolidayList.Where(a => a.Date > nextHoliday1).Min(a => a.Date);
                if (nextHoliday2 != null && (nextHoliday2 - nextHoliday1).TotalDays == 1)

                {
                    hdays = 2;
                    DateTime nextHoliday3 = db.HolidayList.Where(a => a.Date > nextHoliday2).Min(a => a.Date);
                    if (nextHoliday3 != null && (nextHoliday3 - nextHoliday2).TotalDays == 1)

                    {
                        hdays = 3;
                        DateTime nextHoliday4 = db.HolidayList.Where(a => a.Date > nextHoliday3).Min(a => a.Date);

                        if ((nextHoliday4 - nextHoliday3).TotalDays == 1)
                        {
                            hdays = 4;
                        }

                    }
                }
            }

            if (hdays != 0)
            {
                DateTime Temp = Convert.ToDateTime(nextHoliday1);
                double ldays = (Temp - end1).TotalDays;

                if (ldays <= hdays + 1)
                {
                    end1 = (end1).AddDays(hdays + 1);

                }
            }
            else
            {
                end1 = end1.AddDays(1);

            }
            status = "false/" + end1.ToShortDateString() + "---is Joining Date.";
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);
        }
        string status = "";
        public JsonResult checkLeaveEligebleOrNot(string StartDate, string EndDate, int LeaveType)
        {
            //string status = "";
            DateTime star1 = DateTime.Parse(StartDate);
            DateTime end1 = DateTime.Parse(EndDate);
            string lstar = star1.ToString("yyyy-MM-dd");
            string lend = end1.ToString("yyyy-MM-dd");
            int UiStartenddiff = (end1 - star1).Days;
            UiStartenddiff = UiStartenddiff + 1;
            int lHolidayCount = db.HolidayList.Where(a => a.Date >= star1 && a.Date <= end1).Select(a => a.Date).Distinct().Count();
            int UiStartenddiff1 = UiStartenddiff - lHolidayCount;
            var lHolidays = db.HolidayList.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
            string lcode = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
            int emplevescount = db.Leaves.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveType == LeaveType).Count();
            int emplevescounts = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == LeaveType && a.Year == DateTime.Now.Year).Count();
            string basedate = "1900-01-01 00:00:00.000";
            DateTime ccDate = DateTime.Parse(basedate);
            DateTime? StartHoliday = db.HolidayList.Where(a => a.Date == star1 && a.DeleteAt == ccDate.Date).Select(a => (DateTime?)a.Date).FirstOrDefault();
            DateTime? EndHoliday = db.HolidayList.Where(a => a.Date == end1 && a.DeleteAt == ccDate.Date).Select(a => (DateTime?)a.Date).FirstOrDefault();
            string ltypes = db.LeaveTypes.Where(a => a.Id == LeaveType).Select(a => a.Code).FirstOrDefault();
            string Empcode = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            DateTime JoinigDate = Convert.ToDateTime(db.Employes.Where(a => a.Id == lEmpId).Select(a => a.DOJ).FirstOrDefault());
            DateTime lOneyear = JoinigDate.AddMonths(+12);
            if (ltypes == "ML")
            {
                if (star1 <= lOneyear)
                {
                    status = "false/" + "No Medical Leave is eligible upto one year of joining date";
                    return Json(new { message = status }, JsonRequestBehavior.AllowGet);
                }
            }

            LeavesBusiness Lbus = new LeavesBusiness();
            var dtwd = Lbus.getcheckLTCWDOD(lCredentials.EmpPkId, Empcode, lstar, lend, status);
            if (dtwd != "" && !dtwd.Contains("WD")) // if already work diary appplied again leave is applying on that day
            {
                // status = "false/" + star1.ToShortDateString() + " , " + end1.ToShortDateString() + " " + "Already these dates are applied in " + dtwd;
                status = "false/" + "Please Check the date range already applied in  " + dtwd;
                return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            }
            if (lcode != "Attender-Watchman" && lcode != "Watchman")
            {
                if (StartHoliday == star1 || EndHoliday == end1)
                {
                    status = "false/" + "Leave Cannot Start or End  on Holidays.";
                    return Json(new { message = status }, JsonRequestBehavior.AllowGet);
                }

            }
            //if (emplevescounts == 0 && ltypes != "LOP")
            //{
            //    status = "false/" + "No Balance to apply leave";
            //    return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            //}
            if (ltypes == "LOP")
            {
                status = "true";
                return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            }
            if (ltypes == "CL" || ltypes == "C-OFF")
            {
                int isbefore = CLBeforeholidays(star1, end1, LeaveType);
                int isafter = CLAfterholidays(star1, end1, LeaveType);
                int TotalClLeavedays = isbefore + isafter + UiStartenddiff;
                if (TotalClLeavedays <= 7)
                {
                    status = "countzero/" + "Leave apply Starts From " + star1.ToShortDateString() + "  To " + end1.ToShortDateString();
                    return Json(new { message = status, stdate = star1, eddate = end1 }, JsonRequestBehavior.AllowGet);
                }
                //else
                //{

                //    status = "false/" + "More than 10 Casual leaves cannot apply";
                //    return Json(new { message = status, stdate = star1, eddate = end1 }, JsonRequestBehavior.AllowGet);
                //}

            }
            if (ltypes == "CW-OFF")
            {
                int isbefore = CLBeforeholidays(star1, end1, LeaveType);
                int isafter = CLAfterholidays(star1, end1, LeaveType);
                int TotalClLeavedays = isbefore + isafter + UiStartenddiff;
                if (TotalClLeavedays <= 7)
                {
                    status = "countzero/" + "Leave apply Starts From " + star1.ToShortDateString() + "  To " + end1.ToShortDateString();
                    return Json(new { message = status, stdate = star1, eddate = end1 }, JsonRequestBehavior.AllowGet);
                }
               
            }
            if (ltypes == "PL" || ltypes== "SCL")
            {
                int isbefore = CLBeforeholidays(star1, end1, LeaveType);
                int isafter = CLAfterholidays(star1, end1, LeaveType);
                int TotalLeavedays = isbefore + isafter + UiStartenddiff;
                
                    status = "countzero/" + "Leave apply Starts From " + star1.ToShortDateString() + "  To " + end1.ToShortDateString();
                    return Json(new { message = status, stdate = star1, eddate = end1 }, JsonRequestBehavior.AllowGet);
            }
            
           
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);
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
            if (lfirst == 4)
            {
                lfirstname = FirstName.Substring(0, 2);
                if (lfirstname == "DR" || lfirstname == "Dr")
                {
                    lfirstname = lfirstname + "." + FirstName.Substring(2, 2);
                }
                else
                {
                    lfirstname = FirstName.Substring(0, 1);
                }
            }
            if (lfirst == 1)
            {
                lfirstname = FirstName.Substring(0, 1);
            }
            if (lfirst == 2)
            {
                lfirstname = FirstName.Substring(0, 2);
            }
            lfirstname = lfirstname + " " + LastName;
            return lfirstname;
        }

        [HttpGet]
        public JsonResult CheckCombinationLeaves(string StartDate, string EndDate, int LeaveType)
        {
            string status1 = "";
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
            var lholidaylist = db.HolidayList.ToList();
            string lcode = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
            DateTime star1 = DateTime.Parse(StartDate);
            DateTime end1 = DateTime.Parse(EndDate);
            int UiStartenddiff = (end1 - star1).Days;
            UiStartenddiff = UiStartenddiff + 1;
            int holidayCount = db.HolidayList.Where(a => a.Date < end1).Count();
            DateTime? nextHoliday12 = db.HolidayList.Where(a => a.Date > end1).Min(a => (DateTime?)a.Date);
            DateTime nextHoliday1 = Convert.ToDateTime(nextHoliday12);
            string ltypes = db.LeaveTypes.Where(a => a.Id == LeaveType).Select(a => a.Code).FirstOrDefault();
            if (nextHoliday1 != null && (nextHoliday1 - end1).TotalDays != 0 && ltypes == "MTL")
            {
                end1 = end1.AddDays(0);
            }
            else if (nextHoliday1 != null && (nextHoliday1 - end1).TotalDays != 0 && ltypes == "PTL")
            {
                end1 = end1.AddDays(0);
            }
            string ltype = db.LeaveTypes.Where(a => a.Id == LeaveType).Select(a => a.Code).FirstOrDefault();
            if (lcode == "Watchman")
            {
                status1 = "";
                return Json(new { message = status1 }, JsonRequestBehavior.AllowGet);
            }
            else if (ltypes != "CL")
            {
                DateTime lStartDate = isStartDateAdjustmentRequired(star1, LeaveType);
                DateTime lEndDate = isEndDateAdjustmentRequired(end1, LeaveType);
                if (lStartDate == lEndDate)
                {
                    status1 = "";
                    return Json(new { message = status1 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    status1 = "Leave apply Starts From " + lStartDate.ToShortDateString() + "  To " + lEndDate.ToShortDateString();
                    return Json(new { message = status1, stdate = lStartDate, eddate = lEndDate }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { message = status1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult LeaveRequest(LeaveViewModel leaves, FormCollection form)
        {
            
            LogInformation.Info("Leaveapply Code started ");
            MobileApplyLeaveDTO leavedata = new MobileApplyLeaveDTO();

            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            DateTime lStartDate = leaves.lleaves.StartDate;
            DateTime lEndDate = leaves.lleaves.EndDate;
            string empcode = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            //string empid = lEmpId.ToString();
            string startdt = lStartDate.ToString("yyyy-MM-dd");
            string enddt = lEndDate.ToString("yyyy-MM-dd");
            string statuss = leaves.lleaves.Status;
            LeavesBusiness Lbus = new LeavesBusiness();
            var dtwd = Lbus.getcheckLTCWDOD(lCredentials.EmpPkId, empcode, startdt, enddt, statuss);
            if (dtwd != "" && dtwd!="WD") // if already work diary appplied again leave is applying on that day need to allow
            {
                TempData["status"] = "Please Check the date range already applied in  " + dtwd;
            }            
            else if(dtwd == "WD")
            {
                // if already work diary appplied again leave is applying on that day need to allow by deleting workdiary records
                SqlHelper sh = new SqlHelper();
                string str = "select * from WorkDiary where empid = " + empcode + " and(WDDate >=  '" + startdt + "' AND WDDate <= '" + enddt + "');";
                DataTable dt = sh.Get_Table_FromQry(str);
                if (dt.Rows.Count > 0)
                {
                    string deleteworkdairy = " delete from WorkDiary_Det where WDId = (select Id from WorkDiary where empid = " + empcode + " and(WDDate >=  '" + startdt + "' AND WDDate <= '" + enddt + "')); delete from WorkDiary where empid = " + empcode + " and(WDDate >=  '" + startdt + "' AND WDDate <= '" + enddt + "');";
                    sh.Run_UPDDEL_ExecuteNonQuery(deleteworkdairy);
                }
                int lControlling = Convert.ToInt32(Session["lcontrols"].ToString());
                int lSacantioning = Convert.ToInt32(Session["lSancation"].ToString());
                int lType = leaves.lleaves.Id;

                leavedata.MatrenityType = form["MaternityType"];
                leavedata.DeliveryDate = form["lDelivery.DeliveryDate"];
                leavedata.EmpId = lEmpId;
                leavedata.LeaveTypeId = lType;
                leavedata.ControllingAuthority = Convert.ToString(lControlling);
                leavedata.SanctioningAuthority = Convert.ToString(lSacantioning);
                leavedata.StartDate = Convert.ToString(lStartDate);
                leavedata.EndDate = Convert.ToString(lEndDate);
                leavedata.Reason = leaves.lleaves.Reason;
                leavedata.Year = DateTime.Now.Year;
                DateTime lstartdatecl = lStartDate.AddDays(-1);
                DateTime lenddatecl = lEndDate.AddDays(1);
                string lstar = lStartDate.ToString("yyyy-MM-dd");
                string lend = lEndDate.ToString("yyyy-MM-dd");
                int ltype = leavedata.LeaveTypeId;
                try
                {
                    DateTime? Retirement = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.RetirementDate).FirstOrDefault();
                    if (Retirement >= lEndDate)
                    {
                        //LeavesBusiness Lbus = new LeavesBusiness();
                        //var dtwd = Lbus.CLMLPLCombinations(lEmpId, lstar, lend, ltype);

                        //if (dtwd != "" && ltype.ToString() == "1" && dtwd != "1")
                        //{
                        //    TempData["status"] = "Any other LeaveTypes Cannot be applied in combination of CL";
                        //}
                        //else if (dtwd != "" && ltype.ToString() != "1" && dtwd == "1")
                        //{
                        //    TempData["status"] = "Any other LeaveTypes Cannot be applied in combination of CL";
                        //}

                        //else
                        //{
                        string result = LeaveHelper.CreateLeave(leavedata);
                        LogInformation.Info("LeaveapplyWeb, Success. Info: " + result);

                        TempData["status"] = result;
                    }
                    //}
                    else
                    {
                        TempData["status"] = "The Selected Dates should be less than or equal to the Retirement Date" + "  " + Retirement.Value.ToShortDateString() + "  " + "Please select other dates.";
                        return RedirectToAction("LeaveRequest");
                    }
                }

                catch (Exception ex)
                {
                    LogInformation.Info("LeaveapplyWeb, Error. Info: " + ex.Message);
                    TempData["status"] = "Error:" + ex.Message;
                }
            }
            else 
            {
                int lControlling = Convert.ToInt32(Session["lcontrols"].ToString());
                int lSacantioning = Convert.ToInt32(Session["lSancation"].ToString());
                int lType = leaves.lleaves.Id;

                leavedata.MatrenityType = form["MaternityType"];
                leavedata.DeliveryDate = form["lDelivery.DeliveryDate"];
                leavedata.EmpId = lEmpId;
                leavedata.LeaveTypeId = lType;
                leavedata.ControllingAuthority = Convert.ToString(lControlling);
                leavedata.SanctioningAuthority = Convert.ToString(lSacantioning);
                leavedata.StartDate = Convert.ToString(lStartDate);
                leavedata.EndDate = Convert.ToString(lEndDate);
                leavedata.Reason = leaves.lleaves.Reason;
                leavedata.Year = DateTime.Now.Year;
                DateTime lstartdatecl = lStartDate.AddDays(-1);
                DateTime lenddatecl = lEndDate.AddDays(1);
                string lstar = lStartDate.ToString("yyyy-MM-dd");
                string lend = lEndDate.ToString("yyyy-MM-dd");
                int ltype = leavedata.LeaveTypeId;
                try
                {
                    DateTime? Retirement = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.RetirementDate).FirstOrDefault();
                    if (Retirement >= lEndDate)
                    {
                        //LeavesBusiness Lbus = new LeavesBusiness();
                        //var dtwd = Lbus.CLMLPLCombinations(lEmpId, lstar, lend, ltype);

                        //if (dtwd != "" && ltype.ToString() == "1" && dtwd != "1")
                        //{
                        //    TempData["status"] = "Any other LeaveTypes Cannot be applied in combination of CL";
                        //}
                        //else if (dtwd != "" && ltype.ToString() != "1" && dtwd == "1")
                        //{
                        //    TempData["status"] = "Any other LeaveTypes Cannot be applied in combination of CL";
                        //}

                        //else
                        //{
                        string result = LeaveHelper.CreateLeave(leavedata);
                        LogInformation.Info("LeaveapplyWeb, Success. Info: " + result);

                        TempData["status"] = result;
                    }
                    //}
                    else
                    {
                        TempData["status"] = "The Selected Dates should be less than or equal to the Retirement Date" + "  " + Retirement.Value.ToShortDateString() + "  " + "Please select other dates.";
                        return RedirectToAction("LeaveRequest");
                    }
                }

                catch (Exception ex)
                {
                    LogInformation.Info("LeaveapplyWeb, Error. Info: " + ex.Message);
                    TempData["status"] = "Error:" + ex.Message;
                }
            }

            LogInformation.Info("Leaveapply Code Ended");
            return RedirectToAction("LeaveRequest");
        }

      
        

        public DateTime isStartDateAdjustmentRequired(DateTime Startdate, int LeaveType)
        {
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                int hdays = 0;
                DateTime? lastLeave1 = db.Leaves.Where(a => a.EndDate < Startdate).Where(a => a.EmpId == lEmpId).Max(a => (DateTime?)a.EndDate);
                DateTime LastleaveTemp = Convert.ToDateTime(lastLeave1);
                DateTime? StartHoliday = db.HolidayList.Where(a => a.Date == Startdate).Select(a => (DateTime?)a.Date).FirstOrDefault();
                if (lastLeave1 != null)
                {
                    DateTime Temp1 = Convert.ToDateTime(lastLeave1);
                    int lLeavetype = db.Leaves.Where(a => a.EndDate == Temp1).Where(a => a.EmpId == lEmpId).Select(a => a.LeaveType).FirstOrDefault();
                    int leavetype1 = db.LeaveTypes.Where(a => a.Code == "CL").Select(a => a.Id).FirstOrDefault();
                    if (lLeavetype == leavetype1)
                    {
                        //  return Startdate;
                    }
                }
                else
                {
                    return Startdate;
                }
                DateTime lastHoliday1 = db.HolidayList.Where(a => a.Date < Startdate).Max(a => a.Date);

                if (lastHoliday1 != null && (Startdate - lastHoliday1).TotalDays == 1)
                {
                    hdays = 1;
                    DateTime lastHoliday2 = db.HolidayList.Where(a => a.Date < lastHoliday1).Max(a => a.Date);
                    if (lastHoliday2 != null && (lastHoliday1 - lastHoliday2).TotalDays == 1)

                    {
                        hdays = 2;
                        DateTime lastHoliday3 = db.HolidayList.Where(a => a.Date < lastHoliday2).Max(a => a.Date);
                        if (lastHoliday3 != null && (lastHoliday2 - lastHoliday3).TotalDays == 1)

                        {
                            hdays = 3;
                            DateTime lastHoliday4 = db.HolidayList.Where(a => a.Date < lastHoliday3).Max(a => a.Date);
                            if (lastHoliday4 != null && (lastHoliday3 - lastHoliday4).TotalDays == 1)

                            {
                                hdays = 4;
                            }

                        }
                    }
                }
                if (hdays != 0)
                {

                    DateTime Temp1 = Convert.ToDateTime(lastLeave1);

                    double ldays = (Startdate - Temp1).TotalDays;

                    if (ldays <= hdays + 1)
                    {
                        Startdate = Startdate.AddDays(-(--ldays));
                    }
                    return Startdate;
                }
            }

            catch (Exception ex)
            {
                ex.ToString();
            }
            return Startdate;

        }

        public DateTime isEndDateAdjustmentRequired(DateTime Enddate, int LeaveType)
        {
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                int hdays = 0;
                DateTime? nextLeave1 = db.Leaves.Where(a => a.StartDate > Enddate).Where(a => a.EmpId == lEmpId).Min(a => (DateTime?)a.StartDate);
                if (nextLeave1 != null)
                {
                    DateTime Temp1 = Convert.ToDateTime(nextLeave1);
                    int lLeavetype = db.Leaves.Where(a => a.StartDate == Temp1).Where(a => a.EmpId == lEmpId).Select(a => a.LeaveType).FirstOrDefault();
                    int leavetype1 = db.LeaveTypes.Where(a => a.Code == "CL").Select(a => a.Id).FirstOrDefault();
                    if (lLeavetype == leavetype1)
                    {
                        //  return Enddate;
                    }
                }
                else
                {
                    return Enddate;
                }
                DateTime nextHoliday1 = db.HolidayList.Where(a => a.Date > Enddate).Min(a => a.Date);
                if (nextHoliday1 != null && (nextHoliday1 - Enddate).TotalDays == 1)
                {
                    hdays = 1;
                    DateTime nextHoliday2 = db.HolidayList.Where(a => a.Date > nextHoliday1).Min(a => a.Date);
                    if (nextHoliday2 != null && (nextHoliday2 - nextHoliday1).TotalDays == 1)

                    {
                        hdays = 2;
                        DateTime nextHoliday3 = db.HolidayList.Where(a => a.Date > nextHoliday2).Min(a => a.Date);
                        if (nextHoliday3 != null && (nextHoliday3 - nextHoliday2).TotalDays == 1)

                        {
                            hdays = 3;
                            DateTime nextHoliday4 = db.HolidayList.Where(a => a.Date > nextHoliday3).Min(a => a.Date);

                            if ((nextHoliday4 - nextHoliday3).TotalDays == 1)
                            {
                                hdays = 4;
                            }

                        }
                    }
                }
                if (hdays != 0)
                {
                    DateTime Temp = Convert.ToDateTime(nextLeave1);
                    double ldays = (Temp - Enddate).TotalDays;

                    if (ldays <= hdays + 1)
                    {
                        Enddate = (Enddate).AddDays(--ldays);

                    }
                    return Enddate;
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return Enddate;
        }
        [HttpGet]
        public JsonResult GetDates()
        {
            List<string> Dates = new List<string>();
            var holiday = db.HolidayList.ToList();
            List<string> lvalues = new List<string>();
            string basedate = "1900-01-01 12:00:00.000";
            DateTime ccDadte = DateTime.Parse(basedate);
            var lResults = (from holidaylist in holiday
                            where holidaylist.Occasion != "Sunday" && holidaylist.Occasion != "Fourth Saturday" &&
                            holidaylist.Occasion != "Second Saturday" && holidaylist.DeleteAt == ccDadte.Date
                            select new
                            {
                                Dates = holidaylist.Date.ToString("dd/MM/yyyy"),
                            });
            var lresponseArray = lResults.ToArray();
            foreach (
                var item in lresponseArray)
            {
                lvalues.Add(item.Dates);
            }
            return Json(Dates = lvalues.ToList(), JsonRequestBehavior.AllowGet);
        }
        public FileResult CreatePdf()
        {
            String sd = Convert.ToString(Session["lStartDate"]);
            String ed = Convert.ToString(Session["lEndDate"]);
            String ltype = Convert.ToString(Session["lleaveid"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("MyLeaveHistory" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 10 columns  
            PdfPTable tableLayout1 = new PdfPTable(4);
            PdfPTable tableLayout = new PdfPTable(6);
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            doc.Add(Add_Content_To_PDF1(tableLayout1));
            //Add Content to PDF   
            doc.Add(Add_Content_To_PDF(tableLayout));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(byteInfo);
                Font blackFont = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, BaseColor.BLACK);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page:" + i.ToString() + "/" + pages.ToString(), blackFont), 568f, 15f, 0);
                        tableLayout1.FooterRows = 1;
                        tableLayout1.AddCell(new PdfPCell(new Phrase(i.ToString(), new Font(Font.FontFamily.HELVETICA, 2, 1, new BaseColor(0, 0, 0))))
                        {
                            Colspan = 20,
                            Border = 0,
                            PaddingBottom = 5,
                            HorizontalAlignment = Element.ALIGN_LEFT,
                        });
                    }
                }
                byteInfo = stream.ToArray();
            }
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();

            String sd = Convert.ToString(Session["lStartDate"]);
            String ed = Convert.ToString(Session["lEndDate"]);
            String ltype = Convert.ToString(Session["lleaveid"]);

            float[] headers = { 80, 80, 70, 80, 50,70 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            DateTime printdate = DateTime.Now;
            tableLayout.AddCell(new PdfPCell(new Phrase(printdate.Date.ToShortDateString() + " " + printdate.ToShortTimeString(), new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            tableLayout.FooterRows = 1;
            //Add Title to the PDF file at the top  

            List<Leaves> lleaves = db.Leaves.ToList<Leaves>();
            var ldesignation = db.Designations.ToList();
            var lemployees = db.Employes.ToList();
            var lLeaveTypes = db.LeaveTypes.ToList();


            if (sd == "" || ed == "" || ltype == "")
            {

                var data = Rbus.LeaveApplyController(lCredentials.EmpId, lEmpId);
                AddCellToHeader(tableLayout, "StartDate");
                AddCellToHeader(tableLayout, "EndDate");
                AddCellToHeader(tableLayout, "LeaveDays");
                AddCellToHeader(tableLayout, "Reason");
                AddCellToHeader(tableLayout, "Status");
                AddCellToHeader(tableLayout, "Cancel Reason");


                ////Add body  
                for (int i = 0; i < data.Rows.Count; i++)
                {


                    string lsdate = (string)data.Rows[i]["StartDate"].ToString();
                    string lenddate = (string)data.Rows[i]["EndDate"].ToString();
                    string ldays = (string)data.Rows[i]["LeaveDays"].ToString();
                    string llreason = (string)data.Rows[i]["Reason"].ToString();
                    string lstatus = (string)data.Rows[i]["Status"].ToString();
                    string lCancelReason = (string)data.Rows[i]["CancelReason"].ToString();


                    AddCellToBody(tableLayout, lsdate);
                    AddCellToBody(tableLayout, lenddate);
                    AddCellToBody(tableLayout, ldays);
                    AddCellToBody(tableLayout, llreason);
                    AddCellToBody(tableLayout, lstatus);
                    AddCellToBody(tableLayout, lCancelReason);
                }


                return tableLayout;
            }
            int LtypeId = Convert.ToInt32(ltype);
            string lTypes = db.LeaveTypes.Where(a => a.Id == LtypeId).Select(a => a.Type).FirstOrDefault();
            //int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            if (lTypes == "ALL")
            {
                DateTime ToDate = Convert.ToDateTime(ed);
                string strDate = sd;
                string[] sa = strDate.Split('/');
                string strNew = sa[2] + "/" + sa[1] + "/" + sa[0];

                string strDate1 = ed;
                string[] sa1 = strDate1.Split('/');
                string strNew1 = sa1[2] + "/" + sa1[1] + "/" + sa1[0];

                DateTime FromDate = DateTime.ParseExact(strNew, "yyyy/MM/dd", null);
                DateTime Todate = DateTime.ParseExact(strNew1, "yyyy/MM/dd", null);


                DateTime lStartdate = FromDate;
                DateTime lEnddate = ToDate;

                var data = Rbus.SearchLeaveApplyController(lCredentials.EmpId, lEmpId, sd, ed, ltype);
                AddCellToHeader(tableLayout, "StartDate");
                AddCellToHeader(tableLayout, "EndDate");
                AddCellToHeader(tableLayout, "LeaveDays");
                AddCellToHeader(tableLayout, "Reason");
                AddCellToHeader(tableLayout, "Status");
                AddCellToHeader(tableLayout, "Cancel Reason");
                ////Add body  
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    string lsdate = (string)data.Rows[i]["StartDate"].ToString();
                    string lenddate = (string)data.Rows[i]["EndDate"].ToString();
                    string ldays = (string)data.Rows[i]["LeaveDays"].ToString();
                    string llreason = (string)data.Rows[i]["Reason"].ToString();
                    string lstatus = (string)data.Rows[i]["Status"].ToString();
                    string lCancelReason = (string)data.Rows[i]["CancelReason"].ToString();

                    AddCellToBody(tableLayout, lsdate);
                    AddCellToBody(tableLayout, lenddate);
                    AddCellToBody(tableLayout, ldays);
                    AddCellToBody(tableLayout, llreason);
                    AddCellToBody(tableLayout, lstatus);
                    AddCellToBody(tableLayout, lCancelReason);

                }


                return tableLayout;
            }
            else
            {
                DateTime ToDate = Convert.ToDateTime(ed);
                string strDate = sd;
                string[] sa = strDate.Split('/');
                string strNew = sa[2] + "/" + sa[1] + "/" + sa[0];

                string strDate1 = ed;
                string[] sa1 = strDate1.Split('/');
                string strNew1 = sa1[2] + "/" + sa1[1] + "/" + sa1[0];

                DateTime FromDate = DateTime.ParseExact(strNew, "yyyy/MM/dd", null);
                DateTime Todate = DateTime.ParseExact(strNew1, "yyyy/MM/dd", null);


                DateTime lStartdate = FromDate;
                DateTime lEnddate = ToDate;
                var data = Rbus.SearchLeaveApplyController(lCredentials.EmpId, lEmpId, sd, ed, ltype);
                AddCellToHeader(tableLayout, "StartDate");
                AddCellToHeader(tableLayout, "EndDate");
                AddCellToHeader(tableLayout, "LeaveDays");
                AddCellToHeader(tableLayout, "Reason");
                AddCellToHeader(tableLayout, "Status");
                AddCellToHeader(tableLayout, "Cancel Reason"); 
                ////Add body  
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    string lsdate = (string)data.Rows[i]["StartDate"].ToString();
                    string lenddate = (string)data.Rows[i]["EndDate"].ToString();
                    string ldays = (string)data.Rows[i]["LeaveDays"].ToString();
                    string llreason = (string)data.Rows[i]["Reason"].ToString();
                    string lstatus = (string)data.Rows[i]["Status"].ToString();
                    string lCancelReason = (string)data.Rows[i]["CancelReason"].ToString();

                    AddCellToBody(tableLayout, lsdate);
                    AddCellToBody(tableLayout, lenddate);
                    AddCellToBody(tableLayout, ldays);
                    AddCellToBody(tableLayout, llreason);
                    AddCellToBody(tableLayout, lstatus);
                    AddCellToBody(tableLayout, lCancelReason);

                }


                return tableLayout;
            }
        }

        protected PdfPTable Add_Content_To_PDF1(PdfPTable tableLayout1)
        {
            float[] headers1 = { 33, 33, 33, 33 }; //Header Widths  
            tableLayout1.SetWidths(headers1); //Set the pdf headers  
            tableLayout1.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout1.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            tableLayout1.AddCell(new PdfPCell(new Phrase("Leave History", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            List<Leaves> lleaves = db.Leaves.ToList<Leaves>();
            var ldesignation = db.Designations.ToList();
            var lemployees = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartment = db.Departments.ToList();
            var lResults = (from leave in lleaves
                            join emp in lemployees on leave.EmpId equals emp.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join branch in lbranch on emp.Branch equals branch.Id
                            join dept in ldepartment on emp.Department equals dept.Id
                            where lCredentials.EmpId == emp.EmpId

                            select new
                            {
                                EmpCode = emp.EmpId,
                                EmployeeName = emp.ShortName,
                                designation = desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                            }).Distinct();




            //Add header  
            AddCellToHeader(tableLayout1, "EmpCode");
            AddCellToHeader(tableLayout1, "EmpName");
            AddCellToHeader(tableLayout1, "Designation");
            AddCellToHeader(tableLayout1, "Department/Branch");

            ////Add body  

            foreach (var lleave in lResults)
            {

                AddCellToBody(tableLayout1, lleave.EmpCode.ToString());
                AddCellToBody(tableLayout1, lleave.EmployeeName.ToString());
                AddCellToBody(tableLayout1, lleave.designation.ToString());
                AddCellToBody(tableLayout1, lleave.Deptbranch.ToString());


            }

            return tableLayout1;
        }


        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.WHITE)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(60, 141, 188)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        public string HistoryDates(DateTime Startdate, DateTime Enddate)
        {
            string combinedates = "";
            var startday = Startdate.ToString("MMM dd");
            var startYear = Startdate.Year.ToString();
            var endday = Enddate.ToString("MMM dd");
            var endYear = Enddate.Year.ToString();
            if (startYear == endYear)
            {
                combinedates = startday + " " + "To" + " " + endday + "," + " " + startYear;
                return combinedates;
            }
            else
            {
                combinedates = startday + " " + "-" + startYear + " " + "To" + " " + endday + "," + " " + endYear;
                return combinedates;
            }


        }

        public int CLBeforeholidays(DateTime Startdate, DateTime Enddate, int LeaveType)
        {
            int totaldays = 0;
            totaldays = IsCLLeaveOrHolidayday(Startdate, LeaveType, Enddate);
            return totaldays;
        }
        public int IsCLHoliday(DateTime Date, int LeaveType)
        {
            int hdays = 0;
            hdays = db.HolidayList.Where(a => a.Date == Date).Count();
            return hdays;

        }
        public int IsCLLeaveOrHolidayday(DateTime Startdate, int LeaveType, DateTime Enddate)
        {
            var hdays = 0;
            var ldays = 0;
            var hdays1 = 0;
            var ldays1 = 0;
            int TotalDays;
            int TotalDays1 = 0;
            DateTime BeforeStartdate = Startdate.AddDays(-1);
            DateTime tenthStartDay = Startdate.AddDays(-10);

            for (DateTime date = BeforeStartdate.Date; date.Date >= tenthStartDay; date = date.AddDays(-1))
            {
                hdays1 = IsCLHoliday(date, LeaveType);

                if (hdays1 == 0)
                {
                    ldays1 = IsCLLeaveday(date, LeaveType, Enddate);
                }
                else
                {
                    ldays1 = 0;
                }

                if (hdays1 == 0 && ldays1 == 0)
                {
                    TotalDays1 = hdays + ldays;
                    return TotalDays1;
                }
                else
                {
                    hdays += hdays1;
                    ldays += ldays1;
                }
            }

            TotalDays = hdays + ldays + TotalDays1;
            return TotalDays;
        }
        public int IsCLLeaveday(DateTime Startdate, int LeaveType, DateTime Enddate)
        {
            string ltypes = db.LeaveTypes.Where(a => a.Id == LeaveType).Select(a => a.Code).FirstOrDefault();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int leavetype = Convert.ToInt32(LeaveType);
            int leavetype1 = db.LeaveTypes.Where(a => a.Code == ltypes).Select(a => a.Id).FirstOrDefault();
            // var ldays = db.Leaves.Where(a => a.EndDate < Startdate).Where(a => a.EmpId == lEmpId).Where(a => a.LeaveType == leavetype1).Select(a => a.TotalDays).FirstOrDefault();           
            List<Leaves> lStartCount = db.Leaves.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveType == leavetype1).Where(a => Startdate <= a.EndDate && Startdate >= a.StartDate).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.Status != "PartialCancelled").Where(a => a.Status != "Debited").ToList();
            int ldays = lStartCount.Count();
            return ldays;
        }

        public int CLAfterholidays(DateTime Startdate, DateTime Enddate, int LeaveType)
        {
            int totaldays = 0;
            totaldays = IsCLLeaveOrHolidayday1(Enddate, LeaveType, Startdate);
            return totaldays;
        }
        public int IsCLLeaveOrHolidayday1(DateTime Enddate, int LeaveType, DateTime Startdate)
        {
            var hdays = 0;
            var ldays = 0;
            var hdays1 = 0;
            var ldays1 = 0;
            int TotalDays;
            int TotalDays1 = 0;
            DateTime AfterStartdate = Enddate.AddDays(+1);
            DateTime tenthStartDay = Enddate.AddDays(+10);

            for (DateTime date = AfterStartdate.Date; date.Date <= tenthStartDay; date = date.AddDays(+1))
            {
                hdays1 = IsCLHoliday(date, LeaveType);

                if (hdays1 == 0)
                {
                    ldays1 = IsCLLeaveday1(date, LeaveType, Enddate);
                }
                else
                {
                    ldays1 = 0;
                }
                if (hdays1 == 0 && ldays1 == 0)
                {
                    TotalDays1 = hdays + ldays;
                    return TotalDays1;
                }
                else
                {
                    hdays += hdays1;
                    ldays += ldays1;
                }
            }

            TotalDays = hdays + ldays + TotalDays1;
            return TotalDays;
        }
        public int IsCLLeaveday1(DateTime Enddate, int LeaveType, DateTime Startdate)
        {
            string ltypes = db.LeaveTypes.Where(a => a.Id == LeaveType).Select(a => a.Code).FirstOrDefault();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int leavetype = Convert.ToInt32(LeaveType);
            int leavetype1 = db.LeaveTypes.Where(a => a.Code == ltypes).Select(a => a.Id).FirstOrDefault();
            // var ldays = db.Leaves.Where(a => a.EndDate < Startdate).Where(a => a.EmpId == lEmpId).Where(a => a.LeaveType == leavetype1).Select(a => a.TotalDays).FirstOrDefault();
            List<Leaves> lEndCount = db.Leaves.Where(a => a.EmpId == lEmpId).Where(a => a.EndDate >= Enddate && a.StartDate <= Enddate).Where(a => a.LeaveType == leavetype1).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.Status != "PartialCancelled").Where(a => a.Status != "Debited").ToList();
            int ldays = lEndCount.Count();
            return ldays;
        }
    }
}




