using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Reports
{
    public class AllLoanReportBusiness : BusinessBase
    {
        public AllLoanReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        string qrySel;
        string qrysel1;

        string oldtype = "";
        string oldcode = "";
        string type = "";
        string code = "";
        int RowCnt = 0;
        int SlNo = 1;

        IList<Poc1ModelLIC> lst = new List<Poc1ModelLIC>();


        public async Task<IList<Poc1ModelLIC>> GetLoanReport(string Month, string RegEmp, string SupEmp)
        {
            string qry = "";
            string newmonth = ""; 

            string general = PrConstants.REGULAR;
            string adhoc = PrConstants.ADHOC;
            if(Month.Contains("^"))
            {
                Month = "2019-04-19";
            }
            if (RegEmp.Contains("^"))
            {
                RegEmp = "0";

                Month = "2019-04-19";
            }

            DateTime str = Convert.ToDateTime(Month);
            string str1 = str.ToString("yyyy-MM-dd");
            string empid = RegEmp;
            // employee id not equal to null and both LIC and HFC is selected 

            qry = " select Convert(date,lotype.fm,102) as fm,lotype.loan_type as loan_type ,sum(loadj.principal_open_amount) as principal_open_amount,sum(loadj.interest_accured) as interest_accured,sum( case when loadj.principal_open_amount is null then 0 else loadj.principal_open_amount end +  case when loadj.interest_accured is null then 0 else loadj.interest_accured end) as Total from pr_emp_adv_loan_type lotype " +
                   " join pr_emp_adv_loans loadv on loadv.loan_type_mid = lotype.id " +
                     "join pr_emp_adv_loans_adjustments loadj on loadj.emp_adv_loans_mid = loadv.id " +
                     " where  month(Convert(date,lotype.fm,102)) =month('" + str1 + "') and year(Convert(date,lotype.fm,102)) =year('" + str1 + "')  " +
                     "group by lotype.loan_type,lotype.fm ";


            qrysel1 = " select  Convert(date,lotype.fm,102) as fm,sum(loadj.principal_open_amount) as principal_open_amount,sum(loadj.interest_accured) as interest_accured, " +
                      " sum( case when loadj.principal_open_amount is null then 0 else loadj.principal_open_amount end +  case when loadj.interest_accured is null then 0 else loadj.interest_accured end) as Total from pr_emp_adv_loan_type lotype" +
                      " join pr_emp_adv_loans loadv on loadv.loan_type_mid = lotype.id" +
                   " join pr_emp_adv_loans_adjustments loadj on loadj.emp_adv_loans_mid = loadv.id " +
                          " where  month(Convert(date,lotype.fm,102)) = month('" + str1 + "') and year(Convert(date,lotype.fm,102)) =year('" + str1 + "') " +
                        "group by lotype.loan_type,lotype.fm ";


            DataSet ds = await _sha.Get_MultiTables_FromQry(qry + qrysel1);
            DataTable dtALL = ds.Tables[0];
            DataTable dtTot = ds.Tables[1];

            foreach (DataRow dr in dtALL.Rows)
            {


                newmonth = dr["fm"].ToString();
                
                if (Month != "")
                {
                    //prev. br. footer
           

                    //lst.Add(new Poc1ModelLIC
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "H",
                    //    SlNo = "<span style='color:#C8EAFB'>~</span>"
                    //            + ReportColHeader(0, "Branch", Month)
                    //});


                      lst.Add(new Poc1ModelLIC
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            SlNo = "S.No",
                           loan_type = "loan_type",
                           principal_open_amount = "principal_open_amount",
                           interest_accured = "interest_accured",
                           Total = "Total",
                      });
                    
                    
                    lst.Add(new Poc1ModelLIC
                    {
                        RowId = RowCnt++,
                        HRF = "R",

                        SlNo = SlNo++.ToString(),
                        
                        loan_type = dr["loan_type"].ToString(),
                        principal_open_amount = dr["principal_open_amount"].ToString(),
                        interest_accured = dr["interest_accured"].ToString(),
                        Total = dr["Total"].ToString(),
                    });

                    

                }
            }
            if(ds.Tables[0].Rows.Count>0)
            {
                Poc1ModelLIC tot = getTotal(newmonth, dtTot);
                tot.RowId = RowCnt++;
                lst.Add(tot);
            }
                

            
            return lst;
        }

        private Poc1ModelLIC getTotal(string Month, DataTable dt)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["fm"].ToString() == Month)
                .Select(x => new { tot = x["principal_open_amount"].ToString() +  "~" + x["Total"].ToString() }).FirstOrDefault();

            var arrTots = val.tot.ToString().Split('~');


            var tot = new Poc1ModelLIC
            {
                RowId = 0,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>"
                + ReportColFooter(50, "principal_open_amount", arrTots[0])
                //+ ReportColFooter(10, "interest_accured", arrTots[1])
                + ReportColFooter(10, "Total", arrTots[1])
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




        public class Poc1ModelLIC
        {
            public int RowId { get; set; }
            public string HRF { get; set; }
            public string SlNo { get; set; }
            public string Deduction_type { get; set; }
            public string EmpCode { get; set; }
            public string EmpName { get; set; }
            public string Account_No { get; set; }
            public string Amount { get; set; }
            public string Total { get; set; }
            public string loan_type { get; set; }
            public string principal_open_amount { get; set; }
            public string interest_accured { get; set; }
            public string Month { get; set; }




        }
    }
}
