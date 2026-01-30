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
    public class JAIIBCAIIBService : BusinessBase
    {
        log4net.ILog _logger = null;
        public JAIIBCAIIBService(LoginCredential loginCredential,log4net.ILog logger) : base(loginCredential)
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

        public async Task UpdateIncrementAmount()
        {
            int FY = DateTime.Now.Year + 1;
            string FM = DateTime.Now.ToString("MM-dd-yyyy");

            string Incr_WEFdate = "";
            string Emp_code = "";
            string No_of_incr = "";
            string Incr_type = "";
            string incrementamt = "";
            string jcid = "";
            string Basicamount = "";

            string m_id = "";
            string m_type = "";
            string Id = "";
            string EmpId = "";
            string qry = "";

            //int NewNumIndex = 0;
            StringBuilder sbqry = null;

            //get all emp jaiib/caiib for today date
            string qrygetincramoutdetails = "select id,emp_code, incr_incen_type, no_of_inc, incr_WEF_date,Incrementamt from pr_emp_jaib_caib_general " +
                "where incr_WEF_date = format(GETDATE(), 'yyyy-MM-dd') and active = 1 and authorisation = 1";
            DataTable dtincr = await _sha.Get_Table_FromQry(qrygetincramoutdetails);

            foreach (DataRow dr_incr in dtincr.Rows) //process every record one by one
            {
                sbqry = new StringBuilder();
                //trans_id
                sbqry.Append(GenNewTransactionString());

                Emp_code = dr_incr["emp_code"].ToString();
                //Emp_code = "452";
                No_of_incr = dr_incr["no_of_inc"].ToString();
                Incr_type = dr_incr["incr_incen_type"].ToString();
                Incr_WEFdate = dr_incr["incr_WEF_date"].ToString();
                incrementamt = dr_incr["Incrementamt"].ToString();
                jcid = dr_incr["id"].ToString();

                //////Get basicamount from pr_emp_pay_field
                ////string qrybasicamount = "select c.emp_id,c.id,c.m_id,c.m_type,c.amount from pr_earn_field_master m " +
                ////    "join pr_emp_pay_field c on m.id=c.m_id where m.name='Basic' and c.emp_code=" + Emp_code + " and c.active=1";

                ////DataTable dtbasic = await _sha.Get_Table_FromQry(qrybasicamount);

                ////if (dtbasic.Rows.Count > 0)
                ////{
                ////    DataRow row = dtbasic.Rows[0];

                ////    Basicamount = row["amount"].ToString();
                ////    Id = row["id"].ToString();
                ////    m_id = row["m_id"].ToString();
                ////    m_type = row["m_type"].ToString();
                ////    EmpId = row["emp_id"].ToString();
                ////}

                ////int n_incr_amount = Convert.ToInt32(No_of_incr) * Convert.ToInt32(incrementamt);
                //////int currentbasic = Convert.ToInt32(Basicamount) + n_incr_amount;

                ////deactive existing emp pay fields 
                //qry = "Update pr_emp_pay_field set active=0, trans_id=@transidnew where emp_code=" + Emp_code + "" +
                //    " and m_id=" + m_id + ";";
                //sbqry.Append(qry);
                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_pay_field", Id, ""));

                //inserting to pr_emp pay fields with new basic amount
                if (Incr_type == "CAIIB")
                {
                    string getqry = "select id,name,type from pr_earn_field_master where name like '%CAIIB%' and type='pay_fields';";
                    DataTable dt = await _sha.Get_Table_FromQry(getqry);
                    if (dt.Rows.Count > 1)
                    {
                        DataRow efm = dt.Rows[0];
                        m_id = efm["id"].ToString();
                        m_type = efm["type"].ToString();
                    }
                    sbqry.Append(GetNewNumString("pr_emp_pay_field"));
                    qry = "Insert into pr_emp_pay_field (id,emp_id,emp_code,fy,fm,m_id,m_type,amount,active,trans_id) " +
                        "values(@idnew," + EmpId + "," +
                        "" + Emp_code + ",'" + FY + "','" + FM + "',"+ m_id + ",'"
                        + m_type + "'," + incrementamt + ",1,@transidnew);";
                    sbqry.Append(qry);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_pay_field", "@idnew", ""));
                }
                else if(Incr_type == "JAIIB")
                {
                    string getqry = "select id,name,type from pr_earn_field_master where name like '%JAIIB%' and type='pay_fields';";
                    DataTable dt = await _sha.Get_Table_FromQry(getqry);
                    if (dt.Rows.Count > 1)
                    {
                        DataRow efm = dt.Rows[0];
                        m_id = efm["id"].ToString();
                        m_type = efm["type"].ToString();

                    }
                    sbqry.Append(GetNewNumString("pr_emp_pay_field"));
                    qry = "Insert into pr_emp_pay_field (id,emp_id,emp_code,fy,fm,m_id,m_type,amount,active,trans_id) " +
                        "values(@idnew," + EmpId + "," +
                        "" + Emp_code + ",'" + FY + "','" + FM + "'," + m_id + ",'"
                        + m_type + "'," + incrementamt + ",1,@transidnew);";
                    sbqry.Append(qry);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_pay_field", "@idnew", ""));
                }
                

                //deactive JAIIB/CAIIB authorize record 
                qry = "Update pr_emp_jaib_caib_general set active=0, trans_id=@transidnew where emp_code=" + Emp_code + "" +
                    " and active = 1 and authorisation = 1;";
                sbqry.Append(qry);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_jaib_caib_general", jcid, ""));

                try
                {
                    await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                }
                catch(Exception ex)
                {
                    _logger.Info(sbqry.ToString());
                    _logger.Error(ex.Message);  
                    _logger.Error(ex.StackTrace);
                }

            }
        }

        DataTable _dtConstants = null;
        DataTable _incometaxdbconstant = null;
        float rem_taxexemption = 0;
        int iFY;
        DateTime dtFM;

        DataTable dtGetSections;
      
        public float CalcHRA(float er_Amt, string E_designation)
        {
            float ret = 0;
            float HRAOthersPercentage = float.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "HRAOthersPercentage")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            float HRAStaffAsstAttenderPercentage = float.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "HRAStaffAsstAttenderPercentage")
                .Select(x => x["value"].ToString()).FirstOrDefault());


            if (E_designation == "Staff Assistant" || E_designation == "Attender")
            {
                ret = er_Amt * HRAStaffAsstAttenderPercentage;
            }
            else
            {
                ret = er_Amt * HRAOthersPercentage;
            }
            return ret;
        }

        private float CalcDA(float er_Amt, DataTable dtDa_slabs)
        {
            float retVal = 0;
            if (dtDa_slabs.Rows.Count > 0)
            {
                DataRow dsl = dtDa_slabs.Rows[0];
                float da_slab = float.Parse(dsl["da_percent"].ToString());
                retVal = (da_slab / 100) * er_Amt;
            }
            return retVal;
        }

        public float CalcCCA(float er_Amt, string Designation)
        {
            float ret = 0;

            //0.04
            string CCAPercentage = _dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "CCAPercentage")
                .Select(x => x["value"].ToString()).FirstOrDefault();

            //400
            float MinCCAConditionalAmt = float.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "MinCCAConditionalAmt")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            //470
            float MidCCAConditionalAmt = float.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "MidCCAConditionalAmt")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            //870
            float MaxCCAConditionalAmt = float.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "MaxCCAConditionalAmt")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            var n_CCA = er_Amt * float.Parse(CCAPercentage);
            if (Designation == "Attender" || Designation == "Attender Cum Watchman" || Designation == "Attender/J.C" || Designation == "Civil Engg Supervisor" || Designation == "Driver" || Designation == "Stenographer" || Designation == "Telephone Operator Cum Receptionist" || Designation == "Typist" || Designation == "Watchman" || Designation == "Subordinate Staff(Substaff)" || Designation == "Junior Clerk")
            {
                if (n_CCA <= MinCCAConditionalAmt)
                    ret = Convert.ToInt32(n_CCA);
                else
                    ret = MinCCAConditionalAmt;
            }
            else if (Designation == "Staff Assistant" || Designation == "JR Staff Assistant" || Designation == "Staff Assistant Cum Assistant Cashier" || Designation == "JR Staff Assistant")
            {
                if (n_CCA <= MidCCAConditionalAmt)
                    ret = Convert.ToInt32(n_CCA);
                else
                    ret = MidCCAConditionalAmt;
            }
            else if (Designation == "Managing Director" || Designation == "Chief General Manager" || Designation == "General Manager" || Designation == "Deputy General Manager" || Designation == "Assistant General Manager" || Designation == "Senior Manager" || Designation == "Manager Scale-1" || Designation == "Manager" || Designation == "Manager Tech" || Designation == "Deputy General Manager - Retired" || Designation == "GM" || Designation == "IDO/Manager")
            {
                if (n_CCA <= MaxCCAConditionalAmt)
                    ret = Convert.ToInt32(n_CCA);
                else
                    ret = MaxCCAConditionalAmt;
            }

            return (float)Math.Round(ret, 2);
        }

        private int CalcProfessionalTax(float er_Amt)
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

        private float CalcPF(float er_Amt)
        {
            float ret = 0;
            float PFPercentage = float.Parse(_dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "PFPercentage")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            ret = (float)Math.Round(er_Amt * PFPercentage, 2);
            return ret;
        }

        private float CalcIT(float er_Amt)
        {
            float ret = 0;
            float newnetincome = er_Amt;

            float taxableamt = 0;
            float taxexemption = 0;
            float per_ded_amount = 0;

            float GetSection80EE_HL2 = 0;
            foreach (DataRow pds in dtGetSections.Rows)
            {
                var Sectiontype = pds["section"].ToString();
                var pdamount = float.Parse(pds["amount"].ToString());
                if (Sectiontype == "Section80CCD")
                {
                    float GetSection80CD = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "Section80CCD")
                .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount >= GetSection80CD)
                    {
                        taxableamt += (pdamount - GetSection80CD);
                        taxexemption += GetSection80CD;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }

                }
                else if (Sectiontype == "Section80CCC")
                {
                    float GetSection80CCC = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80CCC")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount >= GetSection80CCC)
                    {
                        taxableamt += (pdamount - GetSection80CCC);
                        taxexemption += GetSection80CCC;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
                else if (Sectiontype == "Section80C")
                {
                    float GetSection80C = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80C")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount >= GetSection80C)
                    {
                        taxableamt += (pdamount - GetSection80C);
                        taxexemption += GetSection80C;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
                else if (Sectiontype == "Section80EE_HL1")
                {
                    float GetSection80EE_HL1 = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80EE_HL1")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount >= GetSection80EE_HL1)
                    {
                        taxableamt += (pdamount - GetSection80EE_HL1);
                        taxexemption += GetSection80EE_HL1;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
                else if (Sectiontype == "Section80EE_HL2")
                {
                    GetSection80EE_HL2 = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80EE_HL2")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount >= GetSection80EE_HL2)
                    {
                        taxableamt += (pdamount - GetSection80EE_HL2);
                        taxexemption += GetSection80EE_HL2;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
                else if (Sectiontype == "Section80D_F")
                {
                    float GetSection80D_F = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80D_F")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount >= GetSection80D_F)
                    {
                        taxableamt += (pdamount - GetSection80D_F);
                        taxexemption += GetSection80D_F;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
                else if (Sectiontype == "Section80D_P")
                {
                    float GetSection80D_P = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80D_P")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount >= GetSection80D_P)
                    {
                        taxableamt += (pdamount - GetSection80D_P);
                        taxexemption += GetSection80D_P;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
                else if (Sectiontype == "Section80DD")
                {
                    float GetSection80DD = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80DD")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount >= GetSection80DD)
                    {
                        taxableamt += (pdamount - GetSection80DD);
                        taxexemption += GetSection80DD;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
                else if (Sectiontype == "Section80DDB")
                {
                    float GetSection80DDB = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80DDB")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount >= GetSection80DDB)
                    {
                        taxableamt += (pdamount - GetSection80DDB);
                        taxexemption += GetSection80DDB;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
                else if (Sectiontype == "Section80U")
                {
                    float GetSection80U = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80U")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount >= GetSection80U)
                    {
                        taxableamt += (pdamount - GetSection80U);
                        taxexemption += GetSection80U;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
                //need todo---
                else if (Sectiontype == "Section80G")
                {
                    float GetSection80G = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80G")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount != 0)
                    {
                        taxableamt += pdamount * GetSection80G;
                        taxexemption += pdamount * GetSection80G;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
                else if (Sectiontype == "Section80GGB")
                {
                    float GetSection80GGB = float.Parse(_incometaxdbconstant.Rows.Cast<DataRow>()
                    .Where(x => x["constant"].ToString() == "Section80GGB")
                    .Select(x => x["value"].ToString()).FirstOrDefault());
                    if (pdamount != 0)
                    {
                        //taxableamt += pdamount * GetSection80GGB;
                        taxexemption += pdamount * GetSection80GGB;
                    }
                    else
                    {
                        taxableamt += pdamount;
                    }
                }
            }
            ret = er_Amt - taxexemption;
            rem_taxexemption = taxexemption - per_ded_amount;
            return (float)Math.Round(ret, 2);
        }

        private float CalcITslabs(float er_Amt)
        {

            float RemainingAmount = 0;
            float taxamount = 0;

            string MinAmount = _dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "IncomeTaxSlabMinConditionalAmt")
                .Select(x => x["value"].ToString()).FirstOrDefault();
            string[] arrMIn = MinAmount.Split(',');
            int MinminAmt = int.Parse(arrMIn[0]);//2500000
            int MinmaxAmt = int.Parse(arrMIn[1]);//4999999
            int diffMinamt = MinmaxAmt - MinminAmt;//2.5L

            string MidAmount = _dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "IncomeTaxSlabMidConditionalAmt")
                .Select(x => x["value"].ToString()).FirstOrDefault();
            string[] arrMid = MidAmount.Split(',');
            int MidminAmt = int.Parse(arrMIn[0]);//500000
            int MidmaxAmt = int.Parse(arrMIn[1]);//1000000
            int diffMidAmt = MidmaxAmt - MidminAmt;//5L

            string MaxAmount = _dtConstants.Rows.Cast<DataRow>()
                .Where(x => x["constant"].ToString() == "IncomeTaxSlabMaxConditionalAmt")
                .Select(x => x["value"].ToString()).FirstOrDefault();
            string[] arrMax = MaxAmount.Split(',');
            int MaxminAmt = int.Parse(arrMIn[0]);//1000001
            int MaxmaxAmt = int.Parse(arrMIn[1]);//5000000
            int diffMaxAmt = MidmaxAmt - MidminAmt;//40L

            float MinPercent = float.Parse(_dtConstants.Rows.Cast<DataRow>()  //5%
                .Where(x => x["constant"].ToString() == "IncomeTaxSlabMin")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            float MidPercent = float.Parse(_dtConstants.Rows.Cast<DataRow>()  //20%
                .Where(x => x["constant"].ToString() == "IncomeTaxSlabMid")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            float MaxPercent = float.Parse(_dtConstants.Rows.Cast<DataRow>() //30%
                .Where(x => x["constant"].ToString() == "IncomeTaxSlabMax")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            float cesspercent = float.Parse(_dtConstants.Rows.Cast<DataRow>() //30%
                .Where(x => x["constant"].ToString() == "IncomeTaxCessPercentage")
                .Select(x => x["value"].ToString()).FirstOrDefault());

            float surchargepercent = float.Parse(_dtConstants.Rows.Cast<DataRow>() //30%
                .Where(x => x["constant"].ToString() == "IncomeTaxSurcharge")
                .Select(x => x["value"].ToString()).FirstOrDefault());
            if (er_Amt > MinminAmt)
            {
                RemainingAmount = er_Amt - MinminAmt;//ex:20l-2.5L=17.5
            }
            else
            {
                RemainingAmount = er_Amt;
            }

            if (RemainingAmount > MinminAmt)//17.5>2.5 true
            {
                if (RemainingAmount > diffMinamt && RemainingAmount > MinmaxAmt) //17.5 > 2.5 and 17.5>5l
                {
                    RemainingAmount = RemainingAmount - MinmaxAmt;//17.5-5l=12.5
                    taxamount += (diffMinamt * MinPercent) * cesspercent;
                    if (RemainingAmount > MaxminAmt)//12.5L>5L
                    {
                        if (RemainingAmount > diffMidAmt && RemainingAmount > MidmaxAmt)//12.5l>5l and 12.5>10L
                        {
                            RemainingAmount = RemainingAmount - MinmaxAmt;//12.5-5l=7.5L
                            taxamount += (diffMidAmt * MidPercent) * cesspercent;
                            if (RemainingAmount < MaxminAmt)
                            {
                                taxamount += (RemainingAmount * MaxPercent);
                            }
                        }
                        else
                        {
                            taxamount += (diffMidAmt * MidPercent);
                        }
                    }
                }
                else
                {
                    taxamount += (diffMinamt * MinPercent);
                }
            }
            else
            {
                taxamount = (RemainingAmount * MinPercent);
            }
            var cessamount = taxamount * cesspercent;
            taxamount = (taxamount + cessamount) / 12;

            return (float)Math.Round(taxamount, 2);
        }

        //fill payslip table



        public async Task Gen_PaySlip(int run_pkid)
        {
            string empcodes = "";
            int n_empcode = 0;
            float no_of_days_mnth = 0;
            string qry = "";

            StringBuilder trnsqry;
            trnsqry = new StringBuilder();
            //trans_id
            trnsqry.Append(GenNewTransactionString());
            //Get the empids on service_run
            //DataTable dtempcodes = await _sha.Get_Table_FromQry("select emp_codes from pr_payroll_service_run where run_date_time<=GETDATE() and status=0 ");
            string qryEmpCodes = "";
            if (run_pkid == 0)
            {
                qryEmpCodes = "SELECT id,emp_codes,total_count FROM pr_payroll_service_run WHERE format(run_date_time,'yyyy-MM-dd')=FORMAT(GETDATE(),'yyyy-MM-dd') and active=1;";
            }
            else
            {
                qryEmpCodes = "SELECT id,emp_codes,total_count FROM pr_payroll_service_run WHERE id=" + run_pkid + ";";
            }
            string qryConstants = "SELECT constant, [value] FROM all_constants WHERE app_type='payroll' AND functionality='GenPayslip' AND active=1;";
            string qryIncometaxConstants = "SELECT constant, [value] FROM all_constants WHERE app_type='payroll' AND functionality='IncomeTax' AND active=1;";
            string qryFy_FM = "SELECT fy,format(fm,'yyyy-MM-dd') as fm FROM pr_month_details WHERE active=1;";


            DataSet ds1 = await _sha.Get_MultiTables_FromQry(qryEmpCodes + qryConstants + qryFy_FM + qryIncometaxConstants);

            DataTable dtempcodes = ds1.Tables[0];
            _dtConstants = ds1.Tables[1];
            DataTable _dtFY_FM = ds1.Tables[2];
            _incometaxdbconstant = ds1.Tables[3];

            try
            {
                int srv_id = 0;
                if (_dtFY_FM.Rows.Count > 0)
                {
                    DataRow fyfm = _dtFY_FM.Rows[0];
                    iFY = Convert.ToInt32(fyfm["fy"]);
                    dtFM = Convert.ToDateTime(fyfm["fm"]);
                }
                no_of_days_mnth = new DateTime(dtFM.Year, dtFM.Month, 1).AddMonths(1).AddDays(-1).Day;
                //get empcodes through pr_payroll_service_run
                foreach (DataRow row in dtempcodes.Rows)
                {
                    int processcount = 0;
                    srv_id = Convert.ToInt32(row["id"]);
                    empcodes = row["emp_codes"].ToString();
                    int total_count = int.Parse(row["total_count"].ToString());
                    string[] empids = empcodes.Split(',');
                    foreach (var empcode in empids)
                    {
                        StringBuilder sbqry = new StringBuilder();
                        n_empcode = Convert.ToInt32(empcode);
                        //n_empcode = 702;

                        DataTable getidfrompayslip = await _sha.Get_Table_FromQry("select id from pr_emp_payslip where emp_code=" + n_empcode + " and month(fm)=" + dtFM.Month + ";");


                        //get id from pr_emp_payslip
                        if (getidfrompayslip.Rows.Count > 0)
                        {
                            DataRow pid = getidfrompayslip.Rows[0];
                            var ps_old_id = pid["id"].ToString();

                            //delete records from pr_emp_payslip_allowance,pr_emp_payslip_deductions,pr_emp_payslip
                            sbqry.Append("delete from pr_emp_payslip_allowance where payslip_mid=" + ps_old_id + ";");
                            sbqry.Append("delete from pr_emp_payslip_deductions where payslip_mid=" + ps_old_id + " ;");
                            sbqry.Append("delete from pr_emp_payslip where id=" + ps_old_id + ";");
                        }

                        //empdetails
                        string Eid = "";
                        string E_designation = "";
                        string E_branch = "";
                        float workingdays = 0;
                        float lop_days = 0;
                        string status = "";
                        DateTime statusdate;
                        int statusday = 0;
                        string payfieldtype = "";
                        float er_fld_amt = 0;
                        float er_amount = 0;

                        float no_of_days = 0;

                        //allowances
                        float grossamount = 0;
                        float HRA = 0;
                        float DA = 0;
                        float CCA = 0;
                        float PT = 0;
                        float PF = 0;
                        float IntermRelief = 0;
                        float TelanganaIncrement = 0;

                        float incometax = 0;
                        float club_subscription = 0;
                        float telangana_officers_assc = 0;
                        float deduction_amount = 0;
                        float net_amount = 0;
                        float basic_amount = 0;
                        float vpf_deduction = 0;

                        //deductionfields
                        int ded_id = 0;
                        string ded_name = "";
                        float c_deduction_amount = 0;
                        string ded_type = "";

                        //jaiibcaiibincr
                        float JAIIBCAIIBIncr = 0;

                        //allowamcefields
                        int alw_id = 0;
                        string alw_name = "";
                        float alw_amount = 0;
                        string alw_type = "";
                        float FPIIP = 0;
                        float Branchallowance = 0;


                        float Branchallowance_gen = 0;

                        DateTime n_fromdate;
                        DateTime to_date;


                        int NewNumIndex = 0;

                        //pfcomponents
                        float StagnationIncrement = 0;
                        float AnnualIncrement = 0;
                        float Special_Pay = 0;
                        float FPA_HRA_Allowance = 0;
                        float SPL_Cashier = 0;
                        float SPL_Driver = 0;
                        float Qual_allowance = 0;
                        float Watchmanallw = 0;
                        float SPL_Jamedar = 0;
                        float SPL_Dafter = 0;
                        float SPL_Personal_Pay = 0;
                        float SPL_Electrician = 0;
                        float Special_Increment = 0;
                        float SPL_Typist = 0;
                        float SPL_Stenographer = 0;
                        float SPL_Xerox_machine = 0;
                        float vpf_Percentage = 0;
                        //Get id,branchname,designation
                        string qry1 = "SELECT m.Id,m.Name as designation,b.Name as bname FROM (select e.id,d.Name,e.Branch from employees e JOIN Designations d ON e.CurrentDesignation=d.id WHERE EmpId=" + n_empcode + ") m join Branches b ON m.Branch=b.Id;";

                        //Get status,workingdays and lop_days
                        string qry2 = "SELECT status,format(status_date,'yyyy-MM-dd') as status_date,working_days,lop_days FROM pr_month_attendance WHERE emp_code=" + n_empcode + " AND active=1 ;";

                        //Get basicamount 
                        string qry3 = "SELECT m.name as payfieldtype,amount,c.m_id FROM pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id WHERE c.emp_code=" + n_empcode + "  AND c.active=1 AND amount>0;";

                        //get deductions from deductions table
                        string qry4 = "SELECT m.id,m.name,c.amount,c.m_type FROM pr_emp_deductions c JOIN pr_deduction_field_master m ON c.m_id=m.id WHERE emp_code=" + n_empcode + " AND c.active=1 AND amount>0;";

                        //get hfc deduction details
                        string qry5 = "SELECT id,amount,pay_type FROM pr_emp_hfc_details WHERE emp_code=" + n_empcode + " AND active=1 AND amount>0 and stop='No' ;";

                        //get lic deduction details
                        string qry6 = "SELECT id,amount,pay_type FROM pr_emp_lic_details WHERE emp_code=" + n_empcode + " AND active=1 AND amount>0 and stop='No' ;";

                        //get loans deduction details
                        string qry7 = "SELECT m.id,m.loan_description,c.installment_amount,c.interest_installment_amount FROM pr_emp_adv_loans c JOIN pr_loan_master m ON c.loan_type_mid=m.id WHERE emp_code=" + n_empcode + " AND c.active=1 AND format(c.installment_start_date,'MM')<=" + dtFM.Month + " AND format(c.installment_start_date,'yyyy')<=" + dtFM.Year + ";";

                        //get emp allowance_general
                        string qry8 = "SELECT m.id,m.name,amount,c.m_type FROM pr_emp_allowances_gen c JOIN pr_allowance_field_master m ON m.id=c.m_id  WHERE emp_code=" + n_empcode + " AND amount>0  AND c.active=1;";

                        //get emp allowance_special
                        string qry9 = "SELECT m.id,m.name,amount,c.m_type FROM pr_emp_allowances_spl c JOIN pr_allowance_field_master m ON m.id=c.m_id  WHERE emp_code=" + n_empcode + " AND amount>0  AND c.active=1;";

                        //get emp branch allowance
                        string qry10 = "SELECT format(c.from_date,'yyyy-MM-dd') as from_date ,format(c.to_date,'yyyy-MM-dd') as to_date,m.name,m.amount FROM pr_emp_branch_allowances c JOIN pr_branch_allowance_master m ON c.allowance_mid=m.id WHERE emp_code=" + n_empcode + " AND c.active=1 AND format(c.from_date,'MM')<=" + dtFM.Month + ";"; //todo - add fromdate <=month_details.month

                        //get da_percentage
                        string qry11 = "SELECT da_percent FROM pr_month_details WHERE active=1;";

                        //get section and exemption amount limit 
                        string qry12 = "select section,amount from  pr_emp_perdeductions where emp_code=" + n_empcode + " and active=1";

                        //get JAIIBCAIIBIncrements 
                        //string qry13 = "SELECT * FROM pr_emp_jaib_caib_general WHERE emp_code=" + n_empcode + " AND format(incr_WEF_date,'MM')=" + dtFM.Month + " AND active=1 AND authorisation=1;";

                        //get data from all queries
                        DataSet ds = await _sha.Get_MultiTables_FromQry(qry1 + qry2 + qry3 + qry4 + qry5 + qry6 + qry7 + qry8 + qry9 + qry10 + qry11 + qry12);

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
                        //var dtEmpJAIIBCAIIBIncrements = ds.Tables[12];

                        //employee id,designation,branch
                        if (dtEmpdetails.Rows.Count > 0)
                        {
                            DataRow ed = dtEmpdetails.Rows[0];
                            Eid = ed["id"].ToString();
                            E_designation = ed["designation"].ToString();
                            E_branch = ed["bname"].ToString();
                        }

                        //status,workingday's and lop's from pr_month_attendance
                        if (dtEmpworkingdetails.Rows.Count > 0)
                        {
                            DataRow ewd = dtEmpworkingdetails.Rows[0];
                            status = ewd["status"].ToString();

                            if (status != "StopSalary")
                            {
                                if (status == "Suspended")
                                {
                                    statusdate = Convert.ToDateTime(ewd["status_date"].ToString());
                                    statusday = statusdate.Day;
                                    workingdays = (statusday - new DateTime(dtFM.Year, dtFM.Month, 1).Day);
                                    lop_days = (new DateTime(dtFM.Year, dtFM.Month, 1).AddMonths(1).AddDays(-1).Day - workingdays);
                                }
                                else
                                {
                                    workingdays = Convert.ToInt32(ewd["working_days"]);
                                    lop_days = Convert.ToInt32(ewd["lop_days"]);
                                }
                                float alw_spl_tds_lic = 0;

                                foreach (DataRow ebd in dtEmpbasicdetails.Rows)
                                {
                                    payfieldtype = ebd["payfieldtype"].ToString();
                                    alw_id = Convert.ToInt32(ebd["m_id"]);
                                    er_fld_amt = float.Parse(ebd["amount"].ToString());
                                    er_amount = (er_fld_amt / no_of_days_mnth) * workingdays;

                                    //calculations hra,da,cca based on basic
                                    if (payfieldtype == "Basic")
                                    {
                                        basic_amount = er_amount;
                                        CCA = CalcCCA(basic_amount, E_designation);
                                    }
                                    else if (payfieldtype == "Telangana Increment")
                                    {
                                        TelanganaIncrement = (float)Math.Round(er_amount, 2);
                                    }
                                    else if (payfieldtype == "Interm Relief")
                                    {
                                        IntermRelief = (float)Math.Round(er_amount, 2);
                                    }

                                    else if (payfieldtype != "Basic" && payfieldtype != "Telangana Increment" && payfieldtype != "Interim Relief" && payfieldtype != "Loss of Pay")
                                    {
                                        if (payfieldtype == "Special Pay")
                                        {
                                            Special_Pay = er_amount;
                                        }
                                        else if (payfieldtype == "Special Increment")
                                        {
                                            Special_Increment = er_amount;
                                        }
                                        else if (payfieldtype == "Stagnation Increments")
                                        {
                                            StagnationIncrement = er_amount;
                                        }
                                        else if (payfieldtype == "Annual Increment")
                                        {

                                        }
                                        else if (payfieldtype == "JAIIB Increment")
                                        {
                                            JAIIBCAIIBIncr = er_fld_amt;
                                        }
                                        else if (payfieldtype == "CAIIB Increment")
                                        {
                                            JAIIBCAIIBIncr = er_fld_amt;
                                        }
                                        //Special Increment, Employee TDS, LIC Premium
                                        NewNumIndex++;
                                        sbqry.Append(GetNewNumStringArr("pr_emp_payslip_allowance", NewNumIndex));
                                        qry = "Insert into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,all_amount,all_type," +
                                        "active,trans_id) values(@idnew" + NewNumIndex + "," + Eid + "," + n_empcode + ",@idnew," + alw_id + ",'" + payfieldtype + "'," + Math.Round(er_amount, 2) + ",'" + payfieldtype + "',1,@transidnew);";
                                        sbqry.Append(qry);
                                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance", "@idnew" + NewNumIndex.ToString(), null));
                                        alw_spl_tds_lic += er_amount;
                                    }
                                }

                                //Prof.Tax(deduction)
                                PT = CalcProfessionalTax(basic_amount);

                                //allowancegeneral from pr_emp_allowances_gen
                                foreach (DataRow algen in dtEmpallowance_generaldetails.Rows)
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_payslip_allowance", NewNumIndex));
                                    alw_id = Convert.ToInt32(algen["id"]);
                                    alw_name = algen["name"].ToString();
                                    alw_amount = float.Parse(algen["amount"].ToString());
                                    er_amount = (alw_amount / no_of_days_mnth) * workingdays;
                                    alw_type = algen["m_type"].ToString();
                                    if (alw_name == "FPIIP")
                                    {
                                        FPIIP = er_amount;
                                    }
                                    else if (alw_name == "FPA-HRA Allowance")
                                    {
                                        FPA_HRA_Allowance = er_amount;
                                    }
                                    else if (alw_name == "Personal Qual Allowance")
                                    {
                                        Qual_allowance = er_amount;
                                    }
                                    else if (alw_name == "Br Manager Allowance")
                                    {
                                        Branchallowance_gen = er_amount;
                                    }
                                    grossamount += er_amount;
                                    qry = "INSERT into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,all_amount,all_type," +
                                        "active,trans_id) VALUES(@idnew" + NewNumIndex + "," + Eid + "," + n_empcode + ",@idnew," + alw_id + ",'" + alw_name + "'," + Math.Round(er_amount, 2) + ",'" + alw_type + "',1,@transidnew);";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance", "@idnew" + NewNumIndex.ToString(), null));




                                }

                                //branch_allowance from pr_emp_branch_allowances
                                foreach (DataRow brall in dtEmpbranch_allowance.Rows)
                                {
                                    //fromdate
                                    n_fromdate = Convert.ToDateTime(brall["from_date"]);
                                    //check if from date is not eq to FM
                                    if (n_fromdate.Month < dtFM.Month)
                                    {
                                        n_fromdate = new DateTime(dtFM.Year, dtFM.Month, 1);
                                    }

                                    if (brall["to_date"] is DBNull)
                                    {
                                        //get last day of the month of n_fromdate
                                        to_date = new DateTime(n_fromdate.Year, n_fromdate.Month, 1).AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        to_date = Convert.ToDateTime(brall["to_date"]);
                                        //check if todate is not eq to fm
                                        if (to_date.Month > dtFM.Month)
                                        {
                                            to_date = new DateTime(n_fromdate.Year, n_fromdate.Month, 1).AddMonths(1).AddDays(-1);
                                        }
                                    }
                                    no_of_days = Convert.ToInt32((to_date - n_fromdate).TotalDays + 1);

                                    var bralw_amount = float.Parse(brall["amount"].ToString());
                                    var totdaysinMn = new DateTime(dtFM.Year, dtFM.Month, 1).AddMonths(1).AddDays(-1).Day;

                                    float oneDayAmt = bralw_amount / totdaysinMn;
                                    float balw_amount = no_of_days * oneDayAmt;

                                    alw_name = brall["name"].ToString();
                                    alw_type = "BranchAllowence";
                                    if (Branchallowance_gen == 0 && alw_name == "BR_MGR")
                                    {
                                        Branchallowance = balw_amount;
                                        grossamount += Branchallowance;
                                        NewNumIndex++;
                                        sbqry.Append(GetNewNumStringArr("pr_emp_payslip_allowance", NewNumIndex));
                                        qry = "INSERT into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,all_amount,all_type," +
                                        "active,trans_id) VALUES(@idnew" + NewNumIndex + "," + Eid + "," + n_empcode + ",@idnew," + alw_id + ",'" + alw_name + "'," + Math.Round(balw_amount, 2) + ",'" + alw_type + "',1,@transidnew);";
                                        sbqry.Append(qry);
                                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance", "@idnew" + NewNumIndex.ToString(), null));
                                    }
                                    else if (alw_name != "BR_MGR")
                                    {
                                        grossamount += balw_amount;
                                        NewNumIndex++;
                                        sbqry.Append(GetNewNumStringArr("pr_emp_payslip_allowance", NewNumIndex));
                                        qry = "INSERT into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,all_amount,all_type," +
                                        "active,trans_id) VALUES(@idnew" + NewNumIndex + "," + Eid + "," + n_empcode + ",@idnew," + alw_id + ",'" + alw_name + "'," + Math.Round(balw_amount, 2) + ",'" + alw_type + "',1,@transidnew);";
                                        sbqry.Append(qry);
                                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance", "@idnew" + NewNumIndex.ToString(), null));
                                    }

                                }

                                //specialallowance from pr_emp_allowances_spl
                                foreach (DataRow alspl in dtEmpallowance_specialdetails.Rows)
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_payslip_allowance", NewNumIndex));
                                    alw_id = Convert.ToInt32(alspl["id"]);
                                    alw_name = alspl["name"].ToString();
                                    alw_amount = float.Parse(alspl["amount"].ToString());
                                    er_amount = (alw_amount / no_of_days_mnth) * workingdays;
                                    alw_type = alspl["m_type"].ToString();
                                    if (alw_name == "SPL Cashier")
                                    {
                                        SPL_Cashier = er_amount;
                                    }
                                    else if (alw_name == "SPL Watchman")
                                    {
                                        Watchmanallw = er_amount;
                                    }
                                    else if (alw_name == "SPL Jamedar")
                                    {
                                        SPL_Jamedar = er_amount;
                                    }
                                    else if (alw_name == "SPL Dafter")
                                    {
                                        SPL_Dafter = er_amount;
                                    }
                                    else if (alw_name == "SPL Personal Pay")
                                    {
                                        SPL_Personal_Pay = er_amount;
                                    }
                                    else if (alw_name == "SPL Electrician")
                                    {
                                        SPL_Electrician = er_amount;
                                    }
                                    else if (alw_name == "SPL Typist")
                                    {
                                        SPL_Typist = er_amount;
                                    }
                                    else if (alw_name == "SPL Stenographer")
                                    {
                                        SPL_Stenographer = er_amount;
                                    }
                                    else if (alw_name == "SPL Duplicating/xerox machine")
                                    {
                                        SPL_Xerox_machine = er_amount;
                                    }
                                    else if (alw_name == "SPL Driver")
                                    {
                                        SPL_Driver = er_amount;
                                    }
                                    grossamount += alw_amount;
                                    qry = "INSERT into pr_emp_payslip_allowance(id,emp_id,emp_code,payslip_mid,all_mid,all_name,all_amount,all_type," +
                                        "active,trans_id) VALUES(@idnew" + NewNumIndex + "," + Eid + "," + n_empcode + ",@idnew," + alw_id + ",'" + alw_name + "'," + Math.Round(er_amount, 2) + ",'" + alw_type + "',1,@transidnew);";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_allowance", "@idnew" + NewNumIndex.ToString(), null));
                                }


                                //deductions from pr_emp_deductions
                                foreach (DataRow edd in dtEmpdeductiondetails.Rows)
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                                    ded_id = Convert.ToInt32(edd["id"]);
                                    ded_name = edd["name"].ToString();
                                    c_deduction_amount = float.Parse(edd["amount"].ToString());
                                    er_amount = (c_deduction_amount / no_of_days_mnth) * workingdays;
                                    ded_type = edd["m_type"].ToString();

                                    if (ded_name == "Club Subscription")
                                    {
                                        club_subscription = er_amount;
                                    }
                                    else if (ded_name == "Telangana Officers Assc")
                                    {
                                        telangana_officers_assc = er_amount;
                                    }

                                    else if (ded_name != "PF Contribution" && ded_name != "Club Subscription" && ded_name != "Telangana Officers Assc")
                                    {
                                        if (ded_name == "VPF Deduction")
                                        {
                                            vpf_deduction = er_amount;
                                        }
                                        else if (ded_name == "VPF Percentage")
                                        {
                                            if (vpf_deduction == 0)
                                            {
                                                c_deduction_amount = (PF * c_deduction_amount) / 100;
                                                vpf_Percentage = er_amount;
                                            }
                                        }
                                        //add amounts to deductionamount
                                        if (ded_name != "VPF Percentage")
                                        {
                                            deduction_amount += c_deduction_amount;
                                            qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                            "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + n_empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(er_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                            sbqry.Append(qry);
                                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                        }
                                        else if (ded_name == "VPF Percentage" && vpf_Percentage != 0)
                                        {
                                            deduction_amount += vpf_Percentage;
                                            qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                            "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + n_empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(er_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                            sbqry.Append(qry);
                                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                        }

                                    }

                                }

                                //hfc deductions from pr_emp_hfc_details
                                foreach (DataRow ehdd in dtEmpHFCdeductiondetails.Rows)
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                                    ded_id = Convert.ToInt32(ehdd["id"]);
                                    c_deduction_amount = float.Parse(ehdd["amount"].ToString());
                                    ded_type = ehdd["pay_type"].ToString();
                                    ded_name = "HFC";
                                    deduction_amount += c_deduction_amount;
                                    qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + n_empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                }

                                //lic deductions from pr_emp_lic_details
                                foreach (DataRow eldd in dtEmpLICdeductiondetails.Rows)
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                                    ded_id = Convert.ToInt32(eldd["id"]);
                                    c_deduction_amount = float.Parse(eldd["amount"].ToString());
                                    ded_type = eldd["pay_type"].ToString();
                                    ded_name = "LIC";
                                    deduction_amount += c_deduction_amount;
                                    qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + n_empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + c_deduction_amount + ",'" + ded_type + "',1,@transidnew);";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                }

                                //loans deductions from pr_emp_adv_loans
                                foreach (DataRow elndd in dtEmpLoansdeductiondetails.Rows)
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_payslip_deductions", NewNumIndex));
                                    ded_id = int.Parse(elndd["id"].ToString());
                                    ded_name = elndd["loan_description"].ToString();
                                    var dedinst_amount = float.Parse(elndd["installment_amount"].ToString());
                                    var dedint_amount = float.Parse(elndd["interest_installment_amount"].ToString());
                                    if (dedinst_amount != 0)
                                    {
                                        c_deduction_amount = dedinst_amount;
                                    }
                                    else if (dedint_amount != 0)
                                    {
                                        c_deduction_amount = dedint_amount;
                                    }

                                    ded_type = "Loan";
                                    deduction_amount += c_deduction_amount;
                                    qry = "INSERT into pr_emp_payslip_deductions(id,emp_id,emp_code,payslip_mid,dd_mid,dd_name,dd_amount,dd_type,active,trans_id) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + Eid + "," + n_empcode + ",@idnew," + ded_id + ",'" + ded_name + "'," + Math.Round(c_deduction_amount, 2) + ",'" + ded_type + "',1,@transidnew);";
                                    sbqry.Append(qry);
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip_deductions", "@idnew" + NewNumIndex.ToString(), null));
                                }

                                //HRA calculation
                                var hra_DA_amt = basic_amount + StagnationIncrement + SPL_Jamedar + SPL_Cashier + SPL_Dafter + SPL_Driver + Watchmanallw + JAIIBCAIIBIncr + SPL_Xerox_machine;
                                HRA = CalcHRA(hra_DA_amt, E_designation);

                                //DA calculation
                                DA = CalcDA(hra_DA_amt, dtDa_slabs);

                                //total grossamount
                                grossamount += basic_amount + HRA + DA + CCA + TelanganaIncrement + IntermRelief + alw_spl_tds_lic;

                                //PF_calculation
                                float PF_amt = basic_amount + DA + FPIIP + Special_Pay + FPA_HRA_Allowance + SPL_Cashier + SPL_Driver + Qual_allowance + Watchmanallw + SPL_Dafter + SPL_Jamedar + SPL_Electrician + SPL_Personal_Pay + Special_Increment + SPL_Typist + SPL_Stenographer + SPL_Xerox_machine + StagnationIncrement;
                                PF = CalcPF(PF_amt);


                                //income_tax calculation-todo
                                var pfpt = PF + PT;
                                var sub_netincome = (grossamount - pfpt) * 12;
                                var taxableamount = CalcIT(sub_netincome);
                                incometax = CalcITslabs(taxableamount);

                                //calculate lop_amount
                                no_of_days = new DateTime(dtFM.Year, dtFM.Month, 1).AddMonths(1).AddDays(-1).Day;

                                //adding lop to deduction amount
                                float lop_amount = (float)Math.Round((grossamount / no_of_days) * lop_days, 2);


                                //adding all deductions to deduction amount
                                deduction_amount += PF + PT + incometax + club_subscription + telangana_officers_assc + lop_amount;
                                net_amount = grossamount - deduction_amount;


                                //special DA
                                float getSpecial_DA = float.Parse(_dtConstants.Rows.Cast<DataRow>()
                                .Where(x => x["constant"].ToString() == "Special_DA")
                                .Select(x => x["value"].ToString()).FirstOrDefault());

                                float Special_DA = DA * getSpecial_DA;

                                //special Allowance
                                float getSpecila_Allw = float.Parse(_dtConstants.Rows.Cast<DataRow>()
                               .Where(x => x["constant"].ToString() == "Special_Allw")
                               .Select(x => x["value"].ToString()).FirstOrDefault());

                                float Specila_Allw = (basic_amount + StagnationIncrement + JAIIBCAIIBIncr + AnnualIncrement) * getSpecila_Allw;


                                //if record insert correctly 

                                int seriveempstatus = 0;
                                if (total_count == processcount)
                                {
                                    seriveempstatus = 1;
                                }
                                qry = "UPDATE pr_payroll_service_run set status=" + seriveempstatus + ", process_count=" + processcount + " WHERE id=" + srv_id + ";";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_payroll_service_run", "@idnew" + NewNumIndex.ToString(), null));

                                //final insertion in pr_emp_payslip
                                string parentqry = "";

                                parentqry += GetNewNumString("pr_emp_payslip");

                                parentqry += "INSERT into pr_emp_payslip(id,gen_date,fy,fm,emp_id,emp_code,designation,branch,working_days,lop,er_basic," +
                                    "er_da,er_cca,er_hra,er_interim_relief,er_telangana_inc,gross_amount,dd_provident_fund,dd_income_tax,dd_prof_tax,dd_club_subscription,dd_telangana_officers_assn,deductions_amount,lop_amount,net_amount,active,trans_id,spl_da,spl_allw) " +
                                    "VALUES(@idnew,getdate(),'" + iFY + "','" + dtFM.ToString("yyyy-MM-dd") + "'," + Eid + "," + n_empcode + ",'" + E_designation + "','" + E_branch + "'," + workingdays + "," +
                                    "" + lop_days + "," + basic_amount + "," + DA + "," + CCA + "," + HRA + "," + IntermRelief + "," + TelanganaIncrement + "," + grossamount + "," + PF + "," + incometax + "," + PT + "," + club_subscription + "," + telangana_officers_assc + "," + deduction_amount + "," + lop_amount + "," + net_amount + ",1,@transidnew," + Special_DA + "," + Specila_Allw + ");";

                                parentqry += GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_payslip", "@idnew", null);
                                try
                                {
                                    await _sha.Run_UPDDEL_ExecuteNonQuery(trnsqry + parentqry + sbqry.ToString());
                                }
                                catch (Exception ex)
                                {
                                    _logger.Info(sbqry.ToString());
                                    _logger.Error(ex.Message);
                                    _logger.Error(ex.StackTrace);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }



    }
}
