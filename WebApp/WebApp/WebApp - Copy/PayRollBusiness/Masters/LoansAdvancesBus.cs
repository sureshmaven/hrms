using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PayRollBusiness.Masters
{
    public class LoansAdvancesBus : BusinessBase
    {


        public LoansAdvancesBus(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        int loansno = 0;
        public async Task<string> GetLoanTypeDetails()
        {
            try
            {
                var qryGetLoanType = "select * from pr_loan_master where active=1";
                DataTable dsGetLfields = await _sha.Get_Table_FromQry(qryGetLoanType);

                var dtLTfields = dsGetLfields;
                //var dtALfileds = dsGetLfields.Tables[1];

                var ltjson = JsonConvert.SerializeObject(dtLTfields);

                ltjson = ltjson.Replace("null", "''");

                var javaScriptSerializer = new JavaScriptSerializer();
                var ltDetails = javaScriptSerializer.DeserializeObject(ltjson);

                var resultJson = javaScriptSerializer.Serialize(new { LTDetails = ltDetails });

                return JsonConvert.SerializeObject(resultJson);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "E#Error:#" + msg;
            }
        }
        public async Task<string> GetLoanTypeDetailsforReport()
        {
            try
            {
                var qryGetLoanType = "select * from pr_loan_master";
                DataTable dsGetLfields = await _sha.Get_Table_FromQry(qryGetLoanType);

                var dtLTfields = dsGetLfields;
                //var dtALfileds = dsGetLfields.Tables[1];

                var ltjson = JsonConvert.SerializeObject(dtLTfields);

                ltjson = ltjson.Replace("null", "''");

                var javaScriptSerializer = new JavaScriptSerializer();
                var ltDetails = javaScriptSerializer.DeserializeObject(ltjson);

                var resultJson = javaScriptSerializer.Serialize(new { LTDetails = ltDetails });

                return JsonConvert.SerializeObject(resultJson);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "E#Error:#" + msg;
            }
        }

        public async Task<string> GetLoanDetails(int empid, string type, int loanid, string loancode)

        {
            try
            {
                // string testloancode = WebConfigurationManager.AppSettings["loancode"]; ;

                //var arrtestloancode = testloancode.Split(",");

                string qryLoanDetails;
                string qryLoanDivDetails;
                int mid = 0;
                int lmid = 0;
                if (type == "")
                {
                    string qrygettopid = "select top 1 l.id,lm.loan_id, l.loan_type_mid from pr_emp_adv_loans l join pr_loan_master lm on lm.id=l.loan_type_mid  where emp_code=" + empid + " and l.active=1";
                    DataTable dt3 = await _sha.Get_Table_FromQry(qrygettopid);
                    foreach (DataRow item in dt3.Rows)
                    {
                        mid = int.Parse(item["id"].ToString());
                        lmid = int.Parse(item["loan_type_mid"].ToString());
                        loancode = item["loan_id"].ToString();
                    }
                    if (loancode == @PrConstants.PF_LOANST1_CODE || loancode == @PrConstants.PF_LOANST2_CODE || loancode == @PrConstants.PF_LOANLT1_CODE || loancode == @PrConstants.PF_LOANLT2_CODE || loancode == @PrConstants.PF_LOANLT3_CODE || loancode == @PrConstants.PF_LOANLT4_CODE || loancode == @PrConstants.PF_LOAN1_CODE || loancode == @PrConstants.PF_LOAN2_CODE || loancode == @PrConstants.FESTIVAL_LOAN_CODE)
                    //if (loancode == PrConstants.PF_LOAN1_CODE || loancode == PrConstants.PF_LOAN2_CODE || loancode == PrConstants.FESTIVAL_LOAN_CODE)
                    {
                        qryLoanDetails = "select l.id,l.loan_id,l.loan_description,l.total_amount,l.principal_installment,l.interest_installment," +
                            "format(l.sanction_date,'yyyy-MM-dd') as sanction_date,l.method,l.interest_rate,l.installment_amount,l.interest_installment_amount," +
                            "l.total_recovered_amount,l.completed_installment,format(l.installment_start_date,'yyyy-MM-dd') as " +
                            "loan_start_from, l.code_master as code_master from (select al.loan_type_mid,al.id,al.emp_code,lm.loan_id,lm.loan_description,al.total_amount," +
                            "al.principal_installment,al.interest_installment,al.sanction_date,al.method,al.interest_rate,al.installment_amount," +
                            "al.interest_installment_amount,al.total_recovered_amount,al.completed_installment,al.installment_start_date,al.code_master," +
                            "al.active from pr_emp_adv_loans al inner join pr_loan_master lm on al.loan_type_mid=lm.id) as l " +
                            " where l.emp_code=" + empid + " and l.loan_type_mid=" + lmid + " and l.active=1;";

                        qryLoanDivDetails = "select top 2 c.id,c.slno,format(c.date_disburse,'dd-MM-yyyy') as date_disburse,c.loan_amount,c.interest_rate,c.interest_accured,c.principal_amount_recovered," +
                       "c.interest_amount_recovered,c.priority from pr_emp_adv_loans_child as c inner join pr_emp_adv_loans as m on c.emp_adv_loans_mid=m.id " +
                       "where m.emp_code=" + empid + " and c.emp_adv_loans_mid=" + mid + " and c.active=1 order by emp_adv_loans_mid;";
                    }
                    else
                    {

                        qryLoanDetails = "select l.id,l.loan_id,l.loan_description,l.total_amount,l.principal_installment,l.interest_installment," +
                            "format(l.sanction_date,'yyyy-MM-dd') as sanction_date,l.method,l.interest_rate,l.installment_amount," +
                            "l.interest_installment_amount,l.total_recovered_amount,l.completed_installment,format(l.installment_start_date,'yyyy-MM-dd')" +
                            " as loan_start_from,m.Name as code_master from (select al.loan_type_mid, al.id,al.emp_code,lm.loan_id,lm.loan_description,al.total_amount," +
                            "al.principal_installment,al.interest_installment,al.sanction_date,al.method,al.interest_rate,al.installment_amount," +
                            "al.interest_installment_amount,al.total_recovered_amount,al.completed_installment,al.installment_start_date," +
                            "al.code_master,al.active from pr_emp_adv_loans al inner join pr_loan_master lm on al.loan_type_mid=lm.id) as l " +
                            "inner join All_Masters m on l.code_master=m.Id where l.emp_code=" + empid + " and l.loan_type_mid=" + lmid + " and l.active=1;";

                        qryLoanDivDetails = "select top 4 c.id,c.slno,format(c.date_disburse,'dd-MM-yyyy') as date_disburse,c.loan_amount,c.interest_rate,c.interest_accured,c.principal_amount_recovered," +
                       "c.interest_amount_recovered,c.priority from pr_emp_adv_loans_child as c inner join pr_emp_adv_loans as m on c.emp_adv_loans_mid=m.id " +
                       "where m.emp_code=" + empid + " and c.emp_adv_loans_mid=" + mid + " and c.active=1 order by emp_adv_loans_mid;";

                    }
                }
                else
                {
                    if (loancode == @PrConstants.PF_LOANST1_CODE || loancode == @PrConstants.PF_LOANST2_CODE || loancode == @PrConstants.PF_LOANLT1_CODE || loancode == @PrConstants.PF_LOANLT2_CODE || loancode == @PrConstants.PF_LOANLT3_CODE || loancode == @PrConstants.PF_LOANLT4_CODE || loancode == @PrConstants.PF_LOANST1_CODE || loancode == @PrConstants.PF_LOANST2_CODE || loancode == @PrConstants.PF_LOANLT1_CODE || loancode == @PrConstants.PF_LOANLT2_CODE || loancode == @PrConstants.PF_LOANLT3_CODE || loancode == @PrConstants.PF_LOANLT4_CODE || loancode == @PrConstants.PF_LOAN1_CODE || loancode == @PrConstants.PF_LOAN2_CODE || loancode == @PrConstants.FESTIVAL_LOAN_CODE)
                    //if (loancode == PrConstants.PF_LOAN1_CODE || loancode == PrConstants.PF_LOAN2_CODE || loancode == PrConstants.FESTIVAL_LOAN_CODE)
                    {
                        qryLoanDetails = "select l.id,l.loan_id,l.loan_description,l.total_amount,l.principal_installment,l.interest_installment," +
                            "format(l.sanction_date,'yyyy-MM-dd') as sanction_date,l.method,l.interest_rate,l.installment_amount,l.interest_installment_amount," +
                            "l.total_recovered_amount,l.completed_installment,format(l.installment_start_date,'yyyy-MM-dd') as " +
                            "loan_start_from, l.code_master as code_master from (select al.loan_type_mid, al.id,al.emp_code,lm.loan_id,lm.loan_description,al.total_amount," +
                            "al.principal_installment,al.interest_installment,al.sanction_date,al.method,al.interest_rate,al.installment_amount," +
                            "al.interest_installment_amount,al.total_recovered_amount,al.completed_installment,al.installment_start_date,al.code_master," +
                            "al.active from pr_emp_adv_loans al inner join pr_loan_master lm on al.loan_type_mid=lm.id) as l " +
                            " where l.emp_code=" + empid + " and l.loan_type_mid=" + loanid + " and l.active=1;";

                        qryLoanDivDetails = "select c.id,c.slno,format(c.date_disburse,'yyyy-MM-dd') as date_disburse,c.loan_amount,c.interest_rate,c.interest_accured,c.principal_amount_recovered,c.interest_amount_recovered,c.priority " +
                                            "from pr_emp_adv_loans_child as c inner join pr_emp_adv_loans as m on c.emp_adv_loans_mid=m.id " +
                                            "where m.emp_code=" + empid + " and m.loan_type_mid= (select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id where l.emp_code=" + empid + " and m.loan_description='" + type + "' and l.active=1) " +
                                            "and c.active=1 order by priority;";
                    }
                    else
                    {


                        qryLoanDetails = "select l.id,l.loan_id,l.loan_description,l.total_amount,l.principal_installment,l.interest_installment," +
                            "format(l.sanction_date,'yyyy-MM-dd') as sanction_date,l.method,l.interest_rate,l.installment_amount,l.interest_installment_amount," +
                            "l.total_recovered_amount,l.completed_installment,format(l.installment_start_date,'yyyy-MM-dd') as " +
                            "loan_start_from,m.Name as code_master from (select al.loan_type_mid, al.id,al.emp_code,lm.loan_id,lm.loan_description,al.total_amount," +
                            "al.principal_installment,al.interest_installment,al.sanction_date,al.method,al.interest_rate,al.installment_amount," +
                            "al.interest_installment_amount,al.total_recovered_amount,al.completed_installment,al.installment_start_date,al.code_master," +
                            "al.active from pr_emp_adv_loans al inner join pr_loan_master lm on al.loan_type_mid=lm.id) as l inner join All_Masters m " +
                            "on l.code_master=m.Id where l.emp_code=" + empid + " and l.loan_type_mid=" + loanid + " and l.active=1;";

                        qryLoanDivDetails = "select c.id,c.slno,format(c.date_disburse,'yyyy-MM-dd') as date_disburse,c.loan_amount,c.interest_rate,c.interest_accured,c.principal_amount_recovered,c.interest_amount_recovered,c.priority " +
                                            "from pr_emp_adv_loans_child as c inner join pr_emp_adv_loans as m on c.emp_adv_loans_mid=m.id " +
                                            "where m.emp_code=" + empid + " and m.loan_type_mid= (select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id where l.emp_code=" + empid + " and m.loan_description='" + type + "' and l.active=1) " +
                                            "and c.active=1 order by priority;";
                    }
                }

                DataTable dt1 = await _sha.Get_Table_FromQry(qryLoanDetails);

                List<loansDetails> dlDetails = new List<loansDetails>();
                if (dt1.Rows.Count > 0)
                {
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "loan_description",
                        display = "Loan Type",
                        Value = dt1.Rows[0]["loan_description"].ToString(),
                       
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "total_amount",
                        display = "Total Amount",
                        Value = dt1.Rows[0]["total_amount"].ToString(),
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "principal_installment",
                        display = "No. Of Principal Installments",
                        Value = dt1.Rows[0]["principal_installment"].ToString(),
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "interest_installment",
                        display = "No. Of Interest Installments",
                        Value = dt1.Rows[0]["interest_installment"].ToString(),
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "sanction_date",
                        display = "Sanction Date",
                        Value = dt1.Rows[0]["sanction_date"].ToString(),
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "method",
                        display = "Method",
                        Value = dt1.Rows[0]["method"].ToString(),
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "interest_rate",
                        display = "Interest Rate",
                        Value = dt1.Rows[0]["interest_rate"].ToString(),
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "installment_amount",
                        display = "Installment Amount",
                        Value = dt1.Rows[0]["installment_amount"].ToString(),
                        Action = dt1.Rows[0]["installment_amount"].ToString() == "" ? "new" : "update"

                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "interest_installment_amount",
                        display = "Interest Installment Amount",
                        Value = dt1.Rows[0]["interest_installment_amount"].ToString(),
                        Action = dt1.Rows[0]["interest_installment_amount"].ToString() == "" ? "new" : "update"
                    });
                    //dlDetails.Add(new loansDetails
                    //{
                    //    Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                    //    Name = "total_recovered_amount",
                    //    display = "Total Recovered Amount",
                    //    Value = dt1.Rows[0]["total_recovered_amount"].ToString(),
                    //});
                    //dlDetails.Add(new loansDetails
                    //{
                    //    Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                    //    Name = "completed_installment",
                    //    display = "Completed Installment",
                    //    Value = dt1.Rows[0]["completed_installment"].ToString(),
                    //});
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "loan_start_from",
                        display = "Loan/Installment Starts From",
                        Value = dt1.Rows[0]["loan_start_from"].ToString(),
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "code_master",
                        display = "Loan Vendor Name",
                        Value = dt1.Rows[0]["code_master"].ToString(),
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = Convert.ToInt32(dt1.Rows[0]["Id"]),
                        Name = "loan_id",
                        display = "Loan Id",
                        Value = dt1.Rows[0]["loan_id"].ToString(),
                    });
                }
                else
                {
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "loan_description",
                        display = "Loan Type",
                        Value = ""
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "total_amount",
                        display = "Total Amount",
                        Value = ""
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "principal_installment",
                        display = "No. Of Principal Installements",
                        Value = ""
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "interest_installment",
                        display = "No. Of Interest Installments",
                        Value = ""
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "sanction_date",
                        display = "Sanction Date",
                        Value = ""
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "method",
                        display = "Method",
                        Value = ""
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "interest_rate",
                        display = "Interest Rate",
                        Value = ""
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "installment_amount",
                        display = "Installment Amount",
                        Value = ""
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "interest_installment_amount",
                        display = "Interest Installment Amount",
                        Value = ""
                    });
                    //dlDetails.Add(new loansDetails
                    //{
                    //    Id = 0,
                    //    Name = "total_recovered_amount",
                    //    display = "Recovered Amount",
                    //    Value = ""
                    //});
                    //dlDetails.Add(new loansDetails
                    //{
                    //    Id = 0,
                    //    Name = "completed_installment",
                    //    display = "Completed Installments",
                    //    Value = ""
                    //});
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "loan_start_from",
                        display = "Loan/Installment Starts From",
                        Value = ""
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "code_master",
                        display = "Loan Vendor Name",
                        Value = ""
                    });
                    dlDetails.Add(new loansDetails
                    {
                        Id = 0,
                        Name = "loan_id",
                        display = "Loan Id",
                        Value = "",
                    });
                }

                DataTable dt = await _sha.Get_Table_FromQry(qryLoanDivDetails);


                List<LoansAndAdavnceModel> loansData = new List<LoansAndAdavnceModel>();
                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        loansData.Add(new LoansAndAdavnceModel
                        {
                            id = Convert.ToInt32(dr["id"]),
                            priority = dr["priority"].ToString(),
                            date_disburse = dr["date_disburse"].ToString(),
                            loan_amount = dr["loan_amount"].ToString(),
                            interest_amount_recovered = dr["interest_amount_recovered"].ToString(),
                            interest_rate = dr["interest_rate"].ToString(),
                            interest_accured = dr["interest_accured"].ToString(),
                            principal_amount_recovered = dr["principal_amount_recovered"].ToString(),
                            Action = "Add"
                        });
                    }
                }

                var qryLoanVendor = "select id, Name from ALL_MASTERS where Description='" + PrConstants.Loan_Vendor_Name + "';";
                DataTable dtloanvendor = await _sha.Get_Table_FromQry(qryLoanVendor);

                var dtLoanVendor = dtloanvendor;
                var Lvjson = JsonConvert.SerializeObject(dtLoanVendor);

                var javaScriptSerializer = new JavaScriptSerializer();
                var LvDetails = javaScriptSerializer.DeserializeObject(Lvjson);

                var resultJson = javaScriptSerializer.Serialize(new { LDDetails = dlDetails, ALDetails = loansData, LVDetails = LvDetails });

                return JsonConvert.SerializeObject(resultJson);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "E#Error:#" + msg;
            }
        }

        public async Task<string> SaveLoanDetails(CommonPostDTO values)
        {
            double principal = 0;
            double inaccure = 0;
            string action = values.Loancols[0].Action.ToString();
            
            string res = "";
            //var dtStart = new DateTime(2019, 8, 1);

            //int empCode = values.EntityId;
            //string loan_code = values.StringData;
            //int princeOutstanding = int.Parse(values.Loancols[1].Value.ToString());
            //int installmentAmount = int.Parse(values.Loancols[7].Value.ToString());
            //DateTime dtAmountDisburse = DateTime.Parse(values.Loancols[4].Value.ToString());
            //DateTime dtInstallmentStart = DateTime.Parse(values.Loancols[10].Value.ToString());
            //LoadLedgerReportNEW(empCode, loan_code, princeOutstanding, installmentAmount, 0, 0, dtAmountDisburse, dtInstallmentStart);
            //return "";
            //await LoadLedgerReportNEW(0, 22);
            //return "";

            string query = "select last_num from new_num where table_name='loan_sl_no'";
            DataTable dt = await _sha.Get_Table_FromQry(query);
             loansno = Convert.ToInt32(dt.Rows[0]["last_num"]);
            loansno++;
           
            try
            {

                //loancode
                string loancode = values.StringData;
              

                //Declaration
                string principal_installment = "";
                string interest_installment = "";
                int total_amount_recovered = 0;
                int total_installment;
                int remaining_installment;
                DateTime enddate;
                string installment_end_date = "";
                var divpric_installents = 0;
                var Fpriority_prininstall = 0;
                var divintr_install = 0;
                var Fpriority_intinstall = 0;
                var Spriority_intinstall = 0;


                //
                string total_amount = "";
                string sanction_date = "";
                string method = "";
                string interest_rate = "";
                string installment_amount = "";
                Nullable<int> interest_installment_amount;
                string total_recovered_amount = "";
                string completed_installment = "";
                string code_master = "";
                var loandetails = values.Loancols;
                var subloandetails = values.objectLD;
                string installment_start_date = "";
                //store adv_loans_master
                string loan_description = loandetails[0].Value;

                //financial year and month
                int fy = _LoginCredential.FY;
                string sFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
                  if (action == "update")
                {
                    int NewNumIndex = 0;
                    StringBuilder sbqry = new StringBuilder();
                    string getloanid = "select id from pr_loan_master where loan_id='" + loancode + "'";
                    DataTable dtloannid = await _sha.Get_Table_FromQry(getloanid);
                    int loan_type_id = Convert.ToInt32(dtloannid.Rows[0]["id"]);
                    string qry = "";

                    string Updatedinstamount = values.Loancols[0].Value.ToString();
                    string UpdatedIntinstamount = values.Loancols[0].Value.ToString();
                    int Updated_emp_code = values.EntityId;
                    string updatedfield= values.Loancols[0].Name.ToString();
                    if (values.Loancols.Count>1)
                    {
                        Updatedinstamount = values.Loancols[0].Value.ToString();
                         UpdatedIntinstamount = values.Loancols[1].Value.ToString();
                        qry = "update pr_emp_adv_loans set installment_amount='" + Updatedinstamount + "', interest_installment_amount='" + UpdatedIntinstamount + "' where emp_code=" + Updated_emp_code + " and loan_type_mid=" + loan_type_id + ";";

                    }
                    else
                    {
                        if (updatedfield == "installment_amount")
                        {
                            qry = "update pr_emp_adv_loans set installment_amount='" + Updatedinstamount + "' where emp_code=" + Updated_emp_code + " and loan_type_mid=" + loan_type_id + ";";

                        }
                        else if (updatedfield == "interest_installment_amount")
                        {
                            qry = "update pr_emp_adv_loans set interest_installment_amount='" + UpdatedIntinstamount + "' where emp_code=" + Updated_emp_code + " and loan_type_mid=" + loan_type_id + ";";
                        }
                 

                    }
                    ////sbqry.Append(qry);
                    sbqry.Append(qry);
                    //4. transaction touch

                    await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                    res = "I#Loan #Updated Successfully.";
                }
                if (action == "new" || action== "null")
                {
                    if (loancode == @PrConstants.PF_LOANST1_CODE || loancode == @PrConstants.PF_LOANST2_CODE || loancode == @PrConstants.PF_LOANLT1_CODE || loancode == @PrConstants.PF_LOANLT2_CODE || loancode == @PrConstants.PF_LOANLT3_CODE || loancode == @PrConstants.PF_LOANLT4_CODE || loancode == PrConstants.PF_LOAN1_CODE || loancode == PrConstants.PF_LOAN2_CODE || loancode == PrConstants.FESTIVAL_LOAN_CODE)
                    {
                        principal_installment = loandetails[2].Value;  //total principal installements ex:192
                        interest_installment = loandetails[3].Value;   //total interest installements  ex:48
                        installment_start_date = loandetails[9].Value;
                        //the first installment date   ex:2019-06-01

                        //total installments
                        total_installment = Convert.ToInt32(principal_installment) + Convert.ToInt32(interest_installment); // total installments 192+48=240
                        remaining_installment = total_installment; //240
                        enddate = Convert.ToDateTime(installment_start_date).AddMonths(total_installment - 1);
                        installment_end_date = enddate.ToString("yyyy-MM-dd"); //installement end date 2019-06-01 + 144-1

                        divpric_installents = Convert.ToInt32(principal_installment) / 4; //2nd priority installements 192/4= 48
                        Fpriority_prininstall = divpric_installents * 3;  //1st priority installements 48*3=144

                        divintr_install = Convert.ToInt32(interest_installment) / 4; //2nd priority interest installments 48/4=12
                        Fpriority_intinstall = divintr_install * 3; //1st priority interest installments 12*3=36

                        Spriority_intinstall = Convert.ToInt32(principal_installment) + Convert.ToInt32(Fpriority_intinstall - 1); // 


                        total_amount = loandetails[1].Value;
                        sanction_date = loandetails[4].Value;
                        method = loandetails[5].Value;
                        interest_rate = loandetails[6].Value;
                        installment_amount = loandetails[7].Value;
                        interest_installment_amount = 0;
                        //total_recovered_amount = loandetails[8].Value;
                        //completed_installment = loandetails[9].Value;
                        code_master = "0";

                    }

                    else
                    {
                        int scndsubloanamount = 0;
                        double scndsubloaninst = 0;
                        int fstsubloanamount = Convert.ToInt32(subloandetails[0].loan_amount);
                        int installmentamount = Convert.ToInt32(loandetails[7].Value);
                        if (subloandetails.Count > 1)
                        {
                            scndsubloanamount = Convert.ToInt32(subloandetails[1].loan_amount);
                            scndsubloaninst = Math.Round((double)scndsubloanamount / installmentamount);
                        }

                        double fstsubloaninst = Math.Round((double)fstsubloanamount / installmentamount);


                        principal_installment = loandetails[2].Value;  //total principal installements ex:192
                        interest_installment = loandetails[3].Value;   //total interest installements  ex:48
                        installment_start_date = loandetails[9].Value;
                        //the first installment date   ex:2019-06-01
                        //total installments
                        total_installment = Convert.ToInt32(principal_installment) + Convert.ToInt32(interest_installment); // total installments 192+48=240
                        remaining_installment = total_installment; //240
                        enddate = Convert.ToDateTime(installment_start_date).AddMonths(total_installment - 1);
                        installment_end_date = enddate.ToString("yyyy-MM-dd"); //installement end date 2019-06-01 + 144-1
                                                                               /* divpric_installents = Convert.ToInt32(principal_installment) / 4;*/ //2nd priority installements 192/4= 48
                        divpric_installents = Convert.ToInt32(scndsubloaninst);
                        /*  Fpriority_prininstall = divpric_installents * 3;*/  //1st priority installements 48*3=144
                        Fpriority_prininstall = Convert.ToInt32(fstsubloaninst);
                        divintr_install = Convert.ToInt32(interest_installment) / 4; //2nd priority interest installments 48/4=12
                        Fpriority_intinstall = divintr_install * 3; //1st priority interest installments 12*3=36
                        Spriority_intinstall = Convert.ToInt32(principal_installment) + Convert.ToInt32(Fpriority_intinstall - 1); // 


                        total_amount = loandetails[1].Value;
                        sanction_date = loandetails[4].Value;
                        method = loandetails[5].Value;
                        interest_rate = loandetails[6].Value;
                        installment_amount = loandetails[7].Value;
                        interest_installment_amount = Convert.ToInt32(loandetails[8].Value);
                        //total_recovered_amount = loandetails[9].Value;
                        //completed_installment = loandetails[10].Value;
                        code_master = loandetails[10].Value;
                    }

                    //store advloanschild details
                    var loanchilddetails = values.objectLD;
                    StringBuilder sbqry = new StringBuilder();
                    //1. trans_id
                    sbqry.Append(GenNewTransactionString());

                    int NewNumIndex = 0;
                    string qry = "";
                    int emp_code = values.EntityId;
                    //get id and empcode if already exists in db
                    qry = "select id,emp_code from pr_emp_adv_loans where emp_code=" + emp_code + " and active=1 and loan_type_mid= (select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id where l.emp_code=" + emp_code + " and m.loan_description='" + loan_description + "' and l.active=1);";
                    DataTable Derows = await _sha.Get_Table_FromQry(qry);

                    string employeeid = "";
                    string id = "";
                    foreach (DataRow d in Derows.Rows)
                    {
                        employeeid = d["emp_code"].ToString();
                        id = d["id"].ToString();
                    }


                    //check the Loandetails is empty or not
                    if (values.Loancols != null)
                    {
                        sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans", NewNumIndex));
                        //1. trans_id
                        if (employeeid == "")
                        {
                            qry = "Insert into pr_emp_adv_loans(id, emp_id, emp_code, designation, loan_type_mid, total_amount, total_installment,remaining_installment,principal_installment, interest_installment, completed_installment, sanction_date, installment_start_date,installment_end_date,method, interest_rate, installment_amount,interest_installment_amount, total_recovered_amount, code_master, active, trans_id,fm,fy,loan_sl_no) " +
                           "values(@idnew" + NewNumIndex + ",(select id from employees where EmpId=" + emp_code + ")," + emp_code + ",(select d.id from Designations d inner join Employees e on d.id=e.CurrentDesignation where e.EmpId=" + emp_code + "),(select id from pr_loan_master where loan_description='" + loan_description + "' and active=1)," + total_amount + "" +
                           "," + total_installment + "," + remaining_installment + "," + principal_installment + "," + interest_installment + ",0,'" + sanction_date + "','" + installment_start_date + "','" + installment_end_date + "','" + method + "'," + interest_rate + "," + installment_amount + "," + interest_installment_amount + ",0,(select id from All_Masters where Name='" + code_master + "' and Active=1),1,@transidnew,'" + sFm + "'," + fy + "," + loansno + ");";

                            sbqry.Append(qry);
                        }
                        else
                        {
                            qry = "update pr_emp_adv_loans set active=0 where emp_code=" + emp_code + " and active=1 and loan_type_mid= (select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id where l.emp_code=" + emp_code + " and m.loan_description='" + loan_description + "' and l.active=1);";
                            sbqry.Append(qry);

                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_adv_loans", id.ToString(), ""));

                            qry = "Insert into pr_emp_adv_loans(id, emp_id, emp_code, designation, loan_type_mid, total_amount, total_installment,remaining_installment,principal_installment, interest_installment, completed_installment, sanction_date, installment_start_date,installment_end_date,method, interest_rate, installment_amount,interest_installment_amount, total_recovered_amount, code_master, active, trans_id,fm,fy) " +
                          "values(@idnew" + NewNumIndex + ",(select id from employees where EmpId=" + emp_code + ")," + emp_code + ",(select d.id from Designations d inner join Employees e on d.id=e.CurrentDesignation where e.EmpId=" + emp_code + "),(select id from pr_loan_master where loan_description='" + loan_description + "' and active=1)," + total_amount + "" +
                          "," + total_installment + "," + remaining_installment + "," + principal_installment + "," + interest_installment + ",0,'" + sanction_date + "','" + installment_start_date + "','" + installment_end_date + "','" + method + "'," + interest_rate + "," + installment_amount + "," + interest_installment_amount + ",0,(select id from All_Masters where Name='" + code_master + "' and Active=1),1,@transidnew,'" + sFm + "'," + fy + ");";

                            sbqry.Append(qry);
                        }

                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans", "@idnew" + NewNumIndex.ToString(), ""));

                    }

                    //check subloans details empty or not
                    if (loanchilddetails != null && loancode != @PrConstants.PF_LOANST1_CODE && loancode != @PrConstants.PF_LOANST2_CODE && loancode != @PrConstants.PF_LOANLT1_CODE && loancode != @PrConstants.PF_LOANLT2_CODE && loancode != @PrConstants.PF_LOANLT3_CODE && loancode != @PrConstants.PF_LOANLT4_CODE && loancode != PrConstants.PF_LOAN1_CODE && loancode != PrConstants.PF_LOAN2_CODE && loancode != PrConstants.FESTIVAL_LOAN_CODE)
                    {

                        foreach (var item in loanchilddetails)
                        {

                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                            //int slno = Convert.ToInt32(item.slno);
                            DateTime d = DateTime.Parse(item.date_disburse);
                            string date_disburse = d.ToString("yyyy-MM-dd");
                            string loan_amount = item.loan_amount;
                            int os_interest_amount = 0;
                            Double interest_rate2 = Convert.ToDouble(item.interest_rate);
                            Double interest_accured = Convert.ToDouble(item.interest_accured);
                            Double principal_amount_recovered = Convert.ToDouble(item.principal_amount_recovered);
                            Double interest_amount_recovered = Convert.ToDouble(item.interest_amount_recovered);
                            int priority = Convert.ToInt32(item.priority);
                            Double firstmonthinterest = Convert.ToDouble(loan_amount) * interest_rate2;
                            //int div_fm_interest = 100 * 12;
                            //Double os_this_month_interest = firstmonthinterest / div_fm_interest;
                            Double os_this_month_interest = 0;
                            Double os_total_amount = Convert.ToDouble(loan_amount) + Convert.ToDouble(os_interest_amount);

                            //first principle and first Interest start dates and end dates
                            if (priority == 1)
                            {
                                string principal_start_date = date_disburse; // datedisburse
                                string principal_end_date = Convert.ToDateTime(principal_start_date).AddMonths(Fpriority_prininstall - 1).ToString("yyyy-MM-dd");  //pricipalstartdate+144
                                string interest_start_date = Convert.ToDateTime(date_disburse).AddMonths(Convert.ToInt32(principal_installment)).ToString("yyyy-MM-dd");//dd+193
                                string interest_end_date = Convert.ToDateTime(interest_start_date).AddMonths(Fpriority_intinstall - 1).ToString("yyyy-MM-dd"); //isd+36

                                if (id == "")
                                {
                                    qry = "Insert into pr_emp_adv_loans_child(id,emp_adv_loans_mid,slno,date_disburse," +
                                        "loan_amount,interest_rate,interest_accured,principal_amount_recovered," +
                                        "interest_amount_recovered,total_amount_recovered,priority,principal_start_date," +
                                        "principal_end_date,interest_start_date,interest_end_date,total_principal_installments," +
                                        "total_interest_installments,os_principal_amount,os_interest_amount," +
                                        "os_this_month_interest,os_total_amount,principal_recovered_flag,interest_recovered_flag,active,trans_id,loan_sl_no) " +
                                   "values(@idnew" + NewNumIndex + "," +
                                   "(select id from pr_emp_adv_loans where emp_code=" + emp_code + " and active=1 and loan_type_mid=(select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id where l.emp_code=" + emp_code + " and m.loan_description='" + loan_description + "' and l.active=1))," + priority + "," +
                                   "'" + date_disburse + "'," + loan_amount + "," + interest_rate2 + "," + interest_accured + "," + principal_amount_recovered + "," +
                                   "" + interest_amount_recovered + "," + total_amount_recovered + "," + priority + ",'" + principal_start_date + "'," +
                                   "'" + principal_end_date + "','" + interest_start_date + "','" + interest_end_date + "'," + Fpriority_prininstall + "," + Fpriority_intinstall + "," + loan_amount + "," + os_interest_amount + "," + os_this_month_interest + "," + os_total_amount + ",0,0,1,@transidnew," + loansno + ");";
                                    sbqry.Append(qry);
                                }
                                else
                                {
                                    qry = "update pr_emp_adv_loans_child set active=0 where emp_adv_loans_mid=" + id + " and priority=1;";
                                    ////sbqry.Append(qry);
                                    sbqry.Append(qry);
                                    //4. transaction touch
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_adv_loans_child", "@idnew" + NewNumIndex.ToString(), ""));
                                    qry = "Insert into pr_emp_adv_loans_child(id,emp_adv_loans_mid,slno,date_disburse,loan_amount,interest_rate,interest_accured,principal_amount_recovered,interest_amount_recovered,total_amount_recovered,priority,principal_start_date,principal_end_date,interest_start_date,interest_end_date,total_principal_installments,total_interest_installments,os_principal_amount,os_interest_amount,os_this_month_interest,os_total_amount,principal_recovered_flag,interest_recovered_flag,active,trans_id,loan_sl_no) " +
                                  "values(@idnew" + NewNumIndex + ",(select id from pr_emp_adv_loans where emp_code=" + emp_code + " and active=1 and loan_type_mid=(select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id where l.emp_code=" + emp_code + " and m.loan_description='" + loan_description + "' and l.active=1))," + priority + ",'" + date_disburse + "'," + loan_amount + "," + interest_rate2 + "," + interest_accured + "," + principal_amount_recovered + "," + interest_amount_recovered + "," + total_amount_recovered + "," + priority + ",'" + principal_start_date + "','" + principal_end_date + "','" + interest_start_date + "','" + interest_end_date + "'," + Fpriority_prininstall + "," + Fpriority_intinstall + "," + loan_amount + "," + os_interest_amount + "," + os_this_month_interest + "," + os_total_amount + ",0,0,1,@transidnew," + loansno + ");";
                                    sbqry.Append(qry);
                                }

                            }

                            //second principle and second interest start date and enddate
                            else if (priority == 2)
                            {
                                string principal_start_date = Convert.ToDateTime(date_disburse).AddMonths(Fpriority_prininstall).ToString("yyyy-MM-dd"); //dd+144+1
                                string principal_end_date = Convert.ToDateTime(principal_start_date).AddMonths(divpric_installents - 1).ToString("yyyy-MM-dd"); // psd+47
                                string interest_start_date = Convert.ToDateTime(date_disburse).AddMonths(Convert.ToInt32(Spriority_intinstall) + 1).ToString("yyyy-MM-dd"); //1ied+1
                                string interest_end_date = Convert.ToDateTime(interest_start_date).AddMonths(divintr_install - 1).ToString("yyyy-MM-dd"); //2isd+12

                                if (id == "")
                                {
                                    qry = "Insert into pr_emp_adv_loans_child(id,emp_adv_loans_mid,slno,date_disburse,loan_amount,interest_rate,interest_accured,principal_amount_recovered,interest_amount_recovered,total_amount_recovered,priority,principal_start_date,principal_end_date,interest_start_date,interest_end_date,total_principal_installments,total_interest_installments,os_principal_amount,os_interest_amount,os_this_month_interest,os_total_amount,principal_recovered_flag,interest_recovered_flag,active,trans_id,loan_sl_no) " +
                                  "values(@idnew" + NewNumIndex + ",(select id from pr_emp_adv_loans where emp_code=" + emp_code + " and active=1 and loan_type_mid=(select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id where l.emp_code=" + emp_code + " and m.loan_description='" + loan_description + "' and l.active=1))," + priority + ",'" + date_disburse + "'," + loan_amount + "," + interest_rate2 + "," + interest_accured + "," + principal_amount_recovered + "," + interest_amount_recovered + "," + total_amount_recovered + "," + priority + ",'" + principal_start_date + "','" + principal_end_date + "','" + interest_start_date + "','" + interest_end_date + "'," + divpric_installents + "," + divintr_install + "," + loan_amount + "," + os_interest_amount + "," + os_this_month_interest + "," + os_total_amount + ",0,0,1,@transidnew," + loansno + ");";
                                    sbqry.Append(qry);
                                }
                                else
                                {
                                    qry = "update pr_emp_adv_loans_child set active=0 where emp_adv_loans_mid=" + id + " and priority=2;";
                                    ////sbqry.Append(qry);
                                    sbqry.Append(qry);
                                    //4. transaction touch
                                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_adv_loans_child", "@idnew" + NewNumIndex.ToString(), ""));
                                    qry = "Insert into pr_emp_adv_loans_child(id,emp_adv_loans_mid,slno,date_disburse,loan_amount,interest_rate,interest_accured,principal_amount_recovered,interest_amount_recovered,total_amount_recovered,priority,principal_start_date,principal_end_date,interest_start_date,interest_end_date,total_principal_installments,total_interest_installments,os_principal_amount,os_interest_amount,os_this_month_interest,os_total_amount,principal_recovered_flag,interest_recovered_flag,active,trans_id,loan_sl_no) " +
                                   "values(@idnew" + NewNumIndex + ",(select id from pr_emp_adv_loans where emp_code=" + emp_code + " and active=1 and loan_type_mid=(select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id where l.emp_code=" + emp_code + " and m.loan_description='" + loan_description + "' and l.active=1))," + priority + ",'" + date_disburse + "'," + loan_amount + "," + interest_rate2 + "," + interest_accured + "," + principal_amount_recovered + "," + interest_amount_recovered + "," + total_amount_recovered + "," + priority + ",'" + principal_start_date + "','" + principal_end_date + "','" + interest_start_date + "','" + interest_end_date + "'," + divpric_installents + "," + divintr_install + "," + loan_amount + "," + os_interest_amount + "," + os_this_month_interest + "," + os_total_amount + ",0,0,1,@transidnew," + loansno + ");";
                                    sbqry.Append(qry);
                                }

                            }


                            //4. transaction touch
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_child", "@idnew" + NewNumIndex.ToString(), ""));

                        }


                    }

                    //if subloans details not updated then update subloans mid 
                    else if (values.Loancols != null && loancode != @PrConstants.PF_LOANST1_CODE && loancode != @PrConstants.PF_LOANST2_CODE && loancode != @PrConstants.PF_LOANLT1_CODE && loancode != @PrConstants.PF_LOANLT2_CODE && loancode != @PrConstants.PF_LOANLT3_CODE && loancode != @PrConstants.PF_LOANLT4_CODE && loancode != PrConstants.PF_LOAN1_CODE && loancode != PrConstants.PF_LOAN2_CODE && loancode != PrConstants.FESTIVAL_LOAN_CODE)
                    {

                        qry = "select top 1 id from pr_emp_adv_loans where emp_code=" + emp_code + " and loan_type_mid=(select top 1 lm.id from pr_emp_adv_loans al join pr_loan_master lm on al.loan_type_mid=lm.id where lm.loan_description='" + loan_description + "' and al.emp_code=" + emp_code + " and al.active=1) order by id desc";
                        DataTable pidrow = await _sha.Get_Table_FromQry(qry);
                        string nid = "";
                        foreach (DataRow d in pidrow.Rows)
                        {
                            nid = d["id"].ToString();
                        }
                        if (nid != "")
                        {
                            qry = "update pr_emp_adv_loans_child set emp_adv_loans_mid= @idnew" + NewNumIndex + " where emp_adv_loans_mid=" + nid + "";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", "@idnew" + NewNumIndex.ToString(), ""));
                        }

                    }


                    if (loancode == @PrConstants.PF_LOANST1_CODE || loancode == @PrConstants.PF_LOANST2_CODE || loancode == @PrConstants.PF_LOANLT1_CODE || loancode == @PrConstants.PF_LOANLT2_CODE || loancode == @PrConstants.PF_LOANLT3_CODE || loancode == @PrConstants.PF_LOANLT4_CODE || loancode == PrConstants.PF_LOAN1_CODE || loancode == PrConstants.PF_LOAN2_CODE || loancode == PrConstants.FESTIVAL_LOAN_CODE)
                    {

                        if (id == "")
                        {
                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                            qry = "Insert into pr_emp_adv_loans_child(id,emp_adv_loans_mid,slno,date_disburse,loan_amount,interest_rate,interest_accured,principal_amount_recovered,interest_amount_recovered,total_amount_recovered,priority,principal_start_date,principal_end_date,interest_start_date,interest_end_date,total_principal_installments,total_interest_installments,os_principal_amount,os_interest_amount,os_this_month_interest,os_total_amount,principal_recovered_flag,interest_recovered_flag,active,trans_id,loan_sl_no) " +
                                      "values(@idnew" + NewNumIndex + ",(select id from pr_emp_adv_loans where emp_code=" + emp_code + " and active=1 and loan_type_mid=(select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id where l.emp_code=" + emp_code + " and m.loan_description='" + loan_description + "' and l.active=1)), 1 ,'" + sanction_date + "'," + total_amount + "," + interest_rate + ", 0 , 0 , 0 ,0 , 1 ,'" + installment_start_date + "','" + installment_end_date + "',null,null," + principal_installment + ",0 ," + total_amount + ",0  ,0 ," + total_amount + ",0,0,1,@transidnew," + loansno + ");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_child", "@idnew" + NewNumIndex.ToString(), ""));
                        }
                        else
                        {
                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                            qry = "update pr_emp_adv_loans_child set active=0 where emp_adv_loans_mid=" + id + " and priority=1;";
                            ////sbqry.Append(qry);
                            sbqry.Append(qry);
                            qry = "Insert into pr_emp_adv_loans_child(id,emp_adv_loans_mid,slno,date_disburse,loan_amount,interest_rate,interest_accured,principal_amount_recovered,interest_amount_recovered,total_amount_recovered,priority,principal_start_date,principal_end_date,interest_start_date,interest_end_date,total_principal_installments,total_interest_installments,os_principal_amount,os_interest_amount,os_this_month_interest,os_total_amount,principal_recovered_flag,interest_recovered_flag,active,trans_id,loan_sl_no) " +
                                     "values(@idnew" + NewNumIndex + ",(select id from pr_emp_adv_loans where emp_code=" + emp_code + " and active=1 and loan_type_mid=(select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id where l.emp_code=" + emp_code + " and m.loan_description='" + loan_description + "' and l.active=1)), 1 ,'" + sanction_date + "'," + total_amount + "," + interest_rate + ", 0 , 0 , 0 ,0 , 1 ,'" + installment_start_date + "','" + installment_end_date + "',null,null," + principal_installment + ",0 ," + total_amount + ",0  ,0 ," + total_amount + ",0,0,1,@transidnew," + loansno + ");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_child", "@idnew" + NewNumIndex.ToString(), ""));

                        }
                    }

                    //@idnew
                    sbqry.Append("Select @idnew0;");
                    int loanPkId = await _sha.Run_INS_ExecuteScalar(sbqry.ToString());

                    StringBuilder sbqry1 = new StringBuilder();

                    sbqry1.Append(GenNewTransactionString());

                    await LoadLedgerReportNEW(loanPkId, 0);
                    int fys = _LoginCredential.FY;
                    int currentmonth = DateTime.Now.Month;
                    string fms = Convert.ToString(fys) + "-" + Convert.ToString(currentmonth) + "-" + "01";
                    int Eyear = 0001;
                    int Fyear = 0001;
                    int NewNumIndex1 = 0;
                    Eyear = fys;
                    Fyear = fys - 1;

                    string qrys = "select sum(ledger.interest_accrued )  as finaceyearInterest_Amount , sum(ledger.total_paid) as princeple_amount from pr_emp_adv_loans advloans join pr_emp_adv_loans_child child on advloans.id = child.emp_adv_loans_mid join pr_emp_loans_projection ledger on child.id = ledger.emp_loan_child_id where ledger.emp_code = '" + values.EntityId + "' and advloans.active = 1 and ledger.fm between DATEFROMPARTS (" + Fyear + ", 04, 01 ) and DATEFROMPARTS (" + Eyear + ", 03, 31 ) and ledger.loan_type_mid in (4,5,6,7,8,9,10,11,12,13) ";

                    DataTable dts = await _sha.Get_Table_FromQry(qrys);
                    //getting values from loan ledger
                    if (dts.Rows.Count > 0)
                    {
                        foreach (DataRow d in dts.Rows)
                        {
                            if (!d.IsNull("princeple_amount"))
                            {
                                principal = Convert.ToDouble(d["princeple_amount"]);
                            }
                            else
                            {
                                principal = 0;
                            }
                            if (!d.IsNull("finaceyearInterest_Amount"))
                            {
                                inaccure = Convert.ToDouble(d["finaceyearInterest_Amount"]);
                            }
                            else
                            {
                                inaccure = 0;
                            }

                        }


                    }
                    if (dts.Rows.Count > 0)
                    {
                        foreach (DataRow d in dts.Rows)
                        {
                            if (!d.IsNull("princeple_amount"))
                            {
                                //checking and updating inpersonal deductions
                                string updateQry = "select id,emp_code,m_id,amount from pr_emp_perdeductions where  active=1 and emp_code='" + values.EntityId + "' and m_id = (select id from pr_deduction_field_master where name in ('Housing Loan principle (financial Year recovered amount)')) and amount>0 and fy='" + fys + "';";
                                DataTable dts1 = await _sha.Get_Table_FromQry(updateQry);

                                if (dts1.Rows.Count > 0)
                                {

                                    NewNumIndex1++;

                                    sbqry1.Append(GetNewNumStringArr("pr_emp_perdeductions", NewNumIndex1));
                                    string upQry = "update pr_emp_perdeductions set amount='" + principal + "' where active=1 and emp_code='" + values.EntityId + "' and m_id = (select id from pr_deduction_field_master where name in ('Housing Loan principle (financial Year recovered amount)')) and amount>0 and fy='" + fys + "';";
                                    sbqry1.Append(upQry);
                                    sbqry1.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_perdeductions", "@idnew" + NewNumIndex1.ToString(), ""));
                                }
                                else
                                {
                                    NewNumIndex1++;
                                    sbqry1.Append(GetNewNumStringArr("pr_emp_perdeductions", NewNumIndex1));
                                    string inQry = " insert into pr_emp_perdeductions values(@idnew" + NewNumIndex1 + "," + fys + ",'" + fms + "', (select id from employees where empid = '" + values.EntityId + "'),'" + values.EntityId + "',(select id from pr_deduction_field_master where name = 'Housing Loan principle (financial Year recovered amount)'),'per_ded'," + principal + ",'24(b)',1,@transidnew);";
                                    sbqry1.Append(inQry);
                                    sbqry1.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_perdeductions", "@idnew" + NewNumIndex1.ToString(), ""));
                                }
                            }
                        }
                    }
                    if (dts.Rows.Count > 0)
                    {
                        foreach (DataRow d in dts.Rows)
                        {
                            if (!d.IsNull("finaceyearInterest_Amount"))
                            {
                                string updateQry2 = "select id,emp_code,m_id,amount from pr_emp_perdeductions where  active=1 and emp_code='" + values.EntityId + "' and m_id = (select id from pr_deduction_field_master where name='Housing Loan Interest (financial interest amount)') and amount>0 and fy='" + fys + "';";
                                DataTable dts2 = await _sha.Get_Table_FromQry(updateQry2);

                                if (dts2.Rows.Count > 0)
                                {
                                    NewNumIndex1++;
                                    sbqry1.Append(GetNewNumStringArr("pr_emp_perdeductions", NewNumIndex1));
                                    string upQry2 = "update pr_emp_perdeductions set amount='" + inaccure + "' where active = 1 and emp_code = '" + values.EntityId + "' and m_id = (select id from pr_deduction_field_master where name = 'Housing Loan Interest (financial interest amount)') and amount> 0 and fy = '" + fys + "'; ";
                                    sbqry1.Append(upQry2);
                                    sbqry1.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_perdeductions", "@idnew" + NewNumIndex1.ToString(), ""));
                                }
                                else
                                {
                                    NewNumIndex1++;
                                    sbqry1.Append(GetNewNumStringArr("pr_emp_perdeductions", NewNumIndex1));
                                    string inQry2 = " insert into pr_emp_perdeductions values(@idnew" + NewNumIndex1 + "," + fys + ",'" + fms + "', (select id from employees where empid = '" + values.EntityId + "'),'" + values.EntityId + "',(select id from pr_deduction_field_master where name ='Housing Loan Interest (financial interest amount)'),'per_ded'," + inaccure + ",'Section80C',1,@transidnew);";
                                    sbqry1.Append(inQry2);
                                    sbqry1.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_perdeductions", "@idnew" + NewNumIndex1.ToString(), ""));
                                }
                            }
                        }
                    }
               
                    await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry1.ToString());
                string qryln = "update new_num set last_num=" + loansno + " where table_name='loan_sl_no'";
                await _sha.Run_UPDDEL_ExecuteNonQuery(qryln.ToString());
                res = "I#Loan #Created Successfully.";
                }
        
                return res;


            }
            catch (Exception e)
            {
                string error = e.Message;
                return "E#Error:#" + error;
            }
        }

        public async Task<bool> LoanLedgerReport(CommonPostDTO values)
        {

            //LoadLedgerReportNEW();


            //Declaring Local Variables
            int Emp_code;
            int AmountIssued;
            string LoanName;
            DateTime LoanSanctiodate;

            int daysLeft;
            DateTime LoanStartDate;
            DateTime fm;
            Double InterestRate = 0;
            int LoanOpening = 0;
            int LoanRepaid = 0;
            int InstallmentNumber = 0;
            int InterestInstallmentNumber = 0;
            int LoanClosing = 0;
            int InterestOpening = 0;
            int InterestOpening2 = 0;
            int InterestAccured = 0;
            int InterestAccured2 = 0;
            int InterestRepaid = 0;
            int IntRepaid2 = 0;
            int InterestClosing = 0;
            int InterestClosing2 = 0;
            int InstallmentRepaid = 0;
            int NewNumIndex = 0;
            string method = "";
            int fy;
            int Value = 0;
            string loan_id = "";

            int emp_id;
            int emp_loan_id;
            int loan_type_mid;

            bool Boolresult = false;

            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());

            //Assigning Values to local variables from UI
            var LoanDetails = values.Loancols;
            var ChildLoanDetails = values.objectLD;
            InterestInstallmentNumber = int.Parse(LoanDetails[3].Value);
            loan_id = values.StringData;
            // for 1 loan ledger.
            if (ChildLoanDetails == null || ChildLoanDetails.Count == 1)
            {

                Emp_code = values.EntityId;
                AmountIssued = int.Parse(LoanDetails[1].Value);
                LoanName = LoanDetails[0].Value;

                LoanSanctiodate = DateTime.Parse(LoanDetails[4].Value);
                method = LoanDetails[5].Value;


                LoanOpening = int.Parse(LoanDetails[1].Value);
                InterestRate = Double.Parse(LoanDetails[6].Value);
                LoanClosing = int.Parse(LoanDetails[1].Value);


                if (loan_id == @PrConstants.PF_LOANST1_CODE || loan_id == @PrConstants.PF_LOANST2_CODE || loan_id == @PrConstants.PF_LOANLT1_CODE || loan_id == @PrConstants.PF_LOANLT2_CODE || loan_id == @PrConstants.PF_LOANLT3_CODE || loan_id == @PrConstants.PF_LOANLT4_CODE || loan_id == PrConstants.PF_LOAN1_CODE || loan_id == PrConstants.PF_LOAN2_CODE || loan_id == PrConstants.FESTIVAL_LOAN_CODE)
                {
                    if (loan_id == @PrConstants.PF_LOANST1_CODE || loan_id == @PrConstants.PF_LOANST2_CODE || loan_id == @PrConstants.PF_LOANLT1_CODE || loan_id == @PrConstants.PF_LOANLT2_CODE || loan_id == @PrConstants.PF_LOANLT3_CODE || loan_id == @PrConstants.PF_LOANLT4_CODE || loan_id == PrConstants.PF_LOAN1_CODE || loan_id == PrConstants.PF_LOAN2_CODE && method == "Interest With Equal Installments")
                    {
                        LoanStartDate = DateTime.Parse(LoanDetails[10].Value);
                        fm = DateTime.Parse(LoanDetails[4].Value);
                        daysLeft = DateTime.DaysInMonth(fm.Year, fm.Month) - fm.Day;
                        //Interest calculation per month
                        InterestAccured = int.Parse(Math.Round(((LoanOpening * InterestRate) / (100 * 365)) * daysLeft).ToString());
                        InterestClosing = InterestAccured + InterestOpening;

                    }
                    else
                    {
                        LoanStartDate = DateTime.Parse(LoanDetails[10].Value);
                        fm = DateTime.Parse(LoanDetails[4].Value);
                    }

                }
                else
                {
                    LoanStartDate = DateTime.Parse(LoanDetails[11].Value);
                    fm = DateTime.Parse(ChildLoanDetails[0].date_disburse);
                }


                fy = fm.Year;

                //financial months caluculations
                DateTime fyear = fm.AddYears(1);
                int fyear2 = fyear.Year;
                int fmonth = 03;
                int fday = fm.Day;

                //Interest calculation fro first month
                int Noofdaysinmonth;
                daysLeft = DateTime.DaysInMonth(fm.Year, fm.Month) - fm.Day;
                InterestAccured = int.Parse(Math.Round(((AmountIssued * InterestRate) / (100 * 365)) * daysLeft).ToString());
                InterestClosing = InterestAccured + InterestOpening;


                int _1stloopforinterest = (LoanStartDate.Month + LoanStartDate.Year * 12) - (fm.Month + fm.Year * 12);


                DateTime newfyear = new DateTime(fyear2, fmonth, fday);
                int noofmonthforloop = (newfyear.Month + newfyear.Year * 12) - (LoanStartDate.Month + LoanStartDate.Year * 12);


                string qry = "select l.emp_code,l.emp_id as empid, l.id as loanid, l.loan_type_mid from pr_emp_adv_loans l join pr_loan_master plm on l.loan_type_mid=plm.id where l.emp_code=" + Emp_code + " and plm.loan_id='" + loan_id + "' and l.active =1; ";

                DataTable dtempdetails = await _sha.Get_Table_FromQry(qry);
                emp_id = int.Parse(dtempdetails.Rows[0]["empid"].ToString());
                emp_loan_id = int.Parse(dtempdetails.Rows[0]["loanid"].ToString());
                loan_type_mid = int.Parse(dtempdetails.Rows[0]["loan_type_mid"].ToString());

                //updating the previous records with same emp_code and emp_loan_id.
                qry = "Update pr_emp_loans_projection set active=0 where emp_code=" + Emp_code + "and loan_type_mid=" + loan_type_mid + ";";
                sbqry.Append(qry);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));

                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                      "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ", " + InterestOpening + ", " + InterestAccured + ", " + InterestRepaid + ", " + InterestClosing + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                sbqry.Append(qry);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", "@idnew" + NewNumIndex.ToString(), ""));
                //await _sha.Run_UPDDEL_ExecuteNonQuery(qry);

                LoanRepaid = int.Parse(LoanDetails[7].Value);

                //loop for generate report for finacial year-months
                //if loan id ==pfl1,pfl2,fest
                if (loan_id == @PrConstants.PF_LOANST1_CODE || loan_id == @PrConstants.PF_LOANST2_CODE || loan_id == @PrConstants.PF_LOANLT1_CODE || loan_id == @PrConstants.PF_LOANLT2_CODE || loan_id == @PrConstants.PF_LOANLT3_CODE || loan_id == @PrConstants.PF_LOANLT4_CODE || loan_id == PrConstants.PF_LOAN1_CODE || loan_id == PrConstants.PF_LOAN2_CODE || loan_id == PrConstants.FESTIVAL_LOAN_CODE)
                {
                    if (loan_id == @PrConstants.PF_LOANST1_CODE || loan_id == @PrConstants.PF_LOANST2_CODE || loan_id == @PrConstants.PF_LOANLT1_CODE || loan_id == @PrConstants.PF_LOANLT2_CODE || loan_id == @PrConstants.PF_LOANLT3_CODE || loan_id == @PrConstants.PF_LOANLT4_CODE || loan_id == PrConstants.PF_LOAN1_CODE || loan_id == PrConstants.PF_LOAN2_CODE && method == PrConstants.PF_LOAN_METHOD)
                    {

                        for (int i = 1; i < _1stloopforinterest; i++)
                        {
                            LoanOpening = LoanClosing;
                            if (LoanOpening != 0)
                            {
                                //Calculations
                                fm = fm.AddMonths(1);
                                LoanRepaid = 0;

                                Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);
                                InterestAccured = int.Parse(Math.Round(((LoanOpening * InterestRate) / (100 * 365)) * Noofdaysinmonth).ToString());
                                InterestRepaid = InterestAccured;

                                NewNumIndex++;
                                sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                                qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                      "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ",   0  ,   0  , 0 ,  0  , " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));

                            }

                        }


                        for (int i = 0; i <= noofmonthforloop; i++)
                        {
                            LoanOpening = LoanClosing;
                            if (LoanOpening > 0)
                            {
                                fm = fm.AddMonths(1);
                                InstallmentRepaid = int.Parse(LoanDetails[7].Value);

                                Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);
                                InterestAccured = int.Parse(Math.Round(((LoanOpening * InterestRate) / (100 * 365)) * Noofdaysinmonth).ToString());
                                InterestRepaid = InterestAccured;
                                LoanRepaid = InstallmentRepaid - InterestAccured;
                                LoanClosing = LoanOpening - LoanRepaid;
                                InstallmentNumber++;

                                NewNumIndex++;
                                sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                                qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                      "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ",   0  , " + InterestAccured + "  , " + InterestRepaid + ",  0  , " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));


                            }


                        }


                    }

                    else
                    {
                        for (int i = 1; i < _1stloopforinterest; i++)
                        {
                            LoanOpening = LoanClosing;
                            if (LoanOpening != 0)
                            {
                                //Calculations
                                fm = fm.AddMonths(1);
                                LoanRepaid = 0;

                                NewNumIndex++;
                                sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                                qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                      "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ",   0  ,   0  , 0 ,  0  , " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));

                            }

                        }


                        for (int i = 0; i <= noofmonthforloop; i++)
                        {
                            LoanOpening = LoanClosing;
                            if (LoanOpening != 0)
                            {
                                //Calculations
                                fm = fm.AddMonths(1);
                                LoanRepaid = int.Parse(LoanDetails[7].Value);
                                LoanClosing = LoanOpening - LoanRepaid;
                                InterestOpening = InterestClosing;
                                Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);

                                //Interest calculation per month
                                InterestAccured = int.Parse(Math.Round(((LoanOpening * InterestRate) / (100 * 365)) * Noofdaysinmonth).ToString());
                                //int y = (int)Math.Ceiling(2.4);
                                InterestClosing = InterestAccured + InterestOpening;
                                InstallmentRepaid = LoanRepaid;
                                InstallmentNumber++;

                                NewNumIndex++;
                                sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                                qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                      "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ",   0  ,   0  , 0 ,  0  , " + InstallmentRepaid + ", 1, @transidnew, "+loansno+");";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));

                            }
                        }
                    }

                }


                //if loan id !=pfl1,pfl2,fest
                else
                {

                    for (int i = 1; i < _1stloopforinterest; i++)
                    {
                        LoanOpening = LoanClosing;
                        if (LoanOpening != 0)
                        {


                            //Calculations
                            fm = fm.AddMonths(1);
                            LoanRepaid = 0;

                            InterestOpening = InterestClosing;
                            Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);

                            //Interest calculation per month
                            InterestAccured = int.Parse(Math.Round(((AmountIssued * InterestRate) / (100 * 365)) * Noofdaysinmonth).ToString());
                            //int y = (int)Math.Ceiling(2.4);
                            InterestClosing = InterestAccured + InterestOpening;


                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                            qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                  "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ",   0  ,   0  , 0 ,  0  , " + InstallmentRepaid + ", 1, @transidnew, "+loansno+");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));

                        }

                    }



                    for (int i = 0; i <= noofmonthforloop; i++)
                    {
                        LoanOpening = LoanClosing;
                        //for principal amount calculations
                        if (LoanOpening != 0)
                        {
                            InstallmentNumber++;
                            //Calculations
                            fm = fm.AddMonths(1);
                            Value = LoanOpening - LoanRepaid;
                            if (Value < 0)
                            {
                                LoanRepaid = LoanRepaid + (Value);
                                LoanClosing = 0;
                            }
                            else
                            {
                                LoanRepaid = int.Parse(LoanDetails[7].Value);
                                LoanClosing = LoanOpening - LoanRepaid;
                            }

                            InterestOpening = InterestClosing;
                            Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);

                            //Interest calculation per month
                            InterestAccured = int.Parse(Math.Round(((AmountIssued * InterestRate) / (100 * 365)) * Noofdaysinmonth).ToString());
                            //int y = (int)Math.Ceiling(2.4);
                            InterestClosing = InterestAccured + InterestOpening;
                            InstallmentRepaid = LoanRepaid;


                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", NewNumIndex));
                            qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                  "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ", " + InterestOpening + ", " + InterestAccured + ", " + InterestRepaid + ", " + InterestClosing + ", " + InstallmentRepaid + ", 1, @transidnew, "+loansno+");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", "@idnew" + NewNumIndex.ToString(), ""));

                        }

                        if (Value < 0)
                        {
                            InterestRepaid = (int)Math.Ceiling(Convert.ToDecimal(InterestClosing / InterestInstallmentNumber));
                            Value = 0;
                        }

                        // interest amount calculations
                        if (LoanClosing == 0)
                        {
                            if (InterestClosing != 0)
                            {
                                InstallmentNumber++;
                                fm = fm.AddMonths(1);
                                int Value2 = InterestClosing - InterestRepaid;
                                if (Value2 < 0)
                                {
                                    InterestRepaid = InterestRepaid + (Value2);
                                    InterestOpening = InterestClosing;
                                    InterestClosing = 0;

                                }

                                else
                                {
                                    InterestOpening = InterestClosing;
                                    InterestClosing = InterestOpening - InterestRepaid;
                                }

                                InstallmentRepaid = InterestRepaid;

                                NewNumIndex++;
                                sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                                qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                      "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", 0, 0,  0, " + InterestOpening + ", 0, " + InterestRepaid + ", " + InterestClosing + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));
                            }
                        }
                    }
                }



            }

            //for 2 loans ledger.
            else
            {
                LoanStartDate = DateTime.Parse(LoanDetails[11].Value);
                Emp_code = values.EntityId;
                fm = DateTime.Parse(ChildLoanDetails[0].date_disburse);
                fy = fm.Year;

                //financial months caluculations
                DateTime fyear = fm.AddYears(1);

                //for financial year
                int fyear2 = fyear.Year;
                int fmonth = 03;
                int fday = fm.Day;
                DateTime newfyear = new DateTime(fyear2, fmonth, fday);


                //for first loop without payment , only interest calculation i.e loan start date.
                int _1stloopforinterest = (LoanStartDate.Month + LoanStartDate.Year * 12) - (fm.Month + fm.Year * 12);

                //for loop for no of months from loan disburs
                int noofmonthforloop = (newfyear.Month + newfyear.Year * 12) - (LoanStartDate.Month + LoanStartDate.Year * 12) + 1;

                string qry = "select l.emp_code,lc.loan_amount,l.emp_id as empid, lc.id as loanid, lc.interest_rate, l.loan_type_mid from pr_emp_adv_loans l join pr_emp_adv_loans_child lc on l.id=lc.emp_adv_loans_mid join pr_loan_master plm on l.loan_type_mid=plm.id where l.emp_code=" + Emp_code + " and plm.loan_id='" + loan_id + "' and l.active =1; ";

                DataTable dtempdetails = await _sha.Get_Table_FromQry(qry);
                //for first loan eg:5,00,000
                Emp_code = values.EntityId;
                emp_id = int.Parse(dtempdetails.Rows[0]["empid"].ToString());
                emp_loan_id = int.Parse(dtempdetails.Rows[0]["loanid"].ToString());
                loan_type_mid = int.Parse(dtempdetails.Rows[0]["loan_type_mid"].ToString());
                AmountIssued = int.Parse(dtempdetails.Rows[0]["loan_amount"].ToString());
                InterestRate = Double.Parse(dtempdetails.Rows[0]["interest_rate"].ToString());
                LoanSanctiodate = DateTime.Parse(LoanDetails[4].Value);

                LoanName = LoanDetails[0].Value;
                LoanOpening = int.Parse(dtempdetails.Rows[0]["loan_amount"].ToString());
                LoanClosing = int.Parse(dtempdetails.Rows[0]["loan_amount"].ToString());



                // for second loan eg:15,00,000
                int emp_loan_id2 = int.Parse(dtempdetails.Rows[1]["loanid"].ToString());
                int AmountIssued2 = int.Parse(dtempdetails.Rows[1]["loan_amount"].ToString());
                Double InterestRate2 = Double.Parse(dtempdetails.Rows[1]["interest_rate"].ToString());
                int LoanOpening2 = int.Parse(dtempdetails.Rows[1]["loan_amount"].ToString());
                int LoanClosing2 = int.Parse(dtempdetails.Rows[1]["loan_amount"].ToString());

                //for first loan.
                daysLeft = DateTime.DaysInMonth(fm.Year, fm.Month) - fm.Day;
                //Interest calculation per month
                InterestAccured = int.Parse(Math.Round(((LoanOpening * InterestRate) / (100 * 365)) * daysLeft).ToString());
                InterestClosing = InterestAccured + InterestOpening;

                //for secondloan
                InterestAccured2 = int.Parse(Math.Round(((LoanOpening2 * InterestRate2) / (100 * 365)) * daysLeft).ToString());
                InterestClosing2 = InterestAccured2 + InterestOpening2;


                //updating the previous records with same emp_code and emp_loan_id.
                qry = "Update pr_emp_loans_projection set active=0 where emp_code=" + Emp_code + "and loan_type_mid=" + loan_type_mid + ";";
                sbqry.Append(qry);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));

                //for first loan 15,00,000 first row insertion that is zero interest.
                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", NewNumIndex));
                qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                      "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ", " + InterestOpening + ", " + InterestAccured + ", " + InterestRepaid + ", " + InterestClosing + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                sbqry.Append(qry);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", "@idnew" + NewNumIndex.ToString(), ""));

                //for second loan 5,00,000 first row insertion that is zero interest.
                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", NewNumIndex));
                qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                      "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id2 + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued2 + ", '" + LoanSanctiodate + "', " + InterestRate2 + ", " + InstallmentNumber + ", " + LoanOpening2 + ", " + LoanRepaid + ", " + LoanClosing2 + ", " + InterestOpening2 + ", " + InterestAccured2 + ", " + InterestRepaid + ", " + InterestClosing2 + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                sbqry.Append(qry);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", "@idnew" + NewNumIndex.ToString(), ""));


                //for first loop calculate interest upto loan start date, on loan start date installment to be paid.

                for (int i = 1; i < _1stloopforinterest; i++)
                {
                    LoanOpening = LoanClosing;
                    LoanOpening2 = LoanClosing2;
                    if (LoanOpening != 0)
                    {


                        //Calculations
                        fm = fm.AddMonths(1);
                        LoanRepaid = 0;

                        InterestOpening = InterestClosing;
                        InterestOpening2 = InterestClosing2;

                        int Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);

                        //Interest calculation per month
                        InterestAccured = int.Parse(Math.Round(((LoanOpening * InterestRate) / (100 * 365)) * Noofdaysinmonth).ToString());
                        //int y = (int)Math.Ceiling(2.4);
                        InterestClosing = InterestAccured + InterestOpening;

                        //for secondloan
                        InterestAccured2 = int.Parse(Math.Round(((LoanOpening2 * InterestRate2) / (100 * 365)) * Noofdaysinmonth).ToString());
                        InterestClosing2 = InterestAccured2 + InterestOpening2;

                        NewNumIndex++;
                        sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + "," + InterestOpening + "  ,   " + InterestAccured + " , 0 ,  " + InterestClosing + "  , " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                        sbqry.Append(qry);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));


                        NewNumIndex++;
                        sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_child", NewNumIndex));
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued2 + ", '" + LoanSanctiodate + "', " + InterestRate2 + ", " + InstallmentNumber + ", " + LoanOpening2 + ", " + LoanRepaid + ", " + LoanClosing2 + "," + InterestOpening2 + "  ,   " + InterestAccured2 + " , 0 ,  " + InterestClosing2 + "  , " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                        sbqry.Append(qry);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));

                    }

                }


                //loop for no of months from loan start date to financial month-year.
                for (int i = 0; i < noofmonthforloop; i++)
                {

                    LoanOpening = LoanClosing;
                    //for first principal amount calculations
                    if (LoanOpening > 0)
                    {
                        InstallmentNumber++;
                        //Calculations
                        fm = fm.AddMonths(1);
                        Value = LoanOpening - LoanRepaid;
                        if (Value < 0)
                        {
                            LoanRepaid = LoanRepaid + (Value);
                            LoanClosing = 0;
                        }
                        else
                        {
                            LoanRepaid = int.Parse(LoanDetails[7].Value);
                            LoanClosing = LoanOpening - LoanRepaid;
                        }

                        InterestOpening = InterestClosing;
                        int Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);

                        //Interest calculation per month
                        InterestAccured = int.Parse(Math.Round(((LoanOpening * InterestRate) / (100 * 365)) * Noofdaysinmonth).ToString());
                        //int y = (int)Math.Ceiling(2.4);
                        InterestClosing = InterestAccured + InterestOpening;
                        InstallmentRepaid = LoanRepaid;


                        NewNumIndex++;
                        sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", NewNumIndex));
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ", " + InterestOpening + ", " + InterestAccured + ", " + InterestRepaid + ", " + InterestClosing + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                        sbqry.Append(qry);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));



                        // for second loan ledger from second month

                        //interest Calculations for second loan untill first principle zero.

                        InterestOpening2 = InterestClosing2;
                        //Interest calculation per month
                        InterestAccured2 = int.Parse(Math.Round(((LoanOpening2 * InterestRate2) / (100 * 365)) * Noofdaysinmonth).ToString());
                        InterestClosing2 = InterestAccured2 + InterestOpening2;

                        NewNumIndex++;
                        sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", NewNumIndex));
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id2 + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued2 + ", '" + LoanSanctiodate + "', " + InterestRate2 + ", 0, " + LoanOpening2 + ", 0, " + LoanClosing2 + ", " + InterestOpening2 + ", " + InterestAccured2 + ", 0, " + InterestClosing2 + ", 0 , 1, @transidnew,"+loansno+");";
                        sbqry.Append(qry);
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id2.ToString(), ""));



                    }

                    //for second loan or principal amount calculations
                    else if (LoanOpening <= 0 && LoanOpening2 > 0)
                    {


                        LoanOpening2 = LoanClosing2;
                        //for principal amount calculations
                        if (LoanOpening2 > 0)
                        {
                            InstallmentNumber++;
                            //Calculations
                            fm = fm.AddMonths(1);
                            Value = LoanOpening2 - LoanRepaid;
                            if (Value < 0)
                            {
                                LoanRepaid = LoanRepaid + (Value);
                                LoanClosing2 = 0;
                            }
                            else
                            {
                                LoanRepaid = int.Parse(LoanDetails[7].Value);
                                LoanClosing2 = LoanOpening2 - LoanRepaid;
                            }


                            int Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);
                            InterestOpening2 = InterestClosing2;
                            //Interest calculation per month
                            InterestAccured2 = int.Parse(Math.Round(((LoanOpening2 * InterestRate2) / (100 * 365)) * Noofdaysinmonth).ToString());
                            //int y = (int)Math.Ceiling(2.4);
                            InterestClosing2 = InterestAccured2 + InterestOpening2;


                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", NewNumIndex));
                            qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                  "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ",  0, 0, 0, 0, 0, 0, 0, " + InterestClosing + ", 0, 1, @transidnew,"+loansno+");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));



                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", NewNumIndex));
                            qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                  "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id2 + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued2 + ", '" + LoanSanctiodate + "', " + InterestRate2 + ", " + InstallmentNumber + ", " + LoanOpening2 + ", " + LoanRepaid + ", " + LoanClosing2 + ", " + InterestOpening2 + ", " + InterestAccured2 + ", 0, " + InterestClosing2 + ", 0 , 1, @transidnew,"+loansno+");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id2.ToString(), ""));



                        }

                    }

                    if (Value < 0)
                    {
                        InterestRepaid = (int)Math.Ceiling(Convert.ToDecimal(InterestClosing + InterestClosing2)) / InterestInstallmentNumber;
                        Value = 0;
                        IntRepaid2 = InterestRepaid;
                    }
                    //for first loan interest ledger
                    else if (LoanOpening <= 0 && LoanOpening2 <= 0 && InterestClosing > 0)
                    {

                        if (InterestClosing != 0)
                        {

                            InstallmentNumber++;
                            fm = fm.AddMonths(1);
                            int Value2 = InterestClosing - InterestRepaid;
                            if (Value2 < 0)
                            {
                                InterestRepaid = InterestRepaid + (Value2);
                                InterestOpening = InterestClosing;
                                InterestClosing = 0;

                            }

                            else
                            {
                                InterestOpening = InterestClosing;
                                InterestClosing = InterestOpening - InterestRepaid;
                            }

                            InstallmentRepaid = InterestRepaid;

                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", NewNumIndex));
                            qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                  "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ",  " + InstallmentNumber + ", 0, 0, 0, " + InterestOpening + ", 0, " + InterestRepaid + ", " + InterestClosing + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));


                            //second loan interest as it is desplaying
                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", NewNumIndex));
                            qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                  "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ",  0, 0, 0, 0, 0, 0, 0, " + InterestClosing2 + ", 0, 1, @transidnew,"+loansno+");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));



                        }

                    }

                    //for second loan interest ledger
                    else if (LoanOpening <= 0 && LoanOpening2 <= 0 && InterestClosing <= 0)
                    {

                        if (InterestClosing2 != 0)
                        {
                            InterestRepaid = IntRepaid2;
                            InstallmentNumber++;
                            fm = fm.AddMonths(1);
                            int Value2 = InterestClosing2 - InterestRepaid;
                            if (Value2 < 0)
                            {
                                InterestRepaid = InterestRepaid + (Value2);
                                InterestOpening2 = InterestClosing2;
                                InterestClosing2 = 0;

                            }

                            else
                            {
                                InterestOpening2 = InterestClosing2;
                                InterestClosing2 = InterestOpening2 - InterestRepaid;
                            }

                            InstallmentRepaid = InterestRepaid;

                            NewNumIndex++;
                            sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", NewNumIndex));
                            qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id],loan_sl_no) " +
                                  "values(@idnew" + NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ",  " + InstallmentNumber + ", 0, 0, 0, " + InterestOpening2 + ", 0, " + InterestRepaid + ", " + InterestClosing2 + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));

                        }

                    }
                }

            }

            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                Boolresult = true;
            }
            return Boolresult;
        }

        private int _princClose = 0;
        private int _intrClose = 0;
        // private int _loanTotalPaid = 0;
        private int _loanInstallment = 0;
        private int _InstallmentNumber = 0;
        private int _NewNumIndex = 0;
        private int _noOfInterestInstallments;
        private string _installmentpart = "I";
        private StringBuilder _sbqry = new StringBuilder();

        public async Task LoadLedgerReportNEW(int loanPkId, int partPaymentId)
        {
            //int installmentAmount;
            double interestRate;
            DateTime dtAmountDisburse;
            DateTime dtInstallmentStart;


            string selQry = "";

            if (loanPkId != 0)
            {
                selQry = "select  ch.id as Child_loanId, l.emp_id, l.emp_code, ch.emp_adv_loans_mid, l.loan_type_mid, prlm.loan_id as loan_code,l.method, l.sanction_date, ch.date_disburse, ch.loan_amount, ch.interest_rate,l.installment_amount, ch.os_principal_amount, ch.os_interest_amount,l.installment_start_date,  l.interest_installment   " +
                "from pr_emp_adv_loans_child ch " +
                "join pr_emp_adv_loans l on ch.emp_adv_loans_mid = l.id " +
                "join pr_loan_master prlm on prlm.id= l.loan_type_mid " +
                "join pr_loan_master plm on l.loan_type_mid = plm.id where l.id=" + loanPkId + " and ch.active=1; ";

            }
            else if (partPaymentId != 0)
            {

                selQry = "  select ch.id as Child_loanId,l.id as loanid, l.emp_id, l.emp_code, ch.emp_adv_loans_mid, l.loan_type_mid, prlm.loan_id as loan_code,l.method, l.sanction_date, adj.cash_paid_on as date_disburse, ch.loan_amount, ch.interest_rate,l.installment_amount, ch.os_principal_amount, ch.os_interest_amount,  adj.cash_paid_on as installment_start_date,  l.interest_installment " +
                 " from pr_emp_adv_loans_child ch" +
                 " join pr_emp_adv_loans l on ch.emp_adv_loans_mid = l.id " +
                 "join pr_loan_master prlm on prlm.id= l.loan_type_mid " +
                 "join pr_emp_adv_loans_adjustments adj on adj.emp_adv_loans_mid = l.id join pr_loan_master plm on l.loan_type_mid = plm.id where adj.id =" + partPaymentId +
                 " and adj.active = 1;";

            }

            DataTable dtloandetails = await _sha.Get_Table_FromQry(selQry);



            //for updating 
            int Emp_code = int.Parse(dtloandetails.Rows[0]["emp_code"].ToString());
            int loan_type_mid = int.Parse(dtloandetails.Rows[0]["loan_type_mid"].ToString());
            //int loan_id = int.Parse(dtloandetails.Rows[0]["loanid"].ToString());

            //
            int noOfLoans = dtloandetails.Rows.Count;
            DataRow drLoan = dtloandetails.Rows[0];
            //foreach (DataRow drLoan in dtloandetails.Rows)
            //{
            _princClose = int.Parse(drLoan["os_principal_amount"].ToString()); //open
            _intrClose = int.Parse(drLoan["os_interest_amount"].ToString()); //open
            dtAmountDisburse = DateTime.Parse(drLoan["date_disburse"].ToString());
            dtInstallmentStart = DateTime.Parse(drLoan["installment_start_date"].ToString());
            //dtInstallmentStart = dtInstallmentStart.AddMonths(1);
            _loanInstallment = int.Parse(drLoan["installment_amount"].ToString());
            interestRate = double.Parse(drLoan["interest_rate"].ToString());
            _noOfInterestInstallments = int.Parse(drLoan["interest_installment"].ToString());

            // _sbqry.Append("update pr_emp_loans_projection set active=0 where emp_code=" + Emp_code + " and loan_type_mid =" + loan_type_mid + " and emp_loan_id=" + loan_id + ";");

            _sbqry.Append(GenNewTransactionString());

            DateTime endMonth = new DateTime(2026, 03, 31); //todo

            int installmentAmt = 0;
            if (dtloandetails.Rows.Count == 2)
            {
                ForTwoLoansLedger(dtloandetails);
            }
            else
            {
                while ((dtAmountDisburse < endMonth) && (_princClose + _intrClose) > 0)
                {
                    //principle
                    if (_princClose > 0)
                    {
                        if (installmentAmt == 0 && (dtAmountDisburse.Year == dtInstallmentStart.Year) && (dtAmountDisburse.Month == dtInstallmentStart.Month))
                        {
                            installmentAmt = _loanInstallment;
                        }

                        int days = calDaysForAmount(dtAmountDisburse);
                        CalcMonthlyLedgerOnPrinciple(_princClose, installmentAmt, interestRate, _intrClose, days, dtAmountDisburse, dtloandetails);



                    }


                    else
                    {
                        //interest
                        CalcMonthlyLedgerOnInterest(dtAmountDisburse, _loanInstallment, _intrClose, dtloandetails);


                    }


                    //if not 1st day , adjust to 1st
                    if (dtAmountDisburse.Day != 1)
                        dtAmountDisburse = new DateTime(dtAmountDisburse.Year, dtAmountDisburse.Month, 1);

                    //next month
                    dtAmountDisburse = dtAmountDisburse.AddMonths(1);
                }
            }
            await _sha.Run_UPDDEL_ExecuteNonQuery(_sbqry.ToString());
        }

        private int calDaysForAmount(DateTime startMonth)
        {
            DateTime lastDayDate = new DateTime(startMonth.Year, startMonth.Month, 1).AddMonths(1).AddDays(-1);
            return (int)(lastDayDate.Date - startMonth.Date).TotalDays + 1;
        }

        public void CalcMonthlyLedgerOnPrinciple(int prinOpen, int instAmount, double intrRate, int intrOpen, int mnDays, DateTime dtAmountDisburse, DataTable dtloandetails)
        {
            int emp_id = int.Parse(dtloandetails.Rows[0]["emp_id"].ToString());
            int Emp_code = int.Parse(dtloandetails.Rows[0]["emp_code"].ToString());
            int emp_loan_id = int.Parse(dtloandetails.Rows[0]["emp_adv_loans_mid"].ToString());
            int emp_loan_child_id = int.Parse(dtloandetails.Rows[0]["Child_loanId"].ToString());
            int loan_type_mid = int.Parse(dtloandetails.Rows[0]["loan_type_mid"].ToString());
            string loan_code = dtloandetails.Rows[0]["loan_code"].ToString();
            string method = dtloandetails.Rows[0]["method"].ToString();
            int AmountIssued = int.Parse(dtloandetails.Rows[0]["loan_amount"].ToString());
            DateTime LoanSanctiodate = DateTime.Parse(dtloandetails.Rows[0]["sanction_date"].ToString());
            Double InterestRate = Double.Parse(dtloandetails.Rows[0]["interest_rate"].ToString());

            //_loanTotalPaid += instAmount;
            int prinClose;
            int loan_repaid = 0;
            int intrAmount = 0;
            int intrClose = 0;
            int intrRepaid = 0;

            //pf loan interest with equal installments
            if ((loan_code == @PrConstants.PF_LOANST1_CODE || loan_code == @PrConstants.PF_LOANST2_CODE || loan_code == @PrConstants.PF_LOANLT1_CODE || loan_code == @PrConstants.PF_LOANLT2_CODE || loan_code == @PrConstants.PF_LOANLT3_CODE || loan_code == @PrConstants.PF_LOANLT4_CODE || loan_code == PrConstants.PF_LOAN1_CODE || loan_code == PrConstants.PF_LOAN2_CODE) && method == PrConstants.PF_LOAN_METHOD)
            {
                prinClose = prinOpen - instAmount;
                _princClose = prinClose;

                if (prinOpen > 0)
                {
                    intrAmount = (int)Math.Round(((prinOpen * intrRate) / (100 * 365)) * mnDays);
                    intrRepaid = intrAmount;
                    if (instAmount != 0)
                    {
                        loan_repaid = instAmount - intrAmount;
                    }
                    prinClose = prinOpen - loan_repaid;
                    intrClose = 0;
                    intrOpen = 0;
                }
            }

            else
            {
                prinClose = prinOpen - instAmount;
                _princClose = prinClose;
                loan_repaid = instAmount;

                if (prinOpen > 0)
                {
                    intrAmount = (int)Math.Round(((prinOpen * intrRate) / (100 * 365)) * mnDays);
                    intrClose = intrOpen + intrAmount;
                    if (prinClose < 0)
                    {
                        intrClose = intrOpen + _princClose;
                        intrRepaid = _princClose * -1;
                        _princClose = 0;
                        prinClose = 0;
                    }

                }
                else
                {
                    intrClose = intrOpen + _princClose;
                    intrRepaid = _princClose * -1;
                    _princClose = 0;
                    prinClose = 0;
                }

                _intrClose = intrClose;
            }

            if (instAmount > 0)
                _InstallmentNumber++;

            _NewNumIndex++;

            _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
            _sbqry.Append("Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], " +
                "[emp_loan_id],[emp_loan_child_id], [loan_type_mid], [interest_rate], " +
                "[installment_number], [loan_opening], [total_paid], [loan_closing], [interest_opening], " +
                "[interest_accrued], [interest_repaid], [interest_closing], [installment_amount],[installment_part], [active], " +
                "[trans_id],loan_sl_no) values(@idnew" + _NewNumIndex + ",'" +
                 Helper.getFinancialYear(dtAmountDisburse) + "','" +
                 dtAmountDisburse + "', " +
                 emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_loan_child_id + ", " + loan_type_mid + ", " +
                 InterestRate + ", " + _InstallmentNumber + ", " + prinOpen + ", " +
                 instAmount + ", " + prinClose + ", " + intrOpen + ", " + intrAmount + ", " +
                 intrRepaid + ", " + intrClose + ", " + loan_repaid + ", '" + _installmentpart +
                    "', 1, @transidnew,"+loansno+");");

            if (prinClose == 0 && _noOfInterestInstallments != 0)
            {
                _loanInstallment = intrClose / _noOfInterestInstallments;
            }

        }

        public void CalcMonthlyLedgerOnInterest(DateTime dtAmountDisburse, int instAmount, int intrOpen, DataTable dtloandetails)
        {


            int emp_id = int.Parse(dtloandetails.Rows[0]["emp_id"].ToString());
            int Emp_code = int.Parse(dtloandetails.Rows[0]["emp_code"].ToString());
            int emp_loan_id = int.Parse(dtloandetails.Rows[0]["emp_adv_loans_mid"].ToString());
            int emp_loan_child_id = int.Parse(dtloandetails.Rows[0]["Child_loanId"].ToString());
            int loan_type_mid = int.Parse(dtloandetails.Rows[0]["loan_type_mid"].ToString());
            DateTime LoanSanctiodate = DateTime.Parse(dtloandetails.Rows[0]["sanction_date"].ToString());
            Double InterestRate = Double.Parse(dtloandetails.Rows[0]["interest_rate"].ToString());

            _noOfInterestInstallments--;

            if (_noOfInterestInstallments == 0)
            {
                instAmount = intrOpen;
            }
            int intrClose = intrOpen - instAmount;

            _intrClose = intrClose;
            //_loanTotalPaid += instAmount;

            _InstallmentNumber++;

            _NewNumIndex++;
            _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
            _sbqry.Append("Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], " +
                "[emp_loan_id],[emp_loan_child_id], [loan_type_mid], [interest_rate], " +
                "[installment_number], [loan_opening], [total_paid], [loan_closing], [interest_opening], " +
                "[interest_accrued], [interest_repaid], [interest_closing], [installment_amount],[installment_part], [active], " +
                "[trans_id],loan_sl_no) values(@idnew" + _NewNumIndex + ",'" +
                 Helper.getFinancialYear(dtAmountDisburse) + "','" +
                 dtAmountDisburse + "', " +
                 emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_loan_child_id + ", " + loan_type_mid + ", " +
                 InterestRate + ", " + _InstallmentNumber + ",0, " + instAmount + ", 0, " +
                 intrOpen + ", 0,  " +
                 instAmount + ", " + intrClose + ",  0, '" + _installmentpart + "', 1, @transidnew,"+loansno+"); ");



        }

        public void ForTwoLoansLedger(DataTable dtloandetails)
        {
            int LoanRepaid = 0;
            int Value = 0;
            int InterestOpening = 0;
            int InterestOpening2 = 0;
            int InstallmentRepaid = 0;
            int InterestRepaid = 0;
            int IntRepaid2 = 0;

            //for first loan
            int InterestAccured = 0;
            int InterestClosing = 0;

            //for second loan
            int InterestAccured2 = 0;
            int InterestClosing2 = 0; ;


            string qry = "";
            DataRow drLoan1 = dtloandetails.Rows[0];
            DataRow drLoan2 = dtloandetails.Rows[1];
            DateTime LoanStartDate = DateTime.Parse(drLoan1["installment_start_date"].ToString());
            int InterestInstallmentNumber = int.Parse(drLoan1["interest_installment"].ToString());
            int Emp_code = int.Parse(drLoan1["emp_code"].ToString());
            int loan_id = int.Parse(dtloandetails.Rows[0]["loan_type_mid"].ToString());
            DateTime fm = DateTime.Parse(drLoan1["date_disburse"].ToString());
            int fy = fm.Year;

            //financial months caluculations
            DateTime fyear = fm.AddYears(1);

            //for financial year
            int fyear2 = fyear.Year;
            int fmonth = 03;
            int fday = fm.Day;
            DateTime newfyear = new DateTime(2050, 03, 31);


            //for first loop without payment , only interest calculation i.e loan start date.
            int _1stloopforinterest = (LoanStartDate.Month + LoanStartDate.Year * 12) - (fm.Month + fm.Year * 12);

            //for loop for no of months from loan disburs
            int noofmonthforloop = (newfyear.Month + newfyear.Year * 12) - (LoanStartDate.Month + LoanStartDate.Year * 12) + 1;

            //string qry = "select l.emp_code,lc.loan_amount,l.emp_id as empid, lc.id as loanid, lc.interest_rate, l.loan_type_mid from pr_emp_adv_loans l join pr_emp_adv_loans_child lc on l.id=lc.emp_adv_loans_mid join pr_loan_master plm on l.loan_type_mid=plm.id where l.emp_code=" + Emp_code + " and l.loan_type_mid='" + loan_id + "' and l.active =1; ";

            // DataTable dtempdetails = await _sha.Get_Table_FromQry(qry);

            //for first loan eg:15,00,000
            int emp_id = int.Parse(dtloandetails.Rows[0]["emp_id"].ToString());
            int emp_loan_id = int.Parse(dtloandetails.Rows[0]["emp_adv_loans_mid"].ToString());
            int emp_child_loan_id = int.Parse(dtloandetails.Rows[0]["Child_loanid"].ToString());
            int loan_type_mid = int.Parse(dtloandetails.Rows[0]["loan_type_mid"].ToString());
            int AmountIssued = int.Parse(dtloandetails.Rows[0]["loan_amount"].ToString());
            Double InterestRate = Double.Parse(dtloandetails.Rows[0]["interest_rate"].ToString());
            DateTime LoanSanctiodate = DateTime.Parse(dtloandetails.Rows[0]["sanction_date"].ToString());

            string LoanName = dtloandetails.Rows[0]["loan_code"].ToString();
            int LoanOpening = int.Parse(dtloandetails.Rows[0]["loan_amount"].ToString());
            int LoanClosing = int.Parse(dtloandetails.Rows[0]["loan_amount"].ToString());



            // for second loan eg:5,00,000
            int emp_child_loan_id2 = int.Parse(dtloandetails.Rows[1]["Child_loanid"].ToString());
            int AmountIssued2 = int.Parse(dtloandetails.Rows[1]["loan_amount"].ToString());
            Double InterestRate2 = Double.Parse(dtloandetails.Rows[1]["interest_rate"].ToString());
            int LoanOpening2 = int.Parse(dtloandetails.Rows[1]["loan_amount"].ToString());
            int LoanClosing2 = int.Parse(dtloandetails.Rows[1]["loan_amount"].ToString());

            //for first loan.
            //int daysLeft = DateTime.DaysInMonth(fm.Year, fm.Month) - fm.Day;
            //Interest calculation per month
            //int InterestAccured = int.Parse(Math.Round(((LoanOpening * InterestRate) / (100 * 365)) * daysLeft).ToString());
            //int InterestClosing = InterestAccured + InterestOpening;

            //for secondloan
            //int InterestAccured2 = int.Parse(Math.Round(((LoanOpening2 * InterestRate2) / (100 * 365)) * daysLeft).ToString());
            //int InterestClosing2 = InterestAccured2 + InterestOpening2;


            //updating the previous records with same emp_code and emp_loan_id.
            //qry = "update pr_emp_loans_projection set active=0 where emp_code=" + emp_code + "and loan_type_mid=" + loan_type_mid + ";";
            //_sbqry.append(qry);
            //_sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_loan_ledger", emp_loan_id.ToString(), ""));

            ////for first loan 15,00,000 first row insertion that is zero interest.
            //_NewNumIndex++;
            //_sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
            //qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id]) " +
            //      "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued + ", '" + LoanSanctiodate + "', " + InterestRate + ", " + InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ", " + InterestOpening + ", " + InterestAccured + ", " + InterestRepaid + ", " + InterestClosing + ", " + InstallmentRepaid + ", 1, @transidnew);";
            //_sbqry.Append(qry);
            //_sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loan_ledger", "@idnew" + _NewNumIndex.ToString(), ""));

            ////for second loan 5,00,000 first row insertion that is zero interest.
            //_NewNumIndex++;
            //_sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
            //qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [loan_type_mid], [loan_name], [amount_issued], [sanction_date], [interest_rate], [installment_number], [loan_opening], [loan_repaid], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [installment_repaid], [active], [trans_id]) " +
            //      "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id2 + ", " + loan_type_mid + ", '" + LoanName + "', " + AmountIssued2 + ", '" + LoanSanctiodate + "', " + InterestRate2 + ", " + InstallmentNumber + ", " + LoanOpening2 + ", " + LoanRepaid + ", " + LoanClosing2 + ", " + InterestOpening2 + ", " + InterestAccured2 + ", " + InterestRepaid + ", " + InterestClosing2 + ", " + InstallmentRepaid + ", 1, @transidnew);";
            //_sbqry.Append(qry);
            //_sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loan_ledger", "@idnew" + _NewNumIndex.ToString(), ""));


            //for first loop calculate interest upto loan start date, on loan start date installment to be paid.

            for (int i = 0; i < _1stloopforinterest; i++)
            {
                LoanOpening = LoanClosing;
                LoanOpening2 = LoanClosing2;
                if (LoanOpening != 0)
                {


                    //Calculations

                    LoanRepaid = 0;

                    InterestOpening = InterestClosing;
                    InterestOpening2 = InterestClosing2;

                    int Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);

                    //Interest calculation per month
                    InterestAccured = int.Parse(Math.Round(((LoanOpening * InterestRate) / (100 * 365)) * Noofdaysinmonth).ToString());
                    //int y = (int)Math.Ceiling(2.4);
                    InterestClosing = InterestAccured + InterestOpening;

                    //for secondloan
                    InterestAccured2 = int.Parse(Math.Round(((LoanOpening2 * InterestRate2) / (100 * 365)) * Noofdaysinmonth).ToString());
                    InterestClosing2 = InterestAccured2 + InterestOpening2;

                    _NewNumIndex++;
                    _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
                    qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id],[emp_loan_child_id], [loan_type_mid], [interest_rate], [installment_number], [loan_opening], [installment_amount], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [total_paid], [active], [trans_id],loan_sl_no) " +
                          "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_child_loan_id + ", " + loan_type_mid + ", " + InterestRate + ", " + _InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + "," + InterestOpening + "  ,   " + InterestAccured + " , 0 ,  " + InterestClosing + "  , " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                    _sbqry.Append(qry);
                    _sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));


                    _NewNumIndex++;
                    _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
                    qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id],[emp_loan_child_id], [loan_type_mid],  [interest_rate], [installment_number], [loan_opening], [installment_amount], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [total_paid], [active], [trans_id],loan_sl_no) " +
                          "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_child_loan_id2 + ", " + loan_type_mid + "," + InterestRate2 + ", " + _InstallmentNumber + ", " + LoanOpening2 + ", " + LoanRepaid + ", " + LoanClosing2 + "," + InterestOpening2 + "  ,   " + InterestAccured2 + " , 0 ,  " + InterestClosing2 + "  , " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                    _sbqry.Append(qry);
                    _sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));

                }
                fm = fm.AddMonths(1);

            }


            //loop for no of months from loan start date to financial month-year.
            for (int i = 0; i < noofmonthforloop; i++)
            {

                LoanOpening = LoanClosing;
                //for first principal amount calculations
                if (LoanOpening > 0)
                {
                    _InstallmentNumber++;
                    //Calculations

                    Value = LoanOpening - LoanRepaid;
                    if (Value < 0)
                    {
                        LoanRepaid = LoanRepaid + (Value);
                        LoanClosing = 0;

                    }
                    else
                    {
                        LoanRepaid = int.Parse(drLoan1["installment_amount"].ToString());
                        LoanClosing = LoanOpening - LoanRepaid;
                    }

                    InterestOpening = InterestClosing;
                    int Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);

                    //Interest calculation per month
                    InterestAccured = int.Parse(Math.Round(((LoanOpening * InterestRate) / (100 * 365)) * Noofdaysinmonth).ToString());
                    //int y = (int)Math.Ceiling(2.4);
                    InterestClosing = InterestAccured + InterestOpening;
                    InstallmentRepaid = LoanRepaid;

                    _NewNumIndex++;
                    _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
                    qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id],[emp_loan_child_id], [loan_type_mid], [interest_rate], [installment_number], [loan_opening], [installment_amount], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [total_paid], [active], [trans_id],loan_sl_no) " +
                          "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_child_loan_id + ", " + loan_type_mid + ", " + InterestRate + ", " + _InstallmentNumber + ", " + LoanOpening + ", " + LoanRepaid + ", " + LoanClosing + ", " + InterestOpening + ", " + InterestAccured + ", " + InterestRepaid + ", " + InterestClosing + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                    _sbqry.Append(qry);
                    _sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));



                    // for second loan ledger from second month

                    //interest Calculations for second loan untill first principle zero.

                    InterestOpening2 = InterestClosing2;
                    //Interest calculation per month
                    InterestAccured2 = int.Parse(Math.Round(((LoanOpening2 * InterestRate2) / (100 * 365)) * Noofdaysinmonth).ToString());
                    InterestClosing2 = InterestAccured2 + InterestOpening2;

                    _NewNumIndex++;
                    _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));


                    if (Value < 0)
                    {

                        int LoanRepaid2 = -(Value);
                        LoanClosing2 = LoanOpening2 + (Value);
                        InstallmentRepaid = LoanRepaid2;
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id],[emp_loan_child_id], [loan_type_mid], [interest_rate], [installment_number], [loan_opening], [installment_amount], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [total_paid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_child_loan_id2 + ", " + loan_type_mid + "," + InterestRate2 + ", 0, " + LoanOpening2 + ", " + LoanRepaid2 + "," + LoanClosing2 + ", " + InterestOpening2 + ", " + InterestAccured2 + ", 0, " + InterestClosing2 + "," + InstallmentRepaid + " , 1, @transidnew,"+loansno+");";
                        _sbqry.Append(qry);
                        _sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));


                    }
                    else
                    {
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id],[emp_loan_child_id], [loan_type_mid], [interest_rate], [installment_number], [loan_opening], [installment_amount], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [total_paid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_child_loan_id2 + ", " + loan_type_mid + "," + InterestRate2 + ", 0, " + LoanOpening2 + ", 0, " + LoanClosing2 + ", " + InterestOpening2 + ", " + InterestAccured2 + ", 0, " + InterestClosing2 + ", 0 , 1, @transidnew,"+loansno+");";
                        _sbqry.Append(qry);
                        _sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));
                    }
                    fm = fm.AddMonths(1);

                }

                //for second loan or principal amount calculations
                else if (LoanOpening <= 0 && LoanOpening2 > 0)
                {


                    LoanOpening2 = LoanClosing2;
                    //for principal amount calculations
                    if (LoanOpening2 > 0)
                    {
                        _InstallmentNumber++;
                        //Calculations

                        Value = LoanOpening2 - LoanRepaid;
                        if (Value < 0)
                        {
                            LoanRepaid = LoanRepaid + (Value);
                            LoanClosing2 = 0;

                        }
                        else
                        {
                            LoanRepaid = int.Parse(drLoan1["installment_amount"].ToString());
                            LoanClosing2 = LoanOpening2 - LoanRepaid;
                        }
                        InstallmentRepaid = LoanRepaid;

                        int Noofdaysinmonth = DateTime.DaysInMonth(fm.Year, fm.Month);
                        InterestOpening2 = InterestClosing2;
                        //Interest calculation per month
                        InterestAccured2 = int.Parse(Math.Round(((LoanOpening2 * InterestRate2) / (100 * 365)) * Noofdaysinmonth).ToString());
                        //int y = (int)Math.Ceiling(2.4);
                        InterestClosing2 = InterestAccured2 + InterestOpening2;


                        _NewNumIndex++;
                        _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id],[emp_loan_child_id], [loan_type_mid],  [interest_rate], [installment_number], [loan_opening], [installment_amount], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [total_paid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_child_loan_id + ", " + loan_type_mid + ", " + InterestRate + ",  0, 0, 0, 0, 0, 0, 0, " + InterestClosing + ", 0, 1, @transidnew,"+loansno+");";
                        _sbqry.Append(qry);
                        _sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));


                        if (Value < 0)
                        {
                            InterestClosing2 = InterestClosing2 + (Value);
                            IntRepaid2 = -(Value);
                            InstallmentRepaid = LoanRepaid + IntRepaid2;
                        }

                        _NewNumIndex++;
                        _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id],[emp_loan_child_id], [loan_type_mid],[interest_rate], [installment_number], [loan_opening], [installment_amount], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [total_paid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_child_loan_id + ", " + loan_type_mid + ",  " + InterestRate2 + ", " + _InstallmentNumber + ", " + LoanOpening2 + ", " + LoanRepaid + ", " + LoanClosing2 + ", " + InterestOpening2 + ", " + InterestAccured2 + ", " + IntRepaid2 + ", " + InterestClosing2 + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                        _sbqry.Append(qry);
                        _sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));
                        fm = fm.AddMonths(1);
                    }

                }
                if (Value < 0 && InterestInstallmentNumber > 0)
                {
                    InterestRepaid = (int)Math.Ceiling(Convert.ToDecimal(InterestClosing + InterestClosing2)) / InterestInstallmentNumber;
                    Value = 0;
                    IntRepaid2 = InterestRepaid;
                }
                //for first loan interest ledger
                else if (LoanOpening <= 0 && LoanOpening2 <= 0 && InterestClosing > 0)
                {

                    if (InterestClosing != 0)
                    {

                        _InstallmentNumber++;
                        int Value2 = InterestClosing - InterestRepaid;
                        if (Value2 < 0)
                        {
                            InterestRepaid = InterestRepaid + (Value2);
                            InterestOpening = InterestClosing;
                            InterestClosing = 0;

                        }

                        else
                        {
                            InterestOpening = InterestClosing;
                            InterestClosing = InterestOpening - InterestRepaid;
                        }

                        InstallmentRepaid = InterestRepaid;

                        _NewNumIndex++;
                        _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id],[emp_loan_child_id], [loan_type_mid], [interest_rate], [installment_number], [loan_opening], [installment_amount], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [total_paid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_child_loan_id + ", " + loan_type_mid + ",  " + InterestRate + ",  " + _InstallmentNumber + ", 0, 0, 0, " + InterestOpening + ", 0, " + InterestRepaid + ", " + InterestClosing + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                        _sbqry.Append(qry);
                        _sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));


                        //second loan interest as it is desplaying
                        _NewNumIndex++;
                        _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id], [emp_loan_child_id],[loan_type_mid], [interest_rate], [installment_number], [loan_opening], [installment_amount], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [total_paid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_child_loan_id + ", " + loan_type_mid + ", " + InterestRate + ",  0, 0, 0, 0, 0, 0, 0, " + InterestClosing2 + ", 0, 1, @transidnew,"+loansno+");";
                        _sbqry.Append(qry);
                        _sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));



                    }
                    fm = fm.AddMonths(1);
                }

                //for second loan interest ledger
                else if (LoanOpening <= 0 && LoanOpening2 <= 0 && InterestClosing <= 0)
                {

                    if (InterestClosing2 != 0)
                    {
                        InterestRepaid = IntRepaid2;
                        _InstallmentNumber++;
                        int Value2 = InterestClosing2 - InterestRepaid;
                        if (Value2 < 0)
                        {
                            InterestRepaid = InterestRepaid + (Value2);
                            InterestOpening2 = InterestClosing2;
                            InterestClosing2 = 0;

                        }

                        else
                        {
                            InterestOpening2 = InterestClosing2;
                            InterestClosing2 = InterestOpening2 - InterestRepaid;
                        }

                        InstallmentRepaid = InterestRepaid;

                        _NewNumIndex++;
                        _sbqry.Append(GetNewNumStringArr("pr_emp_loans_projection", _NewNumIndex));
                        qry = "Insert into pr_emp_loans_projection([id], [fy], [fm], [emp_id], [emp_code], [emp_loan_id],[emp_loan_child_id], [loan_type_mid], [interest_rate], [installment_number], [loan_opening], [installment_amount], [loan_closing], [interest_opening], [interest_accrued], [interest_repaid], [interest_closing], [total_paid], [active], [trans_id],loan_sl_no) " +
                              "values(@idnew" + _NewNumIndex + ",'" + fyear2 + "','" + fm + "', " + emp_id + "," + Emp_code + ", " + emp_loan_id + ", " + emp_child_loan_id + ", " + loan_type_mid + "," + InterestRate + ",  " + _InstallmentNumber + ", 0, 0, 0, " + InterestOpening2 + ", 0, " + InterestRepaid + ", " + InterestClosing2 + ", " + InstallmentRepaid + ", 1, @transidnew,"+loansno+");";
                        _sbqry.Append(qry);
                        _sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_loans_projection", emp_loan_id.ToString(), ""));

                    }
                    fm = fm.AddMonths(1);
                }
            }

        }
    }
}


