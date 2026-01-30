using Mavensoft.DAL.Business;
using Mavensoft.DAL.Db;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Customization
{
    public class PFMISReport : BusinessBase
    {
        public PFMISReport(LoginCredential loginCredential) : base(loginCredential)
        {

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
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        IList<MISReportModel> lst1 = new List<MISReportModel>();

        public async Task<IList<MISReportModel>> GetPFMIS_Data(string emp_code, string fromdate, string todate, string pf)
        {
            string qrydeduct = "";
            string qryps = "";
            string qrypf = "";
            string qryded = "";
            DateTime strfromdate = new DateTime();
            DateTime strtodate = new DateTime();
            DateTime firstdayoffromdate = new DateTime();
            DateTime firstdayoftodate = new DateTime();
            string str1 = "";
            string str2 = "";
            string[] datearr;
            //DataTable dt_ded = new DataTable();
            //string[] ded = new string[] { };
            //string[] ded11 = new string[] { };
            //List<string> ded1 = new List<string>();
            //bool deductnull = String.IsNullOrEmpty(deduct);
            //if (deductnull == false)
            //{
            //    qrydeduct = "select id,upper(name)as deductname from pr_deduction_field_master where name !='' and id in("+ deduct + ");";
            //    dt_ded = await _sha.Get_Table_FromQry(qrydeduct);
            //    if(dt_ded.Rows.Count>0)
            //    {
            //        foreach (DataRow dr_ded in dt_ded.Rows)
            //        {
            //            ded1.Add(dr_ded["deductname"].ToString());
            //        }
            //    }
            //}
            //ded11 = ded1.ToArray();
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
                strfromdate = Convert.ToDateTime("01-01-0001");
                firstdayoffromdate = strfromdate;
                strtodate = Convert.ToDateTime("01-01-0001");
                firstdayoftodate = strtodate;
                //datearr = str1.Split('-');
            }
            //if (payslip != "")
            //{
            //    qryps = "and df.ps_type='Regular'";
            //}
            bool pfnull = string.IsNullOrEmpty(pf);
            if (pfnull == false)
            {
                if (pf == "All")
                {
                    qrypf = "and pf.pf_type in('ob_share','ob_share_adhoc','ob_share_encashment')";
                }
                else if (pf == "Regular")
                {
                    qrypf = "and pf.pf_type='ob_share'";
                }
                else if (pf == "Adhoc")
                {
                    qrypf = "and pf.pf_type='ob_share_adhoc'";
                }
                else if (pf == "Encashment")
                {
                    qrypf = "and pf.pf_type='ob_share_encashment'";
                }
            }
            //if (deduct != "")
            //{

            //    if (ded11.Length > 1)
            //    {
            //        for (int i = 0; i < ded11.Length; i++)
            //        {
            //            dedsepqry += ",case when df.[" + ded11[i] + "] is null then 0 else df.[" + ded11[i] + "] end [" + ded11[i] + "]";
            //            dedsepqrytot += ",case when sum(df.[" + ded11[i] + "]) is null then 0 else sum(df.[" + ded11[i] + "]) end as [" + ded11[i] + "_tot]";
            //        }
            //    }
            //    else
            //    {
            //        dedsepqry += ",case when df.[" + deduct + "] is null then 0 else df.[" + deduct + "] end [" + deduct + "]";
            //        dedsepqrytot += ",case when sum(df.[" + deduct + "]) is null then 0 else sum(df.[" + deduct + "]) end as [" + deduct + "_tot]";
            //    }

            //}
            string qrySel = "";
            string qryseltot = "";
            DataTable PF = new DataTable();
            string qrysel1 = "";
            DataTable pftotal = new DataTable();
            int slno = 0;
            if (emp_code != "All" && emp_code != "0")
            {
                qrySel = "select format(pf.fm,'dd-MM-yyyy') as fm,e.Empid,e.Name,e.Designation,pf.own_share [Own Share], pf.bank_share [Bank Share], pf.own_share_open [Own Share Open]," +
                    " pf.own_share_total [Own Share Total], pf.bank_share_open [Bank Share Open], pf.bank_share_total [Bank Share Total], " +
                    " pf.pension_open [Pension Open], pf.pension_total [Pension Total], case when pf.pension_intrest_amount is null then 0 else pf.pension_intrest_amount end [Pension Interest Amount], " +
                    "case when pf.bank_share_intrst_open is null then 0 else pf.bank_share_intrst_open end [Bank Share Interest Open], case when pf.bank_share_intrst_total is null then 0 else pf.bank_share_intrst_total end " +
                    " [Bank Share Interest Total] from emp_fact e join pf_fact pf on e.Empid=pf.emp_code where e.Empid=" + emp_code + " and pf.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' " + qrypf + "; ";
                PF = await _sha.Get_Table_FromQry(qrySel);
                qrysel1 = "select sum(pf.own_share) [Own Share], sum(pf.bank_share) [Bank Share], sum(pf.own_share_open) [Own Share Open], " +
                    " sum(pf.own_share_total) [Own Share Total], sum(pf.bank_share_open) [Bank Share Open], sum(pf.bank_share_total) [Bank Share Total], " +
                    " sum(pf.pension_open) [Pension Open], sum(pf.pension_total) [Pension Total], case when sum(pf.pension_intrest_amount) is null then 0 else sum(pf.pension_intrest_amount) end [Pension Interest Amount], " +
                    " case when sum(pf.bank_share_intrst_open) is null then 0 else sum(pf.bank_share_intrst_open) end [Bank Share Interest Open], case when sum(pf.bank_share_intrst_total) is null then 0 else " +
                    " sum(pf.bank_share_intrst_total) end [Bank Share Interest Total] from pf_fact pf where pf.emp_code=" + emp_code + " and pf.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' " + qrypf + "; ";
                pftotal = await _sha.Get_Table_FromQry(qrysel1);

            }
            else
            {
                //qrySel = "select ob.emp_code,e.ShortName,own_share,bank_share,vpf,pension_open, ob.fm,p.spl_type  from pr_ob_share ob " +
                //"join Employees e on e.empid = ob.emp_code " +
                //"join pr_emp_payslip p on ob.emp_code = p.emp_code and month(p.fm)=" + datearr[1] + "  and year(p.fm)=" + datearr[0] + " where month(ob.fm)= " + datearr[1] + " and year(ob.fm)=" + datearr[0] + " and p.spl_type in( '" + payslip + "')";
                //PF = await _sha.Get_Table_FromQry(qrySel);

                //qrysel1 = "select sum(own_share) as own_share,sum(bank_share) as bank_share,sum(vpf) as vpf,sum(pension_open) as pension_open from pr_ob_share ob " +
                //"join Employees e on e.empid = ob.emp_code " +
                //"join pr_emp_payslip p on ob.emp_code = p.emp_code and month(p.fm)=" + datearr[1] + "  and year(p.fm)=" + datearr[0] + " where month(ob.fm)= " + datearr[1] + " and year(ob.fm)=" + datearr[0] + " and p.spl_type in( '" + payslip + "')";
                //pftotal = await _sha.Get_Table_FromQry(qrysel1);
                qrySel = "select format(pf.fm,'dd-MM-yyyy') as fm,e.Empid,e.Name,e.Designation,pf.own_share [Own Share], pf.bank_share [Bank Share], pf.own_share_open [Own Share Open]," +
                    " pf.own_share_total [Own Share Total], pf.bank_share_open [Bank Share Open], pf.bank_share_total [Bank Share Total], " +
                    " pf.pension_open [Pension Open], pf.pension_total [Pension Total], case when pf.pension_intrest_amount is null then 0 else pf.pension_intrest_amount end [Pension Interest Amount], " +
                    "case when pf.bank_share_intrst_open is null then 0 else pf.bank_share_intrst_open end[Bank Share Interest Open]," +
                    " case when pf.bank_share_intrst_total is null then 0 else pf.bank_share_intrst_total end " +
                    " [Bank Share Interest Total] from emp_fact e join pf_fact pf on e.Empid=pf.emp_code where pf.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' " + qrypf + "; ";
                dt_mis = await _sha.Get_Table_FromQry(qrySel);
                qryseltot = "select sum(pf.own_share) [Own Share], sum(pf.bank_share) [Bank Share], sum(pf.own_share_open) [Own Share Open], " +
                    " sum(pf.own_share_total) [Own Share Total], sum(pf.bank_share_open) [Bank Share Open], sum(pf.bank_share_total) [Bank Share Total], " +
                    " sum(pf.pension_open) [Pension Open], sum(pf.pension_total) [Pension Total], sum(pf.pension_intrest_amount) [Pension Interest Amount], " +
                    " sum(pf.bank_share_intrst_open) [Bank Share Interest Open], case when sum(pf.bank_share_intrst_total) is null then 0 else " +
                    " sum(pf.bank_share_intrst_total) end [Bank Share Interest Total] from pf_fact pf where pf.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' " + qrypf + "; ";
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
                            column15 = dr_mis["fm"].ToString(),
                            column4 = ReportColConvertToDecimal(dr_mis["Own Share"].ToString()),
                            column5 = ReportColConvertToDecimal(dr_mis["Bank Share"].ToString()),
                            column6 = ReportColConvertToDecimal(dr_mis["Own Share Open"].ToString()),
                            column7 = ReportColConvertToDecimal(dr_mis["Own Share Total"].ToString()),
                            column8 = ReportColConvertToDecimal(dr_mis["Bank Share Open"].ToString()),
                            column9 = ReportColConvertToDecimal(dr_mis["Bank Share Total"].ToString()),
                            column10 = ReportColConvertToDecimal(dr_mis["Pension Open"].ToString()),
                            column11 = ReportColConvertToDecimal(dr_mis["Pension Total"].ToString()),
                            column12 = ReportColConvertToDecimal(dr_mis["Pension Interest Amount"].ToString()),
                            column13 = ReportColConvertToDecimal(dr_mis["Bank Share Interest Open"].ToString()),
                            column14 = ReportColConvertToDecimal(dr_mis["Bank Share Interest Total"].ToString()),
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
                     
                            column4 = ReportColConvertToDecimal(dr_midtot["Own Share"].ToString()),
                            column5 = ReportColConvertToDecimal(dr_midtot["Bank Share"].ToString()),
                            column6 = ReportColConvertToDecimal(dr_midtot["Own Share Open"].ToString()),
                            column7 = ReportColConvertToDecimal(dr_midtot["Own Share Total"].ToString()),
                            column8 = ReportColConvertToDecimal(dr_midtot["Bank Share Open"].ToString()),
                            column9 = ReportColConvertToDecimal(dr_midtot["Bank Share Total"].ToString()),
                            column10 = ReportColConvertToDecimal(dr_midtot["Pension Open"].ToString()),
                            column11 = ReportColConvertToDecimal(dr_midtot["Pension Total"].ToString()),
                            column12 = ReportColConvertToDecimal(dr_midtot["Pension Interest Amount"].ToString()),
                            column13 = ReportColConvertToDecimal(dr_midtot["Bank Share Interest Open"].ToString()),
                            column14 = ReportColConvertToDecimal(dr_midtot["Bank Share Interest Total"].ToString()),
                            column1 = "Grand Total",
                            column2 = "",
                            column3 = "",
                        });
                    }
                }
                else
                {
                    foreach (DataRow dr_mis in PF.Rows)
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
                            column15 = dr_mis["fm"].ToString(),
                            column4 = ReportColConvertToDecimal(dr_mis["Own Share"].ToString()),
                            column5 = ReportColConvertToDecimal(dr_mis["Bank Share"].ToString()),
                            column6 = ReportColConvertToDecimal(dr_mis["Own Share Open"].ToString()),
                            column7 = ReportColConvertToDecimal(dr_mis["Own Share Total"].ToString()),
                            column8 = ReportColConvertToDecimal(dr_mis["Bank Share Open"].ToString()),
                            column9 = ReportColConvertToDecimal(dr_mis["Bank Share Total"].ToString()),
                            column10 = ReportColConvertToDecimal(dr_mis["Pension Open"].ToString()),
                            column11 = ReportColConvertToDecimal(dr_mis["Pension Total"].ToString()),
                            column12 = ReportColConvertToDecimal(dr_mis["Pension Interest Amount"].ToString()),
                            column13 = ReportColConvertToDecimal(dr_mis["Bank Share Interest Open"].ToString()),
                            column14 = ReportColConvertToDecimal(dr_mis["Bank Share Interest Total"].ToString()),
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
                    foreach (DataRow dr_midtot in pftotal.Rows)
                    {
                        List<string> colarr1 = new List<string>();
                        //foreach (DataColumn c1 in dt_mistot.Columns)
                        //{
                        //    colarr1.Add(c1.ColumnName.ToString());
                        //}
                        lst1.Add(new MISReportModel
                        {
                            RowId = slno++,
                         
                            column4 = ReportColConvertToDecimal(dr_midtot["Own Share"].ToString()),
                            column5 = ReportColConvertToDecimal(dr_midtot["Bank Share"].ToString()),
                            column6 = ReportColConvertToDecimal(dr_midtot["Own Share Open"].ToString()),
                            column7 = ReportColConvertToDecimal(dr_midtot["Own Share Total"].ToString()),
                            column8 = ReportColConvertToDecimal(dr_midtot["Bank Share Open"].ToString()),
                            column9 = ReportColConvertToDecimal(dr_midtot["Bank Share Total"].ToString()),
                            column10 = ReportColConvertToDecimal(dr_midtot["Pension Open"].ToString()),
                            column11 = ReportColConvertToDecimal(dr_midtot["Pension Total"].ToString()),
                            column12 = ReportColConvertToDecimal(dr_midtot["Pension Interest Amount"].ToString()),
                            column13 = ReportColConvertToDecimal(dr_midtot["Bank Share Interest Open"].ToString()),
                            column14 = ReportColConvertToDecimal(dr_midtot["Bank Share Interest Total"].ToString()),
                            column1 = "Grand Total",
                            column2 = "",
                            column3 = "",
                        });
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lst1;
        }
        public async Task<IList<CommonReportModel>> GetPRandP_Data(string emp_code, string mnth)
        {
            if (emp_code.Contains("^"))
            {
                emp_code = "0";
                mnth = "01-01-01";

            }
            DateTime str = Convert.ToDateTime(mnth);
            string str1 = str.ToString("yyyy-MM-dd");
            string[] datearr = str1.Split('-');
            string qrySel = "";
            DataTable PF = new DataTable();
            
            string daysInMonth = DateTime.DaysInMonth(Convert.ToInt32(datearr[0]), Convert.ToInt32(datearr[1])).ToString();
            
            if (emp_code != "All")
            {
                qrySel = "select a.emp_code,e.ShortName as EmpName,format(a.sanction_date,'dd-MM-yyyy') as sanction_date,b.purpose_name,'' as pfsettlements ,sum(sanctioned_amount)as NrWithdrawl,'' as ref_advances,'' as principal,'' as intrest,'' as premium,'' as Servicecharges,'' as s_drs, '' as others,sum(sanctioned_amount)as Total " +
                    "from pr_emp_pf_nonrepayable_loan a join pr_purpose_of_advance_master b on a.purpose_of_advance = b.id  join Employees e on e.empid=a.emp_code " +
                    "where sanction_date between '" + datearr[0] + "-" + datearr[1] + "-01' and '" + datearr[0] + "-" + datearr[1] + "-" + daysInMonth + "' and a.emp_code='"+ emp_code + "' group by purpose_name,sanction_date ,a.emp_code,e.ShortName  " +
                    "union all " +
                    "select a.emp_code,e.ShortName as EmpName, a.sanction_date,b.purpose_name,'' as pfsettlements ,'' as NrWithdrawl,sum(a.least_of_3) as ref_advances,'' as principal,'' as intrest,'' as premium,'' as Servicecharges,'' as s_drs, '' as others,sum(a.least_of_3) as Total " +
                    "from pr_emp_pf_repayable_loan a join pr_purpose_of_advance_master b on a.purpose_of_advance = b.id join Employees e on e.empid=a.emp_code " +
                    "where sanction_date between '" + datearr[0] + "-" + datearr[1] + "-01' and '" + datearr[0] + "-" + datearr[1] + "-" + daysInMonth + "' and a.emp_code='" + emp_code + "' group by purpose_name,sanction_date,a.emp_code,e.ShortName ";
                PF = await _sha.Get_Table_FromQry(qrySel);
            }
            else
            {
                qrySel = "select a.emp_code,e.ShortName as EmpName,format(a.sanction_date,'dd-MM-yyyy') as sanction_date,b.purpose_name,'' as pfsettlements ,sum(sanctioned_amount)as NrWithdrawl,'' as ref_advances,'' as principal,'' as intrest,'' as premium,'' as Servicecharges,'' as s_drs, '' as others,sum(sanctioned_amount)as Total " +
                    "from pr_emp_pf_nonrepayable_loan a join pr_purpose_of_advance_master b on a.purpose_of_advance = b.id join Employees e on e.empid=a.emp_code " +
                    "where sanction_date between '" + datearr[0] + "-" + datearr[1] + "-01' and '" + datearr[0] + "-" + datearr[1] + "-" + daysInMonth + "' group by purpose_name,sanction_date, a.emp_code,e.ShortName  " +
                    "union all " +
                    "select a.emp_code,e.ShortName as EmpName,a.sanction_date,b.purpose_name,'' as pfsettlements ,'' as NrWithdrawl,sum(a.least_of_3) as ref_advances,'' as principal,'' as intrest,'' as premium,'' as Servicecharges,'' as s_drs, '' as others,sum(a.least_of_3) as Total " +
                    "from pr_emp_pf_repayable_loan a join pr_purpose_of_advance_master b on a.purpose_of_advance = b.id join Employees e on e.empid=a.emp_code " +
                    "where sanction_date between '" + datearr[0] + "-" + datearr[1] + "-01' and '" + datearr[0] + "-" + datearr[1] + "-" + daysInMonth + " ' group by purpose_name,sanction_date, a.emp_code,e.ShortName ";
                PF = await _sha.Get_Table_FromQry(qrySel);
            }
            foreach (DataRow dr in PF.Rows)
            {
                lst.Add(new CommonReportModel
                {
                    column1 = dr["emp_code"].ToString(),
                    column2 = dr["EmpName"].ToString(),

                    column3 = dr["sanction_date"].ToString(),
                    column4 = dr["purpose_name"].ToString(),
                    column5 = ReportColConvertToDecimal(dr["pfsettlements"].ToString()),
                    column6 = ReportColConvertToDecimal(dr["NrWithdrawl"].ToString()),
                    column7 = ReportColConvertToDecimal(dr["ref_advances"].ToString()),
                    column8 = ReportColConvertToDecimal(dr["principal"].ToString()),
                    column9 = ReportColConvertToDecimal(dr["intrest"].ToString()),
                    column10 = ReportColConvertToDecimal(dr["premium"].ToString()),
                    column11 = ReportColConvertToDecimal(dr["Servicecharges"].ToString()),
                    column12 = ReportColConvertToDecimal(dr["s_drs"].ToString()),
                    column13 = ReportColConvertToDecimal(dr["others"].ToString()),
                    column14 = "",
                    column15 = ReportColConvertToDecimal(dr["Total"].ToString()),
                });
            }
            return lst;
        }
        public async Task<IList<MISReportModel>> GetVPF_report_Data(string emp_code, string fromdate, string todate, string vpf)
        {
            string qrydeduct = "";
            string qryps = "";
            string qryvpf = "";
            string qryded = "";
            DateTime strfromdate = new DateTime();
            DateTime strtodate = new DateTime();
            DateTime firstdayoffromdate = new DateTime();
            DateTime firstdayoftodate = new DateTime();
            string str1 = "";
            string str2 = "";
            string[] datearr;
            //DataTable dt_ded = new DataTable();
            //string[] ded = new string[] { };
            //string[] ded11 = new string[] { };
            //List<string> ded1 = new List<string>();
            //bool deductnull = String.IsNullOrEmpty(deduct);
            //if (deductnull == false)
            //{
            //    qrydeduct = "select id,upper(name)as deductname from pr_deduction_field_master where name !='' and id in("+ deduct + ");";
            //    dt_ded = await _sha.Get_Table_FromQry(qrydeduct);
            //    if(dt_ded.Rows.Count>0)
            //    {
            //        foreach (DataRow dr_ded in dt_ded.Rows)
            //        {
            //            ded1.Add(dr_ded["deductname"].ToString());
            //        }
            //    }
            //}
            //ded11 = ded1.ToArray();
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
            bool pfnull = string.IsNullOrEmpty(vpf);
            if (pfnull == false)
            {
                if (vpf == "All")
                {
                    qryvpf = "and pf.pf_type in('ob_share','ob_share_adhoc','ob_share_encashment')";
                }
                else if (vpf == "Regular")
                {
                    qryvpf = "and pf.pf_type='ob_share'";
                }
                else if (vpf == "Adhoc")
                {
                    qryvpf = "and pf.pf_type='ob_share_adhoc'";
                }
                else if (vpf == "Encashment")
                {
                    qryvpf = "and pf.pf_type='ob_share_encashment'";
                }
            }
            //if (deduct != "")
            //{

            //    if (ded11.Length > 1)
            //    {
            //        for (int i = 0; i < ded11.Length; i++)
            //        {
            //            dedsepqry += ",case when df.[" + ded11[i] + "] is null then 0 else df.[" + ded11[i] + "] end [" + ded11[i] + "]";
            //            dedsepqrytot += ",case when sum(df.[" + ded11[i] + "]) is null then 0 else sum(df.[" + ded11[i] + "]) end as [" + ded11[i] + "_tot]";
            //        }
            //    }
            //    else
            //    {
            //        dedsepqry += ",case when df.[" + deduct + "] is null then 0 else df.[" + deduct + "] end [" + deduct + "]";
            //        dedsepqrytot += ",case when sum(df.[" + deduct + "]) is null then 0 else sum(df.[" + deduct + "]) end as [" + deduct + "_tot]";
            //    }

            //}
            string qrySel = "";
            string qryseltot = "";
            DataTable PF = new DataTable();
            string qrysel1 = "";
            DataTable pftotal = new DataTable();
            int slno = 0;
            if (emp_code != "All")
            {
                qrySel = "select format(pf.fm,'dd-MM-yyyy') as fm,e.Empid,e.Name,e.Designation,pf.vpf [Vpf],case when pf.vpf_intrst_amount is null then 0 else pf.vpf_intrst_amount end [Vpf Interest Amount]," +
                    "pf.vpf_open [Vpf Open],pf.vpf_total [Vpf Total], case when pf.vpf_intrst_open is null then 0 else pf.vpf_intrst_open end [Vpf Interest Open], " +
                    "case when pf.vpf_intrst_total is null then 0 else pf.vpf_intrst_total end [Vpf Interest Total] from emp_fact e join pf_fact pf on e.Empid=pf.emp_code " +
                    "where e.Empid=" + emp_code + " and pf.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' " + qryvpf + "; ";
                dt_mis = await _sha.Get_Table_FromQry(qrySel);
                qrysel1 = "select Sum(pf.vpf) [Vpf],case when sum(pf.vpf_intrst_amount) is null then 0 else sum(pf.vpf_intrst_amount) end [Vpf Interest Amount]," +
                    "sum(pf.vpf_open) [Vpf Open],sum(pf.vpf_total) [Vpf Total], case when sum(pf.vpf_intrst_open) is null then 0 else sum(pf.vpf_intrst_open) end [Vpf Interest Open], " +
                    "case when sum(pf.vpf_intrst_total) is null then 0 else sum(pf.vpf_intrst_total) end [Vpf Interest Total] from pf_fact pf where pf.emp_code=" + emp_code + " " +
                    "and pf.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' " + qryvpf + "; ";
                dt_mistot = await _sha.Get_Table_FromQry(qrysel1);

            }
            else
            {
                //qrySel = "select ob.emp_code,e.ShortName,own_share,bank_share,vpf,pension_open, ob.fm,p.spl_type  from pr_ob_share ob " +
                //"join Employees e on e.empid = ob.emp_code " +
                //"join pr_emp_payslip p on ob.emp_code = p.emp_code and month(p.fm)=" + datearr[1] + "  and year(p.fm)=" + datearr[0] + " where month(ob.fm)= " + datearr[1] + " and year(ob.fm)=" + datearr[0] + " and p.spl_type in( '" + payslip + "')";
                //PF = await _sha.Get_Table_FromQry(qrySel);

                //qrysel1 = "select sum(own_share) as own_share,sum(bank_share) as bank_share,sum(vpf) as vpf,sum(pension_open) as pension_open from pr_ob_share ob " +
                //"join Employees e on e.empid = ob.emp_code " +
                //"join pr_emp_payslip p on ob.emp_code = p.emp_code and month(p.fm)=" + datearr[1] + "  and year(p.fm)=" + datearr[0] + " where month(ob.fm)= " + datearr[1] + " and year(ob.fm)=" + datearr[0] + " and p.spl_type in( '" + payslip + "')";
                //pftotal = await _sha.Get_Table_FromQry(qrysel1);
                qrySel = "select format(pf.fm,'dd-MM-yyyy') as fm,e.Empid,e.Name,e.Designation,pf.vpf [Vpf],case when pf.vpf_intrst_amount is null then 0 else pf.vpf_intrst_amount end [Vpf Interest Amount]," +
                    "pf.vpf_open [Vpf Open],pf.vpf_total [Vpf Total], case when pf.vpf_intrst_open is null then 0 else pf.vpf_intrst_open end [Vpf Interest Open], " +
                    "case when pf.vpf_intrst_total is null then 0 else pf.vpf_intrst_total end [Vpf Interest Total] from emp_fact e join pf_fact pf on e.Empid=pf.emp_code " +
                    "where pf.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' " + qryvpf + "; ";
                dt_mis = await _sha.Get_Table_FromQry(qrySel);
                qryseltot = "select Sum(pf.vpf) [Vpf],case when sum(pf.vpf_intrst_amount) is null then 0 else sum(pf.vpf_intrst_amount) end [Vpf Interest Amount]," +
                    "sum(pf.vpf_open) [Vpf Open],sum(pf.vpf_total) [Vpf Total], case when sum(pf.vpf_intrst_open) is null then 0 else sum(pf.vpf_intrst_open) end [Vpf Interest Open]," +
                    " case when sum(pf.vpf_intrst_total) is null then 0 else sum(pf.vpf_intrst_total) end [Vpf Interest Total] from pf_fact pf " +
                    "where pf.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' " + qryvpf + "; ";
                dt_mistot = await _sha.Get_Table_FromQry(qryseltot);
            }
            try
            {
                if (emp_code != "0")
                {
                    foreach (DataRow dr_mis in dt_mis.Rows)
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
                            column10 = dr_mis["fm"].ToString(),
                            column4 = ReportColConvertToDecimal(dr_mis["Vpf"].ToString()),
                            column5 = ReportColConvertToDecimal(dr_mis["Vpf Interest Amount"].ToString()),
                            column6 = ReportColConvertToDecimal(dr_mis["Vpf Open"].ToString()),
                            column7 = ReportColConvertToDecimal(dr_mis["Vpf Total"].ToString()),
                            column8 = ReportColConvertToDecimal(dr_mis["Vpf Interest Open"].ToString()),
                            column9 = ReportColConvertToDecimal(dr_mis["Vpf Interest Total"].ToString()),
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
                            column4 = ReportColConvertToDecimal(dr_midtot["Vpf"].ToString()),
                            column5 = ReportColConvertToDecimal(dr_midtot["Vpf Interest Amount"].ToString()),
                            column6 = ReportColConvertToDecimal(dr_midtot["Vpf Open"].ToString()),
                            column7 = ReportColConvertToDecimal(dr_midtot["Vpf Total"].ToString()),
                            column8 = ReportColConvertToDecimal(dr_midtot["Vpf Interest Open"].ToString()),
                            column9 = ReportColConvertToDecimal(dr_midtot["Vpf Interest Total"].ToString()),
                        });
                    }
                }
            }

            catch (Exception ex)
            {
            }
            return lst1;
        }
        public async Task<IList<CommonReportModel>> GetTDSMIS_Data(string emp_code, string fromdate, string todate, string deduct)
        {

            string qryded = "";
            string qryded1 = "";
            string [] qrydedarr;
            string[] qrydedarr1;
            int RowId = 0 ;
            DateTime strfromdate = new DateTime();
            DateTime strtodate = new DateTime();
            DateTime firstdayoffromdate = new DateTime();
            DateTime firstdayoftodate = new DateTime();
            string str1 = "";
            string str2 = "";
            string[] datearr;
            string dedsepqry = "";
            string dedsepqrytot = "";
            string dedqryfieldall = "";
            string[] emparr;
            DataTable dt_mis = new DataTable();
            DataTable dt_mistot = new DataTable();
            DataTable dtfieldall = new DataTable();
            DataTable dtfieldbind = new DataTable();
         
            emparr = emp_code.Split(',');
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
            DataTable dtfieldname = new DataTable();
            if (deduct!= null && deduct != "")
            {
                DataTable dtfieldqry = new DataTable();
                string[] arrded = deduct.Split(',');
                if(deduct=="0")
                {
                    string qrydeduct = "Select column_name as FieldName from INFORMATION_SCHEMA.columns where table_name='DeductionFact' except Select top 5 column_name as FieldName from INFORMATION_SCHEMA.columns where table_name='DeductionFact';";
                    dtfieldname = await _sha.Get_Table_FromQry(qrydeduct);
                }
                else
                {
                    string qrydeduct = "Select column_name as FieldName from INFORMATION_SCHEMA.columns where table_name='DeductionFact' and ORDINAL_POSITION in(" + deduct + ");";
                    dtfieldname = await _sha.Get_Table_FromQry(qrydeduct);
                }
                if (dtfieldname.Rows.Count > 0)
                {
                    foreach (DataRow drfieldname in dtfieldname.Rows)
                    {
                        qryded += "[" + drfieldname["FieldName"] + "],";
                        qryded1+= "[" + drfieldname["FieldName"] + "]#";
                    }
                    qryded = qryded.Remove(qryded.Length-1,1);
                    qrydedarr1 = qryded1.Split('#');
                }

                if (emp_code == "All")
                {
                    dedqryfieldall = "Select df.Emp_code,ef.Name,ef.Designation,df.fm," + qryded + " from DeductionFact df join Emp_fact ef on ef.empid=df.emp_code  " +
                        " Where df.Deduction_type='tds' and df.fm=ef.ps_fm and df.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' order by df.Emp_code; ";
                    dtfieldall = await _sha.Get_Table_FromQry(dedqryfieldall);
                }
                else
                {
                    dedqryfieldall = "Select df.Emp_code,ef.Name,ef.Designation,df.fm," + qryded + " from DeductionFact df join Emp_fact ef on ef.empid=df.emp_code  " +
                        " Where df.Emp_Code in (" + emp_code + ") and df.Deduction_type='tds' and df.fm=ef.ps_fm and df.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' order by df.Emp_code; ";
                    dtfieldall = await _sha.Get_Table_FromQry(dedqryfieldall);
                }

            }
            string amount = "";
            string qrySel = "";
            string qryseltot = "";
            DataTable Tds = new DataTable();
            string qrysel1 = "";
            DataTable tdstotal = new DataTable();
            DataTable dtdedres = new DataTable();
            int SlNo = 1;
            if (emp_code != "All")
            {
                if(qryded=="")
                {
                    dedsepqry = "Select df.Emp_code,ef.Name,Concat(DATENAME(month,fm),'-',year(fm))as month from DeductionFact df join Emp_fact ef on df.Emp_code=ef.EmpId where df.emp_code in(" + emp_code + ") and df.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                }
                else
                {
                    dedsepqry = "Select df.Emp_code,ef.Name," + qryded + ",Concat(DATENAME(month,fm),'-',year(fm))as month from DeductionFact df join Emp_fact ef on df.Emp_code=ef.EmpId where df.emp_code in(" + emp_code + ") and df.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                }
                Tds = await _sha.Get_Table_FromQry(dedsepqry);
            }
            else
            {
                dedsepqry = "Select distinct df.Emp_code,ef.Name,Concat(DATENAME(month,fm),'-',year(fm))as month from DeductionFact df join Emp_fact ef on df.Emp_code=ef.EmpId where df.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' and deduction_type='tds' order by df.Emp_code;";
                Tds = await _sha.Get_Table_FromQry(dedsepqry);
            }
            try
            {
                if (emp_code == "All")
                {

                    string qrydeddt = "";
                    string qrydedall = "";
                    string[] qrydedarrall;
                    foreach (DataRow drfieldname in dtfieldname.Rows)
                    {
                        qrydeddt += "[" + drfieldname["FieldName"] + "],";
                        qrydedall += "[" + drfieldname["FieldName"] + "]#";
                    }
                    qrydeddt = qryded.Remove(qryded.Length - 1, 1);
                    qrydedarrall = qrydedall.Split('#');


                    for (int M = 0; M < Tds.Rows.Count; M++)
                    {


                        string empcode = Tds.Rows[M]["emp_code"].ToString();
                        string empname = Tds.Rows[M]["name"].ToString();
                        string month = Tds.Rows[M]["month"].ToString();
                       
                        int h;
                        DataRow dr = dtfieldbind.NewRow(); //Creating Row
                        dtfieldbind.Columns.Add("FieldName", typeof(String));

                        for (h = 0; h <= dtfieldall.Columns.Count - 1; h++)
                        {
                            for (int ar = 0; ar <= qrydedarrall.Length - 1; ar++)
                            {
                                if ((dtfieldall.Columns[h].ColumnName == qrydedarrall[ar].Replace("]", "").Replace("[", "")) && dtfieldall.Rows[M][h].ToString() != "")
                                {
                                    dtfieldbind.Rows.Add(qrydedarrall[ar].Replace("]", "").Replace("[", "")); //adding to datatable
                                    break;

                                }
                            }
                        }

                        
                        if (Tds.Rows.Count > 0)
                        {
                            SlNo = 1;
                            if (dtfieldbind.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtfieldbind.Rows.Count; i++)
                                {
                                    string qry11 = "Select ISNULL([" + dtfieldbind.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldbind.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empcode + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                                    DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                    string fieldname = dtfieldbind.Rows[i]["FieldName"].ToString();
                                    amount = dtqry11.Rows[0][fieldname].ToString();

                                }
                            }


                             if (amount != "")
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowId++,
                                    HRF = "H",
                                    grpclmn = "<span style='color:#C8EAFB'>~</span>"
                            + ReportColHeader(0, "Emp code", empcode)
                            + ReportColHeader(10, "Emp Name", empname)
                                + ReportColHeader(20, "Month ", month),
                                    column2 = "`",
                                    column3 = "`",
                                    column4 = "`",
                                });

                            }
                            


                        }

                        if (dtfieldbind.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtfieldbind.Rows.Count; i++)
                            {
                                string qry11 = "Select ISNULL([" + dtfieldbind.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldbind.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empcode + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                                DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                string fieldname = dtfieldbind.Rows[i]["FieldName"].ToString();
                                amount = dtqry11.Rows[0][fieldname].ToString();

                                if (amount != "")
                                {

                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowId++,
                                        HRF = "R",
                                        grpclmn = SlNo++.ToString(),
                                        column2 = fieldname,
                                        column3 = amount + ".00",

                                    });
                                }
                            }
                        }
                        
                        if (amount != "")
                        {

                            lst.Add(new CommonReportModel
                            {
                                RowId = RowId++,
                                HRF = "R",
                                column1 = " ",
                                column2 = " ",
                                column3 = " ",
                                column4 = " ",

                            });
                        }

                        dtfieldbind.Columns.Clear();
                        dtfieldbind.Clear();
                        amount = "";
                    }
                    
                }
                
                else
                {
                    if(deduct=="0")
                    {

                        string qrydeddt = "";
                        string qryded2 = "";
                        string[] qrydedarrdt; 

                        foreach (DataRow drfieldname in dtfieldname.Rows)
                        {
                            qrydeddt += "[" + drfieldname["FieldName"] + "],";
                            qryded2 += "[" + drfieldname["FieldName"] + "]#";
                        }
                        qrydeddt = qryded.Remove(qryded.Length - 1, 1);
                        qrydedarrdt = qryded2.Split('#');

                        for (int ecount = 0; ecount < emparr.Count(); ecount++)
                        { 

                            int h;
                            DataRow dr = dtfieldbind.NewRow(); //Creating Row
                            dtfieldbind.Columns.Add("FieldName", typeof(String));

                          for (h = 0; h <= dtfieldall.Columns.Count-1; h++)
                          {
                            //if (h >= 530)
                            //{ string testh = "testh"; }
                            for (int ar = 0; ar <= qrydedarrdt.Length - 1; ar++)
                            {
                                //if (ar >= 530)
                                //{
                                //    string itsme="test";
                                //}
                                if ((dtfieldall.Columns[h].ColumnName == qrydedarrdt[ar].Replace("]", "").Replace("[", "")) && dtfieldall.Rows[ecount][h].ToString() != "")
                                {
                                    //value in the column
                                    //can able to bind the value the the front end
                               
                                    //dr["FieldName"] = qrydedarrdt[ar].Replace("]", "").Replace("[", ""); //assigning value to the row  
                                    dtfieldbind.Rows.Add(qrydedarrdt[ar].Replace("]", "").Replace("[", "")); //adding to datatable
                                    break;
                                    
                                }
                            }

                          }
                       

                            //RowId = 0;
                            string empid = emparr[ecount].ToString();
                            SlNo = 1;
                            if (Tds.Rows.Count > 0)
                            {

                                if (dtfieldbind.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtfieldbind.Rows.Count; i++)
                                    {
                                        string qry11 = "Select ISNULL([" + dtfieldbind.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldbind.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empid + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                                        DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                        string fieldname = dtfieldbind.Rows[i]["FieldName"].ToString();
                                        amount = dtqry11.Rows[0][fieldname].ToString();

                                    }
                                }


                                //if (dtfieldname.Rows.Count > 0)
                                //{
                                //    for (int i = 0; i < dtfieldname.Rows.Count; i++)
                                //    {
                                //        string qry11 = "Select ISNULL([" + dtfieldname.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldname.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empid + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                                //        DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                //        string fieldname = dtfieldname.Rows[i]["FieldName"].ToString();
                                //        amount = dtqry11.Rows[0][fieldname].ToString();

                                //    }
                                //}

                                if (amount != "")
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowId++,
                                        HRF = "H",
                                        grpclmn = "<span style='color:#C8EAFB'>~</span>"
                                 + ReportColHeader(0, "Emp code", Tds.Rows[ecount]["emp_code"].ToString())
                                 + ReportColHeader(10, "Emp Name", Tds.Rows[ecount]["name"].ToString())
                                    + ReportColHeader(20, "Month ", Tds.Rows[ecount]["month"].ToString()),
                                        column2 = "`",
                                        column3 = "`",
                                        column4 = "`",
                                    });
                                }
                            }
                            if (dtfieldbind.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtfieldbind.Rows.Count; i++)
                                {
                                    string qry11 = "Select ISNULL([" + dtfieldbind.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldbind.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empid + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                                    DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                    string fieldname = dtfieldbind.Rows[i]["FieldName"].ToString();
                                    amount = dtqry11.Rows[0][fieldname].ToString();

                                    //if (dtfieldname.Rows.Count > 0)
                                    //{
                                    //    for (int i = 0; i < dtfieldname.Rows.Count; i++)
                                    //    {
                                    //        string qry11 = "Select ISNULL([" + dtfieldname.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldname.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empid + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                                    //        DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                    //        string fieldname = dtfieldname.Rows[i]["FieldName"].ToString();
                                    //         amount = dtqry11.Rows[0][fieldname].ToString();
                                    if (amount != "")
                                    {

                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = RowId++,
                                            HRF = "R",
                                            grpclmn = SlNo++.ToString(),
                                            column2 = fieldname,
                                            column3 = amount + ".00",

                                        });
                                    }
                                }
                            }
                            dtfieldbind.Columns.Clear();
                            dtfieldbind.Clear();
                            amount = "";
                        }
                    }
                    else
                    {

                        string qrydeddt = "";
                        string qryded2 = "";
                        string[] qrydedarrdt;

                        foreach (DataRow drfieldname in dtfieldname.Rows)
                        {
                            qrydeddt += "[" + drfieldname["FieldName"] + "],";
                            qryded2 += "[" + drfieldname["FieldName"] + "]#";
                        }
                        qrydeddt = qryded.Remove(qryded.Length - 1, 1);
                        qrydedarrdt = qryded2.Split('#');

                        for (int ecount = 0; ecount < emparr.Count(); ecount++)
                        {

                            int h;
                            DataRow dr = dtfieldbind.NewRow(); //Creating Row
                            dtfieldbind.Columns.Add("FieldName", typeof(String));

                            for (h = 0; h <= dtfieldall.Columns.Count - 1; h++)
                            {
                                for (int ar = 0; ar <= qrydedarrdt.Length - 1; ar++)
                                {

                                    if ((dtfieldall.Columns[h].ColumnName == qrydedarrdt[ar].Replace("]", "").Replace("[", "")) && dtfieldall.Rows[ecount][h].ToString() != "")
                                    {
                                        //value in the column
                                        //can able to bind the value the the front end

                                        dtfieldbind.Rows.Add(qrydedarrdt[ar].Replace("]", "").Replace("[", "")); //adding to datatable
                                        break;

                                    }
                                }

                            }


                            //RowId = 0;
                            string empid = emparr[ecount].ToString();
                            SlNo = 1;
                            if (Tds.Rows.Count > 0)
                            {

                                if (dtfieldbind.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtfieldbind.Rows.Count; i++)
                                    {
                                        string qry11 = "Select ISNULL([" + dtfieldbind.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldbind.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empid + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                                        DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                        string fieldname = dtfieldbind.Rows[i]["FieldName"].ToString();
                                        amount = dtqry11.Rows[0][fieldname].ToString();

                                    }
                                }



                                if (amount != "")
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowId++,
                                        HRF = "H",
                                        grpclmn = "<span style='color:#C8EAFB'>~</span>"
                                 + ReportColHeader(0, "Emp code", Tds.Rows[ecount]["emp_code"].ToString())
                                 + ReportColHeader(10, "Emp Name", Tds.Rows[ecount]["name"].ToString())
                                    + ReportColHeader(20, "Month ", Tds.Rows[ecount]["month"].ToString()),
                                        column2 = "`",
                                        column3 = "`",
                                        column4 = "`",
                                    });
                                }
                            }
                            if (dtfieldbind.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtfieldbind.Rows.Count; i++)
                                {
                                    string qry11 = "Select ISNULL([" + dtfieldbind.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldbind.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empid + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                                    DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                    string fieldname = dtfieldbind.Rows[i]["FieldName"].ToString();
                                    amount = dtqry11.Rows[0][fieldname].ToString();

                                    if (amount != "")
                                    {

                                        lst.Add(new CommonReportModel
                                        {
                                            RowId = RowId++,
                                            HRF = "R",
                                            grpclmn = SlNo++.ToString(),
                                            column2 = fieldname,
                                            column3 = amount + ".00",

                                        });
                                    }
                                }
                            }
                            dtfieldbind.Columns.Clear();
                            dtfieldbind.Clear();
                            amount = "";
                        }
                        //for (int ecount = 0; ecount < emparr.Count(); ecount++)
                        //{
                        //    //RowId = 0;
                        //    string empid = emparr[ecount].ToString();
                        //    SlNo = 1;
                        //    if (Tds.Rows.Count > 0)
                        //    {
                        //        if (dtfieldname.Rows.Count > 0)
                        //        {
                        //            for (int i = 0; i < dtfieldname.Rows.Count; i++)
                        //            {
                        //                string qry11 = "Select ISNULL([" + dtfieldname.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldname.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empid + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                        //                DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                        //                string fieldname = dtfieldname.Rows[i]["FieldName"].ToString();
                        //                amount = dtqry11.Rows[0][fieldname].ToString();

                        //            }
                        //        }

                        //        if (amount != "")
                        //        {

                        //            lst.Add(new CommonReportModel
                        //            {
                        //                RowId = RowId++,
                        //                HRF = "H",
                        //                grpclmn = "<span style='color:#C8EAFB'>~</span>"
                        //        + ReportColHeader(0, "Emp code", Tds.Rows[ecount]["emp_code"].ToString())
                        //        + ReportColHeader(10, "Emp Name", Tds.Rows[ecount]["name"].ToString())
                        //            + ReportColHeader(20, "Month ", Tds.Rows[ecount]["month"].ToString()),
                        //                column2 = "`",
                        //                column3 = "`",
                        //                column4 = "`",
                        //            });
                        //        }
                        //    }
                        //    if (dtfieldname.Rows.Count > 0)
                        //    {
                        //        for (int i = 0; i < dtfieldname.Rows.Count; i++)
                        //        {
                        //            string qry11 = "Select ISNULL([" + dtfieldname.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldname.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empid + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='tds';";
                        //            DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                        //            string fieldname = dtfieldname.Rows[i]["FieldName"].ToString();
                        //             amount = dtqry11.Rows[0][fieldname].ToString();
                        //            if (amount != "0")
                        //            {

                        //                lst.Add(new CommonReportModel
                        //                {
                        //                    RowId = RowId++,
                        //                    HRF = "R",
                        //                    grpclmn = SlNo++.ToString(),
                        //                    column2 = fieldname,
                        //                    column3 = amount + ".00",

                        //                });
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    
                    
                        
                }
                return lst;
            }
            catch (Exception ex)
            {
                return lst;
            }
            
        }
        public async Task<IList<CommonReportModel>> GetPSMIS_Data(string emp_code, string fromdate, string todate, string deduct)
        {

            string qryded = "";
            string qryded1 = "";
            string[] qrydedarr1;
            int RowId = 0;
            int SlNo = 1;
            DateTime strfromdate = new DateTime();
            DateTime strtodate = new DateTime();
            DateTime firstdayoffromdate = new DateTime();
            DateTime firstdayoftodate = new DateTime();
            string str1 = "";
            string str2 = "";
            string[] datearr;
            string dedsepqry = "";
            DataTable dt_mis = new DataTable();
            DataTable dt_mistot = new DataTable();
            string[] emparr;
            emparr = emp_code.Split(',');
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
            DataTable dtfieldname = new DataTable();
            if (deduct != null && deduct != "")
            {
                DataTable dtfieldqry = new DataTable();
                string[] arrded = deduct.Split(',');
                if(deduct=="0")
                {
                    string qrydeduct = "Select column_name as FieldName from INFORMATION_SCHEMA.columns where table_name='DeductionFact' except Select top 5 column_name as FieldName from INFORMATION_SCHEMA.columns where table_name='DeductionFact';";
                    dtfieldname = await _sha.Get_Table_FromQry(qrydeduct);
                }
                else
                {
                    string qrydeduct = "Select column_name as FieldName from INFORMATION_SCHEMA.columns where table_name='DeductionFact' and ORDINAL_POSITION in(" + deduct + ");";
                    dtfieldname = await _sha.Get_Table_FromQry(qrydeduct);
                }
                if (dtfieldname.Rows.Count > 0)
                {
                    foreach (DataRow drfieldname in dtfieldname.Rows)
                    {
                        qryded += "[" + drfieldname["FieldName"] + "],";
                        qryded1 += "[" + drfieldname["FieldName"] + "]#";
                    }
                    qryded = qryded.Remove(qryded.Length - 1, 1);
                    qrydedarr1 = qryded1.Split('#');
                }

            }

            DataTable Tds = new DataTable();
            DataTable tdstotal = new DataTable();
            DataTable dtdedres = new DataTable();
            if (emp_code != "All")
            {
                if (qryded == "")
                {
                    dedsepqry = "Select df.Emp_code,ef.Name,Concat(DATENAME(month,fm),'-',year(fm))as month from DeductionFact df join Emp_fact ef on df.Emp_code=ef.EmpId where df.emp_code in(" + emp_code + ") and df.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' and deduction_type='payslip';";
                }
                else
                {
                    dedsepqry = "Select df.Emp_code,ef.Name," + qryded + ",Concat(DATENAME(month,fm),'-',year(fm))as month from DeductionFact df join Emp_fact ef on df.Emp_code=ef.EmpId where df.emp_code in(" + emp_code + ") and df.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' and deduction_type='payslip';";
                }

                Tds = await _sha.Get_Table_FromQry(dedsepqry);
            }
            else
            {
                dedsepqry = "Select distinct df.Emp_code,ef.Name,Concat(DATENAME(month,fm),'-',year(fm))as month from DeductionFact df join Emp_fact ef on df.Emp_code=ef.EmpId where df.fm between '" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and '" + firstdayoftodate.ToString("yyyy-MM-dd") + "' and deduction_type='payslip' order by df.Emp_code;";
                Tds = await _sha.Get_Table_FromQry(dedsepqry);
            }
            try
            {
                if (emp_code == "All")
                {
                    foreach (DataRow drallemp in Tds.Rows)
                    {
                        string empcode = drallemp["emp_code"].ToString();
                        string empname = drallemp["name"].ToString();
                        string month= drallemp["month"].ToString();
                        SlNo = 1;
                        if (Tds.Rows.Count > 0)
                        {
                            lst.Add(new CommonReportModel
                            {
                                RowId = RowId++,
                                HRF = "H",
                                grpclmn = "<span style='color:#C8EAFB'>~</span>"
                            + ReportColHeader(0, "Emp code", empcode)
                            + ReportColHeader(10, "Emp Name", empname)
                                + ReportColHeader(20, "Month ", month),
                                column2 = "`",
                                column3 = "`",
                                column4 = "`",
                            });
                        }
                        if (dtfieldname.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtfieldname.Rows.Count; i++)
                            {
                                string qry11 = "Select ISNULL([" + dtfieldname.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldname.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empcode + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='payslip';";
                                DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                string fieldname = dtfieldname.Rows[i]["FieldName"].ToString();
                                string amount = dtqry11.Rows[0][fieldname].ToString();
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowId++,
                                    HRF = "R",
                                    grpclmn = SlNo++.ToString(),
                                    column2 = fieldname,
                                    column3 = amount + ".00",

                                });
                            }
                        }
                        lst.Add(new CommonReportModel
                        {
                            RowId = RowId++,
                            HRF = "R",
                            column1 = " ",
                            column2 = " ",
                            column3 = " ",
                            column4 = " ",

                        });
                    }
                }

                else
                {
                    if(deduct=="0")
                    {
                        for (int ecount = 0; ecount < emparr.Count(); ecount++)
                        {
                            string empid = emparr[ecount].ToString();
                            SlNo = 1;
                            if (Tds.Rows.Count > 0)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowId++,
                                    HRF = "H",
                                    grpclmn = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColHeader(0, "Emp code", Tds.Rows[ecount]["emp_code"].ToString())
                                + ReportColHeader(10, "Emp Name", Tds.Rows[ecount]["name"].ToString())
                                + ReportColHeader(20, "Month ", Tds.Rows[ecount]["month"].ToString()),
                                    //+ ReportColHeader(20, "", ""),
                                    column2 = "`",
                                    column3 = "`",
                                    column4 = "`",
                                });
                            }
                            if (dtfieldname.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtfieldname.Rows.Count; i++)
                                {
                                    string qry11 = "Select ISNULL([" + dtfieldname.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldname.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empid + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='payslip';";
                                    DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                    string fieldname = dtfieldname.Rows[i]["FieldName"].ToString();
                                    string amount = dtqry11.Rows[0][fieldname].ToString();
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowId++,
                                        HRF = "R",
                                        grpclmn = SlNo++.ToString(),
                                        column2 = fieldname,
                                        column3 = amount + ".00",

                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int ecount = 0; ecount < emparr.Count(); ecount++)
                        {
                            string empid = emparr[ecount].ToString();
                            SlNo = 1;
                            if (Tds.Rows.Count > 0)
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowId++,
                                    HRF = "H",
                                    grpclmn = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColHeader(0, "Emp code", Tds.Rows[ecount]["emp_code"].ToString())
                                + ReportColHeader(10, "Emp Name", Tds.Rows[ecount]["name"].ToString())
                                + ReportColHeader(20, "Month ", Tds.Rows[ecount]["month"].ToString()),
                                    //+ ReportColHeader(20, "", ""),
                                    column2 = "`",
                                    column3 = "`",
                                    column4 = "`",
                                });
                            }
                            if (dtfieldname.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtfieldname.Rows.Count; i++)
                                {
                                    string qry11 = "Select ISNULL([" + dtfieldname.Rows[i]["FieldName"].ToString() + "],0) as [" + dtfieldname.Rows[i]["FieldName"].ToString() + "] from DeductionFact where emp_code in(" + empid + ") and fm='" + firstdayoffromdate.ToString("yyyy-MM-dd") + "' and deduction_type='payslip';";
                                    DataTable dtqry11 = await _sha.Get_Table_FromQry(qry11);
                                    string fieldname = dtfieldname.Rows[i]["FieldName"].ToString();
                                    string amount = dtqry11.Rows[0][fieldname].ToString();
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowId++,
                                        HRF = "R",
                                        grpclmn = SlNo++.ToString(),
                                        column2 = fieldname,
                                        column3 = amount + ".00",

                                    });
                                }
                            }
                        }
                    }
                    
                        
                }
                return lst;
            }
            catch (Exception ex)
            {
                return lst;
            }

        }
    }
}

    