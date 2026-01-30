using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.Net;
using System.Net.Mail;
using Entities;
using HRMSApplication.Helpers;
using Repository;

namespace HRMSApplication.Models
{
    public class HolidayCode
    {
        private static ContextBase db = new ContextBase();

        /// <summary>
        /// Passing Paramter values like date , dayof week and n Parameter
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dow"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static bool NthDayOfMonth(DateTime date, DayOfWeek dow, int n)
        {
            int d = date.Day;
            return date.DayOfWeek == dow && (d - 1) / 7 == (n - 1);
        }
        /// <summary>
        /// Get All Calendar days
        /// </summary>
        public static void getsundays()
        {
            DateTime startDate = new DateTime(2017, 1, 1);
            DateTime endDate = new DateTime(2019, 12, 31);

            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {
                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {

                    case DayOfWeek.Saturday:

                        if (DayOfWeek.Saturday == testDate.DayOfWeek)
                        {
                            bool SecsatDay = NthDayOfMonth(testDate, DayOfWeek.Saturday, 2);

                            bool FoursatDay = NthDayOfMonth(testDate, DayOfWeek.Saturday, 4);


                            if (SecsatDay == true || FoursatDay == true)
                            {
                                HolidayList holidayListForSat = new HolidayList();
                               
                                holidayListForSat.UpdatedBy = 462;
                                holidayListForSat.UpdateDate = DateTime.Now;
                                DateTime? DeleteAtForSat = Convert.ToDateTime("1900-01-01 00:00:00.000");
                                holidayListForSat.DeleteAt = DeleteAtForSat.Value;
                                holidayListForSat.Date = testDate;
                                if (SecsatDay == true)
                                {
                                    holidayListForSat.Occasion = "Second Saturday";

                                }
                                if (FoursatDay == true)
                                {
                                    holidayListForSat.Occasion = "Fourth Saturday";

                                }

                                db.HolidayList.Add(holidayListForSat);
                                db.SaveChanges();
                            }

                        }
                        break;

                    case DayOfWeek.Sunday:

                        HolidayList holidayList = new HolidayList();

                       
                        holidayList.UpdatedBy = 462;
                        holidayList.UpdateDate = DateTime.Now;
                        DateTime? DeleteAt = Convert.ToDateTime("1900-01-01 00:00:00.000");
                        holidayList.DeleteAt = DeleteAt.Value;
                        holidayList.Date = testDate;
                        holidayList.Occasion = "Sunday";
                        db.HolidayList.Add(holidayList);
                        db.SaveChanges();

                        break;



                }
            }

        }

        public  static void mailcode( string errormsg)
        {

            
  var fromAddress = new MailAddress("mavensoft2015@gmail.com", "Surendra");
            var toAddress = new MailAddress("surilsk4@gmail.com", "Erorr");
            const string fromPassword = "mavensoft2015";
            const string subject = "Subject eror msg";
             string body = errormsg;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }


        }
    }
}