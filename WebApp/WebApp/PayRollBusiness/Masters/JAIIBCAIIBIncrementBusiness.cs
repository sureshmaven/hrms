using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PayRollBusiness.Masters
{
    public class JAIIBCAIIBIncrementBusiness : BusinessBase
    {

        string qryGetJAIIBCAIIBIncrfields = null;
        string qryBasicpayInfo = null;

        public JAIIBCAIIBIncrementBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        public async Task<string> GetJAIIBCAIIBIncrementsDetails(int EmpId, string type)

        {
            try
            {
                
                if (type == null)
                {
                    qryGetJAIIBCAIIBIncrfields = "select incr_incen_type,no_of_inc,Incrementamt,convert(varchar,incrementdate,105) as incrementdate ,convert(varchar,incr_WEF_date,105) as incr_WEF_date,authorisation,active " +
                                               "from pr_emp_jaib_caib_general where emp_code =" + EmpId + " and active in (0,1) order by incr_incen_type desc ";
                }
                else
                {
                    qryGetJAIIBCAIIBIncrfields = "select incr_incen_type,no_of_inc,Incrementamt,convert(varchar,incrementdate,105) as incrementdate ,convert(varchar,incr_WEF_date,105) as incr_WEF_date,authorisation,active " +
                                               "from pr_emp_jaib_caib_general where emp_code =" + EmpId + " and active in (0,1) and incr_incen_type='" + type+"';";
                }
                qryBasicpayInfo = "select pfc.amount from pr_earn_field_master as pfm " +
                                            "left join pr_emp_pay_field as pfc " +
                                            "on pfm.id = pfc.m_id " +
                                            "where pfm.id = (select id from pr_earn_field_master where  type='Pay_fields' and name='Basic') and emp_code = " + EmpId + " " +
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
                int FY = _LoginCredential.FY;
                string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
                int EfEmpId = Values.EntityId;
                string qry = "";
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
                    string iwefdate = incr_WEF_date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    string bas_bef_incre= JICRows[5];
                    string id="";
                    string nempid="";
                    string designation = "";
                    string authorisation="";
                    
                    qry = "select m.id,m.emp_code,c.Name,m.authorisation,m.active from pr_emp_jaib_caib_general m " +
                          "inner join (select e.empid,d.Name from employees e inner join Designations d on e.CurrentDesignation=d.Id) c " +
                          "on c.EmpId=m.emp_code where m.emp_code="+Values.EntityId+" and m.incr_incen_type='"+ incr_incen_type + "' and m.active=1";
                    DataTable dt1 = await _sha.Get_Table_FromQry(qry);
                    
                    foreach (DataRow d in dt1.Rows)
                    {
                        
                        id = d["id"].ToString();
                        nempid = d["emp_code"].ToString();
                        designation = d["Name"].ToString();
                        authorisation = d["authorisation"].ToString();
                        
                    }
                    if (designation == "")
                    {
                        qry = "select EmpId,d.Name from Employees e inner join Designations d on e.CurrentDesignation=d.Id where e.EmpId=" + Values.EntityId + ";";
                        DataTable dt2 = await _sha.Get_Table_FromQry(qry);
                        foreach (DataRow d in dt2.Rows)
                        {

                            nempid = d["EmpId"].ToString();
                            designation= d["Name"].ToString();

                        }
                    }
                    if (designation != "Staff Assistant" && no_of_inc >= 2)
                    {
                        string msg = "Only 1 Increment eligible for JAIIB/CAIIB";
                        return "E#Warning!#" + msg;
                    }
                    if (designation == "Staff Assistant" && no_of_inc >= 2 && incr_incen_type=="JAIIB")
                    {
                        string msg = "2 increments only for CAIIB";
                        return "E#Warning!#" + msg;
                    }
                    else if (designation == "Staff Assistant" && no_of_inc > 2 && incr_incen_type == "CAIIB")
                    {
                        string msg = "more than 2 increments not allowed";
                        return "E#Warning!#" + msg;
                    }
                    else
                    {
                        NewNumIndex++;
                        sbqry.Append(GetNewNumStringArr("pr_emp_jaib_caib_general", NewNumIndex));
                        if (id == "")
                        {
                            if (designation == "Staff Assistant" || incr_incen_type == "CAIIB" && no_of_inc <= 2) //allow no of increments=2 for caiib
                            {
                                qry = "Insert into pr_emp_jaib_caib_general ([id],[fy],[fm],[emp_id],[emp_code],[basic_before_inc],[incr_incen_type],[no_of_inc],[Incrementamt],[incrementdate],[incr_WEF_date],[active],[authorisation],[trans_id]) values "
                                   + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + "), " + Values.EntityId + ",'"+ bas_bef_incre + "','" + incr_incen_type + "'," + no_of_inc + "," + Incrementamt + ",'" + idate + "','" + iwefdate + "',1,0, @transidnew);";
                                sbqry.Append(qry);

                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_jaib_caib_general", "@idnew" + NewNumIndex.ToString(), ""));
                            }
                            else if (no_of_inc == 1)
                            {
                                qry = "Insert into pr_emp_jaib_caib_general ([id],[fy],[fm],[emp_id],[emp_code],[basic_before_inc],[incr_incen_type],[no_of_inc],[Incrementamt],[incrementdate],[incr_WEF_date],[active],[authorisation],[trans_id]) values "
                                  + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + "), " + Values.EntityId + ",'" + bas_bef_incre + "','" + incr_incen_type + "'," + no_of_inc + "," + Incrementamt + ",'" + idate + "','" + iwefdate + "',1,0, @transidnew);";
                                sbqry.Append(qry);

                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_jaib_caib_general", "@idnew" + NewNumIndex.ToString(), ""));
                            }

                        }
                        else if (authorisation != "1" && id != "")
                        {
                            if (designation == "Staff Assistant" && incr_incen_type == "CAIIB" && no_of_inc <= 2) //allow no of increments=2 for caiib
                            {
                                
                                qry= "update pr_emp_jaib_caib_general set incr_incen_type='"+ incr_incen_type + "',no_of_inc="+ no_of_inc + ",Incrementamt="+ Incrementamt + ",incrementdate='"+ idate + "',incr_WEF_date='"+ iwefdate + "',basic_before_inc='"+ bas_bef_incre + "', trans_id=@transidnew where emp_code=" + Values.EntityId + " and incr_incen_type='" + incr_incen_type + "' ;";
                                sbqry.Append(qry);                                
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_jaib_caib_general", id, ""));

                                //qry = "Insert into pr_emp_jaib_caib_general ([id],[fy],[fm],[emp_id],[emp_code],[incr_incen_type],[no_of_inc],[Incrementamt],[incrementdate],[incr_WEF_date],[active],[authorisation],[trans_id]) values "
                                //   + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + "), " + Values.EntityId + ",'" + incr_incen_type + "'," + no_of_inc + "," + Incrementamt + ",'" + idate + "','" + iwefdate + "',1,0, @transidnew);";
                                //sbqry.Append(qry);

                                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_jaib_caib_general", "@idnew" + NewNumIndex.ToString(), ""));
                            }
                            else if (no_of_inc == 1)
                            {
                                qry = "update pr_emp_jaib_caib_general set incr_incen_type='" + incr_incen_type + "',no_of_inc=" + no_of_inc + ",Incrementamt=" + Incrementamt + ",incrementdate='" + idate + "',incr_WEF_date='" + iwefdate + "',basic_before_inc='" + bas_bef_incre + "', trans_id=@transidnew where emp_code=" + Values.EntityId + " and incr_incen_type='" + incr_incen_type + "' ;";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_jaib_caib_general", id, ""));

                                //qry = "Insert into pr_emp_jaib_caib_general ([id],[fy],[fm],[emp_id],[emp_code],[incr_incen_type],[no_of_inc],[Incrementamt],[incrementdate],[incr_WEF_date],[active],[authorisation],[trans_id]) values "
                                //   + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid = " + Values.EntityId + "), " + Values.EntityId + ",'" + incr_incen_type + "'," + no_of_inc + "," + Incrementamt + ",'" + idate + "','" + iwefdate + "',1,0, @transidnew);";
                                //sbqry.Append(qry);

                                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_jaib_caib_general", "@idnew" + NewNumIndex.ToString(), ""));
                            }

                        }
                    }
                }

                sbqry.Remove(sbqry.Length - 1, 1);
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#JAIIB/CAIB Increment #Data Added Successfully";
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
