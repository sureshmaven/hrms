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
    public class AdhocPaymentsBusiness : BusinessBase
    {
        public AdhocPaymentsBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public async Task<string> GetAdhocPayDetails(int EmpId)
            {
            int FY = _LoginCredential.FY;
            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
            string[] FM1 = FM.Split('-');
            int Y_FM = int.Parse(FM1[0]);
            int M_FM = int.Parse(FM1[1]);

            string qryGetEFpayfields = "select ef.id as Id, ef.name as Name, epf.amount as Value, case when epf.m_id is null then 'N' else 'U' end as row_type " +
                    "from pr_earn_field_master ef left outer join  pr_emp_adhoc_earn_field epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + EmpId + " and month(epf.fm)="+ M_FM + " and year(epf.fm)="+ Y_FM + " WHERE ef.type='Adhoc';";
                string qryGetDFpayfields = "select df.id as Id, df.name as Name, dpf.amount as Value, case when dpf.m_id is null then 'N' else 'U' end as row_type " +
                    "from pr_deduction_field_master df left outer join  pr_emp_adhoc_deduction_field dpf on df.id = dpf.m_id and dpf.active = 1 and dpf.emp_code = " + EmpId + " and month(dpf.fm)="+ M_FM + " and year(dpf.fm)="+ Y_FM + " WHERE df.type='Adhoc';";
                string qryGetCFpayfields = "select cf.id as Id, cf.name as Name, cpf.amount as Value, case when cpf.m_id is null then 'N' else 'U' end as row_type " +
                    "from pr_contribution_field_master cf left outer join  pr_emp_adhoc_contribution_field cpf on cf.id = cpf.m_id and cpf.active = 1 and cpf.emp_code = " + EmpId + " and month(cpf.fm)="+ M_FM + " and year(cpf.fm)="+ Y_FM + " WHERE cf.type='Adhoc';";


                string qryGetRemarks = "select e.EmpId ,convert(varchar, a.gen_date, 105) as date, a.cheque_no, a.remarks,a.adhoc_type ,case when a.id is null then 'N' else 'U' end as row_type" +
                     " from employees e left join pr_emp_adhoc_det_field a on e.EmpId=a.emp_code and active = 1 and month(a.fm)=" + M_FM + " and year(a.fm)=" + Y_FM + " where e.EmpId=" + EmpId + " ;";

            DataSet dsGetpayfields = await _sha.Get_MultiTables_FromQry(qryGetEFpayfields + qryGetDFpayfields + qryGetCFpayfields+ qryGetRemarks);

                var dtEFfields = dsGetpayfields.Tables[0];
                var dtDFfields = dsGetpayfields.Tables[1];
                var dtCFfields = dsGetpayfields.Tables[2];
                var dtRemarks  = dsGetpayfields.Tables[3];

                var dfjson = JsonConvert.SerializeObject(dtDFfields);
                var efjson = JsonConvert.SerializeObject(dtEFfields);
                var cfjson = JsonConvert.SerializeObject(dtCFfields);
                var Rmarksjson = JsonConvert.SerializeObject(dtRemarks);

                dfjson = dfjson.Replace("null", "''");
                efjson = efjson.Replace("null", "''");
                cfjson = cfjson.Replace("null", "''");
                Rmarksjson = Rmarksjson.Replace("null", "''");

                var javaScriptSerializer = new JavaScriptSerializer();
                var dfDetails = javaScriptSerializer.DeserializeObject(dfjson);
                var efDetails = javaScriptSerializer.DeserializeObject(efjson);
                var cfDetails = javaScriptSerializer.DeserializeObject(cfjson);
                var RmarksDetails = javaScriptSerializer.DeserializeObject(Rmarksjson);

                var resultJson = javaScriptSerializer.Serialize(new { DfDetails = dfDetails, EfDetails = efDetails, CfDetails = cfDetails, RMarksDetails = RmarksDetails });

                return JsonConvert.SerializeObject(resultJson);
            }

        public async Task<string> GetAdhocPayFieldsOnDateSearch(int EmpId, string date)
        {
            int FY = _LoginCredential.FY;
            //string FM = date.ToString("yyyy-MM-dd");
            string[] FM1 = date.Split('-');
            int Y_FM = int.Parse(FM1[2]);
            int M_FM = int.Parse(FM1[1]);

            string qryGetEFpayfields = "select ef.id as Id, ef.name as Name, epf.amount as Value, case when epf.m_id is null then 'N' else 'U' end as row_type " +
                    "from pr_earn_field_master ef left outer join  pr_emp_adhoc_earn_field epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + EmpId + " and month(epf.fm)=" + M_FM + " and year(epf.fm)=" + Y_FM + " WHERE ef.type='Adhoc';";
            string qryGetDFpayfields = "select df.id as Id, df.name as Name, dpf.amount as Value, case when dpf.m_id is null then 'N' else 'U' end as row_type " +
                "from pr_deduction_field_master df left outer join  pr_emp_adhoc_deduction_field dpf on df.id = dpf.m_id and dpf.active = 1 and dpf.emp_code = " + EmpId + " and month(dpf.fm)=" + M_FM + " and year(dpf.fm)=" + Y_FM + " WHERE df.type='Adhoc';";
            string qryGetCFpayfields = "select cf.id as Id, cf.name as Name, cpf.amount as Value, case when cpf.m_id is null then 'N' else 'U' end as row_type " +
                "from pr_contribution_field_master cf left outer join  pr_emp_adhoc_contribution_field cpf on cf.id = cpf.m_id and cpf.active = 1 and cpf.emp_code = " + EmpId + " and month(cpf.fm)=" + M_FM + " and year(cpf.fm)=" + Y_FM + " WHERE cf.type='Adhoc';";


            string qryGetRemarks = "select e.EmpId ,case when convert(varchar, a.gen_date, 105)!=null " +
                  "then convert(varchar, a.gen_date, 105) else '" + date+"' end as date, a.cheque_no, a.remarks,a.adhoc_type ,case when a.id is null then 'N' else 'U' end as row_type" +
                 " from employees e left join pr_emp_adhoc_det_field a on e.EmpId=a.emp_code and active = 1 and month(a.fm)=" + M_FM + " and year(a.fm)=" + Y_FM + " where e.EmpId=" + EmpId + " ;";

            DataSet dsGetpayfields = await _sha.Get_MultiTables_FromQry(qryGetEFpayfields + qryGetDFpayfields + qryGetCFpayfields + qryGetRemarks);

            var dtEFfields = dsGetpayfields.Tables[0];
            var dtDFfields = dsGetpayfields.Tables[1];
            var dtCFfields = dsGetpayfields.Tables[2];
            var dtRemarks = dsGetpayfields.Tables[3];

            var dfjson = JsonConvert.SerializeObject(dtDFfields);
            var efjson = JsonConvert.SerializeObject(dtEFfields);
            var cfjson = JsonConvert.SerializeObject(dtCFfields);
            var Rmarksjson = JsonConvert.SerializeObject(dtRemarks);

            dfjson = dfjson.Replace("null", "''");
            efjson = efjson.Replace("null", "''");
            cfjson = cfjson.Replace("null", "''");
            Rmarksjson = Rmarksjson.Replace("null", "''");

            var javaScriptSerializer = new JavaScriptSerializer();
            var dfDetails = javaScriptSerializer.DeserializeObject(dfjson);
            var efDetails = javaScriptSerializer.DeserializeObject(efjson);
            var cfDetails = javaScriptSerializer.DeserializeObject(cfjson);
            var RmarksDetails = javaScriptSerializer.DeserializeObject(Rmarksjson);

            var resultJson = javaScriptSerializer.Serialize(new { DfDetails = dfDetails, EfDetails = efDetails, CfDetails = cfDetails, RMarksDetails = RmarksDetails });

            return JsonConvert.SerializeObject(resultJson);
        }

        public async Task<string> SaveAdhocPayDetails(CommonPostDTO Values)
        {
            string sFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
            DateTime gen_date = DateTime.Now;
                int FY = _LoginCredential.FY; 

                string FM = "";
                int Y_FM = 0;
                int M_FM = 0;

                int entry_month=0;
                int entry_year=0;
                int EfEmpId = Values.EntityId;

                // N^1=356^~U^3=456^400~U^4=^111;

                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());

                int NewNumIndex = 0;
                string date = "";
                string new_date = "";

                //Insertining details table
                if (Values.StringData4 != null)
            {


                string[] RmarrRows = Values.StringData4.Split(',');
                var Cheque_no = RmarrRows[0];
                if (Cheque_no == "")
                {
                    Cheque_no = null;
                }

                var remarks = RmarrRows[1];
                
                try
                {


                    if (RmarrRows[2] != "")
                    {
                        DateTime Vdate = DateTime.Parse(RmarrRows[2]);

                        FM= Vdate.ToString("yyyy-MM-dd");
                        FY = Helper.getFinancialYear(Vdate);

                        date = Vdate.ToString("MM-dd-yyyy");
                        string[] y_m_date = date.Split('-');

                        entry_month = int.Parse(y_m_date[0]);
                        M_FM = int.Parse(y_m_date[0]);

                        entry_year = int.Parse(y_m_date[2]);
                        Y_FM= int.Parse(y_m_date[2]);

                        new_date = Vdate.ToString("yyyy-MM-dd");
                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                }

                //string qry = "select id,emp_code from pr_emp_adhoc_det_field where emp_code=" + EfEmpId + " and fm='"+ new_date + "';";
                //DataTable dtRemarks = await _sha.Get_Table_FromQry(qry);

                //string emp_code = "";
                //string id = "";
                //foreach (DataRow d in dtRemarks.Rows)
                //{
                //    emp_code = d["emp_code"].ToString();
                //    id = d["id"].ToString();
                //}
                //if (emp_code == "")
                //{
                string qry = "";
                    NewNumIndex++;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_emp_adhoc_det_field", NewNumIndex));

                    //3. qry
                    if (RmarrRows[2] == "")
                    {
                    qry = "update pr_emp_adhoc_earn_field set active=0 where emp_code= " + EfEmpId + " and month(fm)="+ M_FM + " and year(fm)="+ Y_FM + " and active=1;";
                    sbqry.Append(qry);
                    qry = "Insert into pr_emp_adhoc_det_field ([id],[fy],[fm],[emp_id],[emp_code],[gen_date],[cheque_no],[remarks],[active],[trans_id]) values "
                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + sFm + "',(select id from Employees where EmpId=" + Values.EntityId + ")," + Values.EntityId +", null,'" + Cheque_no + "', '" + remarks + "',1, @transidnew);";
                        sbqry.Append(qry);
                    }
                    else
                    {
                    qry = "update pr_emp_adhoc_det_field set active=0 where emp_code= " + EfEmpId + " and month(fm)=" + entry_month + " and year(fm)=" + entry_year + " and active=1;";
                    sbqry.Append(qry);
                    qry = "Insert into pr_emp_adhoc_det_field ([id],[fy],[fm],[emp_id],[emp_code],[gen_date],[cheque_no],[remarks],[active],[trans_id]) values "
                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + sFm + "',(select id from Employees where EmpId=" + Values.EntityId + ")," + Values.EntityId + ",'"+FM+ "', '" + Cheque_no + "', '" + remarks + "',1, @transidnew);";
                        sbqry.Append(qry);
                    }
                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adhoc_det_field", "@idnew" + NewNumIndex, ""));

                //}
                //else
                //{
                //    if (RmarrRows[2] == "")
                //    {
                //        qry = "Update pr_emp_adhoc_det_field SET cheque_no = " + Cheque_no + " ,date = null ,remarks ='" + remarks + "', trans_id=@transidnew  where emp_code=" + Values.EntityId + " ;";

                //    }
                //    else
                //    {
                //        qry = "Update pr_emp_adhoc_det_field SET cheque_no = " + Cheque_no + " ,date ='" + date + "',remarks ='" + remarks + "', trans_id=@transidnew  where emp_code=" + Values.EntityId + " ;";
                //    }


                //    sbqry.Append(qry);

                //    //4. transaction touch
                //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adhoc_det_field", id, Cheque_no));

                //}

            }



                if (Values.StringData != null)
                {
                    //string[] EfarrRows = Values.StringData.Split('~');
                string[] EfarrRows = Values.StringData.Split('&');
                string qry = "";
                qry = "update pr_emp_adhoc_earn_field set active=0 where emp_code= " + EfEmpId + " and month(fm)=" + M_FM + " and year(fm)=" + Y_FM + " and active=1;";
                sbqry.Append(qry);
                foreach (string rdata in EfarrRows)
                    {
                        var arrData = rdata.Split('=');

                        //var arrTypId = arrData[0].Split('^'); //N^1
                        //var type = arrTypId[0];
                        var pkid = arrData[0];

                        //var arrVals = arrData[1].Split('^'); //456^400
                        //var newVal = arrVals[0];
                        var oldVal = arrData[1];
                    
                    if (oldVal != "")
                    {
                        
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_adhoc_earn_field", NewNumIndex));

                        //3. qry
                        qry = "Insert into pr_emp_adhoc_earn_field ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[active],[trans_id],[gen_date]) values "
                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + sFm + "',(select id from Employees where EmpId=" + Values.EntityId + ")," + Values.EntityId + ", " + pkid + ",'EF', " + oldVal + ",1, @transidnew,'"+FM+"'); ";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adhoc_earn_field", "@idnew" + NewNumIndex, ""));
                    }
                        //else if (type == "U" && newVal != "") //update
                        //{
                        //    qry = "Update pr_emp_adhoc_earn_field SET amount=" + newVal + ", trans_id=@transidnew  where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        //    sbqry.Append(qry);

                        //    //4. transaction touch
                        //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adhoc_earn_field", pkid.ToString(), oldVal));
                        //}
                        //else if (type == "U" && newVal == "") //delete
                        //{
                        //    qry = "Update pr_emp_adhoc_earn_field SET active = 0, trans_id=@transidnew  where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        //    sbqry.Append(qry);

                        //    //4. transaction touch
                        //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_adhoc_earn_field", pkid.ToString(), ""));
                        //}
                    }


                }

                if (Values.StringData2 != null)
                {
                    string[] DfarrRows = Values.StringData2.Split('&');
                string qry = "";
                qry = "update pr_emp_adhoc_deduction_field set active=0 where emp_code= " + EfEmpId + " and month(fm)=" + M_FM + " and year(fm)=" + Y_FM + " and active=1;";
                sbqry.Append(qry);
                foreach (string rdata in DfarrRows)
                    {
                        var arrData = rdata.Split('=');

                        //var arrTypId = arrData[0].Split('^'); //N^1
                        //var type = arrTypId[0];
                        var pkid = arrData[0];

                        //var arrVals = arrData[1].Split('^'); //456^400
                        var newVal = arrData[1];
                    //var oldVal = arrVals[1];
                    if (newVal != "")
                    {
                        
                        //if (type == "N")
                        //{
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_adhoc_deduction_field", NewNumIndex));

                        //3. qry
                        qry = "Insert into pr_emp_adhoc_deduction_field ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[active],[trans_id],[gen_date]) values "
                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + sFm + "',(select id from Employees where EmpId=" + Values.EntityId + ")," + Values.EntityId + ", " + pkid + ",'DF', " + newVal + ",1, @transidnew,'" + FM + "');";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adhoc_deduction_field", "@idnew" + NewNumIndex, ""));
                    }
                        // }
                        //else if (type == "U" && newVal != "") //update
                        //{
                        //    qry = "Update pr_emp_adhoc_deduction_field SET amount=" + newVal + ", trans_id=@transidnew   where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        //    sbqry.Append(qry);

                        //    //4. transaction touch
                        //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adhoc_deduction_field", pkid.ToString(), oldVal));
                        //}
                        //else if (type == "U" && newVal == "") //delete
                        //{
                        //    qry = "Update pr_emp_adhoc_deduction_field SET active = 0, trans_id=@transidnew  where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        //    sbqry.Append(qry);

                        //    //4. transaction touch
                        //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_adhoc_deduction_field", pkid.ToString(), ""));
                        //}
                    }
                }

                if (Values.StringData3 != null)
                {
                    string[] CfarrRows = Values.StringData3.Split('&');
                string qry = "";
                qry = "update pr_emp_adhoc_contribution_field set active=0 where emp_code= " + EfEmpId + " and month(fm)=" + entry_month + " and year(fm)=" + entry_year + " and active=1;";
                sbqry.Append(qry);
                foreach (string rdata in CfarrRows)
                    {
                        var arrData = rdata.Split('=');

                        //var arrTypId = arrData[0].Split('^'); //N^1
                        //var type = arrTypId[0];
                       // var pkid = arrTypId[1];
                    var pkid = arrData[0];

                    //var arrVals = arrData[1].Split('^'); //456^400
                    //var newVal = arrVals[0];
                    var newVal = arrData[1];
                    //var oldVal = arrVals[1];
                    if (newVal != "")
                    {
                        //if (type == "N")
                        //{
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_adhoc_contribution_field", NewNumIndex));

                        //3. qry
                        qry = "Insert into pr_emp_adhoc_contribution_field ([id],[fy],[fm],[emp_id],[emp_code],[m_id],[m_type],[amount],[active],[trans_id],[gen_date]) values "
                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from Employees where EmpId=" + Values.EntityId + ")," + Values.EntityId + ", " + pkid + ",'CF', " + newVal + ",1, @transidnew,'" + gen_date + "');";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adhoc_contribution_field", "@idnew" + NewNumIndex, ""));

                    }//}
                        //else if (type == "U" && newVal != "") //update
                        //{
                        //    qry = "Update pr_emp_adhoc_contribution_field SET amount=" + newVal + ", trans_id=@transidnew  where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        //    sbqry.Append(qry);

                        //    //4. transaction touch
                        //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adhoc_contribution_field", pkid.ToString(), oldVal));
                        //}
                        //else if (type == "U" && newVal == "") //delete
                        //{
                        //    qry = "Update pr_emp_adhoc_contribution_field SET active = 0, trans_id=@transidnew  where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        //    sbqry.Append(qry);

                        //    //4. transaction touch
                        //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_adhoc_contribution_field", pkid.ToString(), ""));
                        //}
                    }
                }

               


                // sbqry.Remove(sbqry.Length - 1, 1);
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#Adhoc Payments#Data Added Successfully";
                }
                else
                {
                    return "E#Error#Please Enter Proper Data";
                }
           
        }

    }
}
