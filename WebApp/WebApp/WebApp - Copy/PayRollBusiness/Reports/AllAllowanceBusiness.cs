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
   public class AllAllowanceBusiness : BusinessBase
    {
        public AllAllowanceBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        string oldempid = "";
        string oldAllType = "";
        int RowCnt = 0;
        int SlNo = 1;
        SqlHelperAsync _sha = new SqlHelperAsync();
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        IList<AllowanceTypes> lstDept = new List<AllowanceTypes>();
        #region All Allowance Emp wise
        public async Task<IList<AllowanceTypes>> getAllowanceTypes()
        {
            //string qry = " SELECT id , name  from pr_branch_allowance_master WHERE active = 1 " +
            //    "UNION ALL SELECT id ,name  from pr_allowance_field_master WHERE active = 1";
            string qry = " SELECT id, name from (SELECT id , description as name from pr_branch_allowance_master WHERE active = 1 " +
                " and description not in (SELECT name  from pr_allowance_field_master WHERE active = 1) " +
                " UNION ALL " +
                " SELECT id, name  from pr_allowance_field_master WHERE active = 1) as a order by name;";
            DataTable dt = await _sha.Get_Table_FromQry(qry);
            
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
                        Name= dr["name"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept; 

        }
        public async Task<IList<CommonReportModel>> AllAllowanceData(string emp_code, string types, string mnth)
        {
            string qry = "";

            //emp_code = "0";
            if (emp_code.Contains("^"))
            {
                emp_code = "0";
                types = "0";
                mnth = "01-01-01";
            }
            string Type = types;

            string empid = emp_code;
            string cond1 = "";
            string cond2 = "";
            string res2 = "";
            string mnthqry = "";

            DateTime str = Convert.ToDateTime(mnth);
            string str1 = str.ToString("yyyy-MM-dd");
            string[] sa = str1.Split('-');
            string s1 = sa[0];
            string s2 = sa[1];

            
            StringBuilder ATypes = new StringBuilder();

             mnthqry = "select fm from pr_month_details where active=1 ";
            DataTable dtmoth = await _sha.Get_Table_FromQry(mnthqry);

            DateTime mnthdet = Convert.ToDateTime(dtmoth.Rows[0]["fm"].ToString());
            string monthstr = mnthdet.ToString("yyyy-MM-dd");
            string[] mntharr = monthstr.Split('-');
            string mntharr1 = mntharr[0];
            string mntharr2 = mntharr[1];





            //ATypes = res2;
            if (types == "0" || types != "0" && types != null && types != "" && types != " All ")
            {
                string[] Types = Type.Split(',');

                foreach (string word in Types)
                {
                    string res1 = word.TrimStart();
                    res2 = res1.TrimEnd();
                    ATypes.Append("'");
                    ATypes.Append(res2);
                    ATypes.Append("', ");
                }
                //types = ATypes.Remove(" ");
                types = ATypes.ToString(0, ATypes.Length - 2);


            }

            if (s1 == mntharr1 && s2 == mntharr2)
            {

                if (types != " All " && emp_code == "All")
                {
                    cond1 = "AND pr_branch_allowance_master.name in (" + types + ") ";
                    cond2 = "AND pr_allowance_field_master.name in (" + types + ")  ";
                }
                if (emp_code != "All" && types == " All ")
                {
                    cond1 = "AND emp_code in (" + empid + ") ";
                    cond2 = "AND emp_code in (" + empid + ") ";
                }
                if (emp_code != "All" && types != " All ")
                {
                    cond1 = "AND pr_branch_allowance_master.name in (" + types + ") AND emp_code in (" + empid + ") ";
                    cond2 = "AND pr_allowance_field_master.name in (" + types + ") AND emp_code in (" + empid + ")";
                }

                qry = "select  e.empId,e.Shortname , des.Name as Desig,pr_branch_allowance_master.name as Allowance,amount,benefit ,pr_branch_allowance_master.id as allid " +
                    "from pr_emp_branch_allowances  join employees e on e.EmpId = pr_emp_branch_allowances.emp_code join designations des on des.id = e.currentdesignation  " +
                    "join pr_branch_allowance_master on pr_branch_allowance_master.id = pr_emp_branch_allowances.allowance_mid " +
                    "where pr_emp_branch_allowances.active=1 AND pr_branch_allowance_master.active=1 AND month(pr_emp_branch_allowances.fm)= '" + s2 + "' AND YEAR(pr_emp_branch_allowances.fm)= '" + s1 + "'  AND e.RetirementDate >=GETDATE() AND pr_branch_allowance_master.amount >0 " + cond1 +
                    "UNION ALL " +
                    "select e.empId,  e.Shortname,des.Name as Desig ,pr_allowance_field_master.name AS Allowance,amount,benefit ,pr_allowance_field_master.id as allid " +
                    "from pr_emp_allowances_gen join employees e on e.EmpId = pr_emp_allowances_gen.emp_code  join designations des on des.id = e.currentdesignation   " +
                    "join pr_allowance_field_master on pr_allowance_field_master.id = pr_emp_allowances_gen.m_id " +
                     "where pr_emp_allowances_gen.active=1 and pr_allowance_field_master.active=1 AND month(pr_emp_allowances_gen.fm)= '" + s2 + "' AND YEAR(pr_emp_allowances_gen.fm)= '" + s1 + "' AND e.RetirementDate >=GETDATE() AND pr_emp_allowances_gen.amount>0 " + cond2 +
                    "UNION ALL " +
                    "select e.empId , e.Shortname,des.Name as Desig ,pr_allowance_field_master.name AS Allowance,amount,benefit ,pr_allowance_field_master.id as allid  " +
                    "from pr_emp_allowances_spl join employees e on e.EmpId = pr_emp_allowances_spl.emp_code  join designations des on des.id = e.currentdesignation  " +
                    "join pr_allowance_field_master on pr_allowance_field_master.id = pr_emp_allowances_spl.m_id " +
                "where pr_allowance_field_master.active=1 and pr_emp_allowances_spl.active=1 AND month(pr_emp_allowances_spl.fm)= '" + s2 + "' AND YEAR(pr_emp_allowances_spl.fm)= '" + s1 + "' AND e.RetirementDate >= GETDATE() AND pr_emp_allowances_spl.amount > 0 " + cond2 + "  order by empId asc;";

            }
            else
            {
                if (types != " All " && emp_code == "All")
                {
                    cond1 = "AND  Allow.all_name in (" + types + ") ";
                }
                if (emp_code != "All" && types == " All ")
                {
                    cond1 = "AND allow.emp_code in (" + empid + ") ";
                }
                if (emp_code != "All" && types != " All " && emp_code != "0" && types != "0")
                {
                    cond1 = "AND  Allow.all_name  in (" + types + ") AND allow.emp_code in (" + empid + ") ";
                }
                qry = " select e.EmpId,e.ShortName,des.Name as Desig, Allow.all_name as Allowance, Allow.all_amount as amount,all_type, '' as benefit " +
                    " from pr_emp_payslip_allowance Allow join Employees e  on e.EmpId = Allow.emp_code " +
                    " join designations des on des.id = e.currentdesignation " +
                    " join pr_emp_payslip P on P.id = Allow.payslip_mid  where   Allow.all_amount > 0 and(allow.all_type = 'EMPSA' or allow.all_type = 'EMPA')" +
                    "  and month(p.fm) = '"+ s2 + "' AND YEAR(p.fm)= '"+ s1 + "' AND e.RetirementDate >= GETDATE() "+cond1+" order by allow.emp_code,allow.all_mid";
                string ac = "";

            }

            DataTable dt = await _sha.Get_Table_FromQry(qry);
            foreach (DataRow dr in dt.Rows)
            {
                var grpdata = dr["empId"].ToString();
                //empid = dr["empId"].ToString();
                if (oldempid != grpdata)
                {
                    SlNo = 1;
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>~</span>" 
                        + ReportColHeader(0, "Emp code", dr["empId"].ToString())
                        + ReportColHeader(10, "Emp Name", dr["Shortname"].ToString())
                        + ReportColHeader(20, "Designation",dr["Desig"].ToString()),
                        column1 = "`",
                        column2 = "`",
                        column3 = "`",

                    };
                    lst.Add(crm);
                }

                oldempid = dr["empId"].ToString();
              
                crm = new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "R",
                    grpclmn = SlNo++.ToString(),
                    column1 = dr["Allowance"].ToString(),
                    column2 = dr["benefit"].ToString(),
                    column3 = ReportColConvertToDecimal(dr["amount"].ToString()),
                    //column4 = dr["period"].ToString(),
                    //column5 = dr["INSTPAID"].ToString(),
                    //column6 = dr["BALANCEINST"].ToString(),

                };
                lst.Add(crm);
            }
            return lst;

           // return await _sha.Get_Table_FromQry(qry);

        }

        #endregion

        #region All Allowance Wise
        public async Task<IList<CommonReportModel>> AllAllowanceWiseData(string emp_code, string types, string mnth)
        {
            string qry = "";

            if (emp_code.Contains("^"))
            {
                emp_code = "0";
                types = "0";
                mnth = "01-01-01";
            }

            DateTime str = Convert.ToDateTime(mnth);
            string str1 = str.ToString("yyyy-MM-dd");
            string[] sa = str1.Split('-');
            string s1 = sa[0];
            string s2 = sa[1];

            string Type = types;
            string allowancename = types;
            string empid = emp_code;
            string cond = "";
            string cond1 = "";
            string cond2 = "";
            string res2 = "";
            string mnthqry = "";

            StringBuilder ATypes = new StringBuilder();
            if (types == "0" || types != "0" && types != null && types != "" && types != " All ")
            {

                string[] Types = Type.Split(',');
                foreach (string word in Types)
                {
                    string res1 = word.TrimStart();
                    res2 = res1.TrimEnd();
                    ATypes.Append("'");
                    ATypes.Append(res2);
                    ATypes.Append("', ");
                }
                types = ATypes.ToString(0, ATypes.Length - 2);
            }

            mnthqry = "select fm from pr_month_details where active=1 ";
            DataTable dtmoth = await _sha.Get_Table_FromQry(mnthqry);

            DateTime mnthdet = Convert.ToDateTime(dtmoth.Rows[0]["fm"].ToString());
            string monthstr = mnthdet.ToString("yyyy-MM-dd");
            string[] mntharr = monthstr.Split('-');
            string mntharr1 = mntharr[0];
            string mntharr2 = mntharr[1];


            if (s1 == mntharr1 && s2 == mntharr2)
            {
                if (types != " All " && emp_code == "All")
                {
                    cond1 = "AND pr_branch_allowance_master.name in (" + types + ") ";
                    cond2 = "AND pr_allowance_field_master.name in (" + types + ")  ";
                }
                if (emp_code != "All" && types == " All ")
                {
                    cond1 = "AND emp_code in (" + empid + ")  ";
                    cond2 = "AND emp_code in (" + empid + ") ";
                }
                if (emp_code != "All" && types != " All ")
                {
                    cond1 = "AND pr_branch_allowance_master.name in (" + types + ") AND emp_code in (" + empid + ") ";
                    cond2 = "AND pr_allowance_field_master.name in (" + types + ") AND emp_code in (" + empid + ")";
                }

                qry = "select pr_branch_allowance_master.name as allowancename,pr_branch_allowance_master.description,emp_code,e.ShortName,des.Name as designation,pr_branch_allowance_master.description as Allowance,benefit ,amount,pr_branch_allowance_master.id as allid " +
                    "from pr_emp_branch_allowances join employees e on e.EmpId = pr_emp_branch_allowances.emp_code " +
                    "join designations des on des.id = e.currentdesignation join pr_branch_allowance_master on pr_branch_allowance_master.id = pr_emp_branch_allowances.allowance_mid " +
                    "where  pr_branch_allowance_master.active=1 AND pr_emp_branch_allowances.active = 1 AND month(pr_emp_branch_allowances.fm)= '" + s2 + "' AND YEAR(pr_emp_branch_allowances.fm)= '" + s1 + "' AND e.RetirementDate >=GETDATE() AND pr_branch_allowance_master.amount >0 " + cond1 +
                    "union all select pr_allowance_field_master.name as allowancename , null ,emp_code,e.ShortName,des.Name as designation,pr_allowance_field_master.name as Allowance ,benefit,amount, pr_allowance_field_master.id as allid " +
                    "from pr_emp_allowances_gen join employees e on e.EmpId = pr_emp_allowances_gen.emp_code join pr_allowance_field_master on pr_allowance_field_master.id = pr_emp_allowances_gen.m_id join designations des on des.id = e.currentdesignation " +
                    "where  pr_allowance_field_master.active=1 AND pr_emp_allowances_gen.active=1 AND month(pr_emp_allowances_gen.fm)= '" + s2 + "' AND YEAR(pr_emp_allowances_gen.fm)= '" + s1 + "' AND e.RetirementDate >=GETDATE() AND pr_emp_allowances_gen.amount>0 " + cond2 +
                    "union all select pr_allowance_field_master.name as allowancename,null ,emp_code,e.ShortName,des.Name as designation,pr_allowance_field_master.name as Allowance ,benefit,amount,pr_allowance_field_master.id as allid " +
                    "from pr_emp_allowances_spl join employees e on e.EmpId = pr_emp_allowances_spl.emp_code join pr_allowance_field_master on pr_allowance_field_master.id = pr_emp_allowances_spl.m_id join designations des on des.id = e.currentdesignation " +
                    "where pr_emp_allowances_spl.active=1 AND pr_allowance_field_master.active=1 AND month(pr_emp_allowances_spl.fm)= '" + s2 + "' AND YEAR(pr_emp_allowances_spl.fm)= '" + s1 + "' AND e.RetirementDate >= GETDATE() AND pr_emp_allowances_spl.amount > 0 " + cond2 + "  ORDER BY  allowancename ";
            }
            else
            {
                if (types != " All " && emp_code == "All")
                {
                    cond1 = "AND Allow.all_name in (" + types + ") ";
                }
                if (emp_code != "All" && types == " All ")
                {
                    cond1 = "AND allow.emp_code in (" + empid + ")  ";
                }
                if (emp_code != "All" && types != " All ")
                {
                    cond1 = "AND Allow.all_name in (" + types + ") AND allow.emp_code in (" + empid + ") ";
                }

                qry = " select allow.emp_code,e.ShortName,des.Name as designation, Allow.all_name as Allowance, Allow.all_name as allowancename, Allow.all_amount as amount," +
                    "all_type,'' as benefit from pr_emp_payslip_allowance Allow  join Employees e  on e.EmpId = Allow.emp_code" +
                    " join designations des on des.id = e.currentdesignation " +
                    " join pr_emp_payslip P on P.id = Allow.payslip_mid  where month(p.fm) = '" + s2 + "' AND YEAR(p.fm)= '" + s1 + "' " +
                    "AND e.RetirementDate >= GETDATE() AND Allow.all_amount > 0 and(allow.all_type = 'EMPSA' or allow.all_type = 'EMPA')" +
                    " and month(p.fm) = '" + s2 + "' AND YEAR(p.fm)= '" + s1 + "' AND e.RetirementDate >= GETDATE() " + cond1 + "  order by allowancename";

            }

            DataTable dt = await _sha.Get_Table_FromQry(qry);
            //int count = dt.Rows.Count;
            
            foreach (DataRow dr in dt.Rows)
            {
                var grpdata = dr["allowancename"].ToString();

               // empid = dr["allowancename"].ToString();
                if (oldAllType != grpdata)
                {
                    SlNo = 1;

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>~</span>"
                        + ReportColHeader(0, "Allowance Name", dr["allowancename"].ToString() ),
                        column1 = "`",
                        column2 = "`",
                        column3 = "`",
                        column4 = "`",
                        column5 = "`",
                        column6 = "`",

                    };
                    lst.Add(crm);
                }

                
                //lst.Add(crm);
                crm = new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "R",
                    grpclmn= SlNo++.ToString(),
                    column1 = dr["emp_code"].ToString(),
                    column2 = dr["ShortName"].ToString(),
                    column3 = dr["designation"].ToString(),
                    column4 = dr["Allowance"].ToString(),
                    column5 = dr["benefit"].ToString(),
                    column6 = ReportColConvertToDecimal(dr["amount"].ToString()),

                };
            lst.Add(crm);
                oldAllType = dr["allowancename"].ToString();

            }
            return lst;

        }

        #endregion



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
