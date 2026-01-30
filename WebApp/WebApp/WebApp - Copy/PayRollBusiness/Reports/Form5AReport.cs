using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.Configuration;
using PayRollBusiness;

namespace PayRollBusiness.Reports
{
    public class Form5AReport : BusinessBase
    {
        public Form5AReport(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        //string taxValue = WebConfigurationManager.AppSettings.Get("Taxvalues").ToString();
        //string employeeSal = WebConfigurationManager.AppSettings.Get("EmployeeSalary").ToString();

        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        Helper helper = new Helper();


        IList<Form5A> empSal1 = new List<Form5A>();

        public async Task<IList<Form5A>> getForm5AData(string mnthForm5A)
        {
            if (mnthForm5A == "01-01-01")
            {
                return empSal1;
            }
            DateTime str = Convert.ToDateTime(mnthForm5A);
            string str1 = str.ToString("yyyy-MM-dd");


            int Num1 = 0;
            int Num2 = 0;
            int Num3 = 0;
            int T_Tax = 0;
            int T_Tax2 = 0;
            int rowId = 0;
            string empsal1, empsal2, empsaltax1, empsaltax2;                   
            string sal = "";
            string sal1 = "";
            string sqlstr1 = "SELECT value from all_constants WHERE constant = 'ProfTaxMinMaxAmts'";
            DataTable EmpSals = await _sha.Get_Table_FromQry(sqlstr1);
            string[] ar = new string[2];
            foreach (DataRow dr in EmpSals.Rows){
                sal = dr["value"].ToString();
            }            
            ar = sal.Split(',');
           empsal1 = ar[0];
          empsal2 = ar[1];

            string sqlstr2 = " SELECT value from all_constants where constant = 'ProfTaxMinMaxAmtVals'";
            DataTable EmpSalstax = await _sha.Get_Table_FromQry(sqlstr2);
            string[] ar1 = new string[2];
            foreach (DataRow dr in EmpSalstax.Rows)
            {
                sal1 = dr["value"].ToString();
            }
            ar1 = sal1.Split(',');
            empsaltax1 = ar1[0];
            empsaltax2 = ar1[1];


            //-------------gross<=15000

            string qrySel1 = "select count(* )emp_id from pr_emp_payslip P WHERE month(P.fm)= month('" + str1 + "')  AND year(P.fm)= year('" + str1 + "')  AND gross_amount<= (" + empsal1 + ") AND spl_type='Regular' "; 
            //-------------gross>15000 and <=20000
            string qrySel2 = "select count(* )emp_id from pr_emp_payslip P WHERE month(P.fm)= month('" + str1 + "')  AND year(P.fm)= year('" + str1 + "')  AND gross_amount> (" + empsal1 + ") AND spl_type='Regular' AND gross_amount<= (" + empsal2 + ") ";
            //------------gross>20000 and <=2000000
            string qrySel3 = "select count(* )emp_id from pr_emp_payslip P WHERE month(P.fm)= month('" + str1 + "')  AND year(P.fm)= year('" + str1 + "')  AND gross_amount> ( "+ empsal2 + ") AND spl_type='Regular' ";


            DataSet dsAllemp = await _sha.Get_MultiTables_FromQry(qrySel1 + qrySel2 + qrySel3);
            DataTable dt1 = dsAllemp.Tables[0];
            DataTable dt2 = dsAllemp.Tables[1];
            DataTable dt3 = dsAllemp.Tables[2];

            foreach (DataRow dr1 in dt1.Rows)
            {
                Num1 = int.Parse(dr1["emp_id"].ToString());
            }

            foreach (DataRow dr2 in dt2.Rows)
            {
                Num2 = int.Parse(dr2["emp_id"].ToString());
               T_Tax = Num2 * int.Parse(empsaltax1);


            }
            foreach (DataRow dr3 in dt3.Rows)
            {
                Num3 = int.Parse(dr3["emp_id"].ToString());
               T_Tax2 = Num3 * int.Parse(empsaltax2);


            }

            //IList<Form5A> empSal1 = new List<Form5A>();


            try
            {
                empSal1.Add(new Form5A
                {
                    RowId = rowId++,
                    EmpMonthSalary = "Less than " + empsal1,
                    NumberofEmployees = Num1.ToString(),
                    TaxperMonth = ReportColConvertToDecimal("0"),
                    TaxDeducted = ReportColConvertToDecimal("0"),


                });

                empSal1.Add(new Form5A
                {
                    RowId = rowId++,
                    EmpMonthSalary = "Between " + empsal1 + " and " +empsal2,
                    NumberofEmployees = Num2.ToString(),
                    TaxperMonth = ReportColConvertToDecimal(empsaltax1),
                    TaxDeducted = ReportColConvertToDecimal(T_Tax.ToString()),


                });


                empSal1.Add(new Form5A
                {
                    RowId = rowId++,
                    EmpMonthSalary = "Greater Than " + empsal2,
                    NumberofEmployees = Num3.ToString(),
                    TaxperMonth = ReportColConvertToDecimal(empsaltax2),
                    TaxDeducted = ReportColConvertToDecimal(T_Tax2.ToString()),
                  //  TaxperMonth = empsaltax2,
                  //  TaxDeducted = T_Tax2.ToString(),


                });
                

            }

            catch (Exception ex)
            {

            }

            return empSal1;
        }

        public async Task<IList<Form5A>> getForm5ADataForPOC(string mnthForm5A)
        {
            if (mnthForm5A == "01-01-01")
            {
                return empSal1;
            }
            DateTime str = Convert.ToDateTime(mnthForm5A);
            string str1 = str.ToString("yyyy-MM-dd");
            int fy = _LoginCredential.FY;
            string present_table = helper.getPresentTable(fy,"old_pr_emp_payslip", "pr_emp_payslip", str1);


            int Num1 = 0;
            int Num2 = 0;
            int Num3 = 0;
            int T_Tax = 0;
            int T_Tax2 = 0;
            int rowId = 0;
            string empsal1, empsal2, empsaltax1, empsaltax2;
            string sal = "";
            string sal1 = "";
            string sqlstr1 = "SELECT value from all_constants WHERE constant = 'ProfTaxMinMaxAmts'";
            DataTable EmpSals = await _sha.Get_Table_FromQry(sqlstr1);
            string[] ar = new string[2];
            foreach (DataRow dr in EmpSals.Rows)
            {
                sal = dr["value"].ToString();
            }
            ar = sal.Split(',');
            empsal1 = ar[0];
            empsal2 = ar[1];

            string sqlstr2 = " SELECT value from all_constants where constant = 'ProfTaxMinMaxAmtVals'";
            DataTable EmpSalstax = await _sha.Get_Table_FromQry(sqlstr2);
            string[] ar1 = new string[2];
            foreach (DataRow dr in EmpSalstax.Rows)
            {
                sal1 = dr["value"].ToString();
            }
            ar1 = sal1.Split(',');
            empsaltax1 = ar1[0];
            empsaltax2 = ar1[1];


            //-------------gross<=15000

            string qrySel1 = "select count(* )emp_id from "+ present_table +" P join pr_month_details M ON M.fy = P.fy WHERE month(P.fm)= month('" + str1 + "')  AND year(P.fm)= year('" + str1 + "')  AND gross_amount<= (" + empsal1 + ") AND spl_type='Regular' AND gross_amount!= 0";
            //-------------gross>15000 and <=20000
            string qrySel2 = "select count(* )emp_id from " + present_table + " P join pr_month_details M ON M.fy = P.fy WHERE month(P.fm)= month('" + str1 + "')  AND year(P.fm)= year('" + str1 + "')  AND gross_amount> (" + empsal1 + ") AND spl_type='Regular' AND gross_amount<= (" + empsal2 + ") AND gross_amount!= 0";
            //------------gross>20000 and <=2000000
            string qrySel3 = "select count(* )emp_id from " + present_table + " P join pr_month_details M ON M.fy = P.fy WHERE month(P.fm)= month('" + str1 + "')  AND year(P.fm)= year('" + str1 + "')  AND gross_amount> ( " + empsal2 + ") AND spl_type='Regular' AND gross_amount!= 0";


            DataSet dsAllemp = await _sha.Get_MultiTables_FromQry(qrySel1 + qrySel2 + qrySel3);
            DataTable dt1 = dsAllemp.Tables[0];
            DataTable dt2 = dsAllemp.Tables[1];
            DataTable dt3 = dsAllemp.Tables[2];

            foreach (DataRow dr1 in dt1.Rows)
            {
                Num1 = int.Parse(dr1["emp_id"].ToString());
            }

            foreach (DataRow dr2 in dt2.Rows)
            {
                Num2 = int.Parse(dr2["emp_id"].ToString());
                T_Tax = Num2 * int.Parse(empsaltax1);


            }
            foreach (DataRow dr3 in dt3.Rows)
            {
                Num3 = int.Parse(dr3["emp_id"].ToString());
                T_Tax2 = Num3 * int.Parse(empsaltax2);


            }

            //IList<Form5A> empSal1 = new List<Form5A>();


            try
            {
                empSal1.Add(new Form5A
                {
                    RowId = rowId++,
                    EmpMonthSalary = "Less than " + empsal1,
                    NumberofEmployees = Num1.ToString(),
                    TaxperMonth = ReportColConvertToDecimal("0"),
                    TaxDeducted = ReportColConvertToDecimal("0"),


                });

                empSal1.Add(new Form5A
                {
                    RowId = rowId++,
                    EmpMonthSalary = "Between " + empsal1 + " and " + empsal2,
                    NumberofEmployees = Num2.ToString(),
                    TaxperMonth = ReportColConvertToDecimal(empsaltax1),
                    TaxDeducted = ReportColConvertToDecimal(T_Tax.ToString()),


                });


                empSal1.Add(new Form5A
                {
                    RowId = rowId++,
                    EmpMonthSalary = "Greater Than " + empsal2,
                    NumberofEmployees = Num3.ToString(),
                    TaxperMonth = ReportColConvertToDecimal(empsaltax2),
                    TaxDeducted = ReportColConvertToDecimal(T_Tax2.ToString()),


                });


            }

            catch (Exception ex)
            {

            }

            return empSal1;
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








        //public async Task<IList<Form5A>> getForm5AData(string mnthForm5A)
        //{
        //    if (mnthForm5A == "01-01-01")            {
        //        return empSal1;
        //    }
        //    DateTime str = Convert.ToDateTime(mnthForm5A);
        //    string str1 = str.ToString("yyyy-MM-dd");


        //    int Num1 = 0;
        //    int Num2 = 0;
        //    int Num3 = 0;
        //    int T_Tax = 0;
        //    int T_Tax2 = 0;
        //    string taxval1 = taxValue.Substring(0, taxValue.LastIndexOf("*"));
        //    string taxval2 = taxValue.Substring(4, taxValue.LastIndexOf("*"));
        //    string empsal = employeeSal.Substring(0, employeeSal.LastIndexOf("*"));
        //    string[] EmpSals = new string[3];
        //    EmpSals = employeeSal.Split('*');
        //    string empsal1 = EmpSals[0];
        //    string empsal2 = EmpSals[1];
        //    string empsal3 = EmpSals[2];




        //    //-------------gross<=15000

        //    string qrySel1 = "select count(* )emp_id from pr_emp_payslip where month(fm)=month('" + str1 + "')  and year(fm)=year('" + str1 + "') and gross_amount<=15000";


        //    //-------------gross>15000 and <=20000

        //    string qrySel2 = "select count(* )emp_id from pr_emp_payslip where month(fm)=month('" + str1 + "')  and year(fm)=year('" + str1 + "') and gross_amount>15000 and gross_amount<=20000";


        //    //------------gross>20000 and <=2000000

        //    string qrySel3 = "select count(* )emp_id from pr_emp_payslip where month(fm)=month('" + str1 + "')  and year(fm)=year('" + str1 + "') and gross_amount>20000 and gross_amount<=2000000";


        //    DataSet dsAllemp = await _sha.Get_MultiTables_FromQry(qrySel1+ qrySel2+ qrySel3);
        //    DataTable dt1 = dsAllemp.Tables[0];
        //    DataTable dt2 = dsAllemp.Tables[1];
        //    DataTable dt3 = dsAllemp.Tables[2];

        //    foreach (DataRow dr1 in dt1.Rows)
        //    {
        //         Num1 = int.Parse(dr1["emp_id"].ToString());


        //    }

        //    foreach (DataRow dr2 in dt2.Rows)
        //    {
        //        Num2 = int.Parse(dr2["emp_id"].ToString());
        //        T_Tax = Num2 * int.Parse(taxval1);


        //    }
        //    foreach (DataRow dr3 in dt3.Rows)
        //    {
        //        Num3 = int.Parse(dr3["emp_id"].ToString());
        //        T_Tax2 = Num3 * int.Parse(taxval2);


        //    }

        //    //IList<Form5A> empSal1 = new List<Form5A>();


        //    try
        //    {
        //        empSal1.Add(new Form5A
        //            {
        //                EmpMonthSalary = "Less than" +empsal1,
        //                NumberofEmployees = Num1.ToString(),
        //                TaxperMonth = "0",
        //                TaxDeducted ="0",


        //            });


        //            empSal1.Add(new Form5A
        //            {
        //                EmpMonthSalary = "Between"+ empsal2,
        //                NumberofEmployees = Num2.ToString() ,
        //                TaxperMonth = taxval1,
        //                TaxDeducted = T_Tax.ToString() ,


        //            });


        //            empSal1.Add(new Form5A
        //            {
        //                EmpMonthSalary = "Between"+ empsal3,
        //                NumberofEmployees = Num3.ToString() ,
        //                TaxperMonth = taxval2,
        //                TaxDeducted = T_Tax2.ToString(),


        //            });

        //    }

        //    catch (Exception ex)
        //    {

        //    }

        //    return empSal1;
        //}
    }
}
