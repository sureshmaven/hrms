using Entities;
using HRMSApplication.Filters;
using HRMSApplication.Helpers;
using HRMSApplication.Models;
using HRMSBusiness.Business;
using HRMSBusiness.Db;
using Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;

using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class LeaveApprovalController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        // GET: LeaveApproval
        [HttpGet]
        public ActionResult LeaveApproval()
        {
            LeavesBusiness lbus = new LeavesBusiness();
            int lEmpId = Convert.ToInt32(lCredentials.EmpPkId);
            string lMessage = string.Empty;
            try
            {
                var items = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().Where(x => x.Type != "ALL").OrderBy(x => x.Type).Select(x => new LeaveTypes
                {
                    Id = x.Id,
                    Type = x.Type.Trim(),
                });
                ViewBag.LeaveTypes = new SelectList(items, "Id", "Type");
                int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
                string lcode = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
                ViewBag.LeaveTypes1 = new SelectList(lbus.getAllLeaveTypes(lcode), "Id", "Type");
                //var items5 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.Name != "OtherBranch").Select(x => new Branches
                //{
                //    Id = x.Id,
                //    Name = x.Name
                //});
                //ViewBag.Branch = new SelectList(items5, "Id", "Name");
                var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Branches Where Name != 'TSCAB-CTI'");
                var items5 = dt.AsEnumerable().Select(r => new Branches
                {
                    Id = (Int32)(r["Id"]),
                    Name = (string)(r["Name"] ?? "null")
                }).ToList();
                items5.Insert(0, new Branches
                {
                    Id = -1,
                    Name = "ALL"
                });
                ViewBag.Branch = new SelectList(items5, "Id", "Name");
                var dt1 = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Departments Where Active=1");
                var items6 = dt1.AsEnumerable().Select(r => new Departments
                {
                    Id = (Int32)(r["Id"]),
                    Name = (string)(r["Name"] ?? "null")
                }).ToList();
                items6.Insert(0, new Departments
                {
                    Id = -1,
                    Name = "ALL"
                });
                ViewBag.Departments = new SelectList(items6, "Id", "Name");
                var items11 = Facade.EntitiesFacade.LeavesTypesRepositoryFacade.GetAll().OrderBy(x => x.Code).Where(x => x.Type != "ALL").Select(x => new LeaveTypes
                {
                    Id = x.Id,
                    Type = x.Code.Trim(),
                });
                ViewBag.LeaveTypes11 = new SelectList(items, "Id", "Type");

                ViewBag.LeaveTypes11 = new SelectList(items, "Id", "Type");
                V_LeaveHistory lmodel = new V_LeaveHistory();
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                return View("~/Views/LeaveApproval/_EmpLeaveApproval.cshtml", lmodel);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }

            return View(lMessage);
        }
        [HttpGet]
        public JsonResult GetDates()
        {
            List<string> Dates = new List<string>();
            var holiday = db.HolidayList.ToList();
            List<string> lvalues = new List<string>();
            var lResults = (from holidaylist in holiday
                            where holidaylist.Occasion != "Sunday" && holidaylist.Occasion != "Fourth Saturday" &&
                            holidaylist.Occasion != "Second Saturday"
                            select new
                            {
                                Dates = holidaylist.Date.ToString("dd/MM/yyyy"),
                            });
            var lresponseArray = lResults.ToArray();
            foreach (
                var item in lresponseArray)
            {
                lvalues.Add(item.Dates);
            }
            return Json(Dates = lvalues.ToList(), JsonRequestBehavior.AllowGet);
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


        public ActionResult LeaveApprovalView()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }

        



        [HttpGet]
        public JsonResult LeaveApprovalViews(string status)
        {
            string lMessage = string.Empty;
            string[] LeavesSanctioning = ConfigurationManager.AppSettings["LeavesSanctioning"].Split(',');
            try
            {
                var lleaveHistory = db.V_LeaveHistory.ToList();
                var lleaveTypes = db.LeaveTypes.ToList();
                var lEmployees = db.Employes.ToList();
                var lleaves = db.Leaves.ToList();
                var lBranches = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lControllingAuthority = db.Leaves.Where(a => a.ControllingAuthority == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                int lSancationingAuthority = db.Leaves.Where(a => a.SanctioningAuthority == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                if (lEmpId == lControllingAuthority || lCredentials.EmpId == LeavesSanctioning[0] || lCredentials.EmpId == LeavesSanctioning[1])
                {
                    string[] LeavesSanctionings = ConfigurationManager.AppSettings["LeavesSanctioning"].Split(',');
                    List<string> list = LeavesSanctionings.ToList();

                    if (ConfigurationManager.AppSettings["LeavesSanctioning"].Split(',').Contains(lCredentials.EmpId) )

                    {
                    //    if (lCredentials.EmpId == LeavesSanctioning[0] || lCredentials.EmpId == LeavesSanctioning[1])
                    //{
                        lControllingAuthority = 220;
                        lSancationingAuthority = 220;
                        var lResult = (from employee in lEmployees
                                       join leave in lleaves on employee.Id equals leave.EmpId
                                       join branch in lBranches on leave.BranchId equals branch.Id
                                       join dept in ldept on leave.DepartmentId equals dept.Id

                                       join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                       join desig in ldesignation on leave.DesignationId equals desig.Id
                                       where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                        employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                       select new
                                       {
                                           leave.Id,

                                           employee.EmpId,
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leave.StartDate,
                                           leave.EndDate,
                                           leave.LeaveDays,
                                           leavetypes.Code,
                                           leave.Reason,
                                           leave.Status

                                       }).OrderByDescending(A => A.AppliedDate);
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var lResult = (from employee in lEmployees
                                       join leave in lleaves on employee.Id equals leave.EmpId
                                       join branch in lBranches on leave.BranchId equals branch.Id
                                       join dept in ldept on leave.DepartmentId equals dept.Id

                                       join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                       join desig in ldesignation on leave.DesignationId equals desig.Id
                                       where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                        employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                       select new
                                       {
                                           leave.Id,

                                           employee.EmpId,  
                                           employee.ShortName,
                                           designation = desig.Code,
                                           AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                           Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                           leave.StartDate,
                                           leave.EndDate,
                                           leave.LeaveDays,
                                           leavetypes.Code,
                                           leave.Reason,
                                           leave.Status

                                       }).OrderByDescending(A => A.Status=="Pending");
                        return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                    }
                   

                }
                if (lEmpId == lSancationingAuthority)
                {
                    var lResult = (from employee in lEmployees
                                   join leave in lleaves on employee.Id equals leave.EmpId
                                   join branch in lBranches on leave.BranchId equals branch.Id
                                   join dept in ldept on leave.DepartmentId equals dept.Id

                                   join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                   join desig in ldesignation on leave.DesignationId equals desig.Id
                                   where employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded"
                                   select new
                                   {
                                       leave.Id,
                                       employee.EmpId,
                                       employee.ShortName,
                                       designation = desig.Code,
                                       AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                       Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                       leave.StartDate,
                                       leave.EndDate,
                                       leave.LeaveDays,
                                       leavetypes.Code,
                                       leave.Reason,
                                       leave.Status

                                   }).OrderByDescending(A => A.AppliedDate);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(status))
                {


                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        [HttpGet]
        public JsonResult Tooltip(string Empid)
        {
            string lMessage = string.Empty;
            var lleaveTypes = db.LeaveTypes.ToList();
            var lEmployees = db.Employes.ToList();
            var lleaves = db.Leaves.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int leaverowId = Convert.ToInt32(Empid);
            try
            {

                var lResult = (from leave in lleaves
                               join Employes in lEmployees on leave.EmpId equals Employes.Id
                               join branch in lBranches on leave.BranchId equals branch.Id
                               join dept in ldept on leave.DepartmentId equals dept.Id
                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                               join desig in ldesignation on leave.DesignationId equals desig.Id
                               where leave.Id == leaverowId
                               select new
                               {
                                   leave.Id,
                                   Employes.EmpId,
                                   Employes.ShortName,
                                   designation = desig.Code,
                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                   leave.StartDate,
                                   leave.EndDate,
                                   leave.LeaveDays,
                                   leavetypes.Code,
                                   leave.Reason,
                                   leave.Status

                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = leaverowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                DateTime lStartdate = lresponseArray[0].StartDate;
                DateTime lEnddate = lresponseArray[0].EndDate;
                int ldays = lresponseArray[0].LeaveDays;
                string leavetype = lresponseArray[0].Code;
                string lreason = lresponseArray[0].Reason;
                string lstatus = lresponseArray[0].Status;
                return Json(new
                {
                    lEmployeeId = employeeId,
                    lEmployeeName = employeeName,
                    ldeptbranch = Deptbranchs,
                    ldesig = ldesig12,
                    lstartdate = lStartdate,
                    lenddate = lEnddate,
                    lDays = ldays,
                    LeaveType = leavetype,
                    Reason = lreason,
                    Status = lstatus
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        [HttpGet]
        public JsonResult Nexttip(string Empid)
        {
            string lMessage = string.Empty;
            var lleaveTypes = db.LeaveTypes.ToList();
            var lEmployees = db.Employes.ToList();
            var lleaves = db.Leaves.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int leaverowId = Convert.ToInt32(Empid);
            try
            {

                var lResult = (from leave in lleaves
                               join Employes in lEmployees on leave.EmpId equals Employes.Id
                               join branch in lBranches on leave.BranchId equals branch.Id
                               join dept in ldept on leave.DepartmentId equals dept.Id
                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                               join desig in ldesignation on leave.DesignationId equals desig.Id
                               where leave.Id == leaverowId
                               select new
                               {
                                   leave.Id,
                                   Employes.EmpId,
                                   Employes.ShortName,
                                   designation = desig.Code,
                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                   leave.StartDate,
                                   leave.EndDate,
                                   leave.LeaveDays,
                                   leavetypes.Code,
                                   leave.Reason,
                                   leave.Status

                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = leaverowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                DateTime lStartdate = lresponseArray[0].StartDate;
                DateTime lEnddate = lresponseArray[0].EndDate;
                int ldays = lresponseArray[0].LeaveDays;
                string leavetype = lresponseArray[0].Code;
                string lreason = lresponseArray[0].Reason;
                string lstatus = lresponseArray[0].Status;
                return Json(new
                {
                    lEmployeeId = employeeId,
                    lEmployeeName = employeeName,
                    ldeptbranch = Deptbranchs,
                    ldesig = ldesig12,
                    lstartdate = lStartdate,
                    lenddate = lEnddate,
                    lDays = ldays,
                    LeaveType = leavetype,
                    Reason = lreason,
                    Status = lstatus
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }

        [HttpGet]
        public JsonResult Previoustip(string Empid)
        {
            string lMessage = string.Empty;
            var lleaveTypes = db.LeaveTypes.ToList();
            var lEmployees = db.Employes.ToList();
            var lleaves = db.Leaves.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int leaverowId = Convert.ToInt32(Empid);
            try
            {

                var lResult = (from leave in lleaves
                               join Employes in lEmployees on leave.EmpId equals Employes.Id
                               join branch in lBranches on leave.BranchId equals branch.Id
                               join dept in ldept on leave.DepartmentId equals dept.Id
                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                               join desig in ldesignation on leave.DesignationId equals desig.Id
                               where leave.Id == leaverowId
                               select new
                               {
                                   leave.Id,
                                   Employes.EmpId,
                                   Employes.ShortName,
                                   designation = desig.Code,
                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                   leave.StartDate,
                                   leave.EndDate,
                                   leave.LeaveDays,
                                   leavetypes.Code,
                                   leave.Reason,
                                   leave.Status

                               }).OrderByDescending(A => A.AppliedDate);
                ViewBag.LeaveRowId = leaverowId;
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].ShortName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesig12 = lresponseArray[0].designation;
                DateTime lStartdate = lresponseArray[0].StartDate;
                DateTime lEnddate = lresponseArray[0].EndDate;
                int ldays = lresponseArray[0].LeaveDays;
                string leavetype = lresponseArray[0].Code;
                string lreason = lresponseArray[0].Reason;
                string lstatus = lresponseArray[0].Status;
                return Json(new
                {
                    lEmployeeId = employeeId,
                    lEmployeeName = employeeName,
                    ldeptbranch = Deptbranchs,
                    ldesig = ldesig12,
                    lstartdate = lStartdate,
                    lenddate = lEnddate,
                    lDays = ldays,
                    LeaveType = leavetype,
                    Reason = lreason,
                    Status = lstatus
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }
        [HttpPost]
        public JsonResult LeaveApprovalViews(string EmployeeCodey, string leaveTypes, string LeaveIds)
        {
            Timesheet_Request_Form ltform = new Timesheet_Request_Form();
            WorkDiary wd = new WorkDiary();
            WorkDiary_Det wdet = new WorkDiary_Det();
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var ldbresult = db.Leaves.ToList();
                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeCodey))
                {
                    lCode = EmployeeCodey.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                List<string> lTypes = new List<string>();
                if (!string.IsNullOrEmpty(leaveTypes))
                {
                    lTypes = leaveTypes.Split(new char[] { ',' }).ToList();
                    lTypes.Remove("");
                }
                List<string> lLeaveIds = new List<string>();
                if (!string.IsNullOrEmpty(LeaveIds))
                {
                    lLeaveIds = LeaveIds.Split(new char[] { ',' }).ToList();
                    lLeaveIds.Remove("");
                }
                int j = 0;
                int k = 0;
                for (int i = 0; i < lCode.Count; i++)
                {
                    for (; j < lTypes.Count; j++)
                    {
                        for (; k < lLeaveIds.Count;)
                        {
                            string lECode = lCode[i];
                            string lType = lTypes[j];
                            string lIdss = lLeaveIds[k];
                            int lleaveTypeIds = db.LeaveTypes.Where(a => a.Code == lType).Select(a => a.Id).FirstOrDefault();
                            string lidempid= db.Employes.Where(a => a.EmpId == lECode).Select(a => a.EmpId).FirstOrDefault();
                            int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                            int LeaveId = Convert.ToInt32(lIdss);
                            string lstauts = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                            if (lstauts == "Pending")
                            {
                                int leaverowid = Convert.ToInt32(lIdss);
                                Leaves lcontrolling = Facade.EntitiesFacade.GetLeaveTabledata.GetById(leaverowid);
                                string lcontrolstatus = "Forwarded";
                                string lcontrolvalue = "0";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.Status = "Forwarded";
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolstatus, lcontrolvalue);
                                LeaveHelper.SendEmails(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.LeaveType, lcontrolling.LeaveDays, lcontrolling.Reason, lcontrolstatus, lcontrolvalue);
                                TempData["Forward"] = "Leave Forwarded Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 0;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            if (lstauts == "Forwarded")
                            {
                                int leaverowid = Convert.ToInt32(lIdss);
                                Leaves lSancationing = Facade.EntitiesFacade.GetLeaveTabledata.GetById(leaverowid);
                                string llSancationingstatus = "Approved";
                                string llSancationingvalue = "1";
                                Leaves lupdatef = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatef.Status = "Approved";
                                lupdatef.UpdatedBy = lCredentials.EmpId;
                                lupdatef.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                db.Entry(lupdatef).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, llSancationingstatus, llSancationingvalue);
                                LeaveHelper.SendEmails(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.LeaveType, lSancationing.LeaveDays, lSancationing.Reason, llSancationingstatus, llSancationingvalue);
                                TempData["Approve"] = "Leave Approved Successfully";
                                DateTime start = Convert.ToDateTime(lupdatef.StartDate).AddDays(-1);
                                DateTime end = Convert.ToDateTime(lupdatef.EndDate);
                                DateTime chunkEnd;
                                int dayChunkSize = 1;
                                while ((chunkEnd = start.AddDays(dayChunkSize)) <= end)
                                {

                                    Tuple.Create(start, chunkEnd);
                                    start = chunkEnd;
                                    DateTime chunkend1 = Convert.ToDateTime(start);
                                    //workdiary tobe done
                                    wd.EmpId =Convert.ToInt32(lidempid);
                                    wd.Status = "Approved";
                                    wd.CA = lupdatef.ControllingAuthority;
                                    wd.SA = lupdatef.SanctioningAuthority;
                                    wd.UpdatedDate = DateTime.Now;
                                    wd.WDDate = start;
                                    wd.UpdatedBy = Convert.ToInt32(lupdatef.UpdatedBy);
                                    wd.RefId = lupdatef.Id;
                                    wd.CurBr = lupdatef.BranchId;
                                    wd.Br = 0;
                                    wd.Org = 0;
                                    wd.CurDept = lupdatef.DepartmentId;
                                    wd.CurDesig = lupdatef.DesignationId;
                                    db.WorkDiary.Add(wd);
                                    db.SaveChanges();
                                    int wdtid=db.WorkDiary.OrderByDescending(x=>x.Id).Select(y => y.Id).FirstOrDefault();
                                    wdet.Name = "Leave";
                                      wdet.WDId = wdtid;
                                    string lcod = db.LeaveTypes.Where(a => a.Id == lupdatef.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    wdet.Desc = lcod;
                                   // wdet.WorkType = 0;
                                    db.WorkDiary_Det.Add(wdet);
                                    db.SaveChanges();
                                }
                                if (lupdatef.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatef.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatef.BranchId;
                                    ltform.DepartmentId = (int)lupdatef.DepartmentId;
                                    ltform.DesignationId = (int)lupdatef.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatef.StartDate;
                                    ltform.ReqToDate = lupdatef.EndDate;
                                    ltform.CA = lupdatef.ControllingAuthority;
                                    ltform.SA = lupdatef.SanctioningAuthority;
                                    ltform.Status = lupdatef.Status;
                                    ltform.UpdatedBy = lupdatef.UpdatedBy;
                                    ltform.UpdatedDate = lupdatef.UpdatedDate;
                                    ltform.Processed = 0;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            k++;
                            j++;
                            break;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }

            string id = "";
            return Json(id = "jjj", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Cancel(string EmployeeCodey, string leaveTypes, string LeaveIds, string Reason)
        {
            Timesheet_Request_Form ltform = new Timesheet_Request_Form();
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var lEmpBalance = db.EmpLeaveBalance.Where(a => a.Year == DateTime.Now.Year).ToList();
                var ldbresult = db.Leaves.ToList();
                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeCodey))
                {
                    lCode = EmployeeCodey.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                List<string> lTypes = new List<string>();
                if (!string.IsNullOrEmpty(leaveTypes))
                {
                    lTypes = leaveTypes.Split(new char[] { ',' }).ToList();
                    lTypes.Remove("");
                }
                List<string> lLeaveIds = new List<string>();
                if (!string.IsNullOrEmpty(LeaveIds))
                {
                    lLeaveIds = LeaveIds.Split(new char[] { ',' }).ToList();
                    lLeaveIds.Remove("");
                }
                int j = 0;
                int k = 0;
                for (int i = 0; i < lCode.Count; i++)
                {
                    for (; j < lTypes.Count; j++)
                    {
                        for (; k < lLeaveIds.Count;)
                        {

                            string lECode = lCode[i];
                            string lType = lTypes[j];
                            string lIdss = lLeaveIds[k];
                            int lleaveTypeIds = db.LeaveTypes.Where(a => a.Code == lType).Select(a => a.Id).FirstOrDefault();
                            string lleaveTypecode = db.LeaveTypes.Where(a => a.Code == lType).Select(a => a.Code).FirstOrDefault();
                            int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                            int LeaveId = Convert.ToInt32(lIdss);
                            string lstauts = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                            if (lstauts == "Pending" && lleaveTypecode == "LOP")
                            {
                                Leaves lcontrolling = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string lcontrolstatus = "Cancelled";
                                string lcontrolvalue = "0";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Cancelled";
                                lupdatep.Stage = lstauts;
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();

                                int TotalDays = lEmpLeaveBalancetotal - lLeaveDaystotal;

                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lbalance.LeaveBalance = TotalDays;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lEmpId, lcontrolstatus, lcontrolvalue);
                                LeaveHelper.SendEmails(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.LeaveType, lcontrolling.LeaveDays, lcontrolling.Reason, lcontrolstatus, lcontrolvalue);
                                TempData["cancel"] = "Leave Cancelled Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            else if(lstauts == "Pending" && lleaveTypecode == "W-Off")
                            {
                                Leaves lcontrolling = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string lcontrolstatus = "Cancelled";
                                string lcontrolvalue = "0";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Cancelled";
                                lupdatep.Stage = lstauts;
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();

                                int TotalDays = lEmpLeaveBalancetotal - lLeaveDaystotal;

                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lbalance.LeaveBalance = TotalDays;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lEmpId, lcontrolstatus, lcontrolvalue);
                                LeaveHelper.SendEmails(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.LeaveType, lcontrolling.LeaveDays, lcontrolling.Reason, lcontrolstatus, lcontrolvalue);
                                TempData["cancel"] = "Leave Cancelled Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            else if (lstauts == "Pending" && lleaveTypecode != "LOP" && lleaveTypecode != "W-Off")
                            {
                                Leaves lcontrolling = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string lcontrolstatus = "Cancelled";
                                string lcontrolvalue = "0";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Cancelled";
                                lupdatep.Stage = lstauts;
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();

                                int TotalDays = lEmpLeaveBalancetotal + lLeaveDaystotal;
                                // leaves carryforward

                                string day = "15";
                                string month = "mar";
                                string year = DateTime.Now.Year.ToString();
                                string careylapse = day + "-" + month + "-" + year;
                                DateTime llapsedate = Convert.ToDateTime(careylapse).Date;
                                DateTime lupade = Convert.ToDateTime(lupdatep.UpdatedDate).Date;
                                Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();
                                int? lcaryleavebal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Count();
                                int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                                int? leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                int carrylbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == 1 || a.LeaveTypeId == 2 || a.LeaveTypeId == 3).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                //empleavebalance
                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lEmpId, lcontrolstatus, lcontrolvalue);
                                LeaveHelper.SendEmails(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.LeaveType, lcontrolling.LeaveDays, lcontrolling.Reason, lcontrolstatus, lcontrolvalue);
                                TempData["cancel"] = "Leave Cancelled Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            else if (lstauts == "Forwarded" && lleaveTypecode == "LOP")
                            {
                                Leaves lSancationing = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string llSancationingstatus = "Cancelled";
                                string llSancationingvalue = "1";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Cancelled";
                                lupdatep.Stage = lstauts;
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year== DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();

                                //int TotalDays = 0;

                                //if (lEmpLeaveBalancetotal==0)
                                //{
                                //    TotalDays = 0;
                                //}
                                //else
                                //{
                                   int TotalDays = lEmpLeaveBalancetotal - lLeaveDaystotal;
                                //}

                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, llSancationingstatus, llSancationingvalue);
                                LeaveHelper.SendEmails(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.LeaveType, lSancationing.LeaveDays, lSancationing.Reason, llSancationingstatus, llSancationingvalue);
                                TempData["cancel"] = "Leave Cancelled Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            else if (lstauts == "Forwarded" && lleaveTypecode == "W-Off")
                            {
                                Leaves lSancationing = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string llSancationingstatus = "Cancelled";
                                string llSancationingvalue = "1";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Cancelled";
                                lupdatep.Stage = lstauts;
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();

                                //int TotalDays = 0;

                                //if (lEmpLeaveBalancetotal==0)
                                //{
                                //    TotalDays = 0;
                                //}
                                //else
                                //{
                                int TotalDays = lEmpLeaveBalancetotal - lLeaveDaystotal;
                                //}

                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, llSancationingstatus, llSancationingvalue);
                                LeaveHelper.SendEmails(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.LeaveType, lSancationing.LeaveDays, lSancationing.Reason, llSancationingstatus, llSancationingvalue);
                                TempData["cancel"] = "Leave Cancelled Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            else if (lstauts == "Forwarded" && lleaveTypecode != "LOP" && lleaveTypecode != "W-Off")
                            {
                                Leaves lSancationing = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string llSancationingstatus = "Cancelled";
                                string llSancationingvalue = "1";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Cancelled";
                                lupdatep.Stage = lstauts;
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();

                                int TotalDays = lEmpLeaveBalancetotal + lLeaveDaystotal;
                                // leaves carryforward

                                string day = "15";
                                string month = "mar";
                                string year = DateTime.Now.Year.ToString();
                                string careylapse = day + "-" + month + "-" + year;
                                DateTime llapsedate = Convert.ToDateTime(careylapse).Date;
                                DateTime lupade = Convert.ToDateTime(lupdatep.UpdatedDate).Date;
                                Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();
                                int? lcaryleavebal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).Count();
                                int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                                int? leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Select(a => a.LeaveBalance).FirstOrDefault();
                                int carrylbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                //empleavebalance
                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, llSancationingstatus, llSancationingvalue);
                                LeaveHelper.SendEmails(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.LeaveType, lSancationing.LeaveDays, lSancationing.Reason, llSancationingstatus, llSancationingvalue);
                                TempData["cancel"] = "Leave Cancelled Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }

                            }
                            k++;
                            j++;
                            break;
                        }

                        break;

                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            string id = "";
            return Json(id = "jjj", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Deny(string EmployeeCodey, string leaveTypes, string LeaveIds, string Reason)
        {
            Timesheet_Request_Form ltform = new Timesheet_Request_Form();
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var lEmpBalance = db.EmpLeaveBalance.Where(a => a.Year == DateTime.Now.Year).ToList();
                var ldbresult = db.Leaves.ToList();
                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeCodey))
                {
                    lCode = EmployeeCodey.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                List<string> lTypes = new List<string>();
                if (!string.IsNullOrEmpty(leaveTypes))
                {
                    lTypes = leaveTypes.Split(new char[] { ',' }).ToList();
                    lTypes.Remove("");
                }
                List<string> lLeaveIds = new List<string>();
                if (!string.IsNullOrEmpty(LeaveIds))
                {
                    lLeaveIds = LeaveIds.Split(new char[] { ',' }).ToList();
                    lLeaveIds.Remove("");
                }
                int j = 0;
                int k = 0;
                for (int i = 0; i < lCode.Count; i++)
                {
                    for (; j < lTypes.Count; j++)
                    {
                        for (; k < lLeaveIds.Count;)
                        {

                            string lECode = lCode[i];
                            string lType = lTypes[j];
                            string lIdss = lLeaveIds[k];
                            int lleaveTypeIds = db.LeaveTypes.Where(a => a.Code == lType).Select(a => a.Id).FirstOrDefault();
                            string lleaveTypecode = db.LeaveTypes.Where(a => a.Code == lType).Select(a => a.Code).FirstOrDefault();
                            int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                            int LeaveId = Convert.ToInt32(lIdss);
                            string lstauts = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                            if (lstauts == "Pending" && lleaveTypecode == "LOP")
                            {
                                Leaves lcontrolling = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string lcontrolstatus = "Denied";
                                string lcontrolvalue = "0";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Denied";
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                lupdatep.Stage = lstauts;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();
                                int TotalDays = lEmpLeaveBalancetotal - lLeaveDaystotal;

                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lEmpId, lcontrolstatus, lcontrolvalue);
                                LeaveHelper.SendEmails(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.LeaveType, lcontrolling.LeaveDays, lcontrolling.Reason, lcontrolstatus, lcontrolvalue);
                                TempData["Denied"] = "Leave Denied Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            else if(lstauts == "Pending" && lleaveTypecode == "W-Off")
                            {
                                Leaves lcontrolling = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string lcontrolstatus = "Denied";
                                string lcontrolvalue = "0";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Denied";
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                lupdatep.Stage = lstauts;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();
                                int TotalDays = lEmpLeaveBalancetotal - lLeaveDaystotal;

                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lEmpId, lcontrolstatus, lcontrolvalue);
                                LeaveHelper.SendEmails(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.LeaveType, lcontrolling.LeaveDays, lcontrolling.Reason, lcontrolstatus, lcontrolvalue);
                                TempData["Denied"] = "Leave Denied Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            else if (lstauts == "Pending" && lleaveTypecode != "LOP" && lleaveTypecode != "W-Off")
                            {
                                Leaves lcontrolling = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string lcontrolstatus = "Denied";
                                string lcontrolvalue = "0";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Denied";
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                lupdatep.Stage = lstauts;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();
                                int TotalDays = lEmpLeaveBalancetotal + lLeaveDaystotal;
                                // leaves carryforward

                                string day = "15";
                                string month = "mar";
                                string year = DateTime.Now.Year.ToString();
                                string careylapse = day + "-" + month + "-" + year;
                                DateTime llapsedate = Convert.ToDateTime(careylapse).Date;
                                DateTime lupade = Convert.ToDateTime(lupdatep.UpdatedDate).Date;
                                Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();
                                int? lcaryleavebal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).Count();
                                int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                                int? leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                int carrylbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                //empleavebalance
                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lEmpId, lcontrolstatus, lcontrolvalue);
                                LeaveHelper.SendEmails(lcontrolling.StartDate, lcontrolling.EndDate, lcontrolling.ControllingAuthority, lcontrolling.SanctioningAuthority, lId, lcontrolling.LeaveType, lcontrolling.LeaveDays, lcontrolling.Reason, lcontrolstatus, lcontrolvalue);
                                TempData["Denied"] = "Leave Denied Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            else if (lstauts == "Forwarded" && lleaveTypecode == "LOP")
                            {
                                Leaves lSancationing = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string llSancationingstatus = "Denied";
                                string llSancationingvalue = "1";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Denied";
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                lupdatep.Stage = lstauts;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();

                                int TotalDays = lEmpLeaveBalancetotal - lLeaveDaystotal;

                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, llSancationingstatus, llSancationingvalue);
                                LeaveHelper.SendEmails(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.LeaveType, lSancationing.LeaveDays, lSancationing.Reason, llSancationingstatus, llSancationingvalue);
                                TempData["Denied"] = "Leave Denied Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            else if (lstauts == "Forwarded" && lleaveTypecode == "W-Off")
                            {
                                Leaves lSancationing = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string llSancationingstatus = "Denied";
                                string llSancationingvalue = "1";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Denied";
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                lupdatep.Stage = lstauts;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();

                                int TotalDays = lEmpLeaveBalancetotal - lLeaveDaystotal;

                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, llSancationingstatus, llSancationingvalue);
                                LeaveHelper.SendEmails(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.LeaveType, lSancationing.LeaveDays, lSancationing.Reason, llSancationingstatus, llSancationingvalue);
                                TempData["Denied"] = "Leave Denied Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            else if (lstauts == "Forwarded" && lleaveTypecode != "LOP" && lleaveTypecode != "W-Off")
                            {
                                Leaves lSancationing = Facade.EntitiesFacade.GetLeaveTabledata.GetById(LeaveId);
                                string llSancationingstatus = "Denied";
                                string llSancationingvalue = "1";
                                Leaves lupdatep = (from l in ldbresult where l.EmpId == lId && l.LeaveType == lleaveTypeIds && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.Status = "Denied";
                                lupdatep.LeavesYear = DateTime.Now.Year;
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                lupdatep.CancelReason = Reason;
                                lupdatep.Stage = lstauts;
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();
                                int lEmpLeaveBalancetotal = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                int lLeaveDaystotal = db.Leaves.Where(a => a.EmpId == lId).Where(a => a.LeaveType == lleaveTypeIds).Where(a => a.Id == LeaveId).Select(a => a.LeaveDays).FirstOrDefault();

                                int TotalDays = lEmpLeaveBalancetotal + lLeaveDaystotal;
                                // leaves carryforward

                                string day = "15";
                                string month = "mar";
                                string year = DateTime.Now.Year.ToString();
                                string careylapse = day + "-" + month + "-" + year;
                                DateTime llapsedate = Convert.ToDateTime(careylapse).Date;
                                DateTime lupade = Convert.ToDateTime(lupdatep.UpdatedDate).Date;
                                Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();
                                int? lcaryleavebal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).Count();
                                int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                                int? leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                int carrylbalance = db.Leaves_CarryForward.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lupdatep.LeaveType).Where(a => a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();

                                //empleavebalance
                                EmpLeaveBalance lbalance = lEmpBalance.Where(a => a.EmpId == lId).Where(a => a.LeaveTypeId == lleaveTypeIds && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = lleaveTypeIds;
                                lbalance.EmpId = lId;
                                lbalance.Debits = lbalance.Debits - lLeaveDaystotal;
                                lbalance.LeaveBalance = TotalDays;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                LeaveHelper.SendSms(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, llSancationingstatus, llSancationingvalue);
                                LeaveHelper.SendEmails(lSancationing.StartDate, lSancationing.EndDate, lSancationing.ControllingAuthority, lSancationing.SanctioningAuthority, lId, lSancationing.LeaveType, lSancationing.LeaveDays, lSancationing.Reason, llSancationingstatus, llSancationingvalue);
                                TempData["Denied"] = "Leave Denied Successfully";
                                if (lupdatep.EndDate <= DateTime.Now.Date)
                                {
                                    string lcode = db.LeaveTypes.Where(a => a.Id == lupdatep.LeaveType).Select(a => a.Code).FirstOrDefault();
                                    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();

                                    ltform.UserId = lId;
                                    ltform.BranchId = (int)lupdatep.BranchId;
                                    ltform.DepartmentId = (int)lupdatep.DepartmentId;
                                    ltform.DesignationId = (int)lupdatep.DesignationId;
                                    ltform.Shift_Id = (int)shiftids;
                                    ltform.Reason_Type = lcode;
                                    ltform.Reason_Desc = "Leave";
                                    ltform.ReqFromDate = lupdatep.StartDate;
                                    ltform.ReqToDate = lupdatep.EndDate;
                                    ltform.CA = lupdatep.ControllingAuthority;
                                    ltform.SA = lupdatep.SanctioningAuthority;
                                    ltform.Status = lupdatep.Status;
                                    ltform.UpdatedBy = lupdatep.UpdatedBy;
                                    ltform.UpdatedDate = lupdatep.UpdatedDate;
                                    ltform.Processed = 2;
                                    //db.Timesheet_Request_Form.Add(ltform);
                                    db.SaveChanges();
                                }
                            }
                            k++;
                            j++;
                            break;
                        }

                        break;

                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            string id = "";
            return Json(id = "jjj", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult LeaveApprovalSearch(FormCollection formValues)
        {

            string lMessage = string.Empty;
            try
            {

                string Leavetype = formValues["LeaveType"];
                string AppliedDate = formValues["ADate"];
                string Branch = formValues["Branch"];
                string Department = formValues["Department"];
                string Empid = formValues["EmployeeCode"];
                string radiobuttonvalue = formValues["radio"];
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                var lleaveHistory = db.V_LeaveHistory.ToList();
                var lleaveTypes = db.LeaveTypes.ToList();
                var lEmployees = db.Employes.ToList();
                var lleaves = db.Leaves.ToList();
                var lBranches = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lControllingAuthority = db.Leaves.Where(a => a.ControllingAuthority == lEmpId).Select(a => a.ControllingAuthority).FirstOrDefault();
                int lSancationingAuthority = db.Leaves.Where(a => a.SanctioningAuthority == lEmpId).Select(a => a.SanctioningAuthority).FirstOrDefault();
                var lleaveBalance = db.V_EmpLeaveBalance.ToList();
                //string[] LeavesSanctioning = ConfigurationManager.AppSettings["LeavesSanctioning"].Split(',');
                string[] LeavesSanctionings = ConfigurationManager.AppSettings["LeavesSanctioning"].Split(',');
                List<string> list = LeavesSanctionings.ToList();

                if (ConfigurationManager.AppSettings["LeavesSanctioning"].Split(',').Contains(lCredentials.EmpId) )

                {
                    //if (lCredentials.EmpId == LeavesSanctioning[0] || lCredentials.EmpId == LeavesSanctioning[1])
                    //{
                    lControllingAuthority = 220;
                    lSancationingAuthority = 220;
                    lEmpId = 220;
                    if (lEmpId == lControllingAuthority)
                    {
                        if (radiobuttonvalue == "Branch")
                        {
                            if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch)
                                               where leave.DepartmentId == 46
                                               //where employee.Department == Convert.ToInt32(Department)
                                               //where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.LeaveType.ToString() == Leavetype
                                               where employee.Branch == Convert.ToInt32(Branch)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.LeaveType.ToString() == Leavetype
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }






                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }


                            else if (Leavetype == "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }



                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                    employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where employee.EmpId == Empid
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                     employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }



                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                   employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Empid == "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Branch != 43
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                          employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch != 43
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch != 43
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Branch != 43
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Branch != 43
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Branch != 43
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {

                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {

                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                           employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               // where leave.DepartmentId == 46
                                               where leave.LeaveType.ToString() == Leavetype
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }




                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Branch != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid

                                               //where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }





                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Branch != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               //where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (radiobuttonvalue == "HeadOffice")
                        {
                            if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               select new
                                               {
                                                   leave.Id,

                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status

                                               }).OrderByDescending(A => A.AppliedDate);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            //

                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               select new
                                               {
                                                   leave.Id,

                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status

                                               }).OrderByDescending(A => A.AppliedDate);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               //  where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               // where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               // where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where employee.EmpId == Empid
                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id

                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               // where leave.LeaveType.ToString() == Leavetype
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               // where leave.LeaveType.ToString() == Leavetype
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where employee.EmpId == Empid
                                               //where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                            employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               // where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               where employee.Department != 42
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               // where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department != 42
                                               where employee.EmpId == Empid

                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               where employee.Branch == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate) || employee.Branch_Value1 == Branch
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Empid == "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               // where employee.EmpId == Empid
                                               where employee.Department != 42
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               //where employee.Department != 42
                                               //where leave.BranchId == 43
                                               where employee.EmpId == Empid
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department != 42
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department != 46
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Branch == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department != 46
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department != 46
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department != 46
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                             employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.BranchId == 43 && employee.EmpId == Empid && leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate) && leave.LeaveType.ToString() == Leavetype && employee.Department != 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                   employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                   employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                    employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch == "" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               // where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               select new
                                               {
                                                   leave.Id,

                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status

                                               }).OrderByDescending(A => A.AppliedDate);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                            }
                        }


                    }
                }
                else
                {
                    if (lEmpId == lControllingAuthority)
                    {
                        if (radiobuttonvalue == "Branch")
                        {
                            if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch)
                                               where leave.DepartmentId == 46
                                               //where employee.Department == Convert.ToInt32(Department)
                                               //where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.LeaveType.ToString() == Leavetype
                                               where employee.Branch == Convert.ToInt32(Branch)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.LeaveType.ToString() == Leavetype
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }






                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }


                            else if (Leavetype == "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }



                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                    employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where employee.EmpId == Empid
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                     employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }



                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                   employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Empid == "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Branch != 43
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                          employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch != 43
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch != 43
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Branch != 43
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Branch != 43
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Branch != 43
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {

                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {

                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                           employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               // where leave.DepartmentId == 46
                                               where leave.LeaveType.ToString() == Leavetype
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }




                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Branch != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid

                                               //where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }





                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Branch != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               //where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (radiobuttonvalue == "HeadOffice")
                        {
                            if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               select new
                                               {
                                                   leave.Id,

                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status

                                               }).OrderByDescending(A => A.AppliedDate);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            //

                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               select new
                                               {
                                                   leave.Id,

                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status

                                               }).OrderByDescending(A => A.AppliedDate);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               //  where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               // where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               // where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where employee.EmpId == Empid
                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id

                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               // where leave.LeaveType.ToString() == Leavetype
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               // where leave.LeaveType.ToString() == Leavetype
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where employee.EmpId == Empid
                                               //where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                  employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                            employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               // where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               where employee.Department != 42
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               // where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department != 42
                                               where employee.EmpId == Empid

                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.EmpId == Empid
                                               where employee.Branch == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                 employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate) || employee.Branch_Value1 == Branch
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Empid == "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               // where employee.EmpId == Empid
                                               where employee.Department != 42
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               //where employee.Department != 42
                                               //where leave.BranchId == 43
                                               where employee.EmpId == Empid
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department != 42
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department != 46
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Branch == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department != 46
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department != 46
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where employee.Department != 46
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                             employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.BranchId == 43 && employee.EmpId == Empid && leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate) && leave.LeaveType.ToString() == Leavetype && employee.Department != 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                   employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                   employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                    employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch == "" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                              employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0

                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               // where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where employee.ControllingAuthority == lControllingAuthority.ToString() && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                employee.SanctioningAuthority == lSancationingAuthority.ToString() && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               select new
                                               {
                                                   leave.Id,

                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status

                                               }).OrderByDescending(A => A.AppliedDate);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                            }
                        }


                    }
                    else if (lEmpId == lSancationingAuthority)
                    {
                        if (radiobuttonvalue == "Branch")
                        {
                            if (Leavetype != "" && Empid == "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Empid == "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Empid != "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Empid == "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Empid != "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where employee.EmpId == Empid
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where employee.Department == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Empid != "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Empid != "" && AppliedDate == "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where employee.EmpId == Empid
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.Department == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Empid != "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Branch != "" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate == "" && Branch == "-1")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch != 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch != 43
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch != 43
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch != 43
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch != 43
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Branch != 43
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Branch == "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Branch != "" && Branch != "-1" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Branch == Convert.ToInt32(Branch) || employee.Branch_Value1 == Branch
                                               where leave.DepartmentId == 46
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (radiobuttonvalue == "HeadOffice")
                        {
                            if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }



                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department != "" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.BranchId == 43 &&
                                                employee.Department == Convert.ToInt32(Department)
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department != "" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.BranchId == 43 && leave.LeaveType.ToString() == Leavetype &&
                                                employee.Department == Convert.ToInt32(Department)
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.BranchId == 43 && leave.LeaveType.ToString() == Leavetype && leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)

                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.BranchId == 43 && leave.LeaveType.ToString() == Leavetype && leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)

                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.BranchId == 43 && leave.LeaveType.ToString() == Leavetype && leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)

                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid && leave.LeaveType.ToString() == Leavetype && leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.Department == Convert.ToInt32(Department)
                                               where employee.Branch == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.LeaveType.ToString() == Leavetype
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               // where leave.LeaveType.ToString() == Leavetype
                                               where employee.Department == Convert.ToInt32(Department)
                                               where employee.Branch == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.Branch == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.Branch == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate) || employee.Branch_Value1 == Branch
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }

                            else if (Leavetype == "" && Empid == "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department != 42
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department != 42
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype == "" && Empid == "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department != 46
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid == "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                               leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department != 46
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.LeaveType.ToString() == Leavetype
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate == "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department != 46
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where employee.Department != 46
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department == "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid == "" && AppliedDate != "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate == "" && Department != "" && Department != "-1" && Branch == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               //where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate != "" && Department != "" && Department == "-1" && Branch == "")
                            {
                                return null;
                            }
                            else if (Leavetype == "" && Empid != "" && AppliedDate != "" && Branch == "-1" && Department == "")
                            {
                                return null;
                            }
                            else if (Leavetype != "" && Leavetype == "11" && Empid != "" && AppliedDate != "" && Branch == "" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else if (Leavetype != "" && Leavetype != "11" && Empid != "" && AppliedDate != "" && Branch == "" && Department == "")
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.ControllingAuthority == lControllingAuthority && leave.Status == "Pending" && leave.LeaveDays != 0 ||
                                                leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded" && leave.LeaveDays != 0
                                               where leave.UpdatedDate.Date == Convert.ToDateTime(AppliedDate)
                                               //where employee.Department == Convert.ToInt32(Department)
                                               where employee.EmpId == Empid
                                               where employee.Branch == 43
                                               where leave.LeaveType.ToString() == Leavetype
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var lResult = (from employee in lEmployees
                                               join leave in lleaves on employee.Id equals leave.EmpId
                                               join branch in lBranches on leave.BranchId equals branch.Id
                                               join dept in ldept on leave.DepartmentId equals dept.Id

                                               join leavetypes in lleaveTypes on leave.LeaveType equals leavetypes.Id
                                               join desig in ldesignation on leave.DesignationId equals desig.Id
                                               where leave.SanctioningAuthority == lSancationingAuthority && leave.Status == "Forwarded"
                                               where leave.BranchId == 43
                                               select new
                                               {
                                                   leave.Id,
                                                   employee.EmpId,
                                                   employee.ShortName,
                                                   designation = desig.Code,
                                                   AppliedDate = leave.UpdatedDate.ToShortDateString(),
                                                   Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                                   leave.StartDate,
                                                   leave.EndDate,
                                                   leave.LeaveDays,
                                                   leavetypes.Code,
                                                   leave.Reason,
                                                   leave.Status,
                                                   leave.UpdatedDate
                                               }).OrderByDescending(A => A.UpdatedDate.Date);
                                return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }
    }
}