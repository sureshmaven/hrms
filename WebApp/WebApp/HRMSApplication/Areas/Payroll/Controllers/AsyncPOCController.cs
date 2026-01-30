using Newtonsoft.Json;
using PayRollBusiness.Poc;
using PayrollModels;
using PayrollModels.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PayRollBusiness.Masters;
using HRMSApplication.Helpers;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class AsyncPOCController : Controller
    {
        PocBusiness pocbus = new PocBusiness(LoginHelper.GetCurrentUserForPR());

        public ActionResult AddRow()
        {
            ViewBag.SectionName = "Process";
            return View();
        }

        // GET: Payroll/AsyncPOC
        public ActionResult Department()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
        public ActionResult UpdateColumns()
        {
            ViewBag.SectionName = "Process";
            return View();
        }

        public async Task<string> getDepartmentData(string empid)
        {
            var res = await pocbus.getPocDepartmentMaster(empid);
            return JsonConvert.SerializeObject(res);
        }
        public async Task<string> getAddRowData(int EmpCode)
        {
            var res = await pocbus.getAddRowData(EmpCode);
            return JsonConvert.SerializeObject(res);
        }
        [HttpPost]
        public async Task<string> updateMultipleColumnGridData(CommonPostDTO Values)
        {

            if (Values.multiObject != null)
            {

                return await pocbus.UpdateGeneralData(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }


        }
        [HttpPost]
        public async Task<string> saveDepartmentData(CommonPostDTO data)
        {
            string sRet = "";
            var res = await pocbus.ProcessRequest(data.EntityId, data.StringData);
            return sRet;
        }
        [HttpGet]
        public string getEmpMasters()
        {
            List<CommonGetModel> lst = new List<CommonGetModel>();
            lst.Add(new CommonGetModel { Id = "1", Name = "Empolyee Short Name", Value = "Emp 123" });
            lst.Add(new CommonGetModel { Id = "2", Name = "Sex", Value = "Female" });
            lst.Add(new CommonGetModel { Id = "3", Name = "Basic", Value = "21500" });
            lst.Add(new CommonGetModel { Id = "4", Name = "Dob", Value = "2018-05-27" });
            lst.Add(new CommonGetModel { Id = "5", Name = "Exp", Value = "3 years 2 mothds 5 days" });
            lst.Add(new CommonGetModel { Id = "6", Name = "Blood", Value = "B+" });
            lst.Add(new CommonGetModel { Id = "7", Name = "Email", Value = "chandu@gmail.com" });
            return JsonConvert.SerializeObject(lst);
        }
    }
}