using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Mavensoft.DAL.Db;
using PayrollModels;
using PayrollModels.Process;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Poc
{
    public class PocBusiness : BusinessBase
    {
        public PocBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public async Task<IList<CommonGetModel>> getPocDepartmentMaster(string empid)
        {
            string qrySel = "SELECT m.id, m.department, e.days, case when e.dept_id is null then 'N' else 'U' end as row_type " +
                        "FROM pr_poc_department_master m left outer join pr_poc_department_emp e " +
                        "ON m.id = e.dept_id and e.active = 1 and e.emp_code = " + empid +
                        " WHERE m.active = 1";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);

            IList<CommonGetModel> lstDept = new List<CommonGetModel>();
            foreach (DataRow dr in dt.Rows)
            {
                lstDept.Add(new CommonGetModel
                {
                    Id = dr["id"].ToString(),
                    Name = dr["department"].ToString(),
                    Value = dr["days"].ToString(),
                    RowType = dr["row_type"].ToString()[0]
                });
            }
            return lstDept;
        }
        public async Task<string> UpdateGeneralData(CommonPostDTO Values)
        {
            var emp_code = Values.EntityId;
            var multData = Values.multiObject;
            string qry = "";
            int FY = DateTime.Now.Year + 1;
            string FM = DateTime.Now.ToString("MM-dd-yyyy");
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());

            int NewNumIndex = 0;
            foreach (var emp in multData)
            {
                if(emp.Action== "New")
                {
                    NewNumIndex++;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_emp_lic_details", NewNumIndex));
                    //3. qry
                    //qry = "Insert into pr_emp_lic_details values "
                    //    + "(@idnew" + NewNumIndex + ",2020,4," + EmpCode + "," + EmpCode + ", " + pkid + ", " + newVal + ",1, @transidnew);";
                    qry = "INSERT INTO pr_emp_lic_details VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "'," +
                   "(select id from employees where empid=" + emp_code + ")," + emp_code + "," +
                   "'" + Convert.ToString(emp.AccNum) + "'," + Convert.ToInt32(emp.Amount) + ",'" + Convert.ToString(emp.PayType) + "'," + 
                   Convert.ToInt32(emp.PayMonths) + ",'" + Convert.ToString(emp.StartMonth) + "','" + Convert.ToString(emp.StopMonth) + "',1,@transidnew);";
                    
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_lic_details", "@idnew" + NewNumIndex, ""));
                }
                else if(emp.Action== "Update")
                {
                    //qry = "Update pr_emp_lic_details SET days=" + newVal + ", trans_id=@transidnew where dept_id=" + pkid + " AND Emp_Code=" + EmpCode + " ;";
                    qry = " Update pr_emp_lic_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount="+Convert.ToInt32(emp.Amount)+"" +
                        ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months="+Convert.ToInt32(emp.PayMonths)+ "," +
                        "start_month='" + Convert.ToString(emp.StartMonth) + "',stop_month='" + Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_lic_details", emp.Id.ToString(), ""));
                } else if(emp.Action == "Deleted")
                {
                    qry = " Update pr_emp_lic_details SET active=0 where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_lic_details", emp.Id.ToString(), ""));
                }
            }

            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#Data Submission#Data Submitted Successfully..!!";
            }
            else
            {
                return "E#Error#Error While Data Submission";
            }
        }
            public async Task<IList<CommonGetModel>> getAddRowData(int EmpCode)
        {

            string qryLic = "select id,account_no,amount,pay_type,pay_months,format(start_month,'yyyy-MM-dd') as start_month," +
                "format(stop_month,'yyyy-MM-dd') as stop_month " +
                "from pr_emp_lic_details where emp_code =" + EmpCode+" and active=1";
            DataTable dt = await _sha.Get_Table_FromQry(qryLic);

            IList<CommonGetModel> lstEmpData = new List<CommonGetModel>();
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = dr["id"].ToString(),
                        AccNum = dr["account_no"].ToString(),
                        Amount = dr["amount"].ToString(),
                        PayType = dr["pay_type"].ToString(),
                        PayMonths = dr["pay_months"].ToString(),
                        StartMonth = dr["start_month"].ToString(),
                        StopMonth = dr["stop_month"].ToString(),
                        Action = "Add"
                    });
                }


            }
            else
            {

                lstEmpData.Add(new CommonGetModel
                {
                    AccNum = "",
                    Amount = "",
                    PayType = "",
                    PayMonths = "",
                    StartMonth = "",
                    StopMonth = "",
                    Action = "Add"
                });

            }
            return lstEmpData;
        }

        public async Task<string> ProcessRequest(int EmpCode, string Data)
        {
            // N^1=356^~U^3=456^400~U^4=^111

            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());

            int NewNumIndex = 0;
            string[] arrRows = Data.Split('~');
            foreach(string rdata in arrRows)
            {
                var arrData = rdata.Split('=');

                var arrTypId = arrData[0].Split('^'); //N^1
                var type = arrTypId[0];
                var pkid = arrTypId[1];

                var arrVals = arrData[1].Split('^'); //456^400
                var newVal = arrVals[0];
                var oldVal = arrVals[1];

                string qry = "";
                if(type == "N")
                {
                    NewNumIndex++;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_poc_department_emp", NewNumIndex));
                    //3. qry
                    qry = "Insert into pr_poc_department_emp ([id],[fy],[fm],[emp_id],[emp_code],[dept_id],[days],[active],[trans_id]) values "
                        + "(@idnew"+ NewNumIndex + ",2020,4," + EmpCode + "," + EmpCode + ", " + pkid +", "+ newVal + ",1, @transidnew);";

                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_poc_department_emp", "@idnew"+ NewNumIndex, ""));
                }
                else if(type == "U" && newVal != "") //update
                {
                    qry = "Update pr_poc_department_emp SET days=" + newVal + ", trans_id=@transidnew where dept_id=" + pkid + " AND Emp_Code=" + EmpCode + " ;";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_poc_department_emp", pkid.ToString(), oldVal));
                }
                else if (type == "U" && newVal == "") //delete
                {
                    qry = "Update pr_poc_department_emp SET active = 0, trans_id=@transidnew where dept_id=" + pkid + " AND Emp_Code=" + EmpCode + " ;";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_poc_department_emp", pkid.ToString(), ""));
                }
            }

            if( await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#Added data1#added data 2";
            }
            else
            {
                return "E#Error 123#Error 456";
            }
        }

        public async Task<string> UpdateDept()
        {
            string qryUpd = "Update pr_poc_department_master set department='hr-updt' where id=4;";
           string res = await UpdateRecord(qryUpd, 4);
            return res;
           
        }

        public async Task<string> DeleteDept()
        {
            string qryDel = "Update pr_poc_department_master set Active=0 where id=40;";
            string res = await InactivateRecord(qryDel, 40);
            return res;
        }
    }
}
