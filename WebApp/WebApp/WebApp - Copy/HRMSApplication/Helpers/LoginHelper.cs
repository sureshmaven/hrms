using Entities;
using HRMSApplication.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using static HRMSBusiness.Business.LoginBus;

namespace HRMSApplication.Helpers
{
    public class LoginHelper
    {
        private static readonly string gAuthenticCookieName = ((System.Web.Configuration.AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication")).Forms.Name;

        public static void Logout()
        {
            FormsAuthentication.SignOut();

            if (HttpContext.Current.Request.Cookies[gAuthenticCookieName] != null)
            {
                HttpContext.Current.Request.Cookies[gAuthenticCookieName].Expires = DateTime.Now.AddYears(-30);

                HttpContext.Current.Request.Cookies.Clear();
            }
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
  public static PayrollModels.LoginCredential GetCurrentUserForPR()
        {
            //SqlHelper sh = new SqlHelper();
            string Version = System.Configuration.ConfigurationManager.AppSettings["PayrollVersion"];
            string AppEnvironment = System.Configuration.ConfigurationManager.AppSettings["AppEnvironment"];
            //string query = "select fy, fm from pr_month_details where active=1";
            //DataTable dt = sh.Get_Table_FromQry(query);
            //string fm = Convert.ToDateTime(dt.Rows[0]["fm"]).ToString("yyyy-MMM-dd");
            //string[] sa = fm.Split('-');
            //string dtFM = sa[1];
            //string yr = sa[0];
            int fy = 0;
            int fm = 0;
            DateTime dtFm = DateTime.Now;
           
            try
            {
                fy = int.Parse( HttpContext.Current.Session["FY"].ToString());
                dtFm = DateTime.Parse(  HttpContext.Current.Session["FM"].ToString());
                fm = dtFm.Month;
            }
            catch { }

            LoginCredential hrmsUser = GetCurrentUser();
            PayrollModels.LoginCredential prUser = new PayrollModels.LoginCredential
            {
                EmpCode = int.Parse(hrmsUser.EmpId),
                EmpShortName = (hrmsUser.EmpShortName.Length > 30 ? hrmsUser.EmpShortName.Substring(0, 30) : hrmsUser.EmpShortName),
                BranchCode = hrmsUser.Branch,
                BranchName = hrmsUser.BranchName,
                AppName = "Payroll_Web",
                AppVersion = Version, //read from web.config
                AppEnvironment = AppEnvironment,
                FY = fy,
                FM = fm,
                FinancialMonthDate = dtFm
            };
            return prUser;
        }

        public static string GetCurrentUserPages()
        {
            return HttpContext.Current.Session["RolePages"].ToString();
        }

            public static LoginCredential GetCurrentUser()
        {
            string lm = "";
            LoginCredential lCredentials = new LoginCredential();
            try
            {
                if (HttpContext.Current.Session["EmpId"] != null)
                {
                    lCredentials.EmpId = HttpContext.Current.Session["EmpId"].ToString();
                    lCredentials.LoginMode = HttpContext.Current.Session["LogInMode"].ToString();
                    lCredentials.EmpPkId = HttpContext.Current.Session["EmpPkId"].ToString();
                    lCredentials.CurrDesig = HttpContext.Current.Session["CurrDesig"].ToString();
                    lCredentials.Branch = HttpContext.Current.Session["Branch"].ToString();
                    lCredentials.BranchName = HttpContext.Current.Session["BranchName"].ToString();
                    lCredentials.Department = HttpContext.Current.Session["Department"].ToString();
                    lCredentials.Designation = HttpContext.Current.Session["Designation"].ToString();
                    lCredentials.EmpFullName = HttpContext.Current.Session["EmpFullName"].ToString();
                    lCredentials.EmpShortName = HttpContext.Current.Session["EmpShortName"].ToString();
                    lCredentials.Role = HttpContext.Current.Session["Role"].ToString();
                    lCredentials.Approvals = HttpContext.Current.Session["Approvals"].ToString();
                    lCredentials.RolePages = HttpContext.Current.Session["RolePages"].ToString();
                    string EmpImage = HttpContext.Current.Session["EmpImage"].ToString();
                    if (EmpImage != null)
                    {
                        lCredentials.EmpImage = EmpImage;
                    }
                    else
                    {
                        lCredentials.EmpImage = "m.png";
                    }
                }
            }
            catch (Exception ex)
            {
                lm = ex.ToString();
            }
            return lCredentials;
        }
    }
}