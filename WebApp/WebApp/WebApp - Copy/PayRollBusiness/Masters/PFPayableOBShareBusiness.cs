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

namespace PayRollBusiness.Masters
{
    public class PFPayableOBShareBusiness : BusinessBase
    {
        public PFPayableOBShareBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();

        public async Task<IList<loansOptions>> getLoansPurpose()
        {
            string qrySel = "SELECT id,purpose_name as name,month as value " +
                            "FROM pr_purpose_of_advance_master " +
                            "WHERE active=1 ";
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



        public async Task<string> getOBShareData(string empCode)
        {
            decimal pf1 = 0.00M;
            decimal pf2 = 0.00M;
            int syear = _LoginCredential.FY;
            int eyear = syear - 1;
            //return null;
            //            string shareQry = "select ob.own_share_total as own_share, ob.own_share_intrst_total as own_share_intrst_amount, ob.vpf_total as vpf, ob.vpf_intrst_total as vpf_intrst_amount, ob.bank_share_total as bank_share," +
            //                " ob.bank_share_intrst_total  as bank_share_intrst_amount" +
            //                " from pr_ob_share ob left outer join pr_emp_pf_nonrepayable_loan pn on ob.emp_code = pn.emp_code and pn.process=1 where ob.emp_code = " + Convert.ToString(empCode) + " and ob.active=1 ;";

            //            string shareQry = "select sum(ob.own_share) + pfbal.os_open as own_share,sum(ob.bank_share) + pfbal.bs_open as bank_share,sum(ob.vpf) + pfbal.vpf_open as vpf " +
            //" from pr_ob_share ob  " +
            //"join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code where ob.emp_code = " + Convert.ToString(empCode) + " and ob.fm between DATEFROMPARTS((select year(fm) " +
            //"from pr_month_details where active = 1), 04, 01) and DATEFROMPARTS((select fy from pr_month_details where active= 1), 03, 31 ) " +
            //"and pfbal.fy = (select year(fm) from pr_month_details where active = 1) group by os_open,bs_open,pfbal.vpf_open ";


            string shareQry = "select sum(ob.own_share) + pfbal.os_open + (select CASE WHEN COUNT(1) > 0 THEN sum(ob.own_share)" +
                " ELSE 0 END from pr_ob_share_encashment ob join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code " +
                "where ob.emp_code =  " + Convert.ToString(empCode) + " and (ob.fm between DATEFROMPARTS("+eyear+", " +
                "04, 01) and DATEFROMPARTS("+syear+", 03, 31 )) and pfbal.fy = (select fy-1 " +
                "from pr_month_details where active = 1) )+(select CASE WHEN COUNT(1) > 0 THEN sum(ob.own_share) ELSE 0 " +
                "END from pr_ob_share_adhoc ob join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code " +
                "where ob.emp_code =  " + Convert.ToString(empCode) + " and (ob.fm between DATEFROMPARTS("+eyear+", 04, 01) " +
                "and DATEFROMPARTS("+syear+", 03, 31 )) and pfbal.fy = (select fy-1 " +
                "from pr_month_details where active = 1) )  as os_open,sum(ob.bank_share) + pfbal.bs_open + " +
                "(select CASE WHEN COUNT(1) > 0 THEN sum(ob.bank_share) ELSE 0 END from pr_ob_share_encashment ob " +
                "join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code where ob.emp_code =  " + Convert.ToString(empCode) + " and " +
                "(ob.fm between DATEFROMPARTS("+eyear+", 04, 01) " +
                "and DATEFROMPARTS("+syear+", 03, 31 )) and pfbal.fy = (select fy-1 " +
                "from pr_month_details where active = 1) )+(select CASE WHEN COUNT(1) > 0 THEN sum(ob.bank_share) ELSE 0 END " +
                "from pr_ob_share_adhoc ob join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code " +
                "where ob.emp_code =  " + Convert.ToString(empCode) + " and (ob.fm between DATEFROMPARTS( " +
                ""+eyear+", 04, 01) and DATEFROMPARTS("+syear+", 03, 31 )) " +
                "and pfbal.fy = (select fy-1 from pr_month_details where active = 1) ) as bs_open,sum(ob.vpf) + pfbal.vpf_open + " +
                "(select CASE WHEN COUNT(1) > 0 THEN sum(ob.vpf) ELSE 0 END from pr_ob_share_encashment ob " +
                "join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code where ob.emp_code =  " + Convert.ToString(empCode) + " and " +
                "(ob.fm between DATEFROMPARTS("+eyear+", 04, 01) " +
                "and DATEFROMPARTS("+syear+", 03, 31 )) and pfbal.fy = (select fy-1 " +
                "from pr_month_details where active = 1) )+(select CASE WHEN COUNT(1) > 0 THEN " +
                "sum(ob.vpf) ELSE 0 END from pr_ob_share_adhoc ob join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code " +
                "where ob.emp_code =  " + Convert.ToString(empCode) + " and (ob.fm between DATEFROMPARTS( " +
                ""+eyear+", 04, 01) and DATEFROMPARTS("+syear+", 03, 31 ))" +
                "and pfbal.fy = (select fy-1 from pr_month_details where active = 1) ) as vpf_open from pr_ob_share ob " +
                "join pr_pf_open_bal_year pfbal on pfbal.emp_code = ob.emp_code where ob.emp_code =  " + Convert.ToString(empCode) + " and ob.fm between DATEFROMPARTS("+eyear+", 04, 01) " +
                "and DATEFROMPARTS("+syear+ ", 03, 31 ) and pfbal.fy = (select fy-1 " +
                "from pr_month_details where active = 1) group by os_open,bs_open,pfbal.vpf_open ";
            
            //string shareQry = "select sum(ob.own_share)+pyer.os_open as os_open,sum(ob.bank_share) + pyer.bs_open as bs_open,sum(ob.vpf) + pyer.vpf_open as vpf_open " +
            //    "from pr_ob_share ob join pr_pf_open_bal_year pyer on pyer.emp_code = ob.emp_code " +
            //    "where ob.emp_code =  " + Convert.ToString(empCode) + " and (ob.fm between DATEFROMPARTS (" + eyear + ", 04, 01) and DATEFROMPARTS (" + syear + ", 03, 31 ))  and pyer.fy = " + eyear + "  group by os_open,bs_open,pyer.vpf_open "; 
            string shareNonQry = "select own_share, bank_share, vpf from pr_emp_pf_nonrepayable_loan " +
                "where emp_code =" + Convert.ToString(empCode) + " ;";

            string instbal = "select os_open_int as own_share_intrst_amount ,bs_open_int as bank_share_intrst_amount,vpf_open_int as vpf_intrst_amount from pr_pf_open_bal_year " +
                "where emp_code=" + Convert.ToString(empCode) + " and fy=(select fy-1 from pr_month_details where active=1)";

            string prevQry = "select pf_return as prev_own, vpf_return as prev_vpf, bank_return as prev_bank from pr_pf_open_bal_year " +
                "where emp_code = " + Convert.ToString(empCode) + " and fy = (select fy-1 from pr_month_details where active=1) ";

            //string presQry = "select sum(own_share) as pres_own, sum(vpf) as pres_vpf,sum(bank_share) " +
            //    "as pres_bank from pr_emp_pf_nonrepayable_loan where  year(fm) = year(getdate()) and emp_code =" + Convert.ToString(empCode) + " ;";

            //string presQry = "select top(1) own_share as pres_own, vpf as pres_vpf,bank_share as pres_bank " +
            //    "from pr_emp_pf_nonrepayable_loan where year(fm) = year(getdate()) and emp_code = " + Convert.ToString(empCode) + "  order by process_date desc; ";

            //        string presQry = "select top(1) own_share as pres_own, vpf as pres_vpf,bank_share as pres_bank " +
            //"from pr_emp_pf_nonrepayable_loan where emp_code = " + Convert.ToString(empCode) + "  order by process_date desc; ";
           
            string presQry = "select ISNULL(sum(own_share),0) as pres_own, ISNULL(sum(vpf),0) as pres_vpf,ISNULL(sum(bank_share),0) as pres_bank " +
                "from pr_emp_pf_nonrepayable_loan where emp_code = " + Convert.ToInt32(empCode) + " and authorisation = 1 and process = 1 " +
              "and fm between '" + eyear + "-04-01' and '" + syear + "-03-31' ";
            //string qrys3 = "select total_amount from pr_emp_adv_loans join   pr_loan_master on pr_loan_master.id=pr_emp_adv_loans.loan_type_mid where  pr_loan_master.loan_id='PFL1' and emp_code =" + Convert.ToInt32(empCode) + " ;";
            //DataTable dts4 = await _sha.Get_Table_FromQry(qrys3);
            //if (dts4.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dts4.Rows)
            //    {
            //       pf1 = Convert.ToDecimal(dr["total_amount"]);

            //    }
            //}


            //string qrys4 = "select total_amount from pr_emp_adv_loans join   pr_loan_master on pr_loan_master.id=pr_emp_adv_loans.loan_type_mid where  pr_loan_master.loan_id='PFL2' and emp_code =" + Convert.ToInt32(empCode) + " ;";
            //DataTable dts5 = await _sha.Get_Table_FromQry(qrys4);
            //if (dts5.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dts5.Rows)
            //    {
            //        pf2 = Convert.ToDecimal(dr["total_amount"]);
            //    }
            //}
            DataSet ds = await _sha.Get_MultiTables_FromQry(shareQry + shareNonQry + prevQry + presQry + instbal);
            DataTable share = ds.Tables[0];
            DataTable loanShare = ds.Tables[1];
            DataTable prevshare = ds.Tables[2];
            DataTable presshre = ds.Tables[3];

            DataTable presint = ds.Tables[4];

            IList<ObShare> lstShare = new List<ObShare>();
            IList<ObShareIntrst> lstInstShare = new List<ObShareIntrst>();
            IList<ObSharePrev> lstPrevShare = new List<ObSharePrev>();
            IList<ObSharePres> lstPresShare = new List<ObSharePres>();

            if (share.Rows.Count > 0)
            {
                foreach (DataRow gen in share.Rows)
                {
                    lstShare.Add(new ObShare
                    {
                        fund_own = gen["os_open"].ToString(),
                        fund_bank = gen["bs_open"].ToString(),
                        fund_vpf = gen["vpf_open"].ToString(),
                    });

                    //lstInstShare.Add(new ObShareIntrst
                    //{
                    //    ist_own = gen["own_share_intrst_amount"].ToString(),
                    //    ist_bank = gen["bank_share_intrst_amount"].ToString(),
                    //    ist_vpf = gen["vpf_intrst_amount"].ToString()
                    //});


                }
            }
            if (presint.Rows.Count > 0)
            {
                foreach (DataRow gen1 in presint.Rows)
                {
                    lstInstShare.Add(new ObShareIntrst
                    {
                        ist_own = gen1["own_share_intrst_amount"].ToString(),
                        ist_bank = gen1["bank_share_intrst_amount"].ToString(),
                        ist_vpf = gen1["vpf_intrst_amount"].ToString()
                    });
                }
            }
            else
            {
                lstInstShare.Add(new ObShareIntrst
                {
                    ist_own = "0.00",
                    ist_bank = "0.00",
                    ist_vpf = "0.00"
                });
            }

            if (prevshare.Rows.Count > 0)
            {
                foreach (DataRow gen in prevshare.Rows)
                {
                    lstPrevShare.Add(new ObSharePrev
                    {
                        prev_own = gen["prev_own"].ToString(),
                        prev_vpf = gen["prev_vpf"].ToString(),
                        prev_bank = gen["prev_bank"].ToString()
                    });
                }
            }
            else
            {
                lstPrevShare.Add(new ObSharePrev
                {
                    prev_own = "0.00",
                    prev_vpf = "0.00",
                    prev_bank = "0.00"
                });
            }
            if (presshre.Rows.Count > 0)
            {
                foreach (DataRow gen in presshre.Rows)
                {
                    lstPresShare.Add(new ObSharePres
                    {
                        pres_own = gen["pres_own"].ToString(),
                        pres_vpf = gen["pres_vpf"].ToString(),
                        pres_bank = gen["pres_bank"].ToString()
                    });
                }
            }
            else
            {
                lstPresShare.Add(new ObSharePres
                {
                    pres_own = "0.00",
                    pres_vpf = "0.00",
                    pres_bank = "0.00"
                });
            }
            string qry3 = "select l.emp_code,a.principal_open_amount+a.interest_accured as Loan_Closing from pr_emp_adv_loans_adjustments a join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid  join pr_loan_master lm on lm.id = l.loan_type_mid join pr_emp_adv_loans_child cl on l.id = cl.emp_adv_loans_mid  and cl.id = a.emp_adv_loans_child_mid  " +
                "where l.loan_type_mid in (16, 17, 18, 19, 20, 21, 26, 27) and lm.loan_id ='PFHT1' and a.fm = (select max(fm) from pr_emp_adv_loans_adjustments) " +
                "and a.active = 1  and l.emp_code = " + Convert.ToInt32(empCode) + "  ; ";


            //string qry3 = "select total_amount from pr_emp_adv_loans join   pr_loan_master on pr_loan_master.id=pr_emp_adv_loans.loan_type_mid where  pr_loan_master.loan_id='PFL1' and emp_code =" + Convert.ToInt32(empCode) + " ;";
            DataTable dt4 = await _sha.Get_Table_FromQry(qry3);
            if (dt4.Rows.Count > 0)
            {
                foreach (DataRow dr in dt4.Rows)
                {
                    pf1 = Convert.ToDecimal(dr["Loan_Closing"]);
                    if (pf1 < 0)
                    {
                        pf1 = 0.00M;
                    }
                }


            }
            else
            {
                pf1 = 0.00M;
            }

            string qry4 = "select l.emp_code,a.principal_open_amount+a.interest_accured as Loan_Closing from pr_emp_adv_loans_adjustments a join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid  join pr_loan_master lm on lm.id = l.loan_type_mid join pr_emp_adv_loans_child cl on l.id = cl.emp_adv_loans_mid  and cl.id = a.emp_adv_loans_child_mid  " +
                "where l.loan_type_mid in (16, 17, 18, 19, 20, 21, 26, 27) and lm.loan_id ='PFHT2' and a.fm = (select max(fm) from pr_emp_adv_loans_adjustments) " +
                "and a.active = 1  and l.emp_code = " + Convert.ToInt32(empCode) + "  ; ";

            //string qry4 = "select total_amount from pr_emp_adv_loans join   pr_loan_master on pr_loan_master.id=pr_emp_adv_loans.loan_type_mid where  pr_loan_master.loan_id='PFL2' and emp_code =" + Convert.ToInt32(empCode) + " ;";
            DataTable dt5 = await _sha.Get_Table_FromQry(qry4);
            if (dt5.Rows.Count > 0)
            {
                foreach (DataRow dr in dt5.Rows)
                {
                    pf2 = Convert.ToDecimal(dr["Loan_Closing"]);
                }
            }
            else
            {
                pf2 = 0.00M;
            }

            var sharePoints = JsonConvert.SerializeObject(lstShare);
            var instPoints = JsonConvert.SerializeObject(lstInstShare);
            var prevPoints = JsonConvert.SerializeObject(lstPrevShare);
            var presPoints = JsonConvert.SerializeObject(lstPresShare);
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var shareData = javaScriptSerializer.DeserializeObject(sharePoints);
            var instData = javaScriptSerializer.DeserializeObject(instPoints);
            var prevData = javaScriptSerializer.DeserializeObject(prevPoints);
            var presData = javaScriptSerializer.DeserializeObject(presPoints);
            var resultJson = javaScriptSerializer.Serialize(new
            {
                pf2 = pf2,
                pf1 = pf1,
                share = shareData,
                inst = instData,
                prev = prevData,
                pres = presData

            });
            return resultJson;

        }

    }
}
