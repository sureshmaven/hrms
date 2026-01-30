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
using System.Diagnostics;
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
            //string captchaText = GenerateCaptcha();
            //TempData["CaptchaText"] = captchaText;  // Store CAPTCHA in session
            //LogInformation.Info("Stored CAPTCHA: " + TempData["CaptchaText"]);
            //ViewBag.Captcha = captchaText;  // Send CAPTCHA to view for display
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
            try
            { //bool isLocked = /* Logic to check if the user is locked out */;
                DateTime? lockEndTime = db.Employes.Where(e => e.EmpId == Employee.EmpId).Select(e => e.AccountLockedUntil).FirstOrDefault();
                // if (isLocked && lockEndTime.HasValue)
                if (lockEndTime.HasValue && lockEndTime >= DateTime.Now)
                {
                    ViewBag.ShowTimer = true;
                    ViewBag.LockEndTime = lockEndTime.Value;

                }
                else
                {
                    ViewBag.ShowTimer = false;

                    //CAPTCHA validation as before
                    string userCaptcha = Form["captcha"];
                    LogInformation.Info("UserCaptcha " + userCaptcha);
                    //HttpContext.Response.AppendToLog("UserCaptcha" + userCaptcha);
                    string sessionCaptcha = (string)Session["Captcha"];
                    LogInformation.Info("SessionCaptcha " + sessionCaptcha);
                    var dbEmployee = db.Employes.FirstOrDefault(e => e.EmpId == Employee.EmpId);
                    LogInformation.Info("Dbemployee data : " + dbEmployee);
                    if (dbEmployee == null)
                    {
                        TempData["status"] = "Invalid Username or Password";
                        return RedirectToAction("Index", "Home");
                    }

                    if (string.IsNullOrEmpty(sessionCaptcha) || userCaptcha != sessionCaptcha)
                    {

                        dbEmployee.FailedLoginAttempts += 1;
                        if (dbEmployee.FailedLoginAttempts >= 3)
                        {
                            dbEmployee.AccountLockedUntil = DateTime.Now.AddSeconds(180);  // Lock for 30 seconds
                            TempData["ShowTimer"] = true;
                            TempData["LockedUntil"] = dbEmployee.AccountLockedUntil.Value.ToString("o");
                        }
                        else
                        {
                            TempData["status"] = $"Invalid CAPTCHA. You have {3 - dbEmployee.FailedLoginAttempts} attempts left.";
                            TempData["ShowTimer"] = false;
                        }

                        // Save changes to the database
                        db.SaveChanges();

                        //// Redirect back to the login page
                        return RedirectToAction("Index", "Home");
                    }
                    LoginBus lgbus = new LoginBus();
                    string[] LeavesSanctioning = ConfigurationManager.AppSettings["LeavesSanctioning"].Split(',');
                    LoginResult lResult = lgbus.getLoginInformation(Employee.EmpId, Employee.Password);
                    string EmpId = lResult.EmpId;
                    //password encrypt
                    var SHA512UIHashPwd = Form["HiddenPwd"];
                    var EncryptSalt = Employee.branchhide;
                    var AesPassword = db.Employes.Where(e => e.EmpId == Employee.EmpId).Select(e => e.Password).FirstOrDefault();
                    LogInformation.Info($"AesPassword :  { AesPassword }");
                    var AesDecrptPwd = lgbus.Decrypt(AesPassword);
                    LogInformation.Info($"Decrypted Password: {AesDecrptPwd}");
                    var OriginalDbPwd = Encoding.UTF8.GetBytes(AesDecrptPwd + EncryptSalt);
                    var OriginalPWDHash = SHA512Encryption(OriginalDbPwd);
                    string cleanedSHA512UIHashPwd = SHA512UIHashPwd?.Trim().TrimEnd(',');
                    string cleanedOriginalPWDHash = OriginalPWDHash?.Trim();
                    LogInformation.Info($"SHA512UIHashPwd Input Password Hash: {SHA512UIHashPwd}");
                    LogInformation.Info($"cleanedSHA512UIHashPwd Input Password Hash: {cleanedSHA512UIHashPwd}");
                    LogInformation.Info($"cleanedOriginalPWDHash Input Password Hash: {cleanedOriginalPWDHash}");
                    //HttpContext.Response.AppendToLog($"SHA512UIHashPwd Input Password Hash: {SHA512UIHashPwd} " + "; ");
                    //HttpContext.Response.AppendToLog($"Original Password Hash from DB: {OriginalPWDHash}" + "; ");
                    LogInformation.Info($"Original Password Hash from DB: {OriginalPWDHash}");
                    var ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = System.Web.HttpContext.Current.Request.UserHostAddress;
                    }
                    if (dbEmployee.AccountLockedUntil != null && dbEmployee.AccountLockedUntil > DateTime.Now)
                    {
                        // Account is still locked
                        ViewBag.AccountLockedUntil = dbEmployee.AccountLockedUntil.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                        // TempData["status"] = $"Your account is locked until {dbEmployee.AccountLockedUntil.Value:hh:mm tt}. Please try again later.";
                        TempData["ShowTimer"] = true;
                        TempData["LockedUntil"] = dbEmployee.AccountLockedUntil.Value.ToString("o");

                        ///  return RedirectToAction("Index", "Home");
                    }
                    if (cleanedSHA512UIHashPwd == cleanedOriginalPWDHash)
                    {
                        lResult = lgbus.getLoginInformation(Employee.EmpId, AesDecrptPwd);
                        if (lResult.Success == true)
                        {


                            dbEmployee.FailedLoginAttempts = 0;
                            dbEmployee.AccountLockedUntil = null;
                            db.SaveChanges();
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
                            LogInformation.Info("1Loginuser, Successfully " +lEmpId);
                            string lids = Convert.ToString(lEmpId);

                            string lcontrolling = db.Employes.Where(a => a.ControllingAuthority == lids).Select(a => a.ControllingAuthority).FirstOrDefault();
                            LogInformation.Info("2Loginuser, Successfully " + lcontrolling);
                            string lsanction = db.Employes.Where(a => a.SanctioningAuthority == lids).Select(a => a.SanctioningAuthority).FirstOrDefault();
                            LogInformation.Info("3Loginuser, Successfully " + lsanction);
                            this.Session["RolePages"] = lgbus.getUserPages(EmpId, LeavesSanctioning, lResult.Role, lResult.Approvals, lResult.Designation, lcontrolling, lsanction, lResult.Department);
                            string empstring = "";
                            Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
                            string qry = "select constant from all_constants where functionality='LoginAccess' and active=1";
                            DataTable constat = sh.Get_Table_FromQry(qry);
                            LogInformation.Info("4Loginuser, Successfully " + qry);
                            foreach (DataRow dr in constat.Rows)
                            {
                                empstring = dr["constant"].ToString();
                                LogInformation.Info("5Loginuser, Successfully " + empstring);
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
                                    LogInformation.Info("6Loginuser, Successfully " + lEmpId);
                                    return RedirectToAction("EmployeeDashBoard", "Dashboard");
                                }

                                else
                                {
                                    LogInformation.Info("7Loginuser, Successfully " + lEmpId);
                                    return RedirectToAction("Index", "Dashboard");

                                }
                            }
                            else
                            {
                                LogInformation.Info("8Loginuser, Successfully " + lEmpId);
                                return RedirectToAction("EmployeeDashBoard", "Dashboard");
                            }
                        }
                    }
                    else
                    {
                        LogInformation.Info("9Loginuser, Successfully " );
                        // Increment failed login attempts
                        dbEmployee.FailedLoginAttempts += 1;

                        if (dbEmployee.FailedLoginAttempts >= 3) // Lock account after 5 failed attempts
                        {
                            dbEmployee.AccountLockedUntil = DateTime.Now.AddSeconds(180); // Lock for 30 seconds
                                                                                          // Pass a flag to display the timer
                            TempData["ShowTimer"] = true;
                            //TempData["status"] = $"Your account is locked until {dbEmployee.AccountLockedUntil.Value:hh:mm tt}. Please try again later.";
                        }
                        else
                        {
                            TempData["status"] = $"Invalid Username or Password. You have {3 - dbEmployee.FailedLoginAttempts} attempts left.";
                            TempData["ShowTimer"] = false;
                        }
                        LogInformation.Info("10Loginuser, Successfully " );
                        db.SaveChanges();
                        LogInformation.Info("11Loginuser, Successfully ");
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception e)
            {

                LogInformation.Error("Login Error " + e.StackTrace);
            }
           //` LogInformation.Info("Password validated successfully.");
            TempData["status"] = "Invalid Username or Password";
            LogInformation.Info("Invalid userid or password tried to login with this id " + Employee.EmpId);
            return RedirectToAction("Index", "Home");
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
            string luser = form["EmpId"];
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
            }
            string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
            string appPassword = ConfigurationManager.AppSettings["AppPassword"];

            try
            {
                string lUsername = db.Employes.Where(a => a.EmpId == model.EmpId).Select(a => a.LoginMode).FirstOrDefault();
                string lEmail = db.Employes.Where(a => a.EmpId == model.EmpId).Select(a => a.OfficalEmailId).FirstOrDefault();
                if(lEmail == null)
                {
                     lEmail = db.Employes.Where(a => a.EmpId == model.EmpId).Select(a => a.PersonalEmailId).FirstOrDefault();

                }
                string lAdmin = db.Employes.Where(a => a.EmpId == model.EmpId).Select(a => a.EmpId).FirstOrDefault();
                // string lEmpId = db.Employes.Where(a => a.OfficalEmailId == model.OfficalEmailId).Select(a => a.EmpId).FirstOrDefault();
                string lEmpId = luser;
                string lrdate = db.Employes.Where(a => a.EmpId == lEmpId).Select(a => a.RetirementDate.ToString()).FirstOrDefault();
                DateTime lrdate1 = Convert.ToDateTime(lrdate);
                string EmployeeEmail = "";
                string dateforView = lrdate1.ToString("dd-MM-yyyy");
                if (Convert.ToDateTime(dateforView) < DateTime.Now.Date && lrdate != null)
                {
                    TempData["status"] = "Sorry, your Account is inactive Please Contact administrator";
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    userid = "Emp Code :  {1}\n";
                    pwd = "Password : {2}";
                    //var body = "Password information  \n" + " " + userid + " " + pwd;
                    var body = @"
<!DOCTYPE html> 
<html>
<body style='font-family: Arial, sans-serif; color:#333;'>
<div style='border:1px solid #ddd; border-radius:8px; padding:20px; max-width:600px; margin:auto;'>
<h2 style='color:#004080;'>TGCAB – HRMS Portal</h2>
<p>Dear Employee,</p>
<p>Please find your login details for the HRMS system below:</p>
 
    <table style='border-collapse: collapse; width:100%; margin-top:10px;'>
<tr>
<td style='padding:8px; border:1px solid #ddd;'><strong>Password</strong></td>
<td style='padding:8px; border:1px solid #ddd;'>{1}</td>
</tr>
</table>
 
    <p style='margin-top:20px;'>
      You can log in here: 
<a href='https://hrms.tscab.org' style='color:#004080;'>HRMS Login Portal</a>
</p>
 
    <!-- Important Security Note Section -->
<div style='margin-top:20px; padding:12px; border:1px solid #ffe4a3; background:#fff8e6; border-radius:6px;'>
<p style='margin:0; font-size:14px; color:#333;'>
<strong>Important Security Note:</strong><br>
        For your account’s safety, please change your password immediately under the 
        “My Profile” section. We strongly recommend using a unique and secure password 
        to protect your account.
</p>
<p style='margin:10px 0 0 0; font-size:14px; color:#333;'>
        If you experience any difficulty logging in or resetting your password, 
        please contact the HRMS support team for assistance.
</p>
</div>
 
    <p style='margin-top:20px; font-size:14px; color:#333;'>
      Regards,<br>
<strong>TGCAB-HRMS Admin Team</strong>
</p>
 
    <p style='margin-top:20px; font-size:12px; color:#888;'>
<em>This is a system-generated email. Please do not reply.<br>
      For any queries, contact HR Helpdesk at info@tscab.org.</em>
</p>
</div>
</body>
</html>
";

                    var message = new MailMessage();
                    if (lAdmin == model.EmpId)
                    {

                        lUsername = lEmail;
                        message.To.Add(new MailAddress(lEmail));
                    }
                    else
                    {
                        //lEmpId = model.OfficalEmailId;
                         EmployeeEmail = db.Employes.Where(a => a.EmpId == model.EmpId).Select(a => a.OfficalEmailId).FirstOrDefault();
                        if(EmployeeEmail == null)
                        {
                            EmployeeEmail = db.Employes.Where(a => a.EmpId == model.EmpId).Select(a => a.PersonalEmailId).FirstOrDefault();

                        }
                        string lEmails = EmployeeEmail;
                        if (lEmails != null)
                        {
                            message.To.Add(new MailAddress(lEmails));
                        }
                        else
                        {
                            string Peremail = lEmail;

                            if (Peremail.Contains("com") == true || Peremail.Contains("COM") == true || Peremail.Contains("org") == true || Peremail.Contains("ORG") == true)
                            {
                                ViewBag.Status = "If the email address exists in our system, password reset instructions will be sent";

                            }
                            else
                            {
                                ViewBag.Status = "If the email address exists in our system, password reset instructions will be sent";

                            }
                            return View(model);
                        }
                    }
                    
                    message.From = new MailAddress(senderEmail);
                    message.Subject = "Your Password information for TGCAB-HRMS Application";
                    string lresult = string.Empty;
                    string EmailId = "";

                    if (lUsername == "Employee" || lEmpId != null)
                    {

                        lresult = lEmail;
                        EmailId = lEmail;
                        string lRetrievedPassword = db.Employes.Where(a => a.EmpId == model.EmpId).Select(a => a.Password).FirstOrDefault();
                        var lpassword = lencrptypassword.Decrypt(lRetrievedPassword);
                        model.Password = lpassword;
                    }
                    else
                    {

                        lresult = lEmail;
                        EmailId = lEmail;
                        string lRetrievedPassword = db.Employes.Where(a => a.EmpId == model.EmpId).Select(a => a.Password).FirstOrDefault();
                        var lpassword = lencrptypassword.Decrypt(lRetrievedPassword);
                        model.Password = lpassword;
                    }
                    if (lresult != "")
                    {
                        if (lUsername != null)
                        {

                            message.Body = string.Format(body, EmailId, model.Password);
                        }
                        else
                        {

                            message.Body = string.Format(body, EmailId, model.Password);
                        }

                        ////lEmpId = model.OfficalEmailId;
                         EmployeeEmail = db.Employes.Where(a => a.EmpId == lEmpId).Select(a => a.OfficalEmailId).FirstOrDefault();
                        if (EmployeeEmail == null)
                        {
                            EmployeeEmail = db.Employes.Where(a => a.EmpId == lEmpId).Select(a => a.PersonalEmailId).FirstOrDefault();

                        }
                        string lEmails = EmployeeEmail;
                        message.IsBodyHtml = true;
                        message.To.Add(EmployeeEmail);
                        LogInformation.Info("Employee mail" + EmployeeEmail);
                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)) // Gmail SMTP
                        {
                            smtp.Credentials = new NetworkCredential(senderEmail, appPassword);
                            smtp.EnableSsl = true;  // Use TLS
                            smtp.Send(message);
                            LogInformation.Info("Password mail sent");
                            ViewBag.Status = "Password will be sent to your Email";
                        }
                        ///const string SERVER = "relay-hosting.secureserver.net";

                        // SmtpClient SmtpMail = new SmtpClient(SERVER);
                        //SmtpMail.Send(message);
                        

                    }
                    else
                    {
                        ViewBag.Message = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "We couldn't send the password reset email right now. If the issue continues, contact support.";
                LogInformation.Error("We couldn't send the password reset email right now. If the issue continues, contact support.");
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

        private string SHA512Encryption(byte[] saltedValue)
        {
            var HashCode = SHA512.Create().ComputeHash(saltedValue);
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < HashCode.Length; i++)
            {
                sBuilder.Append(HashCode[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
