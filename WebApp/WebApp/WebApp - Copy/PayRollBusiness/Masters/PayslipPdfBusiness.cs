using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Masters
{
    public class PayslipPdfBusiness : BusinessBase
    {
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();

        public PayslipPdfBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public string AddZerosAfterDecimal(string amount)
        {
            float number1 = float.Parse(amount);
            double number = double.Parse(amount, CultureInfo.InvariantCulture);
            string ret = "";
            ret = String.Format("{0:0.00}", number);
            return ret;
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
        public async Task<string> getEmpCode(string EmpCode)
        {

            string emp_codes = "0";
            string qrySel = "SELECT emp_code " +
                             "FROM pr_emp_Payslip " +

                               " where active=1;";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);

            PayslipPdf paypdf = new PayslipPdf();
            foreach (DataRow dr in dt.Rows)
            {
                //string str;
                emp_codes += dr["emp_code"] + "," + " ";
            }
            return emp_codes;
        }
        DataTable qryselectid;
        //Download PDF
        public async Task<PayslipPdf> getPdfDetails(string EmpCode)
        {
            PayslipPdf paypdf = new PayslipPdf();

            string empcode = "";

            if (EmpCode == " ")
            {
                empcode = "0";

            }
            else
            {
                int pid = 0;

                string qrygetid = "select id from pr_emp_payslip where id=" + Convert.ToInt32(EmpCode) + ";";
                try
                {
                    qryselectid = await _sha.Get_Table_FromQry(qrygetid);
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }

                //else
                //{
                //     qryselectid = await _sha.Get_Table_FromQry("select id from pr_emp_payslip where emp_code='" + Convert.ToInt32(EmpCode) + "' and spl_type='" + pdftypes + "';");
                //}

                if (qryselectid.Rows.Count > 0)
                {
                    DataRow psid = qryselectid.Rows[0];
                    pid = Convert.ToInt32(psid["id"].ToString());
                }
                string qrySel = "SELECT format(g.fm,'MMMM yyyy') as fm,e.shortname,g.emp_code,b.Name,case when g.designation = 'Watchman' then 'Attender' else g.designation end as Description,g.er_basic,g.er_da,g.er_cca," +
                                 "g.er_hra,g.er_interim_relief,g.spl_da,g.spl_allw,g.gross_amount,g.deductions_amount," +
                                 "g.Working_days,g.net_amount,g.dd_provident_fund,g.er_telangana_inc,g.dd_club_subscription," +
                                 "g.dd_income_tax,g.dd_prof_tax,g.dd_telangana_officers_assn,g.err_ceoallw, " +
                                 " format(e.DoJ,'dd-MM-yyyy') as DoJ ,format(e.RetirementDate,'dd-MM-yyyy') as RetirementDate,gen.pf_no,g.NPS " +
                                 "FROM pr_emp_payslip g " +
                                  "JOIN Employees e ON e.id = g.emp_id" +
                                   " left outer JOIN pr_emp_general gen ON gen.emp_id = e.id" +
                                  " JOIN Branches b ON b.Id = e.Branch" +
                                  " JOIN Designations d ON d.Id=e.CurrentDesignation" +
                                   " where g.id=" + pid + ";";
                //" where emp_code='" + Convert.ToInt32(EmpCode) + "' and spl_type='" + pdftypes + "';";

                string qrySel2 = "select distinct g.all_name,g.all_amount " +
                                "FROM pr_emp_payslip_allowance g" +
                                //" JOIN pr_emp_payslip ps ON ps.id=g.payslip_mid" +
                                " where payslip_mid=" + pid + " and  all_name!='Employee TDS' ;";
                //" where emp_code='" + Convert.ToInt32(EmpCode) + "';";

                string qrySel3 = "select dd_name ,sum(dd_amount) as amount from pr_emp_payslip_deductions  " +
                                   " where payslip_mid=" + pid + " and dd_name in ('HFC', 'LIC') group by dd_name;";

                string qrySel4 = "select dd_name ,dd_amount  from pr_emp_payslip_deductions   " +
                    "where payslip_mid=" + pid + " and dd_type!='Loan' and dd_name!='LIC' AND dd_name!='HFC' ; ";

                string qrySel5 = "select dd_name ,dd_amount  from pr_emp_payslip_deductions   " +
                  "where payslip_mid=" + pid + " and dd_type='Loan' and dd_name!='LIC' AND dd_name!='HFC' ; ";
                //" JOIN pr_emp_payslip ps ON ps.id=g.payslip_mid" +

                //" where emp_code='" + Convert.ToInt32(EmpCode) + "';";


                //string queryl6 = "select fm from pr_emp_payslip"+
                //                  " where id = " + pid + "";


                DataSet ds = await _sha.Get_MultiTables_FromQry(qrySel + qrySel2 + qrySel3 + qrySel4 + qrySel5);
                var Empdetails = ds.Tables[0];
                var dtdeductiondetails = ds.Tables[1];
                var dtspclallowance = ds.Tables[2];
                var dtdeduct = ds.Tables[3];
                var dtdeductloan = ds.Tables[4];
                //var dtdeduct1 = ds.Tables[4];



                try
                {
                    IList<PayslipPdfAlwDed> allwList = new List<PayslipPdfAlwDed>();

                    paypdf.Allowences = allwList;

                    IList<PayslipPdfAlwDed> grosswrkingList = new List<PayslipPdfAlwDed>();

                    paypdf.grossworking = grosswrkingList;

                    IList<PayslipPdfAlwDed> netwrkingList = new List<PayslipPdfAlwDed>();

                    paypdf.netwrkingList = netwrkingList;

                    //Deductions list
                    IList<PayslipPdfAlwDed> dedList = new List<PayslipPdfAlwDed>();

                    paypdf.Deductions = dedList;



                    //employee data
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow drEmp = ds.Tables[0].Rows[0];
                        paypdf.EmpCode = drEmp["emp_code"].ToString();
                        paypdf.EmpName = drEmp["shortname"].ToString();
                        paypdf.Designation = drEmp["Description"].ToString();
                        paypdf.Branch = drEmp["Name"].ToString();
                        paypdf.Fm = drEmp["fm"].ToString();
                        paypdf.DOJ = drEmp["DoJ"].ToString();
                        paypdf.RetirementDate = drEmp["RetirementDate"].ToString();
                        paypdf.PfNo = drEmp["pf_no"].ToString();

                        //allowences - basics


                        if (Convert.ToDecimal(drEmp["er_basic"]) > 0 || Convert.ToDecimal(drEmp["er_basic"]) <= 0)
                        {
                            string basic = ReportColConvertToDecimal(drEmp["er_basic"].ToString());


                            allwList.Add(new PayslipPdfAlwDed { Perticular = "Basic", Amount = basic });
                        }
                        if (Convert.ToDecimal(drEmp["er_da"]) > 0 || Convert.ToDecimal(drEmp["er_da"])< 0)
                        {
                            string da = ReportColConvertToDecimal(drEmp["er_da"].ToString());
                            allwList.Add(new PayslipPdfAlwDed { Perticular = "DA", Amount = da });
                        }
                        if (Convert.ToDecimal(drEmp["er_cca"]) > 0 || Convert.ToDecimal(drEmp["er_cca"]) < 0)
                        {
                            string cca = ReportColConvertToDecimal(drEmp["er_cca"].ToString());
                            allwList.Add(new PayslipPdfAlwDed { Perticular = "CCA", Amount = cca });
                        }

                        if (Convert.ToDecimal(drEmp["er_hra"]) > 0 || Convert.ToDecimal(drEmp["er_hra"]) < 0)
                        {
                            string hra = ReportColConvertToDecimal(drEmp["er_hra"].ToString());
                            allwList.Add(new PayslipPdfAlwDed { Perticular = "HRA", Amount = hra });
                        }
                        if (Convert.ToDecimal(drEmp["er_interim_relief"]) > 0 || Convert.ToDecimal(drEmp["er_interim_relief"]) < 0)
                        {
                            string interim = ReportColConvertToDecimal(drEmp["er_interim_relief"].ToString());
                            allwList.Add(new PayslipPdfAlwDed { Perticular = "Interim Relief", Amount = interim });
                        }
                        if (Convert.ToDecimal(drEmp["spl_da"]) > 0 || Convert.ToDecimal(drEmp["spl_da"]) <0)
                        {
                            string splda = ReportColConvertToDecimal(drEmp["spl_da"].ToString());
                            allwList.Add(new PayslipPdfAlwDed { Perticular = "Spcl. DA", Amount = splda });
                        }
                        if (Convert.ToDecimal(drEmp["spl_allw"]) > 0 || Convert.ToDecimal(drEmp["spl_allw"]) < 0)
                        {
                            string splall = ReportColConvertToDecimal(drEmp["spl_allw"].ToString());
                            allwList.Add(new PayslipPdfAlwDed { Perticular = "Spl. Allow ", Amount = splall });
                        }
                        if (Convert.ToDecimal(drEmp["err_ceoallw"]) > 0 || Convert.ToDecimal(drEmp["err_ceoallw"]) < 0)
                        {
                            string splall = ReportColConvertToDecimal(drEmp["err_ceoallw"].ToString());
                            allwList.Add(new PayslipPdfAlwDed { Perticular = "CEO Allowance ", Amount = splall });
                        }
                        if (Convert.ToDecimal(drEmp["dd_provident_fund"]) > 0 || Convert.ToDecimal(drEmp["dd_provident_fund"]) < 0)
                        {
                            string providentfund = ReportColConvertToDecimal(drEmp["dd_provident_fund"].ToString());
                            dedList.Add(new PayslipPdfAlwDed { Perticular = "Provident Fund", Amount = providentfund });
                        }
                        if (Convert.ToDecimal(drEmp["NPS"]) > 0 || Convert.ToDecimal(drEmp["NPS"]) < 0)
                        {
                            string NPS = ReportColConvertToDecimal(drEmp["NPS"].ToString());
                            dedList.Add(new PayslipPdfAlwDed { Perticular = "NPS", Amount = NPS });
                        }
                        if (Convert.ToDecimal(drEmp["dd_income_tax"]) > 0 || Convert.ToDecimal(drEmp["dd_income_tax"]) < 0)
                        {
                            string incometax = ReportColConvertToDecimal(drEmp["dd_income_tax"].ToString());
                            dedList.Add(new PayslipPdfAlwDed { Perticular = "Income tax", Amount = incometax });
                        }
                        if (Convert.ToDecimal(drEmp["dd_prof_tax"]) > 0 || Convert.ToDecimal(drEmp["dd_prof_tax"]) < 0)
                        {
                            string proftax = ReportColConvertToDecimal(drEmp["dd_prof_tax"].ToString());
                            dedList.Add(new PayslipPdfAlwDed { Perticular = "Professional Tax", Amount = proftax });

                        }
                        if (Convert.ToDecimal(drEmp["dd_club_subscription"]) > 0 || Convert.ToDecimal(drEmp["dd_club_subscription"]) < 0)
                        {
                            string clubsubscription = ReportColConvertToDecimal(drEmp["dd_club_subscription"].ToString());
                            dedList.Add(new PayslipPdfAlwDed { Perticular = "Club Subsriptions", Amount = clubsubscription });
                        }
                        
                        if (Convert.ToDecimal(drEmp["dd_telangana_officers_assn"]) > 0 || Convert.ToDecimal(drEmp["dd_telangana_officers_assn"]) < 0)
                        {
                            string telanganofficers = ReportColConvertToDecimal(drEmp["dd_telangana_officers_assn"].ToString());
                            dedList.Add(new PayslipPdfAlwDed { Perticular = "Telangana Officers Association", Amount = telanganofficers });
                        }
                        if (Convert.ToDecimal(drEmp["er_telangana_inc"]) > 0 || Convert.ToDecimal(drEmp["er_telangana_inc"]) < 0)
                        {
                            string telanganainc = ReportColConvertToDecimal(drEmp["er_telangana_inc"].ToString());
                            allwList.Add(new PayslipPdfAlwDed { Perticular = "Telangana Increment", Amount = telanganainc });
                        }
                        //if (Convert.ToDecimal(drEmp["gross_amount"]) > 0)
                        //{
                        paypdf.grossamount = ReportColConvertToDecimal( drEmp["gross_amount"].ToString());
                        paypdf.deductionsamount = ReportColConvertToDecimal(drEmp["deductions_amount"].ToString());
                        paypdf.netamount = ReportColConvertToDecimal(drEmp["net_amount"].ToString());
                        paypdf.workingdays = drEmp["Working_days"].ToString();
                        string netamount= AddZerosAfterDecimal(drEmp["net_amount"].ToString());
                        netwrkingList.Add(new PayslipPdfAlwDed { Perticular = "Net Amount", Amount = netamount });
                        
                    }

                    //other allowences
                    foreach (DataRow drAllw in ds.Tables[1].Rows)
                    {
                        if (Convert.ToDecimal(drAllw["all_amount"]) > 0 || Convert.ToDecimal(drAllw["all_amount"]) < 0)
                        {
                            string allamount = ReportColConvertToDecimal(drAllw["all_amount"].ToString());
                            allwList.Add(new PayslipPdfAlwDed { Perticular = drAllw["all_name"].ToString(), Amount = allamount });

                        }
                    }

                    //Getting the data for hfc/lic 
                    foreach (DataRow drDed in ds.Tables[2].Rows)
                    {
                        if (Convert.ToDecimal(drDed["amount"]) > 0 || Convert.ToDecimal(drDed["amount"]) < 0)
                        {
                            string allamount = ReportColConvertToDecimal(drDed["amount"].ToString());
                            dedList.Add(new PayslipPdfAlwDed { Perticular = drDed["dd_name"].ToString(), Amount = allamount });

                        }

                    }
                    //Getting the data for all deductions including  loans
                    foreach (DataRow drDed1 in ds.Tables[3].Rows)
                    {
                        if (Convert.ToDecimal(drDed1["dd_amount"]) > 0 || Convert.ToDecimal(drDed1["dd_amount"]) < 0)
                        {
                            string allamount = ReportColConvertToDecimal(drDed1["dd_amount"].ToString());
                            string allNAME = drDed1["dd_name"].ToString();
                            if (allNAME == "LIC")
                            {

                            }
                            else if (allNAME == "HFC")
                            {

                            }
                            else if (allNAME == "VPF")
                            {

                            }
                            //newly added on 8/5/2020 by chaitanya
                            else if (allNAME == "Club Subscription")
                            {

                            }
                            else if (allNAME == "CCS - HYD")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            else if (allNAME == "GSLI")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                                else if (allNAME == "ANDHRA BANK, RAMANTHPUR")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            else if (allNAME == "APCCADB - EMP CCS")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            else if (allNAME == "PERSONAL LOAN")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            else if (allNAME == "TELANGANA EMP UNION")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            else if(allNAME== "SC/ST Assn ST Subscription")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            else if(allNAME== "BANKS EMP ASSN TELANGANA")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            else if(allNAME== "VPF Deduction")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            else if (allNAME == "NON PRIORITY PERSONAL LOAN")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            else if (allNAME == "NPS")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            
                            else if (allNAME == "COD_INS_PRM")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            //end
                            //checking the dedList contains the "Telangana Officers Association" or not if exists then not adding to list else adding to list //10/07/2020
                            string telanganaofficeassn = "";
                            string vpfded = "";
                            string bankempasstel = "";
                            string nppl = "";
                            string nps = "";
                            if (dedList.Count>0)
                            {
                                foreach(var x in dedList)
                                {
                                    var toa = x.Perticular;
                                    if(toa == "Telangana Officers Association" )
                                    {
                                        telanganaofficeassn = toa;
                                    }
                                    if(toa== "VPF Deduction")
                                    {
                                        vpfded = toa;
                                    }
                                    if(toa== "BANKS EMP ASSN TELANGANA")
                                    {
                                        bankempasstel = toa;
                                    }
                                    if (toa == "NON PRIORITY PERSONAL LOAN")
                                    {
                                        nppl = toa;
                                    }
                                    if (toa == "NPS")
                                    {
                                        nps = toa;
                                    }
                                }
                            }
                            if(telanganaofficeassn=="" && allNAME == "TELANGANA OFFICERS ASSN")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            if(vpfded=="" && allNAME == "VPF Deduction")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            if(bankempasstel == "" && allNAME == "BANKS EMP ASSN TELANGANA")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            if (nppl == "" && allNAME == "NON PRIORITY PERSONAL LOAN")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            if (nps == "" && allNAME == "NPS")
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            }
                            //end

                            //else if (allNAME == "TELANGANA OFFICERS ASSN" && dedList.Equals("Telangana Officers Association"))
                            //{
                            //    dedList.Add(new PayslipPdfAlwDed { Perticular = drDed1["dd_name"].ToString(), Amount = allamount });
                            //}



                        }

                    }
                    foreach (DataRow drDedloans in ds.Tables[4].Rows)
                    {
                        if (Convert.ToDecimal(drDedloans["dd_amount"]) > 0 || Convert.ToDecimal(drDedloans["dd_amount"]) < 0)
                        {
                            string allamount = ReportColConvertToDecimal(drDedloans["dd_amount"].ToString());
                            string allNAME = drDedloans["dd_name"].ToString();
                            if (allNAME == "LIC")
                            {

                            }
                            else if (allNAME == "HFC")
                            {

                            }
                            else if (allNAME == "VPF")
                            {

                            }
                            //newly added on 8/5/2020 by chaitanya
                            else if (allNAME == "Club Subscription")
                            {

                            }
                            //end
                            else
                            {
                                dedList.Add(new PayslipPdfAlwDed { Perticular = drDedloans["dd_name"].ToString(), Amount = allamount });
                            }



                        }

                    }
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                }

            }
            return paypdf;
        }
        //SendPdf Through Mail
        public async Task<PayslipPdf> SendPdfDetails(string EmpCode)
        {
            string empcode = "";

            if (EmpCode == "")
            {
                empcode = "0";
            }

            string sendmailling = "SELECT e.OfficalEmailId " +
                                  "FROM Employees e " +
                                  "JOIN pr_emp_payslip g ON e.EmpId = g.emp_code" +
                                    " where g.id='" + Convert.ToInt32(EmpCode) + "';";

            string sendmailling2 = "SELECT e.PersonalEmailId " +
                                  "FROM Employees e " +
                                  "JOIN pr_emp_payslip g ON e.EmpId = g.emp_code" +
                                    " where g.id='" + Convert.ToInt32(EmpCode) + "';";


            DataSet ds = await _sha.Get_MultiTables_FromQry(sendmailling + sendmailling2);
            var mail = ds.Tables[0];


            PayslipPdf paypdf = new PayslipPdf();

            try
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow drmail = ds.Tables[0].Rows[0];
                    paypdf.PersonalEmailId = drmail["OfficalEmailId"].ToString();

                }
            }


            catch (Exception ex)
            {
                var message = ex.Message;
            }


            return paypdf;
        }
        //get data from pr_emp_payslip, pr_emp_payslip_allowance, pr_emp_payslip_deductions
        public async Task<IList<Payslippdf>> GetPayslipReportdata(string emp_code, string pdftypes, string Month)
        {
            IList<Payslippdf> lst = new List<Payslippdf>();
            Payslippdf crm = new Payslippdf();
            String query;
            string strMonth = "";
            string strYear = "";
            DateTime str = new DateTime();
            if (pdftypes != "")
            {

            }
            try
            {

                if (Month != "")
                {
                    str = Convert.ToDateTime(Month);
                    strMonth = str.ToString("MM");
                    strYear = str.ToString("yyyy");
                }

                //(755,866)
                string[] arr_empcodes = emp_code.Split(',');
                string multiple_empcode = "";
                foreach (string mec in arr_empcodes)
                {
                    multiple_empcode = multiple_empcode + mec + ",";
                }
                multiple_empcode = multiple_empcode.Substring(0, multiple_empcode.LastIndexOf(","));
                string pdftype = "";
                //('stopsalary','Encashment','Regular')
                if (pdftypes != "")
                {
                    string[] split_pdftype = pdftypes.Split(',');
                    pdftype = "'";
                    foreach (string pdt in split_pdftype)
                    {
                        pdftype = pdftype + pdt + "','";
                    }
                    pdftype = pdftype.Substring(0, pdftype.LastIndexOf(",'"));
                }
                DateTime CheckMonth = Convert.ToDateTime(System.Web.Configuration.WebConfigurationManager.AppSettings["oldmonths"].ToString());
                //string chkMthYear = CheckMonth.ToString("MMM-yyyy");
                //DateTime va = Convert.ToDateTime(chkMthYear);
                //DateTime ve = Convert.ToDateTime(Month);

                if ((str.Year >= CheckMonth.Year && str.Month >= CheckMonth.Month) || ((str.Year > CheckMonth.Year && str.Month < CheckMonth.Month)))
                {

                    //string empcode = "";
                    if (emp_code == "")
                    {
                        query = "SELECT ps.id as Id,e.shortname,ps.emp_code,ps.designation as Description,ps.gross_amount,ps.deductions_amount,ps.spl_type,ps.net_amount FROM pr_emp_payslip ps " +


                                       "JOIN Employees e ON e.EmpId = ps.emp_code " +
                                       "JOIN Branches b ON b.Id = e.Branch " +
                                       "JOIN Designations d ON d.Id = e.CurrentDesignation " +
                                        " where emp_code=0;";

                    }
                    else if (emp_code == "All")
                    {
                        if (emp_code == "All" && pdftypes == "")
                        {
                            query = "SELECT ps.id as Id,e.shortname,ps.emp_code,case when ps.designation = 'Watchman' then 'Attender' else ps.designation end as Description,ps.gross_amount,ps.deductions_amount,ps.spl_type,ps.net_amount FROM pr_emp_payslip ps " +


                                                               "JOIN Employees e ON e.EmpId = ps.emp_code " +
                                                               "JOIN Branches b ON b.Id = e.Branch " +
                                                               "JOIN Designations d ON d.Id = e.CurrentDesignation " +
                                                                " where format(fm,'MM')='" + strMonth + "' and format(fm,'yyyy')='" + strYear + "'";
                        }
                        else
                        {
                            query = "SELECT ps.id as Id,e.shortname,ps.emp_code,case when ps.designation = 'Watchman' then 'Attender' else ps.designation end as Description,ps.gross_amount,ps.deductions_amount,ps.spl_type,ps.net_amount FROM pr_emp_payslip ps " +


                                       "JOIN Employees e ON e.EmpId = ps.emp_code " +
                                       "JOIN Branches b ON b.Id = e.Branch " +
                                       "JOIN Designations d ON d.Id = e.CurrentDesignation " +
                                        " where format(fm,'MM')='" + strMonth + "' and format(fm,'yyyy')='" + strYear + "' and spl_type in (" + pdftype + ")";
                        }


                    }
                    else
                    {

                        query = "SELECT ps.id as Id,e.shortname,ps.emp_code,case when ps.designation = 'Watchman' then 'Attender' else ps.designation end as Description,ps.gross_amount,ps.deductions_amount,ps.spl_type,ps.net_amount FROM pr_emp_payslip ps " +


                          "JOIN Employees e ON e.EmpId = ps.emp_code " +
                          "JOIN Branches b ON b.Id = e.Branch " +
                          "JOIN Designations d ON d.Id = e.CurrentDesignation " +
                           " where emp_code in (" + multiple_empcode + ") and spl_type in (" + pdftype + ") and format(fm,'MM')='" + strMonth + "' and format(fm,'yyyy')='" + strYear + "';";
                    }


                    //string query1 = "select  sum(er_basic) as er_basic from pr_emp_payslip where emp_code = (555)";

                    //DataSet ds = await _sha.Get_MultiTables_FromQry(query + query1);
                    //DataTable dtALL = ds.Tables[0];
                    //DataTable dtTot = ds.Tables[1];
                    DataTable dt = await _sha.Get_Table_FromQry(query);
                    string ls = "";

                    foreach (DataRow dr in dt.Rows)
                    {
                        // empcode = dr["emp_code"].ToString();

                        crm = new Payslippdf
                        {
                            Id = dr["Id"].ToString(),
                            emp_code = dr["emp_code"].ToString(),
                            shortname = dr["shortname"].ToString(),
                            Description = dr["Description"].ToString(),
                           // gross_amount = AddZerosAfterDecimal(dr["gross_amount"].ToString()),
                            gross_amount = ReportColConvertToDecimalandAlign(dr["gross_amount"].ToString()),

                            deductions_amount = ReportColConvertToDecimalandAlign(dr["deductions_amount"].ToString()),
                            net_amount = ReportColConvertToDecimalandAlign(dr["net_amount"].ToString()),
                            spl_type = dr["spl_type"].ToString(),

                        };

                        lst.Add(crm);
                    }



                }

            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return lst;

        }

        public string ReportColConvertToDecimalandAlign(string value)
        {
            string sRet = "";
            if (value == "")
            {
                value = "0";
            }
            decimal Drvalue = Convert.ToDecimal(value.ToString()) + 0.00M;
            decimal DPT = Convert.ToDecimal(String.Format("{0:0.00}", Drvalue));
            string NwDPT = String.Format("{0:n}", DPT);

            sRet += "<span style='float:right'>" + NwDPT + "</span>";

            return sRet;
        }
        public async Task<IList<Payslippdf>> GetPayslipReportdatahrms(string emp_code, string pdftypes, string Month)
        {
            int EmpCode = _LoginCredential.EmpCode;
            IList<Payslippdf> lst = new List<Payslippdf>();
            Payslippdf crm = new Payslippdf();
            int Eyears = 0001;
            int Fyears = 0001;

            if (emp_code == "^2")
            {
                EmpCode = 0;
            }

            else if (emp_code == "^" || Month=="t")
            {
                emp_code = "0";
                Fyears = 0001;
                Eyears = 0001;
            }
            else if (emp_code == "")
            {
                emp_code = "0";
                Fyears = 0001;
                Eyears = 0001;
            }
            else if (emp_code == " ")
            {
                emp_code = "0";
                Fyears = 0001;
                Eyears = 0001;
            }
            else
            {
                Eyears = Int32.Parse(Month);
                Fyears = Int32.Parse(Month) - 1;
            }
            String query;
            string strMonth = "";
            string strYear = "";
            DateTime str = new DateTime();




            if (pdftypes != "")
            {

            }
            try
            {                
                query = "SELECT ps.id as Id,e.shortname,FORMAT(ps.fm, 'MMM yyyy') as emp_code,case when ps.designation = 'Watchman' then 'Attender' else ps.designation end as Description,ps.gross_amount,ps.deductions_amount,ps.spl_type,ps.net_amount FROM pr_emp_payslip ps " +
              "JOIN Employees e ON e.EmpId = ps.emp_code " +
              "JOIN Branches b ON b.Id = e.Branch " +
              "JOIN Designations d ON d.Id = e.CurrentDesignation " +
               " where emp_code in (" + EmpCode + ") and fm between DATEFROMPARTS (" + Fyears + ", 04, 01 ) and DATEFROMPARTS (" + Eyears + ", 03, 31 ) and final_process='True' order by ps.fm desc  ";
                DataTable dt = await _sha.Get_Table_FromQry(query);
                string ls = "";

                foreach (DataRow dr in dt.Rows)
                {
                    // empcode = dr["emp_code"].ToString();

                    crm = new Payslippdf
                    {
                        Id = dr["Id"].ToString(),
                        emp_code = dr["emp_code"].ToString(),
                        shortname = dr["shortname"].ToString(),
                        Description = dr["Description"].ToString(),
                        gross_amount = AddZerosAfterDecimal(dr["gross_amount"].ToString()),
                        deductions_amount = AddZerosAfterDecimal(dr["deductions_amount"].ToString()),
                        net_amount = AddZerosAfterDecimal(dr["net_amount"].ToString()),
                        spl_type = dr["spl_type"].ToString(),

                    };

                    lst.Add(crm);
                }





            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return lst;

        }

    }

}





