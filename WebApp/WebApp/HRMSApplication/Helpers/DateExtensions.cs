using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMSApplication.Helpers
{
    public static class DateExtensions
    {
        public static string GetSqlDateString(this string shortDate)
        {
            string[] dateSplited = new string[3];
            if(!string.IsNullOrEmpty(shortDate))
            {
                if (shortDate.Contains('/'))
                {
                    dateSplited = shortDate.Split('/');
                }
                else if (shortDate.Contains('-'))
                {
                    dateSplited = shortDate.Split('-');
                }
                string datestr = dateSplited[0];
                string monthstr = dateSplited[1];
                string yearstr = dateSplited[2];
                monthstr = monthstr.CheckLessthan10();
                datestr = datestr.CheckLessthan10();
                string date = yearstr + "-" + monthstr + "-" + datestr;
                return date;
            }
            return "";
           
        }

        private static string CheckLessthan10(this string number)
        {
            int num = Convert.ToInt32(number);
            if (num < 10)
            {
                number = "0" + num;
            }
            return number;
        }
    }

}