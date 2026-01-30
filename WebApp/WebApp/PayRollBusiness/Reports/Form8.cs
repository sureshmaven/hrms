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

    public class Form8 : BusinessBase
    {
        public Form8(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        //public async Task<string> form8(string empCode)
        //{
        //    string query = " select EmpId,ShortName,d.Name from Employees e join Designations d on e.CurrentDesignation = d.Id where e.RetirementDate >= GETDATE()";

        //    DataTable dt = await _sha.Get_Table_FromQry(query);
        //    return JsonConvert.SerializeObject(dt);
        //}

        //report2
        public async Task<IList<CommonReportModel>> form8(string Fyear)
        {
            Form7Model fmodel = new Form7Model();
            CommonReportModel crm = new CommonReportModel();
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int RowCnt = 0;
            int totalamount = 0;
            int SlNo = 1;
            int Eyear = 1900;
            int FYear = 1900;
            try
            {
                if (Fyear != null && Fyear != "^1")
                {
                    string[] split1 = Fyear.Split('-');
                    //Console.Write(split[0], split[1]);
                    Eyear = int.Parse(split1[0]);
                    FYear = int.Parse(split1[1]);
                }

                if (Fyear.Contains("^"))
                {
                    FYear = 1900;
                    Eyear = 1900;
                }

                int Fdate = FYear;
                int Tdate = Eyear;

                //string query = " select EmpId,ShortName,d.Name as designation,empgen.pf_no from Employees e " +
                //    "join Designations d on e.CurrentDesignation = d.Id " +
                //    "join pr_emp_general empgen on empgen.emp_code = e.empid " +
                //    "where e.RetirementDate >= GETDATE() and fm between DATEFROMPARTS('" + Tdate + "', 04, 01) and DATEFROMPARTS('" + Fdate + "', 03, 31 );";

                string qry = "select e.EmpId,empgen.pf_no,e.ShortName,d.Name as designation ,floor(6500 * sum(ob.pension_open) / 541) AS COUNTGROSS,sum(ob.pension_open)  AS TPENSION " +
                    "from Employees e join Designations d on d.id = e.CurrentDesignation join pr_emp_general empgen on e.EmpId = empgen.emp_code " +
                    "join pr_ob_share ob on ob.emp_code = e.EmpId " +
                    "where ob.fm between DATEFROMPARTS('" + Tdate + "', 04, 01) and DATEFROMPARTS('" + Fdate + "', 03, 31 )  and empgen.active = 1 and ob.pension_open >= 0   group by e.EmpId,empgen.pf_no,e.ShortName,d.name order by e.EmpId asc";
                DataTable dts = await _sha.Get_Table_FromQry(qry);

                
                foreach (DataRow drs in dts.Rows)
                {
                    totalamount = 0;
                    string empId = drs["EmpId"].ToString();
                    string tdsdetails = "select  REPLACE(RIGHT(CONVERT(VARCHAR(11), ob.fm, 106), 8), ' ', '-') as fm" +
                " from pr_ob_share  ob join pr_emp_general pay on ob.emp_code=pay.emp_code " +
      "join Employees e on e.EmpId=ob.emp_code " +
      "JOIN pr_pf_open_bal_year pfbal on pfbal.emp_code=e.EmpId " +
      " where ob.fm between DATEFROMPARTS(" + Tdate + ", 04, 01) and DATEFROMPARTS(" + Fdate + ", 03, 31 ) and ob.emp_code in (" + empId + ") and pfbal.fy = '"+ Tdate + "'  and pay.active=1  order by ob.fm asc ";

                   
                    DataTable dts1 = await _sha.Get_Table_FromQry(tdsdetails);
                    int totalcount = 0;

                    foreach (DataRow drtds in dts1.Rows)
                    {
                        string fm = drtds["fm"].ToString();
                        fmodel.finmonth = drtds["fm"].ToString();
                        string getqry = "select pension_open from pr_ob_share where fm = '01-" + fm + " ' and emp_code = " + empId + "; ";
                        string qrybasicda = "select er_basic as basic,er_da as da from pr_emp_payslip where fm = '01-" + fm + " ' and emp_code in(" + empId + ")";
                        DataTable dt = await _sha.Get_Table_FromQry(getqry);
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
                        sumbasicda = Math.Round(sumbasicda, MidpointRounding.AwayFromZero);

                        if (sumbasicda < 15000)
                        {
                            totbasicda = Convert.ToInt32(sumbasicda);
                        }
                        else
                        {
                            totbasicda = 15000;
                       
                        }
                        totalamount += totbasicda;
                    
                    }

                    //totalcount = dts1.Rows.Count;
                    //int totalamount = 15000 * totalcount;

                    SlNo = SlNo++;
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),

                        column1 = drs["EmpId"].ToString(),
                        column2 = drs["pf_no"].ToString(),
                        column3 = drs["ShortName"].ToString(),
                        column4 = drs["designation"].ToString(),
                        column5 = ReportColConvertToDecimal(totalamount.ToString()),
                        //column5 = drs["COUNTGROSS"].ToString(),
                        column6 = ReportColConvertToDecimal( drs["TPENSION"].ToString()),

                        //column8 = drs["empid"].ToString(),
                        //column9 = drs["shortname"].ToString(),
                        //column10 = Convert.ToDateTime(drs["sanction_date"]).ToString("dd/MM/yyyy")

                    });
                }
            }
            catch(Exception ex)
            {

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
    }
}
