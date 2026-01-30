using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayRollBusiness.PayrollService;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Masters
{
    public class OtherTdsDeductionBusiness : BusinessBase
    {
        public OtherTdsDeductionBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();

        public async Task<IList<CommonGetModel>> getOtherTDSDeductions(string empid)
        {

            string qryGetEFpayfields = "select id,tds_amount,remarks from pr_emp_other_tds_deductions where emp_code=" + int.Parse(empid) + " and active=1;";

            DataTable dt = await _sha.Get_Table_FromQry(qryGetEFpayfields);
            IList<CommonGetModel> lstEmpData = new List<CommonGetModel>();
            if (dt.Rows.Count > 0)
            {
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "action_type",
                    Value = "update"
                });
                
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "tds_amount",
                    Name = "TDS Amount",
                    Value = dt.Rows[0]["tds_amount"].ToString()
                });
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "remarks",
                    Name = "Remarks",
                    Value = dt.Rows[0]["remarks"].ToString()
                });
            }
            else
            {
               
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "action_type",
                    Value = "insert"
                });
                
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "tds_amount",
                    Name = "TDS Amount",
                    Value = ""
                });
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "remarks",
                    Name = "Remarks",
                    Value = ""
                });
            }
            return lstEmpData;
        }
        public async Task<string> saveOtherTDSDeductions(OtherTDSDeduction Values)
        {
            try
            {
                int FY = _LoginCredential.FY;
                string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
                int EfEmpId = Values.EntityId;
                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());
                int NewNumIndex = 0;
                string qry = "";
                if (Values.stringData != null)
                {
                    //"action_type=insert&tds_amount=200&remarks=retert"
                    string[] EfarrRows = Values.stringData.Split('&');
                    var arrType = EfarrRows[0].Split('=');
                    var arrAmt = EfarrRows[1].Split('=');
                    var arrRmrk = EfarrRows[2].Split('=');

                    var actionType = arrType[1];
                    var amount = arrAmt[1];
                    var remark = arrRmrk[1];
                    if(actionType == "insert")
                    {
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_other_tds_deductions", NewNumIndex));

                        //3. qry
                        qry = "Insert into pr_emp_other_tds_deductions ([id],[fy]," +
                            "[fm],[emp_id],[emp_code],[tds_amount],[remarks],[active],[trans_id]) values "
                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees where empid=" + EfEmpId + ")," +
                            "" + EfEmpId + "," + amount + ",'" + remark + "',1, @transidnew);";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_other_tds_deductions", "@idnew" + NewNumIndex, ""));

                    }
                    else
                    {
                        qry = "Update pr_emp_other_tds_deductions SET tds_amount=" + amount + ",remarks='" + remark + "', trans_id=@transidnew where emp_code=" + Values.EntityId + " and active=1;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_other_tds_deductions", amount.ToString(), Values.EntityId.ToString()));
                    }



                    //foreach (string rdata in EfarrRows)
                    //{
                    //    //"action_type=insert&tds_amount=200&remarks=retert"

                    //    var arrData = rdata.Split('=');

                    //    var arrTypId = arrData[0].Split('^'); //N^1
                    //    var type = arrTypId[0];
                    //    var pkid = arrTypId[1];

                    //    var arrVals = arrData[1].Split('^'); //456^400
                    //    var newVal = arrVals[0];
                    //    var oldVal = arrVals[1];

                    //    //string qry = "";
                    //    //if (type == "N")
                    //    //{
                    //    //    NewNumIndex++;
                    //    //    //2. gen new num
                    //    //    sbqry.Append(GetNewNumStringArr("pr_emp_pay_field", NewNumIndex));

                    //    //    //3. qry
                    //    //    qry = "Insert into pr_emp_pay_field ([id],[emp_id],[emp_code],[fy]," +
                    //    //        "[fm],[m_id],[m_type],[amount],[active],[trans_id]) values "
                    //    //        + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                    //    //        "" + emp_code + "," + FY + ",'" + FM + "'," + pkid + ",'Pay_fields', " + newVal + ",1, @transidnew);";
                    //    //    sbqry.Append(qry);

                    //    //    //4. transaction touch
                    //    //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_pay_field", "@idnew" + NewNumIndex, ""));
                    //    //}
                    //    //else if (type == "U" && newVal != "") //update
                    //    //{
                    //    //    qry = "Update pr_emp_pay_field SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                    //    //    sbqry.Append(qry);

                    //    //    //4. transaction touch
                    //    //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", pkid.ToString(), oldVal));
                    //    //}
                    //    //else if (type == "U" && newVal == "") //delete
                    //    //{
                    //    //    qry = "Update pr_emp_pay_field SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                    //    //    sbqry.Append(qry);

                    //    //    //4. transaction touch
                    //    //    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", pkid.ToString(), oldVal));
                    //    //}

                    //}
                }
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#Other TDS Deductions#Data Added Successfully";
                }
                else
                {
                    return "E#Error 123#Error 456";
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
