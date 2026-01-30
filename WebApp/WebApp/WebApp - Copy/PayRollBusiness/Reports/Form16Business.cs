using System;
using System.Threading.Tasks;
using PayrollModels;

using System.Globalization;
using System.Data;
using System.Linq;
using Mavensoft.DAL.Business;
using System.Collections.Generic;

namespace PayRollBusiness.Reports
{
    public class Form16Business : BusinessBase
    {
        public Form16Business(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        //drop down financial year eg:2019-2020
        public async Task<IList<LICReport>> getFy()
        {
            string qryfy = "select fy as fm_fy from pr_month_details where active=1;";
            int fm_fy = await _sha.Run_INS_ExecuteScalar(qryfy);

            IList<LICReport> fyear = new List<LICReport>();

            int fy = fm_fy - 1;
            int Id = 0;

            fyear.Add(new LICReport
            {
                Id = Id.ToString(),
                fY = "Select",

            });

            Id++;
            fyear.Add(new LICReport
            {

                Id = Id.ToString(),
                fY = (fy + "-" + (fm_fy)).ToString(),

            });

            for (int i = 1; i < 6; i++)
            {
                Id++;
                fm_fy--;
                fy--;
                fyear.Add(new LICReport
                {
                    Id = Id.ToString(),
                    fY = (fy + "-" + (fm_fy)).ToString(),

                });
            }

            return fyear;


        }
        IList<form16Model1> lst = new List<form16Model1>();

        public async Task<IList<form16Model1>> GetForm16empData(string empcode, string fYear)
        {
            int Eyear = 0001;
            int Fyear = 0001;
            if (fYear != null && fYear != "")
            {
                string[] split1 = fYear.Split('-');
                //Console.Write(split[0], split[1]);
                Eyear = int.Parse(split1[1]);
                Fyear = int.Parse(split1[1]) - 1;
            }
            if (empcode == "")
            {
                empcode = "0";
            }

            string Qry1 = "";
            string oldemp = "";
            string newemp = "";

            if (empcode == "All")
            {
                Qry1 = "select distinct emp_code,CONCAT(emp.FirstName,' ',emp.LastName) as name,D.Name as designation, fy, p.trans_id  from pr_emp_payslip P join Employees emp on p.emp_code = EmpId join Designations D on D.Id= emp.CurrentDesignation  " +
                     "where P.fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) ";

                DataTable dt = await _sha.Get_Table_FromQry(Qry1);

                foreach (DataRow dr in dt.Rows)
                {
                    newemp = dr["emp_code"].ToString();
                    if ((oldemp == "" || oldemp != "") && newemp != oldemp)
                    {
                        lst.Add(new form16Model1
                        {
                            Id = dr["trans_id"].ToString(),
                            emp_code = dr["emp_code"].ToString(),
                            shortname = dr["name"].ToString(),
                            Description = dr["designation"].ToString(),
                            fy = dr["fy"].ToString(),

                        });
                    }
                    oldemp = dr["emp_code"].ToString();
                }
                return lst;
            }
            else
            {
                Qry1 = "select emp_code,CONCAT(emp.FirstName,' ',emp.LastName) as name,D.Name as designation, fy, p.trans_id  from pr_emp_payslip P join Employees emp on p.emp_code = EmpId join Designations D on D.Id= emp.CurrentDesignation  " +
                    " where P.fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and p.emp_code= " + empcode;

                DataTable dt = await _sha.Get_Table_FromQry(Qry1);

                foreach (DataRow dr in dt.Rows)
                {
                    newemp = dr["emp_code"].ToString();
                    if ((oldemp == "" || oldemp != "") && newemp != oldemp)
                    {
                        lst.Add(new form16Model1
                        {
                            Id = dr["trans_id"].ToString(),
                            emp_code = dr["emp_code"].ToString(),
                            shortname = dr["name"].ToString(),
                            Description = dr["designation"].ToString(),
                            fy = dr["fy"].ToString(),

                        });
                    }
                    oldemp = dr["emp_code"].ToString();
                }
                return lst;
            }
        }

        public async Task<Form16Model> GetForm16Data(string empId, string fYear)
        {
            //IList<Form16Model> lst = new List<Form16Model>();
            Form16Model fmodel = new Form16Model();
            int Eyear = 0001;
            int Fyear = 0001;
            int AssesmentFyear;
            int AssesmentEyear;
            string AssesmentYear="";
            decimal GrossAmount = 0;
            decimal houserentallow = 0;
            decimal balance = 0;
            if (fYear != null && fYear != " ")
            {
                string[] split1 = fYear.Split('-');
                //Console.Write(split[0], split[1]);
                Eyear = int.Parse(split1[1]);
                Fyear = int.Parse(split1[1]) - 1;
                AssesmentFyear = Fyear + 1;
                AssesmentEyear = Eyear + 1;
                AssesmentYear = AssesmentFyear + "-" + AssesmentEyear;
            }
            if (empId.Contains(" "))
            {
                empId = "0";
                fYear = "1900";
            }

            if (empId != "All")
            {

                string empdetail = "select CONCAT(emp.FirstName,' ',emp.LastName) as name," +
                    "region_for_p_tax as per_address, emp.PanCardNo as pannumber,gen.emp_code as emp_code, gen.designation" +
                    " as designation,gen.sex as gender, p.gross_amount as gross, p.deductions_amount as deductionamt, p.fm as financial_year " +
                    " from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId join pr_emp_payslip p on p.emp_code = gen.emp_code" +
                    " where  gen.emp_code= " + empId + " and p.fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 )";

                string tdsdetails = " select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment," +
                    " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                    " sal_value_of_perquisites, sal_profits_in_lieu_of_salary,gross_salary, house_rent_allowance, total_of_sec10, " +
                    " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income," +
                    " aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess,tax_payable,tax_deducted_at_source,tax_paid_by_the_employer," +
                    " balance_tax,balance_months,tds_per_month from pr_emp_tds_process where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and empcode= " + empId + " and sal_basic is not null";

                string D1 = " select COALESCE(sum(dd_income_tax)  , 0) as Quarter1   from pr_emp_payslip where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Fyear + ", 06, 30 ) and emp_code= " + empId + "";

                string D2 = " select COALESCE(sum(dd_income_tax)  , 0) as Quarter2 from pr_emp_payslip where fm between DATEFROMPARTS(" + Fyear + ", 07, 01 ) and DATEFROMPARTS(" + Fyear + ", 09, 30 ) and emp_code= " + empId + "";

                string D3 = " select COALESCE(sum(dd_income_tax)  , 0) as Quarter3 from pr_emp_payslip where fm between DATEFROMPARTS(" + Fyear + ", 10, 01 ) and DATEFROMPARTS(" + Fyear + ", 12, 31 ) and emp_code= " + empId + "";

                string D4 = " select COALESCE(sum(dd_income_tax)  , 0) as Quarter4 from pr_emp_payslip where fm between DATEFROMPARTS(" + Eyear + ", 01, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and emp_code= " + empId + "";

                string total_Dedu = "select sum(dd_income_tax) from pr_emp_payslip where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and emp_code= " + empId + "";

                string section1 = " select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                            " from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                            " and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 )" +
                            "  and ef.type = 'per_ded' and epf.section_type = 'Section80C' union all select 'Provident Fund' as name, gross , qual,ded " +
                            " from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=1 " +
                            " union all select 'VPF' as name, gross , qual,ded from pr_emp_tds_section_deductions where active = 1 and empcode = " + Convert.ToInt32(empId) + " and m_id=2;";

                string section2 = " select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                      " from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                      " and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and  ef.type = 'per_ded' and epf.section_type = 'Section80CCC'";
                string section3 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.ded as ded  " +
                     " from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                     " and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 )" +
                     " and ef.type = 'per_ded' and epf.section_type = 'Section80CCD'";
                string section4 = "select ef.name as name, epf.gross as gross,epf.qual as qual,epf.section_type as section,epf.ded as ded  " +
                     " from pr_deduction_field_master ef left outer join pr_emp_tds_section_deductions epf on ef.id = epf.m_id and epf.active = 1 " +
                     " and epf.empcode = " + Convert.ToInt32(empId) + " WHERE fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) " +
                     " and ef.type = 'per_ded' and epf.section_type = 'Other'";

                string sanctionAuthority = "select ShortName,FatherName,D.name,e.PresentAddress,PanCardNo ,format(GETDATE(),'dd-MM-yyyy') as date from employees e join Designations D on e.CurrentDesignation=D.Id where e.id=" +
                    "(select SanctioningAuthority from Employees where EmpId = " + empId + ")";
                string bsrcode = "select payment_date,bsrcode_of_bank,challan_no from pr_form16_codes where payment_date between DATEFROMPARTS(" + Eyear + ", 03, 31 ) and  DATEFROMPARTS(" + (Eyear + 1) + ", 03, 31 )  ";

                string deduction_monthwise = "select fm,dd_income_tax from pr_emp_payslip where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and emp_code=" + empId + " order by fm";
                string deduction_date = "";


                DataSet ds = await _sha.Get_MultiTables_FromQry(empdetail + tdsdetails + D1 + D2 + D3 + D4 + total_Dedu + section1 + section2 + section3 + section4 + sanctionAuthority + bsrcode + deduction_monthwise);
                DataTable empdt = ds.Tables[0];
                DataTable tdsdt = ds.Tables[1];
                DataTable D1dt = ds.Tables[2];
                DataTable D2dt = ds.Tables[3];
                DataTable D3dt = ds.Tables[4];
                DataTable D4dt = ds.Tables[5];
                DataTable totDedDt = ds.Tables[6];
                DataTable sect1 = ds.Tables[7];
                DataTable sect2 = ds.Tables[8];
                DataTable sect3 = ds.Tables[9];
                DataTable sect4 = ds.Tables[10];
                DataTable sancAuth = ds.Tables[11];
                DataTable bsrdt = ds.Tables[12];
                DataTable month_dedu = ds.Tables[13];


                try
                {
                    GrossAmount = GetGrossAmount(empId, Fyear.ToString(), Eyear.ToString());
                    IList<monthwiseDeductions> deductionList = new List<monthwiseDeductions>();
                    fmodel.deductions = deductionList;

                    IList<section80Cform16> section80C = new List<section80Cform16>();
                    section80C = GetSectionDetails(empId, Fyear.ToString(), Eyear.ToString());
                    fmodel.sect1 = section80C;

                    foreach (DataRow dr in empdt.Rows)
                    {
                        fmodel.EmpCode = dr["emp_code"].ToString();
                        fmodel.EmpName = dr["name"].ToString();
                        fmodel.Designation = dr["designation"].ToString();
                        //fmodel.fy = fYear;
                        fmodel.fy = AssesmentYear;
                        fmodel.city = dr["per_address"].ToString();
                        fmodel.PAN = dr["pannumber"].ToString();

                    }

                    foreach (DataRow drtds in tdsdt.Rows)
                    {
                        //fmodel.gross_Salaryaspercontainedsec17 = drtds["gross_salary"].ToString();
                        fmodel.gross_Salaryaspercontainedsec17 = AddZerosAfterDecimal(GrossAmount.ToString());
                        fmodel.Valueofperquisitiesu17 = AddZerosAfterDecimal(drtds["sal_value_of_perquisites"].ToString());
                        fmodel.Profitslieuofsalary17 = AddZerosAfterDecimal(drtds["sal_profits_in_lieu_of_salary"].ToString());
                        fmodel.HouseRentAllowance = AddZerosAfterDecimal(drtds["house_rent_allowance"].ToString());
                        fmodel.TotalSection1017 = AddZerosAfterDecimal(drtds["total_of_sec10"].ToString());
                        fmodel.Balance1_2 = AddZerosAfterDecimal(drtds["balance_gross_min_sec10"].ToString());
                        fmodel.StandardDeduction = AddZerosAfterDecimal(drtds["standard_deductions"].ToString());
                        fmodel.Tax_on_Employment = AddZerosAfterDecimal(drtds["tax_of_employement"].ToString());
                        fmodel.Aggregate = AddZerosAfterDecimal(drtds["tds_aggregate"].ToString());
                        fmodel.Income_Charg_Under_Salaries = AddZerosAfterDecimal(drtds["income_chargeable_bal_minus_agg"].ToString());
                        fmodel.Reported_by_Employee = AddZerosAfterDecimal(drtds["other_income_by_the_emp"].ToString());
                        fmodel.Gross_Total_Income = AddZerosAfterDecimal(drtds["gross_total_income"].ToString());
                        fmodel.Aggregate_amount_Under_ChapterVIA = AddZerosAfterDecimal(drtds["aggregate_of_deductible"].ToString());
                        fmodel.TotalIncome8_10 = AddZerosAfterDecimal(drtds["total_income"].ToString());
                        fmodel.Tax_on_Total_Income = AddZerosAfterDecimal(drtds["tax_on_total_income"].ToString());
                        fmodel.Section87A = AddZerosAfterDecimal(drtds["section_87a"].ToString());
                        fmodel.EducationCESS = AddZerosAfterDecimal(drtds["education_cess"].ToString());
                        fmodel.Tax_payable = AddZerosAfterDecimal(drtds["tax_payable"].ToString());
                        fmodel.Tax_deducted_Source = AddZerosAfterDecimal(drtds["tax_deducted_at_source"].ToString());
                        fmodel.Tax_paid_employer = AddZerosAfterDecimal(drtds["tax_paid_by_the_employer"].ToString());
                        fmodel.Tax_Payable_Refundable = AddZerosAfterDecimal(drtds["balance_tax"].ToString().ToString());
                        fmodel.Income_from_House_Property_Interest_on_Housing_Loan = AddZerosAfterDecimal(drtds["interest_on_housing"].ToString());
                    }
                    
                    //foreach (DataRow drsec in sect1.Rows)
                    //{
                    //    section80C.Add(new section80Cform16
                    //    {
                    //        type = drsec["name"].ToString(),
                    //        sect1_grss = drsec["gross"].ToString(),
                    //        sect1_qual = drsec["qual"].ToString(),
                    //        sect1_dedu = drsec["ded"].ToString()
                    //    });
                    //}
                    foreach (DataRow drsec in sect2.Rows)
                    {
                        fmodel.sect2_grss = drsec["gross"].ToString();
                        fmodel.sect2_qual = drsec["qual"].ToString();
                        fmodel.sect2_dedu = drsec["ded"].ToString();
                    }
                    foreach (DataRow drsec in sect3.Rows)
                    {
                        fmodel.sect3_grss = drsec["gross"].ToString();
                        fmodel.sect3_qual = drsec["qual"].ToString();
                        fmodel.sect3_dedu = drsec["ded"].ToString();
                    }
                    foreach (DataRow drsec in sect4.Rows)
                    {
                        fmodel.sect4_grss = drsec["gross"].ToString();
                        fmodel.sect4_qual = drsec["qual"].ToString();
                        fmodel.sect4_dedu = drsec["ded"].ToString();
                    }
                    foreach (DataRow d1 in D1dt.Rows)
                    {
                        fmodel.Quarter1 = d1["Quarter1"].ToString();
                    }
                    foreach (DataRow d1 in D2dt.Rows)
                    {
                        fmodel.Quarter2 = d1["Quarter2"].ToString();
                    }
                    foreach (DataRow d1 in D3dt.Rows)
                    {
                        fmodel.Quarter3 = d1["Quarter3"].ToString();
                    }
                    foreach (DataRow d1 in D4dt.Rows)
                    {
                        fmodel.Quarter4 = d1["Quarter4"].ToString();
                    }

                    foreach (DataRow sandr in sancAuth.Rows)
                    {
                        fmodel.sanctName = sandr["ShortName"].ToString();
                        fmodel.sanctFantherName = sandr["FatherName"].ToString();
                        fmodel.sanctDesignation = sandr["name"].ToString();
                        fmodel.sanctDate = sandr["PresentAddress"].ToString();
                        fmodel.sanctpalce = sandr["date"].ToString();
                        fmodel.sancPAN = sandr["PanCardNo"].ToString();
                    }
                    foreach (DataRow dr in bsrdt.Rows)
                    {
                        fmodel.bsrcode = dr["bsrcode_of_bank"].ToString();
                        fmodel.paymentdate = dr["payment_date"].ToString();
                        fmodel.challanno = dr["challan_no"].ToString();
                    }

                    string oldfm = "";
                    string newfm = "";
                    int dedu = 0;
                    foreach (DataRow drdd in month_dedu.Rows)
                    {
                        dedu = 0;
                        newfm = drdd["fm"].ToString();
                        var months = month_dedu.Rows.Cast<DataRow>().Where(x => x["fm"].ToString() == newfm);
                        if (newfm != oldfm)
                        {
                            foreach (DataRow dr in months)
                            {



                                dedu = dedu + Convert.ToInt32(dr["dd_income_tax"]);

                                oldfm = drdd["fm"].ToString();
                            }
                            deductionList.Add(new monthwiseDeductions
                            {
                                fm = drdd["fm"].ToString(),
                                dedu_amount = dedu.ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }

            return fmodel;
        }

        public async Task<string> store_logindata(int empid, string source, DateTime logintime, string page)
        {
            try
            {

                //DateTime str = logintime;
                //string login_time = str.ToString("yyyy-MM-dd hh:mm");
                string qryIns = "" +
                    "INSERT INTO pr_hrms_payslipdetails(empid,source_name,logintime,page_name) "
                     + "VALUES(" + empid + ",'" + source + "','" + logintime + "','" + page + "')";

                int id = await _sha.Run_INS_ExecuteScalar(qryIns);
            }
            catch (Exception e)
            {

            }
            return "";
            //return View();
        }
        public decimal GetGrossAmount(string empId, string Fyears, string Eyears)
        {
            Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();

            string tdsdetails = "";
            string allowtypes = "";
            string strvpf = "";
            string strdeduct = "";
            string strpfamt = "";

            decimal? d_tdsTotal = 0;
            decimal? d_allowanceTotal = 0;
            decimal d_GrossAmount = 0;
            //salary allowances
            decimal? d_sal_basic = 0;
            decimal? d_sal_fpa = 0;
            decimal? d_sal_housrentallow = 0;
            decimal? d_sal_fpiip = 0;
            decimal? d_sal_da = 0;
            decimal? d_sal_hra = 0;
            decimal? d_sal_cca = 0;
            decimal? d_sal_interim_relief = 0;
            decimal? d_tgincrement = 0;
            decimal? d_sal_spl_allow = 0;
            decimal? d_sal_spcl_da = 0;
            decimal? d_sal_pfperks = 0;
            decimal? d_sal_loanperks = 0;
            decimal? d_sal_incentive = 0;

            //special allowances
            decimal? d_Exgratia = 0;
            decimal? d_Medical_Aid = 0;
            decimal? d_LTC = 0;
            decimal? d_Interest_On_NSC = 0;
            decimal? d_codperks = 0;
            decimal? d_PERQPAY = 0;
            decimal? d_PL_Encash = 0;
            decimal? d_SP_ACSTI = 0;
            decimal? d_BR_MGR = 0;
            decimal? d_GRATUITY = 0;
            decimal? d_SP_DAFTARI = 0;
            decimal? d_PFPerks = 0;
            decimal? d_Leave_Encash = 0;
            decimal? d_Increment = 0;
            decimal? d_STAGALLOW = 0;
            decimal? d_ENCASHMENT = 0;
            try
            {
                tdsdetails = "select sal_basic, sal_fixed_personal_allowance, sal_fpa_hra_allowance, " +
                                             " sal_fpiip, sal_da, sal_hra, sal_cca, sal_interim_relief, sal_telangana_increment, " +
                                              " sal_spl_allow, sal_spcl_da, sal_pfperks, sal_loanperks, sal_incentive, " +
                                              "sal_value_of_perquisites, sal_profits_in_lieu_of_salary, " +
                                              " gross_salary, house_rent_allowance, total_of_sec10, " +
                                              " balance_gross_min_sec10,standard_deductions, tax_of_employement, tds_aggregate, " +
                                              "income_chargeable_bal_minus_agg, other_income_by_the_emp, interest_on_housing, gross_total_income, " +
                                              " aggregate_of_deductible,total_income,tax_on_total_income,section_87a,education_cess, " +
                                               " tax_payable,tax_deducted_at_source,tax_paid_by_the_employer, " +
                                                "balance_tax,balance_months,tds_per_month " +
                                               " from pr_emp_tds_process " +
                                               "where fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and " +
                                               "empcode =  " + Convert.ToInt32(empId) + " and active=1;";
                allowtypes = "select efm.name as allowtype,ep.amount as amount from pr_emp_perearning ep join pr_earn_field_master efm on efm.id=ep.m_id where ep.emp_code=" + Convert.ToInt32(empId) + " and efm.name='Exgratia' and ep.fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) " +
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
                strdeduct = " select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id=tdsded.m_id where tdsded.empcode=" + Convert.ToInt32(empId) + " and fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 )  and dedmas.id=272 " +
                "union all " +
                "select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id = tdsded.m_id where tdsded.empcode = " + Convert.ToInt32(empId) + " and fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and dedmas.id = 71 ";
                strpfamt = "select sum(dd_provident_fund)as pf from pr_emp_payslip where emp_code= " + Convert.ToInt32(empId) + "and fm between  DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) ";

                DataSet ds_Gross = sh.Get_MultiTables_FromQry(tdsdetails + allowtypes + strvpf + strdeduct + strpfamt);
                DataTable dt_tdsdetails = ds_Gross.Tables[0];
                DataTable dt_allowtypes = ds_Gross.Tables[1];
                DataTable dt_strvpf = ds_Gross.Tables[2];
                DataTable dt_strdeduct = ds_Gross.Tables[3];
                DataTable dt_strpfamt = ds_Gross.Tables[4];

                if (dt_tdsdetails.Rows.Count > 0)
                {
                    DataRow dr_tdsdetails = dt_tdsdetails.Rows[0];
                    object o_sal_basic = dr_tdsdetails["sal_basic"];
                    if (o_sal_basic == DBNull.Value)
                    {
                        d_sal_basic = 0;
                    }
                    else
                    {
                        d_sal_basic = Convert.ToDecimal(dr_tdsdetails["sal_basic"]);
                    }
                    object o_sal_fpa = dr_tdsdetails["sal_fixed_personal_allowance"];
                    if (o_sal_fpa == DBNull.Value)
                    {
                        d_sal_fpa = 0;
                    }
                    else
                    {
                        d_sal_fpa = Convert.ToDecimal(dr_tdsdetails["sal_fixed_personal_allowance"]);
                    }
                    object o_sal_housrentallow = dr_tdsdetails["sal_fpa_hra_allowance"];
                    if (o_sal_housrentallow == DBNull.Value)
                    {
                        d_sal_housrentallow = 0;
                    }
                    else
                    {
                        d_sal_housrentallow = Convert.ToDecimal(dr_tdsdetails["sal_fpa_hra_allowance"]);
                    }
                    object o_sal_fpiip = dr_tdsdetails["sal_fpiip"];
                    if (o_sal_fpa == DBNull.Value)
                    {
                        d_sal_fpiip = 0;
                    }
                    else
                    {
                        d_sal_fpiip = Convert.ToDecimal(dr_tdsdetails["sal_fpiip"]);
                    }
                    object o_sal_da = dr_tdsdetails["sal_da"];
                    if (o_sal_da == DBNull.Value)
                    {
                        d_sal_da = 0;
                    }
                    else
                    {
                        d_sal_da = Convert.ToDecimal(dr_tdsdetails["sal_da"]);
                    }
                    object o_sal_hra = dr_tdsdetails["sal_hra"];
                    if (o_sal_hra == DBNull.Value)
                    {
                        d_sal_hra = 0;
                    }
                    else
                    {
                        d_sal_hra = Convert.ToDecimal(dr_tdsdetails["sal_hra"]);
                    }
                    object o_sal_cca = dr_tdsdetails["sal_cca"];
                    if (o_sal_cca == DBNull.Value)
                    {
                        d_sal_cca = 0;
                    }
                    else
                    {
                        d_sal_cca = Convert.ToDecimal(dr_tdsdetails["sal_cca"]);
                    }
                    object o_sal_interim_relief = dr_tdsdetails["sal_interim_relief"];
                    if (o_sal_interim_relief == DBNull.Value)
                    {
                        d_sal_interim_relief = 0;
                    }
                    else
                    {
                        d_sal_interim_relief = Convert.ToDecimal(dr_tdsdetails["sal_interim_relief"]);
                    }
                    object o_sal_telangana_increment = dr_tdsdetails["sal_telangana_increment"];
                    if (o_sal_telangana_increment == DBNull.Value)
                    {
                        d_tgincrement = 0;
                    }
                    else
                    {
                        d_tgincrement = Convert.ToDecimal(dr_tdsdetails["sal_telangana_increment"]);
                    }
                    object o_sal_spl_allow = dr_tdsdetails["sal_spl_allow"];
                    if (o_sal_spl_allow == DBNull.Value)
                    {
                        d_sal_spl_allow = 0;
                    }
                    else
                    {
                        d_sal_spl_allow = Convert.ToDecimal(dr_tdsdetails["sal_spl_allow"]);
                    }
                    object o_sal_spcl_da = dr_tdsdetails["sal_spcl_da"];
                    if (o_sal_spcl_da == DBNull.Value)
                    {
                        d_sal_spcl_da = 0;
                    }
                    else
                    {
                        d_sal_spcl_da = Convert.ToDecimal(dr_tdsdetails["sal_spcl_da"]);
                    }
                    object o_sal_pfperks = dr_tdsdetails["sal_pfperks"];
                    if (o_sal_pfperks == DBNull.Value)
                    {
                        d_sal_pfperks = 0;
                    }
                    else
                    {
                        d_sal_pfperks = Convert.ToDecimal(dr_tdsdetails["sal_pfperks"]);
                    }
                    object o_sal_loanperks = dr_tdsdetails["sal_loanperks"];
                    if (o_sal_loanperks == DBNull.Value)
                    {
                        d_sal_loanperks = 0;
                    }
                    else
                    {
                        d_sal_loanperks = Convert.ToDecimal(dr_tdsdetails["sal_loanperks"]);
                    }
                    object o_sal_incentive = dr_tdsdetails["sal_incentive"];
                    if (o_sal_incentive == DBNull.Value)
                    {
                        d_sal_incentive = 0;
                    }
                    else
                    {
                        d_sal_incentive = Convert.ToDecimal(dr_tdsdetails["sal_incentive"]);
                    }

                    d_tdsTotal = d_sal_basic + d_sal_fpa + d_sal_housrentallow + d_sal_fpiip + d_sal_da + d_sal_hra + d_sal_cca + d_sal_interim_relief + d_tgincrement + d_sal_spl_allow + d_sal_spcl_da + d_sal_pfperks + d_sal_loanperks + d_sal_incentive;
                }
                if (dt_allowtypes.Rows.Count > 0)
                {
                    foreach (DataRow dr_allowancetype in dt_allowtypes.Rows)
                    {
                        var v_allowtype = dr_allowancetype["allowtype"].ToString();
                        if (v_allowtype == "Exgratia")
                        {
                            object o_exgratia = dr_allowancetype["allowtype"];
                            if (o_exgratia == DBNull.Value)
                            {
                                d_Exgratia = 0;
                            }
                            else
                            {
                                d_Exgratia = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "Medical Aid")
                        {
                            object o_medical_aid = dr_allowancetype["allowtype"];
                            if (o_medical_aid == DBNull.Value)
                            {
                                d_Medical_Aid = 0;
                            }
                            else
                            {
                                d_Medical_Aid = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "LTC")
                        {
                            object o_ltc = dr_allowancetype["allowtype"];
                            if (o_ltc == DBNull.Value)
                            {
                                d_LTC = 0;
                            }
                            else
                            {
                                d_LTC = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "Interest On NSC (Earning)")
                        {
                            object o_Interest_On_NSC = dr_allowancetype["allowtype"];
                            if (o_Interest_On_NSC == DBNull.Value)
                            {
                                d_Interest_On_NSC = 0;
                            }
                            else
                            {
                                d_Interest_On_NSC = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "codperks")
                        {
                            object o_codperks = dr_allowancetype["allowtype"];
                            if (o_codperks == DBNull.Value)
                            {
                                d_codperks = 0;
                            }
                            else
                            {
                                d_codperks = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "PERQPAY")
                        {
                            object o_PERQPAY = dr_allowancetype["allowtype"];
                            if (o_PERQPAY == DBNull.Value)
                            {
                                d_PERQPAY = 0;
                            }
                            else
                            {
                                d_PERQPAY = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "PL Encashment")
                        {
                            object o_pl_encash = dr_allowancetype["allowtype"];
                            if (o_pl_encash == DBNull.Value)
                            {
                                d_PL_Encash = 0;
                            }
                            else
                            {
                                d_PL_Encash = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "SP_ACSTI")
                        {
                            object o_SP_ACSTI = dr_allowancetype["allowtype"];
                            if (o_SP_ACSTI == DBNull.Value)
                            {
                                d_SP_ACSTI = 0;
                            }
                            else
                            {
                                d_SP_ACSTI = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "BR_MGR")
                        {
                            object o_BR_MGR = dr_allowancetype["allowtype"];
                            if (o_BR_MGR == DBNull.Value)
                            {
                                d_BR_MGR = 0;
                            }
                            else
                            {
                                d_BR_MGR = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "GRATUITY")
                        {
                            object o_GRATUITY = dr_allowancetype["allowtype"];
                            if (o_GRATUITY == DBNull.Value)
                            {
                                d_GRATUITY = 0;
                            }
                            else
                            {
                                d_GRATUITY = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "SP_DAFTARI")
                        {
                            object o_GRATUITY = dr_allowancetype["allowtype"];
                            if (o_GRATUITY == DBNull.Value)
                            {
                                d_SP_DAFTARI = 0;
                            }
                            else
                            {
                                d_SP_DAFTARI = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "Leave Encashment")
                        {
                            object o_Leave_Encash = dr_allowancetype["allowtype"];
                            if (o_Leave_Encash == DBNull.Value)
                            {
                                d_Leave_Encash = 0;
                            }
                            else
                            {
                                d_Leave_Encash = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "INCREMENT")
                        {
                            object o_INCREMENT = dr_allowancetype["allowtype"];
                            if (o_INCREMENT == DBNull.Value)
                            {
                                d_Increment = 0;
                            }
                            else
                            {
                                d_Increment = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "STAGALLOW")
                        {
                            object o_STAGALLOW = dr_allowancetype["allowtype"];
                            if (o_STAGALLOW == DBNull.Value)
                            {
                                d_STAGALLOW = 0;
                            }
                            else
                            {
                                d_STAGALLOW = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                        else if (v_allowtype == "ENCASHMENT")
                        {
                            object o_ENCASHMENT = dr_allowancetype["allowtype"];
                            if (o_ENCASHMENT == DBNull.Value)
                            {
                                d_ENCASHMENT = 0;
                            }
                            else
                            {
                                d_ENCASHMENT = Convert.ToDecimal(dr_allowancetype["amount"]);
                            }
                        }
                    }

                    d_allowanceTotal = d_Exgratia + d_Medical_Aid + d_LTC + d_Interest_On_NSC + d_codperks + d_PERQPAY + d_PL_Encash + d_SP_ACSTI + d_BR_MGR + d_GRATUITY + d_SP_DAFTARI + d_Leave_Encash + d_Increment + d_STAGALLOW + d_ENCASHMENT;
                }
                d_GrossAmount = Convert.ToDecimal(d_tdsTotal + d_allowanceTotal);
                return d_GrossAmount;
            }
            catch (Exception ex)
            {
                return d_GrossAmount;
            }
            //return d_GrossAmount;
        }
        public IList<section80Cform16> GetSectionDetails(string empId, string Fyears, string Eyears)
        {
            IList<section80Cform16> section80C = new List<section80Cform16>();
            Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
            string sectionQuery = "";
            string sectionpfvpf = "";
            string constants = "";

            decimal? d_pf = 0;
            decimal? d_vpf = 0;
            decimal? zero = 0;
            decimal? amount = 0;
            try
            {
                sectionQuery = "select 'Housing Loan' as name,sum(principal_paid_amount) as amount from pr_emp_adv_loans_adjustments adj join pr_emp_adv_loans l " +
                    "on l.id = adj.emp_adv_loans_mid where l.emp_code = " + Convert.ToInt32(empId) + " and loan_type_mid in(4, 5,6, 7, 8, 9, 10, 11, 12, 13) and adj.fm <= concat('" + Eyears + "', ' -03-01') " +
                    "union all " +
                    "select 'Provident Fund' as name,sum(dd_provident_fund) as gross from pr_emp_payslip where emp_code = " + Convert.ToInt32(empId) + " and fm <= concat('" + Eyears + "', ' -03-01') " +
                    "union all " +
                    "select 'VPF' as name,sum(dd_amount) as gross from pr_emp_payslip_deductions where emp_code = " + Convert.ToInt32(empId) + " and dd_name = 'VPF Deduction' and payslip_mid in (select id from pr_emp_payslip where emp_code = " + Convert.ToInt32(empId) + " and fy = " + Eyears + ") " +
                    "union all " +
                    "select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id = tdsded.m_id where tdsded.empcode = " + Convert.ToInt32(empId) + " and fm = concat('" + Eyears + "', ' -03-01') and dedmas.id = 310 " +
                    "union all " +
                    "select dedmas.name as name,tdsded.gross as amount from pr_emp_tds_section_deductions tdsded join pr_deduction_field_master dedmas on dedmas.id = tdsded.m_id where tdsded.empcode = " + Convert.ToInt32(empId) + " and fm = concat('" + Eyears + "', ' -03-01') and dedmas.id = 71 " +
                    "union all " +
                    "select 'LIC' as name, sum(dd_amount) as amount from pr_emp_payslip_deductions where emp_code = " + Convert.ToInt32(empId) + " and dd_name = 'LIC' and dd_type = 'EPD' and payslip_mid in(select distinct id from pr_emp_payslip where emp_code = " + Convert.ToInt32(empId) + " and fy = " + Eyears + ") ";
                constants = "SELECT constant, [value]  from all_constants where fy=" + Eyears + " and app_type = 'payroll' and constant ='Section80C'";

                DataSet ds_sections = sh.Get_MultiTables_FromQry(sectionQuery + constants);
                //DataTable dt_sectionpfvpf = ds_sections.Tables[0];
                DataTable dt_section = ds_sections.Tables[0];
                DataTable _dtConstants = ds_sections.Tables[1];
                decimal? allowance_amount = 0;
                decimal? max_allowance = 0;
                decimal? min_allowance = decimal.Parse(_dtConstants.Rows.Cast<DataRow>()
                               .Where(x => x["constant"].ToString() == "Section80C")
                               .Select(x => x["value"].ToString()).FirstOrDefault()); //150000

                if (dt_section.Rows.Count > 0)
                {
                    foreach (DataRow dr_section in dt_section.Rows)
                    {
                        var v_section_type = dr_section["name"].ToString();

                        if (v_section_type == "Housing Loan")
                        {
                            object o_amount = dr_section["amount"];
                            if (o_amount == DBNull.Value)
                            {
                                amount = 0;
                            }
                            else
                            {
                                amount = Convert.ToDecimal(dr_section["amount"]);
                            }
                        }
                        else if (v_section_type == "Provident Fund")
                        {
                            object o_amount = dr_section["amount"];
                            if (o_amount == DBNull.Value)
                            {
                                amount = 0;
                            }
                            else
                            {
                                amount = Convert.ToDecimal(dr_section["amount"]);
                            }
                        }
                        else if (v_section_type == "VPF")
                        {
                            object o_amount = dr_section["amount"];
                            if (o_amount == DBNull.Value)
                            {
                                amount = 0;
                            }
                            else
                            {
                                amount = Convert.ToDecimal(dr_section["amount"]);
                            }
                        }
                        else if (v_section_type == "TAX SAVER FD")
                        {
                            object o_amount = dr_section["amount"];
                            if (o_amount == DBNull.Value)
                            {
                                amount = 0;
                            }
                            else
                            {
                                amount = Convert.ToDecimal(dr_section["amount"]);
                            }
                        }
                        else if (v_section_type == "LIC")
                        {
                            object o_amount = dr_section["amount"];
                            if (o_amount == DBNull.Value)
                            {
                                amount = 0;
                            }
                            else
                            {
                                amount = Convert.ToDecimal(dr_section["amount"]);
                            }
                        }
                        if (amount == 0)
                        {
                            continue;
                        }
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
                            section80C.Add(new section80Cform16
                            {
                                type = v_section_type.ToString(),
                                sect1_grss = AddZerosAfterDecimal(amount.ToString()),
                                sect1_qual = AddZerosAfterDecimal(allowance_amount.ToString()),
                                sect1_dedu = AddZerosAfterDecimal(allowance_amount.ToString())
                            });
                        }
                        else
                        {
                            section80C.Add(new section80Cform16
                            {
                                type = v_section_type.ToString(),
                                sect1_grss = AddZerosAfterDecimal(amount.ToString()),
                                sect1_qual = AddZerosAfterDecimal(zero.ToString()),
                                sect1_dedu = AddZerosAfterDecimal(zero.ToString())
                            });
                        }
                    }
                }
                return section80C;
            }
            catch (Exception Ex)
            {
                return section80C;
            }
            
        }
        public string AddZerosAfterDecimal(string amount)
        {
            if(amount!=null && amount !="")
            {
                float number1 = float.Parse(amount);
                double number = double.Parse(amount, CultureInfo.InvariantCulture);
                string ret = "";
                ret = String.Format("{0:0.00}", number);
                return ret;
            }
            else
            {
                amount = "0.00";
                return amount;
            }
        }

    }
}
