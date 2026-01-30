using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using PayrollModels.Masters;
using System.Configuration;
using System.Data;
namespace PayRollBusiness.Reports
{
   public class TsheetReportBusiness: BusinessBase
    {
        public TsheetReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

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
        public async Task<IList<TsheetModel>> GetTsheetdata(string month,string RegEmp,string SupEmp)
        {
            string codes = "";
            string branch = "";
            string oldbranch = "";
            string encashment = PrConstants.ENCASHMENT;
            IList<PayslipPdfAlwDed> allwList = new List<PayslipPdfAlwDed>();
            PayslipPdf paypdf = new PayslipPdf();
            IList<TsheetModel> lst = new List<TsheetModel>();
            TsheetModel crm1 = new TsheetModel();
            TsheetModel crm = new TsheetModel();
            paypdf.Allowences = allwList;
            string qry = "";
           string general= PrConstants.REGULAR;
           string adhoc = PrConstants.ADHOC;
            DateTime str = Convert.ToDateTime(month);
            string str1 = str.ToString("yyyy-MM-dd");
            int RowCnt = 0;
            int SNo = 1;
            string all1 = "";
            string all2 = "";
            string empid = "";
            string oldempid = "";
            string all3 = "";
            string medall = "";
            string splall = "";
            qry = "select case when b.Name='OtherBranch' then d.name else b.Name end as grpcol,ps.emp_code,e.shortname,ps.er_basic as Basic,ps.er_da as da,ps.er_cca as cca,ps.er_hra as hra," +
                "ps.er_interim_relief as IntirimRelief,ps.er_telangana_inc as TelanganaIncrement,ps.spl_allw as SplAllw," +
                "ps.spl_da as SplDa,ps.dd_provident_fund as ProvidentFund,ps.dd_income_tax as IncomeTax,dd_prof_tax as ProfTax," +
                "ps.dd_club_subscription as clubsubscription,ps.id,ps.dd_telangana_officers_assn as TelanganaOfficersAssn," +
                "ps.gross_amount,ps.net_amount,ps.deductions_amount FROM pr_emp_payslip ps " +
                "join employees e on e.empid = ps.emp_code " +
                "join branches b on b.id=e.branch " +
                "join Departments d on d.id = e.Department " +"";
            
             
                //"declare @tmp varchar(250) SET @tmp = '' select @tmp = @tmp + all_name + ', ' + convert(varchar, all_amount) + ',' from pr_emp_payslip_allowance select SUBSTRING(@tmp, 0, LEN(@tmp))";


                if (str1 != "" && RegEmp != "" && RegEmp != "undefined")
                {
                    qry += " where ps.fm ='" + str1 + "' and spl_type in( '"+ general + "' , '" + adhoc + "') order by b.Name";

                }
                if (str1 != "" && SupEmp != "" && RegEmp == "undefined")
                {
                    qry += " where ps.fm ='" + str1 + "' and ps.spl_type='" + adhoc + "'  order by b.Name";
                }
                DataSet ds = await _sha.Get_MultiTables_FromQry(qry);
            

                var payslip = ds.Tables[0];
            // var payallow = ds.Tables[1];


               
                
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                codes = "";
                codes += dr["emp_code"].ToString();
                string query = "select  case when b.Name='OtherBranch' then d.name else b.Name end as grpcol, " +
               "ps.emp_code, pal.all_name, case  when (pal.all_amount<=0 or pal.all_amount is null) then 0 else pal.all_amount " +
               "end as all_amount from pr_emp_payslip_allowance pal join pr_emp_payslip " +
               "ps on ps.emp_code = pal.emp_code  join Employees e on e.empid = ps.emp_code " +
               "join branches b on b.id = e.branch join Departments d on d.id = e.Department ";


                if (RegEmp != "" && RegEmp != "undefined")
                {
                    query += " where  pal.emp_code="+codes+" and ps.fm ='" + str1 + "' and  pal.payslip_mid =" + dr["id"].ToString() + " ";

                }
                if (SupEmp != "" && RegEmp == "undefined")
                {
                    query += " where  pal.emp_code=" + codes + " and ps.fm ='" + str1 + "' and  ps.spl_type='" + adhoc + "'";
                }
                DataTable dt = await _sha.Get_Table_FromQry(query);

                branch = dr["grpcol"].ToString();
                if (oldbranch != branch)
                {
                    crm1 = new TsheetModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        column1 = "<span style='color:#C8EAFB'>~</span><b>Branch: " + dr["grpcol"].ToString() + " </b></span>",
                    };
                    lst.Add(crm1);

                }
                oldbranch = dr["grpcol"].ToString();
              
                crm = new TsheetModel
                {
                    RowId = RowCnt++,
                    HRF = "R",
                    // column1= SNo++.ToString(),
                    // grpclmn = dt.Rows[0]["column1"].ToString(),
                    //column2 = dt1.Rows[0]["column1"].ToString(),
                    column3 = ReportColConvertToDecimal( dr["Basic"].ToString()),
                    column4 = ReportColConvertToDecimal(dr["da"].ToString()),
                    column5 = ReportColConvertToDecimal( Math.Round(Convert.ToDecimal(dr["cca"]), 2).ToString()),
                    column6 = ReportColConvertToDecimal( Math.Round(Convert.ToDecimal(dr["hra"]), 2).ToString()),
                    column7 = ReportColConvertToDecimal( dr["IntirimRelief"].ToString()),
                    column8 = ReportColConvertToDecimal( dr["TelanganaIncrement"].ToString()),
                    column9 = ReportColConvertToDecimal(dr["SplAllw"].ToString()),
                    column10 = ReportColConvertToDecimal( dr["SplDa"].ToString()),
                    column11 = ReportColConvertToDecimal( dr["ProvidentFund"].ToString()),
                    column12 = ReportColConvertToDecimal( dr["IncomeTax"].ToString()),
                    column13 = ReportColConvertToDecimal( dr["ProfTax"].ToString()),
                    column14 = ReportColConvertToDecimal( dr["clubsubscription"].ToString()),
                    column15 = ReportColConvertToDecimal( dr["TelanganaOfficersAssn"].ToString()),
                    column16 = ReportColConvertToDecimal(dr["gross_amount"].ToString()),
                    column17 = ReportColConvertToDecimal( dr["net_amount"].ToString()),
                    column18 = ReportColConvertToDecimal( dr["deductions_amount"].ToString()),
                    column1 = dr["emp_code"].ToString(),
                    column20 = dr["shortname"].ToString()
                    // RowId = Convert.ToInt32(dr["row_no"]),
                    // grpclmn = "<span style='color:#C8EAFB'>~</span>" + dr["column1"].ToString() + "</span>",
                };

                foreach (DataRow dd in dt.Rows)
                {
                   // all1 = "";
                   // all2 = "";
                    //   empid = dr["emp_code"].ToString();
                    //if (oldempid != empid)
                    //{
                    if (dd["all_name"].ToString() == "Physically Handicapped")
                    {
                       // all1 = dd["all_amount"].ToString();
                        crm.column21 = ReportColConvertToDecimal( dd["all_amount"].ToString());

                    }
                    if (dd["all_name"].ToString() == "SPL Care Taker")
                    {
                        crm.SPLCareTaker = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Medical Allowance")
                    {
                         crm.MedAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Cashier")
                    {
                        crm.SPLCashier = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Special Increment")
                    {
                        crm.SplIncr = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Stagnation Increments")
                    {
                        crm.StagIncr = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Annual Increment")
                    {
                        crm.AnlIncr = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "CAIIB Increment")
                    {
                        crm.Caib = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "JAIIB Increment")
                    {
                        crm.Jaib = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Employee Tds")
                    {
                        crm.EmployeeTds = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "LIC Premium")
                    {
                        crm.LICPremium = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Loss of Pay")
                    {
                        crm.LossofPay = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Special Pay")
                    {
                        crm.SpecialPay = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "System Administrator Allowance")
                    {
                        crm.SAdminAl = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Teaching Allowance")
                    {
                        crm.TeachAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Washing Allowance")
                    {
                        crm.WashAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Fixed Allowance")
                    {
                        crm.FixAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Deputation Allowance")
                    {
                        crm.DepuAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Fixed Personal Allowance")
                    {
                        crm.FPA = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "FPA-HRA Allowance")
                    {
                        crm.FPAHRA = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Interim Allowance")
                    {
                        crm.InterimAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "FPIIP")
                    {
                        crm.FPIIPAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "NPSG Allowance")
                    {
                        crm.NPSGAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Officiating Allowance")
                    {
                        crm.OfficiatingAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Personal Pay")
                    {
                        crm.PPay = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Personal Qual Allowance")
                    {
                        crm.Pqa = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Res. Attenders Allowance")
                    {
                        crm.ResAttenders = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "FP Incentive Recovery")
                    {
                        crm.FPIncentive = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Br Manager Allowance")
                    {
                        crm.BrManager = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Petrol & Paper")
                    {
                        crm.PetrolPaper = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Petrol & Paper1")
                    {
                        crm.PetrolPaper1 = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Children Education Allowance")
                    {
                        crm.CEA = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "Fest. Advance")
                    {
                        crm.Fest = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "LT PF Loan")
                    {
                        crm.LTPFLoan = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "INCENTIVE")
                    {
                        crm.INCENTIVE = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "INCENTIVE DIFF")
                    {
                        crm.INCENTIVEDIFF = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "LTC ENCASHMENT")
                    {
                        crm.LTCENCASHMENT = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Driver")
                    {
                        crm.SPLDriver = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Jamedar")
                    {
                        crm.SPLJamedar = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Key")
                    {
                        crm.SPLKey = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Split Duty -Award staff")
                    {
                        crm.SPLSplitDutyAwardstaff = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Typist")
                    {
                        crm.SPLTypist = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Watchman")
                    {
                        crm.SPLWatchman = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Stenographer")
                    {
                        crm.SPLStenographer = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Bill Collector")
                    {
                        crm.SPLBillAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Despatch")
                    {
                        crm.SPLDespatch = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Electrician")
                    {
                        crm.SPLElectrician = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Dafter")
                    {
                        crm.SPLDafter = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Cash Cabin")
                    {
                        crm.SPLCashCabin = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Telephone Operator")
                    {
                        crm.SPLTelephoneOperator = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Library")
                    {
                        crm.SPLLibrary = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Incentive")
                    {
                        crm.SPLIncentive = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Arrear Incentive")
                    {
                        crm.SPLArrearIncentive = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Conveyance")
                    {
                        crm.SPLConveyance = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Split Duty - Managers")
                    {
                        crm.SPLSplitDutyManagers = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Duplicating/xerox machine")
                    {
                        crm.SPLDuplicatingxeroxmachine = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Record room asst allowance")
                    {
                        crm.SPLRecordroomasstallowance = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Record room sub staff all")
                    {
                        crm.SPLRecordroomsubstaffall = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Receptionist allowance")
                    {
                        crm.SPLReceptionistallowance = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Spl.Alw.ACSTI")
                    {
                        crm.SPLSplAlwACSTI = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "SPL Personal Pay")
                    {
                        crm.SPLPersonalPay = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    if (dd["all_name"].ToString() == "FACULTY ALLOWANCE")
                    {
                        crm.FACULTYAlw = ReportColConvertToDecimal( dd["all_amount"].ToString());
                    }
                    //}
                }

                
                oldempid = dr["emp_code"].ToString();
                string query1 = "select  case when b.Name='OtherBranch' then d.name else b.Name end as grpcol," +
                    " ps.emp_code, pdd.dd_name,case when(pdd.dd_amount<=0 or pdd.dd_amount is null) " +
                    "then 0 else pdd.dd_amount end as dd_amount" +
                    " from pr_emp_payslip_deductions pdd join pr_emp_payslip " +
                    "ps on ps.emp_code = pdd.emp_code " +
                    "join Employees e on e.empid = ps.emp_code " +
                    "join branches b on b.id = e.branch join Departments d on d.id = e.Department";



                if (RegEmp != "" && RegEmp != "undefined")
                {
                    query1 += " where  pdd.emp_code=" + codes + " and ps.fm ='" + str1 + "' and  pdd.payslip_mid =" + dr["id"].ToString() + " ";

                }
                if (SupEmp != "" && RegEmp == "undefined")
                {
                    query1 += " where  pal.emp_code=" + codes + " and ps.fm ='" + str1 + "' and  ps.spl_type='" + adhoc + "'";
                }
                DataTable dt1 = await _sha.Get_Table_FromQry(query1);
                foreach (DataRow dd1 in dt1.Rows)
                {

                    if (dd1["dd_name"].ToString() == "APCOB-HFC-LT")
                    {
                        crm.APCOBHFCLT = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "APCOB-HFC-HO")
                    {
                        crm.APCOBHFCHO = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "VIJAYA Coop Society Deduction")
                    {
                        crm.VIJAYACoopSocietyDeduction = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "VISAKHA Coop Society Deduction")
                    {
                        crm.VISAKHACoopSocietyDeduction = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "GSLI")
                    {
                        crm.GSLI = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "Officers Assn Fund")
                    {
                        crm.OfficersAssnFund = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "Cadre Officers Assn Fund")
                    {
                        crm.CadreOfficersAssnFund = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "Club Subscription")
                    {
                        crm.ClubSubscription = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "Union Club Subscription")
                    {
                        crm.UnionClubSubscription = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "SC/ST Assn LT Subscription")
                    {
                        crm.SCSTAssnLTSubscription = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "SC/ST Assn ST Subscription")
                    {
                        crm.SCSTAssnSTSubscription = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "LT Staff Benefit Fund")
                    {
                        crm.LTStaffBenefitFund = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "VPF Deduction")
                    {
                        crm.VPFDeduction = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "VPF Percentage")
                    {
                        crm.VPFPercentage = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "LIC - PENSION")
                    {
                        crm.LICPENSION = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "JEEVAN SURAKSHA")
                    {
                        crm.JEEVANSURAKSHA = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "HDFC")
                    {
                        crm.HDFC = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "CCA - AP")
                    {
                        crm.CCAAP = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "CCS - HYD")
                    {
                        crm.CCSHYD = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "AB-HLF(HYD)")
                    {
                        crm.ABHLFHYD = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "APCCADB - EMP CCS")
                    {
                        crm.APCCADBEMPCCS = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "APCOB-ED.LOAN-HNR.BR")
                    {
                        crm.APCOBEDLOANHNRBR = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "APCOB-ED.LOAN-HO")
                    {
                        crm.APCOBEDLOANHO = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "APSCB-LT-Emp Assn")
                    {
                        crm.APSCBLTEmpAssn = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "DR-CCS-VIZAG")
                    {
                        crm.DRCCSVIZAG = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "PRN-JR CVL JUDGE, VIZAG")
                    {
                        crm.PRNJRCVLJUDGEVIZAG = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "1 ADDL JR CVL JUDGE, TANUKU")
                    {
                        crm.ADDLJRCVLJUDGEANUKU = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "KADAPA DCCB")
                    {
                        crm.KADAPADCCB = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "JR CVL JUDGE, SULLURPET")
                    {
                        crm.JRCVLJUDGESULLURPET = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "KML COOP BANK, VIZAG")
                    {
                        crm.KMLCOOPBANKVIZAG = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "ESTT-EXCESS HRA REC")
                    {
                        crm.ESTTEXCESSHRAREC = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "FESTIVAL ADVANCE")
                    {
                        crm.FESTIVALADVANCE = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "ANDHRA BANK, RAMANTHPUR")
                    {
                        crm.ANDHRABANKRAMANTHPUR = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "SR.CIVIL JUDGE, KADAPA")
                    {
                        crm.SRCIVILJUDGEKADAPA = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "MEDICAL ADVANCE RECOVERY")
                    {
                        crm.MEDICALADVANCERECOVERY = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "XIX JR.CVL JUDGE, C C COURT, HYDERABAD")
                    {
                        crm.XIXJRCVLJUDGECCCOURTHYDERABAD = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "BANKS EMP ASSN TELANGANA")
                    {
                        crm.BANKSEMPASSNTELANGANA = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "LIC MACHILIPATNAM")
                    {
                        crm.LICMACHILIPATNAM = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "VEHICLE LOAN MACHILIPATNAM")
                    {
                        crm.VEHICLELOANMACHILIPATNAM = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "FESTIVAL ADVANCE MEDAK")
                    {
                        crm.FESTIVALADVANCEMEDAK = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "FEST ADV MEDAK")
                    {
                        crm.FESTADVMEDAK = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "XI JR.CVL JUDGE, C C COURT, SEC")
                    {
                        crm.XIJRCVLJUDGECCCOURTSEC = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "FESTIVAL ADVANCE ELURU")
                    {
                        crm.FESTIVALADVANCEELURU = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "TELANGANA EMP UNION")
                    {
                        crm.TELANGANAEMPUNION = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "DCCB DEDUCTION")
                    {
                        crm.DCCBDEDUCTION = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "COURT DEDUCTION")
                    {
                        crm.COURTDEDUCTION = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "LIFE INSURANCE")
                    {
                        crm.LIFEINSURANCE = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "TELANGANA OFFICERS ASSN")
                    {
                        crm.TELANGANAOFFICERSASSN = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "TSCAB OFFICERS ASSN")
                    {
                        crm.TSCABOFFICERSASSN =ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "MISC. DEDUCTION")
                    {
                        crm.MISCDEDUCTION = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                    if (dd1["dd_name"].ToString() == "PERSONAL LOAN")
                    {
                        crm.PERSONALLOAN = ReportColConvertToDecimal( dd1["dd_amount"].ToString());
                    }
                }

                lst.Add(crm);
            }



                return lst;
            
        }
    }
}
