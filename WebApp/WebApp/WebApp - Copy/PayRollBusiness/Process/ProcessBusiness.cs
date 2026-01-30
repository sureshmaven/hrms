using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayRollBusiness.PayrollService;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Security.Cryptography;


namespace PayRollBusiness.Process
{
   public class ProcessBusiness : BusinessBase
    {
        public ProcessBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
            LoginCredential lCredentials = null;
        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        //LoginResult lresult = new LoginResult();

        //LoginResult lResult = lgbus.getLoginInformation(Employee.EmpId, Employee.Password);


        #region AnnualStagIncrementAuthorisation

        public async Task<IList<GetIncrementModel>> getAuthStagIncrementDateChange()
        {
            string qrySel = " select emp_code,e.ShortName,des.Name as desi_mid,basic_amount,increment_amount,increment_type ," +
              " increment_date ,inc.stages from pr_emp_inc_anual_stag inc join employees e on e.empid = inc.emp_code  " +
              "join Designations des on e.CurrentDesignation = des.Id  and inc.active = 1 and inc.process = 1 and inc.post_process = 0 " +
              "and inc.authorisation = 1 " ;
              //"where month(increment_date)= MONTH(getdate())";
            //string qrySel = 
                //" SELECT emp_code,e.ShortName,des.Name as desi_mid,basic_amount,increment_amount,increment_type ," +
                //" increment_date ,inc.stages FROM pr_emp_inc_anual_stag inc " +
                //"JOIN employees e on e.empid = inc.emp_code  " +
                //"JOIN Designations des ON e.CurrentDesignation = des.Id  AND inc.active = 1 AND inc.process = 1 AND inc.post_process = 0 " +
                //"AND inc.authorisation = 1   WHERE (select month(fm) FROM pr_month_details WHERE active=1)=month(inc.fm) " +
                //"AND (select year(fm) FROM pr_month_details WHERE active = 1) = year(inc.fm) ";
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
            string payfieldcheck = "";
            string payfieldinsert = "";
            int NewNumIndex = 0;
            int FY = _LoginCredential.FY;
            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
            DataTable dt_payfieldcheck;
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            try
            {
                //foreach (string Id in Values)
                for(int i=0;i< Values.Count;i+=3)
                {
                    string emp_code = Values[i];
                    string type = Values[i+1];
                    int amount = Convert.ToInt32(Values[i+2]);
                    qry = "update pr_emp_inc_anual_stag set post_process = 1 where emp_code = '" + emp_code + "' and active = 1 and authorisation = 1;";
                    sbqry.Append(qry);

                    qry = "update pr_emp_inc_date_change set process = 1 where emp_code = '" + emp_code + "' and active = 1;";
                    sbqry.Append(qry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_inc_anual_stag", emp_code.ToString(), ""));

                    if(type == "stagnation")
                    {
                        payfieldcheck = "select [id],[emp_code],[fy],[fm] from pr_emp_pay_field where emp_code='"+ emp_code + "' and active=1 and " +
                            "m_id=(select id from pr_earn_field_master where name='Stagnation Increments' and type='pay_fields');";
                        dt_payfieldcheck = sh.Get_Table_FromQry(payfieldcheck);
                        if(dt_payfieldcheck.Rows.Count>0)
                        {
                            string stgQry = "update pr_emp_pay_field set amount=" + amount + " where emp_code='" + emp_code + "' and active=1 " +
                            "and m_id=(select id from pr_earn_field_master where name='Stagnation Increments' and type='pay_fields');";
                            sbqry.Append(stgQry);
                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", emp_code.ToString(), ""));
                        }
                        else if(dt_payfieldcheck.Rows.Count==0)
                        {
                            NewNumIndex++;
                            //2. gen new num
                            sbqry.Append(GetNewNumStringArr("pr_emp_pay_field", NewNumIndex));

                            //3. qry
                            payfieldinsert = "Insert into pr_emp_pay_field ([id],[emp_id],[emp_code],[fy]," +
                                "[fm],[m_id],[m_type],[amount],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                                "" + emp_code + "," + FY + ",'" + FM + "',(select id from pr_earn_field_master where name='Stagnation Increments' and type='pay_fields'),'Pay_fields', " + amount + ",1, @transidnew);";
                            sbqry.Append(payfieldinsert);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_pay_field", "@idnew" + NewNumIndex, ""));
                        }
                    }
                    else
                    {
                        payfieldcheck = "select [id],[emp_code],[fy],[fm] from pr_emp_pay_field where emp_code='"+ emp_code + "' and active=1 and " +
                            "m_id=(select id from pr_earn_field_master where name='Annual Increment' and type='pay_fields');";
                        dt_payfieldcheck = sh.Get_Table_FromQry(payfieldcheck);
                        if(dt_payfieldcheck.Rows.Count>0)
                        {
                            string anualQry = "update pr_emp_pay_field set amount=" + amount + " where emp_code='" + emp_code + "' and active=1 " +
                            "and m_id=(select id from pr_earn_field_master where name='Annual Increment' and type='pay_fields');";
                            sbqry.Append(anualQry);
                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", emp_code.ToString(), ""));
                        }
                        else if(dt_payfieldcheck.Rows.Count==0)
                        {
                            NewNumIndex++;
                            //2. gen new num
                            sbqry.Append(GetNewNumStringArr("pr_emp_pay_field", NewNumIndex));

                            //3. qry
                            payfieldinsert = "Insert into pr_emp_pay_field ([id],[emp_id],[emp_code],[fy]," +
                                "[fm],[m_id],[m_type],[amount],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                                "" + emp_code + "," + FY + ",'" + FM + "',(select id from pr_earn_field_master where name='Annual Increment' and type='pay_fields'),'Pay_fields', " + amount + ",1, @transidnew);";
                            sbqry.Append(payfieldinsert);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_pay_field", "@idnew" + NewNumIndex, ""));
                        }
                    }
                    string empQry = "update pr_emp_biological_field set revision_of_date_change=(select increment_date from pr_emp_inc_anual_stag where emp_code='" + emp_code + "' and active=1) where emp_code='" + emp_code + "';";
                    sbqry.Append(empQry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_biological_field", emp_code.ToString(), ""));

                }
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    retMessage = "I#Post Increment# Data Updated Successfully ..!!";
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

        #region processallowance
        public async Task<string> getAllowancesdata()
        {
            try
            {
                string qry = "select datediff(day,from_date,to_date)as NoOfDays ,am.name as AllowanceType,am.amount as Amount,ba.emp_code as emp_code " +
                    "from pr_emp_branch_allowances ba join pr_branch_allowance_master am on am.id = ba.allowance_mid";
                DataTable dt = await _sha.Get_Table_FromQry(qry);
                StringBuilder sbqry = new StringBuilder();
                sbqry.Append(GenNewTransactionString());
                int NewNumIndex = 0;
               
                foreach (DataRow dr in dt.Rows)
                {
                    NewNumIndex++;
                    int empcode = Convert.ToInt32(dr["emp_code"]);
                    int NoOFDays = Convert.ToInt32(dr["NoOfDays"]);
                    string AllowanceType = dr["AllowanceType"].ToString();
                    float amount = Convert.ToInt32(dr["Amount"]);
                    float OneDayEmpAllowance = amount / NoOFDays;
                    int FY = 2020;
                    DateTime FM = DateTime.Now;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_emp_allowance_process", NewNumIndex));
                    //query
                  string  insqry = "Insert into pr_emp_allowance_process ([id],[fy],[fm],[emp_id],[emp_code],[allowance_type],[allowance_oneday__amount],[no_of_days],[employee_amount],[active]) values "
                                + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + empcode + "),"+empcode+",'" + AllowanceType + "', " + OneDayEmpAllowance + ", " + NoOFDays + "," + amount + ",1);";
                    sbqry.Append(insqry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowance_process", "@idnew" + NewNumIndex, ""));


                }



                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#Employee Pesonal Earning#Data Added Successfully";
                }
            }
            catch(Exception e)
            {

            }
            return "";
        }
        #endregion

      
    }
}
