using System;

namespace Mavensoft.Common
{
    public class TypeFormat
    {
        public static string DbDateToStringDate(object input)
        {
            if (input is DBNull)
                return "";
            else 
                return DateTime.Parse(input.ToString()).ToString("yyyy-MM-dd");
        }

        public static int DbIntToInteger(object input)
        {
            if (input is DBNull)
                return 0;
            else
                return int.Parse(input.ToString());
        }

        public static char DbRowNewOrOld(object input)
        {
            if (input is DBNull)
                return 'N';
            else
                return 'O';
        }
    }
}
