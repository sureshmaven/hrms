using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PayRollBusiness.Masters
{
    public class HouseRentalDetailsBusiness:BusinessBase
    {
        public HouseRentalDetailsBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        public async Task<string> GetHouseRentDetails(int EmpId)
        {
            string qryGetHRentryfields = "select rdm.id as Id,rdm.name,rdc.amount as Value," +
                                         "case when rdc.rent_mid is null then 'N' else 'U' end as row_type " +
                                         "from pr_rentdetails_master as rdm " +
                                         "left join pr_emp_rent_details as rdc " +
                                         "on rdm.id=rdc.rent_mid and rdc.active=1 and rdc.emp_code=" + EmpId+"";

            DataTable dsGetpayfields = await _sha.Get_Table_FromQry(qryGetHRentryfields);

            var dtRDfields = dsGetpayfields;
            

            var rdjson = JsonConvert.SerializeObject(dtRDfields);


            rdjson = rdjson.Replace("null", "''");
            

            var javaScriptSerializer = new JavaScriptSerializer();
            var rdDetails = javaScriptSerializer.DeserializeObject(rdjson);
            

            var resultJson = javaScriptSerializer.Serialize(new { RDDetails = rdDetails});

            return JsonConvert.SerializeObject(resultJson);
        }


        public async Task<string> AddHouseRentDetails(CommonPostDTO Values)
        {
            int FY = _LoginCredential.FY;
            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
            int EfEmpId = Values.EntityId;

            // N^1=356^~U^3=456^400~U^4=^111;

            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());

            int NewNumIndex = 0;


            if (Values.StringData != null)
            {
                string[] EfarrRows = Values.StringData.Split('~');

                foreach (string rdata in EfarrRows)
                {
                    var arrData = rdata.Split('=');

                    var arrTypId = arrData[0].Split('^'); //N^1
                    var type = arrTypId[0];
                    var pkid = arrTypId[1];

                    var arrVals = arrData[1].Split('^'); //456^400
                    var newVal = arrVals[0];
                    var oldVal = arrVals[1];

                    string qry = "";
                    if (type == "N")
                    {
                        NewNumIndex++;
                        
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_rent_details", NewNumIndex));

                        
                        qry = "Insert into pr_emp_rent_details ([id],[fy],[fm],[emp_id],[emp_code],[rent_mid],[amount],[active],[trans_id]) values "
                            + "(" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid=" + Values.EntityId + ")," + Values.EntityId + ", " + pkid + "," + newVal + ",1, @transidnew);";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_rent_details", NewNumIndex.ToString(), ""));
                    }
                    else if (type == "U" && newVal != "") //update
                    {
                        qry = "Update pr_emp_rent_details SET amount=" + newVal + ", trans_id=@transidnew where rent_mid=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_rent_details", pkid.ToString(), oldVal));
                    }
                    else if (type == "U" && newVal == "") //delete
                    {
                        qry = "Update pr_emp_rent_details SET active = 0, trans_id=@transidnew where rent_mid=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_rent_details", pkid.ToString(), ""));
                    }
                }


            }

            

            sbqry.Remove(sbqry.Length - 1, 1);
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#RentDetails #Data Added Successfully";
            }
            else
            {
                return "E#Error 123#Error 456";
            }

        }
    }


}
