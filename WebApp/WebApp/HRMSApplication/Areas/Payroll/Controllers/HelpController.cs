using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class HelpController : Controller
    {
        // GET: Payroll/Help
        public ActionResult Index()
        {
            return View();
        }
    }
}