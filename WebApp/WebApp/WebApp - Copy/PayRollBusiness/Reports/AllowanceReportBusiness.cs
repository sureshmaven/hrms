using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PayRollBusiness.Reports
{
    public class AllowanceReportBusiness : BusinessBase
    {
        public AllowanceReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public async Task<IList<AllowanceTypes>> AllowanceTypes()
        {
            string qrySel = "select id,name,description,amount from pr_branch_allowance_master where active=1 ";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);
            IList<AllowanceTypes> lstDept = new List<AllowanceTypes>();
          
            try
            {
                lstDept.Insert(0, new AllowanceTypes
                {
                    Id = "0",
                    Type = "All"
                });
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new AllowanceTypes
                    {
                        Id = dr["id"].ToString(),
                        Type = dr["description"].ToString(),

                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }
        IList<CommonReportModel> Allowdt = new List<CommonReportModel>();
        CommonReportModel allw = new CommonReportModel();
        int RowCnt = 0;
        public async Task<IList<CommonReportModel>> GetAllowencedata(string empid, string allowanceTypes, string mnth)
        {
           
            if (empid.Contains("^"))
            {
                empid = "0";
                allowanceTypes = "0";
                mnth = "01-01-01";
            }
            DateTime n_fromdate;
            DateTime to_date;
            float no_of_days = 0;
            int SlNo = 1;
            DateTime dtFM = Convert.ToDateTime(mnth);
            string str1 = dtFM.ToString("yyyy-MM-dd");
            string n_empcode = empid;
            string newemp = "";
            string oldemp = "";

            //string qry = "SELECT c.emp_code , e.ShortName,format(c.from_date,'yyyy-MM-dd') as from_date ,format(c.to_date,'yyyy-MM-dd') as to_date,m.description,m.amount FROM pr_emp_branch_allowances c JOIN pr_branch_allowance_master m ON c.allowance_mid=m.id JOIN Employees e ON c.emp_code= e.EmpId WHERE  c.fy  IS  NOt NULL and c.fm  IS  NOt NULL AND month(c.from_date) =" + dtFM.Month + ""; //todo - add fromdate <=month_details.month
            string qry = "SELECT c.emp_code , e.ShortName,format(c.from_date,'dd-MM-yyyy') as from_date,format(c.to_date, 'dd-MM-yyyy') as to_date,m.description,m.amount" +
                "  FROM pr_emp_branch_allowances c JOIN pr_branch_allowance_master m ON c.allowance_mid = m.id JOIN Employees e ON c.emp_code = e.EmpId" +
                " WHERE  '"+str1+"' between from_date and case when to_date is not null " +
                "then to_date else (select max(to_date) from pr_emp_branch_allowances) end and e.RetirementDate>='" + str1 + "'"; //todo - add fromdate <=month_details.month
            if (empid != "All")
            {
                qry += " AND c.emp_code in (" + empid + ")";
            }

            if (allowanceTypes != "null" && allowanceTypes != "0")
            {
                qry += " AND c.allowance_mid in (" + allowanceTypes + ")";
            }
            DataTable dt = await _sha.Get_Table_FromQry(qry);

            var dtEmpbranch_allowance = dt;

            foreach (DataRow dr in dtEmpbranch_allowance.Rows)
            {
                newemp=dr["emp_code"].ToString();
                //fromdate
                n_fromdate = Convert.ToDateTime(dr["from_date"]);
                //check if from date is not eq to FM
                if (n_fromdate.Month < dtFM.Month)
                {
                    //01-06-2019
                    n_fromdate = new DateTime(dtFM.Year, dtFM.Month, 1);
                }

                if (dr["to_date"] is DBNull)
                {
                    //get last day of the month of n_fromdate 31-06-2019
                    to_date = new DateTime(n_fromdate.Year, n_fromdate.Month, 1).AddMonths(1).AddDays(-1);
                }
                else
                {

                    to_date = Convert.ToDateTime(dr["to_date"]);
                    //check if todate is not eq to fm
                    if (to_date.Month > dtFM.Month)
                    {
                        //31-06-2019
                        to_date = new DateTime(n_fromdate.Year, n_fromdate.Month, 1).AddMonths(1).AddDays(-1);
                    }
                }
                //difference b/w dates
                no_of_days = Convert.ToInt32((to_date - n_fromdate).TotalDays + 1);

                var bralw_amount = float.Parse(dr["amount"].ToString());
                var totdaysinMn = new DateTime(dtFM.Year, dtFM.Month, 1).AddMonths(1).AddDays(-1).Day;

                float oneDayAmt = bralw_amount / totdaysinMn;
                float balw_amount = no_of_days * oneDayAmt;

                if (newemp != oldemp || oldemp == "")
                {
                    allw = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>~</span>"
                            + ReportColHeader(0, "Emp Code", dr["emp_code"].ToString())
                            + ReportColHeader(20, "Emp Name", dr["ShortName"].ToString()),
                        column1 = "`",
                        column2 = "`",
                        column3 = "`",

                    };
                    Allowdt.Add(allw);
                }
                if (newemp != oldemp)
                {
                    allw = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        grpclmn = dr["description"].ToString(),
                        column1 = dr["from_date"].ToString(),
                        column2 = dr["to_date"].ToString(),
                        column3 = ReportColConvertToDecimal(Math.Round(balw_amount).ToString()),

                    };
                    Allowdt.Add(allw);
                }
              
                oldemp = dr["emp_code"].ToString();
            }
            
            return Allowdt;
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
