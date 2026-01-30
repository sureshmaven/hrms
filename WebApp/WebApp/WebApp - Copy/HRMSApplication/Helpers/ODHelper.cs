using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HRMSBusiness.Db;
namespace HRMSApplication.Helpers
{
    public class ODHelper
    {
        SqlHelper sh = new SqlHelper();
        public string CheckHoliday(DateTime ED)
        {
            SqlHelper sh = new SqlHelper();
            // string date2 = date1.ToShortDateString();

            string Day = ED.Day.ToString();
            string Month = ED.Month.ToString();
            string Year = ED.Year.ToString();
            string qry1 = " select Occasion, DATEPART(DAY, Date) as Day from HolidayList where DATEPART(DAY, Date) =  '" + Day + " '  and DATEPART(MONTH, Date) = '" + Month + " '  and DATEPART(YEAR, Date)= '" + Year + " ' ";
            DataTable dtbl = sh.Get_Table_FromQry(qry1);
            string status = "";
            if (dtbl.Rows.Count > 0)
            {
                status = "Holiday";
            }
            return status;
        }
        
        public string CheckUnlockOd(string EmpId,DateTime SD,DateTime Ed)
        {
            SqlHelper sh = new SqlHelper();
            // string date2 = date1.ToShortDateString();

           
            string qry1 = " select case  when exists" + "(select * from OD_Unlock where EmpId ='"+EmpId+"' and '"+ SD + "' between OD_Unlock.FromDate and OD_Unlock.ToDate)then 1 else 0 end as status ";

            DataTable dtbl = sh.Get_Table_FromQry(qry1);
            string status = "";
            if (dtbl.Rows.Count > 0)
            {
                status = dtbl.Rows[0]["status"].ToString();
                if(status!="0")
                {
                    
                    string qry2 = " select case  when exists" + "(select * from OD_Unlock where EmpId ='" + EmpId + "' and '" + Ed + "' between OD_Unlock.FromDate and OD_Unlock.ToDate)then 1 else 0 end as status ";
                    DataTable dtb2 = sh.Get_Table_FromQry(qry2);
                    if (dtb2.Rows.Count > 0)
                    {
                        status = dtb2.Rows[0]["status"].ToString();
                    }
                }
                
             }
            return status;
        }
        public string SendEmails(DateTime StartDate, DateTime EnDate, int lControllingId, int lSancationingId, int lempId, int lVistorFrom, string VistorTo,int Purpose, string Description, string Status, string AppliedValue)
        {
            String lMessage = string.Empty;
            try
            {
                ContextBase db = new ContextBase();
                string lControllingEmail = db.Employes.Where(a => a.Id == lControllingId).Select(a => a.OfficalEmailId).FirstOrDefault();
                string lSancatiningEmail = db.Employes.Where(a => a.Id == lSancationingId).Select(a => a.OfficalEmailId).FirstOrDefault();
                string lEmployeeEmail = db.Employes.Where(a => a.Id == lempId).Select(a => a.OfficalEmailId).FirstOrDefault();
                string lEmpCode = db.Employes.Where(a => a.Id == lempId).Select(a => a.EmpId).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.Id == lempId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.Id == lempId).Select(a => a.LastName).FirstOrDefault();
                string lVistingFrom = db.Branches.Where(a => a.Id ==lVistorFrom).Select(a => a.Name).FirstOrDefault();
                string lEmployeeFullName = lFirstName + " " + lLastName;
                string lPurpose = db.OD_Master.Where(a => a.Id == Purpose).Select(a => a.ODType).FirstOrDefault();
                if (Status == "Leave" && AppliedValue == "0")
                {
                    //Emp ,Controlling

                    // string lSubject = "Leave Applied Details:";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A OD Request Sent From ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Visiting From: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lVistingFrom);
                    lb.Append(" to ");
                    lb.Append(VistorTo);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Purpose: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lPurpose);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Description);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "OD Applied";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lControllingEmail);
                    //message.To.Add(lEmployeeEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lControllingEmail);
                    message.To.Add(lEmployeeEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    LogInformation.Info("Email Sent");
                }
                else if (Status == "Forwarded" && AppliedValue == "0") // controlling authority forward message
                {
                    //sancationing email

                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A OD Request Forwarded for Sanctioning Authority</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A OD Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Visiting From: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lVistingFrom);
                    lb.Append(" to ");
                    lb.Append(VistorTo);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Purpose: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lPurpose);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Description);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "OD Forwarded";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lControllingEmail);
                    //message.To.Add(lSancatiningEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/
                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lSancatiningEmail);
                    message.To.Add(lControllingEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    LogInformation.Info("Email Sent");
                }
                else if (Status == "Approved" && AppliedValue == "1")
                {
                    // Emp, Sancationing

                    //  string lSubject = "Approved";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A OD Request Approved</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A OD Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Visiting From: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lVistingFrom);
                    lb.Append(" to ");
                    lb.Append(VistorTo);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Purpose: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lPurpose);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Description);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "OD Approved";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lEmployeeEmail);
                    //message.To.Add(lSancatiningEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lEmployeeEmail);
                    message.To.Add(lSancatiningEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    LogInformation.Info("Email Sent");
                }
                else if (Status == "Cancelled" && AppliedValue == "0") // controlling authority cancel
                {
                    // Emp, controlling

                    //  string lSubject = " Cancelled ";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A OD Request Cancelled from Controlling Authority</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A OD Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Visiting From: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lVistorFrom);
                    lb.Append(" to ");
                    lb.Append(VistorTo);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Purpose: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lPurpose);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Description);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "OD Cancelled";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lEmployeeEmail);
                    //message.To.Add(lControllingEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lEmployeeEmail);
                    message.To.Add(lControllingEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    LogInformation.Info("Email Sent");
                }
                else if (Status == "Cancelled" && AppliedValue == "1") // sancationing authority cancel
                {
                    // Emp, Sancationing

                    // string lSubject = " Cancelled ";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A OD Request Cancelled from Sancationing Authority</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A OD Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Visiting From: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lVistingFrom);
                    lb.Append(" to ");
                    lb.Append(VistorTo);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Purpose: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lPurpose);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Description);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "OD Cancelled";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lEmployeeEmail);
                    //message.To.Add(lSancatiningEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lEmployeeEmail);
                    message.To.Add(lSancatiningEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    LogInformation.Info("Email Sent");
                }
                else if (Status == "Denied" && AppliedValue == "0") // controlling authority cancel
                {
                    // Emp, controlling

                    //  string lSubject = " Cancelled ";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A OD Request Denied from Controlling Authority</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A OD Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Visiting From: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lVistorFrom);
                    lb.Append(" to ");
                    lb.Append(VistorTo);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Purpose: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lPurpose);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Description);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "OD Denied";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lEmployeeEmail);
                    //message.To.Add(lControllingEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lEmployeeEmail);
                    message.To.Add(lControllingEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    LogInformation.Info("Email Sent");
                }
                else if (Status == "Denied" && AppliedValue == "1") // sancationing authority cancel
                {
                    // Emp, Sancationing

                    // string lSubject = " Cancelled ";
                    StringBuilder lb = new StringBuilder();
                    lb.Append("<p><strong>A OD Request Denied from Sancationing Authority</strong></p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>A OD Request has come from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lEmployeeFullName);
                    lb.Append("</span>");
                    lb.Append(" Starting from ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(StartDate.ToString("dd-MMM-yyyy"));
                    lb.Append(" to ");
                    lb.Append(EnDate.ToString("dd-MMM-yyyy"));
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Visiting From: ");
                    lb.Append("<span style='color: blue; font-weight:bold;'>");
                    lb.Append(lVistingFrom);
                    lb.Append(" to ");
                    lb.Append(VistorTo);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>Purpose: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(lPurpose);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p style='font-size:12px; font-weight:normal;'>The following reason is specified: ");
                    lb.Append("<span style='color: blue;'>");
                    lb.Append(Description);
                    lb.Append("</span>");
                    lb.Append("</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>&nbsp;</p>");
                    lb.Append("<p>************ This mail is Generated from 'TSCAB-HRMS' ************</p>");
                    var body = lb.ToString();
                    MailMessage message = new MailMessage();
                    message.Subject = "OD Denied";
                    message.Body = string.Format(body);
                    message.IsBodyHtml = true;

                    //----For Local Mails------//

                    //SmtpClient client = new SmtpClient();
                    //message.From = new MailAddress("mavensoftspl@gmail.com");
                    //client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                    //message.To.Add(lEmployeeEmail);
                    //message.To.Add(lSancatiningEmail);
                    //client.Port = 587;
                    //client.Host = "smtp.gmail.com";
                    //client.EnableSsl = true;
                    //client.Send(message);

                    /*--------- For Production---------*/

                    const string SERVER = "relay-hosting.secureserver.net";
                    message.From = new MailAddress("info@mavensoft.org");
                    message.To.Add(lEmployeeEmail);
                    message.To.Add(lSancatiningEmail);
                    SmtpClient SmtpMail = new SmtpClient(SERVER);
                    SmtpMail.SendMailAsync(message);
                    LogInformation.Info("Email Sent");
                }

            }
            catch (Exception ex)
            {
                LogInformation.Info("Official Emailid is Empty");
                lMessage = ex.ToString();
            }

            return lMessage;
        }

        public string SendSms1(DateTime StartDate, DateTime EnDate, int lControllingId, int lSancationingId, int lempId, int lVistorFrom, string VistorTo, int Purpose, string Status, string AppliedValue)
        {

            if (System.Configuration.ConfigurationManager.AppSettings["sendSMS"] == "false")
            {
                return "SMS message is not sending. Please check configurations";
            }
            Stream os = null;
            String lMessage = string.Empty;
            try
            {
                ContextBase db = new ContextBase();
                string lControllingmobileno = db.Employes.Where(a => a.Id == lControllingId).Select(a => a.MobileNumber).FirstOrDefault();
                string lSancatiningmobileno = db.Employes.Where(a => a.Id == lSancationingId).Select(a => a.MobileNumber).FirstOrDefault();
                string lEmployeemobileno = db.Employes.Where(a => a.Id == lempId).Select(a => a.MobileNumber).FirstOrDefault();
                string lEmpCode = db.Employes.Where(a => a.Id == lempId).Select(a => a.EmpId).FirstOrDefault();
                string lFirstName = db.Employes.Where(a => a.Id == lempId).Select(a => a.FirstName).FirstOrDefault();
                string lLastName = db.Employes.Where(a => a.Id == lempId).Select(a => a.LastName).FirstOrDefault();
                string lVistingFrom = db.Branches.Where(a => a.Id == lVistorFrom).Select(a => a.Name).FirstOrDefault();
                string lPurpose = db.OD_Master.Where(a => a.Id == Purpose).Select(a => a.ODType).FirstOrDefault();
                string lEmployeeFullName = lFirstName + " " + lLastName;
                string lStartDate = StartDate.ToString("dd-MMM-yyyy");
                string lEnDate = EnDate.ToString("dd-MMM-yyyy");
                string countrycode = "91";
                if (Status == "HRMSOD" && AppliedValue == "0")
                {
                    //Emp ,Controlling

                    string lcontent = "Leave Request sent from " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate + "  Visting From " + lVistingFrom + " to " + VistorTo + " Purpose " + lPurpose +  "     Thank you TSCAB-HRMS ";
                    string lcontrolling = countrycode + "" + lControllingmobileno;
                    string lmobilenumber =lcontrolling;
                   // String URI = "http://push.vg4mobile.com/newBulkClient.jsp?senderID=VGTHYD&msisdn=" + lmobilenumber + "&userid=709&msg=" + lcontent + "&pwd=vgthyd1234";
                    String URI = "http://bulkpush.mytoday.com/BulkSms/SingleMsgApi?feedid=351607&username=8686593293&pwd=Tscabaj!1234&time=&msisdn=" + lmobilenumber + "&Text=" + lcontent;
                    try
                    {
                        WebRequest webRequest = WebRequest.Create(URI);
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                        webRequest.Method = "POST";
                        //webRequest.Timeout = 60000; //Put min 1 minutes   
                        byte[] bytes = Encoding.ASCII.GetBytes(URI);

                        webRequest.ContentLength = bytes.Length;   //Count bytes to send     
                        os = webRequest.GetRequestStream();
                        os.Write(bytes, 0, bytes.Length);
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response is HttpWebResponse)
                        {
                            switch (((HttpWebResponse)ex.Response).StatusCode)
                            {
                                case HttpStatusCode.NotFound:
                                    break;

                                default:
                                    throw ex;
                            }
                        }
                    }

                }
                //else if (Status == "Forwarded" && AppliedValue == "0") // controlling authority forward message
                //{
                //    //sancationing email
                //    string lcontent = "OD Request Forwarded for Sanctioning Authority  " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate + "Visting From:" + lVistingFrom + "to " + VistorTo + "Purpose:" + lPurpose + "     Thank you TSCAB-HRMS ";
                //    string lmobilebo = countrycode + "" + lSancatiningmobileno;
                //    String URI = "http://push.vg4mobile.com/newBulkClient.jsp?senderID=VGTHYD&msisdn=" + lmobilebo + "&userid=709&msg=" + lcontent + "&pwd=vgthyd1234";
                //    try
                //    {
                //        WebRequest webRequest = WebRequest.Create(URI);
                //        webRequest.ContentType = "application/x-www-form-urlencoded";
                //        webRequest.Method = "POST";
                //        //webRequest.Timeout = 60000; //Put min 1 minutes   
                //        byte[] bytes = Encoding.ASCII.GetBytes(URI);

                //        webRequest.ContentLength = bytes.Length;   //Count bytes to send     
                //        os = webRequest.GetRequestStream();
                //        os.Write(bytes, 0, bytes.Length);
                //    }
                //    catch (WebException ex)
                //    {
                //        if (ex.Response is HttpWebResponse)
                //        {
                //            switch (((HttpWebResponse)ex.Response).StatusCode)
                //            {
                //                case HttpStatusCode.NotFound:
                //                    break;

                //                default:
                //                    throw ex;
                //            }
                //        }
                //    }
                //}
                else if (Status == "Approved" && AppliedValue == "1")
                {
                    // Emp, Sancationing
                    string lcontent = "OD Request Approved for  " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate + "Visting From:" + lVistingFrom + "to " + VistorTo + "Purpose:" + lPurpose + "     Thank you TSCAB-HRMS ";
                    string lEmployee = countrycode + "" + lEmployeemobileno;
                    string lmobilebo = lEmployee;
                    //  String URI = "http://push.vg4mobile.com/newBulkClient.jsp?senderID=VGTHYD&msisdn=" + lmobilebo + "&userid=709&msg=" + lcontent + "&pwd=vgthyd1234";
                    String URI = "http://bulkpush.mytoday.com/BulkSms/SingleMsgApi?feedid=351607&username=8686593293&pwd=Tscabaj!1234&time=&msisdn=" + lmobilebo + "&Text=" + lcontent;
                    try
                    {
                        WebRequest webRequest = WebRequest.Create(URI);
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                        webRequest.Method = "POST";
                        //webRequest.Timeout = 60000; //Put min 1 minutes   
                        byte[] bytes = Encoding.ASCII.GetBytes(URI);

                        webRequest.ContentLength = bytes.Length;   //Count bytes to send     
                        os = webRequest.GetRequestStream();
                        os.Write(bytes, 0, bytes.Length);
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response is HttpWebResponse)
                        {
                            switch (((HttpWebResponse)ex.Response).StatusCode)
                            {
                                case HttpStatusCode.NotFound:
                                    break;

                                default:
                                    throw ex;
                            }
                        }
                    }
                }
                else if (Status == "Cancelled" && AppliedValue == "0") // controlling authority cancel
                {
                    // Emp, controlling
                    string lcontent = "OD Request Cancelled from Controlling Authority   " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate + "Visting From:" + lVistingFrom + "to " + VistorTo + "Purpose:" + lPurpose + "     Thank you TSCAB-HRMS ";
                    string lEmployee = countrycode + "" + lEmployeemobileno;
                    string lmobilebo = lEmployee;
                   // String URI = "http://push.vg4mobile.com/newBulkClient.jsp?senderID=VGTHYD&msisdn=" + lmobilebo + "&userid=709&msg=" + lcontent + "&pwd=vgthyd1234";
                    String URI = "http://bulkpush.mytoday.com/BulkSms/SingleMsgApi?feedid=351607&username=8686593293&pwd=Tscabaj!1234&time=&msisdn=" + lmobilebo + "&Text=" + lcontent;
                    try
                    {
                        WebRequest webRequest = WebRequest.Create(URI);
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                        webRequest.Method = "POST";
                        //webRequest.Timeout = 60000; //Put min 1 minutes   
                        byte[] bytes = Encoding.ASCII.GetBytes(URI);

                        webRequest.ContentLength = bytes.Length;   //Count bytes to send     
                        os = webRequest.GetRequestStream();
                        os.Write(bytes, 0, bytes.Length);
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response is HttpWebResponse)
                        {
                            switch (((HttpWebResponse)ex.Response).StatusCode)
                            {
                                case HttpStatusCode.NotFound:
                                    break;

                                default:
                                    throw ex;
                            }
                        }
                    }
                }
                else if (Status == "Cancelled" && AppliedValue == "1") // sancationing authority cancel
                {
                    // Emp, Sancationing
                    string lcontent = "OD Request Cancelled from Sancationing Authority   " + lEmployeeFullName + " " + (lEmpCode) + " Starting from " + lStartDate + " to " + lEnDate + "Visting From:" + lVistingFrom + "to " + VistorTo + "Purpose:" + lPurpose + "     Thank you TSCAB-HRMS ";
                    string lEmployee = countrycode + "" + lEmployeemobileno;
                    string lmobilebo = lEmployee;
                    //   String URI = "http://push.vg4mobile.com/newBulkClient.jsp?senderID=VGTHYD&msisdn=" + lmobilebo + "&userid=709&msg=" + lcontent + "&pwd=vgthyd1234";
                    String URI = "http://bulkpush.mytoday.com/BulkSms/SingleMsgApi?feedid=351607&username=8686593293&pwd=Tscabaj!1234&time=&msisdn=" + lmobilebo + "&Text=" + lcontent;
                    try
                    {
                        WebRequest webRequest = WebRequest.Create(URI);
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                        webRequest.Method = "POST";
                        //webRequest.Timeout = 60000; //Put min 1 minutes   
                        byte[] bytes = Encoding.ASCII.GetBytes(URI);

                        webRequest.ContentLength = bytes.Length;   //Count bytes to send     
                        os = webRequest.GetRequestStream();
                        os.Write(bytes, 0, bytes.Length);
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response is HttpWebResponse)
                        {
                            switch (((HttpWebResponse)ex.Response).StatusCode)
                            {
                                case HttpStatusCode.NotFound:
                                    break;

                                default:
                                    throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lMessage = ex.Message;
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }
            return lMessage;

        }
    }
}