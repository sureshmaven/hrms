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
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            Session.SetDataToSession<string>("EmpFullName", lCredentails.EmpFullName);
            Session.SetDataToSession<string>("ActiveEmployee", lCredentails.EmpId);
            Session.SetDataToSession<string>("CurrDesig", lCredentails.CurrDesig);
            Session.SetDataToSession<string>("ActiveImage", lCredentails.EmpImage);
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
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
            var lleaves = db.Leaves.ToList();
            var lemployees = db.Employes.ToList();
            var startOfMonth = new DateTime(currentdate.Year, currentdate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            startOfMonth = startOfMonth.AddDays(-4);
            endOfMonth = endOfMonth.AddDays(4);
            int lmonth = startOfMonth.Month;
            int lmonth1 = endOfMonth.Month;
            int lmonth2 = lmonth + 1;
            int sday = startOfMonth.Day;
            int eday = endOfMonth.Day;
            var lbirthdaysPrevMonth = db.Employes.ToList().Where(a => Convert.ToDateTime(a.DOB).Month == lmonth && Convert.ToDateTime(a.DOB).Day >= sday).Where(a => a.RetirementDate >= DateTime.Now).OrderBy(a => Convert.ToDateTime(a.DOB).Day).ToList();
            var lbirthdaysPrevMonth1 = db.Employes.ToList().Where(a => Convert.ToDateTime(a.DOB).Month == lmonth2).Where(a => a.RetirementDate >= DateTime.Now).OrderBy(a => Convert.ToDateTime(a.DOB).Day).ToList();
            var lbirthdaysPrevMonth2 = db.Employes.ToList().Where(a => Convert.ToDateTime(a.DOB).Month == lmonth1 && Convert.ToDateTime(a.DOB).Day <= eday).Where(a => a.RetirementDate >= DateTime.Now).OrderBy(a => Convert.ToDateTime(a.DOB).Day).ToList();
            var lcurbirthday = db.Employes.ToList().Where(a => Convert.ToDateTime(a.DOB).Month == month).Where(a => a.RetirementDate >= DateTime.Now).OrderBy(a => Convert.ToDateTime(a.DOB).Day).ToList();
            var lbirthdays = lbirthdaysPrevMonth2.Union(lcurbirthday);

            var lbirthdaysNext = lbirthdays.Union(lbirthdaysPrevMonth);
            var lbirthdaysnextnext = lbirthdaysNext.Union(lbirthdaysPrevMonth1);
            foreach (var items in lcurbirthday)
            {
                DateTime ldbmonth = Convert.ToDateTime(items.DOB);
                int ldobmonth = ldbmonth.Month;
                string sdate = ldbmonth.ToString("dd MMMM");

                lEmpbirthdays.Add(items.UploadPhoto);
                lEmpbirthdays.Add(items.ShortName);
                lEmpbirthdays.Add(sdate);
                lEmpbirthdaysss.Add(lEmpbirthdays.ToArray());
                lEmpbirthdays.Remove(sdate);
                lEmpbirthdays.Remove(items.ShortName);
                lEmpbirthdays.Remove(items.UploadPhoto);
            }

            var lnews = db.News.ToList().Where(a => Convert.ToDateTime(a.UpdatedDate.Date) == currentdate.Date).ToList();
            foreach (var item in lnews)
            {
                DateTime ldbdate = Convert.ToDateTime(item.UpdatedDate.Date);
                if (currentdate.Date == ldbdate)
                {
                    lNews.Add(item.Subject);
                    lNews.Add(item.Content);
                    lEmpNews.Add(lNews.ToArray());
                }
                lNews.Remove(item.Content);
                lNews.Remove(item.Subject);
            }

            var lretired = db.Employes.ToList().Where(a => Convert.ToDateTime(a.RetirementDate).Month == month).Where(a => a.Exit_type == "Retd").ToList();
            foreach (var item1 in lretired)
            {
                DateTime ldbmonth = Convert.ToDateTime(item1.RetirementDate);
                string cmonth1 = Convert.ToDateTime(item1.RetirementDate).ToString("MMMM yyyy");
                string sdate = ldbmonth.ToString("dd MMMM yyyy");
                if (cmonth1 == sMonth1)
                {
                    lEmpretirement.Add(item1.UploadPhoto);
                    lEmpretirement.Add(item1.ShortName);
                    lEmpretirement.Add(sdate);
                    lEmpRetirements.Add(lEmpretirement.ToArray());
                }
                lEmpretirement.Remove(sdate);
                lEmpretirement.Remove(item1.ShortName);
                lEmpretirement.Remove(item1.UploadPhoto);
            }

            var newleavestatus = new string[] { "Approved", "Pending", "Forwarded" }; // added by chaitanya on 12/05/2020

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
            foreach (var item in lresultleaves)
            {
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
            ViewBag.empid = lCredentails.EmpId;
            var lmodel = new DashBoard();
            lmodel.Birthdays = lEmpbirthdaysss.ToList();
            lmodel.News = lEmpNews.ToList();
            lmodel.Retirements = lEmpRetirements.ToList();
            lmodel.Leaves = lEmpLeaves.ToList();
            ViewBag.empid = lCredentails.EmpId;
            //System.Threading.Thread.Sleep(3000);
            var memoid = memocount();
            TempData["count"] = memoid;
            var empmemoid = empmemocount();
            TempData["Empcount"] = empmemoid;
            return View(lmodel);
        }


        public ActionResult EmployeeDashBoard()
        {
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            Session.SetDataToSession<string>("EmpFullName", lCredentails.EmpFullName);
            Session.SetDataToSession<string>("ActiveEmployee", lCredentails.EmpId);
            Session.SetDataToSession<string>("ActiveImage", lCredentails.EmpImage);
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
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
            var lbirthdaysPrevMonth = db.Employes.ToList().Where(a => Convert.ToDateTime(a.DOB).Month == lmonth && Convert.ToDateTime(a.DOB).Day >= sday).OrderBy(a => Convert.ToDateTime(a.DOB).Day).ToList();
            var lbirthdaysPrevMonth1 = db.Employes.ToList().Where(a => Convert.ToDateTime(a.DOB).Month == lmonth2).OrderBy(a => Convert.ToDateTime(a.DOB).Day).ToList();
            var lbirthdaysPrevMonth2 = db.Employes.ToList().Where(a => Convert.ToDateTime(a.DOB).Month == lmonth1 && Convert.ToDateTime(a.DOB).Day <= eday).OrderBy(a => Convert.ToDateTime(a.DOB).Day).ToList();

            var lbirthdays = lbirthdaysPrevMonth.Union(lbirthdaysPrevMonth1);

            var lbirthdaysNext = lbirthdays.Union(lbirthdaysPrevMonth2);

            foreach (var items in lbirthdaysNext)
            {
                DateTime ldbmonth = Convert.ToDateTime(items.DOB);
                string sdate = ldbmonth.ToString("dd MMMM dddd");
                lEmpbirthdays.Add(items.UploadPhoto);
                lEmpbirthdays.Add(items.ShortName);
                lEmpbirthdays.Add(sdate);
                lEmpbirthdaysss.Add(lEmpbirthdays.ToArray());
                lEmpbirthdays.Remove(sdate);
                lEmpbirthdays.Remove(items.ShortName);
                lEmpbirthdays.Remove(items.UploadPhoto);
            }

            var lnews = db.News.ToList().Where(a => Convert.ToDateTime(a.UpdatedDate.Date) == currentdate.Date).ToList();
            foreach (var item in lnews)
            {
                DateTime ldbdate = Convert.ToDateTime(item.UpdatedDate.Date);
                if (currentdate.Date == ldbdate)
                {
                    lNews.Add(item.Content);
                    lNews.Add(item.Subject);
                    lNews.Add(item.Notes);
                    lEmpNews.Add(lNews.ToArray());
                }
                lNews.Remove(item.Content);
                lNews.Remove(item.Subject);
                lNews.Remove(item.Notes);
            }

            var lretired = db.Employes.ToList().Where(a => Convert.ToDateTime(a.RetirementDate).Month == month).Where(a => a.Exit_type == "Retd").ToList();
            foreach (var item1 in lretired)
            {
                DateTime ldbmonth = Convert.ToDateTime(item1.RetirementDate);
                string cmonth1 = Convert.ToDateTime(item1.RetirementDate).ToString("MMMM yyyy");
                string sdate = ldbmonth.ToString("dd MMMM dddd");
                if (cmonth1 == sMonth1)
                {
                    lEmpretirement.Add(item1.UploadPhoto);
                    lEmpretirement.Add(item1.ShortName);
                    lEmpretirement.Add(sdate);
                    lEmpRetirements.Add(lEmpretirement.ToArray());
                }
                lEmpretirement.Remove(sdate);
                lEmpretirement.Remove(item1.ShortName);
                lEmpretirement.Remove(item1.UploadPhoto);
            }
            var lmodel = new DashBoard();
            lmodel.Birthdays = lEmpbirthdaysss.ToList();
            lmodel.News = lEmpNews.ToList();
            lmodel.Retirements = lEmpRetirements.ToList();
            var memoid = memocount();
            TempData["count"] = memoid;
            var empmemoid = empmemocount();
            TempData["Empcount"] = empmemoid;
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

        public void SetActiveEmployeeImage(string userName)
        {
            string value = Session.GetDataFromSession<string>("EmpFullName");
            if (string.IsNullOrEmpty(value))
            {
                // Get the emp id for the Supervisor
            }
            // Else part : not required as current employee is already set . 
            // It will be set in Employee selection.           

        }


        public void SetActiveEmployee(string userName)
        {
            string value = Session.GetDataFromSession<string>("ActiveEmployee");
            if (string.IsNullOrEmpty(value))
            {
                // Get the emp id for the Supervisor
            }
            // Else part : not required as current employee is already set . 
            // It will be set in Employee selection.           

        }

        public void SetActiveImage(string image)
        {
            string value = Session.GetDataFromSession<string>("ActiveImage");
            if (string.IsNullOrEmpty(value))
            {
                // Get the emp id for the Supervisor
            }
        }

        public string employeetemptransfer()
        {
            string notmatchedQryCreate = " Create table Employee_transfer_temp_branch (empid int);Create table Employee_transfer_temp_department (empid int);Create table Employee_transfer_temp_designation (empid int);";
            sh.Run_UPDDEL_ExecuteNonQuery(notmatchedQryCreate);
            string notmachedQryCount = "exec sp_emptransfer_deviation;";
            bool result = sh.Run_UPDDEL_ExecuteNonQuery(notmachedQryCount);
            string queryselect = "select count(*) as cntOfEmployee from Employee_transfer_temp_designation " +
                "union select count(*) as cntOfEmployee from Employee_transfer_temp_branch union " +
                "select count(*) as cntOfEmployee from Employee_transfer_temp_department";
            DataTable dt = sh.Get_Table_FromQry(queryselect);
            string droptemp = "Drop table Employee_transfer_temp_branch; Drop table Employee_transfer_temp_department;Drop table Employee_transfer_temp_designation;";
            sh.Run_UPDDEL_ExecuteNonQuery(droptemp);
            return JsonConvert.SerializeObject(dt);

        }

        public int memocount()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var latememo = db.Latememo.ToList();
            //TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            //TempData.Keep();
            var EmpId = lCredentials.EmpId;
            var lUserLoginId = EmpId;
            string currentDate = DateTime.UtcNow.ToString("yyyy-MM-dd 00:00:00");
            var lResult = (from memolist in latememo
                           where memolist.Empid == lUserLoginId
                           && memolist.Status == "Pending" && memolist.Duedate >= DateTime.Parse(currentDate)
                           select memolist).Count();
            var lresponseArray = lResult;
            //var lresponseArray = lResult;
            TempData["count"] = lresponseArray;
            return lresponseArray;
            //TempData["latememos"] = lresponseArray.ToList();
        }
        public int empmemocount()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var latememo = db.Latememo.ToList();
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var EmpId = lCredentials.EmpId;
            var lUserLoginId = EmpId;
            var controlling = db.Employes.Where(a => a.EmpId == EmpId).Select(a => a.Id).FirstOrDefault();
            var contrlid = controlling;
            string currentDate = DateTime.UtcNow.ToString("yyyy-MM-dd 00:00:00");
            var lResult = (from memolist in latememo
                           where memolist.controllingauthority == contrlid.ToString()
                             && memolist.MemoType == "Late Applied Leave"
                           && memolist.Status == "Pending" && memolist.Duedate >= DateTime.Parse(currentDate)
                           select memolist).Count();
            var emplresponseArray = lResult;
            if (lResult != null)
            {
                TempData["Empcount"] = emplresponseArray;
            }

            return emplresponseArray;
        }

        private DateTime getdate()
        {
            throw new NotImplementedException();
        }
    }
}
