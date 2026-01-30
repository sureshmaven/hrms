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
    public class PFPayableBusinessProcess : BusinessBase
    {
        public PFPayableBusinessProcess(LoginCredential loginCredential) : base(loginCredential)
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
        public async Task<string> getPayableEmpDetails(string empCode)
        {
               IList<PfrepayableDetails> lstDept = new List<PfrepayableDetails>();
            try
            {

                string qry = "select pf.pf_account_no as pf_no,pf.purpose_of_advance as padvid,adv.purpose_name as purpose,pr_emp_adv_loans.total_amount,pf.pf_loans_id as loanid,pf.amount_applied_for as amountapp," +
                    " pf.[(basic+DA)*months_1] as basicData,pf.rate_of_interest as rate,pf.amount_applied_for_2 as amountapplied2,pf.calculating_months as calmon,[netownshare+25%netbankshare_3] as netshare,[least_of_3] as leastt,pf.[gross_salary] as grossamt,pf.net_salary as netamt,pf.net_minus_pf as netminuspf,pf.[1/3rd_of_gross_salary] as onethirdg,pf.total_outstanding_loan as total,pf.amount_recommended_for_sanction as sanction,pf.active from pr_emp_pf_repayable_loan pf left outer join pr_purpose_of_advance_master adv on pf.purpose_of_advance = adv.id left outer join pr_loan_master on pr_loan_master.loan_id=pf.pf_loans_id left outer  join pr_emp_adv_loans on pr_emp_adv_loans.loan_type_mid=pr_loan_master.id and pr_emp_adv_loans.emp_code=" + Convert.ToInt32(empCode) + " where pf.active=1 and pf.authorisation=1 and pf.emp_code=" + Convert.ToInt32(empCode);

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
                            pf1 = gen["total_amount"].ToString(),
                            own_share =Convert.ToDecimal(gen["netshare"]),
                            firstinstall= gen["netminuspf"].ToString(),
                            calm = gen["calmon"].ToString(),
                            least= gen["leastt"].ToString(),
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
                    //    string qry1 = "select date_of_join from pr_emp_general where emp_code = " + Convert.ToInt32(empCode) +
                    //    " and active = 1;";
                    //    string qry2 = "select amount as basic from pr_emp_pay_field where emp_code =" + Convert.ToInt32(empCode) + " and " +
                    //        "active = 1 and m_id in (select id from pr_earn_field_master where type = 'pay_fields' and name = 'Basic');";
                    //    string qry3 = "select amount as own_share from pr_emp_deductions where emp_code =" + Convert.ToInt32(empCode) + " and active = 1 " +
                    //        "and m_id in (select id from pr_deduction_field_master where type = 'EPD' and name = 'PF Contribution');";

                    //    string qry4 = "SELECT TOP 1 da_percent FROM pr_month_details where active = 1 ORDER BY ID DESC ;";

                    //    string qry5 = " select amount as vpf from pr_emp_deductions where emp_code =" + Convert.ToInt32(empCode) + " and active = 1 and " +
                    //        "m_id in (select id from pr_deduction_field_master where type = 'EPD' and name = 'VPF');";

                    //    string qry6 = " select pf_no from pr_emp_general where emp_code=" + Convert.ToInt32(empCode) + " and active=1 ;";
                    //    string qry7 = " select gross_amount as grossamt from pr_emp_payslip where emp_code =" + Convert.ToInt32(empCode) + " and " +
                    //       "active = 1 ;";
                    //    string qry8 = "select net_amount as netamt from pr_emp_payslip where emp_code =" + Convert.ToInt32(empCode) + " and " +
                    //      "active = 1 ;";
                    //    string qry9 = "select amount_applied_for as pfloan2,emp_code from  pr_emp_pf_repayable_loan where active=1 and Authorisation=1 and process=0  and pf_loans_id=1 and emp_code =" + Convert.ToInt32(empCode) + " ;";
                    //    string qry10 = "select amount_applied_for as pfloan2,emp_code from  pr_emp_pf_repayable_loan where active=1 and Authorisation=1 and process=0 and pf_loans_id=2 and emp_code =" + Convert.ToInt32(empCode) + " ;";
                    //    string qry11 = "select amount as bank_share from pr_emp_adhoc_contribution_field where emp_id =" + Convert.ToInt32(empCode) + " and active = 1 and m_id in " +
                    //      "(select id from pr_contribution_field_master where type = 'adhoc' and name = 'PF Bank Share');";

                    //    DataSet ds = await _sha.Get_MultiTables_FromQry(qry1 + qry2 + qry3 + qry4 + qry5 + qry6 + qry7 + qry8 + qry9 + qry10 + qry11);
                    //    //DataTable dt = new DataTable();

                    //    DataTable general = ds.Tables[0];
                    //    DataTable basic = ds.Tables[1];
                    //    DataTable own = ds.Tables[2];

                    //    DataTable da = ds.Tables[3];
                    //    DataTable vpf = ds.Tables[4];
                    //    DataTable pf = ds.Tables[5];
                    //    DataTable gross = ds.Tables[6];
                    //    DataTable net = ds.Tables[7];
                    //    DataTable pfloan1 = ds.Tables[8];
                    //    DataTable pfloan2 = ds.Tables[9];
                    //    DataTable bank = ds.Tables[10];
                    //    DataTable table = new DataTable();
                    //    table.Columns.Add("date_of_join", typeof(string));
                    //    table.Columns.Add("basic", typeof(string));
                    //    table.Columns.Add("own_share", typeof(string));
                    //    table.Columns.Add("bank_share", typeof(string));
                    //    table.Columns.Add("da_percent", typeof(string));
                    //    table.Columns.Add("vpf", typeof(string));
                    //    table.Columns.Add("pf_no", typeof(string));
                    //    table.Columns.Add("grossamt", typeof(string));
                    //    table.Columns.Add("netamt", typeof(string));
                    //    table.Columns.Add("pf1", typeof(string));
                    //    table.Columns.Add("pf2", typeof(string));

                    //    string basicData = "";
                    //    string pf_no = "";
                    //    string date_of_join = "";
                    //    string own_share = "";

                    //    string da_percent = "";
                    //    string vpfData = "";
                    //    string grossamt = "";
                    //    string netamt = "";
                    //    string bank_share = "";
                    //    string pf1 = "";
                    //    string pf2 = "";
                    //    if (pfloan1.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow gen in pfloan1.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            pf1 = gen["pfloan1"].ToString();
                    //            row["pfloan1"] = pf1;
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (DataRow gen in pfloan1.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            pf1 = "";
                    //            row["pfloan1"] = "";
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    if (bank.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow bnk in bank.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            bank_share = bnk["bank_share"].ToString();
                    //            row["bank_share"] = bnk["bank_share"];
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (DataRow bnk in bank.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            bank_share = "";
                    //            row["bank_share"] = "";
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    if (pfloan2.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow gen in pfloan2.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            pf2 = gen["pfloan2"].ToString();
                    //            row["pfloan2"] = pf2;
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (DataRow gen in pfloan1.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            pf2 = "";
                    //            row["pfloan2"] = "";
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    if (gross.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow gen in gross.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            grossamt = gen["grossamt"].ToString();
                    //            row["grossamt"] = grossamt;
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (DataRow gen in gross.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            grossamt = "";
                    //            row["grossamt"] = "";
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    if (net.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow gen in net.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            netamt = gen["netamt"].ToString();
                    //            row["netamt"] = netamt;
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (DataRow gen in net.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            netamt = "";
                    //            row["netamt"] = "";
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    //if(general.Rows.Count)
                    //    if (general.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow gen in general.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            date_of_join = gen["date_of_join"].ToString();
                    //            row["date_of_join"] = date_of_join;
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (DataRow gen in general.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            date_of_join = "";
                    //            row["date_of_join"] = "";
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    if (basic.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow bas in basic.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            basicData = bas["basic"].ToString();
                    //            row["basic"] = bas["basic"];

                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        DataRow row = table.NewRow();
                    //        basicData = "";
                    //        row["basic"] = "";

                    //        table.Rows.Add(row);
                    //    }
                    //    if (own.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow ow in own.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            own_share = ow["own_share"].ToString();
                    //            row["own_share"] = ow["own_share"];
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (DataRow ow in own.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            own_share = "";
                    //            row["own_share"] = "";
                    //            table.Rows.Add(row);
                    //        }
                    //    }

                    //    if (da.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow d in da.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            da_percent = d["da_percent"].ToString();
                    //            row["da_percent"] = d["da_percent"];
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (DataRow d in da.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            da_percent = "";
                    //            row["da_percent"] = "";
                    //            table.Rows.Add(row);
                    //        }
                    //    }

                    //    if (vpf.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow vp in vpf.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            vpfData = vp["vpf"].ToString();
                    //            row["vpf"] = vp["vpf"];
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (DataRow vp in vpf.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            vpfData = "";
                    //            row["vpf"] = "";
                    //            table.Rows.Add(row);
                    //        }
                    //    }

                    //    if (pf.Rows.Count > 0)
                    //    {
                    //        foreach (DataRow pfn in pf.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            pf_no = pfn["pf_no"].ToString();
                    //            row["pf_no"] = pfn["pf_no"];
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (DataRow pfn in pf.Rows)
                    //        {
                    //            DataRow row = table.NewRow();
                    //            pf_no = "";
                    //            row["pf_no"] = pfn["pf_no"];
                    //            table.Rows.Add(row);
                    //        }
                    //    }
                    //    //DateTime = 
                    //    //var months_no = (year(getdate()) - year(dob));
                    //    DateTime now = DateTime.Now;
                    //    //var months_no = date_of_join.Subtract(now.d / (365.25 / 12);

                    //    int mno = Math.Abs(((Convert.ToDateTime(date_of_join).Year - now.Year) * 12) + Convert.ToDateTime(date_of_join).Month - now.Month);
                    //    //int monthsApart = 12 * (Convert.ToDateTime(date_of_join).Year - now.Year) + Convert.ToDateTime(date_of_join).Year - now.Month;

                    //    int own_shares = mno * Convert.ToInt32(own_share);
                    //    string own_ins = Convert.ToInt32((own_shares * 8.5) / (100 + own_shares)).ToString();
                    //    int bank_shares = mno * Convert.ToInt32(bank_share);
                    //    int bank_ins = Convert.ToInt32(bank_shares * 8.5 / 100 + bank_shares);
                    //    int vpfs = mno * Convert.ToInt32(vpfData);
                    //    int vpf_ins = Convert.ToInt32(vpfs * 8.5 / 100 + vpfs);
                    //    string gamt = Convert.ToString(grossamt);
                    //    string namount = Convert.ToString(netamt);
                    //    //  string pff1 = pf1;


                    //    lstDept.Add(new PfrepayableDetails
                    //    {
                    //        basic = basicData,
                    //        pf_no = pf_no,
                    //        date_of_join = date_of_join,
                    //        own_share = Convert.ToInt32(own_ins),
                    //        bank_share = Convert.ToInt32(bank_ins),
                    //        da_percent = da_percent,
                    //        vpf = vpf_ins,
                    //        gross = gamt,
                    //        net = namount,
                    //        pf1 = pf1,
                    //        pf2 = pf2,
                    //    });
                    //    table.AcceptChanges();
                    return "E#PF Payable# No Data Found to Process";
                }
                   
                }
            catch (Exception e)
            {
                return "E#PF Payable#"+e.Message;
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
            try
            {
                int lempid = _LoginCredential.EmpCode;
                int emp_code = Values.EntityId;
                int NewNumIndex = 0;
                string qry;
                //string qry="";
                int FY = _LoginCredential.FY;
                string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");

                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());
                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_emp_pf_repayable_loan", NewNumIndex));

                qry = " update pr_emp_pf_repayable_loan set authorisation =1,active=0,process=1,sanction_date=Convert(date,'" + Values.sactiondate + "',105),process_date=convert(date,'" + Values.processdate + "', 105)  where purpose_of_advance=" + Values.purposeType+" " +
                " and emp_code=" + Convert.ToInt32(emp_code) + " and active=1 and authorisation=1;";

                sbqry.Append(qry);

                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pf_repayable_loan", "@idnew" + NewNumIndex, lempid.ToString()));


                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#PF Payable#PF Payable Data Processed Successfully..!!";
                }
                else
                {
                    return "E#PF Payable#Error While PF Payable Data Process";
                }
            }
            catch(Exception ex)
            {
                ex.ToString();
            }
            return "";


        }

    }
}
