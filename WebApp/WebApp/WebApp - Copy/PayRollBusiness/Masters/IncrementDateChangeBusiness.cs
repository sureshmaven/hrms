using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using PayrollModels.Masters;
using PayrollModels.Transactions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Masters
{
   public class IncrementDateChangeBusiness : BusinessBase
    {
        public IncrementDateChangeBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        public async Task<IList<CommonGetModel>> getEmpDetails()
        {
            string qrySel = "select distinct e.EmpId as Id, e.ShortName as name,Case when b.name = 'OtherBranch' then d.name else b.name end as Branch,desig.Name AS Designation,m.Increment_Date_WEF,m.increment_date as IncDate,c.inc_date_change as inc_date_change,c.remarks " +
                  "from pr_emp_inc_anual_stag m left outer join pr_emp_inc_date_change c on m.emp_code=c.emp_code and c.active=1 join Employees e on e.EmpId=m.emp_code JOIN Branches b on e.Branch = b.Id join departments d on e.Department=d.id join Designations desig on e.CurrentDesignation = desig.Id AND m.process = 0 AND m.authorisation = 0 " +
                  "WHERE(select month(fm) FROM pr_month_details WHERE active = 1) = month(m.increment_date) " +
                  "AND e.RetirementDate>=(select fm from  pr_month_details where active=1) and m.Increment_Date_WEF is not null   order by e.EmpId asc;";

            //string qrySel = "SELECT distinct  Employees.EmpId as Id ,Employees.ShortName as name,Designations.Name AS Designation,pr_emp_inc_date_change.inc_date as IncDate ," +
            //    //"pr_emp_inc_anual_stag.process ," +
            //    "pr_emp_inc_date_change.emp_code, Case when Branches.name = 'OtherBranch' then Departments.name else Branches.name end as Branch ," +
            //    "pr_emp_inc_date_change.remarks ,pr_emp_inc_date_change.inc_date_change as inc_date_change FROM Employees " +
            //    //"JOIN pr_emp_inc_anual_stag on pr_emp_inc_anual_stag.emp_id = pr_emp_inc_anual_stag.emp_id " +
            //    "JOIN Designations on Employees.CurrentDesignation = Designations.Id " +
            //    "JOIN Departments on Employees.Department = Departments.Id " +
            //    "JOIN Branches on Employees.Branch = Branches.Id " +
            //    "LEFT JOIN pr_emp_inc_date_change on Employees.EmpId = pr_emp_inc_date_change.emp_code AND pr_emp_inc_date_change.active = 1 " +
            //    "AND pr_emp_inc_date_change.process = 0 AND pr_emp_inc_date_change.authorisation = 0 " +
            //    //"AND pr_emp_inc_anual_stag.process = 0 " +
            //    "WHERE(select month(fm) FROM pr_month_details WHERE active = 1) = month(pr_emp_inc_date_change.inc_date) " +
            //    "AND (select year(fm) FROM pr_month_details WHERE active = 1) = year(pr_emp_inc_date_change.inc_date) AND Employees.RetirementDate >=getdate() ";


            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<CommonGetModel> lstDept = new List<CommonGetModel>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["inc_date_change"].ToString() == "")
                    {
                        lstDept.Add(new CommonGetModel
                        {
                            Id = dr["Id"].ToString(),
                            Name = dr["name"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            Branch = dr["Branch"].ToString().ToString(),
                            Increment_Date_WEF = Convert.ToDateTime(dr["Increment_Date_WEF"]).ToString("dd/MM/yyyy"),
                            //IncDate = dr["IncDate"].ToString()
                            IncDate = Convert.ToDateTime(dr["IncDate"]).ToString("dd/MM/yyyy"),
                            remarks = dr["remarks"].ToString(),
                            change_inc_date = "dd-MM-yyyy",
                        });
                    }

                    else
                    {
                        lstDept.Add(new CommonGetModel
                        {
                            Id = dr["Id"].ToString(),
                            Name = dr["name"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            Branch = dr["Branch"].ToString().ToString(),
                            Increment_Date_WEF = Convert.ToDateTime(dr["Increment_Date_WEF"]).ToString("dd/MM/yyyy"),
                            //IncDate = dr["IncDate"].ToString()
                            IncDate = Convert.ToDateTime(dr["IncDate"]).ToString("dd/MM/yyyy"),
                            remarks = dr["remarks"].ToString(),
                            change_inc_date = Convert.ToDateTime(dr["inc_date_change"]).ToString("dd/MM/yyyy")
                        });

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }

        public async Task<IList<Getmonthdays>> getmonthDays()
        {
            string qrySel = "SELECT fm FROM pr_month_details WHERE  Active = 1";
            //return await _sha.Get_Table_FromQry(query);
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<Getmonthdays> lstDept = new List<Getmonthdays>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new Getmonthdays
                    {
                        month_days = Convert.ToDateTime(dr["fm"]).ToString("dd/MM/yyyy"),
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }
        // search
        public async Task<IList<CommonGetModel>> searchIncrementDateChange(string SearchData)
        {
            DateTime str = Convert.ToDateTime(SearchData);
            string str1 = str.ToString("yyyy-MM-dd");
            string[] sa = str1.Split('-');
            string s1 = sa[0];
            string s2 = sa[1];
            if (s2.StartsWith("0"))
            {
                s2 = s2.Substring(1);
            }
            string s3 = sa[2];
            //cond = " WHERE (b.name='" + branch + "'" + "  or d.name='" + branch + "')" + "AND month = " + s2 + " AND year = " + s1 + "AND  empid=" + EmpIds;

            string qrySel = "select distinct e.ShortName as name,e.EmpId as Id, Case when b.name = 'OtherBranch' then d.name else b.name end as Branch,desig.Name AS Designation,m.Increment_Date_WEF,m.increment_date as IncDate,c.inc_date_change as inc_date_change,c.remarks " +
                  "from pr_emp_inc_anual_stag m left outer join pr_emp_inc_date_change c on m.emp_code=c.emp_code  join Employees e on e.EmpId=m.emp_code JOIN Branches b on e.Branch = b.Id JOIN Departments d on e.Department = d.Id join Designations desig on desig.id=e.currentdesignation  AND m.process = 0 AND m.authorisation = 0 " +
                  " WHERE e.RetirementDate >=getdate() and month(increment_date)=" + s2 + " and m.Increment_Date_WEF is not null order by id; ";


            //string qrySel = "select Employees.EmpId as Id ,Employees.ShortName as name,Designations.Name AS Designation,pr_emp_inc_date_change.inc_date as IncDate , pr_emp_inc_date_change.inc_date_change , pr_emp_inc_date_change.emp_code,  " +
            //    "Case when Branches.name = 'OtherBranch' then Departments.name else Branches.name end as Branch, pr_emp_inc_date_change.remarks  " +
            //    "FROM Employees JOIN Designations on Employees.CurrentDesignation = Designations.Id JOIN Departments on Employees.Department = Departments.Id JOIN Branches " +
            //    "on Employees.Branch = Branches.Id left JOIN pr_emp_inc_date_change on Employees.EmpId = pr_emp_inc_date_change.emp_code   AND pr_emp_inc_date_change.active = 1 AND pr_emp_inc_date_change.process = 0" +
            //    " WHERE pr_emp_inc_date_change.active=1 AND  month(inc_date)=" + s2 + " AND YEAR(inc_date)='" + s1 + "'; ";

            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<CommonGetModel> lstDept = new List<CommonGetModel>();
            try
            {
                //foreach (DataRow dr in dt.Rows)
                //{
                //    lstDept.Add(new CommonGetModel
                //    {
                //        Id = dr["Id"].ToString(),
                //        Name = dr["name"].ToString(),
                //        Designation = dr["Designation"].ToString(),
                //        Branch = dr["Branch"].ToString().ToString(),
                //        //IncDate = dr["IncDate"].ToString()
                //        IncDate = Convert.ToDateTime(dr["IncDate"]).ToString("dd/MM/yyyy"),
                //        remarks = dr["remarks"].ToString()
                //    });
                //}
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["inc_date_change"].ToString() == "")
                    {
                        lstDept.Add(new CommonGetModel
                        {
                            Id = dr["Id"].ToString(),
                            Name = dr["name"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            Branch = dr["Branch"].ToString().ToString(),
                            Increment_Date_WEF = Convert.ToDateTime(dr["Increment_Date_WEF"]).ToString("dd/MM/yyyy"),
                            //IncDate = dr["IncDate"].ToString()
                            IncDate = Convert.ToDateTime(dr["IncDate"]).ToString("dd/MM/yyyy"),
                            remarks = dr["remarks"].ToString(),
                            change_inc_date = "dd-MM-yyyy",
                        });
                    }

                    else
                    {
                        lstDept.Add(new CommonGetModel
                        {
                            Id = dr["Id"].ToString(),
                            Name = dr["name"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            Branch = dr["Branch"].ToString().ToString(),
                            Increment_Date_WEF = Convert.ToDateTime(dr["Increment_Date_WEF"]).ToString("dd/MM/yyyy"),
                            //IncDate = dr["IncDate"].ToString()
                            IncDate = Convert.ToDateTime(dr["IncDate"]).ToString("dd/MM/yyyy"),
                            remarks = dr["remarks"].ToString(),
                            change_inc_date = Convert.ToDateTime(dr["inc_date_change"]).ToString("dd/MM/yyyy")
                        });

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }
        //UpdateIncrementDateChange
        public async Task<string> UpdateIncrementDateChange(List<IncrementDateChangeModel> Values)
        {
            string retMessage = "";
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            try
            {
                string changeData = getchangeDataData(Values);
                string empids = changeData.TrimEnd(',');
                if (empids != "")
                {
                    var arrEmps = empids.Split(',');
                    int changeId = 0;
                    foreach (var emp in arrEmps)
                    {
                        //var arremp = emp.Split('#');
                        var arremp = emp.Split('#');
                        var Id = arremp[0];
                        var fy = arremp[1];
                        var fm = arremp[2];
                        var emp_id = arremp[3];
                        var emp_code = arremp[4];
                        var increment_date = arremp[5];
                        var increment_date_wef = arremp[6];

                        int NewNumIndex = 0;
                        string qry = "";
                        foreach (var item in Values)
                        {
                            
                            NewNumIndex++;
                            if (item.Id == int.Parse(emp_code))
                            {
                                if (item.change_incr_date != null && item.Increment_Date_WEF!= null)
                                {
                                    changeId = int.Parse(emp_code);
                                    qry = " update pr_emp_inc_date_change set Active=0 where  Active=1 AND emp_code=" + emp_code + "; ";

                                    sbqry.Append(qry);
                                    //4. transaction touch
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_inc_date_change", emp_code.ToString(), ""));

                                    // qry = " update pr_emp_inc_date_change set inc_date_change='" + item.IncDate + "' ,remarks='" + item.Remarks + "'  WHERE Active=1 AND emp_code=" + emp_code + ";";
                                    ////2. gen new num
                                    sbqry.Append(GetNewNumStringArr("pr_emp_inc_date_change", NewNumIndex));
                                    qry = "INSERT INTO pr_emp_inc_date_change (id,fy,fm,emp_id,emp_code,inc_date,inc_date_change,remarks,process,authorisation,active,trans_id,Increment_Date_WEF) " +
                                        " VALUES(@idnew" + NewNumIndex + ",'" + fy + "','" + fm + "','" + emp_id + "' ,'" + emp_code + "','" + item.Increment_Date_WEF + "','" + item.change_incr_date + "','" + item.Remarks + "',0,0,1,@transidnew,'" + item.Increment_Date_WEF + "');";

                                    sbqry.Append(qry);
                                    
                                    //qry = " update pr_emp_inc_date_change set Active=0 WHERE emp_code=" + emp_code + ";";
                                    //sbqry.Append(qry);

                                    qry = " update pr_emp_inc_anual_stag set increment_date='" + item.Increment_Date_WEF + "',Increment_Date_WEF='"+ item.Increment_Date_WEF + "' WHERE emp_code=" + emp_code + ";";
                                    sbqry.Append(qry);

                                    //4. transaction touch
                                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_inc_date_change", emp_code.ToString(), ""));

                                    ////2. gen new num
                                    //sbqry.Append(GetNewNumStringArr("pr_emp_inc_date_change", NewNumIndex));
                                    ////3. qry
                                    //qry = "INSERT INTO pr_emp_inc_date_change (id,emp_id,emp_code,inc_date,remarks,process,authorisation,active,trans_id) VALUES(@idnew" + NewNumIndex + ",'"+ emp_id + "' ,'" + emp_code + "','" + item.IncDate + "','" + item.Remarks + "',0,0,1,@transidnew);";

                                    //sbqry.Append(qry);

                                    //4. transaction touch
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_inc_date_change", "@idnew" + NewNumIndex, ""));

                                    qry = "delete from pr_emp_inc_date_change where Active=0 AND emp_code=" + emp_code + "; ";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_inc_date_change", emp_code.ToString(), ""));

                                    await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                                    retMessage = "I#Increment date change# Data Updated Successfully ..!!";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //retMessage = e.Message;
                string msg = e.Message;
                return "E#Error:#" + msg;
            }
            return retMessage;
        }

        private string getchangeDataData(List<IncrementDateChangeModel> Values)
        {
            string qryDetails = "";
            string retStr = "";

            qryDetails = "select m.id,m.fm,m.fy, m.emp_id, m.emp_code,c.remarks,m.Increment_Date_WEF,m.increment_date FROM pr_emp_inc_anual_stag m left outer join pr_emp_inc_date_change c on m.emp_code=c.emp_code and c.active = 1 WHERE m.active = 1;";
            DataTable dtEmpIds = sh.Get_Table_FromQry(qryDetails);
            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["Id"].ToString() + "#" + dr["fy"].ToString() + "#" + dr["fm"].ToString() + "#" + dr["emp_id"].ToString() + "#" + dr["emp_code"].ToString() + "#" + dr["Increment_Date_WEF"].ToString() + "#" + dr["increment_date"].ToString() + "#" + dr["remarks"].ToString() + ",";
            }
            return retStr;
        }


    }
}

