using PayRollBusiness.Transaction;
using PayrollModels.Transactions;
using System;
using System.Data;
using System.Web.Mvc;
using System.Threading.Tasks;
using PayrollModels;
using HRMSApplication.Helpers;
using Newtonsoft.Json;
using PayRollBusiness.Masters;
using PayrollModels.Masters;
using System.Collections.Generic;
using PayRollBusiness;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class HRMSMastersController : Controller
    {
        LoginCredential lCredentials = LoginHelper.GetCurrentUserForPR();
        CommonBusiness commBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());
        PersonalEarnings epfbus = new PersonalEarnings(LoginHelper.GetCurrentUserForPR());
        PersonalDeductionsBusiness dfbus = new PersonalDeductionsBusiness(LoginHelper.GetCurrentUserForPR());
        PromotionBusiness PRbus = new PromotionBusiness(LoginHelper.GetCurrentUserForPR());
        EncashmentBusiness ENbus = new EncashmentBusiness(LoginHelper.GetCurrentUserForPR());
        IncrementDateChangeBusiness ICbus = new IncrementDateChangeBusiness(LoginHelper.GetCurrentUserForPR());
        JAIIBCAIIBIncrementBusiness jcibus = new JAIIBCAIIBIncrementBusiness(LoginHelper.GetCurrentUserForPR());
        AnnualIncrement AInc = new AnnualIncrement(LoginHelper.GetCurrentUserForPR());
        IncrementAnnualStagnationBusiness IAS=new IncrementAnnualStagnationBusiness(LoginHelper.GetCurrentUserForPR());
        IncomeTaxBankPayment IncTBP = new IncomeTaxBankPayment(LoginHelper.GetCurrentUserForPR());
        PfNomineeBusiness pfnbus = new PfNomineeBusiness(LoginHelper.GetCurrentUserForPR());
        #region Promotions
        // GET: Payroll/HRMSMasters
        public ActionResult Promotion()
        {
         
            ViewBag.SectionName = "Masters";
            //  string query1 = "SELECT id,name FROM Designations";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();


            return View();
        }

        [HttpPost]
        public async Task<string> GetEmpSalaryCond(PFPayable Values)
        {
            var PRdata = await PRbus.GetEmpStopsalDetails(Values.EntityId);
            return PRdata;
        }
          [HttpGet]
        public async Task<string> GetEmpPRFieldsDetails(int EmpId)
        {
          
            var PRdata = await PRbus.GetEmpPRDetails(EmpId);
         
            var PRMdata = JsonConvert.SerializeObject(PRdata);
     
            var authorization= await PRbus.GetEmpPRAuthDetails(EmpId);
            var Authdata = JsonConvert.SerializeObject(authorization);
            var categories =await PRbus.GetEmpPRCategories(EmpId);
            var designations= await PRbus.GetEmpPRDesignations(EmpId);
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var prDetails = javaScriptSerializer.DeserializeObject(PRMdata);
           
            var prdesig= javaScriptSerializer.DeserializeObject(designations);
            var prcat = javaScriptSerializer.DeserializeObject(categories);
            var authorise= javaScriptSerializer.DeserializeObject(Authdata);
            var resultJson = javaScriptSerializer.Serialize(new { proDetails = prDetails,desig= prdesig ,catg=prcat,Currauth= authorise });
            return JsonConvert.SerializeObject(resultJson);
        }
        [HttpPost]
        public async Task<string> saveDataPR(CommonPostDTO Values)
        {
                     
            if (Values.object1 != null )
            {
                return await PRbus.AddPRPayDetails(Values);
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Encashment
        //Encashment
        public ActionResult Encashment()
        {

            ViewBag.SectionName = "Masters";
            //  string query1 = "SELECT id,name FROM Designations";


            return View();
        }
        //[HttpGet]
        //public async Task<string> GetEmpEncashDetails(int EmpId)
        //{


        //    var Plbalance = await ENbus.GetEmpPLbalance(EmpId);
        //    var plencashdata = await ENbus.GetEmpPLdata(EmpId);
        //    var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

        //    var resultJson = javaScriptSerializer.Serialize(new { totalpl = Plbalance, plencash= plencashdata });
        //    return JsonConvert.SerializeObject(resultJson);
        //}

        //[HttpPost]
        //public async Task<string> savePLEncash(CommonPostDTO Values)
        //{
        //    if (Values.StringData != null)
        //    {

        //        return await ENbus.AddPlbalanceDetails(Values);
        //    }
        //    else
        //    {
        //        return "E#Error#Please Enter/Modify Data";
        //    }
           
        //}
        #endregion

        #region IncrementDateChange
        public ActionResult IncrementDateChange()
        {
            ViewBag.SectionName = "Masters";
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        [HttpGet]
        public async Task<string> getIncrementDateChange()
        {
            var dtdays = await ICbus.getmonthDays();
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            var empDetail = await ICbus.getEmpDetails();
            var Mdata = JsonConvert.SerializeObject(dtdays);
            var data = JsonConvert.SerializeObject(empDetail);

            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var incdata = javaScriptSerializer.DeserializeObject(data);
            var mdays = javaScriptSerializer.DeserializeObject(Mdata);

            var resultJson = javaScriptSerializer.Serialize(new { empDetails = incdata, monthdays = mdays });
            return JsonConvert.SerializeObject(resultJson);

            //return JsonConvert.SerializeObject(empDetails, dtPayroll1);
        }
        public async Task<string> searchIncrementDateChange(string SearchData)
        {
           
            var empDetails = await ICbus.searchIncrementDateChange(SearchData);
            return JsonConvert.SerializeObject(empDetails);
        }
        [HttpPost]
        public async Task<string> UpdateIncrementDateChange(List<IncrementDateChangeModel> Values)
        {
            try
            {
                var empDetails = await ICbus.UpdateIncrementDateChange(Values);
                TempData["retMessage"] = empDetails;
                return empDetails;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion

        #region JAIIB_CAIIB_Increments

        public ActionResult JAIIBCAIIBIncr()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            DateTime Financial_md = (lCredentials.FinancialMonthDate);
            ViewBag.fm = Financial_md.ToString("yyyy-MM-dd");
            return View();
        }

        public async Task<string> GetJAIIBCAIIBDetails(int EmpId, string type)
        {
            var data= await jcibus.GetJAIIBCAIIBIncrementsDetails(EmpId, type);
            return data;
        }

        [HttpPost]
        public async Task<string> SaveDataJAIIBCAIIBDetails(CommonPostDTO Values)
        {
            if (Values.StringData != null)
            {

                return await jcibus.AddJAIIBCAIIBDetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }



        #endregion

        #region PersonalEarnings
        public ActionResult PersonalEarnings()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        [HttpGet]
        public async Task<string> GetEmpPayFieldsDetails(int EmpId)
        {
            return await epfbus.GetEmpPayDetails(EmpId);
        }
        [HttpPost]
        public async Task<string> saveDataEPF(CommonPostDTO Values)
        {
            if (Values.StringData != null || Values.objPE != null)
            {
                return await epfbus.PerEarDetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }
        #endregion

        #region PersonalDeductions
        public ActionResult PersonalDeductions()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        [HttpGet]
        public async Task<string> GetEmpDeductionDetails(int EmpId, string Field)
        {
            return await dfbus.GetEmpDeductionDetails(EmpId, Field);
        }
        [HttpPost]
        public async Task<string> saveDataPerDeduction(CommonPostDTO Values)
        {
            if (Values.objpd != null || Values.objperdednew != null)
            {
                return await dfbus.PerDeductionsDetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }
        #endregion

        #region AnnualIncrement
        public ActionResult AnnualIncrement()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        [HttpGet]

        #endregion

        #region IncrementAnnualStagnation
        public ActionResult IncrementAnnualStagnation()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        [HttpGet]
        public async Task<string> getIncAnnualStag()
        {
            var empDetails = await IAS.getIncAnnualStag();
            return JsonConvert.SerializeObject(empDetails);
        }
        [HttpPost]
        public async Task<string> UpdateIncAnnualStag(List<string> data)
        {
            var empDetails = await IAS.UpdateIncAnnualStag(data);
            TempData["retMessage"] = empDetails;
            return empDetails;
        }
        #endregion
        public ActionResult StagnationIncrement()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        [HttpGet]
        public async Task<string> getDecCategories()
        {
            var empDetails = await AInc.getDecCategories();
            return JsonConvert.SerializeObject(empDetails);
        }

        [HttpGet]
        public async Task<string> getPayDatesForAnnualInc()
        {
            var res = await AInc.GetPayPeriodForAnnualIncr();
            return JsonConvert.SerializeObject(res);
           // return await AInc.GetPayPeriod();
           
        }
        [HttpGet]
        public async Task<string> getPayDatesForStagInc()
        {
            var res = await AInc.GetPayPeriodForStagnationIncr();
            return JsonConvert.SerializeObject(res);
            // return await AInc.GetPayPeriod();

        }
        public async Task<string> GetAnnulalData(string DesId, string empAnual)
        {
            var empDetails = await AInc.GetAnullaDataByDes(DesId, empAnual);
            return JsonConvert.SerializeObject(empDetails);
          
        }
        public async Task<string> GetStagnationData(string DesId, string empAnual)
        {
            var empDetails = await AInc.GetStagnationDataByDes(DesId, empAnual);
            return JsonConvert.SerializeObject(empDetails);
          
        }
        

        #region Income Tax bank Payment
        public ActionResult IncomeTaxBankPayment()
        {
            ViewBag.SectionName = "Masters";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public async Task<string> getIncomeTaxBankPayment()
        {
            var IncomeTaxdata = await IncTBP.getIncomeTaxBankPayment();
            return JsonConvert.SerializeObject(IncomeTaxdata);

        }
        [HttpPost]
        public async Task<string> updateIncomeTaxBankPayment(CommonPostDTO Values)
        {

            if (Values.IncomeTaxBank != null)
            {

                return await IncTBP.updateIncomeTaxBankPayment(Values);

            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }


        }

        #endregion

        #region PFNominee

        public ActionResult PfNominee()
        {
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            ViewBag.SectionName = "Masters";
            return View();
        }

        [HttpGet]
        public async Task<string> GetPfNomineeDEtails(string EmpId)
        {
          
            var PfNomineedata = await pfnbus.GetEmpPfnomineeDetails(EmpId);
            return JsonConvert.SerializeObject(PfNomineedata);
        }

        [HttpPost]
        public async Task<string> SaveUpdatePfNomineeData(CommonPostDTO Values)
        {

            if (Values.Objpfnominee != null)
            {

                return await pfnbus.InsertUpdatePfNominee(Values);

            }
            else
            {
                return "E#Error#Please Enter/Modified Data.";
                //return "E#Error#Loan Id Not Modified";
            }


        }
        #endregion
    }
}