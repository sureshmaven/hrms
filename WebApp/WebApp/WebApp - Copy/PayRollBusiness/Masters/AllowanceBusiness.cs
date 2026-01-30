using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.Masters
{
    public class AllowanceBusiness : BusinessBase
    {
        public AllowanceBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
    //    Mavensoft.DAL.Business.BusinessBase bs = new Mavensoft.DAL.Business.BusinessBase();

        public async Task<IList<CommonGetModel>> getAllowanceDetails()
        {
            string qrySel = "SELECT id,name,description,amount,case when amount is null then 'N' else 'U' end as row_type,active "+
                            "FROM pr_branch_allowance_master " +
                            "WHERE active=1 " +
                            "ORDER BY name;";
            DataTable dt = await _sha.Get_Table_FromQry(qrySel);

            IList<CommonGetModel> lstDept = new List<CommonGetModel>();
            foreach (DataRow dr in dt.Rows)
            {
                lstDept.Add(new CommonGetModel
                {
                    Id = dr["id"].ToString(),
                    Name = dr["name"].ToString(),
                    Value = dr["amount"].ToString(),
                    RowType = dr["row_type"].ToString()[0],
                    Description = dr["description"].ToString()
                });
            }
            return lstDept;
        }

       

        public async Task<string> UpdateAllowanceMaster(CommonPostDTO Values)
        {
            var allowanceold= Values.objallowancemasterdata;
            var allowancenew = Values.objallowancemasterdatanew;
            string retMessage = "";
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            try
            {
                string allownsData = getAllowanceData(Values);
                string empids = allownsData.TrimEnd(',');
                int NewNumIndex = 0;
                if (allowanceold != null)
                {
                    if (empids != "")
                    {
                        var arrEmps = empids.Split(',');

                        int allowanceId = 0;
                        foreach (var emp in arrEmps)
                        {
                            var arremp = emp.Split('#');
                            var Id = arremp[0];
                            var Name = arremp[1];
                            var Description = arremp[2];
                            
                            string qry = "";
                            foreach (var item in allowanceold)
                            {
                                NewNumIndex++;
                                if (item.Id == int.Parse(Id))
                                {
                                    allowanceId = int.Parse(Id);
                                    DateTime curentDate = DateTime.Today;

                                    //2. gen new num
                                    sbqry.Append(GetNewNumStringArr("pr_branch_allowance_master", NewNumIndex));
                                    //3. qry
                                    qry = "INSERT INTO pr_branch_allowance_master(id,name,description,amount,date,active,trans_id)  VALUES(@idnew" + NewNumIndex + ",'" + Name + "','" + Description + "','" + item.Amount + "','" + curentDate + "',1,@transidnew);";

                                    sbqry.Append(qry);

                                    //4. transaction touch
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_branch_allowance_master", "@idnew" + NewNumIndex, ""));

                                    qry = " update pr_branch_allowance_master set Active=0 where Id=" + Id + ";";
                                    sbqry.Append(qry);

                                    //4. transaction touch
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_branch_allowance_master", Id.ToString(), ""));
                                }

                            }

                        }
                        
                    }
                }

                if (allowancenew != null)
                {
                    
                    DateTime curentDate = DateTime.Today;
                    foreach (var item in allowancenew)
                    {
                        var Allowance = "";
                        string qryname = "select name from pr_branch_allowance_master";
                        DataTable dtqryname = await _sha.Get_Table_FromQry(qryname);
                        foreach (DataRow allo in dtqryname.Rows)
                        {
                            Allowance = allo["name"].ToString();
                            if (item.Name == Allowance)
                            {
                                return "E#Allowance Type#Allowance Type Already Exists";
                                // return "I#Employee Personal Deduction#Deduction Type Already Exists";
                            }
                        }
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_branch_allowance_master", NewNumIndex));
                        //3. qry
                       string qry = "INSERT INTO pr_branch_allowance_master(id,name,description,amount,date,active,trans_id) VALUES(@idnew" + NewNumIndex + ",'" + item.Name + "','" + item.Description + "','" + item.Value + "','" + curentDate + "',1,@transidnew);";

                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_branch_allowance_master", "@idnew" + NewNumIndex, ""));

                        //qry = " update pr_branch_allowance_master set Active=0 where Id=" + item.Id + ";";
                        //sbqry.Append(qry);

                        ////4. transaction touch
                        //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_branch_allowance_master", item.Id.ToString(), ""));


                    }
                }

                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    retMessage = "I#Allowance Master#Allowance Data Updated Successfully..!!";
                }
                else
                {
                    retMessage = "E#Allowance Master#Error While Allowance Data Updation";
                }
            }
            catch (Exception e)
            {
                retMessage = e.Message;
            }
            return retMessage;
        }

        private string getAllowanceData(CommonPostDTO Values)
        {
            string qryDetails = "";
            string retStr = "";

            qryDetails = "select id,name, description from pr_branch_allowance_master where active=1";

            DataTable dtEmpIds = sh.Get_Table_FromQry(qryDetails);

            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["Id"].ToString() + "#" + dr["Name"].ToString() + "#" + dr["Description"].ToString() + ",";

            }
            return retStr;
        }
    }
}
