
using HRMSBusiness.Business;
using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayrollModels.Masters;
using PayRollBusiness.Masters;
using HRMSBusiness;
using System.Threading.Tasks;
using PayrollModels;
using HRMSApplication.Helpers;
using PayRollBusiness;

using PayRollBusiness.Process;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class MastersController : Controller
    {
        CommonBusiness commBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());
        private ContextBase db = new ContextBase();
        PayrollBusiness pbus = new PayrollBusiness();
        AllowanceBusiness abus = new AllowanceBusiness(LoginHelper.GetCurrentUserForPR());
        EmployeeMasterBusiness ebus = new EmployeeMasterBusiness(LoginHelper.GetCurrentUserForPR());
        HouseRentalDetailsBusiness hrdbus = new HouseRentalDetailsBusiness(LoginHelper.GetCurrentUserForPR());
        LoansAdvancesBus labus = new LoansAdvancesBus(LoginHelper.GetCurrentUserForPR());
        PFNonPayableBusiness pfn = new PFNonPayableBusiness(LoginHelper.GetCurrentUserForPR());
        PFPayableBusiness payL = new PFPayableBusiness(LoginHelper.GetCurrentUserForPR());
        PFPayableOBShareBusiness payOB = new PFPayableOBShareBusiness(LoginHelper.GetCurrentUserForPR());
        PromotionBusiness PRbus = new PromotionBusiness(LoginHelper.GetCurrentUserForPR());
        Form12BABusiness form12Ba = new Form12BABusiness(LoginHelper.GetCurrentUserForPR());
        OtherTdsDeductionBusiness otds = new OtherTdsDeductionBusiness(LoginHelper.GetCurrentUserForPR());
        Form16ScreenBusiness form16 = new Form16ScreenBusiness(LoginHelper.GetCurrentUserForPR());

        //GET: Payroll/Masters
        public ActionResult Index()
        {
            ViewBag.SectionName = "Masters";
            return View();
        }
        #region Employee master
        public ActionResult EmployeeMaster()
        {
            ViewBag.DataUrl = "Payroll/PrAllReports/IncrementReportData?fromdate=-1&todate=-2";
            ViewBag.ReportColumns = @"[{""title"": ""Emp Id"", ""data"": ""Id"",  ""autoWidth"": true },{ ""title"": ""Emp Name"",""data"": ""Name"", ""autoWidth"": true },{ ""title"": ""Designation"",""data"": ""Designation"",  ""autoWidth"": true },{ ""title"": ""Increment Date"",""data"": ""IncDate"", ""autoWidth"": true }]"; ;

            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        [HttpGet]
        public async Task<string> EmployeeCodeSearchPostForDetails(string EmpCode)
        {
            var employeeGeneral = await ebus.GetEmployeDeatilsById(EmpCode);// emp general data
            var employeeBiological = await ebus.GetEmployeeBiologicalData(EmpCode); // emp biological data
            var employeePayField = await ebus.GetEmployeePayFieldData(EmpCode); // emp pay field data
            var employeeAllowance = await ebus.GetEmployeeAllowanceData(EmpCode);//allowance data
            var employeeSpecalAllowance = await ebus.GetEmployeeAllowanceSpecialData(EmpCode); //speacil allowance data
            var employeeDeduction = await ebus.GetEmployeeDeductionData(EmpCode);// deduction data
            var employeeLic = await ebus.GetEmployeeLicData(EmpCode);//Lic Data
            var employeeHfc = await ebus.GetEmployeeHfcData(EmpCode);// Hfc Data


            var empGeneral = JsonConvert.SerializeObject(employeeGeneral);
            var empBio = JsonConvert.SerializeObject(employeeBiological);
            var empLic = JsonConvert.SerializeObject(employeeLic);
            var empHfc = JsonConvert.SerializeObject(employeeHfc);

            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var empGeneralDetails = javaScriptSerializer.DeserializeObject(empGeneral);
            var empBioData = javaScriptSerializer.DeserializeObject(empBio);
            var empPayField = javaScriptSerializer.DeserializeObject(employeePayField);
            var empAllowanceData = javaScriptSerializer.DeserializeObject(employeeAllowance);
            var empSpeaciAllowanceData = javaScriptSerializer.DeserializeObject(employeeSpecalAllowance);
            var empDeductionData = javaScriptSerializer.DeserializeObject(employeeDeduction);
            var empLicData = javaScriptSerializer.DeserializeObject(empLic);
            var empHfcData = javaScriptSerializer.DeserializeObject(empHfc);



            var resultJson = javaScriptSerializer.Serialize(new
            {
                General = empGeneralDetails,
                Bio = empBioData,
                Pay = empPayField,
                Allowance = empAllowanceData,
                speailAllowance = empSpeaciAllowanceData,
                Deduction = empDeductionData,
                Lic = empLicData,
                Hfc = empHfcData
            });
            return JsonConvert.SerializeObject(resultJson);
        }

        [HttpGet]
        public async Task<string> getEmpCategoriesList()
        {
            var categories = await PRbus.GetEmpPRCategories();
            return categories;
        }

        [HttpGet]
        public async Task<string> getDesignations()
        {
            var categories = await PRbus.GetEmpDesignations();
            return categories;
        }

        [HttpPost]
        public async Task<string> InsertEmployeeMasterData(CommonPostDTO values)
        {
            return await ebus.InsertGeneralData(values);
        }

        [HttpPost]
        public async Task<string> UpdateEmployeeMasterData(CommonPostDTO Values)
        {

            if (Values.object1 != null || Values.object2 != null || Values.StringData != null
                || Values.StringData2 != null || Values.StringData3 != null
                || Values.StringData4 != null || Values.object3 != null
                || Values.multiObject != null || Values.multiObject2 != null)
            {

                return await ebus.UpdateGeneralData(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }


        }
        [HttpGet]
        public JsonResult EmployeeCodeSearch(string criteria)
        {
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var employees = db.Employes.ToList();
            var lResult = (from userslist in employees
                           where userslist.RetirementDate >= lStartDate
                           select new
                           {
                               userslist.EmpId,
                               Name = userslist.FirstName + "" + userslist.LastName,
                           });
            var lresponseArray = lResult.ToArray();
            return Json(lresponseArray.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        public static DateTime GetCurrentTime(DateTime ldate)
        {
            DateTime serverTime = DateTime.Now;
            DateTime utcTime = serverTime.ToUniversalTime();
            // convert it to Utc using timezone setting of server computer
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }


        #region Allowence master
        public ActionResult Allowance()
        {
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            ViewBag.SectionName = "Masters";
            return View();
        }

        public async Task<string> getAllowanceDetails()
        {

            var res = await abus.getAllowanceDetails();
            return JsonConvert.SerializeObject(res);
        }



        [HttpPost]
        public async Task<string> updateAllowance(CommonPostDTO Values)
        {
            if (Values.objallowancemasterdata != null || Values.objallowancemasterdatanew != null)
            {
                return await abus.UpdateAllowanceMaster(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }

        }
        #endregion

        #region employee_RentDetails

        public ActionResult RentDetails()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        public async Task<string> GetHouseRentDetails(int EmpId)
        {

            return await hrdbus.GetHouseRentDetails(EmpId);
        }

        [HttpPost]
        public async Task<string> SaveHouseRentDetailsValues(CommonPostDTO Values)
        {
            if (Values.StringData != null)
            {
                return await hrdbus.AddHouseRentDetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }
        #endregion

        #region LoansAdvances
        public ActionResult LoansAdvances()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        public async Task<string> Getloansmasterdetails()
        {
            return await labus.GetLoanTypeDetails();
        }
        public async Task<string> GetLoanTypeDetailsforReport()
        {
            return await labus.GetLoanTypeDetailsforReport();
        }
        public async Task<string> GetLoanfieldDetails(int EmpId, string type, int id, string loancode)
        {
            var data = await labus.GetLoanDetails(EmpId, type, id, loancode);
            return data;

        }

        [HttpPost]
        public async Task<string> SaveLoanDetailsfields(CommonPostDTO values)
        {
            return await labus.SaveLoanDetails(values);
        }



        #endregion

        #region RepayableLoans
        public ActionResult RepayableLoans()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public ActionResult RepayableOBShare()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public async Task<string> getPfPayableData(string emp_code)
        {
            var doc = await payL.getPayableEmpDetails(emp_code);
            return JsonConvert.SerializeObject(doc);

        }
        public async Task<string> getOBShareData(string emp_code)
        {
            var doc = await payOB.getOBShareData(emp_code);
            //var doc1 = await payOB.getOBShareData(emp_code);
            return JsonConvert.SerializeObject(doc);
        }
        public async Task<string> getLoanPurposePayable()
        {
            var res = await payL.getLoansPurpose();
            return JsonConvert.SerializeObject(res);
        }
        public async Task<string> getLoanPFPayable()
        {
            var res = await payL.getLoansPFPayable();
            return JsonConvert.SerializeObject(res);
        }
        [HttpPost]
        public async Task<string> SavePFPayableData(PFPayable Values)
        {
            if (Values != null)
            {
                return await payL.savePFPayableData(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }

        }

        //update loan
        [HttpPost]
        public async Task<string> UpdatePFPayableData(PFPayable Values)
        {
            if (Values != null)
            {

                return await payL.UpdatePFPayableData(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }

        }
        //delete loan type 
        [HttpPost]
        public async Task<string> DeletePFPayableLoan(CommonPostDTO values)
        {
            string purposeType = values.StringData.ToString();

            string Emp_code = values.StringData2.ToString();
            var DeleteLoan = await payL.DeletePFPayableLoan(purposeType, Emp_code);
            TempData["retMessage"] = DeleteLoan;
            return DeleteLoan;
        }

        public async Task<string> getOptionByPFrepyableType(string empcode, string type)
        {
            var doc = await payL.getPfLoanData(empcode, type);

            return JsonConvert.SerializeObject(doc);

        }

        #endregion

        #region NonRepayableLoans
        public ActionResult NonRepayableLoans()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public async Task<string> getPfNonPayableData(string emp_code)
        {
            var doc = await pfn.getNonPayableEmpDetails(emp_code);
            return JsonConvert.SerializeObject(doc);
        }
        //public async Task<string> getOptionByPFType(string type)
        //{
        //    var doc = await pfn.getPfLoanDocuments(type);

        //    return JsonConvert.SerializeObject(doc);

        //}
        public async Task<string> getOptionByPFType(string type, string EmpId)
        {
            var doc = await pfn.getPfLoanDocuments(type, EmpId);

            return doc.ToString();

        }
        public async Task<string> getLoanPurpose()
        {
            var res = await pfn.getLoansPurpose();
            return JsonConvert.SerializeObject(res);

        }
        public async Task<string> getLoanPurposeforretire()
        {
            var resforretire = await pfn.getLoansPurposeforretire();
            return JsonConvert.SerializeObject(resforretire);

        }
        [HttpPost]
        public async Task<string> SavePFNonPayableData(PFNonPayable Values)
        {
            if (Values != null)
            {

                return await pfn.savePFNonPayableData(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }

        }
        //update loan
        [HttpPost]
        public async Task<string> UpdatePFNonPayableData(PFNonPayable Values)
        {
            if (Values != null)
            {

                return await pfn.updatePFNonPayableData(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }

        }
        //delete loan type 
        [HttpPost]
        public async Task<string> DeleteLoan(CommonPostDTO values)
        {
            string purposeType = values.StringData.ToString();
            string emp_code = values.StringData2.ToString();
            var DeleteLoan = await pfn.DeleteLoan(purposeType, emp_code);
            TempData["retMessage"] = DeleteLoan;
            return DeleteLoan;
        }
        #endregion

        #region Form12BA
        public ActionResult Form12BA()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }


        public async Task<string> getForm12BADetails(string EmpId)
        {
            return await form12Ba.GetForm12BAData(EmpId);

        }

        [HttpPost]
        public async Task<string> saveForm12BADetails(Form12BA Values)
        {
            if (Values != null)
            {

                return await form12Ba.saveForm12BADetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }

        }
        #endregion

        #region OtherTdsDeduction
        public ActionResult OtherTdsDeduction()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public async Task<string> getOtherTDSDeductions(string EmpId)
        {

            return JsonConvert.SerializeObject(await otds.getOtherTDSDeductions(EmpId));

        }
        [HttpPost]
        public async Task<string> saveOtherTDSDeductions(OtherTDSDeduction Values)
        {
            if (Values != null)
            {

                return await otds.saveOtherTDSDeductions(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }

        }
        #endregion
        #region PF Final Settlement
        public ActionResult PFFinalSettlement()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public async Task<string> getPFFinalData(string emp_code)
        {
            PFFinalSettlement Pfb = new PFFinalSettlement(LoginHelper.GetCurrentUserForPR());

            var doc = await Pfb.getPFDetailsFinal(emp_code);
            return JsonConvert.SerializeObject(doc);
        }




        #endregion
        #region Form 12B
        public ActionResult Form12B()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        public async Task<string> getForm12BDetails(string EmpId)
        {

            string data = await form12Ba.GetForm12Data(EmpId);
            if (data.Length > 0)
            {
                return data;
            }
            else
            {
                return "E#Error#Employee Did't Join In This Financial Year";

            }

        }
        //[HttpPost]
        //public async Task<string> saveData12B(form12b Values)
        //{

        //        return await form12Ba.Save12BData(Values);

        //}
        [HttpPost]
        public async Task<string> saveData12B(CommonPostDTO Values)
        {
            if (Values.StringData != null)
            {
                return await form12Ba.saveForm12Data(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }
        #endregion

        #region Form16
        public async Task<ActionResult> Form16()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            ViewBag.DataUrl = "/Payroll/Masters/GetForm16Data";
            var fYears = await form16.GetForm16Data();
            ViewBag.DdlOneData = new SelectList(fYears, "Id", "fY");
            return View();
        }

        public async Task<string> getData(string mnth)
        {
            var data = await form16.getData(mnth);
            return data;
        }


        [HttpPost]
        public async Task<string> saveForm16(form16 Values)
        {

            if (Values != null)
            {
                return await form16.saveForm16Data(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }
        #endregion

    }
}