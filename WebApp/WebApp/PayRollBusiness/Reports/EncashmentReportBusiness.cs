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
   public class EncashmentReportBusiness:BusinessBase
    {
        public EncashmentReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        string olddate = "";
        string[] EncahDetails;
        string[] Desig;
        string[] Cat;
        string[] Name;
        string[] Basic;
        string[] Incr;
        string[] Allw;
        string[] Da;
        string[] Cca;
        string[] Hra;
        string[] SplAllow;
        string[] SplDa;
        string[] gross;
        string[] Pf;
        string[] Vpf;
        string[] It;
        string encashdata = "";
        int RowCnt = 0;
        DateTime endate ;
    
        string empcode = "";
        public async Task<IList<CommonReportModel>> GetEncashmentReportdata(string fromDate, string toDate)
        {
            int SlNo = 1;
            long er_basic = 0;
            long all_amount = 0;
            long er_da = 0;
            long er_cca = 0;
            long er_hra = 0;
            long spl_da = 0;
            long spl_allw = 0;
            long gross_amount = 0;
            long dd_provident_fund = 0;
            long VPFIT = 0;
            long TOTDED = 0;
            long net = 0;
            string Fdate = fromDate;
            string Tdate = toDate;
           
                if (fromDate == "^1")
                {
                    Fdate = "01-01-01";
                    Tdate = "01-01-01";
                }
                DateTime fdate = Convert.ToDateTime(Fdate);
                DateTime tdate = Convert.ToDateTime(Tdate);
                Fdate = fdate.ToString("yyyy-MM-dd");
                Tdate = tdate.ToString("yyyy-MM-dd");


                string query = " select distinct emp_code,e.shortname as shortname,desg.name as desg,fm as grpcol,er_basic ,spl_type,er_da,er_cca,er_hra,spl_da,spl_allw,dd_provident_fund,dd_income_tax,gross_amount,deductions_amount,net_amount," +
                    "pl.Special_Increment,pl.Stagnation_Increments,pl.Annual_Increment,pl.CAIIB_Increment " +
                    "from pr_emp_payslip p left join(select Special_Increment, Stagnation_Increments,Annual_Increment, CAIIB_Increment " +
                    "from (select emp_code, all_amount, all_type from pr_emp_payslip_allowance where emp_code = emp_code and active = 1) pr_emp_payslip_allowance " +
                    "pivot(max(all_amount) for all_type in (Special_Increment, Stagnation_Increments, Annual_Increment, CAIIB_Increment)) result)as pl on p.emp_code = emp_code " +
                    "join Employees e on e.EmpId=emp_code join Designations desg on desg.id=e.CurrentDesignation  " +
                    "WHERE fm BETWEEN '" + Fdate + "' AND '" + Tdate + "' and spl_type='Encashment' order by fm;";

                string qry2 = " select distinct emp_code,e.shortname as shortname,desg.name as desg,fm as grpcol,er_basic ,spl_type,er_da,er_cca,er_hra,spl_da,spl_allw, " +
                    "dd_provident_fund,dd_income_tax,gross_amount,deductions_amount,net_amount, " +
                   "pl.Special_Increment,pl.Stagnation_Increments,pl.Annual_Increment,pl.CAIIB_Increment " +
                   "from pr_emp_payslip p left join(select Special_Increment, Stagnation_Increments,Annual_Increment, CAIIB_Increment " +
                   "from (select emp_code, all_amount, all_type from pr_emp_payslip_allowance where emp_code = emp_code and active = 1) pr_emp_payslip_allowance " +
                   "pivot(max(all_amount) for all_type in (Special_Increment, Stagnation_Increments, Annual_Increment, CAIIB_Increment)) result)as pl on p.emp_code = emp_code " +
                   "join Employees e on e.EmpId=emp_code join Designations desg on desg.id=e.CurrentDesignation  " +
                   "WHERE fm BETWEEN '" + Fdate + "' AND '" + Tdate + "' and spl_type='Encashment' ";

                DataTable dt2 = await _sha.Get_Table_FromQry(qry2);
                DataTable dt = await _sha.Get_Table_FromQry(query);

                string ls = "";
                foreach (DataRow dr in dt.Rows)
                {
                    encashdata = dr["shortname"].ToString() + '*' + dr["desg"].ToString() + '*' + dr["er_basic"].ToString() + '*' + dr["er_da"].ToString() + '*' +
                       dr["er_cca"].ToString() + '*' + dr["er_hra"].ToString() + '*' + dr["spl_da"].ToString() + '*' + dr["spl_allw"].ToString()
                       + '*' + dr["dd_provident_fund"].ToString() + '*' + dr["dd_income_tax"].ToString() + '*' + dr["gross_amount"].ToString()
                       + '*' + dr["deductions_amount"].ToString() + '*' + dr["net_amount"].ToString();

                EncahDetails = encashdata.Split('*');
                Name = EncahDetails[0].Split(',');//name
                Desig = EncahDetails[1].Split(',');//deg
                //Cat = EncahDetails[2].Split(',');
                Basic = EncahDetails[2].Split(','); //er_basic
                Da = EncahDetails[3].Split(','); //er_da
                Cca = EncahDetails[4].Split(',');//er_cca
                Hra = EncahDetails[5].Split(',');//er_hra
                SplDa = EncahDetails[6].Split(','); //spl_da
                SplAllow = EncahDetails[7].Split(','); //spl_allow
                Pf = EncahDetails[8].Split(',');//dd_provident_fund
                Vpf = EncahDetails[9].Split(','); //dd_income_tax
                gross = EncahDetails[10].Split(','); //gross
                 //Incr = EncahDetails[3].Split(','); //all amount
                //Allw = EncahDetails[4].Split(',');
                //It = EncahDetails[13].Split(',');
                endate = Convert.ToDateTime(dr["grpcol"]);
                string Encashmentdate = endate.ToString("dd/MM/yyyy");
                empcode = dr["emp_code"].ToString();
                //string grpdata = endate.ToString("dd/MM/yyyy");
                if (olddate != Encashmentdate)
                {
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>~</span> Encashment Date : <b>" + Encashmentdate + " </b></span>",
                        column1 = "`",
                        column2 = "`",
                        column3 = "`",
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
                    };
                    lst.Add(crm);
                }
                olddate = endate.ToString("dd/MM/yyyy"); 
                //var grpdata = dr["empId"].ToString();
                crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "R",
                        grpclmn = dr["emp_code"].ToString(),
                        column1 = dr["emp_code"].ToString(),
                        column2 = Name[0].ToString(),
                         column3 = Desig[0].ToString(),
                        //column4 = Cat[0].ToString(),
                        column5 = ReportColConvertToDecimal(Basic[0].ToString()),
                        //column7 = Incr[0].ToString(),
                        //column8 = Allw[0].ToString(),
                        column6 = ReportColConvertToDecimal(Da[0].ToString()),
                        column7 = ReportColConvertToDecimal(Cca[0].ToString()),
                        column8 = ReportColConvertToDecimal(Hra[0].ToString()),
                        column9 = ReportColConvertToDecimal(SplDa[0].ToString()),
                        column10 = ReportColConvertToDecimal(SplAllow[0].ToString()),
                        column11 = ReportColConvertToDecimal(Pf[0].ToString()),
                        column12 = ReportColConvertToDecimal(Vpf[0].ToString()),
                        column13 = ReportColConvertToDecimal(gross[0].ToString()),
                        //column17 = It[0].ToString(),
                        column14 = ReportColConvertToDecimal(dr["deductions_amount"].ToString()),
                        column15 = ReportColConvertToDecimal(dr["net_amount"].ToString()),
                    };
                    lst.Add(crm);
                    
                }


                if (fromDate != "^1" && toDate != "^2")
                {

                    foreach (DataRow dr2 in dt2.Rows)
                    {
                    try
                    {
                        er_basic += Convert.ToInt64(dr2["er_basic"].ToString());
                    }
                    catch (Exception e)
                    {

                    }
                }
                //foreach (DataRow dr2 in dt2.Rows)
                //{
                //    all_amount += float.Parse(dr2["all_amount"].ToString());

                //}
                foreach (DataRow dr2 in dt2.Rows)
                {
                    try
                    {
                        er_da += Convert.ToInt64(dr2["er_da"].ToString());
                    }
                    catch (Exception e)
                    {

                    }
                }
                foreach (DataRow dr2 in dt2.Rows)
                {
                    try
                    {
                        er_cca += Convert.ToInt64(dr2["er_cca"].ToString());
                    }
                    catch (Exception e)
                    {

                    }
                }
                foreach (DataRow dr2 in dt2.Rows)
                {
                    try
                    {
                        er_hra += Convert.ToInt64(dr2["er_hra"].ToString());
                    }
                    catch (Exception e)
                    {

                    }
                }
                foreach (DataRow dr2 in dt2.Rows)
                {
                    try
                    {
                        //string data = dr2["spl_da"].ToString();
                        spl_da +=long.Parse(dr2["spl_da"].ToString());
                    }
                    catch (Exception e)
                    {

                    }
                }
                foreach (DataRow dr2 in dt2.Rows)
                {
                    try
                    {
                        spl_allw += Convert.ToInt64(dr2["spl_allw"].ToString());
                    }
                    catch (Exception e)
                    {

                    }
                }
                foreach (DataRow dr2 in dt2.Rows)
                {
                    try
                    {
                        dd_provident_fund += Convert.ToInt64(dr2["dd_provident_fund"].ToString());
                    }
                    catch (Exception e)
                    {

                    }
                }
                foreach (DataRow dr2 in dt2.Rows)
                {
                    try
                    {
                        VPFIT += Convert.ToInt64(dr2["dd_income_tax"].ToString());
                    }
                    catch (Exception e)
                    {

                    }
                }
                foreach (DataRow dr2 in dt2.Rows)
                {
                    try
                    {
                        gross_amount += Convert.ToInt64(dr2["gross_amount"].ToString());
                    }
                    catch (Exception e)
                    {

                    }
                }
               
                foreach (DataRow dr2 in dt2.Rows)
                    {
                    try
                    {
                        TOTDED += Convert.ToInt64(dr2["deductions_amount"].ToString());

                    }
                    catch (Exception e)
                    {

                    }

                }
                foreach (DataRow dr2 in dt2.Rows)
                {
                    try
                    {
                        net += Convert.ToInt64(dr2["net_amount"].ToString());
                    }
                    catch (Exception e)
                    {

                    }
                }
                //crm = new CommonReportModel
                //{
                //    RowId = RowCnt++,
                //    HRF = "F",
                //    grpclmn = "<span style='color:#C8EAFB'>~</span> Total:<span></span> <span></span>  <b >" +
                //    "   " + er_basic + " <span></span> " + all_amount + "  " + er_da + " <span></span> " + er_cca + " <span></span> " + er_hra + " <span></span> " + spl_allw + " <span></span> " + gross_amount + " <span></span> " + dd_provident_fund + " <span></span> " + VPFIT + " <span></span> " + TOTDED + " <span></span> " + net + "   </b></span>",
                //    // column2 = "" + all_amount + "";

                //};
                if (er_basic != 0)
                {
                    crm = new CommonReportModel
                    {

                        RowId = RowCnt++,
                        HRF = "F",
                        grpclmn= "Total:",
                        column5 = ReportColConvertToDecimal(er_basic.ToString()),
                        //column7 = Incr[0].ToString(),
                        //column8 = Allw[0].ToString(),
                        column6 = ReportColConvertToDecimal(er_da.ToString()),
                        column7 = ReportColConvertToDecimal(er_cca.ToString()),
                        column8 = ReportColConvertToDecimal(er_hra.ToString()),
                        column9 = ReportColConvertToDecimal(spl_da.ToString()),
                        column10 = ReportColConvertToDecimal(spl_allw.ToString()),
                        column11 = ReportColConvertToDecimal(dd_provident_fund.ToString()),
                        column12 = ReportColConvertToDecimal(VPFIT.ToString()),
                        column13 = ReportColConvertToDecimal(gross_amount.ToString()),
                        //column17 = It[0].ToString(),
                        column14 = ReportColConvertToDecimal(TOTDED.ToString()),
                        column15 = ReportColConvertToDecimal(net.ToString()),

                        //grpclmn = "<span style='color:#C8EAFB'>~</span> Total:<span></span> <span></span>  <b >" +
                        ////"   " + er_basic + " <span></span> " + all_amount + "  " + er_da + " <span></span> " + er_cca + " <span></span> " + er_hra + " <span></span> " + spl_allw + " <span></span> " + gross_amount + " <span></span> " + dd_provident_fund + " <span></span> " + VPFIT + " <span></span> " + TOTDED + " <span></span> " + net + "   </b></span>",
                        //"  <span style='color:#eef8fd'>----------------------------------------------------------</span>" +
                        //" " + er_basic + "  <span style='color:#eef8fd'>------</span>" +
                        ////all_amount
                        //// " " + all_amount + " <span style='color:#eef8fd'>------</span>  " +
                        //" " + er_da + " <span style='color:#eef8fd'>------</span>" +
                        //" " + er_cca + " <span style='color:#eef8fd'>-----</span>  " +
                        //" " + er_hra + " <span style='color:#eef8fd'>-----</span> " +
                        //" " + spl_da + " <span style='color:#eef8fd'>------</span> " +
                        //" " + spl_allw + " <span style='color:#eef8fd'>-----</span>" +
                        //" " + dd_provident_fund + " <span style='color:#eef8fd'>----</span> " +
                        //" " + VPFIT + " <span style='color:#eef8fd'>-------</span>" +
                        //" " + gross_amount + " <span style='color:#eef8fd'>------</span>" +
                        //"  " + TOTDED + " <span style='color:#eef8fd'>-------</span>" +
                        //"  " + net + "    </b></span>",

                    };
                    lst.Add(crm);
                }

            }

                return lst;
        }
        private CommonReportModel getTotal(string endate, string code, DataTable dt)
        {
            var val = dt.Rows.Cast<DataRow>()
                .Where(x => x["emp_code"].ToString() == code && x["endate"].ToString() == endate)
                .Select(x => new { tot = x["endate"].ToString(), cnt = x["endate"].ToString() }).FirstOrDefault();

            var tot = new CommonReportModel
            {
                RowId = 0,
                HRF = "F",
                footer = "<span style='color:#eef8fd'>^</span><b>Total :</b> " + endate
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

    }
}
