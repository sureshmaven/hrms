using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.DAL.Db;
using PayrollModels.Transactions;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Mavensoft.Common;
using System.Data;
using PayrollModels;

namespace PayRollBusiness.Transaction
{
    public class AllowanceBusiness : BusinessBase
    {
        public AllowanceBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        SqlHelperAsync _sha = new SqlHelperAsync();
        public async Task<string> InsertAllowance(CommonPostDTO values)
        {
            int NewNumIndex = 0;
            int emp_code = values.EntityId;
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            var dates = values.objdates;
            var todate = "";
            var fromdate = "";
            string updatequery = "";
            string e_date = "";
            int? days = 0;
            decimal? day_amount;
            decimal? total_amount;
            int FY = _LoginCredential.FY;
            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
            //int fy = _LoginCredential.FY;
            //string sFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
            int? m_end = 0;

            string selQry = "select emp_code from pr_emp_general where emp_code =" + emp_code + "and active=1";

            DataTable details = await _sha.Get_Table_FromQry(selQry);
            if(details.Rows.Count>0)
            {
                if (dates != null)
                {
                    foreach (var item in dates)
                    {
                        if (item.to_date == null)
                        {
                            //todate = DateTime.Now.ToString("yyyy-MM-dd");
                            todate = null;
                        }
                        else
                        {
                            todate = item.to_date;
                        }
                        if (item.from_date == null)
                        {
                            //todate = DateTime.Now.ToString("yyyy-MM-dd");
                            fromdate = null;
                        }
                        else
                        {
                            todate = item.to_date;
                        }

                        if (item.action == "new")
                        {
                            //int days = DateTime.DaysInMonth(DateTime.yea, int month);


                            updatequery = "update pr_emp_branch_allowances set active =0 where emp_code=" + emp_code + " and allowance_mid=" + Convert.ToInt32(item.id) + ";";
                            sbqry.Append(updatequery);
                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_branch_allowances", NewNumIndex));
                            string inquery = "INSERT INTO pr_emp_branch_allowances(id,emp_id,emp_code,fy,fm,allowance_mid,[from_date],[to_date],[active],[trans_id])" +
                             "VALUES(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," + emp_code + ","+ FY + ",'"+ FM + "','" + Convert.ToInt32(item.id) +
                             "',CASE WHEN  '" + Convert.ToString(item.from_date) + "' = '' THEN NULL ELSE " + "'" + Convert.ToString(item.from_date) + "'  END ," +
                                "CASE WHEN '" + Convert.ToString(todate) + "'='' THEN NULL ELSE " + "'" + Convert.ToString(todate) + "' END,1,@transidnew) ; ";
                            sbqry.Append(inquery);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_branch_allowances", "@idnew" + NewNumIndex, ""));

                            DateTime lstDt = Convert.ToDateTime(Convert.ToString(item.from_date));
                            DateTime lstDate = new DateTime(lstDt.Year, lstDt.Month, 1).AddMonths(1).AddDays(-1);
                            m_end = lstDate.Day;

                            //DateTime origDT = Convert.ToDateTime(Convert.ToString(item.from_date));
                            //DateTime lastDate = new DateTime(origDT.Year, origDT.Month, 1).AddMonths(1).AddDays(-1);
                            //e_date = lastDate.Year + "-" + lastDate.Month + "-" + lastDate.Day;

                            //if (Convert.ToString(todate) == null)
                            //{
                            //    DateTime origDT = Convert.ToDateTime(Convert.ToString(item.from_date));
                            //    DateTime lastDate = new DateTime(origDT.Year, origDT.Month, 1).AddMonths(1).AddDays(-1);
                            //    e_date = lastDate.Year + "-" + lastDate.Month + "-" + lastDate.Day;

                            //}
                            //else
                            //{
                            //    DateTime origDT = Convert.ToDateTime(Convert.ToString(todate));
                            //    DateTime lastDate = new DateTime(origDT.Year, origDT.Month, 1).AddMonths(1).AddDays(-1);
                            //    e_date = lastDate.Year + "-" + lastDate.Month + "-" + lastDate.Day;

                            //}

                            if (Convert.ToString(todate) == null)
                            {
                                DateTime origDT = Convert.ToDateTime(Convert.ToString(item.from_date));
                                DateTime lastDate = new DateTime(origDT.Year, origDT.Month, 1).AddMonths(1).AddDays(-1);
                                e_date = lastDate.Year + "-" + lastDate.Month + "-" + lastDate.Day;

                            }
                            else
                            {

                                DateTime origDT = Convert.ToDateTime(Convert.ToString(todate));

                                if (origDT.Year == lstDt.Year && origDT.Month == lstDt.Month)
                                {
                                    e_date = Convert.ToString(todate);
                                }
                                else
                                {
                                    DateTime origDTs = Convert.ToDateTime(Convert.ToString(item.from_date));
                                    DateTime lastDate = new DateTime(origDTs.Year, origDTs.Month, 1).AddMonths(1).AddDays(-1);
                                    e_date = lastDate.Year + "-" + lastDate.Month + "-" + lastDate.Day;
                                }

                            }

                            days = Convert.ToInt32((Convert.ToDateTime(e_date) - Convert.ToDateTime(Convert.ToString(item.from_date))).TotalDays);
                            day_amount = Convert.ToDecimal(item.amount) / m_end;
                            total_amount = (days + 1) * day_amount;
                            double amt = Math.Round(Convert.ToDouble(total_amount), 2);

                            if (item.name == "Br Manager Allowance")
                            {
                                string qry1 = "Update pr_emp_allowances_gen SET active=0 where m_id=(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name = 'Br Manager Allowance') AND emp_code=" + emp_code + " ;";
                                sbqry.Append(qry1);
                                NewNumIndex++;
                                sbqry.Append(GetNewNumStringArr("pr_emp_allowances_gen", NewNumIndex));

                                string qry2 = "Insert into pr_emp_allowances_gen ([id],[emp_id],[emp_code],[fy],[fm]," +
                                "[m_id],[m_type],[amount],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," + emp_code + "," + FY + ",'" + FM + "'," +
                                "(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name = 'Br Manager Allowance'),'EMPA', " + amt + ",1, @transidnew);";
                                sbqry.Append(qry2);

                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_gen", "@idnew" + NewNumIndex, ""));

                                ////4. transaction touch
                                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_gen", "(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name = 'Br Manager Allowance')", amt.ToString()));
                                //string qrySelect = "select id from pr_emp_allowances_gen where m_id=(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name ='Br Manager Allowance')  AND emp_code=" + emp_code + " ;";
                                //DataTable dt = await _sha.Get_Table_FromQry(qrySelect);
                                //if (dt.Rows.Count > 0)
                                //{
                                //    // update allowance


                                //    string qry1 = "Update pr_emp_allowances_gen SET amount=" + amt + ", trans_id=@transidnew where m_id=(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name = 'Br Manager Allowance') AND emp_code=" + emp_code + " ;";
                                //    sbqry.Append(qry1);

                                //    //4. transaction touch
                                //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_gen", "(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name = 'Br Manager Allowance')", amt.ToString()));

                                //}
                                //else
                                //{
                                //    //insert into allowance
                                //    NewNumIndex++;
                                //    sbqry.Append(GetNewNumStringArr("pr_emp_allowances_gen", NewNumIndex));

                                //    string qry2 = "Insert into pr_emp_allowances_gen ([id],[emp_id],[emp_code],[fy],[fm]," +
                                //    "[m_id],[m_type],[amount],[active],[trans_id]) values "
                                //    + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," + emp_code + "," + FY + ",'" + FM + "'," +
                                //    "(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name = 'Br Manager Allowance'),'EMPA', " + amt + ",1, @transidnew);";
                                //    sbqry.Append(qry2);

                                //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_gen", "@idnew" + NewNumIndex, ""));


                                //}


                            }
                            else
                            {
                                // specail allowance
                                string qrySelect = "select id from pr_allowance_field_master where name = '" + item.name + "' and type='EMPSA' ";
                                string qryupdate = "update pr_emp_allowances_spl set active=0 where emp_code="+emp_code+ " and m_id=(select id from pr_allowance_field_master where name = '"+item.name+"' and type='EMPSA');";
                                string qrySelectAllowance = "select * from pr_emp_allowances_spl  " +
                                    "where m_id=(select id from pr_allowance_field_master where name = 'SHIFT DUTY ALLOWANCE' and type='EMPSA') AND emp_code=" + emp_code + " ;";
                                DataTable dt = await _sha.Get_Table_FromQry(qrySelect);
                                DataTable dtsplall = await _sha.Get_Table_FromQry(qrySelectAllowance);
                                if (dt.Rows.Count > 0) //specail allowance updation
                                {

                                    if(dtsplall.Rows.Count > 0)
                                    {
                                        string qry = "Update pr_emp_allowances_spl SET amount=" + amt + ", trans_id=@transidnew where m_id=(select id from pr_allowance_field_master where name = '" + item.name + "' and type='EMPSA') AND emp_code=" + emp_code + " ;";
                                        sbqry.Append(qry);
                                        //4. transaction touch
                                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_spl", "(select id from pr_allowance_field_master where name = '" + item.name + "' and type='EMPSA')", amt.ToString()));
                                    }
                                    else
                                    {
                                        NewNumIndex++;
                                        sbqry.Append(GetNewNumStringArr("pr_emp_allowances_spl", NewNumIndex));
                                        string qryy = "Insert into pr_emp_allowances_spl ([id],[emp_id],[emp_code],[fy],[fm]," +
                                          "[m_id],[m_type],[amount],[active],[trans_id]) values "
                                  + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                                 "" + emp_code + "," + FY + ",'" + FM + "',(select id from pr_allowance_field_master where name = '" + item.name + "'),'EMPSA', " + amt + ",1, @transidnew);";
                                        sbqry.Append(qryupdate);
                                        sbqry.Append(qryy);
                                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_spl", "@idnew" + NewNumIndex, ""));
                                    }
                    
                                }
                                else
                                {
                                    // allowance field master entry and specail allowance insertion 
                                    //string qrySelect1 = "select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name ='" + item.name + "'";
                                    //DataTable dt1 = await _sha.Get_Table_FromQry(qrySelect1);
                                    //if (dt1.Rows.Count > 0)
                                    //{
                                    //    NewNumIndex++;
                                    //    sbqry.Append(GetNewNumStringArr("pr_emp_allowances_spl", NewNumIndex));

                                    //    string qry = "Insert into pr_emp_allowances_spl ([id],[emp_id],[emp_code],[fy],[fm]," +
                                    //"[m_id],[m_type],[amount],[active],[trans_id]) values "
                                    //+ "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                                    //"" + emp_code + "," + FY + ",'" + FM + "',(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name ='" + item.name + "'),'EMPA', " + amt + ",1, @transidnew);";
                                    //    sbqry.Append(qry);
                                    //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_spl", "@idnew" + NewNumIndex, ""));
                                    //}
                                    //else
                                    //{
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_allowance_field_master", NewNumIndex));

                                    string qry = "Insert into pr_allowance_field_master ([id],[name],[type],[active],[trans_id],[benefit]) values "
                                + "(@idnew" + NewNumIndex + ",'" + item.name + "'," +
                                "'EMPSA', 1, @transidnew,null);";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_allowance_field_master", "@idnew" + NewNumIndex, ""));

                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_allowances_spl", NewNumIndex));

                                    string qryy = "Insert into pr_emp_allowances_spl ([id],[emp_id],[emp_code],[fy],[fm]," +
                                "[m_id],[m_type],[amount],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                                "" + emp_code + "," + FY + ",'" + FM + "',(select id from pr_allowance_field_master where name = '" + item.name + "'),'EMPSA', " + amt + ",1, @transidnew);";
                                    sbqry.Append(qryy);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_spl", "@idnew" + NewNumIndex, ""));

                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_payslip_customization", NewNumIndex));

                                    string qryyy = "Insert into pr_payslip_customization ([id]," +
                                "[m_id],[field_type],[cust_status],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex + ",(select id from pr_allowance_field_master where name = '" + item.name + "'),'EMPSA', ' Yes ',1, @transidnew);";
                                    sbqry.Append(qryyy);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_payslip_customization", "@idnew" + NewNumIndex, ""));


                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_encashment_earnings_customization", NewNumIndex));

                                    string qry1= "Insert into pr_encashment_earnings_customization ([id]," +
                                "[m_id],[field_type],[cust_status],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex + ",(select id from pr_allowance_field_master where name = '" + item.name + "'),'EMPSA', ' Yes ',1, @transidnew);";
                                    sbqry.Append(qry1);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_encashment_earnings_customization", "@idnew" + NewNumIndex, ""));
                                }

                                //}


                            }

                        }
                        else if (item.action == "update")
                        {
                            if (item.to_date == null && item.from_date != null)
                            {

                                updatequery = "update pr_emp_branch_allowances set from_date='" + Convert.ToString(item.from_date) + "'," +
                                "to_date=null where allowance_mid =" + Convert.ToInt32(item.id) + " and emp_code="+emp_code+ " and active=1;";
                            }

                            else if (item.to_date != null && item.from_date == null)
                            {

                                updatequery = "update pr_emp_branch_allowances set from_date=null," +
                                "to_date='" + Convert.ToString(todate) + "' where allowance_mid =" + Convert.ToInt32(item.id) + " and emp_code=" + emp_code + " and active=1;";
                            }
                            else if (item.from_date == null && item.to_date == null)
                            {
                                updatequery = "update pr_emp_branch_allowances set from_date=null," +
                                 "to_date=null where allowance_mid =" + Convert.ToInt32(item.id) + ";";
                            }
                            else
                            {
                                updatequery = "update pr_emp_branch_allowances set from_date='" + Convert.ToString(item.from_date) + "'," +
                                "to_date='" + item.to_date + "' where allowance_mid =" + Convert.ToInt32(item.id) + " and emp_code=" + emp_code + " and active=1;";
                            }

                            sbqry.Append(updatequery);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_branch_allowances", emp_code.ToString() + NewNumIndex, ""));
                            DateTime lstDt = Convert.ToDateTime(Convert.ToString(item.from_date));
                            DateTime lstDate = new DateTime(lstDt.Year, lstDt.Month, 1).AddMonths(1).AddDays(-1);
                            m_end = lstDate.Day;

                            //DateTime origDT = Convert.ToDateTime(Convert.ToString(item.from_date));
                            //DateTime lastDate = new DateTime(origDT.Year, origDT.Month, 1).AddMonths(1).AddDays(-1);
                            //e_date = lastDate.Year + "-" + lastDate.Month + "-" + lastDate.Day;
                            

                            if (Convert.ToString(todate) == null)
                            {
                                DateTime origDT = Convert.ToDateTime(Convert.ToString(item.from_date));
                                DateTime lastDate = new DateTime(origDT.Year, origDT.Month, 1).AddMonths(1).AddDays(-1);
                                e_date = lastDate.Year + "-" + lastDate.Month + "-" + lastDate.Day;

                            }
                            else 
                            { 

                                DateTime origDT = Convert.ToDateTime(Convert.ToString(todate));

                                if(origDT.Year == lstDt.Year && origDT.Month == lstDt.Month)
                                {
                                     e_date = Convert.ToString(todate);
                                } else
                                {
                                    DateTime origDTs = Convert.ToDateTime(Convert.ToString(item.from_date));
                                    DateTime lastDate = new DateTime(origDTs.Year, origDTs.Month, 1).AddMonths(1).AddDays(-1);
                                    e_date = lastDate.Year + "-" + lastDate.Month + "-" + lastDate.Day;
                                }
                                
                            }
                            days = Convert.ToInt32((Convert.ToDateTime(e_date) - Convert.ToDateTime(Convert.ToString(item.from_date))).TotalDays);
                            day_amount = Convert.ToDecimal(item.amount) / m_end;
                            total_amount = (days + 1) * day_amount;
                            double amt = Math.Round(Convert.ToDouble(total_amount), 2);
                            if (item.name == "Br Manager Allowance")
                            {

                                string qry = "Update pr_emp_allowances_gen SET active=0 where m_id=(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name = 'Br Manager Allowance') AND emp_code=" + emp_code + " ;";
                                sbqry.Append(qry);
                                NewNumIndex++;
                                sbqry.Append(GetNewNumStringArr("pr_emp_allowances_gen", NewNumIndex));

                                string qry2 = "Insert into pr_emp_allowances_gen ([id],[emp_id],[emp_code],[fy],[fm]," +
                                "[m_id],[m_type],[amount],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," + emp_code + "," + FY + ",'" + FM + "'," +
                                "(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name = 'Br Manager Allowance'),'EMPA', " + amt + ",1, @transidnew);";
                                sbqry.Append(qry2);

                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_gen", "@idnew" + NewNumIndex, ""));
                                ////4. transaction touch
                                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_gen", "(select M.id from pr_allowance_field_master M JOIN pr_branch_allowance_master C ON M.name = C.description where C.active = 1 AND M.name = 'Br Manager Allowance')", amt.ToString()));
                            }
                            else
                            {
                                string qrySelect = "select s.m_id from pr_emp_allowances_spl s join pr_allowance_field_master m on m.id = s.m_id where m.name='" + item.name + "' and s.emp_code=" + emp_code + " and s.active=1;";
                                DataTable dt = await _sha.Get_Table_FromQry(qrySelect);
                                if (dt.Rows.Count > 0)
                                {

                                    string qry = "Update pr_emp_allowances_spl SET amount=" + amt + ", trans_id=@transidnew where m_id=(select s.m_id from pr_emp_allowances_spl s join pr_allowance_field_master m on m.id = s.m_id where m.name='" + item.name + "' and s.emp_code=" + emp_code + " and s.active=1)" + "  AND emp_code=" + emp_code + " ;";
                                    sbqry.Append(qry);

                                    //4. transaction touch
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_spl", "(select s.m_id from pr_emp_allowances_spl s join pr_allowance_field_master m on m.id = s.m_id where m.name='" + item.name + "' and s.emp_code=" + emp_code + " and s.active=1)", amt.ToString()));
                                }
                                else
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_allowances_spl", NewNumIndex));
                                    string qryy = "Insert into pr_emp_allowances_spl ([id],[emp_id],[emp_code],[fy],[fm]," +
                                      "[m_id],[m_type],[amount],[active],[trans_id]) values "
                              + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                             "" + emp_code + "," + FY + ",'" + FM + "',(select id from pr_allowance_field_master where name = '" + item.name + "'),'EMPSA', " + amt + ",1, @transidnew);";
                                    sbqry.Append(qryy);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_spl", "@idnew" + NewNumIndex, ""));
                                }
                            }
                        }
                    }
                }
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#Allowance#Allowance Data Submitted Successfully..!!";
                }
                else
                {
                    return "E#Allowance#Error While Allowance Data Submisson";
                }
            } else
            {
                return "E#Allowance#Please Submit Employee Details In Employee Master.";
            }
            
        }
        public async Task<string> GetEmpBranchAllowances(string searchempid)
        {
            string getalwance = "select distinct m.id as Id, m.description as Name,m.amount, " +
                " (CASE WHEN (month(e.to_date) >= month(fm) or year(e.to_date) >year(fm) )  THEN format(e.to_date ,'yyyy-MM-dd') " +
                " ELSE null " +
                " END) as to_date, " +
               " (CASE WHEN ((month(e.to_date) is null ) or month(e.to_date) >= month(fm) or year(e.to_date) >year(fm) )  THEN format(e.from_date,'yyyy-MM-dd') " +
               "  ELSE null " +
               "  END) as from_date " +
                " from pr_branch_allowance_master m " +
                "left outer join pr_emp_branch_allowances e on m.id = e.allowance_mid " +
                "and e.active = 1 and e.emp_code = " + searchempid + " where m.active = 1 ; ";
            DataTable dt = await _sha.Get_Table_FromQry(getalwance);
            var resultJson = JsonConvert.SerializeObject(new { allowancedata = dt });
            return resultJson;

        }


    }
}
