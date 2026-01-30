using Entities;
using HRMSApplication.AuthHelpers;
using HRMSApplication.Models;
using HRMSBusiness.Business;
using HRMSBusiness.Comm;
using HRMSBusiness.Db;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HRMSApplication.Helpers
{
    public class LeaveHelper
    {
        public static string checkLeaveEligebleOrNot(LoginCredential lCredentials, string StartDate, string EndDate, int LeaveType)
        {
            ContextBase db = new ContextBase();
            string status = "";
            DateTime star1 = DateTime.Parse(StartDate);
            DateTime end1 = DateTime.Parse(EndDate);
            string lstar = star1.ToString("yyyy-MM-dd");
            string lend = end1.ToString("yyyy-MM-dd");
            int UiStartenddiff = (end1 - star1).Days;
            UiStartenddiff = UiStartenddiff + 1;
            int lHolidayCount = db.HolidayList.Where(a => a.Date >= star1 && a.Date <= end1).Select(a => a.Date).Distinct().Count();
            int UiStartenddiff1 = UiStartenddiff - lHolidayCount;
            var lHolidays = db.HolidayList.ToList();
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int designation = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.CurrentDesignation).FirstOrDefault();
            string lcode = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
            int emplevescount = db.Leaves.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveType == LeaveType).Count();
            int emplevescounts = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == LeaveType && a.Year == DateTime.Now.Year).Count();
            string basedate = "1900-01-01 00:00:00.000";
            DateTime ccDate = DateTime.Parse(basedate);
            DateTime? StartHoliday = db.HolidayList.Where(a => a.Date == star1 && a.DeleteAt == ccDate.Date).Select(a => (DateTime?)a.Date).FirstOrDefault();
            DateTime? EndHoliday = db.HolidayList.Where(a => a.Date == end1 && a.DeleteAt == ccDate.Date).Select(a => (DateTime?)a.Date).FirstOrDefault();
            string ltypes = db.LeaveTypes.Where(a => a.Id == LeaveType).Select(a => a.Code).FirstOrDefault();
            string Empcode = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            DateTime JoinigDate = Convert.ToDateTime(db.Employes.Where(a => a.Id == lEmpId).Select(a => a.DOJ).FirstOrDefault());
            DateTime lOneyear = JoinigDate.AddMonths(+12);
            var lLeaveHistory = db.V_LeaveHistory.ToList();
            int lcountcomf = lLeaveHistory.Where(a => a.LeaveType == LeaveType).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Pending").Sum(a => a.LeaveDays);
            if (ltypes == "ML")
            {
                if (star1 <= lOneyear)
                {
                    status = "false/" + "No Medical Leave is eligible upto one year of joining date";
                    //return Json(new { message = status }, JsonRequestBehavior.AllowGet);
                    return status;
                }
            }

            LeavesBusiness Lbus = new LeavesBusiness();
            //var dtwd = Lbus.getcheckLTCWDOD(lCredentials.EmpPkId, Empcode, lstar, lend, status);
            //if (dtwd != "")
            //{
            //    // status = "false/" + star1.ToShortDateString() + " , " + end1.ToShortDateString() + " " + "Already these dates are applied in " + dtwd;
            //    status = "false/" + "Please Check the date range already applied in  " + dtwd;
            //    //return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            //    return status;
            //}
            if (lcode != "Attender-Watchman" && lcode != "Watchman")
            {
                if (StartHoliday == star1 || EndHoliday == end1)
                {
                    status = "false/" + "Leave Cannot Start or End  on Holidays.";
                    //return Json(new { message = status }, JsonRequestBehavior.AllowGet);
                    //return false.ToString();
                    return status;
                }
                //return false.ToString();

            }
            //if (emplevescounts == 0 && ltypes != "LOP")
            //{
            //    status = "false/" + "No Balance to apply leave";
            //    return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            //}
            if (ltypes == "LOP")
            {
                status = "true";
                //return Json(new { message = status }, JsonRequestBehavior.AllowGet);
                return status;
            }
            if (ltypes == "W-Off")
            {
                status = "true";
                //return Json(new { message = status }, JsonRequestBehavior.AllowGet);
                return status;
            //}
            //if (ltypes == "CW-Off")
            //{
            //    status = "true";
            //    //return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            //    return status;
            }
            if (ltypes == "CL" || ltypes == "C-OFF")
            {
                int TotalClLeavedays = 0;
                string empId = lCredentials.EmpPkId;
                int isbefore = CLBeforeholidays(db, lCredentials.EmpPkId, star1, end1, LeaveType);
                int isafter = CLAfterholidays(db, lCredentials.EmpPkId, star1, end1, LeaveType);
                if (lstar != lend)
                {
                    TotalClLeavedays = isbefore + isafter + UiStartenddiff;
                }
                else if (lstar == lend)
                {
                    TotalClLeavedays = isbefore + isafter + UiStartenddiff;
                }
                if (designation == 9 || designation == 10 || designation == 11 || designation == 21)
                {
                    if (TotalClLeavedays > 0)
                    {
                        status = "Leave apply Starts From " + star1.ToShortDateString() + "  To " + end1.ToShortDateString();
                        //return Json(new { message = status, stdate = star1, eddate = end1 }, JsonRequestBehavior.AllowGet);
                        return status;
                    }
                    else
                    {
                        if (ltypes == "CL")
                        {
                            status = "More than 10 Casual leaves cannot apply";
                        }
                        else if (ltypes == "C-OFF")
                        {
                            status = "More than 10 C-OFF cannot apply";
                        }
                        //return Json(new { message = status, stdate = star1, eddate = end1 }, JsonRequestBehavior.AllowGet);
                        return status;
                    }
                }
                else
                {
                    lcountcomf = lcountcomf + TotalClLeavedays;
                    if (lcountcomf < 11)
                    {
                        if (TotalClLeavedays <= 10)
                        {
                            status = "Leave apply Starts From " + star1.ToShortDateString() + "  To " + end1.ToShortDateString();
                            //return Json(new { message = status, stdate = star1, eddate = end1 }, JsonRequestBehavior.AllowGet);
                            return status;
                        }
                        else
                        {
                            if (ltypes == "CL")
                            {
                                status = "More than 10 Casual leaves cannot apply";
                            }
                            else if (ltypes == "C-OFF")
                            {
                                status = "More than 10 C-OFF cannot apply";
                            }
                            //return Json(new { message = status, stdate = star1, eddate = end1 }, JsonRequestBehavior.AllowGet);
                            return status;
                        }
                    }
                }
            }

            // return Json(new { message = status }, JsonRequestBehavior.AllowGet);
            return status;
        }


        public static int IsCLLeaveOrHolidayday1(ContextBase db, string empId, DateTime Enddate, int LeaveType, DateTime Startdate)
        {
            var hdays = 0;
            var ldays = 0;
            var hdays1 = 0;
            var ldays1 = 0;
            int TotalDays;
            int TotalDays1 = 0;
            DateTime AfterStartdate = Enddate.AddDays(+1);
            DateTime tenthStartDay = Enddate.AddDays(+10);

            for (DateTime date = AfterStartdate.Date; date.Date <= tenthStartDay; date = date.AddDays(+1))
            {
                hdays1 = IsCLHoliday(db, date, LeaveType);

                if (hdays1 == 0)
                {
                    ldays1 = IsCLLeaveday1(empId, db, date, LeaveType, Enddate);
                }
                else
                {
                    ldays1 = 0;
                }
                if (hdays1 == 0 && ldays1 == 0)
                {
                    TotalDays1 = hdays + ldays;
                    return TotalDays1;
                }
                else
                {
                    hdays += hdays1;
                    ldays += ldays1;
                }
            }

            TotalDays = hdays + ldays + TotalDays1;
            return TotalDays;
        }




        public static int IsCLLeaveday1(string empId, ContextBase db, DateTime Enddate, int LeaveType, DateTime Startdate)
        {
            string ltypes = db.LeaveTypes.Where(a => a.Id == LeaveType).Select(a => a.Code).FirstOrDefault();
            int? empids = Convert.ToInt32(empId);
            int lEmpId = db.Employes.Where(a => a.EmpId == empId).Select(a => a.Id).FirstOrDefault();
            int leavetype = Convert.ToInt32(LeaveType);
            int leavetype1 = db.LeaveTypes.Where(a => a.Code == ltypes).Select(a => a.Id).FirstOrDefault();
            // var ldays = db.Leaves.Where(a => a.EndDate < Startdate).Where(a => a.EmpId == lEmpId).Where(a => a.LeaveType == leavetype1).Select(a => a.TotalDays).FirstOrDefault();
            List<Leaves> lEndCount = db.Leaves.Where(a => a.EmpId == empids).Where(a => a.EndDate >= Enddate && a.StartDate <= Enddate).Where(a => a.LeaveType == leavetype1).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.Status != "PartialCancelled").Where(a => a.Status != "Debited").ToList();
            int ldays = lEndCount.Count();
            return ldays;
        }

        public static int CLAfterholidays(ContextBase db, string empId, DateTime Startdate, DateTime Enddate, int LeaveType)
        {
            int totaldays = 0;
            totaldays = IsCLLeaveOrHolidayday1(db, empId, Enddate, LeaveType, Startdate);
            return totaldays;
        }

        public static int CLBeforeholidays(ContextBase db, string empId, DateTime Startdate, DateTime Enddate, int LeaveType)
        {
            int totaldays = 0;
            totaldays = IsCLLeaveOrHolidayday(db, empId, Startdate, LeaveType, Enddate);
            return totaldays;
        }

        public static int IsCLLeaveOrHolidayday(ContextBase db, string empId, DateTime Startdate, int LeaveType, DateTime Enddate)
        {
            var hdays = 0;
            var ldays = 0;
            var hdays1 = 0;
            var ldays1 = 0;
            int TotalDays;
            int TotalDays1 = 0;
            DateTime BeforeStartdate = Startdate.AddDays(-1);
            DateTime tenthStartDay = Startdate.AddDays(-10);

            for (DateTime date = BeforeStartdate.Date; date.Date >= tenthStartDay; date = date.AddDays(-1))
            {
                hdays1 = IsCLHoliday(db, date, LeaveType);

                if (hdays1 == 0)
                {
                    ldays1 = IsCLLeaveday(db, empId, date, LeaveType, Enddate);
                }
                else
                {
                    ldays1 = 0;
                }

                if (hdays1 == 0 && ldays1 == 0)
                {
                    TotalDays1 = hdays + ldays;
                    return TotalDays1;
                }
                else
                {
                    hdays += hdays1;
                    ldays += ldays1;
                }
            }

            TotalDays = hdays + ldays + TotalDays1;
            return TotalDays;
        }

        public static int IsCLHoliday(ContextBase db, DateTime Date, int LeaveType)
        {
            int hdays = 0;

            hdays = db.HolidayList.Where(a => a.Date == Date).Distinct().Count();
            return hdays;

        }


        public static int IsCLLeaveday(ContextBase db, string empId, DateTime Startdate, int LeaveType, DateTime Enddate)
        {
            string ltypes = db.LeaveTypes.Where(a => a.Id == LeaveType).Select(a => a.Code).FirstOrDefault();
            int lEmpId = db.Employes.Where(a => a.EmpId == empId).Select(a => a.Id).FirstOrDefault();
            int? empids = Convert.ToInt32(empId);
            int leavetype = Convert.ToInt32(LeaveType);
            int leavetype1 = db.LeaveTypes.Where(a => a.Code == ltypes).Select(a => a.Id).FirstOrDefault();
            // var ldays = db.Leaves.Where(a => a.EndDate < Startdate).Where(a => a.EmpId == lEmpId).Where(a => a.LeaveType == leavetype1).Select(a => a.TotalDays).FirstOrDefault();           
            List<Leaves> lStartCount = db.Leaves.Where(a => a.EmpId == empids).Where(a => a.LeaveType == leavetype1).Where(a => Startdate <= a.EndDate && Startdate >= a.StartDate).Where(a => a.Status != "Cancelled").Where(a => a.Status != "Denied").Where(a => a.Status != "PartialCancelled").Where(a => a.Status != "Debited").ToList();
            int ldays = lStartCount.Count();
            return ldays;
        }


        public static string ValidateLeaveRequest(MobileApplyLeaveDTO leavedata, int uinfo)
        {

            LeavesBusiness Lbus = new LeavesBusiness();
            var dtwd = Lbus.getapicheckLTCWDOD(leavedata.EmpId, uinfo, leavedata.StartDate, leavedata.EndDate);
            return dtwd;
        }
        public static string ValidateHolidayRequest(MobileApplyLeaveDTO leavedata, int uinfo)
        {
            LeavesBusiness Lbus = new LeavesBusiness();
            var dtwd = Lbus.getapiholiday(leavedata.EmpId, uinfo, leavedata.StartDate, leavedata.EndDate);
            return dtwd;
        }

        public static string CreateLeave(MobileApplyLeaveDTO leavedata)
        {
            int leavebalance = 0;
            ContextBase db = new ContextBase();
            Timesheet_Request_Form ltform = new Timesheet_Request_Form();
            string userqry = "Select e.EmpId,d.Code, e.Id as EmpPK,e.SanctioningAuthority,e.RetirementDate, e.ControllingAuthority,e.Branch,e.Department,e.CurrentDesignation,e.LoginMode "
                                + "FROM Employees e join Designations d on d.Id=e.CurrentDesignation "
                                + "where e.Id = " + leavedata.EmpId;
            SqlHelper sh = new SqlHelper();
            DataTable dtuser = sh.Get_Table_FromQry(userqry);
            LoginCredential lCredentials = new LoginCredential();
            lCredentials.EmpId = dtuser.Rows[0]["EmpId"].ToString();
            lCredentials.EmpPkId = dtuser.Rows[0]["EmpPK"].ToString();
            lCredentials.Branch = dtuser.Rows[0]["Branch"].ToString();
            lCredentials.Department = dtuser.Rows[0]["Department"].ToString();
            lCredentials.Designation = dtuser.Rows[0]["CurrentDesignation"].ToString();
            lCredentials.LoginMode = dtuser.Rows[0]["LoginMode"].ToString();
            string mtype = leavedata.MatrenityType;
            string status = checkLeaveEligebleOrNot(lCredentials, leavedata.StartDate, leavedata.EndDate, leavedata.LeaveTypeId);
            LeavesBusiness Lbus = new LeavesBusiness();
            leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId.ToString() == lCredentials.EmpPkId && a.LeaveTypeId == leavedata.LeaveTypeId && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
            if (status == "More than 10 Casual leaves cannot apply")
            {
                return status;
            }
            if (status == "More than 10 C-OFF cannot apply")
            {
                return status;
            }

            if (leavedata.LeaveTypeId == 1)
            {
                //pl/ml vs cl
                var dtwd1 = Lbus.CLMLPLHolidayLeave(leavedata.EmpId, leavedata.StartDate, leavedata.EndDate, "2,3,4,5,6,12,13");
                if (dtwd1.StartsWith("E#") == true)
                {
                    string[] arr = dtwd1.Split('#');
                    return arr[1] + "  Consecutive Leaves cannot be applied";
                }

                //var dtwd2 = Lbus.CL10DaysCheck(leavedata.EmpId, leavedata.StartDate, leavedata.EndDate, leavedata.LeaveTypeId);
                //if (dtwd2.StartsWith("E#") == true)
                //{
                //    string[] arr = dtwd2.Split('#');
                //    return arr[1] + " to apply";
                //}
            }

            // cl vs pl/ml
            if (leavedata.LeaveTypeId != 1 && (leavedata.LeaveTypeId == 2 || leavedata.LeaveTypeId == 3 || leavedata.LeaveTypeId == 12 || leavedata.LeaveTypeId == 16 || leavedata.LeaveTypeId == 4 || leavedata.LeaveTypeId == 5 || leavedata.LeaveTypeId == 6 || leavedata.LeaveTypeId == 17 || leavedata.LeaveTypeId == 13))
            {
                var dtwd1 = Lbus.CLMLPLHolidayLeave(leavedata.EmpId, leavedata.StartDate, leavedata.EndDate, "1");
                if (dtwd1.StartsWith("E#") == true)
                {
                    string[] arr = dtwd1.Split('#');
                    return arr[1] + "  Consecutive Leaves cannot be applied";

                }
            }
            //if (leavedata.LeaveTypeId != 1 && (leavedata.LeaveTypeId == 2 || leavedata.LeaveTypeId == 3 || leavedata.LeaveTypeId == 12))
            //{
            //    var dtwd1 = Lbus.CLMLPLHolidayLeave(leavedata.EmpId, leavedata.StartDate, leavedata.EndDate, "2,3");
            //    if (dtwd1.StartsWith("E#") == true)
            //    {
            //        string[] arr = dtwd1.Split('#');
            //        return arr[1] + "  Consecutive Leaves cannot be applied";

            //    }
            //}
            //if (leavedata.LeaveTypeId != 1 )
            //{
            //    var dtwd1 = Lbus.CLMLPLHolidayLeave(leavedata.EmpId, leavedata.StartDate, leavedata.EndDate, leavedata.LeaveTypeId.ToString());
            //    if (dtwd1.StartsWith("E#") == true)
            //    {
            //        string[] arr = dtwd1.Split('#');
            //        return arr[1] + "  Consecutive Leaves cannot be applied";

            //    }
            //}
            //pl/ML hl hl pl/ml
            if (leavedata.LeaveTypeId == 2 || leavedata.LeaveTypeId == 3 || leavedata.LeaveTypeId == 12 || leavedata.LeaveTypeId == 16 || leavedata.LeaveTypeId == 17)
            {
                string addHolidays = Lbus.PLMLAddHolidays(leavedata.EmpId, leavedata.StartDate, leavedata.EndDate, leavedata.LeaveTypeId);
                //add holidays to start date for pl, ml
                if (addHolidays.StartsWith("ASS#"))
                {
                    string[] arr = addHolidays.Split('#');
                    int holcnt = int.Parse(arr[1]);
                    int holcnt1 = int.Parse(arr[2]);
                    DateTime stDt = Convert.ToDateTime(leavedata.StartDate);
                    DateTime end = Convert.ToDateTime(leavedata.EndDate);
                    DateTime stDte = stDt.AddDays(-holcnt);
                    DateTime endate = end.AddDays(+holcnt1);
                    leavedata.StartDate = stDte.ToString("yyyy-MM-dd");
                    leavedata.EndDate = endate.ToString("yyyy-MM-dd");
                }
                if (addHolidays.StartsWith("AS#"))
                {
                    string[] arr = addHolidays.Split('#');
                    int holcnt = int.Parse(arr[1]);

                    DateTime stDt = Convert.ToDateTime(leavedata.StartDate);
                    DateTime end = Convert.ToDateTime(leavedata.EndDate);
                    DateTime stDte = stDt.AddDays(-holcnt);

                    leavedata.StartDate = stDte.ToString("yyyy-MM-dd");

                }
                //add holidays to end date for pl, ml
                else if (addHolidays.StartsWith("AE#"))
                {
                    string[] arr = addHolidays.Split('#');
                    int holcnt = int.Parse(arr[1]);
                    DateTime edDt = Convert.ToDateTime(leavedata.EndDate);
                    DateTime edDte = edDt.AddDays(holcnt);
                    leavedata.EndDate = Convert.ToString(edDte);
                }
            }

            //}



            DateTime strDate = Convert.ToDateTime(leavedata.StartDate).Date;

            string syeardate = strDate.ToString("dd/MM/yyyy");
            string[] sa = syeardate.Split('-');
            string s1 = sa[0];
            string s2 = sa[1];
            string s3 = sa[2];
            LeaveViewModel leaves = new LeaveViewModel();
            leaves.ControllingAuthority = leavedata.ControllingAuthority;
            leaves.SanctioningAuthority = leavedata.SanctioningAuthority;
            leaves.lDelivery = new DeliveryDate_PTL();
            leavedata.Year = DateTime.Now.Year;
            leaves.lleaves = new Leaves();
            leaves.lleaves.LeaveType = leavedata.LeaveTypeId;
            leaves.lleaves.Id = leavedata.LeaveTypeId;
            leaves.lleaves.StartDate = Convert.ToDateTime(leavedata.StartDate);
            leaves.lleaves.EndDate = Convert.ToDateTime(leavedata.EndDate);
            leaves.lleaves.Reason = leavedata.Reason;
            leaves.lleaves.Subject = leavedata.Reason;
            leaves.lleaves.LeavesYear = Convert.ToInt32(s3);
            leaves.lleaves.Stage = leaves.lleaves.Status;
            if (leavedata.DeliveryDate != "")
            {
                leaves.lleaves.DateofDelivery = Convert.ToDateTime(leavedata.DeliveryDate);
            }
            // leaves.lDelivery.DeliveryDate=leavedata.de
            // int lControlling = Convert.ToInt32(leavedata.ControllingAuthority);
            //int lSacantioning = Convert.ToInt32(leavedata.SanctioningAuthority);
            //login data
            LoginBus lbus = new LoginBus();





            int lControlling = Convert.ToInt32(dtuser.Rows[0]["ControllingAuthority"].ToString());
            int lSacantioning = Convert.ToInt32(dtuser.Rows[0]["SanctioningAuthority"].ToString());

            var matrenitytype = leavedata.MatrenityType;

            int lEmpId = Convert.ToInt32(lCredentials.EmpPkId);

            try
            {

                //DateTime? Retirement = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.RetirementDate).FirstOrDefault();
                DateTime? Retirement = Convert.ToDateTime(dtuser.Rows[0]["RetirementDate"].ToString());
                if (Retirement >= leaves.lleaves.EndDate)
                {

                    DateTime lStartDate = leaves.lleaves.StartDate;
                    DateTime lEndDate = leaves.lleaves.EndDate;
                    string lCode = db.LeaveTypes.Where(a => a.Id == leavedata.LeaveTypeId).Select(a => a.Code).FirstOrDefault();
                    var lEmpBalance = db.V_EmpLeaveBalance.ToList();
                    int lPaternityLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.PaternityLeave).FirstOrDefault();
                    var lEmpLeaveBalance = db.EmpLeaveBalance.Where(a => a.Year == DateTime.Now.Year).ToList();
                    // int lControlling = Convert.ToInt32(Session["lcontrols"].ToString());
                    // int lSacantioning = Convert.ToInt32(Session["lSancation"].ToString());
                    int lType = leavedata.LeaveTypeId;

                    //Leaves_CarryForward lbalances = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == lType).Where(a => a.Year == DateTime.Now.Year).FirstOrDefault();
                    //int? lcarrybal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == lType).Where(a => a.Year == DateTime.Now.Year).Select(a => a.CarryForward).FirstOrDefault();
                    //int? previousbal = db.Leaves_CarryForward.Where(a => a.EmpId == lEmpId).Where(a => a.LeaveTypeId == lType).Where(a => a.Year == DateTime.Now.Year).Select(a => a.PreviousYearCF).FirstOrDefault();
                    // int designation = dtuser.Rows[0]["CurrentDesignation"];

                    // string ldesig = db.Designations.Where(a => a.Id == designation).Select(a => a.Code).FirstOrDefault();
                    string ldesig = dtuser.Rows[0]["Code"].ToString();
                    if (ldesig == "Watchman")
                    {

                        LeavesResult lResult = LeaveHelper.GetLeaveApplyWatchMan(lEmpId, lStartDate, lEndDate, leavedata.LeaveTypeId);
                        if (lResult.Success == true)
                        {
                            DateTime lStartdate = leaves.lleaves.StartDate;
                            DateTime lEnddate = leaves.lleaves.EndDate;
                            leaves.lleaves.EmpId = lEmpId;
                            leaves.lleaves.ControllingAuthority = lControlling;
                            leaves.lleaves.SanctioningAuthority = lSacantioning;
                            leaves.lleaves.LeaveType = leaves.lleaves.Id;
                            leaves.lleaves.StartDate = lStartdate;
                            leaves.lleaves.EndDate = lEnddate;
                            leaves.lleaves.Subject = leaves.lleaves.Subject;
                            leaves.lleaves.Reason = leaves.lleaves.Reason;
                            leaves.lleaves.UpdatedDate = GetCurrentTime(DateTime.Now);
                            leaves.lleaves.UpdatedBy = lCredentials.EmpId;
                            leaves.lleaves.Status = "Pending";
                            leaves.lleaves.LeaveDays = lResult.LeaveDays;
                            leaves.lleaves.TotalDays = lResult.TotalDays;
                            leaves.lleaves.leave_balance = leavebalance;
                            leaves.lleaves.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                            if (lCode == "MTL")
                            {
                                leaves.lleaves.MaternityType = matrenitytype;
                            }
                            else
                            {
                                leaves.lleaves.MaternityType = lCode;
                            }
                            // leaves.lleaves.LeavesYear = DateTime.Today.Year;
                            leaves.lleaves.BranchId = Convert.ToInt32(lCredentials.Branch);
                            leaves.lleaves.DepartmentId = Convert.ToInt32(lCredentials.Department);
                            leaves.lleaves.DesignationId = Convert.ToInt32(lCredentials.Designation);
                            db.Leaves.Add(leaves.lleaves);
                            db.SaveChanges();


                            if (lCode == "LOP")
                            {
                                //var lopbalance = new EmpLeaveBalance
                                //{
                                //    LeaveTypeId = lType,
                                //    EmpId = lEmpId,
                                //    LeaveBalance = lResult.TotalLeaves,
                                //    UpdatedBy = lCredentials.EmpId,
                                //    Year = DateTime.Now.Year
                                //};
                                //db.Entry(lopbalance).State = EntityState.Modified;
                                EmpLeaveBalance lopbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == leavedata.LeaveTypeId && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lopbalance.LeaveTypeId = leavedata.LeaveTypeId;
                                lopbalance.EmpId = lEmpId;
                                lopbalance.LeaveBalance = lopbalance.LeaveBalance + lResult.LeaveDays;
                                lopbalance.Debits = lopbalance.Debits + lResult.LeaveDays;
                                lopbalance.Year = DateTime.Now.Year;
                                db.Entry(lopbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                string lApplys = "Leave";
                                string lAppValues = "0";
                                string lresultsSms = LeaveHelper.SendSms(lStartDate, lEndDate, lControlling, lSacantioning, lEmpId, lApplys, lAppValues);
                                string lresultsEmail = LeaveHelper.SendEmails(lStartDate, lEndDate, lControlling, lSacantioning, lEmpId, lType, lResult.LeaveDays, leavedata.Reason, lApplys, lAppValues);
                                return lResult.Message;

                            }
                            if (lCode == "MTL")
                            {
                                EmpLeaveBalance lbalances = lEmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == leavedata.LeaveTypeId && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalances.LeaveTypeId = leavedata.LeaveTypeId;
                                lbalances.EmpId = lEmpId;
                                lbalances.LeaveBalance = lResult.TotalLeaves;
                                lbalances.Debits = lbalances.Debits + lResult.LeaveDays;
                                lbalances.Year = DateTime.Now.Year;
                                db.Entry(lbalances).State = EntityState.Modified;
                                db.SaveChanges();
                                return lResult.Message;
                            }

                            EmpLeaveBalance lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == leavedata.LeaveTypeId && a.Year == DateTime.Now.Year).FirstOrDefault();
                            lbalance.LeaveTypeId = leavedata.LeaveTypeId;
                            lbalance.EmpId = lEmpId;
                            lbalance.LeaveBalance = lResult.TotalLeaves;
                            lbalance.Debits = lbalance.Debits + lResult.LeaveDays;
                            lbalance.Year = DateTime.Now.Year;
                            db.Entry(lbalance).State = EntityState.Modified;
                            db.SaveChanges();
                            return lResult.Message;

                        }
                    }

                    if (lPaternityLeave == 0 && lCode == "PTL")
                    {
                        return "No Leave Balance available to apply leave";

                    }
                    else if (lPaternityLeave != 0 && lCode == "PTL")
                    {
                        string lmale = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.Gender).FirstOrDefault();
                        if (lmale == "Male")
                        {
                            int TotalDays = (lEndDate.Date - lStartDate.Date).Days;
                            TotalDays = TotalDays + 1;
                            if (lPaternityLeave < TotalDays)
                            {
                                return " Only " + lPaternityLeave + " Paternity Leaves are available";

                            }
                            else
                            {
                                // DateTime lFifteendays = lStartDate.AddDays(15);
                                //DateTime lsixMonths = lEndDate.AddMonths(+6);
                                
                                DateTime ldeliveryFifteendays = leaves.lleaves.DateofDelivery.AddDays(-15).Date;
                                
                                    DateTime DateofDelivery = leaves.lleaves.DateofDelivery;
                                DateTime sdate = leaves.lleaves.StartDate;
                                DateTime ldeliverytosixmonths;
                                if (sdate < DateofDelivery)
                                {
                                     ldeliverytosixmonths = leaves.lleaves.DateofDelivery.AddMonths(+6).AddDays(-15).Date;

                                }
                                else
                                {
                                     ldeliverytosixmonths = leaves.lleaves.DateofDelivery.AddMonths(+6).Date;

                                }
                                //string lmale = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.Gender).FirstOrDefault();

                                if (lStartDate >= ldeliveryFifteendays && lEndDate <= ldeliverytosixmonths)
                                {
                                    if (lmale == "Male")
                                    {

                                        int TotalDayss = (lEndDate.Date - lStartDate.Date).Days;
                                        TotalDayss = TotalDayss + 1; // to get total days
                                        int days = 1;                           //  int days = 1;
                                        int ldays = lPaternityLeave - TotalDayss;
                                        int lcount = db.Leaves.Where(a => a.LeaveType == lType && a.EmpId == lEmpId).Count();
                                        if (lcount < 2)
                                        {
                                            if (TotalDays <= 15)
                                            {

                                                DateTime lStartdate = leaves.lleaves.StartDate;
                                                DateTime lEnddate = leaves.lleaves.EndDate;
                                                leaves.lleaves.EmpId = lEmpId;
                                                leaves.lleaves.ControllingAuthority = lControlling;
                                                leaves.lleaves.SanctioningAuthority = lSacantioning;
                                                leaves.lleaves.LeaveType = leaves.lleaves.Id;
                                                leaves.lleaves.StartDate = lStartdate;
                                                leaves.lleaves.EndDate = lEnddate;
                                                leaves.lleaves.Subject = leaves.lleaves.Subject;
                                                leaves.lleaves.Reason = leaves.lleaves.Reason;
                                                leaves.lleaves.UpdatedDate = GetCurrentTime(DateTime.Now);
                                                leaves.lleaves.UpdatedBy = lCredentials.EmpId;
                                                leaves.lleaves.Status = "Pending";
                                                leaves.lleaves.leave_balance = leavebalance;
                                                leaves.lleaves.LeaveDays = TotalDays;
                                                leaves.lleaves.TotalDays = TotalDays;
                                                // leaves.lleaves.LeavesYear = DateTime.Today.Year;
                                                leaves.lleaves.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                                if (lCode == "MTL")
                                                {
                                                    leaves.lleaves.MaternityType = matrenitytype;
                                                }
                                                else
                                                {
                                                    leaves.lleaves.MaternityType = lCode;
                                                }
                                                leaves.lleaves.BranchId = Convert.ToInt32(lCredentials.Branch);
                                                leaves.lleaves.DepartmentId = Convert.ToInt32(lCredentials.Department);
                                                leaves.lleaves.DesignationId = Convert.ToInt32(lCredentials.Designation);
                                                db.Leaves.Add(leaves.lleaves);
                                                db.SaveChanges();
                                                //int ldeliveryforLeaveId = db.Leaves.Where(a => a.StartDate == lStartDate && a.EndDate == lEndDate).Select(a => a.Id).FirstOrDefault();
                                                //var lDeliveryDate = new DeliveryDate_PTL
                                                //{
                                                //    LeaveId = ldeliveryforLeaveId,
                                                //    DeliveryDate = leaves.lDelivery.DeliveryDate,
                                                //    UpdatedBy = Convert.ToInt32(lCredentials.EmpId),
                                                //    UpdatedDate = DateTime.Now,
                                                //};
                                                //db.DeliveryDate_PTL.Add(lDeliveryDate);
                                                //db.SaveChanges();
                                                EmpLeaveBalance lbalance = lEmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == leavedata.LeaveTypeId && a.Year == DateTime.Now.Year).FirstOrDefault();
                                                lbalance.LeaveTypeId = leavedata.LeaveTypeId;
                                                lbalance.EmpId = lEmpId;
                                                lbalance.LeaveBalance = ldays;
                                                lbalance.Year = DateTime.Now.Year;
                                                db.Entry(lbalance).State = EntityState.Modified;
                                                db.SaveChanges();
                                                string lApply = "Leave";
                                                string lAppValue = "0";
                                                string lresultsSms = LeaveHelper.SendSms(lStartDate, lEndDate, lControlling, lSacantioning, lEmpId, lApply, lAppValue);
                                                string lresult = LeaveHelper.SendEmails(lStartDate, lEndDate, lControlling, lSacantioning, lEmpId, lType, ldays, leavedata.Reason, lApply, lAppValue);
                                                return "Paternity Leave applied successfully for " + leaves.lleaves.LeaveDays + " days";


                                            }
                                            else
                                            {
                                                return string.Format("Paternity Leave cannot be exceed 15 days");

                                            }
                                        }
                                        else
                                        {

                                            return "Paternity Leaves are exceed";

                                        }
                                        // }
                                    }
                                    else
                                    {
                                        return "Only Male employees are eligable to apply";

                                    }
                                }
                                else
                                {
                                    return "Paternity Leave can be applied before 15 days of Delivery date or after six months of Delivery date";

                                }
                            }
                        }
                        else
                        {
                            return "Only Male employees are eligible for Paternity Leave";
                        }
                    }

                    //else
                    //{
                    //    return lResult.Message;

                    //}


                    else
                    {
                        LeavesResult lResult = null;
                        try
                        {
                            lResult = LeaveHelper.GetLeavesResult(lEmpId, lStartDate, lEndDate, leavedata.LeaveTypeId, leavedata.MatrenityType, lCredentials.Designation);
                            LogInformation.Info("* 2 result * " + lResult);
                        }
                        catch (Exception ex22)
                        {
                            LogInformation.Info("* 2 error * " + ex22.Message);
                            LogInformation.Info("* 22 error * " + ex22.StackTrace);
                        }

                        if (lResult.Success == true)
                        {
                            if (lCredentials.LoginMode == Constants.SuperAdmin || lCredentials.LoginMode == Constants.AdminHRDPayments ||
                            lCredentials.LoginMode == Constants.AdminHRDPolicy || lCredentials.LoginMode == Constants.Executive || lCredentials.LoginMode == Constants.Manager || lCredentials.LoginMode == Constants.Employee)
                            {

                                DateTime lStartdate = leaves.lleaves.StartDate;
                                DateTime lEnddate = leaves.lleaves.EndDate;
                                leaves.lleaves.EmpId = lEmpId;
                                leaves.lleaves.ControllingAuthority = lControlling;
                                leaves.lleaves.SanctioningAuthority = lSacantioning;
                                leaves.lleaves.LeaveType = leaves.lleaves.Id;
                                leaves.lleaves.StartDate = lStartdate;
                                leaves.lleaves.EndDate = lEnddate;
                                leaves.lleaves.Subject = leaves.lleaves.Subject;
                                leaves.lleaves.Reason = leaves.lleaves.Reason;
                                leaves.lleaves.UpdatedDate = GetCurrentTime(DateTime.Now);
                                leaves.lleaves.UpdatedBy = lCredentials.EmpId;
                                leaves.lleaves.Status = "Pending";
                                leaves.lleaves.leave_balance = leavebalance;
                                // leaves.lleaves.LeavesYear = DateTime.Today.Year;
                                leaves.lleaves.LeaveDays = lResult.LeaveDays;
                                leaves.lleaves.TotalDays = lResult.TotalDays;
                                leaves.lleaves.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                                if (lCode == "MTL")
                                {
                                    leaves.lleaves.MaternityType = matrenitytype;
                                }
                                else
                                {
                                    leaves.lleaves.MaternityType = lCode;
                                }
                                leaves.lleaves.BranchId = Convert.ToInt32(lCredentials.Branch);
                                leaves.lleaves.DepartmentId = Convert.ToInt32(lCredentials.Department);
                                leaves.lleaves.DesignationId = Convert.ToInt32(lCredentials.Designation);
                                db.Leaves.Add(leaves.lleaves);
                                db.SaveChanges();

                                if (lCode == "LOP")
                                {
                                    int leftbalance1 = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == leavedata.LeaveTypeId && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                    EmpLeaveBalance lopbalance = lEmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == leavedata.LeaveTypeId && a.Year == DateTime.Now.Year).FirstOrDefault();
                                    lopbalance.LeaveTypeId = leaves.lleaves.LeaveType;
                                    lopbalance.EmpId = lEmpId;
                                    lopbalance.Debits = lopbalance.Debits + lResult.LeaveDays;
                                    lopbalance.LeaveBalance = lResult.TotalLeaves + leftbalance1;
                                    lopbalance.Year = leavedata.Year;
                                    db.Entry(lopbalance).State = EntityState.Modified;
                                    db.SaveChanges();
                                    string lApplys = "Leave";
                                    string lAppValues = "0";
                                    string lresultsSms = LeaveHelper.SendSms(lStartDate, lEndDate, lControlling, lSacantioning, lEmpId, lApplys, lAppValues);
                                    string lresultsEmail = LeaveHelper.SendEmails(lStartDate, lEndDate, lControlling, lSacantioning, lEmpId, lType, lResult.LeaveDays, leavedata.Reason, lApplys, lAppValues);
                                    return lResult.Message;

                                }
                                if (lCode == "W-Off")
                                {
                                    int leftbalance1 = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == leavedata.LeaveTypeId && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
                                    EmpLeaveBalance lopbalance = lEmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == leavedata.LeaveTypeId && a.Year == DateTime.Now.Year).FirstOrDefault();
                                    lopbalance.LeaveTypeId = leaves.lleaves.LeaveType;
                                    lopbalance.EmpId = lEmpId;
                                    lopbalance.Debits = lopbalance.Debits + lResult.LeaveDays;
                                    lopbalance.LeaveBalance = lResult.TotalLeaves + leftbalance1;
                                    lopbalance.Year = leavedata.Year;
                                    db.Entry(lopbalance).State = EntityState.Modified;
                                    db.SaveChanges();
                                    string lApplys = "Leave";
                                    string lAppValues = "0";
                                    string lresultsSms = LeaveHelper.SendSms(lStartDate, lEndDate, lControlling, lSacantioning, lEmpId, lApplys, lAppValues);
                                    string lresultsEmail = LeaveHelper.SendEmails(lStartDate, lEndDate, lControlling, lSacantioning, lEmpId, lType, lResult.LeaveDays, leavedata.Reason, lApplys, lAppValues);
                                    return lResult.Message;

                                }
                                //if (leaves.lleaves.EndDate <= DateTime.Now.Date)
                                //{
                                //    int lId = db.Employes.Where(a => a.Id == leaves.lleaves.EmpId).Select(a => a.Id).FirstOrDefault();
                                //    string lcode = db.LeaveTypes.Where(a => a.Id == leaves.lleaves.LeaveType).Select(a => a.Code).FirstOrDefault();
                                //    int branchid = db.Employes.Where(a => a.Id == lId).Select(a => a.Branch).FirstOrDefault();
                                //    int? shiftids = db.Employes.Where(a => a.Id == lId).Where(a => a.Branch == branchid).Select(a => a.Shift_Id).FirstOrDefault();
                                //    string lca = db.Employes.Where(a => a.Role == 1 && a.CurrentDesignation == 4 && a.Department == 16).Select(a => a.EmpId).FirstOrDefault();
                                //    //  int idlt = db.Timesheet_Request_Form.OrderByDescending(a => a.Id).Select(a => a.Id).FirstOrDefault();
                                //    // ltform.Id = idlt + 1;
                                //    ltform.UserId = lId;
                                //    ltform.BranchId = (int)leaves.lleaves.BranchId;
                                //    ltform.DepartmentId = (int)leaves.lleaves.DepartmentId;
                                //    ltform.DesignationId = (int)leaves.lleaves.DesignationId;
                                //    if (shiftids != null)
                                //    {
                                //        ltform.Shift_Id = (int)shiftids;
                                //    }
                                //    ltform.Reason_Type = lcode;
                                //    ltform.Reason_Desc = "Leave";
                                //    ltform.ReqFromDate = leaves.lleaves.StartDate;
                                //    ltform.ReqToDate = leaves.lleaves.EndDate;
                                //    ltform.CA = Convert.ToInt32(lca);
                                //    ltform.SA = Convert.ToInt32(lca); 
                                //    ltform.Status = leaves.lleaves.Status;
                                //    ltform.UpdatedBy = leaves.lleaves.UpdatedBy;
                                //    ltform.UpdatedDate = leaves.lleaves.UpdatedDate;
                                //    ltform.entrytime = "";
                                //    ltform.exittime = "";
                                //    ltform.Processed = 0;

                                //    db.Timesheet_Request_Form.Add(ltform);
                                //    db.SaveChanges();
                                //}
                                EmpLeaveBalance lbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == leavedata.LeaveTypeId && a.Year == DateTime.Now.Year).FirstOrDefault();
                                lbalance.LeaveTypeId = leavedata.LeaveTypeId;
                                lbalance.EmpId = lEmpId;
                                lbalance.Debits = lbalance.Debits + lResult.LeaveDays;
                                lbalance.LeaveBalance = lResult.TotalLeaves;
                                lbalance.Year = DateTime.Now.Year;
                                db.Entry(lbalance).State = EntityState.Modified;
                                db.SaveChanges();
                                string lApply = "Leave";
                                string lAppValue = "0";
                                string lresults = LeaveHelper.SendSms(lStartDate, lEndDate, lControlling, lSacantioning, lEmpId, lApply, lAppValue);
                                string lresult = LeaveHelper.SendEmails(lStartDate, lEndDate, lControlling, lSacantioning, lEmpId, lType, lResult.LeaveDays, leavedata.Reason, lApply, lAppValue);
                                return lResult.Message;


                            }
                        }
                        else
                        {
                            return lResult.Message;

                        }
                    }
                }



                else
                {
                    return "The Selected Dates should be less than or equal to the Retirement Date" + "  " + Retirement.Value.ToShortDateString() + "  " + "Please select other dates.";

                }

            }

            catch (Exception ex)
            {
                return ex.Message;
            }

            //return View(lMessage);
            return "Leave applied.";
        }

        public static LeavesResult GetLeavesResult(int lEmpId, DateTime Startdate, DateTime Endate, int LeavetypeId, string mtype, string Designation)
        {
            int leavebalance = 0;
            LogInformation.Info("* 2 *");
            ContextBase db = new ContextBase();
            LeavesResult lResult = new LeavesResult();

            //var lEmpBalance = db.V_EmpLeaveBalance.ToList();
            //var lEmpLeaveBalance = db.EmpLeaveBalance.ToList();
            var lLeaveHistory = db.V_LeaveHistory.ToList();
            var lcount = (from u in db.V_LeaveHistory

                          where u.EmpId == lEmpId && u.Status == "Approved" && u.LeaveType == LeavetypeId
                          select (int?)u.LeaveDays).Sum() ?? 0;
            //int lcount = db.V_LeaveHistory.Where(a => a.LeaveType == LeavetypeId && a.EmpId == lEmpId && a.Status == "Approved").Sum(a => a.LeaveDays);
            // var lLeaves = db.Leaves.ToList();
            var lHolidays = db.HolidayList.ToList();
            var lTypes = db.LeaveTypes.ToList();
            var lLeaveInfo = db.LeaveInfo.ToList();
            var lDateofDeliveryTb = db.DeliveryDate_PTL.ToList();
            var velb = db.V_EmpLeaveBalance.Where(c => c.EmpId == lEmpId)
                               .Select(s => new
                               {
                                   s.CasualLeave,
                                   s.MedicalSickLeave,
                                   s.PrivilegeLeave,
                                   s.MaternityLeave,
                                   s.PaternityLeave,
                                   s.ExtraOrdinaryLeave,
                                   s.SpecialCasualLeave,
                                   s.CompensatoryOff,
                                   s.LOP,
                                   s.woff,
                                   s.CWOFF
                               }).FirstOrDefault();

            //int lCasualLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.CasualLeave).FirstOrDefault();
            
            int lCasualLeave = velb.CasualLeave;
            //int lMedicalSickLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.MedicalSickLeave).FirstOrDefault();
            int lMedicalSickLeave = velb.MedicalSickLeave;
            //int lPrivilegeLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.PrivilegeLeave).FirstOrDefault();
            int lPrivilegeLeave = velb.PrivilegeLeave;
            //int lMaternityLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.MaternityLeave).FirstOrDefault();
            int lMaternityLeave = velb.MaternityLeave;
            //int lPaternityLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.PaternityLeave).FirstOrDefault();
            int lPaternityLeave = velb.PaternityLeave;
            //int lExtraOrdinaryLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.ExtraOrdinaryLeave).FirstOrDefault();
            int lExtraOrdinaryLeave = velb.ExtraOrdinaryLeave;
            //int lSpecialCasualLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.SpecialCasualLeave).FirstOrDefault();
            int lSpecialCasualLeave = velb.SpecialCasualLeave;
           
            // int lCompOff = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a=> a.CompensatoryOff).FirstOrDefault();
            int lCompOff = velb.CompensatoryOff;
            // int lLOP = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.LOP).FirstOrDefault();

            int lLOP = velb.LOP;
            int lwoff = velb.woff;
            int lCWOFF = velb.CWOFF;
            leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == LeavetypeId && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
            int lRetrieveleaveType = lTypes.Where(a => a.Id == LeavetypeId).Select(a => a.Id).FirstOrDefault();
            string lLeaveCode = lTypes.Where(a => a.Id == LeavetypeId).Select(a => a.Code).FirstOrDefault();
            string lType = lTypes.Where(a => a.Id == LeavetypeId).Select(a => a.Type).FirstOrDefault();
            int year = DateTime.Now.Year;
            DateTime today = DateTime.Now;
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(year, 12, 31);
            if (LeavetypeId == lRetrieveleaveType)
            {
                if (lLeaveCode == "CL")
                {
                    if (lCasualLeave == 0)
                    {
                        lResult.Success = false;
                        lResult.Message = string.Format("No Leave Balance available to apply Casual leave");
                        //return lResult;
                    }
                    else
                    {
                        if (Startdate <= Endate)
                        {
                            string basedate = "1900-01-01 12:00:00.000";
                            DateTime ccDadte = DateTime.Parse(basedate);
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days
                            int ldays = lCasualLeave - TotalDays;
                            int Count = db.HolidayList.Where(a => a.Date >= Startdate && a.Date <= Endate).Select(a => a.Date).Distinct().Count();
                            int lHolidayCount = db.HolidayList.Where(a => a.Date >= Startdate && a.Date <= Endate).Select(a => a.Date).Distinct().Count();
                            // int lcount = lLeaveHistory.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Approved").Sum(a => a.LeaveDays);

                            int lTotalss = 0;
                            if (Count != 0)
                            {
                                lTotalss = TotalDays - Count;
                            }
                            else
                            {
                                lTotalss = TotalDays;
                            }
                            if (TotalDays <= 10)
                            {
                                int lTotalLeaves = lTotalss;

                                //if (lcount >= 15)
                                //{
                                //    lResult.Success = false;
                                //    lResult.Message = string.Format("15 Casual Leaves can be applied in year");
                                //    return lResult;
                                //}

                                if (lCasualLeave >= lTotalss)
                                {
                                    if (lTotalLeaves <= 7 || lTotalLeaves == 10)
                                    {
                                        //if (lHolidayCount != 1)
                                        //{
                                        int toaldays = TotalDays - Count;
                                        ldays = lCasualLeave - toaldays;
                                        lResult.LeaveDays = toaldays;       // leave Applied days
                                        lResult.TotalDays = TotalDays;  // total cls applied now 
                                        lResult.TotalLeaves = ldays;   // balance left
                                        lResult.Success = true;
                                        // lResult.Message = lType + " " + " applied successfully";
                                        //int levalue = Convert.ToInt32(lEmpId);
                                        //string lid = db.Employes.Where(a => a.Id == levalue).Select(a => a.EmpId).FirstOrDefault();
                                        //string firstname = db.Employes.Where(a => a.Id == levalue).Select(a => a.FirstName).FirstOrDefault();
                                        //string lastname = db.Employes.Where(a => a.Id == levalue).Select(a => a.LastName).FirstOrDefault();
                                        //// string result = SMSCode.SendSMS(lid, firstname, lastname);
                                        LogInformation.Info("Before return  " + lResult.Message);
                                        if (lResult.LeaveDays > 1)
                                        {
                                            lResult.Message = string.Format("Casual Leave applied Successfully for " + lResult.LeaveDays + " days");
                                        }
                                        else
                                        {
                                            lResult.Message = string.Format("Casual Leave applied Successfully for " + lResult.LeaveDays + " day");
                                        }
                                        //return lResult;                                            
                                        // }
                                        //else
                                        //{
                                        //    lResult.Success = false;
                                        //    lResult.Message = string.Format(" Cannot apply Casual Leaves on Holiday days");
                                        //    //return lResult;
                                        //}
                                    }
                                    else
                                    {
                                        lResult.Success = false;
                                        lResult.Message = string.Format("Casual Leave should not exceed more than 7 days");
                                        //return lResult;
                                    }
                                }

                                else
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format(" Only " + lCasualLeave + " Casual Leaves are available.");
                                    //return lResult;
                                }

                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format("At a time more than 10 Casual Leaves cannot be applied");
                                //return lResult;
                            }
                        }
                    }

                }
                else if (lLeaveCode == "MTL")
                {
                    string lFemale = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.Gender).FirstOrDefault();

                    if (lFemale == "Female")
                    {
                        int lcount1 = db.Leaves.Where(a => a.LeaveType == LeavetypeId && a.EmpId == lEmpId).Count();
                        int lcount2 = db.Leaves.Where(a => a.LeaveType == LeavetypeId && a.EmpId == lEmpId && a.MaternityType == "Miscarriage").Count();
                        if (lcount1 <= 2)
                        {
                            if (mtype == "Miscarriage" && lcount2 > 1)
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format("Cannot apply Miscarriage Maternity leave two times");
                            }
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1;
                            int TotalLeavedays = TotalDays;
                            DateTime lsixMonths = Startdate.AddMonths(+6);
                            if (Startdate <= Endate && TotalDays <= 184)
                            {

                                // to get total days
                                //  int days = 1;
                                int ldays = lMaternityLeave - TotalDays;
                                int lcounts = db.Leaves.Where(a => a.LeaveType == LeavetypeId && a.EmpId == lEmpId).Count();
                                if (lMaternityLeave < TotalDays)
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format("No Leave Balance available to apply Maternity leave");
                                }
                                else if (lMaternityLeave >= TotalDays)
                                {
                                    var count = (from u in db.Leaves
                                                 join ch in db.LeaveTypes on u.LeaveType equals ch.Id
                                                 where u.EmpId == lEmpId
                                                 select (int?)u.LeaveDays).Sum() ?? 0;
                                    if (count >= 365)
                                    {
                                        lResult.Success = false;
                                        lResult.Message = string.Format("Maternity Leaves are exceed");
                                        //return lResult;
                                    }
                                    lResult.LeaveDays = TotalLeavedays;
                                    lResult.TotalLeaves = ldays;
                                    lResult.TotalDays = TotalLeavedays;
                                    lResult.Success = true;
                                    lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                    //return lResult;
                                }
                                else
                                {
                                    DateTime date = Startdate.AddDays(+lMaternityLeave);
                                    string Totaldate = date.ToShortDateString();
                                    lResult.Success = false;
                                    //lResult.Message = string.Format(" Only " + lMaternityLeave + " Maternity Leaves are available.");

                                    lResult.Message = string.Format("MaternityLeave leave can be applied upto" + Totaldate);
                                    //return lResult;
                                }
                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format("Maternity Leave should not exceed more than six months");
                                //return lResult;
                            }

                        }
                        else
                        {
                            lResult.Success = false;
                            lResult.Message = string.Format("No Leave Balance available to apply Maternity leave");
                            //return lResult;
                        }
                    }

                    else
                    {
                        lResult.Success = false;
                        lResult.Message = string.Format("Only Female employees are eligable to apply");
                        //return lResult;
                    }

                }
                else if (lLeaveCode == "PL")
                {
                    if (lPrivilegeLeave == 0)
                    {
                        lResult.Success = false;
                        lResult.Message = string.Format("No Leave Balance available to apply Privilege leave");
                        //return lResult;
                    }
                    else
                    {
                        string ldate = db.Employes.Select(a => a.RetirementDate.ToString()).FirstOrDefault();
                        if (ldate != null)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days
                            int ldays = lPrivilegeLeave - TotalDays;
                            // int lcount = lLeaveHistory.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Approved").Sum(a => a.LeaveDays);
                            if (lcount <= 270)
                            {
                                if (TotalDays <= 120)
                                {
                                    if (lPrivilegeLeave >= TotalDays)
                                    {
                                        lResult.LeaveDays = TotalDays;
                                        lResult.TotalLeaves = ldays;
                                        lResult.TotalDays = TotalDays;
                                        lResult.Success = true;
                                        if (lResult.LeaveDays > 1)
                                        {
                                            lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                        }
                                        else
                                        {
                                            lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " day");
                                        }
                                        //return lResult;
                                    }
                                    else
                                    {
                                        lResult.Success = false;
                                        lResult.Message = string.Format(" Only " + lPrivilegeLeave + " Privilege Leaves are available.");
                                        //return lResult;
                                    }

                                }
                                else
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format("Privilege Leave cannot be applied more than 120 days");
                                    //return lResult;
                                }

                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format(" Only 270 Privilege Leaves can be applied in entire service period");
                                //return lResult;
                            }

                        }
                        else
                        {
                            lResult.Success = false;
                            lResult.Message = string.Format("No Privilege Leaves are allowed in preparatory retirement");
                            //return lResult;
                        }
                    }

                }

                else if (lLeaveCode == "EOL")
                {
                    if (lExtraOrdinaryLeave == 0)
                    {
                        string lCode = db.LeaveTypes.Where(a => a.Id == LeavetypeId).Select(a => a.Code).FirstOrDefault();
                        string userqry = "Select e.EmpId,d.Code, e.Id as EmpPK,e.SanctioningAuthority,e.RetirementDate, e.ControllingAuthority,e.Branch,e.Department,e.CurrentDesignation,e.LoginMode "
                               + "FROM Employees e join Designations d on d.Id=e.CurrentDesignation "
                               + "where e.Id = " + lEmpId;
                        SqlHelper sh = new SqlHelper();
                        DataTable dtuser = sh.Get_Table_FromQry(userqry);
                        int TotalDays = (Endate.Date - Startdate.Date).Days;
                        TotalDays = TotalDays + 1;
                        LoginCredential lCredentials = new LoginCredential();
                        lCredentials.EmpId = dtuser.Rows[0]["EmpId"].ToString();
                        lCredentials.EmpPkId = dtuser.Rows[0]["EmpPK"].ToString();
                        lCredentials.Branch = dtuser.Rows[0]["Branch"].ToString();
                        lCredentials.Department = dtuser.Rows[0]["Department"].ToString();
                        lCredentials.Designation = dtuser.Rows[0]["CurrentDesignation"].ToString();
                        lCredentials.LoginMode = dtuser.Rows[0]["LoginMode"].ToString();
                        int lControlling = Convert.ToInt32(dtuser.Rows[0]["ControllingAuthority"].ToString());
                        int lSacantioning = Convert.ToInt32(dtuser.Rows[0]["SanctioningAuthority"].ToString());
                        Leaves leaves = new Leaves();
                        DateTime lStartdate = Startdate;
                        DateTime lEnddate = Endate;
                        leaves.EmpId = lEmpId;
                        leaves.ControllingAuthority = lControlling;
                        leaves.SanctioningAuthority = lSacantioning;
                        leaves.LeaveType = 12;
                        leaves.StartDate = lStartdate;
                        leaves.EndDate = lEnddate;

                        leaves.Reason = "ExtraOrdinary Leave treated as LOP";
                        leaves.UpdatedDate = GetCurrentTime(DateTime.Now);
                        leaves.UpdatedBy = lEmpId.ToString();
                        leaves.Status = "Pending";
                        leaves.leave_balance = leavebalance;
                        leaves.LeaveDays = TotalDays;
                        leaves.TotalDays = TotalDays;
                        // leaves.lleaves.LeavesYear = DateTime.Today.Year;
                        leaves.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                        if (LeavetypeId == 4)
                        {
                            leaves.MaternityType = lCode;
                        }
                        else
                        {
                            leaves.MaternityType = lCode;
                        }
                        leaves.BranchId = Convert.ToInt32(lCredentials.Branch);
                        leaves.DepartmentId = Convert.ToInt32(lCredentials.Department);
                        leaves.DesignationId = Convert.ToInt32(lCredentials.Designation);
                        db.Leaves.Add(leaves);
                        db.SaveChanges();

                        EmpLeaveBalance lopbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == 12 && a.Year == DateTime.Now.Year).FirstOrDefault();
                        lopbalance.LeaveTypeId = 12;
                        lopbalance.EmpId = lEmpId;
                        lopbalance.LeaveBalance = lopbalance.LeaveBalance + TotalDays;
                        lopbalance.Debits = lopbalance.Debits + TotalDays;
                        lopbalance.Year = DateTime.Now.Year;
                        db.Entry(lopbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        lResult.Message = string.Format("Extra Ordinary Leave treated as LOP");
                        return lResult;

                    }
                    else
                    {
                        DateTime lfouryears = Startdate.AddMonths(+48);
                        lfouryears = lfouryears.AddDays(-1);
                        if (Endate <= lfouryears)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days
                                                       // int days = 1;
                            int ldays = lExtraOrdinaryLeave - TotalDays;
                            // int lcount = lLeaves.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Count();
                            if (lExtraOrdinaryLeave >= TotalDays)
                            {
                                lResult.LeaveDays = TotalDays;
                                lResult.TotalLeaves = ldays;
                                lResult.TotalDays = TotalDays;
                                lResult.Success = true;
                                if (lResult.LeaveDays > 1)
                                {
                                    lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                }
                                else
                                {
                                    lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " day");
                                }
                                //return lResult;
                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format(" Only " + lExtraOrdinaryLeave + "Extra Ordinary Leaves are available.");
                                //return lResult;
                            }

                        }
                        else
                        {
                            lResult.Success = false;
                            lResult.Message = string.Format("ExtraOrdinary leave cannot exceed 4 years");
                            //return lResult;
                        }
                    }
                }
                else if (lLeaveCode == "SCL")
                {
                    if (lSpecialCasualLeave == 0)
                    {
                        lResult.Success = false;
                        lResult.Message = string.Format("No Leave Balance  available to apply SpecialCasual Leave");
                        //return lResult;
                    }
                    else
                    {
                        if (Startdate <= Endate)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days
                            int ldays = lSpecialCasualLeave - TotalDays;
                            int Count = db.HolidayList.Where(a => a.Date >= Startdate && a.Date <= Endate).Select(a => a.Date).Distinct().Count();
                            // int lcount = lLeaveHistory.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Approved").Sum(a => a.LeaveDays);
                            if (Count != 0)
                            {
                                int lTotalLeaves = TotalDays - Count;
                            }
                            else
                            {
                                int lTotalLeaves = TotalDays;
                            }
                            if ((TotalDays - Count) <= 7)
                            {
                                if (TotalDays <= lSpecialCasualLeave)
                                {
                                    lResult.LeaveDays = TotalDays - Count;
                                    lResult.TotalLeaves = ldays;
                                    lResult.TotalDays = TotalDays;
                                    lResult.Success = true;
                                    lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                    //return lResult;
                                }
                                else
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format(" Only " + lSpecialCasualLeave + " SpecialCasual Leaves are available.");
                                    //return lResult;
                                }
                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format("Special Casual Leave cannot be applied more than7 days");
                                //return lResult;
                            }
                        }
                    }
                }
                else if (lLeaveCode == "ML")
                {
                    if (lMedicalSickLeave == 0)
                    {
                        lResult.Success = false;
                        lResult.Message = string.Format("No Leave Balance available to apply Medical/Sick leave");
                        //return lResult;
                    }
                    else
                    {
                        DateTime JoinigDate = Convert.ToDateTime(db.Employes.Where(a => a.Id == lEmpId).Select(a => a.DOJ).FirstOrDefault());
                        DateTime lOneyear = JoinigDate.AddMonths(+12);
                        DateTime lTwentyFourYears = JoinigDate.AddYears(24);
                        // int lcount = lLeaveHistory.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Approved").Sum(a => a.LeaveDays);
                        if (Startdate >= lOneyear)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days
                            int ldays = lMedicalSickLeave - TotalDays;
                            lResult.TotalLeaves = TotalDays;
                            if (lcount <= 365)
                            {
                                if (TotalDays <= lMedicalSickLeave)
                                {
                                    lResult.LeaveDays = TotalDays;
                                    lResult.TotalLeaves = ldays;
                                    lResult.TotalDays = TotalDays;
                                    lResult.Success = true;
                                    if (lResult.LeaveDays > 1)
                                    {
                                        lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                    }
                                    else
                                    {
                                        lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " day");
                                    }
                                    //return lResult;
                                }
                                else
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format(" Only " + lMedicalSickLeave + " Medical Leaves are available.");
                                    //return lResult;
                                }
                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format("Only 365 leaves are allowed upto 24 years of service");
                                //return lResult;
                            }
                        }
                        else
                        {
                            lResult.Success = false;
                            lResult.Message = string.Format("No Medical Leave is eligible upto one year of joining date");
                            //return lResult;
                        }
                    }

                }
                else if (lLeaveCode == "C-OFF")
                {
                    int Count = lHolidays.Where(a => a.Date >= Startdate && a.Date <= Endate).Select(a => a.Date).Distinct().Count();
                    int lHolidayCount = db.HolidayList.Where(a => a.Date >= Startdate && a.Date <= Endate).Select(a => a.Date).Distinct().Count();
                    int TotalDayss = (Endate.Date - Startdate.Date).Days;
                    TotalDayss = TotalDayss + 1; // to get total days
                                                 // int ldays = lCompOff;
                    if (lCompOff < TotalDayss)
                    {
                        lResult.Success = false;
                        lResult.Message = string.Format("No Leave Balance available to apply CompensatoryOff leave");
                        //return lResult;
                    }
                    else
                    {
                        DateTime JoinigDate = Convert.ToDateTime(db.Employes.Where(a => a.Id == lEmpId).Select(a => a.DOJ).FirstOrDefault());

                        int lcountcomf = lLeaveHistory.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Pending").Sum(a => a.LeaveDays);
                        lcountcomf = lcountcomf + TotalDayss;
                        if (Startdate <= Endate)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days
                            int ldays = lCompOff - Count;

                            if (Designation == "9" || Designation == "10" || Designation == "11" || Designation == "21")
                            {
                                if (TotalDays >= 0)
                                {
                                    if (TotalDays >= 0)
                                    {
                                        //if (lHolidayCount != 0)
                                        // {
                                        int toaldays = TotalDays - Count;
                                        ldays = lCompOff - toaldays;
                                        lResult.LeaveDays = toaldays;
                                        lResult.TotalLeaves = ldays;
                                        lResult.TotalDays = TotalDays;
                                        lResult.Success = true;
                                        lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                        //return lResult;
                                        // }
                                    }
                                    // lResult.Success = false;
                                    // lResult.Message = string.Format(" Cannot apply C-OFF Leaves on Holiday days");
                                    //return lResult;

                                }
                                //else if (TotalDays > 10)
                                //{
                                //    lResult.Success = false;
                                //    lResult.Message = string.Format(" Cannot Apply more than 10 leaves at a time.");
                                //    //return lResult;
                                //}
                            }
                            else
                            {
                                if (lcountcomf < 11)
                                {
                                    if (TotalDays <= 10)
                                    {
                                        if (TotalDays >= 0)
                                        {
                                            //if (lHolidayCount != 0)
                                            // {
                                            int toaldays = TotalDays - Count;
                                            ldays = lCompOff - toaldays;
                                            lResult.LeaveDays = toaldays;
                                            lResult.TotalLeaves = ldays;
                                            lResult.TotalDays = TotalDays;
                                            lResult.Success = true;
                                            lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                            //return lResult;
                                            // }
                                        }
                                    }
                                    // lResult.Success = false;
                                    // lResult.Message = string.Format(" Cannot apply C-OFF Leaves on Holiday days");
                                    //return lResult;

                                }
                                else if (TotalDays > 10)
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format(" Cannot Apply more than 10 leaves at a time.");
                                    //return lResult;
                                }
                                else if (lcountcomf > 10)
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format(" Cannot Apply more than 10 C-OFF.");
                                }
                            }

                        }
                    }
                }
                else if (lLeaveCode == "LOP")
                {
                    int TotalDays = (Endate.Date - Startdate.Date).Days;
                    TotalDays = TotalDays + 1;
                    lResult.LeaveDays = TotalDays;
                    lResult.TotalDays = TotalDays;
                    int lbalancedays = lLOP + TotalDays;
                    lResult.TotalLeaves = TotalDays;
                    lResult.Success = true;
                    if (lResult.LeaveDays > 1)
                    {
                        lResult.Message = string.Format(lType + " " + "Leave applied successfully for " + lResult.LeaveDays + " days");
                    }
                    else
                    {
                        lResult.Message = string.Format(lType + " " + "Leave applied successfully for " + lResult.LeaveDays + " day");
                    }

                    //return lResult;

                    /*---------with considering Holiday code----------*/

                    //int Count = lHolidays.Where(a => a.Date >= Startdate).Where(k => k.Date <= Endate).Count();
                    //int lHolidayCount = lHolidays.Where(a => a.Date == Startdate).Where(k => k.Date == Endate).Count();
                    //if(lHolidayCount!=0)
                    //{
                    //    lResult.Success = false;
                    //    lResult.Message = string.Format(" Cannot apply Leave on Holiday days");
                    //    return lResult;
                    //}
                    //else
                    //{
                    //    int TotalDays = (Endate.Date - Startdate.Date).Days;
                    //    TotalDays = TotalDays + 1;
                    //    int ldays = TotalDays - Count;
                    //    lResult.LeaveDays = ldays;
                    //    lResult.TotalDays = TotalDays;
                    //    lResult.Success = true;
                    //    lResult.Message = string.Format(lType + " " + "Leave applied successfully");
                    //    return lResult;
                    //}

                }
                else if (lLeaveCode == "CW-OFF")
                {
                    if (lCWOFF == 0)
                    {
                        lResult.Success = false;
                        lResult.Message = string.Format("No Leave Balance available to apply coompensatory week off leave");
                        //return lResult;
                    }
                    else
                    {
                        string ldate = db.Employes.Select(a => a.RetirementDate.ToString()).FirstOrDefault();
                        if (ldate != null)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days


                            //cw-off
                            int Count = db.HolidayList.Where(a => a.Date >= Startdate && a.Date <= Endate).Select(a => a.Date).Distinct().Count();
                            int lHolidayCount = db.HolidayList.Where(a => a.Date >= Startdate && a.Date <= Endate).Select(a => a.Date).Distinct().Count();
                            // int lcount = lLeaveHistory.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Approved").Sum(a => a.LeaveDays);

                            //int lTotalss = 0;
                            if (Count != 0)
                            {
                                TotalDays = TotalDays - Count;
                            }
                            else
                            {
                                TotalDays = TotalDays;
                            }




                            int ldays = lCWOFF - TotalDays;
                            // int lcount = lLeaveHistory.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Approved").Sum(a => a.LeaveDays);
                            if (lcount <= 270)
                            {
                                if (TotalDays <= 120)
                                {
                                    if (lCWOFF >= TotalDays)
                                    {
                                        lResult.LeaveDays = TotalDays;
                                        lResult.TotalLeaves = ldays;
                                        lResult.TotalDays = TotalDays;
                                        lResult.Success = true;
                                        if (lResult.LeaveDays > 1)
                                        {
                                            lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                        }
                                        else
                                        {
                                            lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " day");
                                        }
                                        //return lResult;
                                    }
                                    else
                                    {
                                        lResult.Success = false;
                                        lResult.Message = string.Format(" Only " + lCWOFF + "  Leaves are available.");
                                        //return lResult;
                                    }

                                }
                                else
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format("Compensatory Week Off Leave cannot be applied more than 120 days");
                                    //return lResult;
                                }

                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format(" Only 270 Compensatory Week Off's Leaves can be applied in entire service period");
                                //return lResult;
                            }

                        }
                        else
                        {
                            lResult.Success = false;
                            lResult.Message = string.Format("No Compensatory Week Off  Leaves are allowed in preparatory retirement");
                            //return lResult;
                        }
                    }

                }

                else if (lLeaveCode == "W-Off")
                {
                    int TotalDays = (Endate.Date - Startdate.Date).Days;
                    TotalDays = TotalDays + 1;
                    lResult.LeaveDays = TotalDays;
                    lResult.TotalDays = TotalDays;
                    int lbalancedays = lwoff + TotalDays;
                    lResult.TotalLeaves = TotalDays;
                    lResult.Success = true;
                    if (lResult.LeaveDays > 1)
                    {
                        lResult.Message = string.Format(lType + " " + "Leave applied successfully for " + lResult.LeaveDays + " days");
                    }
                    else
                    {
                        lResult.Message = string.Format(lType + " " + "Leave applied successfully for " + lResult.LeaveDays + " day");
                    }

                    //return lResult;

                    /*---------with considering Holiday code----------*/

                    //int Count = lHolidays.Where(a => a.Date >= Startdate).Where(k => k.Date <= Endate).Count();
                    //int lHolidayCount = lHolidays.Where(a => a.Date == Startdate).Where(k => k.Date == Endate).Count();
                    //if(lHolidayCount!=0)
                    //{
                    //    lResult.Success = false;
                    //    lResult.Message = string.Format(" Cannot apply Leave on Holiday days");
                    //    return lResult;
                    //}
                    //else
                    //{
                    //    int TotalDays = (Endate.Date - Startdate.Date).Days;
                    //    TotalDays = TotalDays + 1;
                    //    int ldays = TotalDays - Count;
                    //    lResult.LeaveDays = ldays;
                    //    lResult.TotalDays = TotalDays;
                    //    lResult.Success = true;
                    //    lResult.Message = string.Format(lType + " " + "Leave applied successfully");
                    //    return lResult;
                    //}

                }
            }
            db.Dispose();
            LogInformation.Info("* 22 *");
            return lResult;

        }

        public static LeavesResult GetLeaveApplyWatchMan(int lEmpId, DateTime Startdate, DateTime Endate, int LeavetypeId)
        {
            int leavebalance = 0;
            LogInformation.Info("* 1 *");
            ContextBase db = new ContextBase();
            LeavesResult lResult = new LeavesResult();
            //var lEmpBalance = db.V_EmpLeaveBalance.ToList();
            //var lEmpLeaveBalance = db.EmpLeaveBalance.ToList();
            //var lLeaveHistory = db.V_LeaveHistory.ToList();
            var lcount = (from u in db.V_LeaveHistory
                          join ch in db.LeaveTypes on u.LeaveType equals ch.Id
                          where u.EmpId == lEmpId && u.Status == "Approved"
                          select (int?)u.LeaveDays).Sum() ?? 0;
            //int lcount = db.V_LeaveHistory.Where(a => a.LeaveType == LeavetypeId && a.EmpId == lEmpId && a.Status == "Approved").Sum(a => a.LeaveDays);
            var lLeaves = db.Leaves.ToList();
            var lHolidays = db.HolidayList.ToList();
            var lTypes = db.LeaveTypes.ToList();
            var lLeaveInfo = db.LeaveInfo.ToList();
            var velb = db.V_EmpLeaveBalance.Where(c => c.EmpId == lEmpId)
                               .Select(s => new
                               {
                                   s.CasualLeave,
                                   s.MedicalSickLeave,
                                   s.PrivilegeLeave,
                                   s.MaternityLeave,
                                   s.PaternityLeave,
                                   s.ExtraOrdinaryLeave,
                                   s.SpecialCasualLeave,
                                   s.CompensatoryOff,
                                   s.LOP,
                                   s.woff
                               }).FirstOrDefault();

            //int lCasualLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.CasualLeave).FirstOrDefault();
            int lCasualLeave = velb.CasualLeave;
            //int lMedicalSickLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.MedicalSickLeave).FirstOrDefault();
            int lMedicalSickLeave = velb.MedicalSickLeave;
            //int lPrivilegeLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.PrivilegeLeave).FirstOrDefault();
            int lPrivilegeLeave = velb.PrivilegeLeave;
            //int lMaternityLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.MaternityLeave).FirstOrDefault();
            int lMaternityLeave = velb.MaternityLeave;
            //int lPaternityLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.PaternityLeave).FirstOrDefault();
            int lPaternityLeave = velb.PaternityLeave;
            //int lExtraOrdinaryLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.ExtraOrdinaryLeave).FirstOrDefault();
            int lExtraOrdinaryLeave = velb.ExtraOrdinaryLeave;
            //int lSpecialCasualLeave = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.SpecialCasualLeave).FirstOrDefault();
            int lSpecialCasualLeave = velb.SpecialCasualLeave;
            //int lCompOff = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a=> a.CompensatoryOff).FirstOrDefault();
            int lCompOff = velb.CompensatoryOff;
            //int lLOP = lEmpBalance.Where(a => a.EmpId == lEmpId).Select(a => a.LOP).FirstOrDefault();
            int lLOP = velb.LOP;
            int lwoff = velb.woff;
            int lRetrieveleaveType = lTypes.Where(a => a.Id == LeavetypeId).Select(a => a.Id).FirstOrDefault();
            string lLeaveCode = lTypes.Where(a => a.Id == LeavetypeId).Select(a => a.Code).FirstOrDefault();
            leavebalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == LeavetypeId && a.Year == DateTime.Now.Year).Select(a => a.LeaveBalance).FirstOrDefault();
            string lType = lTypes.Where(a => a.Id == LeavetypeId).Select(a => a.Type).FirstOrDefault();
            int year = DateTime.Now.Year;
            DateTime today = DateTime.Now;
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(year, 12, 31);
            if (LeavetypeId == lRetrieveleaveType)
            {
                if (lLeaveCode == "CL")
                {
                    if (lCasualLeave == 0)
                    {
                        lResult.Message = string.Format("No Leave Balance available to apply Casual leave");
                        return lResult;
                    }
                    else
                    {
                        int TotalDays = (Endate.Date - Startdate.Date).Days;
                        TotalDays = TotalDays + 1; // to get total days
                        int ldays = lCasualLeave - TotalDays;
                        if (TotalDays <= 7)
                        {
                            if (lCasualLeave >= TotalDays)
                            {
                                if (TotalDays <= 7)
                                {
                                    lResult.LeaveDays = TotalDays;       // leave Appied days
                                    lResult.TotalDays = TotalDays;  // total cls applied now 
                                    lResult.TotalLeaves = ldays;   // balance left
                                    lResult.Success = true;
                                    lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                    return lResult;
                                }
                                else
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format("Casual Leave should not exceed more than 7 days");
                                    return lResult;
                                }
                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format(" Only " + lCasualLeave + " Casual Leaves are available.");
                                return lResult;
                            }

                        }
                        else
                        {
                            lResult.Success = false;
                            lResult.Message = string.Format("At a time more than 7 Casual Leaves cannot be applied");
                            return lResult;
                        }

                    }

                }
                else if (lLeaveCode == "MTL")
                {
                    string lFemale = db.Employes.Where(a => a.Id == lEmpId).Select(a => a.Gender).FirstOrDefault();

                    if (lFemale == "Female")
                    {
                        int lcount1 = db.Leaves.Where(a => a.LeaveType == LeavetypeId && a.EmpId == lEmpId).Count();
                        if (lcount1 <= 2)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            int TotalLeavedays = TotalDays;
                            TotalDays = TotalDays + 1;
                            DateTime lsixMonths = Startdate.AddMonths(+6);
                            if (Startdate <= lsixMonths && TotalDays <= 184)
                            {
                                // to get total days
                                int days = 1;

                                int ldays = lMaternityLeave - TotalDays;
                                int lcounts = lLeaves.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Count();
                                var count = (from u in db.Leaves
                                             join ch in db.LeaveTypes on u.LeaveType equals ch.Id
                                             where u.EmpId == lEmpId
                                             select (int?)u.LeaveDays).Sum() ?? 0;
                                if (count >= 365)
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format("Maternity Leave cannot be applied");
                                    return lResult;
                                }
                                lResult.LeaveDays = TotalLeavedays;
                                lResult.TotalLeaves = ldays;
                                lResult.TotalDays = TotalLeavedays;
                                lResult.Success = true;
                                lResult.Message = string.Format(lType + " " + "applied successfully for " + TotalDays + " days");
                                return lResult;
                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format("Maternity Leave should not exceed more than six months");
                                return lResult;
                            }
                        }
                        else
                        {
                            DateTime date = Startdate.AddDays(+lMaternityLeave);
                            string Totaldate = date.ToShortDateString();
                            lResult.Success = false;
                            //lResult.Message = string.Format("No Leaves are available to apply Maternity leave");
                            lResult.Message = string.Format("MaternityLeave leave can be applied upto" + Totaldate);
                            return lResult;
                        }
                    }

                    else
                    {
                        lResult.Message = string.Format("Only Female employees are eligable to apply");
                        return lResult;
                    }

                }
                else if (lLeaveCode == "PL")
                {
                    if (lPrivilegeLeave == 0)
                    {
                        lResult.Message = string.Format("No Leave Balance available to apply Privilege leave");
                        return lResult;
                    }
                    else
                    {
                        string ldate = db.Employes.Select(a => a.RetirementDate.ToString()).FirstOrDefault();
                        if (ldate != null)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days
                            int ldays = lPrivilegeLeave - TotalDays;
                            // int lcount = lLeaveHistory.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Approved").Sum(a => a.LeaveDays);
                            if (lcount <= 270)
                            {
                                if (TotalDays <= 120)
                                {
                                    if (lPrivilegeLeave >= TotalDays)
                                    {
                                        lResult.LeaveDays = TotalDays;
                                        lResult.TotalLeaves = ldays;
                                        lResult.TotalDays = TotalDays;
                                        lResult.Success = true;
                                        lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                        return lResult;
                                    }
                                    else
                                    {
                                        lResult.Success = false;
                                        lResult.Message = string.Format(" Only " + lPrivilegeLeave + " Privilege Leaves are available.");
                                        return lResult;
                                    }

                                }
                                else
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format("Privilege Leave cannot be applied more than 120 days");
                                    return lResult;
                                }

                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format(" Only 270 Privilege Leaves can be applied in entire service period");
                                return lResult;
                            }

                        }
                        else
                        {
                            lResult.Success = false;
                            lResult.Message = string.Format("No Privilege Leaves are allowed in preparatory retirement");
                            return lResult;
                        }
                    }

                }

                else if (lLeaveCode == "EOL")
                {
                    if (lExtraOrdinaryLeave == 0)
                    {
                        string lCode = db.LeaveTypes.Where(a => a.Id == LeavetypeId).Select(a => a.Code).FirstOrDefault();
                        string userqry = "Select e.EmpId,d.Code, e.Id as EmpPK,e.SanctioningAuthority,e.RetirementDate, e.ControllingAuthority,e.Branch,e.Department,e.CurrentDesignation,e.LoginMode "
                               + "FROM Employees e join Designations d on d.Id=e.CurrentDesignation "
                               + "where e.Id = " + lEmpId;
                        SqlHelper sh = new SqlHelper();
                        DataTable dtuser = sh.Get_Table_FromQry(userqry);
                        int TotalDays = (Endate.Date - Startdate.Date).Days;
                        TotalDays = TotalDays + 1;
                        LoginCredential lCredentials = new LoginCredential();
                        lCredentials.EmpId = dtuser.Rows[0]["EmpId"].ToString();
                        lCredentials.EmpPkId = dtuser.Rows[0]["EmpPK"].ToString();
                        lCredentials.Branch = dtuser.Rows[0]["Branch"].ToString();
                        lCredentials.Department = dtuser.Rows[0]["Department"].ToString();
                        lCredentials.Designation = dtuser.Rows[0]["CurrentDesignation"].ToString();
                        lCredentials.LoginMode = dtuser.Rows[0]["LoginMode"].ToString();
                        int lControlling = Convert.ToInt32(dtuser.Rows[0]["ControllingAuthority"].ToString());
                        int lSacantioning = Convert.ToInt32(dtuser.Rows[0]["SanctioningAuthority"].ToString());
                        Leaves leaves = new Leaves();
                        DateTime lStartdate = Startdate;
                        DateTime lEnddate = Endate;
                        leaves.EmpId = lEmpId;
                        leaves.ControllingAuthority = lControlling;
                        leaves.SanctioningAuthority = lSacantioning;
                        leaves.LeaveType = 12;
                        leaves.StartDate = lStartdate;
                        leaves.EndDate = lEnddate;
                        leaves.leave_balance = leavebalance;
                        leaves.Reason = "ExtraOrdinary Leave treated as LOP";
                        leaves.UpdatedDate = GetCurrentTime(DateTime.Now);
                        leaves.UpdatedBy = lEmpId.ToString();
                        leaves.Status = "Pending";
                        leaves.LeaveDays = TotalDays;
                        leaves.TotalDays = TotalDays;
                        // leaves.lleaves.LeavesYear = DateTime.Today.Year;
                        leaves.LeaveTimeStamp = GetCurrentTime(DateTime.Now);
                        if (LeavetypeId == 4)
                        {
                            leaves.MaternityType = lCode;
                        }
                        else
                        {
                            leaves.MaternityType = lCode;
                        }
                        leaves.BranchId = Convert.ToInt32(lCredentials.Branch);
                        leaves.DepartmentId = Convert.ToInt32(lCredentials.Department);
                        leaves.DesignationId = Convert.ToInt32(lCredentials.Designation);
                        db.Leaves.Add(leaves);
                        db.SaveChanges();

                        EmpLeaveBalance lopbalance = db.EmpLeaveBalance.Where(a => a.EmpId == lEmpId && a.LeaveTypeId == 12 && a.Year == DateTime.Now.Year).FirstOrDefault();
                        lopbalance.LeaveTypeId = 12;
                        lopbalance.EmpId = lEmpId;
                        lopbalance.LeaveBalance = lopbalance.LeaveBalance + TotalDays;
                        lopbalance.Debits = lopbalance.Debits + TotalDays;
                        lopbalance.Year = DateTime.Now.Year;
                        db.Entry(lopbalance).State = EntityState.Modified;
                        db.SaveChanges();

                        lResult.Message = string.Format("Extra Ordinary Leave treated as LOP");
                        return lResult;

                    }
                    else
                    {
                        DateTime lfouryears = Startdate.AddMonths(+48);
                        lfouryears = lfouryears.AddDays(-1);
                        if (Endate <= lfouryears)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days
                                                       // int days = 1;
                            int ldays = lExtraOrdinaryLeave - TotalDays;
                            //int lcount = lLeaves.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Count();
                            if (lExtraOrdinaryLeave >= TotalDays)
                            {
                                lResult.LeaveDays = TotalDays;
                                lResult.TotalLeaves = ldays;
                                lResult.TotalDays = TotalDays;
                                lResult.Success = true;
                                if (lResult.LeaveDays > 1)
                                {
                                    lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " days");
                                }
                                else
                                {
                                    lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + " day");
                                }
                                return lResult;
                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format(" Only " + lExtraOrdinaryLeave + "Extra Ordinary Leaves are available.");
                                return lResult;
                            }

                        }
                        else
                        {
                            lResult.Success = false;
                            lResult.Message = string.Format("ExtraOrdinary leave cannot exceed 4 years");
                            return lResult;
                        }
                    }
                }
                else if (lLeaveCode == "SCL")
                {
                    if (lSpecialCasualLeave == 0)
                    {
                        lResult.Message = string.Format("No Leave Balance available to apply SpecialCasual Leave");
                        return lResult;
                    }
                    else
                    {
                        if (Startdate <= Endate)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days
                            int ldays = lSpecialCasualLeave - TotalDays;
                            if (TotalDays <= 7)
                            {
                                if (TotalDays <= lSpecialCasualLeave)
                                {
                                    lResult.LeaveDays = TotalDays;
                                    lResult.TotalLeaves = ldays;
                                    lResult.TotalDays = TotalDays;
                                    lResult.Success = true;
                                    if (lResult.LeaveDays > 1)
                                    {
                                        lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + "days");
                                    }
                                    else
                                    {
                                        lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + "day");
                                    }
                                    return lResult;
                                }
                                else
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format(" Only " + lSpecialCasualLeave + " SpecialCasual Leaves are available.");
                                    return lResult;
                                }
                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format("Special Casual Leave cannot be applied more than7 days");
                                return lResult;
                            }
                        }
                    }
                }
                else if (lLeaveCode == "ML")
                {
                    if (lMedicalSickLeave == 0)
                    {
                        lResult.Message = string.Format("No Leave Balance  available to apply Medical/Sick leave");
                        return lResult;
                    }
                    else
                    {
                        DateTime JoinigDate = Convert.ToDateTime(db.Employes.Where(a => a.Id == lEmpId).Select(a => a.DOJ).FirstOrDefault());
                        DateTime lOneyear = JoinigDate.AddMonths(+12);
                        DateTime lTwentyFourYears = JoinigDate.AddYears(24);
                        // int lcount = lLeaveHistory.Where(a => a.LeaveType == LeavetypeId).Where(a => a.EmpId == lEmpId).Where(a => a.Status == "Approved").Sum(a => a.LeaveDays);
                        if (Startdate >= lOneyear)
                        {
                            int TotalDays = (Endate.Date - Startdate.Date).Days;
                            TotalDays = TotalDays + 1; // to get total days
                            int ldays = lMedicalSickLeave - TotalDays;
                            lResult.TotalLeaves = TotalDays;
                            if (lcount <= 365)
                            {
                                if (TotalDays <= lMedicalSickLeave)
                                {
                                    lResult.LeaveDays = TotalDays;
                                    lResult.TotalLeaves = ldays;
                                    lResult.TotalDays = TotalDays;
                                    lResult.Success = true;
                                    if (lResult.LeaveDays > 1)
                                    {
                                        lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + "days");
                                    }
                                    else
                                    {
                                        lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + "day");
                                    }
                                    return lResult;
                                }
                                else
                                {
                                    lResult.Success = false;
                                    lResult.Message = string.Format(" Only " + lMedicalSickLeave + " Medical Leaves are available.");
                                    return lResult;
                                }
                            }
                            else
                            {
                                lResult.Success = false;
                                lResult.Message = string.Format("Only 365 leaves are allowed upto 24 years of service");
                                return lResult;
                            }
                        }
                        else
                        {
                            lResult.Success = false;
                            lResult.Message = string.Format("No Medical Leave is eligible upto one year of joining date");
                            return lResult;
                        }
                    }

                }
                else if (lLeaveCode == "LOP")
                {
                    int TotalDays = (Endate.Date - Startdate.Date).Days;
                    TotalDays = TotalDays + 1;
                    lResult.LeaveDays = TotalDays;
                    lResult.TotalDays = TotalDays;
                    lResult.Success = true;

                    if (lResult.LeaveDays > 1)
                    {
                        lResult.Message = string.Format(lType + " " + "Leave applied successfully for" + lResult.LeaveDays + " days");
                    }
                    else
                    {
                        lResult.Message = string.Format(lType + " " + "Leave applied successfully for" + lResult.LeaveDays + " day");
                    }
                    return lResult;
                }
                else if (lLeaveCode == "woff")
                {
                    int TotalDays = (Endate.Date - Startdate.Date).Days;
                    TotalDays = TotalDays + 1;
                    lResult.LeaveDays = TotalDays;
                    lResult.TotalDays = TotalDays;
                    lResult.Success = true;

                    if (lResult.LeaveDays > 1)
                    {
                        lResult.Message = string.Format(lType + " " + "Leave applied successfully for" + lResult.LeaveDays + " days");
                    }
                    else
                    {
                        lResult.Message = string.Format(lType + " " + "Leave applied successfully for" + lResult.LeaveDays + " day");
                    }
                    return lResult;
                }
                else if (lLeaveCode == "C-OFF")
                {
                    int TotalDayss = (Endate.Date - Startdate.Date).Days;
                    TotalDayss = TotalDayss + 1;
                    if (lCompOff < TotalDayss)
                    {
                        lResult.Success = false;
                        lResult.Message = string.Format("No Leave Balance available to apply Compensatory Off leave");
                        return lResult;
                    }
                    else
                    {
                        int TotalDays = (Endate.Date - Startdate.Date).Days;
                        TotalDays = TotalDays + 1;
                        int ldays = lCompOff - TotalDays;
                        lResult.LeaveDays = TotalDays;
                        lResult.TotalDays = ldays;
                        lResult.TotalLeaves = ldays;
                        lResult.Success = true;
                        if (lResult.LeaveDays > 0)
                        {
                            lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + "days");
                        }
                        else
                        {
                            lResult.Message = string.Format(lType + " " + "applied successfully for " + lResult.LeaveDays + "day");
                        }
                        return lResult;
                    }
                }
            }
            db.Dispose();
            LogInformation.Info("* 11 *");
            return lResult;

        }


        public static string SendEmails(DateTime StartDate, DateTime EnDate, int lControllingId, int lSancationingId, int lempId, int LeaveType, int LeaveDays, string Reason, string Status, string AppliedValue)
        {
            //check SMS required or not
            if (System.Configuration.ConfigurationManager.AppSettings["sendMail"] == "false")
            {
                return "Mail message is not sending. Please check configurations";
            }

            String lMessage = string.Empty;
            try
            {
                ContextBase db = new ContextBase();
                string lControllingEmail = db.Employes.Where(a => a.Id == lControllingId).Select(a => a.OfficalEmailId).FirstOrDefault();
                string lSancatiningEmail = db.Employes.Where(a => a.Id == lSancationingId).Select(a => a.OfficalEmailId).FirstOrDefault();
                string lEmployeeEmail = db.Employes.Where(a => a.Id == lempId).Select(a => a.OfficalEmailId).FirstOrDefault();
                string lEmpCode = db.Employes.Where(a => a.Id == lempId).Select(a => a.EmpId).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.Id == lempId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.Id == lempId).Select(a => a.LastName).FirstOrDefault();
                string lleaveType = db.LeaveTypes.Where(a => a.Id == LeaveType).Select(a => a.Type).FirstOrDefault();
                string lEmployeeFullName = lFirstName + " " + lLastName;

                if (Status == "Leave" && AppliedValue == "0")
                {
                    //Emp ,Controlling

                    // string lSubject = "Leave Applied Details:";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A Leave Request Sent From ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>LeaveType: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lleaveType);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Reason);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = lleaveType;
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lControllingEmail);
                    //message.To.Add(lEmployeeEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lControllingEmail);
                    message.To.Add(lEmployeeEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    //LogInformation.Info("Email Sent");
                }
                else if (Status == "Forwarded" && AppliedValue == "0") // controlling authority forward message
                {
                    //sancationing email

                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A Leave Request Forwarded for Sanctioning Authority</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A leave Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>LeaveType: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lleaveType);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Reason);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "Leave Forwarded";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lControllingEmail);
                    //message.To.Add(lSancatiningEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/
                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lSancatiningEmail);
                    message.To.Add(lControllingEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    //LogInformation.Info("Email Sent");
                }
                else if (Status == "Approved" && AppliedValue == "1")
                {
                    // Emp, Sancationing

                    //  string lSubject = "Approved";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A Leave Request Approved</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A leave Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>LeaveType: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lleaveType);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Reason);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "Leave Approved";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lEmployeeEmail);
                    //message.To.Add(lSancatiningEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lEmployeeEmail);
                    message.To.Add(lSancatiningEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    //LogInformation.Info("Email Sent");
                }
                else if (Status == "Cancelled" && AppliedValue == "0") // controlling authority cancel
                {
                    // Emp, controlling

                    //  string lSubject = " Cancelled ";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A Leave Request Cancelled from Controlling Authority</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A leave Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>LeaveType: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lleaveType);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Reason);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "Leave Cancelled";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lEmployeeEmail);
                    //message.To.Add(lControllingEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lEmployeeEmail);
                    message.To.Add(lControllingEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    //LogInformation.Info("Email Sent");
                }
                else if (Status == "Cancelled" && AppliedValue == "1") // sancationing authority cancel
                {
                    // Emp, Sancationing

                    // string lSubject = " Cancelled ";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A Leave Request Cancelled from Sancationing Authority</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A leave Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>LeaveType: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lleaveType);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Reason);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "Leave Cancelled";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lEmployeeEmail);
                    //message.To.Add(lSancatiningEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lEmployeeEmail);
                    message.To.Add(lSancatiningEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    //LogInformation.Info("Email Sent");
                }
                else if (Status == "Denied" && AppliedValue == "0") // controlling authority cancel
                {
                    // Emp, controlling

                    //  string lSubject = " Cancelled ";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A Leave Request Denied from Controlling Authority</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A leave Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>LeaveType: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lleaveType);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Reason);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "Leave Denied";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lEmployeeEmail);
                    //message.To.Add(lControllingEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lEmployeeEmail);
                    message.To.Add(lControllingEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    //LogInformation.Info("Email Sent");
                }
                else if (Status == "Denied" && AppliedValue == "1") // sancationing authority cancel
                {
                    // Emp, Sancationing

                    // string lSubject = " Cancelled ";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A Leave Request Denied from Sancationing Authority</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A leave Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>LeaveType: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lleaveType);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Reason);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "Leave Denied";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lEmployeeEmail);
                    //message.To.Add(lSancatiningEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lEmployeeEmail);
                    message.To.Add(lSancatiningEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    //LogInformation.Info("Email Sent");
                }
            }
            catch (Exception ex)
            {
                //LogInformation.Info("Official Emailid is Empty");
                lMessage = ex.ToString();
            }

            return lMessage;
        }

        public static string SendSms(DateTime StartDate, DateTime EnDate, int lControllingId, int lSancationingId, int lempId, string Status, string AppliedValue)
        {
            String lMessage = string.Empty;
            //check SMS required or not 
            if (System.Configuration.ConfigurationManager.AppSettings["sendSMS"] == "false")
            {
                lMessage= "SMS message is not sending. Please check configurations";
                return lMessage;
            }
            Stream os = null;
            
            try
            {
                ContextBase db = new ContextBase();
                string lControllingmobileno = db.Employes.Where(a => a.Id == lControllingId).Select(a => a.MobileNumber).FirstOrDefault();
                string lSancatiningmobileno = db.Employes.Where(a => a.Id == lSancationingId).Select(a => a.MobileNumber).FirstOrDefault();
                string lEmployeemobileno = db.Employes.Where(a => a.Id == lempId).Select(a => a.MobileNumber).FirstOrDefault();
                string lEmpCode = db.Employes.Where(a => a.Id == lempId).Select(a => a.EmpId).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.Id == lempId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.Id == lempId).Select(a => a.LastName).FirstOrDefault();
                string lEmployeeFullName = lFirstName + " " + lLastName;
                string lStartDate = StartDate.ToString("dd-MMM-yyyy");
                string lEnDate = EnDate.ToString("dd-MMM-yyyy");
                string countrycode = "91";
                if (Status == "Leave" && AppliedValue == "0")
                {
                    //Emp ,Controlling

                    //    string lcontent1 = "Leave Request sent from "+lEmployeeFullName+" "+(lEmpCode)+" Starting from "+ lStartDate + " to "+lEnDate;
                    //string lcontent2 = "Thank you,";
                    //string lcontent3 = "TSCAB-HRMS";
                    //string lcontent=   lcontent1 + '\n' + lcontent2+'\n' + lcontent3;
                    string lcontent = "Leave Request sent from " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate + "    Thank you TSCAB-HRMS ";
                    string lcontrolling = countrycode + "" + lControllingmobileno;
                    string lmobilenumber = lcontrolling;
                    // String URI = "http://push.vg4mobile.com/newBulkClient.jsp?senderID=VGTHYD&msisdn="+ lmobilenumber + "&userid=709&msg=" + lcontent + "&pwd=vgthyd1234";
                    String URI = "http://bulkpush.mytoday.com/BulkSms/SingleMsgApi?feedid=351607&username=8686593293&pwd=Tscabaj!1234&time=&msisdn=" + lmobilenumber + "&Text=" + lcontent;


                    try
                    {
                        WebRequest webRequest = WebRequest.Create(URI);
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                        webRequest.Method = "POST";
                        //webRequest.Timeout = 60000; //Put min 1 minutes   
                        byte[] bytes = Encoding.ASCII.GetBytes(URI);

                        webRequest.ContentLength = bytes.Length;   //Count bytes to send     
                        os = webRequest.GetRequestStream();
                        os.Write(bytes, 0, bytes.Length);         //Send it 
                        // WebClient webClient = new WebClient();
                        //  Stream stream = webClient.OpenRead(URI);
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response is HttpWebResponse)
                        {
                            switch (((HttpWebResponse)ex.Response).StatusCode)
                            {
                                case HttpStatusCode.NotFound:
                                    break;

                                default:
                                    throw ex;
                            }
                        }
                    }

                }
                //else if (Status == "Forwarded" && AppliedValue == "0") // controlling authority forward message
                //{
                //    //sancationing email

                //    //string lcontent1 = "Leave Request sent from " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate;
                //    //string lcontent4 = "has been Forwarded to Sanctioning Authority";
                //    //string lcontent2 = "Thank you,";
                //    //string lcontent3 = "TSCAB-HRMS";
                //    //string lcontent = lcontent1 + '\n' + lcontent4 + '\n' + lcontent2 + '\n'+ lcontent3;
                //    string lcontent = "Leave Request Forwarded for Sanctioning Authority " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate + "   Thank you TSCAB-HRMS ";

                //    string lmobilebo = countrycode + "" + lSancatiningmobileno;
                //    string lmobilenumber = lmobilebo;
                //    //String URI = "http://push.vg4mobile.com/newBulkClient.jsp?senderID=VGTHYD&msisdn=" + lmobilebo + "&userid=709&msg=" + lcontent + "&pwd=vgthyd1234";
                //    String URI = "http://bulkpush.mytoday.com/BulkSms/SingleMsgApi?feedid=351607&username=8686593293&pwd=Tscab@123&time=&msisdn=" + lmobilenumber + "&Text=" + lcontent;


                //    try
                //    {
                //        WebRequest webRequest = WebRequest.Create(URI);
                //        webRequest.ContentType = "application/x-www-form-urlencoded";
                //        webRequest.Method = "POST";
                //        //webRequest.Timeout = 60000; //Put min 1 minutes   
                //        byte[] bytes = Encoding.ASCII.GetBytes(URI);

                //        webRequest.ContentLength = bytes.Length;   //Count bytes to send     
                //        os = webRequest.GetRequestStream();
                //        os.Write(bytes, 0, bytes.Length);         //Send it 
                //                                                  // WebClient webClient = new WebClient();
                //                                                  // Stream stream = webClient.OpenRead(URI);
                //    }
                //    catch (WebException ex)
                //    {
                //        if (ex.Response is HttpWebResponse)
                //        {
                //            switch (((HttpWebResponse)ex.Response).StatusCode)
                //            {
                //                case HttpStatusCode.NotFound:
                //                    break;

                //                default:
                //                    throw ex;
                //            }
                //        }
                //    }
                //}
                else if (Status == "Approved" && AppliedValue == "1")
                {
                    // Emp, Sancationing
                    //string lcontent1 = "Leave Request sent from " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate;
                    // string lcontent2 = "Thank you,";
                    // string lcontent3 = "TSCAB-HRMS";
                    // string lcontent4 = "has been Approved ";
                    // string lcontent = lcontent1 + '\n' + lcontent4 + '\n' + lcontent2 + '\n' + lcontent3;
                    string lcontent = "Leave Request Approved for " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate + "   Thank you TSCAB-HRMS ";
                    string lEmployee = countrycode + "" + lEmployeemobileno;
                    string lmobilebo = lEmployee;
                    //String URI = "http://push.vg4mobile.com/newBulkClient.jsp?senderID=VGTHYD&msisdn=" + lmobilebo + "&userid=709&msg=" + lcontent + "&pwd=vgthyd1234";
                    String URI = "http://bulkpush.mytoday.com/BulkSms/SingleMsgApi?feedid=351607&username=8686593293&pwd=Tscabaj!1234&time=&msisdn=" + lmobilebo + "&Text=" + lcontent;
                    try
                    {
                        WebRequest webRequest = WebRequest.Create(URI);
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                        webRequest.Method = "POST";
                        // webRequest.Timeout = 60000; //Put min 1 minutes   
                        byte[] bytes = Encoding.ASCII.GetBytes(URI);

                        webRequest.ContentLength = bytes.Length;   //Count bytes to send     
                        os = webRequest.GetRequestStream();
                        os.Write(bytes, 0, bytes.Length);         //Send it 
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response is HttpWebResponse)
                        {
                            switch (((HttpWebResponse)ex.Response).StatusCode)
                            {
                                case HttpStatusCode.NotFound:
                                    break;

                                default:
                                    throw ex;
                            }
                        }
                    }
                }
                else if (Status == "Cancelled" && AppliedValue == "0") // controlling authority cancel
                {
                    // Emp, controlling
                    //string lcontent1 = "Leave Request sent from " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate;
                    //string lcontent2 = "Thank you,";
                    //string lcontent3 = "TSCAB-HRMS";
                    //string lcontent4 = "has been Cancelled from Controlling Authority ";
                    //string lcontent = lcontent1 + '\n' + lcontent4 + '\n' + lcontent2 + '\n' + lcontent3;
                    string lcontent = "Leave Request Cancelled from Controlling Authority " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate + "   Thank you TSCAB-HRMS ";
                    string lEmployee = countrycode + "" + lEmployeemobileno;
                    string lmobilebo = lEmployee;
                    // String URI = "http://push.vg4mobile.com/newBulkClient.jsp?senderID=VGTHYD&msisdn=" + lmobilebo + "&userid=709&msg=" + lcontent + "&pwd=vgthyd1234";
                    String URI = "http://bulkpush.mytoday.com/BulkSms/SingleMsgApi?feedid=351607&username=8686593293&pwd=Tscabaj!1234&time=&msisdn=" + lmobilebo + "&Text=" + lcontent;
                    try
                    {
                        WebRequest webRequest = WebRequest.Create(URI);
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                        webRequest.Method = "POST";
                        //webRequest.Timeout = 60000; //Put min 1 minutes   
                        byte[] bytes = Encoding.ASCII.GetBytes(URI);

                        webRequest.ContentLength = bytes.Length;   //Count bytes to send     
                        os = webRequest.GetRequestStream();
                        os.Write(bytes, 0, bytes.Length);         //Send it 
                                                                  // WebClient webClient = new WebClient();
                                                                  //Stream stream = webClient.OpenRead(URI);
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response is HttpWebResponse)
                        {
                            switch (((HttpWebResponse)ex.Response).StatusCode)
                            {
                                case HttpStatusCode.NotFound:
                                    break;

                                default:
                                    throw ex;
                            }
                        }
                    }
                }
                else if (Status == "Cancelled" && AppliedValue == "1") // sancationing authority cancel
                {
                    //// Emp, Sancationing
                    //string lcontent1 = "Leave Request sent from " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate;
                    //string lcontent2 = "Thank you,";
                    //string lcontent3 = "TSCAB-HRMS";
                    //string lcontent4 = "has been Cancelled from Sanctioning Authority ";
                    //string lcontent = lcontent1 + '\n' + lcontent4 + '\n' + lcontent2 + '\n' + lcontent3;
                    string lcontent = "Leave Request Cancelled from Sancationing Authority " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate + "  Thank you TSCAB-HRMS ";
                    string lEmployee = countrycode + "" + lEmployeemobileno;
                    string lmobilebo = lEmployee;
                    // String URI = "http://push.vg4mobile.com/newBulkClient.jsp?senderID=VGTHYD&msisdn=" + lmobilebo + "&userid=709&msg=" + lcontent + "&pwd=vgthyd1234";
                    String URI = "http://bulkpush.mytoday.com/BulkSms/SingleMsgApi?feedid=351607&username=8686593293&pwd=Tscabaj!1234&time=&msisdn=" + lmobilebo + "&Text=" + lcontent;
                    try
                    {
                        WebRequest webRequest = WebRequest.Create(URI);
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                        // webRequest.Method = "POST";
                        webRequest.Timeout = 60000; //Put min 1 minutes   
                        byte[] bytes = Encoding.ASCII.GetBytes(URI);

                        webRequest.ContentLength = bytes.Length;   //Count bytes to send     
                        os = webRequest.GetRequestStream();
                        os.Write(bytes, 0, bytes.Length);         //Send it 
                                                                  // WebClient webClient = new WebClient();
                                                                  // Stream stream = webClient.OpenRead(URI);
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response is HttpWebResponse)
                        {
                            switch (((HttpWebResponse)ex.Response).StatusCode)
                            {
                                case HttpStatusCode.NotFound:
                                    break;

                                default:
                                    throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }
            return lMessage;

        }


        //Getting current Month Date
        public static IEnumerable<DateTime> AllDatesInMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= days; day++)
            {
                yield return new DateTime(year, month, day);
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

    }
}