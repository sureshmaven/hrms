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

namespace PayRollBusiness.Reports
{
    public class LICReportBusiness : BusinessBase
    {
        public LICReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }


        string qrySel;
        string qrysel1;

        string oldtype = "";
        string oldcode = "";
        string type = "";
        string code = "";
        string Totals = "";
        int RowCnt = 0;
        int SlNo = 1;
        int Total = 0;
        int Ttl;
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        IList<LICReport> HFCGridval = new List<LICReport>();

        CommonReportModel clst = new CommonReportModel();
        public async Task<IList<LICReport>> GetLICReports(string empicode, string LICType, string Month)
        {
            if (empicode.Contains("^"))
            {
                empicode = "0";
                LICType = "0";
                Month = "01-01-01";
            }
            DateTime str = Convert.ToDateTime(Month);
            string str1 = str.ToString("yyyy-MM-dd");
            string Type = LICType;
            string empid = empicode;
            // employee id not equal to null and both LIC and HFC is selected 
        if (empid != "All" && (Type == "0" ))
            {
                //qrySel = " SELECT 'HFC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_hfc_details L JOIN Employees E ON E.empid=L.emp_code " +
                //                        "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm)  AND L.emp_code in (" + empid + ")" +
                //                    "UNION ALL" +
                //                    " SELECT 'LIC'as Type,emp_code,ShortName,account_no,amount FROM pr_emp_lic_details L JOIN Employees E ON E.empid=L.emp_code " +
                //                        "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm)  AND L.emp_code in (" + empid + ")";
                qrySel = " select 'LIC' as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount from pr_emp_payslip_deductions d  " +
                    "JOIN Employees E ON E.empid = d.emp_code join pr_emp_lic_details l on d.emp_code = l.emp_code and l.amount=d.dd_amount join pr_emp_payslip p " +
                    "on d.payslip_mid = p.id where d.emp_code in (" + empid + ") and p.fm = '" + str1 + "' and l.fm='" + str1 + "' and d.dd_name = 'LIC' and d.dd_amount > 0 " +
                    "union all select 'HFC' as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount from pr_emp_payslip_deductions d " +
                    "JOIN Employees E ON E.empid = d.emp_code join pr_emp_hfc_details l on d.emp_code = l.emp_code and l.amount=d.dd_amount join pr_emp_payslip p " +
                    "on d.payslip_mid = p.id where d.emp_code in (" + empid + ") and p.fm = '" + str1 + "' and l.fm='" + str1 + "' and d.dd_name = 'HFC' and d.dd_amount > 0; ";
                DataTable dtHFC = await _sha.Get_Table_FromQry(qrySel);

                try
                {
                   
                    foreach (DataRow dr in dtHFC.Rows)
                    
                        
                        HFCGridval.Add(new LICReport
                      {
                          Type = dr["Type"].ToString(),
                          Code = dr["emp_code"].ToString(),
                          Name = dr["ShortName"].ToString(),
                          Account_No = dr["account_no"].ToString(),
                          Amount = ReportColConvertToDecimal(dr["amount"].ToString()),

                          Total = ReportColConvertToDecimal(dr["amount"].ToString())


                      });
                    
                }
                catch (Exception ex)
                {

                }
            }
        else   if (empid != "All" && (Type == "null" || Type == "2,3"))
            {
                //qrySel = " SELECT 'HFC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_hfc_details L JOIN Employees E ON E.empid=L.emp_code " +
                //        "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm)  AND L.emp_code in (" + empid + ")" +
                //    "UNION ALL" +
                //    " SELECT 'LIC'as Type,emp_code,ShortName,account_no,amount FROM pr_emp_lic_details L JOIN Employees E ON E.empid=L.emp_code " +
                //        "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm)  AND L.emp_code in (" + empid + ")";

                qrySel = " select 'LIC' as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount  from pr_emp_payslip_deductions d  " +
                    "JOIN Employees E ON E.empid = d.emp_code join pr_emp_lic_details l on d.emp_code = l.emp_code  and l.amount=d.dd_amount  join " +
                    "pr_emp_payslip p on d.payslip_mid = p.id where d.emp_code in (" + empid + ") and p.fm = '" + str1 + "' and l.fm='" + str1 + "' and d.dd_name = 'LIC' " +
                    "and d.dd_amount > 0 union all select 'HFC' as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount " +
                    "from pr_emp_payslip_deductions d JOIN Employees E ON E.empid = d.emp_code join pr_emp_hfc_details l" +
                    " on d.emp_code = l.emp_code  and l.amount=d.dd_amount  join pr_emp_payslip p on d.payslip_mid = p.id where d.emp_code in (" + empid + ") " +
                    "and p.fm = '" + str1 + "' and l.fm='" + str1 + "' and d.dd_name = 'HFC' and d.dd_amount > 0; ";
                DataTable dtHFC = await _sha.Get_Table_FromQry(qrySel);

                try
                {
                    foreach (DataRow dr in dtHFC.Rows)
                    {
                        Ttl = Total + Convert.ToInt32(dr["amount"]);

                        HFCGridval.Add(new LICReport
                        {
                            Type = dr["Type"].ToString(),
                            Code = dr["emp_code"].ToString(),
                            Name = dr["ShortName"].ToString(),
                            Account_No = dr["account_no"].ToString(),
                            Amount = ReportColConvertToDecimal(dr["amount"].ToString()),
                            Total = ReportColConvertToDecimal(Ttl.ToString())


                        });
                    }
                }
                catch (Exception ex)
                {

                }

            }

            // employee id not equal to null and either LIC or HFC is selected 
            else if (empid != "All" && (Type == "2" || Type == "3"))
            {
                if (Type == "2")
                {

                    //qrySel = " SELECT 'HFC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_hfc_details L JOIN Employees E ON E.empid=L.emp_code " +
                    //    "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm)  AND L.emp_code in (" + empid + ")";

                    qrySel = "select 'HFC' as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount " +
                        "from pr_emp_payslip_deductions d  JOIN Employees E ON E.empid = d.emp_code join pr_emp_hfc_details l " +
                        "on d.emp_code = l.emp_code join pr_emp_payslip p on d.payslip_mid = p.id" +
                        " where p.fm = '" + str1 + "' and l.fm='" + str1 + "' and d.emp_code in (" + empid + ") and d.dd_name = 'HFC' and d.dd_amount > 0";
                
                    DataTable dtHFC = await _sha.Get_Table_FromQry(qrySel);

                    try
                    {
                        foreach (DataRow dr in dtHFC.Rows)
                        {
                            Ttl = Total + Convert.ToInt32(dr["amount"]);


                            HFCGridval.Add(new LICReport
                            {
                                Type = dr["Type"].ToString(),
                                Code = dr["emp_code"].ToString(),
                                Name = dr["ShortName"].ToString(),
                                Account_No = dr["account_no"].ToString(),
                                Amount = ReportColConvertToDecimal(dr["amount"].ToString()),

                                Total = ReportColConvertToDecimal(Ttl.ToString())

                            });
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }

                //HFC with empid if (Type[0] == "3")
                else
                {

                    //qrySel = "SELECT 'LIC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_lic_details L JOIN Employees E ON E.empid=L.emp_code " +
                    //    "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm)  AND L.emp_code = (" + empid + ")";

                    qrySel = "select 'LIC' as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount " +
                     "from pr_emp_payslip_deductions d  JOIN Employees E ON E.empid = d.emp_code join pr_emp_lic_details l " +
                     "on d.emp_code = l.emp_code and l.amount=d.dd_amount join pr_emp_payslip p on d.payslip_mid = p.id" +
                     " where p.fm = '" + str1 + "' and l.fm='"+str1+"' and d.emp_code in (" + empid + ") and d.dd_name = 'LIC' and d.dd_amount > 0";
                    DataTable dtHFC = await _sha.Get_Table_FromQry(qrySel);

                    try
                    {
                        foreach (DataRow dr in dtHFC.Rows)
                        {
                            Ttl = Total + Convert.ToInt32(dr["amount"]);
                            HFCGridval.Add(new LICReport
                            {
                                Type = dr["Type"].ToString(),
                                Code = dr["emp_code"].ToString(),
                                Name = dr["ShortName"].ToString(),
                                Account_No = dr["account_no"].ToString(),
                                Amount = ReportColConvertToDecimal(dr["amount"].ToString()),
                                Total = ReportColConvertToDecimal(Ttl.ToString())

                                //Total = Total + Convert.ToInt32(dr["amount"])

                            });
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }

            }
            // employee id equal to null and either LIC or HFC is selected 
            else if (empid == "All" && (Type == "2" || Type == "3"))
            {

                if (Type == "2")
                {
                    //qrySel = "SELECT 'HFC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_hfc_details L JOIN Employees E ON E.empid=L.emp_code " +
                    //    "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) ";

                    qrySel = "select 'HFC' as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount " +
                        "from pr_emp_payslip_deductions d  JOIN Employees E ON E.empid = d.emp_code join pr_emp_hfc_details l " +
                        "on d.emp_code = l.emp_code join pr_emp_payslip p on d.payslip_mid = p.id" +
                        " where p.fm = '" + str1 + "' and l.fm='" + str1 + "' and d.dd_name = 'HFC' and d.dd_amount > 0";
                    DataTable dtHFC = await _sha.Get_Table_FromQry(qrySel);

                    try
                    {
                        foreach (DataRow dr in dtHFC.Rows)
                        {
                            Ttl = Total + Convert.ToInt32(dr["amount"]);

                            HFCGridval.Add(new LICReport
                            {
                                Type = dr["Type"].ToString(),
                                Code = dr["emp_code"].ToString(),
                                Name = dr["ShortName"].ToString(),
                                Account_No = dr["account_no"].ToString(),
                                Amount = ReportColConvertToDecimal( dr["amount"].ToString()),
                                Total = ReportColConvertToDecimal(Ttl.ToString())

                                //Total = Total + Convert.ToInt32(dr["amount"])

                            });
                        }
                    }
                    catch (Exception ex)
                    {


                    }

                }

                // if (Type[0] == "3") LIC
                else
                {
                    //qrySel = "SELECT 'LIC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_lic_details L JOIN Employees E ON E.empid=L.emp_code " +
                    //    "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) ";
                    qrySel = "select 'LIC' as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount " +
                   "from pr_emp_payslip_deductions d  JOIN Employees E ON E.empid = d.emp_code join pr_emp_lic_details l " +
                   "on d.emp_code = l.emp_code and l.amount=d.dd_amount join pr_emp_payslip p on d.payslip_mid = p.id" +
                   " where p.fm = '" + str1 + "'  and l.fm='" + str1 + "'  and d.dd_name = 'LIC' and d.dd_amount > 0";

                    DataTable dtHFC = await _sha.Get_Table_FromQry(qrySel);

                    try
                    {
                        foreach (DataRow dr in dtHFC.Rows)
                        {
                            Ttl = Total + Convert.ToInt32(dr["amount"]);

                            HFCGridval.Add(new LICReport
                            {
                                Type = dr["Type"].ToString(),
                                Code = dr["emp_code"].ToString(),
                                Name = dr["ShortName"].ToString(),
                                Account_No = dr["account_no"].ToString(),
                                Amount = ReportColConvertToDecimal(dr["amount"].ToString()),
                                Total = ReportColConvertToDecimal(Ttl.ToString())

                                //Total = Total + Convert.ToInt32(dr["amount"])

                            });
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }


            }
            // employee id equal to null and both LIC & HFC is selected
            else
            {
                //    string qrySel11 = " SELECT 'HFC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_hfc_details L JOIN Employees E ON E.empid=L.emp_code " +
                //            "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm)  " +
                //        "UNION ALL" +
                //        " SELECT 'LIC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_lic_details L JOIN Employees E ON E.empid=L.emp_code " +
                //            "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm)   ";
                string qrySel11 = "";
                if (empid != "All")
                {
                     qrySel11 = "select 'LIC' as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount  from pr_emp_payslip_deductions d" +
                        " JOIN Employees E ON E.empid = d.emp_code join pr_emp_lic_details l on d.emp_code = l.emp_code join pr_emp_payslip p" +
                        " on d.payslip_mid = p.id where p.fm = '" + str1 + "' and l.fm='" + str1 + "' and d.emp_code in (" + empid + ") and d.dd_name = 'LIC' and d.dd_amount > 0 union all select 'HFC' " +
                        " as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount from pr_emp_payslip_deductions d JOIN " +
                        " Employees E ON E.empid = d.emp_code join pr_emp_hfc_details l on d.emp_code = l.emp_code join pr_emp_payslip p " +
                        "on d.payslip_mid = p.id where p.fm = '" + str1 + "' and l.fm='" + str1 + "'  and d.emp_code in (" + empid + ") and d.dd_name = 'HFC' and d.dd_amount > 0; ";
                }
                else
                {
                    qrySel11 = "select 'LIC' as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount  from pr_emp_payslip_deductions d" +
              " JOIN Employees E ON E.empid = d.emp_code join pr_emp_lic_details l on d.emp_code = l.emp_code join pr_emp_payslip p" +
              " on d.payslip_mid = p.id  and l.amount=d.dd_amount  where p.fm = '" + str1 + "' and l.fm='" + str1 + "' and d.dd_name = 'LIC' and d.dd_amount > 0 union all select 'HFC' " +
              " as Type ,d.emp_code,e.ShortName,l.account_no,d.dd_amount as amount from pr_emp_payslip_deductions d JOIN " +
              " Employees E ON E.empid = d.emp_code join pr_emp_hfc_details l on d.emp_code = l.emp_code join pr_emp_payslip p " +
              "on d.payslip_mid = p.id where p.fm = '" + str1 + "' and l.fm='" + str1 + "' and d.dd_name = 'HFC' and d.dd_amount > 0; ";
                }
                DataTable dtHFC = await _sha.Get_Table_FromQry(qrySel11);

                try
                {

                    foreach (DataRow dr in dtHFC.Rows)
                    {
                        Ttl = Total + Convert.ToInt32(dr["amount"]);

                        HFCGridval.Add(new LICReport
                        {
                            Type = dr["Type"].ToString(),
                            Code = dr["emp_code"].ToString(),
                            Name = dr["ShortName"].ToString(),
                            Account_No = dr["account_no"].ToString(),
                            Amount = ReportColConvertToDecimal(dr["amount"].ToString()),
                            Total = ReportColConvertToDecimal(Ttl.ToString())

                            // Total = Total + Convert.ToInt32(dr["amount"])


                        });
                    }
                }
                catch (Exception ex)
                {

                }

            }

            return HFCGridval;

        }
        public async Task<IList<CommonReportModel>> GetLICReport(string empicode, string LICType, string Month)
        {
            if (empicode.Contains("^"))
            {
                empicode = "0";
                LICType = "0";
                Month = "01-01-01";
            }
            DateTime str = Convert.ToDateTime(Month);
            string str1 = str.ToString("yyyy-MM-dd");
            string Type = LICType;
            string empid = empicode;
            // employee id not equal to null and both LIC and HFC is selected 
            if (empicode != "All" && (Type == "null" || Type == "2,3" || Type == "0"))
            {
                qrySel = " SELECT 'HFC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_hfc_details L JOIN Employees E ON E.empid=L.emp_code " +
                   " WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) AND emp_code in (" + empid + ") AND L.amount!=0 AND L.active=1 group by emp_code,ShortName,account_no,amount" +
               " UNION ALL" +
               " SELECT 'LIC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_lic_details L JOIN Employees E ON E.empid=L.emp_code " +
                   " WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) AND emp_code in (" + empid + ") AND L.amount!=0 AND L.active=1 group by emp_code,ShortName,account_no,amount order by emp_code";

                qrysel1 = " Select 'HFC' as Type, Count(emp_code), sum(amount) as Total,emp_code from pr_emp_hfc_details where Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) and emp_code in (" + empid + ") AND amount!=0 AND active=1 group by emp_code" +
                        " union all" +
                        " select 'LIC' as Type, Count(emp_code), sum(amount) as Total,emp_code from pr_emp_lic_details where Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) and emp_code in (" + empid + ") AND amount!=0 AND active=1 group by emp_code order by emp_code";
            //    querysel2 = "";

                DataSet ds = await _sha.Get_MultiTables_FromQry(qrySel + qrysel1);
                DataTable dtALL = ds.Tables[0];
                DataTable dtTot = ds.Tables[1];


                foreach (DataRow dr in dtALL.Rows)
                {
                    type = dr["Type"].ToString();
                    code = dr["emp_code"].ToString();

                   
                      
                        clst = new CommonReportModel
                        {

                            RowId = RowCnt++,
                            SlNo = SlNo++.ToString(),
                            HRF = "R",
                            column1 = dr["Type"].ToString(),
                            column2 = dr["emp_code"].ToString(),
                            column3 = dr["ShortName"].ToString(),
                            column4 = dr["account_no"].ToString(),
                            column5 = ReportColConvertToDecimal(dr["amount"].ToString()),

                        };


                   
                    foreach (DataRow dr1 in dtTot.Rows)
                        {
                         
                            if (dr["emp_code"].ToString() == dr1["emp_code"].ToString())
                            {
                                string tot = getTotalLIC(type, code, dtTot);
                                clst.column6 = ReportColConvertToDecimal(tot.ToString());
                                // tot.RowId = RowCnt++;
                                lst.Add(clst);
                            }

                     
                    }
                }
                   

            }

            // employee id not equal to null and either LIC or HFC is selected 
            else if (empicode != "All" && (Type == "2" || Type == "3"))
            {
                if (Type == "2")
                {
                    qrySel = " SELECT 'HFC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_hfc_details L JOIN Employees E ON E.empid=L.emp_code " +
                        "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm)  AND L.emp_code in (" + empid + ") AND L.amount!=0 AND L.active=1 group by emp_code,ShortName,account_no,amount";

                    qrysel1 = " select 'HFC' as Type, Count(emp_code), sum(amount) as Total,emp_code from pr_emp_hfc_details where Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) and emp_code in (" + empid + ") AND amount!=0 AND active=1 group by emp_code";


                }
                //HFC with empid if (Type[0] == "3")
                else
                {

                    qrySel = "SELECT 'LIC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_lic_details L JOIN Employees E ON E.empid=L.emp_code " +
                        "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm)  AND L.emp_code = (" + empid + ") AND L.amount!=0 AND L.active=1 group by emp_code,ShortName,account_no,amount";

                    qrysel1 = " select 'LIC' as Type, Count(emp_code), sum(amount) as Total,emp_code from pr_emp_lic_details where Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) and emp_code in (" + empid + ") AND amount!=0 AND active=1 group by emp_code ";


                }

                DataSet ds = await _sha.Get_MultiTables_FromQry(qrySel + qrysel1);
                DataTable dtALL = ds.Tables[0];
                DataTable dtTot = ds.Tables[1];

                foreach (DataRow dr in dtALL.Rows)
                {
                    type = dr["Type"].ToString();
                    code = dr["emp_code"].ToString();



                    clst = new CommonReportModel
                    {

                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),
                        HRF = "R",
                        column1 = dr["Type"].ToString(),
                        column2 = dr["emp_code"].ToString(),
                        column3 = dr["ShortName"].ToString(),
                        column4 = dr["account_no"].ToString(),
                        column5 = ReportColConvertToDecimal(dr["amount"].ToString()),

                    };



                    foreach (DataRow dr1 in dtTot.Rows)
                    {

                        if (dr["emp_code"].ToString() == dr1["emp_code"].ToString())
                        {
                            string tot = getTotalLIC(type, code, dtTot);
                            clst.column6 = tot.ToString();
                            // tot.RowId = RowCnt++;
                            lst.Add(clst);
                        }


                    }
                }


            }


            // employee id equal to null and either LIC or HFC is selected 
            else if (empicode == "All" && (Type == "2" || Type == "3"))
            {
                //if (Type[0] == "2") HFC
                if (Type == "2")
                {
                    qrySel = "SELECT 'HFC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_hfc_details L JOIN Employees E ON E.empid=L.emp_code " +
                        "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) AND L.amount!=0 AND L.active=1 group by emp_code,ShortName,account_no,amount";

                    qrysel1 = " select 'HFC' as Type, Count(emp_code), sum(amount) as Total,emp_code from pr_emp_hfc_details WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) AND amount!=0 AND active=1 group by emp_code";
                }
                //if (Type[0] == "3") LIC
                else
                {
                    qrySel = "SELECT 'LIC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_lic_details L JOIN Employees E ON E.empid=L.emp_code " +
                        "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) AND L.amount!=0 AND L.active=1 group by emp_code,ShortName,account_no,amount";

                    qrysel1 = " select 'LIC' as Type, Count(emp_code), sum(amount) as Total,emp_code from pr_emp_lic_details WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) AND amount!=0 AND active=1 group by emp_code ";

                }
                DataSet ds = await _sha.Get_MultiTables_FromQry(qrySel + qrysel1);
                DataTable dtALL = ds.Tables[0];
                DataTable dtTot = ds.Tables[1];

                foreach (DataRow dr in dtALL.Rows)
                {
                    type = dr["Type"].ToString();
                    code = dr["emp_code"].ToString();



                    clst = new CommonReportModel
                    {

                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),
                        HRF = "R",
                        column1 = dr["Type"].ToString(),
                        column2 = dr["emp_code"].ToString(),
                        column3 = dr["ShortName"].ToString(),
                        column4 = dr["account_no"].ToString(),
                        column5 = ReportColConvertToDecimal(dr["amount"].ToString()),

                    };



                    foreach (DataRow dr1 in dtTot.Rows)
                    {

                        if (dr["emp_code"].ToString() == dr1["emp_code"].ToString())
                        {
                            string tot = getTotalLIC(type, code, dtTot);
                            clst.column6 = tot.ToString();
                            // tot.RowId = RowCnt++;
                            lst.Add(clst);
                        }


                    }
                }


            }


        
            // employee id equal to null and both LIC & HFC is selected
            else
            {
                qrySel = " SELECT 'HFC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_hfc_details L JOIN Employees E ON E.empid=L.emp_code " +
                        "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) AND L.amount!=0 AND L.active=1 group by emp_code,ShortName,account_no,amount" +
                        " UNION ALL" +
                        " SELECT 'LIC' as Type,emp_code,ShortName,account_no,amount FROM pr_emp_lic_details L JOIN Employees E ON E.empid=L.emp_code " +
                        "WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) AND L.amount!=0 AND L.active=1 group by emp_code,ShortName,account_no,amount order by emp_code;";
                qrysel1 = " select 'HFC' as Type, Count(emp_code), sum(amount) as Total,emp_code from pr_emp_hfc_details WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) AND amount!=0 AND active=1 group by emp_code" +
                        " UNION ALL" +
                        " select 'LIC' as Type, Count(emp_code), sum(amount) as Total,emp_code from pr_emp_lic_details WHERE Month('" + str1 + "') = Month(fm) AND YEAR('" + str1 + "') between year(fy) and year(fm) AND amount!=0 AND active=1 group by emp_code order by emp_code";


                DataSet ds = await _sha.Get_MultiTables_FromQry(qrySel + qrysel1);
                DataTable dtALL = ds.Tables[0];
                DataTable dtTot = ds.Tables[1];
                foreach (DataRow dr in dtALL.Rows)
                {
                    type = dr["Type"].ToString();
                    code = dr["emp_code"].ToString();



                    clst = new CommonReportModel
                    {

                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),
                        HRF = "R",
                        column1 = dr["Type"].ToString(),
                        column2 = dr["emp_code"].ToString(),
                        column3 = dr["ShortName"].ToString(),
                        column4 = dr["account_no"].ToString(),
                        column5 = ReportColConvertToDecimal(dr["amount"].ToString()),

                    };



                    foreach (DataRow dr1 in dtTot.Rows)
                    {

                        if (dr["emp_code"].ToString() == dr1["emp_code"].ToString())
                        {
                            string tot = getTotalLIC(type, code, dtTot);
                            clst.column6 = tot.ToString();
                            // tot.RowId = RowCnt++;
                            lst.Add(clst);
                        }


                    }
                }


            }

            List<CommonReportModel> distinct = lst.Distinct().ToList();
            return distinct;

        }
        private string getTotalLIC(string type, string code, DataTable dt)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["emp_code"].ToString() == code && x["Type"].ToString() == type)
                .Select(x => new { tot = x["Total"].ToString(), cnt = x["Total"].ToString() }).FirstOrDefault();

            //var tot = new CommonReportModel
            //{
            //    RowId = 0,
            //    HRF = "F",
            //    //SlNo = "<span style='color:#eef8fd'>^</span><b>Total :</b> "  + val.tot
            //    column1 = "<span style='color:#eef8fd'>^</span> <b>"
            //    + ReportColFooter(0, "Total Amount", val.tot) + "</b>"
            //};

            return val.tot;
        }
        private CommonReportModel getTotal(string type, string code, DataTable dt)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["emp_code"].ToString() == code && x["Type"].ToString() == type)
                .Select(x => new { tot = x["Total"].ToString(), cnt = x["Total"].ToString() }).FirstOrDefault();

            var tot = new CommonReportModel
            {
                RowId = 0,
                HRF = "F",
                //SlNo = "<span style='color:#eef8fd'>^</span><b>Total :</b> "  + val.tot
                column1 = "<span style='color:#eef8fd'>^</span> <b>"
                +ReportColFooter(0, "Total Amount", val.tot)+ "</b>"
            };

            return tot;
        }

        public async Task<IList<LICReport>> getTypes()
        {

            IList<LICReport> typeval = new List<LICReport>();
            try
            {
                typeval.Add(new LICReport
                {
                    Id = "0",
                    Type = "All",


                });
                typeval.Add(new LICReport
                {
                    Id = "2",
                    Type = "HFC Details",


                });
                typeval.Add(new LICReport
                {
                    Id = "3",
                    Type = "LIC Details",


                });
            }   
            catch (Exception ex)
            {

            }

            return typeval;

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

