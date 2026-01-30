using HRMSApplication.Helpers;
using HRMSApplication.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities;
using HRMSApplication.Filters;
using System.Web.UI;
using System.Data;
using Mavensoft.DAL.Db;
using Newtonsoft.Json;


namespace HRMSApplication.Controllers
{
    public class DashBoardController : Controller
    {
        SqlHelper sh = new SqlHelper();
        private ContextBase db = new ContextBase();

        // GET: DashBoard
        [SessionTimeoutAttribute]
        [Authorize]
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
        public ActionResult Index()
        {
            LogInformation.Info("DashBoardController.Index() - Method started");

            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();

                // Check if user credentials are null
                if (lCredentails == null)
                {
                    LogInformation.Info("DashBoardController.Index() - User credentials are null, redirecting to login");
                    return RedirectToAction("Login", "Account");
                }

                LogInformation.Info("DashBoardController.Index() - User credentials retrieved: " + lCredentails.EmpId);

                // Check if essential properties are null
                if (string.IsNullOrEmpty(lCredentails.EmpId))
                {
                    LogInformation.Info("DashBoardController.Index() - Employee ID is null or empty, redirecting to login");
                    return RedirectToAction("Login", "Account");
                }

                Session.SetDataToSession<string>("EmpFullName", lCredentails.EmpFullName ?? "");
                Session.SetDataToSession<string>("ActiveEmployee", lCredentails.EmpId);
                Session.SetDataToSession<string>("CurrDesig", lCredentails.CurrDesig ?? "");
                Session.SetDataToSession<string>("ActiveImage", lCredentails.EmpImage ?? "");
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();

                LogInformation.Info("DashBoardController.Index() - Session data set and role pages retrieved");

                List<string[]> lEmpbirthdaysss = new List<string[]>();
                List<string> lEmpbirthdays = new List<string>();

                List<string[]> lEmpRetirements = new List<string[]>();
                List<string> lEmpretirement = new List<string>();

                List<string[]> lEmpNews = new List<string[]>();
                List<string> lNews = new List<string>();

                List<string[]> lEmpLeaves = new List<string[]>();
                List<string> lEmpleaves = new List<string>();

                string Birthdates = string.Empty;
                string sMonth = DateTime.Now.ToString("MMM");
                string sMonth1 = DateTime.Now.ToString("MMMM yyyy");
                DateTime currentdate = GetCurrentTime(DateTime.Now);
                int month = currentdate.Month;

                LogInformation.Info("DashBoardController.Index() - Current date: " + currentdate + ", Month: " + month);

                var lleaves = db.Leaves.ToList();
                var lemployees = db.Employes.ToList();

                LogInformation.Info("DashBoardController.Index() - Leaves count: " + lleaves.Count + ", Employees count: " + lemployees.Count);

                var startOfMonth = new DateTime(currentdate.Year, currentdate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                startOfMonth = startOfMonth.AddDays(-4);
                endOfMonth = endOfMonth.AddDays(4);
                int lmonth = startOfMonth.Month;
                int lmonth1 = endOfMonth.Month;
                int lmonth2 = lmonth + 1;
                int sday = startOfMonth.Day;
                int eday = endOfMonth.Day;

                LogInformation.Info("DashBoardController.Index() - Date range - Start: " + startOfMonth + ", End: " + endOfMonth);
                LogInformation.Info("DashBoardController.Index() - Month calculations - lmonth: " + lmonth + ", lmonth1: " + lmonth1 + ", lmonth2: " + lmonth2);

                // Fixed: Added null checks and safe date conversion with proper entity materialization
                var lbirthdaysPrevMonth = db.Employes
                    .Select(a => new
                    {
                        a.Id,
                        a.EmpId,
                        a.ShortName,
                        a.UploadPhoto,
                        a.DOB,
                        a.RetirementDate,
                        a.Exit_type
                    })
                    .Where(a => a.DOB != null)
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.DOB).Month == lmonth && Convert.ToDateTime(a.DOB).Day >= sday)
                    .Where(a => a.RetirementDate != null && a.RetirementDate >= DateTime.Now)
                    .OrderBy(a => Convert.ToDateTime(a.DOB).Day)
                    .ToList();

                LogInformation.Info("DashBoardController.Index() - Previous month birthdays count: " + lbirthdaysPrevMonth.Count);

                var lbirthdaysPrevMonth1 = db.Employes
                    .Select(a => new
                    {
                        a.Id,
                        a.EmpId,
                        a.ShortName,
                        a.UploadPhoto,
                        a.DOB,
                        a.RetirementDate,
                        a.Exit_type
                    })
                    .Where(a => a.DOB != null)
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.DOB).Month == lmonth2)
                    .Where(a => a.RetirementDate != null && a.RetirementDate >= DateTime.Now)
                    .OrderBy(a => Convert.ToDateTime(a.DOB).Day)
                    .ToList();

                LogInformation.Info("DashBoardController.Index() - Previous month1 birthdays count: " + lbirthdaysPrevMonth1.Count);

                var lbirthdaysPrevMonth2 = db.Employes
                    .Select(a => new
                    {
                        a.Id,
                        a.EmpId,
                        a.ShortName,
                        a.UploadPhoto,
                        a.DOB,
                        a.RetirementDate,
                        a.Exit_type
                    })
                    .Where(a => a.DOB != null)
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.DOB).Month == lmonth1 && Convert.ToDateTime(a.DOB).Day <= eday)
                    .Where(a => a.RetirementDate != null && a.RetirementDate >= DateTime.Now)
                    .OrderBy(a => Convert.ToDateTime(a.DOB).Day)
                    .ToList();

                LogInformation.Info("DashBoardController.Index() - Previous month2 birthdays count: " + lbirthdaysPrevMonth2.Count);

                var lcurbirthday = db.Employes
                    .Select(a => new
                    {
                        a.Id,
                        a.EmpId,
                        a.ShortName,
                        a.UploadPhoto,
                        a.DOB,
                        a.RetirementDate,
                        a.Exit_type
                    })
                    .Where(a => a.DOB != null)
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.DOB).Month == month)
                    .Where(a => a.RetirementDate != null && a.RetirementDate >= DateTime.Now)
                    .OrderBy(a => Convert.ToDateTime(a.DOB).Day)
                    .ToList();

                LogInformation.Info("DashBoardController.Index() - Current month birthdays count: " + lcurbirthday.Count);

                var lbirthdays = lbirthdaysPrevMonth2.Union(lcurbirthday);
                var lbirthdaysNext = lbirthdays.Union(lbirthdaysPrevMonth);
                var lbirthdaysnextnext = lbirthdaysNext.Union(lbirthdaysPrevMonth1);

                LogInformation.Info("DashBoardController.Index() - Total combined birthdays count: " + lbirthdaysnextnext.Count());

                foreach (var items in lcurbirthday)
                {
                    if (items.DOB != null)
                    {
                        DateTime ldbmonth = Convert.ToDateTime(items.DOB);
                        int ldobmonth = ldbmonth.Month;
                        string sdate = ldbmonth.ToString("dd MMMM");

                        LogInformation.Info("DashBoardController.Index() - Processing birthday for employee: " + items.ShortName + ", DOB: " + sdate);

                        lEmpbirthdays.Add(items.UploadPhoto);
                        lEmpbirthdays.Add(items.ShortName);
                        lEmpbirthdays.Add(sdate);
                        lEmpbirthdaysss.Add(lEmpbirthdays.ToArray());
                        lEmpbirthdays.Remove(sdate);
                        lEmpbirthdays.Remove(items.ShortName);
                        lEmpbirthdays.Remove(items.UploadPhoto);
                    }
                }

                LogInformation.Info("DashBoardController.Index() - Birthday data processed, total birthday entries: " + lEmpbirthdaysss.Count);

                var lnews = db.News.ToList().Where(a => Convert.ToDateTime(a.UpdatedDate.Date) == currentdate.Date).ToList();
                LogInformation.Info("DashBoardController.Index() - News count for current date: " + lnews.Count);

                foreach (var item in lnews)
                {
                    DateTime ldbdate = Convert.ToDateTime(item.UpdatedDate.Date);
                    if (currentdate.Date == ldbdate)
                    {
                        LogInformation.Info("DashBoardController.Index() - Processing news: " + item.Subject);

                        lNews.Add(item.Subject);
                        lNews.Add(item.Content);
                        lEmpNews.Add(lNews.ToArray());
                    }
                    lNews.Remove(item.Content);
                    lNews.Remove(item.Subject);
                }

                LogInformation.Info("DashBoardController.Index() - News data processed, total news entries: " + lEmpNews.Count);

                var lretired = db.Employes
                    .Select(a => new
                    {
                        a.Id,
                        a.EmpId,
                        a.ShortName,
                        a.UploadPhoto,
                        a.DOB,
                        a.RetirementDate,
                        a.Exit_type
                    })
                    .Where(a => a.RetirementDate != null)
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.RetirementDate).Month == month)
                    .Where(a => a.Exit_type == "Retd")
                    .ToList();

                LogInformation.Info("DashBoardController.Index() - Retired employees count for current month: " + lretired.Count);

                foreach (var item1 in lretired)
                {
                    if (item1.RetirementDate != null)
                    {
                        DateTime ldbmonth = Convert.ToDateTime(item1.RetirementDate);
                        string cmonth1 = Convert.ToDateTime(item1.RetirementDate).ToString("MMMM yyyy");
                        string sdate = ldbmonth.ToString("dd MMMM yyyy");
                        if (cmonth1 == sMonth1)
                        {
                            LogInformation.Info("DashBoardController.Index() - Processing retirement for employee: " + item1.ShortName + ", Retirement date: " + sdate);

                            lEmpretirement.Add(item1.UploadPhoto);
                            lEmpretirement.Add(item1.ShortName);
                            lEmpretirement.Add(sdate);
                            lEmpRetirements.Add(lEmpretirement.ToArray());
                        }
                        lEmpretirement.Remove(sdate);
                        lEmpretirement.Remove(item1.ShortName);
                        lEmpretirement.Remove(item1.UploadPhoto);
                    }
                }

                LogInformation.Info("DashBoardController.Index() - Retirement data processed, total retirement entries: " + lEmpRetirements.Count);

                var newleavestatus = new string[] { "Approved", "Pending", "Forwarded" }; // added by chaitanya on 12/05/2020
                LogInformation.Info("DashBoardController.Index() - Leave statuses to process: " + string.Join(", ", newleavestatus));

                var lresultleaves = (from leave in lleaves
                                     join emp in lemployees on leave.EmpId equals emp.Id
                                     where (currentdate.Date >= leave.StartDate && currentdate.Date <= leave.EndDate)
                                     //|| (currentdate.Date >= leave.StartDate && currentdate.Date <= leave.EndDate)
                                     //where leave.Status == "Approved"
                                     && newleavestatus.Contains(leave.Status)
                                     select new
                                     {
                                         StartDate = leave.StartDate.ToString("dd MMMM yyyy"),
                                         EndDate = leave.EndDate.ToString("dd MMMM yyyy"),
                                         LeaveStatus = leave.Status, // added by chaitanya on 12/05/2020
                                         EmployeeName = emp.ShortName,
                                         EmployeePhoto = emp.UploadPhoto
                                     });

                LogInformation.Info("DashBoardController.Index() - Leave query result count: " + lresultleaves.Count());

                foreach (var item in lresultleaves)
                {
                    LogInformation.Info("DashBoardController.Index() - Processing leave for employee: " + item.EmployeeName + ", Status: " + item.LeaveStatus + ", Period: " + item.StartDate + " to " + item.EndDate);

                    lEmpleaves.Add(item.EmployeePhoto);
                    lEmpleaves.Add(item.EmployeeName);
                    lEmpleaves.Add(item.StartDate.ToString());
                    lEmpleaves.Add(item.EndDate.ToString());
                    lEmpleaves.Add(item.LeaveStatus.ToString()); // added by chaitanya on 12/05/2020
                    lEmpLeaves.Add(lEmpleaves.ToArray());
                    lEmpleaves.Remove(item.EmployeePhoto);
                    lEmpleaves.Remove(item.EmployeeName);
                    lEmpleaves.Remove(item.StartDate.ToString());
                    lEmpleaves.Remove(item.EndDate.ToString());
                    lEmpleaves.Remove(item.LeaveStatus.ToString()); // added by chaitanya on 12/05/2020
                }

                LogInformation.Info("DashBoardController.Index() - Leave data processed, total leave entries: " + lEmpLeaves.Count);

                ViewBag.empid = lCredentails.EmpId;
                var lmodel = new DashBoard();
                lmodel.Birthdays = lEmpbirthdaysss.ToList();
                lmodel.News = lEmpNews.ToList();
                lmodel.Retirements = lEmpRetirements.ToList();
                lmodel.Leaves = lEmpLeaves.ToList();
                ViewBag.empid = lCredentails.EmpId;

                LogInformation.Info("DashBoardController.Index() - Dashboard model populated - Birthdays: " + lmodel.Birthdays.Count + ", News: " + lmodel.News.Count + ", Retirements: " + lmodel.Retirements.Count + ", Leaves: " + lmodel.Leaves.Count);

                //System.Threading.Thread.Sleep(3000);
                var memoid = memocount();
                TempData["count"] = memoid;
                var empmemoid = empmemocount();
                TempData["Empcount"] = empmemoid;

                LogInformation.Info("DashBoardController.Index() - Memo counts - Personal: " + memoid + ", Employee: " + empmemoid);
                LogInformation.Info("DashBoardController.Index() - Method completed successfully");

                return View(lmodel);
            }
            catch (Exception ex)
            {
                LogInformation.Info("DashBoardController.Index() - Error occurred: " + ex.Message);
                LogInformation.Info("DashBoardController.Index() - Stack trace: " + ex.StackTrace);
                throw;
            }
        }


        public ActionResult EmployeeDashBoard()
        {
            LogInformation.Info("DashBoardController.EmployeeDashBoard() - Method started");

            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();

                // Check if user credentials are null
                if (lCredentails == null)
                {
                    LogInformation.Info("DashBoardController.EmployeeDashBoard() - User credentials are null, redirecting to login");
                    return RedirectToAction("Login", "Account");
                }

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - User credentials retrieved: " + lCredentails.EmpId);

                // Check if essential properties are null
                if (string.IsNullOrEmpty(lCredentails.EmpId))
                {
                    LogInformation.Info("DashBoardController.EmployeeDashBoard() - Employee ID is null or empty, redirecting to login");
                    return RedirectToAction("Login", "Account");
                }

                Session.SetDataToSession<string>("EmpFullName", lCredentails.EmpFullName ?? "");
                Session.SetDataToSession<string>("ActiveEmployee", lCredentails.EmpId);
                Session.SetDataToSession<string>("ActiveImage", lCredentails.EmpImage ?? "");
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Session data set and role pages retrieved");

                List<string[]> lEmpbirthdaysss = new List<string[]>();
                List<string> lEmpbirthdays = new List<string>();

                List<string[]> lEmpRetirements = new List<string[]>();
                List<string> lEmpretirement = new List<string>();

                List<string[]> lEmpNews = new List<string[]>();
                List<string> lNews = new List<string>();

                string Birthdates = string.Empty;
                string sMonth = DateTime.Now.ToString("MMM");
                string sMonth1 = DateTime.Now.ToString("MMMM yyyy");
                DateTime currentdate = GetCurrentTime(DateTime.Now);
                int month = currentdate.Month;

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Current date: " + currentdate + ", Month: " + month);

                var lleaves = db.Leaves.ToList();
                var startOfMonth = new DateTime(currentdate.Year, currentdate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                startOfMonth = startOfMonth.AddDays(-4);
                endOfMonth = endOfMonth.AddDays(4);
                int lmonth = startOfMonth.Month;
                int lmonth1 = endOfMonth.Month;
                int lmonth2 = lmonth + 1;
                int sday = startOfMonth.Day;
                int eday = endOfMonth.Day;

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Date range - Start: " + startOfMonth + ", End: " + endOfMonth);
                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Month calculations - lmonth: " + lmonth + ", lmonth1: " + lmonth1 + ", lmonth2: " + lmonth2);

                // Fixed: Added null checks and safe date conversion with proper entity materialization
                var lbirthdaysPrevMonth = db.Employes
                    .Select(a => new
                    {
                        a.Id,
                        a.EmpId,
                        a.ShortName,
                        a.UploadPhoto,
                        a.DOB,
                        a.RetirementDate,
                        a.Exit_type
                    })
                    .Where(a => a.DOB != null)
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.DOB).Month == lmonth && Convert.ToDateTime(a.DOB).Day >= sday)
                    .OrderBy(a => Convert.ToDateTime(a.DOB).Day)
                    .ToList();

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Previous month birthdays count: " + lbirthdaysPrevMonth.Count);

                var lbirthdaysPrevMonth1 = db.Employes
                    .Select(a => new
                    {
                        a.Id,
                        a.EmpId,
                        a.ShortName,
                        a.UploadPhoto,
                        a.DOB,
                        a.RetirementDate,
                        a.Exit_type
                    })
                    .Where(a => a.DOB != null)
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.DOB).Month == lmonth2)
                    .OrderBy(a => Convert.ToDateTime(a.DOB).Day)
                    .ToList();

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Previous month1 birthdays count: " + lbirthdaysPrevMonth1.Count);

                var lbirthdaysPrevMonth2 = db.Employes
                    .Select(a => new
                    {
                        a.Id,
                        a.EmpId,
                        a.ShortName,
                        a.UploadPhoto,
                        a.DOB,
                        a.RetirementDate,
                        a.Exit_type
                    })
                    .Where(a => a.DOB != null)
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.DOB).Month == lmonth1 && Convert.ToDateTime(a.DOB).Day <= eday)
                    .OrderBy(a => Convert.ToDateTime(a.DOB).Day)
                    .ToList();

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Previous month2 birthdays count: " + lbirthdaysPrevMonth2.Count);

                var lbirthdays = lbirthdaysPrevMonth.Union(lbirthdaysPrevMonth1);
                var lbirthdaysNext = lbirthdays.Union(lbirthdaysPrevMonth2);

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Total combined birthdays count: " + lbirthdaysNext.Count());

                foreach (var items in lbirthdaysNext)
                {
                    if (items.DOB != null)
                    {
                        DateTime ldbmonth = Convert.ToDateTime(items.DOB);
                        string sdate = ldbmonth.ToString("dd MMMM dddd");

                        LogInformation.Info("DashBoardController.EmployeeDashBoard() - Processing birthday for employee: " + items.ShortName + ", DOB: " + sdate);

                        lEmpbirthdays.Add(items.UploadPhoto);
                        lEmpbirthdays.Add(items.ShortName);
                        lEmpbirthdays.Add(sdate);
                        lEmpbirthdaysss.Add(lEmpbirthdays.ToArray());
                        lEmpbirthdays.Remove(sdate);
                        lEmpbirthdays.Remove(items.ShortName);
                        lEmpbirthdays.Remove(items.UploadPhoto);
                    }
                }

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Birthday data processed, total birthday entries: " + lEmpbirthdaysss.Count);

                var lnews = db.News.ToList().Where(a => Convert.ToDateTime(a.UpdatedDate.Date) == currentdate.Date).ToList();
                LogInformation.Info("DashBoardController.EmployeeDashBoard() - News count for current date: " + lnews.Count);

                foreach (var item in lnews)
                {
                    DateTime ldbdate = Convert.ToDateTime(item.UpdatedDate.Date);
                    if (currentdate.Date == ldbdate)
                    {
                        LogInformation.Info("DashBoardController.EmployeeDashBoard() - Processing news: " + item.Subject);

                        lNews.Add(item.Content);
                        lNews.Add(item.Subject);
                        lNews.Add(item.Notes);
                        lEmpNews.Add(lNews.ToArray());
                    }
                    lNews.Remove(item.Content);
                    lNews.Remove(item.Subject);
                    lNews.Remove(item.Notes);
                }

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - News data processed, total news entries: " + lEmpNews.Count);

                var lretired = db.Employes
                    .Select(a => new
                    {
                        a.Id,
                        a.EmpId,
                        a.ShortName,
                        a.UploadPhoto,
                        a.DOB,
                        a.RetirementDate,
                        a.Exit_type
                    })
                    .Where(a => a.RetirementDate != null)
                    .ToList()
                    .Where(a => Convert.ToDateTime(a.RetirementDate).Month == month)
                    .Where(a => a.Exit_type == "Retd")
                    .ToList();

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Retired employees count for current month: " + lretired.Count);

                foreach (var item1 in lretired)
                {
                    if (item1.RetirementDate != null)
                    {
                        DateTime ldbmonth = Convert.ToDateTime(item1.RetirementDate);
                        string cmonth1 = Convert.ToDateTime(item1.RetirementDate).ToString("MMMM yyyy");
                        string sdate = ldbmonth.ToString("dd MMMM dddd");
                        if (cmonth1 == sMonth1)
                        {
                            LogInformation.Info("DashBoardController.EmployeeDashBoard() - Processing retirement for employee: " + item1.ShortName + ", Retirement date: " + sdate);

                            lEmpretirement.Add(item1.UploadPhoto);
                            lEmpretirement.Add(item1.ShortName);
                            lEmpretirement.Add(sdate);
                            lEmpRetirements.Add(lEmpretirement.ToArray());
                        }
                        lEmpretirement.Remove(sdate);
                        lEmpretirement.Remove(item1.ShortName);
                        lEmpretirement.Remove(item1.UploadPhoto);
                    }
                }

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Retirement data processed, total retirement entries: " + lEmpRetirements.Count);

                var lmodel = new DashBoard();
                lmodel.Birthdays = lEmpbirthdaysss.ToList();
                lmodel.News = lEmpNews.ToList();
                lmodel.Retirements = lEmpRetirements.ToList();

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Dashboard model populated - Birthdays: " + lmodel.Birthdays.Count + ", News: " + lmodel.News.Count + ", Retirements: " + lmodel.Retirements.Count);

                var memoid = memocount();
                TempData["count"] = memoid;
                var empmemoid = empmemocount();
                TempData["Empcount"] = empmemoid;

                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Memo counts - Personal: " + memoid + ", Employee: " + empmemoid);
                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Method completed successfully");

                return View(lmodel);
            }
            catch (Exception ex)
            {
                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Error occurred: " + ex.Message);
                LogInformation.Info("DashBoardController.EmployeeDashBoard() - Stack trace: " + ex.StackTrace);
                throw;
            }
        }



        public static DateTime GetCurrentTime(DateTime ldate)
        {
            LogInformation.Info("DashBoardController.GetCurrentTime() - Method called with date: " + ldate);

            try
            {
                DateTime serverTime = DateTime.Now;
                DateTime utcTime = serverTime.ToUniversalTime();
                // convert it to Utc using timezone setting of server computer
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                LogInformation.Info("DashBoardController.GetCurrentTime() - Time conversion completed - Server: " + serverTime + ", UTC: " + utcTime + ", Local: " + localTime);

                return localTime;
            }
            catch (Exception ex)
            {
                LogInformation.Info("DashBoardController.GetCurrentTime() - Error in time conversion: " + ex.Message);
                throw;
            }
        }

        public void SetActiveEmployeeImage(string userName)
        {
            LogInformation.Info("DashBoardController.SetActiveEmployeeImage() - Method called with userName: " + userName);

            try
            {
                string value = Session.GetDataFromSession<string>("EmpFullName");
                LogInformation.Info("DashBoardController.SetActiveEmployeeImage() - Session EmpFullName value: " + value);

                if (string.IsNullOrEmpty(value))
                {
                    LogInformation.Info("DashBoardController.SetActiveEmployeeImage() - EmpFullName is null or empty, getting supervisor emp id");
                    // Get the emp id for the Supervisor
                }
                // Else part : not required as current employee is already set .
                // It will be set in Employee selection.

                LogInformation.Info("DashBoardController.SetActiveEmployeeImage() - Method completed successfully");
            }
            catch (Exception ex)
            {
                LogInformation.Info("DashBoardController.SetActiveEmployeeImage() - Error occurred: " + ex.Message);
                LogInformation.Info("DashBoardController.SetActiveEmployeeImage() - Stack trace: " + ex.StackTrace);
                throw;
            }
        }


        public void SetActiveEmployee(string userName)
        {
            LogInformation.Info("DashBoardController.SetActiveEmployee() - Method called with userName: " + userName);

            try
            {
                string value = Session.GetDataFromSession<string>("ActiveEmployee");
                LogInformation.Info("DashBoardController.SetActiveEmployee() - Session ActiveEmployee value: " + value);

                if (string.IsNullOrEmpty(value))
                {
                    LogInformation.Info("DashBoardController.SetActiveEmployee() - ActiveEmployee is null or empty, getting supervisor emp id");
                    // Get the emp id for the Supervisor
                }
                // Else part : not required as current employee is already set .
                // It will be set in Employee selection.

                LogInformation.Info("DashBoardController.SetActiveEmployee() - Method completed successfully");
            }
            catch (Exception ex)
            {
                LogInformation.Info("DashBoardController.SetActiveEmployee() - Error occurred: " + ex.Message);
                LogInformation.Info("DashBoardController.SetActiveEmployee() - Stack trace: " + ex.StackTrace);
                throw;
            }
        }

        public void SetActiveImage(string image)
        {
            LogInformation.Info("DashBoardController.SetActiveImage() - Method called with image: " + image);

            try
            {
                string value = Session.GetDataFromSession<string>("ActiveImage");
                LogInformation.Info("DashBoardController.SetActiveImage() - Session ActiveImage value: " + value);

                if (string.IsNullOrEmpty(value))
                {
                    LogInformation.Info("DashBoardController.SetActiveImage() - ActiveImage is null or empty");
                    // Get the emp id for the Supervisor
                }

                LogInformation.Info("DashBoardController.SetActiveImage() - Method completed successfully");
            }
            catch (Exception ex)
            {
                LogInformation.Info("DashBoardController.SetActiveImage() - Error occurred: " + ex.Message);
                LogInformation.Info("DashBoardController.SetActiveImage() - Stack trace: " + ex.StackTrace);
                throw;
            }
        }

        public string employeetemptransfer()
        {
            LogInformation.Info("DashBoardController.employeetemptransfer() - Method started");

            try
            {
                string notmatchedQryCreate = " Create table Employee_transfer_temp_branch (empid int);Create table Employee_transfer_temp_department (empid int);Create table Employee_transfer_temp_designation (empid int);";
                LogInformation.Info("DashBoardController.employeetemptransfer() - Creating temporary tables");

                sh.Run_UPDDEL_ExecuteNonQuery(notmatchedQryCreate);
                LogInformation.Info("DashBoardController.employeetemptransfer() - Temporary tables created successfully");

                string notmachedQryCount = "exec sp_emptransfer_deviation;";
                LogInformation.Info("DashBoardController.employeetemptransfer() - Executing stored procedure: sp_emptransfer_deviation");

                bool result = sh.Run_UPDDEL_ExecuteNonQuery(notmachedQryCount);
                LogInformation.Info("DashBoardController.employeetemptransfer() - Stored procedure execution result: " + result);

                string queryselect = "select count(*) as cntOfEmployee from Employee_transfer_temp_designation " +
                    "union select count(*) as cntOfEmployee from Employee_transfer_temp_branch union " +
                    "select count(*) as cntOfEmployee from Employee_transfer_temp_department";

                LogInformation.Info("DashBoardController.employeetemptransfer() - Executing count query");

                DataTable dt = sh.Get_Table_FromQry(queryselect);
                LogInformation.Info("DashBoardController.employeetemptransfer() - Count query result rows: " + dt.Rows.Count);

                string droptemp = "Drop table Employee_transfer_temp_branch; Drop table Employee_transfer_temp_department;Drop table Employee_transfer_temp_designation;";
                LogInformation.Info("DashBoardController.employeetemptransfer() - Dropping temporary tables");

                sh.Run_UPDDEL_ExecuteNonQuery(droptemp);
                LogInformation.Info("DashBoardController.employeetemptransfer() - Temporary tables dropped successfully");

                string resultJson = JsonConvert.SerializeObject(dt);
                LogInformation.Info("DashBoardController.employeetemptransfer() - Method completed, returning JSON result");

                return resultJson;
            }
            catch (Exception ex)
            {
                LogInformation.Info("DashBoardController.employeetemptransfer() - Error occurred: " + ex.Message);
                LogInformation.Info("DashBoardController.employeetemptransfer() - Stack trace: " + ex.StackTrace);
                throw;
            }
        }

        public int memocount()
        {
            LogInformation.Info("DashBoardController.memocount() - Method started");

            try
            {
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                var latememo = db.Latememo.ToList();
                LogInformation.Info("DashBoardController.memocount() - Late memo count from database: " + latememo.Count);

                //TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();

                // Check if user credentials are null
                if (lCredentials == null)
                {
                    LogInformation.Info("DashBoardController.memocount() - User credentials are null, returning 0");
                    return 0;
                }

                LogInformation.Info("DashBoardController.memocount() - User credentials retrieved: " + lCredentials.EmpId);

                // Check if essential properties are null
                if (string.IsNullOrEmpty(lCredentials.EmpId))
                {
                    LogInformation.Info("DashBoardController.memocount() - Employee ID is null or empty, returning 0");
                    return 0;
                }

                //TempData.Keep();
                var EmpId = lCredentials.EmpId;
                var lUserLoginId = EmpId;
                string currentDate = DateTime.UtcNow.ToString("yyyy-MM-dd 00:00:00");

                LogInformation.Info("DashBoardController.memocount() - Current UTC date: " + currentDate);

                var lResult = (from memolist in latememo
                               where memolist.Empid == lUserLoginId
                               && memolist.Status == "Pending" && memolist.Duedate >= DateTime.Parse(currentDate)
                               select memolist).Count();

                LogInformation.Info("DashBoardController.memocount() - Pending memo count for user: " + lResult);

                var lresponseArray = lResult;
                //var lresponseArray = lResult;
                TempData["count"] = lresponseArray;

                LogInformation.Info("DashBoardController.memocount() - Method completed, returning count: " + lresponseArray);

                return lresponseArray;
                //TempData["latememos"] = lresponseArray.ToList();
            }
            catch (Exception ex)
            {
                LogInformation.Info("DashBoardController.memocount() - Error occurred: " + ex.Message);
                LogInformation.Info("DashBoardController.memocount() - Stack trace: " + ex.StackTrace);
                throw;
            }
        }

        public int empmemocount()
        {
            LogInformation.Info("DashBoardController.empmemocount() - Method started");

            try
            {
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                var latememo = db.Latememo.ToList();
                LogInformation.Info("DashBoardController.empmemocount() - Late memo count from database: " + latememo.Count);

                LoginCredential lCredentials = LoginHelper.GetCurrentUser();

                // Check if user credentials are null
                if (lCredentials == null)
                {
                    LogInformation.Info("DashBoardController.empmemocount() - User credentials are null, returning 0");
                    return 0;
                }

                var EmpId = lCredentials.EmpId;

                // Check if essential properties are null
                if (string.IsNullOrEmpty(EmpId))
                {
                    LogInformation.Info("DashBoardController.empmemocount() - Employee ID is null or empty, returning 0");
                    return 0;
                }

                var lUserLoginId = EmpId;

                LogInformation.Info("DashBoardController.empmemocount() - User credentials retrieved: " + EmpId);

                var controlling = db.Employes.Where(a => a.EmpId == EmpId).Select(a => a.Id).FirstOrDefault();
                var contrlid = controlling;

                LogInformation.Info("DashBoardController.empmemocount() - Controlling authority ID: " + contrlid);

                string currentDate = DateTime.UtcNow.ToString("yyyy-MM-dd 00:00:00");
                LogInformation.Info("DashBoardController.empmemocount() - Current UTC date: " + currentDate);

                var lResult = (from memolist in latememo
                               where memolist.controllingauthority == contrlid.ToString()
                                 && memolist.MemoType == "Late Applied Leave"
                               && memolist.Status == "Pending" && memolist.Duedate >= DateTime.Parse(currentDate)
                               select memolist).Count();

                LogInformation.Info("DashBoardController.empmemocount() - Pending late applied leave memo count: " + lResult);

                var emplresponseArray = lResult;
                if (lResult != null)
                {
                    TempData["Empcount"] = emplresponseArray;
                    LogInformation.Info("DashBoardController.empmemocount() - Empcount set in TempData: " + emplresponseArray);
                }

                LogInformation.Info("DashBoardController.empmemocount() - Method completed, returning count: " + emplresponseArray);

                return emplresponseArray;
            }
            catch (Exception ex)
            {
                LogInformation.Info("DashBoardController.empmemocount() - Error occurred: " + ex.Message);
                LogInformation.Info("DashBoardController.empmemocount() - Stack trace: " + ex.StackTrace);
                throw;
            }
        }

        private DateTime getdate()
        {
            LogInformation.Info("DashBoardController.getdate() - Method called but not implemented");
            throw new NotImplementedException();
        }
    }
}
