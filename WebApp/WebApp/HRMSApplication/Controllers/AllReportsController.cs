using HRMSApplication.Helpers;
using HRMSApplication.Models;
using HRMSBusiness.Reports;
using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities;
using HRMSBusiness.Db;
using System.Data;
using HRMSBusiness.Timesheet;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using ClosedXML.Excel;
//using System.Web.Services;

namespace HRMSApplication.Controllers
{
    //  [NoDirectAccess]
    [Authorize]
    public class AllReportsController : Controller
    {
        SqlHelper sh = new SqlHelper();
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        ReportBusiness Rbus = new ReportBusiness();
        TimesheetBusiness Tbus = new TimesheetBusiness();

        public ActionResult Index()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            //Executive role redirection to workdiary report
            if (lCredentials.Role == "2" && (lCredentials.Designation == "1" || lCredentials.Designation == "3" || lCredentials.Designation == "4"))
            {
                return RedirectToAction("WorkDiary");

            }
            //else if (lCredentials.Role == "2" || lCredentials.Role == "3" || lCredentials.Role == "4")
            //{
            //    return RedirectToAction("LateArrivalsReport");

            //}
            else if (lCredentials.Role == "2" || lCredentials.Role == "3" || lCredentials.Role == "4")
            {
                return RedirectToAction("LateArrivalsReport");
            }
            else
            {
                // filter 1: Branch names dropdown   
                ViewBag.DdlOneLabel = "Branch";
                ViewBag.SearchBtn = "false";

                ViewBag.ReportFilters = "branchddlOne";


                var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Branches Where Name!='OtherBranch' and Name!= 'TGCAB-CTI'");
                var items = dt.AsEnumerable().Select(r => new Branches
                {
                    Id = (Int32)(r["Id"]),
                    Name = (string)(r["Name"] ?? "null")
                }).ToList();

                items.Insert(0, new Branches
                {
                    Id = 0,
                    Name = "All"
                });

                ViewBag.DdlOneData = new SelectList(items, "Id", "Name");

                ViewBag.ReportTitle = "Branches List";

                ViewBag.DataUrl = "/AllReports/BranchData?branchid=-1&empid=-2";
                ViewBag.ReportColumns = @"[{""title"": ""Emp Id"", ""data"": ""EmpId"",  ""autoWidth"": true },{ ""title"": ""Emp Name"",""data"": ""ShortName"", ""autoWidth"": true },
                { ""title"": ""Designation"",""data"": ""desg"",  ""autoWidth"": true },
                { ""title"": ""Branch"",""data"": ""BranchName"", ""autoWidth"": true }]";

                return View("~/Views/AllReports/AllReports.cshtml");
            }
        }
        [HttpGet]
        public string BranchData(int branchid, string empid)
        {
            return JsonConvert.SerializeObject(Rbus.Brancheslist(branchid, empid));
        }

        public ActionResult BranchContacts()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";


            ViewBag.ReportFilters = "none";


            ViewBag.ReportTitle = "Branch Contact List";
            ViewBag.DataUrl = "/AllReports/BranchContactsData";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Id"",""data"": ""EmpId"",  ""autoWidth"": true },{""title"": ""Emp Name"", ""data"": ""EmpName"", ""autoWidth"": true },{""title"": ""Branch"",""data"": ""BranchName"", ""autoWidth"": true },{""title"": ""Designation"",""data"": ""Code"",  ""autoWidth"": true },{""title"": ""Extensions"",""data"": ""PhoneNo1"", ""autoWidth"": true },{""title"": ""Mobile Number"",""data"": ""PhoneNo2"", ""autoWidth"": true },{""title"": ""Time"",""data"": ""Time"",  ""autoWidth"": true }]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }


        public ActionResult MissingTimesheet()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.DdlOneLabel = "Branch";
            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "branchddlTwo";


            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Branches Where Name!='OtherBranch' and Name!= 'TGCAB-CTI'");  // Where Name!='OtherBranch'
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            //items.Insert(0, new Branches
            //{
            //    Id = 0,
            //    Name = "All"
            //});
            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "DCCB_Employees"
            });

            ViewBag.DdlOneData = new SelectList(items, "Id", "Name");


            ViewBag.ReportTitle = "Missing Timesheet List";
            ViewBag.DataUrl = "/AllReports/MissingTimesheetData?branchid=-1&Date=-2";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Id"",""data"": ""Empid"",  ""autoWidth"": true },
                                       {""title"": ""Emp Name"",""data"": ""ShortName"",  ""autoWidth"": true },
                                       {""title"": ""Branch"",""data"": ""name"",  ""autoWidth"": true },
                                       {""title"": ""Status"",""data"": ""Status"",  ""autoWidth"": true },
                                       {""title"": ""Date"",""data"": ""Date"",  ""autoWidth"": true },
                                       {""title"": ""Shift"",""data"": ""Shift"",  ""autoWidth"": true }]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string MissingTimesheetData(int branchid, string Date)
        {
            if (Date == "-2")
            {
                Date = "";
            }
            return JsonConvert.SerializeObject(Rbus.getMissingTimesheetList(branchid, Date));

        }
        [HttpGet]
        public string BranchContactsData()
        {
            var dt = Rbus.getBranchContactsList();
            return JsonConvert.SerializeObject(dt);
        }

        public ActionResult WorkDiary()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";
            ViewBag.ReportFilters = "workdiaryGrouping";
            ViewBag.ReportTitle = "Work Diary";
            //ViewBag.ExportColumns = "columns: [0,1,2,3,4,5,6,7,8,9,10,11,13,14,15,16,17]";
            ViewBag.PdfSize = "landscape";


            ViewBag.DataUrl = "/AllReports/WorkDairySearchData?empid=&StartDate=&fromDate&toDate=&status=-1";
            ViewBag.ReportColumns = @"[{""data"": ""empid"", ""title"": ""Employee Code"", ""autoWidth"": true },
                                      {""data"": ""Shortname"", ""title"": ""Employee Name"", ""autoWidth"": true },
                                      {""data"": ""designation"", ""title"": ""Designation"", ""autoWidth"": true },
                                      {""data"": ""branch"", ""title"": ""Branch/Department"", ""autoWidth"": true },
                                      {""data"": ""WorkDate"",""title"": ""Work Date"",""autoWidth"": true },
                                      { ""data"": ""AppliedDate"",""title"": ""Applied Date"",""autoWidth"": true},
                                      {""data"": ""WorkDescription"", ""title"": ""Work Description"", ""autoWidth"": true },
                                      {""data"": ""Status"", ""title"": ""Status"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string WorkDairySearchData(string empid, string StartDate, string fromDate, string toDate, string status)
        {
            string eid = empid != null && empid != "" ? empid : "";
            //string dt = WdDate != null && WdDate != "" ? WdDate : "";
            string fromdt = fromDate != null && fromDate != "" ? fromDate : "";
            string todt = toDate != null && toDate != "" ? toDate : "";
            string lstatus = status != null && status != "" ? status : "";
            // var data = Rbus.getallWorkdairiessearch(StartDate, fromdt, todt, eid, lstatus);
            if (status == "ALL" && empid == "")
            {
                return JsonConvert.SerializeObject(Rbus.allWDNew(StartDate, fromdt, todt, eid, lstatus));
            }
            else
            {
                return JsonConvert.SerializeObject(Rbus.getallWorkdairiessearch(StartDate, fromdt, todt, eid, lstatus));
            }
        }
        //TimeSheetMonth

        public ActionResult TimeSheetMonth(string branch, string fromdate, string todate, string empcode)
        {
            string lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            ViewBag.EmpId = lCredentials.EmpId;
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Employee Timesheet";
            ViewBag.ReportFilters = "TimeSheetMonthGrouping";
            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!='OtherBranch' and Name!= 'TGCAB-CTI'   UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });
            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            if (lCredentials.Branch == "43")
            {
                //var selected4 = (from sub in db.Employes
                //                 where sub.EmpId == lCredentials.EmpId
                //                 select sub.Department).First();

                //ViewBag.DdlOneData1 = new SelectList(items1, "Id", "Name", selected4);
                var deptId = db.Employes
   .Where(sub => sub.EmpId == lCredentials.EmpId)
   .Select(sub => sub.Department)
   .FirstOrDefault();

                var selectedDept = db.Departments
                    .Where(d => d.Id == deptId)
                    .Select(d => d.Name)
                    .FirstOrDefault();
                string formattedSelectedDept = "-" + selectedDept;
                //var selected4 = (from sub in db.Employes
                //                 where sub.EmpId == lCredentials.EmpId
                //                 select sub.Department).First();
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", formattedSelectedDept);

            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
            }

            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");



            if (branch == string.Empty && fromdate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/TimeSheetMonthReportdata";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/TimeSheetMonthReportdata?branch=" + branch + "&fromdate=" + fromdate + "&todate=" + todate + "&empcode=" + empcode;
            }



            ViewBag.ReportColumns = @"[{""data"": ""grpcol"", ""title"": ""Employee Information"", ""autoWidth"": true },
                                   { ""data"": ""Date"",""title"": ""Date"",""autoWidth"": true},
                                   {""title"": ""Shift Start Time"",""data"": ""BranchStartTime"",  ""autoWidth"": true },
            {""title"": ""Emp. In Time"",""data"": ""EmpCheckInTime"",  ""autoWidth"": true },
            {""title"": ""Late by(HH:MM)"",""data"": ""LateBy"",  ""autoWidth"": true },     
            {""title"": ""Shift Out Time"",""data"": ""BranchCheckOutTime"",  ""autoWidth"": true },
            {""title"": ""Emp. Out Time"",""data"": ""EmpCheckOutTime"",  ""autoWidth"": true },            
            {""title"": ""Early by(HH:MM)"",""data"": ""EarlyBy"",  ""autoWidth"": true }           
            ]";

            return View("~/Views/AllReports/TimeSheetMonthGroupings.cshtml");
        }

        [HttpGet]
        public string TimeSheetMonthReportdata(string branch, string fromdate, string todate, string empcode, string self)
        {
            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);

            //if (branch.Contains("-") && branch != "HeadOffice-All")
            //{
            //    branch = branch.Substring(1);
            //}

            //branch = branch.Trim();

            //if (branch.Contains("and"))
            //{
            //    branch = branch.Replace("and", "&");
            //}

            var dt = Tbus.EmpTimesheetMstdata(branch, fromdate, todate, empcode, self, EmpIds);
            return JsonConvert.SerializeObject(dt);
        }
        //Earlyby

        public ActionResult Earlyby(string branch, string fromdate, string todate, string empcode)
        {


            string lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            ViewBag.EmpId = lCredentials.EmpId;
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Early Departure";
            ViewBag.ReportFilters = "EarlyByGrouping";
            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!='OtherBranch'  and Name!= 'TGCAB-CTI'  UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });
            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            if (lCredentials.Branch == "43")
            {
                //var selected4 = (from sub in db.Employes
                //                 where sub.EmpId == lCredentials.EmpId
                //                 select sub.Department).First();

                //ViewBag.DdlOneData1 = new SelectList(items1, "Id", "Name", selected4);
                var deptId = db.Employes
   .Where(sub => sub.EmpId == lCredentials.EmpId)
   .Select(sub => sub.Department)
   .FirstOrDefault();

                var selectedDept = db.Departments
                    .Where(d => d.Id == deptId)
                    .Select(d => d.Name)
                    .FirstOrDefault();
                string formattedSelectedDept = "-" + selectedDept;
                //var selected4 = (from sub in db.Employes
                //                 where sub.EmpId == lCredentials.EmpId
                //                 select sub.Department).First();
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", formattedSelectedDept);

            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
            }

            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");



            if (branch == string.Empty && fromdate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/EarlybyReportdata";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/EarlybyReportdata?branch=" + branch + "&fromdate=" + fromdate + "&todate=" + todate + "&empcode=" + empcode;
            }



            ViewBag.ReportColumns = @"[{""data"": ""grpcol"", ""title"": ""Employee Information"", ""autoWidth"": true },
                                   { ""data"": ""Date"",""title"": ""Date"",""autoWidth"": true},
                                   {""data"": ""BranchEndTime"", ""title"": ""BranchEndTime"", ""autoWidth"": true },
         
                                   { ""data"": ""EmpCheckOut"", ""title"": ""EmpCheckOutTime"", ""autoWidth"": true },
                                   { ""data"": ""EarlyBy"", ""title"": ""EarlyBy(HH:MM)"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/EarlybyGroupings.cshtml");
        }

        [HttpGet]
        public string EarlybyReportdata(string branch, string fromdate, string todate, string empcode, string self)
        {
            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);

            //if (branch.Contains("-") && branch != "HeadOffice-All")
            //{
            //    branch = branch.Substring(1);
            //}

            //branch = branch.Trim();

            //if (branch.Contains("and"))
            //{
            //    branch = branch.Replace("and", "&");
            //}

            var dt = Tbus.EarlyByTimesheetMstdata(branch, fromdate, todate, empcode, self, EmpIds);
            return JsonConvert.SerializeObject(dt);
        }
        //Lateby

        public ActionResult Lateby(string branch, string fromdate, string todate, string empcode)
        {


            string lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            ViewBag.EmpId = lCredentials.EmpId;
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Late Arrivals";
            ViewBag.ReportFilters = "LateByGrouping";
            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!='OtherBranch' and Name!= 'TGCAB-CTI'   UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });
            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            if (lCredentials.Branch == "43")
            {
                var deptId = db.Employes
    .Where(sub => sub.EmpId == lCredentials.EmpId)
    .Select(sub => sub.Department)
    .FirstOrDefault();

                var selectedDept = db.Departments
                    .Where(d => d.Id == deptId)
                    .Select(d => d.Name)
                    .FirstOrDefault();
                string formattedSelectedDept = "-" + selectedDept;
                //var selected4 = (from sub in db.Employes
                //                 where sub.EmpId == lCredentials.EmpId
                //                 select sub.Department).First();
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", formattedSelectedDept);
                //ViewBag.DdlOneData1 = new SelectList(items1, "Id", "Name", selected4);

            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
            }

            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");



            if (branch == string.Empty && fromdate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/LatebyReportdata";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/LatebyReportdata?branch=" + branch + "&fromdate=" + fromdate + "&todate=" + todate + "&empcode=" + empcode;
            }



            ViewBag.ReportColumns = @"[{""data"": ""grpcol"", ""title"": ""Employee Information"", ""autoWidth"": true },
                                   { ""data"": ""Date"",""title"": ""Date"",""autoWidth"": true},
                                   {""data"": ""BranchStartTime"", ""title"": ""BranchStartTime"", ""autoWidth"": true },
         
                                   { ""data"": ""EmpCheckInTime"", ""title"": ""EmpCheckInTime"", ""autoWidth"": true },
                                   { ""data"": ""LateBy"", ""title"": ""LateBy(HH:MM)"", ""autoWidth"": true },
                                     { ""data"": ""LateafterGracePeriod"", ""title"": ""Late After GracePeriod"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/LatebyGroupings.cshtml");
        }

        [HttpGet]
        public string LatebyReportdata(string branch, string fromdate, string todate, string empcode, string self)
        {
            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);

            //if (branch.Contains("-") && branch != "HeadOffice-All")
            //{
            //    branch = branch.Substring(1);
            //}

            //branch = branch.Trim();

            //if (branch.Contains("and"))
            //{
            //    branch = branch.Replace("and", "&");
            //}

            var dt = Tbus.LateByTimesheetMstdata(branch, fromdate, todate, empcode, self, EmpIds);
            return JsonConvert.SerializeObject(dt);
        }
        public ActionResult LeavesCreditDebit()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "none";
            ViewBag.ReportFiltersTwo = "none";
            ViewBag.ReportFiltersThree = "none";

            ViewBag.ReportTitle = "Year Wise Leave Balance";
            ViewBag.DataUrl = "/AllReports/CreditDebitLeaveView";
            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";
            ViewBag.ExportColumns = "columns: [0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32,33,34,35,36,37]";

            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"",""data"": ""EmpId"", ""autoWidth"": true }, {""title"": ""Emp Name"",""data"": ""EmpName"",  ""autoWidth"": true }, {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
            {""title"": ""Department/ Branch"", ""data"": ""BrDept"",  ""autoWidth"": true }, {""title"": ""Year"",""data"": ""Year"", ""autoWidth"": true },
            {""title"": ""CL CF"",""data"": ""CarryForwardFLeaves"",  ""autoWidth"": true },
            {""title"": ""CL LC"", ""data"": ""CreditLeaves"", ""autoWidth"": true },
            {""title"": ""CL LD"",""data"": ""Debits"", ""autoWidth"": true },
            {""title"": ""CL LB"",""data"": ""LeaveBalance"",""autoWidth"": true }, 

            {""title"": ""ML CF"",""data"": ""CarryForwardML"",  ""autoWidth"": true },
            {""title"": ""ML LC"", ""data"": ""ConsumedML"", ""autoWidth"": true },            
            {""title"": ""ML LD"",""data"": ""RemainingML"",  ""autoWidth"": true },
            {""title"": ""ML LB"",""data"": ""TotalMedicalSickLeave"", ""autoWidth"": true },     

            {""title"": ""PL CF"",""data"": ""TotalPrivilegeLeave"", ""autoWidth"": true }, 
            {""title"": ""PL LC"", ""data"": ""ConsumedPL"",  ""autoWidth"": true }, 
            {""title"": ""PL LD"",""data"": ""RemainingPL"",  ""autoWidth"": true }, 
            {""title"": ""PL LB"",""data"": ""CarryForwardPL"", ""autoWidth"": true }, 

            {""title"": ""MTL CF"",""data"": ""TotalMaternityLeave"",  ""autoWidth"": true },
            {""title"": ""MTL LC"", ""data"": ""ConsumedMTL"", ""autoWidth"": true },
            {""title"": ""MTL LD"",""data"": ""RemainingMTL"",  ""autoWidth"": true }, 
            {""title"": ""MTL LB"",""data"": ""CarryForwardMTL"", ""autoWidth"": true }, 
            {""title"": ""PTL CF"",""data"": ""TotalPaternityLeave"",  ""autoWidth"": true },
            {""title"": ""PTL LC"", ""data"": ""ConsumedPTL"", ""autoWidth"": true }, 
            {""title"": ""PTL LD"",""data"": ""RemainingPTL"",  ""autoWidth"": true }, 
            {""title"": ""PTL LB"",""data"": ""CarryForwardPTL"",""autoWidth"": true }, 

            {""title"": ""EOL CF"",""data"": ""TotalExtraordinaryLeave"", ""autoWidth"": true },
            { ""title"": ""EOL LC"", ""data"": ""ConsumedEOL"", ""autoWidth"": true },
            {""title"": ""EOL LD"",""data"": ""RemainingEOL"",  ""autoWidth"": true }, 
            {""title"": ""EOL LB"",""data"": ""CarryForwardEOL"", ""autoWidth"": true }, 

            { ""title"": ""SCL CF"",""data"": ""TotalSpecialCasualLeave"",  ""autoWidth"": true },
            { ""title"": ""SCL LC"", ""data"": ""ConsumedSCL"",  ""autoWidth"": true },
            { ""title"": ""SCL LD"",""data"": ""RemainingSCL"", ""autoWidth"": true }, 
            { ""title"": ""SCL LB"",""data"": ""CarryForwardSCL"", ""autoWidth"": true },


            { ""title"": ""C-OFF CF"",""data"": ""TotalCOFFLeave"",  ""autoWidth"": true },
            { ""title"": ""C-OFF LC"", ""data"": ""ConsumedCOFF"",  ""autoWidth"": true },
            { ""title"": ""C-OFF LD"",""data"": ""RemainingCOFF"", ""autoWidth"": true }, 
            { ""title"": ""C-OFF LB"",""data"": ""CarryForwardCOFF"", ""autoWidth"": true },

            //{ ""title"": ""LOP CF"",""data"": ""TotalLOPLeave"",  ""autoWidth"": true },
            //{ ""title"": ""LOP LC"", ""data"": ""ConsumedLOP"",  ""autoWidth"": true },
            //{ ""title"": ""LOP LD"",""data"": ""RemainingLOP"", ""autoWidth"": true }, 
            { ""title"": ""LOP LB"",""data"": ""CarryForwardLOP"", ""autoWidth"": true }]";


            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string CreditDebitLeaveView(string EmpId)
        {
            return JsonConvert.SerializeObject(Rbus.getEmpLeaveReport(EmpId));

        }


        public ActionResult CadreList()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "true";

            // filter 1: Branch names dropdown
            ViewBag.DdlOneLabel = "Designation";

            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Designations");
            var items = dt.AsEnumerable().Select(r => new Designations
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Designations
            {
                Id = 0,
                Name = "All"
            });
            ViewBag.DdlOneData = new SelectList(items, "Id", "Name");

            ViewBag.ReportFilters = "ddlOne";
            ViewBag.ReportFiltersTwo = "none";
            ViewBag.ReportFiltersThree = "none";

            ViewBag.ReportTitle = "Cadre List";
            ViewBag.DataUrl = "/AllReports/CadreListData?desiId=-1";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Id"", ""data"": ""EmpId"", ""autoWidth"": true },{""title"": ""Emp Name"", ""data"": ""EmpName"", ""autoWidth"": true },{ ""title"": ""Designation"",""data"": ""Designations"",  ""autoWidth"": true },{ ""title"": ""Department/Branch"",""data"": ""BranchDepartmet"", ""autoWidth"": true },{ ""title"": ""Joining Date"",""data"": ""DOJ"", ""autoWidth"": true },{ ""title"": ""Retirement Date"",""data"": ""RetirementDate"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string CadreListData(int desiId)
        {
            int a = desiId;
            return JsonConvert.SerializeObject(Rbus.getAllCadreList(desiId));
        }

        public ActionResult EmpDob()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "true";

            // filter 1: Branch names dropdown
            ViewBag.DdlOneLabel = "Month";
            string[] array = { "All", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec" };
            var items = array.Select((x, Index) => new { Name = x, Id = Index });

            ViewBag.DdlOneData = new SelectList(items, "Id", "Name");

            ViewBag.ReportFilters = "ddlOne";
            ViewBag.ReportFiltersTwo = "none";
            ViewBag.ReportFiltersThree = "none";

            ViewBag.ReportTitle = "DOB List";
            ViewBag.DataUrl = "/AllReports/EmpDobList?dobmid=-1";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },{""title"": ""Emp Name"", ""data"": ""EmpName"", ""autoWidth"": true },{ ""title"": ""Designation"",""data"": ""Designations"",  ""autoWidth"": true },{ ""title"": ""DOB"",""data"": ""DOB"", ""autoWidth"": true },{ ""title"": ""Birth Year"",""data"": ""Year"", ""autoWidth"": true }, { ""title"": ""Current Year"",""data"": ""PresentYear"", ""autoWidth"": true }, { ""title"": ""Age"",""data"": ""Age"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string EmpDobList(int dobmid)
        {
            int a = dobmid;
            return JsonConvert.SerializeObject(Rbus.getAllEmpDob(dobmid));
        }

        public ActionResult KeyOfficials()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "none";
            ViewBag.ReportFiltersTwo = "none";
            ViewBag.ReportFiltersThree = "none";

            ViewBag.ReportTitle = "Work Profile Of Key Officials";
            ViewBag.DataUrl = "/AllReports/KeyOfficialsList";


            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"",""data"": ""EmpId"", ""autoWidth"": true }, {""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true }, {""title"": ""Branch"",""data"": ""BranchDepartmet"", ""autoWidth"": true },
            {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true }, {""title"": ""Graduation"",""data"": ""Graduation"", ""autoWidth"": true },
            {""title"": ""Post Graduation"",""data"": ""PostGraduation"", ""autoWidth"": true }, {""title"": ""Professional Qualifications"",""data"": ""ProfessionalQualifications"", ""autoWidth"": true }, {""title"": ""Total Experience"",""data"": ""TotalExperience"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string KeyOfficialsList()
        {
            return JsonConvert.SerializeObject(Rbus.getAllKeyofficials());

        }

        // Seniority Report
        public ActionResult Seniority()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "none";
            ViewBag.ReportFiltersTwo = "none";
            ViewBag.ReportFiltersThree = "none";

            ViewBag.ReportTitle = "Seniority List";
            ViewBag.DataUrl = "/AllReports/SeniorityData";

            ViewBag.ReportColumns = @"[
                {""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
                {""title"": ""Emp Name"", ""data"": ""EmpName"", ""autoWidth"": true },
                {""title"": ""Designation"", ""data"": ""Designation"", ""autoWidth"": true },
                {""title"": ""Branch/Dept"", ""data"": ""BranchDepartmet"", ""autoWidth"": true },
                {""title"": ""Joining Date"", ""data"": ""DOJ"", ""autoWidth"": true },
                {""title"": ""Years Of Service"", ""data"": ""YearsOfService"", ""autoWidth"": true }
            ]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string SeniorityData()
        {
            return JsonConvert.SerializeObject(Rbus.getAllSeniority());

            // Order employees by Date of Jo
        }
        public string SeniorityListData(int desiId)
        {
            int a = desiId;
            return JsonConvert.SerializeObject(Rbus.getAllSeniorityList(desiId));
        }
        public ActionResult LeavesCarryForward()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "ddl1ddl2";
            ViewBag.ddl1Label = "Search By";
            ViewBag.ddl2Label = "Officer Name";

            string[] array = { "All", "Forwarded", "Approved" };
            var ddlItems = array.Select((x, Index) => new { Name = x, Id = x });
            ViewBag.ddl1data = new SelectList(ddlItems, "Id", "Name");


            var dttwo = new SqlHelper().Get_Table_FromQry("Select [EmpId],[ShortName] from Employees");
            var itemtwo = dttwo.AsEnumerable().Select(r => new Employees
            {
                EmpId = (string)(r["EmpId"]),
                ShortName = (string)(r["ShortName"] ?? "null")
            }).ToList();

            itemtwo.Insert(0, new Employees
            {
                EmpId = "0",
                ShortName = "All"
            });
            ViewBag.ddl2data = new SelectList(itemtwo, "EmpId", "ShortName");

            ViewBag.ReportTitle = "Leave Approval List";

            ViewBag.DataUrl = "/AllReports/LeavesCarryForwardList?srchid=-1&ofcrid=-2";

            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"",""data"": ""EmpId"", ""autoWidth"": true }, {""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true }, {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
            {""title"": ""Branch/Dept"",""data"": ""BranchDepartmet"", ""autoWidth"": true }, {""title"": ""Approved By"",""data"": ""ApprovedBy"", ""autoWidth"": true },
            {""title"": ""Approved By"",""data"": ""ApprovedName"", ""autoWidth"": true }, {""title"": ""Approved Date and Time"",""data"": ""DateTime"", ""autoWidth"": true }, {""title"": ""Status"",""data"": ""Status"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string LeavesCarryForwardList(string srchid, string ofcrid)
        {
            // int ofcid = db.Employes.Where(a => a.EmpId == ofcrid).Select(a => a.Id).FirstOrDefault();
            return JsonConvert.SerializeObject(Rbus.getLeavesCarryForward(srchid, ofcrid));

        }

        public ActionResult HeadOfcAttenders()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "txt1ddl1";
            ViewBag.txt1ddl1txtLabel = "Employee Code";
            ViewBag.txt1ddl1DdlLabel = "Department";

            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Departments where Active=1");
            var ddlItems = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();
            ddlItems.Insert(0, new Departments
            {
                Id = 0,
                Name = "All"
            });
            ViewBag.txt1ddl1DdlData = new SelectList(ddlItems, "Id", "Name");


            ViewBag.ReportTitle = "Head Office Attender List";
            ViewBag.DataUrl = "/AllReports/HeadOfcAttendersList?DptId=-1&EmpCode=-2";


            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"",""data"": ""EmpId"", ""autoWidth"": true }, {""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true }, {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
            {""title"": ""Department"",""data"": ""Department"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string HeadOfcAttendersList(int DptId, string EmpCode)
        {
            return JsonConvert.SerializeObject(Rbus.getHeadofcAttenders(DptId, EmpCode));
        }


        public ActionResult staffCategoryList()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "true";

            ViewBag.ReportFilters = "ddlOne";


            string[] array = { "All", "BC-A", "BC-B", "BC-C", "BC-D", "BC-E", "OC", "SC", "ST" };
            var items = array.Select((x, Index) => new { Name = x, Id = x });

            ViewBag.DdlOneData = new SelectList(items, "Id", "Name");



            ViewBag.DdlOneLabel = "Category";
            ViewBag.DdlTwoLabel = "";
            ViewBag.DdlThreeLabel = "";



            ViewBag.ReportTitle = "Categeory List";
            ViewBag.DataUrl = "/AllReports/staffCategoryItems?DptId=-1";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },{ ""title"": ""Emp Name"",""data"": ""EmpName"", ""autoWidth"": true },{ ""title"": ""Designation"",""data"": ""Designations"",  ""autoWidth"": true },{ ""title"": ""Category"",""data"": ""Category"", ""autoWidth"": true },{ ""title"": ""Gender"",""data"": ""Gender"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");

        }
        [HttpGet]
        public string staffCategoryItems(string DptId)
        {
            return JsonConvert.SerializeObject(Rbus.staffCategory(DptId));
        }

        public ActionResult HeadOfficeList()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "true";

            ViewBag.DdlOneLabel = "Department";
            ViewBag.DdlTwoLabel = "";
            ViewBag.DdlThreeLabel = "";

            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Departments where Active=1");
            var item = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            item.Insert(0, new Departments
            {
                Id = 0,
                Name = "All"
            });

            ViewBag.DdlOneData = new SelectList(item, "Id", "Name");

            ViewBag.ReportFilters = "ddlOne";

            ViewBag.ReportTitle = "Head Office List";
            ViewBag.DataUrl = "/AllReports/headOfficeItems?DptId=-1";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },{ ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },{ ""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },{ ""title"": ""Department"",""data"": ""BranchDepartmet"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string headOfficeItems(int DptId)
        {
            return JsonConvert.SerializeObject(Rbus.headOffice(DptId));
        }

        public ActionResult MonthWiseLeaves()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "true";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.ReportFilters = "ddlOne";
            ViewBag.DdlOneLabel = "Month";

            string[] array = { "All", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec" };
            var items = array.Select((x, Index) => new { Name = x, Id = Index });
            ViewBag.DdlOneData = new SelectList(items, "Id", "Name");

            ViewBag.ReportTitle = "Month Wise Leaves";

            ViewBag.DataUrl = "/AllReports/monthWiseItems?monthid=" + "-1";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },{ ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },{ ""title"": ""Department/Branch"",""data"": ""BranchDepartmet"", ""autoWidth"": true },{ ""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },{ ""title"": ""Applied Date"",""data"": ""AppliedDate"",  ""autoWidth"": true },
                                        { ""title"": ""Applied Time"",""data"": ""AppliedTime"",  ""autoWidth"": true },{ ""title"": ""Start Date"",""data"": ""StartDate"",  ""autoWidth"": true },{ ""title"": ""End Date"",""data"": ""EndDate"",  ""autoWidth"": true },{ ""title"": ""Leave Days"",""data"": ""LeaveDays"",  ""autoWidth"": true },{ ""title"": ""Subject"",""data"": ""Subject"",  ""autoWidth"": true },
                                        { ""title"": ""Reason"",""data"": ""Reason"",  ""autoWidth"": true },{ ""title"": ""Leave Type"",""data"": ""LeaveType"",  ""autoWidth"": true },{ ""title"": ""Status"",""data"": ""Status"",  ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string monthWiseItems(string monthid)
        {
            string monthYear = "";

            if (monthid != "-1" && monthid != "")
            {
                string strDate = monthid;
                string[] sa = strDate.Split('-');
                monthYear = sa[0] + " " + sa[1];
            }
            return JsonConvert.SerializeObject(Rbus.MonthWiseLeaveData(monthYear));
        }

        public ActionResult MonthWiseOD()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "true";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.DdlOneLabel = "Month";
            ViewBag.ReportFilters = "ddlOne";

            string[] array = { "All", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec" };
            var items = array.Select((x, Index) => new { Name = x, Id = Index });
            ViewBag.DdlOneData = new SelectList(items, "Id", "Name");

            ViewBag.ReportTitle = "Month Wise OD";

            ViewBag.DataUrl = "/AllReports/MonthWiseODItems?monthid=" + "-1";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },{ ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },{ ""title"": ""Department/Branch"",""data"": ""BranchDepartmet"", ""autoWidth"": true },{ ""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },
                                        { ""title"": ""Start Date"",""data"": ""StartDate"",  ""autoWidth"": true },{ ""title"": ""End Date"",""data"": ""EndDate"",  ""autoWidth"": true },{ ""title"": ""Visting From"",""data"": ""VistorFrom"",  ""autoWidth"": true },
                                        { ""title"": ""Visting To"",""data"": ""VistorTo"",  ""autoWidth"": true },{ ""title"": ""Description"",""data"": ""Description"",  ""autoWidth"": true },{ ""title"": ""Status"",""data"": ""Status"",  ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string MonthWiseODItems(string monthid)
        {
            string monthYear = "";

            if (monthid != "-1" && monthid != "")
            {
                string strDate = monthid;
                string[] sa = strDate.Split('-');
                monthYear = sa[0] + " " + sa[1];
            }
            return JsonConvert.SerializeObject(Rbus.MonthWiseODData(monthYear));
        }

        public ActionResult EmpPromotions()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();

            string[] payrollaccess = ConfigurationManager.AppSettings["PayrollAccessId"].Split(',');
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";
            ViewBag.PdfSize = "landscape";
            ViewBag.ReportFilters = "txt1ddl1";
            ViewBag.txt1ddl1txtLabel = "Employee Code";
            ViewBag.txt1ddl1DdlLabel = "Promotion Type";

            ViewBag.ReportTitle = "Promotion List";


            string[] array = { "All", "Promotion", "PromotionTransfer" };
            var ddlItems = array.Select((x, Index) => new { Name = x, Id = x });
            ViewBag.txt1ddl1DdlData = new SelectList(ddlItems, "Id", "Name");


            ViewBag.DataUrl = "/AllReports/EmpPromotionsList?trnasfertype=-1&EmpCode=-2";


            bool isPrivilegedUser = payrollaccess.Contains(lCredentials.EmpId);

            // Report columns
            if (isPrivilegedUser)
            {
                ViewBag.ReportColumns = @"[
            {""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true},
            {""title"": ""Emp Name"", ""data"": ""Name"", ""autoWidth"": true},
            {""title"": ""Promotion Type"", ""data"": ""Type"", ""autoWidth"": true},
            {""title"": ""Old Department/Branch"", ""data"": ""OldDepartmentBranch"", ""autoWidth"": true},
            {""title"": ""New Department / Branch"", ""data"": ""NewDepartmentBranch"", ""autoWidth"": true},
            {""title"": ""Old Designation"", ""data"": ""OldDesignation"", ""autoWidth"": true},
            {""title"": ""New Designation"", ""data"": ""NewDesignation"", ""autoWidth"": true},
            {""title"": ""Effective From"", ""data"": ""EffectiveFrom"", ""autoWidth"": true},
            {""title"": ""Old Basic"", ""data"": ""old"", ""autoWidth"": true},
            {""title"": ""New Basic"", ""data"": ""new"", ""autoWidth"": true}
        ]";
            }
            else
            {
                ViewBag.ReportColumns = @"[
            {""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true},
            {""title"": ""Emp Name"", ""data"": ""Name"", ""autoWidth"": true},
            {""title"": ""Promotion Type"", ""data"": ""Type"", ""autoWidth"": true},
            {""title"": ""Old Department/Branch"", ""data"": ""OldDepartmentBranch"", ""autoWidth"": true},
            {""title"": ""New Department / Branch"", ""data"": ""NewDepartmentBranch"", ""autoWidth"": true},
            {""title"": ""Old Designation"", ""data"": ""OldDesignation"", ""autoWidth"": true},
            {""title"": ""New Designation"", ""data"": ""NewDesignation"", ""autoWidth"": true},
            {""title"": ""Effective From"", ""data"": ""EffectiveFrom"", ""autoWidth"": true}
        ]";
            }

            return View("~/Views/AllReports/AllReports.cshtml");
            //if (lCredentials.EmpId == payrollaccess[0] || lCredentials.EmpId == payrollaccess[1] || lCredentials.EmpId == payrollaccess[2] || lCredentials.EmpId == payrollaccess[3] || lCredentials.EmpId == payrollaccess[4])
            //{
            //    ViewBag.ReportColumns = @"[{""title"": ""Emp Code"",""data"": ""EmpId"", ""autoWidth"": true }, {""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true }, {""title"": ""Promotion Type"",""data"": ""Type"", ""autoWidth"": true }, {""title"": ""Old Department/Branch"",""data"": ""OldDepartmentBranch"", ""autoWidth"": true }, 
            //  {""title"": ""New Department / Branch"",""data"": ""NewDepartmentBranch"", ""autoWidth"": true },  
            //  {""title"": ""Old Designation"",""data"": ""OldDesignation"", ""autoWidth"": true }, 
            // {""title"": ""New Designation"",""data"": ""NewDesignation"", ""autoWidth"": true },
            // {""title"": ""Effective From"",""data"": ""EffectiveFrom"", ""autoWidth"": true },
            //    { ""title"": ""Old Basic"",""data"": ""old"", ""autoWidth"": true },
            //          { ""title"": ""New Basic"",""data"": ""new"", ""autoWidth"": true }]";
            //}
            //else
            //{
            //    ViewBag.ReportColumns = @"[{""title"": ""Emp Code"",""data"": ""EmpId"", ""autoWidth"": true }, {""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true }, {""title"": ""Promotion Type"",""data"": ""Type"", ""autoWidth"": true }, {""title"": ""Old Department/Branch"",""data"": ""OldDepartmentBranch"", ""autoWidth"": true }, 
            //  {""title"": ""New Department / Branch"",""data"": ""NewDepartmentBranch"", ""autoWidth"": true },  
            //  {""title"": ""Old Designation"",""data"": ""OldDesignation"", ""autoWidth"": true }, 
            // {""title"": ""New Designation"",""data"": ""NewDesignation"", ""autoWidth"": true },
            // {""title"": ""Effective From"",""data"": ""EffectiveFrom"", ""autoWidth"": true }]";

            //}
            //return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string EmpPromotionsList(string trnasfertype, string EmpCode)
        {
            return JsonConvert.SerializeObject(Rbus.getAllEmpPromotions(trnasfertype, EmpCode));

        }

        public ActionResult MonthWiseTemporaryTransfer()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "true";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.DdlOneLabel = "Month";

            ViewBag.ReportFilters = "ddlOne";

            string[] array = { "All", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec" };
            var items = array.Select((x, Index) => new { Name = x, Id = Index });
            ViewBag.DdlOneData = new SelectList(items, "Id", "Name");

            ViewBag.ReportTitle = "Month Wise Temporary Transfer";

            ViewBag.DataUrl = "/AllReports/MonthWiseTempTransfer?monthid=" + "-1";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },{ ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },{ ""title"": ""Transfer Type"",""data"": ""Type"", ""autoWidth"": true },
                                        { ""title"": ""Old Department/Branch"",""data"": ""OldDepartmentBranch"", ""autoWidth"": true },{ ""title"": ""New Department/Branch"",""data"": ""NewDepartmentBranch"", ""autoWidth"": true },
                                        { ""title"": ""Effective From"",""data"": ""EffectiveFrom"",  ""autoWidth"": true },{ ""title"": ""Effective To"",""data"": ""EffectiveTo"",  ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string MonthWiseTempTransfer(string monthid)
        {
            string monthYear = "";

            if (monthid != "-1" && monthid != "")
            {
                string strDate = monthid;
                string[] sa = strDate.Split('-');
                monthYear = sa[0] + " " + sa[1];
            }
            return JsonConvert.SerializeObject(Rbus.MnthWiseTempTrsfrItemsdata(monthYear));
        }

        //PL
        public ActionResult PrivilegeLeave()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";
            ViewBag.PdfSize = "landscape";
            ViewBag.ReportFilters = "plreportfilters";
            ViewBag.txt1ddl1txtLabel = "Emp Code";
            ViewBag.txt1ddl1DdlLabel = "Status";
            ViewBag.twoBtntwoDt1 = "From Date";
            ViewBag.twoBtntwoDt2 = "To Date";

            string[] array = { "All", "Pending", "Forwarded", "Approved", "Denied", "Cancelled" };
            var items = array.Select((x, Index) => new { Name = x, Id = x });
            ViewBag.txt1ddl1DdlData = new SelectList(items, "Id", "Name");

            ViewBag.ReportTitle = "PL Encashment List";
            ViewBag.DataUrl = "/AllReports/PrivilegeLeaveItems?DptId=-1&EmpCode=-2&fdate=-3&tdate=-4";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
                                        { ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                        { ""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
                                        { ""title"": ""Total Experience"",""data"": ""TotalExperience"", ""autoWidth"": true },
                                        { ""title"": ""Applied Date"",""data"": ""Udate"", ""autoWidth"": true },
                                        { ""title"": ""Year"",""data"": ""CYear"",  ""autoWidth"": true },
                                        { ""title"": ""Total PL"",""data"": ""TotalPL"",  ""autoWidth"": true },
                                        { ""title"": ""PL Encash"",""data"": ""PLEncash"",  ""autoWidth"": true },
                                        { ""title"": ""Subject"",""data"": ""Subject"",  ""autoWidth"": true },
                                        {""title"": ""Status"",
                          data: null, render: function (data, type, LeaveStatus, row) {
                              if (LeaveStatus.Status == 'Pending') {
                                  return ""<h4 style = 'color:#ff8533; font-size:14px;' >Pending</h4>""
                              }
                              else if (LeaveStatus.Status == 'PartialCancelled') {
                                  return ""<h4 style = 'color:#ff4444; font-size:14px;' >PartialCancelled</h4>""
                              }
                              else if (LeaveStatus.Status == 'Cancelled') {
                                  return ""<h4 style = 'color:#ff4444; font-size:14px;' >Cancelled</h4>""
                              }
                              else if (LeaveStatus.Status == 'Approved') {
                                  return ""<h4 style = 'color:#00C851; font-size:14px;' >Approved</h4>""
                              }
                              else if (LeaveStatus.Status == 'Credited') {
                                  return ""<h4 style = 'color:#EC33FF; font-size:14px;' >Credited</h4>""
                              }
                              else if (LeaveStatus.Status == 'Denied') {
                                  return ""<h4 style = 'color:#8B008B; font-size:14px;' >Denied</h4>""
                              }
                              else if (LeaveStatus.Status == 'Forwarded') {
                                  return ""<h4 style = 'color:#33b5e5; font-size:14px;' >Forwarded</h4>""
                              }
                              else if (LeaveStatus.Status == 'Debited') {
                                  return ""<h4 style = 'color:#b366ff; font-size:14px;' >Debited</h4>""
                              } else {

                              }
                          }
}, 


]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string PrivilegeLeaveItems(string DptId, string EmpCode, string fdate, string tdate)
        {
            if (fdate == null)
            {
                fdate = "-3";
            }
            if (tdate == null)
            {
                tdate = "-4";
            }
            return JsonConvert.SerializeObject(Rbus.PrivilegeLeaveItemsdata(DptId, EmpCode, fdate, tdate));
        }

        public ActionResult LongLeaves(string fromDate, string toDate, string leavecnt)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.ReportFilters = "twoSearchButtons";

            ViewBag.twoSrchhBtnname1 = "Leaves 10-20";
            ViewBag.twoSrchhBtnname2 = "Leaves 20 Above";






            ViewBag.ReportTitle = "Long Leaves";
            ViewBag.DataUrl = "/AllReports/LongLeavesItems?fromdate=" + fromDate + "&todate=" + toDate + "&leavecnt=" + leavecnt;
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
                                       {""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                       {""title"": ""Department/Branch"",""data"": ""BranchDepartmet"", ""autoWidth"": true },
                                       {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
                                       {""title"": ""Start Date"",""data"": ""StartDate"", ""autoWidth"": true },
                                       {""title"": ""End Date"",""data"": ""EndDate"",  ""autoWidth"": true },
                                       {""title"": ""Leave Type"",""data"": ""LeaveType"",  ""autoWidth"": true },
                                       {""title"": ""Total Days"",""data"": ""TotalDays"",  ""autoWidth"": true },
                                       {""title"": ""Status"",""data"": ""Status"",  ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string LongLeavesItems(string fromDate, string toDate, string leavecnt)
        {
            return JsonConvert.SerializeObject(Rbus.LongLeavesItemsdata(fromDate, toDate, leavecnt));
        }

        public ActionResult LateAppliedLeaves()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.ReportFilters = "twoDtPickers";

            ViewBag.twoDtLabel1 = "Applied From Date";
            ViewBag.twoDtLabel2 = "Applied To Date";

            ViewBag.ReportTitle = "Late Applied Leaves List";
            ViewBag.DataUrl = "/AllReports/Lateappliedleaveslist?fromDate=-1&toDate=-2";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
                                        { ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                        {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
                                        {""title"": ""Department/Branch"",""data"": ""BranchDepartmet"", ""autoWidth"": true },
                                        { ""title"": ""Applied Date"",""data"": ""AppliedDate"", ""autoWidth"": true },
                                        { ""title"": ""Start Date"",""data"": ""StartDate"",  ""autoWidth"": true },
                                        {""title"": ""End Date"",""data"": ""EndDate"",  ""autoWidth"": true },
                                        { ""title"": ""Type"",""data"": ""LeaveType"",  ""autoWidth"": true },
                                        //{""title"": ""Subject"",""data"": ""Subject"",  ""autoWidth"": true },
                                        { ""title"": ""Reason"",""data"": ""Reason"",  ""autoWidth"": true },
                                        {""title"": ""Status"",""data"": ""Status"",  ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string Lateappliedleaveslist(string fromDate, string toDate)
        {

            return JsonConvert.SerializeObject(Rbus.LateappliedLeaveList(fromDate, toDate));
        }


        public ActionResult FutureAppliedLeaves()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.ReportFilters = "twoDtPickers";

            ViewBag.twoDtLabel1 = "Applied From Date";
            ViewBag.twoDtLabel2 = "Applied To Date";

            ViewBag.ReportTitle = "Future Applied Leaves";
            ViewBag.DataUrl = "/AllReports/Futureappliedleaveslist?fromDate=-1&toDate=-2";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""empid"", ""autoWidth"": true },
                                        { ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                        {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
                                        {""title"": ""Department/Branch"",""data"": ""BranchDepartmet"", ""autoWidth"": true },
                                        { ""title"": ""Applied Date"",""data"": ""AppliedDate"", ""autoWidth"": true },
                                        { ""title"": ""Start Date"",""data"": ""StartDate"",  ""autoWidth"": true },
                                        {""title"": ""End Date"",""data"": ""EndDate"",  ""autoWidth"": true },
                                        { ""title"": ""Leave Type"",""data"": ""LeaveType"",  ""autoWidth"": true },
                                        {""title"": ""Subject"",""data"": ""Subject"",  ""autoWidth"": true },
                                        { ""title"": ""Reason"",""data"": ""Reason"",  ""autoWidth"": true },
                                        {""title"": ""Status"",""data"": ""Status"",  ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string Futureappliedleaveslist(string fromDate, string toDate)
        {

            return JsonConvert.SerializeObject(Rbus.FutureappliedLeaveList(fromDate, toDate));
        }

        public ActionResult CaAndSaOfEmployees(string branch)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.ReportFilters = "CASAFilters";

            ViewBag.twoDtLabel1 = "Applied From Date";
            ViewBag.twoDtLabel2 = "Applied To Date";

            ViewBag.ReportTitle = "CA and SA of Employees";

            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!= 'TGCAB-CTI' and Name!='OtherBranch' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Active=1 order by Name");
            // var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Branches Where Name!='OtherBranch'");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items1.Insert(0, new Departments
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });
            if (lCredentials.Branch == "43")
            {
                var selected4 = (from sub in db.Employes
                                 where sub.EmpId == lCredentials.EmpId
                                 select sub.Department).First();

                ViewBag.DdlOneData = new SelectList(items1, "Id", "Name");

            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData = new SelectList(items, "Name", "Name", brname); //
            }

            ViewBag.DataUrl = "/AllReports/CaAndSaOfEmployeesData?branch=" + branch;
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""Employee_Code"", ""autoWidth"": true },
                                        { ""title"": ""Emp Name"",""data"": ""Employee_Name"", ""autoWidth"": true },
                                        {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
                                        {""title"": ""Branch/Department"",""data"": ""Branch/Department"", ""autoWidth"": true },
                                        {""title"": ""Controlling Authority Code"",""data"": ""Controlling_Authority_Code"",  ""autoWidth"": true },
                                        { ""title"": ""Controlling Authority Name"",""data"": ""Controlling_Authority_Name"",  ""autoWidth"": true },
                                        {""title"": ""Sanctioning Authority Code"",""data"": ""Sanctioning_Authority_Code"",  ""autoWidth"": true },
                                        { ""title"": ""Sanctioning Authority Name"",""data"": ""Sanctioning_Authority_Name"",  ""autoWidth"": true }]";


            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string CaAndSaOfEmployeesData(string branch)
        {
            if (branch.Contains("-") && branch != "HeadOffice-All")
            {
                branch = branch.Substring(1);
            }

            branch = branch.Trim();

            if (branch.Contains("and"))
            {
                branch = branch.Replace("and", "&");
            }
            return JsonConvert.SerializeObject(Rbus.CaAndSaOfEmployeesData2(branch));
        }


        public ActionResult proformapage(string branch)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.ReportFilters = "ProformaFilters";

            ViewBag.twoDtLabel1 = "Applied From Date";
            ViewBag.twoDtLabel2 = "Applied To Date";

            ViewBag.ReportTitle = "PROFORMA-B";

            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Type] from LeaveTypes where Type!='ALL'");
            var items = dt.AsEnumerable().Select(r => new LeaveTypes
            {
                Id = (Int32)(r["Id"]),
                Type = (string)(r["Type"] ?? "null")
            }).ToList();

            ViewBag.LeaveTypes1 = new SelectList(items, "Id", "Type");


            ViewBag.DataUrl = "/AllReports/CreditDebitLeaveData?branch=" + branch;
            ViewBag.ReportColumns = @"[{""title"": ""Date"", ""data"": ""UpdatedDate"", ""autoWidth"": true },
                                        { ""title"": ""Leave Debited"",""data"": ""DebitLeave"", ""autoWidth"": true },
                                        {""title"": ""Leave Credited"",""data"": ""CreditLeave"", ""autoWidth"": true },
                                        {""title"": ""Balance"",""data"": ""LeaveBalance"", ""autoWidth"": true },
                                        ]";


            return View("~/Views/AllReports/proformab.cshtml");
        }

        public JsonResult GetEmpData(string empid)
        {
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var employees = db.Employes.ToList();
            var empname = db.Employes.Where(a => a.EmpId == empid).Select(a => a.ShortName).FirstOrDefault();
            var ldesignation = db.Employes.Where(a => a.EmpId == empid).Select(a => a.CurrentDesignation).FirstOrDefault();
            string desig = db.Designations.Where(a => a.Id == ldesignation).Select(a => a.Name).FirstOrDefault();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();

            return Json(new { lempname = empname, ldisignation = desig }, JsonRequestBehavior.AllowGet);
        }

        public string CreditDebitLeaveData(string empid, string branch, string leaveid, string fdate, string tdate)
        {

            return JsonConvert.SerializeObject(Rbus.Creditdebitleavesdata(empid, leaveid, fdate, tdate));
        }

        public ActionResult LeaveCancelled()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "none";
            ViewBag.PdfSize = "landscape";

            //ViewBag.twoDtLabel1 = "From Date";
            //ViewBag.twoDtLabel2 = "To Date";

            ViewBag.ReportTitle = "Cancelled Leaves";
            ViewBag.DataUrl = "/AllReports/LeaveCancelledList";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
                                        { ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                        { ""title"": ""Start Date"",""data"": ""StartDate"",  ""autoWidth"": true },
                                        {""title"": ""End Date"",""data"": ""EndDate"",  ""autoWidth"": true },
                                        { ""title"": ""Cancelled By"", ""data"": ""CancelledBy"",  ""autoWidth"": true },
                                        {""title"": ""Cancelled Date & Time"",""data"": ""CancelledDateTime"",  ""autoWidth"": true },
                                        { ""title"": ""Reason For Cancelled"",""data"": ""ReasonForCancelled"",  ""autoWidth"": true },
                                        {""title"": ""Status"",""data"": ""LStatus"",  ""autoWidth"": true },
                                        {""title"": ""Stage"",""data"": ""LvcancelStage"",  ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string LeaveCancelledList()
        {
            return JsonConvert.SerializeObject(Rbus.AllLeavesCancelledList());
        }

        public ActionResult MonthwiseCLMLPL()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.ReportFilters = "ddl1ddl2";
            ViewBag.ddl1Label = "Applied Month";
            ViewBag.ddl2Label = "Leave Type";

            string[] array = { "All", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec" };
            var ddlItems = array.Select((x, Index) => new { Name = x, Id = Index });
            ViewBag.ddl1data = new SelectList(ddlItems, "Id", "Name");

            string[] array2 = { "All", "Casual Leave", "Medical/Sick Leave", "Privilege Leave" };
            var dttwo = array2.Select((x, Index) => new { Name = x, Id = Index });
            ViewBag.ddl2data = new SelectList(dttwo, "Id", "Name");

            ViewBag.ReportTitle = "Month Wise CL/ML/PL";
            ViewBag.DataUrl = "/AllReports/MonthwiseCLMLPLList?monthid=" + "-1" + "&leavetype=-2";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
                                        { ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                        {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
                                        {""title"": ""Department/Branch"",""data"": ""BranchDepartmet"", ""autoWidth"": true },
                                        { ""title"": ""Leave Type"",""data"": ""Ltype"",  ""autoWidth"": true },
                                        { ""title"": ""Applied Date"",""data"": ""AppliedDate"", ""autoWidth"": true },
                                        { ""title"": ""Start Date"",""data"": ""StartDate"",  ""autoWidth"": true },
                                        {""title"": ""Difference between Applied Date and From Date "",""data"": ""DiffFromAppliedDate"",  ""autoWidth"": true },
                                        {""title"": ""End Date"",""data"": ""EndDate"",  ""autoWidth"": true },
                                        { ""title"": ""Difference between Applied Date and End Date"",""data"": ""DiffEndAppliedDate"",  ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string MonthwiseCLMLPLList(string monthid, int leavetype)
        {
            string monthYear = "";

            if (monthid != "-1" && monthid != "")
            {
                string strDate = monthid;
                string[] sa = strDate.Split('-');
                monthYear = sa[0] + " " + sa[1];
            }
            return JsonConvert.SerializeObject(Rbus.AllMonthwiseCLMLPLList(monthYear, leavetype));
        }

        public ActionResult PermanentTransfer()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "txt1dtpicker1";
            ViewBag.txtdtpickerLabel1 = "Employee Code";
            ViewBag.txtdtpickerLabel2 = "From Date";
            ViewBag.txtdtpickerLabel3 = "To Date";

            ViewBag.ReportTitle = "Permanent Transfer List";

            //?empcode = -1 & effectivefrom = -2

            ViewBag.DataUrl = "/AllReports/PermanentTransferList?empCode=-1&EffDate=-2&EffTo=-3";


            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"",""data"": ""EmpId"", ""autoWidth"": true },
                                       {""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                       {""title"": ""Transfer Type"",""data"": ""Type"", ""autoWidth"": true },
                                       {""title"": ""Old Department/Branch"",""data"": ""OldDepartmentBranch"", ""autoWidth"": true }, 
                                       {""title"": ""New Department / Branch"",""data"": ""NewDepartmentBranch"", ""autoWidth"": true },  
                                       {""title"": ""Old Designation"",""data"": ""OldDesignation"", ""autoWidth"": true },  
                                       {""title"": ""New Designation"",""data"": ""NewDesignation"", ""autoWidth"": true }, 
                                       {""title"": ""Effective From"",""data"": ""EffectiveFrom"", ""autoWidth"": true },
                                    //],
                                    //   columnDefs: [
                                    //   {
                                    //    targets: -1, // The last column
                                    //    render: function (data, type, row, meta) {
                                    //            return '<button class=""btn btn-danger delete-btn""  data-id =""' + row.Id + '"">Delete</button>';
                                    //    }
                                    //   }
                                       ]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string PermanentTransferList(int empCode, string EffDate, string EffTo)
        {
            if (EffTo == null)
            {
                EffTo = "-3";
            }
            return JsonConvert.SerializeObject(Rbus.getAllEmpPermanentTransfer(empCode, EffDate, EffTo));

        }
        //public ActionResult DeleteRow(Employee_Transfer rowData)
        //{
        //    TempData["RolePages"] = LoginHelper.GetCurrentUserPages();

        //    string empid = rowData.EmpId.ToString();
        //    var etid = db.Employes.Where(a => a.EmpId == empid).Select(a => a.Id).FirstOrDefault();
        //    var transgertype = rowData.Type;
        //    DateTime? effectiveFrom = rowData.EffectiveFrom;

        //    DateTime inputDate = effectiveFrom.GetValueOrDefault();
        //    string outputDateString = inputDate.ToString("yyyy-MM-dd HH:mm:ss.fff");

        //    var upqry = "update Employee_Transfer set Isactive = 0 where Empid='" + etid + "' and Type='" + transgertype + "' and EffectiveFrom='" + outputDateString + "'";
        //    var dt = sh.Run_UPDDEL_ExecuteNonQuery(upqry);

        //    return RedirectToAction("PermanentTransferList");

        //}


        //public ActionResult Seniorty()
        //{
        //    TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
        //    ViewBag.LoginUserName = lCredentials.EmpFullName;
        //    ViewBag.LoginBranch = lCredentials.Branch;
        //    ViewBag.LoginBranchName = lCredentials.BranchName;

        //    ViewBag.SearchBtn = "false";

        //    ViewBag.ReportFilters = "none";

        //    //ViewBag.twoDtLabel1 = "From Date";
        //    //ViewBag.twoDtLabel2 = "To Date";

        //    ViewBag.ReportTitle = "Employee List";
        //    ViewBag.DataUrl = "/AllReports/SeniortyList";
        //    ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
        //                                { ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
        //                                { ""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },
        //                                {""title"": ""Branch/Department"",""data"": ""BranchDepartmet"",  ""autoWidth"": true },
        //                                { ""title"": ""DOB"", ""data"": ""DOB"",  ""autoWidth"": true },
        //                                {""title"": ""DOJ"",""data"": ""DOJ"",  ""autoWidth"": true },
        //                                { ""title"": ""Retirement Date"",""data"": ""RetirementDate"",  ""autoWidth"": true }]";

        //    return View("~/Views/AllReports/Employeelist.cshtml");
        //}
        ////[HttpGet]
        //public string SeniortyList()
        //{
        //    return JsonConvert.SerializeObject(Rbus.AllSeniortyList());
        //}

        public ActionResult StaffMaster()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.ReportFilters = "none";
            ViewBag.DataUrl = "/AllReports/StaffMasterList";

            ViewBag.ReportTitle = "StaffMaster List";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
                                        { ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                        { ""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },
                                        { ""title"": ""DOB"", ""data"": ""DOB"",  ""autoWidth"": true },
                                        {""title"": ""DOJ"",""data"": ""DOJ"",  ""autoWidth"": true },
                                        {""title"": ""Gender"",""data"": ""Gender"",  ""autoWidth"": true },
                                        { ""title"": ""Retirement Date"",""data"": ""RetirementDate"",  ""autoWidth"": true },
                                        { ""title"": ""Father Name"",""data"": ""FatherName"",  ""autoWidth"": true },
                                        { ""title"": ""Mother Name"",""data"": ""MotherName"",  ""autoWidth"": true },
                                        { ""title"": ""Present Address"",""data"": ""preAddress"",  ""autoWidth"": true }, 
                                        { ""title"": ""Permanent Address"",""data"": ""perAddress"",  ""autoWidth"": true }, 
                                        { ""title"": ""Mobile"",""data"": ""MobileNumber"",  ""autoWidth"": true },
                                        { ""title"": ""Category"",""data"": ""Category"",  ""autoWidth"": true },
                                        { ""title"": ""Aadhar No"",""data"": ""AadharCardNo"",  ""autoWidth"": true },
                                        { ""title"": ""Graduation"",""data"": ""Graduation"",  ""autoWidth"": true },
                                        { ""title"": ""PG"",""data"": ""PostGraduation"",  ""autoWidth"": true },
                                        { ""title"": ""Prof Qualifications"",""data"": ""ProfessionalQualifications"",  ""autoWidth"": true },
                                        { ""title"": ""Present Work Place"",""data"": ""PresentWorkPlace"",  ""autoWidth"": true },
                                        { ""title"": ""Personal Email"",""data"": ""PersonalEmailId"",  ""autoWidth"": true },
                                        { ""title"": ""Official Email"",""data"": ""OfficalEmailId"",  ""autoWidth"": true },
                                        { ""title"": ""Emergency ContactNo,"",""data"": ""EmergencyContactNo"",  ""autoWidth"": true },
                                        { ""title"": ""Emergency Contact Name"",""data"": ""EmergencyName"",  ""autoWidth"": true }]";



            //*** Hide,show Columns ***
            ViewBag.ToggleCols = true;
            string[] array = { "Emp Code","Emp Name", "Designation", "DOB", "DOJ", "Gender","Retirement Date", "Father Name", "Mother Name",
                                "Present Address","Permanent Address", "Mobile", "Category","Aadhar No", "Graduation", "PG", "Prof Qualifications", "Present Work Place","Personal Email","Official Email","Emergency ContactNo","Emergency Contact Name"};

            ViewBag.ToggleColsList = array.Select((x, i) => new ToggleColsData { Id = i, Name = x });
            //*** end of Hide,show Columns ***

            return View("~/Views/AllReports/AllToggleCols.cshtml");
        }
        [HttpGet]
        public string StaffMasterList()
        {
            return JsonConvert.SerializeObject(Rbus.AllStaffMasterList());
        }

        public ActionResult TxHistory()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            //ViewBag.ReportFilters = "TwoDtsOneDdl";

            ViewBag.TwoDtsOneDdl3 = "Tx Type ";
            ViewBag.TwoDtsOneDdl1 = "From Date";
            ViewBag.TwoDtsOneDdl2 = "To Date ";


            var dt = new SqlHelper().Get_Table_FromQry("Select distinct(Tx_type) from Tx_History order by (Tx_Type) asc");
            var item = dt.AsEnumerable().Select(r => new Tx_History
            {
                //Id = (Int32)(r["Id"]),
                Tx_type = (string)(r["Tx_type"] ?? "null")
            }).ToList();

            item.Insert(0, new Tx_History
            {
                Id = 0,
                Tx_type = "All"
            });

            ViewBag.TwoDtsOneDdlData = new SelectList(item, "Id", "Tx_type");

            ViewBag.ReportFilters = "TwoDtsOneDdl";
            ViewBag.ReportTitle = "Transaction History";
            ViewBag.DataUrl = "/AllReports/TxHistoryList?fromDate=-1&toDate=-2&LtcType=-3";
            ViewBag.ReportColumns = @"[{""title"": ""Tx_Id"", ""data"": ""Id"", ""autoWidth"": true },
                                        { ""title"": ""Tx_Type"",""data"": ""Tx_type"", ""autoWidth"": true },
                                        { ""title"": ""Tx_Sub type"",""data"": ""Tx_subtype"",  ""autoWidth"": true },
                                        { ""title"": ""Tx_By"", ""data"": ""Tx_By"",  ""autoWidth"": true },
                                        {""title"": ""Tx_On"",""data"": ""Tx_On"",  ""autoWidth"": true },
                                        { ""title"": ""Tx_Date"",""data"": ""Tx_date"",  ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string TxHistoryList(string fromDate, string toDate, string LtcType)
        {

            //string fromdt = fromDate == null && fromDate != "" ? fromDate : "";
            //string todt = toDate == null && toDate != "" ? toDate : "";
            //string ltctype = LtcType == null && LtcType != "" ? LtcType : "";
            //string fromdt = "1900-01-01";
            //string todt = "1900-01-01";
            //string ltctype = "ALL";

            //if (!(fromDate == "-1" && toDate == "-2"))
            //{
            //    fromdt = DateTime.Parse(fromDate).ToString("yyyy-MM-dd");
            //    todt = DateTime.Parse(toDate).ToString("yyyy-MM-dd");
            //    ltctype = LtcType;

            //}
            if (LtcType == null)
            {
                LtcType = "-3";
            }
            return JsonConvert.SerializeObject(Rbus.AllTxHistoryList(fromDate, toDate, LtcType));



        }


        public ActionResult TempTrnsfr()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.ReportFilters = "txt1dtpicker1";

            ViewBag.txtdtpickerLabel1 = "Employee Code";
            ViewBag.txtdtpickerLabel2 = "From Date";
            ViewBag.txtdtpickerLabel3 = "To Date";

            ViewBag.ReportTitle = "Temporary Transfer List";
            ViewBag.DataUrl = "/AllReports/TempTrnsfrList?empCode=-1&EffDate=-2&EffTo=-3";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
                                       {""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                       {""title"": ""Transfer Type"",""data"": ""Type"", ""autoWidth"": true },
                                       {""title"": ""Old Department/Branch"",""data"": ""OldDepartmentBranch"", ""autoWidth"": true }, 
                                       {""title"": ""New Department/Branch"",""data"": ""NewDepartmentBranch"", ""autoWidth"": true },  
                                       {""title"": ""Old Designation"",""data"": ""OldDesignation"", ""autoWidth"": true },  
                                       {""title"": ""New Designation"",""data"": ""NewDesignation"", ""autoWidth"": true }, 
                                       {""title"": ""Effective From"",""data"": ""EffectiveFrom"", ""autoWidth"": true },
                                       {""title"": ""Effective To"",""data"": ""EffectiveTo"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string TempTrnsfrList(int empCode, string EffDate, string EffTo)
        {
            if (EffTo == null)
            {
                EffTo = "-3";
            }
            return JsonConvert.SerializeObject(Rbus.AllTempTrnsfrList(empCode, EffDate, EffTo));
        }

        public ActionResult Topmanagement()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "none";

            //ViewBag.twoDtLabel1 = "From Date";
            //ViewBag.twoDtLabel2 = "To Date";

            ViewBag.ReportTitle = "Top Management List";
            ViewBag.DataUrl = "/AllReports/TopmanagementList";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
                                       {""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                       {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
                                       {""title"": ""Department/Branch"",""data"": ""BranchDepartmet"", ""autoWidth"": true }, 
                                       {""title"": ""Mobile Number"",""data"": ""MobileNumber"", ""autoWidth"": true },  
                                       {""title"": ""Extension"",""data"": ""Extension"", ""autoWidth"": true }]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string TopmanagementList()
        {
            return JsonConvert.SerializeObject(Rbus.AllTopmanagementList());
        }

        public ActionResult Retirement()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "twoBtntwoDtpickers";

            ViewBag.ExportColumns = "columns: [0,1,2,3,4,5,6,7]";

            ViewBag.twoBtntwoDt1 = "From Date";
            ViewBag.twoBtntwoDt2 = "To Date";

            ViewBag.ReportTitle = "Retirements/Resignations";
            ViewBag.DataUrl = "/AllReports/RetirementList?fromdate=-1&todate=-2&RetireType=-3";


            return View("~/Views/AllReports/Retirements.cshtml");
        }
        [HttpGet]
        public string RetirementList(string fromdate, string todate, string RetireType)
        {
            return JsonConvert.SerializeObject(Rbus.AllRetirementList(fromdate, todate, RetireType));
        }

        public ActionResult LTCList()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "TwoDtsOneDdl";
            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";
            ViewBag.TwoDtsOneDdl1 = "From Date";
            ViewBag.TwoDtsOneDdl2 = "To Date";
            ViewBag.TwoDtsOneDdl3 = "Type";

            string[] array2 = { "All", "Availment", "Encashment" };
            var dttwo = array2.Select((x, Index) => new { Name = x, Id = Index });
            ViewBag.TwoDtsOneDdlData = new SelectList(dttwo, "Id", "Name");

            ViewBag.ReportTitle = "LTC List";
            ViewBag.DataUrl = "/AllReports/LTCListdata?fromDate=-1&toDate=-2&LtcType=-3";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Code"", ""data"": ""EmpId"", ""autoWidth"": true },
                                        { ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },
                                        {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
                                        {""title"": ""Department/Branch"",""data"": ""BranchDepartmet"", ""autoWidth"": true },
                                        { ""title"": ""LTC Type"",""data"": ""LTCType"",  ""autoWidth"": true },
                                        { ""title"": ""Applied Date"",""data"": ""AppliedDate"", ""autoWidth"": true },
                                        { ""title"": ""From Date"",""data"": ""StartDate"",  ""autoWidth"": true },
                                        { ""title"": ""To Date"",""data"": ""EndDate"",  ""autoWidth"": true },
                                        {""title"": ""Place Of Visits"",""data"": ""PlaceOfVisits"",  ""autoWidth"": true },
                                        {""title"": ""Mode Of Transport"",""data"": ""ModeOfTransport"",  ""autoWidth"": true },
                                        { ""title"": ""Travel Advance"",""data"": ""TravelAdvance"",  ""autoWidth"": true },
                                        { ""title"": ""Block Period"",""data"": ""Block_Period"",  ""autoWidth"": true },
                                        
                                        {""title"": ""Status"",
                          data: null, render: function (data, type, LeaveStatus, row) {
                              if (LeaveStatus.Status == 'Pending') {
                                  return ""<h4 style = 'color:#ff8533; font-size:14px;' >Pending</h4>""
                              }
                              else if (LeaveStatus.Status == 'PartialCancelled') {
                                  return ""<h4 style = 'color:#ff4444; font-size:14px;' >PartialCancelled</h4>""
                              }
                              else if (LeaveStatus.Status == 'Cancelled') {
                                  return ""<h4 style = 'color:#ff4444; font-size:14px;' >Cancelled</h4>""
                              }
                              else if (LeaveStatus.Status == 'Approved') {
                                  return ""<h4 style = 'color:#00C851; font-size:14px;' >Approved</h4>""
                              }
                              else if (LeaveStatus.Status == 'Credited') {
                                  return ""<h4 style = 'color:#EC33FF; font-size:14px;' >Credited</h4>""
                              }
                              else if (LeaveStatus.Status == 'Denied') {
                                  return ""<h4 style = 'color:#8B008B; font-size:14px;' >Denied</h4>""
                              }
                              else if (LeaveStatus.Status == 'Forwarded') {
                                  return ""<h4 style = 'color:#33b5e5; font-size:14px;' >Forwarded</h4>""
                              }
                              else if (LeaveStatus.Status == 'Debited') {
                                  return ""<h4 style = 'color:#b366ff; font-size:14px;' >Debited</h4>""
                              } else {

                              }
                          }
}, 


                                          
                                        ]";

            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string LTCListdata(string fromDate, string toDate, string LtcType)
        {
            return JsonConvert.SerializeObject(Rbus.AllLTCListdata(fromDate, toDate, LtcType));
        }

        public ActionResult EmployeesRept()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.websiteUrl = System.Configuration.ConfigurationManager.AppSettings["websiteUrl"];

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "none";

            ViewBag.ExportColumns = "columns: [1,2,3,4,5]";
            ViewBag.ReportTitle = "Employee List";
            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "a4";
            //ViewBag.DataUrl = "/AllReports/EmployeesReptList";

            ViewBag.DataUrl = "/AllReports/EmployeesReptList?empid=-1";

            return View("~/Views/AllReports/Employeelist.cshtml");
        }
        [HttpGet]
        public string EmployeesReptList(string empid)
        {
            return JsonConvert.SerializeObject(Rbus.AllEmpReptList(empid));
        }
        //[HttpGet]
        //public string AllEmpReptListsearch(string empid)
        //{
        //    return JsonConvert.SerializeObject(Rbus.AllEmpReptListsearch( empid));
        //}
        public ActionResult LeavesHistory()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";
            ViewBag.ExportColumns = "columns: [0,1,2,3,4,5,6,7,8,9,10,11,13,14,15,16,17]";

            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "TABLOID";

            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Type] from LeaveTypes");
            var item = dt.AsEnumerable().Select(r => new LeaveTypes
            {
                Id = (Int32)(r["Id"]),
                Type = (string)(r["Type"] ?? "null")
            }).ToList();

            item.Insert(0, new LeaveTypes
            {
                Id = 0,
                Type = "All"
            });

            ViewBag.DdlOneData = new SelectList(item, "Id", "Type");


            //
            //var dt1 = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!='OtherBranch' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            //var items = dt1.AsEnumerable().Select(r => new Branches
            //{
            //    Id = (Int32)(r["Id"]),
            //    Name = (string)(r["Name"] ?? "null")
            //}).ToList();


            //var items1 = dt1.AsEnumerable().Select(r => new Departments
            //{
            //    Id = (Int32)(r["Id"]),
            //    Name = (string)(r["Name"] ?? "null")
            //}).ToList();


            //items.Insert(0, new Branches
            //{
            //    Id = 0,
            //    Name = "All"
            //});

            //items.Insert(45, new Branches
            //{
            //    Id = 45,
            //    Name = "HeadOffice-All"
            //});

            var dtbranches = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Branches Where Name!='OtherBranch' and id not in (42,43,54)");
            var items = dtbranches.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            //ViewBag.DdlOneDatabranch = new SelectList(items, "Name", "Name");
            ViewBag.ReportTitle = "Branches List";
            ViewBag.DdlOneDatabranch = items;
            var dtdepartments = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Departments where Active=1 and id!=46");
            var item1 = dtdepartments.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            item1.Insert(0, new Departments
            {
                Id = 0,
                Name = "All"
            });

            //ViewBag.DdlOneDatadepartment = new SelectList(item1, "Name", "Name");
            ViewBag.DdlOneDatadepartment = item1;
            ViewBag.ReportTitle = "Head Office List";
            ViewBag.ReportFilters = "twoSearchButtons";
            ViewBag.twoSrchhBtnname1 = "All Leaves";
            ViewBag.twoSrchhBtnname2 = "Today Leaves";
            ViewBag.ReportTitle = "Emp Leave History";
            ViewBag.DataUrl = "/AllReports/LeavesHistoryList?fromDate=-1&toDate=-2&datetype=-3&LeaveType=-4&LeaveStatus=-5&branch=-6&empcode=-7&datatype1=-8&dept=-9";
            ViewBag.DataUrl1 = "/AllReports/LeavesListToday?branch=-1&empid=-2&datetype1=-3&dept=-4";
            return View("~/Views/AllReports/Leaves.cshtml");
        }
        [HttpGet]
        public string LeavesHistoryList(string fromDate, string toDate, string datetype, string LeaveType, string LeaveStatus, string branch, string empcode, string datatype1, string dept)
        {
            //int bra = Convert.ToInt32 (branch);
            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            string EmpIds = lCredentials.EmpId;
            return JsonConvert.SerializeObject(Rbus.AllLeavesHistoryList(fromDate, toDate, datetype, LeaveType, LeaveStatus, branch, empcode, datatype1, dept));
        }

        public ActionResult OdDuty()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";
            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";

            ViewBag.ExportColumns = "columns: [1,2,3,4,5,6,7,8,9,10,11,12,13]";

            ViewBag.ReportFilters = "twoSearchButtons";

            ViewBag.twoSrchhBtnname1 = "All Leaves";
            ViewBag.twoSrchhBtnname2 = "Today Leaves";


            ViewBag.ReportTitle = "OD List";
            ViewBag.DataUrl = "/AllReports/OdDutyList?fromDate=-1&toDate=-2&datetype=-3";

            return View("~/Views/AllReports/OdList.cshtml");
        }
        [HttpGet]
        public string OdDutyList(string fromDate, string toDate, string datetype)
        {
            return JsonConvert.SerializeObject(Rbus.AllOdDutyList(fromDate, toDate, datetype));
        }

        [HttpGet]
        public string OdDutyListToday()
        {
            return JsonConvert.SerializeObject(Rbus.TodayOdDutyList());
        }
        [HttpGet]
        public string LeavesListToday(string branch, string empid, string datetype1, string dept)
        {
            return JsonConvert.SerializeObject(Rbus.TodayLeavesList(branch, empid, datetype1, dept));
        }

        public ActionResult UserRolesView()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.DataUrl = "/AllReports/UserRolesViewList";

            return View("~/Views/Roles/UserRoles.cshtml");
        }
        [HttpGet]
        public string UserRolesViewList()
        {
            return JsonConvert.SerializeObject(Rbus.AllUserRolesViewList());
        }
        //Daily Time Sheet
        public ActionResult DailyReport()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Branch Device Report";
            ViewBag.ReportFilters = "none";

            ViewBag.DataUrl = "/AllReports/DailyReportdata";
            ViewBag.ReportColumns = @"[{""title"": ""SNO"",""data"": ""Sno"",  ""autoWidth"": true },
                                       /*{""title"": ""Branch Id"",""data"": ""BranchId"",  ""autoWidth"": true },*/
                                       {""title"": ""Branch Name"", ""data"": ""BranchName"", ""autoWidth"": true },
                                       {""title"": ""Start Time"", ""data"": ""starttime"", ""autoWidth"": true },
                                       {""title"": ""End Time"",""data"": ""endtime"", ""autoWidth"": true },
                                       {""title"": ""Device Id"",""data"": ""Device_Id"",  ""autoWidth"": true }]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string DailyReportdata()
        {
            var dt = Tbus.DailyReportTimesheetMstdata();
            return JsonConvert.SerializeObject(dt);
        }

        public ActionResult Timesheedata()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "none";
            ViewBag.DataUrl = "/AllReports/TimesheetMstdata";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Id"",""data"": ""user_id"",  ""autoWidth"": true },{""title"": ""Branch ID"", ""data"": ""device_id"", ""autoWidth"": true },{""title"": ""Start Time"",""data"": ""starttime"", ""autoWidth"": true },{""title"": ""End Time"",""data"": ""endtime"",  ""autoWidth"": true }]";
            return View("~/Views/Timesheet/CreateShiftMaster.cshtml");
        }

        [HttpGet]
        public string TimesheetMstdata()
        {
            var dt = Tbus.getAllTimesheetMstdata();
            return JsonConvert.SerializeObject(dt);
        }

        public ActionResult BranchMasterdata()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "none";
            ViewBag.DataUrl = "/AllReports/BranchMstdata";
            ViewBag.ReportColumns = @"[{""title"": ""Branch Id"",""data"": ""BranchId"",  ""autoWidth"": true },{""title"": ""Device ID"", ""data"": ""Device_id"", ""autoWidth"": true },{""title"": ""Branch Name"",""data"": ""BranchName"", ""autoWidth"": true }]";
            return View("~/Views/Timesheet/BranchMaster.cshtml");
        }

        [HttpGet]
        public string BranchMstdata()
        {
            var dt = Tbus.GetAllBranchMasterData();
            return JsonConvert.SerializeObject(dt);
        }
        //late arrivals report
        public ActionResult LateArrivalsReport(string branch, string fromdate, string todate, string empcode, string Type)
        {


            string lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            ViewBag.EmpId = lCredentials.EmpId;
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Late Arrivals";
            ViewBag.ReportFilters = "oneddlonedtpicker";
            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!='OtherBranch' and Name!= 'TGCAB-CTI' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });
            var deptId = db.Employes
    .Where(sub => sub.EmpId == lCredentials.EmpId)
    .Select(sub => sub.Department)
    .FirstOrDefault();

            var selectedDept = db.Departments
                .Where(d => d.Id == deptId)
                .Select(d => d.Name)
                .FirstOrDefault();
            string formattedSelectedDept = "-" + selectedDept;
            if (lCredentials.Branch == "43")
            {
                //var selected4 = (from sub in db.Employes
                //                 where sub.EmpId == lCredentials.EmpId
                //                 select sub.Department).First();

                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", formattedSelectedDept);

            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
            }

            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");




            if (branch == string.Empty && fromdate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/LateArrivalsReportdata";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/LateArrivalsReportdata?branch=" + branch + "&fromdate=" + fromdate + "&todate=" + todate + "&empcode=" + empcode;
            }

            ViewBag.ReportColumns = @"[{""title"": ""Date"", ""data"": ""Date"", ""autoWidth"": true },
            {""title"": ""Code"", ""data"": ""EmpId"", ""autoWidth"": true }, 
            {""title"": ""Emp.Name"", ""data"": ""EmpName"", ""autoWidth"": true }, 
            {""title"": ""Designation"", ""data"": ""Designation"", ""autoWidth"": true }, 
            {""title"": ""Branch/Dept"",""data"": ""BrDept"",  ""autoWidth"": true },            
            {""title"": ""Shift St. Time"",""data"": ""BranchStartTime"",  ""autoWidth"": true },
            {""title"": ""Emp. In Time"",""data"": ""EmpCheckInTime"",  ""autoWidth"": true },
            {""title"": ""Late By"",""data"": ""LateBy"",  ""autoWidth"": true },
            {""title"": ""Late After Grace Period"",""data"": ""LateafterGracePeriod"",  ""autoWidth"": true }]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string LateArrivalsReportdata(string branch, string fromdate, string todate, string empcode, string Type, string self)
        {
            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);
            if (branch.Contains("-") && branch != "HeadOffice-All")
            {
                branch = branch.Substring(1);
            }

            branch = branch.Trim();

            if (branch.Contains("and"))
            {
                branch = branch.Replace("and", "&");
            }
            var dt = Tbus.LateArrivalsTimesheetMstdata(branch, fromdate, todate, empcode, Type, self, EmpIds);
            return JsonConvert.SerializeObject(dt);
        }
        //Early Departs report
        public ActionResult EarlyDepartsReport(string branch, string fromdate)
        {
            string lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.EmpId = lCredentials.EmpId;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Early Departs";
            ViewBag.ReportFilters = "erdeptddlonedtpikerone";

            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!= 'TGCAB-CTI' and Name!='OtherBranch' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();



            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });

            if (lCredentials.Branch == "43")
            {
                var selected4 = (from sub in db.Employes
                                 where sub.EmpId == lCredentials.EmpId
                                 select sub.Department).First();

                ViewBag.DdlOneData1 = new SelectList(items1, "Id", "Name", selected4);

            }
            else
            {

                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
                //// ViewBag.DdlOneData = new SelectList(items, "Name", "Name");
                //var selected4 = (from sub in db.Employes
                //                 where sub.EmpId == lCredentials.EmpId
                //                 select sub.Branch).First();

                //ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", selected4);
            }


            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");

            if (branch == string.Empty && fromdate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/EarlyDepartsReportdata";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/EarlyDepartsReportdata?branch=" + branch + "&fromdate=" + fromdate;
            }

            ViewBag.ReportColumns = @"[{""title"": ""Date"", ""data"": ""Date"", ""autoWidth"": true },
            {""title"": ""Code"", ""data"": ""EmpId"", ""autoWidth"": true },
            {""title"": ""Emp Name"",""data"": ""EmpName"",  ""autoWidth"": true },
            {""title"": ""Designation"", ""data"": ""Designation"", ""autoWidth"": true }, 
            {""title"": ""Branch/Dept"",""data"": ""BrDept"",  ""autoWidth"": true },
            {""title"": ""Shift End Time"",""data"": ""BranchEndTime"",  ""autoWidth"": true },
            {""title"": ""Emp Out Time"",""data"": ""EmpCheckOut"",  ""autoWidth"": true },
            {""title"": ""Early By(HH:MM)"",""data"": ""EarlyBy"",  ""autoWidth"": true }]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string EarlyDepartsReportdata(string branch, string fromdate, string todate, string empcode, string self)
        {

            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);
            if (branch.Contains("-") && branch != "HeadOffice-All")
            {
                branch = branch.Substring(1);
            }
            else if (branch.Contains(" "))
            {
                branch = branch.TrimStart(' ');
            }
            if (branch.Contains("and"))
            {
                branch = branch.Replace("and", "&");
            }
            var dt = Tbus.EarlyDepartsTimesheetMstdata(branch, fromdate, todate, empcode, self, EmpIds);
            return JsonConvert.SerializeObject(dt);
        }

        //Employee Timesheet
        //public ActionResult EmployeeTimesheet(string branch, string fromdate, string todate, string empcode,string intime,string outtime)
        //{
        //    string lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
        //    int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
        //    ViewBag.Role = role.ToString();
        //    TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
        //    ViewBag.LoginUserName = lCredentials.EmpFullName;
        //    ViewBag.LoginBranch = lCredentials.Branch;
        //    ViewBag.LoginBranchName = lCredentials.BranchName;
        //    ViewBag.EmpId = lCredentials.EmpId;
        //    ViewBag.SearchBtn = "false";
        //    ViewBag.ReportTitle = "Employee Timesheet";
        //    ViewBag.ReportFilters = "earlytimesheetSrch";
        //    ViewBag.PdfSize = "landscape";
        //    ViewBag.pageSize = "LEGAL";
        //    //ViewBag.DataUrl = "/AllReports/EmployeeTimesheetdata";

        //    //var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!='OtherBranch' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
        //    var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!='OtherBranch' order by Name");
        //    var items = dt.AsEnumerable().Select(r => new Branches
        //    {
        //        Id = (Int32)(r["Id"]),
        //        Name = (string)(r["Name"] ?? "null")
        //    }).ToList();


        //    var items1 = dt.AsEnumerable().Select(r => new Departments
        //    {
        //        Id = (Int32)(r["Id"]),
        //        Name = (string)(r["Name"] ?? "null")
        //    }).ToList();


        //    items.Insert(0, new Branches
        //    {
        //        Id = 0,
        //        Name = "All"
        //    });

        //    // commented the below lines of code by chaitanya on 23/03/2020
        //    //items.Insert(45, new Branches
        //    //{
        //    //    Id = 45,
        //    //    Name = "HeadOffice-All"
        //    //});
        //    // added by chaitanya on 23/03/2020
        //    items.Insert(44, new Branches
        //    {
        //        Id = 44,
        //        Name = "HeadOffice"
        //    });
        //    //end

        //    if (lCredentials.Branch == "43")
        //    {
        //        var selected4 = (from sub in db.Employes
        //                         where sub.EmpId == lCredentials.EmpId
        //                         select sub.Department).First();

        //        //ViewBag.DdlOneData1 = new SelectList(items1, "Id", "Name", selected4);
        //        ViewBag.DdlOneData1 = new SelectList(items, "Id", "Name", 44);
        //        //end
        //    }
        //    else
        //    {
        //        string brname = " " + lCredentials.BranchName.Replace(" Br", "");
        //        ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
        //    }

        //    ViewBag.DdlOneData = new SelectList(items, "Name", "Name");


        //    if (branch == string.Empty && fromdate == string.Empty)
        //    {
        //        ViewBag.DataUrl = "/AllReports/EmployeeTimesheetdata";
        //    }
        //    else
        //    {
        //        ViewBag.DataUrl = "/AllReports/EmployeeTimesheetdata?branch=" + branch + "&fromdate=" + fromdate + "&todate=" + todate + "&empcode=" + empcode+"&intime="+intime+"&outtime="+outtime;
        //    }


        //    ViewBag.ReportColumns = @"[{""title"": ""Date"",""data"": ""Date"",  ""autoWidth"": true },
        //    {""title"": ""Code"", ""data"": ""EmpId"", ""autoWidth"": true },
        //    {""title"": ""Emp Name"",""data"": ""EmpName"",  ""autoWidth"": true },
        //    {""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true }, 
        //    {""title"": ""Branch/Dept"",""data"": ""BrDept"",  ""autoWidth"": true },    
        //    {""title"": ""Shift Start Time"",""data"": ""BranchStartTime"",  ""autoWidth"": true },
        //    {""title"": ""Emp. In Time"",""data"": ""EmpCheckInTime"",  ""autoWidth"": true },
        //    {""title"": ""Late by(HH:MM)"",""data"": ""LateBy"",  ""autoWidth"": true },     
        //    {""title"": ""Shift Out Time"",""data"": ""BranchCheckOutTime"",  ""autoWidth"": true },
        //    {""title"": ""Emp. Out Time"",""data"": ""EmpCheckOutTime"",  ""autoWidth"": true },            
        //    {""title"": ""Early by(HH:MM)"",""data"": ""EarlyBy"",  ""autoWidth"": true }           
        //    ]";
        //    return View("~/Views/AllReports/AllReports.cshtml");
        //}

        //public ActionResult CreateExcelFile()
        //{
        public void CreateExcelFile()
        {
            SqlHelper sh = new SqlHelper();
            DataTable dttt = sh.Get_Table_FromQry("select getdate() as today");
            DateTime str = Convert.ToDateTime(dttt.Rows[0]["today"]);
            string datstr1 = str.ToString("dd-MM-yyyy");
            string datstr = str.ToString("yyyy-MM-dd");
            string[] arr = datstr.Split('-');
            if (arr[2].StartsWith("0"))
            {
                arr[2] = arr[2].TrimStart('0');
            }

            sh.Run_UPDDEL_ExecuteNonQuery("create table tempempcount(NumberofEmp int,PrsentEmp int,LeaveEmp int,AbsentEmp int,todaydate date);");
            sh.Run_UPDDEL_ExecuteNonQuery("insert into tempempcount (NumberofEmp)values((select count(EmpId)as NofEmployees from Employees where RetirementDate>getdate()))");
            sh.Run_UPDDEL_ExecuteNonQuery("update tempempcount set PrsentEmp =(select count(distinct user_id) as cntofEMP from timesheet_logs where cast(io_time as date)=cast(getdate() as date) and user_id not in (select empid from DCCB_employees) )");
            sh.Run_UPDDEL_ExecuteNonQuery("update tempempcount set LeaveEmp =(select COUNT (*) from timesheet_Emp_Month where month=" + arr[1] + "  and year=" + arr[0] + " and D" + arr[2] + "_Status  in('CL','ML','PL','MTL','PTL','EOL','SCL','C-OFF','LOP') )");
            sh.Run_UPDDEL_ExecuteNonQuery("update tempempcount set AbsentEmp =(select COUNT (*) from timesheet_Emp_Month where month=" + arr[1] + "  and year=" + arr[0] + " and D" + arr[2] + "_Status in('AB') )");
            sh.Run_UPDDEL_ExecuteNonQuery("update tempempcount set todaydate = (select getdate()) ");
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                //Workbooks wbs = app.Workbooks;
                using (SqlCommand cmd = new SqlCommand("select * from tempempcount; select e.EmpId,e.ShortName EmpName ,case when b.name = 'OtherBranch' then d.name else b.name end as BrDept, t.D" + arr[2] + "_Status as status,cast(getdate() as date) as [date] from Employees e  join timesheet_Emp_Month t on e.EmpId=t.UserId join Branches b on e.Branch=b.Id join Departments d on e.Department=d.id where  t.month=" + arr[1] + " and t.year=" + arr[0] + " and t.D" + arr[2] + "_Status in('CL','ML','PL','MTL','PTL','EOL','SCL','C-OFF','LOP');select e.EmpId,e.ShortName as EmpName,case when b.name = 'OtherBranch' then d.name else b.name end as BrDept, t.D" + arr[2] + "_Status as status,cast(getdate() as date) as [date] from Employees e join timesheet_Emp_Month t on e.EmpId=t.UserId join Branches b on e.Branch=b.Id join Departments d on e.Department=d.id where  t.month=" + arr[1] + " and t.year=" + arr[0] + " and t.D" + arr[2] + "_Status   in('AB')   "))
                {


                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataSet ds = new DataSet())
                        {

                            //cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                sda.Fill(ds); int cntt = 0;
                                foreach (DataTable dt in ds.Tables)
                                {
                                    cntt = cntt + 1;
                                    try
                                    {
                                        if (cntt == 1)
                                        {
                                            wb.Worksheets.Add(dt, "Employees Count");
                                        }
                                        else if (cntt == 2)
                                        {
                                            wb.Worksheets.Add(dt, "Employees in Leaves");
                                        }
                                        else
                                        {
                                            wb.Worksheets.Add(dt, "Absent Employees");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }

                                }
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment;filename=TimesheetReport" + datstr1 + ".xlsx");
                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    Response.End();
                                }

                            }
                        }
                    }
                }
                sh.Run_UPDDEL_ExecuteNonQuery("drop table tempempcount");
            }

        }


        //Employee Timesheet
        public ActionResult EmployeeTimesheet(string branch, string fromdate, string todate, string empcode, string intime, string outtime)
        {
            string lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.EmpId = lCredentials.EmpId;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Employee Timesheet";
            ViewBag.ReportFilters = "earlytimesheetSrch";
            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";
            //ViewBag.DataUrl = "/AllReports/EmployeeTimesheetdata";

            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!='OtherBranch' and Name!= 'TGCAB-CTI' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });


            if (lCredentials.Branch == "43")
            {
                var selected4 = (from sub in db.Employes
                                 where sub.EmpId == lCredentials.EmpId
                                 select sub.Department).First();

                ViewBag.DdlOneData1 = new SelectList(items1, "Id", "Name", selected4);

            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
            }

            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");


            if (branch == string.Empty && fromdate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/EmployeeTimesheetdata";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/EmployeeTimesheetdata?branch=" + branch + "&fromdate=" + fromdate + "&todate=" + todate + "&empcode=" + empcode + "&intime=" + intime + "&outtime=" + outtime;
            }


            ViewBag.ReportColumns = @"[{""title"": ""Date"",""data"": ""Date"",  ""autoWidth"": true },
            {""title"": ""Code"", ""data"": ""EmpId"", ""autoWidth"": true },
            {""title"": ""Emp Name"",""data"": ""EmpName"",  ""autoWidth"": true },
            {""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true }, 
            {""title"": ""Branch/Dept"",""data"": ""BrDept"",  ""autoWidth"": true },    
            {""title"": ""Shift Start Time"",""data"": ""BranchStartTime"",  ""autoWidth"": true },
            {""title"": ""Emp. In Time"",""data"": ""EmpCheckInTime"",  ""autoWidth"": true },
            {""title"": ""Late by(HH:MM)"",""data"": ""LateBy"",  ""autoWidth"": true },     
            {""title"": ""Shift Out Time"",""data"": ""BranchCheckOutTime"",  ""autoWidth"": true },
            {""title"": ""Emp. Out Time"",""data"": ""EmpCheckOutTime"",  ""autoWidth"": true },            
            {""title"": ""Early by(HH:MM)"",""data"": ""EarlyBy"",  ""autoWidth"": true },          
            {""title"": ""Biometric/Manual"",""data"": ""BioManual"",  ""autoWidth"": true }   
            ]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string EmployeeTimesheetdata(string branch, string fromdate, string todate, string empcode, string Type, string self, string intime, string outtime)
        {

            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);
            if (branch.StartsWith("-") && branch != "HeadOffice-All")
            {
                branch = branch.Substring(1);
            }
            else if (branch.Contains(" "))
            {
                branch = branch.TrimStart(' ');
            }
            if (branch.Contains("and"))
            {
                branch = branch.Replace("and", "&");
            }
            //fromdate = fromdate.GetSqlDateString();
            //todate = todate.GetSqlDateString();
            var dt = Tbus.EmployeeTimesheetMstdata(branch, fromdate, todate, empcode, Type, self, EmpIds, intime, outtime);
            return JsonConvert.SerializeObject(dt);
        }
        //Print pdf late monthly memo
        public ActionResult PdfMonthlyMemo(string empcode, string month)
        {


            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            DateTime str = Convert.ToDateTime(month);
            ViewBag.Date = DateTime.Now.Date.ToString("dd/MM/yyyy");
            string str1 = str.ToString("yyyy-MM-dd");
            string[] sa = str1.Split('-');
            string s1 = sa[0];
            string s2 = sa[1];
            if (s2.StartsWith("0"))
            {
                s2 = s2.Substring(1);
            }
            string s3 = sa[2];
            ViewBag.EmpCode = empcode;
            ViewBag.MonthName = str.ToString("MMM/yyyy");
            ViewBag.EmpName = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.ShortName).FirstOrDefault();
            int branch = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Branch).FirstOrDefault();
            string branchs = db.Branches.Where(a => a.Id == branch).Select(a => a.Name).FirstOrDefault();
            int dept = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Department).FirstOrDefault();
            string depts = db.Departments.Where(a => a.Id == dept).Select(a => a.Name).FirstOrDefault();
            string photo = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.UploadPhoto).FirstOrDefault();
            int desig = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.CurrentDesignation).FirstOrDefault();
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
            string months = s2;
            string year = s1;
            ViewBag.Month = s2;
            ViewBag.Year = s1;
            var dt = Tbus.EmpMonthlyLateMemoTimesheet1(months, year, empcode);
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
            return View("~/Views/AllReports/PdfMonthlyMemo.cshtml");


        }
        //Late Attendance Early departure memo Monthly
        public ActionResult MonthlyLateMemoTimesheet(string branch, string fromdate)
        {
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.EmpId = lCredentials.EmpId;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";

            ViewBag.ReportTitle = "Late Memo Monthly";
            ViewBag.ReportFilters = "empmmnthlylatememotimereport";
            //ViewBag.PdfSize = "landscape";
            //ViewBag.pageSize = "TABLOID";

            //ViewBag.ExportColumns = "columns: [1,2,3,4,5,6,7,8,9,10,11,12,13]";

            //   ViewBag.DataUrl = "/AllReports/EmpMonthlyLateData";


            var dt = new SqlHelper().Get_Table_FromQry("Select[Id], concat((' '), (Name)) as Name from Branches where Name != 'HeadOffice' and Name != 'OtherBranch' and Name!= 'TGCAB-CTI' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });

            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            if (lCredentials.Branch == "43")
            {
                var selected4 = (from sub in db.Employes
                                 where sub.EmpId == lCredentials.EmpId
                                 select sub.Department).First();

                ViewBag.DdlOneData1 = new SelectList(items1, "Id", "Name", selected4);

            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
            }

            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");

            if (branch == string.Empty && fromdate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/EmpMonthlyLateMemoData";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/EmpMonthlyLateMemoData?branch=" + branch + "&fromdate=" + fromdate;
            }

            ViewBag.ReportColumns = @"[
   {""title"": ""Code"", ""data"": ""Empcode"", ""autoWidth"": true },
            {""title"": ""Emp Name"",""data"": ""EmpName"",  ""autoWidth"": true },
        {""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },
            {""title"": ""Branch/Dept"",""data"": ""BrDept"",  ""autoWidth"": true },
            {""title"": ""Month"", ""data"": ""Month"", ""autoWidth"": true },
            {""title"": ""Branch(InTime-OutTime)"",""data"": ""BranchTimings"",  ""autoWidth"": true },
  //{""title"": ""Shift End Time"",""data"": ""BranchCheckOutTime"",  ""autoWidth"": true },  
   {""title"": ""Day (In Time - Out Time)"", ""data"": ""IntimeOutTime"", ""autoWidth"": true },
//{""title"": ""NA (In Time - Out Time)"", ""data"": ""NAIntimeOutTime"", ""autoWidth"": true },
            {""title"": ""Total Late Comes"",""data"": ""TotalLateComes"",  ""autoWidth"": true },
            {""title"": ""LeaveAppliedDate"",""data"": ""LeaveAppliedDate"",  ""autoWidth"": true },
            {""title"": ""LeaveType"",""data"": ""LeaveType"",  ""autoWidth"": true },
            {""title"": ""Priornoticegivendays"",""data"": ""Priornoticegivendays"",  ""autoWidth"": true },
            {""title"": ""ReasonForLeave"",""data"": ""ReasonForLeave"",  ""autoWidth"": true },
       
                  
          
            
            ]";
            return View("~/Views/AllReports/AllReportsLateMemo.cshtml");
        }

        [HttpGet]
        public string EmpMonthlyLateMemoData(string branch, string fromdate, string self, string empcode)
        {

            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);
            string monthyear1;
            if (fromdate != "")
            {
                string strDate = fromdate;
                string[] sa = strDate.Split('-');
                monthyear1 = sa[0] + " " + sa[1];
            }
            else
            {
                monthyear1 = "";
            }
            if (branch.Contains("-") && branch != "HeadOffice-All")
            {
                branch = branch.Substring(1);
            }
            else if (branch.Contains(" "))
            {
                branch = branch.TrimStart(' ');
            }
            if (branch.Contains("and"))
            {
                branch = branch.Replace("and", "&");
            }
            var dt = Tbus.EmpMonthlyLateMemoTimesheet(branch, monthyear1, self, EmpIds, empcode);
            return JsonConvert.SerializeObject(dt);
        }
        //Late Attendance Monthly
        public ActionResult MonthlyLateTimesheet(string branch, string fromdate)
        {
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.EmpId = lCredentials.EmpId;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Late Arrivals/Early Departures Monthly";
            ViewBag.ReportFilters = "empmmnthlylatetimereport";
            //ViewBag.PdfSize = "landscape";
            //ViewBag.pageSize = "TABLOID";

            //ViewBag.ExportColumns = "columns: [1,2,3,4,5,6,7,8,9,10,11,12,13]";

            //   ViewBag.DataUrl = "/AllReports/EmpMonthlyLateData";


            var dt = new SqlHelper().Get_Table_FromQry("Select[Id], concat((' '), (Name)) as Name from Branches where Name != 'HeadOffice' and Name!= 'TGCAB-CTI' and Name != 'OtherBranch' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });

            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            if (lCredentials.Branch == "43")
            {
                //var selected4 = (from sub in db.Employes
                //                 where sub.EmpId == lCredentials.EmpId
                //                 select sub.Department).First();

                //ViewBag.DdlOneData1 = new SelectList(items1, "Id", "Name", selected4);
                var deptId = db.Employes
    .Where(sub => sub.EmpId == lCredentials.EmpId)
    .Select(sub => sub.Department)
    .FirstOrDefault();

                var selectedDept = db.Departments
                    .Where(d => d.Id == deptId)
                    .Select(d => d.Name)
                    .FirstOrDefault();
                string formattedSelectedDept = "-" + selectedDept;
                //var selected4 = (from sub in db.Employes
                //                 where sub.EmpId == lCredentials.EmpId
                //                 select sub.Department).First();
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", formattedSelectedDept);

            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
            }

            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");

            if (branch == string.Empty && fromdate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/EmpMonthlyLateData";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/EmpMonthlyLateData?branch=" + branch + "&fromdate=" + fromdate;
            }

            ViewBag.ReportColumns = @"[{""title"": ""Code"", ""data"": ""Empcode"", ""autoWidth"": true },
            {""title"": ""Emp Name"",""data"": ""EmpName"",  ""autoWidth"": true },
        {""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },
            {""title"": ""Branch/Dept"",""data"": ""BrDept"",  ""autoWidth"": true },
            {""title"": ""Month"", ""data"": ""Month"", ""autoWidth"": true },
            {""title"": ""Shift Start Time"",""data"": ""BranchCheckInTime"",  ""autoWidth"": true },
  {""title"": ""Total Late Comes"",""data"": ""TotalLateComes"",  ""autoWidth"": true },
   {""title"": ""Day LA(In-Out)"", ""data"": ""LAIntimeOutTime"", ""autoWidth"": true },
  
 {""title"": ""Shift End Time"",""data"": ""BranchCheckOutTime"",  ""autoWidth"": true },  
  {""title"": ""Total Early Departures"",""data"": ""TotalEarlyDepartures"",  ""autoWidth"": true },
 {""title"": ""Day ED(In-Out)"", ""data"": ""EDIntimeOutTime"", ""autoWidth"": true },
//{""title"": ""Day NA(In-Out)"", ""data"": ""NAIntimeOutTime"", ""autowidth"":true},
         
       
                  
          
            
            ]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string EmpMonthlyLateData(string branch, string fromdate, string self, string empcode)
        {

            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);
            string monthyear1;
            if (fromdate != "")
            {
                string strDate = fromdate;
                string[] sa = strDate.Split('-');
                monthyear1 = sa[0] + " " + sa[1];
            }
            else
            {
                monthyear1 = "";
            }
            if (branch.Contains("-") && branch != "HeadOffice-All")
            {
                branch = branch.Substring(1);
            }
            else if (branch.Contains(" "))
            {
                branch = branch.TrimStart(' ');
            }
            if (branch.Contains("and"))
            {
                branch = branch.Replace("and", "&");
            }
            var dt = Tbus.EmpMonthlyLateTimesheet(branch, monthyear1, self, EmpIds, empcode);
            return JsonConvert.SerializeObject(dt);
        }


        //Employee Monthly Early  Timesheet
        public ActionResult MonthlyEarlyDepature(string branch, string fromdate, string empcode)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Early Departure - Monthly";
            ViewBag.ReportFilters = "empmmnthlyreport";

            //   ViewBag.DataUrl = "/AllReports/MonthlyEarlyDepData";


            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Departments where Active=1");
            var items = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Departments
            {
                Id = 0,
                Name = "All"
            });

            ViewBag.DdlOneData = new SelectList(items, "Id", "Name");

            if (branch == string.Empty && fromdate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/MonthlyEarlyDepData";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/MonthlyEarlyDepData?branch=" + branch + "&fromdate=" + fromdate + "&empcode=" + empcode; ;
            }

            ViewBag.ReportColumns = @"[{""title"": ""Emp Name"", ""data"": ""EmpName"", ""autoWidth"": true },
            {""title"": ""Emp Code"",""data"": ""EmpId"",  ""autoWidth"": true },
            {""title"": ""Branch/Dept"",""data"": ""BrDept"",  ""autoWidth"": true },
            {""title"": ""Month"", ""data"": ""Month"", ""autoWidth"": true },
            {""title"": ""Branch Check Out Time"",""data"": ""BranchCheckOutTime"",  ""autoWidth"": true },
            {""title"": ""Total Early Depatures"",""data"": ""EarlyBy"",  ""autoWidth"": true },
          {""title"": ""Day (In Time - Out Time)"", ""data"": null, ""render"": function ( data, type, LeaveStatus, row ) { return LeaveStatus.EmpCheckInTime + ' -  ' + LeaveStatus.EmpCheckOutTime;}, ""autoWidth"": true }
            ]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string MonthlyEarlyDepData(string branch, string fromdate, string empcode)
        {
            var dt = Tbus.MonthlyEarlyDepatureRes(branch, fromdate, empcode);
            return JsonConvert.SerializeObject(dt);
        }

        // Leaves Applied/Approved but attended duties

        public ActionResult Leavesapprovedbutattended(string branch, string fromdate, string todate)
        {
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Leaves applied/approved but attended duties";
            ViewBag.ReportFilters = "leavesappliedapprovedreport";
            ViewBag.EmpId = lCredentials.EmpId;

            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!= 'TGCAB-CTI' and Name!='OtherBranch' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });

            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            if (lCredentials.Branch == "43")
            {
                var deptId = db.Employes
   .Where(sub => sub.EmpId == lCredentials.EmpId)
   .Select(sub => sub.Department)
   .FirstOrDefault();

                var selectedDept = db.Departments
                    .Where(d => d.Id == deptId)
                    .Select(d => d.Name)
                    .FirstOrDefault();
                string formattedSelectedDept = "-" + selectedDept;
                //var selected4 = (from sub in db.Employes
                //                 where sub.EmpId == lCredentials.EmpId
                //                 select sub.Department).First();
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", formattedSelectedDept);
            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
            }



            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");

            if (branch == string.Empty && fromdate == string.Empty && todate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/LeavesapprovedbutattendedRept";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/LeavesapprovedbutattendedRept?branch=" + branch + "&fromdate=" + fromdate + "&todate=" + todate;
            }

            ViewBag.ReportColumns = @"[{""title"": ""Code"", ""data"": ""EmpId"", ""autoWidth"": true },
            {""title"": ""Emp Name"",""data"": ""EmpName"",  ""autoWidth"": true },
{""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },
            {""title"": ""Branch/Dept"",""data"": ""BrDept"",  ""autoWidth"": true },
            {""title"": ""Applied Date"", ""data"": ""applieddate"", ""autoWidth"": true }, 
            {""title"": ""In Time - Out Time"", ""data"": null, ""render"": function ( data, type, LeaveStatus, row ) { return LeaveStatus.EmpCheckInTime + ' -  ' + LeaveStatus.EmpCheckOutTime;}, ""autoWidth"": true },
            {""title"": ""LeaveType/Status"", ""data"": ""Status"", ""autoWidth"": true }
            ]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }

        [HttpGet]
        public string LeavesapprovedbutattendedRept(string branch, string fromdate, string todate, string self, string empcode)
        {
            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);
            if (branch.Contains("-") && branch != "HeadOffice-All")
            {
                branch = branch.Substring(1);
            }
            else if (branch.Contains(" "))
            {
                branch = branch.TrimStart(' ');
            }
            if (branch.Contains("and"))
            {
                branch = branch.Replace("and", "&");
            }
            var dt = Tbus.LeavesapprovedbutattRept(branch, fromdate, todate, self, EmpIds, empcode);
            return JsonConvert.SerializeObject(dt);
        }
        //Timesheet Manual Approval Report
        public ActionResult TSApprovalViewReport(string branch, string fromdate, string todate, string empcode, string Type)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.EmpId = lCredentials.EmpId;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Timesheet Manual Form";
            ViewBag.ReportFilters = "timesheetmanualreport";
            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";
            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!= 'TGCAB-CTI' and Name!='OtherBranch'    UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });
            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();


            if (lCredentials.Branch == "43")
            {
                var selected4 = (from sub in db.Employes
                                 where sub.EmpId == lCredentials.EmpId
                                 select sub.Department).First();

                ViewBag.DdlOneData1 = new SelectList(items1, "Id", "Name", selected4);

            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
            }

            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");


            if (branch == string.Empty && fromdate == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/timesheetmanualapprovalData";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/timesheetmanualapprovalData?branch=" + branch + "&fromdate=" + fromdate + "&todate=" + todate + "&empcode=" + empcode;
            }

            ViewBag.ReportColumns = @"[{""title"": ""Code"", ""data"": ""EmpId"", ""autoWidth"": true },
            {""title"": ""Emp Name"",""data"": ""Name"",  ""autoWidth"": true },
            {""title"": ""Branch/Dept"",""data"": ""BrDepot"",  ""autoWidth"": true },            
            {""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },          
            {""title"": ""Req Date"",""data"": ""RequestDate"",  ""autoWidth"": true },
            {""title"": ""Entry Time"",""data"": ""entrytime"",  ""autoWidth"": true },
            {""title"": ""Exit Time"",""data"": ""exittime"",  ""autoWidth"": true },
            {""title"": ""Reason Type"",""data"": ""Reason_Type"",  ""autoWidth"": true },
           //// {""title"": ""Description"",""data"": ""Reason_Desc"",  ""autoWidth"": true },
            {""title"": ""Status"",""data"": ""Status"",  ""autoWidth"": true },
            {""title"": ""Approved By"",""data"": ""ApprovedBy"",  ""autoWidth"": true }
            ]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string timesheetmanualapprovalData(string branch, string fromdate, string todate, string empcode, string Type, string self)
        {
            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);
            if (branch.Contains("-") && branch != "HeadOffice-All")
            {
                branch = branch.Substring(1);
            }
            else if (branch.Contains(" "))
            {
                branch = branch.TrimStart(' ');
            }
            if (branch.Contains("and"))
            {
                branch = branch.Replace("and", "&");
            }
            var dt = Tbus.getAllTimesheetApprovals(branch, fromdate, todate, empcode, Type, self, EmpIds);
            return JsonConvert.SerializeObject(dt);
        }
        //Allotment of Shifts Report
        public ActionResult AllotmentofShiftReport(string branch)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Allotment of Shifts";
            ViewBag.ReportFilters = "allotmentshiftreport";
            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[Name] from Branches order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).Where(r => r.Name != "OtherBranch").ToList();

            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });


            ViewBag.Branchess = new SelectList(items, "Id", "Name");

            if (branch == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/AllotmentofShiftData";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/AllotmentofShiftData?branch=" + branch;
            }

            ViewBag.ReportColumns = @"[{""title"": ""Shift Id"", ""data"": ""ShiftId"", ""autoWidth"": true },
            {""title"": ""Branch"", ""data"": ""BrDepot"", ""autoWidth"": true },
            {""title"": ""Department"",""data"": ""Department"",  ""autoWidth"": true },
            {""title"": ""Shift"",""data"": ""ShiftType"",  ""autoWidth"": true },            
            {""title"": ""Start Time"",""data"": ""Starttime"",  ""autoWidth"": true },          
            {""title"": ""End Time"",""data"": ""Endtime"",  ""autoWidth"": true },
            {""title"": ""Grace Period"",""data"": ""Graceperiod"",  ""autoWidth"": true }
            ]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string AllotmentofShiftData(string branch)
        {
            if (branch == "42")
            {
                branch = "43";
            }
            var dt = Tbus.getAllAllotmentofshifts(branch);
            return JsonConvert.SerializeObject(dt);
        }
        //Early Departure Detials Monthly Report
        public ActionResult EarlyDepartureDetailsMonthlyReport(string branch, string monthyear)
        {
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            ViewBag.EmpId = lCredentials.EmpId;
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Early Departure Details Monthly";
            ViewBag.ReportFilters = "earlydeparturedetailsmonthly";
            //ViewBag.PdfSize = "landscape";
            //ViewBag.pageSize = "TABLOID";

            //ViewBag.ExportColumns = "columns: [1,2,3,4,5,6,7]";
            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],concat((' '),(Name)) as Name from Branches where Name!='HeadOffice' and Name!= 'TGCAB-CTI' and Name!='OtherBranch' UNION Select[Id], concat(('-'), (Name)) as Name from Departments d where Name!='OtherDepartment' and Active=1 order by Name");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(0, new Branches
            {
                Id = 0,
                Name = "All"
            });

            var items1 = dt.AsEnumerable().Select(r => new Departments
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            items.Insert(45, new Branches
            {
                Id = 45,
                Name = "HeadOffice-All"
            });

            if (lCredentials.Branch == "43")
            {
                var selected4 = (from sub in db.Employes
                                 where sub.EmpId == lCredentials.EmpId
                                 select sub.Department).First();

                ViewBag.DdlOneData1 = new SelectList(items1, "Id", "Name", selected4);

            }
            else
            {
                string brname = " " + lCredentials.BranchName.Replace(" Br", "");
                ViewBag.DdlOneData1 = new SelectList(items, "Name", "Name", brname);
            }
            ViewBag.DdlOneData = new SelectList(items, "Name", "Name");

            if (branch == string.Empty)
            {
                ViewBag.DataUrl = "/AllReports/EarlyDepartureDetailsMonthlyData";
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/EarlyDepartureDetailsMonthlyData?branch=" + branch + "&monthyear=" + monthyear; ;
            }

            ViewBag.ReportColumns = @"[{""title"": ""Code"", ""data"": ""Empcode"", ""autoWidth"": true },
            {""title"": ""Emp Name"",""data"": ""EmpName"",  ""autoWidth"": true },
 {""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },
            {""title"": ""Branch/Dept"",""data"": ""BrDept"",  ""autoWidth"": true },  
            {""title"": ""Month"",""data"": ""Month"",  ""autoWidth"": true },  
            {""title"": ""Shift End Time"",""data"": ""BranchCheckOutTime"",  ""autoWidth"": true },          
            {""title"": ""TotalEarlyDepartures"",""data"": ""TotalEarlyDepartures"",  ""autoWidth"": true },
            {""title"": ""Day (In Time - Out Time)"", ""data"": ""IntimeOutTime"",  ""autoWidth"": true }
           
            ]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string EarlyDepartureDetailsMonthlyData(string branch, string monthyear, string self, string empcode)
        {
            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);
            string monthyear1;
            if (monthyear != "")
            {
                string strDate = monthyear;
                string[] sa = strDate.Split('-');
                monthyear1 = sa[0] + " " + sa[1];
            }
            else
            {
                monthyear1 = "";
            }
            if (branch.Contains("-") && branch != "HeadOffice-All")
            {
                branch = branch.Substring(1);
            }
            else if (branch.Contains(" "))
            {
                branch = branch.TrimStart(' ');
            }
            if (branch.Contains("and"))
            {
                branch = branch.Replace("and", "&");
            }
            var dt = Tbus.getAllEarlyDeparture(branch, monthyear1, self, EmpIds, empcode);
            return JsonConvert.SerializeObject(dt);
        }
        //Employee Monthly Report
        public ActionResult EmpMonthlyReport(string branch, string monthyear)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;


            ViewBag.ReportFilters = "timesheetreports";
            ViewBag.ReportTitle = "TimeSheet Rerun";
            var dt = new SqlHelper().Get_Table_FromQry("select Id,BranchName from Branch_Device order by BranchName");
            var items = dt.AsEnumerable().Select(r => new Branch_Device
            {
                Id = (Int32)(r["Id"]),
                BranchName = (string)(r["BranchName"] ?? "null")
            }).ToList();

            //items.Insert(0, new Branch_Device
            //{
            // Id = 0,
            // BranchName = "All"
            // });

            ViewBag.Branchess = new SelectList(items, "BranchName", "BranchName");

            if (string.IsNullOrEmpty(branch) && string.IsNullOrEmpty(monthyear))
            {
                ViewBag.DataUrl = "/AllReports/EmpMonthlyreportS?branch=All &monthyear=" + monthyear;
            }
            else
            {
                ViewBag.DataUrl = "/AllReports/EmpMonthlyreportS?branch=" + branch + "&monthyear=" + monthyear;
            }
            ViewBag.ReportColumns = @"[
{""title"": ""Month Year"", ""data"": ""monthyear"", ""autoWidth"": true },
{""title"": ""BranchName"", ""data"": ""BranchName"", ""autoWidth"": true },


{""title"": ""D1"", ""data"": ""D1"", ""autoWidth"": true},
     
{""title"": ""D2"",""data"": ""D2"",  ""autoWidth"": true },
{""title"": ""D3"",""data"": ""D3"",  ""autoWidth"": true },
{""title"": ""D4"",""data"": ""D4"",  ""autoWidth"": true },
{""title"": ""D5"",""data"": ""D5"",  ""autoWidth"": true },
{""title"": ""D6"",""data"": ""D6"",  ""autoWidth"": true },
{""title"": ""D7"",""data"": ""D7"",  ""autoWidth"": true },
{""title"": ""D8"",""data"": ""D8"",  ""autoWidth"": true },
{""title"": ""D9"",""data"": ""D9"",  ""autoWidth"": true },
{""title"": ""D10"",""data"": ""D10"",  ""autoWidth"": true },
{""title"": ""D11"",""data"": ""D11"",  ""autoWidth"": true },
{""title"": ""D12"",""data"": ""D12"",  ""autoWidth"": true },
{""title"": ""D13"",""data"": ""D13"",  ""autoWidth"": true },
{""title"": ""D14"",""data"": ""D14"",  ""autoWidth"": true },
{""title"": ""D15"",""data"": ""D15"",  ""autoWidth"": true },
{""title"": ""D16"",""data"": ""D16"",  ""autoWidth"": true },
{""title"": ""D17"",""data"": ""D17"",  ""autoWidth"": true },
{""title"": ""D18"",""data"": ""D18"",  ""autoWidth"": true },
{""title"": ""D19"",""data"": ""D19"",  ""autoWidth"": true },
{""title"": ""D20"",""data"": ""D20"",  ""autoWidth"": true },
{""title"": ""D21"",""data"": ""D21"",  ""autoWidth"": true },
{""title"": ""D22"",""data"": ""D22"",  ""autoWidth"": true },
{""title"": ""D23"",""data"": ""D23"",  ""autoWidth"": true },
{""title"": ""D24"",""data"": ""D24"",  ""autoWidth"": true },
{""title"": ""D25"",""data"": ""D25"",  ""autoWidth"": true },
{""title"": ""D26"",""data"": ""D26"",  ""autoWidth"": true },

{""title"": ""D27"",""data"": ""D27"",  ""autoWidth"": true },
{""title"": ""D28"",""data"": ""D28"",  ""autoWidth"": true },
{""title"": ""D29"",""data"": ""D29"",  ""autoWidth"": true },
{""title"": ""D30"",""data"": ""D30"",  ""autoWidth"": true },
{""title"": ""D31"",""data"": ""D31"",  ""autoWidth"": true }
            ]";
            return View("~/Views/AllReports/TimesheetRerun.cshtml");
        }

        public ActionResult Insertdata(string branch, string day, string monthyear)
        {
            //TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            try
            {
                int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();

                Tbus.TimesheetRerunInsert(branch, day, monthyear);

                return RedirectToAction("EmpMonthlyReport");
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return RedirectToAction("EmpMonthlyReport");
        }
        [HttpGet]
        public string EmpMonthlyreportS(string branch, string monthyear)
        {
            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);
            string monthyear1;
            if (monthyear != "")
            {
                string strDate = monthyear;
                string[] sa = strDate.Split('-');
                monthyear1 = sa[0] + " " + sa[1];
            }
            else
            {
                monthyear1 = "";
            }
            //if (branch.Contains("-") && branch != "HeadOffice-All")
            //{
            //    branch = branch.Substring(1);
            //}
            //else if (branch.Contains(" "))
            //{
            //    branch = branch.TrimStart(' ');
            //}
            //if (branch.Contains("and"))
            //{
            //    branch = branch.Replace("and", "&");
            //}
            var dtEmps = Tbus.getallempmontlysearch(branch, monthyear);
            var ft = JsonConvert.SerializeObject(dtEmps);
            return ft;
        }
        [HttpGet]
        public string EmpMonthlyreportSearchData(string branch, string monthyear)
        {

            if (monthyear == null)
            {
                monthyear = "";
            }
            if (branch.Contains("HO-"))
            {
                branch = branch.Substring(3);
            }
            var dtEmps = Tbus.getallempmontlysearch(branch, monthyear);

            IList<EmpTsMonthlyReportDTO> lstDto = new List<EmpTsMonthlyReportDTO>();
            int rowid = 0;
            foreach (DataRow dremp in dtEmps.Rows)
            {
                rowid++;
                //fill user info row
                var userinfo = new EmpTsMonthlyReportDTO();
                userinfo.RowId = rowid;
                userinfo.col1 = "Days";
                userinfo.D1 = "D1";
                userinfo.D2 = "D2";
                userinfo.D3 = "D3";
                userinfo.D4 = "D4";
                userinfo.D5 = "D5";
                userinfo.D6 = "D6";
                userinfo.D7 = "D7";
                userinfo.D8 = "D8";
                userinfo.D9 = "D9";
                userinfo.D10 = "D10";
                userinfo.D11 = "D11";
                userinfo.D12 = "D12";
                userinfo.D13 = "D13";
                userinfo.D14 = "D14";
                userinfo.D15 = "D15";
                userinfo.D16 = "D16";
                userinfo.D17 = "D17";
                userinfo.D18 = "D18";
                userinfo.D19 = "D19";
                userinfo.D20 = "D20";
                userinfo.D21 = "D21";
                userinfo.D22 = "D22";
                userinfo.D23 = "D23";
                userinfo.D24 = "D24";
                userinfo.D25 = "D25";
                userinfo.D26 = "D26";
                userinfo.D27 = "D27";
                userinfo.D28 = "D28";
                userinfo.D28 = "D28";
                userinfo.D29 = "D29";
                userinfo.D30 = "D30";
                userinfo.D31 = "D31";
                userinfo.Userinfo = dremp["Userinfo"].ToString();
                lstDto.Add(userinfo);

                // fill days row
                //var userdays = new EmpTsMonthlyReportDTO();
                //userdays.RowId = rowid;
                //userdays.col1 = "Days";
                //userdays.Userinfo = dremp["Userinfo"].ToString();
                //userdays.Userinfo = dremp["Userinfo"].ToString();
                //userdays.D1 = "D1";
                //userdays.D2 = "D2";
                //userdays.D3 = "D3";
                //userdays.D4 = "D4";
                //userdays.D5 = "D5";
                //userdays.D6 = "D6";
                //userdays.D7 = "D7";
                //userdays.D8 = "D8";
                //userdays.D9 = "D9";
                //userdays.D10 = "D10";
                //userdays.D11 = "D11";
                //userdays.D12 = "D12";
                //userdays.D13 = "D13";
                //userdays.D14 = "D14";
                //userdays.D15 = "D15";
                //userdays.D16 = "D16";
                //userdays.D17 = "D17";
                //userdays.D18 = "D18";
                //userdays.D19 = "D19";
                //userdays.D20 = "D20";
                //userdays.D21 = "D21";
                //userdays.D22 = "D22";
                //userdays.D23 = "D23";
                //userdays.D24 = "D24";
                //userdays.D25 = "D25";
                //userdays.D26 = "D26";
                //userdays.D27 = "D27";
                //userdays.D28 = "D28";
                //userdays.D28 = "D28";
                //userdays.D29 = "D29";
                //userdays.D30 = "D30";
                //userdays.D31 = "D31";


                //lstDto.Add(userdays);

                var userin = new EmpTsMonthlyReportDTO();
                userin.RowId = rowid;
                userin.col1 = "In";
                userin.Userinfo = dremp["Userinfo"].ToString();
                userin.Userinfo = dremp["Userinfo"].ToString();

                var userout = new EmpTsMonthlyReportDTO();
                userout.RowId = rowid;
                userout.col1 = "Out";
                userout.Userinfo = dremp["Userinfo"].ToString();
                userout.Userinfo = dremp["Userinfo"].ToString();

                var usersts = new EmpTsMonthlyReportDTO();
                usersts.RowId = rowid;
                usersts.col1 = "Status";
                usersts.Userinfo = dremp["Userinfo"].ToString();
                usersts.Userinfo = dremp["Userinfo"].ToString();

                setEmpINOutStatusFromDbString(dremp, userin, userout, usersts);

                lstDto.Add(userin);
                lstDto.Add(userout);
                lstDto.Add(usersts);
            }
            return JsonConvert.SerializeObject(lstDto);
        }


        private void setEmpINOutStatusFromDbString(DataRow dremp, EmpTsMonthlyReportDTO uin, EmpTsMonthlyReportDTO uout, EmpTsMonthlyReportDTO usta)
        {
            //10:20#17:41#07:21:00#LA#00:40:00
            if (dremp["D1"].ToString() != "")
            {
                var arr = dremp["D1"].ToString().Split('#');
                uin.D1 = arr[0];
                uout.D1 = arr[1];
                usta.D1 = arr[3];
            }
            if (dremp["D2"].ToString() != "")
            {
                var arr = dremp["D2"].ToString().Split('#');
                uin.D2 = arr[0];
                uout.D2 = arr[1];
                usta.D2 = arr[3];
            }
            if (dremp["D3"].ToString() != "")
            {
                var arr = dremp["D3"].ToString().Split('#');
                uin.D3 = arr[0];
                uout.D3 = arr[1];
                usta.D3 = arr[3];
            }
            if (dremp["D4"].ToString() != "")
            {
                var arr = dremp["D4"].ToString().Split('#');
                uin.D4 = arr[0];
                uout.D4 = arr[1];
                usta.D4 = arr[3];
            }
            if (dremp["D5"].ToString() != "")
            {
                var arr = dremp["D5"].ToString().Split('#');
                uin.D5 = arr[0];
                uout.D5 = arr[1];
                usta.D5 = arr[3];
            }
            if (dremp["D6"].ToString() != "")
            {
                var arr = dremp["D6"].ToString().Split('#');
                uin.D6 = arr[0];
                uout.D6 = arr[1];
                usta.D6 = arr[3];
            }
            if (dremp["D7"].ToString() != "")
            {
                var arr = dremp["D7"].ToString().Split('#');
                uin.D7 = arr[0];
                uout.D7 = arr[1];
                usta.D7 = arr[3];
            }
            if (dremp["D8"].ToString() != "")
            {
                var arr = dremp["D8"].ToString().Split('#');
                uin.D8 = arr[0];
                uout.D8 = arr[1];
                usta.D8 = arr[3];
            }
            if (dremp["D9"].ToString() != "")
            {
                var arr = dremp["D9"].ToString().Split('#');
                uin.D9 = arr[0];
                uout.D9 = arr[1];
                usta.D9 = arr[3];
            }
            if (dremp["D10"].ToString() != "")
            {
                var arr = dremp["D10"].ToString().Split('#');
                uin.D10 = arr[0];
                uout.D10 = arr[1];
                usta.D10 = arr[3];
            }
            if (dremp["D11"].ToString() != "")
            {
                var arr = dremp["D11"].ToString().Split('#');
                uin.D11 = arr[0];
                uout.D11 = arr[1];
                usta.D11 = arr[3];
            }
            if (dremp["D12"].ToString() != "")
            {
                var arr = dremp["D12"].ToString().Split('#');
                uin.D12 = arr[0];
                uout.D12 = arr[1];
                usta.D12 = arr[3];
            }
            if (dremp["D13"].ToString() != "")
            {
                var arr = dremp["D13"].ToString().Split('#');
                uin.D13 = arr[0];
                uout.D13 = arr[1];
                usta.D13 = arr[3];
            }
            if (dremp["D14"].ToString() != "")
            {
                var arr = dremp["D14"].ToString().Split('#');
                uin.D14 = arr[0];
                uout.D14 = arr[1];
                usta.D14 = arr[3];
            }
            if (dremp["D15"].ToString() != "")
            {
                var arr = dremp["D15"].ToString().Split('#');
                uin.D15 = arr[0];
                uout.D15 = arr[1];
                usta.D15 = arr[3];
            }
            if (dremp["D16"].ToString() != "")
            {
                var arr = dremp["D16"].ToString().Split('#');
                uin.D16 = arr[0];
                uout.D16 = arr[1];
                usta.D16 = arr[3];
            }
            if (dremp["D17"].ToString() != "")
            {
                var arr = dremp["D17"].ToString().Split('#');
                uin.D17 = arr[0];
                uout.D17 = arr[1];
                usta.D17 = arr[3];
            }
            if (dremp["D18"].ToString() != "")
            {
                var arr = dremp["D18"].ToString().Split('#');
                uin.D18 = arr[0];
                uout.D18 = arr[1];
                usta.D18 = arr[3];
            }
            if (dremp["D19"].ToString() != "")
            {
                var arr = dremp["D19"].ToString().Split('#');
                uin.D19 = arr[0];
                uout.D19 = arr[1];
                usta.D19 = arr[3];
            }
            if (dremp["D20"].ToString() != "")
            {
                var arr = dremp["D20"].ToString().Split('#');
                uin.D20 = arr[0];
                uout.D20 = arr[1];
                usta.D20 = arr[3];
            }
            if (dremp["D21"].ToString() != "")
            {
                var arr = dremp["D21"].ToString().Split('#');
                uin.D21 = arr[0];
                uout.D21 = arr[1];
                usta.D21 = arr[3];
            }
            if (dremp["D22"].ToString() != "")
            {
                var arr = dremp["D22"].ToString().Split('#');
                uin.D22 = arr[0];
                uout.D22 = arr[1];
                usta.D22 = arr[3];
            }
            if (dremp["D23"].ToString() != "")
            {
                var arr = dremp["D23"].ToString().Split('#');
                uin.D23 = arr[0];
                uout.D23 = arr[1];
                usta.D23 = arr[3];
            }
            if (dremp["D24"].ToString() != "")
            {
                var arr = dremp["D24"].ToString().Split('#');
                uin.D24 = arr[0];
                uout.D24 = arr[1];
                usta.D24 = arr[3];
            }
            if (dremp["D25"].ToString() != "")
            {
                var arr = dremp["D25"].ToString().Split('#');
                uin.D25 = arr[0];
                uout.D25 = arr[1];
                usta.D25 = arr[3];
            }
            if (dremp["D26"].ToString() != "")
            {
                var arr = dremp["D26"].ToString().Split('#');
                uin.D26 = arr[0];
                uout.D26 = arr[1];
                usta.D26 = arr[3];
            }
            if (dremp["D27"].ToString() != "")
            {
                var arr = dremp["D27"].ToString().Split('#');
                uin.D27 = arr[0];
                uout.D27 = arr[1];
                usta.D27 = arr[3];
            }
            if (dremp["D28"].ToString() != "")
            {
                var arr = dremp["D28"].ToString().Split('#');
                uin.D28 = arr[0];
                uout.D28 = arr[1];
                usta.D28 = arr[3];
            }
            if (dremp["D29"].ToString() != "")
            {
                var arr = dremp["D29"].ToString().Split('#');
                uin.D29 = arr[0];
                uout.D29 = arr[1];
                usta.D29 = arr[3];
            }
            if (dremp["D30"].ToString() != "")
            {
                var arr = dremp["D30"].ToString().Split('#');
                uin.D30 = arr[0];
                uout.D30 = arr[1];
                usta.D30 = arr[3];
            }
            if (dremp["D31"].ToString() != "")
            {
                var arr = dremp["D31"].ToString().Split('#');
                uin.D31 = arr[0];
                uout.D31 = arr[1];
                usta.D31 = arr[3];
            }

        }


        // for covid 19
        public ActionResult Covid19()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "";


            ViewBag.ReportTitle = "COVID-19 Report As on Date";
            ViewBag.DataUrl = "/AllReports/covid19Data";
            ViewBag.ReportColumns = @"[{""title"": ""EmpId"",""data"": ""col3"",  ""autoWidth"": true,""visible"": true },
            {""title"": ""Name"", ""data"": ""col5"", ""autoWidth"": true },
            {""title"": ""Gender"",""data"": ""col6"", ""autoWidth"": true },
            {""title"": ""Age"",""data"": ""col7"",  ""autoWidth"": true },
            {""title"": ""Relationship"",""data"": ""col8"", ""autoWidth"": true },
            {""title"": ""Address"",""data"": ""col9"", ""autoWidth"": true },
            {""title"": ""Diabetes"", ""data"": ""col10"", ""autoWidth"": true },
            {""title"": ""Hypertension"",""data"": ""col11"", ""autoWidth"": true },
            {""title"": ""Quarantine"",""data"": ""col12"",  ""autoWidth"": true },
            {""title"": ""Complaints"",""data"": ""col13"", ""autoWidth"": true }]";
            //ViewBag.ReportColumns = @"[{""title"": """",""data"": ""Report"",  ""autoWidth"": true }]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string covid19Data()
        {
            var dt = Rbus.covid19DataList();
            return JsonConvert.SerializeObject(dt);
        }

        // for covid 19 form Submitted or Not Submitted
        public ActionResult Covid19Submited()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "OneddlSearch";
            ViewBag.DdlOneLabel = "COVID-19 Survay";

            ViewBag.ReportTitle = "COVID-19 Employee Report ";
            ViewBag.DataUrl = "/AllReports/Covid19SubmitedData/ddlVal=-1";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Id"",""data"": ""EmpID"",  ""autoWidth"": true },
            {""title"": ""Emp Name"", ""data"": ""Name"", ""autoWidth"": true },
            {""title"": ""Designation"",""data"": ""Designation"", ""autoWidth"": true },
            {""title"": ""Dept/Branch"",""data"": ""Branch"",  ""autoWidth"": true }]";
            //ViewBag.ReportColumns = @"[{""title"": """",""data"": ""Report"",  ""autoWidth"": true }]";
            return View("~/Views/AllReports/AllReports.cshtml");
            // return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string Covid19SubmitedData(string ddlVal)
        {
            var dt = Rbus.covid19DataList(ddlVal);
            return JsonConvert.SerializeObject(dt);
        }

        //DCCB  Employees by Sowjanya
        public ActionResult DCCBEmployeeTimesheet(string fromdate, string todate, string empcode)
        {
            string lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.EmpId = lCredentials.EmpId;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "DCCB Employee Timesheet";
            ViewBag.ReportFilters = "DCCBEmployeeSrch";
            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";
            //ViewBag.DataUrl = "/AllReports/EmployeeTimesheetdata";


            ViewBag.DataUrl = "/AllReports/DCCBEmployeeTimesheetdata?&fromdate=" + fromdate + "&todate=" + todate + "&empcode=" + empcode;


            ViewBag.ReportColumns = @"[{""title"": ""Date"",""data"": ""mydate"",  ""autoWidth"": true,  'render': function (jsonDate) {  
                            var date = new Date(jsonDate);  
                            var month = (""0""+ (date.getMonth() + 1)).slice(-2);  
                            return date.getFullYear() + '-' + month + '-' + (""0"" + date.getDate()).slice(-2)
        }  },
            { ""title"": ""Code"", ""data"": ""empid"", ""autoWidth"": true },
            {""title"": ""Emp Name"",""data"": ""empname"",  ""autoWidth"": true },
            {""title"": ""Designation"",""data"": ""designation"",  ""autoWidth"": true }, 
            {""title"": ""District"",""data"": ""district"",  ""autoWidth"": true },    
            {""title"": ""Emp. In Time"",""data"": ""empcheckintime"",  ""autoWidth"": true },
            {""title"": ""Emp. Out Time"",""data"": ""EmpCheckOutTime"",  ""autoWidth"": true },            
            ]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string DCCBEmployeeTimesheetdata(string fromdate, string todate, string empcode)
        {

            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);

            var dt = Tbus.DCCBEmployeeTimesheetMstdata(fromdate, todate, empcode);
            return JsonConvert.SerializeObject(dt);
        }

        //duplicate report dhana
        public ActionResult DCCBEmployeeTimesheet1(string fromdate, string todate, string empcode)
        {
            string lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.EmpId = lCredentials.EmpId;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "DCCB Employee Timesheet Report1Duplicate";
            ViewBag.ReportFilters = "DEmployeeSrchrpt";
            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";
            //ViewBag.DataUrl = "/AllReports/EmployeeTimesheetdata";


            ViewBag.DataUrl = "/AllReports/DCCBEmployeeTimesheetdata1?&fromdate=" + fromdate + "&todate=" + todate + "&empcode=" + empcode;


            ViewBag.ReportColumns = @"[{""title"": ""Date"",""data"": ""mydate"",  ""autoWidth"": true,  'render': function (jsonDate) {  
                            var date = new Date(jsonDate);  
                            var month = (""0""+ (date.getMonth() + 1)).slice(-2);  
                            return date.getFullYear() + '-' + month + '-' + (""0"" + date.getDate()).slice(-2)
        }  },
            { ""title"": ""Code"", ""data"": ""empid"", ""autoWidth"": true },
            {""title"": ""Emp Name"",""data"": ""empname"",  ""autoWidth"": true },
            {""title"": ""Designation"",""data"": ""designation"",  ""autoWidth"": true }, 
            {""title"": ""District"",""data"": ""district"",  ""autoWidth"": true },    
            {""title"": ""Emp. In Time"",""data"": ""empcheckintime"",  ""autoWidth"": true },
            {""title"": ""Emp. Out Time"",""data"": ""EmpCheckOutTime"",  ""autoWidth"": true },            
            ]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }
        [HttpGet]
        public string DCCBEmployeeTimesheetdata1(string fromdate, string todate, string empcode)
        {

            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);

            var dt = Tbus.DCCBEmployeeTimesheetMstdata(fromdate, todate, empcode);
            return JsonConvert.SerializeObject(dt);
        }




        //[WebMethod]
        //public DataTable Get()
        //{
        //    //string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //    SqlConnection con = new SqlConnection("Server=183.82.100.162; Database=sssdb_27may2022; user id=sa; password=Mavensoft;");
        //    using (con)
        //    {
        //        using (SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_realtime_glog"))
        //        {
        //            using (SqlDataAdapter sda = new SqlDataAdapter())
        //            {
        //                cmd.Connection = con;
        //                sda.SelectCommand = cmd;
        //                using (DataTable dt = new DataTable())
        //                {
        //                    dt.TableName = "tbl_realtime_glog";
        //                    sda.Fill(dt);
        //                    Session["sssdata"] = dt;
        //                    return dt;
        //                }
        //            }
        //        }
        //    }
        //}


        public ActionResult GovernmentStaffEmployeeTimesheet(string fromdate, string todate, string empcode)
        {
            string lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            int role = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Role).FirstOrDefault();
            ViewBag.Role = role.ToString();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.EmpId = lCredentials.EmpId;
            ViewBag.SearchBtn = "false";
            ViewBag.ReportTitle = "Government Staff Employee Timesheet";
            ViewBag.ReportFilters = "GovernmentstaffEmployeeSrch";
            ViewBag.PdfSize = "landscape";
            ViewBag.pageSize = "LEGAL";
            //ViewBag.DataUrl = "/AllReports/EmployeeTimesheetdata";


            ViewBag.DataUrl = "/AllReports/GovernmentStaffEmployeeTimesheetdata?&fromdate=" + fromdate + "&todate=" + todate + "&empcode=" + empcode;


            ViewBag.ReportColumns = @"[{""title"": ""Date"",""data"": ""mydate"",  ""autoWidth"": true,  'render': function (jsonDate) {  
                            var date = new Date(jsonDate);  
                            var month = (""0""+ (date.getMonth() + 1)).slice(-2);  
                            return date.getFullYear() + '-' + month + '-' + (""0"" + date.getDate()).slice(-2)
        }  },
            { ""title"": ""Code"", ""data"": ""empid"", ""autoWidth"": true },
            {""title"": ""Emp Name"",""data"": ""name"",  ""autoWidth"": true },
            {""title"": ""Designation"",""data"": ""designation"",  ""autoWidth"": true }, 
            {""title"": ""Branch"",""data"": ""branch"",  ""autoWidth"": true },    
            {""title"": ""Emp. In Time"",""data"": ""empcheckintime"",  ""autoWidth"": true },
            {""title"": ""Emp. Out Time"",""data"": ""empcheckouttime"",  ""autoWidth"": true },            
            ]";
            return View("~/Views/AllReports/AllReports.cshtml");
        }

        public string GovernmentStaffEmployeeTimesheetdata(string fromdate, string todate, string empcode)
        {

            int lEmpId1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            string lEmpId = lEmpId1.ToString();
            int EmpIds = Convert.ToInt32(lCredentials.EmpId);

            var dt = Tbus.GovernmentStaffEmployeeTimesheetMstdata(fromdate, todate, empcode);
            return JsonConvert.SerializeObject(dt);
        }
    }
}