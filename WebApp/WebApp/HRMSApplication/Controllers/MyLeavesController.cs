using Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facade;
using HRMSApplication.Models;
using HRMSApplication.Helpers;
using System.Data.Entity;
using HRMSApplication.Filters;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using HRMSBusiness.Comm;
namespace HRMSApplication.Controllers
{
    [Authorize]
    public class MyLeavesController : Controller
    {
        private ContextBase db = new ContextBase();
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MyLeavesController));
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        // [NoDirectAccess]
        [SessionTimeoutAttribute]
        public ActionResult LeaveSearch()
        {

            string lMessage = string.Empty;
            if (TempData["status"] != null)
            {
                lMessage = TempData["status"].ToString();
            }
            var items = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().OrderBy(x => x.Type).Select(x => new LeaveTypes
            {
                Id = x.Id,
                Type = x.Type.Trim(),
            });
            ViewBag.LeaveTypes = new SelectList(items, "Id", "Type");
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/MyLeaves/_EmpLeaveHistory.cshtml");
        }
        // GET: MyLeaves
        [NoDirectAccess]
        public ActionResult LeaveSearchView()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }

        public DateTime getFormatDate(DateTime dt)
        {

            DateTime theDate = dt;
            string myTime = theDate.ToString("MM/dd/yyyy");
            return Convert.ToDateTime(myTime);
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
        public string LeaveDeptBranchName(string branch, string department, int empid)
        {
            string lResult = string.Empty;
            DateTime lsysdate = GetCurrentTime(DateTime.Now).Date;
            var lleavedates = db.Leaves.Where(a => a.EmpId == empid).Select(a => a.UpdatedDate).ToList();
            var lTransferdates = db.Employee_Transfer.Where(a => a.EmpId == empid).Select(a => a.EffectiveFrom).ToList();
            DateTime? ltransferdates = db.Employee_Transfer.Where(a => a.EmpId == empid).Select(a => (DateTime?)a.EffectiveFrom).FirstOrDefault();
            var oldbranch = db.Employee_Transfer.Where(a => a.EffectiveFrom == ltransferdates).Where(a => a.EmpId == empid).Select(a => a.OldBranch).FirstOrDefault();
            var branchname = db.Branches.Where(a => a.Id == oldbranch).Select(a => a.Name).FirstOrDefault();
            var olddept = db.Employee_Transfer.Where(a => a.EffectiveFrom == ltransferdates).Where(a => a.EmpId == empid).Select(a => a.OldDepartment).FirstOrDefault();
            var deptname = db.Departments.Where(a => a.Id == olddept).Select(a => a.Name).FirstOrDefault();

            for (int i = 0; i < lTransferdates.Count; i++)
            {
                DateTime? leffectdate = lTransferdates[i];
                if (leffectdate > lsysdate)
                {
                    lResult = GetBranchDepartmentConcat(branchname.ToString(), deptname.ToString());
                }
                else
                {
                    lResult = GetBranchDepartmentConcat(branch, department);
                }
            }
            for (int j = 0; j < lleavedates.Count; j++)
            {
                DateTime leffectdate = lleavedates[j];
                lResult = GetBranchDepartmentConcat(branch, department);
            }
            // lResult = GetBranchDepartmentConcat(branch, department);
            return lResult;
        }

        [HttpGet]
        public JsonResult LeaveSearchViews(string StartDate)
        {
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
                var lstatus = db.Leaves.Where(a => a.EmpId == lEmpId).Where(a => a.Status == "PartialCancelled").Where(a => a.LeaveDays == 0).ToList();
                var lResults = (from leave in lleaves
                                join leavetype in lLeaveTypes on leave.LeaveType equals leavetype.Id
                                join emp in lemployees on leave.EmpId equals emp.Id
                                join branch in lBranches on leave.BranchId equals branch.Id
                                join desig in ldesignation on leave.DesignationId equals desig.Id
                                join dept in Departments on leave.DepartmentId equals dept.Id
                                select new
                                {
                                    leave.Id,
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                    AppliedTime = Convert.ToDateTime(leave.UpdatedDate).ToShortTimeString(),
                                    ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    leave.LeaveDays,
                                    leavetype.Code,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
                                    Action = Leavestatus(leave.Status),
                                }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                return Json(lResults.OrderByDescending(a => a.Id), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }

        public string Leavestatus(string status)

        {
            string lmessage = "";
            if (status == "Cancelled")
            {
                lmessage = "Cancelled";
            }
            else if (status == "PartialCancelled")
            {
                lmessage = "Cancelled";
            }
            else if (status == "Denied")
            {
                lmessage = "Cancelled";
            }
            else if (status == "Credit")
            {
                lmessage = "Cancelled";
            }
            else if (status == "Debit")
            {
                lmessage = "Cancelled";
            }
            else
            {
                lmessage = "Cancel";
            }

            return lmessage;
        }

        [HttpGet]
        public ActionResult CancelLeave(string LeaveId, string LeaveStatus)
        {
            var lEmpBalance = db.EmpLeaveBalance.ToList();
           
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int id = 0;
            if (LeaveId != "")
                id = Convert.ToInt32(LeaveId);
            if (LeaveStatus == "Debit" || LeaveStatus == "Credit")
            {
                leaves_CreditDebit lLeaveCancel1 = Facade.EntitiesFacade.GetLeaveDebitorcreditTabledata.GetById(id);
                int lvalue = lLeaveCancel1.LeaveTypeId;
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                string ltype = db.LeaveTypes.Where(a => a.Id == lvalue).Select(a => a.Code).FirstOrDefault();
                Session["CancelId"] = id;
                //Session["lcontrols"] = lLeaveCancel1.ControllingAuthority;
                //Session["lSancation"] = lLeaveCancel1.SanctioningAuthority;
                return RedirectToAction("cancelcreditdebitconformation", "MyLeaves");

            }
            else
            {
                Leaves lLeaveCancel = Facade.EntitiesFacade.GetLeaveTabledata.GetById(id);
                int lvalue = lLeaveCancel.LeaveType;
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                string ltype = db.LeaveTypes.Where(a => a.Id == lvalue).Select(a => a.Code).FirstOrDefault();
                Session["CancelId"] = id;
                Session["lcontrols"] = lLeaveCancel.ControllingAuthority;
                Session["lSancation"] = lLeaveCancel.SanctioningAuthority;
                if (ltype == "LOP")
                {
                    return RedirectToAction("cancelconfirmation", "MyLeaves");
                }
                else
                {
                    return RedirectToAction("cancelconfirmation", "MyLeaves");
                }
            }



        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult LeaveSearchView(FormCollection form)
        {
            var YourRadioButton = Request.Form["Status"];
            Session["sd"] = form["SDate"];
            Session["ed"] = form["EDate"];
            Session["lstatus"] = form["Status"];
            Session["leaveTypeId"] = form["leaveid"];
            Session["lApplied"] = form["lApplied"];
            Session["lRequest"] = form["lRequest"];
            string lMessage = string.Empty;
            try
            {
                var lleaves = db.Leaves.ToList();
                var lleaveHistory = db.V_LeaveHistory.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                DateTime FromDate = Convert.ToDateTime(form["SDate"]);
                DateTime ToDate = Convert.ToDateTime(form["EDate"]);
                string lstatus = form["Status"];
                string leaveTypeId = form["leaveid"];
                string Applieddate = form["lApplied"];
                string Requestdate = form["lRequest"];
                DateTime lStartdate = Convert.ToDateTime(FromDate.ToString("yyyy-MM-dd 00:00:00.000"));
                DateTime lEnddate = Convert.ToDateTime(ToDate.ToString("yyyy-MM-dd 23:59:59.000"));
                var Departments = db.Departments.ToList();
                var lbranch = db.Branches.ToList();
                int leavetypeids = 0;
                int LtypeId = Convert.ToInt32(leaveTypeId);
                string lType = db.LeaveTypes.Where(a => a.Id == LtypeId).Select(a => a.Type).FirstOrDefault();
                if (Applieddate == "Applied")
                {
                    if (lType == "ALL" && lstatus == "ALL")
                    {
                        leavetypeids = Convert.ToInt32(leaveTypeId);

                        var lResults = (from leave in lleaves
                                        join emp in lemployees on leave.EmpId equals emp.Id
                                        join dept in Departments on leave.DepartmentId equals dept.Id
                                        join desig in ldesignation on leave.DesignationId equals desig.Id
                                        join branch in lbranch on leave.BranchId equals branch.Id
                                        join types in lLeaveTypes on leave.LeaveType equals types.Id
                                        where ((leave.UpdatedDate >= lStartdate && leave.UpdatedDate <= lEnddate)
                                        || (leave.UpdatedDate >= lStartdate && leave.UpdatedDate <= lEnddate))
                                        select new
                                        {
                                            leave.Id,
                                            EmployeeCode = emp.EmpId,
                                            emp.ShortName,
                                            designation = desig.Code,
                                            UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                            AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                            ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                            StartDate = leave.StartDate,
                                            EndDate = leave.EndDate,
                                            leave.LeaveDays,
                                            types.Code,
                                            leave.Subject,
                                            leave.Reason,
                                            leave.Status,
                                            Action = Leavestatus(leave.Status),
                                        }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResults, JsonRequestBehavior.AllowGet);
                    }
                    if (lType == "ALL")
                    {
                        leavetypeids = Convert.ToInt32(leaveTypeId);

                        var lResults = (from leave in lleaves
                                        join emp in lemployees on leave.EmpId equals emp.Id
                                        join dept in Departments on leave.DepartmentId equals dept.Id
                                        join desig in ldesignation on leave.DesignationId equals desig.Id
                                        join branch in lbranch on leave.BranchId equals branch.Id
                                        join types in lLeaveTypes on leave.LeaveType equals types.Id
                                        where ((leave.UpdatedDate >= lStartdate && leave.UpdatedDate <= lEnddate)
                                      || (leave.UpdatedDate >= lStartdate && leave.UpdatedDate <= lEnddate))
                                        where leave.Status == lstatus
                                        select new
                                        {
                                            leave.Id,
                                            EmployeeCode = emp.EmpId,
                                            emp.ShortName,
                                            designation = desig.Code,
                                            UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                            AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                            ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                            StartDate = leave.StartDate,
                                            EndDate = leave.EndDate,
                                            leave.LeaveDays,
                                            types.Code,
                                            leave.Subject,
                                            leave.Reason,
                                            leave.Status,
                                            Action = Leavestatus(leave.Status),
                                        }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResults, JsonRequestBehavior.AllowGet);
                    }
                    if (lType != "ALL" && lstatus == "ALL")
                    {
                        leavetypeids = Convert.ToInt32(leaveTypeId);

                        var lResults = (from leave in lleaves
                                        join emp in lemployees on leave.EmpId equals emp.Id
                                        join dept in Departments on leave.DepartmentId equals dept.Id
                                        join desig in ldesignation on leave.DesignationId equals desig.Id
                                        join branch in lbranch on leave.BranchId equals branch.Id
                                        join types in lLeaveTypes on leave.LeaveType equals types.Id
                                        where ((leave.UpdatedDate >= lStartdate && leave.UpdatedDate <= lEnddate)
                                       || (leave.UpdatedDate >= lStartdate && leave.UpdatedDate <= lEnddate))
                                        // where leave.Status == lstatus
                                        where leave.LeaveType == leavetypeids
                                        select new
                                        {
                                            leave.Id,
                                            EmployeeCode = emp.EmpId,
                                            emp.ShortName,
                                            designation = desig.Code,
                                            UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                            AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                            ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                            StartDate = leave.StartDate,
                                            EndDate = leave.EndDate,
                                            leave.LeaveDays,
                                            types.Code,
                                            leave.Subject,
                                            leave.Reason,
                                            leave.Status,
                                            Action = Leavestatus(leave.Status),
                                        }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResults, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        leavetypeids = Convert.ToInt32(leaveTypeId);

                        var lResults = (from leave in lleaves
                                        join types in lLeaveTypes on leave.LeaveType equals types.Id
                                        join emp in lemployees on leave.EmpId equals emp.Id
                                        join dept in Departments on leave.DepartmentId equals dept.Id
                                        join desig in ldesignation on leave.DesignationId equals desig.Id
                                        join branch in lbranch on leave.BranchId equals branch.Id
                                        where ((leave.UpdatedDate >= lStartdate && leave.UpdatedDate <= lEnddate)
                                            || (leave.UpdatedDate >= lStartdate && leave.UpdatedDate <= lEnddate))
                                        where leave.LeaveType == leavetypeids
                                        where leave.Status == lstatus
                                        select new
                                        {
                                            leave.Id,
                                            EmployeeCode = emp.EmpId,
                                            emp.ShortName,
                                            designation = desig.Code,
                                            UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                            AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                            ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                            StartDate = leave.StartDate,
                                            EndDate = leave.EndDate,
                                            leave.LeaveDays,
                                            types.Code,
                                            leave.Subject,
                                            leave.Reason,
                                            leave.Status,
                                            Action = Leavestatus(leave.Status),
                                        }).OrderByDescending(A => A.UpdatedDate).ThenByDescending(a => a.Id);
                        return Json(lResults, JsonRequestBehavior.AllowGet);
                    }



                }
                else if (Applieddate == "Request")
                {
                    if (lType == "ALL" && lstatus == "ALL")
                    {
                        leavetypeids = Convert.ToInt32(leaveTypeId);
                        var lResults = (from leave in lleaves
                                        join emp in lemployees on leave.EmpId equals emp.Id
                                        join dept in Departments on leave.DepartmentId equals dept.Id
                                        join desig in ldesignation on leave.DesignationId equals desig.Id
                                        join branch in lbranch on leave.BranchId equals branch.Id
                                        join types in lLeaveTypes on leave.LeaveType equals types.Id
                                        where ((leave.StartDate >= lStartdate && leave.EndDate <= lEnddate)
                                         || (leave.EndDate >= lStartdate && leave.StartDate <= lEnddate))
                                        //where leave.Status == lstatus



                                        select new
                                        {
                                            leave.Id,
                                            EmployeeCode = emp.EmpId,
                                            emp.ShortName,
                                            designation = desig.Code,
                                            UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                            AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                            ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                            StartDate = leave.StartDate,
                                            EndDate = leave.EndDate,
                                            leave.LeaveDays,
                                            types.Code,
                                            leave.Subject,
                                            leave.Reason,
                                            leave.Status,
                                            Action = Leavestatus(leave.Status),
                                        }).OrderByDescending(A => A.StartDate).ThenByDescending(a => a.Id);
                        return Json(lResults, JsonRequestBehavior.AllowGet);
                    }
                    if (lType == "ALL")
                    {
                        leavetypeids = Convert.ToInt32(leaveTypeId);
                        var lResults = (from leave in lleaves

                                        join emp in lemployees on leave.EmpId equals emp.Id
                                        join dept in Departments on leave.DepartmentId equals dept.Id
                                        join desig in ldesignation on leave.DesignationId equals desig.Id
                                        join branch in lbranch on leave.BranchId equals branch.Id
                                        join types in lLeaveTypes on leave.LeaveType equals types.Id
                                        where ((leave.StartDate >= lStartdate && leave.EndDate <= lEnddate)
                                          || (leave.EndDate >= lStartdate && leave.StartDate <= lEnddate))
                                        where leave.Status == lstatus
                                        select new
                                        {
                                            leave.Id,
                                            EmployeeCode = emp.EmpId,
                                            emp.ShortName,
                                            designation = desig.Code,
                                            UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                            AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                            ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                            StartDate = leave.StartDate,
                                            EndDate = leave.EndDate,
                                            leave.LeaveDays,
                                            types.Code,
                                            leave.Subject,
                                            leave.Reason,
                                            leave.Status,
                                            Action = Leavestatus(leave.Status),
                                        }).OrderByDescending(A => A.StartDate).ThenByDescending(a => a.Id);
                        return Json(lResults, JsonRequestBehavior.AllowGet);
                    }
                    if (lType != "ALL" && lstatus == "ALL")
                    {
                        leavetypeids = Convert.ToInt32(leaveTypeId);
                        var lResults = (from leave in lleaves
                                        join emp in lemployees on leave.EmpId equals emp.Id
                                        join dept in Departments on leave.DepartmentId equals dept.Id
                                        join desig in ldesignation on leave.DesignationId equals desig.Id
                                        join branch in lbranch on leave.BranchId equals branch.Id
                                        join types in lLeaveTypes on leave.LeaveType equals types.Id
                                        where ((leave.StartDate >= lStartdate && leave.EndDate <= lEnddate)
                                          || (leave.EndDate >= lStartdate && leave.StartDate <= lEnddate))
                                        where leave.LeaveType == leavetypeids
                                        // where leave.Status == lstatus
                                        select new
                                        {
                                            leave.Id,
                                            EmployeeCode = emp.EmpId,
                                            emp.ShortName,
                                            designation = desig.Code,
                                            UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                            AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                            ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                            StartDate = leave.StartDate,
                                            EndDate = leave.EndDate,
                                            leave.LeaveDays,
                                            types.Code,
                                            leave.Subject,
                                            leave.Reason,
                                            leave.Status,
                                            Action = Leavestatus(leave.Status),
                                        }).OrderByDescending(A => A.StartDate).ThenByDescending(a => a.Id);
                        return Json(lResults, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        leavetypeids = Convert.ToInt32(leaveTypeId);
                        var lResults = (from leave in lleaves

                                        join emp in lemployees on leave.EmpId equals emp.Id
                                        join dept in Departments on leave.DepartmentId equals dept.Id
                                        join desig in ldesignation on leave.DesignationId equals desig.Id
                                        join branch in lbranch on leave.BranchId equals branch.Id
                                        join types in lLeaveTypes on leave.LeaveType equals types.Id
                                        where ((leave.StartDate >= lStartdate && leave.EndDate <= lEnddate)
                                           || (leave.EndDate >= lStartdate && leave.StartDate <= lEnddate))
                                        where leave.LeaveType == leavetypeids
                                        where leave.Status == lstatus
                                        select new
                                        {
                                            leave.Id,
                                            EmployeeCode = emp.EmpId,
                                            emp.ShortName,
                                            designation = desig.Code,
                                            UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                            AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                            ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                            StartDate = leave.StartDate,
                                            EndDate = leave.EndDate,
                                            leave.LeaveDays,
                                            types.Code,
                                            leave.Subject,
                                            leave.Reason,
                                            leave.Status,
                                            Action = Leavestatus(leave.Status),
                                        }).OrderByDescending(A => A.StartDate).ThenByDescending(a => a.Id);
                        return Json(lResults, JsonRequestBehavior.AllowGet);
                    }

                }


            }

            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        [NoDirectAccess]
        [HttpGet]
        public ActionResult CreditDebitleaves()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items1 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name.ToString()
            }).Distinct();
            ViewBag.Branches = new SelectList(items1, "Id", "Name");

            var items2 = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();
            ViewBag.Departments = new SelectList(items2, "Id", "Name");

            var items3 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();
            ViewBag.Designations = new SelectList(items3, "Id", "Name");

            //var items4 = Facade.EntitiesFacade.GetAllLeaveTypes().Where(a => a.Type != "ALL").Where(a => a.Code != "EOL").Where(a => a.Code != "LOP").Where(a => a.Code != "PTL").Where(a => a.Code != "MTL").Select(x => new LeaveTypes

            var items4 = Facade.EntitiesFacade.GetAllLeaveTypes().Where(a => a.Type != "ALL").Where(a => a.Code != "EOL").Where(a => a.Code != "LOP").Select(x => new LeaveTypes
            {
                Id = x.Id,
                Type = x.Type
            }).Distinct();
            ViewBag.LeaveTypes1 = new SelectList(items4, "Id", "Type");


            var items5 = Facade.EntitiesFacade.GetAll().Select(x => new Employees
            {
                Id = x.Id,
                LastName = x.FirstName.ToString() + " " + x.LastName.ToString()
            }).Distinct();
            ViewBag.EmpNames = new SelectList(items5, "Id", "LastName");
            return View("~/Views/MyLeaves/CreditDebitleaves.cshtml");
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
        public JsonResult employcreditdebit(string bname, string designame, string deptname)
        {

            int stateid1 = Convert.ToInt32(deptname);
            int stateid = Convert.ToInt32(bname);
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            int stateid2 = Convert.ToInt32(designame);
            var query = (from b in db.Employes
                         where b.Branch.Equals(stateid) && b.Department.Equals(stateid1) && b.CurrentDesignation.Equals(stateid2)
                         where b.RetirementDate >= lStartDate
                         where b.Role != 4
                         select new
                         {
                             b.FirstName,
                             b.LastName,
                             b.Id
                         }).Distinct();


            List<Employees> lDesigination = db.Employes.Where(a => a.CurrentDesignation.ToString() == bname).ToList();

            var stateData = query.Select(m => new SelectListItem()
            {
                Text = m.FirstName.ToString() + " " + m.LastName.ToString(),

                Value = m.Id.ToString(),
            });
            return Json(stateData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HLoadByBranchId(string State)
        {
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            int stateid = Convert.ToInt32(State);
            var query = (from b in db.Employes
                         join m in db.Designations on b.CurrentDesignation equals m.Id
                         where b.RetirementDate >= lStartDate
                         where b.Department.Equals(stateid)

                         select new
                         {
                             m.Name,
                             m.Id
                         }).Distinct();



            List<Employees> lBranchs = db.Employes.Where(a => a.Department.ToString() == State).ToList();

            var stateData = query.Select(m => new SelectListItem()
            {
                Text = m.Name.ToString(),
                Value = m.Id.ToString(),
            });
            return Json(stateData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HLoadByDesignationId(string State, string dept, string branc)
        {
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            int stateid = 0;
            int stateid1 = 0;
            int stateid2 = 0;
            if (State != "")
            {
                stateid = Convert.ToInt32(State);
            }
            if (branc != "" && dept != "")
            {
                stateid2 = Convert.ToInt32(branc);
                stateid1 = Convert.ToInt32(dept);
                var query1 = (from b in db.Employes
                              where b.Branch == stateid2 && b.CurrentDesignation == stateid1
                              where b.RetirementDate >= lStartDate
                              select new
                              {
                                  b.FirstName,
                                  b.LastName,
                                  b.Id
                              }).Distinct();
                List<Employees> lDesigination = db.Employes.Where(a => a.CurrentDesignation.ToString() == State).ToList();

                var stateData = query1.Select(m => new SelectListItem()
                {
                    Text = m.FirstName.ToString() + " " + m.LastName.ToString(),

                    Value = m.Id.ToString(),
                });
                return Json(stateData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                stateid1 = Convert.ToInt32(dept);
                var query = (from b in db.Employes
                             where b.Department.Equals(stateid) && b.CurrentDesignation.Equals(stateid1)
                             where b.RetirementDate >= lStartDate
                             select new
                             {
                                 b.FirstName,
                                 b.LastName,
                                 b.Id
                             }).Distinct();

                List<Employees> lDesigination = db.Employes.Where(a => a.CurrentDesignation.ToString() == State).ToList();

                var stateData = query.Select(m => new SelectListItem()
                {
                    Text = m.FirstName.ToString() + " " + m.LastName.ToString(),

                    Value = m.Id.ToString(),
                });
                return Json(stateData, JsonRequestBehavior.AllowGet);
            }


        }

        public JsonResult HLoadByLeaveId(string State, string dept, string branc, string emp)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            int stateid = Convert.ToInt32(emp);
            int lEmpId = db.Employes.Where(a => a.Id == stateid).Select(a => a.Id).FirstOrDefault();
            int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
            string lcode = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
            if (lcode == "ATTD" || lcode == "DRVR" || lcode == "SA" || lcode == "Attender-Watchman" || lcode == "Attender/J.C" || lcode == "Watchman" || lcode == "JR-SA" || lcode == "SA-Assistant Cashier")
            {
                var items1 = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().OrderBy(a => a.Type).Where(a => a.Type != "ALL").Where(a => a.Code != "EOL").Where(a => a.Code != "LOP").Where(a => a.Code != "PTL").Where(a => a.Code != "MTL").Select(x => new LeaveTypes
                {
                    Id = x.Id,
                    Type = x.Type.Trim(),
                });
                var stateData = ViewBag.LeaveTypes1 = new SelectList(items1, "Id", "Type");
                return Json(stateData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var items1 = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().OrderBy(a => a.Type).Where(a => a.Code != "ALL").Where(a => a.Code != "C-OFF").Where(a => a.Code != "EOL").Where(a => a.Code != "LOP").Where(a => a.Code != "PTL").Where(a => a.Code != "MTL").Select(x => new LeaveTypes
                {
                    Id = x.Id,
                    Type = x.Type.Trim(),
                });
                var stateData = ViewBag.LeaveTypes1 = new SelectList(items1, "Id", "Type");

                return Json(stateData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BLoadByBranchId(string State)
        {

            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            int stateid = Convert.ToInt32(State);
            var query = (from b in db.Employes
                         join m in db.Designations on b.CurrentDesignation equals m.Id
                         where b.Branch == stateid
                         where b.RetirementDate >= lStartDate


                         select new
                         {
                             m.Name,
                             m.Id
                         }).Distinct();



            List<Employees> lBranchs = db.Employes.Where(a => a.Department.ToString() == State).ToList();

            var stateData = query.Select(m => new SelectListItem()
            {
                Text = m.Name.ToString(),
                Value = m.Id.ToString(),
            });
            return Json(stateData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BLoadByDesignationId(string State, string dept)
        {

            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            int stateid1 = Convert.ToInt32(dept);
            int stateid = Convert.ToInt32(State);
            var query = (from b in db.Employes
                         where b.Branch.Equals(stateid) && b.CurrentDesignation.Equals(stateid1)
                         where b.RetirementDate >= lStartDate
                         select new
                         {
                             b.FirstName,
                             b.LastName,
                             b.Id
                         }).Distinct();

            List<Employees> lDesigination = db.Employes.Where(a => a.CurrentDesignation.ToString() == State).ToList();

            var stateData = query.Select(m => new SelectListItem()
            {
                Text = m.FirstName.ToString() + " " + m.LastName.ToString(),

                Value = m.Id.ToString(),
            });
            return Json(stateData, JsonRequestBehavior.AllowGet);
        }

        //
        [HttpGet]
        public JsonResult empsearch(string empid)
        {
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            string lrdate = Convert.ToString("lStartDate");
            string status = "false";

            string lempid = empid;
            var employees = db.Employes.ToList();
            var lbranches = db.Branches.ToList();

            var lResult = from cust in employees
                          where cust.EmpId == lempid
                          select new
                          {
                              cust.EmpId,
                              EmployeeName = cust.ShortName 
                          };

            var lResult1 = (from userslist in employees

                            where userslist.RetirementDate >= lStartDate
                            select new
                            {
                                userslist.EmpId,
                                EmployeeName = userslist.ShortName,


                            });
            var lresponseArray = lResult.ToArray();
            string employeeId = "";
            string employeeName = "";
            if (lresponseArray.Length>=1)
            {
                employeeId = lresponseArray[0].EmpId;
                employeeName = lresponseArray[0].EmployeeName;
            }
            else
             {
                return null;
            }
            //string employeeId = lresponseArray[0].EmpId;
            //string employeeName = lresponseArray[0].EmployeeName;

            return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName }, JsonRequestBehavior.AllowGet);


        }

        ////
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreditDebit(leaves_CreditDebit lmodel)
        {
            lmodel.year = DateTime.Now.Year;
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var lEmpLeaveBalance = db.EmpLeaveBalance.Where(a => a.Year == lmodel.year).ToList();

            
            string empid = Convert.ToString(lmodel.EmpId);
            int lempid = db.Employes.Where(a => a.EmpId == empid).Select(a => a.Id).FirstOrDefault();

            int desig = db.Employes.Where(a => a.EmpId == empid).Select(a => a.CurrentDesignation).FirstOrDefault();
            int Department = db.Employes.Where(a => a.EmpId == empid).Select(a => a.Department).FirstOrDefault();
            int Branch = db.Employes.Where(a => a.EmpId == empid).Select(a => a.Branch).FirstOrDefault();


           // int lempid = Convert.ToInt32(lmodel.EmpId);
            int lLeaveId = Convert.ToInt32(lmodel.LeaveTypeId);
            int creditleavedays = Convert.ToInt32(lmodel.CreditLeave);
            int debitleavedays = Convert.ToInt32(lmodel.DebitLeave);
            //int branch = Convert.ToInt32(lmodel.Branch);
            //int department = Convert.ToInt32(lmodel.Department);
            //int designation = Convert.ToInt32(lmodel.CurrentDesignation);
            string lcomments = lmodel.Comments;
            var lLeaves = db.Leaves.ToList();

            int branch = Branch;
            int department = Department;
            int designation = desig;

            // var lcarryforward = db.Leaves_CarryForward.ToList();

            int lcarryForwardBal = db.Leaves_CarryForward.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId).Select(a => a.LeaveBalance).FirstOrDefault();

            var lTypes = db.LeaveTypes.ToList();
            string lFirstname = db.Employes.Where(a => a.Id == lempid).Select(a => a.FirstName).FirstOrDefault();
            string lLastname = db.Employes.Where(a => a.Id == lempid).Select(a => a.LastName).FirstOrDefault();
            string lshortname = db.Employes.Where(a => a.Id == lempid).Select(a => a.ShortName).FirstOrDefault();
            string Gender = db.Employes.Where(a => a.Id == lempid).Select(a => a.Gender).FirstOrDefault();
            int lCasualLeave = lEmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.LeaveBalance).FirstOrDefault();
            int lMedicalSickLeave = lEmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.LeaveBalance).FirstOrDefault();
            int lPrivilegeLeave = lEmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.LeaveBalance).FirstOrDefault();
            int lMaternityLeave = lEmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.LeaveBalance).FirstOrDefault();
            int lPaternityLeave = lEmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.LeaveBalance).FirstOrDefault();
            int lExtraOrdinaryLeave = lEmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.LeaveBalance).FirstOrDefault();
           // int lCompensatory = lEmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.LeaveBalance).FirstOrDefault();
            int lSpecialCasualLeave = lEmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.LeaveBalance).FirstOrDefault();
            int lCompOff = lEmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.LeaveBalance).FirstOrDefault();
            string lLeaveCode = lTypes.Where(a => a.Id == lLeaveId).Select(a => a.Code).FirstOrDefault();
            string lType = lTypes.Where(a => a.Id == lLeaveId).Select(a => a.Type).FirstOrDefault();
            DateTime updatedate = DateTime.Now.Date;
            string controlauthority = db.Employes.Where(a => a.Id == lempid).Select(a => a.ControllingAuthority).FirstOrDefault();
            string sanctionauthority = db.Employes.Where(a => a.Id == lempid).Select(a => a.SanctioningAuthority).FirstOrDefault();
            int id = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            DateTime JoinigDate = Convert.ToDateTime(db.Employes.Where(a => a.Id == id).Select(a => a.DOJ).FirstOrDefault());
            DateTime lOneyear = JoinigDate.AddMonths(+12);
            if (lmodel.type == "Credit")
            {
                if (lLeaveCode == "CL")
                {
                    var lcreditdebit = new leaves_CreditDebit();
                    lcreditdebit.LeaveBalance = lCasualLeave;
                    lcreditdebit.TotalBalance = lCasualLeave + creditleavedays;
                    lcreditdebit.UpdatedBy = lCredentials.EmpId;
                    lcreditdebit.LeaveTypeId = lLeaveId;
                    lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                    //lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                    lcreditdebit.CurrentDesignation = designation;
                    lcreditdebit.type = lmodel.type;
                    lcreditdebit.EmpId = lempid;
                    lcreditdebit.year = lmodel.year;
                    lcreditdebit.CreditLeave = creditleavedays;
                    lcreditdebit.DebitLeave = debitleavedays;
                    lcreditdebit.Comments = lcomments;
                    lcreditdebit.EmpName = lshortname;
                    //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                      lcreditdebit.Head_Branch_Value = Branch;
                    if (lmodel.Head_Branch_Value == 42)
                    {
                        //lcreditdebit.Department = lmodel.Department;
                        lcreditdebit.Department = department;
                        string lBvalue = "OtherBranch";
                        int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Branch = lBranch;
                    }
                    else
                    {
                        // lcreditdebit.Branch = lmodel.Branch;
                        lcreditdebit.Branch = Branch;
                        string lDvalue = "OtherDepartment";
                        int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Department = ldept;
                    }
                    db.leaves_CreditDebit.Add(lcreditdebit);
                    db.SaveChanges();
                    int? lbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (lbalance == 0)
                    {
                        var NewEmpbalance = new EmpLeaveBalance();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lCasualLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        NewEmpbalance.Year = lmodel.year;
                        db.Entry(NewEmpbalance).State = EntityState.Added;
                        db.SaveChanges();

                        TempData["Status"] = "Casual Leave Credited successfully";
                    }
                    else
                    {
                        EmpLeaveBalance NewEmpbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lCasualLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        db.Entry(NewEmpbalance).State = EntityState.Modified;
                        db.SaveChanges();
                        Leaves_CarryForward Yearbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        if (Yearbalance == null)
                        {

                            TempData["Status"] = "Casual Leave Credited successfully";
                        }
                        else
                        {

                            TempData["Status"] = "Casual Leave Credited successfully";
                        }
                    }


                }

                else if (lLeaveCode == "CW-OFF")

                {
                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lCasualLeave;
                        lcreditdebit.TotalBalance = lCasualLeave + creditleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        //lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                        lcreditdebit.Head_Branch_Value = Branch;
                        if (lmodel.Head_Branch_Value == 42)
                        {
                            //lcreditdebit.Department = lmodel.Department;
                            lcreditdebit.Department = department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }
                        else
                        {
                            // lcreditdebit.Branch = lmodel.Branch;
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        int? lbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                        if (lbalance == 0)
                        {
                            var NewEmpbalance = new EmpLeaveBalance();
                            NewEmpbalance.EmpId = lempid;
                            NewEmpbalance.LeaveTypeId = lLeaveId;
                            int totalleavebalances = lCasualLeave + creditleavedays;
                            NewEmpbalance.LeaveBalance = totalleavebalances;
                            NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                            NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                            NewEmpbalance.Year = lmodel.year;
                            db.Entry(NewEmpbalance).State = EntityState.Added;
                            db.SaveChanges();

                            TempData["Status"] = "Compensatory Week Off  Credited successfully";
                        }
                        else
                        {
                            EmpLeaveBalance NewEmpbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                            NewEmpbalance.EmpId = lempid;
                            NewEmpbalance.LeaveTypeId = lLeaveId;
                            int totalleavebalances = lCasualLeave + creditleavedays;
                            NewEmpbalance.LeaveBalance = totalleavebalances;
                            NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                            NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                            db.Entry(NewEmpbalance).State = EntityState.Modified;
                            db.SaveChanges();
                            Leaves_CarryForward Yearbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                            if (Yearbalance == null)
                            {

                                TempData["Status"] = "Compensatory Week Off Leave Credited successfully";
                            }
                            else
                            {

                                TempData["Status"] = "Compensarory Week Off Leave Credited successfully";
                            }
                        }


                    }


                else if (lLeaveCode == "MTL")
                {
                    if (Gender == "Female")
                    {

                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lMaternityLeave;
                        lcreditdebit.TotalBalance = lMaternityLeave + creditleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                        lcreditdebit.Head_Branch_Value = Branch;

                        if (lmodel.Head_Branch_Value == 42)
                        {
                            lcreditdebit.Department = Department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }
                        else
                        {
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        int? lbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                        if (lbalance == 0)
                        {
                            var NewEmpbalance = new EmpLeaveBalance();
                            NewEmpbalance.EmpId = lempid;
                            NewEmpbalance.LeaveTypeId = lLeaveId;
                            int totalleavebalances = lMaternityLeave + creditleavedays;
                            NewEmpbalance.LeaveBalance = totalleavebalances;
                            NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                            NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                            NewEmpbalance.Year = lmodel.year;
                            db.Entry(NewEmpbalance).State = EntityState.Added;
                            db.SaveChanges();

                            TempData["Status"] = "Maternity Leave Credited successfully";
                        }
                        else
                        {
                            EmpLeaveBalance NewEmpbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                            NewEmpbalance.EmpId = lempid;
                            NewEmpbalance.LeaveTypeId = lLeaveId;
                            int totalleavebalances = lMaternityLeave + creditleavedays;
                            NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                            NewEmpbalance.LeaveBalance = totalleavebalances;
                            NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                            db.Entry(NewEmpbalance).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["Status"] = "Maternity Leave Credited successfully";
                        }


                    }
                    else
                    {
                        TempData["Status"] = "Cannot credit leave for Male employees";
                    }
                }

                else if (lLeaveCode == "PL")
                {
                    var lcreditdebit = new leaves_CreditDebit();
                    lcreditdebit.LeaveBalance = lPrivilegeLeave;
                    lcreditdebit.TotalBalance = lPrivilegeLeave + creditleavedays;
                    lcreditdebit.UpdatedBy = lCredentials.EmpId;
                    lcreditdebit.LeaveTypeId = lLeaveId;
                    lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                    // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                    lcreditdebit.CurrentDesignation = designation;
                    lcreditdebit.type = lmodel.type;
                    lcreditdebit.EmpId = lempid;
                    lcreditdebit.year = lmodel.year;
                    lcreditdebit.CreditLeave = creditleavedays;
                    lcreditdebit.DebitLeave = debitleavedays;
                    lcreditdebit.Comments = lcomments;
                    lcreditdebit.EmpName = lshortname;
                    //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                    lcreditdebit.Head_Branch_Value = Branch;
                                  
                    if (lmodel.Head_Branch_Value == 42)
                    {
                        lcreditdebit.Department = Department;
                        string lBvalue = "OtherBranch";
                        int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Branch = lBranch;
                    }
                    else
                    {
                        lcreditdebit.Branch = Branch;
                        string lDvalue = "OtherDepartment";
                        int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Department = ldept;
                    }
                    db.leaves_CreditDebit.Add(lcreditdebit);
                    db.SaveChanges();
                    int? lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (lbalance == 0)
                    {
                        var NewEmpbalance = new EmpLeaveBalance();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lPrivilegeLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        NewEmpbalance.Year = lmodel.year;
                        db.Entry(NewEmpbalance).State = EntityState.Added;
                        db.SaveChanges();

                        TempData["Status"] = "Privilege Leave Credited successfully";
                    }
                    else
                    {
                        EmpLeaveBalance NewEmpbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lPrivilegeLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        db.Entry(NewEmpbalance).State = EntityState.Modified;
                        db.SaveChanges();
                        Leaves_CarryForward Yearbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        if (Yearbalance == null)
                        {

                            TempData["Status"] = "Privilege Leave Credited successfully";
                        }
                        else
                        {
                            TempData["Status"] = "Privilege Leave Credited successfully";
                        }
                    }
                }

                else if (lLeaveCode == "C-OFF")
                {
                    var lcreditdebit = new leaves_CreditDebit();
                    lcreditdebit.LeaveBalance = lCompOff;
                    lcreditdebit.TotalBalance = lCompOff + creditleavedays;
                    lcreditdebit.UpdatedBy = lCredentials.EmpId;
                    lcreditdebit.LeaveTypeId = lLeaveId;
                    lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                    // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                    lcreditdebit.CurrentDesignation = designation;
                    lcreditdebit.type = lmodel.type;
                    lcreditdebit.EmpId = lempid;
                    lcreditdebit.year = lmodel.year;
                    lcreditdebit.CreditLeave = creditleavedays;
                    lcreditdebit.DebitLeave = debitleavedays;
                    lcreditdebit.Comments = lcomments;
                    lcreditdebit.EmpName = lshortname;
                    //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                    lcreditdebit.Head_Branch_Value = Branch;
                                       

                    if (lmodel.Head_Branch_Value == 42)
                    {
                        lcreditdebit.Department = Department;
                        string lBvalue = "OtherBranch";
                        int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Branch = lBranch;
                    }
                    else
                    {
                        lcreditdebit.Branch = Branch;
                        string lDvalue = "OtherDepartment";
                        int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Department = ldept;
                    }

                    var sumcreditleaves = db.leaves_CreditDebit.Where(a => a.EmpId == lempid).Where(a => a.LeaveTypeId == lLeaveId && a.year==lmodel.year).Sum(a => (int?)a.CreditLeave) ?? 0;

               

                    if (sumcreditleaves <= 1000)
                    {
                        int? lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                        int lbalance2 = sumcreditleaves + creditleavedays;
                        if (lbalance2 <= 1000)
                        {
                            db.leaves_CreditDebit.Add(lcreditdebit);
                            db.SaveChanges();

                            int lbalance15 = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.Id).FirstOrDefault();
                            //var NewEmpbalance = new EmpLeaveBalance();
                            EmpLeaveBalance NewEmpbalance = db.EmpLeaveBalance.Find(lbalance15);
                            NewEmpbalance.EmpId = lempid;
                            NewEmpbalance.LeaveTypeId = lLeaveId;
                            int totalleavebalances = lCompOff + creditleavedays;
                            NewEmpbalance.LeaveBalance = totalleavebalances;
                            NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                            NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                            // NewEmpbalance.Id = lbalance15;
                            // db.EmpLeaveBalance.Add(NewEmpbalance);
                            db.Entry(NewEmpbalance).State = EntityState.Modified;
                            db.SaveChanges();

                            //Leaves_CarryForward Yearbalance = new Leaves_CarryForward();
                            int lbalance16 = db.Leaves_CarryForward.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Select(a => a.Id).FirstOrDefault();

                            TempData["Status"] = "CompOffLeave  Credited successfully";
                        }
                        else
                        {
                            TempData["Status"] = "Cannot Credit More Than 10 leaves for One year";
                        }

                    }
                    else
                    {
                        TempData["Status"] = "Cannot Credit More Than 10 leaves for One year";
                    }

                }

                else if (lLeaveCode == "PTL")
                {
                    if (Gender == "Male")
                    {
                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lPaternityLeave;
                        lcreditdebit.TotalBalance = lPaternityLeave + creditleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                        lcreditdebit.Head_Branch_Value = Branch;


                        if (lmodel.Head_Branch_Value == 42)
                        {
                            lcreditdebit.Department = Department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }
                        else
                        {
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        int? lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                        if (lbalance == 0)
                        {
                            var NewEmpbalance = new EmpLeaveBalance();
                            NewEmpbalance.EmpId = lempid;
                            NewEmpbalance.LeaveTypeId = lLeaveId;
                            int totalleavebalances = lPaternityLeave + creditleavedays;
                            NewEmpbalance.LeaveBalance = totalleavebalances;
                            NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                            NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                            db.Entry(NewEmpbalance).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["Status"] = "Paternity Leave Credited successfully";
                        }
                        else
                        {
                            EmpLeaveBalance NewEmpbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                            NewEmpbalance.EmpId = lempid;
                            NewEmpbalance.LeaveTypeId = lLeaveId;
                            int totalleavebalances = lPaternityLeave + creditleavedays;
                            NewEmpbalance.LeaveBalance = totalleavebalances;
                            NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                            NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                            db.Entry(NewEmpbalance).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["Status"] = "Paternity Leave Credited successfully";
                        }
                    }
                    else
                    {
                        TempData["Status"] = "Cannot credit leave for Female employees";
                    }
                }
                               
                else if (lLeaveCode == "EOL")
                {
                    var lcreditdebit = new leaves_CreditDebit();
                    lcreditdebit.LeaveBalance = lExtraOrdinaryLeave;
                    lcreditdebit.TotalBalance = lExtraOrdinaryLeave + creditleavedays;
                    lcreditdebit.UpdatedBy = lCredentials.EmpId;
                    lcreditdebit.LeaveTypeId = lLeaveId;
                    lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                    // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                    lcreditdebit.CurrentDesignation = designation;
                    lcreditdebit.type = lmodel.type;
                    lcreditdebit.EmpId = lempid;
                    lcreditdebit.year = lmodel.year;
                    lcreditdebit.CreditLeave = creditleavedays;
                    lcreditdebit.DebitLeave = debitleavedays;
                    lcreditdebit.Comments = lcomments;
                    lcreditdebit.EmpName = lshortname;
                    //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                    lcreditdebit.Head_Branch_Value = Branch;
                                    
                    if (lmodel.Head_Branch_Value == 42)
                    {
                        lcreditdebit.Department = Department;
                        string lBvalue = "OtherBranch";
                        int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Branch = lBranch;
                    }
                    else
                    {
                        lcreditdebit.Branch = Branch;
                        string lDvalue = "OtherDepartment";
                        int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Department = ldept;
                    }
                    db.leaves_CreditDebit.Add(lcreditdebit);
                    db.SaveChanges();
                    int? lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (lbalance == 0)
                    {
                        var NewEmpbalance = new EmpLeaveBalance();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lExtraOrdinaryLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        NewEmpbalance.Year = lmodel.year;
                        db.Entry(NewEmpbalance).State = EntityState.Added;
                        db.SaveChanges();

                        TempData["Status"] = "ExtraOrdinary Leave Credited successfully";
                    }
                    else
                    {
                        EmpLeaveBalance NewEmpbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lExtraOrdinaryLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        db.Entry(NewEmpbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Status"] = "ExtraOrdinary Leave Credited successfully";
                    }
                }
                else if (lLeaveCode == "SCL")
                {
                    var lcreditdebit = new leaves_CreditDebit();
                    lcreditdebit.LeaveBalance = lSpecialCasualLeave;
                    lcreditdebit.TotalBalance = lSpecialCasualLeave + creditleavedays;
                    lcreditdebit.UpdatedBy = lCredentials.EmpId;
                    lcreditdebit.LeaveTypeId = lLeaveId;
                    lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                    // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                    lcreditdebit.CurrentDesignation = designation;
                    lcreditdebit.type = lmodel.type;
                    lcreditdebit.EmpId = lempid;
                    lcreditdebit.year = lmodel.year;
                    lcreditdebit.CreditLeave = creditleavedays;
                    lcreditdebit.DebitLeave = debitleavedays;
                    lcreditdebit.Comments = lcomments;
                    lcreditdebit.EmpName = lshortname;
                    //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                    lcreditdebit.Head_Branch_Value = Branch;

                    if (lmodel.Head_Branch_Value == 42)
                    {
                        lcreditdebit.Department = Department;
                        string lBvalue = "OtherBranch";
                        int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Branch = lBranch;
                    }
                    else
                    {
                        lcreditdebit.Branch = Branch;
                        string lDvalue = "OtherDepartment";
                        int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Department = ldept;
                    }
                    db.leaves_CreditDebit.Add(lcreditdebit);
                    db.SaveChanges();
                    int? lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (lbalance == 0)
                    {
                        var NewEmpbalance = new EmpLeaveBalance();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lSpecialCasualLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        NewEmpbalance.Year = lmodel.year;
                        db.Entry(NewEmpbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Status"] = "SpecialCasual Leave Credited successfully";
                    }
                    else
                    {
                        EmpLeaveBalance NewEmpbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lSpecialCasualLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        db.Entry(NewEmpbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Status"] = "SpecialCasual Leave Credited successfully";
                    }
                }
                else if (lLeaveCode == "ML")
                {
                    var lcreditdebit = new leaves_CreditDebit();
                    lcreditdebit.LeaveBalance = lMedicalSickLeave;
                    lcreditdebit.TotalBalance = lMedicalSickLeave + creditleavedays;
                    lcreditdebit.UpdatedBy = lCredentials.EmpId;
                    lcreditdebit.LeaveTypeId = lLeaveId;
                    lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                    // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                    lcreditdebit.CurrentDesignation = designation;
                    lcreditdebit.type = lmodel.type;
                    lcreditdebit.EmpId = lempid;
                    lcreditdebit.year = lmodel.year;
                    lcreditdebit.CreditLeave = creditleavedays;
                    lcreditdebit.DebitLeave = debitleavedays;
                    lcreditdebit.Comments = lcomments;
                    lcreditdebit.EmpName = lshortname;
                    //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                    lcreditdebit.Head_Branch_Value = Branch;
                                     
                    if (lmodel.Head_Branch_Value == 42)
                    {
                        lcreditdebit.Department = Department;
                        string lBvalue = "OtherBranch";
                        int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Branch = lBranch;
                    }
                    else
                    {
                        lcreditdebit.Branch = Branch;
                        string lDvalue = "OtherDepartment";
                        int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Department = ldept;
                    }
                    db.leaves_CreditDebit.Add(lcreditdebit);
                    db.SaveChanges();
                    int? lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year && a.Year == lmodel.year).Count();
                    if (lbalance == 0)
                    {
                        var NewEmpbalance = new EmpLeaveBalance();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lMedicalSickLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        NewEmpbalance.Year = lmodel.year;
                        db.Entry(NewEmpbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Status"] = "MedicalSick Leave Credited successfully";
                    }
                    else
                    {
                        EmpLeaveBalance NewEmpbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lMedicalSickLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        db.Entry(NewEmpbalance).State = EntityState.Modified;
                        db.SaveChanges();
                        Leaves_CarryForward Yearbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        if (Yearbalance == null)
                        {
                            TempData["Status"] = "MedicalSick Leave Credited successfully";
                        }
                        else
                        {
                            TempData["Status"] = "MedicalSick Leave Credited successfully";
                        }

                    }
                }
                // Special Medical Leave (Code: SML) – treat similar to Medical Sick Leave

                else if (lLeaveCode == "SML")
                {
                    var lcreditdebit = new leaves_CreditDebit();
                    // Use the same underlying balance bucket as Medical/Sick leave
                    lcreditdebit.LeaveBalance = lMedicalSickLeave;
                    lcreditdebit.TotalBalance = lMedicalSickLeave + creditleavedays;
                    lcreditdebit.UpdatedBy = lCredentials.EmpId;
                    lcreditdebit.LeaveTypeId = lLeaveId;
                    lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                    lcreditdebit.CurrentDesignation = designation;
                    lcreditdebit.type = lmodel.type;
                    lcreditdebit.EmpId = lempid;
                    lcreditdebit.year = lmodel.year;
                    lcreditdebit.CreditLeave = creditleavedays;
                    lcreditdebit.DebitLeave = debitleavedays;
                    lcreditdebit.Comments = lcomments;
                    lcreditdebit.EmpName = lshortname;
                    lcreditdebit.Head_Branch_Value = Branch;
                    if (lmodel.Head_Branch_Value == 42)
                    {
                        lcreditdebit.Department = Department;
                        string lBvalue = "OtherBranch";
                        int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Branch = lBranch;
                    }
                    else
                    {
                        lcreditdebit.Branch = Branch;
                        string lDvalue = "OtherDepartment";
                        int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                        lcreditdebit.Department = ldept;
                    }
                    db.leaves_CreditDebit.Add(lcreditdebit);
                    db.SaveChanges();
                    int? smlBalanceCount = lEmpLeaveBalance
                        .Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year)
                        .Count();
                    if (smlBalanceCount == 0)
                    {
                        var NewEmpbalance = new EmpLeaveBalance();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lMedicalSickLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        NewEmpbalance.Year = lmodel.year;
                        db.Entry(NewEmpbalance).State = EntityState.Added;
                        db.SaveChanges();
                        TempData["Status"] = "Special Medical Leave Credited successfully";
                    }

                    else
                    {
                        EmpLeaveBalance NewEmpbalance = lEmpLeaveBalance
                            .Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year)
                            .FirstOrDefault();
                        NewEmpbalance.EmpId = lempid;
                        NewEmpbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lMedicalSickLeave + creditleavedays;
                        NewEmpbalance.LeaveBalance = totalleavebalances;
                        NewEmpbalance.Credits = NewEmpbalance.Credits + creditleavedays;
                        NewEmpbalance.UpdatedBy = lCredentials.EmpId;
                        db.Entry(NewEmpbalance).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Status"] = "Special Medical Leave Credited successfully";
                    }

                }

            }
            if (lmodel.type == "Debit")
            {
                if (lLeaveCode == "CL")
                {
                    int totalLeaves = lCasualLeave - debitleavedays;
                    int lCount = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (totalLeaves < 0)
                    {
                        string text = "Only";
                        string text1 = "Casual Leaves are there to Debit";
                        TempData["Status"] = text + " " + lCasualLeave + " " + text1;
                    }
                    else if (lCount != 0)
                    {
                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lCasualLeave;
                        lcreditdebit.TotalBalance = lCasualLeave - debitleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                        lcreditdebit.Head_Branch_Value = Branch;
                                       
                        if (lmodel.Head_Branch_Value == 42)
                        {
                            lcreditdebit.Department = Department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }
                        else
                        {
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        EmpLeaveBalance lbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        lbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lCasualLeave - debitleavedays;
                        lbalance.Debits = lbalance.Debits + debitleavedays;
                        lbalance.LeaveBalance = totalleavebalances;
                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Status"] = "Casual Leave Debited successfully";

                    }
                }
                if (lLeaveCode == "CW-OFF")
                {
                    int totalLeaves = lCasualLeave - debitleavedays;
                    int lCount = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (totalLeaves < 0)
                    {
                        string text = "Only";
                        string text1 = "Compensatory Week Off Leaves are there to Debit";
                        TempData["Status"] = text + " " + lCasualLeave + " " + text1;
                    }
                    else if (lCount != 0)
                    {
                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lCasualLeave;
                        lcreditdebit.TotalBalance = lCasualLeave - debitleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                        lcreditdebit.Head_Branch_Value = Branch;

                        if (lmodel.Head_Branch_Value == 42)
                        {
                            lcreditdebit.Department = Department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }
                        else
                        {
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        EmpLeaveBalance lbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        lbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lCasualLeave - debitleavedays;
                        lbalance.Debits = lbalance.Debits + debitleavedays;
                        lbalance.LeaveBalance = totalleavebalances;
                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Status"] = "Compensatory Week Off  Leave Debited successfully";

                    }
                }
                else if (lLeaveCode == "MTL")
                {
                    if (Gender == "Female")
                    {

                        int totalLeaves = lMaternityLeave - debitleavedays;
                        int lCount = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                        if (totalLeaves < 0)
                        {
                            string text = "Only";
                            string text1 = " Maternity Leaves are there to Debit";
                            TempData["Status"] = text + " " + lMaternityLeave + " " + text1;

                        }
                        else if (lCount != 0)
                        {
                            var lcreditdebit = new leaves_CreditDebit();
                            lcreditdebit.LeaveBalance = lMaternityLeave;
                            lcreditdebit.TotalBalance = lMaternityLeave - debitleavedays;
                            lcreditdebit.UpdatedBy = lCredentials.EmpId;
                            lcreditdebit.LeaveTypeId = lLeaveId;
                            lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                            // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                            lcreditdebit.CurrentDesignation = designation;
                            lcreditdebit.type = lmodel.type;
                            lcreditdebit.EmpId = lempid;
                            lcreditdebit.year = lmodel.year;
                            lcreditdebit.CreditLeave = creditleavedays;
                            lcreditdebit.DebitLeave = debitleavedays;
                            lcreditdebit.Comments = lcomments;
                            lcreditdebit.EmpName = lshortname;
                            //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                            lcreditdebit.Head_Branch_Value = Branch;
                                               
                            if (lmodel.Head_Branch_Value == 42)
                            {
                                lcreditdebit.Department = Department;
                                string lBvalue = "OtherBranch";
                                int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                                lcreditdebit.Branch = lBranch;
                            }
                            else
                            {
                                lcreditdebit.Branch = Branch;
                                string lDvalue = "OtherDepartment";
                                int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                                lcreditdebit.Department = ldept;
                            }
                            db.leaves_CreditDebit.Add(lcreditdebit);
                            db.SaveChanges();
                            EmpLeaveBalance lbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                            lbalance.LeaveTypeId = lLeaveId;
                            int totalleavebalances = lMaternityLeave - debitleavedays;
                            lbalance.Debits = lbalance.Debits + debitleavedays;
                            lbalance.LeaveBalance = totalleavebalances;
                            db.Entry(lbalance).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["Status"] = "Maternity Leave Debited successfully";

                        }
                    }
                    else
                    {
                        TempData["Status"] = "Cannot credit leave for Male employees";
                    }

                }

                if (lLeaveCode == "PL")
                {
                    int totalLeaves = lPrivilegeLeave - debitleavedays;
                    int lCount = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (totalLeaves < 0)
                    {
                        string text = "Only";
                        string text1 = " Privilege Leaves are there to Debit";
                        TempData["Status"] = text + " " + lPrivilegeLeave + " " + text1;

                    }
                    else if (lCount != 0)
                    {
                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lPrivilegeLeave;
                        lcreditdebit.TotalBalance = lPrivilegeLeave - debitleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                        lcreditdebit.Head_Branch_Value = Branch;

                        if (lmodel.Head_Branch_Value == 42)
                        {
                            lcreditdebit.Department = Department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }
                        else
                        {
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        EmpLeaveBalance lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        lbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lPrivilegeLeave - debitleavedays;
                        lbalance.Debits = lbalance.Debits + debitleavedays;
                        lbalance.LeaveBalance = totalleavebalances;
                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Status"] = "Privilege Leave Debited successfully";

                    }
                }

                else if (lLeaveCode == "PTL")
                {
                    if (Gender == "Male")
                    {
                        int totalLeaves = lPaternityLeave - debitleavedays;
                        int lCount = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                        if (totalLeaves < 0)
                        {
                            TempData["Status"] = "No more Paternity Leaves are there to Debit";
                        }
                        else if (lCount != 0)
                        {
                            var lcreditdebit = new leaves_CreditDebit();
                            lcreditdebit.LeaveBalance = lPaternityLeave;
                            lcreditdebit.TotalBalance = lPaternityLeave - debitleavedays;
                            lcreditdebit.UpdatedBy = lCredentials.EmpId;
                            lcreditdebit.LeaveTypeId = lLeaveId;
                            lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                            // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                            lcreditdebit.CurrentDesignation = designation;
                            lcreditdebit.type = lmodel.type;
                            lcreditdebit.EmpId = lempid;
                            lcreditdebit.year = lmodel.year;
                            lcreditdebit.CreditLeave = creditleavedays;
                            lcreditdebit.DebitLeave = debitleavedays;
                            lcreditdebit.Comments = lcomments;
                            lcreditdebit.EmpName = lshortname;
                            //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                            lcreditdebit.Head_Branch_Value = Branch;
                                                                                 
                            if (lmodel.Head_Branch_Value == 42)
                            {
                                lcreditdebit.Department = Department;
                                string lBvalue = "OtherBranch";
                                int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                                lcreditdebit.Branch = lBranch;
                            }
                            else
                            {
                                lcreditdebit.Branch = Branch;
                                string lDvalue = "OtherDepartment";
                                int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                                lcreditdebit.Department = ldept;
                            }
                            db.leaves_CreditDebit.Add(lcreditdebit);
                            db.SaveChanges();
                            EmpLeaveBalance lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                            lbalance.LeaveTypeId = lLeaveId;
                            int totalleavebalances = lPaternityLeave - debitleavedays;
                            lbalance.Debits = lbalance.Debits + debitleavedays;
                            lbalance.LeaveBalance = totalleavebalances;
                            db.Entry(lbalance).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["Status"] = "Paternity Leave Debited successfully";

                        }

                    }
                    else
                    {
                        TempData["Status"] = "Cannot Debit leave for Female employees";
                    }
                }
                else if (lLeaveCode == "EOL")
                {
                    int totalLeaves = lExtraOrdinaryLeave - debitleavedays;
                    int lCount = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (totalLeaves < 0)
                    {
                        TempData["Status"] = "No more ExtraOrdinary Leaves are there to Debit";
                    }
                    else if (lCount != 0)
                    {
                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lExtraOrdinaryLeave;
                        lcreditdebit.TotalBalance = lExtraOrdinaryLeave - debitleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                        lcreditdebit.Head_Branch_Value = Branch;

                        if (lmodel.Head_Branch_Value == 42)
                        {
                            lcreditdebit.Department = Department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }
                        else
                        {
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        EmpLeaveBalance lbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        lbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lExtraOrdinaryLeave - debitleavedays;
                        lbalance.Debits = lbalance.Debits + debitleavedays;
                        lbalance.LeaveBalance = totalleavebalances;
                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Status"] = "ExtraOrdinary Leave Debited successfully";

                    }
                }
                else if (lLeaveCode == "SCL")
                {
                    int totalLeaves = lSpecialCasualLeave - debitleavedays;
                    int lCount = lEmpLeaveBalance.Where(a => a.EmpId == lempid).Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (totalLeaves < 0)
                    {
                        string text = "Only";
                        string text1 = " SpecialCasual Leaves are there to Debit";
                        TempData["Status"] = text + " " + lSpecialCasualLeave + " " + text1;

                    }
                    else if (lCount != 0)
                    {
                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lSpecialCasualLeave;
                        lcreditdebit.TotalBalance = lSpecialCasualLeave - debitleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                        lcreditdebit.Head_Branch_Value = Branch;
                        
                        if (lmodel.Head_Branch_Value == 42)
                        {
                            lcreditdebit.Department = Department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }
                        else
                        {
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        EmpLeaveBalance lbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        lbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lSpecialCasualLeave - debitleavedays;
                        lbalance.LeaveBalance = totalleavebalances;
                        lbalance.Debits = lbalance.Debits + debitleavedays;
                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Status"] = "SpecialCasual Leave Debited successfully";

                    }

                }
                else if (lLeaveCode == "ML")
                {
                    int totalLeaves = lMedicalSickLeave - debitleavedays;
                    int lCount = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (totalLeaves < 0)
                    {
                        string text = "Only";
                        string text1 = " MedicalSick Leaves are there to Debit";
                        TempData["Status"] = text + " " + lMedicalSickLeave + " " + text1;

                    }
                    else if (lCount != 0)
                    {
                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lMedicalSickLeave;
                        lcreditdebit.TotalBalance = lMedicalSickLeave - debitleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                        lcreditdebit.Head_Branch_Value = Branch;
                                                                 
                        if (lmodel.Head_Branch_Value == 42)
                        {
                            lcreditdebit.Department = Department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }
                        else
                        {
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        EmpLeaveBalance lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        lbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lMedicalSickLeave - debitleavedays;
                        lbalance.LeaveBalance = totalleavebalances;
                        lbalance.Debits = lbalance.Debits + debitleavedays;
                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();


                    }
                }
                else if (lLeaveCode == "C-OFF")
                {
                    int totalLeaves = lCompOff - debitleavedays;
                    int lCount = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();
                    if (totalLeaves < 0)
                    {
                        string text = "Only";
                        string text1 = " CompOff Leaves are there to Debit";
                        TempData["Status"] = text + " " + lCompOff + " " + text1;

                    }
                    else if (lCount != 0)
                    {
                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lCompOff;
                        lcreditdebit.TotalBalance = lCompOff - debitleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        // lcreditdebit.CurrentDesignation = lmodel.CurrentDesignation;
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        //lcreditdebit.Head_Branch_Value = lmodel.Head_Branch_Value;
                        lcreditdebit.Head_Branch_Value = Branch;
                                              
                        if (lmodel.Head_Branch_Value == 42)
                        {
                            lcreditdebit.Department = Department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }
                        else
                        {
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        EmpLeaveBalance lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).FirstOrDefault();
                        lbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lCompOff - debitleavedays;
                        lbalance.Debits = lbalance.Debits + debitleavedays;
                        lbalance.LeaveBalance = totalleavebalances;
                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Status"] = "CompOff Leave Debited successfully";

                    }
                }
                // Special Medical Leave (Code: SML) – treat similar to Medical Sick Leave for debits

                else if (lLeaveCode == "SML")
                {
                    int totalLeaves = lMedicalSickLeave - debitleavedays;
                    int lCount = lEmpLeaveBalance.Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year).Count();

                    if (totalLeaves < 0)
                    {
                        string text = "Only";
                        string text1 = " Special Medical Leaves are there to Debit";
                        TempData["Status"] = text + " " + lMedicalSickLeave + " " + text1;
                    }
                    else if (lCount != 0)
                    {
                        var lcreditdebit = new leaves_CreditDebit();
                        lcreditdebit.LeaveBalance = lMedicalSickLeave;
                        lcreditdebit.TotalBalance = lMedicalSickLeave - debitleavedays;
                        lcreditdebit.UpdatedBy = lCredentials.EmpId;
                        lcreditdebit.LeaveTypeId = lLeaveId;
                        lcreditdebit.UpdatedDate = GetCurrentTime(DateTime.Now);
                        lcreditdebit.CurrentDesignation = designation;
                        lcreditdebit.type = lmodel.type;
                        lcreditdebit.EmpId = lempid;
                        lcreditdebit.year = lmodel.year;
                        lcreditdebit.CreditLeave = creditleavedays;
                        lcreditdebit.DebitLeave = debitleavedays;
                        lcreditdebit.Comments = lcomments;
                        lcreditdebit.EmpName = lshortname;
                        lcreditdebit.Head_Branch_Value = Branch;
                        if (lmodel.Head_Branch_Value == 42)
                        {
                            lcreditdebit.Department = Department;
                            string lBvalue = "OtherBranch";
                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Branch = lBranch;
                        }

                        else
                        {
                            lcreditdebit.Branch = Branch;
                            string lDvalue = "OtherDepartment";
                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();
                            lcreditdebit.Department = ldept;
                        }
                        db.leaves_CreditDebit.Add(lcreditdebit);
                        db.SaveChanges();
                        EmpLeaveBalance lbalance = lEmpLeaveBalance
                            .Where(a => a.EmpId == lempid && a.LeaveTypeId == lLeaveId && a.Year == lmodel.year)
                            .FirstOrDefault();
                        lbalance.LeaveTypeId = lLeaveId;
                        int totalleavebalances = lMedicalSickLeave - debitleavedays;
                        lbalance.LeaveBalance = totalleavebalances;
                        lbalance.Debits = lbalance.Debits + debitleavedays;
                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Status"] = "Special Medical Leave Debited successfully";
                    }

                }

            }
            return RedirectToAction("CreditDebitleaves");
        }
                     
        [HttpGet]
        public JsonResult TodaysViews(string StartDate)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lMessage = string.Empty;
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;

            try
            {
                var lleaves = db.Leaves.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
                if (lCredentials.LoginMode == Constants.SuperAdmin || lCredentials.LoginMode == Constants.AdminHRDPayments || lCredentials.LoginMode == Constants.AdminHRDPolicy)
                {
                    var lResults = (from leave in lleaves
                                    join leavetype in lLeaveTypes on leave.LeaveType equals leavetype.Id
                                    join emp in lemployees on leave.EmpId equals emp.Id
                                    join branch in lBranches on emp.Branch equals branch.Id
                                    join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                    join dept in Departments on emp.Department equals dept.Id
                                    where leave.LeaveDays != 0 && leave.Status != "Cancelled" && leave.Status != "Denied" && leave.Status != "Debited"
                                    where (lStartDate >= leave.StartDate && lStartDate <= leave.EndDate)
                                     || (lEndDate >= leave.StartDate && lEndDate <= leave.EndDate)

                                    select new
                                    {
                                        leave.Id,
                                        emp.EmpId,
                                        emp.ShortName,
                                        designation = desig.Code,
                                        UpdatedDate = leave.UpdatedDate.ToShortDateString(),
                                        AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                        ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                        Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                        StartDate = leave.StartDate.ToShortDateString(),
                                        EndDate = leave.EndDate.ToShortDateString(),
                                        leave.LeaveDays,
                                        leavetype.Code,
                                        leave.Subject,
                                        leave.Reason,
                                        leave.Status,
                                        Action = Leavestatus(leave.Status),
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

        public string GetAppliedTime(DateTime lapplieddate)
        {
            string lApplied = "";
            DateTime d1 = lapplieddate;
            string lAppliedtime = d1.ToShortTimeString().ToString();
            lApplied = lAppliedtime;
            return lApplied;
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


        public ActionResult CreditDebitView()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CreditDebitViews(string EmpId)
        {
            Session["lEmpId"] = EmpId;
            try
            {
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var lleaves = db.leaves_CreditDebit.ToList();
                var lBranches = db.Branches.ToList();
                var lLeaveTypes = db.LeaveTypes.ToList();
                var Departments = db.Departments.ToList();
                var lemployees = db.Employes.ToList();
                var ldesignation = db.Designations.ToList();
             //   var idList = new int[1, 2, 2, 2, 2]; // same user is selected 4 times
               // var userProfiles = _dataContext.UserProfile.Where(e => idList.Contains(e)).ToList();
               //IEnumerable<string> empids = "296,459,231,237,333,304,288,4,6,15,3,5,17,1,21,9,12,28,10,14,18,19,2,20,16,13,8,11,22,7".Split(',');
                //IEnumerable<string> empids = "550".Split(',');

                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from leave in lleaves
                                join leavetype in lLeaveTypes on leave.LeaveTypeId equals leavetype.Id
                                join emp in lemployees on leave.EmpId equals emp.Id
                                join branch in lBranches on emp.Branch equals branch.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join dept in Departments on emp.Department equals dept.Id
                                orderby leave.UpdatedDate descending, leave.year descending
                                where emp.RetirementDate >= lStartDate
                                // where leave.EmpId == 550
                                //where empids.Contains(emp.Id.ToString())
                                //where leave.year == 2021
                                // where leave.LeaveTypeId == 1
                                select new
                                {
                                    leave.Id,
                                    emp.EmpId,
                                    leave.EmpName,
                                    emp.ShortName,
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    designation = desig.Code,
                                    leave.LeaveBalance,
                                    leave.DebitLeave,
                                    leave.CreditLeave,
                                    TotalLeaveBalance = TotalBalance(leave.CreditLeave, leave.DebitLeave, leave.LeaveBalance),
                                    leavetype.Type,
                                    leave.Comments,
                                    leave.UpdatedDate
                                }).Take(4500);//.OrderByDescending(A => A.UpdatedDate);
                   
                    return Json(data, JsonRequestBehavior.AllowGet);
                   
                }

                else
                {

                    var data = (from leave in lleaves
                                join leavetype in lLeaveTypes on leave.LeaveTypeId equals leavetype.Id
                                join emp in lemployees on leave.EmpId equals emp.Id
                                join branch in lBranches on emp.Branch equals branch.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join dept in Departments on emp.Department equals dept.Id
                                orderby leave.UpdatedDate descending, leave.year descending
                                where (emp.EmpId.ToLower().Contains(EmpId.ToLower()) || emp.ShortName.ToLower().Contains(EmpId.ToLower()) || branch.Name.ToString().ToLower().Contains(EmpId.ToLower()) || dept.Name.ToString().ToLower().Contains(EmpId.ToLower()) || desig.Name.ToString().ToLower().Contains(EmpId.ToLower()))
                                where emp.RetirementDate >= lStartDate
                                select new
                                {
                                    leave.Id,
                                    emp.EmpId,
                                    leave.EmpName,
                                    emp.ShortName,
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    designation = desig.Code,
                                    leave.LeaveBalance,
                                    leave.DebitLeave,
                                    leave.CreditLeave,
                                    TotalLeaveBalance = TotalBalance(leave.CreditLeave, leave.DebitLeave, leave.LeaveBalance),
                                    leavetype.Type,
                                    leave.Comments,
                                    leave.UpdatedDate
                                }).Take(4500);//.OrderByDescending(A => A.UpdatedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return null;
        }

        public int TotalBalance(int CreditLeave, int DebitLeave, int LeaveBalance)
        {

            int lbalance = 0;
            if (CreditLeave != 0)
            {
                lbalance = LeaveBalance + CreditLeave;
            }
            else if (DebitLeave != 0)
            {
                lbalance = LeaveBalance - DebitLeave;
            }
            return lbalance;
        }
        public DateTime[] GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates.ToArray();
        }
        public JsonResult Checkleaves(string StartDate, string EndDate, int EmpId)
        {
            string status = "";
            DateTime star1 = DateTime.Parse(StartDate);
            DateTime end1 = DateTime.Parse(EndDate);
            var lHolidays = db.HolidayList.ToList();
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentails.EmpId).Select(a => a.Id).FirstOrDefault();
            //  int leaveEmpId = db.Leaves.Where(a => a.EmpId == EmpId).Select(a => a.EmpId).FirstOrDefault();
            int emplevescount = db.Leaves.Where(a => a.EmpId == EmpId).Count();
            if (emplevescount > 0)
            {
                string str = "";
                List<Leaves> lStartEndCount = db.Leaves.Where(a => a.EmpId == EmpId).Where(a => a.Status == "Cancelled").ToList();
                foreach (Leaves l in lStartEndCount)
                {

                    DateTime star = l.StartDate;
                    DateTime end = l.EndDate;

                    DateTime[] dates = GetDatesBetween(star, end).ToArray();

                    for (int i = 0; i < dates.Length; i++)
                    {

                        DateTime d = dates[i];
                        if (star1 <= dates[i] && dates[i] <= end1)
                        {
                            //true condition already applied
                            str = str + dates[i].ToShortDateString() + ",";
                        }

                        if (str != "")
                        {
                            status = "false/" + str + "--Already leaves cancelled in these dates.";
                        }



                    }
                }
            }
            else
            {
                status = "countzero";
            }
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult cancelcreditdebitconformation()
        {
            ViewBag.ReportTitle = "Emp Leave History";

            string lMessage = string.Empty;
            int leaveid = Convert.ToInt32(Session["CancelId"].ToString());
           
            leaves_CreditDebit lLeaveCancel = Facade.EntitiesFacade.GetLeaveDebitorcreditTabledata.GetById(leaveid);
            var lmodel = new CancelConfirm();
            string empId1 = db.Employes.Where(a => a.Id == lLeaveCancel.EmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
            string empId2 = db.Employes.Where(a => a.Id == lLeaveCancel.EmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
            Employees lcontrolling = Facade.EntitiesFacade.GetEmpTabledata.GetById(Convert.ToInt32(empId1));
            Employees lsancationing = Facade.EntitiesFacade.GetEmpTabledata.GetById(Convert.ToInt32(empId2));
            string lControllingName = lcontrolling.FirstName + "" + lcontrolling.LastName;
            string lSancationingName = lsancationing.FirstName + "" + lsancationing.LastName;
            lmodel.ControllingAuthority = lControllingName;
            lmodel.SanctioningAuthority = lSancationingName;

            string empId = db.Employes.Where(a => a.Id == lLeaveCancel.EmpId).Select(a => a.EmpId).FirstOrDefault();
            var selected2 = (from sub1 in db.leaves_CreditDebit
                             where sub1.Id == leaveid
                             select sub1.EmpId).FirstOrDefault();
            string lempname = db.Employes.Where(a => a.Id == selected2).Select(a => a.ShortName).FirstOrDefault();
            string lempids = db.Employes.Where(a => a.Id == selected2).Select(a => a.EmpId).FirstOrDefault();
           
            var selected1 = (from sub1 in db.leaves_CreditDebit
                             where sub1.Id == leaveid
                             select sub1.LeaveTypeId).First();
            var items1 = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().OrderBy(a => a.Type).Where(a => a.Code != "ALL").Select(x => new LeaveTypes
            {
                Id = x.Id,
                Type = x.Type.Trim(),
            });
            ViewBag.LeaveTypes1 = new SelectList(items1, "Id", "Type", selected1);
             lmodel.LeaveType = lLeaveCancel.LeaveTypeId.ToString();
            if(lLeaveCancel.type=="Debit")
            { 
            lmodel.LeaveDays = lLeaveCancel.DebitLeave;
            }
            else
            {
                lmodel.LeaveDays = lLeaveCancel.CreditLeave;
            }
            lmodel.Reason = lLeaveCancel.Comments;
            lmodel.EmpName = lempname;
            lmodel.LeavesYear = DateTime.Today.Year;
            lmodel.EmpId = Convert.ToInt32(lempids);
            lmodel.Id = lLeaveCancel.Id;
            lmodel.LeaveType = Convert.ToString(lLeaveCancel.LeaveTypeId);

            if (TempData["AlertMessage"] != null)
            {
                lMessage = TempData["AlertMessage"].ToString();
            }
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var lholiday = db.HolidayList.ToList();
            //lmodel.lholidays = lholiday;
            return View(lmodel);
        }

        [HttpPost]
        public ActionResult cancelcreditdebitconformations(CancelConfirm leaves)
        {
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            int leaveid = Convert.ToInt32(Session["CancelId"].ToString());
         
            int lvalue = Convert.ToInt32(leaves.LeaveType);
            
            var lEmpBalance = db.EmpLeaveBalance.Where(a => a.Year == DateTime.Now.Year).ToList();
            leaves_CreditDebit lLeaveCancel = Facade.EntitiesFacade.GetLeaveDebitorcreditTabledata.GetById(leaveid);
            if (lLeaveCancel.type == "Debit")
            {
                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lLeaveCancel.EmpId).Where(a => a.LeaveTypeId == lvalue && a.Year == lLeaveCancel.year).FirstOrDefault();
                int totalLeaveBalance = lbalance.LeaveBalance;
                int CancelLeaveDays = lLeaveCancel.DebitLeave;
                int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                int Totaldays = totalLeaveBalance + CancelLeaveDays;
                int totalCDLeaveBalance = lLeaveCancel.TotalBalance;
                lbalance.LeaveBalance = Totaldays;
                lLeaveCancel.TotalBalance = totalCDLeaveBalance + CancelLeaveDays;
                //lLeaveCancel.type = "Cancelled";
                db.Entry(lbalance).State = EntityState.Modified;
                db.Entry(lLeaveCancel).State = EntityState.Modified;
                db.SaveChanges();

                if (leaves.canceltype == "Full")
                {
                    //leaves_CreditDebit lLeaveCancel1 = Facade.EntitiesFacade.GetLeaveDebitorcreditTabledata.GetById(leaveid);
                    lLeaveCancel.year = DateTime.Today.Year;
                    lLeaveCancel.type = "DebitCancelled";

                    //lLeaveCancel.type = lLeaveCancel.type;
                    //lLeaveCancel.Status = "Cancelled";
                    //string s = lLeaveCancel.Comments;
                    //string st = leaves.fullCancelReason;

                    //lLeaveCancel.Comments = s + "@CR-@" +st;
                    lLeaveCancel.Reason = leaves.fullCancelReason;
                    lLeaveCancel.UpdatedBy = lCredentails.EmpId;
                    lLeaveCancel.LCDTimeStamp = GetCurrentTime(DateTime.Now.Date);
                    //lLeaveCancel.CancelReason = leaves.fullCancelReason;
                    db.Entry(lLeaveCancel).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                //var lEmpBalance = db.EmpLeaveBalance.Where(a => a.Year == DateTime.Now.Year).ToList();
                //leaves_CreditDebit lLeaveCancel = Facade.EntitiesFacade.GetLeaveDebitorcreditTabledata.GetById(leaveid);
                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lLeaveCancel.EmpId).Where(a => a.LeaveTypeId == lvalue && a.Year == lLeaveCancel.year).FirstOrDefault();
                int totalLeaveBalance = lbalance.LeaveBalance;
                int CancelLeaveDays = lLeaveCancel.CreditLeave;
                int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                int Totaldays = totalLeaveBalance - CancelLeaveDays;
                int totalCDLeaveBalance = lLeaveCancel.TotalBalance;
                lbalance.LeaveBalance = Totaldays;
                lLeaveCancel.TotalBalance = totalCDLeaveBalance - CancelLeaveDays;
                db.Entry(lbalance).State = EntityState.Modified;
                db.SaveChanges();

                if (leaves.canceltype == "Full")
                {
                    lLeaveCancel.year = DateTime.Today.Year;
                    lLeaveCancel.type = "CreditCancelled";
                    lLeaveCancel.Reason = leaves.fullCancelReason;
                    lLeaveCancel.UpdatedBy = lCredentails.EmpId;
                    lLeaveCancel.LCDTimeStamp = GetCurrentTime(DateTime.Now.Date);
                    db.Entry(lLeaveCancel).State = EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["status"] = "Leave Cancelled Successfully";
            }
            TempData["status"] = "Leave Cancelled Successfully";
            return RedirectToAction("LeavesHistory", "AllReports");
        }

        public ActionResult cancelconfirmation()
        {

            ViewBag.ReportTitle = "Emp Leave History";

            string lMessage = string.Empty;
            int leaveid = Convert.ToInt32(Session["CancelId"].ToString());
            Leaves lLeaveCancel = Facade.EntitiesFacade.GetLeaveTabledata.GetById(leaveid);
            var lmodel = new CancelConfirm();

            Employees lcontrolling = Facade.EntitiesFacade.GetEmpTabledata.GetById(lLeaveCancel.ControllingAuthority);
            Employees lsancationing = Facade.EntitiesFacade.GetEmpTabledata.GetById(lLeaveCancel.SanctioningAuthority);

            string lControllingName = lcontrolling.FirstName + "" + lcontrolling.LastName;
            string lSancationingName = lsancationing.FirstName + "" + lsancationing.LastName;
            lmodel.ControllingAuthority = lControllingName;
            lmodel.SanctioningAuthority = lSancationingName;
            string empId = db.Employes.Where(a => a.Id == lLeaveCancel.EmpId).Select(a => a.EmpId).FirstOrDefault();
            var selected2 = (from sub1 in db.Leaves
                             where sub1.Id == leaveid
                             select sub1.EmpId).FirstOrDefault();
            string lempname = db.Employes.Where(a => a.Id == selected2).Select(a => a.ShortName).FirstOrDefault();
            string lempids = db.Employes.Where(a => a.Id == selected2).Select(a => a.EmpId).FirstOrDefault();
            string lleaveType = db.LeaveTypes.Where(a => a.Id == lLeaveCancel.LeaveType).Select(a => a.Type).FirstOrDefault();
            var selected1 = (from sub1 in db.Leaves
                             where sub1.Id == leaveid
                             select sub1.LeaveType).First();
            var items1 = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().OrderBy(a => a.Type).Where(a => a.Code != "ALL").Select(x => new LeaveTypes
            {
                Id = x.Id,
                Type = x.Type.Trim(),
            });
            ViewBag.LeaveTypes1 = new SelectList(items1, "Id", "Type", selected1);
            // lmodel.LeaveType = lleaveType;
            lmodel.LeaveDays = lLeaveCancel.LeaveDays;
            lmodel.Stage = lLeaveCancel.Status;
            lmodel.StartDate = lLeaveCancel.StartDate;
            lmodel.EndDate = lLeaveCancel.EndDate;
            lmodel.Subject = lLeaveCancel.Subject;
            lmodel.TotalDays = lLeaveCancel.TotalDays;
            lmodel.Reason = lLeaveCancel.Reason;
            lmodel.EmpName = lempname;
            lmodel.LeaveDays = lLeaveCancel.LeaveDays;
            lmodel.LeavesYear = DateTime.Today.Year;
            lmodel.LeaveTimeStamp = lLeaveCancel.LeaveTimeStamp;
            lmodel.EmpId = Convert.ToInt32(lempids);
            lmodel.Status = lLeaveCancel.Status;
            lmodel.DateofDelivery = lLeaveCancel.DateofDelivery;
            lmodel.Cancelstartdate = lLeaveCancel.StartDate;
            lmodel.Cancelenddate = lLeaveCancel.EndDate;
            lmodel.Id = lLeaveCancel.Id;
            lmodel.LeaveType = Convert.ToString(lLeaveCancel.LeaveType);

            if (TempData["AlertMessage"] != null)
            {
                lMessage = TempData["AlertMessage"].ToString();
            }
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var lholiday = db.HolidayList.ToList();
            //lmodel.lholidays = lholiday;
            return View(lmodel);
        }
        [HttpPost]
        public ActionResult cancelconfirmations(CancelConfirm leaves)
        {
            Timesheet_Request_Form ltform = new Timesheet_Request_Form();
            try
            {
                LogInformation.Info("AdminLeavecancellation Code started ");
                leaves.LeavesYear = DateTime.Now.Year;
                LoginCredential lCredentails = LoginHelper.GetCurrentUser();
                int leaveid = Convert.ToInt32(Session["CancelId"].ToString());
                int lEmpId = Convert.ToInt32(leaves.EmpId);
                var res = new Leaves();
                var lwkdiary = new WorkDiary();
                var lwordet = new WorkDiary_Det();
                int lvalue = Convert.ToInt32(leaves.LeaveType);
                var lleavetype = db.LeaveTypes.Where(a => a.Id == lvalue).Select(a => a.Code).FirstOrDefault();

                Leaves lLeaveCancel = Facade.EntitiesFacade.GetLeaveTabledata.GetById(leaveid);
                //  Leaves lLeaveCancel = db.Leaves.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveType == lvalue).Where(a => a.StartDate >= leaves.StartDate).Where(a => a.EndDate <= leaves.EndDate).FirstOrDefault();
                int? Leaveempid = db.Leaves.Where(a => a.Id == leaveid).Select(a => a.EmpId).FirstOrDefault();
                int? llastid = db.Leaves.Where(a => a.EmpId == Leaveempid).Where(a => a.StartDate <= leaves.EndDate).Where(a => a.EndDate >= leaves.StartDate).Where(a => a.Status == "PartialCancelled").Select(a => (int?)a.Id).Max();
                int lasttotaldays = db.Leaves.Where(a => a.EmpId == Leaveempid).Where(a => a.Id == llastid).Select(a => a.TotalDays).FirstOrDefault();
                int lastleavedays = db.Leaves.Where(a => a.EmpId == Leaveempid).Where(a => a.Id == llastid).Select(a => a.LeaveDays).FirstOrDefault();
                int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentails.EmpId).Select(a => a.Id).FirstOrDefault();
                int lempids = db.Employes.Where(a => a.EmpId == leaves.EmpId.ToString()).Select(a => a.Id).FirstOrDefault();
                var lEmpBalance = db.EmpLeaveBalance.Where(a => a.Year == DateTime.Now.Year).ToList();
                string mtype = db.Leaves.Where(a => a.EmpId == Leaveempid).Where(a => a.Id == llastid).Select(a => a.MaternityType).FirstOrDefault();
                string ltype = db.LeaveTypes.Where(a => a.Id == lvalue).Select(a => a.Code).FirstOrDefault();
                int? brid = lLeaveCancel.BranchId;
                int? deptid = lLeaveCancel.DepartmentId;
                int? desid = lLeaveCancel.DesignationId;



                if (leaves.canceltype == "Full")
                {
                    lLeaveCancel.LeavesYear = DateTime.Today.Year;
                    lLeaveCancel.Stage = leaves.Status;
                    lLeaveCancel.Status = "Cancelled";
                    lLeaveCancel.UpdatedBy = lCredentails.EmpId;
                    lLeaveCancel.LeaveTimeStamp = GetCurrentTime(DateTime.Now.Date);
                    //lLeaveCancel.UpdatedDate = GetCurrentTime(DateTime.Now.Date);
                    lLeaveCancel.LeaveTimeStamp = GetCurrentTime(DateTime.Now.Date);
                    lLeaveCancel.CancelReason = leaves.fullCancelReason;
                    db.Entry(lLeaveCancel).State = EntityState.Modified;
                    db.SaveChanges();

                    if (llastid == null && ltype == "LOP")
                    {
                        EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lLeaveCancel.EmpId).Where(a => a.LeaveTypeId == lvalue && a.Year == lLeaveCancel.LeavesYear).FirstOrDefault();
                        int totalLeaveBalance = lbalance.LeaveBalance;
                        int CancelLeaveDays = lLeaveCancel.LeaveDays;
                        int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                        int Totaldays = totalLeaveBalance - CancelLeaveDays;
                        lbalance.LeaveTypeId = Convert.ToInt32(leaves.LeaveType);
                        lbalance.EmpId = ids;
                        lbalance.LeaveBalance = Totaldays;
                        lbalance.Debits = lbalance.Debits - CancelLeaveDays;
                        lbalance.UpdatedBy = lCredentails.EmpId;
                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["status"] = "Leave Cancelled Successfully";
                        //timesheet request form insertion for approved cancellation
                        if (leaves.EndDate <= DateTime.Now.Date && leaves.Status=="Approved")
                        {
                            string lcode = db.LeaveTypes.Where(a => a.Id == lLeaveCancel.LeaveType).Select(a => a.Code).FirstOrDefault();
                            int branchid = db.Employes.Where(a => a.Id == ids).Select(a => a.Branch).FirstOrDefault();
                            int? shiftids = db.Employes.Where(a => a.Id == ids).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                            ltform.UserId = ids;
                            ltform.BranchId = (int)lLeaveCancel.BranchId;
                            ltform.DepartmentId = (int)lLeaveCancel.DepartmentId;
                            ltform.DesignationId = (int)lLeaveCancel.DesignationId;
                            ltform.Shift_Id = (int)shiftids;
                            ltform.Reason_Type = "AB";
                            ltform.Reason_Desc = "Leave";
                            ltform.ReqFromDate = leaves.Cancelstartdate;
                            ltform.ReqToDate = leaves.Cancelenddate;
                            ltform.CA = lLeaveCancel.ControllingAuthority;
                            ltform.SA = lLeaveCancel.SanctioningAuthority;
                            ltform.Status = lLeaveCancel.Status;
                            ltform.UpdatedBy = lLeaveCancel.UpdatedBy;
                            ltform.UpdatedDate = lLeaveCancel.UpdatedDate;
                            ltform.Processed = 2;
                            //db.Timesheet_Request_Form.Add(ltform);
                            db.SaveChanges();
                        }
                        var lleave = db.Leaves.ToList();

                        var relation = db.Leaves.Where(i => i.Id == leaveid && i.StartDate == leaves.StartDate && i.EndDate == leaves.EndDate).ToList();

                        foreach (var l in relation)
                        {

                            var wd = db.WorkDiary.Where(w => w.RefId == l.Id && (w.WDDate >= l.StartDate && w.WDDate <= l.EndDate)).ToList();

                            foreach (var w in wd)
                            {
                               
                              //  int llempid = Convert.ToInt32(lCredentails.EmpId);
                              //  var relation1 = db.WorkDiary.Where(i => i.EmpId == llempid && (i.WDDate >= l.StartDate && i.WDDate <= l.EndDate)).Where(i => i.RefId == l.Id).Select(a => a.Id).ToList();
                                var wdet = db.WorkDiary_Det.Where(d => d.WDId == w.Id).ToList();
                                //foreach (var d in wd)
                                //{
                                    foreach (var h in wdet)
                                    {
                                        db.WorkDiary_Det.Remove(h);
                                    }
                               // }
                                db.WorkDiary.Remove(w);
                            }


                            db.SaveChanges();
                        }
                        return RedirectToAction("LeavesHistory", "AllReports");
                    }
                    if (llastid == null)
                    {
                        EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lLeaveCancel.EmpId).Where(a => a.LeaveTypeId == lvalue && a.Year == lLeaveCancel.LeavesYear).FirstOrDefault();
                        int totalLeaveBalance = lbalance.LeaveBalance;
                        int CancelLeaveDays = lLeaveCancel.LeaveDays;
                        int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                        int Totaldays = totalLeaveBalance + CancelLeaveDays;
                        lbalance.Debits = lbalance.Debits - CancelLeaveDays;
                        lbalance.LeaveTypeId = Convert.ToInt32(leaves.LeaveType);
                        lbalance.EmpId = ids;
                        lbalance.LeaveBalance = Totaldays;
                        lbalance.UpdatedBy = lCredentails.EmpId;
                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();
                        //timesheet request form insertion for approved cancellation
                        if (leaves.EndDate <= DateTime.Now.Date && leaves.Status == "Approved")
                        {
                            string lcode = db.LeaveTypes.Where(a => a.Id == lLeaveCancel.LeaveType).Select(a => a.Code).FirstOrDefault();
                            int branchid = db.Employes.Where(a => a.Id == ids).Select(a => a.Branch).FirstOrDefault();
                            int? shiftids = db.Employes.Where(a => a.Id == ids).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                            ltform.UserId = ids;
                            ltform.BranchId = (int)lLeaveCancel.BranchId;
                            ltform.DepartmentId = (int)lLeaveCancel.DepartmentId;
                            ltform.DesignationId = (int)lLeaveCancel.DesignationId;
                            ltform.Shift_Id = (int)shiftids;
                            ltform.Reason_Type = "AB";
                            ltform.Reason_Desc = "Leave";
                            ltform.ReqFromDate = leaves.Cancelstartdate;
                            ltform.ReqToDate = leaves.Cancelenddate;
                            ltform.CA = lLeaveCancel.ControllingAuthority;
                            ltform.SA = lLeaveCancel.SanctioningAuthority;
                            ltform.Status = lLeaveCancel.Status;
                            ltform.UpdatedBy = lLeaveCancel.UpdatedBy;
                            ltform.UpdatedDate = lLeaveCancel.UpdatedDate;
                            ltform.Processed = 2;
                            //db.Timesheet_Request_Form.Add(ltform);
                            db.SaveChanges();
                        }

                        TempData["status"] = "Leave Cancelled Successfully";
                        var lleave = db.Leaves.ToList();

                      

                        var relation = db.Leaves.Where(i => i.Id == leaveid && i.StartDate >= leaves.StartDate && i.EndDate <= leaves.EndDate).ToList();

                        foreach (var l in relation)
                        {

                            var wd = db.WorkDiary.Where(w => w.RefId == l.Id && (w.WDDate >= l.StartDate && w.WDDate <= l.EndDate)).ToList();

                            foreach (var w in wd)
                            {

                                //  int llempid = Convert.ToInt32(lCredentails.EmpId);
                                //  var relation1 = db.WorkDiary.Where(i => i.EmpId == llempid && (i.WDDate >= l.StartDate && i.WDDate <= l.EndDate)).Where(i => i.RefId == l.Id).Select(a => a.Id).ToList();
                                var wdet = db.WorkDiary_Det.Where(d => d.WDId == w.Id).ToList();
                                //foreach (var d in wd)
                                //{
                                foreach (var h in wdet)
                                {
                                    db.WorkDiary_Det.Remove(h);
                                }
                                // }
                                db.WorkDiary.Remove(w);
                            }


                            db.SaveChanges();
                        }
                        return RedirectToAction("LeavesHistory", "AllReports");
                    }
                    else
                    {
                        string day = "15";
                        string month = "mar";
                        string year = DateTime.Now.Year.ToString();
                        string careylapse = day + "-" + month + "-" + year;
                        DateTime llapsedate = Convert.ToDateTime(careylapse).Date;
                        DateTime lupade = Convert.ToDateTime(lLeaveCancel.UpdatedDate).Date;
                        Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();
                        int? lcaryleavebal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Count();
                        int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                        int? leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3 && a.Year == lLeaveCancel.LeavesYear).Select(a => a.LeaveBalance).FirstOrDefault();
                        int carrylbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3 && a.Year == lLeaveCancel.LeavesYear).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                        EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lLeaveCancel.EmpId).Where(a => a.LeaveTypeId == lvalue && a.Year == lLeaveCancel.LeavesYear).FirstOrDefault();
                        int totalLeaveBalance = lbalance.LeaveBalance;
                        int CancelLeaveDays = lLeaveCancel.LeaveDays;
                        int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                        int Totaldays = totalLeaveBalance + lastleavedays;

                        lbalance.LeaveTypeId = Convert.ToInt32(leaves.LeaveType);
                        lbalance.EmpId = ids;
                        lbalance.Debits = lbalance.Debits - CancelLeaveDays;
                        //res.Reason = lLeaveCancel.Reason  ;
                        lbalance.LeaveBalance = Totaldays;
                        lbalance.UpdatedBy = lCredentails.EmpId;

                        db.Entry(lbalance).State = EntityState.Modified;
                        db.SaveChanges();
                        //timesheet request form insertion for approved cancellation
                        if (leaves.EndDate <= DateTime.Now.Date && leaves.Status == "Approved")
                        {
                            string lcode = db.LeaveTypes.Where(a => a.Id == lLeaveCancel.LeaveType).Select(a => a.Code).FirstOrDefault();
                            int branchid = db.Employes.Where(a => a.Id == ids).Select(a => a.Branch).FirstOrDefault();
                            int? shiftids = db.Employes.Where(a => a.Id == ids).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                            ltform.UserId = ids;
                            ltform.BranchId = (int)lLeaveCancel.BranchId;
                            ltform.DepartmentId = (int)lLeaveCancel.DepartmentId;
                            ltform.DesignationId = (int)lLeaveCancel.DesignationId;
                            ltform.Shift_Id = (int)shiftids;
                            ltform.Reason_Type = "AB";
                            ltform.Reason_Desc = "Leave";
                            ltform.ReqFromDate = leaves.Cancelstartdate;
                            ltform.ReqToDate = leaves.Cancelenddate;
                            ltform.CA = lLeaveCancel.ControllingAuthority;
                            ltform.SA = lLeaveCancel.SanctioningAuthority;
                            ltform.Status = lLeaveCancel.Status;
                            ltform.UpdatedBy = lLeaveCancel.UpdatedBy;
                            ltform.UpdatedDate = lLeaveCancel.UpdatedDate;
                            ltform.Processed = 2;
                            //db.Timesheet_Request_Form.Add(ltform);
                            db.SaveChanges();
                        }

                        TempData["status"] = "Leave Cancelled Successfully";
                        var lleave = db.Leaves.ToList();

                        var relation = db.Leaves.Where(i => i.Id == leaveid && i.StartDate == leaves.StartDate && i.EndDate == leaves.EndDate).ToList();

                        foreach (var l in relation)
                        {

                            var wd = db.WorkDiary.Where(w => w.RefId == l.Id && (w.WDDate >= l.StartDate && w.WDDate <= l.EndDate)).ToList();

                            foreach (var w in wd)
                            {

                                //  int llempid = Convert.ToInt32(lCredentails.EmpId);
                                //  var relation1 = db.WorkDiary.Where(i => i.EmpId == llempid && (i.WDDate >= l.StartDate && i.WDDate <= l.EndDate)).Where(i => i.RefId == l.Id).Select(a => a.Id).ToList();
                                var wdet = db.WorkDiary_Det.Where(d => d.WDId == w.Id).ToList();
                                //foreach (var d in wd)
                                //{
                                foreach (var h in wdet)
                                {
                                    db.WorkDiary_Det.Remove(h);
                                }
                                // }
                                db.WorkDiary.Remove(w);
                            }


                            db.SaveChanges();
                        }
                        return RedirectToAction("LeavesHistory", "AllReports");
                    }
                }
                if (leaves.canceltype == "Partial")
                {
                    int lControllingAuthority = Convert.ToInt32(lLeaveCancel.ControllingAuthority);
                    int lSanctioningAuthority = Convert.ToInt32(lLeaveCancel.SanctioningAuthority);
                    res.ControllingAuthority = lControllingAuthority;
                    res.SanctioningAuthority = lSanctioningAuthority;
                    res.LeaveType = Convert.ToInt32(leaves.LeaveType);
                    if (leaves.StartDate == leaves.Cancelstartdate && leaves.EndDate == leaves.Cancelenddate)
                    {
                        lLeaveCancel.LeavesYear = DateTime.Today.Year;
                        lLeaveCancel.Stage = leaves.Status;
                        lLeaveCancel.Status = "Cancelled";
                        lLeaveCancel.UpdatedBy = lCredentails.EmpId;
                        lLeaveCancel.UpdatedDate = GetCurrentTime(DateTime.Now.Date);
                        lLeaveCancel.LeaveTimeStamp = GetCurrentTime(DateTime.Now.Date);
                        lLeaveCancel.CancelReason = leaves.fullCancelReason;
                        db.Entry(lLeaveCancel).State = EntityState.Modified;
                        db.SaveChanges();
                        if (llastid == null && ltype == "LOP")
                        {
                            EmpLeaveBalance lbalance1 = lEmpBalance.Where(a => a.EmpId == lLeaveCancel.EmpId).Where(a => a.LeaveTypeId == lvalue && a.Year == lLeaveCancel.LeavesYear).FirstOrDefault();
                            int holidaycount = db.HolidayList.Where(a => a.Date >= leaves.Cancelstartdate).Where(a => a.Date <= leaves.Cancelenddate).Count();
                            int totalLeaveBalance1 = lbalance1.LeaveBalance;
                            int CancelLeaveDays = lLeaveCancel.LeaveDays - holidaycount;
                            int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                            int Totaldays = totalLeaveBalance1 - CancelLeaveDays;
                            lbalance1.Debits = lbalance1.Debits - CancelLeaveDays;
                            lbalance1.LeaveTypeId = Convert.ToInt32(leaves.LeaveType);
                            lbalance1.EmpId = ids;
                            //res.Reason = lLeaveCancel.Reason;
                            lbalance1.LeaveBalance = Totaldays;
                            lbalance1.UpdatedBy = lCredentails.EmpId;
                            db.Entry(lbalance1).State = EntityState.Modified;
                            db.SaveChanges();
                            //timesheet request form insertion for approved cancellation
                            if (leaves.EndDate <= DateTime.Now.Date && leaves.Status == "Approved")
                            {
                                string lcode = db.LeaveTypes.Where(a => a.Id == lLeaveCancel.LeaveType).Select(a => a.Code).FirstOrDefault();
                                int branchid = db.Employes.Where(a => a.Id == ids).Select(a => a.Branch).FirstOrDefault();
                                int? shiftids = db.Employes.Where(a => a.Id == ids).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                ltform.UserId = ids;
                                ltform.BranchId = (int)lLeaveCancel.BranchId;
                                ltform.DepartmentId = (int)lLeaveCancel.DepartmentId;
                                ltform.DesignationId = (int)lLeaveCancel.DesignationId;
                                ltform.Shift_Id = (int)shiftids;
                                ltform.Reason_Type = "AB";
                                ltform.Reason_Desc = "Leave";
                                ltform.ReqFromDate = leaves.Cancelstartdate;
                                ltform.ReqToDate = leaves.Cancelenddate;
                                ltform.CA = lLeaveCancel.ControllingAuthority;
                                ltform.SA = lLeaveCancel.SanctioningAuthority;
                                ltform.Status = "Cancelled";
                                ltform.UpdatedBy = lLeaveCancel.UpdatedBy;
                                ltform.UpdatedDate = lLeaveCancel.UpdatedDate;
                                ltform.Processed = 2;
                                //db.Timesheet_Request_Form.Add(ltform);
                                db.SaveChanges();
                            }
                            TempData["status"] = "PartialLeave Cancelled Successfully";
                        }
                        if (llastid == null)
                        {

                            string day = "15";
                            string month = "mar";
                            string year = DateTime.Now.Year.ToString();
                            string careylapse = day + "-" + month + "-" + year;
                            DateTime llapsedate = Convert.ToDateTime(careylapse).Date;
                            DateTime lupade = Convert.ToDateTime(lLeaveCancel.UpdatedDate).Date;
                            Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();
                            int? lcaryleavebal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Count();
                            int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                            int? leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                            int carrylbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                            EmpLeaveBalance lbalance1 = lEmpBalance.Where(a => a.EmpId == lLeaveCancel.EmpId).Where(a => a.LeaveTypeId == lvalue && a.Year == DateTime.Now.Year).FirstOrDefault();
                            int holidaycount = db.HolidayList.Where(a => a.Date >= leaves.Cancelstartdate).Where(a => a.Date <= leaves.Cancelenddate).Count();
                            int totalLeaveBalance1 = lbalance1.LeaveBalance;
                            int CancelLeaveDays = lLeaveCancel.LeaveDays - holidaycount;
                            int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                            int Totaldays = totalLeaveBalance1 + CancelLeaveDays;
                            lbalance1.LeaveTypeId = Convert.ToInt32(leaves.LeaveType);
                            lbalance1.EmpId = ids;
                            //res.Reason = lLeaveCancel.Reason;
                            lbalance1.LeaveBalance = Totaldays;
                            lbalance1.Debits = lbalance1.Debits - CancelLeaveDays;
                            lbalance1.UpdatedBy = lCredentails.EmpId;
                            db.Entry(lbalance1).State = EntityState.Modified;
                            db.SaveChanges();
                            //timesheet request form insertion for approved cancellation
                            if (leaves.EndDate <= DateTime.Now.Date && leaves.Status == "Approved")
                            {
                                string lcode = db.LeaveTypes.Where(a => a.Id == lLeaveCancel.LeaveType).Select(a => a.Code).FirstOrDefault();
                                int branchid = db.Employes.Where(a => a.Id == ids).Select(a => a.Branch).FirstOrDefault();
                                int? shiftids = db.Employes.Where(a => a.Id == ids).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                ltform.UserId = ids;
                                ltform.BranchId = (int)lLeaveCancel.BranchId;
                                ltform.DepartmentId = (int)lLeaveCancel.DepartmentId;
                                ltform.DesignationId = (int)lLeaveCancel.DesignationId;
                                ltform.Shift_Id = (int)shiftids;
                                ltform.Reason_Type = "AB";
                                ltform.Reason_Desc = "Leave";
                                ltform.ReqFromDate = leaves.Cancelstartdate;
                                ltform.ReqToDate = leaves.Cancelenddate;
                                ltform.CA = lLeaveCancel.ControllingAuthority;
                                ltform.SA = lLeaveCancel.SanctioningAuthority;
                                ltform.Status = "Cancelled";
                                ltform.UpdatedBy = lLeaveCancel.UpdatedBy;
                                ltform.UpdatedDate = lLeaveCancel.UpdatedDate;
                                ltform.Processed = 2;
                                //db.Timesheet_Request_Form.Add(ltform);
                                db.SaveChanges();
                            }
                            TempData["status"] = "PartialLeave Cancelled Successfully";
                        }
                        else
                        {
                            string day = "15";
                            string month = "mar";
                            string year = DateTime.Now.Year.ToString();
                            string careylapse = day + "-" + month + "-" + year;
                            DateTime llapsedate = Convert.ToDateTime(careylapse).Date;
                            DateTime lupade = Convert.ToDateTime(lLeaveCancel.UpdatedDate).Date;
                            Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();
                            int? lcaryleavebal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Count();
                            int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                            int? leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                            int carrylbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                            EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lLeaveCancel.EmpId).Where(a => a.LeaveTypeId == lvalue && a.Year == DateTime.Now.Year).FirstOrDefault();
                            int totalLeaveBalance = lbalance.LeaveBalance;
                            int CancelLeaveDays = lLeaveCancel.LeaveDays;
                            int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                            lbalance.Debits = lbalance.Debits - CancelLeaveDays;
                            int Totaldays = totalLeaveBalance + lastleavedays;
                            lbalance.LeaveTypeId = Convert.ToInt32(leaves.LeaveType);
                            lbalance.EmpId = ids;

                            //res.Reason = lLeaveCancel.Reason;
                            lbalance.LeaveBalance = Totaldays;
                            lbalance.UpdatedBy = lCredentails.EmpId;

                            db.Entry(lbalance).State = EntityState.Modified;
                            db.SaveChanges();
                            //timesheet request form insertion for approved cancellation
                            if (leaves.EndDate <= DateTime.Now.Date && leaves.Status == "Approved")
                            {
                                string lcode = db.LeaveTypes.Where(a => a.Id == lLeaveCancel.LeaveType).Select(a => a.Code).FirstOrDefault();
                                int branchid = db.Employes.Where(a => a.Id == ids).Select(a => a.Branch).FirstOrDefault();
                                int? shiftids = db.Employes.Where(a => a.Id == ids).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                ltform.UserId = ids;
                                ltform.BranchId = (int)lLeaveCancel.BranchId;
                                ltform.DepartmentId = (int)lLeaveCancel.DepartmentId;
                                ltform.DesignationId = (int)lLeaveCancel.DesignationId;
                                ltform.Shift_Id = (int)shiftids;
                                ltform.Reason_Type = "AB";
                                ltform.Reason_Desc = "Leave";
                                ltform.ReqFromDate = leaves.Cancelstartdate;
                                ltform.ReqToDate = leaves.Cancelenddate;
                                ltform.CA = lLeaveCancel.ControllingAuthority;
                                ltform.SA = lLeaveCancel.SanctioningAuthority;
                                ltform.Status = "Cancelled";
                                ltform.UpdatedBy = lLeaveCancel.UpdatedBy;
                                ltform.UpdatedDate = lLeaveCancel.UpdatedDate;
                                ltform.Processed = 2;
                                //db.Timesheet_Request_Form.Add(ltform);
                                db.SaveChanges();
                            }

                            TempData["status"] = "Leave Cancelled Successfully";
                            var lleave = db.Leaves.ToList();

                            var relation = db.Leaves.Where(i => i.Id == leaveid && i.StartDate == leaves.StartDate && i.EndDate == leaves.EndDate).ToList();

                            foreach (var l in relation)
                            {
                               
                                var wd = db.WorkDiary.Where(w => w.RefId == l.Id && (w.WDDate >= l.StartDate && w.WDDate <= l.EndDate)).ToList();

                                foreach (var w in wd)
                                {
                                   
                                   // int llempid = Convert.ToInt32(lCredentails.EmpId);
                                 //   var relation1 = db.WorkDiary.Where(i => i.EmpId == llempid && (i.WDDate >= l.StartDate && i.WDDate <= l.EndDate)).Where(i => i.RefId == l.Id).Select(a => a.Id).ToList();
                                    var wdet = db.WorkDiary_Det.Where(d => d.WDId == w.Id).ToList();
                                    //foreach (var d in relation1)
                                    //{
                                        foreach (var h in wdet)
                                        {
                                            db.WorkDiary_Det.Remove(h);
                                        }
                                        db.WorkDiary.Remove(w);
                                   // }
                                }


                                db.SaveChanges();
                            }
                            return RedirectToAction("LeavesHistory", "AllReports");
                        }
                    }
                    else
                    {

                        int lControllingAuthoritys = Convert.ToInt32(lLeaveCancel.ControllingAuthority);
                        int lSanctioningAuthoritys = Convert.ToInt32(lLeaveCancel.SanctioningAuthority);
                        res.ControllingAuthority = lControllingAuthoritys;
                        res.SanctioningAuthority = lSanctioningAuthoritys;
                        res.LeaveType = Convert.ToInt32(leaves.LeaveType);
                        res.Status = "PartialCancelled";
                        string lstatus = leaves.Status;
                        res.Stage = leaves.Status;
                        res.Subject = lLeaveCancel.Subject;
                        res.StartDate = leaves.Cancelstartdate;
                        res.EndDate = leaves.Cancelenddate;
                        res.LeaveDays = leaves.CancelDays;
                        res.TotalDays = leaves.CancelDays;
                        res.LeavesYear = DateTime.Today.Year;
                        res.Reason = lLeaveCancel.Reason;
                        res.CancelReason = leaves.patialCancelReason;
                          res.MaternityType = mtype;
                        // res.DateofDelivery = leaves.DateofDelivery;
                        res.LeaveTimeStamp = GetCurrentTime(DateTime.Now.Date);
                        //res.UpdatedDate = GetCurrentTime(DateTime.Now.Date);
                        res.UpdatedDate = lLeaveCancel.UpdatedDate;
                        res.UpdatedBy = lCredentails.EmpId;
                        res.EmpId =lLeaveCancel.EmpId;

                        res.BranchId = brid;
                        res.DepartmentId = deptid;
                        res.DesignationId = desid;
                        db.Leaves.Add(res);
                        db.SaveChanges();
                        TempData["status"] = "PartialLeave Cancelled Successfully";
                        int dayChunkSize = 1;
                        DateTime start = leaves.StartDate.AddDays(-1);
                        DateTime end = leaves.EndDate;

                        DateTime chunkEnd;
                        List<Dates> listDates = new List<Dates>();


                        List<string> lvalues = new List<string>();
                        List<string> Dates = new List<string>();
                        var holidaycounts = db.HolidayList.Where(a => a.Date >= lLeaveCancel.StartDate).Where(a => a.Date <= lLeaveCancel.EndDate).Select(a => a.Date).ToList();
                        List<DateTime> allDates1 = new List<DateTime>();
                        List<DateTime> allDates = new List<DateTime>();
                        DateTime startDate = lLeaveCancel.StartDate;
                        DateTime endDate = lLeaveCancel.EndDate;

                        // List of holidays in the applied range
                        //List<DateTime> holidays = db.HolidayList
                        //    .Where(a => a.Date >= startDate && a.Date <= endDate)
                        //    .Select(a => a.Date)
                        //    .ToList();
                        var holidaysFromDb = db.HolidayList
     .Where(a => a.Date >= startDate && a.Date <= endDate)
     .Select(a => a.Date)
     .ToList();  // Execute the query here

                        // Now safely use .Date in memory (this part runs in C#, not SQL)
                        var holidaySet = new HashSet<DateTime>(holidaysFromDb.Select(h => h.Date));// Full list of applied leave dates
                        List<DateTime> appliedDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                                                .Select(offset => startDate.AddDays(offset))
                                                                .ToList();

                        // Exclude cancelled range
                        //List<DateTime> validDates = appliedDates
                        //    .Where(d => d < leaves.Cancelstartdate || d > leaves.Cancelenddate) 
                        //    && !holidaySet.Contains(d)// remove cancelled days
                        //     .ToList() ;
                        List<DateTime> validDates = appliedDates
    .Where(d =>
        (d < leaves.Cancelstartdate.Date || d > leaves.Cancelenddate.Date) &&  // remove cancelled days
        !holidaySet.Contains(d))                                                // exclude holidays
    .ToList();
                        validDates.Sort();

                        // Now group continuous dates into ranges
                        List<(DateTime Start, DateTime End)> leaveRanges = new List<(DateTime, DateTime)>();
                        DateTime? rangeStart = null;
                        DateTime? prevDate = null;

                        foreach (var date in validDates)
                        {
                            if (rangeStart == null) // start new range
                            {
                                rangeStart = date;
                                prevDate = date;
                            }
                            else if (
                                date == prevDate.Value.AddDays(1))
                            {
                                prevDate = date;
                            }
                            else // break in sequence → save old range
                            {
                                leaveRanges.Add((rangeStart.Value, prevDate.Value));
                                rangeStart = date;
                                prevDate = date;
                            }
                        }

                        // add last range if any
                        if (rangeStart != null)
                        {
                            leaveRanges.Add((rangeStart.Value, prevDate.Value));
                        }

                        // ✅ Insert ranges into DB instead of single dates
                        foreach (var (rangeStartDate, rangeEndDate) in leaveRanges)
                        {
                            //Leaves res = new Leaves();
                            res.EmpId = lLeaveCancel.EmpId;
                            res.StartDate = rangeStartDate;
                            res.EndDate = rangeEndDate;
                            res.LeaveDays = GetdiffbetweendatesLeaves(rangeStartDate, rangeEndDate);
                            res.TotalDays = res.LeaveDays;
                            res.Status = leaves.Status;
                            res.Stage = leaves.Status;
                            res.Subject = leaves.Subject;
                            res.Reason = leaves.Reason;
                            res.LeavesYear = DateTime.Today.Year;
                            res.CancelReason = leaves.patialCancelReason;
                            res.BranchId = brid;
                            res.DepartmentId = deptid;
                            res.DesignationId = desid;
                            res.MaternityType = mtype;
                            res.LeaveTimeStamp = GetCurrentTime(DateTime.Now.Date);
                            res.UpdatedDate = GetCurrentTime(DateTime.Now.Date);
                            res.UpdatedBy = lCredentails.EmpId;

                            db.Leaves.Add(res);
                            db.SaveChanges();
                        }



                        var lleave = db.Leaves.ToList();
                        var lrelation = db.Leaves.Where(i => i.EmpId == lempids && i.StartDate == leaves.StartDate && i.EndDate == leaves.EndDate).ToList();
                        var relation = db.Leaves.Where(i => i.EmpId == lempids && i.StartDate >= leaves.Cancelstartdate && i.EndDate <= leaves.Cancelenddate).ToList();
                        foreach (var ls in lrelation)
                        {
                            db.Leaves.Remove(ls);
                        }
                            foreach (var l in lrelation)
                            {


                               // var wd = db.WorkDiary.Where(w => w.RefId == l.Id && (w.WDDate >= leaves.Cancelstartdate && w.WDDate <= leaves.Cancelenddate)).ToList();

                                //foreach (var w in wd)
                                //{

                                //    int llempid = Convert.ToInt32(lCredentails.EmpId);

                                //    var wdet = db.WorkDiary_Det.Where(d => d.WDId == w.Id).ToList();
                                //    //foreach (var d in relation1)
                                //    //{
                                //    foreach (var h in wdet)
                                //    {
                                //        db.WorkDiary_Det.Remove(h);
                                //    }
                                //    db.WorkDiary.Remove(w);
                                //    // }
                                //}

                            
                          
                        }
                      
                        db.SaveChanges();
                        string day = "15";
                        string month = "mar";
                        string year = DateTime.Now.Year.ToString();
                        string careylapse = day + "-" + month + "-" + year;
                        DateTime llapsedate = Convert.ToDateTime(careylapse).Date;
                        DateTime lupade = Convert.ToDateTime(lLeaveCancel.UpdatedDate).Date;
                        Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();
                        int? lcaryleavebal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Count();
                        int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                        int? leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                        int carrylbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                        EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lLeaveCancel.EmpId).Where(a => a.LeaveTypeId == lvalue && a.Year == DateTime.Now.Year).FirstOrDefault();
                        int totalLeaveBalance = lbalance.LeaveBalance;
                        int holidaycount = db.HolidayList.Where(a => a.Date >= leaves.Cancelstartdate).Where(a => a.Date <= leaves.Cancelenddate).Count();
                        if (lleavetype == "CL")
                        {
                            int CancelLeaveDays = leaves.CancelDays - holidaycount;
                            int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                            int Totaldays = totalLeaveBalance + CancelLeaveDays;
                            lbalance.LeaveTypeId = Convert.ToInt32(lLeaveCancel.LeaveType);
                            lbalance.EmpId = ids;
                            lbalance.Debits = lbalance.Debits - CancelLeaveDays;
                            lbalance.LeaveBalance = Totaldays;
                            lbalance.UpdatedBy = lCredentails.EmpId;
                            db.Entry(lbalance).State = EntityState.Modified;
                            db.SaveChanges();
                            //timesheet request form insertion for approved cancellation
                            if (leaves.EndDate <= DateTime.Now.Date && leaves.Status == "Approved")
                            {
                                string lcode = db.LeaveTypes.Where(a => a.Id == lLeaveCancel.LeaveType).Select(a => a.Code).FirstOrDefault();
                                int branchid = db.Employes.Where(a => a.Id == res.EmpId).Select(a => a.Branch).FirstOrDefault();
                                int? shiftids = db.Employes.Where(a => a.Id == res.EmpId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                ltform.UserId = (int)res.EmpId;
                                ltform.BranchId = (int)lLeaveCancel.BranchId;
                                ltform.DepartmentId = (int)lLeaveCancel.DepartmentId;
                                ltform.DesignationId = (int)lLeaveCancel.DesignationId;
                                ltform.Shift_Id = (int)shiftids;
                                ltform.Reason_Type = "AB";
                                ltform.Reason_Desc = "Leave";
                                ltform.ReqFromDate = leaves.Cancelstartdate;
                                ltform.ReqToDate = leaves.Cancelenddate;
                                ltform.CA = lLeaveCancel.ControllingAuthority;
                                ltform.SA = lLeaveCancel.SanctioningAuthority;
                                ltform.Status = "Cancelled";
                                ltform.UpdatedBy = lLeaveCancel.UpdatedBy;
                                ltform.UpdatedDate = lLeaveCancel.UpdatedDate;
                                ltform.Processed = 2;
                                //db.Timesheet_Request_Form.Add(ltform);
                                db.SaveChanges();
                            }

                        }
                        else if (lleavetype == "LOP")
                        {
                            int CancelLeaveDays = leaves.CancelDays;
                            int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                            int Totaldays = totalLeaveBalance - CancelLeaveDays;
                            lbalance.LeaveTypeId = Convert.ToInt32(lLeaveCancel.LeaveType);
                            lbalance.EmpId = ids;
                            lbalance.Debits = lbalance.Debits - CancelLeaveDays;
                            lbalance.LeaveBalance = Totaldays;
                            lbalance.UpdatedBy = lCredentails.EmpId;
                            db.Entry(lbalance).State = EntityState.Modified;
                            db.SaveChanges();
                            //timesheet request form insertion for approved cancellation
                            if (leaves.EndDate <= DateTime.Now.Date && leaves.Status == "Approved")
                            {
                                string lcode = db.LeaveTypes.Where(a => a.Id == lLeaveCancel.LeaveType).Select(a => a.Code).FirstOrDefault();
                                int branchid = db.Employes.Where(a => a.Id == res.EmpId).Select(a => a.Branch).FirstOrDefault();
                                int? shiftids = db.Employes.Where(a => a.Id == res.EmpId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                ltform.UserId = (int)res.EmpId;
                                ltform.BranchId = (int)lLeaveCancel.BranchId;
                                ltform.DepartmentId = (int)lLeaveCancel.DepartmentId;
                                ltform.DesignationId = (int)lLeaveCancel.DesignationId;
                                ltform.Shift_Id = (int)shiftids;
                                ltform.Reason_Type = "AB";
                                ltform.Reason_Desc = "Leave";
                                ltform.ReqFromDate = leaves.Cancelstartdate;
                                ltform.ReqToDate = leaves.Cancelenddate;
                                ltform.CA = lLeaveCancel.ControllingAuthority;
                                ltform.SA = lLeaveCancel.SanctioningAuthority;
                                ltform.Status = "Cancelled";
                                ltform.UpdatedBy = lLeaveCancel.UpdatedBy;
                                ltform.UpdatedDate = lLeaveCancel.UpdatedDate;
                                ltform.Processed = 2;
                                //db.Timesheet_Request_Form.Add(ltform);
                                db.SaveChanges();
                            }

                        }
                        else
                        {
                            int CancelLeaveDays = leaves.CancelDays;
                            int ids = Convert.ToInt32(lLeaveCancel.EmpId);
                            int Totaldays = totalLeaveBalance + CancelLeaveDays;
                            lbalance.LeaveTypeId = Convert.ToInt32(lLeaveCancel.LeaveType);
                            lbalance.EmpId = ids;
                            lbalance.Debits = lbalance.Debits - CancelLeaveDays;
                            lbalance.LeaveBalance = Totaldays;
                            lbalance.UpdatedBy = lCredentails.EmpId;
                            db.Entry(lbalance).State = EntityState.Modified;
                            db.SaveChanges();
                            //timesheet request form insertion for approved cancellation
                            if (leaves.EndDate <= DateTime.Now.Date && leaves.Status == "Approved")
                            {
                                string lcode = db.LeaveTypes.Where(a => a.Id == lLeaveCancel.LeaveType).Select(a => a.Code).FirstOrDefault();
                                int branchid = db.Employes.Where(a => a.Id == res.EmpId).Select(a => a.Branch).FirstOrDefault();
                                int? shiftids = db.Employes.Where(a => a.Id == res.EmpId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                ltform.UserId = (int)res.EmpId;
                                ltform.BranchId = (int)lLeaveCancel.BranchId;
                                ltform.DepartmentId = (int)lLeaveCancel.DepartmentId;
                                ltform.DesignationId = (int)lLeaveCancel.DesignationId;
                                ltform.Shift_Id = (int)shiftids;
                                ltform.Reason_Type = "AB";
                                ltform.Reason_Desc = "Leave";
                                ltform.ReqFromDate = leaves.Cancelstartdate;
                                ltform.ReqToDate = leaves.Cancelenddate;
                                ltform.CA = lLeaveCancel.ControllingAuthority;
                                ltform.SA = lLeaveCancel.SanctioningAuthority;
                                ltform.Status = "Cancelled";
                                ltform.UpdatedBy = lLeaveCancel.UpdatedBy;
                                ltform.UpdatedDate = lLeaveCancel.UpdatedDate;
                                ltform.Processed = 2;
                                db.Timesheet_Request_Form.Add(ltform);
                                db.SaveChanges();
                            }

                        }
                        return RedirectToAction("LeavesHistory", "AllReports");
                    }

                }
                LogInformation.Info("AdminLeavecancellation Code Ended ");
            }
            catch (Exception ex)

            {
                ex.ToString();
                LogInformation.Info("LeaveapplyWeb, Error. Info: " + ex.Message);
            }
            return RedirectToAction("LeavesHistory", "AllReports");
        }
        public int GetdiffbetweendatesLeaves(DateTime Sd, DateTime Ed)
        {
            DateTime date1 = Sd.Date;
            DateTime date2 = Ed.Date;
            double NoOfDays = (date2 - date1).TotalDays;
            double Leavedays = NoOfDays + 1;
            int Leaveday = Convert.ToInt32(Leavedays);
            return Leaveday;
        }
        [HttpGet]
        public JsonResult GetAuthorityNamess(string Name)
        {
            string lresult = string.Empty;
            try
            {
                var employees = db.Employes.ToList();
                // var dbResult = db.Leaves.ToList();
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
                    return Json(new { lControllingAuthorityAjax = lcontrolling.FirstName + ' ' + lcontrolling.LastName, lSanctioningAuthorityAjax = lsancationing.FirstName + ' ' + lsancationing.LastName }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var lResults = (from userslist in employees
                                    where userslist.Id == lUserLoginId
                                    select new
                                    {
                                        userslist.ControllingAuthority,
                                        userslist.SanctioningAuthority,

                                    });
                    var lresponseArray = lResults.Distinct().ToArray();

                    string lControllingAuthority = lresponseArray[0].ControllingAuthority;
                    string lSanctioningAuthority = lresponseArray[0].SanctioningAuthority;

                    return Json(new { lControllingAuthorityAjax = lControllingAuthority, lSanctioningAuthorityAjax = lSanctioningAuthority }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;

        }
        public JsonResult CancelGetDatesBetween()
        {
            List<string> Dates = new List<string>();

            List<string> lvalues = new List<string>();
            var lleaves = db.Leaves.ToList();
            int leaveid = Convert.ToInt32(Session["CancelId"].ToString());

            Leaves lLeaveCancel = Facade.EntitiesFacade.GetLeaveTabledata.GetById(leaveid);
            var holidaycount = db.HolidayList.Where(a => a.Date >= lLeaveCancel.StartDate).Where(a => a.Date <= lLeaveCancel.EndDate).Select(a => a.Date).ToList();
            List<HolidayList> lholi = db.HolidayList.ToList<HolidayList>();
            List<DateTime> allDates = new List<DateTime>();
            List<DateTime> allDates1 = new List<DateTime>();
            DateTime startDate = lLeaveCancel.StartDate;
            DateTime endDate = lLeaveCancel.EndDate;

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);

            foreach (var item in holidaycount)
            {

                allDates1.Add(item);

            }
            var lholi1 = allDates1.ToArray();
            var lresponseArray = allDates.ToArray();
            if (lLeaveCancel.LeaveType.ToString() == "1")
            {
                DateTime[] Except = lresponseArray.Except(lholi1).ToArray();

                foreach (var item in Except)
                {
                    lvalues.Add(item.Date.ToString("dd/MM/yyyy"));
                }
            }
            else
            {
                foreach (var item in lresponseArray)
                {
                    lvalues.Add(item.Date.ToString("dd/MM/yyyy"));
                }
            }
            return Json(Dates = lvalues.ToList(), JsonRequestBehavior.AllowGet);
        }
        // Create PDF for All Leaves in Reports
        public FileResult CreatePdfAllLeaves()
        {
            string sd = Convert.ToString(Session["sd"]);
            string ed = Convert.ToString(Session["ed"]);
            string lstatus = Convert.ToString(Session["lstatus"]);
            int leaveTypeId = Convert.ToInt32(Session["leaveTypeId"]);
            string lType = db.LeaveTypes.Where(a => a.Id == leaveTypeId).Select(a => a.Type).FirstOrDefault();
            string Applieddate = Convert.ToString(Session["lApplied"]);
            string Requestdate = Convert.ToString(Session["lRequest"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format("AllLeaves" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(new Rectangle(1000f, 1000f));
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 10 columns  
            PdfPTable tableLayout1 = new PdfPTable(10);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF16(tableLayout1, sd, ed, lstatus, lType, Applieddate, Requestdate));
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
        protected PdfPTable Add_Content_To_PDF16(PdfPTable tableLayout1, string sd1, string ed1, string lstatus, string lid, string lapplied, string lrequest)
        {
            float[] headers1 = { 30, 53, 40, 39, 45, 45, 45, 37, 60, 46 }; //Header Widths  
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
            if (lapplied == "")
            {
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
                    AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.Reason.ToString());
                    AddCellToBody(tableLayout1, lemp.Status);

                }
                return tableLayout1;
            }
            else if (lapplied == "Applied")
            {
                if (lstatus == "ALL" && lid == "ALL")
                {
                    DateTime lstartdate = Convert.ToDateTime(sd1);
                    DateTime lenddate = Convert.ToDateTime(ed1);
                    var data = (from leave in lleave
                                join emp in lemployee on leave.EmpId equals emp.Id
                                join dept in ldepartments on emp.Department equals dept.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join branch in lbranch on emp.Branch equals branch.Id
                                join types in leavetypes on leave.LeaveType equals types.Id
                                where ((leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date)
                                   || (leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date))
                                select new
                                {
                                    //leave.Id,
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    UpdatedDate = leave.UpdatedDate,
                                    AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                    ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    leave.LeaveDays,
                                    types.Code,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,

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
                        AddCellToBody(tableLayout1, lemp.designation);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Reason.ToString());
                        AddCellToBody(tableLayout1, lemp.Status);

                    }
                    return tableLayout1;
                }
                else if (lid == "ALL")
                {
                    DateTime lstartdate = Convert.ToDateTime(sd1);
                    DateTime lenddate = Convert.ToDateTime(ed1);
                    int lid1 = Convert.ToInt32(lid);
                    var data = (from leave in lleave
                                join emp in lemployee on leave.EmpId equals emp.Id
                                join dept in ldepartments on emp.Department equals dept.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join branch in lbranch on emp.Branch equals branch.Id
                                join types in leavetypes on leave.LeaveType equals types.Id
                                where leave.Status == lstatus
                                where types.Id == lid1
                                where ((leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date)
                                     || (leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date))



                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    UpdatedDate = leave.UpdatedDate,
                                    AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                    ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    leave.LeaveDays,
                                    types.Code,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
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
                        AddCellToBody(tableLayout1, lemp.designation);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Reason.ToString());
                        AddCellToBody(tableLayout1, lemp.Status);

                    }
                    return tableLayout1;
                }
                else if (lstatus == "ALL" && lid != "ALL")
                {
                    DateTime lstartdate = Convert.ToDateTime(sd1);
                    DateTime lenddate = Convert.ToDateTime(ed1);
                    int lid1 = Convert.ToInt32(lid);
                    var data = (from leave in lleave
                                join emp in lemployee on leave.EmpId equals emp.Id
                                join dept in ldepartments on emp.Department equals dept.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join branch in lbranch on emp.Branch equals branch.Id
                                join types in leavetypes on leave.LeaveType equals types.Id
                                where ((leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date)
                                      || (leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date))
                                where leave.LeaveType == lid1

                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    UpdatedDate = leave.UpdatedDate,
                                    AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                    ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    leave.LeaveDays,
                                    types.Code,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
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
                        AddCellToBody(tableLayout1, lemp.designation);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Reason.ToString());
                        AddCellToBody(tableLayout1, lemp.Status);

                    }
                    return tableLayout1;
                }

                else if (lstatus != "ALL" && lid != "ALL")
                {
                    DateTime lstartdate = Convert.ToDateTime(sd1);
                    DateTime lenddate = Convert.ToDateTime(ed1);
                    int leaveTypeId = Convert.ToInt32(Session["leaveTypeId"]);
                    var data = (from leave in lleave
                                join emp in lemployee on leave.EmpId equals emp.Id
                                join dept in ldepartments on emp.Department equals dept.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join branch in lbranch on emp.Branch equals branch.Id
                                join types in leavetypes on leave.LeaveType equals types.Id
                                where ((leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date)
                                                 || (leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date))
                                where leave.LeaveType == leaveTypeId
                                where leave.Status == lstatus


                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    UpdatedDate = leave.UpdatedDate,
                                    AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                    ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    leave.LeaveDays,
                                    types.Code,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
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
                        AddCellToBody(tableLayout1, lemp.designation);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Reason.ToString());
                        AddCellToBody(tableLayout1, lemp.Status);

                    }
                    return tableLayout1;
                }
            }
            else if (lapplied == "Request")
            {
                if (lstatus == "ALL" && lid == "ALL")
                {
                    DateTime lstartdate = Convert.ToDateTime(sd1);
                    DateTime lenddate = Convert.ToDateTime(ed1);
                    var data = (from leave in lleave
                                join emp in lemployee on leave.EmpId equals emp.Id
                                join dept in ldepartments on emp.Department equals dept.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join branch in lbranch on emp.Branch equals branch.Id
                                join types in leavetypes on leave.LeaveType equals types.Id
                                where ((leave.StartDate.Date >= lstartdate.Date && leave.EndDate.Date <= lenddate.Date)
                                        || (leave.EndDate.Date >= lstartdate.Date && leave.StartDate.Date <= lenddate.Date))
                                select new
                                {
                                    //leave.Id,
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    UpdatedDate = leave.UpdatedDate,
                                    AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                    ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    leave.LeaveDays,
                                    types.Code,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,

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
                        AddCellToBody(tableLayout1, lemp.designation);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Reason.ToString());
                        AddCellToBody(tableLayout1, lemp.Status);

                    }
                    return tableLayout1;
                }
                else if (lid == "ALL")
                {
                    DateTime lstartdate = Convert.ToDateTime(sd1);
                    DateTime lenddate = Convert.ToDateTime(ed1);
                    int lid1 = Convert.ToInt32(lid);
                    var data = (from leave in lleave
                                join emp in lemployee on leave.EmpId equals emp.Id
                                join dept in ldepartments on emp.Department equals dept.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join branch in lbranch on emp.Branch equals branch.Id
                                join types in leavetypes on leave.LeaveType equals types.Id
                                where ((leave.StartDate.Date >= lstartdate.Date && leave.EndDate.Date <= lenddate.Date)
                                         || (leave.EndDate.Date >= lstartdate.Date && leave.StartDate.Date <= lenddate.Date))
                                where leave.Status == lstatus

                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    UpdatedDate = leave.UpdatedDate,
                                    AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                    ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    leave.LeaveDays,
                                    types.Code,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
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
                        AddCellToBody(tableLayout1, lemp.designation);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Reason.ToString());
                        AddCellToBody(tableLayout1, lemp.Status);

                    }
                    return tableLayout1;
                }
                else if (lstatus == "ALL" && lid != "ALL")
                {
                    DateTime lstartdate = Convert.ToDateTime(sd1);
                    DateTime lenddate = Convert.ToDateTime(ed1);
                    int lid1 = Convert.ToInt32(lid);
                    var data = (from leave in lleave
                                join emp in lemployee on leave.EmpId equals emp.Id
                                join dept in ldepartments on emp.Department equals dept.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join branch in lbranch on emp.Branch equals branch.Id
                                join types in leavetypes on leave.LeaveType equals types.Id
                                where ((leave.StartDate.Date >= lstartdate.Date && leave.EndDate.Date <= lenddate.Date)
                                         || (leave.EndDate.Date >= lstartdate.Date && leave.StartDate.Date <= lenddate.Date))
                                where leave.LeaveType == lid1

                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    designation = desig.Code,
                                    UpdatedDate = leave.UpdatedDate,
                                    AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                    ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    StartDate = leave.StartDate,
                                    EndDate = leave.EndDate,
                                    leave.LeaveDays,
                                    types.Code,
                                    leave.Subject,
                                    leave.Reason,
                                    leave.Status,
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
                        AddCellToBody(tableLayout1, lemp.designation);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                        AddCellToBody(tableLayout1, lemp.Code);
                        AddCellToBody(tableLayout1, lemp.Reason.ToString());
                        AddCellToBody(tableLayout1, lemp.Status);

                    }
                }
                return tableLayout1;
            }
            else if (lstatus != "ALL" && lid != "ALL")
            {
                DateTime lstartdate = Convert.ToDateTime(sd1);
                DateTime lenddate = Convert.ToDateTime(ed1);
                int leaveTypeId = Convert.ToInt32(Session["leaveTypeId"]);
                var data = (from leave in lleave
                            join emp in lemployee on leave.EmpId equals emp.Id
                            join dept in ldepartments on emp.Department equals dept.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join branch in lbranch on emp.Branch equals branch.Id
                            join types in leavetypes on leave.LeaveType equals types.Id
                            where ((leave.StartDate.Date >= lstartdate.Date && leave.EndDate.Date <= lenddate.Date)
                                            || (leave.EndDate.Date >= lstartdate.Date && leave.StartDate.Date <= lenddate.Date))
                            where leave.LeaveType == leaveTypeId
                            where leave.Status == lstatus

                            select new
                            {
                                emp.EmpId,
                                emp.ShortName,
                                designation = desig.Code,
                                UpdatedDate = leave.UpdatedDate,
                                AppliedTime = GetAppliedTime(leave.UpdatedDate),
                                ApprovedTime = GetApproveTime(leave.UpdatedDate, leave.LeaveTimeStamp, leave.Status),
                                Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                StartDate = leave.StartDate,
                                EndDate = leave.EndDate,
                                leave.LeaveDays,
                                types.Code,
                                leave.Subject,
                                leave.Reason,
                                leave.Status,
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
                    AddCellToBody(tableLayout1, lemp.designation);
                    AddCellToBody(tableLayout1, lemp.Deptbranch);
                    AddCellToBody(tableLayout1, lemp.UpdatedDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.StartDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.EndDate.ToShortDateString());
                    AddCellToBody(tableLayout1, lemp.Code);
                    AddCellToBody(tableLayout1, lemp.Reason.ToString());
                    AddCellToBody(tableLayout1, lemp.Status);

                }
                return tableLayout1;
            }
            return tableLayout1;
        }
        public class Dates
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
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

        //Export to Excel for Today Leaves
        public void ExportToExcelTodayLeaves(string empid)
        {

            try
            {
                var lemployee = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                var lleave = db.Leaves.ToList();
                var leavetypes = db.LeaveTypes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
                var employeeList = (from leaves in lleave
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
                                        DepartmentBranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                        StartDate = leaves.StartDate.ToShortDateString(),
                                        EndDate = leaves.EndDate.ToShortDateString(),
                                        LeaveType = ltype.Code,
                                        leaves.Subject,
                                        leaves.Reason,
                                        leaves.Status,
                                    });

                var gv = new GridView();
                gv.DataSource = employeeList;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
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

        //Export to Excel for All Leaves
        public void ExportToExcelAllLeaves(string empid)
        {

            try
            {
                string sd = Convert.ToString(Session["sd"]);
                string ed = Convert.ToString(Session["ed"]);
                string lstatus = Convert.ToString(Session["lstatus"]);
                int leaveTypeId = Convert.ToInt32(Session["leaveTypeId"]);
                string lType = db.LeaveTypes.Where(a => a.Id == leaveTypeId).Select(a => a.Type).FirstOrDefault();
                string Applieddate = Convert.ToString(Session["lApplied"]);
                string Requestdate = Convert.ToString(Session["lRequest"]);
                var lemployee = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                var lleave = db.Leaves.ToList();
                var leavetypes = db.LeaveTypes.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                DateTime lEndDate = GetCurrentTime(DateTime.Now).Date;
                if (Applieddate == "")
                {
                    var employeeList = (from leaves in lleave
                                        join emp in lemployee on leaves.EmpId equals emp.Id
                                        join branchs in lbranch on emp.Branch equals branchs.Id
                                        join depart in ldepartments on emp.Department equals depart.Id
                                        join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                        join ltype in leavetypes on leaves.LeaveType equals ltype.Id
                                        select new
                                        {
                                            EmpCode = emp.EmpId,
                                            Name = emp.ShortName,
                                            Designation = desig.Code,
                                            DepartmentBranch = GetBranchDepartmentConcat(branchs.Name, depart.Name),
                                            StartDate = leaves.StartDate.ToShortDateString(),
                                            EndDate = leaves.EndDate.ToShortDateString(),
                                            AppliedDate = leaves.UpdatedDate,
                                            LeaveType = ltype.Code,
                                            leaves.Reason,
                                            leaves.Subject,
                                            leaves.Status,
                                        });
                    var gv = new GridView();
                    gv.DataSource = employeeList;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
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
                else if (Applieddate == "Applied")
                {
                    if (lstatus == "ALL" && lType == "ALL")
                    {
                        DateTime lstartdate = Convert.ToDateTime(sd);
                        DateTime lenddate = Convert.ToDateTime(ed);
                        var employeeList = (from leave in lleave
                                            join emp in lemployee on leave.EmpId equals emp.Id
                                            join dept in ldepartments on emp.Department equals dept.Id
                                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                            join branch in lbranch on emp.Branch equals branch.Id
                                            join types in leavetypes on leave.LeaveType equals types.Id
                                            where ((leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date) || (leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date))
                                            select new
                                            {
                                                EmpCode = emp.EmpId,
                                                Name = emp.ShortName,
                                                Designation = desig.Code,
                                                DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                StartDate = leave.StartDate.ToShortDateString(),
                                                EndDate = leave.EndDate.ToShortDateString(),
                                                AppliedDate = leave.UpdatedDate,
                                                LeaveType = types.Code,
                                                leave.Subject,
                                                leave.Reason,
                                                leave.Status,

                                            });
                        var gv = new GridView();
                        gv.DataSource = employeeList;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
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
                    else if (lType == "ALL")
                    {
                        DateTime lstartdate = Convert.ToDateTime(sd);
                        DateTime lenddate = Convert.ToDateTime(ed);
                        int lid1 = Convert.ToInt32(leaveTypeId);
                        var employeeList = (from leave in lleave
                                            join emp in lemployee on leave.EmpId equals emp.Id
                                            join dept in ldepartments on emp.Department equals dept.Id
                                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                            join branch in lbranch on emp.Branch equals branch.Id
                                            join types in leavetypes on leave.LeaveType equals types.Id
                                            where leave.Status == lstatus
                                            where types.Id == lid1
                                            where ((leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date) || (leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date))
                                            select new
                                            {
                                                EmpCode = emp.EmpId,
                                                Name = emp.ShortName,
                                                Designation = desig.Code,
                                                DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                StartDate = leave.StartDate.ToShortDateString(),
                                                EndDate = leave.EndDate.ToShortDateString(),
                                                AppliedDate = leave.UpdatedDate,
                                                LeaveType = types.Code,
                                                leave.Subject,
                                                leave.Reason,
                                                leave.Status,
                                            });
                        var gv = new GridView();
                        gv.DataSource = employeeList;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
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
                    else if (lstatus == "ALL" && lType != "ALL")
                    {
                        DateTime lstartdate = Convert.ToDateTime(sd);
                        DateTime lenddate = Convert.ToDateTime(ed);
                        int lid1 = Convert.ToInt32(leaveTypeId);
                        var employeeList = (from leave in lleave
                                            join emp in lemployee on leave.EmpId equals emp.Id
                                            join dept in ldepartments on emp.Department equals dept.Id
                                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                            join branch in lbranch on emp.Branch equals branch.Id
                                            join types in leavetypes on leave.LeaveType equals types.Id
                                            where ((leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date)
                                                  || (leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date))
                                            where leave.LeaveType == lid1
                                            select new
                                            {
                                                EmpCode = emp.EmpId,
                                                Name = emp.ShortName,
                                                Designation = desig.Code,
                                                DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                StartDate = leave.StartDate.ToShortDateString(),
                                                EndDate = leave.EndDate.ToShortDateString(),
                                                AppliedDate = leave.UpdatedDate,
                                                LeaveType = types.Code,
                                                leave.Subject,
                                                leave.Reason,
                                                leave.Status,
                                            });
                        var gv = new GridView();
                        gv.DataSource = employeeList;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
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
                    else if (lstatus != "ALL" && lType != "ALL")
                    {
                        DateTime lstartdate = Convert.ToDateTime(sd);
                        DateTime lenddate = Convert.ToDateTime(ed);
                        int lid1 = Convert.ToInt32(leaveTypeId);
                        var employeeList = (from leave in lleave
                                            join emp in lemployee on leave.EmpId equals emp.Id
                                            join dept in ldepartments on emp.Department equals dept.Id
                                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                            join branch in lbranch on emp.Branch equals branch.Id
                                            join types in leavetypes on leave.LeaveType equals types.Id
                                            where ((leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date)
                                                             || (leave.UpdatedDate.Date >= lstartdate.Date && leave.UpdatedDate.Date <= lenddate.Date))
                                            where leave.LeaveType == lid1
                                            where leave.Status == lstatus
                                            select new
                                            {
                                                EmpCode = emp.EmpId,
                                                Name = emp.ShortName,
                                                Designation = desig.Code,
                                                DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                StartDate = leave.StartDate.ToShortDateString(),
                                                EndDate = leave.EndDate.ToShortDateString(),
                                                AppliedDate = leave.UpdatedDate,
                                                LeaveType = types.Code,
                                                leave.Subject,
                                                leave.Reason,
                                                leave.Status,
                                            });
                        var gv = new GridView();
                        gv.DataSource = employeeList;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
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
                }
                else if (Applieddate == "Request")
                {
                    if (lstatus == "ALL" && lType == "ALL")
                    {
                        DateTime lstartdate = Convert.ToDateTime(sd);
                        DateTime lenddate = Convert.ToDateTime(ed);
                        var employeeList = (from leave in lleave
                                            join emp in lemployee on leave.EmpId equals emp.Id
                                            join dept in ldepartments on emp.Department equals dept.Id
                                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                            join branch in lbranch on emp.Branch equals branch.Id
                                            join types in leavetypes on leave.LeaveType equals types.Id
                                            where ((leave.StartDate.Date >= lstartdate.Date && leave.EndDate.Date <= lenddate.Date)
                                                    || (leave.EndDate.Date >= lstartdate.Date && leave.StartDate.Date <= lenddate.Date))
                                            select new
                                            {
                                                EmpCode = emp.EmpId,
                                                Name = emp.ShortName,
                                                Designation = desig.Code,
                                                DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                StartDate = leave.StartDate.ToShortDateString(),
                                                EndDate = leave.EndDate.ToShortDateString(),
                                                AppliedDate = leave.UpdatedDate,
                                                LeaveType = types.Code,
                                                leave.Subject,
                                                leave.Reason,
                                                leave.Status,
                                            });
                        var gv = new GridView();
                        gv.DataSource = employeeList;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
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
                    else if (lType == "ALL")
                    {
                        DateTime lstartdate = Convert.ToDateTime(sd);
                        DateTime lenddate = Convert.ToDateTime(ed);
                        int lid1 = Convert.ToInt32(leaveTypeId);
                        var employeeList = (from leave in lleave
                                            join emp in lemployee on leave.EmpId equals emp.Id
                                            join dept in ldepartments on emp.Department equals dept.Id
                                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                            join branch in lbranch on emp.Branch equals branch.Id
                                            join types in leavetypes on leave.LeaveType equals types.Id
                                            where ((leave.StartDate.Date >= lstartdate.Date && leave.EndDate.Date <= lenddate.Date)
                                                     || (leave.EndDate.Date >= lstartdate.Date && leave.StartDate.Date <= lenddate.Date))
                                            where leave.Status == lstatus
                                            select new
                                            {
                                                EmpCode = emp.EmpId,
                                                Name = emp.ShortName,
                                                Designation = desig.Code,
                                                DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                StartDate = leave.StartDate.ToShortDateString(),
                                                EndDate = leave.EndDate.ToShortDateString(),
                                                AppliedDate = leave.UpdatedDate,
                                                LeaveType = types.Code,
                                                leave.Subject,
                                                leave.Reason,
                                                leave.Status,
                                            });
                        var gv = new GridView();
                        gv.DataSource = employeeList;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
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
                    else if (lstatus == "ALL" && lType != "ALL")
                    {
                        DateTime lstartdate = Convert.ToDateTime(sd);
                        DateTime lenddate = Convert.ToDateTime(ed);
                        int lid1 = Convert.ToInt32(leaveTypeId);
                        var employeeList = (from leave in lleave
                                            join emp in lemployee on leave.EmpId equals emp.Id
                                            join dept in ldepartments on emp.Department equals dept.Id
                                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                            join branch in lbranch on emp.Branch equals branch.Id
                                            join types in leavetypes on leave.LeaveType equals types.Id
                                            where ((leave.StartDate.Date >= lstartdate.Date && leave.EndDate.Date <= lenddate.Date)
                                                     || (leave.EndDate.Date >= lstartdate.Date && leave.StartDate.Date <= lenddate.Date))
                                            where leave.LeaveType == lid1
                                            select new
                                            {
                                                EmpCode = emp.EmpId,
                                                Name = emp.ShortName,
                                                Designation = desig.Code,
                                                DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                StartDate = leave.StartDate.ToShortDateString(),
                                                EndDate = leave.EndDate.ToShortDateString(),
                                                AppliedDate = leave.UpdatedDate,
                                                LeaveType = types.Code,
                                                leave.Subject,
                                                leave.Reason,
                                                leave.Status,
                                            });
                        var gv = new GridView();
                        gv.DataSource = employeeList;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
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
                    else if (lstatus != "ALL" && lType != "ALL")
                    {
                        DateTime lstartdate = Convert.ToDateTime(sd);
                        DateTime lenddate = Convert.ToDateTime(ed);
                        int lid1 = Convert.ToInt32(leaveTypeId);
                        var employeeList = (from leave in lleave
                                            join emp in lemployee on leave.EmpId equals emp.Id
                                            join dept in ldepartments on emp.Department equals dept.Id
                                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                            join branch in lbranch on emp.Branch equals branch.Id
                                            join types in leavetypes on leave.LeaveType equals types.Id
                                            where ((leave.StartDate.Date >= lstartdate.Date && leave.EndDate.Date <= lenddate.Date)
                                                            || (leave.EndDate.Date >= lstartdate.Date && leave.StartDate.Date <= lenddate.Date))
                                            where leave.LeaveType == lid1
                                            where leave.Status == lstatus
                                            select new
                                            {
                                                EmpCode = emp.EmpId,
                                                Name = emp.ShortName,
                                                Designation = desig.Code,
                                                DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                StartDate = leave.StartDate.ToShortDateString(),
                                                EndDate = leave.EndDate.ToShortDateString(),
                                                AppliedDate = leave.UpdatedDate,
                                                LeaveType = types.Code,
                                                leave.Subject,
                                                leave.Reason,
                                                leave.Status,
                                            });
                        var gv = new GridView();
                        gv.DataSource = employeeList;
                        gv.DataBind();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment; filename=LeaveReport.xls");
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
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        //Export To Excel for Credit Debit Leaves
        public void ExportToExcelCreditDebit()
        {
            string lempid = Convert.ToString(Session["lEmpId"]);
            try
            {
                if (lempid == "")
                {
                    var lleaves = db.leaves_CreditDebit.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var data = (from leave in lleaves
                                join leavetype in lLeaveTypes on leave.LeaveTypeId equals leavetype.Id
                                join emp in lemployees on leave.EmpId equals emp.Id
                                join branch in lBranches on emp.Branch equals branch.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join dept in Departments on emp.Department equals dept.Id
                                select new
                                {
                                    EmpCode = emp.EmpId,
                                    EmpName = emp.ShortName,
                                    Designation = desig.Code,
                                    DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    LeaveType = leavetype.Type,
                                    PreviousBalance = leave.LeaveBalance,
                                    leave.DebitLeave,
                                    leave.CreditLeave,
                                    CurrentBalance = TotalBalance(leave.CreditLeave, leave.DebitLeave, leave.LeaveBalance),
                                    leave.Comments,
                                });
                    var data1 = data.ToList();
                    var gv = new GridView();
                    gv.DataSource = data1;
                    if ((data1.Count == 0))
                    {
                        gv.ShowHeaderWhenEmpty = true;
                    }
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=CreditDebitLeavesList.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    StringWriter objStringWriter = new StringWriter();
                    HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                    gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                    gv.Width = 0;
                    gv.RenderControl(objHtmlTextWriter);
                    Response.Output.Write(objStringWriter.ToString());
                    Session.Remove("lEmpId");
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    var lleaves = db.leaves_CreditDebit.ToList();
                    var lBranches = db.Branches.ToList();
                    var lLeaveTypes = db.LeaveTypes.ToList();
                    var Departments = db.Departments.ToList();
                    var lemployees = db.Employes.ToList();
                    var ldesignation = db.Designations.ToList();
                    var data = (from leave in lleaves
                                join leavetype in lLeaveTypes on leave.LeaveTypeId equals leavetype.Id
                                join emp in lemployees on leave.EmpId equals emp.Id
                                join branch in lBranches on emp.Branch equals branch.Id
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join dept in Departments on emp.Department equals dept.Id
                                where (emp.EmpId.ToLower().Contains(lempid.ToLower()))
                                select new
                                {
                                    EmpCode = emp.EmpId,
                                    EmpName = emp.ShortName,
                                    Designation = desig.Code,
                                    DepartmentBranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    LeaveType = leavetype.Type,
                                    PreviousBalance = leave.LeaveBalance,
                                    leave.DebitLeave,
                                    leave.CreditLeave,
                                    CurrentBalance = TotalBalance(leave.CreditLeave, leave.DebitLeave, leave.LeaveBalance),
                                    leave.Comments,
                                });
                    var data1 = data.ToList();
                    var gv = new GridView();
                    gv.DataSource = data1;
                    if ((data1.Count == 0))
                    {
                        gv.ShowHeaderWhenEmpty = true;
                    }
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=CreditDebitLeavesList.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    StringWriter objStringWriter = new StringWriter();
                    HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                    gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                    gv.Width = 0;
                    gv.RenderControl(objHtmlTextWriter);
                    Response.Output.Write(objStringWriter.ToString());
                    Session.Remove("lEmpId");
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
    }
}





