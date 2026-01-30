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
    public class PFFundBusiness : BusinessBase
    {
        public PFFundBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        public async Task<IList<CommonReportModel>> PfFund(string Fyear, string inputtype,string currentmnth)
        {

            CommonReportModel crm = new CommonReportModel();
            IList<CommonReportModel> lst = new List<CommonReportModel>();

            int RowCnt = 0;
            int SlNo = 1;
            int Eyear = 1900;
            int FYear = 1900;
            string oldempid = "";
            //string empid = "";
            string emp_id = "";
            if (Fyear != null && Fyear != "^1")
            {
                string[] split1 = Fyear.Split('-');
                //Console.Write(split[0], split[1]);
                FYear = int.Parse(split1[0]);
                Eyear = int.Parse(split1[1]);
            }

            if (Fyear.Contains("^"))
            {
                FYear = 1900;
                Eyear = 1900;
            }

            int Fdate = FYear;
            int Edate = Eyear;
            int year = 0;
            int curmnth = 0;

            if (currentmnth != null)
            {
                 curmnth = DateTime.Parse("1." + currentmnth + "" + Fdate).Month;
                 year = (curmnth < 4) ? FYear+1 : FYear;
            }

            string qry = "";
            string qry1 = "";
            string qry2 = "";
            string qry11 = "";
            string qryfy = "";
            string qryopenbalfy = "";
            decimal? ContYear = 0;
            decimal? IntOpBal = 0;
            decimal? IntYear = 0;
            decimal? NrLoanOP = 0;
            decimal? Nr_Loan_Yr = 0;
            string nrloan = "";
            if (Fyear != null && (currentmnth != null && currentmnth!=""))
            {
                if (inputtype == "Regular")
                {
                    qry = "select  e.EmpId as EmpId,e.ShortName as ShortName,d.Name as designation,(ISNULL(obal.own_share_open,0)+ISNULL(obal.bank_share_open,0)+ISNULL(obal.vpf_open,0)) as FundOpBal," +
                        "(ISNULL(obal.own_share, 0) + ISNULL(obal.bank_share, 0) + ISNULL(obal.vpf, 0)) as ContYear,(ISNULL(obal.own_share_intrst_open, 0) + ISNULL(obal.bank_share_intrst_open, 0) + ISNULL(obal.vpf_intrst_open, 0)) as IntOpBal," +
                        "(ISNULL(obal.own_share_intrst_amount, 0) + ISNULL(obal.bank_share_intrst_amount, 0) + ISNULL(obal.vpf_intrst_amount, 0)) as IntYear,(ISNULL(obal.own_share_total, 0) + ISNULL(obal.bank_share_total, 0) + ISNULL(obal.vpf_total, 0)) as NrLoanOP " +
                        " from Employees e join pr_ob_share obal on e.empid = obal.emp_code " +
                       "join Designations d on d.id = e.CurrentDesignation " +
                       //--"join pr_emp_general empgen on empgen.emp_code = e.empid  " +
                       "where e.RetirementDate>= GetDate() and obal.fy = '" + year + "' and month(obal.fm)=" + curmnth + "  order by e.EmpId";
                    //added "and empgen.active=1 order by e.EmpId" by chaitanya on 4/5/2020
                    //and obal.emp_code = 793
                }
                else
                {
                    qry = "select  e.EmpId as EmpId,e.ShortName as ShortName,d.Name as designation,(ISNULL(obal.own_share_open,0)+ISNULL(obal.bank_share_open,0)+ISNULL(obal.vpf_open,0)) as FundOpBal," +
                        "(ISNULL(obal.own_share, 0) + ISNULL(obal.bank_share, 0) + ISNULL(obal.vpf, 0)) as ContYear,(ISNULL(obal.own_share_intrst_open, 0) + ISNULL(obal.bank_share_intrst_open, 0) + ISNULL(obal.vpf_intrst_open, 0)) as IntOpBal," +
                        "(ISNULL(obal.own_share_intrst_amount, 0) + ISNULL(obal.bank_share_intrst_amount, 0) + ISNULL(obal.vpf_intrst_amount, 0)) as IntYear,(ISNULL(obal.own_share_total, 0) + ISNULL(obal.bank_share_total, 0) + ISNULL(obal.vpf_total, 0)) as NrLoanOP " +
                        " from Employees e join pr_ob_share  obal on e.empid = obal.emp_code " +
                       "join Designations d on d.id = e.CurrentDesignation " +
                       //--"join pr_emp_general empgen on empgen.emp_code = e.empid  " +
                       "where e.RetirementDate>= '" + Fdate + "-04-01' and e.RetirementDate<= GetDate() and obal.fy = '" + year + "' and month(obal.fm)=" + curmnth + " order by e.EmpId";
                }
            }
            else
            {
                if (inputtype == "Regular")
                {
                    qry = "select  e.EmpId as EmpId,e.ShortName as ShortName,d.Name as designation,(ISNULL(obal.os_open,0)+ISNULL(obal.bs_open,0)+ISNULL(obal.vpf_open,0)) as FundOpBal," +
                       "(ISNULL(obal.os_cur, 0) + ISNULL(obal.bs_cur, 0) + ISNULL(obal.vpf_cur, 0)) as ContYear,(ISNULL(obal.os_open_int, 0) + ISNULL(obal.bs_open_int, 0) + ISNULL(obal.vpf_open_int, 0)) as IntOpBal," +
                       "(ISNULL(obal.os_cur_int, 0) + ISNULL(obal.bs_cur_int, 0) + ISNULL(obal.vpf_cur_int, 0)) as IntYear,(ISNULL(obal.pf_return, 0) + ISNULL(obal.bank_return, 0) + ISNULL(obal.vpf_return, 0)) as NrLoanOP " +
                       "from Employees e join pr_pf_open_bal_year obal on e.empid = obal.emp_code " +
                       "join Designations d on d.id = e.CurrentDesignation " +
                       //--"join pr_emp_general empgen on empgen.emp_code = e.empid  " +
                       "where e.RetirementDate>= GetDate() and obal.fy = '" + Fdate + "'  order by e.EmpId";
                    //added "and empgen.active=1 order by e.EmpId" by chaitanya on 4/5/2020
                    //and obal.emp_code = 793
                }
                else
                {
                    qry = "select  e.EmpId as EmpId,e.ShortName as ShortName,d.Name as designation,(ISNULL(obal.os_open,0)+ISNULL(obal.bs_open,0)+ISNULL(obal.vpf_open,0)) as FundOpBal," +
                       "(ISNULL(obal.os_cur, 0) + ISNULL(obal.bs_cur, 0) + ISNULL(obal.vpf_cur, 0)) as ContYear,(ISNULL(obal.os_open_int, 0) + ISNULL(obal.bs_open_int, 0) + ISNULL(obal.vpf_open_int, 0)) as IntOpBal," +
                       "(ISNULL(obal.os_cur_int, 0) + ISNULL(obal.bs_cur_int, 0) + ISNULL(obal.vpf_cur_int, 0)) as IntYear,(ISNULL(obal.pf_return, 0) + ISNULL(obal.bank_return, 0) + ISNULL(obal.vpf_return, 0)) as NrLoanOP " +
                       "from Employees e join pr_pf_open_bal_year obal on e.empid = obal.emp_code " +
                       "join Designations d on d.id = e.CurrentDesignation " +
                       //--"join pr_emp_general empgen on empgen.emp_code = e.empid  " +
                       "where e.RetirementDate>= '" + Fdate + "-04-01' and e.RetirementDate<= GetDate() and obal.fy = '" + Fdate + "'  order by e.EmpId";
                }
            }
            DataTable dts = await _sha.Get_Table_FromQry(qry);

            foreach (DataRow drs in dts.Rows)
            {
                emp_id = drs["EmpId"].ToString();
                if(currentmnth!=null && currentmnth != "")
                {
                    qry1 = "select case when sum(bank_share+own_share+vpf) is null then 0 else sum(bank_share+own_share+vpf) end as ContYear from pr_ob_share where emp_code=" + emp_id + " and fm = '" + year + "-"+currentmnth+"-01';";
                }
                else
                {
                    qry1 = "select case when sum(bank_share+own_share+vpf) is null then 0 else sum(bank_share+own_share+vpf) end as ContYear from pr_ob_share where emp_code=" + emp_id + " and fm between '" + Fdate + "-04-01' and '" + Edate + "-03-01';";
                }
                //qry1 = "select case when sum(bank_share+own_share+vpf) is null then 0 else sum(bank_share+own_share+vpf) end as ContYear from pr_ob_share where emp_code=" + emp_id + " and fm between '" + Fdate + "-04-01' and '" + Edate + "-03-01';";
                qry11 = "select ((cast(ISNULL(op_bal_inst,0) as decimal)+cast(ISNULL(op_bal_inst_year,0) as decimal))-(cast(ISNULL(op_bal_inst_NRloan,0) as decimal))) as int_year from pr_pf_open_bal_year where fy=" + Fdate + " and emp_code=" + emp_id + "";
                qryfy = "select year(fm)as fy from pr_month_details where active=1;;";
                qryopenbalfy = "select max(fy)as opbalfy from pr_pf_open_bal_year;";
                DataTable dtsnew = await _sha.Get_Table_FromQry(qry1);
                DataTable dts11 = await _sha.Get_Table_FromQry(qry11);
                DataTable dtsfy = await _sha.Get_Table_FromQry(qryfy);
                DataTable dtsopenbalfy = await _sha.Get_Table_FromQry(qryopenbalfy);
                if (oldempid != emp_id)
                {
                    if(Convert.ToInt32(dtsfy.Rows[0]["fy"])==Convert.ToInt32(dtsopenbalfy.Rows[0]["opbalfy"]))
                    {
                        string qrys = "select isnull((select pf_no  from pr_emp_general where emp_code = '" + emp_id + "' and active = 1),'0')  as pf_no";
                        //Nr_Loan_Yr
                        if (currentmnth != null && currentmnth != "")
                        {
                            qry2 = "select sum(ISNULL(nrloan.sanctioned_amount,0)) as Nr_Loan_Yr,emp_code from pr_emp_pf_nonrepayable_loan nrloan " +
                            " where emp_code = '" + emp_id + "' and " +
                            " fm = '" + year + "-" + currentmnth + "-01' group by emp_code";
                        }
                        else
                        {
                            qry2 = "select sum(ISNULL(nrloan.sanctioned_amount,0)) as Nr_Loan_Yr,emp_code from pr_emp_pf_nonrepayable_loan nrloan " +
                            " where emp_code = '" + emp_id + "' and " +
                            " year(fm) = '" + Fdate + "' group by emp_code";
                        }
                        DataTable dt = await _sha.Get_Table_FromQry(qrys);

                        DataTable dts1 = await _sha.Get_Table_FromQry(qry2);
                        
                        foreach (DataRow dt1 in dts1.Rows)
                        {
                            nrloan = dts1.Rows[0]["Nr_Loan_Yr"].ToString();
                            //string nrloan = dts1.Rows[0]["Nr_Loan_Yr"].ToString();
                        }
                        decimal? FundOpBal = 0;

                        FundOpBal = Convert.ToDecimal(drs["FundOpBal"].ToString());
                        //ContYear = Convert.ToDecimal(drs["ContYear"].ToString());
                        ContYear = Convert.ToDecimal(dtsnew.Rows[0]["ContYear"].ToString());
                        IntOpBal = Convert.ToDecimal(drs["IntOpBal"].ToString());
                        //IntYear = Convert.ToDecimal(drs["IntYear"].ToString());
                        IntYear = Convert.ToDecimal(dts11.Rows[0]["int_year"].ToString());
                        NrLoanOP = Convert.ToDecimal(drs["NrLoanOP"].ToString());

                        if (nrloan == "")
                        {
                            Nr_Loan_Yr = 0.00M;
                        }
                        else
                        {
                            if(dts1.Rows.Count>0)
                            {
                                Nr_Loan_Yr = Convert.ToDecimal(dts1.Rows[0]["Nr_Loan_Yr"].ToString());
                            }
                            else
                            {
                                Nr_Loan_Yr = 0.00M;
                            }
                        }

                        //after year end adding contyear value to fundopbal, IntYear to IntOpBal and Nr_Loan_Yr to NrLoanOP;
                        FundOpBal = FundOpBal + ContYear;
                        //ContYear = 0.00m;
                        IntOpBal = IntOpBal + IntYear;
                        //IntYear = 0.00m;
                        NrLoanOP = NrLoanOP + Nr_Loan_Yr;
                        //Nr_Loan_Yr = 0.00m;
                        //end
                        // net fund
                        decimal? netfund1 = FundOpBal + ContYear + IntYear + IntOpBal;
                        decimal? netfun2 = NrLoanOP + Nr_Loan_Yr;
                        decimal? netfund = netfund1 - netfun2;
                        decimal? NetIntOpBal = IntOpBal + IntYear;
                        decimal? NRlnOp = NrLoanOP + Nr_Loan_Yr;

                        SlNo = SlNo++;

                        lst.Add(new CommonReportModel
                        {
                            RowId = RowCnt++,
                            SlNo = SlNo++.ToString(),

                            column1 = drs["EmpId"].ToString(),
                            //column2 = drs["pf_no"].ToString(),
                            column2 = dt.Rows[0]["pf_no"].ToString(),
                            column3 = drs["ShortName"].ToString(),
                            column4 = drs["designation"].ToString(),
                            //column5 = drs["FundOpBal"].ToString(),
                            column5= ReportColConvertToDecimal(FundOpBal.ToString()),
                            //column6 = drs["ContYear"].ToString(),
                            //column6 = dtsnew.Rows[0]["ContYear"].ToString(),
                            column6= ReportColConvertToDecimal(ContYear.ToString()),
                            //column7 = NetIntOpBal.ToString(),
                            column7 = ReportColConvertToDecimal(IntOpBal.ToString()),
                            //column8 = drs["IntYear"].ToString(),
                            //column8 = dts11.Rows[0]["int_year"].ToString(),
                            column8= ReportColConvertToDecimal(IntYear.ToString()),
                            //column9 = NRlnOp.ToString(),
                            column9= NrLoanOP.ToString(),
                            column10 = Convert.ToString(Nr_Loan_Yr),
                            //net fund
                            column11 = Convert.ToString(netfund),
                        });
                    }
                    else
                    {
                        string qrys = "select isnull((select pf_no  from pr_emp_general where emp_code = '" + emp_id + "' and active = 1),'0')  as pf_no";
                        //Nr_Loan_Yr
                        if (currentmnth != null && currentmnth != "")
                        {
                            qry2 = "select sum(ISNULL(nrloan.sanctioned_amount,0)) as Nr_Loan_Yr,emp_code from pr_emp_pf_nonrepayable_loan nrloan " +
                            " where emp_code = '" + emp_id + "' and " +
                            " fm = '" + year + "-" + currentmnth + "-01' group by emp_code";
                        }
                        else
                        {
                            qry2 = "select sum(ISNULL(nrloan.sanctioned_amount,0)) as Nr_Loan_Yr,emp_code from pr_emp_pf_nonrepayable_loan nrloan " +
                            " where emp_code = '" + emp_id + "' and " +
                            " year(fm) = '" + Fdate + "' group by emp_code";
                        }
                            

                        DataTable dt = await _sha.Get_Table_FromQry(qrys);

                        DataTable dts1 = await _sha.Get_Table_FromQry(qry2);
                        
                        foreach (DataRow dt1 in dts1.Rows)
                        {
                            nrloan = dts1.Rows[0]["Nr_Loan_Yr"].ToString();
                            //string nrloan = dts1.Rows[0]["Nr_Loan_Yr"].ToString();
                        }
                        decimal? FundOpBal = 0;

                        FundOpBal = Convert.ToDecimal(drs["FundOpBal"].ToString());
                        //ContYear = Convert.ToDecimal(drs["ContYear"].ToString());
                        ContYear = Convert.ToDecimal(dtsnew.Rows[0]["ContYear"].ToString());
                        IntOpBal = Convert.ToDecimal(drs["IntOpBal"].ToString());
                        //IntYear = Convert.ToDecimal(drs["IntYear"].ToString());
                        IntYear = Convert.ToDecimal(dts11.Rows[0]["int_year"].ToString());
                        NrLoanOP = Convert.ToDecimal(drs["NrLoanOP"].ToString());

                        if (nrloan == "")
                        {
                            Nr_Loan_Yr = 0.00M;
                        }
                        else
                        {
                            if(dts1.Rows.Count>0)
                            {
                                Nr_Loan_Yr = Convert.ToDecimal(dts1.Rows[0]["Nr_Loan_Yr"].ToString());
                            }
                            else
                            {
                                Nr_Loan_Yr = 0.00M;
                            }
                        }
                        // net fund
                        decimal? netfund1 = FundOpBal + ContYear + IntYear + IntOpBal;
                        decimal? netfun2 = NrLoanOP + Nr_Loan_Yr;
                        decimal? netfund = netfund1 - netfun2;
                        decimal? NetIntOpBal = IntOpBal + IntYear;
                        decimal? NRlnOp = NrLoanOP + Nr_Loan_Yr;

                        SlNo = SlNo++;

                        lst.Add(new CommonReportModel
                        {
                            RowId = RowCnt++,
                            SlNo = SlNo++.ToString(),

                            column1 = drs["EmpId"].ToString(),
                            //column2 = drs["pf_no"].ToString(),
                            column2 = dt.Rows[0]["pf_no"].ToString(),
                            column3 = drs["ShortName"].ToString(),
                            column4 = drs["designation"].ToString(),
                            column5 = ReportColConvertToDecimal(drs["FundOpBal"].ToString()),
                            //column6 = drs["ContYear"].ToString(),
                            column6 = ReportColConvertToDecimal(dtsnew.Rows[0]["ContYear"].ToString()),
                            column7 = ReportColConvertToDecimal(NetIntOpBal.ToString()),
                            //column8 = drs["IntYear"].ToString(),
                            column8 = ReportColConvertToDecimal(dts11.Rows[0]["int_year"].ToString()),
                            column9 = NRlnOp.ToString(),
                            column10 = Convert.ToString(Nr_Loan_Yr),
                            //net fund
                            column11 = Convert.ToString(netfund),
                        });
                    }
                    ContYear = 0.00m;
                    IntYear = 0.00m;
                    Nr_Loan_Yr = 0.00m;
                }
                oldempid = drs["EmpId"].ToString();
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
