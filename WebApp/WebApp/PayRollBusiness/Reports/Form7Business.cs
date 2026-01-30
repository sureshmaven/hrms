using System;
using System.Threading.Tasks;
using PayrollModels;

using System.Data;
using System.Linq;
using Mavensoft.DAL.Business;
using System.Collections.Generic;
using Mavensoft.DAL.Db;

namespace PayRollBusiness.Reports
{
    public class Form7Business : BusinessBase
    {
        public Form7Business(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        //drop down financial year eg:2019-2020
        public async Task<IList<LICReport>> getFy()
        {
            string qryfy = "select year(fm) as fm_fy from pr_month_details where active=1;";
            int fm_fy = await _sha.Run_INS_ExecuteScalar(qryfy);

            IList<LICReport> fyear = new List<LICReport>();

            int fy = fm_fy + 1;
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
                fY = (fm_fy + "-" + (fy)).ToString(),

            });

            for (int i = 1; i < 6; i++)
            {
                Id++;
                fm_fy--;
                fy--;
                fyear.Add(new LICReport
                {
                    Id = Id.ToString(),
                    fY = (fm_fy + "-" + (fy)).ToString(),

                });
            }

            return fyear;


        }
        IList<form7Model1> lst = new List<form7Model1>();

        public async Task<IList<form7Model1>> GetForm7empData(string empcode, string fYear)
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
                        lst.Add(new form7Model1
                        {
                            Id = dr["trans_id"].ToString(),
                            emp_code = dr["emp_code"].ToString(),
                            shortname = dr["name"].ToString(),
                            Description = dr["designation"].ToString(),
                            fy = fYear,

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
                        lst.Add(new form7Model1
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

        public async Task<Form7Model> GetForm7Data(string empId, string fYear)
        {

            //IList<Form16Model> lst = new List<Form16Model>();
            Form7Model fmodel = new Form7Model();
            int Eyear = 0001;
            int Fyear = 0001;
            if (fYear != null && fYear != " ")
            {
                string[] split1 = fYear.Split('-');
                //Console.Write(split[0], split[1]);
                Eyear = int.Parse(split1[1]);
                Fyear = int.Parse(split1[1]) - 1;
            }
            if (empId.Contains(" "))
            {
                empId = "0";
                fYear = "1900";
            }

            if (empId != "All")
            {
                string empdetail = "select CONCAT(emp.FirstName,' ',emp.LastName) as name,gen.pf_no as pfno,region_for_p_tax as per_address,case when Martialstatus = 'Married' then SpouseName else emp.FatherName end as SpouseFather, emp.PanCardNo as pannumber," +
                    " gen.emp_code as emp_code, gen.designation as designation,gen.sex as gender, p.gross_amount as gross, p.deductions_amount as deductionamt, p.fm as financial_year " +
                    " from pr_emp_general gen left outer join Employees emp on gen.emp_code = emp.EmpId join pr_emp_payslip p on p.emp_code = gen.emp_code " +
                    " where gen.emp_code = " + empId + " and p.fm between DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS(" + Eyear + ", 03, 31 )";

                string tdsdetails = "select  REPLACE(RIGHT(CONVERT(VARCHAR(11), ob.fm, 106), 8), ' ', '-') as fm" +
                " from pr_ob_share  ob join pr_emp_general pay on ob.emp_code=pay.emp_code " +
      "join Employees e on e.EmpId=ob.emp_code " +
      "JOIN pr_pf_open_bal_year pfbal on pfbal.emp_code=e.EmpId " +
      " where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and ob.emp_code in (" + empId + ")  and pay.active=1 and pfbal.fy= "+ Fyear + "  order by ob.fm asc ";

                DataSet ds = await _sha.Get_MultiTables_FromQry(empdetail + tdsdetails);
                DataTable empdt = ds.Tables[0];
                DataTable tdsdt = ds.Tables[1];

                try
                {


                    IList<contributionmodel> Contributiondata = new List<contributionmodel>();
                    fmodel.sect1 = Contributiondata;

                    foreach (DataRow dr in empdt.Rows)
                    {
                        fmodel.EmpCode = dr["emp_code"].ToString();
                        fmodel.EmpName = dr["name"].ToString();
                        fmodel.father = dr["SpouseFather"].ToString();
                        //fmodel.Spouse = dr["SpouseFather"].ToString();
                        fmodel.Designation = dr["designation"].ToString();
                        fmodel.fy = fYear;
                        fmodel.city = dr["per_address"].ToString();
                        fmodel.PAN = dr["pannumber"].ToString();
                        fmodel.pf_no = dr["pfno"].ToString();
                    }

                    int totalcount = 0;
                    totalcount = tdsdt.Rows.Count;
                    //fmodel.totalamount = 15000 * totalcount;
                    //fmodel.totalpffund = 1250 * totalcount;
                    
                    foreach (DataRow drtds in tdsdt.Rows)
                    {
                        string fm = drtds["fm"].ToString();
                        fmodel.finmonth = drtds["fm"].ToString();
                        string qry = "select pension_open from pr_ob_share where fm = '01-"+ fm +" ' and emp_code = " + empId + "; ";
                        string qrybasicda = "select er_basic as basic,er_da as da from pr_emp_payslip where fm = '01-" + fm + " ' and emp_code in(" + empId + ")";
                        DataTable dt = await _sha.Get_Table_FromQry(qry);
                        DataTable dtbasicda = await _sha.Get_Table_FromQry(qrybasicda);
                        decimal basic = 0;
                        decimal da = 0;
                        decimal sumbasicda = 0;
                        int totbasicda = 0;
                        if (dtbasicda.Rows.Count > 0)
                        {
                            DataRow empTax = dtbasicda.Rows[0];

                            object val = empTax["basic"];
                            object val2 = empTax["da"];
                            if (val == DBNull.Value)
                            {
                                basic = 0;
                            }
                            else
                            {
                                basic = Convert.ToDecimal(dtbasicda.Rows[0]["basic"]);
                            }
                            if (val == DBNull.Value)
                            {
                                da = 0;
                            }
                            else
                            {
                                da = Convert.ToDecimal(dtbasicda.Rows[0]["da"]);
                            }
                        }
                        
                 
                    
                        sumbasicda = basic + da;
                        sumbasicda = Math.Round(sumbasicda,MidpointRounding.AwayFromZero);

                        if (sumbasicda < 15000)
                        {
                            totbasicda = Convert.ToInt32(sumbasicda);
                            Contributiondata.Add(new contributionmodel
                            {
                                month = drtds["fm"].ToString(),
                                amount = sumbasicda.ToString()+".00",
                                //con_pensoinfund = "1250",
                                con_pensoinfund = dt.Rows[0]["pension_open"].ToString()+".00",

                            });
                        }
                        else
                        {
                            totbasicda = 15000;
                            Contributiondata.Add(new contributionmodel
                            {
                                month = drtds["fm"].ToString(),
                                amount = "15000.00",
                                //con_pensoinfund = "1250",
                                con_pensoinfund = dt.Rows[0]["pension_open"].ToString() + ".00",

                            });
                        }
                        fmodel.totalamount += totbasicda;
                        fmodel.totalpffund += Convert.ToInt32(dt.Rows[0]["pension_open"].ToString());
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
    }
}
