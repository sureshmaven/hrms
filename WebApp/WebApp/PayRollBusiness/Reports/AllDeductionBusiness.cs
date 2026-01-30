using Mavensoft.DAL.Business;
using Mavensoft.DAL.Db;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Reports
{
    public class AllDeductionBusiness : BusinessBase
    {
        public AllDeductionBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        SqlHelperAsync _sha = new SqlHelperAsync();
        IList<AllowanceTypes> lstDept = new List<AllowanceTypes>();


        #region All Deductions 

        public async Task<IList<CommonReportModel>> AllDeduction(string empType, string rptType, string month)
        {

            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int SlNo = 1;
            int RowCnt = 0;

            string qry = "";
            string qry1 = "";
            string empsta = empType;
            string rptsta = rptType;
            string mnthqry = "";
            string deduction = "";
            string olddeduction = "";
            int totamnt = 0;

            if (empsta.Contains("^") && rptsta.Contains("^"))
            {

                empsta = "";
                rptsta = "";
                month = "01-01-01";
            }

            mnthqry = "select fm from pr_month_details where active=1 ";
            DataTable dtmoth = await _sha.Get_Table_FromQry(mnthqry);

            DateTime mnthdet = Convert.ToDateTime(dtmoth.Rows[0]["fm"].ToString());
            string monthstr = mnthdet.ToString("yyyy-MM-dd");

            DateTime str = Convert.ToDateTime(month);
            string str1 = str.ToString("yyyy-MM-dd");

            string date = str.ToString("MMMM-yyyy");


            if (str1 == monthstr)
            {
                if (empsta != "undefined" && rptsta != "undefined")
                {

                    if (empsta == "Regu" && rptsta == "Summary")
                    {


                        //qry = " select count(m.emp_code) as emp_count,c.name,sum(m.amount) as amount from pr_emp_deductions m join pr_deduction_field_master c on m.m_id = c.id join pr_emp_payslip p on p.emp_code = m.emp_code " +
                        //    " where month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "')  and p.spl_type = 'Regular' and m.amount>0 and c.type = 'EPD'   group by c.name " +
                        //    " union all " +
                        //    " select count(p.emp_code) as emp_count,'Provident Fund' as name,sum(p.dd_provident_fund) as amount from pr_emp_payslip p  " +
                        //    " where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and  p.spl_type = 'Regular' and p.dd_provident_fund > 0 " +
                        //    " union all " +
                        //    " select count(p.emp_code) as emp_count,'Income Tax' as name,sum(p.dd_income_tax) as amount from pr_emp_payslip p " +
                        //    " where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_income_tax > 0 ";

                        qry = "  select count(m.emp_code) as emp_count,c.name,sum(m.amount) as amount from pr_emp_deductions m join pr_deduction_field_master c on m.m_id = c.id join pr_emp_payslip p on p.emp_code = m.emp_code where month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "')  and p.spl_type = 'Regular'  and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and m.amount > 0 and c.type = 'EPD'   group by c.name " +
                            "union all select count(p.emp_code) as emp_count,'Provident Fund' as name, sum(p.dd_provident_fund) as amount from pr_emp_payslip p   where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_provident_fund > 0 " +
                            "union all  select count(p.emp_code) as emp_count,'Income Tax' as name,sum(p.dd_income_tax) as amount from pr_emp_payslip p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_income_tax > 0 " +
                            //"union all select count(p.emp_code)as emp_count,'LIC' as name,sum(p.amount) as amount from pr_emp_lic_details p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')" +
                            //"union all select count(p.emp_code)as emp_count,'HFC' as name,sum(p.amount) as amount from pr_emp_hfc_details p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            "union all select count(p.emp_code)as emp_count,'LIC' as name,sum(p.dd_amount) as amount from pr_emp_payslip_deductions p where payslip_mid in(select id from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Regular') and p.dd_name = 'LIC' " +
                            "union all select count(p.emp_code)as emp_count,'HFC' as name,sum(p.dd_amount) as amount from pr_emp_payslip_deductions p where payslip_mid in(select id from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Regular') and p.dd_name = 'HFC' " +
                            "union all select count(p.emp_code) as emp_count,'Professional Tax' as dd_name,sum(p.dd_prof_tax) as amount from pr_emp_payslip p where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_prof_tax > 0 order by name";



                        //qry1 = " select sum(x.amount) as amount from ( " +
                        //    "(select sum(m.amount) as amount from pr_emp_deductions m join pr_deduction_field_master c on m.m_id = c.id join pr_emp_payslip p on p.emp_code = m.emp_code where month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') " +
                        //    "and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and m.amount > 0 and c.type = 'EPD') " +
                        //    "union all " +
                        //    "select sum(p.dd_provident_fund) as amount from pr_emp_payslip p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                        //    "and p.spl_type = 'Regular' and p.dd_provident_fund > 0 " +
                        //    "union all " +
                        //    "select sum(p.dd_income_tax) as amount from pr_emp_payslip p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_income_tax > 0" +
                        //    ") as x";

                        //qry1 = " select sum(x.amount) as amount from ( (select sum(m.amount) as amount from pr_emp_deductions m join pr_deduction_field_master c on m.m_id = c.id join pr_emp_payslip p on p.emp_code = m.emp_code where month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and m.amount > 0 and c.type = 'EPD')" +
                        //    "  union all select sum(p.dd_provident_fund) as amount from pr_emp_payslip p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_provident_fund > 0 " +
                        //    "union all select sum(p.dd_income_tax) as amount from pr_emp_payslip p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_income_tax > 0 " +
                        //    "union all select sum(p.amount) as amount from pr_emp_lic_details p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                        //    "union all select sum(p.amount) as amount from pr_emp_hfc_details p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') union all select sum(p.dd_prof_tax) as amount from pr_emp_payslip p where month(p.fm) = month('2019-12-01') AND year(p.fm) = year('2019-12-01') and p.spl_type = 'Regular' and p.dd_prof_tax > 0   ) as x";
                        qry1 = "  select sum(amount)as amount from(select count(m.emp_code) as emp_count,c.name,sum(m.amount) as amount from pr_emp_deductions m join pr_deduction_field_master c on m.m_id = c.id join pr_emp_payslip p on p.emp_code = m.emp_code where month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "')  and p.spl_type = 'Regular'  and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and m.amount > 0 and c.type = 'EPD'   group by c.name " +
                            "union all select count(p.emp_code) as emp_count,'Provident Fund' as name, sum(p.dd_provident_fund) as amount from pr_emp_payslip p   where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_provident_fund > 0 " +
                            "union all  select count(p.emp_code) as emp_count,'Income Tax' as name,sum(p.dd_income_tax) as amount from pr_emp_payslip p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_income_tax > 0 " +
                            //"union all select count(p.emp_code)as emp_count,'LIC' as name,sum(p.amount) as amount from pr_emp_lic_details p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')" +
                            //"union all select count(p.emp_code)as emp_count,'HFC' as name,sum(p.amount) as amount from pr_emp_hfc_details p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            "union all select count(p.emp_code)as emp_count,'LIC' as name,sum(p.dd_amount) as amount from pr_emp_payslip_deductions p where payslip_mid in(select id from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Regular') and p.dd_name = 'LIC' " +
                            "union all select count(p.emp_code)as emp_count,'HFC' as name,sum(p.dd_amount) as amount from pr_emp_payslip_deductions p where payslip_mid in(select id from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Regular') and p.dd_name = 'HFC' " +
                            "union all select count(p.emp_code) as emp_count,'Professional Tax' as dd_name,sum(p.dd_prof_tax) as amount from pr_emp_payslip p where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_prof_tax > 0) as x";

                        DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qry1);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtALL1 = ds.Tables[1];

                        foreach (DataRow drs in dtALL.Rows)
                        {
                            if (drs["emp_count"].ToString() !="0")
                            {
                                if (date != "")
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowCnt++,
                                        HRF = "R",
                                        column1 = "<span style='color:#ff0000'>S.No</span>",
                                        column2 = "<span style='color:#ff0000'>Deduction Description</span>",
                                        column3 = "<span style='color:#ff0000'>Amount Deducted</span>",
                                    });
                                }
                                date = "";
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = SlNo++.ToString(),
                                    column2 = drs["name"].ToString(),
                                    column3 = ReportColFooterAlign(drs["amount"].ToString()),

                                });

                            }

                        }

                        foreach (DataRow drs in dtALL1.Rows)
                        {
                            if (drs["amount"].ToString() != "")
                            {

                                lst.Add(new CommonReportModel
                                {

                                    RowId = RowCnt++,
                                    HRF = "F",
                                    column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooter(120, " Total", ReportColConvertToDecimal(drs["amount"].ToString()))


                                });
                            }

                        }


                    }
                    else if (empsta == "Regu" && rptsta == "Detailed")
                    {


                        //qry = "select c.name as deduction,m.emp_code as emp_code, E.ShortName as ShortName,D.name as  designation,m.amount as amount, m.fm from pr_emp_deductions m  join pr_deduction_field_master c on m.m_id = c.id  join Employees E ON E.EmpId = m.emp_code  join Designations D on E.CurrentDesignation = D.id join pr_emp_payslip p on p.emp_code = m.emp_code  and month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "')  and c.type = 'EPD' and m.amount>0 and p.spl_type = 'Regular' " +
                        //    " union all" +
                        //    " select 'Provident Fund' as deduction,p.emp_code as emp_code, E.ShortName as ShortName,D.name as designation,p.dd_provident_fund as amount, p.fm from  pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.dd_provident_fund > 0 and p.spl_type = 'Regular'" +
                        //    " union all" +
                        //    " select 'Income Tax' as deduction,p.emp_code as emp_code, E.ShortName as ShortName,D.name as designation,p.dd_income_tax as amount, p.fm from  pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.dd_income_tax > 0 and p.spl_type = 'Regular'   order by deduction";

                        qry = " select c.name as deduction,m.emp_code as emp_code, E.ShortName as ShortName,D.name as  designation," +
                            "m.amount as amount from pr_emp_deductions m  join pr_deduction_field_master c on m.m_id = c.id  " +
                            "join Employees E ON E.EmpId = m.emp_code join Designations D on E.CurrentDesignation = D.id join " +
                            "pr_emp_payslip p on p.emp_code = m.emp_code and month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') " +
                            "and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and c.type = 'EPD' and m.amount > 0 " +
                            "and p.spl_type = 'Regular' " +
                            "union all select 'Provident Fund' as deduction,p.emp_code as emp_code, E.ShortName as ShortName," +
                            "D.name as designation,p.dd_provident_fund as amount  from  pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  " +
                            "join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  " +
                            "and p.dd_provident_fund > 0 and p.spl_type = 'Regular' " +
                            "union all select 'Income Tax' as deduction,p.emp_code as emp_code, " +
                            "E.ShortName as ShortName,D.name as designation,p.dd_income_tax as amount from  pr_emp_payslip p join Employees E ON " +
                            "E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') " +
                            "AND year(p.fm) = year('" + str1 + "')  and p.dd_income_tax > 0 and p.spl_type = 'Regular' " +
                             //"union all select 'LIC' as deduction," +
                             //"p.emp_code as emp_code, E.ShortName as ShortName,D.name as designation,p.amount as amount, p.fm from  pr_emp_lic_details p join " +
                             //"Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') " +
                             //"AND year(p.fm) = year('" + str1 + "')  and p.amount > 0 " +
                             //"union all select 'HFC' as deduction,p.emp_code as emp_code, E.ShortName as ShortName," +
                             //"D.name as designation,p.amount as amount, p.fm from  pr_emp_hfc_details p join Employees E ON E.EmpId = p.emp_code  join " +
                             //"Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  " +
                             //"and p.amount != 0   " +

                             " union all select 'HFC' as deduction,p.emp_code as emp_code,E.ShortName as ShortName,D.name as designation,p.dd_amount as amount from pr_emp_payslip_deductions p " +
                            " join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id  " +
                            "where payslip_mid in(select id from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Regular' ) and p.dd_name = 'HFC'" +

                            " union all select 'LIC' as deduction,p.emp_code as emp_code,E.ShortName as ShortName,D.name as designation,p.dd_amount as amount from pr_emp_payslip_deductions p " +
                            " join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id  " +
                            "where payslip_mid in(select id from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Regular' ) and p.dd_name = 'LIC'" +

                            " union all select 'Professional Tax' as deduction,p.emp_code ,E.ShortName as ShortName,D.name as designation,p.dd_prof_tax as amount  " +
                            "from pr_emp_payslip p  join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id " +
                            "where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and p.dd_prof_tax > 0 " +
                            " order by deduction";



                        //qry1 = " select c.name as deduction,count(m.emp_code) as emp_code, sum(m.amount) as amount from pr_emp_deductions m join pr_deduction_field_master c on m.m_id = c.id  join Employees E ON E.EmpId = m.emp_code  join Designations D on E.CurrentDesignation = D.id  join pr_emp_payslip p on p.emp_code = m.emp_code  and month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "')  and c.type = 'EPD' and m.amount>0 and p.spl_type = 'Regular' group by c.name " +
                        //    " union all" +
                        //    " select 'Provident Fund' as deduction,count(p.emp_code) as emp_code, sum(p.dd_provident_fund) as amount from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.dd_provident_fund > 0 and p.spl_type = 'Regular'" +
                        //    " union all " +
                        //    " select 'Income Tax' as deduction,count(p.emp_code) as emp_code,sum(p.dd_income_tax) as amount from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.dd_income_tax > 0 and p.spl_type = 'Regular'   order by deduction";

                        qry1 = "  select c.name as deduction,count(m.emp_code) as emp_code, sum(m.amount) as amount from pr_emp_deductions m join pr_deduction_field_master c on m.m_id = c.id  join Employees E ON E.EmpId = m.emp_code  join Designations D on E.CurrentDesignation = D.id  join pr_emp_payslip p on p.emp_code = m.emp_code  and month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and c.type = 'EPD' and m.amount !=0 and p.spl_type = 'Regular' group by c.name  " +
                            "union all select 'Provident Fund' as deduction,count(p.emp_code) as emp_code, sum(p.dd_provident_fund) as amount from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.dd_provident_fund !=0 and p.spl_type = 'Regular' " +
                            "union all select 'Income Tax' as deduction,count(p.emp_code) as emp_code,sum(p.dd_income_tax) as amount from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.dd_income_tax != 0 and p.spl_type = 'Regular' " +
                           //"union all select 'LIC' as deduction,count(p.emp_code) as emp_code,sum(p.amount) as amount from pr_emp_lic_details p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                           //"union all select 'HFC' as deduction,count(p.emp_code) as emp_code,sum(p.amount) as amount from pr_emp_hfc_details p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +

                             "union all select 'LIC' as deduction,count(p.emp_code) as emp_code,sum(p.dd_amount) as amount from pr_emp_payslip_deductions p where payslip_mid in(select id from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Regular') and p.dd_name = 'LIC' " +
                             "union all select 'HFC' as deduction,count(p.emp_code) as emp_code,sum(p.dd_amount) as amount from pr_emp_payslip_deductions p where payslip_mid in(select id from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Regular') and p.dd_name = 'HFC' " +
                            "union all select 'Professional Tax' as deduction,count(p.emp_code) as emp_code, sum(p.dd_prof_tax) as amount from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.dd_provident_fund != 0 and p.spl_type = 'Regular' " +
                            "order by deduction";

                        DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qry1);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtALL1 = ds.Tables[1];
                        foreach (DataRow dr in dtALL.Rows)
                        {
                            deduction = dr["deduction"].ToString();


                            if (olddeduction != deduction && olddeduction != "")
                            {
                                CommonReportModel tot = getTotal(olddeduction, dtALL1);
                                tot.RowId = RowCnt++;
                                lst.Add(tot);

                            }

                            if (olddeduction != deduction)
                            {
                                SlNo = 1;
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "H",

                                    column1 = "<span style='color:#C8EAFB'>~</span>"
                                        + ReportColHeader(0, "Deduction Type", deduction),
                                    column2 = "`",
                                    column3 = "`",
                                    column4 = "`",
                                    column5 = "`",


                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = "S.No",
                                    column2 = "Emp Code",
                                    column3 = "Emp Name",
                                    column4 = "Designation",
                                    column5 = "Amount",
                                });

                            }
                            olddeduction = dr["deduction"].ToString();
                                                                                 
                            lst.Add(new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = SlNo++.ToString(),
                                column2 = dr["emp_code"].ToString(),
                                column3 = dr["ShortName"].ToString(),
                                column4 = dr["designation"].ToString(),
                                column5 = ReportColConvertToDecimal(dr["amount"].ToString()),

                            });
                        }
                                               
                        if (olddeduction != "")
                        {
                            CommonReportModel tot = getTotal(olddeduction, dtALL1);
                            tot.RowId = RowCnt++;
                            lst.Add(tot);

                        }
                    }

                    else if (empsta == "Supp" && rptsta == "Summary")
                    {
                        //qry = "select count(m.emp_code) as emp_count,c.name,sum(m.amount) as amount from pr_emp_adhoc_deduction_field m join pr_deduction_field_master c on m.m_id = c.id" +
                        //     " join pr_emp_payslip p on p.emp_code = m.emp_code where month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') " +
                        //     "and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.spl_type = 'Adhoc' and m.amount>0 and c.type = 'adhoc'  group by c.name ";

                        //qry = "select count(m.emp_code) as emp_count,c.name,sum(m.amount) as amount from pr_emp_adhoc_deduction_field m join pr_deduction_field_master c on m.m_id = c.id join pr_emp_payslip p on p.emp_code = m.emp_code where month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.spl_type = 'Adhoc'  and c.type = 'adhoc'  group by c.name  " +
                        //    " union all" +
                        //    " select count(p.emp_code) as emp_count,'Provident Fund' as name,sum(p.dd_provident_fund) as amount from pr_emp_payslip p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.spl_type = 'Adhoc' and p.dd_provident_fund > 0 " +
                        //    " union all " +
                        //    " select count(p.emp_code) as emp_count,'Income Tax' as name,sum(p.dd_income_tax) as amount from pr_emp_payslip p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.spl_type = 'Adhoc' and p.dd_income_tax > 0 ";

                        qry = "select count(m.emp_code) as emp_count,c.name,sum(m.amount) as amount " +
                            "from pr_emp_adhoc_deduction_field m " +
                            "join pr_deduction_field_master c on m.m_id = c.id " +
                            "join pr_emp_payslip p on p.emp_code = m.emp_code " +
                            "where month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') " +
                            "and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            "and p.spl_type = 'Adhoc'  and c.type = 'adhoc' and m.active = 1 group by c.name " +
                            "union all select count(p.emp_code) as emp_count,'Income Tax' as name,sum(p.dd_income_tax) as amount " +
                            "from pr_emp_payslip p where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            "and p.spl_type = 'Adhoc' and p.dd_income_tax > 0";
                        //qry1 = " select sum(m.amount) as amount from pr_emp_adhoc_deduction_field m join pr_deduction_field_master c on m.m_id = c.id " +
                        //    "join pr_emp_payslip p on p.emp_code = m.emp_code where month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') " +
                        //    "and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Adhoc' and m.amount>0 and c.type = 'adhoc'";

                        qry1 = "select sum(x.amount) as amount from ( " +
                            "(select sum(m.amount) as amount from pr_emp_adhoc_deduction_field m join pr_deduction_field_master c on m.m_id = c.id join pr_emp_payslip p on p.emp_code = m.emp_code where month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Adhoc' and m.amount > 0 and c.type = 'adhoc')" +
                            " union all " +
                            "select sum(p.dd_provident_fund) as amount from pr_emp_payslip p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.spl_type = 'Adhoc' and p.dd_provident_fund > 0  " +
                            "union all " +
                            "select sum(p.dd_income_tax) as amount from pr_emp_payslip p  where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.spl_type = 'Adhoc' and  p.dd_income_tax > 0 ) as x";

                        DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qry1);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtALL1 = ds.Tables[1];

                        foreach (DataRow drs in dtALL.Rows)
                        {
                            if (date != "")
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = "<span style='color:#ff0000'>S.No</span>",
                                    column2 = "<span style='color:#ff0000'>Deduction Description</span>",
                                    column3 = "<span style='color:#ff0000'>Amount Deducted</span>",
                                });
                            }
                            date = "";

                            lst.Add(new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = SlNo++.ToString(),
                                column2 = drs["name"].ToString(),
                                column3 = ReportColConvertToDecimal(drs["amount"].ToString()),

                            });


                        }

                        foreach (DataRow drs in dtALL1.Rows)
                        {
                            if (drs["amount"].ToString() != "")
                            {
                                lst.Add(new CommonReportModel
                                {

                                    RowId = RowCnt++,
                                    HRF = "F",
                                    column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooter(100, " Total", ReportColConvertToDecimal(drs["amount"].ToString()))


                                });

                            }
                        }
                    }
                    else if (empsta == "Supp" && rptsta == "Detailed")
                    {
                        //qry = " select c.name as deduction,m.emp_code as emp_code, E.ShortName as ShortName,D.name as " +
                        //    " designation,m.amount as amount, m.fm from pr_emp_adhoc_deduction_field m  join pr_deduction_field_master c on m.m_id = c.id" +
                        //    "  join Employees E ON E.EmpId = m.emp_code  join Designations D on E.CurrentDesignation = D.id" +
                        //    " join pr_emp_payslip p on p.emp_code = m.emp_code where c.type = 'adhoc'  and p.spl_type = 'Adhoc'" +
                        //    " and month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "')" +
                        //    " and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') order by deduction ";

                        //qry = "select c.name as deduction,m.emp_code as emp_code, E.ShortName as ShortName,D.name as  designation,m.amount as amount, m.fm from pr_emp_adhoc_deduction_field m  join pr_deduction_field_master c on m.m_id = c.id  join Employees E ON E.EmpId = m.emp_code  join Designations D on E.CurrentDesignation = D.id join pr_emp_payslip p on p.emp_code = m.emp_code where c.type = 'adhoc'  and p.spl_type = 'Adhoc' and month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                        //    " union all" +
                        //    " select 'Provident Fund' as deduction,p.emp_code as emp_code, E.ShortName as ShortName,D.name as designation,p.dd_provident_fund as amount, p.fm from pr_emp_payslip p  join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   where p.spl_type = 'Adhoc' and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and dd_provident_fund > 0" +
                        //    " union all" +
                        //    " select 'Income Tax' as deduction,p.emp_code as emp_code, E.ShortName as ShortName,D.name as designation,p.dd_income_tax as amount, p.fm from pr_emp_payslip p  join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   where p.spl_type = 'Adhoc' and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and dd_income_tax > 0 order by deduction   ";
                        
                        qry = "select c.name as deduction,m.emp_code as emp_code, E.ShortName as ShortName,D.name as designation," +
                            "m.amount as amount, m.fm from pr_emp_adhoc_deduction_field m join pr_deduction_field_master " +
                            "c on m.m_id = c.id join Employees E ON E.EmpId = m.emp_code join Designations " +
                            "D on E.CurrentDesignation = D.id join pr_emp_payslip p on p.emp_code = m.emp_code " +
                            "where c.type = 'adhoc' and m.active=1 and p.spl_type = 'Adhoc' " +
                            "and month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') " +
                            "and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            "union all select 'Income Tax' as deduction,p.emp_code as emp_code, E.ShortName as ShortName," +
                            "D.name as designation,p.dd_income_tax as amount, p.fm from pr_emp_payslip p " +
                            "join Employees E ON E.EmpId = p.emp_code join " +
                            "Designations D on E.CurrentDesignation = D.id where p.spl_type = 'Adhoc' " +
                            "and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            "and dd_income_tax > 0 order by deduction ";

                        //qry1 = "  select c.name as deduction,count(m.emp_code) as emp_code, sum(m.amount) as amount from pr_emp_adhoc_deduction_field m join pr_deduction_field_master c on m.m_id = c.id" +
                        //    "  join Employees E ON E.EmpId = m.emp_code  join Designations D on E.CurrentDesignation = D.id" +
                        //    "  join pr_emp_payslip p on p.emp_code = m.emp_code where c.type = 'adhoc'  and p.spl_type = 'Adhoc'" +
                        //    " and month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "')" +
                        //    " and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')" +
                        //    "group by c.name ";

                        //qry1 = "select c.name as deduction,count(m.emp_code) as emp_code, sum(m.amount) as amount from pr_emp_adhoc_deduction_field m join pr_deduction_field_master c on m.m_id = c.id  join Employees E ON E.EmpId = m.emp_code  join Designations D on E.CurrentDesignation = D.id  join pr_emp_payslip p on p.emp_code = m.emp_code where c.type = 'adhoc'  and p.spl_type = 'Adhoc' and month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') group by c.name " +
                        //    " union all" +
                        //    " select 'Provident Fund' as deduction,count(p.emp_code) as emp_code, sum(p.dd_provident_fund) as amount from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.dd_provident_fund > 0 and p.spl_type = 'Adhoc'" +
                        //    " union all" +
                        //    " select 'Income Tax' as deduction,count(p.emp_code) as emp_code,sum(p.dd_income_tax) as amount from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id   and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.dd_income_tax > 0 and p.spl_type = 'Adhoc'   order by deduction";

                        qry1 = "select c.name as deduction,count(m.emp_code) as emp_code, sum(m.amount) as amount from " +
                            "pr_emp_adhoc_deduction_field m join pr_deduction_field_master c on m.m_id = c.id " +
                            "join Employees E ON E.EmpId = m.emp_code join Designations D on E.CurrentDesignation = D.id " +
                            "join pr_emp_payslip p on p.emp_code = m.emp_code where c.type = 'adhoc' and m.active=1 and p.spl_type = 'Adhoc'" +
                            " and month(m.fm) = month('" + str1 + "') AND year(m.fm) = year('" + str1 + "') and " +
                            "month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') group by c.name " +
                            "union all select 'Income Tax' as deduction,count(p.emp_code) as emp_code,sum(p.dd_income_tax) " +
                            "as amount from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code join Designations" +
                            " D on E.CurrentDesignation = D.id and month(p.fm) = month('" + str1 + "') AND " +
                            "year(p.fm) = year('" + str1 + "') and p.dd_income_tax > 0 and p.spl_type = 'Adhoc' order by deduction";
                        DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qry1);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtALL1 = ds.Tables[1];
                        
                        foreach (DataRow dr in dtALL.Rows)
                        {
                            deduction = dr["deduction"].ToString();


                            if (olddeduction != deduction && olddeduction != "")
                            {
                                CommonReportModel tot = getTotal(olddeduction, dtALL1);
                                tot.RowId = RowCnt++;
                                lst.Add(tot);

                            }

                            if (olddeduction != deduction)
                            {
                                SlNo = 1;
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "H",

                                    column1 = "<span style='color:#C8EAFB'>~</span>"
                                        + ReportColHeader(0, "Deduction Type", deduction)
                                        
                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = "S.No",
                                    column2 = "Emp Code",
                                    column3 = "Emp Name",
                                    column4 = "Designation",
                                    column5 = "Amount",
                                });

                            }
                            olddeduction = dr["deduction"].ToString();
                                                                                 
                            lst.Add(new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = SlNo++.ToString(),
                                column2 = dr["emp_code"].ToString(),
                                column3 = dr["ShortName"].ToString(),
                                column4 = dr["designation"].ToString(),
                                column5 = ReportColConvertToDecimal(dr["amount"].ToString()),

                            });
                        }
                                               
                        if (olddeduction != "")
                        {
                            CommonReportModel tot = getTotal(olddeduction, dtALL1);
                            tot.RowId = RowCnt++;
                            lst.Add(tot);

                        }
                    }

                }
            }

            else
            {
                if (empsta != "undefined" && rptsta != "undefined")
                {

                    if (empsta == "Regu" && rptsta == "Summary")
                    {
                        //qry = "select count(m.emp_code) as emp_count,m.dd_name,sum(m.dd_amount) as amount from pr_emp_payslip_deductions m " +
                        //    " join pr_emp_payslip p on p.id = m.payslip_mid  where  " +
                        //    " month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.spl_type = 'Regular' and m.dd_amount>0 and m.dd_type='EPD' group by m.dd_name ";

                        qry = "select count(m.emp_code) as emp_count,m.dd_name,sum(m.dd_amount) as amount from pr_emp_payslip_deductions m  join pr_emp_payslip p on p.id = m.payslip_mid  where   month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            " and p.spl_type = 'Regular' and m.dd_amount>0 and  (m.dd_type != 'Loan' and m.dd_type != 'adhoc' or m.dd_type='Adj_Pay' or m.dd_type='Dep_Ent') group by m.dd_name " +
                            " union all " +
                            " select count(p.emp_code) as emp_count,'Provident Fund' as dd_name,sum(p.dd_provident_fund) as amount from pr_emp_payslip p   where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            " and p.spl_type = 'Regular' and p.dd_provident_fund > 0 " +
                            " union all " +
                            " select count(p.emp_code) as emp_count,'Income Tax' as dd_name,sum(p.dd_income_tax) as amount from pr_emp_payslip p   where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  " +
                            " and p.spl_type = 'Regular' and p.dd_income_tax > 0  " +
                            " union all  " +
                            "select count(p.emp_code) as emp_count,'Professional Tax' as dd_name,sum(p.dd_prof_tax) as amount " +
                            "from pr_emp_payslip p   where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' " +
                            "and p.dd_prof_tax > 0 "; // +
                            //"union all select count(emp_code) as emp_count,'Personal Loan' as dd_name,sum(dd_amount)as amount from pr_emp_payslip_deductions" +
                            //" where dd_name = 'Personal Loan' and payslip_mid in (select id from pr_emp_payslip where fm = '" + str1 + "' and spl_type = 'Regular')  order by dd_name";
                            
                        //qry1 = " select sum(m.dd_amount) as amount from pr_emp_payslip_deductions m " +
                        //   "join pr_emp_payslip p on p.id = m.payslip_mid  where  " +
                        //   " month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and m.dd_amount>0 and m.dd_type='EPD'  ";

                        qry1 = " select sum(x.amount) as amount from (" +
                            "(select sum(m.dd_amount) as amount from pr_emp_payslip_deductions m join pr_emp_payslip p on p.id = m.payslip_mid" +
                            " where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')" +
                            " and p.spl_type = 'Regular' and m.dd_amount > 0 and  (m.dd_type != 'Loan' and m.dd_type != 'adhoc' or m.dd_type='Adj_Pay' or m.dd_type='Dep_Ent'))" +
                            " union all " +
                            "select sum(p.dd_provident_fund) as amount " +
                            "from pr_emp_payslip P join Employees E ON E.EmpId = p.emp_code " +
                            "where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and dd_provident_fund > 0 " +
                            "union all " +
                            "select sum(p.dd_income_tax) as amount from pr_emp_payslip P join Employees E ON E.EmpId = p.emp_code" +
                            " where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and dd_income_tax > 0 union all select sum(p.dd_prof_tax) as amount " +
                            " from pr_emp_payslip P join Employees E ON E.EmpId = p.emp_code where month(p.fm) = month('" + str1 + "') AND " +
                            "year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and dd_prof_tax > 0 ) as x";

                        DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qry1);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtALL1 = ds.Tables[1];

                        foreach (DataRow drs in dtALL.Rows)
                        {
                            if (date != "")
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = "<span style='color:#ff0000'>S.No</span>",
                                    column2 = "<span style='color:#ff0000'>Deduction Description</span>",
                                    column3 = "<span style='color:#ff0000'>Amount Deducted</span>",
                                });
                            }
                            date = "";
                            lst.Add(new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = SlNo++.ToString(),
                                column2 = drs["dd_name"].ToString(),
                                column3 = ReportColFooterAlign(drs["amount"].ToString()),

                            });

                        }

                        foreach (DataRow drs in dtALL1.Rows)
                        {
                            if (drs["amount"].ToString() != "")
                            {

                                lst.Add(new CommonReportModel
                                {

                                    RowId = RowCnt++,
                                    HRF = "F",
                                    column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooter(120, " Total", ReportColConvertToDecimal(drs["amount"].ToString()))


                                });
                            }
                        }
                    }
                    else if (empsta == "Regu" && rptsta == "Detailed")
                    {
                        //qry = " select m.dd_name as deduction,m.emp_code as emp_code, E.ShortName as ShortName,D.name as " +
                        //   " designation,m.dd_amount as amount from pr_emp_payslip_deductions m  " +
                        //   "  join Employees E ON E.EmpId = m.emp_code  join Designations D on E.CurrentDesignation = D.id" +
                        //   " join pr_emp_payslip p on p.id = m.payslip_mid " +
                        //   " Where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and m.dd_amount>0 and p.spl_type = 'Regular' and m.dd_type='EPD' order by deduction ";

                        qry = " select m.dd_name as deduction,m.emp_code as emp_code, E.ShortName as ShortName,D.name as  designation, m.dd_amount as amount from pr_emp_payslip_deductions m" +
                            " join Employees E ON E.EmpId = m.emp_code  join Designations D on E.CurrentDesignation = D.id  join pr_emp_payslip p on p.id = m.payslip_mid" +
                            " Where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and m.dd_amount > 0 and p.spl_type = 'Regular' and  (m.dd_type != 'Loan' and m.dd_type != 'adhoc' or m.dd_type='Adj_Pay' or m.dd_type='Dep_Ent') " +
                            " union all" +
                            " select 'Provident Fund' as deduction,p.emp_code as emp_code, E.ShortName as ShortName,D.name as designation, p.dd_provident_fund as amount from pr_emp_payslip P " +
                            " join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            " and p.spl_type = 'Regular' and dd_provident_fund > 0 " +
                            " union all " +
                            " select 'Income Tax' as deduction,p.emp_code as emp_code, E.ShortName as ShortName,D.name as designation, p.dd_income_tax as amount from pr_emp_payslip p " +
                            " join Employees E ON E.EmpId = p.emp_code join Designations D on E.CurrentDesignation = D.id where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            " and p.spl_type = 'Regular' and dd_income_tax > 0 " +
                            " union all  select 'Professional Tax' as deduction,p.emp_code as emp_code, E.ShortName as ShortName, D.name as designation, p.dd_prof_tax amount from pr_emp_payslip p  join Employees E ON E.EmpId = p.emp_code join Designations D on E.CurrentDesignation = D.id where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and dd_prof_tax > 0 " +
                            //" union all  select 'Personal Loan' as deduction,pd.emp_code as emp_code ,E.ShortName as ShortName,D.name as designation,dd_amount as amount from pr_emp_payslip_deductions pd  join Employees E ON E.EmpId = pd.emp_code and pd.dd_name = 'Personal Loan' join pr_emp_payslip p on p.id = pd.payslip_mid join Designations D on E.CurrentDesignation = D.id where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and dd_amount > 0 
                            " order by deduction ";

                        //qry1 = "  select m.dd_name as deduction,count(m.emp_code) as emp_code, sum(m.dd_amount) as amount from pr_emp_payslip_deductions m " +
                        //    "  join Employees E ON E.EmpId = m.emp_code  join Designations D on E.CurrentDesignation = D.id" +
                        //    "  join pr_emp_payslip p on p.id = m.payslip_mid where  m.dd_amount>0 and p.spl_type = 'Regular' " +
                        //    " and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                        //    "  group by dd_name ";

                        qry1 = "   select m.dd_name as deduction,count(m.emp_code) as emp_code, sum(m.dd_amount) as amount from pr_emp_payslip_deductions m   join Employees E ON E.EmpId = m.emp_code" +
                            "   join Designations D on E.CurrentDesignation = D.id  join pr_emp_payslip p on p.id = m.payslip_mid   where m.dd_amount > 0 and p.spl_type = 'Regular'  " +
                            "  and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and  (m.dd_type != 'Loan' and m.dd_type != 'adhoc' or m.dd_type='Adj_Pay' or m.dd_type='Dep_Ent') group by dd_name" +
                            "  union all" +
                            "  select 'Provident Fund' as deduction,count(p.emp_code) as emp_code, sum(p.dd_provident_fund) as amount   from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code" +
                            "  join Designations D on E.CurrentDesignation = D.id   where p.spl_type = 'Regular'  and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') " +
                            "  and dd_provident_fund > 0" +
                            "  union all" +
                            "  select 'Income Tax' as deduction,count(p.emp_code) as emp_code, sum(p.dd_income_tax) as amount   from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code" +
                            "  join Designations D on E.CurrentDesignation = D.id    where p.spl_type = 'Regular'  and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and dd_income_tax > 0 " +
                            "  union all   select 'Professional Tax' as deduction,count(p.emp_code) as emp_code, sum(p.dd_prof_tax) as amount   from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code   join Designations D on E.CurrentDesignation = D.id  where p.spl_type = 'Regular'  and month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and dd_prof_tax > 0  " +
                            //" union all  select 'Personal Loan' as deduction,count(pd.emp_code) as emp_code,sum(dd_amount) as amount from pr_emp_payslip_deductions pd  join Employees E ON E.EmpId = pd.emp_code and pd.dd_name = 'Personal Loan' join pr_emp_payslip p on p.id = pd.payslip_mid join Designations D on E.CurrentDesignation = D.id where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Regular' and dd_amount > 0  " +
                            "order by deduction";

                        DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qry1);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtALL1 = ds.Tables[1];
                        foreach (DataRow dr in dtALL.Rows)
                        {

                            deduction = dr["deduction"].ToString();


                            if (olddeduction != deduction && olddeduction != "")
                            {
                                CommonReportModel tot = getTotal(olddeduction, dtALL1);
                                tot.RowId = RowCnt++;
                                lst.Add(tot);

                            }

                            if (olddeduction != deduction)
                            {
                                SlNo = 1;
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "H",

                                    column1 = "<span style='color:#C8EAFB'>~</span>"
                                        + ReportColHeader(0, "Deduction Type", deduction),
                                    column2 = "`",
                                    column3 = "`",
                                    column4 = "`",
                                    column5 = "`",


                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = "S.No",
                                    column2 = "Emp Code",
                                    column3 = "Emp Name",
                                    column4 = "Designation",
                                    column5 = "Amount",
                                });

                            }
                            olddeduction = dr["deduction"].ToString();




                            lst.Add(new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = SlNo++.ToString(),
                                column2 = dr["emp_code"].ToString(),
                                column3 = dr["ShortName"].ToString(),
                                column4 = dr["designation"].ToString(),
                                column5 = ReportColConvertToDecimal(dr["amount"].ToString()),

                            });
                        }



                        if (olddeduction != "")
                        {
                            CommonReportModel tot = getTotal(olddeduction, dtALL1);
                            tot.RowId = RowCnt++;
                            lst.Add(tot);

                        }

                    }

                    else if (empsta == "Supp" && rptsta == "Summary")
                    {
                        //qry = "select count(m.emp_code) as emp_count,m.dd_name as name,sum(m.dd_amount) as amount from pr_emp_payslip_deductions m" +
                        //    "  join Employees E ON E.EmpId = m.emp_code" +
                        //     " join pr_emp_payslip p on p.id = m.payslip_mid where  " +
                        //     " month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.spl_type = 'Adhoc' and m.dd_amount>0  group by m.dd_name ";

                        qry = " select count(m.emp_code) as emp_count,m.dd_name as name,sum(m.dd_amount) as amount from pr_emp_payslip_deductions m  join Employees E ON E.EmpId = m.emp_code " +
                            " join pr_emp_payslip p on p.id = m.payslip_mid where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')   and p.spl_type = 'Adhoc'" +
                            " and m.dd_amount > 0   group by dd_name" +
                            " union all" +
                            " select count(p.emp_code) as emp_count,'Provident Fund' as name,sum(p.dd_provident_fund) as amount from pr_emp_payslip P  join Employees E ON E.EmpId = p.emp_code" +
                            " where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')   and p.spl_type = 'Adhoc' and dd_provident_fund != 0 " +
                            " union all " +
                            " select count(p.emp_code) as emp_count,'Income Tax' as name,sum(p.dd_income_tax) as amount from pr_emp_payslip P  join Employees E ON E.EmpId = p.emp_code" +
                            " where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')   and p.spl_type = 'Adhoc' and dd_income_tax > 0 order by name";

                        //qry1 = " select sum(m.dd_amount) as amount from pr_emp_payslip_deductions m  " +
                        //   "join pr_emp_payslip p on p.id = m.payslip_mid  join Employees E ON E.EmpId = m.emp_code where  " +
                        //   " month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and p.spl_type = 'Adhoc' and m.dd_amount>0 ";

                        qry1 = " select sum(x.amount) as amount from ( " +
                            "(select sum(m.dd_amount) as amount from pr_emp_payslip_deductions m join Employees E ON E.EmpId = m.emp_code " +
                            "join pr_emp_payslip p on p.id = m.payslip_mid where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  and p.spl_type = 'Adhoc'" +
                            "and m.dd_amount > 0  group by dd_name) " +
                            "union all " +
                            "select sum(p.dd_provident_fund) as amount from pr_emp_payslip P join Employees E ON E.EmpId = p.emp_code where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  " +
                            "and p.spl_type = 'Adhoc' and dd_provident_fund != 0  " +
                            "union all " +
                            "select sum(p.dd_income_tax) as amount from pr_emp_payslip P join Employees E ON E.EmpId = p.emp_code where month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  " +
                            "and p.spl_type = 'Adhoc' and dd_income_tax > 0) as x";

                        DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qry1);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtALL1 = ds.Tables[1];

                        foreach (DataRow drs in dtALL.Rows)
                        {

                            if (date != "")
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = "<span style='color:#ff0000'>S.No</span>",
                                    column2 = "<span style='color:#ff0000'>Deduction Description</span>",
                                    column3 = "<span style='color:#ff0000'>Amount Deducted</span>",
                                });
                            }
                            date = "";
                            if (drs["amount"].ToString() != "")
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = SlNo++.ToString(),
                                    column2 = drs["name"].ToString(),
                                    column3 = ReportColFooterAlign(drs["amount"].ToString()),

                                });
                            }
                            else
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = SlNo++.ToString(),
                                    column2 = drs["name"].ToString(),
                                    column3 = ReportColFooterAlign( "0"),

                                });
                            }


                        }

                        foreach (DataRow drs in dtALL1.Rows)
                        {
                            if (drs["amount"].ToString() != "")
                            {
                                lst.Add(new CommonReportModel
                                {

                                    RowId = RowCnt++,
                                    HRF = "F",
                                    column1 = "<span style='color:#eef8fd'>^</span>"
                                + ReportColFooter(100, " Total", ReportColConvertToDecimal(drs["amount"].ToString()))


                                });
                            }

                        }
                    }
                    else if (empsta == "Supp" && rptsta == "Detailed")
                    {
                        //qry = "select m.dd_name as deduction,m.emp_code as emp_code, E.ShortName as ShortName,D.name as designation," +
                        //    " m.dd_amount as amount from pr_emp_payslip_deductions m" +
                        //    " join pr_emp_payslip p on p.id = m.payslip_mid join Employees E ON E.EmpId = m.emp_code" +
                        //    " join Designations D on E.CurrentDesignation = D.id " +
                        //    " where  month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') AND p.spl_type = 'Adhoc' and m.dd_amount>0 ORDER BY m.dd_name";

                        qry = "select m.dd_name as deduction,m.emp_code as emp_code, E.ShortName as ShortName, D.name as designation, m.dd_amount as amount from pr_emp_payslip_deductions m" +
                            " join pr_emp_payslip p on p.id = m.payslip_mid join Employees E ON E.EmpId = m.emp_code join Designations D on E.CurrentDesignation = D.id" +
                            " where  month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') AND p.spl_type = 'Adhoc' and m.dd_amount>0" +
                            " union all" +
                            " select 'Provident Fund',emp_code,ShortName,D.Name,dd_provident_fund from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code" +
                            " join Designations D on E.CurrentDesignation = D.id where  month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') AND p.spl_type = 'Adhoc' and dd_provident_fund != 0 " +
                            " union all" +
                            " select 'Income Tax',emp_code,ShortName,D.Name,dd_income_tax from pr_emp_payslip p join Employees E ON E.EmpId = p.emp_code  join Designations D on E.CurrentDesignation = D.id" +
                            " where  month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "')  AND p.spl_type = 'Adhoc' and dd_income_tax > 0  order by deduction";

                        //qry1 = " select m.dd_name as deduction,count(m.emp_code), sum(m.dd_amount) as amount " +
                        //    " from pr_emp_payslip_deductions m join pr_emp_payslip p  on p.id = m.payslip_mid  join Employees E ON E.EmpId = m.emp_code" +
                        //    " where  month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and m.dd_amount>0 " +
                        //    " AND p.spl_type = 'Adhoc'  group by dd_name";

                        qry1 = " select m.dd_name as deduction,count(m.emp_code), sum(m.dd_amount) as amount  from pr_emp_payslip_deductions m join pr_emp_payslip p  on p.id = m.payslip_mid  join Employees E ON E.EmpId = m.emp_code  where  month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and m.dd_amount>0  AND p.spl_type = 'Adhoc'  group by dd_name" +
                            "   union all" +
                            "   select 'Provident Fund' as deduction,count(p.emp_code), sum(p.dd_provident_fund) as amount  from pr_emp_payslip p    join Employees E ON E.EmpId = p.emp_code  where  month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and (p.dd_provident_fund != 0)  AND p.spl_type = 'Adhoc'" +
                            "   union all" +
                            "   select 'Income Tax' as deduction,count(p.emp_code), sum(p.dd_income_tax) as amount  from pr_emp_payslip p    join Employees E ON E.EmpId = p.emp_code  where  month(p.fm) = month('" + str1 + "') AND year(p.fm) = year('" + str1 + "') and (p.dd_income_tax > 0 or dd_income_tax is not null)  AND p.spl_type = 'Adhoc'  ";

                        DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qry1);
                        DataTable dtALL = ds.Tables[0];
                        DataTable dtALL1 = ds.Tables[1];


                        foreach (DataRow dr in dtALL.Rows)
                        {
                            deduction = dr["deduction"].ToString();


                            if (olddeduction != deduction && olddeduction != "")
                            {
                                CommonReportModel tot = getTotal(olddeduction, dtALL1);
                                tot.RowId = RowCnt++;
                                lst.Add(tot);

                            }

                            if (olddeduction != deduction)
                            {
                                SlNo = 1;
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "H",

                                    column1 = "<span style='color:#C8EAFB'>~</span>"
                                        + ReportColHeader(0, "Deduction Type", deduction)


                                });

                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = "S.No",
                                    column2 = "Emp Code",
                                    column3 = "Emp Name",
                                    column4 = "Designation",
                                    column5 = "Amount",
                                });

                            }
                            olddeduction = dr["deduction"].ToString();




                            lst.Add(new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = SlNo++.ToString(),
                                column2 = dr["emp_code"].ToString(),
                                column3 = dr["ShortName"].ToString(),
                                column4 = dr["designation"].ToString(),
                                column5 = ReportColConvertToDecimal(dr["amount"].ToString()),

                            });
                        }



                        if (olddeduction != "")
                        {
                            CommonReportModel tot = getTotal(olddeduction, dtALL1);
                            tot.RowId = RowCnt++;
                            lst.Add(tot);

                        }
                    }

                }
            }


            return lst;

        }





        #endregion

        private CommonReportModel getTotal(string type, DataTable dt)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["deduction"].ToString() == type)
                .Select(x => new { tot = x["amount"].ToString() }).FirstOrDefault();

            var tot = new CommonReportModel
            {

                RowId = 0,
                HRF = "F",
                //column1 = "<span style='color:#eef8fd'>^</span>"

                //+ ReportColFooter(240, "Total Amount", val.tot)

                column1 = "Total Amount",
                column5 = ReportColConvertToDecimal(val.tot)

            };

            return tot;
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

        public string ReportColFooterAlign( string value)
        {
            string sRet = "";
            if (value == "")
            {
                value = "0";
            }
            decimal Drvalue = Convert.ToDecimal(value.ToString()) + 0.00M;
            decimal DPT = Convert.ToDecimal(String.Format("{0:0.00}", Drvalue));
            string NwDPT = String.Format("{0:n}", DPT);

            sRet +=  "<span style='float:right'>" + NwDPT + "</span>";

            return sRet;
        }

    }
}
