using BusLogic.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
//using Microsoft.SqlServer.Server;

namespace BusLogic
{
    public class TimeSheet : CommonService
    {

        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(TimeSheet));
        private string _mavenConnStr;
        private string _timesheetDB;
        private DateTime _DateToProcess1 = DateTime.Now;
        private DateTime _DateToProcess2 = DateTime.Now;


        private string _Day;
        private string _Month;
        private string _Year;

        private TimeSpan _StartTimeForHalfDay;
        private TimeSpan _EndTimeForHalfDay;
        private TimeSpan TimeForHalfDay1Hour;

        private bool _isSingleDay = true;
        private bool _Holiday = false;
        private int _UserOutTimeGracePeriodMins = 0;
        private int _ShiftUserInOutTimeToleranceMins = 0;


        public TimeSheet(string mavenConnStr, string timesheetConnstr, string DateToProcess) : base(mavenConnStr)
        {
            _mavenConnStr = mavenConnStr;
            _timesheetDB = timesheetConnstr;

            //@ development
            if (DateToProcess != "")
            {
                _DateToProcess1 = DateTime.Parse(DateToProcess);
                _DateToProcess2 = DateTime.Parse(DateToProcess);
            }

            _Day = int.Parse(_DateToProcess1.ToString("dd")).ToString();
            _Month = int.Parse(_DateToProcess1.ToString("MM")).ToString();
            _Year = _DateToProcess1.ToString("yyyy");

            //half day variable
            //var TimeForHalfDay = System.Configuration.ConfigurationManager.AppSettings.Get("TimeConsiderForHalfDay").ToString();
            var TimeForHalfDay = System.Configuration.ConfigurationManager.AppSettings.Get("TimeConsiderForHalfDayNew").ToString();
            TimeForHalfDay1Hour = TimeSpan.Parse(TimeForHalfDay);
            //_StartTimeForHalfDay = TimeSpan.Parse(TimeForHalfDay.Substring(0, TimeForHalfDay.IndexOf("~")));
            //_EndTimeForHalfDay = TimeSpan.Parse(TimeForHalfDay.Substring(TimeForHalfDay.LastIndexOf('~') + 1));

            //user checkout time grace period
            _UserOutTimeGracePeriodMins = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("UserOutTimeGracePeriodMins").ToString());

            //Shift User In Out Time Tolerance Mins
            _ShiftUserInOutTimeToleranceMins = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("ShiftUserInOutTimeToleranceMins").ToString());

        }

        //Generating Monthly wise report from timesheet logs
        public void GenerateTimesheetReportsData(bool isSingleDay, DateTime? datetoprocess, object EmpId, string DeviceId, string BranchId)
        {
            StringBuilder qry = new StringBuilder();
            DateTime ToDate = DateTime.ParseExact("11/11/2024", "d/M/yyyy", CultureInfo.InvariantCulture);
            string dateString = ToDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            DateTime formattedDateTime = DateTime.ParseExact(dateString, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            String formattedToDate = ToDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime formattedDate = DateTime.ParseExact(formattedToDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime startTime;
            DateTime endTime;
            StringBuilder batchUpdateQuery = new StringBuilder();
            SqlHelper sh = new SqlHelper(_mavenConnStr);
            _isSingleDay = isSingleDay;

            if (datetoprocess != null)
            {
                _DateToProcess1 = DateTime.Parse(datetoprocess.ToString());
            }

            //if not single day go back to prev. day and process
            if (!isSingleDay)
            {
                _DateToProcess1 = _DateToProcess1.AddDays(-1);
            }
            string qryfromdate = "select CONCAT((select  case when DAY(getdate()) > 31 then concat(year(getdate()), '-', month(getdate()), '-', '31') else convert(date, DATEADD(DAY, 08, DATEADD(MONTH, -14, DATEADD(DAY, 0 - DAY(getdate()), getdate()))))  end),' 00:00:00.000') as fromdate;";
            //string qryfromdate = "2024-11-08 00:00:00";
            startTime = DateTime.Now;
            DataTable dtfromdate = sh.Get_Table_FromQry(qryfromdate);
            endTime = DateTime.Now;
            Console.WriteLine($"53.1-94: {endTime - startTime}");
            string fromdate = dtfromdate.Rows[0]["fromdate"].ToString();
            DateTime formattedFDate = DateTime.ParseExact(fromdate, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            string formattedfrmDate = formattedFDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime formattedFromDate = DateTime.ParseExact(formattedfrmDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            qry.Append(" SELECT e.Id,EmpId,ds.Id as Designation, case when b.id = 43 then dep.Id else b.Id end as BrDept,e.Shift_Id as Shift_Id from Employees e" +
                              " join Departments dep on dep.Id = e.Department" +
                              " join branches b on e.Branch = b.id" +
                              " join Designations ds on ds.Id = e.CurrentDesignation " +
                              " join shift_master sm on " +
                              " sm.Id = e.Shift_Id " +
                              " where GETDATE() <= e.RetirementDate " +
                              " AND (CONVERT(Datetime, sm.OutTime, 120) " + (_isSingleDay ? ">" : "<") + " " +
                              "CONVERT(Datetime, sm.InTime, 120)) and e.Empid in (844)");

            if (EmpId != null)
                qry.Append(" and EmpId in (" + EmpId + ") ");
            if (BranchId != null)
                qry.Append("  and b.id = " + BranchId + "");
            qry.Append(";");

            if (_isSingleDay)
            {
                qry.Append("SELECT distinct t.user_id as EmpId,case when b.id=43 then d.Id else b.Id end as BrDept,ds.Id as Designation,e.Shift_Id as Shift_Id,Min(t.io_time) as Starttime, " +
                       " Max(t.io_time) as EndTime,cast(io_time as date)as date  from timesheet_logs t join employees e on e.empid = t.user_id join branches b on e.Branch = b.id join " +
                       " Departments d on e.Department = d.id join Designations ds on ds.id = e.CurrentDesignation " +
                       " where (t.io_time between '" + formattedFromDate.ToString("yyyy-MM-dd") + " 00:00:00.000' " +
                       "and '" + formattedDate.ToString("yyyy-MM-dd") + " 23:59:59.000') and e.empid in (844)");

                qry.Append(" group by t.user_id,d.Id,b.Id, ds.Id,b.id,e.Shift_Id,cast(io_time as date); ");

            }
            else
            {
                qry.Append("Select 0; ");
            }
            qry.Append("SELECT e.Id,e.EmpId, e.Shift_Id, s.InTime, b.GracePeriod, s.OutTime,s.ShiftType, s.GroupName " +
                       "from Employees e join Shift_Master s on " +
                       "s.Id = e.Shift_Id join Branches b on " +
                       "b.Id = e.Branch " +
                       "where GETDATE() <= e.RetirementDate ;");
            if (BranchId != null)
            {
                //qryShift += " AND s.BranchId=" + BranchId + " ;";
                qry.Append(" AND s.BranchId=" + BranchId + " ;");
            }
            //leaves, ltc, od
            qry.Append("select lt.Code as 'Source', l.EmpId as 'PkId',l.StartDate,l.EndDate,l.status " +
             "FROM Leaves l join LeaveTypes lt on l.LeaveType = lt.Id " +
             "where('" + formattedFromDate.ToString("yyyy-MM-dd") + "' >= (convert(date, l.StartDate)) and '" + formattedDate.ToString("yyyy-MM-dd") + "' <= (convert(date, l.EndDate))) " +
             "and l.status in ( 'Approved','Pending','Forwarded' )  and l.empid in (121) " + //added on 16/05/2020
             "union " +
             "select 'LTC- '+lt.Code as 'Source', LLtc.EmpId as 'PkId',LLtc.StartDate,LLtc.EndDate,LLtc.status " +
             "FROM Leaves_LTC LLtc join LeaveTypes lt on LLtc.LeaveType=lt.Id " +
             "where('" + formattedFromDate.ToString("yyyy-MM-dd") + "' >= (convert(date, LLtc.StartDate)) and '" + formattedDate.ToString("yyyy-MM-dd") + "' <= (convert(date, LLtc.EndDate))) " +
             "and LLtc.status in ('Approved','Pending','Forwarded') and lltc.Empid in (121)  ");
            qry.Append("Select om.ODType as 'Source', Ood.EmpId as 'PkId',Ood.StartDate,Ood.EndDate,Ood.status " +
            "FROM OD_OtherDuty Ood join OD_Master Om on Ood.Purpose = Om.Id " +
            "where('" + formattedFromDate.ToString("yyyy-MM-dd") + "' >= (convert(date, Ood.StartDate)) and '" + formattedDate.ToString("yyyy-MM-dd") + "' <= (convert(date, Ood.EndDate))) and Ood.status in('Approved','Pending','Forwarded') and Ood.Empid in (121)  ;");
            qry.Append(
             "select Occasion, DATEPART(DAY, Date) as Day " +
               "from HolidayList " +
                "where Date between '" + formattedFromDate.ToString("yyyy-MM-dd") + " 00:00:00.000' " +
                    "and '" + formattedDate.ToString("yyyy-MM-dd") + " 23:59:59.000'"
              );


            for (DateTime datefrom = Convert.ToDateTime(fromdate); datefrom <= formattedDateTime; datefrom = datefrom.AddDays(1))
            {
                batchUpdateQuery.Clear();
                _DateToProcess1 = datefrom;
                _Holiday = false;
                string Day = int.Parse(_DateToProcess1.ToString("dd")).ToString();
                string Month = int.Parse(_DateToProcess1.ToString("MM")).ToString();
                string Year = _DateToProcess1.ToString("yyyy");
                //leaves, ltc, od
                startTime = DateTime.Now; // Record end time
                //qry.Append("select Occasion, DATEPART(DAY, Date) as Day from HolidayList where DATEPART(DAY, Date) = " + Day + " and DATEPART(MONTH, Date) = " + Month + " and DATEPART(YEAR, Date)= " + Year + "");
                DataSet ds = null;
                try
                {
                    startTime = DateTime.Now;
                    string query = qry.ToString();
                    ds = sh.Get_MultiTables_FromQry(query);
                    endTime = DateTime.Now;
                    Console.WriteLine($"53.2-186: {endTime - startTime}");
                }

                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }

                DataTable dtUsers = ds.Tables[0];
                DataTable dtTs = ds.Tables[1];
                DataTable dtShifts = ds.Tables[2];
                DataTable dtLeavesLTC = ds.Tables[3];
                DataTable dtODs = ds.Tables[4];
                DataTable dtcheckholiday = ds.Tables[5];

                if (dtcheckholiday.Rows.Count > 0)
                {
                    _Holiday = true;
                }
                StringBuilder finalBatchQuery = new StringBuilder();
                //finalBatchQuery= string.Empty;
                finalBatchQuery.Clear();
                foreach (DataRow drUser in dtUsers.Rows)
                {
                    try
                    {
                        StringBuilder updtQuery = new StringBuilder();
                        int userid = int.Parse(drUser["EmpId"].ToString());
                        bool isUserAbsent = true;

                        //* Get Timesheet data for that user *
                        DataRow[] drTs = null;
                        if (_isSingleDay)
                        {
                            drTs = dtTs.Select("EmpId='" + userid.ToString() + "'");
                        }

                        else
                        {
                            DataRow[] drUserShift = dtShifts.Select("Id=" + drUser["Id"].ToString());
                            //if user have Shift id
                            if (drUserShift.Length > 0)
                            {
                                startTime = DateTime.Now;
                                drTs = getTsLogsForShiftEmployee(sh, drUser["EmpId"].ToString(), drUserShift);
                                endTime = DateTime.Now;
                                Console.WriteLine($"53.3-232: {endTime - startTime}");
                            }
                            else
                            {
                                //if employee does not have shift id, branch id, designation, or department
                                // todo - log the error - user does not have shift id.
                            }
                        }

                        // ** If user availble in timesheet **
                        if (drTs.Length >= 1)
                        {
                            DataRow[] drUserShift = dtShifts.Select("Id=" + drUser["Id"].ToString());

                            //if user have Shift id
                            if (drUserShift.Length > 0)
                            {
                                startTime = DateTime.Now;
                                string updateQuery = UpdateTsUserStatusNew(sh, drTs[0], drUserShift, datetoprocess);
                                batchUpdateQuery.Append(updateQuery);
                                endTime = DateTime.Now;
                                Console.WriteLine($"53.4-252: {endTime - startTime}");
                                isUserAbsent = false;
                            }
                            //if employee does not have shift id, branch id, designation or department, he will consider as Absent
                            else
                            {
                                //if employee does not have shift id, branch id, designation, or department
                                // todo - log the error - user does not have shift id.
                            }
                        }
                        //*** if user not in timesheet then check leave, ltc table ***

                        else
                        {
                            //Getting EmpId from Leaves table
                            if (!_Holiday)
                            {
                                DataRow[] drLeaveUsers = dtLeavesLTC.Select("PkId=" + drUser["Id"].ToString());
                                if (drLeaveUsers.Length >= 1)
                                {
                                    startTime = DateTime.Now;
                                    string userUniqueId = getUserUniqueId(sh, drUser);
                                    endTime = DateTime.Now;
                                    Console.WriteLine($"53.5-275: {endTime - startTime}");
                                    DateTime Lstartdate = Convert.ToDateTime(drLeaveUsers[0]["StartDate"]);
                                    DateTime Lenddate = Convert.ToDateTime(drLeaveUsers[0]["EndDate"]);
                                    DateTime Today = DateTime.Today;
                                    DateTime Lendatenew;
                                    int datecompare = DateTime.Compare(Lenddate, Today);
                                    //if(t1<t2) = -1
                                    //if(t1=t2) = 0
                                    //if(t1>t2) = 1
                                    if (datecompare >= 0)
                                    {
                                        Lendatenew = Today;
                                    }
                                    else
                                    {
                                        Lendatenew = Lenddate;
                                    }
                                    startTime = DateTime.Now;
                                    var lCode = ManualApprovalCoad(drLeaveUsers[0]["Source"].ToString());
                                    endTime = DateTime.Now;
                                    Console.WriteLine($"53.6-294: {endTime - startTime}");
                                    int interval = 1;
                                    string strupdtQuery = "";
                                    string strupdtQueryNew = "";
                                    string daycode = "00:00#00:00#00:00#" + lCode;
                                    // updating the timesheet_Emp_Month with leavecode PL, ML and etc from LeaveStartdate to today's date.
                                    for (DateTime dateTime = Lstartdate; dateTime <= Lendatenew; dateTime += TimeSpan.FromDays(interval))
                                    {
                                        int Sday = dateTime.Day;
                                        int Smonth = dateTime.Month;
                                        int Syear = dateTime.Year;
                                        string StrWhere = " where UserId = '" + userid + "'  and Month = '" + Smonth + "' and Year = '" + Syear + "' ;";
                                        strupdtQuery = "update timesheet_Emp_Month set D" + Sday + " = '" + lCode + "', D" + Sday + "_GUID ='" + userUniqueId + "', D" + Sday + "_Status = '" + lCode + "' ";
                                        strupdtQueryNew += strupdtQuery + StrWhere;
                                        isUserAbsent = false;
                                    }
                                    batchUpdateQuery.Append(strupdtQueryNew);
                                }
                            }
                        }

                        //**** update OD though he is in timesheet ****

                        DataRow[] drODs = dtODs.Select("PkId=" + drUser["Id"].ToString());

                        if (drODs.Length >= 1)
                        {
                            string userUniqueId = getUserUniqueId(sh, drUser);
                            var lCode = ManualApprovalCoad(drODs[0]["Source"].ToString());

                            if (_Day != Day && _Month != Month && _Year != Year)
                            {
                                batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + Day + " = '" + lCode + "', D" + Day + "_GUID ='" + userUniqueId + "', D" + Day + "_Status = '" + lCode + "' " + " where UserId = '" + userid + "'  and Month = '" + Month + "' and Year = '" + Year + "' ; ");
                                isUserAbsent = false;
                            }
                            else if (_Day != Day && _Month == Month && _Year == Year)
                            {

                                batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + Day + " = '" + lCode + "', D" + Day + "_GUID ='" + userUniqueId + "', D" + Day + "_Status = '" + lCode + "' " + " where UserId = '" + userid + "'  and Month = '" + Month + "' and Year = '" + Year + "' ; ");
                                isUserAbsent = false;
                            }
                            else if (_Day != Day && _Month != Month && _Year == Year)
                            {

                                batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + Day + " = '" + lCode + "', D" + Day + "_GUID ='" + userUniqueId + "', D" + Day + "_Status = '" + lCode + "' " + " where UserId = '" + userid + "'  and Month = '" + Month + "' and Year = '" + Year + "' ; ");
                                isUserAbsent = false;
                            }
                            else

                                batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + _Day + " = '" + lCode + "', D" + _Day + "_GUID ='" + userUniqueId + "', D" + _Day + "_Status = '" + lCode + "' " + " where UserId = '" + userid + "'  and Month = '" + _Month + "' and Year = '" + _Year + "'; ");
                            isUserAbsent = false;

                        }

                        if (!_Holiday)
                        {
                            if (isUserAbsent)
                            {
                                startTime = DateTime.Now;
                                string userUniqueId = getUserUniqueId(sh, drUser);
                                endTime = DateTime.Now;
                                Console.WriteLine($"53.7-370: {endTime - startTime}");
                                if (_Day != Day && _Month != Month && _Year != Year)
                                {
                                    batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + Day + " = 'AB', D" + Day + "_GUID = '" + userUniqueId + "', D" + Day + "_Status = 'AB' , D" + Day + "_meridian = NULL" + " where UserId = '" + userid + "'  and Month = '" + Month + "' and Year = '" + Year + "' ");
                                }
                                else if (_Day != Day && _Month == Month && _Year == Year)
                                {
                                    batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + Day + " = 'AB', D" + Day + "_GUID = '" + userUniqueId + "', D" + Day + "_Status = 'AB' , D" + Day + "_meridian = NULL" + " where UserId = '" + userid + "'  and Month = '" + Month + "' and Year = '" + Year + "' ");

                                }
                                else if (_Day != Day && _Month != Month && _Year == Year)
                                {
                                    batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + Day + " = 'AB', D" + Day + "_GUID = '" + userUniqueId + "', D" + Day + "_Status = 'AB' , D" + Day + "_meridian = NULL" + " where UserId = '" + userid + "'  and Month = '" + Month + "' and Year = '" + Year + "' ");

                                }
                                else
                                    batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + _Day + " = 'AB', D" + _Day + "_GUID = '" + userUniqueId + "', D" + _Day + "_Status = 'AB' , D" + _Day + "_meridian = NULL" + " where UserId = '" + userid + "'  and Month = '" + _Month + "' and Year = '" + _Year + "' ");

                            }
                        }

                        else
                        {
                            if (isUserAbsent)
                            {
                                startTime = DateTime.Now;
                                string userUniqueId = getUserUniqueId(sh, drUser);
                                endTime = DateTime.Now;
                                Console.WriteLine($"53.8-398: {endTime - startTime}");
                                if (_Day != Day && _Month != Month && _Year != Year)
                                {
                                    batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + Day + " = 'HL', D" + Day + "_GUID = '" + userUniqueId + "', D" + Day + "_Status = 'HL' , D" + Day + "_meridian = NULL" + " where UserId = '" + userid + "'  and Month = '" + Month + "' and Year = '" + Year + "' ");
                                }
                                else if (_Day != Day && _Month == Month && _Year == Year)
                                {
                                    batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + Day + " = 'HL', D" + Day + "_GUID = '" + userUniqueId + "', D" + Day + "_Status = 'HL' , D" + Day + "_meridian = NULL" + " where UserId = '" + userid + "'  and Month = '" + Month + "' and Year = '" + Year + "' ");

                                }
                                else if (_Day != Day && _Month != Month && _Year == Year)
                                {
                                    batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + Day + " = 'HL', D" + Day + "_GUID = '" + userUniqueId + "', D" + Day + "_Status = 'HL' , D" + Day + "_meridian = NULL" + " where UserId = '" + userid + "'  and Month = '" + Month + "' and Year = '" + Year + "' ");

                                }
                                else
                                    batchUpdateQuery.Append($"update timesheet_Emp_Month set D" + _Day + " = 'HL', D" + _Day + "_GUID = '" + userUniqueId + "', D" + _Day + "_Status = 'HL' , D" + _Day + "_meridian = NULL" + " where UserId = '" + userid + "'  and Month = '" + _Month + "' and Year = '" + _Year + "' ");

                            }
                        }
                        if (batchUpdateQuery.ToString() != "")
                        {
                            startTime = DateTime.Now;
                            finalBatchQuery = batchUpdateQuery;
                            endTime = DateTime.Now;
                            Console.WriteLine($"53.9-423: {endTime - startTime}");
                        }

                    }

                    catch (Exception e)
                    {
                        _logger.Error(e.Message + " Employee Id" + EmpId + " Device Id" + DeviceId);
                    }

                }

                if (finalBatchQuery.Length > 0)
                {
                    startTime = DateTime.Now;
                    sh.Run_UPDDEL_ExecuteNonQuery(finalBatchQuery.ToString());
                    endTime = DateTime.Now;
                    Console.WriteLine($"53.10-451: {endTime - startTime}");
                }

            }

        }

        //This method is to check the employees who applied leaves and entry in the timesheet for a month.
        public void LeaveAppliedButAttendedDuty()
        {
            DateTime startTime;
            DateTime endTime;
            //DateTime CurDate = DateTime.Today;
            //DateTime firstdaydate = CurDate.AddDays(-15);
            //DateTime lastdaydate = firstdaydate.AddMonths(1).AddDays(-1);

            DateTime CurDate = DateTime.ParseExact("03/12/2024", "d/M/yyyy", CultureInfo.InvariantCulture);
            string dateString = CurDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            DateTime formattedDateTime = DateTime.ParseExact(dateString, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            DateTime firstdaydate = formattedDateTime.AddDays(5);
            DateTime lastdaydate = formattedDateTime;
            string source;
            int pkid, empcode;
            DateTime startdate;
            DateTime enddate;
            DateTime startdate1;
            DateTime enddate1;
            string UpdateQry = "";
            string UpdateQrynew = "";
            string shiftquery = "";
            string qryTs = "";
            string qryTsnew = "";

            SqlHelper sh = new SqlHelper(_mavenConnStr);

            string qryleave = "select lt.Code as 'Source', l.EmpId as 'PkId',l.StartDate,l.EndDate,l.status FROM Leaves l join LeaveTypes lt on l.LeaveType = lt.Id " +
                " where(('" + firstdaydate.ToString("yyyy-MM-dd") + "' <= (convert(date, l.StartDate)) or ('" + firstdaydate.ToString("yyyy-MM-dd") + "' = (convert(date, l.EndDate)))) " +
                " and ('" + lastdaydate.ToString("yyyy-MM-dd") + "' >= (convert(date, l.EndDate))) or " +
                " ('" + lastdaydate.ToString("yyyy-MM-dd") + "' = (convert(date, l.StartDate)))) and l.status in ( 'Approved','Pending','Forwarded' ) " +
                " union " +
                " select 'LTC- ' + lt.Code as 'Source', LLtc.EmpId as 'PkId',LLtc.StartDate,LLtc.EndDate,LLtc.status FROM Leaves_LTC LLtc join LeaveTypes lt " +
                " on LLtc.LeaveType = lt.Id where(('" + firstdaydate.ToString("yyyy-MM-dd") + "' <= (convert(date, LLtc.StartDate)) or('" + firstdaydate.ToString("yyyy-MM-dd") + "' = (convert(date, LLtc.EndDate)))) " +
                " and('" + lastdaydate.ToString("yyyy-MM-dd") + "' >= (convert(date, LLtc.EndDate))) or('" + lastdaydate.ToString("yyyy-MM-dd") + "' = (convert(date, LLtc.StartDate)))) and " +
                " LLtc.status in ('Approved', 'Pending', 'Forwarded'); ";
            startTime = DateTime.Now;
            DataTable dtleaves = sh.Get_Table_FromQry(qryleave);
            endTime = DateTime.Now;
            Console.WriteLine($"58.1-456: {endTime - startTime}");
            if (dtleaves.Rows.Count > 0)
            {
                foreach (DataRow drleaves in dtleaves.Rows)
                {
                    source = drleaves["Source"].ToString();
                    pkid = Convert.ToInt32(drleaves["PkId"]);
                    startdate = Convert.ToDateTime(drleaves["StartDate"]);
                    enddate = Convert.ToDateTime(drleaves["EndDate"]);
                    string getempcode = "SELECT Empid from Employees where id=" + pkid + ";";
                    startTime = DateTime.Now;
                    DataTable dtempid = sh.Get_Table_FromQry(getempcode);
                    endTime = DateTime.Now;
                    Console.WriteLine($"58.2-470: {endTime - startTime}");
                    if (dtempid.Rows.Count > 0)
                    {
                        empcode = Convert.ToInt32(dtempid.Rows[0]["Empid"]);
                        qryTs = "SELECT distinct t.user_id as EmpId,case when b.id=43 then d.Id else b.Id end as BrDept,ds.Id as Designation,e.Shift_Id as Shift_Id,Min(t.io_time) as Starttime, " +
                       " Max(t.io_time) as EndTime  from timesheet_logs t join employees e on e.empid = t.user_id join branches b on e.Branch = b.id join " +
                       " Departments d on e.Department = d.id join Designations ds on ds.id = e.CurrentDesignation " +
                       " where (t.io_time between '" + startdate.ToString("yyyy-MM-dd") + " 00:00:00.000' and '" + enddate.ToString("yyyy-MM-dd") + " 23:59:59.000') and e.Empid=" + empcode + " " +
                       " group by t.user_id,d.Id,b.Id, ds.Id,b.id,e.Shift_Id; ";

                    }
                    startTime = DateTime.Now;
                    DataTable dttimesheet = sh.Get_Table_FromQry(qryTs);
                    endTime = DateTime.Now;
                    Console.WriteLine($"58.3-484: {endTime - startTime}");
                    if (dttimesheet.Rows.Count > 0)
                    {
                        foreach (DataRow drtimesheet in dttimesheet.Rows)
                        {
                            startdate1 = Convert.ToDateTime(drtimesheet["Starttime"]);
                            enddate1 = Convert.ToDateTime(drtimesheet["EndTime"]);
                            for (DateTime sdate = startdate1; sdate <= enddate1; sdate += TimeSpan.FromDays(1))
                            {
                                empcode = Convert.ToInt32(dtempid.Rows[0]["Empid"]);
                                qryTsnew = "SELECT distinct t.user_id as EmpId,case when b.id=43 then d.Id else b.Id end as BrDept,ds.Id as Designation,e.Shift_Id as Shift_Id,Min(t.io_time) as Starttime, " +
                                     " Max(t.io_time) as EndTime  from timesheet_logs t join employees e on e.empid = t.user_id join branches b on e.Branch = b.id join " +
                                     " Departments d on e.Department = d.id join Designations ds on ds.id = e.CurrentDesignation " +
                                    " where (t.io_time between '" + sdate.ToString("yyyy-MM-dd") + " 00:00:00.000' and '" + sdate.ToString("yyyy-MM-dd") + " 23:59:59.000') and e.Empid=" + empcode + " " +
                                    " group by t.user_id,d.Id,b.Id, ds.Id,b.id,e.Shift_Id; ";
                                empcode = Convert.ToInt32(drtimesheet["EmpId"]);
                                shiftquery = "SELECT e.Id,e.EmpId, e.Shift_Id, s.InTime, b.GracePeriod, s.OutTime,s.ShiftType, s.GroupName from Employees e join Shift_Master s on s.Id = e.Shift_Id join Branches b on b.Id = e.Branch where GETDATE() <= e.RetirementDate and e.EmpId = " + empcode + "; ";
                                startTime = DateTime.Now;
                                DataTable dtshift = sh.Get_Table_FromQry(shiftquery);
                                endTime = DateTime.Now;
                                Console.WriteLine($"58.4-504: {endTime - startTime}");
                                DataRow[] drshift = dtshift.Select("EmpId=" + empcode.ToString());
                                startTime = DateTime.Now;
                                UpdateQry = UpdateTsUserStatusNew(sh, drtimesheet, drshift, sdate);
                                endTime = DateTime.Now;
                                Console.WriteLine($"58.5-509: {endTime - startTime}");
                                UpdateQrynew += UpdateQry;
                            }
                        }
                        if (UpdateQrynew != "")
                        {
                            startTime = DateTime.Now;
                            sh.Run_UPDDEL_ExecuteNonQuery(UpdateQrynew);
                            endTime = DateTime.Now;
                            Console.WriteLine($"58.6-518: {endTime - startTime}");
                            UpdateQrynew = "";
                        }
                        else
                        {
                            //do nothing
                        }
                    }
                    else
                    {
                        //do nothing
                    }
                }

            }


        }

        //getting employees list from timesheet who wil have shifts
        private DataRow[] getTsLogsForShiftEmployee(SqlHelper sh, string userid, DataRow[] drUserShift)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;
            // // _logger.Info("2.5.5 getTsLogsForShiftEmployee start in timesheetbussiness "+ startTime);
            DateTime dt1 = DateTime.Parse(_DateToProcess1.ToString("yyyy-MM-dd") + " " + drUserShift[0]["InTime"].ToString());
            DateTime dt2 = DateTime.Parse(_DateToProcess2.ToString("yyyy-MM-dd") + " " + drUserShift[0]["OutTime"].ToString());
            //remove some hrs
            dt1 = dt1.AddMinutes(-_ShiftUserInOutTimeToleranceMins);
            //add some hrs
            dt2 = dt2.AddMinutes(_ShiftUserInOutTimeToleranceMins);

            string day1InTime = dt1.ToString("yyyy-MM-dd HH:mm:ss");
            string day2OutTime = dt2.ToString("yyyy-MM-dd HH:mm:ss");

            string qry = "SELECT distinct t.user_id as EmpId,case when b.id=43 then d.Id else b.Id end as BrDept,ds.Id as Designation,e.Shift_Id as Shift_Id,Min(t.io_time) as Starttime, " +
                       " Max(t.io_time) as EndTime  from timesheet_logs t join employees e on e.empid = t.user_id join branches b on e.Branch = b.id join " +
                       " Departments d on e.Department = d.id join Designations ds on ds.id = e.CurrentDesignation " +
                       " where (t.io_time between '" + day1InTime + "' AND '" + day2OutTime + "') " +
                       " AND user_id=" + userid +
                       " group by t.user_id,d.Id,b.Id, ds.Id,b.id,e.Shift_Id; ";
            DataTable dttm = null;
            try
            {
                dttm = sh.Get_Table_FromQry(qry);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            return dttm.Select();
            endTime = DateTime.Now; // Record end time
            duration = endTime - startTime;
            // // _logger.Info("2.5.6 getTsLogsForShiftEmployee end in timesheetbussiness " + endTime+ " duration "+duration );
        }

        //codes for Reason_Type column in timesheet_request_form
        private string ManualApprovalCoad(string rtype)
        {
            string ret = rtype;
            //add power issue
            if (rtype == "Internet Issue")
            {
                ret = "IntIss";
            }
            else if (rtype == "Power Problem")
            {
                ret = "PowPro";
            }
            else if (rtype == "Machine Problem")
            {
                ret = "McPro";
            }
            else if (rtype == "Other")
            {
                ret = "Other";
            }
            else if (rtype == "Training")
            {
                ret = "OD-Trng";
            }
            else if (rtype == "Inspection")
            {
                ret = "OD-Insp";
            }
            else if (rtype == "Meeting")
            {
                ret = "OD-Mtng";
            }
            else if (rtype == "Official Work")
            {
                ret = "OD-OfWk";
            }
            else if (rtype == "Permission")
            {
                ret = "OD-Perm";
            }
            else if (rtype == "Visit Branch")
            {
                ret = "OD-VstBr";
            }
            else if (rtype == "Visit DCCB")
            {
                ret = "OD-VstDccb";
            }
            else if (rtype == "WORK FROM HOME")
            {
                ret = "OD-WKH";
            }

            return ret;
        }

        //Update Timesheet_Emp_month Table
        private string UpdateTsUserStatusNew(SqlHelper sh, DataRow drUser, DataRow[] drUserShift, DateTime? datetoprocess)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;

            // // _logger.Info("2.5.7 UpdateTsUserStatusNew start in timesheetbussiness "+ startTime);
            var userInTm = DateTime.Parse(drUser["Starttime"].ToString());
            var userOutTm = DateTime.Parse(drUser["EndTime"].ToString());

            if (datetoprocess != null)
            {
                _DateToProcess1 = DateTime.Parse(datetoprocess.ToString());
            }

            string Day = int.Parse(_DateToProcess1.ToString("dd")).ToString();
            string Month = int.Parse(_DateToProcess1.ToString("MM")).ToString();
            string Year = _DateToProcess1.ToString("yyyy");


            //add user date to shifttime
            string sftStDtTm = userInTm.ToString("yyyy-MM-dd") + " " + drUserShift[0]["InTime"].ToString();
            string sftEndDtTm = userOutTm.ToString("yyyy-MM-dd") + " " + drUserShift[0]["OutTime"].ToString();

            //06:00#21:00#15:00#EA#04:09#LD#03:00~EA,LD
            string userDayStatus = "";
            //if user in time and out time are equal
            if (userInTm == userOutTm)
            {
                userDayStatus = getUserDayStatusForInOutSame(
                    userInTm, userOutTm,
                    DateTime.Parse(sftStDtTm), DateTime.Parse(sftEndDtTm),
                    int.Parse(drUserShift[0]["GracePeriod"].ToString())
                    );
            }
            else
            {

                TimeSpan userduration = userOutTm.Subtract(userInTm);

                TimeSpan ShiftDuration = DateTime.Parse(sftEndDtTm).Subtract(DateTime.Parse(sftStDtTm));
                TimeSpan HalfShiftDuration = new TimeSpan(ShiftDuration.Ticks / 2);
                _StartTimeForHalfDay = HalfShiftDuration.Subtract(TimeForHalfDay1Hour);
                _EndTimeForHalfDay = HalfShiftDuration.Add(TimeForHalfDay1Hour);
                //if duration is less than 3hr then day as "PR,NA" or "NA,PR"
                if (userduration < _StartTimeForHalfDay)
                {
                    userDayStatus = getUserDayStatusForInOutSame(
                    userInTm, userOutTm,
                    DateTime.Parse(sftStDtTm), DateTime.Parse(sftEndDtTm),
                    int.Parse(drUserShift[0]["GracePeriod"].ToString())
                    );
                }
                //*** half check ***
                else if (userduration >= _StartTimeForHalfDay && userduration <= _EndTimeForHalfDay)
                {
                    userDayStatus = userInTm.ToString("hh:mm")
                               + "#" + userOutTm.ToString("hh:mm")
                               + "#" + userduration.ToString("hh':'mm")
                               + "#" + EmpDayStatus.HD.ToString()
                               + "~" + EmpDayStatus.HD.ToString();
                }
                else
                {
                    //full day calc
                    userDayStatus = getUserFullDayStatusAndTimings(
                       userInTm, userOutTm,
                       DateTime.Parse(sftStDtTm), DateTime.Parse(sftEndDtTm),
                       int.Parse(drUserShift[0]["GracePeriod"].ToString())
                       );
                }
            }


            string InTm_meridian = DateTime.Parse(drUser["Starttime"].ToString()).ToString("tt");
            string OutTm_meridian = DateTime.Parse(drUser["EndTime"].ToString()).ToString("tt");

            string UserDayStatus = userDayStatus.Substring(0, userDayStatus.IndexOf("~"));
            string UserStatus = userDayStatus.Substring(userDayStatus.LastIndexOf('~') + 1);

            //get the unique id
            string userUniqueId = getUserUniqueId(sh, drUser); //todo - bring unique id in all employees query in the begining and use
            endTime = DateTime.Now; // Record end time
            duration = endTime - startTime;

            // // _logger.Info("2.5.8 UpdateTsUserStatusNew end in timesheetbussiness " + endTime + " Duration " + duration);
            //update to db table
            //if (_Day != Day && _Month != Month && _Year != Year)
            //{
            //    return "update timesheet_Emp_Month set D" + _Day + " = '" + UserDayStatus + "',D" + _Day + "_GUID = '" + userUniqueId + "' , D" + _Day + "_Status = '" + UserStatus + "' , D" + _Day + "_meridian = '" + InTm_meridian + "/" + OutTm_meridian + " "
            //   + " where UserId = " + drUser["EmpId"] + " and Year = " + _Year + " and Month = " + _Month + " ' ; ";
            //}
            //else
            //    return "update timesheet_Emp_Month set D" + Day + " = '" + UserDayStatus + "',D" + Day + "_GUID = '" + userUniqueId + "' , D" + Day + "_Status = '" + UserStatus + "' , D" + Day + "_meridian = '" + InTm_meridian + "/" + OutTm_meridian + "'"
            //  + " where UserId = " + drUser["EmpId"] + " and Year = " + Year + " and Month = " + Month + "  ; ";
            if (_Day != Day && _Month != Month && _Year != Year)
            {
                return "update timesheet_Emp_Month set D" + Day + " = '" + UserDayStatus + "',D" + Day + "_GUID = '" + userUniqueId + "' , D" + Day + "_Status = '" + UserStatus + "', D" + Day + "_meridian = '" + InTm_meridian + "/" + OutTm_meridian + "' "
               + " where UserId = " + drUser["EmpId"] + " and Year = " + Year + " and Month = " + Month + " ; ";
            }
            else if (_Day != Day && _Month == Month && _Year == Year)
            {
                return "update timesheet_Emp_Month set D" + Day + " = '" + UserDayStatus + "',D" + Day + "_GUID = '" + userUniqueId + "' , D" + Day + "_Status = '" + UserStatus + "', D" + Day + "_meridian = '" + InTm_meridian + "/" + OutTm_meridian + "' "
               + " where UserId = " + drUser["EmpId"] + " and Year = " + Year + " and Month = " + Month + " ; ";
            }
            else if (_Day != Day && _Month != Month && _Year == Year)
            {
                return "update timesheet_Emp_Month set D" + Day + " = '" + UserDayStatus + "',D" + Day + "_GUID = '" + userUniqueId + "' , D" + Day + "_Status = '" + UserStatus + "', D" + Day + "_meridian = '" + InTm_meridian + "/" + OutTm_meridian + "' "
               + " where UserId = " + drUser["EmpId"] + " and Year = " + Year + " and Month = " + Month + " ; ";
            }
            else
            {
                return "update timesheet_Emp_Month set D" + _Day + " = '" + UserDayStatus + "',D" + _Day + "_GUID = '" + userUniqueId + "' , D" + _Day + "_Status = '" + UserStatus + "', D" + _Day + "_meridian = '" + InTm_meridian + "/" + OutTm_meridian + "' "
              + " where UserId = " + drUser["EmpId"] + " and Year = " + _Year + " and Month = " + _Month + " ; ";

            }

        }

        //if user have only entry in timesheet
        private string getUserDayStatusForInOutSame(DateTime userInTm, DateTime userOutTm, DateTime sftStTm, DateTime sftEdTm, int brGracePeriod)
        {
            string ret = "", InTimestatus = "", InTimeDuration = "", OutTimestatus = "", OutTimeDuration = "";

            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;

            // // _logger.Info("getUserDayStatusForInOutSame start in timesheetbussiness " + startTime);
            //find out single time is near to shift start or shift end time
            TimeSpan chkMorningtime = sftStTm.Subtract(userInTm);
            TimeSpan chkEveningtime = sftEdTm.Subtract(userOutTm);

            //change -ve time
            if (chkMorningtime < TimeSpan.Zero)
                chkMorningtime = chkMorningtime.Negate();
            if (chkEveningtime < TimeSpan.Zero)
                chkEveningtime = chkEveningtime.Negate();

            if (chkMorningtime < chkEveningtime)
            {
                //go with shift start time
                //Morning check in time - EA, LA, PR
                string[] arrMorningtimestatus = getUserMorningStatus(userInTm, sftStTm, brGracePeriod).Split('#');
                InTimestatus = arrMorningtimestatus[0];
                InTimeDuration = arrMorningtimestatus[1];

                //09:35#09:35#00:00#LA#00:25#NA#00:00~LA,NA
                ret = userInTm.ToString("hh:mm")
                              + "#" + userOutTm.ToString("hh:mm")
                              + "#" + (userInTm == userOutTm ? "00:00" : userOutTm.Subtract(userInTm).ToString("hh':'mm"))
                              + "#" + InTimestatus
                              + "#" + InTimeDuration
                              + "#NA"
                              + "#00:00"
                              + "~" + InTimestatus + ",NA";
            }
            else
            {
                //go with shift evening time
                //Evening check out time - ED, LD, PR
                string[] arrEveningtimestatus = getUserEveningStatus(userOutTm, sftEdTm).Split('#');
                OutTimestatus = arrEveningtimestatus[0];
                OutTimeDuration = arrEveningtimestatus[1];

                //04:15#04:15#00:00#NA#00:00#ED#00:45~NA,ED
                ret = userInTm.ToString("hh:mm")
                              + "#" + userOutTm.ToString("hh:mm")
                               + "#" + (userInTm == userOutTm ? "00:00" : userOutTm.Subtract(userInTm).ToString("hh':'mm"))
                              + "#NA"
                              + "#00:00"
                               + "#" + OutTimestatus
                              + "#" + OutTimeDuration
                              + "~NA," + OutTimestatus;
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // // _logger.Info("getUserDayStatusForInOutSame end in timesheetbussiness " + endTime+ " duration " +duration);
            return ret;

        }

        //Full day present employee status
        private string getUserFullDayStatusAndTimings(DateTime userInTm, DateTime userOutTm, DateTime sftStTm, DateTime sftEdTm, int brGracePeriod)
        {
            string InTimestatus = "", InTimeDuration = "", OutTimestatus = "", OutTimeDuration = "";
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;
            // // _logger.Info("getUserFullDayStatusAndTimings start in timesheetbussiness " + startTime);
            //1. Morning check in time - EA, LA, PR
            string[] arrMorningtimestatus = getUserMorningStatus(userInTm, sftStTm, brGracePeriod).Split('#');
            InTimestatus = arrMorningtimestatus[0];
            InTimeDuration = arrMorningtimestatus[1];

            //2. Evening check out time - ED, LD, PR
            string[] arrEveningtimestatus = getUserEveningStatus(userOutTm, sftEdTm).Split('#');
            OutTimestatus = arrEveningtimestatus[0];
            OutTimeDuration = arrEveningtimestatus[1];

            TimeSpan userduration = userOutTm.Subtract(userInTm);
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // _logger.Info("getUserFullDayStatusAndTimings end in timesheetbussiness " + endTime + " duration " + duration);
            //06:00#21:00#15:00#EA#04:09#LD#03:00~EA,LD
            return userInTm.ToString("hh:mm")
                          + "#" + userOutTm.ToString("hh:mm")
                          + "#" + userduration.ToString("hh':'mm")
                          + "#" + InTimestatus
                          + "#" + InTimeDuration
                          + "#" + OutTimestatus
                          + "#" + OutTimeDuration
                          + "~" + InTimestatus + "," + OutTimestatus;
        }

        //Morning time status
        private string getUserMorningStatus(DateTime userInTm, DateTime sftStTm, int brGracePeriod)
        {
            string InTimestatus = "", InTimeDuration = "";
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;
            // _logger.Info("getUserMorningStatus start in timesheetbussiness " + startTime);
            //1. Morning check in time - EA, LA, PR
            DateTime sftStTmWithGracePeriod = sftStTm.AddMinutes(brGracePeriod);
            TimeSpan userLateArrivalBy = userInTm.Subtract(sftStTmWithGracePeriod);
            TimeSpan userEarlyArrivalBy = sftStTm.Subtract(userInTm);
            if (userLateArrivalBy != null && userLateArrivalBy > TimeSpan.Zero)
            {
                InTimestatus = EmpDayStatus.LA.ToString();
                InTimeDuration = "#" + userInTm.Subtract(sftStTm).ToString("hh':'mm");
            }
            else if (userEarlyArrivalBy != null && userEarlyArrivalBy > TimeSpan.Zero)
            {
                InTimestatus = EmpDayStatus.EA.ToString();
                InTimeDuration = "#" + userEarlyArrivalBy.ToString("hh':'mm");
            }
            else
            {
                InTimestatus = EmpDayStatus.PR.ToString();
                InTimeDuration = "#00:00";
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // _logger.Info("getUserMorningStatus end in timesheetbussiness " + endTime+ "duration "+duration);

            return InTimestatus + InTimeDuration;
        }

        //Evening time status
        private string getUserEveningStatus(DateTime userOutTm, DateTime sftEdTm)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;
            // _logger.Info("getUserEveningStatus start in timesheetbussiness " + startTime);
            string OutTimestatus = "", OutTimeDuration = "";
            TimeSpan userEarlyDepartureBy = sftEdTm.Subtract(userOutTm);
            //TimeSpan userLateDepartureBy = userOutTm.Subtract(sftEdTm);
            TimeSpan userLateDepartureBy = userOutTm.Subtract(sftEdTm.AddMinutes(_UserOutTimeGracePeriodMins));

            if (userEarlyDepartureBy != null && userEarlyDepartureBy > TimeSpan.Zero)
            {
                OutTimestatus = EmpDayStatus.ED.ToString();
                OutTimeDuration = "#" + userEarlyDepartureBy.ToString("hh':'mm");
            }
            else if (userLateDepartureBy != null && userLateDepartureBy > TimeSpan.Zero)
            {
                OutTimestatus = EmpDayStatus.PR.ToString();
                OutTimeDuration = "#" + userOutTm.Subtract(sftEdTm).ToString("hh':'mm");
            }
            else
            {
                OutTimestatus = EmpDayStatus.PR.ToString();
                OutTimeDuration = "#00:00";
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // _logger.Info("getUserEveningStatus end in timesheetbussiness " + endTime+ " duration "+duration);

            return OutTimestatus + OutTimeDuration;
        }

        //to get employee unique id
        private string getUserUniqueId(SqlHelper sh, DataRow drUser)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;
            // _logger.Info("getUserUniqueId start in timesheetbussiness " + startTime);
            var user_id = drUser["EmpId"];
            var BranchId = drUser["BrDept"];
            var DesignationId = drUser["Designation"];
            var Shift_Id = drUser["Shift_Id"];
            string Unique_id = "";
            string qrySelUniqueId = "";
            qrySelUniqueId = "select Unique_Id from timesheet_Emp_Month_UID where User_Id="
                + user_id + " AND BrDept=" + BranchId + " AND Designation =" + DesignationId + " AND Shift_EId = " + Shift_Id + " ";
            DataTable dtEmployee_Month_UID = null;
            try
            {
                dtEmployee_Month_UID = sh.Get_Table_FromQry(qrySelUniqueId);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            if (dtEmployee_Month_UID.Rows.Count > 0)
            {
                Unique_id = dtEmployee_Month_UID.Rows[0][0].ToString();
            }
            else
            {
                var guid = Guid.NewGuid();
                string qryGenUniqueId = "INSERT INTO timesheet_Emp_Month_UID([Unique_Id],[user_id],[BrDept],[Designation],[Shift_EId],[UpdatedDate])"
                               + "VALUES('" + guid + "','" + user_id + "','" + BranchId + "','" + DesignationId + "','" + Shift_Id + "',getdate())";
                try
                {
                    sh.Run_UPDDEL_ExecuteNonQuery(qryGenUniqueId);
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message + " User Id" + user_id + " Branch Id" + BranchId);
                }
                Unique_id = guid.ToString();


            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // _logger.Info("getUserUniqueId end in timesheetbussiness " + endTime + " duration " + duration);
            return Unique_id;
        }

        //updating employee Request form details in timesheetlog_Emp_Month table when data available in timesheetlogs_Request_Form and updating processed column as 1 in timesheet_request_form table
        public void UpdateRequestFormUsers()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;
            // _logger.Info("2.7.1 UpdateRequestFormUsers start in timesheetbussiness " + startTime);
            //get Request form details from timesheet_Request_From table for leave
            string qryRequestForm = "Select e.EmpId ,ReqFromDate, ReqToDate,Reason_Type, trf.Id from Timesheet_Request_Form Trf" +
              " join Employees e on Trf.UserId = e.Id" +
               " join Departments dep on dep.Id = e.Department" +
               " join branches b on e.Branch = b.id" +
               " join Designations ds on ds.Id = e.CurrentDesignation where trf.Processed=0 or trf.Processed=2;";

            SqlHelper sh = new SqlHelper(_mavenConnStr);
            DataTable dtRequestForm = null;
            try
            {
                dtRequestForm = sh.Get_Table_FromQry(qryRequestForm);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            if (dtRequestForm.Rows.Count > 0)
            {
                string qryForRequestform = "";

                //take each request
                foreach (DataRow drRequestform in dtRequestForm.Rows)
                {
                    var Pkid = drRequestform["Id"];
                    DateTime fmDt = DateTime.Parse(drRequestform["ReqFromDate"].ToString());
                    DateTime toDt = DateTime.Parse(drRequestform["ReqToDate"].ToString());

                    while (fmDt <= toDt)
                    {
                        int day = int.Parse(fmDt.ToString("dd"));
                        int month = int.Parse(fmDt.ToString("MM"));
                        int year = int.Parse(fmDt.ToString("yyyy"));


                        var lCode = ManualApprovalCoad(drRequestform["Reason_Type"].ToString());

                        qryForRequestform += "Update timesheet_Emp_Month set D" + day + " = '" + lCode + "',D" + day + "_Status = '" + lCode + "' where UserId = " + drRequestform["EmpId"] + " and Year = " + year + " and Month = " + month + "; ";

                        //next day
                        fmDt = fmDt.AddDays(1);
                    }
                    qryForRequestform += "Update Timesheet_Request_Form set Processed=1 where Id = " + Pkid + ";";
                }
                try
                {
                    sh.Run_UPDDEL_ExecuteNonQuery(qryForRequestform);
                }
                catch (Exception e)
                {
                    _logger.Error(e.ToString());
                }
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // _logger.Info("2.7.2 UpdateRequestFormUsers end in timesheetbussiness " + endTime+ " duration "+duration);

        }

        //Approved Past OD then Cancelled , then updated to user previous status
        public void UpdateRequestFormODUsers()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;
            // _logger.Info("2.9.1 UpdateRequestFormODUsers start in timesheetbussiness " + startTime);
            string updtQuery = "";
            //getting cancelled Approved OD Request form users
            string qryRequestForm = "Select e.EmpId ,ReqFromDate, ReqToDate,Reason_Type, trf.Id from Timesheet_Request_Form Trf" +
              " join Employees e on Trf.UserId = e.Id" +
               " join Departments dep on dep.Id = e.Department" +
               " join branches b on e.Branch = b.id" +
               " join Designations ds on ds.Id = e.CurrentDesignation where trf.Processed=3";
            //qryRequestForm += " and EmpId in (5889); ";

            SqlHelper sh = new SqlHelper(_mavenConnStr);
            DataTable dtRequestForm = null;
            try
            {
                dtRequestForm = sh.Get_Table_FromQry(qryRequestForm);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            startTime = DateTime.Now;
            // _logger.Info("2.9.2 UpdateRequestFormODUsers before forloop " + startTime);
            //checking OD request users > 0
            if (dtRequestForm.Rows.Count > 0)
            {
                foreach (DataRow drRequestform in dtRequestForm.Rows)
                {
                    var EmpId = drRequestform["EmpId"];
                    var PkId = drRequestform["Id"];
                    DateTime fmDt = DateTime.Parse(drRequestform["ReqFromDate"].ToString());
                    DateTime toDt = DateTime.Parse(drRequestform["ReqToDate"].ToString());

                    while (fmDt <= toDt)
                    {
                        try
                        {
                            // // _logger.Info("Update Request Form Cancelled OD users for single day shift Started");
                            GenerateTimesheetReportsData(true, fmDt, EmpId, null, null);
                            // // _logger.Info("Update Request Form Cancelled OD users for single day shift Ended");
                        }
                        catch (Exception e1)
                        {
                            //_logger.Warn(e1.Message);
                        }
                        try
                        {
                            //// _logger.Info("Update Request Form Cancelled OD users for Multiple day shift Started");
                            GenerateTimesheetReportsData(false, fmDt, EmpId, null, null);
                            //// _logger.Info("Update Request Form Cancelled OD users for Multiple day shiftEnded");
                        }
                        catch (Exception e1)
                        {
                            // _logger.Warn(e1.Message);
                        }


                        //next day
                        fmDt = fmDt.AddDays(1);
                    }
                    updtQuery += "Update Timesheet_Request_Form set Processed=1 where Id = " + PkId + ";";

                }
                sh.Run_UPDDEL_ExecuteNonQuery(updtQuery);
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // _logger.Info("2.9.3  UpdateRequestFormODUsers afterloop/end in timesheetbussiness " + endTime+ " duration "+duration);

        }

        //updating employee details in timesheetlog_Emp_Month table for every month on 1st day
        public void InsertEmployeesIntoEmpMonthTable()
        {
            //if day equals to 1
            //if (_Day == 01.ToString())
            //{
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;
            // _logger.Info("2.1.2 InsertEmployeesIntoEmpMonthTable start in timesheetbussiness " + startTime);
            string qryEmployeesNotInmonth = "select EmpId from Employees where EmpId NOT IN(select UserId from timesheet_Emp_Month where Year =" + _Year + " and Month = " + _Month + ") and RetirementDate>=GETDATE();";
            //qryEmployeesNotInmonth += " and EmpId in (744); ";
            SqlHelper sh = new SqlHelper(_mavenConnStr);

            DataTable dtEmployeesNotInMonthTable = sh.Get_Table_FromQry(qryEmployeesNotInmonth);
            // _logger.Info("2.1.3 InsertEmployeesIntoEmpMonthTable before loop " + startTime);
            //for employees of Employee table except Employees in Timesheet_Emp_Month
            if (dtEmployeesNotInMonthTable.Rows.Count > 0)
            {
                string qryTsEmpMonthUpdtList = "";
                int count = dtEmployeesNotInMonthTable.Rows.Count;
                int index = 1;
                foreach (DataRow drUser2 in dtEmployeesNotInMonthTable.Rows)
                {

                    string qryTsEmpMonthUpdt = string.Format("({0},'{1}', {2}", drUser2["EmpId"], _Year, _Month + "),");
                    if (index != (count + 1))
                    {
                        qryTsEmpMonthUpdtList += qryTsEmpMonthUpdt;
                    }
                    index++;
                }
                string qryupdtQuery = "INSERT INTO timesheet_Emp_Month([UserId],[Year],[Month])" + "Values" + qryTsEmpMonthUpdtList.Remove(qryTsEmpMonthUpdtList.Length - 1); ;
                try
                {
                    sh.Run_UPDDEL_ExecuteNonQuery(qryupdtQuery);
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // _logger.Info("2.1.4 UpdateRequestFormODUsers after/end in timesheetbussiness " + endTime+ " duration "+duration);
            //}
        }

        //checking Holidays list and updating
        public void CheckHolidayList()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;
            // _logger.Info("2.3.1 CheckHolidayList start in timesheetbussiness " + startTime);
            string qryFillHolidaysInEmpMonth = "select Occasion, DATEPART(DAY, Date) as Day from HolidayList where DATEPART(DAY, Date) = " + _Day + " and DATEPART(MONTH, Date) = " + _Month + " and DATEPART(YEAR, Date)= " + _Year + ";";
            string qryupdtHolidayList = "";
            SqlHelper sh = new SqlHelper(_mavenConnStr);
            DataTable dtHoliday = sh.Get_Table_FromQry(qryFillHolidaysInEmpMonth);
            if (dtHoliday.Rows.Count > 0)
            {
                //string day = drHoliday["Day"].ToString();
                qryupdtHolidayList = " Update timesheet_Emp_Month set D" + _Day + "='HL',D" + _Day + "_Status = 'HL' where Year=" + _Year + " and Month=" + _Month + "; ";
                _Holiday = sh.Run_UPDDEL_ExecuteNonQuery(qryupdtHolidayList);
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // _logger.Info("2.3.2 CheckHolidayList end in timesheetbussiness " + endTime+ " duration "+duration);
        }

        //First sevice to get data from clint DB to our local DB
        public string GetBioMetricLogs(string _LastInsertedDate)
        {
            string selQry = "";
            string delQry = "";
            string seltimesheetQry = "";
            //chaitanya newly added on 11/4/2020
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            DateTime end1Time;
            DateTime end2Time;
            TimeSpan duration;
            // _logger.Info("1.1.1 GetBioMetricLogs start in timesheetbussiness " + startTime);

            //  selQry = "Select user_id,device_id,update_time,verify_mode,io_mode,io_time from tbl_realtime_glog";
            //delQry = "delete from timesheet_logs where convert(date,io_time) > (select case when DAY(getdate()) >25 " +
            //   "then concat(year(getdate()), '-', month(getdate()), '-', '25') " +
            //   "else convert(date, DATEADD(DAY, 0, DATEADD(MONTH, -1, DATEADD(DAY, 20 - DAY(getdate()), getdate())))) end)";
            //seltimesheetQry = "select * from timesheet_logs where convert(date,io_time) > (select case when DAY(getdate()) > 25 " +
            //    "then concat(year(getdate()), '-', month(getdate()), '-', '25') else convert(date, DATEADD(DAY, 0, DATEADD(MONTH, -1, DATEADD(DAY, 20 - DAY(getdate()), getdate())))) end) " +
            //    "order by io_time";
            delQry = "delete from timesheet_logs where convert(date,io_time) >= (select case when DAY(getdate()) >31 " + "then concat(year(getdate()), '-', month(getdate()), '-', '23') " +
              "else convert(date, DATEADD(DAY, 08, DATEADD(MONTH, -14, DATEADD(DAY, 0 - DAY(getdate()), getdate())))) end AS StartDate)" +
              "and convert(date,io_time) <=(SELECT CASE WHEN DAY(GETDATE()) > 31 THEN CONCAT(YEAR(GETDATE()), '-', MONTH(GETDATE()), '-', '31')" +
              "ELSE CONVERT(DATE, DATEADD(DAY, 11, DATEADD(MONTH, -14, DATEADD(DAY, 0 - DAY(GETDATE()), GETDATE())))) END AS EndDate) and user_id in  (844) ";

            seltimesheetQry = "select * from timesheet_logs where convert(date,io_time) >= (select case when DAY(getdate()) > 31 " +
                "then concat(year(getdate()), '-', month(getdate()), '-', '31') else convert(date, DATEADD(DAY, 8, DATEADD(MONTH, -14, DATEADD(DAY, 0 - DAY(getdate()), getdate())))) end AS StartDate)" +
               "and convert(date,io_time) <=(SELECT CASE WHEN DAY(GETDATE()) > 31 THEN CONCAT(YEAR(GETDATE()), '-', MONTH(GETDATE()), '-', '31')" +
               "ELSE CONVERT(DATE, DATEADD(DAY, 11, DATEADD(MONTH, -14, DATEADD(DAY, 0 - DAY(GETDATE()), GETDATE())))) END AS EndDate) and user_id in  (844)  ";
            ////string dtEmpIds1 = LastInsertedDate();
            //if (_LastInsertedDate != "")
            //{
            //    dtEmpIds1 = (_LastInsertedDate).ToString();
            //}
            //if (dtEmpIds1 != "")
            //{
            //    selQry += " WHERE io_time > '" + dtEmpIds1 + "' ";
            //}
            //selQry += " ORDER BY io_time ASC";

            SqlHelper sh1 = new SqlHelper(_mavenConnStr);
            DataTable seletedtEmpIds = sh1.Get_Table_FromQry(seltimesheetQry);
            if (seletedtEmpIds.Rows.Count > 0)
            {
                sh1.Run_UPDDEL_ExecuteNonQuery(delQry);
            }

            //selQry = "select * from tbl_realtime_glog where convert(date,io_time) > (select case when DAY(getdate()) > 25 " +
            //  "then concat(year(getdate()), '-', month(getdate()), '-', '25') else " +
            //  "convert(date, DATEADD(DAY, 0, DATEADD(MONTH, -1, DATEADD(DAY, 20 - DAY(getdate()), getdate())))) end) " +
            //  "order by io_time";
            selQry = "select user_id, io_time, device_id, update_time, verify_mode, io_mode, log_image from tbl_realtime_glog " +
               "where convert(date, io_time) >= (select case when DAY(getdate()) > 31 " +
               "then concat(year(getdate()), '-', month(getdate()), '-', '31') " +
               "else convert(date, DATEADD(DAY, 08, DATEADD(MONTH, -14, DATEADD(DAY, 0 - DAY(getdate()), getdate())))) end AS StartDate) " +
               "and convert(date, io_time) <= (SELECT CASE WHEN DAY(GETDATE()) > 31 " +
               "THEN CONCAT(YEAR(GETDATE()), '-', MONTH(GETDATE()), '-', '31') " +
               "ELSE CONVERT(DATE, DATEADD(DAY, 11, DATEADD(MONTH, -14, DATEADD(DAY, 0 - DAY(GETDATE()), GETDATE())))) END AS EndDate) and user_id in  (844)" +
               "group by user_id, io_time, device_id, update_time, verify_mode, io_mode, log_image " +
               "order by user_id, io_time;";
            SqlHelper sh = new SqlHelper(_timesheetDB);
            DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);
            end1Time = DateTime.Now;
            // _logger.Info("1.1.2 GetBioMetricLogs before insert tl: " + dtEmpIds.Rows.Count + " new records found " + end1Time);

            string inQry = "";
            if (dtEmpIds.Rows.Count > 0)
            {
                try
                {
                    int qrycount = 0;
                    int innerexexutedcnt = 0;
                    foreach (DataRow dr in dtEmpIds.Rows)
                    {
                        int allcnt = dtEmpIds.Rows.Count;
                        // int somecnt = allcnt / 4;
                        qrycount = qrycount + 1;
                        inQry += "INSERT INTO timesheet_logs([user_id],[device_id],[update_time],[verify_mode],[io_mode],[io_time])"
                                + "VALUES('" + dr["user_id"] + "','"
                                + dr["device_id"] + "','"
                                + Convert.ToDateTime(dr["update_time"]).ToString("yyyy-MM-dd HH:mm:ss") + "','"
                                + dr["verify_mode"] + "','"
                                + dr["io_mode"] + "','"
                                + Convert.ToDateTime(dr["io_time"]).ToString("yyyy-MM-dd HH:mm:ss") + "'); ";

                        // chaitanya newly added on 11 / 4 / 2020
                        // // _logger.Info("GetBioMetricLogs : Records added for the " + dr["user_id"]);
                        if (allcnt >= qrycount)
                        {
                            SqlHelper sh2 = new SqlHelper(_mavenConnStr);

                            sh2.Run_UPDDEL_ExecuteNonQuery(inQry);
                            //    // chaitanya newly added on  11/ 4 / 2020
                            inQry = "";
                            //  // _logger.Info("exicution is completed for user id '" + dr["user_id"] + "' ");
                        }

                    }
                    end2Time = DateTime.Now;
                    duration = end2Time - end1Time;

                    // _logger.Info("1.1.3 GetBioMetricLogs after insert tl: " + dtEmpIds.Rows.Count + " records added "+ end2Time+ " duration "+ duration);
                }
                catch (Exception ex)
                {
                    _logger.Error("Humm something went as unexpected", ex);
                }
            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // _logger.Info("1.1.4 GetBioMetricLogs end in timesheetbussiness " + endTime+ " duration "+ duration);

            return inQry;

        }

        private string LastInsertedDate()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;

            // _logger.Info("LastInsertedDate start in timesheetbussiness " + startTime);
            string retStr = "";
            SqlHelper sh = new SqlHelper(_mavenConnStr);

            //chaitanya newly added on 11/4/2020
            // _logger.Info("GetBioMetricLogs : LastInsertedDate method started its execution");

            DataTable dtEmpIds = sh.Get_Table_FromQry("SELECT TOP 1 io_time FROM timesheet_logs ORDER BY io_time DESC");

            //chaitanya newly added on 11/4/2020
            // _logger.Info("GetBioMetricLogs : Getting the max date from the table " + dtEmpIds.Rows[0]["io_time"]);

            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = DateTime.Parse(dr["io_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            }
            endTime = DateTime.Now;
            //chaitanya newly added on 11/4/2020
            // _logger.Info("GetBioMetricLogs : LastInsertedDate method ended its execution "+ endTime);
            return retStr;
        }

        // Time sheet Run Status
        public void InsertTimesheetReRunStatus(DateTime Datevalue)
        {

            DateTime startTime = DateTime.Now;
            DateTime endTime;
            TimeSpan duration;
            // _logger.Info("1.3.1 InsertTimesheetReRunStatus start in timesheetbussiness " + startTime);
            string strhdate = Datevalue.ToString("yyyy-MM-dd");
            string[] hsa1 = strhdate.Split('-');

            _Year = hsa1[0];
            _Month = hsa1[1];
            _Day = hsa1[2];

            int month = Convert.ToInt32(_Month);
            int year = Convert.ToInt32(_Year);

            // Get All Branches
            SqlHelper sh = new SqlHelper(_mavenConnStr);
            string selQryAllBranchDevice = "select  Device_id, branchName,BranchId from Branch_Device";

            DataTable dtAllBranchDevice = sh.Get_Table_FromQry(selQryAllBranchDevice);

            if (dtAllBranchDevice.Rows.Count > 0)
            {

                string qryDeviceUpdtList = "";

                //All brance in Timesheet_run_stats
                foreach (DataRow drDevice_id in dtAllBranchDevice.Rows)
                {
                    int index = 1;
                    int count = dtAllBranchDevice.Rows.Count;
                    string DeviceId = drDevice_id["Device_id"].ToString();

                    string qryBranchDeviceMonthUpdt = string.Format("({0},{1},'{2}',{3},'{4}')", year, month, drDevice_id["Device_id"], drDevice_id["BranchId"], drDevice_id["branchName"] + "") + ",";

                    if (index != (count + 1))
                    {
                        qryDeviceUpdtList += qryBranchDeviceMonthUpdt;
                    }
                    index++;

                }
                string qryupdtQuery = "INSERT INTO timesheet_run_status([yr],[mn],[device_id],[branch_id],[branch_name])" + "Values" + qryDeviceUpdtList.Remove(qryDeviceUpdtList.Length - 1);
                try
                {
                    sh.Run_UPDDEL_ExecuteNonQuery(qryupdtQuery);
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }

            }
            endTime = DateTime.Now;
            duration = endTime - startTime;
            // _logger.Info(" 1.3.2 InsertTimesheetReRunStatus end in timesheetbussiness " + endTime+ " Duration "+ duration);
        }




        //ClintDB to LocalDB Data Check in Devicewise
        private DataTable BrancheWiseTimeSheetLogs(DateTime DateValue)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;

            // _logger.Info("BrancheWiseTimeSheetLogs start in timesheetbussiness " + startTime);

            DateTime AddOne = Convert.ToDateTime(DateValue).AddDays(1);

            string timesheetconn = System.Configuration.ConfigurationManager.AppSettings.Get("timesheetdb");

            //clintDB
            string strquryAllBranch = "select  device_id,io_time,update_time,user_id,verify_mode,io_mode from tbl_realtime_glog  where (io_time>= '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "' and io_time  <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "'  or io_time >=  '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "'  and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            SqlHelper sss = new SqlHelper(timesheetconn);

            DataTable dtAllBranchDevice = sss.Get_Table_FromQry(strquryAllBranch);

            string inQry = "";
            string Upqry2 = "";

            string DeviceId = "";

            SqlHelper sh = new SqlHelper(_mavenConnStr);

            DataTable dtUpdatesDeviceData = new DataTable();

            if (dtAllBranchDevice.Rows.Count > 0)
            {
                foreach (DataRow dr in dtAllBranchDevice.Rows)
                {
                    DeviceId = dr["device_id"].ToString();
                    string io_time = dr["io_time"].ToString();


                    DateTime strdt = Convert.ToDateTime(io_time);
                    string strdate = strdt.ToString("yyyy-MM-dd");
                    string[] sa1 = strdate.Split('-');
                    string dt1 = sa1[0];
                    string dt2 = sa1[1];
                    string dt3 = sa1[2];
                    if (dt3.StartsWith("0"))
                    {
                        dt3 = dt3.Substring(1);
                    }


                    // if (DeviceId != "DB66ACB6A709442E")
                    //{
                    //hrmsdb 
                    string strAllBranchinLogTable = "select  device_id,io_time from timesheet_logs where  device_id='" + DeviceId + "' and (io_time>= '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "' and io_time  <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "'  or io_time >=  '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "'  and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                    DataTable dtAllStatsBranchDevice = sh.Get_Table_FromQry(strAllBranchinLogTable);

                    if (dtAllStatsBranchDevice.Rows.Count == 0)
                    {

                        inQry += "INSERT INTO timesheet_logs([user_id],[device_id],[update_time],[verify_mode],[io_mode],[io_time])"
                             + "VALUES('" + dr["user_id"] + "','"
                             + dr["device_id"] + "','"
                             + Convert.ToDateTime(dr["update_time"]).ToString("yyyy-MM-dd HH:mm:ss") + "','"
                             + dr["verify_mode"] + "','"
                             + dr["io_mode"] + "','"
                             + Convert.ToDateTime(dr["io_time"]).ToString("yyyy-MM-dd HH:mm:ss") + "'); ";


                    }

                    Upqry2 += " Update timesheet_run_status set D" + dt3 + "=1 where device_id='" + dr["device_id"] + "' and mn = " + dt2 + " and yr = " + dt1 + ";";
                    //}

                }


                try
                {
                    if (inQry != "")
                    {
                        sh.Run_UPDDEL_ExecuteNonQuery(inQry + Upqry2);
                    }
                    else
                    {
                        sh.Run_UPDDEL_ExecuteNonQuery(Upqry2);
                    }

                    string UpdatedBranceDataTable = "select  device_id,io_time from timesheet_logs where (io_time>= '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "' and io_time  <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "'  or io_time >=  '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "'  and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                    dtUpdatesDeviceData = sh.Get_Table_FromQry(UpdatedBranceDataTable);

                }
                catch (Exception ex)
                {
                    _logger.Error("something went as unexpected", ex);
                }

                return dtUpdatesDeviceData;
            }
            endTime = DateTime.Now;
            // _logger.Info("BrancheWiseTimeSheetLogs end in timesheetbussiness " + endTime);
            return null;

        }

        // Singal Day 
        public void UpdateTimeSheetDayWiseBranchStatus(DateTime DateValue)
        {
            DateTime startTime;
            DateTime endTime;
            TimeSpan duration;
            DateTime start1Time;

            DateTime end1Time;
            //// _logger.Info("1.5.1 UpdateTimeSheetDayWiseBranchStatus start in timesheetbussiness " + startTime);

            SqlHelper sh = new SqlHelper(_mavenConnStr);
            string timesheetconn = System.Configuration.ConfigurationManager.AppSettings.Get("timesheetdb");
            SqlHelper sss = new SqlHelper(timesheetconn);

            string StatsDeviceId = "";

            DateTime hdt = Convert.ToDateTime(DateValue);
            string strhdate = hdt.ToString("yyyy-MM-dd");
            string[] hsa1 = strhdate.Split('-');
            string hdt1 = hsa1[0];
            string hdt2 = hsa1[1];
            string hdt3 = hsa1[2];

            if (hdt3.StartsWith("0"))
            {
                hdt3 = hdt3.Substring(1);
            }


            string qryFillHolidaysInEmpMonth = "select Occasion, DATEPART(DAY, Date) as Day from HolidayList where DATEPART(DAY, Date) = " + hdt3 + " and DATEPART(MONTH, Date) = " + hdt2 + " and DATEPART(YEAR, Date)= " + hdt1 + ";";
            startTime = DateTime.Now;
            DataTable dtHoliday = sh.Get_Table_FromQry(qryFillHolidaysInEmpMonth);
            endTime = DateTime.Now;
            Console.WriteLine($"15-1520: {endTime - startTime}");
            if (dtHoliday.Rows.Count > 0)
            {

                string query2 = "Update timesheet_run_status set D" + hdt3 + "=1 where mn = " + hdt2 + " and yr = " + hdt1 + "";
                startTime = DateTime.Now;
                sh.Run_UPDDEL_ExecuteNonQuery(query2);
                endTime = DateTime.Now;
                Console.WriteLine($"16-1528: {endTime - startTime}");

            }
            else
            {
                DateTime AddOne = Convert.ToDateTime(DateValue).AddDays(1);
                // DataTable GetUpdatedBranchData =BrancheWiseTimeSheetLogs(DateValue.Value);
                string AllbranchesListTable = "select Device_Id,BranchId from Branch_Device";
                DataTable FindDevice = sh.Get_Table_FromQry(AllbranchesListTable);
                start1Time = DateTime.Now;
                // _logger.Info("1.5.2 UpdateTimeSheetDayWiseBranchStatus before  loop " + start1Time);
                foreach (DataRow rowSampleOne in FindDevice.Rows)
                {
                    //StatsDeviceId = "DB677C516326132F";
                    StatsDeviceId = rowSampleOne["device_id"].ToString();
                    //string BranchId = "24";
                    string BranchId = rowSampleOne["BranchId"].ToString();
                    string qrytimesheetlogs = "select * from timesheet_logs  where device_id='" + StatsDeviceId + "' " +
                        "and(io_time>= '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                        "and io_time  <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "'  " +
                        "or io_time >=  '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "'  " +
                        "and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    //select (
                    string countlogs = "select count(1) as count from tbl_realtime_glog " +
                        "where device_id='" + StatsDeviceId + "' and(io_time>='" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "'  " +
                        "and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "' or io_time >=  '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "'  " +
                        "and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    string countlogsmdb = "select count(1) as count from timesheet_logs " +
                    "where device_id = '" + StatsDeviceId + "' and(io_time >= '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                    "and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "' or io_time >= '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                    "and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    //")-(select count(1) from timesheet_logs " +
                    //"where device_id = '" + StatsDeviceId + "' and(io_time >= '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                    //"and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "' or io_time >= '" + DateValue.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                    //"and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "')) as count";
                    startTime = DateTime.Now;

                    DataTable dtDeviceTimesheetlogs = sh.Get_Table_FromQry(qrytimesheetlogs);
                    endTime = DateTime.Now;
                    Console.WriteLine($"6-1562: {endTime - startTime}");
                    startTime = DateTime.Now;

                    DataTable dtcountrecords = sss.Get_Table_FromQry(countlogs);
                    endTime = DateTime.Now;
                    Console.WriteLine($"7: {endTime - startTime}");
                    startTime = DateTime.Now;

                    DataTable dtcountrecordsmdb = sh.Get_Table_FromQry(countlogsmdb);
                    endTime = DateTime.Now;
                    Console.WriteLine($"8: {endTime - startTime}");

                    int rcount = Convert.ToInt32(dtcountrecords.Rows[0]["count"]);
                    int rcountmdb = Convert.ToInt32(dtcountrecordsmdb.Rows[0]["count"]);
                    if (dtDeviceTimesheetlogs.Rows.Count > 0)
                    {
                        string qryupdatetimesheet = "Update timesheet_run_status set D" + hdt3 + "=1 where device_id='" + StatsDeviceId + "' and mn = " + hdt2 + " and yr = " + hdt1 + "";
                        startTime = DateTime.Now;

                        sh.Run_UPDDEL_ExecuteNonQuery(qryupdatetimesheet);
                        endTime = DateTime.Now;
                        Console.WriteLine($"9- 1578: {endTime - startTime}");
                    }
                    else if (rcount > rcountmdb)
                    {
                        string query = "Update timesheet_run_status set D" + hdt3 + "=0 where device_id='" + StatsDeviceId + "' and mn = " + hdt2 + " and yr = " + hdt1 + "";
                        startTime = DateTime.Now;

                        sh.Run_UPDDEL_ExecuteNonQuery(query);
                        endTime = DateTime.Now;
                        Console.WriteLine($"10-1586: {endTime - startTime}");
                    }
                    else
                    {
                        string query = "Update timesheet_run_status set D" + hdt3 + "=0 where device_id='" + StatsDeviceId + "' and mn = " + hdt2 + " and yr = " + hdt1 + "";
                        startTime = DateTime.Now;

                        sh.Run_UPDDEL_ExecuteNonQuery(query);
                        endTime = DateTime.Now;
                        Console.WriteLine($"11: {endTime - startTime}");
                    }
                }


            }

        }


        private string GetLogs4ReRun(string DeviceId, string StatsReRunDate)
        {
            DateTime startTime;
            DateTime endTime;

            //// _logger.Info("Enter method GetLogs4ReRun to bring SSSDB logs for Device: " + DeviceId + ", Date: " + StatsReRunDate);
            SqlHelper sh = new SqlHelper(_mavenConnStr);

            //Add Day
            DateTime AddOne = Convert.ToDateTime(StatsReRunDate).AddDays(1);

            //delete rows in mavensoft db for that branch , date
            //string strAllBranchinLogTableData = "delete from timesheet_logs where  device_id='" + DeviceId + "' and (io_time>= '" + Convert.ToDateTime(StatsReRunDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and io_time  <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "'  or io_time >=  '" + Convert.ToDateTime(StatsReRunDate).ToString("yyyy-MM-dd HH:mm:ss") + "'  and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            //sh.Run_UPDDEL_ExecuteNonQuery(strAllBranchinLogTableData);

            //getrows from sssdb and insert into mavesoft db
            string timesheetconn = System.Configuration.ConfigurationManager.AppSettings.Get("timesheetdb");

            //ClintDB
            string strquryAllBranch = "select device_id,io_time,update_time,user_id,verify_mode,io_mode,io_time from tbl_realtime_glog where device_id='" + DeviceId + "' and (io_time>= '" + Convert.ToDateTime(StatsReRunDate).ToString("yyyy-MM-dd HH:mm:ss") + "' and io_time  <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "'  or io_time >=  '" + Convert.ToDateTime(StatsReRunDate).ToString("yyyy-MM-dd HH:mm:ss") + "'  and io_time <= '" + AddOne.ToString("yyyy-MM-dd HH:mm:ss") + "')";

            SqlHelper sss = new SqlHelper(timesheetconn);
            startTime = DateTime.Now;
            DataTable BranchTimeSheetlogs = sss.Get_Table_FromQry(strquryAllBranch);
            endTime = DateTime.Now;
            Console.WriteLine($"70-1660: {endTime - startTime}");
            string inQry = "";


            if (BranchTimeSheetlogs.Rows.Count > 0)
            {
                foreach (DataRow dtBranchdata in BranchTimeSheetlogs.Rows)
                {

                    inQry += "INSERT INTO timesheet_logs([user_id],[device_id],[update_time],[verify_mode],[io_mode],[io_time])"
                    + "VALUES('" + dtBranchdata["user_id"] + "','"
                    + dtBranchdata["device_id"] + "','"
                    + Convert.ToDateTime(dtBranchdata["update_time"]).ToString("yyyy-MM-dd HH:mm:ss") + "','"
                    + dtBranchdata["verify_mode"] + "','"
                    + dtBranchdata["io_mode"] + "','"
                    + Convert.ToDateTime(dtBranchdata["io_time"]).ToString("yyyy-MM-dd HH:mm:ss") + "'); ";
                }

                //    // _logger.Info("2.13.2.3 GetLogs4ReRun before query execute in timesheetbussiness " + startTime);
                try
                {
                    startTime = DateTime.Now;
                    sh.Run_UPDDEL_ExecuteNonQuery(inQry);
                    endTime = DateTime.Now;
                    Console.WriteLine($"71-1685: {endTime - startTime}");
                }
                catch (Exception ex)
                {
                    _logger.Error("** 2.13.2.4 something went wrong while saving to DB. Query: " + ex.ToString() + " , Query=" + inQry);
                    return null;
                }
                DateTime strdt = Convert.ToDateTime(StatsReRunDate);
                string strdate = strdt.ToString("yyyy-MM-dd");
                string[] sa1 = strdate.Split('-');
                string dt1 = sa1[0];
                string dt2 = sa1[1];
                string dt3 = sa1[2];
                if (dt3.StartsWith("0"))
                {
                    dt3 = dt3.Substring(1);
                }

                startTime = DateTime.Now;
                //Timesheet Run Status Update
                string PendingStatusUpdate = "Update timesheet_run_status set D" + dt3 + "=1 where device_id='" + DeviceId + "' and mn = " + dt2 + " and yr = " + dt1 + "";
                //// _logger.Info("Exit method GetLogs4ReRun ");
                endTime = DateTime.Now;
                Console.WriteLine($"72-1707: {endTime - startTime}");
                return PendingStatusUpdate;

            }
            else
            {
                // // _logger.Info("* No data found in SSSDB for date:" + StatsReRunDate + " , Devide Id:" + DeviceId);
            }

            // // _logger.Info("Exit method GetLogs4ReRun ");

            return null;
        }

        //RerunForGiven Branches
        public void RerunForGivenBranches()
        {
            DateTime startTime;
            DateTime endTime;

            //// _logger.Info("2.13.1 RerunForGivenBranches start in timesheetbussiness " + startTime);
            try
            {
                SqlHelper sh = new SqlHelper(_mavenConnStr);
                string strRerunbranches = "select device_id,run_date,branch_id from timesheet_branch_rerun where active=1 ";
                startTime = DateTime.Now;
                DataTable dtAllStatsBranchDevice = sh.Get_Table_FromQry(strRerunbranches);
                endTime = DateTime.Now;
                Console.WriteLine($"57.1-1719: {endTime - startTime}");
                // // _logger.Info("2.13.2 RerunForGivenBranches before loop in timesheetbussiness " + fstartTime);
                if (dtAllStatsBranchDevice.Rows.Count > 0)
                {
                    foreach (DataRow drreturn in dtAllStatsBranchDevice.Rows)
                    {
                        string DeviceId = drreturn["device_id"].ToString();
                        string StatsReRunDate = drreturn["run_date"].ToString();
                        string BranchId = drreturn["branch_id"].ToString();
                        startTime = DateTime.Now;
                        var updQry = GetLogs4ReRun(DeviceId, StatsReRunDate);
                        endTime = DateTime.Now;
                        Console.WriteLine($"57.2-1731: {endTime - startTime}");
                        if (updQry != null)
                        {
                            //update TimesheetReportData
                            startTime = DateTime.Now;
                            GenerateTimesheetReportsData(true, Convert.ToDateTime(StatsReRunDate), null, DeviceId, BranchId);
                            endTime = DateTime.Now;
                            Console.WriteLine($"57.3-1737: {endTime - startTime}");
                            //fail Device update in time sheet ReRun Branch
                            string qryupdateRerunStatus = "Update timesheet_branch_rerun set active=0 where device_id='" + DeviceId + "' and run_date ='" + Convert.ToDateTime(StatsReRunDate).ToString("yyyy-MM-dd") + "' ; ";
                            startTime = DateTime.Now;
                            sh.Run_UPDDEL_ExecuteNonQuery(qryupdateRerunStatus + updQry);
                            endTime = DateTime.Now;
                            Console.WriteLine($"57.4-1743: {endTime - startTime}");
                        }

                    }

                }

            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
            }
        }
        //newly added on 13/05/2020
        public void Check_Last_15days(DateTime Datevalue)
        {
            // DateTime cstartTime = DateTime.Now;
            DateTime startTime;

            DateTime endTime;

            // // _logger.Info("2.11.1 Check_Last_15days start in timesheetbussiness " + cstartTime);
            string strD = "", strD_new = "", strD_new1 = "", strM = "", strY = "";
            string strQuery1 = "", strQuery1_final = "", strQuery2 = "", strQuery2_final = "";
            string[] sa1;
            string dt_dtnew, dty, dtm, dtd;

            DateTime old_date = DateTime.MinValue;
            DateTime new_date = DateTime.Today;

            // // _logger.Info("2.11.2 Check_Last_15days before if loop in timesheetbussiness " + startTime);
            if ((Datevalue != null && Datevalue != old_date) && (Datevalue != null || Datevalue != old_date))
            {

                dt_dtnew = Datevalue.ToString("yyyy-MM-dd");
                sa1 = dt_dtnew.Split('-');
                dty = sa1[0];
                dtm = sa1[1];
                dtd = sa1[2];
            }
            else
            {
                Datevalue = new_date;
                dt_dtnew = Datevalue.ToString("yyyy-MM-dd");
                sa1 = dt_dtnew.Split('-');
                dty = sa1[0];
                dtm = sa1[1];
                dtd = sa1[2];
            }

            //for  insert query excution
            string qryinsertRerunStatus = "", qryinsertRerunStatus_new = "", qryinsertRerunStatus_final = "";
            //to get the first day and last day of a particular date
            DateTime firstday_samemonth = new DateTime(Datevalue.Year, Datevalue.Month, 1);
            DateTime lastday_samemonth1 = firstday_samemonth.AddMonths(1).AddDays(-1);
            DateTime dtlast15days = Convert.ToDateTime(Datevalue).AddDays(-5);
            //Convert.ToDateTime(dtlast15days).ToString("yyyy-MM-dd");
            string dtlast15days_new = dtlast15days.ToString("yyyy-MM-dd");
            string[] sa12 = dtlast15days_new.Split('-');
            string dty1 = sa12[0];
            string dtm1 = sa12[1];
            string dtd1 = sa12[2];

            //to get the first day and last day of a particular date
            DateTime firsttday_nosamemonth = new DateTime(dtlast15days.Year, dtlast15days.Month, 1);
            DateTime lastday_nosamemonth1 = firsttday_nosamemonth.AddMonths(1).AddDays(-1);
            DataSet ds_q1, ds_q2;
            int ds_count = 0; // to loop the table count with dataset
            int interval = 1;

            try
            {
                SqlHelper sh = new SqlHelper(_mavenConnStr);
                //if 15days span are in the same month
                if (dtm == dtm1)
                {

                    for (int i_y = Convert.ToInt32(dty); i_y <= Convert.ToInt32(dty); i_y++)
                    {
                        strY = " where ( yr=" + i_y + ") and ";
                        for (int i_m = Convert.ToInt32(dtm); i_m <= Convert.ToInt32(dtm); i_m++)
                        {
                            strM = "(mn=" + i_m + ") ";
                            for (DateTime dateTime = dtlast15days; dateTime <= Datevalue; dateTime += TimeSpan.FromDays(interval))
                            {
                                strQuery2 = "update timesheet_run_status set ";
                                int day = dateTime.Day;
                                string tempd = day.ToString();
                                if (tempd.StartsWith("0"))
                                {
                                    tempd.Substring(1);
                                    strD = " D" + tempd + "=0 ";
                                    strD_new = " ( D" + tempd + " is null ) ; ";
                                }
                                else
                                {
                                    strD = " D" + tempd + "=0 ";
                                    strD_new = " and ( D" + tempd + " is null ) ; ";
                                }
                                strD = strD.Remove(strD.Length - 1, 1);
                                strQuery2 += strD + strY + strM + strD_new;
                                strQuery2_final += strQuery2;
                                strD = "";
                            }
                            strM = "";
                        }
                        strY = "";
                    }
                    startTime = DateTime.Now;
                    bool b_updatequery = sh.Run_UPDDEL_ExecuteNonQuery(strQuery2_final);
                    endTime = DateTime.Now;
                    Console.WriteLine($"56.1-1862: {endTime - startTime}");
                    if (b_updatequery == true)
                    {
                        //// _logger.Info("Query executed is : " + strQuery2_final);
                        strQuery2_final = "";
                        strQuery2 = "";
                        strD_new = "";
                    }
                    else
                    {
                        //// _logger.Info("Query executed is : "+ strQuery2_final+" and no records updated");
                        strQuery2_final = "";
                        strQuery2 = "";
                        strD_new = "";
                    }
                    for (int i_y = Convert.ToInt32(dty); i_y <= Convert.ToInt32(dty); i_y++)
                    {
                        strY = " from timesheet_run_status where ( yr=" + i_y + ") and ";
                        for (int i_m = Convert.ToInt32(dtm); i_m <= Convert.ToInt32(dtm); i_m++)
                        {
                            strM = "(mn=" + i_m + ") ";
                            //getting the select queries for all days individually and getting 15 tables
                            //Example: select yr,mn,device_id,branch_id, branch_name, D1 from timesheet_run_status where (mn=5) and (yr=2020) and (D14=0 or D14=2)
                            for (DateTime dateTime = dtlast15days; dateTime <= Datevalue; dateTime += TimeSpan.FromDays(interval))
                            {

                                strQuery2 = "select yr,mn,device_id,branch_id, branch_name, ";
                                int day = dateTime.Day;
                                string tempd = day.ToString();
                                if (tempd.StartsWith("0"))
                                {
                                    tempd.Substring(1);
                                    strD = " D" + tempd + ", ";
                                    strD_new = " and ( D" + tempd + "=0 or D" + tempd + "=2); ";
                                }
                                else
                                {
                                    strD = " D" + tempd + " ";
                                    strD_new = " and ( D" + tempd + "=0 or D" + tempd + "=2); ";
                                }
                                strD = strD.Remove(strD.Length - 1, 1);
                                strQuery2 += strD + strY + strM + strD_new;
                                strQuery2_final += strQuery2;
                                strD = "";
                            }
                            strM = "";
                        }
                        strY = "";
                    }
                    //// _logger.Info("Select Query is :"+ strQuery2_final);
                    startTime = DateTime.Now;
                    ds_q2 = sh.Get_MultiTables_FromQry(strQuery2_final);
                    endTime = DateTime.Now;
                    Console.WriteLine($"56.2-1915: {endTime - startTime}");
                    // // _logger.Info("Dataset count after execution is :" + ds_q2.Tables.Count);

                    for (ds_count = 0; ds_count < ds_q2.Tables.Count; ds_count++)
                    {
                        if (ds_q2.Tables[ds_count].Rows.Count > 0)
                        {
                            foreach (DataRow dsreturn in ds_q2.Tables[ds_count].Rows)
                            {
                                string DeviceId = dsreturn["device_id"].ToString();
                                string BranchId = dsreturn["branch_id"].ToString();
                                int year = Convert.ToInt32(dsreturn["yr"]);
                                string month = (dsreturn["mn"].ToString());
                                string day = ds_q2.Tables[ds_count].Columns[5].ColumnName;
                                string day_new = day.Substring(1);
                                //if(month.Length!=2)
                                //{
                                //    month = "0"+month;
                                //}
                                //else
                                //{
                                //}
                                string Query_new = "";
                                if (day_new.Length == 2)
                                {
                                    Query_new = "Select year(run_date) as year,month(run_date) as month, device_id, branch_id from timesheet_branch_rerun where active=1 and year(run_date)=" + year + " and month(run_date)=0" + month + " and run_date='" + year + "-0" + month + "-" + day_new + "' and branch_id=" + BranchId + " and device_id='" + DeviceId + "';";
                                }
                                else if (day_new.Length == 1)
                                {
                                    day_new = "0" + day_new;
                                    Query_new = "Select year(run_date) as year,month(run_date) as month, device_id, branch_id from timesheet_branch_rerun where active=1 and year(run_date)=" + year + " and month(run_date)=0" + month + " and run_date='" + year + "-0" + month + "-" + day_new + "' and branch_id=" + BranchId + " and device_id='" + DeviceId + "';";
                                }
                                startTime = DateTime.Now;
                                DataTable dt_select = sh.Get_Table_FromQry(Query_new);
                                endTime = DateTime.Now;
                                Console.WriteLine($"56.3-1950: {endTime - startTime}");
                                //checking the deviceid, branchid for a particuar date in a timesheet_branch_rerun table where active=1
                                //if record doesn't exist for that date then we insert into timesheet_branch_rerun
                                if (dt_select.Rows.Count == 0)
                                {
                                    qryinsertRerunStatus = "Insert into timesheet_branch_rerun([branch_id], [device_id], [run_date], [active], [rerun_status]) values (" + BranchId + ", '" + DeviceId + "', '" + year + "-" + month + "-" + day_new + "',1,'Complete')";
                                    //// _logger.Info("Select Query Executed is :"+ qryinsertRerunStatus);
                                    startTime = DateTime.Now;
                                    bool b1 = sh.Run_UPDDEL_ExecuteNonQuery(qryinsertRerunStatus);
                                    endTime = DateTime.Now;
                                    Console.WriteLine($"56.4-1960: {endTime - startTime}");
                                    if (b1 == true)
                                    {
                                        // // _logger.Info("Records inserted sucessfully" + qryinsertRerunStatus);
                                        qryinsertRerunStatus = "";
                                    }
                                    else
                                    {
                                        //// _logger.Info("execution Failed" + qryinsertRerunStatus);
                                        qryinsertRerunStatus = "";
                                    }
                                }
                                else
                                {
                                    // if record for deviceid, branchid for a particuar date in a timesheet_branch_rerun table where active=1
                                    //already exists then do nothing
                                }

                            }

                        }
                        //ds_count++;
                    }
                }
                //if 15days span are not in the same month

                else if (dtm != dtm1)

                {


                    for (int i_y = Convert.ToInt32(dty1); i_y <= Convert.ToInt32(dty1); i_y++)
                    {
                        strY = " where ( yr=" + i_y + ") and ";
                        for (int i_m = Convert.ToInt32(dtm1); i_m <= Convert.ToInt32(dtm1); i_m++)
                        {
                            strM = "(mn=" + i_m + ") ";
                            for (DateTime dateTime = dtlast15days; dateTime <= lastday_nosamemonth1; dateTime += TimeSpan.FromDays(interval))
                            {
                                strQuery2 = "update timesheet_run_status set ";
                                int day = dateTime.Day;
                                string tempd = day.ToString();
                                if (tempd.StartsWith("0"))
                                {
                                    tempd.Substring(1);
                                    strD = " D" + tempd + "=0 ";
                                    strD_new = " ( D" + tempd + " is null ) ; ";
                                }
                                else
                                {
                                    strD = " D" + tempd + "=0 ";
                                    strD_new = " and ( D" + tempd + " is null ) ; ";
                                }
                                strD = strD.Remove(strD.Length - 1, 1);
                                strQuery2 += strD + strY + strM + strD_new;
                                strQuery2_final += strQuery2;
                                strD = "";
                            }
                            strM = "";
                        }
                        strY = "";
                    }
                    startTime = DateTime.Now;
                    bool b_updatequerynosamemnonth1 = sh.Run_UPDDEL_ExecuteNonQuery(strQuery2_final);
                    endTime = DateTime.Now;
                    Console.WriteLine($"56.5-2025: {endTime - startTime}");
                    if (b_updatequerynosamemnonth1 == true)
                    {
                        //// _logger.Info("Query executed is : " + strQuery2_final);
                        strQuery2_final = "";
                        strQuery2 = "";
                        strD_new = "";
                    }
                    else
                    {
                        //// _logger.Info("Query executed is : " + strQuery2_final + " and Failed Execution");
                        strQuery2_final = "";
                        strQuery2 = "";
                        strD_new = "";
                    }
                    for (int i_y = Convert.ToInt32(dty1); i_y <= Convert.ToInt32(dty1); i_y++)
                    {
                        strY = " from timesheet_run_status where ( yr=" + i_y + ") and ";
                        for (int i_m = Convert.ToInt32(dtm1); i_m <= Convert.ToInt32(dtm1); i_m++)
                        {
                            strM = "(mn=" + i_m + ") ";
                            //getting the select queries for all days individually for the first month
                            //Example: select yr,mn,device_id,branch_id, branch_name, D1 from timesheet_run_status where (mn=5) and (yr=2020) and (D14=0 or D14=2)
                            for (DateTime dateTime = dtlast15days; dateTime <= lastday_nosamemonth1; dateTime += TimeSpan.FromDays(interval))
                            {

                                strQuery1 = "select yr,mn,device_id,branch_id, branch_name, ";
                                int day = dateTime.Day;
                                string tempd = day.ToString();
                                if (tempd.StartsWith("0"))
                                {
                                    tempd.Substring(1);
                                    strD = " D" + tempd + ", ";
                                    strD_new = " and ( D" + tempd + "=0 or D" + tempd + "=2); ";
                                }
                                else
                                {
                                    strD = " D" + tempd + ", ";
                                    strD_new = " and ( D" + tempd + "=0 or D" + tempd + "=2); ";
                                }
                                strD = strD.Remove(strD.Length - 2, 2);
                                strQuery1 += strD + strY + strM + strD_new;
                                strQuery1_final += strQuery1;
                                strD = ""; strD_new = "";
                            }
                            strM = "";

                        }
                        strY = "";
                    }
                    // // _logger.Info("Select Query for the 1st month is :" + strQuery1_final);
                    startTime = DateTime.Now;
                    ds_q1 = sh.Get_MultiTables_FromQry(strQuery1_final);
                    endTime = DateTime.Now;
                    Console.WriteLine($"56.6-2079: {endTime - startTime}");
                    // // _logger.Info("After Execution dataset tables are :" + ds_q1.Tables.Count);
                    for (ds_count = 0; ds_count < ds_q1.Tables.Count; ds_count++)
                    {
                        if (ds_q1.Tables[ds_count].Rows.Count > 0)
                        {
                            foreach (DataRow dsreturn in ds_q1.Tables[ds_count].Rows)
                            {
                                string DeviceId = dsreturn["device_id"].ToString();
                                string BranchId = dsreturn["branch_id"].ToString();
                                int year = Convert.ToInt32(dsreturn["yr"]);
                                int month = Convert.ToInt32(dsreturn["mn"]);
                                string day = ds_q1.Tables[ds_count].Columns[5].ColumnName;
                                string day_new = day.Substring(1);
                                string Query_new = "";
                                if (day_new.Length == 2)
                                {
                                    //checks for the records in timesheet_branch_rerun for the branch id, device id for that date.
                                    Query_new = "Select year(run_date) as year,month(run_date) as month, device_id, branch_id from timesheet_branch_rerun where active=1 and year(run_date)=" + year + " and month(run_date)=" + month + " and run_date='" + year + "-" + month + "-" + day_new + "' and branch_id=" + BranchId + " and device_id='" + DeviceId + "';";
                                }
                                else if (day_new.Length == 1)
                                {
                                    day_new = "0" + day_new;
                                    Query_new = "Select year(run_date) as year,month(run_date) as month, device_id, branch_id from timesheet_branch_rerun where active=1 and year(run_date)=" + year + " and month(run_date)=" + month + " and run_date='" + year + "-" + month + "-" + day_new + "' and branch_id=" + BranchId + " and device_id='" + DeviceId + "';";
                                }
                                startTime = DateTime.Now;
                                DataTable dt_select = sh.Get_Table_FromQry(Query_new);
                                endTime = DateTime.Now;
                                Console.WriteLine($"56.7-2105: {endTime - startTime}");
                                //checking the deviceid, branchid for a particuar date in a timesheet_branch_rerun table where active=1
                                //if record doesn't exist for that date then we insert into timesheet_branch_rerun
                                if (dt_select.Rows.Count == 0)
                                {
                                    qryinsertRerunStatus = "Insert into timesheet_branch_rerun([branch_id], [device_id], [run_date], [active], [rerun_status]) values (" + BranchId + ", '" + DeviceId + "', '" + year + "-" + month + "-" + day_new + "',1,'Complete')";
                                    //// _logger.Info("Select Query Executed is :" + qryinsertRerunStatus);
                                    startTime = DateTime.Now;
                                    bool b1 = sh.Run_UPDDEL_ExecuteNonQuery(qryinsertRerunStatus);
                                    endTime = DateTime.Now;
                                    Console.WriteLine($"56.8-2115: {endTime - startTime}");
                                    if (b1 == true)
                                    {
                                        // // _logger.Info("Records inserted sucessfully" + qryinsertRerunStatus);
                                        qryinsertRerunStatus = "";
                                    }
                                    else
                                    {
                                        // // _logger.Info("execution Failed" + qryinsertRerunStatus);
                                        qryinsertRerunStatus = "";
                                    }
                                }
                                else
                                {
                                    // if record for deviceid, branchid for a particuar date in a timesheet_branch_rerun table where active=1
                                    //already exists then do nothing
                                }

                            }

                        }
                        //ds_count++;
                    }

                    //updates the timesheet_run_status set D6=0 where D6 = Null 
                    for (int i_y = Convert.ToInt32(dty); i_y <= Convert.ToInt32(dty); i_y++)
                    {
                        strY = " where ( yr=" + i_y + ") and ";
                        for (int i_m = Convert.ToInt32(dtm); i_m <= Convert.ToInt32(dtm); i_m++)
                        {
                            strM = "(mn=" + i_m + ") ";
                            for (DateTime dateTime = firstday_samemonth; dateTime <= Datevalue; dateTime += TimeSpan.FromDays(interval))
                            {
                                strQuery2 = "update timesheet_run_status set ";
                                int day = dateTime.Day;
                                string tempd = day.ToString();
                                if (tempd.StartsWith("0"))
                                {
                                    tempd.Substring(1);
                                    strD = " D" + tempd + "=0 ";
                                    strD_new = " ( D" + tempd + " is null ) ; ";
                                }
                                else
                                {
                                    strD = " D" + tempd + "=0 ";
                                    strD_new = " and ( D" + tempd + " is null ) ; ";
                                }
                                strD = strD.Remove(strD.Length - 1, 1);
                                strQuery2 += strD + strY + strM + strD_new;
                                strQuery2_final += strQuery2;
                                strD = "";
                            }
                            strM = "";
                        }
                        strY = "";
                    }
                    startTime = DateTime.Now;
                    bool b_updatequerynosamemnonth2 = sh.Run_UPDDEL_ExecuteNonQuery(strQuery2_final);
                    endTime = DateTime.Now;
                    Console.WriteLine($"56.9-2170: {endTime - startTime}");
                    if (b_updatequerynosamemnonth2 == true)
                    {
                        //// _logger.Info("Query executed is : " + strQuery2_final);
                        strQuery2_final = "";
                        strQuery2 = "";
                        strD_new = "";
                    }
                    else
                    {
                        //// _logger.Info("Query executed is : " + strQuery2_final + " and execution failed");
                        strQuery2_final = "";
                        strQuery2 = "";
                        strD_new = "";
                    }

                    for (int i_y = Convert.ToInt32(dty); i_y <= Convert.ToInt32(dty); i_y++)
                    {
                        strY = " from timesheet_run_status where ( yr=" + i_y + ") and ";
                        for (int i_m = Convert.ToInt32(dtm); i_m <= Convert.ToInt32(dtm); i_m++)
                        {
                            strM = "(mn=" + i_m + ") ";
                            //getting the select queries for all days individually in the second month
                            //Example: select yr,mn,device_id,branch_id, branch_name, D1 from timesheet_run_status where (mn=5) and (yr=2020) and (D14=0 or D14=2)
                            for (DateTime dateTime = firstday_samemonth; dateTime <= Datevalue; dateTime += TimeSpan.FromDays(interval))
                            {

                                strQuery2 = "select yr,mn,device_id,branch_id, branch_name, ";
                                int day = dateTime.Day;
                                string tempd = day.ToString();
                                if (tempd.StartsWith("0"))
                                {
                                    tempd.Substring(1);
                                    strD = " D" + tempd + ", ";
                                    strD_new = " and ( D" + tempd + "=0 or D" + tempd + "=2); ";
                                }
                                else
                                {
                                    strD = " D" + tempd + " ";
                                    strD_new = " and ( D" + tempd + "=0 or D" + tempd + "=2); ";
                                }
                                strD = strD.Remove(strD.Length - 1, 1);
                                strQuery2 += strD + strY + strM + strD_new;
                                strQuery2_final += strQuery2;
                                strD = "";
                            }
                            strM = "";
                        }
                        strY = "";
                    }

                    startTime = DateTime.Now;
                    ds_q2 = sh.Get_MultiTables_FromQry(strQuery2_final);
                    endTime = DateTime.Now;
                    Console.WriteLine($"56.10-2228: {endTime - startTime}");
                    for (ds_count = 0; ds_count < ds_q2.Tables.Count; ds_count++)
                    {
                        if (ds_q2.Tables[ds_count].Rows.Count > 0)
                        {
                            foreach (DataRow dsreturn in ds_q2.Tables[ds_count].Rows)
                            {
                                string DeviceId = dsreturn["device_id"].ToString();
                                string BranchId = dsreturn["branch_id"].ToString();
                                int year = Convert.ToInt32(dsreturn["yr"]);
                                int month = Convert.ToInt32(dsreturn["mn"]);
                                string day = ds_q2.Tables[ds_count].Columns[5].ColumnName;
                                string day_new = day.Substring(1);
                                string Query_new = "";
                                if (day_new.Length == 2)
                                {
                                    Query_new = "Select year(run_date) as year,month(run_date) as month, device_id, branch_id from timesheet_branch_rerun where active=1 and year(run_date)=" + year + " and month(run_date)=" + month + " and run_date='" + year + "-" + month + "-" + day_new + "' and branch_id=" + BranchId + " and device_id='" + DeviceId + "';";
                                }
                                else if (day_new.Length == 1)
                                {
                                    day_new = "0" + day_new;
                                    Query_new = "Select year(run_date) as year,month(run_date) as month, device_id, branch_id from timesheet_branch_rerun where active=1 and year(run_date)=" + year + " and month(run_date)=" + month + " and run_date='" + year + "-" + month + "-" + day_new + "' and branch_id=" + BranchId + " and device_id='" + DeviceId + "';";
                                }
                                startTime = DateTime.Now;
                                DataTable dt_select = sh.Get_Table_FromQry(Query_new);
                                endTime = DateTime.Now;
                                Console.WriteLine($"56.11-2253: {endTime - startTime}");
                                //checking the deviceid, branchid for a particuar date in a timesheet_branch_rerun table where active=1
                                //if record doesn't exist for that date then we insert into timesheet_branch_rerun
                                if (dt_select.Rows.Count == 0)
                                {
                                    qryinsertRerunStatus = "Insert into timesheet_branch_rerun([branch_id], [device_id], [run_date], [active], [rerun_status]) values (" + BranchId + ", '" + DeviceId + "', '" + year + "-" + month + "-" + day_new + "',1,'Complete')";
                                    //// _logger.Info("Select Query Executed is :" + qryinsertRerunStatus);
                                    startTime = DateTime.Now;
                                    bool b1 = sh.Run_UPDDEL_ExecuteNonQuery(qryinsertRerunStatus);
                                    endTime = DateTime.Now;
                                    Console.WriteLine($"56.12-2263: {endTime - startTime}");
                                    if (b1 == true)
                                    {
                                        //// _logger.Info("Records inserted sucessfully" + qryinsertRerunStatus);
                                        qryinsertRerunStatus = "";
                                    }
                                    else
                                    {
                                        //// _logger.Info("execution Failed" + qryinsertRerunStatus);
                                        qryinsertRerunStatus = "";
                                    }
                                }
                                else
                                {
                                    // if record for deviceid, branchid for a particuar date in a timesheet_branch_rerun table where active=1
                                    //already exists then do nothing
                                }

                            }

                        }
                        //ds_count++;
                    }

                }
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
            }
        }

        public void CheckBiometricLogsFor_1Month()
        {
            SqlHelper sh = new SqlHelper(_mavenConnStr);
            string timesheetconn = System.Configuration.ConfigurationManager.AppSettings.Get("timesheetdb");
            string QueryListDevices = "select distinct device_id from timesheet_branch_rerun where active=1";
            string QueryYearMonth = "";
            string DeviceId = "";
            string year = "";
            string month = "";
            string run_date = "";
            string CheckDeviceExists = "";

            DateTime Run_date1;
            DateTime Run_date2;
            DateTime firstDayofMonth;
            DateTime lastDayofMonth;
            DataTable dt_ListDevices = sh.Get_Table_FromQry(QueryListDevices);
            if (dt_ListDevices.Rows.Count > 0)
            {
                foreach (DataRow dr_Device in dt_ListDevices.Rows)
                {
                    DeviceId = dr_Device["device_id"].ToString();
                    QueryYearMonth = "select distinct year(run_date)as year,month(run_date)as month,branch_id from timesheet_branch_rerun where device_id='" + DeviceId + "'";
                    DataTable dt_YearMonth = sh.Get_Table_FromQry(QueryYearMonth);
                    if (dt_YearMonth.Rows.Count > 0)
                    {
                        foreach (DataRow dr_YearMonth in dt_YearMonth.Rows)
                        {
                            year = dr_YearMonth["year"].ToString();
                            month = dr_YearMonth["month"].ToString();
                            if (month.Length == 1)
                            {
                                month = "0" + month;
                            }
                            run_date = year + "-" + month + "-" + "01";
                            firstDayofMonth = Convert.ToDateTime(run_date);
                            lastDayofMonth = firstDayofMonth.AddMonths(1).AddDays(-1);

                            //ClintDB
                            string strquryAllBranch = "select device_id,io_time,update_time,user_id,verify_mode,io_mode,io_time from tbl_realtime_glog where device_id='" + DeviceId + "' and (io_time>= '" + Convert.ToDateTime(firstDayofMonth).ToString("yyyy-MM-dd HH:mm:ss") + "' and io_time  <= '" + lastDayofMonth.ToString("yyyy-MM-dd HH:mm:ss") + "'  or io_time >=  '" + Convert.ToDateTime(firstDayofMonth).ToString("yyyy-MM-dd HH:mm:ss") + "'  and io_time <= '" + lastDayofMonth.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            SqlHelper sss = new SqlHelper(timesheetconn);
                            DataTable BranchTimeSheetlogs = sss.Get_Table_FromQry(strquryAllBranch);
                            string inQry = "";
                            string PendingStatusUpdate = "";
                            string PendingStatusUpdatebranch = "";
                            if (BranchTimeSheetlogs.Rows.Count > 0)
                            {
                                foreach (DataRow dtBranchdata in BranchTimeSheetlogs.Rows)
                                {
                                    int userid = Convert.ToInt32(dtBranchdata["user_id"]);
                                    string deviceid = dtBranchdata["device_id"].ToString();
                                    DateTime io_time = Convert.ToDateTime(dtBranchdata["io_time"]);
                                    DateTime update_time = Convert.ToDateTime(dtBranchdata["update_time"]);
                                    int io_mode = Convert.ToInt32(dtBranchdata["io_mode"]);
                                    int verify_mode = Convert.ToInt32(dtBranchdata["verify_mode"]);

                                    // for updating the timesheet_rerun_status
                                    DateTime strdt = Convert.ToDateTime(io_time);
                                    string strdate = strdt.ToString("yyyy-MM-dd");
                                    string[] sa1 = strdate.Split('-');
                                    string dt1 = sa1[0];
                                    string dt2 = sa1[1];
                                    string dt3 = sa1[2];
                                    if (dt3.StartsWith("0"))
                                    {
                                        dt3 = dt3.Substring(1);
                                    }

                                    //checks whether record exists in timesheet_logs or not if doesnot exists then insert the record else do nothing.
                                    CheckDeviceExists = "Select device_id,io_time,update_time,user_id,verify_mode,io_mode,io_time from timesheet_logs where device_id='" + deviceid + "' and user_id=" + userid + " and io_time='" + io_time.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                    DataTable dt_check = sh.Get_Table_FromQry(CheckDeviceExists);
                                    if (dt_check.Rows.Count == 0)
                                    {
                                        inQry += "INSERT INTO timesheet_logs([user_id],[device_id],[update_time],[verify_mode],[io_mode],[io_time])"
                                        + "VALUES('" + dtBranchdata["user_id"] + "','"
                                        + dtBranchdata["device_id"] + "','"
                                        + Convert.ToDateTime(dtBranchdata["update_time"]).ToString("yyyy-MM-dd HH:mm:ss") + "','"
                                        + dtBranchdata["verify_mode"] + "','"
                                        + dtBranchdata["io_mode"] + "','"
                                        + Convert.ToDateTime(dtBranchdata["io_time"]).ToString("yyyy-MM-dd HH:mm:ss") + "'); ";

                                        PendingStatusUpdate += "Update timesheet_run_status set D" + dt3 + "=1 where device_id='" + DeviceId + "' and mn = " + dt2 + " and yr = " + dt1 + ";";
                                    }
                                    else
                                    {
                                        //// _logger.Info("*** Record exists for the Device Id "+ deviceid +" for Date "+io_time);
                                    }
                                }
                                if (inQry != "")
                                {
                                    try
                                    {
                                        sh.Run_UPDDEL_ExecuteNonQuery(inQry);
                                    }
                                    catch (Exception ex)
                                    {
                                        // // _logger.Error("** Something went wrong while saving to DB. Query: " + ex.ToString() + " , Query=" + inQry);
                                    }
                                }
                                if (PendingStatusUpdate != "")
                                {
                                    try
                                    {
                                        sh.Run_UPDDEL_ExecuteNonQuery(PendingStatusUpdate);
                                    }
                                    catch (Exception ex)
                                    {
                                        //  // _logger.Error("** Something went wrong while saving to DB. Query: " + ex.ToString() + " , Query=" + PendingStatusUpdate);
                                    }
                                }

                            }
                            PendingStatusUpdatebranch += "Update timesheet_branch_rerun set active=0 where year(run_date)=" + year + " and month(run_date)=" + month + " and device_id='" + DeviceId + "'";
                            if (PendingStatusUpdatebranch != "")
                            {
                                try
                                {
                                    sh.Run_UPDDEL_ExecuteNonQuery(PendingStatusUpdatebranch);
                                    PendingStatusUpdatebranch = "";
                                }
                                catch (Exception ex)
                                {
                                    //  // _logger.Error("** Something went wrong while saving to DB. Query: " + ex.ToString() + " , Query=" + PendingStatusUpdatebranch);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //// _logger.Info("***No active=1 records found in timesheet_branch_rerun table***");
            }
        }

        /*
         * LA-Late Arriaval
         * EA-Early Arrival
         * LD-late Departure
         * ED-Early Departure
         * PR-Present
         * AB-Absent
         * HD-Hald Day
         * NA-Not Applicable
         */
        enum EmpDayStatus
        {
            LA,
            EA,
            ED,
            PR,
            AB,
            NA,
            LD,
            HD
        }
    }
}
