using Entities;
using HRMSBusiness.Db;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMSBusiness.Business
{
    public class WorkDiaryBus
    {
        SqlHelper sh = new SqlHelper();

        public DataTable getAllWorkDiaries(string empid)
        {

            string qry = " SELECT wd.Id,wd.EmpId, workdet.Name,workdet.[Desc], wd.Status,convert(varchar,wd.UpdatedDate,103) as UpdatedDate,"
           + " convert(varchar, wd.WDDate, 103) as WDDate, wd.RefId,ep.ShortName,ds.Code as Designation,"
           + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
           + " FROM WorkDiary wd " +
           " join Employees ep on wd.EmpId = ep.EmpId " +
           " join Designations ds on ds.Id = wd.CurDesig " +
           " join Departments dp on dp.Id = wd.CurDept " +
           " join Branches br on br.Id = wd.CurBr " +
           " join workdiary_det workdet on wd.Id = workdet.WDId " +
           " where ep.EmpId  = " + empid +
           " and  wd.WDDate = convert(date, getDate())" +
           " union all " +
           " Select NULL as Wid,  NULL as Epid, 'Holiday' as Name, Occasion as Description,'' as Status, '' as UpdatedDate," +
           " convert(varchar, Date, 103) as WDDate, '' as RefId,'' as ShortName,'' as Designation, " +
           " '' as BrDepot from HolidayList where [Date] = convert(date, getDate())";
            //" order by workdate desc";

            return sh.Get_Table_FromQry(qry);
        }
        public string deletedesignation(string empid, int id)
        {
            string qry = "delete from Branch_Designation_Mapping where DesignationId = " + id;
            string qry1 = "delete from Designations where id = " + id;
            sh.Run_UPDDEL_ExecuteNonQuery(qry + qry1);
            return "Designation Deleted Sucessfully";
        }
       
        public string addWorkDiary(WorkDiary workdiary, WDViewModel wdvm, string empId, string Cid, string Sid, int lbranch, int ldept, int ldesig)
        {
           
            try
            {
               
                if (wdvm.wddelete=="Delete")
                {
                    string qryInss = "";
                   qryInss += "delete from WorkDiary_Det where id= " + wdvm.WDId + " ;";
                    qryInss+= "delete from WorkDiary where id= " + wdvm.WDId + " and empid="+ empId + ";"; 
                    sh.Run_UPDDEL_ExecuteNonQuery(qryInss);

                    return "Work diary Deleted Successfully";
                }

                if (wdvm.draft == "Draft" && wdvm.ActionType == "N")
                {
                    // Check if a work diary with the same EmpId, WDDate, and Draft status already exists
                    string qryCheck = "SELECT COUNT(*) FROM WorkDiary WHERE EmpId = " + empId + " AND WDDate = '" + String.Format("{0:yyyy-MM-dd}",workdiary.WDDate) + "' AND Status = 'Draft';";
                    int existingDraftCount = sh.Run_INS_ExecuteScalar(qryCheck);

                    if (existingDraftCount > 0)
                    {
                        // If a draft already exists, return a message
                        return "Work diary is already saved in draft.";
                    }
                    else
                    {
                        string qryIns1 = "INSERT INTO WorkDiary([EmpId],[Status],[WDDate],[CA],[SA],[UpdatedDate],[UpdatedBy],[RefId],[Br],[Org],[CurBr],[CurDept],[CurDesig]) "
                                              + "VALUES(" + empId + ",'Draft','" + String.Format("{0:yyyy-MM-dd}'", workdiary.WDDate) + "," + Cid + "," + Sid + "," + "getdate()" + "," + empId + "," + 0 + "," + 0 + "," + 0 + "," + lbranch + "," + ldept + "," + ldesig + ")";
                        qryIns1 += "SELECT CAST(SCOPE_IDENTITY() as int);";

                        var pkid = sh.Run_INS_ExecuteScalar(qryIns1);

                        string qryIns2 = "";
                        for (int i = 0; i < wdvm.workdes.Length; i++)
                        {
                            qryIns2 += "INSERT INTO WorkDiary_Det([Desc],[WDId]) "
                                        + "VALUES('" + wdvm.workdes.GetValue(i).ToString() + "'," + pkid + ");";
                        }
                        qryIns2 += "SELECT 0;";
                        sh.Run_INS_ExecuteScalar(qryIns2);
                        return "Work diary saved Sucessfully";
                    }
                    
                }

                //if (wdvm.ActionType == "N")
                //{
                //    string qryIns1 = "INSERT INTO WorkDiary([EmpId],[Status],[WDDate],[CA],[SA],[UpdatedDate],[UpdatedBy],[RefId],[Br],[Org],[CurBr],[CurDept],[CurDesig]) "
                //                              + "VALUES(" + empId + ",'Pending','" + String.Format("{0:yyyy-MM-dd}'", workdiary.WDDate) + "," + Cid + "," + Sid + "," + "getdate()" + "," + empId + "," + 0 + "," + 0 + "," + 0 + "," + lbranch + "," + ldept + "," + ldesig + ")";
                //    qryIns1 += "SELECT CAST(SCOPE_IDENTITY() as int);";

                //    var pkid = sh.Run_INS_ExecuteScalar(qryIns1);

                //    string qryIns2 = "";
                //    for (int i = 0; i < wdvm.workdes.Length; i++)
                //    {
                //        qryIns2 += "INSERT INTO WorkDiary_Det([Desc],[WDId]) "
                //                    + "VALUES('" + wdvm.workdes.GetValue(i).ToString() + "'," + pkid + ");";
                //    }
                //    qryIns2 += "SELECT 0;";
                //    sh.Run_INS_ExecuteScalar(qryIns2);
                //    return "Work diary Created Sucessfully";
                //    //return "Work diary Created. New Id:" + pkid;
                //}
                if (wdvm.ActionType == "N")
                {
                    // Check if there's a draft record for the selected date
                    string qryCheckDraft = "SELECT Id FROM WorkDiary WHERE EmpId=" + empId + " AND WDDate='" + String.Format("{0:yyyy-MM-dd}", workdiary.WDDate) + "' AND Status='Draft'";
                    var draftId = sh.Run_SEL_ExecuteScalar(qryCheckDraft);

                    if (draftId != null && draftId != DBNull.Value)
                    {
                        // Update the draft record instead of inserting a new one
                        string qryUpdateDraft = "UPDATE WorkDiary SET Status='Pending', UpdatedDate="+"getdate()"+", UpdatedBy=" + empId + " WHERE Id=" + draftId;
                        sh.Run_UPDDEL_ExecuteNonQuery(qryUpdateDraft);

                        string qryUpdateDetails = "Delete from WorkDiary_Det where WDId=" + draftId + ";";
                        for (int i = 0; i < wdvm.workdes.Length; i++)
                        {
                            qryUpdateDetails += "INSERT INTO WorkDiary_Det([Desc],[WDId]) "
                                                + "VALUES('" + wdvm.workdes.GetValue(i).ToString() + "'," + draftId + ");";
                        }
                        qryUpdateDetails += "SELECT 0;";
                        sh.Run_INS_ExecuteScalar(qryUpdateDetails);

                        return "Work diary updated Successfully";
                    }
                    else
                    {
                        // No draft record found, proceed with insert logic
                        string qryIns1 = "INSERT INTO WorkDiary([EmpId],[Status],[WDDate],[CA],[SA],[UpdatedDate],[UpdatedBy],[RefId],[Br],[Org],[CurBr],[CurDept],[CurDesig]) "
                                                          + "VALUES(" + empId + ",'Pending','" + String.Format("{0:yyyy-MM-dd}'", workdiary.WDDate) + "," + Cid + "," + Sid + "," + "getdate()" + "," + empId + "," + 0 + "," + 0 + "," + 0 + "," + lbranch + "," + ldept + "," + ldesig + ")";
                        qryIns1 += "SELECT CAST(SCOPE_IDENTITY() as int);";

                        var pkid = sh.Run_INS_ExecuteScalar(qryIns1);

                        string qryIns2 = "";
                        for (int i = 0; i < wdvm.workdes.Length; i++)
                        {
                            qryIns2 += "INSERT INTO WorkDiary_Det([Desc],[WDId]) "
                                        + "VALUES('" + wdvm.workdes.GetValue(i).ToString() + "'," + pkid + ");";
                        }
                        qryIns2 += "SELECT 0;";
                        sh.Run_INS_ExecuteScalar(qryIns2);

                        return "Work diary created successfully";
                    }
                }
                else
                {

                    string qryIns4 = "";
                    string qrIns5 = "";

                    qrIns5 += "Delete from WorkDiary_Det where WDId=" + wdvm.WDId;
                    sh.Run_UPDDEL_ExecuteNonQuery(qrIns5);
                    string qryIns2 = "";
                    for (int i = 0; i < wdvm.workdes.Length; i++)
                    {
                        qryIns2 += "INSERT INTO WorkDiary_Det([Desc],[WDId]) "
                                    + "VALUES('" + wdvm.workdes.GetValue(i).ToString() + "'," + wdvm.WDId + ");";

                    }
                    qryIns2 += "SELECT 0;";
                    sh.Run_INS_ExecuteScalar(qryIns2);

                    if (wdvm.ActionType == "U" && wdvm.draft == "Draft" && workdiary.Status == "Draft")
                    {
                        qryIns4 += "UPDATE WorkDiary SET Status='Draft',WDDate ='" + workdiary.WDDate + "' where Id= " + wdvm.WDId + ";";
                        sh.Run_UPDDEL_ExecuteNonQuery(qryIns4);

                        return "Work diary Saved Successfully";

                    }

                    if (wdvm.ActionType == "U" && wdvm.draft != "Draft")
                    {
                        qryIns4 += "UPDATE WorkDiary SET Status='Pending',WDDate ='" + workdiary.WDDate + "' where Id= " + wdvm.WDId + ";";
                        sh.Run_UPDDEL_ExecuteNonQuery(qryIns4);

                        return "Work diary updated Successfully";

                    }
                    else
                    {
                        qryIns4 += "UPDATE WorkDiary SET WDDate ='" + workdiary.WDDate + "' where Id= " + wdvm.WDId + ";";
                        sh.Run_UPDDEL_ExecuteNonQuery(qryIns4);

                        return "Work diary Updated Successfully";
                    }
                }

            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
        }

        public DataTable EditWorkDiary(int id)
        {
            SqlHelper sh = new SqlHelper();
            string qryIns1 = "select wd.WDId,w.WDDate,wd.Name as WorkName,wd.[Desc] as WorkDesc"
                 + " FROM WorkDiary w " +
                  " join WorkDiary_Det wd on w.Id=wd.WDId" +
                  "  where wd.WDId=" + id;
            return sh.Get_Table_FromQry(qryIns1);
        }
        public DataTable getWorkApprovalDelete(string EmpId)
        {
            string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, Name =STUFF((SELECT ', ' + Name"
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
            + " join Designations ds on ds.Id = wd.CurDesig "
            + " join Departments dp on dp.Id = wd.CurDept"
            + " join Branches br on br.Id = wd.CurBr"
            + " join workdiary_det workdet on wd.Id = workdet.WDId "
            + "  where wd.EmpId=0 ";
            return sh.Get_Table_FromQry(qry);
        }
        public DataTable getWorkApproval(string EmpId)
        {
            string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + Name"
             + " FROM workdiary_det workdet"
            + " WHERE workdet.wdid =" + " wd.Id" +
            " FOR XML PATH('')), 1, 2, ''), ':' ,"
            + " STUFF((SELECT ', ' + [Desc]"
            + " FROM workdiary_det workdet" +
            " WHERE workdet.wdid =" + " wd.Id"
            + " FOR XML PATH('')), 1, 2, ''))as [Desc]," +
            " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
            + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
            + " FROM WorkDiary wd "
            + "  join Employees ep on wd.EmpId = ep.EmpId"
            + " join Designations ds on ds.Id = wd.CurDesig "
            + " join Departments dp on dp.Id = wd.CurDept"
            + " join Branches br on br.Id = wd.CurBr"
            + " join workdiary_det workdet on wd.Id = workdet.WDId "
            + "  where wd.CA =" + EmpId + " and" + " wd.status in( 'Pending','Approved')" + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " + "order by wd.status desc";
            return sh.Get_Table_FromQry(qry);
        }
        public string checkstatus(string workid, string Id)
        {
            // string lcode = workid.Replace(",", "");
            string lcode = workid.TrimEnd(',');
            string qry = " SELECT status as Status from WorkDiary where CA  =" + Id + " and" + " ID in( " + lcode + ")";
            DataTable dt = sh.Get_Table_FromQry(qry);
            string lcount1 = "";
            if (dt.Rows.Count > 0)
            {
                lcount1 = Convert.ToString(dt.Rows[0]["Status"]);
                return lcount1;
            }
            return lcount1;
        }
        public string ApproveWD(string workid, string Id)
        {
            List<string> lCode = new List<string>();
            if (!string.IsNullOrEmpty(workid))
            {
                lCode = workid.Split(new char[] { ',' }).ToList();
                lCode.Remove("");
            }
            string qryIns2 = "";
            for (int i = 0; i < lCode.Count; i++)
            {
                qryIns2 = "Update WorkDiary Set Status = 'Approved',UpdatedBy =" + Id + " , CA  = " + Id + " where id =" + lCode[i];
                sh.Run_UPDDEL_ExecuteNonQuery(qryIns2);
            }
            return "";
        }

        public string deleteworkdairy1(string workid, string Id)
        {
            List<string> lCode = new List<string>();
            if (!string.IsNullOrEmpty(workid))
            {
                lCode = workid.Split(new char[] { ',' }).ToList();
                lCode.Remove("");
            }
            string qryIns2 = "";
            for (int i = 0; i < lCode.Count; i++)
            {
                qryIns2 = "Delete from WorkDiary where Status = 'Approved' and id =" + lCode[i];
                sh.Run_UPDDEL_ExecuteNonQuery(qryIns2);
            }
            return "";
        }
        public string deleteworkdairy(string workid, string Id)
        {
            List<string> lCode = new List<string>();
            if (!string.IsNullOrEmpty(workid))
            {
                lCode = workid.Split(new char[] { ',' }).ToList();
                lCode.Remove("");
            }
            string qryIns2 = "";
            for (int i = 0; i < lCode.Count; i++)
            {
                qryIns2 = "Delete from WorkDiary where Status = 'Pending' and id =" + lCode[i];
                sh.Run_UPDDEL_ExecuteNonQuery(qryIns2);
            }
            return "";
        }
        public string Admindeleteworkdairy(string workid, string Id)
        {
            List<string> lCode = new List<string>();
            if (!string.IsNullOrEmpty(workid))
            {
                lCode = workid.Split(new char[] { ',' }).ToList();
                lCode.Remove("");
            }
            string qryIns2 = "";
            for (int i = 0; i < lCode.Count; i++)
            {
                qryIns2 = "Delete from WorkDiary where  id =" + lCode[i];
                sh.Run_UPDDEL_ExecuteNonQuery(qryIns2);
            }
            return "";
        }
        public DataTable getWorkToolTip(string Empid)
        {

            string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, Name =STUFF((SELECT  ', ' + Name"
              + " FROM workdiary_det workdet"
             + " WHERE workdet.wdid =" + " wd.Id" +
             " FOR XML PATH('')), 1, 2, ''),"
             + " [Desc] = STUFF((SELECT  ', ' + [Desc]"
             + " FROM workdiary_det workdet" +
             " WHERE workdet.wdid =" + " wd.Id"
             + " FOR XML PATH('')), 1, 2, '')," +
             " wd.Status,convert (varchar,wd.UpdatedDate,103) as UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation ,"
             + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
             + " FROM WorkDiary wd "
             + "  join Employees ep on wd.EmpId = ep.EmpId"
            + " join Designations ds on ds.Id = wd.CurDesig "
            + " join Departments dp on dp.Id = wd.CurDept"
            + " join Branches br on br.Id = wd.CurBr"
             + " join workdiary_det workdet on wd.Id = workdet.WDId "
             + " where wd.Id =" + Empid;
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getMemoToolTip(string Empid)
        {
            string qry = "select lm.Empid,ep.LastName,lm.IssueDate,lm.Duedate,lm.Memodetails,lm.Noofdays,lm.MemoType,lm.Clarification,lm.Responsedate,lm.Status  from Latememo lm join Employees ep on lm.EmpId = ep.EmpId where lm.id=" + Empid;
            return sh.Get_Table_FromQry(qry);
        }

        public DataTable getSanctionWorkApproval(string EmpId)
        {

            string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, Concat(STUFF((SELECT ', ' + Name"
             + " FROM workdiary_det workdet"
            + " WHERE workdet.wdid =" + " wd.Id" +
            " FOR XML PATH('')), 1, 2, ''),' :',"
            + " STUFF((SELECT ', ' + [Desc]"
            + " FROM workdiary_det workdet" +
            " WHERE workdet.wdid =" + " wd.Id"
            + " FOR XML PATH('')), 1, 2, '')) as [Desc]," +
            " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
            + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
            + " FROM WorkDiary wd "
            + "  join Employees ep on wd.EmpId = ep.EmpId"
            + " join Designations ds on ds.Id = wd.CurDesig "
            + " join Departments dp on dp.Id = wd.CurDept"
            + " join Branches br on br.Id = wd.CurBr"
            + " join workdiary_det workdet on wd.Id = workdet.WDId "
            + "  where wd.SA =" + EmpId + " and" + " wd.status in('Approved')" + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " + "order by wd.WDDate desc";
            return sh.Get_Table_FromQry(qry);
        }
        public int GetHoliday(string Date, string ccDate)
        {
            string qry = "SELECT COUNT(Date) as count from holidaylist where Date = ' " + Date + " ' and DeleteAt =  '" + ccDate + " '";
            var dt = sh.Get_Table_FromQry(qry);
            int lcount1 = 0;
            if (dt.Rows.Count > 0)
            {
                lcount1 = Convert.ToInt32(dt.Rows[0]["count"]);
                return lcount1;
            }
            return lcount1;
        }
        public string CheckWorkDairyDate(string Date, string empid,string EmpPkId)
        {
            StringBuilder Source = new StringBuilder();
            string Source1 = "";
            string qry = "select 'Leave' as 'Source', empid,StartDate,EndDate,status "
                + " FROM Leaves where empid = " + EmpPkId + "  and((convert(date,StartDate) >=  '" + Date + "' and convert(date,EndDate) <= '" + Date + "') or(convert(date, EndDate) >=  '" + Date + "' and convert(date, StartDate) <= '" + Date + "'))  and status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited')"
                + " union select 'OD' as 'Source', empid,StartDate,EndDate,status "
                + " FROM OD_OtherDuty where empid = " + EmpPkId + " and((convert(date,StartDate) >=  '" + Date + "' and convert(date,EndDate) <= '" + Date + "') or(convert(date, EndDate) >=  '" + Date + "' and convert(date, StartDate) <= '" + Date + "')) and status not in ('Cancelled', 'PartialCancelled', 'Denied')"
                + " union select 'LTC' as 'Source', empid,StartDate,EndDate,status"
                + " FROM Leaves_LTC where empid = " + EmpPkId + " and((convert(date,StartDate) >=  '" + Date + "' and convert(date,EndDate) <= '" + Date + "') or(convert(date, EndDate) >=  '" + Date + "' and convert(date, StartDate) <= '" + Date + "')) and status not in ('Cancelled', 'PartialCancelled', 'Denied')"
                //+ " union select 'TimeSheet' as 'Source',UserId,ReqDate,UpdatedDate,status"
                //   + " FROM Timesheet_Request_Form where UserId = " + empid + " and((convert(date,ReqDate) >=  '" + StartDate + "' and convert(date,ReqDate) <= '" + EndDate + "') or(convert(date, ReqDate) >=  '" + StartDate + "' and convert(date, ReqDate) <= '" + EndDate + "')) and status not in ('Cancelled','Denied')"
                + " union select 'WD' as 'Source', empid,WDDate,UpdatedDate,status"
               + " FROM WorkDiary where empid = " + empid + " and(WDDate >=  '" + Date + "' AND WDDate <= '" + Date + "') and status not in ('PartialCancelled')";
            DataTable dt = sh.Get_Table_FromQry(qry);
            //if (dt.Rows.Count > 0)
            //{
            foreach (DataRow dr in dt.Rows)
            {
                Source1 = dr["Source"].ToString();
                Source.Append(Source1);
                Source.Append(",");
                Source1 =Convert.ToString(Source);
            }

            //}
            return Source1;
            //string qry = "SELECT COUNT(WDDate) as count from WorkDiary where  WDDate= ' " + Date + " ' AND EmpId = " + empid;
            //var dt = sh.Get_Table_FromQry(qry);
            //int lcount1 = 0;
            //if (dt.Rows.Count > 0)
            //{
            //    lcount1 = Convert.ToInt32(dt.Rows[0]["count"]);
            //    return lcount1;
            //}
            //return lcount1;
        }
        public string getcheckLTCWDOD(string empid, string Empcode, string StartDate, string EndDate, string status)
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

        public string WorkDescription(string lDate, string empid)
        {
            SqlHelper sh = new SqlHelper();
            string qryIns1 = "select wd.[Desc] as WorkDesc"
                 + " FROM WorkDiary w " +
                  " join WorkDiary_Det wd on w.Id=wd.WDId" +
                  "  where empid = " + empid + " and (WDDate >=  '" + lDate + "' AND WDDate <= '" + lDate + "')";
            DataTable dt = sh.Get_Table_FromQry(qryIns1);

            // Convert the DataTable to string
            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                //sb.AppendLine("WorkName: " + row["WorkName"].ToString());
                sb.AppendLine("WorkDesc: " + row["WorkDesc"].ToString());
            }

            return sb.ToString();
        }
    

        public string CheckWorkDairyDates(string Date, string empid,string EmpPkId)
        {
            
            
            string Source = "";
            string qry = "select 'Leave' as 'Source', empid,StartDate,EndDate,status "
                + " FROM Leaves where empid = " + EmpPkId + "  and((convert(date,StartDate) >=  '" + Date + "' and convert(date,EndDate) <= '" + Date + "') or(convert(date, EndDate) >=  '" + Date + "' and convert(date, StartDate) <= '" + Date + "'))  and status not in ('Cancelled', 'PartialCancelled', 'Denied','Credited','Debited')"
                + " union select 'OD' as 'Source', empid,StartDate,EndDate,status "
                + " FROM OD_OtherDuty where empid = " + EmpPkId + " and((convert(date,StartDate) >=  '" + Date + "' and convert(date,EndDate) <= '" + Date + "') or(convert(date, EndDate) >=  '" + Date + "' and convert(date, StartDate) <= '" + Date + "')) and status not in ('Cancelled', 'PartialCancelled', 'Denied')"
                + " union select 'LTC' as 'Source', empid,StartDate,EndDate,status"
                + " FROM Leaves_LTC where empid = " + EmpPkId + " and((convert(date,StartDate) >=  '" + Date + "' and convert(date,EndDate) <= '" + Date + "') or(convert(date, EndDate) >=  '" + Date + "' and convert(date, StartDate) <= '" + Date + "')) and status not in ('Cancelled', 'PartialCancelled', 'Denied')"
                //+ " union select 'TimeSheet' as 'Source',UserId,ReqDate,UpdatedDate,status"
                //   + " FROM Timesheet_Request_Form where UserId = " + empid + " and((convert(date,ReqDate) >=  '" + StartDate + "' and convert(date,ReqDate) <= '" + EndDate + "') or(convert(date, ReqDate) >=  '" + StartDate + "' and convert(date, ReqDate) <= '" + EndDate + "')) and status not in ('Cancelled','Denied')"
                + " union select 'WD' as 'Source', empid,WDDate,UpdatedDate,status"
               + " FROM WorkDiary where empid = " + empid + " and(WDDate >=  '" + Date + "' AND WDDate <= '" + Date + "') and status not in ('PartialCancelled')";
            DataTable dt = sh.Get_Table_FromQry(qry);
            //if (dt.Rows.Count > 0)
            //{
            foreach (DataRow dr in dt.Rows)
            {
                Source = dr["status"].ToString();
                
            }

            //}
            return Source;
            //string lname = "";
            //string qry = "SELECT workdet.Name from WorkDiary wd"
            //    + " join workdiary_det workdet on wd.Id = workdet.WDId "
            //    + " where WDDate= ' " + Date + " ' AND EmpId = " + empid;


            ////string qry = "SELECT COUNT(*) as count  from WorkDiary where WDDate= ' " + Date + " ' AND EmpId=" + empid;
            //var dt = sh.Get_Table_FromQry(qry);

            //if (dt.Rows.Count > 0)
            //{
            //    //string abc = dt.Rows[0]["Name"].ToString();
            //    if (!string.IsNullOrEmpty(dt.Rows[0]["Name"].ToString()))
            //    {
            //        lname = (string)(dt.Rows[0]["Name"]);
            //    }
            //    else
            //    {
            //        lname = "Work";
            //    }

            //    return lname;
            //}
            //return lname;
        }


        //public DataTable SelfallWDNew(string wd, string EndDate, string lworkname, string empid, string status)
        //{

        //    string fdate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");
        //    string tdate = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd");


        //    string qry2 = " Select convert(varchar,w.EmpId) +', '+ emp.Shortname+','+ d.Code  + ',' + case when b.Name='OtherBranch' then dept.Name when b.Name='HeadOffice' then dept.Name else b.Name end as grpcol,FORMAT(w.WDDate,'dd/MM/yyyy') as WorkDate,e.Name as WorkName,e.[Desc] as WorkDescription,w.Status as Status,"
        //              + " case when b.Name='OtherBranch' then dept.Name else b.Name end as BrDepot "
        //                  + " FROM WorkDiary w " +
        //                    "join WorkDiary_Det e on w.Id = e.WDId" +
        //                    " join Employees emp on w.EmpId = emp.EmpId" +
        //                    " join Designations d on w.CurDesig = d.ID" +
        //                    " join Branches b on w.CurBr = b.Id" +
        //                    " join Departments dept on w.CurDept = dept.Id " +
        //                    " Where w.Status!='Draft' " + " and w.EmpId = " + empid +""+
        //                    " and  w.WDDate  >= '" + fdate + "'" +
        //                    " and w.WDDate  <='" + tdate + "'" +
        //                    " union all  Select 'Holiday' as grpcol1, FORMAT([Date],'dd/MM/yyyy')as WorkDate, 'Holiday' as WorkName, Occasion as WorkDescription,'Holiday' as Status, 'Holiday' as BrDepot from HolidayList " +
        //                    " where [Date] >= '" + fdate + "'" +
        //                    " and [Date] <='" + tdate + "'" +
        //                    " order by WorkDate desc";

        //    return sh.Get_Table_FromQry(qry2);
        //}



        public DataTable Workdairysearches(string wd, string EndDate, string lworkname, string empid, string status)
        {
            SqlHelper sh = new SqlHelper();

            if (wd == "" && EndDate == "" && lworkname == "" && status == "")
            {
                string query = " Select w.Id,w.EmpId, emp.Shortname as Name,w.UpdatedDate, d.Code as Designation,b.Name as Branch,dept.Name as Department,w.WDDate as WorkDate,"+
                    " concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription ,w.Status as Status,"
                      + " case when b.Name='OtherBranch' then dept.Name else b.Name end as BrDepot "
                          + " FROM WorkDiary w " +
                            "join WorkDiary_Det e on w.Id = e.WDId" +
                            " join All_Masters M on e.Name = M.Name" +
                            " join Employees emp on w.EmpId = emp.EmpId" +
                            " join Designations d on w.CurDesig = d.ID" +
                            " join Branches b on b.Id = w.CurBr" +
                            " join Departments dept on dept.Id = w.CurDept" +
                            " where w.EmpId = " + empid +
                             " order by w.WDDate desc";
                return sh.Get_Table_FromQry(query);
            }
            else if (wd == "" && EndDate == "" && lworkname == "" && status == "ALL")
            {
                string query = " Select w.Id,w.EmpId, emp.Shortname as Name,w.UpdatedDate, d.Code as Designation,b.Name as Branch,dept.Name as Department,w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when b.Name='OtherBranch' then dept.Name else b.Name end as BrDepot "
                          + " FROM WorkDiary w " +
                            "join WorkDiary_Det e on w.Id = e.WDId" +
                            " join Employees emp on w.EmpId = emp.EmpId" +
                            " join Designations d on w.CurDesig = d.ID" +
                            " join Branches b on b.Id = w.CurBr" +
                            " join Departments dept on dept.Id = w.CurDept" +
                            " where w.EmpId = " + empid +
                            " order by w.WDDate desc";
                return sh.Get_Table_FromQry(query);
            }
            else if (wd != "" && EndDate != "" && lworkname != "" && lworkname != "ALL" && status != "" && status != "ALL")
            {
                string createddate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");
                string enddate = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd");

                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate as UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department, w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                     + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
         + " FROM WorkDiary w " +
         " join Employees ep on w.EmpId = ep.EmpId " +
         " join Designations ds on ds.Id = w.CurDesig " +
         " join Departments dp on dp.Id = w.CurDept " +
         " join Branches br on br.Id = w.CurBr " +
         " join workdiary_det e on w.Id = e.WDId " +
         // " where e.Name = '" + lworkname + "'" +
         //" where w.WDDate = '" + createddate + "'" +
         " where w.WDDate  >= '" + createddate + "'" +
         " and w.WDDate <='" + enddate + "'" +
         " and w.Status = '" + status + "'" +
         " and ep.EmpId  = " + empid +
          " and e.Name = '" + lworkname + "'" +
          " union all Select NULL as Wid," + empid + " as Epid,(select shortname from Employees where empid=" + empid + ")" + "as Name, '' as UpdatedDate," +
          "(select code from Designations d join Employees e on e.CurrentDesignation=d.Id where empid=" + empid + ")" + "as Designation, '' as Branch, '' as Department, " +
          " Date as WorkDate, concat('Holiday ' , Occasion)   as WorkDescription," +
          " '' as Status, " +
          " (case when (select Name from Branches b join Employees e on e.Branch=b.Id where empid=" + empid + ")=" + "'OtherBranch' then" +
        "(select Name from Departments d join Employees e on e.Department = d.Id where empid = " + empid + ")" +
        "else (select Name from Branches b join Employees e on e.Branch = b.Id where empid =" + empid + ")" + "end )as BrDepot " +
          "from HolidayList where [Date] >='" + createddate + "'" +
          " and [Date] <='" + enddate + "'" +
          " order by WorkDate desc";


                return sh.Get_Table_FromQry(qry);
            }
            else if (wd != "" && EndDate != "" && lworkname != "" && lworkname == "ALL" && status != "" && status != "ALL")
            {
                string createddate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");
                string enddate = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd");

                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate as UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department,w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
          + " FROM WorkDiary w " +
          " join Employees ep on w.EmpId = ep.EmpId " +
          " join Designations ds on ds.Id = w.CurDesig " +
          " join Departments dp on dp.Id = w.CurDept " +
          " join Branches br on br.Id = w.CurBr " +
          " join workdiary_det e on w.Id = e.WDId " +
          // " where e.Name = '" + lworkname + "'" +
          //" where w.WDDate = '" + createddate + "'" +




          " where w.Status = '" + status + "'" +
          " and( w.WDDate  >= '" + createddate + "'" +
          " and w.WDDate <='" + enddate + "')" +
          //" where w.Status = '" + status + "'" +
          " and ep.EmpId  = " + empid +
           " union all Select NULL as Wid,  " + empid + " as Epid, (select shortname from Employees where empid=" + empid + ")" + "as Name, '' as UpdatedDate," +
           " (select code from Designations d join Employees e on e.CurrentDesignation=d.Id where empid=" + empid + ")" + "as Designation, '' as Branch, '' as Department, " +
           " Date as WorkDate, 'Holiday:' + Occasion as WorkDescription," +
           " '' as Status," +
           " (case when (select Name from Branches b join Employees e on e.Branch=b.Id where empid=" + empid + ")=" + "'OtherBranch' then" +
        "(select Name from Departments d join Employees e on e.Department = d.Id where empid = " + empid + ")" +
       "else (select Name from Branches b join Employees e on e.Branch = b.Id where empid =" + empid + ")" + "end )as BrDepot " +
           " from HolidayList where [Date] >='" + createddate + "'" +
           " and [Date] <='" + enddate + "'" +
           " order by WorkDate desc";

                return sh.Get_Table_FromQry(qry);
            }
            else if (wd == "" && EndDate == "" && lworkname != "" && lworkname == "ALL" && status != "" && status != "ALL")
            {
                string createddate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");

                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department,w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
          + " FROM WorkDiary w " +
          " join Employees ep on w.EmpId = ep.EmpId " +
          " join Designations ds on ds.Id = w.CurDesig " +
          " join Departments dp on dp.Id = w.CurDept " +
          " join Branches br on br.Id = w.CurBr " +
          " join workdiary_det e on w.Id = e.WDId " +
            // " where e.Name = '" + lworkname + "'" +
            //" where w.WDDate = '" + createddate + "'" +
            " where w.Status = '" + status + "'" +
          " and ep.EmpId  = " + empid +

          " order by w.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (wd == "" && EndDate == "" && lworkname != "" && lworkname == "ALL" && status != "" && status == "ALL")
            {
                // string createddate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");

                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department,w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
          + " FROM WorkDiary w " +
          " join Employees ep on w.EmpId = ep.EmpId " +
          " join Designations ds on ds.Id = w.CurDesig " +
          " join Departments dp on dp.Id = w.CurDept " +
          " join Branches br on br.Id = w.CurBr " +
          " join workdiary_det e on w.Id = e.WDId " +
          // " where e.Name = '" + lworkname + "'" +
          //" where w.WDDate = '" + createddate + "'" +
          //" where w.Status = '" + status + "'" +
          " where ep.EmpId  = " + empid +

          " order by w.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (wd != "" && EndDate != "" && lworkname != "" && lworkname == "ALL" && status != "" && status == "ALL")
            {
                string createddate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");
                string enddate = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd");

                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate as UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department, w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
          + " FROM WorkDiary w " +
          " join Employees ep on w.EmpId = ep.EmpId " +
          " join Designations ds on ds.Id = w.CurDesig " +
          " join Departments dp on dp.Id = w.CurDept " +
          " join Branches br on br.Id = w.CurBr " +
          " join workdiary_det e on w.Id = e.WDId " +
          // " where e.Name = '" + lworkname + "'" +
          //" where w.WDDate = '" + createddate + "'" +
          " where w.WDDate  >= '" + createddate + "'" +
          " and w.WDDate <='" + enddate + "'" +
          //" where w.Status = '" + status + "'" +
          " and ep.EmpId  = " + empid +
           " union all Select NULL as Wid, "+ empid + " as Epid, (select shortname from Employees where empid=" + empid + ")" + "as Name, NULL as UpdatedDate," +
           " (select code from Designations d join Employees e on e.CurrentDesignation=d.Id where empid=" + empid + ")" + "as Designation, (select Name from Branches b join Employees e on e.Branch=b.Id where empid=" + empid + ")" + " as Branch, (select Name from Departments d join Employees e on e.Department=d.Id where empid=" + empid + ")" +  "as Department, " +
           " Date as WorkDate, 'Holiday :' + Occasion as WorkDescription," +
           " '' as Status,   (case when (select Name from Branches b join Employees e on e.Branch=b.Id where empid=" + empid + ")=" + "'OtherBranch' then" +
        "(select Name from Departments d join Employees e on e.Department = d.Id where empid = " + empid + ")" +
	   "else (select Name from Branches b join Employees e on e.Branch = b.Id where empid ="  + empid + ")" + "end )as BrDepot from HolidayList where [Date] >='" + createddate + "'" +
           " and [Date] <='" + enddate + "'" +
           " order by WorkDate desc";

                return sh.Get_Table_FromQry(qry);
            }
            else if (wd != "" && EndDate != "" && lworkname != "" && status == "")
            {
                string createddate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");
                string enddate = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd");

                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department,w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
          + " FROM WorkDiary w " +
          " join Employees ep on w.EmpId = ep.EmpId " +
          " join Designations ds on ds.Id = w.CurDesig " +
          " join Departments dp on dp.Id = w.CurDept " +
          " join Branches br on br.Id = w.CurBr " +
          " join workdiary_det e on w.Id = e.WDId " +
          " where e.Name = '" + lworkname + "'" +
          " and w.WDDate  >= '" + createddate + "'" +
          " and w.WDDate <='" + enddate + "'" +
          " and ep.EmpId  = " + empid +
          " order by w.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (wd == "" && EndDate == "" && lworkname != "" && status != "" && status != "ALL")
            {
                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department,w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
          + " FROM WorkDiary w " +
          " join Employees ep on w.EmpId = ep.EmpId " +
          " join Designations ds on ds.Id = w.CurDesig " +
          " join Departments dp on dp.Id = w.CurDept " +
          " join Branches br on br.Id = w.CurBr " +
          " join workdiary_det e on w.Id = e.WDId " +
          " where e.Name = '" + lworkname + "'" +
          " and ep.EmpId  =  " + empid +
          " and w.Status = '" + status + "'" +
          " order by w.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (wd == "" && lworkname == "" && status != "" && status == "ALL")
            {
                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department,w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
        + " FROM WorkDiary w " +
        " join Employees ep on w.EmpId = ep.EmpId " +
        " join Designations ds on ds.Id = w.CurDesig " +
        " join Departments dp on dp.Id = w.CurDept " +
        " join Branches br on br.Id = w.CurBr " +
        " join workdiary_det e on w.Id = e.WDId " +
        " where ep.EmpId  = " + empid +
        " and w.Status in ('Draft','Pending','Approved')" +
        " and e.Name= '" + lworkname + "'" + "order by w.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (wd != "" && EndDate != "" && lworkname != "" && status != "" && status == "ALL")
            {
                string createddate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");
                string enddate = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd");

                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate as UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department, w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
          + " FROM WorkDiary w " +
          " join Employees ep on w.EmpId = ep.EmpId " +
          " join Designations ds on ds.Id = w.CurDesig " +
          " join Departments dp on dp.Id = w.CurDept " +
          " join Branches br on br.Id = w.CurBr " +
          " join workdiary_det e on w.Id = e.WDId " +
          // " where e.Name = '" + lworkname + "'" +
          //" where w.WDDate = '" + createddate + "'" +
          " where w.WDDate  >= '" + createddate + "'" +
          " and w.WDDate <='" + enddate + "'" +
          " and w.Status in ('Draft','Pending','Approved')" +
            " and e.Name= '" + lworkname + "'" +
          " and ep.EmpId  = " + empid +
           " union all Select NULL as Wid,  " + empid + "  as Epid, (select shortname from Employees where empid=" + empid + ")" + "as Name, NULL as UpdatedDate," +
           " (select code from Designations d join Employees e on e.CurrentDesignation=d.Id where empid=" + empid + ")" + "as Designation, '' as Branch, '' as Department, " +
           " Date as WorkDate, 'Holiday :' +  Occasion as WorkDescription," +
           " '' as Status, " +
           " (case when (select Name from Branches b join Employees e on e.Branch=b.Id where empid=" + empid + ")=" + "'OtherBranch' then" +
           "(select Name from Departments d join Employees e on e.Department = d.Id where empid = " + empid + ")" +
           "else (select Name from Branches b join Employees e on e.Branch = b.Id where empid =" + empid + ")" + "end )as BrDepot " +
           "from HolidayList where [Date] >='" + createddate + "'" +
           " and [Date] <='" + enddate + "'" +
           " order by WorkDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lworkname != "")
            {
                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department,w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
         + " FROM WorkDiary w " +
         " join Employees ep on w.EmpId = ep.EmpId " +
         " join Designations ds on ds.Id = w.CurDesig " +
         " join Departments dp on dp.Id = w.CurDept " +
         " join Branches br on br.Id = w.CurBr " +
         " join workdiary_det e on w.Id = e.WDId " +
         " where e.Name = '" + lworkname + "'" +
         " and ep.EmpId  = " + empid +
         " order by w.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (wd != "" && EndDate != "")
            {
                string createddate = Convert.ToDateTime(wd).ToString("yyyy-MM-dd");
                string enddate = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd");

                string qry = " Select w.Id,w.EmpId, ep.Shortname as Name,w.UpdatedDate, ds.Code as Designation,br.Name as Branch,dp.Name as Department,w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
          + " FROM WorkDiary w " +
          " join Employees ep on w.EmpId = ep.EmpId " +
          " join Designations ds on ds.Id = w.CurDesig " +
          " join Departments dp on dp.Id = w.CurDept " +
          " join Branches br on br.Id = w.CurBr " +
          " join workdiary_det e on w.Id = e.WDId " +
                                //  " where  w.WDDate = '" + createddate + "'" +
                                " where w.WDDate  >= '" + createddate + "'" +
                                " and w.WDDate<='" + enddate + "'" +
                                 " and w.EmpId =" + empid +
                                  " order by w.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (status != "" && status != "ALL")
            {
                string query = " Select w.Id,w.EmpId, emp.Shortname as Name,w.UpdatedDate, d.Code as Designation,b.Name as Branch,dept.Name as Department,w.WDDate as WorkDate," +
                    "concat(STUFF((SELECT ', ' + case when Name = NULL then '' else Name end FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '') ,':', STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = w.Id FOR XML PATH('')), 1, 2, '')) as WorkDescription,w.Status as Status,"
                      + " case when b.Name='OtherBranch' then dept.Name else b.Name end as BrDepot "
                         + " FROM WorkDiary w " +
                           "join WorkDiary_Det e on w.Id = e.WDId" +
                           " join Employees emp on w.EmpId = emp.EmpId" +
                           " join Designations d on w.CurDesig = d.ID" +
                           " join Branches b on b.Id = w.CurBr" +
                           " join Departments dept on dept.Id = w.CurDept" +
                           " where w.Status = '" + status + "'" +
                           " and w.EmpId =" + empid +
                           " order by w.WDDate desc";
                return sh.Get_Table_FromQry(query);
            }
            return null;
        }

        public int CheckTransfers(DateTime? effectiveFromP, DateTime? effectiveFromPT, DateTime? effectiveFromT, DateTime? effectiveToT, string empid, string Type, string Transfer_Type)
        {
            string qry = "";
            string effectivefromP = Convert.ToDateTime(effectiveFromP).ToString("yyyy-MM-dd");
            string effectivefromPT = Convert.ToDateTime(effectiveFromPT).ToString("yyyy-MM-dd");
            string effectivefromT = Convert.ToDateTime(effectiveFromT).ToString("yyyy-MM-dd");
            string effectivetoT = Convert.ToDateTime(effectiveToT).ToString("yyyy-MM-dd");

            if (Transfer_Type == "TemporaryTransfer")
            {
                if (effectiveToT != null)
                {
                    qry = "select count(*) as count from Employee_Transfer where EffectiveFrom='" + effectivefromT + "' and EffectiveTo='" + effectivetoT + "' and EmpId =" + empid + " and Type='" + Transfer_Type + "'";
                }
                else
                {
                    qry = "select count(*) as count from Employee_Transfer where EffectiveFrom='" + effectivefromT + "'  and EmpId =" + empid + " and Type='" + Transfer_Type + "'";
                }
            }
            if (Transfer_Type == "PermanentTransfer")
            {
                qry = "select count(*) as count from Employee_Transfer where EffectiveFrom='" + effectivefromT + "' and EmpId =" + empid + " and Type = '" + Transfer_Type + "'";
            }
            if (Type == "PromotionTransfer")
            {
                qry = "select count(*) as count from Employee_Transfer where EffectiveFrom='" + effectiveFromPT + "' and EmpId =" + empid + " and Type = '" + Type + "'";
            }
            if (Type == "Promotion")
            {
                qry = "select count(*) as count from Employee_Transfer where EffectiveFrom='" + effectivefromP + "' and EmpId =" + empid + " and Type = '" + Type + "'";
            }
            var dt = sh.Get_Table_FromQry(qry);
            int lcount1 = 0;
            if (dt.Rows.Count > 0)
            {
                lcount1 = Convert.ToInt32(dt.Rows[0]["count"]);
                return lcount1;
            }
            return lcount1;
        }
        public DataTable getSanctionWorkApprovalSearch(string EmpId, string lempid, string wddate, string tdate)
        {
            if (lempid != "" && wddate != "" && tdate != "")
            {

                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId,concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc]," +
           " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
           + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
           + " FROM WorkDiary wd "
           + "  join Employees ep on wd.EmpId = ep.EmpId"
           + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
           + " join workdiary_det workdet on wd.Id = workdet.WDId "
           + "  where wd.SA =" + EmpId + " and" + " wd.status =" + " 'Approved'" + " and" + " wd.EmpId =" + lempid + "and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " + "and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
             "and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
             " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate != "" && tdate != "")
            {

                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId,concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc]," +
           " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
           + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
           + " FROM WorkDiary wd "
           + "  join Employees ep on wd.EmpId = ep.EmpId"
           + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
           + " join workdiary_det workdet on wd.Id = workdet.WDId "
           + "  where wd.SA =" + EmpId + " and" + " wd.status =" + " 'Approved'" + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " + "and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
             "and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
             " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else
            {
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc]," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                     + " join Departments dp on dp.Id = wd.CurDept"
                     + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.SA =" + EmpId + "and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " + " and" + " wd.status =" + " 'Approved'" + " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
        }
        public bool UpdateWorkdiaryControlling(int EmpId,string ControllingAuthority,string SanctioningAuthority)
        {
            //string qry = "Select * from WorkDiary where empid="+ EmpId + " and Status not in ('Approved') order by id desc";
            string qry = "update WorkDiary set CA='"+ ControllingAuthority + "',SA='"+ SanctioningAuthority + "' " +
                " where empid="+ EmpId + " and Status !='Approved'";
            return sh.Run_UPDDEL_ExecuteNonQuery(qry);
            
        }
        public DataTable getWorkApprovalSearch(string EmpId, string lempid, string wddate, string tdate, string status)
        {
            if (lempid == "" && wddate == "" && tdate == "" && status == "")
            {
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                " and" + " wd.status in( 'Pending','Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate == "" && tdate != "" && status == "")
            {
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                " and" + " wd.status in( 'Pending','Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate != "" && tdate == "" && status == "")
            {
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                " and" + " wd.status in( 'Pending','Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate == "" && tdate == "" && status == "")
            {
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId,concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId +
                " and" + " wd.EmpId =" + lempid +
                " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                " and" + " wd.status in( 'Pending','Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate != "" && tdate != "" && status == "")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " " +
                "and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                " and" + " wd.status in( 'Pending','Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate == "" && tdate == "" && status != "" && status == "All")
            {
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                " and" + " wd.status in( 'Pending','Approved')" + "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate == "" && tdate == "" && status != "" && status != "All")
            {
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                " and" + " wd.status ='" + status + "'order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate != "" && tdate != "" && status == "")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " and" + " wd.EmpId =" + lempid +
                " and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " + " and" + " wd.status in( 'Pending','Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate == "" && tdate == "" && status != "" && status == "All")
            {
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " and" + " wd.EmpId =" + lempid + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " + " and" + " wd.status in( 'Pending','Approved')" +
                 "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate == "" && tdate == "" && status != "" && status != "All")
            {
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
               + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                 " and" + " wd.EmpId =" + lempid +
                 " and" + " wd.status ='" + status +
                 "'order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate != "" && tdate != "" && status != "" && status == "All")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    " FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
               " and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and" + " wd.status in( 'Pending','Approved')" +
                 "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate != "" && tdate != "" && status != "" && status != "All")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId,concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc], " +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
               + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
               " and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and" + " wd.status ='" + status +
                 "'order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate != "" && tdate != "" && status != "" && status != "All")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
               + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
               "and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and" + " wd.status ='" + status +
                 "' and" + " wd.EmpId =" + lempid +
                 "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate != "" && tdate != "" && status != "" && status == "All")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
               "and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and" + " wd.status in( 'Pending','Approved')" +
                 " and" + " wd.EmpId =" + lempid +
                 "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else
            {
                string fdate = Convert.ToDateTime(wddate).ToString("yyyy-MM-dd");
                string qry = "  SELECT DISTINCT wd.Id,wd.EmpId, concat(STUFF((SELECT ', ' + name FROM workdiary_det workdet " +
                    "WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, ''), ' : ',STUFF((SELECT ', ' + [Desc] " +
                    "FROM workdiary_det workdet WHERE workdet.wdid = wd.Id FOR XML PATH('')), 1, 2, '')) as [Desc] ," +
                " wd.Status,wd.UpdatedDate,wd.WDDate,ep.ShortName,ds.Code as Designation,"
                + " case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot "
                + " FROM WorkDiary wd "
                + "  join Employees ep on wd.EmpId = ep.EmpId"
               + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where wd.CA =" + EmpId + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                " and" + " wd.status in( 'Pending','Approved')" +
                "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
        }
        public DataTable getWorkApprovalSearchDelete(string EmpId, string lempid, string wddate, string tdate, string status)
        {
            if (status == "" || status != "" || status=="undefined")
            {
                status = "Approved";
            }

            if (lempid == "" && wddate == "" && tdate == "")
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where workdet.Name not in ('Leave','OD','LTC')" +
                " and" + " wd.status in( 'Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate == "" && tdate != "")
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where  workdet.Name not in ('Leave','OD','LTC')" +
                " and" + " wd.status in('Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate != "" && tdate == "")
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where  workdet.Name not in ('Leave','OD','LTC')" +
                " and" + " wd.status in('Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate == "" && tdate == "")
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId " +

                " Where" + " wd.EmpId =" + lempid +
                " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                " and" + " wd.status in('Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate != "" && tdate != "")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId " +

                " Where ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " +
                " and" + " wd.status in('Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate == "" && tdate == "")
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "   workdet.Name not in ('Leave','OD','LTC')" +
                " and" + " wd.status in('Approved')" + "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate == "" && tdate == "")
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where  workdet.Name not in ('Leave','OD','LTC')" +
                " and" + " wd.status ='" + status + "'order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate != "" && tdate != "")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where  wd.EmpId =" + lempid +
                " and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " + " and" + " wd.status in('Approved')" +
                 " order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate == "" && tdate == "" && status != "" && status == "All")
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where  wd.EmpId =" + lempid + " and (workdet.Name not in ('Leave','OD','LTC') or workdet.Name  is null) " + " and" + " wd.status in( 'Pending','Approved')" +
                 "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate == "" && tdate == "" && status != "" && status != "All")
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
               + "  where  workdet.Name not in ('Leave','OD','LTC')" +
                 " and" + " wd.EmpId =" + lempid +
                 " and" + " wd.status ='" + status +
                 "'order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate != "" && tdate != "" && status != "" && status == "All")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where  workdet.Name not in ('Leave','OD','LTC')" +
               " and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and" + " wd.status in( 'Pending','Approved')" +
                 "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid == "" && wddate != "" && tdate != "" && status != "" && status != "All")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
               + "  where  workdet.Name not in ('Leave','OD','LTC')" +
               " and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and" + " wd.status ='" + status +
                 "'order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate != "" && tdate != "" && status != "" && status != "All")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
               + "  where  workdet.Name not in ('Leave','OD','LTC')" +
               "and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and" + " wd.status ='" + status +
                 "' and" + " wd.EmpId =" + lempid +
                 "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else if (lempid != "" && wddate != "" && tdate != "" && status != "" && status == "All")
            {
                DateTime fdate = wddate == "" ? DateTime.Now : Convert.ToDateTime(wddate);
                DateTime todate = tdate == "" ? DateTime.Now : Convert.ToDateTime(tdate);
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
                + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where  workdet.Name not in ('Leave','OD','LTC')" +
               "and ((wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "') or  (wd.WDDate >= '" + fdate.ToString("yyyy-MM-dd 00:00:00.000") + "' and wd.WDDate <= '" + todate.ToString("yyyy-MM-dd 23:59:59.000") + "'))" +
                " and" + " wd.status in('Approved')" +
                 " and" + " wd.EmpId =" + lempid +
                 "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
            else
            {
                string fdate = Convert.ToDateTime(wddate).ToString("yyyy-MM-dd");
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
               + " join Designations ds on ds.Id = wd.CurDesig "
                + " join Departments dp on dp.Id = wd.CurDept"
                + " join Branches br on br.Id = wd.CurBr"
                + " join workdiary_det workdet on wd.Id = workdet.WDId "
                + "  where  workdet.Name not in ('Leave','OD','LTC')" +
                " and" + " wd.status in('Approved')" +
                "order by wd.WDDate desc";
                return sh.Get_Table_FromQry(qry);
            }
        }
    }
}

