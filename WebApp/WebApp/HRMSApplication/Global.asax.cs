using HRMSApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace HRMSApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public override void Init()
        {
            this.PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(
                SessionStateBehavior.Required);
        }

        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);

            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                
                        string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        string roles = string.Empty;
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                        new System.Security.Principal.GenericIdentity(username, "Forms"), roles.Split(';'));
                    }
                    catch (Exception)
                    {
                        //somehting went wrong
                    }
                }
            }
        }


        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST, PUT, DELETE");

                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Origin,X-Requested-With,Content-Type,Accept,Authorization");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Credentials", "true");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
            // To Get Holiday List code please un comment This lines
            //HRMSApplication.Models.HolidayCode.getsundays();
            CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            newCulture.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";
            newCulture.DateTimeFormat.DateSeparator = "-";
            Thread.CurrentThread.CurrentCulture = newCulture;
        }
        //protected void Application_BeginRequest(Object sender, EventArgs e)
        //{
        //    //        var allowedOrigins = new List<string>
        //    //{
        //    //    "https://hrms.tscab.org",
        //    //    "http://localhost:63539",
        //    //    "http://192.168.0.31:8089"
        //    //};

        //    //         string origin = HttpContext.Current.Request.Headers["Origin"];
        //   // string host = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        //    // Example output: "https://hrms.tscab.org"
        //    //LogInformation.debug("host   ", host);

        //    //if (!string.IsNullOrEmpty(host) && allowedOrigins.Contains(host))
        //    //{
        //    //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", host);
        //    //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Credentials", "true");
        //    //    HttpContext.Current.Response.AddHeader("X-Frame-Options", "DENY");
        //    //    HttpContext.Current.Response.AddHeader("Cache-Control", "no-store, max-age=0, must-revalidate");
        //    //    HttpContext.Current.Response.AddHeader("X-Content-Type-Options", "nosniff");
        //    //    HttpContext.Current.Response.AddHeader("X-XSS-Protection", "1; mode=block");
        //    //    HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
        //    //    HttpContext.Current.Response.AddHeader("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';");
        //    //}

        //    //if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
        //    //{
        //    //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
        //    //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Origin,X-Requested-With,Content-Type,Accept,Authorization");
        //    //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Credentials", "true");
        //    //    HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
        //    //    HttpContext.Current.Response.End();
        //    //}

        //    //        // Set custom date format
        //    //        CultureInfo newCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        //    //        newCulture.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";
        //    //        newCulture.DateTimeFormat.DateSeparator = "-";
        //    //        Thread.CurrentThread.CurrentCulture = newCulture;

        //    // Add security headers
        //    HttpContext.Current.Response.AddHeader("X-Frame-Options", "DENY");
        //    HttpContext.Current.Response.AddHeader("Cache-Control", "no-store, max-age=0, must-revalidate");
        //    HttpContext.Current.Response.AddHeader("X-Content-Type-Options", "nosniff");
        //    HttpContext.Current.Response.AddHeader("X-XSS-Protection", "1; mode=block");

        //    // Example Content-Security-Policy (CSP) header, adjust according to the assets used in your app
        //    HttpContext.Current.Response.AddHeader("Content-Security-Policy", "default-src 'self' data: blob:; script-src 'self' 'unsafe-inline''unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data:;");

        //    // CORS headers
        //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

        //    if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
        //    {
        //        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST, PUT, DELETE");
        //        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
        //        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Credentials", "true");
        //        HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
        //        HttpContext.Current.Response.End();
        //    }

        //    // Custom Culture Settings
        //    CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        //    newCulture.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";
        //    newCulture.DateTimeFormat.DateSeparator = "-";
        //    Thread.CurrentThread.CurrentCulture = newCulture;
        //}

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            // Remove the "Server" header from the response
            HttpContext.Current.Response.Headers.Remove("Server");

            // Remove additional headers to prevent version disclosure
            HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
            HttpContext.Current.Response.Headers.Remove("X-Powered-By");
            HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");
        }


        //Handling Errors Globally Throughout The Application
        protected void Application_Error(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();
            Exception exception = Server.GetLastError();
              HttpContext.Current.Session["Error"] = exception.Message;
             LogInformation.Error("Global error HRMS:" + exception.Message);
            Server.ClearError();
            FormsAuthentication.SignOut();
            Response.Redirect("/Home/Index");
        }


    }
}
