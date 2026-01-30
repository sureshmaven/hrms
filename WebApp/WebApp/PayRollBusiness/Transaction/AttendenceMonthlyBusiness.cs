using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.DAL.Business;
using Mavensoft.DAL.Db;
using System.Data;
using Mavensoft.Common;
using PayrollModels;
using PayrollModels.Transactions;
using Newtonsoft.Json;

namespace PayRollBusiness.Transaction
{
    public class AttendenceMonthlyBusiness : BusinessBase
    {
        public AttendenceMonthlyBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        SqlHelperAsync _sha = new SqlHelperAsync();
        // get  weekdays ,paid days,FM
        public async Task<DataTable> getPayableDays()
        {
            string query = "SELECT paid_holidays,fy,fm,week_holidays  FROM pr_month_details WHERE  Active = 1";
            return await _sha.Get_Table_FromQry(query);
        }

        public async Task<IList<Getmonthdays>> monthdays()
        {
            string qrySel = "SELECT month_days FROM pr_month_details WHERE  Active = 1";

            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<Getmonthdays> lstDept = new List<Getmonthdays>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new Getmonthdays
                    {
                        month_days = dr["month_days"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }

        public async Task<string> SearchEmpDetails(string empcode)
        {

            var employeeLeaveData = await getmonthattendenceleaves(empcode);//get leaveTypes balence  details
            
            var StatusData = await searchStatusLeaves(empcode); //status

            var getmonthdays = await monthdays(); //monthdays

            var Ldata = JsonConvert.SerializeObject(employeeLeaveData);
            var Sdata = JsonConvert.SerializeObject(StatusData);

            var monthday = JsonConvert.SerializeObject(getmonthdays);

            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var MLeaveData = javaScriptSerializer.DeserializeObject(Ldata);
            var EStatusData = javaScriptSerializer.DeserializeObject(Sdata);

            var Mdays = javaScriptSerializer.DeserializeObject(monthday);

            var resultJson = javaScriptSerializer.Serialize(new { EmpLDetails = MLeaveData, StatusDetails = EStatusData, Mday = Mdays });
            return JsonConvert.SerializeObject(resultJson);
        }

        public async Task<DataTable> searchEmpMonthLeaveDetails(string empcode)
        {
            string query = "select id,lop_days, absent_days, working_days, status ,status_date,sus_per from pr_month_attendance where emp_code =" + empcode + " and active = 1";
            return await _sha.Get_Table_FromQry(query);
        }

        public async Task<IList<MonthlyAttendance>> searchStatusLeaves(string EmpCode)
        {
            string qry = "select DOJ from Employees where EmpId= "+ EmpCode + " ";
            DataTable dt2 = await _sha.Get_Table_FromQry(qry);
            if (dt2.Rows.Count >0)
            {
                string doj1 = dt2.Rows[0]["DOJ"].ToString();
                DateTime fm = _LoginCredential.FinancialMonthDate;
                DateTime doj = Convert.ToDateTime(doj1);
                //int doj = Convert.ToInt32(doj1.ToString());
             
                string qr = "select * from pr_month_attendance where emp_code=" + EmpCode + " ";
                DataTable dt3 = await _sha.Get_Table_FromQry(qr);
                if (dt3.Rows.Count == 0)
                {
                    if (doj <= fm)
                    {

                        int month = _LoginCredential.FM;//  _LoginCredential.FM.; //30;
                        int totPayableDays = DateTime.DaysInMonth(_LoginCredential.FY, _LoginCredential.FM);
                        double lop_days = (Convert.ToDateTime(fm) - Convert.ToDateTime(doj)).TotalDays;
                        float absent_days = 0;
                        double working_days = totPayableDays - lop_days;
                        var dfdata = "Regular";
                        
                        string insertQry = "";
                        int fy = _LoginCredential.FY;
                        string sFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
                        int Id = 0;
                        int NewNumIndex = 0;
                        NewNumIndex++;
                        StringBuilder sbqry = new StringBuilder();
                        string dtwd = await GetLeaveBalance(EmpCode.ToString());
                        string LeaveBalance = dtwd.TrimEnd(',');
                        var LeaveBalances = LeaveBalance.Split(',');
                        float suspercent1 = 0.0f;

                        foreach (var leavebal in LeaveBalances)
                        {
                            var arremp2 = leavebal.Split('#');
                        }
                        string leaves_available = dtwd.TrimEnd(',');
                        //1. trans_id
                        sbqry.Append(GenNewTransactionString());
                        sbqry.Append(GetNewNumStringArr("pr_month_attendance", NewNumIndex));

                        insertQry = "INSERT into pr_month_attendance([id],fy,fm,emp_code,emp_id,status,status_date,leaves_available,lop_days,absent_days,working_days,active,trans_id,sus_per)" +
                          "values(@idnew" + NewNumIndex + "," + fy + ",'" + sFm + "'," + EmpCode + "," +
                          "(select id from Employees where EmpId=" + EmpCode + "),'" + dfdata + "',null,'" + leaves_available + "'," + lop_days + "," + absent_days + "," + working_days + ",1,@transidnew, " + suspercent1 + " )";
                        //insertQry = "INSERT INTO pr_month_attendance VALUES(@idnew" + NewNumIndex + "," + fy + ",'" + fm + "'," + empcode + "," + EmpCode + "," + Designation + "," + loantype + "," + Totalamt + "," + Noofinstall + "," + Intinstallment + ",'" + Sanctiondate + "'," + Method + "," + Intrate + "," + Instalamount + "," + Recoveramount + "," + Completedinstall + ",'" + Loanstartfrom + "','" + Loanvendorname + "'," + Active + "," + Trans_id + ");";

                        //4. transaction touch
                        //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_attendance", "@idnew" + NewNumIndex, ""));
                        sbqry.Append(insertQry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_attendance", "@idnew" + NewNumIndex, ""));

                        if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                        {
                            //return "I#Attendance Monthly# Data Updated Successfully.";
                        }

                    }
                }
            }
            
            string query = "SELECT elb.Id,emp.id ,elb.emp_id,elb.emp_code,elb.lop_days,elb.absent_days,elb.working_days,elb.status," +
                "elb.status_date,case when elb.id is null then 'N' else 'U' end as row_type, " +
                " elb.sus_per,case when elb.id is null then 'N' else 'U' end as row_type " +
                "from pr_month_attendance elb  join employees emp on elb.emp_code = emp.EmpId where emp.EmpId =" + EmpCode + " and active = 1";
            //string query = "select id,lop_days, absent_days, working_days, status ,status_date from pr_month_attendance where emp_id =" + EmpCode + " and active = 1";
            DataTable dt = await _sha.Get_Table_FromQry(query);
            List<MonthlyAttendance> lstEmpData = new List<MonthlyAttendance>();
            try
            {
                if (dt.Rows.Count > 0)
                {
                    //id  FirstName, CurrentDesignation, Department
                    lstEmpData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "lop_days",
                        display = "LOP days",
                        Value = dt.Rows[0]["lop_days"].ToString()
                    });
                    lstEmpData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "absent_days",
                        display = "Absent Days",
                        Value = dt.Rows[0]["absent_days"].ToString()
                    });
                    lstEmpData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "working_days",
                        display = "Working Days",
                        Value = dt.Rows[0]["working_days"].ToString()
                    });
                    lstEmpData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "status",
                        display = "status",
                        Value = dt.Rows[0]["status"].ToString()
                    });
                    lstEmpData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "status_date",
                        display = "status_date",
                        Value = Convert.ToDateTime(dt.Rows[0]["status_date"]).ToString("dd/MM/yyyy")
                    });

                    lstEmpData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "sus_per",
                        display = "sus_per",
                        Value = dt.Rows[0]["sus_per"].ToString()
                    });

                    //Active
                    lstEmpData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "active",
                        //display = "Active",
                        Value = dt.Rows[0]["active"].ToString()
                    });

                }
            }
            catch (Exception ex)
            {

            }
            return lstEmpData;
        }
        public async Task<IList<MonthlyAttendance>> getmonthattendenceleaves(string EmpCode)
        {
            string query = " SELECT elb.Id, elb.EmpId,elb.CasualLeave,elb.MedicalSickLeave,elb.PrivilegeLeave,elb.MaternityLeave," +
                "elb.PaternityLeave,elb.ExtraordinaryLeave,elb.SpecialCasualLeave,elb.CompensatoryOff,elb.LOP,case " +
                "when elb.rowid = 1 then 'N' else 'U' end as row_type from V_EmpLeaveBalance elb join employees emp " +
                "on elb.EmpId = emp.id where emp.EmpId = " + EmpCode + "";

            DataTable dt = await _sha.Get_Table_FromQry(query);
            IList<MonthlyAttendance> lstEmpLeaveData = new List<MonthlyAttendance>();
            try
            {
                if (dt.Rows.Count > 0)
                {
                    //id  FirstName, CurrentDesignation, Department

                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "CL ",
                        display = "CL ",
                        Value = dt.Rows[0]["CasualLeave"].ToString()
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "ML ",
                        Value = dt.Rows[0]["MedicalSickLeave"].ToString()
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "PL ",
                        Value = dt.Rows[0]["PrivilegeLeave"].ToString()
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "SCL ",
                        Value = dt.Rows[0]["SpecialCasualLeave"].ToString()
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "MTL ",
                        Value = dt.Rows[0]["MaternityLeave"].ToString()
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "PTL ",
                        Value = dt.Rows[0]["PaternityLeave"].ToString()
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "EOL ",
                        Value = dt.Rows[0]["ExtraordinaryLeave"].ToString()
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "C-OFF ",
                        Value = dt.Rows[0]["CompensatoryOff"].ToString()
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                        Name = "LOP ",
                        Value = dt.Rows[0]["LOP"].ToString()
                    });
                }
                else
                {
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = 0,
                        Name = "CL",
                        Value = ""
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = 0,
                        Name = "ML",
                        Value = ""
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = 0,
                        Name = "PL ",
                        Value = ""
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = 0,
                        Name = "SCL ",
                        Value = ""
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = 0,
                        Name = "MTL",
                        Value = ""
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = 0,
                        Name = "PTL",
                        Value = ""
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = 0,
                        Name = "EOL",
                        Value = ""
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = 0,
                        Name = "C-OFF",
                        Value = ""
                    });
                    lstEmpLeaveData.Add(new MonthlyAttendance
                    {
                        Id = 0,
                        Name = "LOP",
                        Value = ""
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstEmpLeaveData;
        }
        //Total Payable Days
        public async Task<DataTable> getmonthdetals(string monthyear)
        {
            string query = "SELECT * FROM pr_month_details WHERE fm=" + monthyear;
            return await _sha.Get_Table_FromQry(query);
        }
        // get leave balence
        public async Task<string> GetLeaveBalance(string empid)
        {
            string retStr = "";
            var qry = " SELECT id,EmpId,CasualLeave,MedicalSickLeave,PrivilegeLeave,MaternityLeave,PaternityLeave,ExtraordinaryLeave,SpecialCasualLeave,CompensatoryOff,LOP " +
                "FROM V_EmpLeaveBalance " +
                "WHERE empid=(select id from Employees where EmpId=" + empid + ") ";
            DataTable dt = await _sha.Get_Table_FromQry(qry);
            foreach (DataRow dr in dt.Rows)
            {

                retStr = retStr + "CL" + "#" + dr["CasualLeave"].ToString() + ',' + "ML" + "#" + dr["MedicalSickLeave"].ToString() + ',' + "PL" + "#" + dr["PrivilegeLeave"].ToString() + ',' + "MTL" + "#" + dr["MaternityLeave"].ToString() + ',' + "PTL" + "#" + dr["PaternityLeave"].ToString() + ',' + "EOL" + "#" + dr["ExtraordinaryLeave"].ToString() + ',' + "SCL" + "#" + dr["SpecialCasualLeave"].ToString() + ',' + "C-OFF" + "#" + dr["CompensatoryOff"].ToString() + ',' + "LOP" + "#" + dr["LOP"].ToString() + ",";
            }
            if (retStr == "")
            {
                retStr = "No Leave Balance ";
            }
            return retStr;
        }
        //insert and update details

        public async Task<string> InsertupdateAttendence(UpdateDetails Values)
        {
            string empcode = Values.EmpId;
            string dtwd = await GetLeaveBalance(empcode.ToString());
            string LeaveBalance = dtwd.TrimEnd(',');
            var LeaveBalances = LeaveBalance.Split(',');
            foreach (var leavebal in LeaveBalances)
            {
                var arremp2 = leavebal.Split('#');
            }
            string leaves_available = dtwd.TrimEnd(',');

            try
            {
                //string empcode = Values.EmpId;

                float lop_days = Values.lop_days;
                float absent_days = Values.absent_days;
                float working_days = Values.working_days;

                var dfdata = Values.status;
                var sdata = "";
                var suspercent = "";
                float suspercent1 = 0.0f;
                                               
                if (Values.status_date == null)
                {
                    if (Values.suspend_percent.ToString() == null)
                    {
                        suspercent = null;
                    }
                    else
                    {
                        sdata = null;
                    }
                }
                else
                {
                    if (Values.status_date != null)
                    {
                        if (Values.suspend_percent.ToString() != null)
                        {
                            suspercent1 = Values.suspend_percent;
                            DateTime sdata1 = DateTime.Parse(Values.status_date);
                            sdata = sdata1.ToString("MM-dd-yyyy");
                        }

                        else
                        {
                            DateTime sdata1 = DateTime.Parse(Values.status_date);
                            sdata = sdata1.ToString("MM-dd-yyyy");
                        }
                    }


                }
                string emp_id = empcode;
                string insertQry = "";
                int fy = _LoginCredential.FY;
                string sFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
                int Id = 0;

                int NewNumIndex = 0;
                NewNumIndex++;
                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());

                insertQry += " UPDATE pr_month_attendance SET active=0 WHERE emp_code=" + emp_id + " and active=1;";
                sbqry.Append(insertQry);
                //4. transaction touch
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_month_attendance", Id.ToString(), ""));
                //2. gen new num
                sbqry.Append(GetNewNumStringArr("pr_month_attendance", NewNumIndex));
                if (Values.status_date == null)
                {
                    if (Values.suspend_percent.ToString() == null)
                    {
                        insertQry = "INSERT into pr_month_attendance([id],fy,fm,emp_code,emp_id,status,status_date,leaves_available,lop_da" +
                        "" +
                        "" +
                        "ys,absent_days,working_days,active,trans_id,sus_per)" +
                    "values(@idnew" + NewNumIndex + "," + fy + ", '" + sFm + "'," + empcode + ",(select id from Employees where EmpId=" + empcode + "),'" + dfdata + "',NULL,'" + leaves_available + "'," + lop_days + "," + absent_days + "," + working_days + ",1,@transidnew,'NULL' )";
                    }

                    else
                    {
                        insertQry = "INSERT into pr_month_attendance([id],fy,fm,emp_code,emp_id,status,status_date,leaves_available,lop_da" +
                            "" +
                            "" +
                            "ys,absent_days,working_days,active,trans_id)" +
                        "values(@idnew" + NewNumIndex + "," + fy + ", '" + sFm + "'," + empcode + ",(select id from Employees where EmpId=" + empcode + "),'" + dfdata + "',NULL,'" + leaves_available + "'," + lop_days + "," + absent_days + "," + working_days + ",1,@transidnew )";
                    }
                }
                else
                {
                    if (Values.status_date != null)
                    {
                        if (Values.suspend_percent.ToString() != null)
                        {
                            insertQry = "INSERT into pr_month_attendance([id],fy,fm,emp_code,emp_id,status,status_date,leaves_available,lop_days,absent_days,working_days,active,trans_id,sus_per)" +
                       "values(@idnew" + NewNumIndex + "," + fy + ",'" + sFm + "'," + empcode + ",(select id from Employees where EmpId=" + empcode + "),'" + dfdata + "','" + sdata + "','" + leaves_available + "'," + lop_days + "," + absent_days + "," + working_days + ",1,@transidnew, " + suspercent1 + " )";
                        }
                        else
                        {
                            insertQry = "INSERT into pr_month_attendance([id],fy,fm,emp_code,emp_id,status,status_date,leaves_available,lop_days,absent_days,working_days,active,trans_id)" +
                            "values(@idnew" + NewNumIndex + "," + fy + ",'" + sFm + "'," + empcode + ",(select id from Employees where EmpId=" + empcode + "),'" + dfdata + "','" + sdata + "','" + leaves_available + "'," + lop_days + "," + absent_days + "," + working_days + ",1,@transidnew )";
                        }
                    }

                }
                //insertQry = "INSERT INTO pr_month_attendance VALUES(@idnew" + NewNumIndex + "," + fy + ",'" + fm + "'," + empcode + "," + EmpCode + "," + Designation + "," + loantype + "," + Totalamt + "," + Noofinstall + "," + Intinstallment + ",'" + Sanctiondate + "'," + Method + "," + Intrate + "," + Instalamount + "," + Recoveramount + "," + Completedinstall + ",'" + Loanstartfrom + "','" + Loanvendorname + "'," + Active + "," + Trans_id + ");";
                sbqry.Append(insertQry);

                //4. transaction touch
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_attendance", "@idnew" + NewNumIndex, ""));

                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#Attendance Monthly# Data Updated Successfully.";
                }
                else
                {
                    return "E#Attendance#Pleace enter numbers only not characters and Special characters";
                }
            }

            catch (Exception e)
            {
                string msg = e.Message;
                return "E#Error:#" + msg;
            }

        }
    }

    public class MonthlyAttendance
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string rowType { get; set; }
        public int Id { get; set; }
        public string display { get; set; }


    }
}
