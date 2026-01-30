using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayRollBusiness.Process;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.PayrollService
{
    public class PaySlipSer : BusinessBase
    {
        log4net.ILog _logger = null;

        public PaySlipSer(LoginCredential loginCredential, log4net.ILog logger) : base(loginCredential)
        {
            _logger = logger;
        }

        //DataTables
        DataTable _dtConstants = null;
        //DataTable _incometaxdbconstant = null;
        DataTable dtPFPerks = null;
        DataTable getidfrompayslip = null;
        DataTable getidfrompayslips = null;
        DataTable dtGetSections;
        string getallrecords1 = "";
        string getallrecords12 = "";
        string sNoAttNoBasic = "";
        string NetAmountlessthanzero = "";

        //Global variables for payslip process
        int NewNumIndex = 0;
        DateTime Financial_md;
        Boolean payslip_final = false;

        string fp = "0";
        string sm = "0";

        string Eid = "";
        //int empcode = 0;

        //decimal rem_taxexemption = 0;
        int iFY = 0;
        int dtFM = 0;

        //queries 
        StringBuilder trnsqry;
        StringBuilder Er_Message;
        StringBuilder sbqry;

        string getallrecords = "";
        string getallrecordss = "";
        string getallrecordsss = "";
        string empcodes = "";

        decimal no_of_days_mnth = 0;
        string qry = "";
        string qrys = "";
        string p_type = "";
        string payslip_error_type = "";


        int processcount = 0;
        int seriveempstatus = 0;
        string Final_Process = "False";
        string Sent_Mail = "False";
        int srv_id = 0;
        int PreviousMonth_dtfm = 0;
        int ps_old_id = 0;

        int total_count = 0;
        DateTime adhoc_dtfm;




        public async Task<string> GetFm_And_FY()
        {
            string strQry = "Select fy, format(fm,'MM') as fm,format(fm,'yyyy-MM-dd') as FinancialMonthandDate from pr_month_details where active = 1;";
            DataTable dt = await _sha.Get_Table_FromQry(strQry);
            return dt.Rows[0]["fy"].ToString() + "^" +
                dt.Rows[0]["fm"].ToString() + "^" +
                dt.Rows[0]["FinancialMonthandDate"].ToString();
        }

        //service Start
        public async Task ServiceStarting(string servicename)
        {
            string qryIns = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action]) "
                + "VALUES(getdate(),'" + servicename + "','Start');";
            qryIns += "SELECT CAST(SCOPE_IDENTITY() as int);";
            await _sha.Run_INS_ExecuteScalar(qryIns);
        }

        //Service End
        public async Task ServiceStoping(string servicename)
        {
            string qryIns = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action]) "
                + "VALUES(getdate(),'" + servicename + "','Stop');";
            qryIns += "SELECT CAST(SCOPE_IDENTITY() as int);";

            await _sha.Run_INS_ExecuteScalar(qryIns);
        }
        public decimal CalcCEOAllowance(decimal er_Amt, string E_designation)
        {
            decimal ret = 0;
            decimal CEOPercentage = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "CEOPercentage")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            if (E_designation == "Managing Director")
            {
                ret = er_Amt * CEOPercentage;
            }
            else
            {
                ret = 0;
            }

            return (decimal)Math.Round(ret, 2,MidpointRounding.AwayFromZero);
        }
        //Calculate HRA
        public decimal CalcHRA(decimal er_Amt, string E_designation)
        {
            decimal ret = 0;
            decimal HRAOthersPercentage = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "HRAOthersPercentage")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            decimal HRAStaffAsstAttenderPercentage = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "HRAStaffAsstAttenderPercentage")
                .Select(x => x["value"].ToString()).FirstOrDefault());


            if (E_designation == "Staff Assistant" || E_designation == "Attender" || E_designation == "Driver" || E_designation == "Watchman")
            {
                ret = er_Amt * HRAStaffAsstAttenderPercentage;
            }
            else
            {
                ret = er_Amt * HRAOthersPercentage;
            }

            return (decimal)Math.Round(ret, 2,MidpointRounding.AwayFromZero);
        }

        //Calculate DA
        private decimal CalcDA(decimal er_Amt, DataTable dtDa_slabs)
        {
            decimal retVal = 0;
            decimal d = 0;
            if (dtDa_slabs.Rows.Count > 0)
            {
                DataRow dsl = dtDa_slabs.Rows[0];
                decimal da_slab = decimal.Parse(dsl["da_percent"].ToString());
                retVal = (da_slab / 100) * er_Amt;
                d = retVal % 1;
                double res = 0.5 - Convert.ToDouble(d);
                if (res >0.1)
                {
                    retVal= Math.Truncate(retVal * 100) / 100;
                }
                else
                {
                    retVal = Math.Round(retVal, 2, MidpointRounding.AwayFromZero);
                }
            }

            return retVal;
        }

        private decimal CalcSplDA(decimal er_Amt, DataTable dtDa_slabs)
        {
            decimal retVal = 0;
            if (dtDa_slabs.Rows.Count > 0)
            {
                DataRow dsl = dtDa_slabs.Rows[0];
                decimal da_slab = decimal.Parse(dsl["da_percent"].ToString());
                retVal = (da_slab / 100) * er_Amt;
            }

            return (decimal)Math.Round(retVal, 2);
        }



        //Calculate CCA
        public decimal CalcCCA(decimal er_Amt, decimal AnnualIncrement,string Designation,int workingdays,int no_of_days_mnth)
        {
            decimal ret = 0;

            //0.04
            string CCAPercentage = _dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "CCAPercentage")
                .Select(x => x["value"].ToString()).FirstOrDefault();

            //400
            decimal MinCCAConditionalAmt = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "MinCCAConditionalAmt")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            //470
            decimal MidCCAConditionalAmt = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "MidCCAConditionalAmt")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            //870
            decimal MaxCCAConditionalAmt = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "MaxCCAConditionalAmt")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            var n_CCA =(er_Amt)* decimal.Parse(CCAPercentage);


            if (Designation == "Attender" || Designation == "Attender Cum Watchman" || Designation == "Attender/J.C" || Designation == "Civil Engg Supervisor" || Designation == "Driver" || Designation == "Stenographer" || Designation == "Telephone Operator Cum Receptionist" || Designation == "Typist" || Designation == "Watchman" || Designation == "Subordinate Staff(Substaff)" || Designation == "Junior Clerk")
            {
                if(Designation == "Attender")
                {
                    var n_CCA1 = (er_Amt + AnnualIncrement) * decimal.Parse(CCAPercentage);
                    if (n_CCA1 <= MinCCAConditionalAmt)
                        ret = n_CCA1;
                    else
                    {
                        ret = MinCCAConditionalAmt / no_of_days_mnth * workingdays;
                    }
                }
                else
                {
                    if (n_CCA <= MinCCAConditionalAmt)
                        ret = n_CCA;
                    else
                    {
                        ret = MinCCAConditionalAmt / no_of_days_mnth * workingdays;
                    }
                }
                
                    
            }
            else if (Designation == "Staff Assistant" || Designation == "JR Staff Assistant" || Designation == "Staff Assistant Cum Assistant Cashier" || Designation == "JR Staff Assistant")
            {
                if (n_CCA <= MidCCAConditionalAmt)
                    ret = Convert.ToInt32(n_CCA);
                else
                    ret = MidCCAConditionalAmt / no_of_days_mnth * workingdays;
            }
            else if (Designation == "Managing Director" || Designation == "Chief General Manager" || Designation == "General Manager" || Designation == "Deputy General Manager" || Designation == "Assistant General Manager" || Designation == "Senior Manager" || Designation == "Manager Scale-1" || Designation == "Manager" || Designation == "Manager Tech" || Designation == "Deputy General Manager - Retired" || Designation == "GM" || Designation == "IDO/Manager"||Designation== "Assistant Manager")
            {
                if (n_CCA <= MaxCCAConditionalAmt)
                    ret = Convert.ToInt32(n_CCA);
                else
                    ret = MaxCCAConditionalAmt / no_of_days_mnth * workingdays;
            }

            return (decimal)Math.Round(ret, 2);
        }

        //Calculate Profession Tax
        private int CalcProfessionalTax(decimal er_Amt)
        {
            //15000,20000
            string proftaxMinMaxAmts = _dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "ProfTaxMinMaxAmts")
                .Select(x => x["value"].ToString()).FirstOrDefault();
            string[] arr = proftaxMinMaxAmts.Split(',');
            int minAmt = int.Parse(arr[0]);
            int maxAmt = int.Parse(arr[1]);

            //150,200
            string proftaxMinMaxAmtVals = _dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "ProfTaxMinMaxAmtVals")
                .Select(x => x["value"].ToString()).FirstOrDefault();
            string[] arrVal = proftaxMinMaxAmtVals.Split(',');

            int retVal = 0;

            if (er_Amt <= minAmt)
                retVal = 0;
            else if (er_Amt > minAmt && er_Amt <= maxAmt)
                retVal = int.Parse(arrVal[0]);
            else if (er_Amt > maxAmt)
                retVal = int.Parse(arrVal[1]);
            return retVal;
        }

        //Calculate ProvidentFund
        private decimal CalcPF(decimal er_Amt,string isnps)
        {
            decimal er_Amt1 = 0.00m;
            if (isnps == "No")
            {
                decimal ret = 0;
                decimal PFPercentage = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "PFPercentage")
                    .Select(x => x["value"].ToString()).FirstOrDefault());

                //ret = (decimal)Math.Round(er_Amt * PFPercentage, 0);
                er_Amt1 = (er_Amt * PFPercentage);
                ret = RoundoffDecimaltoHigherValue(er_Amt1);
                //ret = Math.Round((er_Amt * PFPercentage), MidpointRounding.AwayFromZero);
                return ret;
            }
            else
            {
                decimal ret = 0;
                decimal PFPercentageNPS = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "PFPercentageNPS")
                    .Select(x => x["value"].ToString()).FirstOrDefault());

                //ret = (decimal)Math.Round(er_Amt * PFPercentage, 0);
                er_Amt1 = (er_Amt * PFPercentageNPS);
                ret = RoundoffDecimaltoHigherValue(er_Amt1);
                //ret = Math.Round((er_Amt * PFPercentageNPS), MidpointRounding.AwayFromZero);
                return ret;
            }
          
        }
        private decimal RoundoffDecimaltoHigherValue(decimal er_amount)
        {
            decimal amount = Math.Round(er_amount,3);
            string str = amount.ToString();
            string[] strarr =str.Split('.');
            decimal amt = Convert.ToDecimal(strarr[1]);
            if(amt > 495)
            {
                amount=Math.Ceiling((decimal)Math.Round(amount, 2, MidpointRounding.AwayFromZero));
            }
            else
            {
                amount = Math.Floor((decimal)Math.Round(amount, 2, MidpointRounding.AwayFromZero));
            }
            return amount;
        }
        //Calculate NPS
        private decimal CalcNPS(decimal er_Amt, string isnps)
        {
            if (isnps == "No")
            {
                decimal ret = 0;
                return ret;
            }
            else
            {
                decimal ret = 0;
                decimal PFPercentageNPS = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "NPSPercentage")
                    .Select(x => x["value"].ToString()).FirstOrDefault());

                //ret = (decimal)Math.Round(er_Amt * PFPercentage, 0);
                ret = Math.Round((er_Amt * PFPercentageNPS), MidpointRounding.AwayFromZero);
                return ret;
            }

        }
        //Calculate SpecialAllowance
        private decimal CalcSpl_Allw(decimal er_Amt, string Designation)
        {
            decimal ret = 0;

            //special Allowance
            decimal getSpecila_Allw_Min = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
           .Where(x => x["constant"].ToString() == "Special_Allw_Min")
           .Select(x => x["value"].ToString()).FirstOrDefault());

            decimal getSpecila_Allw_Mid = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
           .Where(x => x["constant"].ToString() == "Special_Allw_Mid")
           .Select(x => x["value"].ToString()).FirstOrDefault());

            decimal getSpecila_Allw_Max = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
           .Where(x => x["constant"].ToString() == "Special_Allw_Max")
           .Select(x => x["value"].ToString()).FirstOrDefault());

            if (Designation == "Deputy General Manager" || Designation == "Assistant General Manager")
            {
                ret = er_Amt * getSpecila_Allw_Mid;
            }
            else if (Designation == "Managing Director" || Designation == "Chief General Manager" || Designation == "General Manager")
            {
                ret = er_Amt * getSpecila_Allw_Max;
            }
            else
            {
                ret = er_Amt * getSpecila_Allw_Min;
            }
            return (decimal)Math.Round(ret, 2,MidpointRounding.AwayFromZero);
        }

        //Calculate PFPerks
        private async Task<string> CalcPFPerks(decimal PF, int NewNumIndex, string fm, string empcode, string payslip_type)
        {
            string qryPfperks = "";
            decimal pfperks = PF * decimal.Parse(PrConstants.pf_perks.ToString());

            //string pfp_fm = "";
            //string pfp_m_id = "";
            //decimal pfp_amount = 0;
            //string pfp_m_type = "";



            //PFperksCalculation
            //if (dtPFPerks.Rows.Count > 0)
            //{
            //    DataRow dtpf = dtPFPerks.Rows[0];
            //    pfp_fm = dtpf["fm"].ToString();
            //    pfp_m_id = dtpf["m_id"].ToString();
            //    pfp_amount = decimal.Parse(dtpf["amount"].ToString());
            //    pfp_m_type = dtpf["m_type"].ToString();
            //}


            var tm_id = "";
            var tm_m_id = "";
            decimal tm_amount = 0;
            DataTable dtGetthismonthData = await _sha.Get_Table_FromQry("select m.id,format(c.fm,'MM') as fm,c.m_id,c.amount,c.m_type from pr_emp_perearning c join pr_earn_field_master m on c.m_id=m.id where m.name like '%PFPerks%' and m.type='per_earn' and c.emp_code=" + empcode + " and Month(c.fm)=" + dtFM + " and fy=" + iFY + " and section='" + payslip_type + "' ;");
            if (dtGetthismonthData.Rows.Count > 0)
            {
                DataRow dtpfperk = dtGetthismonthData.Rows[0];
                tm_id = dtpfperk["id"].ToString();
                tm_m_id = dtpfperk["m_id"].ToString();
                tm_amount = decimal.Parse(dtpfperk["amount"].ToString());
            }
            else
            {
                NewNumIndex++;
                qryPfperks += GetNewNumStringArr("pr_emp_perearning", NewNumIndex);
                qryPfperks += "Insert into pr_emp_perearning (id,fy,fm,emp_id,emp_code,m_id,m_type,amount,section,active,trans_id) values(@idnew" + NewNumIndex + "," + iFY + ",'" + fm + "'," + Eid + "," + empcode + ",(select id from pr_earn_field_master where name like '%PFPerks%' and type='per_earn'),'per_earn'," + Math.Round(pfperks, 2) + ",'" + payslip_type + "',1,@transidnew);";
                qryPfperks += GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_perearning", "@idnew" + NewNumIndex.ToString(), null);
            }

            if (tm_amount > 0)
            {
                qryPfperks += "Update pr_emp_perearning set amount=" + Math.Round(pfperks, 2) + " where m_id='" + tm_id + "' and emp_code=" + empcode + " and section='" + payslip_type + "' and active=1;";

            }
            //else if (pfp_amount > 0)
            //{
            //    tm_amount = pfp_amount + pfperks;
            //    qryPfperks += "Update pr_emp_perearning set amount=" + tm_amount + " where m_id='" + tm_id + "'and emp_code=" + empcode + " and active=1;";

            //}

            return qryPfperks;
        }

        //Method to check to payslip finally processed or not 
        private async Task<bool> CheckFinalProcess(string empcode, string payslip_type, DateTime Financial_md)
        {
            Boolean finalprocess = false;

            DataTable getidfrompayslip = await _sha.Get_Table_FromQry("select id,final_process from pr_emp_payslip where emp_code=" + empcode + " and spl_type='" + payslip_type + "' and month(fm)=" + dtFM + " and year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and active=1;");
            if (getidfrompayslip.Rows.Count > 0)
            {
                DataRow pid = getidfrompayslip.Rows[0];
                ps_old_id = int.Parse(pid["id"].ToString());
                payslip_final = Convert.ToBoolean(pid["final_process"]);
            }
            if (payslip_final == true)
            {
                finalprocess = true;
            }

            return finalprocess;
        }

        //Method to getActiveEmployees codes
        private string GetActiveEmpCodes_GeneralAdhocEncashment(string empcodes, string p_type)
        {
            if (p_type.Contains("Regular") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords = "select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Regular')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                    " union all select distinct ded.emp_code, ded.fm, ded.fy, 'Adhoc' as status, atted.status as stpstatus from pr_emp_adhoc_det_field ded " +
                    " join pr_month_attendance atted on atted.emp_code = ded.emp_code " +
                    " where ded.emp_code in (" + empcodes + ") and ded.active = 1 and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + " " +
                    " union all" +
                    " select distinct em.EmpId as emp_code, en.fm, en.fy, 'Encashment' as status, atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance " +
                    " atted on atted.emp_code = em.EmpId  where em.empid in (" + empcodes + ") and   " +
                    "   en.authorisation = 1 and en.PLEncash > 0 and en.process = 1  and Year(atted.fm)= " + Financial_md.Year + " and Year(en.fm)= " + Financial_md.Year + " and Month(en.fm) = " + Financial_md.Month + " " +
                    " and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended')) as x order by x.emp_code;";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords = "select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Suspended')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                    " union all select distinct ded.emp_code, ded.fm, ded.fy, 'Adhoc' as status, atted.status as stpstatus from pr_emp_adhoc_det_field ded " +
                    " join pr_month_attendance atted on atted.emp_code = ded.emp_code " +
                    " where ded.emp_code in (" + empcodes + ") and ded.active = 1 and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + " " +
                    " union all" +
                    " select distinct em.EmpId as emp_code, en.fm, en.fy, 'Encashment' as status, atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance " +
                    " atted on atted.emp_code = em.EmpId  where em.empid in (" + empcodes + ") and   " +
                    "   en.authorisation = 1 and en.PLEncash > 0 and en.process = 1 and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and Year(en.fm)= " + Financial_md.Year + " and Month(en.fm) = " + Financial_md.Month + " " +
                    " and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended')) as x order by x.emp_code;";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Adhoc"))
            {
                getallrecords = "select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Regular')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                    " union all select distinct ded.emp_code, ded.fm, ded.fy, 'Adhoc' as status, atted.status as stpstatus from pr_emp_adhoc_det_field ded " +
                    " join pr_month_attendance atted on atted.emp_code = ded.emp_code " +
                    " where ded.emp_code in (" + empcodes + ") and ded.active = 1 and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + ") as x order by x.emp_code ; ";

            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc"))
            {
                getallrecords = "select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Suspended')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                    " union all select distinct ded.emp_code, ded.fm, ded.fy, 'Adhoc' as status, atted.status as stpstatus from pr_emp_adhoc_det_field ded " +
                    " join pr_month_attendance atted on atted.emp_code = ded.emp_code " +
                    " where ded.emp_code in (" + empcodes + ") and ded.active = 1 and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + ") as x order by x.emp_code ; ";

            }
            else if (p_type.Contains("Regular") && p_type.Contains("Encashment"))
            {
                getallrecords = " select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Regular')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                   " union all" +
                    " select distinct em.EmpId as emp_code, en.fm, en.fy, 'Encashment' as status, atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance " +
                    " atted on atted.emp_code = em.EmpId  where em.empid in (" + empcodes + ") and   " +
                    "   en.authorisation = 1 and en.PLEncash > 0 and en.process = 1 and Year(en.fm)= " + Financial_md.Year + " and Month(en.fm) = " + Financial_md.Month + "" +
                    " and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended')) as x order by x.emp_code;";

            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Encashment"))
            {
                getallrecords = " select * from(select distinct atted.emp_code, atted.fm, atted.fy, atted.status, atted.status as stpstatus from pr_month_attendance atted" +
                    " join pr_emp_pay_field c on atted.emp_code = c.emp_code" +
                    " JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id" +
                    " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and atted.working_days > 0" +
                    " and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  status in ('Suspended')  and" +
                    " c.emp_code in (" + empcodes + ")" +
                    " and pc.field_type = 'pay_fields' and  m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount > 0 and m.name = 'Basic'" +
                   " union all" +
                    " select distinct em.EmpId as emp_code, en.fm, en.fy, 'Encashment' as status, atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance " +
                    " atted on atted.emp_code = em.EmpId  where em.empid in (" + empcodes + ") and   " +
                    "   en.authorisation = 1 and en.PLEncash > 0 and en.process = 1 and Year(en.fm)= " + Financial_md.Year + " and Month(en.fm) = " + Financial_md.Month + "  " +
                    " and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended')) as x order by x.emp_code;";

            }
            else if (p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords = "select * from (select distinct ded.emp_code, ded.fm, ded.fy, 'Adhoc' as status, atted.status as stpstatus from pr_emp_adhoc_det_field ded " +
                    " join pr_month_attendance atted on atted.emp_code = ded.emp_code " +
                    " where ded.emp_code in (" + empcodes + ") and ded.active = 1 and atted.active = 1 and atted.status in ('Regular', 'StopSalary') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + " " +
                    " union all" +
                    " select distinct em.EmpId as emp_code, en.fm, en.fy, 'Encashment' as status, atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance " +
                    " atted on atted.emp_code = em.EmpId  where em.empid in (" + empcodes + ") and   " +
                    "  en.authorisation = 1 and en.PLEncash > 0 and en.process = 1  and Year(en.fm)= " + Financial_md.Year + " and Month(en.fm) = " + Financial_md.Month + " " +
                    " and atted.active = 1 and atted.status in ('Regular', 'StopSalary','Suspended')) as x order by x.emp_code;";
            }
            else if (p_type.Contains("Regular"))
            {
                getallrecords = "select distinct atted.emp_code,atted.fm,atted.fy,status,status as stpstatus from pr_month_attendance atted join pr_emp_pay_field c on atted.emp_code = c.emp_code JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id " +
            " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and working_days> 0 and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and " +
             " status in ('Regular')and c.emp_code in  (" + empcodes + ") and pc.field_type = 'pay_fields' and " +
             " m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount> 0 and m.name = 'Basic' order by emp_code; ";
            }
            else if (p_type.Contains("stopsalary"))
            {
                getallrecords = "select distinct atted.emp_code,atted.fm,atted.fy,status,status as stpstatus from pr_month_attendance atted join pr_emp_pay_field c on atted.emp_code = c.emp_code JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id " +
            " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and working_days> 0 and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and " +
             " status in ('stopsalary')and c.emp_code in  (" + empcodes + ") and pc.field_type = 'pay_fields' and " +
             " m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount> 0 and m.name = 'Basic' order by emp_code; ";
            }
            else if (p_type.Contains("Adhoc"))
            {
                getallrecords = "select distinct ded.emp_code,atted.status as stpstatus ,ded.fm, ded.fy, 'Adhoc' as status from pr_emp_adhoc_det_field ded join pr_month_attendance atted on  atted.emp_code=ded.emp_code " +
                "where ded.emp_code in (" + empcodes + ") and ded.active=1 and atted.active=1 and atted.status in ('Regular','StopSalary','Suspended') and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and  Month(ded.fm) = " + Financial_md.Month + " and Year(ded.fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Encashment"))
            {
                getallrecords = "select distinct em.EmpId as emp_code,en.fm, en.fy, 'Encashment' as status," +
                    " atted.status as stpstatus from PLE_Type en join Employees em on en.empid = em.id join pr_month_attendance atted on atted.emp_code = em.EmpId " +
                    " where em.empid in (" + empcodes + ") and " +
                    "   en.authorisation=1 and en.PLEncash>0 and en.process=1" +
                    " and Year(en.fm)=" + Financial_md.Year + "" +
                    " and Month(en.fm) = " + Financial_md.Month + "" +
                    " and atted.active=1 and atted.status in ('Regular','StopSalary','Suspended');";
            }
            else if (p_type.Contains("Suspended"))
            {
                getallrecords = "select distinct atted.emp_code,atted.fm,atted.fy,status,status as stpstatus from pr_month_attendance atted join pr_emp_pay_field c on atted.emp_code = c.emp_code JOIN pr_earn_field_master m ON c.m_id = m.id join pr_payslip_customization pc on c.m_id = pc.m_id " +
            " where atted.emp_code in (" + empcodes + ") and atted.active = 1 and working_days> 0 and Month(atted.fm) = " + Financial_md.Month + " and Year(atted.fm)= " + Financial_md.Year + " and " +
             " status in ('Suspended') and c.emp_code in  (" + empcodes + ") and pc.field_type = 'pay_fields' and " +
             " m.type = 'pay_fields' and pc.cust_status = 'Yes' AND c.active = 1 AND amount> 0 and m.name = 'Basic' order by emp_code; ";
            }


            return getallrecords;
        }

        //Method to process Regular Payslip,Encashment,StopSalary,Suspended
        private async Task<string> Gen_Regular_Payslip(string empcoded, string empcode, string fm, DateTime Financial_md, string payslip_type, string Final_Process, string Sent_Mail)
        {
             string strfm = fm.Substring(4);
            DateTime Financial_mdloan = (_LoginCredential.FinancialMonthDate);
            string Financial_month1 = Financial_md.ToString("yyyy-MM-dd");
            DateTime PrevMonthStagIncr= Financial_md.AddMonths(-1);
            var MonthendDate = Financial_md.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            DateTime str = Convert.ToDateTime(Financial_mdloan);
            string dday = str.ToString("dd");
            sbqry = new StringBuilder();
            string E_designation = "";
            string str_prevmonth= PrevMonthStagIncr.ToString("yyyy-MM-dd");
            Boolean physical_handicap = false;
            string E_branch = "";
            decimal workingdays = 0;
            decimal Noofdays = 0;
            decimal lop_days = 0;
            string status = "";
            decimal Suspended_per = 0;
            decimal Suspended_per_val = 0;
            DateTime statusdate;
            int statusday = 0;
            string payfieldtype = "";
            decimal er_fld_amt = 0;
            decimal er_amount = 0;
            decimal er_amountEncahment = 0;
            int iFloan = iFY - 1;
            string SusPer_Qry = "";
            DataTable dtSuspensedEmployeesPrev = null;
            //decimal PleEncash = 0;

            //allowances
            decimal grossamount = 0;
            decimal HRA = 0;
            decimal DA = 0;
            decimal CEOAllowanceAmt = 0;
            decimal CEOAllowance = 0;
            decimal CCA = 0;
            decimal PT = 0;
            decimal PF = 0;
            decimal NPS = 0;
            decimal IntermRelief = 0;
            decimal TelanganaIncrement = 0;

            decimal incometax = 0;
            decimal club_subscription = 0;
            decimal telangana_officers_assc = 0;
            decimal deduction_amount = 0;
            decimal net_amount = 0;
            decimal basic_amount = 0;
            decimal vpf_deduction = 0;
            //deductionfields
            int ded_id = 0;
            string ded_name = " ";
            decimal dedinst_amount = 0;
            decimal dedinst_amount2 = 0;
            decimal dedint_amount = 0;
            decimal outintrest_amount = 0;
            decimal dedint_amount2 = 0;
            decimal intrest_amount = 0;
            decimal dedostanding_amount = 0;
            decimal total_loan = 0;
            decimal os_total_amount = 0;
            decimal RecAmt = 0;
            Boolean principal_flag = false;
            Boolean principal_recovered_flag = false;
            Boolean principlerecoveredflag_p1 = false;
            Boolean principlerecoveredflag_p1Int2 = false;
            Boolean principlerecoveredflag_p1Int = false;
            decimal dedostanding_amount2 = 0;
            decimal total_loan2 = 0;
            decimal RecAmt2 = 0;
            decimal c_deduction_amount = 0;
            string ded_type = "";
            decimal lop_amount = 0;
            //jaiibcaiibincr
            decimal JAIIBCAIIBIncr = 0;
            //allowamcefields
            int alw_id = 0;
            string alw_name = "";
            decimal alw_amount = 0;
            string alw_type = "";
            decimal FPIIP = 0;
            decimal Special_DA = 0;
            decimal Specila_Allw = 0;
            decimal Branchallowance_gen = 0;
            //pfcomponents
            decimal StagnationIncrement = 0;
            decimal AnnualIncrement = 0;
            decimal Special_Pay = 0;
            decimal FPA = 0;
            decimal SPL_Cashier = 0;
            decimal SPL_Driver = 0;
            decimal Faculty_Allowance = 0;
            decimal Qual_allowance = 0;
            decimal Watchmanallw = 0;
            decimal SPL_Jamedar = 0;
            decimal SPL_Dafter = 0;
            decimal SPL_Personal_Pay = 0;
            decimal SPL_Electrician = 0;
            decimal Special_Increment = 0;
            decimal SPL_Typist = 0;
            decimal SPL_Stenographer = 0;
            decimal SPL_Xerox_machine = 0;
            decimal vpf_Percentage = 0;
            int m_fm = 0;
            int y_fm = 0;
            int ifyloan = iFY - 1;

            string previousmonthstatus = "update pr_emp_payslip set active=0 where fm='" + str_prevmonth + "' and emp_code=" + empcode + "";
            bool b_previousmonthstatus = await _sha.Run_UPDDEL_ExecuteNonQuery(previousmonthstatus);

            //select id to know payslip record is finally processed or not
            payslip_final = await CheckFinalProcess(empcode, payslip_type, Financial_md);

            //if finalprocess is not done then do payslip process
            if (payslip_final == false)
            {

                //delete records from pr_emp_payslip_allowance, pr_emp_payslip_deductions, pr_emp_payslip
                if (payslip_type == "Regular" || payslip_type == "Suspended" || payslip_type == "StopSalary")
                {
                    getidfrompayslips = await _sha.Get_Table_FromQry("select id from pr_emp_payslip where emp_code=" + empcode + " and spl_type in ('stopsalary','Regular','Suspended') and month(fm)=" + dtFM + " and year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and active=1 ;");

                }
                else
                {
                    getidfrompayslips = await _sha.Get_Table_FromQry("select id from pr_emp_payslip where emp_code=" + empcode + " and spl_type='" + payslip_type + "' and month(fm)=" + dtFM + " and year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and active=1 ;");
                }

                if (getidfrompayslips.Rows.Count > 0)
                {
                    foreach (DataRow payslipid in getidfrompayslips.Rows)
                    {
                        //DataRow pid = getidfrompayslips.Rows[0];                        
                        ps_old_id = int.Parse(payslipid["id"].ToString());
                        sbqry.Append("delete from pr_emp_payslip_allowance where payslip_mid=" + ps_old_id + " ;");
                        sbqry.Append("delete from pr_emp_payslip_deductions where payslip_mid=" + ps_old_id + " ;");
                        sbqry.Append("delete from pr_emp_payslip where id=" + ps_old_id + " and month(fm)=" + dtFM + " and year(fm)=" + Financial_md.Year + ";");

                    }
                }
                //get number of days in month
                string qrynoofdays = " select month_days from pr_month_details where active = 1 ";
                //Get id,branchname,designation
                string qry1 = "SELECT m.Id,m.Name as designation,b.Name as bname FROM (select e.id,d.Name,e.Branch from employees e JOIN Designations d ON e.CurrentDesignation=d.id WHERE EmpId=" + empcode + ") m join Branches b ON m.Branch=b.Id;";

                //Get status,workingdays and lop_days
                //string qry2 = "SELECT format(status_date,'MM') as m_fm,format(status_date,'yyyy') as y_fm,status," +
                //    "format(status_date,'yyyy-MM-dd') as status_date,working_days,case when lop_days is null then 0 " +
                //    "else lop_days end as lop_days,sus_per FROM pr_month_attendance " +
                //    "WHERE emp_code=" + empcode + " AND active=1;";
                string qry2 = "SELECT format(status_date,'MM') as m_fm,format(status_date,'yyyy') as y_fm,status," +
                    "format(status_date, 'yyyy-MM-dd') as status_date,case when month(retirementdate)= month(a.fm) " +
                    "and year(retirementdate)= year(a.fm) then DAY(retirementdate) else working_days end as working_days," +
                    "case when lop_days is null then 0 else lop_days end as lop_days,sus_per FROM pr_month_attendance a " +
                    "join Employees e on e.empid = a.emp_code WHERE emp_code = " + empcode + "  AND active = 1; ";

                string qry3 = "";
                string qry8 = "";
                string qry9 = "";

                //chcek payslip type
                if (payslip_type == "Encashment")
                {
                    //Get basicamount for Regular PaySlip
                    qry3 = "SELECT m.name as payfieldtype,amount,c.m_id FROM pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id join pr_encashment_earnings_customization pc on c.m_id=pc.m_id WHERE emp_code=" + empcode + " and pc.field_type='pay_fields' and m.type='pay_fields' and pc.cust_status='Yes' AND c.active=1 AND amount>0 group by m.name,c.amount,c.m_id  ;";

                    //get emp allowance_general for Regular PaySlip
                    qry8 = "SELECT m.id,m.name,amount,c.m_type FROM pr_emp_allowances_gen c JOIN pr_allowance_field_master m ON m.id=c.m_id join pr_encashment_earnings_customization pc on c.m_id=pc.m_id WHERE emp_code=" + empcode + " and pc.field_type='EMPA' and m.type='EMPA' and pc.cust_status='Yes' AND amount>0  AND c.active=1 group by m.id,m.name,c.m_type,c.amount ;";

                    //get emp allowance_special for Regular PaySlip
                    qry9 = "SELECT m.id,m.name,amount,c.m_type FROM pr_emp_allowances_spl c JOIN pr_allowance_field_master m ON m.id=c.m_id  join pr_encashment_earnings_customization pc on c.m_id=pc.m_id WHERE emp_code=" + empcode + " and pc.field_type='EMPSA' and m.type='EMPSA' and pc.cust_status='Yes' AND amount>0  AND c.active=1 group by m.id,m.name,amount,c.m_type,emp_code ;";
                }
                else
                {

                    //Get basicamount for Regular PaySlip
                    qry3 = "SELECT  m.name as payfieldtype,amount,c.m_id FROM pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id join pr_payslip_customization pc on c.m_id=pc.m_id WHERE emp_code=" + empcode + " and pc.field_type='pay_fields' and m.type='pay_fields' and pc.cust_status='Yes' AND c.active=1 AND amount>0 group by m.name,amount,c.m_id  ;";

                    //get emp allowance_general for Regular PaySlip
                    qry8 = "SELECT  m.id,m.name,amount,c.m_type FROM pr_emp_allowances_gen c JOIN pr_allowance_field_master m ON m.id=c.m_id join pr_payslip_customization pc on c.m_id=pc.m_id WHERE emp_code=" + empcode + " and pc.field_type='EMPA' and m.type='EMPA' and pc.cust_status='Yes' AND amount>0  AND c.active=1 group by m.id,m.name,amount,c.m_type;";

                    //get emp allowance_special for Regular PaySlip
                    qry9 = "SELECT  m.id,m.name,amount,c.m_type FROM pr_emp_allowances_spl c JOIN pr_allowance_field_master m ON m.id=c.m_id  join pr_payslip_customization pc on c.m_id=pc.m_id WHERE emp_code=" + empcode + " and pc.field_type='EMPSA' and m.type='EMPSA' and pc.cust_status='Yes' AND amount>0  AND c.active=1 group by  m.id,m.name,amount,c.m_type;";

                }

                //get deductions from deductions table
                //string qry4 = "SELECT m.id,m.name,CONVERT(numeric(38,0), CAST(c.amount AS decimal)) as amount,c.m_type FROM pr_emp_deductions c JOIN pr_deduction_field_master m ON c.m_id=m.id WHERE emp_code=" + empcode + " AND c.active=1 AND amount>0;";
                //string qry4 = "SELECT m.id,m.name,c.amount  as amount,c.m_type FROM pr_emp_deductions c JOIN pr_deduction_field_master m ON c.m_id=m.id WHERE emp_code=" + empcode + " AND c.active=1 AND amount>0;";

                string qry4 = "SELECT c.fm,case when  m.name='COD_INS_PRM' then case " +
                    "when c.fm like '%"+ strfm + "' then c.amount else 0 end else c.amount end as amount," +
                    "m.id,m.name,c.m_type FROM pr_emp_deductions c " +
                    "JOIN pr_deduction_field_master m ON c.m_id = m.id WHERE emp_code = " + empcode + " " +
                    "AND c.active = 1 AND amount> 0;";
                //get hfc deduction details
                string qry5 = "SELECT id,amount,pay_type FROM pr_emp_hfc_details WHERE emp_code=" + empcode + " AND active=1 AND amount>0 and stop='No' ;";

                //get lic deduction details
                string qry6 = "SELECT id,amount,pay_type FROM pr_emp_lic_details WHERE emp_code=" + empcode + " AND active=1 AND amount>0 and stop='No' ;";

                //get loans deduction details
                //string qry7 = "SELECT m.id,m.loan_description,c.installment_amount,c.interest_installment_amount FROM pr_emp_adv_loans c JOIN pr_loan_master m ON c.loan_type_mid=m.id WHERE emp_code=" + empcode + " AND c.active=1 AND format(c.installment_start_date,'MM')<=" + dtFM.Month + " AND format(c.installment_start_date,'yyyy')<=" + dtFM.Year + ";";
                string qry7 = "SELECT c.id,m.loan_description,lc.os_total_amount,lc.loan_amount as totamt,lc.principal_amount_recovered as recamt,c.installment_amount,c.interest_installment_amount,lc.principal_recovered_flag,os_interest_amount, " +
                    // "os_principal_amount  " +
                    "CASE   WHEN os_principal_amount is null THEN 0   ELSE os_principal_amount END as os_principal_amount " +
                    "FROM pr_emp_adv_loans c JOIN pr_loan_master m " +
                    "ON c.loan_type_mid=m.id Join pr_emp_adv_loans_child lc on c.id=lc.emp_adv_loans_mid WHERE emp_code=" + empcode + " and lc.active=1  AND c.active=1 and lc.principal_recovered_flag = 0 and lc.interest_recovered_flag=0" +
                    "AND c.installment_start_date <='" + MonthendDate + "'  and lc.priority='1'; ";



                string qry14 = "";
                //get emp branch allowance
                string qry10 = "SELECT format(c.from_date,'yyyy-MM-dd') as from_date ,format(c.to_date,'yyyy-MM-dd') as to_date,m.name,m.description,m.amount FROM pr_emp_branch_allowances c JOIN pr_branch_allowance_master m ON c.allowance_mid=m.id WHERE emp_code=" + empcode + " AND c.active=1 AND format(c.from_date,'MM')<=" + dtFM + ";"; //todo - add fromdate <=month_details.month

                //get da_percentage
                string qry11 = "SELECT da_percent FROM pr_month_details WHERE active=1;";
                //get section and exemption amount limit 
                string qry12 = "select section,amount from  pr_emp_perdeductions where emp_code=" + empcode + "  and active=1";
                string qry13 = "select format(c.fm,'MM') as fm,c.m_id,c.amount,c.m_type from pr_emp_perearning c join pr_earn_field_master m on c.m_id=m.id where m.name like '%PFPerks%' and m.type='per_earn' and c.emp_code=" + empcode + " and Month(c.fm)=" + PreviousMonth_dtfm + " ;";
                string qry15 = "SELECT c.id,lc.loan_amount as totamt,lc.principal_recovered_flag,lc.interest_recovered_flag,lc.os_total_amount,lc.principal_amount_recovered as recamt,m.loan_description,c.installment_amount as inst,lc.os_principal_amount as osinst,c.interest_installment_amount as instrtinstl,lc.principal_amount_recovered " +
                    "FROM pr_emp_adv_loans c JOIN pr_loan_master m " +
                    "ON c.loan_type_mid=m.id Join pr_emp_adv_loans_child lc on c.id=lc.emp_adv_loans_mid WHERE emp_code=" + empcode + " and lc.active=1  AND c.active=1 and lc.principal_recovered_flag = 0 and lc.interest_recovered_flag = 0 " +
                    "AND  c.installment_start_date <='" + fm + "'  and lc.priority='2'; ";
                string qry16 = "select  distinct c.id,m.loan_description,c.interest_installment_amount as intrst from pr_emp_adv_loans c join pr_emp_adv_loans_child lc ON c.id = lc.emp_adv_loans_mid  join pr_loan_master m ON c.loan_type_mid = m.id" +
                               " where emp_code=" + empcode + "  and c.active = 1  " +
                               "AND  c.installment_start_date <='" + fm + "' and lc.principal_recovered_flag = 1 and lc.os_principal_amount = 0 and  lc.interest_recovered_flag = 0  ; ";
                string qry17 = "SELECT lc.principal_recovered_flag" +
                    " FROM pr_emp_adv_loans c JOIN pr_loan_master m " +
                    "ON c.loan_type_mid=m.id Join pr_emp_adv_loans_child lc on c.id=lc.emp_adv_loans_mid WHERE emp_code=" + empcode + " and lc.active=1  AND c.active=1 and lc.principal_recovered_flag=1" +
                    "AND  c.installment_start_date <='" + fm + "' and lc.priority='1' ; ";
                string Physicallyhandicappedqry = "select phy_handicapped from pr_emp_general where emp_code=" + empcode + "  and active=1";
                string ISNps = "select NPS from pr_emp_general where emp_code=" + empcode + "  and active=1";
                string str_StagnationIncrement = "select distinct g.all_name,g.all_amount FROM pr_emp_payslip_allowance g where payslip_mid=" +
                    " (select id from pr_emp_payslip where fm='"+str_prevmonth+"' and emp_code="+ empcode + " and spl_type='Regular') and  all_name='Stagnation Increments' ;";
                string str_stagnationincrementprocesscheck = "select [emp_code],[increment_type],[increment_amount],[increment_date],[process]from pr_emp_inc_anual_stag where process=1 and fm='"+ Financial_month1 + "' " +
                    "and increment_type='stagnation' and emp_code="+empcode+"";

                //get data from deputation
                //if (payslip_type == "Regular")
                //{

                //    qry14 = "select c.name,c.id,m.amount from pr_emp_deput_deduction_field m join pr_deduction_field_master c on m.m_id=c.id where m.active=1 and m.emp_code=" + empcode + " and Month(fm)=" + dtFM + " and Year(fm)=" + Financial_md.Year + ";";
                //}
                //get JAIIBCAIIBIncrements 
                //string qry13 = "SELECT * FROM pr_emp_jaib_caib_general WHERE emp_code=" + empcode + " AND format(incr_WEF_date,'MM')=" + dtFM.Month + " AND active=1 AND authorisation=1;";

                //get data from all queries
                DataSet ds = await _sha.Get_MultiTables_FromQry(qry1 + qry2 + qry3 + qry4 + qry5 + qry6 + qry7 + qry8 + qry9 + qry10 + qry11 + qry12 + qry13 + qry15 + qry16 + qry17 + qrynoofdays + Physicallyhandicappedqry+ str_StagnationIncrement+ str_stagnationincrementprocesscheck+ ISNps);

                var dtEmpdetails = ds.Tables[0];
                var dtEmpworkingdetails = ds.Tables[1];
                var dtEmpbasicdetails = ds.Tables[2];

                //deductions
                var dtEmpdeductiondetails = ds.Tables[3];
                var dtEmpHFCdeductiondetails = ds.Tables[4];
                var dtEmpLICdeductiondetails = ds.Tables[5];
                var dtEmpLoansdeductiondetails = ds.Tables[6];
                var dtEmpallowance_generaldetails = ds.Tables[7];
                var dtEmpallowance_specialdetails = ds.Tables[8];
                var dtEmpbranch_allowance = ds.Tables[9];
                var dtDa_slabs = ds.Tables[10];
                dtGetSections = ds.Tables[11];
                dtPFPerks = ds.Tables[12];
                var dtDa_priority2 = ds.Tables[13];
                DataTable dtIntreset_priority1 = ds.Tables[14];
                DataTable dtIntreset_priority2 = ds.Tables[15];
                DataTable dtnoofdays = ds.Tables[16];
                DataTable dtphysicalhandicap = ds.Tables[17];
                DataTable dtprevmonthstgincrement = ds.Tables[18];
                DataTable dtcurrmonthstagincrement = ds.Tables[19];
                DataTable dtnps = ds.Tables[20];
                //DataTable dtDeput_details = null;
                //if (payslip_type == "Regular")
                //{
                //    dtDeput_details = ds.Tables[13];
                //}
                //checking working days to do payslip process
                if (dtphysicalhandicap.Rows.Count > 0)
                {
                    DataRow ed = dtphysicalhandicap.Rows[0];                   
                    physical_handicap= Boolean.Parse(ed["phy_handicapped"].ToString());
                }
                    if (dtEmpworkingdetails.Rows.Count > 0)
                {

                    //employee id,designation,branch
                    if (dtEmpdetails.Rows.Count > 0)
                    {
                        DataRow ed = dtEmpdetails.Rows[0];
                        Eid = ed["id"].ToString();
                        E_designation = ed["designation"].ToString();
                        E_branch = ed["bname"].ToString();
                    }
                    DataRow ewd = dtEmpworkingdetails.Rows[0];
                    status = ewd["status"].ToString();
                    if (dtnoofdays.Rows.Count > 0)
                    {
                        DataRow days = dtnoofdays.Rows[0];
                        Noofdays = decimal.Parse(days["month_days"].ToString());
                    }


                    //paysliptype: Stopsalary,Enchasment Suspend,General,promotion
                    if (status != PrConstants.STOP_SALARY || payslip_type == "StopSalary" || payslip_type == PrConstants.ENCASHMENT)
                    {
                        if (payslip_type == PrConstants.SUSPENDED)
                        {
                            try
                            {
                                m_fm = int.Parse(ewd["m_fm"].ToString());
                                y_fm = int.Parse(ewd["y_fm"].ToString());
                            }
                            catch
                            {
                                Er_Message.Append("Please Add suspended date for this EmployeeID '" + empcode + "'");
                            }
                            if (m_fm == dtFM)
                            {

                                statusdate = Convert.ToDateTime(ewd["status_date"].ToString());
                                statusday = statusdate.Day;
                                workingdays = (statusday - new DateTime(iFY - 1, dtFM, 1).Day);
                                lop_days = (new DateTime(iFY - 1, dtFM, 1).AddMonths(1).AddDays(-1).Day - workingdays);
                            }
                            else if (y_fm <= iFY || (y_fm <= iFY && m_fm < dtFM))
                            {
                                Suspended_per = decimal.Parse(ewd["sus_per"].ToString());
                                SusPer_Qry = "select emp_code from pr_emp_payslip where emp_code='" + empcode + "' and month(fm)='" + m_fm + "' and year(fm)='" + y_fm + "' and spl_type in ('Suspended') ";
                                dtSuspensedEmployeesPrev = await _sha.Get_Table_FromQry(SusPer_Qry);
                                Suspended_per_val = Suspended_per / 100;
                                //statusdate = Convert.ToDateTime(ewd["status_date"].ToString());
                                //statusday = statusdate.Day;
                                workingdays = (new DateTime(iFY - 1, dtFM, 1).AddMonths(1).AddDays(-1).Day) / 2;
                                lop_days = (new DateTime(iFY - 1, dtFM, 1).AddMonths(1).AddDays(-1).Day - workingdays);
                            }

                        }
                        else if (payslip_type == PrConstants.ENCASHMENT)
                        {
                            string encashqry = "select Sum(isnull(cast(PLEncash as decimal),0)) as Amt from PLE_Type where EmpId =(select id from employees where empid=" + empcode + ")  and authorisation = 1 and  PLEncash<=30 and process = 1 and year(fm)=" + Financial_md.Year + " and Month(fm) = " + Financial_md.Month + ";";
                            DataTable dtencash = await _sha.Get_Table_FromQry(encashqry);
                            if (dtencash.Rows.Count > 0)
                            {
                                DataRow de = dtencash.Rows[0];
                                workingdays = decimal.Parse(de["Amt"].ToString());
                                if (workingdays == 15)
                                {
                                    workingdays = Noofdays / 2;
                                }
                                else if (workingdays == 14)
                                {
                                    workingdays = workingdays;
                                }
                                else if (workingdays == 29)
                                {
                                    workingdays = workingdays;
                                }
                                else
                                {
                                    workingdays = Noofdays;
                                }
                                lop_days = no_of_days_mnth - workingdays;


                            }
                        }
                        else if (payslip_type == PrConstants.REGULAR || payslip_type == "StopSalary" || payslip_type == "Deputation")
                        {
                            workingdays = Convert.ToInt32(ewd["working_days"]);
                            lop_days = Convert.ToInt32(ewd["lop_days"]);
                        }

                        decimal alw_spl_tds_lic = 0;

                        decimal prevstagincrement = 0;
                        decimal currstagincrement = 0;
                        if(dtprevmonthstgincrement.Rows.Count>0)
                        {
                            prevstagincrement = Convert.ToDecimal(dtprevmonthstgincrement.Rows[0]["all_amount"]);
                        }
                        if(dtcurrmonthstagincrement.Rows.Count>0)
                        {
                            currstagincrement = Convert.ToDecimal(dtcurrmonthstagincrement.Rows[0]["increment_amount"]);
                        }

                        //paysliptype: Stopsalary,Enchasment Suspend,General (process if working days>0)
                        if (workingdays > 0)
                        {
                            //employee payfields data pr_emp_payfields
                            foreach (DataRow ebd in dtEmpbasicdetails.Rows)
                            {
                                payfieldtype = ebd["payfieldtype"].ToString();
                                alw_id = Convert.ToInt32(ebd["m_id"]);

                                if (dtSuspensedEmployeesPrev != null)
                                {
                                    if (dtSuspensedEmployeesPrev.Rows.Count > 0)
                                    {
                                        er_fld_amt = decimal.Parse(ebd["amount"].ToString()) * Suspended_per_val;
                                        er_amount = er_fld_amt;
                                    }
                                }
                                else
                                {
                                    er_fld_amt = decimal.Parse(ebd["amount"].ToString());
                                    er_amount = (er_fld_amt / no_of_days_mnth) * workingdays;
                                }
                                if (payfieldtype == "Basic")
                                {
                                    string er_amt = er_amount.ToString("0.00");
                                    string[] eramt1 = er_amt.Split('.');
                                    int val1 = Convert.ToInt32(eramt1[1]);
                                    if (val1 != 00)
                                    {
                                        if (val1 <= 50)
                                        {
                                            er_amount = Math.Floor((decimal)Math.Round(er_amount, 2, MidpointRounding.AwayFromZero));
                                        }
                                        else if (val1 > 50)
                                        {
                                            er_amount = Math.Ceiling((decimal)Math.Round(er_amount, 2, MidpointRounding.AwayFromZero));
                                        }
                                        else
                                        {
                                            er_amount = (decimal)Math.Round(er_amount, 2, MidpointRounding.AwayFromZero);
                                        }
                                    }
                                    else
                                    {
                                        er_amount = (decimal)Math.Round(er_amount, 2, MidpointRounding.AwayFromZero);
                                    }
                                }
                                else if (payfieldtype == "Annual Increment")
                                {
                                    AnnualIncrement = Math.Floor((decimal)Math.Round(er_amount));
                                }
                                else
                                {
                                    er_amount = (decimal)Math.Round(er_amount, 2, MidpointRounding.AwayFromZero);
                                }
                                //er_amount = (decimal)Math.Round(er_amount,2,MidpointRounding.AwayFromZero);
                                //calculations hra,da,cca based on basic
                                if (payfieldtype == PrConstants.BASIC)
                                {
                                    basic_amount = er_amount;
                                    if(E_designation=="Attender")
                                    {
                                        er_fld_amt = er_amount;
                                    }
                                    
                                    if (payslip_type != PrConstants.ENCASHMENT)
                                    {
                                        CCA = CalcCCA(er_fld_amt, AnnualIncrement,E_designation, Convert.ToInt32(workingdays), Convert.ToInt32(no_of_days_mnth));
                                    }
                                    else if(payslip_type == PrConstants.ENCASHMENT)
                                    {
                                        er_amountEncahment = (er_fld_amt / no_of_days_mnth) * no_of_days_mnth;
                                        if (workingdays <= 16)
                                        {
                                            //CCA = CalcCCA(er_amountEncahment, E_designation,Convert.ToInt32(workingdays), Convert.ToInt32(no_of_days_mnth)) / 2;
                                            CCA = CalcCCA(er_amountEncahment, AnnualIncrement, E_designation, Convert.ToInt32(workingdays), Convert.ToInt32(no_of_days_mnth));
                                        }
                                        else
                                        {
                                            CCA = CalcCCA(er_fld_amt, AnnualIncrement, E_designation,Convert.ToInt32(workingdays), Convert.ToInt32(no_of_days_mnth));
                                        }
                                    }
                                    else
                                    {
                                        er_amountEncahment = (er_fld_amt / no_of_days_mnth) * no_of_days_mnth;
                                        if (workingdays <= 16)
                                        {
                                            CCA = CalcCCA(er_amountEncahment, AnnualIncrement, E_designation, Convert.ToInt32(workingdays), Convert.ToInt32(no_of_days_mnth)) / 2;
                                        }
                                        else
                                        {
                                            CCA = CalcCCA(er_fld_amt, AnnualIncrement, E_designation, Convert.ToInt32(workingdays), Convert.ToInt32(no_of_days_mnth));
                                        }
                                    }
                                }
                                else if (payfieldtype == PrConstants.TELANAGANA_INCREMENT)
                                {
                                    TelanganaIncrement = (decimal)Math.Round(er_amount, 2,MidpointRounding.AwayFromZero);
                                }
                                else if (payfieldtype == PrConstants.INTERIM_RELIEF)
                                {
                                    if (payfieldtype == "Interm Relief")
                                    {
                                        string er_amtir = er_amount.ToString("0.00");
                                        string[] eramtir1 = er_amtir.Split('.');
                                        int val1 = Convert.ToInt32(eramtir1[1]);
                                        if (val1 != 00)
                                        {
                                            if (val1 <= 50)
                                            {
                                                IntermRelief = Math.Floor((decimal)Math.Round(er_amount, 2, MidpointRounding.AwayFromZero));
                                            }
                                            else if (val1 > 50)
                                            {
                                                IntermRelief = Math.Ceiling((decimal)Math.Round(er_amount, 2, MidpointRounding.AwayFromZero));
                                            }
                                            else
                                            {
                                                IntermRelief = (decimal)Math.Round(er_amount, 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        else
                                        {
                                            IntermRelief = (decimal)Math.Round(er_amount, 2, MidpointRounding.AwayFromZero);
                                        }
                                    }
                                    else
                                    {
                                        IntermRelief = (decimal)Math.Round(er_amount, 2, MidpointRounding.AwayFromZero);
                                    }
                                    //IntermRelief = (decimal)Math.Round(er_amount, 2,MidpointRounding.AwayFromZero);
                                }
                                //if (payfieldtype == "Employee TDS")
                                //{
                                //    if (payslip_type == PrConstants.ENCASHMENT)
                                //    {
                                //        if (workingdays <= 16)
                                //        {
                                //            incometax = incometax = (decimal)Math.Round(er_fld_amt, 2) / 2;
                                //        }
                                //        else
                                //        {
                                //            incometax = incometax = (decimal)Math.Round(er_fld_amt, 2);
                                //        }

                                //    }
                                //    else
                                //    {
                                //        incometax = (decimal)Math.Round(er_fld_amt, 2);
                                //    }

                                //}
                                else if (payfieldtype == "Employee TDS" && payslip_type == PrConstants.ENCASHMENT && workingdays <= 16)
                                {
                                    decimal incometaxnew = 0;
                                    decimal incometaxnew1 = 0;
                                    string incometaxnew2 = "";
                                    incometaxnew = er_fld_amt/ no_of_days_mnth;
                                    incometaxnew1 = incometaxnew * workingdays;
                                    //incometax = incometax = (decimal)Math.Round(er_fld_amt, 2,MidpointRounding.AwayFromZero) / 2;
                                    incometax = incometax = (decimal)Math.Round(incometaxnew1, 0, MidpointRounding.AwayFromZero);
                                    incometaxnew2 = incometax.ToString() + ".00";
                                    incometax = Convert.ToDecimal(incometaxnew2);
                                }
                                else if (payfieldtype == "Employee TDS" && payslip_type == PrConstants.ENCASHMENT && workingdays > 16)
                                {
                                    incometax = (decimal)Math.Round(er_fld_amt, 2,MidpointRounding.AwayFromZero);
                                }
                                else if (payfieldtype == "Employee TDS" && payslip_type != PrConstants.ENCASHMENT)
                                {
                                    incometax = (decimal)Math.Round(er_fld_amt, 2,MidpointRounding.AwayFromZero);
                                }

                                else if (payfieldtype != PrConstants.BASIC && payfieldtype != PrConstants.TELANAGANA_INCREMENT && payfieldtype != PrConstants.INTERIM_RELIEF && payfieldtype != PrConstants.LOSS_OF_PAY)
                                {
                                    if (payfieldtype == PrConstants.SPECIAL_PAY)
                                    {
                                        Special_Pay = er_amount;
                                    }
                                    else if (payfieldtype == PrConstants.SPECIAL_INCREMENT)
                                    {
                                        Special_Increment = er_amount;
                                    }
                                    else if (payfieldtype == PrConstants.STAGNATION_INCREMENT)
                                    {
                                        if(er_amount==0)
                                        {
                                            er_amount = 0;
                                            er_amount += prevstagincrement + currstagincrement;
                                            StagnationIncrement = er_amount;
                                        }
                                        else
                                        {
                                            StagnationIncrement = er_amount;
                                        }
                                        //er_amount = 0;
                                        //er_amount += prevstagincrement+ currstagincrement;
                                        //StagnationIncrement = er_amount;
                                    }
                                    else if (payfieldtype == PrConstants.ANNUAL_INCREMENT)
                                    {
                                        AnnualIncrement = er_amount;
                                    }
                                    else if (payfieldtype == PrConstants.JAIIB_INCREMENT)
                                    {
                                        JAIIBCAIIBIncr = er_amount;
                                    }
                                    else if (payfieldtype == PrConstants.CAIIB_INCREMENT)
                                    {
                                        JAIIBCAIIBIncr = er_amount;
                                    }

                                    //Special Increment, Employee TDS, LIC Premium
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_payslip_allowance", NewNumIndex));
                                    qry = "Insert into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,all_amount,all_type," +
                                    "active,trans_id) values(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + alw_id + ",'" + payfieldtype + "'," + Math.Round(er_amount, 2) + ",'pay_fields',1,@transidnew);";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance", "@idnew" + NewNumIndex.ToString(), null));
                                    alw_spl_tds_lic += er_amount;
                                }
                            }

                            //allowancegeneral from pr_emp_allowances_gen
                            foreach (DataRow algen in dtEmpallowance_generaldetails.Rows)
                            {
                                NewNumIndex++;
                                sbqry.Append(GetNewNumStringArr("pr_emp_payslip_allowance", NewNumIndex));
                                alw_id = Convert.ToInt32(algen["id"]);
                                alw_name = algen["name"].ToString();
                                //alw_amount = decimal.Parse(algen["amount"].ToString());
                                //er_amount = (alw_amount / no_of_days_mnth) * workingdays;
                                if (dtSuspensedEmployeesPrev != null)
                                {
                                    if (dtSuspensedEmployeesPrev.Rows.Count > 0)
                                    {
                                        alw_amount = decimal.Parse(algen["amount"].ToString()) * Suspended_per_val;
                                        er_amount = alw_amount;
                                    }
                                }
                                else
                                {
                                    alw_amount = decimal.Parse(algen["amount"].ToString());
                                    er_amount = (alw_amount / no_of_days_mnth) * workingdays;
                                }

                                er_amount = (decimal)Math.Round(er_amount, 2,MidpointRounding.AwayFromZero);
                                alw_type = algen["m_type"].ToString();

                                //if (alw_name != "Medical Allowance" && alw_name != "Petrol & Paper" && alw_name != "Petrol & Paper 1")
                                //{
                                //}
                                if (alw_name == PrConstants.FPIIP)
                                {
                                    FPIIP = er_amount;
                                }
                                else if (alw_name == PrConstants.FPA)
                                {
                                    FPA = er_amount;
                                }
                                else if (alw_name == PrConstants.PERSONAL_QUAL_ALLOWANCE)
                                {
                                    Qual_allowance = er_amount;
                                }
                                else if (alw_name == PrConstants.BR_MANAGER_ALLOWANCE)
                                {
                                    Branchallowance_gen = er_amount;
                                }
                                alw_type = alw_type.Replace(" ", string.Empty);
                                grossamount += er_amount;
                                qry = "INSERT into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,all_amount,all_type," +
                                    "active,trans_id) VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + alw_id + ",'" + alw_name + "'," + Math.Round(er_amount, 2) + ",'" + alw_type + "',1,@transidnew);";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance", "@idnew" + NewNumIndex.ToString(), null));

                            }

                            //specialallowance from pr_emp_allowances_spl
                            foreach (DataRow alspl in dtEmpallowance_specialdetails.Rows)
                            {
                                NewNumIndex++;
                                sbqry.Append(GetNewNumStringArr("pr_emp_payslip_allowance", NewNumIndex));
                                alw_id = Convert.ToInt32(alspl["id"]);
                                //alw_amount = decimal.Parse(alspl["amount"].ToString());
                                //er_amount = (alw_amount / no_of_days_mnth) * workingdays;
                                if (dtSuspensedEmployeesPrev != null)
                                {
                                    if (dtSuspensedEmployeesPrev.Rows.Count > 0)
                                    {
                                        alw_amount = decimal.Parse(alspl["amount"].ToString()) * Suspended_per_val;
                                        er_amount = alw_amount;
                                    }
                                }
                                else
                                {
                                    alw_amount = decimal.Parse(alspl["amount"].ToString());
                                    er_amount = (alw_amount / no_of_days_mnth) * workingdays;
                                }
                                er_amount = (decimal)Math.Round(er_amount, 2,MidpointRounding.AwayFromZero);
                                alw_name = alspl["name"].ToString();
                                alw_type = alspl["m_type"].ToString();

                                if (alw_name == PrConstants.SPL_CASHIER)
                                {
                                    SPL_Cashier = er_amount;
                                }
                                else if (alw_name == PrConstants.SPL_WATCHMAN)
                                {
                                    Watchmanallw = er_amount;
                                }
                                else if (alw_name == PrConstants.SPL_JAMEDAR)
                                {
                                    SPL_Jamedar = er_amount;
                                }
                                else if (alw_name == PrConstants.SPL_DAFTER)
                                {
                                    SPL_Dafter = er_amount;
                                }
                                else if (alw_name == PrConstants.SPL_PERSONAL_PAY)
                                {
                                    SPL_Personal_Pay = er_amount;
                                }
                                else if (alw_name == PrConstants.SPL_ELECTRICIAN)
                                {
                                    SPL_Electrician = er_amount;
                                }
                                else if (alw_name == PrConstants.SPL_TYPIST)
                                {
                                    SPL_Typist = er_amount;
                                }
                                else if (alw_name == PrConstants.SPL_STENOGRAPHER)
                                {
                                    SPL_Stenographer = er_amount;
                                }
                                else if (alw_name == PrConstants.SPL_DUPLICATING_XEROX_MACHINE)
                                {
                                    SPL_Xerox_machine = er_amount;
                                }
                                else if (alw_name == PrConstants.SPL_DRIVER)
                                {
                                    SPL_Driver = er_amount;
                                }
                                else if (alw_name == PrConstants.FACULTY_ALLOWANCE)
                                {
                                    Faculty_Allowance = er_amount;
                                }
                                alw_type = alw_type.Replace(" ", string.Empty);
                                grossamount += er_amount;
                                qry = "INSERT into  pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,all_amount,all_type," +
                                    "active,trans_id) VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + alw_id + ",'" + alw_name + "'," + Math.Round(er_amount, 2) + ",'" + alw_type + "',1,@transidnew);";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance", "@idnew" + NewNumIndex.ToString(), null));
                            }

                            //hfc deductions from pr_emp_hfc_details
                            if (payslip_type != PrConstants.ENCASHMENT && (status != PrConstants.SUSPENDED))
                            {
                                foreach (DataRow ehdd in dtEmpHFCdeductiondetails.Rows)
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                                    ded_id = Convert.ToInt32(ehdd["id"]);
                                    c_deduction_amount = decimal.Parse(ehdd["amount"].ToString());
                                    ded_type = ehdd["pay_type"].ToString();
                                    ded_name = PrConstants.HFC;
                                    deduction_amount += c_deduction_amount;
                                    ded_type = ded_type.Replace(" ", string.Empty);
                                    qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                }

                                //lic deductions from pr_emp_lic_details
                                foreach (DataRow eldd in dtEmpLICdeductiondetails.Rows)
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                                    ded_id = Convert.ToInt32(eldd["id"]);
                                    c_deduction_amount = decimal.Parse(eldd["amount"].ToString());
                                    ded_type = eldd["pay_type"].ToString();
                                    ded_name = PrConstants.LIC;
                                    deduction_amount += c_deduction_amount;
                                    ded_type = ded_type.Replace(" ", string.Empty);
                                    qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + c_deduction_amount + ",'" + ded_type + "',1,@transidnew);";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                }
                                if (payslip_type != PrConstants.STOP_SALARY)
                                {
                                    //loans deductions from pr_emp_adv_loans
                                    //priority 1 prnc recover
                                    if (dtEmpLoansdeductiondetails.Rows.Count > 0)
                                    {
                                        foreach (DataRow elndd in dtEmpLoansdeductiondetails.Rows)
                                        {
                                            NewNumIndex++;
                                            sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                                            ded_id = int.Parse(elndd["id"].ToString());
                                            ded_name = elndd["loan_description"].ToString();
                                            dedinst_amount = decimal.Parse(elndd["installment_amount"].ToString());
                                            dedint_amount = decimal.Parse(elndd["interest_installment_amount"].ToString());
                                            outintrest_amount = decimal.Parse(elndd["os_interest_amount"].ToString()); ;
                                            dedostanding_amount = decimal.Parse(elndd["os_principal_amount"].ToString()); ;
                                            principal_recovered_flag = Boolean.Parse(elndd["principal_recovered_flag"].ToString());
                                            total_loan = decimal.Parse(elndd["totamt"].ToString());
                                            RecAmt = decimal.Parse(elndd["recamt"].ToString());
                                            os_total_amount = decimal.Parse(elndd["os_total_amount"].ToString());
                                            if (total_loan != RecAmt && dedostanding_amount != 0 && principal_recovered_flag == false)
                                            {
                                                c_deduction_amount = dedinst_amount;
                                                ded_type = "Loan";
                                                deduction_amount += c_deduction_amount;
                                                ded_type = ded_type.Replace(" ", string.Empty);
                                                qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                                    "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                                sbqry.Append(qry);
                                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                            }
                                        }
                                    }
                                    //priority 2 prnc recover
                                    if (dtDa_priority2.Rows.Count > 0)
                                    {
                                        foreach (DataRow elndds in dtDa_priority2.Rows)
                                        {
                                            NewNumIndex++;
                                            sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                                            ded_id = int.Parse(elndds["id"].ToString());
                                            ded_name = elndds["loan_description"].ToString();
                                            dedinst_amount2 = decimal.Parse(elndds["inst"].ToString());
                                            dedint_amount2 = decimal.Parse(elndds["instrtinstl"].ToString());
                                            dedostanding_amount2 = decimal.Parse(elndds["osinst"].ToString());
                                            total_loan2 = decimal.Parse(elndds["totamt"].ToString());
                                            RecAmt2 = decimal.Parse(elndds["recamt"].ToString());
                                            principal_flag = Boolean.Parse(elndds["principal_recovered_flag"].ToString());
                                            os_total_amount = decimal.Parse(elndds["os_total_amount"].ToString());
                                            string queryprinciple = "SELECT lc.principal_recovered_flag  " +
                   "FROM pr_emp_adv_loans c JOIN pr_loan_master m " +
                   "ON c.loan_type_mid=m.id Join pr_emp_adv_loans_child lc on c.id=lc.emp_adv_loans_mid WHERE emp_code=" + empcode + " and lc.active=1  AND c.active=1 " +
                   "AND ((format(c.installment_start_date, 'yyyy') < " + (iFY) + ") " +
                           " OR (format(c.installment_start_date,'yyyy')=" + (iFY) + "" +
                           " AND format(c.installment_start_date,'MM')<=" + dtFM + " or format(c.installment_start_date,'MM')>=" + dtFM + "))  and lc.priority='1' and lc.emp_adv_loans_mid=" + ded_id + "; ";
                                            DataTable principle = await _sha.Get_Table_FromQry(queryprinciple);
                                            if (principle.Rows.Count > 0)
                                            {
                                                DataRow pids = principle.Rows[0];
                                                principlerecoveredflag_p1 = Boolean.Parse(pids["principal_recovered_flag"].ToString());
                                            }

                                            if (principal_flag == false && principlerecoveredflag_p1 == true)
                                            {
                                                c_deduction_amount = dedinst_amount2;
                                                ded_type = "Loan";
                                                deduction_amount += c_deduction_amount;
                                                ded_type = ded_type.Replace(" ", string.Empty);
                                                qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                                    "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                                sbqry.Append(qry);
                                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                            }
                                        }
                                    }
                                    //interest recover
                                    if (dtIntreset_priority1.Rows.Count > 0)
                                    {

                                        foreach (DataRow elnddsi in dtIntreset_priority1.Rows)
                                        {

                                            if (Convert.ToDecimal(elnddsi["intrst"]) > 0)
                                            {
                                                NewNumIndex++;
                                                sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                                                ded_id = int.Parse(elnddsi["id"].ToString());
                                                ded_name = elnddsi["loan_description"].ToString();
                                                intrest_amount = decimal.Parse(elnddsi["intrst"].ToString());
                                                string queryprinciple = "SELECT lc.principal_recovered_flag  " +
                   "FROM pr_emp_adv_loans c JOIN pr_loan_master m " +
                   "ON c.loan_type_mid=m.id Join pr_emp_adv_loans_child lc on c.id=lc.emp_adv_loans_mid WHERE emp_code=" + empcode + " and lc.active=1  AND c.active=1 " +
                   "AND ((format(c.installment_start_date, 'yyyy') < " + (iFY) + ") " +
                           " OR (format(c.installment_start_date,'yyyy')=" + (iFY) + "" +
                           " AND format(c.installment_start_date,'MM')<=" + dtFM + " or format(c.installment_start_date,'MM')>=" + dtFM + "))  and lc.priority='1' and lc.emp_adv_loans_mid=" + ded_id + "; ";
                                                DataTable principle1 = await _sha.Get_Table_FromQry(queryprinciple);
                                                if (principle1.Rows.Count > 0)
                                                {
                                                    DataRow pids = principle1.Rows[0];
                                                    principlerecoveredflag_p1Int = Boolean.Parse(pids["principal_recovered_flag"].ToString());
                                                }
                                                string queryprinciple2 = "SELECT lc.principal_recovered_flag  " +
                   "FROM pr_emp_adv_loans c JOIN pr_loan_master m " +
                   "ON c.loan_type_mid=m.id Join pr_emp_adv_loans_child lc on c.id=lc.emp_adv_loans_mid WHERE emp_code=" + empcode + " and lc.active=1  AND c.active=1 " +
                   "AND ((format(c.installment_start_date, 'yyyy') < " + (iFY) + ") " +
                           " OR (format(c.installment_start_date,'yyyy')=" + (iFY) + "" +
                           " AND format(c.installment_start_date,'MM')<=" + dtFM + " or format(c.installment_start_date,'MM')>=" + dtFM + "))  and lc.priority='2' and lc.emp_adv_loans_mid=" + ded_id + "; ";
                                                DataTable principle2 = await _sha.Get_Table_FromQry(queryprinciple2);
                                                if (principle2.Rows.Count > 0)
                                                {
                                                    DataRow pids = principle2.Rows[0];
                                                    principlerecoveredflag_p1Int2 = Boolean.Parse(pids["principal_recovered_flag"].ToString());
                                                }
                                                if (principle2.Rows.Count <= 0)
                                                {
                                                    principlerecoveredflag_p1Int2 = true;
                                                }
                                                if (principlerecoveredflag_p1Int2 == true && principlerecoveredflag_p1Int == false)
                                                {
                                                    c_deduction_amount = intrest_amount;
                                                    ded_type = "Loan";
                                                    deduction_amount += c_deduction_amount;
                                                    ded_type = ded_type.Replace(" ", string.Empty);
                                                    qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                                        "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                                    sbqry.Append(qry);
                                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                                }
                                                if (principlerecoveredflag_p1Int2 == true && principlerecoveredflag_p1Int == true)
                                                {
                                                    c_deduction_amount = intrest_amount;
                                                    ded_type = "Loan";
                                                    deduction_amount += c_deduction_amount;
                                                    ded_type = ded_type.Replace(" ", string.Empty);
                                                    qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                                        "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                                    sbqry.Append(qry);
                                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            //HRA calculation
                            var hra_DA_amt = basic_amount + StagnationIncrement + SPL_Jamedar + SPL_Cashier + SPL_Dafter + SPL_Driver + Watchmanallw + JAIIBCAIIBIncr + SPL_Xerox_machine + AnnualIncrement + SPL_Personal_Pay+Qual_allowance+Special_Pay;
                            if (E_designation != "Managing Director")
                            {
                                HRA = CalcHRA(hra_DA_amt, E_designation);
                            }
                            //DA calculation
                            DA = CalcDA(hra_DA_amt, dtDa_slabs);
                            if (E_designation == "Managing Director")
                            {
                                CEOAllowanceAmt = basic_amount + DA;
                                CEOAllowance = CalcCEOAllowance(CEOAllowanceAmt, E_designation);
                            }

                            string isnps = dtnps.Rows[0]["NPS"].ToString();
                            //PF_calculation 

                            decimal PF_amt = basic_amount + DA + FPIIP + Special_Pay + FPA + SPL_Cashier + SPL_Driver + Qual_allowance + Watchmanallw + SPL_Dafter + SPL_Jamedar + SPL_Electrician + SPL_Personal_Pay + Special_Increment + SPL_Typist + SPL_Stenographer + SPL_Xerox_machine + StagnationIncrement + AnnualIncrement;
                                PF = CalcPF(PF_amt, isnps);
                            //NPS_calculation 
                            decimal NPS_amt = basic_amount + DA + FPIIP + Special_Pay + FPA + SPL_Cashier + SPL_Driver + Qual_allowance + Watchmanallw + SPL_Dafter + SPL_Jamedar + SPL_Electrician + SPL_Personal_Pay + Special_Increment + SPL_Typist + SPL_Stenographer + SPL_Xerox_machine + StagnationIncrement + AnnualIncrement;
                            NPS = CalcNPS(NPS_amt, isnps);
                            //deductions from pr_emp_deductions
                            foreach (DataRow edd in dtEmpdeductiondetails.Rows)
                            {

                                NewNumIndex++;
                                sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                                ded_id = Convert.ToInt32(edd["id"]);
                                ded_name = edd["name"].ToString();
                                c_deduction_amount = decimal.Parse(edd["amount"].ToString());

                                ded_type = edd["m_type"].ToString();
                                if (payslip_type != PrConstants.ENCASHMENT && status != PrConstants.SUSPENDED)
                                {
                                    if (ded_name == PrConstants.CLUB_SUBSCRIPTION)
                                    {
                                        club_subscription = c_deduction_amount;
                                       
                                        ded_type = ded_type.Replace(" ", string.Empty);
                                        qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                        sbqry.Append(qry);
                                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                    }
                                    else if (ded_name == PrConstants.TELANAGANA_OFFICERS_ASSC)
                                    {
                                        telangana_officers_assc = c_deduction_amount;
                                      
                                        ded_type = ded_type.Replace(" ", string.Empty);
                                        qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                        sbqry.Append(qry);
                                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                    }
                                    else if (ded_name != PrConstants.PF_CONTRIBUTION && ded_name != PrConstants.CLUB_SUBSCRIPTION && ded_name != PrConstants.TELANAGANA_OFFICERS_ASSC)
                                    {
                                        if (ded_name == PrConstants.VPF_DEDUCTION)
                                        {
                                            vpf_deduction = c_deduction_amount;
                                            ded_name = "VPF Deduction";
                                        }
                                        else if (ded_name == PrConstants.VPF_PERCENTAGE)
                                        {
                                            if (vpf_deduction == 0)
                                            {
                                                c_deduction_amount = Math.Ceiling((PF * c_deduction_amount) / 100);
                                                vpf_deduction = c_deduction_amount;
                                                ded_name = "VPF Deduction";
                                            }
                                        }
                                        //add amounts to deductionamount

                                        if (ded_name != PrConstants.VPF_PERCENTAGE && c_deduction_amount>0)
                                        {
                                            deduction_amount += c_deduction_amount;
                                            ded_type = ded_type.Replace(" ", string.Empty);
                                            qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                            "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                            sbqry.Append(qry);
                                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));

                                        }
                                        else if (ded_name == PrConstants.VPF_PERCENTAGE && vpf_Percentage > 0)
                                        {
                                            ded_name = ded_name = "VPF Deduction";
                                            deduction_amount += vpf_Percentage;
                                            ded_type = ded_type.Replace(" ", string.Empty);
                                            qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                            "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                            sbqry.Append(qry);
                                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                        }

                                    }
                                }
                                else if (payslip_type == PrConstants.ENCASHMENT || (status == PrConstants.SUSPENDED && (y_fm <= iFY || (y_fm <= iFY && m_fm < dtFM))))
                                {
                                    if (ded_name == PrConstants.VPF_DEDUCTION || ded_name == PrConstants.VPF_PERCENTAGE)
                                    {
                                        if (ded_name != PrConstants.VPF_PERCENTAGE)
                                        {
                                            if (workingdays == 30)
                                            {
                                                deduction_amount += c_deduction_amount;
                                                vpf_deduction = deduction_amount;
                                            }
                                            else
                                            {
                                                deduction_amount += c_deduction_amount / 2;
                                                vpf_deduction = deduction_amount;
                                            }
                                            ded_type = ded_type.Replace(" ", string.Empty);
                                            qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                            "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(vpf_deduction, 2) + ",'" + ded_type + "',1,@transidnew);";
                                            sbqry.Append(qry);
                                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));

                                        }
                                        else if (ded_name == PrConstants.VPF_PERCENTAGE && vpf_deduction == 0)
                                        {
                                            ded_name = "VPF Deduction";
                                            vpf_deduction = (PF * c_deduction_amount) / 100;
                                            deduction_amount += vpf_deduction;
                                            ded_type = ded_type.Replace(" ", string.Empty);
                                            qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                            "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(vpf_deduction, 2) + ",'" + ded_type + "',1,@transidnew);";
                                            sbqry.Append(qry);
                                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                        }
                                    }
                                }

                            }

                            //Special_Allowances Calculation 
                            decimal Get_spl_allw = (basic_amount + StagnationIncrement + JAIIBCAIIBIncr + AnnualIncrement + Special_Increment);
                            Specila_Allw = CalcSpl_Allw(Get_spl_allw, E_designation);

                            //Special_DA Calculation 
                            Special_DA = CalcSplDA(Specila_Allw, dtDa_slabs);

                            //PFperksCalculation
                            string getPfPerks = await CalcPFPerks(PF_amt, NewNumIndex, fm, empcode, payslip_type);

                            //total grossamount
                            grossamount += basic_amount + HRA + DA + CCA + TelanganaIncrement + IntermRelief + alw_spl_tds_lic + Special_DA + Specila_Allw + CEOAllowance;

                            //Prof.Tax(deduction)
                            if (payslip_type != PrConstants.ENCASHMENT && physical_handicap==false)
                            {
                                PT = CalcProfessionalTax(grossamount);
                            }


                            //if (payslip_type == "Regular")
                            //{

                            //    foreach (DataRow dep in dtDeput_details.Rows)
                            //    {
                            //        var dep_name = dep["name"].ToString();
                            //        var dep_amount = decimal.Parse(dep["amount"].ToString());
                            //        if (dep_name == PrConstants.IT)
                            //        {
                            //            incometax = incometax + dep_amount;
                            //        }
                            //        else if (dep_name == PrConstants.Provident_Fund)
                            //        {
                            //            PF = PF + dep_amount;
                            //        }
                            //    }
                            //}
                            //adding all deductions to deduction amount
                            if (payslip_type != PrConstants.ENCASHMENT)
                            {
                                //calc lop
                                lop_amount = (decimal)Math.Round((grossamount / workingdays) * lop_days, 2);
                                deduction_amount += PF +NPS+ PT + incometax + club_subscription + telangana_officers_assc;
                                net_amount = grossamount - deduction_amount;

                            }
                            else
                            {
                                deduction_amount += PF + NPS + PT + incometax + club_subscription + telangana_officers_assc;
                                net_amount = grossamount - deduction_amount;
                                workingdays = 0;
                            }


                            if (net_amount > 0)
                            {


                                processcount++;
                                if (total_count == processcount)
                                {
                                    seriveempstatus = 1;
                                }
                                //else if (processcount > total_count)
                                //{
                                //    processcount = total_count;
                                //}
                                else if (processcount > 0)
                                {
                                    seriveempstatus = 1;
                                }
                                if (dtSuspensedEmployeesPrev != null)
                                {
                                    if (dtSuspensedEmployeesPrev.Rows.Count > 0)
                                    {
                                        workingdays = 0;
                                    }
                                }
                                //string myTableRow = " ";
                                //string myTableRows = " ";
                                //myTableRow = empcode;
                                //myTableRows = myTableRows + myTableRow;

                                qry = "UPDATE pr_payroll_service_run set status=" + seriveempstatus + ", process_count =" + processcount + " WHERE id=" + srv_id + ";";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_payroll_service_run", srv_id.ToString(), null));

                                payslip_error_type = payslip_type;
                                //final insertion in pr_emp_payslip
                                string parentqry = "";

                                parentqry += GetNewNumString("pr_emp_payslip");
                                parentqry += "delete from pr_emp_payslip_netSalary where emp_code=" + empcode + " and month(fm)=" + dtFM + " and year(fm)=" + Financial_md.Year + " and spl_type='" + payslip_type + "';";
                                parentqry += "INSERT into pr_emp_payslip(id,gen_date,fy,fm,emp_id,emp_code,designation,branch,working_days,lop,er_basic," +
                                    "er_da,er_cca,er_hra,er_interim_relief,er_telangana_inc,gross_amount,dd_provident_fund,dd_income_tax,dd_prof_tax,dd_club_subscription,dd_telangana_officers_assn,deductions_amount,lop_amount,net_amount,active,trans_id,spl_da,spl_allw,spl_type,err_ceoallw,sentmail,downloadpdf,final_process,NPS) " +
                                    "VALUES(@idnew,getdate(),'" + iFY + "','" + Financial_md.Date.ToString("yyyy-MM-dd") + "'," + Eid + "," + empcode + ",'" + E_designation + "','" + E_branch + "'," + workingdays + "," +
                                    "" + lop_days + "," + basic_amount + "," + DA + "," + CCA + "," + HRA + "," + IntermRelief + "," + TelanganaIncrement + "," + Math.Round(grossamount, 2) + "," + PF + "," + incometax + "," + PT + "," + club_subscription + "," + telangana_officers_assc + "," + deduction_amount + "," + lop_amount + "," + net_amount + ",1,@transidnew," + Special_DA + "," + Specila_Allw + ",'" + payslip_type + "'," + CEOAllowance + "," + sm + ",0," + fp + ","+NPS+");";

                                parentqry += GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip", "@idnew", null);

                                try
                                {
                                    await _sha.Run_UPDDEL_ExecuteNonQuery(trnsqry + parentqry + sbqry.ToString() + getPfPerks);
                                }
                                catch (Exception ex)
                                {
                                    string emptymsg = "&";
                                    Er_Message.Append(ex.Message);
                                    Er_Message.Append(emptymsg);
                                    // _logger.Info(sbqry.ToString());
                                    _logger.Error(ex.Message);
                                    _logger.Error(ex.StackTrace);

                                }
                            }
                            if (net_amount < 0)
                            {

                                string parentqry = "";
                                parentqry += "delete from pr_emp_payslip_netSalary where emp_code=" + empcode + " and month(fm)=" + dtFM + " and year(fm)=" + Financial_md.Year + " and spl_type='" + payslip_type + "';";
                                parentqry += "delete from pr_emp_payslip where emp_code=" + empcode + " and month(fm)=" + dtFM + " and year(fm)=" + Financial_md.Year + " and spl_type='" + payslip_type + "';";
                                if (dtSuspensedEmployeesPrev != null)
                                {
                                    if (dtSuspensedEmployeesPrev.Rows.Count > 0)
                                    {
                                        workingdays = 0;
                                    }
                                }

                                parentqry += GetNewNumString("pr_emp_payslip_netSalary");

                                parentqry += "INSERT into pr_emp_payslip_netSalary(id,gen_date,fy,fm,emp_id,emp_code,designation,branch,working_days,lop,er_basic," +
                                    "er_da,er_cca,er_hra,er_interim_relief,er_telangana_inc,gross_amount,dd_provident_fund,dd_income_tax,dd_prof_tax,dd_club_subscription,dd_telangana_officers_assn,deductions_amount,lop_amount,net_amount,active,trans_id,spl_da,spl_allw,spl_type,err_ceoallw,sentmail,downloadpdf,final_process) " +
                                    "VALUES(@idnew,getdate(),'" + iFY + "','" + Financial_md.Date.ToString("yyyy-MM-dd") + "'," + Eid + "," + empcode + ",'" + E_designation + "','" + E_branch + "'," + workingdays + "," +
                                    "" + lop_days + "," + basic_amount + "," + DA + "," + CCA + "," + HRA + "," + IntermRelief + "," + TelanganaIncrement + "," + Math.Round(grossamount, 2) + "," + PF + "," + incometax + "," + PT + "," + club_subscription + "," + telangana_officers_assc + "," + deduction_amount + "," + lop_amount + "," + net_amount + ",1,@transidnew," + Special_DA + "," + Specila_Allw + ",'" + payslip_type + "'," + CEOAllowance + "," + sm + ",0," + fp + ");";

                                parentqry += GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_netSalary", "@idnew", null);

                                try
                                {
                                    await _sha.Run_UPDDEL_ExecuteNonQuery(trnsqry + parentqry + sbqry.ToString() + getPfPerks);
                                }
                                catch (Exception ex)
                                {
                                    string emptymsg = "&";
                                    Er_Message.Append(ex.Message);
                                    Er_Message.Append(emptymsg);
                                    // _logger.Info(sbqry.ToString());
                                    _logger.Error(ex.Message);
                                    _logger.Error(ex.StackTrace);

                                }
                            }
                        }


                    }
                }




            }

            return "E#Payslip Error # Final process is Completed for selected employee.";
        }

        //Method to process Adhoc payslip
        private async Task<string> Gen_Adhoc_Payslip(string empcoded, string empcode, string fm, DateTime Financial_md, string payslip_type, string Final_Process, string Sent_Mail)
        {

            sbqry = new StringBuilder();
            string E_designation = "";
            string E_branch = "";
            decimal workingdays = 0;
            decimal lop_days = 0;


            //decimal PleEncash = 0;

            //allowances
            decimal grossamount = 0;
            decimal HRA = 0;
            decimal DA = 0;
            decimal CCA = 0;
            decimal PT = 0;
            decimal PF = 0;
            decimal NPS = 0;
            decimal IntermRelief = 0;
            decimal TelanganaIncrement = 0;

            decimal incometax = 0;
            decimal club_subscription = 0;
            decimal telangana_officers_assc = 0;
            decimal deduction_amount = 0;
            decimal net_amount = 0;
            decimal basic_amount = 0;
            decimal CEOAllowance = 0;

            //deductionfields
            int ded_id = 0;
            string ded_name = "";
            decimal c_deduction_amount = 0;
            string ded_type = "";

            decimal lop_amount = 0;



            //allowamcefields
            int alw_id = 0;
            string alw_name = "";
            decimal alw_amount = 0;
            string alw_type = "";


            decimal Special_DA = 0;
            decimal Specila_Allw = 0;

           

            char[] delimiters = { ' ', ',' };
            string[] empcodesArray = empcoded.Split(delimiters);


            //select id to know payslip record is finally processed or not
            payslip_final = await CheckFinalProcess(empcode, payslip_type, Financial_md);

            if (payslip_final == false)
            {

                string qry_Get_Emp_Details = "SELECT m.Id,m.Name as designation,b.Name as bname FROM (select e.id,d.Name,e.Branch from employees e JOIN Designations d ON e.CurrentDesignation=d.id WHERE EmpId=" + empcode + ") m join Branches b ON m.Branch=b.Id;";
                //string qry_Get_FMs_Adhocdetails = "select Month(fm) as fm,Year(fm) as fy from pr_emp_adhoc_det_field where emp_code=" + empcode + " and active=1";
                DateTime Financial_Month;

                DataSet dsGet_EmpDetails_FMDetails = await _sha.Get_MultiTables_FromQry(qry_Get_Emp_Details);
                var dtEmployeeDetails = dsGet_EmpDetails_FMDetails.Tables[0];
                //var dtFM_AdhocDetails = dsGet_EmpDetails_FMDetails.Tables[1];

                //employee id,designation,branch
                if (dtEmployeeDetails.Rows.Count > 0)
                {
                    DataRow ed = dtEmployeeDetails.Rows[0];
                    Eid = ed["id"].ToString();
                    E_designation = ed["designation"].ToString();
                    E_branch = ed["bname"].ToString();
                }


                //if financial_month and financial_year exists then process adhoc

                Financial_Month = Convert.ToDateTime(fm);

                string qryGetAdhocEarningfields = "select c.m_id,m.name,c.amount,adhoc_type from pr_emp_adhoc_earn_field c join pr_earn_field_master m on c.m_id=m.id where c.emp_code=" + empcode + " and Month(fm)='" + Financial_Month.Month + "' and Year(fm)='" + Financial_Month.Year + "' and c.active=1;";
                string qryGetAdhocDeductionfields = "select c.m_id,m.name,c.amount,adhoc_type from pr_emp_adhoc_deduction_field c join pr_deduction_field_master m on c.m_id=m.id where c.emp_code=" + empcode + " and Month(fm)='" + Financial_Month.Month + "' and Year(fm)='" + Financial_Month.Year + "' and c.active=1;";
                string qryGetDateDedfields = "select format(fm,'yyyy-MM-dd') as ad_fm from pr_emp_adhoc_det_field where emp_code=" + empcode + " and Month(fm)='" + Financial_Month.Month + "' and Year(fm)='" + Financial_Month.Year + "' and  active=1;";

                //string qryGetAdhocContributionfields = "select c.m_id,m.name,c.amount from pr_emp_adhoc_contribution_field c join pr_contribution_field_master m on c.m_id=m.id where c.emp_code=" + empcode + "";
                DataSet dsGetAdhocData = await _sha.Get_MultiTables_FromQry(qryGetAdhocEarningfields + qryGetAdhocDeductionfields + qryGetDateDedfields);
                var AdhocEarningfields = dsGetAdhocData.Tables[0];
                var AdhocDeductionfields = dsGetAdhocData.Tables[1];
                var AdhocDatefields = dsGetAdhocData.Tables[2];

                if (AdhocDatefields.Rows.Count > 0)
                {
                    string adfm = "";
                    if (AdhocDatefields.Rows.Count > 0)
                    {
                        DataRow dtd = AdhocDatefields.Rows[0];
                        adfm = dtd["ad_fm"].ToString();
                        adhoc_dtfm = Convert.ToDateTime(adfm);//2019-06-12

                    }
                    string ad_fm = adhoc_dtfm.ToString("yyyy-MM-dd");

                    getidfrompayslip = await _sha.Get_Table_FromQry("select id from pr_emp_payslip where emp_code=" + empcode + " and spl_type='" + payslip_type + "' and format(fm,'MM')=" + adhoc_dtfm.Month + " AND format(fm,'yyyy')=" + adhoc_dtfm.Year + ";");

                    if (getidfrompayslip.Rows.Count > 0)
                    {
                        DataRow pid = getidfrompayslip.Rows[0];
                        ps_old_id = int.Parse(pid["id"].ToString());
                    }

                    //delete records from pr_emp_payslip_allowance,pr_emp_payslip_deductions,pr_emp_payslip
                    sbqry.Append("delete from pr_emp_payslip_allowance where payslip_mid=" + ps_old_id + " ;");
                    sbqry.Append("delete from pr_emp_payslip_deductions where payslip_mid=" + ps_old_id + " ;");
                    sbqry.Append("delete from pr_emp_payslip where id=" + ps_old_id + " and spl_type='" + payslip_type + "';");

                    //AdhocEarnings
                    foreach (DataRow aef in AdhocEarningfields.Rows)
                    {
                        alw_id = Convert.ToInt32(aef["m_id"]);
                        alw_name = aef["name"].ToString();
                        alw_amount = decimal.Parse(aef["amount"].ToString());
                        alw_type = aef["adhoc_type"].ToString();
                        //if (alw_name != "Medical Allowance" && alw_name != "Petrol & Paper" && alw_name != "Petrol & Paper 1")
                        //{
                        //}
                        if (alw_name == "Basic" || alw_name == "DA" || alw_name == "HRA" || alw_name == "Interm Relief" || alw_name == "Telangana Increment" || alw_name == "Spcl. DA" || alw_name == "Spl. Allow")
                        {
                            if (alw_name == "Basic")
                            {
                                basic_amount = alw_amount;
                            }
                            else if (alw_name == "DA")
                            {
                                DA = alw_amount;
                            }
                            else if (alw_name == "HRA")
                            {
                                HRA = alw_amount;
                            }
                            else if (alw_name == "Interm Relief")
                            {
                                IntermRelief = alw_amount;
                            }
                            else if (alw_name == "Telangana Increment")
                            {
                                TelanganaIncrement = alw_amount;
                            }
                            else if (alw_name == "Spcl. DA")
                            {
                                Special_DA = alw_amount;
                            }
                            else if (alw_name == "Spl. Allow")
                            {
                                Specila_Allw = alw_amount;
                            }

                        }
                        else
                        {
                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_payslip_allowance", NewNumIndex));

                            qry = "INSERT into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,all_amount,all_type," +
                            "active,trans_id) VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + alw_id + ",'" + alw_name + "'," + Math.Round(alw_amount, 2) + ",'" + alw_type + "',1,@transidnew);";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance", "@idnew" + NewNumIndex.ToString(), null));
                            grossamount += alw_amount;
                        }


                    }

                    grossamount += basic_amount + DA + HRA + CCA + IntermRelief + TelanganaIncrement + Specila_Allw + Special_DA;

                    decimal vpf_amount = 0;

                    
                    //AdhocDeductions
                    foreach (DataRow aef in AdhocDeductionfields.Rows)
                    {
                        ded_id = Convert.ToInt32(aef["m_id"]);
                        ded_name = aef["name"].ToString();
                        c_deduction_amount = decimal.Parse(aef["amount"].ToString());
                        ded_type = aef["adhoc_type"].ToString();
                        if (ded_name == "Prof. Tax" || ded_name == "Income Tax" || ded_name == "Provident Fund" || ded_name == "Club Subscription" || ded_name == "TELANGANA OFFICERS ASSN")
                        {
                            if (ded_name == "Prof. Tax")
                            {
                                PT = c_deduction_amount;
                            }
                            else if (ded_name == "Income Tax")
                            {
                                incometax = c_deduction_amount;
                            }
                            else if (ded_name == "Provident Fund")
                            {
                                PF = c_deduction_amount; 
                            }
                            else if (ded_name == "Club Subscription")
                            {
                                club_subscription = c_deduction_amount;
                            }
                            else if (ded_name == "TELANGANA OFFICERS ASSN")
                            {
                                telangana_officers_assc = c_deduction_amount;
                            }

                        }
                        else
                        {
                            if (ded_name == "VPF")
                            {
                                ded_name = "VPF Deduction";
                                vpf_amount = c_deduction_amount;
                            }
                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                            deduction_amount += c_deduction_amount;
                            ded_type = payslip_type.ToLower();
                            qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                            "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew", null));
                        }
                    }
                    string getPfPerks = "";
                    if (PF > 0)
                    {
                        //PFperksCalculation
                        getPfPerks = await CalcPFPerks(PF, NewNumIndex, fm, empcode, payslip_type);
                    }


                    deduction_amount += PT + PF + incometax + club_subscription + telangana_officers_assc;

                    //net amount
                    net_amount = grossamount - deduction_amount;

                    processcount++;

                    if (total_count == processcount)
                    {
                        seriveempstatus = 1;
                    }
                    //else if (processcount > total_count)
                    //{
                    //    processcount = total_count;
                    //}
                    else if (processcount > 0)
                    {
                        seriveempstatus = 1;
                    }

                    qry = "UPDATE pr_payroll_service_run set status=" + seriveempstatus + ", process_count=" + processcount + " WHERE id=" + srv_id + ";";
                    sbqry.Append(qry);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_payroll_service_run", srv_id.ToString(), null));

                    payslip_error_type = payslip_type;
                    string parentqry = "";

                    parentqry += GetNewNumString("pr_emp_payslip");

                    parentqry += "INSERT into pr_emp_payslip(id,gen_date,fy,fm,emp_id,emp_code,designation,branch,working_days,lop,er_basic," +
                        "er_da,er_cca,er_hra,er_interim_relief,er_telangana_inc,gross_amount,dd_provident_fund,dd_income_tax,dd_prof_tax,dd_club_subscription,dd_telangana_officers_assn,deductions_amount,lop_amount,net_amount,active,trans_id,spl_da,spl_allw,spl_type,err_ceoallw,sentmail,downloadpdf,final_process,NPS) " +
                        "VALUES(@idnew,getdate(),'" + iFY + "','" + adfm + "'," + Eid + "," + empcode + ",'" + E_designation + "','" + E_branch + "'," + workingdays + "," +
                        "" + lop_days + "," + basic_amount + "," + DA + "," + CCA + "," + HRA + "," + IntermRelief + "," + TelanganaIncrement + "," + grossamount + "," + PF + "," + incometax + "," + PT + "," + club_subscription + "," + telangana_officers_assc + "," + deduction_amount + "," + lop_amount + "," + net_amount + ",1,@transidnew," + Special_DA + "," + Specila_Allw + ",'" + payslip_type + "'," + CEOAllowance + "," + Sent_Mail + ",0," + Final_Process + ","+NPS+");";

                    parentqry += GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip", "@idnew", null);
                    try
                    {
                        await _sha.Run_UPDDEL_ExecuteNonQuery(trnsqry + parentqry + sbqry.ToString() + getPfPerks);
                    }
                    catch (Exception ex)
                    {
                        string emptymsg = "&";
                        Er_Message.Append(ex.Message);
                        Er_Message.Append(emptymsg);
                        // _logger.Info(sbqry.ToString());
                        _logger.Error(ex.Message);
                        _logger.Error(ex.StackTrace);

                    }


                }


            }
            return null;
        }

        //Method to Process and fill pr_emp_payslip table
        public async Task<string> Gen_PaySlip(int run_pkid)
        {

            string sRet = "";
            string PslipStatusStop = " ";
            string stopsalary = " ";
            string normal = " ";
            string myTableRow = " ";
            string StopmyTableRow = "";
            string payslip_status = " ";
            string queryStopSalary = " ";
            string Financial_month = "";
            string founderMinus1 = "";
            string StopfounderMinus1 = "";
            string employeecode = "";
            string Cemployeecode = "";
            string StopCemployeecode = "";
            DataTable dt_stop_salary = new DataTable();
            //trans_id
            trnsqry = new StringBuilder();
            trnsqry.Append(GenNewTransactionString());
            Er_Message = new StringBuilder();

            //Get the empids on service_run
            string qryEmpCodes = "";
            if (run_pkid == 0)
            {
                //GetSchedulerRecords
                //qryEmpCodes = "SELECT id,emp_codes,total_count,payslip_type,final_process,sent_mail FROM pr_payroll_service_run WHERE run_date_time between format(GETDATE(),'yyyy-MM-dd') and GETDATE() and active=1 order by run_date_time desc;";
                string currDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qryEmpCodes = "SELECT run_date_time, id,emp_codes,total_count,payslip_type, CompletedEmpCodes,final_process,sent_mail FROM pr_payroll_service_run WHERE run_date_time <= '" + currDate + "' and CompletedEmpCodes is null and active = 1 order by run_date_time desc; ";

            }
            else
            {
                //Get_When_Click_on_Now 
                qryEmpCodes = "SELECT id,emp_codes,total_count,payslip_type,final_process,sent_mail FROM pr_payroll_service_run WHERE id=" + run_pkid + ";";
            }
            //GetAllConstants
            string qryConstants = "SELECT constant, [value] FROM all_constants WHERE app_type='payroll' AND functionality='GenPayslip' AND active=1;";

            DataSet dsGetSchedulerRecords_AllConstants = await _sha.Get_MultiTables_FromQry(qryEmpCodes + qryConstants);

            DataTable dtSchedulerRecords = dsGetSchedulerRecords_AllConstants.Tables[0];
            _dtConstants = dsGetSchedulerRecords_AllConstants.Tables[1];
            try
            {

                string Str_fm_fy = await GetFm_And_FY();
                string[] strfmfy = Str_fm_fy.Split('^');

                //financial year,month,date from _LoginCredential
                iFY = Convert.ToInt32(strfmfy[0]);
                dtFM = Convert.ToInt32(strfmfy[1]);
                Financial_md = Convert.ToDateTime(strfmfy[2]);


                no_of_days_mnth = new DateTime(Financial_md.Year, dtFM, 1).AddMonths(1).AddDays(-1).Day;

                PreviousMonth_dtfm = dtFM - 1;

                //Get SchedulerRecords through pr_payroll_service_run table
                foreach (DataRow row in dtSchedulerRecords.Rows)
                {
                    processcount = 0;
                    srv_id = Convert.ToInt32(row["id"]);
                    empcodes = row["emp_codes"].ToString();
                    total_count = int.Parse(row["total_count"].ToString());
                    p_type = row["payslip_type"].ToString();
                    Final_Process = row["final_process"].ToString();
                    Sent_Mail = row["sent_mail"].ToString();
                    //if (Final_Process == "True")
                    //{
                    //    fp = "1";
                    //}
                    //if (Final_Process == "False")
                    //{
                    //    fp = "0";
                    //}
                    fp = Final_Process == "True" ? "1" : "0";

                    if (Sent_Mail == "True")
                    {
                        sm = "1";
                    }

                    //Get Active Employee_Codes Details of General,Suspended,StopSalary,Encashment,Adhoc through --GetActiveEmpCodes_GeneralAdhocEncashment-- Method
                    string Qry_GetActive_Employee_GeneralAdhocEncashment = GetActiveEmpCodes_GeneralAdhocEncashment(empcodes, p_type);

                    DataTable dtGet_Employee_PSType_Fm = null;
                    if (Qry_GetActive_Employee_GeneralAdhocEncashment != "")
                    {
                        dtGet_Employee_PSType_Fm = await _sha.Get_Table_FromQry(Qry_GetActive_Employee_GeneralAdhocEncashment);
                    }

                    if (dtGet_Employee_PSType_Fm.Rows.Count > 0)
                    {
                        //string myTableRows = " ";
                        //foreach (DataRow dtr in dtGet_Employee_PSType_Fm.Rows)
                        //{
                        //    myTableRow = dtr["emp_code"].ToString();
                        //    myTableRows = myTableRows + myTableRow + "," + " ";
                        //    founderMinus1 = myTableRows.Remove(myTableRows.Length - 2, 1);
                        //    Cemployeecode = founderMinus1;
                        //}

                        Cemployeecode = string.Join("," + " ", dtGet_Employee_PSType_Fm.Rows.Cast<DataRow>()
                                                .Select(x => x["emp_code"].ToString()));

                        foreach (DataRow dtr in dtGet_Employee_PSType_Fm.Rows)
                        {
                            try
                            {
                                employeecode = dtr["emp_code"].ToString();
                                DateTime Financial_month1 = Convert.ToDateTime(dtr["fm"].ToString());
                                Financial_month = Financial_month1.ToString("yyyy-MM-dd");
                                string Financial_year = dtr["fy"].ToString();
                                payslip_status = dtr["status"].ToString();
                                PslipStatusStop = dtr["stpstatus"].ToString();

                                if (PslipStatusStop == "StopSalary" && (payslip_status == "Adhoc" || payslip_status == "Encashment"))
                                {
                                    queryStopSalary = "select emp_code from pr_emp_payslip where emp_code in (" + employeecode + ") " +
                                        " and Month(fm)=" + Financial_md.Month + " and Year(fm)=" + Financial_md.Year + " and " +
                                        " active=1 and spl_type='stopsalary';";
                                    dt_stop_salary = await _sha.Get_Table_FromQry(queryStopSalary);
                                    if (dt_stop_salary.Rows.Count > 0)
                                    {
                                        foreach (DataRow dtr2 in dt_stop_salary.Rows)
                                        {
                                            stopsalary = dtr["emp_code"].ToString();
                                            StopmyTableRow = StopmyTableRow + stopsalary + "," + " ";
                                            StopfounderMinus1 = StopmyTableRow.Remove(StopmyTableRow.Length - 2, 1);
                                            StopCemployeecode = StopfounderMinus1;
                                            if (payslip_status == "Adhoc")
                                            {
                                                await Gen_Adhoc_Payslip(Cemployeecode, employeecode, Financial_month, Financial_md, payslip_status, fp, sm);
                                            }
                                            else
                                            {
                                                await Gen_Regular_Payslip(Cemployeecode, employeecode, Financial_month, Financial_md, payslip_status, fp, sm);
                                            }
                                        }
                                        //await GetCompletedCodes(Cemployeecode, fp);
                                    }
                                }
                                else
                                {
                                    if (payslip_status == "Adhoc")
                                    {
                                        await Gen_Adhoc_Payslip(Cemployeecode, employeecode, Financial_month, Financial_md, payslip_status, fp, sm);
                                    }
                                    else
                                    {
                                        await Gen_Regular_Payslip(Cemployeecode, employeecode, Financial_month, Financial_md, payslip_status, fp, sm);
                                    }

                                }
                                await GetCompletedCodes(Cemployeecode, fp);
                            }
                            catch (Exception ex1)
                            {
                                sRet += employeecode + " - " + ex1.Message + ", ";
                            }

                        }
                    }
                    if (run_pkid == 0)
                    {
                        try
                        {
                            DataTable dtNoAttNoBasic = await _sha.Get_Table_FromQry(GetQryForNoAttNoBasic(empcodes, p_type));
                            if (dtNoAttNoBasic.Rows.Count > 0)
                            {
                                sNoAttNoBasic = string.Join("," + Environment.NewLine, dtNoAttNoBasic.Rows.Cast<DataRow>()
                                                            .Select(x => x["empid"].ToString() + " - " + x["type"].ToString()));

                            }
                            DataTable dtnetamount = await _sha.Get_Table_FromQry(netamount(empcodes, p_type));

                            if (dtnetamount.Rows.Count > 0)
                            {
                                NetAmountlessthanzero = string.Join("," + Environment.NewLine, dtnetamount.Rows.Cast<DataRow>()
                                                                               .Select(x => x["emp_code"].ToString() + " - " + x["type"].ToString()));
                            }
                            if (dtNoAttNoBasic.Rows.Count > 0 || dtnetamount.Rows.Count > 0)
                            {
                                string qry = "update pr_payroll_service_run set  status=1 ";

                                if (sNoAttNoBasic != "" && NetAmountlessthanzero != "")
                                    qry += ",err_desc='" + (sNoAttNoBasic != "" ? (sNoAttNoBasic) : "") + (NetAmountlessthanzero != "" ? ("," + NetAmountlessthanzero) : "") + "' ";

                                else if (NetAmountlessthanzero != "")
                                    qry += ",err_desc='" + (NetAmountlessthanzero != "" ? (NetAmountlessthanzero) : "") + "' ";


                                else if (sNoAttNoBasic != "")
                                    qry += ",err_desc='" + (sNoAttNoBasic != "" ? (sNoAttNoBasic) : "") + "' ";

                                qry += " Where id=" + srv_id;
                                await _sha.Run_UPDDEL_ExecuteNonQuery(qry);

                                return "E#Payslip Error #There are some errors, please check process status.";
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }

            }
            catch (Exception e)
            {
                sRet += e.ToString();
                _logger.Error(e.Message);
                _logger.Error(e.StackTrace);
            }

            return sRet;
        }

        public async Task<string> GetCompletedCodes(string empcodes, string fp)
        {
            string EmployeeName = _LoginCredential.EmpShortName;

            string myTableRow = " ";
            string myTableRows = " ";
            string founderMinus1 = "";
            string Cemployeecode = "";
            string myTableRow1 = " ";
            string myTableRows1 = " ";
            string founderMinus11 = "";
            string Cemployeecode1 = "";
            string payslip_status = " ";
            string Financial_month = "";

            string employeecode = "";

            string qryEmpCodes = "";

            //Get Active Employee_Codes Details of General,Suspended,StopSalary,Encashment,Adhoc through --GetActiveEmpCodes_GeneralAdhocEncashment-- Method
            string Qry_GetActive_Employee_GeneralAdhocEncashment = GetActiveEmpCodes_GeneralAdhocEncashments(empcodes, p_type, fp);
            string Qry_GetActive_Employee_GeneralAdhocEncashment_spl_type = GetActiveEmpCodes_GeneralAdhocEncashment_spl_type(empcodes, p_type, fp);

            DataTable dtGet_Employee_PSType_Fm_allCode = null;
            DataTable dtGet_Employee_PSType_Fm_allCodes_spl = null;
            if (Qry_GetActive_Employee_GeneralAdhocEncashment != "")
            {
                dtGet_Employee_PSType_Fm_allCode = await _sha.Get_Table_FromQry(Qry_GetActive_Employee_GeneralAdhocEncashment);
            }
            if (Qry_GetActive_Employee_GeneralAdhocEncashment_spl_type != "")
            {
                dtGet_Employee_PSType_Fm_allCodes_spl = await _sha.Get_Table_FromQry(Qry_GetActive_Employee_GeneralAdhocEncashment_spl_type);
            }
            if (dtGet_Employee_PSType_Fm_allCode.Rows.Count > 0 && dtGet_Employee_PSType_Fm_allCodes_spl.Rows.Count > 0)
            {
                foreach (DataRow dtr in dtGet_Employee_PSType_Fm_allCode.Rows)
                {
                    myTableRow = dtr["emp_code"].ToString();
                    myTableRows = myTableRows + myTableRow + "," + " ";
                    founderMinus1 = myTableRows.Remove(myTableRows.Length - 2, 1);
                    Cemployeecode = founderMinus1;
                }
                foreach (DataRow dtr1 in dtGet_Employee_PSType_Fm_allCodes_spl.Rows)
                {
                    myTableRow1 = dtr1["spl_type"].ToString();
                    myTableRows1 = myTableRows1 + myTableRow1 + "," + " ";
                    founderMinus11 = myTableRows1.Remove(myTableRows1.Length - 2, 1);
                    Cemployeecode1 = founderMinus11;
                }
                if (Cemployeecode1.Contains("StopSalary"))
                {
                    Cemployeecode1 = "Stop Salary";
                }
                StringBuilder sbqrys = new StringBuilder();
                qrys = "UPDATE pr_payroll_service_run set CompletedEmpCodes='" + Cemployeecode + "',username='" + EmployeeName + "',payslip_type='" + Cemployeecode1 + "' WHERE id=" + srv_id + ";";
                sbqrys.Append(qrys);
                //sbqrys.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_payroll_service_run", srv_id.ToString(), null));

                await _sha.Run_UPDDEL_ExecuteNonQuery(sbqrys.ToString());
            }

            return null;
        }

        private string GetActiveEmpCodes_GeneralAdhocEncashments(string empcodes, string p_type, string fp)
        {
            if (p_type.Contains("Regular") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecordss = " select emp_code,spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Regular', 'Encashment', 'Adhoc') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Encashment', 'Adhoc', 'Suspended') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " order by emp_code asc;";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Adhoc"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Regular', 'Adhoc') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and  Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Adhoc', 'Suspended') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and  Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Encashment"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Regular', 'Encashment') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Encashment"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Encashment', 'Suspended') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            else if (p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Encashment', 'Adhoc') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            else if (p_type.Contains("Regular"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Regular') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            else if (p_type.Contains("Suspended"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Suspended') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            else if (p_type.Contains("stopsalary"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            else if (p_type.Contains("Adhoc"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Adhoc') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + "and Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            else if (p_type.Contains("Encashment"))
            {
                getallrecordss = " select emp_code, spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Encashment') and Month(fm) = " + Financial_md.Month + "  and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " order by emp_code asc ;";
            }
            return getallrecordss;
        }
        private string GetActiveEmpCodes_GeneralAdhocEncashment_spl_type(string empcodes, string p_type, string fp)
        {
            if (p_type.Contains("Regular") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Regular', 'Encashment', 'Adhoc') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Encashment', 'Adhoc', 'Suspended') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Adhoc"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Regular', 'Adhoc') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and  Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Adhoc', 'Suspended') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and  Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Encashment"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Regular', 'Encashment') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Encashment"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Encashment', 'Suspended') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Encashment', 'Adhoc') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Regular"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Regular') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Suspended"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Suspended') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("stopsalary"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Adhoc"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Adhoc') and Month(fm) = " + Financial_md.Month + " and final_process=" + fp + "and Year(fm)= " + Financial_md.Year + " ;";
            }
            else if (p_type.Contains("Encashment"))
            {
                getallrecordsss = " select distinct spl_type from pr_emp_payslip where emp_code in (" + empcodes + ") and active=1 and spl_type in ('Encashment') and Month(fm) = " + Financial_md.Month + "  and final_process=" + fp + " and Year(fm)= " + Financial_md.Year + " ;";
            }

            return getallrecordsss;
        }
        public string netamount(string empcodes, string p_type)
        {
            ////qry = "select net_amount from pr_emp_payslip where where emp_code in (" + empcodes + ") and status in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 ";
            //string[] test = result; // Alternative array creation syntax 
            //string empcodes = String.Join(" ", test);           
            if (p_type.Contains("Regular") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Regular','Adhoc','Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Suspended','Adhoc','Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Adhoc"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Regular','Adhoc')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";

            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Suspended','Adhoc')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";

            }
            else if (p_type.Contains("Regular") && p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Regular','Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Suspended','Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";

            }
            else if (p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Adhoc','Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Regular"))

            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary  where emp_code in (" + empcodes + ") and spl_type in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";

            }
            else if (p_type.Contains("Suspended"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Suspended')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("stopsalary"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Adhoc"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Adhoc')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            else if (p_type.Contains("Encashment"))
            {
                getallrecords12 = "select  emp_code,'Deduction Amount is greater than Gross Amount' as type from pr_emp_payslip_netSalary where  emp_code in (" + empcodes + ") and spl_type in ('Encashment')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 and net_amount<0";
            }
            return getallrecords12;
        }
        //this method is used to get the employee who are not having attendance
        private string GetQryForNoAttNoBasic(string empcodes, string p_type)
        {
            //string[] test = result; // Alternative array creation syntax 
            //string empcodes = String.Join(" ", test);
            if (p_type.Contains("Regular") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Regular' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance-Regular' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No-Basic' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic' and amount>0" +
                                   " union all " +
                                   " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   " union all " +
                                   " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ")  " +
                                   " except " +
                                   " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                   " join Employees em on en.empid = em.id " +
                                   " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                   " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   ") as x order by empid";
            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Suspended' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                    " select distinct emp_code, 'No Attendance-Suspended' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                    " union all " +
                                    " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                   " select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                    " union all " +
                                    " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                    " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                    " union all " +
                                    " select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                    " select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                    " union all " +
                                    " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ")  " +
                                    " except " +
                                    " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                    " join Employees em on en.empid = em.id " +
                                    " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                    " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                    ") as x order by empid";
            }
            else if (p_type.Contains("Regular") && p_type.Contains("Adhoc"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Regular' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance-Regular' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                  " select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                   " union all " +
                                   " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   ") as x order by empid";

            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Adhoc"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Suspended' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance-Suspended' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                   " union all " +
                                   " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                   ") as x order by empid";

            }
            else if (p_type.Contains("Regular") && p_type.Contains("Encashment"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Regular' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance-Regular' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                   " union all " +
                                   " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                    " join Employees em on en.empid = em.id " +
                                    " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                   " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                    ") as x order by empid";

            }
            else if (p_type.Contains("Suspended") && p_type.Contains("Encashment"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance-Suspended' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance-Suspended' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                   " union all " +
                                   " select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                   " union all " +
                                   " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ") " +
                                   " except " +
                                   " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                    " join Employees em on en.empid = em.id " +
                                    " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                   " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                    ") as x order by empid";

            }
            else if (p_type.Contains("Adhoc") && p_type.Contains("Encashment"))
            {
                getallrecords1 = "select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                     " except " +
                                     " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                     " union all " +
                                     " select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                     " except " +
                                     " select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                     " union all " +
                                     " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                     " except " +
                                    " select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                     " union all " +
                                     " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ")  " +
                                     " except " +
                                     " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                     " join Employees em on en.empid = em.id " +
                                     " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                     " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + "  and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped'  " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                     " ) as x order by empid";
            }
            else if (p_type.Contains("Regular"))

            {
                getallrecords1 = "select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                    "except " +
                                    "select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Regular')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                    "union all " +
                                    "select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                    "except " +
                                    "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic'  and amount>0" +
                                    ") as x order by empid";

            }
            else if (p_type.Contains("Suspended"))
            {
                getallrecords1 = "select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                     "union all " +
                                     "select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic' and amount!=0 and amount>0" +
                                     ") as x order by empid";
            }
            else if (p_type.Contains("stopsalary"))
            {
                getallrecords1 = "select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                     "union all " +
                                     "select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic' and amount!=0 and amount>0" +
                                     ") as x order by empid";
            }
            else if (p_type.Contains("Adhoc"))
            {
                getallrecords1 = "select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                     "union all " +
                                     "select empid, 'No Basic-(Adhoc)' as type from employees where empid in (" + empcodes + ") " +
                                     "except " +
                                     "select distinct emp_code, 'No Basic-(Adhoc)' as type  from pr_emp_adhoc_det_field where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                     " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and active=1 and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                     ") as x order by empid";
            }
            else if (p_type.Contains("Encashment"))
            {
                getallrecords1 = " select * from (select empid, 'No Attendance' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                    " select distinct emp_code, 'No Attendance' as type  from pr_month_attendance where emp_code in (" + empcodes + ") and status in ('Suspended','Regular','stopsalary')  and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and active=1 " +
                                    " union all " +
                                    " select empid, 'No Basic' as type from employees where empid in (" + empcodes + ") " +
                                    " except " +
                                    "select distinct c.emp_code,'No Basic' as type  from pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id and m.type='pay_fields' where emp_code in (" + empcodes + ") and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " and m.name='Basic' and amount!=0 and amount>0" +
                                    " union all " +
                                    " select empid, 'No PlEncash Amount' as type from employees where empid in (" + empcodes + ")  " +
                                    " except " +
                                    " select distinct em.EmpId as emp_code,'No PlEncash Amount' as type from PLE_Type en " +
                                    " join Employees em on en.empid = em.id " +
                                    " where em.EmpId in (" + empcodes + ") and en.authorisation=1 and en.PLEncash>0 and en.process=1  and Year(en.fm)=" + Financial_md.Year + " " +
                                    " union all " +
                                     " select emp_code, 'Salary is Stopped' as type from pr_month_attendance " +
                                      "where emp_code in (" + empcodes + ") and status in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + "  " +
                                      "except " +
                                      "select distinct emp_code, 'Salary is Stopped' " +
                                      "as type  from pr_emp_payslip where emp_code in (" + empcodes + ") and spl_type in ('stopsalary') and Month(fm) = " + Financial_md.Month + " and Year(fm)= " + Financial_md.Year + " " +
                                    ") as x order by empid";
            }
            return getallrecords1;
        }

    }

}

