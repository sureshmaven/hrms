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
    public class DateOfLeavingBusiness : BusinessBase
    {
        public DateOfLeavingBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        public async Task<string> UpdateRetirementdate(CommonPostDTO Values)
        {
            //(string FM, int FY, float WH, float PH, DateTime PD, float DApercent, float DApoints, float DAslabs, string activemonth)
            int NewNumIndex = 0;
            int FY = 0;
            NewNumIndex++;
            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());
            string updQry1 = "";
            if (Values.Retirementdata != null)
            {
                try
                {
                    string[] RetirementData = Values.Retirementdata.Split('&');
                    string Empid = RetirementData[0];
                    string RetirementDate = Convert.ToDateTime(RetirementData[1]).ToString("yyyy-MM-dd");

                     updQry1 = "update employees set RetirementDate='" + RetirementDate + "' where EmpId=" + Empid + "";
                    //4. transaction touch
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "employees", Empid, ""));
               

                }

                catch (Exception ex)
                {

                    return "Error:" + ex.Message;
                }

                //return AlertMessage;
            }
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(updQry1.ToString()))
            {
                return "I#Retirement Date #Retirementdate Updated Successfully";
            }
            else
            {
                return "E#Error 123#Error 456";
            }

        }
    }
}
