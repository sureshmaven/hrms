using HRMSApplication.Helpers;
using Newtonsoft.Json;
using PayRollBusiness.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class PocNewController : Controller
    {
        // GET: Payroll/PocNew
        public ActionResult Index()
        {

            ViewBag.ReportTitle = "LIC/HFC";

            ViewBag.ReportFiltersTemplate = "T4-AllEmpSearchOneMultiSelOneMonthpic";

            ViewBag.T3MultiSelLabel = "Deduction Type";
            //ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Emp  Code";
            ViewBag.OneMonthPickerLabel1 = "Month and Year";
            ViewBag.DataUrl = "/Payroll/PocNew/LICReportDataNew";
            ViewBag.ExportColumns = "columns: [1, 2, 3]";
            ViewBag.PdfColumnsWidths = "100,190,200";
            ViewBag.ReportColumns = "[{'title': 'RowID', 'data': 'RowId', 'visible': false }," +
                "{ 'title': 'SlNo','data': 'Col1' }," +
                "{ 'title': 'Account No','data': 'Col2' }," +
                "{ 'title': 'Amount','data': 'Col3' }]";
            ViewBag.ReportColumnsCount = 3;
            return View("~/Areas/Payroll/Views/PocNew/Index.cshtml");
        }

        private string ReportColHeader(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:"+ Mavensoft.Common.PrConstants.PDF_REPORT_HEADER_COLOUR +"'>";
            for(int i=1; i<=spaceCount; i++)
                sRet += "_";
            sRet += "</span>";

            sRet += "<span>"+ lable +": <b>"+ value +"</b></span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }
        private string ReportColFooter(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:"+ Mavensoft.Common.PrConstants.PDF_REPORT_FOOTER_COLOUR +"'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "_";
            sRet += "</span>";

            sRet += "<span>" + lable + ": " + value + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string LICReportDataNew()
        {
            List<CommonReportModel> lst = new List<CommonReportModel>();
            lst.Add(new CommonReportModel
            {
                RowId = 1,
                HRF = "H",
                Col1 = "<span style='color:#C8EAFB'>~</span>" 
                + ReportColHeader(0, "Branch", "Malakpeta") 
                + ReportColHeader(15, "Date", "22-10-209")
                + ReportColHeader(50, "Active", "Yes")
            });
            lst.Add(new CommonReportModel
            {
                RowId = 2,
                HRF = "R",
                Col1 = "S.No.",
                Col2 = "Account No",
                Col3 = "Amount"
            });
            lst.Add(new CommonReportModel
            {
                RowId = 3,
                HRF = "R",
                Col1 = "1",
                Col2 = "Acct 1",
                Col3 = "100"
            });
            lst.Add(new CommonReportModel
            {
                RowId = 4,
                HRF = "R",
                Col1 = "2",
                Col2 = "Acct 2",
                Col3 = "200"
            });
            lst.Add(new CommonReportModel
            {
                RowId = 5,
                HRF = "R",
                Col1 = "3",
                Col2 = "Acct 3",
                Col3 = "300"
            });
            lst.Add(new CommonReportModel
            {
                RowId = 6,
                HRF = "F",
                Col1 = "<span style='color:#eef8fd'>^</span>"
                    + ReportColFooter(0, "Total 1", "600")
                    + ReportColFooter(100, "Total 2", "777")
            });

            //2nd group
            lst.Add(new CommonReportModel
            {
                RowId = 7,
                HRF = "H",
                Col1 = "<span style='color:#C8EAFB'>~</span>"
                + ReportColHeader(0, "Branch", "Ameerpet")
                + ReportColHeader(15, "Date", "09-07-209")
                + ReportColHeader(50, "Active", "No")
            });
            lst.Add(new CommonReportModel
            {
                RowId = 8,
                HRF = "R",
                Col1 = "S.No.",
                Col2 = "Account No",
                Col3 = "Amount"
            });
            lst.Add(new CommonReportModel
            {
                RowId = 9,
                HRF = "R",
                Col1 = "1",
                Col2 = "Acct 11",
                Col3 = "250"
            });
            lst.Add(new CommonReportModel
            {
                RowId = 10,
                HRF = "F",
                Col1 = "<span style='color:"+ Mavensoft.Common.PrConstants.PDF_REPORT_FOOTER_COLOUR +"'>^</span>"
                    + ReportColFooter(0, "Total 3", "$250")
                    + ReportColFooter(100, "Total 4", "$250")
            });

            return JsonConvert.SerializeObject(lst);
        }

        public ActionResult Index2()
        {

            ViewBag.ReportTitle = "POC multi group multi footer 2";

            ViewBag.ReportFiltersTemplate = "T4-AllEmpSearchOneMultiSelOneMonthpic";

            ViewBag.T3MultiSelLabel = "Deduction Type";
            //ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Emp  Code";
            ViewBag.OneMonthPickerLabel1 = "Month and Year";
            ViewBag.DataUrl = "/Payroll/PocNew/MultiHeaderFooter";
            ViewBag.ExportColumns = "columns: [1, 2]";
            ViewBag.PdfColumnsWidths = "280,200";
            ViewBag.ReportColumns = "[" +
                "{'title': 'RowID', 'data': 'RowId', 'visible': false }," +
                "{ 'title': 'Emp Name','data': 'Col1' }," +
                "{ 'title': 'Salary','data': 'Col2' }" +
                "]";
            ViewBag.ReportColumnsCount = 2;

            return View("~/Areas/Payroll/Views/PocNew/Index.cshtml");
        }
        
        public string MultiHeaderFooter()
        {
            int rowid = 0;
            List<CommonReportModel> lst = new List<CommonReportModel>();
            //jan 2019
            lst.Add(new CommonReportModel
            {
                RowId = rowid++,
                HRF = "H",
                Col1 = "<span style='color:"+ Mavensoft.Common.PrConstants.PDF_REPORT_HEADER_COLOUR +"'>~</span>"
                + ReportColHeader(0, "Month", "Jan 2019")
            });
            lst.Add(new CommonReportModel
            {
                RowId = rowid++,
                HRF = "R",
                Col1 = "Name",
                Col2 = "Salary"
            });
            lst.Add(new CommonReportModel
            {
                RowId = rowid++,
                HRF = "R",
                Col1 = "Emp 1",
                Col2 = "10500"
            });
            lst.Add(new CommonReportModel
            {
                RowId = rowid++,
                HRF = "R",
                Col1 = "Emp 2",
                Col2 = "7500"
            });
            lst.Add(new CommonReportModel
            {
                RowId = rowid++,
                HRF = "F",
                Col1 = "<span style='color:" + Mavensoft.Common.PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>^</span>"
                    + ReportColFooter(0, "Total Salary", "18000")
                    + ReportColFooter(100, "Mean Salary", "9000")
            });

            //feb 2019
            lst.Add(new CommonReportModel
            {
                RowId = rowid++,
                HRF = "H",
                Col1 = "<span style='color:" + Mavensoft.Common.PrConstants.PDF_REPORT_HEADER_COLOUR + "'>~</span>"
                + ReportColHeader(0, "Month", "Feb 2019")
            });
            lst.Add(new CommonReportModel
            {
                RowId = rowid++,
                HRF = "R",
                Col1 = "Name",
                Col2 = "Salary"
            });
            lst.Add(new CommonReportModel
            {
                RowId = rowid++,
                HRF = "R",
                Col1 = "Emp 1",
                Col2 = "10000"
            });
            lst.Add(new CommonReportModel
            {
                RowId = rowid++,
                HRF = "R",
                Col1 = "Emp 2",
                Col2 = "7000"
            });
            lst.Add(new CommonReportModel
            {
                RowId = rowid++,
                HRF = "F",
                Col1 = "<span style='color:" + Mavensoft.Common.PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>^</span>"
                    + ReportColFooter(0, "Total Salary", "17000")
                    + ReportColFooter(100, "Mean Salary", "8500")
            });

            return JsonConvert.SerializeObject(lst);
        }
        #region All Loan Report by Uma as Footer

        [HttpGet]
        public async Task<ActionResult> AllLoanReport()
        {

            ViewBag.ReportTitle = "All Loan Report";

            ViewBag.ReportFiltersTemplate = "T10-OneMonthPickerTwoRadioButtons";
            ViewBag.T3MultiSelLabel = "Deduction Type";
            //ViewBag.T3MultiSelList = FormatTypes;
            ViewBag.textboxempcode = "Emp  Code";
            ViewBag.OneMonthPickerLabel1 = "Month and Year";
            ViewBag.DataUrl = "/Payroll/PocNew/AllLoanReportData?inputMonth=^1&RegEmp=^2&SupEmp=^3";

            ViewBag.ExportColumns = "columns: [1, 2, 3, 4,5,6,7]";
            ViewBag.PdfColumnsWidths = "50,150,80,50,50,50,50";
            ViewBag.ReportColumns = "[" +
                "{'title': 'RowID', 'data': 'RowId', 'visible': false }," +
                "{ 'title': 'C1','data': 'column1' }," +
                "{ 'title': 'C2','data':'column2' }," +
                "{ 'title': 'c3','data': 'column3' }," +
                "{ 'title': 'c4','data': 'column4' }," +
                "{ 'title': 'c5','data': 'column5' }," +
                "{ 'title': 'c6','data': 'column6' }," +
                "{ 'title': 'c7','data': 'column7'}]";

            ViewBag.ReportColumnsCount = 7;

            return View("~/Areas/Payroll/Views/PocNew/Index.cshtml");

        }

        public async Task<string> AllLoanReportData(string inputMonth, string RegEmp, string SupEmp)
        {


            AllLoansReportBusiness LICB = new AllLoansReportBusiness(LoginHelper.GetCurrentUserForPR());
            var report = await LICB.GetLoanReport(inputMonth, RegEmp, SupEmp);
            return JsonConvert.SerializeObject(report);

        }

        #endregion
    }

    public class CommonReportModel
    {
        public int RowId { get; set; }
        public string HRF { get; set; }
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public string Col6 { get; set; }
        public string Col7 { get; set; }
        public string Col8 { get; set; }
        public string Col9 { get; set; }
        public string Col10 { get; set; }
        public string Col11 { get; set; }
        public string Col12 { get; set; }
        public string Col13 { get; set; }
        public string Col14 { get; set; }
        public string Col15 { get; set; }
        public string Col16 { get; set; }
        public string Col17 { get; set; }
        public string Col18 { get; set; }
        public string Col19 { get; set; }
        public string Col20 { get; set; }
        public string Col21 { get; set; }
        public string grpclmn { get; set; }
        public string footer { get; set; }
    }
}