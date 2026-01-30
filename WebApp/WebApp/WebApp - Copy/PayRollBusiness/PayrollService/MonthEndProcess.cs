using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace PayRollBusiness.PayrollService
{
    public class MonthEndProcess : BusinessBase
    {
        log4net.ILog _logger = null;
        public MonthEndProcess(LoginCredential loginCredential, log4net.ILog logger) : base(loginCredential)
        {
            _logger = logger;
        }
        public async Task ServiceStarting(string servicename)
        {
            string qryIns = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action]) "
                + "VALUES(getdate(),'" + servicename + "','Start');";
            qryIns += "SELECT CAST(SCOPE_IDENTITY() as int);";
            await _sha.Run_INS_ExecuteScalar(qryIns);
        }
        public async Task ServiceStoping(string servicename)
        {
            string qryIns = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action]) "
                + "VALUES(getdate(),'" + servicename + "','Stop');";
            qryIns += "SELECT CAST(SCOPE_IDENTITY() as int);";

            await _sha.Run_INS_ExecuteScalar(qryIns);
        }

        public async Task<bool> Proc_JAIIB_CAIIB_AnnulIncr()
        {
            bool bRet = false;
            string qryGetdetails = "";
            qryGetdetails = "select distinct emp_code from pr_emp_pay_field;";
            DataTable dtJAIBCAIBempList = await _sha.Get_Table_FromQry(qryGetdetails);
            int emp_code = 0;
            int amount = 0;
            int m_id = 0;
            string qryGetJAIBCAIBDetails = "";

            foreach (DataRow dremp_code in dtJAIBCAIBempList.Rows)
            {

                emp_code = int.Parse(dremp_code["emp_code"].ToString());
                //emp_code = 337;
                amount++;
                m_id++;
                qryGetJAIBCAIBDetails += " declare @idnew" + amount + " int;";
                qryGetJAIBCAIBDetails += " select @idnew" + amount + " = sum(c.amount) from pr_emp_pay_field c join pr_earn_field_master m on c.m_id = m.id where c.emp_code =" + emp_code + " and m.type = 'pay_fields' and c.active = 1 and (m.name like '%CAIIB%' or m.name like  '%Basic%' or m.name like  '%JAIIB%'  or m.name like  '%Annual%' OR m.name like '%Stagnation Increments%');";
                qryGetJAIBCAIBDetails += " declare @id" + m_id + " int;";
                qryGetJAIBCAIBDetails += " select @id" + m_id + " =id from pr_earn_field_master where  type='pay_fields' and active=1 and  name like  '%Basic%' ;";
                qryGetJAIBCAIBDetails += " update pr_emp_pay_field set amount=@idnew" + amount + " where emp_code=" + emp_code + " and m_id=@id" + m_id + " and active=1;";
                qryGetJAIBCAIBDetails += " UPDATE PF SET amount = 0 FROM pr_emp_pay_field PF JOIN pr_earn_field_master FM ON PF.m_id = FM.id WHERE emp_code=" + emp_code + " and (FM.name like '%JAIIB%' OR FM.name like '%CAIIB%' OR FM.name like '%Annual%' OR FM.name like '%Stagnation Increments%');";


            }

            try
            {
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(qryGetJAIBCAIBDetails))
                {
                    bRet = true;
                    _logger.Info(emp_code + " " + "Processed Successfully....! ");
                }
            }
            catch (Exception ex)
            {

                _logger.Error(emp_code + "  " + ex);
                _logger.Error(ex.Message);
            }

            return bRet;
        }


        public async Task<bool> ProcedureAnnull_increments_month_end()
        {
            bool bRet = false;

            StringBuilder sbqry = new StringBuilder();

            string Iqry = "exec increments_month_end '" + PrConstants.dtIncrement + "'";
            sbqry.Append(Iqry);

            bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

            return bRet;
        }
        public async Task<bool> CopyEmpOldAllwDed()
        {
            bool bRet = false;
            // StringBuilder sbqry = new StringBuilder();

            string qryFinacemonth = "select fm,fy from pr_month_details where active= 1";
            string qryEmpCodes = "SELECT distinct emp_code from pr_emp_pay_field;";

            DataSet dtempcodesandfm = await _sha.Get_MultiTables_FromQry(qryFinacemonth + qryEmpCodes);

            DataTable dtfm = dtempcodesandfm.Tables[0];
            DataTable dtempcodes = dtempcodesandfm.Tables[1];


            //Finance month
            DateTime fmdate = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());
            int FY = Convert.ToInt32(dtfm.Rows[0]["fy"].ToString());
            string FM = fmdate.ToString("yyyy-MM-dd");

            // Next Month
            DateTime fmNextMonth = fmdate.AddMonths(1);

            string FMNextMonth = fmNextMonth.ToString("yyyy-MM-dd");

            DateTime dtTime =  Convert.ToDateTime(FMNextMonth);
            int fYear = dtTime.Year;
            int fMonth = dtTime.Month;

            if(fMonth==4)
            {
                FY = FY + 1;
            }

            int NewNumIndex = 0;

            string qry = "";
            string Employee_codes = "";

            StringBuilder trnsqry = new StringBuilder();
            //trans_id
            trnsqry.Append(GenNewTransactionString());

            foreach (DataRow ec in dtempcodes.Rows)
            {
                //int emp_code = 6352;
                int emp_code = Convert.ToInt32(ec["emp_code"].ToString());
                Employee_codes += emp_code.ToString() + ",";


                string qrygetPayfields = "SELECT id,m_id,m_type,amount FROM pr_emp_pay_field WHERE emp_code=" + emp_code + " AND active=1;";

                string qrygetSplAllowances = "SELECT spl.id,spl.m_id,spl.m_type,spl.amount FROM pr_emp_allowances_spl spl " +
                                           " JOIN pr_allowance_field_master mas on spl.m_id = mas.id " +
                                           " WHERE emp_code =" + emp_code + " AND spl.active = 1 AND spl.m_id NOT IN (SELECT fieldmas.id from pr_branch_allowance_master allowmas " +
                                           " JOIN pr_allowance_field_master fieldmas on allowmas.description = fieldmas.name WHERE fieldmas.type = 'EMPSA')";

                string qrygetGenAllowances = "SELECT  gen.id,gen.m_id,gen.m_type,gen.amount FROM pr_emp_allowances_gen gen " +
                                            " JOIN pr_allowance_field_master mas on gen.m_id = mas.id " +
                                            " WHERE emp_code =" + emp_code + " AND gen.active = 1 " +
                                            " AND gen.m_id NOT IN(SELECT id FROM pr_allowance_field_master mas WHERE name IN('Br Manager Allowance'))";

                string qrygetDeductions = "SELECT * FROM pr_emp_deductions WHERE emp_code=" + emp_code + " AND active=1;";

                string qrygetLICDeductions = "SELECT * FROM pr_emp_lic_details WHERE emp_code=" + emp_code + " AND active=1 and stop='No';";

                string qryGetHFCDeductions = "SELECT * FROM pr_emp_hfc_details WHERE emp_code=" + emp_code + " AND active=1 and stop='No';";

                string qryGetEmpDetails = "SELECT id FROM employees WHERE empid=" + emp_code + ";";

                string qryGetgeneral = "SELECT * FROM pr_emp_general WHERE emp_code=" + emp_code + " AND active=1;";

                string qryGetBilogical = "SELECT * FROM pr_emp_biological_field WHERE emp_code=" + emp_code + " AND active=1;";


                DataSet dsGetLstMnthRecords = await _sha.Get_MultiTables_FromQry(qrygetPayfields + qrygetSplAllowances + qrygetGenAllowances + qrygetDeductions + qrygetLICDeductions + qryGetHFCDeductions + qryGetEmpDetails + qryGetgeneral + qryGetBilogical);

                var PayfieldsData = dsGetLstMnthRecords.Tables[0];
                var SPLAllowancesData = dsGetLstMnthRecords.Tables[1];
                var GENAllowancesData = dsGetLstMnthRecords.Tables[2];
                var DeductionsData = dsGetLstMnthRecords.Tables[3];
                var LICDeductionsData = dsGetLstMnthRecords.Tables[4];
                var HFCDeductionsData = dsGetLstMnthRecords.Tables[5];
                var EmpDetails = dsGetLstMnthRecords.Tables[6];
                var Emp_General = dsGetLstMnthRecords.Tables[7];
                var Emp_Bilogical = dsGetLstMnthRecords.Tables[8];

                //empdetails from employees
                if (EmpDetails.Rows.Count > 0)
                {
                    try
                    {
                        DataRow Edls = EmpDetails.Rows[0];
                        var pkid = Edls["id"].ToString();

                        StringBuilder sbqry = new StringBuilder();

                        //fields to read payfileds,spl_alw,gen_alw,deductions
                        int c_id = 0;
                        int c_m_id = 0;
                        string c_m_type = "";
                        int c_amount = 0;
                        string empList = "";



                        //fields to read lic and hfc deductions
                        var c_account_no = "";
                        string c_paytype = "";
                        string c_paymonths = "";
                        string c_stop = "";
                        int c_stop_month = 0;


                        //payfieldsdata pr_emp_pay_field
                        int Rowcount = 0;
                        int Loopcount = 0;
                        if (PayfieldsData.Rows.Count > 0)
                        {
                            foreach (DataRow epf in PayfieldsData.Rows)
                            {
                                Rowcount = PayfieldsData.Rows.Count;
                                Loopcount = Loopcount + 1;
                                c_id = Convert.ToInt32(epf["id"]);

                                empList += c_id.ToString() + ",";
                                //arr = arr.Remove(arr.Length - 1, 1);

                                c_m_id = Convert.ToInt32(epf["m_id"]);
                                c_m_type = epf["m_type"].ToString();

                                if (!string.IsNullOrWhiteSpace((epf["amount"]).ToString()))
                                {
                                    c_amount = Convert.ToInt32((epf["amount"]));
                                }


                                NewNumIndex++;
                                //gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_pay_field", NewNumIndex));

                                string Iqry = "insert into pr_emp_pay_field ([id],[emp_id],[emp_code],[fy]," +
                                    "[fm],[m_id],[m_type],[amount],[active],[trans_id]) VALUES "
                                    + "(@idnew" + NewNumIndex + "," + pkid + "," +
                                    "" + emp_code + "," + FY + ",'" + FMNextMonth + "'," + c_m_id + ",'" + c_m_type + "', " + c_amount + ",1, @transidnew);";
                                sbqry.Append(Iqry);

                                //transaction touch
                                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_pay_field", "@idnew" + NewNumIndex, ""));
                                //update oldid 
                                if (Loopcount == Rowcount)
                                {
                                    empList = empList.Remove(empList.Length - 1);
                                    sbqry.Append("UPDATE pr_emp_pay_field SET active=0 WHERE id in (" + empList + ") AND emp_code=" + emp_code + ";");
                                    // sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", "@idnew" + NewNumIndex, c_id.ToString()));
                                }

                            }

                        }

                        if (SPLAllowancesData.Rows.Count > 0)
                        {
                            Rowcount = 0;
                            Loopcount = 0;
                            empList = "";
                            foreach (DataRow esad in SPLAllowancesData.Rows)
                            {
                                Rowcount = SPLAllowancesData.Rows.Count;
                                Loopcount = Loopcount + 1;
                                c_id = Convert.ToInt32(esad["id"]);
                                empList += c_id.ToString() + ",";

                                c_m_id = Convert.ToInt32(esad["m_id"]);
                                c_m_type = esad["m_type"].ToString();

                                if (!string.IsNullOrWhiteSpace((esad["amount"]).ToString()))
                                {
                                    c_amount = Convert.ToInt32((esad["amount"]));

                                }


                                NewNumIndex++;
                                //gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_allowances_spl", NewNumIndex));

                                //qry
                                qry = "Insert into pr_emp_allowances_spl ([id],[emp_id],[emp_code],[fy],[fm]," +
                                    "[m_id],[m_type],[amount],[active],[trans_id]) values "
                                    + "(@idnew" + NewNumIndex + "," + pkid + "," + emp_code + "," + FY + ",'" + FMNextMonth + "'," +
                                    "" + c_m_id + ",'" + c_m_type + "', " + c_amount + ",1, @transidnew);";
                                sbqry.Append(qry);

                                //transaction touch
                                if (Loopcount == Rowcount)
                                {
                                    empList = empList.Remove(empList.Length - 1);
                                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_spl", "@idnew" + NewNumIndex, ""));
                                    sbqry.Append("UPDATE pr_emp_allowances_spl SET active=0 WHERE id in (" + empList + ") AND emp_code=" + emp_code + ";");
                                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_spl", "@idnew" + NewNumIndex, c_id.ToString()));
                                }
                            }

                        }

                        if (GENAllowancesData.Rows.Count > 0)
                        {
                            Rowcount = 0;
                            Loopcount = 0;
                            empList = "";
                            foreach (DataRow egad in GENAllowancesData.Rows)
                            {
                                Rowcount = GENAllowancesData.Rows.Count;
                                Loopcount = Loopcount + 1;

                                c_id = Convert.ToInt32(egad["id"]);
                                empList += c_id.ToString() + ",";

                                c_m_id = Convert.ToInt32(egad["m_id"]);
                                c_m_type = egad["m_type"].ToString();

                                if (!string.IsNullOrWhiteSpace((egad["amount"]).ToString()))
                                {
                                    c_amount = Convert.ToInt32((egad["amount"]));
                                }

                                NewNumIndex++;
                                //gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_allowances_gen", NewNumIndex));

                                //qry
                                qry = "Insert into pr_emp_allowances_gen ([id],[emp_id],[emp_code],[fy],[fm]," +
                                    "[m_id],[m_type],[amount],[active],[trans_id]) values "
                                    + "(@idnew" + NewNumIndex + "," + pkid + "," + emp_code + "," + FY + ",'" + FMNextMonth + "'," +
                                    "" + c_m_id + ",'" + c_m_type + "', " + c_amount + ",1, @transidnew);";
                                sbqry.Append(qry);

                                //transaction touch
                                if (Loopcount == Rowcount)
                                {
                                    empList = empList.Remove(empList.Length - 1);
                                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_gen", "@idnew" + NewNumIndex, ""));
                                    sbqry.Append("UPDATE pr_emp_allowances_gen SET active=0 WHERE id in (" + empList + ") AND emp_code=" + emp_code + ";");
                                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_gen", "@idnew" + NewNumIndex, c_id.ToString()));
                                }
                            }
                        }


                        if (DeductionsData.Rows.Count > 0)
                        {
                            Rowcount = 0;
                            Loopcount = 0;
                            empList = "";
                            foreach (DataRow edd in DeductionsData.Rows)
                            {
                                Rowcount = DeductionsData.Rows.Count;
                                Loopcount = Loopcount + 1;
                                c_id = Convert.ToInt32(edd["id"]);
                                empList += c_id.ToString() + ",";

                                c_m_id = Convert.ToInt32(edd["m_id"]);
                                c_m_type = edd["m_type"].ToString();

                                if (!string.IsNullOrWhiteSpace(edd["amount"].ToString()))
                                {
                                    c_amount = Convert.ToInt32((edd["amount"]));
                                }

                                NewNumIndex++;
                                //gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_deductions", NewNumIndex));

                                //qry
                                qry = "Insert into pr_emp_deductions ([id],[emp_id],[emp_code],[fy],[fm]," +
                                    "[m_id],[m_type],[amount],[active],[trans_id]) values "
                                    + "(@idnew" + NewNumIndex + "," + pkid + "," + emp_code + "," + FY + ",'" + FMNextMonth + "'," +
                                    "" + c_m_id + ",'" + c_m_type + "', " + c_amount + ",1, @transidnew);";
                                sbqry.Append(qry);

                                //transaction touch
                                if (Loopcount == Rowcount)
                                {
                                    empList = empList.Remove(empList.Length - 1);
                                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_deductions", "@idnew" + NewNumIndex, ""));
                                    sbqry.Append("UPDATE pr_emp_deductions SET active=0 WHERE id in (" + empList + ") AND emp_code=" + emp_code + ";");
                                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_deductions", "@idnew" + NewNumIndex, c_id.ToString()));
                                }
                            }

                        }

                        if (LICDeductionsData.Rows.Count > 0)
                        {
                            Rowcount = 0;
                            Loopcount = 0;
                            empList = "";
                            foreach (DataRow eldd in LICDeductionsData.Rows)
                            {

                                Rowcount = LICDeductionsData.Rows.Count;
                                Loopcount = Loopcount + 1;

                                c_id = Convert.ToInt32(eldd["id"]);
                                empList += c_id.ToString() + ",";

                                if (!string.IsNullOrWhiteSpace(eldd["account_no"].ToString()))
                                {
                                    c_account_no = eldd["account_no"].ToString();
                                }

                                c_amount = Convert.ToInt32(eldd["amount"]);
                                c_paytype = eldd["pay_type"].ToString();
                                c_paymonths = eldd["pay_months"].ToString();
                                c_stop = eldd["stop"].ToString();
                                //c_stop_month = Convert.ToInt32(ehdd["stop_month"]);
                                NewNumIndex++;

                                //2. gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_lic_details", NewNumIndex));

                                if (c_account_no != null && c_account_no != "")
                                {
                                    //3. qry
                                    qry = "INSERT INTO pr_emp_lic_details ([id],[fy],[fm],[emp_id],[emp_code],[account_no]," +
                                        "[amount],[pay_type],[pay_months],[stop],[stop_month],[active],[trans_id]) values(@idnew" + NewNumIndex + "," + FY + "," +
                                        "'" + FMNextMonth + "'," + pkid + "," + emp_code + "," + c_account_no + "," + c_amount + ",'" + c_paytype + "','" + c_paymonths + "','" + c_stop + "'," + c_stop_month + ",1,@transidnew);";
                                }
                                else
                                {
                                    //3. qry
                                    qry = "INSERT INTO pr_emp_lic_details ([id],[fy],[fm],[emp_id],[emp_code]," +
                                        "[amount],[pay_type],[pay_months],[stop],[stop_month],[active],[trans_id]) values(@idnew" + NewNumIndex + "," + FY + "," +
                                        "'" + FMNextMonth + "'," + pkid + "," + emp_code + "," + c_amount + ",'" + c_paytype + "','" + c_paymonths + "','" + c_stop + "'," + c_stop_month + ",1,@transidnew);";
                                }


                                sbqry.Append(qry);
                                if (Loopcount == Rowcount)
                                {
                                    empList = empList.Remove(empList.Length - 1);
                                    // sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_lic_details", "@idnew" + NewNumIndex, ""));
                                    sbqry.Append("UPDATE pr_emp_lic_details SET active = 0 WHERE id in (" + empList + ") AND emp_code=" + emp_code + ";");
                                    // sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_lic_details", "@idnew" + NewNumIndex, c_id.ToString()));
                                }
                            }

                        }
                        if (HFCDeductionsData.Rows.Count > 0)
                        {
                            Rowcount = 0;
                            Loopcount = 0;
                            empList = "";
                            foreach (DataRow ehdd in HFCDeductionsData.Rows)
                            {
                                Rowcount = HFCDeductionsData.Rows.Count;
                                Loopcount = Loopcount + 1;
                                c_id = Convert.ToInt32(ehdd["id"]);
                                empList += c_id.ToString() + ",";

                                if (!string.IsNullOrWhiteSpace(ehdd["account_no"].ToString()))
                                {
                                    c_account_no = ehdd["account_no"].ToString();
                                }

                                c_amount = Convert.ToInt32(ehdd["amount"]);
                                c_paytype = ehdd["pay_type"].ToString();
                                c_paymonths = ehdd["pay_months"].ToString();
                                c_stop = ehdd["stop"].ToString();
                                //c_stop_month = Convert.ToInt32(ehdd["stop_month"]);
                                NewNumIndex++;
                                //2. gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_hfc_details", NewNumIndex));

                                if (c_account_no != null && c_account_no != "")
                                {
                                    //3. qry
                                    qry = "INSERT INTO pr_emp_hfc_details ([id],[fy],[fm],[emp_id],[emp_code],[account_no]," +
                                        "[amount],[pay_type],[pay_months],[stop],[stop_month],[active],[trans_id]) values(@idnew" + NewNumIndex + "," + FY + "," +
                                        "'" + FMNextMonth + "'," + pkid + "," + emp_code + ",'" + c_account_no + "'," + c_amount + ",'" + c_paytype + "','" + c_paymonths + "','" + c_stop + "'," + c_stop_month + ",1,@transidnew);";
                                }
                                else
                                {
                                    //3. qry
                                    qry = "INSERT INTO pr_emp_hfc_details ([id],[fy],[fm],[emp_id],[emp_code]," +
                                        "[amount],[pay_type],[pay_months],[stop],[stop_month],[active],[trans_id]) values(@idnew" + NewNumIndex + "," + FY + "," +
                                        "'" + FMNextMonth + "'," + pkid + "," + emp_code + "," + c_amount + ",'" + c_paytype + "','" + c_paymonths + "','" + c_stop + "'," + c_stop_month + ",1,@transidnew);";
                                }


                                sbqry.Append(qry);
                                if (Loopcount == Rowcount)
                                {
                                    empList = empList.Remove(empList.Length - 1);
                                    // sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_hfc_details", "@idnew" + NewNumIndex, ""));
                                    sbqry.Append("UPDATE pr_emp_hfc_details SET active = 0 WHERE id in (" + empList + ") AND emp_code=" + emp_code + ";");
                                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_hfc_details", "@idnew" + NewNumIndex, c_id.ToString()));
                                }
                            }
                        }

                        if (Emp_General.Rows.Count > 0)
                        {
                            Rowcount = 0;
                            Loopcount = 0;
                            empList = "";
                            foreach (DataRow empgeneral in Emp_General.Rows)
                            {
                                Rowcount = Emp_General.Rows.Count;
                                Loopcount = Loopcount + 1;

                                string gender = empgeneral["sex"].ToString();
                                string martial_status = empgeneral["martial_status"].ToString();

                                string zone = "";
                                string region_for_p_tax = "";
                                string p_tax_region = "";
                                string native_place = "";
                                string division = "";
                                string pf_no = "";
                                string uan_no = "";
                                string c_doj_pf = null;
                                string identify_mark1 = "";
                                string identity_mark2 = "";
                                string religion = "";
                                string cur_reservation = "";
                                string join_reservation = "";
                                string pan_no = "";
                                string branch_code = "";
                                string pay_bank = "";
                                string account_code = "";
                                string bank_accno = "";
                                string customer_id = "";
                                string acc_with_dccb = "";
                                string stl_temp = "";
                                string fest_adv = "";
                                string artr_emp = "";
                                string email_id = "";

                                if (!string.IsNullOrWhiteSpace(empgeneral["zone"].ToString()))
                                {
                                    zone = empgeneral["zone"].ToString();
                                }

                                if (!string.IsNullOrWhiteSpace(empgeneral["region_for_p_tax"].ToString()))
                                {
                                    region_for_p_tax = empgeneral["region_for_p_tax"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["p_tax_region"].ToString()))
                                {
                                    p_tax_region = empgeneral["p_tax_region"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["native_place"].ToString()))
                                {
                                    native_place = empgeneral["native_place"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["division"].ToString()))
                                {
                                    division = empgeneral["division"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["pf_no"].ToString()))
                                {
                                    pf_no = empgeneral["pf_no"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["uan_no"].ToString()))
                                {
                                    uan_no = empgeneral["uan_no"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["doj_pf"].ToString()))
                                {
                                    DateTime doj_pf = Convert.ToDateTime(empgeneral["doj_pf"].ToString());
                                    c_doj_pf = doj_pf.ToString("yyyy-MM-dd");
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["identify_mark1"].ToString()))
                                {
                                    identify_mark1 = empgeneral["identify_mark1"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["identify_mark2"].ToString()))
                                {
                                    identity_mark2 = empgeneral["identify_mark2"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["religion"].ToString()))
                                {
                                    religion = empgeneral["religion"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["cur_reservation"].ToString()))
                                {
                                    cur_reservation = empgeneral["cur_reservation"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["join_reservation"].ToString()))
                                {
                                    join_reservation = empgeneral["join_reservation"].ToString();

                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["pan_no"].ToString()))
                                {
                                    pan_no = empgeneral["pan_no"].ToString();
                                }

                                if (!string.IsNullOrWhiteSpace(empgeneral["branch_code"].ToString()))
                                {
                                    branch_code = empgeneral["branch_code"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["pay_bank"].ToString()))
                                {
                                    pay_bank = empgeneral["pay_bank"].ToString();
                                }

                                if (!string.IsNullOrWhiteSpace(empgeneral["account_code"].ToString()))
                                {
                                    account_code = empgeneral["account_code"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["bank_accno"].ToString()))
                                {
                                    bank_accno = empgeneral["bank_accno"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["customer_id"].ToString()))
                                {
                                    customer_id = empgeneral["customer_id"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["acc_with_dccb"].ToString()))
                                {
                                    acc_with_dccb = empgeneral["acc_with_dccb"].ToString();
                                }
                                //
                                if (!string.IsNullOrWhiteSpace(empgeneral["stl_temp"].ToString()))
                                {
                                    stl_temp = empgeneral["stl_temp"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["fest_adv"].ToString()))
                                {
                                    fest_adv = empgeneral["fest_adv"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["artr_emp"].ToString()))
                                {
                                    artr_emp = empgeneral["artr_emp"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(empgeneral["email_id"].ToString()))
                                {
                                    email_id = empgeneral["email_id"].ToString();
                                }



                                string designation = empgeneral["designation"].ToString();
                                string designation_category = empgeneral["designation_category"].ToString();
                                string address = empgeneral["address"].ToString();
                                string per_address = empgeneral["per_address"].ToString();
                                string per_phoneno = empgeneral["per_phoneno"].ToString();
                                string blood_group = empgeneral["blood_group"].ToString();
                                string aadhaar_no = empgeneral["aadhaar_no"].ToString();
                                DateTime dtdob = Convert.ToDateTime(empgeneral["dob"].ToString());
                                int reg_order = 0;

                                if (!string.IsNullOrWhiteSpace(empgeneral["reg_order"].ToString()))
                                {
                                    reg_order = Convert.ToInt32(empgeneral["reg_order"].ToString());
                                }

                                string dob = dtdob.ToString("yyyy-MM-dd");
                                string exp = empgeneral["exp"].ToString();

                                int designation_no = 0;

                                if (!string.IsNullOrWhiteSpace(empgeneral["designation_no"].ToString()))
                                {
                                    designation_no = Convert.ToInt32(empgeneral["designation_no"].ToString());

                                }

                                bool phy_handicapped = Convert.ToBoolean(empgeneral["phy_handicapped"].ToString());
                                bool house_provided = Convert.ToBoolean(empgeneral["house_provided"].ToString());

                                int emp_age = Convert.ToInt32(empgeneral["emp_age"].ToString());


                                NewNumIndex++;

                                //if(Loopcount=)
                                sbqry.Append("UPDATE pr_emp_general SET active = 0 WHERE  emp_code=" + emp_code + ";");

                                //2. gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_general", NewNumIndex));
                                //3. qry
                                if (c_doj_pf != null)
                                {
                                    qry = "INSERT INTO pr_emp_general ([emp_id],[fy],[fm],[emp_code],[sex]," +
                                   "[martial_status],[zone],[designation],[designation_category],[region_for_p_tax]," +
                                   "[p_tax_region],[address],[per_address],[per_phoneno],[native_place]," +
                                   "[division],[pf_no],[uan_no],[doj_pf],[email_id],[identify_mark1],[identify_mark2]," +
                                   "[blood_group],[religion],[cur_reservation],[join_reservation],[pan_no],[reg_order],[branch_code],[pay_bank],[account_code],[bank_accno],[customer_id],[acc_with_dccb],[phy_handicapped],[house_provided],[emp_age],[designation_no],[stl_temp],[fest_adv],[artr_emp],[aadhaar_no]," +
                                   "[dob],[exp],[active],[trans_id]) values(" + pkid + "," + FY + ",'" + FMNextMonth + "'," + emp_code + ",'" + gender + "','" + martial_status + "','" + zone + "','" + designation + "','" + designation_category + "','" + region_for_p_tax + "','" + p_tax_region + "','" + address + "','" + per_address + "','" + per_phoneno + "','" + native_place + "','" + division + "','" + pf_no + "','" + uan_no + "','" + c_doj_pf + "','" + email_id + "','" + identify_mark1 + "','" + identity_mark2 + "','" + blood_group + "','" + religion + "','" + cur_reservation + "','" + join_reservation + "','" + pan_no + "'," + reg_order + ",'" + branch_code + "','" + pay_bank + "','" + account_code + "','"
                                   + bank_accno + "','" + customer_id + "','" + acc_with_dccb + "','" + phy_handicapped + "','" + house_provided + "'," + emp_age + "," + designation_no + ",'" + stl_temp + "','" + fest_adv + "','" + artr_emp + "','" + aadhaar_no + "','" + dob + "','" + exp + "',1,@transidnew);";
                                }
                                else
                                {
                                    qry = "INSERT INTO pr_emp_general ([emp_id],[fy],[fm],[emp_code],[sex]," +
                                  "[martial_status],[zone],[designation],[designation_category],[region_for_p_tax]," +
                                  "[p_tax_region],[address],[per_address],[per_phoneno],[native_place]," +
                                  "[division],[pf_no],[uan_no],[email_id],[identify_mark1],[identify_mark2]," +
                                  "[blood_group],[religion],[cur_reservation],[join_reservation],[pan_no],[reg_order],[branch_code],[pay_bank],[account_code],[bank_accno],[customer_id],[acc_with_dccb],[phy_handicapped],[house_provided],[emp_age],[designation_no],[stl_temp],[fest_adv],[artr_emp],[aadhaar_no]," +
                                  "[dob],[exp],[active],[trans_id]) values(" + pkid + "," + FY + ",'" + FMNextMonth + "'," + emp_code + ",'" + gender + "','" + martial_status + "','" + zone + "','" + designation + "','" + designation_category + "','" + region_for_p_tax + "','" + p_tax_region + "','" + address + "','" + per_address + "','" + per_phoneno + "','" + native_place + "','" + division + "','" + pf_no + "','" + uan_no + "','" + email_id + "','" + identify_mark1 + "','" + identity_mark2 + "','" + blood_group + "','" + religion + "','" + cur_reservation + "','" + join_reservation + "','" + pan_no + "'," + reg_order + ",'" + branch_code + "','" + pay_bank + "','" + account_code + "','"
                                  + bank_accno + "','" + customer_id + "','" + acc_with_dccb + "','" + phy_handicapped + "','" + house_provided + "'," + emp_age + "," + designation_no + ",'" + stl_temp + "','" + fest_adv + "','" + artr_emp + "','" + aadhaar_no + "','" + dob + "','" + exp + "',1,@transidnew);";
                                }

                                sbqry.Append(qry);
                                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_general", "@idnew" + NewNumIndex, ""));

                            }
                        }

                        if (Emp_Bilogical.Rows.Count > 0)
                        {
                            foreach (DataRow drbio in Emp_Bilogical.Rows)
                            {
                                string father_husband_name = "";
                                string fh_relation = "";
                                string c_father_dob = null;

                                if (!string.IsNullOrWhiteSpace(drbio["father_husband_name"].ToString()))
                                {
                                    father_husband_name = drbio["father_husband_name"].ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(drbio["father_dob"].ToString()))
                                {
                                    DateTime dtfather_dob = Convert.ToDateTime(drbio["father_dob"].ToString());
                                    c_father_dob = dtfather_dob.ToString("yyyy-MM-dd");
                                }
                                if (!string.IsNullOrWhiteSpace(drbio["f/h_relation"].ToString()))
                                {
                                    fh_relation = drbio["f/h_relation"].ToString();
                                }


                                NewNumIndex++;

                                sbqry.Append("UPDATE pr_emp_biological_field SET active = 0 WHERE  emp_code=" + emp_code + ";");

                                //2. gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_biological_field", NewNumIndex));
                                //3. qry
                                if (c_father_dob != null)
                                {


                                    qry = "INSERT INTO pr_emp_biological_field ([emp_code],[emp_id],[fy],[fm],[father_husband_name]," +
                                        " [father_dob],[f/h_relation] ,[active],[trans_id]) values(" + emp_code + "," + pkid + "," + FY + ",'" + FMNextMonth + "','" + father_husband_name + "','" + c_father_dob + "','" + fh_relation + "',1,@transidnew);";
                                }
                                else
                                {


                                    qry = "INSERT INTO pr_emp_biological_field ([emp_code],[emp_id],[fy],[fm],[father_husband_name]," +
                                        " [f/h_relation] ,[active],[trans_id]) values(" + emp_code + "," + pkid + "," + FY + ",'" + FMNextMonth + "','" + father_husband_name + "','" + fh_relation + "',1,@transidnew);";
                                }

                                sbqry.Append(qry);

                                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_biological_field", "@idnew" + NewNumIndex, ""));
                            }

                        }

                        if (await _sha.Run_UPDDEL_ExecuteNonQuery(trnsqry + sbqry.ToString()))
                        {
                            bRet = true;

                            _logger.Info(emp_code + " " + "Processed Successfully....! ");
                        }


                    }
                    catch (Exception ex)
                    {

                        _logger.Error(emp_code + "  " + ex);

                        _logger.Error(ex.Message);
                        _logger.Error(ex.StackTrace);
                    }

                }

                // }
            }
            return bRet;
        }


        #region // Branch Allowances_General_Special
        public async Task<bool> UpdateBranchAllowances_Genearl_Special()
        {
            bool bRet = false;

            string qryFinacemonth = "select fm,fy from pr_month_details where active= 1";
            string qryEmpCodes = "SELECT distinct emp_code from pr_emp_branch_allowances WHERE Active=1";

            DataSet dtempcodesandfm = await _sha.Get_MultiTables_FromQry(qryFinacemonth + qryEmpCodes);

            DataTable dtfm = dtempcodesandfm.Tables[0];
            DataTable dtemp = dtempcodesandfm.Tables[1];

            //Finance month
            DateTime fmdate = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());
            int FY = Convert.ToInt32(dtfm.Rows[0]["fy"].ToString());
            string FM = fmdate.ToString("yyyy-MM-dd");

            // Next Month
            DateTime fmNextMonth = fmdate.AddMonths(1);

            string FMNextMonth = fmNextMonth.ToString("yyyy-MM-dd");

            int NewNumIndex = 0;

            string qry = "";
            int emp_code = 0;

            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());

            if (dtemp.Rows.Count > 0)
            {
                foreach (DataRow dremp in dtemp.Rows)
                {
                    emp_code = Convert.ToInt32(dremp["emp_code"].ToString());

                    string getbranch_general_allow = "SELECT emp_bra_allow.emp_code,emp_bra_allow.emp_id,mas.amount,mas.description,filedmas.id as filedmas_id,filedmas.type,emp_bra_allow.id as branch_all_id,emp_bra_allow.from_date,emp_bra_allow.to_date from pr_emp_branch_allowances emp_bra_allow " +
                                                     " join pr_branch_allowance_master mas on emp_bra_allow.allowance_mid = mas.id " +
                                                     " join pr_allowance_field_master filedmas on mas.description = filedmas.name " +
                                                     "WHERE emp_code =" + emp_code + " AND  emp_bra_allow.active=1 Order by description";

                    DataTable dtbranch_allow = await _sha.Get_Table_FromQry(getbranch_general_allow);

                    //Branch General Allowance
                    if (dtbranch_allow.Rows.Count > 0)
                    {
                        foreach (DataRow drgen in dtbranch_allow.Rows)
                        {
                            string description = drgen["description"].ToString();
                            DateTime dtfrom_date = DateTime.Parse(drgen["from_date"].ToString());
                            DateTime dtTodate = new DateTime();
                            double branch_amount = Convert.ToDouble(drgen["amount"].ToString());
                            var pkid = drgen["emp_id"].ToString();
                            int m_id = Convert.ToInt32(drgen["filedmas_id"].ToString());
                            string c_m_type = drgen["type"].ToString();
                            int branch_alloe_id = Convert.ToInt32(drgen["branch_all_id"].ToString());


                            string Todate = "";
                            int days = 0;
                            int totaldays = 0;
                            double gen_amount = 0;

                            if (!string.IsNullOrWhiteSpace(drgen["to_date"].ToString()))
                            {
                                dtTodate = DateTime.Parse(drgen["to_date"].ToString());
                                Todate = dtTodate.ToString("yyyy-MM-dd");
                            }

                            string Formdate = dtfrom_date.ToString("yyyy-MM-dd");
                            string _dtfrom_date = dtfrom_date.ToString("yyyy-MM");
                            string _Fmdate = fmdate.ToString("yyyy-MM");
                            string _dtTodate = dtTodate.ToString("yyyy-MM");
                            string _dtnextfm = fmNextMonth.ToString("yyyy-MM");


                            //1.Form Date And Finance Same Todate Is Null
                            if (_dtfrom_date == _Fmdate && (Todate == null || Todate == ""))
                            {

                                totaldays = fmNextMonth.AddMonths(1).AddDays(-1).Day;
                                gen_amount = ((branch_amount / totaldays) * days);

                                // sbqry.Append("UPDATE pr_emp_branch_allowances SET Active=0 WHERE id=" + branch_alloe_id + " AND emp_code=" + emp_code + ";");
                            }
                            //2.Form Date And Finance Same Todate  Finance Month
                            else if (_dtfrom_date == _Fmdate && _dtTodate == _Fmdate)
                            {
                                gen_amount = 0;

                                //sbqry.Append("UPDATE pr_emp_branch_allowances SET Active=0 WHERE id=" + branch_alloe_id + " AND emp_code=" + emp_code + ";");
                            }

                            //3.Form Date And Not in Finance Todate is Not in Finance 
                            else if (_dtfrom_date == _Fmdate && _dtTodate == _dtnextfm)
                            {
                                //no. of days
                                days = dtTodate.Day;
                                totaldays = fmdate.AddMonths(1).AddDays(-1).Day;

                                gen_amount = ((branch_amount / totaldays) * days);

                                //sbqry.Append("UPDATE pr_emp_branch_allowances SET From_date='" + FMNextMonth + "' WHERE id=" + branch_alloe_id + " AND emp_code=" + emp_code + ";");

                            }
                            //5.Form Date And Not in Finance Todate is Not in Finance 
                            else if (_dtfrom_date == _dtnextfm && _dtTodate == _dtnextfm)
                            {
                                //no. of days
                                days = fmdate.AddMonths(1).AddDays(-1).Day;

                                gen_amount = ((branch_amount / days) * days);

                                // sbqry.Append("UPDATE pr_emp_branch_allowances SET From_date='" + FMNextMonth + "' WHERE id=" + branch_alloe_id + " AND emp_code=" + emp_code + ";");

                            }

                            //6.Form Date And Previous month this Month Empty
                            else if (dtfrom_date < fmNextMonth && _dtTodate == null || _dtTodate == "")
                            {
                                //no. of days
                                days = fmNextMonth.AddMonths(1).AddDays(-1).Day;

                                gen_amount = ((branch_amount / days) * days);

                                // sbqry.Append("UPDATE pr_emp_branch_allowances SET From_date='" + FMNextMonth + "' WHERE id=" + branch_alloe_id + " AND emp_code=" + emp_code + ";");

                            }

                            //7.Form Date And Previous month this Month Empty
                            else if (dtfrom_date < fmNextMonth && _dtTodate == _dtnextfm)
                            {
                                //no. of days

                                days = dtTodate.Day;

                                totaldays = fmNextMonth.AddMonths(1).AddDays(-1).Day;

                                gen_amount = ((branch_amount / totaldays) * days);

                                // sbqry.Append("UPDATE pr_emp_branch_allowances SET From_date='" + FMNextMonth + "' WHERE id=" + branch_alloe_id + " AND emp_code=" + emp_code + ";");

                            }




                            //Branch pr_emp_allowances_gen
                            if (description == "Br Manager Allowance")
                            {
                                string getbr_gen_allow = "SELECT gen.id,gen.m_id,gen.m_type,gen.amount FROM pr_emp_allowances_gen gen " +
                                               " WHERE emp_code =" + emp_code + " AND gen.active = 1 AND gen.m_id=" + m_id;

                                DataTable dtbr_gen = await _sha.Get_Table_FromQry(getbr_gen_allow);

                                NewNumIndex++;
                                //gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_allowances_gen", NewNumIndex));

                                if (dtbr_gen.Rows.Count > 0 && gen_amount >= 0)
                                {
                                    int c_id = int.Parse(dtbr_gen.Rows[0]["id"].ToString());

                                    sbqry.Append("UPDATE pr_emp_allowances_gen SET active=0 WHERE id=" + c_id + " AND emp_code=" + emp_code + ";");
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_gen", "@idnew" + NewNumIndex, c_id.ToString()));

                                }

                                if (gen_amount > 0)
                                {
                                    //qry
                                    qry = "Insert into pr_emp_allowances_gen ([id],[emp_id],[emp_code],[fy],[fm]," +
                                        "[m_id],[m_type],[amount],[active],[trans_id]) values "
                                        + "(@idnew" + NewNumIndex + "," + pkid + "," + emp_code + "," + FY + ",'" + FMNextMonth + "'," +
                                        "" + m_id + ",'" + c_m_type + "', " + gen_amount + ",1, @transidnew);";
                                    sbqry.Append(qry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_gen", "@idnew" + NewNumIndex, ""));
                                }

                            }
                            else
                            {
                                string getbr_spl_allow = "SELECT spl.id,spl.m_id,spl.m_type,spl.amount FROM pr_emp_allowances_spl spl " +
                                              " WHERE emp_code =" + emp_code + " AND spl.active = 1 AND spl.m_id=" + m_id;

                                DataTable dtbr_spl = await _sha.Get_Table_FromQry(getbr_spl_allow);

                                NewNumIndex++;
                                //gen new num
                                sbqry.Append(GetNewNumStringArr("pr_emp_allowances_spl", NewNumIndex));

                                if (dtbr_spl.Rows.Count > 0 && gen_amount >= 0)
                                {
                                    int c_id = int.Parse(dtbr_spl.Rows[0]["id"].ToString());
                                    //double amount = double.Parse(dtbr_spl.Rows[0]["amount"].ToString());

                                    //transaction touch
                                    sbqry.Append("UPDATE pr_emp_allowances_spl SET active=0 WHERE id=" + c_id + " AND emp_code=" + emp_code + ";");
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_spl", "@idnew" + NewNumIndex, c_id.ToString()));

                                }

                                if (gen_amount > 0)
                                {
                                    //qry
                                    qry = "Insert into pr_emp_allowances_spl ([id],[emp_id],[emp_code],[fy],[fm]," +
                                        "[m_id],[m_type],[amount],[active],[trans_id]) values "
                                        + "(@idnew" + NewNumIndex + "," + pkid + "," + emp_code + "," + FY + ",'" + FMNextMonth + "'," +
                                        "" + m_id + ",'" + c_m_type + "', " + gen_amount + ",1, @transidnew);";
                                    sbqry.Append(qry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_spl", "@idnew" + NewNumIndex, ""));
                                }

                            }
                        }
                    }
                }
            }
            try
            {
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                _logger.Info(emp_code + " " + "Processed Successfully....! ");
            }
            catch (Exception ex)
            {
                _logger.Error(emp_code + " " + ex);
                //_logger.Info(sbqry.ToString());
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                bRet = false;
            }

            return bRet;
        }

        #endregion

        public async Task<bool> ChangeMonthProcess()
        {
            bool bRet = false;

            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());
            int NewNumIndex = 0;
            int daysformonth = 0;
            int WH = 0;
            int PH = 0;
            string PD = null;
            string selectquery = "select format(fm,'yyyy-MM-dd') as fm,da_slabs,da_points,da_percent ,fy ,is_interest_calculated,interest_percent from pr_month_details where active=1";
            DataTable dt = await _sha.Get_Table_FromQry(selectquery);
            if (dt.Rows.Count > 0)
            {

                DateTime dtFm = Convert.ToDateTime(dt.Rows[0]["fm"].ToString());
                // DateTime dt1 = DateTime.Parse(dtFm.ToString("MM-dd-yyyy"));
                //day to 01
                dtFm = new DateTime(dtFm.Year, dtFm.Month, 1);
                //adding month
                dtFm = dtFm.AddMonths(1);
                string fm = dtFm.ToString("yyyy-MM-dd");
                //no. of days
                daysformonth = dtFm.AddMonths(1).AddDays(-1).Day;

                int fy = Helper.getFinancialYear(dtFm);


                string daslabs = dt.Rows[0]["da_slabs"].ToString();
                string dapoints = dt.Rows[0]["da_points"].ToString();
                string dapercent = dt.Rows[0]["da_percent"].ToString();

                string is_interest_calculated = dt.Rows[0]["is_interest_calculated"].ToString();
                string interest_percent = dt.Rows[0]["interest_percent"].ToString();

                string updQry = "update pr_month_details set active=0 where active=1";
                sbqry.Append(updQry);
                // int days = DateTime.DaysInMonth(year, month);

                //2. gen new num
                sbqry.Append(GetNewNumStringArr("pr_month_details", NewNumIndex));
                //query
                string inQry = "Insert into pr_month_details(id,fy,fm,week_holidays,paid_holidays,payment_date,da_slabs,da_points,da_percent,active,[trans_id],month_days,is_interest_calculated,interest_percent)" +
                   "values(@idnew" + NewNumIndex + ",'" + fy + "','" + fm + "'," + WH + "," + PH + ",null,'" + daslabs + "','" + dapoints + "','" + dapercent + "',1,@transidnew,'" + daysformonth + "','" + is_interest_calculated + "','" + interest_percent + "')";
                sbqry.Append(inQry);
                //4. transaction touch
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_details", "@idnew" + NewNumIndex, ""));

                try
                {
                    bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    _logger.Error(ex.StackTrace);
                    bRet = false;
                }


            }

            return bRet;
        }
        public async Task<bool> RevertPayslipService()
        {
            bool bRet = false;
            try
            {
                string qryfm = "select fm from pr_month_details where active=1";
                DataTable dtFm = await _sha.Get_Table_FromQry(qryfm);
                DateTime sfm = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                int Month = sfm.Month;
                int Year = sfm.Year;
                string qrypayslip = "";
                string qrypayslipallow = "";
                string qrypayslipded = "";

                qrypayslipded = "delete from pr_emp_payslip_allowance where payslip_mid in(select id from pr_emp_payslip where year(fm)=" + Year + " and month(fm)=" + Month + ");";
                qrypayslipallow = "delete from pr_emp_payslip_deductions where payslip_mid in(select id from pr_emp_payslip where year(fm)=" + Year + " and month(fm)=" + Month + ");";
                qrypayslip = "delete from pr_emp_payslip where year(fm)=" + Year + " and month(fm)=" + Month + ";";
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(qrypayslipded + qrypayslipallow + qrypayslip);
                return bRet;
            }
            catch (Exception ex)
            {
                return bRet;
            }

        }
        public async Task<bool> RevertOBShareService()
        {
            bool bRet = false;
            try
            {
                string qryfm = "select fm from pr_month_details where active=1";
                DataTable dtFm = await _sha.Get_Table_FromQry(qryfm);
                DateTime sfm = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                int Month = sfm.Month;
                int Year = sfm.Year;
                string qryobshar = "";
                string qryobsharadhoc = "";
                string qryobsharencash = "";

                qryobshar = "delete from pr_ob_share where fm='" + qryfm + "' and " +
                    " emp_code not in(select distinct emp_code from pr_emp_deput_det_field where fm='" + qryfm + "');";
                qryobsharadhoc = "delete from pr_ob_share_adhoc where fm='" + qryfm + "';";
                qryobsharencash = "delete from pr_ob_share_encashment where fm='" + qryfm + "';";
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(qryobshar + qryobsharadhoc + qryobsharencash);
                return bRet;
            }
            catch (Exception ex)
            {
                return bRet;
            }
        }
        public async Task<bool> UpdateMonthlyLoanInstallmentsReverse()
        {
            bool bRet = false;
            int loansno = 0;
            string qryfm = " select fm from pr_month_details where active=1";
            DataTable dtFm = await _sha.Get_Table_FromQry(qryfm);
            DateTime installmentDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
            int Month = installmentDate.Month;
            int Year = installmentDate.Year;
            //qry for all emps        
            string getAllEmployee = "select distinct el.emp_code,el.id from Employees e join pr_emp_adv_loans el on" +
                " e.EmpId = el.emp_code join pr_emp_payslip p on p.emp_code = el.emp_code " +
                "where  p.active = 1 AND month(p.fm)=" + Month + " and year(p.fm)=" + Year + "  order by emp_code ; ";
            //string getAllEmployee1 = "select distinct el.emp_code,el.id from Employees e " +
            //                       " join pr_emp_adv_loans el on e.EmpId = el.emp_code " +
            //                       " where el.active = 1";

            DataSet dtEmployees = await _sha.Get_MultiTables_FromQry(getAllEmployee + qryfm);
            DataTable dtEmployeesdata = dtEmployees.Tables[0];
            foreach (DataRow drEmp in dtEmployeesdata.Rows)
            {
                int Empcode = Convert.ToInt32(drEmp["emp_code"].ToString());
                int loans_id = Convert.ToInt32(drEmp["id"].ToString());
                //int Empcode = 6332;
                //int loans_id = 95741;
                string getdatafromparentchild = "  select l.id as loanId,l.emp_code,l.loan_type_mid,l.loan_sl_no,ch.id as childid from pr_emp_adv_loans l " +
                    "join pr_emp_adv_loans_child ch on l.id = ch.emp_adv_loans_mid where l.emp_code = " + Empcode + "  " +
                    "AND ch.emp_adv_loans_mid = " + loans_id + " ";
                DataTable dtloandata = await _sha.Get_Table_FromQry(getdatafromparentchild);
                if (dtloandata.Rows.Count > 0)
                {
                    int loan_type_mid = int.Parse(dtloandata.Rows[0]["loan_type_mid"].ToString());
                    int loan_sl_no = int.Parse(dtloandata.Rows[0]["loan_sl_no"].ToString());
                    int emp_adv_loans_child_mid1 = int.Parse(dtloandata.Rows[0]["childid"].ToString());
                    int emp_adv_loans_child_mid2 = 0;
                    if (dtloandata.Rows.Count > 1)
                    {
                        emp_adv_loans_child_mid2 = int.Parse(dtloandata.Rows[1]["childid"].ToString());
                    }


                    bRet = InstallmentAmountNewReverse(Empcode, loans_id, loan_type_mid, loan_sl_no, emp_adv_loans_child_mid1, emp_adv_loans_child_mid2, Month, Year).GetAwaiter().GetResult();

                }
                else
                {
                    string getLoans = "  select l.id as loanId,l.emp_code,l.loan_type_mid,l.loan_sl_no,ch.id as childid from pr_emp_adv_loans l " +
                  "join pr_emp_adv_loans_child ch on l.id = ch.emp_adv_loans_mid  join pr_emp_adv_loans_adjustments adj " +
                  "on adj.emp_adv_loans_child_mid = ch.id where l.emp_code = " + Empcode + " " +
                  " AND ch.emp_adv_loans_mid =" + loans_id + " " +
                  "AND month(adj.fm)=" + Month + " and year(adj.fm)=" + Year + " and payment_type not in('Full Clearing','Part Payment') order by ch.interest_rate desc";

                    DataTable dtLoans = await _sha.Get_Table_FromQry(getLoans);

                    if (dtLoans.Rows.Count > 0)
                    {
                        int loan_type_mid = int.Parse(dtLoans.Rows[0]["loan_type_mid"].ToString());
                        int loan_sl_no = int.Parse(dtLoans.Rows[0]["loan_sl_no"].ToString());
                        int emp_adv_loans_child_mid1 = int.Parse(dtLoans.Rows[0]["childid"].ToString());
                        int emp_adv_loans_child_mid2 = 0;
                        if (dtLoans.Rows.Count > 1)
                        {
                            emp_adv_loans_child_mid2 = int.Parse(dtLoans.Rows[1]["childid"].ToString());
                        }


                        bRet = InstallmentAmountNewReverse(Empcode, loans_id, loan_type_mid, loan_sl_no, emp_adv_loans_child_mid1, emp_adv_loans_child_mid2, Month, Year).GetAwaiter().GetResult();


                    }
                }
               
            }
            return bRet;

        }
        public async Task<bool> UpdateMonthlyLoanInstallments()
        {
            bool bRet = false;
            int loansno = 0;
            string qryfm = " select fm from pr_month_details where active=1";
            DataTable dtFm = await _sha.Get_Table_FromQry(qryfm);
            DateTime installmentDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
            int Month = installmentDate.Month;
            int Year = installmentDate.Year;
            //qry for all emps        
            string getAllEmployee = "select distinct el.emp_code,el.id from Employees e join pr_emp_adv_loans el on" +
                " e.EmpId = el.emp_code join pr_emp_payslip p on p.emp_code = el.emp_code " +
                "where el.active = 1 and p.active = 1  and p.active = 1 AND month(p.fm)=" + Month + " and year(p.fm)=" + Year + " order by emp_code ; ";
            //string getAllEmployee1 = "select distinct el.emp_code,el.id from Employees e " +
            //                       " join pr_emp_adv_loans el on e.EmpId = el.emp_code " +
            //                       " where el.active = 1";
    

            DataSet dtEmployees = await _sha.Get_MultiTables_FromQry(getAllEmployee + qryfm);

            DataTable dtEmployeesdata = dtEmployees.Tables[0];


            foreach (DataRow drEmp in dtEmployeesdata.Rows)
            {
                int Empcode = Convert.ToInt32(drEmp["emp_code"].ToString());
                int loans_id = Convert.ToInt32(drEmp["id"].ToString());

                string getLoans = "select mas.loan_id,mas.id as mastertypeid,chi.priority,chi.Id as childloanid," +
                    "chi.emp_adv_loans_mid,adv.method ,chi.principal_amount_recovered,chi.total_amount_recovered," +
                    "chi.loan_amount,chi.os_interest_amount,chi.principal_recovered_flag,chi.interest_recovered_flag," +
                    "chi.loan_sl_no,chi.interest_rate,chi.total_interest_installments,adv.installment_amount from pr_emp_adv_loans adv" +
                    " join pr_loan_master mas on adv.loan_type_mid = mas.id " +
                    " join pr_emp_adv_loans_child chi on adv.id = chi.emp_adv_loans_mid " +
                    " where adv.active = 1 AND adv.emp_code=" + Empcode + "  AND chi.emp_adv_loans_mid =" + loans_id + " " +
                    " order by chi.interest_rate desc";


                DataTable dtLoans = await _sha.Get_Table_FromQry(getLoans);

                if (dtLoans.Rows.Count > 0)
                {

                    string loanType = dtLoans.Rows[0]["loan_id"].ToString().ToUpper();
                    int loanid = int.Parse(dtLoans.Rows[0]["mastertypeid"].ToString());

                    int emp_adv_loans_child_mid = int.Parse(dtLoans.Rows[0]["childloanid"].ToString());

                    // 2nd subloan

                    // int emp_adv_loans_child_mid1 = int.Parse(dtLoans.Rows[1]["childloanid"].ToString());

                    int emp_adv_loans_child_mid1 = 0;
                    if (dtLoans.Rows.Count > 1)
                    {

                        emp_adv_loans_child_mid1 = int.Parse(dtLoans.Rows[1]["childloanid"].ToString());

                    }

                    int emp_adv_loans_mid = int.Parse(dtLoans.Rows[0]["emp_adv_loans_mid"].ToString());
                    string method = dtLoans.Rows[0]["method"].ToString().ToUpper();
                    double os_interest = Convert.ToDouble(dtLoans.Rows[0]["os_interest_amount"].ToString());

                    //Fiance Month
                    loansno = int.Parse(dtLoans.Rows[0]["loan_sl_no"].ToString());

                    //1. loan type FEST
                    if (loanType == PrConstants.FESTIVAL_LOAN_CODE || loanType == PrConstants.PF_LOAN1_CODE || loanType == PrConstants.PF_LOAN2_CODE || loanType == PrConstants.PF_LOANST1_CODE || loanType == PrConstants.PF_LOANST2_CODE || loanType == PrConstants.PF_LOANLT1_CODE || loanType == PrConstants.PF_LOANLT2_CODE || loanType == PrConstants.PF_LOANLT3_CODE || loanType == PrConstants.PF_LOANLT4_CODE)
                    {
                        //2.interest with Equal Installments
                        if (method == "INTEREST WITH EQUAL INSTALLMENTS")
                        {

                            //bRet = UpadateinstallmentsWithEqualinterest(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, loanid, Month, Year).GetAwaiter().GetResult();
                            bRet = InstallmentAmountNew(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();

                        }
                        else
                            //bRet = UpdateInstallmentAmount(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest,loansno).GetAwaiter().GetResult();
                            //By Uma
                            bRet = InstallmentAmountNew(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                    }

                    //2. loan Type not in PF1,PF2,FEST
                    else if (loanType != PrConstants.PF_LOAN1_CODE && loanType != PrConstants.PF_LOAN2_CODE && loanType != PrConstants.FESTIVAL_LOAN_CODE && loanType != PrConstants.PF_LOANST1_CODE && loanType != PrConstants.PF_LOANST2_CODE && loanType != PrConstants.PF_LOANLT1_CODE && loanType != PrConstants.PF_LOANLT2_CODE && loanType != PrConstants.PF_LOANLT3_CODE && loanType != PrConstants.PF_LOANLT4_CODE)
                    {
                        //3.Loans Type For Sub Loans
                        if (dtLoans.Rows.Count == 2)
                        {
                            //frist SubLoans
                            int total_amount_recovered = int.Parse(dtLoans.Rows[0]["total_amount_recovered"].ToString());
                            int loan_amount = int.Parse(dtLoans.Rows[0]["loan_amount"].ToString());
                            loanType = dtLoans.Rows[0]["loan_id"].ToString().ToUpper();
                            int priority = int.Parse(dtLoans.Rows[0]["priority"].ToString());
                            bool principal_recovered_flag = bool.Parse(dtLoans.Rows[0]["principal_recovered_flag"].ToString());
                            bool interest_recovered_flag = bool.Parse(dtLoans.Rows[0]["interest_recovered_flag"].ToString());
                            os_interest = Convert.ToDouble(dtLoans.Rows[0]["os_interest_amount"].ToString());
                            emp_adv_loans_mid = int.Parse(dtLoans.Rows[0]["emp_adv_loans_mid"].ToString());

                            int total_amount_recovered2 = int.Parse(dtLoans.Rows[1]["total_amount_recovered"].ToString());
                            int loan_amount2 = int.Parse(dtLoans.Rows[1]["loan_amount"].ToString());
                            int priority2 = int.Parse(dtLoans.Rows[1]["priority"].ToString());
                            bool principal_recovered_flag2 = bool.Parse(dtLoans.Rows[1]["principal_recovered_flag"].ToString());
                            bool interest_recovered_flag2 = bool.Parse(dtLoans.Rows[1]["interest_recovered_flag"].ToString());
                            int os_interest2 = int.Parse(dtLoans.Rows[1]["os_interest_amount"].ToString());

                            int installment_amount = int.Parse(dtLoans.Rows[0]["installment_amount"].ToString());
                            int principal_amount_recovered1 = int.Parse(dtLoans.Rows[0]["principal_amount_recovered"].ToString());
                            int principal_amount_recovered2 = int.Parse(dtLoans.Rows[1]["principal_amount_recovered"].ToString());
                            //To Recover Frist Subloan Principal Amount 
                            if (loan_amount > total_amount_recovered && priority == 1 && principal_recovered_flag == false)
                            {

                                emp_adv_loans_child_mid = int.Parse(dtLoans.Rows[0]["childloanid"].ToString());
                                emp_adv_loans_child_mid1 = int.Parse(dtLoans.Rows[1]["childloanid"].ToString());

                                //bRet = UpdateInstallmentAmount(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest,loansno).GetAwaiter().GetResult();
                                //By Uma
                                bRet = InstallmentAmountNew(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                            }
                            // To Recover Second Subloan Principal Amount
                            else if (loan_amount2 != total_amount_recovered2 && priority2 == 2 && (principal_recovered_flag2 == false))
                            {

                                emp_adv_loans_child_mid = int.Parse(dtLoans.Rows[1]["childloanid"].ToString());

                                // bRet = UpdateInstallmentAmount(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                                //By Uma
                                bRet = InstallmentAmountNew(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                            }
                            //To Recover Frist Subloan Interest Amount
                            else if ((loan_amount == principal_amount_recovered1 && priority == 1) && (loan_amount2 == principal_amount_recovered2 && priority2 == 2) && (interest_recovered_flag == false))
                            {

                                emp_adv_loans_child_mid = int.Parse(dtLoans.Rows[0]["childloanid"].ToString());

                                //bRet = UpdateInstallmentAmount(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                                //By Uma
                                bRet = InstallmentAmountNew(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                            }
                            //To Recover Second Subloan Interest Amount
                            else if ((loan_amount == principal_amount_recovered1 && priority == 1) && (loan_amount2 == principal_amount_recovered2 && priority2 == 2) && (interest_recovered_flag == true))
                            {
                                emp_adv_loans_child_mid = int.Parse(dtLoans.Rows[1]["childloanid"].ToString());

                                //  bRet = UpdateInstallmentAmount(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                                //By Uma
                                bRet = InstallmentAmountNew(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                            }

                        }
                        else
                            //bRet = UpdateInstallmentAmount(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                            //By Uma
                            bRet = InstallmentAmountNew(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                    }
                }
            }
            return bRet;

        }

        public async Task<bool> UpdateMonthlyLoanInstallmentsBeforeMonthend()
        {
            bool bRet = false;
            int loansno = 0;
            string getAllEmployee = "select distinct el.emp_code,el.id from Employees e join pr_emp_adv_loans el on" +
                " e.EmpId = el.emp_code join pr_emp_payslip p on p.emp_code = el.emp_code " +
                "where el.active = 1 and p.active = 1 order by emp_code ; ";
            string qryfm = " select fm from pr_month_details where active=1";
            DataSet dtEmployees = await _sha.Get_MultiTables_FromQry(getAllEmployee + qryfm);
            DataTable dtEmployeesdata = dtEmployees.Tables[0];
            DataTable dtFm = dtEmployees.Tables[1];
            foreach (DataRow drEmp in dtEmployeesdata.Rows)
            {
                
                int Empcode = Convert.ToInt32(drEmp["emp_code"].ToString());
                int loans_id = Convert.ToInt32(drEmp["id"].ToString());
                string getLoans = "select mas.loan_id,mas.id as mastertypeid,chi.priority,chi.Id as childloanid," +
                    "chi.emp_adv_loans_mid,adv.method ,chi.principal_amount_recovered,chi.total_amount_recovered," +
                    "chi.loan_amount,chi.os_interest_amount,chi.principal_recovered_flag,chi.interest_recovered_flag," +
                    "chi.loan_sl_no,chi.interest_rate,chi.total_interest_installments,adv.installment_amount from pr_emp_adv_loans adv" +
                    " join pr_loan_master mas on adv.loan_type_mid = mas.id " +
                    " join pr_emp_adv_loans_child chi on adv.id = chi.emp_adv_loans_mid " +
                    " where adv.active = 1 AND adv.emp_code=" + Empcode + "  AND chi.emp_adv_loans_mid =" + loans_id + " " +
                    " order by chi.interest_rate desc";

                DataTable dtLoans = await _sha.Get_Table_FromQry(getLoans);
                if (dtLoans.Rows.Count > 0)
                {
                    string loanType = dtLoans.Rows[0]["loan_id"].ToString().ToUpper();
                    int loanid = int.Parse(dtLoans.Rows[0]["mastertypeid"].ToString());
                    int emp_adv_loans_child_mid = int.Parse(dtLoans.Rows[0]["childloanid"].ToString());
                    int emp_adv_loans_child_mid1 = 0;
                    if (dtLoans.Rows.Count > 1)
                    {
                        emp_adv_loans_child_mid1 = int.Parse(dtLoans.Rows[1]["childloanid"].ToString());
                    }
                    int emp_adv_loans_mid = int.Parse(dtLoans.Rows[0]["emp_adv_loans_mid"].ToString());
                    string method = dtLoans.Rows[0]["method"].ToString().ToUpper();
                    double os_interest = Convert.ToDouble(dtLoans.Rows[0]["os_interest_amount"].ToString());
                    //Fiance Month
                    DateTime installmentDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                    int Month = installmentDate.Month;
                    int Year = installmentDate.Year;
                    loansno = int.Parse(dtLoans.Rows[0]["loan_sl_no"].ToString());
                    //1. loan type FEST
                    if (loanType == PrConstants.FESTIVAL_LOAN_CODE || loanType == PrConstants.PF_LOAN1_CODE || loanType == PrConstants.PF_LOAN2_CODE || loanType == PrConstants.PF_LOANST1_CODE || loanType == PrConstants.PF_LOANST2_CODE || loanType == PrConstants.PF_LOANLT1_CODE || loanType == PrConstants.PF_LOANLT2_CODE || loanType == PrConstants.PF_LOANLT3_CODE || loanType == PrConstants.PF_LOANLT4_CODE)
                    {
                        //2.interest with Equal Installments
                        if (method == "INTEREST WITH EQUAL INSTALLMENTS")
                        {
                            bRet = InstallmentAmountNewBeforeMonthEnd(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                        }
                        else
                        {
                            bRet = InstallmentAmountNewBeforeMonthEnd(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                        }
                    }

                    //2. loan Type not in PF1,PF2,FEST
                    else if (loanType != PrConstants.PF_LOAN1_CODE && loanType != PrConstants.PF_LOAN2_CODE && loanType != PrConstants.FESTIVAL_LOAN_CODE && loanType != PrConstants.PF_LOANST1_CODE && loanType != PrConstants.PF_LOANST2_CODE && loanType != PrConstants.PF_LOANLT1_CODE && loanType != PrConstants.PF_LOANLT2_CODE && loanType != PrConstants.PF_LOANLT3_CODE && loanType != PrConstants.PF_LOANLT4_CODE)
                    {
                        //3.Loans Type For Sub Loans
                        if (dtLoans.Rows.Count == 2)
                        {
                            //frist SubLoans
                            int total_amount_recovered = int.Parse(dtLoans.Rows[0]["total_amount_recovered"].ToString());
                            int loan_amount = int.Parse(dtLoans.Rows[0]["loan_amount"].ToString());
                            loanType = dtLoans.Rows[0]["loan_id"].ToString().ToUpper();
                            int priority = int.Parse(dtLoans.Rows[0]["priority"].ToString());
                            bool principal_recovered_flag = bool.Parse(dtLoans.Rows[0]["principal_recovered_flag"].ToString());
                            bool interest_recovered_flag = bool.Parse(dtLoans.Rows[0]["interest_recovered_flag"].ToString());
                            os_interest = Convert.ToDouble(dtLoans.Rows[0]["os_interest_amount"].ToString());
                            emp_adv_loans_mid = int.Parse(dtLoans.Rows[0]["emp_adv_loans_mid"].ToString());

                            int total_amount_recovered2 = int.Parse(dtLoans.Rows[1]["total_amount_recovered"].ToString());
                            int loan_amount2 = int.Parse(dtLoans.Rows[1]["loan_amount"].ToString());
                            int priority2 = int.Parse(dtLoans.Rows[1]["priority"].ToString());
                            bool principal_recovered_flag2 = bool.Parse(dtLoans.Rows[1]["principal_recovered_flag"].ToString());
                            bool interest_recovered_flag2 = bool.Parse(dtLoans.Rows[1]["interest_recovered_flag"].ToString());
                            int os_interest2 = int.Parse(dtLoans.Rows[1]["os_interest_amount"].ToString());

                            int installment_amount = int.Parse(dtLoans.Rows[0]["installment_amount"].ToString());
                            int principal_amount_recovered1 = int.Parse(dtLoans.Rows[0]["principal_amount_recovered"].ToString());
                            int principal_amount_recovered2 = int.Parse(dtLoans.Rows[1]["principal_amount_recovered"].ToString());
                            //To Recover Frist Subloan Principal Amount 
                            if (loan_amount > total_amount_recovered && priority == 1 && principal_recovered_flag == false)
                            {

                                emp_adv_loans_child_mid = int.Parse(dtLoans.Rows[0]["childloanid"].ToString());
                                emp_adv_loans_child_mid1 = int.Parse(dtLoans.Rows[1]["childloanid"].ToString());
                                bRet = InstallmentAmountNewBeforeMonthEnd(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                            }
                            // To Recover Second Subloan Principal Amount
                            else if (loan_amount2 != total_amount_recovered2 && priority2 == 2 && (principal_recovered_flag2 == false))
                            {
                                emp_adv_loans_child_mid = int.Parse(dtLoans.Rows[1]["childloanid"].ToString());
                                bRet = InstallmentAmountNewBeforeMonthEnd(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                            }
                            //To Recover Frist Subloan Interest Amount
                            else if ((loan_amount == principal_amount_recovered1 && priority == 1) && (loan_amount2 == principal_amount_recovered2 && priority2 == 2) && (interest_recovered_flag == false))
                            {

                                emp_adv_loans_child_mid = int.Parse(dtLoans.Rows[0]["childloanid"].ToString());
                                bRet = InstallmentAmountNewBeforeMonthEnd(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                            }
                            //To Recover Second Subloan Interest Amount
                            else if ((loan_amount == principal_amount_recovered1 && priority == 1) && (loan_amount2 == principal_amount_recovered2 && priority2 == 2) && (interest_recovered_flag == true))
                            {
                                emp_adv_loans_child_mid = int.Parse(dtLoans.Rows[1]["childloanid"].ToString());
                                bRet = InstallmentAmountNewBeforeMonthEnd(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                            }

                        }
                        else
                            //bRet = UpdateInstallmentAmount(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                            //By Uma
                            bRet = InstallmentAmountNewBeforeMonthEnd(Empcode, emp_adv_loans_mid, emp_adv_loans_child_mid, emp_adv_loans_child_mid1, loanid, loanType, Month, Year, os_interest, loansno).GetAwaiter().GetResult();
                    }
                }
            }
            return bRet;

        }

        //1. For Fest Loans no interest and sub loans principal Amount
        private async Task<bool> UpdateInstallmentAmount(int EmpCode, int emp_adv_loans_mid, int emp_adv_loans_child_mid, int emp_adv_loans_child_mid1, int loanid, string loanType, int month, int year, int os_interest_amount_first, int loansno)
        {
            StringBuilder sbqry = new StringBuilder();

            sbqry.Append(GenNewTransactionString());

            bool bRet = false;

            string qry = "";
            try
            {

                string getLoans = "select chi.loan_amount,chi.os_principal_amount,chi.principal_amount_recovered,chi.total_amount_recovered, chi.os_total_amount," +
                    "chi.principal_recovered_flag,chi.interest_recovered_flag ,chi.os_interest_amount,chi.interest_amount_recovered,adv.installment_amount," +
                    "adv.completed_installment,adv.remaining_installment,adv.total_installment,adv.installment_start_date,chi.loan_sl_no," +
                    "chi.total_interest_installments,chi.interest_accured,chi.os_interest_amount,adv.interest_installment_amount,chi.priority,adv.interest_installment " +
                    "from pr_emp_adv_loans adv" +
                   " join pr_emp_adv_loans_child chi on adv.id =" + emp_adv_loans_mid +
                   " where adv.active = 1 AND adv.emp_code =" + EmpCode + " AND chi.id =" + emp_adv_loans_child_mid; //+ " AND adv.installment_start_date <= '" + installment_Date + "'";

                string qrygetinstallment_amount = "select dd_amount from pr_emp_payslip psl " +
                                                 " join pr_emp_payslip_deductions ded on psl.id = ded.payslip_mid " +
                                                 " where psl.emp_code=" + EmpCode + " AND ded.dd_type='loan'" + " AND ded.dd_mid=" + emp_adv_loans_mid + " AND month(psl.fm)=" + month + " and ded.dd_amount>0 AND ded.active=1 and year(psl.fm)=" + year;

                string qryfm = "select fm,fy from pr_month_details where active=1";

                //2nd subloan
                string getLoans1 = "select  chi.slno,chi.loan_amount,chi.os_principal_amount,chi.principal_amount_recovered,chi.interest_accured,chi.total_amount_recovered, chi.os_total_amount,chi.principal_recovered_flag,chi.os_interest_amount,chi.interest_amount_recovered,adv.installment_amount,adv.completed_installment,adv.remaining_installment,adv.total_installment,adv.installment_start_date,chi.loan_sl_no from pr_emp_adv_loans adv" +
                               " join pr_emp_adv_loans_child chi on adv.id =" + emp_adv_loans_mid +
                               " where adv.active = 1 AND adv.emp_code =" + EmpCode + " AND chi.id =" + emp_adv_loans_child_mid1; //+ " AND adv.installment_start_date <= '" + installment_Date + "'";

                string getTwoSubLoans = "select chi.id,chi.loan_amount,chi.os_principal_amount,chi.principal_amount_recovered,chi.total_amount_recovered, chi.os_total_amount," +
                         "chi.principal_recovered_flag,chi.interest_recovered_flag ,chi.os_interest_amount,chi.interest_amount_recovered,adv.installment_amount," +
                         "adv.completed_installment,adv.remaining_installment,adv.total_installment," +
                         "adv.installment_start_date,chi.loan_sl_no,chi.total_interest_installments,chi.interest_accured,adv.interest_installment, " +
                         "chi.os_interest_amount,adv.interest_installment_amount,adv.installment_amount,principal_amount_recovered " +
                         "from pr_emp_adv_loans adv" +
                        " join pr_emp_adv_loans_child chi on adv.id =chi.emp_adv_loans_mid " +
                        " where adv.active = 1 AND adv.emp_code =" + EmpCode + " and loan_type_mid = (select id from pr_loan_master where loan_id = '" + loanType + "');";

                DataSet dsfestlonas = await _sha.Get_MultiTables_FromQry(getLoans + qrygetinstallment_amount + qryfm + getLoans1 + getTwoSubLoans);


                DataTable dtloansdata = dsfestlonas.Tables[0];
                DataTable dtddamount = dsfestlonas.Tables[1];
                DataTable dtfince = dsfestlonas.Tables[2];
                DataTable dt = dsfestlonas.Tables[3];
                DataTable dtPriority2 = dsfestlonas.Tables[4];

                int completed_installments = 0;
                int no_of_installments_cover = 0;
                int installment_amountcut = int.Parse(dtPriority2.Rows[0]["installment_amount"].ToString());
                int priority = int.Parse(dtloansdata.Rows[0]["priority"].ToString());

                if (dtloansdata.Rows.Count > 0 && dtddamount.Rows.Count > 0)
                {

                    //installments calculation
                    float intrestinsalamt = float.Parse(dtloansdata.Rows[0]["interest_installment_amount"].ToString());
                    int installmentamount = int.Parse(dtddamount.Rows[0]["dd_amount"].ToString());//2
                    if (installmentamount < intrestinsalamt)
                    {
                        completed_installments = 0;
                        no_of_installments_cover = 0;
                    }
                    else
                    {
                        no_of_installments_cover = Convert.ToInt32(Math.Round(Convert.ToDecimal(installmentamount / installmentamount)).ToString());
                    }
                    completed_installments = int.Parse(dtloansdata.Rows[0]["completed_installment"].ToString());

                    int remaining_installments = int.Parse(dtloansdata.Rows[0]["remaining_installment"].ToString());

                    int total_installments = int.Parse(dtloansdata.Rows[0]["total_installment"].ToString());
                    completed_installments = completed_installments + no_of_installments_cover;
                    remaining_installments = total_installments - completed_installments;

                    int os_principal_amount = int.Parse(dtloansdata.Rows[0]["os_principal_amount"].ToString());
                    int os_interest_amounts = int.Parse(dtloansdata.Rows[0]["os_interest_amount"].ToString());

                    int Principal_balance_amt1 = 0;
                    int second_os_princ = 0;
                    int adj_os_principal_amount1 = int.Parse(dtPriority2.Rows[0]["os_principal_amount"].ToString());
                    int adj_os_interest_amounts1 = int.Parse(dtPriority2.Rows[0]["os_interest_amount"].ToString());
                    int adj_principal_amt_recovered1 = int.Parse(dtPriority2.Rows[0]["principal_amount_recovered"].ToString());
                    int adj_install_amt1 = adj_os_principal_amount1;
                    int intrst_installments = int.Parse(dtPriority2.Rows[0]["interest_installment"].ToString());

                    int adj_os_principal_amount2 = 0;
                    int adj_os_interest_amounts2 = 0;
                    int adj_principal_amt_recovered2 = 0;
                    int adj_install_amt2 = 0;
                    int Principal_balance_amt2 = 0;

                    if (dtPriority2.Rows.Count > 1)
                    {
                        adj_os_principal_amount2 = int.Parse(dtPriority2.Rows[1]["os_principal_amount"].ToString());
                        adj_os_interest_amounts2 = int.Parse(dtPriority2.Rows[1]["os_interest_amount"].ToString());
                        adj_principal_amt_recovered2 = int.Parse(dtPriority2.Rows[1]["principal_amount_recovered"].ToString());
                        adj_install_amt2 = adj_os_principal_amount2;
                    }


                    int total_amount_recovered = 0;
                    int total_amount_recovered2 = 0;
                    int loan_adj_rem = 0;
                    //when loan adjust
                    StringBuilder adjquery = new StringBuilder();

                    DateTime paidDate = Convert.ToDateTime(dtfince.Rows[0]["fm"].ToString());
                    string installments_paid_date = (paidDate.ToString("yyyy-MM-dd"));
                    string Cash_paid_on = installments_paid_date;
                    int fy = Convert.ToInt32(dtfince.Rows[0]["fy"].ToString());
                    string fm = installments_paid_date;
                    int Month = paidDate.Month;
                    int Year = paidDate.Year;
                    string payment_type = "installment";
                    string payment_mode = "installment";
                    int principal_adj_open_amount = 0;
                    int principal_adj_balance_amount = 0;
                    int int_open_amt1 = 0;
                    int int_open_amt2 = 0;
                    //priority 1
                    if (os_principal_amount < installmentamount && os_principal_amount > 0 && priority == 1 && dtPriority2.Rows.Count > 1)
                    {

                        //priority 1 principal
                        int outid1 = int.Parse(dtPriority2.Rows[0]["id"].ToString());
                        int outid2 = int.Parse(dtPriority2.Rows[1]["id"].ToString());
                        //exclude out standing principal and update ost principal  = 0 and princ flag =1
                        adj_principal_amt_recovered1 = adj_principal_amt_recovered1 + adj_os_principal_amount1;
                        total_amount_recovered = adj_principal_amt_recovered1;
                        loan_adj_rem = installmentamount - os_principal_amount;
                        int oustanding_totalamt = (adj_os_principal_amount1 + adj_os_interest_amounts1) - os_principal_amount;
                        Principal_balance_amt1 = adj_principal_amt_recovered1 - total_amount_recovered;

                        string upqueryclosefstloanprin = "update pr_emp_adv_loans_child set os_principal_amount=0, os_total_amount=" + oustanding_totalamt + ", " +
                            "principal_recovered_flag=1,principal_amount_recovered=" + adj_principal_amt_recovered1 + "," +
                            "total_amount_recovered=" + total_amount_recovered + " where id=" + outid1 + " AND active=1 AND principal_amount_recovered>0;";
                        adjquery.Append(upqueryclosefstloanprin);

                        //priorty1 principal loan ledeger
                        sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                        qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid]," +
                            "[principal_open_amount],[principal_paid_amount],[principal_balance_amount]," +
                            "[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                            "[payment_type],[payment_mode],[cash_paid_on],[covered_installments]," +
                            "[installments_paid_date],[amount_paid],[installments_paid]," +
                            "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                           + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," +
                           "" + os_principal_amount + "," + os_principal_amount + "," +
                           "" + 0 + "," + adj_os_interest_amounts1 + "," +
                           "" + 0 + "," + adj_os_interest_amounts1 + ",'" + payment_type + "','" + payment_mode + "', " +
                           "'" + installments_paid_date + "'," + no_of_installments_cover + "," +
                           "'" + installments_paid_date + "'," + adj_install_amt1 + "," +
                           "" + completed_installments + "," + installmentamount + "," +
                           "1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";

                        //string qry11 = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid]," +
                        //    "[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount]," +
                        //    "[principal_balance_amount],[interest_open_amount],[interest_paid_amount]," +
                        //    "[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on]," +
                        //    "[covered_installments],[installments_paid_date],[amount_paid]," +
                        //    "[installments_paid],[installments_amount],[active],[trans_id]," +
                        //    "[fm],[fy],loan_sl_no) values "
                        //   + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid1 + "," +
                        //   "" + second_os_princ + "," + adj_install_amt2 + "," +
                        //   "" + Principal_balance_amt2 + "," + int_open_amt2 + "," + 0 + "," +
                        //   "" + 0 + ",'" + payment_type + "','" + payment_mode + "'," +
                        //   " '" + installments_paid_date + "'," + no_of_installments_cover + "," +
                        //   "'" + installments_paid_date + "'," + adj_install_amt2 + "," +
                        //   "" + completed_installments + "," + installmentamount + "," +
                        //   "1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";
                        sbqry.Append(qry);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));


                        //priority 2 principal
                        if (dtPriority2.Rows.Count >= 2)
                        {
                            second_os_princ = int.Parse(dtPriority2.Rows[1]["os_principal_amount"].ToString());
                            int loan_amt = int.Parse(dtPriority2.Rows[1]["loan_amount"].ToString());
                            if (second_os_princ > loan_adj_rem)
                            {
                                second_os_princ = second_os_princ - loan_adj_rem;
                                int recovered_amt = loan_amt - second_os_princ;
                                Principal_balance_amt2 = second_os_princ;
                                adj_install_amt2 = recovered_amt;
                                loan_adj_rem = 0;
                                int_open_amt2 = adj_os_interest_amounts2;
                                //priority 2 principal amount exclude 
                                string upquerycloseScndloanprin = "update pr_emp_adv_loans_child set os_principal_amount=" + second_os_princ + "," +
                                    " principal_amount_recovered=" + recovered_amt + ", total_amount_recovered=" + recovered_amt + ",os_total_amount=" + second_os_princ + "" +
                                    " where  id=" + outid2 + " AND active=1; ";
                                adjquery.Append(upquerycloseScndloanprin);

                                //loan adj

                                principal_adj_open_amount = Principal_balance_amt1;
                                principal_adj_balance_amount = Principal_balance_amt1;
                                int_open_amt1 = adj_os_interest_amounts1;
                                //1.qry-- - insert into pr_emp_adv_loans_adjustments
                                //sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                                qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid]," +
                                    "[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount]," +
                                    "[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on]," +
                                    "[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                                    "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                                   + "(@idnew+1," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_adj_open_amount + "," +
                                   "" + 0 + "," + principal_adj_balance_amount + "," + adj_os_interest_amounts1 + "," + 0 + "," +
                                   "" + adj_os_interest_amounts1 + ",'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                                   "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + adj_install_amt1 + "," +
                                   "" + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," +
                                   "" + loansno + ");";

                                string qry1 = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid]," +
                                    "[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount]," +
                                    "[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on]," +
                                    "[covered_installments],[installments_paid_date],[amount_paid],[installments_paid],[installments_amount]," +
                                    "[active],[trans_id],[fm],[fy],loan_sl_no) values "
                                   + "(@idnew+1," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid1 + "," + loan_amt + "," +
                                   "" + adj_install_amt2 + "," + second_os_princ + "," + adj_os_interest_amounts2 + "," + 0 + "," + adj_os_interest_amounts2 + "," +
                                   "'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                                   "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + adj_install_amt2 + "," +
                                   "" + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";
                                sbqry.Append(qry + qry1);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
                            }

                            else
                            {
                                loan_adj_rem = loan_adj_rem - second_os_princ;
                                //priority 2 principal amount exclude

                                string upquerycloseScndloanintpr = "update pr_emp_adv_loans_child set os_principal_amount=0,principal_recovered_flag=1 where  id=" + outid2 + " AND active=1; ";
                                adjquery.Append(upquerycloseScndloanintpr);

                                //loan adjustment for priority 2 principal
                                sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));

                                qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid]," +
                                   "[principal_open_amount],[principal_paid_amount],[principal_balance_amount]," +
                                   "[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                                   "[payment_type],[payment_mode],[cash_paid_on],[covered_installments]," +
                                   "[installments_paid_date],[amount_paid],[installments_paid]," +
                                   "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                                  + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid1 + "," +
                                  "" + 0 + "," + second_os_princ + "," + 0 + "," + int_open_amt2 + "," +
                                  "" + 0 + "," + 0 + ",'" + payment_type + "','" + payment_mode + "'," +
                                  " '" + installments_paid_date + "'," + no_of_installments_cover + "," +
                                  "'" + installments_paid_date + "'," + adj_install_amt2 + "," + completed_installments + "," +
                                  "" + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";

                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
                            }


                        }



                        //priority 1 intrst

                        if (loan_adj_rem > 0 && loan_adj_rem > os_interest_amounts)
                        {
                            loan_adj_rem = loan_adj_rem - os_interest_amounts;
                            //update ost interest  = 0 and  intrst flag =1
                            string upqueryclosefstloanint = "update pr_emp_adv_loans_child set os_interest_amount=0,interest_recovered_flag=1," +
                                "active=0 where  id=" + outid1 + " AND active=1; ";
                            adjquery.Append(upqueryclosefstloanint);
                            //loan ledger for priority 1 intrst os_interest_amounts

                            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                            qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                                "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount]," +
                                "[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments]," +
                                "[installments_paid_date],[amount_paid],[installments_paid],[installments_amount]," +
                                "[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + 0 + "," +
                               "" + adj_principal_amt_recovered1 + "," + 0 + "," + os_interest_amounts + "," + 0 + "," + 0 + "," +
                               "'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                               "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + os_interest_amounts + "," +
                               "" + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
                        }
                        else if (loan_adj_rem > 0 && loan_adj_rem < os_interest_amounts)
                        {
                            int os_inst = os_interest_amounts - loan_adj_rem;
                            int intrestamtrecovered = os_interest_amounts - os_inst;
                            loan_adj_rem = 0;
                            // exclude interest amount
                            string upqueryclosefstloanint = "update pr_emp_adv_loans_child set " +
                            " principal_amount_recovered=" + adj_principal_amt_recovered1 + "," +
                            " total_amount_recovered=" + total_amount_recovered + "," +
                            "interest_amount_recovered=" + intrestamtrecovered + ",os_interest_amount=" + os_inst + "" +
                            " where  id=" + outid1 + " AND active=1  AND principal_amount_recovered>0; ";
                            adjquery.Append(upqueryclosefstloanint);

                            //loan ledger for priority 1 intrst os_interest_amounts 
                            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                            qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                                "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount]," +
                                "[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments]," +
                                "[installments_paid_date],[amount_paid],[installments_paid],[installments_amount]," +
                                "[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + 0 + "," +
                               "" + adj_principal_amt_recovered1 + "," + 0 + "," + os_inst + "," + intrestamtrecovered + "," + os_inst + "," +
                               "'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                               "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + os_interest_amounts + "," +
                               "" + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";

                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
                        }
                        //priority 2 intrst

                        if (dtPriority2.Rows.Count >= 2 && loan_adj_rem > 0)
                        {


                            //priotity 2 out intrst 
                            int second_os_inc = int.Parse(dtPriority2.Rows[1]["os_interest_amount"].ToString());
                            if (second_os_inc > loan_adj_rem)
                            {
                                int sec_os = second_os_inc;
                                second_os_inc = second_os_inc - loan_adj_rem;
                                //priority 2 principal amount exclude 
                                string upquerycloseScndloanprin = "update pr_emp_adv_loans_child set os_interest_amount=" + second_os_inc + ",interest_amount_recovered=" + loan_adj_rem + " " +
                                    "where  id=" + outid2 + " AND active=1; ";
                                adjquery.Append(upquerycloseScndloanprin);

                                //loan ledger for priority 2 intrst loan_adj_rem
                                sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                                string qry1 = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid]," +
                                    "[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount]," +
                                    "[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on]," +
                                    "[covered_installments],[installments_paid_date],[amount_paid],[installments_paid],[installments_amount]," +
                                    "[active],[trans_id],[fm],[fy],loan_sl_no) values "
                                       + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid1 + "," + 0 + "," +
                                       "" + 0 + "," + 0 + "," + sec_os + "," + loan_adj_rem + "," + 0 + "," +
                                       "'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                                       "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + adj_install_amt2 + "," +
                                       "" + completed_installments + "," + installmentamount + ",1, " +
                                       "@transidnew,'" + fm + "'," + fy + "," + loansno + ");";
                                sbqry.Append(qry1);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
                            }
                            else
                            {
                                //loan close

                                string loanClose = "update pr_emp_adv_loans set active=0  where  id=" + emp_adv_loans_mid + " AND active=1; ";
                                adjquery.Append(loanClose);

                                string loanCloseChild = "update pr_emp_adv_loans_child set active=0,interest_accured=interest_amount_recovered  where  id=" + outid2 + " AND active=1; ";
                                adjquery.Append(loanCloseChild);

                                //loan ledger  loan close 

                                // interest installments starts

                                string qry1 = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid]," +
                                    "[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount]," +
                                    "[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on]," +
                                    "[covered_installments],[installments_paid_date],[amount_paid],[installments_paid],[installments_amount]," +
                                    "[active],[trans_id],[fm],[fy],loan_sl_no) values "
                                       + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid1 + "," + 0 + "," +
                                       "" + 0 + "," + 0 + "," + 0 + "," + second_os_inc + "," + 0 + "," +
                                       "'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                                       "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + adj_install_amt2 + "," +
                                       "" + completed_installments + "," + installmentamount + ",1, " +
                                       "@transidnew,'" + fm + "'," + fy + "," + loansno + ");";
                                sbqry.Append(qry1);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
                            }

                        }

                        await _sha.Run_UPDDEL_ExecuteNonQuery(adjquery.ToString());

                    }
                    //priority 2
                    else if (os_principal_amount <= installmentamount && os_principal_amount > 0 && priority == 2 && dtPriority2.Rows.Count > 1)
                    {
                        int outid1 = int.Parse(dtPriority2.Rows[0]["id"].ToString());
                        int outid2 = int.Parse(dtPriority2.Rows[1]["id"].ToString());
                        int intinstallamount = 0;
                        //priority 2 principal

                        //exclude out standing principal and update ost principal  = 0 and princ flag =1
                        adj_principal_amt_recovered2 = adj_principal_amt_recovered2 + adj_os_principal_amount2;
                        total_amount_recovered2 = adj_principal_amt_recovered2;
                        loan_adj_rem = installmentamount - os_principal_amount;
                        Principal_balance_amt2 = adj_principal_amt_recovered2 - total_amount_recovered2;
                        string upqueryclosefstloanprin = "update pr_emp_adv_loans_child set os_principal_amount=0," +
                            "principal_recovered_flag=1,principal_amount_recovered=" + adj_principal_amt_recovered2 + "," +
                            "total_amount_recovered=" + total_amount_recovered2 + " where id=" + outid2 + " AND active=1 AND principal_amount_recovered>0;";
                        adjquery.Append(upqueryclosefstloanprin);
                        //adjquery.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), os_principal_amount.ToString()));

                        //priority 2 principal
                        sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                        string qry1 = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid]," +
                            "[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount]," +
                            "[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date]," +
                            "[amount_paid],[installments_paid],[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                            + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid1 + "," + 0 + "," + adj_install_amt2 + "," +
                            "" + Principal_balance_amt2 + "," + adj_os_interest_amounts2 + "," + 0 + "," + adj_os_interest_amounts2 + ",'" + payment_type + "','" + payment_mode + "'," +
                            " '" + installments_paid_date + "'," + no_of_installments_cover + ",'" + installments_paid_date + "'," +
                            "" + installmentamount + "," + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," +
                            "" + fy + "," + loansno + ");";
                        sbqry.Append(qry + qry1);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

                        intinstallamount = (adj_os_interest_amounts1 + adj_os_interest_amounts2) / intrst_installments;
                        string qry2 = "update pr_emp_adv_loans set interest_installment_amount=" + intinstallamount + " where  id=" + emp_adv_loans_mid + " AND active=1; ";
                        await _sha.Run_UPDDEL_ExecuteNonQuery(qry2.ToString());

                        //priority 1 intrst

                        if (loan_adj_rem > 0 && loan_adj_rem > adj_os_interest_amounts1)
                        {
                            loan_adj_rem = loan_adj_rem - adj_os_interest_amounts1;
                            //update ost interest  = 0 and  intrst flag =1
                            string upqueryclosefstloanint = "update pr_emp_adv_loans_child set os_interest_amount=0,interest_recovered_flag=1,active=0 where  id=" + outid1 + " AND active=1; ";

                            adjquery.Append(upqueryclosefstloanint);
                            //sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                            qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                                "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                                "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid]," +
                                "[installments_paid],[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + outid1 + "," + principal_adj_open_amount + "," + adj_install_amt1 + "," +
                               "" + principal_adj_balance_amount + "," + int_open_amt1 + "," + 0 + "," + 0 + ",'" + payment_type + "'," +
                               "'" + payment_mode + "', '" + installments_paid_date + "'," + no_of_installments_cover + ",'" + installments_paid_date + "'," +
                               "" + installmentamount + "," + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," +
                               "" + loansno + ");";

                            string qry13 = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid],[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid1 + "," + Principal_balance_amt2 + "," + adj_install_amt2 + "," + Principal_balance_amt2 + "," + int_open_amt2 + "," + 0 + "," + 0 + ",'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," + no_of_installments_cover + ",'" + installments_paid_date + "'," + installmentamount + "," + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";
                            sbqry.Append(qry + qry13);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
                        }
                        else if (loan_adj_rem > 0 && loan_adj_rem < adj_os_interest_amounts1)
                        {
                            int os_inst = adj_os_interest_amounts1 - loan_adj_rem;
                            int intrestamtrecovered = adj_os_interest_amounts1 - os_inst;
                            int Int_Paid_amt = intrestamtrecovered;
                            int int_bal_amt = os_inst;
                            loan_adj_rem = 0;
                            int_open_amt1 = os_inst;
                            int_open_amt2 = adj_os_interest_amounts2;
                            adj_install_amt2 = 0;
                            total_amount_recovered = adj_principal_amt_recovered1;
                            // exclude interest amount
                            string upqueryclosefstloanint = "update pr_emp_adv_loans_child set " +
                            " principal_amount_recovered=" + adj_principal_amt_recovered1 + "," +
                           " total_amount_recovered=" + total_amount_recovered + "," +
                            "interest_amount_recovered=" + intrestamtrecovered + ",os_interest_amount=" + os_inst + "" +
                            " where  id=" + outid1 + " AND active=1  AND principal_amount_recovered>0; ";
                            adjquery.Append(upqueryclosefstloanint);
                            //sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                            qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                                "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount]," +
                                "[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid]," +
                                "[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + outid1 + "," + principal_adj_open_amount + "," + adj_install_amt1 + "," +
                               "" + principal_adj_balance_amount + "," + adj_os_interest_amounts1 + "," + Int_Paid_amt + "," + int_bal_amt + ",'" + payment_type + "'," +
                               "'" + payment_mode + "', '" + installments_paid_date + "'," + no_of_installments_cover + ",'" + installments_paid_date + "'," +
                               "" + Int_Paid_amt + "," + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";

                            string qry14 = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid]," +
                                "[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount]," +
                                "[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on]," +
                                "[covered_installments],[installments_paid_date],[amount_paid],[installments_paid],[installments_amount]," +
                                "[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid1 + "," + Principal_balance_amt2 + "," +
                               "" + adj_install_amt2 + "," + Principal_balance_amt2 + "," + int_open_amt2 + "," + 0 + "," + int_open_amt2 + "," +
                               "'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," + no_of_installments_cover + "," +
                               "'" + installments_paid_date + "'," + installmentamount + "," + completed_installments + "," + installmentamount + "," +
                               "1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";
                            sbqry.Append(qry + qry14);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

                            intinstallamount = (int_open_amt1 + int_open_amt2) / intrst_installments;
                            qry2 = "update pr_emp_adv_loans set interest_installment_amount=" + intinstallamount + " where  id=" + emp_adv_loans_mid + " AND active=1; ";
                            await _sha.Run_UPDDEL_ExecuteNonQuery(qry2.ToString());
                        }
                        //priority 2 intrst

                        if (dtPriority2.Rows.Count >= 2 && loan_adj_rem > 0)
                        {


                            //priotity 2 out intrst 
                            int second_os_inc = int.Parse(dtPriority2.Rows[1]["os_interest_amount"].ToString());
                            if (second_os_inc > loan_adj_rem)
                            {
                                second_os_inc = second_os_inc - loan_adj_rem;
                                //priority 2 principal amount exclude 
                                string upquerycloseScndloanprin = "update pr_emp_adv_loans_child set os_interest_amount=" + second_os_inc + " where  id=" + outid2 + " AND active=1; ";
                                adjquery.Append(upquerycloseScndloanprin);
                            }
                            else
                            {
                                //loan close

                                string loanClose = "update pr_emp_adv_loans set active=0  where  id=" + emp_adv_loans_mid + " AND active=1; ";
                                adjquery.Append(loanClose);

                                string loanCloseChild = "update pr_emp_adv_loans_child set active=0,interest_amount_recovered=" + second_os_inc + ",os_interest_amount=0,interest_recovered_flag=1  where  id=" + outid2 + " AND active=1; ";
                                adjquery.Append(loanCloseChild);

                            }

                        }


                        await _sha.Run_UPDDEL_ExecuteNonQuery(adjquery.ToString());

                    }
                    //single loan
                    else if (os_principal_amount < installmentamount && os_principal_amount > 0 && priority == 1 && dtPriority2.Rows.Count == 1)
                    {
                        int outid1 = int.Parse(dtPriority2.Rows[0]["id"].ToString());

                        adj_principal_amt_recovered1 = adj_principal_amt_recovered1 + adj_os_principal_amount1;
                        total_amount_recovered = adj_principal_amt_recovered1;
                        loan_adj_rem = installmentamount - os_principal_amount;
                        if (loan_adj_rem > 0)
                        {
                            string upqueryclosefstloanprin = "update pr_emp_adv_loans_child set os_principal_amount=0," +
                            "principal_recovered_flag=1,principal_amount_recovered=" + adj_principal_amt_recovered1 + "," +
                            "total_amount_recovered=" + total_amount_recovered + " where id=" + outid1 + " " +
                            "AND active=1 AND principal_amount_recovered>0;";
                            adjquery.Append(upqueryclosefstloanprin);
                            //loan adjustment for principle
                            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                            qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                                "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount]," +
                                "[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments]," +
                                "[installments_paid_date],[amount_paid],[installments_paid],[installments_amount]," +
                                "[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + os_principal_amount + "," +
                               "" + loan_adj_rem + "," + adj_os_principal_amount1 + "," + os_interest_amounts + "," + 0 + "," + os_interest_amounts + "," +
                               "'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                               "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + os_interest_amounts + "," +
                               "" + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";

                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

                        }
                        else
                        {
                            string upqueryclosefstloanprin = "update pr_emp_adv_loans_child set os_principal_amount=0," +
                            "principal_recovered_flag=1,principal_amount_recovered=" + adj_principal_amt_recovered1 + "," +
                            "total_amount_recovered=" + total_amount_recovered + ", active=0 where id=" + outid1 + " AND active=1 AND principal_amount_recovered>0;";
                            adjquery.Append(upqueryclosefstloanprin);
                            if (adj_os_interest_amounts1 <= 0)
                            {
                                string loanClose = "update pr_emp_adv_loans set active=0 where id=" + emp_adv_loans_mid + ";";
                                adjquery.Append(loanClose);
                            }
                            //
                            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                            qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                                "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount]," +
                                "[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments]," +
                                "[installments_paid_date],[amount_paid],[installments_paid],[installments_amount]," +
                                "[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + os_principal_amount + "," +
                               "" + loan_adj_rem + "," + 0 + "," + os_interest_amounts + "," + 0 + "," + os_interest_amounts + "," +
                               "'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                               "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + os_interest_amounts + "," +
                               "" + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";

                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
                            if (adj_os_interest_amounts1 > 0)
                            {
                                int intinstallamount = 0;
                                intinstallamount = adj_os_interest_amounts1 / intrst_installments;
                                string qry2 = "update pr_emp_adv_loans set interest_installment_amount=" + intinstallamount + " where  id=" + emp_adv_loans_mid + " AND active=1; ";
                                await _sha.Run_UPDDEL_ExecuteNonQuery(qry2.ToString());
                            }
                        }


                        if (loan_adj_rem > 0 && loan_adj_rem > os_interest_amounts)
                        {
                            loan_adj_rem = loan_adj_rem - os_interest_amounts;
                            //update ost interest  = 0 and  intrst flag =1
                            string upqueryclosefstloanint = "update pr_emp_adv_loans_child set os_interest_amount=0,interest_recovered_flag=1,active=0 where  id=" + outid1 + " AND active=1; ";
                            adjquery.Append(upqueryclosefstloanint);

                            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                            qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                                "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount]," +
                                "[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments]," +
                                "[installments_paid_date],[amount_paid],[installments_paid],[installments_amount]," +
                                "[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + 0 + "," +
                               "" + 0 + "," + 0 + "," + os_interest_amounts + "," + os_interest_amounts + "," + 0 + "," +
                               "'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                               "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + os_interest_amounts + "," +
                               "" + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";

                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));


                        }
                        else if (loan_adj_rem > 0 && loan_adj_rem <= os_interest_amounts)
                        {

                            int os_inst = os_interest_amounts - loan_adj_rem;
                            int intrestamtrecovered = os_interest_amounts - os_inst;
                            loan_adj_rem = 0;
                            // exclude interest amount
                            string upqueryclosefstloanint = "update pr_emp_adv_loans_child set " +
                            " principal_amount_recovered=" + adj_principal_amt_recovered1 + "," +
                            " total_amount_recovered=" + total_amount_recovered + "," +
                            "interest_amount_recovered=" + intrestamtrecovered + ",os_interest_amount=" + os_inst + "" +
                            " , active=0 where  id=" + outid1 + " AND active=1  AND principal_amount_recovered>0; ";
                            adjquery.Append(upqueryclosefstloanint);

                            string loanClose = "update pr_emp_adv_loans set active=0 ,total_recovered_amount=" + total_amount_recovered + " where id=" + emp_adv_loans_mid + ";";
                            adjquery.Append(loanClose);


                            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                            qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount]," +
                                "[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount]," +
                                "[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments]," +
                                "[installments_paid_date],[amount_paid],[installments_paid],[installments_amount]," +
                                "[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + 0 + "," +
                               "" + 0 + "," + 0 + "," + os_interest_amounts + "," + loan_adj_rem + "," + os_inst + "," +
                               "'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," +
                               "" + no_of_installments_cover + ",'" + installments_paid_date + "'," + os_interest_amounts + "," +
                               "" + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";

                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

                        }

                        await _sha.Run_UPDDEL_ExecuteNonQuery(adjquery.ToString());
                    }
                    //Interest Priority 1


                    //monthly installment
                    else
                    {
                        int os_total_amount = int.Parse(dtloansdata.Rows[0]["os_total_amount"].ToString());
                        float os_interest_amount = int.Parse(dtloansdata.Rows[0]["os_interest_amount"].ToString());
                        int os_interest_amountt = int.Parse(dtloansdata.Rows[0]["os_interest_amount"].ToString());
                        int loan_amount = int.Parse(dtloansdata.Rows[0]["loan_amount"].ToString());
                        int intinstallments = int.Parse(dtloansdata.Rows[0]["interest_installment"].ToString());


                        bool principal_recovered_flag = bool.Parse(dtloansdata.Rows[0]["principal_recovered_flag"].ToString());
                        bool interest_recovered_flag = bool.Parse(dtloansdata.Rows[0]["interest_recovered_flag"].ToString());
                        loansno = int.Parse(dtloansdata.Rows[0]["loan_sl_no"].ToString());

                        int principal_amount_recovered = int.Parse(dtloansdata.Rows[0]["principal_amount_recovered"].ToString());
                        int principal_open_amount = os_principal_amount;
                        int principal_balance_amount = principal_open_amount - installmentamount;
                        int principal_paid_amt = installmentamount + principal_amount_recovered;
                        if (principal_balance_amount <= 0)
                        {
                            principal_balance_amount = 0;
                        }

                        os_principal_amount = principal_balance_amount;
                        os_total_amount = principal_balance_amount;



                        ///// insert 2nd subloan  26/09/19

                        int installmentamount2 = 0;
                        int completed_installments2 = 0;
                        int remaining_installments2 = 0;
                        int no_of_installments_cover2 = 0;
                        int total_installments2 = 0;
                        int slno = 0;
                        int os_total_amount2 = 0;
                        int os_principal_amount2 = 0;
                        int principal_open_amount2 = 0;
                        int principal_balance_amount2 = 0;
                        int Int_accured2 = 0;
                        if (dt.Rows.Count > 0)
                        {

                            completed_installments2 = int.Parse(dt.Rows[0]["completed_installment"].ToString());
                            remaining_installments2 = int.Parse(dt.Rows[0]["remaining_installment"].ToString());
                            no_of_installments_cover2 = 0;
                            total_installments2 = int.Parse(dt.Rows[0]["total_installment"].ToString());
                            completed_installments2 = 0;
                            remaining_installments2 = total_installments2 - completed_installments2;
                            Int_accured2 = int.Parse(dt.Rows[0]["interest_accured"].ToString());
                            slno = int.Parse(dt.Rows[0]["slno"].ToString());

                            os_total_amount2 = int.Parse(dt.Rows[0]["os_total_amount"].ToString());
                            os_principal_amount2 = int.Parse(dt.Rows[0]["os_principal_amount"].ToString());
                            principal_open_amount2 = os_principal_amount2;
                            principal_balance_amount2 = principal_open_amount2 - installmentamount2;
                            if (principal_balance_amount2 <= 0)
                            {
                                principal_balance_amount2 = 0;
                            }

                            os_principal_amount2 = principal_balance_amount2;
                            os_total_amount2 = principal_balance_amount2;
                        }



                        //Payments Dates
                        //DateTime paidDate = Convert.ToDateTime(dtfince.Rows[0]["fm"].ToString());
                        //string installments_paid_date = (paidDate.ToString("yyyy-MM-dd"));
                        //string Cash_paid_on = installments_paid_date;
                        //int fy = Convert.ToInt32(dtfince.Rows[0]["fy"].ToString());
                        //string fm = installments_paid_date;
                        //int Month = paidDate.Month;
                        //int Year = paidDate.Year;

                        //PaymentMode

                        // name change salary to instalment
                        //string payment_type = "Salary";
                        //string payment_type = "installment";
                        //string payment_mode = "installment";


                        //when loan not completed
                        if (principal_amount_recovered != loan_amount && principal_recovered_flag == false)
                        {
                            //1. qry---insert into pr_emp_adv_loans_adjustments
                            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                            qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid],[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + installmentamount + "," + principal_balance_amount + "," + 0 + "," + 0 + "," + 0 + ",'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," + no_of_installments_cover + ",'" + installments_paid_date + "'," + installmentamount + "," + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));

                            if (dtPriority2.Rows.Count >= 2)
                            {
                                if (principal_amount_recovered != loan_amount && principal_recovered_flag == false && slno == 2)
                                {
                                    ///// insert 2nd subloan  26/09/19
                                    string qry1 = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid],[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                                   + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid1 + "," + principal_open_amount2 + "," + installmentamount2 + "," + principal_balance_amount2 + "," + Int_accured2 + "," + 0 + "," + Int_accured2 + ",null,null,'" + installments_paid_date + "'," + no_of_installments_cover2 + ",null," + installmentamount2 + "," + completed_installments2 + ",null ,0, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";

                                    sbqry.Append(qry1);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));
                                }
                            }

                            //2. qry update pr_emp_adv_loans_child table

                            total_amount_recovered = int.Parse(dtloansdata.Rows[0]["total_amount_recovered"].ToString());

                            principal_amount_recovered = principal_amount_recovered + installmentamount;
                            total_amount_recovered = principal_amount_recovered;

                            double total_recovered_amount = installmentamount * completed_installments;


                            if (total_amount_recovered != loan_amount)
                            {
                                qry = "update pr_emp_adv_loans_child SET principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + " where id=" + emp_adv_loans_child_mid + " AND active=1;";
                                //string instUpdates = "update pr_emp_adv_loans SET interest_installment_amount=" + interestinstallmentamts + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                                sbqry.Append(qry);

                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));
                            }
                            else
                            {

                                if (loanType == PrConstants.FESTIVAL_LOAN_CODE)
                                {
                                    qry = "update pr_emp_adv_loans_child SET principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + ",active=0 " + ",principal_recovered_flag=1" + " where id=" + emp_adv_loans_child_mid + ";";
                                    sbqry.Append(qry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));
                                }
                                else if (loanType == PrConstants.VEH_LOANLT5_CODE || loanType == PrConstants.VEH_LOANLT6_CODE)
                                {
                                    int interestinstallmentamts = os_interest_amountt / intinstallments;
                                    qry = "update pr_emp_adv_loans_child SET principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + ",principal_recovered_flag=1" + " where id=" + emp_adv_loans_child_mid + ";";
                                    string instUpdates = "update pr_emp_adv_loans SET interest_installment_amount=" + interestinstallmentamts + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                                    sbqry.Append(qry + instUpdates);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));
                                }
                                else
                                {

                                    //principal recovered end
                                    qry = "update pr_emp_adv_loans_child SET principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + ",principal_recovered_flag=1" + " where id=" + emp_adv_loans_child_mid + ";";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));

                                    //sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                                    string qryAdj = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid],[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                                 + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + 0 + "," + installmentamount + "," + 0 + "," + os_interest_amountt + "," + 0 + "," + os_interest_amountt + ",'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," + no_of_installments_cover + ",'" + installments_paid_date + "'," + installmentamount + "," + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";
                                    sbqry.Append(qryAdj);

                                    // update interest amount 
                                    bRet = UpdateSubLoansForPrincipalamountInterestAmount(emp_adv_loans_mid, emp_adv_loans_child_mid, Month, Year, paidDate, loansno).GetAwaiter().GetResult();
                                    double? interestinstallmentamt = 0;
                                    string selquery = "select top 1 ch.os_interest_amount,ch.principal_recovered_flag,ch.total_interest_installments," +
                                        "ch.interest_recovered_flag,adv.interest_installment from pr_emp_adv_loans_child " +
                                        "ch join pr_emp_adv_loans adv on adv.id =ch.emp_adv_loans_mid " +
                                        "where adv.active = 1 AND adv.emp_code =" + EmpCode + " " +
                                        "AND interest_recovered_flag !=1 and ch.emp_adv_loans_mid=" + emp_adv_loans_mid + " order by ch.interest_rate desc;";
                                    DataTable dttt = await _sha.Get_Table_FromQry(selquery);

                                    os_interest_amount = int.Parse(dttt.Rows[0]["os_interest_amount"].ToString());
                                    //interest installment calculation
                                    //if (interest_recovered_flag==false && principal_recovered_flag==true && os_interest_amount>0)
                                    //{

                                    StringBuilder instUpdate = new StringBuilder();
                                    int interestinstallments = int.Parse(dttt.Rows[0]["interest_installment"].ToString());
                                    interestinstallmentamt = Convert.ToDouble(os_interest_amount / interestinstallments);
                                    string instUpdates = "update pr_emp_adv_loans SET interest_installment_amount=" + interestinstallmentamt + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                                    sbqry.Append(instUpdates);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), emp_adv_loans_mid.ToString()));
                                    //await _sha.Run_UPDDEL_ExecuteNonQuery(instUpdate.ToString());
                                    //}

                                }


                            }


                            //3.qry update pr_emp_adv_loans table
                            if (total_amount_recovered != loan_amount)
                            {
                                qry = "update pr_emp_adv_loans SET completed_installment=" + completed_installments + ",remaining_installment=" + remaining_installments + " ,total_recovered_amount=" + total_recovered_amount + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                                sbqry.Append(qry);

                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), EmpCode.ToString()));
                            }
                            else if (loanType == PrConstants.FESTIVAL_LOAN_CODE)
                            {

                                qry = "update pr_emp_adv_loans SET completed_installment=" + completed_installments + ",remaining_installment=" + remaining_installments + " ,total_recovered_amount=" + total_recovered_amount + ",active=0 " + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                                sbqry.Append(qry);

                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), EmpCode.ToString()));

                            }
                            //else if (loanType == PrConstants.VEH_LOANLT5_CODE || loanType == PrConstants.VEH_LOANLT6_CODE)
                            //{
                            //    qry = "update pr_emp_adv_loans_child SET principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + ",active=0 " + ",principal_recovered_flag=1" + " where id=" + emp_adv_loans_child_mid + ";";
                            //    sbqry.Append(qry);

                            //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));
                            //}

                        }
                        else
                        {

                            float interest_open_amount = os_interest_amount;
                            int interest_paid_amount = installmentamount;
                            float interest_balance_amount = interest_open_amount - installmentamount;
                            int interest_accured = int.Parse(dtloansdata.Rows[0]["interest_accured"].ToString());
                            int inst_amount_recovered = int.Parse(dtloansdata.Rows[0]["interest_amount_recovered"].ToString());

                            inst_amount_recovered = inst_amount_recovered + interest_paid_amount;

                            int diff_inst = interest_accured - inst_amount_recovered;
                            os_interest_amount = diff_inst;

                            total_amount_recovered = int.Parse(dtloansdata.Rows[0]["total_amount_recovered"].ToString());
                            int interest_amount_recovered = int.Parse(dtddamount.Rows[0]["dd_amount"].ToString());

                            total_amount_recovered += interest_amount_recovered;

                            //int interest_balance_amount = interest_accured - interest_amount_recovered;


                            //1. qry---insert into pr_emp_adv_loans_adjustments
                            sbqry.Append(GetNewNumString("pr_emp_adv_loans_adjustments"));
                            qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[installments_paid_date],[amount_paid],[installments_paid],[installments_amount],[active],[trans_id],[fm],[fy],loan_sl_no) values "
                               + "(@idnew," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + 0 + "," + 0 + "," + 0 + "," + interest_open_amount + "," + interest_paid_amount + "," + interest_balance_amount + ",'" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," + no_of_installments_cover + ",'" + installments_paid_date + "'," + installmentamount + "," + completed_installments + "," + installmentamount + ",1, @transidnew,'" + fm + "'," + fy + "," + loansno + ");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew", ""));


                            //if (os_interest_amount > 0 )
                            if (interest_accured != inst_amount_recovered)
                            {
                                //os_interest_amount = interest_accured - 
                                qry = "update pr_emp_adv_loans_child SET " +
                                    "interest_amount_recovered=interest_amount_recovered + " + interest_amount_recovered + ", " +
                                    "os_interest_amount=os_interest_amount - " + interest_paid_amount + ", " +
                                    " os_total_amount= " + os_total_amount + "  where id=" + emp_adv_loans_child_mid + " AND active=1;";
                                sbqry.Append(qry);

                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));


                                qry = "update pr_emp_adv_loans SET completed_installment=" + completed_installments + ",remaining_installment=" + remaining_installments + " ,total_recovered_amount=" + total_amount_recovered + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                                sbqry.Append(qry);

                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), EmpCode.ToString()));

                            }
                            else
                            {
                                string upqry = "update pr_emp_adv_loans_child SET interest_amount_recovered=interest_amount_recovered+" + interest_amount_recovered + " , os_interest_amount=" + os_interest_amount + ", os_total_amount= " + os_total_amount + ",active=0 " + ",interest_recovered_flag = 1" + " where id=" + emp_adv_loans_child_mid + ";";
                                //sbqry.Append(upqry);
                                //string  upqrys = "update pr_emp_adv_loans SET active=0 where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + " ";
                                //sbqry.Append(upqrys);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));
                                await _sha.Run_UPDDEL_ExecuteNonQuery(upqry);

                                string instFlagsQry = " select os_interest_amount from pr_emp_adv_loans_child where emp_adv_loans_mid=" + emp_adv_loans_mid + ";";
                                DataTable instFlagsTable = await _sha.Get_Table_FromQry(instFlagsQry);
                                if (instFlagsTable.Rows.Count >= 2)
                                {
                                    int osinterestamt1 = Convert.ToInt32(instFlagsTable.Rows[0]["os_interest_amount"]);
                                    int osinterestamt2 = Convert.ToInt32(instFlagsTable.Rows[1]["os_interest_amount"]);
                                    if (osinterestamt1 == 0 && osinterestamt2 != 0)
                                    {
                                        //double? interestinstallmentamts = 0;
                                        string selsquery = "select top 1 ch.os_interest_amount,ch.principal_recovered_flag,ch.total_interest_installments,ch.interest_recovered_flag from pr_emp_adv_loans_child " +
                                            "ch join pr_emp_adv_loans adv on adv.id =ch.emp_adv_loans_mid " +
                                            "where adv.active = 1 AND adv.emp_code =" + EmpCode + " " +
                                            "AND interest_recovered_flag !=1 and ch.emp_adv_loans_mid=" + emp_adv_loans_mid + " and os_interest_amount>0 order by ch.interest_rate desc;";
                                        DataTable dttts = await _sha.Get_Table_FromQry(selsquery);

                                        os_interest_amount = int.Parse(dttts.Rows[0]["os_interest_amount"].ToString());
                                        //interest installment calculation
                                        //if (interest_recovered_flag==false && principal_recovered_flag==true && os_interest_amount>0)
                                        //{


                                        //int interestinstallments = int.Parse(dttts.Rows[0]["total_interest_installments"].ToString());
                                        //interestinstallmentamts = Convert.ToDouble(os_interest_amount / interestinstallments);
                                        //string instUpdates = "update pr_emp_adv_loans SET interest_installment_amount=" + interestinstallmentamts + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                                        //sbqry.Append(instUpdates);

                                        //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), emp_adv_loans_mid.ToString()));
                                    }
                                }
                            }
                            string instFlagQry = " select os_interest_amount from pr_emp_adv_loans_child where emp_adv_loans_mid=" + emp_adv_loans_mid + " AND active=1;";
                            DataTable instFlagTable = await _sha.Get_Table_FromQry(instFlagQry);
                            if (instFlagTable.Rows.Count >= 2)
                            {
                                int osinterestamt1 = Convert.ToInt32(instFlagTable.Rows[0]["os_interest_amount"]);
                                int osinterestamt2 = Convert.ToInt32(instFlagTable.Rows[1]["os_interest_amount"]);
                                if (osinterestamt1 == 0 && osinterestamt2 == 0)
                                {
                                    qry = "update pr_emp_adv_loans SET active=0 " + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), EmpCode.ToString()));

                                }
                            }
                            //adv loans active 0
                            else if (diff_inst == 0 && os_interest_amount_first == 0)
                            {
                                qry = "update pr_emp_adv_loans SET completed_installment=" + completed_installments + ",remaining_installment=" + remaining_installments + " ,total_recovered_amount=" + total_amount_recovered + ",active=0 " + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                                string upqrys = "update pr_emp_adv_loans SET active=0 where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + " ";
                                sbqry.Append(qry + upqrys);

                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), EmpCode.ToString()));
                            }

                        }
                    }
                }
                try
                {
                    bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

                }
                catch (Exception ex)
                {
                    _logger.Info(sbqry.ToString());
                    _logger.Error(ex.Message);
                    _logger.Error(ex.StackTrace);

                    bRet = false;
                }
            }

            catch (Exception e)
            {

            }



            return bRet;

        }
        private async Task<bool> InstallmentAmountNewReverse(int Empcode, int loans_id, int loan_type_mid, int loan_sl_no, int emp_adv_loans_child_mid1, int emp_adv_loans_child_mid2, int Month, int Year)
        {
            LoansPaymentProcess loanProc = new LoansPaymentProcess(getLoginCredential(), _logger);
            var flg = false;
            StringBuilder sbqry = new StringBuilder();
            sbqry.Clear();
            //sbqry.Append(GenNewTransactionString());
            int loan_id = 0;
            int child_id = 0;
            int adj_id = 0;
            int completed_installments = 0;
            int remaining_installments = 0;
            double total_recovered_amount = 0;
            double installment_amount = 0;
            double interest_installment_amount = 0;
            double interest_accured = 0;
            double principal_amount_recovered = 0;
            double interest_amount_recovered = 0;
            double total_amount_recovered = 0;
            double os_principal_amount = 0;
            double os_interest_amount=0;
            double os_this_month_interest = 0;
            double os_total_amount = 0;
            bool prin_recovered_flag;
            bool int_recovered_flag;
            bool bRet = false;
            int fm = Month - 1;

            string orgtablequery = "select * from pr_emp_adv_loans_adjustments where emp_adv_loans_mid = "+ loans_id + " ";
            string oldtablequery = "select * from pr_emp_adv_loans_adjustments_bef_monthend where emp_adv_loans_mid = " + loans_id + " ";
            DataTable dtorgtable = await _sha.Get_Table_FromQry(orgtablequery);
            DataTable dtoldtable = await _sha.Get_Table_FromQry(oldtablequery);
            if(dtorgtable.Rows.Count>0 && dtoldtable.Rows.Count==0)
            {
                string ParentChildtablequery = "select l.id as loan_id,ch.id as childid, l.loan_type_mid, l.completed_installment,l.remaining_installment,l.installment_amount," +
                    "l.interest_installment_amount,l.total_recovered_amount,ch.interest_accured,ch.principal_amount_recovered,ch.interest_amount_recovered," +
                    "ch.total_amount_recovered,ch.os_principal_amount,ch.os_interest_amount,ch.os_this_month_interest,ch.os_total_amount, " +
                    "ch.principal_recovered_flag,ch.interest_recovered_flag,ch.active from  pr_emp_adv_loans_child_bef_monthend ch " +
                     "join pr_emp_adv_loans_bef_monthend l on ch.emp_adv_loans_mid = l.id where emp_code = " + Empcode + " " +
                      "and l.loan_type_mid = " + loan_type_mid + " ";
                DataTable dtParentChildtablequery = await _sha.Get_Table_FromQry(ParentChildtablequery);

                foreach (DataRow elnddss in dtParentChildtablequery.Rows)
                {
                    //parent table 
                    loan_id = int.Parse(elnddss["loan_id"].ToString());
                    child_id = int.Parse(elnddss["childid"].ToString());
                    completed_installments = int.Parse(elnddss["completed_installment"].ToString());
                    remaining_installments = int.Parse(elnddss["remaining_installment"].ToString());
                    installment_amount = Convert.ToDouble(elnddss["installment_amount"].ToString());
                    interest_installment_amount = Convert.ToDouble(elnddss["interest_installment_amount"].ToString());
                    total_recovered_amount = Convert.ToDouble(elnddss["total_recovered_amount"].ToString());

                    //child table
                    interest_accured = Convert.ToDouble(elnddss["interest_accured"].ToString());
                    principal_amount_recovered = Convert.ToDouble(elnddss["principal_amount_recovered"].ToString());
                    interest_amount_recovered = Convert.ToDouble(elnddss["interest_amount_recovered"].ToString());
                    total_amount_recovered = Convert.ToDouble(elnddss["total_amount_recovered"].ToString());
                    os_principal_amount = Convert.ToDouble(elnddss["os_principal_amount"].ToString());
                    os_interest_amount = Convert.ToDouble(elnddss["os_interest_amount"].ToString());
                    os_this_month_interest = Convert.ToDouble(elnddss["os_this_month_interest"].ToString());
                    os_total_amount = Convert.ToDouble(elnddss["os_total_amount"].ToString());
                    prin_recovered_flag = bool.Parse(elnddss["principal_recovered_flag"].ToString());
                    int_recovered_flag = bool.Parse(elnddss["interest_recovered_flag"].ToString());

                    sbqry.Append("update pr_emp_adv_loans set completed_installment = " + completed_installments + ", " +
              "remaining_installment =" + remaining_installments + ",installment_amount=" + installment_amount + "," +
              "interest_installment_amount=" + interest_installment_amount + ",total_recovered_amount=" + total_recovered_amount + "" +
              " where id=" + loan_id + " " +
              "and loan_type_mid=" + loan_type_mid + ";");

                    sbqry.Append("update pr_emp_adv_loans_child set interest_accured = " + interest_accured + ", " +
                                           "principal_amount_recovered =" + principal_amount_recovered + "," +
                                            "interest_amount_recovered =" + interest_amount_recovered + "," +
                                             " total_amount_recovered =" + total_amount_recovered + "," +
                                              " os_principal_amount =" + os_principal_amount + ", " +
                                               "os_interest_amount =" + os_interest_amount + ",os_this_month_interest=" + os_this_month_interest + "," +
                                               "os_total_amount=" + os_total_amount + ",principal_recovered_flag='" + prin_recovered_flag + "',interest_recovered_flag='" + int_recovered_flag + "' " +
                                             " where id=" + child_id + " and emp_adv_loans_mid= " + loan_id + "");
                    sbqry.Append("delete from  pr_emp_adv_loans_adjustments where emp_adv_loans_mid = " + loan_id + "  and emp_adv_loans_child_mid=" + child_id + "  AND month(fm)=" + Month + " and year(fm)=" + Year + " and  payment_type!='Full Clearing' ");

                }
                }

            else
            {
                string qry = "select l.id as loan_id,ch.id as childid,adj.id as adjid,l.loan_type_mid," +
                "l.completed_installment,l.remaining_installment,l.installment_amount,l.interest_installment_amount," +
                "l.total_recovered_amount,ch.interest_accured,ch.principal_amount_recovered,ch.interest_amount_recovered," +
                "ch.total_amount_recovered,ch.os_principal_amount,ch.os_interest_amount,ch.os_this_month_interest,ch.os_total_amount," +
                "ch.principal_recovered_flag,ch.interest_recovered_flag,ch.active  " +
                "from pr_emp_adv_loans_adjustments_bef_monthend adj join pr_emp_adv_loans_child_bef_monthend ch on adj.emp_adv_loans_child_mid = ch.id " +
                "join pr_emp_adv_loans_bef_monthend l on ch.emp_adv_loans_mid = l.id where emp_code = " + Empcode + " " +
                "and l.loan_type_mid = " + loan_type_mid + " AND month(adj.fm)=" + fm + " " +
                "and year(adj.fm)=" + Year + " and payment_type not in('Full Clearing','Part Payment') ";

            DataSet loansdata = await _sha.Get_MultiTables_FromQry(qry);
            DataTable loansreverertdata = loansdata.Tables[0];

            if (loansreverertdata.Rows.Count > 0)
            {
                foreach (DataRow elnddss in loansreverertdata.Rows)
                {
                    //parent table 
                    loan_id = int.Parse(elnddss["loan_id"].ToString());
                    child_id = int.Parse(elnddss["childid"].ToString());
                    adj_id = int.Parse(elnddss["adjid"].ToString());
                     completed_installments = int.Parse(elnddss["completed_installment"].ToString());
                    remaining_installments = int.Parse(elnddss["remaining_installment"].ToString());
                    installment_amount = Convert.ToDouble(elnddss["installment_amount"].ToString());
                    interest_installment_amount = Convert.ToDouble(elnddss["interest_installment_amount"].ToString());
                    total_recovered_amount= Convert.ToDouble(elnddss["total_recovered_amount"].ToString());

                    //child table
                    interest_accured = Convert.ToDouble(elnddss["interest_accured"].ToString());
                    principal_amount_recovered = Convert.ToDouble(elnddss["principal_amount_recovered"].ToString());
                    interest_amount_recovered = Convert.ToDouble(elnddss["interest_amount_recovered"].ToString());
                    total_amount_recovered = Convert.ToDouble(elnddss["total_amount_recovered"].ToString());
                    os_principal_amount = Convert.ToDouble(elnddss["os_principal_amount"].ToString());
                    os_interest_amount = Convert.ToDouble(elnddss["os_interest_amount"].ToString());
                    os_this_month_interest = Convert.ToDouble(elnddss["os_this_month_interest"].ToString());
                    os_total_amount = Convert.ToDouble(elnddss["os_total_amount"].ToString());
                    prin_recovered_flag = bool.Parse(elnddss["principal_recovered_flag"].ToString());
                    int_recovered_flag = bool.Parse(elnddss["interest_recovered_flag"].ToString());


                    sbqry.Append("update pr_emp_adv_loans set completed_installment = "+ completed_installments + ", " +
              "remaining_installment ="+ remaining_installments + ",installment_amount="+ installment_amount + "," +
              "interest_installment_amount="+ interest_installment_amount + ",total_recovered_amount="+ total_recovered_amount + "" +
              " where id=" + loan_id + " " +
              "and loan_type_mid=" + loan_type_mid + ";");

                    sbqry.Append("update pr_emp_adv_loans_child set interest_accured = "+ interest_accured + ", " +
    "principal_amount_recovered ="+ principal_amount_recovered + "," +
    "interest_amount_recovered ="+ interest_amount_recovered + "," +
    " total_amount_recovered ="+ total_amount_recovered + "," +
    " os_principal_amount ="+ os_principal_amount + ", " +
    "os_interest_amount ="+ os_interest_amount + ",os_this_month_interest="+ os_this_month_interest + "," +
    "os_total_amount="+ os_total_amount + ",principal_recovered_flag='" + prin_recovered_flag + "',interest_recovered_flag='"+ int_recovered_flag + "' " +
    " where id=" + child_id + " and emp_adv_loans_mid= " + loan_id + "");

                    sbqry.Append("delete from  pr_emp_adv_loans_adjustments where emp_adv_loans_mid = " + loan_id + "  and emp_adv_loans_child_mid="+ child_id + "  AND month(fm)=" + Month + " and year(fm)=" + Year + " and  payment_type!='Full Clearing' ");
                    }
                    //bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

                    //if (!elnddss.IsNull("interest_accured"))
                    //{
                    //    Int_accured = Convert.ToDouble(elnddss["interest_accured"].ToString());
                    //}
                    //flg = loanProc.RevertLoansData(loan_id, child_id, adj_id, instAmt, intinstAmt, prin_recovered_flag, int_recovered_flag, Prin_Paid_amt, Int_Paid_amt, Int_accured, loan_type_mid, Month, Year, 'I').GetAwaiter().GetResult();

                }
            }

            try
            {
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                _logger.Info(Empcode + "Process Sucessfully");
            }
            catch (Exception ex)
            {
                _logger.Info(Empcode + " " + ex);
                _logger.Info(sbqry.ToString());
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);

                bRet = false;
            }
            return bRet;
        }

        #region calculation of installment amount
        private async Task<bool> InstallmentAmountNew(int EmpCode, int emp_adv_loans_mid, int emp_adv_loans_child_mid, int emp_adv_loans_child_mid1, int loanid, string loanType, int month, int year, double os_interest_amount_first, int loansno)
        {
            LoansPaymentProcess loanProc = new LoansPaymentProcess(getLoginCredential(), _logger);
            var flg = false;

            StringBuilder sbqry = new StringBuilder();

            sbqry.Append(GenNewTransactionString());
            int loan_id = 0;
            double instAmt = 0;
            bool bRet = false;

            string qry = "";

            string qrygetinstallment_amount = "select distinct dd_amount,ded.dd_mid from pr_emp_payslip psl " +
                                             " join pr_emp_payslip_deductions ded on psl.id = ded.payslip_mid " +
                                             " where psl.emp_code=" + EmpCode + " AND ded.dd_type='loan'" + " AND ded.dd_mid=" + emp_adv_loans_mid + " AND month(psl.fm)=" + month + " and ded.dd_amount>0 AND ded.active=1 and year(psl.fm)=" + year;

            DataSet dsfestlonas = await _sha.Get_MultiTables_FromQry(qrygetinstallment_amount);


            DataTable dtddamount = dsfestlonas.Tables[0];

            if (dtddamount.Rows.Count > 0)
            {
                foreach (DataRow elnddss in dtddamount.Rows)
                {
                    loan_id = int.Parse(elnddss["dd_mid"].ToString());
                    instAmt = Convert.ToDouble(elnddss["dd_amount"].ToString());

                    //test loans payments            
                    flg = loanProc.InstallmentPartPayments(loan_id, instAmt, 'I').GetAwaiter().GetResult();
                }
            }

            else
            {
                flg = loanProc.InstallmentPartPayments(emp_adv_loans_mid, instAmt, 'I').GetAwaiter().GetResult();
            }
            try
            {
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                _logger.Info(EmpCode + "Process Sucessfully");
            }
            catch (Exception ex)
            {
                _logger.Info(EmpCode + " " + ex);
                _logger.Info(sbqry.ToString());
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);

                bRet = false;
            }
            return bRet;
        }
        #endregion
        private async Task<bool> InstallmentAmountNewBeforeMonthEnd(int EmpCode, int emp_adv_loans_mid, int emp_adv_loans_child_mid, int emp_adv_loans_child_mid1, int loanid, string loanType, int month, int year, double os_interest_amount_first, int loansno)
        {
            LoansBeforeMonthEndProcess loanProc = new LoansBeforeMonthEndProcess(getLoginCredential(), _logger);
            var flg = false;

            StringBuilder sbqry = new StringBuilder();

            sbqry.Append(GenNewTransactionString());
            int loan_id = 0;
            double instAmt = 0;
            bool bRet = false;

            string qry = "";

            string qrygetinstallment_amount = "select distinct dd_amount,ded.dd_mid from pr_emp_payslip psl " +
                                             " join pr_emp_payslip_deductions ded on psl.id = ded.payslip_mid " +
                                             " where psl.emp_code=" + EmpCode + " AND ded.dd_type='loan'" + " AND ded.dd_mid=" + emp_adv_loans_mid + " AND month(psl.fm)=" + month + " and ded.dd_amount>0 AND ded.active=1 and year(psl.fm)=" + year;

            DataSet dsfestlonas = await _sha.Get_MultiTables_FromQry(qrygetinstallment_amount);


            DataTable dtddamount = dsfestlonas.Tables[0];

            if (dtddamount.Rows.Count > 0)
            {
                foreach (DataRow elnddss in dtddamount.Rows)
                {
                    loan_id = int.Parse(elnddss["dd_mid"].ToString());
                    instAmt = Convert.ToDouble(elnddss["dd_amount"].ToString());

                    //test loans payments            
                    flg = loanProc.InstallmentPartPaymentsBeforeMonthend(loan_id, instAmt, 'I').GetAwaiter().GetResult();
                }
            }

            else
            {
                flg = loanProc.InstallmentPartPaymentsBeforeMonthend(emp_adv_loans_mid, instAmt, 'I').GetAwaiter().GetResult();
            }
            try
            {
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                //_logger.Info(EmpCode + "Process Sucessfully" + emp_adv_loans_mid);
                _logger.Info(EmpCode + "Process Sucessfully");
            }
            catch (Exception ex)
            {
                _logger.Info(EmpCode + " " + ex);
                _logger.Info(sbqry.ToString());
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);

                bRet = false;
            }
            return bRet;
        }
        private static LoginCredential getLoginCredential()
        {
            //string qryfm = " select fm from pr_month_details where active=1";

            return new LoginCredential
            {
                AppName = "PR Service",
                AppVersion = "1.0.0",
                AppEnvironment = "Dev",
                FinancialMonthDate = DateTime.Now,
                FY = 2020,
                EmpCode = 0,
                EmpShortName = "Service"
            };
        }
        //2.Interest with Equal Installments

        private async Task<bool> UpadateinstallmentsWithEqualinterest(int EmpCode, int emp_adv_loans_mid, int emp_adv_loans_child_mid, int loanid, int Month, int Year)
        {

            bool bRet = false;
            StringBuilder sbqry = null;

            int NewNumIndex = 0;
            string qry = "";

            string qrygetLoans = "select chi.loan_amount,chi.os_principal_amount,chi.os_this_month_interest,chi.os_interest_amount,chi.principal_recovered_flag,chi.date_disburse,chi.os_interest_amount,chi.principal_amount_recovered,chi.interest_accured,chi.interest_rate,chi.priority,chi.interest_amount_recovered,chi.interest_rate,chi.total_amount_recovered,chi.os_total_amount,adv.installment_amount,adv.completed_installment,adv.remaining_installment,adv.installment_start_date,adv.principal_installment,adv.interest_installment,adv.total_installment,adv.sanction_date,adv.installment_start_date from pr_emp_adv_loans adv" +
                             " join pr_emp_adv_loans_child chi on adv.id =" + emp_adv_loans_mid +
                             " where adv.active = 1 AND adv.emp_code =" + EmpCode + " AND chi.id =" + emp_adv_loans_child_mid;

            string qrygetinstallment_amount = "select dd_amount from pr_emp_payslip psl " +
                                              " join pr_emp_payslip_deductions ded on psl.id = ded.payslip_mid " +
                                              " where psl.emp_code=" + EmpCode + " AND ded.dd_type='loan'" + " AND ded.dd_mid=" + loanid + " AND month(psl.fm)=" + Month + " AND year(psl.fm)=" + Year;

            string qryfm = "select fm,fy from pr_month_details where active=1";

            DataSet dtloans = await _sha.Get_MultiTables_FromQry(qrygetLoans + qrygetinstallment_amount + qryfm);

            DataTable dtloansdata = dtloans.Tables[0];
            DataTable dtdd_amount = dtloans.Tables[1];
            DataTable dtfm = dtloans.Tables[2];

            if (dtloansdata.Rows.Count > 0 && dtdd_amount.Rows.Count > 0)
            {
                foreach (DataRow drinstall in dtloansdata.Rows)
                {
                    sbqry = new StringBuilder();


                    //installments calculation
                    int installmentamount = Convert.ToInt32(dtdd_amount.Rows[0]["dd_amount"].ToString());
                    int completed_installments = Convert.ToInt32(drinstall["completed_installment"].ToString());
                    int remaining_installments = Convert.ToInt32(drinstall["remaining_installment"].ToString());
                    int no_of_installments_cover = 1;
                    int total_installments = Convert.ToInt32(drinstall["total_installment"].ToString());
                    completed_installments = completed_installments + no_of_installments_cover;
                    remaining_installments = total_installments - completed_installments;

                    int os_principal_amount = Convert.ToInt32(drinstall["os_principal_amount"].ToString());
                    int os_total_amount = Convert.ToInt32(drinstall["os_total_amount"].ToString());
                    int loan_amount = Convert.ToInt32(drinstall["loan_amount"].ToString());
                    int total_amount_recovered = Convert.ToInt32(drinstall["total_amount_recovered"].ToString());

                    //principal amount
                    int principal_amount_recovered = Convert.ToInt32(drinstall["principal_amount_recovered"].ToString());
                    int principal_open_amount = os_principal_amount;
                    int principal_balance_amount = 0;



                    //interest amount
                    int interest_installment = Convert.ToInt32(drinstall["interest_installment"].ToString());
                    int interest_accured = Convert.ToInt32(drinstall["interest_accured"].ToString());
                    double interest_rate = Convert.ToDouble(drinstall["interest_rate"].ToString());
                    int os_interest_amount = Convert.ToInt32(drinstall["os_interest_amount"].ToString());
                    int interest_amount_recovered = Convert.ToInt32(drinstall["interest_amount_recovered"].ToString());


                    //Payments Dates
                    DateTime paidDate = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());
                    DateTime dtAmountDisburse = Convert.ToDateTime(drinstall["date_disburse"].ToString());
                    string installments_paid_date = paidDate.ToString("yyy-MM-dd");
                    string Cash_paid_on = paidDate.ToString("yyy-MM-dd");
                    string date_disburse = dtAmountDisburse.ToString("yyyy-MM-dd");

                    int fy = Convert.ToInt32(dtfm.Rows[0]["fy"].ToString());


                    //PaymentMode

                    // name change salary to instalment
                    string payment_type = "installment";
                    string payment_mode = "installment";


                    //trans_id
                    sbqry.Append(GenNewTransactionString());

                    int days = 0;
                    int InterestAccured = 0;
                    int thmonth_installment_amount = 0;

                    if (total_amount_recovered == 0)
                    {
                        DateTime nextmonth = paidDate.AddMonths(1).AddDays(-1);

                        days = GetInterestdisbursetoFinanceMonth(dtAmountDisburse, nextmonth);

                        InterestAccured = int.Parse(Math.Round(((loan_amount * interest_rate) / (100 * 365)) * days).ToString());
                        interest_accured += InterestAccured;

                        principal_open_amount = loan_amount;

                        if (installmentamount > InterestAccured)
                        {
                            thmonth_installment_amount = (installmentamount - InterestAccured);
                        }
                        else
                        {
                            thmonth_installment_amount = principal_open_amount - (installmentamount + InterestAccured);
                        }

                        principal_balance_amount = principal_open_amount - thmonth_installment_amount;
                        principal_amount_recovered = thmonth_installment_amount;
                        interest_amount_recovered = InterestAccured;
                        os_principal_amount = principal_balance_amount;

                    }
                    else
                    {

                        days = Helper.findLastDayOfMonth(paidDate);
                        InterestAccured = int.Parse(Math.Round(((os_principal_amount * interest_rate) / (100 * 365)) * days).ToString());

                        if (installmentamount > InterestAccured)
                        {
                            thmonth_installment_amount = (installmentamount - InterestAccured);
                        }
                        else
                        {
                            thmonth_installment_amount = principal_open_amount - (installmentamount + InterestAccured);
                        }

                        principal_balance_amount = os_principal_amount - thmonth_installment_amount;
                        principal_amount_recovered = thmonth_installment_amount;
                        interest_amount_recovered = InterestAccured;
                        os_principal_amount = principal_balance_amount;
                        interest_accured += InterestAccured;


                    }

                    int interest_paid_amount = InterestAccured;

                    NewNumIndex++;

                    //1. qry---insert into pr_emp_adv_loans_adjustments
                    sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments", NewNumIndex));


                    qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[installments_paid],[installments_amount],[installments_paid_date],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id],[fm],[fy]) values "
                           + "(@idnew" + NewNumIndex + "," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + thmonth_installment_amount + "," + principal_balance_amount + "," + InterestAccured + "," + 0 + "," + interest_paid_amount + "," + 0 + "," + completed_installments + "," + installmentamount + ",'" + installments_paid_date + "','" + payment_type + "','" + payment_mode + "', '" + installments_paid_date + "'," + no_of_installments_cover + "," + installmentamount + ",1, @transidnew,'" + installments_paid_date + "'," + fy + ");";
                    sbqry.Append(qry);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew" + NewNumIndex, ""));


                    //2. qry update pr_emp_adv_loans_child table



                    total_amount_recovered = completed_installments * installmentamount;
                    principal_amount_recovered = (total_amount_recovered - interest_accured);

                    if (total_amount_recovered <= loan_amount)
                    {
                        qry = "update pr_emp_adv_loans_child SET interest_accured=" + interest_accured + ",os_interest_amount=" + 0 + " ,principal_amount_recovered=" + principal_amount_recovered + ", total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + ",interest_amount_recovered=" + interest_accured + " where id=" + emp_adv_loans_child_mid + " AND priority=1 AND active=1;";
                        sbqry.Append(qry);

                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));
                    }
                    else
                    {

                        qry = "update pr_emp_adv_loans_child SET interest_accured=" + interest_accured + ",os_interest_amount=" + 0 + " , principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + ",interest_amount_recovered=" + interest_accured + ",active=0 " + ",principal_recovered_flag=1" + " where id=" + emp_adv_loans_child_mid + " AND priority=1;";
                        sbqry.Append(qry);

                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), installmentamount.ToString()));

                    }

                    //3.qry update pr_emp_adv_loans table
                    if (total_amount_recovered <= loan_amount)
                    {
                        qry = "update pr_emp_adv_loans SET completed_installment=" + completed_installments + ",remaining_installment=" + remaining_installments + " ,total_recovered_amount=" + total_amount_recovered + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                        sbqry.Append(qry);

                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), EmpCode.ToString()));
                    }
                    else
                    {
                        qry = "update pr_emp_adv_loans SET completed_installment=" + completed_installments + ",remaining_installment=" + remaining_installments + " ,total_recovered_amount=" + total_amount_recovered + ",active=0 " + " where id=" + emp_adv_loans_mid + " AND emp_code=" + EmpCode + ";";
                        sbqry.Append(qry);

                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", emp_adv_loans_mid.ToString(), EmpCode.ToString()));
                    }

                }
                try
                {
                    bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

                }
                catch (Exception ex)
                {
                    _logger.Info(sbqry.ToString());
                    _logger.Error(ex.Message);
                    _logger.Error(ex.StackTrace);

                    bRet = false;
                }

            }

            return bRet;
        }


        #region // Interest And PartPayment
        public async Task<bool> UpdateInterestAndPartPayment()
        {
            bool bRet = false;

            string qry = "";
            StringBuilder sbqry = null;
            try
            {
                //qry for all emps        
                string getAllEmployee = "SELECT distinct el.emp_code, el.id, el.loan_type_mid " +
                    "FROM Employees e JOIN pr_emp_adv_loans el ON e.EmpId = el.emp_code " +
                    "WHERE el.active = 1  AND el.loan_type_mid NOT IN (SELECT id FROM pr_loan_master WHERE loan_id IN ('PFL1','PFL2','FEST'))";
                //" AND el.emp_code=6305";


                string qryfm = " select fy,fm from pr_month_details where active=1";

                DataSet dtEmployees = await _sha.Get_MultiTables_FromQry(getAllEmployee + qryfm);

                DataTable dtEmployeesdata = dtEmployees.Tables[0];
                DataTable dtFm = dtEmployees.Tables[1];

                foreach (DataRow drEmp in dtEmployeesdata.Rows)
                {
                    int Empcode = Convert.ToInt32(drEmp["emp_code"].ToString());
                    int loan_mid = Convert.ToInt32(drEmp["id"].ToString());

                    string getsubloansdata = "select child.loan_amount,child.date_disburse,child.id as emp_adv_loanchild_mid,interest_accured,principal_amount_recovered,child.total_amount_recovered,child.interest_rate,child.loan_sl_no from pr_emp_adv_loans_child child where emp_adv_loans_mid=" + loan_mid + " AND active=1 AND interest_recovered_flag =0";

                    DataTable dtsubloans = await _sha.Get_Table_FromQry(getsubloansdata);

                    //Fiance Month
                    DateTime dtfm = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                    int Month = dtfm.Month;
                    int Year = dtfm.Year;
                    int loanslno = 0;
                    sbqry = new StringBuilder();

                    //SubLoans Interest Calculation
                    if (dtsubloans.Rows.Count == 2)
                    {
                        foreach (DataRow drinteres in dtsubloans.Rows)
                        {
                            int total_amount_recovered = Convert.ToInt32(drinteres["total_amount_recovered"].ToString());
                            int loan_amount = Convert.ToInt32(drinteres["loan_amount"].ToString());
                            int emp_adv_loan_child_mid = Convert.ToInt32(drinteres["emp_adv_loanchild_mid"].ToString());
                            loanslno = Convert.ToInt32(drinteres["loan_sl_no"].ToString());
                            if (total_amount_recovered != loan_amount)
                            {
                                bRet = UpdateSubLoansForPrincipalamountInterestAmount(loan_mid, emp_adv_loan_child_mid, Month, Year, dtfm, loanslno).GetAwaiter().GetResult();
                            }
                        }
                    }
                    else
                    {
                        int emp_adv_loan_child_mid = int.Parse(dtsubloans.Rows[0]["emp_adv_loanchild_mid"].ToString());

                        bRet = UpdateSubLoansForPrincipalamountInterestAmount(loan_mid, emp_adv_loan_child_mid, Month, Year, dtfm, loanslno).GetAwaiter().GetResult();
                    }

                }

            }
            catch (Exception e)
            {

            }
            return bRet;
        }

        //sub Loan Interest  Calculation
        private async Task<bool> UpdateSubLoansForPrincipalamountInterestAmount(int loan_mid, int emp_adv_loan_child_mid, int Month, int Year, DateTime date, int loansno)
        {
            bool bRet = false;
            string qry = "";
            StringBuilder sbqry = null;
            int princ_op = 0;
            int princ_op_after_pp = 0;
            int installment_intrest_days = 0;
            int partpay_intrest_days = 0;
            int InterestAccured = 0;
            string updqry1 = "";
            string updqry2 = "";
            sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());

            string getsubloans = "SELECT adj.principal_open_amount,adj.principal_paid_amount,adj.principal_balance_amount,adj.amount_paid,adj.payment_type, adj.cash_paid_on,chi.id as loans_child_mid,chi.interest_rate,chi.principal_start_date,chi.loan_amount,chi.date_disburse,chi.total_amount_recovered, chi.os_interest_amount,adj.id as adjustmentsid,chi.interest_accured as child_interest_accured,adj.interest_accured as adj_interest_accured from  pr_emp_adv_loans_child chi" +
                               " join pr_emp_adv_loans_adjustments adj on chi.id = adj.emp_adv_loans_child_mid" +
                               " WHERE chi.active=1 AND chi.principal_recovered_flag = 0  AND chi.emp_adv_loans_mid =" + loan_mid + " AND chi.id=" + emp_adv_loan_child_mid + " AND month(adj.cash_paid_on)=" + Month + " AND year(adj.cash_paid_on)=" + Year + " order by principal_open_amount desc";

            DataTable dtLoansdata = await _sha.Get_Table_FromQry(getsubloans);

            if (dtLoansdata.Rows.Count > 0)
            {
                princ_op = int.Parse(dtLoansdata.Rows[0]["principal_open_amount"].ToString());
                int emp_adv_loans_child_mid = int.Parse(dtLoansdata.Rows[0]["loans_child_mid"].ToString());
                int loan_amount = int.Parse(dtLoansdata.Rows[0]["loan_amount"].ToString());


                string part_pay_date = "";
                try
                {
                    part_pay_date = dtLoansdata.Rows.Cast<DataRow>()
                        .Where(x => x["payment_type"].ToString() == "Part Payment")
                        .Select(x => x["cash_paid_on"].ToString()).FirstOrDefault();

                    if (part_pay_date != null)
                    {
                        DateTime dtPp = DateTime.Parse(part_pay_date);

                        installment_intrest_days = dtPp.Day;
                        partpay_intrest_days = Helper.findLastDayOfMonth(dtPp) - installment_intrest_days;
                        princ_op_after_pp = int.Parse(dtLoansdata.Rows[0]["principal_open_amount"].ToString());
                    }
                }
                catch
                {
                }

                if (part_pay_date != "" && part_pay_date != null) //part payment and installment
                {
                    DataRow dr = dtLoansdata.Rows[0];
                    int install_pkid = 0;
                    int pp_pkid = 0;

                    if (dr["payment_type"].ToString() == "Part Payment")
                    {
                        pp_pkid = int.Parse(dtLoansdata.Rows[0]["adjustmentsid"].ToString());
                    }

                    install_pkid = int.Parse(dtLoansdata.Rows[0]["adjustmentsid"].ToString());


                    double interest_rate = Convert.ToDouble(dr["interest_rate"].ToString());
                    int InterestAccured_installment = int.Parse(Math.Round(((princ_op * interest_rate) / (100 * 365)) * installment_intrest_days).ToString());
                    int InterestAccured_pp = int.Parse(Math.Round(((princ_op_after_pp * interest_rate) / (100 * 365)) * partpay_intrest_days).ToString());
                    int InterestAccured_child = Convert.ToInt32(dr["child_interest_accured"].ToString());
                    InterestAccured = InterestAccured_installment + InterestAccured_pp;
                    InterestAccured_child += InterestAccured;
                    int interest_balance_amount_pp = InterestAccured_installment;
                    int interest_balance_amount = InterestAccured_pp;

                    int interest_open_amount = Convert.ToInt32(dr["os_interest_amount"].ToString());

                    //1. Update pr_emp_adv_loans_adjustments for interest_accured
                    updqry1 = "update pr_emp_adv_loans_adjustments SET interest_accured=" + InterestAccured_installment + ",interest_open_amount=" + interest_open_amount + ",interest_balance_amount =" + interest_balance_amount + "   where id=" + pp_pkid + " and emp_adv_loans_child_mid=" + emp_adv_loans_child_mid + ";";
                    sbqry.Append(updqry1);
                    updqry2 = "update pr_emp_adv_loans_adjustments SET interest_accured=" + InterestAccured_pp + ",interest_open_amount=" + interest_open_amount + ",interest_balance_amount =" + interest_balance_amount_pp + "   where id=" + install_pkid + "  and emp_adv_loans_child_mid=" + emp_adv_loans_child_mid + ";";
                    sbqry.Append(updqry2);

                    //2.Update pr_emp_adv_loans_child for interest_accured
                    qry = "update pr_emp_adv_loans_child SET interest_accured=" + InterestAccured_child + ",os_interest_amount=" + InterestAccured_child + ",os_this_month_interest =" + 0 + " where id=" + emp_adv_loans_child_mid + " AND emp_adv_loans_mid=" + loan_mid + ";";
                    sbqry.Append(qry);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), loan_mid.ToString()));

                }
                else if (part_pay_date == "" || part_pay_date == null) //no part payment, only installment
                {
                    DateTime dtInstalmentPaid = DateTime.Parse(dtLoansdata.Rows[0]["cash_paid_on"].ToString());

                    installment_intrest_days = Helper.findLastDayOfMonth(dtInstalmentPaid);
                    DataRow dr = dtLoansdata.Rows[0];

                    int adjustmentsid = Convert.ToInt32(dr["adjustmentsid"].ToString());

                    double interest_rate = Convert.ToDouble(dr["interest_rate"].ToString());
                    int InterestAccured_child = Convert.ToInt32(dr["child_interest_accured"].ToString());

                    if (princ_op == loan_amount)
                    {
                        DateTime date_disburse = DateTime.Parse(dr["date_disburse"].ToString());

                        //Date of Disburse to Finance month intrest 
                        DateTime nextmonth = date.AddMonths(1).AddDays(-1);

                        installment_intrest_days = GetInterestdisbursetoFinanceMonth(date_disburse, nextmonth);

                        InterestAccured = int.Parse(Math.Round(((princ_op * interest_rate) / (100 * 365)) * installment_intrest_days).ToString());

                        InterestAccured_child += InterestAccured;
                    }
                    else
                    {
                        InterestAccured = int.Parse(Math.Round(((princ_op * interest_rate) / (100 * 365)) * installment_intrest_days).ToString());

                        InterestAccured_child += InterestAccured;
                    }

                    int interest_balance_amount = InterestAccured_child;
                    int interest_open_amount = Convert.ToInt32(dr["os_interest_amount"].ToString());

                    //1. Update pr_emp_adv_loans_adjustments for interest_accured
                    updqry1 = "update pr_emp_adv_loans_adjustments SET interest_accured=" + InterestAccured + ",interest_open_amount =" + interest_open_amount + ",interest_balance_amount =" + interest_balance_amount + " where id=" + adjustmentsid + " and emp_adv_loans_child_mid=" + emp_adv_loans_child_mid + " ;";
                    sbqry.Append(updqry1);

                    //2.Update pr_emp_adv_loans_child for interest_accured
                    qry = "update pr_emp_adv_loans_child SET interest_accured=" + InterestAccured_child + ",os_interest_amount=" + InterestAccured_child + ",os_this_month_interest =" + 0 + " where id=" + emp_adv_loans_child_mid + " AND emp_adv_loans_mid=" + loan_mid + ";";
                    sbqry.Append(qry);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loans_child_mid.ToString(), loan_mid.ToString()));

                }
            }
            else
            {
                string qrygetdata = "Select child.loan_amount,child.interest_accured,child.interest_rate from pr_emp_adv_loans_child child  where child.id=" + emp_adv_loan_child_mid + " AND child.emp_adv_loans_mid=" + loan_mid + " AND child.principal_recovered_flag = 0 ";

                DataTable dtchilddata = await _sha.Get_Table_FromQry(qrygetdata);


                if (dtchilddata.Rows.Count > 0)
                {
                    // DateTime date_disburse = DateTime.Parse(dtchilddata.Rows[0]["date_disburse"].ToString());
                    double interest_rate = Convert.ToDouble(dtchilddata.Rows[0]["interest_rate"].ToString());
                    int InterestAccured_child = int.Parse(dtchilddata.Rows[0]["interest_accured"].ToString());
                    int loan_amount = int.Parse(dtchilddata.Rows[0]["loan_amount"].ToString());

                    //Date of Disburse to Finance month intrest 
                    DateTime nextmonth = date.AddMonths(1).AddDays(-1);

                    installment_intrest_days = Helper.findLastDayOfMonth(nextmonth);

                    InterestAccured = int.Parse(Math.Round(((loan_amount * interest_rate) / (100 * 365)) * installment_intrest_days).ToString());

                    InterestAccured_child += InterestAccured;

                    //2.Update pr_emp_adv_loans_child for interest_accured
                    qry = "update pr_emp_adv_loans_child SET interest_accured=" + InterestAccured_child + ",os_interest_amount=" + InterestAccured_child + ",os_this_month_interest =" + 0 + " where id=" + emp_adv_loan_child_mid + " AND emp_adv_loans_mid=" + loan_mid + ";";
                    sbqry.Append(qry);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", emp_adv_loan_child_mid.ToString(), loan_mid.ToString()));

                }
            }
            try
            {
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());

            }
            catch (Exception ex)
            {
                _logger.Info(sbqry.ToString());
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);

                bRet = false;
            }

            return bRet;
        }


        private int GetInterestdisbursetoFinanceMonth(DateTime date, DateTime fianceDate)
        {
            int days = 0;
            days = Convert.ToInt32((fianceDate - date).TotalDays);

            return days;

        }
        #endregion

        public async Task<bool> ChangeAttendanceProcess()
        {

            bool bRet = false;
            string inQry = "";
            string qryUpdate = "";
            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());

            string qryGetAttendance = "select distinct emp_code,emp_id,status,status_date,lop_days, working_days,sus_per from pr_month_attendance WHERE Active=1";

            string getfm = "select fy,fm ,month_days from pr_month_details where Active=1";

            //var qry = " SELECT l.id,e.EmpId,CasualLeave,MedicalSickLeave,PrivilegeLeave, MaternityLeave,PaternityLeave,ExtraordinaryLeave,SpecialCasualLeave,CompensatoryOff,LOP FROM V_EmpLeaveBalance l join employees e on l.empid = e.id  "; 




            DataSet dtAttendance = await _sha.Get_MultiTables_FromQry(qryGetAttendance + getfm);

            DataTable dtempcodes = dtAttendance.Tables[0];
            DataTable dtfm = dtAttendance.Tables[1];
            //DataTable dtleaves = dtAttendance.Tables[2];


            //Fiance month
            DateTime fmdate = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());
            int FY = Convert.ToInt32(dtfm.Rows[0]["fy"].ToString());
            string FM = fmdate.ToString("yyyy-MM-dd");


            int NewNumIndex = 0;

            if (dtempcodes.Rows.Count > 0)
            {
                foreach (DataRow drAttendance in dtempcodes.Rows)
                {
                    int emp = Convert.ToInt32(drAttendance["emp_code"].ToString());
                    var qry = " SELECT l.id,e.EmpId,CasualLeave,MedicalSickLeave,PrivilegeLeave, MaternityLeave,PaternityLeave,ExtraordinaryLeave,SpecialCasualLeave,CompensatoryOff,LOP FROM V_EmpLeaveBalance l join employees e on l.empid = e.id where e.EmpId=" + emp + " ";
                    DataTable dtleaves = await _sha.Get_Table_FromQry(qry);
                    foreach (DataRow drleaves in dtleaves.Rows)
                    {
                        int emp1 = Convert.ToInt32(drleaves["EmpId"].ToString());

                        if (emp == emp1)
                        {
                            string retStr = "";
                            retStr = retStr + "CL" + "#" + drleaves["CasualLeave"].ToString() + ',' + "ML" + "#" + drleaves["MedicalSickLeave"].ToString() + ',' + "PL" + "#" + drleaves["PrivilegeLeave"].ToString() + ',' + "MTL" + "#" + drleaves["MaternityLeave"].ToString() + ',' + "PTL" + "#" + drleaves["PaternityLeave"].ToString() + ',' + "EOL" + "#" + drleaves["ExtraordinaryLeave"].ToString() + ',' + "SCL" + "#" + drleaves["SpecialCasualLeave"].ToString() + ',' + "C-OFF" + "#" + drleaves["CompensatoryOff"].ToString() + ',' + "LOP" + "#" + drleaves["LOP"].ToString() + ",";

                            int emp_code = Convert.ToInt32(drAttendance["emp_code"].ToString());

                            int emp_id = Convert.ToInt32(drAttendance["emp_id"].ToString());

                            int working_days = Convert.ToInt32(dtfm.Rows[0]["month_days"].ToString());

                            string Status = "";
                            string c_status_date = null;
                            float suspend_per = 0.0f;
                            // string sus = drAttendance["sus_per"].ToString();

                            if (drAttendance["sus_per"] != null && drAttendance["sus_per"].ToString() != "")
                            {
                                double x = Convert.ToDouble(drAttendance["sus_per"].ToString());
                                suspend_per = (float)x;
                            }


                            if (!string.IsNullOrWhiteSpace(drAttendance["status"].ToString()))
                            {
                                Status = drAttendance["status"].ToString();

                            }
                            if (!string.IsNullOrWhiteSpace(drAttendance["status_date"].ToString()))
                            {
                                DateTime dtstatus_date = Convert.ToDateTime(drAttendance["status_date"].ToString());
                                c_status_date = dtstatus_date.ToString("yyyy-MM-dd");
                            }

                            //2. gen new num
                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_month_attendance", NewNumIndex));
                            //query
                            //for update records
                            qryUpdate = "update pr_month_attendance set active = 0 where emp_code=" + emp_code + ";";
                            sbqry.Append(qryUpdate);

                            if (c_status_date != null && Status != "Suspended")
                            {
                                inQry = "Insert into pr_month_attendance(id,fy,fm,emp_id,emp_code,status,status_date,leaves_available,lop_days,absent_days,working_days,active,[trans_id])" +
                                 "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + emp_id + "," + emp_code + ",'" + Status + "','" + c_status_date + "','" + retStr + "'," + 0 + "," + 0 + "," + working_days + ",1, @transidnew);";
                            }
                            else if (Status == "Suspended" && c_status_date != null)
                            {
                                inQry = "Insert into pr_month_attendance(id,fy,fm,emp_id,emp_code,status,status_date,leaves_available,lop_days,absent_days,working_days,active,[trans_id],sus_per)" +
                                 "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + emp_id + "," + emp_code + ",'" + Status + "','" + c_status_date + "','" + retStr + "'," + 0 + "," + 0 + "," + working_days + ",1, @transidnew," + suspend_per + ");";
                            }
                            else
                            {
                                inQry = "Insert into pr_month_attendance(id,fy,fm,emp_id,emp_code,status,leaves_available,lop_days,absent_days,working_days,active,[trans_id])" +
                               "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + emp_id + "," + emp_code + ",'" + Status + "','" + retStr + "'," + 0 + "," + 0 + "," + working_days + ",1, @transidnew);";
                            }
                            sbqry.Append(inQry);
                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_attendance", "@idnew" + NewNumIndex, ""));
                        }
                    }
                }
            }

            try
            {
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
            }
            catch (Exception ex)
            {
                _logger.Info(sbqry.ToString());
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                bRet = false;
            }



            return bRet;
        }

        #region //OB_Share Update
        public async Task<bool> UpdateOB_Share()
        {
            bool bRet = false;
            string inQry = "";
            string qryUpdate = "";
            string qryvpfdd_amount=""; DataTable vpf_dd_amount;//newly added on 22/05/2020
            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());


            //string qrygetVpfdata1 = "select epay.emp_code,epay.emp_id,epay.dd_provident_fund, epay.gross_amount,epay.id as payslip_mid from pr_emp_payslip epay " +
            //                     " WHERE (Month(epay.fm) =(select Month(fm) from pr_month_details where Active=1))  AND (Year(epay.fm)=(select Year(fm) from pr_month_details where Active=1))";
            string qrygetVpfdata = "select epay.emp_code,epay.id as payslip_mid,epay.emp_id,count(epay.emp_code),sum(distinct epay.dd_provident_fund) as dd_provident_fund ,sum( distinct epay.gross_amount) as gross_amount ,case  when sum(paydedu.dd_amount) > 0.00 then sum(paydedu.dd_amount)  else 0.00 end as dd_amount " +
                " from pr_emp_payslip epay  Inner join pr_emp_payslip_deductions paydedu on epay.emp_code = paydedu.emp_code " +
                " WHERE(Month(epay.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(epay.fm) = (select Year(fm) from pr_month_details where Active = 1)) " +
                " and epay.spl_type='Regular' group by epay.emp_code,epay.emp_id ,epay.id ";//added n
                                                                                                                                  //new added "and epay.spl_type='Regular'" in the above line //chaitanya on 4/5/2020
            string qrygetVpfdataencashdata = "select epay.emp_code,epay.id as payslip_mid,epay.emp_id,count(epay.emp_code),sum(distinct epay.dd_provident_fund) as dd_provident_fund ,sum( distinct epay.gross_amount) as gross_amount ,case  when sum(paydedu.dd_amount) > 0.00 then sum(paydedu.dd_amount)  else 0.00 end as dd_amount " +
                           " from pr_emp_payslip epay  Inner join pr_emp_payslip_deductions paydedu on epay.emp_code = paydedu.emp_code " +
                           " WHERE(Month(epay.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(epay.fm) = (select Year(fm) from pr_month_details where Active = 1)) " +
                           " AND paydedu.dd_name = 'VPF Deduction' and epay.spl_type='Encashment' group by epay.emp_code,epay.emp_id ,epay.id ";//added n

            string qrygetVpfdataAdhocdata = "select epay.emp_code,epay.id as payslip_mid,epay.emp_id,count(epay.emp_code),sum(distinct epay.dd_provident_fund) as dd_provident_fund ,sum( distinct epay.gross_amount) as gross_amount ,case  when sum(paydedu.dd_amount) > 0.00 then sum(paydedu.dd_amount)  else 0.00 end as dd_amount " +
               " from pr_emp_payslip epay  Inner join pr_emp_payslip_deductions paydedu on epay.emp_code = paydedu.emp_code " +
               " WHERE(Month(epay.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(epay.fm) = (select Year(fm) from pr_month_details where Active = 1)) " +
               " AND paydedu.dd_name = 'VPF Deduction' and epay.spl_type='Adhoc' group by epay.emp_code,epay.emp_id ,epay.id ";//added n

            string qrygetfm = "select fm,fy from pr_month_details where Active=1";

            
            DataSet dtPF = await _sha.Get_MultiTables_FromQry(qrygetVpfdata + qrygetfm+ qrygetVpfdataencashdata+ qrygetVpfdataAdhocdata);


            DataTable dtPFData = dtPF.Tables[0];
            DataTable dtfm = dtPF.Tables[1];
            DataTable dtPFEncashData = dtPF.Tables[2];
            DataTable dtPFAdhocData = dtPF.Tables[3];
            DateTime fmdate = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());
            int FY = Convert.ToInt32(dtfm.Rows[0]["fy"].ToString());
            string FM = fmdate.ToString("yyyy-MM-dd");

            

            // Last Month
            DateTime fmlastMonth = fmdate.AddMonths(-1);

            string FMLastMonth = fmlastMonth.ToString("yyyy-MM-dd");

            string[] sa1 = FMLastMonth.Split('-');
            int month = Convert.ToInt32(sa1[1].ToString());

            //Last Year
            int fy = Convert.ToInt32(sa1[0].ToString());

            int NewNumIndex = 0;

            if (dtPFData.Rows.Count > 0)
            {
                foreach (DataRow drFP in dtPFData.Rows)
                {
                    int Emp_code = Convert.ToInt32(drFP["emp_code"].ToString());
                    //int Emp_code = 836;
                    int Emp_id = Convert.ToInt32(drFP["emp_id"].ToString());
                    int payslip_mid = int.Parse(drFP["payslip_mid"].ToString());

                    //newly added on 16/05/2020
                    qryvpfdd_amount = "select dd_amount from pr_emp_payslip_deductions where payslip_mid=(select id from pr_emp_payslip where emp_code=" + Emp_code + " and year(fm)=(select Year(fm) from pr_month_details where Active = 1) and month(fm)=(select Month(fm) from pr_month_details where Active = 1) and spl_type='Regular' ) AND dd_name = 'VPF Deduction'";
                    vpf_dd_amount = await _sha.Get_Table_FromQry(qryvpfdd_amount);
                    //end

                    string VPFdata = "SELECT paydedu.dd_amount from pr_emp_payslip_deductions paydedu where paydedu.payslip_mid =" + payslip_mid + " AND paydedu.dd_name = 'VPF Deduction'";

                    // string GetLastmonthDetailes = " SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount,obshare.bank_share_intrst_open,obshare.own_share_intrst_open,obshare.vpf_intrst_open,obshare.own_share_intrst_total,obshare.bank_share_intrst_total,obshare.vpf_intrst_total from pr_ob_share obshare  " +
                        // " WHERE active=1 AND obshare.emp_code = " + Emp_code;
                    string GetLastmonthDetailes = "SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.vpf_intrst_total,obshare.own_share_intrst_total,obshare.bank_share_intrst_total from pr_ob_share obshare  WHERE (Month(obshare.fm) =" + month + ")  AND (Year(obshare.fm)=" +fy+ ") AND obshare.emp_code="+ Emp_code;

                    string exsitedemp = "SELECT obshare.id,obshare.emp_code,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount From pr_ob_share obshare  WHERE(Month(obshare.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(obshare.fm) = (select Year(fm) from pr_month_details where Active = 1)) and obshare.emp_code =" + Emp_code;

                    string qrybasicda = "select er_basic as basic,er_da as da from pr_emp_payslip where fm=(select fm from pr_month_details where active=1) and emp_code=" + Emp_code + ";"; ;

                    DataSet dtVPF_lastmonth = await _sha.Get_MultiTables_FromQry(GetLastmonthDetailes + exsitedemp+ qrybasicda);
                    DataTable dtbasicda = dtVPF_lastmonth.Tables[2];
                    double vpf_amount = 0;
                    double vpf_amount_intrest_amount = 0;

                    double c_own_share_open = 0;
                    double c_own_share_total = 0;
                    double c_bank_share_open = 0;
                    double c_bank_share_total = 0;
                    double c_vpf_open = 0;
                    double c_vpf_total = 0;

                    // TOTAL INTEREST CALCULATIONS RELATED NAMES ARE COMMENTED 18/19/19
                    // Interest 
                    //double own_share_intrst_amount = 0;
                    //double own_sahre_intrst_open = 0;
                    double own_share_intrst_total = 0;
                    //double vpf_intrst_open = 0;
                    double vpf_intrst_total = 0;
                    //double bank_share_intrst_amount = 0;
                    //double bank_share_intrst_open = 0;
                    double bank_share_intrst_total = 0;
                    //double c_own_share_intrst_amount = 0;
                    //double c_own_sahre_intrst_open = 0;
                    //double c_own_share_intrst_total = 0;
                    //double c_vpf_amount_intrest_amount = 0;
                    //double c_vpf_intrst_open = 0;
                    //double c_vpf_intrst_total = 0;
                    //double c_bank_share_intrst_amount = 0;
                    //double c_bank_share_intrst_open = 0;
                    //double c_bank_share_intrst_total = 0;


                    double own_share_open = 0;
                    double own_share_total = 0;
                    double bank_share_open = 0;
                    double bank_share_total = 0;
                    double vpf_open = 0;
                    double vpf_total = 0;

                    //newly added on 16/05/2020
                    string str_age;
                    DataTable dt_age;
                    //end

                    //  DataTable dtVPF = dtVPF_lastmonth.Tables[0];
                    DataTable dtVPF = dtPF.Tables[0];
                    DataTable dtLastmonthDetailes = dtVPF_lastmonth.Tables[0];
                    DataTable dtemp = dtVPF_lastmonth.Tables[1];
                    //DataTable dtemp = dtPF.Tables[0];
                    if (dtLastmonthDetailes.Rows.Count > 0)
                    {
                        try
                        {
                            //own_share_amounts
                            own_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_open"].ToString());
                            own_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_total"].ToString());

                            //bank_share_amounts
                            bank_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_open"].ToString());
                            bank_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_total"].ToString());


                            //vpf_amounts
                            vpf_open = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_open"].ToString());
                            vpf_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_total"].ToString());

                            //bank_share_interest
                            //bank_share_intrst_amount  = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_intrst_amount"].ToString());
                            //17/09/19 commented 
                            //if(!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["bank_share_intrst_open"].ToString()))
                            //{
                            //    bank_share_intrst_open = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_intrst_open"].ToString());
                            //}

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString()))
                            {
                                bank_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString());
                            }

                            //own_share_interest 
                            //own_share_intrst_amount  = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_intrst_amount"].ToString());

                            //17/09/19 commented 
                            //if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["own_share_intrst_open"].ToString()))
                            //{
                            //    own_sahre_intrst_open = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_intrst_open"].ToString());
                            //}
                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString()))
                            {
                                own_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString());
                            }

                            //    //vpf_interest
                            //    vpf_amount_intrest_amount = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_intrst_amount"].ToString());

                            //17/09/19 commented 
                            //if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["vpf_intrst_open"].ToString()))
                            //{
                            //    vpf_intrst_open = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_intrst_open"].ToString());
                            //}
                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString()))
                            {
                                vpf_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString());
                            }

                        }
                        catch (Exception e)
                        {
                            // added on 22/04/2020
                            _logger.Error(e.Message);
                            //

                        }
                    }

                    if (dtVPF.Rows.Count > 0)
                    {
                        //vpf_amount = double.Parse(dtVPF.Rows[0]["dd_amount"].ToString()); // this will take 1st row dd_amount value for all the employees
                        //vpf_amount = double.Parse(drFP["dd_amount"].ToString()); // newly added on 4/5/2020 by chaitanya 
                        if (vpf_dd_amount.Rows.Count>0)
                        {
                            vpf_amount = double.Parse(vpf_dd_amount.Rows[0]["dd_amount"].ToString()); // newly added on 22/5/2020 by chaitanya 
                            vpf_amount_intrest_amount = (vpf_amount * 8.5) / 100;
                        }
                        else
                        {
                            // do nothing
                        }
                        //vpf_amount = double.Parse(vpf_dd_amount.Rows[0]["dd_amount"].ToString()); // newly added on 22/5/2020 by chaitanya 
                        //vpf_amount_intrest_amount = (vpf_amount * 8.5) / 100;

                        //vpf_open and total amounts
                        c_vpf_open = vpf_total;

                        c_vpf_total = (c_vpf_open + vpf_amount);

                    }

                    double own_share_amount = Convert.ToDouble(drFP["dd_provident_fund"].ToString());
                    double gross_amount = Convert.ToDouble(drFP["gross_amount"].ToString());

                    double pension_amount = Math.Round((gross_amount * 8.33) / 100);
                    double pension_open = 0;
                    double pension_total = 0;
                    double basic = Convert.ToDouble(dtbasicda.Rows[0]["basic"]);
                    double da = Convert.ToDouble(dtbasicda.Rows[0]["da"]);

                    //newly added on 16/05/2020
                    //str_age = "SELECT EmpId,DOB,DOJ,RetirementDate,CASE WHEN dateadd(year, datediff (year, DOB, getdate()), DOB) > getdate()" +
                    //            "THEN datediff(year, DOB, getdate()) -1 " +
                    //            "ELSE datediff(year, DOB, getdate()) END as Age FROM Employees where Empid=" + Emp_code;
                    str_age = "select Empid,DOB,datediff(month,DOB,getdate()) as Age from Employees where Empid = " + Emp_code;
                    dt_age = await _sha.Get_Table_FromQry(str_age);
                    if (Convert.ToInt32(dt_age.Rows[0]["Age"]) <= 696)
                    {
                        if (pension_amount <= 1250)
                        {
                            pension_amount = ((basic + da) * 8.33) / 100;
                            pension_open = pension_amount;
                            pension_open = Math.Round(pension_open, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            pension_open = 1250;
                        }
                    }
                    else
                    {
                        pension_open = 0;
                    }
                    //end

                    //existing condition
                    //if (pension_amount <= 1250)
                    //{
                    //    pension_open = pension_amount;
                    //}
                    //else
                    //{
                    //    pension_open = 1250;

                    //}
                    //end

                    pension_total += pension_open;

                    double bank_share_amount = (own_share_amount - pension_open);

                    //1.Intrest amount
                    // double own_share_intrest_amount = (own_share_amount * 8.5) / 100;

                    //double bank_share_intrest_amount = (bank_share_amount * 8.5) / 100;

                    //own_share_open and Total _amounts
                    c_own_share_open = own_share_total;
                    c_own_share_total = (c_own_share_open + own_share_amount);

                    //// ownshare Interest

                    //    if (!string.IsNullOrWhiteSpace(dtemp.Rows[0]["own_share_intrst_amount"].ToString()))
                    //    {
                    //        c_own_share_intrst_amount = double.Parse(dtemp.Rows[0]["own_share_intrst_amount"].ToString());
                    //    }

                    //    //Bank_share Interest
                    //    if (!string.IsNullOrWhiteSpace(dtemp.Rows[0]["bank_share_intrst_amount"].ToString()))
                    //    {
                    //        c_bank_share_intrst_amount = double.Parse(dtemp.Rows[0]["bank_share_intrst_amount"].ToString());
                    //    }
                    //    //vpf Interest vpf_amount_intrest_amount
                    //    if (!string.IsNullOrWhiteSpace(dtemp.Rows[0]["vpf_intrst_amount"].ToString()))
                    //    {
                    //        c_vpf_amount_intrest_amount = double.Parse(dtemp.Rows[0]["vpf_intrst_amount"].ToString());
                    //    }



                    //c_own_sahre_intrst_open = own_share_intrst_total;

                    //c_own_share_intrst_total = (c_own_sahre_intrst_open + c_own_share_intrst_amount);


                    //c_bank_share_intrst_open = bank_share_intrst_total;
                    //c_bank_share_intrst_total = (c_bank_share_intrst_open + c_bank_share_intrst_amount);

                    //c_vpf_intrst_open = vpf_intrst_total;
                    //c_vpf_intrst_total = (c_vpf_intrst_open + c_vpf_amount_intrest_amount);
                    //Bank_Share_Open And Total _Amounts
                    c_bank_share_open = bank_share_total;
                    c_bank_share_total = (c_bank_share_open + bank_share_amount);

                    //2. gen new num
                    NewNumIndex++;

                    sbqry.Append(GetNewNumStringArr("pr_ob_share", NewNumIndex));
                    //query

                    //for update records
                    qryUpdate = "update pr_ob_share set active = 0 where emp_code=" + Emp_code + ";";
                    sbqry.Append(qryUpdate);

                    if (dtemp.Rows.Count > 0)
                    {
                        int c_id = int.Parse(dtemp.Rows[0]["id"].ToString());

                        //   inQry = "Update pr_ob_share set own_share=" + own_share_amount + ",own_share_intrst_amount=" + own_share_intrest_amount + " ,vpf =" + vpf_amount + " ,vpf_intrst_amount =" + vpf_amount_intrest_amount + " ,bank_share=" + bank_share_amount + ",bank_share_intrst_amount=" + bank_share_intrest_amount + " ,own_share_open =" + c_own_share_open + " ,own_share_total=" + c_own_share_total + " ,vpf_open=" + c_vpf_open + " ,vpf_total=" + c_vpf_total + " ,bank_share_open=" + c_bank_share_open + " ,bank_share_total=" + c_bank_share_total + ",pension_open="+ pension_open+ ",pension_total="+ pension_total + ",own_share_intrst_open="+ c_own_sahre_intrst_open + ",bank_share_intrst_open="+ c_bank_share_intrst_open + ", vpf_intrst_open="+ c_vpf_intrst_open + ",own_share_intrst_total="+ c_own_share_intrst_total + ",bank_share_intrst_total="+ c_bank_share_intrst_total + ",vpf_intrst_total="+ c_vpf_intrst_total + "  WHERE id=" + c_id + " AND emp_code=" + Emp_code + ";";

                        inQry = "Update pr_ob_share set own_share=" + own_share_amount + ",vpf =" + vpf_amount + " ,bank_share=" + bank_share_amount + ",own_share_open =" + c_own_share_open + " ,own_share_total=" + c_own_share_total + " ,vpf_open=" + c_vpf_open + " ,vpf_total=" + c_vpf_total + " ,bank_share_open=" + c_bank_share_open + " ,bank_share_total=" + c_bank_share_total + ",pension_open=" + pension_open + ",pension_total=" + pension_total + ",   own_share_intrst_total=" + own_share_intrst_total + ",bank_share_intrst_total=" + bank_share_intrst_total + ",vpf_intrst_total=" + vpf_intrst_total + "  WHERE id=" + c_id + " AND emp_code=" + Emp_code + ";";
                        _logger.Info("Record exist for Emp_code "+Emp_code+", So updating the record with set own_share = " + own_share_amount + ", vpf = " + vpf_amount + ", bank_share = " + bank_share_amount + ", own_share_open = " + c_own_share_open + ", own_share_total = " + c_own_share_total + ", vpf_open = " + c_vpf_open + ", vpf_total = " + c_vpf_total + ", bank_share_open = " + c_bank_share_open + ", bank_share_total = " + c_bank_share_total + ", pension_open = " + pension_open + ", pension_total = " + pension_total + ", own_share_intrst_total = " + own_share_intrst_total + ", bank_share_intrst_total = " + bank_share_intrst_total + ", vpf_intrst_total = " + vpf_intrst_total + "  WHERE id = " + c_id + " AND emp_code = " + Emp_code +"");
                    }
                    else
                    {
                        inQry = "Insert into pr_ob_share(id,fy,fm,emp_id,emp_code," +
                        "own_share,vpf,bank_share," +
                        "active,[trans_id],[own_share_open]," +
                        "[own_share_total],[vpf_open],[vpf_total]," +
                        "[bank_share_open]," + "[bank_share_total],[pension_open],[pension_total],[own_share_intrst_total],[bank_share_intrst_total],[vpf_intrst_total])" +
                        "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + Emp_id + "," + Emp_code + "," +
                        "" + own_share_amount + "," + vpf_amount + "," + bank_share_amount + "," +
                        "1, @transidnew " + "," + "" + c_own_share_open + "," +
                        "" + c_own_share_total + "," + c_vpf_open + " , " + c_vpf_total + " ," +
                        " " + c_bank_share_open + " , " + c_bank_share_total + "," + pension_open + "," + pension_total + "   ," + own_share_intrst_total + "," + bank_share_intrst_total + "," + vpf_intrst_total + ");";
                        
                        //********************** LOG FILE******************************
                        //newly added on 25/05/2020 to check the records in log file.
                        _logger.Info("Record inserting for Emp_Code: " + Emp_code + " in pr_ob_share table.");
                        _logger.Info("Insert into pr_ob_share(id,fy,fm,emp_id,emp_code," +
                            "own_share,vpf,bank_share," +
                            "active,[trans_id],[own_share_open]," +
                            "[own_share_total],[vpf_open],[vpf_total]," +
                            "[bank_share_open]," + "[bank_share_total],[pension_open],[pension_total],[own_share_intrst_total],[bank_share_intrst_total],[vpf_intrst_total])" +
                            "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + Emp_id + "," + Emp_code + "," +
                            "" + own_share_amount + "," + vpf_amount + "," + bank_share_amount + "," +
                            "1, @transidnew " + "," + "" + c_own_share_open + "," +
                            "" + c_own_share_total + "," + c_vpf_open + " , " + c_vpf_total + " ," +
                            " " + c_bank_share_open + " , " + c_bank_share_total + "," + pension_open + "," + pension_total + "   ," + own_share_intrst_total + "," + bank_share_intrst_total + "," + vpf_intrst_total + ");");
                        //********************** LOG FILE******************************
                        //end
                    }
                    sbqry.Append(inQry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_attendance", "@idnew" + NewNumIndex, ""));
                }
            }
            if (dtPFEncashData.Rows.Count > 0)
            {
                foreach (DataRow drFP in dtPFEncashData.Rows)
                {
                    int Emp_code = Convert.ToInt32(drFP["emp_code"].ToString());
                    //int Emp_code = 836;
                    int Emp_id = Convert.ToInt32(drFP["emp_id"].ToString());
                    int payslip_mid = int.Parse(drFP["payslip_mid"].ToString());

                    //newly added on 16/05/2020
                    qryvpfdd_amount = "select dd_amount from pr_emp_payslip_deductions where payslip_mid=(select id from pr_emp_payslip where emp_code=" + Emp_code + " and year(fm)=(select Year(fm) from pr_month_details where Active = 1) and month(fm)=(select Month(fm) from pr_month_details where Active = 1) and spl_type='Encashment' ) AND dd_name = 'VPF Deduction'";
                    vpf_dd_amount = await _sha.Get_Table_FromQry(qryvpfdd_amount);
                    //end

                    string VPFdata = "SELECT paydedu.dd_amount from pr_emp_payslip_deductions paydedu where paydedu.payslip_mid =" + payslip_mid + " AND paydedu.dd_name = 'VPF Deduction'";

                    // string GetLastmonthDetailes = " SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount,obshare.bank_share_intrst_open,obshare.own_share_intrst_open,obshare.vpf_intrst_open,obshare.own_share_intrst_total,obshare.bank_share_intrst_total,obshare.vpf_intrst_total from pr_ob_share_encashment obshare  " +
                        // " WHERE active=1 AND obshare.emp_code = " + Emp_code;
                    string GetLastmonthDetailes = "SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.vpf_intrst_total,obshare.own_share_intrst_total,obshare.bank_share_intrst_total from pr_ob_share_encashment obshare  WHERE (Month(obshare.fm) =" + month + ")  AND (Year(obshare.fm)=" +fy+ ") AND obshare.emp_code="+ Emp_code;

                    string exsitedemp = "SELECT obshare.id,obshare.emp_code,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount From pr_ob_share_encashment obshare  WHERE(Month(obshare.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(obshare.fm) = (select Year(fm) from pr_month_details where Active = 1)) and obshare.emp_code =" + Emp_code;

                    DataSet dtVPF_lastmonth = await _sha.Get_MultiTables_FromQry(GetLastmonthDetailes + exsitedemp);

                    double vpf_amount = 0;
                    double vpf_amount_intrest_amount = 0;

                    double c_own_share_open = 0;
                    double c_own_share_total = 0;
                    double c_bank_share_open = 0;
                    double c_bank_share_total = 0;
                    double c_vpf_open = 0;
                    double c_vpf_total = 0;

                    // TOTAL INTEREST CALCULATIONS RELATED NAMES ARE COMMENTED 18/19/19
                    // Interest 
                    double own_share_intrst_total = 0;
                    double vpf_intrst_total = 0;
                    double bank_share_intrst_total = 0;
                    double own_share_open = 0;
                    double own_share_total = 0;
                    double bank_share_open = 0;
                    double bank_share_total = 0;
                    double vpf_open = 0;
                    double vpf_total = 0;

                    //newly added on 16/05/2020
                    string str_age;
                    DataTable dt_age;
                    //end

                    //  DataTable dtVPF = dtVPF_lastmonth.Tables[0];
                    DataTable dtVPF = dtPF.Tables[0];
                    DataTable dtLastmonthDetailes = dtVPF_lastmonth.Tables[0];
                    DataTable dtemp = dtVPF_lastmonth.Tables[1];
                    //DataTable dtemp = dtPF.Tables[0];
                    if (dtLastmonthDetailes.Rows.Count > 0)
                    {
                        try
                        {
                            //own_share_amounts
                            own_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_open"].ToString());
                            own_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_total"].ToString());

                            //bank_share_amounts
                            bank_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_open"].ToString());
                            bank_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_total"].ToString());


                            //vpf_amounts
                            vpf_open = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_open"].ToString());
                            vpf_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_total"].ToString());

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString()))
                            {
                                bank_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString());
                            }

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString()))
                            {
                                own_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString());
                            }

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString()))
                            {
                                vpf_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString());
                            }

                        }
                        catch (Exception e)
                        {
                            // added on 22/04/2020
                            _logger.Error(e.Message);
                            //

                        }
                    }

                    if (dtVPF.Rows.Count > 0)
                    {
                       
                        if (vpf_dd_amount.Rows.Count > 0)
                        {
                            vpf_amount = double.Parse(vpf_dd_amount.Rows[0]["dd_amount"].ToString()); // newly added on 22/5/2020 by chaitanya 
                            vpf_amount_intrest_amount = (vpf_amount * 8.5) / 100;
                        }
                        else
                        {
                            // do nothing
                        }
      
                        c_vpf_open = vpf_total;

                        c_vpf_total = (c_vpf_open + vpf_amount);

                    }

                    double own_share_amount = Convert.ToDouble(drFP["dd_provident_fund"].ToString());
                    double gross_amount = Convert.ToDouble(drFP["gross_amount"].ToString());

                    double pension_amount = Math.Round((gross_amount * 8.33) / 100);
                    double pension_open = 0;
                    double pension_total = 0;

                    //newly added on 16/05/2020
                    str_age = "SELECT EmpId,DOB,DOJ,RetirementDate,CASE WHEN dateadd(year, datediff (year, DOB, getdate()), DOB) > getdate()" +
                                "THEN datediff(year, DOB, getdate()) -1 " +
                                "ELSE datediff(year, DOB, getdate()) END as Age FROM Employees where Empid=" + Emp_code;
                    dt_age = await _sha.Get_Table_FromQry(str_age);
                    if (Convert.ToInt32(dt_age.Rows[0]["Age"]) <= 58)
                    {
                        if (pension_amount <= 1250)
                        {
                            pension_open = pension_amount;
                        }
                        else
                        {
                            pension_open = 1250;
                        }
                    }
                    else
                    {
                        pension_open = 0;
                    }
    
                    pension_total += pension_open;

                    double bank_share_amount = (own_share_amount - pension_open);
                    c_own_share_open = own_share_total;
                    c_own_share_total = (c_own_share_open + own_share_amount);
                    c_bank_share_open = bank_share_total;
                    c_bank_share_total = (c_bank_share_open + bank_share_amount);

                    //2. gen new num
                    NewNumIndex++;

                    sbqry.Append(GetNewNumStringArr("pr_ob_share_encashment", NewNumIndex));
                    //query

                    //for update records
                    qryUpdate = "update pr_ob_share_encashment set active = 0 where emp_code=" + Emp_code + ";";
                    sbqry.Append(qryUpdate);

                    if (dtemp.Rows.Count > 0)
                    {
                        int c_id = int.Parse(dtemp.Rows[0]["id"].ToString());
                        inQry = "Update pr_ob_share_encashment set own_share=" + own_share_amount + ",vpf =" + vpf_amount + " ,bank_share=" + bank_share_amount + ",own_share_open =" + c_own_share_open + " ,own_share_total=" + c_own_share_total + " ,vpf_open=" + c_vpf_open + " ,vpf_total=" + c_vpf_total + " ,bank_share_open=" + c_bank_share_open + " ,bank_share_total=" + c_bank_share_total + ",pension_open=" + pension_open + ",pension_total=" + pension_total + ",   own_share_intrst_total=" + own_share_intrst_total + ",bank_share_intrst_total=" + bank_share_intrst_total + ",vpf_intrst_total=" + vpf_intrst_total + "  WHERE id=" + c_id + " AND emp_code=" + Emp_code + ";";
                        _logger.Info("Record exist for Emp_code " + Emp_code + ", So updating the record with set own_share = " + own_share_amount + ", vpf = " + vpf_amount + ", bank_share = " + bank_share_amount + ", own_share_open = " + c_own_share_open + ", own_share_total = " + c_own_share_total + ", vpf_open = " + c_vpf_open + ", vpf_total = " + c_vpf_total + ", bank_share_open = " + c_bank_share_open + ", bank_share_total = " + c_bank_share_total + ", pension_open = " + pension_open + ", pension_total = " + pension_total + ", own_share_intrst_total = " + own_share_intrst_total + ", bank_share_intrst_total = " + bank_share_intrst_total + ", vpf_intrst_total = " + vpf_intrst_total + "  WHERE id = " + c_id + " AND emp_code = " + Emp_code + "");
                    }
                    else
                    {
                        inQry = "Insert into pr_ob_share_encashment(id,fy,fm,emp_id,emp_code," +
                        "own_share,vpf,bank_share," +
                        "active,[trans_id],[own_share_open]," +
                        "[own_share_total],[vpf_open],[vpf_total]," +
                        "[bank_share_open]," + "[bank_share_total],[pension_open],[pension_total],[own_share_intrst_total]," +
                        "[bank_share_intrst_total],[vpf_intrst_total],[is_interest_caculated],[pension_intrest_amount])" +
                        "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + Emp_id + "," + Emp_code + "," +
                        "" + own_share_amount + "," + vpf_amount + "," + bank_share_amount + "," +
                        "1, @transidnew " + "," + "" + c_own_share_open + "," +
                        "" + c_own_share_total + "," + c_vpf_open + " , " + c_vpf_total + " ," +
                        " " + c_bank_share_open + " , " + c_bank_share_total + "," + pension_open + "," + pension_total + "   ," + own_share_intrst_total + "," + bank_share_intrst_total + "," + vpf_intrst_total + ",0,0);";

                        //********************** LOG FILE******************************
                        //newly added on 25/05/2020 to check the records in log file.
                        _logger.Info("Record inserting for Emp_Code: " + Emp_code + " in pr_ob_share_encashment table.");
                        _logger.Info("Insert into pr_ob_share_encashment(id,fy,fm,emp_id,emp_code," +
                            "own_share,vpf,bank_share," +
                            "active,[trans_id],[own_share_open]," +
                            "[own_share_total],[vpf_open],[vpf_total]," +
                            "[bank_share_open]," + "[bank_share_total],[pension_open],[pension_total],[own_share_intrst_total]," +
                            "[bank_share_intrst_total],[vpf_intrst_total],[is_interest_caculated],[pension_intrest_amount])" +
                            "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + Emp_id + "," + Emp_code + "," +
                            "" + own_share_amount + "," + vpf_amount + "," + bank_share_amount + "," +
                            "1, @transidnew " + "," + "" + c_own_share_open + "," +
                            "" + c_own_share_total + "," + c_vpf_open + " , " + c_vpf_total + " ," +
                            " " + c_bank_share_open + " , " + c_bank_share_total + "," + pension_open + "," + pension_total + "   ," + own_share_intrst_total + "," + bank_share_intrst_total + "," + vpf_intrst_total + ",0,0);");
                        //********************** LOG FILE******************************
                        //end
                    }
                    sbqry.Append(inQry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_attendance", "@idnew" + NewNumIndex, ""));
                }
            }
            if (dtPFAdhocData.Rows.Count > 0)
            {
                foreach (DataRow drFP in dtPFAdhocData.Rows)
                {
                    int Emp_code = Convert.ToInt32(drFP["emp_code"].ToString());
                    //int Emp_code = 836;
                    int Emp_id = Convert.ToInt32(drFP["emp_id"].ToString());
                    int payslip_mid = int.Parse(drFP["payslip_mid"].ToString());

                    //newly added on 16/05/2020
                    qryvpfdd_amount = "select dd_amount from pr_emp_payslip_deductions where payslip_mid=(select id from pr_emp_payslip where emp_code=" + Emp_code + " and year(fm)=(select Year(fm) from pr_month_details where Active = 1) and month(fm)=(select Month(fm) from pr_month_details where Active = 1) and spl_type='Adhoc' ) AND dd_name = 'VPF Deduction'";
                    vpf_dd_amount = await _sha.Get_Table_FromQry(qryvpfdd_amount);
                    //end

                    string VPFdata = "SELECT paydedu.dd_amount from pr_emp_payslip_deductions paydedu where paydedu.payslip_mid =" + payslip_mid + " AND paydedu.dd_name = 'VPF Deduction'";

                    // string GetLastmonthDetailes = " SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount,obshare.bank_share_intrst_open,obshare.own_share_intrst_open,obshare.vpf_intrst_open,obshare.own_share_intrst_total,obshare.bank_share_intrst_total,obshare.vpf_intrst_total from pr_ob_share_adhoc obshare  " +
                        // " WHERE active=1 AND obshare.emp_code = " + Emp_code;
                    string GetLastmonthDetailes = "SELECT obshare.own_share_open,obshare.own_share_total,obshare.vpf_open, obshare.vpf_total,obshare.bank_share_open,obshare.bank_share_total,obshare.vpf_intrst_total,obshare.own_share_intrst_total,obshare.bank_share_intrst_total from pr_ob_share_adhoc obshare  WHERE (Month(obshare.fm) =" + month + ")  AND (Year(obshare.fm)=" +fy+ ") AND obshare.emp_code="+ Emp_code;

                    string exsitedemp = "SELECT obshare.id,obshare.emp_code,obshare.bank_share_intrst_amount,obshare.own_share_intrst_amount, obshare.vpf_intrst_amount From pr_ob_share_adhoc obshare  WHERE(Month(obshare.fm) = (select Month(fm) from pr_month_details where Active = 1))  AND(Year(obshare.fm) = (select Year(fm) from pr_month_details where Active = 1)) and obshare.emp_code =" + Emp_code;

                    DataSet dtVPF_lastmonth = await _sha.Get_MultiTables_FromQry(GetLastmonthDetailes + exsitedemp);

                    double vpf_amount = 0;
                    double vpf_amount_intrest_amount = 0;

                    double c_own_share_open = 0;
                    double c_own_share_total = 0;
                    double c_bank_share_open = 0;
                    double c_bank_share_total = 0;
                    double c_vpf_open = 0;
                    double c_vpf_total = 0;

                    // TOTAL INTEREST CALCULATIONS RELATED NAMES ARE COMMENTED 18/19/19
                    // Interest 
                    double own_share_intrst_total = 0;
                    double vpf_intrst_total = 0;
                    double bank_share_intrst_total = 0;
                    double own_share_open = 0;
                    double own_share_total = 0;
                    double bank_share_open = 0;
                    double bank_share_total = 0;
                    double vpf_open = 0;
                    double vpf_total = 0;

                    //newly added on 16/05/2020
                    string str_age;
                    DataTable dt_age;
                    //end

                    //  DataTable dtVPF = dtVPF_lastmonth.Tables[0];
                    DataTable dtVPF = dtPF.Tables[0];
                    DataTable dtLastmonthDetailes = dtVPF_lastmonth.Tables[0];
                    DataTable dtemp = dtVPF_lastmonth.Tables[1];
                    //DataTable dtemp = dtPF.Tables[0];
                    if (dtLastmonthDetailes.Rows.Count > 0)
                    {
                        try
                        {
                            //own_share_amounts
                            own_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_open"].ToString());
                            own_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_total"].ToString());

                            //bank_share_amounts
                            bank_share_open = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_open"].ToString());
                            bank_share_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_total"].ToString());


                            //vpf_amounts
                            vpf_open = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_open"].ToString());
                            vpf_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_total"].ToString());

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString()))
                            {
                                bank_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["bank_share_intrst_total"].ToString());
                            }

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString()))
                            {
                                own_share_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["own_share_intrst_total"].ToString());
                            }

                            if (!string.IsNullOrWhiteSpace(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString()))
                            {
                                vpf_intrst_total = double.Parse(dtLastmonthDetailes.Rows[0]["vpf_intrst_total"].ToString());
                            }

                        }
                        catch (Exception e)
                        {
                            // added on 22/04/2020
                            _logger.Error(e.Message);
                            //

                        }
                    }

                    if (dtVPF.Rows.Count > 0)
                    {

                        if (vpf_dd_amount.Rows.Count > 0)
                        {
                            vpf_amount = double.Parse(vpf_dd_amount.Rows[0]["dd_amount"].ToString()); // newly added on 22/5/2020 by chaitanya 
                            vpf_amount_intrest_amount = (vpf_amount * 8.5) / 100;
                        }
                        else
                        {
                            // do nothing
                        }

                        c_vpf_open = vpf_total;

                        c_vpf_total = (c_vpf_open + vpf_amount);

                    }

                    double own_share_amount = Convert.ToDouble(drFP["dd_provident_fund"].ToString());
                    double gross_amount = Convert.ToDouble(drFP["gross_amount"].ToString());

                    double pension_amount = Math.Round((gross_amount * 8.33) / 100);
                    double pension_open = 0;
                    double pension_total = 0;

                    //newly added on 16/05/2020
                    str_age = "SELECT EmpId,DOB,DOJ,RetirementDate,CASE WHEN dateadd(year, datediff (year, DOB, getdate()), DOB) > getdate()" +
                                "THEN datediff(year, DOB, getdate()) -1 " +
                                "ELSE datediff(year, DOB, getdate()) END as Age FROM Employees where Empid=" + Emp_code;
                    dt_age = await _sha.Get_Table_FromQry(str_age);
                    if (Convert.ToInt32(dt_age.Rows[0]["Age"]) <= 58)
                    {
                        if (pension_amount <= 1250)
                        {
                            pension_open = pension_amount;
                        }
                        else
                        {
                            pension_open = 1250;
                        }
                    }
                    else
                    {
                        pension_open = 0;
                    }

                    pension_total += pension_open;

                    double bank_share_amount = (own_share_amount - pension_open);
                    c_own_share_open = own_share_total;
                    c_own_share_total = (c_own_share_open + own_share_amount);
                    c_bank_share_open = bank_share_total;
                    c_bank_share_total = (c_bank_share_open + bank_share_amount);

                    //2. gen new num
                    NewNumIndex++;

                    sbqry.Append(GetNewNumStringArr("pr_ob_share_adhoc", NewNumIndex));
                    //query

                    //for update records
                    qryUpdate = "update pr_ob_share_adhoc set active = 0 where emp_code=" + Emp_code + ";";
                    sbqry.Append(qryUpdate);

                    if (dtemp.Rows.Count > 0)
                    {
                        int c_id = int.Parse(dtemp.Rows[0]["id"].ToString());
                        inQry = "Update pr_ob_share_adhoc set own_share=" + own_share_amount + ",vpf =" + vpf_amount + " ,bank_share=" + bank_share_amount + ",own_share_open =" + c_own_share_open + " ,own_share_total=" + c_own_share_total + " ,vpf_open=" + c_vpf_open + " ,vpf_total=" + c_vpf_total + " ,bank_share_open=" + c_bank_share_open + " ,bank_share_total=" + c_bank_share_total + ",pension_open=" + pension_open + ",pension_total=" + pension_total + ",   own_share_intrst_total=" + own_share_intrst_total + ",bank_share_intrst_total=" + bank_share_intrst_total + ",vpf_intrst_total=" + vpf_intrst_total + "  WHERE id=" + c_id + " AND emp_code=" + Emp_code + ";";
                        _logger.Info("Record exist for Emp_code " + Emp_code + ", So updating the record with set own_share = " + own_share_amount + ", vpf = " + vpf_amount + ", bank_share = " + bank_share_amount + ", own_share_open = " + c_own_share_open + ", own_share_total = " + c_own_share_total + ", vpf_open = " + c_vpf_open + ", vpf_total = " + c_vpf_total + ", bank_share_open = " + c_bank_share_open + ", bank_share_total = " + c_bank_share_total + ", pension_open = " + pension_open + ", pension_total = " + pension_total + ", own_share_intrst_total = " + own_share_intrst_total + ", bank_share_intrst_total = " + bank_share_intrst_total + ", vpf_intrst_total = " + vpf_intrst_total + "  WHERE id = " + c_id + " AND emp_code = " + Emp_code + "");
                    }
                    else
                    {
                        inQry = "Insert into pr_ob_share_adhoc(id,fy,fm,emp_id,emp_code," +
                        "own_share,vpf,bank_share," +
                        "active,[trans_id],[own_share_open]," +
                        "[own_share_total],[vpf_open],[vpf_total]," +
                        "[bank_share_open]," + "[bank_share_total],[pension_open],[pension_total],[own_share_intrst_total]," +
                        "[bank_share_intrst_total],[vpf_intrst_total],[is_interest_caculated],[pension_intrest_amount])" +
                        "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + Emp_id + "," + Emp_code + "," +
                        "" + own_share_amount + "," + vpf_amount + "," + bank_share_amount + "," +
                        "1, @transidnew " + "," + "" + c_own_share_open + "," +
                        "" + c_own_share_total + "," + c_vpf_open + " , " + c_vpf_total + " ," +
                        " " + c_bank_share_open + " , " + c_bank_share_total + "," + pension_open + "," + pension_total + "   ," + own_share_intrst_total + "," + bank_share_intrst_total + "," + vpf_intrst_total + ",0,0);";

                        //********************** LOG FILE******************************
                        //newly added on 25/05/2020 to check the records in log file.
                        _logger.Info("Record inserting for Emp_Code: " + Emp_code + " in pr_ob_share_encashment table.");
                        _logger.Info("Insert into pr_ob_share_adhoc(id,fy,fm,emp_id,emp_code," +
                            "own_share,vpf,bank_share," +
                            "active,[trans_id],[own_share_open]," +
                            "[own_share_total],[vpf_open],[vpf_total]," +
                            "[bank_share_open]," + "[bank_share_total],[pension_open],[pension_total],[own_share_intrst_total]," +
                            "[bank_share_intrst_total],[vpf_intrst_total],[is_interest_caculated],[pension_intrest_amount])" +
                            "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "'," + Emp_id + "," + Emp_code + "," +
                            "" + own_share_amount + "," + vpf_amount + "," + bank_share_amount + "," +
                            "1, @transidnew " + "," + "" + c_own_share_open + "," +
                            "" + c_own_share_total + "," + c_vpf_open + " , " + c_vpf_total + " ," +
                            " " + c_bank_share_open + " , " + c_bank_share_total + "," + pension_open + "," + pension_total + "   ," + own_share_intrst_total + "," + bank_share_intrst_total + "," + vpf_intrst_total + ",0,0);");
                        //********************** LOG FILE******************************
                        //end
                    }
                    sbqry.Append(inQry);
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_attendance", "@idnew" + NewNumIndex, ""));
                }
            }
            try
            {
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
            }
            catch (Exception ex)
            {
                _logger.Info(sbqry.ToString());
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                bRet = false;
            }

            return bRet;
        }

        #endregion


        #region // Upddate_Payslip_Deductions ,Allowance

        public async Task<bool> UpdatePayslip_Deductions_Allowance()
        {
            bool bRet = false;

            string qryUpdate = "";
            string qryUpdates = "";
            string qryupdatepayslip = "";
            string qryupdatepayslip_allowance = "";

            string payslipId = "";
            string PayslipAllow_Deduc = "";

            int rowCount = 0;
            int loopCount = 0;

            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());

            string getpayslip_deductions = "select pde.id as payslip_deductions_id, epay.fm ,pde.Active,epay.id as payslip_id from pr_emp_payslip epay" +
                                         " join pr_emp_payslip_deductions pde on epay.id = pde.payslip_mid " +
                                         " WHERE pde.Active=1";

            string getpayslip_Allowance = "select pallow.id as payslip_allowance_id, epay.fm,pallow.Active,epay.id as payslip_id from pr_emp_payslip epay " +
                                         " join pr_emp_payslip_allowance pallow on epay.id = pallow.payslip_mid " +
                                         " WHERE pallow.Active=1 ";

            string PayslipFinal = " select id from pr_emp_payslip where final_process='False' and active=1 ";
            DataSet getpayslipdata = await _sha.Get_MultiTables_FromQry(getpayslip_deductions + getpayslip_Allowance + PayslipFinal);

            DataTable dtdeductionsData = getpayslipdata.Tables[0];
            DataTable dtallowanceData = getpayslipdata.Tables[1];
            DataTable dtPayslipFinal = getpayslipdata.Tables[2];

            if (dtPayslipFinal.Rows.Count > 0)
            {
                foreach (DataRow drPayslip in dtPayslipFinal.Rows)
                {
                    rowCount = dtPayslipFinal.Rows.Count;
                    loopCount = loopCount + 1;
                    int payslip_ids = Convert.ToInt32(drPayslip["id"].ToString());
                    payslipId += payslip_ids.ToString() + ",";
                    //1. Update Payslip Final Process
                    if (rowCount == loopCount)
                    {
                        payslipId = payslipId.Remove(payslipId.Length - 1);
                        qryUpdates = "Update pr_emp_payslip SET final_process='True',active=0 WHERE id in (" + payslipId + ") ;";
                        sbqry.Append(qryUpdates);
                    }
                }
            }

            if (dtdeductionsData.Rows.Count > 0)
            {
                rowCount = 0;
                loopCount = 0;
                payslipId = "";
                PayslipAllow_Deduc = "";
                foreach (DataRow drdeduction in dtdeductionsData.Rows)
                {
                    rowCount = dtdeductionsData.Rows.Count;
                    loopCount = loopCount + 1;

                    int payslip_deductions_id = Convert.ToInt32(drdeduction["payslip_deductions_id"].ToString());
                    int payslip_id = Convert.ToInt32(drdeduction["payslip_id"].ToString());

                    PayslipAllow_Deduc += payslip_deductions_id.ToString() + ",";
                    payslipId += payslip_id.ToString() + ",";

                    if (rowCount == loopCount)
                    {
                        //1. Update Payslip_Deductions Actiove 0
                        PayslipAllow_Deduc = PayslipAllow_Deduc.Remove(PayslipAllow_Deduc.Length - 1);
                        payslipId = payslipId.Remove(payslipId.Length - 1);
                        qryUpdate = "Update pr_emp_payslip_deductions SET active=0 WHERE id in (" + PayslipAllow_Deduc + ") AND payslip_mid in (" + payslipId + ");";
                        sbqry.Append(qryUpdate);

                        //2. Update Payslip Active Zero
                        qryupdatepayslip = "Update pr_emp_payslip SET active=0 WHERE id in (" + payslipId + ");";
                        sbqry.Append(qryupdatepayslip);
                    }

                }
            }

            if (dtallowanceData.Rows.Count > 0)
            {
                rowCount = 0;
                loopCount = 0;
                payslipId = "";
                PayslipAllow_Deduc = "";
                foreach (DataRow drAllowance in dtallowanceData.Rows)
                {
                    rowCount = dtallowanceData.Rows.Count;
                    loopCount = loopCount + 1;

                    int payslip_allowance_id = Convert.ToInt32(drAllowance["payslip_allowance_id"].ToString());
                    int payslip_id = Convert.ToInt32(drAllowance["payslip_id"].ToString());

                    PayslipAllow_Deduc += payslip_allowance_id.ToString() + ",";
                    payslipId += payslip_id.ToString() + ",";

                    if (rowCount == loopCount)
                    {
                        //2. Update Payslip Active Zero
                        PayslipAllow_Deduc = PayslipAllow_Deduc.Remove(PayslipAllow_Deduc.Length - 1);
                        payslipId = payslipId.Remove(payslipId.Length - 1);
                        qryupdatepayslip = "Update pr_emp_payslip SET active=0 WHERE id in (" + payslipId + ");";
                        sbqry.Append(qryupdatepayslip);

                        //1. Update Payslip_Allowance
                        qryupdatepayslip_allowance = "Update pr_emp_payslip_allowance SET active=0 WHERE id in (" + PayslipAllow_Deduc + ") AND payslip_mid in (" + payslipId + ");";
                        sbqry.Append(qryupdatepayslip_allowance);
                    }

                }
            }

            try
            {
                bRet = await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
            }
            catch (Exception ex)
            {
                _logger.Info(sbqry.ToString());
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                bRet = false;
            }

            return bRet;

        }
        #endregion

        #region // Personal Earning_Deductions

        public async Task<bool> UpdatePersonal_Earning_Deduction()
        {
            bool bRet = false;
            string Iqry = "";
            StringBuilder sbqry = new StringBuilder();
            //trans_id
            sbqry.Append(GenNewTransactionString());

            string getperearn = " Select distinct emp_code from  pr_emp_perearning earn where active=1";
            string getperded = " Select distinct emp_code  from pr_emp_perdeductions ded where active = 1";
            string getfm = " Select fy,fm ,month_days from pr_month_details where Active=1";

            DataSet dtperearn_ded = await _sha.Get_MultiTables_FromQry(getperearn + getperded + getfm);

            DataTable dtperearn = dtperearn_ded.Tables[0];
            DataTable dtperded = dtperearn_ded.Tables[1];
            DataTable dtfm = dtperearn_ded.Tables[2];

            int NewNumIndex = 0;

            //Finance Month
            DateTime fmdate = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());
            int FY = Convert.ToInt32(dtfm.Rows[0]["fy"].ToString());
            string FM = fmdate.ToString("yyyy-MM-dd");

            DateTime Lastmonth = fmdate.AddMonths(-1);
            string fm = Lastmonth.ToString("yyyy-MM-dd");

            //1. Personal Earning
            if (dtperearn.Rows.Count > 0)
            {
                foreach (DataRow drearn in dtperearn.Rows)
                {
                    int Emp_code = Convert.ToInt32(drearn["emp_code"].ToString());

                    string getper_earn = "Select ear.emp_id,ear.m_id,ear.m_type,ear.amount,ear.section,ear.id from pr_emp_perearning ear WHERE ear.emp_code=" + Emp_code + " AND Active=1";

                    string getlastmonth_earn = "SELECT sum(amount) as amount from pr_emp_perearning per_earn where emp_code=" + Emp_code + " AND  Active=0";


                    DataSet dtperearn_Data = await _sha.Get_MultiTables_FromQry(getper_earn + getlastmonth_earn);

                    DataTable dtperearndata = dtperearn_Data.Tables[0];
                    DataTable dtLast_Amount = dtperearn_Data.Tables[1];

                    int m_id = Convert.ToInt32(dtperearndata.Rows[0]["m_id"].ToString());
                    int emp_id = Convert.ToInt32(dtperearndata.Rows[0]["emp_id"].ToString());


                    string Section = "";
                    string m_type = "";


                    if (!string.IsNullOrWhiteSpace(dtperearndata.Rows[0]["section"].ToString()))
                    {
                        Section = dtperearndata.Rows[0]["section"].ToString();

                    }

                    if (!string.IsNullOrWhiteSpace(dtperearndata.Rows[0]["m_type"].ToString()))
                    {
                        m_type = "";// dtperearndata.Rows[0]["m_type"].ToString();
                    }

                    double _amount = 0;
                    double lastmonth_Amount = 0;
                    double total_Amount = 0;

                    if (dtLast_Amount.Rows.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(dtLast_Amount.Rows[0]["amount"].ToString()))
                        {
                            lastmonth_Amount = double.Parse(dtLast_Amount.Rows[0]["amount"].ToString());
                        }

                    }

                    foreach (DataRow dr in dtperearndata.Rows)
                    {
                        int e_mid = Convert.ToInt32(dr["id"].ToString());

                        double Amount = Convert.ToDouble(dr["amount"].ToString());

                        if (Amount > 0)
                        {
                            _amount += Amount;

                        }

                        //update oldid 
                        sbqry.Append("Update pr_emp_perearning set Active= 0  WHERE id=" + e_mid + " AND emp_code=" + Emp_code + ";");
                    }

                    total_Amount = (_amount + lastmonth_Amount);

                    NewNumIndex++;
                    //gen new num
                    sbqry.Append(GetNewNumStringArr("pr_emp_perearning", NewNumIndex));

                    Iqry = "insert into pr_emp_perearning ([id],[emp_id],[emp_code],[fy]," +
                       "[fm],[m_id],[m_type],[amount],[section],[active],[trans_id])" + " VALUES (@idnew" + NewNumIndex + "," + emp_id + "," +
                        +Emp_code + "," + FY + ",'" + FM + "'," + m_id + ",'" + m_type + "', " + total_Amount + ",'" + Section + "',1, @transidnew);";
                    sbqry.Append(Iqry);

                    //transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_perearning", "@idnew" + NewNumIndex, ""));
                }

            }

            //2.Personal Dedctions
            if (dtperded.Rows.Count > 0)
            {
                foreach (DataRow drded in dtperded.Rows)
                {
                    int emp_code = Convert.ToInt32(drded["emp_code"].ToString());

                    string getper_ded = "Select ded.emp_id,ded.m_id,ded.m_type,ded.amount,ded.section,ded.id from pr_emp_perdeductions ded WHERE ded.emp_code=" + emp_code + " AND Active=1";

                    DataTable dtperdeddata = await _sha.Get_Table_FromQry(getper_ded);

                    foreach (DataRow dr in dtperdeddata.Rows)
                    {
                        int dedm_id = Convert.ToInt32(dr["m_id"].ToString());
                        double deAmount = Convert.ToDouble(dr["amount"].ToString());
                        int Emp_id = Convert.ToInt32(dr["emp_id"].ToString());
                        int ded_mid = Convert.ToInt32(dr["id"].ToString());

                        string Sectionded = "";
                        string dedm_type = "";

                        if (!string.IsNullOrWhiteSpace(dr["section"].ToString()))
                        {
                            Sectionded = dr["section"].ToString();

                        }

                        if (!string.IsNullOrWhiteSpace(dr["m_type"].ToString()))
                        {
                            dedm_type = dr["m_type"].ToString();
                        }

                        NewNumIndex++;
                        //gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_perdeductions", NewNumIndex));

                        Iqry = "insert into pr_emp_perdeductions ([id],[emp_id],[emp_code],[fy]," +
                           "[fm],[m_id],[m_type],[amount],[section],[active],[trans_id])" + " VALUES (@idnew" + NewNumIndex + ", " + Emp_id + ", " +
                          +emp_code + "," + FY + ",'" + FM + "'," + dedm_id + ",'" + dedm_type + "', " + deAmount + ",'" + Sectionded + "',1, @transidnew);";

                        sbqry.Append(Iqry);

                        //transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_perdeductions", "@idnew" + NewNumIndex, ""));
                        //update oldid 
                        sbqry.Append("UPDATE pr_emp_perdeductions SET active=0 WHERE id=" + ded_mid + " AND emp_code=" + emp_code + ";");
                    }
                }
            }
            try
            {
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

            return bRet;
        }
        #endregion


        #region // Update TDS_Process

        public async Task<bool> UpdateTDS_Tax_Deducted()
        {
            bool bRet = false;
            StringBuilder sbqry = new StringBuilder();
            //trans_id
            sbqry.Append(GenNewTransactionString());

            string qryUpdate = "";

            string getTDS = "select distinct empcode,tds.tax_deducted_at_source,tds.tds_per_month from pr_emp_tds_process tds  where active=1";

            DataTable dttdsdata = await _sha.Get_Table_FromQry(getTDS);

            int Emp_code = 0;
            if (dttdsdata.Rows.Count > 0)
            {
                foreach (DataRow drtds in dttdsdata.Rows)
                {
                    Emp_code = Convert.ToInt32(drtds["empcode"].ToString());

                    double tax_deducted_at_source = Convert.ToDouble(drtds["tax_deducted_at_source"].ToString());
                    double tds_per_month = Convert.ToDouble(drtds["tds_per_month"].ToString());

                    //Added Tds Tax_Deduction_Source_Amount
                    double tax_deducted_at_source_Amount = (tax_deducted_at_source + tds_per_month);

                    //1. Update pr_emp_tds_process 
                    qryUpdate = "Update pr_emp_tds_process set tax_deducted_at_source = " + tax_deducted_at_source_Amount + " where empcode=" + Emp_code + " AND active=1 " + ";";
                    sbqry.Append(qryUpdate);

                }
                try
                {
                    if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                    {
                        bRet = true;
                        _logger.Info(Emp_code + " " + "Processed Successfully....! ");

                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(Emp_code + "" + ex);
                    _logger.Error(ex.Message);
                    _logger.Error(ex.StackTrace);
                }

            }

            return bRet;
        }

        #endregion

        #region //Update Adhoc Deduction_Contribution_Det_Earn

        public async Task<bool> UpdateAdhoc_Deductions_Contribution_Det_Earn()
        {
            bool bRet = false;

            StringBuilder sbqry = new StringBuilder();
            //trans_id
            sbqry.Append(GenNewTransactionString());

            string qryUpdate = "";

            string getEmployess = "SELECT empid from Employees";

            DataTable dtempdata = await _sha.Get_Table_FromQry(getEmployess);
            int emp_code = 0;
            foreach (DataRow dremp in dtempdata.Rows)
            {
                emp_code = Convert.ToInt32(dremp["empid"].ToString());

                string getadhoc_deduction = " SELECT ded.id from pr_emp_adhoc_deduction_field ded WHERE ded.emp_code=" + emp_code + " AND Active=1";
                string getadhoc_contribution = " SELECT con.id from pr_emp_adhoc_contribution_field con WHERE con.emp_code=" + emp_code + " AND Active=1";
                string getadhoc_det = " SELECT det.id from pr_emp_adhoc_det_field det WHERE det.emp_code=" + emp_code + " AND Active=1";
                string getadhoc_earn = " SELECT earn.id from pr_emp_adhoc_earn_field earn WHERE earn.emp_code=" + emp_code + " AND Active=1";


                DataSet dtgetalldetailes = await _sha.Get_MultiTables_FromQry(getadhoc_deduction + getadhoc_contribution + getadhoc_det + getadhoc_earn);

                //DataTables
                DataTable dtadhoc_deductiondata = dtgetalldetailes.Tables[0];
                DataTable dtadhoc_contribution = dtgetalldetailes.Tables[1];
                DataTable dtadhoc_det = dtgetalldetailes.Tables[2];
                DataTable dtadhoc_earn = dtgetalldetailes.Tables[3];

                //1. pr_emp_adhoc_deduction_field

                if (dtadhoc_deductiondata.Rows.Count > 0)
                {
                    foreach (DataRow drded in dtadhoc_deductiondata.Rows)
                    {
                        int c_id = Convert.ToInt32(drded["id"].ToString());

                        //1. Update pr_emp_adhoc_deduction_field Active=0 
                        qryUpdate = "Update pr_emp_adhoc_deduction_field set active =0" + " where emp_code=" + emp_code + " AND id=" + c_id + ";";
                        sbqry.Append(qryUpdate);

                    }
                }

                //2. pr_emp_adhoc_contribution_field

                if (dtadhoc_contribution.Rows.Count > 0)
                {
                    foreach (DataRow drcon in dtadhoc_contribution.Rows)
                    {
                        int c_id = Convert.ToInt32(drcon["id"].ToString());

                        //1. Update pr_emp_adhoc_contribution_field Active=0 
                        qryUpdate = "Update pr_emp_adhoc_contribution_field set active =0" + " where emp_code=" + emp_code + " AND id=" + c_id + ";";
                        sbqry.Append(qryUpdate);

                    }
                }

                //3.pr_emp_adhoc_det_field
                if (dtadhoc_det.Rows.Count > 0)
                {
                    foreach (DataRow drdet in dtadhoc_det.Rows)
                    {
                        int c_id = Convert.ToInt32(drdet["id"].ToString());

                        //1. Update pr_emp_adhoc_det_field Active=0 
                        qryUpdate = "Update pr_emp_adhoc_det_field set active =0" + " where emp_code=" + emp_code + " AND id=" + c_id + ";";
                        sbqry.Append(qryUpdate);

                    }
                }

                //4. pr_emp_adhoc_earn_field
                if (dtadhoc_earn.Rows.Count > 0)
                {
                    foreach (DataRow drearn in dtadhoc_earn.Rows)
                    {
                        int c_id = Convert.ToInt32(drearn["id"].ToString());

                        //1. Update pr_emp_adhoc_earn_field Active=0 
                        qryUpdate = "Update pr_emp_adhoc_earn_field set active =0" + " where emp_code=" + emp_code + " AND id=" + c_id + ";";
                        sbqry.Append(qryUpdate);

                    }
                }

            }
            try
            {
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    bRet = true;
                    _logger.Info(emp_code + " " + "Processed Successfully....! ");

                }
            }
            catch (Exception ex)
            {
                _logger.Error(emp_code + " " + ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

            return bRet;
        }
        #endregion

#region
        //before hourend loans details are dumping into beforemonthend tables.
        public async Task<int> BeforeHourEndProcessforLoans()
        {
            int prevmonloans = 0;
            int prevmonloans1 = 1;
            string str_adv_loans_ins = "";
            string str_adv_loans_del = "";
            string str_adv_loans_child_ins = "";
            string str_adv_loans_child_del = "";
            string str_adv_loans_adjustments_ins = "";
            string str_adv_loans_adjustments_del = "";
            try
            {
                str_adv_loans_del = "delete from pr_emp_adv_loans_bef_monthend;";
                str_adv_loans_child_del = "delete from pr_emp_adv_loans_child_bef_monthend;";
                str_adv_loans_adjustments_del = "delete from pr_emp_adv_loans_adjustments_bef_monthend;";

                bool res = await _sha.Run_UPDDEL_ExecuteNonQuery(str_adv_loans_del + str_adv_loans_child_del + str_adv_loans_adjustments_del);

                str_adv_loans_ins = "INSERT INTO pr_emp_adv_loans_bef_monthend SELECT * FROM pr_emp_adv_loans ;";
                str_adv_loans_child_ins = "INSERT INTO pr_emp_adv_loans_child_bef_monthend SELECT * FROM pr_emp_adv_loans_child ;";
                str_adv_loans_adjustments_ins = "INSERT INTO pr_emp_adv_loans_adjustments_bef_monthend SELECT * FROM pr_emp_adv_loans_adjustments ;";

                DataSet ds_loans = await _sha.Get_MultiTables_FromQry(str_adv_loans_ins + str_adv_loans_child_ins + str_adv_loans_adjustments_ins);
                _logger.Info("Sucessfully Inserted into pr_emp_adv_loans_bef_monthend, pr_emp_adv_loans_child_bef_monthend and pr_emp_adv_loans_adjustments_bef_monthend");
                return prevmonloans;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error("Something went wrong : "+ str_adv_loans_ins);
                _logger.Error("Something went wrong : " + str_adv_loans_child_ins);
                _logger.Error("Something went wrong : " + str_adv_loans_adjustments_ins);
            }
            return prevmonloans1;
        }
        #endregion

        //get finacial month
        public async Task<DateTime> Getfm()
        {
            string qryfmfy = " select fm from pr_month_details where active=1";
            DataTable dtfm = await _sha.Get_Table_FromQry(qryfmfy);
            DateTime paidDate = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());
            DateTime dtFm = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());

            return dtFm;
        }

    }
}
