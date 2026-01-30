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
using System.Globalization;
using HRMSApplication.Filters;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class HolidayListsController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        // GET: HolidayLists/Create
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        [HttpGet]
        public ActionResult Create()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/HolidayLists/_HolidayPartialView.cshtml");
        }


        private bool NthDayOfMonth(DateTime date, DayOfWeek dow, int n)
        {
            int d = date.Day;
            return date.DayOfWeek == dow && (d - 1) / 7 == (n - 1);
        }
        public void getsundays()
        {
            DateTime startDate = new DateTime(2017, 1, 1);
            DateTime endDate = new DateTime(2019, 12, 31);

            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {
                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {

                    case DayOfWeek.Saturday:

                        if (DayOfWeek.Saturday == testDate.DayOfWeek)
                        {
                            bool SecsatDay = NthDayOfMonth(testDate, DayOfWeek.Saturday, 2);

                            bool FoursatDay = NthDayOfMonth(testDate, DayOfWeek.Saturday, 4);


                            if (SecsatDay == true || FoursatDay == true)
                            {
                                HolidayList holidayListForSat = new HolidayList();
                                LoginCredential lCredentialsForSat = LoginHelper.GetCurrentUser();
                                int IdForSat = db.Employes.Where(a => a.EmpId == lCredentialsForSat.EmpId).Select(a => a.Id).FirstOrDefault();
                                holidayListForSat.UpdatedBy = IdForSat;
                                holidayListForSat.UpdateDate = DateTime.Now;
                                DateTime? DeleteAtForSat = Convert.ToDateTime("1900-01-01 00:00:00.000");
                                holidayListForSat.DeleteAt = DeleteAtForSat.Value;
                                holidayListForSat.Date = testDate;
                                if (SecsatDay == true)
                                {
                                    holidayListForSat.Occasion = "Second Saturday";

                                }
                                if (FoursatDay == true)
                                {
                                    holidayListForSat.Occasion = "Fourth Saturday";

                                }

                                db.HolidayList.Add(holidayListForSat);
                                db.SaveChanges();
                            }

                        }
                        break;

                    case DayOfWeek.Sunday:

                        HolidayList holidayList = new HolidayList();

                        
                        int Id = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                        holidayList.UpdatedBy = Id;
                        holidayList.UpdateDate = DateTime.Now;
                        DateTime? DeleteAt = Convert.ToDateTime("1900-01-01 00:00:00.000");
                        holidayList.DeleteAt = DeleteAt.Value;
                        holidayList.Date = testDate;
                        holidayList.Occasion = "Sunday";
                        db.HolidayList.Add(holidayList);
                        db.SaveChanges();

                        break;



                }
            }

        }
        public JsonResult checkholiday(string Date)
        {
            DateTime star1 = DateTime.Parse(Date);
            DateTime? DeleteAt = Convert.ToDateTime("1900-01-01");
           // var holiday = from ba in db.HolidayList where ba.Date.Equals(star1) select ba;
            var holiday = db.HolidayList.Where(a => a.Date == star1).Where(a => a.DeleteAt == DeleteAt).Select( a=> a. Date);
            int count = holiday.Count();
        

            if (count == 0)
            {
                return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: HolidayLists/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(HolidayList holidayList)
        {
            // uncomment this line to get saturdays and sundays based on the year
           // getsundays();
            
            if (holidayList != null)
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int Id = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                holidayList.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                holidayList.UpdateDate = DateTime.Now;
                //string sourceDateText = holidayList.Date.ToString("dd/MM/yyyy");
                //DateTime sourceDate = DateTime.Parse(sourceDateText);
                // string formatted = sourceDate.ToString("yyyy-MM-dd");          
                DateTime? DeleteAt = Convert.ToDateTime("1900-01-01");
                holidayList.DeleteAt = DeleteAt.Value;
                db.HolidayList.Add(holidayList);
                db.SaveChanges();
                TempData["AlertMessage"] = "Holiday Created Successfully";
                return RedirectToAction("Create");
            }
            return View(holidayList);
        }

        [NoDirectAccess]
        public ActionResult HolidayView()
        {
            return View();
        }
        [HttpGet]
        public JsonResult HolidayViews(string Occasion)
        {
            LogInformation.Info("Holidaylist grid load Started");
            try
            {
                var dbResult = db.HolidayList.ToList();


                if (string.IsNullOrEmpty(Occasion))
                {
                    string basedate = "1900-01-01 12:00:00.000";
                    DateTime ccDadte = DateTime.Parse(basedate);
                    var holiday = (from HolidayList in dbResult
                                   where HolidayList.DeleteAt == ccDadte.Date

                                   select new
                                   {
                                       HolidayList.Id,
                                       HolidayList.Occasion,
                                       HolidayList.Date,
                                       HolidayList.UpdateDate,
                                       HolidayList.DeleteAt,
                                       HolidayList.UpdatedBy,
                                   });
                    holiday = holiday.OrderBy(x => x.Date);
                    LogInformation.Info("Holidaylist grid End");
                    return Json(holiday, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var holiday = (from HolidayList in dbResult
                                   select new
                                   {
                                       HolidayList.Id,
                                       HolidayList.Occasion,
                                       HolidayList.Date,
                                       HolidayList.UpdateDate,
                                       HolidayList.DeleteAt,
                                       HolidayList.UpdatedBy,
                                   });
                    return Json(holiday, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception e)
            {
                e.ToString();
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        // GET: HolidayLists/Edit/5
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HolidayList holidayList = db.HolidayList.Find(id);

            if (holidayList == null)
            {
                return HttpNotFound();
            }
            string dateforView = holidayList.Date.ToString("dd-MM-yyyy");
            TempData["WantedDate"] = dateforView;
            return View(holidayList);
        }
        // POST: HolidayLists/Edit/5       
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(HolidayList holidayList)
        {
            if (holidayList != null)
            {

                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int Id = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                holidayList.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                DateTime? DeleteAt = Convert.ToDateTime("1900-01-01");
                holidayList.UpdateDate = DateTime.Now;
                holidayList.DeleteAt = DeleteAt.Value;
                db.Entry(holidayList).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Holiday Updated Successfully";
                return RedirectToAction("Create");
            }
            return View(holidayList);
        }

        // GET: HolidayLists/Delete/5
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HolidayList holiday = db.HolidayList.Find(id);
            if (holiday != null)
            {
                HolidayList holidaylist = holiday;
                holidaylist.DeleteAt = DateTime.Now;
                db.HolidayList.Remove(holiday);
                db.SaveChanges();
                TempData["AlertMessage"] = "Holiday Details deleted Successfully.";
                return RedirectToAction("Create");
            }
            else
            {
                TempData["AlertMessage"] = "cannot be deleted.";
                return RedirectToAction("Create");
            }
        }
    }
}
