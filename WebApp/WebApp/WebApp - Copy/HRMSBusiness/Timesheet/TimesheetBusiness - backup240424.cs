using Entities;
using HRMSBusiness.Db;
using HRMSBusiness.Reports;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMSBusiness.Timesheet
{
    public class TimesheetBusiness
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ReportBusiness));
        SqlHelper sh = new SqlHelper();
        public DataTable getAllTimesheetMstdata()
        {
            string qry = "SELECT EID, ShiftType, InTime, OutTime, BranchId, GroupName, UpdatedDate, UpdatedBy from Shift_Master";
            return sh.Get_Table_FromQry(qry);
        }
        public DataTable DailyReportTimesheetMstdata()
        {
            string qry = "Select CAST(ROW_NUMBER() over(order by BranchId) AS INT) Sno,BranchName,b.StartTime as starttime,b.EndTime as endtime,Device_Id from Branch_Device bd join Branches b on b.id=bd.branchid order by BranchName";
            return sh.Get_Table_FromQry(qry);
        }
        public DataTable GetAllBranchMasterData()
        {
            string qry = "SELECT BranchId,Device_id,BranchName from Branch_Device";
            return sh.Get_Table_FromQry(qry);
        }

        public string GetShiftids(int empid)
        {
            string qry = "(select max(s.id) as shiftid from shift_master s join Employees e on e.branch=s.BranchId join Designations des on e.CurrentDesignation=des.Id where  e.EmpId=(select EmpId from Employees where Id = " + empid + " " +
             ")and s.shifttype = (case when(select count(*) from designations ds where ds.name  like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )) ";
            DataTable dt = sh.Get_Table_FromQry(qry);
            string shiftid = dt.Rows[0]["shiftid"].ToString();
            return shiftid;
        }
        public DataTable EarlyByTimesheetMstdata(string branch, string fromdate, string todate, string empcode, string self, int EmpIds)

        {
            string qry = "";
            string cond = "";
            string cond1 = "";
            string cond2 = "";
            string cond3 = "";
            string cond4 = "";
            string cond5 = "";
            string cond6 = "";
            if (branch == string.Empty || branch == null)
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";


                string qrySelect = " select convert(varchar, e.EmpId) + ', ' + e.Shortname + ',' + des.Code + ',' + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol" +
               " ,Convert(varchar,GETDATE(),103) as Date, sm.OutTime as BranchEndTime,'hh:mm tt' as EmpCheckOut," +
               " '' as EarlyBy ";

                qry = qrySelect + " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
               " join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id" + cond
               + " order by Date desc";

            }
            else
            {
                if (fromdate != "")
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
                    //todate day
                    string strtoDate = todate;
                    string[] sa1 = strtoDate.Split('/');
                    string st1 = sa1[0];
                    if (st1.StartsWith("0"))
                    {
                        st1 = st1.Substring(1);
                    }

                    string s2t = sa1[1];
                    //fromdate day
                    string strDate = fromdate;
                    string[] sa = strDate.Split('/');
                    string s1 = sa[0];
                    if (s1.StartsWith("0"))
                    {
                        s1 = s1.Substring(1);
                    }

                    string s2 = sa[1];
                    string s3 = sa[2];
                    cond1 = "D" + s1 + ",";
                    cond2 = s2;
                    cond3 = " where t.year=" + s3 + " and t.month=" + s2;
                    cond4 = fromdate;
                    if (empcode == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and emp.EmpId = " + empcode;
                    }
                    if (branch == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')and empid=" + EmpIds + "";
                    }
                    else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate == "" && todate == "" && empcode != "" && self == "False")
                    {
                        cond5 = "and e.EmpId =" + empcode;
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {

                        cond5 = "and e.EmpId =" + empcode;

                    }
                    else if (branch == "HeadOffice-All" && branch != "All" && fromdate == "" && todate == "" && empcode != "")
                    {
                        cond = " and e.Department != 46" + " and e.Branch = 43" + "and e.EmpId =" + empcode;
                    }
                    else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate == "" && todate == "" && empcode == "")
                    {
                        cond = "and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {

                        cond5 = "and e.EmpId =" + empcode;

                    }
                    else if (branch == "All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    //if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    //{
                    //    cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    //}
                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and empid=" + empcode + " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";

                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";

                    }

                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and empid=" + EmpIds + "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and empid=" + EmpIds + "";
                    }
                    else if (branch == "HeadOffice-All" && fromdate != string.Empty)
                    {
                        cond5 = " and e.Department != 46" + " and e.Branch = 43";
                    }



                    DateTime start = Convert.ToDateTime(fromdate).AddDays(-1);
                    DateTime end = Convert.ToDateTime(todate);
                    DateTime chunkEnd;
                    int dayChunkSize = 1;
                    while ((chunkEnd = start.AddDays(dayChunkSize)) <= end)
                    {

                        Tuple.Create(start, chunkEnd);
                        start = chunkEnd;
                        DateTime chunkend1 = Convert.ToDateTime(start);
                        string start1 = chunkend1.ToString("dd/MM/yyyy");
                        string[] starta = start1.Split('-');
                        string st1s = starta[0];
                        if (st1s.StartsWith("0"))
                        {
                            st1s = st1s.Substring(1);
                        }
                        cond6 = "D" + st1s;

                        qry += "select Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                          " convert(varchar, e.EmpId) + ', ' + e.Shortname + ',' + des.Code + ',' + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol" +
                                           " ,sm.OutTime as BranchEndTime,FORMAT(CAST(SUBSTRING(" + "D" + st1s + "," + "7, 5) AS DATETIME),'hh:mm') as EmpCheckOut," +
                                            " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then (SELECT  CONVERT(varchar(5),DATEADD(minute,DATEDIFF(minute, '', FORMAT(CAST(SUBSTRING(" + "D" + st1s + "," + "31, 5) AS DATETIME), 'hh:mm tt')), '00:00'), 114)) else '00:00' end as EarlyBy" +

                                            " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                            " join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!=''" + "and " + cond6 + "_Status like '%ED%' Union ";

                    }
                    qry = qry.Substring(0, qry.Length - 6) + "order by Date desc";

                }

            }



            return sh.Get_Table_FromQry(qry);
        }
        public DataTable EmpTimesheetMstdata(string branch, string fromdate, string todate, string empcode, string self, int EmpIds)

        {
            string qry = "";
            string cond = "";
            string cond1 = "";
            string cond2 = "";
            string cond3 = "";
            string cond4 = "";
            string cond5 = "";
            string cond6 = "";
            string qrymanual = "";
            string leavetypeName = "";
            string EmpcheckintymName = "";

            if (branch.Contains("~and"))
            {
                branch = branch.Replace("~and", "&");
            }
            DateTime fromdate1 = Convert.ToDateTime("1900-01-01");
            if (fromdate != "")
            {
                fromdate1 = Convert.ToDateTime(fromdate);
            }
            if (branch == string.Empty || branch == null)
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";


                qry = " select convert(varchar, e.EmpId) + ', ' + e.Shortname + ',' + des.Code + ',' + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol" +
              " ,Convert(varchar,GETDATE(),103) as Date,'hh:mm tt' as EmpCheckInTime,'' as LateBy," +
              " b.EndTime as BranchCheckOutTime,'hh:mm tt' as EmpCheckOutTime,'' as EarlyBy  " +
              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
              " join Departments d on e.Department = d.id" + cond
               + " order by Date desc";
            }
            else
            {
                if (fromdate != "")
                {
                    if (branch.StartsWith("-") && branch != "HeadOffice-All")
                    {
                        branch = branch.Substring(1);
                    }

                    branch = branch.Trim();

                    //if (branch.Contains("and"))
                    //{
                    //    branch = branch.Replace("and", "&");
                    //}
                    //todate day
                    string strtoDate = todate;
                    string[] sa1 = strtoDate.Split('/');
                    string st1 = sa1[0];
                    if (st1.StartsWith("0"))
                    {
                        st1 = st1.Substring(1);
                    }

                    string s2t = sa1[1];
                    //fromdate day
                    string strDate = fromdate;
                    string[] sa = strDate.Split('/');
                    string s1 = sa[0];
                    if (s1.StartsWith("0"))
                    {
                        s1 = s1.Substring(1);
                    }

                    string s2 = sa[1];
                    string s3 = sa[2];
                    cond1 = "D" + s1 + ",";
                    cond2 = s2;
                    cond3 = " where t.year=" + s3 + " and t.month=" + s2;
                    cond4 = fromdate;
                    if (empcode == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and emp.EmpId = " + empcode;
                    }
                    if (branch == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')and e.empid=" + EmpIds + "";
                    }
                    else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate == "" && todate == "" && empcode != "" && self == "False")
                    {
                        cond5 = "and e.EmpId =" + empcode;
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {

                        cond5 = "and e.EmpId =" + empcode;

                    }
                    else if (branch == "HeadOffice-All" && branch != "All" && fromdate == "" && todate == "" && empcode != "")
                    {
                        cond = " and e.Department != 46" + " and e.Branch = 43" + "and e.EmpId =" + empcode;
                    }
                    else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate == "" && todate == "" && empcode == "")
                    {
                        cond = "and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {

                        cond5 = "and e.EmpId =" + empcode;

                    }
                    else if (branch == "All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    //if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    //{
                    //    cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    //}
                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and e.empid=" + empcode + " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";

                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";

                    }

                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and e.empid=" + EmpIds + "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and e.empid=" + EmpIds + "";
                    }
                    else if (branch == "HeadOffice-All" && fromdate != string.Empty)
                    {
                        cond5 = " and e.Department != 46" + " and e.Branch = 43";
                    }


                    //between fromdate and todate
                    List<DateTime> allDates = new List<DateTime>();
                    // string fromdate= DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
                    for (DateTime date = Convert.ToDateTime(fromdate); date <= Convert.ToDateTime(todate); date = date.AddDays(1))
                    {
                        allDates.Add(date);
                    }
                    DateTime[] dates = allDates.ToArray();
                    string test = string.Join(",", dates);

                    DateTime start = Convert.ToDateTime(fromdate).AddDays(-1);
                    DateTime end = Convert.ToDateTime(todate);
                    DateTime chunkEnd;
                    int dayChunkSize = 1;
                    while ((chunkEnd = start.AddDays(dayChunkSize)) <= end)
                    {

                        Tuple.Create(start, chunkEnd);
                        start = chunkEnd;
                        DateTime chunkend1 = Convert.ToDateTime(start);
                        string start1 = chunkend1.ToString("dd/MM/yyyy");
                        string start2 = chunkend1.ToString("yyyy/MM/dd");

                        string[] starta = start1.Split('-');
                        string st1s = starta[0];
                        if (st1s.StartsWith("0"))
                        {
                            st1s = st1s.Substring(1);
                        }
                        cond6 = "D" + st1s;
                        string leavetypequery = "select lt.code from leaves l join leavetypes lt on l.leavetype = lt.id join employees e on e.id = l.empid join branches b on e.Branch = b.id join Departments d on e.Department = d.id  where leavesyear = " + s3 + "and startdate = '" + start + "'  and  l.Status in ('Approved','Pending','Forwarded') " + cond5;
                        var dt = sh.Get_Table_FromQry(leavetypequery);
                        leavetypeName = dt.AsEnumerable().FirstOrDefault()?[0].ToString();
                        string Empcheckintym = "select distinct case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) Not In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF','OD-OfWk')  then SUBSTRING(" + "D" + st1s + "," + "19, 2)" +
                                                "end as EmpCheckInTime from timesheet_emp_month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch" +
                                                " where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId != 1 and e.RetirementDate >= convert(Date, '" + start + "')" + cond5 + " and sm.shifttype = (case when(select count(*)" +
                                                    "from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto ";

                        var dt1 = sh.Get_Table_FromQry(Empcheckintym);
                        EmpcheckintymName = dt1.AsEnumerable().FirstOrDefault()[0].ToString();
                        string Empcheckouttym = "select case  when SUBSTRING(" + "D" + st1s + "," + "28, 2) not In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF','OD-OfWk')   then SUBSTRING(D6,28, 2) " +
                                                 "end as EmpCheckOutTime from timesheet_emp_month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch" +
                                                " where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId != 1 and e.RetirementDate >= convert(Date, '" + start + "')" + cond5 + " and sm.shifttype = (case when(select count(*)" +
                                                    "from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto ";

                        if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA"))
                        {
                            qry += "select " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date, convert(varchar, e.EmpId) + ', ' + e.Shortname + ',' + des.Code + ',' + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol," +
                                    "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy " +
                                    "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                    "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select e.id from employees e  where e.empid=" + empcode + ") and " +
                                    "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                     " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                        }
                        else
                        {


                            qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                      " convert(varchar, e.EmpId) + ', ' + e.Shortname + ',' + des.Code + ',' + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol" +
                                       "  ,sm.InTime as BranchStartTime," +
           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " when " + cond6 + " like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end )  end as EmpCheckInTime," +
           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
           " sm.OutTime as BranchCheckOutTime," +
           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " when " + cond6 + " like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end ) end as EmpCheckOutTime," +
           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy" +
                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id join od_otherduty od on od.empid = e.id " +
                                        " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!='' " + " " +
                                        " and sm.shifttype =(case when(select count(*)from designations " +
                                        " ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "' between sm.validfrom and sm.validto Union ";

                        }
                    }
                    if (branch == string.Empty || branch == null)
                    {
                        qrymanual = " Select '00:00#00:00#00:00#EA#00:00#PR#00:00' as D1,Convert(varchar(10),cast(trf.reqfromdate as DATETIME),103) as Date," +
                       "convert(varchar, e.EmpId)+ ', ' + e.Shortname + ',' + des.Code + ',' + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol ,sm.InTime as BranchStartTime,trf.entrytime as EmpCheckInTime,'00.00'as LateBy,sm.OutTime as BranchCheckOutTime," +
                       " trf.exittime as EmpCheckOutTime,'00.00' as EarlyBy from timesheet_request_form trf join employees e " +
                       " on trf.userid=e.id join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id  join Departments d on " +
                       " e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where trf.Reason_type in('Other','Machine Problem','Internet Issue','Power Problem','All') " +
                       " and trf.entrytime is not null and trf.exittime is not null and trf.userid='" + empcode + "' ";
                    }
                    else
                    {
                        qrymanual = " Select '00:00#00:00#00:00#EA#00:00#PR#00:00' as D1,Convert(varchar(10),cast(trf.reqfromdate as DATETIME),103) as Date," +
                       " convert(varchar, e.EmpId)+ ', ' + e.Shortname + ',' + des.Code + ',' + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol ,sm.InTime as BranchStartTime,trf.entrytime as EmpCheckInTime,'00.00'as LateBy,sm.OutTime as BranchCheckOutTime," +
                       " trf.exittime as EmpCheckOutTime,'00.00' as EarlyBy from timesheet_request_form trf join employees e " +
                       " on trf.userid=e.id join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id  join Departments d on " +
                       " e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where trf.Reason_type in('Other','Machine Problem','Internet Issue','Power Problem','All') " +
                       " and trf.entrytime is not null and trf.exittime is not null ";
                    }
                    if (self == "True")
                    {
                        qrymanual += " and trf.reqfromdate between cast('" + fromdate1 + "' as DATETIME) and cast('" + end + "' as DATETIME) and " +
                            "(b.name='" + branch + "' or d.name='" + branch + "') and e.Empid='" + empcode + "'";
                    }
                    else
                    {
                        if (branch == "All" && empcode == "")
                        {
                            qrymanual += " and trf.reqfromdate between cast('" + fromdate1 + "' as DATETIME) and cast('" + end + "' as DATETIME) ";
                        }
                        else if (branch == "All" && empcode != "")
                        {
                            qrymanual += " and trf.reqfromdate between cast('" + fromdate1 + "' as DATETIME) and cast('" + end + "' as DATETIME) and " +
                            " e.Empid=" + empcode + " ";
                        }
                        else if (branch != "All" && empcode == "")
                        {
                            qrymanual += " and trf.reqfromdate between cast('" + fromdate1 + "' as DATETIME) and cast('" + end + "' as DATETIME) " +
                                " and (b.name='" + branch + "' or d.name='" + branch + "') ";
                        }
                        else if (branch != "All" && empcode != "")
                        {
                            qrymanual += " and trf.reqfromdate between cast('" + fromdate1 + "' as DATETIME) and cast('" + end + "' as DATETIME) " +
                            " and (b.name='" + branch + "' or d.name='" + branch + "') and e.Empid=" + empcode + " ";
                        }
                    }
                    qry = qry + qrymanual + "order by Date desc";

                }

            }



            return sh.Get_Table_FromQry(qry);
        }

        public DataTable LateByTimesheetMstdata(string branch, string fromdate, string todate, string empcode, string self, int EmpIds)

        {
            string qry = "";
            string cond = "";
            string cond1 = "";
            string cond2 = "";
            string cond3 = "";
            string cond4 = "";
            string cond5 = "";
            string cond6 = "";
            if (branch == string.Empty || branch == null)
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";


                string qrySelect = " select convert(varchar, e.EmpId) + ', ' + e.Shortname + ',' + des.Code + ',' + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol" +
               " ,Convert(varchar,GETDATE(),103) as Date, sm.InTime as BranchStartTime,'hh:mm tt' as EmpCheckInTime," +
               " '' as LateBy," +
               " '00:10' as LateafterGracePeriod";

                qry = qrySelect + " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
               " join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id" + cond
               + " order by Date desc";
            }
            else
            {
                if (fromdate != "")
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
                    //todate day
                    string strtoDate = todate;
                    string[] sa1 = strtoDate.Split('/');
                    string st1 = sa1[0];
                    if (st1.StartsWith("0"))
                    {
                        st1 = st1.Substring(1);
                    }

                    string s2t = sa1[1];
                    //fromdate day
                    string strDate = fromdate;
                    string[] sa = strDate.Split('/');
                    string s1 = sa[0];
                    if (s1.StartsWith("0"))
                    {
                        s1 = s1.Substring(1);
                    }

                    string s2 = sa[1];
                    string s3 = sa[2];
                    cond1 = "D" + s1 + ",";
                    cond2 = s2;
                    cond3 = " where t.year=" + s3 + " and t.month=" + s2;
                    cond4 = fromdate;
                    if (empcode == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and emp.EmpId = " + empcode;
                    }
                    if (branch == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')and empid=" + EmpIds + "";
                    }
                    else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate == "" && todate == "" && empcode != "" && self == "False")
                    {
                        cond5 = "and e.EmpId =" + empcode;
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {

                        cond5 = "and e.EmpId =" + empcode;

                    }
                    else if (branch == "HeadOffice-All" && branch != "All" && fromdate == "" && todate == "" && empcode != "")
                    {
                        cond = " and e.Department != 46" + " and e.Branch = 43" + "and e.EmpId =" + empcode;
                    }
                    else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate == "" && todate == "" && empcode == "")
                    {
                        cond = "and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {

                        cond5 = "and e.EmpId =" + empcode;

                    }
                    else if (branch == "All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    //if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    //{
                    //    cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    //}
                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and empid=" + empcode + " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";

                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";

                    }

                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and empid=" + EmpIds + "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and empid=" + EmpIds + "";
                    }
                    else if (branch == "HeadOffice-All" && fromdate != string.Empty)
                    {
                        cond5 = " and e.Department != 46" + " and e.Branch = 43";
                    }


                    //between fromdate and todate
                    List<DateTime> allDates = new List<DateTime>();
                    // string fromdate= DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
                    for (DateTime date = Convert.ToDateTime(fromdate); date <= Convert.ToDateTime(todate); date = date.AddDays(1))
                    {
                        allDates.Add(date);
                    }
                    DateTime[] dates = allDates.ToArray();
                    string test = string.Join(",", dates);

                    DateTime start = Convert.ToDateTime(fromdate).AddDays(-1);
                    DateTime end = Convert.ToDateTime(todate);
                    DateTime chunkEnd;
                    int dayChunkSize = 1;
                    while ((chunkEnd = start.AddDays(dayChunkSize)) <= end)
                    {

                        Tuple.Create(start, chunkEnd);
                        start = chunkEnd;
                        DateTime chunkend1 = Convert.ToDateTime(start);
                        string start1 = chunkend1.ToString("dd/MM/yyyy");
                        string[] starta = start1.Split('-');
                        string st1s = starta[0];
                        if (st1s.StartsWith("0"))
                        {
                            st1s = st1s.Substring(1);
                        }
                        cond6 = "D" + st1s;

                        qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                          " convert(varchar, e.EmpId) + ', ' + e.Shortname + ',' + des.Code + ',' + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol" +
                                           " ,sm.InTime as BranchStartTime,FORMAT(CAST(SUBSTRING(" + "D" + st1s + "," + "1, 5) AS DATETIME),'hh:mm tt') as EmpCheckInTime," +
                                            "case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then (SELECT  CONVERT(varchar(5),DATEADD(minute,DATEDIFF(minute, '', FORMAT(CAST(SUBSTRING(" + "D" + st1s + "," + "22, 5) AS DATETIME), 'hh:mm tt')), '00:00'), 114)) else '00:00' end as LateBy," +
                                            " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  (select convert(nvarchar(5),CAST(DATEADD(MINUTE,-10,SUBSTRING(" + "D" + st1s + "," + "22, 5)) as time)))   else '00:00' end as LateafterGracePeriod" +
                                            " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                            " join Departments d on e.Department = d.id join shift_master sm on sm.BranchId = e.Branch where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.empid!= '1' and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!=''" + "and " + cond6 + "_Status like '%LA%' and  sm.shifttype =(case when(select count(*) from designations  ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "' between sm.validfrom and sm.validto  Union ";

                    }
                    qry = qry.Substring(0, qry.Length - 6) + "order by Date desc";

                }

            }



            return sh.Get_Table_FromQry(qry);
        }
        public DataTable LateArrivalsTimesheetMstdata(string branch, string fromdate, string todate, string empcode, string Type, string self, int EmpIds)

        {
            string qry = "";
            string cond = "";
            string cond1 = "";
            string cond2 = "";
            string cond3 = "";
            string cond4 = "";
            string cond5 = "";
            string cond6 = "";
            if (branch == string.Empty || branch == null)
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";


                string qrySelect = " select convert(varchar, e.EmpId)" +
                    " ,e.ShortName as EmpName,e.EmpId,des.code as Designation,case when b.name='OtherBranch' then d.name when b.name='HeadOffice' then d.name else b.name end  as BrDept" +
               " ,Convert(varchar,GETDATE(),103) as Date, sm.InTime as BranchStartTime,'hh:mm tt' as EmpCheckInTime," +
               " '' as LateBy," +
               " '00:10' as LateafterGracePeriod";

                qry = qrySelect + " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
               " join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id" + cond
               + " order by Date desc";
            }
            else
            {
                if (fromdate != "")
                {
                    if (branch.Contains("-") && branch != "HeadOffice-All")
                    {
                        branch = branch.Substring(1);
                        if (branch == "O Bkg-Br")
                        {
                            branch = "HO Bkg-Br";
                        }

                    }

                    branch = branch.Trim();

                    if (branch.Contains("and"))
                    {
                        branch = branch.Replace("and", "&");
                    }
                    //todate day
                    string strtoDate = todate;
                    string[] sa1 = strtoDate.Split('/');
                    string st1 = sa1[0];
                    if (st1.StartsWith("0"))
                    {
                        st1 = st1.Substring(1);
                    }

                    string s2t = sa1[1];
                    //fromdate day
                    string strDate = fromdate;
                    string[] sa = strDate.Split('/');
                    string s1 = sa[0];
                    if (s1.StartsWith("0"))
                    {
                        s1 = s1.Substring(1);
                    }

                    string s2 = sa[1];
                    string s3 = sa[2];
                    cond1 = "D" + s1 + ",";
                    cond2 = s2;
                    cond3 = " where t.year=" + s3 + " and t.month=" + s2;
                    cond4 = fromdate;
                    if (empcode == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and emp.EmpId = " + empcode;
                    }
                    if (branch == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')and empid=" + EmpIds + "";
                    }
                    else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate == "" && todate == "" && empcode != "" && self == "False")
                    {
                        cond5 = "and e.EmpId =" + empcode;
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {

                        cond5 = "and e.EmpId =" + empcode;

                    }
                    else if (branch == "HeadOffice-All" && branch != "All" && fromdate == "" && todate == "" && empcode != "")
                    {
                        cond = " and e.Department != 46" + " and e.Branch = 43" + "and e.EmpId =" + empcode;
                    }
                    else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate == "" && todate == "" && empcode == "")
                    {
                        cond = "and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {

                        cond5 = "and e.EmpId =" + empcode;

                    }
                    else if (branch == "All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    //if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    //{
                    //    cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    //}
                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and empid=" + empcode + " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";

                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";

                    }

                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and empid=" + EmpIds + "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and empid=" + EmpIds + "";
                    }
                    else if (branch == "HeadOffice-All" && fromdate != string.Empty)
                    {
                        cond5 = " and e.Department != 46" + " and e.Branch = 43";
                    }


                    //between fromdate and todate
                    List<DateTime> allDates = new List<DateTime>();
                    // string fromdate= DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
                    for (DateTime date = Convert.ToDateTime(fromdate); date <= Convert.ToDateTime(todate); date = date.AddDays(1))
                    {
                        allDates.Add(date);
                    }
                    DateTime[] dates = allDates.ToArray();
                    string test = string.Join(",", dates);

                    DateTime start = Convert.ToDateTime(fromdate).AddDays(-1);
                    DateTime end = Convert.ToDateTime(todate);
                    DateTime chunkEnd;
                    int dayChunkSize = 1;
                    while ((chunkEnd = start.AddDays(dayChunkSize)) <= end)
                    {

                        Tuple.Create(start, chunkEnd);
                        start = chunkEnd;
                        DateTime chunkend1 = Convert.ToDateTime(start);
                        string start1 = chunkend1.ToString("dd/MM/yyyy");
                        string[] starta = start1.Split('-');
                        string st1s = starta[0];
                        if (st1s.StartsWith("0"))
                        {
                            st1s = st1s.Substring(1);
                        }
                        cond6 = "D" + st1s;

                        qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                           " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when b.name = 'OtherBranch' then d.name when b.name = 'HeadOffice' then d.name else b.name end as BrDept" +
                                           " ,sm.InTime as BranchStartTime,FORMAT(CAST(SUBSTRING(" + "D" + st1s + "," + "1, 5) AS DATETIME),'hh:mm tt') as EmpCheckInTime," +
                                            "case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then (SELECT  CONVERT(varchar(5),DATEADD(minute,DATEDIFF(minute, '', FORMAT(CAST(SUBSTRING(" + "D" + st1s + "," + "22, 5) AS DATETIME), 'hh:mm tt')), '00:00'), 114)) else '00:00' end as LateBy," +
                                            " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  (select convert(nvarchar(5),CAST(DATEADD(MINUTE,-10,SUBSTRING(" + "D" + st1s + "," + "22, 5)) as time)))   else '00:00' end as LateafterGracePeriod" +
                                            " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                            " join Departments d on e.Department = d.id join shift_master sm on sm.BranchId = e.Branch where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.empid!= '1' and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!=''" + "and " + cond6 + "_Status like '%LA%' and sm.shifttype=" +
                                            " (case when (select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "' between sm.validfrom and sm.validto Union ";


                    }
                    qry = qry.Substring(0, qry.Length - 6) + "order by Date desc";

                }

            }



            return sh.Get_Table_FromQry(qry);
        }
        public DataTable EarlyDepartsTimesheetMstdata(string branch, string fromdate, string todate, string empcode, string self, int EmpIds)

        {
            string qry = "";
            string cond = "";
            string cond1 = "";
            string cond2 = "";
            string cond3 = "";
            string cond4 = "";
            string cond5 = "";
            string cond6 = "";
            if (branch == string.Empty || branch == null)
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";


                string qrySelect = " select e.ShortName as EmpName,e.EmpId,des.code as Designation,case when b.name = 'OtherBranch' then d.name when b.name = 'HeadOffice' then d.name else b.name end as BrDept" +
               " ,Convert(varchar,GETDATE(),103) as Date, sm.OutTime as BranchEndTime,'hh:mm tt' as EmpCheckOut," +
               " '' as EarlyBy ";

                qry = qrySelect + " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
               " join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id" + cond
               + " order by Date desc";

            }
            else
            {
                if (fromdate != "")
                {
                    if (branch.Contains("-") && branch != "HeadOffice-All")
                    {
                        branch = branch.Substring(1);
                        if (branch == "O Bkg-Br")
                        {
                            branch = "HO Bkg-Br";
                        }
                    }

                    branch = branch.Trim();

                    if (branch.Contains("and"))
                    {
                        branch = branch.Replace("and", "&");
                    }
                    //todate day
                    string strtoDate = todate;
                    string[] sa1 = strtoDate.Split('/');
                    string st1 = sa1[0];
                    if (st1.StartsWith("0"))
                    {
                        st1 = st1.Substring(1);
                    }

                    string s2t = sa1[1];
                    //fromdate day
                    string strDate = fromdate;
                    string[] sa = strDate.Split('/');
                    string s1 = sa[0];
                    if (s1.StartsWith("0"))
                    {
                        s1 = s1.Substring(1);
                    }

                    string s2 = sa[1];
                    string s3 = sa[2];
                    cond1 = "D" + s1 + ",";
                    cond2 = s2;
                    cond3 = " where t.year=" + s3 + " and t.month=" + s2;
                    cond4 = fromdate;
                    if (empcode == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and emp.EmpId = " + empcode;
                    }
                    if (branch == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')and empid=" + EmpIds + "";
                    }
                    else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate == "" && todate == "" && empcode != "" && self == "False")
                    {
                        cond5 = "and e.EmpId =" + empcode;
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {

                        cond5 = "and e.EmpId =" + empcode;

                    }
                    else if (branch == "HeadOffice-All" && branch != "All" && fromdate == "" && todate == "" && empcode != "")
                    {
                        cond = " and e.Department != 46" + " and e.Branch = 43" + "and e.EmpId =" + empcode;
                    }
                    else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate == "" && todate == "" && empcode == "")
                    {
                        cond = "and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {

                        cond5 = "and e.EmpId =" + empcode;

                    }
                    else if (branch == "All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    //if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    //{
                    //    cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    //}
                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and empid=" + empcode + " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";

                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";

                    }

                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and empid=" + EmpIds + "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and empid=" + EmpIds + "";
                    }
                    else if (branch == "HeadOffice-All" && fromdate != string.Empty)
                    {
                        cond5 = " and e.Department != 46" + " and e.Branch = 43";
                    }



                    DateTime start = Convert.ToDateTime(fromdate).AddDays(-1);
                    DateTime end = Convert.ToDateTime(todate);
                    DateTime chunkEnd;
                    int dayChunkSize = 1;
                    while ((chunkEnd = start.AddDays(dayChunkSize)) <= end)
                    {

                        Tuple.Create(start, chunkEnd);
                        start = chunkEnd;
                        DateTime chunkend1 = Convert.ToDateTime(start);
                        string start1 = chunkend1.ToString("dd/MM/yyyy");
                        string[] starta = start1.Split('-');
                        string st1s = starta[0];
                        if (st1s.StartsWith("0"))
                        {
                            st1s = st1s.Substring(1);
                        }
                        cond6 = "D" + st1s;

                        qry += "select Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                         "  e.ShortName as EmpName,e.EmpId,des.code as Designation,case when b.name = 'OtherBranch' then d.name when b.name = 'HeadOffice' then d.name else b.name end as BrDept" +
                                           " ,sm.OutTime as BranchEndTime,case when SUBSTRING(D" + st1s + ",28, 2) = 'NA' then SUBSTRING(D" + st1s + ",28, 2) + ''  when D" + st1s + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF') or D" + st1s + " like '%OD%' then D" + st1s + " else SUBSTRING(D" + st1s + ",7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end ) end as EmpCheckOut," +
                                            " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then (SELECT  CONVERT(varchar(5),DATEADD(minute,DATEDIFF(minute, '', FORMAT(CAST(SUBSTRING(" + "D" + st1s + "," + "31, 5) AS DATETIME), 'hh:mm tt')), '00:00'), 114)) else '00:00' end as EarlyBy" +

                                            " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                            " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   " +
                                            " where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!=''" + "and " + cond6 + "_Status like '%ED%' " +
                                            " and  sm.shifttype =(case when(select count(*) from designations " +
                                            " ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                            "  else '0' end ) and '" + start + "' between sm.validfrom and sm.validto   Union ";

                    }
                    qry = qry.Substring(0, qry.Length - 8) + " order by Date desc  ";

                }

            }



            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getallempmontlysearch(string branch, string monthyear)
        {
            //string branchid = "";
            //string date = "";
            //string qry2 = "select distinct t.branch_id as branch_id,t.run_date as run_date,t.active as active,bd.BranchName as BranchName from timesheet_branch_rerun t join branch_device bd on bd.id=t.branch_id  where t.active=1 ";
            //var dt = sh.Get_Table_FromQry(qry2);
            //foreach (DataRow dr in dt.Rows)
            //{
            //    branchid = dr["BranchId"].ToString();
            //    date = dr["run_date"].ToString();
            //}
            string cond = "";

            if (branch == "" && monthyear == "")
            {
                cond = " where (branch_name is null )";
            }

            else if (branch == "-1" && monthyear == "-2")
            {
                cond = " where (branch_name='" + branch + "' )";
            }
            else if (branch != "" && branch != "All" && monthyear != "")
            {
                DateTime str = Convert.ToDateTime(monthyear);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where (branch_name='" + branch + "')" + " and mn=" + s2 + " and yr=" + s1 + "";
            }
            else if (branch == "" && monthyear != "")
            {
                DateTime str = Convert.ToDateTime(monthyear);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where  mn = " + s2 + " and yr = " + s1 + "";
            }
            else if (branch != "" && branch == "All" && monthyear == "" && monthyear == string.Empty)
            {
                cond = "";
            }
            else if (branch != "" && branch == "All" && monthyear != "")
            {
                DateTime str = Convert.ToDateTime(monthyear);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where mn = " + s2 + " and yr = " + s1 + "";
            }
            else if (branch != "" && branch != "All" && monthyear == "" && monthyear == string.Empty)
            {
                cond = " where (branch_name='" + branch + "')";
            }


            else if (branch == "" && branch == "All" && monthyear != "")
            {
                DateTime str = Convert.ToDateTime(monthyear);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where  mn = " + s2 + " and yr = " + s1 + "";
            }

            string qry = " select Concat(CONVERT(CHAR(3), DATENAME(MONTH,(concat(yr,'-',mn,'-',01) ))),'-',yr) as monthyear, device_id as DeviceId,branch_name as BranchName, CAST(D1 As Int) as D1,CAST(D2 As Int) as D2 ,CAST(D3 As Int) as D3,CAST(D4 As Int) as D4,CAST(D5 As Int) as D5,CAST(D6 As Int) as D6,CAST(D7 As Int) as D7,CAST(D8 As Int) as D8,CAST(D9 As Int) as D9,CAST(D10 As Int) as D10,CAST(D11 As Int) as D11,CAST(D12 As Int) as D12,CAST(D13 As Int) as D13,CAST(D14 As Int) as D14,CAST(D15 As Int) as D15,CAST(D16 As Int) as D16,CAST(D17 As Int) as D17,CAST(D18 As Int) as D18,CAST(D19 As Int) as D19,CAST(D20 As Int) as D20,CAST(D21 As Int) as D21,CAST(D22 As Int) as D22,CAST(D23 As Int) as D23,CAST(D24 As Int) as D24,CAST(D25 As Int) as D25,CAST(D26 As Int) as D26,CAST(D27 As Int) as D27,CAST(D28 As Int) as D28,CAST(D29 As Int) as D29,CAST(D30 As Int) as D30,CAST(D31 As Int) as D31 from timesheet_run_status" + cond + " order by branch_name";
            return sh.Get_Table_FromQry(qry);


        }
        //public DataTable EmployeeTimesheetMstdata(string branch, string fromdate, string todate, string empcode, string Type, string self, int EmpIds, string intime, string outtime)

        //{
        //    string typ = intime;
        //    string typ1 = outtime;
        //    string qry = "";
        //    string cond = "";
        //    string cond1 = "";
        //    string cond2 = "";
        //    string cond3 = "";
        //    string cond4 = "";
        //    string cond5 = "";
        //    string cond6 = "";
        //    if (branch == string.Empty || branch == null)
        //    {
        //        cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";


        //        qry = " select e.ShortName as EmpName,e.EmpId,des.code as Designation,case when b.name = 'OtherBranch' then d.name when b.name = 'HeadOffice' then d.name else b.name end as BrDept" +
        //      " ,Convert(varchar,GETDATE(),103) as Date,'hh:mm tt' as EmpCheckInTime,'' as LateBy," +
        //      " b.EndTime as BranchCheckOutTime,'hh:mm tt' as EmpCheckOutTime,'' as EarlyBy  " +
        //      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //      " join Departments d on e.Department = d.id" + cond
        //       + " order by Date desc";
        //    }


        //    else
        //    {
        //        if (fromdate != "")
        //        {
        //            //if (branch.Contains("-") && branch != "HeadOffice-All")
        //            //{
        //            //    branch = branch.Substring(1);
        //            //}

        //            branch = branch.Trim();

        //            if (branch.Contains("and"))
        //            {
        //                branch = branch.Replace("and", "&");
        //            }
        //            //todate day
        //            string strtoDate = todate;
        //            string[] sa1 = strtoDate.Split('/');
        //            string st1 = sa1[0];
        //            if (st1.StartsWith("0"))
        //            {
        //                st1 = st1.Substring(1);
        //            }

        //            string s2t = sa1[1];
        //            string s3t = sa1[2];
        //            //fromdate day
        //            string strDate = fromdate;
        //            string[] sa = strDate.Split('/');
        //            string s1 = sa[0];
        //            if (s1.StartsWith("0"))
        //            {
        //                s1 = s1.Substring(1);
        //            }

        //            string s2 = sa[1];
        //            string s3 = sa[2];
        //            cond1 = "D" + s1 + ",";
        //            cond2 = s2;
        //            cond3 = " where t.year=" + s3 + " and t.month=" + s2;
        //            cond4 = fromdate;
        //            string percent = "%";
        //            if (empcode == string.Empty || fromdate == string.Empty)
        //            {
        //                cond5 = " and emp.EmpId = " + empcode;
        //            }
        //            if (branch == string.Empty || fromdate == string.Empty)
        //            {
        //                //cond5 = " and (f.BranchName='" + branch + "'" + "  or d.name='" + branch + "')";
        //                cond5 = "and f.BranchName like '" + branch + "" + percent + "'";
        //            }
        //            if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
        //            {
        //                cond5 = " and empid=" + EmpIds + "";
        //            }
        //            else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate == "" && todate == "" && empcode != "" && self == "False")
        //            {
        //                cond5 = "and e.EmpId =" + empcode;
        //            }
        //            else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
        //            {

        //                cond5 = "and e.EmpId =" + empcode;

        //            }
        //            else if (branch != "All" && branch != "HeadOffice-All" && empcode == "" && fromdate != string.Empty && self == "False")
        //            {

        //                //cond5 = " and (f.BranchName='" + branch + "'" + "  or d.name='" + branch + "')";
        //                cond5 = "and f.BranchName like '" + branch + "" + percent + "'";
        //            }
        //            else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
        //            {


        //                //cond5 = " and e.empid=" + empcode + " and (f.BranchName='" + branch + "'" + "  or d.name='" + branch + "') ";
        //                cond5 = " and e.empid=" + empcode + " and f.BranchName like '" + branch + "" + percent + "'";
        //            }
        //            else if (branch == "HeadOffice-All" && branch != "All" && fromdate == "" && todate == "" && empcode != "")
        //            {
        //                cond = " and e.Department != 46" + " and e.Branch = 43" + "and e.EmpId =" + empcode;
        //            }
        //            else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate == "" && todate == "" && empcode == "")
        //            {
        //                cond = "and (f.BranchName='" + branch + "'" + "  or d.name='" + branch + "')";
        //            }
        //            else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate != "" && todate != "" && empcode == "" && self == "False")
        //            {
        //                cond = "and (f.BranchName='" + branch + "'" + "  or d.name='" + branch + "')";
        //            }
        //            else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
        //            {
        //                //cond5 = " and empid=" + empcode + " and (f.BranchName='" + branch + "'" + "  or d.name='" + branch + "')";
        //                cond5 = " and empid=" + empcode + " and f.BranchName like '" + branch + "" + percent + "'";
        //            }
        //            else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
        //            {

        //                cond5 = "and e.EmpId =" + empcode;

        //            }
        //            else if (branch == "All" && fromdate != string.Empty && self == "False")
        //            {
        //                cond5 = "";
        //            }
        //            //if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
        //            //{
        //            //    cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
        //            //}
        //            else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
        //            {
        //                cond5 = "";
        //            }
        //            else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
        //            {
        //                cond5 = " and empid=" + empcode + " and f.BranchName like '" + branch + "" + percent + "'";
        //                //cond5 = " and empid=" + empcode + " and (f.BranchName='" + branch + "'" + "  or d.name='" + branch + "')";

        //            }
        //            else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
        //            {
        //                cond5 = "and f.BranchName like '" + branch + "" + percent + "'";
        //            }
        //            else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
        //            {
        //                cond5 = " and f.BranchName like '" + branch + "" + percent + "'";

        //            }

        //            else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
        //            {
        //                cond5 = "and empid=" + EmpIds + "";
        //            }
        //            else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
        //            {
        //                cond5 = "and empid=" + EmpIds + "";
        //            }
        //            else if (branch == "HeadOffice-All" && fromdate != string.Empty && empcode == "")
        //            {
        //                cond5 = " ";
        //            }
        //            else if (branch == "HeadOffice-All" && fromdate != string.Empty && empcode != "")
        //            {
        //                cond5 = "and empid=" + EmpIds + " ";
        //            }
        //            //else if (branch != "" && empcode == "" )
        //            //{
        //            //    cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
        //            //}

        //            //else if (branch != "" && empcode != "")
        //            //{
        //            //    cond5 = " and empid=" + empcode + "  and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
        //            //}
        //            //between fromdate and todate
        //            List<DateTime> allDates = new List<DateTime>();
        //            // string fromdate= DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
        //            for (DateTime date = Convert.ToDateTime(fromdate); date <= Convert.ToDateTime(todate); date = date.AddDays(1))
        //            {
        //                allDates.Add(date);
        //            }
        //            DateTime[] dates = allDates.ToArray();
        //            string test = string.Join(",", dates);

        //            DateTime start = Convert.ToDateTime(fromdate).AddDays(-1);
        //            DateTime end = Convert.ToDateTime(todate);
        //            DateTime chunkEnd;
        //            int dayChunkSize = 1;
        //            while ((chunkEnd = start.AddDays(dayChunkSize)) <= end)
        //            {

        //                Tuple.Create(start, chunkEnd);
        //                start = chunkEnd;
        //                DateTime chunkend1 = Convert.ToDateTime(start);
        //                string start1 = chunkend1.ToString("dd/MM/yyyy");
        //                string[] starta = start1.Split('-');
        //                string st1s = starta[0];
        //                string mst1s = starta[1];
        //                string yst1s = starta[2];
        //                if (st1s.StartsWith("0"))
        //                {
        //                    st1s = st1s.Substring(1);
        //                }
        //                cond6 = "D" + st1s;
        //                string cond7 = "";
        //                string conyy = " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "ML" + "%" + "'" + "and " + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "CL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "PL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "MTL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "PTL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "LOP" + "%" + "'" + " and" +
        //                                                                            " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "C-OFF" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "SCL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "EOL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "OD" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "LA" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "ED" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "AB" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "PR" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "NA" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "MA" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "HD" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "HL" + "%" + "'";
        //                string contype = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + intime + "__" + "%" + "'";
        //                string contype6 = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + "__" + outtime + "%" + "'";
        //                //  string contype = "D" + st1s + " = '" + typ+"' ";
        //                string contypeod = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + outtime + "__" + "%" + "'";
        //                string contype1 = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + "__" + outtime + "%" + "'";
        //                string contype2 = " D" + st1s + " " + "Like" + "" + "'" + "%" + intime + "%" + "'";
        //                string contype4 = " D" + st1s + " " + "Like" + "" + "'" + "%" + outtime + "%" + "'";
        //                string contypelast2 = " D" + st1s + " " + "Like" + "" + "'" + "%" + intime + "%" + "'" + " and" + " D" + st1s + " " + "Like" + "" + "'" + "%" + outtime + "%" + "'";
        //                string contypelast3 = " D" + st1s + " " + "=" + "" + "'" + intime + "'" + " and" + " D" + st1s + " " + "=" + "" + "'" + outtime + "'";

        //                string conlast = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + intime + "__" + "%" + "'" + " and" + " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + "__" + outtime + "%" + "'";
        //                string contype3 = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + outtime + "__" + "%" + "'";
        //                string date = yst1s + "-" + mst1s + "-" + st1s;
        //                string queryunion = " select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                            " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                             "  ,sm.InTime as BranchStartTime," +
        // " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        // " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        // " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        // " sm.OutTime as BranchCheckOutTime," +
        // " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        // " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        // " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id " +
        //                              "join branches b on e.Branch = b.id  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id " +
        //                              " join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE()) and  (convert(date,logs.io_time)!='' ) Union ";
        //                if ((typ != typ1 && typ1 != "" && (typ == "Others" || typ1 == "Others") && branch != "" && fromdate != "") && (typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "C-OFF" || typ == "SCL" || typ == "EOL"))
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                 "  ,sm.InTime as BranchStartTime," +
        //     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //     " sm.OutTime as BranchCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id " +
        //                                  "join branches b on e.Branch = b.id  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id " +
        //                                  " join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + " and e.empid=0" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ == "NA" && typ1 == "" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                    " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                     "  ,sm.InTime as BranchStartTime," +
        //         " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //         " sm.OutTime as BranchCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id   " +
        //                                      "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ1 != "" && typ == "" && typ1 == "NA" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                    " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                     "  ,sm.InTime as BranchStartTime," +
        //         " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //         " sm.OutTime as BranchCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                      "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype6 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ1 == "" && typ == "AB" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                 " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                  "  ,sm.InTime as BranchStartTime," +
        //                                      " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                      " sm.OutTime as BranchCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                   " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                   "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype2 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ == "" && typ1 != "" && typ1 == "AB" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                 " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                  "  ,sm.InTime as BranchStartTime," +
        //                                      " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                      " sm.OutTime as BranchCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                   " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                   "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype4 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ1 != "" && (typ == "AB" && typ1 == "AB") && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                 " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                  "  ,sm.InTime as BranchStartTime," +
        //                                      " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                      " sm.OutTime as BranchCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                   " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                   "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contypelast2 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ1 != "" && (typ == "LA" && typ1 == "NA") && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                 " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                  "  ,sm.InTime as BranchStartTime," +
        //                                      " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                      " sm.OutTime as BranchCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                   " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                   "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + conlast + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ == "" && (typ1 == "CL" || typ1 == "ML" || typ1 == "PTL" || typ1 == "PL" || typ1 == "MTL" || typ1 == "SCL" || typ1 == "LOP" || typ1 == "EOL" || typ1 == "C-OFF") && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                   " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when b.name = 'OtherBranch' then d.name when b.name = 'HeadOffice' then d.name else b.name end as BrDept" +
        //                                    "  ,sm.InTime as BranchStartTime," +
        //        " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //        " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //        " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //        " sm.OutTime as BranchCheckOutTime," +
        //        " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //        " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //        " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                     " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                     "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype4 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ == "" && typ1 != "" && typ1 == "HL" && fromdate != "" && branch != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                   " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                    "  ,sm.InTime as BranchStartTime," +
        //        " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //        " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //        " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //        " sm.OutTime as BranchCheckOutTime," +
        //        " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //        " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //        " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                     " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                     "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype4 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ == "" && typ1 != "" && typ1 == "OD" && typ != "Others" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype3 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ1 != "" && typ != "Others" && typ == "NA" && typ1 == "NA" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contypelast3 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ != "" && typ != "Others" && typ == "PR" && typ1 == "OD" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype1 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if ((typ != typ1 && typ1 != "" && (typ != "Others" || typ1 != "Others") && branch != "" && fromdate != "") && ((typ == "AB" || typ == "HL") && typ1 == "OD"))
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                 "  ,sm.InTime as BranchStartTime," +
        //     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //     " sm.OutTime as BranchCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + " and e.empid=0" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ1 != "" && (typ == "NA" || typ == "PR" || typ == "LA" || typ == "ED") && (typ1 == "NA" || typ1 == "PR" || typ1 == "LA" || typ1 == "ED") && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                 " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                  "  ,sm.InTime as BranchStartTime," +
        //                                      " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                      " sm.OutTime as BranchCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                   " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                   "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + conlast + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ == "" && typ1 != "" && typ1 == "HL" && fromdate != "" && branch != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                    " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                     "  ,sm.InTime as BranchStartTime," +
        //         " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //         " sm.OutTime as BranchCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                      "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype2 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ1 != "" && typ1 == "HL" && typ == "HL" && fromdate != "" && branch != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                    " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                     "  ,sm.InTime as BranchStartTime," +
        //         " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //         " sm.OutTime as BranchCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                      "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype2 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if ((typ != typ1 && typ1 != "" && (typ == "Others" || typ1 == "Others") && branch != "" && fromdate != "") && (typ1 == "CL" || typ1 == "PL" || typ1 == "ML" || typ1 == "MTL" || typ1 == "PTL" || typ1 == "LOP" || typ1 == "C-OFF" || typ1 == "SCL" || typ1 == "EOL"))
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                 "  ,sm.InTime as BranchStartTime," +
        //     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //     " sm.OutTime as BranchCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + " and e.empid=0" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if ((typ != typ1 && typ1 != "" && (typ != "Others" || typ1 != "Others") && branch != "" && fromdate != "") && (typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "C-OFF" || typ == "SCL" || typ == "EOL" || typ == "HL" || typ == "HD" || typ == "NA" || typ == "Others") && (typ1 == "OD" || typ1 == "LA" || typ1 == "NA" || typ1 == "PR" || typ1 == "AB" || typ1 == "ED" || typ1 == "MA" || typ == "NA" || typ1 == "HD" || typ1 == "HL"))
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                 "  ,sm.InTime as BranchStartTime," +
        //     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //     " sm.OutTime as BranchCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + " and e.empid=0" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if ((typ != typ1 && typ1 != "" && (typ != "Others" || typ1 != "Others") && branch != "" && fromdate != "") && (typ1 == "CL" || typ1 == "PL" || typ1 == "ML" || typ1 == "MTL" || typ1 == "PTL" || typ1 == "LOP" || typ1 == "C-OFF" || typ1 == "SCL" || typ1 == "EOL" || typ1 == "HL" || typ1 == "HD" || typ1 == "NA" || typ1 == "Others" || typ1 == "AB" || typ1 == "LA" || typ1 == "ED" || typ1 == "MA") && (typ == "OD" || typ == "LA" || typ == "NA" || typ == "PR" || typ == "AB" || typ == "NA" || typ == "ED" || typ == "MA" || typ == "HD" || typ == "HL"))
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                 "  ,sm.InTime as BranchStartTime," +
        //     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //     " sm.OutTime as BranchCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + " and e.empid=0" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (fromdate != "" && typ != typ1 && (typ == "Others" || typ1 == "Others") && branch != "" && ((typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "C-OFF" || typ == "SCL" || typ == "EOL")))
        //                {

        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                 "  ,sm.InTime as BranchStartTime," +
        //     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //     " sm.OutTime as BranchCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + " and e.empid=0" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }



        //                else if (typ == "Others" && typ1 != "" && typ1 != "Others" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + conyy + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ1 == "Others" && typ != "" && typ != "Others" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + conyy + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if ((typ == "Others" || typ1 == "Others") && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + conyy + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if ((typ == "Others" || typ1 == "Others") && typ != "" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + conyy + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ == "Others" && typ1 == "" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + conyy + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ1 == "Others" && typ == "" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + conyy + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if ((typ == "Others" || typ1 == "Others") && typ1 != "" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + conyy + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }

        //                else if ((typ != typ1 && typ1 != "" && branch != "" && fromdate != "") && (typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "C-OFF" || typ == "SCL" || typ == "EOL"))
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                 "  ,sm.InTime as BranchStartTime," +
        //     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //     " sm.OutTime as BranchCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + " and e.empid=0" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }

        //                else
        //             if (fromdate != "" && (typ != "" && typ != "Others" && (typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "C-OFF" || typ == "SCL" || typ == "EOL" || typ == "HL" || typ == "AB" || typ == "HD" || typ == "OD" || typ == "LA" || typ == "ED")) && typ1 == "Others" && branch != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                    " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                     "  ,sm.InTime as BranchStartTime," +
        //         " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //         " sm.OutTime as BranchCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                      "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contypelast2 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ == "" && fromdate != "" && typ1 != "" && typ1 != "Others" && (typ1 == "AB") && branch != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                    " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                     "  ,sm.InTime as BranchStartTime," +
        //         " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //         " sm.OutTime as BranchCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                      "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contypeod + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else
        //                if (fromdate != "" && typ1 != "" && typ1 != "Others" && (typ1 != "" && typ1 != "Others" && (typ1 == "CL" || typ1 == "PL" || typ1 == "ML" || typ1 == "MTL" || typ1 == "PTL" || typ1 == "LOP" || typ1 == "C-OFF" || typ1 == "SCL" || typ1 == "EOL" || typ1 == "HL" || typ1 == "AB" || typ1 == "HD") && branch != "" && typ1 == "Others"))
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                    " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                     "  ,sm.InTime as BranchStartTime," +
        //         " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //         " sm.OutTime as BranchCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                      "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contypelast2 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if ((typ == "NA" || typ == "PR" || typ == "LA" || typ == "ED") && (typ1 == "NA" || typ1 == "PR" || typ1 == "LA" || typ1 == "ED") && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                 " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                  "  ,sm.InTime as BranchStartTime," +
        //                                      " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                      " sm.OutTime as BranchCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                   " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                   "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + conlast + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (fromdate != "" && (typ == "NA" || typ == "AB" || typ == "HL" || typ == "HD" || typ == "PR" || typ == "LA" || typ == "ED") && (typ1 == "AB" || typ1 == "HD" || typ1 == "HL" || typ1 == "NA" || typ1 == "PR" || typ1 == "LA" || typ1 == "ED") && branch != "")
        //                {


        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                 "  ,sm.InTime as BranchStartTime," +
        //     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //     " sm.OutTime as BranchCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + " and e.empid=0" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (fromdate != "" && typ1 != "" && typ1 != "Others" && (typ1 == "CL" || typ1 == "PL" || typ1 == "ML" || typ1 == "MTL" || typ1 == "PTL" || typ1 == "LOP" || typ1 == "C-OFF" || typ1 == "SCL" || typ1 == "EOL" || typ1 == "HL" || typ1 == "AB" || typ1 == "HD") && branch != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                    " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                     "  ,sm.InTime as BranchStartTime," +
        //         " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //         " sm.OutTime as BranchCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                      "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype2 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }

        //                else if (fromdate != "" && typ != "" && typ != "Others" && (typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "C-OFF" || typ == "SCL" || typ == "EOL" || typ == "HL" || typ == "AB" || typ == "HD") && branch != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                    " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                     "  ,sm.InTime as BranchStartTime," +
        //         " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //         " sm.OutTime as BranchCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                      "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype2 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }

        //                else if (fromdate != "" && typ1 != "" && typ1 != "Others" && (typ1 == "CL" || typ1 == "PL" || typ1 == "ML" || typ1 == "MTL" || typ1 == "PTL" || typ1 == "LOP" || typ1 == "C-OFF" || typ1 == "SCL" || typ1 == "EOL" || typ1 == "HL" || typ1 == "AB" || typ1 == "HD") && branch != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                    " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                     "  ,sm.InTime as BranchStartTime," +
        //         " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //         " sm.OutTime as BranchCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //         " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //         " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                      " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                      "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype2 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ1 != "" && typ != "Others" && typ != typ1 && (typ == "OD" && (typ1 == "LA" || typ1 == "ED")) && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ1 != "" && typ != "Others" && typ != typ1 && (typ1 == "OD" && (typ == "LA" || typ == "ED")) && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype1 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ != "Others" && typ == "AB" && typ1 == "AB" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != "" && typ != "Others" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ == "" && typ != "Others" && branch != "" && fromdate != "")
        //                {
        //                    string fromdate1 = s3 + "-" + s2 + "-" + s1;
        //                    string todate2 = s3t + "-" + s2t + "-" + st1;

        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                     " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                     " ,sm.InTime as BranchStartTime," +
        //                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                     " sm.OutTime as BranchCheckOutTime," +
        //                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                     " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id  " +
        //                     //"join Employee_Transfer et on et.EmpId=e.id join branches b on et.NewBranch = b.id " +
        //                     " join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id " +
        //                     " join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + "   " +
        //                     " and (convert(date,logs.io_time)='" + date + "' )" +
        //                     " and " + contype1 + " and " + cond6 + "!=''" + "   Union ";
        //                }
        //                else if (typ == "NA" && typ == "NA" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ1 != "" && typ1 != "Others" && branch != "" && fromdate != "" && typ1 == "OD")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contypeod + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ1 != "" && typ1 != "Others" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype1 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }

        //                else if (typ1 != "" && typ1 != "" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                      " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                       "  ,sm.InTime as BranchStartTime," +
        //           " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //           " sm.OutTime as BranchCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //           " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //           " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                        " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                        "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + contype2 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ == "" && typ1 == "" && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                 "  ,sm.InTime as BranchStartTime," +
        //                                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                     " sm.OutTime as BranchCheckOutTime," +
        //                                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if ((typ == "Others" || typ1 == "Others") && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                 "  ,sm.InTime as BranchStartTime," +
        //                                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                     " sm.OutTime as BranchCheckOutTime," +
        //                                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }


        //                else if (fromdate != "" && branch != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                 "  ,sm.InTime as BranchStartTime," +
        //                                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                     " sm.OutTime as BranchCheckOutTime," +
        //                                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (typ != typ1 && branch != "" && fromdate != "")
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                 "  ,sm.InTime as BranchStartTime," +
        //     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //     " sm.OutTime as BranchCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + " and e.empid=0" + " and  (convert(date,logs.io_time)='" + date + "' ) Union ";
        //                }
        //                else if (branch != "")
        //                {

        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                     " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                      "  ,sm.InTime as BranchStartTime," +
        //                                          " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                          " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                          " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                          " sm.OutTime as BranchCheckOutTime," +
        //                                          " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                          " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                          " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                       " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                       "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!=''" + " and  (convert(date,logs.io_time)='" + date + "' )  Union ";
        //                }
        //                else
        //                {
        //                    qry += "select distinct " + "D" + st1s + ",Convert(varchar(10),cast(io_time as DATETIME),103) as Date," +
        //                                                                                                " e.ShortName as EmpName,e.EmpId,des.code as Designation,case when  f.BranchName in ('HEADOFFICE-1','HEADOFFICE-2','HEADOFFICE-3','HEADOFFICE-4') then 'Head Office' else f.BranchName end as BrDept" +
        //                                                                                                 "  ,sm.InTime as BranchStartTime," +
        //                                                                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
        //                                                                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) end as EmpCheckInTime," +
        //                                                                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
        //                                                                     " sm.OutTime as BranchCheckOutTime," +
        //                                                                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond1 + "28, 2) + '' " +
        //                                                                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','C-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5)  end as EmpCheckOutTime," +
        //                                                                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond1 + "31, 5) else '00.00' end as EarlyBy" +
        //                                                                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
        //                                                                                                  "  join timesheet_logs logs on logs.user_id=t.userid join branch_device f on f.device_id=logs.device_id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!=''" + " and  convert(date,io_time)='" + start + "'  Union ";
        //                }

        //            }

        //            qry = qry.Substring(0, qry.Length - 6) + "order by Date desc";

        //        }

        //    }



        //    return sh.Get_Table_FromQry(qry);
        //}
        public DataTable EmployeeTimesheetMstdata(string branch, string fromdate, string todate, string empcode, string Type, string self, int EmpIds, string intime, string outtime)

        {
            string typ = intime;
            string typ1 = outtime;
            string qry = "";
            string query = "";
            string cond = "";
            string cond1 = "";
            string cond2 = "";
            string cond3 = "";
            string cond4 = "";
            string cond5 = "";
            string cond6 = "";
            var leavetypeName = "";
            var EmpcheckintymName = "";
            string leavetypequery = "";
            string Empcheckintym = "";
            string temptabl = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'timesheettemp';";
            DataTable Dt = sh.Get_Table_FromQry(temptabl);
            if (Dt.Rows.Count > 0)
            {
                string deletetemptbl = "drop table timesheettemp";
                sh.Run_UPDDEL_ExecuteNonQuery(deletetemptbl);
            }
            string qrymanualmain1 = "";
            string qrymanual = "", qrymanual1 = "select * into timesheettemp from ( ", qrymanual2 = " union all ", qrymanual3 = " ) as a  ", qrymanualmain = "";
            if (branch == string.Empty || branch == null || branch == "Select")
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                qry = " select e.ShortName as EmpName,e.EmpId,'Others' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                  " ,Convert(varchar,GETDATE(),103) as Date,'hh:mm tt' as EmpCheckInTime,'' as LateBy," +
                  " b.EndTime as BranchCheckOutTime,'hh:mm tt' as EmpCheckOutTime,'' as EarlyBy,'Biometric' as BioManual  " +
                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                  " join Departments d on e.Department = d.id" + cond
                   + " order by Date desc";
                return sh.Get_Table_FromQry(qry);
            }
            else
            {
                if (fromdate != "")
                {
                    if (branch.StartsWith("-") && branch != "HeadOffice-All")
                    {
                        branch = branch.Substring(1);
                    }
                    branch = branch.Trim();
                    if (branch.Contains("and"))
                    {
                        branch = branch.Replace("and", "&");
                    }
                    //todate day
                    string strtoDate = todate;
                    string[] sa1 = strtoDate.Split('/');
                    string st1 = sa1[0];
                    if (st1.StartsWith("0"))
                    {
                        st1 = st1.Substring(1);
                    }
                    string s2t = sa1[1];
                    //fromdate day
                    string strDate = fromdate;
                    string[] sa = strDate.Split('/');
                    string s1 = sa[0];
                    if (s1.StartsWith("0"))
                    {
                        s1 = s1.Substring(1);
                    }
                    string s2 = sa[1];
                    string s3 = sa[2];
                    cond1 = "D" + s1 + ",";
                    cond2 = s2;
                    cond3 = " where t.year=" + s3 + " and t.month=" + s2;
                    cond4 = fromdate;
                    if (empcode == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and emp.EmpId = " + empcode;
                    }
                    if (branch == string.Empty || fromdate == string.Empty)
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    {
                        //cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')and e.empid=" + EmpIds + "";
                        cond5 = " and e.empid=" + EmpIds + "";
                    }
                    else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate == "" && todate == "" && empcode != "" && self == "False")
                    {
                        cond5 = "and e.EmpId =" + empcode;
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {
                        cond5 = "and e.EmpId =" + empcode;
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && empcode == "" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and e.empid=" + empcode + " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch == "HeadOffice-All" && branch != "All" && fromdate == "" && todate == "" && empcode != "")
                    {
                        cond = " and e.Department != 46" + " and e.Branch = 43" + "and e.EmpId =" + empcode;
                    }
                    else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate == "" && todate == "" && empcode == "")
                    {
                        cond = "and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate != "" && todate != "" && empcode == "" && self == "False")
                    {
                        cond = "and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and e.empid.=" + empcode + " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && self == "False")
                    {
                        cond5 = "and e.EmpId =" + empcode;
                    }
                    else if (branch == "All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    //if (branch != string.Empty && fromdate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    //{
                    //    cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    //}
                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && empcode != "" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and e.empid=" + empcode + " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    ////else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    ////{
                    ////    cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    ////}
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "False")
                    {
                        cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }

                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and e.empid=" + EmpIds + "";
                    }
                    else if (branch != "All" && branch != "HeadOffice-All" && fromdate != string.Empty && self == "True")
                    {
                        cond5 = "and e.empid=" + EmpIds + "";
                    }
                    else if (branch == "HeadOffice-All" && fromdate != string.Empty)
                    {
                        cond5 = " and e.Department != 46" + " and e.Branch = 43";
                    }
                    //else if (branch != "" && empcode == "" )
                    //{
                    //    cond5 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    //}

                    //else if (branch != "" && empcode != "")
                    //{
                    //    cond5 = " and empid=" + empcode + "  and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    //}
                    //between fromdate and todate
                    List<DateTime> allDates = new List<DateTime>();
                    // string fromdate= DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
                    for (DateTime date = Convert.ToDateTime(fromdate); date <= Convert.ToDateTime(todate); date = date.AddDays(1))
                    {
                        allDates.Add(date);
                    }
                    DateTime[] dates = allDates.ToArray();
                    string test = string.Join(",", dates);
                    DateTime startnew = Convert.ToDateTime(fromdate);
                    DateTime start = Convert.ToDateTime(fromdate).AddDays(-1);
                    DateTime end = Convert.ToDateTime(todate);
                    DateTime chunkEnd;
                    int dayChunkSize = 1;
                    while ((chunkEnd = start.AddDays(dayChunkSize)) <= end)
                    {

                        Tuple.Create(start, chunkEnd);
                        start = chunkEnd;
                        DateTime chunkend1 = Convert.ToDateTime(start);
                        string start1 = chunkend1.ToString("dd/MM/yyyy");
                        string start2 = chunkend1.ToString("yyyy/MM/dd");
                        string[] starta = start1.Split('-');
                        string st1s = starta[0];
                        if (st1s.StartsWith("0"))
                        {
                            st1s = st1s.Substring(1);
                        }
                        cond6 = "D" + st1s;
                        string empidcode = "";
                        string AllEmpid = "";
                        string cond7 = "";
                        string conyy = " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "ML" + "%" + "'" + "and " + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "CL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "PL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "MTL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "PTL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "LOP" + "%" + "W-Off" + "%" + "'" + " and" +
                                                                                    " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "C-OFF" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "SCL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "EOL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "OD" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "LA" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "ED" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "AB" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "PR" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "NA" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "MA" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "HD" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "HL" + "%" + "'" + " and" + " D" + st1s + " " + "not" + " " + "Like" + "" + "'" + "%" + "CW-OFF" + "%" + "'";
                        string contype = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + intime + "%" + "'";
                        string contype6 = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + "__" + outtime + "%" + "'";
                        //  string contype = "D" + st1s + " = '" + typ+"' ";
                        string contypeod = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + outtime + "__" + "%" + "'";
                        string contype1 = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + "__" + outtime + "%" + "'";
                        string contype2 = " D" + st1s + " " + "Like" + "" + "'" + "%" + intime + "%" + "'";
                        string contype4 = " D" + st1s + " " + "Like" + "" + "'" + "%" + outtime + "%" + "'";
                        string contypelast2 = " D" + st1s + " " + "Like" + "" + "'" + "%" + intime + "%" + "'" + " and" + " D" + st1s + " " + "Like" + "" + "'" + "%" + outtime + "%" + "'";
                        string contypelast3 = " D" + st1s + " " + "=" + "" + "'" + intime + "'" + " and" + " D" + st1s + " " + "=" + "" + "'" + outtime + "'";

                        string conlast = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + intime + "__" + "%" + "'" + " and" + " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + "__" + outtime + "%" + "'";
                        string contype3 = " D" + st1s + "_" + "Status" + " " + "Like" + "" + "'" + "%" + outtime + "__" + "%" + "'";
                        if (empcode == "" && fromdate != string.Empty && todate != string.Empty && (branch == "All" || branch == "HeadOffice-All") && self == "False")
                        {
                            Empcheckintym = "select distinct case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) Not In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF','OD-OfWk')  then SUBSTRING(" + "D" + st1s + "," + "19, 2)" +
                                                "end as EmpCheckInTime, t.UserId ,lt.code from timesheet_emp_month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id  join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join leaves  l on e.id = l.empid join leavetypes lt on l.leavetype = lt.id " +
                                                " where t.year = Year('" + start + "') and t.month = Month('" + start + "') and SUBSTRING(" + "D" + st1s + "," + "19, 2) != ''  and t.userid in (select e.empid from leaves l join leavetypes lt on l.leavetype = lt.id join employees e on e.id = l.empid join branches b on e.Branch = b.id join Departments d on e.Department = d.id " +
                                                " where leavesyear = " + s3 + " and startdate = '" + start + "' and (lt.code = 'CL' or lt.code = 'CW-OFF') and  l.Status in ('Approved','Pending','Forwarded')) and e.EmpId != 1 and e.RetirementDate >= convert(Date, '" + start + "')" + cond5 + " and sm.shifttype = (case when(select count(*)" +
                                                "from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto and leavesyear = " + s3 + " and startdate = '" + start + "' and (lt.code = 'CL' or lt.code = 'CW-OFF') and  l.Status in ('Approved','Pending','Forwarded')";

                            var dt1 = sh.Get_Table_FromQry(Empcheckintym);

                            //EmpcheckintymName = dt1.AsEnumerable().FirstOrDefault()?[0].ToString();    
                            if (dt1.Rows.Count > 0)
                            {
                                foreach (DataRow leavetym in dt1.Rows)
                                {
                                    EmpcheckintymName = leavetym["EmpCheckInTime"].ToString();
                                    empidcode = leavetym["UserId"].ToString();
                                    leavetypeName = leavetym["code"].ToString();
                                    query = "select distinct t.User_Id as Empid from timesheet_logs t, employees e where e.empid = t.user_id and  t.io_time >= '" + start2 + " 00:00:00.000'  and t.io_time <= '" + start2 + " 23:59:59.000'  and  e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "') order by t.user_id";
                                    DataTable dtt = sh.Get_Table_FromQry(query);
                                    //string   kar = "select distinct t.User_Id as Empid from timesheet_logs t join employees e on e.empid = t.user_id where user_id not  in('" + empidcode + "') and  io_time >= '" + start2 + " 00:00:00.000'  and io_time <= '" + start2 + " 23:59:59.000'  and  e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "') order by t.user_id";
                                    //var employeid = dtt.AsEnumerable().FirstOrDefault()[0].ToString();
                                    if (dtt.Rows.Count > 0)
                                    {
                                        var allemps = dtt.Rows;
                                        Boolean empcheck = false;
                                        foreach (DataRow empid in allemps)
                                        {
                                            AllEmpid = empid["Empid"].ToString();
                                            if (AllEmpid != empidcode)
                                            {
                                                empcheck = true;
                                                //qry += " select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                //                           " convert(varchar, e.EmpId) as empcode , des.Code as designation, e.Shortname as empname,  + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol" +
                                                //                            "  ,sm.InTime as BranchStartTime," + "  ,sm.groupname as biomanual," +
                                                //" case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                //" when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " when " + cond6 + " like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end )  end as EmpCheckInTime," +
                                                //" case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                //" sm.OutTime as BranchCheckOutTime," +
                                                //" case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                //" when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " when " + cond6 + " like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end ) end as EmpCheckOutTime," +
                                                //" case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy" +
                                                //                             " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id join od_otherduty od on od.empid = e.id " +
                                                //                             " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!='' " + " " +
                                                //                             " and sm.shifttype =(case when(select count(*)from designations " +
                                                //                             " ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "' between sm.validfrom and sm.validto Union ";

                                                //                                     qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                //                           " convert(varchar, e.EmpId) + ', ' + e.Shortname + ',' + des.Code + ',' + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol" +
                                                //                            "  ,sm.InTime as BranchStartTime," +
                                                //" case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                //" when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " when " + cond6 + " like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end )  end as EmpCheckInTime," +
                                                //" case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                //" sm.OutTime as BranchCheckOutTime," +
                                                //" case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                //" when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " when " + cond6 + " like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end ) end as EmpCheckOutTime," +
                                                //" case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy" +
                                                //                             " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id join od_otherduty od on od.empid = e.id " +
                                                //                             " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!='' " + " " +
                                                //                             " and sm.shifttype =(case when(select count(*)from designations " +
                                                //                             " ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "' between sm.validfrom and sm.validto Union ";

                                                //qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                //                           " convert(varchar, e.EmpId) as empcode , des.Code as designation, e.Shortname as empname,  + case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as grpcol" +
                                                //                            "  ,sm.InTime as BranchStartTime," + "  ,sm.InTime as BranchStartTime,"
                                                //" case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                //" when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " when " + cond6 + " like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end )  end as EmpCheckInTime," +
                                                //" case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                //" sm.OutTime as BranchCheckOutTime," +
                                                //" case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                //" when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " when " + cond6 + " like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end ) end as EmpCheckOutTime," +
                                                //" case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy" +
                                                //                             " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id join od_otherduty od on od.empid = e.id " +
                                                //                             " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, GETDATE())" + cond5 + " and " + cond6 + "!='' " + " " +
                                                //                             " and sm.shifttype =(case when(select count(*)from designations " +
                                                //                             " ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "' between sm.validfrom and sm.validto Union "; ;
                                            }
                                            else
                                            {
                                                if (intime == "CW-OFF" || intime == "" || intime == "CL")
                                                {
                                                    if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                                    {
                                                        qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                                                "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                                                "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                                                "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                                                "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where empid = " + empidcode + ") and " +
                                                                "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                                                 " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                                    }
                                                }


                                            }

                                        }

                                        if (empcheck == true)
                                        {
                                            StringBuilder qryBuilder = new StringBuilder();
                                            qryBuilder.Append("select distinct ")
                                                .Append("D").Append(st1s).Append(",")
                                                .Append("Convert(varchar(10),cast('").Append(start).Append("' as DATETIME),103) as Date,")
                                                .Append("e.Shortname as EmpName, convert(varchar, e.EmpId) as EmpId, des.Code as Designation,")
                                                .Append("case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as BrDept,")
                                                .Append("sm.InTime as BranchStartTime,")
                                                .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 19, 2) = 'NA' then SUBSTRING(").Append("D").Append(st1s).Append(", 19, 2) + '' ")
                                                .Append(" when ").Append(cond6).Append(" In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or ").Append(cond6).Append(" like '%OD%' then ").Append(cond6)
                                                .Append(" when ").Append(cond6).Append(" like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(").Append("D").Append(st1s).Append(", 1, 5) + (case when D").Append(st1s).Append("_meridian is not null then  subString(D").Append(st1s).Append("_meridian, 1, 2) else '' end) end as EmpCheckInTime,")
                                                .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 19, 2) = 'LA' then  SUBSTRING(").Append("D").Append(st1s).Append(", 22, 5) else '00.00' end as LateBy,")
                                                .Append("sm.OutTime as BranchCheckOutTime,")
                                                .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 28, 2) = 'NA' then SUBSTRING(").Append(cond6).Append(", 28, 2) + '' ")
                                                .Append(" when ").Append(cond6).Append(" In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or ").Append(cond6).Append(" like '%OD%' then ").Append(cond6)
                                                .Append(" when ").Append(cond6).Append(" like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(").Append("D").Append(st1s).Append(", 7, 5) + (case when D").Append(st1s).Append("_meridian is not null then  subString(D").Append(st1s).Append("_meridian, 4, 2) else '' end) end as EmpCheckOutTime,")
                                                .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 28, 2) = 'ED' then  SUBSTRING(").Append(cond6).Append(", 31, 5) else '00.00' end as EarlyBy,")
                                                .Append("sm.groupname as BioManual")
                                                .Append(" from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation = des.Id")
                                                .Append(" join branches b on e.Branch = b.id join od_otherduty od on od.empid = e.id join Departments d on e.Department = d.id")
                                                .Append(" join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id = t.UserId")
                                                .Append(" where t.year = Year('").Append(start).Append("') and t.month = Month('").Append(start).Append("') and e.EmpId != 1 and e.RetirementDate >= convert(Date, GETDATE())")
                                                .Append(" and ").Append("D").Append(st1s).Append(" LIKE ").Append("'%").Append(intime).Append("%'")
                                                .Append(cond5).Append(" and ").Append(cond6).Append(" != '' ")
                                                .Append("and sm.shifttype = (case when (select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1' else '0' end)")
                                                .Append("and '").Append(start).Append("' between sm.validfrom and sm.validto Union ");

                                            qry += qryBuilder.ToString();
                                        }

                                    }
                                }
                            }
                            if (dt1.Rows.Count == 0)
                            {
                                query = "select distinct t.User_Id as Empid from timesheet_logs t join employees e on e.empid = t.user_id and  io_time >= '" + start2 + " 00:00:00.000'  and io_time <= '" + start2 + " 23:59:59.000'  and  e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "') order by t.user_id";
                                DataTable dtt = sh.Get_Table_FromQry(query);
                                //string   kar = "select distinct t.User_Id as Empid from timesheet_logs t join employees e on e.empid = t.user_id where user_id not  in('" + empidcode + "') and  io_time >= '" + start2 + " 00:00:00.000'  and io_time <= '" + start2 + " 23:59:59.000'  and  e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "') order by t.user_id";
                                //var employeid = dtt.AsEnumerable().FirstOrDefault()[0].ToString();
                                if (dtt.Rows.Count > 0)
                                {
                                    var allemps = dtt.Rows;
                                    Boolean checkemps = false;
                                    foreach (DataRow empid in allemps)
                                    {
                                        AllEmpid = empid["Empid"].ToString();
                                        if (AllEmpid != empidcode)
                                        {
                                            checkemps = true;

                                        }

                                    }
                                    if (checkemps == true)
                                    {
                                        StringBuilder qryBuilder = new StringBuilder();
                                        qryBuilder.Append("select distinct ")
                                            .Append("D").Append(st1s).Append(",")
                                            .Append("Convert(varchar(10),cast('").Append(start).Append("' as DATETIME),103) as Date,")
                                            .Append("e.Shortname as EmpName, convert(varchar, e.EmpId) as EmpId, des.Code as Designation,")
                                            .Append("case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as BrDept,")
                                            .Append("sm.InTime as BranchStartTime,")
                                            .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 19, 2) = 'NA' then SUBSTRING(").Append("D").Append(st1s).Append(", 19, 2) + '' ")
                                            .Append(" when ").Append(cond6).Append(" In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or ").Append(cond6).Append(" like '%OD%' then ").Append(cond6)
                                            .Append(" when ").Append(cond6).Append(" like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(").Append("D").Append(st1s).Append(", 1, 5) + (case when D").Append(st1s).Append("_meridian is not null then  subString(D").Append(st1s).Append("_meridian, 1, 2) else '' end) end as EmpCheckInTime,")
                                            .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 19, 2) = 'LA' then  SUBSTRING(").Append("D").Append(st1s).Append(", 22, 5) else '00.00' end as LateBy,")
                                            .Append("sm.OutTime as BranchCheckOutTime,")
                                            .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 28, 2) = 'NA' then SUBSTRING(").Append(cond6).Append(", 28, 2) + '' ")
                                            .Append(" when ").Append(cond6).Append(" In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or ").Append(cond6).Append(" like '%OD%' then ").Append(cond6)
                                            .Append(" when ").Append(cond6).Append(" like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(").Append("D").Append(st1s).Append(", 7, 5) + (case when D").Append(st1s).Append("_meridian is not null then  subString(D").Append(st1s).Append("_meridian, 4, 2) else '' end) end as EmpCheckOutTime,")
                                            .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 28, 2) = 'ED' then  SUBSTRING(").Append(cond6).Append(", 31, 5) else '00.00' end as EarlyBy,")
                                            .Append("sm.groupname as BioManual")
                                            .Append(" from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation = des.Id")
                                            .Append(" join branches b on e.Branch = b.id join od_otherduty od on od.empid = e.id join Departments d on e.Department = d.id")
                                            .Append(" join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id = t.UserId")
                                            .Append(" where t.year = Year('").Append(start).Append("') and t.month = Month('").Append(start).Append("') and e.EmpId != 1 and e.RetirementDate >= convert(Date, GETDATE())")
                                            .Append(" and ").Append("D").Append(st1s).Append(" LIKE ").Append("'%").Append(intime).Append("%'")
                                            .Append(cond5).Append(" and ").Append(cond6).Append(" != '' ")
                                            .Append("and sm.shifttype = (case when (select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1' else '0' end)")
                                            .Append("and '").Append(start).Append("' between sm.validfrom and sm.validto Union ");

                                        qry += qryBuilder.ToString();
                                    }
                                }
                            }
                            //else
                            //{
                            //    query = "select distinct t.User_Id as Empid from timesheet_logs t join employees e on e.empid = t.user_id and  io_time >= '" + start2 + " 00:00:00.000'  and io_time <= '" + start2 + " 23:59:59.000'  and  e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "') order by t.user_id";
                            //    DataTable dtt = sh.Get_Table_FromQry(query);
                            //    //string   kar = "select distinct t.User_Id as Empid from timesheet_logs t join employees e on e.empid = t.user_id where user_id not  in('" + empidcode + "') and  io_time >= '" + start2 + " 00:00:00.000'  and io_time <= '" + start2 + " 23:59:59.000'  and  e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "') order by t.user_id";
                            //    //var employeid = dtt.AsEnumerable().FirstOrDefault()[0].ToString();
                            //    if (dtt.Rows.Count > 0)
                            //    {
                            //        var allemps = dtt.Rows;
                            //        foreach (DataRow empid in allemps)
                            //        {
                            //            AllEmpid = empid["Empid"].ToString();
                            //            if (AllEmpid != empidcode)
                            //            {
                            //                StringBuilder qryBuilder = new StringBuilder();
                            //                qryBuilder.Append("select distinct ")
                            //                    .Append("D").Append(st1s).Append(",")
                            //                    .Append("Convert(varchar(10),cast('").Append(start).Append("' as DATETIME),103) as Date,")
                            //                    .Append("e.Shortname as EmpName, convert(varchar, e.EmpId) as EmpId, des.Code as Designation,")
                            //                    .Append("case when b.Name = 'OtherBranch' then d.Name when b.Name = 'HeadOffice' then d.Name else b.Name end as BrDept,")
                            //                    .Append("sm.InTime as BranchStartTime,")
                            //                    .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 19, 2) = 'NA' then SUBSTRING(").Append("D").Append(st1s).Append(", 19, 2) + '' ")
                            //                    .Append(" when ").Append(cond6).Append(" In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or ").Append(cond6).Append(" like '%OD%' then ").Append(cond6)
                            //                    .Append(" when ").Append(cond6).Append(" like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(").Append("D").Append(st1s).Append(", 1, 5) + (case when D").Append(st1s).Append("_meridian is not null then  subString(D").Append(st1s).Append("_meridian, 1, 2) else '' end) end as EmpCheckInTime,")
                            //                    .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 19, 2) = 'LA' then  SUBSTRING(").Append("D").Append(st1s).Append(", 22, 5) else '00.00' end as LateBy,")
                            //                    .Append("sm.OutTime as BranchCheckOutTime,")
                            //                    .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 28, 2) = 'NA' then SUBSTRING(").Append(cond6).Append(", 28, 2) + '' ")
                            //                    .Append(" when ").Append(cond6).Append(" In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or ").Append(cond6).Append(" like '%OD%' then ").Append(cond6)
                            //                    .Append(" when ").Append(cond6).Append(" like  '%WEEK-OFF%'  AND od.Status= 'Cancelled' then 'AB' else SUBSTRING(").Append("D").Append(st1s).Append(", 7, 5) + (case when D").Append(st1s).Append("_meridian is not null then  subString(D").Append(st1s).Append("_meridian, 4, 2) else '' end) end as EmpCheckOutTime,")
                            //                    .Append("case when SUBSTRING(").Append("D").Append(st1s).Append(", 28, 2) = 'ED' then  SUBSTRING(").Append(cond6).Append(", 31, 5) else '00.00' end as EarlyBy,")
                            //                    .Append("sm.groupname as BioManual")
                            //                    .Append(" from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation = des.Id")
                            //                    .Append(" join branches b on e.Branch = b.id join od_otherduty od on od.empid = e.id join Departments d on e.Department = d.id")
                            //                    .Append(" join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id = t.UserId")
                            //                    .Append(" where t.year = Year('").Append(start).Append("') and t.month = Month('").Append(start).Append("') and e.EmpId != 1 and e.RetirementDate >= convert(Date, GETDATE())")
                            //                    .Append(" and ").Append("D").Append(st1s).Append(" LIKE ").Append("'%").Append(intime).Append("%'")
                            //                    .Append(cond5).Append(" and ").Append(cond6).Append(" != '' ")
                            //                    .Append("and sm.shifttype = (case when (select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1' else '0' end)")
                            //                    .Append("and '").Append(start).Append("' between sm.validfrom and sm.validto Union ");

                            //                qry += qryBuilder.ToString();
                            //            }

                            //        }



                            //    }
                            //}
                        }
                        else
                        {
                            leavetypequery = "select lt.code from leaves l join leavetypes lt on l.leavetype = lt.id join employees e on e.id = l.empid join branches b on e.Branch = b.id join Departments d on e.Department = d.id  where leavesyear = " + s3 + " and startdate = '" + start + "'  and  l.Status in ('Approved','Pending','Forwarded') " + cond5;
                            var dt = sh.Get_Table_FromQry(leavetypequery);
                            leavetypeName = dt.AsEnumerable().FirstOrDefault()?[0].ToString();
                            Empcheckintym = "select distinct case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) Not In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF','OD-OfWk')  then SUBSTRING(" + "D" + st1s + "," + "19, 2)" +
                                              "end as EmpCheckInTime from timesheet_emp_month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch" +
                                              " where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId != 1 and e.RetirementDate >= convert(Date, '" + start + "')" + cond5 + " and sm.shifttype = (case when(select count(*)" +
                                              "from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto ";

                            var dt1 = sh.Get_Table_FromQry(Empcheckintym);
                            EmpcheckintymName = dt1.AsEnumerable().FirstOrDefault()?[0].ToString();

                            string Empcheckouttym = "select case  when SUBSTRING(" + "D" + st1s + "," + "28, 2) not In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF','OD-OfWk')   then SUBSTRING(D6,28, 2) " +
                                                 "end as EmpCheckOutTime from timesheet_emp_month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch" +
                                                " where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId != 1 and e.RetirementDate >= convert(Date, '" + start + "')" + cond5 + " and sm.shifttype = (case when(select count(*)" +
                                                    "from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto ";

                            if ((typ != typ1 && typ1 != "" && (typ == "Others" || typ1 == "Others") && branch != "" && fromdate != "") && (typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "W-Off" || typ == "C-OFF" || typ == "CW-OFF" || typ == "SCL" || typ == "EOL"))
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                             "  ,sm.InTime as BranchStartTime," +
                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                 " sm.OutTime as BranchCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual" +
                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and '" + start + "'  between sm.validfrom and sm.validto and e.empid!=0" + "  Union ";
                                }
                            }
                            else if (typ != "" && typ == "NA" && typ1 == "" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                        "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                        "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                        "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                        "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                        "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                         " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                 "  ,sm.InTime a" +
                                                 "s BranchStartTime," +
                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                     " sm.OutTime as BranchCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual" +
                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                  " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype + " and " + cond6 + "!='' " + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" +

                                                    " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ1 != "" && typ == "" && typ1 == "NA" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                 "  ,sm.InTime as BranchStartTime," +
                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                     " sm.OutTime as BranchCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual" +
                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                  " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype6 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ1 == "" && typ == "AB" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                             " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                              "  ,sm.InTime as BranchStartTime," +
                                                  " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                  " sm.OutTime as BranchCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual" +
                                                                               " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                               " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype2 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "" && typ1 != "" && typ1 == "AB" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                             " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                              "  ,sm.InTime as BranchStartTime," +
                                                  " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                  " sm.OutTime as BranchCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual" +
                                                                               " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                               " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype4 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ1 != "" && (typ == "AB" && typ1 == "AB") && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                             " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                              "  ,sm.InTime as BranchStartTime," +
                                                  " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                  " sm.OutTime as BranchCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual" +
                                                                               " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                               " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contypelast2 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ1 != "" && (typ == "LA" && typ1 == "NA") && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct  " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                             " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                              "  ,sm.InTime as BranchStartTime," +
                                                  " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                  " sm.OutTime as BranchCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual" +
                                                                               " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                               " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + conlast + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "" && (typ1 == "CL" || typ1 == "ML" || typ1 == "PTL" || typ1 == "PL" || typ1 == "MTL" || typ1 == "SCL" || typ1 == "LOP" || typ1 == "W-Off" || typ1 == "EOL" || typ1 == "C-OFF" || typ1 == "CW-OFF") && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                               " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                "  ,sm.InTime as BranchStartTime," +
                    " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                    " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                    " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                    " sm.OutTime as BranchCheckOutTime," +
                    " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                    " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                    " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                 " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                 " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype4 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "" && typ1 != "" && typ1 == "HL" && fromdate != "" && branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                               " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                "  ,sm.InTime as BranchStartTime," +
                    " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                    " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                    " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                    " sm.OutTime as BranchCheckOutTime," +
                    " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                    " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                    " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                 " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                 " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype4 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "" && typ1 != "" && typ1 == "OD" && typ != "Others" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end ) end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype3 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ1 != "" && typ != "Others" && typ == "NA" && typ1 == "NA" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contypelast3 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ != "" && typ != "Others" && typ == "PR" && typ1 == "OD" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype1 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if ((typ != typ1 && typ1 != "" && (typ != "Others" || typ1 != "Others") && branch != "" && fromdate != "") && ((typ == "AB" || typ == "HL") && typ1 == "OD"))
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                             "  ,sm.InTime as BranchStartTime," +
                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                 " sm.OutTime as BranchCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + " and e.empid!=0  " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ1 != "" && (typ == "NA" || typ == "PR" || typ == "LA" || typ == "ED") && (typ1 == "NA" || typ1 == "PR" || typ1 == "LA" || typ1 == "ED") && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                             " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                              "  ,sm.InTime as BranchStartTime," +
                                                  " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                  " sm.OutTime as BranchCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                                               " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                               " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + conlast + " and " + cond6 + "!=''" + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "" && typ1 != "" && typ1 == "HL" && fromdate != "" && branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                 "  ,sm.InTime as BranchStartTime," +
                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                     " sm.OutTime as BranchCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                  " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype2 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ1 != "" && typ1 == "HL" && typ == "HL" && fromdate != "" && branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                 "  ,sm.InTime as BranchStartTime," +
                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                     " sm.OutTime as BranchCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                  " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype2 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if ((typ != typ1 && typ1 != "" && (typ == "Others" || typ1 == "Others") && branch != "" && fromdate != "") && (typ1 == "CL" || typ1 == "PL" || typ1 == "ML" || typ1 == "MTL" || typ1 == "PTL" || typ1 == "LOP" || typ1 == "C-OFF" || typ1 == "CW-OFF" || typ1 == "SCL" || typ1 == "EOL"))
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                             "  ,sm.InTime as BranchStartTime," +
                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                 " sm.OutTime as BranchCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + " and e.empid!=0  " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if ((typ != typ1 && typ1 != "" && (typ != "Others" || typ1 != "Others") && branch != "" && fromdate != "") && (typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "W-Off" || typ == "C-OFF" || typ == "CW-OFF" || typ == "SCL" || typ == "EOL" || typ == "HL" || typ == "HD" || typ == "NA" || typ == "Others") && (typ1 == "OD" || typ1 == "LA" || typ1 == "NA" || typ1 == "PR" || typ1 == "AB" || typ1 == "ED" || typ1 == "MA" || typ == "NA" || typ1 == "HD" || typ1 == "HL"))
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct  " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                             "  ,sm.InTime as BranchStartTime," +
                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                 " sm.OutTime as BranchCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + " and D" + st1s + " like('%" + typ + "%') and e.empid!=0 " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if ((typ != typ1 && typ1 != "" && (typ != "Others" || typ1 != "Others") && branch != "" && fromdate != "") && (typ1 == "CL" || typ1 == "PL" || typ1 == "ML" || typ1 == "MTL" || typ1 == "PTL" || typ1 == "LOP" || typ1 == "W-Off" || typ1 == "CW-OFF" || typ1 == "C-OFF" || typ1 == "SCL" || typ1 == "EOL" || typ1 == "HL" || typ1 == "HD" || typ1 == "NA" || typ1 == "Others" || typ1 == "AB" || typ1 == "LA" || typ1 == "ED" || typ1 == "MA") && (typ == "OD" || typ == "LA" || typ == "NA" || typ == "PR" || typ == "AB" || typ == "NA" || typ == "ED" || typ == "MA" || typ == "HD" || typ == "HL"))
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                             "  ,sm.InTime as BranchStartTime," +
                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                 " sm.OutTime as BranchCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + " and e.empid!=0 " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (fromdate != "" && typ != typ1 && (typ == "Others" || typ1 == "Others") && branch != "" && ((typ == "CL" || typ == "PL" || typ == "ML" || typ == "CW-OFF" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "W-Off" || typ == "C-OFF" || typ == "SCL" || typ == "EOL")))
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where l.empid = (select id from employees where " + cond5 + ") and " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')   and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {


                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                             "  ,sm.InTime as BranchStartTime," +
                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                 " sm.OutTime as BranchCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + " and e.empid!=0  " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "Others" && typ1 != "" && typ1 != "Others" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,8) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "28,2) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + conyy + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ1 == "Others" && typ != "" && typ != "Others" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF,'CW-OFF'') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + conyy + " and " + cond6 + "!=''  " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if ((typ == "Others" || typ1 == "Others") && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end ) end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + conyy + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if ((typ == "Others" || typ1 == "Others") && typ != "" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + conyy + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "Others" && typ1 == "" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + conyy + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ1 == "Others" && typ == "" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + conyy + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if ((typ == "Others" || typ1 == "Others") && typ1 != "" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + conyy + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }

                            else if ((typ != typ1 && typ1 != "" && branch != "" && fromdate != "") && (typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "W-Off" || typ == "C-OFF" || typ == "CW-OFF" || typ == "SCL" || typ == "EOL"))
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct  " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                             "  ,sm.InTime as BranchStartTime," +
                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                 " sm.OutTime as BranchCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + " and D" + st1s + " = '" + typ + "' " + cond5 + " and e.empid!=0 " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }

                            else if (fromdate != "" && (typ != "" && typ != "Others" && (typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "W-Off" || typ == "C-OFF" || typ == "CW-OFF" || typ == "SCL" || typ == "EOL" || typ == "HL" || typ == "AB" || typ == "HD" || typ == "OD" || typ == "LA" || typ == "ED")) && typ1 == "Others" && branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                 "  ,sm.InTime as BranchStartTime," +
                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                     " sm.OutTime as BranchCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                  " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contypelast2 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "" && fromdate != "" && typ1 != "" && typ1 != "Others" && (typ1 == "AB") && branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct  " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                 "  ,sm.InTime as BranchStartTime," +
                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                     " sm.OutTime as BranchCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                  " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contypeod + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (fromdate != "" && typ1 != "" && typ1 != "Others" && (typ1 != "" && typ1 != "Others" && (typ1 == "CL" || typ1 == "PL" || typ1 == "ML" || typ1 == "MTL" || typ1 == "PTL" || typ1 == "LOP" || typ1 == "W-Off" || typ1 == "C-OFF" || typ1 == "SCL" || typ1 == "EOL" || typ1 == "HL" || typ1 == "AB" || typ1 == "HD") && branch != "" && typ1 == "Others"))
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                 "  ,sm.InTime as BranchStartTime," +
                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                     " sm.OutTime as BranchCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                  " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contypelast2 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if ((typ == "NA" || typ == "PR" || typ == "LA" || typ == "ED") && (typ1 == "NA" || typ1 == "PR" || typ1 == "LA" || typ1 == "ED") && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                             " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                              "  ,sm.InTime as BranchStartTime," +
                                                  " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                  " sm.OutTime as BranchCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                  " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                  " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                                               " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                               " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + conlast + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (fromdate != "" && (typ == "NA" || typ == "AB" || typ == "HL" || typ == "HD" || typ == "PR" || typ == "LA" || typ == "ED") && (typ1 == "AB" || typ1 == "HD" || typ1 == "HL" || typ1 == "NA" || typ1 == "PR" || typ1 == "LA" || typ1 == "ED") && branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct  " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {


                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                             "  ,sm.InTime as BranchStartTime," +
                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end ) end as EmpCheckInTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                 " sm.OutTime as BranchCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + " and e.empid!=0 " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (fromdate != "" && typ1 != "" && typ1 != "Others" && (typ1 == "CL" || typ1 == "PL" || typ1 == "ML" || typ1 == "MTL" || typ1 == "PTL" || typ1 == "LOP" || typ1 == "W-Off" || typ1 == "C-OFF" || typ1 == "CW-OFF" || typ1 == "SCL" || typ1 == "EOL" || typ1 == "HL" || typ1 == "AB" || typ1 == "HD") && branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                 "  ,sm.InTime as BranchStartTime," +
                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                     " sm.OutTime as BranchCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                  " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype2 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }

                            else if (fromdate != "" && typ != "" && typ != "Others" && (typ == "CL" || typ == "PL" || typ == "ML" || typ == "MTL" || typ == "PTL" || typ == "LOP" || typ == "W-Off" || typ == "C-OFF" || typ == "SCL" || typ == "CW-OFF" || typ == "EOL" || typ == "HL" || typ == "AB" || typ == "HD") && branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                 "  ,sm.InTime as BranchStartTime," +
                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                     " sm.OutTime as BranchCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                  " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype2 + " and " + cond6 + "!='' " + " " +
                                                  " and   sm.shifttype =   (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" +

                                                    " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }

                            else if (fromdate != "" && typ1 != "" && typ1 != "Others" && (typ1 == "CL" || typ1 == "PL" || typ1 == "ML" || typ1 == "MTL" || typ1 == "PTL" || typ1 == "LOP" || typ1 == "W-Off" || typ1 == "C-OFF" || typ1 == "CW-OFF" || typ1 == "SCL" || typ1 == "EOL" || typ1 == "HL" || typ1 == "AB" || typ1 == "HD") && branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                 "  ,sm.InTime as BranchStartTime," +
                     " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                     " sm.OutTime as BranchCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                     " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                     " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                  " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                  " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype2 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ1 != "" && typ != "Others" && typ != typ1 && (typ == "OD" && (typ1 == "LA" || typ1 == "ED")) && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct  " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ1 != "" && typ != "Others" && typ != typ1 && (typ1 == "OD" && (typ == "LA" || typ == "ED")) && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF''CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF''CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype1 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ != "Others" && typ == "AB" && typ1 == "AB" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF''CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF''CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != "" && typ != "Others" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1" +
                                                    " and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "" && typ != "Others" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end ) end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 " +
                                                    "and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype1 + " and " + cond6 + "!='' " + " and   sm.shifttype =   (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" +

                                                    " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "NA" && typ == "NA" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ1 != "" && typ1 != "Others" && branch != "" && fromdate != "" && typ1 == "OD")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contypeod + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ1 != "" && typ1 != "Others" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId  where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype1 + " and " + cond6 + "!='' " + " and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" + " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }

                            else if (typ1 != "" && typ1 != "" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                  " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                   "  ,sm.InTime as BranchStartTime," +
                       " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                       " sm.OutTime as BranchCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                       " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                       " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                    " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                    " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + contype2 + " and " + cond6 + "!='' " + " and   sm.shifttype =   (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" +

                                                    " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ == "" && typ1 == "" && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                             "  ,sm.InTime as BranchStartTime," +
                                                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                 " sm.OutTime as BranchCheckOutTime," +
                                                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + cond6 + "!='' " + " and   sm.shifttype =   (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" +

                                                    " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if ((typ == "Others" || typ1 == "Others") && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                             "  ,sm.InTime as BranchStartTime," +
                                                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                 " sm.OutTime as BranchCheckOutTime," +
                                                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + cond6 + "!='' " + " and   sm.shifttype =   (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" +

                                                    " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }

                            else if (fromdate != "" && branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                             "  ,sm.InTime as BranchStartTime," +
                                                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                 " sm.OutTime as BranchCheckOutTime," +
                                                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + cond6 + "!='' " + " and   sm.shifttype =   (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" +

                                                    " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (typ != typ1 && branch != "" && fromdate != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                             "  ,sm.InTime as BranchStartTime," +
                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                 " sm.OutTime as BranchCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + " and e.empid!=0  " + " and   sm.shifttype =   (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" +

                                                    " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else if (branch != "")
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {


                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                                 " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                                  "  ,sm.InTime as BranchStartTime," +
                                                      " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                      " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                      " sm.OutTime as BranchCheckOutTime," +
                                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                      " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                      " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                                                   " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                                   " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + cond6 + "!='' " + " and   sm.shifttype =   (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" +

                                                    " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }
                            else
                            {
                                if ((leavetypeName == "CL" || leavetypeName == "CW-OFF") && (EmpcheckintymName == "PR" || EmpcheckintymName == "NA" || EmpcheckintymName == "EA" || EmpcheckintymName == "LA"))
                                {
                                    qry += "select distinct  " + "D" + st1s + "," + "" + "  Convert(varchar(10),cast('" + start + "'  as DATETIME),103)as Date,e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation," +
                                            "'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept," +
                                            "sm.InTime as BranchStartTime,lt.code as EmpCheckInTime,'00.00' as LateBy,sm.OutTime as BranchCheckOutTime,'' as EmpCheckOutTime,'00.00' as EarlyBy,'Biometric' as BioManual " +
                                            "from leaves l join employees e on e.id = l.empid join leavetypes lt on lt.id = l.leavetype join timesheet_emp_month t on t.userid = e.empid join Designations des on e.CurrentDesignation = des.Id join branches b on e.Branch = b.id " +
                                            "join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch where  " +
                                            "startdate = '" + start + "' and t.year = Year('" + start + "') and t.month = Month('" + start + "')  " + cond5 + "  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'" +
                                             " else '0' end ) and '" + start + "'  between sm.validfrom and sm.validto Union ";
                                }
                                else
                                {

                                    qry += "select distinct " + "D" + st1s + "," + "" + "Convert(varchar(10),cast('" + start + "' as DATETIME),103)" + "as Date," +
                                                                                                            " e.ShortName as EmpName,e.EmpId,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept" +
                                                                                                             "  ,sm.InTime as BranchStartTime," +
                                                                                 " case  when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'NA' then SUBSTRING(" + "D" + st1s + "," + "19, 2) + '' " +
                                                                                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + "  else SUBSTRING(" + "D" + st1s + "," + "1,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,1,2) else '' end ) end as EmpCheckInTime," +
                                                                                 " case when SUBSTRING(" + "D" + st1s + "," + "19, 2) = 'LA' then  SUBSTRING(" + "D" + st1s + "," + "22, 5) else '00.00' end as LateBy," +
                                                                                 " sm.OutTime as BranchCheckOutTime," +
                                                                                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'NA' then SUBSTRING(" + cond6 + ",28, 2) + '' " +
                                                                                 " when " + cond6 + " In ('AB','CL','ML','PL','MTL','PTL','EOL','SCL','LOP','W-Off','C-OFF','CW-OFF') or " + cond6 + " like '%OD%' then " + cond6 + " else SUBSTRING(" + "D" + st1s + "," + "7,5) + ( case when D" + st1s + "_meridian is not null then  subString(D" + st1s + "_meridian,4,2) else '' end )  end as EmpCheckOutTime," +
                                                                                 " case when SUBSTRING(" + "D" + st1s + "," + "28, 2) = 'ED' then  SUBSTRING(" + cond6 + ",31, 5) else '00.00' end as EarlyBy,'Biometric' as BioManual " +
                                                                                                              " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
                                                                                                              " join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch  join timesheet_logs tl on tl.user_id=t.UserId   where t.year = Year('" + start + "') and t.month = Month('" + start + "') and e.EmpId!=1 and e.RetirementDate >=convert(Date, '" + start + "')" + cond5 + " and " + cond6 + "!='' " + " and   sm.shifttype =   (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end )" +

                                                    " and               '" + start + "'  between sm.validfrom and sm.validto   Union ";
                                }
                            }

                        }
                    }


                    //qry = qry.Substring(0, qry.Length - 6) + "order by Date desc";
                    qry = qry.Substring(0, qry.Length - 6);
                    if (branch == string.Empty || branch == null)
                    {
                        qrymanual = " Select '00:00#00:00#00:00#EA#00:00#PR#00:00' as D1,Convert(varchar(10),cast(trf.reqfromdate as DATETIME),103) as Date," +
                       " e.ShortName as EmpName, e.Empid as Empid,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept,sm.InTime as BranchStartTime,trf.entrytime as EmpCheckInTime,'00.00'as LateBy,sm.OutTime as BranchCheckOutTime," +
                       " trf.exittime as EmpCheckOutTime,'00.00' as EarlyBy,Concat('Manual','/',trf.status) as BioManual from timesheet_request_form trf join employees e " +
                       " on trf.userid=e.id join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id  join Departments d on " +
                       " e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where trf.Reason_type in('Other','Machine Problem','Internet Issue','Power Problem','All') " +
                       " and trf.entrytime is not null and trf.exittime is not null and trf.userid='" + empcode + "' ";
                    }
                    else
                    {
                        qrymanual = " Select '00:00#00:00#00:00#EA#00:00#PR#00:00' as D1,Convert(varchar(10),cast(trf.reqfromdate as DATETIME),103) as Date," +
                       " e.ShortName as EmpName, e.Empid as Empid,'DesignationDesignationDesignation' as Designation,'DesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignationDesignation ' as BrDept,sm.InTime as BranchStartTime,trf.entrytime as EmpCheckInTime,'00.00'as LateBy,sm.OutTime as BranchCheckOutTime," +
                       " trf.exittime as EmpCheckOutTime,'00.00' as EarlyBy,Concat('Manual','/',trf.status) as BioManual from timesheet_request_form trf join employees e " +
                       " on trf.userid=e.id join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id  join Departments d on " +
                       " e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id where trf.Reason_type in('Other','Machine Problem','Internet Issue','Power Problem','All') " +
                       " and trf.entrytime is not null and trf.exittime is not null ";
                    }
                    if (self == "True")
                    {
                        qrymanual += " and trf.reqfromdate between cast('" + startnew + "' as DATETIME) and cast('" + end + "' as DATETIME) and " +
                            "(b.name='" + branch + "' or d.name='" + branch + "') and e.Empid='" + empcode + "'";
                    }
                    else
                    {
                        if (branch == "All" && empcode == "")
                        {
                            qrymanual += " and trf.reqfromdate between cast('" + startnew + "' as DATETIME) and cast('" + end + "' as DATETIME) ";
                        }
                        else if (branch == "All" && empcode != "")
                        {
                            qrymanual += " and trf.reqfromdate between cast('" + startnew + "' as DATETIME) and cast('" + end + "' as DATETIME) and " +
                            " e.Empid=" + empcode + " ";
                        }
                        else if (branch != "All" && empcode == "")
                        {
                            qrymanual += " and trf.reqfromdate between cast('" + startnew + "' as DATETIME) and cast('" + end + "' as DATETIME) " +
                                " and (b.name='" + branch + "' or d.name='" + branch + "') ";
                        }
                        else if (branch != "All" && empcode != "")
                        {
                            qrymanual += " and trf.reqfromdate between cast('" + startnew + "' as DATETIME) and cast('" + end + "' as DATETIME) " +
                            " and (b.name='" + branch + "' or d.name='" + branch + "') and e.Empid=" + empcode + " ";
                        }
                    }
                    qrymanualmain = qrymanual1 + qry + qrymanual2 + qrymanual + qrymanual3;
                    sh.Run_UPDDEL_ExecuteNonQuery(qrymanualmain);
                    //string table = "select * from timesheettemp ";
                    //DataTable dttemp1 = sh.Get_Table_FromQry(table);
                    string updateDisigna = "";
                    string updateBranchDept = "";
                    try
                    {

                        //for (int i= 0;i <= dttemp1.Rows.Count; i++)
                        //{ 
                        //string empId = dttemp1.Rows[i]["EmpId"].ToString();
                        //DateTime datefor = Convert.ToDateTime(dttemp1.Rows[i]["Date"].ToString());
                        //updateDisigna = "update timesheettemp  set timesheettemp.Designation=Case when B.recordvalue is Null then (select Name from Designations D join Employees E on E.CurrentDesignation = D.Id where e.EmpId=" + empId + " ) else B.recordvalue end from timesheettemp A " +
                        //    "CROSS APPLY (    SELECT[dbo].getDesignation(" + empId + ",cast('" + datefor + "' as DATETIME)) AS recordvalue ) B " +
                        //    "WHERE EmpId = " + empId + " ;";
                        //updateBranchDept = " update timesheettemp  set timesheettemp.BrDept=Case when B.recordvalue is Null then (select case when Bran.name = 'OtherBranch' then D.name when bran.name = 'HeadOffice' then D.name else bran.name end from Branches Bran join Employees E on E.Branch = Bran.Id join  Departments D on E.Department=D.id where e.EmpId=" + empId + " ) else B.recordvalue end from timesheettemp A " +
                        //    "CROSS APPLY (  SELECT[dbo].getBrancDept(" + empId + ",cast('" + datefor + "' as DATETIME)) AS recordvalue ) B " +
                        //    "WHERE EmpId = " + empId + "  ;";

                        updateDisigna = "update timesheettemp set timesheettemp.Designation=isnull([dbo].getDesignation(timesheettemp.empid,(CAST(concat(substring(convert(varchar, timesheettemp.date), 1, 2), '-', dbo.getmonth(convert(int, substring(convert(varchar, timesheettemp.date), 4, 2))), '-', substring(convert(varchar, timesheettemp.date), 7, 4), ' 00:00:00') as datetime))),'tbdtbd')";
                        updateBranchDept = "update timesheettemp set timesheettemp.BrDept=isnull([dbo].getBrancDept(timesheettemp.empid,(CAST(concat(substring(convert(varchar, timesheettemp.date), 1, 2), '-', dbo.getmonth(convert(int, substring(convert(varchar, timesheettemp.date), 4, 2))), '-', substring(convert(varchar, timesheettemp.date), 7, 4), ' 00:00:00') as datetime))),'tbdtbd')";

                        sh.Run_UPDDEL_ExecuteNonQuery(updateDisigna);
                        sh.Run_UPDDEL_ExecuteNonQuery(updateBranchDept);
                        //}

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    string qrytemp = "select distinct EmpId from timesheettemp where Designation='tbdtbd' and BrDept='tbdtbd'";
                    DataTable dttemp = sh.Get_Table_FromQry(qrytemp);
                    string upadate = "";
                    foreach (DataRow dr in dttemp.Rows)
                    {
                        string emp_code = dr["EmpId"].ToString();

                        upadate = "Update timesheettemp set timesheettemp.Brdept = (select case when Bran.name = 'OtherBranch' then D.name when bran.name = 'HeadOffice' then D.name else bran.name end from Branches Bran join Employees E on E.Branch = Bran.Id join Departments D on E.Department = D.id where e.EmpId = " + emp_code + "), timesheettemp.Designation = (select D.name from Designations D join Employees E on E.currentDesignation = D.Id where E.empId = " + emp_code + ") where Designation = 'tbdtbd' and BrDept = 'tbdtbd'  and timesheettemp.EmpId=" + emp_code + ";";
                        sh.Run_UPDDEL_ExecuteNonQuery(upadate);



                    }

                    qrymanualmain1 = "Select * from timesheettemp; ";
                }
                return sh.Get_Table_FromQry(qrymanualmain1);
            }



            //return sh.Get_Table_FromQry(qry);
            //return sh.Get_Table_FromQry(qrymanualmain);
        }

        public DataTable EmployeeManualEntryDenied(string Empid, DateTime ReqFromDate)
        {
            string Reqfromdate1 = "";
            Reqfromdate1 = Convert.ToDateTime(ReqFromDate).ToString("yyyy-MM-dd");
            DataTable dtcheckdeniedemployees;
            string qrycheckmanualtimesheetform = "select e.empid,trf.userid,status from timesheet_request_form trf join employees e on trf.userid=e.id where e.Empid=" + Empid + " and trf.ReqFromDate='" + Reqfromdate1 + "' and trf.status='Denied'";
            dtcheckdeniedemployees = sh.Get_Table_FromQry(qrycheckmanualtimesheetform);
            return dtcheckdeniedemployees;
        }
        public DataTable EmpMonthlyLateTimesheet(string branch, string fromdate, string self, int EmpIds, string empcode)
        {
            string cond = "";
            string month = "";
            string year = "";
            string fulldate = "";
            if (branch == string.Empty && fromdate == string.Empty)
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
            }
            else if (branch != string.Empty && fromdate == string.Empty && branch != "All" && branch != "HeadOffice-All")
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')and  empid=" + EmpIds;
            }
            else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate != "" && self == "True")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                month = sa[0];
                year = sa[1];
                fulldate = " and ' " + month + "-" + year + "-" + 01 + "'  between sm.validfrom and sm.ValidTo ";
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')" + "and month = " + s2 + " and year = " + s1 + "and  empid=" + EmpIds;
            }

            else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate != "" && self == "False")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                month = sa[0];
                year = sa[1];
                fulldate = " and ' " + month + "-" + year + "-" + 01 + "'  between sm.validfrom and sm.ValidTo ";
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')" + "and month = " + s2 + " and year = " + s1;
            }
            else if (branch == "" && fromdate != "")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                month = sa[0];
                year = sa[1];
                fulldate = " and ' " + month + "-" + year + "-" + 01 + "'  between sm.validfrom and sm.ValidTo ";
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1;
            }
            else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate == "")
            {
                cond = "";
            }
            else if (branch != "" && branch == "All" && fromdate != "" && self == "False" && empcode == "")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                month = sa[0];
                year = sa[1];
                fulldate = " and ' " + month + "-" + year + "-" + 01 + "'  between sm.validfrom and sm.ValidTo ";
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1;
            }
            else if (branch != "" && branch == "All" && fromdate != "" && self == "False" && empcode != "")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                month = sa[0];
                year = sa[1];
                fulldate = " and ' " + month + "-" + year + "-" + 01 + "'  between sm.validfrom and sm.ValidTo ";
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1 + " and e.empid=" + empcode;
            }
            else if (branch != "" && branch == "All" && fromdate != "" && self == "True")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                month = sa[0];
                year = sa[1];
                fulldate = " and ' " + month + "-" + year + "-" + 01 + "'  between sm.validfrom and sm.ValidTo ";
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1 + "and empid=" + EmpIds;
            }
            else if (branch == "HeadOffice-All" && fromdate == string.Empty)
            {
                cond = " where e.Department != 46" + " and e.Branch = 43";
            }
            else if (branch == "HeadOffice-All" && fromdate != string.Empty)
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                month = sa[0];
                year = sa[1];
                fulldate = " and ' " + month + "-" + year + "-" + 01 + "'  between sm.validfrom and sm.ValidTo ";
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where e.Department != 46" + " and e.Branch = 43" + " and month = " + s2 + "and year = " + s1;
            }


            string qry = "select t.Id,t.userId as Empcode,des.code as Designation,e.ShortName as EmpName," +
   " case when b.name = 'OtherBranch' then d.name else b.name end as BrDept," +
   " Concat(CONVERT(varchar(3), DateName(MM,DATEADD(MONTH,t.Month,-1))),'-',t.Year) as Month," +
   " sm.InTime as BranchCheckInTime,sm.OutTime as BranchCheckOutTime," +
   " sum((case when D1_Status  like '%ED%'    then 1  else 0 end) +(case when D2_Status  like '%ED%'    then 1  else 0 end) +(case when D3_Status  like '%ED%'    then 1  else 0 end) + " +
" (case when D4_Status  like '%ED%'    then 1  else 0 end) + (case when D5_Status  like '%ED%'    then 1  else 0 end) + (case when D6_Status  like '%ED%'    then 1  else 0 end) + (case when D7_Status  like '%ED%'    then 1  else 0 end) + " +
" (case when D8_Status  like '%ED%'    then 1  else 0 end) + (case when D9_Status  like '%ED%'    then 1  else 0 end) + (case when D10_Status  like '%ED%'    then 1  else 0 end) + (case when D11_Status  like '%ED%'    then 1  else 0 end) + (case when D12_Status  like '%ED%'    then 1  else 0 end) + (case when D13_Status  like '%ED%'    then 1  else 0 end) + (case when D14_Status  like '%ED%'    then 1  else 0 end) + (case when D15_Status  like '%ED%'    then 1  else 0 end) + (case when D16_Status  like '%ED%'    then 1  else 0 end) + (case when D17_Status  like '%ED%'    then 1  else 0 end) + (case when D18_Status  like '%ED%'    then 1  else 0 end) + (case when D19_Status  like '%ED%'    then 1  else 0 end) + (case when D20_Status  like '%ED%'    then 1  else 0 end) + (case when D21_Status  like '%ED%'    then 1  else 0 end) + (case when D22_Status  like '%ED%'    then 1  else 0 end) + (case when D23_Status  like '%ED%'    then 1  else 0 end) + (case when D24_Status  like '%ED%'    then 1  else 0 end) + (case when D25_Status  like '%ED%'    then 1  else 0 end) + (case when D26_Status  like '%ED%'    then 1  else 0 end) + (case when D27_Status  like '%ED%'    then 1  else 0 end) + (case when D28_Status  like '%ED%'    then 1  else 0 end) + (case when D29_Status  like '%ED%'    then 1  else 0 end) + (case when D30_Status  like '%ED%'    then 1  else 0 end) + (case when D31_Status  like '%ED%'    then 1  else 0 end) ) as TotalEarlyDepartures " +
   " ,sum((case when D1_Status  like '%LA%'    then 1  else 0 end) +(case when D2_Status  like '%LA%'    then 1  else 0 end) +(case when D3_Status  like '%LA%'    then 1  else 0 end) + " +
   " (case when D4_Status  like '%LA%'    then 1  else 0 end) + (case when D5_Status  like '%LA%'    then 1  else 0 end) + (case when D6_Status  like '%LA%'    then 1  else 0 end) + (case when D7_Status  like '%LA%'    then 1  else 0 end) + " +
   " (case when D8_Status  like '%LA%'    then 1  else 0 end) + (case when D9_Status  like '%LA%'    then 1  else 0 end) + (case when D10_Status  like '%LA%'    then 1  else 0 end) + (case when D11_Status  like '%LA%'    then 1  else 0 end) + (case when D12_Status  like '%LA%'    then 1  else 0 end) + (case when D13_Status  like '%LA%'    then 1  else 0 end) + (case when D14_Status  like '%LA%'    then 1  else 0 end) + (case when D15_Status  like '%LA%'    then 1  else 0 end) + (case when D16_Status  like '%LA%'    then 1  else 0 end) + (case when D17_Status  like '%LA%'    then 1  else 0 end) + (case when D18_Status  like '%LA%'    then 1  else 0 end) + (case when D19_Status  like '%LA%'    then 1  else 0 end) + (case when D20_Status  like '%LA%'    then 1  else 0 end) + (case when D21_Status  like '%LA%'    then 1  else 0 end) + (case when D22_Status  like '%LA%'    then 1  else 0 end) + (case when D23_Status  like '%LA%'    then 1  else 0 end) + (case when D24_Status  like '%LA%'    then 1  else 0 end) + (case when D25_Status  like '%LA%'    then 1  else 0 end) + (case when D26_Status  like '%LA%'    then 1  else 0 end) + (case when D27_Status  like '%LA%'    then 1  else 0 end) + (case when D28_Status  like '%LA%'    then 1  else 0 end) + (case when D29_Status  like '%LA%'    then 1  else 0 end) + (case when D30_Status  like '%LA%'    then 1  else 0 end) + (case when D31_Status  like '%LA%'    then 1  else 0 end) ) as TotalLateComes, " +
  " concat((case when D1_Status like  '%ED%'     then '01' + '(' +  (SUBSTRING(t.d1, 1, 5)   )+( case when t.D1_meridian is not null then  subString(t.d1_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d1, 7, 5)   )+( case when t.D1_meridian is not null then  subString(t.d1_meridian,4,2) else '' end )+')'  end) , (case when D2_Status like  '%ED%'     then '<br/>' + '02' + '(' +    (SUBSTRING(t.d2, 1, 5)   )+( case when t.D2_meridian is not null then  subString(t.d2_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d2,7, 5)   )+ ( case when t.D2_meridian is not null then  subString(t.d2_meridian,4,2) else '' end )+')'   end) ,(case when D3_Status like  '%ED%'     then '<br/>' + '03' + '(' +    (SUBSTRING(t.d3, 1, 5)   ) +( case when t.D3_meridian is not null then  subString(t.d3_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d3,7, 5)   ) +( case when t.D3_meridian is not null then  subString(t.d3_meridian,4,2) else '' end ) +')'  end) ,  (case when D4_Status like  '%ED%'     then '<br/>' + '04' + '(' +    (SUBSTRING(t.d4, 1, 5)   ) +( case when t.D4_meridian is not null then  subString(t.d4_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d4, 7, 5)   ) + ( case when t.D4_meridian is not null then  subString(t.d4_meridian,4,2) else '' end )+')'   end),(case when D5_Status like  '%ED%'     then '<br/>' + '05' + '(' +    (SUBSTRING(t.d5, 1, 5)   ) + ( case when t.D5_meridian is not null then  subString(t.d5_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d5,7, 5)   ) + ( case when t.D5_meridian is not null then  subString(t.d5_meridian,4,2) else '' end )+')'   end), (case when D6_Status like  '%ED%'     then '<br/>' + '06' + '(' +    (SUBSTRING(t.d6, 1, 5)   ) +( case when t.D6_meridian is not null then  subString(t.d6_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d6, 7, 5)   ) + ( case when t.D6_meridian is not null then  subString(t.d6_meridian,4,2) else '' end )+')'   end),(case when D7_Status like  '%ED%'     then '<br/>' + '07' + '(' +    (SUBSTRING(t.d7, 1, 5)   ) + ( case when t.D7_meridian is not null then  subString(t.d7_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d7, 7, 5)   ) +( case when t.D7_meridian is not null then  subString(t.d7_meridian,4,2) else '' end )+')'   end), (case when D8_Status like  '%ED%'     then '<br/>' + '08' + '(' +    (SUBSTRING(t.d8, 1, 5)   ) + ( case when t.D8_meridian is not null then  subString(t.d8_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d8, 7, 5)   ) +( case when t.D8_meridian is not null then  subString(t.d8_meridian,4,2) else '' end )+')'  end),(case when D9_Status like  '%ED%'     then '<br/>' + '09' + '(' +    (SUBSTRING(t.d9, 1, 5)   ) +( case when t.D9_meridian is not null then  subString(t.d9_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d9, 7, 5)   ) +( case when t.D9_meridian is not null then  subString(t.d9_meridian,4,2) else '' end )+')'  end),(case when D10_Status like  '%ED%'     then '<br/>' + '10' + '(' +    (SUBSTRING(t.d10,1, 5)   ) +( case when t.D10_meridian is not null then  subString(t.d10_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d10, 7, 5)   ) + ( case when t.D10_meridian is not null then  subString(t.d10_meridian,4,2) else '' end ) +')'  end),(case when D11_Status like  '%ED%'     then '<br/>' + '11' + '(' +    (SUBSTRING(t.d11, 1, 5)   ) + ( case when t.D11_meridian is not null then  subString(t.d11_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d11,7, 5)   ) +( case when t.D11_meridian is not null then  subString(t.d11_meridian,4,2) else '' end )+')'   end),(case when D12_Status like  '%ED%'     then '<br/>' + '12' + '(' +    (SUBSTRING(t.d12, 1, 5)   ) +( case when t.D12_meridian is not null then  subString(t.d12_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d12, 7, 5)   ) + ( case when t.D12_meridian is not null then  subString(t.d12_meridian,4,2) else '' end )+')'  end),(case when D13_Status like  '%ED%'     then '<br/>' + '13' + '(' +    (SUBSTRING(t.d13, 1, 5)   ) +( case when t.D13_meridian is not null then  subString(t.d13_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d13, 7, 5)   ) + ( case when t.D13_meridian is not null then  subString(t.d13_meridian,4,2) else '' end )+')'  end),(case when D14_Status like  '%ED%'     then '<br/>' + '14' + '(' +    (SUBSTRING(t.d14, 1, 5)   ) +( case when t.D14_meridian is not null then  subString(t.d14_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d14, 7, 5)   ) +( case when t.D14_meridian is not null then  subString(t.d14_meridian,4,2) else '' end )+')'  end),(case when D15_Status like  '%ED%'     then '<br/>' + '15' + '(' +    (SUBSTRING(t.d15, 1, 5)   ) +( case when t.D15_meridian is not null then  subString(t.d15_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d15, 7, 5)   ) +( case when t.D15_meridian is not null then  subString(t.d15_meridian,4,2) else '' end ) +')' end),(case when D16_Status like  '%ED%'     then '<br/>' + '16' + '(' +    (SUBSTRING(t.d16, 1, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d16, 7, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,4,2) else '' end ) +')'  end),(case when D17_Status like  '%ED%'     then '<br/>' + '17' + '(' +    (SUBSTRING(t.d17, 1, 5)   ) +( case when t.D17_meridian is not null then  subString(t.d17_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d17, 7, 5)   ) +( case when t.D17_meridian is not null then  subString(t.d17_meridian,4,2) else '' end )+')'  end),(case when D18_Status like  '%ED%'     then '<br/>' + '18' + '(' +    (SUBSTRING(t.d18, 1, 5)   ) +( case when t.D18_meridian is not null then  subString(t.d18_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d18, 7, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,4,2) else '' end ) +')'  end), (case when D19_Status like  '%ED%'     then '<br/>' + '19' + '(' +    (SUBSTRING(t.d19, 1, 5)   ) +( case when t.D19_meridian is not null then  subString(t.d19_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d19, 7, 5)   ) +( case when t.D19_meridian is not null then  subString(t.d19_meridian,4,2) else '' end )+')'  end), (case when D20_Status like  '%ED%'     then '<br/>' + '20' + '(' +    (SUBSTRING(t.d20, 1, 5)   ) +( case when t.D20_meridian is not null then  subString(t.d20_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d20, 7, 5)   ) +( case when t.D20_meridian is not null then  subString(t.d20_meridian,4,2) else '' end )+')'  end), (case when D21_Status like  '%ED%'     then '<br/>' + '21' + '(' +    (SUBSTRING(t.d21, 1, 5)   ) +( case when t.D21_meridian is not null then  subString(t.d21_meridian,4,2) else '' end )+ '-' +    (SUBSTRING(t.d21, 7, 5)   ) +( case when t.D21_meridian is not null then  subString(t.d21_meridian,4,2) else '' end ) +')'  end), (case when D22_Status like  '%ED%'     then '<br/>' + '22' + '(' +    (SUBSTRING(t.d22, 1, 5)   ) +( case when t.D22_meridian is not null then  subString(t.d22_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d22, 7, 5)   )  +( case when t.D22_meridian is not null then  subString(t.d22_meridian,4,2) else '' end )+')' end), (case when D23_Status like  '%ED%'     then '<br/>' + '23' + '(' +    (SUBSTRING(t.d23, 1, 5)   ) +( case when t.D23_meridian is not null then  subString(t.d23_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d23, 7, 5)   ) +( case when t.D23_meridian is not null then  subString(t.d23_meridian,4,2) else '' end )+')'  end), (case when D24_Status like  '%ED%'     then '<br/>' + '24' + '(' +    (SUBSTRING(t.d24, 1, 5)   ) +( case when t.D24_meridian is not null then  subString(t.d24_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d24, 7, 5)   ) ++( case when t.D24_meridian is not null then  subString(t.d24_meridian,1,2) else '' end )+')'  end),(case when D25_Status like  '%ED%'     then '<br/>' + '25' + '(' +    (SUBSTRING(t.d25, 1, 5)   ) +( case when t.D25_meridian is not null then  subString(t.d25_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d25, 7, 5)   ) +( case when t.D25_meridian is not null then  subString(t.d25_meridian,4,2) else '' end )+')'  end),(case when D26_Status like  '%ED%'     then '<br/>' + '26' + '(' +    (SUBSTRING(t.d26, 1, 5)   ) +( case when t.D26_meridian is not null then  subString(t.d26_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d26, 7, 5)   ) +( case when t.D26_meridian is not null then  subString(t.d26_meridian,4,2) else '' end )+')'  end),(case when D27_Status like  '%ED%'     then '<br/>' + '27' + '(' +    (SUBSTRING(t.d27, 1, 5)   ) +( case when t.D27_meridian is not null then  subString(t.d27_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d27,7, 5)   ) +( case when t.D27_meridian is not null then  subString(t.d27_meridian,4,2) else '' end )+')'  end),(case when D28_Status like  '%ED%'     then '<br/>' + '28' + '(' +    (SUBSTRING(t.d28, 1, 5)   ) +( case when t.D28_meridian is not null then  subString(t.d28_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d28,7, 5)   )+( case when t.D28_meridian is not null then  subString(t.d28_meridian,4,2) else '' end ) +')'  end),(case when D29_Status like  '%ED%'     then '<br/>' + '29' + '(' +    (SUBSTRING(t.d29, 1, 5)   ) +( case when t.D29_meridian is not null then  subString(t.d29_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d29, 7, 5)   ) +( case when t.D29_meridian is not null then  subString(t.d29_meridian,4,2) else '' end )+')'  end),(case when D30_Status like  '%ED%'     then '<br/>' + '30' + '(' +    (SUBSTRING(t.d30, 1, 5)   )+( case when t.D30_meridian is not null then  subString(t.d30_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d30, 7, 5)   )+( case when t.D30_meridian is not null then  subString(t.d30_meridian,4,2) else '' end ) +')'  end),(case when D31_Status like  '%ED%'     then '<br/>' + '31' + '(' +    (SUBSTRING(t.d31, 1, 5)   ) +( case when t.D31_meridian is not null then  subString(t.d31_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d31, 7, 5)   ) +( case when t.D31_meridian is not null then  subString(t.d31_meridian,4,2) else '' end )+')'  end)) as EDIntimeOutTime" +
   " ,concat((case when D1_Status like  '%LA%'     then '01' + '(' +  (SUBSTRING(t.d1, 1, 5)   )+( case when t.D1_meridian is not null then  subString(t.d1_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d1, 7, 5)   )+( case when t.D1_meridian is not null then  subString(t.d1_meridian,4,2) else '' end )+')'  end) , (case when D2_Status like  '%LA%'     then '<br/>' + '02' + '(' +    (SUBSTRING(t.d2, 1, 5)   )+( case when t.D2_meridian is not null then  subString(t.d2_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d2,7, 5)   )+ ( case when t.D2_meridian is not null then  subString(t.d2_meridian,4,2) else '' end )+')'   end) ,(case when D3_Status like  '%LA%'     then '<br/>' + '03' + '(' +    (SUBSTRING(t.d3, 1, 5)   ) +( case when t.D3_meridian is not null then  subString(t.d3_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d3,7, 5)   ) +( case when t.D3_meridian is not null then  subString(t.d3_meridian,4,2) else '' end ) +')'  end) ,  (case when D4_Status like  '%LA%'     then '<br/>' + '04' + '(' +    (SUBSTRING(t.d4, 1, 5)   ) +( case when t.D4_meridian is not null then  subString(t.d4_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d4, 7, 5)   ) + ( case when t.D4_meridian is not null then  subString(t.d4_meridian,4,2) else '' end )+')'   end),(case when D5_Status like  '%LA%'     then '<br/>' + '05' + '(' +    (SUBSTRING(t.d5, 1, 5)   ) + ( case when t.D5_meridian is not null then  subString(t.d5_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d5,7, 5)   ) + ( case when t.D5_meridian is not null then  subString(t.d5_meridian,4,2) else '' end )+')'   end), (case when D6_Status like  '%LA%'     then '<br/>' + '06' + '(' +    (SUBSTRING(t.d6, 1, 5)   ) +( case when t.D6_meridian is not null then  subString(t.d6_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d6, 7, 5)   ) + ( case when t.D6_meridian is not null then  subString(t.d6_meridian,4,2) else '' end )+')'   end),(case when D7_Status like  '%LA%'     then '<br/>' + '07' + '(' +    (SUBSTRING(t.d7, 1, 5)   ) + ( case when t.D7_meridian is not null then  subString(t.d7_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d7, 7, 5)   ) +( case when t.D7_meridian is not null then  subString(t.d7_meridian,4,2) else '' end )+')'   end), (case when D8_Status like  '%LA%'     then '<br/>' + '08' + '(' +    (SUBSTRING(t.d8, 1, 5)   ) + ( case when t.D8_meridian is not null then  subString(t.d8_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d8, 7, 5)   ) +( case when t.D8_meridian is not null then  subString(t.d8_meridian,4,2) else '' end )+')'  end),(case when D9_Status like  '%LA%'     then '<br/>' + '09' + '(' +    (SUBSTRING(t.d9, 1, 5)   ) +( case when t.D9_meridian is not null then  subString(t.d9_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d9, 7, 5)   ) +( case when t.D9_meridian is not null then  subString(t.d9_meridian,4,2) else '' end )+')'  end),(case when D10_Status like  '%LA%'     then '<br/>' + '10' + '(' +    (SUBSTRING(t.d10,1, 5)   ) +( case when t.D10_meridian is not null then  subString(t.d10_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d10, 7, 5)   ) + ( case when t.D10_meridian is not null then  subString(t.d10_meridian,4,2) else '' end ) +')'  end),(case when D11_Status like  '%LA%'     then '<br/>' + '11' + '(' +    (SUBSTRING(t.d11, 1, 5)   ) + ( case when t.D11_meridian is not null then  subString(t.d11_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d11,7, 5)   ) +( case when t.D11_meridian is not null then  subString(t.d11_meridian,4,2) else '' end )+')'   end),(case when D12_Status like  '%LA%'     then '<br/>' + '12' + '(' +    (SUBSTRING(t.d12, 1, 5)   ) +( case when t.D12_meridian is not null then  subString(t.d12_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d12, 7, 5)   ) + ( case when t.D12_meridian is not null then  subString(t.d12_meridian,4,2) else '' end )+')'  end),(case when D13_Status like  '%LA%'     then '<br/>' + '13' + '(' +    (SUBSTRING(t.d13, 1, 5)   ) +( case when t.D13_meridian is not null then  subString(t.d13_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d13, 7, 5)   ) + ( case when t.D13_meridian is not null then  subString(t.d13_meridian,4,2) else '' end )+')'  end),(case when D14_Status like  '%LA%'     then '<br/>' + '14' + '(' +    (SUBSTRING(t.d14, 1, 5)   ) +( case when t.D14_meridian is not null then  subString(t.d14_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d14, 7, 5)   ) +( case when t.D14_meridian is not null then  subString(t.d14_meridian,4,2) else '' end )+')'  end),(case when D15_Status like  '%LA%'     then '<br/>' + '15' + '(' +    (SUBSTRING(t.d15, 1, 5)   ) +( case when t.D15_meridian is not null then  subString(t.d15_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d15, 7, 5)   ) +( case when t.D15_meridian is not null then  subString(t.d15_meridian,4,2) else '' end ) +')' end),(case when D16_Status like  '%LA%'     then '<br/>' + '16' + '(' +    (SUBSTRING(t.d16, 1, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d16, 7, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,4,2) else '' end ) +')'  end),(case when D17_Status like  '%LA%'     then '<br/>' + '17' + '(' +    (SUBSTRING(t.d17, 1, 5)   ) +( case when t.D17_meridian is not null then  subString(t.d17_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d17, 7, 5)   ) +( case when t.D17_meridian is not null then  subString(t.d17_meridian,4,2) else '' end )+')'  end),(case when D18_Status like  '%LA%'     then '<br/>' + '18' + '(' +    (SUBSTRING(t.d18, 1, 5)   ) +( case when t.D18_meridian is not null then  subString(t.d18_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d18, 7, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,4,2) else '' end ) +')'  end), (case when D19_Status like  '%LA%'     then '<br/>' + '19' + '(' +    (SUBSTRING(t.d19, 1, 5)   ) +( case when t.D19_meridian is not null then  subString(t.d19_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d19, 7, 5)   ) +( case when t.D19_meridian is not null then  subString(t.d19_meridian,4,2) else '' end )+')'  end), (case when D20_Status like  '%LA%'     then '<br/>' + '20' + '(' +    (SUBSTRING(t.d20, 1, 5)   ) +( case when t.D20_meridian is not null then  subString(t.d20_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d20, 7, 5)   ) +( case when t.D20_meridian is not null then  subString(t.d20_meridian,4,2) else '' end )+')'  end), (case when D21_Status like  '%LA%'     then '<br/>' + '21' + '(' +    (SUBSTRING(t.d21, 1, 5)   ) +( case when t.D21_meridian is not null then  subString(t.d21_meridian,4,2) else '' end )+ '-' +    (SUBSTRING(t.d21, 7, 5)   ) +( case when t.D21_meridian is not null then  subString(t.d21_meridian,4,2) else '' end ) +')'  end), (case when D22_Status like  '%LA%'     then '<br/>' + '22' + '(' +    (SUBSTRING(t.d22, 1, 5)   ) +( case when t.D22_meridian is not null then  subString(t.d22_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d22, 7, 5)   )  +( case when t.D22_meridian is not null then  subString(t.d22_meridian,4,2) else '' end )+')' end), (case when D23_Status like  '%LA%'     then '<br/>' + '23' + '(' +    (SUBSTRING(t.d23, 1, 5)   ) +( case when t.D23_meridian is not null then  subString(t.d23_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d23, 7, 5)   ) +( case when t.D23_meridian is not null then  subString(t.d23_meridian,4,2) else '' end )+')'  end), (case when D24_Status like  '%LA%'     then '<br/>' + '24' + '(' +    (SUBSTRING(t.d24, 1, 5)   ) +( case when t.D24_meridian is not null then  subString(t.d24_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d24, 7, 5)   ) ++( case when t.D24_meridian is not null then  subString(t.d24_meridian,1,2) else '' end )+')'  end),(case when D25_Status like  '%LA%'     then '<br/>' + '25' + '(' +    (SUBSTRING(t.d25, 1, 5)   ) +( case when t.D25_meridian is not null then  subString(t.d25_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d25, 7, 5)   ) +( case when t.D25_meridian is not null then  subString(t.d25_meridian,4,2) else '' end )+')'  end),(case when D26_Status like  '%LA%'     then '<br/>' + '26' + '(' +    (SUBSTRING(t.d26, 1, 5)   ) +( case when t.D26_meridian is not null then  subString(t.d26_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d26, 7, 5)   ) +( case when t.D26_meridian is not null then  subString(t.d26_meridian,4,2) else '' end )+')'  end),(case when D27_Status like  '%LA%'     then '<br/>' + '27' + '(' +    (SUBSTRING(t.d27, 1, 5)   ) +( case when t.D27_meridian is not null then  subString(t.d27_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d27,7, 5)   ) +( case when t.D27_meridian is not null then  subString(t.d27_meridian,4,2) else '' end )+')'  end),(case when D28_Status like  '%LA%'     then '<br/>' + '28' + '(' +    (SUBSTRING(t.d28, 1, 5)   ) +( case when t.D28_meridian is not null then  subString(t.d28_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d28,7, 5)   )+( case when t.D28_meridian is not null then  subString(t.d28_meridian,4,2) else '' end ) +')'  end),(case when D29_Status like  '%LA%'     then '<br/>' + '29' + '(' +    (SUBSTRING(t.d29, 1, 5)   ) +( case when t.D29_meridian is not null then  subString(t.d29_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d29, 7, 5)   ) +( case when t.D29_meridian is not null then  subString(t.d29_meridian,4,2) else '' end )+')'  end),(case when D30_Status like  '%LA%'     then '<br/>' + '30' + '(' +    (SUBSTRING(t.d30, 1, 5)   )+( case when t.D30_meridian is not null then  subString(t.d30_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d30, 7, 5)   )+( case when t.D30_meridian is not null then  subString(t.d30_meridian,4,2) else '' end ) +')'  end),(case when D31_Status like  '%LA%'     then '<br/>' + '31' + '(' +    (SUBSTRING(t.d31, 1, 5)   ) +( case when t.D31_meridian is not null then  subString(t.d31_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d31, 7, 5)   ) +( case when t.D31_meridian is not null then  subString(t.d31_meridian,4,2) else '' end )+')'  end)) as LAIntimeOutTime from timesheet_Emp_Month t" +
   " join Employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id join Departments d on e.Department = d.id join shift_master sm on sm.branchid = e.branch " +
   " " + cond + "" +
   "   " + fulldate + " " + //between sm.validfrom and sm.ValidTo" +
   " and sm.ShiftType =(case when (select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end) " +
   //'2021-06-01'

   " and e.EmpId!=1 and (D1_Status   like  '%LA%' or  D1_Status   like  '%ED%'  or   D2_Status  like  '%LA%' or   D2_Status  like  '%ED%'  or  D3_Status  like  '%LA%' or  D3_Status  like  '%ED%'  or  D4_Status  like  '%LA%' or  D4_Status  like  '%ED%'  or  D5_Status  like  '%LA%' or  D5_Status  like  '%ED%'   or  D6_Status  like  '%LA%'  or  D7_Status  like  '%LA%'  or  D8_Status  like  '%LA%'  or  D9_Status  like  '%LA%'  or  D10_Status  like  '%LA%'  or  D11_Status  like  '%LA%'  or  D12_Status  like  '%LA%'  or  D13_Status  like  '%LA%'  or  D14_Status  like  '%LA%'  or  D15_Status  like  '%LA%'  or  D16_Status  like  '%LA%'  or  D17_Status  like  '%LA%'  or  D18_Status  like  '%LA%'  or  D19_Status  like  '%LA%'  or  D20_Status  like  '%LA%'  or  D21_Status  like  '%LA%'  or  D22_Status  like  '%LA%'  or  D23_Status  like  '%LA%'  or  D24_Status  like  '%LA%'  or  D25_Status  like  '%LA%'  or  D26_Status  like  '%LA%'  or  D27_Status  like  '%LA%'  or  D28_Status  like  '%LA%'  or  D29_Status  like  '%LA%'  or  D30_Status  like  '%LA%'  or  D31_Status like  '%LA%' or  D6_Status  like  '%ED%'  or  D7_Status  like  '%ED%'  or  D8_Status  like  '%ED%'  or  D9_Status  like  '%ED%'  or  D10_Status  like  '%ED%'  or  D11_Status  like  '%ED%'  or  D12_Status  like  '%ED%'  or  D13_Status  like  '%ED%'  or  D14_Status  like  '%ED%'  or  D15_Status  like  '%ED%'  or  D16_Status  like  '%ED%'  or  D17_Status  like  '%ED%'  or  D18_Status  like  '%ED%'  or  D19_Status  like  '%ED%'  or  D20_Status  like  '%ED%'  or  D21_Status  like  '%ED%'  or  D22_Status  like  '%ED%'  or  D23_Status  like  '%ED%'  or  D24_Status  like  '%ED%'  or  D25_Status  like  '%ED%'  or  D26_Status  like  '%ED%'  or  D27_Status  like  '%ED%'  or  D28_Status  like  '%ED%'  or  D29_Status  like  '%ED%'  or  D30_Status  like  '%ED%'  or  D31_Status like  '%ED%')" +
   " GROUP BY D1_Status,D2_Status,D3_Status,D4_Status,D5_Status,D6_Status,D7_Status,D8_Status,D9_Status,D10_Status,D11_Status,D12_Status,D13_Status,D14_Status,D15_Status,D16_Status,D17_Status,D18_Status,D19_Status,D20_Status,D21_Status,D22_Status,D23_Status,D24_Status,D25_Status,D26_Status,D27_Status,D28_Status,D29_Status,D30_Status,D31_Status,UserId,month,year,ShortName,b.Name,d.Name,des.code,b.StartTime,b.EndTime,sm.InTime,sm.OutTime,t.D1,t.D2 ,t.D3,t.D4,t.D5,t.D6,t.D7,t.D8,t.D9,t.D10,t.D11,t.D12,t.D13,t.D14,t.D15,t.D16,t.D17,t.D18,t.D19,t.D19,t.D20,t.D21,t.D22,t.D23,t.D24,t.D25,t.D26,t.D27,t.D28,t.D29,t.D30,t.D31,t.Id,t.D1_meridian,t.D2_meridian,t.D3_meridian,t.D4_meridian,t.D5_meridian,t.D6_meridian,t.D7_meridian,t.D8_meridian,t.D9_meridian,t.D10_meridian,t.D11_meridian,t.D12_meridian,t.D13_meridian,t.D14_meridian,t.D15_meridian,t.D16_meridian,t.D17_meridian,t.D18_meridian,t.D19_meridian,t.D20_meridian,t.D21_meridian,t.D22_meridian,t.D23_meridian,t.D24_meridian,t.D25_meridian,t.D26_meridian,t.D27_meridian,t.D28_meridian,t.D29_meridian,t.D30_meridian,t.D31_meridian";


            return sh.Get_Table_FromQry(qry);
        }
        public DataTable EmpMonthlyLateMemoTimesheet(string branch, string fromdate, string self, int EmpIds, string empcode)
        {
            string cond = "";
            string S4 = "";
            if (branch == string.Empty && fromdate == string.Empty)
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
            }
            else if (branch != string.Empty && fromdate == string.Empty && branch != "All" && branch != "HeadOffice-All")
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')and  empid=" + EmpIds;
            }
            else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate != "" && self == "True")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')" + "and month = " + s2 + " and year = " + s1 + " and  empid=" + EmpIds;
            }

            else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate != "" && self == "False")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                S4 = sa[0] + "-" + sa[1] + "-" + "01";
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')" + "and month = " + s2 + " and year = " + s1;
            }
            else if (branch == "" && fromdate != "")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1;
            }
            else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate == "")
            {
                cond = "";
            }
            else if (branch != "" && branch == "All" && fromdate != "" && self == "False" && empcode == "")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                S4 = sa[0] + "-" + sa[1] + "-" + "01";
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1;
            }
            else if (branch != "" && branch == "All" && fromdate != "" && self == "False" && empcode != "")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                S4 = sa[0] + "-" + sa[1] + "-" + "01";
                cond = " where month=" + s2 + "and year =" + s1 + " and e.empid=" + empcode;
            }
            else if (branch != "" && branch == "All" && fromdate != "" && self == "True")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1 + "and empid=" + EmpIds;
            }
            else if (branch == "HeadOffice-All" && fromdate == string.Empty)
            {
                cond = " where e.Department != 46" + " and e.Branch = 43";
            }
            else if (branch == "HeadOffice-All" && fromdate != string.Empty)
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                S4 = sa[0] + "-" + sa[1] + "-" + "01";
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where e.Department != 46" + " and e.Branch = 43" + " and month = " + s2 + "and year = " + s1;
            }


            string qry = "select t.Id,t.userId as Empcode,des.code as Designation,e.ShortName as EmpName," +
   " case when b.name = 'OtherBranch' then d.name else b.name end as BrDept," +
   " Concat(CONVERT(varchar(3), DateName(MM,DATEADD(MONTH,t.Month,-1))),'-',t.Year) as Month," +
   " sm.InTime+ ' - ' +sm.OutTime as BranchTimings,sm.OutTime as BranchCheckOutTime," +
   " sum((case when D1_Status  like '%ED%'    then 1  else 0 end) +(case when D2_Status  like '%ED%'    then 1  else 0 end) +(case when D3_Status  like '%ED%'    then 1  else 0 end) + " +
" (case when D4_Status  like '%ED%'    then 1  else 0 end) + (case when D5_Status  like '%ED%'    then 1  else 0 end) + (case when D6_Status  like '%ED%'    then 1  else 0 end) + (case when D7_Status  like '%ED%'    then 1  else 0 end) + " +
" (case when D8_Status  like '%ED%'    then 1  else 0 end) + (case when D9_Status  like '%ED%'    then 1  else 0 end) + (case when D10_Status  like '%ED%'    then 1  else 0 end) + (case when D11_Status  like '%ED%'    then 1  else 0 end) + (case when D12_Status  like '%ED%'    then 1  else 0 end) + (case when D13_Status  like '%ED%'    then 1  else 0 end) + (case when D14_Status  like '%ED%'    then 1  else 0 end) + (case when D15_Status  like '%ED%'    then 1  else 0 end) + (case when D16_Status  like '%ED%'    then 1  else 0 end) + (case when D17_Status  like '%ED%'    then 1  else 0 end) + (case when D18_Status  like '%ED%'    then 1  else 0 end) + (case when D19_Status  like '%ED%'    then 1  else 0 end) + (case when D20_Status  like '%ED%'    then 1  else 0 end) + (case when D21_Status  like '%ED%'    then 1  else 0 end) + (case when D22_Status  like '%ED%'    then 1  else 0 end) + (case when D23_Status  like '%ED%'    then 1  else 0 end) + (case when D24_Status  like '%ED%'    then 1  else 0 end) + (case when D25_Status  like '%ED%'    then 1  else 0 end) + (case when D26_Status  like '%ED%'    then 1  else 0 end) + (case when D27_Status  like '%ED%'    then 1  else 0 end) + (case when D28_Status  like '%ED%'    then 1  else 0 end) + (case when D29_Status  like '%ED%'    then 1  else 0 end) + (case when D30_Status  like '%ED%'    then 1  else 0 end) + (case when D31_Status  like '%ED%'    then 1  else 0 end) ) as TotalEarlyDepartures " +
   " ,sum((case when D1_Status  like '%LA%'    then 1  else 0 end) +(case when D2_Status  like '%LA%'    then 1  else 0 end) +(case when D3_Status  like '%LA%'    then 1  else 0 end) + " +
   " (case when D4_Status  like '%LA%'    then 1  else 0 end) + (case when D5_Status  like '%LA%'    then 1  else 0 end) + (case when D6_Status  like '%LA%'    then 1  else 0 end) + (case when D7_Status  like '%LA%'    then 1  else 0 end) + " +
   " (case when D8_Status  like '%LA%'    then 1  else 0 end) + (case when D9_Status  like '%LA%'    then 1  else 0 end) + (case when D10_Status  like '%LA%'    then 1  else 0 end) + (case when D11_Status  like '%LA%'    then 1  else 0 end) + (case when D12_Status  like '%LA%'    then 1  else 0 end) + (case when D13_Status  like '%LA%'    then 1  else 0 end) + (case when D14_Status  like '%LA%'    then 1  else 0 end) + (case when D15_Status  like '%LA%'    then 1  else 0 end) + (case when D16_Status  like '%LA%'    then 1  else 0 end) + (case when D17_Status  like '%LA%'    then 1  else 0 end) + (case when D18_Status  like '%LA%'    then 1  else 0 end) + (case when D19_Status  like '%LA%'    then 1  else 0 end) + (case when D20_Status  like '%LA%'    then 1  else 0 end) + (case when D21_Status  like '%LA%'    then 1  else 0 end) + (case when D22_Status  like '%LA%'    then 1  else 0 end) + (case when D23_Status  like '%LA%'    then 1  else 0 end) + (case when D24_Status  like '%LA%'    then 1  else 0 end) + (case when D25_Status  like '%LA%'    then 1  else 0 end) + (case when D26_Status  like '%LA%'    then 1  else 0 end) + (case when D27_Status  like '%LA%'    then 1  else 0 end) + (case when D28_Status  like '%LA%'    then 1  else 0 end) + (case when D29_Status  like '%LA%'    then 1  else 0 end) + (case when D30_Status  like '%LA%'    then 1  else 0 end) + (case when D31_Status  like '%LA%'    then 1  else 0 end) ) as TotalLateComes, " +
   " concat((case when D1_Status like  '%LA%'     then '01' + '(' +  (SUBSTRING(t.d1, 1, 5)   )+( case when t.D1_meridian is not null then  subString(t.d1_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d1, 7, 5)   )+( case when t.D1_meridian is not null then  subString(t.d1_meridian,4,2) else '' end )+')'  end) , (case when D2_Status like  '%LA%'     then '<br/>' + '02' + '(' +    (SUBSTRING(t.d2, 1, 5)   )+( case when t.D2_meridian is not null then  subString(t.d2_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d2,7, 5)   )+ ( case when t.D2_meridian is not null then  subString(t.d2_meridian,4,2) else '' end )+')'   end) ,(case when D3_Status like  '%LA%'     then '<br/>' + '03' + '(' +    (SUBSTRING(t.d3, 1, 5)   ) +( case when t.D3_meridian is not null then  subString(t.d3_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d3,7, 5)   ) +( case when t.D3_meridian is not null then  subString(t.d3_meridian,4,2) else '' end ) +')'  end) ,  (case when D4_Status like  '%LA%'     then '<br/>' + '04' + '(' +    (SUBSTRING(t.d4, 1, 5)   ) +( case when t.D4_meridian is not null then  subString(t.d4_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d4, 7, 5)   ) + ( case when t.D4_meridian is not null then  subString(t.d4_meridian,4,2) else '' end )+')'   end),(case when D5_Status like  '%LA%'     then '<br/>' + '05' + '(' +    (SUBSTRING(t.d5, 1, 5)   ) + ( case when t.D5_meridian is not null then  subString(t.d5_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d5,7, 5)   ) + ( case when t.D5_meridian is not null then  subString(t.d5_meridian,4,2) else '' end )+')'   end), (case when D6_Status like  '%LA%'     then '<br/>' + '06' + '(' +    (SUBSTRING(t.d6, 1, 5)   ) +( case when t.D6_meridian is not null then  subString(t.d6_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d6, 7, 5)   ) + ( case when t.D6_meridian is not null then  subString(t.d6_meridian,4,2) else '' end )+')'   end),(case when D7_Status like  '%LA%'     then '<br/>' + '07' + '(' +    (SUBSTRING(t.d7, 1, 5)   ) + ( case when t.D7_meridian is not null then  subString(t.d7_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d7, 7, 5)   ) +( case when t.D7_meridian is not null then  subString(t.d7_meridian,4,2) else '' end )+')'   end), (case when D8_Status like  '%LA%'     then '<br/>' + '08' + '(' +    (SUBSTRING(t.d8, 1, 5)   ) + ( case when t.D8_meridian is not null then  subString(t.d8_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d8, 7, 5)   ) +( case when t.D8_meridian is not null then  subString(t.d8_meridian,4,2) else '' end )+')'  end),(case when D9_Status like  '%LA%'     then '<br/>' + '09' + '(' +    (SUBSTRING(t.d9, 1, 5)   ) +( case when t.D9_meridian is not null then  subString(t.d9_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d9, 7, 5)   ) +( case when t.D9_meridian is not null then  subString(t.d9_meridian,4,2) else '' end )+')'  end),(case when D10_Status like  '%LA%'     then '<br/>' + '10' + '(' +    (SUBSTRING(t.d10,1, 5)   ) +( case when t.D10_meridian is not null then  subString(t.d10_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d10, 7, 5)   ) + ( case when t.D10_meridian is not null then  subString(t.d10_meridian,4,2) else '' end ) +')'  end),(case when D11_Status like  '%LA%'     then '<br/>' + '11' + '(' +    (SUBSTRING(t.d11, 1, 5)   ) + ( case when t.D11_meridian is not null then  subString(t.d11_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d11,7, 5)   ) +( case when t.D11_meridian is not null then  subString(t.d11_meridian,4,2) else '' end )+')'   end),(case when D12_Status like  '%LA%'     then '<br/>' + '12' + '(' +    (SUBSTRING(t.d12, 1, 5)   ) +( case when t.D12_meridian is not null then  subString(t.d12_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d12, 7, 5)   ) + ( case when t.D12_meridian is not null then  subString(t.d12_meridian,4,2) else '' end )+')'  end),(case when D13_Status like  '%LA%'     then '<br/>' + '13' + '(' +    (SUBSTRING(t.d13, 1, 5)   ) +( case when t.D13_meridian is not null then  subString(t.d13_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d13, 7, 5)   ) + ( case when t.D13_meridian is not null then  subString(t.d13_meridian,4,2) else '' end )+')'  end),(case when D14_Status like  '%LA%'     then '<br/>' + '14' + '(' +    (SUBSTRING(t.d14, 1, 5)   ) +( case when t.D14_meridian is not null then  subString(t.d14_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d14, 7, 5)   ) +( case when t.D14_meridian is not null then  subString(t.d14_meridian,4,2) else '' end )+')'  end),(case when D15_Status like  '%LA%'     then '<br/>' + '15' + '(' +    (SUBSTRING(t.d15, 1, 5)   ) +( case when t.D15_meridian is not null then  subString(t.d15_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d15, 7, 5)   ) +( case when t.D15_meridian is not null then  subString(t.d15_meridian,4,2) else '' end ) +')' end),(case when D16_Status like  '%LA%'     then '<br/>' + '16' + '(' +    (SUBSTRING(t.d16, 1, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d16, 7, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,4,2) else '' end ) +')'  end),(case when D17_Status like  '%LA%'     then '<br/>' + '17' + '(' +    (SUBSTRING(t.d17, 1, 5)   ) +( case when t.D17_meridian is not null then  subString(t.d17_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d17, 7, 5)   ) +( case when t.D17_meridian is not null then  subString(t.d17_meridian,4,2) else '' end )+')'  end),(case when D18_Status like  '%LA%'     then '<br/>' + '18' + '(' +    (SUBSTRING(t.d18, 1, 5)   ) +( case when t.D18_meridian is not null then  subString(t.d18_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d18, 7, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,4,2) else '' end ) +')'  end), (case when D19_Status like  '%LA%'     then '<br/>' + '19' + '(' +    (SUBSTRING(t.d19, 1, 5)   ) +( case when t.D19_meridian is not null then  subString(t.d19_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d19, 7, 5)   ) +( case when t.D19_meridian is not null then  subString(t.d19_meridian,4,2) else '' end )+')'  end), (case when D20_Status like  '%LA%'     then '<br/>' + '20' + '(' +    (SUBSTRING(t.d20, 1, 5)   ) +( case when t.D20_meridian is not null then  subString(t.d20_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d20, 7, 5)   ) +( case when t.D20_meridian is not null then  subString(t.d20_meridian,4,2) else '' end )+')'  end), (case when D21_Status like  '%LA%'     then '<br/>' + '21' + '(' +    (SUBSTRING(t.d21, 1, 5)   ) +( case when t.D21_meridian is not null then  subString(t.d21_meridian,4,2) else '' end )+ '-' +    (SUBSTRING(t.d21, 7, 5)   ) +( case when t.D21_meridian is not null then  subString(t.d21_meridian,4,2) else '' end ) +')'  end), (case when D22_Status like  '%LA%'     then '<br/>' + '22' + '(' +    (SUBSTRING(t.d22, 1, 5)   ) +( case when t.D22_meridian is not null then  subString(t.d22_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d22, 7, 5)   )  +( case when t.D22_meridian is not null then  subString(t.d22_meridian,4,2) else '' end )+')' end), (case when D23_Status like  '%LA%'     then '<br/>' + '23' + '(' +    (SUBSTRING(t.d23, 1, 5)   ) +( case when t.D23_meridian is not null then  subString(t.d23_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d23, 7, 5)   ) +( case when t.D23_meridian is not null then  subString(t.d23_meridian,4,2) else '' end )+')'  end), (case when D24_Status like  '%LA%'     then '<br/>' + '24' + '(' +    (SUBSTRING(t.d24, 1, 5)   ) +( case when t.D24_meridian is not null then  subString(t.d24_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d24, 7, 5)   ) ++( case when t.D24_meridian is not null then  subString(t.d24_meridian,1,2) else '' end )+')'  end),(case when D25_Status like  '%LA%'     then '<br/>' + '25' + '(' +    (SUBSTRING(t.d25, 1, 5)   ) +( case when t.D25_meridian is not null then  subString(t.d25_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d25, 7, 5)   ) +( case when t.D25_meridian is not null then  subString(t.d25_meridian,4,2) else '' end )+')'  end),(case when D26_Status like  '%LA%'     then '<br/>' + '26' + '(' +    (SUBSTRING(t.d26, 1, 5)   ) +( case when t.D26_meridian is not null then  subString(t.d26_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d26, 7, 5)   ) +( case when t.D26_meridian is not null then  subString(t.d26_meridian,4,2) else '' end )+')'  end),(case when D27_Status like  '%LA%'     then '<br/>' + '27' + '(' +    (SUBSTRING(t.d27, 1, 5)   ) +( case when t.D27_meridian is not null then  subString(t.d27_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d27,7, 5)   ) +( case when t.D27_meridian is not null then  subString(t.d27_meridian,4,2) else '' end )+')'  end),(case when D28_Status like  '%LA%'     then '<br/>' + '28' + '(' +    (SUBSTRING(t.d28, 1, 5)   ) +( case when t.D28_meridian is not null then  subString(t.d28_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d28,7, 5)   )+( case when t.D28_meridian is not null then  subString(t.d28_meridian,4,2) else '' end ) +')'  end),(case when D29_Status like  '%LA%'     then '<br/>' + '29' + '(' +    (SUBSTRING(t.d29, 1, 5)   ) +( case when t.D29_meridian is not null then  subString(t.d29_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d29, 7, 5)   ) +( case when t.D29_meridian is not null then  subString(t.d29_meridian,4,2) else '' end )+')'  end),(case when D30_Status like  '%LA%'     then '<br/>' + '30' + '(' +    (SUBSTRING(t.d30, 1, 5)   )+( case when t.D30_meridian is not null then  subString(t.d30_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d30, 7, 5)   )+( case when t.D30_meridian is not null then  subString(t.d30_meridian,4,2) else '' end ) +')'  end),(case when D31_Status like  '%LA%'     then '<br/>' + '31' + '(' +    (SUBSTRING(t.d31, 1, 5)   ) +( case when t.D31_meridian is not null then  subString(t.d31_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d31, 7, 5)   ) +( case when t.D31_meridian is not null then  subString(t.d31_meridian,4,2) else '' end )+')'  end)) as IntimeOutTime from timesheet_Emp_Month t" +
   " join Employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id join Departments d on e.Department = d.id join shift_master sm on sm.Branchid = e.Branch " +
   " " + cond + " and e.empid!=1  and sm.shifttype = (case when(select count(*) from designations ds where ds.name like 'Attender%' and ds.id = des.id) <> 0 then '1'  else '0' end ) and '" + S4 + "'  between sm.validfrom and sm.validto and (D1_Status   like  '%LA%'  or   D2_Status  like  '%LA%'  or  D3_Status  like  '%LA%'  or  D4_Status  like  '%LA%'  or  D5_Status  like  '%LA%'  or  D6_Status  like  '%LA%'  or  D7_Status  like  '%LA%'  or  D8_Status  like  '%LA%'  or  D9_Status  like  '%LA%'  or  D10_Status  like  '%LA%'  or  D11_Status  like  '%LA%'  or  D12_Status  like  '%LA%'  or  D13_Status  like  '%LA%'  or  D14_Status  like  '%LA%'  or  D15_Status  like  '%LA%'  or  D16_Status  like  '%LA%'  or  D17_Status  like  '%LA%'  or  D18_Status  like  '%LA%'  or  D19_Status  like  '%LA%'  or  D20_Status  like  '%LA%'  or  D21_Status  like  '%LA%'  or  D22_Status  like  '%LA%'  or  D23_Status  like  '%LA%'  or  D24_Status  like  '%LA%'  or  D25_Status  like  '%LA%'  or  D26_Status  like  '%LA%'  or  D27_Status  like  '%LA%'  or  D28_Status  like  '%LA%'  or  D29_Status  like  '%LA%'  or  D30_Status  like  '%LA%'  or  D31_Status like  '%LA%')" +
   " GROUP BY D1_Status,D2_Status,D3_Status,D4_Status,D5_Status,D6_Status,D7_Status,D8_Status,D9_Status,D10_Status,D11_Status,D12_Status,D13_Status,D14_Status,D15_Status,D16_Status,D17_Status,D18_Status,D19_Status,D20_Status,D21_Status,D22_Status,D23_Status,D24_Status,D25_Status,D26_Status,D27_Status,D28_Status,D29_Status,D30_Status,D31_Status,UserId,month,year,ShortName,b.Name,d.Name,des.code,b.StartTime,b.EndTime,sm.InTime,sm.OutTime,t.D1,t.D2 ,t.D3,t.D4,t.D5,t.D6,t.D7,t.D8,t.D9,t.D10,t.D11,t.D12,t.D13,t.D14,t.D15,t.D16,t.D17,t.D18,t.D19,t.D19,t.D20,t.D21,t.D22,t.D23,t.D24,t.D25,t.D26,t.D27,t.D28,t.D29,t.D30,t.D31,t.Id,t.D1_meridian,t.D2_meridian,t.D3_meridian,t.D4_meridian,t.D5_meridian,t.D6_meridian,t.D7_meridian,t.D8_meridian,t.D9_meridian,t.D10_meridian,t.D11_meridian,t.D12_meridian,t.D13_meridian,t.D14_meridian,t.D15_meridian,t.D16_meridian,t.D17_meridian,t.D18_meridian,t.D19_meridian,t.D20_meridian,t.D21_meridian,t.D22_meridian,t.D23_meridian,t.D24_meridian,t.D25_meridian,t.D26_meridian,t.D27_meridian,t.D28_meridian,t.D29_meridian,t.D30_meridian,t.D31_meridian"
   + " having sum((case when D1_Status  like '%LA%'    then 1  else 0 end) +(case when D2_Status  like '%LA%'    then 1  else 0 end) +(case when D3_Status  like '%LA%'    then 1  else 0 end) +  (case when D4_Status  like '%LA%'    then 1  else 0 end) + (case when D5_Status  like '%LA%'    then 1  else 0 end) + (case when D6_Status  like '%LA%'    then 1  else 0 end) + (case when D7_Status  like '%LA%'    then 1  else 0 end) +  (case when D8_Status  like '%LA%'    then 1  else 0 end) + (case when D9_Status  like '%LA%'    then 1  else 0 end) + (case when D10_Status  like '%LA%'    then 1  else 0 end) + (case when D11_Status  like '%LA%'    then 1  else 0 end) + (case when D12_Status  like '%LA%'    then 1  else 0 end) + (case when D13_Status  like '%LA%'    then 1  else 0 end) + (case when D14_Status  like '%LA%'    then 1  else 0 end) + (case when D15_Status  like '%LA%'    then 1  else 0 end) + (case when D16_Status  like '%LA%'    then 1  else 0 end) + (case when D17_Status  like '%LA%'    then 1  else 0 end) + (case when D18_Status  like '%LA%'    then 1  else 0 end) + (case when D19_Status  like '%LA%'    then 1  else 0 end) + (case when D20_Status  like '%LA%'    then 1  else 0 end) + (case when D21_Status  like '%LA%'    then 1  else 0 end) + (case when D22_Status  like '%LA%'    then 1  else 0 end) + (case when D23_Status  like '%LA%'    then 1  else 0 end) + (case when D24_Status  like '%LA%'    then 1  else 0 end) + (case when D25_Status  like '%LA%'    then 1  else 0 end) + (case when D26_Status  like '%LA%'    then 1  else 0 end) + (case when D27_Status  like '%LA%'    then 1  else 0 end) + (case when D28_Status  like '%LA%'    then 1  else 0 end) + (case when D29_Status  like '%LA%'    then 1  else 0 end) + (case when D30_Status  like '%LA%'    then 1  else 0 end) + (case when D31_Status  like '%LA%'    then 1  else 0 end))>=3 ";

            return sh.Get_Table_FromQry(qry);
        }
        public DataTable EmpMonthlyLateMemoTimesheet1(string month, string year, string empcode)
        {
            string cond = "";
            cond = " where t.userid=" + empcode + " and month = " + month + " and year = " + year;

            string qry = "select t.Id,t.userId as Empcode,des.Name as Designation,e.ShortName as EmpName," +
   " case when b.name = 'OtherBranch' then d.name else b.name end as BrDept," +
   " Concat(CONVERT(varchar(3), DateName(MM,DATEADD(MONTH,t.Month,-1))),'-',t.Year) as Month," +
   " sm.InTime+ ' - ' +sm.OutTime as BranchTimings,sm.OutTime as BranchCheckOutTime," +
   " sum((case when D1_Status  like '%ED%'    then 1  else 0 end) +(case when D2_Status  like '%ED%'    then 1  else 0 end) +(case when D3_Status  like '%ED%'    then 1  else 0 end) + " +
" (case when D4_Status  like '%ED%'    then 1  else 0 end) + (case when D5_Status  like '%ED%'    then 1  else 0 end) + (case when D6_Status  like '%ED%'    then 1  else 0 end) + (case when D7_Status  like '%ED%'    then 1  else 0 end) + " +
" (case when D8_Status  like '%ED%'    then 1  else 0 end) + (case when D9_Status  like '%ED%'    then 1  else 0 end) + (case when D10_Status  like '%ED%'    then 1  else 0 end) + (case when D11_Status  like '%ED%'    then 1  else 0 end) + (case when D12_Status  like '%ED%'    then 1  else 0 end) + (case when D13_Status  like '%ED%'    then 1  else 0 end) + (case when D14_Status  like '%ED%'    then 1  else 0 end) + (case when D15_Status  like '%ED%'    then 1  else 0 end) + (case when D16_Status  like '%ED%'    then 1  else 0 end) + (case when D17_Status  like '%ED%'    then 1  else 0 end) + (case when D18_Status  like '%ED%'    then 1  else 0 end) + (case when D19_Status  like '%ED%'    then 1  else 0 end) + (case when D20_Status  like '%ED%'    then 1  else 0 end) + (case when D21_Status  like '%ED%'    then 1  else 0 end) + (case when D22_Status  like '%ED%'    then 1  else 0 end) + (case when D23_Status  like '%ED%'    then 1  else 0 end) + (case when D24_Status  like '%ED%'    then 1  else 0 end) + (case when D25_Status  like '%ED%'    then 1  else 0 end) + (case when D26_Status  like '%ED%'    then 1  else 0 end) + (case when D27_Status  like '%ED%'    then 1  else 0 end) + (case when D28_Status  like '%ED%'    then 1  else 0 end) + (case when D29_Status  like '%ED%'    then 1  else 0 end) + (case when D30_Status  like '%ED%'    then 1  else 0 end) + (case when D31_Status  like '%ED%'    then 1  else 0 end) ) as TotalEarlyDepartures " +
   " ,sum((case when D1_Status  like '%LA%'    then 1  else 0 end) +(case when D2_Status  like '%LA%'    then 1  else 0 end) +(case when D3_Status  like '%LA%'    then 1  else 0 end) + " +
   " (case when D4_Status  like '%LA%'    then 1  else 0 end) + (case when D5_Status  like '%LA%'    then 1  else 0 end) + (case when D6_Status  like '%LA%'    then 1  else 0 end) + (case when D7_Status  like '%LA%'    then 1  else 0 end) + " +
   " (case when D8_Status  like '%LA%'    then 1  else 0 end) + (case when D9_Status  like '%LA%'    then 1  else 0 end) + (case when D10_Status  like '%LA%'    then 1  else 0 end) + (case when D11_Status  like '%LA%'    then 1  else 0 end) + (case when D12_Status  like '%LA%'    then 1  else 0 end) + (case when D13_Status  like '%LA%'    then 1  else 0 end) + (case when D14_Status  like '%LA%'    then 1  else 0 end) + (case when D15_Status  like '%LA%'    then 1  else 0 end) + (case when D16_Status  like '%LA%'    then 1  else 0 end) + (case when D17_Status  like '%LA%'    then 1  else 0 end) + (case when D18_Status  like '%LA%'    then 1  else 0 end) + (case when D19_Status  like '%LA%'    then 1  else 0 end) + (case when D20_Status  like '%LA%'    then 1  else 0 end) + (case when D21_Status  like '%LA%'    then 1  else 0 end) + (case when D22_Status  like '%LA%'    then 1  else 0 end) + (case when D23_Status  like '%LA%'    then 1  else 0 end) + (case when D24_Status  like '%LA%'    then 1  else 0 end) + (case when D25_Status  like '%LA%'    then 1  else 0 end) + (case when D26_Status  like '%LA%'    then 1  else 0 end) + (case when D27_Status  like '%LA%'    then 1  else 0 end) + (case when D28_Status  like '%LA%'    then 1  else 0 end) + (case when D29_Status  like '%LA%'    then 1  else 0 end) + (case when D30_Status  like '%LA%'    then 1  else 0 end) + (case when D31_Status  like '%LA%'    then 1  else 0 end) ) as TotalLateComes, " +
   " concat((case when D1_Status like  '%LA%'     then '01' + '(' +  (SUBSTRING(t.d1, 1, 5)   )+( case when t.D1_meridian is not null then  subString(t.d1_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d1, 7, 5)   )+( case when t.D1_meridian is not null then  subString(t.d1_meridian,4,2) else '' end )+')'  end) , (case when D2_Status like  '%LA%'     then '<br/>' + '02' + '(' +    (SUBSTRING(t.d2, 1, 5)   )+( case when t.D2_meridian is not null then  subString(t.d2_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d2,7, 5)   )+ ( case when t.D2_meridian is not null then  subString(t.d2_meridian,4,2) else '' end )+')'   end) ,(case when D3_Status like  '%LA%'     then '<br/>' + '03' + '(' +    (SUBSTRING(t.d3, 1, 5)   ) +( case when t.D3_meridian is not null then  subString(t.d3_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d3,7, 5)   ) +( case when t.D3_meridian is not null then  subString(t.d3_meridian,4,2) else '' end ) +')'  end) ,  (case when D4_Status like  '%LA%'     then '<br/>' + '04' + '(' +    (SUBSTRING(t.d4, 1, 5)   ) +( case when t.D4_meridian is not null then  subString(t.d4_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d4, 7, 5)   ) + ( case when t.D4_meridian is not null then  subString(t.d4_meridian,4,2) else '' end )+')'   end),(case when D5_Status like  '%LA%'     then '<br/>' + '05' + '(' +    (SUBSTRING(t.d5, 1, 5)   ) + ( case when t.D5_meridian is not null then  subString(t.d5_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d5,7, 5)   ) + ( case when t.D5_meridian is not null then  subString(t.d5_meridian,4,2) else '' end )+')'   end), (case when D6_Status like  '%LA%'     then '<br/>' + '06' + '(' +    (SUBSTRING(t.d6, 1, 5)   ) +( case when t.D6_meridian is not null then  subString(t.d6_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d6, 7, 5)   ) + ( case when t.D6_meridian is not null then  subString(t.d6_meridian,4,2) else '' end )+')'   end),(case when D7_Status like  '%LA%'     then '<br/>' + '07' + '(' +    (SUBSTRING(t.d7, 1, 5)   ) + ( case when t.D7_meridian is not null then  subString(t.d7_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d7, 7, 5)   ) +( case when t.D7_meridian is not null then  subString(t.d7_meridian,4,2) else '' end )+')'   end), (case when D8_Status like  '%LA%'     then '<br/>' + '08' + '(' +    (SUBSTRING(t.d8, 1, 5)   ) + ( case when t.D8_meridian is not null then  subString(t.d8_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d8, 7, 5)   ) +( case when t.D8_meridian is not null then  subString(t.d8_meridian,4,2) else '' end )+')'  end),(case when D9_Status like  '%LA%'     then '<br/>' + '09' + '(' +    (SUBSTRING(t.d9, 1, 5)   ) +( case when t.D9_meridian is not null then  subString(t.d9_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d9, 7, 5)   ) +( case when t.D9_meridian is not null then  subString(t.d9_meridian,4,2) else '' end )+')'  end),(case when D10_Status like  '%LA%'     then '<br/>' + '10' + '(' +    (SUBSTRING(t.d10,1, 5)   ) +( case when t.D10_meridian is not null then  subString(t.d10_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d10, 7, 5)   ) + ( case when t.D10_meridian is not null then  subString(t.d10_meridian,4,2) else '' end ) +')'  end),(case when D11_Status like  '%LA%'     then '<br/>' + '11' + '(' +    (SUBSTRING(t.d11, 1, 5)   ) + ( case when t.D11_meridian is not null then  subString(t.d11_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d11,7, 5)   ) +( case when t.D11_meridian is not null then  subString(t.d11_meridian,4,2) else '' end )+')'   end),(case when D12_Status like  '%LA%'     then '<br/>' + '12' + '(' +    (SUBSTRING(t.d12, 1, 5)   ) +( case when t.D12_meridian is not null then  subString(t.d12_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d12, 7, 5)   ) + ( case when t.D12_meridian is not null then  subString(t.d12_meridian,4,2) else '' end )+')'  end),(case when D13_Status like  '%LA%'     then '<br/>' + '13' + '(' +    (SUBSTRING(t.d13, 1, 5)   ) +( case when t.D13_meridian is not null then  subString(t.d13_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d13, 7, 5)   ) + ( case when t.D13_meridian is not null then  subString(t.d13_meridian,4,2) else '' end )+')'  end),(case when D14_Status like  '%LA%'     then '<br/>' + '14' + '(' +    (SUBSTRING(t.d14, 1, 5)   ) +( case when t.D14_meridian is not null then  subString(t.d14_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d14, 7, 5)   ) +( case when t.D14_meridian is not null then  subString(t.d14_meridian,4,2) else '' end )+')'  end),(case when D15_Status like  '%LA%'     then '<br/>' + '15' + '(' +    (SUBSTRING(t.d15, 1, 5)   ) +( case when t.D15_meridian is not null then  subString(t.d15_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d15, 7, 5)   ) +( case when t.D15_meridian is not null then  subString(t.d15_meridian,4,2) else '' end ) +')' end),(case when D16_Status like  '%LA%'     then '<br/>' + '16' + '(' +    (SUBSTRING(t.d16, 1, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d16, 7, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,4,2) else '' end ) +')'  end),(case when D17_Status like  '%LA%'     then '<br/>' + '17' + '(' +    (SUBSTRING(t.d17, 1, 5)   ) +( case when t.D17_meridian is not null then  subString(t.d17_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d17, 7, 5)   ) +( case when t.D17_meridian is not null then  subString(t.d17_meridian,4,2) else '' end )+')'  end),(case when D18_Status like  '%LA%'     then '<br/>' + '18' + '(' +    (SUBSTRING(t.d18, 1, 5)   ) +( case when t.D18_meridian is not null then  subString(t.d18_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d18, 7, 5)   ) +( case when t.D16_meridian is not null then  subString(t.d16_meridian,4,2) else '' end ) +')'  end), (case when D19_Status like  '%LA%'     then '<br/>' + '19' + '(' +    (SUBSTRING(t.d19, 1, 5)   ) +( case when t.D19_meridian is not null then  subString(t.d19_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d19, 7, 5)   ) +( case when t.D19_meridian is not null then  subString(t.d19_meridian,4,2) else '' end )+')'  end), (case when D20_Status like  '%LA%'     then '<br/>' + '20' + '(' +    (SUBSTRING(t.d20, 1, 5)   ) +( case when t.D20_meridian is not null then  subString(t.d20_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d20, 7, 5)   ) +( case when t.D20_meridian is not null then  subString(t.d20_meridian,4,2) else '' end )+')'  end), (case when D21_Status like  '%LA%'     then '<br/>' + '21' + '(' +    (SUBSTRING(t.d21, 1, 5)   ) +( case when t.D21_meridian is not null then  subString(t.d21_meridian,4,2) else '' end )+ '-' +    (SUBSTRING(t.d21, 7, 5)   ) +( case when t.D21_meridian is not null then  subString(t.d21_meridian,4,2) else '' end ) +')'  end), (case when D22_Status like  '%LA%'     then '<br/>' + '22' + '(' +    (SUBSTRING(t.d22, 1, 5)   ) +( case when t.D22_meridian is not null then  subString(t.d22_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d22, 7, 5)   )  +( case when t.D22_meridian is not null then  subString(t.d22_meridian,4,2) else '' end )+')' end), (case when D23_Status like  '%LA%'     then '<br/>' + '23' + '(' +    (SUBSTRING(t.d23, 1, 5)   ) +( case when t.D23_meridian is not null then  subString(t.d23_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d23, 7, 5)   ) +( case when t.D23_meridian is not null then  subString(t.d23_meridian,4,2) else '' end )+')'  end), (case when D24_Status like  '%LA%'     then '<br/>' + '24' + '(' +    (SUBSTRING(t.d24, 1, 5)   ) +( case when t.D24_meridian is not null then  subString(t.d24_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d24, 7, 5)   ) ++( case when t.D24_meridian is not null then  subString(t.d24_meridian,1,2) else '' end )+')'  end),(case when D25_Status like  '%LA%'     then '<br/>' + '25' + '(' +    (SUBSTRING(t.d25, 1, 5)   ) +( case when t.D25_meridian is not null then  subString(t.d25_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d25, 7, 5)   ) +( case when t.D25_meridian is not null then  subString(t.d25_meridian,4,2) else '' end )+')'  end),(case when D26_Status like  '%LA%'     then '<br/>' + '26' + '(' +    (SUBSTRING(t.d26, 1, 5)   ) +( case when t.D26_meridian is not null then  subString(t.d26_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d26, 7, 5)   ) +( case when t.D26_meridian is not null then  subString(t.d26_meridian,4,2) else '' end )+')'  end),(case when D27_Status like  '%LA%'     then '<br/>' + '27' + '(' +    (SUBSTRING(t.d27, 1, 5)   ) +( case when t.D27_meridian is not null then  subString(t.d27_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d27,7, 5)   ) +( case when t.D27_meridian is not null then  subString(t.d27_meridian,4,2) else '' end )+')'  end),(case when D28_Status like  '%LA%'     then '<br/>' + '28' + '(' +    (SUBSTRING(t.d28, 1, 5)   ) +( case when t.D28_meridian is not null then  subString(t.d28_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d28,7, 5)   )+( case when t.D28_meridian is not null then  subString(t.d28_meridian,4,2) else '' end ) +')'  end),(case when D29_Status like  '%LA%'     then '<br/>' + '29' + '(' +    (SUBSTRING(t.d29, 1, 5)   ) +( case when t.D29_meridian is not null then  subString(t.d29_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d29, 7, 5)   ) +( case when t.D29_meridian is not null then  subString(t.d29_meridian,4,2) else '' end )+')'  end),(case when D30_Status like  '%LA%'     then '<br/>' + '30' + '(' +    (SUBSTRING(t.d30, 1, 5)   )+( case when t.D30_meridian is not null then  subString(t.d30_meridian,1,2) else '' end ) + '-' +    (SUBSTRING(t.d30, 7, 5)   )+( case when t.D30_meridian is not null then  subString(t.d30_meridian,4,2) else '' end ) +')'  end),(case when D31_Status like  '%LA%'     then '<br/>' + '31' + '(' +    (SUBSTRING(t.d31, 1, 5)   ) +( case when t.D31_meridian is not null then  subString(t.d31_meridian,1,2) else '' end )+ '-' +    (SUBSTRING(t.d31, 7, 5)   ) +( case when t.D31_meridian is not null then  subString(t.d31_meridian,4,2) else '' end )+')'  end)) as IntimeOutTime from timesheet_Emp_Month t" +
   " join Employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id " +
   " " + cond + "and e.empid!=1 and (D1_Status   like  '%LA%'  or   D2_Status  like  '%LA%'  or  D3_Status  like  '%LA%'  or  D4_Status  like  '%LA%'  or  D5_Status  like  '%LA%'  or  D6_Status  like  '%LA%'  or  D7_Status  like  '%LA%'  or  D8_Status  like  '%LA%'  or  D9_Status  like  '%LA%'  or  D10_Status  like  '%LA%'  or  D11_Status  like  '%LA%'  or  D12_Status  like  '%LA%'  or  D13_Status  like  '%LA%'  or  D14_Status  like  '%LA%'  or  D15_Status  like  '%LA%'  or  D16_Status  like  '%LA%'  or  D17_Status  like  '%LA%'  or  D18_Status  like  '%LA%'  or  D19_Status  like  '%LA%'  or  D20_Status  like  '%LA%'  or  D21_Status  like  '%LA%'  or  D22_Status  like  '%LA%'  or  D23_Status  like  '%LA%'  or  D24_Status  like  '%LA%'  or  D25_Status  like  '%LA%'  or  D26_Status  like  '%LA%'  or  D27_Status  like  '%LA%'  or  D28_Status  like  '%LA%'  or  D29_Status  like  '%LA%'  or  D30_Status  like  '%LA%'  or  D31_Status like  '%LA%')" +
   " GROUP BY D1_Status,D2_Status,D3_Status,D4_Status,D5_Status,D6_Status,D7_Status,D8_Status,D9_Status,D10_Status,D11_Status,D12_Status,D13_Status,D14_Status,D15_Status,D16_Status,D17_Status,D18_Status,D19_Status,D20_Status,D21_Status,D22_Status,D23_Status,D24_Status,D25_Status,D26_Status,D27_Status,D28_Status,D29_Status,D30_Status,D31_Status,UserId,month,year,ShortName,b.Name,d.Name,des.Name,b.StartTime,b.EndTime,sm.InTime,sm.OutTime,t.D1,t.D2 ,t.D3,t.D4,t.D5,t.D6,t.D7,t.D8,t.D9,t.D10,t.D11,t.D12,t.D13,t.D14,t.D15,t.D16,t.D17,t.D18,t.D19,t.D19,t.D20,t.D21,t.D22,t.D23,t.D24,t.D25,t.D26,t.D27,t.D28,t.D29,t.D30,t.D31,t.Id,t.D1_meridian,t.D2_meridian,t.D3_meridian,t.D4_meridian,t.D5_meridian,t.D6_meridian,t.D7_meridian,t.D8_meridian,t.D9_meridian,t.D10_meridian,t.D11_meridian,t.D12_meridian,t.D13_meridian,t.D14_meridian,t.D15_meridian,t.D16_meridian,t.D17_meridian,t.D18_meridian,t.D19_meridian,t.D20_meridian,t.D21_meridian,t.D22_meridian,t.D23_meridian,t.D24_meridian,t.D25_meridian,t.D26_meridian,t.D27_meridian,t.D28_meridian,t.D29_meridian,t.D30_meridian,t.D31_meridian"
   + " having sum((case when D1_Status  like '%LA%'    then 1  else 0 end) +(case when D2_Status  like '%LA%'    then 1  else 0 end) +(case when D3_Status  like '%LA%'    then 1  else 0 end) +  (case when D4_Status  like '%LA%'    then 1  else 0 end) + (case when D5_Status  like '%LA%'    then 1  else 0 end) + (case when D6_Status  like '%LA%'    then 1  else 0 end) + (case when D7_Status  like '%LA%'    then 1  else 0 end) +  (case when D8_Status  like '%LA%'    then 1  else 0 end) + (case when D9_Status  like '%LA%'    then 1  else 0 end) + (case when D10_Status  like '%LA%'    then 1  else 0 end) + (case when D11_Status  like '%LA%'    then 1  else 0 end) + (case when D12_Status  like '%LA%'    then 1  else 0 end) + (case when D13_Status  like '%LA%'    then 1  else 0 end) + (case when D14_Status  like '%LA%'    then 1  else 0 end) + (case when D15_Status  like '%LA%'    then 1  else 0 end) + (case when D16_Status  like '%LA%'    then 1  else 0 end) + (case when D17_Status  like '%LA%'    then 1  else 0 end) + (case when D18_Status  like '%LA%'    then 1  else 0 end) + (case when D19_Status  like '%LA%'    then 1  else 0 end) + (case when D20_Status  like '%LA%'    then 1  else 0 end) + (case when D21_Status  like '%LA%'    then 1  else 0 end) + (case when D22_Status  like '%LA%'    then 1  else 0 end) + (case when D23_Status  like '%LA%'    then 1  else 0 end) + (case when D24_Status  like '%LA%'    then 1  else 0 end) + (case when D25_Status  like '%LA%'    then 1  else 0 end) + (case when D26_Status  like '%LA%'    then 1  else 0 end) + (case when D27_Status  like '%LA%'    then 1  else 0 end) + (case when D28_Status  like '%LA%'    then 1  else 0 end) + (case when D29_Status  like '%LA%'    then 1  else 0 end) + (case when D30_Status  like '%LA%'    then 1  else 0 end) + (case when D31_Status  like '%LA%'    then 1  else 0 end))>=3 ";

            return sh.Get_Table_FromQry(qry);
        }
        public DataTable MonthlyEarlyDepatureRes(string branch, string fromdate, string empcode)
        {
            string cond = "";
            if (branch != "" && branch != "0" && fromdate != "")
            {
                cond = "where d.id='" + branch + "'" + "  and RIGHT(CONVERT(VARCHAR(10), t.UpdatedDate, 105), 7)='" + fromdate + "'";

            }
            else if (branch == "" && fromdate != "")
            {
                cond = "where RIGHT(CONVERT(VARCHAR(10), t.UpdatedDate, 105), 7)='" + fromdate + "'";
            }
            else if (branch != "" && branch != "0" && fromdate == "")
            {
                cond = "where  d.id='" + branch + "'";
            }
            else if (branch != "" && branch == "0" && fromdate != "")
            {
                cond = "where RIGHT(CONVERT(VARCHAR(10), t.UpdatedDate, 105), 7)='" + fromdate + "'";
            }
            else if (branch != "" && branch == "0" && fromdate != "")
            {
                cond = "where  d.id='" + branch + "'";
            }
            string qry = "select RIGHT(CONVERT(VARCHAR(10), t.UpdatedDate, 105), 7) AS Month,D4,e.ShortName as EmpName,e.EmpId,des.Name as Designation" +
           " case when b.name='OtherBranch' then d.name else b.name end " +
           " as BrDept, b.StartTime as BranchStartTime,SUBSTRING(t.d4, 1, 5) as EmpCheckInTime,'' as LateBy,  " +
           " b.EndTime as BranchCheckOutTime,SUBSTRING(t.d4, 1, 5) as EmpCheckOutTime,'' as EarlyBy  " +
           " from timesheet_Emp_Month t join employees e on t.UserId = e.EmpId join Designations on e.CurrentDesignation=des.Id join branches b on e.Branch = b.id " +
           " join Departments d on e.Department = d.id join timesheet_Emp_Month_UID U on t.D4_GUID = U.Unique_Id" + " " + cond;

            return sh.Get_Table_FromQry(qry);
        }

        public DataTable LeavesapprovedbutattRept(string branch, string fromdate, string todate, string self, int EmpIds, string empcode)
        {

            string cond2 = "";
            string cond3 = "";
            string cond4 = "";
            string cond6 = "";
            string condE2 = "";
            string condE3 = "";
            string condE4 = "";
            string qry = "";
            if (fromdate != "" && todate != "" && branch != "")
            {

                //between fromdate and todate
                List<DateTime> allDates = new List<DateTime>();
                // string fromdate= DateTime.Parse(fromdate.ToString()).ToString("dd/MM/yyyy");
                for (DateTime date = Convert.ToDateTime(fromdate); date <= Convert.ToDateTime(todate); date = date.AddDays(1))
                {
                    allDates.Add(date);
                }
                DateTime[] dates = allDates.ToArray();
                string test = string.Join(",", dates);

                DateTime start = Convert.ToDateTime(fromdate).AddDays(-1);
                DateTime end = Convert.ToDateTime(todate);
                DateTime chunkEnd;
                int dayChunkSize = 1;
                while ((chunkEnd = start.AddDays(dayChunkSize)) <= end)
                {

                    Tuple.Create(start, chunkEnd);
                    start = chunkEnd;
                    DateTime chunkend1 = Convert.ToDateTime(start);
                    string start1 = chunkend1.ToString("dd/MM/yyyy");

                    fromdate = start1;
                    cond4 = fromdate;
                    condE4 = todate;
                    if (branch == string.Empty && fromdate == string.Empty && todate == string.Empty)
                    {
                        cond6 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    if (branch != string.Empty && fromdate != string.Empty && todate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "True")
                    {
                        cond6 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "') and e.EmpId=" + EmpIds + "";
                    }
                    if (branch != string.Empty && fromdate != string.Empty && todate != string.Empty && branch != "All" && branch != "HeadOffice-All" && self == "False")
                    {
                        cond6 = " and (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
                    }
                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && todate != string.Empty && self == "False" && empcode == "")
                    {
                        cond6 = "";
                    }

                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && todate != string.Empty && self == "False" && empcode != "")
                    {
                        cond6 = "and e.EmpId=" + empcode;

                    }
                    else if (branch == "All" && branch != "HeadOffice-All" && fromdate != string.Empty && todate != string.Empty && self == "True")
                    {
                        cond6 = "and UserId=" + EmpIds + "";
                    }
                    else if (branch == "HeadOffice-All" && fromdate != string.Empty && todate != string.Empty)
                    {
                        cond6 = " and e.Department != 46" + " and e.Branch = 43";
                    }
                    DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                    //DateTime fdt2 = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                    qry += "  select distinct t.User_id as EmpId,e.Shortname as EmpName,des.code as Designation," +
                        "CONVERT(varchar(30), CAST(MIN(t.io_time) as Date),103 ) as applieddate,case when b.name = 'OtherBranch' then d.name else b.name end as BrDept," +
                        "Concat(lt.Code, '/', l.Status) as Status,CONVERT(varchar(15), CAST(Min(t.io_time) AS TIME), 100) as EmpCheckInTime," +
                        "CONVERT(varchar(15), CAST(Max(t.io_time) AS TIME), 100) as EmpCheckOutTime from leaves l" +
                        " join Employees e on l.EmpId = e.Id join timesheet_logs t on t.user_id = e.Empid and " +
                        "cast(t.io_time as Date)  = '" + fdt + "'  join leavetypes lt on l.LeaveType = lt.Id " +
                        "join Branches b on b.Id = e.Branch join Designations des on e.CurrentDesignation = des.Id " +
                        "join Departments d on d.Id = e.Department where(l.Startdate <= '" + fdt + "' and l.enddate >= '" + fdt + "') and " +
                        "year(t.io_time) = Year('" + fdt + "') and month(t.io_time) = Month('" + fdt + "')  " + cond6 + "" +
                        " group by e.ShortName ,t.user_id,des.code, des.Name ,b.name ,d.name,lt.Code,l.status" + "  union";
                }
            }

            else
            {
                qry = "select distinct t.User_id as EmpId,e.Shortname as EmpName,des.code as Designation, " +
                    "CONVERT(varchar(30), CAST(MIN(t.io_time) as Date),103 ) as applieddate,case when b.name = 'OtherBranch' then d.name else b.name end as BrDept, " +
                    "Concat(lt.Code, '/', l.Status) as Status,CONVERT(varchar(15), CAST(Min(t.io_time) AS TIME), 100) as EmpCheckInTime, " +
                    "CONVERT(varchar(15), CAST(Max(t.io_time) AS TIME), 100) as EmpCheckOutTime from leaves l join Employees e on l.EmpId = e.Id " +
                    "join timesheet_logs t on t.user_id = e.Empid join leavetypes lt on l.LeaveType = lt.Id" +
                    "  join Branches b on b.Id = e.Branch join Designations des on e.CurrentDesignation = des.Id" +
                    "  join Departments d on d.Id = e.Department  where b.Name = ''" +
                    "  group by e.ShortName ,t.user_id,des.code,des.Name ,b.name ,d.name,lt.Code,l.status" + "  union";
            }
            qry = qry.Substring(0, qry.Length - 6);
            return sh.Get_Table_FromQry(qry);
        }
        public DataTable getAllTimesheetApprovals(string branch, string fromdate, string todate, string empcode, string Type, string self, int EmpIds)
        {
            string cond = "";
            //when branch is null and fromdate is null.
            if (branch == string.Empty && fromdate == string.Empty && string.IsNullOrEmpty(Type) && string.IsNullOrEmpty(empcode))
            {
                cond = "and (b.name='" + branch + "'" + "  or dept.name='" + branch + "')";
            }
            else if (branch == string.Empty && empcode == string.Empty && fromdate != "" && todate == "" && Type == "")
            {
                cond = "and convert(varchar, ts.ReqFromDate,23) ='" + cond + "'";
            }
            else if (branch == string.Empty && empcode == string.Empty && fromdate == "" && todate != "" && Type == "")
            {
                cond = "and convert(varchar, ts.ReqFromDate,23) ='" + cond + "'";
            }
            else if (branch != string.Empty && empcode == string.Empty && fromdate == "" && todate != "" && Type == "")
            {
                cond = "and convert(varchar, ts.ReqFromDate,23) ='" + cond + "'";
            }
            else if (branch != string.Empty && empcode == string.Empty && fromdate != "" && todate == "" && Type == "")
            {
                cond = "and convert(varchar, ts.ReqFromDate,23) ='" + cond + "'";
            }
            else if (branch != string.Empty && empcode != string.Empty && fromdate == "" && todate != "" && Type == "")
            {
                cond = "and convert(varchar, ts.ReqFromDate,23) ='" + cond + "'";
            }
            else if (branch != string.Empty && empcode != string.Empty && fromdate != "" && todate == "" && Type == "")
            {
                cond = "and convert(varchar, ts.ReqFromDate,23) ='" + cond + "'";
            }
            //when branch contains "Name" and (fromdate&todate) are not null and empcode is not null.
            else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && Type == "")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = "and emp.EmpId =" + empcode + " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))";
            }
            //when branch contains "All" and (fromdate&todate) are not null and empcode is null.
            else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode == "" && Type == "")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))";
            }
            //when branch contains "All" and (fromdate&todate) are  null and empcode is null.
            else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate == "" && todate == "" && empcode == "" & Type == "")
            {
                cond = "";
            }
            //when branch contains "Name" and (fromdate&todate) are  null and empcode is null.
            else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate == "" && todate == "" && empcode == "" && Type == "")
            {
                cond = "and (b.name='" + branch + "'" + "  or dept.name='" + branch + "')";
            }
            //when branch contains "Name" and (fromdate&todate) are  null and empcode is not null.
            else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate == "" && todate == "" && empcode != "" && Type == "" && self == "False")
            {
                cond = "and emp.EmpId =" + empcode;
            }
            //when branch contains "Name" and (fromdate&todate) are not null and empcode is null.
            else if (branch != "" && branch != "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode == "" && Type == "")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + "and emp.EmpId =" + EmpIds;
            }
            //when fromdate&todate are not null
            else if (branch == "" && fromdate != "" && todate != "" && empcode == "" && Type == "")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))";
            }
            //when branch contains 'All' and fromdate&todate are not null and empcode is not null
            else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && Type == "" && self == "False")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + "and emp.EmpId =" + empcode;

            }
            //when branch contains 'All-HO' and fromdate&todate&empcode are null
            else if (branch == "HeadOffice-All" && branch != "All" && branch != "" && fromdate == "" && todate == "" && empcode == "" && Type == "")
            {
                cond = " and emp.Department != 46" + " and emp.Branch = 43";
            }
            //when branch contains 'All-HO' and fromdate&todate are null and empcode is not null
            else if (branch == "HeadOffice-All" && branch != "All" && fromdate == "" && todate == "" && empcode != "" && Type == "")
            {
                cond = " and emp.Department != 46" + " and emp.Branch = 43" + "and emp.EmpId =" + empcode;
            }
            //when branch contains 'Name' and fromdate&todate&empcode is null
            else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate == "" && todate == "" && empcode == "" && Type == "")
            {
                cond = "and (b.name='" + branch + "'" + "  or dept.name='" + branch + "')";
            }
            //when branch contains 'All-HO' and fromdate&todate are not null and empcode is null
            else if (branch == "HeadOffice-All" && branch != "All" && fromdate != "" && todate != "" && empcode == "" && Type == "" && Type == "")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and emp.Department != 46" + " and emp.Branch = 43" + " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))";
            }
            //when branch contains 'All-HO' and fromdate&todate&empcode are not null  
            else if (branch == "HeadOffice-All" && branch != "All" && fromdate != "" && todate != "" && empcode != "" && Type == "")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = "and (b.name='" + branch + "'" + "  or dept.name='" + branch + "')";
            }
            else if (branch == "" && fromdate == "" && todate == "" && empcode == "" && Type != "" && Type != "All")
            {

                cond = "and ts.Reason_Type ='" + Type + "'";
            }
            else if (branch == "" && fromdate != "" && todate != "" && empcode == "" && Type != "" && Type != "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and ts.Reason_Type ='" + Type + "'";
            }
            else if (branch == "" && fromdate != "" && todate != "" && empcode != "" && Type != "" && Type != "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and ts.Reason_Type ='" + Type + "'" + " and emp.EmpId ='" + empcode + "'";
            }
            else if (branch == "All" && fromdate == "" && todate == "" && empcode == "" && Type != "" && Type != "All")
            {
                cond = "and ts.Reason_Type ='" + Type + "'";
            }
            else if (branch == "All" && fromdate != "" && todate != "" && empcode == "" && Type != "" && Type != "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((ts.ReqFromDate >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (ts.ReqFromDate <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and ts.Reason_Type ='" + Type + "'";
            }
            else if (branch == "HeadOffice-All" && fromdate != "" && todate != "" && empcode == "" && Type != "" && Type != "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and ts.Reason_Type ='" + Type + "'" + " and emp.Department != 46" + " and emp.Branch = 43";
            }
            else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate != "" && todate != "" && empcode == "" && Type != "" && Type != "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and ts.Reason_Type ='" + Type + "'";
            }
            else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate != "" && todate != "" && empcode != "" && Type != "" && Type != "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and ts.Reason_Type ='" + Type + "'" + " and emp.EmpId =" + empcode;
            }
            else if (branch == "All" && fromdate != "" && todate != "" && empcode != "" && Type != "" && Type != "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and ts.Reason_Type ='" + Type + "'" + " and emp.EmpId ='" + empcode + "'";
            }
            else if (branch == "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && Type != "" && Type != "All")
            {
                cond = "and (b.name='" + branch + "'" + "  or dept.name='" + branch + "')";
            }
            else if (branch == "" && fromdate != "" && todate != "" && empcode != "" && Type == "")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and emp.EmpId =" + empcode;
            }
            else if (branch == "" && fromdate == "" && todate == "" && empcode == "" && Type == "All")
            {
                cond = "";
            }
            else if (branch == "" && fromdate != "" && todate != "" && empcode == "" && Type == "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))";
            }
            else if (branch == "All" && fromdate != "" && todate != "" && empcode == "" && Type == "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))";
            }
            else if (branch == "All" && fromdate != "" && todate != "" && empcode != "" && Type == "All" && self == "False")
            {

                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and emp.EmpId =" + empcode;
            }

            else if (branch == "All" && fromdate != "" && todate != "" && empcode != "" && Type == "All" && self == "True")
            {

                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and emp.EmpId =" + empcode + " and emp.EmpId=" + EmpIds + "";
            }
            else if (branch == "HeadOffice-All" && fromdate != "" && todate != "" && empcode != "" && Type == "All")
            {
                cond = "and (b.name='" + branch + "'" + "  or dept.name='" + branch + "')";
            }
            else if (branch != "All" && branch != "HeadOffice-All" && branch != "" && fromdate != "" && todate != "" && empcode != "" && Type == "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + "and (b.name='" + branch + "'" + "  or dept.name='" + branch + "')" + " and emp.EmpId =" + empcode;
            }
            else if (branch == "" && fromdate == "" && todate == "" && empcode != "" && Type == "All")
            {
                cond = " and emp.EmpId =" + empcode;
            }
            else if (branch == "" && fromdate != "" && todate != "" && empcode != "" && Type == "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and emp.EmpId =" + empcode;
            }
            else if (branch == "HeadOffice-All" && fromdate != "" && todate != "" && empcode == "" && Type == "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = " and ((convert(varchar, ts.ReqFromDate,23) >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (convert(varchar, ts.ReqFromDate,23) <= '" + tdt.ToString("yyyy-MM-dd") + "'))" + " and emp.Department != 46" + " and emp.Branch = 43"; ;
            }
            else if (branch != "HeadOffice-All" && branch != "All" && branch != "" && fromdate != "" && todate != "" && empcode == "" && Type == "All")
            {
                DateTime fdt = fromdate == "" ? DateTime.Now : Convert.ToDateTime(fromdate);
                DateTime tdt = todate == "" ? DateTime.Now : Convert.ToDateTime(todate);
                cond = "and (b.name='" + branch + "'" + "  or dept.name='" + branch + "')" + " and ((ts.ReqFromDate >= '" + fdt.ToString("yyyy-MM-dd") + "') and  (ts.ReqFromDate <= '" + tdt.ToString("yyyy-MM-dd") + "'))";
            }
            else if (branch == "" && fromdate == "" && todate == "" && empcode != "" && Type == "")
            {
                cond = " and emp.EmpId =" + empcode;
            }
            else if (branch == "All" && branch != "HeadOffice-All" && branch != "" && fromdate == "" && todate == "" && empcode != "" && Type == "")
            {
                cond = " and emp.EmpId =" + empcode;
            }
            else if (branch == "All" && branch != "HeadOffice-All" && branch != "" && fromdate == "" && todate == "" && empcode != "" && Type != "" && Type != "All")
            {
                cond = " and emp.EmpId =" + empcode + " and ts.Reason_Type ='" + Type + "'";
            }
            else if (branch == "All" && branch != "HeadOffice-All" && branch != "" && fromdate == "" && todate == "" && empcode != "" && Type == "All")
            {
                cond = " and emp.EmpId =" + empcode;
            }
            else if (branch != "All" && branch != "HeadOffice-All" && branch != "" && fromdate == "" && todate == "" && empcode != "" && Type != "All" && Type != "")
            {
                cond = "and emp.EmpId =" + empcode + " and ts.Reason_Type ='" + Type + "'";
            }
            else if (branch != "All" && branch != "HeadOffice-All" && branch != "" && fromdate == "" && todate == "" && empcode == "" && Type != "All" && Type != "")
            {
                cond = "and ts.Reason_Type ='" + Type + "'";
            }
            string query = "Select emp.EmpId, emp.Shortname as Name, d.Code as Designation,convert(varchar, ts.ReqFromDate, 103) as RequestDate,ts.Status as Status,ts.Reason_Type,ts.Reason_Desc,ts.entrytime,ts.exittime,(select top 1 shortname  from employees where role=1 and CurrentDesignation=4 and Department=16) as ApprovedBy,"
                 + " case when b.Name='OtherBranch' then dept.Name else b.Name end as BrDepot "
                            + " FROM Timesheet_Request_Form ts " +
                              " join Employees emp on ts.UserId = emp.Id" +
                               " join Employees emp2 on ts.CA = emp2.Id" +
                              " join Designations d on emp.CurrentDesignation = d.ID" +
                              " join Branches b on emp.Branch = b.Id" +
                              " join Departments dept on emp.Department = dept.Id" +
                              " Where ts.Status in('Approved','Cancelled','Denied','Pending')" + " " + cond + " and emp.empid!=1 and ts.Reason_Desc not in ('Leave','OD','LTC')";
            return sh.Get_Table_FromQry(query);
        }
        public DataTable getAllAllotmentofshifts(string branch)
        {
            string cond = "";
            DateTime str = DateTime.Now;
            string str1 = str.ToString("yyyy-MM-dd");
            string[] sa = str1.Split('-');
            string year = sa[0];
            string month = sa[1];
            string day = "01";
            string fulldate = "  '" + year + "-" + month + "-" + day + "' ";
            //When Branch is null.
            if (branch == "")
            {
                cond = "where sm.BranchId ='" + branch + "'";
            }
            //When Branch contains an Id.
            else if (branch != "" && branch != "0")
            {
                cond = "where " + fulldate + " between sm.validfrom and sm.validto and sm.BranchId =" + branch;
            }
            //When Branch contains 'All'.
            else if (branch == "0")
            {
                cond = "where " + fulldate + " between sm.validfrom and sm.validto and sm.BranchId! =43";
            }
            string query = "select sm.Id As ShiftId,case when b.Name = 'OtherBranch' then 'HeadOffice' else b.Name end as BrDepot,"
                            + " case when sm.ShiftType='0' then '0' when sm.ShiftType = '1' then '1st' when sm.ShiftType = '2' then '2nd' when sm.ShiftType = '3' then '3rd' end as ShiftType,"
                            + " sm.InTime as Starttime,sm.OutTime as Endtime,sm.GroupName as Department ,'00:10 Mins' as Graceperiod"
                            + " from Branches b " +
                              " join Shift_Master sm  on b.Id = sm.BranchId " + cond;

            return sh.Get_Table_FromQry(query);
        }
        public DataTable getAllEarlyDeparture(string branch, string fromdate, string self, int EmpIds, string empcode)
        {
            string cond = "";
            if (branch == string.Empty && fromdate == string.Empty)
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
            }
            else if (branch != string.Empty && fromdate == string.Empty && branch != "All" && branch != "HeadOffice-All")
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
            }
            else if (branch != string.Empty && fromdate != "" && branch != "All" && branch != "HeadOffice-All" && self == "False")
            {
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')";
            }
            else if (branch != "" && branch != "All" && fromdate != "" && branch != "HeadOffice-All" && self == "True")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where (b.name='" + branch + "'" + "  or d.name='" + branch + "')" + "and month = " + s2 + " and year = " + s1 + " and empid=" + EmpIds;
            }
            else if (branch == "" && fromdate != "")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1;
            }
            else if (branch != "" && branch == "All" && branch != "HeadOffice-All" && fromdate == "")
            {
                cond = "";
            }
            else if (branch != "" && branch == "All" && fromdate != "" && branch != "HeadOffice-All" && self == "False" && empcode == "")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1;
            }

            else if (branch != "" && branch == "All" && fromdate != "" && branch != "HeadOffice-All" && self == "False" && empcode != "")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1 + " and e.empid=" + empcode;
            }

            else if (branch != "" && branch == "All" && fromdate != "" && branch != "HeadOffice-All" && self == "True")
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where month=" + s2 + "and year =" + s1 + "and empid=" + EmpIds;
            }

            else if (branch == "HeadOffice-All" && fromdate == string.Empty)
            {
                cond = " where e.Department != 46";
            }
            else if (branch == "HeadOffice-All" && fromdate != string.Empty)
            {
                DateTime str = Convert.ToDateTime(fromdate);
                string str1 = str.ToString("yyyy-MM-dd");
                string[] sa = str1.Split('-');
                string s1 = sa[0];
                string s2 = sa[1];
                if (s2.StartsWith("0"))
                {
                    s2 = s2.Substring(1);
                }
                string s3 = sa[2];
                cond = " where e.Department != 46" + " and month = " + s2 + "and year = " + s1;
            }
            string qry = "select t.Id,t.userId as Empcode,e.ShortName as EmpName,des.Name as Designation," +
" case when b.name = 'OtherBranch' then d.name else b.name end as BrDept," +
" Concat(CONVERT(varchar(3), DateName(MM,DATEADD(MONTH,t.Month,-1))),'-',t.Year) as Month," +
" sm.OutTime as BranchCheckOutTime," +
" sum((case when D1_Status  like '%ED%'    then 1  else 0 end) +(case when D2_Status  like '%ED%'    then 1  else 0 end) +(case when D3_Status  like '%ED%'    then 1  else 0 end) + " +
" (case when D4_Status  like '%ED%'    then 1  else 0 end) + (case when D5_Status  like '%ED%'    then 1  else 0 end) + (case when D6_Status  like '%ED%'    then 1  else 0 end) + (case when D7_Status  like '%ED%'    then 1  else 0 end) + " +
" (case when D8_Status  like '%ED%'    then 1  else 0 end) + (case when D9_Status  like '%ED%'    then 1  else 0 end) + (case when D10_Status  like '%ED%'    then 1  else 0 end) + (case when D11_Status  like '%ED%'    then 1  else 0 end) + (case when D12_Status  like '%ED%'    then 1  else 0 end) + (case when D13_Status  like '%ED%'    then 1  else 0 end) + (case when D14_Status  like '%ED%'    then 1  else 0 end) + (case when D15_Status  like '%ED%'    then 1  else 0 end) + (case when D16_Status  like '%ED%'    then 1  else 0 end) + (case when D17_Status  like '%ED%'    then 1  else 0 end) + (case when D18_Status  like '%ED%'    then 1  else 0 end) + (case when D19_Status  like '%ED%'    then 1  else 0 end) + (case when D20_Status  like '%ED%'    then 1  else 0 end) + (case when D21_Status  like '%ED%'    then 1  else 0 end) + (case when D22_Status  like '%ED%'    then 1  else 0 end) + (case when D23_Status  like '%ED%'    then 1  else 0 end) + (case when D24_Status  like '%ED%'    then 1  else 0 end) + (case when D25_Status  like '%ED%'    then 1  else 0 end) + (case when D26_Status  like '%ED%'    then 1  else 0 end) + (case when D27_Status  like '%ED%'    then 1  else 0 end) + (case when D28_Status  like '%ED%'    then 1  else 0 end) + (case when D29_Status  like '%ED%'    then 1  else 0 end) + (case when D30_Status  like '%ED%'    then 1  else 0 end) + (case when D31_Status  like '%ED%'    then 1  else 0 end) ) as TotalEarlyDepartures, " +
" concat((case when D1_Status like  '%ED%'     then '01' + '(' +  (SUBSTRING(t.d1, 1, 5)   ) + '-' +    (SUBSTRING(t.d1, 7, 5)   )+')'  end) , (case when D2_Status like  '%ED%'     then '<br/>' + '02' + '(' +    (SUBSTRING(t.d2, 1, 5)   ) + '-' +    (SUBSTRING(t.d2,7, 5)   )+')'   end) ,(case when D3_Status like  '%ED%'     then '<br/>' + '03' + '(' +    (SUBSTRING(t.d3, 1, 5)   ) + '-' +    (SUBSTRING(t.d3,7, 5)   ) +')'  end) ,  (case when D4_Status like  '%ED%'     then '<br/>' + '04' + '(' +    (SUBSTRING(t.d4, 1, 5)   ) + '-' +    (SUBSTRING(t.d4, 7, 5)   )+')'   end),(case when D5_Status like  '%ED%'     then '<br/>' + '05' + '(' +    (SUBSTRING(t.d5, 1, 5)   ) + '-' +    (SUBSTRING(t.d5,7, 5)   )+')'   end), (case when D6_Status like  '%ED%'     then '<br/>' + '06' + '(' +    (SUBSTRING(t.d6, 1, 5)   ) + '-' +    (SUBSTRING(t.d6, 7, 5)   )+')'   end),(case when D7_Status like  '%ED%'     then '<br/>' + '07' + '(' +    (SUBSTRING(t.d7, 1, 5)   ) + '-' +    (SUBSTRING(t.d7, 7, 5)   )+')'   end), (case when D8_Status like  '%ED%'     then '<br/>' + '08' + '(' +    (SUBSTRING(t.d8, 1, 5)   ) + '-' +    (SUBSTRING(t.d8, 7, 5)   ) +')'  end),(case when D9_Status like  '%ED%'     then '<br/>' + '09' + '(' +    (SUBSTRING(t.d9, 1, 5)   ) + '-' +    (SUBSTRING(t.d9, 7, 5)   ) +')'  end),(case when D10_Status like  '%ED%'     then '<br/>' + '10' + '(' +    (SUBSTRING(t.d10,1, 5)   ) + '-' +    (SUBSTRING(t.d10, 7, 5)   ) +')'  end),(case when D11_Status like  '%ED%'     then '<br/>' + '11' + '(' +    (SUBSTRING(t.d11, 1, 5)   ) + '-' +    (SUBSTRING(t.d11,7, 5)   )+')'   end),(case when D12_Status like  '%ED%'     then '<br/>' + '12' + '(' +    (SUBSTRING(t.d12, 1, 5)   ) + '-' +    (SUBSTRING(t.d12, 7, 5)   ) +')'  end),(case when D13_Status like  '%ED%'     then '<br/>' + '13' + '(' +    (SUBSTRING(t.d13, 1, 5)   ) + '-' +    (SUBSTRING(t.d13, 7, 5)   ) +')'  end),(case when D14_Status like  '%ED%'     then '<br/>' + '14' + '(' +    (SUBSTRING(t.d14, 1, 5)   ) + '-' +    (SUBSTRING(t.d14, 7, 5)   ) +')'  end),(case when D15_Status like  '%ED%'     then '<br/>' + '15' + '(' +    (SUBSTRING(t.d15, 1, 5)   ) + '-' +    (SUBSTRING(t.d15, 7, 5)   )  +')' end),(case when D16_Status like  '%ED%'     then '<br/>' + '16' + '(' +    (SUBSTRING(t.d16, 1, 5)   ) + '-' +    (SUBSTRING(t.d16, 7, 5)   ) +')'  end),(case when D17_Status like  '%ED%'     then '<br/>' + '17' + '(' +    (SUBSTRING(t.d17, 1, 5)   ) + '-' +    (SUBSTRING(t.d17, 7, 5)   ) +')'  end),(case when D18_Status like  '%ED%'     then '<br/>' + '18' + '(' +    (SUBSTRING(t.d18, 1, 5)   ) + '-' +    (SUBSTRING(t.d18, 7, 5)   ) +')'  end), (case when D19_Status like  '%ED%'     then '<br/>' + '19' + '(' +    (SUBSTRING(t.d19, 1, 5)   ) + '-' +    (SUBSTRING(t.d19, 7, 5)   ) +')'  end), (case when D20_Status like  '%ED%'     then '<br/>' + '20' + '(' +    (SUBSTRING(t.d20, 1, 5)   ) + '-' +    (SUBSTRING(t.d20, 7, 5)   ) +')'  end), (case when D21_Status like  '%ED%'     then '<br/>' + '21' + '(' +    (SUBSTRING(t.d21, 1, 5)   ) + '-' +    (SUBSTRING(t.d21, 7, 5)   ) +')'  end), (case when D22_Status like  '%ED%'     then '<br/>' + '22' + '(' +    (SUBSTRING(t.d22, 1, 5)   ) + '-' +    (SUBSTRING(t.d22, 7, 5)   )  +')' end), (case when D23_Status like  '%ED%'     then '<br/>' + '23' + '(' +    (SUBSTRING(t.d23, 1, 5)   ) + '-' +    (SUBSTRING(t.d23, 7, 5)   ) +')'  end), (case when D24_Status like  '%ED%'     then '<br/>' + '24' + '(' +    (SUBSTRING(t.d24, 1, 5)   ) + '-' +    (SUBSTRING(t.d24, 7, 5)   ) +')'  end),(case when D25_Status like  '%ED%'     then '<br/>' + '25' + '(' +    (SUBSTRING(t.d25, 1, 5)   ) + '-' +    (SUBSTRING(t.d25, 7, 5)   ) +')'  end),(case when D26_Status like  '%ED%'     then '<br/>' + '26' + '(' +    (SUBSTRING(t.d26, 1, 5)   ) + '-' +    (SUBSTRING(t.d26, 7, 5)   ) +')'  end),(case when D27_Status like  '%ED%'     then '<br/>' + '27' + '(' +    (SUBSTRING(t.d27, 1, 5)   ) + '-' +    (SUBSTRING(t.d27,7, 5)   ) +')'  end),(case when D28_Status like  '%ED%'     then '<br/>' + '28' + '(' +    (SUBSTRING(t.d28, 1, 5)   ) + '-' +    (SUBSTRING(t.d28,7, 5)   ) +')'  end),(case when D29_Status like  '%ED%'     then '<br/>' + '29' + '(' +    (SUBSTRING(t.d29, 1, 5)   ) + '-' +    (SUBSTRING(t.d29, 7, 5)   ) +')'  end),(case when D30_Status like  '%ED%'     then '<br/>' + '30' + '(' +    (SUBSTRING(t.d30, 1, 5)   ) + '-' +    (SUBSTRING(t.d30, 7, 5)   ) +')'  end),(case when D31_Status like  '%ED%'     then '<br/>' + '31' + '(' +    (SUBSTRING(t.d31, 1, 5)   ) + '-' +    (SUBSTRING(t.d31, 7, 5)   ) +')'  end)) as IntimeOutTime from timesheet_Emp_Month t" +
" join Employees e on t.UserId = e.EmpId join Designations des on e.CurrentDesignation=des.Id  join branches b on e.Branch = b.id join Departments d on e.Department = d.id join shift_master sm on sm.Id = e.Shift_Id " +
" " + cond + " and (D1_Status   like  '%ED%'  or   D2_Status  like  '%ED%'  or  D3_Status  like  '%ED%'  or  D4_Status  like  '%ED%'  or  D5_Status  like  '%ED%'  or  D6_Status  like  '%ED%'  or  D7_Status  like  '%ED%'  or  D8_Status  like  '%ED%'  or  D9_Status  like  '%ED%'  or  D10_Status  like  '%ED%'  or  D11_Status  like  '%ED%'  or  D12_Status  like  '%ED%'  or  D13_Status  like  '%ED%'  or  D14_Status  like  '%ED%'  or  D15_Status  like  '%ED%'  or  D16_Status  like  '%ED%'  or  D17_Status  like  '%ED%'  or  D18_Status  like  '%ED%'  or  D19_Status  like  '%ED%'  or  D20_Status  like  '%ED%'  or  D21_Status  like  '%ED%'  or  D22_Status  like  '%ED%'  or  D23_Status  like  '%ED%'  or  D24_Status  like  '%ED%'  or  D25_Status  like  '%ED%'  or  D26_Status  like  '%ED%'  or  D27_Status  like  '%ED%'  or  D28_Status  like  '%ED%'  or  D29_Status  like  '%ED%'  or  D30_Status  like  '%ED%'  or  D31_Status like  '%ED%')" +
" GROUP BY D1_Status,D2_Status,D3_Status,D4_Status,D5_Status,D6_Status,D7_Status,D8_Status,D9_Status,D10_Status,D11_Status,D12_Status,D13_Status,D14_Status,D15_Status,D16_Status,D17_Status,D18_Status,D19_Status,D20_Status,D21_Status,D22_Status,D23_Status,D24_Status,D25_Status,D26_Status,D27_Status,D28_Status,D29_Status,D30_Status,D31_Status,UserId,month,year,ShortName,b.Name,d.Name,des.Name,b.StartTime,b.EndTime,sm.OutTime,t.D1,t.D2 ,t.D3,t.D4,t.D5,t.D6,t.D7,t.D8,t.D9,t.D10,t.D11,t.D12,t.D13,t.D14,t.D15,t.D16,t.D17,t.D18,t.D19,t.D19,t.D20,t.D21,t.D22,t.D23,t.D24,t.D25,t.D26,t.D27,t.D28,t.D29,t.D30,t.D31,t.Id";

            return sh.Get_Table_FromQry(qry);
        }
        //Employee Monthly Report
        public DataTable getAllEmpMonthlyReportData()
        {
            string qry = "select ShortName,EmpId from Employees";

            return sh.Get_Table_FromQry(qry);
        }
        public List<LeaveTypes> getAllODLeaveTypes()
        {
            List<LeaveTypes> Leavelst = new List<LeaveTypes>();

            var dt = sh.Get_Table_FromQry("select od.Id,concat(('OD'),'-',(od.ODType)) as Type from OD_Master od" +
                                         " union all select lt.Id, concat(('Leave'),'-',(lt.Code)) as Type from LeaveTypes lt " +
                                         " where lt.Id not in(5,4,6,7,11,12,13) order by Type ");
            Leavelst = dt.AsEnumerable().Select(r => new LeaveTypes
            {
                Id = (Int32)(r["Id"]),
                Type = (string)(r["Type"] ?? "null")
            }).ToList();

            Leavelst.Insert(0, new LeaveTypes
            {
                Id = 0,
                Type = "Internet Issue"
            });
            Leavelst.Insert(1, new LeaveTypes
            {
                Id = 1,
                Type = "Machine Problem"
            });

            return Leavelst;
        }

        public void TimesheetRerunInsert(string branch, string day, string monthyear)
        {
            string branchid = "";
            string deviceid = "";
            string qry = "select BranchId,Device_Id from Branch_Device where BranchName='" + branch + "';";
            var dt = sh.Get_Table_FromQry(qry);
            foreach (DataRow dr in dt.Rows)
            {
                branchid = dr["BranchId"].ToString();
                deviceid = dr["Device_Id"].ToString();
            }
            DateTime str = Convert.ToDateTime(monthyear);
            string str1 = str.ToString("yyyy-MM-dd");
            string[] sa = str1.Split('-');
            string s1 = sa[0];
            string s2 = sa[1];
            if (s2.StartsWith("0"))
            {
                s2 = s2.Substring(1);
            }
            string date = s1 + "-" + s2 + "-" + day;

            string query = "Insert into timesheet_branch_rerun(branch_id,device_id,run_date,active,rerun_status)Values('" + branchid + "','" + deviceid + "','" + date + "',1,'Complete');";
            string query2 = " Update timesheet_run_status set D" + day + "=2 where branch_id='" + branchid + "' and device_id='" + deviceid + "'  and mn = " + s2 + " and yr = " + s1 + "";
            sh.Run_UPDDEL_ExecuteNonQuery(query + query2);
            // return "Inserted Sucessfully";
        }

        //

        public DataTable DCCBEmployeeTimesheetMstdata(string fromdate, string todate, string empcode)

        {
            string qry = "";
            if (fromdate == "")
            {
                qry = "select distinct t.User_id as EmpId,e.shortname as EmpName,des.name as Designation,b.District_Name as district,CONVERT(varchar(30), CAST(MIN(t.io_time) as Date),103 ) as Date, " +
                    "CONVERT(varchar(15), CAST(Min(t.io_time) AS TIME), 100) as EmpCheckInTime, CONVERT(varchar(15), CAST(Max(t.io_time) AS TIME), 100) as EmpCheckOutTime " +
                    "from DCCB_Employees e join timesheet_logs t on t.user_id = e.empid and cast(t.io_time as Date) = '" + fromdate + "' " +
                    "join DCCB_Districts b on b.Id = e.DCCB_district join Designations des on e.designation = des.Id  " +
                    "where year(t.io_time) = Year('" + fromdate + "') and month(t.io_time) = Month('" + fromdate + "') group by e.ShortName ,t.user_id,des.name, " +
                    "b.District_Name";
            }
            else
            {
                DateTime fmdate = DateTime.ParseExact(fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var fmdate1 = fmdate.ToString("yyyy-MM-dd");
                DateTime tdate = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var tdate1 = tdate.ToString("yyyy-MM-dd");
                if (empcode == "")
                {
                    try
                    {
                        //[dbo].[pdccbtimesheet]
                        Log.Debug("DCCB Employee Timesheet Data!!!");
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                        SqlCommand cmd = new SqlCommand("pdccbtimesheet", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fromdate", SqlDbType.VarChar);
                        cmd.Parameters.AddWithValue("@todate", SqlDbType.VarChar);
                        cmd.Parameters["@fromdate"].Value = fmdate1;
                        cmd.Parameters["@todate"].Value = tdate1;
                        cmd.Connection = con;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        qry = "select *from tdccbtimesheet";
                        //return sh.Get_Table_FromQry(qry);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("TimeSheetBusiness.cs DCCBEmployeeTimesheetMstdata() Error :" + ex.Message);
                        Log.Error("TimeSheetBusiness.cs DCCBEmployeeTimesheetMstdata() Error : " + ex.StackTrace);
                    }

                }
                else
                {
                    try
                    {
                        Log.Debug("DCCB Employee Timesheet Data!!!");
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                        SqlCommand cmd = new SqlCommand("pdccbtimesheetwithempid", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@emp_id", SqlDbType.VarChar);
                        cmd.Parameters.AddWithValue("@fromdate", SqlDbType.VarChar);
                        cmd.Parameters.AddWithValue("@todate", SqlDbType.VarChar);
                        cmd.Parameters["@emp_id"].Value = empcode;
                        cmd.Parameters["@fromdate"].Value = fmdate1;
                        cmd.Parameters["@todate"].Value = tdate1;
                        cmd.Connection = con;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        qry = "select *from tdccbtimesheet";
                        // return sh.Get_Table_FromQry(qry);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("TimeSheetBusiness.cs DCCBEmployeeTimesheetMstdata() with empid Error :" + ex.Message);
                        Log.Error("TimeSheetBusiness.cs DCCBEmployeeTimesheetMstdata() with empid Error : " + ex.StackTrace);
                    }
                }
            }



            return sh.Get_Table_FromQry(qry);
        }

        public DataTable GovernmentStaffEmployeeTimesheetMstdata(string fromdate, string todate, string empcode)
        {
            string qry = "";
            if (fromdate == "")
            {
                qry = "select distinct t.User_id as EmpId,g.name as EmpName,des.name as Designation,b.Name as Branch,CONVERT(varchar(30), CAST(MIN(t.io_time) as Date),103 ) as Date, " +
                    "CONVERT(varchar(15), CAST(Min(t.io_time) AS TIME), 100) as EmpCheckInTime, CONVERT(varchar(15), CAST(Max(t.io_time) AS TIME), 100) as EmpCheckOutTime " +
                    "from Government_Staff g join timesheet_logs t on t.user_id = g.empid and cast(t.io_time as Date) = '" + fromdate + "' " +
                    "join Branches b on b.Id = g.branch join Designations des on g.designation = des.Id  " +
                    "where year(t.io_time) = Year('" + fromdate + "') and month(t.io_time) = Month('" + fromdate + "') group by g.Name ,t.user_id,des.name, " +
                    "b.Name";
            }
            else
            {
                DateTime fmdate = DateTime.ParseExact(fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var fmdate1 = fmdate.ToString("yyyy-MM-dd");
                DateTime tdate = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var tdate1 = tdate.ToString("yyyy-MM-dd");
                if (empcode == "")
                {
                    try
                    {
                        //[dbo].[pgovernmentstafftimesheet]
                        Log.Debug("Government Staff Employee Timesheet Data!!!");
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                        SqlCommand cmd = new SqlCommand("pgovernmentstafftimesheet", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fromdate", SqlDbType.VarChar);
                        cmd.Parameters.AddWithValue("@todate", SqlDbType.VarChar);
                        cmd.Parameters["@fromdate"].Value = fmdate1;
                        cmd.Parameters["@todate"].Value = tdate1;
                        cmd.Connection = con;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        qry = "select *from tgovernmentstaff_timesheet";
                        //return sh.Get_Table_FromQry(qry);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("TimeSheetBusiness.cs GovernmentStaffEmployeeTimesheetMstdata() Error :" + ex.Message);
                        Log.Error("TimeSheetBusiness.cs GovernmentStaffEmployeeTimesheetMstdata() Error : " + ex.StackTrace);
                    }

                }
                else
                {
                    try
                    {
                        Log.Debug("Government Staff Timesheet Data!!!");
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                        SqlCommand cmd = new SqlCommand("pgovernmentstafftimesheetwithempid", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@emp_id", SqlDbType.VarChar);
                        cmd.Parameters.AddWithValue("@fromdate", SqlDbType.VarChar);
                        cmd.Parameters.AddWithValue("@todate", SqlDbType.VarChar);
                        cmd.Parameters["@emp_id"].Value = empcode;
                        cmd.Parameters["@fromdate"].Value = fmdate1;
                        cmd.Parameters["@todate"].Value = tdate1;
                        cmd.Connection = con;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        qry = "select *from tgovernmentstaff_timesheet";
                        // return sh.Get_Table_FromQry(qry);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("TimeSheetBusiness.cs GovernmentStaffEmployeeTimesheetMstdata() with empid Error :" + ex.Message);
                        Log.Error("TimeSheetBusiness.cs GovernmentStaffEmployeeTimesheetMstdata() with empid Error : " + ex.StackTrace);
                    }
                }
            }
            return sh.Get_Table_FromQry(qry);
        }


    }
}

