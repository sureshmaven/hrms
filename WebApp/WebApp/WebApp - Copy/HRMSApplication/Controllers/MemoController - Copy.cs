using Entities;
using HRMSApplication.Helpers;
using HRMSApplication.Models;
using HRMSBusiness.Business;
using HRMSBusiness.Timesheet;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRMSBusiness.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;
using System.Data;
using Mavensoft.DAL.Db;
using Newtonsoft.Json;

namespace HRMSApplication.Controllers
{

    public class MemoController : Controller
    {
        private ContextBase db = new ContextBase();

        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        TimesheetBusiness Tbus = new TimesheetBusiness();
        SqlHelper sh = new SqlHelper();

        // GET: Memo
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MemoCreate()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();

        }
       


        [HttpGet]
        public JsonResult GetEmployeeData(string empcode)
        {
            string branchs = "";
            //string totalexperience = "";
            string lresult = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                var employees = db.Employes.ToList();
                int count = db.Employes.Where(a => a.EmpId == empcode).Count();
                var lshortname = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.ShortName).FirstOrDefault();
                var ldesignation = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.CurrentDesignation).FirstOrDefault();
                string desig = db.Designations.Where(a => a.Id == ldesignation).Select(a => a.Name).FirstOrDefault();
                int branch = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Branch).FirstOrDefault();
                string branchss = db.Branches.Where(a => a.Id == branch).Select(a => a.Name).FirstOrDefault();
                // string controlling=db.Employes.Where(a => a.EmpId == empcode).Select(a => a.ControllingAuthority).FirstOrDefault();
                string lca = db.Employes.Where(a => a.Role == 1 && a.CurrentDesignation == 4 && a.Department == 16).Select(a => a.ShortName).FirstOrDefault();
                // var lca = db.Employes.Where(a => a.EmpId == controlling).Select(a => a.ShortName).FirstOrDefault();
                int dept = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Department).FirstOrDefault();
                string depts = db.Departments.Where(a => a.Id == dept).Select(a => a.Name).FirstOrDefault();

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
                                   userslist.SanctioningAuthority,
                               });
                var lresponseArray = lResult.ToArray();


                string lSanctioningAuthority = lresponseArray[0].SanctioningAuthority;
                //int lcontrol = Convert.ToInt32(lControllingAuthority);
                int lsancationcontrol = Convert.ToInt32(lSanctioningAuthority);
                //Session["lcontrols"] = lcontrol;
                Session["lSancation"] = lsancationcontrol;
                // Employees lcontrolling = Facade.EntitiesFacade.GetEmpTabledata.GetById(lcontrol);
                Employees lsancationing = Facade.EntitiesFacade.GetEmpTabledata.GetById(lsancationcontrol);


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

                return Json(new { lshortnmaeAjax = lshortname, ldesigAjax = desig, lbranchAjax = branchs, lSanctioningAuthorityAjax = lsancationing.ShortName, Status = lstatus1 }, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public ActionResult MemoCreate(Latememo latememo,string MemoType)
        {
            string EmpId = lCredentials.EmpId;
            var Empname= db.Employes.Where(a => a.EmpId == latememo.Empid).Select(a => a.LastName).FirstOrDefault();
            //var empname = db.Employes.Where(a => a.EmpId == EmpId).Select(a => a.LastName).FirstOrDefault();
            var Noofdays = latememo.Noofdays;
            var clarify = "";
            //DateTime date = ;
            latememo.Clarification = clarify;
            var memodetails = latememo.Memodetails;
            //var issuedby = +Convert.ToInt32(EmpId) + "." + empname + ".";
            latememo.Issueby = Convert.ToInt32(EmpId);
            //latememo.IssueDate = Getdate();
            var dateAndTime = DateTime.Now;
            var date = dateAndTime.Date;
            latememo.IssueDate = date;
            latememo.MemoType = latememo.MemoType;
            latememo.Duedate = latememo.Duedate;
            //var Priornoticegivendays = latememo.Priornoticegivendays;
            var reason = "";
            //latememo.ReasonForLeave = reason;
            //latememo.Leaveapplieddate = latememo.Leaveapplieddate;
            //latememo.leavetype = latememo.leavetype;
            //latememo.Responsedate = null;
            latememo.Status = "Pending";
            //latememo.Action = "Active";
            db.Latememo.Add(latememo);
            db.SaveChanges();
            TempData["AlertMessage"] = "Memo Created Successfully";
            return RedirectToAction("MemoCreate");
        }
        public ActionResult EmployeeClarification()
        {
            return View();
        }
        public ActionResult EmployeeMemoEdit()
        {
            return View();
        }
        [HttpGet]
        public ActionResult MemoReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var latememo = db.Latememo.ToList();
            //TempData["RolePages"] = LoginHelper.GetCurrentUserPages();

            //TempData.Keep();
            var EmpId = lCredentials.EmpId;
            var lUserLoginId = EmpId;

            var lResult = (from memolist in latememo
                           where memolist.Empid == lUserLoginId 
                           && memolist.Status == "Pending"
                           select memolist);
            var lresponseArray = lResult;
            TempData["latememos"] = lresponseArray.ToList();
            //TempData["latememos"] = latememo;



            TempData.Keep();

            return View();

        }
        [HttpPost]
        public JsonResult MemoReport(Latememo latememo, int Id, string Clarification)
        {
            string lMessage = string.Empty;
            try
            {
                Latememo latememo1 = db.Latememo.Where(a => a.Id == Id).FirstOrDefault();
                latememo1.Id = Id;
                latememo1.Responsedate = DateTime.Now;
                latememo1.Status = "Complete";
                var Clarify = latememo.Clarification;
                latememo1.Clarification = Clarify;
                db.Entry(latememo1).State = EntityState.Modified;
                db.SaveChanges();
                //TempData["AlertMessage1"] = "Latememo Clarification Updated Successfully";
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            //return null;
            string id = "jjjj";
            return Json(id, JsonRequestBehavior.AllowGet);



        }

        [HttpPost]
        public JsonResult CancelMemo(Latememo latememo, int Id)
        {
            string lMessage = string.Empty;
            try
            {
                Latememo latememo1 = db.Latememo.Where(a => a.Id == Id).FirstOrDefault();
                latememo1.Id = Id;
                latememo1.Status = "Cancelled";                
                db.Entry(latememo1).State = EntityState.Modified;
                db.SaveChanges();
                
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            string id = "jjjj";
            return Json(id, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DisplayReport()
        {
            //var latememo = db.Latememo.ToList();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            TempData.Keep();
            TempData["latememos"] = db.Latememo.ToList();
            TempData.Keep();
            //var datademo = TempData["latememos"];
            return View();
        }


        [HttpGet]
        public ActionResult MemoList()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            int lId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpid = db.Employes.Where(a => a.Id == lId).Select(a => a.EmpId).FirstOrDefault();
            var empcontrol = db.View_ChangingAuthority.Select(a => a.ControllingEmpId).Distinct().ToList();
            var empsanction = db.View_ChangingAuthority.Select(a => a.SanctioningEmpId).Distinct().ToList();
            TempData["RolePages"] = lCredentials.RolePages;
            ViewData["Control"] = empcontrol;
            ViewData["Sanction"] = empsanction;
            TempData["Empid"] = lCredentials.EmpId;
            int lControllingAuthority = db.View_ChangingAuthority.Where(a => a.ControllingEmpId == lEmpid).Count();
            string lSancationingAuthority = db.View_ChangingAuthority.Where(a => a.SanctioningAuthority == lEmpid).Select(a => a.SanctioningAuthority).FirstOrDefault();
            ViewBag.ExportColumns = "columns: [0, 1,2,3,4, 5, 6, 7, 8, 9, 10, 11, 12]";
            ViewBag.pageSize = "LEGAL";
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            return View("/Views/Memo/MemoList.cshtml");
        }

        [HttpGet]
        public JsonResult MemoListData()
        {
            var latememo = db.Latememo.ToList();
            //var employees = db.Employes.ToList();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            TempData.Keep();
            var EmpId = lCredentials.EmpId;
            var lUserLoginId = EmpId;
            //var dt = new SqlHelper().Get_Table_FromQry("Select  * from Latememo   UNION Select LastName from Employees ");

            var lResult = (from memolist in latememo
                           join st in db.Employes
                           on memolist.Empid equals st.EmpId
                           join br in db.Branches
                           on st.Branch equals br.Id
                           join dg in db.Designations
                           on st.CurrentDesignation equals dg.Id
                           join dept in db.Departments on st.Department equals dept.Id 
                           where memolist.Status!="Cancelled"
                           select new
                           {

                               memolist.Id,
                               memolist.Empid,
                               //memolist.EmpName,
                               st.ShortName,
                               dg.Code,
                               //case when br.Name = 'OtherBranch' then dept.Name else b.Name 
                               br.Name,
                               dept.Description,
                               //dept.Name,
                               memolist.IssueDate,
                               memolist.Duedate,
                               memolist.Memodetails,
                               memolist.Noofdays,
                               memolist.MemoType,
                               memolist.Clarification,
                               memolist.Responsedate,
                               memolist.Issueby,
                               memolist.Status,
                               //memolist.Action,

                           });
            var lresponseArray = lResult;
            return Json(lResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Employeememoreport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var latememo = db.Latememo.ToList();
            //TempData["RolePages"] = LoginHelper.GetCurrentUserPages();

            //TempData.Keep();
            var EmpId = lCredentials.EmpId;
            var lUserLoginId = EmpId;

            var lResult = (from memolist in latememo
                           where memolist.Empid == lUserLoginId
                           && memolist.Status == "Complete"
                           select memolist);
            var lresponseArray = lResult;
            TempData["latememos"] = lresponseArray.ToList();
            //TempData["latememos"] = latememo;



            TempData.Keep();

            return View();
        }

        [HttpGet]
        public string MemoTooltip(string MemoId)
        {
			
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getMemoToolTip(MemoId);
            return JsonConvert.SerializeObject(dt);
        }

        ////MemoEdit

        public ActionResult Edit(int? id)
        {
            ViewBag.id = id;
            Latememo lmemo = db.Latememo.Find(id);
            string empcode = db.Employes.Where(a => a.EmpId == lmemo.Empid).Select(a => a.EmpId).FirstOrDefault();
            ViewBag.empcode = empcode;
            var lshortname = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.ShortName).FirstOrDefault();
            ViewBag.shortname = lshortname;
            var ldesignation = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.CurrentDesignation).FirstOrDefault();

            string desig = db.Designations.Where(a => a.Id == ldesignation).Select(a => a.Name).FirstOrDefault();
            ViewBag.designation = desig;
            int branch = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Branch).FirstOrDefault();
            string branchss = db.Branches.Where(a => a.Id == branch).Select(a => a.Name).FirstOrDefault();
            int dept = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Department).FirstOrDefault();
            string depts = db.Departments.Where(a => a.Id == dept).Select(a => a.Name).FirstOrDefault();
            ViewBag.branch = branchss;
            string controlling = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.SanctioningAuthority).FirstOrDefault();
            string sanction = db.Employes.Where(a => a.Id.ToString() == controlling).Select(a => a.ShortName).FirstOrDefault();
            string memotype = db.Latememo.Where(a => a.Id == id).Select(a => a.MemoType).FirstOrDefault();
            ViewBag.memotype = memotype;
            ViewBag.SA = sanction;
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View(lmemo);
        }

        [HttpPost]
        public JsonResult Edit(Latememo latememo,int Id,string memotype,int noofdays,string duedate,string memodetails)
        {

            string lMessage = string.Empty;
            Latememo latememo1 = db.Latememo.Where(a => a.Id == Id).FirstOrDefault();
            try
            {
                
                latememo1.Id = Id;
                latememo1.Noofdays = noofdays;
                latememo1.Duedate = Convert.ToDateTime(duedate);
                latememo1.MemoType = memotype;
                latememo1.Memodetails = memodetails;
                db.Entry(latememo1).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            //string id = "jjjj";
            return Json(latememo1);
        }

        public ActionResult Latememo(string MemoType, int memoid)
        {
            var EmpId = lCredentials.EmpId;
            ViewBag.EmpCode = EmpId;
            ViewBag.MemoType = MemoType;
            ViewBag.memoid = memoid;
            var memodata = db.Latememo.Where(a => a.Id == memoid).Select(a => a).FirstOrDefault();
            ViewBag.Date = DateTime.Now;
            ViewBag.Month = ViewBag.Date.ToString("MMMM");
            var IssueDate = memodata.IssueDate;
            var month = IssueDate.Month.ToString();
            var date = IssueDate.ToShortDateString();
            ViewBag.IssueDate = date;
            ViewBag.Memodetails = memodata.Memodetails;
            ViewBag.Noofdays = memodata.Noofdays;
            ViewBag.IssueDate = memodata.IssueDate;
            ViewBag.Duedate = memodata.Duedate;
            ViewBag.Clarification = memodata.Clarification;
            ViewBag.Responsedate = memodata.Responsedate;
            ViewBag.EmpName = db.Employes.Where(a => a.EmpId == EmpId).Select(a => a.ShortName).FirstOrDefault();
            int branch = db.Employes.Where(a => a.EmpId == EmpId).Select(a => a.Branch).FirstOrDefault();
            string branchs = db.Branches.Where(a => a.Id == branch).Select(a => a.Name).FirstOrDefault();
            int dept = db.Employes.Where(a => a.EmpId == EmpId).Select(a => a.Department).FirstOrDefault();
            string depts = db.Departments.Where(a => a.Id == dept).Select(a => a.Name).FirstOrDefault();
            string photo = db.Employes.Where(a => a.EmpId == EmpId).Select(a => a.UploadPhoto).FirstOrDefault();
            int desig = db.Employes.Where(a => a.EmpId == EmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
            string Designation = db.Designations.Where(a => a.Id == desig).Select(a => a.Name).FirstOrDefault();
            if (branch == 43)
            {
                ViewBag.branchname = depts;
            }
            else
            {
                ViewBag.branchname = branchs;
            }
            ViewBag.Designation = Designation;
            string months = DateTime.Now.Month.ToString();
            string year = DateTime.Now.Month.ToString();
            //ViewBag.Month = months;
            ViewBag.Year = year;
            var dt = Tbus.EmpMonthlyLateMemoTimesheet1(months, year, EmpId);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewBag.ShiftStartTime = dr["BranchTimings"];
                    ViewBag.TotalLateComes = dr["TotalLateComes"];
                    var days = dr["InTimeOutTime"].ToString().Replace("<br/>", " , ");
                    if (days.StartsWith(" ,"))
                    {
                        string days1 = days.ToString().Substring(1);
                        ViewBag.DayinOut = days1.Substring(1);
                    }
                    else
                    {
                        ViewBag.DayinOut = days;
                    }
                }
            }
            if (MemoType == "Late memo")
            {
                return View("/Views/Memo/PdfMemos.cshtml");

            }
            else if (MemoType == "Absent memo")
            {
                return View("/Views/Memo/AbsentMemo.cshtml");
            }
            else if (MemoType == "Non updation of work dairy")
            {
                return View("/Views/Memo/WorkDairyMemo.cshtml");
            }
            else if (MemoType == "Non submission of medical certificate")
            {
                return View("/Views/Memo/MedicalMemo.cshtml");
            }
            return View();
        }
    }
}

