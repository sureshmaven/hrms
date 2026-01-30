using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PayRollBusiness.Reports
{
    public class MultirateBusiness : BusinessBase
    {
        public MultirateBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public class loansOptionss
        {
            public string BrId { get; set; }
            public string Name { get; set; }
        }
        int RowCnt = 0;
        public async Task<IList<loansOptionss>> getloans()
        {
            string loan = "";
            int id = 0;

            IList<loansOptionss> typeval = new List<loansOptionss>();
            loansOptionss crm = new loansOptionss();
            string qr1 = "  select Id, loan_description as Name from pr_loan_master where id not in(1,2,3,14,15,16,17,18,19,20,21,23,26,27)";
            DataTable dt = await _sha.Get_Table_FromQry(qr1);




            try
            {
                crm = new loansOptionss
                {
                    BrId = "0",
                    Name = "All",
                };
                typeval.Insert(0, crm); 
                foreach (DataRow dr in dt.Rows)
                {

                    crm = new loansOptionss
                    {
                        BrId = dr["Id"].ToString(),
                        Name = dr["Name"].ToString(),


                    };

                    typeval.Add(crm);
                }
            }
            catch (Exception ex)
            {

            }

            return typeval;
        }
        List<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        public async Task<IList<CommonReportModel>> Multirate(string loan, string inputMonth)
        {

            if (loan.Contains("^1"))
            {
                inputMonth = "01-01-01";
            }
            string qry1 = "";
            string qry2 = "";
            string qry21 = "";
            string qry3 = "";
            string code = "";
            string loan_description = "";
            string oldcode = "";
            string oldloan_description = "";
            string con1 = "";
            int rowno = 0;
            int SlNo = 1;
            string amount = "";
            string code_year = "";
            string loan_description_year = "";
            string oldcode_bal = "";
            string oldloan_description_bal = "";
            string empcode = "";
            string loandec = "";

            DateTime str = Convert.ToDateTime(inputMonth);
            string str1 = str.ToString("yyyy-MM-dd");
            string yerstr = str.ToString("yyyy");
            string mnthstr = str.ToString("MM");

            if (loan == "^1" || loan == "ALL")
            {
                con1 = " and lm.id not in(1,2,3,14,15,16,17,18,19,20,21,23,26,27) ";

            }
            else if (loan != "^1" || loan != "ALL")
            {
                con1 = " and lm.id in (" + loan + ")";


            }


            qry1 = "select distinct lm.loan_description,l.emp_code,ShortName,D.Name as Designation,loan_amount,principal_amount_recovered from pr_emp_adv_loans_child ch join pr_emp_adv_loans l on l.id=ch.emp_adv_loans_mid join Employees E on l.emp_code= E.EmpId join Designations D on D.Id= E.CurrentDesignation join pr_loan_master lm on lm.id= l.loan_type_mid  join pr_emp_adv_loans_adjustments adj on ch.id = adj.emp_adv_loans_child_mid " +
                "where  month(adj.fm)=month('" + str1 + "') and year(adj.fm)=YEAR('" + str1 + "') and ch.active=1 " + con1 + "order by loan_description,l.emp_code ";


            qry2 = " select lm.loan_description,l.emp_code,ShortName, D.Name,sum(interest_accured) as interest from pr_emp_adv_loans_adjustments ad join pr_emp_adv_loans l on l.id= ad.emp_adv_loans_mid join Employees E on l.emp_code= E.EmpId join Designations D on D.Id= E.CurrentDesignation join pr_loan_master lm on lm.id= l.loan_type_mid where  ad.fm  < DATEFROMPARTS ('" + yerstr + "', 04, 01 )  and ad.active=1 " + con1 + " group by emp_code,ShortName,D.Name,lm.loan_description";

            string qry4 = " select lm.loan_description,l.emp_code,ShortName, D.Name, sum(interest_accured) as interest from pr_emp_adv_loans_adjustments ad join pr_emp_adv_loans l on l.id = ad.emp_adv_loans_mid join Employees E on l.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation join pr_loan_master lm on lm.id = l.loan_type_mid where ad.fm  between DATEFROMPARTS('" + yerstr + "', 04, 01 ) and DATEFROMPARTS('" + yerstr + "', '" + mnthstr + "', DAY(EOMONTH('" + str1 + "')) ) and ad.active = 1 " + con1 + " group by emp_code,ShortName,D.Name,lm.loan_description";

            qry3 = " select lm.loan_description,l.emp_code,ShortName, D.Name as Designation,(sum(loan_amount)-sum(principal_amount_recovered)) as Pending, sum(ch.interest_accured) as Total_interest, ((sum(loan_amount)-sum(principal_amount_recovered))+sum(ch.interest_accured)) as Balance from pr_emp_adv_loans_child  ch join pr_emp_adv_loans l on l.id=ch.emp_adv_loans_mid join Employees E on l.emp_code= E.EmpId join Designations D on D.Id= E.CurrentDesignation join pr_loan_master lm on lm.id= l.loan_type_mid join pr_emp_adv_loans_adjustments adj  on ch.id = adj.emp_adv_loans_child_mid " +
                "where month(adj.fm)=month('" + str1 + "') and year(adj.fm)=YEAR('" + str1 + "') and ch.active=1 " + con1 + " group by emp_code,ShortName,D.Name,lm.loan_description";


            DataSet ds = await _sha.Get_MultiTables_FromQry(qry1 + qry2 + qry3 + qry4);
            DataTable dtALL = ds.Tables[0];
            DataTable dtALL1 = ds.Tables[1];
            DataTable dtAll2 = ds.Tables[2];
            DataTable dtAll3 = ds.Tables[3];

            foreach (DataRow drs in dtALL.Rows)
            {
                code = drs["emp_code"].ToString();
                loan_description = drs["loan_description"].ToString();
                if (oldloan_description != loan_description)
                {
                    lst.Add(new CommonReportModel
                    {

                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style='color:#C8EAFB'>~</span>"
                                        + ReportColHeader(0, "Loan Type", drs["loan_description"].ToString()),
                        column7 = "<span style='color:#C8EAFB'>~</span>"
                                        + ReportColHeaderValueOnly(0, " "),
                        column8 = "<span style='color:#C8EAFB'>~</span>"
                                        + ReportColHeaderValueOnly(0, " "),
                    });


                }
                if (oldcode != code)
                {
                    rowno = RowCnt++;
                    lst.Add(new CommonReportModel
                    {
                        RowId = rowno,
                        HRF = "R",
                        column1 = SlNo++.ToString(),
                        column2 = drs["emp_code"].ToString(),
                        column3 = drs["ShortName"].ToString(),
                        column4 = drs["Designation"].ToString(),
                        column5 = ReportColConvertToDecimal(drs["loan_amount"].ToString()),
                        column6 = ReportColConvertToDecimal(drs["principal_amount_recovered"].ToString()),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = drs["loan_description"].ToString(),

                    });

                }
                else if (oldloan_description != loan_description)
                {
                    rowno = RowCnt++;
                    lst.Add(new CommonReportModel
                    {
                        RowId = rowno,
                        HRF = "R",
                        column1 = SlNo++.ToString(),
                        column2 = drs["emp_code"].ToString(),
                        column3 = drs["ShortName"].ToString(),
                        column4 = drs["Designation"].ToString(),
                        column5 = ReportColConvertToDecimal(drs["loan_amount"].ToString()),
                        column6 = ReportColConvertToDecimal(drs["principal_amount_recovered"].ToString()),
                        column7 = ReportColConvertToDecimal("0"),
                        column8 = ReportColConvertToDecimal("0"),
                        column9 = drs["loan_description"].ToString(),
                    });

                }
                else if(oldloan_description == loan_description)
                {
                    try
                    {
                        //var emp = lst.FirstOrDefault(c => c.column2 == oldcode);
                        var found = lst.FirstOrDefault(c => { return c.column7 == "0" && c.column2 == oldcode && c.column9 == loan_description; });
                        //var found = lst.Where(c => c.column7 == "0" && c.column2 == oldcode && c.column9 == loan_description);
                        string loan1 = ReportColConvertToDecimal(drs["loan_amount"].ToString());
                        string loan1reco = ReportColConvertToDecimal(drs["principal_amount_recovered"].ToString());
                        found.column7 = loan1;
                        found.column8 = loan1reco;

                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }

                }
                if (oldcode != code)
                {
                    if (dtALL1.Rows.Count > 0)
                    {
                        foreach (DataRow drs1 in dtALL1.Rows)
                        {

                            oldcode = drs1["emp_code"].ToString();
                            oldloan_description = drs1["loan_description"].ToString();
                            if (loan_description == oldloan_description && oldcode == code)
                            {
                                if (dtAll3.Rows.Count > 0)
                                {
                                    foreach (DataRow drs3 in dtAll3.Rows)
                                    {
                                        code_year = drs3["emp_code"].ToString();
                                        loan_description_year = drs3["loan_description"].ToString();
                                        if (loan_description_year == oldloan_description && oldcode == code_year)
                                        {
                                            if ((empcode == "" && loandec == "") || (empcode != code_year && loandec != loan_description_year))
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = RowCnt++,
                                                    HRF = "R",
                                                    column1 = "",
                                                    column2 = "",
                                                    column3 = "",
                                                    column4 = "",
                                                    column5 = ReportColConvertToDecimal(drs1["interest"].ToString()),
                                                    column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                                    column7 = ReportColConvertToDecimal("0"),
                                                    column8 = ReportColConvertToDecimal("0"),
                                                    column9 = drs["loan_description"].ToString(),
                                                });
                                            }
                                            else if ((empcode == "" && loandec == "") || (empcode == code_year && loandec != loan_description_year))
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = RowCnt++,
                                                    HRF = "R",
                                                    column1 = "",
                                                    column2 = "",
                                                    column3 = "",
                                                    column4 = "",
                                                    column5 = ReportColConvertToDecimal(drs1["interest"].ToString()),
                                                    column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                                    column7 = ReportColConvertToDecimal("0"),
                                                    column8 = ReportColConvertToDecimal("0"),
                                                    column9 = drs["loan_description"].ToString(),
                                                });
                                            }

                                            else if ((empcode == "" && loandec == "") || (empcode != code_year && loandec == loan_description_year))
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = RowCnt++,
                                                    HRF = "R",
                                                    column1 = "",
                                                    column2 = "",
                                                    column3 = "",
                                                    column4 = "",
                                                    column5 = ReportColConvertToDecimal(drs1["interest"].ToString()),
                                                    column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                                    column7 = ReportColConvertToDecimal("0"),
                                                    column8 = ReportColConvertToDecimal("0"),
                                                    column9 = drs["loan_description"].ToString(),
                                                });
                                            }

                                        }
                                        empcode = drs3["emp_code"].ToString();
                                        loandec = drs3["loan_description"].ToString();
                                    }
                                }
                                else
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowCnt++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",
                                        column3 = "",
                                        column4 = "",
                                        column5 = ReportColConvertToDecimal("0"),
                                        column6 = ReportColConvertToDecimal("0"),
                                        column7 = ReportColConvertToDecimal("0"),
                                        column8 = ReportColConvertToDecimal("0"),
                                        column9 = drs["loan_description"].ToString(),
                                    });
                                }
                            }

                        }
                    }
                    else
                    {
                        empcode = "";
                        loandec = "";
                        foreach (DataRow drs3 in dtAll3.Rows)
                        {
                            code_year = drs3["emp_code"].ToString();
                            loan_description_year = drs3["loan_description"].ToString();
                            if (loan_description_year == loan_description && code == code_year)
                            {
                                if ((empcode == "" && loandec == "") || (empcode != code_year && loandec != loan_description_year))
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowCnt++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",
                                        column3 = "",
                                        column4 = "",
                                        column5 = ReportColConvertToDecimal("0"),
                                        column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                        column7 = ReportColConvertToDecimal("0"),
                                        column8 = ReportColConvertToDecimal("0"),
                                        column9 = drs["loan_description"].ToString(),
                                    });
                                }
                                else if ((empcode == "" && loandec == "") || (empcode == code_year && loandec != loan_description_year))
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowCnt++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",
                                        column3 = "",
                                        column4 = "",
                                        column5 = ReportColConvertToDecimal("0"),
                                        column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                        column7 = ReportColConvertToDecimal("0"),
                                        column8 = ReportColConvertToDecimal("0"),
                                        column9 = drs["loan_description"].ToString(),
                                    });
                                }

                                else if ((empcode == "" && loandec == "") || (empcode != code_year && loandec == loan_description_year))
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowCnt++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",
                                        column3 = "",
                                        column4 = "",
                                        column5 = ReportColConvertToDecimal("0"),
                                        column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                        column7 = ReportColConvertToDecimal("0"),
                                        column8 = ReportColConvertToDecimal("0"),
                                        column9 = drs["loan_description"].ToString(),
                                    });
                                }
                            }
                            empcode = drs3["emp_code"].ToString();
                            loandec = drs3["loan_description"].ToString();
                        }
                    }

                    empcode = "";
                    loandec = "";

                    foreach (DataRow drs2 in dtAll2.Rows)
                    {
                        oldcode_bal = drs2["emp_code"].ToString();
                        oldloan_description_bal = drs2["loan_description"].ToString();
                        if (loan_description == oldloan_description_bal && oldcode_bal == code)
                        {
                            if (empcode == "" && loandec == "")
                            {
                                lst.Add(new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = "",
                                    column2 = "",
                                    column3 = "",
                                    column4 = "",
                                    column5 = ReportColConvertToDecimal(drs2["Pending"].ToString()),
                                    column6 = "",
                                    column7 = ReportColConvertToDecimal(drs2["Total_interest"].ToString()),
                                    column8 = ReportColConvertToDecimal(drs2["Balance"].ToString()),
                                    column9 = drs["loan_description"].ToString(),
                                });
                            }
                            empcode = drs2["emp_code"].ToString();
                            loandec = drs2["loan_description"].ToString();
                        }
                    }
                }
                else if (oldcode == code && oldloan_description != loan_description)
                {
                    if (dtALL1.Rows.Count > 0)
                    {
                        foreach (DataRow drs1 in dtALL1.Rows)
                        {

                            oldcode = drs1["emp_code"].ToString();
                            oldloan_description = drs1["loan_description"].ToString();
                            if (loan_description == oldloan_description && oldcode == code)
                            {
                                if (dtAll3.Rows.Count > 0)
                                {
                                    foreach (DataRow drs3 in dtAll3.Rows)
                                    {
                                        code_year = drs3["emp_code"].ToString();
                                        loan_description_year = drs3["loan_description"].ToString();
                                        if (loan_description_year == oldloan_description && oldcode == code_year)
                                        {
                                            if ((empcode == "" && loandec == "") || (empcode != code_year && loandec != loan_description_year))
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = RowCnt++,
                                                    HRF = "R",
                                                    column1 = "",
                                                    column2 = "",
                                                    column3 = "",
                                                    column4 = "",
                                                    column5 = ReportColConvertToDecimal(drs1["interest"].ToString()),
                                                    column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                                    column7 = ReportColConvertToDecimal("0"),
                                                    column8 = ReportColConvertToDecimal("0"),
                                                    column9 = drs["loan_description"].ToString(),
                                                });
                                            }
                                            else if ((empcode == "" && loandec == "") || (empcode == code_year && loandec != loan_description_year))
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = RowCnt++,
                                                    HRF = "R",
                                                    column1 = "",
                                                    column2 = "",
                                                    column3 = "",
                                                    column4 = "",
                                                    column5 = ReportColConvertToDecimal(drs1["interest"].ToString()),
                                                    column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                                    column7 = ReportColConvertToDecimal("0"),
                                                    column8 = ReportColConvertToDecimal("0"),
                                                    column9 = drs["loan_description"].ToString(),
                                                });
                                            }

                                            else if ((empcode == "" && loandec == "") || (empcode != code_year && loandec == loan_description_year))
                                            {
                                                lst.Add(new CommonReportModel
                                                {
                                                    RowId = RowCnt++,
                                                    HRF = "R",
                                                    column1 = "",
                                                    column2 = "",
                                                    column3 = "",
                                                    column4 = "",
                                                    column5 = ReportColConvertToDecimal(drs1["interest"].ToString()),
                                                    column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                                    column7 = ReportColConvertToDecimal("0"),
                                                    column8 = ReportColConvertToDecimal("0"),
                                                    column9 = drs["loan_description"].ToString(),
                                                });
                                            }

                                        }
                                        empcode = drs3["emp_code"].ToString();
                                        loandec = drs3["loan_description"].ToString();
                                    }
                                }
                                else
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowCnt++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",
                                        column3 = "",
                                        column4 = "",
                                        column5 = ReportColConvertToDecimal("0"),
                                        column6 = ReportColConvertToDecimal("0"),
                                        column7 = ReportColConvertToDecimal("0"),
                                        column8 = ReportColConvertToDecimal("0"),
                                        column9 = drs["loan_description"].ToString(),
                                    });
                                }
                            }

                        }
                    }
                    else
                    {
                        empcode = "";
                        loandec = "";
                        foreach (DataRow drs3 in dtAll3.Rows)
                        {
                            code_year = drs3["emp_code"].ToString();
                            loan_description_year = drs3["loan_description"].ToString();
                            if (loan_description_year == loan_description && code == code_year)
                            {
                                if ((empcode == "" && loandec == "") || (empcode != code_year && loandec != loan_description_year))
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowCnt++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",
                                        column3 = "",
                                        column4 = "",
                                        column5 = ReportColConvertToDecimal("0"),
                                        column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                        column7 = ReportColConvertToDecimal("0"),
                                        column8 = ReportColConvertToDecimal("0"),
                                        column9 = drs["loan_description"].ToString(),
                                    });
                                }
                                else if ((empcode == "" && loandec == "") || (empcode == code_year && loandec != loan_description_year))
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowCnt++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",
                                        column3 = "",
                                        column4 = "",
                                        column5 = ReportColConvertToDecimal("0"),
                                        column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                        column7 = ReportColConvertToDecimal("0"),
                                        column8 = ReportColConvertToDecimal("0"),
                                        column9 = drs["loan_description"].ToString(),
                                    });
                                }

                                else if ((empcode == "" && loandec == "") || (empcode != code_year && loandec == loan_description_year))
                                {
                                    lst.Add(new CommonReportModel
                                    {
                                        RowId = RowCnt++,
                                        HRF = "R",
                                        column1 = "",
                                        column2 = "",
                                        column3 = "",
                                        column4 = "",
                                        column5 = ReportColConvertToDecimal("0"),
                                        column6 = ReportColConvertToDecimal(drs3["interest"].ToString()),
                                        column7 = ReportColConvertToDecimal("0"),
                                        column8 = ReportColConvertToDecimal("0"),
                                        column9 = drs["loan_description"].ToString(),
                                    });
                                }
                            }
                            empcode = drs3["emp_code"].ToString();
                            loandec = drs3["loan_description"].ToString();
                        }
                    }

                    empcode = "";
                    loandec = "";

                    foreach (DataRow drs2 in dtAll2.Rows)
                    {
                        oldcode_bal = drs2["emp_code"].ToString();
                        oldloan_description_bal = drs2["loan_description"].ToString();
                        if (loan_description == oldloan_description_bal && oldcode_bal == code)
                        {
                            if (empcode == "" && loandec == "")
                            {
                                lst.Add(new CommonReportModel
                                {
                                  
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    column1 = "",
                                    column2 = "",
                                    column3 = "",
                                    column4 = "",
                                    column5 = ReportColConvertToDecimal(drs2["Pending"].ToString()),
                                    column6 = "",
                                    column7 = ReportColConvertToDecimal(drs2["Total_interest"].ToString()),
                                    column8 = ReportColConvertToDecimal(drs2["Balance"].ToString()),
                                    column9 = drs["loan_description"].ToString(),
                                });
                            }
                            empcode = drs2["emp_code"].ToString();
                            loandec = drs2["loan_description"].ToString();
                        }
                    }
                }

                //else if(oldcode != code && oldloan_description != loan)
                //{
                //    foreach (DataRow drs1 in dtALL1.Rows)
                //    {

                //        oldcode = drs1["emp_code"].ToString();
                //        oldloan_description = drs1["loan_description"].ToString();
                //        if (loan_description == oldloan_description && oldcode == code)
                //        {
                //            lst.Add(new CommonReportModel
                //            {
                //                RowId = RowCnt++,
                //                HRF = "R",
                //                column1 = "",
                //                column2 = "",
                //                column3 = "",
                //                column4 = "",
                //                column5 = "0",
                //                column6 = "0",
                //                column7 = "0",
                //                column8 = "0",
                //            });
                //        }

                //    }



                //    foreach (DataRow drs2 in dtAll2.Rows)
                //    {
                //        oldcode = drs2["emp_code"].ToString();
                //        oldloan_description = drs2["loan_description"].ToString();
                //        if (loan_description == oldloan_description && oldcode == code)
                //        {
                //            lst.Add(new CommonReportModel
                //            {
                //                RowId = RowCnt++,
                //                HRF = "R",
                //                column1 = "",
                //                column2 = "",
                //                column3 = "",
                //                column4 = "",
                //                column5 = "0",
                //                column6 = "",
                //                column7 = "0",
                //                column8 = "0",
                //            });
                //        }
                //    }
                //}
                oldcode = drs["emp_code"].ToString();
                oldloan_description = drs["loan_description"].ToString();
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

