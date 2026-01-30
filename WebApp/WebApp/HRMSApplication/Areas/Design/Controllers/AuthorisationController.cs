using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMSApplication.Areas.Design.Controllers
{
    public class AuthorisationController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }
        public ActionResult PromotionAuthorisation()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }
        public ActionResult IncrementDateChange()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }
        public ActionResult AnnualStagIncrementAuthorisation()
        {
            ViewBag.SectionName = "Authorisation";
            return View();      
        }
        public ActionResult JAIIBCAIIBGeneral()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }
        public ActionResult Encashment()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }
        public ActionResult ROIChange()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }
        public ActionResult NonRepayableLoans()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }
        public ActionResult RepayableLoans()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }
        public ActionResult PFAuthorisation()
        {
            ViewBag.SectionName = "Authorisation";
            return View();
        }

    }
}