using HRMSApplication.Helpers;
using Newtonsoft.Json;
using PayRollBusiness.Masters;
using PayRollBusiness.Process;
using PayRollBusiness.Reports;
using PayrollModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System;
namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class PrAllGroupFooterController : Controller
    {
        LoanReportBusiness lrbus = new LoanReportBusiness(LoginHelper.GetCurrentUserForPR());
        TSumReportBusiness tsbus = new TSumReportBusiness(LoginHelper.GetCurrentUserForPR());
        GeneralVochersReportBusiness Gvbus = new GeneralVochersReportBusiness(LoginHelper.GetCurrentUserForPR());
        InterfaceVocherBusiness IntBus = new InterfaceVocherBusiness(LoginHelper.GetCurrentUserForPR());
        Form12BReportBusiness fbus = new Form12BReportBusiness(LoginHelper.GetCurrentUserForPR());
        PFPayableReportBusiness PFRB = new PFPayableReportBusiness(LoginHelper.GetCurrentUserForPR());
        SalBrReportBusiness SALB = new SalBrReportBusiness(LoginHelper.GetCurrentUserForPR());
        Form3ABusiness F3A = new Form3ABusiness(LoginHelper.GetCurrentUserForPR());
        PFPayableOBShareBusiness payOB = new PFPayableOBShareBusiness(LoginHelper.GetCurrentUserForPR());
        LoginCredential lCredentials = null;
        private void setReportCommonViewBag()
        {
            lCredentials = LoginHelper.GetCurrentUserForPR();

            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            //ViewBag.PdfOrientation = "landscape";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");

            ViewBag.fm1 = (lCredentials.FinancialMonthDate).ToString("dd-MM-yyyy"); // formate dd/mm/yyyy

        }

       
        #region LIC/HFC Report by Sowjanya as Footer

        [HttpGet]
        public async Task<ActionResult> LICReport()
        {
            setReportCommonViewBag();

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getTypes";

            //*** get Types(LIC/HFC) *** Total
            LICReportBusiness LICB = new LICReportBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await LICB.getTypes();

            ViewBag.ReportTitle = "LIC/HFC";
            ViewBag.ReportFiltersTemplate = "TLIC-AllEmpSearchOneMultiSelOneMonthpic";
            ViewBag.T1MultiSelLabel = "Deduction Type";
            ViewBag.T1MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Employee Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/LICReportData?emp_code=^1&Types=^2&mnth=^3";
            ViewBag.ExportColumns = "columns: [1, 2, 3,4,5,6,7]";
            ViewBag.PdfColumnsWidths = "100,90,90,50,50,50,60";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId' , 'visible': false }," +
                    "{ 'title': 'SlNo','data': 'SlNo','sortable': false }," +
                       "{ 'title': 'Deduction Type','data': 'column1','sortable': false }," +
                "{ 'title': 'EmpCode','data': 'column2' ,'sortable': false }," +
                  "{ 'title': 'Emp Name','data': 'column3' ,'sortable': false }," +
                "{ 'title': 'Account No ','data': 'column4' ,'sortable': false }," +
                "{ 'title': 'Amount','data': 'column5' ,'sortable': false }, { 'title': 'Total','data': 'column6' ,'sortable': false }]";

            ViewBag.ReportColumnsCount = 7;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> LICReportData(string emp_code, string Types, string mnth)
        {
            LICReportBusiness LICB = new LICReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await LICB.GetLICReport(emp_code, Types, mnth);
            return JsonConvert.SerializeObject(report);

        }

        #endregion

        #region Allowance Report by  Sowjanya 
        public async Task<ActionResult> AllowanceReport()
        {
            setReportCommonViewBag();

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/AllowanceTypes";
            AllowanceReportBusiness arbus = new AllowanceReportBusiness(LoginHelper.GetCurrentUserForPR());
            var AllowanceType = await arbus.AllowanceTypes();
            ViewBag.ReportTitle = "Allowance ";
             ViewBag.ReportFiltersTemplate = "GroupingT4-AllEmpSearchOneMultiSelOneMonthpic";

            ViewBag.T4MultiSelLabel = "AllowanceType";
            ViewBag.T4MultiSelList = AllowanceType;
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/AllowanceReportData?emp_code=^1&Types=^2&mnth=^3";

            ViewBag.PdfNoOfCols = 5;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4]";
            //ViewBag.PdfColumnsWidths = "150,100,140,100";
            ViewBag.PdfGrpColHeaderLabels = "Code: ,Emp Name:, ,  ";

            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId' , 'visible': false }," +
           " {'title': 'Allowance Type', 'data': 'grpclmn' ,'sortable': false }," +
           "{ 'title': 'From Date ','data': 'column1' ,'sortable': false }," +
           "{ 'title': 'To Date','data': 'column2' ,'sortable': false }, " +
           "{ 'title': 'Amount','data': 'column3' ,'sortable': false,className: 'dt-body-right' }] ";
            ViewBag.ReportColumnsCount = 4;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }
        public async Task<string> AllowanceReportData(string emp_code, string Types, string mnth)
        {

            AllowanceReportBusiness arbus = new AllowanceReportBusiness(LoginHelper.GetCurrentUserForPR());
            var AllowanceDetails = await arbus.GetAllowencedata(emp_code, Types, mnth);
            return JsonConvert.SerializeObject(AllowanceDetails);

        }

        #endregion

        #region sal branchwise by Lalitha/ sowjanya

        //Salary bill of the branch for month

        [HttpGet]
        public async Task<ActionResult> SalBrReport()
        {

            setReportCommonViewBag();
            ViewBag.T3MultiSelLabel = "Branch";
            ViewBag.ReportFiltersTemplate = "T7-SalbrOneMonthPickeronedropdown";
            ViewBag.OneMonthPickerLabel1 = "Month";

            ViewBag.ReportTitle = "Salary Bill of Branch";

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/SalBrReportData?Branch=^1&inputMonth=^2";
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6, 7,8]";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId' , 'visible': false }," +
                "{ 'title': 'SlNo','data': 'column1' ,'sortable': false }," +
                "{ 'title': 'Emp Code','data': 'column2' ,'sortable': false }," +
           " {'title': 'Emp Name', 'data': 'column3' ,'sortable': false }," +
           "{ 'title': 'Designation','data': 'column4'  ,'sortable': false}," +
           "{ 'title': 'Gross Amount','data': 'column5' ,'sortable': false,className: 'dt-body-right' }," +
            "{ 'title': 'Deductions','data': 'column6' ,'sortable': false,className: 'dt-body-right' }," +
           "{ 'title': 'Net Amount','data': 'column7' ,'sortable': false,className: 'dt-body-right' }," +
           "{ 'title': 'Payslip Type','data': 'column8' ,'sortable': false }]";
            ViewBag.ReportColumnsCount = 8;
            ViewBag.ReportFooterColumnsCount = 8;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }
        public async Task<string> GetBranches()
        {
            return await SALB.getBranches();
        }

        public async Task<string> SalBrReportData(string Branch, string inputMonth)
        {

            var report = await SALB.GetSalBrReport(Branch, inputMonth);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region Tsum Report BY RAJYALAKSHMI
        [HttpGet]
        public ActionResult TSumReport()
        {

            setReportCommonViewBag();
            //    //*** get Branches list
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getBranches";
            ViewBag.T3MultiSelLabel = "Branch";
            ViewBag.ReportFiltersTemplate = "T9-TwoRadioButtonsOneMonthPicker";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.ReportTitle = "TSum";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/TSumReportData?inputMonth=^1&RegEmp=^2&SupEmp=^3";
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6, 7]";
           // ViewBag.PdfColumnsWidths = "100,80,80,75,80,75";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId' , 'visible': false }," +
                 "{ 'title': 'Summary Details','data': 'SlNo' ,'sortable': false }," +
              //"{ 'title': 'Summary Details','data': 'column1'  ,'sortable': false}," +
              "{ 'title': 'Gross Salary','data': 'column2' ,'sortable': false ,className: 'dt-body-right'}," +
           " {'title': 'Provident Fund', 'data': 'column3' ,'sortable': false ,className: 'dt-body-right'}," +
              " {'title': 'NPS', 'data': 'column7' ,'sortable': false ,className: 'dt-body-right'}," +
           "{ 'title': 'VPF','data': 'column4' ,'sortable': false ,className: 'dt-body-right'}," +
           "{ 'title': 'Total Deductions','data': 'column5' ,'sortable': false ,className: 'dt-body-right'}," +
            "{ 'title': 'Net Salary','data': 'column6' ,'sortable': false ,className: 'dt-body-right'}]";
            ViewBag.ReportColumnsCount = 7;
            ViewBag.ReportFooterColumnsCount = 6;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> TSumReportData(string inputMonth, string RegEmp, string SupEmp)
        {
            string ipmn = "01-01-01";
            if (inputMonth != "^1")
            {
                ipmn = inputMonth;
            }
            var report = await tsbus.TSumReportData(ipmn, RegEmp, SupEmp);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region TSheetReport By RAJYALAKSHMI
        public ActionResult TSheetReport()
        {
           
            //setReportCommonViewBag1();
            lCredentials = LoginHelper.GetCurrentUserForPR();
            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            ViewBag.Removepdf = "RemovePdf";
            ViewBag.PdfOrientation = "landscape";
            @ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.ReportTitle = "Tsheet";
            ViewBag.ReportFiltersTemplate = "T8-OneMonthPickerTwoRadioButtons";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/TSheetReportData?inputMonth=^1&RegEmp=^2&SupEmp=^3";
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131]";
            ViewBag.PdfNoOfCols = 20;
            // ViewBag.PdfColumnsWidths = "20,30,45,45,30,30,30,30,30,30,30,30,20,30,30,35,35,20,20,20";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId' , 'visible': false }," +
                     "{'title': 'Emp Code','data': 'column1' ,'sortable': false }," +
                     //"{'title': 'Emp Code','data': 'column19' ,'sortable': false }," +
                     "{'title': 'Emp Name','data': 'column20' ,'sortable': false }," +
                     "{'title': 'Basic', 'data': 'column3' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'DA','data': 'column4' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'CCA','data': 'column5' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'HRA','data': 'column6' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'IR','data': 'column7' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Spl.DA','data': 'column10' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Spl.Allow','data': 'column9'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'Telangana Incr','data': 'column8' ,'sortable': false ,className: 'dt-body-right'}," +
                      // "{'title': 'Allowance Data','data': 'grpclmn' ,'sortable': false }," +
                      "{ 'title': 'CLS','data': 'column14' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'TOA','data': 'column15' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'IT','data': 'column12' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'PF','data': 'column11' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'PT','data': 'column13' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'PH','data': 'column21' ,'sortable': false ,className: 'dt-body-right' }," +
                      //allowances
                      "{'title':  'SPL Care Taker','data': 'SPLCareTaker','sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'Medical Allowance','data': 'MedAlw' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SPL Cashier','data': 'SPLCashier' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'Special Increment','data': 'SplIncr' ,'sortable': false ,className: 'dt-body-right' }," +
                       "{'title': 'Stagnation Increments','data': 'StagIncr' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'Annual Increment', 'data': 'AnlIncr' ,'sortable': false  ,className: 'dt-body-right' }," +
                     "{ 'title': 'CAIIB Increment','data': 'Caib' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'JAIIB Increment','data': 'Jaib' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'Employee Tds','data': 'EmployeeTds' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'LIC Premium','data': 'LICPremium' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Loss of Pay','data': 'LossofPay' ,'sortable': false ,className: 'dt-body-right'  }," +
                     "{ 'title': 'Special Pay','data': 'SpecialPay'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'System Administrator Allowance','data': 'SAdminAl' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'Teaching Allowance','data': 'TeachAlw' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'Washing Allowance','data': 'WashAlw' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'Fixed Allowance','data': 'FixAlw' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'Deputation Allowance','data': 'DepuAlw' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'Fixed Personal Allowance','data': 'FPA' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'FPA-HRA Allowance','data': 'FPAHRA' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'Interim Allowance','data': 'InterimAlw' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'FPIIP','data': 'FPIIPAlw' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'NPSG Allowance','data': 'NPSGAlw' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'Officiating Allowance', 'data': 'OfficiatingAlw' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Personal Pay','data': 'PPay' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Personal Qual Allowance','data': 'Pqa' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'Res. Attenders Allowance','data': 'ResAttenders' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'FP Incentive Recovery','data': 'FPIncentive' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Br Manager Allowance','data': 'BrManager' ,'sortable': false ,className: 'dt-body-right'  }," +
                     "{ 'title': 'Petrol & Paper','data': 'PetrolPaper'  ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Petrol & Paper1','data': 'PetrolPaper1' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'Children Education Allowance','data': 'CEA' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'Fest Advance','data': 'Fest' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'LT PF Loan','data': 'LTPFLoan' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'INCENTIVE','data': 'INCENTIVE' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'INCENTIVE DIFF','data': 'INCENTIVEDIFF' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'LTC ENCASHMENT','data': 'LTCENCASHMENT' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'SPL Driver','data': 'SPLDriver' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'SPL Jamedar','data': 'SPLJamedar' ,'sortable': false ,className: 'dt-body-right' }," +
                        "{ 'title': 'SPL Key','data': 'SPLKey' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPL Split Duty -Award staff','data': 'SPLSplitDutyAwardstaff' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPL Typist','data': 'SPLTypist' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPL Watchman','data': 'SPLWatchman' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'SPL Stenographer','data': 'SPLStenographer' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'SPL Bill Collector','data': 'SPLBillAlw' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'SPL Despatch','data': 'SPLDespatch' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'SPL Electrician', 'data': 'SPLElectrician' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'PSPL Dafter','data': 'SPLDafter' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'SPL Cash Cabin','data': 'SPLCashCabin' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'SPL Telephone Operator','data': 'SPLTelephoneOperator' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'SPL Library','data': 'SPLLibrary' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'SPL Incentive','data': 'SPLIncentive' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'SPL Arrear Incentive','data': 'SPLArrearIncentive'  ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'SPL Conveyance','data': 'SPLConveyance' ,'sortable': false ,className: 'dt-body-right'  }," +
                     "{'title': 'SPL Split Duty - Managers','data': 'SPLSplitDutyManagers' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPL Duplicating/xerox machine','data': 'SPLDuplicatingxeroxmachine' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPL Record room asst allowance','data': 'SPLRecordroomasstallowance' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPL Record room sub staff all','data': 'SPLRecordroomsubstaffall' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPL Receptionist allowance','data': 'SPLReceptionistallowance' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SPL Spl.Alw.ACSTI','data': 'SPLSplAlwACSTI' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'SPL Personal Pay','data': 'SPLPersonalPay' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'FACULTY ALLOWANCE','data': 'FACULTYAlw' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{'title':  'APCOB-HFC-LT','data': 'APCOBHFCLT' ,'sortable': false ,className: 'dt-body-right' }," +
                     //diductions
                     "{'title': 'APCOB-HFC-HO', 'data': 'APCOBHFCHO' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'VIJAYA Coop Society Deduction','data': 'VIJAYACoopSocietyDeduction' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'VISAKHA Coop Society Deduction','data': 'VISAKHACoopSocietyDeduction' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'GSLI','data': 'GSLI' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Officers Assn Fund','data': 'OfficersAssnFund' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'Cadre Officers Assn Fund','data': 'CadreOfficersAssnFund' ,'sortable': false ,className: 'dt-body-right'  }," +
                     "{ 'title': 'Club Subscription','data': 'ClubSubscription'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'Union Club Subscription','data': 'UnionClubSubscription' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'SC/ST Assn LT Subscription','data': 'SCSTAssnLTSubscription' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'SC/ST Assn ST Subscription','data': 'SCSTAssnSTSubscription' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'LT Staff Benefit Fund','data': 'LTStaffBenefitFund' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'VPF Deduction','data': 'VPFDeduction' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'VPF Percentage','data': 'VPFPercentage' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'LIC - PENSION','data': 'LICPENSION' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'JEEVAN SURAKSHA','data': 'JEEVANSURAKSHA' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'HDFC','data': 'HDFC' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'CCA - AP','data': 'CCAAP' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'CCS - HYD', 'data': 'CCSHYD' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'AB-HLF(HYD)','data': 'ABHLFHYD' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'APCCADB - EMP CCS','data': 'APCCADBEMPCCS' ,'sortable': false ,className: 'dt-body-right'}, " +
                     "{ 'title': 'APCOB-ED.LOAN-HNR.BR','data': 'APCOBEDLOANHNRBR' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'APCOB-ED.LOAN-HO','data': 'APCOBEDLOANHO' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'APSCB-LT-Emp Assn','data': 'APSCBLTEmpAssn' ,'sortable': false  ,className: 'dt-body-right'}," +
                     "{ 'title': 'DR-CCS-VIZAG','data': 'DRCCSVIZAG'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'PRN-JR CVL JUDGE, VIZAG','data': 'PRNJRCVLJUDGEVIZAG' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': '1 ADDL JR CVL JUDGE, TANUKU','data': 'ADDLJRCVLJUDGEANUKU' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'KADAPA DCCB','data': 'KADAPADCCB' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'JR CVL JUDGE, SULLURPET','data': 'JRCVLJUDGESULLURPET' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'KML COOP BANK, VIZAG','data': 'KMLCOOPBANKVIZAG' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'ESTT-EXCESS HRA REC','data': 'ESTTEXCESSHRAREC' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'FESTIVAL ADVANCE','data': 'FESTIVALADVANCE' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'ANDHRA BANK, RAMANTHPUR','data': 'ANDHRABANKRAMANTHPUR' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'SR.CIVIL JUDGE, KADAPA','data': 'SRCIVILJUDGEKADAPA' ,'sortable': false ,className: 'dt-body-right'}," +
                        "{ 'title': 'MEDICAL ADVANCE RECOVERY','data': 'MEDICALADVANCERECOVERY' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'XIX JR.CVL JUDGE, C C COURT, HYDERABAD','data': 'XIXJRCVLJUDGECCCOURTHYDERABAD' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'BANKS EMP ASSN TELANGANA','data': 'BANKSEMPASSNTELANGANA' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'LIC MACHILIPATNAM','data': 'LICMACHILIPATNAM' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'VEHICLE LOAN MACHILIPATNAM','data': 'VEHICLELOANMACHILIPATNAM' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{'title':  'FESTIVAL ADVANCE MEDAK','data': 'FESTIVALADVANCEMEDAK' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'FEST ADV MEDAK','data': 'FESTADVMEDAK' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{'title': 'XI JR.CVL JUDGE, C C COURT, SEC', 'data': 'XIJRCVLJUDGECCCOURTSEC' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'FESTIVAL ADVANCE ELURU','data': 'FESTIVALADVANCEELURU' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'TELANGANA EMP UNION','data': 'TELANGANAEMPUNION' ,'sortable': false ,className: 'dt-body-right' }, " +
                     "{ 'title': 'DCCB DEDUCTION','data': 'DCCBDEDUCTION' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'COURT DEDUCTION','data': 'COURTDEDUCTION' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'LIFE INSURANCE','data': 'LIFEINSURANCE' ,'sortable': false ,className: 'dt-body-right' }," +
                     "{ 'title': 'TELANGANA OFFICERS ASSN','data': 'TELANGANAOFFICERSASSN'  ,'sortable': false ,className: 'dt-body-right'}," +
                     "{ 'title': 'TSCAB OFFICERS ASSN','data': 'TSCABOFFICERSASSN' ,'sortable': false ,className: 'dt-body-right'}," +
                     "{'title': 'MISC. DEDUCTION','data': 'MISCDEDUCTION' ,'sortable': false ,className: 'dt-body-right' }," +
                      "{ 'title': 'PERSONAL LOAN','data': 'PERSONALLOAN' ,'sortable': false ,className: 'dt-body-right'}," +

                      "{ 'title': 'Gross Amount','data': 'column16' ,'sortable': false  ,className: 'dt-body-right'}," +
                      "{ 'title': 'Deduction Amount','data': 'column18' ,'sortable': false ,className: 'dt-body-right'}," +
                      "{ 'title': 'Net Amount','data': 'column17' ,'sortable': false ,className: 'dt-body-right'}] ";
            ViewBag.ReportColumnsCount = 20;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> TSheetReportData(string inputMonth, string RegEmp, string SupEmp)
        {
            TsheetReportBusiness tbus = new TsheetReportBusiness(LoginHelper.GetCurrentUserForPR());
            string ipmn = "01-01-01";
            if (inputMonth != "^1")
            {
                ipmn = inputMonth;
            }

            return JsonConvert.SerializeObject(await tbus.GetTsheetdata(ipmn, RegEmp, SupEmp));
        }

        #endregion

        #region Loan report  by RAJYALAKSHMI
        public ActionResult EmpLoanReport()
        {
            setReportCommonViewBag();
            ViewBag.ReportFiltersTemplate = "T5-AllAndSingleEmpSearchRbtn";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.reportTitle = "Loan Installments";
            ViewBag.PdfNoOfCols = 6;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6]";
            // ViewBag.PdfColumnsWidths = "85,80,65,100,55,55,55";
            //ViewBag.PdfGrpColHeaderLabels = "Loan(s): ,,,,,";
            ViewBag.PdfGrpColHeaderLabels = "Emp Code: ,Emp Name: ,Designation:,Branch:,,";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/LoanReportGroupingData?empcode=^1";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
                "{'title': 'Loan Type','data': 'grpclmn' ,'sortable': false }," +
              // " { 'title': 'Loan Type','data': 'column1' ,'sortable': false }," +
              " {'title': 'Loan Amount', 'data': 'column2' ,'sortable': false ,className: 'dt-body-right'}," +
              " { 'title': 'Installment Amount','data': 'column3'  ,'sortable': false,className: 'dt-body-right'}," +
              " { 'title': 'No of Installments','data': 'column4'  ,'sortable': false}," +
              " { 'title': 'Installment Paid ','data': 'column5' ,'sortable': false }," +
              " { 'title': 'Balance Installment','data': 'column6' ,'sortable': false }]";
            ViewBag.ReportColumnsCount = 6;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> LoanReportGroupingData(string empcode)
        {
            return JsonConvert.SerializeObject(await lrbus.GetLoanGroupingReports(empcode));
        }
        #endregion 

        #region Form12BAReport by RAJYALAKSHMI
        public ActionResult Form12BAReport()
        {
            setReportCommonViewBag();
            ViewBag.ReportFiltersTemplate = "T5-AllAndSingleEmpSearchRbtn";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.ReportTitle = "Form 12BA ";
            ViewBag.PdfNoOfCols = 4;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4]";
            // ViewBag.PdfColumnsWidths = "85,80,65,100,55,55,55";
            //ViewBag.PdfGrpColHeaderLabels = "Loan(s): ,,,,,";
            ViewBag.PdfGrpColHeaderLabels = "Emp Code: ,Emp Name: ,Designation:,Branch:,,";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/Form12BReportGroupingData?empcode=^1";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
                "{'title': 'Nature of perquisite(see rule 3)','data': 'grpclmn' ,'sortable': false }," +
              //" { 'title': 'Nature of perquisite(see rule 3)','data': 'column1' ,'sortable': false }," +
              " {'title': 'Value of perquisite', 'data': 'column2' ,'sortable': false }," +
              " { 'title': 'Recovered Amount','data': 'column3' ,'sortable': false ,className: 'dt-body-right'}," +
              " { 'title': 'Amount of perquisite Chargeable to Tax', 'data': 'column4'  ,'sortable': false,className: 'dt-body-right'}]";
            ViewBag.ReportColumnsCount = 4;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> Form12BReportGroupingData(string empcode)
        {
            return JsonConvert.SerializeObject(await fbus.GetForm12BGroupingReports(empcode));
        }
        #endregion
        #region Pf contribution card by Lalitha 19/08/2020
        public async Task<ActionResult> Pfcontributioncardhrms()
        {
            setReportCommonViewBag();
         
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
            lCredentials = LoginHelper.GetCurrentUserForPR();
            ViewBag.ReportTitle = "PF Contribution Card";

            ViewBag.ReportFiltersTemplate = "GroupingTPFINSTCAL-AllEmpSearchOneMultiSelOneDropdown";
            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getFy";
            var fYears = await lbus.getFy();
            ViewBag.T3DropdownList = fYears;
            ViewBag.Empname = lCredentials.EmpShortName;
            ViewBag.empcode = lCredentials.EmpCode;
            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.textboxempcode = "Employees Code";
            //ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFinterestcalData?empcode=^1&fYear=^2";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4, 5]";

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PfcontributioncardDatahrms";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
                    "{ 'title': 'Month','data': 'grpclmn','sortable': false}," +
                 "{ 'title': 'OWN','data': 'column2' ,'sortable': false}," +
                 "{ 'title': 'BANK','data': 'column3' ,'sortable': false}," +
                  "{ 'title': 'VPF','data': 'column4','sortable': false}," +
                  "{ 'title': 'Total','data': 'column5','sortable': false}" +
              "]";
            ViewBag.ReportColumnsCount = 5;
            ViewBag.ReportFooterColumnsCount = 5;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterViewhrms.cshtml");

        }
      
        public async Task<string> PfcontributioncardDatahrms()
        {
            setReportCommonViewBag();
           
                lCredentials = LoginHelper.GetCurrentUserForPR();
                string empCode = Convert.ToString(lCredentials.EmpCode);
           
            TDSReportBusiness TDSBus = new TDSReportBusiness(LoginHelper.GetCurrentUserForPR());
            var doc = await TDSBus.getOBShareData(empCode);
            //return JsonConvert.SerializeObject(await TDSBus.getOBShareData(empCode));
            return JsonConvert.SerializeObject(doc);
        }
        #endregion

        #region All AllAllowance Emp wise Report by Indraja
        public async Task<ActionResult> AllAllowanceEmpwise()
        {
            setReportCommonViewBag();

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getTypes";
            AllAllowanceBusiness AllABus = new AllAllowanceBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await AllABus.getAllowanceTypes();

            ViewBag.ReportTitle = "All Allowance Emp Wise";
            ViewBag.ReportFiltersTemplate = "T7-EmpcodeOneMonthPickeronedropdown";
            ViewBag.T3MultiSelLabel = "Allowances ";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Emp  Code";
            ViewBag.OneMonthPickerLabel1 = "Month ";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/AllAllowanceData?emp_code=^1&AllowanceTypes=^2&mnth=^3";

            ViewBag.PdfNoOfCols = 3;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4]";
            //ViewBag.PdfColumnsWidths = "160,150,180";
            //ViewBag.PdfGrpColHeaderLabels = "Code: ,Emp Name: ,Designation:";
           
            //ViewBag.PdfColumnsWidths = "170,160,160";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
               "{'title': 'Sl.No','data': 'grpclmn' ,'sortable': false }," +
              //  "{'title': 'data','data': 'grpclmn' }," +
              " { 'title': 'Allowance','data': 'column1' ,'sortable': false}," +
              " {'title': 'Pay Benfit', 'data': 'column2' ,'sortable': false}," +
              " { 'title': 'Amount ','data': 'column3' ,'sortable': false ,className: 'dt-body-right'}," +
              
              "]";
            ViewBag.ReportColumnsCount = 4;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> AllAllowanceData(string emp_code, string AllowanceTypes, string mnth)
        {
            AllAllowanceBusiness AllABus = new AllAllowanceBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await AllABus.AllAllowanceData(emp_code, AllowanceTypes, mnth);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region encashment Register Repport By indraja
        public ActionResult EncashmentRegisterReports()
        {
            lCredentials = LoginHelper.GetCurrentUserForPR();
            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";

            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.ReportTitle = "Encashment Register ";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";
           
            //ViewBag.PdfNoOfCols = 11;
            //ViewBag.ExportColumns = "columns: [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18]";
            //ViewBag.PdfColumnsWidths = "30,50,45,50,50,45,50,50,50,50,40";

            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 ]";
            ViewBag.PdfNoOfCols = 14;
            ViewBag.PdfColumnsWidths = "35,20,40,30,30,45,50,50,50,40,40,20,20,20";
            ViewBag.fm = lCredentials.FinancialMonthDate;
          
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/EncashmentRegisterReportData?fromDate=^1&toDate=^2";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',  'autoWidth': true, 'visible': false }," +
                "{'title': 'Emp Code','data': 'grpclmn' ,'sortable': false}," +
              //"{'title': 'Emp Code','data': 'column1' ,'sortable': false}," +
              " { 'title': 'Emp Name','data': 'column2' ,'sortable': false}," +
              " {'title': 'DESG', 'data': 'column3','sortable': false }," +
              //" {'title': 'CAT', 'data': 'column4','sortable': false }," +
              " { 'title': 'BASIC ','data': 'column5','sortable': false,className: 'dt-body-right' }," +
              //" { 'title': 'INCR ','data': 'column7','sortable': false }," +
              //" { 'title': 'ALLW ','data': 'column8','sortable': false }," +
              " { 'title': 'DA','data': 'column6','sortable': false,className: 'dt-body-right' }," +
              " { 'title': 'CCA ','data': 'column7','sortable': false,className: 'dt-body-right' }," +
              " { 'title': 'HRA','data': 'column8','sortable': false,className: 'dt-body-right' }," +
               " { 'title': 'SplDA','data': 'column9','sortable': false,className: 'dt-body-right' }," +
              " { 'title': 'SplAllow ','data': 'column10','sortable': false,className: 'dt-body-right' }," +
             " { 'title': 'PF','data': 'column11','sortable': false,className: 'dt-body-right' }," +
              " { 'title': 'VPF ','data': 'column12','sortable': false,className: 'dt-body-right' }," +
              " { 'title': 'GROSS ','data': 'column13','sortable': false,className: 'dt-body-right' }," +

              //" { 'title': 'IT','data': 'column17','sortable': false }," +
              " { 'title': 'TOTDED','data': 'column14','sortable': false,className: 'dt-body-right' }," +
              " { 'title': 'Net','data': 'column15','sortable': false,className: 'dt-body-right' }]";
            ViewBag.ReportColumnsCount = 14;
            ViewBag.ReportFooterColumnsCount = 3;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> EncashmentRegisterReportData(string fromDate, string toDate)
        {
            EncashmentReportBusiness ebus = new EncashmentReportBusiness(LoginHelper.GetCurrentUserForPR());

            return JsonConvert.SerializeObject(await ebus.GetEncashmentReportdata(fromDate, toDate));
        }
        #endregion

        #region All AllAllowance Wise Report by Indraja
        public async Task<ActionResult> AllAllowanceWise()
        {
            setReportCommonViewBag();

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getTypes";
            AllAllowanceBusiness AllABus = new AllAllowanceBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await AllABus.getAllowanceTypes();
            ViewBag.ReportTitle = "All Allowance Wise";
            ViewBag.ReportFiltersTemplate = "T7-EmpcodeOneMonthPickeronedropdown";

            ViewBag.T3MultiSelLabel = "Allowances ";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Emp  Code";
            ViewBag.OneMonthPickerLabel1 = "Month ";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/AllAllowanceWiseData?emp_code=^1&AllowanceTypes=^2&mnth=^3";

            ViewBag.PdfNoOfCols = 5;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6,7]";
            ViewBag.PdfColumnsWidths = "150,75,80,70,80,50";
            ViewBag.PdfGrpColHeaderLabels = "Allowance Name:,,,,,";
            
            //ViewBag.PdfColumnsWidths = "80,80,90,80,80,80";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
               "{'title': 'Sl.No','data': 'grpclmn' ,'sortable': false }," +
                 " { 'title': 'Emp Code','data': 'column1' ,'sortable': false}," +
                " { 'title': 'Emp Name','data': 'column2' ,'sortable': false}," +
                " { 'title': 'Designation','data': 'column3' ,'sortable': false}," +
                " { 'title': 'Allowance Description','data': 'column4','sortable': false }," +
                " {'title': 'Pay Benfit', 'data': 'column5' ,'sortable': false}," +
                " { 'title': 'Amount ','data': 'column6','sortable': false,className: 'dt-body-right' }," +
                "]";
            ViewBag.ReportColumnsCount = 7;

            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> AllAllowanceWiseData(string emp_code, string AllowanceTypes, string mnth)
        {
            AllAllowanceBusiness AllABus = new AllAllowanceBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await AllABus.AllAllowanceWiseData(emp_code, AllowanceTypes, mnth);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region  Loan Schedule Report By RAJYALAKSHMI
        [HttpGet]
        public ActionResult LoanScheduleReport()
        {
            setReportCommonViewBag();
            ViewBag.ReportTitle = "Loan & Schedule ";
            ViewBag.ReportFiltersTemplate = "TLS-TwoRadioButtonsOneMonthPicker";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.OneMonthPickerLabel1 = "Month ";
            ViewBag.PdfNoOfCols = 6;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6]";
            ViewBag.PdfColumnsWidths = "95,70,100,75,60,75";
           // ViewBag.PdfGrpColHeaderLabels = "Loan(s): ,,,,,";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/LoanScheduleReportData?loancode=^1&schedulecode=^2&mnth=^3";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
               " { 'title': 'Emp Code','data': 'grpclmn' ,'sortable': false }," +
              //" {'title': 'Employee Code', 'data': 'column1' ,'sortable': false }," +
              " {'title': 'Emp Name', 'data': 'column2' ,'sortable': false }," +
              " { 'title': 'Designation','data': 'column3','sortable': false }," +
              " { 'title': 'Amount','data': 'column4' ,'sortable': false ,className: 'dt-body-right'}," +
              " { 'title': 'Installments Paid ','data': 'column5' ,'sortable': false }," +
              " { 'title': 'No.Of Installments','data': 'column6' ,'sortable': false }]";
            ViewBag.ReportColumnsCount = 6;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        

        }

        public async Task<string> LoanScheduleReportData(string loancode, string schedulecode, string mnth)
        {
            LoanScheduleReportBusiness lsrbus = new LoanScheduleReportBusiness(LoginHelper.GetCurrentUserForPR());
            return JsonConvert.SerializeObject(await lsrbus.GetLoanScheduleReports(loancode, schedulecode, mnth));
        }
        #endregion

        #region EncashmentSummary Report BY RAJYALAKSHMI
        [HttpGet]
        public ActionResult EncashSummary()
        {

            setReportCommonViewBag();
            ViewBag.T3MultiSelLabel = "Branch";
            ViewBag.ReportFiltersTemplate = "T14-TwoRadioButtonTodatePicker";
            ViewBag.OneMonthPickerLabel1 = "Month ";
            ViewBag.ReportTitle = "Encashment Summary";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";
            ViewBag.fm = lCredentials.FinancialMonthDate;
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/EncashSummarytData?fromDate=^1&toDate=^2&RegEmp=^3&SupEmp=^4";
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6]";
            // ViewBag.PdfColumnsWidths = "100,80,80,75,80,75";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId' , 'visible': false }," +
                 "{ 'title': 'Gross Salary','data': 'SlNo' ,'sortable': false ,className: 'dt-body-right'}," +
              //"{ 'title': 'Gross Salary','data': 'column1'  ,'sortable': false}," +
              "{ 'title': 'Provident Fund','data': 'column2' ,'sortable': false ,className: 'dt-body-right'}," +
           " {'title': 'VPF', 'data': 'column3' ,'sortable': false ,className: 'dt-body-right'}," +
           "{ 'title': 'Income Tax','data': 'column4' ,'sortable': false ,className: 'dt-body-right'}," +
           "{ 'title': 'Total Deductions','data': 'column5' ,'sortable': false ,className: 'dt-body-right'}," +
            "{ 'title': 'Net Salary','data': 'column6' ,'sortable': false ,className: 'dt-body-right'}]";
            ViewBag.ReportColumnsCount = 6;
            ViewBag.ReportFooterColumnsCount = 6;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> EncashSummarytData(string fromDate, string toDate, string RegEmp, string SupEmp)
        {
            EncashmentSummaryBus esbus = new EncashmentSummaryBus(LoginHelper.GetCurrentUserForPR());
            //string ipmn = "01-01-01";
            //if (inputMonth != "^1")
            //{
            //    ipmn = inputMonth;
            //}
            var report = await esbus.EncashSummaryData(fromDate, toDate, RegEmp, SupEmp);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region  SingleRateLoanOutStanding Report By RAJYALAKSHMI
        [HttpGet]
        public ActionResult SingleRateLoanOutStanding()
        {
            //setReportCommonViewBag();
            lCredentials = LoginHelper.GetCurrentUserForPR();
            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            ViewBag.ReportTitle = "Single Rate Loan OutStanding";
            ViewBag.ReportFiltersTemplate = "T11-OneRadioButtonOneMonthPicker";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.OneMonthPickerLabel1 = "Month ";

            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");

            ViewBag.PdfNoOfCols = 12;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]";
            ViewBag.PdfColumnsWidths = "10,50,20,40,50,43,50,30,40,50,30,30,30";
            // ViewBag.PdfGrpColHeaderLabels = "Loan(s): ,,,,,";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/SingleRateLoanOutStandingData?loancode=^1&schedulecode=^2&mnth=^3";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId' , 'visible': false }," +
                  "{ 'title': 'Emp Code','data': 'SlNo' ,'sortable': false,className: 'dt-body-right' }," +
               //"{ 'title': 'Emp Code','data': 'column1'  ,'sortable': false}," +
               "{ 'title': 'Emp Name','data': 'column2' ,'sortable': false }," +
               "{'title': 'Desg', 'data': 'column3' ,'sortable': false,className: 'dt-body-right' }," +
               "{ 'title': 'IntRate','data': 'column4' ,'sortable': false,className: 'dt-body-right' }," +
               "{ 'title': 'Principal Opening','data': 'column5' ,'sortable': false,className: 'dt-body-right' }," +
               "{ 'title': 'Curr_Month Received','data': 'column6' ,'sortable': false,className: 'dt-body-right' }," +
               "{ 'title': 'Principal Paid','data': 'column12' ,'sortable': false,className: 'dt-body-right' }," +
               "{ 'title': 'Loan Closing','data': 'column7' ,'sortable': false,className: 'dt-body-right' }," +
               "{ 'title': 'Interest Opening','data': 'column8' ,'sortable': false,className: 'dt-body-right' }," +
                "{ 'title': 'Interest Accured','data': 'column9' ,'sortable': false,className: 'dt-body-right' }," +
               "{ 'title': 'Interest Repaid','data': 'column10' ,'sortable': false,className: 'dt-body-right' }," +
               "{ 'title': 'Interest Closing','data': 'column11' ,'sortable': false,className: 'dt-body-right' }]";
            ViewBag.ReportColumnsCount = 12;
            ViewBag.ReportFooterColumnsCount = 3;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");


        }

        public async Task<string> SingleRateLoanOutStandingData(string loancode, string mnth)
        {
            SingleRateLoanOutStandingBus srbus = new SingleRateLoanOutStandingBus(LoginHelper.GetCurrentUserForPR());
            return JsonConvert.SerializeObject(await srbus.GetSingleRateLoanOutStandingData(loancode, mnth));
        }
        #endregion
        #region  SingleRateLoanOutStanding Report By Lalitha 20/08/2020
        [HttpGet]
        public ActionResult SingleRateLoanOutStandinghrms()
        {
            //setReportCommonViewBag();
            lCredentials = LoginHelper.GetCurrentUserForPR();
            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            ViewBag.ReportTitle = "Single Rate Loan OutStanding";
            ViewBag.ReportFiltersTemplate = "T11-OneRadioButtonOneMonthPicker";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.OneMonthPickerLabel1 = "Month ";

            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd"); 
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.PdfNoOfCols = 12;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]";
            ViewBag.PdfColumnsWidths = "10,50,20,40,50,43,50,30,40,50,30,30,30";
            // ViewBag.PdfGrpColHeaderLabels = "Loan(s): ,,,,,";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/SingleRateLoanOutStandingDatahrms?empcode=^1&mnth=^2";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId' , 'visible': false }," +
                  "{ 'title': 'Emp Code','data': 'SlNo' ,'sortable': false }," +
               //"{ 'title': 'Emp Code','data': 'column1'  ,'sortable': false}," +
               "{ 'title': 'Employee Name','data': 'column2' ,'sortable': false }," +
            " {'title': 'Desg', 'data': 'column3' ,'sortable': false }," +
            "{ 'title': 'IntRate','data': 'column4' ,'sortable': false }," +
            "{ 'title': 'Principal Opening','data': 'column5' ,'sortable': false }," +
             "{ 'title': 'Curr_Month Received','data': 'column6' ,'sortable': false }," +
             "{ 'title': 'Principal Paid','data': 'column12' ,'sortable': false }," +
               "{ 'title': 'Loan Closing','data': 'column7' ,'sortable': false }," +
               "{ 'title': 'Interest Opening','data': 'column8' ,'sortable': false }," +
                 "{ 'title': 'Interest Accured','data': 'column9' ,'sortable': false }," +
             "{ 'title': 'Interest Repaid','data': 'column10' ,'sortable': false }," +
             "{ 'title': 'Interest Closing','data': 'column11' ,'sortable': false }]";
            ViewBag.ReportColumnsCount = 12;
            ViewBag.ReportFooterColumnsCount = 3;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllSinglerateViewhrms .cshtml");


        }

        public async Task<string> SingleRateLoanOutStandingDatahrms(string empcode, string mnth)
        {
            setReportCommonViewBag();
            lCredentials = LoginHelper.GetCurrentUserForPR();
            empcode = Convert.ToString(lCredentials.EmpCode);
            SingleRateLoanOutStandingBus srbus = new SingleRateLoanOutStandingBus(LoginHelper.GetCurrentUserForPR());
            return JsonConvert.SerializeObject(await srbus.GetSingleRateLoanOutStandingDatahrms(empcode, mnth));
        }
        #endregion
        #region GeneralVochers Report BY RAJYALAKSHMI
        [HttpGet]
        public ActionResult GeneralVochers()
        {
            setReportCommonViewBag();
            //    //*** get Branches list
            //ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getBranches";
            //ViewBag.T3MultiSelLabel = "Branch";
            ViewBag.ReportFiltersTemplate = "T10-FourRadioButtonsOneMonthPicker";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.ReportTitle = "General Vochers";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/GeneralVochersData?mnpkrVal1=^1&RegEmpval=^2&SupEmpval=^3&Debitval=^4&Creditval=^5&All=^6";
            ViewBag.ExportColumns = "columns: [1, 2, 3,]";
            // ViewBag.PdfColumnsWidths = "100,80,80,75,80,75";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId' , 'visible': false }," +
                 "{ 'title': ' Description of GL Head','data': 'SlNo' ,'sortable': false }," +
              "{ 'title': 'FAS GL CODE','data': 'column2' ,'sortable': false }," +
           " {'title': 'Amount', 'data': 'column3' ,'sortable': false, className: 'dt-body-right'}]";
            ViewBag.ReportColumnsCount = 3;
            ViewBag.ReportFooterColumnsCount = 3;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> GeneralVochersData(string mnpkrVal1, string RegEmpval, string SupEmpval, string Debitval, string Creditval,string All)
        {
            string ipmn = "01-01-01";
            if (mnpkrVal1 != "^1")
            {
                ipmn = mnpkrVal1;
            }
            var report = await Gvbus.GeneralVochersData(ipmn, RegEmpval, SupEmpval, Debitval, Creditval, All);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region InterfaceVochers Report BY RAJYALAKSHMI
        [HttpGet]
        public ActionResult InterfaceVochers()
        {
            setReportCommonViewBag();
            //    //*** get Branches list
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getBranches";
            ViewBag.T3MultiSelLabel = "Branch";
            ViewBag.ReportFiltersTemplate = "T10-FourRadioButtonsOneMonthPicker";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.ReportTitle = "Interface Vochers";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/InterfaceVochersData?mnpkrVal1=^1&RegEmpval=^2&SupEmpval=^3&Debitval=^4&Creditval=^5&All=^6";
            ViewBag.ExportColumns = "columns: [1, 2,]";
            // ViewBag.PdfColumnsWidths = "100,80,80,75,80,75";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId' , 'visible': false }," +
             "{ 'title': ' ','data': 'SlNo' ,'sortable': false }," +
              //"{ 'title': 'FAS GL CODE','data': 'column2' ,'sortable': false }," +
           " {'title': ' ', 'data': 'column3' ,'sortable': false }]";
            ViewBag.ReportColumnsCount = 2;
            ViewBag.ReportFooterColumnsCount = 2;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> InterfaceVochersData(string mnpkrVal1, string RegEmpval, string SupEmpval, string Debitval, string Creditval, string All)
        {
            string ipmn = "01-01-01";
            if (mnpkrVal1 != "^1")
            {
                ipmn = mnpkrVal1;
            }
            var report = await IntBus.InterfaceVochersData(ipmn, RegEmpval, SupEmpval, Debitval, Creditval,All);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region Employees Loan Projection Report By indhraja
        [HttpGet]
        public async Task<ActionResult> LoanProjection()
        {
            //setReportCommonViewBag();
            lCredentials = LoginHelper.GetCurrentUserForPR();
            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getLoanTypes";

            //*** get loan types ***
            LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
            var LoanTypesList = await lbus.getloantypes();
            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getFy";
            var fYears = await lbus.getFy();
            ViewBag.T3DropdownList = fYears;

            ViewBag.ReportTitle = "Loan Projection";
            ViewBag.ReportFiltersTemplate = "GroupingT2-AllEmpSearchOneMultiSelOneDropdown";
            ViewBag.T3MultiSelLabel = "Loan(s) ";
            ViewBag.T3MultiSelList = LoanTypesList;

            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.textboxempcode = "Employees Code";

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/LoanLedgerData?Loans=^2&empcode=^1&fYear=^3&priority=^4";

            ViewBag.PdfNoOfCols = 12;
            ViewBag.ExportColumns = "columns: [ 1,2,3 , 4, 5, 6, 7, 8, 9, 10,11,12]";
            ViewBag.PdfColumnsWidths = "50,30,60,50,50,50,40,40,50,40,40,40";
            ViewBag.PdfGrpColHeaderLabels = " , , , , ,,,,";

            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
               "{'title': 'Month  ','data': 'grpclmn'  }," +
               "{'title': 'Priority','data': 'column12'  }," +
                // //"{ 'title': 'Emp Code','data': 'emp_code' , 'sortable': false}," +
                //"{ 'title': 'Month','data': 'column1', 'sortable': false}," +
                "{ 'title': 'Loan Opening','data': 'column2', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': 'Interest Rate %','data': 'column11', 'sortable': false}," +
                "{ 'title': 'Loan Repaid ','data': 'column3', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': 'Loan Closing','data': 'column4', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': 'Interest Opening','data': 'column5', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': 'Interest Accrued','data': 'column6', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': 'Interest Repaid','data': 'column7', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': 'Interest Closing','data': 'column8', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': 'Installment Repaid','data': 'column9', 'sortable': false,className: 'dt-body-right'}," +
            //" {'title': 'Amount Issued', 'data': 'amount_issued'  , 'sortable': false}," +
           " {'title': 'Loan Type', 'data': 'column10', 'sortable': false}]"; ;
            ViewBag.ReportColumnsCount = 12;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> LoanLedgerData(string Loans, string empcode, string fYear, string priority)
        {
            LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
            return JsonConvert.SerializeObject(await lbus.GetLedgerReports(empcode, Loans, fYear,priority ));
        }

        #endregion

        #region LoanLedger Report by Indraja and Nishanth
        [HttpGet]
        public async Task<ActionResult> EmployeesLoanLedger()
        {

            //setReportCommonViewBag();

            lCredentials = LoginHelper.GetCurrentUserForPR();
            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";

            //*** get loan types ***
            ViewBag.T3MultiSelLabels = "Loan(s) ";
            EmployeeLoanDetailsBusiness ELDB = new EmployeeLoanDetailsBusiness(LoginHelper.GetCurrentUserForPR());

            LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
            var LoanTypesList = await ELDB.getloantypes();
            ViewBag.T3MultiSelLists = LoanTypesList;

            //*** get Branches list

            //0.
            ViewBag.ReportTitle = "Loan Ledger";

            //1.
            ViewBag.ReportFiltersTemplate = "T9-TwoDropDowns";
          
            //3.
            var fYears = await lbus.getFy();
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");

            ViewBag.PdfNoOfCols = 11;
            ViewBag.ExportColumns = "columns: [1,2,3 , 4, 5, 6, 7, 8, 9, 10, 11, 12, 13]";
            ViewBag.PdfColumnsWidths = "50,55,65,50,50,50,40,40,40";
            ViewBag.PdfGrpColHeaderLabels = " , , , , ,,,,";

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/EmployeesLoanDetailsData?Year=^1&Loans=^2&empcode=^3&ptiority=^4";
            //ViewBag.ExportColumns = "columns: [0, 1, 2, 3, 4]";
            //ViewBag.ReportColumns = "[{'title': 'Branch Info','data': 'grpcol'  }," +
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
              "{'title': 'Month    ','data': 'grpclmn' ,'sortable': false }," +
                " { 'title': 'Priority','data': 'column15' ,'sortable': false }," +
               //" { 'title': 'Month','data': 'column2' ,'sortable': false }," +
               " { 'title': 'Loan Amount','data': 'column3' ,'sortable': false,className: 'dt-body-right' }," +
                " { 'title': 'Interest Rate %','data': 'column13' ,'sortable': false }," +
                  " {'title': 'Installment Amount ', 'data': 'column4' ,'sortable': false,className: 'dt-body-right' }," +
                 " { 'title': 'Principal Open ','data': 'column5' ,'sortable': false,className: 'dt-body-right' }," +
                " { 'title': 'Principal paid ','data': 'column14' ,'sortable': false,className: 'dt-body-right' }," +
                   " { 'title': 'Principal Balance ','data': 'column6' ,'sortable': false,className: 'dt-body-right' }," +
               " { 'title': 'Interest Open ','data': 'column8' ,'sortable': false,className: 'dt-body-right' }," +
               " { 'title': 'Interest Accured','data': 'column7' ,'sortable': false,className: 'dt-body-right' }," +
               " { 'title': 'Interest Paid ','data': 'column9' ,'sortable': false,className: 'dt-body-right' }," +
               " { 'title': 'Interest Balance ','data': 'column10' ,'sortable': false,className: 'dt-body-right' }," +
               " { 'title': 'Payment Type','data': 'column12','sortable': false  }]";
            ViewBag.ReportColumnsCount = 13;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> EmployeesLoanDetailsData( string Year, string Loans, string empcode,string ptiority)
        {
            EmployeeLoanDetailsBusiness ELDB = new EmployeeLoanDetailsBusiness(LoginHelper.GetCurrentUserForPR());
            return JsonConvert.SerializeObject(await ELDB.GetLoanDetails( Year, Loans, empcode, ptiority));
        }
        #endregion

        #region All Loan Report by Uma as Footer

        [HttpGet]
        public  ActionResult AllLoanReport()
        {

            setReportCommonViewBag();
            ViewBag.ReportTitle = "All Loan ";

            ViewBag.ReportFiltersTemplate = "T10-OneMonthPickerTwoRadioButtons";
            ViewBag.T3MultiSelLabel = "Deduction Type";
            //ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Emp  Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/AllLoanReportData?inputMonth=^1&RegEmp=^2&SupEmp=^3";

            ViewBag.ExportColumns = "columns: [1, 2, 3, 4,5,6,7]";
            ViewBag.PdfColumnsWidths = "50,150,80,50,50,50,50";
            ViewBag.ReportColumns = "[" +
                "{'title': 'RowID', 'data': 'RowId', 'visible': false }," +
                "{ 'title': '','data': 'column1' ,'sortable': false}," +
                "{ 'title': '','data':'column2' ,'sortable': false}," +
                "{ 'title': '','data': 'column3' ,'sortable': false}," +
                "{ 'title': '','data': 'column4' ,'sortable': false}," +
                "{ 'title': '','data': 'column5' ,'sortable': false,className: 'dt-body-right' }," +
                "{ 'title': '','data': 'column6' ,'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column7' ,'sortable': false,className: 'dt-body-right'}]";

            ViewBag.ReportColumnsCount = 7;
            ViewBag.ReportFooterColumnsCount = 0;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }

        public async Task<string> AllLoanReportData(string inputMonth, string RegEmp, string SupEmp)
        {
            string ipmn = "01-01-01";
            if (inputMonth != "^1")
            {
                ipmn = inputMonth;
            }

            AllLoansReportBusiness LICB = new AllLoansReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await LICB.GetLoanReport(ipmn, RegEmp, SupEmp);
            return JsonConvert.SerializeObject(report);

        }

        #endregion

        #region Pf repayable sanctiondate by Lalitha

        //PF non repayable sanctiondate

        [HttpGet]
        public async Task<ActionResult> PfRepayableSanctionReport()
        {

            setReportCommonViewBag();
            //    //*** get Branches list
            PFPayableReportBusiness PFNR = new PFPayableReportBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await PFNR.getloans();
            ViewBag.T3MultiSelLabel = "Loan Types";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.ReportFiltersTemplate = "T7-OneMonthPickeronedropdown";
            // ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";
            ViewBag.ReportTitle = "Refundable Advances";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFRepayableSanData?loan=^1&from=^2&To=^3";
            ViewBag.ExportColumns = "columns: [ 1, 2,3,4,5]";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId', 'visible': false } ," +
                "{ 'title': 'SlNo','data': 'SlNo','sortable': false }," +
                 " { 'title': 'Emp Code','data': 'column2','sortable': false }," +
               " { 'title': 'Emp Name','data': 'column3','sortable': false }," +
                //" { 'title': 'Type of Loan','data': 'column6','sortable': false }," +
                   " { 'title': 'Sanction Date','data': 'column4','sortable': false }," +
               " { 'title': 'Sanction Amount','data': 'column5','sortable': false ,className: 'dt-body-right'}," +
              " ]";
            ViewBag.ReportColumnsCount = 5;
            ViewBag.ReportFooterColumnsCount = 4;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> PFRepayableSanData(string loan, string from, string To)
        {

            PFPayableReportBusiness PFNR = new PFPayableReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await PFNR.PFRepayableSanData(loan, from, To );
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region Pf nOnrepayable sanction by Lalitha

        //PF non repayable sanction 

        [HttpGet]
        public async Task<ActionResult> PfNonRepayableReport()
        {

            //setReportCommonViewBag();
            lCredentials = LoginHelper.GetCurrentUserForPR();

            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            //    //*** get Branches list
            PFNonPayableBusiness PFNR = new PFNonPayableBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await PFNR.getloans();
            ViewBag.T3MultiSelLabel = "Loan Types";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.ReportFiltersTemplate = "T7-OneMonthPickeronedropdown";
            //ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";
            ViewBag.ReportTitle = "NR Withdrawals";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFNonRepayableData?loan=^1&from=^2&To=^3";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4, 5, 6, 7, 8]";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId', 'visible': false } ," +
                "{ 'title': 'SlNo','data': 'SlNo','sortable': false }," +
                    " { 'title': 'Emp Code','data': 'column2','sortable': false }," +
               " { 'title': 'Emp Name','data': 'column3','sortable': false }," +
                 //" { 'title': 'Type of Loans','data': 'column4','sortable': false }," +
               " { 'title': 'Sanction Date','data': 'column5','sortable': false }," +
               " {'title': 'From Emp Contribution', 'data': 'column6','sortable': false ,className: 'dt-body-right'}," +
               " { 'title': 'From VPF Contribution','data': 'column7','sortable': false ,className: 'dt-body-right'}," +
               " { 'title': 'From Bank Contribution','data': 'column8','sortable': false ,className: 'dt-body-right'}," +
               " { 'title': 'Total Loan Amount','data': 'column9','sortable': false,className: 'dt-body-right' }," +
              " ]";
            ViewBag.ReportColumnsCount = 8;
            ViewBag.ReportFooterColumnsCount = 4;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> PFNonRepayableData(string loan, string from, string To)
        {

            PFNonPayableBusiness PFNR = new PFNonPayableBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await PFNR.PfNonRepayableReport(loan, from, To);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region   Pf nOnrepayable loanbreakup by Lalitha
        //PF non repayable loanbreakup

        [HttpGet]
        public async Task<ActionResult> PfNonRepayableReportloanbreakup()
        {

            //setReportCommonViewBag();
            lCredentials = LoginHelper.GetCurrentUserForPR();

            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            
            //    //*** get Branches list
            PFNonPayableBusiness PFNR = new PFNonPayableBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await PFNR.getloans();
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            ViewBag.T3MultiSelLabel = "Loan Types";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.ReportFiltersTemplate = "T7-OneMonthPickeronedropdown";
            //ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";
            ViewBag.ReportTitle = "NR Withdrawals";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFNonRepayableDataEffective?loan=^1&from=^2&To=^3";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4, 5, 6, 7]";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId', 'visible': false } ," +
                "{ 'title': 'SlNo','data': 'SlNo','sortable': false }," +
                 //"{'title': 'Emp Code','data': 'column8' ,'sortable': false }," +
                 //"{'title': 'Emp Name','data': 'column9' ,'sortable': false }," +
                 //" { 'title': 'Sanction Date','data': 'column10','sortable': false }," +
                 " { 'title': 'Loan Description','data': 'column2','sortable': false }," +
               " { 'title': 'No of Loans','data': 'column3','sortable': false }," +
               " {'title': 'From Emp Contribution', 'data': 'column4','sortable': false ,className: 'dt-body-right'}," +
               " { 'title': 'From VPF Contribution','data': 'column5','sortable': false ,className: 'dt-body-right'}," +
               " { 'title': 'From Bank Contribution','data': 'column6','sortable': false ,className: 'dt-body-right'}," +
               " { 'title': 'Total Loan Amount','data': 'column7','sortable': false ,className: 'dt-body-right'}," +
              " ]";
            ViewBag.ReportColumnsCount = 7;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> PFNonRepayableDataEffective(string loan, string from, string To)
        {

            PFNonPayableBusiness PFNR = new PFNonPayableBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await PFNR.PfNonRepayableReportEffective(loan,  from,  To);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region Pf repayable loanbreakup by Lalitha

        //PF non repayable loanbreakup

        [HttpGet]
        public async Task<ActionResult> PfRepayableReport()
        {

            setReportCommonViewBag();
            //    //*** get Branches list
            PFPayableReportBusiness PFNR = new PFPayableReportBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await PFNR.getloans();
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            ViewBag.T3MultiSelLabel = "Loan Types";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.ReportFiltersTemplate = "T7-OneMonthPickeronedropdown";
            // ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";
            ViewBag.ReportTitle = "Refundable Advances";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFRepayableData?loan=^1&from=^2&To=^3";
            ViewBag.ExportColumns = "columns: [ 1,2,3,4]";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',  'visible': false } ," +
                "{ 'title': 'SlNo','data': 'SlNo','sortable': false }," +

                //" { 'title': 'Emp Code','data': 'column6','sortable': false}," +
                //" { 'title': 'Name','data': 'column5','sortable': false}," +
                //" { 'title': 'Sanction Date','data': 'column7','sortable': false}," +
                " { 'title': 'Loan Description','data': 'column8','sortable': false}," +
               " { 'title': 'No of Loans','data': 'column3','sortable': false }," +
               " { 'title': 'Total Loan Amount','data': 'column4','sortable': false,className: 'dt-body-right' }," +
              " ]";
            ViewBag.ReportColumnsCount = 4;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> PFRepayableData(string loan, string from, string To)
        {

            PFPayableReportBusiness PFNR = new PFPayableReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await PFNR.PFRepayableData(loan, from, To);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region Pf repayable effectivedate by Lalitha

        //PF  repayable effectivedate

        [HttpGet]
        public async Task<ActionResult> PfRepayableEffectiveReport()
        {
            lCredentials = LoginHelper.GetCurrentUserForPR();

            //ViewBag.SectionName = "Reports";
            //ViewBag.LoginUserName = lCredentials.EmpShortName;
            //ViewBag.LoginBranch = lCredentials.BranchCode;
            //ViewBag.LoginBranchName = lCredentials.BranchName;
            //ViewBag.PdfOrientation = "landscape";
            //DateTime Financial_md = (lCredentials.FinancialMonthDate);
            //ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            setReportCommonViewBag();
            //    //*** get Branches list
            PFPayableReportBusiness PFNR = new PFPayableReportBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await PFNR.getloans();
            ViewBag.T3MultiSelLabel = "Loan Types";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.ReportFiltersTemplate = "T7-OneMonthPickeronedropdown";
            //ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";

            ViewBag.ReportTitle = "Refundable Advances";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFRepayableEffData?loan=^1&from=^2&To=^3";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4, 5, 6, 7, 8]";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',  'visible': false } ," +
                "{ 'title': 'SlNo','data': 'SlNo','sortable': false }," +
                 " { 'title': 'Emp Code','data': 'column2','sortable': false }," +
               " { 'title': ' Emp Name','data': 'column3','sortable': false }," +
                 //" { 'title': 'Branch','data': 'column4','sortable': false }," +
               " { 'title': 'Designation','data': 'column5','sortable': false }," +
                " { 'title': 'Loan Type','data': 'column10','sortable': false }," +
                   " { 'title': 'Sanction Date','data': 'column7','sortable': false }," +
                     " { 'title': 'Process Date','data': 'column8','sortable': false }," +
               " { 'title': 'Sanction Amount','data': 'column9','sortable': false ,className: 'dt-body-right'}," +
              " ]";
            ViewBag.ReportColumnsCount = 8;
            ViewBag.ReportFooterColumnsCount = 7;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> PFRepayableEffData(string loan, string from, string To)
        {

            PFPayableReportBusiness PFNR = new PFPayableReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await PFNR.PFRepayableEffData(loan,  from,  To);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region All TDS Report by Uma as Footer

        public ActionResult TDSReport()
        {
            setReportCommonViewBag();
            ViewBag.ReportTitle = "TDS";
            ViewBag.ReportFiltersTemplate = "T1-SearchtdsEmpcodes";
            ViewBag.T3MultiSelLabel = "Deduction Type";
            //ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Emp  Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/TDSReportData?EmpCode=^1";
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4]";
            ViewBag.PdfColumnsWidths = "50,150,80,50";
            ViewBag.ReportColumns = "[" +
                "{'title': 'RowID', 'data': 'RowId', 'visible': false }," +
                "{ 'title': ' ','data': 'column1' ,'sortable': false}," +
                "{ 'title': ' ','data':'column2' ,'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': ' ','data': 'column3' ,'sortable': false}," +
                "{ 'title': ' ','data': 'column4' ,'sortable': false}]";
            ViewBag.ReportColumnsCount = 4;

            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> TDSReportData(string EmpCode)
        {


            TDSReportBusiness tdsb = new TDSReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await tdsb.getTdsDetails(EmpCode);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region Pf nOnrepayable effective by Lalitha

        //PF non repayable Effective 

        [HttpGet]
        public async Task<ActionResult> PfNonRepayableEffdateReport()
        {

            lCredentials = LoginHelper.GetCurrentUserForPR();
            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            //    //*** get Branches list
            PFNonPayableBusiness PFNR = new PFNonPayableBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await PFNR.getloans();


            ViewBag.T3MultiSelLabel = "Loan Types";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.ReportFiltersTemplate = "T7-OneMonthPickeronedropdown";
            //ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";
            lCredentials = LoginHelper.GetCurrentUserForPR();
          
            ViewBag.ReportTitle = "NR Withdrawals";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFNonRepayableEffData?loan=^1&from=^2&To=^3";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]";
          //  ViewBag.PdfColumnsWidths = "85,80,65,100,55,55,55,10,10,10,10";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',   'visible': false } ," +
                "{ 'title': 'SlNo','data': 'SlNo','sortable': false }," +
                    " { 'title': 'Emp Code','data': 'column2','sortable': false }," +
               " { 'title': 'Emp Name','data': 'column3','sortable': false }," +
                " { 'title': 'Designation','data': 'column4','sortable': false }," +
                 " { 'title': 'Loan Purpose','data': 'column12','sortable': false }," +
               " { 'title': 'Sanction Date','data': 'column6','sortable': false }," +
                " { 'title': 'Process Date','data': 'column7','sortable': false }," +
               " {'title': 'From Emp Contribution', 'data': 'column8','sortable': false,className: 'dt-body-right' }," +
               " { 'title': 'From VPF Contribution','data': 'column9','sortable': false ,className: 'dt-body-right'}," +
               " { 'title': 'From Bank Contribution','data': 'column10','sortable': false,className: 'dt-body-right' }," +
               " { 'title': 'Total Loan Amount','data': 'column11','sortable': false ,className: 'dt-body-right'}," +
              " ]";
            ViewBag.ReportColumnsCount = 11;
            ViewBag.ReportFooterColumnsCount = 7;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> PFNonRepayableEffData(string loan, string from, string To)
        {

            PFNonPayableBusiness PFNR = new PFNonPayableBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await PFNR.PfNonRepayableEffReport(loan, from, To);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region All Deductions sowjanya
        public ActionResult AllDeduction()
        {
            setReportCommonViewBag();

            AllDeductionBusiness AllABus = new AllDeductionBusiness(LoginHelper.GetCurrentUserForPR());
            ViewBag.ReportTitle = "All Deductions";
            ViewBag.ReportFiltersTemplate = "T10-DeducOneMonthPickerFourRadioButtons";
            ViewBag.OneMonthPickerLabel1 = "Month ";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/AllDeductionData?empType=^1&rptType=^2&month=^3";

            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5]";
            //ViewBag.PdfColumnsWidths = "90, 100, 100,100,100";
            ViewBag.ReportColumns = "[" +
                "{'title': 'RowID', 'data': 'RowId', 'visible': false }," +
                "{ 'title': ' ','data': 'column1' ,'sortable': false}," +
                "{ 'title': ' ','data':'column2' ,'sortable': false}," +
                "{ 'title': ' ','data': 'column3' ,'sortable': false}," +
                 "{ 'title': ' ','data': 'column4' ,'sortable': false}," +
                "{ 'title': ' ','data': 'column5' ,'sortable': false,className: 'dt-body-right'}]";
            ViewBag.ReportColumnsCount = 5;
            ViewBag.ReportFooterColumnsCount = 4;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> AllDeductionData(string empType, string rptType, string month)
        {
            string ipmn = "01-01-01";
            if (month != "^1")
            {
                ipmn = month;
            }
            AllDeductionBusiness AllABus = new AllDeductionBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await AllABus.AllDeduction(empType, rptType, ipmn);
            return JsonConvert.SerializeObject(report);

        }

        #endregion
       

        #region pfcontributionreport by Lalitha
        public ActionResult PFContributionEffMonth()
        {
            setReportCommonViewBag();

            ViewBag.ReportTitle = "EMPLOYEES CONTRIBUTION LEDGER FOR THE CONTRIBUTION EFFECTIVE MONTH";

            ViewBag.ReportFiltersTemplate = "T2-OneMonthPicker";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFContributionEffMonthData?inputMonth=^1";
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6, 7, 8, 9 ,10]";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId', 'visible': false,'sortable': false } ," +
                " { 'title': 'SlNo', 'data': 'column1','sortable': false }," +
                  // "{ 'title': 'EmpName','data': 'column3' }," +
                  "{ 'title': 'Emp Code','data': 'column16','sortable': false}," +
                  "{ 'title': 'Emp Name','data': 'column17','sortable': false}," +
                 "{ 'title': 'Employee Op.Bal','data': 'column4','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Employee Month','data': 'column5','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Employee Clg.Bal','data': 'column6', 'sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Employer Op.Bal','data': 'column7','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Employer Month','data': 'column8','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Employer Clg.Bal','data': 'column9','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Total','data': 'column10','sortable': false,className: 'dt-body-right'}," +
                  //"{ 'title': 'NR Loans','data': 'column11','sortable': false}," +
                  "{ 'title': 'Interest','data': 'column12','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'VPF During The Month','data': 'column13','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Opening Vpf','data': 'column14','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Closing Vpf','data': 'column15','sortable': false,className: 'dt-body-right'}]";
            ViewBag.ReportColumnsCount = 10;
            ViewBag.ReportFooterColumnsCount = 10;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }
        [HttpGet]
        public async Task<string> PFContributionEffMonthData(string inputMonth)
        {
            string ipmn = "01-01-01";
            if (inputMonth != "^1")
            {
                ipmn = inputMonth;
            }

            return JsonConvert.SerializeObject(await PFRB.PFContributionEffData(ipmn));
        }
        #endregion

        #region All Form16B General Report by Lalitha 

        public async Task <ActionResult> Form16B()
        {
            setReportCommonViewBag();
            ViewBag.ReportTitle = "Form 16  (General)";
            ViewBag.ReportFiltersTemplate = "T1-SearchEmpcodes";
            ViewBag.twoDtpickersLabel1 = "From ";
            ViewBag.twoDtpickersLabel2 = "To ";
            ViewBag.textboxempcode = "Emp  Code";
            LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
           
            var fYears = await lbus.getFyforform16b();
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/Form16BReportData?&EmpCode=^1&Year=^2";
            ViewBag.ExportColumns = "columns: [ 1, 2]";
            ViewBag.PdfColumnsWidths = "300,300";
            ViewBag.ReportColumns = "[" +
                "{'title': 'RowID', 'data': 'RowId', 'visible': false, }," +
                "{ 'title': ' ','data': 'column1' ,'sortable': false}," +
                "{ 'title': ' ','data':'column2' ,'sortable': false,className: 'dt-body-right'}," +
             " ]";
            ViewBag.ReportColumnsCount = 2;

            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> Form16BReportData(string EmpCode, string Year)
        {


            TDSReportBusiness tdsb = new TDSReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await tdsb.Form16BDetails(EmpCode, Year);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region  Multirate by Sowjanya
        public async Task<ActionResult> MultiRate()
        {

            setReportCommonViewBag();
            //    //*** get Branches list
            MultirateBusiness MRB = new MultirateBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await MRB.getloans();
            ViewBag.T3MultiSelLabel = "Loan Types";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.ReportFiltersTemplate = "T7-MultiOneMonthPickeronedropdown";
            ViewBag.OneMonthPickerLabel1 = "Month ";
            ViewBag.ReportTitle = "Multirate";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/MultiRateData?loan=^1&inputMonth=^2";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4, 5, 6, 7, 8]";
            ViewBag.PdfColumnsWidths = "85,90, 50,50, 60,40,20,20";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',   'visible': false } ," +
                "{ 'title': 'SlNo','data': 'column1','sortable': false,className: 'dt-body-right' }," +
                "{ 'title': 'Emp Code','data': 'column2','sortable': false,className: 'dt-body-right' }," +
                "{ 'title': 'Emp Name','data': 'column3','sortable': false,className: 'dt-body-right' }," +
                "{ 'title': 'Designation','data': 'column4','sortable': false,className: 'dt-body-right' }," +
                "{ 'title': 'HIGH-AMT  INT-CUM  PEND-PRN','data': 'column5','sortable': false,className: 'dt-body-right' }," +
                "{ 'title': 'HIGH-RECV INT-YEAR ','data': 'column6','sortable': false,className: 'dt-body-right' }," +
                "{ 'title': 'LOW-AMT REC-CUM PEND-INT','data': 'column7','sortable': false,className: 'dt-body-right' }," +
                "{ 'title': 'LOW-RECV REC-YEAR BALANCE','data': 'column8','sortable': false,className: 'dt-body-right' }," +
              " ]";
            ViewBag.ReportColumnsCount = 8;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }

        public async Task<string> MultiRateData(string loan, string inputMonth)
        {

            MultirateBusiness MRB = new MultirateBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await MRB.Multirate(loan, inputMonth);
            return JsonConvert.SerializeObject(report);

        }

        #endregion

        #region pf Interest Calculations report by Lalitha
        public async Task<ActionResult> PFInterestCalculations()
        {
            LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
            setReportCommonViewBag();

            ViewBag.ReportTitle = "PF INTEREST CALCULATIONS";

            ViewBag.ReportFiltersTemplate = "GroupingTPFINSTCAL-AllEmpSearchOneMultiSelOneDropdown";
            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getFy";
            var fYears = await lbus.getFy();
            ViewBag.T3DropdownList = fYears;

          
            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFinterestcalData?empcode=^1&fYear=^2";
             ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4, 5, 6, 7]";
            //ViewBag.PdfColumnsWidths = "50,50,50,50,140,150";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',   'visible': false } ," +
                 //"{ 'title': 'SlNo','data': 'SlNo','sortable': false }," +
                 //" { 'title': 'PF ACCOUNT NUMBER', 'data': 'grpclmn' }," +
                 "{ 'title': 'Month','data': 'grpclmn'}," +
                 "{ 'title': 'OWN','data': 'column3' }," +
                 "{ 'title': 'BANK','data': 'column4'}," +
                  "{ 'title': 'VPF','data': 'column5'}," +
                  "{ 'title': 'OWN INT','data': 'column6'}," +
                  "{ 'title': 'BANK INT','data': 'column7'}," +
                  "{ 'title': 'VPF INT','data': 'column8'}," +
                  " ]";
            ViewBag.ReportColumnsCount = 7;
            
            return  View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");
        }
        [HttpGet]
        public async Task<string> PFinterestcalData(string empcode,string fYear)
        {
            

            return JsonConvert.SerializeObject(await PFRB.PFintcalData(empcode, fYear));
        }
        #endregion


        #region Pf contribution card by indraja
        public async Task<ActionResult> Pfcontributioncard()
        {
            setReportCommonViewBag();
            LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
            ViewBag.ReportTitle = "PF Contribution Card";

            ViewBag.ReportFiltersTemplate = "GroupingTPFINSTCAL-AllEmpSearchOneMultiSelOneDropdown";
            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getFy";
            var fYears = await lbus.getFy();
            ViewBag.T3DropdownList = fYears;

            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.textboxempcode = "Employees Code";
            //ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFinterestcalData?empcode=^1&fYear=^2";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4, 5]";

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PfcontributioncardData?empcode=^1&fYear=^2";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
                    "{ 'title': 'Month','data': 'grpclmn','sortable': false}," +
                 "{ 'title': 'OWN','data': 'column2' ,'sortable': false,className: 'dt-body-right'}," +
                 "{ 'title': 'BANK','data': 'column3' ,'sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'VPF','data': 'column4','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Total','data': 'column5','sortable': false,className: 'dt-body-right'}" +
              "]";
            ViewBag.ReportColumnsCount = 5;
            ViewBag.ReportFooterColumnsCount = 5;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> PfcontributioncardData(string empCode, string fYear)
        {
           
            return JsonConvert.SerializeObject(await PFRB.PfcontributioncardData(empCode, fYear));
        }
        #endregion
        #region Pf Final Settlement card by Lalitha
        public async Task<ActionResult> PfFinalSettlement()
        {
            setReportCommonViewBag();
            LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
            ViewBag.ReportTitle = "PF Final Settlement";

            ViewBag.ReportFiltersTemplate = "GroupingTPFINSTSET-AllEmpSearchOneMultiSelOneDropdown";
            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getFy";
            var fYears = await lbus.getFy();
            ViewBag.T3DropdownList = fYears;

            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Date:";
            ViewBag.textboxempcode = "Employees Code";
            //ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFinterestcalData?empcode=^1&fYear=^2";
            ViewBag.ExportColumns = "columns: [ 1, 2]";

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PffinalsetlementData?empcode=^1&fdate=^2";
            ViewBag.ReportColumns = "[" +
                "{'title': 'RowID', 'data': 'RowId', 'visible': false }," +
                "{ 'title': ' ','data': 'column1' ,'sortable': false}," +
                "{ 'title': ' ','data':'column2' ,'sortable': false,className: 'dt-body-right'}," +
             " ]";
            ViewBag.ReportColumnsCount = 2;

            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> PffinalsetlementData(string empCode, string fdate)
        {

            var report= await PFRB.PfFinalSettlementData(empCode, fdate);
            return JsonConvert.SerializeObject(report);
        }
        #endregion

        #region Employees Yearly Pay Report By Sowjanya
        [HttpGet]
        public async Task<ActionResult> YearlyPay()
        {
            setReportCommonViewBag();

            YearlyPayBusiness YPBus = new YearlyPayBusiness(LoginHelper.GetCurrentUserForPR());
            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getFy";
            var fYears = await YPBus.getFy();
            ViewBag.T3DropdownList = fYears;

            ViewBag.ReportTitle = "Yearly Pay";
            ViewBag.ReportFiltersTemplate = "GroupingT2-AllEmpSearchOneDropdown";
           

            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.textboxempcode = "Employees Code";

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/YearlyPayData?empcode=^1&fYear=^2";

            ViewBag.PdfNoOfCols = 9;
            ViewBag.ExportColumns = "columns: [  1,2,3 , 4, 5, 6, 7, 8, 9,10,11,12,13,14]";
            ViewBag.PdfColumnsWidths = "30,30,30,30,30,30,40,40,40,30,30,30,30,30";
            ViewBag.PdfGrpColHeaderLabels = " , , , , , , , , , , , , , , , ";

            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
                "{ 'title': '','data': 'column1', 'sortable': false}," +
                "{ 'title': '','data': 'column2', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column3', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column4', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column5', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column6', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column7', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column8', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column9', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column10', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column11', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column12', 'sortable': false,className: 'dt-body-right'}," +
                "{ 'title': '','data': 'column13', 'sortable': false,className: 'dt-body-right'}," +
                "{'title': '', 'data': 'column14', 'sortable': false,className: 'dt-body-right'}]"; 
            ViewBag.ReportColumnsCount = 14;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/YearlyPayView.cshtml");
        }

        public async Task<string> YearlyPayData(string empcode, string fYear)
        {
            YearlyPayBusiness YPBus = new YearlyPayBusiness(LoginHelper.GetCurrentUserForPR());
            return JsonConvert.SerializeObject(await YPBus.GetYearlypayData(empcode, fYear));
        }

        #endregion

        #region form 8 Report by Indraja
        public async Task<ActionResult> Form8()
        {
            setReportCommonViewBag();

           
            ViewBag.ReportTitle = "Pension Annual Report(Form :8)";
            ViewBag.ReportFiltersTemplate = "T16-onedatepicker";

            //ViewBag.textboxempcode = "Emp  Code";
            //ViewBag.OneMonthPickerLabel1 = "Month ";
            JAIIB_CAIIB_Business JACA = new JAIIB_CAIIB_Business(LoginHelper.GetCurrentUserForPR());
            var fYears = await JACA.getFy();
            ViewBag.T3DropdownList = fYears;
            ViewBag.T3MultiSelList = fYears;
            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/Form8Data?Fyear=^1";

            ViewBag.PdfNoOfCols = 5;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6,7]";
            ViewBag.PdfColumnsWidths = "150,75,80,70,80,50";
            ViewBag.PdfGrpColHeaderLabels = "Allowance Name:,,,,,";

            //ViewBag.PdfColumnsWidths = "80,80,90,80,80,80";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
               "{'title': 'Sl.No','data': 'SlNo' ,'sortable': false }," +
                 " { 'title': 'Emp Code','data': 'column1' ,'sortable': false}," +
                  " { 'title': 'Pf Code','data': 'column2' ,'sortable': false}," +
                " { 'title': 'Emp Name','data': 'column3' ,'sortable': false}," +
                " { 'title': 'Designation','data': 'column4' ,'sortable': false}," +
                " { 'title': ' Pensionable Wages ','data': 'column5','sortable': false }," +
                " {'title': 'Annual Pension Contribution', 'data': 'column6' ,'sortable': false}," +
                //" { 'title': 'Amount ','data': 'column6','sortable': false }," +
                "]";
            ViewBag.ReportColumnsCount = 7;

            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> Form8Data(string Fyear)
        {
            Form8 Form8 = new Form8(LoginHelper.GetCurrentUserForPR());
            var report = await Form8.form8(Fyear);
            return JsonConvert.SerializeObject(report);

        }
        #endregion        
        #region Form 3A by sowjanya
               
        public async Task<ActionResult> Form3A()
        {
            setReportCommonViewBag();
            LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
            ViewBag.ReportTitle = "Form 3A";

            ViewBag.ReportFiltersTemplate = "Groupingform3-AllEmpSearchOneMultiSelOneDropdown";
            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getFy";
            var fYears = await lbus.getFy();
            ViewBag.T3DropdownList = fYears;

            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.textboxempcode = "Employees Code";
            //ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFinterestcalData?empcode=^1&fYear=^2";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4, 5, 6,7,8]";

            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/form3AData?empcode=^1&fYear=^2";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
                  "{ 'title': 'Month','data': 'column1','sortable': false}," +
                  "{ 'title': 'Amount Of Wages','data': 'column2' ,'sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'E.P.F.','data': 'column3' ,'sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'E.P.F. Difference Between 12% & 8.33% (4a)','data': 'column4','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Pension Fund Contribution 8.33% (4b)','data': 'column5','sortable': false,className: 'dt-body-right'}," +
                  "{ 'title': 'Refund Of Advance ','data': 'column6','sortable': false,className: 'dt-body-right'}," +

                  "{ 'title': 'No.Of Days/ Period Of Non-Contribute Services','data': 'column7','sortable': false}," +
                  "{ 'title': 'No.Of Days/ Period Of Non-Contribute Services','data': 'column7','sortable': false}," +
                   "{ 'title': 'Remarks','data': 'column8','sortable': false,className: 'dt-body-right'}" +
              "]";
            ViewBag.ReportColumnsCount = 8;
            ViewBag.ReportFooterColumnsCount = 8;
            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> form3AData(string empCode, string fYear)
        {

            return JsonConvert.SerializeObject(await F3A.form3AData(empCode, fYear));
        }
        #endregion

        //#region  Form24 By sowjanay
        //public async Task<ActionResult> Form24()
        //{
        //    setReportCommonViewBag();
        //    LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
        //    ViewBag.ReportTitle = "Form 3A";

        //    ViewBag.ReportFiltersTemplate = "Groupingform3-AllEmpSearchOneMultiSelOneDropdown";
        //    //Financial Period Drop downlist
        //    ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/getFy";
        //    var fYears = await lbus.getFy();
        //    ViewBag.T3DropdownList = fYears;

        //    ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
        //    ViewBag.T3DdlOneLabel = "Financial Period:";
        //    ViewBag.textboxempcode = "Employees Code";
        //    //ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PFinterestcalData?empcode=^1&fYear=^2";
        //    ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4, 5, 6,7,8]";

        //    ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/form24Data?empcode=^1&fYear=^2";
        //    ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
        //            "{ 'title': 'Month','data': 'column1','sortable': false}," +
        //         "{ 'title': 'Amount Of Wages','data': 'column2' ,'sortable': false}," +
        //         "{ 'title': 'E.P.F.','data': 'column3' ,'sortable': false}," +
        //          "{ 'title': 'E.P.F. Difference Between 12% & 8.33% (4a)','data': 'column4','sortable': false}," +
        //          "{ 'title': 'Pension Fund Contribution 8.33% (4b)','data': 'column5','sortable': false}," +
        //          "{ 'title': 'Refund Of Advance ','data': 'column6','sortable': false}," +
        //              "{ 'title': 'No.Of Days/ Period Of Non-Contribute Services','data': 'column7','sortable': false}," +
        //                  "{ 'title': 'Remarks','data': 'column8','sortable': false}" +
        //      "]";
        //    ViewBag.ReportColumnsCount = 8;
        //    ViewBag.ReportFooterColumnsCount = 8;
        //    return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        //}
        //public async Task<string> form24Data(string empCode, string fYear)
        //{

        //    return JsonConvert.SerializeObject(await F3A.form3AData(empCode, fYear));
        //}

        //#endregion


        #region PF Found by Indraja
        public async Task<ActionResult> PfFund()
        {
            //setReportCommonViewBag();

            lCredentials = LoginHelper.GetCurrentUserForPR();

            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");

            ViewBag.ReportTitle = "PF Fund";
            ViewBag.ReportFiltersTemplate = "T16-onedatepicker";

            //ViewBag.textboxempcode = "Emp  Code";
            //ViewBag.OneMonthPickerLabel1 = "Month ";
            JAIIB_CAIIB_Business JACA = new JAIIB_CAIIB_Business(LoginHelper.GetCurrentUserForPR());
            var fYears = await JACA.getFy();
            ViewBag.T3DropdownList = fYears;
            ViewBag.T3MultiSelList = fYears;
            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.DataUrl = "/Payroll/PrAllGroupFooter/PfFundData?Fyear=^1&inputtype=^2&currentmnth=^3";

            ViewBag.PdfNoOfCols = 5;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6,7,8,9,10,11,12]";
            ViewBag.PdfColumnsWidths = "50,75,80,100,80,50";
            ViewBag.PdfGrpColHeaderLabels = "Allowance Name:,,,,,";

            //ViewBag.PdfColumnsWidths = "80,80,90,80,80,80";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
               "{'title': 'Sl.No','data': 'SlNo' ,'sortable': false }," +
                 " { 'title': 'Emp Code','data': 'column1' ,'sortable': false}," +
                  " { 'title': 'Pf Code','data': 'column2' ,'sortable': false}," +
                " { 'title': 'Emp Name','data': 'column3' ,'sortable': false}," +
                " { 'title': 'Designation','data': 'column4' ,'sortable': false}," +
                " { 'title': ' Fund - Op - Bal ','data': 'column5','sortable': false ,className: 'dt-body-right'}," +
                " {'title': 'Cont-Year', 'data': 'column6' ,'sortable': false,className: 'dt-body-right'}," +
                  " { 'title': ' Int-Op-Bal ','data': 'column7','sortable': false ,className: 'dt-body-right'}," +
                    " { 'title': ' Int-Year','data': 'column8','sortable': false ,className: 'dt-body-right'}," +
                " { 'title': 'Nr-Loan-Op ','data': 'column9','sortable': false ,className: 'dt-body-right'}," +
                  " { 'title': 'Nr-Loan-Yr ','data': 'column10','sortable': false,className: 'dt-body-right' }," +
                   " { 'title': 'Net-fund ','data': 'column11','sortable': false ,className: 'dt-body-right' }," +
                //" { 'title': 'Net-fund','data': 'column11','sortable': false }," +
                "]";
            ViewBag.ReportColumnsCount = 12;
            

            return View("~/Areas/Payroll/Views/PrAllGroupFooter/PrAllGroupingFooterView.cshtml");

        }
        public async Task<string> PfFundData(string Fyear, string inputtype,string currentmnth)
        {
            PFFundBusiness PfFund = new PFFundBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await PfFund.PfFund(Fyear, inputtype, currentmnth);
            return JsonConvert.SerializeObject(report);
        }
        #endregion        

    }
}