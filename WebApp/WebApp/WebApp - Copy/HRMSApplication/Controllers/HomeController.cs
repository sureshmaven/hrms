using Entities;
using HRMSApplication.Filters;
using HRMSApplication.Helpers;
using HRMSApplication.Models;
using HRMSBusiness.Business;
using HRMSBusiness.Comm;
using HRMSBusiness.Db;
using IpPublicKnowledge;
using Mavensoft.DAL.Db;
using Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static HRMSBusiness.Business.LoginBus;

namespace HRMSApplication.Controllers
{
    public class HomeController : Controller
    {
        private ContextBase db = new ContextBase();
        private ContextBase db1 = new ContextBase("SecondConnection");

        LoginCredential lCredentails = LoginHelper.GetCurrentUser();
        PswdEncryptDecrypt lencrptypassword = new PswdEncryptDecrypt();

        [HttpGet]
        public ActionResult Index()

        {
            LoginHelper.Logout();
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
            }
            var lmodel = new Employees();
            var rng = RandomNumberGenerator.Create();
            var salt = new byte[12];
            rng.GetBytes(salt);
            var saltString = "";
            var a = 0;
            foreach (var i in salt)
            {
                saltString += i;
            }
            lmodel.branchhide = saltString;
            return View(lmodel);
        }
        [Authorize]
        [HttpGet]
        public ActionResult PayrollIndex()
        {


            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            Session.SetDataToSession<string>("EmpFullName", lCredentails.EmpFullName);
            Session.SetDataToSession<string>("ActiveEmployee", lCredentails.EmpId);
            Session.SetDataToSession<string>("CurrDesig", lCredentails.CurrDesig); Session.SetDataToSession<string>("ActiveImage", lCredentails.EmpImage);
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }
        [HttpPost]
        public ActionResult Loginuser(Employees Employee, FormCollection Form)
        {
            // var SHA512UIHashPwd = Form["HiddenPwd"];
            // var EncryptSalt = Employee.branchhide;
            // string Radiovalue = Form["HRMS"];
            LoginBus lgbus = new LoginBus();
            // string[] payrollaccess = ConfigurationManager.AppSettings["PayrollAccessId"].Split(',');
            string[] LeavesSanctioning = ConfigurationManager.AppSettings["LeavesSanctioning"].Split(',');
            LoginResult lResult = lgbus.getLoginInformation(Employee.EmpId, Employee.Password);
            string EmpId = lResult.EmpId;

            //var AesPassword = db.Employes.Where(e => e.EmpId == Employee.EmpId).Select(e => e.Password).FirstOrDefault();
            //var AesDecrptPwd = lgbus.Decrypt(AesPassword);
            //var OriginalDbPwd = Encoding.UTF8.GetBytes(AesDecrptPwd + EncryptSalt);
            //var OriginalPWDHash = SHA512Encryption(OriginalDbPwd);
            //LoginResult lResult = new LoginResult();
            //string EmpId = Employee.EmpId;
            //if (SHA512UIHashPwd == OriginalPWDHash)
            //{
            //    //remove password comparing in below method, because , we already checked pwd.
            //     lResult = lgbus.getLoginInformation(Employee.EmpId, AesDecrptPwd);

            //}
            //else
            //{
            //    TempData["status"] = "Invalid Username or Password";
            //    LogInformation.Info("Invalid userid or password tried to login with this id " + Employee.EmpId);
            //    return RedirectToAction("Index", "Home");
            //}

            // var ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            var ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            if (lResult.Success == true)
            {
                FormsAuthentication.SetAuthCookie(Employee.EmpId, false);
                this.Session["EmpId"] = Employee.EmpId;
                this.Session["LoginMode"] = lResult.loginMode;
                this.Session["EmpPkId"] = lResult.EmpPkId;
                this.Session["EmpImage"] = lResult.EmployeeImage;
                this.Session["EmpFullName"] = lResult.EmployeeFullName;
                this.Session["EmpShortName"] = lResult.ShortName;
                this.Session["Branch"] = lResult.Branch;
                this.Session["BranchName"] = lResult.BranchName;
                this.Session["Department"] = lResult.Department;
                this.Session["Designation"] = lResult.Designation;
                this.Session["Role"] = lResult.Role;
                this.Session["CurrDesig"] = lResult.CurrDesig;
                this.Session["Approvals"] = lResult.Approvals;
                // LogInformation.Debug(" Logged Ip Address : " + ip + " " + "At :" + DateTime.Now);
                LogInformation.Info("Loginuser, Successfully " + lResult.ShortName + " Logged Ip Address : " + ip + " " + "At :" + DateTime.Now);

                int lEmpId = db.Employes.Where(a => a.EmpId == Employee.EmpId).Select(a => a.Id).FirstOrDefault();
                string lids = Convert.ToString(lEmpId);
                string lcontrolling = db.Employes.Where(a => a.ControllingAuthority == lids).Select(a => a.ControllingAuthority).FirstOrDefault();
                string lsanction = db.Employes.Where(a => a.SanctioningAuthority == lids).Select(a => a.SanctioningAuthority).FirstOrDefault();

                this.Session["RolePages"] = lgbus.getUserPages(EmpId, LeavesSanctioning, lResult.Role, lResult.Approvals, lResult.Designation, lcontrolling, lsanction, lResult.Department);
                string empstring = "";
                Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
                string qry = "select constant from all_constants where functionality='LoginAccess' and active=1";
                DataTable constat = sh.Get_Table_FromQry(qry);
                foreach (DataRow dr in constat.Rows)
                {
                    empstring = dr["constant"].ToString();
                }
                if ((lResult.loginMode == Constants.SuperAdmin) || (lResult.loginMode == Constants.AdminHRDPayments) || (lResult.loginMode == Constants.AdminHRDPolicy)
                    || (lResult.loginMode == Constants.Executive) || (lResult.loginMode == Constants.Manager) || (lResult.loginMode == Constants.Employee))
                {

                    if (empstring.Split(',').Contains(lResult.EmpId))
                    {
                        return RedirectToAction("PayrollIndex", "Home");
                    }
                    else if (lResult.loginMode == Constants.Employee)
                    {
                        return RedirectToAction("EmployeeDashBoard", "Dashboard");
                    }

                    else
                    {

                        return RedirectToAction("Index", "Dashboard");

                    }
                }
                else
                {
                    return RedirectToAction("EmployeeDashBoard", "Dashboard");
                }
            }
            else
            {
                TempData["status"] = lResult.Message;
                // ViewBag.Status = lResult.Message;
                LogInformation.Info("Invalid userid or password tried to login with this id " + Employee.EmpId);
                return RedirectToAction("Index", "Home");

            }



        }


        public JsonResult CheckEmpid(string EmpID)
        {
            var queryAllEmployee = from br in db.Employes where br.EmpId.Equals(EmpID) select br;
            int count = queryAllEmployee.Count();


            if (count != 0)
            {
                return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Policies()


        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            Employees lmodel = new Employees();
            lmodel.LoginMode = lCredentails.LoginMode;
            TempData["Loginmode"] = lCredentails.LoginMode;
            return View(lmodel);

        }

        [HttpGet]
        [NoDirectAccess]
        //[SessionTimeoutAttribute]
        public ActionResult ForgotPassword()
        {
            //TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ForgotPassword(Employees model, FormCollection form)
        {
            string userid = "";
            string pwd = "";
            string luser = form["OfficalEmailId"];
            try
            {
                string lUsername = db.Employes.Where(a => a.OfficalEmailId == model.OfficalEmailId).Select(a => a.LoginMode).FirstOrDefault();
                string lAdmin = db.Employes.Where(a => a.OfficalEmailId == model.OfficalEmailId).Select(a => a.OfficalEmailId).FirstOrDefault();
                string lEmpId = db.Employes.Where(a => a.EmpId == model.OfficalEmailId).Select(a => a.EmpId).FirstOrDefault();

                string lrdate = db.Employes.Where(a => a.EmpId == luser).Select(a => a.RetirementDate.ToString()).FirstOrDefault();
                DateTime lrdate1 = Convert.ToDateTime(lrdate);

                string dateforView = lrdate1.ToString("dd-MM-yyyy");
                if (Convert.ToDateTime(dateforView) < DateTime.Now.Date && lrdate != null)
                {
                    TempData["status"] = "Sorry, your Account is inactive Please Contact administrator";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
    
                    userid = "Email Id/Emp Code : {0} {1}\n";
                    pwd = "Password : {2}";
                    var body = "Password information  \n" + " " + userid + " " + pwd;
                    var message = new MailMessage();
                    if (lAdmin == model.OfficalEmailId)
                    {

                        lUsername = model.OfficalEmailId;
                        message.To.Add(new MailAddress(model.OfficalEmailId));
                    }
                    else
                    {
                        lEmpId = model.OfficalEmailId;
                        string EmployeeEmail = db.Employes.Where(a => a.EmpId == lEmpId).Select(a => a.OfficalEmailId).FirstOrDefault();
                        string lEmails = EmployeeEmail;
                        if (lEmails != null)
                        {
                            message.To.Add(new MailAddress(lEmails));
                        }
                        else
                        {
                            string Peremail = model.OfficalEmailId;

                            if (Peremail.Contains("com") == true || Peremail.Contains("COM") == true)
                            {
                                ViewBag.Message = "Email Id  Could not be found";

                            }
                            else
                            {
                                ViewBag.Message = "Emp Code Could not be found";

                            }
                            return View(model);
                        }
                    }
                    message.From = new MailAddress("info@mavensoft.org");
                    message.Subject = "Your Password information for TSCAB-HRMS Application";
                    string lresult = string.Empty;
                    string EmailId = "";
                    if (lUsername == "Employee" || lEmpId != null)
                    {

                        lresult = model.OfficalEmailId;
                        EmailId = model.OfficalEmailId;
                        string lRetrievedPassword = db.Employes.Where(a => a.EmpId == model.OfficalEmailId).Select(a => a.Password).FirstOrDefault();
                        var lpassword = lencrptypassword.Decrypt(lRetrievedPassword);
                        model.Password = lpassword;
                    }
                    else
                    {

                        lresult = model.OfficalEmailId;
                        EmailId = model.OfficalEmailId;
                        string lRetrievedPassword = db.Employes.Where(a => a.OfficalEmailId == model.OfficalEmailId).Select(a => a.Password).FirstOrDefault();
                        var lpassword = lencrptypassword.Decrypt(lRetrievedPassword);
                        model.Password = lpassword;
                    }
                    if (lresult != "")
                    {
                        if (lUsername != null)
                        {

                            message.Body = string.Format(body, "", EmailId, model.Password);
                        }
                        else
                        {

                            message.Body = string.Format(body, "", EmailId, model.Password);
                        }

                        lEmpId = model.OfficalEmailId;
                        string EmployeeEmail = db.Employes.Where(a => a.EmpId == lEmpId).Select(a => a.OfficalEmailId).FirstOrDefault();
                        string lEmails = EmployeeEmail;
                        message.IsBodyHtml = true;
                        const string SERVER = "relay-hosting.secureserver.net";
                        message.From = new MailAddress("info@mavensoft.org");
                        message.To.Add(EmployeeEmail);
                        SmtpClient SmtpMail = new SmtpClient(SERVER);
                        SmtpMail.Send(message);
                        ViewBag.Message = "Password will be sent to your Email";

                    }
                    else
                    {
                        ViewBag.Message = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }
        public ActionResult Error()
        {
            return View();
        }

        [HttpGet]
        [SessionTimeoutAttribute]
        public ActionResult Logout()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var name = this.Session["EmpShortName"];
            FormsAuthentication.SignOut();
            LogInformation.Info(name + " Logout Successfully ");

            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            // LogInformation.Info("Logout" + lResult.ShortName + "Successfully Loggedin");
            return RedirectToAction("Index", "Home");
        }

        [NoDirectAccess]
        [HttpGet]
        [SessionTimeoutAttribute]
        public ActionResult Terms()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            Employees lmodel = new Employees();
            lmodel.LoginMode = lCredentails.LoginMode;
            TempData["Loginmode"] = lCredentails.LoginMode;
            return View(lmodel);
        }

        [NoDirectAccess]
        [HttpGet]
        [SessionTimeoutAttribute]
        public ActionResult Help()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            Employees lmodel = new Employees();
            lmodel.LoginMode = lCredentails.LoginMode;
            TempData["Loginmode"] = lCredentails.LoginMode;
            return View(lmodel);
        }
    }
}
