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
using HRMSApplication.Helpers;
using HRMSApplication.Models;
using HRMSBusiness.Business;
using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using HRMSBusiness.Reports;
using System.Web.UI.WebControls;
using System.Web.UI;
using HRMSBusiness.Db;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class WorkDiaryController : Controller
    {
        //ODHelper lhelper = new ODHelper();
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        [HttpGet]
        // GET: WorkDairies
        public ActionResult Index()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
           
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            var lControllingAuthority = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
            var lSancationingAuthority = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
            int lcId = Convert.ToInt32(lControllingAuthority);
            int lsId = Convert.ToInt32(lSancationingAuthority);
            string lControllingName = db.Employes.Where(a => a.Id == lcId).Select(a => a.ShortName).FirstOrDefault();
            string lSancationingName = db.Employes.Where(a => a.Id == lsId).Select(a => a.ShortName).FirstOrDefault();
            ViewBag.Controlid = lControllingAuthority;
            ViewBag.Sanctionid = lSancationingAuthority;
            ViewBag.Controlling = lControllingName;
            ViewBag.Sancationing = lSancationingName;
            ViewBag.Message = lCredentials.LoginMode;
            TempData["Loginmode"] = lCredentials.LoginMode;
  
            var items4 = Facade.EntitiesFacade.GetworkmasterAll().Where(a=>a.Name!="LTC").Where(a=>a.Name!= "Leave").Where(a => a.Name != "OD").OrderBy(a => a.Name).Select(x => new All_Masters
            {
                Id = x.Id,
                Name = x.Name,
            }).OrderBy(a=>a.Id);

            ViewBag.worklist = new SelectList(items4, "Id", "Name");
            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from All_Masters");
            var items5 = dt.AsEnumerable().Select(r => new All_Masters
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();
            items5.Insert(0, new All_Masters
            {
                Id = 0,
                Name = "ALL"
            });
            ViewBag.worklist1 = new SelectList(items5, "Id", "Name");
            var empcontrol = db.View_ChangingAuthority.Select(a => a.ControllingEmpId).Distinct().ToList();
            var empsanction = db.View_ChangingAuthority.Select(a => a.SanctioningEmpId).Distinct().ToList();
            ViewData["Control"] = empcontrol;
            ViewData["Sanction"] = empsanction;
            TempData["Empid"] = lCredentials.EmpId;
            var lmodel = new WorkDiary { Loginmode = lCredentials.LoginMode };
            return View(lmodel);
        }
        [HttpGet]
        public JsonResult Edit(int id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            WorkDiaryBus wdbus = new WorkDiaryBus();
            ArrayList rows = new ArrayList();
            var dt = wdbus.EditWorkDiary(id);
            foreach (DataRow dataRow in dt.Rows)
                rows.Add(string.Join(";", dataRow.ItemArray.Select(item => item.ToString())));
            return Json(new { lrows = rows }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit(WorkDiary workdiary, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string lcontrol = form["control"];
                string lsanction = form["sanction"];
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var lempid = db.Employes.Where(a => a.Id == lId).Select(a => a.EmpId).FirstOrDefault();
                workdiary.UpdatedBy = Convert.ToInt32(lCredentials.EmpId);
                workdiary.UpdatedDate = DateTime.Now;
                var lControllingAuthority = db.Employes.Where(a => a.Id == lId).Select(a => a.ControllingAuthority).FirstOrDefault();
                var lSancationingAuthority = db.Employes.Where(a => a.Id == lId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                int lcId = Convert.ToInt32(lControllingAuthority);
                int lsId = Convert.ToInt32(lSancationingAuthority);
                var lcontrolempid = db.Employes.Where(a => a.Id == lcId).Select(a => a.EmpId).FirstOrDefault();
                var lsanctionempid = db.Employes.Where(a => a.Id == lsId).Select(a => a.EmpId).FirstOrDefault();
                workdiary.CA = Convert.ToInt32(lcontrolempid);
                workdiary.SA = Convert.ToInt32(lsanctionempid);
                workdiary.EmpId = Convert.ToInt32(lempid);
                workdiary.WDDate = workdiary.WDDate;
                workdiary.Loginmode = lCredentials.LoginMode;
                workdiary.Status = "Pending";
                workdiary.Id = lId;
                db.Entry(workdiary).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Workdiary Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(workdiary);
        }
        [HttpGet]
        public JsonResult workdetails()
        {
           
            int lEmpId = db.Employes.Where(a => a.EmpId.ToLower() == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();

            var employees = db.Employes.ToList();
            var lbranches = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var ldesignations = db.Designations.ToList();
            var lResult = (from userslist in employees
                           join branchlist in lbranches on userslist.Branch equals branchlist.Id
                           join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                           join dept in ldepartments on userslist.Department equals dept.Id
                           where userslist.Id == lEmpId
                           select new
                           {
                               userslist.EmpId,
                               EmployeeName = userslist.ShortName,
                               Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                               desig.Name,
                               userslist.ControllingAuthority,
                               userslist.SanctioningAuthority


                           });
            var lresponseArray = lResult.ToArray();
            string employeeId = lresponseArray[0].EmpId;
            string employeeName = lresponseArray[0].EmployeeName;
            string Deptbranchs = lresponseArray[0].Deptbranch;
            string ldesignation = lresponseArray[0].Name;
            string lControllingAuthority = lresponseArray[0].ControllingAuthority;
            string lSanctioningAuthority = lresponseArray[0].SanctioningAuthority;
            int lcontrol = Convert.ToInt32(lControllingAuthority);
            int lsancationcontrol = Convert.ToInt32(lSanctioningAuthority);
            Session["lcontrols"] = lcontrol;
            Session["lSancation"] = lsancationcontrol;
            Employees lcontrolling = Facade.EntitiesFacade.GetEmpTabledata.GetById(lcontrol);
            Employees lsancationing = Facade.EntitiesFacade.GetEmpTabledata.GetById(lsancationcontrol);
            return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation, lControllingAuthorityAjax = lcontrolling.ShortName, lSanctioningAuthorityAjax = lsancationing.ShortName }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Workdiarypost(WorkDiary workdiary, WDViewModel WD, FormCollection form)
        {
            DateTime? Retirement = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.RetirementDate).FirstOrDefault();
            DateTime? Workdairydate;
            if (WD.wddelete=="Delete")
            {
                Workdairydate = db.WorkDiary.Where(a => a.Id == WD.WDId).Select(a => a.WDDate).FirstOrDefault();
            }
            else
            {
                Workdairydate = workdiary.WDDate;
            }
            //if (Retirement >= workdiary.WDDate)
            if (Retirement >= Workdairydate)
            {
                if (Request.Form["draft"] != null)
                {
                    WD.draft = "Draft";
                    // Code for function 1
                }
                else if (Request.Form["submitt"] != null)
                {
                    // code for function 2
                }
                WorkDiaryBus wdbus = new WorkDiaryBus();
                var ControlFormId = form["controlid"];
                var SanctionForId = form["sanctionid"];

                var Othervalue = form["others"];

                WorkDiary_Det lworktype = new WorkDiary_Det();
                for (int i = 0; i < WD.workdes.Length; i++)
                {
                   // lworktype.WorkType = WD.workname.GetValue(i).ToString();
                    //if (WD.workname.GetValue(i).ToString() == "Others")
                    //{

                    //    ((string[])WD.workname)[i] = ((string[])WD.others)[i].ToString();
                    //}
                    if (WD.workdes.GetValue(i).ToString().Contains("'"))
                    {
                        ((string[])WD.workdes)[i] = ((string[])WD.workdes)[i].ToString().Replace("'", "`");
                    }
                    //if (WD.workname.GetValue(i).ToString().Contains("'"))
                    //{
                    //    ((string[])WD.workname)[i] = ((string[])WD.workname)[i].ToString().Replace("'", "`");
                    //}
                    //if (WD.workname.GetValue(i).ToString().Contains("&"))
                    //{
                    //    ((string[])WD.workname)[i] = ((string[])WD.workname)[i].ToString().Replace("&", "and");
                    //}

                }
                int? lbranch = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Branch).FirstOrDefault();
                int? ldept = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Department).FirstOrDefault();
                int? ldesig = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
                //int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();


                TempData["AlertMessage"] = wdbus.addWorkDiary(workdiary, WD, LoginHelper.GetCurrentUser().EmpId, ControlFormId, SanctionForId, (int)lbranch, (int)ldept, (int)ldesig);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["AlertMessage"] = "The Selected Dates should be less than or equal to the Retirement Date" + "  " + Retirement.Value.ToShortDateString() + "  " + "Please select other dates.";
                return RedirectToAction("Index");
            }
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
        [HttpGet]
        [Route("allworkdiaries")]
        public DataTable WorkdiaryViewsNew()
        {

            // var a = HttpContext.Current.Session["EmpId"].ToString();
           
            WorkDiaryBus wdbus = new WorkDiaryBus();
            var dtwd = wdbus.getAllWorkDiaries(lCredentials.EmpId);
            return dtwd;
            //return Json(dtwd, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult WorkApprovalAdmin()
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
            return View("/Views/WorkDiary/WorkApprovalAdmin.cshtml");
        }
        [HttpGet]
        public ActionResult WorkApproval()
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
                return View("/Views/WorkDiary/WorkApproval.cshtml");
        }
        [HttpGet]
        public ActionResult SanctionWorkApproval()
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
            int lSancationingAuthority = db.View_ChangingAuthority.Where(a => a.SanctioningEmpId == lEmpid).Count();
            return View("/Views/WorkDiary/SanctionWorkApproval.cshtml");

        }
        public string GetsDates(DateTime ldate)
        {
            string ldates = "";
            DateTime d1 = ldate;
            ldates = d1.ToShortDateString().ToString() + " - " + d1.ToShortTimeString().ToString();
            return ldates;
        }
        //public JsonResult checkWorkDiary(string Date)
        //{
        //    string status = "";
        //    LoginCredential lcredentials = LoginHelper.GetCurrentUser();
        //    DateTime star1 = DateTime.Parse(Date);
        //    string basedate = "1900-01-01 00:00:00.000";
        //    DateTime ccDate = DateTime.Parse(basedate);
        //    string sqlDate = star1.ToString("yyyy-MM-dd HH:mm:ss.fff");
        //    WorkDiaryBus wdbus = new WorkDiaryBus();
        //    var count = wdbus.GetHoliday(sqlDate, basedate);

        //    if (count == 0)
        //    {
        //        status = "countzero";
        //        return Json(new { message = "status" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        status = "false/" + sqlDate + "--WorkDiary Cannot Applied on Holidays";
        //        return Json(new { message = "status" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public JsonResult checkWorkDiaryDates(string Date)
        //{
        //    string status = "";
        //    DateTime star1 = DateTime.Parse(Date);
        //    LoginCredential lCredentails = LoginHelper.GetCurrentUser();
        //    string lDate = star1.ToString("yyyy-MM-dd");
        //    WorkDiaryBus wdbus = new WorkDiaryBus();
        //    int count = wdbus.CheckWorkDairyDates(lDate, lCredentails.EmpId);
        //    if (count == 0)
        //    {
        //        status = "countzero";
        //        return Json(new { message = status }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        status = "false/" + lDate + "--Already WorkDiary applied in these dates.";
        //        return Json(new { message = status }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public JsonResult CheckWorkDiary(string Date)
        {
            string status = "";
            LoginCredential lcredentials = LoginHelper.GetCurrentUser();
            DateTime star1 = DateTime.Parse(Date);
            string basedate = "1900-01-01 00:00:00.000";
            string lDate = star1.ToString("yyyy-MM-dd");
            WorkDiaryBus wdbus = new WorkDiaryBus();
            var count = wdbus.GetHoliday(lDate, basedate);


            var lcount1 = wdbus.CheckWorkDairyDate(lDate, lcredentials.EmpId, lcredentials.EmpPkId);
            
            // wd status 
            string wd_status = wdbus.CheckWorkDairyDates(lDate, lcredentials.EmpId, lcredentials.EmpPkId);
            if (wd_status == "Draft")
            {
                //string dt = wdbus.WorkDescription(lDate,lcredentials.EmpId);
                string workDescription = wdbus.WorkDescription(lDate, lcredentials.EmpId);

                ViewBag.WorkDesc = workDescription;
                string status1 = string.IsNullOrEmpty(workDescription) ? "" : "Draft"; // Example logic to check if it's a draft

                return Json(new { message = $"{status1}. {workDescription}" }, JsonRequestBehavior.AllowGet);

                // status = "false/" + "Please Check Your Work Diary is in Draft ";
                //return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (lcount1 != "" && lcount1.Contains("WD"))
                {
                    status = "false/" + "Please Check the date range already applied in  WD ";
                    return Json(new { message = status }, JsonRequestBehavior.AllowGet);
                }

                else if (lcount1 != "" && !lcount1.Contains("OD"))
                {
                    status = "false/" + "Please Check the date range already applied in  " + lcount1;
                    return Json(new { message = status }, JsonRequestBehavior.AllowGet);
                }
            }            
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("allworkdiaries")]
        public string allWorkDiaries()
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus wdbus = new WorkDiaryBus();
            var dt = wdbus.getAllWorkDiaries(lCredentials.EmpId);
            return JsonConvert.SerializeObject(dt);
        }
        [HttpGet]
        public string EditNew(int id)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus wdbus = new WorkDiaryBus();
            ViewBag.editid = id;
            var dt = wdbus.EditWorkDiary(id);
            return JsonConvert.SerializeObject(dt);
        }
        [HttpGet]
        [Route("allworkapproval")]
        public string WorkApprovalView(string EmpId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getWorkApproval(lCredentials.EmpPkId);
            return JsonConvert.SerializeObject(dt);
        }
        [HttpGet]
        
        public string WorkApprovalViewDelete(string EmpId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getWorkApprovalDelete(lCredentials.EmpPkId);
            return JsonConvert.SerializeObject(dt);
        }
        [HttpPost]
        public string WorkApprovals(string WorkId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt1 = Wbus.checkstatus(WorkId, lCredentials.EmpPkId);
            var dt = "";
            if (dt1 == "Approved")
            {
                TempData["Forward"] = "WorkDiary has already been Approved";
            }
            else
            {
                string lcontrolling = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a=>a.ControllingAuthority).FirstOrDefault();
                
                dt = Wbus.ApproveWD(WorkId, lCredentials.EmpPkId);
                TempData["Approve"] = "WorkDiary Approved Successfully";
            }
            return JsonConvert.SerializeObject(dt);
        }

        //delete workdairy pending records
        [HttpPost]
        public string WorkDairyDelete(string WorkId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt1 = Wbus.checkstatus(WorkId, lCredentials.EmpPkId);
            var dt = "";
            if (dt1 == "Pending")
            {
                //string lcontrolling = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.ControllingAuthority).FirstOrDefault();

                dt = Wbus.deleteworkdairy(WorkId, lCredentials.EmpPkId);
                TempData["Approve"] = "WorkDiary Deleted Successfully";
            }
            else
            {
                TempData["Forward"] = "WorkDiary Approved Records Cannot Delete";
            }
            return JsonConvert.SerializeObject(dt);
        }

        [HttpPost]
        public string WorkDairyDelete1(string WorkId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
          
           
                //string lcontrolling = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.ControllingAuthority).FirstOrDefault();

               var dt = Wbus.deleteworkdairy1(WorkId, lCredentials.EmpPkId);
                TempData["Approve"] = "WorkDiary Deleted Successfully";
         
            return JsonConvert.SerializeObject(dt);
        }
       

        [HttpGet]
        public string WDTooltip(string WorkId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getWorkToolTip(WorkId);
            return JsonConvert.SerializeObject(dt);
        }
        [HttpGet]
        public string WDTooltipDelete(string WorkId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getWorkToolTip(WorkId);
            return JsonConvert.SerializeObject(dt);
        }
        [HttpGet]
        public string WDNexttip(string WorkId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getWorkToolTip(WorkId);
            return JsonConvert.SerializeObject(dt);
        }
        [HttpGet]
        public string WDNexttipDelete(string WorkId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getWorkToolTip(WorkId);
            return JsonConvert.SerializeObject(dt);
        }
        [HttpGet]
        public string WDPrevioustip(string WorkId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getWorkToolTip(WorkId);
            return JsonConvert.SerializeObject(dt);
        }
        [HttpGet]
        public string WDPrevioustipDelete(string WorkId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getWorkToolTip(WorkId);
            return JsonConvert.SerializeObject(dt);
        }
        [HttpGet]
        [Route("allworkapproval")]
        public string SanctionWorkdairyApprovalView(string EmpId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getSanctionWorkApproval(lCredentials.EmpPkId);
            return JsonConvert.SerializeObject(dt);
        }

        [HttpPost]
        public string Selfworkdairysearch(string StartDate, string EndDate, string workname, string status)
        {
            {
                workname = "0";
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();                             
                if (workname == "")
                {
                    Session["lworkDate"] = StartDate;
                    Session["lEndDate"] = EndDate;
                    Session["lEmpId"] = workname;
                    Session["lstatus"] = status;


                    WorkDiaryBus wbus = new WorkDiaryBus();
                    var data = wbus.Workdairysearches(StartDate, EndDate, workname, lCredentials.EmpId, status);
                    return JsonConvert.SerializeObject(data);
                }
                else
                {
                    if (workname != "0") { 

                        int wid = Convert.ToInt32(workname);
                        string lworkname = db.All_Masters.Where(a => a.Id == wid).Select(a => a.Name).FirstOrDefault();
                        Session["lworkDate"] = StartDate;
                        Session["lEndDate"] = EndDate;
                        Session["lEmpId"] = lworkname;
                        Session["lstatus"] = status;

                        WorkDiaryBus wbus = new WorkDiaryBus();
                        var data = wbus.Workdairysearches(StartDate, EndDate, lworkname, lCredentials.EmpId, status);
                        return JsonConvert.SerializeObject(data);
                    }
                    else
                    {
                        string lworkname = "ALL";
                        Session["lworkDate"] = StartDate;
                        Session["lEndDate"] = EndDate;
                        Session["lEmpId"] = lworkname;
                        Session["lstatus"] = status;

                        WorkDiaryBus wbus = new WorkDiaryBus();
                        var data = wbus.Workdairysearches(StartDate, EndDate, lworkname, lCredentials.EmpId, status);
                        return JsonConvert.SerializeObject(data);
                    }
                  
                }


            }
        }
        public FileResult CreatePdfWorkDairyApply()
        {
            string StartDate = Convert.ToString(Session["lworkDate"]);
            string EndDate = Convert.ToString(Session["lEndDate"]);
            string workname = Convert.ToString(Session["lEmpId"]);
            string lstatus = Convert.ToString(Session["lstatus"]);      
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("WorkDairyList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 7 columns  
            PdfPTable tableLayout1 = new PdfPTable(6);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDFwd1(tableLayout1, StartDate, EndDate, workname, lstatus));
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
            Session.Remove("lworkDate");
            Session.Remove("lEndDate");
            Session.Remove("lEmpId");
            Session.Remove("lstatus");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDFwd1(PdfPTable tableLayout1,string sd, string ed, string wn, string status)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            string lworkDate = string.Empty;
            if (lworkDate != "")
            {
                DateTime star1 = DateTime.Parse(lworkDate);
                lworkDate = star1.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            float[] headers1 = { 7, 15, 15, 10, 45, 10 }; //Header Widths  
            tableLayout1.SetWidths(headers1); //Set the pdf headers  
            tableLayout1.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout1.HeaderRows = 1;
            DateTime printdate = DateTime.Now;
            tableLayout1.AddCell(new PdfPCell(new Phrase(printdate.Date.ToShortDateString() + " " + printdate.ToShortTimeString(), new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            tableLayout1.FooterRows = 1;
            tableLayout1.AddCell(new PdfPCell(new Phrase("WorkDairyReport", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lworkdairy = db.WorkDiary.ToList();
            var lworkdairydet = db.WorkDiary_Det.ToList();
            var lemp = db.Employes.ToList();
            var desig = db.Designations.ToList();
            WorkDiaryBus wbus = new WorkDiaryBus();
            var data = wbus.Workdairysearches(sd, ed, wn, lCredentials.EmpId, status);
            //Adding headers  
            AddCellToHeader(tableLayout1, "EmpId");
            AddCellToHeader(tableLayout1, "EmpName");
            AddCellToHeader(tableLayout1, "Designation");
            AddCellToHeader(tableLayout1, "WorkDate");
            //AddCellToHeader(tableLayout1, "WorkName");
            AddCellToHeader(tableLayout1, "WorkDescription");
            AddCellToHeader(tableLayout1, "Status");

            //Adding body  
            for (int i = 0; i < data.Rows.Count; i++)
            {
                string lempid = (string)data.Rows[i]["EmpId"].ToString();
                string lShortName = (string)data.Rows[i]["Name"].ToString();
                string lCode = (string)data.Rows[i]["Designation"].ToString();
                DateTime lwdate = (DateTime)data.Rows[i]["WorkDate"];
                //string lworkName = (string)data.Rows[i]["WorkName"].ToString();
                string lworkDesc = (string)data.Rows[i]["WorkDescription"].ToString();
                string lStatus = (string)data.Rows[i]["Status"].ToString();
                AddCellToBody(tableLayout1, lempid);
                AddCellToBody(tableLayout1, lShortName);
                AddCellToBody(tableLayout1, lCode);
                AddCellToBody(tableLayout1, lwdate.ToString("dd/MM/yyyy"));
                //AddCellToBody(tableLayout1, lworkName);
                AddCellToBody(tableLayout1, lworkDesc);
                AddCellToBody(tableLayout1, lStatus);
            }
            return tableLayout1;
        }
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.WHITE)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
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
        public string WDApprovalSearch(string EmpId, string wdate, string tdate,string status)
        {
            LoginCredential lcredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();    
            var dt = Wbus.getWorkApprovalSearch(lcredentials.EmpPkId, EmpId, wdate, tdate,status);
            return JsonConvert.SerializeObject(dt);
        }
        public string WDApprovalSearchDelete(string EmpId, string wdate, string tdate, string status)
        {
            LoginCredential lcredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            var dt = Wbus.getWorkApprovalSearchDelete(lcredentials.EmpPkId, EmpId, wdate, tdate, status);
            return JsonConvert.SerializeObject(dt);
        }
        public string WDApprovalSanctionSearch(string EmpId, string wdate , string tdate)
        {
            Session["lempid"] = EmpId;
            Session["lwdate"] = wdate;
            LoginCredential lcredentials = LoginHelper.GetCurrentUser();
            WorkDiaryBus Wbus = new WorkDiaryBus();
            string lEmpId = db.Employes.Where(a => a.EmpId == EmpId).Select(a => a.EmpId).FirstOrDefault();
            var dt = Wbus.getSanctionWorkApprovalSearch(lcredentials.EmpPkId, EmpId, wdate , tdate);
            return JsonConvert.SerializeObject(dt);
        }
        public void ExportGridToExcelSanctionWD()
        {
            try
            {
                string lempid = Convert.ToString(Session["lempid"]);
                string lwdate = Convert.ToString(Session["lwdate"]);
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                ReportBusiness Rbus = new ReportBusiness();
                var data = Rbus.getSanctionWorkApprovalExcel(lCredentials.EmpPkId,lempid,lwdate);
                var gv = new GridView();
                gv.DataSource = data;
                if ((data.Rows.Count == 0))
                {
                    gv.ShowHeaderWhenEmpty = true;
                }
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=WorkDairyList.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                gv.Width = 0;
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                Session.Remove("lempid");
                Session.Remove("lwdate");
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
    }
}

    
