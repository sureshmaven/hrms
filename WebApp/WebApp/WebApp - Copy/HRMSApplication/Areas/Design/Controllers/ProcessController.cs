using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMSApplication.Areas.Design.Controllers
{
    public class ProcessController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
        public ActionResult PaySlip()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
        public ActionResult EncashmentProcess()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
        public ActionResult GratuityProcess()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
        public ActionResult TDSUpdate()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
        public ActionResult PostIncrements()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
        public ActionResult Single_employee()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
    }
}