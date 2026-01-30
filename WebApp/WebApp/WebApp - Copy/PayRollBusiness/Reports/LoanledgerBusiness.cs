using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Reports
{
    public class LoanledgerBusiness : BusinessBase
    {
        
        public LoanledgerBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        string oldempid = "";
        int RowCnt = 0;
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        IList<AllowanceTypes> lstDept = new List<AllowanceTypes>();
        public async Task<IList<loansTypes>> getloantypes()
        {
            string qrySel = "select id,loan_id,loan_description,interest_rate,Active from pr_loan_master";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<loansTypes> lstDept = new List<loansTypes>();
            try
            {
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
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        public async Task<IList<CommonReportModel>> GetLedgerReports(string empCode, string loanIds, string fy, string priority )

        {

            int Eyear = 0001;
            int Fyear = 0001;

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
            if (fy != null)
            {
                Eyear = int.Parse(fy);
                Fyear = int.Parse(fy)-1;
            }
            if (empCode.Contains("^"))
            {
                empCode = "0";
                loanIds = "0";
                fy = "1900";
            }
            string empid = empCode;
            //string qry = "SELECT distinct ( e.EmpId+ ', '+ e.ShortName + ', '+ 'Sanctioned : '+ ''+ REPLACE(RIGHT(CONVERT(VARCHAR(11), advlons.sanction_date,105),10), ' ', '-') + ', ' + 'Amount : '+ ''+ CAST(advlons.total_amount as varchar(15)) + ', '+ 'ROI : '+ ''+  CAST(ledg.interest_rate as varchar(15)) +'%'   ) as grpcol , lm.loan_description, REPLACE(RIGHT(CONVERT(VARCHAR(11), ledg.fm, 106), 8), ' ', '-') as fm ,ledg.interest_rate," +
            //    "ledg.loan_opening,ledg.total_paid,ledg.loan_closing,ledg.interest_opening, ledg.fm ,ledg.interest_repaid,ledg.interest_accrued,ledg.interest_closing, ledg.installment_amount " +
            //    "FROM pr_emp_loans_projection ledg join employees e on e.EmpId = ledg.emp_code JOIN pr_emp_adv_loans advlons on advlons.emp_code = e.EmpId " +
            //    "JOIN pr_loan_master lm on lm.id = ledg.loan_type_mid " +
            //    "WHERE ledg.fy = " + fy + " AND ledg.active=1 AND advlons.active=1 AND lm.active=1";
            //string qry = "SELECT distinct e.EmpId,ledg.fm as orderby_fm,ledg.emp_loan_id as LoanId,advlons.total_amount,ledg.interest_rate as interest_rate ,ledg.loan_opening AS lopening,ledg.total_paid,ledg.loan_closing,ledg.interest_opening, REPLACE(RIGHT(CONVERT(VARCHAR(11), ledg.fm, 106), 8), ' ', '-') as fm ,ledg.interest_repaid,ledg.interest_accrued,ledg.interest_closing, ledg.installment_amount ,lm.loan_description " +
            //    "FROM pr_emp_loans_projection ledg join employees e on e.EmpId = ledg.emp_code JOIN pr_emp_adv_loans advlons on advlons.emp_code = e.EmpId " +
            //    "JOIN pr_loan_master lm on lm.id = ledg.loan_type_mid " +
            //    "WHERE  advlons.active = 1 AND lm.active = 1 and ledg.active=1";
            string qry = "SELECT distinct e.EmpId,e.ShortName,case when ledg.interest_rate= (select max(interest_rate) from pr_emp_loans_projection) then 1 else 2 end as priority," +
                " d.code,advlons.loan_sl_no,advlons.sanction_date,lm.loan_id,advlons.installment_start_date,ledg.fm as orderby_fm,ledg.emp_loan_id as LoanId,advlons.total_amount,ledg.interest_rate as interest_rate ,ledg.loan_opening AS lopening,ledg.total_paid,ledg.loan_closing,ledg.interest_opening, REPLACE(RIGHT(CONVERT(VARCHAR(11), ledg.fm, 6), 6), ' ', '') as fm ,ledg.interest_repaid,ledg.interest_accrued,ledg.interest_closing, ledg.installment_amount ,lm.loan_description " +
             "FROM pr_emp_loans_projection ledg join employees e on e.EmpId = ledg.emp_code JOIN pr_emp_adv_loans advlons on advlons.emp_code = e.EmpId " +
             "JOIN pr_loan_master lm on lm.id = ledg.loan_type_mid  " +
             " JOIN pr_emp_adv_loans_child child on child.id = ledg.emp_loan_child_id " +
             "join Designations d on e.CurrentDesignation=d.id  " +
             "WHERE ledg.fm between DATEFROMPARTS (" + Fyear + ", 04, 01 ) and DATEFROMPARTS (" + Eyear + ", 03, 31 ) AND advlons.active = 1 AND lm.active = 1 and ledg.active=1";

            if (loanIds != "null" && priority == "All" )
            {
                qry += " AND lm.id in (" + loanIds + ") and advlons.loan_type_mid in (" + loanIds + ") ";
            }
            if (loanIds != "null" && priority == "1")
            {
                qry += " AND lm.id in (" + loanIds + ") and advlons.loan_type_mid in (" + loanIds + ")  and ledg.interest_rate = (select max(interest_rate) from pr_emp_loans_projection) ";
            }

            if (loanIds != "null" && priority == "2")
            {
                qry += " AND lm.id in (" + loanIds + ") and advlons.loan_type_mid in (" + loanIds + ")  and  ledg.interest_rate != (select max(interest_rate) from pr_emp_loans_projection) ";
            }

            if (empCode != "All" && priority == "All")
            {
                qry += " AND ledg.emp_code in ( " + empCode + ")  order by e.EmpId,ledg.fm,ledg.emp_loan_id,ledg.loan_opening desc ;"; //ORDER BY ledg.fm
            }
            else if (empCode != "All" && priority == "1")
            {
                qry += " AND ledg.emp_code in ( " + empCode + ") and ledg.interest_rate = (select max(interest_rate) from pr_emp_loans_projection) order by e.EmpId,ledg.fm,ledg.emp_loan_id,ledg.loan_opening desc ;"; //ORDER BY ledg.fm
            }
            else if (empCode != "All" && priority == "1")
            {
                qry += " AND ledg.emp_code in ( " + empCode + ") and ledg.interest_rate != (select max(interest_rate) from pr_emp_loans_projection) order by e.EmpId,ledg.fm,ledg.emp_loan_id,ledg.loan_opening desc ;"; //ORDER BY ledg.fm
            }
            else
            {
                qry += " order by e.EmpId,ledg.fm,priority,ledg.emp_loan_id, ledg.loan_opening desc ;";
            }
            string str = qry;
            //return await _sha.Get_Table_FromQry(qry);

            DataTable dt = await _sha.Get_Table_FromQry(qry);
            try
            {
                foreach (DataRow dr in dt.Rows)
                {

                    empid = dr["LoanId"].ToString();
                   
                    //string dd = dr["EmpId"].ToString();
                    if (oldempid != empid)
                    {
                        var grpdata = dr["EmpId"].ToString();
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>~</span>"
                            + ReportColHeader(0, "Emp Code", dr["EmpId"].ToString())
                             //grpclmn = "<span style='color:#C8EAFB'>~</span>"
                             //+ ReportColHeader(10, "Interest Rate", dr["interest_rate"].ToString())
                             + ReportColHeader2(4, "Name", dr["shortname"].ToString()," ( ", dr["Code"].ToString())+")"
                              //+ ReportColHeader(10, "Desig", dr["Code"].ToString())
                              + ReportColHeader(10, "Loan Amt", ReportColConvertToDecimal(dr["total_amount"].ToString()))
                             + ReportColHeader(7, "Sanction", Convert.ToDateTime( dr["sanction_date"]).ToString("dd-MM-yyyy"))
                             + ReportColHeader(7, "Loan Start", Convert.ToDateTime(dr["installment_start_date"]).ToString("dd-MM-yyyy"))
                             + ReportColHeader(7, "Loan Id", dr["loan_sl_no"].ToString()),
                            column1 = "`",
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
                        };
                        lst.Add(crm);
                    }
                    oldempid = dr["LoanId"].ToString();
                    
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        grpclmn = dr["fm"].ToString(),
                        column1 = dr["fm"].ToString(),
                        column12 = "P"+dr["priority"].ToString(),
                        column2 = ReportColConvertToDecimal(dr["lopening"].ToString()),
                        column3 = ReportColConvertToDecimal(dr["installment_amount"].ToString()),
                        column4 = ReportColConvertToDecimal(dr["loan_closing"].ToString()),
                        column5 = ReportColConvertToDecimal(dr["interest_opening"].ToString()),
                        column6 = ReportColConvertToDecimal(dr["interest_accrued"].ToString()),
                        column7 = ReportColConvertToDecimal(dr["interest_repaid"].ToString()),
                        column8 = ReportColConvertToDecimal(dr["interest_closing"].ToString()),
                        column9 = ReportColConvertToDecimal(dr["total_paid"].ToString()),
                        column10 = dr["loan_id"].ToString(),
                        column11 = dr["interest_rate"].ToString() + "%",

                    };
                    lst.Add(crm);
                }
            }
            catch (Exception e)
            {

            }
            return lst;
           
        }

        

        public async Task<DataTable> GetLoanLedgerData()
        {
            string qryLoan = "select CONVERT(VARCHAR(11), pl.fm, 105) AS fm, pl.amount_issued as amount_issued,pl.sanction_date as sanction_date,pl.interest_rate as interest_rate,pl.loan_opening as loan_opening,pl.total_paid as total_paid, pl.loan_closing,pl.interest_opening as interest_opening,pl.interest_accrued as interest_accrued,pl.interest_repaid as interest_repaid,pl.interest_closing as interest_closing,pl.installment_repaid as installment_repaid from pr_emp_loans_projection pl join pr_emp_adv_loans pal on pal.active = 1";
            //DataTable dt = await _sha.Get_Table_FromQry(qryLoan);
            return await _sha.Get_Table_FromQry(qryLoan);
        }
        public async Task<string> getfinancialperiod()
        {
            string query1 = "SELECT id,code FROM Designations";
            DataTable dt1 = await _sha.Get_Table_FromQry(query1);
            var resultJson = JsonConvert.SerializeObject(new { designations = dt1 });
            return resultJson;
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

            for (int i = 1; i < 6 ; i++)
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
        public async Task<IList<LICReport>> getFyforform16b()
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

            for (int i = 1; i < 10; i++)
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
        public async Task<IList<MISReportPS>> getpsdropdownvalues()
        {
            string qryps = "select distinct spl_type as name from pr_emp_payslip;";
            DataTable dt_ps = await _sha.Get_Table_FromQry(qryps);
            int count = 0;

            IList<MISReportPS> psvalues = new List<MISReportPS>();
            if (dt_ps.Rows.Count > 0)
            {
                psvalues.Add(new MISReportPS
                {
                    id = count.ToString(),
                    value = "Select"
                });
                count++;
                psvalues.Add(new MISReportPS
                {
                    id = count.ToString(),
                    value = "All"
                });
                count++;
                foreach ( DataRow dr_ps in dt_ps.Rows)
                {
                    psvalues.Add(new MISReportPS
                    {
                        id = count.ToString(),
                        value = dr_ps["name"].ToString()
                    });
                    count++;
                }
            }
            return psvalues;
        }
        public async Task<IList<MISReportDeduct>> getdeductdropdownvalues()
        {
            string qrydeduct = "select distinct upper(name)as deductname from pr_deduction_field_master where name !='';";
            DataTable dt_deduct = await _sha.Get_Table_FromQry(qrydeduct);
            int count = 0;

            IList<MISReportDeduct> deductvalues = new List<MISReportDeduct>();
            if (dt_deduct.Rows.Count > 0)
            {
                deductvalues.Add(new MISReportDeduct
                {
                    id = count.ToString(),
                    value = "Select"
                });
                count++;
                foreach (DataRow dr_ps in dt_deduct.Rows)
                {
                    deductvalues.Add(new MISReportDeduct
                    {
                        id = count.ToString(),
                        value = dr_ps["deductname"].ToString()
                    });
                    count++;
                }
            }
            return deductvalues;
        }
        public async Task<IList<MISReportDeduct>> gettdsdropdownvalues()
        {
            string qrydeduct = "select distinct upper(name)as deductname from pr_deduction_field_master where name !='';";
            DataTable dt_deduct = await _sha.Get_Table_FromQry(qrydeduct);
            int count = 0;

            IList<MISReportDeduct> deductvalues = new List<MISReportDeduct>();
            if (dt_deduct.Rows.Count > 0)
            {
                deductvalues.Add(new MISReportDeduct
                {
                    id = count.ToString(),
                    value = "Select"
                });
                count++;
                foreach (DataRow dr_ps in dt_deduct.Rows)
                {
                    deductvalues.Add(new MISReportDeduct
                    {
                        id = count.ToString(),
                        value = dr_ps["deductname"].ToString()
                    });
                    count++;
                }
            }
            return deductvalues;
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
