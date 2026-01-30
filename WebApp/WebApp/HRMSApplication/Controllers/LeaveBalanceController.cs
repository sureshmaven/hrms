using Entities;
using HRMSApplication.Filters;
using HRMSApplication.Helpers;
using HRMSApplication.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMSBusiness.Comm;
using HRMSBusiness.Db;
using System.Data;
using Newtonsoft.Json;
using HRMSBusiness.Reports;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class LeaveBalanceController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        // GET: LeaveBalance
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        [HttpGet]
        public ActionResult LeaveBalance()
        {
            string lMessage = string.Empty;
            try
            {
                var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!='OtherBranch' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
                var items5 = dt.AsEnumerable().Select(r => new Branches
                {
                    Id = (Int32)(r["Id"]),
                    Name = (string)(r["Name"] ?? "null")
                }).ToList();

                items5.Insert(0, new Branches
                {
                    Id = 0,
                    Name = "All"
                });

                items5.Insert(45, new Branches
                {
                    Id = 45,
                    Name = "HeadOffice-All"
                });


                ViewBag.Branch = new SelectList(items5, "Name", "Name");


                //var dt1 = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Departments where Name!='HeadOffice' and Active=1");
                //var items6 = dt1.AsEnumerable().Select(r => new Departments
                //{
                //    Id = (Int32)(r["Id"]),
                //    Name = (string)(r["Name"] ?? "null")
                //}).ToList();

                //items6.Insert(0, new Departments
                //{
                //    Id = -1,
                //    Name = "All"
                //});

                //ViewBag.Departments = new SelectList(items6, "Id", "Name");

                var items = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().Select(x => new LeaveTypes
                {
                    Type = x.Type.Trim(),
                });
                ViewBag.LeaveTypes = new SelectList(items, "Type", "Type");

                V_LeaveHistory lmodel = new V_LeaveHistory();
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                return View("~/Views/LeaveBalance/_EmpLeaveBalance.cshtml", lmodel);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }

            return View(lMessage);
        }

        public DateTime getFormatDate(DateTime dt)
        {

            DateTime theDate = dt;
            string myTime = theDate.ToString("MM/dd/yyyy");
            return Convert.ToDateTime(myTime);
        }
        [NoDirectAccess]
        public ActionResult LeaveBalanceView()
        {

            return View();
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
        public JsonResult LeaveBalanceViews(string StartDate)
        {
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var lleaveHistory = db.V_LeaveHistory.ToList();
                var lemployees = db.Employes.ToList();
                var lleaveBalance = db.V_EmpLeaveBalance.ToList();
                var ldepartments = db.Departments.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var lDesignation = db.Designations.ToList();
                var lBranches = db.Branches.ToList();

                var date = DateTime.Now;
                if (lCredentials.LoginMode == Constants.SuperAdmin || lCredentials.LoginMode == Constants.AdminHRDPolicy)
                {
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where empbal.RowId == 1 && emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),

                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }

        [HttpPost]
        public JsonResult LeaveBalanceView(FormCollection formValues, V_LeaveHistory leave)
        {
            string EmployeeCode = formValues["EmployeeCode"];
            string FirstName = formValues["FirstName"];
            string LastName = formValues["LastName"];
            string Branch = formValues["Branch"];
            Branch = Branch.Trim();
            if (Branch.StartsWith("-"))
            {
                Branch = Branch.Remove(0, 1);
            }
            //string radiobuttonvalue = formValues["radio"];
            String Department = Convert.ToString(leave.Department);
            DateTime FromDate = Convert.ToDateTime(formValues["SDate"]);
            DateTime ToDate = Convert.ToDateTime(formValues["EDate"]);
            string lMessage = string.Empty;
            var lleaveHistory = db.V_LeaveHistory.ToList();
            var lleaveBalance = db.V_EmpLeaveBalance.ToList();
            var lLeaveTypes = db.LeaveTypes.ToList();
            var lemployees = db.Employes.ToList();
            var ldepartments = db.Departments.ToList();
            var lDesignation = db.Designations.ToList();
            var lBranches = db.Branches.ToList();

            var date = DateTime.Now;
            try
            {
                if (EmployeeCode == "" && FirstName == "" && Branch == "")
                {
                    var lResult = (from empbal in lleaveBalance
                                   join emp in lemployees on empbal.EmpId equals emp.Id
                                   join branch in lBranches on emp.Branch equals branch.Id
                                   join dept in ldepartments on emp.Department equals dept.Id
                                   join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                   where emp.RetirementDate >= date.Date
                                   select new
                                   {
                                       EmployeeCode = emp.EmpId,
                                       EmployeeName = emp.ShortName,
                                       emp.ShortName,
                                       Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                       ldes.Code,
                                       empbal.CasualLeave,
                                       empbal.MedicalSickLeave,
                                       empbal.PrivilegeLeave,
                                       empbal.MaternityLeave,
                                       empbal.PaternityLeave,
                                       empbal.ExtraOrdinaryLeave,
                                       empbal.SpecialCasualLeave,
                                       empbal.LOP,
                                       empbal.CompensatoryOff,
                                       empbal.CWOFF,
                                       empbal.SpecialMedicalLeave,
                                   });
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                }
                else if (EmployeeCode == "" && FirstName != "" && Branch == "")
                {
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.ShortName.ToLower().Contains(FirstName.ToLower())
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);
                }
                else if (EmployeeCode != "" && FirstName != "" && Branch == "")
                {
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.ShortName.ToLower().Contains(FirstName.ToLower()) && emp.EmpId == EmployeeCode
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }
                else if (EmployeeCode != "" && FirstName == "" && Branch == "")
                {
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.EmpId == EmployeeCode
                                    //where emp.Department == 46
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,

                                        empbal.SpecialMedicalLeave,

                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);
                }

                else if (EmployeeCode != "" && FirstName != "" && Branch != "" && Branch != "HeadOffice-All" && Branch != "All")
                {
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where ((branch.Name == Branch || dept.Name == Branch) && (emp.ShortName.ToLower().Contains(FirstName.ToLower())) && emp.EmpId == EmployeeCode)
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,

                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }
                else if (EmployeeCode != "" && FirstName != "" && Branch != "" && Branch != "HeadOffice-All" && Branch == "All")
                {
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.ShortName.ToLower().Contains(FirstName.ToLower()) && emp.EmpId == EmployeeCode
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,

                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }
                else if (EmployeeCode != "" && FirstName != "" && Branch != "" && Branch == "HeadOffice-All" && Branch != "All")
                {
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where (branch.Id == 43 && dept.Id != 46 && emp.ShortName.ToLower().Contains(FirstName.ToLower()) && emp.EmpId == EmployeeCode)
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,

                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }

                else if (EmployeeCode == "" && FirstName == "" && Branch != "" && Branch != "HeadOffice-All" && Branch != "All")
                {
                    var BranchId = db.Branches.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();
                    var DeptId = db.Departments.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where (branch.Id == Convert.ToInt32(BranchId) || dept.Id == Convert.ToInt32(DeptId))
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }
                else if (EmployeeCode == "" && FirstName == "" && Branch != "" && Branch != "HeadOffice-All" && Branch == "All")
                {
                    var BranchId = db.Branches.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();
                    var DeptId = db.Departments.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }
                else if (EmployeeCode == "" && FirstName == "" && Branch != "" && Branch == "HeadOffice-All" && Branch != "All")
                {
                    var BranchId = db.Branches.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();
                    var DeptId = db.Departments.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where branch.Id == 43 && dept.Id != 46
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }

                else if (EmployeeCode != "" && FirstName == "" && Branch != "" && Branch == "HeadOffice-All" && Branch != "All")
                {
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.EmpId == EmployeeCode
                                    && emp.Department != 46 && emp.Branch == 43
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }
                else if (EmployeeCode != "" && FirstName == "" && Branch != "" && Branch != "HeadOffice-All" && Branch != "All")
                {
                    var BranchId = db.Branches.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();
                    var DeptId = db.Departments.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();

                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.EmpId == EmployeeCode && (branch.Id == Convert.ToInt32(BranchId) || dept.Id == Convert.ToInt32(DeptId))
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }
                else if (EmployeeCode != "" && FirstName == "" && Branch != "" && Branch != "HeadOffice-All" && Branch == "All")
                {
                    var BranchId = db.Branches.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();
                    var DeptId = db.Departments.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();

                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.EmpId == EmployeeCode
                                    //&& branch.Id == 43 && dept.Id !=46
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }

                else if (EmployeeCode == "" && FirstName != "" && Branch != "" && Branch != "HeadOffice-All" && Branch != "All")
                {
                    var BranchId = db.Branches.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();
                    var DeptId = db.Departments.Where(a => a.Name == Branch).Select(a => a.Id).FirstOrDefault();
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.ShortName.ToLower().Contains(FirstName.ToLower())
                                    && (branch.Id == Convert.ToInt32(BranchId) || dept.Id == Convert.ToInt32(DeptId))
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }
                else if (EmployeeCode == "" && FirstName != "" && Branch != "" && Branch != "HeadOffice-All" && Branch == "All")
                {
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.ShortName.ToLower().Contains(FirstName.ToLower())
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,
                                        empbal.CWOFF,
                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }
                else if (EmployeeCode == "" && FirstName != "" && Branch != "" && Branch == "HeadOffice-All" && Branch != "All")
                {
                    var lResults = (from empbal in lleaveBalance
                                    join emp in lemployees on empbal.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join dept in ldepartments on emp.Department equals dept.Id
                                    join ldes in lDesignation on emp.CurrentDesignation equals ldes.Id
                                    where emp.ShortName.ToLower().Contains(FirstName.ToLower()) && branch.Id == 43 && dept.Id != 46
                                    where emp.RetirementDate >= date.Date
                                    select new
                                    {
                                        EmployeeCode = emp.EmpId,
                                        EmployeeName = emp.ShortName,
                                        emp.ShortName,
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        ldes.Code,
                                        empbal.CasualLeave,
                                        empbal.MedicalSickLeave,
                                        empbal.PrivilegeLeave,
                                        empbal.MaternityLeave,
                                        empbal.PaternityLeave,
                                        empbal.ExtraOrdinaryLeave,
                                        empbal.SpecialCasualLeave,
                                        empbal.LOP,
                                        empbal.CompensatoryOff,

                                        empbal.SpecialMedicalLeave,
                                    });
                    return Json(lResults.Distinct(), JsonRequestBehavior.AllowGet);

                }

            }

            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }

        [HttpGet]
        public JsonResult YearViewBalances(string empid)
        {

            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
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
                               EmployeeName = userslist.FirstName + "" + userslist.LastName,
                               Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                               desig.Name,

                           });
            var lresponseArray = lResult.ToArray();
            string employeeId = lresponseArray[0].EmpId;
            string employeeName = lresponseArray[0].EmployeeName;
            string Deptbranchs = lresponseArray[0].Deptbranch;
            string ldesignation = lresponseArray[0].Name;
            return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation }, JsonRequestBehavior.AllowGet);
        }
        //for employee year balance
        [HttpGet]
        public ActionResult YearBalance()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/LeaveBalance/YearWiseLeaveBalance.cshtml");
        }

        [HttpGet]
        public string YearWiseLeaveBalanceViews(string EmpId)
        {
            try
            {

                ReportBusiness Rbus = new ReportBusiness();
                string EmpIds = lCredentials.EmpId;
                return JsonConvert.SerializeObject(Rbus.YearWLBalance(EmpIds));
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }
        //Leave Credit
        public int TotalBal(int Leavetypes, int lempid)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int lcreditleave = 0;
            //var lAppliedLeaves = db.Leaves.Where(a => a.LeaveType == Leavetypes).Where(a => a.EmpId == lempid)
            //    .Where(a => a.Status == "Approved").Sum(a => (int?)a.LeaveDays) ?? 0;
            //var lbal1 = db.Leaves_CarryForward.Where(a => a.LeaveTypeId == Leavetypes).Where(a => a.EmpId == lempid).Select(a => a.LeaveBalance).FirstOrDefault();
            var lAppliedLeaves1 = db.Leaves_LTC.Where(a => a.LeaveType == Leavetypes.ToString()).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Cancelled").Where(a => a.Status == "Denied").
               Sum(a => (int?)a.TotalDays) ?? 0;
            var lbal = db.leaves_CreditDebit.Where(a => a.LeaveTypeId == Leavetypes).Where(a => a.EmpId == lEmpId).Where(a => a.type == "Credit").Sum(a => (int?)a.CreditLeave) ?? 0;
            lcreditleave = lbal + lAppliedLeaves1;
            return lcreditleave;
        }
        //Total Balance
        public int LeaveBalance(int Leavetypes, int lempid)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int lcreditleave = 0;
            var lbal = db.EmpLeaveBalance.Where(a => a.LeaveTypeId == Leavetypes).Where(a => a.EmpId == lEmpId).Select(a => a.LeaveBalance).FirstOrDefault();
            //var lAppliedLeaves = db.Leaves.Where(a => a.LeaveType == Leavetypes).Where(a => a.EmpId == lempid)
            //    .Where(a => a.Status == "Approved").Sum(a => (int?)a.LeaveDays) ?? 0;
            lcreditleave = lbal;
            return lcreditleave;
        }

        public int TotalAppliedLeaves(int total, int Leavetypes, int lempid)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int ltotalapplied = 0;
            var lAppliedLeaves = db.Leaves.Where(a => a.LeaveType == Leavetypes).Where(a => a.EmpId == lEmpId)
                .Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.EmpId == lEmpId).Where(a => a.Status != "PartialCancelled").Sum(a => (int?)a.LeaveDays) ?? 0;
            if (lAppliedLeaves != 0 && total != 0)
            {
                ltotalapplied = (total - lAppliedLeaves);
            }
            if (lAppliedLeaves == 0)
            {
                ltotalapplied = total;
            }
            return ltotalapplied;
        }

        //Leave Debit
        public int ConsumedLeaves(int Leavetypes, int lempid)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int ltotalapplied = 0;
            //var lAppliedLeaves = db.Leaves.Where(a => a.LeaveType == Leavetypes).Where(a => a.EmpId == lempid).Where(a=>a.Status!="Credited").Where(a=>a.Status!="Debited")
            //    .Sum(a => (int?)a.LeaveDays) ?? 0;
            //var lbal = db.leaves_CreditDebit.Where(a => a.LeaveTypeId == Leavetypes).Where(a => a.type == "Debit").Where(a => a.EmpId == lempid).Sum(a => (int?)a.DebitLeave) ?? 0;
            //{
            //    ltotalapplied = lAppliedLeaves+ lbal;
            //}
            var lAppliedLeaves = db.Leaves.Where(a => a.LeaveType == Leavetypes).Where(a => a.EmpId == lEmpId).Where(a => a.Status != "Credited").Where(a => a.Status != "Cancelled").Where(a => a.Status != "PartialCancelled").Where(a => a.Status != "Denied")
                .Sum(a => (int?)a.LeaveDays) ?? 0;
            var lAppliedLeaves1 = db.Leaves_LTC.Where(a => a.LeaveType == Leavetypes.ToString()).Where(a => a.EmpId == lEmpId).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").
               Sum(a => (int?)a.TotalDays) ?? 0;
            //ltotalapplied = lAppliedLeaves + lbal;
            ltotalapplied = lAppliedLeaves + lAppliedLeaves1;
            return ltotalapplied;
        }
    }

}