using Entities;
using HRMSBusiness.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMSBusiness.Business
{
    public class LeavesBusiness
    {
        SqlHelper sh = new SqlHelper();
        public string AddPlbalanceDetails(string EmpCode, string CurrYear, string encashpl)
        {
            string fdate = "";
            DateTime fnDate;
            DateTime daterange;
            string qry = "select top 1 UpdatedDate  from PLE_Type where empid=(select id from employees where empid="+ EmpCode + ")  order by id desc";
            DataTable dt= sh.Get_Table_FromQry(qry);
            var encshdate = DateTime.Now.ToString("yyyy/MM/dd");

            DateTime enDate = Convert.ToDateTime(encshdate);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                  fdate = dr["UpdatedDate"].ToString();
                }
               
                
            }
            if (fdate != "")
            {
                fnDate = Convert.ToDateTime(fdate);
                daterange = Convert.ToDateTime(fdate);
                int years = daterange.Year;
                int totalDays = Convert.ToInt32((fnDate - enDate).TotalDays);

                int diff = System.Math.Abs(totalDays);

                if ((enDate.Year % 400) == 0 || (enDate.Year % 4) == 0)
                {
                    if (encashpl == "15" && diff < 364)
                    {
                        return "Already This Employee Has Got Encashment";
                    }
                    if (encashpl == "30" && diff < 729)
                    {
                        return "Only 15 days are eligible for Encashment";
                    }
                }
                else
                {
                    if (encashpl == "15" && diff < 365)
                    {
                        return "Already This Employee Has Got Encashment";
                    }
                    if (encashpl == "30" && diff < 730)
                    {
                        return "Only 15 days are eligible for Encashment";
                    }

                }
            }
            return null;
        }
        public DataTable SelfLeaveHistory(string empid)
        {
            string qry = " SELECT ep.EmpId,ep.ShortName, lleave.UpdatedDate,lleave.StartDate,lleave.EndDate,Lt.Code,lleave.LeaveDays,lleave.Subject,lleave.Reason,ds.Code as Designation,lleave.Status,"
                       + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                       + " FROM Leaves lleave " +
                       " join Employees ep on lleave.EmpId = ep.Id " +
                       " join Designations ds on ds.Id = ep.CurrentDesignation " +
                       " join Departments dp on dp.Id = lleave.DepartmentId " +
                       " join Branches br on br.Id = lleave.BranchId " +
                       " join LeaveTypes Lt on Lt.Id = lleave.LeaveType " +
                       " where ep.EmpId  = " + empid + " lleave.LeaveDays = " + 0 + " order by lleave.UpdatedDate desc";
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable SelfLeaveHistorypost(string StartDate, string EndDate, string leaveTypeId, string EmpPkId)
        {
            string qry = "";
            if (StartDate == "" || EndDate == "" || leaveTypeId == "")
            {
                qry = " SELECT ep.EmpId,ep.ShortName, lleave.UpdatedDate,lleave.StartDate,lleave.EndDate,Lt.Code,lleave.LeaveDays,lleave.Subject,lleave.Reason,ds.Code as Designation,lleave.Status,"
                         + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                         + " FROM Leaves lleave " +
                         " join Employees ep on lleave.EmpId = ep.Id " +
                         " join Designations ds on ds.Id = ep.CurrentDesignation " +
                         " join Departments dp on dp.Id = lleave.DepartmentId " +
                         " join Branches br on br.Id = lleave.BranchId " +
                         " join LeaveTypes Lt on Lt.Id = lleave.LeaveType " +
                         " where ep.Id  = " + EmpPkId + " lleave.LeaveDays = " + 0 + " order by lleave.UpdatedDate desc";

            }
            if (leaveTypeId == "11")
            {
                qry = " SELECT ep.EmpId,ep.ShortName, lleave.UpdatedDate,lleave.StartDate,lleave.EndDate,Lt.Code,lleave.LeaveDays,lleave.Subject,lleave.Reason,ds.Code as Designation,lleave.Status,"
                         + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                         + " FROM Leaves lleave " +
                         " join Employees ep on lleave.EmpId = ep.Id " +
                         " join Designations ds on ds.Id = ep.CurrentDesignation " +
                         " join Departments dp on dp.Id = lleave.DepartmentId " +
                         " join Branches br on br.Id = lleave.BranchId " +
                         " join LeaveTypes Lt on Lt.Id = lleave.LeaveType " +
                         " where ((lleave.StartDate >= " + StartDate + " and" +
                         " lleave.EndDate <= " + EndDate + ")" + "or" +
                         "( lleave.EndDate >=" + StartDate + " and" + "lleave.StartDate <= " +
                         EndDate + ")) " + " and" + "ep.Id =" + EmpPkId + " order by lleave.UpdatedDate desc";

            }
            return sh.Get_Table_FromQry(qry);

        }

        public DataTable getAuthorityNames(string empid)
        {
            string qry = " select e.ShortName,ControllingAuthorityID = (select  top 1 empid from employees where id = e.ControllingAuthority),SanctioningAuthorityID = (select  top 1 EMPID from employees where id = e.sanctioningauthority), ControllingCAName = (select  top 1 ShortName from employees where id = e.ControllingAuthority),SanctioningSAName = (select  top 1 shortname from employees where id = e.sanctioningauthority) "
                + " FROM Employees e " +
                " where empid =" + empid;
            return sh.Get_Table_FromQry(qry);
        }


        public List<LeaveTypes> getLeaveTypesSearch(string lcodes)
        {
            List<LeaveTypes> Leavelst = new List<LeaveTypes>();
            //if (lcodes == "Attender" || lcodes == "Driver" || lcodes == "SA" || lcodes == "Attender-Watchman" || lcodes == "Attender/J.C" || lcodes == "Watchman" || lcodes == "JR-SA" || lcodes == "SA-Assistant Cashier")
            //{
            //    var dt = sh.Get_Table_FromQry("select Id,Type,Code,Description from LeaveTypes where Type!='ALL' order by Type");
            //    Leavelst = dt.AsEnumerable().Select(r => new LeaveTypes
            //    {
            //        Id = (Int32)(r["Id"]),
            //        Type = (string)(r["Type"] ?? "null"),
            //        Description = (string)(r["Description"] ?? "null"),
            //        Code = (string)(r["Code"] ?? "null")
            //    }).ToList();
            //}
            //else
            //{
            var dt = sh.Get_Table_FromQry("select Id,Type,Code,Description from LeaveTypes where Code!='ALL' and Type!='Week-Off' order by Type");
            Leavelst = dt.AsEnumerable().Select(r => new LeaveTypes
            {
                Id = (Int32)(r["Id"]),
                Type = (string)(r["Type"] ?? "null"),
                Description = (string)(r["Description"] ?? "null"),
                Code = (string)(r["Code"] ?? "null")
            }).ToList();
            //}
            return Leavelst;
        }

        public List<LeaveTypes> getAllLeaveTypes(string designCode)
        {
            List<LeaveTypes> Leavelst = new List<LeaveTypes>();
            //if (designCode == "Attender" || designCode == "Driver" || designCode == "SA" || designCode == "Attender-Watchman" || designCode == "Attender/J.C" || designCode == "Watchman" || designCode == "JR-SA" || designCode == "SA-Assistant Cashier")
            //{
            //    var dt = sh.Get_Table_FromQry("select Id,Type,Code,Description from LeaveTypes  order by Type");
            //    Leavelst = dt.AsEnumerable().Select(r => new LeaveTypes
            //    {
            //        Id = (Int32)(r["Id"]),
            //        Type = (string)(r["Type"] ?? "null"),
            //        Description = (string)(r["Description"] ?? "null"),
            //        Code = (string)(r["Code"] ?? "null")
            //    }).ToList();
            //}
            //else
            //{
            var dt = sh.Get_Table_FromQry("select Id,Type,Code,Description from LeaveTypes where Code!='ALL' and Type!='Week-Off' order by Type");
            Leavelst = dt.AsEnumerable().Select(r => new LeaveTypes
            {
                Id = (Int32)(r["Id"]),
                Type = (string)(r["Type"] ?? "null"),
                Description = (string)(r["Description"] ?? "null"),
                Code = (string)(r["Code"] ?? "null")
            }).ToList();
            //}
            return Leavelst;
        }
        public DataTable getLeaveHistory(string empid)
        {
            string qry = "select e.Empid as EmpId,e.shortname as EmployeeName,d.name as designation,l.UpdatedDate as UpdatedDate,case when dp.Name != 'OtherDepartment' then dp.Name when b.Name != 'Otherbranch' then b.Name end as Deptbranch,l.StartDate as StartDates,l.EndDate as EndDates,lt.Code as Code,l.LeaveDays as diffdays,l.Subject as Subject,l.Reason as Reason,l.CancelReason,l.Status as Status" +
              " from leaves l join Employees e on e.id = l.EmpId" +
              " join leavetypes lt on l.LeaveType = lt.id " +
              " join Designations d on d.Id = l.DesignationId " +
              " join Departments dp on dp.Id = l.DepartmentId " +
              " join Branches b on b.Id = l.BranchId " +
              " where l.Empid = " + empid + " and l.Leavedays != 0 order by l.updateddate DESC ";
            return sh.Get_Table_FromQry(qry);
        }
        //ML condition 
        public string MLcondition(int empid, string appStDate, string appEndDate, string leavetype)
        {
            string Source = "";
            //Add one year to employee DoJ
            string qry1 = " SELECT DATEADD(year, 1, DOJ) AS DateAdd from employees where '" + empid + "';";

            DateTime star1 = DateTime.Parse(appStDate);
            DataTable dt = sh.Get_Table_FromQry(qry1);
            if (dt.Rows.Count > 0 && leavetype == "2")
            {
                Source = (string)dt.Rows[0]["DateAdd"].ToString();
                DateTime dateml = Convert.ToDateTime(Source).Date;

                if (dt.Rows.Count > 0 && leavetype == "2" && star1 <= dateml)
                {
                    Source = "No Medical Leave is eligible upto one year of joining date";
                }
            }

            return Source;

        }

        //HolidayCount cl,ml,pl combinations
        public string CLMLPLHolidayLeave(int empid, string appStDate, string appEndDate, string leavetype)
        {
            //Scenario 1  - cl, H1, H2, (apply pl, ml)
            string qry1 = "Select top 1 DATEDIFF(Day, EndDate+1, '" + appStDate + "') as LastLeaveDayDiff , EndDate LastLeaveDate,LeaveType from Leaves where LeaveType in (" + leavetype + " ) and Status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited') and EndDate<'" + appStDate + "' and empid='" + empid + "' order by EndDate desc; ";
            string qry2 = " select count(*) as HolidaysCount from(select distinct[Date] from HolidayList where Date > (select top 1 EndDate from Leaves where LeaveType in (" + leavetype + ")  and EndDate < '" + appStDate + "' and empid = '" + empid + "' order by EndDate desc) and Date< '" + appStDate + "') as x; ";

            //scenario 2 - (apply pl, ml), H1, H2, CL
            string qry3 = "select top 1 DATEDIFF(Day, '" + appEndDate + "',StartDate-1) as LatestLeaveDayDiff, StartDate LatestLeaveDate,LeaveType from Leaves where LeaveType in (" + leavetype + ") and Status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited') and StartDate>'" + appEndDate + "' and empid='" + empid + "' order by StartDate asc; ";
            string qry4 = "select count(*) as HolidaysCount from(select distinct[Date] from HolidayList where Date > '" + appEndDate + "' and Date < (select top 1 StartDate from Leaves where LeaveType  in (" + leavetype + ") and StartDate > '" + appEndDate + "'and empid = '" + empid + "' order by StartDate asc)) as x; ";


            DataSet ds1 = sh.Get_MultiTables_FromQry(qry1 + qry2 + qry3 + qry4);
            DataTable dt1 = ds1.Tables[0];
            DataTable dt2 = ds1.Tables[1];
            DataTable dt3 = ds1.Tables[2];
            DataTable dt4 = ds1.Tables[3];


            //Scenario 1  - cl, H1, H2, (apply pl, ml)
            if (dt1.Rows.Count > 0 && dt2.Rows.Count > 0)
            {
                int lastLeaveDayDiff = int.Parse(dt1.Rows[0]["LastLeaveDayDiff"].ToString());
                int holidaysCount = int.Parse(dt2.Rows[0]["HolidaysCount"].ToString());
                if (lastLeaveDayDiff == holidaysCount)
                {
                    string cd = "CL";
                    if (dt1.Rows[0]["LeaveType"].ToString() == "2")
                    {
                        cd = "ML";
                    }
                    else if (dt1.Rows[0]["LeaveType"].ToString() == "3")
                    {
                        cd = "PL";
                    }
                    return "E#There is " + cd + " applied on " + Convert.ToDateTime(dt1.Rows[0]["LastLeaveDate"]).ToString("dd-MM-yyyy");
                }
            }

            //scenario 2 - (apply pl, ml), H1, H2, CL
            if (dt3.Rows.Count > 0 && dt4.Rows.Count > 0)
            {
                int latestLeaveDayDiff = int.Parse(dt3.Rows[0]["LatestLeaveDayDiff"].ToString());
                int holidaysCount = int.Parse(dt4.Rows[0]["HolidaysCount"].ToString());
                if (latestLeaveDayDiff == holidaysCount)
                {
                    string cd = "CL";
                    if (dt3.Rows[0]["LeaveType"].ToString() == "2")
                    {
                        cd = "ML";
                    }
                    else if (dt3.Rows[0]["LeaveType"].ToString() == "3")
                    {
                        cd = "PL";
                    }
                    return "E#There is " + cd + " applied on " + Convert.ToDateTime(dt3.Rows[0]["LatestLeaveDate"]).ToString("dd-MM-yyyy");
                }
            }

            return "S#OK";
        }
        //Morethan 10 days cl conditions
        public string CL10DaysCheck(int empid, string appStDate, string appEndDate, int leavetype)
        {
            //Scenario 1  -prev.leave and current cl
            string qry1 = "Select top 1 Startdate,EndDate from Leaves where LeaveType=" + leavetype + " and Status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited') and StartDate<'" + appStDate + "' and empid='" + empid + "' order by StartDate desc; ";
            string qry2 = "select count(*) as HolidaysCount from (select distinct [Date] from HolidayList where  Date>(select top 1 StartDate from Leaves where LeaveType =" + leavetype + " and StartDate< '" + appStDate + "' and empid='" + empid + "' order by StartDate desc) and Date< '" + appStDate + "') as x; ";

            //scenario 2 - current cl and next.leave
            string qry3 = "select top 1 Startdate,EndDate from Leaves where LeaveType=" + leavetype + " and Status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited') and EndDate>'" + appEndDate + "' and empid='" + empid + "' order by EndDate desc; ";
            string qry4 = "select count(*) as HolidaysCount from (select distinct [Date] from HolidayList where Date>'" + appEndDate + "' and Date<(select top 1 EndDate from Leaves where LeaveType=" + leavetype + " and EndDate>'" + appEndDate + "'and empid='" + empid + "' order by EndDate desc)) as x; ";

            DataSet ds1 = sh.Get_MultiTables_FromQry(qry1 + qry2 + qry3 + qry4);
            DataTable dt1 = ds1.Tables[0];
            DataTable dt2 = ds1.Tables[1];
            DataTable dt3 = ds1.Tables[2];
            DataTable dt4 = ds1.Tables[3];

            DateTime appst = DateTime.Parse(appStDate);
            DateTime apped = DateTime.Parse(appEndDate);

            #region 7dayslogic
            if (dt1.Rows.Count > 0 && dt2.Rows.Count > 0)
            {
                DateTime dtst = DateTime.Parse(dt1.Rows[0]["StartDate"].ToString());
                DateTime dted = DateTime.Parse(dt1.Rows[0]["EndDate"].ToString());

                double stdtTOstdtDiff = (appst - dtst).TotalDays + 1;
                double stdtTOeddtDiff = (dted - dtst).TotalDays + 1;
                int holidaysCount = int.Parse(dt2.Rows[0]["HolidaysCount"].ToString());

                //consecutive leave including holidays
                if (stdtTOstdtDiff == (holidaysCount + stdtTOeddtDiff + 1))
                {
                    double appStToappEdDiff = (apped - appst).TotalDays + 1;
                    if ((stdtTOeddtDiff + appStToappEdDiff) > 7)
                    {
                        return "E#More than 7 Consecutive CLs are not allowed.";
                    }
                }
            }

            if (dt3.Rows.Count > 0 && dt4.Rows.Count > 0)
            {
                DateTime dtst = DateTime.Parse(dt3.Rows[0]["StartDate"].ToString());
                DateTime dted = DateTime.Parse(dt3.Rows[0]["EndDate"].ToString());
                //diff between current leave eddt to next.leave eddt
                double eddtTOeddtDiff = (dted - apped).TotalDays + 1;
                double stdtTOeddtDiff = (dted - dtst).TotalDays + 1;
                int holidaysCount = int.Parse(dt4.Rows[0]["HolidaysCount"].ToString());
                //consecutive or not
                if (eddtTOeddtDiff == (holidaysCount + stdtTOeddtDiff + 1))
                {
                    double appStToappEdDiff = (apped - appst).TotalDays + 1;
                    if ((stdtTOeddtDiff + appStToappEdDiff) > 7)
                    {
                        return "E#More than 7 Consecutive CLs are not allowed.";
                    }
                }
            }
            #endregion

            #region 10days logic
            //Scenario 1  - Pl/ml, H1, H2, (apply pl, ml)
            if (dt1.Rows.Count > 0 && dt2.Rows.Count > 0)
            {
                DateTime dtst = DateTime.Parse(dt1.Rows[0]["StartDate"].ToString());
                DateTime dted = DateTime.Parse(dt1.Rows[0]["EndDate"].ToString());

                //diff between current leave stdt to prev.leave stdt
                double stdtTOstdtDiff = (appst - dtst).TotalDays + 1;
                double stdtTOeddtDiff = (dted - dtst).TotalDays + 1;
                int holidaysCount = int.Parse(dt2.Rows[0]["HolidaysCount"].ToString());

                double appStToappEdDiff = (apped - appst).TotalDays + 1;
                if (stdtTOstdtDiff == (holidaysCount + stdtTOeddtDiff + 1))
                {

                    if ((stdtTOeddtDiff + appStToappEdDiff + holidaysCount) > 10)
                    {
                        return "E#More than 10 CLs are not allowed.";
                    }
                }


            }

            //scenario 2 - (apply pl, ml), H1, H2, pl/ml
            if (dt3.Rows.Count > 0 && dt4.Rows.Count > 0)
            {
                DateTime dtst = DateTime.Parse(dt3.Rows[0]["StartDate"].ToString());
                DateTime dted = DateTime.Parse(dt3.Rows[0]["EndDate"].ToString());

                //diff between current leave eddt to next.leave eddt
                double eddtTOeddtDiff = (dted - apped).TotalDays + 1;
                double stdtTOeddtDiff = (dted - dtst).TotalDays + 1;

                int holidaysCount = int.Parse(dt4.Rows[0]["HolidaysCount"].ToString());

                double appStToappEdDiff = (apped - appst).TotalDays + 1;

                if (eddtTOeddtDiff == (holidaysCount + stdtTOeddtDiff + 1))
                {

                    if ((stdtTOeddtDiff + appStToappEdDiff + holidaysCount) > 10)
                    {
                        return "E#More than 10 CLs are not allowed.";
                    }
                }
            }
            #endregion

            return "S#OK";
        }

        //Add holidays if PL/ML HL HL PL/ML
        public string PLMLAddHolidays(int empid, string appStDate, string appEndDate, int leavetype)
        {
            //Scenario 1  - cl, H1, H2, (apply pl, ml)
            string qry1 = "Select top 1 DATEDIFF(Day, EndDate+1, '" + appStDate + "') as LastLeaveDayDiff , EndDate LastLeaveDate from Leaves where LeaveType in(2,3,12 ) and Status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited') and EndDate<'" + appStDate + "' and empid='" + empid + "' order by EndDate desc; ";
            string qry2 = "select count(*) as HolidaysCount from (select distinct [Date] from HolidayList where  Date>(select top 1 EndDate from Leaves where LeaveType in(2,3,12 ) and EndDate< '" + appStDate + "' and empid='" + empid + "' order by EndDate desc) and Date< '" + appStDate + "') as x; ";

            //scenario 2 - (apply pl, ml), H1, H2, CL
            string qry3 = "select top 1 DATEDIFF(Day, '" + appEndDate + "',StartDate-1) as LatestLeaveDayDiff, StartDate LatestLeaveDate from Leaves where LeaveType in(2,3,12 ) and Status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited') and StartDate>'" + appEndDate + "' and empid='" + empid + "' order by StartDate asc; ";
            string qry4 = "select count(*) as HolidaysCount from (select distinct [Date] from HolidayList where Date>'" + appEndDate + "' and Date<(select top 1 StartDate from Leaves where LeaveType in(2,3,12 ) and StartDate>'" + appEndDate + "'and empid='" + empid + "' order by StartDate asc)) as x; ";

            DataSet ds1 = sh.Get_MultiTables_FromQry(qry1 + qry2 + qry3 + qry4);
            DataTable dt1 = ds1.Tables[0];
            DataTable dt2 = ds1.Tables[1];
            DataTable dt3 = ds1.Tables[2];
            DataTable dt4 = ds1.Tables[3];
            if(dt1.Rows.Count > 0 && dt2.Rows.Count > 0 && dt3.Rows.Count > 0 && dt4.Rows.Count > 0)
            {
                int lastLeaveDayDiff = int.Parse(dt1.Rows[0]["LastLeaveDayDiff"].ToString());
                int holidaysCount = int.Parse(dt2.Rows[0]["HolidaysCount"].ToString());
                int latestLeaveDayDiff = int.Parse(dt3.Rows[0]["LatestLeaveDayDiff"].ToString());
                int holidaysCount1 = int.Parse(dt4.Rows[0]["HolidaysCount"].ToString());
                string holidaysCounts = Convert.ToString(holidaysCount);
                string holidaysCounts1 = Convert.ToString(holidaysCount1);
                if (lastLeaveDayDiff == holidaysCount && latestLeaveDayDiff == holidaysCount1)
                {
                    return "ASS#"+ holidaysCount+"#"+ holidaysCount1 ;
                }
            }

            //Scenario 1  - Pl/ml, H1, H2, (apply pl, ml)
            if (dt1.Rows.Count > 0 && dt2.Rows.Count > 0)
            {
                int lastLeaveDayDiff = int.Parse(dt1.Rows[0]["LastLeaveDayDiff"].ToString());
                int holidaysCount = int.Parse(dt2.Rows[0]["HolidaysCount"].ToString());
                if (lastLeaveDayDiff == holidaysCount)
                {
                    return "AS#" + holidaysCount;
                }
            }

            //scenario 2 - (apply pl, ml), H1, H2, pl/ml
            if (dt3.Rows.Count > 0 && dt4.Rows.Count > 0)
            {
                int latestLeaveDayDiff = int.Parse(dt3.Rows[0]["LatestLeaveDayDiff"].ToString());
                int holidaysCount = int.Parse(dt4.Rows[0]["HolidaysCount"].ToString());
                if (latestLeaveDayDiff == holidaysCount)
                {
                    return "AE#" + holidaysCount;
                }
            }

            return "S#OK";
        }

        //checking cl ,pl,ml combinations
        public string CLMLPLCombinations(int empid, string StartDate, string EndDate, int leavetype)
        {

            string qry = " select 'Leave' as 'Source', empid,StartDate,EndDate,LeaveType,status "
                + " FROM Leaves where empid = " + empid + "  and((convert(date,StartDate) >=  '" + StartDate + "' and convert(date,EndDate) <= '" + EndDate + "') or(convert(date, EndDate) >=  '" + StartDate + "' and convert(date, StartDate) <= '" + EndDate + "')) and status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited')";
            DataTable dt = sh.Get_Table_FromQry(qry);
            string Source = "";
            if (dt.Rows.Count > 0)
            {
                Source = Convert.ToString(dt.Rows[0]["LeaveType"]);
                return Source;
            }
            return Source;
        }

        public string getcheckLTCWDOD(string empid, string Empcode, string StartDate, string EndDate, string status)
        {
            string Source = "";
            var qry = "select 'Leave' as 'Source', empid,StartDate,EndDate,status "
                + " FROM Leaves where empid = " + empid + "  and((convert(date,StartDate) >=  '" + StartDate + "' and convert(date,EndDate) <= '" + EndDate + "') or(convert(date, EndDate) >=  '" + StartDate + "' and convert(date, StartDate) <= '" + EndDate + "'))  and status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited')"
                + " union select 'OD' as 'Source', empid,StartDate,EndDate,status "
                + " FROM OD_OtherDuty where empid = " + empid + " and((convert(date,StartDate) >=  '" + StartDate + "' and convert(date,EndDate) <= '" + EndDate + "') or(convert(date, EndDate) >=  '" + StartDate + "' and convert(date, StartDate) <= '" + EndDate + "')) and status not in ('Cancelled', 'PartialCancelled', 'Denied')"
                + " union select 'LTC' as 'Source', empid,StartDate,EndDate,Status COLLATE SQL_Latin1_General_CP1_CI_AS as Status"
                + " FROM Leaves_LTC where empid = " + empid + " and((convert(date,StartDate) >=  '" + StartDate + "' and convert(date,EndDate) <= '" + EndDate + "') or(convert(date, EndDate) >=  '" + StartDate + "' and convert(date, StartDate) <= '" + EndDate + "')) and status not in ('Cancelled', 'PartialCancelled', 'Denied')"
                //+ " union select 'TimeSheet' as 'Source',UserId,ReqDate,UpdatedDate,status"
                //   + " FROM Timesheet_Request_Form where UserId = " + empid + " and((convert(date,ReqDate) >=  '" + StartDate + "' and convert(date,ReqDate) <= '" + EndDate + "') or(convert(date, ReqDate) >=  '" + StartDate + "' and convert(date, ReqDate) <= '" + EndDate + "')) and status not in ('Cancelled','Denied')"
                + " union select 'WD' as 'Source', empid,WDDate,UpdatedDate,Status COLLATE SQL_Latin1_General_CP1_CI_AS as Status"
               + " FROM WorkDiary where empid = " + Empcode + " and(WDDate >=  '" + StartDate + "' AND WDDate <= '" + EndDate + "') and status not in ('Draft', 'PartialCancelled')";
            DataTable dt = sh.Get_Table_FromQry(qry);
            if (dt.Rows.Count > 0)
            {
                Source = (string)dt.Rows[0]["Source"].ToString();
            }
            return Source;


        }

        public string getapiholiday(int empid, int Empcode, string StartDate, string EndDate)
        {
            string Source = "";
            var qry = "select 'Holidays' as 'Source' ,date from HolidayList where date='" + StartDate + "' or date='" + EndDate + "'";
            DataTable dt = sh.Get_Table_FromQry(qry);
            if (dt.Rows.Count > 0)
            {
                Source = (string)dt.Rows[0]["Source"].ToString();
            }
            return Source;


        }
        //Mobile API Date Validation
        public string getapicheckLTCWDOD(int empid, int Empcode, string StartDate, string EndDate)
        {
            string Source = "";
            var qry = "select 'Leave' as 'Source', empid,StartDate,EndDate,status "
              + " FROM Leaves where empid = " + empid + "  and((convert(date,StartDate) >=  '" + StartDate + "' and convert(date,EndDate) <= '" + EndDate + "') or(convert(date, EndDate) >=  '" + StartDate + "' and convert(date, StartDate) <= '" + EndDate + "'))  and status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited')"
              + " union select 'OD' as 'Source', empid,StartDate,EndDate,status "
              + " FROM OD_OtherDuty where empid = " + empid + " and((convert(date,StartDate) >=  '" + StartDate + "' and convert(date,EndDate) <= '" + EndDate + "') or(convert(date, EndDate) >=  '" + StartDate + "' and convert(date, StartDate) <= '" + EndDate + "')) and status not in ('Cancelled', 'PartialCancelled', 'Denied')"
              + " union select 'LTC' as 'Source', empid,StartDate,EndDate,status"
              + " FROM Leaves_LTC where empid = " + empid + " and((convert(date,StartDate) >=  '" + StartDate + "' and convert(date,EndDate) <= '" + EndDate + "') or(convert(date, EndDate) >=  '" + StartDate + "' and convert(date, StartDate) <= '" + EndDate + "')) and status not in ('Cancelled', 'PartialCancelled', 'Denied')"
              //+ " union select 'TimeSheet' as 'Source',UserId,ReqDate,UpdatedDate,status"
              //   + " FROM Timesheet_Request_Form where UserId = " + empid + " and((convert(date,ReqDate) >=  '" + StartDate + "' and convert(date,ReqDate) <= '" + EndDate + "') or(convert(date, ReqDate) >=  '" + StartDate + "' and convert(date, ReqDate) <= '" + EndDate + "')) and status not in ('Cancelled','Denied')"
              + " union select 'WD' as 'Source', empid,WDDate,UpdatedDate,status"
             + " FROM WorkDiary where empid = " + Empcode + " and(WDDate >=  '" + StartDate + "' AND WDDate <= '" + EndDate + "') and status not in ('Draft', 'PartialCancelled')";
            DataTable dt = sh.Get_Table_FromQry(qry);
            if (dt.Rows.Count > 0)
            {
                Source = (string)dt.Rows[0]["Source"].ToString();
            }
            return Source;


        }

        //TimeSheet Date Validation.
        public string getTimesheetcheckLTCLeave(int empid, int Empcode, string StartDate, string EndDate)
        {
            string Source = "";
            var qry = "select 'Leave' as 'Source', empid,StartDate,EndDate,status "
                + " FROM Leaves where empid = " + empid + "  and((convert(date,StartDate) >=  '" + StartDate + "' and convert(date,EndDate) <= '" + EndDate + "') or(convert(date, EndDate) >=  '" + StartDate + "' and convert(date, StartDate) <= '" + EndDate + "'))  and status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited')"

                + " union select 'LTC' as 'Source', empid,StartDate,EndDate,status"
                + " FROM Leaves_LTC where empid = " + empid + " and((convert(date,StartDate) >=  '" + StartDate + "' and convert(date,EndDate) <= '" + EndDate + "') or(convert(date, EndDate) >=  '" + StartDate + "' and convert(date, StartDate) <= '" + EndDate + "')) and status not in ('Cancelled', 'PartialCancelled', 'Denied')";

            DataTable dt = sh.Get_Table_FromQry(qry);
            if (dt.Rows.Count > 0)
            {
                Source = (string)dt.Rows[0]["Source"].ToString();
            }
            return Source;


        }
        //   public string partialcancellation(string empid, string StartDate, string EndDate)
        //   {
        //       string qry = " select Id,EmpId,dateadd(d, v.number, d.StartDate) startdate,d.EndDate,d.status" +
        //" from leaves d join master..spt_values v on v.type = 'P'" +
        // " Where v.number between 0 and datediff(d, StartDate, EndDate)" +
        // " and d.status in('Approved', 'Pending', 'PartialCancelled', 'Forwarded') and empid = " + empid + "";


        //       sh.Get_Table_FromQry(qry);
        //       return " ";
        //   }

        // LTC Report  Index

        public DataTable LTCReport()
        {

            string qry = "select CAST(ROW_NUMBER() over(order by e.Empid) AS INT) SNO," +
                "e.EmpId,e.Id as EmpCode,e.ShortName as EmployeeName,CONVERT(VARCHAR, lt.UpdatedDate, 103) " +
                "as AppliedDate,d.code as Designations,case when b.id = 43 then dp.name else b.name end" +
                " as BranchDepartmet,lt.LTCType,bp.Block_Period,CONVERT(VARCHAR, lt.StartDate, 103) " +
                "as FromDate,CONVERT(VARCHAR, lt.EndDate, 103) as ToDate,lt.PlaceOfVisits,lt.TravelAdvance,lt.ModeOfTransport,lt.Status from   employees e  join designations d " +
                "on e.CurrentDesignation = d.id join departments dp on e.department = dp.id join branches b on e.branch = b.id join   " +
                "Leaves_LTC lt on lt.empid = e.id join BlockPeriod bp on lt.Block_Period = bp.id " +
                "where e.RetirementDate >= getdate()";

            return sh.Get_Table_FromQry(qry);
        }

    }
}
