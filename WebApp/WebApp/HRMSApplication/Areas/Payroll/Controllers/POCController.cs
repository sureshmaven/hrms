using HRMSApplication.Helpers;
using Newtonsoft.Json;
using PayRollBusiness.Process;
using PayRollBusiness.Reports;
using PayrollModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class POCController : Controller
    {
        EncashmentReportBusiness ebus = new EncashmentReportBusiness(LoginHelper.GetCurrentUserForPR());
        TsheetReportBusiness tbus = new TsheetReportBusiness(LoginHelper.GetCurrentUserForPR());
        LoginCredential lCredentials = null;
        private void setReportCommonViewBag()
        {
            lCredentials = LoginHelper.GetCurrentUserForPR();

            ViewBag.SectionName = "POC";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            //ViewBag.PdfOrientation = "landscape";
            ViewBag.PdfOrientation = "portrait";

        }

        #region POC by Raju K - Grid Headers Issue

        public ActionResult GridHeadersIssue()
        {
            setReportCommonViewBag();

            ViewBag.ReportTitle = "Form 5A";

            ViewBag.ReportFiltersTemplate = "T2-OneMonthPicker";
            ViewBag.OneMonthPickerLabel1 = "Month and Year";

            ViewBag.DataUrl = "/Payroll/POC/Form5AData?inputMonth=^1";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3]";
            ViewBag.PdfColumnsWidths = "126,119,119,126";
            ViewBag.ReportColumns = "[{'title': 'Employee Monthly Salary / Wages / both', 'data': 'EmpMonthSalary' }," +
                "{ 'title': 'Number of Employees','data': 'NumberofEmployees'}," +
                "{ 'title': 'Rate of Tax per Month','data': 'TaxperMonth' }," +
                "{ 'title': 'Amount of Tax Deducted','data': 'TaxDeducted'}]"; ;

            return View("~/Areas/Payroll/Views/POC/PocHeadersIssue.cshtml");
        }
        [HttpGet]
        public async Task<string> Form5AData(string inputMonth)
        {
            string ipmn = "01-01-01";
            if (inputMonth != "^1")
            {
                ipmn = inputMonth;
            }

            Form5AReport FRM5A = new Form5AReport(LoginHelper.GetCurrentUserForPR());

            return JsonConvert.SerializeObject(await FRM5A.getForm5AData(ipmn));
        }

        #endregion

        #region POC by Raju K
        public ActionResult PocMessageTop()
        {
            setReportCommonViewBag();

            ViewBag.ReportTitle = "Poc Title 123";

            ViewBag.ReportFiltersTemplate = "T1-poc1";
            ViewBag.twoDtpickersLabel1 = "Date 1";
            ViewBag.twoDtpickersLabel2 = "Date 2";

            ViewBag.DataUrl = "/Payroll/POC/PocMessageTopData";
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6, 7]";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',  'autoWidth': true, 'visible': false }," +
                "{ 'title': 'SlNo','data': 'SlNo', 'autoWidth': true }," +
                "{ 'title': 'Code','data': 'EmpCode',  'autoWidth': true }," +
                "{ 'title': 'Emp Name','data': 'EmpName',  'autoWidth': true }," +
                "{ 'title': 'Designation','data': 'Designation',  'autoWidth': true }," +
                "{ 'title': 'Gross Salary','data': 'GrossSalary',  'autoWidth': true }," +
                "{ 'title': 'Deductions','data': 'Deductions',  'autoWidth': true }," +
                "{ 'title': 'Net Salary','data': 'NetSalary', 'autoWidth': true }]";

            return View("~/Areas/Payroll/Views/POC/PocAllReports.cshtml");
        }

        [HttpGet]
        public string PocMessageTopData()
        {
            PocReportBusiness bus = new PocReportBusiness(lCredentials);
            return JsonConvert.SerializeObject(bus.GetPoc1Data());
        }
        #endregion

        // GET: Payroll/POC
        #region POC1
        public ActionResult POC1()
        {
            ViewBag.SectionName = "POC";

            return View();
        }
        #endregion

        #region poc pview

        [HttpGet]
        public async Task<ActionResult> partialview()
        {
            setReportCommonViewBag();

            ViewBag.All = "EmpAll";
            ViewBag.Empwise = "EmpCode";

            ViewBag.ReportFiltersTemplate = "GroupingT1-AllEmpSearchOneMultiSelOneMonthpic";

            return View("~/Areas/Payroll/Views/POC/partialview.cshtml");
        }


        #endregion

        #region POC GroupTable by RK
        public ActionResult GroupByPOC()
        {
            setReportCommonViewBag();

            ViewBag.ReportFiltersTemplate = "T5-AllAndSingleEmpSearchRbtn";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.ReportTitle = "LoanReport";

            ViewBag.PdfNoOfCols = 6;
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3,4,5,6]";
            ViewBag.PdfColumnsWidths = "80,80,80,60,90,50";
            ViewBag.PdfGrpColHeaderLabels = "Ecode:,Name:,Designation:,Br:,,";

            ViewBag.DataUrl = "/Payroll/POC/LoanReportGroupingData?empcode=^1";

            ViewBag.ReportColumns = "[{'title': 'Employee Information','data': 'grpcol','autoWidth': true }," +
              " { 'title': 'Loan Type','data': 'loan_description', 'autoWidth': true }," +
              " {'title': 'Loan Amount', 'data': 'total_amount',  'autoWidth': true }," +
              " { 'title': 'Installment Amount ','data': 'installment_amount',  'autoWidth': true }," +
              " { 'title': 'Period','data': 'period', 'autoWidth': true }," +
              " { 'title': 'Inst Paid ','data': 'INSTPAID',  'autoWidth': true }," +
              " { 'title': 'Balance Inst','data': 'BALANCEINST',  'autoWidth': true }]";
            return View();
        }
        public async Task<string> LoanReportGroupingData(string empcode)
        {
            empcode = "All";
            LoanReportBusiness lrbus = new LoanReportBusiness(lCredentials);
            return JsonConvert.SerializeObject(await lrbus.GetLoanGroupingReports(empcode));
        }

        public ActionResult GrpByAllowencePOC()
        {
            setReportCommonViewBag();

            ViewBag.DataUrl = "/Payroll/POC/GrpByAllowencePOCData";

            ViewBag.PdfNoOfCols = 3;
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3]";
            ViewBag.PdfColumnsWidths = "150,150,190";
            ViewBag.PdfGrpColHeaderLabels = "Id:,Name:,Desi:";

            ViewBag.ReportColumns = "[{'title': 'Employee Information','data': 'grpcol','autoWidth': true }," +
              " { 'title': 'Allowance','data': 'Allowance', 'autoWidth': true }," +
              " {'title': 'Pay Benfit', 'data': 'benefit',  'autoWidth': true }," +
              " { 'title': 'Amount ','data': 'amount',  'autoWidth': true }]";

            return View("~/Areas/Payroll/Views/POC/GroupByPOC.cshtml");
        }
        public async Task<string> GrpByAllowencePOCData()
        {
            string emp_code = "All";
            string Types = null;
            string mnth = "Jun-2019";
            AllAllowanceBusiness AllABus = new AllAllowanceBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await AllABus.AllAllowanceData(emp_code, Types, mnth);
            return JsonConvert.SerializeObject(report);
        }
        #endregion

        #region grid chart by Vinod
        public ActionResult GridwithChartPdf()
        {
            return View();
        }
        #endregion

        #region LIC/HFC Report by Sowjanya as Footer

        [HttpGet]
        public async Task<ActionResult> POCLICReport()
        {
            setReportCommonViewBag();

            ViewBag.DataUrl = "/Payroll/POC/getTypes";

            //*** get Types(LIC/HFC) *** Total
            LICReportBusiness LICB = new LICReportBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await LICB.getTypes();

            ViewBag.ReportTitle = "LIC/HFC";

            ViewBag.ReportFiltersTemplate = "T4-AllEmpSearchOneMultiSelOneMonthpic";

            ViewBag.T3MultiSelLabel = "Deduction Type";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Emp  Code";
            ViewBag.OneMonthPickerLabel1 = "Month and Year";
            ViewBag.DataUrl = "/Payroll/POC/LICReportData?emp_code=^1&Types=^2&mnth=^3";
            ViewBag.ExportColumns = "columns: [1, 2, 3]";
            //ViewBag.PdfColumnsWidths = "80,130,90,90,100";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',  'autoWidth': true, 'visible': false }," +
                "{ 'title': 'SlNo','data': 'SlNo', 'autoWidth': true }," +
                "{ 'title': 'Account No ','data': 'Account_No',  'autoWidth': true }," +
                "{ 'title': 'Amount ','data': 'Amount',  'autoWidth': true },]";


            return View("~/Areas/Payroll/Views/POC/PocAllReports.cshtml");
        }

        public async Task<string> LICReportData(string emp_code, string Types, string mnth)
        {
            LICReportBusiness LICB = new LICReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await LICB.GetLICReport(emp_code, Types, mnth);
            return JsonConvert.SerializeObject(report);

        }

        #endregion

        #region encashment Register Repport By raji
        public ActionResult EncashmentRegisterReport()
        {
            setReportCommonViewBag();


            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.ReportTitle = "Loan Report";
            ViewBag.PdfNoOfCols = 11;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]";
            ViewBag.PdfColumnsWidths = "30,50,45,50,50,45,50,50,50,50,40";
            //ViewBag.ExportColumns = "columns: [1,2,3, 4, 5, 6, 7, 8, 9, 10, 11]";
            // ViewBag.ExportColumns = "columns: [0, 1, 2, 3,4,5,6]";
            //ViewBag.PdfColumnsWidths = "50,50,50,60,50,50,50,10,10,10,10";
            ViewBag.DataUrl = "/Payroll/POC/EncashmentRegisterReportData?fromDate=^1&toDate=^2";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',  'autoWidth': true, 'visible': false }," + 
                "{'title': '','data': 'grpclmn','autoWidth': true }," +
               "{'title': 'Empid','data': 'column2','autoWidth': true }," +
              " { 'title': 'empname','data': 'column3', 'autoWidth': true }," +
              " {'title': 'DESG CAT', 'data': 'column4','autoWidth': true }," +
              " { 'title': 'BASIC INCR ','data': 'column6', 'autoWidth': true }," +
              " { 'title': 'ALLW DA','data': 'column7', 'autoWidth': true }," +
              " { 'title': 'CCA HRA','data': 'column8', 'autoWidth': true }," +
              " { 'title': 'SplAllow SplDA','data': 'column9', 'autoWidth': true }," +
              " { 'title': 'GROSS PF','data': 'column10', 'autoWidth': true }," +
              " { 'title': 'VPF IT','data': 'column11', 'autoWidth': true }," +
              " { 'title': 'TOTDED','data': 'column12', 'autoWidth': true }," +
              " { 'title': 'net','data': 'column13', 'autoWidth': true }]";
            return View("~/Areas/Payroll/Views/POC/PocAllReports.cshtml");
        }
        public async Task<string> EncashmentRegisterReportData(string fromDate, string toDate)
        {
            return JsonConvert.SerializeObject(await ebus.GetEncashmentReportdata(fromDate, toDate));
        }
        #endregion


        #region TSheetReport By Raji
        public ActionResult TSheetReport()
        {
            setReportCommonViewBag();
            ViewBag.ReportTitle = "Tsheet";
            ViewBag.ReportFiltersTemplate = "T8-OneMonthPickerTwoRadioButtons";
            ViewBag.DataUrl = "/Payroll/POC/TSheetReportData?inputMonth=^1&RegEmp=^2&SupEmp=^3";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17]";
            ViewBag.PdfColumnsWidths = "20,30,45,45,30,30,30,30,30,30,30,30,20,30,30,35,35,45";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',  'autoWidth': true, 'visible': false }," +
                     "{'title': 'Branch','data': 'column1', 'autoWidth': true }," +
                     "{'title': 'Employee Code','data': 'column19', 'autoWidth': true }," +
                     "{'title': 'Employee Name','data': 'column20', 'autoWidth': true }," +
                     "{'title': 'Basic', 'data': 'column3',  'autoWidth': true }," +
                     "{ 'title': 'Da','data': 'column4',  'autoWidth': true }," +
                     "{ 'title': 'CCA','data': 'column5', 'autoWidth': true }, " +
                     "{ 'title': 'hra','data': 'column6', 'autoWidth': true }," +
                     "{ 'title': 'Intirim Relief','data': 'column7', 'autoWidth': true }," +
                     "{ 'title': 'Telangana Increment','data': 'column8', 'autoWidth': true }," +
                     "{ 'title': 'Spl Allw','data': 'column9', 'autoWidth': true }," +
                     "{ 'title': 'Spl Da','data': 'column10', 'autoWidth': true }," +
                     "{ 'title': 'Provident Fund','data': 'column11', 'autoWidth': true }," +
                     "{ 'title': 'Income Tax','data': 'column12', 'autoWidth': true }," +
                     "{ 'title': 'Prof Tax','data': 'column13', 'autoWidth': true }," +
                     "{ 'title': 'Club Subscription','data': 'column14', 'autoWidth': true }," +
                     "{ 'title': 'Telangana Officers Assn','data': 'column15', 'autoWidth': true }," +
                     "{'title': 'Allowance Data','data': 'grpclmn','autoWidth': true }," +
                      "{'title': 'Deductiondata Data','data': 'column2','autoWidth': true }," +
                      "{ 'title': 'Deductions Amount','data': 'column18', 'autoWidth': true }," +
                     "{ 'title': 'Gross','data': 'column16', 'autoWidth': true }," +
                     "{ 'title': 'Net','data': 'column17', 'autoWidth': true }] ";

            return View("~/Areas/Payroll/Views/POC/PocAllReports.cshtml");
        }

        public async Task<string> TSheetReportData(string inputMonth, string RegEmp, string SupEmp)
        {
            string ipmn = "01-01-01";
            if (inputMonth != "^1")
            {
                ipmn = inputMonth;
            }

            return JsonConvert.SerializeObject(await tbus.GetTsheetdata(ipmn, RegEmp, SupEmp));
        }

        #endregion
    }
}