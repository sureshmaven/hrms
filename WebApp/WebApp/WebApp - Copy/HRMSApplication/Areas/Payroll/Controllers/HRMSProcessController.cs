//using System;
using HRMSApplication.Helpers;
using Newtonsoft.Json;
using PayRollBusiness;
using PayRollBusiness.Process;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class HRMSProcessController : Controller
    {
        CommonBusiness commBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());
        ProcessBusiness PBus = new ProcessBusiness(LoginHelper.GetCurrentUserForPR());
        // GET: Payroll/HRMSProcess
        
        #region PostIncrements
        public ActionResult PostIncrements()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        [HttpGet]
        public async Task<string> getProcessStagIncrementDateChange()
        {
            var empDetails = await PBus.getAuthStagIncrementDateChange();
            return JsonConvert.SerializeObject(empDetails);
        }
        [HttpPost]
        public async Task<string> UpprocessStagIncDateChange(List<string> data)
        {
            var empDetails = await PBus.UpAuthStagIncDateChange(data);
            TempData["retMessage"] = empDetails;
            return empDetails;
        }
        #endregion
        public ActionResult SingleEmployee()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public ActionResult AllEmployees()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public ActionResult NonRepayableLoans()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
        public ActionResult RepayableLoans()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
    }
}