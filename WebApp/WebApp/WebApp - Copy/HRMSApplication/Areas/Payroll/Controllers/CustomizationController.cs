using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using PayRollBusiness.Customization;
using HRMSApplication.Helpers;
using PayrollModels;
using Newtonsoft.Json;
using PayRollBusiness.Reports;
using HRMSBusiness.Db;
using Entities;
using System.Data;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class CustomizationController : Controller
    {
        LoanledgerBusiness lbus1 = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
        Loans_MasterBusiness lbus = new Loans_MasterBusiness(LoginHelper.GetCurrentUserForPR());
        // GET: Payroll/Customization
        public ActionResult Loan_Master()
        {
            ViewBag.SectionName = "Customization";
            return View();
        }

        public ActionResult Payslip()
        {
            ViewBag.SectionName = "Customization";
            return View();
        }

        public ActionResult Encashment()
        {
            ViewBag.SectionName = "Customization";
            return View();
        }

        public async Task<string> GetLoans_MasterData()
        {
            var LoansMtrdata = await lbus.GetLoans_MastersData();
            //return LoansMtrdata;
            return JsonConvert.SerializeObject(LoansMtrdata);
        }

       public async Task<string> GetPayslipStructure()
        {
            return JsonConvert.SerializeObject(await lbus.GetPayslipStructure());


        }

        public async Task<string> GetEncashmentStructure()
        {
            return await lbus.GetEncashmentStructure();


        }

        [HttpPost]
        public async Task<string> updateLoansMastersData(CommonPostDTO Values)
        {

            if (Values.mstrloanobject != null)
            {

                return await lbus.UpdateLoansData(Values);

            }
            else
            {
                return "E#Error#Please Enter/Modified Data.";
                //return "E#Error#Loan Id Not Modified";
            }


        }

        [HttpPost]
        public async Task<string> updatePayslipStructure(PayslipStructure Values)
        {

            if (Values.payslip != null)
            {

                return await lbus.updatePayslipStructure(Values);

            }
            else
            {
                return "E#Error#Please Enter/Modified Data.";
               
            }


        }

        [HttpPost]
        public async Task<string> updateEncashmentStructure(EncashmentStructure Values)
        {

            if (Values.enashEarn != null || Values.enashDed != null)
            {

                return await lbus.updateEncashmentStructure(Values);

            }
            else
            {
                return "E#Error#Please Enter/Modified Data.";
               
            }


        }
        #region MIS
        LoginCredential lCredentials = LoginHelper.GetCurrentUserForPR();
        public ActionResult MIS_Transaction()
        {
            ViewBag.SectionName = "Customization";
            return View("~/Areas/Payroll/Views/Customization/MIS_Transaction.cshtml");
        }
        //loan mis raji
        [HttpGet]
        public async Task<ActionResult> LoansMIS_report()
        {
            setReportCommonViewBag();
            ViewBag.ReportName = "Loan";
            ViewBag.ReportTitle = "Loan MIS on Transactions";
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";

            ViewBag.textboxempcode = "Emp Code";
            ViewBag.OneMonthPickerLabel1 = "Month";

            ViewBag.DataUrl = "/Payroll/Customization/LoanMIS_Data?empcode=^1&mnth=^2&payslip=^3";

            ViewBag.PdfNoOfCols = 9;
            ViewBag.ExportColumns = "columns:[0,1,2,3,4,5]";
            ViewBag.PdfColumnsWidths = "30,50,50,50,50,35";
            //ViewBag.PdfGrpColHeaderLabels = " , , , , , , , , , , , , ";

            ViewBag.ReportColumns = "[{ 'title': 'Emp Code','data': 'column1', 'autoWidth': true }," +
                " {'title': 'Emp Name', 'data': 'column2',  'autoWidth': true }," +
                " {'title': 'Loan Type', 'data': 'column3',  'autoWidth': true }," +
           " {'title': 'Total Loan Amount', 'data': 'column4',  'autoWidth': true }," +
           "{ 'title': 'Total Installments','data': 'column5',  'autoWidth': true }," +
             " {'title': 'Completed installments', 'data': 'column6',  'autoWidth': true }," +
           " {'title': 'Remaining Installments', 'data': 'column7',  'autoWidth': true }," +
           "{ 'title': 'Inst Amount','data': 'column8',  'autoWidth': true }," +
           " {'title': 'Outstanding Principle', 'data': 'column9',  'autoWidth': true }," +
           "{ 'title': 'Outstanding Interest','data': 'column10',  'autoWidth': true }," +
           "{ 'title': ' FM','data': 'column11', 'autoWidth': true }]";
            ViewBag.ReportColumnsCount = 6;
            return View("~/Areas/Payroll/Views/Customization/LoansMIS_report.cshtml");
        }

        public async Task<string> LoanMIS_Data(string empcode, string mnth, string payslip)
        {
            LoanMISReport lFMIS = new LoanMISReport(LoginHelper.GetCurrentUserForPR());
            // var JAIIB_CAIIBDetails = await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType);
            return JsonConvert.SerializeObject(await lFMIS.GetLoanMIS_Data(empcode, mnth, payslip));
        }

        [HttpGet]
        public async Task<ActionResult> EarningsMIS_report()
        {

            EarningsMISReport EMIS = new EarningsMISReport(LoginHelper.GetCurrentUserForPR());
            setReportCommonViewBag();
            ViewBag.ReportName = "Earnings";
            ViewBag.ReportTitle = "Earnings MIS on Transactions ";
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";

            ViewBag.textboxempcode = "Emp Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            var earnings = await EMIS.GetEarningsMIS_Datadropdown();
            ViewBag.lstearnings = new SelectList(earnings, "column_name", "column_name");
            ViewBag.DataUrl = "/Payroll/Customization/EarningsMIS_Data?empcode=^1&mnth=^2&payslip=^3";

            ViewBag.PdfNoOfCols = 236;
            ViewBag.ExportColumns = "columns:[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235]";
            ViewBag.PdfColumnsWidths = "30,50,50,50,50,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35";
            //ViewBag.PdfGrpColHeaderLabels = " , , , , , , , , , , , , ";

            ViewBag.ReportColumns = "[{ 'title': 'Emp Code','data': 'column1', 'autoWidth': true }," +
                "{'title': 'Allowance_type','data': 'allowance_type' ,'sortable': false }," +
                    "{'title': 'dim_tid_ref_allow','data': 'dim_tid_ref_allow' ,'sortable': false }," +
                     "{'title': 'ABHLFHYD', 'data': 'ABHLFHYD' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'ANNUAL INCREMENT','data': 'ANNUALINCREMENT' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'APCCADBEMP','data': 'APCCADBEMP' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'APCOBEDLHNR','data': 'APCOBEDLHNR' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'APCOBEDLHO','data': 'APCOBEDLHO' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'APCOBHFCHO','data': 'APCOBHFCHO' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'APCOBHFCLT','data': 'APCOBHFCLT'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'APSCBLTASSN','data': 'APSCBLTASSN' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'ARR_GR_AMT','data': 'ARR_GR_AMT' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'ARREAR GROSS AMOUNT','data': 'ARREARGROSSAMOUNT' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'BASIC','data': 'BASIC' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'BR MANAGER ALLOWANCE','data': 'BRMANAGERALLOWANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'BR_MGR','data': 'BR_MGR' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'CAIIB','data': 'CAIIB' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'CAIIBINCREMENT','data': 'CAIIBINCREMENT','sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'CCA','data': 'CCA' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'CCSAP','data': 'CCSAP' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'CCSHYD','data': 'CCSHYD' ,'sortable': false ,className: 'dt-body-right' }," +
                       "{'title': 'CD2','data': 'CD2' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'CEO ALLOWANCE', 'data': 'CEOALLOWANCE' ,'sortable': false  ,className: 'dt-body-right' }," +
                     "{ 'title': 'CEOALLW','data': 'CEOALLW' ,'sortable': false ,className: 'dt-body-right' }," +
                     //"{ 'title': 'CHILDREN EDUCATION ALLOWANCE','data':'CHILDRENEDUCATIONALLOWANCE': 'Jaib' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'CODINSPRM','data': 'CODINSPRM' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'CODINSPRM1','data': 'CODINSPRM1' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'CODPERKS','data': 'CODPERKS' ,'sortable': false ,className: 'dt-body-right'  }," +
                     "{ 'title': 'COURT','data': 'COURT'  ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'COURTAB','data': 'COURTAB' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'COURTDED','data': 'COURTDED' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'COURTHYD','data': 'COURTHYD' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'COURTKADAPA','data': 'COURTKADAPA' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'COURTSECBAD','data': 'COURTSECBAD' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'COURTSLRPET','data': 'COURTSLRPET' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'COURTTANUKU','data': 'COURTTANUKU' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'COURTVIZAG','data': 'COURTVIZAG' ,'sortable': false ,className: 'dt-body-right' }," +
                        "{'title': 'DA','data': 'DA' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'DCCB','data': 'DCCB' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'Deputation Allowance', 'data': 'DEPU' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'DEPUTATIONALLOWANCE','data': 'DEPUTATIONALLOWANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'DRCCSVIZAG','data': 'DRCCSVIZAG' ,'sortable': false ,className: 'dt-body-right' }, " +
                      "{ 'title': 'EASSNSUB','data': 'EASSNSUB' ,'sortable': false ,className: 'dt-body-right' }," +
                       "{ 'title': 'ESI','data': 'ESI' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'ESTT','data': 'ESTT' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'EXGRATIA','data': 'EXGRATIA' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'FA','data': 'FA' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'FACULTYALLOWANCE','data': 'FACULTYALLOWANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'FAELURU','data': 'FAELURU' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'FAMEDHAK','data': 'FAMEDHAK' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'FAONGOLE','data': 'FAONGOLE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'FACULTY ALLOWANCE','data': 'FCLTYALLW' ,'sortable': false ,className: 'dt-body-right' }," +
                        "{ 'title': 'Festival Advance','data': 'FEST' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'FESTADVANCE','data': 'FESTADVANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'FGPRM','data': 'FGPRM' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'FIXEDALLOWANCE','data': 'FIXEDALLOWANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'FIXEDPERSONALALLOWANCE','data': 'FIXEDPERSONALALLOWANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'FPINCENTIVERECOVERY','data': 'FPINCENTIVERECOVERY' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'FPAHRAALLOWANCE','data': 'FPAHRAALLOWANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'FPIIP', 'data': 'FPIIP' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'FP Incentive Recovery','data': 'FPIR' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'FXDALLW','data': 'FXDALLW' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'GSLI','data': 'GSLI' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'HDCF','data': 'HDCF' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'HFC','data': 'HFC' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Housing Loan - 2','data': 'HL2'  ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Housing Loan 2A','data': 'HL2A' ,'sortable': false ,className: 'dt-body-right'  }," +
                     "{'title': 'Housing Loan 2B-2C','data': 'HL2BC' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'Housing Addl.Loan - 2D','data': 'HLADD' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'Housing Loan Commerical','data': 'HLCOM' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'Housing Loan-Plot','data': 'HLPLT' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'HOUINSUPRE','data': 'HOUINSUPRE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'HOUPROPINS','data': 'HOUPROPINS' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'Housing Loan Main','data': 'HOUS1' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'Housing Loan 2','data': 'HOUS2' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'Housing Loan 3','data': 'HOUS3' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'HOUSI', 'data': 'HOUSI' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'HRA','data': 'HRA' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'HRADIFFAUG10','data': 'HRADIFFAUG10' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'HYDCOURT','data': 'HYDCOURT' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'INCENTIVE','data': 'INCENTIVE' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'INCENTIVEDIFF','data': 'INCENTIVEDIFF' ,'sortable': false ,className: 'dt-body-right'  }," +
                     "{ 'title': 'INCREMENT','data': 'INCREMENT'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'INTERESTONNSC','data': 'INTERESTONNSC' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'Interim Allowance','data': 'INTERIM' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'INTERIMALLOWANCE','data': 'INTERIMALLOWANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'INTERIMRELIEF','data': 'INTERIMRELIEF' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'INTERMRELIEF','data': 'INTERMRELIEF' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'JAIIB','data': 'JAIIB' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'JAIIBINCREMENT','data': 'JAIIBINCREMENT' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'JEEVAN','data': 'JEEVAN' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'KADAPADCCB','data': 'KADAPADCCB' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'KMLCOOPBANK','data': 'KMLCOOPBANK' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'LEAVEENCASHMENT', 'data': 'LEAVEENCASHMENT' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'LIC','data': 'LICPREMIUM' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'LICMACHILI','data': 'LICMACHILI' ,'sortable': false ,className: 'dt-body-right'}, " +
                     "{ 'title': 'LICPENSION','data': 'LICPENSION' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'LIFEINS','data': 'LIFEINS' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'LOANPERKS','data': 'LOANPERKS' ,'sortable': false  ,className: 'dt-body-right'}," +
                     "{ 'title': 'LOSSOFPAY','data': 'LOSSOFPAY'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'LT PF Loan','data': 'LTPFLOAN' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'LTASSNLEVY','data': 'LTASSNLEVY' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'LTC','data': 'LTC' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'LTC ENCASHMENT','data': 'LTCENCASHMENT' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'LTCADVREC','data': 'LTCADVREC' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'MARR','data': 'MARR' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'Medical Allowance','data': 'MEDICAL' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'MEDICALAID','data': 'MEDICALAID' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'MEDICALALLOWANCE','data': 'MEDICALALLOWANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                        "{ 'title': 'MEDICALADVANCE','data': 'MEDICALADVANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'MISCDED','data': 'MISCDED' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'MISCEARN','data': 'MISCEARN' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'MISCELLAN','data': 'MISCELLAN' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'NPSGALLOWANCE','data': 'NPSGALLOWANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'NPSG Allowance','data': 'NPSGA' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'OFFASSN','data': 'OFFASSN' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'Officiating Allowance', 'data': 'OFFI' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'OFFICIATINGALLOWANCE','data': 'OFFICIATINGALLOWANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'ONDAYSAL','data': 'ONDAYSAL' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'ONEDAYSAL','data': 'ONEDAYSAL' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'OTWAGES','data': 'OTWAGES' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'PERNLON1','data': 'PERNLON1' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Personal Pay','data': 'PERPAY'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'Personal Qual Allowance','data': 'PERQPAY' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'PERS','data': 'PERS' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'PERSONALPAY','data': 'PERSONALPAY' ,'sortable': false ,className: 'dt-body-right'  }," +
                     "{ 'title': 'PERSONALQUALALLOWANCE','data': 'PERSONALQUALALLOWANCE'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'PETROLPAPER','data': 'PETROLPAPER' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'PETROLPAPER1','data': 'PETROLPAPER1' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'PF','data': 'PF' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'PF5','data': 'PF5' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'PF Loan ST 1','data': 'PFHT1' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'PF Loan ST 2','data': 'PFHT2' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'PF Advance LT1','data': 'PFLT1' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'PF Advance LT2','data': 'PFLT2' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'PF Advance LT3','data': 'PFLT3' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'PF Advance LT4','data': 'PFLT4' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'PFPERKS', 'data': 'PFPERKS' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'Physically Handicapped','data': 'PH' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'PHONE','data': 'PHONE' ,'sortable': false ,className: 'dt-body-right'}, " +
                     "{ 'title': 'PHYSICALLYHANDICAPPED','data': 'PHYSICALLYHANDICAPPED' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'RESATTENDERSALLOWANCE','data': 'RESATTENDERSALLOWANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'Res. Attenders Allowance','data': 'RESATTN' ,'sortable': false  ,className: 'dt-body-right'}," +
                     "{ 'title': 'SBF','data': 'SBF'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SBFLN','data': 'SBFLN' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'SHIFTDUTYALLOWANCE','data': 'SHIFTDUTYALLOWANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SOCIE','data': 'SOCIE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SP_ACSTI','data': 'SP_ACSTI' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPL Arrear Incentive','data': 'SP_ARREAR' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPL Bill Collector','data': 'SP_BILLCOLL' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPL Care Taker','data': 'SP_CARETAKE' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SP_CASHCAD','data': 'SP_CASHCAD' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SPL Cashier','data': 'SP_CASHIER' ,'sortable': false ,className: 'dt-body-right'}," +
                        "{ 'title': 'SPL Conveyance','data': 'SP_CONVEY' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPL Dafter','data': 'SP_DAFTARI' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPL Despatch','data': 'SP_DESPATCH' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPL Driver','data': 'SP_DRIVER' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SPL Electrician','data': 'SP_ELEC' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SPL Incentive','data': 'SP_INCENTIVE' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'SPL Jamedar','data': 'SP_JAMEDAR' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'SPL Key', 'data': 'SP_KEY' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPL Library','data': 'SP_LIBRARY' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPL Lift Operator','data': 'SP_LIFT' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'SPL Non Promotional','data': 'SP_NONPROM' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPL Personal Pay','data': 'SP_PERPAY' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPL Record room asst allowance','data': 'SP_RECASST' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'SPL Receptionist allowance','data': 'SP_RECEPTION'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPL Record room sub staff all','data': 'SP_RECSUB' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'SPL Split Duty -Award staff','data': 'SP_SD_AWARD' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'SPL Split Duty - Managers','data': 'SP_SD_MGR' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'SP_SHFTDUTY','data': 'SP_SHFTDUTY'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPL Stenographer','data': 'SP_STENO' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'SPL Telephone Operator','data': 'SP_TELEPHONE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPL Typist','data': 'SP_TYPIST' ,'sortable': false ,className: 'dt-body-right'  }," +
                     "{ 'title': 'SPL Watchman','data': 'SP_WATCHMAN'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPL Duplicating/xerox machine','data': 'SP_XEROX' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'SPCLDA','data': 'SPCLDA' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPECIALINCREMENT','data': 'SPECIALINCREMENT' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPECIALPAY','data': 'SPECIALPAY' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPLARREARINCENTIVE','data': 'SPLARREARINCENTIVE' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPLBILLCOLLECTOR','data': 'SPLBILLCOLLECTOR' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPLCARETAKER','data': 'SPLCARETAKER' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SPLCASHCABIN','data': 'SPLCASHCABIN' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SPLCASHIER','data': 'SPLCASHIER' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'SPLCONVEYANCE','data': 'SPLCONVEYANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'SPLDAFTAR', 'data': 'SPLDAFTAR' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPLDESPATCH','data': 'SPLDESPATCH' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPLDRIVER','data': 'SPLDRIVER' ,'sortable': false ,className: 'dt-body-right'}, " +
                     "{ 'title': 'SPLDUPLICATINGXEROXMACHINE','data': 'SPLDUPLICATINGXEROXMACHINE' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPLELECTRICIAN','data': 'SPLELECTRICIAN' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPLINCENTIVE','data': 'SPLINCENTIVE' ,'sortable': false  ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPLJAMEDAR','data': 'SPLJAMEDAR'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPLKEY','data': 'SPLKEY' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'SPLLIBRARY','data': 'SPLLIBRARY' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPLLIFTOPERATOR','data': 'SPLLIFTOPERATOR' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPLNONPROMOTIONAL','data': 'SPLNONPROMOTIONAL' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPLPERSONALPAY','data': 'SPLPERSONALPAY' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPLRECEPTIONISTALLOWANCE','data': 'SPLRECEPTIONISTALLOWANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPLRECORDROOMASSTALLOWANCE','data': 'SPLRECORDROOMASSTALLOWANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SPLRECORDROOMSUBSTAFFALL','data': 'SPLRECORDROOMSUBSTAFFALL' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SPLRECORDROOMUBSTAFFALLOWANCE','data': 'SPLRECORDROOMUBSTAFFALLOWANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                        "{ 'title': 'SPLSPLALWACSTI','data': 'SPLSPLALWACSTI' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPLSPLITDUTYMANAGERS','data': 'SPLSPLITDUTYMANAGERS' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPLSPLITDUTYAWARDSTAFF','data': 'SPLSPLITDUTYAWARDSTAFF' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'SPLSTENOGRAPHER','data': 'SPLSTENOGRAPHER' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SPLTELEPHONEOPERATOR','data': 'SPLTELEPHONEOPERATOR' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SPLTYPIST','data': 'SPLTYPIST' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'SPLWATCHMAN','data': 'SPLWATCHMAN' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'SPLALLOW', 'data': 'SPLALLOW' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPLINCR','data': 'SPLINCR' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SPLPAY','data': 'SPLPAY' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'STAGALLOW','data': 'STAGALLOW' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'STAGNATIONINCREMENTS','data': 'STAGNATIONINCREMENTS' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SUBCLUB','data': 'SUBCLUB' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'SUBLT','data': 'SUBLT'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'SUBST','data': 'SUBST' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'SUBUNION','data': 'SUBUNION' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SYSADMN','data': 'SYSADMN' ,'sortable': false ,className: 'dt-body-right'}," +
                         "{ 'title': 'SYSTEMADMINISTRATORALLOWANCE','data': 'SYSTEMADMINISTRATORALLOWANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'TDS','data': 'TDS' ,'sortable': false ,className: 'dt-body-right'}," +
                        "{'title':  'TEACH','data': 'TEACH' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'TEACHINGALLOWANCE','data': 'TEACHINGALLOWANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'TGASSN', 'data': 'TGASSN' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'TGUNION','data': 'TGUNION' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'TOFA','data': 'TOFA' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'TSCABOA','data': 'TSCABOA' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'STAGNATIONINCREMENTS','data': 'STAGNATIONINCREMENTS' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'UNIONLEVY','data': 'UNIONLEVY' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Vehicle Loan (2W)','data': 'VEH2W'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'Vehicle Loan (4W)','data': 'VEH4W' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'VEHINS','data': 'VEHINS' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'VEHMACHILI','data': 'VEHMACHILI' ,'sortable': false ,className: 'dt-body-right'}," +
                         "{ 'title': 'VIJAYA','data': 'VIJAYA' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'VISAKHA','data': 'VISAKHA' ,'sortable': false ,className: 'dt-body-right'}," +
                          "{ 'title': 'VPF','data': 'VPF' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'WASHALLW','data': 'WASHALLW' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'TEACH','data': 'TEACH' ,'sortable': false ,className: 'dt-body-right' }," +


                " {'title': 'WASHINGALLOWANCE', 'data': 'WASHINGALLOWANCE',  'autoWidth': true }]";

            ViewBag.ReportColumnsCount = 236;
            return View("~/Areas/Payroll/Views/Customization/EarningsMIS_report.cshtml");
        }

        public async Task<string> EarningsMIS_Data(string empcode, string mnth, string payslip)
        {
            EarningsMISReport EMIS = new EarningsMISReport(LoginHelper.GetCurrentUserForPR());
            // var JAIIB_CAIIBDetails = await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType);
            return JsonConvert.SerializeObject(await EMIS.GetEarningsMIS_Data(empcode, mnth, payslip));
        }

        //PF report
        private void setReportCommonViewBag()
        {
            ViewBag.SectionName = "Customization";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "portrait";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
        }

        [HttpGet]
        public async Task<ActionResult> PFMISreport()
        {

            // setReportCommonViewBag();
            ViewBag.SectionName = "Customization";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
          
            //DateTime Financial_md = (lCredentials.FinancialMonthDate);           
            ViewBag.fm1 = Financial_md.ToString("dd-MM-yyyy"); // formate dd/mm/yyyy
            ViewBag.ReportName = "PF";
            ViewBag.ReportTitle = "PF MIS on Transactions";
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";
            //var psdata = await lbus1.getpsdropdownvalues();
            var pfdata = await lbus1.getpsdropdownvalues();
            //var deductdata = await lbus1.getdeductdropdownvalues();
            ViewBag.textboxempcode = "Emp Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            //ViewBag.psData = new SelectList(psdata, "Id", "value");
            ViewBag.pfData = new SelectList(pfdata, "Id", "value");
            //ViewBag.deductData = new SelectList(deductdata, "Id", "value");
            ViewBag.DataUrl = "/Payroll/Customization/PFMIS_Data?empcode=^1&fromdate=^2&todate=^3&vpf=^4";

            ViewBag.PdfNoOfCols = 14;
            ViewBag.ExportColumns = "columns:[0,1,2,3,4,5,6,7,8,9,10,11,12,13]";
            ViewBag.PdfColumnsWidths = "30,50,50,50,50,35,35,35,35,35,35,35,35,35";
            //ViewBag.PdfGrpColHeaderLabels = " , , , , , , , , , , , , ";

            ViewBag.ReportColumns = "[{ 'title': 'Emp Code','data': 'column1', 'autoWidth': true }," +
                " {'title': 'Emp Name', 'data': 'column2',  'autoWidth': true }," +
                " {'title': 'Designation', 'data': 'column3',  'autoWidth': true }," +
                " {'title': 'Month', 'data': 'column15',  'autoWidth': true }," +
           "{ 'title': 'Own Share','data': 'column4',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Bank Share','data': 'column5',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Own Share Open','data': 'column6',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Own Share Total','data': 'column7',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Bank Share Open','data': 'column8',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Bank Share Total','data': 'column9',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Pension Open','data': 'column10',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Pension Total','data': 'column11',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Pension Interest Amount','data': 'column12',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Bank Share Interest Open','data': 'column13',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Bank Share Interest Total','data': 'column14', 'autoWidth': true ,className: 'dt-body-right'}]";
            ViewBag.ReportColumnsCount = 14;
            return View("~/Areas/Payroll/Views/Customization/PFMISReport.cshtml");
        }

        public async Task<string> PFMIS_Data(string empcode,string fromdate,string todate,string vpf)
        {
            PFMISReport pFMIS = new PFMISReport(LoginHelper.GetCurrentUserForPR());
             
            // var JAIIB_CAIIBDetails = await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType);
            return JsonConvert.SerializeObject(await pFMIS.GetPFMIS_Data(empcode, fromdate, todate, vpf));
        }
        // R&P Report
        [HttpGet]
        public async Task<ActionResult> PFRandP_Report()
        {
            //setReportCommonViewBag();
            ViewBag.SectionName = "Customization";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            
            ViewBag.ReportName = "RandP";
            ViewBag.ReportTitle = "R and P MIS on Transactions";
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";

            ViewBag.textboxempcode = "Emp Code";
            ViewBag.OneMonthPickerLabel1 = "Month";

            ViewBag.DataUrl = "/Payroll/Customization/PFRandP_Report_Data?empcode=^1&mnth=^2";

            ViewBag.PdfNoOfCols = 12;
            ViewBag.ExportColumns = "columns:[0,1,2,3,4,5,6,7,8,9,10,11,12]";
            ViewBag.PdfColumnsWidths = "30,50,50,50,50,35,40,55,55,50,50,50,50";
            
            ViewBag.ReportColumnsCount = 10;
            return View();
        }

        public async Task<string> PFRandP_Report_Data(string empcode, string mnth)
        {
            PFMISReport pFMIS = new PFMISReport(LoginHelper.GetCurrentUserForPR());

            return JsonConvert.SerializeObject(await pFMIS.GetPRandP_Data(empcode, mnth));
        }

        // VPF Report
        [HttpGet]
        public async Task<ActionResult> VPFMIS_report()
        {
            //setReportCommonViewBag();
            ViewBag.SectionName = "Customization";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            
           // DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm1 = Financial_md.ToString("dd-MM-yyyy"); // formate dd/mm/yyyy
            ViewBag.ReportName = "VPF";
            ViewBag.ReportTitle = "VPF MIS on Transactions ";
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";

            ViewBag.textboxempcode = "Emp Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            var vpfdata = await lbus1.getpsdropdownvalues();
            ViewBag.vpfData = new SelectList(vpfdata, "Id", "value");
            ViewBag.DataUrl = "/Payroll/Customization/VPFMIS_report_Data?empcode=^1&fromdate=^2&todate=^3&vpf=^4";

            ViewBag.PdfNoOfCols = 9;
            ViewBag.ExportColumns = "columns:[0,1,2,3,4,5,6,7,8]";
            ViewBag.PdfColumnsWidths = "30,50,50,50,50,35,35,35,35";
            ViewBag.ReportColumns = "[{ 'title': 'Emp Code','data': 'column1', 'autoWidth': true }," +
                " {'title': 'Emp Name', 'data': 'column2',  'autoWidth': true }," +
                " {'title': 'Designation', 'data': 'column3',  'autoWidth': true }," +
                " {'title': 'Month', 'data': 'column10',  'autoWidth': true }," +
                " {'title': 'Vpf', 'data': 'column4',  'autoWidth': true ,className: 'dt-body-right'}," +
           " {'title': 'Vpf Interest Amount', 'data': 'column5',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Vpf Open','data': 'column6',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Vpf Total','data': 'column7',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Vpf Interest Open','data': 'column8',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Vpf Interest Total','data': 'column9', 'autoWidth': true ,className: 'dt-body-right'}]";
            ViewBag.ReportColumnsCount = 9;
            return View("~/Areas/Payroll/Views/Customization/PFMISReport.cshtml");
        }

        public async Task<string> VPFMIS_report_Data(string empcode, string fromdate,string todate, string vpf)
        {
            PFMISReport pFMIS = new PFMISReport(LoginHelper.GetCurrentUserForPR());

            // var JAIIB_CAIIBDetails = await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType);
            return JsonConvert.SerializeObject(await pFMIS.GetVPF_report_Data(empcode, fromdate,todate, vpf));
        }
        // PayslipEarn Report
        [HttpGet]
        public async Task<ActionResult> PSEarnMIS_report()
        {
            setReportCommonViewBag();
            ViewBag.ReportName = "PayslipEarn";
            ViewBag.ReportTitle = "";
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickersEarn";

            ViewBag.textboxempcode = "Emp Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            var earndata = await lbus1.getpsdropdownvalues();
            ViewBag.EarnData = new SelectList(earndata, "Id", "value");
            ViewBag.DataUrl = "/Payroll/Customization/PSEarnMIS_report_Data?empcode=^1&fromdate=^2&todate=^3&earning=^4";

            ViewBag.PdfNoOfCols = 9;
            ViewBag.ExportColumns = "columns:[0,1,2,3,4,5]";
            ViewBag.PdfColumnsWidths = "30,50,50,50,50,35";
            ViewBag.ReportColumns = "[{ 'title': 'Emp Code','data': 'column1', 'autoWidth': true }," +
                " {'title': 'Emp Name', 'data': 'column2',  'autoWidth': true }," +
                " {'title': 'OWN PF', 'data': 'column3',  'autoWidth': true }," +
           " {'title': 'Bank PF', 'data': 'column4',  'autoWidth': true }," +
           "{ 'title': 'VPF','data': 'column5',  'autoWidth': true }," +
           "{ 'title': 'Total PF','data': 'column6',  'autoWidth': true }," +
           "{ 'title': ' Pension','data': 'column7', 'autoWidth': true }]";
            ViewBag.ReportColumnsCount = 6;
            return View("~/Areas/Payroll/Views/Customization/PFMISReport.cshtml");
        }

        public async Task<string> PSEarnMIS_report_Data(string empcode, string fromdate, string todate, string earning)
        {
            PFMISReport pFMIS = new PFMISReport(LoginHelper.GetCurrentUserForPR());

            // var JAIIB_CAIIBDetails = await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType);
            return JsonConvert.SerializeObject(await pFMIS.GetVPF_report_Data(empcode, fromdate, todate, earning));
        }
        // PayslipEarn Report
        [HttpGet]
        public async Task<ActionResult> TDSEarnMIS_report()
        {
            setReportCommonViewBag();
            ViewBag.ReportName = "TDSEarn";
            ViewBag.ReportTitle = "";
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickersEarn";

            ViewBag.textboxempcode = "Emp Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            var earndata = await lbus1.getpsdropdownvalues();
            ViewBag.EarnData = new SelectList(earndata, "Id", "value");
            ViewBag.DataUrl = "/Payroll/Customization/TDSEarnMIS_report_Data?empcode=^1&fromdate=^2&todate=^3&earning=^4";

            ViewBag.PdfNoOfCols = 9;
            ViewBag.ExportColumns = "columns:[0,1,2,3,4,5]";
            ViewBag.PdfColumnsWidths = "30,50,50,50,50,35";
            ViewBag.ReportColumns = "[{ 'title': 'Emp Code','data': 'column1', 'autoWidth': true }," +
                " {'title': 'Emp Name', 'data': 'column2',  'autoWidth': true }," +
                " {'title': 'OWN PF', 'data': 'column3',  'autoWidth': true }," +
           " {'title': 'Bank PF', 'data': 'column4',  'autoWidth': true }," +
           "{ 'title': 'VPF','data': 'column5',  'autoWidth': true }," +
           "{ 'title': 'Total PF','data': 'column6',  'autoWidth': true }," +
           "{ 'title': ' Pension','data': 'column7', 'autoWidth': true }]";
            ViewBag.ReportColumnsCount = 6;
            return View("~/Areas/Payroll/Views/Customization/PFMISReport.cshtml");
        }

        public async Task<string> TDSEarnMIS_report_Data(string empcode, string fromdate, string todate, string earning)
        {
            PFMISReport pFMIS = new PFMISReport(LoginHelper.GetCurrentUserForPR());

            // var JAIIB_CAIIBDetails = await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType);
            return JsonConvert.SerializeObject(await pFMIS.GetVPF_report_Data(empcode, fromdate, todate, earning));
        }
        public async Task<ActionResult> TDSdeductMIS_report()
        {
            
            setReportCommonViewBag();
            ViewBag.ReportName = "TDSDeduct";
            ViewBag.ReportTitle = "TDSDeduct MIS on Transactions";
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickersDeduct";

            ViewBag.fm1 = (lCredentials.FinancialMonthDate).ToString("dd-MM-yyyy"); // formate dd/mm/yyyy
            ViewBag.textboxempcode = "Emp Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            var deddata = await lbus1.getdeductdropdownvalues();
            var dt = new SqlHelper().Get_Table_FromQry("select ORDINAL_POSITION as id,concat('  ',column_name) as value from INFORMATION_SCHEMA.columns where table_name='DeductionFact' except select top 5 ORDINAL_POSITION as id,concat('  ',column_name) from INFORMATION_SCHEMA.columns where table_name='DeductionFact';");
            var item = dt.AsEnumerable().Select(r => new DeductionFact
            {
                id = (Int32)(r["id"]),
                value = (string)(r["value"] ?? "null")
            }).ToList();

            item.Insert(0, new DeductionFact
            {
                id = 0,
                value = "All"
            });

            //ViewBag.DdlOneData = new SelectList(item, "Id", "Type");
            ViewBag.DeductData = new SelectList(item, "id", "value");
            ViewBag.DataUrl = "/Payroll/Customization/TDSDeductMIS_report_Data?empcode=^1&fromdate=^2&todate=^3&deduct=^4";

            ViewBag.PdfNoOfCols = 3;
            ViewBag.ExportColumns = "columns: [1, 2, 3]";
            ViewBag.PdfColumnsWidths = "50,150,50";
            ViewBag.ReportColumns = "[" +
                "{'title': 'RowID', 'data': 'RowId', 'visible': false }," +
                "{ 'title': 'S.NO','data': 'grpclmn','sortable': false}," +
                "{ 'title': 'Field Name','data': 'column2' ,'sortable': false}," +
                //"{ 'title': 'Amount','data':'column3' ,'sortable': false}," +
                "{ 'title': 'Amount','data': 'column3' ,'sortable': false}]";
            ViewBag.ReportColumnsCount = 3;
            return View("~/Areas/Payroll/Views/Customization/TDSMISReport.cshtml");
        }

        public async Task<string> TDSDeductMIS_report_Data(string empcode, string fromdate, string todate, string deduct)
        {
            PFMISReport TdsMIS = new PFMISReport(LoginHelper.GetCurrentUserForPR());

            // var JAIIB_CAIIBDetails = await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType);
            return JsonConvert.SerializeObject(await TdsMIS.GetTDSMIS_Data(empcode, fromdate, todate, deduct));
        }
        public async Task<ActionResult> PSdeductMIS_report()
        {

            setReportCommonViewBag();
            ViewBag.ReportName = "PayslipDeduct";
            ViewBag.ReportTitle = "PayslipDeduct MIS on Transactions";
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickerspayslipDeduct";
            ViewBag.fm1 = (lCredentials.FinancialMonthDate).ToString("dd-MM-yyyy"); // formate dd/mm/yyyy
            ViewBag.textboxempcode = "Emp Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            var deddata = await lbus1.getdeductdropdownvalues();
            var dt = new SqlHelper().Get_Table_FromQry("select ORDINAL_POSITION as id,concat('  ',column_name) as value from INFORMATION_SCHEMA.columns where table_name='DeductionFact' except select top 5 ORDINAL_POSITION as id,concat('  ',column_name) from INFORMATION_SCHEMA.columns where table_name='DeductionFact';");
            var item = dt.AsEnumerable().Select(r => new DeductionFact
            {
                id = (Int32)(r["id"]),
                value = (string)(r["value"] ?? "null")
            }).ToList();

            item.Insert(0, new DeductionFact
            {
                id = 0,
                value = "All"
            });

            //ViewBag.DdlOneData = new SelectList(item, "Id", "Type");
            ViewBag.PSDeductData = new SelectList(item, "id", "value");
            ViewBag.DataUrl = "/Payroll/Customization/PSDeductMIS_report_Data?empcode=^1&fromdate=^2&todate=^3&deduct=^4";

            ViewBag.PdfNoOfCols = 3;
            ViewBag.ExportColumns = "columns: [1, 2, 3]";
            ViewBag.PdfColumnsWidths = "50,150,50";
            ViewBag.ReportColumns = "[" +
                "{'title': 'RowID', 'data': 'RowId', 'visible': false }," +
                "{ 'title': 'S.NO','data': 'grpclmn','sortable': false}," +
                "{ 'title': 'Field Name','data': 'column2' ,'sortable': false}," +
                "{ 'title': 'Amount','data': 'column3' ,'sortable': false}]";
            ViewBag.ReportColumnsCount = 3;
            return View("~/Areas/Payroll/Views/Customization/TDSMISReport.cshtml");
        }

        public async Task<string> PSDeductMIS_report_Data(string empcode, string fromdate, string todate, string deduct)
        {
            PFMISReport TdsMIS = new PFMISReport(LoginHelper.GetCurrentUserForPR());

            // var JAIIB_CAIIBDetails = await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType);
            return JsonConvert.SerializeObject(await TdsMIS.GetPSMIS_Data(empcode, fromdate, todate, deduct));
        }
        [HttpGet]
        public async Task<ActionResult> EmployeeMIS_report()
        {

            // setReportCommonViewBag();
            ViewBag.SectionName = "Customization";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");

            //DateTime Financial_md = (lCredentials.FinancialMonthDate);           
            ViewBag.fm1 = Financial_md.ToString("dd-MM-yyyy"); // formate dd/mm/yyyy
            ViewBag.ReportName = "EmployeeMIS";
            ViewBag.ReportTitle = "Employee MIS on Transactions";
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";
            var pfdata = await lbus1.getpsdropdownvalues();
            ViewBag.textboxempcode = "Emp Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.pfData = new SelectList(pfdata, "Id", "value");
            ViewBag.DataUrl = "/Payroll/Customization/EmployeeMIS_report_Data?empcode=^1&fromdate=^2&todate=^3&emp=^4";

            ViewBag.PdfNoOfCols = 19;
            ViewBag.ExportColumns = "columns:[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18]";
            //ViewBag.ExportColumns = "columns:[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22]";
            ViewBag.PdfColumnsWidths = "30,25,25,25,25,25,25,25,25,25,25,25,25,25,25,25,25,25,25";
            //ViewBag.PdfColumnsWidths = "30,50,50,50,50,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35,35";
            //ViewBag.PdfGrpColHeaderLabels = " , , , , , , , , , , , , ";

            ViewBag.ReportColumns = "[{ 'title': 'Emp Code','data': 'column1', 'autoWidth': true }," +
                " {'title': 'Emp Name', 'data': 'column2',  'autoWidth': true }," +
                " {'title': 'Designation', 'data': 'column3',  'autoWidth': true }," +
                " {'title': 'Month', 'data': 'column4',  'autoWidth': true }," +
           "{ 'title': 'Basic','data': 'column5',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'DA','data': 'column6',  'autoWidth': true ,className: 'dt-body-right'}," +
          "{ 'title': 'CCA','data': 'column7',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'HRA','data': 'column8',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Intern Relief','data': 'column9',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Telangana Incr','data': 'column10',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Gross Amount','data': 'column11',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Provident Fund','data': 'column12',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Income Tax','data': 'column13',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Professional Tax','data': 'column14',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Telangana Off Fund','data': 'column15',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Telangana Off Assn','data': 'column16',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Net Amount','data': 'column17',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Spl DA','data': 'column18',  'autoWidth': true ,className: 'dt-body-right'}," +
           "{ 'title': 'Spl Allow','data': 'column19', 'autoWidth': true ,className: 'dt-body-right'}]";
            ViewBag.ReportColumnsCount =19;
            return View("~/Areas/Payroll/Views/Customization/EmployeeMISReport.cshtml");
        }

        public async Task<string> EmployeeMIS_report_Data(string empcode, string fromdate, string todate, string emp)
        {
            PFMISReport pFMIS = new PFMISReport(LoginHelper.GetCurrentUserForPR());

            // var JAIIB_CAIIBDetails = await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType);
            return JsonConvert.SerializeObject(await pFMIS.GetPFMIS_Data(empcode, fromdate, todate, emp));
        }
        #endregion
    }
}