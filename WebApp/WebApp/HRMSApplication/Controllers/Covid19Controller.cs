using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using Entities;
using Repository;
using HRMSApplication.Models;
using HRMSApplication.Helpers;
using HRMSApplication.Filters;
using HRMSBusiness.Business;
using Newtonsoft.Json;

namespace HRMSApplication.Controllers
{
    public class Covid19Controller : Controller
    {
        Covid19Business cbus = new Covid19Business();
        private ContextBase db = new ContextBase();
        // GET: Covid19
        public ActionResult Covid19Form()
        {
            ViewBag.FormSubmitted = "false";
            LoginCredential lcredentials = LoginHelper.GetCurrentUser();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var empid = db.Employes.Where(a => a.EmpId == lcredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            var getempdata = cbus.empdata(empid);
            DataTable sureveydata = cbus.empdatainservey(empid);
            DataTable editdata = cbus.EditCovidFormData(empid);
            ViewBag.data = editdata;
            ViewBag.MyStates = JsonConvert.SerializeObject(editdata);
            if (sureveydata.Rows.Count > 0)
            {
                //ViewBag.FormSubmitted = "true";
                //TempData["AlertMessage"] = " Covid-19 Form Submitted Sucessfully..!";
               // return RedirectToAction("Covid19FormReadOnly", "Covid19");
                //return RedirectToAction("Index", "DashBoard");
                ////return RedirectToAction("dashboard")
                return View();
            }
            else
            {
                ViewBag.name = getempdata.Rows[0]["lastname"];
                ViewBag.gender = getempdata.Rows[0]["gender"];
                return View();
            }
            //ViewBag.name = getempdata.Rows[0]["lastname"];
            //ViewBag.gender = getempdata.Rows[0]["gender"];
            //return View();
        }

        [HttpPost]
        public ActionResult Covid19Post(CovidViewModel LTC, string EmpCode)
        {
            ViewBag.FormSubmitted = "false";
            LoginCredential lcredentials = LoginHelper.GetCurrentUser();
            var empid = db.Employes.Where(a => a.EmpId == lcredentials.EmpId).Select(a => a.EmpId).FirstOrDefault();
            var coviddata = cbus.AddCovid19(LTC, empid);
            TempData["AlertMessage"] = "Your Covid-19 Form submitted, Thank you!";
            return RedirectToAction("Index", "DashBoard");
        }

        public ActionResult Covid19FormReadOnly()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }
    }
}