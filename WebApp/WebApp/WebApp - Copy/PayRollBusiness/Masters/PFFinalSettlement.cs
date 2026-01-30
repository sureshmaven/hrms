using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Masters
{


    public class PFFinalSettlement : BusinessBase
    {
        public PFFinalSettlement(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();


        public async Task<string> getPFDetailsFinal(string empCode)
        {
            string checkQry = "select id from pr_emp_payslip where active=1 and emp_code = " + Convert.ToInt32(empCode) + " ;";

            DataTable checkTable = await _sha.Get_Table_FromQry(checkQry);
            if (checkTable.Rows.Count <= 0)
            {
                return "E#TDS Process#Process Payslip For Employee ID " + empCode;
            }
            else
            {



                string name = "";
                string per_address = "";
                string pan_no = "";
                string emp_code = "";
                string designation = "";
                string sex = "";
                string fm = "";
                string employer_name_address = PrConstants.Name;
                string employer_pan = PrConstants.PAN;
                string employer_tan = PrConstants.TAN;



                float standard_deduction = 0;
                float ent_allowance = 0;
                float emp_tax = 0;
                decimal? other = 0;
                int? count = 0;
                int? houseloan_allownace = 0;
                decimal? loan_instl_amt = 0;
                decimal? aggregate_of_4 = 0;
                decimal? Income_chargable = 0;
                decimal? total_gross_income = 0;
                decimal? deductible = 0;
                decimal? total_income = 0;
                float income = 0;
                float tds_per_month = 0;
                float edu_cess = 0;
                float tax_payable = 0;
                float tax_till = 0;
                float tax_balance = 0;
                //decimal? house_rent_allowance = 0;
                //decimal? total_section = 0;
                decimal? min_allowance = PrConstants.SECTION_C;
                decimal? max_allowance = 0;
                decimal? min_allowance_other = PrConstants.OTHER_SECTION;
                decimal? max_allowance_other = 0;
                decimal? round_income = 0;
                decimal? section_87a = 0;
                //decimal? da = 0;

                decimal? paid_by_emp = 0;


                int FY = _LoginCredential.FY;
                //string fmonth = _LoginCredential.f
                string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
                string fin_month = _LoginCredential.FinancialMonthDate.ToString("dd-MM-yyyy");
                //string Env_Fm_Fy = commBus.Fyp_Fy();
                DateTime str = Convert.ToDateTime(FM);
                string checkMonth = str.ToString("MM");
                string checkYear = str.ToString("yyyy");
                int fin_year = FY - 1;
                string startYear = fin_year + "-04-01";
                string endYear = FY + "-03-31";
                int months = Math.Abs(((Convert.ToDateTime(FM).Year - Convert.ToDateTime(endYear).Year) * 12) + Convert.ToDateTime(FM).Month - Convert.ToDateTime(endYear).Month) + 1;
                //general
                string general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                            "per_address, gen.pan_no,gen.emp_code, gen.designation,gen.sex " +
                            "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                            "where gen.emp_code = " + Convert.ToInt32(empCode) + ";";

                decimal? basic = 0;
                decimal da_percent = 0;

                decimal? per_allowance = 0;
                decimal? fpa_allow = 0;
                decimal? fpiip = 0;

                decimal? int_ref = 0;
                decimal? t_inc = 0;

                decimal? pf_perk = 0;
                decimal? loan_perk = 0;
                decimal? incentives = 0;
                decimal? spl_allowance = 0;
                decimal? spl_da = 0;
                decimal hra = 0;
                decimal cca = 0;
                decimal? values_of_17_2 = 0;
                decimal? values_of_17_3 = 0;

                //present month
                decimal? pbasic = 0;
                decimal? pda_percent = 0;
                decimal? pcca = 0;
                decimal? phra = 0;
                decimal? pint_ref = 0;
                decimal? pt_inc = 0;
                decimal? pspl_da = 0;
                decimal? pspl_allowance = 0;
                decimal? pper_allowance = 0;
                decimal? pfpa_allow = 0;
                decimal? pfpiip = 0;
                decimal? gross = 0;

                decimal? house_rent_allowance = 0;
                decimal? total_section = 0;
                decimal? balance = 0;

                string qryConstants = "SELECT constant, [value] FROM all_constants WHERE app_type='payroll' AND functionality='GenPayslip' AND active=1;";
                string qryIncometaxConstants = "SELECT constant, [value] FROM all_constants WHERE app_type='payroll' AND functionality='IncomeTax' AND active=1;";
                string qryFy_FM = "SELECT fy,format(fm,'yyyy-MM-dd') as fm FROM pr_month_details WHERE active=1;";


                DataSet ds1 = await _sha.Get_MultiTables_FromQry(qryConstants + qryFy_FM + qryIncometaxConstants);


               var _dtConstants = ds1.Tables[0];
                DataTable _dtFY_FM = ds1.Tables[1];
              var  _incometaxdbconstant = ds1.Tables[2];


                //basic, da_percent,cca,hra,int_ref,t_inc,spl_da,spl_allowance
                string basicQry = "select sum(er_basic) as basic,sum(er_da) as da_percent,sum(er_cca) as cca,sum(er_hra) as hra," +
                    "sum(er_interim_relief) as int_ref, sum(er_telangana_inc) as t_inc,sum(spl_da) as spl_da, sum(spl_allw) as spl_allowance " +
                    "from pr_emp_payslip where emp_code = " + Convert.ToInt32(empCode) + " and active = 1;";
                //sum of previous for basic, da_percent,cca,hra,int_ref,t_inc,spl_da,spl_allowance
                string basicSumQry = "select sum(er_basic) as basic,sum(er_da) as da_percent,sum(er_cca) as cca,sum(er_hra) as hra," +
                    "sum(er_interim_relief) as int_ref, sum(er_telangana_inc) as t_inc,sum(spl_da) as spl_da, sum(spl_allw) as spl_allowance, sum(dd_provident_fund) pf  " +
                    "from pr_emp_payslip where emp_code = " + Convert.ToInt32(empCode) + " and fm<'" + FM + "' and fm>='" + startYear + "'; ";

                //allowance ==> Fixed Personal Allowance,FPA-HRA Allowance,FPIIP
                string allowaceQry = "select all_name, all_amount from pr_emp_payslip_allowance where active=1 and emp_code = " + Convert.ToInt32(empCode) + ";";

                //sum Personal Allowance
                string sumFixedQry = "select sum(all_amount) as all_amount from pr_emp_payslip_allowance a join pr_emp_payslip p on p.id=a.payslip_mid  " +
                    "where all_name='Fixed Personal Allowance' and a.emp_code = " + Convert.ToInt32(empCode) + " and p.fm<'" + FM + "' and p.fm>='" + startYear + "'; ";


                string sumFPAQry = "select sum(all_amount) as all_amount from pr_emp_payslip_allowance a join pr_emp_payslip p on p.id=a.payslip_mid  " +
                    "where all_name='FPA-HRA Allowance' and a.emp_code = " + Convert.ToInt32(empCode) + " and p.fm<'" + FM + "' and p.fm>='" + startYear + "'; ";

                // sum FPIIP
                //string sumFPIIPQry = "select sum(all_amount) from pr_emp_payslip_allowance where all_name = 'FPIIP' " +
                //    "and emp_code = " + Convert.ToInt32(empCode) + " and fm between '" + FM + "' and '" + startYear + "'; ";

                string sumFPIIPQry = "select sum(all_amount) as all_amount from pr_emp_payslip_allowance a join pr_emp_payslip p on p.id=a.payslip_mid  " +
                    "where all_name='FPIIP' and a.emp_code = " + Convert.ToInt32(empCode) + " and p.fm<'" + FM + "' and p.fm>='" + startYear + "'; ";
                //sum PFPerks                                                   
                string PFPerks = "select distinct sum(amount) as pf_perk from " +
                            "pr_emp_perearning where emp_code = " + Convert.ToInt32(empCode) + " and fm<='" + FM + "' and fm>='" + startYear + "' and m_id in " +
                            "(select id from pr_earn_field_master where type = 'per_earn' and name = 'PFPerks'); ";
                // sum LOANPerks                                                    
                string LOANPerks = "select distinct sum(amount) as loan_perk from " +
                            "pr_emp_perearning where emp_code = " + Convert.ToInt32(empCode) + " and fm<='" + FM + "' and fm>='" + startYear + "' and m_id in " +
                            "(select id from pr_earn_field_master where type = 'per_earn' and name = 'LOANPerks'); ";
                // sum Incentive                                                    
                string incentive = "select distinct sum(amount) as incentive from " +
                            "pr_emp_perearning where emp_code = " + Convert.ToInt32(empCode) + " and fm<='" + FM + "' and fm>='" + startYear + "' and m_id in " +
                            "(select id from pr_earn_field_master where type = 'per_earn' and name = 'Incentive'); ";
                // sum  val 17 -2
                string valQry = "select sum(perq_amt+rec_amt+tax_amt) as val172 from pr_emp_incometax_12ba " +
                    "where emp_code = " + Convert.ToInt32(empCode) + " and fm<='" + FM + "' and fm>='" + startYear + "' and " +
                    "m_id in (select id from pr_emp_incometax_12ba_master where nature_of_perq = 'Total Value of Perquisites');";

                // sum val 17 -3
                string val2Qry = "select sum(perq_amt+rec_amt+tax_amt) as val173 " +
                    "from pr_emp_incometax_12ba where emp_code = " + Convert.ToInt32(empCode) + " and active = 1 " +
                    " and fm<='" + FM + "' and fm>='" + startYear + "' and m_id in (select id from pr_emp_incometax_12ba_master " +
                    "where nature_of_perq = 'Total Value of Profits in lieu of Salary as per 17(3)');";

                //rent allowance
                //string rent = "select top 1 amount as amount from pr_emp_rent_details where emp_code = " + Convert.ToInt32(empCode) + " and active=1;";

                string rent = " select sum(amount) as rentamount from pr_emp_rent_details where emp_code = " + Convert.ToInt32(empCode) + " and fy=" + FY + "and active=1 and rent_mid!=1;";

                //other
                string otherQry = "select distinct amount as other from " +
                            "pr_emp_perearning where emp_code = " + Convert.ToInt32(empCode) + " and active = 1 and m_id in " +
                            "(select id from pr_earn_field_master where type = 'per_earn' and name = 'Other'); ";

                //house loan
                string houseQry = "select loan_type_mid,installment_amount  from pr_emp_adv_loans where emp_code = " + Convert.ToInt32(empCode) + " and active = 1;";

                //section 80c
                //string cdQry = "select ef.id as id, ef.name as name, epf.amount as amount from pr_deduction_field_master " +
                //    "ef left outer join  pr_emp_perdeductions epf on ef.id = epf.m_id and epf.active = 1 " +
                //    "and epf.emp_code = " + Convert.ToInt32(empCode) + " WHERE ef.type = 'per_ded' and epf.section = 'Section80C'";

                string cdQry = "select ef.id as id,ef.name as name, epf.amount as amount from pr_deduction_field_master ef left outer join  " +
                   "pr_emp_perdeductions epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + Convert.ToInt32(empCode) + " " +
                   "WHERE ef.type = 'per_ded' and epf.section = 'Section80C' union all select '1' as id,'Provident Fund' AS name, sum(dd_provident_fund) as amount  " +
                   "from pr_emp_payslip where active = 1 and emp_code = " + Convert.ToInt32(empCode) + " union " +
                   "all select '2' as id,'VPF' as name, sum(paydedu.dd_amount) as amount from pr_emp_payslip_deductions " +
                   "paydedu where paydedu.dd_name = 'VPF Deduction' and active=1 and emp_code = " + Convert.ToInt32(empCode) + "";

                //section 80ccc
                string cccQry = "select ef.id as id, ef.name as name, epf.amount as amount from pr_deduction_field_master " +
                    "ef left outer join  pr_emp_perdeductions epf on ef.id = epf.m_id and epf.active = 1 " +
                    "and epf.emp_code = " + Convert.ToInt32(empCode) + " WHERE ef.type = 'per_ded' and epf.section = 'Section80CCC'";
                //section 80ccd
                string ccdQry = "select ef.id as id, ef.name as name, epf.amount as amount from pr_deduction_field_master " +
                    "ef left outer join  pr_emp_perdeductions epf on ef.id = epf.m_id and epf.active = 1 " +
                    "and epf.emp_code = " + Convert.ToInt32(empCode) + " WHERE ef.type = 'per_ded' and epf.section = 'Section80CCD'";


                //section other section
                string otherSectionQry = "select ef.id as id, ef.name as name, epf.amount as amount,epf.section as section from pr_deduction_field_master " +
                    "ef left outer join  pr_emp_perdeductions epf on ef.id = epf.m_id and epf.active = 1 " +
                    "and epf.emp_code = " + Convert.ToInt32(empCode) + " WHERE ef.type = 'per_ded' and epf.section !='Section80C' and epf.section !='Section80CCC' and epf.section !='Section80CCD' and epf.section like '%Section%';";
                //tax till
                string taxQry = "select tax_deducted_at_source from pr_emp_tds_process where empcode = " + Convert.ToInt32(empCode) + " and active=1;";
                //other tds deductions
                //spl_type in ('Regular', 'Encashment', 'Adhoc', 'stopsalary') and
                string tdsQry = "select sum(tds_amount) as tds_amount from pr_emp_other_tds_deductions where " +
                    "emp_code = " + Convert.ToInt32(empCode) + " and fm<='" + FM + "' and fm>='" + startYear + "';";

                string empTaxQry = "select (DATEDIFF(month, '" + startYear + "', '" + FM + "')+1) - (select count(emp_code) as records from pr_emp_payslip " +
                    "where emp_code = " + Convert.ToInt32(empCode) + " and fm<='" + FM + "' and fm>='" + startYear + "') as loss;";

                //string vpfQry = "select sum(dd_amount) as amount from pr_emp_payslip_deductions paydedu where paydedu.emp_code = " + Convert.ToInt32(empCode) + " and paydedu.fm<='" + FM + "' and paydedu.fm>='" + startYear + "' AND paydedu.dd_name = 'VPF Deduction'";

                //VPF
                string vpfQry = "select sum(dd_amount) as amount from pr_emp_payslip_deductions a join pr_emp_payslip p on p.id = a.payslip_mid  where dd_name = 'VPF Deduction' and a.emp_code = " + Convert.ToInt32(empCode) + "  and p.fm < '" + FM + "' and p.fm >= '" + startYear + "';";



                DataSet ds = await _sha.Get_MultiTables_FromQry(general + basicQry + basicSumQry + allowaceQry +
                    sumFixedQry + sumFPAQry + sumFPIIPQry + PFPerks + LOANPerks + incentive + valQry + val2Qry + rent + otherQry + houseQry +
                    cdQry + cccQry + ccdQry + otherSectionQry + taxQry + tdsQry + empTaxQry + vpfQry);

                //tables
                var dtGeneral = ds.Tables[0];
                var dtBasic = ds.Tables[1];
                var dtBasicSum = ds.Tables[2];
                var dtAllowance = ds.Tables[3];
                var dtSumFixed = ds.Tables[4];
                var dtsumFPA = ds.Tables[5];
                var dtsumfpiip = ds.Tables[6];
                var dtPfperks = ds.Tables[7];
                var dtLoanPerks = ds.Tables[8];
                var dtIncentives = ds.Tables[9];
                var dt172 = ds.Tables[10];
                var dt173 = ds.Tables[11];
                var dtRent = ds.Tables[12];
                var dtOther = ds.Tables[13];
                var dtHouse = ds.Tables[14];

                var dtCD = ds.Tables[15];
                var dtCCC = ds.Tables[16];
                var dtCCD = ds.Tables[17];
                var dtEdu = ds.Tables[18];
                var dtTax = ds.Tables[19];
                var dtTDS = ds.Tables[20];
                var dtEmpTax = ds.Tables[21];
                var dtVpf = ds.Tables[22];

                int loss_months = 0;
                decimal? vpf_data = 0;
                decimal? pf = 0;

                if (dtEmpTax.Rows.Count > 0)
                {
                    DataRow empTax = dtEmpTax.Rows[0];

                    object val = empTax["loss"];
                    if (val == DBNull.Value)
                    {
                        loss_months = 0;
                    }
                    else
                    {
                        loss_months = Convert.ToInt32(empTax["loss"]);
                    }

                }
                //vpf

                if (dtVpf.Rows.Count > 0)
                {
                    DataRow vpfData = dtVpf.Rows[0];

                    object val = vpfData["amount"];
                    if (val == DBNull.Value)
                    {
                        vpf_data = 0;
                    }
                    else
                    {
                        vpf_data = Convert.ToDecimal(vpfData["amount"]);
                    }

                }

                // General Data
                if (dtGeneral.Rows.Count > 0)
                {
                    DataRow gen = dtGeneral.Rows[0];
                    name = Convert.ToString(gen["name"]);
                    per_address = Convert.ToString(gen["per_address"]);
                    pan_no = Convert.ToString(gen["pan_no"]);
                    emp_code = Convert.ToString(gen["emp_code"]);
                    designation = Convert.ToString(gen["designation"]);
                    sex = Convert.ToString(gen["sex"]);
                    fm = fin_month;
                }

                //present basic, da_percent,cca,hra,int_ref,t_inc,spl_da,spl_allowance
                if (dtBasic.Rows.Count > 0)
                {

                    DataRow rbasic = dtBasic.Rows[0];

                    //basic 
                    object val = rbasic["basic"];
                    if (val == DBNull.Value)
                    {
                        pbasic = 0;
                    }
                    else
                    {
                        pbasic = Convert.ToDecimal(rbasic["basic"]) * months;
                    }

                    //da_percent 
                    object val1 = rbasic["da_percent"];
                    if (val1 == DBNull.Value)
                    {
                        pda_percent = 0;
                    }
                    else
                    {
                        pda_percent = Convert.ToDecimal(rbasic["da_percent"]) * months;
                    }
                    //cca
                    object val2 = rbasic["cca"];
                    if (val2 == DBNull.Value)
                    {
                        pcca = 0;
                    }
                    else
                    {
                        pcca = Convert.ToDecimal(rbasic["cca"]) * months;
                    }
                    //hra
                    object val3 = rbasic["hra"];
                    if (val3 == DBNull.Value)
                    {
                        phra = 0;
                    }
                    else
                    {
                        phra = Convert.ToDecimal(rbasic["hra"]) * months;
                    }
                    //int_ref
                    object va4 = rbasic["int_ref"];
                    if (va4 == DBNull.Value)
                    {
                        pint_ref = 0;
                    }
                    else
                    {
                        pint_ref = Convert.ToDecimal(rbasic["int_ref"]) * months;
                    }

                    object val5 = rbasic["t_inc"];
                    if (val5 == DBNull.Value)
                    {
                        pt_inc = 0;
                    }
                    else
                    {
                        pt_inc = Convert.ToDecimal(rbasic["t_inc"]) * months;
                    }

                    object val6 = rbasic["spl_da"];
                    if (val6 == DBNull.Value)
                    {
                        pspl_da = 0;
                    }
                    else
                    {
                        pspl_da = Convert.ToDecimal(rbasic["spl_da"]) * months;
                    }

                    object val7 = rbasic["spl_allowance"];
                    if (val7 == DBNull.Value)
                    {
                        pspl_allowance = 0;
                    }
                    else
                    {
                        pspl_allowance = Convert.ToDecimal(rbasic["spl_allowance"]) * months;
                    }




                }
                // present + sum and final values are basic, da_percent,cca,hra,int_ref,t_inc,spl_da,spl_allowance
                if (dtBasicSum.Rows.Count > 0)
                {
                    DataRow sbasic = dtBasicSum.Rows[0];

                    //basic 
                    object val = sbasic["basic"];
                    if (val == DBNull.Value)
                    {
                        basic = 0 + pbasic;
                    }
                    else
                    {
                        basic = Convert.ToDecimal(sbasic["basic"]) + pbasic;
                    }

                    //da_percent 
                    object val1 = sbasic["da_percent"];
                    if (val1 == DBNull.Value)
                    {
                        da_percent = Convert.ToDecimal(0 + pda_percent);
                    }
                    else
                    {
                        da_percent = Convert.ToDecimal(Convert.ToDecimal(sbasic["da_percent"]) + pda_percent);
                    }
                    //cca
                    object val2 = sbasic["cca"];
                    if (val2 == DBNull.Value)
                    {
                        cca = Convert.ToDecimal(0 + pcca);
                    }
                    else
                    {
                        cca = Convert.ToDecimal(Convert.ToDecimal(sbasic["cca"]) + pcca);
                    }
                    //hra
                    object val3 = sbasic["hra"];
                    if (val3 == DBNull.Value)
                    {
                        hra = Convert.ToDecimal(0 + phra);
                    }
                    else
                    {
                        hra = Convert.ToDecimal(Convert.ToDecimal(sbasic["hra"]) + phra);
                    }
                    //int_ref
                    object va4 = sbasic["int_ref"];
                    if (va4 == DBNull.Value)
                    {
                        int_ref = 0 + pint_ref;
                    }
                    else
                    {
                        int_ref = Convert.ToDecimal(sbasic["int_ref"]) + pint_ref;
                    }

                    object val5 = sbasic["t_inc"];
                    if (val5 == DBNull.Value)
                    {
                        t_inc = 0 + pt_inc;
                    }
                    else
                    {
                        t_inc = Convert.ToDecimal(sbasic["t_inc"]) + pt_inc;
                    }

                    object val6 = sbasic["spl_da"];
                    if (val6 == DBNull.Value)
                    {
                        spl_da = 0 + pspl_da;
                    }
                    else
                    {
                        spl_da = Convert.ToDecimal(sbasic["spl_da"]) + pspl_da;
                    }

                    object val7 = sbasic["spl_allowance"];
                    if (val7 == DBNull.Value)
                    {
                        spl_allowance = 0 + pspl_allowance;
                    }
                    else
                    {
                        spl_allowance = Convert.ToDecimal(sbasic["spl_allowance"]) + pspl_allowance;
                    }

                    object val8 = sbasic["pf"];
                    if (val8 == DBNull.Value)
                    {
                        pf = 0;
                    }
                    else
                    {
                        pf = Convert.ToDecimal(sbasic["pf"]);
                    }



                    //basic = Convert.ToDecimal(sbasic["basic"]) + pbasic;
                    //da_percent = Convert.ToDecimal(Convert.ToDecimal(sbasic["da_percent"]) + pda_percent);
                    //cca = Convert.ToDecimal(Convert.ToDecimal(sbasic["cca"]) + pcca);
                    //hra = Convert.ToDecimal(Convert.ToDecimal(sbasic["hra"]) + phra);
                    //int_ref = Convert.ToDecimal(sbasic["int_ref"]) + pint_ref;
                    //t_inc = Convert.ToDecimal(sbasic["t_inc"]) + pt_inc;
                    //spl_da = Convert.ToDecimal(sbasic["spl_da"]) + pspl_da;
                    //spl_allowance = Convert.ToDecimal(sbasic["spl_allowance"]) + pspl_allowance;


                }
                //present allowances pper_allowance,pfpa_allow,pfpiip

                foreach (DataRow algen in dtAllowance.Rows)
                {


                    string alw_name = algen["all_name"].ToString();
                    float alw_amount = float.Parse(algen["all_amount"].ToString());

                    if (alw_name == "Fixed Personal Allowance")
                    {
                        pper_allowance = Convert.ToDecimal(alw_amount) * months;
                    }
                    else if (alw_name == PrConstants.FPA_HRA_ALLOWANCE)
                    {
                        pfpa_allow = Convert.ToDecimal(alw_amount) * months;
                    }
                    else if (alw_name == PrConstants.FPIIP)
                    {
                        pfpiip = Convert.ToDecimal(alw_amount) * months;
                    }

                }
                //dtSumFixed
                if (dtSumFixed.Rows.Count > 0)
                {
                    DataRow sumf = dtSumFixed.Rows[0];

                    object val = sumf["all_amount"];
                    if (val == DBNull.Value)
                    {
                        per_allowance = 0 + pper_allowance;
                    }
                    else
                    {
                        per_allowance = Convert.ToDecimal(sumf["all_amount"]) + pper_allowance;
                    }

                }
                //dtsumFPA
                if (dtsumFPA.Rows.Count > 0)
                {
                    DataRow sumfpa = dtsumFPA.Rows[0];

                    object val = sumfpa["all_amount"];
                    if (val == DBNull.Value)
                    {
                        fpa_allow = 0 + pfpa_allow;
                    }
                    else
                    {
                        fpa_allow = Convert.ToDecimal(sumfpa["all_amount"]) + pfpa_allow;
                    }

                }
                //dtsumfpiip
                if (dtsumfpiip.Rows.Count > 0)
                {
                    DataRow sumfpiip = dtsumfpiip.Rows[0];
                    object val = sumfpiip["all_amount"];
                    if (val == DBNull.Value)
                    {
                        fpiip = 0 + pfpiip;
                    }
                    else
                    {
                        fpiip = Convert.ToDecimal(sumfpiip["all_amount"]) + pfpiip;
                    }
                    //fpiip = Convert.ToDecimal(sumfpiip["all_amount"])+ pfpiip;

                }

                // dtPfPerks
                if (dtPfperks.Rows.Count > 0)
                {
                    DataRow pfPerk = dtPfperks.Rows[0];

                    object val = pfPerk["pf_perk"];
                    if (val == DBNull.Value)
                    {
                        pf_perk = 0;
                    }
                    else
                    {
                        pf_perk = Convert.ToDecimal(pfPerk["pf_perk"]);
                    }

                }
                //dtLoanPerks
                if (dtLoanPerks.Rows.Count > 0)
                {
                    DataRow lperk = dtLoanPerks.Rows[0];

                    object val = lperk["loan_perk"];
                    if (val == DBNull.Value)
                    {
                        loan_perk = 0;
                    }
                    else
                    {
                        loan_perk = Convert.ToDecimal(lperk["loan_perk"]);
                    }

                }
                //dtIncentives
                if (dtIncentives.Rows.Count > 0)
                {
                    DataRow ins = dtIncentives.Rows[0];

                    object val = ins["incentive"];
                    if (val == DBNull.Value)
                    {
                        incentives = 0;
                    }
                    else
                    {
                        incentives = Convert.ToDecimal(ins["incentive"]);
                    }

                }
                //dt172
                if (dt172.Rows.Count > 0)
                {
                    DataRow VAL = dt172.Rows[0];
                    object val = VAL["val172"];
                    if (val == DBNull.Value)
                    {
                        values_of_17_2 = 0;
                    }
                    else
                    {
                        values_of_17_2 = Convert.ToDecimal(VAL["val172"]);
                    }
                }
                //dt173
                if (dt173.Rows.Count > 0)
                {
                    DataRow VAL1 = dt173.Rows[0];
                    object val = VAL1["val173"];
                    if (val == DBNull.Value)
                    {
                        values_of_17_3 = 0;
                    }
                    else
                    {
                        values_of_17_2 = Convert.ToDecimal(VAL1["val173"]);
                    }
                }

                if (dtRent.Rows.Count > 0)
                {
                    DataRow rents = dtRent.Rows[0];

                    object val = rents["rentamount"];
                    if (val == DBNull.Value)
                    {
                        house_rent_allowance = 0;
                    }
                    else
                    {
                        house_rent_allowance = Convert.ToDecimal(rents["rentamount"]);
                    }
                }
                //decimal daval = Convert.ToDecimal(da);
                //decimal hraval = Convert.ToDecimal(hra);
                //rent amount
                decimal rentamt = Convert.ToDecimal(house_rent_allowance);
                //salary(a)
                decimal salary = da_percent + Convert.ToDecimal(basic);
                //40%  of salary(b)
                decimal bvalue = Convert.ToDecimal((0.40)) * (salary);
                //houserentpaid - 10% of salary(c)
                decimal hrentpaid = Math.Abs(rentamt - (Convert.ToDecimal((0.10)) * (salary)));
                house_rent_allowance = Math.Abs(Math.Min(salary, Math.Min(bvalue, hrentpaid)));



                if (dtOther.Rows.Count > 0)
                {
                    DataRow otr = dtOther.Rows[0];

                    other = Convert.ToDecimal(otr["other"]);

                }

                if (dtHouse.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtHouse.Rows)
                    {
                        if (dr["loan_type_mid"].ToString() == "4" || dr["loan_type_mid"].ToString() == "5" || dr["loan_type_mid"].ToString() == "6" ||
                            dr["loan_type_mid"].ToString() == "7" || dr["loan_type_mid"].ToString() == "8" || dr["loan_type_mid"].ToString() == "9"
                            || dr["loan_type_mid"].ToString() == "10" || dr["loan_type_mid"].ToString() == "11" || dr["loan_type_mid"].ToString() == "12"
                            || dr["loan_type_mid"].ToString() == "13")
                        {
                            count++;
                            loan_instl_amt = loan_instl_amt + Convert.ToDecimal(dr["installment_amount"]);
                        }

                    }

                }
                if (count > 0)
                {

                    houseloan_allownace = 200000;
                }

                IList<section80C> lstCer = new List<section80C>();
                IList<section80CCC> lstCCC = new List<section80CCC>();
                IList<section80CCD> lstCCD = new List<section80CCD>();
                IList<sectionOther> lstOther = new List<sectionOther>();


                if (dtCD.Rows.Count > 0)
                {
                    foreach (DataRow cer in dtCD.Rows)
                    {
                        //decimal amount = Convert.ToDecimal(cer["amount"].ToString());
                        decimal? amount = 0;

                        object val = cer["amount"];
                        if (val == DBNull.Value)
                        {
                            amount = 0;
                        }
                        else
                        {
                            amount = Convert.ToDecimal(cer["amount"]);
                        }

                        string dd_name = Convert.ToString(cer["name"].ToString());
                        if (dd_name == "Provident Fund")
                        {
                            amount = Convert.ToDecimal(pf + (amount * months));

                        }
                        else if (dd_name == "VPF")
                        {
                            amount = Convert.ToDecimal(vpf_data + (amount * months));
                        }
                        decimal? allowance_amount = 0;
                        if (min_allowance > 0 && max_allowance <= 150000)
                        {
                            if (amount > min_allowance)
                            {
                                allowance_amount = min_allowance;
                                min_allowance = min_allowance - allowance_amount;
                                max_allowance = max_allowance + allowance_amount;
                            }
                            else
                            {
                                min_allowance = min_allowance - amount;
                                allowance_amount = amount;
                                max_allowance = max_allowance + amount;
                            }
                            lstCer.Add(new section80C
                            {
                                id = cer["id"].ToString(),
                                name = cer["name"].ToString(),
                                amount ="0",
                                allowance_amount ="0",
                                ded_amount = "0",
                            });
                        }
                        else
                        {
                            lstCer.Add(new section80C
                            {
                                id = cer["id"].ToString(),
                                name = cer["name"].ToString(),
                                amount = "0",
                                allowance_amount = "0",
                                ded_amount = "0"
                            });
                        }

                    }


                }
                else
                {
                    lstCer.Add(new section80C
                    {
                        id = "",
                        name = "",
                        amount = "0",
                        allowance_amount ="0",
                        ded_amount = "0"
                    });
                }

                if (dtCCC.Rows.Count > 0)
                {
                    foreach (DataRow ccc in dtCCC.Rows)
                    {
                        decimal amount = Convert.ToDecimal(ccc["amount"].ToString());
                        decimal? allowance_amount = 0;
                        if (min_allowance > 0 && max_allowance <= 150000)
                        {
                            if (amount > min_allowance)
                            {
                                allowance_amount = min_allowance;
                                min_allowance = min_allowance - allowance_amount;
                                max_allowance = max_allowance + allowance_amount;
                            }
                            else
                            {
                                min_allowance = min_allowance - amount;
                                allowance_amount = amount;
                                max_allowance = max_allowance + amount;
                            }
                            lstCCC.Add(new section80CCC
                            {
                                id = ccc["id"].ToString(),
                                name = ccc["name"].ToString(),
                                amount = "0",
                                allowance_amount = "0",
                                ded_amount ="0",
                            });
                        }
                        else
                        {
                            lstCCC.Add(new section80CCC
                            {
                                id = ccc["id"].ToString(),
                                name = ccc["name"].ToString(),
                                amount = "0",
                                allowance_amount = "0",
                                ded_amount = "0"
                            });
                        }

                    }

                }
                else
                {
                    lstCCC.Add(new section80CCC
                    {
                        id = "",
                        name = "",
                        amount = "0",
                        allowance_amount = "0",
                        ded_amount = "0"
                    });
                }

                if (dtCCD.Rows.Count > 0)
                {
                    foreach (DataRow ccd in dtCCD.Rows)
                    {
                        decimal amount = Convert.ToDecimal(ccd["amount"].ToString());
                        decimal? allowance_amount = 0;
                        if (min_allowance > 0 && max_allowance <= 150000)
                        {
                            if (amount > min_allowance)
                            {
                                allowance_amount = min_allowance;
                                min_allowance = min_allowance - allowance_amount;
                                max_allowance = max_allowance + allowance_amount;
                            }
                            else
                            {
                                min_allowance = min_allowance - amount;
                                allowance_amount = amount;
                                max_allowance = max_allowance + amount;
                            }
                            lstCCD.Add(new section80CCD
                            {
                                id = ccd["id"].ToString(),
                                name = ccd["name"].ToString(),
                                amount = "0",
                                allowance_amount = "0",
                                ded_amount = "0",
                            });
                        }
                        else
                        {
                            lstCCD.Add(new section80CCD
                            {
                                id = ccd["id"].ToString(),
                                name = ccd["name"].ToString(),
                                amount = "0",
                                allowance_amount = "0",
                                ded_amount = "0"
                            });
                        }

                    }

                }
                else
                {
                    lstCCD.Add(new section80CCD
                    {
                        id = "",
                        name = "",
                        amount = "0",
                        allowance_amount = "0",
                        ded_amount = "0"
                    });
                }

                if (dtEdu.Rows.Count > 0)
                {
                    foreach (DataRow ed in dtEdu.Rows)
                    {
                        decimal amount = Convert.ToDecimal(ed["amount"].ToString());
                        decimal? allowance_amount = 0;
                        if (min_allowance_other > 0 && max_allowance_other <= 150000)
                        {
                            if (amount > min_allowance_other)
                            {
                                allowance_amount = min_allowance_other;
                                min_allowance_other = min_allowance_other - allowance_amount;
                                max_allowance_other = max_allowance_other + allowance_amount;
                            }
                            else
                            {
                                min_allowance_other = min_allowance_other - amount;
                                allowance_amount = amount;
                                max_allowance_other = max_allowance_other + amount;
                            }
                            lstOther.Add(new sectionOther
                            {
                                id = ed["id"].ToString(),
                                name = ed["name"].ToString(),
                                section = ed["section"].ToString(),
                                amount = "0",
                                allowance_amount = "0",
                                ded_amount = "0",
                            });
                        }
                        else
                        {
                            lstOther.Add(new sectionOther
                            {
                                id = ed["id"].ToString(),
                                name = ed["name"].ToString(),
                                section = ed["section"].ToString(),
                                amount = "0",
                                allowance_amount = "0",
                                ded_amount ="0"
                            });
                        }

                    }

                }
                else
                {
                    lstOther.Add(new sectionOther
                    {
                        id = "",
                        name = "",
                        section = "",
                        amount = "0",
                        allowance_amount ="0",
                        ded_amount = "0"
                    });
                }

                if (dtTax.Rows.Count > 0)
                {
                    DataRow tax = dtTax.Rows[0];

                    tax_till = (float)Convert.ToDecimal(tax["tax_deducted_at_source"]);

                }

                if (dtTDS.Rows.Count > 0)
                {
                    DataRow TDS = dtTDS.Rows[0];

                    object val = TDS["tds_amount"];
                    if (val == DBNull.Value)
                    {
                        paid_by_emp = 0;
                    }
                    else
                    {
                        paid_by_emp = Convert.ToDecimal(TDS["tds_amount"]);
                    }
                }

                IList<TDSProcess> lstTDS = new List<TDSProcess>();

                gross = basic + da_percent + per_allowance + int_ref + t_inc + fpa_allow + fpiip + pf_perk + loan_perk + incentives + spl_allowance + spl_da + hra + cca + values_of_17_2 + values_of_17_3 + paid_by_emp; // 1.

                total_section = house_rent_allowance;

                balance = gross - total_section;
                standard_deduction = PrConstants.STANDARD_DEDUCTION;
                emp_tax = PrConstants.EMP_TAX - (loss_months * 200);
                aggregate_of_4 = Convert.ToDecimal((standard_deduction + emp_tax));
                Income_chargable = balance - aggregate_of_4; //6

                total_gross_income = Income_chargable + other - houseloan_allownace;

                deductible = Convert.ToDecimal(max_allowance + max_allowance_other);
                total_income = total_gross_income - deductible;
                //total_income = Convert.ToDecimal(841240.01);
                round_income = Convert.ToDecimal(Math.Round(Convert.ToDouble(total_income)));

                if (total_income > 500000)
                {
                    income = 0;
                    section_87a = 0;
                    edu_cess = income * 4 / 100;

                    tax_payable = (float)income + edu_cess;
                    if (tax_till > tax_payable)
                    {
                        tax_balance = tax_till - tax_payable;
                    }
                    else
                    {
                        tax_balance = tax_payable - tax_till;
                    }

                    tds_per_month = tax_balance / months;
                }
                else
                {
                    income = 0;
                    section_87a = Convert.ToDecimal(income);
                    edu_cess = 0;

                    tax_payable = 0;

                    tax_till = 0;
                    tax_balance = 0;

                    tds_per_month = 0;
                }
                //string employer_name_address = PrConstants.Name;
                //string employer_pan = PrConstants.PAN;
                //string employer_tan = PrConstants.TAN;

                lstTDS.Add(new TDSProcess
                {
                    name = name,
                    per_address = per_address,
                    pan_no = pan_no,
                    emp_code = emp_code,
                    designation = designation,
                    sex = sex,
                    fm = fm,
                    employer_name_address = employer_name_address,
                    employer_pan = employer_pan,
                    employer_tan = employer_tan,
                    basic = "0",
                    da_percent = "0",
                    per_allowance = "0",
                    int_ref = "0",
                    t_inc = "0",
                    fpa_allow = "0",
                    fpiip = "0",
                    pf_perk = "0",
                    loan_perk = "0",
                    incentives = "0",
                    spl_allowance = "0",
                    spl_da = "0",
                    hra = "0",
                    cca = "0",
                    values_of_17_2 = "0",
                    values_of_17_3 = "0",
                    gross = "0", // 2 pending
                    standard_deduction = "0",
                    ent_allowance = "0",
                    emp_tax = "0",
                    aggregate_of_4 = "0",
                    Income_chargable = "0",
                    other = "0",
                    houseloan_allownace = "0",
                    total_gross_income = "0",
                    loan_instl_amt = "0",
                    deductible = "0",
                    total_income = "0",
                    income = "0",
                    edu_cess = "0",
                    tax_payable = "0",
                    tax_till = "0",
                    tax_balance = "0",
                    balance_month = "0",
                    tds_per_month = "0",
                    house_rent_allowance = "0",
                    total_section = "0",
                    balance = "0",
                    round_income = "0",
                    section_87a = "0",
                    paid_by_emp = "0",

                }); ;
                var tds = JsonConvert.SerializeObject(lstTDS);
                var ccT = JsonConvert.SerializeObject(lstCer);
                var cccT = JsonConvert.SerializeObject(lstCCC);
                var ccdT = JsonConvert.SerializeObject(lstCCD);
                var otherT = JsonConvert.SerializeObject(lstOther);

                var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var tdsData = javaScriptSerializer.DeserializeObject(tds);
                var ccData = javaScriptSerializer.DeserializeObject(ccT);
                var cccData = javaScriptSerializer.DeserializeObject(cccT);
                var ccdData = javaScriptSerializer.DeserializeObject(ccdT);
                var otherData = javaScriptSerializer.DeserializeObject(otherT);

                var resultJson = javaScriptSerializer.Serialize(new
                {
                    tds = tdsData,
                    cc = ccData,
                    ccc = cccData,
                    ccd = ccdData,
                    other = otherData

                });
                int NewNumIndex = 0;

                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());

                int finalSub = 0;

                //if (final == true)
                //{
                //    finalSub = 1;
                //}
                //else
                //{
                //    finalSub = 0;
                //}

                string selQry = "select id,final from pr_emp_tds_process where active=1 and empcode = " + Convert.ToInt32(empCode) + " and Month(fm)='" + checkMonth + "' and Year(fm)='" + checkYear + "';";

                DataTable details = await _sha.Get_Table_FromQry(selQry);
                if (details.Rows.Count > 0)
                {

                    DataRow finaldt = details.Rows[0];

                    int finalCount = Convert.ToInt32(finaldt["final"]);

                    if (finalCount == 1)
                    {
                        return "E#TDS Process#Employee Already Processed..!!";
                    }
                    else
                    {
                        NewNumIndex++;
                        string tdsUpdate = "update pr_emp_tds_process set active=0 where empcode = " + Convert.ToInt32(empCode) + ";";
                        sbqry.Append(tdsUpdate);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_tds_process", empCode.ToString() + NewNumIndex, ""));

                        NewNumIndex++;
                        string certUpdate = "update pr_emp_tds_section_deductions set active=0 where empcode = " + Convert.ToInt32(empCode) + ";";
                        sbqry.Append(certUpdate);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_tds_section_deductions", empCode.ToString() + NewNumIndex, ""));


                        if (tdsData != null)
                        {
                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_tds_process", NewNumIndex));

                            string qry = "INSERT INTO pr_emp_tds_process ([id],[fy],[fm],[empid],[empcode]," +
                                "[tan_no],[sal_basic],[sal_fixed_personal_allowance],[sal_fpa_hra_allowance],[sal_fpiip]," +
                                "[sal_da],[sal_hra],[sal_cca],[sal_interim_relief],[sal_telangana_increment],[sal_spl_allow]," +
                                "[sal_spcl_da],[sal_pfperks],[sal_loanperks],[sal_incentive],[sal_value_of_perquisites]," +
                                "[sal_profits_in_lieu_of_salary],[gross_salary],[house_rent_allowance],[total_of_sec10]," +
                                "[balance_gross_min_sec10],[standard_deductions],[tax_of_employement],[tds_aggregate]," +
                                "[income_chargeable_bal_minus_agg],[other_income_by_the_emp],[interest_on_housing]," +
                                "[gross_total_income],[aggregate_of_deductible],[total_income],[tax_on_total_income]," +
                                "[section_87a],[education_cess],[tax_payable],[tax_deducted_at_source],[tax_paid_by_the_employer]," +
                                "[balance_tax],[balance_months],[tds_per_month],[active],[trans_id],[tds_update],[final]) VALUES (@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                                    " where empid=" + empCode + ")," + empCode + ",'" + employer_tan + "'," + basic + "," +
                                    "" + per_allowance + "," + fpa_allow + "," + fpiip + "," +
                                    "" + da_percent + "," + hra + "," + cca + "," + int_ref + "," +
                                    "" + t_inc + "," + spl_allowance + "," + spl_da + "," + pf_perk + "," +
                                    "" + loan_perk + "," + incentives + "," + values_of_17_2 + "," + values_of_17_3 + "," + gross + "," + house_rent_allowance + "," +
                                    "" + total_section + "," + balance + "," + standard_deduction + "," + emp_tax + "," +
                                    "" + aggregate_of_4 + "," + Income_chargable + "," + other + "," + houseloan_allownace + "," +
                                    "" + total_gross_income + "," + deductible + "," + total_income + "," + income + "," +
                                    "" + section_87a + "," + edu_cess + "," + tax_payable + "," + tax_till + "," +
                                    "" + paid_by_emp + "," + tax_balance + "," + months + "," + tds_per_month + ",1,@transidnew,0," + finalSub + ");";

                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_tds_process", "@idnew" + NewNumIndex, empCode.ToString()));
                        }

                        if (lstCer != null)
                        {
                            foreach (var ccDatas in lstCer)
                            {
                                if (ccDatas.id != "")
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_tds_section_deductions", NewNumIndex));

                                    string bioQry = "INSERT INTO pr_emp_tds_section_deductions ([id],[fy],[fm],[empid],[empcode],[m_id]," +
                                        "[section_type],[gross],[qual],[ded],[active],[trans_id]) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                                        " where empid=" + empCode + ")," + empCode + "," + ccDatas.id + ",'Section80C'," + ccDatas.amount + "," + ccDatas.allowance_amount + "," + ccDatas.ded_amount + ",1,@transidnew);";
                                    sbqry.Append(bioQry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_tds_section_deductions", "@idnew" + NewNumIndex, empCode.ToString()));
                                }

                            }
                        }
                        if (lstCCC != null)
                        {
                            foreach (var cccDatas in lstCCC)
                            {
                                if (cccDatas.id != "")
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_tds_section_deductions", NewNumIndex));

                                    string bioQry = "INSERT INTO pr_emp_tds_section_deductions ([id],[fy],[fm],[empid],[empcode],[m_id]," +
                                        "[section_type],[gross],[qual],[ded],[active],[trans_id]) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                                        " where empid=" + empCode + ")," + empCode + "," + cccDatas.id + ",'Section80CCC'," + cccDatas.amount + "," + cccDatas.allowance_amount + "," + cccDatas.ded_amount + ",1,@transidnew);";
                                    sbqry.Append(bioQry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_tds_section_deductions", "@idnew" + NewNumIndex, empCode.ToString()));
                                }

                            }
                        }
                        if (lstCCD != null)
                        {
                            foreach (var ccdDatas in lstCCD)
                            {
                                if (ccdDatas.id != "")
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_tds_section_deductions", NewNumIndex));

                                    string bioQry = "INSERT INTO pr_emp_tds_section_deductions ([id],[fy],[fm],[empid],[empcode],[m_id]," +
                                        "[section_type],[gross],[qual],[ded],[active],[trans_id]) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                                        " where empid=" + empCode + ")," + empCode + "," + ccdDatas.id + ",'Section80CCD'," + ccdDatas.amount + "," + ccdDatas.allowance_amount + "," + ccdDatas.ded_amount + ",1,@transidnew);";
                                    sbqry.Append(bioQry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_tds_section_deductions", "@idnew" + NewNumIndex, empCode.ToString()));
                                }

                            }
                        }
                        if (lstOther != null)
                        {
                            foreach (var otherDatas in lstOther)
                            {
                                if (otherDatas.id != "")
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_tds_section_deductions", NewNumIndex));

                                    string bioQry = "INSERT INTO pr_emp_tds_section_deductions ([id],[fy],[fm],[empid],[empcode],[m_id]," +
                                        "[section_type],[gross],[qual],[ded],[active],[trans_id]) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                                        " where empid=" + empCode + ")," + empCode + "," + otherDatas.id + ",'Other'," + otherDatas.amount + "," + otherDatas.allowance_amount + "," + otherDatas.ded_amount + ",1,@transidnew);";
                                    sbqry.Append(bioQry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_tds_section_deductions", "@idnew" + NewNumIndex, empCode.ToString()));
                                }

                            }
                        }
                        if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                        {
                            //return "I#TDS Process#TDS Processed Successfully..!!";
                        }
                        else
                        {
                            //return "E#TDS Process#Error While TDS Process..!!";
                        }
                    }



                }
                else
                {

                    try
                    {

                        if (tdsData != null)
                        {
                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_tds_process", NewNumIndex));

                            string qry = "INSERT INTO pr_emp_tds_process ([id],[fy],[fm],[empid],[empcode]," +
                                "[tan_no],[sal_basic],[sal_fixed_personal_allowance],[sal_fpa_hra_allowance],[sal_fpiip]," +
                                "[sal_da],[sal_hra],[sal_cca],[sal_interim_relief],[sal_telangana_increment],[sal_spl_allow]," +
                                "[sal_spcl_da],[sal_pfperks],[sal_loanperks],[sal_incentive],[sal_value_of_perquisites]," +
                                "[sal_profits_in_lieu_of_salary],[gross_salary],[house_rent_allowance],[total_of_sec10]," +
                                "[balance_gross_min_sec10],[standard_deductions],[tax_of_employement],[tds_aggregate]," +
                                "[income_chargeable_bal_minus_agg],[other_income_by_the_emp],[interest_on_housing]," +
                                "[gross_total_income],[aggregate_of_deductible],[total_income],[tax_on_total_income]," +
                                "[section_87a],[education_cess],[tax_payable],[tax_deducted_at_source],[tax_paid_by_the_employer]," +
                                "[balance_tax],[balance_months],[tds_per_month],[active],[trans_id],[tds_update],[final]) VALUES (@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                                    " where empid=" + empCode + ")," + empCode + ",'" + employer_tan + "'," + basic + "," +
                                    "" + per_allowance + "," + fpa_allow + "," + fpiip + "," +
                                    "" + da_percent + "," + hra + "," + cca + "," + int_ref + "," +
                                    "" + t_inc + "," + spl_allowance + "," + spl_da + "," + pf_perk + "," +
                                    "" + loan_perk + "," + incentives + "," + values_of_17_2 + "," + values_of_17_3 + "," + gross + "," + house_rent_allowance + "," +
                                    "" + total_section + "," + balance + "," + standard_deduction + "," + emp_tax + "," +
                                    "" + aggregate_of_4 + "," + Income_chargable + "," + other + "," + houseloan_allownace + "," +
                                    "" + total_gross_income + "," + deductible + "," + total_income + "," + income + "," +
                                    "" + section_87a + "," + edu_cess + "," + tax_payable + "," + tax_till + "," +
                                    "" + paid_by_emp + "," + tax_balance + "," + months + "," + tds_per_month + ",1,@transidnew,0," + finalSub + ");";

                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_tds_process", "@idnew" + NewNumIndex, empCode.ToString()));
                        }

                        if (lstCer != null)
                        {
                            foreach (var ccDatas in lstCer)
                            {
                                if (ccDatas.id != "")
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_tds_section_deductions", NewNumIndex));

                                    string bioQry = "INSERT INTO pr_emp_tds_section_deductions ([id],[fy],[fm],[empid],[empcode],[m_id]," +
                                        "[section_type],[gross],[qual],[ded],[active],[trans_id]) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                                        " where empid=" + empCode + ")," + empCode + "," + ccDatas.id + ",'Section80C'," + ccDatas.amount + "," + ccDatas.allowance_amount + "," + ccDatas.ded_amount + ",1,@transidnew);";
                                    sbqry.Append(bioQry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_tds_section_deductions", "@idnew" + NewNumIndex, empCode.ToString()));
                                }

                            }
                        }
                        if (lstCCC != null)
                        {
                            foreach (var cccDatas in lstCCC)
                            {
                                if (cccDatas.id != "")
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_tds_section_deductions", NewNumIndex));

                                    string bioQry = "INSERT INTO pr_emp_tds_section_deductions ([id],[fy],[fm],[empid],[empcode],[m_id]," +
                                        "[section_type],[gross],[qual],[ded],[active],[trans_id]) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                                        " where empid=" + empCode + ")," + empCode + "," + cccDatas.id + ",'Section80CCC'," + cccDatas.amount + "," + cccDatas.allowance_amount + "," + cccDatas.ded_amount + ",1,@transidnew);";
                                    sbqry.Append(bioQry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_tds_section_deductions", "@idnew" + NewNumIndex, empCode.ToString()));
                                }

                            }
                        }
                        if (lstCCD != null)
                        {
                            foreach (var ccdDatas in lstCCD)
                            {
                                if (ccdDatas.id != "")
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_tds_section_deductions", NewNumIndex));

                                    string bioQry = "INSERT INTO pr_emp_tds_section_deductions ([id],[fy],[fm],[empid],[empcode],[m_id]," +
                                        "[section_type],[gross],[qual],[ded],[active],[trans_id]) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                                        " where empid=" + empCode + ")," + empCode + "," + ccdDatas.id + ",'Section80CCD'," + ccdDatas.amount + "," + ccdDatas.allowance_amount + "," + ccdDatas.ded_amount + ",1,@transidnew);";
                                    sbqry.Append(bioQry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_tds_section_deductions", "@idnew" + NewNumIndex, empCode.ToString()));
                                }

                            }
                        }
                        if (lstOther != null)
                        {
                            foreach (var otherDatas in lstOther)
                            {
                                if (otherDatas.id != "")
                                {
                                    NewNumIndex++;
                                    sbqry.Append(GetNewNumStringArr("pr_emp_tds_section_deductions", NewNumIndex));

                                    string bioQry = "INSERT INTO pr_emp_tds_section_deductions ([id],[fy],[fm],[empid],[empcode],[m_id]," +
                                        "[section_type],[gross],[qual],[ded],[active],[trans_id]) " +
                                        "VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                                        " where empid=" + empCode + ")," + empCode + "," + otherDatas.id + ",'Other'," + otherDatas.amount + "," + otherDatas.allowance_amount + "," + otherDatas.ded_amount + ",1,@transidnew);";
                                    sbqry.Append(bioQry);

                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_tds_section_deductions", "@idnew" + NewNumIndex, empCode.ToString()));
                                }

                            }
                        }
                        if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                        {
                            //return "I#TDS Process#TDS Processed Successfully..!!";
                        }
                        else
                        {
                            //return "E#TDS Process#Error While TDS Process..!!";
                        }
                    }
                    catch (Exception e)
                    {
                        return "E#TDS Process#" + e.Message;
                    }
                }
                return resultJson;
            }
        }
           

    }


}
