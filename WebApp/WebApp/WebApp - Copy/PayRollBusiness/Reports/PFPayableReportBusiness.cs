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
using System.Configuration;
using PayRollBusiness.Process;
using System.Linq;

namespace PayRollBusiness.Masters
{
    public class PFPayableReportBusiness : BusinessBase
    {
        CommonReportModel crm = new CommonReportModel();
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        public PFPayableReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();

        public class loansOptionss
        {
            public string BrId { get; set; }
            public string Name { get; set; }
        }
        public async Task<IList<loansOptionss>> getloans()
        {
            string loan = "";
            int id = 0;
            IList<loansOptionss> typeval = new List<loansOptionss>();
            loansOptionss crm = new loansOptionss();
            string qr1 = "SELECT id,purpose_name as name,month as value " +
                            "FROM pr_purpose_of_advance_master " +
                            "WHERE active=1 AND ptype='REPAY'";
            DataTable dt = await _sha.Get_Table_FromQry(qr1);
            typeval.Insert(0, new loansOptionss
            {
                BrId = "0",
                Name = "ALL"
            });
            foreach (DataRow dr in dt.Rows)
            {
                loan = dr["Name"].ToString();
                id = Convert.ToInt32(dr["Id"].ToString());
                try
                {


                    crm = new loansOptionss
                    {
                        BrId = id.ToString(),
                        Name = loan,


                    };

                    typeval.Add(crm);
                }
                catch (Exception ex)
                {

                }
            }

            return typeval;
        }

        public async Task<IList<CommonReportModel>> PFRepayableData(string loan, string from, string To)
        {

            decimal noofloans = 0;
            decimal sanctionamt = 0;
            if (loan != "^1")
            {
                string[] loans = loan.Split(',');
                StringBuilder ATypes = new StringBuilder();
                if (loans.Length > 1)
                {

                    foreach (string word in loans)
                    {
                        ATypes.Append("'");
                        ATypes.Append(word);

                        ATypes.Append("', ");
                    }
                    loan = ATypes.ToString(0, ATypes.Length - 2);
                }
            }
            CommonReportModel crm = new CommonReportModel();
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int RowCnt = 0;


            string purpose = "";
            string oldpurpose = "";
            string oldbranch1 = "";
            int SlNo = 1;
            string cond = "";

            string ipmn = "01-01-01";
            // IList<PrrepayableDataModel> lstDept = new List<PrrepayableDataModel>();
            DateTime Fdate = DateTime.Now, Tdate = DateTime.Now;
            if (from == "^2")
            {
                from = "01-01-01";
            }

            if (To == "^3")
            {
                To = "01-01-01";
            }
            DateTime str = Convert.ToDateTime(from);
            string fromdate = str.ToString("yyyy-MM-dd");
            string[] from_sa = fromdate.Split('-');
            string s1 = from_sa[0];
            string s2 = from_sa[1];
            string s3 = from_sa[2];

            DateTime str1 = Convert.ToDateTime(To);
            string todate = str1.ToString("yyyy-MM-dd");
            string[] to_sa = todate.Split('-');
            string to_s1 = to_sa[0];
            string to_s2 = to_sa[1];
            string to_s3 = to_sa[2];


            if (loan == "^1")
            {
                loan = "0";
            }
            if (loan != "^1" && loan == "ALL")
            {
                cond = " where process=1   and sanction_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " )  group by purpose_of_advance,purpose_name  ";
            }
            if (loan != "^1" && loan != "ALL")
            {
                cond = " where process=1  and purpose_of_advance in (" + loan + ") and sanction_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " )  group by purpose_of_advance,purpose_name ";

            }

            //string qry = "select w.ShortName,w.EmpId,format(al.sanction_date,'dd-MM-yyyy') as sanction_date, case when b.Name!='OtherBranch' then b.Name else 'Head" +
            //    "Office' end as grpcol, count(*) as No_loans,sum(al.amount_recommended_for_sanction) as samt ,pam.purpose_name as purpose " +
            //    "from pr_emp_pf_repayable_loan al join pr_loan_master on al.pf_loans_id = pr_loan_master.loan_id join Employees " +
            //    "w on w.empid = al.emp_code join branches b on b.id = w.branch join departments dept on dept.id = w.department " +
            //    "join designations d on d.id = w.currentdesignation join pr_purpose_of_advance_master pam on pam.id = al.purpose_of_advance " +
            //    "inner join pr_loan_master lm on lm.loan_id = al.pf_loans_id  ";

            string qry = "  select b.purpose_name as purpose,count(purpose_of_advance) as No_loans,sum(a.amount_recommended_for_sanction) as samt" +
                    " from pr_emp_pf_repayable_loan a join pr_purpose_of_advance_master b on a.purpose_of_advance = b.id ";

            //if (loan != "" && loan == "0")
            //{
            //    qry += " where month(sanction_date)='" + s2 + "'  and year(sanction_date)='" + s1 + "' group by b.name,pam.purpose_name";

            //}

            if (loan != "")
            {
                qry += cond;

            }



            DataTable dt = new DataTable();
            DataSet ds = await _sha.Get_MultiTables_FromQry(qry);
            DataTable dtSalbr = ds.Tables[0];

            foreach (DataRow drs in dtSalbr.Rows)
            {
                purpose = drs["purpose"].ToString();

                if (oldpurpose != "" && oldpurpose != purpose)
                {
                    ////prev. br. footer
                    //CommonReportModel tot = getTotal(oldpurpose, dtSalbr, noofloans, sanctionamt);
                    //tot.RowId = RowCnt++;
                    //lst.Add(tot);

                    ////grp header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",
                    //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //            + ReportColFooterlesscol(70, "Loan Type", purpose),
                    //   // column2 = "`",
                    //    column3 = "`",
                    //    column4 = "`",
                    //});

                    //rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2 = "Type of Loan",
                    //    column3 = "No of Loans",
                    //    column4 = "Loan Amount",
                    //});
                }
                else if (oldpurpose == "")
                {
                    //grp header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",
                    //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //            + ReportColFooterlesscol(70, "Loan Type ", purpose),
                    //    //column2 = "`",
                    //    column3 = "`",
                    //    column4 = "`",
                    //});

                    ////rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2 = "Type of Loan",
                    //    column3 = "No of Loans",
                    //    column4 = "Loan Amount",
                    //});

                }
                oldpurpose = drs["purpose"].ToString();
                if (oldbranch1 != purpose)
                {
                    //sanctionamt = 0;
                    //noofloans = 0;
                    SlNo = SlNo++;
                    decimal Dsamt = Convert.ToDecimal(drs["samt"].ToString()) + 0.00M;
                    string Nsamt = String.Format("{0:n}", Dsamt);
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),

                        //column2 = drs["purpose"].ToString(),

                        column3 = drs["No_loans"].ToString(),
                        column4 = Nsamt,

                        //column5 = drs["ShortName"].ToString(),
                        //column6 = drs["EmpId"].ToString(),
                        //column7 = drs["sanction_date"].ToString(),
                        column8 = drs["purpose"].ToString()
                    });

                    noofloans = noofloans + Convert.ToDecimal(drs["No_loans"].ToString());
                    sanctionamt = sanctionamt + Convert.ToDecimal(drs["samt"].ToString());
                }
                else
                {

                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),
                        //column2 = drs["purpose"].ToString(),
                        column3 = drs["No_loans"].ToString(),
                        column4 = drs["samt"].ToString(),
                        //column5 = drs["ShortName"].ToString(),
                        //column6 = drs["EmpId"].ToString(),
                        //column7 = drs["sanction_date"].ToString(),
                        column8 = drs["purpose"].ToString()
                    });

                    noofloans = noofloans + Convert.ToDecimal(drs["No_loans"].ToString());
                    sanctionamt = sanctionamt + Convert.ToDecimal(drs["samt"].ToString());
                }
                oldbranch1 = drs["purpose"].ToString();
            }
            if (oldpurpose != "")
            {
                CommonReportModel tot = getTotal(oldpurpose, dtSalbr, noofloans, sanctionamt);
                tot.RowId = RowCnt++;
                lst.Add(tot);

            }





            return lst;

        }
        public async Task<IList<CommonReportModel>> PFRepayableSanData(string loan, string from, string To)
        {
            decimal totalsan = 0;

            List<decimal> grandsantot = new List<decimal>(); // added by chaitanya on 14/03/2020

            if (loan != "^1")
            {
                string[] loans = loan.Split(',');
                StringBuilder ATypes = new StringBuilder();
                if (loans.Length > 1)
                {
                    foreach (string word in loans)
                    {
                        ATypes.Append("'");
                        ATypes.Append(word);

                        ATypes.Append("', ");
                    }
                    loan = ATypes.ToString(0, ATypes.Length - 2);
                }
            }
            CommonReportModel crm = new CommonReportModel();
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int RowCnt = 0;


            string branch1 = "";
            string oldbranch = "";
            string oldbranch1 = "";
            int SlNo = 1;
            string cond = "";

            string ipmn = "01-01-01";
            // IList<PrrepayableDataModel> lstDept = new List<PrrepayableDataModel>();
            DateTime Fdate = DateTime.Now, Tdate = DateTime.Now;
            if (from == "^2")
            {
                from = "01-01-01";
            }

            if (To == "^3")
            {
                from = "01-01-01";
            }
            DateTime str = Convert.ToDateTime(from);
            string fromdate = str.ToString("yyyy-MM-dd");
            string[] from_sa = fromdate.Split('-');
            string s1 = from_sa[0];
            string s2 = from_sa[1];
            string s3 = from_sa[2];

            DateTime str1 = Convert.ToDateTime(To);
            string todate = str1.ToString("yyyy-MM-dd");
            string[] to_sa = todate.Split('-');
            string to_s1 = to_sa[0];
            string to_s2 = to_sa[1];
            string to_s3 = to_sa[2];




            if (loan == "^1")
            {
                loan = "0";
            }

            if (loan != "^1" && loan == "ALL" && from != "")
            {
                cond = " where process=1  and  al.sanction_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " ) group by b.name,pam.purpose_name,al.emp_code,w.ShortName,al.sanction_date order by  pam.purpose_name,al.sanction_date asc";

            }
            if (loan != "^1" && loan != "ALL" && from != "")
            {
                cond = " where process=1  and purpose_of_advance in (" + loan + ") and al.sanction_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " )  group by b.name,pam.purpose_name,al.emp_code,w.ShortName,al.sanction_date order by pam.purpose_name,  al.sanction_date asc";

            }


            //string qry = "select al.emp_code as EmpCode,w.ShortName as Name,al.Sanction_date as sanctiondate,case when b.Name!='OtherBranch' then b.Name else 'HeadOffice' end as grpcol, count(*) as No_loans,sum(al.amount_recommended_for_sanction) as samt ,pam.purpose_name as purpose from pr_emp_pf_repayable_loan al join pr_loan_master on al.pf_loans_id = pr_loan_master.loan_id join Employees w on w.empid = al.emp_code join branches b on b.id = w.branch join departments dept on dept.id = w.department join designations d on d.id = w.currentdesignation join pr_purpose_of_advance_master pam on pam.id = al.purpose_of_advance inner join pr_loan_master lm on lm.loan_id = al.pf_loans_id  ";
            string qry = "select al.emp_code as EmpCode,w.ShortName as Name,al.Sanction_date as sanctiondate, count(*) as No_loans,sum(al.amount_recommended_for_sanction) as samt ," +
                   "pam.purpose_name as purpose from pr_emp_pf_repayable_loan al join pr_loan_master on al.pf_loans_id = pr_loan_master.loan_id " +
                   "join Employees w on w.empid = al.emp_code join branches b on b.id = w.branch join departments dept on dept.id = w.department " +
                   "join designations d on d.id = w.currentdesignation join pr_purpose_of_advance_master pam on pam.id = al.purpose_of_advance inner " +
                   "join pr_loan_master lm on lm.loan_id = al.pf_loans_id  ";
            if (loan != "" && loan == "0")
            {
                qry += " where al.sanction_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " ) group by b.name,pam.purpose_name,al.emp_code,w.ShortName,al.sanction_date";

            }
            else if (loan != "")
            {
                qry += cond;

            }


            DataTable dt = new DataTable();
            DataSet ds = await _sha.Get_MultiTables_FromQry(qry);
            DataTable dtSalbr = ds.Tables[0];

            foreach (DataRow drs in dtSalbr.Rows)
            {
                branch1 = drs["purpose"].ToString();

                if (oldbranch != "" && oldbranch != branch1)
                {
                    //prev. br. footer
                    CommonReportModel tot = getTotalSan(oldbranch, dtSalbr, totalsan);
                    grandsantot.Add(totalsan); //added by chaitanya on 14/03/2020
                    tot.RowId = RowCnt++;
                    lst.Add(tot);

                    //grp header
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        SlNo = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColFooterlesscol(1, "Loan Type ", branch1),
                        column2 = "`",
                        column3 = "`",
                        column4 = "`",
                        column5 = "`",
                    });

                    //rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2 = "EmpCode",
                    //    column3 = "Name",
                    //    column4 = "Sanction Date",
                    //    column5 = "Sanction Amount",
                    //});

                }
                else if (oldbranch == "")
                {
                    //grp header
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        SlNo = "<span style='color:#C8EAFB'>~</span>"
                                + ReportColFooterlesscol(1, "Loan Type ", branch1),
                        column2 = "`",
                        column3 = "`",
                        column4 = "`",
                        column5 = "`",
                    });

                    //rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2 = "EmpCode",
                    //    column3 = "Name",
                    //    column4 = "Sanction Date",
                    //    column5 = "Sanction Amount",
                    //});

                }
                oldbranch = drs["purpose"].ToString();
                if (oldbranch1 != branch1)
                {
                    SlNo = 1;
                    totalsan = 0;
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),

                        column2 = drs["EmpCode"].ToString(),

                        column3 = drs["Name"].ToString(),
                        column4 = Convert.ToDateTime(drs["sanctiondate"]).ToString("dd/MM/yyyy"),

                        column5 = ReportColConvertToDecimal( drs["samt"].ToString()),
                    });
                    totalsan = totalsan + Convert.ToDecimal(drs["samt"]);
                }
                else
                {
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),

                        column2 = drs["EmpCode"].ToString(),

                        column3 = drs["Name"].ToString(),
                        column4 = Convert.ToDateTime(drs["sanctiondate"]).ToString("dd/MM/yyyy"),

                        column5 = ReportColConvertToDecimal( drs["samt"].ToString()),
                    });
                    totalsan = totalsan + Convert.ToDecimal(drs["samt"]);
                }
                oldbranch1 = drs["purpose"].ToString();
            }
            if (oldbranch != "")
            {
                CommonReportModel tot = getTotalSan(oldbranch, dtSalbr, totalsan);
                grandsantot.Add(totalsan); //added by chaitanya on 14/03/2020
                tot.RowId = RowCnt++;
                lst.Add(tot);

            }

            //added by chaitanya on 14/03/2020 -- start
            if (oldbranch != "")
            {
                decimal sum = 0;
                for (int i = 0; i < grandsantot.Count; i++)
                {
                    sum += grandsantot[i];
                }
                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    SlNo = "Grand Total",
                    column5 = ReportColConvertToDecimal( sum.ToString())
                });
                // end
            }
            return lst;

        }
        public async Task<IList<CommonReportModel>> PFRepayableEffData(string loan, string from, string To)
        {
            decimal totalsan = 0;

            decimal temptotal = 0; //added by chaitanya on 14/03/2020

            string oldbranch1 = "";
            if (loan != "^1")
            {
                string[] loans = loan.Split(',');
                StringBuilder ATypes = new StringBuilder();
                if (loans.Length > 1)
                {
                    foreach (string word in loans)
                    {
                        ATypes.Append("'");
                        ATypes.Append(word);

                        ATypes.Append("', ");
                    }
                    loan = ATypes.ToString(0, ATypes.Length - 2);
                }
            }
            CommonReportModel crm = new CommonReportModel();
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int RowCnt = 0;


            string branch1 = "";
            string oldbranch = "";

            int SlNo = 1;
            string cond = "";



            //string ipmn = "01-01-01";
            // IList<PrrepayableDataModel> lstDept = new List<PrrepayableDataModel>();
            DateTime Fdate = DateTime.Now, Tdate = DateTime.Now;
            if (from == "^2")
            {
                from = "01-01-01";
            }

            if (To == "^3")
            {
                To = "01-01-01";
            }
            DateTime str = Convert.ToDateTime(from);
            string fromdate = str.ToString("yyyy-MM-dd");
            string[] from_sa = fromdate.Split('-');
            string s1 = from_sa[0];
            string s2 = from_sa[1];
            string s3 = from_sa[2];

            DateTime str1 = Convert.ToDateTime(To);
            string todate = str1.ToString("yyyy-MM-dd");
            string[] to_sa = todate.Split('-');
            string to_s1 = to_sa[0];
            string to_s2 = to_sa[1];
            string to_s3 = to_sa[2];


            if (loan == "^1")
            {
                loan = "0";
            }
            if (loan != "^1" && from != "" && To != "")
            {
                cond = " where process=1   and al.process_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " ) group by b.name,pam.purpose_name,al.emp_code,w.ShortName,al.sanction_date,al.process_date,des.code  order by al.Sanction_date desc";

            }



            string qry = "select des.code as desig,al.process_date as pdate,al.emp_code as EmpCode,w.ShortName as Name,al.Sanction_date as sanctiondate," +
                "case when b.Name!='OtherBranch' then b.Name else 'HeadOffice' end as grpcol, count(*) as No_loans,sum(al.amount_recommended_for_sanction) " +
                "as samt ,pam.purpose_name as purpose from pr_emp_pf_repayable_loan al join pr_loan_master on al.pf_loans_id = pr_loan_master.loan_id " +
                "join Employees w on w.empid = al.emp_code join designations des on des.id=w.currentdesignation join branches b on b.id = w.branch " +
                "join departments dept on dept.id = w.department join designations d on d.id = w.currentdesignation " +
                "join pr_purpose_of_advance_master pam on pam.id = al.purpose_of_advance inner join pr_loan_master lm on lm.loan_id = al.pf_loans_id  ";
            if (loan != "" && loan == "0")
            {
                qry += " where al.process_date between DATEFROMPARTS(" + s1 + ", " + s2 + ", " + s3 + ") and DATEFROMPARTS(" + to_s1 + ", " + to_s2 + ", " + to_s3 + " ) group by b.name,pam.purpose_name,al.emp_code,w.ShortName,al.sanction_date,al.process_date,des.code  order by al.Sanction_date desc ";

            }
            else if (loan != "")
            {
                qry += cond;

            }



            DataTable dt = new DataTable();
            DataSet ds = await _sha.Get_MultiTables_FromQry(qry);
            DataTable dtSalbr = ds.Tables[0];

            foreach (DataRow drs in dtSalbr.Rows)
            {
                branch1 = drs["purpose"].ToString();

                if (oldbranch != "" && oldbranch != branch1)
                {
                    SlNo = SlNo++;
                    ////prev. br. footer
                    //CommonReportModel tot = getTotalSan1(oldbranch, dtSalbr, totalsan);
                    //tot.RowId = RowCnt++;
                    //lst.Add(tot);

                    ////grp header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",
                    //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //            + ReportColHeader(0, "Loan Type ", branch1),
                    //    column2 = "`",
                    //    column3 = "`",
                    //    column4 = "`",
                    //    column5 = "`",
                    //    column6 = "`",
                    //    column7 = "`",
                    //    column8 = "`",
                    //    column9 = "`",
                    //});

                    //rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2 = "EmpCode",
                    //    column3 = "Name",
                    //    column4 = "Branch",
                    //    column5 = "Designation",
                    //    column6 = "Loan Type",
                    //    column7 = "Sanction Date",
                    //    column8 = "Process Date",
                    //    column9 = "Sanction Amount",
                    //});
                }
                else if (oldbranch == "")
                {
                    ////grp header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",
                    //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //            + ReportColHeader(0, "Loan Type ", branch1),
                    //    column2 = "`",
                    //    column3 = "`",
                    //    column4 = "`",
                    //    column5 = "`",
                    //    column6 = "`",
                    //    column7 = "`",
                    //    column8 = "`",
                    //    column9 = "`",
                    //});

                    //rows header
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "R",
                    //    SlNo = "S.No",
                    //    column2 = "EmpCode",
                    //    column3 = "Name",
                    //    column4 = "Branch",
                    //    column5 = "Designation",
                    //    column6 = "Loan Type",
                    //    column7 = "Sanction Date",
                    //    column8 = "Process Date",
                    //    column9 = "Sanction Amount",
                    //});

                }
                oldbranch = drs["purpose"].ToString();
                if (oldbranch1 != branch1)
                {
                    temptotal += totalsan; //added by chaitanya on 14/03/2020
                    //totalsan = 0; // resetting the sanction amount to 0 for every change in purpose type so commented //chaitanya on 14/03/2020
                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),
                        column2 = drs["EmpCode"].ToString(),
                        column3 = drs["Name"].ToString(),
                        column4 = drs["grpcol"].ToString(),
                        column5 = drs["desig"].ToString(),
                        column6 = drs["purpose"].ToString(),
                        column7 = Convert.ToDateTime(drs["sanctiondate"]).ToString("dd/MM/yyyy"),
                        column8 = Convert.ToDateTime(drs["pdate"]).ToString("dd/MM/yyyy"),
                        column9 = ReportColConvertToDecimal( drs["samt"].ToString()),
                        column10 = drs["purpose"].ToString()
                    });
                    totalsan = totalsan + Convert.ToDecimal(drs["samt"]);
                }
                else
                {

                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        SlNo = SlNo++.ToString(),
                        column2 = drs["EmpCode"].ToString(),
                        column3 = drs["Name"].ToString(),
                        column4 = drs["grpcol"].ToString(),
                        column5 = drs["desig"].ToString(),
                        column6 = drs["purpose"].ToString(),
                        column7 = Convert.ToDateTime(drs["sanctiondate"]).ToString("dd/MM/yyyy"),
                        column8 = Convert.ToDateTime(drs["pdate"]).ToString("dd/MM/yyyy"),
                        column9 = ReportColConvertToDecimal(drs["samt"].ToString()),
                        column10 = drs["purpose"].ToString()
                    });
                    totalsan = totalsan + Convert.ToDecimal(drs["samt"]);
                }
                oldbranch1 = drs["purpose"].ToString();
            }
            if (oldbranch != "")
            {
                CommonReportModel tot = getTotalSan1(oldbranch, dtSalbr, totalsan);
                tot.RowId = RowCnt++;
                lst.Add(tot);

            }





            return lst;

        }


        private CommonReportModel getTotalSan(string branch, DataTable dt, decimal totalsan)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["purpose"].ToString() == branch)
                .Select(x => new { tot = totalsan.ToString() }).FirstOrDefault();




            var arrTots = val.tot;


            var tot = new CommonReportModel
            {
                RowId = 0,
                HRF = "F",
                //SlNo = "<span style='color:#eef8fd'>^</span>"
                //+ ReportColFooter(130, "Sanction Amount ", arrTots)

                SlNo = "Sanction Amount",
                column5 = ReportColConvertToDecimal( arrTots)
            };

            return tot;
        }

        private CommonReportModel getTotalSan1(string branch, DataTable dt, decimal totalsan)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["purpose"].ToString() == branch)
                .Select(x => new { tot = totalsan.ToString() }).FirstOrDefault();




            var arrTots = val.tot;


            var tot = new CommonReportModel
            {
                RowId = 0,
                HRF = "F",
                //SlNo = "<span style='color:#eef8fd'>^</span>"
                //+ ReportColFooter(130, "Sanction Amount ", arrTots)

                SlNo = "Sanction Amount",
                column9 = ReportColConvertToDecimal( arrTots)
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
        private CommonReportModel getTotal(string branch, DataTable dt, decimal noofloans, decimal sanctionamt)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["purpose"].ToString() == branch)
              //.Select(x => new { tot = x["Total"].ToString() }).FirstOrDefault();
              .Select(x => new { tot = noofloans.ToString() + "^" + sanctionamt.ToString() }).FirstOrDefault();
            var arrTots = val.tot.Split('^');
            decimal DTtlLnAmt = Convert.ToDecimal(arrTots[1].ToString()) + 0.00M;
            string NTtlLoanAMount = String.Format("{0:n}", DTtlLnAmt);

            var tot = new CommonReportModel
            {
                RowId = 0,
                HRF = "F",
                //SlNo = "<span style='color:#eef8fd'>^</span>"
                //+ ReportColFooterValueOnly(20, "Grand Total")
                //  + ReportColFooterValueOnly(47, arrTots[0])
                //+ ReportColFooterValueOnly(52, arrTots[1])
                SlNo = "Grand Total",
                column3 = arrTots[0],
                column4 = NTtlLoanAMount,
            };

            return tot;
        }
        //pf interest calculatutions
        public async Task<IList<CommonReportModel>> PFintcalData(string empCode, string fy)
        {
            string oldempid1 = "";
            string oldempid = "";
            int RowCnt = 0;
            int Eyear = 0001;
            int Fyear = 0001;
            long er_basic = 0;

            decimal ownshare = 0;
            decimal bankshare = 0;
            decimal vpfshare = 0;
            decimal ownshareint = 0;
            decimal bankshareint = 0;
            decimal vpfint = 0;


            if (fy.Contains("^"))
            {
                Eyear = DateTime.Now.Year;
                Fyear = DateTime.Now.Year - 1;
            }
            if (fy != null && fy != "^2")
            {
                Eyear = int.Parse(fy);
                Fyear = int.Parse(fy) - 1;
            }
            if (empCode.Contains("^"))
            {
                empCode = "0";

                fy = "1900";
            }
            string empid = empCode;

            string qry = " select distinct pay.uan_no as pfno,obshare.emp_code as EmpId,e.ShortName as EmpName,REPLACE(RIGHT(CONVERT(VARCHAR(11), obshare.fm, 106), 8), ' ', '-') as orderbyfm ,obshare.fm , " +
                " own_share as own,bank_share as bank,vpf as vpf,own_share_intrst_amount as ownint,bank_share_intrst_amount as bankint " +
                " , vpf_intrst_amount as vpfint from pr_ob_share obshare join pr_emp_general pay on pay.emp_code=obshare.emp_code join employees e " +
  " on e.EmpId = obshare.emp_code WHERE obshare.fm between DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS" +
 " (" + Eyear + ", 03, 31 ) AND obshare.active = 1";

            if (empCode != "All")
            {
                qry += " AND obshare.emp_code in ( " + empCode + ") order by  obshare.emp_code,obshare.fm desc ;"; //ORDER BY obshare.fm
            }
            else
            {
                qry += " order by obshare.emp_code,obshare.fm desc ;";
            }

            //return await _sha.Get_Table_FromQry(qry);

            DataTable dt = await _sha.Get_Table_FromQry(qry);
            try
            {
                foreach (DataRow dr in dt.Rows)
                {

                    empid = dr["EmpId"].ToString();

                    //string dd = dr["EmpId"].ToString();
                    if (oldempid != empid)
                    {
                        var grpdata = dr["EmpId"].ToString();
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>~</span>"
                            + ReportColHeader(0, "EmpCode", dr["EmpId"].ToString())
                             //grpclmn = "<span style='color:#C8EAFB'>^</span>"
                             + ReportColHeader(45, "Name", dr["EmpName"].ToString())
                             + ReportColHeader(45, "PF NO", dr["pfno"].ToString())

                        };

                        lst.Add(crm);

                    }

                    oldempid = dr["EmpId"].ToString();

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        grpclmn = dr["orderbyfm"].ToString(),
                        //  column2 = dr["orderbyfm"].ToString(),
                        column3 = dr["own"].ToString(),
                        column4 = dr["bank"].ToString(),
                        column5 = dr["vpf"].ToString(),
                        column6 = dr["ownint"].ToString(),
                        column7 = dr["bankint"].ToString(),
                        column8 = dr["vpfint"].ToString(),


                    };

                    lst.Add(crm);

                    ownshare = Convert.ToInt64(dr["own"].ToString());
                    bankshare = Convert.ToInt64(dr["bank"].ToString());
                    vpfshare = Convert.ToInt64(dr["vpf"].ToString());
                    try
                    {
                        ownshareint = Convert.ToInt64(dr["ownint"].ToString());
                    }
                    catch (Exception e)
                    {
                    }
                    try
                    {
                        bankshareint = Convert.ToInt64(dr["bankint"].ToString());
                    }
                    catch (Exception e)
                    {
                    }
                    try
                    {
                        vpfint = Convert.ToInt64(dr["vpfint"].ToString());
                    }
                    catch (Exception e)
                    {
                    }

                    if (ownshare != 0)
                    {
                        if (oldempid1 != empid)
                        {
                            crm = new CommonReportModel
                            {


                                RowId = RowCnt++,
                                HRF = "F",
                                //column3
                                grpclmn = "Total",
                                column3 = ownshare.ToString(),
                                column4 = bankshare.ToString(),
                                column5 = vpfshare.ToString(),
                                column6 = ownshareint.ToString(),
                                column7 = bankshareint.ToString(),
                                column8 = vpfint.ToString()
                                //grpclmn = "<span style='color:#eef8fd'>^</span>"
                                //            + ReportColFooterValueOnly(10, "Total")
                                //          + ReportColFooterValueOnly(25, ownshare.ToString())
                                //+ ReportColFooterValueOnly(20, bankshare.ToString())
                                //+ ReportColFooterValueOnly(25, vpfshare.ToString())
                                //+ ReportColFooterValueOnly(25, ownshareint.ToString())
                                //+ ReportColFooterValueOnly(25, bankshareint.ToString())
                                //+ ReportColFooterValueOnly(25, vpfint.ToString())

                            };
                            lst.Add(crm);
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return lst;

        }

        //pf contribution effective
        public async Task<IList<CommonReportModel>> PFContributionEffData(string mnth)
        {
            decimal empopgbalance = 0;
            decimal empmonth = 0;
            decimal empclbal = 0;
            decimal empropbal = 0;
            decimal emprmonth = 0;
            decimal emprclsgbal = 0;
            decimal totals = 0;
            decimal nrloan = 0;
            decimal intst = 0;
            CommonReportModel crm = new CommonReportModel();
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int RowCnt = 0;


            string branch1 = "";
            string oldbranch = "";

            int SlNo = 1;


            string ipmn = "01-01-01";
            // IList<PrrepayableDataModel> lstDept = new List<PrrepayableDataModel>();
            DateTime Fdate = DateTime.Now, Tdate = DateTime.Now;
            if (mnth == "^2")
            {
                mnth = ipmn;
            }

            DateTime str = Convert.ToDateTime(mnth);
            string str1 = str.ToString("yyyy-MM-dd");
            string[] sa = str1.Split('-');
            string s1 = sa[0];
            string s2 = sa[1];
            if (s2.StartsWith("0"))
            {
                s2 = s2.Substring(1);
            }
            string s3 = sa[2];



            string qry = "select  ob.emp_code,emp.ShortName," +
                         " ob.own_share_open emp_op_bal, ps.dd_provident_fund emp_month, ob.own_share_total emp_clg_bal," +
                         "  ob.bank_share_open empr_op_bal," +

                        //"(ps.dd_provident_fund - 1250) empr__month, " + // newly added on 12/05/2020 by chaitanya
                        "(case ps.dd_provident_fund when 0 then 0 else ps.dd_provident_fund end) empr__month, " +
                        //end
                         "ob.bank_share_total empr_clg_bal," +
                         "  sum(ob.own_share_open + ps.dd_provident_fund + ob.own_share_total + " +
                         "  ob.bank_share_open + " +

                         //"(ps.dd_provident_fund - 1250) " + // newly added on 12/05/2020 by chaitanya
                         "(case ps.dd_provident_fund when 0 then 0 else ps.dd_provident_fund end) " +
                         //end
                         " + ob.bank_share_total) as total," +
                         "  (own_share_intrst_amount + vpf_intrst_amount + bank_share_intrst_amount) as interest,ob.vpf ,ob.vpf_open ,ob.vpf_total " +
                         "  from pr_ob_share ob" +
                         "  join pr_emp_payslip ps on ob.emp_code = ps.emp_code and ps.spl_type='Regular'" +
                         "  join Employees emp on emp.EmpId = ob.emp_code" +
                         "  where month(ob.fm) = '" + s2 + "' and year(ob.fm) = '" + s1 + "' and" +
                         "  month(ps.fm) = '" + s2 + "' and year(ps.fm) = '" + s1 + "' " +
                         " group by" +
                         " ob.own_share_open, ps.dd_provident_fund, ob.own_share_total," +
                         "  ob.bank_share_open, ps.dd_provident_fund, ob.bank_share_total," +
                         " own_share_intrst_amount, vpf_intrst_amount, bank_share_intrst_amount," +
                         " ob.emp_code, emp.ShortName,ob.vpf ,ob.vpf_open ,ob.vpf_total order by emp_code asc";


            DataTable dt = new DataTable();
            DataSet ds = await _sha.Get_MultiTables_FromQry(qry);
            DataTable dtSalbr = ds.Tables[0];

            int count = 0;
            int count1 = 0;
            string newemp = "";
            string oldemp = "";
            if (dtSalbr.Rows.Count > 0)
            {
                count = dtSalbr.Rows.Count;

                foreach (DataRow drs in dtSalbr.Rows)
                {
                    count1 = count1;
                    string emp_op_bal = drs["emp_op_bal"].ToString();
                    string empr_op_bal = drs["empr_op_bal"].ToString();
                    string total = drs["total"].ToString();
                    string vpf_open = drs["vpf_open"].ToString();
                    string interst = drs["interest"].ToString();
                    if (emp_op_bal == "" || emp_op_bal == null || emp_op_bal == "0")
                    {
                        emp_op_bal = "0";
                    }
                    
                    if (empr_op_bal == "" || empr_op_bal == null)
                    {
                        empr_op_bal = "0";
                    }
                    if (total == "" || total == null)
                    {
                        total = "0";
                    }
                    if (vpf_open == "" || vpf_open == null  || vpf_open=="0")
                    {
                        vpf_open = "0";
                    }
                    if (interst == "" || interst == "0" || interst == null)
                    {
                        interst = "0";
                    }
                    newemp = drs["emp_code"].ToString();

                    //if (oldemp == "")
                    //{
                    // for first record
                    //lst.Add(new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",
                    //    //column1 = "<span style='color:#C8EAFB'>~</span>"
                    //    //                + ReportColHeader(0, "Emp Code  ", drs["emp_code"].ToString())
                    //    //                 + ReportColHeader(10, "Employee Name  ", drs["ShortName"].ToString()),
                    //    //                  //+ ReportColHeader(10, "Sanction Date  ", drs["sanction_date"].ToString()),
                    //    column4 = "`",
                    //    column5 = "`",
                    //    column6 = "`",
                    //    column7 = "`",
                    //    column8 = "`",
                    //    column9 = "`",
                    //    column10 = "`",
                    //    //column11 = "`",
                    //    column12 = "`",
                    //    column13 = "`",
                    //    column14 = "`",
                    //    column15= "`",
                    //});
                    //}

                    if (newemp != oldemp && oldemp != "")
                    {
                        // employee changes Then total
                        //crm = new CommonReportModel
                        //{
                        //    RowId = RowCnt++,
                        //    //HRF = "F",
                        //    column1 = "Total",
                        //    column4 = empopgbalance.ToString(),
                        //    column5 = empmonth.ToString(),
                        //    column6 = empclbal.ToString(),
                        //    column7 = empropbal.ToString(),
                        //    column8 = emprmonth.ToString(),
                        //    column9 = emprclsgbal.ToString(),
                        //    column10 = totals.ToString(),
                        //    //column11 = nrloan.ToString(),
                        //    column12 = intst.ToString(),
                        //    column13 = "`",
                        //    column14 = "`",
                        //    column15 = "`",
                        //};
                        //lst.Add(crm);
                        // employee changes header
                        //lst.Add(new CommonReportModel
                        //{
                        //    RowId = RowCnt++,
                        //    HRF = "H",
                        //    //column1 = "<span style='color:#C8EAFB'>~</span>"
                        //    //                + ReportColHeader(0, "Emp Code  ", drs["emp_code"].ToString())
                        //    //                 + ReportColHeader(10, "Employee Name  ", drs["ShortName"].ToString()),
                        //    //                 //+ ReportColHeader(10, "Sanction Date  ", drs["sanction_date"].ToString()),
                        //    column4 = "`",
                        //    column5 = "`",
                        //    column6 = "`",
                        //    column7 = "`",
                        //    column8 = "`",
                        //    column9 = "`",
                        //    column10 = "`",
                        //    //column11 = "`",
                        //    column12 = "`",
                        //    column13 = "`",
                        //    column14 = "`",
                        //    column15 = "`",
                        //});
                        empopgbalance = 0;
                        empmonth = 0;
                        empclbal = 0;
                        empropbal = 0;
                        emprmonth = 0;
                        emprclsgbal = 0;
                        totals = 0;
                        nrloan = 0;
                        intst = 0;

                        

                        //for next employee totals
                        empopgbalance = empopgbalance + Convert.ToDecimal(emp_op_bal); //Convert.ToDecimal(drs["emp_op_bal"]);
                        empmonth = empmonth + Convert.ToDecimal(drs["emp_month"]);
                        empclbal = empclbal + Convert.ToDecimal(drs["emp_clg_bal"]);
                        empropbal = empropbal + Convert.ToDecimal(empr_op_bal); // Convert.ToDecimal(drs["empr_op_bal"]);
                        emprmonth = emprmonth + Convert.ToDecimal(drs["empr__month"]);
                        emprclsgbal = emprclsgbal + Convert.ToDecimal(drs["empr_clg_bal"]);
                        totals = totals + Convert.ToDecimal(total); // Convert.ToDecimal(drs["total"]);
                        //nrloan = nrloan + Convert.ToDecimal(drs["nr_loans"]);
                        //intst = intst + Convert.ToDecimal(drs["interest"]);
                        if (intst != 0)
                        {
                            intst = intst + Convert.ToDecimal(drs["interest"]);
                        }
                        else
                        {
                            intst = 0;
                        }

                    }
                    decimal d = Convert.ToDecimal(emp_op_bal.ToString());
                    decimal demp_op_bal = d + 0.00M;
                    string Nemp_op_bal = String.Format("{0:n}", demp_op_bal);
                    decimal Demp_month = Convert.ToDecimal(drs["emp_month"].ToString()) + 0.00M;
                    string NDemp_month = String.Format("{0:n}", Demp_month);

                    decimal Demp_clg_bal = Convert.ToDecimal( drs["emp_clg_bal"].ToString()) + 0.00M;
                    string NDemp_clg_bal = String.Format("{0:n}", Demp_clg_bal);

                    decimal Dempr_op_bal = Convert.ToDecimal(empr_op_bal.ToString()) + 0.00M;
                    string NDempr_op_bal = String.Format("{0:n}", Dempr_op_bal);

                    decimal Dempr__month = Convert.ToDecimal(drs["empr__month"].ToString()) + 0.00M;
                    string NDempr__month = String.Format("{0:n}", Dempr__month);

                    decimal Dempr_clg_bal = Convert.ToDecimal(drs["empr_clg_bal"].ToString()) + 0.00M;
                    string NDempr_clg_bal = String.Format("{0:n}", Dempr_clg_bal);


                    decimal Dtotal = Convert.ToDecimal(total.ToString()) + 0.00M;
                    string NDtotal = String.Format("{0:n}", Dtotal);

                    decimal Dinterest = Convert.ToDecimal(interst.ToString()) + 0.00M;
                    string NDinterest = String.Format("{0:n}", Dinterest);

                    decimal DVpf = Convert.ToDecimal(drs["vpf"].ToString()) + 0.00M;
                    string NDVpf = String.Format("{0:n}", DVpf);

                    decimal Dvpf_open = Convert.ToDecimal(vpf_open.ToString()) + 0.00M;
                    string NDvpf_open = String.Format("{0:n}", Dvpf_open);

                    decimal Dvpf_total = Convert.ToDecimal(drs["vpf_total"].ToString()) + 0.00M;
                    string NDvpf_total = String.Format("{0:n}", Dvpf_total);


                    lst.Add(new CommonReportModel
                    {
                        RowId = RowCnt++,
                        column1 = SlNo++.ToString(),
                        column16 = drs["emp_code"].ToString(),
                        column17 = drs["ShortName"].ToString(),
                        column4 = Nemp_op_bal.ToString(),//drs["emp_op_bal"].ToString(),
                        column5 =  NDemp_month.ToString(),
                        column6 = NDemp_clg_bal.ToString(),
                        column7 =  NDempr_op_bal.ToString(), //drs["empr_op_bal"].ToString(),
                        column8 =  NDempr__month.ToString(),
                        column9 =  NDempr_clg_bal.ToString(),
                        column10 =  NDtotal.ToString() , // drs["total"].ToString(),
                        //column11 = drs["nr_loans"].ToString(),
                        column12 =  NDinterest.ToString(),
                        column13 = NDVpf.ToString(),
                        column14 = NDvpf_open.ToString(), // drs["vpf_open"].ToString(),
                        column15 =  NDvpf_total.ToString(),

                    });;

                    if (oldemp == newemp || oldemp == "")
                    {
                        // for same employee data concatination
                        empopgbalance = empopgbalance + Convert.ToDecimal(emp_op_bal); // Convert.ToDecimal(drs["emp_op_bal"]);
                        empmonth = empmonth + Convert.ToDecimal(drs["emp_month"]);
                        empclbal = empclbal + Convert.ToDecimal(drs["emp_clg_bal"]);
                        empropbal = empropbal + Convert.ToDecimal( empr_op_bal); // Convert.ToDecimal(drs["empr_op_bal"]);
                        emprmonth = emprmonth + Convert.ToDecimal(drs["empr__month"]);
                        emprclsgbal = emprclsgbal + Convert.ToDecimal(drs["empr_clg_bal"]);
                        totals = totals + Convert.ToDecimal(total); // Convert.ToDecimal(drs["total"]);
                        //nrloan = nrloan + Convert.ToDecimal(drs["nr_loans"]);
                        //intst = intst + Convert.ToDecimal(drs["interest"]);
                        if (intst != 0)
                        {
                            intst = intst + Convert.ToDecimal(drs["interest"]);
                        }
                        else
                        {
                            intst = 0;
                        }
                    }

                    if (count1 == count)
                    {

                        lst.Add(crm);
                    }

                    oldemp = drs["emp_code"].ToString();

                }
                //oldemp = "";
                //foreach (DataRow dr1 in dtSalbr.Rows)
                //{
                //    newemp = dr1["emp_code"].ToString();
                //    if (newemp != oldemp && oldemp !="")
                //    {
                //        crm = new CommonReportModel
                //        {
                //            RowId = RowCnt++,
                //            //HRF = "F",
                //            column1 = "Total",
                //            column4 = empopgbalance.ToString(),
                //            column5 = empmonth.ToString(),
                //            column6 = empclbal.ToString(),
                //            column7 = empropbal.ToString(),
                //            column8 = emprmonth.ToString(),
                //            column9 = emprclsgbal.ToString(),
                //            column10 = totals.ToString(),
                //            column11 = nrloan.ToString(),
                //            column12 = intst.ToString()
                //        };
                //        lst.Add(crm);
                //    }
                //    oldemp= dr1["emp_code"].ToString();
                //}

            }



            return lst;

        }

      
        //pf contribution card
        public async Task<IList<CommonReportModel>> PfcontributioncardData(string empCode, string fy)
        {
            string oldempid = "";
            int RowCnt = 0;
            int Eyear = 0001;
            int Fyear = 0001;
            decimal ownsh_bal = 0; decimal bank_bal = 0; decimal vpf_bal = 0;
            decimal ownshare = 0;
            decimal ownshare1 = 0; decimal bankshare1 = 0; decimal vpfshare1 = 0;
            decimal vpfshare = 0;
            decimal bankshare = 0;
            string oldempid1 = "";

            decimal total_int_op_bal = 0; decimal total_intaccru = 0.00M; decimal TOTAL_Intrest = 0; decimal TOTAL_pre_own_contribution = 0;
            decimal vpfintaccru = 0; decimal pfintaccru = 0; decimal bankintaccru = 0; decimal Interst_on_opbal =0.00M;

            decimal Bankreturn = 0; decimal VPFreturn = 0; decimal PFreturn = 0;

            decimal os_open_int = 0; decimal bs_open_int = 0; decimal vpf_open_int = 0;
            decimal ownbal = 0; decimal bankbal = 0; decimal vpfbal = 0;
            decimal total = 0; decimal total_opbal = 0; decimal tot_amut = 0; decimal totbal = 0;
            decimal PREV_YEAR_FROM_OWN_CONTRIBUTION = 0;
            decimal PREV_YEAR_FROM_BANK_CONTRIBUTION = 0;
            decimal non_refundable_loans_own_contn = 0; decimal non_refundable_loans_bank_contn = 0; decimal total_non_refundable_loan_availed = 0;
            decimal non_refundable_loans_vpf_contn = 0;
            decimal non_refundable_own_cont = 0.00M;
            decimal? total_rep_loan_outstanding = 0;

            if (fy.Contains("^"))
            {
                Eyear = DateTime.Now.Year;
                Fyear = DateTime.Now.Year - 1;
            }
            if (fy != null && fy != "^2")
            {
                Eyear = int.Parse(fy);
                Fyear = int.Parse(fy) - 1;
            }

            string empid = "";

            string qry = "";
            //string empcodes = empCode;

            if (empCode.Contains("^1"))
            {
                empCode = "0";
            }

            int count = 0;

            if (empCode != "All" && empCode != "0")
            {

                //qry = "select ob.emp_code,e.ShortName,pay.pf_no,pay.uan_no, REPLACE(RIGHT(CONVERT(VARCHAR(11), ob.fm, 106), 8), ' ', '-') as fm,own_share,bank_share ,vpf,own_share_total,bank_share_total,vpf_total , " +
                //    " pfbal.os_open AS ownbal ,pfbal.bs_open as bankbal, pfbal.vpf_open as vpfbal,pfbal.os_open_int,pfbal.bs_open_int,pfbal.vpf_open_int,active = 1 , pfbal.pf_return,pfbal.vpf_return,pfbal.bank_return,pfbal.os_cur_int,pfbal.vpf_cur_int,pfbal.bs_cur_int, active = 1 " +
                //    " from pr_ob_share ob join pr_emp_general pay on ob.emp_code = pay.emp_code join Employees e on e.EmpId = ob.emp_code JOIN pr_pf_open_bal_year pfbal on pfbal.emp_code = e.EmpId  " +
                //    " where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and ob.emp_code in (" + empCode + ")  and pay.active=1 and pfbal.fy=" + Fyear + " order by ob.fm asc ";
                //qry = "select distinct h.fm1,h.emp_code,h.ShortName,h.pf_no,h.uan_no,h.fm,h.own_share , h.bank_share ,h.vpf , h.own_share_total , " +
                //        " h.bank_share_total,h.vpf_total  ,h.ownbal,h.bankbal,h.vpfbal, h.os_open_int,h.bs_open_int,h.vpf_open_int,h.active,h.pf_return," +
                //        " h.vpf_return,h.bank_return,h.os_cur_int,h.vpf_cur_int,h.bs_cur_int,h.op_bal_inst,h.op_bal_inst_year,h.op_bal_inst_NRloan " +
                //        " from(select ob.fm as fm1, ob.emp_code, e.ShortName, pay.pf_no, pay.uan_no, REPLACE(RIGHT(CONVERT(VARCHAR(11), ob.fm, 106), 8), ' '," +
                //        " '-') as fm, own_share, bank_share, vpf, own_share_total, bank_share_total, vpf_total, pfbal.os_open AS ownbal, " +
                //        " pfbal.bs_open as bankbal,pfbal.vpf_open as vpfbal, pfbal.os_open_int, pfbal.bs_open_int, pfbal.vpf_open_int, active = 1, " +
                //        " pfbal.pf_return, pfbal.vpf_return,pfbal.bank_return, pfbal.os_cur_int, pfbal.vpf_cur_int, pfbal.bs_cur_int,pfbal.op_bal_inst," +
                //        " pfbal.op_bal_inst_year,pfbal.op_bal_inst_NRloan from pr_ob_share ob join pr_emp_general pay on ob.emp_code = pay.emp_code join " +
                //        " Employees e on e.EmpId = ob.emp_code JOIN pr_pf_open_bal_year pfbal on pfbal.emp_code = e.EmpId where ob.fm between " +
                //        " DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and ob.emp_code in (" + empCode + ") and pay.active = 1 and pfbal.fy = " + Fyear + ") as h";
                qry = "select distinct h.fm1,h.emp_code,h.ShortName,h.pf_no,h.uan_no,h.fm,h.own_share +" +
                    " ( case when(select count(c1.own_share) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code " +
                    "and h.fm1 = c1.fm) = 0 then 0 else (select c1.own_share from pr_ob_share_encashment c1 " +
                    "where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end)+ ( case when(select count(c1.own_share) " +
                    "from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else " +
                    "(select CASE WHEN COUNT(1) > 0 THEN sum(c1.own_share) ELSE 0 END from pr_ob_share_adhoc c1 " +
                    "where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) own_share,h.bank_share + ( case when(select count(c1.bank_share)" +
                    " from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.bank_share from pr_ob_share_encashment " +
                    "c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end)+( case when(select count(c1.bank_share) " +
                    "from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 " +
                    "else (select CASE WHEN COUNT(1) > 0 THEN sum(c1.bank_share) ELSE 0 END from pr_ob_share_adhoc c1 " +
                    "where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) bank_share,h.vpf + ( case when(select count(c1.vpf) " +
                    "from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 " +
                    "else (select c1.vpf from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) +" +
                    "( case when(select count(c1.vpf) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 " +
                    "then 0 else (select CASE WHEN COUNT(1) > 0 THEN sum(c1.vpf) ELSE 0 END from pr_ob_share_adhoc c1 " +
                    "where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) vpf,h.own_share_total +" +
                    " ( case when(select count(c1.own_share_total) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0" +
                    " then 0 else (select c1.own_share_total from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) + " +
                    "( case when(select count(c1.own_share_total) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 " +
                    "then 0 else (select c1.own_share_total from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end)  " +
                    "own_share_total,h.bank_share_total + ( case when(select count(c1.bank_share_total) from pr_ob_share_encashment c1 " +
                    "where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.bank_share_total from pr_ob_share_encashment c1 " +
                    "where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) +( case when(select count(c1.bank_share_total) " +
                    "from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else " +
                    "(select c1.bank_share_total from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) " +
                    "bank_share_total,h.vpf_total + ( case when(select count(c1.vpf_total) from pr_ob_share_encashment c1 " +
                    "where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.vpf_total from pr_ob_share_encashment c1 " +
                    "where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end)+(case when(select count(c1.vpf_total) " +
                    "from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else " +
                    "(select c1.vpf_total from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) vpf_total," +
                    "h.ownbal,h.bankbal,h.vpfbal, h.os_open_int,h.bs_open_int,h.vpf_open_int,h.active,h.pf_return,h.vpf_return,h.bank_return," +
                    "h.os_cur_int,h.vpf_cur_int,h.bs_cur_int,h.op_bal_inst,h.op_bal_inst_year,h.op_bal_inst_NRloan from(select ob.fm as fm1, ob.emp_code, e.ShortName, pay.pf_no, pay.uan_no, " +
                    "REPLACE(RIGHT(CONVERT(VARCHAR(11), ob.fm, 106), 8), ' ', '-') as fm, own_share, bank_share, vpf, own_share_total, " +
                    "bank_share_total, vpf_total, pfbal.os_open AS ownbal, pfbal.bs_open as bankbal,pfbal.vpf_open as vpfbal, pfbal.os_open_int," +
                    " pfbal.bs_open_int, pfbal.vpf_open_int, active = 1, pfbal.pf_return, pfbal.vpf_return,pfbal.bank_return, pfbal.os_cur_int, " +
                    "pfbal.vpf_cur_int, pfbal.bs_cur_int,pfbal.op_bal_inst,pfbal.op_bal_inst_year,pfbal.op_bal_inst_NRloan from pr_ob_share ob join pr_emp_general pay  on ob.emp_code = pay.emp_code " +
                    "join Employees e on e.EmpId = ob.emp_code JOIN pr_pf_open_bal_year pfbal on pfbal.emp_code = e.EmpId " +
                    "where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01)  and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and " +
                    "ob.emp_code in (" + empCode + ") and pay.active = 1 and pfbal.fy = " + Fyear + ") as h ";

                string prevQry = "select ISNULL(inst_Acrued_pf_con,0) as inst_accured, ISNULL(pf_return,0) as own_share, ISNULL(vpf_return,0) as vpf, ISNULL(bank_return,0) as bank_share,ISNULL(op_bal_inst,0) as op_bal_inst,ISNULL(op_bal_inst_year,0) as op_bal_inst_year ,ISNULL(op_bal_inst_NRloan,0) as op_bal_inst_NRloan from pr_pf_open_bal_year " +
                "where emp_code = " + Convert.ToString(empCode) + " and fy = (select fy-1 from pr_month_details where active=1) ";

                //string presQry = "select top(1) own_share as own_share, vpf as vpf,bank_share " +
                //"as bank_share from pr_emp_pf_nonrepayable_loan where  year(fm) = year(getdate()) and emp_code =" + Convert.ToString(empCode) + "  order by process_date desc;";
                string presQry = "select ISNULL(sum(own_share),0) as own_share,ISNULL(sum(vpf),0) as vpf,ISNULL(sum(bank_share),0) as bank_share " +
                    "from pr_emp_pf_nonrepayable_loan where emp_code = " + Convert.ToString(empCode) + "  and authorisation = 1 and process = 1 " +
                    "and fm between DATEFROMPARTS(" + Fyear + ", 04, 01)  and DATEFROMPARTS(" + Eyear + ", 03, 31 ) ";
//                string presQry = "select top(1) own_share as own_share, vpf as vpf,bank_share " +
//"as bank_share from pr_emp_pf_nonrepayable_loan where   emp_code =" + Convert.ToString(empCode) + "  order by process_date desc;";
                DataTable dt = await _sha.Get_Table_FromQry(qry);
                DataTable non_dt = await _sha.Get_Table_FromQry(presQry);
                DataTable non_dt1 = await _sha.Get_Table_FromQry(prevQry);
                decimal? non_own_share = 0.00M;
                decimal? non_bank_share = 0.00M;
                decimal? non_vpf = 0; 
                decimal? non_own_share_prev = 0;
                decimal? non_bank_share__prev = 0;
                decimal? non_vpf__prev = 0;
                if (non_dt.Rows.Count > 0)
                {
                    non_own_share = Convert.ToDecimal((non_dt.Rows[0]["own_share"]));
                    non_bank_share = Convert.ToDecimal((non_dt.Rows[0]["bank_share"]));
                    if(non_bank_share==0)
                    {
                        non_bank_share = 0.00M;
                    }
                    non_vpf = Convert.ToDecimal((non_dt.Rows[0]["vpf"]));
                }
                if (non_dt1.Rows.Count > 0)
                {
                    non_own_share_prev = Convert.ToDecimal((non_dt1.Rows[0]["own_share"]));
                    non_bank_share__prev = Convert.ToDecimal((non_dt1.Rows[0]["bank_share"]));
                    non_vpf__prev = Convert.ToDecimal((non_dt1.Rows[0]["vpf"]));
                }

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        count++;
                        empid = dr["emp_code"].ToString();
                        int count1 = dt.Select().Where(s => s["emp_code"].ToString() == empid).Count();
                        if (oldempid != empid)
                        {
                            string qry2 = " select l.emp_code,sum(a.principal_open_amount+a.interest_accured) as Loan_Closing  " +
                                "from pr_emp_adv_loans_adjustments a join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid  join pr_loan_master lm on lm.id = l.loan_type_mid " +
                                "join pr_emp_adv_loans_child cl on l.id = cl.emp_adv_loans_mid  and cl.id = a.emp_adv_loans_child_mid   " +
                                "where l.loan_type_mid in (16, 17, 18, 19, 20, 21,26,27) and " + //added 26,27 in the query on 27/05/2020
                                "a.fm=( select max(fm) from pr_emp_adv_loans_adjustments) " + //added on 30/05/2020
                                "and a.active = 1  " +
                                "and l.emp_code = "+ empid + "  group by  l.emp_code  ";

                            DataTable dt2 = await _sha.Get_Table_FromQry(qry2);
                        
                            if (dt2.Rows.Count > 0)
                            {
                                // Added a loop to get the total number of Loan_Closing "total_rep_loan_outstanding" on 27/05/2020
                                for (int trlo=0; trlo< dt2.Rows.Count; trlo++)
                                {
                                    total_rep_loan_outstanding += Convert.ToDecimal(dt2.Rows[trlo]["Loan_Closing"]);
                                    if(total_rep_loan_outstanding < 0 )
                                    {
                                        total_rep_loan_outstanding = 0;
                                    }
                                }
                                
                            }
                            else
                            {
                                total_rep_loan_outstanding = 0.00M;
                            }
                            //header
                            var grpdata = dr["emp_code"].ToString();
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "H",
                                grpclmn = "<span style='color:#C8EAFB'>~</span>"
                                    + ReportColHeader(0, "Emp Code", dr["emp_code"].ToString())
                                    + ReportColHeader(17, "Emp Name", dr["ShortName"].ToString())
                                    + ReportColHeader(17, "PF NO", dr["pf_no"].ToString())
                                    + ReportColHeader(17, "Uan No", dr["uan_no"].ToString()),
                                //column2 = "`",
                                //column3 = "`",
                                //column4 = "`",
                                //column5 = "`",

                            };
                            lst.Add(crm);
                            
                            //rows
                            // op bal
                            ownbal = Convert.ToDecimal(dr["ownbal"]);
                            bankbal = Convert.ToDecimal(dr["bankbal"]);
                            vpfbal = Convert.ToDecimal(dr["vpfbal"]);
                            total_opbal =Math.Round( ownbal + bankbal + vpfbal ,2);

                            if (non_dt1.Rows.Count > 0)
                            {                              

                                PREV_YEAR_FROM_OWN_CONTRIBUTION = Convert.ToDecimal(non_dt1.Rows[0]["own_share"]); //Convert.ToDecimal(dr["pf_return"]);
                                PREV_YEAR_FROM_OWN_CONTRIBUTION += Convert.ToDecimal(non_dt1.Rows[0]["vpf"]);
                                PREV_YEAR_FROM_BANK_CONTRIBUTION = Convert.ToDecimal(non_dt1.Rows[0]["bank_share"]);
                            }
                            else
                            {
                                PREV_YEAR_FROM_OWN_CONTRIBUTION = 0;//Convert.ToDecimal(dr["pf_return"]);
                                PREV_YEAR_FROM_OWN_CONTRIBUTION += 0;
                                PREV_YEAR_FROM_BANK_CONTRIBUTION = 0;
                            }
                                crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                // grpclmn 
                                grpclmn = "OPBal",
                                column2 =ReportColConvertToDecimal( dr["ownbal"].ToString()),
                                column3 = ReportColConvertToDecimal( dr["bankbal"].ToString()),
                                column4 = ReportColConvertToDecimal( dr["vpfbal"].ToString()),
                                column5 =ReportColConvertToDecimal( Convert.ToString(total_opbal)),

                            };
                                 lst.Add(crm);
                        }

                        oldempid = dr["emp_code"].ToString();
                        decimal own_share = 0;
                        decimal bank_share = 0;
                        decimal vpf = 0;
                        // own bank vpf column wise total
                        if (!dr.IsNull("own_share"))
                        {
                            own_share = Convert.ToDecimal(dr["own_share"].ToString());
                        }
                        else
                        {
                            own_share = 0;
                        }
                        if (!dr.IsNull("bank_share"))
                        {
                            bank_share = Convert.ToDecimal(dr["bank_share"].ToString());
                        }
                        else
                        {
                            bank_share = 0;
                        }
                        if (!dr.IsNull("vpf"))
                        {
                            vpf = Convert.ToDecimal(dr["vpf"].ToString());
                        }
                        else
                        {
                            vpf = 0;
                        }
                        total =Math.Round(own_share + bank_share + vpf ,2);
                        //string id =Convert.ToString( total);
                        tot_amut +=(total+ total_opbal);
                         totbal += (tot_amut+ total_opbal);
                        total_opbal = 0;
                        decimal bank_share1 = Math.Round( Convert.ToDecimal(dr["bank_share"].ToString()),2);
                        int OwSh = Convert.ToInt32(dr["own_share"].ToString());
                        decimal DrownShare = OwSh + 0.00M;
                        int Nbksh1 = Convert.ToInt32(dr["bank_share"].ToString());
                        decimal DrBksh1 = Nbksh1 + 0.00M;
                        int Nvpf = Convert.ToInt32(dr["vpf"].ToString());
                        decimal DrVpf = Nvpf + 0.00M;
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            grpclmn = dr["fm"].ToString(),
                            column2 =ReportColConvertToDecimal( DrownShare.ToString()),
                            column3 = ReportColConvertToDecimal( DrBksh1.ToString()),//dr["bank_share"].ToString(),
                            column4 = ReportColConvertToDecimal( DrVpf.ToString()),
                            column5 = ReportColConvertToDecimal( Convert.ToString(tot_amut)),
                        };

                        lst.Add(crm);
                        try
                        {
                            ownshare = Convert.ToDecimal(dr["ownbal"]);
                        }
                        catch (Exception e)
                        {

                        }
                        try
                        {
                            bankshare = Convert.ToDecimal(dr["bankbal"]);

                        }
                        catch (Exception e)
                        {

                        }
                        // vpf footer
                        try
                        {
                            vpfshare1 = Convert.ToDecimal(dr["vpfbal"]);
                        }
                        catch (Exception e)
                        {

                        }

                        if (!dr.IsNull("own_share"))
                        {
                            if (oldempid != empid)
                            {
                                ownshare1 = Convert.ToDecimal(dr["own_share"]);
                            }
                            ownshare1 = ownshare1 + Convert.ToDecimal(dr["own_share"]);
                        }
                        else
                        {
                            ownshare1 = 0;
                        }
                        // own total
                        ownsh_bal =Math.Round( ownshare + ownshare1 ,2);

                        // bank total
                        if (!dr.IsNull("bank_share"))
                        {
                            if (oldempid != empid)
                            {
                                bankshare1 = Convert.ToDecimal(dr["bank_share"]);
                            }
                            bankshare1 = bankshare1 + Convert.ToDecimal(dr["bank_share"]);
                        }
                        else
                        {
                            bankshare1 = 0;
                        }

                        // bank total
                        bank_bal =Math.Round( bankshare + bankshare1 ,2);

                        // vpf total
                        if (!dr.IsNull("vpf"))
                        {
                            if (oldempid != empid)
                            {
                                vpfshare = Convert.ToDecimal(dr["vpf"]);
                            }
                            vpfshare = vpfshare + Convert.ToDecimal(dr["vpf"]);
                        }
                        else
                        {
                            vpfshare = 0;
                        }
                        //vpf total
                        vpf_bal =Math.Round( vpfshare + vpfshare1 ,2);
                        try
                        {
                            os_open_int = Convert.ToDecimal(dr["os_open_int"].ToString());
                            bs_open_int = Convert.ToDecimal(dr["bs_open_int"].ToString());
                            vpf_open_int = Convert.ToDecimal(dr["vpf_open_int"].ToString());
                            total_int_op_bal =Math.Round(os_open_int + bs_open_int + vpf_open_int ,2);
                            //os_open_int = Convert.ToDecimal(dr["op_bal_inst_year"].ToString());
                            //bs_open_int = Convert.ToDecimal(dr["op_bal_inst"].ToString());
                            //vpf_open_int = Convert.ToDecimal(dr["vpf_open_int"].ToString());
                            //total_int_op_bal = os_open_int + bs_open_int;
                        }
                        catch (Exception e)
                        {

                        }
                        try
                        {
                            // intrest Accrued vpfintcurr
                            if (!dr.IsNull("vpf_cur_int"))
                            {
                                vpfintaccru =Math.Round(Convert.ToDecimal(dr["vpf_cur_int"]),2);
                            }
                            else
                            {
                                vpfintaccru = 0;
                            }
                            if (!dr.IsNull("op_bal_inst_year"))//if (!dr.IsNull("os_cur_int"))
                            {                                
                                pfintaccru =Math.Round(Convert.ToDecimal(dr["op_bal_inst_year"]),2);
                            }
                            else
                            {
                                pfintaccru = 0;
                            }
                            if (!dr.IsNull("bs_cur_int"))
                            {
                                bankintaccru =Math.Round(Convert.ToDecimal(dr["bs_cur_int"]),2);
                            }
                            else
                            {
                                bankintaccru = 0;
                            }
                            total_intaccru = pfintaccru+0.00M ;
                            //total_intaccru = pfintaccru + bankintaccru + vpfintaccru;
                        }
                        catch (Exception e)
                        {

                        }
                        try
                        {
                            // TOTAL_Intrest
                            
                            TOTAL_Intrest =Math.Round(Convert.ToDecimal(total_int_op_bal) + Convert.ToDecimal(total_intaccru),2);
                            if (non_dt1.Rows.Count > 0)
                            {
                                Interst_on_opbal = Math.Round(Convert.ToDecimal(non_dt1.Rows[0]["op_bal_inst"]), 2);
                            }
                      
                           



                        }
                        catch (Exception e)
                        {

                        }


                        try
                        {
                            if (non_dt1.Rows.Count > 0)
                            {
                                foreach (DataRow dr1 in non_dt1.Rows)
                                {
                                    // pre year own contribution   //Bankreturn,VPFreturn,PFreturn
                                    if (!dr1.IsNull("bank_share"))
                                    {
                                        Bankreturn = Math.Round(Convert.ToDecimal((non_dt1.Rows[0]["bank_share"])), 2);

                                    }
                                    else
                                    {
                                        Bankreturn = 0;
                                    }
                                    if (!dr1.IsNull("vpf"))
                                    {
                                        VPFreturn = Math.Round(Convert.ToDecimal((non_dt1.Rows[0]["vpf"])), 2);

                                    }
                                    else
                                    {
                                        VPFreturn = 0;
                                    }
                                    if (!dr1.IsNull("own_share"))
                                    {
                                        PFreturn = Math.Round(Convert.ToDecimal((non_dt1.Rows[0]["own_share"])), 2);

                                    }
                                    else
                                    {
                                        PFreturn = 0;
                                    }
                                }
                                TOTAL_pre_own_contribution = Bankreturn + VPFreturn + PFreturn;
                            }
                          
                        }
                        catch (Exception e)
                        {

                        }
                        // NON REFUNDABLE LOANS 
                        non_refundable_loans_own_contn = Convert.ToDecimal(non_own_share);
                        non_refundable_loans_bank_contn = Convert.ToDecimal(non_bank_share);
                        non_refundable_loans_vpf_contn = Convert.ToDecimal(non_vpf);
                        non_refundable_own_cont =Math.Round(non_refundable_loans_own_contn + non_refundable_loans_vpf_contn ,2);
                        // total NON REFUNDABLE LOANS 
                        total_non_refundable_loan_availed =Math.Round( non_refundable_own_cont + non_refundable_loans_bank_contn ,2);
                        TOTAL_pre_own_contribution =Math.Round( TOTAL_pre_own_contribution + total_non_refundable_loan_availed ,2);

                    }
                    catch (Exception e)
                    {

                    }
                }
                if (dt.Rows.Count > 0)
                {
                    decimal total_balns =Math.Round(total_opbal + tot_amut ,2);
                    if (ownshare != 0)
                    {
                        if (oldempid1 != empid)
                        {
                            crm = new CommonReportModel
                            {

                                RowId = RowCnt++,
                                HRF = "R",
                                grpclmn = "Total",
                                column2 =ReportColConvertToDecimal( ownsh_bal.ToString()),
                                column3 = ReportColConvertToDecimal( bank_bal.ToString()),
                                column4 = ReportColConvertToDecimal( vpf_bal.ToString()),
                                column5 = ReportColConvertToDecimal(Convert.ToString(total_balns)),

                            };

                            lst.Add(crm);
                        }
                    }

                    decimal net_fund_epf = TOTAL_Intrest + total_balns - TOTAL_pre_own_contribution;
                    net_fund_epf = Convert.ToDecimal(net_fund_epf - total_rep_loan_outstanding);
                    decimal op_bal_inst_NRloan = 0.00M;   decimal inst_Acrued_pf_con = 0.00M;
                    if (non_dt1.Rows.Count > 0)
                    {

                        if (!non_dt1.Rows[0].IsNull("op_bal_inst_NRloan"))
                        {
                            op_bal_inst_NRloan = Convert.ToDecimal(non_dt1.Rows[0]["op_bal_inst_NRloan"]);
                            op_bal_inst_NRloan = Math.Round(op_bal_inst_NRloan, 2);
                        }
                    }
                    else
                    {
                        op_bal_inst_NRloan = 0.00M;
                    }
                    if (non_dt1.Rows.Count > 0)
                    {
                        if (!non_dt1.Rows[0].IsNull("inst_accured"))
                        {
                            inst_Acrued_pf_con = Convert.ToDecimal(non_dt1.Rows[0]["inst_accured"]);
                            inst_Acrued_pf_con = Math.Round(inst_Acrued_pf_con, 2);
                        }
                    }
                    else
                    {
                        inst_Acrued_pf_con = 0.00M;
                    }
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                        //+ ReportColFooter(0, "Intrest OP.BAL ", Convert.ToString(total_int_op_bal))
                        + ReportColDataAlign(0, "    Intrest Opening Balance<span style='color:#eef8fd'> </span>",ReportColConvertToDecimal( Convert.ToString(total_int_op_bal)))

                    };

                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                        + ReportColDataAlign(0, "    Intrest Acured (On above Intrest Opening Balance) <span style='color:#eef8fd'></span>",ReportColConvertToDecimal( Convert.ToString(total_intaccru)))

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"

                        //+ ReportColFooter(0, "    ACRUED PF Contribution (Open Bal + Own + Bank + Vpf) ", Convert.ToString(totbal))

                        + ReportColDataAlign(0, "    Acured PF Contribution (Open Bal + Own + Bank + Vpf) <span style='color:#eef8fd'></span>", ReportColConvertToDecimal( Convert.ToString(total_balns)))


                    };
                    lst.Add(crm);
                    
                   //decimal tot_inst_acc_amt=Interst_on_opbal + pfintaccru - op_bal_inst_NRloan;
                    decimal tot_inst_acc_amt = inst_Acrued_pf_con;
                    //5 extra amount and minus 5 rs  340 Retired employee
                    //if (empCode == "340")
                    //{
                    //    tot_inst_acc_amt = tot_inst_acc_amt - 5;
                    //}
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                        + ReportColDataAlign(0, "    Intrest Acrued on PF Contribution <span style='color:#eef8fd'></span>", ReportColConvertToDecimal( Convert.ToString(tot_inst_acc_amt)))

                    };
                    lst.Add(crm);
                    decimal i = Interst_on_opbal + totbal + total_intaccru + total_int_op_bal;
                    decimal EPFINCLUDING_INTREST = Math.Round(Interst_on_opbal + total_balns + total_intaccru + total_int_op_bal);
                    EPFINCLUDING_INTREST = Math.Round(EPFINCLUDING_INTREST, MidpointRounding.AwayFromZero);
                    
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"

                        //+ ReportColFooter(0, "    TOTAL AMOUNT IN EPF INCLUDING INTREST (A) ", Convert.ToString(Interst_on_opbal+ totbal+ total_intaccru+ total_int_op_bal))

                        + ReportColDataAlign(0, "    TOTAL AMOUNT IN EPF INCLUDING INTREST (A) <span style='color:#eef8fd'></span>", ReportColConvertToDecimal( Convert.ToString(EPFINCLUDING_INTREST )))
                        
                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                        + ReportColDataAlign(0, "    NON REFUNDABLE LOAN DETAILS <span style='color:#eef8fd'>           </span>", "-")

                    };

                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        

                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                        + ReportColDataAlign(0, " Availed Till Previous Year From Own Contribution <span style='color:#eef8fd'></span>", ReportColConvertToDecimal( Convert.ToString(PREV_YEAR_FROM_OWN_CONTRIBUTION)))

                    };

                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                       + ReportColDataAlign(0, "Availed  During the Year From Own Contribution  <span style='color:#eef8fd'></span>", ReportColConvertToDecimal( Convert.ToString(non_refundable_own_cont)))

                    };

                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                       + ReportColDataAlign(0, "Availed Till Previous Year From Bank Contribution <span style='color:#eef8fd'></span>", ReportColConvertToDecimal( Convert.ToString(PREV_YEAR_FROM_BANK_CONTRIBUTION)))

                    };

                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                       + ReportColDataAlign(0, "Availed  During the Year From Bank Contribution <span style='color:#eef8fd'></span> ", ReportColConvertToDecimal( Convert.ToString(non_refundable_loans_bank_contn)))

                    };

                    lst.Add(crm);
                    decimal tot_nonrefund_loan_avail = (PREV_YEAR_FROM_OWN_CONTRIBUTION + non_refundable_own_cont + PREV_YEAR_FROM_BANK_CONTRIBUTION + non_refundable_loans_bank_contn);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                       //+ ReportColFooter(0, "TOTAL NON REFUNDABLE LOAN AVAILED <span style='color:#eef8fd'>                                  </span>", Convert.ToString(TOTAL_pre_own_contribution))
                       + ReportColDataAlign(0, "TOTAL NON REFUNDABLE LOAN AVAILED <span style='color:#eef8fd'></span>", ReportColConvertToDecimal( Convert.ToString(tot_nonrefund_loan_avail)))

                    };

                    lst.Add(crm);

                    //decimal net_fund_epf = TOTAL_Intrest + total_balns - TOTAL_pre_own_contribution;
                    //net_fund_epf =Convert.ToDecimal(net_fund_epf - total_rep_loan_outstanding);
                    //decimal op_bal_inst_NRloan = 0.00M;
                    //if (!non_dt1.Rows[0].IsNull("op_bal_inst_NRloan"))
                    //{
                    //    op_bal_inst_NRloan = Convert.ToDecimal(non_dt1.Rows[0]["op_bal_inst_NRloan"]);
                    //    op_bal_inst_NRloan = Math.Round(op_bal_inst_NRloan, 2);
                    //}
                    //else
                    //{
                    //    op_bal_inst_NRloan = 0.00M;
                    //}
                    //string op_bal_inst_NRloan = Convert.ToString(op_bal_inst_NRloans);
                    
                    //crm = new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "F",
                    //    grpclmn = "<span style='color:#eef8fd'>^</span>"
                    //   + ReportColFooter(0, "NET AMOUNT IN EPF INCLUDING INTREST <span style='color:#eef8fd'>                                </span> ", Convert.ToString(net_fund_epf)) // pf return, bank retun, 

                    //};

                    //lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                       + ReportColDataAlign(0, "Intrest on Total Non Refundable Loan availed  <span style='color:#eef8fd'></span> ", ReportColConvertToDecimal( Convert.ToString(op_bal_inst_NRloan))) // pf return, bank retun, 

                    };

                    lst.Add(crm);
                    decimal tot_ded_B= PREV_YEAR_FROM_OWN_CONTRIBUTION + non_refundable_own_cont + PREV_YEAR_FROM_BANK_CONTRIBUTION + non_refundable_loans_bank_contn + Convert.ToDecimal(op_bal_inst_NRloan);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"

                      // + ReportColFooter(0, "NET AMOUNT IN EPF INCLUDING INTREST (A-B) <span style='color:#eef8fd'>                                </span> ", Convert.ToString(Interst_on_opbal + totbal + total_intaccru + total_int_op_bal-Convert.ToDecimal(non_dt1.Rows[0]["op_bal_inst_NRloan"]))) // pf return, bank retun, 

                      + ReportColDataAlign(0, "Total Deduction (B) <span style='color:#eef8fd'></span> ", ReportColConvertToDecimal( Convert.ToString(tot_ded_B)))//Convert.ToDecimal(non_dt1.Rows[0]["op_bal_inst_NRloan"]))) // pf return, bank retun, 

                    };

                    lst.Add(crm);
                    //b values
                    decimal NETAMOUNTINEPF = PREV_YEAR_FROM_OWN_CONTRIBUTION + non_refundable_own_cont + PREV_YEAR_FROM_BANK_CONTRIBUTION + non_refundable_loans_bank_contn + op_bal_inst_NRloan;
                    //Convert.ToString(EPFINCLUDING_INTREST )

                    //5 extra amount and minus 5 rs  340 Retired employee
                    decimal NET_AMOUNT_IN_EPF_INCLUDING_INTREST_A_B =  EPFINCLUDING_INTREST -NETAMOUNTINEPF;
                    if (empCode == "340")
                    {
                        NET_AMOUNT_IN_EPF_INCLUDING_INTREST_A_B = NET_AMOUNT_IN_EPF_INCLUDING_INTREST_A_B - 5;
                    }
                    else
                    {
                        NET_AMOUNT_IN_EPF_INCLUDING_INTREST_A_B = EPFINCLUDING_INTREST - NETAMOUNTINEPF;
                    }

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"

                      // + ReportColFooter(0, "NET AMOUNT IN EPF INCLUDING INTREST (A-B) <span style='color:#eef8fd'>                                </span> ", Convert.ToString(Interst_on_opbal + totbal + total_intaccru + total_int_op_bal-Convert.ToDecimal(non_dt1.Rows[0]["op_bal_inst_NRloan"]))) // pf return, bank retun, 

                       + ReportColDataAlign(0, "NET AMOUNT IN EPF INCLUDING INTREST (A-B) <span style='color:#eef8fd'> </span> ", ReportColConvertToDecimal( Convert.ToString( NET_AMOUNT_IN_EPF_INCLUDING_INTREST_A_B )))//Convert.ToDecimal(non_dt1.Rows[0]["op_bal_inst_NRloan"]))) // pf return, bank retun, 
                       
                    };

                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                   + ReportColDataAlign(0, "TOTAL REPAYABLE LOAN  OUTSTANDING  <span style='color:#eef8fd'></span> ", ReportColConvertToDecimal( Convert.ToString(total_rep_loan_outstanding)))

                    };

                    lst.Add(crm);
                    //FOR Default
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                 + ReportColFooter1(140, "For Default", "")

                    };

                    lst.Add(crm);

                    // date 
                    string Date = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                 + ReportColFooter(0, "DATE", Date)

                    };

                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                  + ReportColFooter1(140, "SECRETARY", "")

                    };

                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn = "<span style='color:#eef8fd'>^</span>"
                 + ReportColFooter1(0, "This memorandum is issued solely for information and it cannot be hypothecated <br /> or used in any way as a security to a " +
                 "third party. the balance brought fpeword <br /> should be checked with total of the previous year." +
                 "the amount credited for <br /> the year may also be checked and discrepancies if any noticed therein, should <br /> be brought to notice of the trustees. ", "")

                    };

                    lst.Add(crm);

                    //}
                }
            }

            if (empCode == "All")
            {
                string qr2 = "select distinct(ob.emp_code) from pr_ob_share ob " +
                    " join pr_emp_general pay on ob.emp_code = pay.emp_code join Employees e on e.EmpId = ob.emp_code " +
                    " JOIN pr_pf_open_bal_year pfbal on pfbal.emp_code = e.EmpId " +
                    " where ob.fm between DATEFROMPARTS (" + Fyear + ", 04, 01) and DATEFROMPARTS (" + Eyear + ", 03, 31 ) and pfbal.fy='"+ Fyear + "'  order by  ob.emp_code ; ";
                //DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS(" + Eyear + ", 03, 31 )
                DataTable dtqr2 = await _sha.Get_Table_FromQry(qr2);
                foreach (DataRow drqr2 in dtqr2.Rows)
                {
                    string emp = drqr2["emp_code"].ToString();
                    qry = "select  ob.emp_code,e.ShortName,pay.pf_no,pay.uan_no, REPLACE(RIGHT(CONVERT(VARCHAR(11), ob.fm, 106), 8), ' ', '-') as fm,own_share,bank_share ,vpf,own_share_total,bank_share_total,vpf_total ,pfbal.os_open AS ownbal ,pfbal.bs_open as bankbal, " +
                        "pfbal.vpf_open as vpfbal,pfbal.os_open_int,pfbal.bs_open_int,pfbal.vpf_open_int,active = 1 ,  pfbal.pf_return,pfbal.vpf_return,pfbal.bank_return, pfbal.os_cur_int,pfbal.vpf_cur_int,pfbal.bs_cur_int, active = 1   " +
                        "from pr_ob_share  ob join pr_emp_general pay on ob.emp_code=pay.emp_code join Employees e on e.EmpId=ob.emp_code " +
                        "JOIN pr_pf_open_bal_year pfbal on pfbal.emp_code=e.EmpId  " +
                        "where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and ob.emp_code in (" + emp + ") and pay.active = 1 and pfbal.fy='"+ Fyear + "' order by ob.fm asc ";

                    string prevQry = "select pf_return as own_share, vpf_return as vpf, bank_return as bank_share from pr_pf_open_bal_year " +
                "where emp_code in (" + emp + ") and fy = (select fy-1 from pr_month_details where active=1) ";

                    //string presQry = "select own_share as own_share, vpf as vpf,bank_share " +
                    //"as bank_share from pr_emp_pf_nonrepayable_loan where  year(fm) = year(getdate()) and emp_code in (" + emp + ") ;";
                    string presQry = "select own_share as own_share, vpf as vpf,bank_share " +
                  "as bank_share from pr_emp_pf_nonrepayable_loan where   emp_code in (" + emp + ") ;";

                    string non_qry1 = "select fm, own_share, bank_share, vpf from pr_emp_pf_nonrepayable_loan " +
                        "where year(fm) < year(getdate())  and emp_code in (" + emp + ") ";

                    string non_qry2 = "select fm, own_share, bank_share, vpf from pr_emp_pf_nonrepayable_loan " +
                        "where year(fm) = year(getdate())  and emp_code in (" + emp + ") ";
                    DataTable dtqr = await _sha.Get_Table_FromQry(qry);
                    DataTable non_dt2 = await _sha.Get_Table_FromQry(presQry);
                    DataTable non_dt1 = await _sha.Get_Table_FromQry(prevQry);

                    decimal? non_own_share = 0;
                    decimal? non_bank_share = 0;
                    decimal? non_vpf = 0;
                    decimal? non_own_share_prev = 0;
                    decimal? non_bank_share_prev = 0;
                    decimal? non_vpf_prev = 0;
                    if (non_dt2.Rows.Count > 0)
                    {
                        non_own_share = Convert.ToDecimal((non_dt2.Rows[0]["own_share"]));
                        non_bank_share = Convert.ToDecimal((non_dt2.Rows[0]["bank_share"]));
                        non_vpf = Convert.ToDecimal((non_dt2.Rows[0]["vpf"]));
                    }
                    if (non_dt1.Rows.Count > 0)
                    {
                        non_own_share_prev = Convert.ToDecimal((non_dt1.Rows[0]["own_share"]));
                        non_bank_share_prev = Convert.ToDecimal((non_dt1.Rows[0]["bank_share"]));
                        non_vpf_prev = Convert.ToDecimal((non_dt1.Rows[0]["vpf"]));
                    }
                    foreach (DataRow dr in dtqr.Rows)
                    {
                        try
                        {

                            count++;
                            empid = dr["emp_code"].ToString();
                            int count1 = dtqr.Select().Where(s => s["emp_code"].ToString() == empid).Count();
                            if (oldempid != empid)
                            {
                                string qry2 = " select l.emp_code,sum(a.principal_balance_amount) as Loan_Closing  " +
                                "from pr_emp_adv_loans_adjustments a join pr_emp_adv_loans l on l.id = a.emp_adv_loans_mid  join pr_loan_master lm on lm.id = l.loan_type_mid " +
                                "join pr_emp_adv_loans_child cl on l.id = cl.emp_adv_loans_mid  and cl.id = a.emp_adv_loans_child_mid   " +
                                
                                "where l.loan_type_mid in (16, 17, 18, 19, 20, 21,26,27) and " +//added 26,27 on 27/05/2020
                                //"and a.fm between DATEFROMPARTS(" + Fyear + ", 04, 01) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) " + //
                                //Commented above and added below line because the singlerateoutstanding loan closing balance should be equal to "total_rep_loan_outstanding" on 27/05/2020
                                //"a.fm=( select max(fm) from pr_emp_adv_loans_adjustments where fm not in (select max(fm) from pr_emp_adv_loans_adjustments)) " +
                                "a.fm=( select max(fm) from pr_emp_adv_loans_adjustments ) "+ //added on 30/05/2020
                                "and l.emp_code = " + empid + "  group by  l.emp_code  ";

                                DataTable dt2 = await _sha.Get_Table_FromQry(qry2);
                                if(dt2.Rows.Count > 0)
                                {
                                    // Added a loop to get the total number of Loan_Closing "total_rep_loan_outstanding" on 27/05/2020
                                    for (int trlo = 0; trlo < dt2.Rows.Count; trlo++)
                                    {
                                        total_rep_loan_outstanding += Convert.ToDecimal(dt2.Rows[trlo]["Loan_Closing"]);
                                        if (total_rep_loan_outstanding < 0)
                                        {
                                            total_rep_loan_outstanding = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    total_rep_loan_outstanding = 0.00M;
                                }
                                
                                //header
                                var grpdata = dr["emp_code"].ToString();


                                crm = new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "H",
                                    grpclmn = "<span style='color:#C8EAFB'>~</span>"
                                        + ReportColHeader(0, "EmpCode", dr["emp_code"].ToString())
                                        + ReportColHeader(17, "Emp Name", dr["ShortName"].ToString())
                                        + ReportColHeader(17, "PF NO", dr["pf_no"].ToString())
                                        + ReportColHeader(17, "Uan No", dr["uan_no"].ToString()),
                                    //column2 = "`",
                                    //column3 = "`",
                                    //column4 = "`",
                                    //column5 = "`",

                                };
                                lst.Add(crm);

                                //rows
                                // op bal
                                ownbal = Convert.ToDecimal(dr["ownbal"]);
                                bankbal = Convert.ToDecimal(dr["bankbal"]);
                                vpfbal = Convert.ToDecimal(dr["vpfbal"]);
                                total_opbal = ownbal + bankbal + vpfbal;
                                //PREV_YEAR_FROM_OWN_CONTRIBUTION = Convert.ToDecimal(dr["pf_return"]);
                                //PREV_YEAR_FROM_BANK_CONTRIBUTION = Convert.ToDecimal(dr["bank_return"]);
                                //PREV_YEAR_FROM_OWN_CONTRIBUTION = Convert.ToDecimal((non_dt1.Rows[0]["own_share"])); //Convert.ToDecimal(dr["pf_return"]);
                                //PREV_YEAR_FROM_OWN_CONTRIBUTION += Convert.ToDecimal((non_dt1.Rows[0]["vpf"]));
                                //PREV_YEAR_FROM_BANK_CONTRIBUTION = Convert.ToDecimal((non_dt1.Rows[0]["bank_share"]));
                                if (non_dt1.Rows.Count > 0)
                                {
                                    PREV_YEAR_FROM_OWN_CONTRIBUTION = Convert.ToDecimal(non_dt1.Rows[0]["own_share"]); //Convert.ToDecimal(dr["pf_return"]);
                                    PREV_YEAR_FROM_OWN_CONTRIBUTION += Convert.ToDecimal(non_dt1.Rows[0]["vpf"]);
                                    PREV_YEAR_FROM_BANK_CONTRIBUTION = Convert.ToDecimal(non_dt1.Rows[0]["bank_share"]);
                                }
                                else
                                {
                                    PREV_YEAR_FROM_OWN_CONTRIBUTION = 0.00M;//Convert.ToDecimal(dr["pf_return"]);
                                    PREV_YEAR_FROM_OWN_CONTRIBUTION += 0.00M;
                                    PREV_YEAR_FROM_BANK_CONTRIBUTION = 0.00M;
                                }
                                crm = new CommonReportModel
                                {
                                    RowId = RowCnt++,
                                    HRF = "R",
                                    // grpclmn 
                                    grpclmn = "OPBal",
                                    column2 =ReportColConvertToDecimal( dr["ownbal"].ToString()),
                                    column3 = ReportColConvertToDecimal(  dr["bankbal"].ToString()),
                                    column4 = ReportColConvertToDecimal( dr["vpfbal"].ToString()),
                                    column5 = ReportColConvertToDecimal( Convert.ToString(total_opbal)),

                                };

                                lst.Add(crm);
                            }

                            oldempid = dr["emp_code"].ToString();


                            decimal own_share = 0;
                            decimal bank_share = 0;
                            decimal vpf = 0;
                            // own bank vpf column wise total
                            if (!dr.IsNull("own_share"))
                            {
                                own_share = Convert.ToDecimal(dr["own_share"].ToString());
                            }
                            else
                            {
                                own_share = 0;
                            }
                            if (!dr.IsNull("bank_share"))
                            {
                                bank_share = Convert.ToDecimal(dr["bank_share"].ToString());
                            }
                            else
                            {
                                bank_share = 0;
                            }
                            if (!dr.IsNull("vpf"))
                            {
                                vpf = Convert.ToDecimal(dr["vpf"].ToString());
                            }
                            else
                            {
                                vpf = 0;
                            }
                            total = own_share + bank_share + vpf;
                            int DRTl = Convert.ToInt32(total.ToString());
                            decimal ALdRTtl = DRTl + 0.00M;
                            total = ALdRTtl;
                            //string id =Convert.ToString( total);

                            tot_amut += total;
                            int ALOwnshr = Convert.ToInt32(dr["own_share"].ToString());
                            decimal DrALownshr = ALOwnshr + 0.00M;
                            int ALBnkshr = Convert.ToInt32(dr["bank_share"].ToString());
                            decimal DrALBnkshr = ALBnkshr + 0.00M;
                            int ALVpf = Convert.ToInt32(dr["vpf"].ToString());
                            decimal DrALVpf = ALVpf + 0.00M;
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                // grpclmn 

                                grpclmn = dr["fm"].ToString(),
                                column2 =ReportColConvertToDecimal( DrALownshr.ToString()),

                                column3 = ReportColConvertToDecimal( DrALBnkshr.ToString()),
                                column4 = ReportColConvertToDecimal( DrALVpf.ToString()),
                                column5 = ReportColConvertToDecimal( Convert.ToString(total)),
                                //Convert.ToInt64(dr2["own_share"].ToString());
                            };

                            lst.Add(crm);

                            // own footer
                            try
                            {
                                //ownshare = Convert.ToInt64(dr["ownbal"].ToString()); //commented on 28/05/2020
                                ownshare = Convert.ToDecimal(dr["ownbal"].ToString()); //added on 28/05/2020

                            }
                            catch (Exception e)
                            {

                            }
                            try
                            {
                                //bankshare = Convert.ToInt64(dr["bankbal"].ToString()); //commented on 28/05/2020
                                bankshare = Convert.ToDecimal(dr["bankbal"].ToString()); //added on 28/05/2020

                            }
                            catch (Exception e)
                            {

                            }
                            // vpf footer
                            try
                            {

                                //vpfshare1 = Convert.ToInt64(dr["vpfbal"].ToString()); //commented on 28/05/2020
                                vpfshare1 = Convert.ToDecimal(dr["vpfbal"].ToString()); //added on 28/05/2020

                            }
                            catch (Exception e)
                            {

                            }

                            if (!dr.IsNull("own_share"))
                            {
                                //empid = dr["emp_code"].ToString();
                                if (oldempid != empid)
                                {
                                    ownshare1 = Convert.ToDecimal(dr["own_share"]);
                                }
                                ownshare1 = ownshare1 + Convert.ToDecimal(dr["own_share"]);
                            }
                            else
                            {
                                ownshare1 = 0;
                            }
                            // own total
                            ownsh_bal = ownshare + ownshare1;

                            // bank total
                            if (!dr.IsNull("bank_share"))
                            {
                                //empid = dr["emp_code"].ToString();
                                if (oldempid != empid)
                                {
                                    bankshare1 = Convert.ToDecimal(dr["bank_share"]);
                                }
                                bankshare1 = bankshare1 + Convert.ToDecimal(dr["bank_share"]);
                            }
                            else
                            {
                                bankshare1 = 0;
                            }

                            // bank total
                            bank_bal = bankshare + bankshare1;

                            // vpf total
                            if (!dr.IsNull("vpf"))
                            {

                                //empid = dr["emp_code"].ToString();
                                if (oldempid != empid)
                                {
                                    vpfshare = Convert.ToDecimal(dr["vpf"].ToString());

                                }
                                vpfshare = vpfshare + Convert.ToDecimal(dr["vpf"]);
                            }
                            else
                            {
                                vpfshare = 0;
                            }
                            //vpf total
                            vpf_bal = vpfshare + vpfshare1;
                            //decimal id= vpf_bal+ bank_bal + ownsh_bal;
                            try
                            {

                                // intrest op bal
                                //os_open_int = Convert.ToDecimal(dr["os_open_int"].ToString());
                                //bs_open_int = Convert.ToDecimal(dr["bs_open_int"].ToString());
                                //vpf_open_int = Convert.ToDecimal(dr["vpf_open_int"].ToString());
                                //total_int_op_bal = os_open_int + bs_open_int + vpf_open_int;
                                os_open_int = Convert.ToDecimal(dr["os_open_int"].ToString());
                                bs_open_int = Convert.ToDecimal(dr["bs_open_int"].ToString());
                                vpf_open_int = Convert.ToDecimal(dr["vpf_open_int"].ToString());
                                total_int_op_bal = os_open_int + bs_open_int+ vpf_open_int;
                            }
                            catch (Exception e)
                            {

                            }

                            try
                            {
                                // intrest Accrued bs_cur_int
                                if (!dr.IsNull("vpf_cur_int"))
                                {
                                    vpfintaccru = Convert.ToDecimal(dr["vpf_cur_int"]);

                                }
                                else
                                {
                                    vpfintaccru = 0;
                                }
                                if (!dr.IsNull("op_bal_inst_year"))//if (!dr.IsNull("os_cur_int"))
                                {
                                    //pfintaccru = Convert.ToDecimal(dr["op_bal_inst"]);
                                    pfintaccru = Convert.ToDecimal(dr["op_bal_inst_year"]);

                                }
                                else
                                {
                                    pfintaccru = 0;
                                }
                                if (!dr.IsNull("bs_cur_int"))
                                {
                                    bankintaccru = Convert.ToDecimal(dr["bs_cur_int"]);

                                }
                                else
                                {
                                    bankintaccru = 0;
                                }
                                //vpfintaccru = Convert.ToDecimal(dr["vpfintcurr"]);
                                //pfintaccru = Convert.ToDecimal(dr["pfintcurr"]);
                                //bankintaccru = Convert.ToDecimal(dr["bankintcurr"]);
                                //total_intaccru = pfintaccru + bankintaccru + vpfintaccru;
                                total_intaccru = pfintaccru+0.00M ;
                            }
                            catch (Exception e)
                            {

                            }
                            try
                            {
                                // TOTAL_Intrest
                                TOTAL_Intrest = Convert.ToDecimal(total_int_op_bal) + Convert.ToInt64(total_intaccru);
                            }
                            catch (Exception e)
                            {

                            }


                            try
                            {
                                foreach (DataRow dr1 in non_dt1.Rows)
                                {
                                    // pre year own contribution   //Bankreturn,VPFreturn,PFreturn
                                    if (!dr1.IsNull("bank_share"))
                                    {
                                        Bankreturn = Convert.ToDecimal((non_dt1.Rows[0]["bank_share"]));

                                    }
                                    else
                                    {
                                        Bankreturn = 0;
                                    }
                                    if (!dr1.IsNull("vpf"))
                                    {
                                        VPFreturn = Convert.ToDecimal((non_dt1.Rows[0]["vpf"]));

                                    }
                                    else
                                    {
                                        VPFreturn = 0;
                                    }
                                    if (!dr1.IsNull("own_share"))
                                    {
                                        PFreturn = Convert.ToDecimal((non_dt1.Rows[0]["own_share"]));

                                    }
                                    else
                                    {
                                        PFreturn = 0;
                                    }
                                }
                                //Bankreturn = Convert.ToDecimal(dr["Bankreturn"].ToString());
                                //VPFreturn = Convert.ToDecimal(dr["VPFreturn"].ToString());
                                //PFreturn = Convert.ToDecimal(dr["PFreturn"].ToString());
                                TOTAL_pre_own_contribution = Bankreturn + VPFreturn + PFreturn;
                            }
                            catch (Exception e)
                            {

                            }
                            // NON REFUNDABLE LOANS 
                            non_refundable_loans_own_contn = Convert.ToDecimal(non_own_share)+0.00M;
                            non_refundable_loans_bank_contn = Convert.ToDecimal(non_bank_share)+0.00M;
                            non_refundable_loans_vpf_contn = Convert.ToDecimal(non_vpf)+0.00M;
                            non_refundable_own_cont = non_refundable_loans_own_contn + non_refundable_loans_vpf_contn;
                            // total NON REFUNDABLE LOANS 
                            total_non_refundable_loan_availed = non_refundable_own_cont + non_refundable_loans_bank_contn;


                        }
                        catch (Exception e)
                        {

                        }
                    }
                    if (dtqr.Rows.Count > 0)
                    {
                        decimal total_balns = total_opbal + tot_amut;
                        if (ownshare != 0)
                        {
                            if (oldempid1 != empid)
                            {
                                crm = new CommonReportModel
                                {

                                    RowId = RowCnt++,
                                    HRF = "R",
                                    grpclmn = "Total",
                                    column2 =ReportColConvertToDecimal( ownsh_bal.ToString()),
                                    column3 = ReportColConvertToDecimal( bank_bal.ToString()),
                                    column4 = ReportColConvertToDecimal( vpf_bal.ToString()),
                                    column5 = ReportColConvertToDecimal( Convert.ToString(total_balns)),

                                };

                                lst.Add(crm);
                            }
                        }

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                            + ReportColDataAlign(0, "    Intrest Opening Balance <span style='color:#eef8fd'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>", ReportColConvertToDecimal( Convert.ToString(total_int_op_bal)))

                        };

                        lst.Add(crm);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                            + ReportColDataAlign(0, "    Intrest Acured <span style='color:#eef8fd'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>", ReportColConvertToDecimal( Convert.ToString(total_intaccru)))

                        };

                        lst.Add(crm);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                            + ReportColDataAlign(0, "    Total Intrest <span style='color:#eef8fd'>  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  </span>", ReportColConvertToDecimal( Convert.ToString(TOTAL_Intrest)))

                        };

                        lst.Add(crm);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                            + ReportColDataAlign(0, "    NON REFUNDABLE LOAN DETAILS <span style='color:#eef8fd'>           </span>", "-")

                        };

                        lst.Add(crm);
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                            + ReportColDataAlign(0, " AVAILED TILL PREV .YEAR FROM  OWN CONTRIBUTION   <span style='color:#eef8fd'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>", ReportColConvertToDecimal( Convert.ToString(PREV_YEAR_FROM_OWN_CONTRIBUTION)))

                        };

                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                           + ReportColDataAlign(0, "AVAILED  DURING THE YEAR FROM OWN CONTRIBUTION  <span style='color:#eef8fd'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>", ReportColConvertToDecimal( Convert.ToString(non_refundable_own_cont)))

                        };

                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                           + ReportColDataAlign(0, "AVAILED TILL PREV .YEAR FROM  BANK CONTRIBUTION <span style='color:#eef8fd'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>", ReportColConvertToDecimal( Convert.ToString(PREV_YEAR_FROM_BANK_CONTRIBUTION)))

                        };

                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                           + ReportColDataAlign(0, "AVAILED  DURING THE YEAR FROM BANK CONTRIBUTION <span style='color:#eef8fd'>&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;  </span> ", ReportColConvertToDecimal( Convert.ToString(non_refundable_loans_bank_contn)))

                        };

                        lst.Add(crm);
                        decimal tot_nonrefund_loan_avail= PREV_YEAR_FROM_OWN_CONTRIBUTION + non_refundable_own_cont + PREV_YEAR_FROM_BANK_CONTRIBUTION + non_refundable_loans_bank_contn;
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                           //+ ReportColFooter(0, "TOTAL NON REFUNDABLE LOAN AVAILED <span style='color:#eef8fd'>                                  </span>", Convert.ToString(TOTAL_pre_own_contribution))
                           + ReportColDataAlign(0, "TOTAL NON REFUNDABLE LOAN AVAILED <span style='color:#eef8fd'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>", ReportColConvertToDecimal( Convert.ToString(tot_nonrefund_loan_avail)))//newly added on 27/05/2020

                        };

                        lst.Add(crm);
                        decimal net_fund_epf = TOTAL_Intrest + total_balns - TOTAL_pre_own_contribution ;
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                           + ReportColDataAlign(0, "NET AMOUNT IN EPF INCLUDING INTREST <span style='color:#eef8fd' >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  </span> ", ReportColConvertToDecimal( Convert.ToString(net_fund_epf))) // pf return, bank retun, vpf return

                        };

                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                       + ReportColDataAlign(0, "TOTAL  REPAYABLE  LOAN  OUTSTANDING  <span style='color:#eef8fd'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span> ", ReportColConvertToDecimal( Convert.ToString(total_rep_loan_outstanding)))

                        };

                        lst.Add(crm);
                        //FOR Default
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                     + ReportColFooter1(140, "FOR Default", "")

                        };

                        lst.Add(crm);

                        // date 
                        string Date = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                     + ReportColFooter(0, "DATE", Date)

                        };

                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                      + ReportColFooter1(140, "SECRETARY", "")

                        };

                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "F",
                            grpclmn = "<span style='color:#eef8fd'>^</span>"
                     + ReportColFooter1(0, "This memorandum is issued solely for information and it cannot be hypothecated <br /> or used in any way as a security to a " +
                     "third party. the balance brought fpeword <br /> should be checked with total of the previous year." +
                     "the amount credited for <br /> the year may also be checked and discrepancies if any noticed therein, should <br /> be brought to notice of the trustees. ", "")

                        };

                        lst.Add(crm);

                    }
                }
            }

            return lst;

        }
        public string ReportColDataAlign(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable + ": " + "</span>" + "<span style='float:right'>" + value + "</span>";

           


            //sRet += "<span>" + lable + ": " + "</span>" + "<span style='float:right'>" + value + "<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
            //   "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+
            //   "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+
            //   "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>" + "</span>";

            return sRet;
        }
        public string ReportColdata(int spaceCount, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";

            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";
            sRet += "<span style='float:right'>" + value + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string ReportColFooter1(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable + " " + value + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }


        public async Task<IList<CommonReportModel>> PfFinalSettlementData(string empCode, string fy)
        {
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int RowCnt = 0;
            string fystart = "";

            if (empCode == "" || empCode == "^1")
            {
                empCode = "0";
            }
            if (fy == "^2")
            {
                fy = "1900-01-01";
            }
            double diffdates = 0;
            if (fy != "^2" && empCode != "0")
            {
                fy = Convert.ToDateTime(fy).ToString("yyyy-MM-dd");
                string strDate = fy;
                string[] sa = strDate.Split('-');
                string s1 = sa[0];
                if (s1.StartsWith("0"))
                {
                    s1 = s1.Substring(1);
                }

                string s2 = sa[1];
                string s3 = sa[2];
                fystart = "01" + "-" + "04" + "-" + s1;
                diffdates = (Convert.ToDateTime(fy) - Convert.ToDateTime(fystart)).TotalDays;
            }

            if (empCode != "All" && empCode != "0")
            {

                string query = " select top 1 A.emp_code,emp.ShortName,e.designation,e.pf_no,e.uan_no," +
                    "(ISNULL(A.os_open,0)+ISNULL(A.bs_open,0)+ISNULL(A.vpf_open,0)) as PFBAL , " +
                    "(ISNULL(A.os_open_int, 0) + ISNULL(A.bs_open_int, 0) + ISNULL(A.vpf_open_int, 0)) as interestbal," +
                    " (ISNULL(A.pf_return, 0) + ISNULL(A.bank_return, 0) + ISNULL(A.vpf_return, 0)) as NrLoanOP " +
                    "from pr_pf_open_bal_year A join pr_emp_general e on e.emp_code=A.emp_code " +
                    "join employees emp on emp.empid=e.emp_code where a.emp_code =  " + empCode + " and a.fy <= year(getdate()) order by a.fy desc";
                //   string non_qry2 = "  select emp_code,own_share_intrst_total+bank_share_intrst_total+vpf_intrst_total as interestbal from pr_ob_share where  active=1  and emp_code = " + empCode + " and fm<=Convert(varchar,'" + fy + "',105)";
                DataTable dt = await _sha.Get_Table_FromQry(query);
                //  DataTable non_dt = await _sha.Get_Table_FromQry(non_qry2);
                decimal? pfbalance = 0;

                decimal? pfinterestbal = 0;
                decimal? NRloansopng = 0;

                if (dt.Rows.Count > 0)
                {
                    pfbalance = Convert.ToDecimal((dt.Rows[0]["PFBAL"]));
                    pfinterestbal = Convert.ToDecimal((dt.Rows[0]["interestbal"]));
                    NRloansopng = Convert.ToDecimal((dt.Rows[0]["NrLoanOP"]));
                }
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            column1 = "<span style='color:#C8EAFB'>~</span>"
                                               + ReportColHeader(0, "Emp Code", dr["emp_code"].ToString())
                                               + ReportColHeader(17, "Emp Name", dr["ShortName"].ToString())
                                               + ReportColHeader(17, "PF NO", dr["pf_no"].ToString())
                                               + ReportColHeader(17, "Uan No", dr["uan_no"].ToString())

                        };
                    }
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "PF Balance AS ON :  " + "   " + "    " + Convert.ToDateTime(fy).ToString("dd/MM/yyyy"),
                        column2 =ReportColConvertToDecimal( pfbalance.ToString()),

                    };

                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "INTEREST @8.55% FROM  " + "   " + fystart + " - " + Convert.ToDateTime(fy).ToString("dd/MM/yyyy"),

                        //calculation (pfbalance * 8.55/100)/365 * no of days

                        column2 = ReportColConvertToDecimal( Math.Round(((Convert.ToDouble(pfbalance) * (8.55 / 100)) / 365 * diffdates), 2).ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "INTEREST BALANCE AS ON :" + "   " + fystart,
                        column2 =ReportColConvertToDecimal( pfinterestbal.ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "INTEREST @8.55% FROM  " + "   " + fystart + " - " + Convert.ToDateTime(fy).ToString("dd/MM/yyyy"),
                        //calculation (pfinterestbal * 8.55/100)/365 * no of days
                        column2 = ReportColConvertToDecimal( Math.Round(((Convert.ToDouble(pfinterestbal) * (8.55 / 100)) / 365 * diffdates), 2).ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "<span style = 'color:#008000' >Total </ span >",
                        column2 = ReportColConvertToDecimal( Math.Round(((Convert.ToDecimal(pfbalance) + Convert.ToDecimal(((Convert.ToDouble(pfbalance) * (8.55 / 100)) / 365 * diffdates)) + Convert.ToDecimal(pfinterestbal) + Convert.ToDecimal((Convert.ToDouble(pfinterestbal) * (8.55 / 100)) / 365 * diffdates))), 2).ToString()),

                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "Less :",

                        column2 = "",

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "NR WITHDRAWALS(O.B)",

                        column2 = ReportColConvertToDecimal( NRloansopng.ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "INT.ON WITHDRAWALS",

                        //calculation (NRloansopng * 8.55/100)/365 * no of days
                        column2 = ReportColConvertToDecimal( Math.Round(((Convert.ToDouble(NRloansopng) * (8.55 / 100)) / 365 * diffdates), 2).ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "TOTAL DEDUCTIONS",

                        column2 = ReportColConvertToDecimal( Math.Round((Convert.ToDecimal(NRloansopng) + Convert.ToDecimal((Convert.ToDouble(NRloansopng) * (8.55 / 100)) / 365 * diffdates)), 2).ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",
                        column1 = "NET AMOUNT PAYABLE",
                        column2 = ReportColConvertToDecimal( Math.Round((((Convert.ToDecimal(pfbalance) + Convert.ToDecimal(((Convert.ToDouble(pfbalance) * (8.55 / 100)) / 365 * diffdates)) + Convert.ToDecimal(pfinterestbal) + Convert.ToDecimal((Convert.ToDouble(pfinterestbal) * (8.55 / 100)) / 365 * diffdates))) - ((Convert.ToDecimal(NRloansopng) + Convert.ToDecimal((Convert.ToDouble(NRloansopng) * (8.55 / 100)) / 365 * diffdates)))), 2).ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "PRODUCT-WISE :",

                        column2 = "",

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "PRODUCTS ON PRINCIPAL",

                        column2 = "0.00",

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "PRODUCTS ON INTEREST",

                        column2 = "0.00",

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "LESS:PRODUCTS ON LOANS",

                        column2 = "0.00",

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "NET PRODUCTS ",

                        column2 = "0.00",

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "NET INTEREST ACCRUED ",

                        column2 = "0.00",

                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "<span style = 'color:#008000' >TOTAL AMOUNT PAYABLE AS UNDER  </ span >",

                        column2 = "",

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "PRINCIPAL ",
                        column2 =ReportColConvertToDecimal( pfbalance.ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "INTEREST OP.BALANCE",
                        column2 = ReportColConvertToDecimal( pfinterestbal.ToString()),

                    };
                    lst.Add(crm);
                    decimal intaccr = Convert.ToDecimal(((((Convert.ToDecimal(pfbalance) + Convert.ToDecimal(((Convert.ToDouble(pfbalance) * (8.55 / 100)) / 365 * diffdates)) + Convert.ToDecimal(pfinterestbal) + Convert.ToDecimal((Convert.ToDouble(pfinterestbal) * (8.55 / 100)) / 365 * diffdates))) - ((Convert.ToDecimal(NRloansopng) + Convert.ToDecimal((Convert.ToDouble(NRloansopng) * (8.55 / 100)) / 365 * diffdates))))));

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = " NET INTEREST ACCRUED",
                        column2 = ReportColConvertToDecimal( Math.Round(Convert.ToDecimal(Convert.ToDecimal(intaccr) * Convert.ToDecimal((((8.55 / 100) * diffdates)) / 365)), 2).ToString()),
                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "LESS LOANS AVAILED ",
                        column2 =ReportColConvertToDecimal( NRloansopng.ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "F",

                        column1 = "<span style = 'color:#008000' >AMOUNT PAYABLE</ span >",
                        column2 = ReportColConvertToDecimal( Math.Round((((Convert.ToDecimal(pfbalance) + Convert.ToDecimal(((Convert.ToDouble(pfbalance) * (8.55 / 100)) / 365 * diffdates)) + Convert.ToDecimal(pfinterestbal) + Convert.ToDecimal((Convert.ToDouble(pfinterestbal) * (8.55 / 100)) / 365 * diffdates))) - ((Convert.ToDecimal(NRloansopng) + Convert.ToDecimal((Convert.ToDouble(NRloansopng) * (8.55 / 100)) / 365 * diffdates)))), 2).ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "AMOUNT SETTLED ON",
                        column2 = "0.00",

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "<span style = 'color:#008000' >NET AMOUNT PAYABLE </ span >",

                        column2 = ReportColConvertToDecimal( Math.Round(Convert.ToDecimal(Convert.ToDecimal(intaccr) * Convert.ToDecimal((((8.55 / 100) * diffdates)) / 365)), 2).ToString()),

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "MANAGER",
                        column2 = "",

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        column1 = "",
                        column2 = "SECRETARY",

                    };
                    lst.Add(crm);
                }

                //lst1.Add(lst);

                //}
            }

            if (empCode == "All")
            {
                string non_qry2 = "  select emp_code from pr_ob_share where  active=1 and fm<=Convert(varchar,'" + fy + "',105)";
                DataTable dtqr2 = await _sha.Get_Table_FromQry(non_qry2);

                foreach (DataRow drs in dtqr2.Rows)
                {
                    // string emp = drqr2["emp_code"].ToString();
                    string emp_code = drs["emp_code"].ToString();

                    string query = " select top 1 A.emp_code,emp.ShortName,e.designation,e.pf_no,e.uan_no, " +
                        " (ISNULL(A.os_open,0)+ISNULL(A.bs_open,0)+ISNULL(A.vpf_open,0)) as PFBAL , " +
                        "(ISNULL(A.os_open_int, 0) + ISNULL(A.bs_open_int, 0) + ISNULL(A.vpf_open_int, 0)) as interestbal, " +
                        " (ISNULL(A.pf_return, 0) + ISNULL(A.bank_return, 0) + ISNULL(A.vpf_return, 0)) as NrLoanOP " +
                        "  from pr_pf_open_bal_year A join pr_emp_general e on e.emp_code=A.emp_code " +
                        "join employees emp on emp.empid=e.emp_code where a.emp_code =  " + emp_code + " and a.fy <= year(getdate()) order by a.fy desc";
                    //   string non_qry2 = "  select emp_code,own_share_intrst_total+bank_share_intrst_total+vpf_intrst_total as interestbal from pr_ob_share where  active=1  and emp_code = " + empCode + " and fm<=Convert(varchar,'" + fy + "',105)";
                    DataTable dt = await _sha.Get_Table_FromQry(query);
                    //  DataTable non_dt = await _sha.Get_Table_FromQry(non_qry2);

                    foreach (DataRow dr in dt.Rows)
                    {
                        decimal? pfbalance = 0;

                        decimal? pfinterestbal = 0;
                        decimal? NRloansopng = 0;



                        pfbalance = Convert.ToDecimal(dr["PFBAL"]);
                        pfinterestbal = Convert.ToDecimal(dr["interestbal"]);
                        NRloansopng = Convert.ToDecimal(dr["NrLoanOP"]);

                        if (dt.Rows.Count > 0)
                        {


                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "H",
                                column1 = "<span style='color:#C8EAFB'>~</span>"
                                                   + ReportColHeader(0, "EmpCode", dt.Rows[0]["emp_code"].ToString())
                                                   + ReportColHeader(17, "Emp Name", dt.Rows[0]["ShortName"].ToString())
                                                   + ReportColHeader(17, "PF NO", dt.Rows[0]["pf_no"].ToString())
                                                   + ReportColHeader(17, "Uan No", dt.Rows[0]["uan_no"].ToString())

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "PF Balance AS ON :  " + "   " + "    " + fy,
                                column2 = ReportColConvertToDecimal( pfbalance.ToString()),

                            };

                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "INTEREST @8.55% FROM  " + "   " + fystart + " - " + fy,

                                //calculation (pfbalance * 8.55/100)/365 * no of days

                                column2 = ReportColConvertToDecimal( Math.Round(((Convert.ToDouble(pfbalance) * (8.55 / 100)) / 365 * diffdates), 2).ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "INTEREST BALANCE AS ON :" + "   " + fystart,
                                column2 = ReportColConvertToDecimal( pfinterestbal.ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "INTEREST @8.55% FROM  " + "   " + fystart + " - " + fy,
                                //calculation (pfinterestbal * 8.55/100)/365 * no of days
                                column2 = ReportColConvertToDecimal( Math.Round(((Convert.ToDouble(pfinterestbal) * (8.55 / 100)) / 365 * diffdates), 2).ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",
                                column1 = "<span style = 'color:#008000' >Total </ span >",
                                column2 = ReportColConvertToDecimal( Math.Round(((Convert.ToDecimal(pfbalance) + Convert.ToDecimal(((Convert.ToDouble(pfbalance) * (8.55 / 100)) / 365 * diffdates)) + Convert.ToDecimal(pfinterestbal) + Convert.ToDecimal((Convert.ToDouble(pfinterestbal) * (8.55 / 100)) / 365 * diffdates))), 2).ToString()),

                            };
                            lst.Add(crm);

                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "Less :",

                                column2 = "",

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "NR WITHDRAWALS(O.B)",

                                column2 = ReportColConvertToDecimal( NRloansopng.ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "INT.ON WITHDRAWALS",

                                //calculation (NRloansopng * 8.55/100)/365 * no of days
                                column2 = ReportColConvertToDecimal( Math.Round(((Convert.ToDouble(NRloansopng) * (8.55 / 100)) / 365 * diffdates), 2).ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "TOTAL DEDUCTIONS",

                                column2 = ReportColConvertToDecimal( Math.Round((Convert.ToDecimal(NRloansopng) + Convert.ToDecimal((Convert.ToDouble(NRloansopng) * (8.55 / 100)) / 365 * diffdates)), 2).ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",
                                column1 = "NET AMOUNT PAYABLE",
                                column2 = ReportColConvertToDecimal( Math.Round((((Convert.ToDecimal(pfbalance) + Convert.ToDecimal(((Convert.ToDouble(pfbalance) * (8.55 / 100)) / 365 * diffdates)) + Convert.ToDecimal(pfinterestbal) + Convert.ToDecimal((Convert.ToDouble(pfinterestbal) * (8.55 / 100)) / 365 * diffdates))) - ((Convert.ToDecimal(NRloansopng) + Convert.ToDecimal((Convert.ToDouble(NRloansopng) * (8.55 / 100)) / 365 * diffdates)))), 2).ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "PRODUCT-WISE :",

                                column2 = "",

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "PRODUCTS ON PRINCIPAL",

                                column2 = "0.00",

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "PRODUCTS ON INTEREST",

                                column2 = "0.00",

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "LESS:PRODUCTS ON LOANS",

                                column2 = "0.00",

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "NET PRODUCTS ",

                                column2 = "0.00",

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "NET INTEREST ACCRUED ",

                                column2 = "0.00",

                            };
                            lst.Add(crm);

                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "<span style = 'color:#008000' >TOTAL AMOUNT PAYABLE AS UNDER  </ span >",

                                column2 = "",

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "PRINCIPAL ",
                                column2 = ReportColConvertToDecimal( pfbalance.ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "INTEREST OP.BALANCE",
                                column2 = ReportColConvertToDecimal( pfinterestbal.ToString()),

                            };
                            lst.Add(crm);
                            decimal intaccr = Convert.ToDecimal(((((Convert.ToDecimal(pfbalance) + Convert.ToDecimal(((Convert.ToDouble(pfbalance) * (8.55 / 100)) / 365 * diffdates)) + Convert.ToDecimal(pfinterestbal) + Convert.ToDecimal((Convert.ToDouble(pfinterestbal) * (8.55 / 100)) / 365 * diffdates))) - ((Convert.ToDecimal(NRloansopng) + Convert.ToDecimal((Convert.ToDouble(NRloansopng) * (8.55 / 100)) / 365 * diffdates))))));

                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = " NET INTEREST ACCRUED",
                                column2 = ReportColConvertToDecimal(Math.Round(Convert.ToDecimal(Convert.ToDecimal(intaccr) * Convert.ToDecimal((((8.55 / 100) * diffdates)) / 365)), 2).ToString()),
                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "LESS LOANS AVAILED ",
                                column2 = ReportColConvertToDecimal( NRloansopng.ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "F",

                                column1 = "<span style = 'color:#008000' >AMOUNT PAYABLE</ span >",
                                column2 = ReportColConvertToDecimal( Math.Round((((Convert.ToDecimal(pfbalance) + Convert.ToDecimal(((Convert.ToDouble(pfbalance) * (8.55 / 100)) / 365 * diffdates)) + Convert.ToDecimal(pfinterestbal) + Convert.ToDecimal((Convert.ToDouble(pfinterestbal) * (8.55 / 100)) / 365 * diffdates))) - ((Convert.ToDecimal(NRloansopng) + Convert.ToDecimal((Convert.ToDouble(NRloansopng) * (8.55 / 100)) / 365 * diffdates)))), 2).ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "AMOUNT SETTLED ON",
                                column2 = "0.00",

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "<span style = 'color:#008000' >NET AMOUNT PAYABLE </ span >",

                                column2 = ReportColConvertToDecimal( Math.Round(Convert.ToDecimal(Convert.ToDecimal(intaccr) * Convert.ToDecimal((((8.55 / 100) * diffdates)) / 365)), 2).ToString()),

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "MANAGER",
                                column2 = "",

                            };
                            lst.Add(crm);
                            crm = new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "R",
                                column1 = "",
                                column2 = "SECRETARY",

                            };
                            lst.Add(crm);
                        }

                    }
                }

            }


            return lst;
        }
    }
}