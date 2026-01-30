using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMSApplication.Areas.Design.Controllers
{
    public class CustomizationController : Controller
    {
        // GET: Design/Customization
        public ActionResult Index()
        {
            ViewBag.SectionName = "Customization";
            return View();
        }
        public ActionResult LoansAdvances()
        {
            ViewBag.SectionName = "Customization";
            return View();
        }
    }
}