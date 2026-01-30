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
using System.Configuration;
using PayRollBusiness.Process;
using System.Linq;
namespace PayRollBusiness.Masters
{
    public class PFPayableBusiness : BusinessBase
    {
        CommonReportModel crm = new CommonReportModel();
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        public PFPayableBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();

        public async Task<IList<loansOptions>> getLoansPurpose()
        {
            string qrySel = "SELECT id,purpose_name as name,month as value,ptype " +
                            "FROM pr_purpose_of_advance_master " +
                            "WHERE active=1 and ptype='REPAY'  ";
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
        public async Task<IList<loansOptions>> getLoansPFPayable()
        {
            string qrySel = "select id,loan_id,interest_rate,loan_description from pr_loan_master where loan_id in('PFL1','PFL2') and active=1";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);

            IList<loansOptions> lstDept = new List<loansOptions>();
            foreach (DataRow dr in dt.Rows)
            {
                lstDept.Add(new loansOptions
                {
                    id = dr["loan_id"].ToString(),
                    name = dr["loan_description"].ToString(),
                    value = dr["interest_rate"].ToString(),
                });
            }
            return lstDept;
        }
        public async Task<string> getPfLoanData(string empCode, string type)
        {


            IList<PfrepayableDetails> lstDept = new List<PfrepayableDetails>();
            try
            {
                string qry = "select pf.pf_account_no as pf_no,pf.purpose_of_advance as padvid,adv.purpose_name as " +
                    "purpose,pf.pf_loans_id as loanid,pf.amount_applied_for as amountapp," +
                    " pf.[(basic+DA)*months_1] as basicData,pf.rate_of_interest as rate,pf.amount_applied_for_2 as " +
                    "amountapplied2,pf.calculating_months as calmon,[netownshare+25%netbankshare_3] as netshare,[least_of_3] " +
                    "as leastt,pf.[gross_salary] as grossamt,pf.net_salary as netamt,pf.net_minus_pf as netminuspf,pf.[1/3rd_of_gross_salary] " +
                    "as onethirdg,pf.total_outstanding_loan as total,pf.amount_recommended_for_sanction as sanction,pf.active from pr_emp_pf_repayable_loan pf " +
                    "join pr_purpose_of_advance_master adv on pf.purpose_of_advance = adv.id  where pf.active=1 and pf.purpose_of_advance in ('" + type + "') " +
                    "and pf.emp_code=" + Convert.ToInt32(empCode);

                DataTable dt = await _sha.Get_Table_FromQry(qry);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow gen in dt.Rows)
                    {


                        lstDept.Add(new PfrepayableDetails
                        {
                            ploanadv = gen["padvid"].ToString(),
                            basic = gen["basicData"].ToString(),
                            pf_no = gen["pf_no"].ToString(),
                            purpose_name = gen["purpose"].ToString(),

                            da_percent = gen["leastt"].ToString(),
                            sanctionamt = gen["sanction"].ToString(),
                            gross = gen["grossamt"].ToString(),
                            net = gen["netamt"].ToString(),
                            pf1 = gen["amountapp"].ToString(),
                            own_share = Convert.ToInt32(gen["netshare"]),
                            firstinstall = gen["netminuspf"].ToString(),
                            calm = gen["calmon"].ToString(),
                            least = gen["leastt"].ToString(),
                            applyamount1 = gen["amountapp"].ToString(),
                            rate_of_basic_da = gen["rate"].ToString(),
                            applyamount2 = gen["amountapplied2"].ToString(),

                            pftype = gen["loanid"].ToString(),

                        });
                    }

                    //return lstDept;
                    var loan = JsonConvert.SerializeObject(lstDept);

                    var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var loanData = javaScriptSerializer.DeserializeObject(loan);


                    var resultJson = javaScriptSerializer.Serialize(new
                    {
                        loan = loanData

                    });
                    return resultJson;
                }
                else
                {

                    string basicData = "";
                    string pf_no = "";
                    string date_of_join = "";
                    string own_share = "";

                    string da_percent = "";
                    string vpfData = "";
                    string grossamt = "";
                    string netamt = "";
                    string bank_share = "";
                    string pf1 = "";
                    string pf2 = "";
                    string selQry = "";
                    string qrycheck = "select id from pr_ob_share where emp_code=" + Convert.ToInt32(empCode) + " and active=1;";
                    DataTable dtcheck = await _sha.Get_Table_FromQry(qrycheck);

                    if (dtcheck.Rows.Count > 0)
                    {
                        selQry = "select distinct case when prnon.own_share is null then (o.own_share_total+o.own_share_intrst_total) else ((ISNULL(o.own_share_intrst_total, 0 )+o.own_share_total)-prnon.own_share) end as own_share,case when prnon.vpf is null then (o.vpf_total+o.vpf_intrst_total) else ((ISNULL(o.vpf_intrst_total, 0 )+o.vpf_total) - prnon.vpf )end as vpf,case when prnon.bank_share is null then (o.bank_share_total+o.bank_share_intrst_total) else ((ISNULL(o.bank_share_intrst_total, 0 )+o.bank_share_total) - prnon.bank_share) end as bank_share,o.da,o.basic,e.pf_no as pf_no,format(emp.RetirementDate, 'yyyy,MM,dd') as  RetirementDate from " +
                                           "pr_ob_share o left join pr_emp_pf_nonrepayable_loan prnon on o.emp_code=prnon.emp_code and o.active=1  and prnon.process=1 left outer join pr_emp_general e on o.emp_code = e.emp_code join Employees emp on e.emp_code=emp.EmpId where o.emp_code =" + Convert.ToInt32(empCode) + "and o.active=1";
                        //basic,pfno,vpf,ownshare,bankshare
                        //selQry = "select o.own_share_total as own_share,o.bank_share_total as bank_share,o.vpf_total as vpf,o.da,o.basic,e.pf_no,format(emp.RetirementDate, 'yyyy,MM,dd') as  RetirementDate from " +
                        //  "pr_ob_share o left outer join pr_emp_general e on o.emp_code = e.emp_code left outer join Employees emp on e.emp_code=emp.EmpId where o.emp_code =" + Convert.ToInt32(empCode) + "and o.active=1";

                        DataTable dt1 = await _sha.Get_Table_FromQry(selQry);
                        if (dt1.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt1.Rows)
                            {
                                basicData = dr["basic"].ToString();
                                pf_no = dr["pf_no"].ToString();
                                own_share = Convert.ToInt32(dr["own_share"]).ToString(); ;
                                bank_share = Convert.ToInt32(dr["bank_share"]).ToString(); ;
                                da_percent = dr["da"].ToString();
                                vpfData = Convert.ToInt32(dr["vpf"]).ToString(); ;
                            }
                        }
                    }
                    //Get basicamount 
                    string qryda3 = "SELECT m.name as payfieldtype,amount,c.m_id FROM pr_emp_pay_field c JOIN pr_earn_field_master m ON c.m_id=m.id WHERE c.emp_code=" + Convert.ToInt32(empCode) + "  AND c.active=1 AND amount>0;";
                    DataTable dadt = await _sha.Get_Table_FromQry(qryda3);
                    foreach (DataRow ebd in dadt.Rows)
                    {


                    }



                    string qry1 = " select gross_amount as grossamt from pr_emp_payslip where emp_code =" + Convert.ToInt32(empCode) + " and " +
                   "active = 1 ;";
                    DataTable dt2 = await _sha.Get_Table_FromQry(qry1);
                    if (dt2.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt2.Rows)
                        {
                            grossamt = dr["grossamt"].ToString();
                        }
                    }

                    string qry2 = "select net_amount as netamt from pr_emp_payslip where emp_code =" + Convert.ToInt32(empCode) + " and " +
                      "active = 1 ;";

                    DataTable dt3 = await _sha.Get_Table_FromQry(qry2);
                    if (dt3.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt3.Rows)
                        {
                            netamt = dr["netamt"].ToString();
                        }
                    }
                    string qry3 = "select amount_applied_for as pfloan2,emp_code from  pr_emp_pf_repayable_loan where active=1 and pf_loans_id=1 and emp_code =" + Convert.ToInt32(empCode) + " ;";
                    DataTable dt4 = await _sha.Get_Table_FromQry(qry3);
                    if (dt4.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt4.Rows)
                        {
                            pf1 = dr["pfloan2"].ToString();
                        }
                    }


                    string qry4 = "select amount_applied_for as pfloan2,emp_code from  pr_emp_pf_repayable_loan where active=1 and pf_loans_id=2 and emp_code =" + Convert.ToInt32(empCode) + " ;";
                    DataTable dt5 = await _sha.Get_Table_FromQry(qry4);
                    if (dt5.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt5.Rows)
                        {
                            pf2 = dr["pfloan2"].ToString();
                        }
                    }



                    lstDept.Add(new PfrepayableDetails
                    {
                        basic = basicData,
                        pf_no = pf_no,
                        date_of_join = date_of_join,
                        own_share = Convert.ToInt32(own_share),
                        bank_share = Convert.ToInt32(bank_share),
                        da_percent = da_percent,
                        vpf = Convert.ToInt32(vpfData),
                        gross = grossamt,
                        net = netamt,
                        pf1 = pf1,
                        pf2 = pf2,
                    });


                }
            }
            catch (Exception e)
            {
                return "E#PF Payable#" + e.Message;
            }
            //return lstDept;
            var loan1 = JsonConvert.SerializeObject(lstDept);

            var javaScriptSerializer1 = new System.Web.Script.Serialization.JavaScriptSerializer();
            var loanData1 = javaScriptSerializer1.DeserializeObject(loan1);


            var resultJson1 = javaScriptSerializer1.Serialize(new
            {
                loan = loanData1

            });
            return resultJson1;
        }

        public async Task<string> getPayableEmpDetails(string empCode)
        {
            TdsProcessBusiness Tds = new TdsProcessBusiness(_LoginCredential);
            string da_percent1 = "";
            string basicData1 = "";
            IList<PfrepayableDetails> lstDept = new List<PfrepayableDetails>();
            try
            {
                string qry = "select pf.pf_account_no as pf_no,pf.purpose_of_advance as padvid,adv.purpose_name as purpose,pr_emp_adv_loans.total_amount,pf.pf_loans_id as loanid,pf.amount_applied_for as amountapp," +
                    " pf.[(basic+DA)*months_1] as basicData,pf.rate_of_interest as rate,pf.amount_applied_for_2 as amountapplied2,pf.calculating_months as calmon,[netownshare+25%netbankshare_3] as netshare,[least_of_3] as leastt,pf.[gross_salary] as grossamt,pf.net_salary as netamt,pf.net_minus_pf as netminuspf,pf.[1/3rd_of_gross_salary] as onethirdg,pf.total_outstanding_loan as total,pf.amount_recommended_for_sanction as sanction,pf.active from pr_emp_pf_repayable_loan pf left outer join pr_purpose_of_advance_master adv on pf.purpose_of_advance = adv.id left outer join pr_loan_master on pr_loan_master.loan_id=pf.pf_loans_id left outer  join pr_emp_adv_loans on pr_emp_adv_loans.loan_type_mid=pr_loan_master.id and pr_emp_adv_loans.emp_code=" + Convert.ToInt32(empCode) + " where pf.active=1 and authorisation=0 and pf.emp_code=" + Convert.ToInt32(empCode);

                //string qry = "select id from pr_emp_pf_repayable_loan where active=1 and emp_code=" + Convert.ToInt32(empCode);
                DataTable dt = await _sha.Get_Table_FromQry(qry);

                string qrey1 = "select distinct amount as basic from pr_emp_pay_field where emp_code = " + Convert.ToInt32(empCode) + " and " +
                        "active = 1 and m_id in (select id from pr_earn_field_master where type = 'pay_fields' and name = 'Basic');";
                DataTable dtbasic1 = await _sha.Get_Table_FromQry(qrey1);
                if (dtbasic1.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtbasic1.Rows)
                    {
                        basicData1 = dr["basic"].ToString();
                    }
                }
                //da from monthdetails 
                string qrey3 = " SELECT TOP 1 da_percent as da FROM pr_month_details where active = 1  ORDER BY ID DESC";
                DataTable das1 = await _sha.Get_Table_FromQry(qrey3);
                if (das1.Rows.Count > 0)
                {
                    foreach (DataRow dr in das1.Rows)
                    {
                        da_percent1 = dr["da"].ToString();
                    }
                }

                float b1 = await Tds.calculateBasic(empCode);
                decimal basic1 = Convert.ToDecimal(b1);

                float d1 = await Tds.calculateDa(empCode);

                decimal bac1 = Convert.ToDecimal(da_percent1) / 100 * (Convert.ToDecimal(d1));
                //covid
                decimal covid1 = (basic1 + bac1) * 3;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow gen in dt.Rows)
                    {

                        lstDept.Add(new PfrepayableDetails
                        {
                            basic_da = Convert.ToString(covid1),
                            ploanadv = gen["padvid"].ToString(),
                            basic = gen["basicData"].ToString(),
                            pf_no = gen["pf_no"].ToString(),
                            purpose_name = gen["purpose"].ToString(),
                            da_percent = gen["leastt"].ToString(),
                            sanctionamt = gen["sanction"].ToString(),
                            gross = gen["grossamt"].ToString(),
                            net = gen["netamt"].ToString(),
                            pf1 = gen["total_amount"].ToString(),
                            own_share = Convert.ToInt32(gen["netshare"]),
                            firstinstall = gen["netminuspf"].ToString(),
                            calm = gen["calmon"].ToString(),
                            least = gen["leastt"].ToString(),
                            applyamount1 = gen["amountapp"].ToString(),
                            rate_of_basic_da = gen["rate"].ToString(),
                            applyamount2 = gen["amountapplied2"].ToString(),

                            pftype = gen["loanid"].ToString(),

                        });


                    }



                    //return lstDept;
                    var loan = JsonConvert.SerializeObject(lstDept);

                    var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var loanData = javaScriptSerializer.DeserializeObject(loan);


                    var resultJson = javaScriptSerializer.Serialize(new
                    {
                        loan = loanData

                    });
                    return resultJson;
                }

                else
                {

                    string basicData = "";
                    string pf_no = "";
                    string date_of_join = "";
                    string own_share = "";
                    string calculmon = "";
                    string da_percent = "";
                    string vpfData = "";
                    string grossamt = "";
                    string netamt = "";
                    string bank_share = "";
                    string pf1 = "";
                    string pf2 = "";
                    string selQry = "";
                    string dor = "";

                    //basic from peay field master table
                    string qrey = "select distinct amount as basic from pr_emp_pay_field where emp_code = " + Convert.ToInt32(empCode) + " and " +
                        "active = 1 and m_id in (select id from pr_earn_field_master where type = 'pay_fields' and name = 'Basic');";
                    DataTable dtbasic = await _sha.Get_Table_FromQry(qrey);
                    if (dtbasic.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtbasic.Rows)
                        {
                            basicData = dr["basic"].ToString();
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
                    string qrycheck = "select id from pr_ob_share where emp_code=" + Convert.ToInt32(empCode) + " and active=1;";
                    DataTable dtcheck = await _sha.Get_Table_FromQry(qrycheck);
                    string vpfbankown_amount = await getown_bank_vpf(empCode);
                    string[] amounts= vpfbankown_amount.Split(',');
                    
                    if (dtcheck.Rows.Count > 0)
                    {
                        //pfno,vpf,ownshare,bankshare
                        //selQry = "select o.own_share_total as own_share,o.bank_share_total as bank_share,o.vpf_total as vpf,o.da,o.basic,e.pf_no,format(emp.RetirementDate, 'yyyy,MM,dd') as  RetirementDate from " +
                        //  "pr_ob_share o left outer join pr_emp_general e on o.emp_code = e.emp_code left outer join Employees emp on e.emp_code=emp.EmpId where o.emp_code =" + Convert.ToInt32(empCode) + "and o.active=1";
                        selQry = "select top 1 case when prnon.own_share is null then (o.own_share_total+(ISNULL(o.own_share_intrst_total, 0 ))) else ((ISNULL(o.own_share_intrst_total, 0 )+o.own_share_total)-(select sum (own_share) from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and process=1)) end as own_share,case when prnon.vpf is null then (o.vpf_total+(ISNULL(o.vpf_intrst_total, 0 ))) else ((ISNULL(o.vpf_intrst_total, 0 )+o.vpf_total) - (select sum (vpf) from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and process=1))end as vpf,case when prnon.bank_share is null then (o.bank_share_total+(ISNULL(o.bank_share_intrst_total, 0 ))) else ((ISNULL(o.bank_share_intrst_total, 0 )+o.bank_share_total) - (select sum (bank_share) from pr_emp_pf_nonrepayable_loan where emp_code=" + Convert.ToInt32(empCode) + " and process=1)) end as bank_share,o.da,o.basic,e.pf_no as pf_no,format(emp.RetirementDate, 'yyyy,MM,dd') as  RetirementDate from " +
                     "pr_ob_share o left join pr_emp_pf_nonrepayable_loan prnon on o.emp_code=prnon.emp_code and o.active=1  and prnon.process=1 left outer join pr_emp_general e on o.emp_code = e.emp_code join Employees emp on e.emp_code=emp.EmpId where o.emp_code =" + Convert.ToInt32(empCode) + " and o.active=1";
                        DataTable dt1 = await _sha.Get_Table_FromQry(selQry);
                        if (dt1.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt1.Rows)
                            {

                                pf_no = dr["pf_no"].ToString();
                                own_share = Convert.ToInt32(dr["own_share"]).ToString();
                                bank_share = Convert.ToInt32(dr["bank_share"]).ToString();

                                vpfData = Convert.ToInt32(dr["vpf"]).ToString();
                                dor = dr["RetirementDate"].ToString();
                            }
                        }
                    }
                    if (amounts.Length > 0)
                    {
                        bank_share = amounts[0];
                        own_share = amounts[1];
                        vpfData = amounts[2];
                    }
                    int monthDiff = GetMonthDifference(Convert.ToDateTime(dor), DateTime.Now);
                    if (monthDiff >= 60)
                    {
                        calculmon = "60";
                    }
                    else
                    {
                        calculmon = monthDiff.ToString();
                    }
                    //TdsProcessBusiness Tds = new TdsProcessBusiness(_LoginCredential);
                    float b = await Tds.calculateBasic(empCode);
                    decimal basic = Convert.ToDecimal(b);

                    float d = await Tds.calculateDa(empCode);

                    decimal bac = Convert.ToDecimal(da_percent) / 100 * (Convert.ToDecimal(d));
                    //covid
                    decimal covid = (basic + bac) * 3;
                    decimal basicss = (basic + bac) * 10;
                    string basics = Convert.ToString(basicss);
                    string covids = Convert.ToString(covid);
                    string qry1 = " select gross_amount as grossamt from pr_emp_payslip where emp_code =" + Convert.ToInt32(empCode) + " and " +
                       "active = 1 ;";
                    DataTable dt2 = await _sha.Get_Table_FromQry(qry1);
                    if (dt2.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt2.Rows)
                        {
                            grossamt = dr["grossamt"].ToString();
                        }
                    }

                    string qry2 = "select net_amount as netamt from pr_emp_payslip where emp_code =" + Convert.ToInt32(empCode) + " and " +
                      "active = 1 ;";

                    DataTable dt3 = await _sha.Get_Table_FromQry(qry2);
                    if (dt3.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt3.Rows)
                        {
                            netamt = dr["netamt"].ToString();
                        }
                    }
                    string qry3 = "select total_amount from pr_emp_adv_loans join   pr_loan_master on pr_loan_master.id=pr_emp_adv_loans.loan_type_mid where  pr_loan_master.loan_id='PFL1' and emp_code =" + Convert.ToInt32(empCode) + " ;";
                    DataTable dt4 = await _sha.Get_Table_FromQry(qry3);
                    if (dt4.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt4.Rows)
                        {
                            pf1 = dr["total_amount"].ToString();
                        }
                    }


                    string qry4 = "select total_amount from pr_emp_adv_loans join   pr_loan_master on pr_loan_master.id=pr_emp_adv_loans.loan_type_mid where  pr_loan_master.loan_id='PFL2' and emp_code =" + Convert.ToInt32(empCode) + " ;";
                    DataTable dt5 = await _sha.Get_Table_FromQry(qry4);
                    if (dt5.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt5.Rows)
                        {
                            pf2 = dr["total_amount"].ToString();
                        }
                    }



                    //string share = (((Convert.ToDecimal(own_share) + Convert.ToDecimal(vpfData))) + ((Convert.ToDecimal(bank_share) * 25) / 100)).ToString();
                    string share = (((Convert.ToDecimal(own_share) + Convert.ToDecimal(vpfData))) + ((Convert.ToDecimal(bank_share) * 25) / 100)).ToString();


                    //Principal* ROI*((1 + ROI)n / ((1 + ROI)n - 1))

                    lstDept.Add(new PfrepayableDetails
                    {
                        //3 % covid calculation
                        basic_da = covids,
                        //10 remaing ones
                        basic = basics,
                        pf_no = pf_no,
                        date_of_join = date_of_join,
                        //own_share = Convert.ToDecimal(share),
                        own_share = Convert.ToDecimal(own_share),
                        bank_share = Convert.ToDecimal(bank_share),
                        da_percent = da_percent,
                        vpf = Convert.ToInt32(vpfData),
                        gross = grossamt,
                        calm = calculmon,
                        net = netamt,
                        pf1 = pf1,
                        pf2 = pf2,
                    });


                }
            }
            catch (Exception e)
            {
                return "E#PF Payable#" + e.Message;
            }
            //return lstDept;
            var loan1 = JsonConvert.SerializeObject(lstDept);

            var javaScriptSerializer1 = new System.Web.Script.Serialization.JavaScriptSerializer();
            var loanData1 = javaScriptSerializer1.DeserializeObject(loan1);


            var resultJson1 = javaScriptSerializer1.Serialize(new
            {
                loan = loanData1

            });
            return resultJson1;
        }
        public async Task<string> savePFPayableData(PFPayable Values)
        {
            IList<PfrepayableDetails> lstDept = new List<PfrepayableDetails>();
            string qry = "";
            try
            {
                string Purposename = "";
                string pfltype = "";
                string pfname = "";
                string qry1 = "select pf.pf_account_no as pf_no,adv.purpose_name as purpose,pf.pf_loans_id as loanid,pf.amount_applied_for as amountapp," +
                   " pf.[(basic+DA)*months_1] as basicData,pf.rate_of_interest as rate,pf.amount_applied_for_2 as amountapplied2,pf.calculating_months as calmon,[netownshare+25%netbankshare_3] as netshare,[least_of_3] as leastt,pf.[gross_salary] as grossamt,pf.net_salary as netamt,pf.net_minus_pf as netminuspf,pf.[1/3rd_of_gross_salary] as onethirdg,pf.total_outstanding_loan as total,pf.amount_recommended_for_sanction as sanction,pf.active from pr_emp_pf_repayable_loan pf join pr_purpose_of_advance_master adv on pf.purpose_of_advance = adv.id  where pf.purpose_of_advance=" + Values.purposeType + " and  pf.active=1   and pf.emp_code=" + Values.EntityId;

                DataTable dt = await _sha.Get_Table_FromQry(qry1);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow gen in dt.Rows)
                    {
                        Purposename = gen["purpose"].ToString();
                        pfltype = gen["loanid"].ToString();
                    }
                    if (Purposename != "")
                    {
                        return "E#PF Payable#Already Applied for this purpose select another Purpose";
                    }

                }
                string qry2 = "select pf.pf_account_no as pf_no,adv.purpose_name as purpose,pf.pf_loans_id as loanid,pf.amount_applied_for as amountapp," +
                   " pf.[(basic+DA)*months_1] as basicData,pf.rate_of_interest as rate,pf.amount_applied_for_2 as amountapplied2,pf.calculating_months as calmon,[netownshare+25%netbankshare_3] as netshare,[least_of_3] as leastt,pf.[gross_salary] as grossamt,pf.net_salary as netamt,pf.net_minus_pf as netminuspf,pf.[1/3rd_of_gross_salary] as onethirdg,pf.total_outstanding_loan as total,pf.amount_recommended_for_sanction as sanction,pf.active from pr_emp_pf_repayable_loan pf join pr_purpose_of_advance_master adv on pf.purpose_of_advance = adv.id  where pf.pf_loans_id in(1,2) and pf.purpose_of_advance=" + Values.purposeType + " and  pf.active=1   and pf.emp_code=" + Values.EntityId;
                DataTable dt1 = await _sha.Get_Table_FromQry(qry2);
                if (dt1.Rows.Count > 0)
                {

                    return "E#PF Payable#Already Applied Cannot apply  another Loan";


                }


                string qrey4 = "select pf.pf_account_no as pf_no,adv.purpose_name as purpose,pf.pf_loans_id as loanid,pf.amount_applied_for as amountapp," +
                 " pf.[(basic+DA)*months_1] as basicData,pf.rate_of_interest as rate,pf.amount_applied_for_2 as amountapplied2,pf.calculating_months as calmon,[netownshare+25%netbankshare_3] as netshare,[least_of_3] as leastt,pf.[gross_salary] as grossamt,pf.net_salary as netamt,pf.net_minus_pf as netminuspf,pf.[1/3rd_of_gross_salary] as onethirdg,pf.total_outstanding_loan as total,pf.amount_recommended_for_sanction as sanction,pf.active from pr_emp_pf_repayable_loan pf join pr_purpose_of_advance_master adv on pf.purpose_of_advance = adv.id  where pf.active=1   and pf.emp_code=" + Values.EntityId;
                DataTable dtt2 = await _sha.Get_Table_FromQry(qrey4);
                if (dtt2.Rows.Count > 0)
                {

                    foreach (DataRow gen in dtt2.Rows)
                    {

                        pfname = gen["loanid"].ToString();
                        if (Values.pfloansid == "PFL2" && pfname == "PFL2")
                        {
                            return "E#PF Payable#Already Applied PF Loan 2 Cannot apply Same loan select another PF Loan Type";
                        }
                        else if (Values.pfloansid == "PFL1" && pfname == "PFL1")
                        {
                            return "E#PF Payable#Already Applied PF Loan 1 Cannot apply Same loan select another PF Loan Type";
                        }
                    }


                }

                //string qry4 = "select pf.pf_account_no as pf_no,adv.purpose_name as purpose,pf.pf_loans_id as loanid,pf.amount_applied_for as amountapp," +
                //  " pf.[(basic+DA)*months_1] as basicData,pf.rate_of_interest as rate,pf.amount_applied_for_2 as amountapplied2,pf.calculating_months as calmon,[netownshare+25%netbankshare_3] as netshare,[least_of_3] as leastt,pf.[gross_salary] as grossamt,pf.net_salary as netamt,pf.net_minus_pf as netminuspf,pf.[1/3rd_of_gross_salary] as onethirdg,pf.total_outstanding_loan as total,pf.amount_recommended_for_sanction as sanction,pf.active from pr_emp_pf_repayable_loan pf join pr_purpose_of_advance_master adv on pf.purpose_of_advance = adv.id  where pf.pf_loans_id ='"+ pfname + "' and pf.active=1   and pf.emp_code=" + Values.EntityId;
                // DataTable dt2 = await _sha.Get_Table_FromQry(qry4);
                //if (dtt2.Rows.Count > 0)
                //{

                //    return "E#PF Payable#Already Applied Cannot apply Same loan select another PF Loan Type";


                //}

                int lempid = _LoginCredential.EmpCode;
                int emp_code = Values.EntityId;
                int NewNumIndex = 0;
                //string qry="";
                int FY = _LoginCredential.FY;
                string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");

                decimal? basic_da = 0;
                basic_da = Convert.ToDecimal(Values.basic_da);
                decimal? eligibility_amount = 0;
                eligibility_amount = Convert.ToDecimal(Values.apply_amount);
                string pfno = Values.pf_no;
                int puposeofadv = Convert.ToInt32(Values.purposeType);
                string pfloanid = Values.pfloansid.ToString();
                decimal? amountapp = Convert.ToDecimal(Values.apply_amount);
                decimal? basicmonthda = Convert.ToDecimal(Values.basic_da);
                decimal? rate = Convert.ToDecimal(Values.rate_of_interest);
                decimal? amountapp2 = Convert.ToDecimal(Values.amount_applied_for_2);
                string calculating_months = Values.calculating_months;
                decimal? ownshare = Convert.ToDecimal(Values.netownsharenetbankshare);
                decimal? least3 = Convert.ToDecimal(Values.least_of_3);
                decimal? grosssal = Convert.ToDecimal(Values.gross_salary);
                decimal? net_salary = Convert.ToDecimal(Values.net_salary);
                decimal? net_minus_pf = Convert.ToDecimal(Values.net_minus_pf);
                decimal? onethirdgross = Convert.ToDecimal(Values.rdofgrosssalary);
                decimal? total_outstanding_loan = Convert.ToDecimal(Values.apply_amount);
                decimal? amount_recommended_for_sanction = Convert.ToDecimal(Values.amountrecommendedforsanction);
                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());
                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_emp_pf_repayable_loan", NewNumIndex));

                qry = "INSERT INTO pr_emp_pf_repayable_loan ([id],[emp_id],[emp_code],[fy],[fm]," +
                    "[pf_account_no],[purpose_of_advance],[pf_loans_id],[amount_applied_for]," +
                    "[(basic+DA)*months_1],[rate_of_interest],[amount_applied_for_2],[calculating_months],[netownshare+25%netbankshare_3],[least_of_3],[gross_salary],[net_salary],[net_minus_pf],[1/3rd_of_gross_salary],[total_outstanding_loan],[amount_recommended_for_sanction],[authorisation],[process],[active],[trans_id]) " +
                    "VALUES(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                    "" + emp_code + "," + FY + ",'" + FM + "','" + Values.pf_no + "','" + Values.purposeType + "'," +
                    "'" + pfloanid + "'," + amountapp + "," + basicmonthda + "," + rate + "," +
                    "" + amountapp2 + "," + calculating_months + "," + ownshare + "," + least3 + "," + grosssal + "," + net_salary + "," + net_minus_pf + "," + onethirdgross + "," + total_outstanding_loan + "," + amount_recommended_for_sanction + ",0,0,1,@transidnew);";

                sbqry.Append(qry);

                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_repayable_loan", lempid.ToString(), ""));


                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#PF Payable#PF Payable Data Inserted Successfully..!!";
                }
                else
                {
                    return "E#PF Payable#Error While PF Payable Data Insertion";
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return "";


        }
        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }


        private string ReportColHeader(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + Mavensoft.Common.PrConstants.PDF_REPORT_HEADER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "_";
            sRet += "</span>";

            sRet += "<span>" + lable + ": <b>" + value + "</b></span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }
        private string ReportColFooter(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + Mavensoft.Common.PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "_";
            sRet += "</span>";

            sRet += "<span>" + lable + ": " + value + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }



        private CommonReportModel getTotal(string branch, DataTable dt)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["grpcol"].ToString() == branch)
                .Select(x => new { tot = x["Total"].ToString() }).FirstOrDefault();

            var arrTots = val.tot;


            var tot = new CommonReportModel
            {
                RowId = 0,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>"
                + ReportColFooter(50, "Sanction Amount", arrTots)

            };

            return tot;
        }

        public async Task<IList<CommonReportModel>> GetPFrepayableLoanGroupingReports(string empCode, string fromdate, string todate)
        {

            string grpclmn = "";
            int RowCnt = 0;
            string name = "";
            string empcode = "";
            string PfLoanType = "";
            string Disignation = "";
            string samt = "";
            string sdate = "";
            string Purposeofadvance = "";
            // IList<PrrepayableDataModel> lstDept = new List<PrrepayableDataModel>();
            DateTime Fdate = DateTime.Now, Tdate = DateTime.Now;
            if (fromdate != "^2")
            {
                Fdate = Convert.ToDateTime(fromdate);
                Tdate = Convert.ToDateTime(todate);
            }
            else
            {
                Fdate = DateTime.Now.Date;
                Tdate = DateTime.Now.Date;
            }
            string qry = "";
            int SlNo = 1;
            //var empid = "0";
            string branch1 = "";
            string oldbranch = "";
            string empcodes = empCode;
            //string[] empids = empcodes.Split(',');
            //foreach (var id in empids)
            //{
            //empid = id;

            if (empCode.Contains("^"))
            {
                empcodes = "0";

            }
            //qry = "select ((case when b.Name = 'OtherBranch' then dept.Name when b.Name = 'HeadOffice' then dept.Name else b.Name end )+'           '+lm.loan_description ) as grpcol, convert(varchar, w.EmpId) as EmpCode,w.Shortname as name,d.Code as Desig, case when b.Name = 'OtherBranch' then dept.Name when b.Name = 'HeadOffice' then dept.Name else b.Name end as branch,lm.loan_description,al.amount_recommended_for_sanction as samt,Convert(varchar,al.fm,105) as sdate from pr_emp_pf_repayable_loan al join pr_loan_master on al.pf_loans_id = pr_loan_master.id join Employees w on w.empid = al.emp_code join branches b on b.id = w.branch join departments dept on dept.id = w.department join designations d on d.id = w.currentdesignation inner join pr_loan_master lm on lm.id = al.pf_loans_id  where  ((al.fm >= '" + Fdate+ "' and al.fm <=  '" + Tdate+"') or (al.fm >= '"+Fdate+"' and al.fm <= '"+Fdate+ "')) ";
            qry = "select case when b.Name = 'OtherBranch' then dept.Name when b.Name = 'HeadOffice' then dept.Name else b.Name end as grpcol, convert(varchar, w.EmpId) as EmpCode," +
                "w.Shortname as name,lm.loan_description as LoanDescription,d.Code as Designation, " +
                "lm.loan_description,pam.purpose_name as Purposeofadvance," +
                "al.amount_recommended_for_sanction as samt,Convert(varchar,al.sanction_date,105) as sdate from pr_emp_pf_repayable_loan al " +
                "join pr_loan_master on al.pf_loans_id = pr_loan_master.loan_id join Employees w on w.empid = al.emp_code " +
                "join branches b on b.id = w.branch join departments dept on dept.id = w.department join designations d on d.id = w.currentdesignation " +
                "join pr_purpose_of_advance_master pam on pam.id = al.purpose_of_advance " +
                "inner join pr_loan_master lm on lm.loan_id = al.pf_loans_id  where process=1 and  ((al.sanction_date >=convert(date, '" + Fdate + "',102) and al.sanction_date <= convert(date, '" + Tdate + "',102)) or (al.sanction_date >= convert(date,'" + Fdate + "',102) and al.sanction_date <= convert(date,'" + Fdate + "',102)) )";

            if (empCode != "All")
            {
                qry += " and al.emp_code in (" + empcodes + ") ";

            }
            string qry2 = " select case when b.Name = 'OtherBranch' then dept.Name when b.Name = 'HeadOffice' then dept.Name else b.Name end  as grpcol,sum(al.amount_recommended_for_sanction) as Total from pr_emp_pf_repayable_loan al join pr_loan_master on al.pf_loans_id = pr_loan_master.loan_id join Employees w on w.empid = al.emp_code join branches b on b.id = w.branch join departments dept on dept.id = w.department join designations d on d.id = w.currentdesignation join pr_purpose_of_advance_master pam on pam.id = al.purpose_of_advance inner join pr_loan_master lm on lm.loan_id = al.pf_loans_id where process=1 and  ((al.sanction_date >=convert(date, '" + Fdate + "',102) and al.sanction_date <= convert(date, '" + Tdate + "',102)) or (al.sanction_date >= convert(date,'" + Fdate + "',102) and al.sanction_date <= convert(date,'" + Fdate + "',102)) ) group by b.name,al.purpose_of_advance,dept.name";
            DataTable dt = new DataTable();
            DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qry2);
            DataTable dtSalbr = ds.Tables[0];
            DataTable dtSalbr1 = ds.Tables[1];



            foreach (DataRow drs in dtSalbr.Rows)
            {
                branch1 = drs["grpcol"].ToString();

                if (oldbranch != "" && oldbranch != branch1)
                {
                    //prev. br. footer
                    CommonReportModel tot = getTotal(oldbranch, dtSalbr1);
                    tot.RowId = RowCnt++;
                    lst.Add(tot);

                    //grp header
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        SlNo = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColHeader(0, "Branch", branch1)
                    });

                    //rows header
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        SlNo = "S.No",
                        column2 = "Name",
                        column3 = "Emp Code",
                        column4 = "PfLoanType",
                        column5 = "Disignation",
                        column6 = "Sanction Amount",
                        column7 = "Sanction Date",
                        column8 = "Purpose of Advance",
                    });
                }
                else if (oldbranch == "")
                {
                    //grp header
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        SlNo = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColHeader(0, "Branch", branch1)
                    });

                    //rows header
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        SlNo = "S.No",
                        column2 = "Name",
                        column3 = "Emp Code",
                        column4 = "PfLoanType",
                        column5 = "Disignation",
                        column6 = "Sanction Amount",
                        column7 = "Sanction Date",
                        column8 = "Purpose of Advance",
                    });

                }
                oldbranch = drs["grpcol"].ToString();
                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    SlNo = SlNo++.ToString(),
                    // grpclmn = drs["grpcol"].ToString(),
                    column2 = drs["name"].ToString(),
                    column3 = drs["EmpCode"].ToString(),
                    column4 = drs["LoanDescription"].ToString(),
                    column5 = drs["Designation"].ToString(),
                    column6 = drs["samt"].ToString(),
                    column7 = drs["sdate"].ToString(),
                    column8 = drs["Purposeofadvance"].ToString()
                });
            }
            if (oldbranch != "")
            {
                CommonReportModel tot = getTotal(oldbranch, dtSalbr1);
                tot.RowId = RowCnt++;
                lst.Add(tot);

            }

            return lst;

        }


        //update
        public async Task<string> UpdatePFPayableData(PFPayable Values)
        {
            IList<PfrepayableDetails> lstDept = new List<PfrepayableDetails>();
            string qry = "";
            try
            {

                int lempid = _LoginCredential.EmpCode;
                int emp_code = Values.EntityId;
                int NewNumIndex = 0;
                //string qry="";
                //int FY = _LoginCredential.FY;
                //string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");

                //decimal? basic_da = 0;
                //basic_da = Convert.ToDecimal(Values.basic_da);
                //decimal? eligibility_amount = 0;
                //eligibility_amount = Convert.ToDecimal(Values.apply_amount);
                //string pfno = Values.pf_no;
                //int puposeofadv = Convert.ToInt32(Values.purposeType);
                //string pfloanid = Values.pfloansid.ToString();
                decimal? amountapp = Convert.ToDecimal(Values.apply_amount);
                //decimal? basicmonthda = Convert.ToDecimal(Values.basic_da);
                //decimal? rate = Convert.ToDecimal(Values.rate_of_interest);
                decimal? amountapp2 = Convert.ToDecimal(Values.amount_applied_for_2);
                //string calculating_months = Values.calculating_months;
                //decimal? ownshare = Convert.ToDecimal(Values.netownsharenetbankshare);
                decimal? least3 = Convert.ToDecimal(Values.least_of_3);
                //decimal? grosssal = Convert.ToDecimal(Values.gross_salary);
                //decimal? net_salary = Convert.ToDecimal(Values.net_salary);
                decimal? net_minus_pf = Convert.ToDecimal(Values.net_minus_pf);
                //decimal? onethirdgross = Convert.ToDecimal(Values.rdofgrosssalary);
                decimal? total_outstanding_loan = Convert.ToDecimal(Values.apply_amount);
                decimal? amount_recommended_for_sanction = Convert.ToDecimal(Values.amountrecommendedforsanction);
                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());
                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_emp_pf_repayable_loan", NewNumIndex));

                qry = "update pr_emp_pf_repayable_loan set amount_applied_for=" + amountapp + " , [amount_applied_for_2]=" + amountapp2 + ", net_minus_pf =" + net_minus_pf + ", " +
                    "  [least_of_3] =" + least3 + ", total_outstanding_loan=" + total_outstanding_loan + ",amount_recommended_for_sanction=" + amount_recommended_for_sanction + "  where emp_code='" + Values.EntityId + "' and purpose_of_advance='" + Values.purposeType + "' ";

                sbqry.Append(qry);

                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_repayable_loan", lempid.ToString(), ""));

                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#PF Payable#PF Payable Data Updated Successfully..!!";
                }
                else
                {
                    return "E#PF Payable#Error While PF Payable Data Insertion";
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return "";

        }

        public async Task<string> DeletePFPayableLoan(string purposeType, string Emp_code)
        {
            string qry = "";
            string retMessage = "";
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            try
            {
                qry = "delete from pr_emp_pf_repayable_loan where emp_code = " + Emp_code + "  and purpose_of_advance = " + purposeType + " and active = 1 and authorisation = 0; ";

                sbqry.Append(qry);

                //4. transaction touch 
                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_nonrepayable_loan", id.ToString(), ""));

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
        public async Task<string> getown_bank_vpf(string empCode)
        {
            int syear = _LoginCredential.FY;
            int eyear = syear - 1;
            StringBuilder sb = new StringBuilder();
            string amount = "";

            string shareQry = "select sum(ob.own_share) + pfbal.os_open + (select CASE WHEN COUNT(1) > 0 THEN sum(ob.own_share)" +
                " ELSE 0 END from pr_ob_share_encashment ob join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code " +
                "where ob.emp_code =  " + Convert.ToString(empCode) + " and (ob.fm between DATEFROMPARTS((select year(fm) from pr_month_details where active = 1), " +
                "04, 01) and DATEFROMPARTS((select fy from pr_month_details where active= 1), 03, 31 )) and pfbal.fy = (select year(fm) " +
                "from pr_month_details where active = 1) )+(select CASE WHEN COUNT(1) > 0 THEN sum(ob.own_share) ELSE 0 " +
                "END from pr_ob_share_adhoc ob join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code " +
                "where ob.emp_code =  " + Convert.ToString(empCode) + " and (ob.fm between DATEFROMPARTS((select year(fm) from pr_month_details where active = 1), 04, 01) " +
                "and DATEFROMPARTS((select fy from pr_month_details where active= 1), 03, 31 )) and pfbal.fy = (select year(fm) " +
                "from pr_month_details where active = 1) )  as own_share,sum(ob.bank_share) + pfbal.bs_open + " +
                "(select CASE WHEN COUNT(1) > 0 THEN sum(ob.bank_share) ELSE 0 END from pr_ob_share_encashment ob " +
                "join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code where ob.emp_code =  " + Convert.ToString(empCode) + " and " +
                "(ob.fm between DATEFROMPARTS((select year(fm) from pr_month_details where active = 1), 04, 01) " +
                "and DATEFROMPARTS((select fy from pr_month_details where active= 1), 03, 31 )) and pfbal.fy = (select year(fm) " +
                "from pr_month_details where active = 1) )+(select CASE WHEN COUNT(1) > 0 THEN sum(ob.bank_share) ELSE 0 END " +
                "from pr_ob_share_adhoc ob join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code " +
                "where ob.emp_code =  " + Convert.ToString(empCode) + " and (ob.fm between DATEFROMPARTS((select year(fm) " +
                "from pr_month_details where active = 1), 04, 01) and DATEFROMPARTS((select fy from pr_month_details where active= 1), 03, 31 )) " +
                "and pfbal.fy = (select fy-1 from pr_month_details where active=1) ) as bank_share,sum(ob.vpf) + pfbal.vpf_open + " +
                "(select CASE WHEN COUNT(1) > 0 THEN sum(ob.vpf) ELSE 0 END from pr_ob_share_encashment ob " +
                "join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code where ob.emp_code =  " + Convert.ToString(empCode) + " and " +
                "(ob.fm between DATEFROMPARTS((select year(fm) from pr_month_details where active = 1), 04, 01) " +
                "and DATEFROMPARTS((select fy from pr_month_details where active= 1), 03, 31 )) and pfbal.fy = (select year(fm) " +
                "from pr_month_details where active = 1) )+(select CASE WHEN COUNT(1) > 0 THEN " +
                "sum(ob.vpf) ELSE 0 END from pr_ob_share_adhoc ob join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code " +
                "where ob.emp_code =  " + Convert.ToString(empCode) + " and (ob.fm between DATEFROMPARTS((select year(fm) " +
                "from pr_month_details where active = 1), 04, 01) and DATEFROMPARTS((select fy from pr_month_details where active= 1), 03, 31 ))" +
                "and pfbal.fy = (select fy-1 from pr_month_details where active=1) ) as vpf from pr_ob_share ob " +
                "join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code where ob.emp_code =  " + Convert.ToString(empCode) + " and ob.fm between DATEFROMPARTS((select year(fm) from pr_month_details where active = 1), 04, 01) " +
                "and DATEFROMPARTS((select fy from pr_month_details where active= 1), 03, 31 ) and pfbal.fy = (select year(fm) " +
                "from pr_month_details where active = 1) group by os_open,bs_open,pfbal.vpf_open ";

            string instbal = "select os_open_int as own_share_intrst_amount ,bs_open_int as bank_share_intrst_amount,vpf_open_int as vpf_intrst_amount from pr_pf_open_bal_year where emp_code = "+Convert.ToInt32(empCode)+" and fy = (select year(fm) as fm from pr_month_details where active = 1); ";

            string prevQry = "select pf_return as prev_own, vpf_return as prev_vpf, bank_return as prev_bank from pr_pf_open_bal_year " +
                "where emp_code = " + Convert.ToString(empCode) + " and fy = (select fy-1 from pr_month_details where active=1) ";

            
            string presQry = "select sum(own_share) as pres_own, sum(vpf) as pres_vpf,sum(bank_share) as pres_bank " +
                "from pr_emp_pf_nonrepayable_loan where emp_code = " + Convert.ToInt32(empCode) + " and authorisation = 1 and process = 1 " +
              "and fm between '" + eyear + "-04-01' and '" + syear + "-03-31' ";

            string qry3PFHT1 = "select l.emp_code,a.principal_open_amount+a.interest_accured as Loan_Closing from pr_emp_adv_loans_adjustments a join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid  join pr_loan_master lm on lm.id = l.loan_type_mid join pr_emp_adv_loans_child cl on l.id = cl.emp_adv_loans_mid  and cl.id = a.emp_adv_loans_child_mid  " +
                "where l.loan_type_mid in (16, 17, 18, 19, 20, 21, 26, 27) and lm.loan_id ='PFHT1' and a.fm = (select max(fm) from pr_emp_adv_loans_adjustments) " +
                "and a.active = 1  and l.emp_code = " + Convert.ToInt32(empCode) + "  ; ";

            string qry4PFHT2 = "select l.emp_code,a.principal_open_amount+a.interest_accured as Loan_Closing from pr_emp_adv_loans_adjustments a join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid  join pr_loan_master lm on lm.id = l.loan_type_mid join pr_emp_adv_loans_child cl on l.id = cl.emp_adv_loans_mid  and cl.id = a.emp_adv_loans_child_mid  " +
                "where l.loan_type_mid in (16, 17, 18, 19, 20, 21, 26, 27) and lm.loan_id ='PFHT2' and a.fm = (select max(fm) from pr_emp_adv_loans_adjustments) " +
                "and a.active = 1  and l.emp_code = " + Convert.ToInt32(empCode) + "  ; ";

            DataSet ds = await _sha.Get_MultiTables_FromQry(shareQry+ instbal+ prevQry+presQry+ qry3PFHT1 + qry4PFHT2);
            DataTable dtshareQry = ds.Tables[0];
            DataTable dtinstbal = ds.Tables[1];
            DataTable dtprevQry = ds.Tables[2];
            DataTable dtpresQry = ds.Tables[3];
            DataTable dtqry3PFHT1 = ds.Tables[4];
            DataTable dtqry4PFHT2 = ds.Tables[5];

            decimal own_share = 0.00m;
            decimal bank_share = 0.00m;
            int vpf = 0;
            decimal pfht1_2loan = 0.00m;

            decimal A_value_own_share = 0.00m;
            decimal A_value_bank_share = 0.00m;
            int A_value_vpf = 0;
            decimal B_value_own_share = 0.00m;
            decimal B_value_bank_share = 0.00m;
            decimal B_value_vpf = 0.00m;
            decimal C_value_loan= 0.00m;
            decimal C_value_bank_share = 0.00m;
            int C_value_vpf = 0;

            if (dtshareQry.Rows.Count>0)
            {
                own_share += Convert.ToDecimal(dtshareQry.Rows[0]["own_share"]);
                bank_share += Convert.ToDecimal(dtshareQry.Rows[0]["bank_share"]);
                vpf += Convert.ToInt32(dtshareQry.Rows[0]["vpf"]);
            }
            if(dtinstbal.Rows.Count>0)
            {
                own_share += Convert.ToDecimal(dtinstbal.Rows[0]["own_share_intrst_amount"]);
                bank_share += Convert.ToDecimal(dtinstbal.Rows[0]["bank_share_intrst_amount"]);
                vpf += Convert.ToInt32(dtinstbal.Rows[0]["vpf_intrst_amount"]);
            }
            
            A_value_own_share = own_share;
            A_value_bank_share = bank_share;
            A_value_vpf = vpf;
            own_share = 0;
            bank_share = 0;
            vpf = 0;

            if (dtprevQry.Rows.Count > 0)
            {
                own_share += Convert.ToDecimal(dtprevQry.Rows[0]["prev_own"]);
                bank_share += Convert.ToDecimal(dtprevQry.Rows[0]["prev_bank"]);
                vpf += Convert.ToInt32(dtprevQry.Rows[0]["prev_vpf"]);
            }
            if (dtpresQry.Rows.Count > 0)
            {
                own_share += Convert.ToDecimal(dtpresQry.Rows[0]["pres_own"]);
                bank_share += Convert.ToDecimal(dtpresQry.Rows[0]["pres_bank"]);
                vpf += Convert.ToInt32(dtpresQry.Rows[0]["pres_vpf"]);
            }
            B_value_own_share = own_share;
            B_value_bank_share = bank_share;
            B_value_vpf = vpf;
            if(dtqry3PFHT1.Rows.Count>0)
            {
                pfht1_2loan += Convert.ToInt32(dtqry3PFHT1.Rows[0]["Loan_Closing"]);
            }
            if (dtqry4PFHT2.Rows.Count > 0)
            {
                pfht1_2loan += Convert.ToInt32(dtqry4PFHT2.Rows[0]["Loan_Closing"]);
            }
            C_value_loan = pfht1_2loan;
            sb.Append((A_value_bank_share - (B_value_bank_share + C_value_bank_share)).ToString());
            sb.Append(",");
            sb.Append((A_value_own_share - (B_value_own_share + C_value_loan)).ToString());
            sb.Append(",");
            sb.Append((A_value_vpf - (B_value_vpf + C_value_vpf)).ToString());
            amount = sb.ToString();

            return amount;
        }

    }
}
