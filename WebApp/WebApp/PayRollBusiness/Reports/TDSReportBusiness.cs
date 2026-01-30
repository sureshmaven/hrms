
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

namespace PayRollBusiness.Reports
{

    public class TDSReportBusiness : BusinessBase
    {
        // CommonReportModel crm = new CommonReportModel();
        public TDSReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        public string AddZerosAfterDecimal(string amount)
        {
            float number = float.Parse(amount);
            string ret = "";
            ret = String.Format("{0:0.00}", number);
            return ret;
        }
        //pfreport by lalitha 25/08/2020
        public class uanpfmodel
        {
            public string UAN { get; set; }
            public string PF { get; set; }
        }
        public async Task<string> getOBShareData(string empCode)
        {
            decimal pf1 = 0;
            decimal pf2 = 0;

            string uanpfdetails = "select pf_no,uan_no from pr_emp_general where emp_code=" + empCode + "";
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

            string shareNonQry = "select own_share, bank_share, vpf from pr_emp_pf_nonrepayable_loan " +
                "where emp_code =" + Convert.ToString(empCode) + " ;";

            string instbal = "select os_open_int as own_share_intrst_amount ,bs_open_int as bank_share_intrst_amount,vpf_open_int as vpf_intrst_amount from pr_pf_open_bal_year " +
                "where emp_code=" + Convert.ToString(empCode) + " and fy=(select year(fm) as fm from pr_month_details where active=1)";

            string prevQry = "select pf_return as prev_own, vpf_return as prev_vpf, bank_return as prev_bank from pr_pf_open_bal_year " +
                "where emp_code = " + Convert.ToString(empCode) + " and fy = (select fy-1 from pr_month_details where active=1) ";



            string presQry = "select top(1) own_share as pres_own, vpf as pres_vpf,bank_share as pres_bank " +
                "from pr_emp_pf_nonrepayable_loan where year(fm) = year(getdate()) and emp_code = " + Convert.ToString(empCode) + "  order by process_date desc; ";

            DataSet ds = await _sha.Get_MultiTables_FromQry(shareQry + shareNonQry + prevQry + presQry + instbal + uanpfdetails);
            DataTable share = ds.Tables[0];
            DataTable loanShare = ds.Tables[1];
            DataTable prevshare = ds.Tables[2];
            DataTable presshre = ds.Tables[3];

            DataTable presint = ds.Tables[4];
            DataTable UANPF = ds.Tables[5];

            IList<ObShare> lstShare = new List<ObShare>();
            IList<ObShareIntrst> lstInstShare = new List<ObShareIntrst>();
            IList<ObSharePrev> lstPrevShare = new List<ObSharePrev>();
            IList<ObSharePres> lstPresShare = new List<ObSharePres>();
            IList<uanpfmodel> UAN = new List<uanpfmodel>();
            if (UANPF.Rows.Count > 0)
            {
                foreach (DataRow uan in UANPF.Rows)
                {
                    UAN.Add(new uanpfmodel
                    {
                        UAN = uan["uan_no"].ToString(),
                        PF = uan["pf_no"].ToString(),
                    });
                }
            }
            if (share.Rows.Count > 0)
            {
                foreach (DataRow gen in share.Rows)
                {
                    lstShare.Add(new ObShare
                    {
                        fund_own = gen["own_share"].ToString(),
                        fund_bank = gen["bank_share"].ToString(),
                        fund_vpf = gen["vpf"].ToString(),
                    });




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
            string qry3 = "select l.emp_code,a.principal_balance_amount as Loan_Closing from pr_emp_adv_loans_adjustments a join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid  join pr_loan_master lm on lm.id = l.loan_type_mid join pr_emp_adv_loans_child cl on l.id = cl.emp_adv_loans_mid  and cl.id = a.emp_adv_loans_child_mid  " +
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
                        pf1 = 0;
                    }
                }


            }

            string qry4 = "select l.emp_code,a.principal_balance_amount as Loan_Closing from pr_emp_adv_loans_adjustments a join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid  join pr_loan_master lm on lm.id = l.loan_type_mid join pr_emp_adv_loans_child cl on l.id = cl.emp_adv_loans_mid  and cl.id = a.emp_adv_loans_child_mid  " +
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

            var uandetails = JsonConvert.SerializeObject(UAN);
            var sharePoints = JsonConvert.SerializeObject(lstShare);
            var instPoints = JsonConvert.SerializeObject(lstInstShare);
            var prevPoints = JsonConvert.SerializeObject(lstPrevShare);
            var presPoints = JsonConvert.SerializeObject(lstPresShare);
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var UanData = javaScriptSerializer.DeserializeObject(uandetails);
            var shareData = javaScriptSerializer.DeserializeObject(sharePoints);
            var instData = javaScriptSerializer.DeserializeObject(instPoints);
            var prevData = javaScriptSerializer.DeserializeObject(prevPoints);
            var presData = javaScriptSerializer.DeserializeObject(presPoints);
            var resultJson = javaScriptSerializer.Serialize(new
            {
                UanData1 = UanData,
                pf2 = pf2,
                pf1 = pf1,
                share = shareData,
                inst = instData,
                prev = prevData,
                pres = presData

            });
            return resultJson;

        }
        public async Task<IList<CommonReportModel>> getTdsDetails(string EmpCode)
        {
            string employer_pan = PrConstants.PAN;
            int iFY = _LoginCredential.FY;
            int dtFM = _LoginCredential.FM;
            DateTime Financial_md = (_LoginCredential.FinancialMonthDate);
            IList<CommonReportModel> lst = new List<CommonReportModel>();
            string general = "";
            string tdsdetail = "";
            string daQry = "";
            DateTime str;
            string str1;
            string section1 = "";
            string section2 = "";
            string section3 = "";
            string section4 = "";
            string allowances = "";
            string checkprocess = " ";
            int rowid = 0;
            try
            {

                // CommonReportModel crm = new TDSReport();

                if (EmpCode == "^2")
                {
                    EmpCode = "0";
                }

                if (EmpCode.Contains("^"))
                {
                    EmpCode = "0";


                }
                string checkquery = "select max(fm) as fm from pr_emp_tds_process";
                DataTable dtchk = await _sha.Get_Table_FromQry(checkquery);
                DateTime dttt = Convert.ToDateTime(dtchk.Rows[0]["fm"]);
                string datee = dttt.ToString("yyyy-MM-dd");
                string [] dateghg=datee.Split('-');
                int year =Convert.ToInt32(dateghg[0]);
                int month = Convert.ToInt32(dateghg[1]);
                int day = Convert.ToInt32(dateghg[2]);
                if(iFY!= year && dtFM!= month)
                {
                    Financial_md = dttt;
                }
                if (EmpCode == "All")
                {
                    //str = Convert.ToDateTime(month);
                    //str1 = str.ToString("yyyy-MM-dd");
                    string emp_codes = "0";
                    string qrySel = "SELECT empcode " +
                                     "FROM pr_emp_tds_process " +

                                       " where  Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and active=1 and sal_basic>0;";
                    DataTable dt = await _sha.Get_Table_FromQry(qrySel);


                    foreach (DataRow dr in dt.Rows)
                    {

                        //string str;
                        emp_codes += dr["empcode"] + "," ;

                    }
                    emp_codes = emp_codes.Substring(0, emp_codes.Length - 1);
                    string[] arrEmpId = emp_codes.Split(',');
                    foreach (string empId in arrEmpId)
                    {
                        if (empId == " ")
                        {

                            //str = Convert.ToDateTime(month);
                            //str1 = str.ToString("yyyy-MM-dd");

                            general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                        "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code,d.Description " +
                                        " as designation,gen.sex as gender " +
                                        "from pr_emp_general gen left outer " +
                                        "join Employees emp on gen.emp_code = emp.EmpId " +
                                        " JOIN Designations d ON d.Id=emp.CurrentDesignation " +
                                         "where gen.emp_code = '0' ;";

                            daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                             "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                            tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                             " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                              " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                              "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                              " gross_salary, house_rent_allowance, total_of_sec10, " +
                                              " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                              "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                              " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                               " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                "balance_tax,balance_months,tds_per_month " +
                                               " from pr_emp_tds_process " +
                                               "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                                               "empcode =  '0';";
                            section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                             "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                             "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                             "ef.type = 'per_ded' and epf.section_type = 'Section80C' and ef.name not in('LIC','VPF','Provident Fund','Housing Loan Main','Housing Addl.Loan - 2D','GSLI') union all select 'Provident Fund' as name, gross , qual,ded " +
                             "from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 " +
                             " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2 " +
                             " union all select 'LIC' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=3 " +
                             " union all select 'Housing Loan Main' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=4 and gross>0 " +
                             " union all select 'Housing Addl.Loan - 2D' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=5 and gross>0 " +
                             " union all select 'GSLI' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=6 and gross>0 " +
                             " union all select 'Housing Loan 2B-2C' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=7 and gross>0 " +
                             " union all select 'Housing Loan 2A' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=8 and gross>0;";

                            section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                  "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                  "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                                  "ef.type = 'per_ded' and epf.section_type = 'Section80CCC' "; ;
                            section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                 "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                 "and epf.empcode = '0' WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                                 "ef.type = 'per_ded' and epf.section_type = 'Section80CCD' ";
                                 //"union select  'NPS' as name, ef.NPS as gross, ef.NPS as qual,  ef.NPS as ded from pr_emp_payslip ef WHERE ef.emp_code = " + Convert.ToInt32(empId) + " and ef.active = 1 and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " ";
                            section4 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.section as section,epf.ded as ded  " +
                                 "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                 "and epf.empcode = '0' WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                                 "ef.type = 'per_ded' and epf.section_type = 'Other'";
                            DataSet ds1 = await _sha.Get_MultiTables_FromQry(general + daQry + tdsdetail + section1 + section2 + section3 + section4);
                            DataTable dtALL1 = ds1.Tables[0];
                            DataTable dtmon1 = ds1.Tables[1];
                            DataTable dttds1 = ds1.Tables[2];
                            DataTable sec11 = ds1.Tables[3];
                            DataTable sec21 = ds1.Tables[4];
                            DataTable sec31 = ds1.Tables[5];
                            DataTable sec41 = ds1.Tables[6];
                            if (ds1.Tables[2].Rows.Count > 0)
                            {
                                if (ds1.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drEmp = ds1.Tables[0].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "H",
                                        //column1 = "<span style = 'background-color:#ADD8E6' >NAME OF THE EMPLOYEE: " + drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")" + "</span>",
                                        column1 = "<span style='color:#C8EAFB'>~</span>"
                             + ReportColHeader(0, "Name and Address of the Employer", "" + drEmp["name"].ToString() + ", TGCAB,Hyderabad"),
                                        column2 = "`",
                                        column3 = "`",
                                        column4 = "`",
                                    });

                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "<span style = 'background-color:#ADD8E6' >Name and Address of the Employer:</span>",
                                    //    column2 = "<span style = 'background-color:#ADD8E6' >TSCAB,Hyderabad</span>",

                                    //    column3 = "<span style = 'background-color:#ADD8E6' >Name:</span>",
                                    //    column4 = "<span style = 'background-color:#ADD8E6' >" + drEmp["name"].ToString() + "</span>",

                                    //});
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "PAN NO:",
                                        column2 = ReportColFooterAlignleft(drEmp["pannumber"].ToString()),
                                        column3 = "Emp Code:",
                                        column4 = drEmp["emp_code"].ToString(),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",

                                        column1 = "TAN NO",
                                        column2 = ReportColFooterAlignleft("HYDT06401D"),
                                        column3 = "Designation:",
                                        column4 = drEmp["designation"].ToString(),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "Gender:",
                                        column4 = drEmp["gender"].ToString(),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "PAN No:",
                                        column4 = drEmp["pannumber"].ToString(),

                                    });
                                }
                                if (ds1.Tables[1].Rows.Count > 0)
                                {


                                    DataRow drtime = ds1.Tables[1].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "Month:",
                                        column4 = drtime["fm"].ToString(),

                                    });
                                }
                                if (ds1.Tables[2].Rows.Count > 0)
                                {


                                    DataRow drtds = ds1.Tables[2].Rows[0];
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Basic",
                                        column4 = ReportColConvertToDecimal(drtds["sal_basic"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Fixed Personal Allowance",
                                        column4 = ReportColConvertToDecimal(drtds["sal_fixed_personal_allowance"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "FPA-HRA Allowance",
                                        column4 = ReportColConvertToDecimal(drtds["sal_fpa_hra_allowance"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "FPIIP",
                                        column4 = ReportColConvertToDecimal(drtds["sal_fpiip"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "DA",
                                        column4 = ReportColConvertToDecimal(drtds["sal_da"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "HRA",
                                        column4 = ReportColConvertToDecimal(drtds["sal_hra"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "CCA",
                                        column4 = ReportColConvertToDecimal(drtds["sal_cca"].ToString()),
                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "INTERIM RELIEF",
                                        column4 = ReportColConvertToDecimal(drtds["sal_interim_relief"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Telangana Increment",
                                        column4 = ReportColConvertToDecimal(drtds["sal_telangana_increment"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spl. Allow",
                                        column4 = ReportColConvertToDecimal(drtds["sal_spl_allow"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spcl. DA",
                                        column4 = ReportColConvertToDecimal(drtds["sal_spcl_da"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "PFPerks",
                                        column4 = ReportColConvertToDecimal(drtds["sal_pfperks"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "LOANPerks",
                                        column4 = ReportColConvertToDecimal(drtds["sal_loanperks"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Incentive",
                                        column4 = ReportColConvertToDecimal(drtds["sal_incentive"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Value of perquisites u/s 17(2)(as per Form.12BA)",
                                        column4 = ReportColConvertToDecimal(drtds["sal_value_of_perquisites"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Profits in lieu of salary u/s 17(3)(as per Form.12BA)",
                                        column4 = ReportColConvertToDecimal(drtds["sal_profits_in_lieu_of_salary"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Gross Salary",
                                        column4 = ReportColConvertToDecimal(drtds["gross_salary"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "2. Less : Allowance to the Extent exempt under Section 10",

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "House Rent Allowance",
                                        column4 = ReportColConvertToDecimal(drtds["house_rent_allowance"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Total of Section 10",
                                        column4 = ReportColConvertToDecimal(drtds["total_of_sec10"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "3. Balance(1-2)",
                                        column4 = ReportColConvertToDecimal(drtds["balance_gross_min_sec10"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "4. Deductions :",


                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "a. Standard Deduction",
                                        column4 = ReportColConvertToDecimal(drtds["standard_deductions"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "b. Tax on Employment",
                                        column4 = ReportColConvertToDecimal(drtds["tax_of_employement"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "5. Aggregate of 4(a) and (b)",
                                        column4 = ReportColConvertToDecimal(drtds["tds_aggregate"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "6. Income Chargeable Under the Head Salaries (3-5)",
                                        column4 = ReportColConvertToDecimal(drtds["income_chargeable_bal_minus_agg"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "7. Add : Any Other Income:",


                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Reported by the Employee",
                                        column4 = ReportColConvertToDecimal(drtds["other_income_by_the_emp"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Interest On Housing",
                                        column4 = ReportColConvertToDecimal(drtds["interest_on_housing"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "8. Gross Total Income",
                                        column4 = ReportColConvertToDecimal(drtds["gross_total_income"].ToString()),

                                    });
                                }


                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "9. Deductions Under Chapter VI-A",
                                    column2 = ReportColFooterAlignleft("Gross.Amt"),
                                    column3 = "Qual.Amt",
                                    column4 = "Ded.Amt",


                                });
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(A) Sections 80C,80CCC and 80CCD",



                                });
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(a) Section 80C",
                                });
                                if (ds1.Tables[3].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect1 in sec11.Rows)
                                    {
                                        //DataRow sect1 = ds.Tables[3].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {

                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect1["name"].ToString(),
                                            column2 = sect1["gross"].ToString(),
                                            column3 = ReportColFooterAlign(sect1["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect1["ded"].ToString()),

                                        }); ;
                                    }
                                }

                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "b) Section 80CCC",
                                });
                                if (ds1.Tables[4].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect2 in sec21.Rows)
                                    {
                                        int id = count++;
                                        //DataRow sect2 = ds.Tables[4].Rows[0];
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect2["name"].ToString(),
                                            column2 = sect2["gross"].ToString(),
                                            column3 = ReportColFooterAlign(sect2["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect2["ded"].ToString()),

                                        });
                                    }
                                }

                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(c) Section 80CCD",
                                });

                                if (ds1.Tables[5].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect3 in sec31.Rows)
                                    {
                                        //DataRow sect3 = ds.Tables[5].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect3["name"].ToString(),
                                            column2 = sect3["gross"].ToString(),
                                            column3 = ReportColFooterAlign(sect3["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect3["ded"].ToString()),

                                        });
                                    }
                                }
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(B) Other Sections Under Chaper VI-A",
                                });
                                if (ds1.Tables[6].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect4 in sec41.Rows)
                                    {
                                        //DataRow sect4 = ds.Tables[6].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + "" + sect4["section"].ToString() + ",   " + sect4["name"].ToString(),
                                            column2 = sect4["gross"].ToString(),
                                            column3 = ReportColFooterAlign(sect4["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect4["ded"].ToString()),

                                        });
                                    }
                                }
                                if (ds1.Tables[2].Rows.Count > 0)
                                {


                                    DataRow drEmp1 = ds1.Tables[2].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "10. Aggregate of deductible amount Under Chapter VIA",
                                        column2 = ReportColConvertToDecimal(drEmp1["aggregate_of_deductible"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "11. Total Income (8-10)",
                                        column2 = ReportColConvertToDecimal(drEmp1["total_income"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "12.Tax on Total Income",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_on_total_income"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "(b) Section 87A",
                                        column2 = ReportColConvertToDecimal(drEmp1["section_87a"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "14. Education CESS",
                                        column2 = ReportColConvertToDecimal(drEmp1["education_cess"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "15. Tax payable",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_payable"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "16.Less : a.Tax deducted at Source",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_deducted_at_source"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Less : b.Tax paid by the employer",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_paid_by_the_employer"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "17. Balance Tax (15-16)",
                                        column2 = ReportColConvertToDecimal(drEmp1["balance_tax"].ToString()),

                                    });

                                    
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "18. Balance Months",
                                        column2 = ReportColConvertToDecimal(drEmp1["balance_months"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "19. TDS Per Month",
                                        column2 = ReportColConvertToDecimal(drEmp1["tds_per_month"].ToString()),

                                    });
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                }
                            }
                        }
                        else
                        {

                            //str = Convert.ToDateTime(month);
                            //str1 = str.ToString("yyyy-MM-dd");
                            general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                        "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code,d.Description " +
                                        " as designation,gen.sex as gender " +
                                        "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                                        " JOIN Designations d ON d.Id=emp.CurrentDesignation " +
                                         "where gen.emp_code = " + Convert.ToInt32(empId) + " and active=1;";

                            daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                                "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                            tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                             " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                              " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                              "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                              " gross_salary, house_rent_allowance, total_of_sec10, " +
                                              " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                              "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                              " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                               " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                "balance_tax,balance_months,tds_per_month " +
                                               " from pr_emp_tds_process " +
                                               "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                                               "empcode =  " + Convert.ToInt32(empId) + " and active=1;";
                            section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                             "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                             "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                             "ef.type = 'per_ded' and epf.section_type = 'Section80C' and epf.gross>0 and ef.name not in('LIC','VPF','Provident Fund','Housing Loan Main','Housing Addl.Loan - 2D','GSLI') union all select 'Provident Fund' as name, gross , qual,ded " +
                             "from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 and gross>0 " +
                             " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2 and gross>0 " +
                             " union all select 'LIC' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=3 " +
                            "  union all select 'Housing Loan Main' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=4 and gross>0 " +
                             " union all select 'Housing Addl.Loan - 2D' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=5 and gross>0 " +
                             " union all select 'GSLI' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=6 and gross>0 " +
                             " union all select 'Housing Loan 2B-2C' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=7 and gross>0 " +
                             " union all select 'Housing Loan 2A' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=8 and gross>0;";

                            section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                  "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                  "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                                  "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                            section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                 "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                 "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + "and  " +
                                 "ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                            section4 = " select concat(ep.section, ' --> ', ef.name) as name,epf.section_type, epf.gross as gross,epf.qual as qual,epf.ded as ded  from pr_deduction_field_master ef" +
                                  " left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 and epf.empcode = " + Convert.ToInt32(empId) + "" +
                                   " join pr_emp_perdeductions ep on ef.id = ep.m_id and ep.active = 1  and ep.emp_code = " + Convert.ToInt32(empId) + "" +
                                    "WHERE Year(epf.fm)= " + Financial_md.Year + " and epf.fy = " + iFY + " and epf.section_type = 'Other' and epf.gross>0  and ep.amount>0 and ef.name !='Officers Assn Fund'";
                                    //" union all " +
                                    //" select concat('Section24(b)', ' --> ', ef.name) as name,epf.section_type, epf.gross as gross,epf.qual as qual,epf.ded as ded  from pr_deduction_field_master ef " +
                                    //" left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 and epf.empcode = " + Convert.ToInt32(empId) + " " +
                                    //"WHERE Year(epf.fm)= " + Financial_md.Year + " and epf.fy = " + iFY + " and epf.section_type = 'Other' and epf.gross > 0  and ef.name != 'Officers Assn Fund' " +
                                    //" and ef.name in((select name from pr_deduction_field_master where name like '%_interest') union all (select name from pr_deduction_field_master where name like '%_intrest') union all(select name from pr_deduction_field_master where name like '%_int_%')union all(select name from pr_deduction_field_master where name like '%_int'));";
                            allowances = "select all_name,all_amount from pr_emp_tds_process_allowances " +
                                "where Year(fm)=" + Financial_md.Year + " and Month(fm)=" + Financial_md.Month + " and all_name !='CCA' and fy=" + iFY + " and " +
                                "emp_code =  " + Convert.ToInt32(empId) + ";";
                            checkprocess = "select sal_basic " +
                                           " from pr_emp_tds_process " +
                                           "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                                           "empcode =  " + Convert.ToInt32(empId) + " and active=1 and sal_basic>0  ;";
                            DataSet ds = await _sha.Get_MultiTables_FromQry(general + daQry + tdsdetail + section1 + section2 + section3 + section4 + allowances + checkprocess);
                            DataTable dtALL = ds.Tables[0];
                            DataTable dtmon = ds.Tables[1];
                            DataTable dttds = ds.Tables[2];
                            DataTable sec1 = ds.Tables[3];
                            DataTable sec2 = ds.Tables[4];
                            DataTable sec3 = ds.Tables[5];
                            DataTable sec4 = ds.Tables[6];
                            DataTable allow = ds.Tables[7];
                            DataTable process = ds.Tables[8];
                            if (ds.Tables[8].Rows.Count > 0)
                            {

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drEmp = ds.Tables[0].Rows[0];



                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "H",
                                        //column1 = "<span style = 'background-color:#ADD8E6' >NAME OF THE EMPLOYEE: " + drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")" + "</span>",
                                        column1 = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColHeader(0, "Name and Address of the Employer", "  " + "TGCAB,Hyderabad" + "  ," + " " + "  " + "  " + " " + "Emp Name:" + " " + drEmp["name"].ToString()),
                                        column2 = "`",
                                        column3 = "`",
                                        column4 = "`",
                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "PAN NO:",
                                        column2 = ReportColFooterAlignleft(employer_pan),
                                        column3 = "Emp Code:",
                                        column4 = drEmp["emp_code"].ToString(),
                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",

                                        column1 = "TAN NO",
                                        column2 = ReportColFooterAlignleft("HYDT06401D"),
                                        column3 = "Designation:",
                                        column4 = drEmp["designation"].ToString(),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "Gender:",
                                        column4 = drEmp["gender"].ToString(),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "PAN No:",
                                        column4 = drEmp["pannumber"].ToString(),

                                    });
                                }
                                if (ds.Tables[1].Rows.Count > 0)
                                {


                                    DataRow drtime = ds.Tables[1].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "Month:",
                                        column4 = drtime["fm"].ToString(),

                                    });
                                }

                                if (ds.Tables[2].Rows.Count > 0)
                                {


                                    DataRow drtds = ds.Tables[2].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Basic",
                                        column2 = ReportColConvertToDecimal(drtds["sal_basic"].ToString()),

                                    });

                                    foreach (DataRow alws in allow.Rows)
                                    {
                                        //DataRow sect1 = ds.Tables[3].Rows[0];

                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = alws["all_name"].ToString(),
                                            column2 = ReportColConvertToDecimal(alws["all_amount"].ToString()),


                                        });

                                    }

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "DA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_da"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "HRA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_hra"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "CCA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_cca"].ToString()),
                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "INTERIM RELIEF",
                                        column2 = AddZerosAfterDecimal(drtds["sal_interim_relief"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Telangana Increment",
                                        column2 = AddZerosAfterDecimal(drtds["sal_telangana_increment"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spl. Allow",
                                        column2 = ReportColConvertToDecimal(drtds["sal_spl_allow"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spcl. DA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_spcl_da"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "PFPerks",
                                        column2 = ReportColConvertToDecimal(drtds["sal_pfperks"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "LOANPerks",
                                        column2 = ReportColConvertToDecimal(drtds["sal_loanperks"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Incentive",
                                        column2 = ReportColConvertToDecimal(drtds["sal_incentive"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Value of perquisites u/s 17(2)(as per Form.12BA)",
                                        column2 = ReportColConvertToDecimal(drtds["sal_value_of_perquisites"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Profits in lieu of salary u/s 17(3)(as per Form.12BA)",
                                        column2 = ReportColConvertToDecimal(drtds["sal_profits_in_lieu_of_salary"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Gross Salary",
                                        column2 = ReportColConvertToDecimal((drtds["gross_salary"].ToString())),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "2. Less : Allowance to the Extent exempt under Section 10",

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "House Rent Allowance",
                                        column2 = ReportColConvertToDecimal(drtds["house_rent_allowance"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Total of Section 10",
                                        column2 = ReportColConvertToDecimal(drtds["total_of_sec10"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "3. Balance(1-2)",
                                        column2 = ReportColConvertToDecimal((drtds["balance_gross_min_sec10"].ToString())),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "4. Deductions :",


                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "a. Standard Deduction",
                                        column2 = ReportColConvertToDecimal(drtds["standard_deductions"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "b. Tax on Employment",
                                        column2 = ReportColConvertToDecimal(drtds["tax_of_employement"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "5. Aggregate of 4(a) and (b)",
                                        column2 = ReportColConvertToDecimal(drtds["tds_aggregate"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "6. Income Chargeable Under the Head Salaries (3-5)",
                                        column2 = ReportColConvertToDecimal((drtds["income_chargeable_bal_minus_agg"].ToString())),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "7. Add : Any Other Income:",


                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Reported by the Employee",
                                        column2 = ReportColConvertToDecimal(drtds["other_income_by_the_emp"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Interest On Housing",
                                        column2 = ReportColConvertToDecimal(drtds["interest_on_housing"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "8. Gross Total Income",
                                        column2 = ReportColConvertToDecimal(drtds["gross_total_income"].ToString()),

                                    });
                                }


                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "9. Deductions Under Chapter VI-A",
                                    column2 = ReportColFooterAlignleft("Gross.Amt"),
                                    column3 = "Qual.Amt",
                                    column4 = "Ded.Amt",


                                });
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(A) Sections 80C,80CCC and 80CCD",



                                });
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(a) Section 80C",
                                });
                                if (ds.Tables[3].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect1 in sec1.Rows)
                                    {
                                        //DataRow sect1 = ds.Tables[3].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect1["name"].ToString(),
                                            column2 = ReportColConvertToDecimal(sect1["gross"].ToString()),
                                            column3 = ReportColConvertToDecimal(sect1["qual"].ToString()),
                                            column4 = ReportColConvertToDecimal(sect1["ded"].ToString()),

                                        });
                                    }
                                }

                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "b) Section 80CCC",
                                });
                                if (ds.Tables[4].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect2 in sec2.Rows)
                                    {
                                        //DataRow sect2 = ds.Tables[4].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect2["name"].ToString(),
                                            column2 = ReportColConvertToDecimal(sect2["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect2["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect2["ded"].ToString()),

                                        });
                                    }
                                }

                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(c) Section 80CCD",
                                });

                                if (ds.Tables[5].Rows.Count > 0)
                                {
                                    foreach (DataRow sect3 in sec3.Rows)
                                    {
                                        var count = 1;
                                        //DataRow sect3 = ds.Tables[5].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect3["name"].ToString(),
                                            column2 = ReportColConvertToDecimal(sect3["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect3["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect3["ded"].ToString()),

                                        });
                                    }
                                }
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(B) Other Sections Under Chaper VI-A",
                                });
                                if (ds.Tables[6].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect4 in sec4.Rows)
                                    {
                                        //DataRow sect4 = ds.Tables[6].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect4["name"].ToString(),
                                            column2 = ReportColConvertToDecimal(sect4["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect4["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect4["ded"].ToString()),

                                        });
                                    }
                                }
                                if (ds.Tables[2].Rows.Count > 0)
                                {


                                    DataRow drEmp1 = ds.Tables[2].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "10. Aggregate of deductible amount Under Chapter VIA",
                                        column2 = ReportColConvertToDecimal(drEmp1["aggregate_of_deductible"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "11. Total Income (8-10)",
                                        column2 = ReportColConvertToDecimal(drEmp1["total_income"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "12.Tax on Total Income",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_on_total_income"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "(b) Section 87A",
                                        column2 = ReportColConvertToDecimal(drEmp1["section_87a"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "14. Education CESS",
                                        column2 = ReportColConvertToDecimal(drEmp1["education_cess"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "15. Tax payable",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_payable"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "16.Less : a.Tax deducted at Source",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_deducted_at_source"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Less : b.Tax paid by the employer",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_paid_by_the_employer"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "17. Balance Tax (15-16)",
                                        column2 = ReportColConvertToDecimal(drEmp1["balance_tax"].ToString()),

                                    });

                                    
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "18. Balance Months",
                                        column2 = ReportColConvertToDecimal(drEmp1["balance_months"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "19. TDS Per Month",
                                        column2 = ReportColConvertToDecimal(drEmp1["tds_per_month"].ToString()),

                                    });
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                }
                            }
                        }

                    }


                }

                else
                {
                    string[] arrEmpId = EmpCode.Split(',');
                    foreach (string empId in arrEmpId)
                    {
                        //str = Convert.ToDateTime(month);
                        //str1 = str.ToString("yyyy-MM-dd");

                        general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                   "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code,d.Description " +
                                   " as designation,gen.sex as gender " +
                                   "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                                   " JOIN Designations d ON d.Id=emp.CurrentDesignation " +
                                    "where gen.emp_code = " + Convert.ToInt32(empId) + " and active=1;";

                        daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                                                   "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                        tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                         " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                          " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                          "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                          " gross_salary, house_rent_allowance, total_of_sec10, " +
                                          " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                          "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                          " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                           " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                            "balance_tax,balance_months,tds_per_month " +
                                           " from pr_emp_tds_process " +
                                           "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                                           "empcode =  " + Convert.ToInt32(empId) + " and active=1;";
                        section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                             "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                             "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                             "ef.type = 'per_ded' and epf.section_type = 'Section80C' and epf.gross>0 and ef.name not in('LIC','VPF','Provident Fund','Housing Loan Main','Housing Addl.Loan - 2D','GSLI') union all select 'Provident Fund' as name, gross , qual,ded " +
                             "from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 and gross>0 " +
                             " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2 and gross>0 " +
                             " union all select 'LIC' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=3 and gross>0 " +
                             " union all select 'Housing Loan Main' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=4 and gross>0 " +
                            " union all select 'Housing Addl.Loan - 2D' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=5 and gross>0 " +
                            " union all select 'GSLI' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=6 and gross>0 " +
                             " union all select 'Housing Loan 2B-2C' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=7 and gross>0 " +
                             " union all select 'Housing Loan 2A' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=8 and gross>0;";

                        section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                              "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                              "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                              "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                        section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                             "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                             "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                             "ef.type = 'per_ded' and epf.section_type = 'Section80CCD' ";
                             //union select  'NPS' as name, ef.NPS as gross, ef.NPS as qual,  ef.NPS as ded from pr_emp_payslip ef WHERE ef.emp_code = " + Convert.ToInt32(empId) + " and ef.active = 1 and Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " ";
                        section4 = " select concat(ep.section, ' --> ', ef.name) as name,epf.section_type, epf.gross as gross,epf.qual as qual,epf.ded as ded  from pr_deduction_field_master ef" +
                                  " left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 and epf.empcode = " + Convert.ToInt32(empId) + "" +
                                   " join pr_emp_perdeductions ep on ef.id = ep.m_id and ep.active = 1  and ep.emp_code = " + Convert.ToInt32(empId) + "" +
                                    "WHERE Year(epf.fm)= " + Financial_md.Year + " and epf.fy = " + iFY + " and epf.section_type = 'Other' and epf.gross>0  and ep.amount>0 and ef.name !='Officers Assn Fund'";
                                    //" union all " +
                                    //" select concat('Section24(b)', ' --> ', ef.name) as name,epf.section_type, epf.gross as gross,epf.qual as qual,epf.ded as ded  from pr_deduction_field_master ef " +
                                    //" left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 and epf.empcode = " + Convert.ToInt32(empId) + " "+
                                    //"WHERE Year(epf.fm)= " + Financial_md.Year + " and epf.fy = "+ iFY + " and epf.section_type = 'Other' and epf.gross > 0  and ef.name != 'Officers Assn Fund' " +
                                    //" and ef.name in((select name from pr_deduction_field_master where name like '%_interest') union all (select name from pr_deduction_field_master where name like '%_intrest') union all(select name from pr_deduction_field_master where name like '%_int_%')union all(select name from pr_deduction_field_master where name like '%_int'));";

                        allowances = "select all_name,all_amount from pr_emp_tds_process_allowances " +
                               "where Year(fm)=" + Financial_md.Year + " and Month(fm)=" + Financial_md.Month + " and all_name !='CCA' and fy=" + iFY + " and " +
                               "emp_code =  " + Convert.ToInt32(empId) + ";";
                        checkprocess = "select sal_basic " +
                                           " from pr_emp_tds_process " +
                                           "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                                           "empcode =  " + Convert.ToInt32(empId) + " and active=1 and sal_basic>0 ;";
                        DataSet ds = await _sha.Get_MultiTables_FromQry(general + daQry + tdsdetail + section1 + section2 + section3 + section4 + allowances + checkprocess);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtmon = ds.Tables[1];
                        DataTable dttds = ds.Tables[2];
                        DataTable sec1 = ds.Tables[3];
                        DataTable sec2 = ds.Tables[4];
                        DataTable sec3 = ds.Tables[5];
                        DataTable sec4 = ds.Tables[6];
                        DataTable allow = ds.Tables[7];
                        DataTable process = ds.Tables[8];
                        if (ds.Tables[8].Rows.Count > 0)
                        {

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DataRow drEmp = ds.Tables[0].Rows[0];



                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "H",
                                    //column1 = "<span style = 'background-color:#ADD8E6' >NAME OF THE EMPLOYEE: " + drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")" + "</span>",
                                    column1 = "<span style='color:#C8EAFB'>~</span>"
                            + ReportColHeader(0, "Name and Address of the Employer", "  " + "TGCAB,Hyderabad" + "  ," + " " + "  " + "  " + " " + "Emp Name:" + " " + drEmp["name"].ToString()),
                                    column2 = "`",
                                    column3 = "`",
                                    column4 = "`",
                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "PAN NO:",
                                    column2 = ReportColFooterAlignleft(employer_pan),
                                    column3 = "Emp Code:",
                                    column4 = drEmp["emp_code"].ToString(),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",

                                    column1 = "TAN NO",
                                    column2 = ReportColFooterAlignleft("HYDT06401D"),
                                    column3 = "Designation:",
                                    column4 = drEmp["designation"].ToString(),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column3 = "Gender:",
                                    column4 = drEmp["gender"].ToString(),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column3 = "PAN No:",
                                    column4 = drEmp["pannumber"].ToString(),

                                });
                            }
                            if (ds.Tables[1].Rows.Count > 0)
                            {


                                DataRow drtime = ds.Tables[1].Rows[0];

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column3 = "Month:",
                                    column4 = drtime["fm"].ToString(),

                                });
                            }

                            if (ds.Tables[2].Rows.Count > 0)
                            {


                                DataRow drtds = ds.Tables[2].Rows[0];

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Basic",
                                    column2 = ReportColConvertToDecimal(drtds["sal_basic"].ToString()),

                                });

                                foreach (DataRow alws in allow.Rows)
                                {
                                    //DataRow sect1 = ds.Tables[3].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = alws["all_name"].ToString(),
                                        column2 = ReportColConvertToDecimal(alws["all_amount"].ToString()),


                                    });

                                }

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "DA",
                                    column2 = ReportColConvertToDecimal(drtds["sal_da"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "HRA",
                                    column2 = ReportColConvertToDecimal(drtds["sal_hra"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "CCA",
                                    column2 = ReportColConvertToDecimal(drtds["sal_cca"].ToString()),
                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "INTERIM RELIEF",
                                    column2 = ReportColConvertToDecimal(drtds["sal_interim_relief"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Telangana Increment",
                                    column2 = ReportColConvertToDecimal(drtds["sal_telangana_increment"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Spl. Allow",
                                    column2 = ReportColConvertToDecimal(drtds["sal_spl_allow"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Spcl. DA",
                                    column2 = ReportColConvertToDecimal(drtds["sal_spcl_da"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "PFPerks",
                                    column2 = ReportColConvertToDecimal(drtds["sal_pfperks"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "LOANPerks",
                                    column2 = ReportColConvertToDecimal(drtds["sal_loanperks"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Incentive",
                                    column2 = ReportColConvertToDecimal(drtds["sal_incentive"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Value of perquisites u/s 17(2)(as per Form.12BA)",
                                    column2 = ReportColConvertToDecimal(drtds["sal_value_of_perquisites"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Profits in lieu of salary u/s 17(3)(as per Form.12BA)",
                                    column2 = ReportColConvertToDecimal(drtds["sal_profits_in_lieu_of_salary"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Gross Salary",
                                    column2 = ReportColConvertToDecimal((drtds["gross_salary"].ToString())),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "2. Less : Allowance to the Extent exempt under Section 10",

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "House Rent Allowance",
                                    column2 = ReportColConvertToDecimal(drtds["house_rent_allowance"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Total of Section 10",
                                    column2 = ReportColConvertToDecimal(drtds["total_of_sec10"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "3. Balance(1-2)",
                                    column2 = ReportColConvertToDecimal((drtds["balance_gross_min_sec10"].ToString())),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "4. Deductions :",


                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "a. Standard Deduction",
                                    column2 = ReportColConvertToDecimal(drtds["standard_deductions"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "b. Tax on Employment",
                                    column2 = ReportColConvertToDecimal(drtds["tax_of_employement"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "5. Aggregate of 4(a) and (b)",
                                    column2 = ReportColConvertToDecimal(drtds["tds_aggregate"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "6. Income Chargeable Under the Head Salaries (3-5)",
                                    column2 = ReportColConvertToDecimal(drtds["income_chargeable_bal_minus_agg"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "7. Add : Any Other Income:",


                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Reported by the Employee",
                                    column2 = ReportColConvertToDecimal(drtds["other_income_by_the_emp"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Interest On Housing",
                                    column2 = ReportColConvertToDecimal(drtds["interest_on_housing"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "8. Gross Total Income",
                                    column2 = ReportColConvertToDecimal((drtds["gross_total_income"].ToString())),

                                });
                            }


                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "9. Deductions Under Chapter VI-A",
                                column2 = ReportColFooterAlignleft("Gross.Amt"),
                                column3 = "Qual.Amt",
                                column4 = "Ded.Amt",


                            });
                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "(A) Sections 80C,80CCC and 80CCD",



                            });
                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "(a) Section 80C",
                            });
                            if (sec1.Rows.Count > 0)
                            {
                                var count = 1;
                                foreach (DataRow sect1 in sec1.Rows)
                                {
                                    //DataRow sect1 = ds.Tables[3].Rows[0];
                                    int id = count++;
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "" + id + ". " + sect1["name"].ToString(),
                                        column2 = ReportColConvertToDecimal(sect1["gross"].ToString()),
                                        column3 = ReportColFooterAlign(sect1["qual"].ToString()),
                                        column4 = ReportColFooterAlign(sect1["ded"].ToString()),

                                    });
                                }
                            }

                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "b) Section 80CCC",
                            });
                            if (ds.Tables[4].Rows.Count > 0)
                            {
                                var count = 1;
                                foreach (DataRow sect2 in sec2.Rows)
                                {
                                    //DataRow sect2 = ds.Tables[4].Rows[0];
                                    int id = count++;
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "" + id + ". " + sect2["name"].ToString(),
                                        column2 = ReportColConvertToDecimal(sect2["gross"].ToString()),
                                        column3 = ReportColFooterAlign(sect2["qual"].ToString()),
                                        column4 = ReportColFooterAlign(sect2["ded"].ToString()),

                                    });
                                }
                            }

                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "(c) Section 80CCD",
                            });

                            if (ds.Tables[5].Rows.Count > 0)
                            {
                                foreach (DataRow sect3 in sec3.Rows)
                                {
                                    var count = 1;
                                    //DataRow sect3 = ds.Tables[5].Rows[0];
                                    int id = count++;
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "" + id + ". " + sect3["name"].ToString(),
                                        column2 = ReportColConvertToDecimal(sect3["gross"].ToString()),
                                        column3 = ReportColFooterAlign(sect3["qual"].ToString()),
                                        column4 = ReportColFooterAlign(sect3["ded"].ToString()),

                                    });
                                }
                            }
                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "(B) Other Sections Under Chaper VI-A",
                            });
                            if (ds.Tables[6].Rows.Count > 0)
                            {
                                var count = 1;
                                foreach (DataRow sect4 in sec4.Rows)
                                {
                                    //DataRow sect4 = ds.Tables[6].Rows[0];
                                    int id = count++;
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "" + id + ". " + sect4["name"].ToString(),
                                        column2 = ReportColConvertToDecimal(sect4["gross"].ToString()),
                                        column3 = ReportColFooterAlign(sect4["qual"].ToString()),
                                        column4 = ReportColFooterAlign(sect4["ded"].ToString()),

                                    });
                                }
                            }
                            if (ds.Tables[2].Rows.Count > 0)
                            {


                                DataRow drEmp1 = ds.Tables[2].Rows[0];

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "10. Aggregate of deductible amount Under Chapter VIA",
                                    column2 = ReportColConvertToDecimal(drEmp1["aggregate_of_deductible"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "11. Total Income (8-10)",
                                    column2 = ReportColConvertToDecimal(drEmp1["total_income"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "12.Tax on Total Income",
                                    column2 = ReportColConvertToDecimal(drEmp1["tax_on_total_income"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(b) Section 87A",
                                    column2 = ReportColConvertToDecimal(drEmp1["section_87a"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "14. Education CESS",
                                    column2 = ReportColConvertToDecimal(drEmp1["education_cess"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "15. Tax payable",
                                    column2 = ReportColConvertToDecimal(drEmp1["tax_payable"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "16.Less : a.Tax deducted at Source",
                                    column2 = ReportColConvertToDecimal(drEmp1["tax_deducted_at_source"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Less : b.Tax paid by the employer",
                                    column2 = ReportColConvertToDecimal(drEmp1["tax_paid_by_the_employer"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "17. Balance Tax (15-16)",
                                    column2 = ReportColConvertToDecimal(drEmp1["balance_tax"].ToString()),

                                });
                                
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "18. Balance Months",
                                    column2 = drEmp1["balance_months"].ToString(),

                                });
                                
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "19. TDS Per Month",
                                    column2 = ReportColConvertToDecimal(drEmp1["tds_per_month"].ToString()),

                                });
                                //lst.Add(new CommonReportModel
                                //{
                                //    RowId = rowid++,
                                //    HRF = "R",
                                //    column1 = "",
                                //    column2 = "",

                                //});
                                //lst.Add(new CommonReportModel
                                //{
                                //    RowId = rowid++,
                                //    HRF = "R",
                                //    column1 = "",
                                //    column2 = "",

                                //});
                                //lst.Add(new CommonReportModel
                                //{
                                //    RowId = rowid++,
                                //    HRF = "R",
                                //    column1 = "",
                                //    column2 = "",

                                //});
                            }
                        }
                    }
                }
                return lst;

            }
            catch (Exception e)
            {

            }
            return lst;
        }

        public async Task<IList<CommonReportModel>> getTdsDetailshrms(string EmpCode)
        {
            string employer_pan = PrConstants.PAN;
            string qryfm = "select fy, fm from pr_month_details where active = 1";
            DataTable fmdt = sh.Get_Table_FromQry(qryfm);
            int iFY = Convert.ToInt32(fmdt.Rows[0]["fy"]);
            int dtFM = _LoginCredential.FM;
            DateTime Financial_md = (_LoginCredential.FinancialMonthDate);
            IList<CommonReportModel> lst = new List<CommonReportModel>();
            string general = "";
            string tdsdetail = "";
            string daQry = "";
            DateTime str;
            string str1;
            string section1 = "";
            string section2 = "";
            string section3 = "";
            string section4 = "";
            string allowances = "";
            string checkprocess = " ";
            int rowid = 0;
            try
            {

                // CommonReportModel crm = new TDSReport();

                if (EmpCode == "^2")
                {
                    EmpCode = "0";
                }

                if (EmpCode.Contains("^"))
                {
                    EmpCode = "0";


                }
                if (EmpCode == "All")
                {
                    //str = Convert.ToDateTime(month);
                    //str1 = str.ToString("yyyy-MM-dd");
                    string emp_codes = "0";
                    string qrySel = "SELECT empcode " +
                                     "FROM pr_emp_tds_process " +

                                       " where  Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and active=1 and sal_basic>0;";
                    DataTable dt = await _sha.Get_Table_FromQry(qrySel);


                    foreach (DataRow dr in dt.Rows)
                    {

                        //string str;
                        emp_codes += dr["empcode"] + "," ;

                    }
                    emp_codes = emp_codes.Substring(0, emp_codes.Length - 1);
                    string[] arrEmpId = emp_codes.Split(',');
                    foreach (string empId in arrEmpId)
                    {
                        if (empId == " ")
                        {

                            //str = Convert.ToDateTime(month);
                            //str1 = str.ToString("yyyy-MM-dd");

                            general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                        "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code,d.Description " +
                                        " as designation,gen.sex as gender " +
                                        "from pr_emp_general gen left outer " +
                                        "join Employees emp on gen.emp_code = emp.EmpId " +
                                        " JOIN Designations d ON d.Id=emp.CurrentDesignation " +
                                         "where gen.emp_code = '0' ;";

                            daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                             "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";

                            tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                             " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                              " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                              "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                              " gross_salary, house_rent_allowance, total_of_sec10, " +
                                              " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                              "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                              " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                               " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                "balance_tax,balance_months,tds_per_month " +
                                               " from pr_emp_tds_process " +
                                               "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                                               "empcode =  '0';";
                            section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                             "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                             "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                             "ef.type = 'per_ded' and epf.section_type = 'Section80C' and ef.name not in('LIC','VPF','Provident Fund','Housing Loan Main','Housing Addl.Loan - 2D','GSLI') union all select 'Provident Fund' as name, gross , qual,ded " +
                             "from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 " +
                             " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2 " +
                             " union all select 'LIC' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=3 " +
                             " union all select 'Housing Loan Main' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=4 and gross>0 " +
                            " union all select 'Housing Addl.Loan - 2D' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=5 and gross>0 " +
                            " union all select 'GSLI' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=6 and gross>0 " +
                             " union all select 'Housing Loan 2B-2C' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=7 and gross>0 " +
                             " union all select 'Housing Loan 2A' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=8 and gross>0;";

                            section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                  "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                  "and epf.empcode = '0' WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                                  "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                            section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                 "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                 "and epf.empcode = '0' WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                                 "ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                            section4 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.section as section,epf.ded as ded  " +
                                 "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                 "and epf.empcode = '0' WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                                 "ef.type = 'per_ded' and epf.section_type = 'Other' ";
                            DataSet ds1 = await _sha.Get_MultiTables_FromQry(general + daQry + tdsdetail + section1 + section2 + section3 + section4);
                            DataTable dtALL1 = ds1.Tables[0];
                            DataTable dtmon1 = ds1.Tables[1];
                            DataTable dttds1 = ds1.Tables[2];
                            DataTable sec11 = ds1.Tables[3];
                            DataTable sec21 = ds1.Tables[4];
                            DataTable sec31 = ds1.Tables[5];
                            DataTable sec41 = ds1.Tables[6];
                            if (ds1.Tables[2].Rows.Count > 0)
                            {
                                if (ds1.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drEmp = ds1.Tables[0].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "H",
                                        //column1 = "<span style = 'background-color:#ADD8E6' >NAME OF THE EMPLOYEE: " + drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")" + "</span>",
                                        column1 = "<span style='color:#C8EAFB'>~</span>"
                             + ReportColHeader(0, "Name and Address of the Employer", "" + drEmp["name"].ToString() + ", TGCAB,Hyderabad"),
                                        column2 = "`",
                                        column3 = "`",
                                        column4 = "`",
                                    });

                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "<span style = 'background-color:#ADD8E6' >Name and Address of the Employer:</span>",
                                    //    column2 = "<span style = 'background-color:#ADD8E6' >TSCAB,Hyderabad</span>",

                                    //    column3 = "<span style = 'background-color:#ADD8E6' >Name:</span>",
                                    //    column4 = "<span style = 'background-color:#ADD8E6' >" + drEmp["name"].ToString() + "</span>",

                                    //});
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "PAN NO:",
                                        column2 = ReportColFooterAlignleft(drEmp["pannumber"].ToString()),
                                        column3 = "Emp Code:",
                                        column4 = drEmp["emp_code"].ToString(),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",

                                        column1 = "TAN NO",
                                        column2 = ReportColFooterAlignleft("HYDT06401D"),
                                        column3 = "Designation:",
                                        column4 = drEmp["designation"].ToString(),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "Gender:",
                                        column4 = drEmp["gender"].ToString(),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "PAN No:",
                                        column4 = drEmp["pannumber"].ToString(),

                                    });
                                }
                                if (ds1.Tables[1].Rows.Count > 0)
                                {


                                    DataRow drtime = ds1.Tables[1].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "Month:",
                                        column4 = drtime["fm"].ToString(),

                                    });
                                }
                                if (ds1.Tables[2].Rows.Count > 0)
                                {


                                    DataRow drtds = ds1.Tables[2].Rows[0];
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Basic",
                                        column4 = ReportColConvertToDecimal(drtds["sal_basic"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Fixed Personal Allowance",
                                        column4 = ReportColConvertToDecimal(drtds["sal_fixed_personal_allowance"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "FPA-HRA Allowance",
                                        column4 = ReportColConvertToDecimal(drtds["sal_fpa_hra_allowance"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "FPIIP",
                                        column4 = ReportColConvertToDecimal(drtds["sal_fpiip"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "DA",
                                        column4 = ReportColConvertToDecimal(drtds["sal_da"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "HRA",
                                        column4 = ReportColConvertToDecimal(drtds["sal_hra"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "CCA",
                                        column4 = ReportColConvertToDecimal(drtds["sal_cca"].ToString()),
                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "INTERIM RELIEF",
                                        column4 = ReportColConvertToDecimal(drtds["sal_interim_relief"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Telangana Increment",
                                        column4 = ReportColConvertToDecimal(drtds["sal_telangana_increment"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spl. Allow",
                                        column4 = ReportColConvertToDecimal(drtds["sal_spl_allow"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spcl. DA",
                                        column4 = ReportColConvertToDecimal(drtds["sal_spcl_da"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "PFPerks",
                                        column4 = ReportColConvertToDecimal(drtds["sal_pfperks"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "LOANPerks",
                                        column4 = ReportColConvertToDecimal(drtds["sal_loanperks"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Incentive",
                                        column4 = ReportColConvertToDecimal(drtds["sal_incentive"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Value of perquisites u/s 17(2)(as per Form.12BA)",
                                        column4 = ReportColConvertToDecimal(drtds["sal_value_of_perquisites"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Profits in lieu of salary u/s 17(3)(as per Form.12BA)",
                                        column4 = ReportColConvertToDecimal(drtds["sal_profits_in_lieu_of_salary"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Gross Salary",
                                        column4 = ReportColConvertToDecimal(drtds["gross_salary"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "2. Less : Allowance to the Extent exempt under Section 10",

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "House Rent Allowance",
                                        column4 = ReportColConvertToDecimal(drtds["house_rent_allowance"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Total of Section 10",
                                        column4 = ReportColConvertToDecimal(drtds["total_of_sec10"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "3. Balance(1-2)",
                                        column4 = ReportColConvertToDecimal(drtds["balance_gross_min_sec10"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "4. Deductions :",


                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "a. Standard Deduction",
                                        column4 = ReportColConvertToDecimal(drtds["standard_deductions"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "b. Tax on Employment",
                                        column4 = ReportColConvertToDecimal(drtds["tax_of_employement"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "5. Aggregate of 4(a) and (b)",
                                        column4 = ReportColConvertToDecimal(drtds["tds_aggregate"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "6. Income Chargeable Under the Head Salaries (3-5)",
                                        column4 = ReportColConvertToDecimal(drtds["income_chargeable_bal_minus_agg"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "7. Add : Any Other Income:",


                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Reported by the Employee",
                                        column4 = ReportColConvertToDecimal(drtds["other_income_by_the_emp"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Interest On Housing",
                                        column4 = ReportColConvertToDecimal(drtds["interest_on_housing"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "8. Gross Total Income",
                                        column4 = ReportColConvertToDecimal(drtds["gross_total_income"].ToString()),

                                    });
                                }


                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "9. Deductions Under Chapter VI-A",
                                    column2 = ReportColFooterAlignleft("Gross.Amt"),
                                    column3 = "Qual.Amt",
                                    column4 = "Ded.Amt",


                                });
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(A) Sections 80C,80CCC and 80CCD",



                                });
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(a) Section 80C",
                                });
                                if (sec11.Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect1 in sec11.Rows)
                                    {
                                        //DataRow sect1 = ds.Tables[3].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {

                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect1["name"].ToString(),
                                            column2 = ReportColFooterAlign(sect1["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect1["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect1["ded"].ToString()),

                                        }); ;
                                    }
                                }

                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "b) Section 80CCC",
                                });
                                if (ds1.Tables[4].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect2 in sec21.Rows)
                                    {
                                        int id = count++;
                                        //DataRow sect2 = ds.Tables[4].Rows[0];
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect2["name"].ToString(),
                                            column2 = ReportColFooterAlign(sect2["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect2["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect2["ded"].ToString()),

                                        });
                                    }
                                }

                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(c) Section 80CCD",
                                });

                                if (ds1.Tables[5].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect3 in sec31.Rows)
                                    {
                                        //DataRow sect3 = ds.Tables[5].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect3["name"].ToString(),
                                            column2 = ReportColFooterAlign(sect3["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect3["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect3["ded"].ToString()),

                                        });
                                    }
                                }
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(B) Other Sections Under Chaper VI-A",
                                });
                                if (ds1.Tables[6].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect4 in sec41.Rows)
                                    {
                                        //DataRow sect4 = ds.Tables[6].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + "" + sect4["section"].ToString() + ",   " + sect4["name"].ToString(),
                                            column2 = ReportColFooterAlign(sect4["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect4["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect4["ded"].ToString()),

                                        });
                                    }
                                }
                                if (ds1.Tables[2].Rows.Count > 0)
                                {


                                    DataRow drEmp1 = ds1.Tables[2].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "10. Aggregate of deductible amount Under Chapter VIA",
                                        column2 = ReportColConvertToDecimal(drEmp1["aggregate_of_deductible"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "11. Total Income (8-10)",
                                        column2 = ReportColConvertToDecimal(drEmp1["total_income"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "12.Tax on Total Income",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_on_total_income"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "(b) Section 87A",
                                        column2 = ReportColConvertToDecimal(drEmp1["section_87a"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "14. Education CESS",
                                        column2 = ReportColConvertToDecimal(drEmp1["education_cess"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "15. Tax payable",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_payable"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "16.Less : a.Tax deducted at Source",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_deducted_at_source"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Less : b.Tax paid by the employer",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_paid_by_the_employer"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "17. Balance Tax (15-16)",
                                        column2 = ReportColConvertToDecimal(drEmp1["balance_tax"].ToString()),

                                    });
                                    
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "18. Balance Months",
                                        column2 =  drEmp1["balance_months"].ToString(),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "19. TDS Per Month",
                                        column2 = ReportColConvertToDecimal(drEmp1["tds_per_month"].ToString()),

                                    });
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                }
                            }
                        }
                        else
                        {

                            //str = Convert.ToDateTime(month);
                            //str1 = str.ToString("yyyy-MM-dd");
                            general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                        "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code,d.Description " +
                                        " as designation,gen.sex as gender " +
                                        "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                                        " JOIN Designations d ON d.Id=emp.CurrentDesignation " +
                                         "where gen.emp_code = " + Convert.ToInt32(empId) + " and active=1;";

                            daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                              "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                            tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                             " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                              " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                              "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                              " gross_salary, house_rent_allowance, total_of_sec10, " +
                                              " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                              "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                              " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                               " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                "balance_tax,balance_months,tds_per_month " +
                                               " from pr_emp_tds_process " +
                                               "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                                               "empcode =  " + Convert.ToInt32(empId) + " and active=1;";
                            section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                             "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                             "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                             "ef.type = 'per_ded' and epf.section_type = 'Section80C' and epf.gross>0 and ef.name not in('LIC','VPF','Provident Fund','Housing Loan Main','Housing Addl.Loan - 2D','GSLI') union all select 'Provident Fund' as name, gross , qual,ded " +
                             "from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 and gross>0 " +
                             " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2 and gross>0 " +
                             " union all select 'LIC' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=3 " +
                             " union all select 'Housing Loan Main' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=4 and gross>0 " +
                            " union all select 'Housing Addl.Loan - 2D' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=5 and gross>0 " +
                            " union all select 'GSLI' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=6 and gross>0 " +
                             " union all select 'Housing Loan 2B-2C' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=7 and gross>0 " +
                             " union all select 'Housing Loan 2A' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=8 and gross>0;";

                            section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                  "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                  "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                                  "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                            section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                 "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                 "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + "and  " +
                                 "ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                            section4 = " select concat(ep.section, ' --> ', ef.name) as name,epf.section_type, epf.gross as gross,epf.qual as qual,epf.ded as ded  from pr_deduction_field_master ef" +
                                  " left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 and epf.empcode = " + Convert.ToInt32(empId) + "" +
                                   " join pr_emp_perdeductions ep on ef.id = ep.m_id and ep.active = 1  and ep.emp_code = " + Convert.ToInt32(empId) + "" +
                                    "WHERE Year(epf.fm)= " + Financial_md.Year + " and epf.fy = " + iFY + " and epf.section_type = 'Other' and epf.gross>0  and ep.amount>0  and ef.name !='Officers Assn Fund'";
                                    //" union all " +
                                    //" select concat('Section24(b)', ' --> ', ef.name) as name,epf.section_type, epf.gross as gross,epf.qual as qual,epf.ded as ded  from pr_deduction_field_master ef " +
                                    //" left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 and epf.empcode = " + Convert.ToInt32(empId) + " " +
                                    //"WHERE Year(epf.fm)= " + Financial_md.Year + " and epf.fy = " + iFY + " and epf.section_type = 'Other' and epf.gross > 0  and ef.name != 'Officers Assn Fund' " +
                                    //" and ef.name in(select name from pr_deduction_field_master where name like '%_interest');";
                            allowances = "select all_name,all_amount from pr_emp_tds_process_allowances " +
                                "where Year(fm)=" + Financial_md.Year + "  and fy=" + iFY + " and " +
                                "emp_code =  " + Convert.ToInt32(empId) + ";";
                            checkprocess = "select sal_basic " +
                                           " from pr_emp_tds_process " +
                                           "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                                           "empcode =  " + Convert.ToInt32(empId) + " and active=1 and sal_basic>0  ;";
                            DataSet ds = await _sha.Get_MultiTables_FromQry(general + daQry + tdsdetail + section1 + section2 + section3 + section4 + allowances + checkprocess);
                            DataTable dtALL = ds.Tables[0];
                            DataTable dtmon = ds.Tables[1];
                            DataTable dttds = ds.Tables[2];
                            DataTable sec1 = ds.Tables[3];
                            DataTable sec2 = ds.Tables[4];
                            DataTable sec3 = ds.Tables[5];
                            DataTable sec4 = ds.Tables[6];
                            DataTable allow = ds.Tables[7];
                            DataTable process = ds.Tables[8];
                            if (ds.Tables[8].Rows.Count > 0)
                            {

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drEmp = ds.Tables[0].Rows[0];



                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "H",
                                        //column1 = "<span style = 'background-color:#ADD8E6' >NAME OF THE EMPLOYEE: " + drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")" + "</span>",
                                        column1 = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColHeader(0, "Name and Address of the Employer", "  " + "TGCAB,Hyderabad" + "  ," + " " + "  " + "  " + " " + "Emp Name:" + " " + drEmp["name"].ToString()),
                                        column2 = "`",
                                        column3 = "`",
                                        column4 = "`",
                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "PAN NO:",
                                        column2 = ReportColFooterAlignleft(employer_pan),
                                        column3 = "Emp Code:",
                                        column4 = drEmp["emp_code"].ToString(),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",

                                        column1 = "TAN NO",
                                        column2 = ReportColFooterAlignleft("HYDT06401D"),
                                        column3 = "Designation:",
                                        column4 = drEmp["designation"].ToString(),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "Gender:",
                                        column4 = drEmp["gender"].ToString(),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "PAN No:",
                                        column4 = drEmp["pannumber"].ToString(),

                                    });
                                }
                                if (ds.Tables[1].Rows.Count > 0)
                                {


                                    DataRow drtime = ds.Tables[1].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column3 = "Month:",
                                        column4 = drtime["fm"].ToString(),

                                    });
                                }

                                if (ds.Tables[2].Rows.Count > 0)
                                {


                                    DataRow drtds = ds.Tables[2].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Basic",
                                        column2 = ReportColConvertToDecimal(drtds["sal_basic"].ToString()),

                                    });

                                    foreach (DataRow alws in allow.Rows)
                                    {
                                        //DataRow sect1 = ds.Tables[3].Rows[0];

                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = alws["all_name"].ToString(),
                                            column2 = ReportColConvertToDecimal(alws["all_amount"].ToString()),


                                        });

                                    }

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "DA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_da"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "HRA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_hra"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "CCA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_cca"].ToString()),
                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "INTERIM RELIEF",
                                        column2 = ReportColConvertToDecimal(drtds["sal_interim_relief"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Telangana Increment",
                                        column2 = ReportColConvertToDecimal(drtds["sal_telangana_increment"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spl. Allow",
                                        column2 = ReportColConvertToDecimal(drtds["sal_spl_allow"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spcl. DA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_spcl_da"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "PFPerks",
                                        column2 = ReportColConvertToDecimal(drtds["sal_pfperks"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "LOANPerks",
                                        column2 = ReportColConvertToDecimal(drtds["sal_loanperks"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Incentive",
                                        column2 = ReportColConvertToDecimal(drtds["sal_incentive"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Value of perquisites u/s 17(2)(as per Form.12BA)",
                                        column2 = ReportColConvertToDecimal(drtds["sal_value_of_perquisites"].ToString()),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Profits in lieu of salary u/s 17(3)(as per Form.12BA)",
                                        column2 = ReportColConvertToDecimal(drtds["sal_profits_in_lieu_of_salary"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Gross Salary",
                                        column2 = ReportColConvertToDecimal((drtds["gross_salary"].ToString())),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "2. Less : Allowance to the Extent exempt under Section 10",

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "House Rent Allowance",
                                        column2 = ReportColConvertToDecimal(drtds["house_rent_allowance"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Total of Section 10",
                                        column2 = ReportColConvertToDecimal(drtds["total_of_sec10"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "3. Balance(1-2)",
                                        column2 = ReportColConvertToDecimal((drtds["balance_gross_min_sec10"].ToString())),

                                    });


                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "4. Deductions :",


                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "a. Standard Deduction",
                                        column2 = ReportColConvertToDecimal(drtds["standard_deductions"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "b. Tax on Employment",
                                        column2 = ReportColConvertToDecimal(drtds["tax_of_employement"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "5. Aggregate of 4(a) and (b)",
                                        column2 = ReportColConvertToDecimal(drtds["tds_aggregate"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "6. Income Chargeable Under the Head Salaries (3-5)",
                                        column2 = ReportColConvertToDecimal((drtds["income_chargeable_bal_minus_agg"].ToString())),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "7. Add : Any Other Income:",


                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Reported by the Employee",
                                        column2 = ReportColConvertToDecimal(drtds["other_income_by_the_emp"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Interest On Housing",
                                        column2 = ReportColConvertToDecimal(drtds["interest_on_housing"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "8. Gross Total Income",
                                        column2 = ReportColConvertToDecimal((drtds["gross_total_income"].ToString())),

                                    });
                                }


                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "9. Deductions Under Chapter VI-A",
                                    column2 = ReportColFooterAlignleft("Gross.Amt"),
                                    column3 = "Qual.Amt",
                                    column4 = "Ded.Amt",


                                });
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(A) Sections 80C,80CCC and 80CCD",



                                });
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(a) Section 80C",
                                });
                                if (sec1.Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect1 in sec1.Rows)
                                    {
                                        //DataRow sect1 = ds.Tables[3].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect1["name"].ToString(),
                                            column2 = ReportColConvertToDecimal(sect1["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect1["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect1["ded"].ToString()),

                                        });
                                    }
                                }

                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "b) Section 80CCC",
                                });
                                if (ds.Tables[4].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect2 in sec2.Rows)
                                    {
                                        //DataRow sect2 = ds.Tables[4].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect2["name"].ToString(),
                                            column2 = ReportColConvertToDecimal(sect2["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect2["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect2["ded"].ToString()),

                                        });
                                    }
                                }

                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(c) Section 80CCD",
                                });

                                if (ds.Tables[5].Rows.Count > 0)
                                {
                                    foreach (DataRow sect3 in sec3.Rows)
                                    {
                                        var count = 1;
                                        //DataRow sect3 = ds.Tables[5].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect3["name"].ToString(),
                                            column2 = ReportColConvertToDecimal(sect3["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect3["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect3["ded"].ToString()),

                                        });
                                    }
                                }
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(B) Other Sections Under Chaper VI-A",
                                });
                                if (ds.Tables[6].Rows.Count > 0)
                                {
                                    var count = 1;
                                    foreach (DataRow sect4 in sec4.Rows)
                                    {
                                        //DataRow sect4 = ds.Tables[6].Rows[0];
                                        int id = count++;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "" + id + ". " + sect4["name"].ToString(),
                                            column2 = ReportColConvertToDecimal(sect4["gross"].ToString()),
                                            column3 = ReportColFooterAlign(sect4["qual"].ToString()),
                                            column4 = ReportColFooterAlign(sect4["ded"].ToString()),

                                        });
                                    }
                                }
                                if (ds.Tables[2].Rows.Count > 0)
                                {


                                    DataRow drEmp1 = ds.Tables[2].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "10. Aggregate of deductible amount Under Chapter VIA",
                                        column2 = AddZerosAfterDecimal(drEmp1["aggregate_of_deductible"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "11. Total Income (8-10)",
                                        column2 = ReportColConvertToDecimal(drEmp1["total_income"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "12.Tax on Total Income",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_on_total_income"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "(b) Section 87A",
                                        column2 = ReportColConvertToDecimal(drEmp1["section_87a"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "14. Education CESS",
                                        column2 = ReportColConvertToDecimal(drEmp1["education_cess"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "15. Tax payable",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_payable"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "16.Less : a.Tax deducted at Source",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_deducted_at_source"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Less : b.Tax paid by the employer",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_paid_by_the_employer"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "17. Balance Tax (15-16)",
                                        column2 = ReportColConvertToDecimal(drEmp1["balance_tax"].ToString()),

                                    });
                                    
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "18. Balance Months",
                                        column2 =  drEmp1["balance_months"].ToString(),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "19. TDS Per Month",
                                        column2 = ReportColConvertToDecimal(drEmp1["tds_per_month"].ToString()),

                                    });
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                    //lst.Add(new CommonReportModel
                                    //{
                                    //    RowId = rowid++,
                                    //    HRF = "R",
                                    //    column1 = "",
                                    //    column2 = "",

                                    //});
                                }
                            }
                        }

                    }


                }

                else
                {
                    string[] arrEmpId = EmpCode.Split(',');
                    foreach (string empId in arrEmpId)
                    {
                        //str = Convert.ToDateTime(month);
                        //str1 = str.ToString("yyyy-MM-dd");

                        general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                   "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code,d.Description " +
                                   " as designation,gen.sex as gender " +
                                   "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                                   " JOIN Designations d ON d.Id=emp.CurrentDesignation " +
                                    "where gen.emp_code = " + Convert.ToInt32(empId) + " and active=1;";

                        daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                                "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                        tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                         " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                          " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                          "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                          " gross_salary, house_rent_allowance, total_of_sec10, " +
                                          " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                          "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                          " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                           " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                            "balance_tax,balance_months,tds_per_month " +
                                           " from pr_emp_tds_process " +
                                           "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                                           "empcode =  " + Convert.ToInt32(empId) + " and active=1;";
                        section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                             "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                             "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                             "ef.type = 'per_ded' and epf.section_type = 'Section80C' and epf.gross>0 and ef.name not in('LIC','VPF','Provident Fund','Housing Loan Main','Housing Addl.Loan - 2D','GSLI') union all select 'Provident Fund' as name, gross , qual,ded " +
                             "from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 and gross>0 " +
                             " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2 and gross>0 " +
                             " union all select 'LIC' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=3 " +
                             " union all select 'Housing Loan Main' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=4 and gross>0 " +
                            " union all select 'Housing Addl.Loan - 2D' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=5 and gross>0 " +
                            " union all select 'GSLI' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=6 and gross>0 " +
                             " union all select 'Housing Loan 2B-2C' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=7 and gross>0 " +
                             " union all select 'Housing Loan 2A' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=8 and gross>0;";

                        section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                              "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                              "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                              "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                        section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                             "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                             "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and  " +
                             "ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                        section4 = " select concat(ep.section, ' --> ', ef.name) as name,epf.section_type, epf.gross as gross,epf.qual as qual,epf.ded as ded  from pr_deduction_field_master ef" +
                                  " left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 and epf.empcode = " + Convert.ToInt32(empId) + "" +
                                   " join pr_emp_perdeductions ep on ef.id = ep.m_id and ep.active = 1  and ep.emp_code = " + Convert.ToInt32(empId) + "" +
                                    "WHERE Year(epf.fm)= " + Financial_md.Year + " and epf.fy = " + iFY + " and epf.section_type = 'Other' and epf.gross>0  and ep.amount>0 and ef.name !='Officers Assn Fund'";
                                    //" union all " +
                                    //" select concat('Section24(b)', ' --> ', ef.name) as name,epf.section_type, epf.gross as gross,epf.qual as qual,epf.ded as ded  from pr_deduction_field_master ef " +
                                    //" left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 and epf.empcode = " + Convert.ToInt32(empId) + " " +
                                    //"WHERE Year(epf.fm)= " + Financial_md.Year + " and epf.fy = " + iFY + " and epf.section_type = 'Other' and epf.gross > 0  and ef.name != 'Officers Assn Fund' " +
                                    //" and ef.name in(select name from pr_deduction_field_master where name like '%_interest')";

                        allowances = "select all_name,all_amount from pr_emp_tds_process_allowances " +
                               "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                               "emp_code =  " + Convert.ToInt32(empId) + ";"; 
                        checkprocess = "select sal_basic " +
                                           " from pr_emp_tds_process " +
                                           "where Year(fm)=" + Financial_md.Year + " and fy=" + iFY + " and " +
                                           "empcode =  " + Convert.ToInt32(empId) + " and active=1 and sal_basic>0 ;";
                        DataSet ds = await _sha.Get_MultiTables_FromQry(general + daQry + tdsdetail + section1 + section2 + section3 + section4 + allowances + checkprocess);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtmon = ds.Tables[1];
                        DataTable dttds = ds.Tables[2];
                        DataTable sec1 = ds.Tables[3];
                        DataTable sec2 = ds.Tables[4];
                        DataTable sec3 = ds.Tables[5];
                        DataTable sec4 = ds.Tables[6];
                        DataTable allow = ds.Tables[7];
                        DataTable process = ds.Tables[8];
                        if (ds.Tables[8].Rows.Count > 0)
                        {

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DataRow drEmp = ds.Tables[0].Rows[0];



                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "H",
                                    //column1 = "<span style = 'background-color:#ADD8E6' >NAME OF THE EMPLOYEE: " + drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")" + "</span>",
                                    column1 = "<span style='color:#C8EAFB'>~</span>"
                            + ReportColHeader(0, "Name and Address of the Employer", "  " + "TGCAB,Hyderabad" + "  ," + " " + "  " + "  " + " " + "Emp Name:" + " " + drEmp["name"].ToString()),
                                    column2 = "`",
                                    column3 = "`",
                                    column4 = "`",
                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "PAN NO:",
                                    column2 = ReportColFooterAlignleft(employer_pan),
                                    column3 = "Emp Code:",
                                    column4 = drEmp["emp_code"].ToString(),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",

                                    column1 = "TAN NO",
                                    column2 = ReportColFooterAlignleft("HYDT06401D"),
                                    column3 = "Designation:",
                                    column4 = drEmp["designation"].ToString(),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column3 = "Gender:",
                                    column4 = drEmp["gender"].ToString(),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column3 = "PAN No:",
                                    column4 = drEmp["pannumber"].ToString(),

                                });
                            }
                            if (ds.Tables[1].Rows.Count > 0)
                            {


                                DataRow drtime = ds.Tables[1].Rows[0];

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column3 = "Month:",
                                    column4 = drtime["fm"].ToString(),

                                });
                            }

                            if (ds.Tables[2].Rows.Count > 0)
                            {


                                DataRow drtds = ds.Tables[2].Rows[0];

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Basic",
                                    column2 = ReportColConvertToDecimal(drtds["sal_basic"].ToString()),

                                });

                                foreach (DataRow alws in allow.Rows)
                                {
                                    //DataRow sect1 = ds.Tables[3].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = alws["all_name"].ToString(),
                                        column2 = ReportColConvertToDecimal(alws["all_amount"].ToString()),


                                    });

                                }

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "DA",
                                    column2 = ReportColConvertToDecimal(drtds["sal_da"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "HRA",
                                    column2 = ReportColConvertToDecimal(drtds["sal_hra"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "CCA",
                                    column2 = ReportColConvertToDecimal(drtds["sal_cca"].ToString()),
                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "INTERIM RELIEF",
                                    column2 = ReportColConvertToDecimal(drtds["sal_interim_relief"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Telangana Increment",
                                    column2 = ReportColConvertToDecimal(drtds["sal_telangana_increment"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Spl. Allow",
                                    column2 = ReportColConvertToDecimal(drtds["sal_spl_allow"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Spcl. DA",
                                    column2 = ReportColConvertToDecimal(drtds["sal_spcl_da"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "PFPerks",
                                    column2 = ReportColConvertToDecimal(drtds["sal_pfperks"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "LOANPerks",
                                    column2 = ReportColConvertToDecimal(drtds["sal_loanperks"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Incentive",
                                    column2 = ReportColConvertToDecimal(drtds["sal_incentive"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Value of perquisites u/s 17(2)(as per Form.12BA)",
                                    column2 = ReportColConvertToDecimal(drtds["sal_value_of_perquisites"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Profits in lieu of salary u/s 17(3)(as per Form.12BA)",
                                    column2 = ReportColConvertToDecimal(drtds["sal_profits_in_lieu_of_salary"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Gross Salary",
                                    column2 = ReportColConvertToDecimal(drtds["gross_salary"].ToString()),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "2. Less : Allowance to the Extent exempt under Section 10",

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "House Rent Allowance",
                                    column2 = ReportColConvertToDecimal(drtds["house_rent_allowance"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Total of Section 10",
                                    column2 = ReportColConvertToDecimal(drtds["total_of_sec10"].ToString()),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "3. Balance(1-2)",
                                    column2 = ReportColConvertToDecimal((drtds["balance_gross_min_sec10"].ToString())),

                                });


                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "4. Deductions :",


                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "a. Standard Deduction",
                                    column2 = ReportColConvertToDecimal(drtds["standard_deductions"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "b. Tax on Employment",
                                    column2 = ReportColConvertToDecimal(drtds["tax_of_employement"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "5. Aggregate of 4(a) and (b)",
                                    column2 = ReportColConvertToDecimal(drtds["tds_aggregate"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "6. Income Chargeable Under the Head Salaries (3-5)",
                                    column2 = ReportColConvertToDecimal((drtds["income_chargeable_bal_minus_agg"].ToString())),

                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "7. Add : Any Other Income:",


                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Reported by the Employee",
                                    column2 = ReportColConvertToDecimal(drtds["other_income_by_the_emp"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Interest On Housing",
                                    column2 = ReportColConvertToDecimal(drtds["interest_on_housing"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "8. Gross Total Income",
                                    column2 = ReportColConvertToDecimal((drtds["gross_total_income"].ToString())),

                                });
                            }


                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "9. Deductions Under Chapter VI-A",
                                column2 = ReportColFooterAlignleft("Gross.Amt"),
                                column3 = "Qual.Amt",
                                column4 = "Ded.Amt",


                            });
                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "(A) Sections 80C,80CCC and 80CCD",



                            });
                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "(a) Section 80C",
                            });
                            if (sec1.Rows.Count > 0)
                            {
                                var count = 1;
                                foreach (DataRow sect1 in sec1.Rows)
                                {
                                    //DataRow sect1 = ds.Tables[3].Rows[0];
                                    int id = count++;
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "" + id + ". " + sect1["name"].ToString(),
                                        column2 = ReportColConvertToDecimal(sect1["gross"].ToString()),
                                        column3 = ReportColFooterAlign(sect1["qual"].ToString()),
                                        column4 = ReportColFooterAlign(sect1["ded"].ToString()),

                                    });
                                }
                            }

                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "b) Section 80CCC",
                            });
                            if (ds.Tables[4].Rows.Count > 0)
                            {
                                var count = 1;
                                foreach (DataRow sect2 in sec2.Rows)
                                {
                                    //DataRow sect2 = ds.Tables[4].Rows[0];
                                    int id = count++;
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "" + id + ". " + sect2["name"].ToString(),
                                        column2 = ReportColConvertToDecimal(sect2["gross"].ToString()),
                                        column3 = ReportColFooterAlign(sect2["qual"].ToString()),
                                        column4 = ReportColFooterAlign(sect2["ded"].ToString()),

                                    });
                                }
                            }

                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "(c) Section 80CCD",
                            });

                            if (ds.Tables[5].Rows.Count > 0)
                            {
                                foreach (DataRow sect3 in sec3.Rows)
                                {
                                    var count = 1;
                                    //DataRow sect3 = ds.Tables[5].Rows[0];
                                    int id = count++;
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "" + id + ". " + sect3["name"].ToString(),
                                        column2 = ReportColConvertToDecimal(sect3["gross"].ToString()),
                                        column3 = ReportColFooterAlign(sect3["qual"].ToString()),
                                        column4 = ReportColFooterAlign(sect3["ded"].ToString()),

                                    });
                                }
                            }
                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "(B) Other Sections Under Chaper VI-A",
                            });
                            if (ds.Tables[6].Rows.Count > 0)
                            {
                                var count = 1;
                                foreach (DataRow sect4 in sec4.Rows)
                                {
                                    //DataRow sect4 = ds.Tables[6].Rows[0];
                                    int id = count++;
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "" + id + ". " + sect4["name"].ToString(),
                                        column2 = ReportColConvertToDecimal(sect4["gross"].ToString()),
                                        column3 = ReportColFooterAlign(sect4["qual"].ToString()),
                                        column4 = ReportColFooterAlign(sect4["ded"].ToString()),

                                    });
                                }
                            }
                            if (ds.Tables[2].Rows.Count > 0)
                            {


                                DataRow drEmp1 = ds.Tables[2].Rows[0];

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "10. Aggregate of deductible amount Under Chapter VIA",
                                    column2 = ReportColConvertToDecimal(drEmp1["aggregate_of_deductible"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "11. Total Income (8-10)",
                                    column2 = ReportColConvertToDecimal(drEmp1["total_income"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "12.Tax on Total Income",
                                    column2 = ReportColConvertToDecimal(drEmp1["tax_on_total_income"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(b) Section 87A",
                                    column2 = ReportColConvertToDecimal(drEmp1["section_87a"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "14. Education CESS",
                                    column2 = ReportColConvertToDecimal(drEmp1["education_cess"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "15. Tax payable",
                                    column2 = ReportColConvertToDecimal(drEmp1["tax_payable"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "16.Less : a.Tax deducted at Source",
                                    column2 = ReportColConvertToDecimal(drEmp1["tax_deducted_at_source"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Less : b.Tax paid by the employer",
                                    column2 = ReportColConvertToDecimal(drEmp1["tax_paid_by_the_employer"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "17. Balance Tax (15-16)",
                                    column2 = ReportColConvertToDecimal(drEmp1["balance_tax"].ToString()),

                                });
                                
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "18. Balance Months",
                                    column2 = drEmp1["balance_months"].ToString(),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "19. TDS Per Month",
                                    column2 = ReportColConvertToDecimal(drEmp1["tds_per_month"].ToString()),

                                });
                                //lst.Add(new CommonReportModel
                                //{
                                //    RowId = rowid++,
                                //    HRF = "R",
                                //    column1 = "",
                                //    column2 = "",

                                //});
                                //lst.Add(new CommonReportModel
                                //{
                                //    RowId = rowid++,
                                //    HRF = "R",
                                //    column1 = "",
                                //    column2 = "",

                                //});
                                //lst.Add(new CommonReportModel
                                //{
                                //    RowId = rowid++,
                                //    HRF = "R",
                                //    column1 = "",
                                //    column2 = "",

                                //});
                            }
                        }
                    }
                }
                return lst;

            }
            catch (Exception e)
            {

            }
            return lst;
        }
        public async Task<IList<CommonReportModel>> Form16BDetails(string EmpCode, string month)
        {
            int iFY = _LoginCredential.FY;
            int dtFM = _LoginCredential.FM;
            DateTime Financial_md = (_LoginCredential.FinancialMonthDate);
            decimal? totalded = 0;
            decimal? totaltax = 0;
            decimal? total = 0;
            decimal? loanperks = 0;
            decimal? pfperks = 0;
            decimal? splda = 0;
            decimal? splall = 0;
            decimal? telincre = 0;
            decimal? interrelief = 0;
            decimal? salcca = 0;
            decimal? salhra = 0;
            decimal? salda = 0;
            decimal? salfpip = 0;
            decimal? salfixedperall = 0;
            decimal? hraallo = 0;
            decimal? basic = 0;
            decimal? staginc = 0;
            decimal? specpay = 0;
            decimal? encash = 0;
            decimal? section_total = 0;

            decimal? Exgratia = 0;
            decimal? Medical_Aid = 0;
            decimal? codperks = 0;
            decimal? annual_inc = 0;
            decimal? ltc = 0;
            decimal? interonnsc = 0;
            decimal? perqpay = 0;
            decimal? plencash = 0;
            decimal? spacsti = 0;
            decimal? brmgr = 0;
            decimal? gratuity = 0;
            decimal? spdaftari = 0;
            decimal? pfperksear = 0;
            decimal? leaveencash = 0;
            decimal? incrment = 0;

            decimal h2d = 0;
            decimal hs1 = 0;
            decimal dhouse2 = 0;
            decimal dhouse2a = 0;
            decimal dhouseloancomm = 0;
            decimal dhouselnplot = 0;
            decimal dhouseln2 = 0;
            decimal dhouseln3 = 0;
            decimal dhouselnint = 0;
            decimal pff = 0;
            decimal vpf = 0;
            decimal dlic = 0;
            decimal dhouse2b2c = 0;
            decimal dhdfchlprincple = 0;
            decimal dtaxsaverfd = 0;
            decimal dstagallow = 0;
            decimal dencash = 0;
            IList<CommonReportModel> lst = new List<CommonReportModel>();
            string general = "";
            string tdsdetail = "";
            string daQry = "";
            DateTime str;
            string str1;
            string section1 = "";
            string section2 = "";
            string section3 = "";
            string section4 = "";
            string section5 = "";
            string section6 = "";
            string section7 = "";
            string qrySel = "";
            int rowid = 0;
            string house2d = "";
            string house1 = "";
            string house2b2c = "";
            string house2 = "";
            string house2a = "";
            string houselncomm = "";
            string houselnplot = "";
            string houseln2 = "";
            string houseln3 = "";
            string houselnint = "";
            string str_allowance = "";
            string pfamt = "";
            string lic = "";
            string option = "";
            string strvpf = "";
            string strdeduct = "";

            // CommonReportModel crm = new TDSReport();
            int Eyears = 0001;
            int Fyears = 0001;

            if (EmpCode == "^2")
            {
                EmpCode = "0";
            }

            else if (EmpCode.Contains("^"))
            {
                EmpCode = "0";
                Fyears = 0001;
                Eyears = 0001;
            }

            else
            {
                Eyears = Int32.Parse(month);
                Fyears = Int32.Parse(month) - 1;
            }
            if (EmpCode == "All")
            {
                //str = Convert.ToDateTime(month);
                //str1 = str.ToString("yyyy-MM-dd");
                string emp_codes = "0";
                if (iFY == Eyears)
                {
                    qrySel = "SELECT empcode " +
                             "FROM pr_emp_tds_process " +
                             " where fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and active=1 and sal_basic>0";
                }
                else
                {
                    qrySel = "SELECT empcode " +
                             "FROM pr_emp_tds_process " +
                             " where  Month(fm)=03 and year(fm)=" + Eyears + " and sal_basic>0";

                }
                DataTable dt = await _sha.Get_Table_FromQry(qrySel);


                foreach (DataRow dr in dt.Rows)
                {

                    //string str;
                    emp_codes += dr["empcode"] + ",";

                }
                emp_codes = emp_codes.Substring(0, emp_codes.Length - 1);
                string[] arrEmpId = emp_codes.Split(',');
                if (emp_codes != "")
                {
                    foreach (string empId in arrEmpId)
                    {
                        if (empId == " ")
                        {
                            if (iFY == Eyears)
                            {
                                general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                        "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code, gen.designation" +
                                        " as designation,gen.sex as gender " +
                                        "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                                         "where gen.emp_code = '0' ;";

                                daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                            "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                                tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                                 " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                                  " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                                  "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                                  " gross_salary, house_rent_allowance, total_of_sec10, " +
                                                  " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                                  "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                                  " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                                   " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                    "balance_tax,balance_months,tds_per_month " +
                                                   " from pr_emp_tds_process " +
                                                   "where fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and " +
                                                   "empcode =  '0' and active=1;";
                                section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = '0' WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Section80C'";
                                section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                      "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                      "and epf.empcode = '0' WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                      "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                                section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = '0' WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                                section4 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = '0' WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Other'";
                                section5 = " select pe.amount as amount ,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Stagnation Increments' and pe.active=1 where  pe.emp_code = '0' and pe.fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                section6 = " select pe.amount as amount,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Special Pay' and pe.active=1 where pe.emp_code ='0' and pe.fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                section7 = "select gross_amount as encashamt from pr_emp_payslip where spl_type='Encashment' and active=1 and emp_code = '0' and fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                house2d = "select sum(principal_paid_amount) as hl2damount from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                                    "on l.id = adj.emp_adv_loans_mid where l.emp_code = '0' and loan_type_mid = 7 " +
                                    //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                    "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";

                                house1 = "select sum(principal_paid_amount) as house1 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = '0' and loan_type_mid = 10 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                                house2 = "select sum(principal_paid_amount) as house2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = '0' and loan_type_mid = 4 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                                house2a = "select sum(principal_paid_amount) as house2a from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = '0' and loan_type_mid = 5 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselncomm = "select sum(principal_paid_amount) as houseloancomm from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                          "on l.id = adj.emp_adv_loans_mid where l.emp_code = '0' and loan_type_mid = 8 " +
                          //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                          "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselnplot = "select sum(principal_paid_amount) as houseloanplot from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = '0' and loan_type_mid = 9 " +
                         //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                         "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houseln2 = "select sum(principal_paid_amount) as houseloan2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = '0' and loan_type_mid = 11 " +
                         //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                         "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houseln3 = "select sum(principal_paid_amount) as houseloan3 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = '0' and loan_type_mid = 12 " +
                         //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                         "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselnint = "select sum(principal_paid_amount) as houseloanint from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                        "on l.id = adj.emp_adv_loans_mid where l.emp_code = '0' and loan_type_mid = 13 " +
                        //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                        "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                pfamt = "select sum(dd_provident_fund)as pf from pr_emp_payslip where emp_code='0' and fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                                //"//and fm <= concat('" + Eyears + "', ' -03-01')";
                                lic = "select sum(dd_amount) as lic from pr_emp_payslip_deductions where emp_code='0' and dd_name='LIC' and payslip_mid in(select distinct id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and fy=" + Eyears + ")";
                                option = "Select [Option] from pr_tax_option_emp_wise where EmpId=" + Convert.ToInt32(empId) + ";";
                                house2b2c = "select sum(principal_paid_amount) as house2b2c from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = '0' and loan_type_mid = 6 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                                str_allowance = "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id=ep.m_id where ep.emp_code='0' and efm.name='Exgratia' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'Medical Aid' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'LTC' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )   " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'Interest On NSC (Earning)' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'codperks' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'PERQPAY' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'PL Encashment' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'SP_ACSTI' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'BR_MGR' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'GRATUITY' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'SP_DAFTARI' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'PFPerks' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'Leave Encashment' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'INCREMENT' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                 "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'STAGALLOW' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = '0' and efm.name = 'ENCASHMENT' and ep.fm = concat('" + Eyears + "', ' -03-01') ";
                                strvpf = "select sum(dd_amount) as vpf from pr_emp_payslip_deductions where emp_code='0' and dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip where emp_code='0' and  fy=" + Eyears + ")";
                                strdeduct = " select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id=tdsded.m_id where tdsded.empcode='0' and fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  and dedmas.id=310 " +
                                "union all " +
                                "select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id = tdsded.m_id where tdsded.empcode = '0' and fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and dedmas.id = 71 ";
                            }
                            else
                            {
                                general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                        "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code, gen.designation" +
                                        " as designation,gen.sex as gender " +
                                        "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                                         "where gen.emp_code = '0' ;";

                                daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                            "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                                tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                                 " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                                  " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                                  "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                                  " gross_salary, house_rent_allowance, total_of_sec10, " +
                                                  " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                                  "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                                  " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                                   " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                    "balance_tax,balance_months,tds_per_month " +
                                                   " from pr_emp_tds_process " +
                                                   "where  Month(fm)=03 and year(fm)=" + Eyears + " and " +
                                                   "empcode =  '0' ;";
                                section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = '0' WHERE  Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Section80C'";
                                section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                      "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                      "and epf.empcode = '0' WHERE  Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                      "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                                section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = '0' WHERE  Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                                section4 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = '0' WHERE  Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Other'";
                                section5 = " select pe.amount as amount ,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Stagnation Increments' and pe.active=1 where  pe.emp_code = " + Convert.ToInt32(empId) + " and Month(pe.fm)=03 and year(fm)=" + Eyears + "";
                                section6 = " select pe.amount as amount,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Special Pay' and pe.active=1 where pe.emp_code = " + Convert.ToInt32(empId) + " and Month(pe.fm)=03 and year(pe.fm)=" + Eyears + "";
                                section7 = "select sum(gross_amount) as encashamt from pr_emp_payslip where spl_type='Encashment'  and emp_code = " + Convert.ToInt32(empId) + " and Month(fm)=03 and year(fm)=" + Eyears + "";
                                house2d = "select sum(principal_paid_amount) as hl2damount from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                                    "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 7 " +
                                    "and adj.fm <= concat('" + Eyears + "', ' -03-01')";

                                house1 = "select sum(principal_paid_amount) as house1 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 10 " +
                           "and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                house2 = "select sum(principal_paid_amount) as house2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 4 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                                house2a = "select sum(principal_paid_amount) as house2a from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 5 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselncomm = "select sum(principal_paid_amount) as houseloancomm from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                          "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 8 " +
                          //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                          "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselnplot = "select sum(principal_paid_amount) as houseloanplot from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 9 " +
                         //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                         "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houseln2 = "select sum(principal_paid_amount) as houseloan2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 11 " +
                         //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                         "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houseln3 = "select sum(principal_paid_amount) as houseloan3 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 12 " +
                         //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                         "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselnint = "select sum(principal_paid_amount) as houseloanint from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                        "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 13 " +
                        //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                        "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                pfamt = "select sum(dd_provident_fund)as pf from pr_emp_payslip where emp_code= " + Convert.ToInt32(empId) + " and fm <= concat('" + Eyears + "', ' -03-01')";
                                lic = "select sum(dd_amount) as lic from pr_emp_payslip_deductions where emp_code=" + Convert.ToInt32(empId) + " and dd_name='LIC' and payslip_mid in(select distinct id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and fy=" + Eyears + ")";
                                option = "Select [Option] from pr_tax_option_emp_wise where EmpId=" + Convert.ToInt32(empId) + ";";
                                house2b2c = "select sum(principal_paid_amount) as house2b2c from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 6 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                                str_allowance = "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id=ep.m_id where ep.emp_code=" + Convert.ToInt32(empId) + " and efm.name='Exgratia' and ep.fm=concat('" + Eyears + "', ' -03-01')" +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Medical Aid' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'LTC' and ep.fm = concat('" + Eyears + "', ' -03-01')  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Interest On NSC (Earning)' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'codperks' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PERQPAY' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PL Encashment' and ep.fm = concat('" + Eyears + "', ' -03-01')" +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'SP_ACSTI' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'BR_MGR' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'GRATUITY' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'SP_DAFTARI' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PFPerks' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Leave Encashment' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'INCREMENT' and ep.fm = concat('2020', ' -03-01') " +
                                "union all " +
                                 "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'STAGALLOW' and ep.fm = concat('2020', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'ENCASHMENT' and ep.fm = concat('2020', ' -03-01') ";

                                strvpf = "select sum(dd_amount) as vpf from pr_emp_payslip_deductions where emp_code=" + Convert.ToInt32(empId) + " and dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and  fy=" + Eyears + ")";
                                strdeduct = " select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id=tdsded.m_id where tdsded.empcode=" + Convert.ToInt32(empId) + " and fm=concat('" + Eyears + "', ' -03-01') and dedmas.id=310 " +
                                "union all " +
                                "select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id = tdsded.m_id where tdsded.empcode = " + Convert.ToInt32(empId) + " and fm = concat('" + Eyears + "', ' -03-01') and dedmas.id = 71 ";
                            }
                            DataSet ds1 = await _sha.Get_MultiTables_FromQry(general + daQry + tdsdetail + section1 + section2 + section3 + section4 + section5 + section6 + section7 + house2d + house1 + house2 + house2a + houselncomm + houselnplot + houseln2 + houseln3 + houselnint + pfamt + lic + option + house2b2c + str_allowance + strvpf + strdeduct);
                            DataTable dtALL1 = ds1.Tables[0];
                            DataTable dtmon1 = ds1.Tables[1];
                            DataTable dttds1 = ds1.Tables[2];
                            DataTable sec11 = ds1.Tables[3];
                            DataTable sec21 = ds1.Tables[4];
                            DataTable sec31 = ds1.Tables[5];
                            DataTable sec41 = ds1.Tables[6];
                            DataTable sec51 = ds1.Tables[7];
                            DataTable sec61 = ds1.Tables[8];
                            DataTable sec71 = ds1.Tables[9];
                            DataTable dt_hose2d = ds1.Tables[10];
                            DataTable dt_house1 = ds1.Tables[11];
                            DataTable dt_house2 = ds1.Tables[12];
                            DataTable dt_hose21 = ds1.Tables[13];
                            DataTable dt_houselncomm = ds1.Tables[14];
                            DataTable dt_houselnplot = ds1.Tables[15];
                            DataTable dt_houseln2 = ds1.Tables[16];
                            DataTable dt_houseln3 = ds1.Tables[17];
                            DataTable dt_houselnint = ds1.Tables[18];
                            DataTable dt_pfamt = ds1.Tables[19];
                            DataTable dt_lic = ds1.Tables[20];
                            DataTable dt_option = ds1.Tables[21];
                            DataTable dt_house2b2c = ds1.Tables[22];
                            DataTable dt_str_allowance = ds1.Tables[23];
                            DataTable dt_strvpf = ds1.Tables[24];
                            DataTable dt_strdeduct = ds1.Tables[25];

                            if (ds1.Tables[2].Rows.Count > 0)
                            {
                                if (ds1.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drEmp = ds1.Tables[0].Rows[0];



                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "H",
                                        //column1 = "<span style = 'background-color:#ADD8E6' >NAME OF THE EMPLOYEE: " + drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")" + "</span>",
                                        column1 = "<span style='color:#C8EAFB'>~</span>"
                                     + ReportColFooterlesscol(150, "Employee Name", drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")"),
                                        column2 = "`",

                                    });



                                }
                                if (ds1.Tables[1].Rows.Count > 0)
                                {


                                    DataRow drtime = ds1.Tables[1].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Month:",
                                        column2 = drtime["fm"].ToString(),

                                    });
                                }
                                if (ds1.Tables[2].Rows.Count > 0)
                                {


                                    DataRow drtds = ds1.Tables[2].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Basic",
                                        column2 = ReportColConvertToDecimal(drtds["sal_basic"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Fixed Personal Allowance",
                                        column2 = ReportColConvertToDecimal(drtds["sal_fixed_personal_allowance"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "FPA-HRA Allowance",
                                        column2 = ReportColConvertToDecimal(drtds["sal_fpa_hra_allowance"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "FPIIP",
                                        column2 = ReportColConvertToDecimal(drtds["sal_fpiip"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "DA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_da"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "HRA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_hra"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "CCA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_cca"].ToString()),
                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "INTERIM RELIEF",
                                        column2 = ReportColConvertToDecimal(drtds["sal_interim_relief"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Telangana Increment",
                                        column2 = ReportColConvertToDecimal(drtds["sal_telangana_increment"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spl. Allow",
                                        column2 = ReportColConvertToDecimal(drtds["sal_spl_allow"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spcl. DA",
                                        column2 = ReportColConvertToDecimal(drtds["sal_spcl_da"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "PFPerks",
                                        column2 = ReportColConvertToDecimal(drtds["sal_pfperks"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "LOANPerks",
                                        column2 = ReportColConvertToDecimal(drtds["sal_loanperks"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Incentive",
                                        column2 = ReportColConvertToDecimal(drtds["sal_incentive"].ToString()),

                                    });



                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = " Deductions :",


                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "a. Standard Deduction",
                                        column2 = ReportColConvertToDecimal(drtds["standard_deductions"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "b. Tax on Employment",
                                        column2 = ReportColConvertToDecimal(drtds["tax_of_employement"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "5. Aggregate of 4(a) and (b)",
                                        column2 = ReportColConvertToDecimal(drtds["tds_aggregate"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "6. Income Chargeable Under the Head Salaries (3-5)",
                                        column2 = ReportColConvertToDecimal(drtds["income_chargeable_bal_minus_agg"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column2 = "7. Add : Any Other Income:",


                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Reported by the Employee",
                                        column2 = ReportColConvertToDecimal(drtds["other_income_by_the_emp"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Interest On Housing",
                                        column2 = ReportColConvertToDecimal(drtds["interest_on_housing"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "8. Gross Total Income",
                                        column2 = ReportColConvertToDecimal(drtds["gross_total_income"].ToString()),

                                    });
                                }


                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "9. Deductions Under Chapter VI-A",
                                    column2 = ReportColFooterAlignleft("Gross.Amt"),



                                });
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(A) Sections 80C,80CCC and 80CCD",



                                });
                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(a) Section 80C",
                                });
                                if (sec11.Rows.Count > 0)
                                {
                                    foreach (DataRow sect1 in sec11.Rows)
                                    {
                                        //DataRow sect1 = ds.Tables[3].Rows[0];

                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = sect1["name"].ToString(),
                                            column2 = sect1["gross"].ToString(),


                                        });
                                    }
                                }

                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "b) Section 80CCC",
                                });
                                if (ds1.Tables[4].Rows.Count > 0)
                                {
                                    foreach (DataRow sect2 in sec21.Rows)
                                    {
                                        //DataRow sect2 = ds.Tables[4].Rows[0];
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = sect2["name"].ToString(),
                                            column2 = sect2["gross"].ToString(),


                                        });
                                    }
                                }

                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "(c) Section 80CCD",
                                });

                                if (ds1.Tables[5].Rows.Count > 0)
                                {
                                    foreach (DataRow sect3 in sec31.Rows)
                                    {
                                        //DataRow sect3 = ds.Tables[5].Rows[0];

                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = sect3["name"].ToString(),
                                            column2 = sect3["gross"].ToString(),


                                        });
                                    }
                                }

                                if (ds1.Tables[2].Rows.Count > 0)
                                {


                                    DataRow drEmp1 = ds1.Tables[2].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "10. Aggregate of deductible amount Under Chapter VIA",
                                        column2 = ReportColConvertToDecimal(drEmp1["aggregate_of_deductible"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "11. Total Income (8-10)",
                                        column2 = ReportColConvertToDecimal(drEmp1["total_income"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "12.Tax on Total Income",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_on_total_income"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "(b) Section 87A",
                                        column2 = ReportColConvertToDecimal(drEmp1["section_87a"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "14. Education CESS",
                                        column2 = ReportColConvertToDecimal(drEmp1["education_cess"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "15. Tax payable",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_payable"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "16.Less : a.Tax deducted at Source",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_deducted_at_source"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Less : b.Tax paid by the employer",
                                        column2 = ReportColConvertToDecimal(drEmp1["tax_paid_by_the_employer"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "17. Balance Tax (15-16)",
                                        column2 = ReportColConvertToDecimal(drEmp1["balance_tax"].ToString()),

                                    });

                                    
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "18. Balance Months",
                                        column2 = ReportColConvertToDecimal(drEmp1["balance_months"].ToString()),

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "19. TDS Per Month",
                                        column2 = ReportColConvertToDecimal(drEmp1["tds_per_month"].ToString()),

                                    });

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",

                                    });
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",

                                    });

                                }


                            }
                        }
                        else
                        {
                            if (iFY == Eyears)
                            {
                                general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                        "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code, gen.designation" +
                                        " as designation,gen.sex as gender " +
                                        "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                                         "where gen.emp_code = " + Convert.ToInt32(empId) + ";";

                                daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                            "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                                tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                                 " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                                  " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                                  "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                                  " gross_salary, house_rent_allowance, total_of_sec10, " +
                                                  " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                                  "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                                  " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                                   " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                    "balance_tax,balance_months,tds_per_month " +
                                                   " from pr_emp_tds_process " +
                                                   "where fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and " +
                                                   "empcode =  " + Convert.ToInt32(empId) + " and active=1;";
                                section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Section80C' and ef.name not in('LIC','VPF','Provident Fund','Housing Loan Main','Housing Addl.Loan - 2D','GSLI') union all select 'Provident Fund' as name, gross , qual,ded " +
                                     "from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 " +
                                     " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2 " +
                                     " union all select 'LIC' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=3 " +
                                     " union all select 'Housing Loan Main' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=4 and gross>0 " +
                                    " union all select 'Housing Addl.Loan - 2D' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=5 and gross>0 " +
                                     " union all select 'GSLI' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=6 and gross>0 " +
                             " union all select 'Housing Loan 2B-2C' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=7 and gross>0 " +
                             " union all select 'Housing Loan 2A' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=8 and gross>0;";

                                section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                      "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                      "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                      "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                                section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                                section4 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.section_type as section, epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Other'";
                                section5 = " select pe.amount as amount ,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Stagnation Increments' and pe.active=1 where  pe.emp_code = " + Convert.ToInt32(empId) + " and pe.fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                section6 = " select pe.amount as amount,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Special Pay' and pe.active=1 where pe.emp_code = " + Convert.ToInt32(empId) + " and pe.fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                section7 = "select gross_amount as encashamt from pr_emp_payslip where spl_type='Encashment' and active=1 and emp_code = " + Convert.ToInt32(empId) + " and fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                house2d = "select sum(principal_paid_amount) as hl2damount from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                                    "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 7 " +
                                    //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                    "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";

                                house1 = "select sum(principal_paid_amount) as house1 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 10 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                                house2 = "select sum(principal_paid_amount) as house2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 4 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                                house2a = "select sum(principal_paid_amount) as house2a from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 5 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselncomm = "select sum(principal_paid_amount) as houselncomm from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                          "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 8 " +
                          //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                          "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselnplot = "select sum(principal_paid_amount) as houselnplot from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 9 " +
                         //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                         "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houseln2 = "select sum(principal_paid_amount) as houseln2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 11 " +
                         //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                         "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houseln3 = "select sum(principal_paid_amount) as houseln3 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 12 " +
                         //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                         "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselnint = "select sum(principal_paid_amount) as houselnint from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                        "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 13 " +
                        //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                        "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";

                                pfamt = "select sum(dd_provident_fund)as pf from pr_emp_payslip where emp_code= " + Convert.ToInt32(empId) + "and fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                                lic = "select sum(dd_amount) as lic from pr_emp_payslip_deductions where emp_code=" + Convert.ToInt32(empId) + " and dd_name='LIC' and payslip_mid in(select distinct id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and fy=" + Eyears + ")";
                                //"//and fm <= concat('" + Eyears + "', ' -03-01')";
                                option = "Select [Option] from pr_tax_option_emp_wise where EmpId=" + Convert.ToInt32(empId) + "";
                                house2b2c = "select sum(principal_paid_amount) as house2b2c from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 6 " +
                           //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                           "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                str_allowance = "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id=ep.m_id where ep.emp_code=" + Convert.ToInt32(empId) + " and efm.name='Exgratia' and ep.fm=concat('" + Eyears + "', ' -03-01')" +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Medical Aid' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'LTC' and ep.fm = concat('" + Eyears + "', ' -03-01')  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Interest On NSC (Earning)' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'codperks' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PERQPAY' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PL Encashment' and ep.fm = concat('" + Eyears + "', ' -03-01')" +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'SP_ACSTI' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'BR_MGR' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'GRATUITY' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'SP_DAFTARI' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PFPerks' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Leave Encashment' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name ='INCREMENT' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                 "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'STAGALLOW' and ep.fm = concat('2020', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'ENCASHMENT' and ep.fm = concat('2020', ' -03-01') ";

                                strvpf = "select sum(dd_amount) as vpf from pr_emp_payslip_deductions where emp_code=" + Convert.ToInt32(empId) + " and dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and  fy=" + Eyears + ") ";
                                strdeduct = " select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id=tdsded.m_id where tdsded.empcode=" + Convert.ToInt32(empId) + " and fm=concat('" + Eyears + "', ' -03-01') and dedmas.id=310 " +
                                "union all " +
                                "select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id = tdsded.m_id where tdsded.empcode = " + Convert.ToInt32(empId) + " and fm = concat('" + Eyears + "', ' -03-01') and dedmas.id = 71 ";
                            }
                            else
                            {
                                general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                                                   "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code, gen.designation" +
                                                                   " as designation,gen.sex as gender " +
                                                                   "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                                                                    "where gen.emp_code = " + Convert.ToInt32(empId) + ";";

                                daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                             "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                                tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                                 " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                                  " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                                  "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                                  " gross_salary, house_rent_allowance, total_of_sec10, " +
                                                  " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                                  "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                                  " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                                   " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                    "balance_tax,balance_months,tds_per_month " +
                                                   " from pr_emp_tds_process " +
                                                   "where  Month(fm)=03 and year(fm)=" + Eyears + " and " +
                                                   "empcode =  " + Convert.ToInt32(empId) + ";";
                                section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE  Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Section80C' and ef.name not in('LIC','VPF','Provident Fund','Housing Loan Main','Housing Addl.Loan - 2D','GSLI') union all select 'Provident Fund' as name, gross , qual,ded " +
                                     "from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 " +
                                     " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2 " +
                                     " union all select 'LIC' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=3 " +
                                     " union all select 'Housing Loan Main' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=4 and gross>0 " +
                                    " union all select 'Housing Addl.Loan - 2D' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=5 and gross>0 " +
                                    " union all select 'GSLI' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=6 and gross>0 " +
                             " union all select 'Housing Loan 2B-2C' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=7 and gross>0 " +
                             " union all select 'Housing Loan 2A' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=8 and gross>0;";

                                section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                      "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                      "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                      "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                                section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE  Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                                section4 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.section_type as section, epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE  Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Other'";
                                section5 = " select pe.amount as amount ,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Stagnation Increments' and pe.active=1 where  pe.emp_code = " + Convert.ToInt32(empId) + " and   Month(pe.fm)=03 and year(pe.fm)=" + Eyears + " ";
                                section6 = " select pe.amount as amount,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Special Pay' and pe.active=1 where pe.emp_code = " + Convert.ToInt32(empId) + " and  Month(pe.fm)=03 and year(pe.fm)=" + Eyears + " ";
                                section7 = "select gross_amount as encashamt from pr_emp_payslip where spl_type='Encashment' and active=1 and emp_code = " + Convert.ToInt32(empId) + " and Month(fm)=03 and year(fm)=" + Eyears + " ";
                                house2d = "select sum(principal_paid_amount) as hl2damount from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                                    "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 7 " +
                                    "and adj.fm <= concat('" + Eyears + "', ' -03-01')";

                                house1 = "select sum(principal_paid_amount) as house1 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 10 " +
                           "and adj.fm <= concat('" + Eyears + "', ' -03-01')";

                                house2 = "select sum(principal_paid_amount) as house2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 4 " +
                           "and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                //"and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                                house2a = "select sum(principal_paid_amount) as house2a from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 5 " +
                           "and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                //"and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselncomm = "select sum(principal_paid_amount) as houselncomm from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                          "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 8 " +
                          "and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                //"and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselnplot = "select sum(principal_paid_amount) as houselnplot from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 9 " +
                         "and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                //"and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houseln2 = "select sum(principal_paid_amount) as houseln2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 11 " +
                         "and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                //"and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houseln3 = "select sum(principal_paid_amount) as houseln3 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                         "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 12 " +
                         "and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                //"and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                                houselnint = "select sum(principal_paid_amount) as houselnint from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                        "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 13 " +
                        "and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                //"and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";

                                pfamt = "select sum(dd_provident_fund)as pf from pr_emp_payslip where emp_code= " + Convert.ToInt32(empId) + " and fm <= concat('" + Eyears + "', ' -03-01')";
                                lic = "select sum(dd_amount) as lic from pr_emp_payslip_deductions where emp_code=" + Convert.ToInt32(empId) + " and dd_name='LIC' and payslip_mid in(select distinct id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and fy=" + Eyears + ")";
                                option = "Select [Option] from pr_tax_option_emp_wise where EmpId=" + Convert.ToInt32(empId) + "";
                                house2b2c = "select sum(principal_paid_amount) as house2b2c from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                           "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 6 " +
                           "and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                str_allowance = "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id=ep.m_id where ep.emp_code=" + Convert.ToInt32(empId) + " and efm.name='Exgratia' and ep.fm=concat('" + Eyears + "', ' -03-01')" +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Medical Aid' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'LTC' and ep.fm = concat('" + Eyears + "', ' -03-01')  " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Interest On NSC (Earning)' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'codperks' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PERQPAY' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PL Encashment' and ep.fm = concat('" + Eyears + "', ' -03-01')" +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'SP_ACSTI' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'BR_MGR' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'GRATUITY' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'SP_DAFTARI' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PFPerks' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Leave Encashment' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name ='INCREMENT' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                                "union all " +
                                 "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'STAGALLOW' and ep.fm = concat('2020', ' -03-01') " +
                                "union all " +
                                "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'ENCASHMENT' and ep.fm = concat('2020', ' -03-01') ";
                                strvpf = "select sum(dd_amount) as vpf from pr_emp_payslip_deductions where emp_code=" + Convert.ToInt32(empId) + " and dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and  fy=" + Eyears + ")";
                                strdeduct = " select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id=tdsded.m_id where tdsded.empcode=" + Convert.ToInt32(empId) + " and fm=concat('" + Eyears + "', ' -03-01') and dedmas.id=310 " +
                                "union all " +
                                "select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id = tdsded.m_id where tdsded.empcode = " + Convert.ToInt32(empId) + " and fm = concat('" + Eyears + "', ' -03-01') and dedmas.id = 71 ";

                            }
                            DataSet ds = await _sha.Get_MultiTables_FromQry(general + daQry + tdsdetail + section1 + section2 + section3 + section4 + section5 + section6 + section7 + house2d + house1 + house2 + house2a + houselncomm + houselnplot + houseln2 + houseln3 + houselnint + pfamt + lic + option + house2b2c + str_allowance + strvpf + strdeduct);
                            DataTable dtALL = ds.Tables[0];
                            DataTable dtmon = ds.Tables[1];
                            DataTable dttds = ds.Tables[2];
                            DataTable sec1 = ds.Tables[3];
                            DataTable sec2 = ds.Tables[4];
                            DataTable sec3 = ds.Tables[5];
                            DataTable sec4 = ds.Tables[6];
                            // stagnation increment 
                            DataTable sec5 = ds.Tables[7];
                            //special pay
                            DataTable sec6 = ds.Tables[8];
                            //Encashment amount
                            DataTable sec7 = ds.Tables[9];

                            DataTable hl2d = ds.Tables[10];
                            DataTable hous1 = ds.Tables[11];
                            DataTable dt_house2 = ds.Tables[12];
                            DataTable dt_house2a = ds.Tables[13];
                            DataTable dt_houselncomm = ds.Tables[14];
                            DataTable dt_houselnplot = ds.Tables[15];
                            DataTable dt_houseln2 = ds.Tables[16];
                            DataTable dt_houseln3 = ds.Tables[17];
                            DataTable dt_houselnint = ds.Tables[18];
                            DataTable pf = ds.Tables[19];
                            DataTable dt_lic = ds.Tables[20];
                            DataTable dt_opt = ds.Tables[21];
                            DataTable dt_house2b2c = ds.Tables[22];
                            DataTable dt_allowance = ds.Tables[23];
                            DataTable dt_vpf = ds.Tables[24];
                            DataTable dt_deduct = ds.Tables[25];
                            if (ds.Tables[2].Rows.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drEmp = ds.Tables[0].Rows[0];



                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "H",
                                        //column1 = "<span style = 'background-color:#ADD8E6' >NAME OF THE EMPLOYEE: " + drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")" + "</span>",
                                        column1 = "<span style='color:#C8EAFB'>~</span>"
                                       //+ ReportColFooterlesscol(0, "Emp Name", drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")")
                                        + ReportColFooterlesscol(0, "Emp Code", drEmp["emp_code"].ToString() )

                                        + ReportColFooterlesscol(2, "Emp Name", drEmp["name"].ToString() )


                                        + ReportColFooterlesscol(3, "Designation", drEmp["designation"].ToString() )
                                      + ReportColFooterlesscol(3, "Pan", drEmp["pannumber"].ToString() ),

                                        column2 = "`",
                                        column3 = "`",
                                        column4 = "`",
                                        column5 = "`",
                                        column6 = "`",


                                    });



                                }


                            }

                            if (ds.Tables[2].Rows.Count > 0)
                            {

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",

                                    column1 = "<span style = 'color:#008000' >1. PARTICULARS OF SALARY INCOME </ span >",


                                });

                                DataRow drtds1 = ds.Tables[2].Rows[0];
                                object o_basic = drtds1["sal_basic"];
                                if (o_basic == DBNull.Value)
                                {
                                    basic = 0;
                                }
                                else
                                {
                                    basic = Convert.ToDecimal(drtds1["sal_basic"]);
                                }
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Basic",
                                    //column2 = drtds1["sal_basic"].ToString(),
                                    column2 = ReportColConvertToDecimal(basic.ToString()),

                                });
                                //basic = Convert.ToDecimal(drtds1["sal_basic"]);


                                DataRow sec5ds;
                                if (ds.Tables[7].Rows.Count > 0)
                                {
                                    sec5ds = ds.Tables[7].Rows[0];
                                    object o_staginc = sec5ds["amount"];
                                    if (o_staginc == DBNull.Value)
                                    {
                                        staginc = 0;
                                    }
                                    else
                                    {
                                        staginc = Convert.ToDecimal(sec5ds["amount"]);
                                    }
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Stagnation Incremements",
                                        //column2 = sec5ds["amount"].ToString(),
                                        column2 = ReportColConvertToDecimal(staginc.ToString()),

                                    });
                                    //staginc = Convert.ToDecimal(sec5ds["amount"]);

                                }
                                DataRow sec6ds;
                                if (ds.Tables[8].Rows.Count > 0)
                                {
                                    sec6ds = ds.Tables[8].Rows[0];
                                    object o_specpay = sec6ds["amount"];
                                    if (o_specpay == DBNull.Value)
                                    {
                                        specpay = 0;
                                    }
                                    else
                                    {
                                        specpay = Convert.ToDecimal(sec6ds["amount"]);
                                    }
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Special Pay",
                                        //column2 = sec6ds["amount"].ToString(),
                                        column2 = ReportColConvertToDecimal(specpay.ToString()),

                                    });
                                    //specpay = Convert.ToDecimal(sec6ds["amount"]);
                                }
                            }
                            DataRow drtds;
                            if (ds.Tables[2].Rows.Count > 0)
                            {
                                drtds = ds.Tables[2].Rows[0];
                                object o_salfixedperall =   drtds["sal_fixed_personal_allowance"];
                                if (o_salfixedperall == DBNull.Value)
                                {
                                    salfixedperall = 0;
                                }
                                else
                                {
                                    salfixedperall = Convert.ToDecimal(drtds["sal_fixed_personal_allowance"]);
                                }
                                if (salfixedperall != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Fixed Personal Allowance",
                                        //column2 = drtds["sal_fixed_personal_allowance"].ToString(),
                                        column2 = ReportColConvertToDecimal(salfixedperall.ToString()),

                                    });


                                }
                                //salfixedperall = Convert.ToDecimal(drtds["sal_fixed_personal_allowance"]);
                                object o_hraallo = drtds["sal_fpa_hra_allowance"];
                                if (o_hraallo == DBNull.Value)
                                {
                                    hraallo = 0;
                                }
                                else
                                {
                                    hraallo = Convert.ToDecimal(drtds["sal_fpa_hra_allowance"]);
                                }
                                if (hraallo != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "FPA-HRA Allowance",
                                        //column2 = drtds["sal_fpa_hra_allowance"].ToString(),
                                        column2 = ReportColConvertToDecimal(hraallo.ToString()),

                                    });
                                }

                                //hraallo = Convert.ToDecimal(drtds["sal_fpa_hra_allowance"]);
                                object o_salfpip = drtds["sal_fpiip"];
                                if (o_salfpip == DBNull.Value)
                                {
                                    salfpip = 0;
                                }
                                else
                                {
                                    salfpip = Convert.ToDecimal(drtds["sal_fpiip"]);
                                }
                                if (salfpip != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "FPIIP",
                                        //column2 = drtds["sal_fpiip"].ToString(),
                                        column2 = ReportColConvertToDecimal(salfpip.ToString()),

                                    });
                                }

                                //salfpip = Convert.ToDecimal(drtds["sal_fpiip"]);
                                object o_salda = drtds["sal_da"];
                                if (o_salda == DBNull.Value)
                                {
                                    salda = 0;
                                }
                                else
                                {
                                    salda = Convert.ToDecimal(drtds["sal_da"]);
                                }
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "DA",
                                    //column2 = drtds["sal_da"].ToString(),
                                    column2 = ReportColConvertToDecimal(salda.ToString()),

                                });
                                //salda = Convert.ToDecimal(drtds["sal_da"]);
                                object o_salhra = drtds["sal_hra"];
                                if (o_salhra == DBNull.Value)
                                {
                                    salhra = 0;
                                }
                                else
                                {
                                    salhra = Convert.ToDecimal(drtds["sal_hra"]);
                                }
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "HRA",
                                    //column2 = drtds["sal_hra"].ToString(),
                                    column2 = ReportColConvertToDecimal(salhra.ToString()),

                                });
                                //salhra = Convert.ToDecimal(drtds["sal_hra"]);
                                object o_salcca = drtds["sal_cca"];
                                if (o_salcca == DBNull.Value)
                                {
                                    salcca = 0;
                                }
                                else
                                {
                                    salcca = Convert.ToDecimal(drtds["sal_cca"]);
                                }
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "CCA",
                                    column2 = ReportColConvertToDecimal(drtds["sal_cca"].ToString()),
                                });
                                //salcca = Convert.ToDecimal(drtds["sal_cca"]);
                                object o_interrelief = drtds["sal_interim_relief"];
                                if (o_interrelief == DBNull.Value)
                                {
                                    interrelief = 0;
                                }
                                else
                                {
                                    interrelief = Convert.ToDecimal(drtds["sal_interim_relief"]);
                                }
                                if (interrelief != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "INTERIM RELIEF",
                                        //column2 = drtds["sal_interim_relief"].ToString(),
                                        column2 = ReportColConvertToDecimal(interrelief.ToString()),

                                    });
                                }

                                //interrelief = Convert.ToDecimal(drtds["sal_interim_relief"]);
                                object o_telincre = drtds["sal_telangana_increment"];
                                if (o_telincre == DBNull.Value)
                                {
                                    telincre = 0;
                                }
                                else
                                {
                                    telincre = Convert.ToDecimal(drtds["sal_telangana_increment"]);
                                }
                                if (telincre != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Telangana Increment",
                                        //column2 = drtds["sal_telangana_increment"].ToString(),
                                        column2 = ReportColConvertToDecimal(telincre.ToString()),

                                    });
                                }

                                //telincre = Convert.ToDecimal(drtds["sal_telangana_increment"]);
                                object o_splall = drtds["sal_spl_allow"];
                                if (o_splall == DBNull.Value)
                                {
                                    splall = 0;
                                }
                                else
                                {
                                    splall = Convert.ToDecimal(drtds["sal_spl_allow"]);
                                }
                                if (splall != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spl. Allow",
                                        //column2 = drtds["sal_spl_allow"].ToString(),
                                        column2 = ReportColConvertToDecimal(splall.ToString()),

                                    });
                                }

                                //splall = Convert.ToDecimal(drtds["sal_spl_allow"]);
                                object o_splda = drtds["sal_spcl_da"];
                                if (o_splda == DBNull.Value)
                                {
                                    splda = 0;
                                }
                                else
                                {
                                    splda = Convert.ToDecimal(drtds["sal_spcl_da"]);
                                }
                                if (splda != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Spcl. DA",
                                        //column2 = drtds["sal_spcl_da"].ToString(),
                                        column2 = ReportColConvertToDecimal(splda.ToString()),

                                    });
                                }

                                //splda = Convert.ToDecimal(drtds["sal_spcl_da"]);

                                if (dt_allowance.Rows.Count > 0)
                                {
                                    foreach (DataRow dr_allowtype in dt_allowance.Rows)
                                    {
                                        var v_allowtype = dr_allowtype["allowtype"].ToString();
                                        if (v_allowtype == "Exgratia")
                                        {
                                            object o_Exgratia = dr_allowtype["allowtype"];
                                            if (o_Exgratia == DBNull.Value)
                                            {
                                                Exgratia = 0;
                                            }
                                            else
                                            {
                                                Exgratia = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (Exgratia != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "Exgratia ",
                                                    //column2 = drtds["Exgratia"].ToString(),
                                                    column2 = ReportColConvertToDecimal(Exgratia.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "Medical Aid")
                                        {
                                            object o_Medical_Aid = dr_allowtype["allowtype"];
                                            if (o_Medical_Aid == DBNull.Value)
                                            {
                                                Medical_Aid = 0;
                                            }
                                            else
                                            {
                                                Medical_Aid = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Medical Aid ",
                                                //column2 = drtds["Medical_Aid"].ToString(),
                                                column2 = ReportColConvertToDecimal(Medical_Aid.ToString()),

                                            });
                                        }
                                        else if (v_allowtype == "LTC")
                                        {
                                            object o_ltc = dr_allowtype["allowtype"];
                                            if (o_ltc == DBNull.Value)
                                            {
                                                ltc = 0;
                                            }
                                            else
                                            {
                                                ltc = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (ltc != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "LTC",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(ltc.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "Interest On NSC (Earning)")
                                        {
                                            object o_interonnsc = dr_allowtype["allowtype"];
                                            if (o_interonnsc == DBNull.Value)
                                            {
                                                interonnsc = 0;
                                            }
                                            else
                                            {
                                                interonnsc = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (interonnsc != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "Interest On NSC (Earning)",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(interonnsc.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "codperks")
                                        {
                                            object o_codperk = dr_allowtype["allowtype"];
                                            if (o_codperk == DBNull.Value)
                                            {
                                                codperks = 0;
                                            }
                                            else
                                            {
                                                codperks = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (codperks != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "Code Perks",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(codperks.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "PERQPAY")
                                        {
                                            object o_perqpay = dr_allowtype["allowtype"];
                                            if (o_perqpay == DBNull.Value)
                                            {
                                                perqpay = 0;
                                            }
                                            else
                                            {
                                                perqpay = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (perqpay != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "Personal Qual Allowance",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(perqpay.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "PL Encashment")
                                        {
                                            object o_plencash = dr_allowtype["allowtype"];
                                            if (o_plencash == DBNull.Value)
                                            {
                                                plencash = 0;
                                            }
                                            else
                                            {
                                                plencash = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (plencash != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "PL Encashment",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(plencash.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "SP_ACSTI")
                                        {
                                            object o_spacsti = dr_allowtype["allowtype"];
                                            if (o_spacsti == DBNull.Value)
                                            {
                                                spacsti = 0;
                                            }
                                            else
                                            {
                                                spacsti = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (spacsti != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "SPL Spl.Alw.ACSTI",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(spacsti.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "BR_MGR")
                                        {
                                            object o_brmgr = dr_allowtype["allowtype"];
                                            if (o_brmgr == DBNull.Value)
                                            {
                                                brmgr = 0;
                                            }
                                            else
                                            {
                                                brmgr = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (brmgr != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "Br Manager Allowance",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(brmgr.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "STAGALLOW")
                                        {
                                            object o_stagincr = dr_allowtype["allowtype"];
                                            if (o_stagincr == DBNull.Value)
                                            {
                                                staginc = 0;
                                            }
                                            else
                                            {
                                                staginc = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (brmgr != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "Stagnation Increments",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(staginc.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "GRATUITY")
                                        {
                                            object o_gratuity = dr_allowtype["allowtype"];
                                            if (o_gratuity == DBNull.Value)
                                            {
                                                gratuity = 0;
                                            }
                                            else
                                            {
                                                gratuity = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (gratuity != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "GRATUITY",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(gratuity.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "SP_DAFTARI")
                                        {
                                            object o_spdaftari = dr_allowtype["allowtype"];
                                            if (o_spdaftari == DBNull.Value)
                                            {
                                                spdaftari = 0;
                                            }
                                            else
                                            {
                                                spdaftari = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (spdaftari != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "SPL Daftar",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(spdaftari.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "ENCASHMENT")
                                        {
                                            object o_dencash = dr_allowtype["allowtype"];
                                            if (o_dencash == DBNull.Value)
                                            {
                                                dencash = 0;
                                            }
                                            else
                                            {
                                                dencash = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (dencash != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "ENCASHMENT",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(dencash.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "PFPerks")
                                        {
                                            object o_pfperkear = dr_allowtype["allowtype"];
                                            if (o_pfperkear == DBNull.Value)
                                            {
                                                pfperksear = 0;
                                            }
                                            else
                                            {
                                                pfperksear = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (pfperksear != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "PFPerks",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(pfperksear.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "Leave Encashment")
                                        {
                                            object o_leaveencash = dr_allowtype["allowtype"];
                                            if (o_leaveencash == DBNull.Value)
                                            {
                                                leaveencash = 0;
                                            }
                                            else
                                            {
                                                leaveencash = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (leaveencash != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "Leave Encashment",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(leaveencash.ToString()),

                                                });
                                            }
                                        }
                                        else if (v_allowtype == "INCREMENT")
                                        {
                                            object o_increment = dr_allowtype["allowtype"];
                                            if (o_increment == DBNull.Value)
                                            {
                                                incrment = 0;
                                            }
                                            else
                                            {
                                                incrment = Convert.ToDecimal(dr_allowtype["amount"]);
                                            }
                                            if (incrment != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "Annual Increment",
                                                    //column2 = drtds["ltc"].ToString(),
                                                    column2 = ReportColConvertToDecimal(incrment.ToString()),

                                                });
                                            }
                                        }
                                    }
                                    //DataRow dr_exgratia = dt_allowance.Rows[0];


                                    //Exgratia = Convert.ToDecimal(drtds["Exgratia"]);

                                    //Medical_Aid = Convert.ToDecimal(drtds["Medical_Aid"]);
                                    //DataRow dr_medaid = dt_allowance.Rows[1];

                                    //DataRow dr_ltc = dt_allowance.Rows[2];

                                }
                                if (leaveencash == 0)
                                {
                                    DataRow dtencash;
                                    if (ds.Tables[9].Rows.Count > 0)
                                    {
                                        dtencash = ds.Tables[9].Rows[0];
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "Leave Encashment",
                                            column2 = ReportColConvertToDecimal(dtencash["encashamt"].ToString()),

                                        });
                                        encash = Convert.ToDecimal(dtencash["encashamt"]);
                                    }
                                    //else
                                    //{
                                    //    lst.Add(new CommonReportModel
                                    //    {
                                    //        RowId = rowid++,
                                    //        HRF = "R",
                                    //        column1 = "Leave Encashment",
                                    //        column2 = "0",

                                    //    });
                                    //}
                                }



                                if (pfperksear == 0)
                                {
                                    object o_loanperks = drtds["sal_pfperks"];
                                    if (o_loanperks == DBNull.Value)
                                    {
                                        loanperks = 0;
                                    }
                                    else
                                    {
                                        loanperks = Convert.ToDecimal(drtds["sal_pfperks"]);
                                    }
                                    if (loanperks != 0)
                                    {
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "PFPerks",
                                            //column2 = drtds["sal_pfperks"].ToString(),
                                            column2 = ReportColConvertToDecimal(loanperks.ToString()),

                                        });
                                    }
                                }

                                //loanperks = Convert.ToDecimal(drtds["sal_pfperks"]);

                            }



                            decimal? grosstotal = loanperks + encash + splda + telincre + splall + interrelief + salcca + salhra + salda + salfpip + hraallo + salfixedperall + basic + staginc + specpay + interonnsc + perqpay + plencash + spacsti + brmgr + gratuity + spdaftari + pfperksear + leaveencash + incrment + dencash;


                            if (ds.Tables[2].Rows.Count > 0)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "<span style = 'background-color:#FFFF99' >GROSS TOTAL INCOME </span>",

                                    column2 = ReportColConvertToDecimal(grosstotal.ToString()),

                                });
                                if (dt_opt.Rows.Count > 0)
                                {
                                    if (Convert.ToInt32(dt_opt.Rows[0]["option"]) == 1)
                                    {
                                        if (ds.Tables[2].Rows.Count > 0)
                                        {

                                            DataRow secded = ds.Tables[2].Rows[0];
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "<span style = 'color:#008000' >2. DEDUCTIONS </ span >",


                                            });
                                            object o_stdded = secded["standard_deductions"];
                                            decimal? standded = 0;
                                            if (o_stdded == DBNull.Value)
                                            {
                                                standded = 0;
                                            }
                                            else
                                            {
                                                standded = Convert.ToDecimal(secded["standard_deductions"]);
                                            }
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = " Standard Deduction",
                                                //column2 = secded["standard_deductions"].ToString(),
                                                column2 = ReportColConvertToDecimal(standded.ToString()),

                                            });
                                            object o_taxemp = secded["tax_of_employement"];
                                            decimal? taxemp = 0;
                                            if (o_taxemp == DBNull.Value)
                                            {
                                                taxemp = 0;
                                            }
                                            else
                                            {
                                                taxemp = Convert.ToDecimal(secded["tax_of_employement"]);
                                            }
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Tax on Employment",
                                                //column2 = secded["tax_of_employement"].ToString(),
                                                column2 = ReportColConvertToDecimal(taxemp.ToString()),

                                            });
                                            //if (Convert.ToDecimal(secded["interest_on_housing"]) > 0)
                                            //{
                                            object o_interhouse = secded["interest_on_housing"];
                                            decimal? interestonhouse = 0;
                                            if (o_interhouse == DBNull.Value)
                                            {
                                                interestonhouse = 0;
                                            }
                                            else
                                            {
                                                interestonhouse = Convert.ToDecimal(secded["interest_on_housing"]);
                                            }
                                            if (interestonhouse != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "Interest On Housing",
                                                    //column2 = secded["interest_on_housing"].ToString(),
                                                    column2 = ReportColConvertToDecimal(interestonhouse.ToString()),

                                                });
                                            }
                                            //}
                                            //if (Convert.ToDecimal(secded["house_rent_allowance"]) > 0)
                                            //{
                                            object o_houserentallow = secded["house_rent_allowance"];
                                            decimal? houserentallow = 0;
                                            if (o_houserentallow == DBNull.Value)
                                            {
                                                houserentallow = 0;
                                            }
                                            else
                                            {
                                                houserentallow = Convert.ToDecimal(secded["house_rent_allowance"]);
                                            }
                                            if (houserentallow != 0)
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = "House Rent Allowance",
                                                    //column2 = secded["house_rent_allowance"].ToString(),
                                                    column2 = ReportColConvertToDecimal(houserentallow.ToString()),

                                                });
                                            }

                                            //}
                                            if (ds.Tables[6].Rows.Count > 0)
                                            {
                                                DataRow sect4 = ds.Tables[6].Rows[0];
                                                if (Convert.ToDecimal(sect4["ded"]) > 0)
                                                {
                                                    foreach (DataRow sect41 in sec4.Rows)
                                                    {
                                                        //DataRow sect1 = ds.Tables[3].Rows[0];

                                                        lst.Add(new CommonReportModel
                                                        {
                                                            RowId = rowid++,
                                                            HRF = "R",
                                                            column1 = sect41["name"].ToString(),

                                                            column2 = ReportColConvertToDecimal(sect41["ded"].ToString()),

                                                        });
                                                        section_total = section_total + Convert.ToDecimal(sect41["ded"]);
                                                    }
                                                    //totalded = Convert.ToDecimal(secded["standard_deductions"]) + Convert.ToDecimal(secded["tax_of_employement"]) + Convert.ToDecimal(secded["interest_on_housing"]) + Convert.ToDecimal(secded["house_rent_allowance"]) + section_total;
                                                    totalded = standded + taxemp + interestonhouse + houserentallow + section_total;
                                                }
                                            }
                                            else
                                            {
                                                //totalded = Convert.ToDecimal(secded["standard_deductions"]) + Convert.ToDecimal(secded["tax_of_employement"]) + Convert.ToDecimal(secded["interest_on_housing"]) + Convert.ToDecimal(secded["house_rent_allowance"]);
                                                totalded = standded + taxemp + interestonhouse + houserentallow;
                                            }
                                            //decimal totalded = Convert.ToDecimal(secded["standard_deductions"]) + Convert.ToDecimal(secded["tax_of_employement"]) + Convert.ToDecimal(secded["interest_on_housing"]) + Convert.ToDecimal(secded["house_rent_allowance"]) + Convert.ToDecimal(secded["interest_on_housing"]) + Convert.ToDecimal(secded["house_rent_allowance"]);
                                            total = grosstotal - totalded;
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "<span style =  'background-color:#FFFF99' >TOTAL INCOME </ span >",
                                                column2 = ReportColConvertToDecimal(total.ToString()),

                                            });



                                        }
                                    }
                                    else if (Convert.ToInt32(dt_opt.Rows[0]["option"]) == 2)
                                    {
                                        total = grosstotal;
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "<span style =  'background-color:#FFFF99' >TOTAL INCOME </ span >",
                                            column2 = ReportColConvertToDecimal(total.ToString()),

                                        });
                                    }
                                }


                            }

                            if (dt_opt.Rows.Count > 0)
                            {
                                if (Convert.ToInt32(dt_opt.Rows[0]["option"]) == 1)
                                {
                                    if (ds.Tables[3].Rows.Count > 0)
                                    {
                                        lst.Add(new CommonReportModel
                                        {

                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "<span style = 'color:#008000' >LESS : DEDUCTIONS U/S 80(C) & 80(CCC) </ span >",




                                        });

                                        lst.Add(new CommonReportModel
                                        {

                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "(A) Sections 80C,80CCC and 80CCD",



                                        });
                                        lst.Add(new CommonReportModel
                                        {

                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "(a) Section 80C",
                                        });
                                    }

                                    if (ds.Tables[3].Rows.Count > 0)
                                    {
                                        foreach (DataRow sect1 in sec1.Rows)
                                        {
                                            //DataRow sect1 = ds.Tables[3].Rows[0];

                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = sect1["name"].ToString(),

                                                column2 = ReportColConvertToDecimal(sect1["ded"].ToString()),

                                            });
                                            section_total = section_total + Convert.ToDecimal(sect1["ded"]);
                                        }


                                        lst.Add(new CommonReportModel
                                        {

                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "b) Section 80CCC",
                                        });
                                        if (ds.Tables[4].Rows.Count > 0)
                                        {
                                            foreach (DataRow sect2 in sec2.Rows)
                                            {
                                                //DataRow sect2 = ds.Tables[4].Rows[0];
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = sect2["name"].ToString(),

                                                    column2 = ReportColConvertToDecimal(sect2["ded"].ToString()),

                                                });
                                                section_total = section_total + Convert.ToDecimal(sect2["ded"]);
                                            }




                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Total",
                                                column2 = ReportColConvertToDecimal(section_total.ToString()),


                                            });
                                        }
                                    }


                                    if (ds.Tables[10].Rows.Count > 0)
                                    {
                                        DataRow drhl2 = ds.Tables[10].Rows[0];
                                        object o_h2d = drhl2["hl2damount"];
                                        if (o_h2d == DBNull.Value)
                                        {
                                            h2d = 0;
                                        }
                                        else
                                        {
                                            h2d = Convert.ToDecimal(drhl2["hl2damount"]);
                                        }
                                        if (h2d != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = " Housing Addl.Loan - 2D    ",
                                                //column2 = drhl2["hl2damount"].ToString(),
                                                column2 = ReportColConvertToDecimal(h2d.ToString()),
                                            });
                                        }
                                    }


                                    if (ds.Tables[11].Rows.Count > 0)
                                    {
                                        DataRow drhl2 = ds.Tables[11].Rows[0];
                                        object o_hs1 = drhl2["house1"];
                                        if (o_hs1 == DBNull.Value)
                                        {
                                            hs1 = 0;
                                        }
                                        else
                                        {
                                            hs1 = Convert.ToDecimal(drhl2["house1"]);
                                        }
                                        if (hs1 != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "   Housing Loan 1 ",
                                                //column2 = drhl2["house1"].ToString(),
                                                column2 = ReportColConvertToDecimal(hs1.ToString()),

                                            });
                                        }
                                    }

                                    if (dt_house2.Rows.Count > 0)
                                    {
                                        DataRow dr_houseloan2 = dt_house2.Rows[0];
                                        object o_dhoul2 = dr_houseloan2["house2"];
                                        if (o_dhoul2 == DBNull.Value)
                                        {
                                            dhouse2 = 0;
                                        }
                                        else
                                        {
                                            dhouse2 = Convert.ToDecimal(dr_houseloan2["house2"]);
                                        }
                                        if (dhouse2 != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  Housing Loan - 2",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dhouse2.ToString()),

                                            });
                                        }
                                    }

                                    if (dt_house2a.Rows.Count > 0)
                                    {
                                        DataRow dr_house2a = dt_house2a.Rows[0];
                                        object o_dhoul2 = dr_house2a["house2a"];
                                        if (o_dhoul2 == DBNull.Value)
                                        {
                                            dhouse2a = 0;
                                        }
                                        else
                                        {
                                            dhouse2a = Convert.ToDecimal(dr_house2a["house2a"]);
                                        }
                                        if (dhouse2a != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  Housing Loan 2A",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dhouse2a.ToString()),

                                            });
                                        }
                                    }

                                    if (dt_houselncomm.Rows.Count > 0)
                                    {
                                        DataRow dr_houselncomm = dt_houselncomm.Rows[0];
                                        object o_dhouselncomm = dr_houselncomm["houselncomm"];
                                        if (o_dhouselncomm == DBNull.Value)
                                        {
                                            dhouseloancomm = 0;
                                        }
                                        else
                                        {
                                            dhouseloancomm = Convert.ToDecimal(dr_houselncomm["houselncomm"]);
                                        }
                                        if (dhouseloancomm != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  Housing Loan Commerical",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dhouseloancomm.ToString()),

                                            });
                                        }
                                    }

                                    if (dt_houselnplot.Rows.Count > 0)
                                    {
                                        DataRow dr_houselnplot = dt_houselnplot.Rows[0];
                                        object o_dhouselnplot = dr_houselnplot["houselnplot"];
                                        if (o_dhouselnplot == DBNull.Value)
                                        {
                                            dhouselnplot = 0;
                                        }
                                        else
                                        {
                                            dhouselnplot = Convert.ToDecimal(dr_houselnplot["houselnplot"]);
                                        }
                                        if (dhouselnplot != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  Housing Loan-Plot",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dhouselnplot.ToString()),

                                            });
                                        }
                                    }

                                    if (dt_houseln2.Rows.Count > 0)
                                    {
                                        DataRow dr_houseln2 = dt_houseln2.Rows[0];
                                        object o_dhouseln2 = dr_houseln2["houseln2"];
                                        if (o_dhouseln2 == DBNull.Value)
                                        {
                                            dhouseln2 = 0;
                                        }
                                        else
                                        {
                                            dhouseln2 = Convert.ToDecimal(dr_houseln2["houseln2"]);
                                        }
                                        if (dhouseln2 != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  Housing Loan 2",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dhouseln2.ToString()),

                                            });
                                        }
                                    }

                                    if (dt_houseln3.Rows.Count > 0)
                                    {
                                        DataRow dr_houseln3 = dt_houseln3.Rows[0];
                                        object o_dhouseln3 = dr_houseln3["houseln3"];
                                        if (o_dhouseln3 == DBNull.Value)
                                        {
                                            dhouseln3 = 0;
                                        }
                                        else
                                        {
                                            dhouseln3 = Convert.ToDecimal(dr_houseln3["houseln3"]);
                                        }
                                        if (dhouseln3 != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  Housing Loan 3",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dhouseln3.ToString()),

                                            });
                                        }
                                    }

                                    if (dt_houselnint.Rows.Count > 0)
                                    {
                                        DataRow dr_houselnint = dt_houselnint.Rows[0];
                                        object o_dhouselnint = dr_houselnint["houselnint"];
                                        if (o_dhouselnint == DBNull.Value)
                                        {
                                            dhouselnint = 0;
                                        }
                                        else
                                        {
                                            dhouselnint = Convert.ToDecimal(dr_houselnint["houselnint"]);
                                        }
                                        if (dhouseln3 != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  Housing Loan Int",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dhouselnint.ToString()),

                                            });
                                        }
                                    }

                                    if (pf.Rows.Count > 0)
                                    {
                                        DataRow drhl9 = pf.Rows[0];
                                        object o_pff = drhl9["pf"];
                                        if (o_pff == DBNull.Value)
                                        {
                                            pff = 0;
                                        }
                                        else
                                        {
                                            pff = Convert.ToDecimal(drhl9["pf"]);
                                        }
                                        if (pff != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  Provident Fund",
                                                //column2 = drhl2["pf"].ToString(),
                                                column2 = ReportColConvertToDecimal(pff.ToString()),

                                            });
                                        }
                                    }
                                    if (dt_vpf.Rows.Count > 0)
                                    {
                                        DataRow dr_vpf = dt_vpf.Rows[0];
                                        object o_vpf = dr_vpf["vpf"];
                                        if (o_vpf == DBNull.Value)
                                        {
                                            vpf = 0;
                                        }
                                        else
                                        {
                                            vpf = Convert.ToDecimal(dr_vpf["vpf"]);
                                        }
                                        if (vpf != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  VPF",
                                                //column2 = drhl2["pf"].ToString(),
                                                column2 = ReportColConvertToDecimal(vpf.ToString()),

                                            });
                                        }
                                    }

                                    //decimal dlic = 0;
                                    if (dt_lic.Rows.Count > 0)
                                    {
                                        DataRow dr_lic = dt_lic.Rows[0];
                                        object o_dlic = dr_lic["lic"];
                                        if (o_dlic == DBNull.Value)
                                        {
                                            dlic = 0;
                                        }
                                        else
                                        {
                                            dlic = Convert.ToDecimal(dr_lic["lic"]);
                                        }
                                        if (dlic != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  LIC",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dlic.ToString()),

                                            });
                                        }
                                    }

                                    if (dt_house2b2c.Rows.Count > 0)
                                    {
                                        DataRow dr_house2b2c = dt_house2b2c.Rows[0];
                                        object o_dhouse2b2c = dr_house2b2c["house2b2c"];
                                        if (o_dhouse2b2c == DBNull.Value)
                                        {
                                            dhouse2b2c = 0;
                                        }
                                        else
                                        {
                                            dhouse2b2c = Convert.ToDecimal(dr_house2b2c["house2b2c"]);
                                        }
                                        if (dhouse2b2c != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  Housing Loan 2B-2C",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dhouse2b2c.ToString()),

                                            });
                                        }
                                    }
                                    if (dt_deduct.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr_deduct in dt_deduct.Rows)
                                        {
                                            var v_deduct = dr_deduct["name"].ToString();
                                            if (v_deduct == "HDFC HOUSING LOAN PRINCIPLE")
                                            {
                                                object o_hdfchlprin = dr_deduct["name"];
                                                if (o_hdfchlprin == DBNull.Value)
                                                {
                                                    dhdfchlprincple = 0;
                                                }
                                                else
                                                {
                                                    dhdfchlprincple = Convert.ToDecimal(dr_deduct["amount"]);
                                                }
                                                if (dhdfchlprincple != 0)
                                                {
                                                    lst.Add(new CommonReportModel
                                                    {
                                                        RowId = rowid++,
                                                        HRF = "R",
                                                        column1 = "  HDFC HOUSING LOAN PRINCIPLE",
                                                        //column2 = dr_lic["lic"].ToString(),
                                                        column2 = ReportColConvertToDecimal(dhdfchlprincple.ToString()),

                                                    });
                                                }
                                            }
                                            else if (v_deduct == "TAX SAVER FD")
                                            {
                                                object o_taxsaverfd = dr_deduct["name"];
                                                if (o_taxsaverfd == DBNull.Value)
                                                {
                                                    dtaxsaverfd = 0;
                                                }
                                                else
                                                {
                                                    dtaxsaverfd = Convert.ToDecimal(dr_deduct["amount"]);
                                                }
                                                if (dtaxsaverfd != 0)
                                                {
                                                    lst.Add(new CommonReportModel
                                                    {
                                                        RowId = rowid++,
                                                        HRF = "R",
                                                        column1 = "  TAX SAVER FD",
                                                        //column2 = dr_lic["lic"].ToString(),
                                                        column2 = ReportColConvertToDecimal(dtaxsaverfd.ToString()),

                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }



                            decimal DEDUCTIONS = h2d + hs1 + pff + dlic + dhouse2b2c + dhouselnint + dhouseloancomm + dhouselnplot + dhouseln3 + dhouseln2 + dhouse2a + dhouse2 + vpf + dhdfchlprincple + dtaxsaverfd;
                            if (dt_opt.Rows.Count > 0)
                            {
                                if (Convert.ToInt32(dt_opt.Rows[0]["option"]) == 1)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "   ",
                                        column2 = ReportColConvertToDecimal(DEDUCTIONS.ToString()),
                                        column3 = "sdsd",
                                    });

                                    if (DEDUCTIONS > 150000)
                                    {
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "   ",
                                            column2 = ReportColConvertToDecimal(150000.ToString()),

                                        });
                                    }

                                }
                            }


                            //decimal? taxableincome = total - section_total;
                            if (ds.Tables[2].Rows.Count > 0)
                            {


                                DataRow drEmp1 = ds.Tables[2].Rows[0];

                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "<span style =  'background-color:#FFFF99' >TAXABLE INCOME</ span >",
                                    column2 = ReportColConvertToDecimal(drEmp1["tax_payable"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Income Tax",
                                    column2 = ReportColConvertToDecimal(drEmp1["tax_on_total_income"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = " Section 87A",
                                    column2 = ReportColConvertToDecimal(drEmp1["section_87a"].ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Education Cess",
                                    column2 = ReportColConvertToDecimal(drEmp1["education_cess"].ToString()),

                                });
                                decimal tottax = Convert.ToDecimal(drEmp1["tax_on_total_income"]) + Convert.ToDecimal(drEmp1["education_cess"]);
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "<span style =  'background-color:#FFFF99' >TOTAL TAX </ span >",
                                    column2 = ReportColConvertToDecimal(tottax.ToString()),

                                });
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "<span style =  'background-color:#FFFF99' >TAX DEDUCTED</ span >",
                                    //column2 = drEmp1["tds_per_month"].ToString(),
                                    column2 = ReportColConvertToDecimal(drEmp1["balance_tax"].ToString()),

                                });
                                //totaltax = Convert.ToDecimal(drEmp1["section_87a"]) + Convert.ToDecimal(drEmp1["education_cess"]);


                                //lst.Add(new CommonReportModel
                                //{
                                //    RowId = rowid++,
                                //    HRF = "R",
                                //    column1 = "<span style =  'background-color:#FFFF99' >TOTAL TAX </ span >",
                                //    column2 = totaltax.ToString(),

                                //});
                                //if (ds.Tables[1].Rows.Count > 0)
                                //{


                                //    DataRow drtime = ds.Tables[1].Rows[0];

                                //    lst.Add(new CommonReportModel
                                //    {
                                //        RowId = rowid++,
                                //        HRF = "R",

                                //        column1 = drtime["fm"].ToString(),
                                //        column2 = "DEPUTY GENERAL MANAGER",
                                //    });


                                //}
                            }

                        }

                    }
                }
             
            }

            else
            {
                string[] arrEmpId = EmpCode.Split(',');
                foreach (string empId in arrEmpId)
                {
                    string qry = "select id from pr_emp_tds_process where empcode=" + Convert.ToInt32(empId) + " and active=1";
                    DataTable checkDt = await _sha.Get_Table_FromQry(qry);

                    if (checkDt.Rows.Count > 0)
                    {

                        if (iFY == Eyears)
                        {
                            general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                   "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code, gen.designation" +
                                   " as designation,gen.sex as gender " +
                                   "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                                    "where gen.emp_code = " + Convert.ToInt32(empId) + ";";

                            daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                             "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                            tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                             " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                              " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                              "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                              " gross_salary, house_rent_allowance, total_of_sec10, " +
                                              " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                              "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                              " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                               " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                "balance_tax,balance_months,tds_per_month " +
                                               " from pr_emp_tds_process " +
                                               "where fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and " +
                                               "empcode =  " + Convert.ToInt32(empId) + " and active=1;";
                            section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Section80C' and ef.name not in('LIC','VPF','Provident Fund','Housing Loan Main','Housing Addl.Loan - 2D','GSLI') union all select 'Provident Fund' as name, gross , qual,ded " +
                                     "from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 " +
                                     " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2 " +
                                     " union all select 'LIC' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=3 " +
                                     " union all select 'Housing Loan Main' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=4 and gross>0 " +
                                    " union all select 'Housing Addl.Loan - 2D' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=5 and gross>0 " +
                                    " union all select 'GSLI' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=6 and gross>0 " +
                             " union all select 'Housing Loan 2B-2C' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=7 and gross>0 " +
                             " union all select 'Housing Loan 2A' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=8 and gross>0;";

                            section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                  "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                  "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                  "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                            section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                 "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                 "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                 "ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                            section4 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                 "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                 "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and  " +
                                 "ef.type = 'per_ded' and epf.section_type = 'Other'";

                            section5 = " select pe.amount as amount ,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Stagnation Increments' and pe.active=1 where  pe.emp_code = " + Convert.ToInt32(empId) + " and pe.fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            section6 = " select pe.amount as amount,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Special Pay' and pe.active=1 where pe.emp_code = " + Convert.ToInt32(empId) + " and pe.fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            section7 = "select gross_amount as encashamt from pr_emp_payslip where spl_type='Encashment' and active=1 and emp_code = " + Convert.ToInt32(empId) + " and fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            house2d = "select sum(principal_paid_amount) as hl2damount from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                                "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 7 " +
                                //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                                "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";

                            house1 = "select sum(principal_paid_amount) as house1 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                       "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 10 " +
                       //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                       "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                            house2 = "select sum(principal_paid_amount) as house2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                       "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 4 " +
                       //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                       "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                            house2a = "select sum(principal_paid_amount) as house2a from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                       "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 5 " +
                       //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                       "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            houselncomm = "select sum(principal_paid_amount) as houseloancomm from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                      "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 8 " +
                      //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                      "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            houselnplot = "select sum(principal_paid_amount) as houseloanplot from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                     "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 9 " +
                     //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                     "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            houseln2 = "select sum(principal_paid_amount) as houseloan2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                     "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 11 " +
                     //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                     "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            houseln3 = "select sum(principal_paid_amount) as houseloan3 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                     "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 12 " +
                     //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                     "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            houselnint = "select sum(principal_paid_amount) as houseloanint from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                    "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 13 " +
                    //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                    "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            pfamt = "select sum(dd_provident_fund)as pf from pr_emp_payslip where emp_code= " + Convert.ToInt32(empId) + "and fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                            //"//and fm <= concat('" + Eyears + "', ' -03-01')";
                            lic = "select sum(dd_amount) as lic from pr_emp_payslip_deductions where emp_code=" + Convert.ToInt32(empId) + " and dd_name='LIC' and payslip_mid in(select distinct id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and fy=" + Eyears + ")";
                            option = "Select [Option] from pr_tax_option_emp_wise where EmpId=" + Convert.ToInt32(empId) + ";";
                            house2b2c = "select sum(principal_paid_amount) as house2b2c from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                       "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 6 " +
                       //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                       "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                            str_allowance = "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id=ep.m_id where ep.emp_code=" + Convert.ToInt32(empId) + " and efm.name='Exgratia' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Medical Aid' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'LTC' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )   " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Interest On NSC (Earning)' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'codperks' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PERQPAY' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PL Encashment' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'SP_ACSTI' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'BR_MGR' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'GRATUITY' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'SP_DAFTARI' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PFPerks' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Leave Encashment' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name ='INCREMENT' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name ='STAGALLOW' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name ='ENCASHMENT' and ep.fm = concat('" + Eyears + "', ' -03-01') ";
                            strvpf = "select sum(dd_amount) as vpf from pr_emp_payslip_deductions where emp_code=" + Convert.ToInt32(empId) + " and dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and  fy=" + Eyears + ")";
                            strdeduct = " select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id=tdsded.m_id where tdsded.empcode=" + Convert.ToInt32(empId) + " and fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  and dedmas.id=310 " +
                            "union all " +
                            "select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id = tdsded.m_id where tdsded.empcode = " + Convert.ToInt32(empId) + " and fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and dedmas.id = 71 ";
                        }
                        else
                        {
                            general = "select CONCAT(emp.FirstName,' ',emp.LastName) as name, " +
                                   "per_address, gen.pan_no as pannumber,gen.emp_code as emp_code, gen.designation" +
                                   " as designation,gen.sex as gender " +
                                   "from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId " +
                                    "where gen.emp_code = " + Convert.ToInt32(empId) + ";";

                            daQry = "select distinct format(p.fm, 'yyyy-MM-dd') as fm, da_percent from pr_month_details m " +
                             "join pr_emp_payslip p on p.fm = (select  max(fm) from pr_emp_payslip) where m.active = 1; ";
                            tdsdetail = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                             " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                              " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                              "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                              " gross_salary, house_rent_allowance, total_of_sec10, " +
                                              " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                              "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                              " case when aggregate_of_deductible is null then 0 else aggregate_of_deductible end as aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                               " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                "balance_tax,balance_months,tds_per_month " +
                                               " from pr_emp_tds_process " +
                                               "where Month(fm)=03 and year(fm)=" + Eyears + " and " +
                                               "empcode =  " + Convert.ToInt32(empId) + " ";
                            section1 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                     "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                     "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                     "ef.type = 'per_ded' and epf.section_type = 'Section80C' and ef.name not in('LIC','VPF','Provident Fund','Housing Loan Main','Housing Addl.Loan - 2D','GSLI') union all select 'Provident Fund' as name, gross , qual,ded " +
                                     "from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 and Month(fm)=03 and year(fm)=" + Eyears + "" +
                                     " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2 and Month(fm)=03 and year(fm)=" + Eyears + "" +
                                     " union all select 'LIC' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=3 and Month(fm)=03 and year(fm)=" + Eyears + "" +
                                     " union all select 'Housing Loan Main' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=4 and gross>0 and Month(fm)=03 and year(fm)=" + Eyears + " " +
                                    " union all select 'Housing Addl.Loan - 2D' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=5 and gross>0 and Month(fm)=03 and year(fm)=" + Eyears + "" +
                                    " union all select 'GSLI' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=6 and gross>0 and Month(fm)=03 and year(fm)=" + Eyears + "" +
                                    " union all select 'Housing Loan 2B-2C' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=7 and gross>0 and Month(fm)=03 and year(fm)=" + Eyears + "" +
                                    " union all select 'Housing Loan 2A' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=8 and gross>0 and Month(fm)=03 and year(fm)=" + Eyears + ";";

                            section2 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                  "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                  "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                  "ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                            section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                 "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                 "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                 "ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                            section4 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                                 "from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                                 "and epf.empcode = " + Convert.ToInt32(empId) + " WHERE Month(fm)=03 and year(fm)=" + Eyears + " and  " +
                                 "ef.type = 'per_ded' and epf.section_type = 'Other'";

                            section5 = " select pe.amount as amount ,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Stagnation Increments' and pe.active=1 where  pe.emp_code = " + Convert.ToInt32(empId) + " and Month(pe.fm)=03 and year(fm)=" + Eyears + "";
                            section6 = " select pe.amount as amount,pe.emp_code from pr_emp_pay_field  pe join pr_earn_field_master p on p.id=pe.m_id and p.type='pay_fields' and p.name='Special Pay' and pe.active=1 where pe.emp_code = " + Convert.ToInt32(empId) + " and Month(pe.fm)=03 and year(pe.fm)=" + Eyears + "";
                            section7 = "select sum(gross_amount) as encashamt from pr_emp_payslip where spl_type='Encashment'  and emp_code = " + Convert.ToInt32(empId) + " and Month(fm)=03 and year(fm)=" + Eyears + "";
                            house2d = "select sum(principal_paid_amount) as hl2damount from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                                "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 7 " +
                                "and adj.fm <= concat('" + Eyears + "', ' -03-01')";

                            house1 = "select sum(principal_paid_amount) as house1 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                       "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 10 " +
                       "and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                            house2 = "select sum(principal_paid_amount) as house2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                       "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 4 " +
                       //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                       "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                            house2a = "select sum(principal_paid_amount) as house2a from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                       "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 5 " +
                       //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                       "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            houselncomm = "select sum(principal_paid_amount) as houseloancomm from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                      "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 8 " +
                      //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                      "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            houselnplot = "select sum(principal_paid_amount) as houseloanplot from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                     "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 9 " +
                     //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                     "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            houseln2 = "select sum(principal_paid_amount) as houseloan2 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                     "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 11 " +
                     //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                     "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            houseln3 = "select sum(principal_paid_amount) as houseloan3 from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                     "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 12 " +
                     //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                     "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            houselnint = "select sum(principal_paid_amount) as houseloanint from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                    "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 13 " +
                    //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                    "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )";
                            pfamt = "select sum(dd_provident_fund)as pf from pr_emp_payslip where emp_code= " + Convert.ToInt32(empId) + " and fm <= concat('" + Eyears + "', ' -03-01')";
                            lic = "select sum(dd_amount) as lic from pr_emp_payslip_deductions where emp_code=" + Convert.ToInt32(empId) + " and dd_name='LIC' and payslip_mid in(select distinct id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and fy=" + Eyears + ")";
                            option = "Select [Option] from pr_tax_option_emp_wise where EmpId=" + Convert.ToInt32(empId) + ";";
                            house2b2c = "select sum(principal_paid_amount) as house2b2c from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                       "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid = 6 " +
                       //"and adj.fm <= concat('" + Eyears + "', ' -03-01')";
                       "and adj.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";
                            str_allowance = "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id=ep.m_id where ep.emp_code=" + Convert.ToInt32(empId) + " and efm.name='Exgratia' and ep.fm=concat('" + Eyears + "', ' -03-01')" +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Medical Aid' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'LTC' and ep.fm = concat('" + Eyears + "', ' -03-01')  " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Interest On NSC (Earning)' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'codperks' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PERQPAY' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PL Encashment' and ep.fm = concat('" + Eyears + "', ' -03-01')" +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'SP_ACSTI' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'BR_MGR' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'GRATUITY' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'SP_DAFTARI' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'PFPerks' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name = 'Leave Encashment' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name ='INCREMENT' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name ='STAGALLOW' and ep.fm = concat('" + Eyears + "', ' -03-01') " +
                            "union all " +
                            "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id = ep.m_id where ep.emp_code = " + Convert.ToInt32(empId) + " and efm.name ='ENCASHMENT' and ep.fm = concat('" + Eyears + "', ' -03-01') ";
                            strvpf = "select sum(dd_amount) as vpf from pr_emp_payslip_deductions where emp_code=" + Convert.ToInt32(empId) + " and dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip where emp_code=" + Convert.ToInt32(empId) + " and  fy=" + Eyears + ")";
                            strdeduct = " select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id=tdsded.m_id where tdsded.empcode=" + Convert.ToInt32(empId) + " and fm=concat('" + Eyears + "', ' -03-01') and dedmas.id=310 " +
                            "union all " +
                            "select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id = tdsded.m_id where tdsded.empcode = " + Convert.ToInt32(empId) + " and fm = concat('" + Eyears + "', ' -03-01') and dedmas.id = 71 ";
                        }
                        DataSet ds = await _sha.Get_MultiTables_FromQry(general + daQry + tdsdetail + section1 + section2 + section3 + section4 + section5 + section6 + section7 + house2d + house1 + house2 + house2a + houselncomm + houselnplot + houseln2 + houseln3 + houselnint + pfamt + lic + option + house2b2c + str_allowance + strvpf + strdeduct);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtmon = ds.Tables[1];
                        DataTable dttds = ds.Tables[2];
                        DataTable sec1 = ds.Tables[3];
                        DataTable sec2 = ds.Tables[4];
                        DataTable sec3 = ds.Tables[5];
                        DataTable sec4 = ds.Tables[6];
                        // stagnation increment 
                        DataTable sec5 = ds.Tables[7];
                        //special pay
                        DataTable sec6 = ds.Tables[8];
                        //Encashment amount
                        DataTable sec7 = ds.Tables[9];


                        //House Loan
                        DataTable hl2d = ds.Tables[10];
                        DataTable hous1 = ds.Tables[11];
                        DataTable dt_house2 = ds.Tables[12];
                        DataTable dt_house2a = ds.Tables[13];
                        DataTable dt_houselncomm = ds.Tables[14];
                        DataTable dt_houselnplot = ds.Tables[15];
                        DataTable dt_houseln2 = ds.Tables[16];
                        DataTable dt_houseln3 = ds.Tables[17];
                        DataTable dt_houselnint = ds.Tables[18];
                        DataTable pf = ds.Tables[19];
                        DataTable dt_lic = ds.Tables[20];
                        DataTable dt_opt = ds.Tables[21];
                        DataTable dt_house2b2c = ds.Tables[22];
                        DataTable dt_allowance = ds.Tables[23];
                        DataTable dt_vpf = ds.Tables[24];
                        DataTable dt_deduct = ds.Tables[25];
                        if (ds.Tables[2].Rows.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DataRow drEmp = ds.Tables[0].Rows[0];



                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "H",
                                    //column1 = "<span style = 'background-color:#ADD8E6' >NAME OF THE EMPLOYEE: " + drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")" + "</span>",
                                    column1 = "<span style='color:#C8EAFB'>~</span>"
                                    //+ ReportColFooterlesscol(150, "Employee Name", drEmp["name"].ToString() + " " + "(" + (drEmp["emp_code"].ToString()) + ")")
                                    + ReportColFooterlesscol(0, "Emp Code", drEmp["emp_code"].ToString() )

                                    + ReportColFooterlesscol(2, "Emp Name", drEmp["name"].ToString() )

                                    + ReportColFooterlesscol(3, "Designation", drEmp["designation"].ToString() )
                                  + ReportColFooterlesscol(3, "Pan", drEmp["pannumber"].ToString() ),

                                    column2 = "`",
                                });


                            }


                        }


                        if (ds.Tables[2].Rows.Count > 0)
                        {

                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",

                                column1 = "<span style = 'color:#008000' >1. PARTICULARS OF SALARY INCOME </ span >",


                            });

                            DataRow drtds1 = ds.Tables[2].Rows[0];
                            object bas = drtds1["sal_basic"];
                            if (bas == DBNull.Value)
                            {
                                basic = 0;
                            }
                            else
                            {
                                basic = Convert.ToDecimal(drtds1["sal_basic"]);
                            }
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "Basic",
                                //column2 = drtds1["sal_basic"].ToString(),
                                column2 = ReportColConvertToDecimal(basic.ToString()),

                            });
                            //basic = Convert.ToDecimal(drtds1["sal_basic"]);


                            DataRow sec5ds;
                            if (ds.Tables[7].Rows.Count > 0)
                            {
                                sec5ds = ds.Tables[7].Rows[0];
                                object stag = sec5ds["amount"];
                                if (stag == DBNull.Value)
                                {
                                    staginc = 0;
                                }
                                else
                                {
                                    staginc = Convert.ToDecimal(sec5ds["amount"]);
                                }
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Stagnation Incremements",
                                    //column2 = sec5ds["amount"].ToString(),
                                    column2 = ReportColConvertToDecimal(staginc.ToString()),
                                });
                                //staginc = Convert.ToDecimal(sec5ds["amount"]);

                            }
                            DataRow sec6ds;
                            if (ds.Tables[8].Rows.Count > 0)
                            {
                                sec6ds = ds.Tables[8].Rows[0];
                                object spep = sec6ds["amount"];
                                if (spep == DBNull.Value)
                                {
                                    specpay = 0;
                                }
                                else
                                {
                                    specpay = Convert.ToDecimal(sec6ds["amount"]);
                                }
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Special Pay",
                                    //column2 = sec6ds["amount"].ToString(),
                                    column2 = ReportColConvertToDecimal(specpay.ToString()),

                                });
                                //specpay = Convert.ToDecimal(sec6ds["amount"]);
                            }
                        }
                        DataRow drtds;
                        if (ds.Tables[2].Rows.Count > 0)
                        {
                            drtds = ds.Tables[2].Rows[0];
                            object salfixpa = drtds["sal_fixed_personal_allowance"];
                            if (salfixpa == DBNull.Value)
                            {
                                salfixedperall = 0;
                            }
                            else
                            {
                                salfixedperall = Convert.ToDecimal(drtds["sal_fixed_personal_allowance"]);
                            }
                            if (salfixedperall != 0)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Fixed Personal Allowance",
                                    //column2 = drtds["sal_fixed_personal_allowance"].ToString(),
                                    column2 = ReportColConvertToDecimal(salfixedperall.ToString()),

                                });
                            }

                            //salfixedperall = Convert.ToDecimal(drtds["sal_fixed_personal_allowance"]);
                            object hraa = drtds["sal_fpa_hra_allowance"];
                            if (hraa == DBNull.Value)
                            {
                                hraallo = 0;
                            }
                            else
                            {
                                hraallo = Convert.ToDecimal(drtds["sal_fpa_hra_allowance"]);
                            }
                            if (hraallo != 0)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "FPA-HRA Allowance",
                                    //column2 = drtds["sal_fpa_hra_allowance"].ToString(),
                                    column2 = ReportColConvertToDecimal(hraallo.ToString()),

                                });
                            }

                            //hraallo = Convert.ToDecimal(drtds["sal_fpa_hra_allowance"]);
                            object o_salfpip = drtds["sal_fpiip"];
                            if (o_salfpip == DBNull.Value)
                            {
                                salfpip = 0;
                            }
                            else
                            {
                                salfpip = Convert.ToDecimal(drtds["sal_fpiip"]);
                            }
                            if (salfpip != 0)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "FPIIP",
                                    //column2 = drtds["sal_fpiip"].ToString(),
                                    column2 = ReportColConvertToDecimal(salfpip.ToString()),

                                });
                            }

                            //salfpip = Convert.ToDecimal(drtds["sal_fpiip"]);
                            object o_salda = drtds["sal_da"];
                            if (o_salda == DBNull.Value)
                            {
                                salda = 0;
                            }
                            else
                            {
                                salda = Convert.ToDecimal(drtds["sal_da"]);
                            }
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "DA",
                                //column2 = drtds["sal_da"].ToString(),
                                column2 = ReportColConvertToDecimal(salda.ToString()),

                            });
                            //salda = Convert.ToDecimal(drtds["sal_da"]);
                            object o_salhra = drtds["sal_hra"];
                            if (o_salhra == DBNull.Value)
                            {
                                salhra = 0;
                            }
                            else
                            {
                                salhra = Convert.ToDecimal(drtds["sal_hra"]);
                            }
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "HRA",
                                //column2 = drtds["sal_hra"].ToString(),
                                column2 = ReportColConvertToDecimal(salhra.ToString()),
                            });
                            //salhra = Convert.ToDecimal(drtds["sal_hra"]);
                            object o_salcca = drtds["sal_cca"];
                            if (o_salcca == DBNull.Value)
                            {
                                salcca = 0;
                            }
                            else
                            {
                                salcca = Convert.ToDecimal(drtds["sal_cca"]);
                            }
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "CCA",
                                //column2 = drtds["sal_cca"].ToString(),
                                column2 = ReportColConvertToDecimal(salcca.ToString()),
                            });
                            //salcca = Convert.ToDecimal(drtds["sal_cca"]);
                            object o_interrelief = drtds["sal_interim_relief"];
                            if (o_interrelief == DBNull.Value)
                            {
                                interrelief = 0;
                            }
                            else
                            {
                                interrelief = Convert.ToDecimal(drtds["sal_interim_relief"]);
                            }
                            if (interrelief != 0)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "INTERIM RELIEF",
                                    //column2 = drtds["sal_interim_relief"].ToString(),
                                    column2 = ReportColConvertToDecimal(interrelief.ToString()),

                                });
                            }

                            //interrelief = Convert.ToDecimal(drtds["sal_interim_relief"]);
                            object o_telincre = drtds["sal_telangana_increment"];
                            if (o_telincre == DBNull.Value)
                            {
                                telincre = 0;
                            }
                            else
                            {
                                telincre = Convert.ToDecimal(drtds["sal_telangana_increment"]);
                            }
                            if (telincre != 0)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Telangana Increment",
                                    //column2 = drtds["sal_telangana_increment"].ToString(),
                                    column2 = ReportColConvertToDecimal(telincre.ToString()),

                                });
                            }

                            //telincre = Convert.ToDecimal(drtds["sal_telangana_increment"]);
                            object o_splall = drtds["sal_spl_allow"];
                            if (o_splall == DBNull.Value)
                            {
                                splall = 0;
                            }
                            else
                            {
                                splall = Convert.ToDecimal(drtds["sal_spl_allow"]);
                            }
                            if (splall != 0)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Spl. Allow",
                                    //column2 = drtds["sal_spl_allow"].ToString(),
                                    column2 = ReportColConvertToDecimal(splall.ToString()),
                                });
                            }

                            //splall = Convert.ToDecimal(drtds["sal_spl_allow"]);
                            object o_splda = drtds["sal_spcl_da"];
                            if (o_splda == DBNull.Value)
                            {
                                splda = 0;
                            }
                            else
                            {
                                splda = Convert.ToDecimal(drtds["sal_spcl_da"]);
                            }
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "Spcl. DA",
                                column2 = ReportColConvertToDecimal(drtds["sal_spcl_da"].ToString()),

                            });
                            //splda = Convert.ToDecimal(drtds["sal_spcl_da"]);

                            //Exgratia = Convert.ToDecimal(drtds["Exgratia"]);

                            //Medical_Aid = Convert.ToDecimal(drtds["Medical_Aid"]);

                            ////codperks = Convert.ToDecimal(drtds["codperks"]);
                            //object o_codperks = drtds["codperks"];
                            //if (o_codperks == DBNull.Value)
                            //{
                            //    codperks = 0;
                            //}
                            //else
                            //{
                            //    codperks = Convert.ToDecimal(drtds["codperks"]);
                            //}
                            //lst.Add(new CommonReportModel
                            //{
                            //    RowId = rowid++,
                            //    HRF = "R",
                            //    column1 = "Cod perks  ",
                            //    //column2 = drtds["codperks"].ToString(),
                            //    column2 = codperks.ToString(),

                            //});
                            ////codperks = Convert.ToDecimal(drtds["codperks"]);

                            ////annual_inc = Convert.ToDecimal(drtds["annual_inc"]);
                            //object o_annual_inc = drtds["annual_inc"];
                            //if (o_annual_inc == DBNull.Value)
                            //{
                            //    annual_inc = 0;
                            //}
                            //else
                            //{
                            //    annual_inc = Convert.ToDecimal(drtds["annual_inc"]);
                            //}
                            //lst.Add(new CommonReportModel
                            //{
                            //    RowId = rowid++,
                            //    HRF = "R",
                            //    column1 = "Annual Increment",
                            //    //column2 = drtds["annual_inc"].ToString(),
                            //    column2 = annual_inc.ToString(),

                            //});
                            ////annual_inc = Convert.ToDecimal(drtds["annual_inc"]);


                            //ltc = Convert.ToDecimal(drtds["ltc"]);

                            if (dt_allowance.Rows.Count > 0)
                            {
                                foreach (DataRow dr_allowtype in dt_allowance.Rows)
                                {
                                    var v_allowtype = dr_allowtype["allowtype"].ToString();
                                    if (v_allowtype == "Exgratia")
                                    {
                                        object o_Exgratia = dr_allowtype["allowtype"];
                                        if (o_Exgratia == DBNull.Value)
                                        {
                                            Exgratia = 0;
                                        }
                                        else
                                        {
                                            Exgratia = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (Exgratia != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Exgratia ",
                                                //column2 = drtds["Exgratia"].ToString(),
                                                column2 = ReportColConvertToDecimal(Exgratia.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "Medical Aid")
                                    {
                                        object o_Medical_Aid = dr_allowtype["allowtype"];
                                        if (o_Medical_Aid == DBNull.Value)
                                        {
                                            Medical_Aid = 0;
                                        }
                                        else
                                        {
                                            Medical_Aid = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "Medical Aid ",
                                            //column2 = drtds["Medical_Aid"].ToString(),
                                            column2 = ReportColConvertToDecimal(Medical_Aid.ToString()),

                                        });
                                    }
                                    else if (v_allowtype == "LTC")
                                    {
                                        object o_ltc = dr_allowtype["allowtype"];
                                        if (o_ltc == DBNull.Value)
                                        {
                                            ltc = 0;
                                        }
                                        else
                                        {
                                            ltc = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (ltc != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "LTC",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(ltc.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "Interest On NSC (Earning)")
                                    {
                                        object o_interonnsc = dr_allowtype["allowtype"];
                                        if (o_interonnsc == DBNull.Value)
                                        {
                                            interonnsc = 0;
                                        }
                                        else
                                        {
                                            interonnsc = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (interonnsc != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Interest On NSC (Earning)",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(interonnsc.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "codperks")
                                    {
                                        object o_codperk = dr_allowtype["allowtype"];
                                        if (o_codperk == DBNull.Value)
                                        {
                                            codperks = 0;
                                        }
                                        else
                                        {
                                            codperks = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (codperks != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Code Perks",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(codperks.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "PERQPAY")
                                    {
                                        object o_perqpay = dr_allowtype["allowtype"];
                                        if (o_perqpay == DBNull.Value)
                                        {
                                            perqpay = 0;
                                        }
                                        else
                                        {
                                            perqpay = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (perqpay != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Personal Qual Allowance",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(perqpay.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "PL Encashment")
                                    {
                                        object o_plencash = dr_allowtype["allowtype"];
                                        if (o_plencash == DBNull.Value)
                                        {
                                            plencash = 0;
                                        }
                                        else
                                        {
                                            plencash = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (plencash != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "PL Encashment",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(plencash.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "SP_ACSTI")
                                    {
                                        object o_spacsti = dr_allowtype["allowtype"];
                                        if (o_spacsti == DBNull.Value)
                                        {
                                            spacsti = 0;
                                        }
                                        else
                                        {
                                            spacsti = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (spacsti != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "SPL Spl.Alw.ACSTI",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(spacsti.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "BR_MGR")
                                    {
                                        object o_brmgr = dr_allowtype["allowtype"];
                                        if (o_brmgr == DBNull.Value)
                                        {
                                            brmgr = 0;
                                        }
                                        else
                                        {
                                            brmgr = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (brmgr != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Br Manager Allowance",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(brmgr.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "STAGALLOW")
                                    {
                                        object o_staginc = dr_allowtype["allowtype"];
                                        if (o_staginc == DBNull.Value)
                                        {
                                            staginc = 0;
                                        }
                                        else
                                        {
                                            staginc = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (staginc != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Stagnation Increments",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(staginc.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "GRATUITY")
                                    {
                                        object o_gratuity = dr_allowtype["allowtype"];
                                        if (o_gratuity == DBNull.Value)
                                        {
                                            gratuity = 0;
                                        }
                                        else
                                        {
                                            gratuity = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (gratuity != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "GRATUITY",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(gratuity.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "SP_DAFTARI")
                                    {
                                        object o_spdaftari = dr_allowtype["allowtype"];
                                        if (o_spdaftari == DBNull.Value)
                                        {
                                            spdaftari = 0;
                                        }
                                        else
                                        {
                                            spdaftari = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (spdaftari != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "SPL Daftar",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(spdaftari.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "ENCASHMENT")
                                    {
                                        object o_dencash = dr_allowtype["allowtype"];
                                        if (o_dencash == DBNull.Value)
                                        {
                                            dencash = 0;
                                        }
                                        else
                                        {
                                            dencash = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (dencash != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "ENCASHMENT",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(dencash.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "PFPerks")
                                    {
                                        object o_pfperkear = dr_allowtype["allowtype"];
                                        if (o_pfperkear == DBNull.Value)
                                        {
                                            pfperksear = 0;
                                        }
                                        else
                                        {
                                            pfperksear = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (pfperksear != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "PFPerks",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(pfperksear.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "Leave Encashment")
                                    {
                                        object o_leaveencash = dr_allowtype["allowtype"];
                                        if (o_leaveencash == DBNull.Value)
                                        {
                                            leaveencash = 0;
                                        }
                                        else
                                        {
                                            leaveencash = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (leaveencash != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Leave Encashment",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(leaveencash.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_allowtype == "INCREMENT")
                                    {
                                        object o_increment = dr_allowtype["allowtype"];
                                        if (o_increment == DBNull.Value)
                                        {
                                            incrment = 0;
                                        }
                                        else
                                        {
                                            incrment = Convert.ToDecimal(dr_allowtype["amount"]);
                                        }
                                        if (incrment != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "Annual Increment",
                                                //column2 = drtds["ltc"].ToString(),
                                                column2 = ReportColConvertToDecimal(incrment.ToString()),

                                            });
                                        }
                                    }
                                }
                                //DataRow dr_exgratia = dt_allowance.Rows[0];


                                //Exgratia = Convert.ToDecimal(drtds["Exgratia"]);

                                //Medical_Aid = Convert.ToDecimal(drtds["Medical_Aid"]);
                                //DataRow dr_medaid = dt_allowance.Rows[1];

                                //DataRow dr_ltc = dt_allowance.Rows[2];

                            }

                            //ltc = Convert.ToDecimal(drtds["ltc"]);
                            if (leaveencash == 0)
                            {

                                DataRow dtencash;
                                if (ds.Tables[9].Rows.Count > 0)
                                {
                                    dtencash = ds.Tables[9].Rows[0];
                                    object o_encash = dtencash["encashamt"];
                                    if (o_encash == DBNull.Value)
                                    {
                                        encash = 0;
                                    }
                                    else
                                    {
                                        encash = Convert.ToDecimal(dtencash["encashamt"]);
                                    }
                                    //if (dtencash != null)
                                    if (encash != 0)
                                    {
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "Leave Encashment",
                                            column2 = ReportColConvertToDecimal(encash.ToString()),

                                        });
                                        //encash = Convert.ToDecimal(dtencash["encashamt"]);
                                    }
                                }
                            }
                            //else
                            //{
                            //    lst.Add(new CommonReportModel
                            //    {
                            //        RowId = rowid++,
                            //        HRF = "R",
                            //        column1 = "Leave Encashment",
                            //        column2 = "0",

                            //    });
                            //}

                            if (pfperksear == 0)
                            {
                                object o_pfperks = drtds["sal_pfperks"];
                                if (o_pfperks == DBNull.Value)
                                {
                                    pfperks = 0;
                                }
                                else
                                {
                                    pfperks = Convert.ToDecimal(drtds["sal_pfperks"]);
                                }
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "PFPerks",
                                    //column2 = drtds["sal_pfperks"].ToString(),
                                    column2 = ReportColConvertToDecimal(pfperks.ToString()),

                                });
                            }

                            //pfperks = Convert.ToDecimal(drtds["sal_pfperks"]);

                            object o_loanperks = drtds["sal_loanperks"];
                            if (o_loanperks == DBNull.Value)
                            {
                                loanperks = 0;
                            }
                            else
                            {
                                loanperks = Convert.ToDecimal(drtds["sal_loanperks"]);
                            }
                            if (loanperks != 0)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Loan Perks",
                                    //column2 = drtds["sal_loanperks"].ToString(),
                                    column2 = ReportColConvertToDecimal(loanperks.ToString()),

                                });
                            }

                            //loanperks = Convert.ToDecimal(drtds["sal_loanperks"]);

                        }


                        decimal? grosstotal = splda + telincre + splall + interrelief + salcca + salhra + salda + salfpip + hraallo + salfixedperall + basic + staginc + specpay + Exgratia + Medical_Aid + codperks + pfperks + loanperks + annual_inc + ltc + encash + interonnsc + perqpay + plencash + spacsti + brmgr + gratuity + spdaftari + pfperksear + leaveencash + incrment + dencash;


                        if (ds.Tables[2].Rows.Count > 0)
                        {
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "<span style = 'background-color:#FFFF99' >GROSS TOTAL INCOME </span>",

                                column2 = ReportColConvertToDecimal(grosstotal.ToString()),

                            });
                            if (Convert.ToInt32(dt_opt.Rows[0]["option"]) == 1)
                            {
                                if (ds.Tables[2].Rows.Count > 0)
                                {

                                    DataRow secded = ds.Tables[2].Rows[0];
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "<span style = 'color:#008000' >2. DEDUCTIONS </ span >",


                                    });
                                    object o_stndded = secded["standard_deductions"];
                                    decimal? stand_ded = 0;
                                    if (o_stndded == DBNull.Value)
                                    {
                                        stand_ded = 0;
                                    }
                                    else
                                    {
                                        stand_ded = Convert.ToDecimal(secded["standard_deductions"]);
                                    }
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = " Standard Deduction",
                                        //column2 = secded["standard_deductions"].ToString(),
                                        column2 = ReportColConvertToDecimal(stand_ded.ToString()),

                                    });
                                    object o_taxemp = secded["tax_of_employement"];
                                    decimal? taxemp = 0;
                                    if (o_taxemp == DBNull.Value)
                                    {
                                        taxemp = 0;
                                    }
                                    else
                                    {
                                        taxemp = Convert.ToDecimal(secded["tax_of_employement"]);
                                    }
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "Tax on Employment",
                                        //column2 = secded["tax_of_employement"].ToString(),
                                        column2 = ReportColConvertToDecimal(taxemp.ToString()),

                                    });

                                    //if (Convert.ToDecimal(secded["interest_on_housing"]) > 0)
                                    //{
                                    object o_inthouse = secded["interest_on_housing"];
                                    decimal? interonhouse = 0;
                                    if (o_inthouse == DBNull.Value)
                                    {
                                        interonhouse = 0;
                                    }
                                    else
                                    {
                                        interonhouse = Convert.ToDecimal(secded["interest_on_housing"]);
                                    }
                                    if (interonhouse != 0)
                                    {
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "Interest On Housing",
                                            //column2 = secded["interest_on_housing"].ToString(),
                                            column2 = ReportColConvertToDecimal(interonhouse.ToString()),

                                        });
                                    }

                                    //}
                                    //if (Convert.ToDecimal(secded["house_rent_allowance"]) > 0)
                                    //{
                                    object o_hourtall = secded["house_rent_allowance"];
                                    decimal? houserentallow = 0;
                                    if (o_hourtall == DBNull.Value)
                                    {
                                        houserentallow = 0;
                                    }
                                    else
                                    {
                                        houserentallow = Convert.ToDecimal(secded["house_rent_allowance"]);
                                    }
                                    if (houserentallow != 0)
                                    {
                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = rowid++,
                                            HRF = "R",
                                            column1 = "House Rent Allowance",
                                            //column2 = secded["house_rent_allowance"].ToString(),
                                            column2 = ReportColConvertToDecimal(houserentallow.ToString()),

                                        });
                                    }

                                    //}
                                    if (ds.Tables[6].Rows.Count > 0)
                                    {
                                        DataRow sect4 = ds.Tables[6].Rows[0];
                                        if (Convert.ToDecimal(sect4["ded"]) > 0)
                                        {
                                            foreach (DataRow sect41 in sec4.Rows)
                                            {
                                                //DataRow sect1 = ds.Tables[3].Rows[0];

                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = rowid++,
                                                    HRF = "R",
                                                    column1 = sect41["name"].ToString(),

                                                    column2 = ReportColConvertToDecimal(sect41["ded"].ToString()),

                                                });
                                                section_total = section_total + Convert.ToDecimal(sect41["ded"]);
                                            }
                                            //totalded = Convert.ToDecimal(secded["standard_deductions"]) + Convert.ToDecimal(secded["tax_of_employement"]) + Convert.ToDecimal(secded["interest_on_housing"]) + Convert.ToDecimal(secded["house_rent_allowance"]) + section_total;
                                            totalded = stand_ded + taxemp + interonhouse + houserentallow + section_total;
                                        }
                                    }
                                    else
                                    {
                                        //totalded = Convert.ToDecimal(secded["standard_deductions"]) + Convert.ToDecimal(secded["tax_of_employement"]) + Convert.ToDecimal(secded["interest_on_housing"]) + Convert.ToDecimal(secded["house_rent_allowance"]);
                                        totalded = stand_ded + taxemp + interonhouse + houserentallow;
                                    }

                                    total = grosstotal - totalded;
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "<span style =  'background-color:#FFFF99' >TOTAL INCOME </ span >",
                                        column2 = ReportColConvertToDecimal(total.ToString()),

                                    });



                                }
                            }
                            else if (Convert.ToInt32(dt_opt.Rows[0]["option"]) == 2)
                            {
                                total = grosstotal;
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "<span style =  'background-color:#FFFF99' >TOTAL INCOME </ span >",
                                    column2 = ReportColConvertToDecimal(total.ToString()),

                                });
                            }

                        }
                        if (Convert.ToInt32(dt_opt.Rows[0]["option"]) == 1)
                        {
                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "<span style = 'color:#008000' >LESS : DEDUCTIONS U/S 80(C) & 80(CCC) </ span >",
                            });

                            lst.Add(new CommonReportModel
                            {

                                RowId = rowid++,
                                HRF = "R",
                                column1 = "(A) Sections 80C,80CCC and 80CCD",
                            });
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "(a) Section 80C",
                            });


                            if (ds.Tables[3].Rows.Count > 0)
                            {
                                foreach (DataRow sect1 in sec1.Rows)
                                {
                                    //DataRow sect1 = ds.Tables[3].Rows[0];

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = sect1["name"].ToString(),

                                        column2 = ReportColConvertToDecimal(sect1["ded"].ToString()),

                                    });
                                    section_total = section_total + Convert.ToDecimal(sect1["ded"]);
                                }


                                lst.Add(new CommonReportModel
                                {

                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "b) Section 80CCC",
                                });

                                foreach (DataRow sect2 in sec2.Rows)
                                {
                                    //DataRow sect2 = ds.Tables[4].Rows[0];
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = sect2["name"].ToString(),

                                        column2 = ReportColConvertToDecimal(sect2["ded"].ToString()),

                                    });
                                    section_total = section_total + Convert.ToDecimal(sect2["ded"]);
                                }




                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "Total",
                                    column2 = ReportColConvertToDecimal(section_total.ToString()),


                                });
                            }
                            //decimal? taxableincome = total - section_total;
                            //decimal h2d = 0;
                            if (ds.Tables[10].Rows.Count > 0)
                            {
                                DataRow drhl2 = ds.Tables[10].Rows[0];
                                object o_houaddlo2d = drhl2["hl2damount"];
                                if (o_houaddlo2d == DBNull.Value)
                                {
                                    h2d = 0;
                                }
                                else
                                {
                                    h2d = Convert.ToDecimal(drhl2["hl2damount"]);
                                }
                                if (h2d != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = " Housing Addl.Loan - 2D    ",
                                        //column2 = drhl2["hl2damount"].ToString(),
                                        column2 = ReportColConvertToDecimal(h2d.ToString()),

                                    });
                                }

                            }
                            //decimal h2d = Convert.ToDecimal(hl2d.Rows[0]["hl2damount"]);
                            //decimal hs1 = 0;
                            if (ds.Tables[11].Rows.Count > 0)
                            {
                                DataRow drhl2 = ds.Tables[11].Rows[0];
                                object o_hs1 = drhl2["house1"];
                                if (o_hs1 == DBNull.Value)
                                {
                                    hs1 = 0;
                                }
                                else
                                {
                                    hs1 = Convert.ToDecimal(drhl2["house1"]);
                                }
                                if (hs1 != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "   Housing Loan 1 ",
                                        //column2 = drhl2["house1"].ToString(),
                                        column2 = ReportColConvertToDecimal(hs1.ToString()),

                                    });
                                }
                            }
                            //decimal dhouse2 = 0;
                            if (dt_house2.Rows.Count > 0)
                            {
                                DataRow dr_houseloan2 = dt_house2.Rows[0];
                                object o_dhoul2 = dr_houseloan2["house2"];
                                if (o_dhoul2 == DBNull.Value)
                                {
                                    dhouse2 = 0;
                                }
                                else
                                {
                                    dhouse2 = Convert.ToDecimal(dr_houseloan2["house2"]);
                                }
                                if (dhouse2 != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  Housing Loan - 2",
                                        //column2 = dr_lic["lic"].ToString(),
                                        column2 = ReportColConvertToDecimal(dhouse2.ToString()),

                                    });
                                }
                            }
                            //decimal dhouse2a = 0;
                            if (dt_house2a.Rows.Count > 0)
                            {
                                DataRow dr_house2a = dt_house2a.Rows[0];
                                object o_dhoul2 = dr_house2a["house2a"];
                                if (o_dhoul2 == DBNull.Value)
                                {
                                    dhouse2a = 0;
                                }
                                else
                                {
                                    dhouse2a = Convert.ToDecimal(dr_house2a["house2a"]);
                                }
                                if (dhouse2a != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  Housing Loan 2A",
                                        //column2 = dr_lic["lic"].ToString(),
                                        column2 = ReportColConvertToDecimal(dhouse2a.ToString()),

                                    });
                                }
                            }
                            //decimal dhouseloancomm = 0;
                            if (dt_houselncomm.Rows.Count > 0)
                            {
                                DataRow dr_houselncomm = dt_houselncomm.Rows[0];
                                object o_dhouselncomm = dr_houselncomm["houseloancomm"];
                                if (o_dhouselncomm == DBNull.Value)
                                {
                                    dhouseloancomm = 0;
                                }
                                else
                                {
                                    dhouseloancomm = Convert.ToDecimal(dr_houselncomm["houseloancomm"]);
                                }
                                if (dhouseloancomm != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  Housing Loan Commerical",
                                        //column2 = dr_lic["lic"].ToString(),
                                        column2 = ReportColConvertToDecimal(dhouseloancomm.ToString()),

                                    });
                                }
                            }
                            //decimal dhouselnplot = 0;
                            if (dt_houselnplot.Rows.Count > 0)
                            {
                                DataRow dr_houselnplot = dt_houselnplot.Rows[0];
                                object o_dhouselnplot = dr_houselnplot["houseloanplot"];
                                if (o_dhouselnplot == DBNull.Value)
                                {
                                    dhouselnplot = 0;
                                }
                                else
                                {
                                    dhouselnplot = Convert.ToDecimal(dr_houselnplot["houseloanplot"]);
                                }
                                if (dhouselnplot != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  Housing Loan-Plot",
                                        //column2 = dr_lic["lic"].ToString(),
                                        column2 = ReportColConvertToDecimal(dhouselnplot.ToString()),

                                    });
                                }
                            }
                            //decimal dhouseln2 = 0;
                            if (dt_houseln2.Rows.Count > 0)
                            {
                                DataRow dr_houseln2 = dt_houseln2.Rows[0];
                                object o_dhouseln2 = dr_houseln2["houseloan2"];
                                if (o_dhouseln2 == DBNull.Value)
                                {
                                    dhouseln2 = 0;
                                }
                                else
                                {
                                    dhouseln2 = Convert.ToDecimal(dr_houseln2["houseloan2"]);
                                }
                                if (dhouseln2 != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  Housing Loan 2",
                                        //column2 = dr_lic["lic"].ToString(),
                                        column2 = ReportColConvertToDecimal(dhouseln2.ToString()),

                                    });
                                }
                            }
                            //decimal dhouseln3 = 0;
                            if (dt_houseln3.Rows.Count > 0)
                            {
                                DataRow dr_houseln3 = dt_houseln3.Rows[0];
                                object o_dhouseln3 = dr_houseln3["houseloan3"];
                                if (o_dhouseln3 == DBNull.Value)
                                {
                                    dhouseln3 = 0;
                                }
                                else
                                {
                                    dhouseln3 = Convert.ToDecimal(dr_houseln3["houseloan3"]);
                                }
                                if (dhouseln3 != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  Housing Loan 3",
                                        //column2 = dr_lic["lic"].ToString(),
                                        column2 = ReportColConvertToDecimal(dhouseln3.ToString()),

                                    });
                                }
                            }
                            //decimal dhouselnint = 0;
                            if (dt_houselnint.Rows.Count > 0)
                            {
                                DataRow dr_houselnint = dt_houselnint.Rows[0];
                                object o_dhouselnint = dr_houselnint["houseloanint"];
                                if (o_dhouselnint == DBNull.Value)
                                {
                                    dhouselnint = 0;
                                }
                                else
                                {
                                    dhouselnint = Convert.ToDecimal(dr_houselnint["houseloanint"]);
                                }
                                if (dhouseln3 != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  Housing Loan Int",
                                        //column2 = dr_lic["lic"].ToString(),
                                        column2 = ReportColConvertToDecimal(dhouselnint.ToString()),

                                    });
                                }
                            }
                            //decimal hs1 = Convert.ToDecimal(hous1.Rows[0]["house1"]);
                            //decimal pff = 0;
                            if (pf.Rows.Count > 0)
                            {
                                DataRow drhl9 = pf.Rows[0];
                                object o_pff = drhl9["pf"];
                                if (o_pff == DBNull.Value)
                                {
                                    pff = 0;
                                }
                                else
                                {
                                    pff = Convert.ToDecimal(drhl9["pf"]);
                                }
                                if (pff != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  Provident Fund",
                                        //column2 = drhl2["pf"].ToString(),
                                        column2 = ReportColConvertToDecimal(pff.ToString()),

                                    });
                                }
                            }
                            if (dt_vpf.Rows.Count > 0)
                            {
                                DataRow dr_vpf = dt_vpf.Rows[0];
                                object o_vpf = dr_vpf["vpf"];
                                if (o_vpf == DBNull.Value)
                                {
                                    vpf = 0;
                                }
                                else
                                {
                                    vpf = Convert.ToDecimal(dr_vpf["vpf"]);
                                }
                                if (vpf != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  VPF",
                                        //column2 = drhl2["pf"].ToString(),
                                        column2 = ReportColConvertToDecimal(vpf.ToString()),

                                    });
                                }
                            }
                            //decimal pff = Convert.ToDecimal(pf.Rows[0]["pf"]);

                            //decimal dlic = 0;
                            if (dt_lic.Rows.Count > 0)
                            {
                                DataRow dr_lic = dt_lic.Rows[0];
                                object o_dlic = dr_lic["lic"];
                                if (o_dlic == DBNull.Value)
                                {
                                    dlic = 0;
                                }
                                else
                                {
                                    dlic = Convert.ToDecimal(dr_lic["lic"]);
                                }
                                if (dlic != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  LIC",
                                        //column2 = dr_lic["lic"].ToString(),
                                        column2 = ReportColConvertToDecimal(dlic.ToString()),

                                    });
                                }
                            }
                            //decimal dhouse2b2c = 0;
                            if (dt_house2b2c.Rows.Count > 0)
                            {
                                DataRow dr_house2b2c = dt_house2b2c.Rows[0];
                                object o_dhouse2b2c = dr_house2b2c["house2b2c"];
                                if (o_dhouse2b2c == DBNull.Value)
                                {
                                    dhouse2b2c = 0;
                                }
                                else
                                {
                                    dhouse2b2c = Convert.ToDecimal(dr_house2b2c["house2b2c"]);
                                }
                                if (dhouse2b2c != 0)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "  Housing Loan 2B-2C",
                                        //column2 = dr_lic["lic"].ToString(),
                                        column2 = ReportColConvertToDecimal(dhouse2b2c.ToString()),

                                    });
                                }
                            }
                            if (dt_deduct.Rows.Count > 0)
                            {
                                foreach (DataRow dr_deduct in dt_deduct.Rows)
                                {
                                    var v_deduct = dr_deduct["name"].ToString();
                                    if (v_deduct == "HDFC HOUSING LOAN PRINCIPLE")
                                    {
                                        object o_hdfchlprin = dr_deduct["name"];
                                        if (o_hdfchlprin == DBNull.Value)
                                        {
                                            dhdfchlprincple = 0;
                                        }
                                        else
                                        {
                                            dhdfchlprincple = Convert.ToDecimal(dr_deduct["amount"]);
                                        }
                                        if (dhdfchlprincple != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  HDFC HOUSING LOAN PRINCIPLE",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dhdfchlprincple.ToString()),

                                            });
                                        }
                                    }
                                    else if (v_deduct == "TAX SAVER FD")
                                    {
                                        object o_taxsaverfd = dr_deduct["name"];
                                        if (o_taxsaverfd == DBNull.Value)
                                        {
                                            dtaxsaverfd = 0;
                                        }
                                        else
                                        {
                                            dtaxsaverfd = Convert.ToDecimal(dr_deduct["amount"]);
                                        }
                                        if (dtaxsaverfd != 0)
                                        {
                                            lst.Add(new CommonReportModel
                                            {
                                                RowId = rowid++,
                                                HRF = "R",
                                                column1 = "  TAX SAVER FD",
                                                //column2 = dr_lic["lic"].ToString(),
                                                column2 = ReportColConvertToDecimal(dtaxsaverfd.ToString()),

                                            });
                                        }
                                    }
                                }
                            }
                        }

                        decimal DEDUCTIONS = h2d + hs1 + pff + dlic + dhouse2b2c + dhouselnint + dhouseloancomm + dhouselnplot + dhouseln3 + dhouseln2 + dhouse2a + dhouse2 + vpf + dhdfchlprincple + dtaxsaverfd;

                        if (Convert.ToInt32(dt_opt.Rows[0]["option"]) == 1)
                        {
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "   ",
                                column2 = AddZerosAfterDecimal(DEDUCTIONS.ToString()),
                                column3 = "sdsd",
                            });

                            if (DEDUCTIONS > 150000)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "   ",
                                    column2 = ReportColConvertToDecimal(150000.ToString()),

                                });
                            }

                        }




                        if (ds.Tables[2].Rows.Count > 0)
                        {
                            DataRow drEmp1 = ds.Tables[2].Rows[0];

                            if (Convert.ToInt32(dt_opt.Rows[0]["option"]) == 1)
                            {
                                if (DEDUCTIONS > 150000)
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "<span style =  'background-color:#FFFF99' >TAXABLE INCOME</ span >",
                                        column2 = ReportColConvertToDecimal((total - 150000).ToString()),

                                    });
                                }
                                else
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = rowid++,
                                        HRF = "R",
                                        column1 = "<span style =  'background-color:#FFFF99' >TAXABLE INCOME</ span >",
                                        column2 = ReportColConvertToDecimal((total - DEDUCTIONS).ToString()),

                                    });
                                }


                            }
                            else if (Convert.ToInt32(dt_opt.Rows[0]["option"]) == 2)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = rowid++,
                                    HRF = "R",
                                    column1 = "<span style =  'background-color:#FFFF99' >TAXABLE INCOME</ span >",
                                    column2 = ReportColConvertToDecimal((total).ToString()),

                                });
                            }

                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "Income Tax",
                                column2 = ReportColConvertToDecimal(drEmp1["tax_on_total_income"].ToString()),

                            });
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = " Section 87A",
                                column2 = ReportColConvertToDecimal(drEmp1["section_87a"].ToString()),

                            });
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "Education Cess",
                                column2 = ReportColConvertToDecimal(drEmp1["education_cess"].ToString()),

                            });
                            decimal tottax = Convert.ToDecimal(drEmp1["tax_on_total_income"]) + Convert.ToDecimal(drEmp1["education_cess"]);
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "<span style =  'background-color:#FFFF99' >TOTAL TAX </ span >",
                                column2 = ReportColConvertToDecimal(tottax.ToString()),

                            });
                            lst.Add(new CommonReportModel
                            {
                                RowId = rowid++,
                                HRF = "R",
                                column1 = "<span style =  'background-color:#FFFF99' >TAX DEDUCTED</ span >",
                                //column2 = drEmp1["tds_per_month"].ToString(),
                                column2 = ReportColConvertToDecimal(drEmp1["balance_tax"].ToString()),

                            });
                            //totaltax = Convert.ToDecimal(drEmp1["section_87a"]) + Convert.ToDecimal(drEmp1["education_cess"]);


                            //lst.Add(new CommonReportModel
                            //{
                            //    RowId = rowid++,
                            //    HRF = "R",
                            //    column1 = "<span style =  'background-color:#FFFF99' >TOTAL TAX </ span >",
                            //    column2 = totaltax.ToString(),

                            //});
                            //if (ds.Tables[1].Rows.Count > 0)
                            //{


                            //    DataRow drtime = ds.Tables[1].Rows[0];

                            //    lst.Add(new CommonReportModel
                            //    {
                            //        RowId = rowid++,
                            //        HRF = "R",

                            //        column1 = drtime["fm"].ToString(),
                            //        column2 = "DEPUTY GENERAL MANAGER",
                            //    });


                            //}
                        }

                    }
                }

            }
            return lst;
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

public string ReportColFooterAlign(string value)
        {
            string sRet = "";
            if (value == "")
            {
                value = "0";
            }
            decimal Drvalue = Convert.ToDecimal(value.ToString()) + 0.00M;
            decimal DPT = Convert.ToDecimal(String.Format("{0:0.00}", Drvalue));
            string NwDPT = String.Format("{0:n}", DPT);

            sRet += "<span style='float:right'>" + NwDPT + "</span>";

            return sRet;
        }


        public string ReportColFooterAlignleft(string value)
        {
            string sRet = "";
        
            sRet += "<span style='float:left'>" + value + "</span>";

            return sRet;
        }


    }
}

