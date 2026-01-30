
using HRMSApplication.Helpers;
using Newtonsoft.Json;
using PayRollBusiness;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        CommonBusiness CommonBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());

        [HttpGet]
        public async Task<string> SearchEmployee(string EmpCode)
        {  
            return await CommonBus.SearchEmployee(EmpCode);
        }
        //NR Loan Emp search with out reteirment date
        [HttpGet]
        public async Task<string> SearchEmployeeNRL(string EmpCode)
        {  
            return await CommonBus.SearchEmployeeNRL(EmpCode);
        }
         
        [HttpGet]
        public async Task<string> SearchEmployeeDateOfLeaving(string EmpCode)
        {
            return await CommonBus.SearchEmployeeDateOfLeaving(EmpCode);
        }

        [HttpGet]
        public async Task<string> SearchEmployeeWithChkIDs(string EmpCode)
        {
            return await CommonBus.SearchEmployeeWithChkIDs(EmpCode);
        }

        [HttpGet]
        public async Task<string> SearchEmployeeWithScheduleNames(string EmpCode)
        {
            return await CommonBus.GetScheduleTypeDetails();
        }

        [HttpGet]
        public async Task<string> BgLoadInitData()
        {
            string sData = await CommonBus.Bg_Load_InitData();
            string[] arr = sData.Split('*');
            DateTime dt = DateTime.Parse(arr[1]);
            Session["FY"] = arr[0];
            Session["FM"] = new DateTime(dt.Year, dt.Month, 1);
           
            return JsonConvert.SerializeObject("");
        }
    }
}