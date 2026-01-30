using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayRollBusiness.Process;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Masters
{
    public class PFNonPayableBusiness : BusinessBase
    {
        public PFNonPayableBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        IList<loansOptionss> lstDept = new List<loansOptionss>();
        public async Task<IList<loansOptions>> getLoansPurpose()
        {
            string qrySel = "SELECT id,purpose_name as name,month as value " +
                            "FROM pr_purpose_of_advance_master " +
                            "WHERE ptype='NONREPAY' and purpose_name !='NR 90% Withdrawl'";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);

            IList<loansOptions> lstDept = new List<loansOptions>();
            foreach (DataRow dr in dt.Rows)
            {
                lstDept.Add(new loansOptions
                {
                    id = dr["id"].ToString(),
                    name = dr["name"].ToString(),
                    value = dr["value"].ToString()
                });
            }
            return lstDept;
        }
        public async Task<IList<loansOptions>> getLoansPurposeforretire()
        {
            string qrySel = "SELECT id,purpose_name as name,month as value " +
                            "FROM pr_purpose_of_advance_master " +
                            "WHERE ptype='NONREPAY'";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);

            IList<loansOptions> lstDeptforretire = new List<loansOptions>();
            foreach (DataRow dr in dt.Rows)
            {
                lstDeptforretire.Add(new loansOptions
                {
                    id = dr["id"].ToString(),
                    name = dr["name"].ToString(),
                    value = dr["value"].ToString()
                });
            }
            return lstDeptforretire;
        }

        public class loansOptionss
        {
            public string BrId { get; set; }
            public string Name { get; set; }
        }
        public async Task<IList<loansOptionss>> getloans()
        {
            string loan = "";
            int id = 0;
            IList<loansOptionss> typeval = new List<loansOptionss>();
            loansOptionss crm = new loansOptionss();
            string qr1 = "SELECT id,purpose_name as name,month as value " +
                            "FROM pr_purpose_of_advance_master " +
                            "WHERE ptype='NONREPAY'";
            DataTable dt = await _sha.Get_Table_FromQry(qr1);
            typeval.Insert(0, new loansOptionss
            {
                BrId = "0",
                Name = "ALL"
            });
            foreach (DataRow dr in dt.Rows)
            {
                loan = dr["Name"].ToString();
                id = Convert.ToInt32(dr["Id"].ToString());
                try
                {


                    typeval.Add(new loansOptionss
                    {
                        BrId = id.ToString(),
                        Name = loan,


                    });


                }
                catch (Exception ex)
                {

                }
            }

            return typeval;
        }

        public async Task<string> getPfLoanDocuments(string type, string emp_code)
        {
            string Purposename = "";
            string purposeToSend = "";

            string qry1 = "select pr.purpose_name from pr_emp_pf_nonrepayable_loan pn join pr_purpose_of_advance_master pr on pn.purpose_of_advance = pr.id where pn.emp_code=" + Convert.ToInt32(emp_code) + " and pn.purpose_of_advance=" + Convert.ToInt32(type);

            DataTable dt1 = await _sha.Get_Table_FromQry(qry1);

            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow gen in dt1.Rows)
                {
                    Purposename = gen["purpose_name"].ToString();
                }
                if (Purposename != "")
                {
                    purposeToSend = "E#PF NonRePayable# Employee Already Applied for this " + Purposename + " Purpose.";
                }

            }

            string qrySel = "SELECT id, document_name as name " +
                            "FROM pr_list_of_documents_master " +
                            "WHERE active=1 and loan_id=" + Convert.ToInt32(type);
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);

            IList<loansTypes> lstDept = new List<loansTypes>();

            foreach (DataRow dr in dt.Rows)
            {
                lstDept.Add(new loansTypes
                {
                    id = dr["id"].ToString(),
                    name = dr["name"].ToString(),
                });
            }
            //return lstDept;

            var loan = JsonConvert.SerializeObject(lstDept);

            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var loanData = javaScriptSerializer.DeserializeObject(loan);

            var resultJson = javaScriptSerializer.Serialize(new
            {
                loan = loanData,
                purpose = purposeToSend

            });
            return resultJson;
        }
        public async Task<string> getNonPayableEmpDetails(string empCode)
        {
            try
            {
                DateTime fm = _LoginCredential.FinancialMonthDate;
                DateTime startdate = Convert.ToDateTime("2020-04-01");
                string qry = "select id from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and active=1 and authorisation=0 ;";

                DataTable dt = await _sha.Get_Table_FromQry(qry);

                if (dt.Rows.Count > 0)
                {

                    IList<PfDetails> lstDept = new List<PfDetails>();
                    string genQry = "Select pf.pf_account_no as pf_no,pf.purpose_of_advance,adv.purpose_name,pf.rate_of_basic_da," +
                        "pf.eligibility_amount,pf.own_share,pf.vpf,pf.bank_share,pf.total,adv.month,pf.amount_applied,pf.sanctioned_amount  " +
                        "from pr_emp_pf_nonrepayable_loan pf " +
                        "join pr_purpose_of_advance_master adv on pf.purpose_of_advance = adv.id " +
                        "WHERE pf.emp_code = " + Convert.ToInt32(empCode) + " and pf.active = 1 and authorisation=0 ;";
                    string certQry = "select top 1  id,cert_name,status from pr_emp_pf_non_cert_elg where " +
                        "emp_code=" + Convert.ToInt32(empCode) + " and active=1 and authorisation=0 and process =0;";
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

                    //string selQry = "select case when prnon.own_share is null then o.own_share_total " +
                    //    "else ((o.own_share_total+o.own_share_intrst_total)-prnon.own_share) end as own_share," +
                    //    "case when prnon.vpf is null then o.vpf_total else ((o.vpf_total+vpf_intrst_total) - prnon.vpf) " +
                    //    "end as vpf,case when prnon.bank_share is null then o.bank_share_total " +
                    //    "else ((o.bank_share_total+o.bank_share_intrst_total) - prnon.bank_share) end as bank_share,o.da,o.basic,e.pf_no from " +
                    //    "pr_ob_share o left join pr_emp_pf_nonrepayable_loan prnon on o.emp_code=prnon.emp_code and o.active=1 " +
                    //    " and prnon.process=1 left outer join pr_emp_general e on o.emp_code = e.emp_code " +
                    //    "where o.emp_code =" + Convert.ToInt32(empCode) + " and o.active=1";

                    // code commented 12/03/2021

                    //string selQry = "select case when prnon.own_share is null then o.own_share_total " +
                    //    "else ((o.own_share_total + (case when o.own_share_intrst_total is null then 0 else own_share_intrst_total end))-prnon.own_share) " +
                    //    "end as own_share,case when prnon.vpf is null then o.vpf_total else ((o.vpf_total + (case when vpf_intrst_total is null then 0 else vpf_intrst_total end)) -prnon.vpf)end as vpf," +
                    //    "case when prnon.bank_share is null then o.bank_share_total else ((o.bank_share_total + (case when o.bank_share_intrst_total is null then 0 else o.bank_share_intrst_total  end)) -prnon.bank_share)" +
                    //    "end as bank_share,case when o.da is null then 0 else o.da end as da,case when o.basic is null then 0 else o.basic " +
                    //    "end as basic,e.pf_no from pr_ob_share o left join pr_emp_pf_nonrepayable_loan prnon on o.emp_code = prnon.emp_code " +
                    //    "and o.active = 1 and prnon.process = 1 left outer join pr_emp_general e on " +
                    //    "o.emp_code = e.emp_code where o.emp_code = " + Convert.ToInt32(empCode) + " and o.active = 1";

                    string selQry = " select prnon.own_share ,prnon.vpf as vpf,prnon.bank_share as bank_share, " +
                         " case when o.da is null then 0 else o.da end as da,case when o.basic is null then 0 else o.basic end as basic,e.pf_no " +
                         "from pr_ob_share o left join pr_emp_pf_nonrepayable_loan prnon on o.emp_code = prnon.emp_code and o.active = 1  " +
                         "left outer join pr_emp_general e on o.emp_code = e.emp_code where o.emp_code = " + Convert.ToInt32(empCode) + " and o.active = 1 and prnon.active = 1 ";

                    //string selQry = "select sum(ob.own_share) + pfbal.os_open + os_open_int as own_share,sum(ob.bank_share) + pfbal.bs_open + bs_open_int as bank_share, " +
                    //    "sum(ob.vpf) + pfbal.vpf_open + vpf_open_int as vpf  ,ob.da,ob.basic,e.pf_no " +
                    //    "from pr_ob_share ob left outer join pr_emp_pf_nonrepayable_loan pn on ob.emp_code = pn.emp_code and pn.process = 1  left outer  join pr_emp_general e on ob.emp_code = e.emp_code join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code " +
                    //    "where ob.emp_code = " + Convert.ToInt32(empCode) + " and ob.fm between DATEFROMPARTS((select year(fm) from pr_month_details where active = 1), 04, 01) " +
                    //    "and DATEFROMPARTS((select fy from pr_month_details where active= 1), 03, 31 ) and pfbal.fy = (select year(fm) from pr_month_details where active = 1) " +
                    //    "group by os_open,bs_open,pfbal.vpf_open ,os_open_int,vpf_open_int,bs_open_int,da,basic,pf_no ";

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
                    string basic = "";
                    string da_percent = "";
                    string pf_no = "";
                    string own_share = "";
                    string bank_share = "";
                    string vpf = "";
                    string dor = "";

                    string qrycheck = "select id from pr_ob_share where emp_code=" + Convert.ToInt32(empCode) + " and active=1;";

                    DataTable dtcheck = await _sha.Get_Table_FromQry(qrycheck);

                    if (dtcheck.Rows.Count > 0)
                    {

                        //basic from peay field master table
                        string qrey = "select distinct amount as basic from pr_emp_pay_field where emp_code = " + Convert.ToInt32(empCode) + " and " +
                            "active = 1 and m_id in (select id from pr_earn_field_master where type = 'pay_fields' and name = 'Basic');";
                        DataTable dtbasic = await _sha.Get_Table_FromQry(qrey);
                        if (dtbasic.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtbasic.Rows)
                            {
                                basic = dr["basic"].ToString();
                            }
                        }
                        //da from monthdetails 
                        string qrey2 = " SELECT TOP 1 da_percent as da FROM pr_month_details where active = 1  ORDER BY ID DESC";
                        DataTable das = await _sha.Get_Table_FromQry(qrey2);
                        if (das.Rows.Count > 0)
                        {
                            foreach (DataRow dr in das.Rows)
                            {
                                da_percent = dr["da"].ToString();
                            }
                        }


                        //string selQry = "select o.own_share_total as own_share,o.bank_share_total as bank_share,o.vpf_total as vpf,o.da,o.basic,e.pf_no,format(emp.RetirementDate, 'yyyy,MM,dd') as  RetirementDate from " +
                        //    "pr_ob_share o left outer join pr_emp_general e on o.emp_code = e.emp_code left outer join Employees emp on e.emp_code=emp.EmpId where o.emp_code =" + Convert.ToInt32(empCode) + "and o.active=1";
                        //query added by lalitha for ownshar bank share vpf calculations
                        string selQrypr = "select top 1 case when prnon.own_share is null then (o.own_share_total+(ISNULL(o.own_share_intrst_total, 0 ))) else ((ISNULL(o.own_share_intrst_total, 0 )+o.own_share_total)-(select sum (own_share) from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and process=1)) end as own_share,case when prnon.vpf is null then (o.vpf_total+(ISNULL(o.vpf_intrst_total, 0 ))) else ((ISNULL(o.vpf_intrst_total, 0 )+o.vpf_total) - (select sum (vpf) from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and process=1))end as vpf,case when prnon.bank_share is null then (o.bank_share_total+(ISNULL(o.bank_share_intrst_total, 0 ))) else ((ISNULL(o.bank_share_intrst_total, 0 )+o.bank_share_total) - (select sum (bank_share) from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and process=1)) end as bank_share,o.da,o.basic,e.pf_no as pf_no,format(emp.RetirementDate, 'yyyy,MM,dd') as  RetirementDate from " +
                        "pr_ob_share o left join pr_emp_pf_nonrepayable_loan prnon on o.emp_code=prnon.emp_code and o.active=1  and prnon.process=1 left outer join pr_emp_general e on o.emp_code = e.emp_code join Employees emp on e.emp_code=emp.EmpId where o.emp_code =" + Convert.ToInt32(empCode) + " and o.active=1";
                        DataTable details = await _sha.Get_Table_FromQry(selQrypr);
                        DateTime lastfm = fm.AddMonths(-1);
                        string qrygetnonrepay = "select case when own_share is null then 0 else own_share end own_share,case when vpf is null then 0 else vpf end vpf,case when bank_share is null then 0 else bank_share end bank_share from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and active=1;";
                        string qrygetonbshare = "select case when sum(own_share) is null then 0 else sum(own_share) end as own_share_sum,case when sum(vpf) is null then 0 else sum(vpf) end as vpf_sum, case when sum(bank_share) is null then 0  else sum(bank_share) end as bank_share_sum from pr_ob_share where  emp_code=" + Convert.ToInt32(empCode) + " and fm>='" + startdate.ToString("yyyy-MM-dd") + "' and fm<='" + fm.ToString("yyyy-MM-dd") + "';";
                        string qrygetonbshareadhoc = "select case when sum(own_share) is null then 0 else sum(own_share) end as own_share_sum,case when sum(vpf) is null then 0 else sum(vpf) end as vpf_sum, case when sum(bank_share) is null then 0  else sum(bank_share) end as bank_share_sum from pr_ob_share_adhoc where  emp_code=" + Convert.ToInt32(empCode) + " and fm>='" + startdate.ToString("yyyy-MM-dd") + "' and fm<='" + fm.ToString("yyyy-MM-dd") + "';";
                        string qrygetonbshareencash = "select case when sum(own_share) is null then 0 else sum(own_share) end as own_share_sum, case when sum(vpf) is null then 0 else sum(vpf) end as vpf_sum,case when sum(bank_share) is null then 0  else sum(bank_share) end as bank_share_sum from pr_ob_share_encashment where  emp_code=" + Convert.ToInt32(empCode) + " and fm>='" + startdate.ToString("yyyy-MM-dd") + "' and fm<='" + fm.ToString("yyyy-MM-dd") + "';";
                        string qrygetopbalyear = "select case when os_open is null then 0 else os_open end os_open, ISNULL(os_open_int,0)as os_open_int,case when vpf_open is null then 0 else vpf_open end vpf_open,ISNULL(vpf_open_int,0)as vpf_open_int, case when bs_open is null then 0 else bs_open end bs_open, ISNULL(bs_open_int,0)as bs_open_int from  pr_pf_open_bal_year where emp_code=" + Convert.ToInt32(empCode) + " and fy=" + startdate.Year + ";";
                        //string qrygetnonrepay1 = "select case when sum(own_share) is null then 0 else sum(own_share) end own_share_sum, case when sum(vpf) is null then 0 else sum(vpf) end vpf_sum,case when sum(bank_share) is null then 0 else sum(bank_share) end bank_share_sum from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and year(fm) =" + startdate.Year + ";";
                        string qrygetnonrepay1 = "select own_share as own_share_sum, vpf as vpf_sum,bank_share as bank_share_sum from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and year(fm) =" + startdate.Year + " order by process_date desc;";
                        string qrypfvpfbankreturn = "select pf_return,vpf_return,bank_return from pr_pf_open_bal_year where emp_code=" + Convert.ToInt32(empCode) + " and fy =" + startdate.Year + "";
                        string qryloanadjust = "select principal_balance_amount from pr_emp_adv_loans_adjustments where emp_adv_loans_mid in(select id from pr_emp_adv_loans where emp_code=" + Convert.ToInt32(empCode) + " and loan_type_mid in(16,17,18,19,20,21,26,27)) and fm='" + lastfm.ToString("yyyy-MM-dd") + "'";
                        string qryopenbalyearinst = "select (ISNULL(cast(op_bal_own_inst as decimal), 0) + ISNULL(cast(op_bal_inst_own_year as decimal), 0) - ISNULL(cast(op_bal_inst_nrloan_own as decimal), 0)) as op_bal_own_inst, " +
                            "(ISNULL(cast(op_bal_bank_inst as decimal), 0) + ISNULL(cast(op_bal_inst_bank_year as decimal), 0) - ISNULL(cast(op_bal_inst_nrloan_bank as decimal), 0)) as op_bal_bank_inst, " +
                            "(ISNULL(cast(op_bal_vpf_inst as decimal), 0) + ISNULL(cast(op_bal_inst_vpf_year as decimal), 0) - ISNULL(cast(op_bal_inst_nrloan_vpf as decimal), 0)) as op_bal_vpf_inst " +
                            "from pr_pf_open_bal_year where emp_code=" + Convert.ToInt32(empCode) + " and fy=" + startdate.Year + ";";


                        DataSet dsget = await _sha.Get_MultiTables_FromQry(qrygetnonrepay + qrygetonbshare + qrygetopbalyear + qrygetonbshareadhoc + qrygetonbshareencash + qrygetnonrepay1 + qrypfvpfbankreturn + qryloanadjust+ qryopenbalyearinst);
                        DataTable dtnonrepay = dsget.Tables[0];
                        DataTable dtonshare = dsget.Tables[1];
                        DataTable dtopbalyear = dsget.Tables[2];
                        DataTable dtonshareadhoc = dsget.Tables[3];
                        DataTable dtonshareencash = dsget.Tables[4];
                        DataTable dtnonrepay1 = dsget.Tables[5];
                        DataTable dtpfvpfbankreturn = dsget.Tables[6];
                        DataTable dtloanadjust = dsget.Tables[7];
                        DataTable dtopbalyearinst = dsget.Tables[8];
                        int own_share_new = 0;
                        int bank_share_new = 0;
                        int vpf_new = 0;
                        if (dtonshare.Rows.Count > 0)
                        {
                            own_share_new += Convert.ToInt32(dtonshare.Rows[0]["own_share_sum"]);
                            bank_share_new += Convert.ToInt32(dtonshare.Rows[0]["bank_share_sum"]);
                            vpf_new += Convert.ToInt32(dtonshare.Rows[0]["vpf_sum"]);
                        }
                        if (dtonshareadhoc.Rows.Count > 0)
                        {
                            own_share_new += Convert.ToInt32(dtonshareadhoc.Rows[0]["own_share_sum"]);
                            bank_share_new += Convert.ToInt32(dtonshareadhoc.Rows[0]["bank_share_sum"]);
                            vpf_new += Convert.ToInt32(dtonshareadhoc.Rows[0]["vpf_sum"]);
                        }
                        if (dtonshareencash.Rows.Count > 0)
                        {
                            own_share_new += Convert.ToInt32(dtonshareencash.Rows[0]["own_share_sum"]);
                            bank_share_new += Convert.ToInt32(dtonshareencash.Rows[0]["bank_share_sum"]);
                            vpf_new += Convert.ToInt32(dtonshareencash.Rows[0]["vpf_sum"]);
                        }
                        if (dtopbalyear.Rows.Count > 0)
                        {
                            own_share_new += Convert.ToInt32(dtopbalyear.Rows[0]["os_open"]);
                            bank_share_new += Convert.ToInt32(dtopbalyear.Rows[0]["bs_open"]);
                            vpf_new += Convert.ToInt32(dtopbalyear.Rows[0]["vpf_open"]);
                            own_share_new += Convert.ToInt32(dtopbalyear.Rows[0]["os_open_int"]);
                            bank_share_new += Convert.ToInt32(dtopbalyear.Rows[0]["bs_open_int"]);
                            vpf_new += Convert.ToInt32(dtopbalyear.Rows[0]["vpf_open_int"]);
                        }
                        int owns1 = 0;
                        int banks1 = 0;
                        int vpf1 = 0;
                        if (dtnonrepay.Rows.Count > 0)
                        {
                            own_share_new -= Convert.ToInt32(dtnonrepay.Rows[0]["own_share"]);
                            owns1 = Convert.ToInt32(dtnonrepay.Rows[0]["own_share"]);
                            bank_share_new -= Convert.ToInt32(dtnonrepay.Rows[0]["bank_share"]);
                            banks1 = Convert.ToInt32(dtnonrepay.Rows[0]["bank_share"]);
                            vpf_new -= Convert.ToInt32(dtnonrepay.Rows[0]["vpf"]);
                            vpf1 = Convert.ToInt32(dtnonrepay.Rows[0]["vpf"]); ;
                        }


                        if (dtnonrepay1.Rows.Count > 0)
                        {
                            if (owns1 == 0)
                            {
                                own_share_new -= Convert.ToInt32(dtnonrepay1.Rows[0]["own_share_sum"]);
                            }
                            if (banks1 == 0)
                            {
                                bank_share_new -= Convert.ToInt32(dtnonrepay1.Rows[0]["bank_share_sum"]);
                            }
                            if (vpf1 == 0)
                            {
                                vpf_new -= Convert.ToInt32(dtnonrepay1.Rows[0]["vpf_sum"]);
                            }
                        }
                        if (dtpfvpfbankreturn.Rows.Count > 0)
                        {
                            own_share_new -= Convert.ToInt32(dtpfvpfbankreturn.Rows[0]["pf_return"]);
                            vpf_new -= Convert.ToInt32(dtpfvpfbankreturn.Rows[0]["vpf_return"]);
                            bank_share_new -= Convert.ToInt32(dtpfvpfbankreturn.Rows[0]["bank_return"]);
                        }
                        if (dtloanadjust.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtloanadjust.Rows.Count; i++)
                            {
                                own_share_new -= Convert.ToInt32(dtloanadjust.Rows[i]["principal_balance_amount"]);
                                //vpf_new -= Convert.ToInt32(dtpfvpfbankreturn.Rows[0]["vpf_return"]);
                                //bank_share_new -= Convert.ToInt32(dtpfvpfbankreturn.Rows[0]["bank_return"]);
                            }
                        }
                        if(dtopbalyearinst.Rows.Count>0)
                        {
                            own_share_new += Convert.ToInt32(dtopbalyearinst.Rows[0]["op_bal_own_inst"]);
                            vpf_new += Convert.ToInt32(dtopbalyearinst.Rows[0]["op_bal_vpf_inst"]);
                            bank_share_new += Convert.ToInt32(dtopbalyearinst.Rows[0]["op_bal_bank_inst"]);
                        }

                        IList<PfDetails> lstDept = new List<PfDetails>();
                        foreach (DataRow dr in details.Rows)
                        {


                            pf_no = dr["pf_no"].ToString();
                            //own_share = Convert.ToInt32(dr["own_share"]).ToString();
                            //bank_share = Convert.ToInt32(dr["bank_share"]).ToString();
                            own_share = own_share_new.ToString();
                            bank_share = bank_share_new.ToString();

                            //vpf = Convert.ToInt32(dr["vpf"]).ToString();
                            vpf = vpf_new.ToString();
                            dor = dr["RetirementDate"].ToString();

                        }
                        int ownshare = Convert.ToInt32(own_share);
                        int bankshare = Convert.ToInt32(bank_share);
                        int vpfdata = Convert.ToInt32(vpf);
                        TdsProcessBusiness Tds = new TdsProcessBusiness(_LoginCredential);
                        float b = await Tds.calculateBasic(empCode);
                        decimal basics = Convert.ToDecimal(b);
                        string da = "";
                        float d = await Tds.calculateDa(empCode);
                        decimal daper = (Convert.ToDecimal(da_percent) / 100 * (Convert.ToDecimal(d)));


                        lstDept.Add(new PfDetails
                        {
                            basic = basics.ToString(),
                            pf_no = pf_no,
                            own_share = ownshare,
                            bank_share = bankshare,
                            da_percent = daper.ToString(),
                            vpf = vpfdata,
                            dor = dor,
                        });
                        var loan = JsonConvert.SerializeObject(lstDept);


                        var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        var loanData = javaScriptSerializer.DeserializeObject(loan);


                        var resultJson = javaScriptSerializer.Serialize(new
                        {
                            loan = loanData,


                        });
                        return resultJson;
                    }
                    else
                    {
                        return "E#PF Non Repayable#No Data Found";
                    }


                }
            }
            catch (Exception e)
            {
                return "E#PF Non Payable#No Data Found.";
            }

        }
        //update

        public async Task<string> updatePFNonPayableData(PFNonPayable Values)
        {
            int emp_code = Values.EntityId;
            int NewNumIndex = 0;
            string qry;

            int FY = _LoginCredential.FY;
            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
            decimal? basic_da = 0;
            basic_da = Convert.ToDecimal(Values.basic_da);
            decimal? apply_amount = 0;
            apply_amount = Convert.ToDecimal(Values.apply_amount);
            decimal? own_share = 0;
            own_share = Convert.ToDecimal(Values.own_share);
            decimal? vpf = 0;
            vpf = Convert.ToDecimal(Values.vpn_share);
            decimal? total = 0;
            total = Convert.ToDecimal(Values.total);
            decimal? bank = 0;
            bank = Convert.ToDecimal(Values.bank_share);
            int loanType = Convert.ToInt32(Values.purposeType);
            decimal? elg_amount = 0;
            elg_amount = Convert.ToDecimal(Values.elg_amount);
            decimal? sanction_amount = 0;
            sanction_amount = Convert.ToDecimal(Values.sanction);

            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            NewNumIndex++;
            //sbqry.Append(GetNewNumStringArr("pr_emp_pf_nonrepayable_loan", NewNumIndex));
            qry = "update pr_emp_pf_nonrepayable_loan set rate_of_basic_da=" + basic_da + " ,eligibility_amount=" + elg_amount + ", amount_applied=" + apply_amount + ", sanctioned_amount=" + sanction_amount + " , own_share=" + own_share + ", vpf=" + vpf + ", bank_share=" + bank + "  where emp_code = " + emp_code + " and purpose_of_advance = '" + Values.purposeType + "' ; ";

            sbqry.Append(qry);

            //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_nonrepayable_loan", "@idnew" + NewNumIndex, emp_code.ToString()));

            //certification code

            if (Values.certificates != null)
            {
                foreach (string str in Values.certificates)
                {
                    NewNumIndex++;
                    string[] certs = str.Split(',');
                    int cert_id = Convert.ToInt32(certs[0]);
                    string cert_name = certs[1];
                    string status = certs[2];
                    sbqry.Append(GetNewNumStringArr("pr_emp_pf_non_cert_elg", NewNumIndex));
                    qry = "update pr_emp_pf_non_cert_elg set  status = '" + status + "' where emp_code = " + emp_code + " and cert_id =(select cert_id from pr_emp_pf_non_cert_elg where id=" + cert_id + ") ; ";

                    sbqry.Append(qry);

                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_non_cert_elg", "@idnew" + NewNumIndex, emp_code.ToString()));
                }
            }

            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#PF Non Payable#PF Non Payable Data Updated Successfully..!!";
            }
            else
            {
                return "E#PF Non Payable#Error While PF Non Payable Data Updation";
            }

        }
        public async Task<string> savePFNonPayableData(PFNonPayable Values)
        {
            int emp_code = Values.EntityId;
            int NewNumIndex = 0;
            string qry;
            //int FY = DateTime.Now.Year + 1;
            //string FM = DateTime.Now.ToString("MM-dd-yyyy");

            int FY = _LoginCredential.FY;
            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
            decimal? basic_da = 0;
            basic_da = Convert.ToDecimal(Values.basic_da);
            decimal? apply_amount = 0;
            apply_amount = Convert.ToDecimal(Values.apply_amount);
            decimal? own_share = 0;
            own_share = Convert.ToDecimal(Values.own_share);
            decimal? vpf = 0;
            vpf = Convert.ToDecimal(Values.vpn_share);
            decimal? total = 0;
            total = Convert.ToDecimal(Values.total);
            decimal? bank = 0;
            bank = Convert.ToDecimal(Values.bank_share);
            int loanType = Convert.ToInt32(Values.purposeType);
            decimal? elg_amount = 0;
            elg_amount = Convert.ToDecimal(Values.elg_amount);
            decimal? sanction_amount = 0;
            sanction_amount = Convert.ToDecimal(Values.sanction);

            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            NewNumIndex++;
            sbqry.Append(GetNewNumStringArr("pr_emp_pf_nonrepayable_loan", NewNumIndex));

            qry = "INSERT INTO pr_emp_pf_nonrepayable_loan ([id],[emp_id],[emp_code],[fy],[fm]," +
                "[pf_account_no],[purpose_of_advance],[rate_of_basic_da],[eligibility_amount],[amount_applied]," +
                "[own_share],[vpf],[bank_share],[total],[authorisation],[process],[active],[trans_id],[sanctioned_amount]) " +
                "VALUES(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                "" + emp_code + "," + FY + ",'" + FM + "','" + Values.pf_no + "','" + Values.purposeType + "'," +
                "" + basic_da + "," + elg_amount + "," + apply_amount + "," + own_share + "," + vpf + "," +
                "" + bank + "," + total + ",0,0,1,@transidnew," + sanction_amount + ");";

            sbqry.Append(qry);

            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_nonrepayable_loan", "@idnew" + NewNumIndex, emp_code.ToString()));
            if (Values.certificates != null)
            {
                foreach (string str in Values.certificates)
                {
                    NewNumIndex++;
                    string[] certs = str.Split(',');
                    int cert_id = Convert.ToInt32(certs[0]);
                    string cert_name = certs[1];
                    string status = certs[2];
                    sbqry.Append(GetNewNumStringArr("pr_emp_pf_non_cert_elg", NewNumIndex));

                    qry = "INSERT INTO pr_emp_pf_non_cert_elg ([id],[emp_id],[emp_code],[loan_id]," +
                        "[cert_id],[cert_name],[status],[active],[trans_id],[authorisation],[process]) " +
                        "VALUES(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                        "" + emp_code + "," + loanType + "," + cert_id + ",'" + cert_name + "','" + status + "',1,@transidnew,0,0);";

                    sbqry.Append(qry);

                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_non_cert_elg", "@idnew" + NewNumIndex, emp_code.ToString()));
                }
            }

            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#PF Non Payable#PF Non Payable Data Inserted Successfully..!!";
            }
            else
            {
                return "E#PF Non Payable#Error While PF Non Payable Data Updation";
            }

        }


        //report
        public async Task<IList<CommonReportModel>> PfNonRepayableReport(string loan, string from, string To)
        {

            decimal empcon = 0;
            decimal vpfcon = 0;
            decimal bankcon = 0;
            decimal totals = 0;

            decimal grandtot1 = 0; // added by chaitanya on 11/03/2020
            decimal grandtot2 = 0; // added by chaitanya on 11/03/2020
            //int grndtotcount=0; // added by chaitanya on 11/03/2020
            List<decimal> lstgrndtot = new List<decimal>(); // added by chaitanya on 11/03/2020

            if (loan != "^1")
            {
                string[] loans = loan.Split(',');
                StringBuilder ATypes = new StringBuilder();
                if (loans.Length > 1)
                {
                    foreach (string word in loans)
                    {
                        ATypes.Append("'");
                        ATypes.Append(word);

                        ATypes.Append("', ");
                    }
                    loan = ATypes.ToString(0, ATypes.Length - 2);
                }
            }
            CommonReportModel crm = new CommonReportModel();
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int RowCnt = 0;


            string branch1 = "";
            string purpose = "";
            string oldbranch = "";
            string oldbranch1 = "";
            int SlNo = 1;
            string cond = "";

            string ipmn = "01-01-01";
            // IList<PrrepayableDataModel> lstDept = new List<PrrepayableDataModel>();
            if (from == "^2")
            {
                from = "01-01-01";
            }

            if (To == "^3")
            {
                from = "01-01-01";
            }
            DateTime str = Convert.ToDateTime(from);
            string fromdate = str.ToString("yyyy-MM-dd");
            string[] from_sa = fromdate.Split('-');
            string s1 = from_sa[0];
            string s2 = from_sa[1];
            string s3 = from_sa[2];

            DateTime str1 = Convert.ToDateTime(To);
            string todate = str1.ToString("yyyy-MM-dd");
            string[] to_sa = todate.Split('-');
            string to_s1 = to_sa[0];
            string to_s2 = to_sa[1];
            string to_s3 = to_sa[2];


            if (loan == "^1")
            {
                loan = "0";
            }
            if (loan != "^1" && loan == "ALL")
            {
                cond = "AND non.process=1  group by purpose_name,non.purpose_of_advance,non.own_share,non.vpf,non.bank_share,non.emp_code," +
                    "e.shortname,non.sanction_date order by purpose_name, sanction_date asc  ";

            }
            if (loan != "^1" && loan != "ALL")
            {
                cond = "AND non.process=1 and non.purpose_of_advance in (" + loan + ")  group by purpose_name," +
                    "non.purpose_of_advance,non.own_share,non.vpf,non.bank_share,non.emp_code,e.shortname,non.sanction_date order by purpose_name, sanction_date asc ";

            }

            //string qry = "select  non.emp_code as empcode,e.shortname as sname," +
            //    "non.sanction_date as sdate,adv.purpose_name as purpose,count(*) as No_loans,sum(non.own_share) as Emp_Con, " +
            //    "sum(non.vpf) as VPF_Con,sum(non.bank_share) as Bank_Con,non.own_share + non.vpf + non.bank_share as Total " +
            //    "from pr_emp_pf_nonrepayable_loan non  join pr_purpose_of_advance_master adv on non.purpose_of_advance = adv.id " +
            //    "join pr_loan_master lm on lm.id = non.purpose_of_advance join employees e on non.emp_code = e.EmpId " +
            //    "join Branches b on e.Branch = b.id where  non.sanction_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " ) ";
            string qry = "select  non.emp_code as empcode,e.shortname as sname,non.sanction_date as sdate,adv.purpose_name as purpose," +
                "count(*) as No_loans,sum(non.own_share) as Emp_Con, sum(non.vpf) as VPF_Con,sum(non.bank_share) as Bank_Con," +
                "non.own_share + non.vpf + non.bank_share as Total from pr_emp_pf_nonrepayable_loan non join pr_purpose_of_advance_master adv " +
                "on non.purpose_of_advance = adv.id join pr_loan_master lm on lm.id = non.purpose_of_advance join employees e " +
                "on non.emp_code = e.EmpId join Branches b on e.Branch = b.id where  non.sanction_date " +
                "between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " )";
            if (loan != "" && loan == "0")
            {
                qry += " group by purpose_name, non.purpose_of_advance,non.own_share,non.vpf,non.bank_share,non.emp_code," +
                    "e.shortname,non.sanction_date order by emp_code asc";

            }
            else if (loan != "")
            {
                qry += cond;

            }


            DataTable dt = new DataTable();
            DataSet ds = await _sha.Get_MultiTables_FromQry(qry);
            DataTable dtSalbr = ds.Tables[0];

            foreach (DataRow drs in dtSalbr.Rows)
            {
                purpose = drs["purpose"].ToString();

                if (oldbranch != "" && oldbranch != purpose)
                {
                    //prev. br. footer
                    CommonReportModel tot = getTotal(oldbranch, dtSalbr, empcon, vpfcon, bankcon, totals);
                    grandtot1 = totals; // added by chaitanya on 11/03/2020
                    lstgrndtot.Add(totals); //added by chaitanya on 11 / 03 / 2020
                                            // grndtotcount++; // added by chaitanya on 11/03/2020
                    tot.RowId = RowCnt++;
                    lst.Add(tot);

                    //grp header
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        SlNo = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColHeader(0, "Loan Type", purpose),
                        column2 = "`",
                        column3 = "`",
                        column4 = "`",
                        column5 = "`",
                        column6 = "`",
                        column7 = "`",
                        column8 = "`",
                        column9 = "`",
                    });

                    ////rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2 = "EmpCode",
                    //    column3 = "Name",
                    //    column4 = "Loan Description",
                    //    column5 = "Sanction Date",
                    //    column6 = "From Emp Contribution",
                    //    column7 = "From VPF Contribution",
                    //    column8 = "From Bank Contribution",
                    //    column9 = "Total Loan Amount",

                    //});
                }
                else if (oldbranch == "")
                {
                    //grp header
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        SlNo = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColHeader(0, "Loan Type", purpose),
                        column2 = "`",
                        column3 = "`",
                        column4 = "`",
                        column5 = "`",
                        column6 = "`",
                        column7 = "`",
                        column8 = "`",
                        column9 = "`",
                    });

                    ////rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2 = "EmpCode",
                    //    column3 = "Name",
                    //    column4 = "Loan Description",
                    //    column5 = "Sanction Date",
                    //    column6 = "From Emp Contribution",
                    //    column7 = "From VPF Contribution",
                    //    column8 = "From Bank Contribution",
                    //    column9 = "Total Loan Amount",

                    //});

                }
                oldbranch = drs["purpose"].ToString();
                if (oldbranch1 != purpose)
                {
                    // SlNo = SlNo++;
                    SlNo = 1;
                    empcon = 0;
                    vpfcon = 0;
                    bankcon = 0;
                    totals = 0;
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),
                        column2 = drs["empcode"].ToString(),
                        column3 = drs["sname"].ToString(),
                        column4 = drs["purpose"].ToString(),
                        // column3 = drs["grpcol"].ToString(),
                        column5 = Convert.ToDateTime(drs["sdate"]).ToString("dd/MM/yyyy"),
                        column6 = ReportColConvertToDecimal(drs["Emp_Con"].ToString()),
                        column7 = ReportColConvertToDecimal(drs["VPF_Con"].ToString()),
                        column8 = ReportColConvertToDecimal(drs["Bank_Con"].ToString()),
                        column9 = ReportColConvertToDecimal(drs["Total"].ToString()),
                        column10 = drs["purpose"].ToString()
                    });

                    empcon = empcon + Convert.ToDecimal(drs["Emp_Con"]);
                    vpfcon = vpfcon + Convert.ToDecimal(drs["VPF_Con"]);
                    bankcon = bankcon + Convert.ToDecimal(drs["Bank_Con"]);
                    totals = totals + Convert.ToDecimal(drs["Total"]);
                }
                else
                {
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),
                        column2 = drs["empcode"].ToString(),
                        column3 = drs["sname"].ToString(),
                        column4 = drs["purpose"].ToString(),
                        // column3 = drs["grpcol"].ToString(),
                        column5 = Convert.ToDateTime(drs["sdate"]).ToString("dd/MM/yyyy"),
                        column6 = ReportColConvertToDecimal(drs["Emp_Con"].ToString()),
                        column7 = ReportColConvertToDecimal(drs["VPF_Con"].ToString()),
                        column8 = ReportColConvertToDecimal(drs["Bank_Con"].ToString()),
                        column9 = ReportColConvertToDecimal(drs["Total"].ToString())

                    });

                    empcon = empcon + Convert.ToDecimal(drs["Emp_Con"]);
                    vpfcon = vpfcon + Convert.ToDecimal(drs["VPF_Con"]);
                    bankcon = bankcon + Convert.ToDecimal(drs["Bank_Con"]);
                    totals = totals + Convert.ToDecimal(drs["Total"]);

                }
                oldbranch1 = drs["purpose"].ToString();
            }
            if (oldbranch != "")
            {
                CommonReportModel tot = getTotal(oldbranch, dtSalbr, empcon, vpfcon, bankcon, totals);
                grandtot2 = totals; //added by chaitanya on 11 / 03 / 2020
                lstgrndtot.Add(totals);//added by chaitanya on 11 / 03 / 2020
                //grndtotcount++; //added by chaitanya on 11 / 03 / 2020
                tot.RowId = RowCnt++;
                lst.Add(tot);

            }

            //added by chaitanya on 11 / 03 / 2020 -- start
            if (oldbranch != "")
            {
                decimal sum = 0;
                for (int i = 0; i < lstgrndtot.Count; i++)
                {
                    sum += lstgrndtot[i];
                }
                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    SlNo = "Grand Total",
                    column9 = ReportColConvertToDecimal(sum.ToString())
                });
            }
            //end


            return lst;

        }
        //report
        public async Task<IList<CommonReportModel>> PfNonRepayableEffReport(string loan, string from, string To)
        {


            decimal empcon = 0;
            decimal vpfcon = 0;
            decimal bankcon = 0;
            decimal totals = 0;
            if (loan != "^1")
            {
                string[] loans = loan.Split(',');
                StringBuilder ATypes = new StringBuilder();
                if (loans.Length > 1)
                {
                    foreach (string word in loans)
                    {
                        ATypes.Append("'");
                        ATypes.Append(word);

                        ATypes.Append("', ");
                    }
                    loan = ATypes.ToString(0, ATypes.Length - 2);
                }
            }
            CommonReportModel crm = new CommonReportModel();
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int RowCnt = 0;


            string branch1 = "";
            string oldbranch = "";
            string oldbranch1 = "";
            int SlNo = 1;
            string cond = "";

            string ipmn = "01-01-01";
            // IList<PrrepayableDataModel> lstDept = new List<PrrepayableDataModel>();
            if (from == "^2")
            {
                from = "01-01-01";
            }

            if (To == "^3")
            {
                To = "01-01-01";
            }
            DateTime str = Convert.ToDateTime(from);
            string fromdate = str.ToString("yyyy-MM-dd");
            string[] from_sa = fromdate.Split('-');
            string s1 = from_sa[0];
            string s2 = from_sa[1];
            string s3 = from_sa[2];

            DateTime str1 = Convert.ToDateTime(To);
            string todate = str1.ToString("yyyy-MM-dd");
            string[] to_sa = todate.Split('-');
            string to_s1 = to_sa[0];
            string to_s2 = to_sa[1];
            string to_s3 = to_sa[2];


            if (loan == "^1")
            {
                loan = "0";
            }

            if (loan != "^1" && loan == "ALL")
            {
                cond = "AND non.process=1   group by b.name,purpose_name,non.purpose_of_advance,non.own_share,non.vpf,non.bank_share,non.emp_code,e.shortname," +
                    "non.process_date,non.sanction_date,des.code order by non.sanction_date desc ";

            }
            if (loan != "^1" && loan != "ALL")
            {
                cond = "AND non.process=1 and non.purpose_of_advance in (" + loan + ")  group by b.name,purpose_name,non.purpose_of_advance,non.own_share," +
                    "non.vpf,non.bank_share,non.emp_code,e.shortname,non.process_date,non.sanction_date,des.code order by non.sanction_date desc";

            }


            string qry = "select  des.code as desig,non.process_date as pdate,non.emp_code as empcode,e.shortname as sname," +
                "case when b.Name!='OtherBranch' then b.Name else 'HeadOffice' end as grpcol,non.sanction_date as sdate,adv.purpose_name as purpose,count(*) as No_loans," +
                "sum(non.own_share) as Emp_Con, sum(non.vpf) as VPF_Con,sum(non.bank_share) as Bank_Con,non.own_share + non.vpf + non.bank_share as Total" +
                " from pr_emp_pf_nonrepayable_loan non  join pr_purpose_of_advance_master adv on non.purpose_of_advance = adv.id " +
                "join pr_loan_master lm on lm.id = non.purpose_of_advance join employees e on non.emp_code = e.EmpId " +
                "join Branches b on e.Branch = b.id join designations des on des.id=e.currentdesignation where non.process_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " ) ";
            if (loan != "" && loan == "0")
            {
                qry += " group by b.name,purpose_name, non.purpose_of_advance,non.own_share,non.vpf,non.bank_share,non.emp_code,e.shortname,non.sanction_date,non.process_date,des.code order by non.sanction_date desc";

            }
            else if (loan != "")
            {
                qry += cond;

            }


            DataTable dt = new DataTable();
            DataSet ds = await _sha.Get_MultiTables_FromQry(qry);
            DataTable dtSalbr = ds.Tables[0];

            foreach (DataRow drs in dtSalbr.Rows)
            {
                branch1 = drs["grpcol"].ToString();
                // SlNo = 1;
                if (oldbranch != "" && oldbranch != branch1)
                {
                    ////prev. br. footer
                    //CommonReportModel tot = getTotaleffective(oldbranch, dtSalbr, empcon, vpfcon, bankcon, totals);
                    //tot.RowId = RowCnt++;
                    //lst.Add(tot);
                    //SlNo = 1;
                    ////grp header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",
                    //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //            + ReportColHeader(0, "Branch", branch1),
                    //    column2 = "`",
                    //    column3 = "`",
                    //    column4 = "`",
                    //    column5 = "`",
                    //    column6 = "`",
                    //    column7 = "`",
                    //    column8 = "`",
                    //    column9 = "`",
                    //    column10 = "`",
                    //    column11 = "`",
                    //});

                    //rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2="EmpCode",
                    //    column3 = "Name",
                    //    column4 = "Design",
                    //    column5 = "Loan Description",
                    //    column6 = "Sanction Date",
                    //    column7 = "Process Date",
                    //    column8 = "From Emp Contribution",
                    //    column9 = "From VPF Contribution",
                    //    column10 = "From Bank Contribution",
                    //    column11 = "Total Loan Amount",

                    //});
                }
                else if (oldbranch == "")
                {
                    ////grp header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",
                    //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //            + ReportColHeader(0, "Branch", branch1),
                    //    column2 = "`",
                    //    column3 = "`",
                    //    column4 = "`",
                    //    column5 = "`",
                    //    column6 = "`",
                    //    column7 = "`",
                    //    column8 = "`",
                    //    column9 = "`",
                    //    column10 = "`",
                    //    column11 = "`",
                    //});

                    //rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2 = "EmpCode",
                    //    column3 = "Name",
                    //    column4 = "Design",
                    //    column5 = "Loan Description",
                    //    column6 = "Sanction Date",
                    //    column7 = "Process Date",
                    //    column8 = "From Emp Contribution",
                    //    column9 = "From VPF Contribution",
                    //    column10 = "From Bank Contribution",
                    //    column11 = "Total Loan Amount",

                    //});

                }
                oldbranch = drs["grpcol"].ToString();
                //if (oldbranch1 != branch1)
                //{
                SlNo = SlNo++;
                //empcon = 0;
                //vpfcon = 0;
                //bankcon = 0;
                //totals = 0;
                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    SlNo = SlNo++.ToString(),
                    column2 = drs["empcode"].ToString(),
                    column3 = drs["sname"].ToString(),
                    column4 = drs["desig"].ToString(),
                    column5 = drs["purpose"].ToString(),
                    // column3 = drs["grpcol"].ToString(),
                    column6 = Convert.ToDateTime(drs["sdate"]).ToString("dd/MM/yyyy"),
                    column7 = Convert.ToDateTime(drs["pdate"]).ToString("dd/MM/yyyy"),
                    column8 = ReportColConvertToDecimal(drs["Emp_Con"].ToString()),
                    column9 = ReportColConvertToDecimal(drs["VPF_Con"].ToString()),
                    column10 = ReportColConvertToDecimal(drs["Bank_Con"].ToString()),
                    column11 = ReportColConvertToDecimal(drs["Total"].ToString()),
                    column12 = drs["purpose"].ToString()
                });



                empcon = empcon + Convert.ToDecimal(drs["Emp_Con"]);
                vpfcon = vpfcon + Convert.ToDecimal(drs["VPF_Con"]);
                bankcon = bankcon + Convert.ToDecimal(drs["Bank_Con"]);
                totals = totals + Convert.ToDecimal(drs["Total"]);
                //}
                //else
                //{
                //    SlNo = SlNo++;
                //    lst.Add(new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        SlNo = SlNo++.ToString(),
                //        column2 = drs["empcode"].ToString(),
                //        column3 = drs["sname"].ToString(),
                //        column4 = drs["desig"].ToString(),
                //        column5 = drs["purpose"].ToString(),
                //        // column3 = drs["grpcol"].ToString(),
                //        column6 = Convert.ToDateTime(drs["sdate"]).ToString("dd/MM/yyyy"),
                //        column7 = Convert.ToDateTime(drs["pdate"]).ToString("dd/MM/yyyy"),
                //        column8 = drs["Emp_Con"].ToString(),
                //        column9 = drs["VPF_Con"].ToString(),
                //        column10 = drs["Bank_Con"].ToString(),
                //        column11 = drs["Total"].ToString()
                //    });

                //    empcon = empcon + Convert.ToDecimal(drs["Emp_Con"]);
                //    vpfcon = vpfcon + Convert.ToDecimal(drs["VPF_Con"]);
                //    bankcon = bankcon + Convert.ToDecimal(drs["Bank_Con"]);
                //    totals = totals + Convert.ToDecimal(drs["Total"]);
                //}
                oldbranch1 = drs["grpcol"].ToString();
            }
            if (oldbranch != "")
            {
                CommonReportModel tot = getTotaleffective(oldbranch, dtSalbr, empcon, vpfcon, bankcon, totals);
                tot.RowId = RowCnt++;
                lst.Add(tot);

            }


            return lst;

        }


        //report2
        public async Task<IList<CommonReportModel>> PfNonRepayableReportEffective(string loan, string from, string To)
        {

            string purpose = "";
            string oldpurpose = "";
            decimal noofloans = 0;
            decimal empcon = 0;
            decimal vpfcon = 0;
            decimal bankcon = 0;
            decimal totals = 0;
            if (loan != "^1")
            {
                string[] loans = loan.Split(',');
                StringBuilder ATypes = new StringBuilder();
                if (loans.Length > 1)
                {
                    foreach (string word in loans)
                    {
                        ATypes.Append("'");
                        ATypes.Append(word);

                        ATypes.Append("', ");
                    }
                    loan = ATypes.ToString(0, ATypes.Length - 2);
                }
            }
            CommonReportModel crm = new CommonReportModel();
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int RowCnt = 0;


            string branch1 = "";
            string oldbranch = "";
            string oldbranch1 = "";
            int SlNo = 1;
            string cond = "";

            //string ipmn = "01-01-01";
            // IList<PrrepayableDataModel> lstDept = new List<PrrepayableDataModel>();

            DateTime Fdate = DateTime.Now, Tdate = DateTime.Now;
            if (from == "^2")
            {
                from = "01-01-01";
            }

            if (To == "^3" || To == null)
            {
                To = "01-01-01";
            }
            DateTime str = Convert.ToDateTime(from);
            string fromdate = str.ToString("yyyy-MM-dd");
            string[] from_sa = fromdate.Split('-');
            string s1 = from_sa[0];
            string s2 = from_sa[1];
            string s3 = from_sa[2];

            DateTime str1 = Convert.ToDateTime(To);
            string todate = str1.ToString("yyyy-MM-dd");
            string[] to_sa = todate.Split('-');
            string to_s1 = to_sa[0];
            string to_s2 = to_sa[1];
            string to_s3 = to_sa[2];



            if (loan == "^1")
            {
                loan = "0";
            }
            if (loan != "^1" && loan == "ALL")
            {
                cond = "AND a.process=1   group by purpose_of_advance,purpose_name ";

            }
            if (loan != "^1" && loan != "ALL" && from != "")
            {
                cond = "AND a.process=1 and  b.id in (" + loan + ") group by purpose_of_advance,purpose_name ";

            }

            //string qry = "select e.EmpId,e.ShortName,non.sanction_date, case when b.Name!='OtherBranch' then b.Name else 'HeadOffice' end as grpcol,adv.purpose_name as purpose,count(*) as No_loans," +
            //    "sum(non.own_share) as Emp_Con, sum(non.vpf) as VPF_Con,sum(non.bank_share) as Bank_Con,non.own_share + non.vpf + non.bank_share as Total,adv.purpose_name as loantype" +
            //    " from pr_emp_pf_nonrepayable_loan non  join pr_purpose_of_advance_master adv on non.purpose_of_advance = adv.id " +
            //    "join pr_loan_master lm on lm.id = non.purpose_of_advance join employees e on non.emp_code = e.EmpId join Branches b on e.Branch = b.id " +
            //    "where  non.sanction_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " ) ";

            string qry = " select count(purpose_of_advance) as No_loans,b.purpose_name as purpose,sum(a.own_share)as Emp_Con," +
                "sum(a.vpf)as  VPF_Con, sum(a.bank_share)as Bank_Con,sum(a.own_share+a.vpf+ a.bank_share) as total from pr_emp_pf_nonrepayable_loan a  " +
                "join pr_purpose_of_advance_master b on a.purpose_of_advance = b.id where a.sanction_date between '" + fromdate + "' and '" + todate + "' ";

            //string qry = "select  case when b.Name!='OtherBranch' then b.Name else 'HeadOffice' end as grpcol,adv.purpose_name as purpose,count(*) as No_loans," +
            //    "sum(non.own_share) as Emp_Con, sum(non.vpf) as VPF_Con,sum(non.bank_share) as Bank_Con,non.own_share + non.vpf + non.bank_share as Total" +
            //    " from pr_emp_pf_nonrepayable_loan non  join pr_purpose_of_advance_master adv on non.purpose_of_advance = adv.id " +
            //    "join pr_loan_master lm on lm.id = non.purpose_of_advance join employees e on non.emp_code = e.EmpId join Branches b on e.Branch = b.id " +
            //    "where  non.sanction_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " )";
            //if (loan != "" && loan == "0")
            //{
            //    qry += " group by b.name,purpose_name, non.purpose_of_advance,non.own_share,non.vpf,non.bank_share";

            //}
            if (loan != "")
            {
                qry += cond;

            }

            DataTable dt = new DataTable();
            DataSet ds = await _sha.Get_MultiTables_FromQry(qry);
            DataTable dtSalbr = ds.Tables[0];



            foreach (DataRow drs in dtSalbr.Rows)
            {

                branch1 = drs["purpose"].ToString();

                if (oldbranch != "" && oldbranch != branch1)
                {
                    ////prev. br. footer
                    //CommonReportModel tot = getTotal1(oldbranch, dtSalbr, noofloans, empcon, vpfcon, bankcon, totals);
                    //tot.RowId = RowCnt++;
                    //lst.Add(tot);

                    ////grp header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",
                    //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //            + ReportColHeader(0, "Loan Type  ", branch1),
                    //    //column2 = "`",
                    //    column3 = "`",
                    //    column4 = "`",
                    //    column5 = "`",
                    //    column6 = "`",
                    //    column7 = "`",
                    //});

                    //rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",

                    //    column2 = "Type Of Loan",
                    //    column3 = "No of Loans",
                    //    column4 = "From Emp Contribution",
                    //    column5 = "From VPF Contribution",
                    //    column6 = "From Bank Contribution",
                    //    column7 = "Total Loan Amount",

                    //});
                }
                else if (oldbranch == "")
                {
                    ////grp header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",
                    //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //            + ReportColHeader(0, "Loan Type  ", branch1),
                    //    //column2 = "`",
                    //    column3 = "`",
                    //    column4 = "`",
                    //    column5 = "`",
                    //    column6 = "`",
                    //    column7 = "`",
                    //});

                    //rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2 = "Type Of Loan",
                    //    column3 = "No of Loans",
                    //    column4 = "From Emp Contribution",
                    //    column5 = "From VPF Contribution",
                    //    column6 = "From Bank Contribution",
                    //    column7 = "Total Loan Amount",

                    //});

                }
                oldbranch = drs["purpose"].ToString();
                //if (oldbranch1 != branch1)
                //{
                //noofloans = 0;
                //empcon = 0;
                //vpfcon = 0;
                //bankcon = 0;
                //totals = 0;
                //SlNo = 1;
                SlNo = SlNo++;
                decimal Demp_con = Convert.ToDecimal(drs["Emp_Con"].ToString()) + 0.00M;
                string NDemp_con = String.Format("{0:n}", Demp_con);
                decimal DVPF_Con = Convert.ToDecimal(drs["VPF_Con"].ToString()) + 0.00M;
                string NVPF_Con = String.Format("{0:n}", DVPF_Con);
                decimal DBank_Con = Convert.ToDecimal(drs["Bank_Con"].ToString()) + 0.00M;
                string NBank_Con = String.Format("{0:n}", DBank_Con);
                decimal Dtotal = Convert.ToDecimal(drs["total"].ToString()) + 0.00M;
                string Ntotal = String.Format("{0:n}", Dtotal);

                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    SlNo = SlNo++.ToString(),

                    column2 = drs["purpose"].ToString(),
                    // column3 = drs["grpcol"].ToString(),
                    column3 = drs["No_loans"].ToString(),
                    column4 = NDemp_con,
                    column5 = NVPF_Con,
                    column6 = NBank_Con,
                    column7 = Ntotal,

                    //column8 = drs["empid"].ToString(),
                    // column9 = drs["shortname"].ToString(),
                    // column10=Convert.ToDateTime(drs["sanction_date"]).ToString("dd/MM/yyyy"),
                    // column11= drs["loantype"].ToString()

                });
                noofloans = noofloans + Convert.ToDecimal(drs["No_loans"]);
                empcon = empcon + Convert.ToDecimal(drs["Emp_Con"]);
                vpfcon = vpfcon + Convert.ToDecimal(drs["VPF_Con"]);
                bankcon = bankcon + Convert.ToDecimal(drs["Bank_Con"]);
                totals = totals + (Convert.ToDecimal(drs["Emp_Con"]) + Convert.ToDecimal(drs["VPF_Con"]) + Convert.ToDecimal(drs["Bank_Con"]));
                //}
                //else
                //{
                //    lst.Add(new CommonReportModel
                //    {
                //        RowId = RowCnt++,
                //        SlNo = SlNo++.ToString(),

                //       //column2 = drs["purpose"].ToString(),
                //        // column3 = drs["grpcol"].ToString(),
                //        column3 = drs["No_loans"].ToString(),
                //        column4 = drs["Emp_Con"].ToString(),
                //        column5 = drs["VPF_Con"].ToString(),
                //        column6 = drs["Bank_Con"].ToString(),
                //        column7 = drs["Total"].ToString(),

                //        column8 = drs["empid"].ToString(),
                //        column9 = drs["shortname"].ToString(),
                //        column10 = drs["sanction_date"].ToString()
                //    });
                //    noofloans = noofloans + Convert.ToDecimal(drs["No_loans"]);
                //    empcon = empcon + Convert.ToDecimal(drs["Emp_Con"]);
                //    vpfcon = vpfcon + Convert.ToDecimal(drs["VPF_Con"]);
                //    bankcon = bankcon + Convert.ToDecimal(drs["Bank_Con"]);
                //    totals = totals + Convert.ToDecimal(drs["Total"]);
                //}
                oldbranch1 = drs["purpose"].ToString();
            }
            if (oldbranch != "")
            {
                CommonReportModel tot = getTotal1(oldbranch, dtSalbr, noofloans, empcon, vpfcon, bankcon, totals);
                tot.RowId = RowCnt++;
                lst.Add(tot);

            }


            return lst;

        }




        private CommonReportModel getTotal1(string branch, DataTable dt, decimal noofloans, decimal empcon, decimal vpfcon, decimal bankcon, decimal totals)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["purpose"].ToString() == branch)
              //.Select(x => new { tot = x["Total"].ToString() }).FirstOrDefault();
              .Select(x => new { tot = noofloans.ToString() + "~" + empcon.ToString() + "~" + vpfcon.ToString() + "~" + bankcon.ToString() + "~" + totals.ToString() }).FirstOrDefault();
            var arrTots = val.tot.Split('~');
            decimal DTtlEmpCont = Convert.ToDecimal(arrTots[1].ToString()) + 0.00M;
            string NTtlEmpCont = String.Format("{0:n}", DTtlEmpCont);
            decimal DTtlVpfCont = Convert.ToDecimal(arrTots[2].ToString()) + 0.00M;
            string NTtlVpfCont = String.Format("{0:n}", DTtlVpfCont);
            decimal DTtlBankCont = Convert.ToDecimal(arrTots[3].ToString()) + 0.00M;
            string NTtlBankCont = String.Format("{0:n}", DTtlBankCont);
            decimal dTtlLoanAmt = Convert.ToDecimal(arrTots[4].ToString()) + 0.00M;
            string NTtlLoanAmt = String.Format("{0:n}", dTtlLoanAmt);
            var tot = new CommonReportModel
            {
                RowId = 0,
                HRF = "F",
                //SlNo = "<span style='color:#eef8fd'>^</span>"
                //+ ReportColFooterValueOnly(20, "Total  ")
                //  + ReportColFooterValueOnly(20, arrTots[0])
                //+ ReportColFooterValueOnly(18, arrTots[1])
                //+ ReportColFooterValueOnly(28, arrTots[2])
                //+ ReportColFooterValueOnly(31, arrTots[3])
                //+ ReportColFooterValueOnly(31, arrTots[4])
                SlNo = "Grand Total",
                column3 = arrTots[0],
                column4 = NTtlEmpCont,
                column5 = NTtlVpfCont,
                column6 = NTtlBankCont,
                column7 = NTtlLoanAmt
            };

            return tot;
        }
        private CommonReportModel getTotal(string branch, DataTable dt, decimal empcon, decimal vpfcon, decimal bankcon, decimal totals)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["purpose"].ToString() == branch)
              //.Select(x => new { tot = x["Total"].ToString() }).FirstOrDefault();
              .Select(x => new { tot = empcon.ToString() + "~" + vpfcon.ToString() + "~" + bankcon.ToString() + "~" + totals.ToString() }).FirstOrDefault();
            var arrTots = val.tot.Split('~');


            var tot = new CommonReportModel
            {
                RowId = 0,
                HRF = "F",
                //SlNo = "<span style='color:#eef8fd'>^</span>"
                //+ ReportColFooterValueOnly(10, "Total  ")
                //  + ReportColFooterValueOnly(55, arrTots[0])
                // + ReportColFooterValueOnly(22, arrTots[1])
                // + ReportColFooterValueOnly(17, arrTots[2])
                // + ReportColFooterValueOnly(19, arrTots[3])
                SlNo = "Total",
                column6 = ReportColConvertToDecimal(arrTots[0]),
                column7 = ReportColConvertToDecimal(arrTots[1]),
                column8 = ReportColConvertToDecimal(arrTots[2]),
                column9 = ReportColConvertToDecimal(arrTots[3]),

            };

            return tot;
        }



        private CommonReportModel getTotaleffective(string branch, DataTable dt, decimal empcon, decimal vpfcon, decimal bankcon, decimal totals)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["grpcol"].ToString() == branch)
              //.Select(x => new { tot = x["Total"].ToString() }).FirstOrDefault();
              .Select(x => new { tot = empcon.ToString() + "~" + vpfcon.ToString() + "~" + bankcon.ToString() + "~" + totals.ToString() }).FirstOrDefault();
            var arrTots = val.tot.Split('~');


            var tot = new CommonReportModel
            {
                RowId = 0,
                HRF = "F",
                // SlNo = "<span style='color:#eef8fd'>^</span>"
                //+ ReportColFooterValueOnly(40, "Total  ")
                //  + ReportColFooterValueOnly(55, arrTots[0])
                // + ReportColFooterValueOnly(22,  arrTots[1])
                // + ReportColFooterValueOnly(17,  arrTots[2])
                // + ReportColFooterValueOnly(19,  arrTots[3])
                SlNo = "Total",
                column8 = ReportColConvertToDecimal(arrTots[0]),
                column9 = ReportColConvertToDecimal(arrTots[1]),
                column10 = ReportColConvertToDecimal(arrTots[2]),
                column11 = ReportColConvertToDecimal(arrTots[3]),
            };

            return tot;
        }

        public async Task<string> DeleteLoan(string purposeType, string emp_code)
        {
            string qry = "";
            string retMessage = "";
            string id = "";
            StringBuilder sbqry = new StringBuilder();

            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            try
            {
                qry = "delete from pr_emp_pf_nonrepayable_loan where purpose_of_advance = (select  id from pr_purpose_of_advance_master where purpose_name = '" + purposeType + "' and ptype='NONREPAY')  and authorisation = 0 ";
                sbqry.Append(qry);

                qry = "delete from pr_emp_pf_non_cert_elg where emp_code=" + emp_code + " ";
                sbqry.Append(qry);

                //4. transaction touch 
                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_nonrepayable_loan", id.ToString(), ""));

                //}
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    retMessage = "I#Loan # Deleted Successfully";
                }

            }
            catch (Exception e)
            {
                string msg = e.Message;
                return "E#Error:#" + msg;
            }
            return retMessage;
        }

        public string ReportColConvertToDecimal(string value)
        {

            if (value == "")
            {
                value = "0";
            }
            decimal Drvalue = Convert.ToDecimal(value.ToString()) + 0.00M;
            decimal DPT = Convert.ToDecimal(String.Format("{0:0.00}", Drvalue));
            string NwDPT = String.Format("{0:n}", DPT);


            return NwDPT;
        }

    }
}
