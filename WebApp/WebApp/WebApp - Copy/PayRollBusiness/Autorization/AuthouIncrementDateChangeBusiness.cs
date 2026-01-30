using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Autorization
{
   public class AuthouIncrementDateChangeBusiness : BusinessBase
    {
        public AuthouIncrementDateChangeBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        /// Authorisation increment date change 
        #region AuthIncrementDateChange
        public async Task<IList<CommonGetModel>> getAuthorisationDetails()
        {

            string qrySel = "select distinct Employees.EmpId as Id ,Employees.ShortName as Name,Designations.Name AS Designation," +
                "pr_emp_inc_date_change.Increment_Date_WEF,pr_emp_inc_date_change.inc_date as IncDate ,pr_emp_inc_date_change.inc_date_change as change_inc_date," +
                "pr_emp_inc_date_change.emp_code, Case when Branches.name = 'OtherBranch' then Departments.name else Branches.name end as Branch ," +
                "pr_emp_inc_date_change.trans_id ,pr_emp_inc_date_change.active ,pr_emp_inc_date_change.authorisation,pr_emp_inc_date_change.remarks  " +
                "from Employees join Designations on Employees.CurrentDesignation = Designations.Id " +
                "join Departments on Employees.Department = Departments.Id join Branches on Employees.Branch = Branches.Id " +
                "join pr_emp_inc_date_change on Employees.EmpId = pr_emp_inc_date_change.emp_code and pr_emp_inc_date_change.authorisation = 0 " +
                "and pr_emp_inc_date_change.active = 1 and pr_emp_inc_date_change.inc_date_change is not null  ";
                //"where month(inc_date) = MONTH(getdate()) ";
                //"where (select month(fm) from pr_month_details where active=1)=month(pr_emp_inc_date_change.fm) " +
                //"and (select year(fm) from pr_month_details where active = 1) = year(pr_emp_inc_date_change.fm) ";
                //"and pr_emp_inc_date_change.fm=pr_emp_inc_date_change.inc_date ";
                //"where month(inc_date)= MONTH(getdate()) ";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<CommonGetModel> lstDept = new List<CommonGetModel>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new CommonGetModel
                    {
                        Id = dr["Id"].ToString(),
                        Name = dr["Name"].ToString(),
                        Designation = dr["Designation"].ToString(),
                        Branch = dr["Branch"].ToString().ToString(),
                        Increment_Date_WEF = Convert.ToDateTime(dr["Increment_Date_WEF"]).ToString("dd/MM/yyyy"),
                        IncDate = Convert.ToDateTime(dr["IncDate"]).ToString("dd/MM/yyyy"),
                        change_inc_date = Convert.ToDateTime(dr["change_inc_date"]).ToString("dd/MM/yyyy"),
                        remarks= dr["remarks"].ToString().ToString()
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }
        public async Task<string> AuthIncrementDateChange(List<inc> Values)
        {
            string qry = "";
            string retMessage = "";
            int NewNumIndex = 0;
            // string data=inc.
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
                        var arremp = emp.Split('#');
                        var Id = arremp[0];
                        var fy = arremp[1];
                        var fm = arremp[2];
                        var emp_id = arremp[3];
                        var emp_code = arremp[4];
                        string remarks = arremp[5];
                        //int NewNumIndex = 0;
                        //string qry = "";
                        string IncDate = "";
                        foreach (var item in Values)
                        {
                            if (item.Id == int.Parse(emp_code))
                            {
                                string change_incr_date = "";
                                change_incr_date = Convert.ToDateTime(item.change_incr_date).ToString("yyyy/MM/dd");
                                if(change_incr_date == null|| change_incr_date=="")
                                {
                                    change_incr_date = Convert.ToDateTime(item.Increment_Date_WEF).ToString("yyyy/MM/dd");
                                }
                                string change_incr_date_WFT = Convert.ToDateTime(item.Increment_Date_WEF).ToString("yyyy/MM/dd");
                                changeId = int.Parse(emp_code);
                                NewNumIndex++;
                                qry = "update pr_emp_inc_date_change set authorisation = 1 where emp_code = '" + item.Id + "' and authorisation = 0 and process = 0 and active=1";
                                sbqry.Append(qry);
                                qry = " update pr_emp_inc_anual_stag set increment_date='"+ change_incr_date + "',Increment_Date_WEF='"+ change_incr_date_WFT + "' where emp_code=" + item.Id + " and active=1;";
                                sbqry.Append(qry);
                                //qry = "update pr_emp_inc_date_change set active = 0 where emp_code = '" + item.Id + "' and process = 0 ; ";
                                //sbqry.Append(qry);
                                ////2. gen new num
                                //sbqry.Append(GetNewNumStringArr("pr_emp_inc_date_change", NewNumIndex));
                                ////3. qry
                                //qry = "INSERT INTO pr_emp_inc_date_change (id,fy,fm,emp_id,emp_code,inc_date,remarks,process,authorisation,active,trans_id) VALUES(@idnew" + NewNumIndex + ",'" + fy + "','" + fm + "','" + emp_id + "' ,'" + emp_code + "','" + change_incr_date + "','" + remarks + "',0,1,1,@transidnew);";

                                //sbqry.Append(qry);

                                //4. transaction touch
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_inc_date_change", item.Id.ToString(), ""));


                                //4. transaction touch
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_inc_date_change", item.Id.ToString(), ""));
                            }
                        }
                    }
                }
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    retMessage = "I#Authorisation IncrementDateChange # Data Updated Successfully ..!!";
                }

            }
            catch (Exception e)
            {
                string msg = e.Message;
                return "E#Error:#" + msg;
            }
            return retMessage;
        }
        private string getchangeDataData(List<inc> Values)
        {
            string qryDetails = "";
            string retStr = "";

            qryDetails = "select id,fy,fm, emp_id, emp_code,remarks from pr_emp_inc_date_change where active = 1";
            DataTable dtEmpIds = sh.Get_Table_FromQry(qryDetails);
            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["Id"].ToString() + "#" + dr["fy"].ToString() + "#" + dr["fm"].ToString() + "#" + dr["emp_id"].ToString() + "#" + dr["emp_code"].ToString() + "#" + dr["remarks"].ToString() + ",";
            }
            return retStr;
        }
        #endregion
        #region AnnualStagIncrementAuthorisation

        public async Task<IList<GetIncrementModel>> getAuthStagIncrementDateChange()
        {
            string qrySel = "select emp_code,e.ShortName,des.Name as desi_mid,basic_amount,increment_amount,increment_type ,increment_date ,inc.stages  " +
                "from pr_emp_inc_anual_stag inc join employees e on e.empid = inc.emp_code  " +
                "join Designations des on e.CurrentDesignation = des.Id and inc.active = 1 and inc.process = 1 and inc.authorisation=0 " ;
                //"where month(increment_date)= MONTH(getdate()) ";
                //"where (select month(fm) from pr_month_details where active=1)=month(inc.fm) " +
                //"and (select year(fm) from pr_month_details where active = 1) = year(inc.fm) ";
                //"and inc.fm=inc.increment_date ";
                //"where month(increment_date)= MONTH(getdate())";

            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<GetIncrementModel> lstDept = new List<GetIncrementModel>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new GetIncrementModel
                    {
                        Id = dr["emp_code"].ToString(),
                        Name = dr["ShortName"].ToString(),
                        Basic = dr["basic_amount"].ToString().ToString(),
                        Increment = dr["increment_amount"].ToString(),
                        increment_type = dr["increment_type"].ToString(),
                        increment_date = Convert.ToDateTime(dr["increment_date"]).ToString("dd/MM/yyyy"),
                        desi_mid = dr["desi_mid"].ToString(),
                        stages = dr["stages"].ToString()

                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }

        public async Task<string> UpAuthStagIncDateChange(List<string> Values)
        {
            string qry = "";
            string retMessage = "";
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            try
            {
                foreach (string Id in Values)
                {
                    qry = "update pr_emp_inc_anual_stag set authorisation = 1 where emp_code = '" + Id + "' and active = 1 and authorisation = 0";
                    sbqry.Append(qry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_inc_anual_stag", Id.ToString(), ""));

                }
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    retMessage = "I#Authorisation AnnualStagIncrement# Data Updated Successfully ..!!";
                }

            }
            catch (Exception e)
            {
                string msg = e.Message;
                return "E#Error:#" + msg;
            }
            return retMessage;
        }
        #endregion
    }
}
