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

namespace PayRollBusiness.Transaction
{
    public class DeputationEntryBusiness : BusinessBase
    {
        public DeputationEntryBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        public async Task<string> LoadCurrentMonth()
        {
            try
            {
                var qryMonth = "select format(fm,'MMM-yyyy') from pr_month_details where active=1";

                DataTable dtMonthfield = await _sha.Get_Table_FromQry(qryMonth);

                var dtMfield = dtMonthfield;
                //var dtALfileds = dsGetLfields.Tables[1];
                var ltjson = JsonConvert.SerializeObject(dtMfield);
                // var aljson = JsonConvert.SerializeObject(dtALfileds);
                ltjson = ltjson.Replace("null", "''");
                // aljson = aljson.Replace("null", "''");
                var javaScriptSerializer = new JavaScriptSerializer();
                var resultJson = javaScriptSerializer.DeserializeObject(ltjson);
               

                return JsonConvert.SerializeObject(resultJson);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "E#Error:#" + msg;
            }
        }
        public async Task<string> GetDeputationEntryDetails(int EmpId)
        {

            string qryGetDFentryfields = "select distinct dfm.id as Id, dfm.name, dfc.amount as Value, " +
                               "case when dfc.m_id is null then 'N' else 'U' end as row_type " +
                               "from pr_deduction_field_master dfm " +
                               "left outer join  pr_emp_deput_deduction_field dfc " +
                               "on dfm.id = dfc.m_id and dfc.active = 1 and dfc.emp_code =" + EmpId + "" +
                               "where dfm.type='Dep_Ent' " +
                               "and fm=(select fm from pr_month_details where active=1) "+
                               " Union all "+
                               "select distinct  dfm.id as Id, dfm.name, null as Value, case when dfc.m_id is null then 'N' else 'U' end as row_type " +
                               "from pr_deduction_field_master dfm left outer join  pr_emp_deput_deduction_field dfc on dfm.id = dfc.m_id " +
                               "and dfc.active = 1 and dfc.emp_code =" + EmpId + " and fm=(select fm from pr_month_details where active=1)  " +
                               "where dfm.type='Dep_Ent'  and dfm.name not in (select distinct dfm.name from pr_deduction_field_master dfm left outer " +
                               "join  pr_emp_deput_deduction_field dfc on dfm.id = dfc.m_id and dfc.active = 1 " +
                               "and dfc.emp_code =" + EmpId + " where dfm.type='Dep_Ent' and fm=(select fm from pr_month_details where active=1) );";



            string qryGetCFentryfields = "select cfm.id as Id, cfm.name, cfc.amount as Value, " +
                                         "case when cfc.m_id is null then 'N' else 'U' end as row_type " +
                                         "from pr_contribution_field_master cfm " +
                                         "left outer join  pr_emp_deput_contribution_field cfc " +
                                         "on cfm.id = cfc.m_id and cfc.active = 1 and cfc.emp_code =" + EmpId + "" +
                                         "where cfm.type='Dep_Ent' "+
                                         "and fm=(select fm from pr_month_details where active=1) " +
                                         "union all " +
                                         "select cfm.id as Id, cfm.name, null as Value, case when cfc.m_id is null then 'N' else 'U' end as row_type " +
                                         "from pr_contribution_field_master cfm left outer " +
                                         "join  pr_emp_deput_contribution_field cfc on cfm.id = cfc.m_id and cfc.active = 1 and cfc.emp_code = " + EmpId + " " +
                                         "where cfm.type = 'Dep_Ent'; ";

            string qryGetDeptEntryfields = "select cheque_no,format(payment_date,'dd-MM-yyyy') as payment_date,cheque_amount,remarks " +
                                          "from pr_emp_deput_det_field  where emp_code=" + EmpId + " and active=1 and fm=(select fm from pr_month_details where active=1);";

            




            DataSet dsGetpayfields = await _sha.Get_MultiTables_FromQry(qryGetDFentryfields + qryGetCFentryfields + qryGetDeptEntryfields);

            var dtDFfields = dsGetpayfields.Tables[0];
            var dtCFfields = dsGetpayfields.Tables[1];
            var dtDEfields = dsGetpayfields.Tables[2];

     

            var dfjson = JsonConvert.SerializeObject(dtDFfields);
            var cfjson = JsonConvert.SerializeObject(dtCFfields);
            var dejson = JsonConvert.SerializeObject(dtDEfields);
            

            dfjson = dfjson.Replace("null", "''");
            cfjson = cfjson.Replace("null", "''");
            dejson = dejson.Replace("null", "''");
            

            var javaScriptSerializer = new JavaScriptSerializer();
            var dfDetails = javaScriptSerializer.DeserializeObject(dfjson);
            var cfDetails = javaScriptSerializer.DeserializeObject(cfjson);
            var deDetails = javaScriptSerializer.DeserializeObject(dejson);
            

            var resultJson = javaScriptSerializer.Serialize(new { DfDetails = dfDetails, CfDetails = cfDetails, DEfDetails = deDetails});

            return JsonConvert.SerializeObject(resultJson);
        }

        public async Task<string> AddDeputationEntryDetails(CommonPostDTO Values)
        {
            string msg = "";
            try
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
                            sbqry.Append(GetNewNumStringArr("pr_emp_deput_deduction_field", NewNumIndex));

                            //3. qry
                            qry = "Insert into pr_emp_deput_deduction_field ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[active],[trans_id]) values "
                                + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid=" + Values.EntityId + ")," + Values.EntityId + ", " + pkid + ",'DF', " + newVal + ",1, @transidnew);";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_deput_deduction_field", "@idnew" + NewNumIndex, ""));
                        }
                        else if (type == "U" && newVal != "") //update
                        {
                            qry = "Update pr_emp_deput_deduction_field SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_deput_deduction_field", pkid.ToString(), oldVal));
                        }
                        else if (type == "U" && newVal == "") //delete
                        {
                            qry = "Update pr_emp_deput_deduction_field SET active = 0, trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_deput_deduction_field", pkid.ToString(), ""));
                        }
                    }


                }

                if (Values.StringData2 != null)
                {
                    string[] DfarrRows = Values.StringData2.Split('~');

                    foreach (string rdata in DfarrRows)
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
                            sbqry.Append(GetNewNumStringArr("pr_emp_deput_contribution_field", NewNumIndex));

                            //3. qry
                            qry = "Insert into pr_emp_deput_contribution_field ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[active],[trans_id]) values "
                                + "(" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid=" + Values.EntityId + ")," + Values.EntityId + ", " + pkid + ",'DF', " + newVal + ",1, @transidnew);";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_deput_contribution_field", NewNumIndex.ToString(), ""));
                        }
                        else if (type == "U" && newVal != "") //update
                        {
                            qry = "Update pr_emp_deput_contribution_field SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_deput_contribution_field", pkid.ToString(), oldVal));
                        }
                        else if (type == "U" && newVal == "") //delete
                        {
                            qry = "Update pr_emp_deput_contribution_field SET active = 0, trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_deput_contribution_field", pkid.ToString(), ""));
                        }
                    }
                }

                if (Values.StringData3 != null)
                {
                    try
                    {
                        string date = "";
                        string cheque_no;
                        string[] DeptRows = Values.StringData3.Split('&');
                        if (DeptRows[0] == "")
                        {
                            msg = "please enter cheque number";
                            return "E#Error:#" + msg;
                        }
                        else
                        {
                            cheque_no = DeptRows[0];
                        }
                        if (DeptRows[1] == "")
                        {
                            msg = "please select cash paid on";
                            return "E#Error:#" + msg;
                        }
                        else
                        {
                            DateTime payment_date = Convert.ToDateTime(DeptRows[1]);
                            date = payment_date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        
                        string remarks = DeptRows[2];
                        string cheque_Amount = DeptRows[3];

                        string qry = "select id,emp_code from pr_emp_deput_det_field where emp_code=" + EfEmpId + ";";
                        DataTable Derows = await _sha.Get_Table_FromQry(qry);

                        string emp_id = "";
                        string id = "";
                        foreach (DataRow d in Derows.Rows)
                        {
                            emp_id = d["emp_code"].ToString();
                            id = d["id"].ToString();
                        }
                        if (emp_id == "")
                        {
                            NewNumIndex++;

                            //2. gen new num
                            sbqry.Append(GetNewNumStringArr("pr_emp_deput_det_field", NewNumIndex));
                            //3. qry
                            qry = "Insert into pr_emp_deput_det_field ([id],[fy],[fm],[emp_id],[emp_code],[payment_date],[cheque_no],[cheque_amount],[remarks],[active],[trans_id]) values "
                               + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid=" + Values.EntityId + ")," + Values.EntityId + ",'" + date + "','" + cheque_no + "','"+ cheque_Amount + "','" + remarks + "',1, @transidnew);";

                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_deput_det_field", NewNumIndex.ToString(), ""));
                        }
                        else
                        {

                            qry = "Update pr_emp_deput_det_field SET active = 0, trans_id=@transidnew where emp_code=" + Values.EntityId + " ;";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_deput_det_field", id.ToString(), ""));
                            NewNumIndex++;

                            //2. gen new num
                            sbqry.Append(GetNewNumStringArr("pr_emp_deput_det_field", NewNumIndex));
                            //3. qry
                            qry = "Insert into pr_emp_deput_det_field ([id],[fy],[fm],[emp_id],[emp_code],[payment_date],[cheque_no],[cheque_amount],[remarks],[active],[trans_id]) values "
                               + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid=" + Values.EntityId + ")," + Values.EntityId + ",'" + date + "','" + cheque_no + "','"+ cheque_Amount + "','" + remarks + "',1, @transidnew);";

                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_deput_det_field", "@idnew" + NewNumIndex.ToString(), ""));
                            //if (cheque_no != null && date != null)
                            //{
                            //    qry = "Update pr_emp_deput_det_field SET cheque_no=" + cheque_no + ",remarks ='" + remarks + "',payment_date='" + date + "', trans_id=@transidnew where emp_code=" + Values.EntityId + " ;";
                            //}
                            //else if (date != null)
                            //{
                            //    qry = "Update pr_emp_deput_det_field SET cheque_no=null,remarks ='" + remarks + "',payment_date='" + date + "', trans_id=@transidnew where emp_code=" + Values.EntityId + " ;";
                            //}
                            //else if (cheque_no != null)
                            //{
                            //    qry = "Update pr_emp_deput_det_field SET cheque_no=" + cheque_no + ",remarks ='" + remarks + "',payment_date=null, trans_id=@transidnew where emp_code=" + Values.EntityId + " ;";
                            //}
                            //else if (cheque_no == null && date == null)
                            //{
                            //    qry = "Update pr_emp_deput_det_field SET cheque_no=null,remarks ='" + remarks + "',payment_date=null, trans_id=@transidnew where emp_code=" + Values.EntityId + " ;";
                            //}


                            //sbqry.Append(qry);

                            ////4. transaction touch
                            //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_deput_det_field", id, id.ToString()));
                        }

                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                        return "E#Error:#" + msg;
                    }

                }



                sbqry.Remove(sbqry.Length - 1, 1);
                await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());


                string SelQuery = "select emp_id,emp_code,fy,fm,amount,name from " +
                    "pr_emp_deput_contribution_field dc join pr_contribution_field_master cm on dc.m_id = cm.id " +
                    "where fm = '"+ FM + "'  and emp_code="+ EfEmpId + "  union all select emp_id, emp_code, fy, fm, amount, name from " +
                    "pr_emp_deput_deduction_field dc join pr_deduction_field_master cm on dc.m_id = cm.id " +
                    " where fm = '" + FM + "'  and emp_code=" + EfEmpId + "  ";

                DataTable dtdeputation = await _sha.Get_Table_FromQry(SelQuery);

                string name;
                double amount;
                double pension_open=0;
                double VPF = 0;
                double bankshare = 0;
                double ownshare = 0;
                int emp_code;
                int oldemp;
                if (dtdeputation.Rows.Count > 0)
                {
                    string qry = "";
                    foreach (DataRow dr in dtdeputation.Rows)
                    {
                        emp_code = Convert.ToInt32(dr["emp_code"]);
                        name = dr["name"].ToString();
                        if (name == "Max Pension")
                        {
                            pension_open = Convert.ToDouble(dr["amount"]);
                        }
                        if (name == "PF Contribution")
                        {
                            bankshare = Convert.ToDouble(dr["amount"]);
                        }

                        if (name == "Provident Fund")
                        {
                            ownshare = Convert.ToDouble(dr["amount"]);
                        }
                        if (name == "VPF")
                        {
                            VPF = Convert.ToDouble(dr["amount"]);
                        }
                        oldemp = emp_code;
                                         
                    }
                    string getdata = "select * from pr_ob_share where emp_code=" + EfEmpId + "  and fm= '" + FM + "' ";
                    DataTable dtobshare = await _sha.Get_Table_FromQry(getdata);
                    if (dtobshare.Rows.Count > 0)
                    {
                        qry = "delete from pr_ob_share  where emp_code=" + EfEmpId + " AND fm='" + FM + "' ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                       // sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_ob_share", EfEmpId.ToString(), ""));
                    }
                    NewNumIndex++;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_ob_share", NewNumIndex));

                    //3. qry
                    qry = "Insert into pr_ob_share ([id],[emp_id],[emp_code],[fy],[fm],[pension_open],[pension_total],[vpf]," +
                        "[bank_share],[own_share],[active],[trans_id]) values "
                        + "(" + NewNumIndex + ",(select id from employees where empid=" + Values.EntityId + ")," + Values.EntityId + ", " + FY + ",'" + FM + "' ," +
                "" + pension_open + "," + pension_open + ", " + VPF + "," + bankshare + "," + ownshare + ",1, @transidnew);";
                    sbqry.Append(qry);
                }

                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#DeputationEntry #Data Added Successfully";
                }
                else
                {
                    return "E#Error 123#Error 456";
                }
         
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return "E#Error:#" + msg;
            }
        }
        
    }
}
