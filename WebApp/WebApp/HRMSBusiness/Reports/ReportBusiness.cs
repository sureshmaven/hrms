using Entities;
using HRMSBusiness.Db;
using HRMSBusiness.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HRMSBusiness.Reports
{
    public class ReportBusiness
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ReportBusiness));
        SqlHelper sh = new SqlHelper();
        public List<Designations> getAllDesignations()
        {
            var dt = sh.Get_Table_FromQry("Select  Id, Code, Name, Description from Designations");
            List<Designations> desilst = dt.AsEnumerable().Select(r => new Designations
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null"),
                Description = (string)(r["Description"] ?? "null"),
                Code = (string)(r["Code"] ?? "null")
            }).ToList();

            return desilst;
        }
        public DataTable getAllDesignationsDt()
        {
            var dt = sh.Get_Table_FromQry("Select top 3 Id, Code, Name, Description from Designations");
            return dt;
        }
        public DataTable Brancheslist(int brid, string empid)
        {
            string qry = "Select e.EmpId,e.ShortName, case when b.Name='OtherBranch' then dept.Name else b.Name end  as BranchName,d.Name as DesignationName ,d.Code as desg"
                 + " FROM Employees e " +
                 " join Branches b on e.Branch = b.Id" +
                 " join Designations d on e.CurrentDesignation = d.Id" +
                 " join Departments dept on e.Department=dept.Id " +
                 " where  e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and e.Empid != 123456";

            // 0 for all rows, -1 for no rows
            if (brid != 0 && brid == 42 && empid != "")
            {
                qry += "And EmpId=" + empid + " And b.name = 'OtherBranch' " + " order by BranchName";
            }
            else if (brid != 0 && brid != 42 && empid != "")
            {
                qry += "And EmpId=" + empid + " AND e.Branch=" + brid + " And b.name != 'OtherBranch' " + " order by BranchName";
            }
            if (brid != 0 && brid == 42 && empid == "")
            {
                qry += " And b.name = 'OtherBranch' " + " order by BranchName";
            }
            else if (brid != 0 && brid != 42 && empid == "")
            {
                qry += " AND e.Branch=" + brid + " And b.name != 'OtherBranch' " + " order by BranchName";
            }
            else if (brid == 0 && empid != "")
            {
                qry += "And EmpId=" + empid;
            }
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getAllBranches(int brid)
        {
            string qry = "Select e.EmpId,e.ShortName, case when b.Name='OtherBranch' then dept.Name else b.Name end  as BranchName,d.Name as DesignationName"
                 + " FROM Employees e " +
                 " join Branches b on e.Branch = b.Id" +
                 " join Designations d on e.CurrentDesignation = d.Id" +
                 " join Departments dept on e.Department=dept.Id " +
                 " where  e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and e.Empid != 123456";

            // 0 for all rows, -1 for no rows
            if (brid != 0 && brid == 42)
            {
                qry += "  And b.name = 'OtherBranch' " + " order by BranchName";
            }
            else if (brid != 0 && brid != 42)
            {
                qry += " AND e.Branch=" + brid + " And b.name != 'OtherBranch' " + " order by BranchName";
            }
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getBranchContactsList()
        {
            string qry = "SELECT b.Name AS BranchName,e.empid as EmpId,e.shortname AS EmpName,d.code AS Code,b.PhoneNo1,b.PhoneNo2,(b.StartTime + ' ' + b.EndTime) AS Time FROM employees e JOIN branches b ON e.branch = b.id  JOIN designations d ON d.id = e.currentdesignation WHERE e.retirementdate >= GETDATE() AND b.name != 'OtherBranch'  and controllingauthority  in (216,211); ";
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getMissingTimesheetList(int branchid, string Date)
        {
            SqlConnection con = new SqlConnection("Server=mavensoftserver; Database=Hrms_021224_after; user id=sa; password=Maven@12345;");
            //SqlConnection con = new SqlConnection("Server=HRMS; Database=hrmspayroll; user id=sa; password=tscab@123;");
            using (con)
            {
                //if (branchid == 0)
                //{
                //    using (SqlCommand cmd = new SqlCommand("SELECT emp.Empid,emp.ShortName,case when br.Name='OtherBranch' then dept.Name else br.Name end as name,case when((select distinct count(*) from leaves l where '" + Date + "' between l.startDate and l.enddate and l.empid = emp.Id) > 0) then 'Leave' else 'Not Attended'  end as Status,FORMAT(CAST('" + Date + "' AS date),'yyyy/MM/dd') AS Date,case when emp.shift_id=0 then 'Shift' else '' end as Shift FROM [dbo].employees emp join branches br on br.id=emp.Branch join departments dept on dept.id=emp.department where emp.retirementdate>getdate() and emp. empid not in(select user_id  from [dbo].timesheet_logs as tb2 where cast(io_time as date)='" + Date + "') "))
                //    {
                //        using (SqlDataAdapter sda = new SqlDataAdapter())
                //        {
                //            cmd.Connection = con;
                //            sda.SelectCommand = cmd;
                //            using (DataTable dt = new DataTable())
                //            {
                //                try
                //                {
                //                    sda.Fill(dt);
                //                }
                //                catch (Exception ex)
                //                {
                //                   Log.Debug("catch block" + ex);
                //                }

                //            return dt;
                //            }

                //        }
                //    }
                //}
                if (branchid == 0)
                {
                    using (SqlCommand cmd = new SqlCommand("select Empid, ShortName, 'Dccb_Employee' as name,case when((select distinct count(*) from timesheet_logs l" +
                        " where '" + Date + "' between l.io_time and l.io_time and l.user_id = de.empid) > 0) then 'Leave' else 'Not Attended'  end as Status," +
                        "FORMAT(CAST('" + Date + "' AS date), 'yyyy/MM/dd') AS Date,'' as Shift from dccb_employees de where empid != 123456 and empid not in " +
                        "(select user_id from timesheet_logs where cast(io_time as date) = '" + Date + "')"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            using (DataTable dt = new DataTable())
                            {
                                try
                                {
                                    sda.Fill(dt);
                                }
                                catch (Exception ex)
                                {
                                    Log.Debug("catch block" + ex);
                                }

                                return dt;
                            }

                        }
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT emp.Empid,emp.ShortName,case when br.Name='OtherBranch' then dept.Name else br.Name end as name," +
                        "case when((select distinct count(*) from leaves l where '" + Date + "' between l.startDate and l.enddate and l.empid = emp.Id) > 0) then 'Leave' " +
                        "else 'Not Attended'  end as Status,FORMAT(CAST('" + Date + "' AS date),'yyyy/MM/dd') AS Date,case when emp.shift_id=0 then 'Shift' else '' " +
                        "end as Shift FROM [dbo].employees emp join branches br on br.id=emp.Branch join departments dept on dept.id=emp.department where " +
                        "emp.retirementdate>getdate() and empid != 123456 and emp. empid not in(select user_id  from [dbo].timesheet_logs as tb2 where cast(io_time as date)='" + Date + "') " +
                        "and emp.branch='" + branchid + "' order by status desc"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                return dt;
                            }

                        }
                    }
                }
            }
        }

        public DataTable getAllWorkDairies()
        {
            string query = "Select w.EmpId, emp.Shortname as Name, d.Code as Designation,w.WDDate as WorkDate,e.Name as WorkName,e.[Desc] as WorkDescription,w.Status as Status,"
                 + " case when b.Name='OtherBranch' then dept.Name else b.Name end as BrDepot "
                            + " FROM WorkDiary w " +
                              "join WorkDiary_Det e on w.Id = e.WDId" +
                              " join Employees emp on w.EmpId = emp.EmpId" +
                              " join Designations d on emp.CurrentDesignation = d.ID" +
                              " join Branches b on emp.Branch = b.Id" +
                              " join Departments dept on emp.Department = dept.Id" +
                              " Where w.Status!='Draft' and empid != 123456 ";
            return sh.Get_Table_FromQry(query);
        }


        public DataTable allWDNew(string wd, string fromdt, string todt, string employeecode, string status)
        {

            DateTime fdate1 = Convert.ToDateTime(fromdt);
            DateTime tdate1 = Convert.ToDateTime(todt);

            string fdate = fdate1.ToString("yyyy-MM-dd");
            string tdate = tdate1.ToString("yyyy-MM-dd");


            //only emp data
            string qry1 = " Select convert(varchar,w.EmpId) as EmpId , emp.Shortname, d.Code , case when b.Name='OtherBranch' then dept.Name when b.Name='HeadOffice' then dept.Name else b.Name end as branch," +
                "case when b.Name = 'OtherBranch' then dept.Name else b.Name end as BrDepot" +
                " FROM WorkDiary w join WorkDiary_Det e on w.Id = e.WDId " +
                " join Employees emp on w.EmpId = emp.EmpId " +
                " join Designations d on emp.currentdesignation = d.ID join Branches b on emp.branch = b.Id join Departments dept on emp.department = dept.Id " +
                " Where w.Status != 'Draft' and w.Status in ('Pending','Approved') and empid != 123456  and  emp.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and CurrentDesignation in (8, 7, 6, 5) and  (w.WDDate >= '" + fdate + "' and w.WDDate <='" + tdate + "') "
                + (employeecode != "" ? "AND w.EmpId=" + employeecode : "")
                + " group by convert(varchar, w.EmpId), emp.Shortname , d.Code , case when b.Name = 'OtherBranch' then dept.Name when b.Name = 'HeadOffice' then dept.Name else b.Name end,case when b.Name = 'OtherBranch' then dept.Name else b.Name end; ";

            //holidays
            string qry2 = " Select FORMAT([Date],'dd/MM/yyyy')as WorkDate, 'Holiday :' + Occasion as WorkDescription from HolidayList " +
                            "where [Date]>='" + fdate + "' and [Date]<='" + tdate + "' " +
                          " order by WorkDate desc;";

            ////workdiary + emp data
            //string qry3 = "  SELECT DISTINCT wd.EmpId, ep.ShortName,ds.Code as Designation,wd.WDDate,case when ep.branch = 43 then dp.Name when ep.branch = 46 then dp.Name else br.Name end as branch,FORMAT(wd.WDDate,'dd/MM/yyyy') as WorkDate," +
            //    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription, " +
            //    "wd.Status, case when br.Name = 'OtherBranch' then dp.Name else br.Name end as BrDepot,FORMAT(wd.UpdatedDate, 'd/MM/yyyy') as AppliedDate FROM WorkDiary wd join Employees ep on wd.EmpId = ep.EmpId join Designations ds " +
            //    " on ds.Id = wd.CurDesig join Departments dp on dp.Id = wd.CurDept join Branches br on br.Id = wd.CurBr join workdiary_det workdet on wd.Id = workdet.WDId where wd.status in('Pending', 'Approved') and(workdet.Name not in ('Leave', 'OD', 'LTC') or workdet.Name is null) " +
            //    "and CurrentDesignation in (8, 7, 6, 5)  and  ep.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + (employeecode != "" ? "AND w.EmpId=" + employeecode : "") + " and (wd.WDDate>='" + fdate + "' and wd.WDDate<='" + tdate + "') order by " +
            //    "CONVERT(DateTime, wd.WDDate, 101)  asc  ";

            //string notapplied = "SELECT convert(varchar,e.EmpId) as empid , e.Shortname, d.Code as designation,case when e.branch = 43 then dept.Name when e.branch = 46 then dept.Name else b.Name end as branch,bc.mydate as WorkDate," +
            //        "case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then 'Leave' else '' end + case when ((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct case when l1.LeaveType= 0 then '' else lt.Code end from leaves l1 join LeaveTypes lt on l1.leaveType=lt.Id where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='Cancelled' and l1.status!='Denied')" +
            //        " else ' ' end as WorkDescription, case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct l1.status from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='Cancelled' and l1.status!='Denied') else 'Not Applied' end as Status," +
            //        " case when e.branch = 43 then dept.Name else b.Name end as BrDepot,case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct FORMAT(l1.UpdatedDate,'dd/MM/yyyy') from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='Cancelled' and l1.status!='Denied') else '' end  as AppliedDate " +
            //        " FROM calender bc CROSS JOIN Employees e join Designations d on e.CurrentDesignation = d.ID join Branches b on e.branch = b.Id join Departments dept on e.department = dept.Id " +
            //        "WHERE NOT EXISTS(SELECT * FROM WorkDiary a WHERE a.empid = e.empid AND(DATEADD(day,DATEDIFF(day, 0, a.wddate), 0) = bc.mydate OR DATEADD(day, DATEDIFF(day, 0, a.wddate), 0) = bc.mydate)) and " +
            //        "mydate between '" + fdate + "' and '" + tdate + "' and retirementdate> '" + fdate + "' and e.empid not in (1, 123456) and CurrentDesignation in (8, 7, 6, 5) and bc.mydate not in (select date from holidaylist) " +
            //        " order by e.empid,bc.mydate ";

            string qry3 = " select x.EmpId,x.ShortName,x.Designation,x.WorkDate,x.AppliedDate,x.branch,x.branch,x.BrDepot,x.WorkDescription,x.Status from " +
                "( SELECT DISTINCT wd.EmpId, ep.ShortName,ds.Code as Designation,case when ep.branch = 43 then dp.Name when ep.branch = 46 then dp.Name else br.Name end as branch,FORMAT(wd.WDDate, 'dd/MM/yyyy') as WorkDate,concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription, wd.Status, case when br.Name = 'OtherBranch' then dp.Name else br.Name end as BrDepot,FORMAT(wd.UpdatedDate, 'd/MM/yyyy') as AppliedDate FROM WorkDiary wd join Employees ep on wd.EmpId = ep.EmpId join Designations ds on ds.Id = ep.currentdesignation join Departments dp on dp.Id =ep.department join Branches br on br.Id = ep.branch join workdiary_det workdet on wd.Id = workdet.WDId where wd.status in('Pending', 'Approved') and(workdet.Name not in ('Leave', 'OD', 'LTC') or workdet.Name is null) and CurrentDesignation in (8, 7, 6, 5)  and ep.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and(wd.WDDate >= '" + fdate + "' and wd.WDDate <= '" + tdate + "')  " +
                " union all  " +
                " SELECT convert(varchar, e.EmpId) as empid , e.Shortname, d.Code as designation,case when e.branch = 43 then dept.Name when e.branch = 46 then dept.Name else b.Name end as branch,FORMAT(bc.mydate, 'dd/MM/yyyy') as WorkDate,case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then 'Leave' else '' end + case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then(select distinct case when l1.LeaveType = 0 then '' else lt.Code end from leaves l1 join LeaveTypes lt on l1.leaveType = lt.Id where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status != 'PartialCancelled' and l1.status != 'Cancelled' and l1.status != 'Denied') else ' ' end as WorkDescription, case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then(select distinct l1.status from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status != 'PartialCancelled' and l1.status != 'Cancelled' and l1.status != 'Denied') else 'Not Applied' end as Status, case when e.branch = 43 then dept.Name else b.Name end as BrDepot,case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then(select distinct FORMAT(l1.UpdatedDate, 'dd/MM/yyyy') from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status != 'PartialCancelled' and l1.status != 'Cancelled' and l1.status != 'Denied') else '' end as AppliedDate  FROM calender bc CROSS JOIN Employees e join Designations d on e.CurrentDesignation = d.ID join Branches b on e.branch = b.Id join Departments dept on e.department = dept.Id WHERE NOT EXISTS(SELECT * FROM WorkDiary a WHERE a.empid = e.empid AND(DATEADD(day, DATEDIFF(day, 0, a.wddate), 0) = bc.mydate OR DATEADD(day, DATEDIFF(day, 0, a.wddate), 0) = bc.mydate)) and mydate between '" + fdate + "' and '" + tdate + "' and retirementdate> convert(Date, GETDATE()) and e.empid not in (1, 123456) and CurrentDesignation in (8, 7, 6, 5) and bc.mydate not in (select date from holidaylist)) as x  order by WorkDate ";

            //DataSet ds = sh.Get_MultiTables_FromQry(qry1 + qry2 + qry3 + notapplied);
            DataSet ds = sh.Get_MultiTables_FromQry(qry1 + qry2 + qry3);

            DataTable dtemps = ds.Tables[0];
            DataTable dtholidays = ds.Tables[1];
            DataTable dtall = ds.Tables[2];
            //DataTable dtnotapplied = ds.Tables[3];
            DataTable dtallwds = new DataTable();// ds.Tables[2];

            dtallwds.Columns.Add("empid");
            dtallwds.Columns.Add("Shortname");
            dtallwds.Columns.Add("designation");
            dtallwds.Columns.Add("branch");
            dtallwds.Columns.Add("WorkDate");
            //dtallwds.Columns.Add("WorkName");
            dtallwds.Columns.Add("WorkDescription");
            dtallwds.Columns.Add("Status");
            dtallwds.Columns.Add("BrDepot");
            dtallwds.Columns.Add("AppliedDate");
            //dtallwds = ds.Tables[2];
            //foreach (DataRow dremps in dtemps.Rows)
            //{
            foreach (DataRow d1r in dtall.Rows)
            {
                //DateTime adt =Convert.ToDateTime( d1r["AppliedDate"]);
                //string applieddate = adt.ToString("dd/MM/yyyy").Replace('-', '/');
                DataRow dr = dtallwds.NewRow();
                dr["empid"] = d1r["EmpId"];
                dr["Shortname"] = d1r["Shortname"];
                dr["designation"] = d1r["designation"];
                dr["branch"] = d1r["branch"];

                DateTime dt = (Convert.ToDateTime(d1r["WorkDate"]));
                dr["WorkDate"] = dt.ToString("dd/MM/yyyy").Replace('-', '/');

                //dr["WorkDate"] = (Convert.ToDateTime(d1r["WorkDate"])).Date;
                //dr["WorkName"] = d1r["WorkName"];
                dr["WorkDescription"] = d1r["WorkDescription"];
                dr["Status"] = d1r["Status"];
                dr["BrDepot"] = d1r["BrDepot"];
                dr["AppliedDate"] = d1r["AppliedDate"];
                // dr.AcceptChanges();
                dtallwds.Rows.Add(dr);
                dtallwds.AcceptChanges();
            }
            //}
            //    foreach (DataRow d1r in dtnotapplied.Rows)
            //{
            //    //DateTime adt =Convert.ToDateTime( d1r["AppliedDate"]);
            //    //string applieddate = adt.ToString("dd/MM/yyyy").Replace('-', '/');
            //    DataRow dr = dtallwds.NewRow();
            //    dr["empid"] = d1r["empid"];
            //    dr["Shortname"] = d1r["Shortname"];
            //    dr["designation"] = d1r["designation"];
            //    dr["branch"] = d1r["branch"];

            //    dr["WorkDate"] = (Convert.ToDateTime(d1r["WorkDate"])).Date;
            //    //dr["WorkName"] = d1r["WorkName"];
            //    dr["WorkDescription"] = d1r["WorkDescription"];
            //    dr["Status"] = d1r["Status"];
            //    dr["BrDepot"] = d1r["BrDepot"];
            //    dr["AppliedDate"] = d1r["AppliedDate"];
            //    // dr.AcceptChanges();
            //    dtallwds.Rows.Add(dr);
            //    dtallwds.AcceptChanges();
            //}
            ////dtallwds.Columns["WorkDate"].DataType = typeof(DateTime);
            //loop dt emps 
            try
            {
                foreach (DataRow dremps in dtemps.Rows)
                {
                    foreach (DataRow drholi in dtholidays.Rows)
                    {

                        DataRow dr = dtallwds.NewRow();
                        //string WorkDate = Convert.ToDateTime(dtallwds.Rows[2]["WorkDate"]).ToString("dd/MM/yyyy");
                        dr["empid"] = dremps["empid"];
                        dr["Shortname"] = dremps["Shortname"];
                        dr["designation"] = dremps["Code"];
                        dr["branch"] = dremps["branch"];
                        //dr["WorkDate"] = (Convert.ToDateTime(drholi["WorkDate"])).Date;
                        //string s = drholi["WorkDate"].ToString();
                        // DateTime dt1 = DateTime.Parse(s);
                        DateTime dt = (Convert.ToDateTime(drholi["WorkDate"])).Date;
                        dr["WorkDate"] = dt.ToString("dd/MM/yyyy").Replace('-', '/');
                        //dr["WorkName"] = drholi["WorkName"];
                        dr["WorkDescription"] = drholi["WorkDescription"];
                        dr["Status"] = "Holiday";
                        dr["BrDepot"] = dremps["BrDepot"];
                        // dr.AcceptChanges(); 

                        dtallwds.Rows.InsertAt(dr, 0);

                        dtallwds.AcceptChanges();

                    }

                }
            }
            catch (Exception ex)
            {
                var e = ex;
            }

            // dtallwds.DefaultView.Sort = "WorkDate DESC";
            //dtallwds.DefaultView.Sort = "empid DESC";

            DataView dv = dtallwds.DefaultView;
            dv.Sort = "WorkDate desc";
            DataTable sortedDT = dv.ToTable();

            //dtallwds = dtallwds.DefaultView.ToTable();
            // string workdate = Convert.ToDateTime(dtallwds.Rows[2]["WorkDate"]).ToString("dd/MM/yyyy");
            //IEnumerable<DataRow> drs = dtallwds.AsEnumerable().OrderBy(x => x.Field<DateTime>("WorkDate"));


            return sortedDT;
        }

        public DataTable getallWorkdairiessearch(string wd, string fromdt, string todt, string employeecode, string status)
        {
            if (fromdt == "" && todt == "" && employeecode == "" && status == "-1")
            {
                // string fdate = Convert.ToDateTime(fromdt).ToString("yyyy-MM-dd");
                //string tdate = Convert.ToDateTime(todt).ToString("yyyy-MM-dd");
                string query = "Select convert(varchar,w.EmpId) as empid , emp.Shortname, d.Code as designation,case when emp.branch = 43 " +
                    "then dept.Name when emp.branch = 46 then dept.Name else b.Name end as branch,w.WDDate as WorkDate," +
                    "e.Name as WorkName,e.[Desc] as WorkDescription,w.Status as Status,"
                      + " case when emp.branch = 43 then dept.Name else b.Name end as BrDepot "
                               + " FROM WorkDiary w " +
                                 "join WorkDiary_Det e on w.Id = e.WDId" +
                                 " join Employees emp on w.EmpId = emp.EmpId" +
                                 " join Designations d on emp.currentdeisgnation = d.ID" +
                                 " join Branches b on emp.branch = b.Id" +
                                 " join Departments dept on emp.department = dept.Id " +
                                   " Where w.Status != 'Draft'" +
                                 // " and  w.WDDate = '" + createddate + "'" +

                                 " and emp.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" +
                                 " and  w.Status = '" + status + "'" +

                            " order by w.WDDate desc";

                return sh.Get_Table_FromQry(query);
            }


            else if (fromdt != "" && todt != "" && employeecode != "" && status != "" && status != "ALL" && status != "NotApplied")
            {
                string fdate = Convert.ToDateTime(fromdt).ToString("yyyy-MM-dd");
                string tdate = Convert.ToDateTime(todt).ToString("yyyy-MM-dd");
                //string query = "Select convert(varchar,w.EmpId)  as empid , emp.Shortname, d.Code as designation , case " +
                //    "when emp.branch = 43 then dept.Name when emp.branch = 46 then dept.Name else b.Name end as branch," +
                //    "w.WDDate as WorkDate,concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                //      + " case when emp.branch = 43 then dept.Name else b.Name end as BrDepot,FORMAT(w.UpdatedDate, 'd/MM/yyyy')as AppliedDate "
                //               + " FROM WorkDiary w " +
                //                 "join WorkDiary_Det e on w.Id = e.WDId" +
                //                 " join Employees emp on w.EmpId = emp.EmpId" +
                //                 " join Designations d on w.CurDesig = d.ID" +
                //                 " join Branches b on w.CurBr = b.Id" +
                //                 " join Departments dept on w.CurDept = dept.Id " +
                //                   " Where w.Status != 'Draft'" +
                //                  // " and  w.WDDate = '" + createddate + "'" +
                //                  " and emp.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" +
                //                 " and w.EmpId = " + employeecode +
                //                 " and  w.Status = '" + status + "'" +
                //                  " and w.WDDate>= '" + fdate + "'" +
                //            " and emp.CurrentDesignation in (8, 7, 6, 5) and w.WDDate<='" + tdate + "'" +
                //            "order by w.WDDate desc";
                string query = " SELECT DISTINCT convert(varchar,wd.empId) as empid, ep.Shortname,ds.Code as designation,  case when ep.branch = 43 then dp.Name when ep.branch = 46 then dp.Name else br.Name end as branch,FORMAT(wd.WDDate, 'dd/MM/yyyy') as WorkDate," +
                "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription, " +
                "wd.Status, case when br.Name = 'OtherBranch' then dp.Name else br.Name end as BrDepot,FORMAT(wd.UpdatedDate, 'd/MM/yyyy') as AppliedDate FROM WorkDiary wd join Employees ep on wd.EmpId = ep.EmpId join Designations ds  on ep.currentdesignation = ds.ID" +
                         " join Branches br on ep.branch = br.Id" +
                         " join Departments dp on ep.department = dp.Id " +
                         " Where wd.Status= '" + status + "'and wd.empid = " + employeecode + " and ep.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + (employeecode != "" ? "AND wd.EmpId=" + employeecode : "")
                         + " and ep.CurrentDesignation in (8, 7, 6, 5) AND (wd.WDDate>='" + fdate + "' and wd.WDDate<='" + tdate + "') ";

                return sh.Get_Table_FromQry(query);
            }
            else if (fromdt != "" && todt != "" && employeecode != "" && status != "" && status != "ALL" && status == "NotApplied")
            {
                string fdate = Convert.ToDateTime(fromdt).ToString("yyyy-MM-dd");
                string tdate = Convert.ToDateTime(todt).ToString("yyyy-MM-dd");
                string query = "SELECT convert(varchar,e.EmpId) as empid , e.Shortname, d.Code as designation,case when e.branch = 43 then dept.Name when e.branch = 46 then dept.Name else b.Name end as branch,bc.mydate as WorkDate," +
                    "case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then 'Leave' else '' end  + case when ((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct case when l1.LeaveType= 0 then '' else lt.Code end from leaves l1 join LeaveTypes lt on l1.leaveType=lt.Id where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='PartialCancelled' and l1.status!='Cancelled' and l1.status!='Denied')" +
                    " else ' ' end as WorkDescription, case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct l1.status from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='PartialCancelled' and l1.status!='Cancelled' and l1.status!='Denied') else 'Not Applied' end as Status," +
                    " case when e.branch = 43 then dept.Name else b.Name end as BrDepot,case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct FORMAT(l1.UpdatedDate,'dd/MM/yyyy') from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='PartialCancelled' and l1.status!='Cancelled' and l1.status!='Denied') else '' end  as AppliedDate " +
                    " FROM calender bc CROSS JOIN Employees e join Designations d on e.CurrentDesignation = d.ID join Branches b on e.branch = b.Id join Departments dept on e.department = dept.Id " +
                    "WHERE NOT EXISTS(SELECT * FROM WorkDiary a WHERE a.empid = e.empid AND(DATEADD(day,DATEDIFF(day, 0, a.wddate), 0) = bc.mydate OR DATEADD(day, DATEDIFF(day, 0, a.wddate), 0) = bc.mydate)) and " +
                    "mydate between '" + fdate + "' and '" + tdate + "' and retirementdate>convert(Date, GETDATE()) and e.empid not in (1, 123456) and CurrentDesignation in (8, 7, 6, 5) and bc.mydate not in (select date from holidaylist) " +
                    "and e.empid = " + employeecode + " order by e.empid,bc.mydate ";

                return sh.Get_Table_FromQry(query);
            }

            else if (fromdt != "" && todt != "" && employeecode != "" && status != "" && status == "ALL")
            {
                string fdate = Convert.ToDateTime(fromdt).ToString("yyyy-MM-dd");
                string tdate = Convert.ToDateTime(todt).ToString("yyyy-MM-dd");
                //  //not applied
                //  string query = "SELECT convert(varchar,e.EmpId) as empid , e.Shortname, d.Code as designation,case when e.branch = 43 then dept.Name when e.branch = 46 then dept.Name else b.Name end as branch,bc.mydate as WorkDate," +
                //      "case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then 'Leave' else '' end  + case when ((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct case when l1.LeaveType= 0 then '' else lt.Code end from leaves l1 join LeaveTypes lt on l1.leaveType=lt.Id where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='Cancelled')" +
                //      " else ' ' end as WorkDescription, case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct l1.status from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='Cancelled') else 'Not Applied' end as Status," +
                //      " case when e.branch = 43 then dept.Name else b.Name end as BrDepot,case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct FORMAT(l1.UpdatedDate,'dd/MM/yyyy') from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='Cancelled') else '' end  as AppliedDate " +
                //      " FROM calender bc CROSS JOIN Employees e join Designations d on e.CurrentDesignation = d.ID join Branches b on e.branch = b.Id join Departments dept on e.department = dept.Id " +
                //      "WHERE NOT EXISTS(SELECT * FROM WorkDiary a WHERE a.empid = e.empid AND(DATEADD(day,DATEDIFF(day, 0, a.wddate), 0) = bc.mydate OR DATEADD(day, DATEDIFF(day, 0, a.wddate), 0) = bc.mydate)) and " +
                //      "mydate between '" + fdate + "' and '" + tdate + "' and retirementdate> '" + fdate + "' and e.empid not in (1, 123456) and CurrentDesignation in (8, 7, 6, 5) and bc.mydate not in (select date from holidaylist) " +
                //      "and e.empid = " + employeecode + " order by e.empid,bc.mydate ";

                //  //string qry3 = "  SELECT DISTINCT wd.EmpId, ep.ShortName,ds.Code as Designation,  case when ep.branch = 43 then dp.Name when ep.branch = 46 then dp.Name else br.Name end as branch,FORMAT(wd.WDDate,'dd/MM/yyyy') as WorkDate," +
                //  //"concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription, " +
                //  //"wd.Status, case when br.Name = 'OtherBranch' then dp.Name else br.Name end as BrDepot,FORMAT(wd.UpdatedDate, 'd/MM/yyyy') as AppliedDate FROM WorkDiary wd join Employees ep on wd.EmpId = ep.EmpId join Designations ds  on wd.CurDesig = ds.ID" +
                //  //         " join Branches br on wd.CurBr = br.Id" +
                //  //         " join Departments dp on wd.CurDept = dp.Id " +
                //  //         " Where wd.Status!='Draft' " + "and wd.empid = " + employeecode + " and ep.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + (employeecode != "" ? "AND wd.EmpId=" + employeecode : "")
                //  //         + " and ep.CurrentDesignation in (8, 7, 6, 5) AND (wd.WDDate>='" + fdate + "' and wd.WDDate<='" + tdate + "') ";
                //  string qry3 = "  SELECT wd.EmpId, ep.ShortName,ds.Code as Designation,  case when ep.branch = 43 then dp.Name when ep.branch = 46 then dp.Name else br.Name end as branch,FORMAT(wd.WDDate,'dd/MM/yyyy') as WorkDate," +
                //  "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription, " +
                //  "wd.Status, case when br.Name = 'OtherBranch' then dp.Name else br.Name end as BrDepot,FORMAT(wd.UpdatedDate, 'd/MM/yyyy') as AppliedDate FROM WorkDiary wd join Employees ep on wd.EmpId = ep.EmpId join Designations ds  on wd.CurDesig = ds.ID" +
                //           " join Branches br on wd.CurBr = br.Id" +
                //           " join Departments dp on wd.CurDept = dp.Id " +
                //           " Where wd.Status!='Draft' " + "and wd.empid = " + employeecode + " and ep.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + (employeecode != "" ? "AND wd.EmpId=" + employeecode : "")
                //           + " and ep.CurrentDesignation in (8, 7, 6, 5) AND (wd.WDDate>='" + fdate + "' and wd.WDDate<='" + tdate + "') group by wd.Empid,ep.ShortName,ds.Code,ep.branch,dp.name,br.name,wd.wddate,wd.id,wd.status,wd.updateddate order by wd.wddate; ";

                //  string qry2 = " Select FORMAT([Date],'dd/MM/yyyy')as WorkDate, 'Holiday :' +Occasion as WorkDescription from HolidayList " +
                //  "where [Date]>='" + fdate + "' and [Date]<='" + tdate + "' " +
                //" order by WorkDate desc;";

                /***** for unio query all emp**/
                string query = "select x.empid,x.Shortname,x.designation,x.branch,x.WorkDate,x.WorkDescription,x.Status,x.BrDepot,x.AppliedDate from " +
                    "(SELECT wd.EmpId, ep.ShortName, ds.Code as Designation,  case when ep.branch = 43 then dp.Name when ep.branch = 46 then dp.Name else br.Name end as branch, cast(wd.WDDate as Date) as WorkDate, concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription, wd.Status, case when br.Name = 'OtherBranch' then dp.Name else br.Name end as BrDepot,Format(wd.UpdatedDate,'dd/MM/yyyy') as AppliedDate FROM WorkDiary wd join Employees ep on wd.EmpId = ep.EmpId join Designations ds on ep.currentdesignation = ds.ID join Branches br on wd.CurBr = br.Id join Departments dp on wd.CurDept = dp.Id  Where wd.Status != 'Draft' and wd.empid = " + employeecode + " and ep.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))AND wd.EmpId = " + employeecode + " and ep.CurrentDesignation in (8, 7, 6, 5) " +
                    "AND(wd.WDDate >= '" + fdate + "' and wd.WDDate <= '" + tdate + "') group by wd.Empid,ep.ShortName,ds.Code,ep.branch,dp.name,br.name,wd.wddate,wd.id,wd.status,wd.updateddate " +
                    "union all " +
                    " SELECT convert(varchar, e.EmpId) as empid , e.Shortname, d.Code as designation,case when e.branch = 43 then dept.Name when e.branch = 46 then dept.Name else b.Name end as branch,cast(bc.mydate as Date) as WorkDate,case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then 'Leave' else '' end + case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then(select distinct case when l1.LeaveType = 0 then '' else lt.Code end from leaves l1 join LeaveTypes lt on l1.leaveType = lt.Id where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='PartialCancelled' and l1.status != 'Cancelled' and l1.status !='Denied') else ' ' end as WorkDescription, case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then(select distinct l1.status from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and" +
                    "  l1.status!='PartialCancelled' and l1.status != 'Cancelled' and l1.status !='Denied') else 'Not Applied' end as Status, case when e.branch = 43 then dept.Name else b.Name end as BrDepot,case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then(select distinct Format(l1.UpdatedDate, 'dd/MM/yyyy') from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='PartialCancelled' and l1.status != 'Cancelled' and l1.status !='Denied') else '' end as AppliedDate  FROM calender bc CROSS JOIN Employees e join Designations d on e.CurrentDesignation = d.ID join Branches b on e.branch = b.Id join Departments dept on e.department = dept.Id WHERE NOT EXISTS(SELECT * FROM WorkDiary a WHERE a.empid = e.empid AND(DATEADD(day, DATEDIFF(day, 0, a.wddate), 0) = bc.mydate OR DATEADD(day, DATEDIFF(day, 0, a.wddate), 0) = bc.mydate)) and " +
                    "mydate between '" + fdate + "' and '" + tdate + "' and retirementdate> '" + fdate + "' and e.empid not in (1, 123456) and CurrentDesignation in (8, 7, 6, 5) " +
                    "and bc.mydate not in (select date from holidaylist) and e.empid = " + employeecode + "  " +
                    " union all " +
                    "  Select '' as empid,'' Shortname,'' designation,'' branch,cast([Date] as date)as WorkDate, 'Holiday :' + Occasion as WorkDescription,'' Status,'' BrDepot,'' as AppliedDate from HolidayList where [Date] >= '" + fdate + "' and[Date] <= '" + tdate + "'  ) as x order by x.WorkDate; ";

                //only emp data
                string qry1 = " Select convert(varchar,w.EmpId) as EmpId,  emp.Shortname , d.Code as designation , case when b.Name='OtherBranch' then dept.Name when b.Name='HeadOffice' then dept.Name else b.Name end as branch,case when b.Name = 'OtherBranch' then dept.Name else b.Name end as BrDepot" +
                    " FROM WorkDiary w join WorkDiary_Det e on w.Id = e.WDId " +
                    " join Employees emp on w.EmpId = emp.EmpId " +
                    " join Designations d on emp.currentdesignation = d.ID join Branches b on emp.branch= b.Id join Departments dept on emp.department= dept.Id " +
                    " Where w.Status != 'Draft' and w.Status in ('Pending','Approved')  and emp.CurrentDesignation in (8, 7, 6, 5) and  emp.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))  and (w.WDDate >= '" + fdate + "' and w.WDDate <='" + tdate + "') "
                    + (employeecode != "" ? "AND w.EmpId=" + employeecode : "")
                    + " group by convert(varchar, w.EmpId) , emp.Shortname , d.Code , case when b.Name = 'OtherBranch' then dept.Name when b.Name = 'HeadOffice' then dept.Name else b.Name end,case when b.Name = 'OtherBranch' then dept.Name else b.Name end; ";

                //DataSet ds = sh.Get_MultiTables_FromQry(qry3 + query + qry2 + qry1);
                DataSet ds = sh.Get_MultiTables_FromQry(query + qry1);
           
                DataTable dtall = ds.Tables[0];
                //DataTable dtnotapplied = ds.Tables[1];
                //DataTable dtholidays = ds.Tables[2];
                DataTable dtemps = ds.Tables[1];
                DataTable dtallwds = new DataTable();// ds.Tables[2];

                dtallwds.Columns.Add("empid");
                dtallwds.Columns.Add("Shortname");
                dtallwds.Columns.Add("designation");
                dtallwds.Columns.Add("branch");
                //dtallwds.Columns.Add("WorkDate", typeof(DateTime));
                dtallwds.Columns.Add("WorkDate");
                //dtallwds.Columns.Add("WorkName");
                dtallwds.Columns.Add("WorkDescription");
                dtallwds.Columns.Add("Status");
                dtallwds.Columns.Add("BrDepot");
                dtallwds.Columns.Add("AppliedDate");
                //dtallwds = ds.Tables[2];
                //foreach (DataRow d1r in dtall.Rows)
                //               {
                //                   //DateTime adt =Convert.ToDateTime( d1r["AppliedDate"]);
                //                   //string applieddate = adt.ToString("dd/MM/yyyy").Replace('-', '/');
                //                   DataRow dr = dtallwds.NewRow();
                //                   dr["empid"] = d1r["empid"];
                //                   dr["Shortname"] = d1r["Shortname"];
                //                   dr["designation"] = d1r["designation"];
                //                   dr["branch"] = d1r["branch"];
                //                   dr["WorkDate"] = (Convert.ToDateTime(d1r["WorkDate"])).Date;
                //                   //dr["WorkName"] = d1r["WorkName"];
                //                   dr["WorkDescription"] = d1r["WorkDescription"];
                //                   dr["Status"] = d1r["Status"];
                //                   dr["BrDepot"] = d1r["BrDepot"];
                //                   dr["AppliedDate"] = d1r["AppliedDate"];
                //                   // dr.AcceptChanges();
                //                   dtallwds.Rows.Add(dr);
                //                   dtallwds.AcceptChanges();
                //               }
                //               foreach (DataRow d1r in dtnotapplied.Rows)
                //               {
                //                   //DateTime adt =Convert.ToDateTime( d1r["AppliedDate"]);
                //                   //string applieddate = adt.ToString("dd/MM/yyyy").Replace('-', '/');
                //                   DataRow dr = dtallwds.NewRow();
                //                   dr["empid"] = d1r["empid"];
                //                   dr["Shortname"] = d1r["Shortname"];
                //                   dr["designation"] = d1r["designation"];
                //                   dr["branch"] = d1r["branch"];
                //                   dr["WorkDate"] = (Convert.ToDateTime(d1r["WorkDate"])).Date;
                //                   //dr["WorkName"] = d1r["WorkName"];
                //                   dr["WorkDescription"] = d1r["WorkDescription"];
                //                   dr["Status"] = d1r["Status"];
                //                   dr["BrDepot"] = d1r["BrDepot"];
                //                   dr["AppliedDate"] = d1r["AppliedDate"];
                //                   // dr.AcceptChanges();
                //                   dtallwds.Rows.Add(dr);
                //                   dtallwds.AcceptChanges();
                //               }               

                foreach (DataRow dremps in dtemps.Rows)
                {
                    foreach (DataRow d1r in dtall.Rows)
                    {
                        //DateTime adt =Convert.ToDateTime( d1r["AppliedDate"]);
                        //string applieddate = adt.ToString("dd/MM/yyyy").Replace('-', '/');
                        DataRow dr = dtallwds.NewRow();
                        dr["empid"] = dremps["empid"];
                        dr["Shortname"] = dremps["Shortname"];
                        dr["designation"] = dremps["designation"];
                        dr["branch"] = dremps["branch"];
                        //dr["WorkDate"] = (Convert.ToDateTime(d1r["WorkDate"])).Date;
                        //dr["WorkName"] = d1r["WorkName"];
                        DateTime dt = (Convert.ToDateTime(d1r["WorkDate"]));
                        dr["WorkDate"] = dt.ToString("dd/MM/yyyy").Replace('-', '/');
                        dr["WorkDescription"] = d1r["WorkDescription"];
                        dr["Status"] = d1r["Status"];
                        dr["BrDepot"] = d1r["BrDepot"];
                        dr["AppliedDate"] = d1r["AppliedDate"];
                        // dr.AcceptChanges();
                        dtallwds.Rows.Add(dr);
                        dtallwds.AcceptChanges();
                    }

                }
                //foreach (DataRow d1r in dtnotapplied.Rows)
                //{
                //    //DateTime adt =Convert.ToDateTime( d1r["AppliedDate"]);
                //    //string applieddate = adt.ToString("dd/MM/yyyy").Replace('-', '/');
                //    DataRow dr = dtallwds.NewRow();
                //    dr["empid"] = d1r["empid"];
                //    dr["Shortname"] = d1r["Shortname"];
                //    dr["designation"] = d1r["designation"];
                //    dr["branch"] = d1r["branch"];
                //    dr["WorkDate"] = (Convert.ToDateTime(d1r["WorkDate"])).Date;
                //    //dr["WorkName"] = d1r["WorkName"];
                //    dr["WorkDescription"] = d1r["WorkDescription"];
                //    dr["Status"] = d1r["Status"];
                //    dr["BrDepot"] = d1r["BrDepot"];
                //    dr["AppliedDate"] = d1r["AppliedDate"];
                //    // dr.AcceptChanges();
                //    dtallwds.Rows.Add(dr);
                //    dtallwds.AcceptChanges();
                //}

                //foreach (DataRow dremps in dtemps.Rows)
                //{
                //    foreach (DataRow drholi in dtholidays.Rows)
                //    {
                //        DataRow dr = dtallwds.NewRow();
                //        //string WorkDate = Convert.ToDateTime(dtallwds.Rows[2]["WorkDate"]).ToString("dd/MM/yyyy");
                //        dr["empid"] = dremps["empid"];
                //        dr["Shortname"] = dremps["Shortname"];
                //        dr["designation"] = dremps["designation"];
                //        dr["branch"] = dremps["branch"];
                //        dr["WorkDate"] = (Convert.ToDateTime(drholi["WorkDate"])).Date;
                //        //dr["WorkName"] = drholi["WorkName"];
                //        dr["WorkDescription"] = drholi["WorkDescription"];
                //        dr["Status"] = "Holiday";
                //        dr["BrDepot"] = dremps["BrDepot"];
                //        // dr.AcceptChanges(); 
                //        dtallwds.Rows.InsertAt(dr, 0);
                //        dtallwds.AcceptChanges();
                //    }
                //}
                return dtallwds;
            }
            else if (fromdt != "" && todt != "" && employeecode == "" && status != "" && status != "ALL" && status == "NotApplied")
            {
                string fdate = Convert.ToDateTime(fromdt).ToString("yyyy-MM-dd");
                string tdate = Convert.ToDateTime(todt).ToString("yyyy-MM-dd");
                //string query = "SELECT convert(varchar,e.EmpId) as empid , e.Shortname, d.Code as designation,case when e.branch = 43 " +
                //    "then dept.Name when e.branch = 46 then dept.Name else b.Name end as branch,bc.mydate as WorkDate,'' as WorkName,'' " +
                //    "as WorkDescription,'' as Status, case " +
                //    "when e.branch = 43 then dept.Name else b.Name end as BrDepot,'' as AppliedDate FROM calender bc " +
                //    "CROSS JOIN Employees e join Designations d on e.CurrentDesignation = d.ID " +
                //    "join Branches b on e.branch = b.Id join Departments dept on e.department = dept.Id " +
                //    "WHERE NOT EXISTS(SELECT * FROM WorkDiary a WHERE a.empid = e.empid " +
                //    "AND(DATEADD(day, DATEDIFF(day, 0, a.wddate), 0) = bc.mydate " +
                //    "OR DATEADD(day, DATEDIFF(day, 0, a.wddate), 0) = bc.mydate)) and mydate between '" + fdate + "' and '" + tdate + "' " +
                //    "and retirementdate> '" + fdate + "' and e.empid not in (1, 123456) and CurrentDesignation in (8, 7, 6, 5) " +
                //    "and bc.mydate not in (select date from holidaylist) " +
                //    "order by e.empid,bc.mydate";

                string query = "SELECT convert(varchar,e.EmpId) as empid , e.Shortname, d.Code as designation,case when e.branch = 43 then dept.Name when e.branch = 46 then dept.Name else b.Name end as branch,convert(varchar,bc.mydate,103) as WorkDate," +
                    "case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then 'Leave' else '' end + case when ((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct case when l1.LeaveType= 0 then '' else lt.Code end from leaves l1 join LeaveTypes lt on l1.leaveType=lt.Id where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='PartialCancelled' and l1.status!='Cancelled' and l1.status!='Denied')" +
                    " else ' ' end as WorkDescription, case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct l1.status from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='PartialCancelled' and l1.status!='Cancelled' and l1.status!='Denied') else 'Not Applied' end as Status," +
                    " case when e.branch = 43 then dept.Name else b.Name end as BrDepot,case when((select count(*) from leaves l where bc.mydate between l.startDate and l.enddate and l.empid = e.Id) > 0) then (select distinct FORMAT(l1.UpdatedDate,'dd/MM/yyyy') from leaves l1 where bc.mydate between l1.startDate and l1.enddate and l1.empid = e.Id and l1.status!='PartialCancelled' and l1.status!='Cancelled' and l1.status!='Denied') else '' end  as AppliedDate " +
                    " FROM calender bc CROSS JOIN Employees e join Designations d on e.CurrentDesignation = d.ID join Branches b on e.branch = b.Id join Departments dept on e.department = dept.Id " +
                    "WHERE NOT EXISTS(SELECT * FROM WorkDiary a WHERE a.empid = e.empid AND(DATEADD(day,DATEDIFF(day, 0, a.wddate), 0) = bc.mydate OR DATEADD(day, DATEDIFF(day, 0, a.wddate), 0) = bc.mydate)) and " +
                    "mydate between '" + fdate + "' and '" + tdate + "' and retirementdate> convert(Date, GETDATE()) and e.empid not in (1, 123456) and CurrentDesignation in (8, 7, 6, 5) and bc.mydate not in (select date from holidaylist)";
                return sh.Get_Table_FromQry(query);
            }

            else if (employeecode == "" && fromdt != "" && todt != "" && status != "" && status != "ALL")
            {
                string fdate = Convert.ToDateTime(fromdt).ToString("yyyy-MM-dd");
                string tdate = Convert.ToDateTime(todt).ToString("yyyy-MM-dd");
                //string createddate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");

                string query = "  select empid,Shortname,designation,branch,WorkDate,WorkDescription,Status,BrDepot,AppliedDate from (SELECT DISTINCT convert(varchar,wd.EmpId) as empid, ep.Shortname,ds.Code as designation,  case when ep.branch = 43 then dp.Name when ep.branch = 46 then dp.Name else br.Name end as branch,FORMAT(wd.WDDate,'dd/MM/yyyy') as WorkDate," +
                "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription, " +
                "wd.Status, case when br.Name = 'OtherBranch' then dp.Name else br.Name end as BrDepot,FORMAT(wd.UpdatedDate, 'd/MM/yyyy') as AppliedDate FROM WorkDiary wd join Employees ep on wd.EmpId = ep.EmpId join Designations ds  on ep.currentdesignation = ds.ID" +
                         " join Branches br on wd.CurBr = br.Id" +
                         " join Departments dp on wd.CurDept = dp.Id " +
                         " Where wd.Status!='Draft' " + " and ep.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + (employeecode != "" ? "AND wd.EmpId=" + employeecode : "")
                         + " and ep.CurrentDesignation in (8, 7, 6, 5) AND (wd.WDDate>='" + fdate + "' and wd.WDDate<='" + tdate + "') and wd.Status='" + status + "') as x order by WorkDate";


                return sh.Get_Table_FromQry(query);
            }

            else
            {
                string fdate = "01-01-01";
                string tdate = "01-01-01";
                //string createddate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");
                string query = "Select convert(varchar,w.EmpId)as  empid , emp.Shortname, d.Code as designation , case " +
                    "when emp.branch = 43 then dept.Name when emp.branch = 46 then dept.Name " +
                    "else b.Name end as branch,w.WDDate as WorkDate,e.Name as WorkName,e.[Desc] as WorkDescription,w.Status as Status,"
                      + " case when emp.branch = 43 then dept.Name else b.Name end as BrDepot,FORMAT(w.UpdatedDate, 'd/MM/yyyy')as AppliedDate "
                          + " FROM WorkDiary w " +
                            "join WorkDiary_Det e on w.Id = e.WDId" +
                            " join Employees emp on w.EmpId = emp.EmpId" +
                            " join Designations d on emp.currentdesignation = d.ID" +
                            " join Branches b on w.CurBr = b.Id" +
                            " join Departments dept on w.CurDept = dept.Id " +
                             " Where w.Status != 'Draft'" +
                            " and w.Status = '" + status + "'" +
                            " and emp.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" +
                           //" and  w.WDDate = '" + createddate + "'";
                           " and w.WDDate>= '" + fdate + "'" +
                            " and w.WDDate<='" + tdate + "'" +
                            "order by w.WDDate desc";


                return sh.Get_Table_FromQry(query);
            }


            return null;
        }


        public DataTable getAllPL()
        {
            string qry = " Select e.EmpId,e.Shortname as Name,d.code as Designation,p.TravelAdvance,e.TotalExperience,p.TotalPL, p.PLEncash,p.Reason,p.Subject,p.Status,p.UpdatedDate,p.Currentyear"
                 + " FROM PLE_Type p" +
                 " join Employees e on p.EmpId = e.Id" +
                 " join Designations d on e.currentdesignation = d.Id";
            return sh.Get_Table_FromQry(qry);
        }
        public DataTable getAllPLsearch(string status, string empcode)
        {
            if (status == "" || empcode == "")
            {
                string qry = " Select e.EmpId,e.Shortname as Name,d.code as Designation,p.TravelAdvance,e.TotalExperience,p.TotalPL, p.PLEncash,p.Reason,p.Subject,p.Status,p.UpdatedDate,p.Currentyear"
                + " FROM PLE_Type p" +
                " join Employees e on p.EmpId = e.Id" +
                " join Designations d on e.currentdesignation = d.Id";
                return sh.Get_Table_FromQry(qry);
            }
            else
            {
                string qry = " Select e.EmpId,e.Shortname as Name,d.code as Designation,p.TravelAdvance,e.TotalExperience,p.TotalPL, p.PLEncash,p.Reason,p.Subject,p.Status,p.UpdatedDate,p.Currentyear"
                    + " FROM PLE_Type p" +
                    " join Employees e on p.EmpId = e.Id" +
                    " join Designations d on e.currentdesignation = d.Id" +
                    " where e.EmpId = " + empcode +
                    " and" +
                    " p.Status = '" + status + "'";
                return sh.Get_Table_FromQry(qry);
            }
        }

        public DataTable WorkDiaryExcel(string lworkDate, string lEmpId)
        {
            string qry = "";

            if (lworkDate == "" && lEmpId == "")
            {

                qry = "  SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
                 + " FROM workdiary_det workdet"
                + " WHERE workdet.wdid =" + " wd.Id" +
                " FOR XML PATH('')), 1, 2, ''),"
                + " [Desc] = STUFF((SELECT ', ' + [Desc]"
                + " FROM workdiary_det workdet" +
                " WHERE workdet.wdid =" + " wd.Id"
                + " FOR XML PATH('')), 1, 2, ''), " +
                " CONVERT(varchar, wd.WDDate,103)as WDDate,wd.Status"

                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = ep.CurrentDesignation "
                + " join Departments dp on dp.Id = ep.Department"
                + " join Branches br on br.Id = ep.Branch"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + " WHERE wd.Status != 'Pending'"
                + "order by WDDate desc";
            }
            else if (lworkDate != "" && lEmpId == "")
            {
                qry = " SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
                 + " FROM workdiary_det workdet"
                + " WHERE workdet.wdid =" + " wd.Id" +
                " FOR XML PATH('')), 1, 2, ''),"
                + " [Desc] = STUFF((SELECT ', ' + [Desc]"
                + " FROM workdiary_det workdet" +
                " WHERE workdet.wdid =" + " wd.Id"
                + " FOR XML PATH('')), 1, 2, ''), " +
                " CONVERT(varchar, wd.WDDate,103)as WDDate,wd.Status"

                        + " FROM WorkDiary wd "
                        + "  join Employees ep on wd.EmpId = ep.EmpId"
                        + " join Designations ds on ds.Id = ep.CurrentDesignation "
                        + " join Departments dp on dp.Id = ep.Department"
                        + " join Branches br on br.Id = ep.Branch"
                        + " join workdiary_det workdet on wd.Id = workdet.WDId "
                        + "WHERE wd.WDDate= " + " '" + lworkDate + " '"
                        + " and wd.Status != 'Pending' ";
            }
            else if (lworkDate == "" && lEmpId != "")
            {
                qry = "  SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
                 + " FROM workdiary_det workdet"
                + " WHERE workdet.wdid =" + " wd.Id" +
                " FOR XML PATH('')), 1, 2, ''),"
                + " [Desc] = STUFF((SELECT ', ' + [Desc]"
                + " FROM workdiary_det workdet" +
                " WHERE workdet.wdid =" + " wd.Id"
                + " FOR XML PATH('')), 1, 2, ''), " +
                " CONVERT(varchar, wd.WDDate,103)as WDDate,wd.Status"

        + " FROM WorkDiary wd "
        + "  join Employees ep on wd.EmpId = ep.EmpId"
        + " join Designations ds on ds.Id = ep.CurrentDesignation "
        + " join Departments dp on dp.Id = ep.Department"
        + " join Branches br on br.Id = ep.Branch"
        + " join workdiary_det workdet on wd.Id = workdet.WDId "
        + "WHERE wd.EmpId= " + lEmpId
         + " and wd.Status != 'Pending' ";
            }
            else if (lworkDate != "" && lEmpId != "")
            {
                qry = "  SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
                 + " FROM workdiary_det workdet"
                + " WHERE workdet.wdid =" + " wd.Id" +
                " FOR XML PATH('')), 1, 2, ''),"
                + " [Desc] = STUFF((SELECT ', ' + [Desc]"
                + " FROM workdiary_det workdet" +
                " WHERE workdet.wdid =" + " wd.Id"
                + " FOR XML PATH('')), 1, 2, ''), " +
                " CONVERT(varchar, wd.WDDate,103)as WDDate ,wd.Status"
                        + " FROM WorkDiary wd "
                        + "  join Employees ep on wd.EmpId = ep.EmpId"
                        + " join Designations ds on ds.Id = ep.CurrentDesignation "
                        + " join Departments dp on dp.Id = ep.Department"
                        + " join Branches br on br.Id = ep.Branch"
                        + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "WHERE wd.EmpId= " + lEmpId + " and " + "wd.WDDate='" + lworkDate + "' and wd.Status !='Pending'";
            }
            return sh.Get_Table_FromQry(qry);
        }
        public DataTable WorkDiaryPDF(string lworkDate, string lEmpId)
        {
            string qry = "";

            if (lworkDate == "" && lEmpId == "")
            {

                qry = "  SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
          + " FROM workdiary_det workdet"
         + " WHERE workdet.wdid =" + " wd.Id" +
         " FOR XML PATH('')), 1, 2, ''),"
         + " [Desc] = STUFF((SELECT ', ' + [Desc]"
         + " FROM workdiary_det workdet" +
         " WHERE workdet.wdid =" + " wd.Id"
         + " FOR XML PATH('')), 1, 2, '')," +
                "wd.WDDate,wd.Status"
         + " FROM WorkDiary wd "
         + "  join Employees ep on wd.EmpId = ep.EmpId"
         + " join Designations ds on ds.Id = ep.CurrentDesignation "
         + " join Departments dp on dp.Id = ep.Department"
         + " join Branches br on br.Id = ep.Branch"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + " WHERE wd.Status != 'Pending'"
                + "order by WDDate desc";
            }
            else if (lworkDate != "" && lEmpId == "")
            {
                qry = " SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
                         + " FROM workdiary_det workdet"
                        + " WHERE workdet.wdid =" + " wd.Id" +
                        " FOR XML PATH('')), 1, 2, ''),"
                        + " [Desc] = STUFF((SELECT ', ' + [Desc]"
                        + " FROM workdiary_det workdet" +
                        " WHERE workdet.wdid =" + " wd.Id"
                        + " FOR XML PATH('')), 1, 2, '')," +
              "wd.WDDate,wd.Status"

                        + " FROM WorkDiary wd "
                        + "  join Employees ep on wd.EmpId = ep.EmpId"
                        + " join Designations ds on ds.Id = ep.CurrentDesignation "
                        + " join Departments dp on dp.Id = ep.Department"
                        + " join Branches br on br.Id = ep.Branch"
                        + " join workdiary_det workdet on wd.Id = workdet.WDId "
                        + "WHERE wd.WDDate= " + " '" + lworkDate + " '"
                        + " and wd.Status != 'Pending' ";
            }
            else if (lworkDate == "" && lEmpId != "")
            {
                qry = "  SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
         + " FROM workdiary_det workdet"
        + " WHERE workdet.wdid =" + " wd.Id" +
        " FOR XML PATH('')), 1, 2, ''),"
        + " [Desc] = STUFF((SELECT ', ' + [Desc]"
        + " FROM workdiary_det workdet" +
        " WHERE workdet.wdid =" + " wd.Id"
        + " FOR XML PATH('')), 1, 2, '')," +
               "wd.WDDate,wd.Status"

        + " FROM WorkDiary wd "
        + "  join Employees ep on wd.EmpId = ep.EmpId"
        + " join Designations ds on ds.Id = ep.CurrentDesignation "
        + " join Departments dp on dp.Id = ep.Department"
        + " join Branches br on br.Id = ep.Branch"
        + " join workdiary_det workdet on wd.Id = workdet.WDId "
        + "WHERE wd.EmpId= " + lEmpId
         + " and wd.Status != 'Pending' ";
            }
            else if (lworkDate != "" && lEmpId != "")
            {
                qry = "  SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
                         + " FROM workdiary_det workdet"
                        + " WHERE workdet.wdid =" + " wd.Id" +
                        " FOR XML PATH('')), 1, 2, ''),"
                        + " [Desc] = STUFF((SELECT ', ' + [Desc]"
                        + " FROM workdiary_det workdet" +
                        " WHERE workdet.wdid =" + " wd.Id"
                        + " FOR XML PATH('')), 1, 2, '')," +
               "wd.WDDate,wd.Status"
                        + " FROM WorkDiary wd "
                        + "  join Employees ep on wd.EmpId = ep.EmpId"
                        + " join Designations ds on ds.Id = ep.CurrentDesignation "
                        + " join Departments dp on dp.Id = ep.Department"
                        + " join Branches br on br.Id = ep.Branch"
                        + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "WHERE wd.EmpId= " + lEmpId + " and " + "wd.WDDate='" + lworkDate + "' and wd.Status !='Pending'";
            }
            return sh.Get_Table_FromQry(qry);
        }



        public DataTable getSanctionWorkApproval(string EmpId)
        {
            string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, Name =STUFF((SELECT ', ' + name"
          + " FROM workdiary_det workdet"
         + " WHERE workdet.wdid =" + " wd.Id" +
         " FOR XML PATH('')), 1, 2, ''),"
         + " [Desc] = STUFF((SELECT ', ' + [Desc]"
         + " FROM workdiary_det workdet" +
         " WHERE workdet.wdid =" + " wd.Id"
         + " FOR XML PATH('')), 1, 2, '')," +
         " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
         + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
         + " FROM WorkDiary wd "
         + "  join Employees ep on wd.EmpId = ep.EmpId"
         + " join Designations ds on ds.Id = ep.CurrentDesignation "
         + " join Departments dp on dp.Id = ep.Department"
         + " join Branches br on br.Id = ep.Branch"
         + " join workdiary_det workdet on wd.Id = workdet.WDId "
         + "  where wd.SA =" + EmpId + " and" + " wd.status =" + " 'Approved'" + " order by wd.WDDate desc";
            return sh.Get_Table_FromQry(qry);
        }
        public DataTable getYearwiseleavebalance()
        {
            string qry = " select e.EmpId,e.ShortName,d.name as CurrentDesignation,"
 + " case when e.Branch='43' then (select top 1 name from  Departments where id=e.Department)else (select top 1 name from  Branches where id = e.Branch) end as Branch,elc.CasualLeave as TotalBalanceCl, l.CasualLeave as RemainingCl,(elc.CasualLeave - l.CasualLeave) as TotalCLLeavesApplied,elc.LeaveCredit as CLCredit,elc.LeaveDebit as CLDebit,elc.PrivilegeLeave as TotalBalancePL, l.PrivilegeLeave as RemainingPL,(elc.PrivilegeLeave - l.PrivilegeLeave) as TotalPLLeavesApplied,elc.leavetypeid,elc.LeaveCredit as PLCredit,elc.LeaveDebit as PLDebit,l.PrivilegeLeave as RemainingPL,elc.PrivilegeLeave as TotalBalancePL,elc.LeaveCredit as MLCredit,elc.LeaveDebit as MLDebit,l.MedicalSickLeave as RemainingML,elc.MedicalSickLeave as TotalBalanceML,(elc.MedicalSickLeave - l.MedicalSickLeave) as TotalMLLeavesApplied ,elc.LeaveCredit as MTLCredit,elc.LeaveDebit as MTLDebit,l.MaternityLeave as RemainingMTL,elc.MaternityLeave as TotalBalanceMTL,(elc.MaternityLeave - l.MaternityLeave) as TotalMTLLeavesApplied ,elc.LeaveCredit as PTLCredit,elc.LeaveDebit as PTLDebit,l.PaternityLeave as RemainingPTL,elc.PaternityLeave as TotalBalancePTL,(elc.PaternityLeave - l.PaternityLeave) as TotalPTLLeavesApplied ,elc.LeaveCredit as EOLCredit,elc.LeaveDebit as EOLDebit,l.ExtraordinaryLeave as RemainingEOL,elc.ExtraordinaryLeave as TotalBalanceEOL,(elc.ExtraordinaryLeave - l.ExtraordinaryLeave) as TotalEOLLeavesApplied ,elc.LeaveCredit as SCLCredit,elc.LeaveDebit as SCLDebit,l.ExtraordinaryLeave as RemainingSCL,elc.specialcasualleave as TotalBalanceSCL,(elc.specialcasualleave - l.specialcasualleave) as TotalSCLLeavesApplied"
+ " from V_EmpLeavesCarryForward elc"
+ " join Employees e on elc.empid = e.id"
+ "join V_EmpLeaveBalance l on l.empid = e.id"
+ " join designations d on d.id = e.currentdesignation where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) ";
            return sh.Get_Table_FromQry(qry);
        }
        public DataTable getStaffMasterList()
        {
            string qry = " SELECT vt.EmpId,vt.FatherName,vt.MotherName,vt.DOB,vt.DOJ,vt.RetirementDate,vt.EmpName,vt.PresentAddress,vt.ProfessionalQualifications,vt.Graduation,vt.PostGraduation,vt.MobileNumber,vt.Category,vt.AadharCardNo,vt.EffectiveFrom,vt.EffectiveTo,vt.Type,d.Code,"
            + " case when vt.OldBranch = '43' then(select top 1 name from Departments where id = vt.OldDepartment)"
            + "else (select top 1 name from  Branches where id = vt.OldBranch) end as oldBrDept,"
            + " case when vt.NewBranch = '43' then(select top 1 name from Departments where id = vt.NewDepartment) else (select top 1 name from  branches where id = vt.NewBranch) end as LatestBrDept,"
            + " case when e.CurrentDesignation = vt.OldDesignation then(select top 1 name from Designations where id = vt.OldDesignation) else (select top 1 name from  Designations where id = vt.newDesignation) end as PresentOldDesig,"
            + " case when b.name='OtherBranch' then dp.name else b.name end as CurrentBrDept"
            + " from view_employee_transfer vt " + " join employees e on vt.EmpId = e.EmpId" + " join designations d on d.id = e.currentdesignation" + " join Branches b on b.id = e.Branch" + " join Departments dp on dp.id = e.Department"
            + " where e.RetirementDate >=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))  order by vt.EffectiveFrom desc";
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getStaffMasterListSearch(String Emp)
        {
            string qry = " SELECT vt.EmpId,vt.FatherName,vt.MotherName,vt.DOB,vt.DOJ,vt.RetirementDate,vt.EmpName,vt.PresentAddress,vt.ProfessionalQualifications,vt.Graduation,vt.PostGraduation,vt.MobileNumber,vt.Category,vt.AadharCardNo,vt.EffectiveFrom,vt.EffectiveTo,vt.Type,d.Code,"
            + " case when vt.OldBranch = '43' then(select top 1 name from Departments where id = vt.OldDepartment)"
            + "else (select top 1 name from  Branches where id = vt.OldBranch) end as oldBrDept,"
            + " case when vt.NewBranch = '43' then(select top 1 name from Departments where id = vt.NewDepartment) else (select top 1 name from  branches where id = vt.NewBranch) end as LatestBrDept,"
            + " case when e.CurrentDesignation = vt.OldDesignation then(select top 1 name from Designations where id = vt.OldDesignation) else (select top 1 name from  Designations where id = vt.newDesignation) end as PresentOldDesig,"
            + " case when b.name='OtherBranch' then dp.name else b.name end as CurrentBrDept "
            + " from view_employee_transfer vt " + " join employees e on vt.EmpId = e.EmpId" + " join designations d on d.id = e.currentdesignation" + " join Branches b on b.id = e.Branch" + " join Departments dp on dp.id = e.Department"
            + " where e.RetirementDate >= convert(Date, GETDATE()) "
            + " and vt.EmpId like " + " '%" + Emp + "'"
            + " or vt.EmpName like " + " '%" + Emp + "'"
            + " or d.Code like " + " '%" + Emp + "'"
            + "  order by vt.EffectiveFrom desc";
            return sh.Get_Table_FromQry(qry);
        }
        public DataTable WorkDiaryPDFEmp(string lworkDate, string lEmpId, string emp)
        {
            string qry = "";

            if (lworkDate == "" && lEmpId == "")
            {

                qry = "  SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
          + " FROM workdiary_det workdet"
         + " WHERE workdet.wdid =" + " wd.Id" +
         " FOR XML PATH('')), 1, 2, ''),"
         + " [Desc] = STUFF((SELECT ', ' + [Desc]"
         + " FROM workdiary_det workdet" +
         " WHERE workdet.wdid =" + " wd.Id"
         + " FOR XML PATH('')), 1, 2, '')," +
                "wd.WDDate,wd.Status"
         + " FROM WorkDiary wd "
         + "  join Employees ep on wd.EmpId = ep.EmpId"
         + " join Designations ds on ds.Id = ep.CurrentDesignation "
         + " join Departments dp on dp.Id = ep.Department"
         + " join Branches br on br.Id = ep.Branch"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + " WHERE wd.Status != 'Pending'"
                + "order by WDDate desc";
            }
            else if (lworkDate != "" && lEmpId == "")
            {
                qry = " SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
                         + " FROM workdiary_det workdet"
                        + " WHERE workdet.wdid =" + " wd.Id" +
                        " FOR XML PATH('')), 1, 2, ''),"
                        + " [Desc] = STUFF((SELECT ', ' + [Desc]"
                        + " FROM workdiary_det workdet" +
                        " WHERE workdet.wdid =" + " wd.Id"
                        + " FOR XML PATH('')), 1, 2, '')," +
              "wd.WDDate,wd.Status"

                        + " FROM WorkDiary wd "
                        + "  join Employees ep on wd.EmpId = ep.EmpId"
                        + " join Designations ds on ds.Id = ep.CurrentDesignation "
                        + " join Departments dp on dp.Id = ep.Department"
                        + " join Branches br on br.Id = ep.Branch"
                        + " join workdiary_det workdet on wd.Id = workdet.WDId "
                        + "WHERE wd.WDDate= " + " '" + lworkDate + " '"
                        + " and wd.Status != 'Pending' ";
            }
            else if (lworkDate == "" && lEmpId != "")
            {
                qry = "  SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
         + " FROM workdiary_det workdet"
        + " WHERE workdet.wdid =" + " wd.Id" +
        " FOR XML PATH('')), 1, 2, ''),"
        + " [Desc] = STUFF((SELECT ', ' + [Desc]"
        + " FROM workdiary_det workdet" +
        " WHERE workdet.wdid =" + " wd.Id"
        + " FOR XML PATH('')), 1, 2, '')," +
               "wd.WDDate,wd.Status"

        + " FROM WorkDiary wd "
        + "  join Employees ep on wd.EmpId = ep.EmpId"
        + " join Designations ds on ds.Id = ep.CurrentDesignation "
        + " join Departments dp on dp.Id = ep.Department"
        + " join Branches br on br.Id = ep.Branch"
        + " join workdiary_det workdet on wd.Id = workdet.WDId "
        + "WHERE wd.EmpId= " + lEmpId
         + " and wd.Status != 'Pending' ";
            }
            else if (lworkDate != "" && lEmpId != "")
            {
                qry = "  SELECT DISTINCT wd.EmpId,ep.ShortName,ds.Code as Designation, Name =STUFF((SELECT ', ' + name"
                         + " FROM workdiary_det workdet"
                        + " WHERE workdet.wdid =" + " wd.Id" +
                        " FOR XML PATH('')), 1, 2, ''),"
                        + " [Desc] = STUFF((SELECT ', ' + [Desc]"
                        + " FROM workdiary_det workdet" +
                        " WHERE workdet.wdid =" + " wd.Id"
                        + " FOR XML PATH('')), 1, 2, '')," +
               "wd.WDDate,wd.Status"
                        + " FROM WorkDiary wd "
                        + "  join Employees ep on wd.EmpId = ep.EmpId"
                        + " join Designations ds on ds.Id = ep.CurrentDesignation "
                        + " join Departments dp on dp.Id = ep.Department"
                        + " join Branches br on br.Id = ep.Branch"
                        + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "WHERE wd.EmpId= " + lEmpId + " and " + "wd.WDDate='" + lworkDate + "' and wd.Status !='Pending'";
            }
            return sh.Get_Table_FromQry(qry);
        }

        public IList<YrLeaveBal> getEmpLeaveReport(string EmpId)
        {
            //string lMessage = string.Empty;
            var ballist = new List<YrLeaveBal>();
            try
            {
                Log.Debug("Employee year wise balance Code begins here!!!");
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

                SqlCommand cmd = new SqlCommand("EmployeeYearWiseLeavebalance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@empid1", SqlDbType.VarChar);
                cmd.Parameters["@empid1"].Value = 0;

                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                Log.Debug("Employee year wise balance connction open here!!! line no :992");
                con.Open();
                cmd.ExecuteNonQuery();
                Log.Debug("Employee year wise balance sp execute here!!!");
                da.Fill(dt);
                Log.Debug("Employee year wise balance data fill here!!!" + dt);
                con.Close();

                string empid = "0";
                YrLeaveBal empbal = new YrLeaveBal();
                Log.Debug("Employee year wise balance foreach loop here!!!");

                foreach (DataRow dr in dt.Rows)
                {
                    Log.Debug("Employee year wise balance foreach loop here!!!");
                    if (empid != dr["EmpId"].ToString())
                    {
                        //new emp
                        Log.Debug("Employee year wise balance if loop here!!!");
                        empid = dr["EmpId"].ToString();
                        if (empid != "0")
                        {
                            empbal = new YrLeaveBal();
                            ballist.Add(empbal);
                        }
                        empbal.EmpId = dr["EmpId"].ToString();
                        empbal.EmpName = dr["EmpName"].ToString();
                        empbal.Designation = dr["Designation"].ToString();
                        empbal.BrDept = dr["BrDept"].ToString();
                        empbal.Year = dr["Year"].ToString();
                    }

                    // cl
                    if (dr["LeaveType"].ToString() == "1")
                    {
                        empbal.CarryForwardFLeaves = dr["CarryForwardFLeaves"].ToString();
                        empbal.CreditLeaves = dr["CreditLeaves"].ToString();
                        empbal.Debits = dr["Debits"].ToString();
                        empbal.LeaveBalance = dr["LeaveBalance"].ToString();
                    }
                    // ml
                    else if (dr["LeaveType"].ToString() == "2")
                    {
                        empbal.TotalMedicalSickLeave = dr["LeaveBalance"].ToString();
                        empbal.CarryForwardML = dr["CarryForwardFLeaves"].ToString();
                        empbal.ConsumedML = dr["CreditLeaves"].ToString();
                        empbal.RemainingML = dr["Debits"].ToString();

                    }
                    // pl
                    else if (dr["LeaveType"].ToString() == "3")
                    {
                        empbal.TotalPrivilegeLeave = dr["CarryForwardFLeaves"].ToString();
                        empbal.ConsumedPL = dr["CreditLeaves"].ToString();
                        empbal.RemainingPL = dr["Debits"].ToString();
                        empbal.CarryForwardPL = dr["LeaveBalance"].ToString();
                    }
                    // mtl
                    else if (dr["LeaveType"].ToString() == "4")
                    {
                        empbal.TotalMaternityLeave = dr["CarryForwardFLeaves"].ToString();
                        empbal.ConsumedMTL = dr["CreditLeaves"].ToString();
                        empbal.RemainingMTL = dr["Debits"].ToString();
                        empbal.CarryForwardMTL = dr["LeaveBalance"].ToString();
                    }
                    // ptl
                    else if (dr["LeaveType"].ToString() == "5")
                    {
                        empbal.TotalPaternityLeave = dr["CarryForwardFLeaves"].ToString();
                        empbal.ConsumedPTL = dr["CreditLeaves"].ToString();
                        empbal.RemainingPTL = dr["Debits"].ToString();
                        empbal.CarryForwardPTL = dr["LeaveBalance"].ToString();
                    }
                    // eol
                    else if (dr["LeaveType"].ToString() == "6")
                    {
                        empbal.TotalExtraordinaryLeave = dr["CarryForwardFLeaves"].ToString();
                        empbal.ConsumedEOL = dr["CreditLeaves"].ToString();
                        empbal.RemainingEOL = dr["Debits"].ToString();
                        empbal.CarryForwardEOL = dr["LeaveBalance"].ToString();
                    }
                    // scl
                    else if (dr["LeaveType"].ToString() == "7")
                    {
                        empbal.TotalSpecialCasualLeave = dr["CarryForwardFLeaves"].ToString();
                        empbal.ConsumedSCL = dr["CreditLeaves"].ToString();
                        empbal.RemainingSCL = dr["Debits"].ToString();
                        empbal.CarryForwardSCL = dr["LeaveBalance"].ToString();
                    }
                    //C-OFF
                    else if (dr["LeaveType"].ToString() == "13")
                    {
                        empbal.TotalCOFFLeave = dr["CarryForwardFLeaves"].ToString();
                        empbal.ConsumedCOFF = dr["CreditLeaves"].ToString();
                        empbal.RemainingCOFF = dr["Debits"].ToString();
                        empbal.CarryForwardCOFF = dr["LeaveBalance"].ToString();
                    }
                    //LOP
                    else if (dr["LeaveType"].ToString() == "12")
                    {
                        //empbal.TotalLOPLeave = dr["CarryForwardFLeaves"].ToString();
                        //empbal.ConsumedLOP = dr["CreditLeaves"].ToString();
                        //empbal.RemainingLOP = dr["Debits"].ToString();
                        empbal.CarryForwardLOP = dr["LeaveBalance"].ToString();
                    }
                    //W-Off
                    else if (dr["LeaveType"].ToString() == "16")
                    {
                        //empbal.TotalLOPLeave = dr["CarryForwardFLeaves"].ToString();
                        //empbal.ConsumedLOP = dr["CreditLeaves"].ToString();
                        //empbal.RemainingLOP = dr["Debits"].ToString();
                        empbal.CarryForwardLOP = dr["LeaveBalance"].ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Debug("catch block" + ex);
            }

            return ballist;


        }
        public DataTable getAllSeniority()
        {
            string qry = "    SELECT " +
                "e.EmpId,e.ShortName AS EmpName, " +
                "dsg.code AS Designation,  " +
                "case when br.name = 'OtherBranch' then dept.name else br.name end  AS BranchDepartmet, " +
                "CONVERT(varchar(10), e.DOJ, 103) AS DOJ, " +
                    " DATEDIFF(year, e.DOJ, GETDATE()) AS YearsOfService " +
                "FROM Employees e " +
                "LEFT JOIN Designations dsg ON dsg.Id = e.CurrentDesignation " +
                "LEFT JOIN Branches br ON br.Id = e.Branch " +
                "LEFT JOIN Departments dept ON dept.Id = e.Department " +
                "WHERE e.retirementdate >= getdate() AND e.currentdesignation in (1,2,3,4,5,6) ORDER BY currentdesignation ASC ";

            return sh.Get_Table_FromQry(qry);

        }
        public DataTable getAllSeniorityList(int desiId)
        {
            string cond1 = "";
            // 0 for all rows, -1 for no rows
            if (desiId > 0 )
            {
                cond1 = " AND e.currentdesignation=" + desiId;
            }

            string qry = "SELECT " +
                "e.EmpId,e.ShortName AS EmpName, " +
                "dsg.code AS Designation,  " +
                "case when br.name = 'OtherBranch' then dept.name else br.name end  AS BranchDepartmet, " +
                "CONVERT(varchar(10), e.DOJ, 103) AS DOJ, " +
                    " DATEDIFF(year, e.DOJ, GETDATE()) AS YearsOfService " +
                "FROM Employees e " +
                "LEFT JOIN Designations dsg ON dsg.Id = e.CurrentDesignation " +
                "LEFT JOIN Branches br ON br.Id = e.Branch " +
                "LEFT JOIN Departments dept ON dept.Id = e.Department " +
                "WHERE e.retirementdate >= getdate()  "+cond1+ "" +
                "ORDER BY e.DOJ ASC "; 

            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getAllCadreList(int desiId)
        {
            string cond1 = "";
            // 0 for all rows, -1 for no rows
            if (desiId != 0)
            {
                cond1 = " AND e.currentdesignation=" + desiId;
            }

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id," +
                         " v.EmpId,v.EmpName,v.Name,d.code as Designations, e.currentdesignation as DesgID, case when b.id = 43 then dp.name else b.name end as BranchDepartmet,CONVERT(VARCHAR, e.DOJ, 103) as DOJ," +
                         " CONVERT(VARCHAR, e.RetirementDate, 103) as RetirementDate" +
                         " from view_employee_senioritylist v join employees e on v.empid = e.empid" +
                         " join designations d on e.CurrentDesignation = d.id join branches b on e.Branch = b.id join departments dp on e.department = dp.id" +
                         " where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " + cond1
                         + " order by v.designations";

            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getAllEmpDob(int dobmid)
        {
            string cond1 = "";
            // 0 for all rows, -1 for no rows
            if (dobmid != 0)
            {
                cond1 = " AND  MONTH(E.DOB)=" + dobmid;
            }

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id," +
                         " v.EmpId,v.EmpName,v.code as Designations, CONVERT(VARCHAR, DOB, 103) as DOB, MONTH(E.DOB) as dobmid, YEAR(E.DOB) AS Year, year(getdate()) as PresentYear," +
                         " (year(getdate()) - year(dob)) as Age,month(e.dob) as MonthDOB" +
                         " from view_employee_DOB_RetirementDateMonthWise v join employees e on v.empid = e.empid" +
                         " join branches b on e.Branch = b.id join departments dp on e.department = dp.id" +
                         " where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " + cond1
                         + " order by month(e.dob),year(e.dob) ,v.designations";

            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getAllKeyofficials()
        {

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id," +
                        " e.EmpId,e.ShortName as Name,d.code as Designation,case when b.id = 43 then dp.name else b.name end as BranchDepartmet," +
                        " e.Graduation,E.PostGraduation,E.ProfessionalQualifications,e.TotalExperience from   employees e  join" +
                        " designations d on e.CurrentDesignation = d.id" +
                        " join departments dp on e.department = dp.id join branches b on e.branch = b.id where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))  and" +
                        " (d.code = 'MD' or d.code = 'CGM' or d.code = 'DGM' or d.code = 'GM')";

            return sh.Get_Table_FromQry(qry);

        }
        public DataTable getLeavesCarryForward(string srchid, string ofcrid)
        {
            string cond1 = "";

            //// 0 for all rows, -1 for no rows
            if (srchid == "-1" && ofcrid == "-2")
            {
                string lstatus = "'-1'";
                cond1 = "and" + "(" + "lt.Status =" + lstatus + ")";
            }
            else if (srchid != "-1" && ofcrid == "-2" && srchid != "All")
            {
                cond1 = "and" + "(" + "lt.Status ='" + srchid + "'" + ")" + "order by e.empid";
            }
            else if (srchid == "All" && ofcrid == "0")
            {
                cond1 = "and" + "(" + "lt.status = 'Forwarded' or lt.Status = 'Approved'" + ")" + " order by e.empid";
            }
            else if (srchid == "Forwarded" && ofcrid == "0")
            {
                cond1 = "and" + "(" + "lt.status = 'Forwarded'" + ")" + "  order by e.empid";
            }
            else if (srchid == "Approved" && ofcrid == "0")
            {
                cond1 = "and" + "(" + "lt.Status = 'Approved'" + ")" + " order by e.empid";
            }
            else if (srchid != "All" && srchid != "-1" && srchid != string.Empty && ofcrid == string.Empty)
            {
                cond1 = " and" + "(" + "lt.Status = '" + srchid + "'" + ")" + " order by e.empid";
            }
            else if (ofcrid != "All" && ofcrid != "-1" && ofcrid != "0" && srchid == string.Empty)
            {
                cond1 = " and lt.UpdatedBy = '" + ofcrid + "'" + " order by e.empid";
            }
            else if (srchid == "Forwarded" && ofcrid != "All" && ofcrid != "-1" && ofcrid != "0")
            {
                cond1 = "and" + "(" + "lt.status = 'Forwarded' and lt.UpdatedBy = '" + ofcrid + "'" + ")" + " order by e.empid";
            }

            else if (srchid == "Approved" && ofcrid != "All" && ofcrid != "-2" && ofcrid != "0")
            {
                cond1 = "and" + "(" + "lt.status = 'Approved' and lt.UpdatedBy = '" + ofcrid + "'" + ")" + " order by e.empid";
            }
            else if (srchid == "-1" && ofcrid != "-2" && ofcrid != string.Empty && ofcrid != "0")
            {
                cond1 = "and" + "(" + "lt.UpdatedBy = '" + ofcrid + "'  and (lt.status = 'Forwarded' or lt.Status = 'Approved')" + ")" + "  order by e.empid";
            }
            else if (srchid == "All" && ofcrid != "-2" && ofcrid != string.Empty)
            {
                cond1 = "and" + "(" + "lt.UpdatedBy = '" + ofcrid + "'  and (lt.status = 'Forwarded' or lt.Status = 'Approved')" + ")" + "  order by e.empid";
            }
            else if (srchid == "All" && ofcrid == "-2")
            {
                cond1 = "and" + "(" + "lt.status = 'Forwarded' or lt.Status = 'Approved'" + ")" + " order by e.empid";
            }
            else if (srchid == "-1" && ofcrid == "0")
            {
                cond1 = "and" + "(" + "lt.status = 'Forwarded' or lt.Status = 'Approved'" + ")" + " order by e.empid";
            }

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id," +
                            " e.EmpId,e.ShortName as Name,d.code as Designation," +
                           " case when b.id = 43 then dp.name else b.name end as BranchDepartmet,(select empid from employees where empid=lt.UpdatedBy) as ApprovedBy," +
                            " ApprovedName = (select  shortname from employees where  empid = lt.UpdatedBy)," +
                            " concat((convert(varchar, lt.UpdatedDate, 103)), ' - ', CONVERT(char(5), lt.updateddate, 108)) as DateTime," +
                            " lt.Status from   employees e  join designations d on e.CurrentDesignation = d.id" +
                            " join departments dp on e.department = dp.id join branches b on e.branch = b.id join" +
                            " Leaves lt on lt.empid = e.id join leavetypes lp on lp.id = lt.LeaveType " + "where e.RetirementDate>=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))"
                              //" lt.status = 'Forwarded' or lt.Status = 'Approved'";
                              + cond1;

            return sh.Get_Table_FromQry(qry);

        }
        public DataTable getHeadofcAttenders(int DptId, string EmpCode)
        {
            string cond1 = "";

            // 0 for all rows, -1 for no rows
            if (DptId != 0 && EmpCode == "-2" && DptId != -1)
            {
                cond1 = " and  dp.id =" + DptId;
            }
            else if (EmpCode != "-2" && DptId == 0)
            {
                cond1 = " and  e.empid =" + EmpCode;
            }
            else if (DptId != 0 && EmpCode != "-2" && DptId != -1)
            {
                cond1 = " and  e.empid =" + EmpCode + " and dp.id =" + DptId;
            }
            else if (DptId == -1 && EmpCode == "-2")
            {
                cond1 = " and  e.empid =" + EmpCode + " and dp.id =" + DptId;
            }
            else if (DptId == -1 && EmpCode != "")
            {
                cond1 = "and  e.empid =" + EmpCode;
            }
            else
            {
                cond1 = "";
            }
            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id," +
                               " v.EmpId,e.ShortName as Name,d.code as Designation,v.deptname as Department, dp.id as DptId" +
                               " from view_employee_dept v join employees e on v.empid = e.empid join designations d on e.CurrentDesignation = d.id" +
                               " join departments dp  on e.department = dp.id where dp.name != 'OtherDepartment' and e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))  and d.code = 'ATTD'"
                               + cond1
                               + " order by e.empid";

            return sh.Get_Table_FromQry(qry);
        }

        public DataTable headOffice(int DptId)
        {
            string cond1 = "";
            if (DptId != 0)
            {
                cond1 = " where  dp.id =" + DptId;

            }
            else
            {
                cond1 = "and e.Branch=43";
            }

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id, " +
                        " v.EmpId,e.ShortName as Name,d.code as Designation,case when b.id = 43 then dp.name else b.name end as BranchDepartmet, dp.id as DptId" +
                        " from view_employee_dept v join employees e on v.empid = e.empid join designations d on e.CurrentDesignation = d.id " +
                        " join branches b on e.Branch = b.id join departments dp " +
                        " on e.department = dp.id and e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and e.EmpId != 123456 " + cond1;
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable MonthWiseLeaveData(string monthid)
        {

            string cond1 = "";
            // 0 for all rows, -1 for no rows
            if (monthid != "0")
            {
                cond1 = " and" + "(" + "RIGHT(CONVERT(VARCHAR(11), lt.StartDate, 106), 8)='" + monthid + "'or  RIGHT(CONVERT(VARCHAR(11), lt.StartDate, 106), 8)='" + monthid + "')";
            }

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id, " +
                            " e.EmpId,e.ShortName as Name,d.code as Designation, " +
                            "  case when b.id = 43 then dp.name else b.name end as BranchDepartmet, CONVERT(VARCHAR, DOB, 103) as DOB, MONTH(E.DOB) as dobmid," +
                            " CONVERT(VARCHAR, lt.UpdatedDate, 103) as AppliedDate, MONTH(lt.UpdatedDate) as AdMonth, CONVERT(varchar, lt.updateddate, 108) as AppliedTime, " +
                             " CONVERT(VARCHAR, lt.StartDate, 103) as StartDate, MONTH(lt.StartDate) as SdMonth, CONVERT(VARCHAR, lt.EndDate, 103) as EndDate,  MONTH(lt.EndDate) as EdMonth, lt.LeaveDays, " +
                             " lt.Subject,lt.Reason,lp.code as LeaveType,lt.Status " +
                              " from   employees e  join designations d on e.CurrentDesignation = d.id " +
                              " join departments dp on e.department = dp.id join branches b on e.branch = b.id join " +
                                "  Leaves lt on lt.empid = e.id join leavetypes lp on lp.id = lt.LeaveType " + "where  e.RetirementDate>=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and lt.Status!='Credited' and lt.Status!='Debited'" + cond1;
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable MonthWiseODData(string monthid)
        {

            string cond1 = "";
            // 0 for all rows, -1 for no rows
            if (monthid != "0")
            {
                cond1 = "  and RIGHT(CONVERT(VARCHAR(11), lt.StartDate, 106), 8)='" + monthid + "'and  RIGHT(CONVERT(VARCHAR(11), lt.EndDate,106), 8)='" + monthid + "'";
            }

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id, " +
                            " e.EmpId,e.ShortName as Name,d.code as Designation, " +
                            " case when b.id = 43 then dp.name else b.name end as BranchDepartmet, " +
                             " CONVERT(VARCHAR, lt.StartDate, 103) as StartDate, MONTH(lt.StartDate) as SdMonth, CONVERT(VARCHAR, lt.EndDate, 103) as EndDate, MONTH(lt.EndDate) as EdMonth, VistorFrom=(select name from branches where id=lt.VistorFrom), " +
                             " lt.VistorTo,lt.Description,lt.Status " +
                              " from   employees e  join designations d on e.CurrentDesignation = d.id " +
                              " join departments dp on e.department = dp.id join branches b on e.branch = b.id join " +
                              " OD_OtherDuty lt on lt.empid = e.id  " + " where e.RetirementDate>=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + cond1;
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable staffCategory(string DptId)
        {
            string cond1 = "", cond2 = "";

            if (DptId == "-1")
            {
                // string ltransfer = Convert.ToString(trnasfertype);
                string ltransfer = "'-1";
                cond1 = " and  e.Category =" + ltransfer + "'";
            }
            else if (DptId == "All")
            {
                cond1 = "";
            }

            else
            {
                cond1 = "  and e.Category ='" + DptId + "'";
            }

            string qry = " select CAST(ROW_NUMBER() over(order by branch) AS INT) Id," +
                   " v.EmpId,v.EmpName,d.code as Designations,e.Category,e.Gender" +
                   " from view_employee_category v join employees e on v.empid = e.empid" +
                   " join designations d on e.CurrentDesignation = d.id join branches b on e.Branch = b.id join departments dp on e.department = dp.id" +
                   " where e.RetirementDate>=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " + cond1 + cond2 + " order by e.empid";
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getAllEmpPromotions(string trnasfertype, string EmpCode)
        {
            string cond1 = "", cond2 = "";

            // 0 for all rows, -1 for no rows
            if (trnasfertype == "-1")
            {
                // string ltransfer = Convert.ToString(trnasfertype);
                string ltransfer = "'-1'";
                cond1 = " and  EmpTrans.type =" + ltransfer;
            }
            else if (trnasfertype != "-1" && EmpCode != "-2" && trnasfertype != "All" && trnasfertype != "0")
            {
                cond1 = " and  e.EmpId ='" + EmpCode + "' and  EmpTrans.type ='" + trnasfertype + "' order by e.empid";
            }
            else if (trnasfertype != "-1" && EmpCode != "-2" && trnasfertype == "All")
            {
                cond1 = " and  e.EmpId ='" + EmpCode + "' and  (EmpTrans.type ='Promotion' or   EmpTrans.type ='PromotionTransfer') order by e.empid";
            }
            else if (EmpCode != "-2" && trnasfertype == "0")
            {
                cond1 = " and  e.EmpId ='" + EmpCode + "'  order by e.empid";
            }
            else if (EmpCode != "-2" && trnasfertype == "All")
            {
                cond1 = " and  e.EmpId ='" + EmpCode + "' and  EmpTrans.type ='Promotion' or   EmpTrans.type ='Transfer' order by e.empid";
            }
            else if (EmpCode == "-2" && trnasfertype != "All" && trnasfertype != "-1")
            {
                cond1 = " and  EmpTrans.type = '" + trnasfertype + "' order by e.empid";

            }
            string qry = "select distinct e.EmpId,e.ShortName as Name,EmpTrans.Type,case when EmpTrans.oldbranch = 43 then(select name from Departments where id = EmpTrans.OldDepartment)  else (select name from branches where id = EmpTrans.OldBranch) end as OldDepartmentBranch, " +
                " case when EmpTrans.NewBranch = 43 then(select name from Departments where id = EmpTrans.NewDepartment)  else (select name from branches where id = EmpTrans.NewBranch) end as NewDepartmentBranch,(select Code from Designations where id = EmpTrans.OldDesignation) as OldDesignation,   (select Code from Designations where id = EmpTrans.NewDesignation) as NewDesignation, " +
                " convert(varchar, EmpTrans.EffectiveFrom, 103) as EffectiveFrom,convert(varchar, EmpTrans.EffectiveTo, 103) as EffectiveTo, " +
                " EmpTrans.old_basic as old,case when Emptrans.new_basic is null then null else pay.amount end as new from Employee_Transfer EmpTrans join Employees E on EmpTrans.empId = E.Id join Designations D on D.Id = EmpTrans.OldDesignation " +
                " join pr_emp_pay_field pay on EmpTrans.EmpId = pay.emp_id and pay.m_type = 'Pay_fields' and pay.m_id = 11 and pay.active = 1 join pr_earn_field_master pearn on pearn.id = pay.m_id and pearn.active = 1 " +
                " where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and(EmpTrans.authorisation = 1 or EmpTrans.authorisation = 0 or EmpTrans.authorisation is null) and EmpTrans.Type not in ('PermanentTransfer','TemporaryTransfer') "
                + cond1;

            //string qry = " select distinct " +
            //                " e.EmpId,e.ShortName as Name,LT.Type," +
            //                " case when lt.oldbranch=43 then (select name from Departments where id=lt.OldDepartment) else" +
            //                " (select name from branches where id=lt.OldBranch) end as OldDepartmentBranch," +
            //                " case when lt.NewBranch=43 then (select name from Departments where id=lt.NewDepartment) else" +
            //                " (select name from branches where id=lt.NewBranch) end as NewDepartmentBranch," +
            //                " (select Code from Designations where id=lt.OldDesignation) as OldDesignation," +
            //                " (select Code from Designations where id=lt.NewDesignation) as NewDesignation," +
            //                " convert(varchar,lt.EffectiveFrom,103) as EffectiveFrom,convert(varchar,lt.EffectiveTo,103) as EffectiveTo ,  lt.new_basic  as new,amount as old from   employees e  join   designations d on e.CurrentDesignation=d.id" +
            //                " join departments dp  on e.department=dp.id join branches b on e.branch=b.id join Employee_Transfer lt on lt.empid=e.id " +
            //               //  " join Employee_Transfer p on p.EmpId=e.Id" +
            //               " join pr_emp_pay_field pay on lt.EmpId = pay.emp_id and pay.m_type='Pay_fields'" +
            //               "  join pr_earn_field_master pearn on pearn.id=pay.m_id and pearn.active=1 " +
            //                "  where  lt.Type in ('Promotion','PromotionTransfer') and e.RetirementDate>=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and lt.authorisation=1  "
            //                  + cond1;
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable MnthWiseTempTrsfrItemsdata(string monthid)
        {

            string cond1 = "";
            // 0 for all rows, -1 for no rows
            if (monthid != "0")
            {
                cond1 = "  and RIGHT(CONVERT(VARCHAR(11), lt.EffectiveFrom,106), 8)='" + monthid + "'";
            }

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id, " +
                            " e.EmpId,e.ShortName as Name,LT.Type, " +
                            " case when lt.oldbranch = 43 then(select name from Departments where id = lt.OldDepartment) else " +
                            " (select name from branches where id = lt.OldBranch) end as OldDepartmentBranch," +
                            " case when lt.NewBranch = 43 then(select name from Departments where id = lt.NewDepartment) else " +
                            " (select name from branches where id = lt.NewBranch) end as NewDepartmentBranch," +
                            "convert(varchar,lt.EffectiveFrom,103) as EffectiveFrom,convert(varchar,lt.EffectiveTo,103) as EffectiveTo from   employees e  join designations d on e.CurrentDesignation = d.id " +
                            " join departments dp on e.department = dp.id join branches b on e.branch = b.id join " +
                            " Employee_Transfer lt on lt.empid = e.id  where lt.Type = 'TemporaryTransfer' and e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " + cond1;
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable PrivilegeLeaveItemsdata(string DptId, string EmpCode, string fdate, string tdate)
        {
            string cond1 = "";
            string year1 = "";
            string year2 = "";
            if (fdate != "-3" && tdate != "-4")
            {
                DateTime dt1 = fdate == "" ? DateTime.Now : Convert.ToDateTime(fdate);
                DateTime dt2 = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                fdate = fdate;
                tdate = tdate;

                year1 = dt1.ToString("yyyy");
                year2 = dt2.ToString("yyyy");
            }
            // 0 for all rows, -1 for no rows
            if (DptId == "-1" && EmpCode == "-2" && fdate == "-3" && tdate == "-4")
            {
                cond1 = " and lt.Status = '-1'";
            }

            else if (DptId != "-1" && EmpCode == "-2" && DptId != "All" && DptId != "0" && fdate != "-3" && tdate != "-4")
            {
                //DateTime dt1 = fdate == "" ? DateTime.Now : Convert.ToDateTime(fdate);
                //DateTime dt2 = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                cond1 = " and (convert(varchar,lt.UpdatedDate,103) >= '" + fdate + "' and convert(varchar,lt.UpdatedDate,103) <= '" + tdate + "'  ) and ((lt.CurrentYear between '" + year1 + "' and '" + year2 + "')or (SubString(lt.CurrentYear,1,4)='" + year1 + "' or SubString(lt.CurrentYear,1,4)='" + year2 + "' or SubString(lt.CurrentYear,6,4)='" + year1 + "' or SubString(lt.CurrentYear,6,4)='" + year2 + "')) order by e.empid";
            }

            //all
            else if (EmpCode == "-2" && DptId == "All" && fdate != "-3" && tdate != "-4")
            {
                //DateTime dt1 = fdate == "" ? DateTime.Now : Convert.ToDateTime(fdate);
                //DateTime dt2 = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                cond1 = " and (convert(varchar,lt.UpdatedDate,103) >= '" + fdate + "' and convert(varchar,lt.UpdatedDate,103)  <= '" + tdate + "') and ((lt.CurrentYear between '" + year1 + "' and '" + year2 + "')or (SubString(lt.CurrentYear,1,4)='" + year1 + "' or SubString(lt.CurrentYear,1,4)='" + year2 + "' or SubString(lt.CurrentYear,6,4)='" + year1 + "' or SubString(lt.CurrentYear,6,4)='" + year2 + "')) order by e.empid";
            }
            else if (DptId != "-1" && EmpCode != "-2" && DptId != "All" && DptId != "0" && fdate != "-3" && tdate != "-4")
            {
                //DateTime dt1 = fdate == "" ? DateTime.Now : Convert.ToDateTime(fdate);
                //DateTime dt2 = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                cond1 = " and (convert(varchar,lt.UpdatedDate,103) >= '" + fdate + "' and convert(varchar,lt.UpdatedDate,103)  <= '" + tdate + "' )" + " and ((lt.CurrentYear between '" + year1 + "' and '" + year2 + "')or (SubString(lt.CurrentYear,1,4)='" + year1 + "' or SubString(lt.CurrentYear,1,4)='" + year2 + "' or SubString(lt.CurrentYear,6,4)='" + year1 + "' or SubString(lt.CurrentYear,6,4)='" + year2 + "')) and  e.EmpId ='" + EmpCode + "'  order by e.empid";
            }

            //empcode
            else if (EmpCode != "-2" && DptId == "All" && fdate != "-3" && tdate != "-4")
            {
                //DateTime dt1 = fdate == "" ? DateTime.Now : Convert.ToDateTime(fdate);
                //DateTime dt2 = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                cond1 = " and (convert(varchar,lt.UpdatedDate,103)  >= '" + fdate + "' and convert(varchar,lt.UpdatedDate,103)  <= '" + tdate + "')  and ((lt.CurrentYear between '" + year1 + "' and '" + year2 + "')or (SubString(lt.CurrentYear,1,4)='" + year1 + "' or SubString(lt.CurrentYear,1,4)='" + year2 + "' or SubString(lt.CurrentYear,6,4)='" + year1 + "' or SubString(lt.CurrentYear,6,4)='" + year2 + "')) and  e.EmpId =" + EmpCode + "  order by e.empid";
            }
            else if (EmpCode == "-2" && DptId != "All" && fdate != "-3" && tdate != "-4")
            {
                //DateTime dt1 = fdate == "" ? DateTime.Now : Convert.ToDateTime(fdate);
                //DateTime dt2 = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                cond1 = " and (convert(varchar,lt.UpdatedDate,103)  >= '" + fdate + "' and convert(varchar,lt.UpdatedDate,103)  <= '" + tdate + "') and ((lt.CurrentYear between '" + year1 + "' and '" + year2 + "')or (SubString(lt.CurrentYear,1,4)='" + year1 + "' or SubString(lt.CurrentYear,1,4)='" + year2 + "' or SubString(lt.CurrentYear,6,4)='" + year1 + "' or SubString(lt.CurrentYear,6,4)='" + year2 + "')) and e.EmpId = '" + EmpCode + "'" +
                   " order by e.empid";
            }
            else if (DptId != "-1" && EmpCode != "-2" && DptId == "All" && fdate != "-3" && tdate != "-4")
            {
                //DateTime dt1 = fdate == "" ? DateTime.Now : Convert.ToDateTime(fdate);
                //DateTime dt2 = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                cond1 = " and (convert(varchar,lt.UpdatedDate,103)  >= '" + fdate + "' or convert(varchar,lt.UpdatedDate,103)  <= '" + tdate + "') and ((lt.CurrentYear between '" + year1 + "' and '" + year2 + "')or (SubString(lt.CurrentYear,1,4)='" + year1 + "' or SubString(lt.CurrentYear,1,4)='" + year2 + "' or SubString(lt.CurrentYear,6,4)='" + year1 + "' or SubString(lt.CurrentYear,6,4)='" + year2 + "')) and  e.EmpId ='" + EmpCode + "'and  e.EmpId ='" + EmpCode + "') " +
                   " order by e.empid";
            }
            else if (EmpCode != "-2" && DptId == "0" && fdate == "-3" && tdate == "-4")
            {
                cond1 = " and  e.EmpId ='" + EmpCode + "'  order by e.empid";
            }
            else if (EmpCode != "-2" && DptId == "All")
            {
                //DateTime dt1 = fdate == "" ? DateTime.Now : Convert.ToDateTime(fdate);
                //DateTime dt2 = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                cond1 = " and (convert(varchar,lt.UpdatedDate,103)  >= '" + fdate + "' or convert(varchar,lt.UpdatedDate,103)  <= '" + tdate + "')  and ((lt.CurrentYear between '" + year1 + "' and '" + year2 + "')or (SubString(lt.CurrentYear,1,4)='" + year1 + "' or SubString(lt.CurrentYear,1,4)='" + year2 + "' or SubString(lt.CurrentYear,6,4)='" + year1 + "' or SubString(lt.CurrentYear,6,4)='" + year2 + "')) and  e.EmpId ='" + EmpCode + "'  order by e.empid";
            }
            else if (EmpCode == "-2" && DptId != "All" && DptId != "-1" && fdate != "-3" && tdate != "-4")
            {
                //DateTime dt1 = fdate == "" ? DateTime.Now : Convert.ToDateTime(fdate);
                //DateTime dt2 = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                cond1 = " and (convert(varchar,lt.UpdatedDate,103)  >= '" + fdate + "' or convert(varchar,lt.UpdatedDate,103)  <= '" + tdate + "') and ((lt.CurrentYear between '" + year1 + "' and '" + year2 + "')or (SubString(lt.CurrentYear,1,4)='" + year1 + "' or SubString(lt.CurrentYear,1,4)='" + year2 + "' or SubString(lt.CurrentYear,6,4)='" + year1 + "' or SubString(lt.CurrentYear,6,4)='" + year2 + "')) order by e.empid";
            }


            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id, convert(varchar,lt.UpdatedDate,103) as Udate, lt.CurrentYear  as CYear, " +
                               " e.EmpId,e.ShortName as Name,d.code as Designation,lt.TotalExperience,lt.TotalPL,lt.PLEncash,lt.Subject,lt.Status " +
                               "   from  PLE_Type lt join designations d on lt.DesignationId = d.id " +
                                " join departments dp on lt.DepartmentId = dp.id join branches b on lt.BranchId = b.id " +
                                 "  join Employees e on e.id=lt.empid  where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))  "
                               + cond1;



            return sh.Get_Table_FromQry(qry);
        }


        public DataTable LongLeavesItemsdata(string fromDate, string toDate, string leavecnt)
        {
            string cond1 = "";
            string qry = "";

            DateTime fdt;
            DateTime tdt;

            if (fromDate != "" && toDate != "")
            {
                fdt = Convert.ToDateTime(fromDate);
                tdt = Convert.ToDateTime(toDate);

                if (leavecnt == "10-20")
                {

                    cond1 = "  and (( DATEDIFF(dd,lt.StartDate,lt.EndDate) + 1) <= 20 and ( DATEDIFF(dd,lt.StartDate,lt.EndDate) + 1) >= 10) and (lt.StartDate between '" + fdt + "' and '" + tdt + "' or lt.EndDate between '" + fdt + "' and '" + tdt + "')";
                }
                else
                {
                    cond1 = "  and ( DATEDIFF(dd,lt.StartDate,lt.EndDate) + 1) > 20 and (lt.StartDate between '" + fdt + "' and '" + tdt + "' or lt.EndDate between '" + fdt + "' and '" + tdt + "')";
                }
                qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id, " +
                               "  e.EmpId,e.ShortName as Name,d.code as Designation," +
                               "  case when b.id = 43 then dp.name else b.name end as BranchDepartmet, " +
                               "  CONVERT(VARCHAR, lt.StartDate, 103) as StartDate,CONVERT(VARCHAR, lt.EndDate, 103) as EndDate,lp.code as LeaveType, " +
                               " DATEDIFF(dd,lt.StartDate,lt.EndDate) + 1 as TotalDays," +
                               " lt.Status from   employees e  join designations d on e.CurrentDesignation = d.id " +
                               "  join departments dp on e.department = dp.id join branches b on e.branch = b.id join " +
                               "  Leaves lt on lt.empid = e.id join leavetypes lp on lp.id = lt.LeaveType where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))  " +
                               "" + cond1 + " and lt.status != 'Cancelled' and lt.status != 'Denied' and lt.Status!='Credited' and lt.Status!='Debited'";
            }
            else
            {
                fromDate = "01-01-01";
                toDate = "01-01-01";

                fdt = Convert.ToDateTime(fromDate);
                tdt = Convert.ToDateTime(toDate);
                cond1 = "  and (lt.StartDate between '" + fdt + "' and '" + tdt + "' or lt.EndDate between '" + fdt + "' and '" + tdt + "')  ";

                qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id, " +
                               "  e.EmpId,e.ShortName as Name,d.code as Designation," +
                               "  case when b.id = 43 then dp.name else b.name end as BranchDepartmet, " +
                               "  CONVERT(VARCHAR, lt.StartDate, 103) as StartDate,CONVERT(VARCHAR, lt.EndDate, 103) as EndDate,lp.code as LeaveType, " +
                               " DATEDIFF(dd,lt.StartDate,lt.EndDate) + 1 as TotalDays," +
                               " lt.Status from   employees e  join designations d on e.CurrentDesignation = d.id " +
                               "  join departments dp on e.department = dp.id join branches b on e.branch = b.id join " +
                               "  Leaves lt on lt.empid = e.id join leavetypes lp on lp.id = lt.LeaveType where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " +
                               "" + cond1 + " and lt.status != 'Cancelled' and lt.status != 'Denied' and lt.Status!='Credited' and lt.Status!='Debited'";
            }
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable LateappliedLeaveList(string fromDate, string toDate)
        {

            string cond1 = "";

            if (fromDate == "-1" && toDate == "-2")
            {
                cond1 = " and (lt.UpdatedDate >= " + fromDate + " and lt.UpdatedDate  <= " + toDate + ")" +
                   " or (lt.UpdatedDate >= " + fromDate + " and lt.UpdatedDate <= " + toDate + ")" + "and lt.Status!='Credited' and lt.Status!='Debited'";
            }
            else if (fromDate != "-1" && toDate != "-2")
            {
                DateTime dt1 = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime dt2 = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " and lt.UpdatedDate >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.UpdatedDate  <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "' and lt.Status!='Credited' and lt.Status!='Debited' and ( (convert(date,lt.UpdatedDate) > lt.StartDate) and ( convert(date,lt.UpdatedDate) > lt.EndDate)) Order by lt.UpdatedDate";


            }
            //else if (fromDate != "-1" && toDate != "-2" && fromDate == toDate)
            //{
            //    DateTime dt1 = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
            //    DateTime dt2 = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

            //    cond1 = " where lt.UpdatedDate >= '" + dt1.ToString("yyyy-MM-dd") + "' Order by lt.UpdatedDate";

            //}


            string qry = " select CAST(ROW_NUMBER() over(order by lt.updateddate) AS INT) Id," +
                           " e.EmpId,e.ShortName as Name,d.code as Designation," +
                            " case when b.id = 43 then dp.name else b.name end as BranchDepartmet,convert(varchar, lt.UpdatedDate, 103) as AppliedDate," +
                            " convert(varchar, lt.StartDate, 103) as StartDate," +
                            " convert(varchar, lt.EndDate, 103) as EndDate,lp.code as LeaveType,lt.Subject,lt.Reason,lt.Status" +
                             " from   employees e  join designations d on e.CurrentDesignation = d.id" +
                            " join departments dp on e.department = dp.id join branches b on e.branch = b.id join" +
                            " Leaves lt on lt.empid = e.id join leavetypes lp on lp.id = lt.LeaveType " + "where e.RetirementDate>=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + cond1;

            return sh.Get_Table_FromQry(qry);
        }



        public DataTable FutureappliedLeaveList(string fromDate, string toDate)
        {

            string cond1 = "";

            if (fromDate == "-1" && toDate == "-2")
            {
                cond1 = " and(lt.UpdatedDate >= " + fromDate + " and lt.UpdatedDate  <= " + toDate + ")" +
                   " or (lt.UpdatedDate >= " + fromDate + " and lt.UpdatedDate <= " + toDate + ")" + "and lt.Status!='Credited' and lt.Status!='Debited'";
            }
            else if (fromDate != "-1" && toDate != "-2")
            {
                DateTime dt1 = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime dt2 = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " and lt.UpdatedDate >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.UpdatedDate  <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "' and lt.Status!='Credited' and lt.Status!='Debited' and (lt.StartDate>lt.UpdatedDate) Order by lt.UpdatedDate";

            }

            string qry = " select CAST(ROW_NUMBER() over(order by lt.updateddate) AS INT) Id," +
                           " e.EmpId as empid,e.ShortName as Name,d.code as Designation," +
                            " case when b.id = 43 then dp.name else b.name end as BranchDepartmet,convert(varchar, lt.UpdatedDate, 103) as AppliedDate," +
                            " convert(varchar, lt.StartDate, 103) as StartDate," +
                            " convert(varchar, lt.EndDate, 103) as EndDate,lp.code as LeaveType,lt.Subject,lt.Reason,lt.Status" +
                             " from   employees e  join designations d on e.CurrentDesignation = d.id" +
                            " join departments dp on e.department = dp.id join branches b on e.branch = b.id join" +
                            " Leaves lt on lt.empid = e.id join leavetypes lp on lp.id = lt.LeaveType " + " where  e.RetirementDate>=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + cond1;

            return sh.Get_Table_FromQry(qry);
        }



        public DataTable CaAndSaOfEmployeesData2(string branch)
        {

            string cond1 = "";

            //if (branchid != 0 && branchid == 42)
            //{
            //    cond1 = "and (bar.name='" + branchid + "'" + "  or dept.name='" + branchid + "')";
            //}
            //else if (branchid != 0 && branchid != 42)
            //{
            //    cond1 = "and (bar.name='" + branchid + "'" + "  or dept.name='" + branchid + "')";
            //}
            //if (branchid != 0 && branchid == 42)
            //{
            //    cond1 += "  And bar.name = 'OtherBranch' ";
            //}
            //else if (branchid != 0 && branchid != 42)
            //{
            //    cond1 += " AND bar.id=" + branchid + " And bar.name != 'OtherBranch'";
            //}
            if (branch == string.Empty)
            {
                cond1 = " and (bar.Name='" + branch + "'" + "  or dept.Name='" + branch + "')";
            }
            if (branch != string.Empty && branch != "All" && branch != "HeadOffice-All")
            {
                cond1 = " and (bar.Name='" + branch + "'" + "  or dept.Name='" + branch + "')";
            }
            else if (branch == "All" && branch != "HeadOffice-All")
            {
                cond1 = "";
            }

            else if (branch == "HeadOffice-All")
            {
                cond1 = " and e1.Department != 46" + " and e1.Branch = 43";
            }

            string qry = "select e1.empid Employee_Code, e1.ShortName Employee_Name, des.Code Designation,bar.id,case when bar.Name='OtherBranch' then dept.Name else bar.Name end as 'Branch/Department'," +
            "e2.empid Controlling_Authority_Code, e2.ShortName Controlling_Authority_Name," +
            "e3.EmpId Sanctioning_Authority_Code, e3.ShortName Sanctioning_Authority_Name from employees e1  join employees e2 on e1.ControllingAuthority = e2.Id " +
            "join employees e3 on e1.SanctioningAuthority = e3.Id " +
             "join Designations des on e1.CurrentDesignation = des.Id " +
            "join Branches bar on e1.branch = bar.Id join Departments dept on e1.Department = dept.Id where e1.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)" + cond1 + " order by bar.Name";



            return sh.Get_Table_FromQry(qry);
        }


        public DataTable Creditdebitleavesdata(string empid, string leaveid, string fdate, string tdate)
        {
            string cond1 = "";
            var fdate1 = "";
            var fdate2 = "";
            if (fdate != null && tdate != null)
            {
                fdate1 = Convert.ToDateTime(fdate).ToString("yyyy-MM-dd");
                fdate2 = Convert.ToDateTime(tdate).ToString("yyyy-MM-dd");
            }
            else
            {
                fdate1 = "";
                fdate2 = "";
            }
            //string qry = "select UpdatedDate,DebitLeave,CreditLeave,LeaveBalance " +
            //    "from Leaves_CreditDebit where empid=(select id from Employees where empid=555) " +
            //    "and LeaveTypeId = 1 and UpdatedDate between '2018-03-27 11:07:22.217' and '2019-05-07 20:16:10.950'";



            if (empid == null && leaveid == null && fdate == null && tdate == null)
            {
                cond1 = "";
            }

            string qry = "select Convert(varchar,UpdatedDate,103) as UpdatedDate,DebitLeave,CreditLeave,LeaveBalance " +
               "from Leaves_CreditDebit where empid=(select id from Employees where empid='" + empid + "') " +
               "and LeaveTypeId ='" + leaveid + "' and UpdatedDate between '" + fdate1 + "' and '" + fdate2 + "'";
            return sh.Get_Table_FromQry(qry);


        }
        public DataTable AllLeavesCancelledList()
        {
            string cond1 = "";
            //if (below)
            //{

            //    cond1 = "lt.totaldays>=10 and lt.totaldays<=20";
            //}
            //else
            //{
            //    cond1 = "lt.totaldays > 20";
            //}
            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id," +
                        " e.EmpId,e.ShortName as Name," +
                        " CONVERT(VARCHAR, lt.StartDate, 103) as StartDate,  lt.status as LStatus, CONVERT(VARCHAR, lt.EndDate, 103) as EndDate," +
                        " (select top 1 shortname from employees where empId = lt.UpdatedBy or id = lt.UpdatedBy) as CancelledBy," +
                        " concat((convert(varchar, lt.UpdatedDate, 103)), ' - ', CONVERT(varchar, lt.updateddate, 108)) as CancelledDateTime ," +
                        " lt.Reason as ReasonForCancelled," +
                        " lt.Stage as LvcancelStage from   employees e  join designations d on e.CurrentDesignation = d.id" +
                        " join departments dp on e.department = dp.id join branches b on e.branch = b.id join" +
                        " Leaves lt on lt.empid = e.id join leavetypes lp on lp.id = lt.LeaveType where lt.status = 'Cancelled' and e.RetirementDate>=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))";
            //" and " +
            //"" + cond1 + " and lt.status != 'Cancelled' and lt.status != 'Denied' ";
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable AllMonthwiseCLMLPLList(string monthid, int leavetype)
        {
            string cond1 = "";

            if (monthid != "0" && leavetype == -2)
            {
                cond1 = "  and RIGHT(CONVERT(VARCHAR(11), lt.UpdatedDate, 106), 8)='" + monthid + "'" + " or RIGHT(CONVERT(VARCHAR(11), lt.UpdatedDate, 106), 8)='" + monthid + "'";
            }
            else if (monthid == "0" && leavetype == -2)
            {
                cond1 = "";
            }
            else if (monthid == "0" && leavetype == 1)
            {
                cond1 = " and lp.Type= 'Casual Leave'";
            }
            else if (monthid == "0" && leavetype == 2)
            {
                cond1 = " and lp.Type= 'Medical/Sick Leave'";
            }
            else if (monthid == "0" && leavetype == 3)
            {
                cond1 = " and lp.Type= 'Privilege Leave'";
            }

            else if (monthid == "0" && leavetype == 1)
            {
                cond1 = "";
            }
            else if (leavetype == 1 && monthid == "-1")
            {
                cond1 = " and lp.Type= 'Casual Leave'";
            }
            else if (leavetype == 3 && monthid == "-1")
            {
                cond1 = " and lp.Type= 'Privilege Leave'";
            }
            else if (leavetype == 2 && monthid == "-1")
            {
                cond1 = " and lp.Type= 'Medical/Sick Leave'";
            }
            else if (leavetype == 0 && monthid != "0")
            {
                cond1 = " and RIGHT(CONVERT(VARCHAR(11), lt.UpdatedDate, 106), 8)='" + monthid + "'";
            }
            else if (leavetype == 0)
            {
                cond1 = "";
            }
            else if (leavetype == 0 || monthid == "0")
            {
                cond1 = "";
            }

            else if (monthid != "0" && monthid != "-1" && leavetype != 0 && leavetype != -2)
            {
                cond1 = " and RIGHT(CONVERT(VARCHAR(11), lt.UpdatedDate, 106), 8)='" + monthid + "' and lt.LeaveType =" + leavetype;
            }
            else
            {

            }

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id," +
                        " e.EmpId,e.ShortName as Name,d.code as Designation," +
                        " case when b.id = 43 then dp.name else b.name end as BranchDepartmet,  lt.LeaveType as Lcode, lp.Type as Ltype, " +
                        " CONVERT(VARCHAR, lt.UpdatedDate, 103) as AppliedDate,CONVERT(VARCHAR, lt.StartDate, 103) as StartDate," +
                        "  replace(DATEDIFF(day, lt.StartDate, lt.UpdatedDate),'-','') AS DiffFromAppliedDate, CONVERT(VARCHAR, lt.EndDate, 103)as EndDate," +
                        " replace(DATEDIFF(day, lt.EndDate, lt.UpdatedDate),'-','') AS DiffEndAppliedDate" +
                        " from employees e  join designations d on e.CurrentDesignation = d.id" +
                        " join departments dp  on e.department = dp.id join branches b on e.branch = b.id join" +
                        " Leaves lt on lt.empid = e.id join leavetypes lp on lp.id = lt.LeaveType and lp.code in ('CL','PL','ML') " + " where e.RetirementDate>=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + cond1;

            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getAllEmpPermanentTransfer(int empCode, string EffDate, string EffTo)
        {
            string cond1 = "";
            //  
            // 0 for all rows, -1 for no rows

            if (empCode == -1 && EffDate == "-2" && EffTo == "-3")
            {
                cond1 = " and  e.EmpId =" + empCode;
            }
            else if (empCode != -1 && EffDate == string.Empty && EffTo == string.Empty)
            {
                cond1 = " and  e.EmpId =" + empCode;
            }
            else if (EffDate != "-2" && EffTo != "-3" && empCode == -1)
            {
                DateTime dt1 = EffDate == "" ? DateTime.Now : Convert.ToDateTime(EffDate);
                DateTime dt2 = EffDate == "" ? DateTime.Now : Convert.ToDateTime(EffTo);
                cond1 = " and ((lt.EffectiveFrom >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.EffectiveFrom <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (lt.EffectiveTo >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.EffectiveTo <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "'))";
            }
            else if (EffDate != "-2" && EffTo != "-3" && empCode != -1)
            {
                DateTime dt1 = EffDate == "" ? DateTime.Now : Convert.ToDateTime(EffDate);
                DateTime dt2 = EffDate == "" ? DateTime.Now : Convert.ToDateTime(EffTo);
                cond1 = " and ((lt.EffectiveFrom >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.EffectiveFrom <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (lt.EffectiveTo >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.EffectiveTo <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "')) and e.EmpId =" + empCode;
            }

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id," +
                           " e.EmpId,e.ShortName as Name,LT.Type," +
                           " case when lt.oldbranch = 43 then(select name from Departments where id = lt.OldDepartment) else" +
                           " (select name from branches where id = lt.OldBranch) end as OldDepartmentBranch," +
                           " case when lt.NewBranch = 43 then(select name from Departments where id = lt.NewDepartment) else " +
                           " (select name from branches where id = lt.NewBranch) end as NewDepartmentBranch," +
                           " (select Code from Designations where id=lt.OldDesignation) as OldDesignation," +
                           " (select Code from Designations where id=lt.NewDesignation) as NewDesignation," +
                           " convert(varchar, lt.EffectiveFrom, 103) as EffectiveFrom from employees e  join designations d on e.CurrentDesignation = d.id" +
                           " join departments dp  on e.department = dp.id join branches b on e.branch = b.id join" +
                           " Employee_Transfer lt on lt.empid = e.id  where  lt.Type='PermanentTransfer' and e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + cond1;
            //+ cond1 + cond2 + "' order by e.empid";
            return sh.Get_Table_FromQry(qry);
        }

        //public DataTable AllSeniortyList()
        //{

        //    string qry = "select CAST(ROW_NUMBER() over(order by department) AS INT) Id," +
        //                  " e.EmpId,e.ShortName as Name,d.code as Designation,case when b.id = 43 then dp.name else b.name end as BranchDepartmet," +
        //                  " convert(varchar, e.DOB, 103) as DOB,convert(varchar, e.DOJ, 103) AS DOJ, convert(varchar, e.RetirementDate, 103) AS RetirementDate" +
        //                  " from employees e  join designations d on e.CurrentDesignation = d.id" +
        //                  " join departments dp  on e.department = dp.id join branches b on e.branch = b.id" +
        //                  " where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) ";
        //    //+ cond1 + cond2 + "' order by e.empid";
        //    return sh.Get_Table_FromQry(qry);
        //}

        public DataTable AllStaffMasterList()
        {

            string qry = " select CAST(ROW_NUMBER() over(order by e.Empid) AS INT) Id," +
                       "  e.EmpId,e.ShortName as Name,d.code as Designation," +
                       " convert(varchar, e.dob, 103) as DOB,e.Gender,convert(varchar, e.DOJ, 103) as DOJ,convert(varchar, e.RetirementDate, 103) as RetirementDate," +
                       " E.FatherName,e.MotherName,e.PresentAddress as preAddress,e.permanentAddress as perAddress,e.MobileNumber," +
                       " e.Category,e.AadharCardNo,e.Graduation,e.PostGraduation,e.ProfessionalQualifications," +
                        " case when b.id = 43 then dp.name else b.name end as PresentWorkPlace,PersonalEmailId,OfficalEmailId,e.EmergencyContactNo,e.EmergencyName  " +
                        // " case when t.oldbranch = 43 then(select name from Departments where id = t.OldDepartment) else" +
                        //  " (select name from branches where id = t.OldBranch) end as TransferandPostingsFrom," +
                        //  " case when t.NewBranch = 43 then(select name from Departments where id = t.NewDepartment) else" +
                        //   " (select name from branches where id = t.NewBranch) end as TransferandPostingsTo,convert(varchar, t.UpdatedDate, 103) as AppliedDate" +
                        " from employees e  join designations d on e.CurrentDesignation = d.id" +
                        " join departments dp  on e.department = dp.id join branches b on e.branch = b.id " +
                        // " left join Employee_Transfer t on t.empid = e.id " +
                        " where e.RetirementDate>=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) ";
            //+ cond1 + cond2 + "' order by e.empid";
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable AllTxHistoryList(string fromDate, string toDate, string LtcType) //string fromdate, string todate, int leavetype
        {
            string cond1 = "";
            if (fromDate == "-1" && toDate == "-2" && LtcType == "-3")
            {
                fromDate = "1900-01-01";
                toDate = "1900-01-01";
                cond1 = " WHERE (Tx_date >= '" + fromDate + "' and Tx_date <= '" + toDate + "')";
            }
            else if (fromDate != "-1" && toDate != "-2" && LtcType == "All")
            {
                DateTime dt1 = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime dt2 = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where Tx_date >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and Tx_date  <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "' Order by Tx_date";
            }
            else if (fromDate != "-1" && toDate != "-2" && LtcType != "All")
            {
                DateTime dt1 = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime dt2 = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where Tx_date >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and Tx_date  <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "' and Tx_type = '" + LtcType + "'  Order by Tx_date";
            }
            string qry = "SELECT Id,Tx_type,Tx_subtype,Tx_By,Tx_On, convert(varchar,Tx_date, 103) AS Tx_date FROM V_Tx_History " + cond1;

            return sh.Get_Table_FromQry(qry);
        }


        public DataTable AllTempTrnsfrList(int empCode, string EffDate, string EffTo)
        {

            string cond1 = "";

            if (empCode == -1 && EffDate == "-2" && EffTo == "-3")
            {
                cond1 = " and  e.EmpId =" + empCode;
            }
            else if (empCode != -1 && EffDate == string.Empty && EffTo == string.Empty)
            {
                cond1 = " and  e.EmpId =" + empCode;
            }
            else if (EffDate != "-2" && EffTo != "-3" && empCode == -1)
            {
                DateTime dt1 = EffDate == "" ? DateTime.Now : Convert.ToDateTime(EffDate);
                DateTime dt2 = EffDate == "" ? DateTime.Now : Convert.ToDateTime(EffTo);
                cond1 = " and ((lt.EffectiveFrom >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.EffectiveFrom <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (lt.EffectiveTo >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.EffectiveTo <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "'))";
            }
            else if (EffDate != "-2" && EffTo != "-3" && empCode != -1)
            {
                DateTime dt1 = EffDate == "" ? DateTime.Now : Convert.ToDateTime(EffDate);
                DateTime dt2 = EffDate == "" ? DateTime.Now : Convert.ToDateTime(EffTo);
                cond1 = " and ((lt.EffectiveFrom >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.EffectiveFrom <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (lt.EffectiveTo >= '" + dt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.EffectiveTo <= '" + dt2.ToString("yyyy-MM-dd 23:59:59.000") + "')) and e.EmpId =" + empCode;
            }


            string qry = "  select CAST(ROW_NUMBER() over(order by e.Empid) AS INT) Id," +
                           " e.EmpId,e.ShortName as Name,LT.Type," +
                           " case when lt.oldbranch = 43 then(select name from Departments where id = lt.OldDepartment) else " +
                            " (select name from branches where id = lt.OldBranch) end as OldDepartmentBranch," +
                           " case when lt.NewBranch = 43 then(select name from Departments where id = lt.NewDepartment) else " +
                           " (select name from branches where id = lt.NewBranch) end as NewDepartmentBranch, " +
                          " (select Code from Designations where id=lt.OldDesignation) as OldDesignation, " +
                          " (select Code from Designations where id=lt.NewDesignation) as NewDesignation, " +
                           " convert(varchar, lt.EffectiveFrom, 103) as EffectiveFrom,convert(varchar, lt.EffectiveTo, 103) as " +
                           " EffectiveTo from employees e  join designations d on e.CurrentDesignation = d.id " +
                           " join departments dp  on e.department = dp.id join branches b on e.branch = b.id join" +
                           " Employee_Transfer lt on lt.empid = e.id  where lt.Type = 'TemporaryTransfer' and e.RetirementDate >=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + cond1;
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable AllTopmanagementList()
        {
            string qry = "  select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id," +
                            " e.EmpId,e.ShortName as Name,d.code as Designation,case when b.id = 43 then dp.name else b.name end as BranchDepartmet,e.MobileNumber," +
                            " e.EmergencyContactNo as Extension" +
                            " from employees e  join designations d on e.CurrentDesignation = d.id" +
                            " join departments dp  on e.department = dp.id join branches b on e.branch = b.id" +
                            " where d.name in ('President', 'Chief General Manager', 'General Manager', " +
                            " 'Deputy General Manager', 'Assistant General Manager') and e.RetirementDate >=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) ";
            return sh.Get_Table_FromQry(qry);
        }


        public DataTable AllRetirementList(string fromdate, string todate, string RetireType)
        {
            string cond1 = "";
            if (fromdate == "-1" && todate == "-2")
            {
                cond1 = " where e.EmpId = '000009999999' ";
            }
            else if (RetireType == "RetEmp")
            {
                cond1 = " where e.RetirementDate <= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))";
            }

            else if (RetireType == "AllEmp")
            {
                cond1 = " where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))";
            }
            else if (RetireType == "Resig")
            {
                cond1 = " where e.RetirementDate <= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))";
            }
            else if (RetireType == "VrsEmp")
            {
                cond1 = " where e.RetirementDate <= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))";
            }
            else if (RetireType == "DeathEmp")
            {
                cond1 = " where e.RetirementDate <= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))";
            }
            else if (fromdate != "-1" && todate != "-2" && fromdate != string.Empty && todate != string.Empty && RetireType == "AllEmployees")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond1 = " where ((RetirementDate >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (RetirementDate <= '" + tdt.ToString("yyyy-MM-dd") + "')) and (Exit_type='Retd' or Exit_type='Resig' or Exit_type='VRS' or Exit_type='Death') ";
                //shows result within the given date range.
                //and (e.RetirementDate <= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) or e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)))"; //changed the condition e.RetirementDate (>=)(<=) convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))
            }
            else if (fromdate != "-1" && todate != "-2" && fromdate != string.Empty && todate != string.Empty && RetireType == "Retired")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond1 = " where ((RetirementDate >= '" + fdt.ToString("yyyy-MM-dd") + "') and Exit_type='Retd' and  (RetirementDate <= '" + tdt.ToString("yyyy-MM-dd") + "'))";
                //Shows results for retirement date for the date range, less or greater or equal to the current date 
                //and (e.RetirementDate <= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) or e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)))";
            }
            //
            else if (fromdate != "-1" && todate != "-2" && fromdate != string.Empty && todate != string.Empty && RetireType == "Resigned")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond1 = " where ((RetirementDate >= '" + fdt.ToString("yyyy-MM-dd") + "') and  Exit_type='Resig' and  (RetirementDate <= '" + tdt.ToString("yyyy-MM-dd") + "'))";
                //Shows results for resigned date for the date range, less or greater or equal to the current date
                //and (e.RetirementDate <= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) or e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)))";
            }
            //newly added on 02/06/2020
            else if (fromdate != "-1" && todate != "-2" && fromdate != string.Empty && todate != string.Empty && RetireType == "VRS Employees")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond1 = " where ((RetirementDate >= '" + fdt.ToString("yyyy-MM-dd") + "') and  Exit_type='VRS' and  (RetirementDate <= '" + tdt.ToString("yyyy-MM-dd") + "')) ";
                //Shows results for retirement date for the date range, less or equal to the current date cant be greater than currentdate.
                //and (e.RetirementDate <= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) or e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)))";
            }
            else if (fromdate != "-1" && todate != "-2" && fromdate != string.Empty && todate != string.Empty && RetireType == "Death")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond1 = " where ((RetirementDate >= '" + fdt.ToString("yyyy-MM-dd") + "') and  Exit_type='Death' and  (RetirementDate <= '" + tdt.ToString("yyyy-MM-dd") + "')) and   e.RetirementDate <= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))";
            }
            //end
            string qry = "  select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id," +
                        " e.EmpId,  e.Id as EmpidCode, e.ShortName as Name,d.code as Designation,case when b.id = 43 then dp.name else b.name end as BranchDepartmet," +
                        " convert(varchar, e.dob, 103) as DOB,convert(varchar, e.DOJ, 103) as DOJ,convert(varchar, e.RetirementDate, 103) as RetirementDate," +
                        " (year(getdate()) - year(dob)) as Age" +
                        "  from employees e  join designations d on e.CurrentDesignation = d.id" +
                        " join departments dp  on e.department = dp.id join branches b on e.branch = b.id" + cond1;
            return sh.Get_Table_FromQry(qry);
        }


        public DataTable AllLTCListdata(string fromDate, string toDate, string LtcType)
        {
            string cond1 = "";
            if (fromDate != "-1" && toDate != "-2" && LtcType != "-3" && LtcType != "All" && LtcType != "Select")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "and ((lt.startdate >= '" + fdt.ToString("yyyy-MM-dd") + "' and lt.startdate <= '" + tdt.ToString("yyyy-MM-dd") + "')" + "or" +
                        "(lt.enddate >= '" + fdt.ToString("yyyy-MM-dd") + "' and lt.enddate <= '" + tdt.ToString("yyyy-MM-dd") + "'))" +
                    "and  LTCType = '" + LtcType + "'";
            }
            if (fromDate != "-1" && toDate != "-2" && LtcType == "Select")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "and ((lt.startdate >= '" + fdt.ToString("yyyy-MM-dd") + "' and lt.startdate <= '" + tdt.ToString("yyyy-MM-dd") + "')" + "or" +
                        "(lt.enddate >= '" + fdt.ToString("yyyy-MM-dd") + "' and lt.enddate <= '" + tdt.ToString("yyyy-MM-dd") + "'))";

            }
            if (fromDate != "-1" && toDate != "-2" && LtcType != "-3" && LtcType == "All")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "and ((lt.startdate >= '" + fdt.ToString("yyyy-MM-dd") + "' and lt.startdate <= '" + tdt.ToString("yyyy-MM-dd") + "')" + "or" +
                        "(lt.enddate >= '" + fdt.ToString("yyyy-MM-dd") + "' and lt.enddate <= '" + tdt.ToString("yyyy-MM-dd") + "'))" +
                    "and  (LTCType = 'Availment' or  LTCType = 'Encashment') ";
            }
            else if (LtcType == "All")
            {

                cond1 = "";
            }
            else if (LtcType != "-3" && fromDate == "-1" && toDate == "-2" && LtcType != "All")
            {
                cond1 = "and  LTCType = '" + LtcType + "'";
            }

            string qry = "  select CAST(ROW_NUMBER() over(order by e.Empid) AS INT) Id," +
                            " e.EmpId,e.ShortName as Name,d.code as Designation,case when b.id = 43 then dp.name else b.name end as BranchDepartmet," +
                            " lt.LTCType,CONVERT(VARCHAR, lt.UpdatedDate, 103) as AppliedDate,CONVERT(VARCHAR, lt.StartDate, 103) as StartDate," +
                            " CONVERT(VARCHAR, lt.EndDate, 103) as EndDate,lt.PlaceOfVisits,lt.ModeOfTransport,lt.TravelAdvance," +
                            " bp.Block_Period,lt.Status from   employees e  join" +
                            " designations d on e.CurrentDesignation = d.id" +
                            " join departments dp on e.department = dp.id join branches b on e.branch = b.id join" +
                            " Leaves_LTC lt on lt.empid = e.id join BlockPeriod bp on lt.Block_Period = bp.id where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " + cond1;
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable AllEmpReptList(string empid)
        {
            string qry = "";
            string cond = "";
            if (empid == "" || empid == "-1")
            {
                cond = "";
            }
            else
            {
                cond = "and e1.EmpId=" + empid + " ";
            }
            //qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id," +
            //             " e.EmpId, e.Id as EmpidCode, e.ShortName as Name,d.code as Designations,case when b.id = 43 then dp.name else b.name end " +
            //             " as BranchDepartmet,concat(e.firstname, '', e.lastname) as EmpFullName, case when et.NewBranch = 43 then Dp1.name else b1.name end  as WorkingBranchDepartmet" +
            //             " from employees e join designations d on e.CurrentDesignation = d.id join branches" +
            //             " b on e.Branch = b.id join departments dp" +
            //             " on e.department = dp.id " +
            //             " join Employee_Transfer et  on et.EmpId=e.Id join Departments Dp1 on Dp1.id = et.NewDepartment Join Branches b1 on b1.id = et.NewBranch "+
            //             " where e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " + cond;

            //            qry = "; with ex as (select ROW_NUMBER() over(partition by et.empid order by et.id desc) as rnum," +
            //                "et.id,et.empid as etid,e.EmpId,et.NewDepartment, case when et.NewBranch is null then '' " +
            //                "else et.NewBranch end as NewBranch,e.Branch,case when b.Name='OtherBranch' then d.name else b.name end BranchDepartmet,case when b1.Name='OtherBranch' then d1.name else b1.name end WorkingBranchDepartmet " +
            //                "from Employees(nolock)e inner join Employee_Transfer(nolock) et on e.id = et.EmpId " +
            //                "inner join Branches b on et.NewBranch=b.id inner join Branches b1 on e.branch=b1.Id " +
            //                "inner join Departments d on et.NewDepartment=d.Id inner join Departments d1 on et.NewDepartment=d1.Id) " +
            // " select* into #tmp   from ex where rnum=1 " +
            // " select ex.BranchDepartmet,ex.WorkingBranchDepartmet,e1.EmpId,e1.id as EmpidCode,e1.ShortName as Name,concat(e1.firstname, '', e1.lastname) as EmpFullName,d.code as Designations,ex.etid ,ex.Id from Employees e1 left join #tmp ex on e1.id=ex.etid join designations d on e1.CurrentDesignation = d.id " +
            //" join departments dp on e1.department = dp.id  where e1.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))  " + cond + "  order by e1.id   ";

            qry = "; with ex as (select ROW_NUMBER() over(partition by et.empid order by et.id desc) as rnum," +
              "et.id,et.empid as etid,e.EmpId,et.NewDepartment, case when et.NewBranch is null then '' " +
              "else et.NewBranch end as NewBranch,e.Branch,case when b.Name='OtherBranch' then d.name else b.name end BranchDepartmet,case when b1.Name='OtherBranch' then d1.name else b1.name end WorkingBranchDepartmet " +
              "from Employees(nolock)e inner join Employee_Transfer(nolock) et on e.id = et.EmpId " +
              "inner join Branches b on et.NewBranch=b.id inner join Branches b1 on e.branch=b1.Id " +
              "inner join Departments d on et.NewDepartment=d.Id inner join Departments d1 on et.NewDepartment=d1.Id) " +
" select* into #tmp   from ex where rnum=1 " +
" select case when br.Name='OtherBranch' then dp.name else br.name end BranchDepartmet," +
" case when br.Name = 'OtherBranch' then dp.name else br.name end WorkingBranchDepartmet,e1.EmpId,e1.id as EmpidCode,e1.ShortName as Name,concat(e1.firstname, '', e1.lastname) as EmpFullName,d.code as Designations,ex.etid ,ex.Id from Employees e1 left join #tmp ex on e1.id=ex.etid join designations d on e1.CurrentDesignation = d.id " +
" join departments dp on e1.department = dp.id  " +
"join Branches br on e1.Branch=br.Id " +
" where e1.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))  " + cond + "  order by e1.id   ";

            return sh.Get_Table_FromQry(qry);
        }

        public DataTable AllLeavesHistoryList(string fromDate, string toDate, string datetype, string LeaveType, string LeaveStatus, string branch, string empcode, string datatype1, string dept)
        {
            string cond1 = "";
            string cond2 = "";
            string cond3 = "";
            string branchlist = "";
            string bcon = "";
            string dcon = "";
            if (branch != null && branch != "")
            {
                string[] branchdata = branch.Split(',');
                string[] deptdata = dept.Split(',');
                if (branchdata.Length > 1 || deptdata.Length > 1)
                {
                    foreach (var item in branchdata)
                    {
                        if (item == "0")
                        {
                            branch = "0";
                        }
                    }
                    foreach (var item in deptdata)
                    {
                        if (item == "0")
                        {
                            dept = "0";
                        }
                    }
                }
                //if (branch.Contains("0")  || dept.Contains("0"))
                //{
                //    var branchcount = branch.Count(Char.IsDigit);
                //    var deptcount = dept.Count(Char.IsDigit);
                //    if (branchcount > 1 || deptcount>0)
                //    {
                //        branch = "0";
                //        dept = "0";
                //    }
                // string   branch1= branch.Substring(2);
                // }
                branch = branch.Trim();

                int number = 0;

                //foreach (string item in branch.Split(new char[] { ',' }))
                //{
                //    if (number > 0)
                //    {
                //        branchlist = branchlist + "," + "'" + item + "'";
                //    }
                //    else
                //    {
                //        branchlist = "'" + item + "'";
                //    }
                //    number++;
                //}
                if (branch.Contains("and"))
                {
                    branch = branch.Replace("and", "&");
                }
            }
            else if (branch == "")
            {
                branch = "All";
            }
            // if (datatype1 == branch) { }


            if (fromDate == "-1" && (toDate == null || toDate == "") && datetype == null && LeaveType == null && LeaveStatus == null)
            {
                cond1 = " where (LeaveType = '" + LeaveType + "')";
            }

            //BOTH BRANCH,DEPT
            // branches,dept
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && (LeaveType == string.Empty || LeaveType == "ALLTypes") && (LeaveStatus == string.Empty || LeaveStatus == "ALL") && branch != string.Empty && dept != string.Empty && empcode == string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  " +
                    "and (branchid  in(" + branch + ") or deptid  in(" + dept + ") )";
            }
            //branch,dept,empcode
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && (LeaveType == string.Empty || LeaveType == "ALLTypes") && (LeaveStatus == string.Empty || LeaveStatus == "ALL") && branch != string.Empty && dept != string.Empty && empcode != string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and EmpId=" + empcode + "  and (branchid  in(" + branch + ") or deptid  in(" + dept + "))";
                // cond2 = "and e.Branch!=43";

            }
            //branch,dept,empcode,single leave type
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && dept != string.Empty && empcode != string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  " +
                        " and EmpId=" + empcode + "  and (branchid  in(" + branch + ") or deptid  in(" + dept + "))";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  " +
                        "and(LeaveType = '" + LeaveType + "') and EmpId=" + empcode + "  and (branchid  in(" + branch + ") or deptid  in(" + dept + "))";
                }
            }
            //branch,dept,empcode,single leave type sowjanya
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && dept != string.Empty && empcode != string.Empty && branch != "0" && dept != "0" && branch == "null" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and EmpId=" + empcode + " ";
                    cond2 = "and e.Branch!=43";
                    //dcon = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(LeaveType = '" + LeaveType + "')  and EmpId=" + empcode + "  and deptid  in(" + dept + ")";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(LeaveType = '" + LeaveType + "') and EmpId=" + empcode + "  and branchid  in(" + branch + ") ";
                    cond2 = "and e.Branch!=43";
                    //dcon = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(LeaveType = '" + LeaveType + "')  and EmpId=" + empcode + "  and deptid  in(" + dept + ")";
                }
            }

            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch == "null")

            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') ";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(LeaveType = '" + LeaveType + "')    ";
                }
                // cond2 = "and e.Branch!=43";
                //dcon = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(LeaveType = '" + LeaveType + "')   and deptid  in(" + dept + ")";
            }
            //branch,dept,single status
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType == string.Empty && LeaveStatus != "ALL" && branch != string.Empty && dept != string.Empty && empcode == string.Empty && branch != "0" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(status = '" + LeaveStatus + "')   and (branchid  in(" + branch + ") or deptid  in(" + dept + ")) ";

            }

            //branch,dept,single status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType == string.Empty && LeaveStatus != "ALL" && branch != string.Empty && dept != string.Empty && empcode != string.Empty && branch != "0" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(status = '" + LeaveStatus + "') and EmpId=" + empcode + "  and (branchid  in(" + branch + ") or deptid  in(" + dept + ")) ";

            }
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && (LeaveType == "ALLTypes" || LeaveType == "") && LeaveStatus != "ALL" && branch != string.Empty && dept != string.Empty && empcode != string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') " +
                    " and status = '" + LeaveStatus + "'  and EmpId=" + empcode + "  and (branchid  in(" + branch + ") or deptid  in(" + dept + ")) ";


            }
            //branch,dept,empcode,single leave type,single status
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != string.Empty && dept != string.Empty && empcode != string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') " +
                        " and status = '" + LeaveStatus + "'  and EmpId=" + empcode + "  and (branchid  in(" + branch + ") or deptid  in(" + dept + ")) ";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(LeaveType = '" + LeaveType + "')" +
                        " and status = '" + LeaveStatus + "'  and EmpId=" + empcode + "  and (branchid  in(" + branch + ") or deptid  in(" + dept + ")) ";
                }

            }
            //null branch,null dept,empcode,single leave type,single status sowjanya 
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != string.Empty && dept != string.Empty && empcode != string.Empty && branch != "0" && dept != "0" && branch == "null" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and status = '" + LeaveStatus + "'  and EmpId=" + empcode + " ";
                    cond2 = "and e.Branch=43";
                    //dcon = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(LeaveType = '" + LeaveType + "')and status = '" + LeaveStatus + "'   and EmpId=" + empcode + "  and deptid  in(" + dept + ")";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(LeaveType = '" + LeaveType + "') and status = '" + LeaveStatus + "'  and EmpId=" + empcode + " ";
                    cond2 = "and e.Branch=43";
                    //dcon = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and(LeaveType = '" + LeaveType + "')and status = '" + LeaveStatus + "'   and EmpId=" + empcode + "  and deptid  in(" + dept + ")";
                }

            }

            //branch,dept,single leave type,single status
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus != "ALL" && LeaveStatus != string.Empty && branch != string.Empty && dept != string.Empty && empcode == string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') " +
                        " and status = '" + LeaveStatus + "'   and (branchid  in(" + branch + ") or deptid  in(" + dept + ")) ";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  " +
                        "and(LeaveType = '" + LeaveType + "') and status = '" + LeaveStatus + "'   and (branchid  in(" + branch + ") or deptid  in(" + dept + ")) ";

                }

            }

            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType == string.Empty && LeaveStatus != "ALL" && LeaveStatus != string.Empty && branch != string.Empty && dept != string.Empty && empcode == string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  " +
                    " and status = '" + LeaveStatus + "'   and (branchid  in(" + branch + ") or deptid  in(" + dept + ")) ";

            }

            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus != "ALL" && LeaveStatus == string.Empty && branch != string.Empty && dept != string.Empty && empcode == string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') " +
                        " and (branchid  in(" + branch + ") or deptid  in(" + dept + ")) ";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  " +
                        "and(LeaveType = '" + LeaveType + "')  and (branchid  in(" + branch + ") or deptid  in(" + dept + ")) ";

                }

            }
            //branch,dept,empcode,all leave type,all status
            //else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != string.Empty && dept != string.Empty && empcode != string.Empty && branch != "0" && dept != "0" && branch != "null")
            //{
            //    DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
            //    DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
            //    bcon = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL')) and EmpId=" + empcode + "  and branchid  in(" + branch + ") ";
            //    // cond2 = "and e.Branch!=43";
            //    dcon = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL'))  and EmpId=" + empcode + "  and deptid  in(" + dept + ")";
            //}
            // null branch,dept,all leave type,all status sowjanya
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != string.Empty && dept != string.Empty && empcode != string.Empty && branch != "0" && dept != "0" && branch == "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and deptid in(" + dept + ") and EmpId in(" + empcode + ")";
                //cond2 = "and e.Branch!=43";
                //dcon = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL'))   and deptid  in(" + dept + ")";
            }

            // null branch,dept,all leave type,all status sowjanya
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != string.Empty && dept != string.Empty && empcode == string.Empty && branch != "0" && dept != "0" && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))";
                //cond2 = "and e.Branch!=43";
                //dcon = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL'))   and deptid  in(" + dept + ")";
            }
            //  branch,dept,all leave type,all status sowjanya
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != string.Empty && dept != string.Empty && empcode == string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))  and( branchid  in(" + branch + ") or deptid  in(" + dept + "))";
                //cond2 = "and e.Branch!=43";
                //cond2 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL'))   and deptid  in(" + dept + ")";
            }
            // single branch,all dept, branch is null sowjanay
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != string.Empty && dept != string.Empty && empcode == string.Empty && branch != "0" && dept == "0" && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and status = '" + LeaveStatus + "'";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "'";
                }
            }


            // all branch,single dept,
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && dept != "null" && empcode == string.Empty && (branch == "0" || branch == "null") && dept != "0" && LeaveStatus == string.Empty && LeaveType == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')   and (branchid not in(" + branch + ",43) or deptid in (" + dept + "))";
            }


            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && dept != "null" && empcode != string.Empty && (branch == "0" || branch == "null") && dept != "0" && LeaveStatus == string.Empty && LeaveType == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')   and (branchid not in(" + branch + ",43) or deptid in (" + dept + ")) and empId in (" + empcode + ")";
            }
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != "null" && empcode == string.Empty && branch != "0" && (dept == "0" || dept == "null") && LeaveStatus == string.Empty && LeaveType == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')   and (branchid in(" + branch + ") or deptid not in (" + dept + "))";
            }

            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != "null" && empcode != string.Empty && branch != "0" && (dept == "0" || dept == "null") && LeaveStatus == string.Empty && LeaveType == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')   and (branchid in(" + branch + ") or deptid not in (" + dept + ")) and empId in (" + empcode + ")";
            }
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveStatus != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && empcode == string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null") && LeaveStatus == string.Empty && LeaveType == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  ";
            }

            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && empcode == string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null") && LeaveStatus == string.Empty && LeaveType == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  ";
            }

            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && empcode != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null") && LeaveStatus == string.Empty && LeaveType == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and EmpId=" + empcode + "";
            }

            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && empcode != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null") && LeaveStatus == string.Empty && LeaveType == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and empId in (" + empcode + ")";
            }

            //****date:Applied,branches
            // all branches ,empcode
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && empcode != string.Empty && branch == "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and EmpId=" + empcode + " and branchid not in(" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //single branches, empid not dept
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && dept == "null" && branch != "0" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and EmpId=" + empcode + "  and branchid  in(" + branch + ")";
                cond2 = "and e.Branch!=43";

            }
            //single branch
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && dept == "null" && branch != "0" && empcode == string.Empty && branch != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and branchid  in(" + branch + ")";
                cond2 = "and e.Branch!=43";

            }
            //all branches not dept
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && dept == "null" && branch == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and branchid not in(" + branch + ")";
                cond2 = "and e.Branch!=43";

            }
            //all branches,all leave types,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && branch == "0" && empcode != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + " and branchid not in(" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //single branch,all leavetypes,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && branch != "0" && dept != "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + " and (branchid  in(" + branch + ") or deptid in(" + dept + "))";
            }
            //single branch,all leavetypes
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && branch != "0" && dept == "null" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and  branchid  in(" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //all branches,single leave type
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch == "0" && empcode == string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType = '" + LeaveType + "')  and branchid  not in(" + branch + ") ";
                cond2 = "and e.Branch!=43";
            }
            //all branches,single leave type,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch == "0" && dept == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType = '" + LeaveType + "') and EmpId=" + empcode + " and branchid  not in(" + branch + ") ";
                cond2 = "and e.Branch!=43";
            }
            //all leavestatus,single branch
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL" && branch != "0" && dept == "null" && empcode == string.Empty && branch != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and branchid   in(" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //all leavestatus,null branch  Sowjanya
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL" && branch != "0" && dept == "null" && empcode == string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))";
            }
            //all leavestatus,single branch,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL" && branch != "0" && dept == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId=" + empcode + " and branchid   in(" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //all branch,all leavestatus,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL" && branch == "0" && dept == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId=" + empcode + " and branchid not  in(" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //all branch,all leavestatus
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL" && branch == "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and branchid not  in(" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //all branch,single leave status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus != "ALL" && branch == "0" && empcode != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and status = '" + LeaveStatus + "' and EmpId=" + empcode + " and branchid not  in(" + branch + ") ";
                cond2 = "and e.Branch!=43";
            }
            //all branch,single leave status
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus != "ALL" && branch == "0" && empcode == string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and status = '" + LeaveStatus + "'  and branchid not  in(" + branch + ") ";
                cond2 = "and e.Branch!=43";
            }
            //single branch,single leave status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus != "ALL" && (branch != "0" && branch != "null") && empcode != string.Empty && (dept == "null" || dept == "0"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and status = '" + LeaveStatus + "' and EmpId=" + empcode + " and branchid  in(" + branch + ") ";
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch == "All" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))and EmpId=" + empcode + " and BrDept! ='" + branch + "'";
            }
            //single branch,all leave types,all leve status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != "0" && branch == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))and EmpId=" + empcode + "";

            }
            //sibgle branch,single leavetype,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && branch != "0" && empcode != string.Empty && dept == "null" && LeaveStatus == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and  LeaveType ='" + LeaveType + "'  and EmpId=" + empcode + " and branchid   in(" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //all branch,all leave types,all leve status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch == "0" && dept == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))and EmpId=" + empcode + " and branchid  not in(" + branch + ")";
            }

            //all branch,single leave types,single leave status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch == "0" && dept == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and status = '" + LeaveStatus + "' and  LeaveType ='" + LeaveType + "' and EmpId=" + empcode + " and branchid  not in(" + branch + ")";
                cond2 = "and e.Branch!=43";
            }


            // for employee code is empty
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && branch == "All")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and BrDept!='All'";

            }

            //****date:Applied,dept 
            //all dept not branch
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && empcode == string.Empty && branch != string.Empty && branch == "null" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and deptid not in(" + dept + ")";
                cond2 = "and e.Branch=43";

            }
            // all dept ,empcode
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && empcode != string.Empty && branch == "null" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and EmpId=" + empcode + " and deptid not in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //single dept, empid 
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && branch == "null" && dept != "0" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and EmpId=" + empcode + "  and deptid  in(" + dept + ")";
                cond2 = "and e.Branch=43";

            }
            //single dept
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && branch == "null" && dept != "0" && empcode == string.Empty && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and deptid  in(" + dept + ")";
                cond2 = "and e.Branch=43";

            }
            //all dept,all leave types,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && dept == "0" && empcode != string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + " and deptid not in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //single dept,all leavetypes,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && dept != "0" && branch == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + " and deptid  in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //single dept,all leavetypes
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && dept != "0" && branch == "null" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and  deptid  in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //all dept,single leave type
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && dept == "0" && empcode == string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType = '" + LeaveType + "')  and deptid  not in(" + dept + ") ";
                cond2 = "and e.Branch=43";
            }
            //all dept,single leave type,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && dept == "0" && branch == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType = '" + LeaveType + "') and EmpId=" + empcode + " and deptid  not in(" + dept + ") ";
                cond2 = "and e.Branch=43";
            }
            //all leavestatus,single dept
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL" && dept != "0" && branch == "null" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and deptid   in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //all leavestatus,single dept,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL" && dept != "0" && branch == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId=" + empcode + " and deptid   in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //all dept,all leavestatus,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL" && dept == "0" && branch == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId=" + empcode + " and deptid not  in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //all dept,all leavestatus
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL" && dept == "0" && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and deptidid not  in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //all dept,single leave status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus != "ALL" && dept == "0" && empcode != string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and status = '" + LeaveStatus + "' and EmpId=" + empcode + " and deptid not  in(" + dept + ") ";
                cond2 = "and e.Branch=43";
            }
            //all dept,single leave status
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus != "ALL" && dept == "0" && empcode == string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and status = '" + LeaveStatus + "'  and deptid not  in(" + dept + ") ";
                cond2 = "and e.Branch=43";
            }
            //single dept,all leave types,all leve status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && dept != "0" && empcode != string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))and EmpId=" + empcode + " and branchid   in(" + branch + ")";
                cond2 = "and e.Branch=43";
            }
            //single dept,single leavetype,single leave status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus != "ALL" && dept != "0" && empcode != string.Empty && branch == "null" && LeaveStatus != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                               " and status = '" + LeaveStatus + "' and EmpId=" + empcode + " and deptid   in(" + dept + ")";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "' and EmpId=" + empcode + " and deptid   in(" + dept + ")";
                }
            }
            //null dept,single leavetype,all leave status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != "" && LeaveStatus == "ALL" && dept != "0" && dept != "null" && empcode != string.Empty && branch == "null" && LeaveStatus != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                        " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId=" + empcode + " and deptid   in(" + dept + ")";
                    cond2 = "and e.Branch=43";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                        " and  LeaveType ='" + LeaveType + "' and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId=" + empcode + " and deptid   in(" + dept + ")";
                    cond2 = "and e.Branch=43";
                }
            }
            //single dept,single leavetype,all leave status,empcode sowjanya
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != "" && LeaveStatus == "ALL" && dept != "0" && dept == "null" && empcode != string.Empty && branch != "0" && branch == "null" && LeaveStatus != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId=" + empcode + "";
                    cond2 = "and e.Branch=43";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId=" + empcode + "";
                    cond2 = "and e.Branch=43";
                }

            }

            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveType != "" && LeaveStatus == "ALL" && dept != "0" && dept == "null" && empcode != string.Empty && branch != "0" && branch == "null" && LeaveStatus != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId=" + empcode + "";
                    cond2 = "and e.Branch=43";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId=" + empcode + "";
                    cond2 = "and e.Branch=43";
                }

            }
            //single dept,all leavetype,all leave status
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && dept != "0" && dept != "null" && empcode == string.Empty && branch == "null" && LeaveStatus != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and  LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId=" + empcode + " and deptid   in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //single dept,single leavetype,empcode
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && dept != "0" && empcode != string.Empty && branch == "null" && LeaveStatus == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and  LeaveType ='" + LeaveType + "'  and EmpId=" + empcode + " and deptid   in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //single dept,single LeaveStatus
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && dept != "0" && empcode == string.Empty && branch == "null" && LeaveStatus != "ALL" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and status = '" + LeaveStatus + "'  and deptid   in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }
            //single dept,single leavetype,single leave status sowjanya
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && LeaveType != "" && dept != "0" && empcode == string.Empty && branch == "null" && LeaveStatus != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and status = '" + LeaveStatus + "'  and deptid   in(" + dept + ")";
                    cond2 = "and e.Branch=43";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "'  and deptid   in(" + dept + ")";
                    cond2 = "and e.Branch=43";
                }
            }
            //single dept,single leavetype,single leave status Sowjanya
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && LeaveType == "" && dept != "0" && empcode != string.Empty && branch == "null" && LeaveStatus != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and empId  in (" + empcode + ") and status = '" + LeaveStatus + "'";
            }
            //single dept,single leavetype,single leave status Sowjanya
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && LeaveType == "" && dept != "0" && empcode == string.Empty && branch == "null" && LeaveStatus != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and status = '" + LeaveStatus + "'";

            }
            // single dept,single leavetype
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && dept != "0" && empcode == string.Empty && branch == "null" && LeaveStatus == string.Empty && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and  LeaveType ='" + LeaveType + "'  and deptid   in(" + dept + ")";
                cond2 = "and e.Branch=43";
            }




            //applied date
            //******all dept, all branch*****
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && empcode == string.Empty && branch != string.Empty && branch == "0" && dept == "0")
            {
                //DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                //DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and deptid not in(" + dept + ")";
                ////cond2 = "and e.Branch=43";
                cond1 = "";
            }
            //empcode
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && empcode != string.Empty && branch != string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and EmpId=" + empcode + "  and deptid not in(" + dept + ")";
                //cond2 = "and e.Branch=43";

            }
            //empcode,single leavetype
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && empcode != string.Empty && branch != string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                        " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and EmpId=" + empcode + "  ";
                    //cond2 = "and e.Branch=43";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                        " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  LeaveType ='" + LeaveType + "' and EmpId=" + empcode + "  ";
                }

            }

            //single leavetype
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus == string.Empty && empcode == string.Empty && branch != string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                        " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                        " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  LeaveType ='" + LeaveType + "' ";
                }
                //cond2 = "and e.Branch=43";

            }
            //single leave status
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus != "ALL" && empcode == string.Empty && branch != string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and status = '" + LeaveStatus + "'    and deptid not in(" + dept + ")";
                //cond2 = "and e.Branch=43";

            }

            //single leave status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus != "ALL" && empcode != string.Empty && branch != string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and status = '" + LeaveStatus + "'   and EmpId=" + empcode + "   and deptid not in(" + dept + ")";
                //cond2 = "and e.Branch=43";

            }            //empcode,single leavetype,single leave status
            //empcode,all leavetype,all leave status
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && empcode != string.Empty && branch != string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML') and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId=" + empcode + " ";
                //cond2 = "and e.Branch=43";

            }
            //all leavetype,all leave status
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && empcode == string.Empty && branch != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML') and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) ";

                //cond2 = "and e.Branch=43";

            }
            //single leavetype,all leave status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && empcode == string.Empty && branch != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                     " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) ";
                    //cond2 = "and e.Branch=43";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                        " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and   LeaveType ='" + LeaveType + "'  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) ";
                    //cond2 = "and e.Branch=43";
                }

            }

            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && empcode == string.Empty && branch != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                     " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) ";
                    //cond2 = "and e.Branch=43";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                        " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) ";
                    //cond2 = "and e.Branch=43";
                }

            }

            if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != "ALLTypes" && (LeaveStatus == "ALL" || LeaveStatus == "") && empcode == string.Empty && branch != string.Empty && (branch != "0" && branch != "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                     " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (branchid in('" + branch + "') or deptid not in(" + dept + ",46) )";
                    //cond2 = "and e.Branch=43";
                }
                else
                {
                    cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                        " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and   LeaveType ='" + LeaveType + "'  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (branchid in('" + branch + "') or deptid not in(" + dept + ",46) )";
                    //cond2 = "and e.Branch=43";
                }

            }
            //all leavetype,single leave status,empcode
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus != "ALL" && empcode != string.Empty && branch != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML') and status = '" + LeaveStatus + "'  and EmpId in (" + empcode + " )";
                //cond2 = "and e.Branch=43";

            }

            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && empcode != string.Empty && branch != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML') and EmpId in (" + empcode + " )";
                //cond2 = "and e.Branch=43";

            }

            //all le]7avetype,single leave status
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus != "ALL" && empcode == string.Empty && branch != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML') and status = '" + LeaveStatus + "' ";


            }

            //all le]7avetype,single leave status
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && LeaveStatus != "ALL" && empcode == string.Empty && branch != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')  ";


            }
            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && empcode == string.Empty && branch != string.Empty && (branch == "0" || branch == "null") && dept != "0" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML') and status = '" + LeaveStatus + "' and (branchid not in (" + branch + ",43) or deptid in (" + dept + "))";

            }

            else if (datetype == "Applied" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && empcode != string.Empty && branch != string.Empty && (branch == "0" || branch == "null") && dept != "0" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where (Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103)" +
                    " <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML') and status = '" + LeaveStatus + "' and empId in (" + empcode + ") and (branchid not in (" + branch + ",43) or deptid in (" + dept + "))";

            }

            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && branch != "0" && empcode != string.Empty && branch != "null" && dept == "0" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))and EmpId=" + empcode + " and (branchid in (" + branch + ") or deptid not in (" + dept + "))" +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "'";
            }
            // sowjanya all dept ,all leave type, singel status, branch is null
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && branch != "0" && empcode != string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))and EmpId=" + empcode + " " +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "'";
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType == string.Empty && LeaveStatus == "ALL" && branch != "0" && branch != "null" && empcode != string.Empty && dept != "null" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);


                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId in(" + empcode + ") and ( branchid in(" + branch + ") or deptid in(" + dept + "))";

            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus == "ALL" && branch != "0" && branch != "null" && empcode != string.Empty && dept != "null" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId in(" + empcode + ") and ( branchid in(" + branch + ") or deptid in(" + dept + "))";
                }
                else
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))   and  LeaveType in ('" + LeaveType + "') and EmpId in (" + empcode + ") and ( branchid in(" + branch + ") or deptid in(" + dept + "))";
                }
            }
            //sowjanya 
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && (branch == "0" || branch == "null") && empcode != string.Empty && (dept == "null" || dept == "0"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId in(" + empcode + ") ";
                }
                else
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and EmpId in (" + empcode + ") ";
                }
            }

            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && (branch == "0" || branch == "null") && empcode != string.Empty && (dept == "null" || dept == "0"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and EmpId in(" + empcode + ") ";
                }
                else
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId in (" + empcode + ") ";
                }
            }

            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && branch == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))  and BrDept !='" + branch + "'";
            }

            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus != "ALL" && branch == "0" && LeaveStatus != "")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date, AppliedDate, 103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and(Convert(date, AppliedDate, 103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and status = '" + LeaveStatus + "' and branchid not in (" + branch + ") ";
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch == "All")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))and  BrDept! ='" + branch + "'";
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != "0" && branch != "null" && (dept == "0" || dept == "null") && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and branchid ='" + branch + "'";
            }

            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != "0" && branch != "null" && (dept == "0" || dept == "null") && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and branchid ='" + branch + "' and empid=" + empcode + "";
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus != "ALL" && branch != "0" && branch != "null" && dept != "null" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')) " +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "' and (branchid in(" + branch + ") or deptid not in(" + dept + ",46))";
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && LeaveStatus == string.Empty && branch != "0" && branch != "null" && dept != "null" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')) " +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))  and (branchid in(" + branch + ") or deptid not in(" + dept + ",46))";
            }
            //else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode != string.Empty  && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && branch != "0" && branch != "null" && dept != "null" && dept != "0")
            //{
            //    DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
            //    DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
            //    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')) " +
            //           " and (LeaveType IN('CL', 'PL', 'LOP', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL')) and status = '" + LeaveStatus + "' and Empid="+empcode+" and (branchid in(" + branch + ") or deptid in(" + dept + "))";
            //}
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && branch == "All")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')) and BrDept !='" + branch + "'" +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "'";
            }

            //branch is not nll
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != "" && LeaveStatus == "ALL" && branch != "All" && branch != "null" && branch != "0" && dept != "null" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and( branchid in('" + branch + "') or deptid in(" + dept + "))";
                }
                else
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and  LeaveType ='" + LeaveType + "'  and( branchid in(" + branch + ") or deptid in(" + dept + "))";

                }
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode != string.Empty && LeaveType != "ALLTypes" && LeaveType != "" && LeaveStatus == "ALL" && branch != "null" && branch == "0" && dept != "null" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and empId in( " + empcode + " ) and (deptid in(" + dept + ") or branchid not in (" + branch + ",43))";
                }
                else
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and  LeaveType ='" + LeaveType + "' and (deptid in(" + dept + ") or branchid not in (" + branch + ",43)) and empId in(" + empcode + ")";
                }
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode != string.Empty && LeaveType != "ALLTypes" && LeaveType != "" && LeaveStatus == "ALL" && branch != "null" && branch != "0" && dept != "null" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and empId in( " + empcode + " ) and (deptid not in(" + dept + ") or branchid  in (" + branch + "))";
                }
                else
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and  LeaveType ='" + LeaveType + "' and (deptid not in(" + dept + ") or branchid  in (" + branch + ")) and empId in(" + empcode + ")";
                }
            }

            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode == string.Empty && LeaveType != "ALLTypes" && LeaveType != "" && LeaveStatus == "ALL" && branch != "null" && branch == "0" && dept != "null" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (deptid in('" + dept + "') or branchid not in ('" + branch + "',43) )";
                }
                else
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and  LeaveType ='" + LeaveType + "' and (deptid in('" + dept + "') or branchid not in ('" + branch + "',43) )";
                }
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode == string.Empty && LeaveType == "ALLTypes" && LeaveType != "" && LeaveStatus == "ALL" && branch != "All" && branch != "null" && branch == "0" && dept != "null" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                   " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and (deptid in('" + dept + "') or branchid not in (" + branch + ",43))";
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode != string.Empty && LeaveType == "ALLTypes" && LeaveType != "" && LeaveStatus == "ALL" && branch != "All" && branch != "null" && branch == "0" && dept != "null" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                   " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and empId in (" + empcode + ") and (deptid in('" + dept + "') or branchid not in (" + branch + ",43))";
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && branch == "All")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))  and  LeaveType ='" + LeaveType + "'  and BrDept !='" + branch + "'";
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch == "All")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                       " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "'  and BrDept !='" + branch + "'";
            }

            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode == string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != "0" && LeaveStatus != "" && branch != "null" && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and status = '" + LeaveStatus + "' and ( branchid in(" + branch + ") or deptid not in(" + dept + "))";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "' and ( branchid in(" + branch + ") or deptid not in(" + dept + "))";
                }
            }
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode == string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus != "ALL" && (branch == "0" || branch == "null") && LeaveStatus != "" && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and status = '" + LeaveStatus + "'";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "' ";
                }
            }

            if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode == string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && (branch == "0" || branch == "null") && LeaveStatus != "" && (dept != "0" && dept != "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and status = '" + LeaveStatus + "' and (branchid not in(" + branch + ",43) or deptid in (" + dept + "))";
                }
                else
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "'  and (branchid not in(" + branch + ",43) or deptid in (" + dept + "))";
                }
            }

            if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && (branch == "0" || branch == "null") && LeaveStatus != "" && (dept != "0" && dept != "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and status = '" + LeaveStatus + "' and (branchid not in(" + branch + ",43) or deptid in (" + dept + ")) and empId in (" + empcode + ")";
                }
                else
                {
                    cond1 = " where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "'  and (branchid not in(" + branch + ",43) or deptid in (" + dept + ")) and empId in(" + empcode + ")";
                }
            }

            // all dept, Empcode,singal leavetyype, singel status branch null sowjanay
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode == string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != "All" && LeaveStatus != "" && branch == "null" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and status = '" + LeaveStatus + "'";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "'";
                }
            }
            // all dept, Empcode,singal leavetyype, empcode,singel status branch not null sowjanay
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus != "ALL" && (branch == "0" || branch == "null") && LeaveStatus != "" && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and status = '" + LeaveStatus + "' and empId=" + empcode + "";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "' and empId=" + empcode + "";
                }
            }

            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != "0" && branch != "null" && LeaveStatus != "" && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and status = '" + LeaveStatus + "' and empId=" + empcode + " and (branchid in (" + branch + ") or deptid not in (" + dept + "))";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "' and empId=" + empcode + "and (branchid in (" + branch + ") or deptid not in (" + dept + "))";
                }
            }

            // all dept, Empcode,singal leavetyype, empcode,singel status branch null sowjanay
            else if (datetype == "Applied" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != "0" && LeaveStatus != "" && branch == "null" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and status = '" + LeaveStatus + "'and empId=" + empcode + "";
                }
                else
                {
                    cond1 = "where ((Convert(date,AppliedDate,103) >='" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') and  (Convert(date,AppliedDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                           " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "' and empId=" + empcode + "";
                }
            }

            else if (datetype == "Applied" && fromDate == string.Empty && toDate == string.Empty && LeaveType == "ALLTypes" && (LeaveStatus != "ALL" || LeaveStatus == string.Empty))
            {
                cond1 = " Where (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))";
            }
            else if (datetype == "Applied" && fromDate == string.Empty && toDate == string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL")
            {
                cond1 = " Where (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))";
            }
            else if (datetype == "Applied" && fromDate == string.Empty && toDate == string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL")
            {
                cond1 = " Where (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))";
            }
            else if (datetype == "Applied" && fromDate == string.Empty && toDate == string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty)
            {
                cond1 = " Where  LeaveType ='" + LeaveType + "'";
            }
            else if (datetype == "Applied" && fromDate == string.Empty && toDate == string.Empty && LeaveType == string.Empty && LeaveStatus != "ALL")
            {
                cond1 = " Where status = '" + LeaveStatus + "'";
            }


            //Request Condition
            //****date:Request,branches 
            //all branch
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && empcode == string.Empty && branch == "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and branchid  not in (" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //single branches
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && empcode == string.Empty && branch != "0" && dept == "null" && branch != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and branchid   in (" + branch + ")";
            }
            //single branches,empcode
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && empcode != string.Empty && branch != "0" && branch != "null" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and EmpId=" + empcode + " and branchid   in (" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //single dept, branch empty, from date true, to date true, no leavetype, no leavestatus
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && dept != string.Empty && empcode != string.Empty && dept != "0" && dept != "null" && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and EmpId=" + empcode + " and deptid  in (" + dept + ")";
                cond2 = "and e.Branch!=43";
            }
            //single branches,empcode,single leavetype
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && empcode != string.Empty && branch != "0" && dept == "null" && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and EmpId=" + empcode + " and  LeaveType ='" + LeaveType + "'";
                cond2 = "and e.Branch!=43";

                //added by chaitanya on 16/04/2020
                //Initially it was only 1 condition i.e query in else condition
                //Added if else condition for branch =null no branch condition in where, else branch condition in query
            }
            //no branches,empcode,single leavetype 
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && empcode != string.Empty && branch != "0" && dept == "null" && branch != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and EmpId=" + empcode + " and  LeaveType ='" + LeaveType + "' and branchid   in (" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //end
            //newly added by chaitanya on 17/04/2020
            //no branch, single department, single empcode,from and todates, singleleavetype, empty status
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && empcode != string.Empty && branch == "null" && dept != "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and EmpId=" + empcode + " and  LeaveType ='" + LeaveType + "'";
                cond2 = "and e.Department!=46";
            }
            //no branch, no department, single empcode,from and todates, singleleavetype, empty status
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && empcode != string.Empty && branch == "null" && dept != "0" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and EmpId=" + empcode + " and  LeaveType ='" + LeaveType + "' and deptid   in (" + dept + ")";
                cond2 = "and e.Department!=46";
            }
            //end

            //single branches,empcode,single leavetype,single status
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != string.Empty && empcode != string.Empty && branch != "0" && branch != "null" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and EmpId=" + empcode + " and  LeaveType ='" + LeaveType + "'  and status = '" + LeaveStatus + "' and branchid   in (" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //single dept, single empcode, single leavetype, single status
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && branch != string.Empty && empcode != string.Empty && dept != "0" && branch == "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and EmpId=" + empcode + " and  LeaveType ='" + LeaveType + "'  and status = '" + LeaveStatus + "' and deptid =" + dept + " ";
                cond2 = "and e.Department!=46";
            }
            //single branches,empcode,all leavetype,all status
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != string.Empty && empcode != string.Empty && branch != "0" && branch == "null" && dept != "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) )and EmpId in (" + empcode + ") and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL')) ";
            }
            //single branches,empcode,all leavetype,single status
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && branch != string.Empty && empcode != string.Empty && branch != "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))and EmpId=" + empcode + " and  status = '" + LeaveStatus + "' and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and branchid   in (" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //single branches,empcode,single leavetype,all status
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && branch != string.Empty && empcode != string.Empty && (branch == "0" || branch == "null") && (dept == "null" || dept == "0"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = "Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) )and EmpId=" + empcode + "and LeaveType ='" + LeaveType + "'and" +
                        " (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) ";
                //if (LeaveType == "LTC" || LeaveType == "PLE")
                //{
                //    cond1 = " Where ((Convert(date,StartDate,103)  >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and " +
                //        "Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                //        "and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                //        "and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                //        "and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')) and EmpId=" + empcode + " and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))";
                //}
                //else
                {
                    //cond1 = " Where ((Convert(date,StartDate,103)  >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and " +
                    //    "Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                    //    "and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                    //    "and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                    //    "and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')) and EmpId=" + empcode + " and  LeaveType ='" + LeaveType + "'  and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))";
                    //cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and " +
                    // "'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                    //"and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')) and EmpId=" + empcode + " and  LeaveType ='" + LeaveType + "'  and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))";
                }

            }
            //all branch,empcode
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && empcode != string.Empty && branch == "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))and EmpId=" + empcode + " and branchid  not in (" + branch + ")";
                cond2 = "and e.Branch!=43";
            }
            //all branch,all leave types,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && branch == "0" && empcode != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + " and branchid  not in (" + branch + ",43) ";

            }
            //no branch, no dept, empcode= single value, leavetype=single value not all, status=single value not all, with fromdate and todate values //newly added on 05/06/2020
            //else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus != string.Empty && LeaveStatus != "ALL" && branch != "0" && branch == "null" && empcode != string.Empty && dept != "0" && dept == "null")
            //{
            //    DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
            //    DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
            //    cond1 = " Where ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
            //        " and LeaveType = '" + LeaveType + "' and EmpId=" + empcode + " and status ='" + LeaveStatus + "' ";
            //    //cond2 = "and e.Branch!=43 ";

            //}
            //no branch, no dept, single empcode, no leavetype, no leavestatus, fromdate true and todate true
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType == string.Empty && LeaveStatus == string.Empty && LeaveStatus != "ALL" && branch != "0" && branch == "null" && empcode != string.Empty && dept != "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                    " and EmpId=" + empcode + " ";
                //cond2 = "and e.Branch!=43 ";

            }
            //no branch, no dept, single empcode, no leavetype, leavestatus singlevalue not all, fromdate true and todate true
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType == string.Empty && LeaveStatus != string.Empty && LeaveStatus != "ALL" && branch != "0" && branch == "null" && empcode != string.Empty && dept != "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                    " and EmpId=" + empcode + " and status = '" + LeaveStatus + "'";
                //cond2 = "and e.Branch!=43 ";

            }
            //end
            //single branch,all LeaveType,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && branch != "0" && empcode != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + " and branchid in(" + branch + ")";
            }
            //sigle leave types
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch == "null" && dept == "null" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and  LeaveType ='" + LeaveType + "' ";
                // cond2 = " and lt.Code='" + LeaveType + "'";
                // cond2 = "and e.Branch!=43";
                // cond3 = "  and(lt.Code = '" + LeaveType + "')";
            }

            //empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch == "null" && branch != "0" && dept == "null" && dept != "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //cond1 = " Where (Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "')  and EmpId=" + empcode + "  ";
                // cond2 = " and lt.Code='" + LeaveType + "'";
                //cond2 = " and e.EmpId=" + empcode + "";
                // cond3 = "  and(lt.Code = '" + LeaveType + "')";
            }
            //all branch,sigle type,all status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && branch != "null" && empcode != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and" +
                        " (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId=" + empcode + " and branchid not in (" + branch + ")";
                }
                else
                {
                    cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and" +
                        " (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (LeaveType = '" + LeaveType + "')  and EmpId=" + empcode + " and branchid not in (" + branch + ")";
                }

            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && branch == "All" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))and EmpId=" + empcode + " and BrDept! ='" + branch + "'";
                // cond2 = " and lt.Code='" + LeaveType + "'";
            }
            //allbranch,singleLeaveType,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && branch == "0" && empcode != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))and EmpId=" + empcode + " " +
                    "and branchid not in(" + branch + ") and (LeaveType = '" + LeaveType + "') ";
                cond2 = "and e.Branch!=43";
                //cond2 = " and lt.Code='" + LeaveType + "'";
            }
            //allbranch,singleavetype
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && branch == "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and (LeaveType = '" + LeaveType + "') and  branchid not in (" + branch + ")";
                // cond2 = " and lt.Code='" + LeaveType + "' ";
                //cond2 = "and e.Branch!=43";
            }
            //allbranch,singleleavetype,singlestatus,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && LeaveType != string.Empty && branch == "0" && empcode != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "'  and EmpId=" + empcode + " and  branchid not in (" + branch + ") ";
                //cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }
            //allbranch,all types,all status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch == "0" && empcode != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and EmpId=" + empcode + " ";
                //cond2 = "and e.Branch!=43";
            }
            //allbranch,all types,all status
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch == "0" && empcode == string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','SML')) ";
            }
            //all bran,all leavetype,single status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && branch == "0" && empcode != string.Empty && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "' and EmpId=" + empcode + " and branchid not  in(" + branch + ")";
                // cond2 = " and  lv.Status = '" + LeaveStatus + "'";
                // cond2 = "and e.Branch!=43";
                //cond3 = " and lv.Type = '" + LeaveStatus + "'";
            }

            //all dept
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && empcode == string.Empty && dept == "0" && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and deptid  not in (" + dept + ")";
                //cond2 = "and e.Branch!=43";
            }
            //all dept,empcode
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && empcode != string.Empty && dept == "0" && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))and EmpId=" + empcode + " and deptid  not in (" + dept + ")";
                cond2 = "and e.Branch!=43";
            }
            //all dept,all leave types,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && dept == "0" && empcode != string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + " and deptid  not in (" + dept + ") ";
                cond2 = "and e.Branch!=43";
            }
            //single dept,all LeaveType,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && dept != "0" && empcode != string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + " and deptid in(" + dept + ")";
            }
            //all dept,sigle type,all status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && dept == "0" && branch == "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and" +
                        " (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId=" + empcode + " ";
                }
                else
                {
                    cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and" +
                        " (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (LeaveType = '" + LeaveType + "')  and EmpId=" + empcode + " ";
                }
            }
            //alldept,singleLeaveType,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && dept == "0" && empcode != string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))and EmpId=" + empcode + " " +
                    "and deptid not in(" + dept + ") and (LeaveType = '" + LeaveType + "') ";
                //cond2 = "and e.Branch!=43";
                //cond2 = " and lt.Code='" + LeaveType + "'";
            }
            //alldept,singleavetype
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && dept == "0" && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and (LeaveType = '" + LeaveType + "') and  deptid not in (" + dept + ")";
                // cond2 = " and lt.Code='" + LeaveType + "' ";
                //cond2 = "and e.Branch!=43";
            }
            //alldept,singleleavetype,singlestatus,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && LeaveType != string.Empty && dept == "0" && empcode != string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and  LeaveType ='" + LeaveType + "' and status = '" + LeaveStatus + "'  and EmpId=" + empcode + " and  deptid not in (" + dept + ") ";
                //cond2 = "and e.Branch!=43";
                //cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }
            //alldept,all types,all status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && dept == "0" && empcode != string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + " and deptid not in (" + dept + ")";
                //cond2 = "and e.Branch!=43";
            }
            //alldept,all types,all status
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && dept == "0" && empcode == string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))  and deptid not in (" + dept + ")";
                //cond2 = "and e.Branch=43";
            }
            //all dept,all leavetype,single status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && dept == "0" && empcode != string.Empty && branch == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "' and EmpId=" + empcode + " and deptid not  in(" + dept + ")";
                // cond2 = " and  lv.Status = '" + LeaveStatus + "'";
                //cond2 = "and e.Branch=43";
                //cond3 = " and lv.Type = '" + LeaveStatus + "'";
            }


            //allbranch,all dept
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && empcode == string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  ";
            }
            //allbranch,all dept,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && empcode != string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where    EmpId=" + empcode + " and ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  ";
            }
            //allbranch,all dept,empcode,status all
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL" && empcode != string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId=" + empcode + " ";

            }
            //sinle leave status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus != "ALL" && empcode != string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "   and EmpId=" + empcode + " and status in('" + LeaveStatus + "')  ";
            }
            //all leave types,all leave status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && empcode != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and (status IN('Pending',  'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + " ";
                //cond1 = " Where ((Convert(date,StartDate,103)  >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and " +
                //        "Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                //        "and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                //        "and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                //        "and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //       "  and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL')) and EmpId=" + empcode + " ";
            }
            //all leavetypes,sinle status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && empcode != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "' and EmpId=" + empcode + "  and deptid not in(" + dept + ")";
                //cond2 = " and  lv.Status = '" + LeaveStatus + "'";

                //cond3 = " and lv.Type = '" + LeaveStatus + "'";
            }
            //single leavetype,all leave status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && empcode != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId=" + empcode + "";
                }
                else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and EmpId=" + empcode + "";
                }

            }
            //single status,single leave type,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus == string.Empty && LeaveStatus != "ALL" && empcode != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //if (LeaveType == "LTC" || LeaveType == "PLE")
                //{
                //    cond1 = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //           "  and  EmpId=" + empcode + " ";
                //}
                //else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and  LeaveType ='" + LeaveType + "'  and EmpId=" + empcode + " ";
                }
            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus != string.Empty && LeaveStatus != "ALL" && empcode != string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           "  and status = '" + LeaveStatus + "' and EmpId=" + empcode + " ";
                }
                else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and  LeaveType ='" + LeaveType + "'  and status = '" + LeaveStatus + "' and EmpId=" + empcode + " ";
                }
            }
            //single leave types
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and  LeaveType ='" + LeaveType + "'";
            }
            //all leave types
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))  ";
            }
            //sinle leave status
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus != "ALL" && empcode == string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and status = '" + LeaveStatus + "'  ";
            }
            //all leave status
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus != "ALL" && empcode == string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and  (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and deptid not in(" + dept + ")";
            }
            //leavetypes,all status
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && empcode == string.Empty && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) ";
            }

            //all status,single leavetype
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus == "ALL" && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and" +
                        " (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) ";
                }
                else
                {
                    // cond1 = " Where ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or ) and" +
                    //" (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "'";

                    cond1 = "Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) ) and" +
                        " (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and LeaveType ='" + LeaveType + "'";
                }


                //cond2 = " and lt.Code='" + LeaveType + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')";
            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && (LeaveType == "ALLTypes" || LeaveType == string.Empty) && LeaveStatus == "ALL" && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and" +
                    " (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) ";



                //cond2 = " and lt.Code='" + LeaveType + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')";
            }

            //single leavetype,single leave status
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and  status = '" + LeaveStatus + "'   ";
                }
                else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and   LeaveType ='" + LeaveType + "' and  status = '" + LeaveStatus + "'  ";
                }
                // cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }
            // added new on 04/06/2020
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveStatus != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveStatus == "Debit")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and   LeaveType in ('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF') and  status in('" + LeaveStatus + "','DebitCancelled','CreditCancelled')";

                }
                else if (LeaveStatus == "Cancelled")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and   LeaveType in ('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF') and  status in('" + LeaveStatus + "','DebitCancelled','CreditCancelled')";
                }
                else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and   LeaveType in ('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF') and  status ='" + LeaveStatus + "'";

                }
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveStatus == string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && (branch == "0" || branch == "null") && (dept == "0" || dept == "null"))
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and   LeaveType in ('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF') ";
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && (branch != "0" && branch != "null") && (dept == "0" || dept == "null") && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and   LeaveType in ('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF') and  status = '" + LeaveStatus + "' and (branchid in (" + branch + ") or deptid not in (" + dept + "))";
            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && (branch != "0" && branch != "null") && (dept == "0" || dept == "null") && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and   LeaveType in ('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF') and  empId in(" + empcode + ")  and  status = '" + LeaveStatus + "' and (branchid in (" + branch + ") or deptid not in (" + dept + "))";
            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && (LeaveType == "ALLTypes" || LeaveType == "") && LeaveStatus != string.Empty && LeaveStatus != "ALL" && (branch == "0" || branch == "null") && (dept != "0" && dept != "null") && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and   LeaveType in ('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF') and  status = '" + LeaveStatus + "' and (branchid  not in (" + branch + ",43) or deptid in (" + dept + "))";
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && (LeaveType == "ALLTypes" || LeaveType == "") && LeaveStatus == string.Empty && LeaveStatus != "ALL" && (branch == "0" || branch == "null") && (dept != "0" && dept != "null") && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and   LeaveType in ('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF') and (branchid  not in (" + branch + ",43) or deptid in (" + dept + "))";
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && (LeaveType == "ALLTypes" || LeaveType == "") && LeaveStatus != string.Empty && LeaveStatus != "ALL" && (branch == "0" || branch == "null") && (dept != "0" && dept != "null") && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and   LeaveType in ('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF') and  empId in(" + empcode + ")  and  status = '" + LeaveStatus + "' and (branchid  not in (" + branch + ") or deptid in (" + dept + "))";
            }
            //nobranch,no dept,empcode,all leavetypes,single status
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && empcode != string.Empty && branch == "null" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))  and EmpId=" + empcode + "  and  status = '" + LeaveStatus + "'  ";
                // cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }
            //nobranch,no dept,all leavetypes,single status
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && empcode == string.Empty && branch == "null" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))    and  status = '" + LeaveStatus + "'  ";
                // cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }

            //nobranch,no dept,single leavetypes
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus == string.Empty && empcode == string.Empty && branch == "null" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and  LeaveType ='" + LeaveType + "'  ";
                // cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }

            //all fields empty
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && empcode == string.Empty && branch == "null" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "   ";
                // cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";
                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }
            //all fields empty
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveType == string.Empty && LeaveStatus != string.Empty && LeaveStatus != "ALL" && empcode == string.Empty && branch == "null" && branch != "0" && dept == "null" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and  status = '" + LeaveStatus + "'  ";
                // cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";
                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }


            //employee code removed
            else if (datetype == "Request" && fromDate != "-1" && toDate != "-2" && fromDate != string.Empty && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != string.Empty && branch == "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and BrDept!='All'";

            }


            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode == string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))  and (branchid ='" + branch + "' or deptid ='" + dept + "')";
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && empcode != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and EmpId in (" + empcode + ") and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))  and (branchid ='" + branch + "' or deptid ='" + dept + "')";
            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))";


                // cond2 = " and lt.Code='" + LeaveType + "'";

                cond3 = "  and(lt.Code = '" + LeaveType + "')";
            }


            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != string.Empty && branch == "All")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103)) and BrDept! ='" + branch + "'";
                //cond2 = " and lt.Code='" + LeaveType + "'";
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //initially it was a query in if condition
                //added by chaitanya on 17/04/2020
                if (branch != "null" && dept == "null")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and x.branchid in (" + branch + ")";
                }
                else if (branch == "null" && dept != "null")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and x.deptid in (" + dept + ")";
                }
                else if (branch != "null" && dept != "null")
                {
                    //cond1 = " Where  ((Convert(date,x.StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,x.StartDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,x.EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,x.EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                    //       "  and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (x.deptid in (" + dept + ") or x.branchid in (" + branch + ")) ";
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))";
                    //dcon = " Where  ((Convert(date,x.StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,x.StartDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,x.EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,x.EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                    //     "  and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (x.deptid in (" + dept + ") or x.branchid in (" + branch + "))";
                }
                //end

            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus != "ALL" && branch == "All")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and status = '" + LeaveStatus + "'  and BrDept !='" + branch + "'";
            }

            //@@@@@branch,department
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && empcode == string.Empty && (LeaveType == string.Empty || LeaveType == "ALLTypes") && (LeaveStatus == string.Empty || LeaveStatus == "ALL") && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "   and (branchid  in (" + branch + ") or deptid  in (" + dept + "))";

            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && empcode != string.Empty && (LeaveType == string.Empty || LeaveType == "ALLTypes") && (LeaveStatus == string.Empty || LeaveStatus == "ALL") && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and EmpId in(" + empcode + ") and (branchid  in (" + branch + ") or deptid  in (" + dept + "))";

            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && empcode == string.Empty && (LeaveType == string.Empty || LeaveType == "ALLTypes") && LeaveStatus != "ALL" && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and Status ='" + LeaveStatus + "' and (branchid  in (" + branch + ") or deptid  in (" + dept + "))";

            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && empcode != string.Empty && (LeaveType == string.Empty || LeaveType == "ALLTypes") && LeaveStatus != "ALL" && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and empId in (" + empcode + ") and Status ='" + LeaveStatus + "' and (branchid  in (" + branch + ") or deptid  in (" + dept + "))";

            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && empcode == string.Empty && (LeaveType != string.Empty && LeaveType != "ALLTypes") && (LeaveStatus == "ALL" || LeaveStatus == string.Empty) && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //if (LeaveType == "LTC" || LeaveType == "PLE")
                //{
                //    cond1 = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //           "   and (branchid  in (" + branch + ") or deptid  in (" + dept + "))";
                //}
                //else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           "  and LeaveType ='" + LeaveType + "' and (branchid  in (" + branch + ") or deptid  in (" + dept + "))";
                }

            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && empcode != string.Empty && (LeaveType != string.Empty && LeaveType != "ALLTypes") && (LeaveStatus == "ALL" || LeaveStatus == string.Empty) && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //if (LeaveType == "LTC" || LeaveType == "PLE")
                //{
                //    cond1 = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //           "  and empId in(" + empcode + ") and (branchid  in (" + branch + ") or deptid  in (" + dept + "))";
                //}
                //else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and empId in(" + empcode + ") and LeaveType ='" + LeaveType + "' and (branchid  in (" + branch + ") or deptid  in (" + dept + "))";
                }

            }
            //Newly added by chaitanya on 18/04/2020
            //Leavetype =alltype, status =all and department has a single value other than All
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch == "null" && branch != "0" && dept != "0" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                   "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and  deptid in(" + dept + ")";
            }
            //Leavetype =alltype, status =all and branch has a single value other than All
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != "null" && branch != "0" && dept != "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                   "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and  branchid in(" + branch + ")";
            }
            //end

            //all leavestatus,alltype -- (added new condition branch!="null" by chaitanya on 18/04/2020)
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != "0" && branch != "null" && dept != "0" && dept != "null" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                   "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and  (branchid in(" + branch + ") or deptid in(" + dept + "))";

            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && branch != "0" && branch != "null" && dept != "0" && dept != "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                   "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and  (branchid in(" + branch + ") or deptid in(" + dept + ")) and empId in(" + empcode + ")";

            }

            //added new condition by chaitanya on 18/04/2020
            //allleavetypes,single status department value other than All
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && branch != "0" && branch == "null" && dept != "0" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                     " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "' and  deptid in(" + dept + ")";
            }
            //allleavetypes,single status branch value other than All
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && branch != "0" && branch != "null" && dept != "0" && dept == "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "' and  branchid in(" + branch + ")";
            }
            //end
            //all leavetypes,single status (added new condition by chaitanya on 18/04/2020 branch!="null" && dept!="null") 
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && branch != "0" && branch != "null" && dept != "0" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "' and  (branchid in(" + branch + ") or deptid in(" + dept + "))";

                //dcon = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //     " and (LeaveType IN('CL', 'PL', 'LOP', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL')) and status = '" + LeaveStatus + "' and  deptid in(" + dept + ")";
                //cond2 = " and  lv.Status = '" + LeaveStatus + "'";

                //cond3 = " and lv.Type = '" + LeaveStatus + "'";
            }

            //newly added by chaitanya on 18/04/2020 selecting all fields except all and status =all
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && branch != "0" && dept != "0" && branch != "null" && dept != "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                          " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and Empid in(" + empcode + ") and (branchid in(" + branch + ") or deptid in (" + dept + "))";
                }
                else
                {

                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                          " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and Empid in (" + empcode + ")  and (branchid in(" + branch + ") or deptid in (" + dept + "))";
                }


            }
            //end

            //single leave type,all status
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && branch != "0" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                //commented by chaitanya on 16/04/2020
                //bcon = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //       " and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and branchid in(" + branch + ")";

                //dcon = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //      " and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and deptid in(" + dept + ")";
                //end    
                //newly added by chaitanya on 17/04/2020
                if (branch == "null" && dept == "null")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "'";
                }
                else if (branch != "null" && dept == "null")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and branchid in(" + branch + ")";
                }
                else if (branch == "null" && dept != "null")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                      " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and deptid in(" + dept + ")";
                }
                else if (branch != "null" && dept != "null")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                      " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and( branchid in(" + branch + ") or deptid in(" + dept + "))";

                    //dcon = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,StartDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                    //  " and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and deptid in(" + dept + ") ";
                }
                //end

                //cond2 = " and lt.Code='" + LeaveType + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')";
            }
            //newly added by chaitanya on 17/04/2020
            //branch,department,empcode,fromdate,todate,leavetype,status==""
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && LeaveStatus == string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and  LeaveType ='" + LeaveType + "'  and EmpId = '" + empcode + "'  and branchid in (" + branch + ") ";

                cond2 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                 " and  LeaveType ='" + LeaveType + "' and  EmpId = '" + empcode + "' and deptid in (" + dept + ") ";
                //cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }
            //end

            //newly added by chaitanya on 17/04/2020
            //select all values apart from All 
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != "null" && LeaveStatus != "ALL" && LeaveStatus != "null" && branch != "0" && dept != "0" && branch != "null" && dept != "null" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and status = '" + LeaveStatus + "'  and EmpId = '" + empcode + "' and (branchid in (" + branch + ") or deptid in (" + dept + "))";
                }
                else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and  LeaveType ='" + LeaveType + "'  and status = '" + LeaveStatus + "'  and EmpId = '" + empcode + "' and (branchid in (" + branch + ") or deptid in (" + dept + "))";
                }

                //cond2 = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                // " and  LeaveType ='" + LeaveType + "' and  status = '" + LeaveStatus + "' and EmpId = '" + empcode + "' and deptid in (" + dept + ") ";
                //cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }
            //end

            //single leavetype,single status 
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType == string.Empty && LeaveStatus != "ALL" && LeaveStatus == String.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);

                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and (branchid in (" + branch + ") or deptid in (" + dept + ")) ";


            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveType != string.Empty && LeaveStatus != "ALL" && LeaveStatus != string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //if (LeaveType == "LTC" || LeaveType == "PLE")
                //{
                //    cond1 = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //           "   and status = '" + LeaveStatus + "'  and (branchid in (" + branch + ") or deptid in (" + dept + ")) ";

                //}
                //else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and  LeaveType ='" + LeaveType + "'  and status = '" + LeaveStatus + "'   and (branchid in (" + branch + ") or deptid in (" + dept + ")) ";

                }
                //cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }
            //branch,department,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus == string.Empty && empcode != string.Empty && branch != "0" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "   and EmpId=" + empcode + " and branchid  in (" + branch + ")";

                cond2 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "   and EmpId=" + empcode + " and deptid  in (" + dept + ")";
            }
            //all leavestatus,alltype,,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL" && empcode != string.Empty && branch != "0" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','SML')) and EmpId=" + empcode + " and  branchid in(" + branch + ")";

                cond2 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','SML')) and EmpId=" + empcode + " and  deptid in(" + dept + ")";
            }
            //all leavetypes,single status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL" && empcode != string.Empty && branch != "0" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "' and EmpId=" + empcode + " and  branchid in(" + branch + ")";

                cond2 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                     " and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "' and EmpId=" + empcode + " and  deptid in(" + dept + ")";
                //cond2 = " and  lv.Status = '" + LeaveStatus + "'";

                //cond3 = " and lv.Type = '" + LeaveStatus + "'";
            }
            //single leave type,all status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == "ALL" && empcode != string.Empty && branch != "0" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and EmpId=" + empcode + "  and branchid in(" + branch + ")";

                cond2 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                      " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))  and LeaveType ='" + LeaveType + "' and EmpId=" + empcode + " and deptid in(" + dept + ")";
                //cond2 = " and lt.Code='" + LeaveType + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')";
            }
            //single leavetype,single status,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && toDate != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != "ALL" && empcode != string.Empty && branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and  LeaveType ='" + LeaveType + "' and EmpId=" + empcode + " and branchid in (" + branch + ") ";

                cond2 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                 " and  LeaveType ='" + LeaveType + "' and EmpId=" + empcode + " and deptid in (" + dept + ") ";
                //cond2 = " and lt.Code='" + LeaveType + "'" + " and lv.Status = '" + LeaveStatus + "'";

                //cond3 = "  and(lt.Code = '" + LeaveType + "')and lv.Type = '" + LeaveStatus + "'";
            }

            //single branch,all dept
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus == string.Empty && branch != "0" && dept == "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "   and (branchid  in (" + branch + ") or deptid  not in (" + dept + ",46))";

            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus != "ALL" && branch == "0" && dept != "0" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('" + LeaveStatus + "'))   and (branchid not in (" + branch + ",43) or deptid  in (" + dept + "))";


                }
                else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and (status IN('" + LeaveStatus + "'))  and LeaveType IN('" + LeaveType + "')   and (branchid not in (" + branch + ",43) or deptid in (" + dept + "))";

                }
            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus != "ALL" && branch != "0" && dept == "0" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('" + LeaveStatus + "'))   and (branchid  in (" + branch + ") or deptid  not in (" + dept + ",46))";


                }
                else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and (status IN('" + LeaveStatus + "'))  and LeaveType IN('" + LeaveType + "')   and (branchid  in (" + branch + ") or deptid  not in (" + dept + ",46))";

                }
            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus == "ALL" && branch != "0" && dept == "0" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))   and (branchid  in (" + branch + ") or deptid  not in (" + dept + ",46))";


                }
                else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and LeaveType IN('" + LeaveType + "')   and (branchid  in (" + branch + ") or deptid  not in (" + dept + ",46))";

                }
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus == "ALL" && branch != "0" && dept == "0" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId in (" + empcode + ")  and (branchid  in (" + branch + ") or deptid  not in (" + dept + ",46))";


                }
                else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and EmpId in (" + empcode + ") and LeaveType IN('" + LeaveType + "')   and (branchid  in (" + branch + ") or deptid  not in (" + dept + ",46))";

                }
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus == "ALL" && branch == "0" && dept != "0" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //if (LeaveType == "LTC" || LeaveType == "PLE")
                //{
                //    cond1 = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //       " and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))   and (branchid not in (" + branch + ",43) or deptid   in (" + dept + "))";


                //}
                //else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and LeaveType IN('" + LeaveType + "')   and (branchid not in (" + branch + ",43) or deptid  in (" + dept + "))";

                }
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus == "ALL" && branch == "0" && dept != "0" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //if (LeaveType == "LTC" || LeaveType == "PLE")
                //{
                //    cond1 = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //       " and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))   and (branchid not in (" + branch + ",43) or deptid   in (" + dept + ")) and EmpId in (" + empcode + ")";


                //}
                //else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and LeaveType IN('" + LeaveType + "')   and (branchid not in (" + branch + ",43) or deptid  in (" + dept + ")) and EmpId in (" + empcode + ")";

                }
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch == "0" && dept != "0" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //if (LeaveType == "LTC" || LeaveType == "PLE")
                //{
                //    cond1 = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //       " and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))   and (branchid not in (" + branch + ",43) or deptid   in (" + dept + "))";


                //}
                //else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and LeaveType IN('" + LeaveType + "')   and (branchid not in (" + branch + ",43) or deptid  in (" + dept + "))";

                }
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty && branch != "0" && dept == "0" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                if (LeaveType == "LTC" || LeaveType == "PLE")
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))   and (branchid  in (" + branch + ") or deptid  not in (" + dept + ",46))";


                }
                else
                {
                    cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                           " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and LeaveType IN('" + LeaveType + "')   and (branchid  in (" + branch + ") or deptid  not in (" + dept + ",46))";

                }
            }

            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus == "ALL" && branch != "0" && dept == "0" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))   and (branchid  in (" + branch + ") or deptid not in(" + dept + "))";


            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus == "ALL" && branch != "0" && dept == "0" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + "  and ( branchid  in (" + branch + ") or deptid  not in (" + dept + ") )";

            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus == "ALL" && branch == "0" && dept != "0" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //cond1 = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //       " and (status IN('Pending', 'Debit', 'Credit', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (LeaveType IN('CL', 'PL', 'LOP', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL'))   and (branchid not  in (" + branch + ",43) or deptid   in (" + dept + "))";
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))   and (branchid not  in (" + branch + ",43) or deptid   in (" + dept + "))";

            }
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveType == "ALLTypes" && LeaveStatus != string.Empty && LeaveStatus == "ALL" && branch == "0" && dept != "0" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and EmpId=" + empcode + "  and (branchid not  in (" + branch + ",43) or deptid in(" + dept + "))";


            }
            //end
            //all branch,single dept
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus == string.Empty && branch == "0" && dept != "0" && empcode == string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "   and branchid not  in (" + branch + ",43)";

                cond2 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "   and deptid   in (" + dept + ")";
            }
            //all branch,single dept,empcode
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType == string.Empty && LeaveStatus == string.Empty && branch == "0" && dept != "0" && empcode != string.Empty)
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       " and EmpId=" + empcode + " and (branchid not in (" + branch + ",43) or deptid in (" + dept + ")) ";

            }
            //all branch,single dept,empcode,single leave type
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveStatus == string.Empty && empcode != string.Empty && branch == "0" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and LeaveType ='" + LeaveType + "' and EmpId=" + empcode + "  and (branchid not  in (" + branch + ",43) or deptid in (" + dept + "))";

                cond2 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "   and LeaveType ='" + LeaveType + "' and EmpId=" + empcode + "  and deptid   in (" + dept + ")";
            }
            //all branch,single dept,empcode,single leave type,single leave status
            else if (datetype == "Request" && fromDate != "-1" && fromDate != string.Empty && toDate != "-2" && LeaveType != string.Empty && LeaveStatus != string.Empty && empcode != string.Empty && branch == "0" && dept != "0")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                //cond1 = " Where  ((Convert(date,StartDate,103) >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) <= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) >= '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "'))" +
                //       "  and LeaveType ='" + LeaveType + "'  and status = '" + LeaveStatus + "' and EmpId=" + empcode + "  and branchid not  in (" + branch + ",43)";
                cond1 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "  and LeaveType ='" + LeaveType + "'  and status = '" + LeaveStatus + "' and EmpId=" + empcode + "  and (branchid not in (" + branch + ",43) or deptid in (" + dept + "))";

                cond2 = " Where  ((Convert(date,StartDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "'and '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or (Convert(date,EndDate,103) between '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and'" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "') or '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103) or '" + tdt.ToString("yyyy-MM-dd 00:00:00.000") + "' between Convert(date,StartDate,103) and  Convert(date,EndDate,103))" +
                       "   and LeaveType ='" + LeaveType + "'  and status = '" + LeaveStatus + "' and EmpId=" + empcode + "  and deptid   in (" + dept + ")";
            }
            else if (datetype == "Request" && fromDate == string.Empty && toDate == string.Empty && LeaveType == "ALLTypes" && LeaveStatus != "ALL")
            {
                cond1 = " Where (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML')) and status = '" + LeaveStatus + "'";
            }
            else if (datetype == "Request" && fromDate == string.Empty && toDate == string.Empty && LeaveType == string.Empty && LeaveStatus == "ALL")
            {
                cond1 = " Where (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled'))";
            }
            else if (datetype == "Request" && fromDate == string.Empty && toDate == string.Empty && LeaveType == "ALLTypes" && LeaveStatus == "ALL")
            {
                cond1 = " Where (status IN('Pending', 'Debit','DebitCancelled', 'Credit','CreditCancelled', 'Forwarded', 'Approved', 'Denied', 'PartialCancelled', 'Cancelled')) and (LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))";
            }
            else if (datetype == "Request" && fromDate == string.Empty && toDate == string.Empty && LeaveType != "ALLTypes" && LeaveStatus == string.Empty)
            {
                cond1 = " Where  LeaveType ='" + LeaveType + "'";
            }
            else if (datetype == "Request" && fromDate == string.Empty && toDate == string.Empty && LeaveType == string.Empty && LeaveStatus != "ALL")
            {
                cond1 = " Where status = '" + LeaveStatus + "'";
            }

            string qry = "";
            if (fromDate == "-1" && toDate == null && datetype == null && LeaveType == null && LeaveStatus == null)
            {
                qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate," +
               "x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from (select e.EmpId, e.Id, e.ShortName as EmpName, " +
               "ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ," +
               "CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) " +
               "as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject," +
               "lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled') then lv.UpdatedDate else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') " +
               "join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId " +
               "join Departments d on d.Id = lv.DepartmentId where e.empid=0 and  e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)  " + cond2 + "  " +
               "union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) " +
               "as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate," +
               "CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays," +
               "lv.Type,lv.comments as Subject,lv.Comments,l.CancelReason,(case when lv.Type not in('Credit','Debit') then l.UpdatedBy else '' end) as UpdatedBy,(case when  lv.Type not in('Credit','Debit') then l.UpdatedDate else null end) as UpdatedDate from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  join LeaveTypes lt on lt.Id = lv.LeaveTypeId " +
               "join Designations ds on ds.Id = lv.CurrentDesignation join Leaves l on e.Id = l.EmpId  join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department " +
               "where e.empid=0 and e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) " + cond2 + ") as x  " + cond1;


                //qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate," +
                //    "x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from(select e.EmpId, e.Id, e.ShortName as EmpName, " +
                //    "ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ," +
                //    "CONVERT(VARCHAR, lv.StartDate, 103) as StartDate, CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) " +
                //    "as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime,lv.LeaveDays as LeaveDays,lv.status as status,lv.subject as Subject," +
                //    "lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled') then lv.UpdatedDate else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') " +
                //    "join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId " +
                //    "join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) " + cond2 + " " +
                //    "union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) " +
                //    "as BrDept,lt.Code as LeaveType ,CONVERT(varchar, l.UpdatedDate, 103),CONVERT(varchar, l.UpdatedDate, 103),CONVERT(varchar, l.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, l.UpdatedDate, 108) AS AppliedTime,L.LeaveDays as LeaveDays," +
                //    "l.Status,l.Subject as Subject,l.Reason,l.CancelReason,(case when l.Status  in('Cancelled') then l.UpdatedBy else '' end) as UpdatedBy,(case when l.status  in('Cancelled') then l.UpdatedDate else null end) as UpdatedDate from Employees e join Leaves l on e.Id = l.EmpId join LeaveTypes lt on lt.Id = l.leavetype " +
                //    "join Designations ds on ds.Id = l.DesignationId join Branches b on b.Id = l.BranchId join Departments d on d.Id = l.DepartmentId " +
                //    "where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) " + cond3 + ") as x " +cond1;
            }
            //else if(datatype1=="branch")
            //{
            //    qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate," +
            //        "x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from(select e.EmpId, e.Id, e.ShortName as EmpName, " +
            //        "ds.[code] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ," +
            //        "CONVERT(VARCHAR, lv.StartDate, 103) as StartDate, CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) " +
            //        "as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays as LeaveDays,lv.status as status,lv.subject as Subject," +
            //        "lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled','PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled','PartialCancelled') then FORMAT(lv.UpdatedDate,'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') " +
            //        "join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId " +
            //        "join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) and e.Branch!=43 " + cond2 + ") as x " + cond1;
            //}
            //else if (datatype1 == "department")
            //{
            //    qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate," +
            //        "x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from(select e.EmpId, e.Id, e.ShortName as EmpName, " +
            //        "ds.[code] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ," +
            //        "CONVERT(VARCHAR, lv.StartDate, 103) as StartDate, CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) " +
            //        "as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays as LeaveDays,lv.status as status,lv.subject as Subject," +
            //        "lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled','PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled','PartialCancelled') then FORMAT(lv.UpdatedDate,'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') " +
            //        "join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId " +
            //        "join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) and e.Branch=43 " + cond2 + ") as x " + cond1;
            //}

            else if (branch != string.Empty && dept != string.Empty && branch != "0" && dept != "0" && branch != null && dept != null && branch != "null" && dept != "null")
            {
                if (LeaveType == "LTC")
                {
                    qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays as LeaveDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) and e.Branch!=43 )as x " + cond1 + "";
                }
                else if (LeaveType == "PLE")
                {
                    qry = "  select ROW_NUMBER() OVER(ORDER BY x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as LeaveDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate, CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) and e.Branch!=43 )as x " + cond1 + "";
                }
                else
                {
                    //qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid  from (select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled','PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)  and e.Branch!=43  union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,' ' as CancelReason,' '  as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) and e.Branch!=43) " +
                    //    "as x   " + cond1 +
                    //    " union all  select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)  and e.Branch = 43  union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) and e.Branch = 43) " +
                    //    "as x    " + cond2;
                    qry = "select ROW_NUMBER() OVER (ORDER BY a.empid) as Id,a.EmpId,a.LeaveId, a.EmpName,a.Designation,a.BrDept,a.StartDate,a.EndDate,a.AppliedDate,a.AppliedTime,a.LeaveType,a.LeaveDays,a.status,a.Subject,a.Reason,a.CancelReason,a.UpdatedBy,a.UpdatedDate,a.branchid,a.deptid from (select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from " +
                    "(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) )as x " +
                    "union all " +
                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) )as x" +
                    " union all " +
                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)) as x " +
                    "union all " +
                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,'' as Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,lv.Reason as CancelReason,(case when lv.Type in('DebitCancelled','CreditCancelled') then lv.UpdatedBy else '' end) as UpdatedBy, (case when lv.Type in('DebitCancelled','CreditCancelled') then FORMAT(lv.LCDTimeStamp, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) ) as x ) as a" + cond1 + "";
                }
            }
            //single branch,all dept
            else if (branch != string.Empty && dept != string.Empty && branch != "0" && dept == "0" && branch != "null" && dept != "null")
            {
                if (LeaveType == "LTC")
                {
                    qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays as LeaveDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) and e.Branch!=43 )as x " + cond1 + "";
                }
                else if (LeaveType == "PLE")
                {
                    qry = "  select ROW_NUMBER() OVER(ORDER BY x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as LeaveDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate, CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) and e.Branch!=43 )as x " + cond1 + "";
                }
                else
                {
                    //qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid  from (select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled','PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)  and e.Branch!=43  union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,' ' as CancelReason,' '  as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) and e.Branch!=43) " +
                    //    "as x   " + cond1 +
                    //    "union all  select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)  and e.Branch = 43  union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) and e.Branch = 43) " +
                    //    "as x    " + cond2;
                    qry = "select ROW_NUMBER() OVER (ORDER BY a.empid) as Id,a.EmpId,a.LeaveId, a.EmpName,a.Designation,a.BrDept,a.StartDate,a.EndDate,a.AppliedDate,a.AppliedTime,a.LeaveType,a.LeaveDays,a.status,a.Subject,a.Reason,a.CancelReason,a.UpdatedBy,a.UpdatedDate,a.branchid,a.deptid from (select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from " +
                    "(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME))as x " +
                    " union all " +
                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) )as x " +
                    " union all " +
                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)) as x " +
                    " union all " +
                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,'' as Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,lv.Reason as CancelReason,(case when lv.Type in('DebitCancelled','CreditCancelled') then lv.UpdatedBy else '' end) as UpdatedBy, (case when lv.Type in('DebitCancelled','CreditCancelled') then FORMAT(lv.LCDTimeStamp, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) ) as x ) as a " + cond1 + "";
                }

            }
            //all branch,single dept
            else if (branch != string.Empty && dept != string.Empty && branch == "0" && dept != "0" && branch != "null" && dept != "null")
            {
                if (LeaveType == "LTC")
                {
                    qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays as LeaveDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.BranchId join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)  )as x " + cond1 + "";
                }
                else if (LeaveType == "PLE")
                {
                    qry = "  select ROW_NUMBER() OVER(ORDER BY x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as LeaveDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate, CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.BranchId join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) )as x " + cond1 + "";
                }
                else
                {
                    qry = "select ROW_NUMBER() OVER (ORDER BY a.empid) as Id,a.EmpId,a.LeaveId, a.EmpName,a.Designation,a.BrDept,a.StartDate,a.EndDate,a.AppliedDate,a.AppliedTime,a.LeaveType,a.LeaveDays,a.status,a.Subject,a.Reason,a.CancelReason,a.UpdatedBy,a.UpdatedDate,a.branchid,a.deptid from (select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from " +
                        "(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) )as x" +
                        " union all " +
                        "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.BranchId join Departments d on d.Id = e.DepartmentId where e.RetirementDate >=CAST(CAST(GETDATE() AS DATE) AS DATETIME) )as x " +
                        " union all " +
                        "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.BranchId join Departments d on d.Id = e.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)) as x " +
                        " union all " +
                        "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Comments,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,lv.Reason as CancelReason,(case when lv.Type in('DebitCancelled','CreditCancelled') then lv.UpdatedBy else '' end) as UpdatedBy, (case when lv.Type in('DebitCancelled','CreditCancelled') then FORMAT(lv.LCDTimeStamp, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = e.CurrentDesignation join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) ) as x  ) as a " + cond1 + "";
                }


            }
            else
            {
                if (LeaveType == "LTC")
                {
                    qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays as LeaveDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) )as x " + cond1 + "";
                }
                else if (LeaveType == "PLE")
                {
                    qry = "  select ROW_NUMBER() OVER(ORDER BY x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as LeaveDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate, CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = e.BranchId join Departments d on d.Id = e.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) )as x " + cond1 + "";
                }
                else
                {
                    //LTC and PLE added//
                    //qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= getdate()  " + cond2 + " )as x " +
                    //    " union all " +
                    //    " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) " + cond2 + " )as x " +
                    //    "union all" +
                    //    "  select ROW_NUMBER() OVER(ORDER BY x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate, CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) " + cond2 + " " +
                    //    "union all" +
                    //    " select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) " + cond2 + ") as x " + cond1 + "";

                    qry = "select ROW_NUMBER() OVER (ORDER BY a.empid) as Id,a.EmpId,a.LeaveId, a.EmpName,a.Designation,a.BrDept,a.StartDate,a.EndDate,a.AppliedDate,a.AppliedTime,a.LeaveType,a.LeaveDays,a.status,a.Subject,a.Reason,a.CancelReason,a.UpdatedBy,a.UpdatedDate,a.branchid,a.deptid from (" +
                        "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.leavetimestamp, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) )as x " +
                        " union all " +
                        "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)  )as x " +
                        " union all" +
                        " select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME))as x" +
                        " union all " +
                        " select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Comments,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,lv.Reason  as CancelReason,(case when lv.Type in('DebitCancelled','CreditCancelled') then lv.UpdatedBy else '' end) as UpdatedBy, (case when lv.Type in('DebitCancelled','CreditCancelled') then FORMAT(lv.LCDTimeStamp, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = e.CurrentDesignation join Branches b on b.Id = e.Branch join Departments d on d.Id = e.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) ) as x  ) as a " + cond1 + "";
                }

                // qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept," +
                //"x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid " +
                //" from (select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) " +
                //"as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate," +
                //"CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId," +
                //"lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled','PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy," +
                //"(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status " +
                //"not in ('Credited', 'Debited')" +
                //" join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId " +
                //"join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)  " + cond2 + "  " +
                //"union all select e.EmpId, e.Id, " +
                //"e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ," +
                //"CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate," +
                //"CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) " +
                //"as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,' ' as CancelReason,' '  as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  " +
                //"join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch " +
                //"join Departments d on d.Id = lv.Department where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) " + cond2 + ") as x  " + cond1;


                //qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate," +
                //    "x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from(select e.EmpId, e.Id, e.ShortName as EmpName, " +
                //    "ds.[code] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ," +
                //    "CONVERT(VARCHAR, lv.StartDate, 103) as StartDate, CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) " +
                //    "as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays as LeaveDays,lv.status as status,lv.subject as Subject," +
                //    "lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled','PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled','PartialCancelled') then FORMAT(lv.UpdatedDate,'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') " +
                //    "join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId " +
                //    "join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)  " + cond2 + ") as x " + cond1;
            }
            //string qry = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate," +
            //    "x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from (select e.EmpId, e.Id, e.ShortName as EmpName, " +
            //    "ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ," +
            //    "CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) " +
            //    "as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject," +
            //    "lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled') then lv.UpdatedDate else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') " +
            //    "join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId " +
            //    "join Departments d on d.Id = lv.DepartmentId where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)  " + cond2 + "  " +
            //    "union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) " +
            //    "as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate," +
            //    "CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays," +
            //    "lv.Type,lv.comments as Subject,lv.Comments,l.CancelReason,(case when lv.Type not in('Credit','Debit') then l.UpdatedBy else '' end) as UpdatedBy,(case when  lv.Type not in('Credit','Debit') then l.UpdatedDate else null end) as UpdatedDate from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  join LeaveTypes lt on lt.Id = lv.LeaveTypeId " +
            //    "join Designations ds on ds.Id = lv.CurrentDesignation join Leaves l on e.Id = l.EmpId  join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department " +
            //    "where e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) " + cond3 + ") as x  " + cond1;


            //string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id, lt.Id as ODId," +
            //          " e.EmpId,e.ShortName as Name,d.code as Designation," +
            //          " CONVERT(VARCHAR, lt.UpdatedDate, 103) as AppliedDate,case when b.id = 43 then dp.name else b.name end " +
            //          " as BranchDepartmet," +
            //          " case when lt.VistorFrom = 42 then(select name from Departments where id = dp.id) else " +
            //          " (select name from branches where id = b.id) end as Vfrom,lt.VistorTo," +
            //          " concat((CONVERT(VARCHAR, lt.startdate, 103)), ' ', right(convert(varchar(32), lt.startdate, 100), 8)) as " +
            //          " FromDate, concat((CONVERT(VARCHAR, lt.EndDate, 103)), ' ', right(convert(varchar(32), lt.EndDate,100), 8)) as ToDate," +
            //          " concat(datediff(day, lt.startdate, lt.enddate), ' days and ', CAST((lt.EndDate - lt.StartDate) as time(0))) " +
            //          " as Duration, m.odtype as Purpose,lt.Status,lt.Description" +
            //          " from   employees e  join designations d on e.CurrentDesignation = d.id" +
            //          " join departments dp on e.department = dp.id join branches b on e.branch = b.id join" +
            //          " OD_OtherDuty lt on lt.empid = e.id join OD_Master m on lt.Purpose = m.id " +
            //          " where e.RetirementDate >=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + cond1;


            return sh.Get_Table_FromQry(qry);
        }



        public DataTable AllOdDutyList(string fromDate, string toDate, string datetype)
        {

            string cond1 = "";
            if (fromDate == "-1" && toDate == null && datetype == null)
            {
                cond1 = " and " + "(" + "(lt.startdate = '" + "' and lt.enddate = '" + "')" + ")";
            }
            else if (fromDate != "-1" && toDate != "-2" && datetype != "Applied")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "and" + "(" + "(lt.startdate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.startdate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (lt.enddate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.startdate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "')" + ")";
            }
            else if (fromDate != "-1" && toDate != "-2" && datetype == "Applied")
            {
                DateTime fdt = fromDate == "" ? DateTime.Now : Convert.ToDateTime(fromDate);
                DateTime tdt = toDate == "" ? DateTime.Now : Convert.ToDateTime(toDate);
                cond1 = "and" + "(" + "(lt.UpdatedDate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.UpdatedDate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (lt.UpdatedDate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.UpdatedDate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "')" + ")";
            }
            else
            {
                cond1 = "";
            }

            string qry = " select CAST(ROW_NUMBER() over(order by department) AS INT) Id, lt.Id as ODId," +
                        " e.EmpId,e.ShortName as Name,d.code as Designation," +
                        " CONVERT(VARCHAR, lt.UpdatedDate, 103) as AppliedDate,case when b.id = 43 then dp.name else b.name end " +
                        " as BranchDepartmet," +
                        " case when lt.VistorFrom = 42 then(select name from Departments where id = dp.id) else " +
                        " (select name from branches where id = lt.VistorFrom) end as Vfrom,lt.VistorTo," +
                        " concat((CONVERT(VARCHAR, lt.startdate, 103)), ' ', right(convert(varchar(32), lt.startdate, 100), 8)) as " +
                        " FromDate, concat((CONVERT(VARCHAR, lt.EndDate, 103)), ' ', right(convert(varchar(32), lt.EndDate,100), 8)) as ToDate," +
                        " concat(datediff(day, lt.startdate, lt.enddate), ' days and ', CAST((lt.EndDate - lt.StartDate) as time(0))) " +
                        " as Duration, m.odtype as Purpose,lt.Status,lt.Description,lt.CancelReason" +
                        " from   employees e  join designations d on e.CurrentDesignation = d.id" +
                        " join departments dp on e.department = dp.id join branches b on e.branch = b.id join" +
                        " OD_OtherDuty lt on lt.empid = e.id join OD_Master m on lt.Purpose = m.id " +
                        " where e.RetirementDate >=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" + cond1;

            return sh.Get_Table_FromQry(qry);
        }
        public DataTable getSanctionWorkApprovalExcel(string EmpId, string empcode, string workdate)
        {
            if (empcode == "" || workdate == "")
            {
                string qry = "  SELECT DISTINCT wd.EmpId as EmpCode,ep.ShortName as Name," +
                    " case when br.Name='OtherBranch' then dp.Name else br.Name end as BranchDepartment, " +
                    "ds.Code as Designation, CONVERT(VARCHAR, wd.UpdatedDate, 103) as AppliedDate, CONVERT(VARCHAR, wd.WDDate, 103) as WorkDate, "
             //       "WorkName =STUFF((SELECT ', ' + name"
             // + " FROM workdiary_det workdet"
             //+ " WHERE workdet.wdid =" + " wd.Id" +
             //" FOR XML PATH('')), 1, 2, ''),"
             + " [WorkDescription] = STUFF((SELECT ', ' + [Desc]"
             + " FROM workdiary_det workdet" +
             " WHERE workdet.wdid =" + " wd.Id"
             + " FOR XML PATH('')), 1, 2, '')," +
             " wd.Status"

             + " FROM WorkDiary wd "
             + "  join Employees ep on wd.EmpId = ep.EmpId"
             + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
             + " join workdiary_det workdet on wd.Id = workdet.WDId "
             + "  where wd.SA =" + EmpId + " and" + " wd.status =" + " 'Approved'" + "and" + " workdet.Name not in ('Leave','OD','LTC')" + " order by CONVERT(VARCHAR, wd.WDDate, 103) desc";
                return sh.Get_Table_FromQry(qry);
            }
            else
            {
                string wddate = Convert.ToDateTime(workdate).ToString("yyyy-MM-dd");
                string qry = "  SELECT DISTINCT wd.EmpId as EmpCode,ep.ShortName as Name," +
                     " case when br.Name='OtherBranch' then dp.Name else br.Name end as BranchDepartment, " +
                    "ds.Code as Designation, CONVERT(VARCHAR, wd.UpdatedDate, 103) as AppliedDate,CONVERT(VARCHAR, wd.WDDate, 103) as WorkDate, "
             //       "WorkName =STUFF((SELECT ', ' + name"
             // + " FROM workdiary_det workdet"
             //+ " WHERE workdet.wdid =" + " wd.Id" +
             //" FOR XML PATH('')), 1, 2, ''),"
             + " [WorkDescription] = STUFF((SELECT ', ' + [Desc]"
             + " FROM workdiary_det workdet" +
             " WHERE workdet.wdid =" + " wd.Id"
             + " FOR XML PATH('')), 1, 2, '')," +
             " wd.Status"

             + " FROM WorkDiary wd "
             + "  join Employees ep on wd.EmpId = ep.EmpId"
             + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
             + " join workdiary_det workdet on wd.Id = workdet.WDId "
             + "  where wd.SA =" + EmpId + " and" + " wd.status =" + " 'Approved'" + " and" + " wd.EmpId =" + empcode + " and" + " wd.WDDate = '" + wddate + "'" + "and" + " workdet.Name not in ('Leave','OD','LTC')" + " order by CONVERT(VARCHAR, wd.WDDate, 103) desc";
                return sh.Get_Table_FromQry(qry);
            }
        }

        public DataTable TodayOdDutyList()
        {

            string qry = " select CAST(ROW_NUMBER() over(order by e.id) AS INT) Id, lt.Id as ODId," +
                          " e.EmpId,e.ShortName as Name,d.code as Designation," +
                          " CONVERT(VARCHAR, lt.UpdatedDate, 103) as AppliedDate,case when b.id = 43 then dp.name" +
                          " else b.name end as BranchDepartmet," +
                          " case when lt.VistorFrom = 42 then(select name from Departments where id = dp.id) else" +
                          " (select name from branches where id = lt.VistorFrom) end as Vfrom,lt.VistorTo," +
                          " concat((CONVERT(VARCHAR, lt.startdate, 103)), ' ', right(convert(varchar(32)," +
                          " lt.startdate, 100), 8)) as FromDate," +
                          " concat((CONVERT(VARCHAR, lt.EndDate, 103)), ' ', right(convert(varchar(32)," +
                          " lt.EndDate, 100), 8)) as ToDate," +
                          " concat(datediff(day, lt.startdate, lt.enddate), ' days and ', CAST((lt.EndDate - lt.StartDate) as" +
                          " time(0))) as Duration, m.odtype as Purpose,lt.Status,lt.Description,lt.CancelReason" +
                          " from   employees e  join designations d on e.CurrentDesignation = d.id" +
                          " join departments dp on e.department = dp.id join branches b on e.branch = b.id join" +
                          " OD_OtherDuty lt on lt.empid = e.id join OD_Master m on lt.Purpose = m.id " +
                          " where lt.Status not in('Cancelled') and e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME))" +
                          " and (getdate() >= lt.startdate and convert(date, getdate()) <=convert(date, lt.enddate) or getdate() >= lt.startdate and" +
                          " getdate() <= lt.enddate)";

            return sh.Get_Table_FromQry(qry);
        }


        public DataTable TodayLeavesList(string branch, string empid, string datatype1, string dept)
        {
            datatype1 = null;
            string qry = "";
            string cond1 = "";
            string bcon = "";
            string dcon = "";
            string cond2 = "";
            //string branch1 = branch.Trim();
            //branch = branch1;
            if (branch != null && branch != "-1")
            {
                string[] branchdata = branch.Split(',');
                string[] deptdata = dept.Split(',');
                if (branchdata.Length > 1 || deptdata.Length > 1)
                {
                    foreach (var item in branchdata)
                    {
                        if (item == "0")
                        {
                            branch = "0";
                        }
                    }
                    foreach (var item in deptdata)
                    {
                        if (item == "0")
                        {
                            dept = "0";
                        }
                    }
                }
                //if (branch.Contains("-") && branch != "HeadOffice-All")
                //{
                //    branch = branch.Substring(1);
                //}
                branch = branch.Trim();
                if (branch.Contains("and"))
                {
                    branch = branch.Replace("and", "&");
                }
            }


            //single branch
            if (empid == "" && branch != "0" && dept == "null")
            {
                cond1 = " b.id  in (" + branch + ")  and b.Id!=43  and ";
            }
            //single branch,empcode
            else if (empid != "" && branch != "0" && dept == "null")
            {
                cond1 = " b.id  in (" + branch + ")and e.empid = '" + empid + "'  and b.Id!=43  and ";
            }
            //all branch
            else if (empid == "" && branch == "0" && dept == "null")
            {
                cond1 = " b.id not in (" + branch + ") and b.Id!=43 and ";
            }
            //all branch,empcode
            else if (empid != "" && branch == "0" && dept == "null")
            {
                cond1 = "b.id  not in(" + branch + ") and e.empid = '" + empid + "' and   b.Id!=43 and";
            }
            //all branch ,all dept
            else if (branch == "0" && dept == "0" && empid == "")
            {
                cond1 = "";
            }
            //all branch ,all dept,empcode
            else if (branch == "0" && dept == "0" && empid != "")
            {
                cond2 = " where empid = " + empid + "  ";
            }
            //***dept
            //single dept
            else if (empid == "" && dept != "0" && branch == "null")
            {
                cond1 = " d.id  in (" + dept + ")  and b.Id=43  and ";
            }
            //single dept,empcode
            else if (empid != "" && dept != "0" && branch == "null")
            {
                cond1 = " d.id  in (" + dept + ")and e.empid = '" + empid + "'  and b.Id=43  and ";
            }
            //all dept
            else if (empid == "" && dept == "0" && branch == "null")
            {
                cond1 = " d.id not in (" + dept + ") and b.Id=43 and ";
            }
            //all dept,empcode
            else if (empid != "" && dept == "0" && branch == "null")
            {
                cond1 = "d.id  not in(" + dept + ") and e.empid = '" + empid + "' and   b.Id=43 and";
            }
            //single branch,single depts
            else if (branch != "0" && dept != "0" && empid == "")
            {
                bcon = " b.id  in (" + branch + ")  and b.Id!=43  and ";
                dcon = " d.id  in (" + dept + ")  and b.Id=43  and ";
            }
            //single branch,all depts
            else if (branch != "0" && dept == "0" && empid == "")
            {
                bcon = " b.id  in (" + branch + ")  and b.Id!=43  and ";
                dcon = " d.id not in (" + dept + ")  and b.Id=43  and ";
            }
            //single branch,all depts,empcode
            else if (branch != "0" && dept == "0" && empid != "")
            {
                bcon = " b.id  in (" + branch + ")  and e.empid = '" + empid + "'  and b.Id!=43  and ";
                dcon = " d.id not in (" + dept + ") and e.empid = '" + empid + "'  and b.Id=43  and ";
            }
            //single branch,single depts,empcode 
            else if (branch != "0" && dept != "0" && empid != "")
            {
                bcon = " b.id  in (" + branch + ")   and b.Id!=43  and ";
                dcon = " d.id  in (" + dept + ")  and b.Id=43  and ";
                cond2 = " where empid = " + empid + "  ";
            }
            //all branch,single depts,empcode
            else if (branch == "0" && dept != "0" && empid != "")
            {
                bcon = " b.id not  in (" + branch + ")  and e.empid = '" + empid + "'  and b.Id!=43  and ";
                dcon = " d.id  in (" + dept + ") and e.empid = '" + empid + "'  and b.Id=43  and ";
            }
            //all branch,single depts
            else if (branch == "0" && dept != "0" && empid == "")
            {
                bcon = " b.id  not in (" + branch + ")  and b.Id!=43  and ";
                dcon = " d.id  in (" + dept + ")  and b.Id=43  and ";
            }

            else if (empid != "" && branch != "" && branch != "0")
            {
                cond1 = "e.empid = '" + empid + "' and (b.Name = '" + branch + "'  or d.Name = '" + branch + "') and ";
            }
            else if (empid != "" && branch != "" && branch != "0")
            {
                cond1 = "e.empid = '" + empid + "' and (b.id in (" + branch + "))   and ";
            }
            //else if (empid != "" && branch != "All")
            //{
            //    cond1 = "e.empid = '" + empid + "' and ";
            //}
            else if (empid != "" && branch == "All")
            {
                cond1 = "e.empid = '" + empid + "' and  ";
            }
            else if (empid != "" && branch == "")
            {
                cond1 = "e.empid = '" + empid + "' and  ";
            }
            //    if (datatype1 == null)
            //    {
            //        qry = " select ROW_NUMBER() OVER(ORDER BY x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation, x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate " +
            //"from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[code] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept,lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate," +
            //"CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status as status,lv.subject as Subject," +
            //"lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled') then FORMAT(lv.UpdatedDate,'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate " +
            //"from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt " +
            //"on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId " +
            //"join Departments d on d.Id = lv.DepartmentId where " + cond1 + " " +
            //" lv.StartDate<=convert(Date,getdate())  and lv.EndDate>=convert(Date,getdate())   and e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " +
            //") as x";
            //    }
            if (branch == "-1" && empid == "-2")
            {
                empid = "0";
                branch = "0";
                //cond1 = "e.empid = " + empid + " and b.Name = '" + branch + "' and ";
                qry = "  select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation, x.BrDept,x.StartDate,x.EndDate," +
                   "x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from(select e.EmpId, e.Id, e.ShortName as EmpName," +
                   " ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ," +
                   "CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) " +
                   "as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject," +
                   "lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled') then lv.UpdatedDate else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') " +
                   "join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId " +
                   "join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId  where e.empid=0 and e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " +
                   "and ((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) " +
                   "or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))     " +
                   "union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end )" +
                   " as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103)," +
                   "CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId," +
                   "(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject," +
                   "lv.Comments,l.CancelReason,(case when lv.Type not in('Credit','Debit') then l.UpdatedBy else '' end) as UpdatedBy,(case when  lv.Type not in('Credit','Debit') then l.UpdatedDate else null end) as UpdatedDate from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId " +
                   "join Designations ds on ds.Id = lv.CurrentDesignation join Leaves l on e.Id = l.EmpId join Branches b on b.Id = lv.Branch " +
                   "join Departments d on d.Id = lv.Department where e.empid=0 and e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " +
                   "and ((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  " +
                   "and getdate() <= lv.UpdatedDate))) as x";
            }
            // singlebranch,single dept
            else if (branch != "0" && dept != "0" && branch != "null" && dept != "null")
            {
                qry = "  select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation, x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in ('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId  where   " + bcon + "  e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and ((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))     union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,' ' as CancelReason,' '  as UpdatedBy, ' ' as UpdatedDate from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where   " + bcon + "      e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and ((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))) as x " + cond2 + " " +
                    " union all select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation, x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in ('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId  where  " + dcon + "    e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))     union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where  " + dcon + "     e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))) as x " + cond2 + " ";

                //qry = "select ROW_NUMBER() OVER (ORDER BY a.empid) as Id,a.EmpId,a.LeaveId, a.EmpName,a.Designation,a.BrDept,a.StartDate,a.EndDate,a.AppliedDate,a.AppliedTime,a.LeaveType,a.LeaveDays,a.status,a.Subject,a.Reason,a.CancelReason,a.UpdatedBy,a.UpdatedDate,a.branchid,a.deptid from (" +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + bcon + " e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    " union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + bcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    " union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + bcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Comments,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x  " +
                //                    " union all " +

                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + dcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + dcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + dcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Comments,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x  ) as a " + cond2 + " ";
            }
            //single branch,all dept
            else if (branch != "0" && dept == "0" && branch != "null" && dept != "null")
            {
                qry = "  select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation, x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in ('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId  where  " + bcon + "    e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and ((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))     union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,' ' as CancelReason,' '  as UpdatedBy, ' ' as UpdatedDate from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where   " + bcon + "    e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and ((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))) as x " + cond2 + " " +
                    " union all select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation, x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in ('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId  where  " + dcon + "   e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))     union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where  " + dcon + "   e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))) as x " + cond2 + " ";


                //qry = "select ROW_NUMBER() OVER (ORDER BY a.empid) as Id,a.EmpId,a.LeaveId, a.EmpName,a.Designation,a.BrDept,a.StartDate,a.EndDate,a.AppliedDate,a.AppliedTime,a.LeaveType,a.LeaveDays,a.status,a.Subject,a.Reason,a.CancelReason,a.UpdatedBy,a.UpdatedDate,a.branchid,a.deptid from (" +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + bcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    " union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + bcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    " union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + bcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Comments,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x  " +
                //                    " union all " +

                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + dcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + dcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + dcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Comments,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x  ) as a " + cond2 + " ";
            }

            //all branch,single dept
            else if (branch == "0" && dept != "0" && branch != "null" && dept != "null")
            {
                qry = "  select row_number()  over(order by  x.id) as id, x.empid,x.leaveid, x.empname,x.designation, x.brdept,x.startdate,x.enddate,x.applieddate,x.appliedtime,x.leavetype,x.leavedays,x.status,x.subject,x.reason,x.cancelreason,x.updatedby,x.updateddate from(select e.empid, e.id, e.shortname as empname, ds.[name] as designation, (select case when b.id = 43 then d.name else b.name end) as brdept, lt.code as leavetype ,convert(varchar, lv.startdate, 103) as startdate,convert(varchar, lv.enddate, 103) as enddate,convert(varchar, lv.updateddate, 103) as applieddate,convert(varchar, lv.updateddate, 108) as appliedtime, lv.id as leaveid,lv.leavedays,lv.status,lv.subject as subject,lv.reason,lv.cancelreason,(case when lv.status in ('cancelled', 'partialcancelled') then lv.updatedby else '' end) as updatedby,(case when lv.status in('cancelled', 'partialcancelled') then format(lv.updateddate, 'dd/mm/yyyy, hh:mm tt') else null end) as updateddate from employees e join leaves lv on e.id = lv.empid  and lv.status not in ('credited', 'debited') join leavetypes lt on lt.id = lv.leavetype join designations ds on ds.id = lv.designationid join branches b on b.id = lv.branchid join departments d on d.id = lv.departmentid  where  " + bcon + "    e.retirementdate >= convert(date, getdate()) and ((getdate() >= lv.startdate and convert(date, getdate()) <= convert(date, lv.enddate)) or(getdate() >= lv.startdate and getdate() <= lv.enddate))     union all select e.empid, e.id, e.shortname as empname, ds.[name] as designation,(select case when b.id = 43 then d.name else b.name end ) as brdept,lt.code as leavetype ,convert(varchar, lv.updateddate, 103),convert(varchar, lv.updateddate, 103),convert(varchar, lv.updateddate, 103) as applieddate,convert(varchar, lv.updateddate, 108) as appliedtime, lv.id as leaveid,(case when lv.creditleave = 0 then lv.debitleave else lv.creditleave end) as leavedays,lv.type,lv.comments as subject,lv.comments,' ' as cancelreason,' '  as updatedby, ' ' as updateddate from employees e join leaves_creditdebit lv on e.id = lv.empid join leavetypes lt on lt.id = lv.leavetypeid join designations ds on ds.id = lv.currentdesignation join branches b on b.id = lv.branch join departments d on d.id = lv.department where   " + bcon + "    e.retirementdate >= convert(date, getdate()) and ((getdate() >= lv.updateddate and convert(date, getdate()) <= convert(date, lv.updateddate)) or(getdate() >= lv.updateddate  and getdate() <= lv.updateddate))) as x " + cond2 + " " +
                    " union all select row_number()  over(order by  x.id) as id, x.empid,x.leaveid, x.empname,x.designation, x.brdept,x.startdate,x.enddate,x.applieddate,x.appliedtime,x.leavetype,x.leavedays,x.status,x.subject,x.reason,x.cancelreason,x.updatedby,x.updateddate from(select e.empid, e.id, e.shortname as empname, ds.[name] as designation, (select case when b.id = 43 then d.name else b.name end) as brdept, lt.code as leavetype ,convert(varchar, lv.startdate, 103) as startdate,convert(varchar, lv.enddate, 103) as enddate,convert(varchar, lv.updateddate, 103) as applieddate,convert(varchar, lv.updateddate, 108) as appliedtime, lv.id as leaveid,lv.leavedays,lv.status,lv.subject as subject,lv.reason,lv.cancelreason,(case when lv.status in ('cancelled', 'partialcancelled') then lv.updatedby else '' end) as updatedby,(case when lv.status in('cancelled', 'partialcancelled') then format(lv.updateddate, 'dd/mm/yyyy, hh:mm tt') else null end) as updateddate from employees e join leaves lv on e.id = lv.empid  and lv.status not in ('credited', 'debited') join leavetypes lt on lt.id = lv.leavetype join designations ds on ds.id = lv.designationid join branches b on b.id = lv.branchid join departments d on d.id = lv.departmentid  where  " + dcon + "   e.retirementdate >= convert(date, getdate()) and((getdate() >= lv.startdate and convert(date, getdate()) <= convert(date, lv.enddate)) or(getdate() >= lv.startdate and getdate() <= lv.enddate))     union all select e.empid, e.id, e.shortname as empname, ds.[name] as designation,(select case when b.id = 43 then d.name else b.name end ) as brdept,lt.code as leavetype ,convert(varchar, lv.updateddate, 103),convert(varchar, lv.updateddate, 103),convert(varchar, lv.updateddate, 103) as applieddate,convert(varchar, lv.updateddate, 108) as appliedtime, lv.id as leaveid,(case when lv.creditleave = 0 then lv.debitleave else lv.creditleave end) as leavedays,lv.type,lv.comments as subject,lv.comments,' ' as cancelreason,' ' as updatedby, ' ' as updateddate from employees e join leaves_creditdebit lv on e.id = lv.empid join leavetypes lt on lt.id = lv.leavetypeid join designations ds on ds.id = lv.currentdesignation join branches b on b.id = lv.branch join departments d on d.id = lv.department where  " + dcon + "   e.retirementdate >= convert(date, getdate()) and((getdate() >= lv.updateddate and convert(date, getdate()) <= convert(date, lv.updateddate)) or(getdate() >= lv.updateddate  and getdate() <= lv.updateddate))) as x " + cond2 + " ";


                //qry = "select ROW_NUMBER() OVER (ORDER BY a.empid) as Id,a.EmpId,a.LeaveId, a.EmpName,a.Designation,a.BrDept,a.StartDate,a.EndDate,a.AppliedDate,a.AppliedTime,a.LeaveType,a.LeaveDays,a.status,a.Subject,a.Reason,a.CancelReason,a.UpdatedBy,a.UpdatedDate,a.branchid,a.deptid from (" +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + bcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    " union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + bcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    " union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + bcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Comments,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x  " +
                //                    " union all " +

                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + dcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + dcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + dcon + " e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x " +
                //                    "union all " +
                //                    "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Comments,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x  ) as a " + cond2 + " ";
            }
            else
            {
                qry = "  select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation, x.BrDept," +
                    "x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate " +
                    "from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else" +
                    " b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate," +
                    "CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate," +
                    "CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject," +
                    "lv.Reason,lv.CancelReason,(case when lv.status in ('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy," +
                    "(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') " +
                    "join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId" +
                    " join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId  " +
                    "where " + cond1 + " e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) and ((getdate() >= lv.StartDate and convert(date, getdate())" +
                    " <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate)) " +
                    "  union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation," +
                    "(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ," +
                    "CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103)" +
                    " as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId," +
                    "(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject," +
                    "lv.Comments,' ' as CancelReason,' '  as UpdatedBy, ' ' as UpdatedDate from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId " +
                    "join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch " +
                    "join Departments d on d.Id = lv.Department where " + cond1 + " e.RetirementDate >= convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) " +
                    "and ((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) " +
                    "or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))) as x" + cond2;

                //qry = "select ROW_NUMBER() OVER (ORDER BY a.empid) as Id,a.EmpId,a.LeaveId, a.EmpName,a.Designation,a.BrDept,a.StartDate,a.EndDate,a.AppliedDate,a.AppliedTime,a.LeaveType,a.LeaveDays,a.status,a.Subject,a.Reason,a.CancelReason,a.UpdatedBy,a.UpdatedDate,a.branchid,a.deptid from (" +
                //        "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where "+cond1 +" e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //        "union all " +
                //        "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.TotalDays,'' as CancelReason,lv.subject as Subject,'LTC'+lv.LTCType as Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join Leaves_LTC lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where " + cond1+" e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))  )as x " +
                //        "union all " +
                //        "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.PLEncash as TotalDays,x.status,x.Subject,x.Reason,'' as CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.UpdatedDate, 103) as StartDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.PLEncash,'' as CancelReason,lv.subject as Subject,lv.Reason,lv.status,(case when lv.status in('Cancelled', 'PartialCancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled', 'PartialCancelled') then FORMAT(lv.UpdatedDate, 'dd/MM/yyyy, hh:mm tt') else null end) as UpdatedDate,b.id as branchid,d.Id as deptid from Employees e join PLE_Type lv on e.Id = lv.EmpId and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where "+cond1+" e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x " +
                //        "union all " +
                //        "select x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Comments,x.CancelReason,x.UpdatedBy,x.UpdatedDate,x.branchid,x.deptid from(select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103) StartDate,CONVERT(varchar, lv.UpdatedDate, 103) EndDate,CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type as status,lv.comments as Subject,lv.Comments,' ' as CancelReason,' ' as UpdatedBy, ' ' as UpdatedDate ,b.id as branchid,d.Id as deptid from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where e.RetirementDate >= convert(Date, GETDATE()) and((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  and getdate() <= lv.UpdatedDate))  ) as x  ) as a "+cond2+" ";

            }

            //qry = "  select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation, x.BrDept,x.StartDate,x.EndDate," +
            //   "x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason,x.UpdatedBy,x.UpdatedDate from(select e.EmpId, e.Id, e.ShortName as EmpName," +
            //   " ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end) as BrDept, lt.Code as LeaveType ," +
            //   "CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) " +
            //   "as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject," +
            //   "lv.Reason,lv.CancelReason,(case when lv.status in('Cancelled') then lv.UpdatedBy else '' end) as UpdatedBy,(case when lv.status in('Cancelled') then lv.UpdatedDate else null end) as UpdatedDate from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') " +
            //   "join LeaveTypes lt on lt.Id = lv.LeaveType join Designations ds on ds.Id = lv.DesignationId " +
            //   "join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId  where " + cond1 + " e.RetirementDate >= convert(Date, GETDATE()) " +
            //   "and ((getdate() >= lv.StartDate and convert(date, getdate()) <= convert(date, lv.EndDate)) " +
            //   "or(getdate() >= lv.StartDate and getdate() <= lv.EndDate))    " +
            //   "union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end )" +
            //   " as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103)," +
            //   "CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId," +
            //   "(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject," +
            //   "lv.Comments,l.CancelReason,(case when lv.Type not in('Credit','Debit') then l.UpdatedBy else '' end) as UpdatedBy,(case when  lv.Type not in('Credit','Debit') then l.UpdatedDate else null end) as UpdatedDate from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId join LeaveTypes lt on lt.Id = lv.LeaveTypeId " +
            //   "join Designations ds on ds.Id = lv.CurrentDesignation join Leaves l on e.Id = l.EmpId join Branches b on b.Id = lv.Branch " +
            //   "join Departments d on d.Id = lv.Department where  " + cond1 + " e.RetirementDate >= convert(Date, GETDATE()) " +
            //   "and ((getdate() >= lv.UpdatedDate and convert(date, getdate()) <= convert(date, lv.UpdatedDate)) or(getdate() >= lv.UpdatedDate  " +
            //   "and getdate() <= lv.UpdatedDate))) as x";


            return sh.Get_Table_FromQry(qry);
        }

        public DataTable AllUserRolesViewList()
        {

            string qry = " select * from UserRoles order by Id asc";

            return sh.Get_Table_FromQry(qry);
        }
        // leaves        
        public DataTable YearWLBalance(string EmpIds)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString); SqlCommand cmd = new SqlCommand("EmployeeYearWiseLeavebalance", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@empid1", SqlDbType.VarChar);
            cmd.Parameters["@empid1"].Value = EmpIds;

            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            cmd.ExecuteNonQuery();
            da.Fill(dt);
            var count = dt.Rows;
            foreach (DataRow dr in dt.Rows)
            {
                var ltype = dr["LeaveType"];
                string selectQry = "select Type from LeaveTypes where Id=" + ltype;
                DataTable leavetype = sh.Get_Table_FromQry(selectQry);
                var st = leavetype.Rows[0]["Type"];
                var res = sh.Get_Table_FromQry(selectQry);
                dr["LeaveType"] = st;
                dt.AcceptChanges();
            }
            con.Close();
            return dt;
        }

        public string olddesig(int empid, string Id)
        {

            string qry = " SELECT top 1 NewDesignation from Employee_Transfer  where empid  =" + empid + " order by Id desc";
            DataTable dt = sh.Get_Table_FromQry(qry);
            string lcount1 = "";
            if (dt.Rows.Count > 0)
            {
                lcount1 = Convert.ToString(dt.Rows[0]["NewDesignation"]);
                return lcount1;
            }
            return lcount1;
        }

        public string NewBranch(int empid, string Id)
        {

            string qry = " SELECT  Branch from Employees  where Id  =" + empid + "";
            DataTable dt = sh.Get_Table_FromQry(qry);
            string lcount1 = "";
            if (dt.Rows.Count > 0)
            {
                lcount1 = Convert.ToString(dt.Rows[0]["Branch"]);
                return lcount1;
            }
            return lcount1;
        }


        public string oldBranch(int empid, string Id)
        {

            string qry = " SELECT top 1 OldBranch from Employee_Transfer  where empid  =" + empid + " and Type!='TemporaryTransfer' order by Id desc";
            DataTable dt = sh.Get_Table_FromQry(qry);
            string lcount1 = "";
            if (dt.Rows.Count > 0)
            {
                lcount1 = Convert.ToString(dt.Rows[0]["OldBranch"]);
                return lcount1;
            }
            return lcount1;
        }
        public string NewDept(int empid, string Id)
        {

            string qry = " SELECT Department from Employees  where Id  =" + empid + " ";
            DataTable dt = sh.Get_Table_FromQry(qry);
            string lcount1 = "";
            if (dt.Rows.Count > 0)
            {
                lcount1 = Convert.ToString(dt.Rows[0]["Department"]);
                return lcount1;
            }
            return lcount1;
        }
        public string oldDept(int empid, string Id)
        {

            string qry = " SELECT top 1 OldDepartment from Employee_Transfer  where empid  =" + empid + " and Type!='TemporaryTransfer' order by Id desc";
            DataTable dt = sh.Get_Table_FromQry(qry);
            string lcount1 = "";
            if (dt.Rows.Count > 0)
            {
                lcount1 = Convert.ToString(dt.Rows[0]["OldDepartment"]);
                return lcount1;
            }
            return lcount1;
        }
        //// leave apply controller 

        public DataTable SearchLeaveApplyController(string EmpIds, int lEmpId, string StartDate, string EndDate, string LeaveType)
        {
            string cond1 = "";
            string cond2 = "";
            string cond3 = "";
            if (StartDate == "-1" && EndDate == "-2")
            {

                DateTime fdt1 = StartDate == "" ? DateTime.Now : Convert.ToDateTime(StartDate);
                DateTime tdt1 = EndDate == "" ? DateTime.Now : Convert.ToDateTime(EndDate);
                //  cond1 = " and (lt.startdate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.startdate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (lt.enddate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.enddate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "')";
                // cond1 = "and (Convert(date,StartDate,103) >= '" + fdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and (Convert(date,EndDate,103) <= '" + tdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' ";
                cond1 = " and  (Convert(date,StartDate,103) >= '" + fdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,EndDate,103) <= '" + tdt1.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,StartDate,103) >= '" + fdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,StartDate,103) <= '" + tdt1.ToString("yyyy-MM-dd 00:00:00.000") + "')";
                //" ((leave.StartDate >= lStartdate && leave.EndDate <= lEnddate) || (leave.EndDate >= lStartdate && leave.StartDate <= lEnddate))";
                cond2 = " ";
            }
            else if (StartDate != "-1" && EndDate != "-2" && LeaveType == "11")
            {
                DateTime fdt1 = StartDate == "" ? DateTime.Now : Convert.ToDateTime(StartDate);
                DateTime tdt1 = EndDate == "" ? DateTime.Now : Convert.ToDateTime(EndDate);
                cond1 = " Where (Convert(date,StartDate,103) >= '" + fdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,StartDate,103) <= '" + tdt1.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,EndDate,103) >= '" + fdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,StartDate,103) <= '" + tdt1.ToString("yyyy-MM-dd 00:00:00.000") + "')" +
                       "  and(LeaveType IN('CL', 'PL', 'LOP','W-Off', 'All', 'C-OFF', 'SCL', 'EOL', 'MTL', 'ML', 'PTL','CW-OFF','SML'))";

                cond2 = " ";
            }
            else if (StartDate == "-1" && EndDate == "-2" && LeaveType == "")
            {

                DateTime fdt1 = StartDate == "" ? DateTime.Now : Convert.ToDateTime(StartDate);
                DateTime tdt1 = EndDate == "" ? DateTime.Now : Convert.ToDateTime(EndDate);
                //  cond1 = " and (lt.startdate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.startdate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (lt.enddate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.enddate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "')";
                // cond1 = "and (Convert(date,StartDate,103) >= '" + fdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and (Convert(date,EndDate,103) <= '" + tdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' ";
                cond1 = " ";
                cond2 = " ";
                //" ((leave.StartDate >= lStartdate && leave.EndDate <= lEnddate) || (leave.EndDate >= lStartdate && leave.StartDate <= lEnddate))";

            }
            else if (StartDate == "" && EndDate != "" && LeaveType != "")
            {


                DateTime fdt1 = StartDate == "" ? DateTime.Now : Convert.ToDateTime(StartDate);
                DateTime tdt1 = EndDate == "" ? DateTime.Now : Convert.ToDateTime(EndDate);
                //  cond1 = " and (lt.startdate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.startdate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (lt.enddate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.enddate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "')";
                // cond1 = "and (Convert(date,StartDate,103) >= '" + fdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and (Convert(date,EndDate,103) <= '" + tdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' ";
                cond1 = " ";
                cond2 = " ";
                //" ((leave.StartDate >= lStartdate && leave.EndDate <= lEnddate) || (leave.EndDate >= lStartdate && leave.StartDate <= lEnddate))";


            }
            else if (StartDate != "-1" && EndDate == "" && LeaveType == "")
            {

                //DateTime fdt1 = StartDate == "" ? DateTime.Now : Convert.ToDateTime(StartDate);
                //DateTime tdt1 = EndDate == "" ? DateTime.Now : Convert.ToDateTime(EndDate);
                //  cond1 = " and (lt.startdate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.startdate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (lt.enddate >= '" + fdt.ToString("yyyy-MM-dd 00:00:00.000") + "' and lt.enddate <= '" + tdt.ToString("yyyy-MM-dd 23:59:59.000") + "')";
                // cond1 = "and (Convert(date,StartDate,103) >= '" + fdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and (Convert(date,EndDate,103) <= '" + tdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' ";
                cond1 = " ";
                cond2 = " ";
                //" ((leave.StartDate >= lStartdate && leave.EndDate <= lEnddate) || (leave.EndDate >= lStartdate && leave.StartDate <= lEnddate))";

            }
            else if (StartDate != "-1" && EndDate != "-2" && LeaveType != "-3" && LeaveType != "11")
            {
                DateTime fdt1 = StartDate == "" ? DateTime.Now : Convert.ToDateTime(StartDate);
                DateTime tdt1 = EndDate == "" ? DateTime.Now : Convert.ToDateTime(EndDate);
                cond2 = " and LeaveType=" + LeaveType + "";
                cond1 = " Where (Convert(date,StartDate,103) >= '" + fdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,StartDate,103) <= '" + tdt1.ToString("yyyy-MM-dd 00:00:00.000") + "') or  (Convert(date,EndDate,103) >= '" + fdt1.ToString("yyyy-MM-dd 00:00:00.000") + "' and Convert(date,StartDate,103) <= '" + tdt1.ToString("yyyy-MM-dd 00:00:00.000") + "')";
                cond3 = "  and(lv.LeaveTypeId = '" + LeaveType + "')";
            }

            DateTime fdt = StartDate == "" ? DateTime.Now : Convert.ToDateTime(StartDate);
            DateTime tdt = EndDate == "" ? DateTime.Now : Convert.ToDateTime(EndDate);
            string lResults = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason from (select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.id = " + lEmpId + " and e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME) " + cond2 + "  union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 103) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,'' as CancelReason from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where lv.EmpId = " + lEmpId + " " + cond3 + " and e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)) as x  " + cond1;

            return sh.Get_Table_FromQry(lResults);
        }
        public DataTable LeaveApplyControllerapi(int lEmpId)
        {

            //string lResults = "select top 5 EmpId,ControllingAuthority,SanctioningAuthority,lt.Code as LeaveType,StartDate,EndDate,Reason,l.UpdatedBy,l.UpdatedDate,Status,LeaveDays,LeavesYear from  leaves l join LeaveTypes lt on lt.Id = l.LeaveType Where Empid = " + lEmpId + " order by l.UpdatedDate desc";
            string lResults = " select top 10 ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.ControllingAuthority as ControllingAuthority,x.SanctioningAuthority as SanctioningAuthority from (select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime,lv.UpdatedDate AS UpdatedDate, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.ControllingAuthority,lv.SanctioningAuthority from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.id = " + lEmpId + " and e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)   union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime,lv.UpdatedDate AS UpdatedDate, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,ControllingAuthority,SanctioningAuthority from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where lv.EmpId = " + lEmpId + "  and e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)) as x  order by x.UpdatedDate  DESC ";


            return sh.Get_Table_FromQry(lResults);
        }
        public DataTable LeaveApplyController(string EmpIds, int lEmpId)
        {

            string lResults = " select ROW_NUMBER()  OVER(ORDER BY  x.Id) As Id, x.EmpId,x.LeaveId, x.EmpName,x.Designation,x.BrDept,x.StartDate,x.EndDate,x.AppliedDate,x.AppliedTime,x.LeaveType,x.LeaveDays,x.status,x.Subject,x.Reason,x.CancelReason from (select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation, (select case when b.id = 43 then d.name else b.name end ) as BrDept, lt.Code as LeaveType ,CONVERT(VARCHAR, lv.StartDate, 103) as StartDate,CONVERT(VARCHAR, lv.EndDate, 103) as EndDate,CONVERT(VARCHAR, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,lv.LeaveDays,lv.status,lv.subject as Subject,lv.Reason,lv.CancelReason from Employees e join Leaves lv on e.Id = lv.EmpId  and lv.status not in ('Credited', 'Debited') join LeaveTypes lt on lt.Id = lv.LeaveType  join Designations ds on ds.Id = lv.DesignationId join Branches b on b.Id = lv.BranchId join Departments d on d.Id = lv.DepartmentId where e.id = " + lEmpId + " and e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)   union all select e.EmpId, e.Id, e.ShortName as EmpName, ds.[Name] as Designation,(select case when b.id = 43 then d.name else b.name end ) as BrDept,lt.Code as LeaveType ,CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103),CONVERT(varchar, lv.UpdatedDate, 103) as AppliedDate,CONVERT(varchar, lv.UpdatedDate, 108) AS AppliedTime, lv.Id as LeaveId,(case when lv.CreditLeave = 0 then lv.DebitLeave else lv.CreditLeave end) as LeaveDays,lv.Type,lv.comments as Subject,lv.Comments,'' CancelReason from Employees e join Leaves_CreditDebit lv on e.Id = lv.EmpId  join LeaveTypes lt on lt.Id = lv.LeaveTypeId join Designations ds on ds.Id = lv.CurrentDesignation join Branches b on b.Id = lv.Branch join Departments d on d.Id = lv.Department where lv.EmpId = " + lEmpId + "  and e.RetirementDate >= CAST(CAST(GETDATE() AS DATE) AS DATETIME)) as x  ";
            return sh.Get_Table_FromQry(lResults);
        }

        public DataTable AllLeaveApplyController(string EmpIds, int lEmpId, string StartDate, string EndDate)
        {
            string cond1 = "";
            DateTime fdt2 = StartDate == "" ? DateTime.Now : Convert.ToDateTime(StartDate);
            DateTime tdt2 = EndDate == "" ? DateTime.Now : Convert.ToDateTime(EndDate);
            cond1 = "and updateddate >= '" + fdt2.ToString("yyyy-MM-dd 00:00:00.000") + "' and updateddate <= '" + tdt2.ToString("yyyy-MM-dd 00:00:00.000") + "'  ";

            string lResults = "select (select empid from employees where id = " + lEmpId + ") as EmpId," +
               " (select distinct shortname from employees " +
                " where empid =" + EmpIds + " ) as EmpName ,(select distinct s.code from Designations s, " +
                " employees e where s.id = e.CurrentDesignation " +
                " and e.empid = " + EmpIds + ") as Designation,convert(varchar, UpdatedDate, 103) as AppliedDate," +
                "(select case when b.id = 43 " +
                " then d.name else b.name end from branches b,departments d,   " +
                " employees e where b.id = e.branch and d.id = e.department  " +
                " and e.empid = " + EmpIds + ")as BrDept,convert(varchar, startdate, 103) as StartDate," +
                " convert(varchar, EndDate, 103) as EndDate," +
                " (SELECT code from LeaveTypes where id = Leavetype) as Leavetype,LeaveDays," +
                " Subject,Reason,status from Leaves where empid = " + lEmpId + " " +
                " and status not in ('Credited', 'Debited') and StartDate >= '" + fdt2.ToString("yyyy-MM-dd 00:00:00.000") + "' and  EndDate <= '" + tdt2.ToString("yyyy-MM-dd 00:00:00.000") + "' " +
                "  union select (select empid from employees " +
                " where id = " + lEmpId + ") as EmpId,EmpName," +
                " (select distinct s.code from Designations s, employees e where s.id = e.CurrentDesignation" +
                " and e.empid = " + EmpIds + ") as Designation," +
                " convert(varchar, UpdatedDate, 103) as AppliedDate,(select case when b.id = 43 then d.name " +
                " else b.name end from branches b,departments d, " +
                " employees e where b.id = e.branch and d.id = e.department  and e.empid = " + EmpIds + ") as BrDept," +
                " convert(varchar, updateddate, 103) as updateddate," +
                " convert(varchar, updateddate, 103) as updateddate, (SELECT code from LeaveTypes" +
                " where id = Leavetypeid) " +
                " as Leavetypeid,(case when CreditLeave=0 then DebitLeave else CreditLeave end) as LeaveDays,comments,comments, type as status from Leaves_CreditDebit" +
                " where empid = " + lEmpId + " " + cond1;

            return sh.Get_Table_FromQry(lResults);
        }

        public IList<covidmodel> covid19DataList()
        {
            string CovidQry = "select 'Total' as Total, 'Employees Count' as Employees, 'Family Members' as 'Family Members','Male' as Male," +
                " 'Female' as Female, 'Children' as Children, 'Aged' as Aged, 'Diabetes' as Diabetes,'Hypertension' as Hypertension,'Quarantine' as Quarantine,'Complaints' as Complaints , 'Pending Employees' as 'Pending Employees' " +
                "union all select(select str(count(1))  from[KovidSurvey]),(select str(count(empid)) from[KovidSurvey] where relationship='Self')," +
                "str((select(count(empid)) from[KovidSurvey] where relationship !='Self')),(select str(count(empid)) from[KovidSurvey] where gender = 'Male')," +
                "(select str(count(empid)) from[KovidSurvey] where gender!= 'Male'),(select str(count(empid)) from[KovidSurvey] where age<11)," +
                "(select str(count(empid)) from[KovidSurvey] where age> 59),(select str(count(empid)) from[KovidSurvey] where Diabetes = 'Yes')," +
                "(select str(count(empid)) from[KovidSurvey] where Hypertension = 'Yes'),(select str(count(empid)) from[KovidSurvey] where Quarantine like '%Yes%')," +
                "(select str(count(empid)) from[KovidSurvey] where Complaints like '%Yes%'),str((select count(1) from Employees where RetirementDate> CAST(CAST(GETDATE() AS DATE) AS DATETIME)) - (select(count(distinct empid)) from[KovidSurvey]))";

            IList<covidmodel> lst = new List<covidmodel>();
            covidmodel cv = new covidmodel();
            int rowcnt = 0;
            //string CovidQry = "select * from v_Kovid_19 order by Total desc";
            DataTable Dt = sh.Get_Table_FromQry(CovidQry);
            string Kovidsurvey = "select 'Id' as Id, 'EmpId' as EmpId, 'Sno' as Sno,'Name' as Name,'Gender' as Gender, 'Age' as Age, 'Relationship' as Relationship," +
                " 'Address' as Address,'Complaints' as Complaints,'Diabetes' as Diabetes,'Hypertension' as Hypertension , 'Quarantine' as 'Quarantine' " +
                "union all " +
                "select cast(Id as varchar(500)) as Id, cast(EmpId as varchar(500)) as EmpId, cast(Sno as varchar(1000)) as Sno,Name as Name, " +
                "Gender as Gender, cast(Age as varchar(500)) as Age, Relationship as Relationship,Address as Address,Complaints as Complaints,Diabetes as Diabetes," +
                "Hypertension as Hypertension , Quarantine as 'Quarantine' from[KovidSurvey]";
            DataTable Dt1 = sh.Get_Table_FromQry(Kovidsurvey);
            if (Dt1.Rows.Count > 1)
            {
                //foreach (DataRow dr in Dt.Rows)
                //{
                //    cv = new covidmodel
                //    {
                //        col1 = rowcnt++.ToString(),
                //        col2 = dr["Total"].ToString(),
                //        col3 = dr["Employees"].ToString(),
                //        col4 = dr["Family Members"].ToString(),
                //        col5 = dr["Male"].ToString(),
                //        col6 = dr["Female"].ToString(),
                //        col7 = dr["Children"].ToString(),
                //        col8 = dr["Aged"].ToString(),
                //        col9 = dr["Diabetes"].ToString(),
                //        col10 = dr["Hypertension"].ToString(),
                //        col11 = dr["Quarantine"].ToString(),
                //        col12 = dr["Complaints"].ToString(),
                //        col13 = dr["Pending Employees"].ToString()

                //    };
                //    lst.Add(cv);
                //}

                //cv = new covidmodel
                //{
                //    col1 = rowcnt++.ToString(),
                //    col2 = "Employee Family Details:",
                //    col3 = "",
                //    col4 = "",
                //    col5 = "",
                //    col6 = "",
                //    col7 = "",
                //    col8 = "",
                //    col9 = "",
                //    col10 = "",
                //    col11 = "",
                //    col12 = "",
                //    col13 = ""

                //};
                //lst.Add(cv);

                foreach (DataRow dr1 in Dt1.Rows)
                {
                    cv = new covidmodel
                    {
                        col1 = rowcnt++.ToString(),
                        col2 = dr1["Id"].ToString(),
                        col3 = dr1["EmpId"].ToString(),
                        col4 = dr1["Sno"].ToString(),
                        col5 = dr1["Name"].ToString(),
                        col6 = dr1["Gender"].ToString(),
                        col7 = dr1["Age"].ToString(),
                        col8 = dr1["Relationship"].ToString(),
                        col9 = dr1["Address"].ToString(),
                        col10 = dr1["Diabetes"].ToString(),
                        col11 = dr1["Hypertension"].ToString(),
                        col12 = dr1["Quarantine"].ToString(),
                        col13 = dr1["Complaints"].ToString(),

                    };
                    lst.Add(cv);
                }
            }
            return lst;
        }

        public DataTable covid19DataList(string ddl)
        {
            string CovidQry = "";
            if (ddl != null && ddl == "Submitted")
            {
                CovidQry = "select null as EmpID,'Count of Employee : '+ cast(Count(Distinct K.EmpId) as varchar(500)) as Name,Null as Designation,Null as Branch from KovidSurvey K " +
     "Join Employees E on K.EmpId = E.EmpId " +
     "join Designations D on E.CurrentDesignation = D.Id join Branches B on B.Id = E.Branch " +
     "union all " +
     "select distinct K.EmpID,K.Name,D.Name as Designation,case when b.Name='OtherBranch' then dept.Name else b.Name end  as Branch from KovidSurvey K " +
     "Join Employees E on K.EmpId = E.EmpId join Designations D on E.CurrentDesignation = D.Id " +
     "join Branches B on B.Id = E.Branch join Departments dept on  E.Department= dept.id " +
     " ";
            }
            else if (ddl != null && ddl == "Not Submitted")
            {
                CovidQry = "select null as EmpID,'Count of Employee : '+ cast(Count(E.EmpId) as varchar(500)) as Name,Null as Designation,Null as Branch from Employees E " +
     "join Designations D on E.CurrentDesignation = D.Id join Branches B on B.Id = E.Branch where E.EmpId not in(select distinct EmpId from KovidSurvey where Relationship='Self') and RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME)" +
     "union all " +
     "select E.EmpID,ShortName as Name,D.Name as Designation," +
     "case when b.Name='OtherBranch' then dept.Name else b.Name end  as Branch from Employees E " +
     "join Designations D on E.CurrentDesignation = D.Id join Departments dept on  E.Department= dept.id " +
     "join Branches B on B.Id = E.Branch where E.EmpId not in(select distinct EmpId from KovidSurvey where Relationship='Self') and RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) ";
            }

            else
            {
                CovidQry = "select K.EmpID,K.Name,D.Name as Designation,B.Name as Branch from KovidSurvey K " +
     "Join Employees E on K.EmpId = E.EmpId join Designations D on E.CurrentDesignation = D.Id " +
     "join Branches B on B.Id = E.Branch where K.Relationship = 'NotSelf'";
            }

            return sh.Get_Table_FromQry(CovidQry);
        }
        public class covidmodel
        {
            public string col1 { get; set; }
            public string col2 { get; set; }
            public string col3 { get; set; }
            public string col4 { get; set; }
            public string col5 { get; set; }
            public string col6 { get; set; }
            public string col7 { get; set; }
            public string col8 { get; set; }
            public string col9 { get; set; }
            public string col10 { get; set; }
            public string col11 { get; set; }
            public string col12 { get; set; }
            public string col13 { get; set; }
        }

        public DataTable GetAllMemoList1()
        {
            string qry = "  select " +
                            " e.EmpId,e.ShortName as Name," +
                             " lm.IssueDate as IssueDate,lm.DueDate as DueDate,lm.Memodetails as MemoDetails," +
                             " lm.Noofdays as NoOfDays,lm.Clarification as Explanation,lm.Responsedate as RDate," +
                             " lm.IssueBy as IssueBy,lm.Status as Status,lm.MemoType as MemoType" +
                            " from employees e  join latememo lm on e.EmpId = lm.Empid" +
                             " where e.RetirementDate >=convert(Date, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) ";
            return sh.Get_Table_FromQry(qry);
        }
    }
}
