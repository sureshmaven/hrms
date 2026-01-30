using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Web.Script.Serialization;
using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using System.Data;
namespace PayRollBusiness.Autorization
{
   public class JAIIBCAIIBIncrementAuthBusiness: BusinessBase
    {

        string qryGetJAIIBCAIIBIncrfields = null;
        string qryBasicpayInfo = null;

        public JAIIBCAIIBIncrementAuthBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        public async Task<string> GetJAIIBCAIIBIncrementsDetails(int EmpId, string type)
        {
            try
            {

                if (type == null)
                {
                    qryGetJAIIBCAIIBIncrfields = "select distinct incr_incen_type,no_of_inc,Incrementamt,convert(varchar,incrementdate,105) as incrementdate ,convert(varchar,incr_WEF_date,105) as incr_WEF_date " +
                                               "from pr_emp_jaib_caib_general j_genaral " +
                                               "join pr_month_details md on md.fy=md.fy where emp_code =" + EmpId + " and authorisation = 0 and month(md.fm)>=month(incr_WEF_date) and year(md.fm)>=year(incr_WEF_date)  and md.active=1 and j_genaral.active=1  order by incr_incen_type desc ";
                }
                else
                {
                    qryGetJAIIBCAIIBIncrfields = "select distinct incr_incen_type,no_of_inc,Incrementamt,convert(varchar,incrementdate,105) as incrementdate ,convert(varchar,incr_WEF_date,105) as incr_WEF_date " +
                                               "from pr_emp_jaib_caib_general j_genaral " +
                                               "join pr_month_details md on md.fy=md.fy  where emp_code =" + EmpId + " and authorisation = 0 and month(md.fm)>=month(incr_WEF_date) and year(md.fm)>=year(incr_WEF_date)  and md.active=1 and j_genaral.active=1  and  incr_incen_type='" + type + "';";
                }
                qryBasicpayInfo = "select j.basic_before_inc as amount from pr_earn_field_master as pfm " +
                                            "left join pr_emp_pay_field as pfc " +
                                            "on pfm.id = pfc.m_id  left outer join pr_emp_jaib_caib_general j on j.emp_code=pfc.emp_code " +
                                            "where pfm.id = (select id from pr_earn_field_master where  type='Pay_fields' and name='Basic')  and pfc.emp_code = " + EmpId + " " +
                                            "and pfc.active = 1";
                DataSet dsGetJCIfields = await _sha.Get_MultiTables_FromQry(qryGetJAIIBCAIIBIncrfields + qryBasicpayInfo);

                var dtJCIfields = dsGetJCIfields.Tables[0];
                var dtBPfield = dsGetJCIfields.Tables[1];

                var jcijson = JsonConvert.SerializeObject(dtJCIfields);
                var bpbjson = JsonConvert.SerializeObject(dtBPfield);

                jcijson = jcijson.Replace("null", "''");
                bpbjson = bpbjson.Replace("null", "''");

                var javaScriptSerializer = new JavaScriptSerializer();
                var jciDetails = javaScriptSerializer.DeserializeObject(jcijson);
                var bpbDetails = javaScriptSerializer.DeserializeObject(bpbjson);
                var resultJson = javaScriptSerializer.Serialize(new { JCIDetails = jciDetails, BPBDetails = bpbDetails });
                return JsonConvert.SerializeObject(resultJson);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "E#Error:#" + msg;
            }
        }


        public async Task<string> AddJAIIBCAIIBDetails(CommonPostDTO Values)
        {
            try
            {
                int FY = DateTime.Now.Year + 1;
                string FM = DateTime.Now.ToString("MM-dd-yyyy");
                int EfEmpId = Values.EntityId;

                // N^1=356^~U^3=456^400~U^4=^111;

                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());

                int NewNumIndex = 0;

                if (Values.StringData != null)
                {

                    string[] JICRows = Values.StringData.Split('&');
                    string incr_incen_type = JICRows[0];
                    int no_of_inc = Convert.ToInt32(JICRows[1]);
                    float Incrementamt = float.Parse(JICRows[2]);
                    DateTime incrementdate = Convert.ToDateTime(JICRows[3]);
                    string idate = incrementdate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime incr_WEF_date = Convert.ToDateTime(JICRows[4]);
                    string iwefdate = incrementdate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    NewNumIndex++;

                    if (incr_incen_type == "JAIIB")
                    {
                        string stgQry = "update pr_emp_pay_field set amount=" + Incrementamt + " where emp_code='" + Values.EntityId + "' and active=1 " +
                            "and m_id=(select id from pr_earn_field_master where name='JAIIB Increment' and type='pay_fields');";
                        sbqry.Append(stgQry);
                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", Values.EntityId.ToString(), ""));
                    }
                    else
                    {
                        string anualQry = "update pr_emp_pay_field set amount=" + Incrementamt + " where emp_code='" + Values.EntityId + "' and active=1 " +
                            "and m_id=(select id from pr_earn_field_master where name='CAIIB Increment' and type='pay_fields');";
                        sbqry.Append(anualQry);
                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", Values.EntityId.ToString(), ""));
                    }

                    string qry = "update pr_emp_jaib_caib_general set authorisation=1 where incr_incen_type='"+incr_incen_type+ "' and active=1 and emp_code=" + Values.EntityId + "";
                    sbqry.Append(qry);
                 

                }




                
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#JAIIB/CAIB Increment #Data Authorized Successfully";
                }
                else
                {
                    return "E#JIIB #please ";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "E#Error:#" + msg;
            }


        }
    }
}
