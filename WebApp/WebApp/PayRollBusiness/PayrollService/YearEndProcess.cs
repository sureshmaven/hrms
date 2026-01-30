using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.PayrollService
{
   public class YearEndProcess : BusinessBase
    {


        log4net.ILog _logger = null;

        public YearEndProcess(LoginCredential loginCredential, log4net.ILog logger) : base(loginCredential)
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
        //*** End of common code ***
        //rent details
        public async Task Year_end_Rent_Details()
        {
            string qry = "";
            qry = "update pr_emp_rent_details set active=0 where active=1 ";
            await _sha.Run_UPDDEL_ExecuteNonQuery(qry);
        }
        //personal earnings
        public async Task Year_end_perearning()
        {

            string qry = "";
           
                qry = "update pr_emp_perearning set active=0 where active=1 ";
                await _sha.Run_UPDDEL_ExecuteNonQuery(qry);
           
        }
        // personal deductions
        public async Task Year_end_perdeductions()
        {
            string qry = "";
            qry = "update pr_emp_perdeductions set active=0 where active=1  ";
            await _sha.Run_UPDDEL_ExecuteNonQuery(qry);
        }
        //tds_deductions
        public async Task Year_end_tds_deductions()
        {
            string qry = "";
            qry = "update pr_emp_other_tds_deductions set active=0 where active=1  ";
            await _sha.Run_UPDDEL_ExecuteNonQuery(qry);
        }
        //tds_process
        public async Task Year_end_tds_process()
        {
            string qry = "";
            qry = "update pr_emp_tds_process set active=0 where active=1 ";
            await _sha.Run_UPDDEL_ExecuteNonQuery(qry);
        }
        //form 12ba
        public async Task Year_end_incometax_12ba()
        {
            string qry = "";
            qry = "update pr_emp_incometax_12ba set active=0 where active=1 ";
            await _sha.Run_UPDDEL_ExecuteNonQuery(qry);
        }

        public async Task Yesr_End_obshar_process()
        {
            string oldempid = "";
            string Emp_code = "";
           
            int NewNumIndex = 0;
            long pfprint = 0; long vpfprint = 0; long BankPrint = 0;
            long PFint = 0; long VPFint = 0; long Bankint = 0;
            decimal? pf_return = 0; decimal? VPFreturn = 0; decimal? Bankreturn = 0;
            long pfintcurr = 0; long vpfintcurr = 0; long bankintcurr = 0;
            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());
                     
            try
            {
                string qry = " select * from pr_month_details where active=1 ";
                DataTable qr = await _sha.Get_Table_FromQry(qry);
                DateTime Fdate = Convert.ToDateTime(qr.Rows[0]["fm"].ToString());
                string Edate = qr.Rows[0]["fy"].ToString();
                string Fdates = Fdate.ToString("yyyy");
                //DateTime Tdate = Convert.ToDateTime(SearchDatato);

                string qryfmfy1 = "select distinct ob_bal.fy, ob_bal.emp_code,pay.emp_code,ob_bal.os_open,ob_bal.vpf_open,ob_bal.bs_open,ob_bal.os_open_int,ob_bal.bs_open_int,ob_bal.vpf_open_int,ob_bal.pf_return,ob_bal.bank_return,ob_bal.vpf_return, ob_bal.os_cur_int,ob_bal.bs_cur_int,ob_bal.vpf_cur_int, ob_bal.pf_int_rate,ob_bal.run_date,ob_bal.trans_id " +
                    " from pr_pf_open_bal_year ob_bal join pr_emp_payslip pay on pay.emp_code = ob_bal.emp_code where pay.active = 1";
                
                DataTable dt2 = await _sha.Get_Table_FromQry(qryfmfy1);
                string query = "select distinct e.emp_code,ob_shar.fm,case when ob_shar.	own_share_intrst_amount is null then 0 else ob_shar.own_share_intrst_amount end as own_share_intrst_amount ," +
                    "case when ob_shar.vpf is null then 0 else ob_shar.vpf end as vpf ," +
                    "case when ob_shar.vpf_intrst_amount is null then 0 else ob_shar.vpf_intrst_amount end as vpf_intrst_amount   ," +
                    "case when ob_shar.bank_share is null then 0 else ob_shar.bank_share end as bank_share ," +
                    "case when ob_shar.bank_share_intrst_amount is null then 0 else ob_shar.bank_share_intrst_amount end as bank_share_intrst_amount ," +

                    "case when ob_shar.own_share_open is null then 0 else ob_shar.own_share_open end as own_share_open," +
                      "case when ob_shar.bank_share_open is null then 0 else ob_shar.bank_share_open end as bank_share_open ," +
                        "case when ob_shar.vpf_open is null then 0 else ob_shar.vpf_open end as vpf_open ," +

                    "case when ob_shar.	own_share_total is null then 0 else ob_shar.own_share_total end as own_share_total ," +
                    "case when ob_shar.vpf_total is null then 0 else ob_shar.vpf_total end as vpf_total   ,	" +
                    "case when ob_shar.bank_share_total is null then 0 else ob_shar.bank_share_total end as bank_share_total ," +

                    "case when ob_shar.own_share_intrst_open is null then 0 else ob_shar.own_share_intrst_open end as own_share_intrst_open   ," +
                    "case when ob_shar.bank_share_intrst_open is null then 0 else ob_shar.bank_share_intrst_open end as bank_share_intrst_open ," +
                       "case when ob_shar.vpf_intrst_open is null then 0 else ob_shar.vpf_intrst_open end as vpf_intrst_open ," +

                    "case when ob_shar.bank_share_intrst_total is null then 0 else ob_shar.bank_share_intrst_total end as bank_share_intrst_total ," +
                    "case when ob_shar.own_share_intrst_total is null then 0 else ob_shar.own_share_intrst_total end as own_share_intrst_total  ," +
                    "case when ob_shar.vpf_intrst_total is null then 0 else ob_shar.vpf_intrst_total end as vpf_intrst_total " +

                    "from pr_emp_general e left join pr_ob_share ob_shar on e.emp_code = ob_shar.emp_code and ob_shar.active=1  left join pr_emp_payslip pay on pay.emp_code = ob_shar.emp_code  "+
                    "where e.active=1 and ob_shar.fm between DATEFROMPARTS('" + Fdates + "', 04, 01) and DATEFROMPARTS('" + Edate + "', 03, 31 ) ";
                //and ob_shar.active = 1
                DataTable dt = await _sha.Get_Table_FromQry(query);

                foreach (DataRow dr1 in dt.Rows)
                {
                    string inQry = "";
                   Emp_code = dr1["emp_code"].ToString();
                    string qryfmfy2 = "select distinct e.emp_code,ob_shar.fm,case when ob_shar.	own_share_intrst_amount is null then 0 else ob_shar.own_share_intrst_amount end as own_share_intrst_amount ,case when ob_shar.vpf is null then 0 else ob_shar.vpf end as vpf ," +
                "case when ob_shar.vpf_intrst_amount is null then 0 else ob_shar.vpf_intrst_amount end as vpf_intrst_amount  ,case when ob_shar.bank_share is null then 0 else ob_shar.bank_share end as bank_share ," +
                "case when ob_shar.bank_share_intrst_amount is null then 0 else ob_shar.bank_share_intrst_amount end as bank_share_intrst_amount ," +

                "case when ob_shar.own_share_open is null then 0 else ob_shar.own_share_open end as own_share_open," +
                  "case when ob_shar.bank_share_open is null then 0 else ob_shar.bank_share_open end as bank_share_open ," +
                    "case when ob_shar.vpf_open is null then 0 else ob_shar.vpf_open end as vpf_open ," +

                "case when ob_shar.	own_share_total is null then 0 else ob_shar.own_share_total end as own_share_total ," +
                "case when ob_shar.vpf_total is null then 0 else ob_shar.vpf_total end as vpf_total   ,	" +
                "case when ob_shar.bank_share_total is null then 0 else ob_shar.bank_share_total end as bank_share_total ," +

                "case when ob_shar.own_share_intrst_open is null then 0 else ob_shar.own_share_intrst_open end as own_share_intrst_open   ," +
                "case when ob_shar.bank_share_intrst_open is null then 0 else ob_shar.bank_share_intrst_open end as bank_share_intrst_open ," +
                   "case when ob_shar.vpf_intrst_open is null then 0 else ob_shar.vpf_intrst_open end as vpf_intrst_open ," +

                "case when ob_shar.bank_share_intrst_total is null then 0 else ob_shar.bank_share_intrst_total end as bank_share_intrst_total ," +
                "case when ob_shar.own_share_intrst_total is null then 0 else ob_shar.own_share_intrst_total end as own_share_intrst_total  ," +
                "case when ob_shar.vpf_intrst_total is null then 0 else ob_shar.vpf_intrst_total end as vpf_intrst_total " +

                "from pr_emp_general e left join pr_ob_share ob_shar on e.emp_code = ob_shar.emp_code " + //and ob_shar.active=1  
                "left join pr_emp_payslip pay on pay.emp_code = ob_shar.emp_code where e.active=1 "+
                "and ob_shar.fm between DATEFROMPARTS('" + Fdates + "', 04, 01) and DATEFROMPARTS('" + Edate + "', 03, 31 ) and e.emp_code='" + Emp_code + "' ";
                DataTable dt3 = await _sha.Get_Table_FromQry(qryfmfy2);

                    foreach (DataRow dr in dt3.Rows)
                    {

                        try
                        {
                            pfprint += Convert.ToInt64(dr["own_share_open"].ToString());
                        }

                        catch (Exception e)
                        {

                        }
                        try
                        {
                            vpfprint += Convert.ToInt64(dr["vpf_open"].ToString());
                        }

                        catch (Exception e)
                        {

                        }
                        try
                        {
                            BankPrint += Convert.ToInt64(dr["bank_share_open"].ToString());
                        }

                        catch (Exception e)
                        {

                        }

                        // insterest open
                        try
                        {
                            PFint += Convert.ToInt64(dr["own_share_intrst_open"].ToString());
                        }

                        catch (Exception e)
                        {

                        }
                        try
                        {
                            VPFint += Convert.ToInt64(dr["vpf_intrst_open"].ToString());
                        }

                        catch (Exception e)
                        {

                        }
                        try
                        {
                            Bankint += Convert.ToInt64(dr["bank_share_intrst_open"].ToString());
                        }

                        catch (Exception e)
                        {

                        }

                        // insterest curr
                        try
                        {
                            pfintcurr += Convert.ToInt64(dr["own_share_intrst_total"].ToString());
                        }
                        catch (Exception e)
                        {

                        }
                        try
                        {
                            vpfintcurr += Convert.ToInt64(dr["vpf_intrst_total"].ToString());
                        }
                        catch (Exception e)
                        {

                        }
                        try
                        {
                            bankintcurr += Convert.ToInt64(dr["bank_share_intrst_total"].ToString());
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    //DateTime years1 = Convert.ToDateTime(dr1["fm"].ToString());
                    DateTime years ;
                    if (dr1["fm"].ToString() != "")
                    {
                         years = Convert.ToDateTime(dr1["fm"].ToString()).AddYears(1);
                    }
                    else
                    {
                        years = DateTime.Now;
                    }
                    //DateTime years =Convert.ToDateTime(dr1["fm"].ToString()).AddYears(1);
                    string year = years.ToString("yyyy");
                    foreach (DataRow rn in dt2.Rows)
                    {
                        if (!rn.IsNull("pf_return"))
                        {
                            pf_return = Convert.ToDecimal(rn["pf_return"]);
                        }
                        else
                        {
                            pf_return = 0;
                        }
                        if (!rn.IsNull("bank_return"))
                        {
                            VPFreturn = Convert.ToDecimal(rn["bank_return"]);
                        }
                        else
                        {
                            VPFreturn = 0;
                        }
                        if (!rn.IsNull("vpf_return"))
                        {
                            Bankreturn = Convert.ToDecimal(rn["vpf_return"]);
                        }
                        else
                        {
                            Bankreturn = 0;
                        }
                    }
                    //pfreturn = Convert.ToInt64(dt2.Rows[0]["pfreturn"]) ;
                    //VPFreturn = Convert.ToInt64(dt2.Rows[0]["VPFreturn"]) ;
                   // Bankreturn = Convert.ToInt64(dt2.Rows[0]["Bankreturn"]) ;

                    //   pfintcurr = Convert.ToDecimal(dr["pfintcurr"]);
                    //  vpfintcurr = Convert.ToDecimal(dr["vpfintcurr"]); 
                    //   bankintcurr = Convert.ToDecimal(dr["bankintcurr"]);

                    //DateTime intDate = Convert.ToDateTime(dt2.Rows[0]["intDate"]);
                    //decimal pfintrate = Convert.ToDecimal(dt2.Rows[0]["pfintrate"].ToString());
                    string intDate = DateTime.Now.ToString("yyyy/MM/dd");
                    decimal pfintrate = 0;

                  
                    if (oldempid != Emp_code)
                    {
                        NewNumIndex++;
                        sbqry.Append(GetNewNumStringArr("pr_pf_open_bal_year", NewNumIndex));
                        inQry = "Insert into pr_pf_open_bal_year(id, emp_id, emp_code, fy, os_open, vpf_open, bs_open, os_open_int, vpf_open_int, bs_open_int, pf_return, vpf_return, bank_return, os_cur_int, vpf_cur_int, bs_cur_int, run_date, pf_int_rate, trans_id) " +
                            " values(@idnew" + NewNumIndex + ", (select id from Employees where EmpId = '" + Emp_code + "') ," + Emp_code + ", " + year + ", " + pfprint + ", " + vpfprint + ", " + BankPrint + ", " + PFint + ", " + VPFint + ", " + Bankint + "," + pf_return + ", " + VPFreturn + ", " + Bankreturn + " ," + pfintcurr + " ," + vpfintcurr + " ," + bankintcurr + ",'" + intDate + "'," + pfintrate + "  ,@transidnew)";
                        //inQry = "Insert into pr_pfopeningbal(id, emp_id, emp_code, year, pfprint, vpfprint, BankPrint, PFint, VPFint, Bankint, PFreturn, VPFreturn, Bankreturn, pfintcurr, " +
                        //    "vpfintcurr, bankintcurr, intDate, pfintrate, active, trans_id) " +
                        //    "values(@idnew" + NewNumIndex + ",  (select id from Employees where EmpId = '" + Emp_code + "') ," + Emp_code + ", " + year + ", " + pfprint + ", " + vpfprint + ", " + BankPrint + ", " + PFint + ", " + VPFint + ", " + Bankint + "," + pfreturn + ", " + VPFreturn + ", " +
                        //    " " + Bankreturn + " ," + pfintcurr + " ," + vpfintcurr + " ," + bankintcurr + ",'" + intDate + "'," + pfintrate + " ,1 ,@transidnew);";
                        sbqry.Append(inQry);
                    }
                    oldempid = dr1["emp_code"].ToString();
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_pf_open_bal_year", "@idnew" + NewNumIndex, ""));
                    try
                    {
                        await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                    }
                    catch (Exception ex)
                    {
                        // _logger.Info(sbqry.ToString());
                        _logger.Error(ex.Message);
                        _logger.Error(ex.StackTrace);
                    }
                                     
                }

            }

            catch(Exception e)
            {
                // _logger.Info(sbqry.ToString());
                _logger.Error(e.Message);
                _logger.Error(e.StackTrace);
            }
            
        }
        public async Task<bool> CopyData()
        {
            bool bRet = false;
            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());
            int NewNumIndex = 0;
            string qryFinacemonth = "select month(fm) as fm from pr_month_details where active= 1";
            DataSet dtempcodesandfm = await _sha.Get_MultiTables_FromQry(qryFinacemonth);
            DataTable dtfm = dtempcodesandfm.Tables[0];
            int fmdate = int.Parse(dtfm.Rows[0]["fm"].ToString());
            fmdate = fmdate - 1;
            if (fmdate == 3)
            {
                //copy payslip data
                NewNumIndex++;
                string CopypayslipData = "Insert Into pr_emp_payslip_Old Select * From pr_emp_payslip ";
                sbqry.Append(CopypayslipData);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_Old", " " + NewNumIndex, ""));
                //delete payslip data
                NewNumIndex++;
                string DeletepayslipData = "delete From pr_emp_payslip ";
                sbqry.Append(DeletepayslipData);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip", " " + NewNumIndex, ""));
                //copy payslip Allowance data
                NewNumIndex++;
                string CopypayslipAllowanceData = "Insert Into pr_emp_payslip_allowance_Old Select * From pr_emp_payslip_allowance ";
                sbqry.Append(CopypayslipAllowanceData);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance_Old", " " + NewNumIndex, ""));
                //delete payslip data
                NewNumIndex++;
                string DeletepayslipAllowanceData = "delete From pr_emp_payslip_allowance ";
                sbqry.Append(DeletepayslipAllowanceData);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance", " " + NewNumIndex, ""));
                //copy payslip deductions data
                NewNumIndex++;
                string CopypayslipdeductionsData = "Insert Into pr_emp_payslip_deductions_Old Select * From pr_emp_payslip_deductions ";
                sbqry.Append(CopypayslipdeductionsData);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions_Old", " " + NewNumIndex, ""));
                //delete payslip data
                NewNumIndex++;
                string DeletepayslideductionsData = "delete From pr_emp_payslip_deductions ";
                sbqry.Append(DeletepayslideductionsData);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", " " + NewNumIndex, ""));
                await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
            }
            return bRet;
        }

    }
}
