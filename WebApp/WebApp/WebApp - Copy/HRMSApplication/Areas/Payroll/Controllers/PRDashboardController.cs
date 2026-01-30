using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    public class PRDashboardController : Controller
    {
      
        CommonBusiness cbus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());
        // GET: Payroll/PRDashboard
        public ActionResult Index()
        {
            return View();
        }
        public async Task<string> GetEmpData(string EmpCode)
        {
            var Empdata = await cbus.GetEmpData();
            return Empdata;
        }
        public async Task<string> GetProcessCount()
        {
            string sData = await cbus.Bg_Load_InitDataCount();
            return JsonConvert.SerializeObject(sData);
        }
    }
}