using HRMSApplication.Models;
using HRMSBusiness.Business;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace HRMSApplication.Helpers
{
    public class PayrollHelper
    {
       
   
        public DateTime GetCurrentTime(DateTime ldate)
        {
            DateTime serverTime = DateTime.Now;
            DateTime utcTime = serverTime.ToUniversalTime();
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }

    }
}