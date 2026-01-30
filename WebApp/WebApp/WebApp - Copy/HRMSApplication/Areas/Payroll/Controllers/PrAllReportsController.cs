using HRMSApplication.Helpers;
using Newtonsoft.Json;
using PayRollBusiness.Masters;
using PayRollBusiness.Process;
using PayRollBusiness.Reports;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class PrAllReportsController : Controller
    {
        
        LoginCredential lCredentials = LoginHelper.GetCurrentUserForPR();
        PayslipPdfBusiness prpdffbud = new PayslipPdfBusiness(LoginHelper.GetCurrentUserForPR());
        AllowanceReportBusiness arbus = new AllowanceReportBusiness(LoginHelper.GetCurrentUserForPR());
        PFPayableBusiness PFRbus = new PFPayableBusiness(LoginHelper.GetCurrentUserForPR());
        LoanReportBusiness lrbus = new LoanReportBusiness(LoginHelper.GetCurrentUserForPR());
        LoanScheduleReportBusiness lsrbus = new LoanScheduleReportBusiness(LoginHelper.GetCurrentUserForPR());
        TsheetReportBusiness tbus = new TsheetReportBusiness(LoginHelper.GetCurrentUserForPR());
        TDSReportBusiness tds = new TDSReportBusiness(LoginHelper.GetCurrentUserForPR());
        JAIIB_CAIIB_Business JACA = new JAIIB_CAIIB_Business(LoginHelper.GetCurrentUserForPR());
        Form16Business Form16Bus = new Form16Business(LoginHelper.GetCurrentUserForPR());
        NegativeNetSalaryBusiness netSal = new NegativeNetSalaryBusiness(LoginHelper.GetCurrentUserForPR());

        public ActionResult Index()
        {
            ViewBag.SectionName = "Reports";
            return View();
        }

        private void setReportCommonViewBag()
        {
            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            //ViewBag.PdfOrientation = "landscape";
            // ViewBag.PdfOrientation = "portrait";
            ViewBag.PdfOrientation = "portrait";
            //string fm = (lCredentials.FM).ToString();
            //string fy = lCredentials.FY.ToString();
            //string Financial_md = (lCredentials.FinancialMonthDate).ToShortDateString();
            //ViewBag.fm = Financial_md;
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
        }
        private void setReportCommonViewBag1()
        {
            ViewBag.SectionName = "Reports";
            ViewBag.LoginUserName = lCredentials.EmpShortName;
            ViewBag.LoginBranch = lCredentials.BranchCode;
            ViewBag.LoginBranchName = lCredentials.BranchName;
            ViewBag.PdfOrientation = "landscape";
            ViewBag.pageSize = "a0";
            ViewBag.PdfOrientation = "portrait";
            //string fm = (lCredentials.FM).ToString();
            //string fy = lCredentials.FY.ToString();
            //ViewBag.fm = fy + "-" + fm + "-" + "01";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
        }
        #region Increment Report by Indraja
        public ActionResult IncrementReport()
        {
            setReportCommonViewBag();
            ViewBag.fm = lCredentials.FinancialMonthDate;
            ViewBag.ReportTitle = " Increment ";
            
            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";
            
            ViewBag.DataUrl = "/Payroll/PrAllReports/IncrementReportData?fromdate=^1&todate=^2";            
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3]";
            ViewBag.PdfColumnsWidths = "80,160,150,100";

            ViewBag.ReportColumns = "[{'title': 'Emp Code', 'data': 'Code',  'autoWidth': true }," +
                "{ 'title': 'Emp Name','data': 'EName', 'autoWidth': true }," +
                "{ 'title': 'Designation','data': 'Designation',  'autoWidth': true }," +
                "{ 'title': 'Increment Date','data': 'IncDate', 'autoWidth': true }]"; ;

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }
        public async Task<string> IncrementReportData(string fromdate, string todate)
        {
            IncrementReportBusiness IRBUS = new IncrementReportBusiness(LoginHelper.GetCurrentUserForPR());
           // var empDetails = await IRBUS.searchIncrementReport(fromdate, todate);
            return JsonConvert.SerializeObject(await IRBUS.searchIncrementReport(fromdate, todate));
        }
        #endregion

        #region IncrementDateChange Report by Rajyalakshmi
        public ActionResult IncrementDateChangeReport()
        {
            setReportCommonViewBag();
            ViewBag.fm = lCredentials.FinancialMonthDate;
            ViewBag.ReportTitle = " Increment Date Change";

            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";

            ViewBag.DataUrl = "/Payroll/PrAllReports/IncrementDateChangeReportData?fromdate=^1&todate=^2";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3, 4, 5, 6]";
            ViewBag.PdfColumnsWidths = "50,70,70,70,80,70,70";

            ViewBag.ReportColumns = "[{'title': 'Emp Code', 'data': 'emp_code',  'autoWidth': true }," +
                "{ 'title': 'Emp Name','data': 'shortname', 'autoWidth': true  }," +
                 "{ 'title': 'Designation','data': 'Desg', 'autoWidth': true }," +
                 "{ 'title': 'Basic Amount','data': 'basic_amount', 'autoWidth': true ,className: 'dt-body-right'}," +
                "{ 'title': 'Increment Date','data': 'increment_date',  'autoWidth': true }," +
                 "{ 'title': 'Increment Date Change','data': 'inc_date_change',  'autoWidth': true }," +
                "{ 'title': 'Increment Amount','data': 'increment_amount', 'autoWidth': true ,className: 'dt-body-right' }]"; ;

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }
        public async Task<string> IncrementDateChangeReportData(string fromdate, string todate)
        {
            IncrementDateChangeReportBusiness incdchngbus= new IncrementDateChangeReportBusiness(LoginHelper.GetCurrentUserForPR());
            // var empDetails = await IRBUS.searchIncrementReport(fromdate, todate);
            return JsonConvert.SerializeObject(await incdchngbus.searchIncrementReport(fromdate, todate));
        }
        #endregion
        #region Members Leaving Service by Sowjanya
        public ActionResult MembersLeavingService()
        {
            setReportCommonViewBag();

            ViewBag.ReportTitle = "FORM NO.5 (Members Leaving Service)";
            ViewBag.ReportFiltersTemplate = "T2-OneMonthPicker";
            ViewBag.OneMonthPickerLabel1 = "Month";

            ViewBag.DataUrl = "/Payroll/PrAllReports/MembersLeavingServiceData?inputMonth=^1";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3, 4, 5, 6]";
            ViewBag.PdfColumnsWidths = "60,80,80,80,80,50,60";
            ViewBag.ReportColumns = "[{'title': 'Emp Code', 'data': 'Emp_Code',  'autoWidth': true }," +
                "{ 'title': 'Emp Name','data': 'Name_of_Member', 'autoWidth': true }," +
                "{ 'title': 'Father / Husband','data': 'Father_or_Husband_Name',  'autoWidth': true }," +
                "{ 'title': 'Account.No', 'data': 'Account_No',  'autoWidth': true }," +
                "{ 'title': 'Leaving Date','data': 'Date_of_Leaving',  'autoWidth': true }," +
                "{ 'title': 'Reason','data': 'RelievingReason',  'autoWidth': true }," +
                "{ 'title': 'Remarks','data': 'Remarks', 'autoWidth': true }]"; ;

            return View("~/Areas/Payroll/Views/PrAllReports/MembersLeaving.cshtml");
        }
        [HttpGet]
        public async Task<string> MembersLeavingServiceData(string inputMonth)
        {
            string ipmn="01-01-01" ;
            if (inputMonth != "^1")
            {
                ipmn = inputMonth;
            }

            MembersLeavingServiceReport MLSR = new MembersLeavingServiceReport(LoginHelper.GetCurrentUserForPR());
            //var LeavingEmp = await MLSR.getLeavingEmployees(ipmn);
            return JsonConvert.SerializeObject(await MLSR.getLeavingEmployees(ipmn));
        }
        #endregion

        //#region LoanLedger Report by Indraja

        //[HttpGet]
        //public async Task<ActionResult> LoanLedger()
        //{
        //    setReportCommonViewBag();

        //    ViewBag.DataUrl = "/Payroll/PrAllReports/getLoanTypes";

        //    //*** get loan types ***
        //    LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
        //    var LoanTypesList = await lbus.getloantypes();

        //    //Financial Period Drop downlist
        //    ViewBag.DataUrl = "/Payroll/PrAllReports/getFy";
        //    var fYears = await lbus.getFy();
        //    ViewBag.T3DropdownList = fYears;
           

        //    ViewBag.ReportTitle = "Loan Ledger";
        //    ViewBag.ReportFiltersTemplate = "T3-AllEmpSearchOneMultiSelOneDropdown";
        //    ViewBag.T3MultiSelLabel = "Loan ";
        //    ViewBag.T3MultiSelList = LoanTypesList;

        //    ViewBag.DdlOneData = new SelectList(fYears,"Id", "fY");
        //    ViewBag.T3DdlOneLabel = "Financial Period:";
        //    ViewBag.textboxempcode = "Employees Code";

        //    ViewBag.DataUrl = "/Payroll/PrAllReports/LoanLedgerData?Loans=^2&empcode=^1&fYear=^3";
        //    ViewBag.ExportColumns = "columns: [0, 1, 3, 4, 5, 6, 7, 8, 9, 10,11]";
        //    ViewBag.PdfColumnsWidths = "35,45,42,40,40,42,44,40,40,44,40,41";
        //    ViewBag.ReportColumns = "[" +
        //        "{ 'title': 'Emp Code','data': 'emp_code', 'autoWidth': true , 'sortable': false}," +
        //        "{ 'title': 'Month','data': 'fm', 'autoWidth': true , 'sortable': false}," +
        //        "{ 'title': 'Interest Rate ','data': 'interest_rate',  'autoWidth': true , 'sortable': false}," +
        //        "{ 'title': 'Loan Opening','data': 'loan_opening', 'autoWidth': true , 'sortable': false}," +
        //        "{ 'title': 'Loan Repaid ','data': 'loan_repaid',  'autoWidth': true , 'sortable': false}," +
        //        "{ 'title': 'Loan Closing','data': 'loan_closing',  'autoWidth': true , 'sortable': false}," +
        //        "{ 'title': 'Interest Opening','data': 'interest_opening',  'autoWidth': true , 'sortable': false}," +
        //        "{ 'title': 'Interest Accrued','data': 'interest_accrued',  'autoWidth': true , 'sortable': false}," +
        //        "{ 'title': 'interest Repaid','data': 'interest_repaid',  'autoWidth': true , 'sortable': false}," +
        //        "{ 'title': 'interest Closing','data': 'interest_closing',  'autoWidth': true , 'sortable': false}," +
        //        "{ 'title': 'installment Repaid','data': 'installment_repaid',  'autoWidth': true , 'sortable': false}," +
        //    " {'title': 'Amount Issued', 'data': 'amount_issued',  'autoWidth': true , 'sortable': false}," +
        //   " {'title': 'Loan Type', 'data': 'loan_description',  'autoWidth': true , 'sortable': false}]"; ;

        //    return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        //}
        
        //public async Task<string> LoanLedgerData(string Loans,string empcode,string fYear)
        //{
        //    LoanledgerBusiness lbus = new LoanledgerBusiness(LoginHelper.GetCurrentUserForPR());
        //    return JsonConvert.SerializeObject(await lbus.GetLedgerReports(empcode, Loans, fYear));
        //}
       
        //#endregion

        #region Form 5A by Sowjanya
        public ActionResult From5AReport()
        {
        
            setReportCommonViewBag();

            ViewBag.ReportTitle = "Form 5A";

            ViewBag.ReportFiltersTemplate = "T2-OneMonthPicker";
            ViewBag.OneMonthPickerLabel1 = "Month";

            ViewBag.DataUrl = "/Payroll/PrAllReports/Form5AData?inputMonth=^1";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3]";
            ViewBag.PdfColumnsWidths = "150,150,153";
            ViewBag.ReportColumns = "[{'title': 'Sl.No', 'data': 'RowId', 'visible' : false , 'sortable' : false}," +
                "{'title': 'Employee Monthly Salary / Wages / both', 'data': 'EmpMonthSalary', 'sortable' : false }," +
                "{ 'title': 'Number of Employees','data': 'NumberofEmployees', 'sortable' : false}," +
                "{ 'title': 'Rate of Tax per Month','data': 'TaxperMonth' , 'sortable' : false,className: 'dt-body-right'}," +
                "{ 'title': 'Amount of Tax Deducted','data': 'TaxDeducted', 'sortable' : false,className: 'dt-body-right'}]"; 

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
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




        #region Allowance Report by Indhraja  / Sowjanya
        public async Task<ActionResult> AllowanceReport()
        {
            setReportCommonViewBag();

            ViewBag.DataUrl = "/Payroll/PrAllReports/AllowanceTypes";

            var AllowanceType = await arbus.AllowanceTypes();

            ViewBag.ReportFiltersTemplate = "T4-AllEmpSearchOneMultiSelOneMonthpic";

            ViewBag.T3MultiSelLabel = "Type";
            ViewBag.T3MultiSelList = AllowanceType;

            ViewBag.DataUrl = "/Payroll/PrAllReports/AllowanceReportData?emp_code=^1&Types=^2&mnth=^3";

            ViewBag.ReportTitle = "Allowance ReportData";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3, 4, 5]";
            ViewBag.PdfColumnsWidths = "60,100,100,80,100,50";
            ViewBag.ReportColumns = "[{ 'title': 'Employee Code','data': 'EmpId', 'autoWidth': true }," +
                "{ 'title': 'Employee Name','data': 'EmployeeName', 'autoWidth': true }," +
           " {'title': 'Allowance Type', 'data': 'AllowanceType',  'autoWidth': true }," +
           "{ 'title': 'From Date ','data': 'from_date',  'autoWidth': true }," +
           "{ 'title': 'To Date','data': 'to_date', 'autoWidth': true }, " +
           "{ 'title': 'Amount($)','data': 'amount', 'autoWidth': true }] "; ;

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }
        public async Task<string> AllowanceReportData(string emp_code, string Types, string mnth)
        {

            //return await arbus.GetAllowencedata(empicode, Loans);
            var AllowanceDetails = await arbus.GetAllowencedata(emp_code, Types, mnth);
            return JsonConvert.SerializeObject(AllowanceDetails);

        }

        #endregion

        #region LIC/HFC Report by Sowjanya

        [HttpGet]
        public async Task<ActionResult> LICReport()
        {
            setReportCommonViewBag();

            ViewBag.DataUrl = "/Payroll/PrAllReports/getTypes";

            //*** get Types(LIC/HFC) *** Total
            LICReportBusiness LICB = new LICReportBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await LICB.getTypes();

            ViewBag.ReportTitle = "LIC/HFC";

            ViewBag.ReportFiltersTemplate = "T4-AllEmpSearchOneMultiSelOneMonthpic";

            ViewBag.T3MultiSelLabel = "Deduction Type";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Emp  Code";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.DataUrl = "/Payroll/PrAllReports/LICReportData?emp_code=^1&Types=^2&mnth=^3";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3, 4]";
            ViewBag.PdfColumnsWidths = "80,130,90,90,100";
            ViewBag.ReportColumns = "[{ 'title': 'Type','data': 'Type', 'autoWidth': true }," +
                "{ 'title': 'Emp Code','data': 'Code', 'autoWidth': true }," +
           " {'title': 'Emp Name', 'data': 'Name',  'autoWidth': true }," +
           "{ 'title': 'Account No ','data': 'Account_No',  'autoWidth': true }," +
           "{ 'title': 'Amount ','data': 'Amount',  'autoWidth': true,className: 'dt-body-right' }," +
           "{ 'title': 'Total Amount','data': 'Total', 'autoWidth': true ,className: 'dt-body-right'},]";

            ViewBag.ReportColumnsCount = 6;
            //ViewBag.ReportColumns = "[{ 'title': 'Type','data': 'Alldt', 'autoWidth': true }," +
            //   "{ 'title': 'Tot','data': 'Tot', 'autoWidth': true },]";

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }

        public async Task<string> LICReportData(string emp_code, string Types, string mnth)
        {
            LICReportBusiness LICB = new LICReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await LICB.GetLICReports(emp_code, Types, mnth);
            return JsonConvert.SerializeObject(report);

        }

        #endregion

        #region sal branchwise by Lalitha

        //Salary bill of the branch for month

        [HttpGet]
        public async Task<ActionResult> SalBrReport()
        {

         setReportCommonViewBag();
        //    //*** get Branches list
         SalBrReportBusiness SALBR= new SalBrReportBusiness(LoginHelper.GetCurrentUserForPR());
   var FormatTypes = await SALBR.getBranches();


           ViewBag.T3MultiSelLabel = "Branch";
           ViewBag.T3MultiSelList = FormatTypes;
          ViewBag.ReportFiltersTemplate = "T7-OneMonthPickeronedropdown";
            ViewBag.OneMonthPickerLabel1 = "Month and Year";

          ViewBag.ReportTitle = "Salary Bill of Branch";

            ViewBag.DataUrl = "/Payroll/PrAllReports/SalBrReportData?Branch=^1&inputMonth=^2";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3, 4]";
            ViewBag.ReportColumns = "[{ 'title': 'Employee Code','data': 'Code', 'autoWidth': true }," +
           " {'title': 'Employee Name', 'data': 'name',  'autoWidth': true }," +
              " {'title': 'Branch/Dept', 'data': 'branch',  'autoWidth': true }," +
           "{ 'title': 'Designation','data': 'desig',  'autoWidth': true }," +
           "{ 'title': 'Gross Amount','data': 'grossamt',  'autoWidth': true }," +
            "{ 'title': 'Deductions','data': 'deductions',  'autoWidth': true }," +
           "{ 'title': 'Net Amount','data': 'netamt', 'autoWidth': true },]";

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }

        public async Task<string> SalBrReportData(string Branch, string inputMonth)
        {

            SalBrReportBusiness SALB = new SalBrReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await SALB.GetSalBrReport(Branch, inputMonth);
            return JsonConvert.SerializeObject(report);

        }
        #endregion

        #region All Allowance Report by Indhraja

    [HttpGet]
        public async Task<ActionResult> AllAllowance()
        {
            setReportCommonViewBag();

            ViewBag.DataUrl = "/Payroll/PrAllReports/getTypes";

            //*** get Types(LIC/HFC) *** Total
            AllAllowanceBusiness AllABus = new AllAllowanceBusiness(LoginHelper.GetCurrentUserForPR());
            var FormatTypes = await AllABus.getAllowanceTypes();

            // ViewBag.ReportTitle = "LIC / HFC";
            ViewBag.ReportTitle = "All Allowance";
            ViewBag.ReportFiltersTemplate = "T4-AllEmpSearchOneMultiSelOneMonthpic";

            ViewBag.T3MultiSelLabel = "Type";
            ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Emp  Code";
            ViewBag.OneMonthPickerLabel1 = "Month and Year";
            ViewBag.DataUrl = "/Payroll/PrAllReports/AllAllowanceData?emp_code=^1&types=^2&mnth=^3";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3]";

            ViewBag.PdfColumnsWidths = "170,160,160";
            ViewBag.ReportColumns = "[{'title': 'Employee Information','data': 'grpcol','autoWidth': true }," +
              " { 'title': 'Allowance','data': 'Allowance', 'autoWidth': true }," +
              " {'title': 'Pay Benfit', 'data': 'benefit',  'autoWidth': true }," +
              " { 'title': 'Amount ','data': 'amount',  'autoWidth': true }," +
              //" { 'title': 'Period','data': 'period', 'autoWidth': true }," +
              //" { 'title': 'Inst Paid ','data': 'INSTPAID',  'autoWidth': true }," +
              //" { 'title': 'Balance Inst','data': 'BALANCEINST',  'autoWidth': true }" +
              "]";

            return View("~/Areas/Payroll/Views/PrAllReports/AllAllowanceGrouping.cshtml");
        }

        public async Task<string> AllAllowanceData(string emp_code, string types, string mnth)
        {
            AllAllowanceBusiness AllABus = new AllAllowanceBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await AllABus.AllAllowanceData(emp_code, types, mnth);
            return JsonConvert.SerializeObject(report);

        }


        #endregion
     
        #region PF Repayable report grouping by Lalitha
        public ActionResult PFRepayableGroupingReport()
        {
            setReportCommonViewBag();


            ViewBag.ReportFiltersTemplate = "T8-AllAndSingleEmpSearchRbtn";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.twoDtpickersLabel2 = "To";
            ViewBag.ReportTitle = "PFRepayable";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3,4,5,6]";
            ViewBag.DataUrl = "/Payroll/PrAllReports/PFRepayableGroupingData?empcode=^1&fromDate=^2&toDate=^3";
            ViewBag.ReportColumns = "[{'title': 'Loan Information','data': 'grpcol','autoWidth': true }," +
              " { 'title': 'Code','data': 'EmpCode', 'autoWidth': true }," +
              " {'title': 'EmpName', 'data': 'name',  'autoWidth': true }," +
              " { 'title': 'Branch/Dept','data': 'branch',  'autoWidth': true }," +
              " { 'title': 'Designation','data': 'Desig',  'autoWidth': true }," +
              " { 'title': 'Sanction Date','data': 'sdate', 'autoWidth': true }," +
              
              " { 'title': 'Sanction amount','data': 'samt',  'autoWidth': true }]";
            return View("~/Areas/Payroll/Views/PrAllReports/PFRepayableLoanGroupings.cshtml");
        }
        public string PFRepayableGroupingData(string empcode,string fromdate,string todate)
        {
            return JsonConvert.SerializeObject( PFRbus.GetPFrepayableLoanGroupingReports(empcode,fromdate,todate));
        }
        #endregion

        #region TSheetReport By Raji
        public ActionResult TSheetReport()
        {
            setReportCommonViewBag1();
            @ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.ReportTitle = "Tsheet";
            ViewBag.ReportFiltersTemplate = "T8-OneMonthPickerTwoRadioButtons";
            ViewBag.DataUrl = "/Payroll/PrAllReports/TSheetReportData?inputMonth=^1&RegEmp=^2&SupEmp=^3";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17]";
            ViewBag.PdfColumnsWidths = "20,30,45,45,30,30,30,30,30,30,30,30,20,30,30,35,35,45";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId',  'autoWidth': true, 'visible': false }," +
                     "{'title': 'Branch/Department','data': 'column1', 'autoWidth': true }," +
                     "{'title': 'Employee Code','data': 'column19', 'autoWidth': true }," +
                     "{'title': 'Employee Name','data': 'column20', 'autoWidth': true }," +
                     "{'title': 'Basic', 'data': 'column3',  'autoWidth': true }," +
                     "{ 'title': 'DA','data': 'column4',  'autoWidth': true }," +
                     "{ 'title': 'CCA','data': 'column5', 'autoWidth': true }, " +
                     "{ 'title': 'HRA','data': 'column6', 'autoWidth': true }," +
                     "{ 'title': 'Interm Relief','data': 'column7', 'autoWidth': true }," +
                     "{ 'title': 'Spl.DA','data': 'column10', 'autoWidth': true }," +
                     "{ 'title': 'Spl.Allow','data': 'column9', 'autoWidth': true }," +
                     "{ 'title': 'Telangana Increment','data': 'column8', 'autoWidth': true }," +
                      "{'title': 'Allowance Data','data': 'grpclmn','autoWidth': true }," +
                      "{ 'title': 'Club Subscription','data': 'column14', 'autoWidth': true }," +
                      "{ 'title': 'Telangana Officers Assn','data': 'column15', 'autoWidth': true }," +
                      "{ 'title': 'Income Tax','data': 'column12', 'autoWidth': true }," +
                      "{ 'title': 'Provident Fund','data': 'column11', 'autoWidth': true }," +
                      "{ 'title': 'Professional Tax','data': 'column13', 'autoWidth': true }," +
                      "{'title':  'Deduction Data','data': 'column2','autoWidth': true }," +
                      "{ 'title': 'Gross Amount','data': 'column16', 'autoWidth': true }," +
                      "{ 'title': 'Deduction Amount','data': 'column18', 'autoWidth': true }," +
                      "{ 'title': 'Net Amount','data': 'column17', 'autoWidth': true }] "; 

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }

        public async Task<string> TSheetReportData(string inputMonth,string RegEmp,string SupEmp)
        {
            string ipmn = "01-01-01";
            if (inputMonth != "^1")
            {
                ipmn = inputMonth;
            }

            return JsonConvert.SerializeObject(await tbus.GetTsheetdata(ipmn,RegEmp,SupEmp));
        }

        #endregion

        #region MonthDetails Report By Raji
        public async Task<ActionResult> MonthDetailsReport()
        {

            setReportCommonViewBag();
            ViewBag.ReportTitle = "Month Details";
            ViewBag.ReportFiltersTemplate = "FinacialPeriod";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.DataUrl = "/Payroll/PrAllReports/MonthDetailsReportData?fYear=^1";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3, 4]";
            ViewBag.PdfColumnsWidths = "80,80,80,70";
            MonthDetailsReportBusiness MnthDet = new MonthDetailsReportBusiness(LoginHelper.GetCurrentUserForPR());

            var fYears = await MnthDet.getFyforMonthdetailsReport();
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");

            //ViewBag.ReportColumns = "[{'title': 'Financial Year', 'data': 'fy', 'sortable' : false }," +
            //    "{ 'title': 'Financial Month','data': 'fm', 'sortable' : false}," +
            //    "{ 'title': 'Da Slabs','data': 'da_slabs' , 'sortable' : false}," +
            //    "{ 'title': 'Da Points','data': 'da_points', 'sortable' : false}," +
            //    "{ 'title': 'Da Percent','data': 'da_percent', 'sortable' : false}," +
            //    "{ 'title': 'Interest Calculated','data': 'is_interest_calculated', 'sortable' : false}," +
            //     "{ 'title': 'Interest Percentage','data': 'interest_percent', 'sortable' : false}]";

            return View("~/Areas/Payroll/Views/PrAllReports/MonthDetailsReportView.cshtml");
        
        }
        [HttpGet]
        public async Task<string> MonthDetailsReportData(string fYear)
        {
       
            MonthDetailsReportBusiness MnthDet = new MonthDetailsReportBusiness(LoginHelper.GetCurrentUserForPR());

            return JsonConvert.SerializeObject(await MnthDet.MonthDetailsReportData(fYear));
        }


        #endregion

        #region PaySlipPdfGeneration
        public ActionResult PayslipPdfReport()
        {
            setReportCommonViewBag();


            ViewBag.ReportFiltersTemplate = "T6-AllAndSingleEmpPdfGenbtn";
            ViewBag.buttons = "b1-Pdfreportbutton";
            ViewBag.OneMonthPickerLabel1 = "Month";
            ViewBag.textboxempcode = "Employees Code";
            ViewBag.ReportTitle = "Payslip";
            ViewBag.PdfNoOfCols = 13;
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]";
            ViewBag.PdfColumnsWidths = "30,50,45,50,50,45,50,50,50,50,40,40,30";
            ViewBag.DataUrl = "/Payroll/PrAllReports/PayslipPdfReportReportData?empcode=^1&pdftypes=^2&mnpkrVal1=^3";
            ViewBag.ReportColumns = "[{ 'title': 'Emp Id','data': 'column1'}," +
                "{'title': 'Employee Name','data': 'column2'}," +
                " { 'title': 'Designation','data': 'column3'}," +
                " {'title': 'Total Gross', 'data': 'column4'}," +
                " { 'title': 'Total Deduction ','data': 'column5'}," +
                " { 'title': 'Net Salary','data': 'column6'}," +
                " { 'title': 'Payslip Type','data': 'column7' }]";

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }

        //public async Task<string> PayslipPdfReportReportData(string empcode, string pdftypes,string mnpkrVal1)
        //{
        //    return JsonConvert.SerializeObject(await prpdffbud.GetPayslipReportdata(empcode, pdftypes, mnpkrVal1));
        //}
        #endregion

        #region PaySlipPDFGenerationbyTirumalarao
        public ActionResult PayslipReport()
        {
            //string fm = (lCredentials.FM).ToString();
            //string fy = (lCredentials.FY).ToString();
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            ViewBag.SectionName = "Reports";
            ViewBag.OneMonthPickerLabel1 = "Month";
            return View();
        }

        public async Task<string> PayslipPdfReportReportData(string empcode, string pdftypes, string mnpkrVal1)
        {
           return JsonConvert.SerializeObject(await prpdffbud.GetPayslipReportdata(empcode, pdftypes, mnpkrVal1));
        }
        #endregion
        #region PaySlipPDFGenerationby uma
        public async Task<ActionResult> PayslipReporthrms()
        {
            Form16Business Form16Bus = new Form16Business(LoginHelper.GetCurrentUserForPR());
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            //string fm = (lCredentials.FM).ToString();
            //string fy = (lCredentials.FY).ToString();
                       
            
            int empid = lCredentials.EmpCode;
            string empid1 = Convert.ToString(empid);
            DateTime logintime =DateTime.Now;
            // like system or phone
            string source = "";
            // like page (which page)
            string page = "";

            var store_logindata = await Form16Bus.store_logindata(empid,source,logintime,page);

            ViewBag.DataUrl = "/Payroll/PrAllReports/getFy";
           
            var fYears = await Form16Bus.getFy();
            ViewBag.T3DropdownList = fYears;

            ViewBag.ReportTitle = "Form 16";
            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            return View("~/Areas/Payroll/Views/PrAllReports/PayslipReporthrms.cshtml");
        }
                
        public async Task<string> PayslipPdfReportReportDatahrms(string empcode, string pdftypes, string mnpkrVal1)
        {
            return JsonConvert.SerializeObject(await prpdffbud.GetPayslipReportdatahrms(empcode, pdftypes, mnpkrVal1));
        }
        #endregion
        #region TDS Report by Indraja

        [HttpGet]
        public async Task<ActionResult> TDSReport()
        {
            setReportCommonViewBag();

            //*** get loan types ***

            //Financial Period Drop downlist

            ViewBag.ReportTitle = "TDS Report";
            ViewBag.ReportFiltersTemplate = "T1-SearchEmpcode";

            ViewBag.DataUrl = "/Payroll/PrAllReports/TDSReportData?empcode=^1";

            ViewBag.PdfNoOfCols = 3;
            ViewBag.ExportColumns = "columns: [  0,1,2,3]";
            ViewBag.PdfColumnsWidths = "50,55,65";
            ViewBag.PdfGrpColHeaderLabels = " , , ,";

            ViewBag.ReportColumns = "[" +

                "{ 'title': '','data': 'interest_repaid', 'sortable': false}," +
                "{ 'title': '','data': 'interest_closing', 'sortable': false}," +
                "{ 'title': '','data': 'total_paid', 'sortable': false}," +
           " {'title': '', 'data': 'loan_description', 'sortable': false}]"; ;

            return View("~/Areas/Payroll/Views/PrAllReports/TDSReport.cshtml");
        }

        public async Task<string> TDSReportData(string empcode)
        {
            TDSReportBusiness TDSBus = new TDSReportBusiness(LoginHelper.GetCurrentUserForPR());
            return JsonConvert.SerializeObject(await TDSBus.getTdsDetails(empcode));
        }
        #endregion
        #region TDS Report by Lalitha 20/08/2020

        [HttpGet]
        public async Task<ActionResult> TDSReporthrms()
        {
            setReportCommonViewBag();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            //*** get loan types ***

            //Financial Period Drop downlist

            ViewBag.ReportTitle = "TDS Report";
            ViewBag.ReportFiltersTemplate = "T1-SearchEmpcode";

            ViewBag.DataUrl = "/Payroll/PrAllReports/TDSReportDatahrms";

            ViewBag.PdfNoOfCols = 3;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4]";
            ViewBag.PdfColumnsWidths = "50,150,80,50";
            ViewBag.ReportColumns = "[" +
                "{'title': 'RowID', 'data': 'RowId', 'visible': false }," +
                "{ 'title': ' ','data': 'column1' ,'sortable': false}," +
                "{ 'title': ' ','data':'column2' ,'sortable': false}," +
                "{ 'title': ' ','data': 'column3' ,'sortable': false}," +
                "{ 'title': ' ','data': 'column4' ,'sortable': false}]";
            ViewBag.ReportColumnsCount = 4;

            return View("~/Areas/Payroll/Views/PrAllReports/TDSReporthrms.cshtml");
        }

        public async Task<string> TDSReportDatahrms()
        {
           
            string empcode = "";
            setReportCommonViewBag();
            if (empcode.Contains("^"))
            { empcode = empcode; }
            else
            {
                lCredentials = LoginHelper.GetCurrentUserForPR();
                
                empcode = Convert.ToString(lCredentials.EmpCode);
            }
            TDSReportBusiness TDSBus = new TDSReportBusiness(LoginHelper.GetCurrentUserForPR());
            return JsonConvert.SerializeObject(await TDSBus.getTdsDetailshrms(empcode));
        }
        #endregion
       

        #region Promotion Report by Indraja
        public ActionResult PromotionReport()
        {
            setReportCommonViewBag();
            ViewBag.fm = lCredentials.FinancialMonthDate;
            ViewBag.ReportTitle = " Employee Promotions ";

            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";
            ViewBag.twoDtpickersLabel1 = "Effective From Date";
            ViewBag.twoDtpickersLabel2 = "Effective To Date";

            ViewBag.DataUrl = "/Payroll/PrAllReports/PromotionReportData?fromdate=^1&todate=^2";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3, 4, 5, 6, 7]";
            ViewBag.PdfColumnsWidths = "40,70,80,80,50,50,60,60";

            ViewBag.ReportColumns = "[{'title': 'Emp Code', 'data': 'emp_code',  'autoWidth': true }," +
           "{ 'title': 'Emp Name','data': 'ShortName', 'autoWidth': true }," +
           "{ 'title': 'Old Designation','data': 'old_desig',  'autoWidth': true }," + // OLD
           "{ 'title': 'New Designation','data': 'new_Desig',  'autoWidth': true }," + // NEW
           "{ 'title': 'Old Basic','data': 'amount', 'autoWidth': true }," +
           "{ 'title': 'New Basic','data': 'basic_pay_fixed', 'autoWidth': true }," +
           "{ 'title': 'Promotion Date','data': 'promotion_date', 'autoWidth': true }," +
           "{ 'title': 'Effective Date','data': 'Effective_date', 'autoWidth': true }" +
           "]"; 

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }
        public async Task<string> PromotionReportData(string fromdate, string todate)
        {
            PromotionReportBusiness PBUS = new PromotionReportBusiness(LoginHelper.GetCurrentUserForPR());
            // var empDetails = await IRBUS.searchIncrementReport(fromdate, todate);
            return JsonConvert.SerializeObject(await PBUS.PromotionReportData(fromdate, todate));
        }
        #endregion
        #region poc report for old and new table collaboration by Chandu
        public ActionResult OldTableReportPOC()
        {

            setReportCommonViewBag();

            ViewBag.ReportTitle = "Old Table Collaboration POC";

            ViewBag.ReportFiltersTemplate = "T3-OneMonthPicker";
            ViewBag.OneMonthPickerLabel1 = "Month";

            ViewBag.DataUrl = "/Payroll/PrAllReports/OldTableReportPOCS?inputMonth=^1";
            ViewBag.ExportColumns = "columns: [ 1, 2, 3]";
            ViewBag.PdfColumnsWidths = "150,150,153";
            ViewBag.ReportColumns = "[{'title': 'Sl.No', 'data': 'RowId', 'visible' : false , 'sortable' : false}," +
                "{'title': 'Employee Monthly Salary / Wages / both', 'data': 'EmpMonthSalary', 'sortable' : false }," +
                "{ 'title': 'Number of Employees','data': 'NumberofEmployees', 'sortable' : false}," +
                "{ 'title': 'Rate of Tax per Month','data': 'TaxperMonth' , 'sortable' : false}," +
                "{ 'title': 'Amount of Tax Deducted','data': 'TaxDeducted', 'sortable' : false}]";

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }
        [HttpGet]
        public async Task<string> OldTableReportPOCS(string inputMonth)
        {
            string ipmn = "01-01-01";
            if (inputMonth != "^1")
            {
                ipmn = inputMonth;
            }

            Form5AReport FRM5A = new Form5AReport(LoginHelper.GetCurrentUserForPR());

            return JsonConvert.SerializeObject(await FRM5A.getForm5ADataForPOC(ipmn));
        }
        #endregion

        #region poc report for old and new table collaboration with from and to date by Chandu 
        //OldTableReportPOCForFromDateToDate
        //OldTableReportPOCSFromAndTwo
        public ActionResult OldTableReportPOCForFromDateToDate()
        {
            setReportCommonViewBag();
            ViewBag.fm = lCredentials.FinancialMonthDate;
            ViewBag.ReportTitle = " Increment ";

            ViewBag.ReportFiltersTemplate = "T1-twoDtpickers";
            ViewBag.twoDtpickersLabel1 = "From Date";
            ViewBag.twoDtpickersLabel2 = "To Date";

            ViewBag.DataUrl = "/Payroll/PrAllReports/OldTableReportPOCSFromAndTo?fromdate=^1&todate=^2";
            ViewBag.ExportColumns = "columns: [0, 1, 2, 3]";
            ViewBag.PdfColumnsWidths = "80,160,150,100";

            ViewBag.ReportColumns = "[{'title': 'Emp Code', 'data': 'Code',  'autoWidth': true }," +
                "{ 'title': 'Emp Name','data': 'EName', 'autoWidth': true }," +
                "{ 'title': 'Designation','data': 'Designation',  'autoWidth': true }," +
                "{ 'title': 'Increment Date','data': 'IncDate', 'autoWidth': true }]"; ;

            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }
        public async Task<string> OldTableReportPOCSFromAndTo(string fromdate, string todate)
        {
            IncrementReportBusiness IRBUS = new IncrementReportBusiness(LoginHelper.GetCurrentUserForPR());
            // var empDetails = await IRBUS.searchIncrementReport(fromdate, todate);
            //return JsonConvert.SerializeObject(await IRBUS.searchIncrementReportForPOC(fromdate, todate));
            return JsonConvert.SerializeObject(await IRBUS.searchIncrementReportForPOCForJoin(fromdate, todate));
            
        }
        #endregion

        #region JAIIB_CAIIB Report by Sowjanya
        [HttpGet]
        public async Task<ActionResult> JAIIB_CAIIB_ReportDashboard()
        {
            setReportCommonViewBag();

            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllReports/getFy";
            // ViewBag.DataUrl1 = "/Payroll/PrAllReports/getIncType";
            var fYears = await JACA.getFy();
            ViewBag.T3DropdownList = fYears;
            ViewBag.T3MultiSelList = fYears;
            ViewBag.T3MultiSelLabel = "Increment / Incentive Type";
            ViewBag.ReportTitle = "JAIIB / CAIIB";
            ViewBag.ReportFiltersTemplate = "T2-AllEmpSearchTwoDropdown";


            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.textboxempcode = "Employees Code";

            ViewBag.DataUrl = "/Payroll/PrAllReports/JAIIB_CAIIB_Data?empcode=dash&fYear=dyear&incType=dtype";

            ViewBag.PdfNoOfCols = 9;
            ViewBag.ExportColumns = "columns:[0,1,2,3,4,5,6,7,8,9]";
            ViewBag.PdfColumnsWidths = "30,50,50,50,50,35,40,55,55,50";
            //ViewBag.PdfGrpColHeaderLabels = " , , , , , , , , , , , , ";

            ViewBag.ReportColumns = "[{ 'title': 'Emp Code','data': 'emp_code', 'autoWidth': true }," +
                " {'title': 'Emp Name', 'data': 'ShortName',  'autoWidth': true }," +
                " {'title': 'Designation', 'data': 'Description',  'autoWidth': true }," +
           " {'title': 'Basic Before Incr', 'data': 'basic_before_inc',  'autoWidth': true }," +
           "{ 'title': 'Incentive Type','data': 'incr_incen_type',  'autoWidth': true }," +
           "{ 'title': 'Incr Amt','data': 'Incrementamt',  'autoWidth': true }," +
             "{ 'title': 'Basic After Incr','data': 'Basic_After_Inc',  'autoWidth': true }," +
           "{ 'title': 'Incr Date','data': 'incrementdate',  'autoWidth': true }," +
           "{ 'title': 'W.A.F Date','data': 'incr_WEF_date',  'autoWidth': true }," +
           "{ 'title': ' Status','data': 'status', 'autoWidth': true }]";
            ViewBag.ReportColumnsCount = 10;
            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }
        [HttpGet]
        public async Task<ActionResult> JAIIB_CAIIB_Report()
        {
            setReportCommonViewBag();

            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllReports/getFy";
           // ViewBag.DataUrl1 = "/Payroll/PrAllReports/getIncType";
            var fYears = await JACA.getFy();
            ViewBag.T3DropdownList = fYears;
            ViewBag.T3MultiSelList = fYears;
            ViewBag.T3MultiSelLabel = "Increment / Incentive Type";
            ViewBag.ReportTitle = "JAIIB / CAIIB";
            ViewBag.ReportFiltersTemplate = "T2-AllEmpSearchTwoDropdown";


            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.textboxempcode = "Employees Code";

            ViewBag.DataUrl = "/Payroll/PrAllReports/JAIIB_CAIIB_Data?empcode=^1&fYear=^2&incType=^3";

            ViewBag.PdfNoOfCols = 9;
            ViewBag.ExportColumns = "columns:[0,1,2,3,4,5,6,7,8,9]";
            ViewBag.PdfColumnsWidths = "30,50,50,50,50,35,40,55,55,50";
            //ViewBag.PdfGrpColHeaderLabels = " , , , , , , , , , , , , ";

            ViewBag.ReportColumns = "[{ 'title': 'Emp Code','data': 'emp_code', 'autoWidth': true }," +
                " {'title': 'Emp Name', 'data': 'ShortName',  'autoWidth': true }," +
                " {'title': 'Designation', 'data': 'Description',  'autoWidth': true }," +
           " {'title': 'Basic Before Incr', 'data': 'basic_before_inc',  'autoWidth': true ,className: 'dt-body-right' }," +
           "{ 'title': 'Incentive Type','data': 'incr_incen_type',  'autoWidth': true }," +
           "{ 'title': 'Incr Amt','data': 'Incrementamt',  'autoWidth': true ,className: 'dt-body-right'}," +
             "{ 'title': 'Basic After Incr','data': 'Basic_After_Inc',  'autoWidth': true ,className: 'dt-body-right' }," +
           "{ 'title': 'Incr Date','data': 'incrementdate',  'autoWidth': true }," +
           "{ 'title': 'W.A.F Date','data': 'incr_WEF_date',  'autoWidth': true }," +
           "{ 'title': ' Status','data': 'status', 'autoWidth': true }]";
            ViewBag.ReportColumnsCount = 10;
            return View("~/Areas/Payroll/Views/PrAllReports/PrAllReportView.cshtml");
        }

        public async Task<string> JAIIB_CAIIB_Data(string empcode, string fYear, string incType)
        {
            JAIIB_CAIIB_Business JACA = new JAIIB_CAIIB_Business(LoginHelper.GetCurrentUserForPR());

           // var JAIIB_CAIIBDetails = await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType);
            return JsonConvert.SerializeObject(await JACA.GetJAIIB_CAIIB_Data(empcode, fYear, incType));
        }
        #endregion

        #region  Form 16 Repot by Sowjanya
        [HttpGet]
        public async Task<ActionResult> Form16Report()
        {
            ViewBag.SectionName = "Reports";
            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllReports/getFy";
            var fYears = await Form16Bus.getFy();
            ViewBag.T3DropdownList = fYears;

            ViewBag.ReportTitle = "Form 16";


            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.textboxempcode = "Employees Code";
            return View("~/Areas/Payroll/Views/PrAllReports/Form16ReportView.cshtml");
        }

        public async Task<string> Form16ReportData(string empcode, string mnpkrVal1)
        {
            Form16Business Form16Bus = new Form16Business(LoginHelper.GetCurrentUserForPR());
            return JsonConvert.SerializeObject(await Form16Bus.GetForm16empData(empcode, mnpkrVal1));
        }

        #endregion

        #region PaySlipPDF Negative NetSalary
        public ActionResult NetSalaryReport()
        {
            //string fm = (lCredentials.FM).ToString();
            //string fy = (lCredentials.FY).ToString();
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            ViewBag.SectionName = "Reports";
            ViewBag.OneMonthPickerLabel1 = "Month";
            return View("~/Areas/Payroll/Views/PrAllReports/PayslipSalaryNegative.cshtml");
        }

        public async Task<string> NetSalaryReportData(string empcode, string pdftypes, string mnpkrVal1)
        {
            return JsonConvert.SerializeObject(await netSal.GetNetSalaryReportdata(empcode, pdftypes, mnpkrVal1));
        }
        #endregion
        #region  Form 7 Report by Lalitha
        [HttpGet]
        public async Task<ActionResult> Form7Report()
        {
            ViewBag.SectionName = "Reports";
            //Financial Period Drop downlist
            ViewBag.DataUrl = "/Payroll/PrAllReports/getFy";
            var fYears = await Form16Bus.getFy();
            ViewBag.T3DropdownList = fYears;

            ViewBag.ReportTitle = "Form 7";


            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            ViewBag.T3DdlOneLabel = "Financial Period:";
            ViewBag.textboxempcode = "Employees Code";
            return View("~/Areas/Payroll/Views/PrAllReports/Form7ReportView.cshtml");
        }

        public async Task<string> Form7ReportData(string empcode, string mnpkrVal1)
        {
            Form7Business Form7Bus = new Form7Business(LoginHelper.GetCurrentUserForPR());
            return JsonConvert.SerializeObject(await Form7Bus.GetForm7empData(empcode, mnpkrVal1));
        }

        #endregion
    }
}

    