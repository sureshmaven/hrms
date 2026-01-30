using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PayRollBusiness.Masters;
using PayrollModels;
using HRMSApplication.Helpers;
using System.Threading.Tasks;
using PayRollBusiness.Autorization;
using PayRollBusiness;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class AuthorizationController : Controller
    {
        CommonBusiness commBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());
        AuthouIncrementDateChangeBusiness AICbus = new AuthouIncrementDateChangeBusiness(LoginHelper.GetCurrentUserForPR());
        PromotionBusiness PRbus = new PromotionBusiness(LoginHelper.GetCurrentUserForPR());
        PromotionAutBus prabus = new PromotionAutBus(LoginHelper.GetCurrentUserForPR());
        JAIIBCAIIBIncrementAuthBusiness jabus = new JAIIBCAIIBIncrementAuthBusiness(LoginHelper.GetCurrentUserForPR());
        PFPayableBusinessAuth payL = new PFPayableBusinessAuth(LoginHelper.GetCurrentUserForPR());
        PFNonReyPayableAuthBusiness pna = new PFNonReyPayableAuthBusiness(LoginHelper.GetCurrentUserForPR());
        public ActionResult Index()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }
        #region Promotions
        // GET: Payroll/HRMSMasters
        public ActionResult PromotionAuthorisation()
        {

            ViewBag.SectionName = "Authorisation";
            //  string query1 = "SELECT id,name FROM Designations";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();


            return View();
        }

        [HttpGet]
        public async Task<string> GetEmpPRFieldsDetails(int EmpId)
        {

            var PRdata = await prabus.GetEmpPRDetails(EmpId);
            var prdataactive1 = await prabus.GetEmpPRDetailsactive1(EmpId);
            var PRMdata = JsonConvert.SerializeObject(PRdata);
            var practive1 = JsonConvert.SerializeObject(prdataactive1);
            var authorization = await PRbus.GetEmpPRAuthDetails(EmpId);
            var Authdata = JsonConvert.SerializeObject(authorization);
            var categories = await prabus.GetEmpPRCategories(EmpId);
            var designations = await prabus.GetEmpPRDesignations(EmpId);
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var prDetails = javaScriptSerializer.DeserializeObject(PRMdata);
            var prDetails1 = javaScriptSerializer.DeserializeObject(practive1);
            var prdesig = javaScriptSerializer.DeserializeObject(designations);
            var prcat = javaScriptSerializer.DeserializeObject(categories);
            var authorise = javaScriptSerializer.DeserializeObject(Authdata);
            var resultJson = javaScriptSerializer.Serialize(new { proDetails = prDetails, proDetails1 = prDetails1,desig = prdesig, catg = prcat, Currauth = authorise });
            return JsonConvert.SerializeObject(resultJson);
        }
        [HttpPost]
        public async Task<string> saveDataPR(CommonPostDTO Values)
        {

            if (Values.object1 != null)
            {
                return await prabus.AddPRPayDetails(Values);
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region JAIIB_CAIIB_Increments

        public ActionResult JAIIBCAIIBIncrAuthrization()
        {
            ViewBag.SectionName = "Authorisation";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        public async Task<string> GetJAIIBCAIIBDetails(int EmpId, string type)
        {
            var data = await jabus.GetJAIIBCAIIBIncrementsDetails(EmpId, type);
            return data;
        }

        [HttpPost]
        public async Task<string> SaveDataJAIIBCAIIBDetails(CommonPostDTO Values)
        {
            if (Values.StringData != null)
            {

                return await jabus.AddJAIIBCAIIBDetails(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
        }



        #endregion

       
        // GET: Payroll/Authorisation
        #region IncrementDateChange
        public ActionResult IncrementDateChange()
        {
            ViewBag.SectionName = "Authorisation";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        [HttpGet]
        public async Task<string> getAuthIncrementDateChange()
        {
            var empDetails = await AICbus.getAuthorisationDetails();
            return JsonConvert.SerializeObject(empDetails);
        }

        [HttpPost]
        public async Task<string> AuthIncrementDateChange(List<inc> Values)
        {
            var empDetails = await AICbus.AuthIncrementDateChange(Values);
            TempData["retMessage"] = empDetails;
            return empDetails;
        }
        #endregion

        #region AnnualStagIncrementAuthorisation
        public ActionResult AnnualStagIncrementAuthorisation()
        {
            ViewBag.SectionName = "Authorisation";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        
        [HttpGet]
        public async Task<string> getAuthStagIncrementDateChange()
        {
            var empDetails = await AICbus.getAuthStagIncrementDateChange();
            return JsonConvert.SerializeObject(empDetails);
        }
        [HttpPost]
        public async Task<string> UpAuthStagIncDateChange(List<string> data)
        {
            var empDetails = await AICbus.UpAuthStagIncDateChange(data);
            TempData["retMessage"] = empDetails;
            return empDetails;
        }
        #endregion


        #region PFRepayable
        public ActionResult PFRepayable()
        {

            ViewBag.SectionName = "Authorisation";
            //  string query1 = "SELECT id,name FROM Designations";

            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();


            return View();
        }
        public async Task<string> getPfPayableData(string emp_code)
        {
            var doc = await payL.getPayableEmpDetails(emp_code);
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
        #endregion
        #region PFNonRepayable
        public ActionResult PFNonRepayable()
        {
            ViewBag.SectionName = "Authorisation";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public async Task<string> getPfNonPayableData(string emp_code)
        {
            var doc = await pna.getNonPayableEmpDetails(emp_code);
            return JsonConvert.SerializeObject(doc);
        }
        [HttpPost]
        public async Task<string> authoriseLoan(authorise Value)
        {
            if (Value.EntityId != null)
            {

                return await pna.getAuthorise(Value.EntityId,Value.loantype);
            }
            else
            {
                return "E#Error#Please Enter Employee Code";
            }
        }
        #endregion

        public ActionResult PFAuthorisation()
        {
            ViewBag.SectionName = "Authorisation";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public ActionResult Encashment()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }
    }
}