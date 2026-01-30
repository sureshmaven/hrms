using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.DAL.Business;
using Mavensoft.DAL.Db;
using PayrollModels;
using System.Data;

namespace PayRollBusiness.Customization
{
    public class TDSMISReport : BusinessBase
    {
        public TDSMISReport(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        IList<MISReportModel> lst1 = new List<MISReportModel>();
        public async Task<IList<MISReportModel>> GetTDSMIS_Data(string emp_code, string fromdate, string todate, string deduct)
        {
            string qrydeduct = "";
            string qrypf = "";
            string qryded = "";
            DateTime strfromdate = new DateTime();
            DateTime strtodate = new DateTime();
            DateTime firstdayoffromdate = new DateTime();
            DateTime firstdayoftodate = new DateTime();
            string str1 = "";
            string str2 = "";
            string[] datearr;
            string dedsepqry = "";
            string dedsepqrytot = "";
            DataTable dt_mis = new DataTable();
            DataTable dt_mistot = new DataTable();
            if (emp_code.Contains("^"))
            {
                emp_code = "0";
                //mnth = "01-01-01";
                //payslip = "";
            }
            //DateTime str = Convert.ToDateTime(mnth);
            if (fromdate != "^2")
            {
                strfromdate = Convert.ToDateTime(fromdate);
                firstdayoffromdate = new DateTime(strfromdate.Year, strfromdate.Month, 1);
                strtodate = Convert.ToDateTime(todate);
                firstdayoftodate = new DateTime(strtodate.Year, strtodate.Month, 1);
                str1 = strfromdate.ToString("yyyy-MM-dd");
                str2 = strtodate.ToString("yyyy-MM-dd");
                datearr = str1.Split('-');
            }
            else
            {
                strfromdate = Convert.ToDateTime("01-01-01");
                firstdayoffromdate = strfromdate;
                strtodate = Convert.ToDateTime("01-01-01");
                firstdayoftodate = strtodate;
                //datearr = str1.Split('-');
            }
            //if (payslip != "")
            //{
            //    qryps = "and df.ps_type='Regular'";
            //}
            bool pfnull = string.IsNullOrEmpty(deduct);
            //if (pfnull == false)
            //{
            //    if (deduct == "All")
            //    {
            //        qrypf = "and pf.pf_type in('ob_share','ob_share_adhoc','ob_share_encashment')";
            //    }
            //    else if (deduct == "Regular")
            //    {
            //        qrypf = "and pf.pf_type='ob_share'";
            //    }
            //    else if (pf == "Adhoc")
            //    {
            //        qrypf = "and pf.pf_type='ob_share_adhoc'";
            //    }
            //    else if (pf == "Encashment")
            //    {
            //        qrypf = "and pf.pf_type='ob_share_encashment'";
            //    }
            //}
            string qrySel = "";
            string qryseltot = "";
            DataTable Tds = new DataTable();
            string qrysel1 = "";
            DataTable tdstotal = new DataTable();
            int slno = 0;
            if (emp_code != "All")
            {
                qrySel = "select efact.Empid,efact.Name,efact.Designation,tfact.house_rent_allowance,tfact.standard_deductions,tfact.tax_of_employement," +
                    "tfact.tds_aggregate,tfact.tds_per_month from Emp_fact efact join TdsFact tfact on efact.empid=tfact.empcode where efact.Empid=" +
                    " " + emp_code + " and tfact.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "'; ";
                Tds = await _sha.Get_Table_FromQry(qrySel);
                qrysel1 = "select case when sum(tfact.house_rent_allowance) is null then 0 else sum(tfact.house_rent_allowance) end [house_rent_allowance]," +
                    " case when (sum(tfact.standard_deductions)) is null then 0 else sum(tfact.standard_deductions)end [standard_deductions], " +
                    "case when sum(tfact.tax_of_employement) is null then 0 else sum(tfact.tax_of_employement) end [tax_of_employement], " +
                    "case when sum(tfact.tds_aggregate) is null then 0 else sum(tfact.tds_aggregate) end [tds_aggregate], case when sum(tfact.tds_per_month) is null then 0 else sum(tfact.tds_per_month) end [tds_per_month] " +
                    "from Emp_fact efact join TdsFact tfact on efact.empid=tfact.empcode where efact.empid=" + emp_code + " and tfact.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "'; ";
                tdstotal = await _sha.Get_Table_FromQry(qrysel1);

            }
            else
            {
                qrySel = "select efact.Empid,efact.Name,efact.Designation,tfact.house_rent_allowance,tfact.standard_deductions,tfact.tax_of_employement," +
                    "tfact.tds_aggregate,tfact.tds_per_month from Emp_fact efact join TdsFact tfact on efact.empid=tfact.empcode where " +
                    " tfact.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "'; ";
                dt_mis = await _sha.Get_Table_FromQry(qrySel);
                qryseltot = "select case when sum(tfact.house_rent_allowance) is null then 0 else sum(tfact.house_rent_allowance) end [house_rent_allowance]," +
                    " case when (sum(tfact.standard_deductions)) is null then 0 else sum(tfact.standard_deductions)end [standard_deductions], " +
                    "case when sum(tfact.tax_of_employement) is null then 0 else sum(tfact.tax_of_employement) end [tax_of_employement], " +
                    "case when sum(tfact.tds_aggregate) is null then 0 else sum(tfact.tds_aggregate) end [tds_aggregate], case when sum(tfact.tds_per_month) is null then 0 else sum(tfact.tds_per_month) end [tds_per_month] " +
                    "from Emp_fact efact join TdsFact tfact on efact.empid=tfact.empcode where tfact.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "'; ";
                dt_mistot = await _sha.Get_Table_FromQry(qryseltot);
            }
            try
            {
                if (emp_code == "All")
                {
                    foreach (DataRow dr_mis in dt_mis.Rows)
                    {
                        int drcount = dr_mis.ItemArray.Length;
                        string drcolname = "column";
                        List<string> colarr = new List<string>();

                        int colcount = dt_mis.Columns.Count;
                        lst1.Add(new MISReportModel
                        {
                            RowId = slno++,
                            //drcolname+""+i = colarr[i].ToString(),
                            column1 = dr_mis["Empid"].ToString(),
                            column2 = dr_mis["Name"].ToString(),
                            column3 = dr_mis["Designation"].ToString(),
                            column4 = dr_mis["house_rent_allowance"].ToString(),
                            column5 = dr_mis["standard_deductions"].ToString(),
                            column6 = dr_mis["tax_of_employement"].ToString(),
                            column7 = dr_mis["tds_aggregate"].ToString(),
                            column8 = dr_mis["tds_per_month"].ToString(),
                        });
                        //lst.Add(new CommonReportModel
                        //{
                        //    RowId = slno++,

                        //    column1 = dr_mis["Empid"].ToString(),
                        //    column2 = dr_mis["Name"].ToString(),
                        //    column3 = dr_mis["Tdsamount"].ToString(),
                        //    column4 = dr_mis["bank_share"].ToString(),
                        //    column5 = dr_mis["GSLI"].ToString(),
                        //    column6 = dr_mis["Gross"].ToString(),
                        //});
                    }
                    foreach (DataRow dr_midtot in dt_mistot.Rows)
                    {
                        List<string> colarr1 = new List<string>();
                        //foreach (DataColumn c1 in dt_mistot.Columns)
                        //{
                        //    colarr1.Add(c1.ColumnName.ToString());
                        //}
                        lst1.Add(new MISReportModel
                        {
                            RowId = slno++,
                            column1 = "Grand Total",
                            column2 = "",
                            column3 = "",
                            column4 = dr_midtot["house_rent_allowance"].ToString(),
                            column5 = dr_midtot["standard_deductions"].ToString(),
                            column6 = dr_midtot["tax_of_employement"].ToString(),
                            column7 = dr_midtot["tds_aggregate"].ToString(),
                            column8 = dr_midtot["tds_per_month"].ToString(),
                        });
                    }
                }
                else
                {
                    foreach (DataRow dr_mis in Tds.Rows)
                    {
                        int drcount = dr_mis.ItemArray.Length;
                        string drcolname = "column";
                        List<string> colarr = new List<string>();

                        int colcount = dt_mis.Columns.Count;
                        //foreach (DataColumn c in dt_mis.Columns)
                        //{
                        //    colarr.Add(c.ColumnName.ToString());
                        //}

                        //string vpf = "";
                        //if (dr_mis["vpf"].ToString() != "0")
                        //{
                        //    vpf = dr_mis["vpf"].ToString();
                        //}

                        lst1.Add(new MISReportModel
                        {
                            RowId = slno++,
                            //drcolname+""+i = colarr[i].ToString(),
                            column1 = dr_mis["Empid"].ToString(),
                            column2 = dr_mis["Name"].ToString(),
                            column3 = dr_mis["Designation"].ToString(),
                            column4 = dr_mis["house_rent_allowance"].ToString(),
                            column5 = dr_mis["standard_deductions"].ToString(),
                            column6 = dr_mis["tax_of_employement"].ToString(),
                            column7 = dr_mis["tds_aggregate"].ToString(),
                            column8 = dr_mis["tds_per_month"].ToString(),
                        });
                        //lst.Add(new CommonReportModel
                        //{
                        //    RowId = slno++,

                        //    column1 = dr_mis["Empid"].ToString(),
                        //    column2 = dr_mis["Name"].ToString(),
                        //    column3 = dr_mis["Tdsamount"].ToString(),
                        //    column4 = dr_mis["bank_share"].ToString(),
                        //    column5 = dr_mis["GSLI"].ToString(),
                        //    column6 = dr_mis["Gross"].ToString(),
                        //});
                    }
                    foreach (DataRow dr_midtot in tdstotal.Rows)
                    {
                        List<string> colarr1 = new List<string>();
                        //foreach (DataColumn c1 in dt_mistot.Columns)
                        //{
                        //    colarr1.Add(c1.ColumnName.ToString());
                        //}
                        lst1.Add(new MISReportModel
                        {
                            RowId = slno++,
                            column1 = "Grand Total",
                            column2 = "",
                            column3 = "",
                            column4 = dr_midtot["house_rent_allowance"].ToString(),
                            column5 = dr_midtot["standard_deductions"].ToString(),
                            column6 = dr_midtot["tax_of_employement"].ToString(),
                            column7 = dr_midtot["tds_aggregate"].ToString(),
                            column8 = dr_midtot["tds_per_month"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lst1;
        }
    }
}
