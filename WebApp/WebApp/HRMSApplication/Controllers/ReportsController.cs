using Entities;
using HRMSApplication.Filters;
using HRMSApplication.Helpers;
using HRMSApplication.Models;
using HRMSBusiness.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        [HttpGet]
        public ActionResult Index()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items = Facade.EntitiesFacade.GetAllBranches().Where(a => a.Name != "OtherBranch").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name,
            });
            ViewBag.Branch = new SelectList(items, "Id", "Name");

            return View("~/Views/Reports/_ReportsPartialView.cshtml");
        }


        [HttpGet]
        [Route("allbranchreports")]
        public string allBranchReport()
        {
            ReportBusiness Rbus = new ReportBusiness();
            var dt = Rbus.getAllBranches(0);
            return JsonConvert.SerializeObject(dt);
        }

        //[HttpGet]
        //public JsonResult ReportView(string EmpId)
        //{
        //    try
        //    {
        //        LoginCredential lCredentails = LoginHelper.GetCurrentUser();

        //    }
        //    catch (Exception e)
        //    {
        //        e.ToString();
        //    }
        //    return null;
        //}
        [HttpPost]
        public JsonResult ReportViews(string branch)
        {
            Session["lbranch"] = branch;
            try
            {
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var lResult = db.Employes.ToList();
                var bResult = db.Branches.ToList();
                var dResult = db.Designations.ToList();
                var Department = db.Departments.ToList();
                if (branch != "")
                {
                    if (branch == "42")
                    {
                        string lHeadoffice = "OtherDepartment";
                        var data = (from employee in lResult
                                    join desig in dResult on employee.CurrentDesignation equals desig.Id
                                    join dept in Department on employee.Department equals dept.Id
                                    where dept.Name != lHeadoffice
                                    where employee.RetirementDate >= lStartDate
                                    select new
                                    {
                                        EmpId = employee.EmpId,
                                        ShortName = employee.ShortName,
                                        BranchCode = desig.Name,
                                        BranchDept = dept.Name,
                                        DeptId = dept.Id
                                    }).OrderBy(A => A.DeptId);
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        int lbranch = Convert.ToInt32(branch);
                        var data = (from employee in lResult
                                    join desig in dResult on employee.CurrentDesignation equals desig.Id
                                    join branches in bResult on employee.Branch equals branches.Id
                                    where branches.Name != "OtherBranch"
                                    where branches.Id == lbranch
                                    where employee.RetirementDate >= lStartDate
                                    select new
                                    {
                                        EmpId = employee.EmpId,
                                        ShortName = employee.ShortName,
                                        BranchCode = desig.Name,
                                        BranchDept = branches.Name,
                                        BranchId = branches.Id,
                                    }).OrderBy(A => A.BranchId);
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (branch == "")
                    {

                        var data = (from employee in lResult
                                    join desig in dResult on employee.CurrentDesignation equals desig.Id
                                    join branches in bResult on employee.Branch equals branches.Id
                                    where branches.Name != "OtherBranch"
                                    where employee.RetirementDate >= lStartDate
                                    select new
                                    {
                                        EmpId = employee.EmpId,
                                        ShortName = employee.ShortName,
                                        BranchCode = desig.Name,
                                        BranchDept = branches.Name,
                                        BranchId = branches.Id,
                                    }).OrderBy(A => A.BranchId);
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        public DateTime getFormatDate(DateTime dt)
        {

            DateTime theDate = dt;
            string myTime = theDate.ToString("MM/dd/yyyy");
            return Convert.ToDateTime(myTime);
        }
        [HttpGet]
        public ActionResult TransferReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.BranchCode.ToString() + " " + x.Name.ToString(),
            });
            ViewBag.Branch = new SelectList(items, "Id", "Name");
            return View("~/Views/Reports/TransferReport.cshtml");
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
                requireformate =  Department;
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
        public string GetControlSanctionAuthority(string control, string sanction, string status)
        {
            string requireformate = "";
            //var leaves = db.Leaves.ToList();
            //LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            //var empid = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            if (status == "Forwarded")
            {
                requireformate = control;
            }
            else if (status == "Approved")
            {
                requireformate = sanction;
            }
            return requireformate;
        }
        [HttpGet]
        public JsonResult TransferReportView(string EmpId)
        {
            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();
                var ltransfer = db.Employee_Transfer.ToList();
                var dbResult = db.Employes.ToList();
                //var Banks = db.Banks.ToList();

                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Type != "TemporaryTransfer" && transfer.Type != "PermanentTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {

                                    transfer.Id,
                                    empid = emplist.EmpId,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo


                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        [HttpPost]
        public JsonResult TransferReportViews(FormCollection formValues)
        {
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            Session["lemp"] = formValues["EmpId"];
            Session["ltype"] = formValues["Type"];
            try
            {
                string EmployeeCode = formValues["EmpId"];
                string lType = formValues["Type"];
                string ldesignation = formValues["NewDesignation"];
                string ldepartment = formValues["NewDepartment"];
                string lbranch = formValues["NewBranch"];
                var ltransfer = db.Employee_Transfer.ToList();
                var dbResult = db.Employes.ToList();
                var Banks = db.Banks.ToList();
                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;

                if (lType == "ALL" && EmployeeCode != "")
                {

                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where emplist.EmpId == EmployeeCode
                                where transfer.Type != "TemporaryTransfer" && transfer.Type != "PermanentTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {

                                    transfer.Id,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    empid = emplist.EmpId,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo
                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (EmployeeCode != "" && lType != "")
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where emplist.EmpId == EmployeeCode && transfer.Type == lType
                                where transfer.Type != "TemporaryTransfer" && transfer.Type != "PermanentTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {

                                    transfer.Id,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    empid = emplist.EmpId,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo
                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else if (lType == "ALL")
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Type != "TemporaryTransfer" && transfer.Type != "PermanentTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    transfer.Id,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    empid = emplist.EmpId,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo
                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (EmployeeCode == "" && lType == "")
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Type != "TemporaryTransfer" && transfer.Type != "PermanentTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    transfer.Id,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    empid = emplist.EmpId,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo
                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (EmployeeCode != "" && lType == "")
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where emplist.EmpId == EmployeeCode
                                where transfer.Type != "TemporaryTransfer" && transfer.Type != "PermanentTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    transfer.Id,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    empid = emplist.EmpId,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo
                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (EmployeeCode == "" && lType != "")
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Type == lType
                                where transfer.Type != "TemporaryTransfer" && transfer.Type != "PermanentTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    transfer.Id,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    empid = emplist.EmpId,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo
                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                
            }


            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        // Report For Temporary Transfer
        [HttpGet]
        public ActionResult Temp()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.BranchCode.ToString() + " " + x.Name.ToString(),
            });
            ViewBag.Branch = new SelectList(items, "Id", "Name");
            return View("~/Views/Reports/_TempTransfer.cshtml");
        }
        [HttpGet]
        public JsonResult TempView(string EmpId)
        {
            try
            {
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();
                var ltransfer = db.Employee_Transfer.ToList();
                var dbResult = db.Employes.ToList();
                //var Banks = db.Banks.ToList();

                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Type == "TemporaryTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {

                                    transfer.Id,
                                    empid = emplist.EmpId,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo


                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        [HttpPost]
        public JsonResult TempViews(string StartDate, string EndDate)
        {
            try { 
            //  int M_no = Convert.ToInt32(branch);
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            Session["sd"] = StartDate;
            Session["ed"] = EndDate;
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            //var Banks = db.Banks.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;

                var lresult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
                if (StartDate == "" || EndDate == "")
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Type == "TemporaryTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {

                                    transfer.Id,
                                    empid = emplist.EmpId,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo


                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else  if (StartDate != "" || EndDate != "")
                {
                      DateTime fromdate = Convert.ToDateTime(StartDate);
                DateTime todate = Convert.ToDateTime(EndDate);
                DateTime eStartDate = GetCurrentTime(DateTime.Now).Date;
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Type == "TemporaryTransfer"
                                where emplist.RetirementDate >= lStartDate
                                where (transfer.EffectiveFrom >= fromdate && transfer.EffectiveTo <= todate ) || (transfer.EffectiveFrom <= fromdate && transfer.EffectiveTo >= todate)
                                select new
                                {

                                    transfer.Id,
                                    empid = emplist.EmpId,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo


                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Type == "TemporaryTransfer"
                                where emplist.RetirementDate >= lStartDate                             
                                select new
                                {

                                    transfer.Id,
                                    empid = emplist.EmpId,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo


                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        // Report For Permenant Transfer
        [HttpGet]
        public ActionResult Perm()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.BranchCode.ToString() + " " + x.Name.ToString(),
            });
            ViewBag.Branch = new SelectList(items, "Id", "Name");
            return View("~/Views/Reports/_PermTransfer.cshtml");
        }
        [HttpGet]
        public JsonResult PermView(string EmpId)
        {
            try
            {
                var ltransfer = db.Employee_Transfer.ToList();
                var dbResult = db.Employes.ToList();
                //var Banks = db.Banks.ToList();

                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Type == "PermanentTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {

                                    transfer.Id,
                                    empid = emplist.EmpId,
                                    transfer.Type,
                                    EmpName = emplist.ShortName,
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo


                                }).OrderByDescending(a => a.EffectiveFrom);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        [HttpGet]
        public ActionResult DepartmentReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Name != "OtherDepartment" && a.Active == 1).Select(x => new Departments
            {
                Id = x.Id,
                Name = x.Name
            });
            ViewBag.Branch = new SelectList(items, "Id", "Name");

            return View("~/Views/Reports/_Departmentlist.cshtml");
        }
        [HttpGet]
        public JsonResult DepartmentView(string EmpId)
        {
            try
            {
                var dbResult = db.view_employee_dept.ToList();
                var lResult = db.Employes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                join elist in lResult on emplist.EmpId equals elist.EmpId
                                where emplist.DeptName != "OtherDepartment"
                                where elist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    Name = emplist.EmpName,
                                    designation = emplist.Code,
                                    Deptbranch = emplist.DeptName
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
        public JsonResult DepartmentViews(string branch)
        {
            Session["ldept"] = branch;
            try
            {
                var dbResult = db.view_employee_dept.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var lResult = db.Employes.ToList();
                if (branch != "")
                {
                    int ldepartment = Convert.ToInt32(branch);
                    var data = (from emplist in dbResult
                                join elist in lResult on emplist.EmpId equals elist.EmpId
                                where emplist.DepartmentId == ldepartment
                                where elist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    Name = emplist.EmpName,
                                    designation = emplist.Code,
                                    Deptbranch = emplist.DeptName
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var data = (from emplist in dbResult
                                join elist in lResult on emplist.EmpId equals elist.EmpId
                                where emplist.DeptName != "OtherDepartment"
                                where elist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    Name = emplist.EmpName,
                                    designation = emplist.Code,
                                    Deptbranch = emplist.DeptName
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
        // Category
        [HttpGet]
        public ActionResult CategoryReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/_Categorylist.cshtml");
        }
        [HttpGet]
        public JsonResult CategoryView(string EmpId)
        {
            try
            {
                var dbResult = db.view_employee_category.ToList();
                var lResult = db.Employes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                join elist in lResult on emplist.EmpId equals elist.EmpId
                                where elist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    emplist.category,
                                    emplist.gender
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
        public JsonResult CategoryViews(string branch)
        {
            Session["lcategory"] = branch;
            try
            {
                var dbResult = db.view_employee_category.ToList();
                var lResult = db.Employes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (branch != "")
                {

                    var data = (from emplist in dbResult
                                join elist in lResult on emplist.EmpId equals elist.EmpId
                                where emplist.category == branch
                                where elist.RetirementDate >= lStartDate

                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    emplist.category,
                                    emplist.gender
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var data = (from emplist in dbResult
                                join elist in lResult on emplist.EmpId equals elist.EmpId
                                where elist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    emplist.category,
                                    emplist.gender
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
        public ActionResult DOBReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/_DOBlist.cshtml");
        }
        [HttpGet]
        public JsonResult DOBView(string EmpId)
        {
            try
            {
                var dbResult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                where emplist.RetirementDate_MonthWise >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    emplist.DOB_MonthWise,
                                    Age = dateofBirth(emplist.DOB_MonthWise),
                                    Year = emplist.DOB_MonthWise.Year,
                                    CurrentYear = GetCurrentTime(DateTime.Now).Year
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
        public JsonResult DOBViews(string branch)
        {
            Session["ldob"] = branch;
            if (branch == "")
            {
                var dbResult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var data = (from emplist in dbResult
                            where emplist.RetirementDate_MonthWise >= lStartDate
                            select new
                            {
                                emplist.Id,
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                emplist.DOB_MonthWise,
                                Age = dateofBirth(emplist.DOB_MonthWise),
                                Year = emplist.DOB_MonthWise.Year,
                                CurrentYear = GetCurrentTime(DateTime.Now).Year
                            });

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int M_no = Convert.ToInt32(branch);
                try
                {
                    var dbResult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
                    DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;

                    if (branch != "")
                    {

                        var data = (from emplist in dbResult
                                    where emplist.RetirementDate_MonthWise >= lStartDate
                                    select new
                                    {
                                        emplist.Id,
                                        emplist.EmpId,
                                        emplist.EmpName,
                                        emplist.Code,
                                        emplist.DOB_MonthWise,
                                        Age = dateofBirth(emplist.DOB_MonthWise),
                                        Year = emplist.DOB_MonthWise.Year,
                                        CurrentYear = GetCurrentTime(DateTime.Now).Year
                                    });
                        data = (data.ToList().Where(u => Convert.ToDateTime(u.DOB_MonthWise).Month == M_no)).ToList();
                        return Json(data, JsonRequestBehavior.AllowGet);


                    }
                    else
                    {
                        var data = (from emplist in dbResult
                                    where emplist.RetirementDate_MonthWise >= lStartDate
                                    select new
                                    {
                                        emplist.Id,
                                        emplist.EmpId,
                                        emplist.EmpName,
                                        emplist.Code,
                                        emplist.DOB_MonthWise,
                                        Age = dateofBirth(emplist.DOB_MonthWise),
                                        Year = emplist.DOB_MonthWise.Year,
                                        CurrentYear = GetCurrentTime(DateTime.Now).Year
                                    });

                        return Json(data, JsonRequestBehavior.AllowGet);

                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                }
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
        [HttpGet]
        public ActionResult RetirementReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/Retirementlist.cshtml");
        }
        [HttpGet]
        public JsonResult RetirementView(string EmpId)
        {
            try
            {
                DateTime retiremtdate = GetCurrentTime(DateTime.Now.Date);
                var dbResult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                orderby emplist.designations
                                where emplist.RetirementDate_MonthWise >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),

                                    emplist.DOJ,
                                    emplist.DOB_MonthWise,
                                    Age = dateofBirth(emplist.DOB_MonthWise),
                                    emplist.RetirementDate_MonthWise

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
        public JsonResult RetirementViewss(string EmpId)
        {
            try
            {
                DateTime retiremtdate = GetCurrentTime(DateTime.Now.Date);
                var lemployee = db.Employes.ToList();
                var dbResult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                join lemplist in lemployee on emplist.EmpId equals lemplist.EmpId
                                orderby emplist.designations
                                where emplist.RetirementDate_MonthWise < lStartDate
                                select new
                                {

                                    lemplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),

                                    emplist.DOJ,
                                    emplist.DOB_MonthWise,
                                    Age = dateofBirth(emplist.DOB_MonthWise),
                                    emplist.RetirementDate_MonthWise

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
        public string dateofBirth(DateTime dob)
        {
            string lmessage = string.Empty;
            int age = 0;
            dob = Convert.ToDateTime(dob);
            DateTime ltodaydate = GetCurrentTime(DateTime.Now).Date;
            age = ltodaydate.Year - dob.Year;
            if (ltodaydate < dob)
                age = age - 1;
            lmessage = age + "  " + "years";
            return lmessage;

        }
        [HttpPost]
        public JsonResult RetirementViews(string StartDate, string EndDate)
        {
            //  int M_no = Convert.ToInt32(branch);
            Session["sd"] = StartDate;
            Session["ed"] = EndDate;
            var dbResult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
            DateTime currentdate = GetCurrentTime(DateTime.Now.Date);
            if (StartDate == "")
            {
                var data = (from emplist in dbResult
                            orderby emplist.designations
                            where emplist.RetirementDate_MonthWise >= currentdate
                            select new
                            {
                                emplist.Id,
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),

                                emplist.DOJ,
                                emplist.DOB_MonthWise,
                                Age = dateofBirth(emplist.DOB_MonthWise),
                                emplist.RetirementDate_MonthWise

                            });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (EndDate == "")
            {
                var data = (from emplist in dbResult
                            orderby emplist.designations
                            where emplist.RetirementDate_MonthWise >= currentdate
                            select new
                            {
                                emplist.Id,
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),

                                emplist.DOJ,
                                emplist.DOB_MonthWise,
                                Age = dateofBirth(emplist.DOB_MonthWise),
                                emplist.RetirementDate_MonthWise

                            });
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            try
            {
                DateTime lcurrentDate = GetCurrentTime(DateTime.Now.Date);
                DateTime lstartdate = Convert.ToDateTime(StartDate);
                DateTime lenddate = Convert.ToDateTime(EndDate);
                DateTime eStartDate = GetCurrentTime(DateTime.Now).Date;

                if (StartDate != "" || EndDate != "")
                {

                    var data = (from emplist in dbResult
                                orderby emplist.designations
                                where ((emplist.RetirementDate_MonthWise >= lstartdate && emplist.RetirementDate_MonthWise <= lenddate) || (emplist.RetirementDate_MonthWise <= lstartdate) && (emplist.RetirementDate_MonthWise >= lenddate))
                                select new
                                {

                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),

                                    emplist.DOJ,
                                    emplist.DOB_MonthWise,
                                    Age = dateofBirth(emplist.DOB_MonthWise),
                                    emplist.RetirementDate_MonthWise
                                });

                    return Json(data, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var data = (from emplist in dbResult
                                orderby emplist.designations
                                where emplist.RetirementDate_MonthWise >= eStartDate
                                select new
                                {

                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),

                                    emplist.DOJ,
                                    emplist.DOB_MonthWise,
                                    Age = dateofBirth(emplist.DOB_MonthWise),
                                    emplist.RetirementDate_MonthWise
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
        public ActionResult CadreReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items2 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
            {
                Id = x.Id,
                Name = x.Name
            });
            ViewBag.CurrentDesignation = new SelectList(items2, "Id", "Name");
            return View("~/Views/Reports/_Cadrelist.cshtml");
        }
        public JsonResult CadreView(string EmpId)
        {
            try
            {
                var dbResult = db.view_employee_senioritylist.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.name,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                    emplist.DOJ,
                                    emplist.RetirementDate
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
        public JsonResult CadreViews(string branch)
        {
            Session["lcadre"] = branch;
            try
            {
                var dbResult = db.view_employee_senioritylist.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
               int designations = Convert.ToInt32(branch);
                string lname = db.Designations.Where(a => a.Id == designations).Select(a => a.Code).FirstOrDefault();
                if (branch != "")
                {
                    //int lbranch = Convert.ToInt32(branch);
                    var data = (from emplist in dbResult
                                where emplist.Code == lname
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,

                                    emplist.name,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                    emplist.DOJ,
                                    emplist.RetirementDate
                                });

                    return Json(data, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var data = (from emplist in dbResult
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.name,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                    emplist.DOJ,
                                    emplist.RetirementDate
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
        public ActionResult TopmanagementReport()
        {
            var items2 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
            {
                Id = x.Id,
                Name = x.Name
            });
            ViewBag.CurrentDesignation = new SelectList(items2, "Name", "Name");
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/TopmanagementList.cshtml");
        }
        public JsonResult TopmanagementView(string EmpId)
        {
            try
            {
                var dbResult = db.view_employee_senioritylist.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                where emplist.Code == "President" || emplist.Code == "CGM" || emplist.Code == "DGM" || emplist.Code == "GM" || emplist.Code == "AGM"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.name,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                    emplist.MobileNumber,
                                    emplist.PhoneNo1
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
        public ActionResult BranchNumbers()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }
        [HttpGet]
        public JsonResult BranchNumView(string Name)
        {

            try
            {
                var dbResult = db.v_BranchContactList.ToList();
                var lResult = db.Employes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(Name))
                {
                    var data = (from emplist in dbResult
                                join elist in lResult on emplist.EmpId equals elist.EmpId
                                where emplist.BranchName != "OtherBranch"
                                where elist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.BranchName,
                                    emplist.name,
                                    emplist.EmpName,
                                    emplist.code,
                                    emplist.PhoneNo1,
                                    emplist.PhoneNo2,
                                    Time = emplist.StartTime + " " + "To" + " " + emplist.EndTime

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
        public ActionResult LeaveDays()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }
        [HttpGet]
        public JsonResult NoLeaveDays(string leave)
        {
            Session["lleave"] = leave;
            try
            {

                var lemployee = db.Employes.ToList();
                var lleave = db.Leaves.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                var leavetypes = db.LeaveTypes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(leave))
                {
                    var data = (from leaves in lleave
                                join emp in lemployee on leaves.EmpId equals emp.Id
                                join branchs in lbranch on emp.Branch equals branchs.Id
                                join depart in ldepartments on emp.Department equals depart.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join ltype in leavetypes on leaves.LeaveType equals ltype.Id
                                where leaves.TotalDays >= 10 && leaves.TotalDays <= 20
                                where leaves.Status != "Cancelled" && leaves.Status != "Denied"
                                where emp.RetirementDate >= lStartDate
                                select new
                                {
                                    emp.Id,
                                    emp.EmpId,
                                    Name = emp.ShortName,
                                    leaves.StartDate,
                                    leaves.EndDate,
                                    leaves.TotalDays,
                                    ltype.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    designation = desig.Code,
                                    leaves.Status

                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = (from leaves in lleave
                                join emp in lemployee on leaves.EmpId equals emp.Id
                                join branchs in lbranch on emp.Branch equals branchs.Id
                                join depart in ldepartments on emp.Department equals depart.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join ltype in leavetypes on leaves.LeaveType equals ltype.Id
                                where leaves.TotalDays >= 10
                                where leaves.Status != "Cancelled" && leaves.Status != "Denied"
                                where (emp.EmpId.ToLower().Contains(leave.ToLower()))
                                where emp.RetirementDate >= lStartDate
                                select new
                                {
                                    emp.Id,
                                    emp.EmpId,
                                    Name = emp.ShortName,
                                    leaves.StartDate,
                                    leaves.EndDate,
                                    leaves.TotalDays,
                                    ltype.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    designation = desig.Code,
                                    leaves.Status

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
        public ActionResult LeaveDaysAbove20(string leave)
        {
            Session["lleave20"] = leave;
            var lemployee = db.Employes.ToList();
            var lleave = db.Leaves.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            var leavetypes = db.LeaveTypes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var data = (from leaves in lleave
                        join emp in lemployee on leaves.EmpId equals emp.Id
                        join branchs in lbranch on emp.Branch equals branchs.Id
                        join depart in ldepartments on emp.Department equals depart.Id
                        join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                        join ltype in leavetypes on leaves.LeaveType equals ltype.Id
                        where leaves.TotalDays > 20
                        where leaves.Status != "Cancelled" && leaves.Status != "Denied"
                        where emp.RetirementDate >= lStartDate
                        select new
                        {
                            emp.Id,
                            emp.EmpId,
                            Name = emp.ShortName,
                            leaves.StartDate,
                            leaves.EndDate,
                            leaves.TotalDays,
                            ltype.Code,
                            Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                            designation = desig.Code,
                            leaves.Status

                        });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult TotalExperience()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();

        }
        [HttpGet]
        public JsonResult totalExpview(string empid)
        {
            try
            {
                var lemployee = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(empid))
                {
                    var data = (from emp in lemployee
                                join branchs in lbranch on emp.Branch equals branchs.Id
                                join depart in ldepartments on emp.Department equals depart.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                where desig.Code == "MD" || desig.Code == "CGM" || desig.Code == "DGM" || desig.Code == "GM"
                                where emp.RetirementDate >= lStartDate
                                select new
                                {
                                    emp.Id,
                                    emp.EmpId,
                                    Name = emp.ShortName,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                    designation = desig.Code,
                                    emp.Graduation,
                                    emp.PostGraduation,
                                    emp.ProfessionalQualifications,
                                    emp.TotalExperience

                                }).OrderByDescending(a => a.designation);
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
        public ActionResult AttenderReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Name != "OtherDepartment" && a.Active == 1).Select(x => new Departments
            {
                Id = x.Id,
                Name = x.Name
            });
            ViewBag.Branch = new SelectList(items, "Id", "Name");

            return View("~/Views/Reports/_Attenderlist.cshtml");
        }
        [HttpGet]
        public JsonResult AttenderView(string EmpId)
        {
            try
            {
                var dbResult = db.view_employee_dept.ToList();
                var lResult = db.Employes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var items = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Name != "OtherDepartment" && a.Active == 1).Select(x => new Departments
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.Branch = new SelectList(items, "Id", "Name");
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                join elist in lResult on emplist.EmpId equals elist.EmpId
                                where emplist.DeptName != "OtherDepartment"
                                where emplist.Code == "ATTD" || emplist.Code == "Substaff" || emplist.Code == "DRVR"
                                where elist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    Name = emplist.EmpName,
                                    designation = emplist.Code,
                                    Deptbranch = emplist.DeptName
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
        public JsonResult AttenderViews(string empid, string department)
        {
            Session["lempid"] = empid;
            Session["ldepartment"] = department;
            try
            {
                if (empid == "" || department == "")
                {
                    var dbResult = db.view_employee_dept.ToList();
                    var lResult = db.Employes.ToList();
                    DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                    var data = (from emplist in dbResult
                                join elist in lResult on emplist.EmpId equals elist.EmpId
                                where emplist.DeptName != "OtherDepartment"
                                where emplist.Code == "ATTD" || emplist.Code == "Substaff" || emplist.Code == "DRVR"
                                where elist.RetirementDate >= lStartDate 
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    Name = emplist.EmpName,
                                    designation = emplist.Code,
                                    Deptbranch = emplist.DeptName
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int dept = Convert.ToInt32(department);
                    var dbResult = db.view_employee_dept.ToList();
                    var lResult = db.Employes.ToList();
                    DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                    var data = (from emplist in dbResult
                                join elist in lResult on emplist.EmpId equals elist.EmpId
                                where emplist.DeptName != "OtherDepartment"
                                where emplist.Code == "ATTD" || emplist.Code == "Substaff" || emplist.Code == "DRVR"
                                where elist.RetirementDate >= lStartDate
                                where emplist.EmpId == empid && emplist.DepartmentId == dept
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    Name = emplist.EmpName,
                                    designation = emplist.Code,
                                    Deptbranch = emplist.DeptName
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
        //staff Master report
        [HttpGet]
        public ActionResult SMReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/_StaffMasterlist.cshtml");
        }
        [HttpGet]
        [Route("allbranchreports")]
        public string SMView()
        {           
            ReportBusiness Rbus = new ReportBusiness();
            var dt = Rbus.getStaffMasterList();                    
            return JsonConvert.SerializeObject(dt);
        }
        [HttpPost]
        public string SMViews(string EmpID)
        {
            Session["lempid"] = EmpID;
            ReportBusiness Rbus = new ReportBusiness();
            var dt = Rbus.getStaffMasterListSearch(EmpID);
            return JsonConvert.SerializeObject(dt);
        }
        public string GetOldTransferValues(int? oldBranch)
        {
            string lBranches = "";
            if (oldBranch != null)
            {
                var lresult = db.Branches.ToList();
                var data = (from oldbranch in lresult
                            where oldbranch.Id == oldBranch
                            select new
                            {
                                oldBranch = oldbranch.Name,
                            });
                foreach (var item in data)
                {
                    lBranches = item.oldBranch;
                }
            }
            return lBranches;
        }
        public string GetNewTransferValues(int? NewDepartment)
        {
            string lDepartments = "";
            if (NewDepartment != null)
            {
                var lresult1 = db.Departments.ToList();
                var data = (from olddepartment in lresult1
                            where olddepartment.Id == NewDepartment
                            select new
                            {
                                Newdept = olddepartment.Name,
                            });
                foreach (var item in data)
                {
                    lDepartments = item.Newdept;
                }
            }
            return lDepartments;
        }
        public string GetOldDesignationsTransferValues(int? OldDesignation)
        {
            string loldDesignations = "";
            if (OldDesignation != null)
            {
                var lresult = db.Designations.ToList();
                var data = (from emplist in lresult
                            where emplist.Id == OldDesignation
                            select new
                            {
                                Olddesignations = emplist.Code,
                            });
                foreach (var item in data)
                {
                    loldDesignations = item.Olddesignations;
                }
            }
            return loldDesignations;
        }
        public string GetNewDesignationsTransferValues(int? NewDesignation)
        {
            string lNewDesignations = "";
            if (NewDesignation != null)
            {
                var lresult = db.Designations.ToList();
                var data = (from emplist in lresult
                            where emplist.Id == NewDesignation
                            select new
                            {
                                Newdesignations = emplist.Code,
                            });
                foreach (var item in data)
                {
                    lNewDesignations = item.Newdesignations;
                }
            }
            return lNewDesignations;
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
        public ActionResult PermanentTransfers()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/PermanentTransfers.cshtml");
        }
        [HttpGet]
        public JsonResult PermanentTransfersView(string EmpId)
        {
            try
            {
                var ltransfer = db.Employee_Transfer.ToList();
                var dbResult = db.Employes.ToList();
                //var Banks = db.Banks.ToList();

                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Transfer_Type == "Permanent"
                                select new
                                {

                                    transfer.Id,
                                    empid = emplist.EmpId,
                                    transfer.Transfer_Type,
                                    EmpName = GetFirstandLastName(emplist.FirstName, emplist.LastName),
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo


                                });
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        [HttpGet]
        public ActionResult TemporaryTransfers()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/TemporaryTransfers.cshtml");
        }
        [HttpGet]
        public JsonResult TemporaryTransfersView(string EmpId)
        {
            try
            {
                var ltransfer = db.Employee_Transfer.ToList();
                var dbResult = db.Employes.ToList();
                //var Banks = db.Banks.ToList();

                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Transfer_Type == "Temporary"
                                select new
                                {

                                    transfer.Id,
                                    empid = emplist.EmpId,
                                    transfer.Transfer_Type,
                                    EmpName = GetFirstandLastName(emplist.FirstName, emplist.LastName),
                                    olddesignation = desig.Code,
                                    newdesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo


                                });
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        [HttpGet]
        public ActionResult YearWiseLeaveBalanceReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/YearWiseLeaveBalanceReport.cshtml");

        }
        //[HttpGet]
        //[Route("allyearwiseleavebalance")]
        //public string allyearwiseleavebalance()
        //{
        //    ReportBusiness Rbus = new ReportBusiness();
        //    var data = Rbus.getYearwiseleavebalance();
        //    return JsonConvert.SerializeObject(data);
        //}
        [HttpGet]
        public JsonResult YearWiseLeaveBalanceViews(string EmpId)
        {
            try
            {
                var lYearbalance = db.V_EmpLeavesCarryForward.ToList();
                var lRemainBalance = db.V_EmpLeaveBalance.ToList();
                var lemployee = db.Employes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from lyearbalance in lYearbalance
                                join lrembalance in lRemainBalance on lyearbalance.EmpId equals lrembalance.EmpId
                                join emp in lemployee on lyearbalance.EmpId equals emp.Id
                                where emp.RetirementDate >= lStartDate
                                select new
                                {
                                    EmpId = emp.EmpId,
                                    EmployeeName = emp.ShortName,
                                    Dept = GetBranchDepartmentConcat(lyearbalance.DeptName, lyearbalance.BranchName),
                                    Designations = lyearbalance.DesignationName,
                                    Year = lyearbalance.Year,

                                    TotalBalanceCL = lyearbalance.CasualLeave,
                                    RemainingCL = TotalAppliedLeaves(lyearbalance.CasualLeave, 1, lyearbalance.EmpId),
                                    TotalAppliedCL = ConsumedLeaves(1, lyearbalance.EmpId),
                                    CLCarryForward = lyearbalance.CarryForward,

                                    TotalBalanceML = lyearbalance.MedicalSickLeave,
                                    RemainingML = TotalAppliedLeaves(lyearbalance.MedicalSickLeave, 2, lyearbalance.EmpId),
                                    TotalAppliedML = ConsumedLeaves(2, lyearbalance.EmpId),
                                    MLCarryForward = lyearbalance.CarryForward,

                                    TotalBalancePL = lyearbalance.PrivilegeLeave,
                                    RemainingPL = TotalAppliedLeaves(lyearbalance.PrivilegeLeave, 3, lyearbalance.EmpId),
                                    TotalAppliedPL = ConsumedLeaves(3, lyearbalance.EmpId),
                                    PLCarryForward = lyearbalance.CarryForward,

                                    TotalBalanceMTL = lyearbalance.MaternityLeave,
                                    RemainingMTL = TotalAppliedLeaves(lyearbalance.MaternityLeave, 4, lyearbalance.EmpId),
                                    TotalAppliedMTL = ConsumedLeaves(4, lyearbalance.EmpId),
                                    MTLCarryForward = lyearbalance.CarryForward,

                                    TotalBalancePTL = lyearbalance.PaternityLeave,
                                    RemainingPTL = TotalAppliedLeaves(lyearbalance.PaternityLeave, 5, lyearbalance.EmpId),
                                    TotalAppliedPTL = ConsumedLeaves(5, lyearbalance.EmpId),
                                    PTLCarryForward = lyearbalance.CarryForward,

                                    TotalBalanceEOL = lyearbalance.ExtraOrdinaryLeave,
                                    RemainingEOL = TotalAppliedLeaves(lyearbalance.ExtraOrdinaryLeave, 6, lyearbalance.EmpId),
                                    TotalAppliedEOL = ConsumedLeaves(6, lyearbalance.EmpId),
                                    EOLCarryForward = lyearbalance.CarryForward,

                                    TotalBalanceSCL = lyearbalance.SpecialCasualLeave,
                                    RemainingSCL = TotalAppliedLeaves(lyearbalance.SpecialCasualLeave, 7, lyearbalance.EmpId),
                                    TotalAppliedSCL = ConsumedLeaves(7, lyearbalance.EmpId),
                                    SCLCarryForward = lyearbalance.CarryForward,
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
        //Cancel Leave Report
        public ActionResult Cancelleavereport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/_Cancelleave.cshtml");
        }
        [HttpGet]
        public JsonResult Cancelleavereportview(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                var lleaves = db.Leaves.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                var lResults = (from leave in lleaves
                                join leavetype in lLeaveTypes on leave.LeaveType equals leavetype.Id
                                join emp in lemployees on leave.EmpId equals emp.Id
                                join branch1 in lBranches on emp.Branch equals branch1.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join dept in Departments on emp.Department equals dept.Id
                                join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                join emp2 in lemployees on leave.UpdatedBy equals emp2.EmpId
                                where leave.Status == "Cancelled"
                                select new
                                {
                                    leave.Id,
                                    emp.EmpId,
                                    EmployeeName = emp.ShortName,
                                    CancelledBy = (leave.UpdatedBy + '-' + GetFirstandLastName(emp2.FirstName, emp2.LastName)),
                                    leave.UpdatedDate,
                                    CancelTime = GetAppliedTime(leave.UpdatedDate),
                                    // leave.UpdatedDate,
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    Stage = leave.Stage,
                                    Status = leave.Status,
                                    Reason = leave.CancelReason,
                                });
                return Json(lResults.OrderByDescending(a => a.UpdatedDate) , JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public int TotalAppliedLeaves(int total, int Leavetype, int lempid)
        {
            int ltotalapplied = 0;
            int lAppliedLeaves = db.Leaves.Where(a => a.LeaveType == Leavetype).Where(a => a.EmpId == lempid)
                .Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Sum(a => (int?)a.LeaveDays) ?? 0;
            if (lAppliedLeaves != 0)
            {
                ltotalapplied = (total - lAppliedLeaves);
            }
            else if (total == 0)
            {
                ltotalapplied = 0;
            }
            return ltotalapplied;
        }
        public int ConsumedLeaves(int Leavetype, int lempid)
        {
            int ltotalapplied = 0;
            int lAppliedLeaves = db.Leaves.Where(a => a.LeaveType == Leavetype).Where(a => a.EmpId == lempid)
                .Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Sum(a => (int?)a.LeaveDays) ?? 0;
            if (lAppliedLeaves != 0)
            {
                ltotalapplied = lAppliedLeaves;
            }
            return ltotalapplied;
        }
        //Retired List
        [HttpGet]
        public ActionResult RetiredReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/_RetiredEmplist.cshtml");
        }
        [HttpGet]
        public JsonResult RetiredView(string EmpId)
        {
            try
            {
                var dbResult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                where emplist.RetirementDate_MonthWise < lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                    emplist.RetirementDate_MonthWise,

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
        public JsonResult RetiredempViews(string StartDate, string EndDate)
        {
            //  int M_no = Convert.ToInt32(branch);
            Session["sd"] = StartDate;
            Session["ed"] = EndDate;
            var lemployee = db.Employes.ToList();
            var dbResult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
            DateTime currentdate = GetCurrentTime(DateTime.Now).Date;
            if (StartDate == "")
            {
                var data = (from emplist in dbResult
                            join lemplist in lemployee on emplist.EmpId equals lemplist.EmpId
                            orderby emplist.designations
                            where emplist.RetirementDate_MonthWise < currentdate
                            select new
                            {
                                lemplist.Id,
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),

                                emplist.DOJ,
                                emplist.DOB_MonthWise,
                                Age = dateofBirth(emplist.DOB_MonthWise),
                                emplist.RetirementDate_MonthWise

                            });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (EndDate == "")
            {
                var data = (from emplist in dbResult
                            join lemplist in lemployee on emplist.EmpId equals lemplist.EmpId
                            orderby emplist.designations
                            where emplist.RetirementDate_MonthWise < currentdate
                            select new
                            {
                                lemplist.Id,
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),

                                emplist.DOJ,
                                emplist.DOB_MonthWise,
                                Age = dateofBirth(emplist.DOB_MonthWise),
                                emplist.RetirementDate_MonthWise

                            });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            try
            {
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                DateTime lstartdate = Convert.ToDateTime(StartDate);
                DateTime lenddate = Convert.ToDateTime(EndDate);
                DateTime eStartDate = GetCurrentTime(DateTime.Now).Date;
                if (StartDate != "" || EndDate != "")
                {

                    var data = (from emplist in dbResult
                                join lemplist in lemployee on emplist.EmpId equals lemplist.EmpId
                                orderby emplist.designations
                                where emplist.RetirementDate_MonthWise < lStartDate
                                where ((emplist.RetirementDate_MonthWise >= lstartdate && emplist.RetirementDate_MonthWise <= lenddate) || (emplist.RetirementDate_MonthWise <= lstartdate) && (emplist.RetirementDate_MonthWise >= lenddate))


                                select new
                                {

                                    lemplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                    emplist.DOJ,
                                    emplist.DOB_MonthWise,
                                    Age = dateofBirth(emplist.DOB_MonthWise),
                                    emplist.RetirementDate_MonthWise
                                });


                    return Json(data, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var data = (from emplist in dbResult
                                join lemplist in lemployee on emplist.EmpId equals lemplist.EmpId
                                orderby emplist.designations
                                where emplist.RetirementDate_MonthWise >= eStartDate
                                select new
                                {

                                    lemplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.Code,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),

                                    emplist.DOJ,
                                    emplist.DOB_MonthWise,
                                    Age = dateofBirth(emplist.DOB_MonthWise),
                                    emplist.RetirementDate_MonthWise
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
        // Code For List of Approval Done by Controlling and Sanctioning Authority
        public ActionResult Report1()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items3c = (from emp in db.Employes
                           where emp.Role != 4
                           select new
                           {
                               emp.EmpId,
                               s = emp.EmpId + emp.FirstName + emp.LastName,
                           });
            ViewBag.EmpId = new SelectList(items3c, "EmpId", "s");
            return View("~/Views/Reports/_CS1.cshtml");
        }
        [HttpGet]
        public JsonResult View1(string EmpId)
        {
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lMessage = string.Empty;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    where leave.Status == "Approved" || leave.Status == "Forwarded"
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    return Json(lResults, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        public string GetApproveTime(DateTime lapplieddate, DateTime lleavetimestamp, string lstatus)
        {
            string lApproved = "";
            DateTime d1 = lleavetimestamp;
            if (lstatus == "Pending")
            {
                lApproved = "00:00:00";
            }
            else
            {
                lApproved = d1.ToShortDateString().ToString() + " - " + d1.ToShortTimeString().ToString();
            }
            return lApproved;
        }
        [HttpPost]
        public JsonResult Views1(string branch, string branch2)
        {
            Session["branch"] = branch;
            Session["branch2"] = branch2;
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            try
            {
                var lleaves = db.Leaves.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                if (branch == "" || branch2 == "")
                {
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branch1 in lBranches on emp.Branch equals branch1.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    where leave.Status == "Approved" || leave.Status == "Forwarded"
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branch1.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    return Json(lResults, JsonRequestBehavior.AllowGet);
                }
                else if (branch == "Forwarded")
                {
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branch1 in lBranches on emp.Branch equals branch1.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    where leave.Status == "Forwarded"
                                    where emp1.EmpId == branch2
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branch1.Name, dept.Name),
                                        Authority = emp1.EmpId,
                                        ApprovedBy = emp1.ShortName,
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    });
                    return Json(lResults.OrderByDescending(a => a.Id), JsonRequestBehavior.AllowGet);
                }
                else
                {   
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branch1 in lBranches on emp.Branch equals branch1.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp1 in lemployees on leave.SanctioningAuthority equals emp1.Id
                                    where leave.Status == "Approved"
                                    where emp1.EmpId == branch2
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branch1.Name, dept.Name),
                                        Authority = emp1.EmpId,
                                        ApprovedBy = emp1.ShortName,
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    });
                    return Json(lResults.OrderByDescending(a => a.Id), JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        //Creating a PDF File For Attender List
        public FileResult CreatePdf()
        {
            string lempid = Convert.ToString(Session["lempid"]);
            string ldepartment = Convert.ToString(Session["ldepartment"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("AttenderList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 4 columns  
            PdfPTable tableLayout1 = new PdfPTable(4);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF1(tableLayout1, lempid, ldepartment));
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
        protected PdfPTable Add_Content_To_PDF1(PdfPTable tableLayout1, string lempid1, string ldepartment1)
        {
            float[] headers1 = { 10, 10, 10, 10 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("AttenderList", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var dbResult = db.view_employee_dept.ToList();
            var lResult = db.Employes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (lempid1 == "" && ldepartment1 == "")
            {
                var data = (from emplist in dbResult
                            join elist in lResult on emplist.EmpId equals elist.EmpId
                            where emplist.DeptName != "OtherDepartment"
                            where emplist.Code == "ATTD" || emplist.Code == "Substaff" || emplist.Code == "DRVR"
                            where elist.RetirementDate >= lStartDate
                            select new
                            {

                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                emplist.DeptName
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Department");


                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId.ToString());
                    AddCellToBody(tableLayout1, lemp.EmpName.ToString());
                    AddCellToBody(tableLayout1, lemp.Code.ToString());
                    AddCellToBody(tableLayout1, lemp.DeptName.ToString());
                }
                return tableLayout1;
            }
            else
            {
                int dept = Convert.ToInt32(ldepartment1);
                var data = (from emplist in dbResult
                            join elist in lResult on emplist.EmpId equals elist.EmpId
                            where emplist.DeptName != "OtherDepartment"
                            where emplist.Code == "ATTD" || emplist.Code == "Substaff" || emplist.Code == "DRVR"
                            where elist.RetirementDate >= lStartDate
                            where emplist.EmpId == lempid1 &&   emplist.DepartmentId == dept
                            select new
                            {

                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                emplist.DeptName
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Department");


                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId.ToString());
                    AddCellToBody(tableLayout1, lemp.EmpName.ToString());
                    AddCellToBody(tableLayout1, lemp.Code.ToString());
                    AddCellToBody(tableLayout1, lemp.DeptName.ToString());
                }
                return tableLayout1;
            }
        }
        // Method to add single cell to the Header  
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
        //Code For Export To Excel in Year Wise Leave Balance
        public void Button1_Click1(object sender, EventArgs e)
        {
            ExportGridToExcel1();
        }
        public void ExportGridToExcel1()
        {

            try
            {
                var lYearbalance = db.V_EmpLeavesCarryForward.ToList();
                var lRemainBalance = db.V_EmpLeaveBalance.ToList();
                var lemployee = db.Employes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var employeeList = (from lyearbalance in lYearbalance
                                    join lrembalance in lRemainBalance on lyearbalance.EmpId equals lrembalance.EmpId
                                    join emp in lemployee on lyearbalance.EmpId equals emp.Id
                                    where emp.RetirementDate >= lStartDate
                                    select new
                                    {
                                        EmpId = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        Dept = GetBranchDepartmentConcat(lyearbalance.DeptName, lyearbalance.BranchName),
                                        Designations = lyearbalance.DesignationName,
                                        Year = lyearbalance.Year,

                                        TotalBalanceCL = lyearbalance.CasualLeave,
                                        RemainingCL = TotalAppliedLeaves(lyearbalance.CasualLeave, 1, lyearbalance.EmpId),
                                        TotalAppliedCL = ConsumedLeaves(1, lyearbalance.EmpId),
                                        CLCarryForward = lyearbalance.CarryForward,

                                        TotalBalanceML = lyearbalance.MedicalSickLeave,
                                        RemainingML = TotalAppliedLeaves(lyearbalance.MedicalSickLeave, 2, lyearbalance.EmpId),
                                        TotalAppliedML = ConsumedLeaves(2, lyearbalance.EmpId),
                                        MLCarryForward = lyearbalance.CarryForward,

                                        TotalBalancePL = lyearbalance.PrivilegeLeave,
                                        RemainingPL = TotalAppliedLeaves(lyearbalance.PrivilegeLeave, 3, lyearbalance.EmpId),
                                        TotalAppliedPL = ConsumedLeaves(3, lyearbalance.EmpId),
                                        PLCarryForward = lyearbalance.CarryForward,

                                        TotalBalanceMTL = lyearbalance.MaternityLeave,
                                        RemainingMTL = TotalAppliedLeaves(lyearbalance.MaternityLeave, 4, lyearbalance.EmpId),
                                        TotalAppliedMTL = ConsumedLeaves(4, lyearbalance.EmpId),
                                        MTLCarryForward = lyearbalance.CarryForward,

                                        TotalBalancePTL = lyearbalance.PaternityLeave,
                                        RemainingPTL = TotalAppliedLeaves(lyearbalance.PaternityLeave, 5, lyearbalance.EmpId),
                                        TotalAppliedPTL = ConsumedLeaves(5, lyearbalance.EmpId),
                                        PTLCarryForward = lyearbalance.CarryForward,

                                        TotalBalanceEOL = lyearbalance.ExtraOrdinaryLeave,
                                        RemainingEOL = TotalAppliedLeaves(lyearbalance.ExtraOrdinaryLeave, 6, lyearbalance.EmpId),
                                        TotalAppliedEOL = ConsumedLeaves(6, lyearbalance.EmpId),
                                        EOLCarryForward = lyearbalance.CarryForward,

                                        TotalBalanceSCL = lyearbalance.SpecialCasualLeave,
                                        RemainingSCL = TotalAppliedLeaves(lyearbalance.SpecialCasualLeave, 7, lyearbalance.EmpId),
                                        TotalAppliedSCL = ConsumedLeaves(7, lyearbalance.EmpId),
                                        SCLCarryForward = lyearbalance.CarryForward,
                                    }).ToList();

                var gv = new GridView();
                gv.DataSource = employeeList;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=YearWiseReport.xls");
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
        // Create PDF for Top Management List 
        public FileResult CreatePdfTop()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("TopManagementList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 6 columns  
            PdfPTable tableLayout1 = new PdfPTable(6);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF2(tableLayout1));
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
        protected PdfPTable Add_Content_To_PDF2(PdfPTable tableLayout1)
        {
            float[] headers1 = { 12, 20, 20, 35, 20, 20 }; //Header Widths  
            tableLayout1.SetWidths(headers1); //Set the pdf headers  
            tableLayout1.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout1.HeaderRows = 1;
            tableLayout1.AddCell(new PdfPCell(new Phrase("TopManagementList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var dbResult = db.view_employee_senioritylist.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var data = (from emplist in dbResult
                        where emplist.Code == "President" || emplist.Code == "CGM" || emplist.Code == "DGM" || emplist.Code == "GM" || emplist.Code == "AGM"
                        where emplist.RetirementDate >= lStartDate
                        select new
                        {
                            emplist.Id,
                            emplist.EmpId,
                            emplist.EmpName,
                            emplist.Code,
                            BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                            emplist.MobileNumber,
                            emplist.PhoneNo1
                        });
            //Adding headers  
            AddCellToHeader(tableLayout1, "EmpId");
            AddCellToHeader(tableLayout1, "EmpName");
            AddCellToHeader(tableLayout1, "Designation");
            AddCellToHeader(tableLayout1, "BranchName");
            AddCellToHeader(tableLayout1, "MobileNumber");
            AddCellToHeader(tableLayout1, "PhoneNo1");
            //Adding body  
            foreach (var lemp in data)
            {
                AddCellToBody(tableLayout1, lemp.EmpId.ToString());
                AddCellToBody(tableLayout1, lemp.EmpName.ToString());
                AddCellToBody(tableLayout1, lemp.Code.ToString());
                AddCellToBody(tableLayout1, lemp.BranchName.ToString());
                AddCellToBody(tableLayout1, lemp.MobileNumber.ToString());
                AddCellToBody(tableLayout1, lemp.PhoneNo1.ToString());
            }
            return tableLayout1;
        }
        //Create PDF File for Key Officals
        public FileResult CreatePdfTotalExp()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("TotalExperienceList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout1 = new PdfPTable(5);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF3(tableLayout1));
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
        protected PdfPTable Add_Content_To_PDF3(PdfPTable tableLayout1)
        {
            float[] headers1 = { 40, 50, 30, 65, 60, }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("TotalExperienceList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lemployee = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var data = (from emp in lemployee
                        join branchs in lbranch on emp.Branch equals branchs.Id
                        join depart in ldepartments on emp.Department equals depart.Id
                        join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                        where desig.Code == "MD" || desig.Code == "CGM" || desig.Code == "DGM" || desig.Code == "GM"
                        where emp.RetirementDate >= lStartDate
                        select new
                        {
                            emp.Id,
                            details = emp.EmpId + " " + "-" + "\n" + "\n" + " " + emp.ShortName,
                            Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                            designation = desig.Code,
                            Qualifications = emp.Graduation + "," + emp.PostGraduation + "," + emp.ProfessionalQualifications,
                            emp.TotalExperience

                        });
            //Adding headers  
            AddCellToHeader(tableLayout1, "Details");
            //AddCellToHeader(tableLayout1, "Name");
            AddCellToHeader(tableLayout1, "Department");
            AddCellToHeader(tableLayout1, "Designation");
            AddCellToHeader(tableLayout1, "Qualifications");
            //AddCellToHeader(tableLayout1, "PostGraduation");
            //AddCellToHeader(tableLayout1, "ProfessionalQualifications");
            AddCellToHeader(tableLayout1, "TotalExperience");
            //Adding body  
            foreach (var lemp in data)
            {
                AddCellToBody(tableLayout1, lemp.details);
                // AddCellToBody(tableLayout1, lemp.Name.ToString());
                AddCellToBody(tableLayout1, lemp.Deptbranch);
                AddCellToBody(tableLayout1, lemp.designation);
                AddCellToBody(tableLayout1, lemp.Qualifications);
                //AddCellToBody(tableLayout1, lemp.PostGraduation);
                //AddCellToBody(tableLayout1, lemp.ProfessionalQualifications.ToString());
                AddCellToBody(tableLayout1, lemp.TotalExperience);
            }
            return tableLayout1;
        }
        //Create PDF File for Category
        public FileResult CreatePdfCategory()
        {

            string lcategory = Convert.ToString(Session["lcategory"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("CategoryList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout1 = new PdfPTable(5);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            doc.Add(Add_Content_To_PDF4(tableLayout1, lcategory));
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
                        tableLayout1.AddCell(new PdfPCell(new Phrase(i.ToString() + "/" + pages.ToString(), new Font(Font.FontFamily.HELVETICA, 2, 1, new BaseColor(0, 0, 0))))
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
            Session.Remove("lcategory");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF4(PdfPTable tableLayout1, string category)
        {

            float[] headers1 = { 15, 45, 30, 30, 30 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("CategoryList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lemployee = db.view_employee_category.ToList();
            if (category == "")
            {
                var data = (from emp in lemployee
                            select new
                            {
                                emp.EmpId,
                                emp.EmpName,
                                emp.Code,
                                emp.category,
                                emp.gender,
                            });

                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Code");
                AddCellToHeader(tableLayout1, "Category");
                AddCellToHeader(tableLayout1, "Gender");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.category);
                    AddCellToBody(tableLayout1, lemp.gender);
                }
            }
            else
            {
                var data = (from emp in lemployee
                            where emp.category == category
                            select new
                            {
                                emp.EmpId,
                                emp.EmpName,
                                emp.Code,
                                emp.category,
                                emp.gender,
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Code");
                AddCellToHeader(tableLayout1, "Category");
                AddCellToHeader(tableLayout1, "Gender");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.category);
                    AddCellToBody(tableLayout1, lemp.gender);
                }
            }
            return tableLayout1;
        }
        //Create PDF File for DOB
        public FileResult CreatePdfDOB()
        {
            string ldob = Convert.ToString(Session["ldob"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("DOBList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 7 columns  
            PdfPTable tableLayout1 = new PdfPTable(7);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF5(tableLayout1, ldob));
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
            Session.Remove("ldob");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF5(PdfPTable tableLayout1, string dob)
        {
            float[] headers1 = { 21, 43, 21, 27, 20, 30, 30 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("DOBList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });

            var lemployee = db.view_employee_DOB_RetirementDateMonthWise.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (dob == "")
            {
                var data = (from emp in lemployee
                            where emp.RetirementDate_MonthWise >= lStartDate
                            select new
                            {
                                emp.EmpId,
                                emp.EmpName,
                                emp.Code,
                                emp.DOB_MonthWise,
                                Year = emp.DOB_MonthWise.Year,
                                CurrentYear = GetCurrentTime(DateTime.Now).Year,
                                Age = dateofBirth(emp.DOB_MonthWise),
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Code");
                AddCellToHeader(tableLayout1, "DOB");
                AddCellToHeader(tableLayout1, "Year");
                AddCellToHeader(tableLayout1, "PresentYear");
                AddCellToHeader(tableLayout1, "Age");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.DOB_MonthWise.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.Year.ToString());
                    AddCellToBody(tableLayout1, lemp.CurrentYear.ToString());
                    AddCellToBody(tableLayout1, lemp.Age.ToString());
                }
            }
            else
            {
                int M_no = Convert.ToInt32(dob);
                var data = (from emplist in lemployee
                            where emplist.RetirementDate_MonthWise >= lStartDate
                            select new
                            {
                                emplist.Id,
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                emplist.DOB_MonthWise,
                                Age = dateofBirth(emplist.DOB_MonthWise),
                                Year = emplist.DOB_MonthWise.Year,
                                CurrentYear = GetCurrentTime(DateTime.Now).Year
                            });
                data = (data.ToList().Where(u => Convert.ToDateTime(u.DOB_MonthWise).Month == M_no)).ToList();
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Code");
                AddCellToHeader(tableLayout1, "DOB");
                AddCellToHeader(tableLayout1, "Year");
                AddCellToHeader(tableLayout1, "PresentYear");
                AddCellToHeader(tableLayout1, "Age");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.DOB_MonthWise.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.Year.ToString());
                    AddCellToBody(tableLayout1, lemp.CurrentYear.ToString());
                    AddCellToBody(tableLayout1, lemp.Age.ToString());
                }
            }
            return tableLayout1;

        }
        //Create PDF File for Branches list
        public FileResult CreatePdfBranch()
        {
            string lbranch = Convert.ToString(Session["lbranch"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("BranchList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 4 columns  
            PdfPTable tableLayout1 = new PdfPTable(4);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF6(tableLayout1, lbranch));
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
            Session.Remove("lbranch");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF6(PdfPTable tableLayout1, string branches)
        {
            float[] headers1 = { 30, 30, 30, 30 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("BranchList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var lResult = db.Employes.ToList();
            var bResult = db.Branches.ToList();
            var dResult = db.Designations.ToList();
            var Department = db.Departments.ToList();
            if (branches == "")
            {
                var data = (from employee in lResult
                            join desig in dResult on employee.CurrentDesignation equals desig.Id
                            join branch in bResult on employee.Branch equals branch.Id
                            where employee.RetirementDate >= lStartDate
                            where branch.Name != "OtherBranch"
                            select new
                            {
                                EmpId = employee.EmpId,
                                ShortName = employee.ShortName,
                                BranchCode = desig.Name,
                                BranchDept = branch.Name,
                                BranchId = branch.Id,
                                desigId = desig.Id
                            }).OrderBy(A => A.BranchId).ThenBy(a => a.desigId);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Code");
                AddCellToHeader(tableLayout1, "BranchName");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.ShortName);
                    AddCellToBody(tableLayout1, lemp.BranchCode);
                    AddCellToBody(tableLayout1, lemp.BranchDept);
                }
                return tableLayout1;
            }
            else
            if (branches == "42")
            {
                string lHeadoffice = "OtherDepartment";
                var data = (from employee in lResult
                            join desig in dResult on employee.CurrentDesignation equals desig.Id
                            join dept in Department on employee.Department equals dept.Id
                            where dept.Name != lHeadoffice
                            where employee.RetirementDate >= lStartDate
                            select new
                            {
                                EmpId = employee.EmpId,
                                ShortName = employee.ShortName,
                                BranchCode = desig.Name,
                                BranchDept = dept.Name,
                                DeptId = dept.Id
                            }).OrderBy(A => A.DeptId);

                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Code");
                AddCellToHeader(tableLayout1, "BranchName");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.ShortName);
                    AddCellToBody(tableLayout1, lemp.BranchCode);
                    AddCellToBody(tableLayout1, lemp.BranchDept);
                }
                return tableLayout1;

            }
            else
            {
                int lbranch = Convert.ToInt32(branches);
                var data = (from employee in lResult
                            join desig in dResult on employee.CurrentDesignation equals desig.Id
                            join b in bResult on employee.Branch equals b.Id
                            where b.Name != "OtherBranch"
                            where b.Id == lbranch
                            where employee.RetirementDate >= lStartDate
                            select new
                            {
                                EmpId = employee.EmpId,
                                ShortName = employee.ShortName,
                                BranchCode = desig.Name,
                                BranchDept = b.Name,
                                BranchId = b.Id,
                            }).OrderBy(A => A.BranchId);
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Code");
                AddCellToHeader(tableLayout1, "BranchName");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.ShortName);
                    AddCellToBody(tableLayout1, lemp.BranchCode);
                    AddCellToBody(tableLayout1, lemp.BranchDept);
                }
                return tableLayout1;
            }

        }
        //Create PDF File for Seniority list
        public FileResult CreatePdfSeniority()
        {
            string lempid = Convert.ToString(Session["lseniorempid"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("SeniorityList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 6 columns  
            PdfPTable tableLayout1 = new PdfPTable(6);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF7(tableLayout1,lempid));
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
        protected PdfPTable Add_Content_To_PDF7(PdfPTable tableLayout1,string empid)
        {
            float[] headers1 = { 50, 30, 50, 30, 30, 50 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("SeniorityList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var dbResult = db.view_employee_senioritylist.ToList();
            var lemployees = db.Employes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (empid == "")
            {
                var data = (from emp in dbResult
                            join emplist in lemployees on emp.EmpId equals emplist.EmpId
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {
                                details = emp.EmpId + " " + "-" + "\n" + "\n" + " " + emp.EmpName,
                                emp.Code,
                                BranchName = GetBranchDepartmentConcat(emp.BranchName, emp.DeptName),
                                DOB = emplist.DOB.Value.ToString("dd/MM/yyyy"),
                                DOJ = emplist.DOJ.Value.ToString("dd/MM/yyyy"),
                                RetirementDate = emplist.RetirementDate.Value.ToString("dd/MM/yyyy"),
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "details");
                //AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch/Dept");
                AddCellToHeader(tableLayout1, "DOB");
                AddCellToHeader(tableLayout1, "DOJ");
                AddCellToHeader(tableLayout1, "RetirementDate");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.details);
                    // AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.BranchName);
                    AddCellToBody(tableLayout1, lemp.DOB);
                    AddCellToBody(tableLayout1, lemp.DOJ);
                    AddCellToBody(tableLayout1, lemp.RetirementDate);
                }
                return tableLayout1;
            }
            else
            {
                var data = (from emp in dbResult
                            join emplist in lemployees on emp.EmpId equals emplist.EmpId
                            where emplist.RetirementDate >= lStartDate
                            where emp.EmpId.Contains(empid) 
                            select new
                            {
                                details = emp.EmpId + " " + "-" + "\n" + "\n" + " " + emp.EmpName,
                                emp.Code,
                                BranchName = GetBranchDepartmentConcat(emp.BranchName, emp.DeptName),
                                DOB = emplist.DOB.Value.ToString("dd/MM/yyyy"),
                                DOJ = emplist.DOJ.Value.ToString("dd/MM/yyyy"),
                                RetirementDate = emplist.RetirementDate.Value.ToString("dd/MM/yyyy"),
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "details");
                //AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch/Dept");
                AddCellToHeader(tableLayout1, "DOB");
                AddCellToHeader(tableLayout1, "DOJ");
                AddCellToHeader(tableLayout1, "RetirementDate");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.details);
                    // AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.BranchName);
                    AddCellToBody(tableLayout1, lemp.DOB);
                    AddCellToBody(tableLayout1, lemp.DOJ);
                    AddCellToBody(tableLayout1, lemp.RetirementDate);
                }
                return tableLayout1;
            }
        }
        //Create PDF File for Permenant Transfer
        public FileResult CreatePdfPermenant()
        {
            string ecode = Convert.ToString(Session["ecode"]);
            string ef = Convert.ToString(Session["ef"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("PermanentTransferList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 8 columns  
            PdfPTable tableLayout1 = new PdfPTable(8);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF8(tableLayout1,ecode,ef));
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
            Session.Remove("ecode");
            Session.Remove("ef");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF8(PdfPTable tableLayout1,string ecode1, string ef1)
        {
            float[] headers1 = { 25, 35, 32, 35, 35, 35, 35, 35 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("PermanentTransferList", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (ecode1 == "" && ef1 == "")
            {
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "PermanentTransfer"
                            where emplist.RetirementDate >= lStartDate    
                            select new
                            {
                                empid = emplist.EmpId,
                                transfer.Type,
                                EmpName = emplist.ShortName,
                                olddesignation = desig.Code,
                                newdesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,

                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "OldDesignation");
                AddCellToHeader(tableLayout1, "NewDesignation");
                AddCellToHeader(tableLayout1, "Old Branch/Dept");
                AddCellToHeader(tableLayout1, "New Branch/Dept");
                AddCellToHeader(tableLayout1, "EffectiveFrom");

                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.empid);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Type);
                    AddCellToBody(tableLayout1, lemp.olddesignation);
                    AddCellToBody(tableLayout1, lemp.newdesignation.ToString());
                    AddCellToBody(tableLayout1, lemp.oldDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.newDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.EffectiveFrom.Value.ToString("dd/MM/yyyy"));
                }
                return tableLayout1;
            }
            else
            {
                DateTime fromdate = Convert.ToDateTime(ef1);
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "PermanentTransfer"
                            where emplist.RetirementDate >= lStartDate
                            where transfer.EffectiveFrom == fromdate && emplist.EmpId == ecode1
                            select new
                            {
                                empid = emplist.EmpId,
                                transfer.Type,
                                EmpName = emplist.ShortName,
                                olddesignation = desig.Code,
                                newdesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,

                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "OldDesignation");
                AddCellToHeader(tableLayout1, "NewDesignation");
                AddCellToHeader(tableLayout1, "Old Branch/Dept");
                AddCellToHeader(tableLayout1, "New Branch/Dept");
                AddCellToHeader(tableLayout1, "EffectiveFrom");

                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.empid);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Type);
                    AddCellToBody(tableLayout1, lemp.olddesignation);
                    AddCellToBody(tableLayout1, lemp.newdesignation.ToString());
                    AddCellToBody(tableLayout1, lemp.oldDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.newDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.EffectiveFrom.Value.ToString("dd/MM/yyyy"));
                }
                return tableLayout1;
            }
        }
        //Create PDF File for Temporary Transfer
        public FileResult CreatePdfTemporary()
        {
            String sd = Convert.ToString(Session["sd"]);
            String ed = Convert.ToString(Session["ed"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("TemporaryTransferList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(900f, 900f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 8 columns  
            PdfPTable tableLayout1 = new PdfPTable(8);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF9(tableLayout1, sd, ed));
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
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page:" + i.ToString() + "/" + pages.ToString(), blackFont), 450f, 10f, 0);
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
            Session.Remove("sd");
            Session.Remove("ed");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF9(PdfPTable tableLayout1, string sd1, string ed1)
        {
            float[] headers1 = { 27, 40, 37, 35, 35, 35, 38, 35 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("TemporaryTransferList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (sd1 == "" || ed1 == "")
            {
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "TemporaryTransfer"
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {
                                empid = emplist.EmpId,
                                transfer.Type,
                                EmpName = emplist.ShortName,
                                olddesignation = desig.Code,
                                newdesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,
                                transfer.EffectiveTo,
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "Old Branch/Dept");
                AddCellToHeader(tableLayout1, "New Branch/Dept");
                AddCellToHeader(tableLayout1, "OldDesignation");
                AddCellToHeader(tableLayout1, "NewDesignation");
                AddCellToHeader(tableLayout1, "EffectiveFrom");

                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.empid);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Type);
                    AddCellToBody(tableLayout1, lemp.oldDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.newDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.olddesignation);
                    AddCellToBody(tableLayout1, lemp.newdesignation.ToString());
                    AddCellToBody(tableLayout1, lemp.EffectiveFrom.Value.ToString("dd/MM/yyyy"));

                }
                return tableLayout1;
            }
            else
            {
                DateTime fromdate = Convert.ToDateTime(sd1);
                DateTime todate = Convert.ToDateTime(ed1);
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "TemporaryTransfer"
                            where emplist.RetirementDate >= lStartDate
                            where (transfer.EffectiveFrom >= fromdate && transfer.EffectiveTo <= todate) || (transfer.EffectiveFrom <= fromdate && transfer.EffectiveTo >= todate)
                            select new
                            {
                                empid = emplist.EmpId,
                                transfer.Type,
                                EmpName = emplist.ShortName,
                                olddesignation = desig.Code,
                                newdesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,

                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "Old Branch/Dept");
                AddCellToHeader(tableLayout1, "New Branch/Dept");
                AddCellToHeader(tableLayout1, "OldDesignation");
                AddCellToHeader(tableLayout1, "NewDesignation");
                AddCellToHeader(tableLayout1, "EffectiveFrom");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.empid);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Type);
                    AddCellToBody(tableLayout1, lemp.oldDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.newDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.olddesignation);
                    AddCellToBody(tableLayout1, lemp.newdesignation.ToString());
                    AddCellToBody(tableLayout1, lemp.EffectiveFrom.Value.ToString("dd/MM/yyyy"));
                }
                return tableLayout1;
            }
        }
        //Create PDF File for Promotion
        public FileResult CreatePdfPromotion()
        {
            string lemp = Convert.ToString(Session["lemp"]);
            string ltype = Convert.ToString(Session["ltype"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("PromotionList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 8 columns  
            PdfPTable tableLayout1 = new PdfPTable(8);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF10(tableLayout1, lemp, ltype));
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
            Session.Remove("lemp");
            Session.Remove("ltype");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF10(PdfPTable tableLayout1, string emp, string type)
        {
            float[] headers1 = { 29, 50, 35, 38, 40, 35, 38, 39 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("PromotionList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (emp == "" && type == "")
            {
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type != "TemporaryTransfer" && transfer.Type != "PermanentTransfer"
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {
                                empid = emplist.EmpId,
                                transfer.Type,
                                EmpName = emplist.ShortName,
                                olddesignation = desig.Code,
                                newdesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,
                                transfer.EffectiveTo
                            }).OrderByDescending(a => a.EffectiveFrom);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "OldDesig");
                AddCellToHeader(tableLayout1, "NewDesig");
                AddCellToHeader(tableLayout1, "Old Branch/Dept");
                AddCellToHeader(tableLayout1, "New Branch/Dept");
                AddCellToHeader(tableLayout1, "EffectiveFrom");

                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.empid);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Type);
                    AddCellToBody(tableLayout1, lemp.olddesignation);
                    AddCellToBody(tableLayout1, lemp.newdesignation.ToString());
                    AddCellToBody(tableLayout1, lemp.oldDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.newDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.EffectiveFrom.Value.ToString("dd/MM/yyyy"));
                }
                return tableLayout1;
            }
            else
            {
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type != "TemporaryTransfer" && transfer.Type != "PermanentTransfer"
                            where emplist.RetirementDate >= lStartDate
                            where emplist.EmpId == emp || transfer.Type == type
                            select new
                            {
                                empid = emplist.EmpId,
                                transfer.Type,
                                EmpName = emplist.ShortName,
                                olddesignation = desig.Code,
                                newdesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,
                                transfer.EffectiveTo
                            }).OrderByDescending(a => a.EffectiveFrom);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "OldDesig");
                AddCellToHeader(tableLayout1, "NewDesig");
                AddCellToHeader(tableLayout1, "Old Branch/Dept");
                AddCellToHeader(tableLayout1, "New Branch/Dept");
                AddCellToHeader(tableLayout1, "EffectiveFrom");

                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.empid);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Type);
                    AddCellToBody(tableLayout1, lemp.olddesignation);
                    AddCellToBody(tableLayout1, lemp.newdesignation.ToString());
                    AddCellToBody(tableLayout1, lemp.oldDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.newDeptbranch.ToString());
                    AddCellToBody(tableLayout1, lemp.EffectiveFrom.Value.ToString("dd/MM/yyyy"));
                }
                return tableLayout1;
            }
        }
        //Create PDF File for Branch Numbers
        public FileResult CreatePdfBranchNumbers()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("BranchContactList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 6 columns  
            PdfPTable tableLayout1 = new PdfPTable(6);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF11(tableLayout1));
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
        protected PdfPTable Add_Content_To_PDF11(PdfPTable tableLayout1)
        {
            float[] headers1 = { 60, 60, 45, 38, 40, 50 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("BranchContactList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var dbResult = db.v_BranchContactList.ToList();
            var lResult = db.Employes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var data = (from emplist in dbResult
                        join elist in lResult on emplist.EmpId equals elist.EmpId
                        where emplist.BranchName != "OtherBranch"
                        where elist.RetirementDate >= lStartDate
                        select new
                        {
                            details = emplist.EmpId + " " + "-" + "\n" + "\n" + " " + emplist.EmpName,
                            emplist.BranchName,
                            // emplist.name,
                            //emplist.EmpName,
                            emplist.code,
                            emplist.PhoneNo1,
                            emplist.PhoneNo2,
                            Time = emplist.StartTime + " " + "-" + " " + emplist.EndTime

                        });
            //Adding headers  
            AddCellToHeader(tableLayout1, "Emp Details");
            //AddCellToHeader(tableLayout1, "Name");
            AddCellToHeader(tableLayout1, "BranchName");
            AddCellToHeader(tableLayout1, "Designation");
            AddCellToHeader(tableLayout1, "Extension");
            AddCellToHeader(tableLayout1, "Mobile No.");
            AddCellToHeader(tableLayout1, "BranchTime");
            //Adding body  
            foreach (var lemp in data)
            {
                AddCellToBody(tableLayout1, lemp.details);
                //AddCellToBody(tableLayout1, lemp.EmpName);
                AddCellToBody(tableLayout1, lemp.BranchName);
                AddCellToBody(tableLayout1, lemp.code);
                AddCellToBody(tableLayout1, lemp.PhoneNo1);
                AddCellToBody(tableLayout1, lemp.PhoneNo2);
                AddCellToBody(tableLayout1, lemp.Time);
            }
            return tableLayout1;
        }
        //Create PDF File for Cadre List
        public FileResult CreatePdfCadre()
        {
            string lcadre = Convert.ToString(Session["lcadre"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("CadreList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 6 columns  
            PdfPTable tableLayout1 = new PdfPTable(6);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF12(tableLayout1, lcadre));
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(byteInfo);
                Font blackFont = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, BaseColor.BLACK);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase(i.ToString(), blackFont), 568f, 15f, 0);
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
            Session.Remove("lcadre");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF12(PdfPTable tableLayout1, string cadre)
        {
            float[] headers1 = { 25, 50, 35, 38, 40, 39 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("CadreList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var dbResult = db.view_employee_senioritylist.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (cadre == "")
            {
                var data = (from emplist in dbResult
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                emplist.DOJ,
                                emplist.RetirementDate
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "DOJ");
                AddCellToHeader(tableLayout1, "RetirementDate");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.BranchName);
                    AddCellToBody(tableLayout1, lemp.DOJ.Value.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.RetirementDate.Value.ToString("dd/MM/yyyy"));

                }
                return tableLayout1;
            }
            else
            {
                int designations = Convert.ToInt32(cadre);
                string lname = db.Designations.Where(a => a.Id == designations).Select(a => a.Code).FirstOrDefault();
                var data = (from emplist in dbResult
                            where emplist.Code == lname
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {
                                emplist.Id,
                                emplist.EmpId,
                                emplist.EmpName,

                                emplist.name,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                emplist.DOJ,
                                emplist.RetirementDate
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "DOJ");
                AddCellToHeader(tableLayout1, "RetirementDate");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.name);
                    AddCellToBody(tableLayout1, lemp.BranchName);
                    AddCellToBody(tableLayout1, lemp.DOJ.Value.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.RetirementDate.Value.ToString("dd/MM/yyyy"));

                }
                return tableLayout1;
            }
        }
        //Create PDF File for Retirement List
        public FileResult CreatePdfRetirement()
        {
            string sd = Convert.ToString(Session["sd"]);
            string ed = Convert.ToString(Session["ed"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("RetirementList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 6 columns  
            PdfPTable tableLayout1 = new PdfPTable(6);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF13(tableLayout1, sd, ed));
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
            Session.Remove("sd");
            Session.Remove("ed");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF13(PdfPTable tableLayout1, string sd1, string ed1)
        {
            float[] headers1 = { 30, 60, 40, 50, 50, 50 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("RetirementList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var dbResult = db.view_employee_senioritylist.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (sd1 == "" && ed1 == "")
            {
                var data = (from emplist in dbResult
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                emplist.DOJ,
                                emplist.RetirementDate
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "BranchName");
                AddCellToHeader(tableLayout1, "DOJ");
                AddCellToHeader(tableLayout1, "RetirementDate");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.BranchName);
                    AddCellToBody(tableLayout1, lemp.DOJ.Value.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.RetirementDate.Value.ToString("dd/MM/yyyy"));

                }
                return tableLayout1;
            }
            else
            {
                DateTime lstartdate = Convert.ToDateTime(sd1);
                DateTime lenddate = Convert.ToDateTime(ed1);
                var data = (from emplist in dbResult
                            where emplist.RetirementDate >= lStartDate
                            where ((emplist.RetirementDate >= lstartdate && emplist.RetirementDate <= lenddate) || (emplist.RetirementDate <= lstartdate) && (emplist.RetirementDate >= lenddate))
                            select new
                            {
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                emplist.DOJ,
                                emplist.RetirementDate
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "BranchName");
                AddCellToHeader(tableLayout1, "DOJ");
                AddCellToHeader(tableLayout1, "RetirementDate");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.BranchName);
                    AddCellToBody(tableLayout1, lemp.DOJ.Value.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.RetirementDate.Value.ToString("dd/MM/yyyy"));

                }
                return tableLayout1;
            }
        }
        //Create PDF File for Retired Employees
        public FileResult CreatePdfRetiredEmployees()
        {
            string sd = Convert.ToString(Session["sd"]);
            string ed = Convert.ToString(Session["ed"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("RetirementList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 6 columns  
            PdfPTable tableLayout1 = new PdfPTable(6);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF20(tableLayout1, sd, ed));
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(byteInfo);
                Font blackFont = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, BaseColor.BLACK);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase(i.ToString(), blackFont), 568f, 15f, 0);
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
            Session.Remove("sd");
            Session.Remove("ed");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF20(PdfPTable tableLayout1, string sd1, string ed1)
        {
            float[] headers1 = { 29, 56, 40, 60, 45, 50 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("RetirementList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var dbResult = db.view_employee_senioritylist.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (sd1 == "" && ed1 == "")
            {
                var data = (from emplist in dbResult
                            where emplist.RetirementDate < lStartDate
                            select new
                            {
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                emplist.DOJ,
                                emplist.RetirementDate
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "BranchName");
                AddCellToHeader(tableLayout1, "DOJ");
                AddCellToHeader(tableLayout1, "RetirementDate");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.BranchName);
                    AddCellToBody(tableLayout1, lemp.DOJ.Value.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.RetirementDate.Value.ToString("dd/MM/yyyy"));
                }
                return tableLayout1;
            }
            else
            {
                DateTime lstartdate = Convert.ToDateTime(sd1);
                DateTime lenddate = Convert.ToDateTime(ed1);
                var data = (from emplist in dbResult
                            where emplist.RetirementDate < lStartDate
                            where ((emplist.RetirementDate >= lstartdate && emplist.RetirementDate <= lenddate) || (emplist.RetirementDate <= lstartdate) && (emplist.RetirementDate >= lenddate))
                            select new
                            {
                                emplist.EmpId,
                                emplist.EmpName,
                                emplist.Code,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                emplist.DOJ,
                                emplist.RetirementDate
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "BranchName");
                AddCellToHeader(tableLayout1, "DOJ");
                AddCellToHeader(tableLayout1, "RetirementDate");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.BranchName);
                    AddCellToBody(tableLayout1, lemp.DOJ.Value.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.RetirementDate.Value.ToString("dd/MM/yyyy"));
                }
                return tableLayout1;
            }
        }
        //Create PDF File for Emp Long Leaves List
        public FileResult CreatePdfLongLeaves()
        {
            String lleave = Convert.ToString(Session["lleave"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("LongLeaves" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 9 columns  
            PdfPTable tableLayout1 = new PdfPTable(9);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF14(tableLayout1, lleave));
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
            Session.Remove("lleave");    
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF14(PdfPTable tableLayout1, string empid)
        {
            float[] headers1 = { 33, 53, 45, 30, 45, 45, 30, 42, 38 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("EmpLongLeaves", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lemployee = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            var lleave = db.Leaves.ToList();
            var leavetypes = db.LeaveTypes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (empid == "")
            {
                var data = (from leaves in lleave
                            join emp in lemployee on leaves.EmpId equals emp.Id
                            join branchs in lbranch on emp.Branch equals branchs.Id
                            join depart in ldepartments on emp.Department equals depart.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join ltype in leavetypes on leaves.LeaveType equals ltype.Id
                            where leaves.TotalDays >= 10 && leaves.TotalDays <= 20
                            where leaves.Status != "Cancelled" && leaves.Status != "Denied"
                            where emp.RetirementDate >= lStartDate
                            select new
                            {
                                emp.EmpId,
                                Name = emp.ShortName,
                                leaves.StartDate,
                                leaves.EndDate,
                                leaves.TotalDays,
                                ltype.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                designation = desig.Code,
                                leaves.Status,
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "TotalDays");
                AddCellToHeader(tableLayout1, "Status");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.Name);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.TotalDays.ToString());
                    AddCellToBody(tableLayout1, lemp.Status);

                }
                return tableLayout1;
            }
            else
            {
                var data = (from leaves in lleave
                            join emp in lemployee on leaves.EmpId equals emp.Id
                            join branchs in lbranch on emp.Branch equals branchs.Id
                            join depart in ldepartments on emp.Department equals depart.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join ltype in leavetypes on leaves.LeaveType equals ltype.Id
                            where leaves.TotalDays >= 10 && leaves.TotalDays <= 20
                            where leaves.Status != "Cancelled" && leaves.Status != "Denied"
                            where emp.RetirementDate >= lStartDate
                            where emp.EmpId == empid
                            select new
                            {
                                emp.EmpId,
                                Name = emp.ShortName,
                                leaves.StartDate,
                                leaves.EndDate,
                                leaves.TotalDays,
                                ltype.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                designation = desig.Code,
                                leaves.Status,
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "TotalDays");
                AddCellToHeader(tableLayout1, "Status");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.Name);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.TotalDays.ToString());
                    AddCellToBody(tableLayout1, lemp.Status);

                }
                return tableLayout1;
            }
        }
        //Create PDF File for Long Leaves   
        public FileResult CreatePdfLongLeavesAbove20()
        {
            String lleave = Convert.ToString(Session["lleave20"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("LongLeaves" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 9 columns  
            PdfPTable tableLayout1 = new PdfPTable(9);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDFleave(tableLayout1, lleave));
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
            Session.Remove("lleave20");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDFleave(PdfPTable tableLayout1, string empid)
        {
            float[] headers1 = { 33, 53, 45, 30, 45, 45, 30, 42, 38 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("EmpLongLeaves", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lemployee = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            var lleave = db.Leaves.ToList();
            var leavetypes = db.LeaveTypes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (empid == "")
            {
                var data = (from leaves in lleave
                            join emp in lemployee on leaves.EmpId equals emp.Id
                            join branchs in lbranch on emp.Branch equals branchs.Id
                            join depart in ldepartments on emp.Department equals depart.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join ltype in leavetypes on leaves.LeaveType equals ltype.Id
                            where leaves.TotalDays > 20
                            where leaves.Status != "Cancelled" && leaves.Status != "Denied"
                            where emp.RetirementDate >= lStartDate
                            select new
                            {
                                emp.EmpId,
                                Name = emp.ShortName,
                                leaves.StartDate,
                                leaves.EndDate,
                                leaves.TotalDays,
                                ltype.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                designation = desig.Code,
                                leaves.Status,
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "TotalDays");
                AddCellToHeader(tableLayout1, "Status");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.Name);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.TotalDays.ToString());
                    AddCellToBody(tableLayout1, lemp.Status);

                }
                return tableLayout1;
            }
            else
            {
                var data = (from leaves in lleave
                            join emp in lemployee on leaves.EmpId equals emp.Id
                            join branchs in lbranch on emp.Branch equals branchs.Id
                            join depart in ldepartments on emp.Department equals depart.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join ltype in leavetypes on leaves.LeaveType equals ltype.Id
                            where leaves.TotalDays > 20
                            where leaves.Status != "Cancelled" && leaves.Status != "Denied"
                            where emp.RetirementDate >= lStartDate
                            where emp.EmpId == empid
                            select new
                            {
                                emp.EmpId,
                                Name = emp.ShortName,
                                leaves.StartDate,
                                leaves.EndDate,
                                leaves.TotalDays,
                                ltype.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                designation = desig.Code,
                                leaves.Status,
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "TotalDays");
                AddCellToHeader(tableLayout1, "Status");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.Name);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.TotalDays.ToString());
                    AddCellToBody(tableLayout1, lemp.Status);

                }
                return tableLayout1;
            }
        }
        // Create PDF File For Emp Today Leaves
        public FileResult CreatePdfTodayLeaves()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("TodayLeaves" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 9 columns  
            PdfPTable tableLayout1 = new PdfPTable(9);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF15(tableLayout1));
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
        protected PdfPTable Add_Content_To_PDF15(PdfPTable tableLayout1)
        {
            float[] headers1 = { 31, 53, 45, 34, 45, 45, 30, 42, 38 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("EmpTodayLeaves", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lemployee = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            var lleave = db.Leaves.ToList();
            var leavetypes = db.LeaveTypes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
            var data = (from leaves in lleave
                        join emp in lemployee on leaves.EmpId equals emp.Id
                        join branchs in lbranch on emp.Branch equals branchs.Id
                        join depart in ldepartments on emp.Department equals depart.Id
                        join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                        join ltype in leavetypes on leaves.LeaveType equals ltype.Id
                        where (lStartDate >= leaves.StartDate && lStartDate <= leaves.EndDate)
                        || (lEndDate <= leaves.EndDate && lEndDate >= leaves.EndDate)
                        where leaves.Status == "Approved" || leaves.Status == "Pending" || leaves.Status == "Forwarded"
                        select new
                        {
                            emp.EmpId,
                            emp.ShortName,
                            Designation = desig.Code,
                            Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                            leaves.StartDate,
                            leaves.EndDate,
                            ltype.Code,
                            leaves.Reason,
                            leaves.Status,
                        });
            //Adding headers  
            AddCellToHeader(tableLayout1, "EmpId");
            AddCellToHeader(tableLayout1, "Name");
            AddCellToHeader(tableLayout1, "Designation");
            AddCellToHeader(tableLayout1, "Branch");
            AddCellToHeader(tableLayout1, "StartDate");
            AddCellToHeader(tableLayout1, "EndDate");
            AddCellToHeader(tableLayout1, "Type");
            AddCellToHeader(tableLayout1, "Reason");
            AddCellToHeader(tableLayout1, "Status");
            //Adding body  
            foreach (var lemp in data)
            {
                AddCellToBody(tableLayout1, lemp.EmpId);
                AddCellToBody(tableLayout1, lemp.ShortName);
                AddCellToBody(tableLayout1, lemp.Designation);
                AddCellToBody(tableLayout1, lemp.Deptbranch);
                AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.Code);
                AddCellToBody(tableLayout1, lemp.Reason.ToString());
                AddCellToBody(tableLayout1, lemp.Status);

            }
            return tableLayout1;
        }
        // Create PDF File For Emp All Leaves
        public FileResult CreatePdfAllLeaves()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("AllLeaves" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 10 columns  
            PdfPTable tableLayout1 = new PdfPTable(10);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF16(tableLayout1));
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF16(PdfPTable tableLayout1)
        {
            float[] headers1 = { 33, 53, 45, 39, 55, 55, 55, 42, 38, 46 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("EmpAllLeaves", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lemployee = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            var lleave = db.Leaves.ToList();
            var leavetypes = db.LeaveTypes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
            var data = (from leaves in lleave
                        join emp in lemployee on leaves.EmpId equals emp.Id
                        join branchs in lbranch on emp.Branch equals branchs.Id
                        join depart in ldepartments on emp.Department equals depart.Id
                        join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                        join ltype in leavetypes on leaves.LeaveType equals ltype.Id
                        select new
                        {
                            emp.EmpId,
                            emp.ShortName,
                            Designation = desig.Code,
                            Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                            leaves.StartDate,
                            leaves.EndDate,
                            leaves.UpdatedDate,
                            ltype.Code,
                            leaves.Reason,
                            leaves.Status,
                        });
            //Adding headers  
            AddCellToHeader(tableLayout1, "EmpId");
            AddCellToHeader(tableLayout1, "Name");
            AddCellToHeader(tableLayout1, "Designation");
            AddCellToHeader(tableLayout1, "Branch");
            AddCellToHeader(tableLayout1, "AppliedDate");
            AddCellToHeader(tableLayout1, "StartDate");
            AddCellToHeader(tableLayout1, "EndDate");
            AddCellToHeader(tableLayout1, "Type");
            AddCellToHeader(tableLayout1, "Reason");
            AddCellToHeader(tableLayout1, "Status");
            //Adding body  
            foreach (var lemp in data)
            {
                AddCellToBody(tableLayout1, lemp.EmpId);
                AddCellToBody(tableLayout1, lemp.ShortName);
                AddCellToBody(tableLayout1, lemp.Designation);
                AddCellToBody(tableLayout1, lemp.Deptbranch);
                AddCellToBody(tableLayout1, lemp.UpdatedDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.Code);
                AddCellToBody(tableLayout1, lemp.Reason.ToString());
                AddCellToBody(tableLayout1, lemp.Status);

            }
            return tableLayout1;
        }
        // Create PDF File For Employees List
        public FileResult CreatePdfEmployee()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("EmployeeList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout1 = new PdfPTable(5);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF17(tableLayout1));
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF17(PdfPTable tableLayout1)
        {
            float[] headers1 = { 30, 45, 35, 50, 60 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("EmployeeList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lemployee = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            DateTime lstartdate = GetCurrentTime(DateTime.Now).Date;
            var data = (from emp in lemployee
                        join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                        join branch in lbranch on emp.Branch equals branch.Id
                        join dept in ldepartments on emp.Department equals dept.Id
                        where emp.RetirementDate >= lstartdate
                        select new
                        {
                            emp.EmpId,
                            emp.ShortName,
                            Designation = desig.Code,
                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                            EmpName = emp.FirstName + "  " + emp.LastName,
                            desigId = desig.Id,
                        }).OrderBy(a => a.desigId);
            //Adding headers  
            AddCellToHeader(tableLayout1, "EmpId");
            AddCellToHeader(tableLayout1, "Name");
            AddCellToHeader(tableLayout1, "Designation");
            AddCellToHeader(tableLayout1, "Branch");
            AddCellToHeader(tableLayout1, "EmpFullName");
            //Adding body  
            foreach (var lemp in data)
            {
                AddCellToBody(tableLayout1, lemp.EmpId);
                AddCellToBody(tableLayout1, lemp.ShortName);
                AddCellToBody(tableLayout1, lemp.Designation);
                AddCellToBody(tableLayout1, lemp.Deptbranch);
                AddCellToBody(tableLayout1, lemp.EmpName);

            }
            return tableLayout1;
        }
        // Create PDF File For Head Office List
        public FileResult CreatePdfDept()
        {
            string ldept = Convert.ToString(Session["ldept"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("HeadOfficeList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 4 columns  
            PdfPTable tableLayout1 = new PdfPTable(4);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF18(tableLayout1, ldept));
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
            Session.Remove("ldept");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF18(PdfPTable tableLayout1, string dept)
        {
            float[] headers1 = { 30, 50, 30, 70 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("HeadOfficeList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var dbResult = db.view_employee_dept.ToList();
            var lResult = db.Employes.ToList();
            var designation = db.Designations.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (dept == "")
            {
                var data = (from emplist in dbResult
                            join elist in lResult on emplist.EmpId equals elist.EmpId
                            join dlist in designation on emplist.Code equals dlist.Code
                            where emplist.DeptName != "OtherDepartment"
                            where elist.RetirementDate >= lStartDate
                            select new
                            {
                                emplist.EmpId,
                                Name = emplist.EmpName,
                                designation = emplist.Code,
                                Deptbranch = emplist.DeptName,
                                desigId = dlist.Id
                            }).OrderBy(a => a.desigId);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.Name);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                }


                return tableLayout1;
            }
            else
            {
                int ldepartment = Convert.ToInt32(dept);
                var data = (from emplist in dbResult
                            join elist in lResult on emplist.EmpId equals elist.EmpId
                            where emplist.DepartmentId == ldepartment
                            where elist.RetirementDate >= lStartDate
                            select new
                            {
                                emplist.Id,
                                emplist.EmpId,
                                Name = emplist.EmpName,
                                designation = emplist.Code,
                                Deptbranch = emplist.DeptName
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.Name);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                }


                return tableLayout1;
            }
        }
        // Create PDF File For All OD List
        public FileResult CreatePdfAllOD()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("ODList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            //Document doc = new Document();
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 11 columns  
            PdfPTable tableLayout1 = new PdfPTable(11);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF21(tableLayout1));
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF21(PdfPTable tableLayout1)
        {
            float[] headers1 = { 40, 75, 50, 50, 50, 50, 70, 70, 50, 50, 70 }; //Header Widths  
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
            var data = (from otherduty in ldeputation
                        join emp in lemployees on otherduty.EmpId equals emp.Id
                        join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                        join dept in Departments on emp.Department equals dept.Id
                        join branch in lBranches on emp.Branch equals branch.Id
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
                AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.UpdatedDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.VistorFrom);
                AddCellToBody(tableLayout1, lemp.VistorTo);
                AddCellToBody(tableLayout1, lemp.ODType);
                AddCellToBody(tableLayout1, lemp.Status);
                AddCellToBody(tableLayout1, lemp.Description);
            }
            return tableLayout1;
        }
        // Create PDF File For Today OD List
        public FileResult CreatePdftodayOD()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("ODList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            //Document doc = new Document();
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 11 columns  
            PdfPTable tableLayout1 = new PdfPTable(11);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF22(tableLayout1));
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(byteInfo);
                Font blackFont = FontFactory.GetFont("HELVETICA", 8, Font.NORMAL, BaseColor.BLACK);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase(i.ToString(), blackFont), 470f, 11f, 0);
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
        protected PdfPTable Add_Content_To_PDF22(PdfPTable tableLayout1)
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
            DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
            var data = (from otherduty in ldeputation
                        join emp in lemployees on otherduty.EmpId equals emp.Id
                        join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                        join dept in Departments on emp.Department equals dept.Id
                        join branch in lBranches on emp.Branch equals branch.Id
                        join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                        join od in ltype on otherduty.Purpose equals od.Id
                        where (lStartDate >= otherduty.StartDate.Date && lStartDate <= otherduty.EndDate.Date)
                        || (lEndDate >= otherduty.StartDate.Date && lEndDate <= otherduty.EndDate.Date)
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
                AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.UpdatedDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.VistorFrom);
                AddCellToBody(tableLayout1, lemp.VistorTo);
                AddCellToBody(tableLayout1, lemp.ODType);
                AddCellToBody(tableLayout1, lemp.Status);
                AddCellToBody(tableLayout1, lemp.Description);
            }
            return tableLayout1;
        }
        // Create PDF File For Staff Master List
        public FileResult CreatePdfSM()
        {
            string lempid = Convert.ToString(Session["lempid"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("StaffMasterList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 15 columns  
            PdfPTable tableLayout1 = new PdfPTable(15);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF23(tableLayout1, lempid));
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
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page:" + i.ToString() + "/" + pages.ToString(), blackFont), 470f, 12f, 0);
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
            Session.Remove("lempid");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF23(PdfPTable tableLayout1, string empid)
        {
            float[] headers1 = { 12, 26, 15, 15, 15, 15, 15, 20, 10, 15, 15, 15, 15, 15, 15 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("StaffMasterList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var dbResult = db.view_employee_transfer.ToList();
            var branch = db.Branches.ToList();
            var department = db.Departments.ToList();
            var designations = db.Designations.ToList();
            var lResult = db.Employes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (empid == "")
            {
                var data = (from emplist in dbResult
                            join elist in lResult on emplist.EmpId equals elist.EmpId
                            where elist.RetirementDate >= lStartDate
                            select new
                            {
                                emplist.EmpId,
                                emplist.EmpName,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                emplist.Designation,
                                emplist.FatherName,
                                emplist.MotherName,
                                emplist.DOB,
                                emplist.DOJ,
                                emplist.RetirementDate,
                                emplist.PresentAddress,
                                emplist.ProfessionalQualifications,
                                emplist.Graduation,
                                emplist.PostGraduation,
                                emplist.MobileNumber,
                                emplist.Category,
                                OldDesignation = GetOldDesignationsTransferValues(emplist.OldDesignation),
                                NewDesignation = GetNewDesignationsTransferValues(emplist.NewDesignation),
                                oldDepartment = GetBranchDepartmentConcat(GetOldTransferValues(emplist.OldBranch), GetNewTransferValues(emplist.OldDepartment)),
                                NewDepartment = GetBranchDepartmentConcat(GetOldTransferValues(emplist.NewBranch), GetNewTransferValues(emplist.NewDepartment)),
                                emplist.EffectiveFrom,
                                emplist.EffectiveTo,
                                emplist.Type
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "FatherName");
                AddCellToHeader(tableLayout1, "MotherName");
                AddCellToHeader(tableLayout1, "DOB");
                AddCellToHeader(tableLayout1, "DOJ");
                AddCellToHeader(tableLayout1, "EmpAddress");
                AddCellToHeader(tableLayout1, "Category");
                AddCellToHeader(tableLayout1, "Graduation");
                AddCellToHeader(tableLayout1, "PostGraduation");
                AddCellToHeader(tableLayout1, "OldDesignation");
                AddCellToHeader(tableLayout1, "NewDesignation");
                AddCellToHeader(tableLayout1, "OldDepartment");
                AddCellToHeader(tableLayout1, "NewDepartment");
                AddCellToHeader(tableLayout1, "Type");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.FatherName);
                    AddCellToBody(tableLayout1, lemp.MotherName);
                    AddCellToBody(tableLayout1, lemp.DOB.Value.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.DOJ.Value.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.PresentAddress);
                    AddCellToBody(tableLayout1, lemp.Category);
                    AddCellToBody(tableLayout1, lemp.Graduation);
                    AddCellToBody(tableLayout1, lemp.PostGraduation);
                    AddCellToBody(tableLayout1, lemp.OldDesignation);
                    AddCellToBody(tableLayout1, lemp.NewDesignation);
                    AddCellToBody(tableLayout1, lemp.oldDepartment);
                    AddCellToBody(tableLayout1, lemp.NewDepartment);
                    AddCellToBody(tableLayout1, lemp.Type);
                }
                return tableLayout1;
            }
            else
            {
                var data = (from emplist in dbResult
                            join elist in lResult on emplist.EmpId equals elist.EmpId
                            where elist.RetirementDate >= lStartDate
                            where emplist.EmpId == empid || emplist.EmpName == empid || emplist.Designation == empid
                            select new
                            {
                                emplist.EmpId,
                                emplist.EmpName,
                                BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                emplist.Designation,
                                emplist.FatherName,
                                emplist.MotherName,
                                emplist.DOB,
                                emplist.DOJ,
                                emplist.RetirementDate,
                                emplist.PresentAddress,
                                emplist.ProfessionalQualifications,
                                emplist.Graduation,
                                emplist.PostGraduation,
                                emplist.MobileNumber,
                                emplist.Category,
                                OldDesignation = GetOldDesignationsTransferValues(emplist.OldDesignation),
                                NewDesignation = GetNewDesignationsTransferValues(emplist.NewDesignation),
                                oldDepartment = GetBranchDepartmentConcat(GetOldTransferValues(emplist.OldBranch), GetNewTransferValues(emplist.OldDepartment)),
                                NewDepartment = GetBranchDepartmentConcat(GetOldTransferValues(emplist.NewBranch), GetNewTransferValues(emplist.NewDepartment)),
                                emplist.EffectiveFrom,
                                emplist.EffectiveTo,
                                emplist.Type
                            });
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "FatherName");
                AddCellToHeader(tableLayout1, "MotherName");
                AddCellToHeader(tableLayout1, "DOB");
                AddCellToHeader(tableLayout1, "DOJ");
                AddCellToHeader(tableLayout1, "EmpAddress");
                AddCellToHeader(tableLayout1, "Category");
                AddCellToHeader(tableLayout1, "Graduation");
                AddCellToHeader(tableLayout1, "PostGraduation");
                AddCellToHeader(tableLayout1, "OldDesignation");
                AddCellToHeader(tableLayout1, "NewDesignation");
                AddCellToHeader(tableLayout1, "OldDepartment");
                AddCellToHeader(tableLayout1, "NewDepartment");
                AddCellToHeader(tableLayout1, "Type");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmpName);
                    AddCellToBody(tableLayout1, lemp.FatherName);
                    AddCellToBody(tableLayout1, lemp.MotherName);
                    AddCellToBody(tableLayout1, lemp.DOB.Value.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.DOJ.Value.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.PresentAddress);
                    AddCellToBody(tableLayout1, lemp.Category);
                    AddCellToBody(tableLayout1, lemp.Graduation);
                    AddCellToBody(tableLayout1, lemp.PostGraduation);
                    AddCellToBody(tableLayout1, lemp.OldDesignation);
                    AddCellToBody(tableLayout1, lemp.NewDesignation);
                    AddCellToBody(tableLayout1, lemp.oldDepartment);
                    AddCellToBody(tableLayout1, lemp.NewDepartment);
                    AddCellToBody(tableLayout1, lemp.Type);
                }
                return tableLayout1;
            }
        }
        //Code For Export To Excel in Staff Master List
        public void Button1_Click(object sender, EventArgs e, string EmpId)
        {
            string lEmpId = Session["lempid"].ToString();
            ExportGridToExcel(lEmpId);
        }
        public void ExportGridToExcel(string EmpId)
        {
            try
            {
                var dbResult = db.view_employee_transfer.ToList();
                var branch = db.Branches.ToList();
                var department = db.Departments.ToList();
                var designations = db.Designations.ToList();
                var lResult = db.Employes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var employeeList = (from emplist in dbResult
                                        join elist in lResult on emplist.EmpId equals elist.EmpId
                                        where elist.RetirementDate >= lStartDate &&emplist.EmpId==EmpId
                                        select new
                                        {
                                            emplist.EmpId,
                                            emplist.EmpName,
                                            BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                            emplist.Designation,
                                            emplist.FatherName,
                                            emplist.MotherName,
                                            emplist.DOB,
                                            emplist.DOJ,
                                            emplist.RetirementDate,
                                            emplist.PresentAddress,
                                            emplist.ProfessionalQualifications,
                                            emplist.Graduation,
                                            emplist.PostGraduation,
                                            emplist.MobileNumber,
                                            emplist.Category,
                                            OldDesignation = GetOldDesignationsTransferValues(emplist.OldDesignation),
                                            NewDesignation = GetNewDesignationsTransferValues(emplist.NewDesignation),
                                            oldDepartment = GetBranchDepartmentConcat(GetOldTransferValues(emplist.OldBranch), GetNewTransferValues(emplist.OldDepartment)),
                                            NewDepartment = GetBranchDepartmentConcat(GetOldTransferValues(emplist.NewBranch), GetNewTransferValues(emplist.NewDepartment)),
                                            emplist.EffectiveFrom,
                                            emplist.EffectiveTo,
                                            emplist.Type
                                        }).ToList();
                    var gv = new GridView();
                    gv.DataSource = employeeList;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=StaffMasterList.xls");
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
                }
                else
                {
                    var employeeList = (from emplist in dbResult
                                        join elist in lResult on emplist.EmpId equals elist.EmpId
                                        where (emplist.EmpId.ToLower().Contains(EmpId.ToLower()) || (emplist.EmpName.ToLower().Contains(EmpId.ToLower())) || (emplist.Designation.ToString().ToLower().Contains(EmpId.ToLower())))
                                        where elist.RetirementDate >= lStartDate
                                        select new
                                        {
                                            emplist.EmpId,
                                            emplist.EmpName,
                                            BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                            emplist.Designation,
                                            emplist.FatherName,
                                            emplist.MotherName,
                                            emplist.DOB,
                                            emplist.DOJ,
                                            emplist.RetirementDate,
                                            emplist.PresentAddress,
                                            emplist.ProfessionalQualifications,
                                            emplist.Graduation,
                                            emplist.PostGraduation,
                                            emplist.MobileNumber,
                                            emplist.Category,
                                            OldDesignation = GetOldDesignationsTransferValues(emplist.OldDesignation),
                                            NewDesignation = GetNewDesignationsTransferValues(emplist.NewDesignation),
                                            oldDepartment = GetBranchDepartmentConcat(GetOldTransferValues(emplist.OldBranch), GetNewTransferValues(emplist.OldDepartment)),
                                            NewDepartment = GetBranchDepartmentConcat(GetOldTransferValues(emplist.NewBranch), GetNewTransferValues(emplist.NewDepartment)),
                                            emplist.EffectiveFrom,
                                            emplist.EffectiveTo,
                                            emplist.Type
                                        }).ToList();
                    var gv = new GridView();
                    gv.DataSource = employeeList;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=StaffMasterList.xls");
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
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        // Creating PDF for Year Wise Leave Balance
        public FileResult CreatePdfYearwiseReport()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("YearWiseLeaveBalanceReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout1 = new PdfPTable(28);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF25(tableLayout1));
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF25(PdfPTable tableLayout1)
        {
            float[] headers1 = { 15,10,10,10,10,10,10,10,10,10,10,10,15,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10 }; //Header Widths  
            tableLayout1.SetWidths(headers1); //Set the pdf headers  
            tableLayout1.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout1.HeaderRows = 1;
            //tableLayout1.AddCell(new PdfPCell(new Phrase("LC-Leave Credit | LD-Leave Debit | LB-Leave Balance | CF-Carry Forward 1-CL 2-ML 3-PL 4-MTL 5-PTL 6-EOL 7-SCL", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            //{
            //    Colspan = 20,
            //    Border = 0,
            //    PaddingBottom = 5,
            //    HorizontalAlignment = Element.ALIGN_CENTER,
            //});
            var lYearbalance = db.V_EmpLeavesCarryForward.ToList();
            var lRemainBalance = db.V_EmpLeaveBalance.ToList();
            var lemployee = db.Employes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var data = (from lyearbalance in lYearbalance
                        join lrembalance in lRemainBalance on lyearbalance.EmpId equals lrembalance.EmpId
                        join emp in lemployee on lyearbalance.EmpId equals emp.Id
                        where emp.RetirementDate >= lStartDate
                        select new
                        {
                            EmpId = emp.EmpId,
                            EmployeeName = GetFirstandLastName(emp.FirstName, emp.LastName),
                            Dept = GetBranchDepartmentConcat(lyearbalance.DeptName, lyearbalance.BranchName),
                            Designations = lyearbalance.DesignationName,
                            Year = lyearbalance.Year,

                            TotalBalanceCL = lyearbalance.CasualLeave,
                            RemainingCL = TotalAppliedLeaves(lyearbalance.CasualLeave, 1, lyearbalance.EmpId),
                            TotalAppliedCL = ConsumedLeaves(1, lyearbalance.EmpId),
                            CLCarryForward = lyearbalance.CarryForward,

                            TotalBalanceML = lyearbalance.MedicalSickLeave,
                            RemainingML = TotalAppliedLeaves(lyearbalance.MedicalSickLeave, 2, lyearbalance.EmpId),
                            TotalAppliedML = ConsumedLeaves(2, lyearbalance.EmpId),
                            MLCarryForward = lyearbalance.CarryForward,

                            TotalBalancePL = lyearbalance.PrivilegeLeave,
                            RemainingPL = TotalAppliedLeaves(lyearbalance.PrivilegeLeave, 3, lyearbalance.EmpId),
                            TotalAppliedPL = ConsumedLeaves(3, lyearbalance.EmpId),
                            PLCarryForward = lyearbalance.CarryForward,

                            TotalBalanceMTL = lyearbalance.MaternityLeave,
                            RemainingMTL = TotalAppliedLeaves(lyearbalance.MaternityLeave, 4, lyearbalance.EmpId),
                            TotalAppliedMTL = ConsumedLeaves(4, lyearbalance.EmpId),
                            MTLCarryForward = lyearbalance.CarryForward,

                            TotalBalancePTL = lyearbalance.PaternityLeave,
                            RemainingPTL = TotalAppliedLeaves(lyearbalance.PaternityLeave, 5, lyearbalance.EmpId),
                            TotalAppliedPTL = ConsumedLeaves(5, lyearbalance.EmpId),
                            PTLCarryForward = lyearbalance.CarryForward,

                            TotalBalanceEOL = lyearbalance.ExtraOrdinaryLeave,
                            RemainingEOL = TotalAppliedLeaves(lyearbalance.ExtraOrdinaryLeave, 6, lyearbalance.EmpId),
                            TotalAppliedEOL = ConsumedLeaves(6, lyearbalance.EmpId),
                            EOLCarryForward = lyearbalance.CarryForward,

                            TotalBalanceSCL = lyearbalance.SpecialCasualLeave,
                            RemainingSCL = TotalAppliedLeaves(lyearbalance.SpecialCasualLeave, 7, lyearbalance.EmpId),
                            TotalAppliedSCL = ConsumedLeaves(7, lyearbalance.EmpId),
                            SCLCarryForward = lyearbalance.CarryForward,
                        });
            //Adding headers  
            AddCellToHeader(tableLayout1, "EmpId");

            AddCellToHeader(tableLayout1, "LC");
            AddCellToHeader(tableLayout1, "LB");
            AddCellToHeader(tableLayout1, "LD");
          

            AddCellToHeader(tableLayout1, "CF");
            AddCellToHeader(tableLayout1, " LC");
            AddCellToHeader(tableLayout1, "LB");
            AddCellToHeader(tableLayout1, "LD");

            AddCellToHeader(tableLayout1, "CF");
            AddCellToHeader(tableLayout1, " LC");
            AddCellToHeader(tableLayout1, "LB");
            AddCellToHeader(tableLayout1, "LD");

            AddCellToHeader(tableLayout1, "CF");
            AddCellToHeader(tableLayout1, " LC");
            AddCellToHeader(tableLayout1, "LB");
            AddCellToHeader(tableLayout1, "LD");


            AddCellToHeader(tableLayout1, "CF");
            AddCellToHeader(tableLayout1, " LC");
            AddCellToHeader(tableLayout1, "LB");
            AddCellToHeader(tableLayout1, "LD");


            AddCellToHeader(tableLayout1, "CF");
            AddCellToHeader(tableLayout1, " LC");
            AddCellToHeader(tableLayout1, "LB");
            AddCellToHeader(tableLayout1, "LD");


            AddCellToHeader(tableLayout1, "CF");
            AddCellToHeader(tableLayout1, "LC");
            AddCellToHeader(tableLayout1, "LB");
            AddCellToHeader(tableLayout1, "LD");

            foreach (var lemp in data)
            {
                AddCellToBody(tableLayout1, lemp.EmpId.ToString());
                //AddCellToBody(tableLayout1, lemp.EmployeeName.ToString());
                //AddCellToBody(tableLayout1, lemp.Dept.ToString());
                //AddCellToBody(tableLayout1, lemp.Designations.ToString());
                //AddCellToBody(tableLayout1, lemp.Year.ToString());

                AddCellToBody(tableLayout1, lemp.TotalBalanceCL.ToString());
                AddCellToBody(tableLayout1, lemp.RemainingCL.ToString());
                AddCellToBody(tableLayout1, lemp.TotalAppliedCL.ToString());
                //AddCellToBody(tableLayout1, lemp.CLCarryForward.ToString());

                AddCellToBody(tableLayout1, lemp.TotalBalanceML.ToString());
                AddCellToBody(tableLayout1, lemp.RemainingML.ToString());
                AddCellToBody(tableLayout1, lemp.TotalAppliedML.ToString());
                AddCellToBody(tableLayout1, lemp.MLCarryForward.ToString());

                AddCellToBody(tableLayout1, lemp.TotalBalancePL.ToString());
                AddCellToBody(tableLayout1, lemp.RemainingPL.ToString());
                AddCellToBody(tableLayout1, lemp.TotalAppliedPL.ToString());
                AddCellToBody(tableLayout1, lemp.PLCarryForward.ToString());


                AddCellToBody(tableLayout1, lemp.TotalBalanceMTL.ToString());
                AddCellToBody(tableLayout1, lemp.RemainingMTL.ToString());
                AddCellToBody(tableLayout1, lemp.TotalAppliedMTL.ToString());
                AddCellToBody(tableLayout1, lemp.MTLCarryForward.ToString());

                AddCellToBody(tableLayout1, lemp.TotalBalancePTL.ToString());
                AddCellToBody(tableLayout1, lemp.RemainingPTL.ToString());
                AddCellToBody(tableLayout1, lemp.TotalAppliedPTL.ToString());
                AddCellToBody(tableLayout1, lemp.PTLCarryForward.ToString());

                AddCellToBody(tableLayout1, lemp.TotalBalanceEOL.ToString());
                AddCellToBody(tableLayout1, lemp.RemainingEOL.ToString());
                AddCellToBody(tableLayout1, lemp.TotalAppliedEOL.ToString());
                AddCellToBody(tableLayout1, lemp.EOLCarryForward.ToString());

                AddCellToBody(tableLayout1, lemp.TotalBalanceSCL.ToString());
                AddCellToBody(tableLayout1, lemp.RemainingSCL.ToString());
                AddCellToBody(tableLayout1, lemp.TotalAppliedSCL.ToString());
                AddCellToBody(tableLayout1, lemp.SCLCarryForward.ToString());
            }
            return tableLayout1;
        }
        //Code For Export To Excel in Year Wise Leave Balance
        public void Button1_ClickYear(object sender, EventArgs e)
        {
            ExportGridToExcelYear();
        }
        public void ExportGridToExcelYear()
        {

            try
            {
                var lYearbalance = db.V_EmpLeavesCarryForward.ToList();
                var lRemainBalance = db.V_EmpLeaveBalance.ToList();
                var lemployee = db.Employes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var employeeList = (from lyearbalance in lYearbalance
                                    join lrembalance in lRemainBalance on lyearbalance.EmpId equals lrembalance.EmpId
                                    join emp in lemployee on lyearbalance.EmpId equals emp.Id
                                    where emp.RetirementDate >= lStartDate
                                    select new
                                    {
                                        EmpId = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        Dept = GetBranchDepartmentConcat(lyearbalance.DeptName, lyearbalance.BranchName),
                                        Designations = lyearbalance.DesignationName,
                                        Year = lyearbalance.Year,

                                        TotalBalanceCL = lyearbalance.CasualLeave,
                                        RemainingCL = TotalAppliedLeaves(lyearbalance.CasualLeave, 1, lyearbalance.EmpId),
                                        TotalAppliedCL = ConsumedLeaves(1, lyearbalance.EmpId),
                                        CLCarryForward = lyearbalance.CarryForward,

                                        TotalBalanceML = lyearbalance.MedicalSickLeave,
                                        RemainingML = TotalAppliedLeaves(lyearbalance.MedicalSickLeave, 2, lyearbalance.EmpId),
                                        TotalAppliedML = ConsumedLeaves(2, lyearbalance.EmpId),
                                        MLCarryForward = lyearbalance.CarryForward,

                                        TotalBalancePL = lyearbalance.PrivilegeLeave,
                                        RemainingPL = TotalAppliedLeaves(lyearbalance.PrivilegeLeave, 3, lyearbalance.EmpId),
                                        TotalAppliedPL = ConsumedLeaves(3, lyearbalance.EmpId),
                                        PLCarryForward = lyearbalance.CarryForward,

                                        TotalBalanceMTL = lyearbalance.MaternityLeave,
                                        RemainingMTL = TotalAppliedLeaves(lyearbalance.MaternityLeave, 4, lyearbalance.EmpId),
                                        TotalAppliedMTL = ConsumedLeaves(4, lyearbalance.EmpId),
                                        MTLCarryForward = lyearbalance.CarryForward,

                                        TotalBalancePTL = lyearbalance.PaternityLeave,
                                        RemainingPTL = TotalAppliedLeaves(lyearbalance.PaternityLeave, 5, lyearbalance.EmpId),
                                        TotalAppliedPTL = ConsumedLeaves(5, lyearbalance.EmpId),
                                        PTLCarryForward = lyearbalance.CarryForward,

                                        TotalBalanceEOL = lyearbalance.ExtraOrdinaryLeave,
                                        RemainingEOL = TotalAppliedLeaves(lyearbalance.ExtraOrdinaryLeave, 6, lyearbalance.EmpId),
                                        TotalAppliedEOL = ConsumedLeaves(6, lyearbalance.EmpId),
                                        EOLCarryForward = lyearbalance.CarryForward,

                                        TotalBalanceSCL = lyearbalance.SpecialCasualLeave,
                                        RemainingSCL = TotalAppliedLeaves(lyearbalance.SpecialCasualLeave, 7, lyearbalance.EmpId),
                                        TotalAppliedSCL = ConsumedLeaves(7, lyearbalance.EmpId),
                                        SCLCarryForward = lyearbalance.CarryForward,
                                    }).ToList();

                var gv = new GridView();
                gv.DataSource = employeeList;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=YearWiseReport.xls");
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
        // Create PDF File for Cancel Report
        public FileResult CreatePdfCancel()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("CancelReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(10f, 10f, 10f, 10f);
            PdfPTable tableLayout1 = new PdfPTable(10);
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            doc.Add(Add_Content_To_PDF28(tableLayout1));
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
                        //tableLayout1.FooterRows = 1;
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
        protected PdfPTable Add_Content_To_PDF28(PdfPTable tableLayout1)
        {
            float[] headers1 = { 20, 45, 30, 30, 30, 30, 45, 45,45,70 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("CancelReport", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lleaves = db.Leaves.ToList();
            var lBranches = db.Branches.ToList();
            var lLeaveTypes = db.LeaveTypes.ToList();
            var Departments = db.Departments.ToList();
            var lemployees = db.Employes.ToList();
            var ldesignation = db.Designations.ToList();
            var lResults = (from leave in lleaves
                            join leavetype in lLeaveTypes on leave.LeaveType equals leavetype.Id
                            join emp in lemployees on leave.EmpId equals emp.Id
                            join branch1 in lBranches on emp.Branch equals branch1.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join dept in Departments on emp.Department equals dept.Id
                            join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                            join emp2 in lemployees on leave.UpdatedBy equals emp2.EmpId
                            where leave.Status == "Cancelled"
                            select new
                            {
                                leave.Id,
                                emp.EmpId,
                                EmployeeName = emp.ShortName,
                                CancelledBy = (leave.UpdatedBy + '-' + GetFirstandLastName(emp2.FirstName, emp2.LastName)),
                                leave.UpdatedDate,
                                CancelTime = GetAppliedTime(leave.UpdatedDate),
                                StartDate = leave.StartDate,
                                EndDate = leave.EndDate,
                                Stage = leave.Stage,
                                Status = leave.Status,
                                Reason = leave.CancelReason,
                            });
            //Adding headers  
            AddCellToHeader(tableLayout1, "EmpId");
            AddCellToHeader(tableLayout1, "EmpName");
            AddCellToHeader(tableLayout1, "StartDate");
            AddCellToHeader(tableLayout1, "EndDate");
            AddCellToHeader(tableLayout1, "Status");
            AddCellToHeader(tableLayout1, "Stage");
            AddCellToHeader(tableLayout1, "CancelledBy");
            AddCellToHeader(tableLayout1, "CancelledDate");
            AddCellToHeader(tableLayout1, "CancelledTime");
            AddCellToHeader(tableLayout1, "Reason for Cancellation");
            //Adding body  
            foreach (var lemp in lResults)
            {
                AddCellToBody(tableLayout1, lemp.EmpId);
                AddCellToBody(tableLayout1, lemp.EmployeeName);
                AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.Status);
                AddCellToBody(tableLayout1, lemp.Stage);
                AddCellToBody(tableLayout1, lemp.CancelledBy);
                AddCellToBody(tableLayout1, lemp.UpdatedDate.ToString("dd/MM/yyyy"));
                AddCellToBody(tableLayout1, lemp.CancelTime);
                AddCellToBody(tableLayout1, lemp.Reason);
            }
            return tableLayout1;
        }

        //Create PDF File for Leave Approval List
        public FileResult CreatePdfApproval()
        {
            string lcategory = Convert.ToString(Session["branch"]);
            string lempid = Convert.ToString(Session["branch2"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("ApprovalList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout1 = new PdfPTable(10);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF28(tableLayout1, lcategory, lempid));
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
            Session.Remove("branch");
            Session.Remove("branch2");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDF28(PdfPTable tableLayout1, string category, string empid)
        {

            float[] headers1 = { 15, 40, 25, 30, 25, 30, 30, 30, 45, 30 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("ApprovalList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            if (category == "" || empid == "")
            {
                var lleaves = db.Leaves.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                var data = (from leave in lleaves
                            join emp in lemployees on leave.EmpId equals emp.Id
                            join branch in lBranches on emp.Branch equals branch.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join dept in Departments on emp.Department equals dept.Id
                            join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                            join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                            where leave.Status == "Approved" || leave.Status == "Forwarded"
                            select new
                            {
                                emp.EmpId,
                                EmployeeName = emp.ShortName,
                                designation = desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                leave.Id,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.ApprovedTime);

                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Code");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "Authority");
                AddCellToHeader(tableLayout1, "ApprovedBy");
                AddCellToHeader(tableLayout1, "ApprovedTime");
                AddCellToHeader(tableLayout1, "Subject");
                AddCellToHeader(tableLayout1, "Reason");
                AddCellToHeader(tableLayout1, "Status");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmployeeName);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                    AddCellToBody(tableLayout1, lemp.Authority);
                    AddCellToBody(tableLayout1, lemp.ApprovedBy);
                    AddCellToBody(tableLayout1, lemp.ApprovedTime);
                    AddCellToBody(tableLayout1, lemp.Subject);
                    AddCellToBody(tableLayout1, lemp.Reason);
                    AddCellToBody(tableLayout1, lemp.Status);
                }
            }
            else if (category != "" && empid != "")
            {
                var lleaves = db.Leaves.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                var data = (from leave in lleaves
                            join emp in lemployees on leave.EmpId equals emp.Id
                            join branch in lBranches on emp.Branch equals branch.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join dept in Departments on emp.Department equals dept.Id
                            join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                            join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                            where leave.Status == category
                            where emp2.EmpId == empid || emp1.EmpId == empid
                            select new
                            {
                                emp.EmpId,
                                EmployeeName = emp.ShortName,
                                designation = desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                leave.Id,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.ApprovedTime);

                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Code");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "Authority");
                AddCellToHeader(tableLayout1, "ApprovedBy");
                AddCellToHeader(tableLayout1, "ApprovedTime");
                AddCellToHeader(tableLayout1, "Subject");
                AddCellToHeader(tableLayout1, "Reason");
                AddCellToHeader(tableLayout1, "Status");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.EmployeeName);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                    AddCellToBody(tableLayout1, lemp.Authority);
                    AddCellToBody(tableLayout1, lemp.ApprovedBy);
                    AddCellToBody(tableLayout1, lemp.ApprovedTime);
                    AddCellToBody(tableLayout1, lemp.Subject);
                    AddCellToBody(tableLayout1, lemp.Reason);
                    AddCellToBody(tableLayout1, lemp.Status);
                }
                return tableLayout1;
            }
            return tableLayout1;
        }
        // Report for Month Wise Leaves
        public ActionResult MonthWiseLeave()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/Monthwise_Leaves.cshtml");
        }
        [HttpGet]
        public JsonResult MonthWiseLeaveView(string StartDate)
        {
            try
            {
                var lleaves = db.Leaves.ToList();
                var lemp = db.Employes.ToList();
                var ltype = db.LeaveTypes.ToList();
                var branch = db.Branches.ToList();
                var Dept = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                var data = (from leave in lleaves
                            join emp in lemp on leave.EmpId equals emp.Id
                            join type in ltype on leave.LeaveType equals type.Id
                            join branchs in branch on emp.Branch equals branchs.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join dept in Dept on emp.Department equals dept.Id
                            select new
                            {
                                leave.Id,
                                emp.EmpId,
                                emp.ShortName,
                                designation = desig.Code,
                                UpdatedDate = leave.UpdatedDate.ToString("dd/MM/yyyy"),
                                AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                StartDate = leave.StartDate.ToString("dd/MM/yyyy"),
                                EndDate = leave.EndDate.ToString("dd/MM/yyyy"),
                                diffdays = Getdiffbetweendates(leave.StartDate, leave.EndDate),
                                type.Code,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.UpdatedDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string lMessage = ex.Message;
            }
            return null;
        }
        public string GetAppliedTime(DateTime lapplieddate)
        {
            string lApplied = "";
            DateTime d1 = lapplieddate;
            string lAppliedtime = d1.ToShortTimeString().ToString();
            lApplied = lAppliedtime;
            return lApplied;
        }
        [HttpPost]
        public JsonResult MonthWiseLeaveViews(string branch)
        {
            Session["lmonth"] = branch;
            try
            {
                if (branch == "")
                {
                    var lleaves = db.Leaves.ToList();
                    var lemp = db.Employes.ToList();
                    var ltype = db.LeaveTypes.ToList();
                    var branches = db.Branches.ToList();
                    var Dept = db.Departments.ToList();
                    var ldesignation = db.Designations.ToList();
                    var data = (from leave in lleaves
                                join emp in lemp on leave.EmpId equals emp.Id
                                join type in ltype on leave.LeaveType equals type.Id
                                join branchs in branches on emp.Branch equals branchs.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join dept in Dept on emp.Department equals dept.Id
                                select new
                                {
                                    leave.Id,
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    UpdatedDate = leave.UpdatedDate.ToString("dd/MM/yyyy"),
                                    AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                    ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                    StartDate = leave.StartDate.ToString("dd/MM/yyyy"),
                                    EndDate = leave.EndDate.ToString("dd/MM/yyyy"),
                                    diffdays = Getdiffbetweendates(leave.StartDate, leave.EndDate),
                                    type.Code,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
                                }).OrderByDescending(a => a.UpdatedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else if (branch != "")
                {
                    int M_no = Convert.ToInt32(branch);
                    int Y_no = DateTime.Now.Year;
                    var lleaves = db.Leaves.ToList();
                    var lemp = db.Employes.ToList();
                    var ltype = db.LeaveTypes.ToList();
                    var branches = db.Branches.ToList();
                    var Dept = db.Departments.ToList();
                    var ldesignation = db.Designations.ToList();
                    var data = (from leave in lleaves
                                join emp in lemp on leave.EmpId equals emp.Id
                                join type in ltype on leave.LeaveType equals type.Id
                                join branchs in branches on emp.Branch equals branchs.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join dept in Dept on emp.Department equals dept.Id
                                select new
                                {
                                    leave.Id,
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    UpdatedDate = leave.UpdatedDate.ToString("dd/MM/yyyy"),
                                    AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                    ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                    StartDate = leave.StartDate.ToString("dd/MM/yyyy"),
                                    EndDate = leave.EndDate.ToString("dd/MM/yyyy"),
                                    diffdays = Getdiffbetweendates(leave.StartDate, leave.EndDate),
                                    type.Code,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
                                }).OrderByDescending(a => a.UpdatedDate);
                    var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.StartDate).Month == M_no).Where(u => Convert.ToDateTime(u.StartDate).Year == Y_no)).ToList();
                    return Json(data1, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string lMessage = ex.Message;
            }
            return null;
        }
        public double GetdiffbetweendatesLeaves(DateTime Sd, DateTime Ed)
        {
            DateTime date1 = Sd.Date;
            DateTime date2 = Ed.Date;
            double NoOfDays = (date2 - date1).TotalDays;
            double Leavedays = NoOfDays + 1;
            return Leavedays;
        }
        //Report for Month wise OD
        public ActionResult MonthwiseOD()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/Monthwise_OD.cshtml");
        }

        [HttpGet]
        public JsonResult MonthwiseODView(string StartDate)
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
                               join branch in lBranches on emp.Branch equals branch.Id
                               join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                               join dept in Departments on emp.Department equals dept.Id
                               join type in ltype on otherduty.Purpose equals type.Id
                               orderby otherduty.UpdatedDate descending
                               select new
                               {

                                   otherduty.Id,
                                   emp.EmpId,
                                   EmpName = emp.ShortName,
                                   designation = desig.Code,
                                   VistorFrom = visitbran.Name,
                                   otherduty.VistorTo,
                                   UpdatedDate = otherduty.UpdatedDate.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                   StartDate = otherduty.StartDate,
                                   EndDate = otherduty.EndDate,
                                   Purpose = type.ODType,
                                   otherduty.Status,
                                   otherduty.Description,
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
        public JsonResult MonthwiseODViews(string branch)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            Session["lod"] = branch;
            if (branch == "")
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
                               join branchs in lBranches on emp.Branch equals branchs.Id
                               join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                               join dept in Departments on emp.Department equals dept.Id
                               join type in ltype on otherduty.Purpose equals type.Id
                               orderby otherduty.UpdatedDate descending
                               select new
                               {

                                   otherduty.Id,
                                   emp.EmpId,
                                   EmpName = emp.ShortName,
                                   designation = desig.Code,
                                   VistorFrom = visitbran.Name,
                                   otherduty.VistorTo,
                                   UpdatedDate = otherduty.UpdatedDate.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                   StartDate = otherduty.StartDate,
                                   EndDate = otherduty.EndDate,
                                   Purpose = type.ODType,
                                   otherduty.Status,
                                   otherduty.Description,
                               }).OrderByDescending(A => A.Id);
                return Json(lResult, JsonRequestBehavior.AllowGet);
            }
            else if (branch != "")
            {
                int M_no = Convert.ToInt32(branch);
                int Y_no = 2018;
                try
                {
                    var ltype = db.OD_Master.ToList();
                    var ldeputation = db.OD_OtherDuty.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var data = (from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id
                                join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                join branchs in lBranches on emp.Branch equals branchs.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join dept in Departments on emp.Department equals dept.Id
                                join type in ltype on otherduty.Purpose equals type.Id
                                orderby otherduty.UpdatedDate descending
                                select new
                                {
                                    otherduty.Id,
                                    emp.EmpId,
                                    EmpName = emp.ShortName,
                                    designation = desig.Code,
                                    VistorFrom = visitbran.Name,
                                    otherduty.VistorTo,
                                    UpdatedDate = otherduty.UpdatedDate.ToShortDateString(),
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                    StartDate = otherduty.StartDate,
                                    EndDate = otherduty.EndDate,
                                    Purpose = type.ODType,
                                    otherduty.Status,
                                    otherduty.Description,
                                }).OrderByDescending(A => A.Id);
                    var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.StartDate).Month == M_no).Where(u => Convert.ToDateTime(u.StartDate).Year == Y_no)).ToList();
                    return Json(data1, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    lMessage = ex.Message;
                }
                return null;
            }
            return null;
        }
        //Report for Month wise Temporary Transfer
        public ActionResult MonthwiseTempTransfer()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/Monthwise_TempTransfer.cshtml");
        }
        [HttpGet]
        public JsonResult MonthwiseTempTransView(string StartDate)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            try
            {
                var ltransfer = db.Employee_Transfer.ToList();
                var dbResult = db.Employes.ToList();
                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "TemporaryTransfer"
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {

                                transfer.Id,
                                EmpId = emplist.EmpId,
                                transfer.Type,
                                emplist.ShortName,
                                OldDesignation = desig.Code,
                                NewDesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,
                                transfer.EffectiveTo
                            }).OrderByDescending(a => a.EffectiveFrom);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }
        [HttpPost]
        public JsonResult MonthwiseTempTransViews(string branch)
        {
            Session["ltemp"] = branch;
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            if (branch == "")
            {
                var ltransfer = db.Employee_Transfer.ToList();
                var dbResult = db.Employes.ToList();
                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "TemporaryTransfer"
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {

                                transfer.Id,
                                EmpId = emplist.EmpId,
                                transfer.Type,
                                emplist.ShortName,
                                OldDesignation = desig.Code,
                                NewDesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,
                                transfer.EffectiveTo
                            }).OrderByDescending(a => a.EffectiveFrom);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (branch != "")
            {
                int M_no = Convert.ToInt32(branch);
                int Y_no = 2018;
                try
                {
                    var ltransfer = db.Employee_Transfer.ToList();
                    var dbResult = db.Employes.ToList();
                    var Branches = db.Branches.ToList();
                    var Departments = db.Departments.ToList();
                    var Designations = db.Designations.ToList();
                    DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                    var data = (from transfer in ltransfer
                                join emplist in dbResult on transfer.EmpId equals emplist.Id
                                join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                                join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                                join desig in Designations on transfer.OldDesignation equals desig.Id
                                join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                                join dept in Departments on transfer.OldDepartment equals dept.Id
                                join newdept in Departments on transfer.NewDepartment equals newdept.Id
                                where transfer.Type == "TemporaryTransfer"
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {

                                    transfer.Id,
                                    EmpId = emplist.EmpId,
                                    transfer.Type,
                                    emplist.ShortName,
                                    OldDesignation = desig.Code,
                                    NewDesignation = desig1.Code,
                                    oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                    newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                    transfer.EffectiveFrom,
                                    transfer.EffectiveTo
                                }).OrderByDescending(a => a.EffectiveFrom);

                    var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.EffectiveFrom).Month == M_no).Where(u => Convert.ToDateTime(u.EffectiveFrom).Year == Y_no)).ToList();
                    return Json(data1, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    lMessage = ex.Message;
                }
                return null;
            }
            return null;
        }
        //Report for Month Wise CL
        public ActionResult MonthwiseCl()

        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/Monthwise_CL.cshtml");
        }
        [HttpGet]
        public JsonResult MonthwiseClView(string EmpId)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            try
            {
                var lleave = db.Leaves.ToList();
                var lemp = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignations = db.Designations.ToList();
                var ltype = db.LeaveTypes.ToList();
                var lResult = (from leave in lleave
                               join emp in lemp on leave.EmpId equals emp.Id
                               join desig in ldesignations on emp.CurrentDesignation equals desig.Id
                               join department in ldept on emp.Department equals department.Id
                               join branches in lbranch on emp.Branch equals branches.Id
                               join type in ltype on leave.LeaveType equals type.Id
                               where type.Code == "CL" || type.Code == "ML" || type.Code == "PL"
                               select new
                               {
                                   EmpId = emp.EmpId,
                                   EmpName = emp.ShortName,
                                   Designation = desig.Code,
                                   Deptbranch = GetBranchDepartmentConcat(branches.Name, department.Name),
                                   Type = type.Code,
                                   AppliedDate = leave.UpdatedDate,
                                   StartDate = leave.StartDate,
                                   EndDate = leave.EndDate,
                                   DfromDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.StartDate),
                                   DtoDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.EndDate),
                               }).OrderBy(a => a.Type).OrderByDescending(A => A.AppliedDate);
                return Json(lResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }
        [HttpPost]
        public JsonResult MonthwiseClViews(string branch, string branch1)
        {
            Session["month"] = branch;
            Session["leavetype"] = branch1;
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            if (branch == "" && branch1 == "")
            {
                var lleave = db.Leaves.ToList();
                var lemp = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignations = db.Designations.ToList();
                var ltype = db.LeaveTypes.ToList();
                var lResult = (from leave in lleave
                               join emp in lemp on leave.EmpId equals emp.Id
                               join desig in ldesignations on emp.CurrentDesignation equals desig.Id
                               join department in ldept on emp.Department equals department.Id
                               join branches in lbranch on emp.Branch equals branches.Id
                               join type in ltype on leave.LeaveType equals type.Id
                               where type.Code == "CL" || type.Code == "ML" || type.Code == "PL"
                               select new
                               {
                                   EmpId = emp.EmpId,
                                   EmpName = emp.ShortName,
                                   Designation = desig.Code,
                                   Deptbranch = GetBranchDepartmentConcat(branches.Name, department.Name),
                                   Type = type.Code,
                                   AppliedDate = leave.UpdatedDate,
                                   StartDate = leave.StartDate,
                                   EndDate = leave.EndDate,
                                   DfromDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.StartDate),
                                   DtoDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.EndDate),
                               }).OrderBy(a => a.Type).OrderByDescending(A => A.AppliedDate);
                return Json(lResult, JsonRequestBehavior.AllowGet);
            }
            else if (branch != "" && branch1 == "")
            {
                try
                {
                    int M_no = Convert.ToInt32(branch);
                    int Y_no = 2018;
                    var lleave = db.Leaves.ToList();
                    var lemp = db.Employes.ToList();
                    var branches = db.Branches.ToList();
                    var Dept = db.Departments.ToList();
                    var ldesignations = db.Designations.ToList();
                    var ltype = db.LeaveTypes.ToList();
                    var data = (from leave in lleave
                                join emp in lemp on leave.EmpId equals emp.Id
                                join desig in ldesignations on emp.CurrentDesignation equals desig.Id
                                join dept in Dept on emp.Department equals dept.Id
                                join branchs in branches on emp.Branch equals branchs.Id
                                join type in ltype on leave.LeaveType equals type.Id
                                where type.Code == "CL" || type.Code == "ML" || type.Code == "PL"
                                select new
                                {
                                    EmpId = emp.EmpId,
                                    EmpName = emp.ShortName,
                                    Designation = desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                    Type = type.Code,
                                    AppliedDate = leave.UpdatedDate,
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    DfromDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.StartDate),
                                    DtoDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.EndDate),
                                });
                    var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.StartDate).Month == M_no).Where(u => Convert.ToDateTime(u.StartDate).Year == Y_no)).ToList();
                    return Json(data1, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    lMessage = ex.Message;
                }

            }
            else if (branch == "" && branch1 != "")
            {
                var lleave = db.Leaves.ToList();
                var lemp = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignations = db.Designations.ToList();
                var ltype = db.LeaveTypes.ToList();
                var lResult = (from leave in lleave
                               join emp in lemp on leave.EmpId equals emp.Id
                               join desig in ldesignations on emp.CurrentDesignation equals desig.Id
                               join department in ldept on emp.Department equals department.Id
                               join branches in lbranch on emp.Branch equals branches.Id
                               join type in ltype on leave.LeaveType equals type.Id
                               where type.Code == branch1
                               select new
                               {
                                   EmpId = emp.EmpId,
                                   EmpName = emp.ShortName,
                                   Designation = desig.Code,
                                   Deptbranch = GetBranchDepartmentConcat(branches.Name, department.Name),
                                   Type = type.Code,
                                   AppliedDate = leave.UpdatedDate,
                                   StartDate = leave.StartDate,
                                   EndDate = leave.EndDate,
                                   DfromDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.StartDate),
                                   DtoDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.EndDate),
                               }).OrderBy(a => a.Type).OrderByDescending(A => A.AppliedDate);
                return Json(lResult, JsonRequestBehavior.AllowGet);
            }
            else if (branch != "" && branch1 != "")
            {
                try
                {
                    int M_no = Convert.ToInt32(branch);
                    int Y_no = 2018;
                    var lleave = db.Leaves.ToList();
                    var lemp = db.Employes.ToList();
                    var branches = db.Branches.ToList();
                    var Dept = db.Departments.ToList();
                    var ldesignations = db.Designations.ToList();
                    var ltype = db.LeaveTypes.ToList();
                    var data = (from leave in lleave
                                join emp in lemp on leave.EmpId equals emp.Id
                                join desig in ldesignations on emp.CurrentDesignation equals desig.Id
                                join dept in Dept on emp.Department equals dept.Id
                                join branchs in branches on emp.Branch equals branchs.Id
                                join type in ltype on leave.LeaveType equals type.Id
                                where type.Code == branch1
                                select new
                                {
                                    EmpId = emp.EmpId,
                                    EmpName = emp.ShortName,
                                    Designation = desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                    Type = type.Code,
                                    AppliedDate = leave.UpdatedDate,
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    DfromDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.StartDate),
                                    DtoDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.EndDate),
                                });
                    var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.StartDate).Month == M_no).Where(u => Convert.ToDateTime(u.StartDate).Year == Y_no)).ToList();
                    return Json(data1, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    lMessage = ex.Message;
                }
            }
            return null;
        }
        public double Getdiffbetweenleavedates(DateTime Sd, DateTime Ed)
        {
            DateTime date1 = Sd.Date;
            DateTime date2 = Ed.Date;
            double NoOfDays = (date2 - date1).TotalDays; 
            return NoOfDays;
        }
        //Code to Calculate the difference between two dates.
        public double Getdiffbetweendates(DateTime Sd, DateTime Ed)
        {
            DateTime date1 = Sd.Date;
            DateTime date2 = Ed.Date;
            double NoOfDays = (date2 - date1).TotalDays;
            double Totalleavedays = NoOfDays + 1;
            return Totalleavedays;

        }
        //Report for Leave Approvals done by Admin/Manager/Executive.
        public ActionResult myleaveapproval()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/MyLeaveApproval.cshtml");
        }
        [HttpGet]
        public JsonResult myleaveapprovalview(string EmpId)
        {
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lMessage = string.Empty;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    join type in db.LeaveTypes on leave.LeaveType equals type.Id
                                    where leave.Status == "Approved" && emp2.Id == lEmpId || leave.Status == "Forwarded" && emp1.Id == lEmpId
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.StartDate,
                                        leave.EndDate,
                                        type.Code,
                                        leave.Id,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var jsonResult = Json(lResults, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;                   
                }
            }

            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        [HttpPost]
        public JsonResult myleaveapprovalviews(string branch,string EmpId)
        {
            try
            {

                Session["lstatus"] = branch;
                Session["lempid"] = EmpId;
                //int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lMessage = string.Empty;              
                if (EmpId == "" && branch == "")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branches in lBranches on emp.Branch equals branches.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    join type in db.LeaveTypes on leave.LeaveType equals type.Id
                                    where leave.Status == "Approved" && emp2.Id == lEmpId || leave.Status == "Forwarded" && emp1.Id == lEmpId
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.StartDate,
                                        leave.EndDate,
                                        type.Code,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var jsonResult = Json(lResults, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else if (EmpId != "" && branch == "")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branches in lBranches on emp.Branch equals branches.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    join type in db.LeaveTypes on leave.LeaveType equals type.Id
                                    where leave.Status == "Approved" && emp2.Id == lEmpId || leave.Status == "Forwarded" && emp1.Id == lEmpId
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.StartDate,
                                        leave.EndDate,
                                        type.Code,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var jsonResult = Json(lResults, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else if (EmpId != "" && branch=="Forwarded")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branches in lBranches on emp.Branch equals branches.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    join type in db.LeaveTypes on leave.LeaveType equals type.Id
                                    where leave.Status == branch && emp1.Id == lEmpId && emp.EmpId == EmpId
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.StartDate,
                                        leave.EndDate,
                                        type.Code,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var jsonResult = Json(lResults, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;

                }          
                else if (EmpId != "" && branch == "Approved")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branches in lBranches on emp.Branch equals branches.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    join type in db.LeaveTypes on leave.LeaveType equals type.Id
                                    where leave.Status == branch && emp2.Id == lEmpId && emp.EmpId == EmpId
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.StartDate,
                                        leave.EndDate,
                                        type.Code,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var jsonResult = Json(lResults, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else if (EmpId != "" && branch == "All")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branches in lBranches on emp.Branch equals branches.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    join type in db.LeaveTypes on leave.LeaveType equals type.Id
                                    where (leave.Status == "Approved" && emp2.Id == lEmpId || leave.Status == "Forwarded" && emp1.Id == lEmpId) && emp.EmpId == EmpId          
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.StartDate,
                                        leave.EndDate,
                                        type.Code,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var jsonResult = Json(lResults, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }                
                else if (EmpId == "" && branch == "Forwarded")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branches in lBranches on emp.Branch equals branches.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    join type in db.LeaveTypes on leave.LeaveType equals type.Id
                                    where leave.Status == branch && emp1.Id == lEmpId 
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.StartDate,
                                        leave.EndDate,
                                        type.Code,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var jsonResult = Json(lResults, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;

                }
                else if (EmpId == "" && branch == "Approved")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branches in lBranches on emp.Branch equals branches.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    join type in db.LeaveTypes on leave.LeaveType equals type.Id
                                    where leave.Status == branch && emp2.Id == lEmpId 
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.StartDate,
                                        leave.EndDate,
                                        type.Code,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var jsonResult = Json(lResults, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else if (EmpId == "" && branch == "All")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branches in lBranches on emp.Branch equals branches.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    join type in db.LeaveTypes on leave.LeaveType equals type.Id
                                    where (leave.Status == "Approved" && emp2.Id == lEmpId || leave.Status == "Forwarded" && emp1.Id == lEmpId) 
                                    select new
                                    {
                                        emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        designation = desig.Code,
                                        Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                        Authority = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Id,
                                        leave.StartDate,
                                        leave.EndDate,
                                        type.Code,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var jsonResult = Json(lResults, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        //Code for LTC Report
        public ActionResult LTCview()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/LTCReport.cshtml");
        }
        [HttpGet]
        public JsonResult LTCViews(string EmpId)
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
                if (String.IsNullOrEmpty(EmpId))
                {
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on emp.CurrentDesignation equals desig.Id
                                join branchs in lbranch on emp.Branch equals branchs.Id
                                join depart in ldepartments on emp.Department equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
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
        public JsonResult LTCViewss(string StartDate, string EndDate, string lType)
        {
            try
            {
                Session["lstartdate"] = StartDate;
                Session["lenddate"] = EndDate;
                Session["ltype"] = lType;
                var lResult = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var dResult = db.Designations.ToList();
                var ltcresult = db.Leaves_LTC.ToList();
                var blockperiod = db.BlockPeriod.ToList();
                if (StartDate != "" && EndDate != "" && lType != "")
                {
                    DateTime lStartDate = Convert.ToDateTime(StartDate);
                    DateTime lEndDate = Convert.ToDateTime(EndDate);
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on emp.CurrentDesignation equals desig.Id
                                join branchs in lbranch on emp.Branch equals branchs.Id
                                join depart in ldepartments on emp.Department equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                                where employee.LtcType == lType
                                where (employee.StartDate >= lStartDate && employee.EndDate <= lEndDate) ||
                                (employee.StartDate <= lStartDate && employee.EndDate >= lEndDate)
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
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = (from employee in ltcresult
                                join emp in lResult on employee.EmpId equals emp.Id
                                join desig in dResult on emp.CurrentDesignation equals desig.Id
                                join branchs in lbranch on emp.Branch equals branchs.Id
                                join depart in ldepartments on emp.Department equals depart.Id
                                join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
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
        // Create PDF File for LTC Report
        public FileResult LTCPDF()
        {
            string lsdate = Convert.ToString(Session["lstartdate"]);
            string ledate = Convert.ToString(Session["lenddate"]); 
            string letype = Convert.ToString(Session["ltype"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("LTCReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(10f, 10f, 10f, 10f);
            PdfPTable tableLayout1 = new PdfPTable(10);
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            doc.Add(Add_Content_To_PDFLTC(tableLayout1,lsdate,ledate,letype));
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
                        //tableLayout1.FooterRows = 1;
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
        protected PdfPTable Add_Content_To_PDFLTC(PdfPTable tableLayout1, string lStartDate, string lEndDate, string lType)
        {
           
            float[] headers1 = { 75, 50, 50, 90, 50, 60, 70, 70, 50, 50 }; //Header Widths  
            tableLayout1.SetWidths(headers1);
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("LTC Report", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            DateTime lStartDate1 = GetCurrentTime(DateTime.Now).Date;
            if (lStartDate != "" && lEndDate != "" && lType != "")
            {
                DateTime lsd = Convert.ToDateTime(lStartDate);
                DateTime led = Convert.ToDateTime(lEndDate);
                var lResult = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var dResult = db.Designations.ToList();
                var ltcresult = db.Leaves_LTC.ToList();
                var blockperiod = db.BlockPeriod.ToList();
                var data = (from employee in ltcresult
                            join emp in lResult on employee.EmpId equals emp.Id
                            join desig in dResult on emp.CurrentDesignation equals desig.Id
                            join branchs in lbranch on emp.Branch equals branchs.Id
                            join depart in ldepartments on emp.Department equals depart.Id
                            join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                            where employee.LtcType == lType
                            where (employee.StartDate >= lsd && employee.EndDate <= led) ||
                            (employee.StartDate <= lsd && employee.EndDate >= led)
                            select new
                            {
                                Empdetails = emp.EmpId + " " + "-" + " " + emp.ShortName,
                                desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                employee.UpdatedDate,
                                Dates = employee.StartDate.ToShortDateString() + " " + "-" + " " + employee.EndDate.ToShortDateString(),
                                employee.Subject,
                                employee.TotalExperience,
                                employee.Status,
                                employee.TravelAdvance,
                                employee.ModeOfTransport,
                                employee.PlaceOfVisits,
                                employee.LtcType,
                                block.Block_Period,
                            });

                //Adding headers  
                AddCellToHeader(tableLayout1, "Emp Details");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "Dates");
                AddCellToHeader(tableLayout1, "Blockperiod");
                AddCellToHeader(tableLayout1, "TravelAdvance");
                AddCellToHeader(tableLayout1, "ModeOfTransport");
                AddCellToHeader(tableLayout1, "PlaceOf Visit");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "Status");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.Empdetails);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                    AddCellToBody(tableLayout1, lemp.Dates);
                    AddCellToBody(tableLayout1, lemp.Block_Period);
                    AddCellToBody(tableLayout1, lemp.TravelAdvance);
                    AddCellToBody(tableLayout1, lemp.ModeOfTransport);
                    AddCellToBody(tableLayout1, lemp.PlaceOfVisits);
                    AddCellToBody(tableLayout1, lemp.LtcType);
                    AddCellToBody(tableLayout1, lemp.Status);
                }
                return tableLayout1;
            }
            else
            {
                var lResult = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var dResult = db.Designations.ToList();
                var ltcresult = db.Leaves_LTC.ToList();
                var blockperiod = db.BlockPeriod.ToList();
                var data = (from employee in ltcresult
                            join emp in lResult on employee.EmpId equals emp.Id
                            join desig in dResult on emp.CurrentDesignation equals desig.Id
                            join branchs in lbranch on emp.Branch equals branchs.Id
                            join depart in ldepartments on emp.Department equals depart.Id
                            join block in blockperiod on employee.Block_Period equals Convert.ToString(block.Id)
                            select new
                            {
                                Empdetails = emp.EmpId + " " + "-" + " " + emp.ShortName,
                                desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                employee.UpdatedDate,
                                Dates = employee.StartDate.ToShortDateString() + " " + "-" + " " + employee.EndDate.ToShortDateString(),
                                employee.Subject,
                                employee.TotalExperience,
                                employee.Status,
                                employee.TravelAdvance,
                                employee.ModeOfTransport,
                                employee.PlaceOfVisits,
                                employee.LtcType,
                                block.Block_Period,
                            });

                //Adding headers  
                AddCellToHeader(tableLayout1, "Emp Details");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "Dates");
                AddCellToHeader(tableLayout1, "Blockperiod");
                AddCellToHeader(tableLayout1, "TravelAdvance");
                AddCellToHeader(tableLayout1, "ModeOfTransport");
                AddCellToHeader(tableLayout1, "PlaceOf Visit");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "Status");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.Empdetails);
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                    AddCellToBody(tableLayout1, lemp.Dates);
                    AddCellToBody(tableLayout1, lemp.Block_Period);
                    AddCellToBody(tableLayout1, lemp.TravelAdvance);
                    AddCellToBody(tableLayout1, lemp.ModeOfTransport);
                    AddCellToBody(tableLayout1, lemp.PlaceOfVisits);
                    AddCellToBody(tableLayout1, lemp.LtcType);
                    AddCellToBody(tableLayout1, lemp.Status);
                }
                return tableLayout1;
            }

        }
        // Create PDF File for Month wise Leave Report
        public FileResult MonthwiseLeavePDF()
        {
            try
            {
                string month = Convert.ToString(Session["lmonth"]);
                
                // If no month is selected, use current month as default
                if (string.IsNullOrEmpty(month))
                {
                    month = DateTime.Now.Month.ToString();
                    Session["lmonth"] = month;
                }

                MemoryStream workStream = new MemoryStream();
                StringBuilder status = new StringBuilder("");
                DateTime dTime = DateTime.Now;
                string strPDFFileName = string.Format("MonthwiseLeaveReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
                Document doc = new Document(new Rectangle(1000f, 1000f));
                doc.SetMargins(20f, 20f, 20f, 20f);
                //Create PDF Table with 5 columns  
                PdfPTable tableLayout1 = new PdfPTable(11);
                //file will created in this path  
                string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
                PdfWriter.GetInstance(doc, workStream).CloseStream = false;
                doc.Open();
                doc.Add(Add_Content_To_PDFLeave(tableLayout1));
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
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page:" + i.ToString() + "/" + pages.ToString(), blackFont), 470f, 12f, 0);
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
                Session.Remove("lmonth");
                return File(workStream, "application/pdf", strPDFFileName);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine("PDF Generation Error: " + ex.Message);
                
                // Return a simple error PDF or redirect to error page
                MemoryStream errorStream = new MemoryStream();
                Document errorDoc = new Document();
                PdfWriter.GetInstance(errorDoc, errorStream);
                errorDoc.Open();
                errorDoc.Add(new Paragraph("Error generating PDF. Please try again."));
                errorDoc.Close();
                byte[] errorBytes = errorStream.ToArray();
                return File(errorBytes, "application/pdf", "Error.pdf");
            }
        }
        protected PdfPTable Add_Content_To_PDFLeave(PdfPTable tableLayout1)
        {
            string month = Convert.ToString(Session["lmonth"]);
            float[] headers1 = { 40, 75, 55, 70, 70, 70, 70, 50, 60, 70, 70 }; //Header Widths  
            tableLayout1.SetWidths(headers1);
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("MonthwiseLeaveReport", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var lResult = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var dResult = db.Designations.ToList();
            var ltcresult = db.Leaves_LTC.ToList();
            var blockperiod = db.BlockPeriod.ToList();
            var lleaves = db.Leaves.ToList();
            var lemp = db.Employes.ToList();
            var ltype = db.LeaveTypes.ToList();
            var branch = db.Branches.ToList();
            var Dept = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            if (month == "")
            {
                var data = (from leave in lleaves
                            join emp in lemp on leave.EmpId equals emp.Id
                            join type in ltype on leave.LeaveType equals type.Id
                            join branchs in branch on emp.Branch equals branchs.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join dept in Dept on emp.Department equals dept.Id
                            select new
                            {
                                leave.Id,
                                emp.EmpId,
                                emp.ShortName,
                                designation = desig.Code,
                                UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                StartDate = leave.StartDate.ToShortDateString(),
                                EndDate = leave.EndDate.ToShortDateString(),
                                leave.LeaveDays,
                                type.Code,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.UpdatedDate);

                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "LeaveType");
                AddCellToHeader(tableLayout1, "Status");
                AddCellToHeader(tableLayout1, "Subject");
                AddCellToHeader(tableLayout1, "Reason");
                //Adding body  
                foreach (var lemployee in data)
                {
                    AddCellToBody(tableLayout1, lemployee.EmpId);
                    AddCellToBody(tableLayout1, lemployee.ShortName);
                    AddCellToBody(tableLayout1, lemployee.designation);
                    AddCellToBody(tableLayout1, lemployee.Deptbranch);
                    AddCellToBody(tableLayout1, lemployee.UpdatedDate);
                    AddCellToBody(tableLayout1, lemployee.StartDate);
                    AddCellToBody(tableLayout1, lemployee.EndDate);
                    AddCellToBody(tableLayout1, lemployee.Code);
                    AddCellToBody(tableLayout1, lemployee.Status);
                    AddCellToBody(tableLayout1, lemployee.Subject);
                    AddCellToBody(tableLayout1, lemployee.Reason);

                }
                return tableLayout1;
            }
            else
            {
                int M_no = Convert.ToInt32(month);
                int Y_no = DateTime.Now.Year;
                var data = (from leave in lleaves
                            join emp in lemp on leave.EmpId equals emp.Id
                            join type in ltype on leave.LeaveType equals type.Id
                            join branchs in branch on emp.Branch equals branchs.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join dept in Dept on emp.Department equals dept.Id

                            select new
                            {
                                leave.Id,
                                emp.EmpId,
                                emp.ShortName,
                                designation = desig.Code,
                                UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                StartDate = leave.StartDate.ToShortDateString(),
                                EndDate = leave.EndDate.ToShortDateString(),
                                leave.LeaveDays,
                                type.Code,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.UpdatedDate);
                var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.StartDate).Month == M_no).Where(u => Convert.ToDateTime(u.StartDate).Year == Y_no)).ToList();
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "LeaveType");
                AddCellToHeader(tableLayout1, "Status");
                AddCellToHeader(tableLayout1, "Subject");
                AddCellToHeader(tableLayout1, "Reason");
                //Adding body  
                foreach (var lemployee in data1)
                {
                    AddCellToBody(tableLayout1, lemployee.EmpId);
                    AddCellToBody(tableLayout1, lemployee.ShortName);
                    AddCellToBody(tableLayout1, lemployee.designation);
                    AddCellToBody(tableLayout1, lemployee.Deptbranch);
                    AddCellToBody(tableLayout1, lemployee.UpdatedDate);
                    AddCellToBody(tableLayout1, lemployee.StartDate);
                    AddCellToBody(tableLayout1, lemployee.EndDate);
                    AddCellToBody(tableLayout1, lemployee.Code);
                    AddCellToBody(tableLayout1, lemployee.Status);
                    AddCellToBody(tableLayout1, lemployee.Subject);
                    AddCellToBody(tableLayout1, lemployee.Reason);

                }
                return tableLayout1;
            }

        }
        // Create PDF File for Month wise OD Report
        public FileResult MonthwiseODPDF()
        {

            string month = Convert.ToString(Session["lod"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("MonthwiseODReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout1 = new PdfPTable(9);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            doc.Add(Add_Content_To_PDFOD(tableLayout1));
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
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page:" + i.ToString() + "/" + pages.ToString(), blackFont), 470f, 12f, 0);
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
            Session.Remove("lod");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDFOD(PdfPTable tableLayout1)
        {
            string month = Convert.ToString(Session["lod"]);
            float[] headers1 = { 40, 75, 55, 70, 70, 70, 70, 50, 60 }; //Header Widths  
            tableLayout1.SetWidths(headers1);
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("MonthwiseODReport", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var ltype = db.OD_Master.ToList();
            var ldeputation = db.OD_OtherDuty.ToList();
            var lBranches = db.Branches.ToList();
            var lLeaveTypes = db.LeaveTypes.ToList();
            var Departments = db.Departments.ToList();
            var lemployees = db.Employes.ToList();
            var ldesignation = db.Designations.ToList();
            if (month == "")
            {
                var data = (from otherduty in ldeputation
                            join emp in lemployees on otherduty.EmpId equals emp.Id
                            join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                            join branch in lBranches on emp.Branch equals branch.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join dept in Departments on emp.Department equals dept.Id
                            join type in ltype on otherduty.Purpose equals type.Id
                            orderby otherduty.UpdatedDate descending
                            select new
                            {

                                otherduty.Id,
                                emp.EmpId,
                                EmpName = emp.ShortName,
                                designation = desig.Code,
                                VistorFrom = visitbran.Name,
                                otherduty.VistorTo,
                                UpdatedDate = otherduty.UpdatedDate.ToShortDateString(),
                                Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                SD = Convert.ToString(otherduty.StartDate),
                                ED = Convert.ToString(otherduty.EndDate),
                               // Purpose = type.ODType,
                                otherduty.Status,
                                otherduty.Description,
                            }).OrderByDescending(A => A.Id);

                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
               // AddCellToHeader(tableLayout1, "Purpose");
                AddCellToHeader(tableLayout1, "Status");
                AddCellToHeader(tableLayout1, "Description");
                //Adding body  
                foreach (var lemployee in data)
                {
                    AddCellToBody(tableLayout1, lemployee.EmpId);
                    AddCellToBody(tableLayout1, lemployee.EmpName);
                    AddCellToBody(tableLayout1, lemployee.designation);
                    AddCellToBody(tableLayout1, lemployee.Deptbranch);
                    AddCellToBody(tableLayout1, lemployee.UpdatedDate);
                    AddCellToBody(tableLayout1, lemployee.SD);
                    AddCellToBody(tableLayout1, lemployee.ED);
                   // AddCellToBody(tableLayout1, lemployee.Purpose);
                    AddCellToBody(tableLayout1, lemployee.Status);
                    AddCellToBody(tableLayout1, lemployee.Description);

                }
                return tableLayout1;
            }
            else
            {
                int M_no = Convert.ToInt32(month);
                int Y_no = 2018;
                var data = (from otherduty in ldeputation
                            join emp in lemployees on otherduty.EmpId equals emp.Id
                            join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                            join branch in lBranches on emp.Branch equals branch.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join dept in Departments on emp.Department equals dept.Id
                            join type in ltype on otherduty.Purpose equals type.Id
                            orderby otherduty.UpdatedDate descending
                            select new
                            {

                                otherduty.Id,
                                emp.EmpId,
                                EmpName = emp.ShortName,
                                designation = desig.Code,
                                VistorFrom = visitbran.Name,
                                otherduty.VistorTo,
                                UpdatedDate = otherduty.UpdatedDate.ToShortDateString(),
                                Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                SD = Convert.ToString(otherduty.StartDate),
                                ED = Convert.ToString(otherduty.EndDate),
                                //Purpose = type.ODType,
                                otherduty.Status,
                                otherduty.Description,
                            }).OrderByDescending(A => A.Id);
                var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.SD).Month == M_no).Where(u => Convert.ToDateTime(u.SD).Year == Y_no)).ToList();
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Branch");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                //AddCellToHeader(tableLayout1, "Purpose");
                AddCellToHeader(tableLayout1, "Status");
                AddCellToHeader(tableLayout1, "Description");
                //Adding body  
                foreach (var lemployee in data1)
                {
                    AddCellToBody(tableLayout1, lemployee.EmpId);
                    AddCellToBody(tableLayout1, lemployee.EmpName);
                    AddCellToBody(tableLayout1, lemployee.designation);
                    AddCellToBody(tableLayout1, lemployee.Deptbranch);
                    AddCellToBody(tableLayout1, lemployee.UpdatedDate);
                    AddCellToBody(tableLayout1, lemployee.SD);
                    AddCellToBody(tableLayout1, lemployee.ED);
                   // AddCellToBody(tableLayout1, lemployee.Purpose);
                    AddCellToBody(tableLayout1, lemployee.Status);
                    AddCellToBody(tableLayout1, lemployee.Description);

                }
                return tableLayout1;
            }

        }
        public void ExportGridToExcelmonthwiseOD()
        {
            try
            {
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var ltype = db.OD_Master.ToList();
                var ldeputation = db.OD_OtherDuty.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                string month = Convert.ToString(Session["lod"]);
                if (month == "")
                {
                    var data = (from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id
                                join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                join branch in lBranches on emp.Branch equals branch.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join dept in Departments on emp.Department equals dept.Id
                                join type in ltype on otherduty.Purpose equals type.Id
                                orderby otherduty.UpdatedDate descending
                                select new
                                {       
                                    emp.EmpId,
                                    EmployeeName = emp.ShortName,
                                    Designation = desig.Code,
                                    VistorFrom = visitbran.Name,
                                    otherduty.VistorTo,
                                    AppliedDate = otherduty.UpdatedDate.ToShortDateString(),
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = Convert.ToString(otherduty.StartDate),
                                    EndDate = Convert.ToString(otherduty.EndDate),
                                    otherduty.Status,
                                    otherduty.Description,
                                });
                    var gv = new GridView();
                    gv.DataSource = data;
                    if (data.Count() == 0)
                    {
                        gv.ShowHeaderWhenEmpty = true;
                    }
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=OD.xls");
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
                }
                else
                {
                    int M_no = Convert.ToInt32(month);
                    int Y_no = 2018;
                    var data = (from otherduty in ldeputation
                                join emp in lemployees on otherduty.EmpId equals emp.Id
                                join visitbran in lBranches on otherduty.VistorFrom equals visitbran.Id
                                join branch in lBranches on emp.Branch equals branch.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join dept in Departments on emp.Department equals dept.Id
                                join type in ltype on otherduty.Purpose equals type.Id
                                orderby otherduty.UpdatedDate descending
                                select new
                                {
                                    emp.EmpId,
                                    EmployeeName = emp.ShortName,
                                    Designation = desig.Code,
                                    VistorFrom = visitbran.Name,
                                    otherduty.VistorTo,
                                    AppliedDate = otherduty.UpdatedDate.ToShortDateString(),
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = Convert.ToString(otherduty.StartDate),
                                    EndDate = Convert.ToString(otherduty.EndDate),
                                    otherduty.Status,
                                    otherduty.Description,
                                });
                    var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.StartDate).Month == M_no).Where(u => Convert.ToDateTime(u.StartDate).Year == Y_no)).ToList();
                    var gv = new GridView();
                    gv.DataSource = data1;
                    if ((data1.Count == 0))
                    {
                        gv.ShowHeaderWhenEmpty = true;
                    }
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=OD.xls");
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
                }
            }
            catch(Exception e)
            {
                e.ToString();
            }
        }
                // Create PDF File for Month wise Temporary Transfer Report
                public FileResult MonthwiseTempPDF()
        {

            string month = Convert.ToString(Session["ltemp"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("MonthwiseTemporaryTransferReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout1 = new PdfPTable(7);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            doc.Add(Add_Content_To_PDFTemp(tableLayout1));
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
                        tableLayout1.AddCell(new PdfPCell(new Phrase(i.ToString() + "/" + pages.ToString(), new Font(Font.FontFamily.HELVETICA, 2, 1, new BaseColor(0, 0, 0))))
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
            Session.Remove("ltemp");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDFTemp(PdfPTable tableLayout1)
        {
            string month = Convert.ToString(Session["ltemp"]);
            float[] headers1 = { 40, 75, 55, 70, 70, 70, 70 }; //Header Widths  
            tableLayout1.SetWidths(headers1);
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("MonthwiseTemporaryTransferReport", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            if (month == "")
            {
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "TemporaryTransfer"
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {

                                transfer.Id,
                                EmpId = emplist.EmpId,
                                transfer.Type,
                                emplist.ShortName,
                                OldDesignation = desig.Code,
                                NewDesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                EffectiveFrom = transfer.EffectiveFrom,
                                EffectiveTo = transfer.EffectiveTo,
                            }).OrderByDescending(a => a.EffectiveFrom);

                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "OldDesig");
                AddCellToHeader(tableLayout1, "NewDesig");
                AddCellToHeader(tableLayout1, "OldBranch");
                AddCellToHeader(tableLayout1, "NewBranch");
                AddCellToHeader(tableLayout1, "EffectiveFrom");

                //Adding body  
                foreach (var lemployee in data)
                {
                    AddCellToBody(tableLayout1, lemployee.EmpId);
                    AddCellToBody(tableLayout1, lemployee.ShortName);
                    AddCellToBody(tableLayout1, lemployee.OldDesignation);
                    AddCellToBody(tableLayout1, lemployee.NewDesignation);
                    AddCellToBody(tableLayout1, lemployee.oldDeptbranch);
                    AddCellToBody(tableLayout1, lemployee.newDeptbranch);
                    AddCellToBody(tableLayout1, lemployee.EffectiveFrom.Value.ToShortDateString());
                }
                return tableLayout1;
            }
            else
            {
                int M_no = Convert.ToInt32(month);
                int Y_no = 2018;
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "TemporaryTransfer"
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {

                                transfer.Id,
                                EmpId = emplist.EmpId,
                                transfer.Type,
                                emplist.ShortName,
                                OldDesignation = desig.Code,
                                NewDesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                EffectiveFrom = transfer.EffectiveFrom,
                                EffectiveTo = transfer.EffectiveTo,
                            }).OrderByDescending(a => a.EffectiveFrom);
                var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.EffectiveFrom).Month == M_no).Where(u => Convert.ToDateTime(u.EffectiveFrom).Year == Y_no)).ToList();
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "OldDesig");
                AddCellToHeader(tableLayout1, "NewDesig");
                AddCellToHeader(tableLayout1, "OldBranch");
                AddCellToHeader(tableLayout1, "NewBranch");
                AddCellToHeader(tableLayout1, "EffectiveFrom");
                //Adding body  
                foreach (var lemployee in data1)
                {
                    AddCellToBody(tableLayout1, lemployee.EmpId);
                    AddCellToBody(tableLayout1, lemployee.ShortName);
                    AddCellToBody(tableLayout1, lemployee.OldDesignation);
                    AddCellToBody(tableLayout1, lemployee.NewDesignation);
                    AddCellToBody(tableLayout1, lemployee.oldDeptbranch);
                    AddCellToBody(tableLayout1, lemployee.newDeptbranch);
                    AddCellToBody(tableLayout1, lemployee.EffectiveFrom.Value.ToShortDateString());
                }
                return tableLayout1;
            }
        }

        // Create PDF File for Month wise CL Report
        public FileResult MonthwiseCLPDF()
        {

            string month = Convert.ToString(Session["month"]);
            string type = Convert.ToString(Session["leavetype"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("MonthwiseLeaveReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout1 = new PdfPTable(10);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            doc.Add(Add_Content_To_PDFCL(tableLayout1));
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
                        tableLayout1.AddCell(new PdfPCell(new Phrase(i.ToString() + "/" + pages.ToString(), new Font(Font.FontFamily.HELVETICA, 2, 1, new BaseColor(0, 0, 0))))
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
            Session.Remove("month");
            Session.Remove("leavetype");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDFCL(PdfPTable tableLayout1)
        {
            string month = Convert.ToString(Session["month"]);
            string types = Convert.ToString(Session["leavetype"]);
            float[] headers1 = { 40, 75, 55, 70, 70, 70, 70, 70, 70, 70 }; //Header Widths  
            tableLayout1.SetWidths(headers1);
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("MonthwiseLeaveReport", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            if (month == "" && types == "")
            {
                var lleave = db.Leaves.ToList();
                var lemp = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignations = db.Designations.ToList();
                var ltype = db.LeaveTypes.ToList();
                var data = (from leave in lleave
                            join emp in lemp on leave.EmpId equals emp.Id
                            join desig in ldesignations on emp.CurrentDesignation equals desig.Id
                            join department in ldept on emp.Department equals department.Id
                            join branches in lbranch on emp.Branch equals branches.Id
                            join type in ltype on leave.LeaveType equals type.Id
                            where type.Code == "CL" || type.Code == "ML" || type.Code == "PL"
                            select new
                            {
                                EmpId = emp.EmpId,
                                EmpName = emp.ShortName,
                                Designation = desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branches.Name, department.Name),
                                Type = type.Code,
                                AppliedDate = leave.UpdatedDate,
                                StartDate = leave.StartDate,
                                EndDate = leave.EndDate,
                                DfromDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.StartDate),
                                DtoDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.EndDate),
                            }).OrderBy(a => a.Type);



                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Deptbranch");
                AddCellToHeader(tableLayout1, "LeaveType");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Difference between From Date & Applied Date");
                AddCellToHeader(tableLayout1, "Difference between To Date & Applied Date");
                //Adding body  
                foreach (var lemployee in data)
                {
                    AddCellToBody(tableLayout1, lemployee.EmpId);
                    AddCellToBody(tableLayout1, lemployee.EmpName);
                    AddCellToBody(tableLayout1, lemployee.Designation);
                    AddCellToBody(tableLayout1, lemployee.Deptbranch);
                    AddCellToBody(tableLayout1, lemployee.Type);
                    AddCellToBody(tableLayout1, lemployee.AppliedDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.StartDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.EndDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.DfromDate.ToString());
                    AddCellToBody(tableLayout1, lemployee.DtoDate.ToString());
                }
                return tableLayout1;
            }
            else if (month != "" && types == "")
            {
                int M_no = Convert.ToInt32(month);
                int Y_no = 2018;
                var lleave = db.Leaves.ToList();
                var lemp = db.Employes.ToList();
                var branches = db.Branches.ToList();
                var Dept = db.Departments.ToList();
                var ldesignations = db.Designations.ToList();
                var ltype = db.LeaveTypes.ToList();
                var data = (from leave in lleave
                            join emp in lemp on leave.EmpId equals emp.Id
                            join desig in ldesignations on emp.CurrentDesignation equals desig.Id
                            join dept in Dept on emp.Department equals dept.Id
                            join branchs in branches on emp.Branch equals branchs.Id
                            join type in ltype on leave.LeaveType equals type.Id
                            where type.Code == "CL" || type.Code == "ML" || type.Code == "PL"
                            select new
                            {
                                EmpId = emp.EmpId,
                                EmpName = emp.ShortName,
                                Designation = desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                Type = type.Code,
                                AppliedDate = leave.UpdatedDate,
                                StartDate = leave.StartDate,
                                EndDate = leave.EndDate,
                                DfromDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.StartDate),
                                DtoDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.EndDate),
                            });
                var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.StartDate).Month == M_no).Where(u => Convert.ToDateTime(u.StartDate).Year == Y_no)).ToList();



                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Deptbranch");
                AddCellToHeader(tableLayout1, "LeaveType");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Difference between From Date & Applied Date");
                AddCellToHeader(tableLayout1, "Difference between To Date & Applied Date");
                //Adding body  
                foreach (var lemployee in data1)
                {
                    AddCellToBody(tableLayout1, lemployee.EmpId);
                    AddCellToBody(tableLayout1, lemployee.EmpName);
                    AddCellToBody(tableLayout1, lemployee.Designation);
                    AddCellToBody(tableLayout1, lemployee.Deptbranch);
                    AddCellToBody(tableLayout1, lemployee.Type);
                    AddCellToBody(tableLayout1, lemployee.AppliedDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.StartDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.EndDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.DfromDate.ToString());
                    AddCellToBody(tableLayout1, lemployee.DtoDate.ToString());
                }
                return tableLayout1;
            }
            else if (month == "" && types != "")
            {
                var lleave = db.Leaves.ToList();
                var lemp = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignations = db.Designations.ToList();
                var ltype = db.LeaveTypes.ToList();
                var data = (from leave in lleave
                            join emp in lemp on leave.EmpId equals emp.Id
                            join desig in ldesignations on emp.CurrentDesignation equals desig.Id
                            join department in ldept on emp.Department equals department.Id
                            join branches in lbranch on emp.Branch equals branches.Id
                            join type in ltype on leave.LeaveType equals type.Id
                            where type.Code == types
                            select new
                            {
                                EmpId = emp.EmpId,
                                EmpName = emp.ShortName,
                                Designation = desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branches.Name, department.Name),
                                Type = type.Code,
                                AppliedDate = leave.UpdatedDate,
                                StartDate = leave.StartDate,
                                EndDate = leave.EndDate,
                                DfromDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.StartDate),
                                DtoDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.EndDate),
                            }).OrderBy(a => a.Type);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Deptbranch");
                AddCellToHeader(tableLayout1, "LeaveType");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Difference between From Date & Applied Date");
                AddCellToHeader(tableLayout1, "Difference between To Date & Applied Date");
                //Adding body  
                foreach (var lemployee in data)
                {
                    AddCellToBody(tableLayout1, lemployee.EmpId);
                    AddCellToBody(tableLayout1, lemployee.EmpName);
                    AddCellToBody(tableLayout1, lemployee.Designation);
                    AddCellToBody(tableLayout1, lemployee.Deptbranch);
                    AddCellToBody(tableLayout1, lemployee.Type);
                    AddCellToBody(tableLayout1, lemployee.AppliedDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.StartDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.EndDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.DfromDate.ToString());
                    AddCellToBody(tableLayout1, lemployee.DtoDate.ToString());
                }
                return tableLayout1;
            }
            else if (month != "" && types != "")
            {
                int M_no = Convert.ToInt32(month);
                int Y_no = 2018;
                var lleave = db.Leaves.ToList();
                var lemp = db.Employes.ToList();
                var branches = db.Branches.ToList();
                var Dept = db.Departments.ToList();
                var ldesignations = db.Designations.ToList();
                var ltype = db.LeaveTypes.ToList();
                var data = (from leave in lleave
                            join emp in lemp on leave.EmpId equals emp.Id
                            join desig in ldesignations on emp.CurrentDesignation equals desig.Id
                            join dept in Dept on emp.Department equals dept.Id
                            join branchs in branches on emp.Branch equals branchs.Id
                            join type in ltype on leave.LeaveType equals type.Id
                            where type.Code == types
                            select new
                            {
                                EmpId = emp.EmpId,
                                EmpName = emp.ShortName,
                                Designation = desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branchs.Name, dept.Name),
                                Type = type.Code,
                                AppliedDate = leave.UpdatedDate,
                                StartDate = leave.StartDate,
                                EndDate = leave.EndDate,
                                DfromDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.StartDate),
                                DtoDate = Getdiffbetweenleavedates(leave.UpdatedDate, leave.EndDate),
                            });
                var data1 = (data.ToList().Where(u => Convert.ToDateTime(u.StartDate).Month == M_no).Where(u => Convert.ToDateTime(u.StartDate).Year == Y_no)).ToList();
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Deptbranch");
                AddCellToHeader(tableLayout1, "LeaveType");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Difference between From Date & Applied Date");
                AddCellToHeader(tableLayout1, "Difference between To Date & Applied Date");
                //Adding body  
                foreach (var lemployee in data1)
                {
                    AddCellToBody(tableLayout1, lemployee.EmpId);
                    AddCellToBody(tableLayout1, lemployee.EmpName);
                    AddCellToBody(tableLayout1, lemployee.Designation);
                    AddCellToBody(tableLayout1, lemployee.Deptbranch);
                    AddCellToBody(tableLayout1, lemployee.Type);
                    AddCellToBody(tableLayout1, lemployee.AppliedDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.StartDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.EndDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemployee.DfromDate.ToString());
                    AddCellToBody(tableLayout1, lemployee.DtoDate.ToString());
                }
                return tableLayout1;
            }
            return tableLayout1;

        }
        // Report For Future Leave Report
        public ActionResult FutureLeave()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.BranchCode.ToString() + " " + x.Name.ToString(),
            });
            ViewBag.Branch = new SelectList(items, "Id", "Name");
            return View("~/Views/Reports/FutureLeaveList.cshtml");
        }
        [HttpGet]
        public JsonResult FutureLeaveView(string EmpId)
        {
            try
            {
                var lleave = db.Leaves.ToList();
                var ltransfer = db.Employee_Transfer.ToList();
                var dbResult = db.Employes.ToList();
                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                var type = db.LeaveTypes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from leave in lleave
                                join emp in dbResult on leave.EmpId equals emp.Id
                                join ltype in type on leave.LeaveType equals ltype.Id
                                join lbranch in Branches on emp.Branch equals lbranch.Id
                                join ldept in Departments on emp.Branch equals ldept.Id
                                join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                                where leave.StartDate.Date > leave.UpdatedDate.Date
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = ldesig.Code,
                                    DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                    ltype.Code,
                                    leave.StartDate,
                                    leave.EndDate,
                                    leave.UpdatedDate,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
                                }).OrderByDescending(a => a.UpdatedDate);
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
        public JsonResult FutureLeaveViews(string StartDate, string EndDate)
        {
            Session["sd"] = StartDate;
            Session["ed"] = EndDate;
            var lleave = db.Leaves.ToList();
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            var type = db.LeaveTypes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (StartDate == "")
            {
                var data = (from leave in lleave
                            join emp in dbResult on leave.EmpId equals emp.Id
                            join ltype in type on leave.LeaveType equals ltype.Id
                            join lbranch in Branches on emp.Branch equals lbranch.Id
                            join ldept in Departments on emp.Branch equals ldept.Id
                            join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                            where leave.StartDate.Date > leave.UpdatedDate.Date
                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                designation = ldesig.Code,
                                DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                ltype.Code,
                                leave.StartDate,
                                leave.EndDate,
                                leave.UpdatedDate,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.UpdatedDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (EndDate == "")
            {
                var data = (from leave in lleave
                            join emp in dbResult on leave.EmpId equals emp.Id
                            join ltype in type on leave.LeaveType equals ltype.Id
                            join lbranch in Branches on emp.Branch equals lbranch.Id
                            join ldept in Departments on emp.Branch equals ldept.Id
                            join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                            where leave.StartDate.Date > leave.UpdatedDate.Date
                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                designation = ldesig.Code,
                                DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                ltype.Code,
                                leave.StartDate,
                                leave.EndDate,
                                leave.UpdatedDate,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.UpdatedDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            try
            {
                DateTime fromdate = Convert.ToDateTime(StartDate);
                DateTime todate = Convert.ToDateTime(EndDate);
                DateTime eStartDate = GetCurrentTime(DateTime.Now).Date;
                var lresult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
                if (StartDate != "" || EndDate != "")
                {

                    var data = (from leave in lleave
                                join emp in dbResult on leave.EmpId equals emp.Id
                                join ltype in type on leave.LeaveType equals ltype.Id
                                join lbranch in Branches on emp.Branch equals lbranch.Id
                                join ldept in Departments on emp.Branch equals ldept.Id
                                join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                                where leave.StartDate.Date > leave.UpdatedDate.Date
                                where (leave.StartDate >= fromdate && leave.EndDate <= todate) || (leave.StartDate <= fromdate && leave.EndDate >= todate)
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = ldesig.Code,
                                    DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                    ltype.Code,
                                    leave.StartDate,
                                    leave.EndDate,
                                    leave.UpdatedDate,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
                                }).OrderByDescending(a => a.UpdatedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var data = (from leave in lleave
                                join emp in dbResult on leave.EmpId equals emp.Id
                                join ltype in type on leave.LeaveType equals ltype.Id
                                join lbranch in Branches on emp.Branch equals lbranch.Id
                                join ldept in Departments on emp.Branch equals ldept.Id
                                join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                                where leave.StartDate.Date > leave.UpdatedDate.Date
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = ldesig.Code,
                                    DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                    ltype.Code,
                                    leave.StartDate,
                                    leave.EndDate,
                                    leave.UpdatedDate,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
                                }).OrderByDescending(a => a.UpdatedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        //Code for Download PDF for Future Leave Report
        public FileResult CreatePdfFutureLeave()
        {
            String sd = Convert.ToString(Session["sd"]);
            String ed = Convert.ToString(Session["ed"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("FutureLeaveList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 10 columns  
            PdfPTable tableLayout1 = new PdfPTable(11);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDFfuture(tableLayout1, sd, ed));
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
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page:" + i.ToString() + "/" + pages.ToString(), blackFont), 470f, 12f, 0);
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
            Session.Remove("sd");
            Session.Remove("ed");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDFfuture(PdfPTable tableLayout1, string sd1, string ed1)
        {
            float[] headers1 = { 27, 40, 35, 47, 35, 35, 38, 30, 40, 40, 30 }; //Header Widths 
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("FutureLeaveList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lleave = db.Leaves.ToList();
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            var type = db.LeaveTypes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (sd1 == "" || ed1 == "")
            {
                var data = (from leave in lleave
                            join emp in dbResult on leave.EmpId equals emp.Id
                            join ltype in type on leave.LeaveType equals ltype.Id
                            join lbranch in Branches on emp.Branch equals lbranch.Id
                            join ldept in Departments on emp.Branch equals ldept.Id
                            join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                            where leave.StartDate.Date > leave.UpdatedDate.Date
                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                designation = ldesig.Code,
                                DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                ltype.Code,
                                leave.StartDate,
                                leave.EndDate,
                                leave.UpdatedDate,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.UpdatedDate);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Department/Branch");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "Subject");
                AddCellToHeader(tableLayout1, "Reason");
                AddCellToHeader(tableLayout1, "Status");

                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.ShortName);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.DeptBranch);
                    AddCellToBody(tableLayout1, lemp.UpdatedDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.Subject);
                    AddCellToBody(tableLayout1, lemp.Reason);
                    AddCellToBody(tableLayout1, lemp.Status);
                }
                return tableLayout1;
            }
            else
            {
                DateTime fromdate = Convert.ToDateTime(sd1);
                DateTime todate = Convert.ToDateTime(ed1);
                var data = (from leave in lleave
                            join emp in dbResult on leave.EmpId equals emp.Id
                            join ltype in type on leave.LeaveType equals ltype.Id
                            join lbranch in Branches on emp.Branch equals lbranch.Id
                            join ldept in Departments on emp.Branch equals ldept.Id
                            join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                            where leave.StartDate.Date > leave.UpdatedDate.Date
                            where (leave.StartDate >= fromdate && leave.EndDate <= todate) || (leave.StartDate <= fromdate && leave.EndDate >= todate)
                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                designation = ldesig.Code,
                                DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                ltype.Code,
                                leave.StartDate,
                                leave.EndDate,
                                leave.UpdatedDate,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.UpdatedDate);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Department/Branch");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "Subject");
                AddCellToHeader(tableLayout1, "Reason");
                AddCellToHeader(tableLayout1, "Status");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.ShortName);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.DeptBranch);
                    AddCellToBody(tableLayout1, lemp.UpdatedDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.StartDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.EndDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.Subject);
                    AddCellToBody(tableLayout1, lemp.Reason);
                    AddCellToBody(tableLayout1, lemp.Status);
                }
                return tableLayout1;
            }
        }
        // Report For Late Leave Report
        public ActionResult LateLeave()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.BranchCode.ToString() + " " + x.Name.ToString(),
            });
            ViewBag.Branch = new SelectList(items, "Id", "Name");
            return View("~/Views/Reports/LateLeavelist.cshtml");
        }
        [HttpGet]
        public JsonResult LateLeaveView(string EmpId)
        {
            try
            {
                var lleave = db.Leaves.ToList();
                var ltransfer = db.Employee_Transfer.ToList();
                var dbResult = db.Employes.ToList();
                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                var type = db.LeaveTypes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from leave in lleave
                                join emp in dbResult on leave.EmpId equals emp.Id
                                join ltype in type on leave.LeaveType equals ltype.Id
                                join lbranch in Branches on emp.Branch equals lbranch.Id
                                join ldept in Departments on emp.Branch equals ldept.Id
                                join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                                where leave.StartDate.Date < leave.UpdatedDate.Date
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    ltype.Code,
                                    designation = ldesig.Code,
                                    DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                    leave.StartDate,
                                    leave.EndDate,
                                    leave.UpdatedDate,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
                                }).OrderByDescending(a => a.UpdatedDate);
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
        public JsonResult LateLeaveViews(string StartDate, string EndDate)
        {

            Session["sd"] = StartDate;
            Session["ed"] = EndDate;
            var lleave = db.Leaves.ToList();
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            var type = db.LeaveTypes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (StartDate == "")
            {
                var data = (from leave in lleave
                            join emp in dbResult on leave.EmpId equals emp.Id
                            join ltype in type on leave.LeaveType equals ltype.Id
                            join lbranch in Branches on emp.Branch equals lbranch.Id
                            join ldept in Departments on emp.Branch equals ldept.Id
                            join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                            where leave.StartDate.Date < leave.UpdatedDate.Date
                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                ltype.Code,
                                designation = ldesig.Code,
                                DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                leave.StartDate,
                                leave.EndDate,
                                leave.UpdatedDate,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.UpdatedDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (EndDate == "")
            {
                var data = (from leave in lleave
                            join emp in dbResult on leave.EmpId equals emp.Id
                            join ltype in type on leave.LeaveType equals ltype.Id
                            join lbranch in Branches on emp.Branch equals lbranch.Id
                            join ldept in Departments on emp.Branch equals ldept.Id
                            join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                            where leave.StartDate.Date < leave.UpdatedDate.Date
                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                ltype.Code,
                                designation = ldesig.Code,
                                DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                leave.StartDate,
                                leave.EndDate,
                                leave.UpdatedDate,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.UpdatedDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            try
            {
                DateTime fromdate = Convert.ToDateTime(StartDate);
                DateTime todate = Convert.ToDateTime(EndDate);
                DateTime eStartDate = GetCurrentTime(DateTime.Now).Date;
                var lresult = db.view_employee_DOB_RetirementDateMonthWise.ToList();
                if (StartDate != "" || EndDate != "")
                {

                    var data = (from leave in lleave
                                join emp in dbResult on leave.EmpId equals emp.Id
                                join ltype in type on leave.LeaveType equals ltype.Id
                                join lbranch in Branches on emp.Branch equals lbranch.Id
                                join ldept in Departments on emp.Branch equals ldept.Id
                                join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                                where leave.StartDate.Date < leave.UpdatedDate.Date
                                where (leave.StartDate <= fromdate && leave.EndDate >= todate) || (leave.StartDate >= fromdate && leave.EndDate <= todate)
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    ltype.Code,
                                    designation = ldesig.Code,
                                    DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                    leave.StartDate,
                                    leave.EndDate,
                                    leave.UpdatedDate,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
                                }).OrderByDescending(a => a.UpdatedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var data = (from leave in lleave
                                join emp in dbResult on leave.EmpId equals emp.Id
                                join ltype in type on leave.LeaveType equals ltype.Id
                                join lbranch in Branches on emp.Branch equals lbranch.Id
                                join ldept in Departments on emp.Branch equals ldept.Id
                                join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                                where leave.StartDate.Date < leave.UpdatedDate.Date
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    ltype.Code,
                                    designation = ldesig.Code,
                                    DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                    leave.StartDate,
                                    leave.EndDate,
                                    leave.UpdatedDate,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
                                }).OrderByDescending(a => a.UpdatedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        //Code for Download PDF for Late Leave Report
        public FileResult CreatePdfLateLeave()
        {
            String sd = Convert.ToString(Session["sd"]);
            String ed = Convert.ToString(Session["ed"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("LateLeaveList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 10 columns  
            PdfPTable tableLayout1 = new PdfPTable(11);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDFLate(tableLayout1, sd, ed));
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
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page:" + i.ToString() + "/" + pages.ToString(), blackFont), 470f, 12f, 0);
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
            Session.Remove("sd");
            Session.Remove("ed");
            return File(workStream, "application/pdf", strPDFFileName);
        }
        protected PdfPTable Add_Content_To_PDFLate(PdfPTable tableLayout1, string sd1, string ed1)
        {
            float[] headers1 = { 27, 40, 35, 47, 35, 35, 38, 30, 40, 40, 30 }; //Header Widths
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("LateLeaveList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            var lleave = db.Leaves.ToList();
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            var type = db.LeaveTypes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (sd1 == "" && ed1 == "")
            {
                var data = (from leave in lleave
                            join emp in dbResult on leave.EmpId equals emp.Id
                            join ltype in type on leave.LeaveType equals ltype.Id
                            join lbranch in Branches on emp.Branch equals lbranch.Id
                            join ldept in Departments on emp.Branch equals ldept.Id
                            join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                            where leave.StartDate.Date < leave.UpdatedDate.Date
                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                ltype.Code,
                                designation = ldesig.Code,
                                DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                leave.StartDate,
                                leave.EndDate,
                                leave.UpdatedDate,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
                            }).OrderByDescending(a => a.UpdatedDate);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Department/Branch");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "Subject");
                AddCellToHeader(tableLayout1, "Reason");
                AddCellToHeader(tableLayout1, "Status");

                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.ShortName);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.DeptBranch);
                    AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.Subject);
                    AddCellToBody(tableLayout1, lemp.Reason);
                    AddCellToBody(tableLayout1, lemp.Status);
                }
                return tableLayout1;
            }
            else
            {
                DateTime fromdate = Convert.ToDateTime(sd1);
                DateTime todate = Convert.ToDateTime(ed1);
                var data = (from leave in lleave
                            join emp in dbResult on leave.EmpId equals emp.Id
                            join ltype in type on leave.LeaveType equals ltype.Id
                            join lbranch in Branches on emp.Branch equals lbranch.Id
                            join ldept in Departments on emp.Branch equals ldept.Id
                            join ldesig in Designations on emp.CurrentDesignation equals ldesig.Id
                            where leave.StartDate.Date < leave.UpdatedDate.Date
                            where (leave.StartDate <= fromdate && leave.EndDate >= todate) || (leave.StartDate >= fromdate && leave.EndDate <= todate)
                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                ltype.Code,
                                designation = ldesig.Code,
                                DeptBranch = GetBranchDepartmentConcat(lbranch.Name, ldept.Name),
                                leave.StartDate,
                                leave.EndDate,
                                leave.UpdatedDate,
                                leave.Subject,
                                leave.Reason,
                                leave.Status
                            }).OrderByDescending(a => a.UpdatedDate);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "Name");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "Department/Branch");
                AddCellToHeader(tableLayout1, "AppliedDate");
                AddCellToHeader(tableLayout1, "StartDate");
                AddCellToHeader(tableLayout1, "EndDate");
                AddCellToHeader(tableLayout1, "Type");
                AddCellToHeader(tableLayout1, "Subject");
                AddCellToHeader(tableLayout1, "Reason");
                AddCellToHeader(tableLayout1, "Status");
                //Adding body  
                foreach (var lemp in data)
                {
                    AddCellToBody(tableLayout1, lemp.EmpId);
                    AddCellToBody(tableLayout1, lemp.ShortName);
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.DeptBranch);
                    AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.Subject);
                    AddCellToBody(tableLayout1, lemp.Reason);
                    AddCellToBody(tableLayout1, lemp.Status);
                }
                return tableLayout1;
            }
        }
        //Report for Work Dairy
        [HttpGet]
        public ActionResult workdiary()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/WorkDiaryReport.cshtml");
        }
        [HttpGet]
        [Route("allworkdairy")]
        public string allworkdairy()
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            ReportBusiness Rbus = new ReportBusiness();
            var data = Rbus.getAllWorkDairies();
            return JsonConvert.SerializeObject(data);
        }
        [HttpPost]
        public string workdairysearch(string StartDate,string fromDate, string toDate, string EmpId ,string status)
        {
            Session["lworkDate"] = StartDate;
            Session["lEmpId"] = EmpId;
            ReportBusiness Rbussearch = new ReportBusiness();
            var data = Rbussearch.getallWorkdairiessearch(StartDate,fromDate,  toDate, EmpId,status);
            return JsonConvert.SerializeObject(data);
        }
        // Creating PDF for Work Dairy Report
        public FileResult CreatePdfWorkDairy()
        {
            string lworkDate = Convert.ToString(Session["lworkDate"]);
            string lEmpId = Convert.ToString(Session["lEmpId"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("WorkDairyList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 7 columns  
            PdfPTable tableLayout1 = new PdfPTable(7);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDFwd(tableLayout1));
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
            Session.Remove("lEmpId");
            return File(workStream, "application/pdf", strPDFFileName);

        }
        protected PdfPTable Add_Content_To_PDFwd(PdfPTable tableLayout1)
        {
            string lworkDate = string.Empty;
            string lEmpId = Convert.ToString(Session["lEmpId"]);
            lworkDate = Convert.ToString(Session["lworkDate"]);
            if (lworkDate != "")
            {
                DateTime star1 = DateTime.Parse(lworkDate);
                lworkDate = star1.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            ReportBusiness Rbus = new ReportBusiness();
            float[] headers1 = { 7, 15, 15, 10, 35, 45, 10 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("WorkDairyList", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
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
            if (lworkDate == "" && lEmpId == "")
            {

                var data = Rbus.WorkDiaryPDF(lworkDate, lEmpId);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "WorkDate");
                AddCellToHeader(tableLayout1, "WorkName");
                AddCellToHeader(tableLayout1, "WorkDescription");
                AddCellToHeader(tableLayout1, "Status");

                //Adding body  
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    string lempid = (string)data.Rows[i]["EmpId"].ToString();
                    string lShortName = (string)data.Rows[i]["ShortName"].ToString();
                    string lCode = (string)data.Rows[i]["Designation"].ToString();
                    DateTime lwdate = (DateTime)data.Rows[i]["WDDate"];
                    string lworkName = (string)data.Rows[i]["Name"].ToString();
                    string lworkDesc = (string)data.Rows[i]["Desc"].ToString();
                    string lStatus = (string)data.Rows[i]["Status"].ToString();
                    AddCellToBody(tableLayout1, lempid);
                    AddCellToBody(tableLayout1, lShortName);
                    AddCellToBody(tableLayout1, lCode);
                    AddCellToBody(tableLayout1, lwdate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lworkName);
                    AddCellToBody(tableLayout1, lworkDesc);
                    AddCellToBody(tableLayout1, lStatus);
                }
                return tableLayout1;
            }
            else if (lworkDate != "" && lEmpId != "")
            {
                var data1 = Rbus.WorkDiaryPDF(lworkDate, lEmpId);

                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "WorkDate");
                AddCellToHeader(tableLayout1, "WorkName");
                AddCellToHeader(tableLayout1, "WorkDescription");
                AddCellToHeader(tableLayout1, "Status");

                //Adding body  
                for (int i = 0; i < data1.Rows.Count; i++)
                {
                    string lempid = (string)data1.Rows[i]["EmpId"].ToString();
                    string lShortName = (string)data1.Rows[i]["ShortName"].ToString();
                    string lCode = (string)data1.Rows[i]["Designation"].ToString();
                    DateTime lwdate = (DateTime)data1.Rows[i]["WDDate"];
                    string lworkName = (string)data1.Rows[i]["Name"].ToString();
                    string lworkDesc = (string)data1.Rows[i]["Desc"].ToString();
                    string lStatus = (string)data1.Rows[i]["Status"].ToString();
                    AddCellToBody(tableLayout1, lempid);
                    AddCellToBody(tableLayout1, lShortName);
                    AddCellToBody(tableLayout1, lCode);
                    AddCellToBody(tableLayout1, lwdate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lworkName);
                    AddCellToBody(tableLayout1, lworkDesc);
                    AddCellToBody(tableLayout1, lStatus);
                }
                return tableLayout1;
            }
            else if (lworkDate != "")
            {
                var data2 = Rbus.WorkDiaryPDF(lworkDate, lEmpId);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "WorkDate");
                AddCellToHeader(tableLayout1, "WorkName");
                AddCellToHeader(tableLayout1, "WorkDescription");
                AddCellToHeader(tableLayout1, "Status");

                //Adding body  
                for (int i = 0; i < data2.Rows.Count; i++)
                {
                    string lempid = (string)data2.Rows[i]["EmpId"].ToString();
                    string lShortName = (string)data2.Rows[i]["ShortName"].ToString();
                    string lCode = (string)data2.Rows[i]["Designation"].ToString();
                    DateTime lwdate = (DateTime)data2.Rows[i]["WDDate"];
                    string lworkName = (string)data2.Rows[i]["Name"].ToString();
                    string lworkDesc = (string)data2.Rows[i]["Desc"].ToString();
                    string lStatus = (string)data2.Rows[i]["Status"].ToString();
                    AddCellToBody(tableLayout1, lempid);
                    AddCellToBody(tableLayout1, lShortName);
                    AddCellToBody(tableLayout1, lCode);
                    AddCellToBody(tableLayout1, lwdate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lworkName);
                    AddCellToBody(tableLayout1, lworkDesc);
                    AddCellToBody(tableLayout1, lStatus);
                }
                return tableLayout1;
            }
            else if (lEmpId != "")
            {
                var data3 = Rbus.WorkDiaryPDF(lworkDate, lEmpId);
                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "WorkDate");
                AddCellToHeader(tableLayout1, "WorkName");
                AddCellToHeader(tableLayout1, "WorkDescription");
                AddCellToHeader(tableLayout1, "Status");

                //Adding body  
                for (int i = 0; i < data3.Rows.Count; i++)
                {
                    string lempid = (string)data3.Rows[i]["EmpId"].ToString();
                    string lShortName = (string)data3.Rows[i]["ShortName"].ToString();
                    string lCode = (string)data3.Rows[i]["Designation"].ToString();
                    DateTime lwdate = (DateTime)data3.Rows[i]["WDDate"];
                    string lworkName = (string)data3.Rows[i]["Name"].ToString();
                    string lworkDesc = (string)data3.Rows[i]["Desc"].ToString();
                    string lStatus = (string)data3.Rows[i]["Status"].ToString();
                    AddCellToBody(tableLayout1, lempid);
                    AddCellToBody(tableLayout1, lShortName);
                    AddCellToBody(tableLayout1, lCode);
                    AddCellToBody(tableLayout1, lwdate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout1, lworkName);
                    AddCellToBody(tableLayout1, lworkDesc);
                    AddCellToBody(tableLayout1, lStatus);
                }
                return tableLayout1;
            }
            return tableLayout1;
        }
        //Report for PL
        [HttpGet]
        public ActionResult Plreport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Reports/PL.cshtml");
        }
        [HttpGet]
        [Route("allPL")]
        public string allPL()
        {
            ReportBusiness Rbus = new ReportBusiness();
            var data = Rbus.getAllPL();
            return JsonConvert.SerializeObject(data);
        }
        public string Plsearch(string Status, string EmpId)
        {
            Session["lstatus"] = Status;
            Session["lEmpId"] = EmpId;
            ReportBusiness Rbus = new ReportBusiness();
            var data = Rbus.getAllPLsearch(Status, EmpId);
            return JsonConvert.SerializeObject(data);
        }
        //Creating a PDF File for PL Report
        public FileResult CreatePdfPL()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("PLList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 8 columns  
            PdfPTable tableLayout1 = new PdfPTable(8);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDFPL(tableLayout1));
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
            return File(workStream, "application/pdf", strPDFFileName);
        }

        protected PdfPTable Add_Content_To_PDFPL(PdfPTable tableLayout1)
        {
            string lstatus = Convert.ToString(Session["lstatus"]);
            string lempid = Convert.ToString(Session["lEmpId"]);
            float[] headers1 = { 25, 35, 32, 35, 35, 35, 35, 35 }; //Header Widths  
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
            tableLayout1.AddCell(new PdfPCell(new Phrase("PLList", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
            });
            if (lstatus == "" || lempid == "")
            {
                var lemp = db.Employes.ToList();
                var ldesig = db.Designations.ToList();
                var lpl = db.PLE_Type.ToList();
                var data = (from pl in lpl
                            join emp in lemp on pl.EmpId equals emp.Id
                            join desig in ldesig on emp.CurrentDesignation equals desig.Id
                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                desig.Code,
                                pl.TotalExperience,
                                pl.TotalPL,
                                pl.PLEncash,
                                pl.Subject,
                                pl.Status,
                            });

                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "TotalExperience");
                AddCellToHeader(tableLayout1, "TotalPL");
                AddCellToHeader(tableLayout1, "PLEncash");
                AddCellToHeader(tableLayout1, "Subject");
                AddCellToHeader(tableLayout1, "Status");

                foreach (var ldata in data)
                {
                    AddCellToBody(tableLayout1, ldata.EmpId);
                    AddCellToBody(tableLayout1, ldata.ShortName);
                    AddCellToBody(tableLayout1, ldata.Code);
                    AddCellToBody(tableLayout1, ldata.TotalExperience);
                    AddCellToBody(tableLayout1, ldata.TotalPL);
                    AddCellToBody(tableLayout1, ldata.PLEncash);
                    AddCellToBody(tableLayout1, ldata.Subject);
                    AddCellToBody(tableLayout1, ldata.Status);
                }
                return tableLayout1;
            }
            else
            {
                var lemp = db.Employes.ToList();
                var ldesig = db.Designations.ToList();
                var lpl = db.PLE_Type.ToList();
                var data = (from pl in lpl
                            join emp in lemp on pl.EmpId equals emp.Id
                            join desig in ldesig on emp.CurrentDesignation equals desig.Id
                            where emp.EmpId == lempid
                            where pl.Status == lstatus
                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                desig.Code,
                                pl.TotalExperience,
                                pl.TotalPL,
                                pl.PLEncash,
                                pl.Subject,
                                pl.Status,
                            });

                //Adding headers  
                AddCellToHeader(tableLayout1, "EmpId");
                AddCellToHeader(tableLayout1, "EmpName");
                AddCellToHeader(tableLayout1, "Designation");
                AddCellToHeader(tableLayout1, "TotalExperience");
                AddCellToHeader(tableLayout1, "TotalPL");
                AddCellToHeader(tableLayout1, "PLEncash");
                AddCellToHeader(tableLayout1, "Subject");
                AddCellToHeader(tableLayout1, "Status");

                foreach (var ldata in data)
                {
                    AddCellToBody(tableLayout1, ldata.EmpId);
                    AddCellToBody(tableLayout1, ldata.ShortName);
                    AddCellToBody(tableLayout1, ldata.Code);
                    AddCellToBody(tableLayout1, ldata.TotalExperience);
                    AddCellToBody(tableLayout1, ldata.TotalPL);
                    AddCellToBody(tableLayout1, ldata.PLEncash);
                    AddCellToBody(tableLayout1, ldata.Subject);
                    AddCellToBody(tableLayout1, ldata.Status);
                }
                return tableLayout1;
            }
        }
        public void ExportGridToExcelLeaves()
        {
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string status = Convert.ToString(Session["lstatus"]);
                string empid = Convert.ToString(Session["lempid"]);
                if (status == "" && empid == "")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    where leave.Status == "Approved" && emp2.Id == lEmpId || leave.Status == "Forwarded" && emp1.Id == lEmpId
                                    select new
                                    {
                                        EmpCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        Designation = desig.Code,
                                        StatusApprovedBy = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var gv = new GridView();
                    gv.DataSource = lResults;
                    if (lResults.Count() == 0)
                    {
                        gv.ShowHeaderWhenEmpty = true;
                        Response.AddHeader("content-disposition", "attachment; filename=MyLeaveApprovalList.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.End();
                    }
                    gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=MyLeaveApprovalList.xls");
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
                }
                else if (status == "Forwarded" && empid != "")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    where leave.Status == status && emp1.Id == lEmpId && emp.EmpId == empid
                                    select new
                                    {
                                        EmpCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        Designation = desig.Code,
                                        StatusApprovedBy = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var gv = new GridView();
                    gv.DataSource = lResults;
                    if (lResults.Count() == 0)
                    {
                        gv.ShowHeaderWhenEmpty = true;
                        Response.AddHeader("content-disposition", "attachment; filename=MyLeaveApprovalList.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.End();
                    }
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=MyLeaveApprovalList.xls");
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
                }
                else if (status == "Approved" && empid != "")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    where leave.Status == status && emp2.Id == lEmpId && emp.EmpId == empid
                                    select new
                                    {
                                        EmpCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        Designation = desig.Code,
                                        StatusApprovedBy = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var gv = new GridView();
                    gv.DataSource = lResults;
                    if (lResults.Count() == 0)
                    {
                        gv.ShowHeaderWhenEmpty = true;
                        Response.AddHeader("content-disposition", "attachment; filename=MyLeaveApprovalList.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.End();
                    }
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=MyLeaveApprovalList.xls");
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
                }
                else if (status == "All" && empid != "")
                {
                    var lleaves = db.Leaves.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var lResults = (from leave in lleaves
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    join emp2 in lemployees on leave.SanctioningAuthority equals emp2.Id
                                    join emp1 in lemployees on leave.ControllingAuthority equals emp1.Id
                                    where (leave.Status == "Approved" && emp2.Id == lEmpId || leave.Status == "Forwarded" && emp1.Id == lEmpId) && emp.EmpId == empid
                                    select new
                                    {
                                        EmpCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        Designation = desig.Code,
                                        StatusApprovedBy = GetControlSanctionAuthority(emp1.EmpId, emp2.EmpId, leave.Status),
                                        ApprovedBy = GetControlSanctionAuthority(emp1.ShortName, emp2.ShortName, leave.Status),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        leave.Status,
                                    }).OrderByDescending(a => a.ApprovedTime);
                    var gv = new GridView();
                    gv.DataSource = lResults;
                    if (lResults.Count() == 0)
                    {
                        gv.ShowHeaderWhenEmpty = true;
                        Response.AddHeader("content-disposition", "attachment; filename=MyLeaveApprovalList.xls");
                        Response.ContentType = "application/ms-excel";
                        Response.End();
                    }
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=MyLeaveApprovalList.xls");
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
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        //Code for Export To Excel for Sanctioning History in Work Dairy.
        public void ExportGridToExcelSanctionWD()
        {
            try
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                ReportBusiness Rbus = new ReportBusiness();
                var data = Rbus.getSanctionWorkApproval(lCredentials.EmpPkId);
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
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        //Code for Export To Excel for Sanctioning History in Work Dairy.
        public void WorkDiaryExcel()
        {
            try
            {
                string sqlDate = string.Empty;
                string lworkDate = Convert.ToString(Session["lworkDate"]);
                string lEmpId = Convert.ToString(Session["lEmpId"]);
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                if (lworkDate != "")
                {
                    DateTime star1 = DateTime.Parse(lworkDate);
                    sqlDate = star1.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
                ReportBusiness Rbus = new ReportBusiness();
                var data = Rbus.WorkDiaryExcel(sqlDate, lEmpId);
                var gv = new GridView();
                gv.DataSource = data;
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

            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        [HttpPost]
        public JsonResult PermViews(string EmpId, string effectivefrom)
        {
            Session["ecode"] = EmpId;
            Session["ef"] = effectivefrom;
            var ltransfer = db.Employee_Transfer.ToList();
            var dbResult = db.Employes.ToList();
            //var Banks = db.Banks.ToList();
            var Branches = db.Branches.ToList();
            var Departments = db.Departments.ToList();
            var Designations = db.Designations.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (EmpId == "" || effectivefrom =="")
            {
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "PermanentTransfer"
                            where emplist.RetirementDate >= lStartDate
                            select new
                            {

                                transfer.Id,
                                empid = emplist.EmpId,
                                transfer.Type,
                                EmpName = emplist.ShortName,
                                olddesignation = desig.Code,
                                newdesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,
                                transfer.EffectiveTo,
                            }).OrderByDescending(a => a.EffectiveFrom);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (EmpId != "" && effectivefrom != "")
            {
                DateTime fromdate = Convert.ToDateTime(effectivefrom);
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "PermanentTransfer"
                            where emplist.EmpId == EmpId && transfer.EffectiveFrom == fromdate
                            select new
                            {

                                transfer.Id,
                                empid = emplist.EmpId,
                                transfer.Type,
                                EmpName = emplist.ShortName,
                                olddesignation = desig.Code,
                                newdesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,
                                transfer.EffectiveTo,
                            }).OrderByDescending(a => a.EffectiveFrom);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = (from transfer in ltransfer
                            join emplist in dbResult on transfer.EmpId equals emplist.Id
                            join branchlist in Branches on transfer.OldBranch equals branchlist.Id
                            join newbranch in Branches on transfer.NewBranch equals newbranch.Id
                            join desig in Designations on transfer.OldDesignation equals desig.Id
                            join desig1 in Designations on transfer.NewDesignation equals desig1.Id
                            join dept in Departments on transfer.OldDepartment equals dept.Id
                            join newdept in Departments on transfer.NewDepartment equals newdept.Id
                            where transfer.Type == "PermanentTransfer"
                            select new
                            {
                                transfer.Id,
                                empid = emplist.EmpId,
                                transfer.Type,
                                EmpName = emplist.ShortName,
                                olddesignation = desig.Code,
                                newdesignation = desig1.Code,
                                oldDeptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                newDeptbranch = GetBranchDepartmentConcat(newbranch.Name, newdept.Name),
                                transfer.EffectiveFrom,
                                transfer.EffectiveTo,
                            }).OrderByDescending(a => a.EffectiveFrom);
                return Json(data, JsonRequestBehavior.AllowGet);
            }   
            }
        public void ExportGridToExcelSM()
        {
            string lempid = Convert.ToString(Session["lempid"]);
            var lemployee = db.view_employee_transfer.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartments = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            var ltransfer = db.Employee_Transfer.ToList();
            var lemp = db.Employes.ToList();
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            if (lempid == "")
            {
                var employeeList = (from emp in lemployee          
                                    where emp.RetirementDate >= lStartDate
                                    select new
                                    {
                                        emp.EmpId,
                                        emp.EmpName,
                                        Designation = emp.Designation,
                                        emp.DOB,
                                        emp.DOJ,
                                        emp.RetirementDate,
                                        emp.FatherName,
                                        emp.MotherName,
                                        EmpAddress = emp.PresentAddress,
                                        emp.MobileNumber,
                                        emp.Category,
                                        emp.Graduation,
                                        emp.PostGraduation,
                                        emp.ProfessionalQualifications,
                                        PresentWorkPlace = GetBranchDepartmentConcat(emp.BranchName, emp.DeptName),
                                        From = GetBranchDepartmentConcat(GetOldTransferValues(emp.OldBranch), GetNewTransferValues(emp.OldDepartment)),
                                        To = GetBranchDepartmentConcat(GetOldTransferValues(emp.NewBranch), GetNewTransferValues(emp.NewDepartment)),
                                        Date = emp.EffectiveFrom.ToString(),
                                    }).ToList();

                var gv = new GridView();
                gv.DataSource = employeeList;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=StaffMasterReport.xls");
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
                Session.Remove("lempid");
                Response.End();
            }
            else
            {
                var employeeList = (from emp in lemployee                      
                                    where emp.RetirementDate >= lStartDate
                                    where emp.EmpName == lempid || emp.EmpId == lempid || emp.Designation == lempid
                                    select new
                                    {
                                        emp.EmpId,
                                        emp.EmpName,
                                        Designation = emp.Designation,
                                        emp.DOB,
                                        emp.DOJ,
                                        emp.RetirementDate,
                                        emp.FatherName,
                                        emp.MotherName,
                                        EmpAddress = emp.PresentAddress,
                                        emp.MobileNumber,
                                        emp.Category,
                                        emp.Graduation,
                                        emp.PostGraduation,
                                        emp.ProfessionalQualifications,
                                        PresentWorkPlace = GetBranchDepartmentConcat(emp.BranchName, emp.DeptName),
                                        From = GetBranchDepartmentConcat(GetOldTransferValues(emp.OldBranch), GetNewTransferValues(emp.OldDepartment)),
                                        To = GetBranchDepartmentConcat(GetOldTransferValues(emp.NewBranch), GetNewTransferValues(emp.NewDepartment)),
                                        Date = emp.EffectiveFrom.ToString(),
                                    }).ToList();
                var gv = new GridView();
                gv.DataSource = employeeList;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=StaffMasterReport.xls");
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
                Session.Remove("lempid");
                Response.End();
            }
        }
    }
}
