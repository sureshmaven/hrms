using System;

using System.Web.Mvc;

namespace HRMSApplication.Controllers

{

    public class ErrorController : Controller

    {

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ErrorController));

        // GET: Error

        public ActionResult Index()

        {

            return View("General");

        }

        // GET: Error/General

        public ActionResult General()

        {

            Response.StatusCode = 500;

            Response.TrySkipIisCustomErrors = true;

            ViewBag.Title = "Unexpected Error";

            ViewBag.Message = "Something went wrong while processing your request. Please try again later or reach out to your HRMS administrator if the problem persists.";

            logger.Info("General error page accessed");

            return View();

        }

        // GET: Error/NotFound

        public ActionResult NotFound()

        {

            Response.StatusCode = 404;

            Response.TrySkipIisCustomErrors = true;

            ViewBag.Title = "Page Not Found";

            ViewBag.Message = "The page you are looking for doesn’t exist or may have been moved. Please verify the link or contact your HRMS administrator if you need assistance.";

            logger.Info("404 error page accessed");

            return View();

        }

        // GET: Error/ServerError

        public ActionResult ServerError()

        {

            Response.StatusCode = 500;

            Response.TrySkipIisCustomErrors = true;

            ViewBag.Title = "Server Error";

            ViewBag.Message = "The system encountered an internal issue. Please try again after some time. If the issue continues, contact your HRMS administrator.";

            logger.Info("500 error page accessed");

            return View();

        }

        // GET: Error/Unauthorized

        public ActionResult Unauthorized()

        {

            Response.StatusCode = 401;

            Response.TrySkipIisCustomErrors = true;

            ViewBag.Title = "Unauthorized Access";

            ViewBag.Message = "You do not have permission to access this page. Please check your access rights or contact your HRMS administrator for support.";

            logger.Info("401 error page accessed");

            return View("Unauthorized");

        }

        // Handle 403 Forbidden

        public ActionResult Forbidden()

        {

            Response.StatusCode = 403;

            Response.TrySkipIisCustomErrors = true;

            ViewBag.Title = "Access Forbidden";

            ViewBag.Message = "Your access to this resource is restricted. Please contact your HRMS administrator if you believe this is an error.";

            logger.Info("403 error page accessed");

            return View();

        }

    }

}

