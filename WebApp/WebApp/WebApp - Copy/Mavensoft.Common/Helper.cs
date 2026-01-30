using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mavensoft.Common
{
    public class Helper
    {
        public static int getFinancialYear(DateTime dt)
        {
            int iRet = 0;
            if (dt.Month == 1 || dt.Month == 2 || dt.Month == 3)
                iRet = dt.Year;
            else
                iRet = dt.Year + 1;
            return iRet;
        }

        public static int findLastDayOfMonth(DateTime dt)
        {
            var nextmonth = dt.AddMonths(1);
            return new DateTime(nextmonth.Year, nextmonth.Month, 1).AddDays(-1).Day;
        }

        public static string NumbersToWords(int inputNumber)
        {
            int inputNo = inputNumber;

            if (inputNo == 0)
                return "Zero";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
        //new and old data table collaboration
        public static string getDetesForFromDateToDate(int fy,string SearchDatafrom,string SearchDatato)
        {
           
            DateTime startFy = Convert.ToDateTime(fy - 1 + "-04-01");
            DateTime endFy = Convert.ToDateTime(fy + "-03-30");
            

            string strDateSend = "";
           
            DateTime from = Convert.ToDateTime(SearchDatafrom);
            DateTime to = Convert.ToDateTime(SearchDatato);
            //to check whether given dates within the finacial year or not
            if ((from >= startFy && from <= endFy) && (to >= startFy && to <= endFy)) {
                strDateSend = "New#" + SearchDatafrom + "#" + SearchDatato + "";
            }
            //to check whether given dates within the finacial year and old financial year or not
            else if (from <startFy && to > startFy)
            {
                strDateSend = "Old#" + SearchDatafrom + "#" + startFy + "#New#" + startFy + "#" + endFy + "";
            }
            //to check whether given dates completely within old financial year or not
            else if (from < startFy && to < startFy)
            {
                strDateSend = "Old#" + SearchDatafrom + "#" + SearchDatato + "";
            }
            else
            {
                strDateSend = "New#" + SearchDatafrom + "#" + SearchDatato + "";
            }
            
            return strDateSend;
        
}

        public string getPresentTable(int fy,string oldtTable, string presentTable, string totalDate)
        {
            int pyear = Convert.ToInt32((Convert.ToDateTime(totalDate)).Year);
            int pmonth = Convert.ToInt32((Convert.ToDateTime(totalDate)).Month);

            int diffYear = fy - pyear;
            if (diffYear > 1 && pmonth <= 3)
            {
                return oldtTable;
            }
            else
            {
                return presentTable;
            }

        }
        //new and old data table collaboration without joins
        public static string getDetesForFromDateToDateWithoutJoin(int fy, string SearchDatafrom, 
            string SearchDatato, string qry,string orgTable,string oldTable)
        {

            DateTime startFy = Convert.ToDateTime(fy - 1 + "-04-01");
            DateTime endFy = Convert.ToDateTime(fy + "-03-30");


            string strDateSendQry = "";

            DateTime from = Convert.ToDateTime(SearchDatafrom);
            DateTime to = Convert.ToDateTime(SearchDatato);
            //to check whether given dates within the finacial year or not
            if ((from >= startFy && from <= endFy) && (to >= startFy && to <= endFy))
            {
                //strDateSendQry = "New#" + SearchDatafrom + "#" + SearchDatato + "";
                strDateSendQry = qry + " '" + from.ToString("yyyy-MM-dd") + "' and '" + to.ToString("yyyy-MM-dd") + "'";


            }
            //to check whether given dates within the finacial year and old financial year or not
            else if (from < startFy && to > startFy)
            {
                //strDateSendQry = "Old#" + SearchDatafrom + "#" + startFy + "#New#" + startFy + "#" + endFy + "";
                //string qry1 = qry.Replace(orgTable, oldTable);
                //string Result = qry.Replace("World", "stackoverflow");
                //string Result = qry.Replace("World", "stackoverflow");
                strDateSendQry = qry + " '" + startFy + "' and '" + endFy + "' union all " + qry.Replace(orgTable, oldTable)+ " '" + from.ToString("yyyy-MM-dd") + "' and '" + startFy + "'";
                
            }
            //to check whether given dates completely within old financial year or not
            else if (from < startFy && to < startFy)
            {
                //strDateSendQry = "Old#" + SearchDatafrom + "#" + SearchDatato + "";
                strDateSendQry = qry.Replace(orgTable, oldTable) + " '" + from.ToString("yyyy-MM-dd") + "' and '" + to.ToString("yyyy-MM-dd") + "'";
            }
            else
            {
                //strDateSendQry = "New#" + SearchDatafrom + "#" + SearchDatato + "";
                strDateSendQry = qry + " '" + from.ToString("yyyy-MM-dd") + "' and '" + to.ToString("yyyy-MM-dd") + "'";
            }

            return strDateSendQry;

        }

        public static string getDetesForFromDateToDateWithJoins(int fy, string SearchDatafrom,
            string SearchDatato, string qry, string[] oldTables)
        {

            DateTime startFy = Convert.ToDateTime(fy - 1 + "-04-01");
            DateTime endFy = Convert.ToDateTime(fy + "-03-30");
            string oldQry = qry;
            
            string strDateSendQry = "";
            
            DateTime from = Convert.ToDateTime(SearchDatafrom);
            DateTime to = Convert.ToDateTime(SearchDatato);
            //to check whether given dates within the finacial year or not
            if ((from >= startFy && from <= endFy) && (to >= startFy && to <= endFy))
            {
                
                //strDateSendQry = "New#" + SearchDatafrom + "#" + SearchDatato + "";
                strDateSendQry = qry + " '" + from.ToString("yyyy-MM-dd") + "' and '" + to.ToString("yyyy-MM-dd") + "'";


            }
            //to check whether given dates within the finacial year and old financial year or not
            else if (from < startFy && to > startFy)
            {
                foreach (string s in oldTables)
                {
                    if (qry.Contains(s))
                    {
                        string replaceStr = "old_" + s;
                        oldQry = oldQry.Replace(s, replaceStr);

                    }
                }

                strDateSendQry = qry + " '" + startFy + "' and '" + endFy + "' union all " + oldQry + " '" + from.ToString("yyyy-MM-dd") + "' and '" + startFy + "'";

            }
            //to check whether given dates completely within old financial year or not
            else if (from < startFy && to < startFy)
            {
                foreach (string s in oldTables)
                {
                    if (qry.Contains(s))
                    {
                        string replaceStr = "old_" + s;
                        oldQry = oldQry.Replace(s, replaceStr);

                    }
                }
                //strDateSendQry = "Old#" + SearchDatafrom + "#" + SearchDatato + "";
                strDateSendQry = oldQry + " '" + from.ToString("yyyy-MM-dd") + "' and '" + to.ToString("yyyy-MM-dd") + "'";
            }
            else
            {
                //strDateSendQry = "New#" + SearchDatafrom + "#" + SearchDatato + "";
                strDateSendQry = qry + " '" + from.ToString("yyyy-MM-dd") + "' and '" + to.ToString("yyyy-MM-dd") + "'";
            }

            return strDateSendQry;

        }
    }
}
