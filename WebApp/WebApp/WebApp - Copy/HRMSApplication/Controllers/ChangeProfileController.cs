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

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class ChangeProfileController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        // GET: ChangeProfile
        public ActionResult Index()
        {

            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }
        [HttpGet]
        public JsonResult ChangeProfile1555(string criteria)
        {

            var employees = db.Employes.ToList();
            var lResult = (from userslist in employees
                           select new
                           {
                               userslist.EmpId,
                               Name = userslist.ShortName,
                           });
            var lresponseArray = lResult.ToArray();
            return Json(lresponseArray.ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ChangeProfile111(string prefix)
        {


            string status = "false";
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (prefix == "")
            {
                string lempid = prefix;
                var employees = db.Employes.ToList();
                var lbranches = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var ldesignations = db.Designations.ToList();
                var lResult = (from userslist in employees
                               join branchlist in lbranches on userslist.Branch equals branchlist.Id
                               join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                               join dept in ldepartments on userslist.Department equals dept.Id
                               select new
                               {
                                   userslist.EmpId,
                                   EmployeeName = userslist.ShortName,
                                   Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                   desig.Name,

                               });
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].EmployeeName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesignation = lresponseArray[0].Name;
                return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation, Status = status }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string lstatus = "True";
                string lempid = prefix;
                int count = db.Employes.Where(a => a.EmpId == lempid).Count();
                string RetirementDate = db.Employes.Where(a => a.EmpId == lempid).Select(a => a.RetirementDate.ToString()).FirstOrDefault();
                DateTime lrdatee = Convert.ToDateTime(RetirementDate).Date;
                if (count == 0)
                {
                    string lstatus1 = "Notfound";
                    var employees = db.Employes.ToList();
                    var lbranches = db.Branches.ToList();
                    var ldepartments = db.Departments.ToList();
                    var ldesignations = db.Designations.ToList();
                    var lResult = (from userslist in employees
                                   join branchlist in lbranches on userslist.Branch equals branchlist.Id
                                   join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                                   join dept in ldepartments on userslist.Department equals dept.Id
                                   select new
                                   {
                                       userslist.EmpId,
                                       EmployeeName = userslist.ShortName,
                                       Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                       desig.Name,

                                   });
                    var lresponseArray = lResult.ToArray();
                    //string employeeId = "";
                    //string employeeName = "";
                    //string Deptbranchs = "";
                    //string ldesignation = "";
                    //if (prefix != "")
                    //{
                    string employeeId = lresponseArray[0].EmpId;
                    string employeeName = lresponseArray[0].EmployeeName;
                    string Deptbranchs = lresponseArray[0].Deptbranch;
                    string ldesignation = lresponseArray[0].Name;

                    return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation, Status = lstatus1 }, JsonRequestBehavior.AllowGet);
                }
                else
                if (lrdatee < lStartDate)
                {
                    string lstatus1 = "AlreadyRetired";
                    var employees = db.Employes.ToList();
                    var lbranches = db.Branches.ToList();
                    var ldepartments = db.Departments.ToList();
                    var ldesignations = db.Designations.ToList();
                    var lResult = (from userslist in employees
                                   join branchlist in lbranches on userslist.Branch equals branchlist.Id
                                   join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                                   join dept in ldepartments on userslist.Department equals dept.Id
                                   select new
                                   {
                                       userslist.EmpId,
                                       EmployeeName = userslist.ShortName,
                                       Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                       desig.Name,

                                   });
                    var lresponseArray = lResult.ToArray();
                    //string employeeId = "";
                    //string employeeName = "";
                    //string Deptbranchs = "";
                    //string ldesignation = "";
                    //if (prefix != "")
                    //{
                    string employeeId = lresponseArray[0].EmpId;
                    string employeeName = lresponseArray[0].EmployeeName;
                    string Deptbranchs = lresponseArray[0].Deptbranch;
                    string ldesignation = lresponseArray[0].Name;

                    return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation, Status = lstatus1 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var employees = db.Employes.ToList();
                    var lbranches = db.Branches.ToList();
                    var ldepartments = db.Departments.ToList();
                    var ldesignations = db.Designations.ToList();
                    var lResult = (from userslist in employees
                                   join branchlist in lbranches on userslist.Branch equals branchlist.Id
                                   join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                                   join dept in ldepartments on userslist.Department equals dept.Id
                                   where userslist.EmpId == lempid
                                   select new
                                   {
                                       userslist.EmpId,
                                       EmployeeName = userslist.ShortName,
                                       Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                       desig.Name,

                                   });
                    var lresponseArray = lResult.ToArray();
                    //string employeeId = "";
                    //string employeeName = "";
                    //string Deptbranchs = "";
                    //string ldesignation = "";
                    //if (prefix != "")
                    //{
                    string employeeId = lresponseArray[0].EmpId;
                    string employeeName = lresponseArray[0].EmployeeName;
                    string Deptbranchs = lresponseArray[0].Deptbranch;
                    string ldesignation = lresponseArray[0].Name;

                    return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation, Status = lstatus }, JsonRequestBehavior.AllowGet);
                }
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
        public static DateTime GetCurrentTime(DateTime ldate)
        {
            DateTime serverTime = DateTime.Now;
            DateTime utcTime = serverTime.ToUniversalTime();
            // convert it to Utc using timezone setting of server computer
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }
        [HttpGet]
        public JsonResult Getprofiledetails(string EmployeeCodey)
        {
            string lresult = string.Empty;
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                var employees = db.Employes.ToList();
                int lUserLoginId = employees.Where(a => a.EmpId.ToLower() == lCredentials.EmpId.ToLower()).Select(a => a.Id).FirstOrDefault();
                int empid = db.Employes.Where(a => a.EmpId == EmployeeCodey).Select(a => a.Id).FirstOrDefault();
                string RetirementDate = db.Employes.Where(a => a.Id == empid).Select(a => a.RetirementDate.ToString()).FirstOrDefault();
                DateTime lCurrentDate = GetCurrentTime(DateTime.Now).Date;
                DateTime lrdatee = Convert.ToDateTime(RetirementDate).Date;
                if (lrdatee >= lCurrentDate)
                {
                    var lResult = (from userslist in employees
                                   where userslist.EmpId == EmployeeCodey
                                   select new
                                   {
                                       userslist.PresentAddress,
                                       userslist.PermanentAddress,
                                       userslist.MobileNumber,
                                       userslist.HomeNumber,
                                       userslist.EmergencyContactNo,
                                       userslist.OfficalEmailId,
                                       userslist.PersonalEmailId,
                                       userslist.Graduation,
                                       userslist.PostGraduation,
                                       userslist.ProfessionalQualifications,

                                   });
                    var lresponseArray = lResult.ToArray();

                    string lPresentAddress = lresponseArray[0].PresentAddress;
                    string lPermanentAddress = lresponseArray[0].PermanentAddress;
                    string lMobileNumber = lresponseArray[0].MobileNumber;
                    string lHomeNumber = lresponseArray[0].HomeNumber;
                    string lEmergencyNumber = lresponseArray[0].EmergencyContactNo;
                    string lprofEmailId = lresponseArray[0].OfficalEmailId;
                    string lPersonalEmailId = lresponseArray[0].PersonalEmailId;
                    string lGraduation = lresponseArray[0].Graduation;
                    string lPostGraduation = lresponseArray[0].PostGraduation;
                    string lProfessionalQualifications = lresponseArray[0].ProfessionalQualifications;
                    Session["laddresss"] = lPresentAddress;
                    Session["lPaddresss"] = lPermanentAddress;
                    Session["lmobiles"] = lMobileNumber;
                    Session["lHmobiles"] = lHomeNumber;
                    Session["lEmermobiles"] = lEmergencyNumber;
                    Session["lPemails"] = lprofEmailId;
                    Session["lemails"] = lPersonalEmailId;
                    Session["lGrad"] = lGraduation;
                    Session["lPGrad"] = lPostGraduation;
                    Session["lqaulifications"] = lProfessionalQualifications;
                    return Json(new
                    {
                        lpresentAddressAjax = lPresentAddress,
                        lPermanentAddressAjax = lPermanentAddress,
                        lmobilenumberAjax = lMobileNumber,
                        lhomenumberAjax = lHomeNumber,
                        lemernumberAjax = lEmergencyNumber,
                        lprofemailAjax = lprofEmailId,
                        lpersonalemailAjax = lPersonalEmailId,
                        lgradAjax = lGraduation,
                        lPgradAjax = lPostGraduation,
                        lprofessionalqualificationAjax = lProfessionalQualifications
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        lpresentAddressAjax = "",
                        lPermanentAddressAjax = "",
                        lmobilenumberAjax = "",
                        lhomenumberAjax = "",
                        lemernumberAjax = "",
                        lprofemailAjax = "",
                        lpersonalemailAjax = "",
                        lgradAjax = "",
                        lPgradAjax = "",
                        lprofessionalqualificationAjax = ""
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;
        }
        [HttpPost]
        public ActionResult ProfileIndex(FormCollection form)
        {
            var employeelist = db.Employes.ToList();
            string empid = form["Employeeid"];
            string PermAddress = form["perAddress"];
            string PresentAddress = form["presadd"];
            string MobNumber = form["MobiNumber"];
            string HomNumber = form["HomNumber"];
            string EmeNumber = form["EmerNumber"];
            string OffEmailId = form["OffEmail"];
            string perEmailId = form["PerEmail"];
            string graduation = form["gradua"];
            string postgraduation = form["PostGrad"];
            string profqualification = form["ProfQualification"];
            //if (profqualification == "" && PermAddress == "" && PresentAddress == "" && MobNumber == "" && HomNumber == "" && EmeNumber == "" && OffEmailId == "" &&perEmailId == "" &&graduation == "" && postgraduation == "" && profqualification == "")
            //{
            //    TempData["AlertMessage"] = "Please Enter New Details";
            //}
            if (PermAddress != "")
            {
                Employees lemployee = employeelist.Where(a => a.EmpId == empid).FirstOrDefault();
                if (lemployee == null)
                {
                    TempData["AlertMessage"] = "EmpCode doesnot Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    lemployee.PermanentAddress = PermAddress;
                    db.Entry(lemployee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated successfully";
                }
            }
            if (PresentAddress != "")
            {
                Employees lemployee = employeelist.Where(a => a.EmpId == empid).FirstOrDefault();
                if (lemployee == null)
                {
                    TempData["AlertMessage"] = "EmpCode doesnot Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    lemployee.PresentAddress = PresentAddress;
                    db.Entry(lemployee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated successfully";
                }
            }
            if (MobNumber != "")
            {
                Employees lemployee = employeelist.Where(a => a.EmpId == empid).FirstOrDefault();
                if (lemployee == null)
                {
                    TempData["AlertMessage"] = "EmpCode doesnot Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    lemployee.MobileNumber = MobNumber;
                    db.Entry(lemployee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated successfully";
                }
            }
            if (HomNumber != "")
            {
                Employees lemployee = employeelist.Where(a => a.EmpId == empid).FirstOrDefault();
                if (lemployee == null)
                {
                    TempData["AlertMessage"] = "EmpCode doesnot Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    lemployee.HomeNumber = HomNumber;
                    db.Entry(lemployee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated successfully";
                }
            }
            if (EmeNumber != "")
            {
                Employees lemployee = employeelist.Where(a => a.EmpId == empid).FirstOrDefault();
                if (lemployee == null)
                {
                    TempData["AlertMessage"] = "EmpCode doesnot Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    lemployee.EmergencyContactNo = EmeNumber;
                    db.Entry(lemployee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated successfully";
                }
            }
            if (OffEmailId != "")
            {
                Employees lemployee = employeelist.Where(a => a.EmpId == empid).FirstOrDefault();
                if (lemployee == null)
                {
                    TempData["AlertMessage"] = "EmpCode doesnot Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    lemployee.OfficalEmailId = OffEmailId;
                    db.Entry(lemployee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated successfully";
                }
            }
            if (perEmailId != "")
            {
                Employees lemployee = employeelist.Where(a => a.EmpId == empid).FirstOrDefault();
                if (lemployee == null)
                {
                    TempData["AlertMessage"] = "EmpCode doesnot Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    lemployee.PersonalEmailId = perEmailId;
                    db.Entry(lemployee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated successfully";
                }
            }
            if (graduation != "")
            {
                Employees lemployee = employeelist.Where(a => a.EmpId == empid).FirstOrDefault();
                if (lemployee == null)
                {
                    TempData["AlertMessage"] = "EmpCode doesnot Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    lemployee.Graduation = graduation;
                    db.Entry(lemployee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated successfully";
                }
            }
            if (postgraduation != "")
            {
                Employees lemployee = employeelist.Where(a => a.EmpId == empid).FirstOrDefault();
                if (lemployee == null)
                {
                    TempData["AlertMessage"] = "EmpCode doesnot Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    lemployee.PostGraduation = postgraduation;
                    db.Entry(lemployee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated successfully";
                }
            }

            if (profqualification != "")
            {
                Employees lemployee = employeelist.Where(a => a.EmpId == empid).FirstOrDefault();
                if (lemployee == null)
                {
                    TempData["AlertMessage"] = "EmpCode doesnot Exists";
                    return RedirectToAction("Index");
                }
                else
                {
                    lemployee.ProfessionalQualifications = profqualification;
                    db.Entry(lemployee).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated successfully";
                }
            }

            return RedirectToAction("Index");
        }
    }
}


