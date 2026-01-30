using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayRollBusiness.Process;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Masters
{
    public class EmployeeLoanDetailsBusiness : BusinessBase
    {
        public EmployeeLoanDetailsBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        string oldBranch = "";
        string oldloan = "";
        int RowCnt = 0;
        string oldeempid = "";
        int SlNo = 1;
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        IList<AllowanceTypes> lstDept = new List<AllowanceTypes>();
        public async Task<IList<AllowanceTypes>> getbranchlist()
        {

            string qryBranchlist = "select id,Name from Branches";
            DataTable dt = await _sha.Get_Table_FromQry(qryBranchlist);

            try
            {
                lstDept.Insert(0, new AllowanceTypes
                {
                    Id = "0",
                    Name = "All"
                });
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new AllowanceTypes
                    {
                        Id = dr["id"].ToString(),
                        Name = dr["Name"].ToString(),
                    });


                }
                
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }

        public async Task<IList<CommonReportModel>> GetLoanDetails(string Year, string loantype, string empcode, string priority)
        {
            string Eyear = "0001";
            string Fyear = "0001";
            if (priority == "100")
            {
                priority = "All";
            }
            else if (priority == "101")
            {
                priority = "1";
            }
            else if (priority == "102")
            {
                priority = "2";
            }
            if (Year != "^1")
            {
                Eyear = Year.Substring(5);
                Fyear = Year.Substring(0, 4);
            }

            if (loantype.Contains("^"))
            {
                //Branch = "0";
                loantype = "0";
                empcode = "0";
                Eyear = "0001";
                Fyear = "0001";

            }
            // branches
            //StringBuilder ATypes = new StringBuilder();

            //string[] Types = Branch.Split(',');
            //if (Branch != "0")
            //{


            //    foreach (string word in Types)
            //    {
            //        ATypes.Append("'");
            //        ATypes.Append(word);
            //        ATypes.Append("', ");
            //    }

            //    Branch = ATypes.ToString(0, ATypes.Length - 2);
            //}
            // loan types
            StringBuilder ALTypes = new StringBuilder();
            string[] LTypes = loantype.Split(',');
            if (loantype != "0")
            {
                foreach (string loans in LTypes)
                {
                    ALTypes.Append("'");
                    ALTypes.Append(loans);
                    ALTypes.Append("', ");
                }

                loantype = ALTypes.ToString(0, ALTypes.Length - 2);
            }
            string qry = "select e.EmpId,e.ShortName ,des.Code as Desig ,loan_description, loans.sanction_date , child.priority," +
                "case when loans.installment_start_date is null then loans.sanction_date else loans.installment_start_date " +
                "end as installment_start_date,child.interest_rate,loans.completed_installment, loans.remaining_installment,loans.total_installment , " +
                "child.loan_amount  ,  adju.cash_paid_on, concat( convert(char(3), adju.fm, 0),'-',year(adju.fm)) as fm, " +
                "case when br.Name != 'OtherBranch' then br.Name else dep.name end as DeptBranch, adju.installments_amount," +
                "adju.principal_open_amount, adju.principal_balance_amount,adju.interest_accured,adju.principal_paid_amount,adju.interest_open_amount,adju.interest_paid_amount, adju.interest_balance_amount,adju.payment_type " +
                "from pr_emp_adv_loans loans join pr_loan_master mas on loans.loan_type_mid = mas.id   join pr_emp_adv_loans_child child on loans.id = child.emp_adv_loans_mid " +
                "join pr_emp_adv_loans_adjustments adju on adju.emp_adv_loans_child_mid = child.id join Employees e on e.empid = loans.emp_code join designations des on des.id = e.currentdesignation " +
                "join Branches br on e.Branch = br.Id join departments dep on dep.Id = e.Department  " +
                "where   adju.fm between DATEFROMPARTS (" + Fyear + ", 04, 01 ) and DATEFROMPARTS (" + Eyear + ", 03, 31 )  ";
            //return await _sha.Get_Table_FromQry(qry);

            //if(Branch != "0")
            //{
            //    qry += "AND br.Id in (" + Branch + ") ";
            //}

            if (empcode != "All" && priority == "All")
            {
                qry += " AND loans.emp_code in ( " + empcode + ")  "; //ORDER BY loan_description
            }
            else if (empcode != "All" && priority == "1")
            {
                qry += " AND loans.emp_code in ( " + empcode + ") and priority = '"+priority+"'  "; //ORDER BY loan_description
            }
            else if (empcode != "All" && priority == "2")
            {
                qry += " AND loans.emp_code in ( " + empcode + ") and priority = '" + priority + "'  "; //ORDER BY loan_description
            }
            if (loantype == "0" && priority == "All")
            {
                qry += " order by loans.emp_code,loan_description, adju.fm, child.priority ,cash_paid_on ,principal_paid_amount desc; "; //ORDER BY loan_description
            }
            else if (loantype == "0" && priority == "1")
            {
                qry += " and priority = '" + priority + "' order by loans.emp_code,loan_description, adju.fm, child.priority ,cash_paid_on ,principal_paid_amount desc; "; //ORDER BY loan_description
            }
            else if (loantype == "0" && priority == "2")
            {
                qry += " and priority = '" + priority + "' order by loans.emp_code,loan_description, adju.fm, child.priority ,cash_paid_on ,principal_paid_amount desc; "; //ORDER BY loan_description
            }
            if (loantype != "0" && priority == "All")
            {
                qry += " AND loan_type_mid in (" + loantype + ") order by loan_description, loans.emp_code,adju.fm,adju.id, child.priority,cash_paid_on asc;";
            }

            else if (loantype != "0" && priority == "1")
            {
                qry += " AND loan_type_mid in (" + loantype + ") and priority = '" + priority + "' order by loan_description, loans.emp_code,adju.fm,adju.id, child.priority,cash_paid_on asc;";
            }

            else if (loantype != "0" && priority == "2")
            {
                qry += " AND loan_type_mid in (" + loantype + ") and priority = '" + priority + "' order by loan_description, loans.emp_code,adju.fm,adju.id, child.priority,cash_paid_on asc;";
            }



            DataTable dt = await _sha.Get_Table_FromQry(qry);
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //var DeptBranch = dr["DeptBranch"].ToString();

                    var loan = dr["loan_description"].ToString();

                    string empid = dr["EmpId"].ToString();


                    if (oldloan != loan && oldeempid != empid) //&& oldBranch != DeptBranch
                    {


                        crm = (new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>~</span>"
                            + ReportColHeader(01, "Loan", dr["loan_description"].ToString()),
                            column2 = "`",
                            column3 = "`",
                            column4 = "`",
                            column5 = "`",
                            column6 = "`",
                            column7 = "`",
                            column8 = "`",
                            column9 = "`",
                            column10 = "`",
                            column11 = "`",
                            column12 = "`",
                            column13 = "`",
                            column14 = "`",
                            column15 = "`",
                        });

                        lst.Add(crm);

                        crm = (new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>~</span>"
                             + ReportColHeader(3, "Emp Code", dr["EmpId"].ToString())
                             + ReportColHeader2(5, "Name", dr["ShortName"].ToString(), "(", dr["Desig"].ToString()) + ")"
                             //+ ReportColHeader(7, "Desig", dr["Desig"].ToString())
                             + ReportColHeader(7, "Amt", ReportColConvertToDecimal(dr["loan_amount"].ToString()))
                            + ReportColHeader(6, "Sanction", Convert.ToDateTime(dr["sanction_date"]).ToString("dd-MM-yyyy"))
                              + ReportColHeader(4, "Loan Start ", Convert.ToDateTime(dr["installment_start_date"]).ToString("dd-MM-yyyy"))
                               + ReportColHeader1(4, "Instl Comp / Total ", dr["completed_installment"].ToString(), "/", dr["total_installment"].ToString()),


                        });
                        lst.Add(crm);
                    }

                    if (oldloan == loan && oldeempid != empid) //&& oldBranch != DeptBranch
                    {


                        crm = (new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>~</span>"
                            + ReportColHeader(01, "Loan", dr["loan_description"].ToString()),
                            column2 = "`",
                            column3 = "`",
                            column4 = "`",
                            column5 = "`",
                            column6 = "`",
                            column7 = "`",
                            column8 = "`",
                            column9 = "`",
                            column10 = "`",
                            column11 = "`",
                            column12 = "`",
                            column13 = "`",
                            column14 = "`",
                            column15 = "`",
                        });

                        lst.Add(crm);

                        crm = (new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>~</span>"
                             + ReportColHeader(3, "Emp Code", dr["EmpId"].ToString())
                             + ReportColHeader2(5, "Name", dr["ShortName"].ToString(), "(", dr["Desig"].ToString()) + ")"
                             //+ ReportColHeader(7, "Desig", dr["Desig"].ToString())
                             + ReportColHeader(7, "Amt", ReportColConvertToDecimal(dr["loan_amount"].ToString()))
                            + ReportColHeader(6, "Sanction", Convert.ToDateTime(dr["sanction_date"]).ToString("dd-MM-yyyy"))
                              + ReportColHeader(4, "Loan Start ", Convert.ToDateTime(dr["installment_start_date"]).ToString("dd-MM-yyyy"))
                               + ReportColHeader1(4, "Instl Comp / Total ", dr["completed_installment"].ToString(), "/", dr["total_installment"].ToString()),


                        });
                        lst.Add(crm);
                    }

                    if (oldloan != loan && oldeempid == empid) //&& oldBranch != DeptBranch
                    {
                        crm = (new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColHeader(01, "Loan", dr["loan_description"].ToString()),
                            column2 = "`",
                            column3 = "`",
                            column4 = "`",
                            column5 = "`",
                            column6 = "`",
                            column7 = "`",
                            column8 = "`",
                            column9 = "`",
                            column10 = "`",
                            column11 = "`",
                            column12 = "`",
                            column13 = "`",
                            column14 = "`",
                            column15 = "`",
                        });

                        lst.Add(crm);
                        crm = (new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColHeader(2, "Emp Code", dr["EmpId"].ToString())
                             + ReportColHeader2(5, "Name", dr["ShortName"].ToString(), "(", dr["Desig"].ToString()) + ")"
                             //+ ReportColHeader(7, "Desig", dr["Desig"].ToString())
                             + ReportColHeader(7, "Amt", ReportColConvertToDecimal(dr["loan_amount"].ToString()))
                            + ReportColHeader(6, "Sanction", Convert.ToDateTime(dr["sanction_date"]).ToString("dd-MM-yyyy"))
                              + ReportColHeader(4, "Loan Start ", Convert.ToDateTime(dr["installment_start_date"]).ToString("dd-MM-yyyy"))
                               + ReportColHeader1(4, "Instl Comp / Total ", dr["completed_installment"].ToString(), "/", dr["total_installment"].ToString()),


                        });
                        lst.Add(crm);
                    }

                    oldloan = dr["loan_description"].ToString();
                    //oldBranch = dr["DeptBranch"].ToString();
                    oldeempid = dr["EmpId"].ToString();


                    oldeempid = dr["EmpId"].ToString();
                    string inacured = ReportColConvertToDecimal(dr["interest_accured"].ToString());
                    string payment = dr["payment_type"].ToString();
                    if (inacured == "")
                    {
                        inacured = "0";
                    }
                    if (payment != "Part Payment")
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            grpclmn = dr["fm"].ToString(),
                            column15 = "P" + dr["priority"].ToString(),
                            //column1 = dr["EmpId"].ToString(),
                            column2 = dr["fm"].ToString(),
                            column3 = ReportColConvertToDecimal(dr["loan_amount"].ToString()),
                            column4 = ReportColConvertToDecimal(dr["installments_amount"].ToString()),
                            column5 = ReportColConvertToDecimal(dr["principal_open_amount"].ToString()),
                            column14 = ReportColConvertToDecimal(dr["principal_paid_amount"].ToString()),
                            column6 = ReportColConvertToDecimal(dr["principal_balance_amount"].ToString()),
                            column7 = inacured,
                            column8 = ReportColConvertToDecimal(dr["interest_open_amount"].ToString()),
                            column9 = ReportColConvertToDecimal(dr["interest_paid_amount"].ToString()),
                            column10 = ReportColConvertToDecimal(dr["interest_balance_amount"].ToString()),
                            column11 = dr["DeptBranch"].ToString(),
                            column12 = dr["payment_type"].ToString(),

                            column13 = dr["interest_rate"].ToString() + "%",

                        };
                        lst.Add(crm);
                    }
                    else if (payment == "Part Payment")
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            grpclmn = dr["fm"].ToString(),
                            column15 = "P" + dr["priority"].ToString(),
                            //column1 = dr["EmpId"].ToString(),
                            column2 = dr["fm"].ToString(),
                            column3 = ReportColConvertToDecimal(dr["loan_amount"].ToString()),
                            column4 = ReportColConvertToDecimal(dr["installments_amount"].ToString()),
                            column5 = ReportColConvertToDecimal(dr["principal_open_amount"].ToString()),
                            column14 = ReportColConvertToDecimal(dr["principal_paid_amount"].ToString()),
                            column6 = ReportColConvertToDecimal(dr["principal_balance_amount"].ToString()),
                            column7 = inacured,
                            column8 = ReportColConvertToDecimal(dr["interest_open_amount"].ToString()),
                            column9 = ReportColConvertToDecimal(dr["interest_paid_amount"].ToString()),
                            column10 = ReportColConvertToDecimal(dr["interest_balance_amount"].ToString()),
                            column11 = dr["DeptBranch"].ToString(),
                            column12 = "Part.Pay",

                            column13 = dr["interest_rate"].ToString() + "%",

                        };
                        lst.Add(crm);

                    }

                }
            }
            catch (Exception e)
            {

            }
            return lst;
        }


        public async Task<IList<loansTypes>> getloantypes()
        {
            string qrySel = "select id,loan_id,loan_description,interest_rate,Active from pr_loan_master";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<loansTypes> lstDept = new List<loansTypes>();
            try
            {
                lstDept.Insert(0, new loansTypes
                {
                    id = "0",
                    name = "All"
                });
                foreach (DataRow dr in dt.Rows)
                {

                    lstDept.Add(new loansTypes
                    {
                        id = dr["Id"].ToString(),
                        name = dr["loan_description"].ToString(),
                    });

                }
                lstDept.Add(new loansTypes
                {
                    id = "100",
                    name = "ALL",

                });
                lstDept.Add(new loansTypes
                {
                    id = "101",
                    name = "Priority1",

                });

                lstDept.Add(new loansTypes
                {
                    id = "102",
                    name = "Priority2",

                });

            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }


        public string ReportColHeader1(int spaceCount, string lable, string value, string value1, string value2)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_HEADER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable + ": <b>" + value + "  " + value1 + " " + value2 + "   </b></span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }
        public string ReportColHeader2(int spaceCount, string lable, string value, string value1, string value2)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_HEADER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable + ": <b>" + value + "  " + value1 + " " + value2 + " </b></span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
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
