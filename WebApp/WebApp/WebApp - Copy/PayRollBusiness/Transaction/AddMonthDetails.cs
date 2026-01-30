using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Mavensoft.DAL;
using Mavensoft.DAL.Db;
using PayrollModels.Transactions;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Mavensoft.Common;
using PayrollModels;

namespace PayRollBusiness.Transaction
{
   public class AddMonthDetails: BusinessBase
    {
        public AddMonthDetails(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        //SqlHelper sh = new SqlHelper();
        SqlHelperAsync _sha = new SqlHelperAsync();

        public async Task<string> InserMonthDetails(CommonPostDTO Values)
        {
            //(string FM, int FY, float WH, float PH, DateTime PD, float DApercent, float DApoints, float DAslabs, string activemonth)
            int NewNumIndex = 0;
            int FY = 0;
            NewNumIndex++;
            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());
            string AlertMessage = "";
            if (Values.Monthdata != null)
            {
                try
                {
                    string[] monthdetails = Values.Monthdata.Split('&');
                    string WH = monthdetails[0];
                    string PH = monthdetails[1];
                    string PD = monthdetails[2];
                    string DAslabs = monthdetails[3];
                    string DApoints = monthdetails[4];
                    string DApercent = monthdetails[5];

                  
                    //2. gen new num
                    //sbqry.Append(GetNewNumStringArr("pr_month_details", NewNumIndex));
                    //string inQry = "Insert into pr_month_details(id,fy,fm,week_holidays,paid_holidays,payment_date,da_slabs,da_points,da_percent)" +
                    //    "values(@idnew" + NewNumIndex + ",'" + FY + "','" + FM + "','" + WH + "','" + PH + "','" + PD + "','" + DAslabs + "','" + DApoints + "','" + DApercent + "')";
                    //sbqry.Append(inQry);
                    ////4. transaction touch
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_month_details", "@idnew" + NewNumIndex, ""));
                    var Fm = "";
                    FY = _LoginCredential.FY;
                    string query = "Select fm from pr_month_details where active=1";
                    DataTable dt = await _sha.Get_Table_FromQry(query);
                    if (dt.Rows.Count > 0)
                    {
                        if (PD == "")
                        {
                            Fm = Convert.ToDateTime(dt.Rows[0]["fm"]).ToString("yyyy-MM-dd");
                            string updQry1 = "update pr_month_details set fy=" + FY + ",fm=Convert(date,'" + Fm + "'), week_holidays=" + WH + ",paid_holidays=" + PH + ",payment_date=null," +
                              "da_slabs= " + DAslabs + ",da_points=" + DApoints + ",da_percent=" + DApercent + " where FM='" + Fm + "' and active=1";
                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_month_details", Fm.ToString(), ""));
                            await UpdateRecord(updQry1, 4);
                        }
                        else if (PD != "")
                        {
                            PD = Convert.ToDateTime(PD).ToString("yyyy-MM-dd");
                            Fm = Convert.ToDateTime(dt.Rows[0]["fm"]).ToString("yyyy-MM-dd");
                        string updQry = "update pr_month_details set fy=" + FY + ",fm=Convert(date,'" + Fm + "'), week_holidays=" + WH + ",paid_holidays=" + PH + ",payment_date=Convert(date,'" + PD + "')," +
                          "da_slabs= " + DAslabs + ",da_points=" + DApoints + ",da_percent=" + DApercent + " where FM='" + Fm + "' and active=1";
                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_month_details", Fm.ToString(), ""));
                        await UpdateRecord(updQry, 4);
                       // AlertMessage = "I#Error#Please Enter/Modify Data";
                    }
                    }

                    //else
                    //{
                    //    await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                    //    //await InsertRecord(inQry);
                    //    AlertMessage = "I#Error#Please Enter/Modify Data";
                    //}
                }

                catch (Exception ex)
                {

                    return "Error:" + ex.Message;
                }
                
               //return AlertMessage;
            }
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#MonthDetails #MonthDetails Updated Successfully";
            }
            else
            {
                return "E#Error 123#Error 456";
            }

        }
        public async Task<string> getmonthdetals(string monthyear)
        {
            string query1 = "select fm from pr_month_details where active=1";
            DataTable dt1 = await _sha.Get_Table_FromQry(query1);
            var acivemonth = "";
            var acivemonth1 = "";
            if (dt1.Rows.Count > 0)
            {
                acivemonth = Convert.ToDateTime(dt1.Rows[0]["fm"]).ToString("MMM/yyyy");
                acivemonth1 = Convert.ToDateTime(dt1.Rows[0]["fm"]).ToString("yyyy-MM-dd");
            }
            string month = Convert.ToDateTime(monthyear).ToString("yyyy/MM/01");
            string query = "Select fy,format(fm,'MMM-yyyy') as fm,week_holidays,paid_holidays,format(payment_date,'dd-MM-yyyy') as payment_date,da_slabs,da_points,da_percent,month_days from pr_month_details where active=1";
             //return await _sha.Get_Table_FromQry(query);
            DataTable dt= await _sha.Get_Table_FromQry(query);
            var resultJson = JsonConvert.SerializeObject(new { monthdetails = dt,activemonth= acivemonth,activemonth1=acivemonth1 });
            return resultJson;
        }

       
        public async Task<string> getselectedmonthdata(string selectedmonth)
        {
            
            string month = Convert.ToDateTime(selectedmonth).ToString("yyyy/MM/01");
            string query = "Select fy,format(fm,'MMM-yyyy') as fm,week_holidays,paid_holidays,format(payment_date,'yyyy/MM/dd') as payment_date,da_slabs,da_points,da_percent from pr_month_details where fm='" + month + "'";
            //return await _sha.Get_Table_FromQry(query);
            DataTable dt = await _sha.Get_Table_FromQry(query);
            var resultJson = JsonConvert.SerializeObject(new { monthdetails = dt });
            return resultJson;
        }
        public async Task<DataTable> getcurrentmonthyear()
        {
            string mnthyear = "(select convert(date,(DATEADD(month, DATEDIFF(month,0, CONVERT(Date, getdate(), 120)),0))))";
           return await _sha.Get_Table_FromQry(mnthyear);
        }

        //public async Task<string> Fillmonthdata(AddPaymonthModal mnthmodal)
        //{
        //    string mnthyear = "(select convert(date,(DATEADD(month, DATEDIFF(month,0, CONVERT(Date, getdate(), 120)),0))))";
        //    DataTable dt = await getmonthdetals(mnthyear);
        //    AddPaymonthModal mont = new AddPaymonthModal();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        mont.FY= Convert.ToInt32(row["fy"]);
        //        mont.FM = Convert.ToDateTime(row["fm"]);
        //        mont.Weekholidays = Convert.ToInt32(row["week_holidays"]);
        //        mont.Paidholidays = Convert.ToInt32(row["paid_holidays"]);
        //        mont.Paymentdate = Convert.ToDateTime(row["payment_date"]);
        //        mont.DAslabs = Convert.ToInt32(row["da_slabs"]);
        //        mont.DApoints = float.Parse(row["da_points"].ToString());
        //        mont.DApercent = float.Parse(row["da_percent"].ToString());



        //    }

        //    return mont.ToString();
        //}
    }
}
