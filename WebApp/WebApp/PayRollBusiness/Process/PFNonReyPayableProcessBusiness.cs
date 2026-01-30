using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PayRollBusiness.Process
{
    public class PFNonReyPayableProcessBusiness : BusinessBase
    {
        public PFNonReyPayableProcessBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        public async Task<string> getNonPayableEmpDetails(string empCode)
        {
            string qry = "select id from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and active=1 and authorisation=1 and process=0;";

            DataTable dt = await _sha.Get_Table_FromQry(qry);

            if (dt.Rows.Count > 0)
            {
                IList<PfDetails> lstDept = new List<PfDetails>();
                string genQry = "Select pf.pf_account_no as pf_no,pf.purpose_of_advance,adv.purpose_name,pf.rate_of_basic_da," +
                    "pf.eligibility_amount,pf.own_share,pf.vpf,pf.bank_share,pf.total,adv.month,pf.amount_applied,pf.sanctioned_amount " +
                    "from pr_emp_pf_nonrepayable_loan pf " +
                    "join pr_purpose_of_advance_master adv on pf.purpose_of_advance = adv.id " +
                    "WHERE pf.emp_code = " + Convert.ToInt32(empCode) + " and pf.active = 1 and authorisation=1 and process=0;";
                string certQry = "select id,cert_name,status from pr_emp_pf_non_cert_elg where " +
                    "emp_code=" + Convert.ToInt32(empCode) + " and active=1 and authorisation=1 and process=0;";
                DataSet ds = await _sha.Get_MultiTables_FromQry(genQry + certQry);
                DataTable common = ds.Tables[0];
                foreach (DataRow gen in common.Rows)
                {
                    lstDept.Add(new PfDetails
                    {
                        pf_no = gen["pf_no"].ToString(),
                        purpose_name = gen["purpose_name"].ToString(),
                        rate_of_basic_da = Math.Round(Convert.ToDouble(gen["rate_of_basic_da"].ToString())),
                        eligibility_amount = gen["eligibility_amount"].ToString(),
                        own_share = Convert.ToInt32(gen["own_share"]),
                        vpf = Convert.ToInt32(gen["vpf"]),
                        bank_share = Convert.ToInt32(gen["bank_share"]),
                        total = gen["total"].ToString(),
                        amount_applied = gen["amount_applied"].ToString(),
                        purpose_of_advance = gen["purpose_of_advance"].ToString(),
                        month = gen["month"].ToString(),
                        sanction = gen["sanctioned_amount"].ToString(),

                    });
                }
                DataTable cert = ds.Tables[1];
                IList<certDetails> lstCer = new List<certDetails>();
                foreach (DataRow cer in cert.Rows)
                {
                    lstCer.Add(new certDetails
                    {
                        id = cer["id"].ToString(),
                        cert_name = cer["cert_name"].ToString(),
                        status = cer["status"].ToString(),
                    });
                }

                //string selQry = "select o.own_share_total as own_share,o.bank_share_total as bank_share,o.vpf_total as vpf,o.da,o.basic,e.pf_no from " +
                //    "pr_ob_share o left outer join pr_emp_general e on o.emp_code = e.emp_code where o.emp_code =" + Convert.ToInt32(empCode) + "and o.active=1";
                string selQry = "select top 1 case when prnon.own_share is null then o.own_share_total " +
                    "else ((o.own_share_total + (case when o.own_share_intrst_total is null then 0 else own_share_intrst_total end))-prnon.own_share) " +
                    "end as own_share,case when prnon.vpf is null then (o.vpf_total+(ISNULL(o.vpf_intrst_total, 0 )))" +
                    " else ((ISNULL(o.vpf_intrst_total, 0 )+o.vpf_total) - (select sum (vpf) from pr_emp_pf_nonrepayable_loan " +
                    "where emp_code=" + Convert.ToInt32(empCode) + " and process=1))end as vpf," +
                    "case when prnon.bank_share is null then o.bank_share_total else ((o.bank_share_total + (case when o.bank_share_intrst_total is null then 0 " +
                    "else o.bank_share_intrst_total  end)) -prnon.bank_share) end as bank_share," +
                    "o.da,o.basic,e.pf_no as pf_no,format(emp.RetirementDate, 'yyyy,MM,dd') as  RetirementDate from " +
                       "pr_ob_share o left join pr_emp_pf_nonrepayable_loan prnon on o.emp_code=prnon.emp_code and o.active=1  and prnon.process=1 left outer join pr_emp_general e on o.emp_code = e.emp_code join Employees emp on e.emp_code=emp.EmpId where o.emp_code =" + Convert.ToInt32(empCode) + " and o.active=1";
                DataTable details = await _sha.Get_Table_FromQry(selQry);

                IList<PfDetails> lstDepts = new List<PfDetails>();
                foreach (DataRow dr in details.Rows)
                {
                    lstDepts.Add(new PfDetails
                    {
                        basic = dr["basic"].ToString(),
                        pf_no = dr["pf_no"].ToString(),
                        own_share = Convert.ToInt32(dr["own_share"]),
                        bank_share = Convert.ToInt32(dr["bank_share"]),
                        da_percent = dr["da"].ToString(),
                        vpf = Convert.ToInt32(dr["vpf"]),
                    });
                }

                var loans = JsonConvert.SerializeObject(lstDepts);
                var loan = JsonConvert.SerializeObject(lstDept);
                var certificates = JsonConvert.SerializeObject(lstCer);
                var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var loanData = javaScriptSerializer.DeserializeObject(loan);
                var certData = javaScriptSerializer.DeserializeObject(certificates);
                var loansData = javaScriptSerializer.DeserializeObject(loans);
                var resultJson = javaScriptSerializer.Serialize(new
                {
                    loan = loanData,
                    cert = certData,
                    loans = loansData

                });
                return resultJson;
            }
            else
            {
                return "No Data Found";
            }
        }
        public async Task<string> getProcess(PFNonPayable Values)
        {
            int emp_code = _LoginCredential.EmpCode;
            //string selQry = "select own_share, bank_share, vpf from pr_emp_pf_nonrepayable_loan where emp_code =" + Values.EntityId + "and active=1";
            string selQry = "select distinct case when prnon.own_share is null then (o.own_share_total+o.own_share_intrst_total) else ((ISNULL(o.own_share_intrst_total, 0 )+o.own_share_total)-prnon.own_share) end as own_share,case when prnon.vpf is null then (o.vpf_total+o.vpf_intrst_total) else ((ISNULL(o.vpf_intrst_total, 0 )+o.vpf_total) - prnon.vpf )end as vpf,case when prnon.bank_share is null then (o.bank_share_total+o.bank_share_intrst_total) else ((ISNULL(o.bank_share_intrst_total, 0 )+o.bank_share_total) - prnon.bank_share) end as bank_share,o.da,o.basic,e.pf_no as pf_no,format(emp.RetirementDate, 'yyyy,MM,dd') as  RetirementDate from " +
                        "pr_ob_share o left join pr_emp_pf_nonrepayable_loan prnon on o.emp_code=prnon.emp_code and o.active=1  and prnon.process=1 left outer join pr_emp_general e on o.emp_code = e.emp_code join Employees emp on e.emp_code=emp.EmpId where o.emp_code =" + Values.EntityId + "and o.active=1";
            DataTable details = await _sha.Get_Table_FromQry(selQry);
            decimal? own_share = 0;
            decimal? bank_share = 0;
            decimal? vpf = 0;
            if (details.Rows.Count > 0)
            {
                DataRow gen = details.Rows[0];

                own_share = Convert.ToDecimal(gen["own_share"]);
                bank_share = Convert.ToDecimal(gen["bank_share"]);
                vpf = Convert.ToDecimal(gen["vpf"]);

            }

            int NewNumIndex = 0;
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            NewNumIndex++;
            sbqry.Append(GetNewNumStringArr("pr_emp_pf_nonrepayable_loan", NewNumIndex));
            string qry = " update pr_emp_pf_nonrepayable_loan set process =1, active=0, sanction_date=Convert(date,'" + Values.sactiondate + "',105),process_date=convert(date,'" + Values.processdate + "', 105) " +
                " where emp_code=" + Values.EntityId + " and active=1 and authorisation=1 and process=0;";
            sbqry.Append(qry);

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_nonrepayable_loan", "@idnew" + NewNumIndex, emp_code.ToString()));

            //NewNumIndex++;
            //sbqry.Append(GetNewNumStringArr("pr_ob_share", NewNumIndex));
            //string obQry = " update pr_ob_share set own_share_total = own_share_total-" + own_share + ", bank_share_total=bank_share_total-" + bank_share + ", vpf_total=vpf_total-" + vpf + " " +
            //    " where emp_code=" + Values.EntityId + " and active=1;";
            //sbqry.Append(obQry);

            //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_ob_share", "@idnew" + NewNumIndex, emp_code.ToString()));


            NewNumIndex++;
            sbqry.Append(GetNewNumStringArr("pr_emp_pf_non_cert_elg", NewNumIndex));
            string qry1 = " update pr_emp_pf_non_cert_elg set active =0 , process=1 " +
                " where emp_code=" + Values.EntityId + " and active=1;";
            sbqry.Append(qry1);

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_non_cert_elg", "@idnew" + NewNumIndex, emp_code.ToString()));
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#PF Non Payable#Loan Processed Successfully..!!";
            }
            else
            {
                return "E#PF Non Payable#Error While Loan Process";
            }
        }

        public async Task<string> GetEmpOBFieldsDetails()
        {
            //string query = " select distinct pm.id as Id, format(pm.fm,'yyyy-MM-dd') as fm ,pm.interest_percent as interest from pr_ob_share ob join pr_month_details pm on ob.fm=pm.fm  and pm.is_interest_calculated=0";
            //string query = " select distinct ob.fm as fm ,pm.interest_percent as interest from pr_ob_share ob join pr_month_details pm on ob.fm=pm.fm ";
            
            string query = " select distinct pm.id as Id, format(pm.fm,'yyyy-MM-dd') as fm ,pm.interest_percent as interest from pr_ob_share ob join pr_month_details pm on ob.active=pm.active  and pm.is_interest_calculated=0 where pm.active=1 ";

            DataTable dt = await _sha.Get_Table_FromQry(query);
            return JsonConvert.SerializeObject(dt);
        }
        //pf interest calculation 
        public async Task<string> SavepfIntData(CommonPostDTO Values)
        {
            string nrempcode = "";
            string sucmes = "";
            string empcode = "";
            decimal ownshare = 0;
            decimal vpf = 0;
            decimal bankshare = 0;
            decimal vpfinst = 0;
            decimal bankshareinst = 0;
            decimal ownshareinst = 0;
            decimal ownshareinsttotal = 0;
            decimal bankshareinsttotal = 0;
            decimal vpfinsttotal = 0;
           
            decimal ownshareinstopenprev = 0;
            decimal bankshareopenprev = 0;
            decimal vpfinstopenprev = 0;
            decimal nrownshare = 0;
            decimal nrbankshare = 0;
            decimal nrvpfshare = 0;
            foreach (var pfint in Values.PfIntest)
            {

                string strDate = pfint.Id;
                string[] sa = strDate.Split('-');
                string s1 = sa[0];
                
                string s2 = sa[1];
                string s3 = sa[2];
              string  fystart = "01" + "-" + DateTime.Now.Month + "-" + s1;
            
                string querynrloan = "select emp_code,ISNULL(own_share, 0 ) as own_share , ISNULL(bank_share, 0 ) as bank_share,ISNULL(vpf, 0 ) as vpf,process_date from pr_emp_pf_nonrepayable_loan where  year(process_date)=" + s1+ " and month(process_date)=" + s2+ "";
                DataTable dtloan = await _sha.Get_Table_FromQry(querynrloan);
             
                foreach (DataRow dr in dtloan.Rows)
                {
                    nrempcode = dr["emp_code"].ToString();
                    nrownshare= Convert.ToDecimal(dr["own_share"]);
                    nrbankshare= Convert.ToDecimal(dr["bank_share"]);
                    nrvpfshare = Convert.ToDecimal(dr["vpf"]);
                    string fm = dr["process_date"].ToString();
                    double diffdates = (Convert.ToDateTime(fystart) - Convert.ToDateTime(fm)).TotalDays;
                    decimal ownreturn = Convert.ToDecimal(Math.Round(((Convert.ToDouble(nrownshare) * (Convert.ToDouble(pfint.Value )/100)) / 365 * diffdates), 2));
                    decimal bankreturn = Convert.ToDecimal(Math.Round(((Convert.ToDouble(nrbankshare) * (Convert.ToDouble(pfint.Value) / 100)) / 365 * diffdates), 2));
                    decimal vpfreturn = Convert.ToDecimal(Math.Round(((Convert.ToDouble(nrvpfshare) * (Convert.ToDouble(pfint.Value) / 100)) / 365 * diffdates), 2));
                    //string openingbalqry = "select top 1 PFreturn,Bankreturn,vpfreturn from pr_pfopeningbal where emp_code=" + nrempcode + " and year ="+s1+" order by year desc";
                    //DataTable pfopenbal = await _sha.Get_Table_FromQry(openingbalqry);
                    //foreach (DataRow drs in pfopenbal.Rows)
                    //{
                    //    decimal ownshareopen = Convert.ToDecimal(drs["PFreturn"])+ ownreturn;
                    //    decimal bankshareopen = Convert.ToDecimal(drs["Bankreturn"])+ bankreturn;
                    //    decimal vpfshareopen = Convert.ToDecimal(drs["vpfreturn"])+ vpfreturn;


                    //    string queryupdate = " update pr_pfopeningbal set PFreturn=" + ownshareopen + ",Bankreturn=" + bankshareopen + ",vpfreturn=" + vpfshareopen + " where year =" + s1 + " and emp_code in (" + nrempcode + " )";
                    //    await _sha.Run_UPDDEL_ExecuteNonQuery(queryupdate);


                    //}
                    string queryupdate = " update pr_emp_pf_nonrepayable_loan set ownshare_interest="+ ownreturn + ",bankshare_interest="+bankreturn+",vpf_interest="+ vpfreturn + " where emp_code="+ nrempcode + " and year(process_date)=" + s1 + " and month(process_date)=" + s2 + "";
                    await _sha.Run_UPDDEL_ExecuteNonQuery(queryupdate);
                    string queryflagupdate = " update pr_emp_pf_nonrepayable_loan set is_interest_caculated=1 where emp_code=" + nrempcode + " and year(process_date)=" + s1 + " and month(process_date)=" + s2 + "";
                    await _sha.Run_UPDDEL_ExecuteNonQuery(queryflagupdate);
                }
                string query = " select distinct emp_code from pr_ob_share where fm='" + pfint.Id + "' ";


                    DataTable dt = await _sha.Get_Table_FromQry(query);

                foreach (DataRow dr in dt.Rows)
                {

                    string emp = dr["emp_code"].ToString();

                    string query3 = " select distinct emp_code,fm,ISNULL(bank_share_intrst_total,0) as bank_share_intrst_total,ISNULL(own_share_intrst_total,0) as own_share_intrst_total ,ISNULL(vpf_intrst_total,0) as vpf_intrst_total from pr_ob_share where fm=DATEADD(month, -1,'" + pfint.Id + "') and  emp_code=" + emp + "";

                    DataTable dt2 = await _sha.Get_Table_FromQry(query3);

                    if (dt2.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            ownshareinstopenprev = Math.Round(Convert.ToDecimal(dr2["own_share_intrst_total"]), 2);
                            bankshareopenprev = Math.Round(Convert.ToDecimal(dr2["bank_share_intrst_total"]), 2);
                            vpfinstopenprev = Math.Round(Convert.ToDecimal(dr2["vpf_intrst_total"]), 2);
                            string queryupdates = " update pr_ob_share set is_interest_caculated=1,own_share_intrst_open=" + ownshareinstopenprev + ",bank_share_intrst_open=" + bankshareopenprev + ",vpf_intrst_open=" + vpfinstopenprev + " where fm='" + pfint.Id + "' and emp_code in (" + emp + " )";
                            await _sha.Run_UPDDEL_ExecuteNonQuery(queryupdates);
                        }
                    }
                    string query2 = " select distinct emp_code,fm,ISNULL(own_share,0) as own_share,ISNULL (own_share_intrst_amount,0) as own_share_intrst_amount,ISNULL(vpf,0) as vpf,ISNULL(vpf_intrst_amount,0) as vpf_intrst_amount,ISNULL(bank_share,0) as bank_share,ISNULL(bank_share_intrst_amount,0) as bank_share_intrst_amount ,ISNULL(own_share_open,0) as own_share_open,ISNULL(own_share_total,0) as own_share_total,ISNULL(vpf_open,0) as vpf_open,ISNULL(vpf_total,0) as vpf_total,ISNULL(bank_share_open,0) as bank_share_open,ISNULL(bank_share_total,0) as bank_share_total,pension_open,pension_total,pension_intrest_amount,ISNULL(bank_share_intrst_open,0) as bank_share_intrst_open,ISNULL(bank_share_intrst_total,0) as bank_share_intrst_total,ISNULL(own_share_intrst_open,0) as own_share_intrst_open,ISNULL(own_share_intrst_total,0)as own_share_intrst_total,ISNULL(vpf_intrst_open,0) as vpf_intrst_open,ISNULL(vpf_intrst_total,0) as vpf_intrst_total from pr_ob_share where fm='" + pfint.Id + "' and  emp_code=" + emp + "";
                    DataTable dt1 = await _sha.Get_Table_FromQry(query2);
                    if (dt1.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in dt1.Rows)
                        {
                            empcode = dr1["emp_code"].ToString();
                            ownshare = Math.Round(Convert.ToDecimal(dr1["own_share"]), 2);
                            bankshare = Math.Round(Convert.ToDecimal(dr1["bank_share"]), 2);
                            vpf = Math.Round(Convert.ToDecimal(dr1["vpf"]), 2);
                            // interest calculation ownshare,vpf,bankshare
                            ownshareinst = Math.Round(((Convert.ToDecimal(ownshare) * Convert.ToDecimal(pfint.Value)) / 100), 2);
                            vpfinst = Math.Round(((Convert.ToDecimal(vpf) * Convert.ToDecimal(pfint.Value)) / 100), 2);
                            bankshareinst = Math.Round(((Convert.ToDecimal(bankshare) * Convert.ToDecimal(pfint.Value)) / 100), 2);
                            //interst total calucalation ownshare,vpf,bankshare
                            ownshareinsttotal = Math.Round((Convert.ToDecimal(dr1["own_share_intrst_open"]) + ownshareinst), 2);
                            bankshareinsttotal = Math.Round((Convert.ToDecimal(dr1["bank_share_intrst_open"]) + bankshareinst), 2);
                            vpfinsttotal = Math.Round((Convert.ToDecimal(dr1["vpf_intrst_open"]) + vpfinst), 2);
                            string queryupdate = " update pr_ob_share  set is_interest_caculated=1,own_share_intrst_amount=" + ownshareinst + ",bank_share_intrst_amount=" + bankshareinst + ",vpf_intrst_amount=" + vpfinst + ",own_share_intrst_total=" + ownshareinsttotal + ",bank_share_intrst_total=" + bankshareinsttotal + ",vpf_intrst_total=" + vpfinsttotal + " where fm='" + pfint.Id + "' and emp_code in (" + emp + " )";
                            if (await _sha.Run_UPDDEL_ExecuteNonQuery(queryupdate))
                            {
                                sucmes = "I#PF Interest Calculations#Processed Successfully";

                            }
                            else
                            {
                                return "E#PF Interest Calculations#Error While PF Interest calculation Process";
                            }

                        }
                    }
                }
                if(sucmes.Contains("I#PF Interest Calculations#Processed Successfully"))
                {
                    string queryupdate2 = " update pr_month_details set is_interest_calculated=1 , interest_percent= " + pfint.Value + " where fm='" + pfint.Id + "'";
                    await _sha.Run_UPDDEL_ExecuteNonQuery(queryupdate2);
                }
                else
                {
                    return "E#PF Interest Calculations#Error While PF Interest calculation Process";
                }
               
            }

            return sucmes;



        }


        // pf contribution card int calculations
        public async Task<string> pfconcardPorcess()
        {
            
            string pfcard = "";
            string empcode = "";
            decimal? pfreturn = 0;
            decimal? vpfreturn = 0;
            decimal? bankreturn = 0;
            string process_date = "";
            decimal? bankshare_interest = 0;
            decimal? ownshare_interest = 0;
            decimal? vpf_interest = 0;
            decimal? pfintrate = 0;
            string currDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            try
            {
                StringBuilder sbqry = new StringBuilder();
                // string query = " select distinct pm.id as Id, format(pm.fm,'yyyy-MM-dd') as fm ,pm.interest_percent as interest from pr_ob_share ob join pr_month_details pm on ob.fm=pm.fm  and pm.is_interest_calculated=0";
                string query = "select distinct m_details.interest_percent, m_details.is_interest_calculated, non_rpay.emp_code,m_details.is_interest_calculated,m_details.fm , non_rpay.process_date,non_rpay.ownshare_interest,non_rpay.bankshare_interest,non_rpay.vpf_interest  " +
                    "from pr_emp_pf_nonrepayable_loan non_rpay join pr_month_details  m_details on  m_details.is_interest_calculated = non_rpay.is_interest_caculated " +
                    "where year(non_rpay.process_date)= year(m_details.fm) and month(non_rpay.process_date)= month(m_details.fm)  and non_rpay.is_interest_caculated = 1 ";
                     DataTable dt = await _sha.Get_Table_FromQry(query);
                foreach (DataRow dr in dt.Rows)
                {
                    empcode = dr["emp_code"].ToString();
                    process_date = dr["process_date"].ToString();
                    pfintrate = Convert.ToDecimal( dr["interest_percent"]);
                    pfintrate = Convert.ToDecimal(pfintrate);
                    string query2 = "select pf_return,bank_return,vpf_return from pr_pf_open_bal_year where emp_code='" + empcode + "' ";
                    DataTable dt2 = await _sha.Get_Table_FromQry(query2);

                        ownshare_interest = Convert.ToDecimal(dr["ownshare_interest"]);
                        bankshare_interest = Convert.ToDecimal(dr["bankshare_interest"]);
                        vpf_interest = Convert.ToDecimal(dr["vpf_interest"]);
                    if (dt2.Rows.Count > 0)
                    {
                        pfreturn = Convert.ToDecimal((dt2.Rows[0]["pf_return"]));
                        bankreturn = Convert.ToDecimal((dt2.Rows[0]["bank_return"]));
                        vpfreturn = Convert.ToDecimal((dt2.Rows[0]["vpf_return"]));
                    }
                    pfreturn =Convert.ToDecimal( pfreturn+ ownshare_interest);
                 
                   // pfreturn = Math.Round(pfreturn);
                    //pfreturn_tot =Convert.ToDecimal( pfreturn + ownshare_interest);
                    bankreturn = Convert.ToDecimal(bankreturn + bankshare_interest);
                    vpfreturn = Convert.ToDecimal(vpfreturn + vpf_interest);

                        string insertQry = "";
                        int NewNumIndex = 0;
                        NewNumIndex++;
                        int Id = 0;
                        //1. trans_id
                        sbqry.Append(GenNewTransactionString());

                    //insertQry += " UPDATE pr_emp_pf_nonrepayable_loan SET is_interest_caculated=1 WHERE emp_code=" + empcode + " and active=1;";
                    //sbqry.Append(insertQry);
                    // (await _sha.Run_UPDDEL_ExecuteNonQuery(insertQry.ToString()))


                    //insertQry = "update pr_pfopeningbal set PFreturn='" + pfreturn + "' ,  Bankreturn='" + bankreturn + "', VPFreturn='" + vpfreturn + "' ,intDate='"+ currDate + "' , pfintrate='"+ pfintrate + "'  where emp_code='" + empcode + "' and  active=1 ";
                    insertQry = "update pr_pf_open_bal_year set pf_return='" + pfreturn + "' ,  bank_return='" + bankreturn + "', vpf_return='" + vpfreturn + "' ,run_date='" + currDate + "' , pf_int_rate = '" + pfintrate + "'  where emp_code = '" + empcode + "'  ";


                    if (await _sha.Run_UPDDEL_ExecuteNonQuery(insertQry.ToString()))
                        {
                            pfcard = "I#PF contribution card Calculations#Processed Successfully";

                        }
                        else
                        {
                            pfcard = "E#PF contribution card Calculations#Error While PF Interest calculation Process";
                        }
                    if (pfcard.Contains("I#PF contribution card Calculations#Processed Successfully"))
                    {
                        string qr = " UPDATE pr_emp_pf_nonrepayable_loan SET is_interest_caculated=1 WHERE emp_code=" + empcode + " and  is_interest_caculated=1 and process_date='"+ process_date + "' ";
                        await _sha.Run_UPDDEL_ExecuteNonQuery(qr);
                    }
                    else
                    {
                        pfcard = "E#PF contribution card Calculations#Error While PF Interest calculation Process";
                    }

                }
               
            }
            catch (Exception e)
            {

            }
            return pfcard;
        }
        

       

    }
}
