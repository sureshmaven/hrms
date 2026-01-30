using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using System.Data;
using System.Web.Script.Serialization;

namespace PayRollBusiness.Masters
{
    public class PfNomineeBusiness:BusinessBase
    {
        public PfNomineeBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        public async Task<IList<PfNomineemodel>> GetEmpPfnomineeDetails(string EmpId)
        {

            string getloansdata = "select id,member_name,gender,relation,format(dob,'yyyy-MM-dd') as dob,age,format(date,'yyyy-MM-dd') as date,percentage from pr_pf_nominee where emp_code=" + EmpId + " and active=1";
            DataTable dt = await _sha.Get_Table_FromQry(getloansdata);
            //var resultJson = JsonConvert.SerializeObject(dt);
            //return resultJson;

            IList<PfNomineemodel> lstPfnominee = new List<PfNomineemodel>();

            foreach (DataRow dr in dt.Rows)
            {
                lstPfnominee.Add(new PfNomineemodel
                {
                    id = dr["id"].ToString(),
                    membername = dr["member_name"].ToString(),
                    gender = dr["gender"].ToString(),
                    relation = dr["relation"].ToString(),
                    dob= dr["dob"].ToString(),
                    age= dr["age"].ToString(),
                    date= dr["date"].ToString(),
                    percentage= dr["percentage"].ToString(),
                    Action = "Add"
                });
            }

            return lstPfnominee;
        }


        public async Task<string> InsertUpdatePfNominee(CommonPostDTO Values)
        {
            var multData = Values.Objpfnominee;
            string empid = Values.StringData;
            string qry = "";
            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());
            var loanname = "";
            var loanid = "";
            int NewNumIndex = 0;

            foreach (var pf in multData)
            {
                if (pf.percentage=="")
                {
                    pf.percentage = null;
                }
                    if (pf.Action == "New")
                {
                    //string qryname = "select loan_id,loan_description from pr_loan_master";
                    //DataTable dtqryname = await _sha.Get_Table_FromQry(qryname);
                    //foreach (DataRow lntypes in dtqryname.Rows)
                    //{
                    //    loanid = lntypes["loan_id"].ToString();
                    //    loanname = lntypes["loan_description"].ToString();
                    //    if (loans.loan_id == loanid || loans.loan_description == loanname)
                    //    {
                    //        return "E#Loantype Type#Loan Type Already Exists";
                    //        // return "I#Employee Personal Deduction#Deduction Type Already Exists";
                    //    }
                    //}
                    NewNumIndex++;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_pf_nominee", NewNumIndex));
                    qry = "INSERT INTO pr_pf_nominee(id,emp_id,emp_code,member_name,gender,relation,dob,age,date,percentage,active,trans_id) " +
                        "VALUES(@idnew" + NewNumIndex + ",(select id from employees where empid=" + empid + ")," + empid + ",'" + pf.membername+ "','" + pf.gender + "','" + pf.relation+ "','"+pf.dob+"','"+pf.age+"','"+pf.date+"','"+pf.percentage+"',1,@transidnew)";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_pf_nominee", "@idnew" + NewNumIndex, ""));
                }

                else if (pf.Action == "Update")
                {
                    //qry = "Update pr_emp_lic_details SET days=" + newVal + ", trans_id=@transidnew where dept_id=" + pkid + " AND Emp_Code=" + EmpCode + " ;";
                    qry = " Update pr_pf_nominee SET member_name='" + pf.membername + "',gender='" + pf.gender + "',relation='" + pf.relation + "',dob='" + pf.dob + "',age='" + pf.age + "',date='" + pf.date + "',percentage='" + pf.percentage + "' where id=" + pf.id + " and emp_code="+empid+"";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_pf_nominee", pf.id.ToString(), ""));
                }
                else if (pf.Action == "Deleted")
                {
                    qry = " Update pr_pf_nominee SET active=0 where id=" + pf.id + " AND emp_code=" + empid+ " ;";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_loan_master", pf.id.ToString(), ""));
                }
            }
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#Data Submission#Data Submitted Successfully..!!";
            }
            else
            {
                return "E#Error#Error While Data Submission";
            }
        }
    }
}
