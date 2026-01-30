using System;
using System.Data;
using System.Web.Mvc;
using System.Threading.Tasks;
using HRMSApplication.Helpers;
using PayrollModels;
using PayRollBusiness.Transaction;
using HRMSBusiness.Business;
using PayrollModels.Transactions;
using Newtonsoft.Json;
using PayRollBusiness;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        CommonBusiness commBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());
        EmployeePayFieldsBus epfbus = new EmployeePayFieldsBus(LoginHelper.GetCurrentUserForPR());
        DeputationEntryBusiness debus = new DeputationEntryBusiness(LoginHelper.GetCurrentUserForPR());
        AdjestPaymentsBusiness apbus = new AdjestPaymentsBusiness(LoginHelper.GetCurrentUserForPR());
        PayrollBusiness PayrollBusiness = new PayrollBusiness();
        PayrollBusiness pbus = new PayrollBusiness();
        AdhocPaymentsBusiness adhocbus = new AdhocPaymentsBusiness(LoginHelper.GetCurrentUserForPR());
        AttendenceMonthlyBusiness AMbus = new AttendenceMonthlyBusiness(LoginHelper.GetCurrentUserForPR());
        AddMonthDetails abus = new AddMonthDetails(LoginHelper.GetCurrentUserForPR());
        AllowanceBusiness albus = new AllowanceBusiness(LoginHelper.GetCurrentUserForPR());
        AdjustmentsLoansBusiness adjLoanbus = new AdjustmentsLoansBusiness(LoginHelper.GetCurrentUserForPR());
        DateOfLeavingBusiness dol = new DateOfLeavingBusiness(LoginHelper.GetCurrentUserForPR());
        public ActionResult Index()
        {
            ViewBag.SectionName = "Transaction";
            return View();
        }
        public async Task<ActionResult> Dateofleaving()
        {
            ViewBag.SectionName = "Transaction";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            DataTable dtPay = await AMbus.getPayableDays();
            foreach (DataRow fm in dtPay.Rows)
            {
                ViewBag.fm = fm["fm"];
            }
            return View();
        }

        public async Task<string> UpdateRetirementDate(CommonPostDTO Values)
        {

            if (Values.Retirementdata != null)
            {
                return await dol.UpdateRetirementdate(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }
        #region  for Add Monthdetails
        public ActionResult MonthDetails()
        {
            ViewBag.SectionName = "Transaction";
            try
            {
                ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
                return View();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }


        public async Task<string> GetMonthdetailsData(string month)
        {

            var monthdata = await abus.getmonthdetals(month);
            return monthdata;
        }
        public async Task<string> Getselectedmonthdata(string selected)
        {
            var monthdata = await abus.getselectedmonthdata(selected);
            return monthdata;
        }
        [HttpPost]
        public async Task<string> AddPAyMonthDetailsPost(CommonPostDTO Values)
        {
            if (Values.Monthdata != null)
            {
                return await abus.InserMonthDetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }

        }
        #endregion


        #region Allowance
        public ActionResult EmpAllowance()
        {
            ViewBag.SectionName = "Transaction";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }


        public async Task<string> Insertallowance(CommonPostDTO values)
        {
            if (values.objdates != null)
            {
                return await albus.InsertAllowance(values);
            }
            if (values.EntityId != 0 && values.objdates == null)
            {
                return "E#Error#Please Enter/Modify Data";
            }
            else
            {
                return "E#Error#Please Enter Employee Code";
            }

        }

        public async Task<string> GetBranchEmpAllowanceData(string EmpCode)
        {
            var allowancedata = await albus.GetEmpBranchAllowances(EmpCode);
            return allowancedata;
        }
        #endregion


        #region Employee pay fields
        //Index view for employee pay fields screen. 
        public ActionResult EmployeePayFields()
        {
            ViewBag.SectionName = "Transaction";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        //Searching Employee pay fields details based on Id
        [HttpGet]
        public async Task<string> GetEmpPayFieldsDetails(int EmpId)
        {
            return await epfbus.GetEmpPayDetails(EmpId);
        }

        [HttpPost]
        public async Task<string> saveDataEPF(CommonPostDTO Values)
        {
            if (Values.StringData != null || Values.StringData2 != null)
            {
                return await epfbus.AddPayDetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }
        #endregion


        #region Operations for Adject Payments(Earning, Deduction, Contribution)  
        // Adject Payments view page
        public ActionResult AdjPaymetnsView()
        {
            ViewBag.SectionName = "Transaction";
            return View();
        }

        //Serach operation for perticuler employee
        // for updating amount values, getting empid from UI and retriving data from database and desplaying in Index view.
        public async Task<string> GetAdjestPayFields(int EmpId)
        {
            return await apbus.GetAdjestPayDetails(EmpId);
        }

        //getting amount values from UI and sending to business layer and insertin amount values into database
        public async Task<string> SaveDataAdjestPayments(CommonPostDTO Values)
        {
            if (Values.StringData != null || Values.StringData2 != null || Values.StringData3 != null)
            {
                return await apbus.SaveAdjestPayDetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }
        #endregion


        #region  Attendence Monthly 

        [HttpGet]
        public async Task<ActionResult> AttendenceMonthly()
        {
            ViewBag.SectionName = "Transaction";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            ViewBag.Fm_AdjustLoanDatePicker = commBus.Fm_AdjustLoanDatePicker();
            try
            {
                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                DataTable dtPayroll1 = await AMbus.getPayableDays();                
                var getmonthdays = await AMbus.monthdays();
                var Days = getmonthdays[0].month_days;
                ViewBag.monthdays = Days;
                if (dtPayroll1.Rows.Count > 0)
                {
                    foreach (DataRow drPayroll1 in dtPayroll1.Rows)
                    {
                        ViewBag.fm = drPayroll1["fm"];
                        ViewBag.FY = drPayroll1["fy"];
                        ViewBag.Paidhol = drPayroll1["paid_holidays"];
                        ViewBag.Weekhol = drPayroll1["week_holidays"];

                    }
                }

                ViewBag.TotalPayable = Days;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }
        // search AttendenceMonthly
        [HttpGet]
        public async Task<string> SearchEmpDetails(string empcode)
        {
            var Details = await AMbus.SearchEmpDetails(empcode);
            return Details;
        }
        //InserMonthaAttendence
        [HttpPost]
        public async Task<string> UpdateAttendance(PayrollModels.Transactions.UpdateDetails Values)
        {
            try
            {
                string EmpCode = Values.EmpId;
                string Result = await AMbus.InsertupdateAttendence(Values);
                //TempData["AlertMessage"] = message;
                return Result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        #endregion


        #region DeputationEntry

        public ActionResult DeputationEntryFields()
        {
            ViewBag.SectionName = "Transaction";
            ViewBag.Fm_AdjustLoanDatePicker = commBus.Fm_AdjustLoanDatePicker();
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        public async Task<string> GetCurrentMonth()
        {
            return await debus.LoadCurrentMonth();
        }

        [HttpGet]
        public async Task<string> GetDeputationEntryFieldsDetails(int EmpId)
        {
            return await debus.GetDeputationEntryDetails(EmpId);
        }

        [HttpPost]
        public async Task<string> SaveDeputationEntryValues(CommonPostDTO Values)
        {
            if (Values.StringData != null || Values.StringData2 != null || Values.StringData3 != null)
            {
                return await debus.AddDeputationEntryDetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }
        #endregion


        #region Adhoc Payments 
        //Adhoc Paymetns view
        public ActionResult AdhocPaymetnsView()
        {
            ViewBag.SectionName = "Transaction";
            ViewBag.Fm_AdjustLoanDatePicker = commBus.Fm_AdjustLoanDatePicker();
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }


        //Serach operation for perticuler employee
        // for updating amount values, getting empid from UI and retriving data from database and desplaying in Index view.
        public async Task<string> GetAdhocPayFields(int EmpId)
        {
            return await adhocbus.GetAdhocPayDetails(EmpId);
        }


        //Serach operation for perticuler employee on perticuler date
        public async Task<string> GetAdhocPayFieldsOnDateSearch(int EmpId, string date)
        {
            return await adhocbus.GetAdhocPayFieldsOnDateSearch(EmpId, date);
        }



        //getting amount values from UI and sending to business layer and insertin amount values into database
        public async Task<string> SaveDataAdhocPayments(CommonPostDTO Values)
        {
            if (Values.StringData != null || Values.StringData2 != null || Values.StringData3 != null || Values.StringData4 != ",,")
            {
                return await adhocbus.SaveAdhocPayDetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }

        #endregion


        #region Advances_Loans(adjust loans)
        //Index view for  adjustments Loans screen. 
        public async Task<ActionResult> AdjustmentsLoans()
        {
            ViewBag.SectionName = "Transaction";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            ViewBag.Fm_AdjustLoanDatePicker = commBus.Fm_AdjustLoanDatePicker();
            string result = await commBus.RunorRevertbuttonenabledisable();
            ViewBag.HourEndRevertEnableDisable = result;
            return View();
        }

        //Serach operation for perticuler employee
        // Searching for loans sanctioned for employee
        public async Task<string> SearchEmployee(string EmpCode)
        {
            return await adjLoanbus.SearchEmployee(EmpCode);
        }
        public async Task<string> GetAdjustmentsLoans(int EmpId)
        {
            return await adjLoanbus.GetAdjustmentsLoanDetails(EmpId);
        }

        public async Task<string> GetChildLoanDetailsofemp(int Id)
        {
            return await adjLoanbus.GetChildLoanDetailsofemp(Id);
        }

        public async Task<string> GetWithoutinterestdata(int Id)
        {
            return JsonConvert.SerializeObject(await adjLoanbus.GetWithoutinterestdata(Id));

        }



        //getting amount values from UI and sending to business layer and insertin amount values into database
        public async Task<string> saveAdjLoanData(CommonPostDTO Values)
        {
            if (Values.StringData3 != null)
            {
                return await adjLoanbus.saveAdjLoanData(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }
        #endregion

    }
}