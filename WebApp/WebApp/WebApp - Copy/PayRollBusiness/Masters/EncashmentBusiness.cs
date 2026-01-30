using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PayRollBusiness.Masters
{
    public class EncashmentBusiness : BusinessBase
    {
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        public EncashmentBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        public async Task<IList<CommonGetModel>> getEncashDetails()
        {
            string qrySel = "   select pl.fm as fm,pl.fy as fy,pl.currentyear as encashyear,pl.UpdatedDate as UpdatedDate,pl.TotalPL as totalpl,pl.PLEncash as plencash,emp.EmpId as Id,emp.ShortName as Name, Case when Branches.name = 'OtherBranch' then Departments.name else Branches.name end as Branch ,d.name as Designation from PLE_Type pl join Employees emp on emp.Id = pl.EmpId join designations d on d.id = emp.currentdesignation join Departments on emp.Department = Departments.Id join Branches on emp.Branch = Branches.Id where pl.authorisation=1 and pl.process=0";

            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<CommonGetModel> lstDept = new List<CommonGetModel>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {

                    lstDept.Add(new CommonGetModel
                    {
                        Id = dr["Id"].ToString(),
                        Name = dr["name"].ToString(),
                        Designation = dr["Designation"].ToString(),
                        Branch = dr["Branch"].ToString().ToString(),
                        //IncDate = dr["IncDate"].ToString()
                        UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]).ToString("dd/MM/yyyy"),
                        totalpl= dr["totalpl"].ToString(),
                        plencash = dr["PLEncash"].ToString(),
                        encashyear= dr["encashyear"].ToString(),
                        encashfm = Convert.ToDateTime(dr["fm"]).ToString("dd/MM/yyyy"),
                        encashfy = dr["fy"].ToString(),
                        //change_inc_date = Convert.ToDateTime(dr["inc_date_change"]).ToString("dd/MM/yyyy"),
                        // remarks = dr["remarks"].ToString()

                    });

                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }
      
        public async Task<string> processEncashDetails(List<string> Values)
        {
            string retMessage = "";
            string query1 = "";
       
            int lempid = _LoginCredential.EmpCode;
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            try
            {
               
                    int NewNumIndex = 0;
                        string qry = "";

                foreach (var item in Values)
                {

                    NewNumIndex++;

                    qry = " update PLE_Type set process=1,UpdatedDate=getdate() where authorisation=1 and process =0 and Empid in (select id from employees where empid in ( ";
                    string sub = "";
                
                    foreach (string Id in Values)
                    {
                        sub = sub + Id + ",";
                        //i++;

                    }

                    sub = sub.TrimEnd(',');
                    query1 = qry + sub + ")) ;";
                }
                    sbqry.Append(query1);


                        //4. transaction touch
                       sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "PLE_Type", lempid.ToString(), ""));


                        await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                        retMessage = "I#Encashment Process# Done Successfully ..!!";

                    } 
            
            catch (Exception e)
            {
                //retMessage = e.Message;
                string msg = e.Message;

            }
            return retMessage;

        }
        public async Task<string> GetEmpPRDesignations(int Empid)
        {
            string query1 = "SELECT id,code FROM Designations";
            DataTable dt1 = await _sha.Get_Table_FromQry(query1);
            var resultJson = JsonConvert.SerializeObject(new { designations = dt1 });
            return resultJson;
        }
        public async Task<string> GetEmpPLbalance(int Empid)
        {
            int year = DateTime.Now.Year;
            int plbalance = 0;
            string plbal = "";
            string query1 = "select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid="+ Empid + ") and Year="+ year + " and LeaveTypeId=3";
            DataTable dt1 = await _sha.Get_Table_FromQry(query1);
            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow dr in dt1.Rows)
                {
                    plbal = dr["LeaveBalance"].ToString();


                    plbalance = Convert.ToInt32(plbal);
                }
            }
           
            return plbal;
        }

        //public async Task<string> GetEmpPLdata(int Empid)
        //{
        //    int year = DateTime.Now.Year;
        //    string totalpl = "";
        //    string empcode = "";
        //    string prevencashdate = "";
        //    string encashed= "";
        //    string dataencash = "";
        //    string query1 = " select emp_code,fy,fm,format(prev_encash_date,'yyyy-MM-dd') as prev_encash_date,total_pl,encash,active,trans_id from pr_emp_encashment where emp_code=" + Empid + " and active=1" ;
        //    DataTable dt1 = await _sha.Get_Table_FromQry(query1);
        //    if (dt1.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in dt1.Rows)
        //        {
        //            empcode = dr["emp_code"].ToString();
        //            prevencashdate= dr["prev_encash_date"].ToString();
        //            totalpl = dr["total_pl"].ToString();
        //            encashed = dr["encash"].ToString();
        //        }
        //    }
        //    dataencash = empcode+"," + prevencashdate + "," + encashed+ "," + totalpl  ;
        //    return dataencash;
        //}


        //public async Task<string> AddPlbalanceDetails(CommonPostDTO Values)
        //{
        //    try {
        //        int plbalance = 0;
        //        int FY = DateTime.Now.Year + 1;
        //        string FM = DateTime.Now.ToString("MM-dd-yyyy");
        //        int EfEmpId = Values.EntityId;

        //        // N^1=356^~U^3=456^400~U^4=^111;
        //        string[] plarrRows = Values.StringData.Split(',');

        //        int totalpl1 = Convert.ToInt32(plarrRows[0]);

        //        int encashpl1 = Convert.ToInt32(plarrRows[2]);
        //        if (encashpl1 > totalpl1)
        //        {
        //            return "E#Error#Please Enter Valid PL Balance";
        //        }

        //        StringBuilder sbqry = new StringBuilder();
        //        //1. trans_id
        //        sbqry.Append(GenNewTransactionString());

        //        int NewNumIndex = 0;


        //        if (Values.StringData != null)
        //        {

        //            //string[] plarrRows = Values.StringData.Split(',');


        //            var encashpl = plarrRows[2];
        //            var totalpl = plarrRows[0];
        //            var date = plarrRows[1];
        //            var encshdate = DateTime.Parse(date).ToString("yyyy/MM/dd");
        //            DateTime enDate = Convert.ToDateTime(encshdate);


        //            string qry = "";

        //            NewNumIndex++;
        //            //2. gen new num
        //            sbqry.Append(GetNewNumStringArr("pr_emp_encashment", NewNumIndex));
        //            string query1 = " select Id,emp_code,fy,fm,prev_encash_date,total_pl,encash,active,trans_id from pr_emp_encashment where emp_code=" + EfEmpId + " and active=1";
        //            DataTable dt1 = await _sha.Get_Table_FromQry(query1);
        //            if (dt1.Rows.Count > 0)
        //            {
        //                foreach (DataRow dr in dt1.Rows)
        //                {
        //                    DateTime pldate = Convert.ToDateTime(plarrRows[1]);

        //                    string Id = dr["Id"].ToString();
        //                    string empid = dr["emp_code"].ToString();
        //                    string fdate = dr["prev_encash_date"].ToString();
        //                    DateTime fnDate = Convert.ToDateTime(fdate);
        //                    DateTime daterange = Convert.ToDateTime(fdate);
        //                    int years = daterange.Year;
        //                    int totalDays = Convert.ToInt32((fnDate - enDate).TotalDays);
        //                    int diff = System.Math.Abs(totalDays);
        //                    if((enDate.Year % 400) == 0 || (enDate.Year % 4) == 0)
        //                    {
        //                        if (encashpl == "15" && diff < 364)
        //                        {
        //                            return "E#Error#Already This Employee Has Got Encashment";
        //                        }
        //                        if(encashpl == "30" && diff < 729)
        //                        {
        //                            return "E#Error#Only 15 days are eligible for Encashment";
        //                        }
        //                    } else
        //                    {
        //                        if (encashpl == "15" && diff < 365)
        //                        {
        //                            return "E#Error#Already This Employee Has Got Encashment";
        //                        }
        //                        if (encashpl == "30" && diff < 730)
        //                        {
        //                            return "E#Error#Only 15 days are eligible for Encashment";
        //                        }
        //                    }
                            
        //                    //DateTime today = DateTime.Now.Date;
        //                    //var Daterange = (today - pldate).TotalDays;
        //                    //if (Daterange <= 365)
        //                    //{
        //                    //    if (plarrRows[2] == "30")
        //                    //    {
        //                    //        return "E#Error#Cannot avail more than 15";
        //                    //    }
        //                    //}

        //                    string qry2 = " Update pr_emp_encashment set Active=0 where emp_code=" + empid + ";";
        //                    sbqry.Append(qry2);
        //                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_encashment", "@idnew" + NewNumIndex, ""));
        //                }

        //            }
        //            int year = DateTime.Now.Year;
        //            string query2 = "select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=" + Values.EntityId + ") and Year=" + year + " and LeaveTypeId=3";
        //            DataTable dt2 = await _sha.Get_Table_FromQry(query2);
        //            if (dt2.Rows.Count > 0)
        //            {
        //                foreach (DataRow dr in dt2.Rows)
        //                {
        //                    string plbal = dr["LeaveBalance"].ToString();


        //                    plbalance = Convert.ToInt32(plbal);
        //                }
        //            }
        //            //3. qry
        //            qry = "Insert into pr_emp_encashment ([id],[fy],[fm],[emp_id],[emp_code],[prev_encash_date],[total_pl],[encash],[active],[trans_id]) values "
        //                        + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + "), " + Values.EntityId + ", '" + encshdate + "','" + plbalance + "'," + encashpl + ",1, @transidnew);";
        //            sbqry.Append(qry);

        //            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_encashment", "@idnew" + NewNumIndex, ""));

        //            string encashplbal = encashpl;
        //            int encash = Convert.ToInt32(encashplbal);
        //            int lplbalance = plbalance - encash;
        //            string qry3 = " Update EmpLeaveBalance set LeaveBalance=" + lplbalance + "  where empid=(select id from employees where empid=" + Values.EntityId + ") and Year=" + year + " and LeaveTypeId=3 ";
        //            sbqry.Append(qry3);
        //            //4. transaction touch
        //            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_encashment", "@idnew" + NewNumIndex, ""));
        //        }


        //        if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
        //        {
        //            return "I#Encashment#Data Added Successfully";
        //        }
        //        else
        //        {
        //            return "E#Error 123#Error 456";
        //        }
               

        //    }
        //    catch(Exception ex)
        //    {
        //        string msg = ex.Message;
        //        return "E#Error:#"+msg;
        //    }
        //    }
       

    }

    }      

