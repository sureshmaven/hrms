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

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class TimeSheetController : Controller
    {
        private ContextBase db = new ContextBase();

        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        TimesheetBusiness Tbus = new TimesheetBusiness();
        SqlHelper sh = new SqlHelper();
        //timesheet request form
        [HttpGet]
        public ActionResult Timesheetrequestform()
        {

            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.ODLeaveTypes = new SelectList(Tbus.getAllODLeaveTypes(), "Type", "Type");
            return View("~/Views/Timesheet/Timesheetrequestform.cshtml");
        }
        [HttpGet]
        public JsonResult GetAuthorityNamess(string Name)
        {
            string lresult = string.Empty;
            try
            {
                var employees = db.Employes.ToList();
                // var dbResult = db.Leaves.ToList();
                int lUserLoginId = employees.Where(a => a.EmpId.ToLower() == Name).Select(a => a.Id).FirstOrDefault();
                if (string.IsNullOrEmpty(Name))
                {
                    var lResult = (from userslist in employees
                                   where userslist.Id == lUserLoginId
                                   select new
                                   {

                                       userslist.ControllingAuthority,
                                       userslist.SanctioningAuthority,
                                       userslist.EmpId,

                                   });
                    var lresponseArray = lResult.ToArray();

                    string lControllingAuthority = lresponseArray[0].ControllingAuthority;
                    string lSanctioningAuthority = lresponseArray[0].SanctioningAuthority;
                    string luserid = lresponseArray[0].EmpId;
                    int lcontrol = Convert.ToInt32(lControllingAuthority);
                    int lsancationcontrol = Convert.ToInt32(lSanctioningAuthority);

                    Session["lcontrols"] = lcontrol;
                    Session["lSancation"] = lsancationcontrol;

                    Employees lcontrolling = Facade.EntitiesFacade.GetEmpTabledata.GetById(lcontrol);
                    Employees lsancationing = Facade.EntitiesFacade.GetEmpTabledata.GetById(lsancationcontrol);

                    string empid = luserid;
                    return Json(new { lControllingAuthorityAjax = lcontrolling.ShortName, lSanctioningAuthorityAjax = lsancationing.ShortName, luseridAjax = empid }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var lResults = (from userslist in employees
                                    where userslist.Id == lUserLoginId
                                    select new
                                    {
                                        userslist.ControllingAuthority,
                                        userslist.SanctioningAuthority,
                                        userslist.EmpId,
                                    });
                    var lresponseArray = lResults.Distinct().ToArray();

                    string lControllingAuthority = lresponseArray[0].ControllingAuthority;
                    string lSanctioningAuthority = lresponseArray[0].SanctioningAuthority;
                    string empid = lresponseArray[0].EmpId;
                    return Json(new { lControllingAuthorityAjax = lControllingAuthority, lSanctioningAuthorityAjax = lSanctioningAuthority, luseridAjax = empid }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;

        }

        public FileResult CreatePdfTS()
        {
            String lstartdate = Convert.ToString(Session["sd"]);
            String lenddate = Convert.ToString(Session["ed"]);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("Requesthistory" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(20f, 20f, 20f, 20f);
            //Create PDF Table with 4 columns  
            PdfPTable tableLayout1 = new PdfPTable(4);
            PdfPTable tableLayout = new PdfPTable(5);
            doc.SetMargins(20f, 20f, 20f, 20f);
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF
            doc.Add(Add_Content_To_PDF1(tableLayout1));
            //Add Content to PDF   
            doc.Add(Add_Content_To_PDF(tableLayout, lstartdate, lenddate));
            // Closing the document  
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return File(workStream, "application/pdf", strPDFFileName);
        }

        protected PdfPTable Add_Content_To_PDF1(PdfPTable tableLayout1)
        {
            float[] headers1 = { 33, 33, 33, 33 }; //Header Widths  
            tableLayout1.SetWidths(headers1); //Set the pdf headers  
            tableLayout1.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout1.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            tableLayout1.AddCell(new PdfPCell(new Phrase("Request History", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            int EmployeeId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int Empid = Convert.ToInt32(EmployeeId);
            var dbresult = db.Timesheet_Request_Form.ToList();
            var lEmployees = db.Employes.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();

            var lod = db.Timesheet_Request_Form.ToList();
            var ldesignation1 = db.Designations.ToList();
            var lemployees = db.Employes.ToList();
            var lbranch = db.Branches.ToList();
            var ldepartment = db.Departments.ToList();
            var lResults = (from OD in lod
                            join emp in lemployees on OD.UserId equals emp.Id
                            join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                            join branch in lbranch on emp.Branch equals branch.Id
                            join dept in ldepartment on emp.Department equals dept.Id
                            where lCredentials.EmpId == emp.EmpId

                            select new
                            {
                                EmpCode = emp.EmpId,
                                EmployeeName = emp.ShortName,
                                designation = desig.Code,
                                Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                            }).Distinct();
            //Adding headers  
            AddCellToHeader(tableLayout1, "EmpId");
            AddCellToHeader(tableLayout1, "EmpName");
            AddCellToHeader(tableLayout1, "Designation");
            AddCellToHeader(tableLayout1, "Department");


            //Adding body  
            foreach (var lemp in lResults)
            {
                AddCellToBody(tableLayout1, lemp.EmpCode.ToString());
                AddCellToBody(tableLayout1, lemp.EmployeeName.ToString());
                AddCellToBody(tableLayout1, lemp.designation.ToString());
                AddCellToBody(tableLayout1, lemp.Deptbranch.ToString());
            }
            return tableLayout1;
        }

           protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, string StartDate, string EndDate)
        {
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            float[] headers = { 50, 22, 25, 25 ,25}; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            List<Leaves> lleaves = db.Leaves.ToList<Leaves>();
            int EmployeeId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int Empid = Convert.ToInt32(EmployeeId);
            var dbresult = db.Timesheet_Request_Form.ToList();
            var lEmployees = db.Employes.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            if (StartDate == "" || EndDate == "")
            {
                var data = (from ts in dbresult
                            join employee in lEmployees on ts.UserId equals employee.Id
                            join branches in lBranches on ts.BranchId equals branches.Id
                            join dept in ldept on ts.DepartmentId equals dept.Id
                            join desig in ldesignation on ts.DesignationId equals desig.Id
                            where ts.UserId == EmployeeId

                            select new
                            {
                                ts.Id,
                                employee.EmpId,
                                EmployeeName = employee.ShortName,
                                ts.ReqFromDate,
                                ts.Reason_Type,
                                ts.Reason_Desc,
                                designation = desig.Code,
                                AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                ts.Status
                            }).OrderByDescending(A => A.AppliedDate);
                //Add header       
                AddCellToHeader(tableLayout, "Applied Date");
                AddCellToHeader(tableLayout, "Request Date");
                AddCellToHeader(tableLayout, "Reason Type");
                //AddCellToHeader(tableLayout, "Description");
                AddCellToHeader(tableLayout, "Status");

                //Add body  
                foreach (var ts in data)
                {
                    AddCellToBody(tableLayout, ts.AppliedDate);
                    AddCellToBody(tableLayout, ts.ReqFromDate.ToString("dd/MM/yy"));
                    AddCellToBody(tableLayout, ts.Reason_Type);
                   // AddCellToBody(tableLayout, ts.Reason_Desc);
                    AddCellToBody(tableLayout, ts.Status);
                }

                return tableLayout;
            }
            else
            {
                DateTime ToDate = Convert.ToDateTime(EndDate);
                string strDate = StartDate;
                string[] sa = strDate.Split('/');
                string strNew = sa[2] + "/" + sa[1] + "/" + sa[0];
                string strDate1 = EndDate;
                string[] sa1 = strDate1.Split('/');
                string strNew1 = sa1[2] + "/" + sa1[1] + "/" + sa1[0];
                DateTime FromDate = DateTime.ParseExact(strNew, "yyyy/MM/dd", null);
                DateTime Todate = DateTime.ParseExact(strNew1, "yyyy/MM/dd", null);
                DateTime lStartdate = FromDate;
                DateTime lEnddate = ToDate;
                var data = (from ts in dbresult
                            join employee in lEmployees on ts.UserId equals employee.Id
                            join branches in lBranches on ts.BranchId equals branches.Id
                            join dept in ldept on ts.DepartmentId equals dept.Id
                            join desig in ldesignation on ts.DesignationId equals desig.Id
                            where ts.UserId == EmployeeId
                            where ((ts.ReqFromDate.Date >= lStartdate && ts.ReqFromDate.Date <= lEnddate)
                            || (ts.ReqFromDate.Date >= lStartdate && ts.ReqFromDate.Date <= lEnddate))
                            select new
                            {
                                ts.Id,
                                employee.EmpId,
                                EmployeeName = employee.ShortName,
                                ts.ReqFromDate,
                                ts.Reason_Type,
                                ts.Reason_Desc,
                                designation = desig.Code,
                                AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                ts.Status
                            }).OrderByDescending(A => A.AppliedDate);
                //Add header       
                AddCellToHeader(tableLayout, "Applied Date");
                AddCellToHeader(tableLayout, "Request Date");
                AddCellToHeader(tableLayout, "Reason Type");
              //  AddCellToHeader(tableLayout, "Description");
                AddCellToHeader(tableLayout, "Status");

                //Add body  
                foreach (var ts in data)
                {
                    AddCellToBody(tableLayout, ts.AppliedDate);
                    AddCellToBody(tableLayout, ts.ReqFromDate.ToString("dd/MM/yyyy"));
                    AddCellToBody(tableLayout, ts.Reason_Type);
                   // AddCellToBody(tableLayout, ts.Reason_Desc);
                    AddCellToBody(tableLayout, ts.Status);
                }

                return tableLayout;
            }

        }



        public string HistoryDates(DateTime Startdate, DateTime Enddate)
        {
            string combinedates = "";
            var startday = Startdate.ToString("MMM dd");
            var startYear = Startdate.Year.ToString();
            var startTime = Startdate.ToLongTimeString();
            var endday = Enddate.ToString("MMM dd");
            var endYear = Enddate.Year.ToString();
            var endTime = Enddate.ToLongTimeString();
            if (startYear == endYear)
            {
                combinedates = startday + " " + startTime + " " + "To" + " " + endday + " " + endTime + " " + "," + " " + startYear;
                return combinedates;
            }
            else
            {
                combinedates = startday + " " + startTime + " " + " " + "," + startYear + " " + "To" + " " + endday + " " + endTime + " " + "," + " " + endYear;
                return combinedates;
            }


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
               string lca=db.Employes.Where(a=>a.Role==1 && a.CurrentDesignation==4 && a.Department==16).Select(a => a.ShortName).FirstOrDefault();
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
                               });
                var lresponseArray = lResult.ToArray();

                //  totalexperience = lresponseArray[0].TotalExperience;

                //  string ltotalexperience = totalexperience;

                // Session["ltotalexp"] = totalexperience;
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

                    return Json(new { lshortnmaeAjax = lshortname, ldesigAjax = desig, lbranchAjax = branchs,lcaajax=lca ,Status = lstatus1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                lresult = ex.Message;

            }
            return null;
        }

        [HttpPost]
        public ActionResult Timesheetrequestform(Timesheet_Request_Form Tsheet)
        {
            try
            {

                string selectempid =Convert.ToString(Tsheet.UserId);
                DateTime dateti = Tsheet.ReqFromDate;
                string datetime = dateti.ToString("yyyy-MM-dd");
                string datetimeforden = dateti.ToString("dd-MM-yyyy");
                //int count = db.timesheet_logs.Where(a=> a.user_id== Tsheet.UserId).Where(a=> a.io_time.ToString("dd-MM-yyyy") == Tsheet.ReqFromDate.ToString("dd-MM-yyy")).Count();
                string qrycnt = "select * from timesheet_logs where cast(io_time as Date) = '"+ datetime + "' and user_id="+ selectempid + "";
                DataTable dt = sh.Get_Table_FromQry(qrycnt);
                if (dt.Rows.Count <= 0)
                {
                    var dt1 = Tbus.EmployeeManualEntryDenied(selectempid, Tsheet.ReqFromDate);
                    if (dt1.Rows.Count > 0)
                    {
                        TempData["AlertMessage"] = "Employee " + dt1.Rows[0]["empid"] + " Record is already Denied for Date " + datetimeforden + "";
                        return RedirectToAction("Timesheetrequestform");
                    }
                    else
                    {
                        int lEmpId = db.Employes.Where(a => a.EmpId == selectempid).Select(a => a.Id).FirstOrDefault();
                        string controlling = db.Employes.Where(a => a.EmpId == selectempid).Select(a => a.ControllingAuthority).FirstOrDefault();
                        string sanctioning = db.Employes.Where(a => a.EmpId == selectempid).Select(a => a.SanctioningAuthority).FirstOrDefault();
                        string lca = db.Employes.Where(a => a.Role == 1 && a.CurrentDesignation == 4 && a.Department == 16).Select(a => a.EmpId).FirstOrDefault();

                        int lbranch = db.Employes.Where(a => a.EmpId == selectempid).Select(a => a.Branch).FirstOrDefault();
                        int ldept = db.Employes.Where(a => a.EmpId == selectempid).Select(a => a.Department).FirstOrDefault();
                        int ldesig = db.Employes.Where(a => a.EmpId == selectempid).Select(a => a.CurrentDesignation).FirstOrDefault();
                        int? lshifttime = db.Employes.Where(a => a.EmpId == selectempid).Select(a => a.Shift_Id).FirstOrDefault();
                        int lshift = Convert.ToInt32(lshifttime);
                        Tsheet.UserId = lEmpId;
                        Tsheet.BranchId = lbranch;
                        Tsheet.DepartmentId = ldept;
                        Tsheet.DesignationId = ldesig;
                        Tsheet.ReqToDate = Tsheet.ReqFromDate;
                        Tsheet.entrytime = Tsheet.entrytime;
                        Tsheet.exittime = Tsheet.exittime;
                        Tsheet.UpdatedDate = DateTime.Now.Date;
                        Tsheet.CA = Convert.ToInt32(lca);
                        Tsheet.SA = Convert.ToInt32(lca);
                        Tsheet.Status = "Pending";
                        Tsheet.UpdatedBy = lCredentials.EmpId;
                        Tsheet.Processed = -1;
                        Tsheet.Shift_Id = lshift;
                        Tsheet.Reason_Desc = Tsheet.Reason_Type;
                        db.Timesheet_Request_Form.Add(Tsheet);
                        db.SaveChanges();
                        TempData["AlertMessage"] = "Timesheet Request Created Successfully";
                        return RedirectToAction("Timesheetrequestform");
                    }
                    //db.Timesheet_Request_Form.Add(Tsheet);
                    //db.SaveChanges();
                    //TempData["AlertMessage"] = "Timesheet Request Created Successfully";
                    //return RedirectToAction("Timesheetrequestform");
                }
                else
                {
                    TempData["AlertMessage"] = selectempid+" Attendance already existed ";
                    return RedirectToAction("Timesheetrequestform");
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return View(Tsheet);
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
            if (branch == "HeadOffice")
            {
                requireformate = Department;
            }
            else
            {
                requireformate = requireformate.Replace('/', ' ');
            }
            return requireformate;
        }
        public ActionResult TimesheetView(string EmpId)
        {
           /// int EmployeeId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
           // int Empid = Convert.ToInt32(EmployeeId);
            var dbresult = db.Timesheet_Request_Form.ToList();
            var lEmployees = db.Employes.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            if (string.IsNullOrEmpty(EmpId))
            {
                var data = (from ts in dbresult
                            join employee in lEmployees on ts.UserId equals employee.Id
                            join branches in lBranches on ts.BranchId equals branches.Id
                            join dept in ldept on ts.DepartmentId equals dept.Id
                            join desig in ldesignation on ts.DesignationId equals desig.Id
                            where ts.UserId == Convert.ToInt32(EmpId)
                            select new
                            {
                                ts.Id,
                                employee.EmpId,
                                EmployeeName = employee.ShortName,
                                ts.ReqFromDate,
                                ts.ReqToDate,
                                ts.entrytime,
                                ts.exittime,
                                ts.Reason_Type,
                                ts.Reason_Desc,
                                designation = desig.Code,
                                AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                ts.Status
                            }).OrderByDescending(A => A.AppliedDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = (from ts in dbresult
                            join Employee in lEmployees on ts.UserId equals Employee.Id
                            join branches in lBranches on ts.BranchId equals branches.Id
                            join dept in ldept on ts.DepartmentId equals dept.Id
                            join desig in ldesignation on ts.DesignationId equals desig.Id
                            where ts.UserId == Convert.ToInt32(EmpId)
                            select new
                            {
                                ts.Id,
                                Employee.EmpId,
                                EmployeeName = Employee.ShortName,
                                ts.ReqFromDate,
                                 ts.ReqToDate,
                                ts.entrytime,
                                ts.exittime,
                                ts.Reason_Type,
                                ts.Reason_Desc,
                                designation = desig.Code,
                                AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                ts.Status
                            }).OrderByDescending(A => A.AppliedDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult TimesheetSearchViewfilters(string startdate, string enddate)
        {
            Session["sd"] = startdate;
            Session["ed"] = enddate;
            string lMessage = string.Empty;
            int EmployeeId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int Empid = Convert.ToInt32(EmployeeId);
            var dbresult = db.Timesheet_Request_Form.ToList();
            var lEmployees = db.Employes.ToList();
            var lBranches = db.Branches.ToList();
            var ldept = db.Departments.ToList();
            var ldesignation = db.Designations.ToList();
            try
            {
                if (startdate == "" && enddate == "")
                {
                    var data = (from ts in dbresult
                                join Employee in lEmployees on ts.UserId equals Employee.Id
                                join branches in lBranches on ts.BranchId equals branches.Id
                                join dept in ldept on ts.DepartmentId equals dept.Id
                                join desig in ldesignation on ts.DesignationId equals desig.Id
                               // where ts.UserId == EmployeeId
                                select new
                                {
                                    ts.Id,
                                    Employee.EmpId,
                                    EmployeeName = Employee.ShortName,
                                    ts.ReqFromDate,
                                    ts.entrytime,
                                    ts.exittime,
                                    ts.Reason_Type,
                                    ts.Reason_Desc,
                                    designation = desig.Code,
                                    AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                    Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                    ts.Status
                                }).OrderByDescending(A => A.AppliedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    DateTime lStartdate = Convert.ToDateTime(startdate);
                    DateTime lEnddate = Convert.ToDateTime(enddate);
                    var data = (from ts in dbresult
                                join employee in lEmployees on ts.UserId equals employee.Id
                                join branches in lBranches on ts.BranchId equals branches.Id
                                join dept in ldept on ts.DepartmentId equals dept.Id
                                join desig in ldesignation on ts.DesignationId equals desig.Id
                               // where ts.UserId == EmployeeId
                                where ((ts.ReqFromDate.Date >= lStartdate && ts.ReqFromDate.Date <= lEnddate)
                                 || (ts.ReqFromDate.Date >= lStartdate && ts.ReqFromDate.Date <= lEnddate))
                                select new
                                {
                                    ts.Id,
                                    employee.EmpId,
                                    EmployeeName = employee.ShortName,
                                    ts.ReqFromDate,
                                    ts.entrytime,
                                    ts.exittime,
                                    ts.Reason_Type,
                                    ts.Reason_Desc,
                                    designation = desig.Code,
                                    AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                    Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                    ts.Status
                                }).OrderByDescending(A => A.AppliedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }


        //Edit
        public ActionResult Edit(int? id)
        {
            string branchs = "";
            Timesheet_Request_Form timesheet = db.Timesheet_Request_Form.Find(id);
            string empcode = db.Employes.Where(a => a.Id == timesheet.UserId).Select(a => a.EmpId).FirstOrDefault();
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
            if (branch == 43)
            {

                branchs = depts;

            }
            else
            {
                branchs = branchss;


            }
            ViewBag.branch = branchs;
            string controlling = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.ControllingAuthority).FirstOrDefault();
            // var lca = db.Employes.Where(a => a.EmpId == controlling).Select(a => a.ShortName).FirstOrDefault();
            string lca = db.Employes.Where(a => a.Role == 1 && a.CurrentDesignation == 4 && a.Department == 16).Select(a => a.ShortName).FirstOrDefault();
            ViewBag.lca = lca;
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.ODLeaveTypes = new SelectList(Tbus.getAllODLeaveTypes(), "Type", "Type");
            Session["ltimeSheet"] = timesheet.ReqFromDate.ToString("dd-MM-yyyy");
            string dateforView = timesheet.ReqFromDate.ToString("dd-MM-yyyy");
            TempData["RequiredDate"] = dateforView;
            TempData["Requiredreasontype"] = timesheet.Reason_Type;
            TempData["UserId"] = empcode;
            DateTime entry = Convert.ToDateTime(timesheet.entrytime);
            DateTime exit= Convert.ToDateTime(timesheet.exittime);
            ViewBag.entry = entry.ToString("h:mm tt");
            ViewBag.exit= exit.ToString("h:mm tt");
           
            return View(timesheet);
        }
        [HttpPost]
        public ActionResult Edit(Timesheet_Request_Form timesheet)
        {

            string date_format = Convert.ToDateTime(timesheet.ReqFromDate).ToString("yyyy-MM-dd hh:mm:ss");
            int empcodes = Convert.ToInt32(timesheet.UserId);
            int lEmpId = db.Employes.Where(a => a.Id == empcodes).Select(a => a.Id).FirstOrDefault();
            string controlling = db.Employes.Where(a => a.Id == empcodes).Select(a => a.ControllingAuthority).FirstOrDefault();
            string sanctioning = db.Employes.Where(a => a.Id == empcodes).Select(a => a.SanctioningAuthority).FirstOrDefault();
            int lbranch = db.Employes.Where(a => a.Id == empcodes).Select(a => a.Branch).FirstOrDefault();
            int ldept = db.Employes.Where(a => a.Id == empcodes).Select(a => a.Department).FirstOrDefault();
            int ldesig = db.Employes.Where(a => a.Id == empcodes).Select(a => a.CurrentDesignation).FirstOrDefault();
            int? lshifttime = db.Employes.Where(a => a.Id == empcodes).Select(a => a.Shift_Id).FirstOrDefault();
            int lshift = Convert.ToInt32(lshifttime);
            timesheet.UpdatedDate = DateTime.Now.Date;
            timesheet.BranchId = lbranch;
            timesheet.UserId = Convert.ToInt32(empcodes);
            timesheet.DepartmentId = ldept;
            timesheet.DesignationId = ldesig;
            timesheet.entrytime = timesheet.entrytime;
            timesheet.exittime = timesheet.exittime;
            timesheet.ReqToDate = Convert.ToDateTime(date_format);
            timesheet.ReqFromDate = Convert.ToDateTime(date_format);
            timesheet.CA = Convert.ToInt32(controlling);
            timesheet.SA = Convert.ToInt32(sanctioning);
            timesheet.Status = "Pending";
            timesheet.UpdatedBy = lCredentials.EmpId;
            timesheet.Reason_Desc = timesheet.Reason_Type;
            timesheet.Processed = -1;
            timesheet.Shift_Id = lshift;

            db.Entry(timesheet).State = EntityState.Modified;
            db.SaveChanges();
            TempData["AlertMessage"] = "TimeSheet Updated Successfully";
            return RedirectToAction("Timesheetrequestform");
        }
        //Delete
        public ActionResult Delete(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            Timesheet_Request_Form timesheet = db.Timesheet_Request_Form.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
               // int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                db.Timesheet_Request_Form.Remove(timesheet);
                db.SaveChanges();
                TempData["AlertMessage"] = "Time Sheet Details deleted Successfully.";
                return RedirectToAction("Timesheetrequestform");
            }
        }
        [HttpGet]
        public ActionResult TSApprovalView()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Timesheet/TimesheetApproval.cshtml");
        }
        [HttpGet]
        public JsonResult TSApprovalViews(string status)
        {
            string lMessage = string.Empty;
            
                var lEmployees = db.Employes.ToList();
               var lBranches = db.Branches.ToList();
               var ldept = db.Departments.ToList();
               var ldesignation = db.Designations.ToList();
               var ltimesheet = db.Timesheet_Request_Form.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lControllingAuthority = db.Timesheet_Request_Form.Where(a => a.CA == lEmpId).Select(a => a.CA).FirstOrDefault();
                int lSancationingAuthority = db.Timesheet_Request_Form.Where(a => a.SA == lEmpId).Select(a => a.SA).FirstOrDefault();
                //if (lEmpId == lControllingAuthority)
                //{
                    var Duration = string.Empty;
            JsonResult lResult = Json(from ts in ltimesheet
                                      join employee in lEmployees on ts.UserId equals employee.Id
                                      join branches in lBranches on ts.BranchId equals branches.Id
                                      join dept in ldept on ts.DepartmentId equals dept.Id
                                      join desig in ldesignation on ts.DesignationId equals desig.Id
                                      //where (ts.Status == "Pending"  )
                                      select new
                                      {
                                          ts.Id,
                                          employee.EmpId,
                                          EmployeeName = employee.ShortName,
                                          ts.ReqFromDate,
                                          ts.entrytime,
                                          ts.exittime,
                                          ts.Reason_Type,
                                          ts.Reason_Desc,
                                          designation = desig.Code,
                                          AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                          Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                          ts.Status
                                      });          

            //if (lResult != null)
            //{
            //    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json(null, JsonRequestBehavior.AllowGet);
            //}

            //}
            //if (lEmpId == lSancationingAuthority)
            //{
            //    var lResult = (from ts in ltimesheet
            //                   join employee in lEmployees on ts.UserId equals Convert.ToInt32(employee.EmpId)
            //                   join branches in lBranches on employee.Branch equals branches.Id
            //                   join dept in ldept on employee.Department equals dept.Id
            //                   join desig in ldesignation on employee.CurrentDesignation equals desig.Id
            //                   where ts.SA == lSancationingAuthority && (ts.Status == "Pending" || ts.Status == "Approved" || ts.Status == "Cancelled" || ts.Status == "Denied")
            //                   select new
            //                   {
            //                       ts.Id,
            //                       ts.UserId,
            //                       EmployeeName = employee.ShortName,
            //                       ts.ReqDate,                                     
            //                       ts.Reason_Type,
            //                       ts.Reason_Desc,
            //                       designation = desig.Code,
            //                       AppliedDate = ts.UpdatedDate.ToShortDateString(),
            //                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
            //                       ts.Status
            //                   }).OrderByDescending(A => A.AppliedDate);              
            //    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);
            //}

            // return null;
            lResult.MaxJsonLength = int.MaxValue;

            return Json(lResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TSApprovalViewsfilters(string startdate,string enddate,string status)
        {

            string lMessage = string.Empty;
            try
            {
               // var lEmployees = db.Employes.ToList();
               // var lBranches = db.Branches.ToList();
               // var ldept = db.Departments.ToList();
               // var ldesignation = db.Designations.ToList();
                var ltimesheet = db.Timesheet_Request_Form.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                //int lControllingAuthority = db.Timesheet_Request_Form.Where(a => a.CA == lEmpId).Select(a => a.CA).FirstOrDefault();
                //int lSancationingAuthority = db.Timesheet_Request_Form.Where(a => a.SA == lEmpId).Select(a => a.SA).FirstOrDefault();
                if (startdate != "" && enddate != "" && status=="All")
                {
                    var Duration = string.Empty;
                    DateTime lStartdate = Convert.ToDateTime(startdate);
                    DateTime lEnddate = Convert.ToDateTime(enddate);
                    var lResult = (from ts in ltimesheet
                                   join employee in db.Employes on ts.UserId equals employee.Id
                                   join branches in db.Branches on ts.BranchId equals branches.Id
                                   join dept in db.Departments on ts.DepartmentId equals dept.Id
                                   join desig in db.Designations on ts.DesignationId equals desig.Id
                                   where ((ts.ReqFromDate.Date >= lStartdate && ts.ReqFromDate.Date <= lEnddate)
                                 || (ts.ReqFromDate.Date >= lStartdate && ts.ReqFromDate.Date <= lEnddate))
                                  // where  (ts.Status == "Pending" || ts.Status == "Approved" || ts.Status == "Cancelled" || ts.Status == "Denied")
                                   select new
                                   {
                                       ts.Id,
                                       employee.EmpId,
                                       EmployeeName = employee.ShortName,
                                       ts.ReqFromDate,
                                       ts.Reason_Type,
                                       ts.Reason_Desc,
                                       ts.entrytime,
                                       ts.exittime,
                                       designation = desig.Code,
                                       AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       ts.Status
                                   }).OrderByDescending(a => a.Status);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                }
               else if (startdate != "" && enddate != "" && status != "" && status!="All")
                {
                    DateTime lStartdate = Convert.ToDateTime(startdate);
                    DateTime lEnddate = Convert.ToDateTime(enddate);
                    var Duration = string.Empty;
                    var lResult = (from ts in ltimesheet
                                   join employee in db.Employes on ts.UserId equals employee.Id
                                   join branches in db.Branches on ts.BranchId equals branches.Id
                                   join dept in db.Departments on ts.DepartmentId equals dept.Id
                                   join desig in db.Designations on ts.DesignationId equals desig.Id
                                  
                                   where (ts.Status == status)
                                   where ((ts.ReqFromDate.Date >= lStartdate && ts.ReqFromDate.Date <= lEnddate)
                                   || (ts.ReqFromDate.Date >= lStartdate && ts.ReqFromDate.Date <= lEnddate))
                                   select new
                                   {
                                       ts.Id,
                                       employee.EmpId,
                                       EmployeeName = employee.ShortName,
                                       ts.ReqFromDate,
                                        ts.entrytime,
                                       ts.exittime,
                                       ts.Reason_Type,
                                       ts.Reason_Desc,
                                       designation = desig.Code,
                                       AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       ts.Status
                                   }).OrderByDescending(a => a.Status);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                }
                else 
                {
                    DateTime lStartdate = Convert.ToDateTime(startdate);
                    DateTime lEnddate = Convert.ToDateTime(enddate);
                    var Duration = string.Empty;
                    var lResult = (from ts in ltimesheet
                                   join employee in db.Employes on ts.UserId equals employee.Id
                                   join branches in db.Branches on ts.BranchId equals branches.Id
                                   join dept in db.Departments on ts.DepartmentId equals dept.Id
                                   join desig in db.Designations on ts.DesignationId equals desig.Id

                                   where (ts.Status == "Pending" || ts.Status == "Approved" || ts.Status == "Cancelled" || ts.Status == "Denied")
                                   where ((ts.ReqFromDate.Date >= lStartdate && ts.ReqFromDate.Date <= lEnddate)
                                   || (ts.ReqFromDate.Date >= lStartdate && ts.ReqFromDate.Date <= lEnddate))
                                   select new
                                   {
                                       ts.Id,
                                       employee.EmpId,
                                       EmployeeName = employee.ShortName,
                                       ts.ReqFromDate,
                                       ts.entrytime,
                                       ts.exittime,
                                       ts.Reason_Type,
                                       ts.Reason_Desc,
                                       designation = desig.Code,
                                       AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       ts.Status
                                   }).OrderByDescending(a => a.Status);
                    return Json(lResult.Distinct(), JsonRequestBehavior.AllowGet);

                }


            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }
    


        [HttpPost]
        public JsonResult Deny(string EmployeeCodey, string leaveTypes, string LeaveIds)
        {
            string lMessage = string.Empty;
            try
            {
                string lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
                var ldbresult = db.Timesheet_Request_Form.ToList();
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
                            int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                           // int Lid = Convert.ToInt32(lId);
                            int LeaveId = Convert.ToInt32(lIdss);
                            int lleaveTypeIds = db.LeaveTypes.Where(a => a.Type == lType).Select(a => a.Id).FirstOrDefault();
                            string lstauts = db.Timesheet_Request_Form.Where(a => a.UserId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                            if (lstauts == "Pending")
                            {               
                                Timesheet_Request_Form lupdatep = (from l in ldbresult where l.UserId == lId && l.Id == LeaveId select l).FirstOrDefault();
                                lupdatep.UpdatedBy = lCredentials.EmpId;
                                lupdatep.Processed = -1;
                                lupdatep.Status = "Denied";    
                                db.Entry(lupdatep).State = EntityState.Modified;
                                db.SaveChanges();                         
                                TempData["AlertMessage"] = "TimeSheet Denied Successfully";
                            }
                            else if (lstauts == "Denied")
                            {
                                TempData["AlertMessage"] = "TimeSheet already Denied.";
                            }
                            else if (lstauts == "Approved")
                            {
                                TempData["AlertMessage"] = "TimeSheet is already Approved.";
                            }
                            else if (lstauts == "Cancelled")
                            {
                                TempData["AlertMessage"] = "TimeSheet is already Cancelled.";
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
        public JsonResult Approve(string EmployeeCodey, string LeaveIds)
        {
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                var ldbresult = db.Timesheet_Request_Form.ToList();
                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeCodey))
                {
                    lCode = EmployeeCodey.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                List<string> lLeaveIds = new List<string>();
                if (!string.IsNullOrEmpty(LeaveIds))
                {
                    lLeaveIds = LeaveIds.Split(new char[] { ',' }).ToList();
                    lLeaveIds.Remove("");
                }
                int k = 0;
                for (int i = 0; i < lCode.Count; i++)
                {
                    for (; k < lLeaveIds.Count;)
                    {
                        string lECode = lCode[i];
                        string lIdss = lLeaveIds[k];
                        int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                       // int Lid = Convert.ToInt32(lId);
                        int LeaveId = Convert.ToInt32(lIdss);
                        string lstauts = db.Timesheet_Request_Form.Where(a => a.UserId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                        if (lstauts == "Pending")
                        {
                            int leaverowid = Convert.ToInt32(lIdss);                      
                            Timesheet_Request_Form lupdatep = (from l in ldbresult where l.UserId == lId && l.Id == LeaveId select l).FirstOrDefault();                  
                            lupdatep.Status = "Approved";
                            lupdatep.UpdatedBy = lCredentials.EmpId;                  
                            lupdatep.Processed = 0;
                            db.Entry(lupdatep).State = EntityState.Modified;
                            db.SaveChanges();                            
                            TempData["AlertMessage"] = "TimeSheet Approved Successfully";
                        }
                        else if (lstauts == "Denied")
                        {
                            TempData["AlertMessage"] = "TimeSheet already Denied.";
                        }
                        else if (lstauts == "Approved")
                        {
                            TempData["AlertMessage"] = "TimeSheet is already Approved.";
                        }
                        else if (lstauts == "Cancelled")
                        {
                            TempData["AlertMessage"] = "TimeSheet is already Cancelled.";
                        }
                        k++;
                       
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

        [HttpGet]
        public JsonResult ODTooltip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            int tsrowid = Convert.ToInt32(EmployeeCodey);
            try
            {
                var lEmployees = db.Employes.ToList();
                var lBranches = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                var ltimesheet = db.Timesheet_Request_Form.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lControllingAuthority = db.Timesheet_Request_Form.Where(a => a.CA == lEmpId).Select(a => a.CA).FirstOrDefault();
                int lSancationingAuthority = db.Timesheet_Request_Form.Where(a => a.SA == lEmpId).Select(a => a.SA).FirstOrDefault();
                if (lEmpId == lControllingAuthority)
                {
                    var Duration = string.Empty;
                    var lResult = (from ts in ltimesheet
                                   join employee in lEmployees on ts.UserId equals employee.Id
                                   join branches in lBranches on ts.BranchId equals branches.Id
                                   join dept in ldept on ts.DepartmentId equals dept.Id
                                   join desig in ldesignation on ts.DesignationId equals desig.Id
                                   //where employee.ControllingAuthority == lControllingAuthority.ToString() && (ts.Status == "Pending" || ts.Status == "Approved" || ts.Status == "Cancelled" || ts.Status == "Denied")
                                   where ts.Id == tsrowid

                                   select new
                                   {
                                       ts.Id,
                                       employee.EmpId,
                                       EmployeeName = employee.ShortName,
                                       ts.ReqFromDate,
                                       EntryTime = ts.entrytime,
                                       ExitTime = ts.exittime,
                                       ts.Reason_Type,
                                       ts.Reason_Desc,
                                       designation = desig.Code,
                                       AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       ts.Status
                                   }).OrderByDescending(a => a.Status);
                    ViewBag.LeaveRowId = tsrowid;
                    var lresponseArray = lResult.ToArray();
                    string employeeId = lresponseArray[0].EmpId;
                    string employeeName = lresponseArray[0].EmployeeName;
                    string lentrytime = lresponseArray[0].EntryTime;
                    string lexittime = lresponseArray[0].ExitTime;
                    string ReqFromDate = lresponseArray[0].ReqFromDate.ToShortDateString();
                    string Reason_Type = lresponseArray[0].Reason_Type;
                    string Reason_Desc = lresponseArray[0].Reason_Desc;
                    string designation = lresponseArray[0].designation;
                    string AppliedDate = lresponseArray[0].AppliedDate;
                    string Deptbranch = lresponseArray[0].Deptbranch;
                    string Status = lresponseArray[0].Status;

                    return Json(new
                    {
                        lEmployeeId = employeeId,
                        lEmployeeName = employeeName,
                        lReqFromDate = ReqFromDate,
                        lentrytime = lentrytime,
                        lexittime = lexittime,
                        lReason_Type = Reason_Type,
                        lReason_Desc = Reason_Desc,
                        lAppliedDate = AppliedDate,
                        lDeptbranch = Deptbranch,
                        lStatus = Status

                    }, JsonRequestBehavior.AllowGet); 
                   

                }
                
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;
        }

        //Previous Button code
        [HttpGet]
        public JsonResult ODPrevioustip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            int tsrowid = Convert.ToInt32(EmployeeCodey);
            try
            {
                var lEmployees = db.Employes.ToList();
                var lBranches = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                var ltimesheet = db.Timesheet_Request_Form.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lControllingAuthority = db.Timesheet_Request_Form.Where(a => a.CA == lEmpId).Select(a => a.CA).FirstOrDefault();
                int lSancationingAuthority = db.Timesheet_Request_Form.Where(a => a.SA == lEmpId).Select(a => a.SA).FirstOrDefault();
                if (lEmpId == lControllingAuthority)
                {
                    var Duration = string.Empty;
                    var lResult = (from ts in ltimesheet
                                   join employee in lEmployees on ts.UserId equals employee.Id
                                   join branches in lBranches on ts.BranchId equals branches.Id
                                   join dept in ldept on ts.DepartmentId equals dept.Id
                                   join desig in ldesignation on ts.DesignationId equals desig.Id
                                   //where employee.ControllingAuthority == lControllingAuthority.ToString() && (ts.Status == "Pending" || ts.Status == "Approved" || ts.Status == "Cancelled" || ts.Status == "Denied")
                                   where ts.Id == tsrowid

                                   select new
                                   {
                                       ts.Id,
                                       employee.EmpId,
                                       EmployeeName = employee.ShortName,
                                       ts.ReqFromDate,
                                       EntryTime = ts.entrytime,
                                       ExitTime = ts.exittime,
                                       ts.Reason_Type,
                                       ts.Reason_Desc,
                                       designation = desig.Code,
                                       AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       ts.Status
                                   }).OrderByDescending(a => a.Status);
                    ViewBag.LeaveRowId = tsrowid;
                    var lresponseArray = lResult.ToArray();
                    string employeeId = lresponseArray[0].EmpId;
                    string employeeName = lresponseArray[0].EmployeeName;
                    string ReqFromDate = lresponseArray[0].ReqFromDate.ToShortDateString();
                    string lentrytime = lresponseArray[0].EntryTime;
                    string lexittime = lresponseArray[0].ExitTime;
                    string Reason_Type = lresponseArray[0].Reason_Type;
                    string Reason_Desc = lresponseArray[0].Reason_Desc;
                    string designation = lresponseArray[0].designation;
                    string AppliedDate = lresponseArray[0].AppliedDate;
                    string Deptbranch = lresponseArray[0].Deptbranch;
                    string Status = lresponseArray[0].Status;

                    return Json(new
                    {
                        lEmployeeId = employeeId,
                        lEmployeeName = employeeName,
                        lReqFromDate = ReqFromDate,
                        lReason_Type = Reason_Type,
                        lentrytime=lentrytime,
                        lexittime = lexittime,
                        lReason_Desc = Reason_Desc,
                        lAppliedDate = AppliedDate,
                        lDeptbranch = Deptbranch,
                        lStatus = Status

                    }, JsonRequestBehavior.AllowGet);


                }

            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }



        //Next buttonclick code
        [HttpGet]
        public JsonResult ODNexttip(string EmployeeCodey)
        {
            string lMessage = string.Empty;
            int tsrowid = Convert.ToInt32(EmployeeCodey);
            try
            {
                var lEmployees = db.Employes.ToList();
                var lBranches = db.Branches.ToList();
                var ldept = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                var ltimesheet = db.Timesheet_Request_Form.ToList();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.LastName).FirstOrDefault();
                int lControllingAuthority = db.Timesheet_Request_Form.Where(a => a.CA == lEmpId).Select(a => a.CA).FirstOrDefault();
                int lSancationingAuthority = db.Timesheet_Request_Form.Where(a => a.SA == lEmpId).Select(a => a.SA).FirstOrDefault();
                if (lEmpId == lControllingAuthority)
                {
                    var Duration = string.Empty;
                    var lResult = (from ts in ltimesheet
                                   join employee in lEmployees on ts.UserId equals employee.Id
                                   join branches in lBranches on ts.BranchId equals branches.Id
                                   join dept in ldept on ts.DepartmentId equals dept.Id
                                   join desig in ldesignation on ts.DesignationId equals desig.Id
                                   //where employee.ControllingAuthority == lControllingAuthority.ToString() && (ts.Status == "Pending" || ts.Status == "Approved" || ts.Status == "Cancelled" || ts.Status == "Denied")
                                   where ts.Id == tsrowid

                                   select new
                                   {
                                       ts.Id,
                                       employee.EmpId,
                                       EmployeeName = employee.ShortName,
                                       ts.ReqFromDate,
                                      EntryTime= ts.entrytime,
                                      ExitTime= ts.exittime,
                                       ts.Reason_Type,
                                       ts.Reason_Desc,
                                       designation = desig.Code,
                                       AppliedDate = ts.UpdatedDate.ToShortDateString(),
                                       Deptbranch = GetBranchDepartmentConcat(branches.Name, dept.Name),
                                       ts.Status
                                   }).OrderByDescending(a => a.Status);
                    ViewBag.LeaveRowId = tsrowid;
                    var lresponseArray = lResult.ToArray();
                    string employeeId = lresponseArray[0].EmpId;
                    string employeeName = lresponseArray[0].EmployeeName;
                    string ReqFromDate = lresponseArray[0].ReqFromDate.ToShortDateString();
                    string lentrytime1 = lresponseArray[0].EntryTime;
                    string lexittime1= lresponseArray[0].ExitTime;
                    string Reason_Type = lresponseArray[0].Reason_Type;
                    string Reason_Desc = lresponseArray[0].Reason_Desc;
                    string designation = lresponseArray[0].designation;
                    string AppliedDate = lresponseArray[0].AppliedDate;
                    string Deptbranch = lresponseArray[0].Deptbranch;
                    string Status = lresponseArray[0].Status;

                    return Json(new
                    {
                        lEmployeeId = employeeId,
                        lEmployeeName = employeeName,
                        lReqFromDate = ReqFromDate,
                        lReason_Type = Reason_Type,
                        lentrytime= lentrytime1,
                        lexittime=lexittime1,
                        lReason_Desc = Reason_Desc,
                        lAppliedDate = AppliedDate,
                        lDeptbranch = Deptbranch,
                        lStatus = Status

                    }, JsonRequestBehavior.AllowGet);


                }

            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            return null;

        }




        [HttpPost]
        public JsonResult Cancel(string EmployeeCodey, string LeaveIds)
        {
            string lMessage = string.Empty;
            try
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();            
                var ldbresult = db.Timesheet_Request_Form.ToList();
                List<string> lCode = new List<string>();
                if (!string.IsNullOrEmpty(EmployeeCodey))
                {
                    lCode = EmployeeCodey.Split(new char[] { ',' }).ToList();
                    lCode.Remove("");
                }
                List<string> lLeaveIds = new List<string>();
                if (!string.IsNullOrEmpty(LeaveIds))
                {
                    lLeaveIds = LeaveIds.Split(new char[] { ',' }).ToList();
                    lLeaveIds.Remove("");
                }
                int k = 0;
                for (int i = 0; i < lCode.Count; i++)
                {
                    for (; k < lLeaveIds.Count;)
                    {
                        string lECode = lCode[i];
                        string lIdss = lLeaveIds[k];
                        int lId = db.Employes.Where(a => a.EmpId == lECode).Select(a => a.Id).FirstOrDefault();
                       // int Lid = Convert.ToInt32(lId);
                        int LeaveId = Convert.ToInt32(lIdss);
                        string lstauts = db.Timesheet_Request_Form.Where(a => a.UserId == lId).Where(a => a.Id == LeaveId).Select(a => a.Status).FirstOrDefault();
                        if (lstauts == "Pending")
                        {                        
                            Timesheet_Request_Form lupdatep = (from l in ldbresult where l.UserId == lId && l.Id == LeaveId select l).FirstOrDefault();
                            lupdatep.Status = "Cancelled";
                            lupdatep.UpdatedBy = lCredentials.EmpId;
                            lupdatep.Processed = -1;                        
                            db.Entry(lupdatep).State = EntityState.Modified;
                            db.SaveChanges();                         
                            TempData["AlertMessage"] = "TimeSheet Cancelled Successfully";
                        }
                        else if (lstauts == "Denied")
                        {
                            TempData["AlertMessage"] = "TimeSheet already Denied.";
                        }
                        else if (lstauts == "Approved")
                        {
                            TempData["AlertMessage"] = "TimeSheet is already Approved.";
                        }
                        else if (lstauts == "Cancelled")
                        {
                            TempData["AlertMessage"] = "TimeSheet is already Cancelled.";
                        }
                        k++;
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
        public JsonResult checkForHoliday(string RequestDate)
        {
            string status = "";            
            var lHolidays = db.HolidayList.ToList();
            DateTime lrdate = Convert.ToDateTime(RequestDate);
            int Count = lHolidays.Where(a => a.Date >= lrdate && a.Date <= lrdate).Select(a => a.Date).Distinct().Count();
            if (Count == 0)
            {
                status = "false/";
                return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            }
            else if (Count != 0)
            {
                status = "true/" + "Time Sheet Cannot be applied on Holiday.";
                return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkForRequestDate(string RequestDate,string empcode)
        {
            string status = "";
            DateTime leditdate = Convert.ToDateTime(Session["ltimeSheet"]);
            int lEmpId = db.Employes.Where(a => a.EmpId == empcode).Select(a => a.Id).FirstOrDefault(); 
            var ltimesheet = db.Timesheet_Request_Form.ToList();
            DateTime lrdate = Convert.ToDateTime(RequestDate);
            int Count = ltimesheet.Where(a => a.ReqFromDate.ToString("dd-MM-yyyy") == lrdate.ToString("dd-MM-yyyy")).Where(a=>a.UserId == lEmpId).Where(a=>a.Status!="Denied").Where(a => a.Status != "Cancelled").Select(a => a.ReqFromDate).Count();
            LeavesBusiness Lbus = new LeavesBusiness();
            var timesheet = Lbus.getTimesheetcheckLTCLeave(lEmpId, lEmpId, lrdate.ToString(), lrdate.ToString());
            if(timesheet !="")
            {
                status = "true/" + "Please Check the date range already applied in "  + timesheet  ;
                return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            }
            if (Count == 0)
            {
                status = "false/";
                return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            }
            else if (Count != 0)
            {
                status = "true/" + "Time Sheet has already been applied on this Date.";
                return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = status }, JsonRequestBehavior.AllowGet);
         
           
        }  
    }
}