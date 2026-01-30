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
using System.Web.UI.WebControls;
using System.Collections;
using Newtonsoft.Json;

namespace PayRollBusiness.Reports
{
    public class SalBrReportBusiness : BusinessBase
    {
        public SalBrReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        IList<CommonReportModel> lst = new List<CommonReportModel>();

        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        public async Task<IList<CommonReportModel>> GetSalBrReport(string branch, string inputMonth)

        {
            if (branch.Contains("42"))
            {
                branch = branch.Replace("42", "43");
            }
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            string oldbranch = "";
            string branch1 = "";
            int SlNo = 1;
            string cond = "";

            string qr1 = "";
            string qry2 = "";
            string cond2 = "";
            int RowCnt = 0;
            string ipmn = "01-01-01";
            string ord = " ";
            string grp = "group by case when b.Name != 'OtherBranch' then b.Name else 'HeadOffice' end";

            if (inputMonth == "^2")
            {
                inputMonth = ipmn;
            }

            DateTime str = Convert.ToDateTime(inputMonth);
            string str1 = str.ToString("yyyy-MM-dd");
            string[] sa = str1.Split('-');
            string s1 = sa[0];
            string s2 = sa[1];
            if (s2.StartsWith("0"))
            {
                s2 = s2.Substring(1);
            }
            string s3 = sa[2];
            if (branch == "^1" && inputMonth == "^2")
            {
                cond = "   ";
                cond2 = "   ";
            }
            else if (branch != "^1" && branch != "42" && inputMonth != "^2")
            {
                cond = " AND e.Branch in (" + branch + ")  ";
                ord = " order by b.name";
                cond2 = " AND e.Branch in (" + branch + ") ";
                grp = "group by case when b.Name != 'OtherBranch' then b.Name else 'HeadOffice' end ";

            }
            else if (branch != "^1" && branch == "42" && inputMonth != "^2")
            {

                cond = " AND b.name='OtherBranch'  ";
                ord = " order by b.name";
                cond2 = " AND b.name='OtherBranch' ";

            }
            else if (branch == "0" && inputMonth == "^2")
            {
                cond = "";
                cond2 = "";
            }
            else if (branch == "^1" && inputMonth != "")
            {
                cond = "   ";
                ord = " ";
                cond2 = "    ";

            }

            if (branch.Contains("0") || branch.Contains("^"))
            {
                qr1 = "select case when b.Name!='OtherBranch' then b.Name else 'HeadOffice' end as grpcol, d.name as desig,ps.spl_type,e.shortname as name, ps.emp_code as Code ,case when b.Name!='OtherBranch' then b.Name else 'HeadOffice' end  as branch, ps.gross_amount as grossamt, ps.deductions_amount as deductions, ps.net_amount as netamt,ps.dd_provident_fund as provident_fund,ps.NPS from pr_emp_payslip ps join employees e on e.empid=ps.emp_code join branches b on b.id=e.branch join designations d on d.id=e.CurrentDesignation where   month(fm)='" + s2 + "' and year(fm)='" + s1 + "'";
            }
            else
            {
                qr1 = "select case when b.Name!='OtherBranch' then b.Name else 'HeadOffice' end as grpcol, d.name as desig,ps.spl_type,e.shortname as name, ps.emp_code as Code ,case when b.Name!='OtherBranch' then b.Name else 'HeadOffice' end  as branch, ps.gross_amount as grossamt, ps.deductions_amount as deductions, ps.net_amount as netamt,ps.dd_provident_fund as provident_fund from pr_emp_payslip ps join employees e on e.empid=ps.emp_code join branches b on b.id=e.branch join designations d on d.id=e.CurrentDesignation where   month(fm)='" + s2 + "' and year(fm)='" + s1 + "'" + cond;
            }
            qr1 += ord;


            if (branch.Contains("0") || branch.Contains("^"))
            {
                qry2 = " select case when b.Name!='OtherBranch' then b.Name else 'HeadOffice' end  as branch, sum(ps.gross_amount) as grosstotal,sum(ps.deductions_amount) as dtotal,sum(ps.net_amount) as nettotal, sum(ps.dd_provident_fund) as provident_total,sum(ps.NPS) as NPS_Total from pr_emp_payslip ps join employees e on e.empid = ps.emp_code join branches b on b.id = e.branch join designations d on d.id = e.CurrentDesignation where  month(fm)='" + s2 + "' and year(fm)='" + s1 + "' ";

            }
            else
            {
                qry2 = " select case when b.Name != 'OtherBranch' then b.Name else 'HeadOffice' end as branch, sum(ps.gross_amount) as grosstotal,sum(ps.deductions_amount) as dtotal,sum(ps.net_amount) as nettotal, sum(ps.dd_provident_fund) as provident_total,sum(ps.NPS) as NPS_Total from pr_emp_payslip ps join employees e on e.empid = ps.emp_code join branches b on b.id = e.branch join designations d on d.id = e.CurrentDesignation where  month(fm)= '" + s2 + "' and year(fm)= '" + s1 + "'" + cond2;
            }
            qry2 += grp;
            //string qry2 = " select  sum(ps.gross_amount) as grosstotal,sum(ps.net_amount) as nettotal ,b.Name from pr_emp_payslip ps join employees e on e.empid = ps.emp_code join branches b on b.id = e.branch join designations d on d.id = e.CurrentDesignation where  active = 1 and month(fm)='" + s2 + "' and year(fm)='" + s1 + "'" + cond2;
            DataSet ds = await _sha.Get_MultiTables_FromQry(qr1 + qry2);
            DataTable dtSalbr = ds.Tables[0];
            DataTable dtSalbr1 = ds.Tables[1];
            foreach (DataRow drs1 in dtSalbr1.Rows)
            {
                string oldbranch1 = drs1["branch"].ToString();
                foreach (DataRow drs in dtSalbr.Rows)
                {
                    branch1 = drs["grpcol"].ToString();
                    if (oldbranch1 == branch1)
                    {

                        if (oldbranch != "" && oldbranch != branch1)
                        {
                            SlNo = 1;
                            ////prev. br. footer
                            //CommonReportModel tot = getTotal(oldbranch, dtSalbr1);
                            //tot.RowId = RowCnt++;
                            //lst.Add(tot);

                            //grp header
                            lst.Add(new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "H",
                                column1 = "<span style='color:#C8EAFB'>~</span>"
                                        + ReportColHeader(0, "Branch", branch1),
                                column2 = "`",
                                column3 = "`",
                                column4 = "`",
                                column5 = "`",
                                column6 = "`",
                                column7 = "`",
                                column8 = "`",
                            });


                        }
                        else if (oldbranch == "")
                        {
                            //grp header
                            lst.Add(new CommonReportModel
                            {
                                RowId = RowCnt++,
                                HRF = "H",
                                column1 = "<span style='color:#C8EAFB'>~</span>"
                                        + ReportColHeader(0, "Branch", branch1),
                                column2 = "`",
                                column3 = "`",
                                column4 = "`",
                                column5 = "`",
                                column6 = "`",
                                column7 = "`",
                                column8 = "`",
                            });



                        }
                    }

                    //oldbranch = drs["grpcol"].ToString();



                    if (oldbranch1 == branch1)
                    {
                        lst.Add(new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "R",
                            column1 = SlNo++.ToString(),
                            column2 = drs["Code"].ToString(),
                            column3 = drs["name"].ToString(),
                            column4 = drs["desig"].ToString(),
                            column5 = ReportColConvertToDecimal(drs["grossamt"].ToString()),
                            column6 = ReportColConvertToDecimal(drs["deductions"].ToString()),
                            column7 = ReportColConvertToDecimal(drs["netamt"].ToString()),
                            column8 = drs["spl_type"].ToString()
                        });
                    }
                    oldbranch = drs["grpcol"].ToString();



                    //if (oldbranch != "")
                    //{
                    //    CommonReportModel tot = getTotal(oldbranch, dtSalbr1);
                    //    tot.RowId = RowCnt++;
                    //    lst.Add(tot);

                    //}

                }






                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(125, "Gross Total", ReportColConvertToDecimal(drs1["grosstotal"].ToString()))
                            + ReportColFooter(10, "Deductions Total", ReportColConvertToDecimal(drs1["dtotal"].ToString()))
                            + ReportColFooter(10, "Net Total", ReportColConvertToDecimal(drs1["nettotal"].ToString()))
                });

                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "Payment Order", "")
                });


                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "An Amount of Rupees   ", ReportColConvertToDecimal(drs1["grosstotal"] + drs1["provident_total"].ToString()))
              
                });
                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                            + ReportColFooter(0, "be debited to the establishment account of your branch as detailed here under", "")
               
                });
                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                           + ReportColFooter(0, "a).  Total Gross Salary      &nbsp &nbsp  &nbsp &nbsp  &nbsp &nbsp      = Rs &nbsp &nbsp  &nbsp &nbsp", ReportColConvertToDecimal(drs1["grosstotal"].ToString()))
                });
                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                           + ReportColFooter(0, "b).  Bank Contribution Towards", "")
                });
                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                           + ReportColFooter(10, "(i). Provident Fund      &nbsp &nbsp  &nbsp &nbsp     = Rs &nbsp &nbsp  &nbsp &nbsp", ReportColConvertToDecimal(drs1["provident_total"].ToString()))
                });

                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                               + ReportColFooter(10, "(ii). Family pension      &nbsp &nbsp  &nbsp &nbsp     = Rs &nbsp &nbsp  &nbsp &nbsp", "0.00")
                });
                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                          + ReportColFooter(10, "(iii).NPS     &nbsp &nbsp  &nbsp &nbsp &nbsp &nbsp   &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp  = Rs &nbsp &nbsp  &nbsp &nbsp", ReportColConvertToDecimal(drs1["NPS_Total"].ToString()))
                });

                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                     //         + ReportColFooter(35, "Total      &nbsp &nbsp  &nbsp &nbsp     = Rs &nbsp &nbsp  &nbsp &nbsp", (Convert.ToDouble(drs1["grosstotal"]) + Convert.ToDouble(drs1["provident_total"])).ToString())
                     + ReportColFooter(35, "Total      &nbsp &nbsp  &nbsp &nbsp     = Rs &nbsp &nbsp  &nbsp &nbsp", ReportColConvertToDecimal(drs1["grosstotal"] + drs1["provident_total"].ToString()))

                });
                lst.Add(new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "F",
                    column1 = "<span style='color:#eef8fd'>^</span>"
                           + ReportColFooter(0, "Passed for payment of Gross Amount of Rs.   ", ReportColConvertToDecimal(drs1["grosstotal"].ToString()))
                           + ReportColFooter(30, "Net Amount of Rs.  ", ReportColConvertToDecimal(drs1["nettotal"].ToString()))
                });

            }

            return lst;
        }

        //private CommonReportModel getTotal(string branch, DataTable dt)
        //{
        //    var val = dt.Rows.Cast<DataRow>()
        //        .Where(x => x["branch"].ToString() == branch)
        //        .Select(x => new { tot = x["grosstotal"].ToString() + "~" + x["dtotal"].ToString() + "~" + x["nettotal"].ToString() + "~" +x["provident_total"].ToString() }).FirstOrDefault();

        //    var arrTots = val.tot.ToString().Split('~');


        //    CommonReportModel tot = new CommonReportModel
        //    {
        //        RowId = 0,
        //        HRF = "F",
        //        column1 = "<span style='color:#eef8fd'>^</span>"
        //        + ReportColFooter(150, "Gross Total", arrTots[0])
        //        + ReportColFooter(10, "Deductions Total", arrTots[1])
        //        + ReportColFooter(10, "Net Total", arrTots[2])
        //    };
        //    lst.Add(tot);
        //    tot = new CommonReportModel
        //    {
        //        RowId = 1,
        //        HRF = "F",
        //        column1 = "<span style='color:#eef8fd'>^</span>"
        //        + ReportColFooter(0, "Payment Order","")
        //    };
        //    lst.Add(tot);
        //    tot = new CommonReportModel
        //    {
        //        RowId = 1,
        //        HRF = "F",
        //        column1 = "<span style='color:#eef8fd'>^</span>"
        //        + ReportColFooter(0, "An Amount of Rupees   ", (arrTots[0]+arrTots[3]).ToString())
        //    };
        //    lst.Add(tot);
        //    tot = new CommonReportModel
        //    {
        //        RowId = 2,
        //        HRF = "F",
        //        column1 = "<span style='color:#eef8fd'>^</span>"
        //        + ReportColFooter(0, "be debited to the establishment account of your branch as detailed here under", "")
        //    };
        //    lst.Add(tot);
        //    tot = new CommonReportModel
        //    {
        //        RowId = 3,
        //        HRF = "F",
        //        column1 = "<span style='color:#eef8fd'>^</span>"
        //       + ReportColFooter(0, "a).  Total Gross Salary           = Rs", arrTots[0])
        //    };
        //    lst.Add(tot);
        //    tot = new CommonReportModel
        //    {
        //        RowId = 4,
        //        HRF = "F",
        //        column1 = "<span style='color:#eef8fd'>^</span>"
        //       + ReportColFooter(0, "b).  Bank Contribution Towards", "")
        //    };
        //    lst.Add(tot);
        //    tot = new CommonReportModel
        //    {
        //        RowId = 5,
        //        HRF = "F",
        //        column1 = "<span style='color:#eef8fd'>^</span>"
        //       + ReportColFooter(10, "(i). Provident Fund           = Rs", arrTots[3])
        //    };
        //    lst.Add(tot);
        //    tot = new CommonReportModel
        //    {
        //        RowId = 6,
        //        HRF = "F",
        //        column1 = "<span style='color:#eef8fd'>^</span>"
        //       + ReportColFooter(50, "Total           = Rs", (arrTots[0]+arrTots[3]).ToString())
        //    };
        //    lst.Add(tot);
        //    tot = new CommonReportModel
        //    {
        //        RowId = 7,
        //        HRF = "F",
        //        column1 = "<span style='color:#eef8fd'>^</span>"
        //        + ReportColFooter(0, "Passed for payment of Gross Amount of Rs.   ", arrTots[0])
        //        + ReportColFooter(30, "Net Amount of Rs.  ", arrTots[2])
        //    };
        //    lst.Add(tot);
        //    return tot;
        //}


        public async Task<string> getBranches()
        {
            try
            {

                //salarybillofmonth crm = new salarybillofmonth();
                string qr1 = "Select distinct [Id], [Name] from Branches Where Name!='OtherBranch'";
                DataTable dsGetLfields = await _sha.Get_Table_FromQry(qr1);


                var dtLTfields = dsGetLfields;
                //var dtALfileds = dsGetLfields.Tables[1];

                var ltjson = JsonConvert.SerializeObject(dtLTfields);

                ltjson = ltjson.Replace("null", "''");

                var javaScriptSerializer = new JavaScriptSerializer();
                var ltDetails = javaScriptSerializer.DeserializeObject(ltjson);

                var resultJson = javaScriptSerializer.Serialize(new { LTDetails = ltDetails });

                return JsonConvert.SerializeObject(resultJson);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "E#Error:#" + msg;
            }

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
