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
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.UI.WebControls;
using System.Web.UI;
using HRMSBusiness.Business;
using System.Threading.Tasks;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class DutyController : Controller
    {
        
        private ContextBase db = new ContextBase();
        ODHelper lhelper = new ODHelper();
        

        LoginCredential lCredentials = LoginHelper.GetCurrentUser();


        public ActionResult UnlockOD()
        {
            //var a = TempData["AlertMessage"];
            //if(a !=null)
            //{
            //    TempData["AlertMessage"] = "OD Unlocked Successfully";
            //}
            
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();

            return View();

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UnlockOD(OD_Unlock odu, string EmpCode)
        {
            OD_Unlock odl = new OD_Unlock();
            odl.EmpId = odu.EmpId;
            odl.UpdatedAt = GetCurrentTime(DateTime.Now);
            odl.FromDate = odu.FromDate;
            odl.ToDate = odu.ToDate;
            odl.UpdatedBy = lCredentials.EmpId;
            odl.Note = odu.Note;
            string sts1 = lhelper.CheckUnlockOd(odu.EmpId, odl.FromDate, odl.ToDate);
            if (sts1 == "1")
            {
                TempData["AlertMessage"] = "Please Check the date range already Unlock ";
            }
            else
            {
                db.OD_Unlock.Add(odl);
                db.SaveChanges();
                TempData["AlertMessage"] = "OD Unlocked Successfully";
            }
            

            return RedirectToAction("UnlockOD");

        }

       

        // GET: Duty
        [HttpGet]
        public ActionResult Index()
        {
            string lMessage = string.Empty;

            if (TempData["AlertMessage"] != null)
            {
                lMessage = TempData["AlertMessage"].ToString();
            }
            ViewBag.Message = lCredentials.LoginMode;
            TempData["Loginmode"] = lCredentials.LoginMode;
            var items = Facade.EntitiesFacade.GetAllODMaster().Where(a => a.Status == true).Select(x => new OD_Master
            {
                Id = x.Id,
                ODType = x.ODType
            });
            ViewBag.Purpose = new SelectList(items, "Id", "ODType");
            int lEmId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            var data = new Persistence.EmployeesRepository().GetIt(lEmId);
            var selected = (from sub in db.Employes where sub.Id == lEmId select sub.Branch).FirstOrDefault();
            if (selected == 43)
            {
                selected = 42;
                var items5 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.Name != "OtherBranch").Where(a => a.Name != "TBD").Select(x => new Branches
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.Branch = new SelectList(items5, "Id", "Name", selected);
            }

            else
            {
                var items5 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.Name != "OtherBranch").Where(a => a.Name != "TBD").Select(x => new Branches
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.Branch = new SelectList(items5, "Id", "Name", selected);
            }


            var lmodel = new OD_OtherDuty { Loginmode = lCredentials.LoginMode, VistorFrom = selected };
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View(lmodel);
        }
        public DateTime[] GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date.Date);
            return allDates.ToArray();
        }
        public TimeSpan[] GetTimeBetween1(DateTime startDate, DateTime endDate)
        {
            List<TimeSpan> allDates = new List<TimeSpan>();
            string startTime = startDate.ToString("HH:mm:ss");
            string endTime = endDate.ToString("HH:mm:ss");
            TimeSpan starttime1 = TimeSpan.Parse(startTime);
            TimeSpan enddate1 = TimeSpan.Parse(endTime);
            TimeSpan time1 = TimeSpan.FromHours(1);
            for (TimeSpan date = starttime1; date <= enddate1; date = date.Add(time1))
                allDates.Add(date);
            return allDates.ToArray();
        }
        [HttpGet]
        public JsonResult checkLeaveEligebleOrNot(string StartDate, string EndDate)

        {
            string status = "";
            DateTime star1 = DateTime.Parse(StartDate);
            DateTime startdate = DateTime.Parse(StartDate).Date;
            DateTime enddate = DateTime.Parse(EndDate).Date;
            DateTime end1 = DateTime.Parse(EndDate);
            string lstar = star1.ToString("yyyy-MM-dd");
            string lend = end1.ToString("yyyy-MM-dd");
            string startime = star1.ToString("HH:mm:ss");
            string endtime = end1.ToString("HH:mm:ss");
            TimeSpan startime1 = TimeSpan.Parse(startime);
            TimeSpan endtime1 = TimeSpan.Parse(endtime);
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int emplevescount = db.OD_OtherDuty.Where(a => a.EmpId == lEmpId).Count();
            LeavesBusiness Lbus = new LeavesBusiness();
            string Empcode = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            var dtwd = Lbus.getcheckLTCWDOD(lCredentials.EmpPkId, Empcode, lstar, lend, status);
            if (dtwd != "" && dtwd != "OD" && dtwd != "WD")
            {
                // status = "false/" + star1.ToShortDateString() + " , " + end1.ToShortDateString() + " " + "Already these dates are applied in " + dtwd;
                status = "false/" + "Please Check the date range already applied in  " + dtwd;
                //return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            }

            //if (emplevescount > 0)
            //{
            //    string str = "";
            //    List<OD_OtherDuty> lStartEndCount = db.OD_OtherDuty.Where(a => a.EmpId == lEmpId).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").ToList();
            //    foreach (OD_OtherDuty l in lStartEndCount)
            //    {

            //        DateTime star = l.StartDate;
            //        DateTime end = l.EndDate;
            //        DateTime[] dates = GetDatesBetween(star, end).ToArray();
            //        TimeSpan[] time = GetTimeBetween1(star, end).ToArray();
            //        string lstartime = star.ToString("HH:mm:ss");
            //        string lendtime = end.ToString("HH:mm:ss");
            //        DateTime lstartime1 = DateTime.Parse(lstartime);
            //        DateTime lendtime1 = DateTime.Parse(lendtime);
            //        for (int i = 0; i < dates.Length; i++)
            //        {
            //            DateTime d = dates[i];
            //            for (int J = 0; J < time.Length; J++)
            //            {
            //                TimeSpan t = time[J];

            //                if ((startdate == dates[i] || dates[i] == enddate) && (startime1 <= t && t <= endtime1))
            //                {
            //                    //true condition already applied
            //                    str = str + dates[i].ToShortDateString();
            //                }

            //                if (str != "")
            //                {
            //                    status = "false/" + str + "--Already OD applied in these dates.";
            //                    return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            //                }

            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    status = "countzero";
            //}
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [ValidateInput(false)]
        public JsonResult DeputationHistorySearchViews(string StartDate, string EndDate)
        {
            string lMessage = string.Empty;
            Session["sd"] = StartDate;
            Session["ed"] = EndDate;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();

                if (StartDate == "" || EndDate == "")
                {
                    var ldeputation = db.OD_OtherDuty.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var lodmaster = db.OD_Master.ToList();
                    var ldesignation = db.Designations.ToList();

                    var lResults = (from otherduty in ldeputation
                                    join emp in lemployees on otherduty.EmpId equals emp.Id
                                    join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                    join branch in lBranches on otherduty.BranchId equals branch.Id
                                    join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                    join dept in Departments on otherduty.DepartmentId equals dept.Id
                                    join lodmst in lodmaster on otherduty.Purpose equals lodmst.Id
                                    where otherduty.EmpId == lEmpId
                                    select new
                                    {
                                        otherduty.Id,
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        designation = desig.Code,
                                        VistorFrom = visitbran.Name,
                                        otherduty.VistorTo,
                                        otherduty.UpdatedDate,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        otherduty.StartDate,
                                        otherduty.EndDate,
                                        Purpose = lodmst.ODType,
                                        Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                        otherduty.Status,
                                        otherduty.Description,
                                        otherduty.CancelReason
                                    }).OrderByDescending(a => a.UpdatedDate);
                    return Json(lResults, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DateTime ToDate = Convert.ToDateTime(EndDate);
                    string strDate = StartDate;
                    string[] sa = strDate.Split('/');
                    string strNew = sa[2] + "/" + sa[1] + "/" + sa[0];
                    string strDate1 = EndDate;
                    string[] sa1 = strDate1.Split('/');
                    string strNew1 = sa1[2] + "/" + sa1[1] + "/" + sa1[0];
                    DateTime FromDate = DateTime.ParseExact(strNew, "yyyy/MM/dd", null);
                    DateTime Todate = DateTime.ParseExact(strNew1, "yyyy/MM/dd", null);
                    var ldeputation = db.OD_OtherDuty.ToList();
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
                                    join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                    join branch in lBranches on otherduty.BranchId equals branch.Id
                                    join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                    join dept in Departments on otherduty.DepartmentId equals dept.Id
                                    join lodmst in lodmaster on otherduty.Purpose equals lodmst.Id
                                    where ((otherduty.StartDate.Date >= lStartdate && otherduty.EndDate.Date <= lEnddate)
                                     || (otherduty.EndDate.Date >= lStartdate && otherduty.StartDate.Date <= lEnddate)) && otherduty.EmpId == lEmpId
                                    select new
                                    {
                                        otherduty.Id,
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        designation = desig.Code,
                                        VistorFrom = visitbran.Name,
                                        otherduty.VistorTo,
                                        otherduty.UpdatedDate,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        otherduty.StartDate,
                                        otherduty.EndDate,
                                        Purpose = lodmst.ODType,
                                        Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                        otherduty.Status,
                                        otherduty.Description,
                                        otherduty.CancelReason
                                    }).OrderByDescending(a => a.UpdatedDate);
                    return Json(lResults, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        public ActionResult GetAuthorityNames()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAuthorityNamess(string Name)
        {
            string lresult = string.Empty;
            try
            {
                var employees = db.Employes.ToList();
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
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;
        }
        public string GetBranchDepartmentConcat1(string branch, string Department)
        {
            
            string requireformate = "";
            if (branch != "OtherBranch")
            {
                Department = branch;
            }
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
        public static DateTime GetCurrentTime(DateTime ldate)
        {
            DateTime serverTime = DateTime.Now;
            DateTime utcTime = serverTime.ToUniversalTime();
            // convert it to Utc using timezone setting of server computer
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(OD_OtherDuty duty)
        {
            DateTime? Retirement = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.RetirementDate).FirstOrDefault();
            if (Retirement >= duty.EndDate)
            {
                //duty.UpdatedDate = 
                duty.UpdatedDate = GetCurrentTime(DateTime.Now);
                string dt = duty.UpdatedDate.ToShortDateString();
                //string dt = "08-04-2024";
                DateTime dt1 = DateTime.Parse(dt);


                DateTime ed = duty.EndDate;
                string ed1 = ed.ToShortDateString();
                DateTime ed2 = DateTime.Parse(ed1);

                //if (duty.UpdatedDate > duty.EndDate)
                if (dt1 > ed2)
                {
                    TimeSpan difference = dt1 - ed2;
                    int diff = (int)difference.TotalDays;
                    int count = 0;
                    // int diff = Convert.ToInt32(difference);
                    if (diff == 1)
                    {
                        int lcontrolling = Convert.ToInt32(Session["lcontrols"].ToString());
                        int lsanctioning = Convert.ToInt32(Session["lSancation"].ToString());
                        int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                        duty.UpdatedBy = lCredentials.EmpId;
                        duty.EmpId = lEmpId;
                        duty.UpdatedDate = GetCurrentTime(DateTime.Now);
                        duty.Status = "Pending";
                        duty.ControllingAuthority = lcontrolling;
                        duty.SanctioningAuthority = lsanctioning;
                        duty.BranchId = Convert.ToInt32(lCredentials.Branch);
                        duty.DepartmentId = Convert.ToInt32(lCredentials.Department);
                        duty.DesignationId = Convert.ToInt32(lCredentials.Designation);
                        db.OD_OtherDuty.Add(duty);
                        db.SaveChanges();
                        string lApply = "HRMSOD";
                        string lAppValue = "0";
                        string lresultsSms = lhelper.SendSms1(duty.StartDate, duty.EndDate, duty.ControllingAuthority, duty.SanctioningAuthority, duty.EmpId, duty.VistorFrom, duty.VistorTo, duty.Purpose, lApply, lAppValue);
                        string lresult = lhelper.SendEmails(duty.StartDate, duty.EndDate, duty.ControllingAuthority, duty.SanctioningAuthority, lEmpId, duty.VistorFrom, duty.VistorTo, duty.Purpose, duty.Description, lApply, lAppValue);
                        TempData["AlertMessage"] = "OD Applied Successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        for (int i = 1; i <= diff; i++)
                        {
                             DateTime enddate = ed2.AddDays(i);
                            string sts = lhelper.CheckHoliday(enddate);
                            if (sts == "Holiday")
                            {
                                if (count == 1)
                                {
                                    string sts1 = lhelper.CheckUnlockOd(lCredentials.EmpId, duty.StartDate, duty.EndDate);
                                    if (sts1 == "0")
                                    {
                                        TempData["AlertMessage"] = "OD can be applied upto one working day post OD date, contact HR!";
                                        return RedirectToAction("Index");
                                    }
                                    else
                                    {
                                        int lcontrolling = Convert.ToInt32(Session["lcontrols"].ToString());
                                        int lsanctioning = Convert.ToInt32(Session["lSancation"].ToString());
                                        int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                                        duty.UpdatedBy = lCredentials.EmpId;
                                        duty.EmpId = lEmpId;
                                        duty.UpdatedDate = GetCurrentTime(DateTime.Now);
                                        duty.Status = "Pending";
                                        duty.ControllingAuthority = lcontrolling;
                                        duty.SanctioningAuthority = lsanctioning;
                                        duty.BranchId = Convert.ToInt32(lCredentials.Branch);
                                        duty.DepartmentId = Convert.ToInt32(lCredentials.Department);
                                        duty.DesignationId = Convert.ToInt32(lCredentials.Designation);
                                        db.OD_OtherDuty.Add(duty);
                                        db.SaveChanges();
                                        string lApply = "HRMSOD";
                                        string lAppValue = "0";
                                        string lresultsSms = lhelper.SendSms1(duty.StartDate, duty.EndDate, duty.ControllingAuthority, duty.SanctioningAuthority, duty.EmpId, duty.VistorFrom, duty.VistorTo, duty.Purpose, lApply, lAppValue);
                                        string lresult = lhelper.SendEmails(duty.StartDate, duty.EndDate, duty.ControllingAuthority, duty.SanctioningAuthority, lEmpId, duty.VistorFrom, duty.VistorTo, duty.Purpose, duty.Description, lApply, lAppValue);
                                        TempData["AlertMessage"] = "OD Applied Successfully";
                                        return RedirectToAction("Index");
                                    }
                                    //TempData["AlertMessage"] = "OD can be applied upto one working day post OD date.";
                                   
                                }
                               
                            }
                            else
                            {
                                count++;
                            }
                        }
                        if(count<2)
                        {
                            int lcontrolling = Convert.ToInt32(Session["lcontrols"].ToString());
                            int lsanctioning = Convert.ToInt32(Session["lSancation"].ToString());
                            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                            duty.UpdatedBy = lCredentials.EmpId;
                            duty.EmpId = lEmpId;
                            duty.UpdatedDate = GetCurrentTime(DateTime.Now);
                            duty.Status = "Pending";
                            duty.ControllingAuthority = lcontrolling;
                            duty.SanctioningAuthority = lsanctioning;
                            duty.BranchId = Convert.ToInt32(lCredentials.Branch);
                            duty.DepartmentId = Convert.ToInt32(lCredentials.Department);
                            duty.DesignationId = Convert.ToInt32(lCredentials.Designation);
                            db.OD_OtherDuty.Add(duty);
                            db.SaveChanges();
                            string lApply = "HRMSOD";
                            string lAppValue = "0";
                            string lresultsSms = lhelper.SendSms1(duty.StartDate, duty.EndDate, duty.ControllingAuthority, duty.SanctioningAuthority, duty.EmpId, duty.VistorFrom, duty.VistorTo, duty.Purpose, lApply, lAppValue);
                            string lresult = lhelper.SendEmails(duty.StartDate, duty.EndDate, duty.ControllingAuthority, duty.SanctioningAuthority, lEmpId, duty.VistorFrom, duty.VistorTo, duty.Purpose, duty.Description, lApply, lAppValue);
                            TempData["AlertMessage"] = "OD Applied Successfully";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            
                            string sts = lhelper.CheckUnlockOd(lCredentials.EmpId,duty.StartDate, duty.EndDate);
                            if (sts == "0")
                            {
                                TempData["AlertMessage"] = "OD can be applied upto one working day post OD date, contact HR!";
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                int lcontrolling = Convert.ToInt32(Session["lcontrols"].ToString());
                                int lsanctioning = Convert.ToInt32(Session["lSancation"].ToString());
                                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                                duty.UpdatedBy = lCredentials.EmpId;
                                duty.EmpId = lEmpId;
                                duty.UpdatedDate = GetCurrentTime(DateTime.Now);
                                duty.Status = "Pending";
                                duty.ControllingAuthority = lcontrolling;
                                duty.SanctioningAuthority = lsanctioning;
                                duty.BranchId = Convert.ToInt32(lCredentials.Branch);
                                duty.DepartmentId = Convert.ToInt32(lCredentials.Department);
                                duty.DesignationId = Convert.ToInt32(lCredentials.Designation);
                                db.OD_OtherDuty.Add(duty);
                                db.SaveChanges();
                                string lApply = "HRMSOD";
                                string lAppValue = "0";
                                string lresultsSms = lhelper.SendSms1(duty.StartDate, duty.EndDate, duty.ControllingAuthority, duty.SanctioningAuthority, duty.EmpId, duty.VistorFrom, duty.VistorTo, duty.Purpose, lApply, lAppValue);
                                string lresult = lhelper.SendEmails(duty.StartDate, duty.EndDate, duty.ControllingAuthority, duty.SanctioningAuthority, lEmpId, duty.VistorFrom, duty.VistorTo, duty.Purpose, duty.Description, lApply, lAppValue);
                                TempData["AlertMessage"] = "OD Applied Successfully";
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
                else
                {
                    int lcontrolling = Convert.ToInt32(Session["lcontrols"].ToString());
                    int lsanctioning = Convert.ToInt32(Session["lSancation"].ToString());
                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    duty.UpdatedBy = lCredentials.EmpId;
                    duty.EmpId = lEmpId;
                    duty.UpdatedDate = GetCurrentTime(DateTime.Now);
                    duty.Status = "Pending";
                    duty.ControllingAuthority = lcontrolling;
                    duty.SanctioningAuthority = lsanctioning;
                    duty.BranchId = Convert.ToInt32(lCredentials.Branch);
                    duty.DepartmentId = Convert.ToInt32(lCredentials.Department);
                    duty.DesignationId = Convert.ToInt32(lCredentials.Designation);
                    db.OD_OtherDuty.Add(duty);
                    db.SaveChanges();
                    string lApply = "HRMSOD";
                    string lAppValue = "0";
                    string lresultsSms = lhelper.SendSms1(duty.StartDate, duty.EndDate, duty.ControllingAuthority, duty.SanctioningAuthority, duty.EmpId, duty.VistorFrom, duty.VistorTo, duty.Purpose, lApply, lAppValue);
                    string lresult = lhelper.SendEmails(duty.StartDate, duty.EndDate, duty.ControllingAuthority, duty.SanctioningAuthority, lEmpId, duty.VistorFrom, duty.VistorTo, duty.Purpose, duty.Description, lApply, lAppValue);
                    TempData["AlertMessage"] = "OD Applied Successfully";
                    return RedirectToAction("Index");
                }
                }
            else
            {
                TempData["AlertMessage"] = "The Selected Dates should be less than or equal to the Retirement Date" + "  " + Retirement.Value.ToShortDateString() + "  " + "Please select other dates.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult History()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }

        [HttpGet]
        public JsonResult ODHistoryViews(string StartDate)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            try
            {
                var ltype = db.OD_Master.ToList();
                var ldeputation = db.OD_OtherDuty.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                var lResult = (from otherduty in ldeputation
                               join emp in lemployees on otherduty.EmpId equals emp.Id
                               join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                               join branch in lBranches on otherduty.BranchId equals branch.Id
                               join desig in ldesignation on otherduty.DesignationId equals desig.Id
                               join dept in Departments on otherduty.DepartmentId equals dept.Id
                               join type in ltype on otherduty.Purpose equals type.Id
                               orderby otherduty.UpdatedDate descending
                               select new
                               {

                                   otherduty.Id,
                                   emp.EmpId,
                                   EmployeeName = emp.ShortName,
                                   designation = desig.Code,
                                   VistorFrom = visitbran.Name,
                                   otherduty.VistorTo,
                                   UpdatedDate = otherduty.UpdatedDate.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                   StartDate = GetsDates(otherduty.StartDate),
                                   EndDate = GetsDates(otherduty.EndDate),
                                   Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                   Purpose = type.ODType,
                                   otherduty.Status,
                                   otherduty.Description,
                                   Action = otherduty.Status == "Cancelled" ? "Cancelled" : "Cancel"
                               }).OrderByDescending(A => A.Id);
                return Json(lResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }

        [HttpPost]
        public JsonResult ODHistoryView(FormCollection form)
        {
            var YourRadioButton = Request.Form["Status"];
            Session["lsd"] = form["Stdate"];
            Session["led"] = form["Endate"];
            Session["lApplied"] = form["lApplied"];
            Session["lRequest"] = form["lRequest"];
            string lMessage = string.Empty;
            try
            {
                var ltype = db.OD_Master.ToList();
                var ldeputation = db.OD_OtherDuty.ToList();
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
                                    join visitbran in lbranch on deput.VistorFrom equals visitbran.Id
                                    join branch in lbranch on deput.BranchId equals branch.Id
                                    join desig in ldesignation on deput.DesignationId equals desig.Id
                                    join dept in Departments on deput.DepartmentId equals dept.Id
                                    join type in ltype on deput.Purpose equals type.Id
                                    where ((deput.UpdatedDate.Date >= lStartdate.Date && deput.UpdatedDate.Date <= lEnddate.Date)
                                        || (deput.UpdatedDate.Date >= lStartdate.Date && deput.UpdatedDate.Date <= lEnddate.Date))
                                    select new
                                    {
                                        deput.Id,
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        VistorFrom = visitbran.Name,
                                        deput.VistorTo,
                                        UpdatedDate = deput.UpdatedDate.ToShortDateString(),
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        StartDate = GetsDates(deput.StartDate),
                                        EndDate = GetsDates(deput.EndDate),
                                        Duration = GetDiffDays(deput.StartDate, deput.EndDate),
                                        Purpose = type.ODType,
                                        deput.Status,
                                        deput.Description,
                                        Action = deput.Status == "Cancelled" ? "Cancelled" : "Cancel"
                                    }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                    return Json(lResults, JsonRequestBehavior.AllowGet);
                }

                else if (Applieddate == "Request")
                {

                    var lResults = (from deput in ldeputation
                                    join emp in lemployees on deput.EmpId equals emp.Id
                                    join visitbran in lbranch on deput.VistorFrom equals visitbran.Id
                                    join branch in lbranch on deput.BranchId equals branch.Id
                                    join desig in ldesignation on deput.DesignationId equals desig.Id
                                    join dept in Departments on deput.DepartmentId equals dept.Id
                                    join type in ltype on deput.Purpose equals type.Id
                                    where deput.StartDate.Date >= lStartdate.Date && deput.EndDate.Date <= lEnddate.Date
                                    || (deput.EndDate.Date >= lStartdate.Date
                                   && deput.StartDate.Date <= lEnddate.Date)
                                    select new
                                    {

                                        deput.Id,
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        VistorFrom = visitbran.Name,
                                        deput.VistorTo,
                                        UpdatedDate = deput.UpdatedDate.ToShortDateString(),
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        StartDate = GetsDates(deput.StartDate),
                                        EndDate = GetsDates(deput.EndDate),
                                        Duration = GetDiffDays(deput.StartDate, deput.EndDate),
                                        Purpose = type.ODType,
                                        deput.Status,
                                        deput.Description,
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
        public JsonResult Todaysdeputation(string StartDate)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
            try
            {
                var ltype = db.OD_Master.ToList();
                var ldeputation = db.OD_OtherDuty.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                var lResult = ((from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id
                                join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                join branch in lBranches on otherduty.BranchId equals branch.Id
                                join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                join dept in Departments on otherduty.DepartmentId equals dept.Id
                                join lodmst in ltype on otherduty.Purpose equals lodmst.Id
                                where otherduty.Status != "Cancelled" && otherduty.Status != "Denied"
                                where ((lStartDate >= otherduty.StartDate.Date && lStartDate <= otherduty.EndDate.Date)
                                      || (lEndDate >= otherduty.EndDate.Date && lStartDate <= otherduty.EndDate.Date))
                                orderby otherduty.Id descending
                                select new
                                {

                                    otherduty.Id,
                                    emp.EmpId,
                                    EmployeeName = emp.ShortName,
                                    VistorFrom = visitbran.Name,
                                    otherduty.VistorTo,
                                    designation = desig.Code,
                                    UpdatedDate = otherduty.UpdatedDate.ToShortDateString(),
                                    Deptbranch = GetBranchDepartmentConcat(visitbran.Name, dept.Name),
                                    StartDate = GetsDates(otherduty.StartDate),
                                    EndDate = GetsDates(otherduty.EndDate),
                                    Purpose = lodmst.ODType,
                                    Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                    otherduty.Status,
                                    otherduty.Description,
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
        [HttpGet]
        public JsonResult Selfdeputation(string StartDate)
        {

            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
            try
            {
                var ldeputation = db.OD_OtherDuty.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var lodmaster = db.OD_Master.ToList();
                var ldesignation = db.Designations.ToList();

                var lResults = (from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id
                                join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                join branch in lBranches on otherduty.BranchId equals branch.Id
                                join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                join dept in Departments on otherduty.DepartmentId equals dept.Id
                                join lodmst in lodmaster on otherduty.Purpose equals lodmst.Id
                                where otherduty.EmpId == lEmpId
                                select new
                                {
                                    otherduty.Id,
                                    emp.EmpId,
                                    EmployeeName = emp.ShortName,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    VistorFrom = visitbran.Name,
                                    otherduty.VistorTo,
                                    UpdatedDate = otherduty.UpdatedDate.ToShortDateString(),
                                    UpdatedDate1 = otherduty.UpdatedDate,
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = GetsDates(otherduty.StartDate),
                                    EndDate = GetsDates(otherduty.EndDate),
                                    Purpose = lodmst.ODType,
                                    Duration = GetDiffDays(otherduty.StartDate, otherduty.EndDate),
                                    otherduty.Status,
                                    otherduty.Description,
                                    otherduty.CancelReason
                                }).OrderByDescending(a => a.UpdatedDate1);
                return Json(lResults, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        [HttpGet]
        public ActionResult Approvals()
        {
            string lMessage = string.Empty;
            try
            {
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                if (lCredentials.LoginMode == "SuperAdmin")
                {
                    int lempid = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    string LEmpid = Convert.ToString(lempid);
                    var lOtherdutybranch = db.OD_OtherDuty.Where(a => a.EmpId == lempid).Select(a => a.VistorFrom).FirstOrDefault();
                    var items5 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.Name != "OtherBranch").Where(a => a.Name != "TBD").Select(x => new Branches
                    {
                        Id = x.Id,
                        Name = x.Name
                    });
                    ViewBag.Branch = new SelectList(items5, "Id", "Name", lOtherdutybranch);
                    var items = Facade.EntitiesFacade.GetAllODMaster().Where(a => a.Status == true).Select(x => new OD_Master
                    {
                        Id = x.Id,
                        ODType = x.ODType
                    });
                    ViewBag.Purpose = new SelectList(items, "Id", "ODType");
                    OD_OtherDuty lmodel = new OD_OtherDuty();
                    string lempid1 = Convert.ToString(lmodel.EmpId);
                    lempid1 = LEmpid;
                    lmodel.Loginmode = lCredentials.LoginMode;
                    ViewBag.Message = lCredentials.LoginMode;
                    TempData["Loginmode"] = lCredentials.LoginMode;
                    return View(lmodel);
                }
                else
                {
                    int lempid = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    var lOtherdutybranch = db.OD_OtherDuty.Where(a => a.EmpId == lempid).Select(a => a.VistorFrom).FirstOrDefault();
                    var items5 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.Name != "OtherBranch").Where(a => a.Name != "TBD").Select(x => new Branches
                    {
                        Id = x.Id,
                        Name = x.Name
                    });
                    ViewBag.Branch = new SelectList(items5, "Id", "Name", lOtherdutybranch);
                    var items = Facade.EntitiesFacade.GetAllODMaster().Where(a => a.Status == true).Select(x => new OD_Master
                    {
                        Id = x.Id,
                        ODType = x.ODType
                    });
                    ViewBag.Purpose = new SelectList(items, "Id", "ODType");
                    OD_OtherDuty lmodel = new OD_OtherDuty();
                    lmodel.Loginmode = lCredentials.LoginMode;
                    ViewBag.Message = lCredentials.LoginMode;
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


        public ActionResult ODApprovalView()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ODApprovalViews(string status)
        {
            string lMessage = string.Empty;
            try
            {
                var lEmployees = db.Employes.ToList();
                var lOtherduty = db.OD_OtherDuty.ToList();
                var lBranches = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var lodmaster = db.OD_Master.ToList();
                var ldesignation = db.Designations.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lControllingAuthority = db.OD_OtherDuty.Where(a => a.ControllingAuthority == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                int lSancationingAuthority = db.OD_OtherDuty.Where(a => a.SanctioningAuthority == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                if (lEmpId == lControllingAuthority)
                {
                    var Duration = string.Empty;
                    var lResult = (from duty in lOtherduty
                                   join employee in lEmployees on duty.EmpId equals employee.Id
                                   join branch in lBranches on duty.VistorFrom equals branch.Id
                                   join branches in lBranches on duty.BranchId equals branches.Id
                                   join dept in ldept on duty.DepartmentId equals dept.Id
                                   join desig in ldesignation on duty.DesignationId equals desig.Id
                                   join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                   where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                   duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                   select new
                                   {
                                       duty.Id,
                                       employee.EmpId,
                                       EmployeeName = employee.ShortName,
                                       employee.ShortName,
                                       designation = desig.Code,
                                       AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       VistorFrom = branch.Name,
                                       duty.VistorTo,
                                       StartDate = GetsDates(duty.StartDate),
                                       EndDate = GetsDates(duty.EndDate),
                                       Purpose = lodmst.ODType,
                                       Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                       duty.Status,
                                       duty.Description
                                   }).OrderByDescending(A => A.Status=="Pending");
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                }
                if (lEmpId == lSancationingAuthority)
                {
                    var lResult = (from duty in lOtherduty
                                   join employee in lEmployees on duty.EmpId equals employee.Id
                                   join branch in lBranches on duty.VistorFrom equals branch.Id
                                   join branches in lBranches on duty.BranchId equals branches.Id
                                   join dept in ldept on duty.DepartmentId equals dept.Id
                                   join desig in ldesignation on duty.DesignationId equals desig.Id
                                   join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                   where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                   select new
                                   {
                                       duty.Id,
                                       employee.EmpId,
                                       EmployeeName = employee.ShortName,
                                       employee.ShortName,
                                       designation = desig.Code,
                                       AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       VistorFrom = branch.Name,
                                       duty.VistorTo,
                                       StartDate = GetsDates(duty.StartDate),
                                       EndDate = GetsDates(duty.EndDate),
                                       Purpose = lodmst.ODType,
                                       Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                       duty.Status,
                                       duty.Description
                                   }).OrderByDescending(A => A.AppliedDate);
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
        public JsonResult ODApprovalViews(string EmployeeCodey, string LeaveIds)
        {
            Timesheet_Request_Form ltform = new Timesheet_Request_Form();
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var ldbresult = db.OD_OtherDuty.ToList();
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
                        string lstauts = db.OD_OtherDuty.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                        if (lstauts == "Pending")
                        {
                            int leaverowid = Convert.ToInt32(lIdss);
                            OD_OtherDuty lcontrolling = Facade.EntitiesFacade.GetOtherdutyTabledata.GetById(leaverowid);
                            string lcontrolstatus = "Forwarded";
                            string lcontrolvalue = "0";
                            OD_OtherDuty lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatep.UpdatedBy = lCredentials.EmpId;
                            lupdatep.Status = "Forwarded";
                            db.Entry(lupdatep).State = EntityState.Modified;
                            db.SaveChanges();
                            string lresultsSms = lhelper.SendSms1(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.VistorFrom, lcontrolling.VistorTo, lcontrolling.Purpose, lcontrolstatus, lcontrolvalue);
                            string lresult = lhelper.SendEmails(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.VistorFrom, lcontrolling.VistorTo, lcontrolling.Purpose, lcontrolling.Description, lcontrolstatus, lcontrolvalue);
                            TempData["Forward"] = "OD Forwarded Successfully";
                        }
                        if (lstauts == "Forwarded")
                        {
                            int leaverowid = Convert.ToInt32(lIdss);
                            OD_OtherDuty lSancationing = Facade.EntitiesFacade.GetOtherdutyTabledata.GetById(leaverowid);
                            string llSancationingstatus = "Approved";
                            string llSancationingvalue = "1";
                            OD_OtherDuty lupdatef = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatef.Status = "Approved";
                            lupdatef.UpdatedBy = lCredentials.EmpId;
                            db.Entry(lupdatef).State = EntityState.Modified;
                            db.SaveChanges();
                            string lresultsSms = lhelper.SendSms1(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.VistorFrom, lSancationing.VistorTo, lSancationing.Purpose, llSancationingstatus, llSancationingvalue);
                            string lresult = lhelper.SendEmails(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.VistorFrom, lSancationing.VistorTo, lSancationing.Purpose, lSancationing.Description, llSancationingstatus, llSancationingvalue);
                            TempData["Approve"] = "OD Approved Successfully";
                            if (lupdatef.EndDate <= DateTime.Now.Date) { 
                                string lcode = db.OD_Master.Where(a => a.Id == lupdatef.Purpose).Select(a => a.ODType).FirstOrDefault();
                            int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                            int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                            ltform.UserId = lId;
                            ltform.BranchId = (int)lupdatef.BranchId;
                            ltform.DepartmentId = (int)lupdatef.DepartmentId;
                            ltform.DesignationId = (int)lupdatef.DesignationId;
                            ltform.Shift_Id = (int)shiftids;
                            ltform.Reason_Type = lcode;
                            ltform.Reason_Desc = "OD";
                            ltform.ReqFromDate = lupdatef.StartDate;
                            ltform.ReqToDate = lupdatef.EndDate;
                            ltform.CA = lupdatef.ControllingAuthority;
                            ltform.SA = lupdatef.SanctioningAuthority;
                            ltform.Status = lupdatef.Status;
                            ltform.UpdatedBy = lupdatef.UpdatedBy;
                            ltform.UpdatedDate = lupdatef.UpdatedDate;
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
        public JsonResult Cancel(string EmployeeCodey, string LeaveIds, string Reason)
        {
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var lEmpBalance = db.EmpLeaveBalance.ToList();
                var ldbresult = db.OD_OtherDuty.ToList();
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
                        string lstauts = db.OD_OtherDuty.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                        if (lstauts == "Pending")
                        {
                            OD_OtherDuty lcontrolling = Facade.EntitiesFacade.GetOtherdutyTabledata.GetById(LeaveId);
                            string lcontrolstatus = "Cancelled";
                            string lcontrolvalue = "0";
                            OD_OtherDuty lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatep.Status = "Cancelled";
                            lupdatep.UpdatedBy = lCredentials.EmpId;
                            lupdatep.CancelReason = Reason;
                            lupdatep.Stage = lstauts;
                            db.Entry(lupdatep).State = EntityState.Modified;
                            db.SaveChanges();
                            string lresultsSms = lhelper.SendSms1(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.VistorFrom, lcontrolling.VistorTo, lcontrolling.Purpose, lcontrolstatus, lcontrolvalue);
                            string lresult = lhelper.SendEmails(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.VistorFrom, lcontrolling.VistorTo, lcontrolling.Purpose, lcontrolling.Description, lcontrolstatus, lcontrolvalue);
                            TempData["cancel"] = "OD Cancelled Successfully";
                        }
                        else if (lstauts == "Forwarded")
                        {
                            OD_OtherDuty lSancationing = Facade.EntitiesFacade.GetOtherdutyTabledata.GetById(LeaveId);
                            string llSancationingstatus = "Cancelled";
                            string llSancationingvalue = "1";
                            OD_OtherDuty lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatep.Status = "Cancelled";
                            lupdatep.UpdatedBy = lCredentials.EmpId;
                            lupdatep.CancelReason = Reason;
                            lupdatep.Stage = lstauts;
                            db.Entry(lupdatep).State = EntityState.Modified;
                            db.SaveChanges();
                            string lresultsSms = lhelper.SendSms1(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.VistorFrom, lSancationing.VistorTo, lSancationing.Purpose, llSancationingstatus, llSancationingvalue);
                            string lresult = lhelper.SendEmails(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.VistorFrom, lSancationing.VistorTo, lSancationing.Purpose, lSancationing.Description, llSancationingstatus, llSancationingvalue);
                            TempData["cancel"] = "OD Cancelled Successfully";
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
        public JsonResult Deny(string EmployeeCodey, string LeaveIds, string Reason)
        {
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var lEmpBalance = db.EmpLeaveBalance.ToList();
                var ldbresult = db.OD_OtherDuty.ToList();
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
                        string lstauts = db.OD_OtherDuty.Where(a => a.EmpId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                        if (lstauts == "Pending")
                        {
                            OD_OtherDuty lcontrolling = Facade.EntitiesFacade.GetOtherdutyTabledata.GetById(LeaveId);
                            string lcontrolstatus = "Denied";
                            string lcontrolvalue = "0";
                            OD_OtherDuty lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatep.Status = "Denied";
                            lupdatep.UpdatedBy = lCredentials.EmpId;
                            lupdatep.CancelReason = Reason;
                            lupdatep.Stage = lstauts;
                            db.Entry(lupdatep).State = EntityState.Modified;
                            db.SaveChanges();
                            string lresultsSms = lhelper.SendSms1(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.VistorFrom, lcontrolling.VistorTo, lcontrolling.Purpose, lcontrolstatus, lcontrolvalue);
                            string lresult = lhelper.SendEmails(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.VistorFrom, lcontrolling.VistorTo, lcontrolling.Purpose, lcontrolling.Description, lcontrolstatus, lcontrolvalue);
                            TempData["Denied"] = "OD Denied Successfully";
                        }
                        else if (lstauts == "Forwarded")
                        {
                            OD_OtherDuty lSancationing = Facade.EntitiesFacade.GetOtherdutyTabledata.GetById(LeaveId);
                            string llSancationingstatus = "Denied";
                            string llSancationingvalue = "1";
                            OD_OtherDuty lupdatep = (from l in ldbresult where l.EmpId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatep.Status = "Denied";
                            lupdatep.UpdatedBy = lCredentials.EmpId;
                            lupdatep.CancelReason = Reason;
                            lupdatep.Stage = lstauts;
                            db.Entry(lupdatep).State = EntityState.Modified;
                            db.SaveChanges();
                            string lresultsSms = lhelper.SendSms1(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.VistorFrom, lSancationing.VistorTo, lSancationing.Purpose, llSancationingstatus, llSancationingvalue);
                            string lresult = lhelper.SendEmails(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.VistorFrom, lSancationing.VistorTo, lSancationing.Purpose, lSancationing.Description, llSancationingstatus, llSancationingvalue);
                            TempData["Denied"] = "OD Denied Successfully";
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



        public ActionResult ODCancelRequest(string LeaveId, string Reason)
        {      
             
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
               
                if (LeaveId != "")
                {
                    int id = 0;
                    id = Convert.ToInt32(LeaveId);
                    
                   
                    OD_OtherDuty lOdCancel = Facade.EntitiesFacade.GetOtherdutyTabledata.GetById(id);
                    var lstatus = lOdCancel.Status;
                    int lids = Convert.ToInt32(lOdCancel.EmpId);
                    lOdCancel.Status = "Cancelled";
                    lOdCancel.CancelReason = Reason;
                    db.Entry(lOdCancel).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["status"] = "OD Cancelled Successfully";
                    var ldbresult = db.OD_OtherDuty.ToList();
                    OD_OtherDuty lod = (from l in ldbresult where l.Id == id select l).FirstOrDefault();
                    if (lod.EndDate <= DateTime.Now.Date && lstatus == "Approved")
                    {
                        Timesheet_Request_Form odform = new Timesheet_Request_Form();
                        string lcode = db.LeaveTypes.Where(a => a.Id == lod.Purpose).Select(a => a.Code).FirstOrDefault();
                        int branchid = db.Employes.Where(a => a.Id == lids).Select(a => a.Branch).FirstOrDefault();
                        int? shiftids = db.Employes.Where(a => a.Id == lids).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                        odform.UserId = (int)lod.EmpId;
                        odform.BranchId = (int)lod.BranchId;
                        odform.DepartmentId = (int)lod.DepartmentId;
                        odform.DesignationId = (int)lod.DesignationId;
                        odform.Shift_Id = (int)shiftids;
                        odform.Reason_Type = "AB";
                        odform.Reason_Desc = "OD";
                        odform.ReqFromDate = lod.StartDate;
                        odform.ReqToDate = lod.EndDate;
                        odform.CA = lod.ControllingAuthority;
                        odform.SA = lod.SanctioningAuthority;
                        odform.Status = "Cancelled";
                        odform.UpdatedBy = lod.UpdatedBy;
                        odform.UpdatedDate = lod.UpdatedDate;
                        odform.Processed = 3;
                        //db.Timesheet_Request_Form.Add(odform);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                e.ToString();
            }
            return RedirectToAction("OdDuty", "AllReports");

        }

        [HttpGet]
        public JsonResult ODTooltip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            var lEmployees = db.Employes.ToList();
            var lOtherduty = db.OD_OtherDuty.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var lodmaster = db.OD_Master.ToList();
            var ldesignation = db.Designations.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int OderrowId = Convert.ToInt32(EmployeeCodey);
            try
            {

                var lResult = (from duty in lOtherduty
                               join employee in lEmployees on duty.EmpId equals employee.Id
                               join branch in lBranches on duty.VistorFrom equals branch.Id
                               join branches in lBranches on duty.BranchId equals branches.Id
                               join dept in ldept on duty.DepartmentId equals dept.Id
                               join desig in ldesignation on duty.DesignationId equals desig.Id
                               join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                               where duty.Id == OderrowId

                               select new
                               {
                                   duty.Id,
                                   employee.EmpId,
                                   EmployeeName = employee.ShortName,
                                   employee.ShortName,
                                   designation = desig.Code,
                                   AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                   VisitFrom = branch.Name,
                                   VisitTo = duty.VistorTo,
                                   StartingDate = GetsDates(duty.StartDate),
                                   EndDate = GetsDates(duty.EndDate),
                                   Purpose = lodmst.ODType,
                                   Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                   duty.Status,
                                   duty.Description
                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = OderrowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                string lVisitingfrom = lresponseArray[0].VisitFrom;
                string Vistingto = lresponseArray[0].VisitTo;
                string ODStartDate = lresponseArray[0].StartingDate;
                string ODEndDate = lresponseArray[0].EndDate;
                string ODDuration = lresponseArray[0].Duration;
                string ODPurpose = lresponseArray[0].Purpose;
                string ODStatus = lresponseArray[0].Status;
              
                string ODDescription = lresponseArray[0].Description;
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
                   ldescription= ODDescription,
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
        public JsonResult ODNexttip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            var lEmployees = db.Employes.ToList();
            var lOtherduty = db.OD_OtherDuty.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var lodmaster = db.OD_Master.ToList();
            var ldesignation = db.Designations.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int OderrowId = Convert.ToInt32(EmployeeCodey);
            try
            {

                var lResult = (from duty in lOtherduty
                               join employee in lEmployees on duty.EmpId equals employee.Id
                               join branch in lBranches on duty.VistorFrom equals branch.Id
                               join branches in lBranches on duty.BranchId equals branches.Id
                               join dept in ldept on duty.DepartmentId equals dept.Id
                               join desig in ldesignation on duty.DesignationId equals desig.Id
                               join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                               where duty.Id == OderrowId

                               select new
                               {
                                   duty.Id,
                                   employee.EmpId,
                                   EmployeeName = employee.ShortName,
                                   employee.ShortName,
                                   designation = desig.Code,
                                   AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                   VisitFrom = branch.Name,
                                   VisitTo = duty.VistorTo,
                                   StartingDate = GetsDates(duty.StartDate),
                                   EndDate = GetsDates(duty.EndDate),
                                   Purpose = lodmst.ODType,
                                   Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                   duty.Status,
                                   duty.Description
                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = OderrowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                string lVisitingfrom = lresponseArray[0].VisitFrom;
                string Vistingto = lresponseArray[0].VisitTo;
                string ODStartDate = lresponseArray[0].StartingDate;
                string ODEndDate = lresponseArray[0].EndDate;
                string ODDuration = lresponseArray[0].Duration;
                string ODPurpose = lresponseArray[0].Purpose;
                string ODStatus = lresponseArray[0].Status;
                string ODDescription = lresponseArray[0].Description;
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
                    ldescription = ODDescription,
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
        public JsonResult ODPrevioustip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            var lEmployees = db.Employes.ToList();
            var lOtherduty = db.OD_OtherDuty.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var lodmaster = db.OD_Master.ToList();
            var ldesignation = db.Designations.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int OderrowId = Convert.ToInt32(EmployeeCodey);
            try
            {

                var lResult = (from duty in lOtherduty
                               join employee in lEmployees on duty.EmpId equals employee.Id
                               join branch in lBranches on duty.VistorFrom equals branch.Id
                               join branches in lBranches on duty.BranchId equals branches.Id
                               join dept in ldept on duty.DepartmentId equals dept.Id
                               join desig in ldesignation on duty.DesignationId equals desig.Id
                               join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                               where duty.Id == OderrowId

                               select new
                               {
                                   duty.Id,
                                   employee.EmpId,
                                   EmployeeName = employee.ShortName,
                                   employee.ShortName,
                                   designation = desig.Code,
                                   AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                   VisitFrom = branch.Name,
                                   VisitTo = duty.VistorTo,
                                   StartingDate = GetsDates(duty.StartDate),
                                   EndDate = GetsDates(duty.EndDate),
                                   Purpose = lodmst.ODType,
                                   Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                   duty.Status,
                                   duty.Description
                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = OderrowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                string lVisitingfrom = lresponseArray[0].VisitFrom;
                string Vistingto = lresponseArray[0].VisitTo;
                string ODStartDate = lresponseArray[0].StartingDate;
                string ODEndDate = lresponseArray[0].EndDate;
                string ODDuration = lresponseArray[0].Duration;
                string ODPurpose = lresponseArray[0].Purpose;
                string ODStatus = lresponseArray[0].Status;
                string ODDescription = lresponseArray[0].Description;
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
                    ldescription = ODDescription,
                    lstatus = ODStatus
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
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
                    ldurations = lonlyhours.ToString().PadLeft(2, '0') + ":" + lminutes.ToString().PadLeft(2, '0') + ":" + lzeros.PadRight(2, '0');
                }
                else
                {
                    ldurations = TotalDays + " days - " + lonlyhours + ":" + lminutes.ToString().PadRight(2, '0') + ":" + lzeros.PadRight(2, '0');
                }
            }
            return ldurations;
        }

        public string GetsDates(DateTime ldate)
        {
            string ldates = "";
            DateTime d1 = ldate;
            ldates = d1.ToShortDateString().ToString() + " - " + d1.ToShortTimeString().ToString();
            return ldates;
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
        //Creating a PDF File For OD History
        public FileResult CreatePdf()
        {
            String lstartdate = Convert.ToString(Session["sd"]);
            String lenddate = Convert.ToString(Session["ed"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("MyDeputationHistory" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 4 columns  
            PdfPTable tableLayout1 = new PdfPTable(4);
            PdfPTable tableLayout = new PdfPTable(5);
            doc.SetMargins(20f, 20f, 20f, 20f);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF1(tableLayout1));
            //Add Content to PDF   
            doc.Add(Add_Content_To_PDF(tableLayout, lstartdate, lenddate));
            // Closing the document  
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;       
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF1(PdfPTable tableLayout1)
        {

            float[] headers1 = { 33, 33, 33, 33 }; //Header Widths  
            tableLayout1.SetWidths(headers1); //Set the pdf headers  
            tableLayout1.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout1.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            tableLayout1.AddCell(new PdfPCell(new Phrase("OD History", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            var lod = db.OD_OtherDuty.ToList();
            var ldesignation = db.Designations.ToList();
            var lemployees = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartment = db.Departments.ToList();
            var lResults = (from OD in lod
                            join emp in lemployees on OD.EmpId equals emp.Id
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

            //Adding headers  
            AddCellToHeader(tableLayout1, "EmpCode");
            AddCellToHeader(tableLayout1, "EmpName");
            AddCellToHeader(tableLayout1, "Designation");
            AddCellToHeader(tableLayout1, "Department/Branch");

            //Adding body  
            foreach (var lleave in lResults)
            {
                AddCellToBody(tableLayout1, lleave.EmpCode.ToString());
                AddCellToBody(tableLayout1, lleave.EmployeeName.ToString());
                AddCellToBody(tableLayout1, lleave.designation.ToString());
                AddCellToBody(tableLayout1, lleave.Deptbranch.ToString());
            }
            return tableLayout1;
        }

        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, string StartDate, string EndDate)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            float[] headers = { 50, 22, 25, 60, 40 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            List<Leaves> lleaves = db.Leaves.ToList<Leaves>();
            var lod = db.OD_OtherDuty.ToList();
            var ldesignation = db.Designations.ToList();
            var ldepartment = db.Departments.ToList();
            var lemployees = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var odmaster = db.OD_Master.ToList();
            var lLeaveTypes = db.LeaveTypes.ToList();
            if (StartDate == "" || EndDate == "")
            {
                var lResults = (from OD in lod
                                join emp in lemployees on OD.EmpId equals emp.Id
                                join desig in ldesignation on OD.DesignationId equals desig.Id
                                join branch in lbranch on OD.VistorFrom equals branch.Id
                                join dept in ldepartment on OD.DepartmentId equals dept.Id
                                join lpurpose in odmaster on OD.Purpose equals lpurpose.Id
                                where OD.EmpId == lEmpId
                                select new
                                {
                                    StartDate = HistoryDates(OD.StartDate, OD.EndDate) + "  " + " ( " + "From" +" "+ GetBranchDepartmentConcat1(branch.Name, dept.Name) + "  " + "To" + " " + OD.VistorTo + ")",
                                    purpose = lpurpose.ODType,
                                    State = OD.Status,
                                    Desp = OD.Description,
                                    CancelRes = OD.CancelReason,
                                });
                //Add header       
                AddCellToHeader(tableLayout, "Visit Details");
                AddCellToHeader(tableLayout, "Purpose");
                AddCellToHeader(tableLayout, "Status");
                AddCellToHeader(tableLayout, "Description");
                AddCellToHeader(tableLayout, "Cancel Reason");

                //Add body  
                foreach (var od in lResults)
                {
                    AddCellToBody(tableLayout, od.StartDate.ToString());
                    AddCellToBody(tableLayout, od.purpose);
                    AddCellToBody(tableLayout, od.State);
                    AddCellToBody(tableLayout, od.Desp);
                    AddCellToBody(tableLayout, od.CancelRes);
                }

                return tableLayout;
            }
            else
            {
                DateTime ToDate = Convert.ToDateTime(EndDate);
                string strDate = StartDate;
                string[] sa = strDate.Split('/');
                string strNew = sa[2] + "/" + sa[1] + "/" + sa[0];
                string strDate1 = EndDate;
                string[] sa1 = strDate1.Split('/');
                string strNew1 = sa1[2] + "/" + sa1[1] + "/" + sa1[0];
                DateTime FromDate = DateTime.ParseExact(strNew, "yyyy/MM/dd", null);
                DateTime Todate = DateTime.ParseExact(strNew1, "yyyy/MM/dd", null);
                DateTime lStartdate = FromDate;
                DateTime lEnddate = ToDate;
                var lResults = (from OD in lod
                                join emp in lemployees on OD.EmpId equals emp.Id
                                join desig in ldesignation on OD.DesignationId equals desig.Id
                                join branch in lbranch on OD.BranchId equals branch.Id
                                join dept in ldepartment on OD.DepartmentId equals dept.Id
                                join lpurpose in odmaster on OD.Purpose equals lpurpose.Id
                                where ((OD.StartDate.Date >= lStartdate && OD.EndDate.Date <= lEnddate)
              || (OD.EndDate.Date >= lStartdate && OD.StartDate.Date <= lEnddate)) && OD.EmpId == lEmpId
                                select new
                                {
                                    StartDate = HistoryDates(OD.StartDate, OD.EndDate) + "  " + " ( " + "From" + GetBranchDepartmentConcat(branch.Name, dept.Name) + "  " + "To" + " " + OD.VistorTo + ")",
                                    purpose = lpurpose.ODType,
                                    State = OD.Status,
                                    Desp = OD.Description,
                                    CancelRes=OD.CancelReason
                                });
                //Add header       
                AddCellToHeader(tableLayout, "Visit Details");
                AddCellToHeader(tableLayout, "Purpose");
                AddCellToHeader(tableLayout, "Status");
                AddCellToHeader(tableLayout, "Description");
                AddCellToHeader(tableLayout, "Cancel Reason");

                //Add body  
                foreach (var od in lResults)
                {
                    AddCellToBody(tableLayout, od.StartDate.ToString());
                    AddCellToBody(tableLayout, od.purpose);
                    AddCellToBody(tableLayout, od.State);
                    AddCellToBody(tableLayout, od.Desp);
                    AddCellToBody(tableLayout, od.CancelRes);
                }

                return tableLayout;
            }

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
            var startTime = Startdate.ToLongTimeString();
            var endday = Enddate.ToString("MMM dd");
            var endYear = Enddate.Year.ToString();
            var endTime = Enddate.ToLongTimeString();
            if (startYear == endYear)
            {
                combinedates = startday + " " + startTime + " " + "To" + " " + endday + " " + endTime + " " + "," + " " + startYear;
                return combinedates;
            }
            else
            {
                combinedates = startday + " " + startTime + " " + " " + "," + startYear + " " + "To" + " " + endday + " " + endTime + " " + "," + " " + endYear;
                return combinedates;
            }


        }

        [HttpPost]
        public JsonResult ODApprovalSearch(FormCollection formValues)
        {

            string lMessage = string.Empty;
            try
            {

                string Purpose = formValues["Purpose"];
                string AppliedDate = formValues["ADate"];
                string Branch = formValues["VistorFrom"];
                string Empid = formValues["EmpId"];
                var lEmployees = db.Employes.ToList();
                var lOtherduty = db.OD_OtherDuty.ToList();
                var lBranches = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var lodmaster = db.OD_Master.ToList();
                var ldesignation = db.Designations.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lControllingAuthority = db.OD_OtherDuty.Where(a => a.ControllingAuthority == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                int lSancationingAuthority = db.OD_OtherDuty.Where(a => a.SanctioningAuthority == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                if (lEmpId == lControllingAuthority)
                {
                    if (Purpose != "" && Empid == "" && AppliedDate == "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose == "" && Empid != "" && AppliedDate == "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where employee.EmpId == Empid

                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }

                    else if (Purpose == "" && Empid == "" && AppliedDate != "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)

                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }

                    else if (Purpose == "" && Empid == "" && AppliedDate == "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose != "" && Empid == "" && AppliedDate == "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.Purpose.ToString() == Purpose
                                       where duty.VistorFrom == Convert.ToInt32(Branch)

                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }

                    else if (Purpose != "" && Empid != "" && AppliedDate == "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where employee.EmpId == Empid
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }

                    else if (Purpose != "" && Empid == "" && AppliedDate != "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }

                    else if (Purpose == "" && Empid != "" && AppliedDate != "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where employee.EmpId == Empid
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose == "" && Empid != "" && AppliedDate == "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where employee.EmpId == Empid
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose == "" && Empid == "" && AppliedDate != "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose != "" && Empid != "" && AppliedDate != "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where employee.EmpId == Empid
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose != "" && Empid != "" && AppliedDate == "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       where employee.EmpId == Empid
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose == "" && Empid != "" && AppliedDate != "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       where employee.EmpId == Empid
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose != "" && Empid == "" && AppliedDate != "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }

                    else if (Purpose != "" && Empid != "" && AppliedDate != "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where duty.Purpose.ToString() == Purpose
                                       where employee.EmpId == Empid
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.ControllingAuthority == lControllingAuthority && duty.Status == "Pending" ||
                                       duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                    }
                }
                else if (lEmpId == lSancationingAuthority)
                {
                    if (Purpose != "" && Empid == "" && AppliedDate == "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose == "" && Empid != "" && AppliedDate == "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where employee.EmpId == Empid

                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }

                    else if (Purpose == "" && Empid == "" && AppliedDate != "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)

                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose == "" && Empid == "" && AppliedDate == "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.VistorFrom == Convert.ToInt32(Branch)

                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose != "" && Empid == "" && AppliedDate == "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.Purpose.ToString() == Purpose
                                       where duty.VistorFrom == Convert.ToInt32(Branch)

                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }

                    else if (Purpose != "" && Empid != "" && AppliedDate == "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where employee.EmpId == Empid
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }

                    else if (Purpose != "" && Empid == "" && AppliedDate != "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }

                    else if (Purpose == "" && Empid != "" && AppliedDate != "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where employee.EmpId == Empid
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose == "" && Empid != "" && AppliedDate == "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where employee.EmpId == Empid
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose == "" && Empid == "" && AppliedDate != "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose != "" && Empid != "" && AppliedDate != "" && Branch == "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where employee.EmpId == Empid
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description
                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose != "" && Empid != "" && AppliedDate == "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       where employee.EmpId == Empid
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose == "" && Empid != "" && AppliedDate != "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       where employee.EmpId == Empid
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose != "" && Empid == "" && AppliedDate != "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where duty.Purpose.ToString() == Purpose
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else if (Purpose != "" && Empid != "" && AppliedDate != "" && Branch != "")
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       where duty.VistorFrom == Convert.ToInt32(Branch)
                                       where duty.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                       where duty.Purpose.ToString() == Purpose
                                       where employee.EmpId == Empid
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var lResult = (from duty in lOtherduty
                                       join employee in lEmployees on duty.EmpId equals employee.Id
                                       join branch in lBranches on duty.VistorFrom equals branch.Id
                                       join branches in lBranches on duty.BranchId equals branches.Id
                                       join dept in ldept on duty.DepartmentId equals dept.Id
                                       join desig in ldesignation on duty.DesignationId equals desig.Id
                                       join lodmst in lodmaster on duty.Purpose equals lodmst.Id
                                       where duty.SanctioningAuthority == lSancationingAuthority && duty.Status == "Forwarded"
                                       select new
                                       {
                                           duty.Id,
                                           employee.EmpId,
                                           EmployeeName = employee.ShortName,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = duty.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                           VistorFrom = branch.Name,
                                           duty.VistorTo,
                                           StartDate = GetsDates(duty.StartDate),
                                           EndDate = GetsDates(duty.EndDate),
                                           Purpose = lodmst.ODType,
                                           Duration = GetDiffDays(duty.StartDate, duty.EndDate),
                                           duty.Status,
                                           duty.Description

                                       }).OrderByDescending(A => A.AppliedDate);
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
        // Create PDF File For All OD List
        public FileResult CreatePdfAllOD()
        {
            string lsd = Convert.ToString(Session["lsd"]);
            string led = Convert.ToString(Session["led"]);
            string lApplied = Convert.ToString(Session["lApplied"]);
            string lRequest = Convert.ToString(Session["lRequest"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("ODList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 11 columns  
            PdfPTable tableLayout1 = new PdfPTable(11);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF21(tableLayout1, lsd, led, lApplied, lRequest));
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
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page:" + i.ToString() + "/" + pages.ToString(), blackFont), 470f, 11f, 0);
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
            Session.Remove("lsd");
            Session.Remove("led");
            Session.Remove("lApplied");
            Session.Remove("lRequest");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF21(PdfPTable tableLayout1, string sd, string ed, string la, string lr)
        {
            float[] headers1 = { 40, 75, 50, 50, 50, 50, 70, 70, 50, 50, 70 }; //Header Widths  
            tableLayout1.SetWidths(headers1); //Set the pdf headers  
            tableLayout1.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout1.HeaderRows = 1;
            DateTime printdate = GetCurrentTime(DateTime.Now);
            tableLayout1.AddCell(new PdfPCell(new Phrase(printdate.Date.ToShortDateString() + " " + printdate.ToShortTimeString(), new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            tableLayout1.FooterRows = 1;
            tableLayout1.AddCell(new PdfPCell(new Phrase("ODList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lemployees = db.Employes.ToList();
            var ldesignation = db.Designations.ToList();
            var lBranches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var ldeputation = db.OD_OtherDuty.ToList();
            var ltype = db.OD_Master.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (la == "")
            {
                var data = (from otherduty in ldeputation
                            join emp in lemployees on otherduty.EmpId equals emp.Id
                            join desig in ldesignation on otherduty.DesignationId equals desig.Id
                            join dept in Departments on otherduty.DepartmentId equals dept.Id
                            join branch in lBranches on otherduty.BranchId equals branch.Id
                            join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                            join od in ltype on otherduty.Purpose equals od.Id
                            select new
                            {
                                emp.EmpId,
                                Name = emp.ShortName,
                                desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                VistorFrom = visitbran.Name,
                                otherduty.VistorTo,
                                otherduty.StartDate,
                                otherduty.EndDate,
                                otherduty.UpdatedDate,
                                od.ODType,
                                otherduty.Status,
                                otherduty.Description,
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                //AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "FromDate");
                AddCellToHeader(tableLayout1, "ToDate");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "VisitFrom");
                AddCellToHeader(tableLayout1, "VisitTo");
                AddCellToHeader(tableLayout1, "Purpose");
                AddCellToHeader(tableLayout1, "Status");
                AddCellToHeader(tableLayout1, "Description");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.Name);
                    //AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                    AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.VistorFrom);
                    AddCellToBody(tableLayout1, lemp.VistorTo);
                    AddCellToBody(tableLayout1, lemp.ODType);
                    AddCellToBody(tableLayout1, lemp.Status);
                    AddCellToBody(tableLayout1, lemp.Description);
                }
                return tableLayout1;
            }
            else if (la == "Applied")
            {
                if (sd == "" && ed == "")
                {
                    var data = (from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id
                                join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                join dept in Departments on otherduty.DepartmentId equals dept.Id
                                join branch in lBranches on otherduty.BranchId equals branch.Id
                                join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                join od in ltype on otherduty.Purpose equals od.Id
                                select new
                                {
                                    emp.EmpId,
                                    Name = emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    VistorFrom = visitbran.Name,
                                    otherduty.VistorTo,
                                    otherduty.StartDate,
                                    otherduty.EndDate,
                                    otherduty.UpdatedDate,
                                    od.ODType,
                                    otherduty.Status,
                                    otherduty.Description,
                                });
                    //Adding headers  
                    AddCellToHeader(tableLayout1, "EmpId");
                    AddCellToHeader(tableLayout1, "Name");
                    //AddCellToHeader(tableLayout1, "Designation");
                    AddCellToHeader(tableLayout1, "Branch");
                    AddCellToHeader(tableLayout1, "FromDate");
                    AddCellToHeader(tableLayout1, "ToDate");
                    AddCellToHeader(tableLayout1, "AppliedDate");
                    AddCellToHeader(tableLayout1, "VisitFrom");
                    AddCellToHeader(tableLayout1, "VisitTo");
                    AddCellToHeader(tableLayout1, "Purpose");
                    AddCellToHeader(tableLayout1, "Status");
                    AddCellToHeader(tableLayout1, "Description");
                    //Adding body  
                    foreach (var lemp in data)
                    {
                        AddCellToBody(tableLayout1, lemp.EmpId);
                        AddCellToBody(tableLayout1, lemp.Name);
                        //AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.VistorFrom);
                        AddCellToBody(tableLayout1, lemp.VistorTo);
                        AddCellToBody(tableLayout1, lemp.ODType);
                        AddCellToBody(tableLayout1, lemp.Status);
                        AddCellToBody(tableLayout1, lemp.Description);
                    }
                    return tableLayout1;
                }

                else
                {
                    DateTime lstartdate = Convert.ToDateTime(sd);
                    DateTime lenddate = Convert.ToDateTime(ed);
                    var data = (from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id
                                join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                join dept in Departments on otherduty.DepartmentId equals dept.Id
                                join branch in lBranches on otherduty.BranchId equals branch.Id
                                join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                join od in ltype on otherduty.Purpose equals od.Id
                                where ((otherduty.UpdatedDate.Date >= lstartdate.Date && otherduty.UpdatedDate.Date <= lenddate.Date)
                                      || (otherduty.UpdatedDate.Date >= lstartdate.Date && otherduty.UpdatedDate.Date <= lenddate.Date))
                                select new
                                {
                                    emp.EmpId,
                                    Name = emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    VistorFrom = visitbran.Name,
                                    otherduty.VistorTo,
                                    otherduty.StartDate,
                                    otherduty.EndDate,
                                    otherduty.UpdatedDate,
                                    od.ODType,
                                    otherduty.Status,
                                    otherduty.Description,
                                });
                    //Adding headers  
                    AddCellToHeader(tableLayout1, "EmpId");
                    AddCellToHeader(tableLayout1, "Name");
                    //AddCellToHeader(tableLayout1, "Designation");
                    AddCellToHeader(tableLayout1, "Branch");
                    AddCellToHeader(tableLayout1, "FromDate");
                    AddCellToHeader(tableLayout1, "ToDate");
                    AddCellToHeader(tableLayout1, "AppliedDate");
                    AddCellToHeader(tableLayout1, "VisitFrom");
                    AddCellToHeader(tableLayout1, "VisitTo");
                    AddCellToHeader(tableLayout1, "Purpose");
                    AddCellToHeader(tableLayout1, "Status");
                    AddCellToHeader(tableLayout1, "Description");
                    //Adding body  
                    foreach (var lemp in data)
                    {
                        AddCellToBody(tableLayout1, lemp.EmpId);
                        AddCellToBody(tableLayout1, lemp.Name);
                        //AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.VistorFrom);
                        AddCellToBody(tableLayout1, lemp.VistorTo);
                        AddCellToBody(tableLayout1, lemp.ODType);
                        AddCellToBody(tableLayout1, lemp.Status);
                        AddCellToBody(tableLayout1, lemp.Description);
                    }


                }


                return tableLayout1;
            }
            else if (la == "Request")
            {
                if (sd == "" && ed == "")
                {
                    var data = (from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id
                                join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                join dept in Departments on otherduty.DepartmentId equals dept.Id
                                join branch in lBranches on otherduty.BranchId equals branch.Id
                                join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                join od in ltype on otherduty.Purpose equals od.Id
                                select new
                                {
                                    emp.EmpId,
                                    Name = emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    VistorFrom = visitbran.Name,
                                    otherduty.VistorTo,
                                    otherduty.StartDate,
                                    otherduty.EndDate,
                                    otherduty.UpdatedDate,
                                    od.ODType,
                                    otherduty.Status,
                                    otherduty.Description,
                                });
                    //Adding headers  
                    AddCellToHeader(tableLayout1, "EmpId");
                    AddCellToHeader(tableLayout1, "Name");
                    //AddCellToHeader(tableLayout1, "Designation");
                    AddCellToHeader(tableLayout1, "Branch");
                    AddCellToHeader(tableLayout1, "FromDate");
                    AddCellToHeader(tableLayout1, "ToDate");
                    AddCellToHeader(tableLayout1, "AppliedDate");
                    AddCellToHeader(tableLayout1, "VisitFrom");
                    AddCellToHeader(tableLayout1, "VisitTo");
                    AddCellToHeader(tableLayout1, "Purpose");
                    AddCellToHeader(tableLayout1, "Status");
                    AddCellToHeader(tableLayout1, "Description");
                    //Adding body  
                    foreach (var lemp in data)
                    {
                        AddCellToBody(tableLayout1, lemp.EmpId);
                        AddCellToBody(tableLayout1, lemp.Name);
                        //AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.VistorFrom);
                        AddCellToBody(tableLayout1, lemp.VistorTo);
                        AddCellToBody(tableLayout1, lemp.ODType);
                        AddCellToBody(tableLayout1, lemp.Status);
                        AddCellToBody(tableLayout1, lemp.Description);
                    }
                    return tableLayout1;
                }
                else
                {
                    DateTime lstartdate = Convert.ToDateTime(sd);
                    DateTime lenddate = Convert.ToDateTime(ed);
                    var data = (from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id
                                join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                join dept in Departments on otherduty.DepartmentId equals dept.Id
                                join branch in lBranches on otherduty.BranchId equals branch.Id
                                join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                join od in ltype on otherduty.Purpose equals od.Id
                                where otherduty.StartDate.Date >= lstartdate.Date && otherduty.EndDate.Date <= lenddate.Date
                                   || (otherduty.EndDate.Date >= lstartdate.Date
                                  && otherduty.StartDate.Date <= lenddate.Date)
                                select new
                                {
                                    emp.EmpId,
                                    Name = emp.ShortName,
                                    desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    VistorFrom = visitbran.Name,
                                    otherduty.VistorTo,
                                    otherduty.StartDate,
                                    otherduty.EndDate,
                                    otherduty.UpdatedDate,
                                    od.ODType,
                                    otherduty.Status,
                                    otherduty.Description,
                                });
                    //Adding headers  
                    AddCellToHeader(tableLayout1, "EmpId");
                    AddCellToHeader(tableLayout1, "Name");
                    //AddCellToHeader(tableLayout1, "Designation");
                    AddCellToHeader(tableLayout1, "Branch");
                    AddCellToHeader(tableLayout1, "FromDate");
                    AddCellToHeader(tableLayout1, "ToDate");
                    AddCellToHeader(tableLayout1, "AppliedDate");
                    AddCellToHeader(tableLayout1, "VisitFrom");
                    AddCellToHeader(tableLayout1, "VisitTo");
                    AddCellToHeader(tableLayout1, "Purpose");
                    AddCellToHeader(tableLayout1, "Status");
                    AddCellToHeader(tableLayout1, "Description");
                    //Adding body  
                    foreach (var lemp in data)
                    {
                        AddCellToBody(tableLayout1, lemp.EmpId);
                        AddCellToBody(tableLayout1, lemp.Name);
                        //AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.VistorFrom);
                        AddCellToBody(tableLayout1, lemp.VistorTo);
                        AddCellToBody(tableLayout1, lemp.ODType);
                        AddCellToBody(tableLayout1, lemp.Status);
                        AddCellToBody(tableLayout1, lemp.Description);
                    }

                }
                return tableLayout1;
            }
            return tableLayout1;
        }
        //Code for Export to Excel for OD.
        public void ExportToExcelTodayOD(string empid)
        {

            try
            {
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                var lBranches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var ldeputation = db.OD_OtherDuty.ToList();
                var ltype = db.OD_Master.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
                var employeeList = (from otherduty in ldeputation
                                    join emp in lemployees on otherduty.EmpId equals emp.Id
                                    join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                    join dept in Departments on otherduty.DepartmentId equals dept.Id
                                    join branch in lBranches on otherduty.BranchId equals branch.Id
                                    join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                    join od in ltype on otherduty.Purpose equals od.Id
                                    where (lStartDate >= otherduty.StartDate.Date && lStartDate <= otherduty.EndDate.Date)
                                    || (lEndDate >= otherduty.StartDate.Date && lEndDate <= otherduty.EndDate.Date)
                                    select new
                                    {
                                        Empcode = emp.EmpId,
                                        Name = emp.ShortName,
                                        Designation = desig.Code,
                                        DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        VistingFrom = visitbran.Name,
                                        VisitingTo = otherduty.VistorTo,
                                        otherduty.StartDate,
                                        otherduty.EndDate,
                                        AppliedDate = otherduty.UpdatedDate,
                                        Purpose = od.ODType,
                                        otherduty.Description,
                                        otherduty.Status,
                                    });
                var gv = new GridView();
                gv.DataSource = employeeList;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=ODReport.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "GB2312";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                gv.Width = 5;
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        //Code for Export to Excel in All OD
        public void ExportToExcelAllOD(string empid)
        {
            try
            {
                string sd = Convert.ToString(Session["lsd"]);
                string ed = Convert.ToString(Session["led"]);
                string lApplied = Convert.ToString(Session["lApplied"]);
                string lRequest = Convert.ToString(Session["lRequest"]);
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                var lBranches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var ldeputation = db.OD_OtherDuty.ToList();
                var ltype = db.OD_Master.ToList();
                if (lApplied == "")
                {
                    var data = (from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id
                                join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                join dept in Departments on otherduty.DepartmentId equals dept.Id
                                join branch in lBranches on otherduty.BranchId equals branch.Id
                                join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                join od in ltype on otherduty.Purpose equals od.Id
                                select new
                                {
                                    Empcode = emp.EmpId,
                                    Name = emp.ShortName,
                                    Designation = desig.Code,
                                    DepartmenttBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    VistingFrom = visitbran.Name,
                                    VistingTo = otherduty.VistorTo,
                                    otherduty.StartDate,
                                    otherduty.EndDate,
                                    AppliedDate = otherduty.UpdatedDate,
                                    Purpose = od.ODType,
                                    otherduty.Description,
                                    otherduty.Status,
                                });
                    var gv = new GridView();
                    gv.DataSource = data;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=ODReport.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "GB2312";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    StringWriter objStringWriter = new StringWriter();
                    HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                    gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                    gv.Width = 5;
                    gv.RenderControl(objHtmlTextWriter);
                    Response.Output.Write(objStringWriter.ToString());
                    Session.Remove("lsd");
                    Session.Remove("led");
                    Session.Remove("lApplied");
                    Session.Remove("lRequest");
                    Response.Flush();
                    Response.End();
                }
                else if (lApplied == "Applied")
                {
                    if (sd == "" && ed == "")
                    {
                        var data = (from otherduty in ldeputation
                                    join emp in lemployees on otherduty.EmpId equals emp.Id
                                    join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                    join dept in Departments on otherduty.DepartmentId equals dept.Id
                                    join branch in lBranches on otherduty.BranchId equals branch.Id
                                    join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                    join od in ltype on otherduty.Purpose equals od.Id
                                    select new
                                    {
                                        Empcode = emp.EmpId,
                                        Name = emp.ShortName,
                                        Designation = desig.Code,
                                        DepartmenttBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        VistingFrom = visitbran.Name,
                                        VistingTo = otherduty.VistorTo,
                                        otherduty.StartDate,
                                        otherduty.EndDate,
                                        AppliedDate = otherduty.UpdatedDate,
                                        Purpose = od.ODType,
                                        otherduty.Status,
                                        otherduty.Description,
                                    });
                        var gv = new GridView();
                        gv.DataSource = data;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=ODReport.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "GB2312";
                        Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                        StringWriter objStringWriter = new StringWriter();
                        HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                        gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                        gv.Width = 5;
                        gv.RenderControl(objHtmlTextWriter);
                        Response.Output.Write(objStringWriter.ToString());
                        Session.Remove("lsd");
                        Session.Remove("led");
                        Session.Remove("lApplied");
                        Session.Remove("lRequest");
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        DateTime lstartdate = Convert.ToDateTime(sd);
                        DateTime lenddate = Convert.ToDateTime(ed);
                        var data = (from otherduty in ldeputation
                                    join emp in lemployees on otherduty.EmpId equals emp.Id
                                    join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                    join dept in Departments on otherduty.DepartmentId equals dept.Id
                                    join branch in lBranches on otherduty.BranchId equals branch.Id
                                    join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                    join od in ltype on otherduty.Purpose equals od.Id
                                    where ((otherduty.UpdatedDate.Date >= lstartdate.Date && otherduty.UpdatedDate.Date <= lenddate.Date)
                                          || (otherduty.UpdatedDate.Date >= lstartdate.Date && otherduty.UpdatedDate.Date <= lenddate.Date))
                                    select new
                                    {
                                        Empcode = emp.EmpId,
                                        Name = emp.ShortName,
                                        Designation = desig.Code,
                                        DepartmenttBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        VistingFrom = visitbran.Name,
                                        VistingTo = otherduty.VistorTo,
                                        otherduty.StartDate,
                                        otherduty.EndDate,
                                        AppliedDate = otherduty.UpdatedDate,
                                        Purpose = od.ODType,
                                        otherduty.Status,
                                        otherduty.Description,
                                    });
                        var gv = new GridView();
                        gv.DataSource = data;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=ODReport.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "GB2312";
                        Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                        StringWriter objStringWriter = new StringWriter();
                        HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                        gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                        gv.Width = 5;
                        gv.RenderControl(objHtmlTextWriter);
                        Response.Output.Write(objStringWriter.ToString());
                        Session.Remove("lsd");
                        Session.Remove("led");
                        Session.Remove("lApplied");
                        Session.Remove("lRequest");
                        Response.Flush();
                        Response.End();
                    }
                }
                else if (lApplied == "Request")
                {
                    if (sd == "" && ed == "")
                    {
                        var data = (from otherduty in ldeputation
                                    join emp in lemployees on otherduty.EmpId equals emp.Id
                                    join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                    join dept in Departments on otherduty.DepartmentId equals dept.Id
                                    join branch in lBranches on otherduty.BranchId equals branch.Id
                                    join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                    join od in ltype on otherduty.Purpose equals od.Id
                                    select new
                                    {
                                        Empcode = emp.EmpId,
                                        Name = emp.ShortName,
                                        Designation = desig.Code,
                                        DepartmenttBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        VistingFrom = visitbran.Name,
                                        VistingTo = otherduty.VistorTo,
                                        otherduty.StartDate,
                                        otherduty.EndDate,
                                        AppliedDate = otherduty.UpdatedDate,
                                        Purpose = od.ODType,
                                        otherduty.Status,
                                        otherduty.Description,
                                    });
                        var gv = new GridView();
                        gv.DataSource = data;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=ODReport.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "GB2312";
                        Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                        StringWriter objStringWriter = new StringWriter();
                        HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                        gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                        gv.Width = 5;
                        gv.RenderControl(objHtmlTextWriter);
                        Response.Output.Write(objStringWriter.ToString());
                        Session.Remove("lsd");
                        Session.Remove("led");
                        Session.Remove("lApplied");
                        Session.Remove("lRequest");
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        DateTime lstartdate = Convert.ToDateTime(sd);
                        DateTime lenddate = Convert.ToDateTime(ed);
                        var data = (from otherduty in ldeputation
                                    join emp in lemployees on otherduty.EmpId equals emp.Id
                                    join desig in ldesignation on otherduty.DesignationId equals desig.Id
                                    join dept in Departments on otherduty.DepartmentId equals dept.Id
                                    join branch in lBranches on otherduty.BranchId equals branch.Id
                                    join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                    join od in ltype on otherduty.Purpose equals od.Id
                                    where otherduty.StartDate.Date >= lstartdate.Date && otherduty.EndDate.Date <= lenddate.Date
                                       || (otherduty.EndDate.Date >= lstartdate.Date
                                      && otherduty.StartDate.Date <= lenddate.Date)
                                    select new
                                    {
                                        Empcode = emp.EmpId,
                                        Name = emp.ShortName,
                                        Designation = desig.Code,
                                        DepartmenttBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        VistingFrom = visitbran.Name,
                                        VistingTo = otherduty.VistorTo,
                                        otherduty.StartDate,
                                        otherduty.EndDate,
                                        AppliedDate = otherduty.UpdatedDate,
                                        Purpose = od.ODType,
                                        otherduty.Status,
                                        otherduty.Description,
                                    });
                        var gv = new GridView();
                        gv.DataSource = data;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=ODReport.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.Charset = "GB2312";
                        Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                        StringWriter objStringWriter = new StringWriter();
                        HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                        gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                        gv.Width = 5;
                        gv.RenderControl(objHtmlTextWriter);
                        Response.Output.Write(objStringWriter.ToString());
                        Session.Remove("lsd");
                        Session.Remove("led");
                        Session.Remove("lApplied");
                        Session.Remove("lRequest");
                        Response.Flush();
                        Response.End();
                    }

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
    }
}




