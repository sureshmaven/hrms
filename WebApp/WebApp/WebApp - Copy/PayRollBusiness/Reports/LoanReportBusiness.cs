using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.DAL.Business;
using PayrollModels;
using PayrollModels.Masters;
using Newtonsoft.Json;
using System.Data;

namespace PayRollBusiness.Reports
{
   public class LoanReportBusiness:BusinessBase
    {
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        public LoanReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }


        int RowCnt = 0;
        int SNo = 1;
        string codes = "";
        string oldempid = "";
        string empid = "";
        public async Task<DataTable> GetLoanReports(string empCode)
        {
            string qry = "";
            //var empid = "0";
            string empcodes = empCode;
            //string[] empids = empcodes.Split(',');
            //foreach (var id in empids)
            //{
                //empid = id;

                if (empCode.Contains("^"))
                {
                empcodes = "0";

                }
                 qry = "select lm.loan_description,al.total_amount,al.installment_amount,(al.completed_installment+al.remaining_installment) as period, " +
                    "al.completed_installment as INSTPAID,al.remaining_installment as BALANCEINST from pr_emp_adv_loans al " +
                    "inner join pr_loan_master lm on lm.id = al.loan_type_mid ";

                if (empCode != "All")
                {
                    qry += " where al.emp_code in (" + empcodes + ") ";
                   
                }


            //}
            return await _sha.Get_Table_FromQry(qry);

        }

        public async Task<IList<CommonReportModel>> GetLoanGroupingReports(string empCode)
        {
            string qry = "";
            //var empid = "0";
            string empcodes = empCode;
            //string[] empids = empcodes.Split(',');
            //foreach (var id in empids)
            //{
            //empid = id;

            if (empCode.Contains("^"))
            {
                empcodes = "0";

            }
            qry = "Select distinct  w.EmpId ,w.Shortname ,d.Code as Desig,case when b.Name = 'OtherBranch' then dept.Name when b.Name = 'HeadOffice' " +
                "then dept.Name else b.Name end as Branch , lm.loan_description,al.total_amount,al.installment_amount, " +
                "(al.completed_installment + al.remaining_installment) as period, al.completed_installment as INSTPAID, " +
                "al.remaining_installment as BALANCEINST from pr_emp_adv_loans al join Employees w on w.empid = al.emp_code and al.active=1 " +
                "join branches b on b.id = w.branch join departments dept on dept.id = w.department " +
                "join designations d on d.id = w.currentdesignation inner join pr_loan_master lm on lm.id = al.loan_type_mid";

            if (empCode != "All")
            {
                qry += " where al.emp_code in (" + empcodes + ") and w.retirementdate>=fm  order by w.EmpId ";

            }

            else if(empCode=="All")
            {
                qry += " and w.retirementdate>=fm order by w.EmpId ";
            }

            DataTable dt=await _sha.Get_Table_FromQry(qry);
            foreach(DataRow dr in dt.Rows)
            {
           
                empid = dr["EmpId"].ToString();
                if (oldempid!= empid)
                {
                    var grpdata = dr["EmpId"].ToString();
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>~</span>"
                        + ReportColHeader(0, "Emp Code",dr["EmpId"].ToString()+",")
                        + ReportColHeader(5, "Emp Name",dr["Shortname"].ToString()+",")
                        + ReportColHeader(5, "Designation", dr["Desig"].ToString() + ",")
                        + ReportColHeader(5, "Branch/Department", dr["Branch"].ToString() + ","),
                        column2 = "`",
                        column3 = "`",
                        column4 = "`",
                        column5 = "`",
                        column6 = "`",

                    };
                    lst.Add(crm);
                }

                oldempid= dr["EmpId"].ToString();

                crm = new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "R",
                   // grpclmn = SNo++.ToString(),
                    grpclmn = dr["loan_description"].ToString(),
                    column2 = ReportColConvertToDecimal(dr["total_amount"].ToString()),
                    column3 = ReportColConvertToDecimal(dr["installment_amount"].ToString()),
                    column4 = dr["period"].ToString(),
                    column5 = dr["INSTPAID"].ToString(),
                    column6 = dr["BALANCEINST"].ToString(),

                };
                lst.Add(crm);
            }
            return lst;

        }

        private string ReportColHeader(int spaceCount, string lable, string value1,string value2,string value3,string value4)
        {
            string sRet = "<span style='color:#C8EAFB'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "";
            sRet += "</span>";

            sRet += "<span>" + lable + " <b>" + value1 + "," + value2 + "," + value3 + "," + value4 + "</b></span>";

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
