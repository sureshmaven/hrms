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
    public class ChangeAuthorityController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        // GET: ChangeAuthority
        [HttpGet]
        public ActionResult Index()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }
        [HttpPost]
        public ActionResult ChangeAuthority(FormCollection form)
        {


            var emplist = db.Employes.ToList();
            string lEmpIdValue = form["fffff"];
            var res = new Employee_Transfer();
            string lid = lEmpIdValue.Replace(",", "");
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            var ldbresult = db.Employes.ToList();
            int ltansferId = db.Employes.Where(a => a.EmpId == lid).Select(a => a.Id).FirstOrDefault();
            var ldata = Facade.EntitiesFacade.GetEmpTabledata.GetById(ltansferId);
            return RedirectToAction("Index");
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
        public JsonResult Authority1555(string criteria)
        {

            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var employees = db.Employes.ToList();
            var lResult = (from userslist in employees
                           where userslist.RetirementDate >= lStartDate
                           select new
                           {
                               userslist.EmpId,
                               Name = userslist.ShortName,
                           });
            var lresponseArray = lResult.ToArray();
            return Json(lresponseArray.ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Authority111(string prefix)
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
                               where userslist.RetirementDate >= lStartDate
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
                                   where userslist.RetirementDate >= lStartDate

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
                                   where userslist.RetirementDate >= lStartDate
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
        [HttpPost]
        public JsonResult GetAllEmployee(FormCollection form)
        {
            var authority = form["EmployeeCode"];
            // var authorityid = db.Employes.Where(a => a.EmpId == authority).Select(a => a.Id).FirstOrDefault();
            var employees = db.View_ChangingAuthority.Where(a => a.ControllingEmpId == authority || a.SanctioningEmpId == authority).ToList();
            //var lbranches = db.Branches.ToList();
            //var ldepartments = db.Departments.ToList();
            //var ldesignations = db.Designations.ToList();
            var lResult = (from userslist in employees
                               //join branchlist in lbranches on userslist.Branch equals branchlist.Id
                               //join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                               //join dept in ldepartments on userslist.Department equals dept.Id

                           select new
                           {
                               userslist.EmpId,
                               EmployeeName = userslist.EmpName,
                               Deptbranch = GetBranchDepartmentConcat(userslist.BranchName, userslist.DeptName),
                               Name = userslist.Designation,
                               ControllingAuthority = userslist.ControllingAuthority,
                               SanctioningAuthority = userslist.SanctioningAuthority
                           });
            return Json(lResult, JsonRequestBehavior.AllowGet);

        }
        //Update control authority
        [HttpPost]
        public JsonResult UpdateAuthority(string EmployeeCodey, string control, string sanction)
        {

            string lMessage = string.Empty;
            try
            {

                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeCodey))
                {
                    lCode = EmployeeCodey.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                string success_result = "";
                string fail_result = "";
                string success_result1 = "";
                for (int i = 0; i < lCode.Count; i++)
                {
                    string lempid1 = lCode[i];
                    var controlid = db.Employes.Where(a => a.EmpId == control).Select(a => a.Id).FirstOrDefault();
                    var sanctionid = db.Employes.Where(a => a.EmpId == sanction).Select(a => a.Id).FirstOrDefault();
                    var controlEcode = db.Employes.Where(a => a.EmpId == lempid1).Select(a => a.ControllingAuthority).FirstOrDefault();
                    var sanctionlEcode = db.Employes.Where(a => a.EmpId == lempid1).Select(a => a.SanctioningAuthority).FirstOrDefault();
                    var employeelist = db.Employes.ToList();
                    if (controlEcode == Convert.ToString(sanctionid))
                    {
                        success_result1 += lempid1.ToString() + ",";
                        TempData["AlertMessage"] = "These employee " + success_result1 + " are  already assigned as Controlling Authority";
                    }

                    else if (controlEcode == controlid.ToString())
                    {
                        Employees lemployee = employeelist.Where(a => a.EmpId == lempid1).FirstOrDefault();
                        lemployee.ControllingAuthority = sanctionid.ToString();
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                        success_result += lempid1.ToString() + ",";
                        TempData["AlertMessage"] = "Controlling Authority (" + "" + control + "" + ") Changed successfully for Employee (" + "" + success_result + "" + ")";
                    }
                    else
                    {
                        fail_result += lempid1.ToString() + ",";
                        TempData["AlertMessageFail"] = "The Selected Employees does not contain " + "" + control + " " + "as Controlling Authority for Employee (" + "" + fail_result + "" + ")";
                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            //string id = "";
            //return Json(id = "jjj", JsonRequestBehavior.AllowGet);
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }

        //update sanction Authority
        [HttpPost]
        public JsonResult UpdateAuthority2(string EmployeeCodey, string control, string sanction)
        {

            string lMessage = string.Empty;
            try
            {

                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeCodey))
                {
                    lCode = EmployeeCodey.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                string success_result = "";
                string success_result1 = "";
                string fail_result = "";
                for (int i = 0; i < lCode.Count; i++)
                {
                    string lempid1 = lCode[i];
                    var controlid = db.Employes.Where(a => a.EmpId == control).Select(a => a.Id).FirstOrDefault();
                    var sanctionid = db.Employes.Where(a => a.EmpId == sanction).Select(a => a.Id).FirstOrDefault();
                    var controlEcode = db.Employes.Where(a => a.EmpId == lempid1).Select(a => a.ControllingAuthority).FirstOrDefault();
                    var sanctionlEcode = db.Employes.Where(a => a.EmpId == lempid1).Select(a => a.SanctioningAuthority).FirstOrDefault();
                    var employeelist = db.Employes.ToList();
                    if (sanctionlEcode == Convert.ToString(sanctionid))
                    {
                        success_result1 += lempid1.ToString() + ",";
                        TempData["AlertMessage"] = "These employees " + success_result1 + " are already assigned as Sanctioning Authority";
                    }
                    else if (sanctionlEcode == controlid.ToString())
                    {
                        Employees lemployee = employeelist.Where(a => a.EmpId == lempid1).FirstOrDefault();
                        lemployee.SanctioningAuthority = sanctionid.ToString();
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                        success_result += lempid1.ToString() + ",";
                        TempData["AlertMessage"] = "Sanctioning Authority Changed successfully for Employee (" + "" + success_result + "" + ")";
                    }

                    else
                    {
                        fail_result += lempid1.ToString() + ",";
                        TempData["AlertMessageFail"] = "The Selected Employees does not contain" + "" + sanction + " " + "as Sanctioning Authority for Employee (" + "" + fail_result + "" + ")";
                    }

                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            //string id = "";
            //return Json(id = "jjj", JsonRequestBehavior.AllowGet);
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }
    }
}