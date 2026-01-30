using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayRollBusiness.Masters;
using PayrollModels;
using PayrollModels.Transactions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PayRollBusiness.Transaction
{
    public class AdjustmentsLoansBusiness : BusinessBase
    {
        int emp_adv_loans_mid = 0;
        int emp_adv_loans_child_mid = 0;
        public AdjustmentsLoansBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        int loansno = 0;

        public async Task<string> SearchEmployee(string EmpCode)
        {
            List<EmployeeSearchResult> retList = new List<EmployeeSearchResult>();
            string emp_option = "";
            //string str_qryempcode = "";
            //List <string>retList = new List<string>();
            //string ECode = "",EName = "",EDesignation = "",EBranch = "",EDoj = "",EDRetire = "",EDoc = "", EselOpt="";

            string strQry = "select e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
            + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
            + " join Designations d on e.CurrentDesignation = d.Id"
            + " join Branches b on e.Branch = b.Id"
            + " join Departments dep on e.Department = dep.Id where empid =" + EmpCode + "";

            DataTable dtEmpDetails = null;
            try
            {
                dtEmpDetails = await _sha.Get_Table_FromQry(strQry);
            }
            catch
            {
            }

            if (dtEmpDetails == null || dtEmpDetails.Rows.Count == 0)
            {
                strQry = "select e.empid, e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
                + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
                + " join Designations d on e.CurrentDesignation = d.Id "
                + " join Branches b on e.Branch = b.Id join Departments dep on e.Department = dep.Id ";

                int ecode = 0;
                if (int.TryParse(EmpCode, out ecode))
                {
                    strQry += " where  e.RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) and empid LIKE '" + EmpCode + "%'; ";
                }
                else
                {
                    strQry += " where  e.RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) and  FirstName LIKE '%" + EmpCode + "%' OR LastName LIKE '%" + EmpCode + "%'; ";
                }

                dtEmpDetails = await _sha.Get_Table_FromQry(strQry);
            }
            DateTime curdate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            //int curFY = _LoginCredential.FY;
            string[] arrFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM").Split('-');
            int curFY = Convert.ToInt32(arrFm[0]);
            int curFM = Convert.ToInt32(arrFm[1]);

            foreach (DataRow drEDet in dtEmpDetails.Rows)
            {
                DateTime retdate = Convert.ToDateTime(drEDet["RetirementDate"]);
                int reFm = retdate.Month;
                int retFy = retdate.Year;

                if (curFY >= retFy && curFM > reFm)
                {

                    retList.Add(new EmployeeSearchResult
                    {
                        ECode = "",
                        EName = "",
                        EDesignation = "",
                        EBranch = "",
                        EDoj = "",
                        EDRetire = "",
                        EDoc = "",
                        EselOpt = "",
                    });
                }

                else
                {
                    retList.Add(new EmployeeSearchResult
                    {
                        ECode = drEDet["empid"].ToString(),
                        EName = drEDet["Name"].ToString(),
                        EDesignation = drEDet["Designation"].ToString(),
                        EBranch = drEDet["deptbranch"].ToString(),
                        EDoj = drEDet["DOJ"].ToString(),
                        EDRetire = drEDet["RetirementDate"].ToString(),
                        EDoc = Convert.ToDateTime(drEDet["DOJ"].ToString()).AddMonths(12).ToString("dd-MM-yyyy"),
                        EselOpt = emp_option,
                    });
                }


            }

            return JsonConvert.SerializeObject(retList);

        }
        public async Task<string> GetAdjustmentsLoanDetails(int EmpId)
        {
            string qry1p = "select advloan.id, lmaster.loan_description, emp_code, total_amount, sanction_date, installment_start_date from pr_emp_adv_loans advloan join pr_loan_master lmaster on lmaster.id=advloan.loan_type_mid where advloan.emp_code=" + EmpId + " and advloan.active=1;";
            DataTable dtAdjLoans = await _sha.Get_Table_FromQry(qry1p);

            var adjLoanjson = JsonConvert.SerializeObject(dtAdjLoans);

            adjLoanjson = adjLoanjson.Replace("null", "''");

            var javaScriptSerializer = new JavaScriptSerializer();

            var adjloanDetails = javaScriptSerializer.DeserializeObject(adjLoanjson);


            var resultJson = javaScriptSerializer.Serialize(new { AdjLoanDetails = adjloanDetails });

            return JsonConvert.SerializeObject(resultJson);

        }

        public async Task<string> GetChildLoanDetailsofemp(int Id)
        {

            emp_adv_loans_mid = Id;
            string qry = "select id, date_disburse, loan_amount, interest_rate, interest_accured " +
                         " from pr_emp_adv_loans_child where emp_adv_loans_mid=" + Id + " and active=1;";
            DataTable dtChildLoans = await _sha.Get_Table_FromQry(qry);

            var adjChildLoanjson = JsonConvert.SerializeObject(dtChildLoans);

            adjChildLoanjson = adjChildLoanjson.Replace("null", "''");

            var javaScriptSerializer = new JavaScriptSerializer();

            var adjChildloanDetails = javaScriptSerializer.DeserializeObject(adjChildLoanjson);


            var resultJson = javaScriptSerializer.Serialize(new { AdjChildLoanDetails = adjChildloanDetails });

            return JsonConvert.SerializeObject(resultJson);

        }


        public async Task<string> GetWithoutinterestdata(int Id)
        {
            IList<ALwithoutInterestModel> lstwithoutInterest = new List<ALwithoutInterestModel>();
            IList<ALDeductionModel> lstDeduction = new List<ALDeductionModel>();
            try
            {
                emp_adv_loans_child_mid = Id;
                //string qry = "select case when pmaster.principal_installment=pmaster.completed_installment " +
                //             " then convert(varchar, 0)+'/' + convert(varchar, pmaster.interest_installment)" +
                //             " else convert(varchar, pmaster.completed_installment) + '/' + convert(varchar, pmaster.principal_installment + pmaster.interest_installment) end as installments, " +
                //             " pchild.id as id, loan_amount, interest_accured, total_amount_recovered, principal_amount_recovered ," +
                //             " interest_amount_recovered,pmaster.interest_installment_amount,pmaster.disp_remaining_installment,pmaster.disp_completed_installment, " +
                //             "convert(varchar, pmaster.completed_installment) + '/' + convert(varchar, pmaster.principal_installment) as installments," +
                //             " pmaster.installment_amount , convert(varchar, pmaster.principal_installment + pmaster.interest_installment) as totinstallments " +
                //             " from  pr_emp_adv_loans_child pchild join pr_emp_adv_loans pmaster on pchild.emp_adv_loans_mid=pmaster.id " +
                //             " where pchild.id= " + Id + " ";

                string qry = "select case when pmaster.principal_installment=pmaster.completed_installment " +
                       " then convert(varchar, 0)+'/' + convert(varchar, pmaster.interest_installment)" +
                       " else convert(varchar, pmaster.completed_installment) + '/' + convert(varchar, pmaster.principal_installment + pmaster.interest_installment) end as installments, " +
                       " pchild.id as id, loan_amount, interest_accured, total_amount_recovered, principal_amount_recovered ," +
                       " interest_amount_recovered,pmaster.interest_installment_amount,pmaster.remaining_installment,pmaster.completed_installment, " +
                       "convert(varchar, pmaster.completed_installment) + '/' + convert(varchar, pmaster.principal_installment) as installments," +
                       " pmaster.installment_amount , convert(varchar, pmaster.principal_installment + pmaster.interest_installment) as totinstallments " +
                       " from  pr_emp_adv_loans_child pchild join pr_emp_adv_loans pmaster on pchild.emp_adv_loans_mid=pmaster.id " +
                       " where pchild.id= " + Id + " ";

                string qry2 = "select os_principal_amount,os_principal_amount as cal_os_amount ,os_interest_amount as interest2, os_interest_amount, os_this_month_interest, os_total_amount  from pr_emp_adv_loans_child where id=" + Id + ";";
                DataSet dtChildLoans = await _sha.Get_MultiTables_FromQry(qry + qry2);

                DataTable dtWithoutInterest = dtChildLoans.Tables[0];
                DataTable dtDeduction = dtChildLoans.Tables[1];




                if (dtWithoutInterest.Rows.Count > 0)
                {
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "id",
                        // display = "",
                        value = dtWithoutInterest.Rows[0]["id"].ToString()
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "loan_amount",
                        display = "Amount",
                        value = dtWithoutInterest.Rows[0]["loan_amount"].ToString()
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "interest_accured",
                        display = "Interest Amount",
                        value = dtWithoutInterest.Rows[0]["interest_accured"].ToString()
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "total_amount_recovered",
                        display = "Recovered Amount",
                        value = dtWithoutInterest.Rows[0]["total_amount_recovered"].ToString()
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "principal_amount_recovered",
                        display = "Principle Recovered",
                        value = dtWithoutInterest.Rows[0]["principal_amount_recovered"].ToString()
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "interest_amount_recovered",
                        display = "Interest Recovered",
                        value = dtWithoutInterest.Rows[0]["interest_amount_recovered"].ToString()
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "totinstallments",
                        display = "Total Installments",
                        value = dtWithoutInterest.Rows[0]["totinstallments"].ToString()
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "completed_installment",
                        display = "Completed Installments",
                        value = dtWithoutInterest.Rows[0]["completed_installment"].ToString()
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "remaining_installment",
                        display = "Remaining Installments",
                        value = dtWithoutInterest.Rows[0]["remaining_installment"].ToString()
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "installments_amount",
                        display = "Principal Installments Amount",
                        value = dtWithoutInterest.Rows[0]["installment_amount"].ToString()
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "installments_amount",
                        display = "Interest Installments Amount",
                        value = dtWithoutInterest.Rows[0]["interest_installment_amount"].ToString()
                    });

                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "cal_os_amount",
                        //display = "",
                        value = dtDeduction.Rows[0]["cal_os_amount"].ToString()
                    });
                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "os_principal_amount",
                        display = "Loan Amount",
                        value = dtDeduction.Rows[0]["os_principal_amount"].ToString()
                    });
                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "interest2",
                        //display = "",
                        value = dtDeduction.Rows[0]["interest2"].ToString()
                    });
                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "os_interest_amount",
                        display = "Interest Amount",
                        value = dtDeduction.Rows[0]["os_interest_amount"].ToString()
                    });
                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "os_this_month_interest",
                        display = "This Month Interest",
                        value = dtDeduction.Rows[0]["os_this_month_interest"].ToString()
                    });
                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "os_total_amount",
                        display = "Total Amount",
                        value = dtDeduction.Rows[0]["os_total_amount"].ToString()
                    });

                }
                else
                {
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "id",
                        // display = "",
                        value = ""
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "loan_amount",
                        display = "Amount",
                        value = ""
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "interest_accured",
                        display = "Interest Amount",
                        value = ""
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "total_amount_recovered",
                        display = "Recovered Amount",
                        value = ""
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "principal_amount_recovered",
                        display = "Principle Recovered",
                        value = ""
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "interest_amount_recovered",
                        display = "Interest Recovered",
                        value = ""
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "installments",
                        display = "Installments",
                        value = ""
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "installments_amount",
                        display = "Principal Installments Amount",
                        value = ""
                    });
                    lstwithoutInterest.Add(new ALwithoutInterestModel
                    {
                        id = 0,
                        dbcolumn = "installments_amount",
                        display = "Interest Installments Amount",
                        value = ""
                    });

                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "cal_os_amount",
                        //display = "",
                        value = ""
                    });
                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "os_principal_amount",
                        display = "Loan Amount",
                        value = ""
                    });
                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "interest2",
                        //display = "",
                        value = ""
                    });
                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "os_interest_amount",
                        display = "Interest Amount",
                        value = ""
                    });
                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "os_this_month_interest",
                        display = "This Month Interest",
                        value = ""
                    });
                    lstDeduction.Add(new ALDeductionModel
                    {
                        id = 0,
                        dbcolumn = "os_total_amount",
                        display = "Total Amount",
                        value = ""
                    });
                }
            }


            catch (Exception e)
            {
                return e.Message;
            }
            return JsonConvert.SerializeObject(new { lstWithoutInterest = lstwithoutInterest, lstdeduction = lstDeduction, });
        }



        public async Task<string> saveAdjLoanData(CommonPostDTO Values)
        {
            //string query = "select last_num from new_num where table_name='loan_sl_no'";
            //DataTable dt = await _sha.Get_Table_FromQry(query);
            //loansno = Convert.ToInt32(dt.Rows[0]["last_num"]);
            //loansno++;
            //empi_id
            string loanType = " ";
            int emp_id = Values.EntityId;
            int emp_adv_loans_mid = 0;
            //Adjustloans table data string
            var WithoutInterestData = Values.StringData.Split('&');
            var idData = WithoutInterestData[0].Split('=');
            var amountData = WithoutInterestData[1].Split('=');
            //var interest_amountData = WithoutInterestData[2].Split('=');
            //var recovered_amountData = WithoutInterestData[3].Split('=');
            //var principle_recoveredData = WithoutInterestData[4].Split('=');
            //var interest_recoveredData = WithoutInterestData[5].Split('=');
            //var installmentsData = WithoutInterestData[6].Split('=');
            //var installment_amountData = WithoutInterestData[7].Split('=');

            int id = int.Parse(idData[1]);
            int amount = int.Parse(amountData[1]);
            //int interest_amount = int.Parse(interest_amountData[1]);
            //int recovered_amount = int.Parse(recovered_amountData[1]);
            //int principle_recovered = int.Parse(principle_recoveredData[1]);
            //int interest_recovered = int.Parse(interest_recoveredData[1]);
            //string value = installmentsData[1];
            //var x = value.Replace("%2F", "/");
            //var value2 = x.Split('/');
            //int dbnoofinstallments = int.Parse(value2[0]);
            //int dbinstallmetns = int.Parse(value2[1]);
            //int installment_amount = int.Parse(installment_amountData[1]);


            var Outstabding = Values.StringData2.Split('&');
            // splitting Outstabding 
            var cal_os_principal_amountData = Outstabding[0].Split('=');
            var os_principal_amountData = Outstabding[1].Split('=');
            var cal_os_interest_amountData = Outstabding[2].Split('=');
            var os_interest_amountData = Outstabding[3].Split('=');
            var os_total_amountData = Outstabding[5].Split('=');

            //real data for inserting data into db
            int hidden_os_principal_amount = int.Parse(cal_os_principal_amountData[1]);
            int os_principal_amount = int.Parse(os_principal_amountData[1]);

            int paid_os_principal = hidden_os_principal_amount - os_principal_amount;

            int hidden_os_interest_amount = int.Parse(cal_os_interest_amountData[1]);
            int os_interest_amount = int.Parse(os_interest_amountData[1]);
            int paid_os_interest = os_interest_amount - hidden_os_interest_amount;
            int os_total_amount = int.Parse(os_total_amountData[1]);
            //financial year and month
            int fy = _LoginCredential.FY;
            string fm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");


            //int tinstallments = dbnoofinstallments + noofinstallments;
            //int trecovered_amount = recovered_amount + amountpaid;
            //int tprinciple_recovered = principle_recovered + amountpaid;
            //int tos_loan_amount = os_loan_amount - amountpaid;



            //entered values from UI
            var EnteredValues = Values.StringData3.Split(',');
            string Fradio = EnteredValues[0];
            string Sradio = EnteredValues[1];
            DateTime paiddate = DateTime.Parse(EnteredValues[2]);
            string paidondate = paiddate.ToString("MM-dd-yyyy");
            //int noofinstallments = int.Parse(EnteredValues[3]);
            int amountpaid = int.Parse(EnteredValues[3]);
            string GetLoans = "SELECT mas.loan_id,mas.id as mastertypeid,adv.id,chi.priority,chi.Id as childloanid," +
             "chi.emp_adv_loans_mid,adv.method ,chi.principal_amount_recovered,chi.total_amount_recovered," +
             "chi.loan_amount,os_principal_amount,chi.os_interest_amount,chi.principal_recovered_flag,chi.interest_recovered_flag," +
             "chi.loan_sl_no,chi.interest_rate,chi.total_interest_installments,adv.installment_amount,adv.total_installment, " +
             "adv.interest_installment_amount,adv.completed_installment,adv.total_recovered_amount,adv.sanction_date,chi.date_disburse " +
             "FROM pr_emp_adv_loans adv" +
             " JOIN pr_loan_master mas ON adv.loan_type_mid = mas.id " +
             " JOIN pr_emp_adv_loans_child chi ON adv.id = chi.emp_adv_loans_mid " +
             " WHERE adv.active = 1 AND chi.id =" + id + " " +
             " ORDER BY chi.interest_rate desc;";

            string qryGetfm = "Select fm,fy from pr_month_details where active=1;";

            DataSet dtLoansAndMonth = await _sha.Get_MultiTables_FromQry(GetLoans + qryGetfm);

            DataTable dtLoans = dtLoansAndMonth.Tables[0];
            DataTable dtFm = dtLoansAndMonth.Tables[1];
            string qrychild = "select m.completed_installment, m.remaining_installment, m.installment_amount," +
                " m.total_installment ,* from pr_emp_adv_loans_child c join pr_emp_adv_loans m on c.emp_adv_loans_mid=m.id " +
                " where c.id=" + id + " and c.loan_amount=" + amount + ";";
            DataTable dtChildLoans = await _sha.Get_Table_FromQry(qrychild);
            StringBuilder sbqry = new StringBuilder();
            int loanid = Convert.ToInt32(dtLoans.Rows[0]["id"]);
            string GetTwoSubLoans = "SELECT mas.loan_id,chi.priority,chi.Id as childloanid," +
             "chi.emp_adv_loans_mid,chi.total_amount_recovered," +
             "chi.loan_amount,os_principal_amount,chi.os_interest_amount,chi.principal_recovered_flag,chi.interest_recovered_flag," +
             "chi.loan_sl_no,adv.installment_amount,adv.total_installment, " +
             "adv.interest_installment_amount,adv.total_recovered_amount " +
             "FROM pr_emp_adv_loans adv" +
             " JOIN pr_loan_master mas ON adv.loan_type_mid = mas.id " +
             " JOIN pr_emp_adv_loans_child chi ON adv.id = chi.emp_adv_loans_mid " +
             " WHERE adv.active = 1 and adv.id="+ loanid + "" +
             " ORDER BY chi.interest_rate desc;";
            DataTable subloans = await _sha.Get_Table_FromQry(GetTwoSubLoans);
            int total_os_amt = 0;
            

            if (dtLoans.Rows.Count > 0)
            {
                loanType = dtLoans.Rows[0]["loan_id"].ToString().ToUpper();
                            
            }

            //updating only principal amount
            if (loanType == PrConstants.PF_LOAN1_CODE || loanType == PrConstants.PF_LOAN2_CODE)
            {
                if (amountpaid <= hidden_os_principal_amount)
                {
                    foreach (DataRow drChild_table in dtChildLoans.Rows)
                    {
                        //installments calculation
                        int emi = int.Parse(drChild_table["installment_amount"].ToString());
                        int completed_installments = int.Parse(drChild_table["completed_installment"].ToString());
                        int no_of_installments_cover=0;
                        if (emi!=null || emi !=0)
                        {
                            no_of_installments_cover = int.Parse(Math.Round(Convert.ToDecimal(amountpaid / emi)).ToString());
                        }
                           
                        int total_installment = int.Parse(drChild_table["total_installment"].ToString());
                     
                        int remaining_installments = total_installment - completed_installments;
                        loansno = int.Parse(drChild_table["loan_sl_no"].ToString());
                        //------------------------
                        if (Fradio == "Full Clearing")
                        {
                            completed_installments = total_installment;
                            remaining_installments = 0;
                        }
                        else
                        {
                            completed_installments = completed_installments + no_of_installments_cover;
                            remaining_installments = remaining_installments - no_of_installments_cover;
                        }



                        int emp_adv_loans_child_mid = int.Parse(drChild_table["id"].ToString());
                        emp_adv_loans_mid = int.Parse(drChild_table["emp_adv_loans_mid"].ToString());
                        int loan_amount = int.Parse(drChild_table["loan_amount"].ToString());

                        int principal_amount_recovered = int.Parse(drChild_table["principal_amount_recovered"].ToString());
                        int principal_open_amount = hidden_os_principal_amount;
                        int principal_balance_amount = principal_open_amount - amountpaid;
                        int interest_accured = int.Parse(drChild_table["interest_accured"].ToString());

                        //interest related calculations
                        int interest_open_amount = int.Parse(drChild_table["os_interest_amount"].ToString());
                        interest_open_amount = paid_os_interest + interest_open_amount;
                        //1. trans_id
                        sbqry.Append(GenNewTransactionString());

                        int NewNumIndex = 0;


                        string qry = "";
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments", NewNumIndex));
                        //qry = "Update pr_emp_adv_loans_adjustments set Active = 0 where id=" + id + ";";
                        //sbqry.Append(qry);

                        //update loan_adjustments table
                        qry = "update pr_emp_adv_loans_adjustments set active=0 where emp_adv_loans_mid=" + emp_adv_loans_mid + " and emp_adv_loans_child_mid=" + emp_adv_loans_child_mid + ";";
                        sbqry.Append(qry);


                        //3. qry---insert into pr_emp_adv_loans_adjustments
                        qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[installments_paid],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id],fm,fy,loan_sl_no) values "
                           + "(@idnew" + NewNumIndex + "," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + amountpaid + "," + principal_balance_amount + "," + interest_open_amount + "," + 0 + "," + interest_open_amount + "," + completed_installments + ",'" + Fradio + "','" + Sradio + "', '" + paidondate + "'," + no_of_installments_cover + "," + amountpaid + ",1, @transidnew, '" + fm + "'," + fy + "," + loansno + ");";
                        sbqry.Append(qry);

                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew" + NewNumIndex, ""));

                        int interest_amount_recovered = int.Parse(drChild_table["interest_amount_recovered"].ToString());
                        int total_amount_recovered = int.Parse(drChild_table["total_amount_recovered"].ToString());

                        principal_amount_recovered = principal_amount_recovered + amountpaid;
                        total_amount_recovered = interest_amount_recovered + principal_amount_recovered;
                        if (os_total_amount < emi)
                        {
                            DateTime sandate = Convert.ToDateTime(dtLoans.Rows[0]["sanction_date"]);
                            float IntRATE = float.Parse(dtLoans.Rows[0]["interest_rate"].ToString());
                            //Payments Dates
                            DateTime paidDate = Convert.ToDateTime(dtFm.Rows[0]["fm"].ToString());
                            int yrDays = 365;
                            double retIntAmt = 0;
                            int retIntAmt1 = 0;
                            //calc days
                            int days = 0;

                            if (sandate.Month == paidDate.Month && sandate.Year == paidDate.Year) //in same month
                                days = Helper.findLastDayOfMonth(paidDate) - sandate.Day;
                            else
                                days = Helper.findLastDayOfMonth(paidDate);
                            if (paiddate.Year != 1900) //if any prev. partpay in the same month remove those days
                            {
                                days -= paiddate.Day;
                            }
                            retIntAmt = Math.Round(((os_principal_amount * (IntRATE / 100)) / yrDays) * days);
                            retIntAmt1 = Convert.ToInt32(retIntAmt);
                            os_total_amount = os_total_amount + retIntAmt1;
                            qry = "Update pr_emp_adv_loans set installment_amount=" + os_total_amount + " where id=" + emp_adv_loans_mid + ";";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));
                            qry = "update pr_emp_adv_loans_child set principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + principal_balance_amount + ", os_interest_amount=" + interest_open_amount + ", os_total_amount= " + os_total_amount + " where id=" + emp_adv_loans_child_mid + " and active=1;";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));
                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id =" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));
                        }
                        else if (os_total_amount == 0)
                        {

                            qry = "update pr_emp_adv_loans_child set active=0, principal_recovered_flag=1, interest_recovered_flag=1  , principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + " where id=" + emp_adv_loans_child_mid + ";";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            qry = "Update pr_emp_adv_loans set active=0,completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id=" + emp_adv_loans_mid + ";";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));

                        }
                        else if (os_principal_amount == 0 && os_total_amount != 0)
                        {
                            qry = "update pr_emp_adv_loans_child set  principal_recovered_flag=1 , principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + " where emp_adv_loans_mid=" + emp_adv_loans_mid + " and id=" + emp_adv_loans_child_mid + ";";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id =" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));
                        }
                        else
                        {

                            qry = "update pr_emp_adv_loans_child set principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_interest_amount=" + interest_open_amount + ", os_total_amount= " + os_total_amount + " where id=" + emp_adv_loans_child_mid + " and active=1;";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id =" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));

                        }

                    }

                }
                else if (hidden_os_principal_amount != 0 && amountpaid > hidden_os_principal_amount)
                {

                    foreach (DataRow drChild_table in dtChildLoans.Rows)
                    {
                        //installments calculation
                        int emi = int.Parse(drChild_table["installment_amount"].ToString());
                        int no_of_installments_cover= 0;
                        if (emi!=null || emi != 0)
                        {
                            int.Parse(Math.Round(Convert.ToDecimal(amountpaid / emi)).ToString());
                        }
                        int completed_installments = int.Parse(drChild_table["completed_installment"].ToString());
                        int remaining_installments = int.Parse(drChild_table["remaining_installment"].ToString());
                       
                        int total_installment = int.Parse(drChild_table["total_installment"].ToString());

                        int firstsubloanintallments = int.Parse(drChild_table["total_principal_installments"].ToString());
                        //------------------------
                        if (Fradio == "Full Clearing")
                        {
                            //completed_installments = total_installment;
                            completed_installments = firstsubloanintallments;
                            remaining_installments = 0;
                        }
                        else
                        {
                            completed_installments = completed_installments + no_of_installments_cover;
                            remaining_installments = remaining_installments - no_of_installments_cover;
                        }

                        //------------------------

                        int emp_adv_loans_child_mid = int.Parse(drChild_table["id"].ToString());
                        emp_adv_loans_mid = int.Parse(drChild_table["emp_adv_loans_mid"].ToString());
                        int loan_amount = int.Parse(drChild_table["loan_amount"].ToString());

                        int principal_amount_recovered = int.Parse(drChild_table["principal_amount_recovered"].ToString());
                        int principal_open_amount = loan_amount - (principal_amount_recovered);
                        int principal_paid_amount = principal_open_amount;
                        int x_amountpaid = amountpaid - principal_open_amount;
                        int interest_accured = int.Parse(drChild_table["interest_accured"].ToString());

                        //interest related calculations
                        int interest_open_amount = int.Parse(drChild_table["os_interest_amount"].ToString());
                        interest_open_amount = paid_os_interest + interest_open_amount;
                        int interest_paid_amount = x_amountpaid;
                        int interest_balance_amount = interest_open_amount - x_amountpaid;
                        if (interest_open_amount == 0 || interest_balance_amount <= 0)
                        {
                            interest_balance_amount = 0;
                        }

                        //1. trans_id
                        sbqry.Append(GenNewTransactionString());

                        int NewNumIndex = 0;

                        string qry = "";
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments", NewNumIndex));
                        //qry = "Update pr_emp_adv_loans_adjustments set Active = 0 where id=" + id + ";";
                        //sbqry.Append(qry);


                        //update loan_adjustments table
                        qry = "update pr_emp_adv_loans_adjustments set active=0 where emp_adv_loans_mid=" + emp_adv_loans_mid + " and emp_adv_loans_child_mid=" + emp_adv_loans_child_mid + ";";
                        sbqry.Append(qry);


                        //3. qry---insert into pr_emp_adv_loans_adjustments
                        qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[installments_paid],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id],fm,fy) values "
                           + "(@idnew" + NewNumIndex + "," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + principal_paid_amount + "," + 0 + "," + interest_open_amount + "," + interest_paid_amount + "," + interest_balance_amount + "," + completed_installments + ",'" + Fradio + "','" + Sradio + "', '" + paidondate + "'," + no_of_installments_cover + "," + amountpaid + ",1, @transidnew,'" + fm + "'," + fy + ");";
                        sbqry.Append(qry);



                        //qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principle_paid_amount],[principle_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id]) values "
                        //   + "(@idnew" + NewNumIndex + "," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + amountpaid + "," + principal_balance_amount + ",'" + Fradio + ",'" + interest_accured + "','" + Sradio + "', '" + paidondate + "'," + Fradio + "," + "'," + Fradio + "," + "'," + Fradio + "," + Sradio + ",1, @transidnew);";


                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew" + NewNumIndex, ""));


                        // update loans_child table

                        int interest_amount_recovered = int.Parse(drChild_table["interest_amount_recovered"].ToString());
                        int total_amount_recovered = int.Parse(drChild_table["total_amount_recovered"].ToString());

                        interest_amount_recovered = interest_amount_recovered + x_amountpaid;
                        principal_amount_recovered = principal_amount_recovered + principal_open_amount;
                        total_amount_recovered = interest_amount_recovered + principal_amount_recovered;



                        if (os_principal_amount == 0 && os_interest_amount == 0)
                        {

                            qry = "update pr_emp_adv_loans_child set active=0, interest_recovered_flag=1, principal_recovered_flag=1, principal_amount_recovered=" + principal_amount_recovered + ",interest_amount_recovered=" + interest_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_interest_amount=" + os_interest_amount + ", os_total_amount= " + os_total_amount + " where id=" + id + " and active=1;";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            //todo

                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id=" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));


                        }
                        else
                        {
                            qry = "update pr_emp_adv_loans_child set principal_recovered_flag=1, principal_amount_recovered=" + principal_amount_recovered + ",interest_amount_recovered=" + interest_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_interest_amount=" + os_interest_amount + ", os_total_amount= " + os_total_amount + " where id=" + id + " and active=1;";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id=" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));

                        }


                    }

                }
                //updating only os_interest amount when debited
                else if (hidden_os_principal_amount == 0)
                {
                    foreach (DataRow drChild_table in dtChildLoans.Rows)
                    {
                        //installments calculation
                        int emi = int.Parse(drChild_table["installment_amount"].ToString());
                        int no_of_installments_cover = 0;
                        if (emi != null && emi != 0)
                        {
                            no_of_installments_cover = int.Parse(Math.Round(Convert.ToDecimal(amountpaid / emi)).ToString());
                        }
                        int completed_installments = int.Parse(drChild_table["completed_installment"].ToString());
                        int remaining_installments = int.Parse(drChild_table["remaining_installment"].ToString());
                        int total_installment = int.Parse(drChild_table["total_installment"].ToString());
                        int intrst_install_amt = int.Parse(drChild_table["interest_installment_amount"].ToString());
                        //------------------------
                        if (Fradio == "Full Clearing")
                        {
                            completed_installments = total_installment;
                            remaining_installments = 0;
                        }
                        else
                        {
                            completed_installments = completed_installments + no_of_installments_cover;
                            remaining_installments = remaining_installments - no_of_installments_cover;
                        }
                        //------------------------

                        int emp_adv_loans_child_mid = int.Parse(drChild_table["id"].ToString());
                        emp_adv_loans_mid = int.Parse(drChild_table["emp_adv_loans_mid"].ToString());
                        int loan_amount = int.Parse(drChild_table["loan_amount"].ToString());

                        int principal_amount_recovered = int.Parse(drChild_table["principal_amount_recovered"].ToString());
                        int principal_open_amount = loan_amount - (principal_amount_recovered);
                        int principal_paid_amount = principal_open_amount;
                        int interest_accured = int.Parse(drChild_table["interest_accured"].ToString());

                        //interest related calculations
                        int interest_open_amount = int.Parse(drChild_table["os_interest_amount"].ToString());
                        int interest_paid_amount = amountpaid;
                        int interest_balance_amount = interest_open_amount - amountpaid;
                        int interest_installment_amount = 0;
                        if (interest_balance_amount < intrst_install_amt)
                        {
                            interest_installment_amount = interest_balance_amount;
                        }
                        //1. trans_id
                        sbqry.Append(GenNewTransactionString());

                        int NewNumIndex = 0;

                        string qry = "";
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments", NewNumIndex));
                        //qry = "Update pr_emp_adv_loans_adjustments set Active = 0 where id=" + id + ";";
                        //sbqry.Append(qry);


                        //update loan_adjustments table
                        qry = "update pr_emp_adv_loans_adjustments set active=0 where emp_adv_loans_mid=" + emp_adv_loans_mid + " and emp_adv_loans_child_mid=" + emp_adv_loans_child_mid + ";";
                        sbqry.Append(qry);


                        //3. qry---insert into pr_emp_adv_loans_adjustments
                        qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[installments_paid],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id],fm,fy) values "
                           + "(@idnew" + NewNumIndex + "," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + 0 + "," + 0 + "," + 0 + "," + interest_open_amount + "," + interest_paid_amount + "," + interest_balance_amount + "," + completed_installments + ",'" + Fradio + "','" + Sradio + "', '" + paidondate + "'," + no_of_installments_cover + "," + amountpaid + ",1, @transidnew,'" + fm + "'," + fy + ");";
                        sbqry.Append(qry);


                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew" + NewNumIndex, ""));


                        // update loans_child table

                        int interest_amount_recovered = int.Parse(drChild_table["interest_amount_recovered"].ToString());
                        int total_amount_recovered = int.Parse(drChild_table["total_amount_recovered"].ToString());

                        interest_amount_recovered = interest_amount_recovered + amountpaid;
                        total_amount_recovered = interest_amount_recovered + principal_amount_recovered;


                        if (os_interest_amount == 0)
                        {
                            qry = "update pr_emp_adv_loans_child set interest_recovered_flag=1,active=0, interest_amount_recovered=" + interest_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_interest_amount=" + os_interest_amount + ", os_total_amount= " + os_total_amount + " where id=" + id + " and active=1;";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            //todo
                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id=" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));


                        }

                        else
                        {
                            qry = "update pr_emp_adv_loans_child set interest_amount_recovered=" + interest_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_interest_amount=" + os_interest_amount + ", os_total_amount= " + os_total_amount + " where id=" + id + " and active=1;";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", interest_installment_amount=" + interest_installment_amount + ",remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id=" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));
                        }
                    }


                }
            }
            else  if (loanType != PrConstants.PF_LOAN1_CODE || loanType != PrConstants.PF_LOAN2_CODE)
                {
                if (amountpaid <= hidden_os_principal_amount)
                {
                    foreach (DataRow drChild_table in dtChildLoans.Rows)
                    {
                        //installments calculation
                        int emi = int.Parse(drChild_table["installment_amount"].ToString());
                        int no_of_installments_cover = 0;
                        if (emi != null && emi != 0)
                        {
                            no_of_installments_cover= int.Parse(Math.Round(Convert.ToDecimal(amountpaid / emi)).ToString());
                        }
                        int completed_installments = int.Parse(drChild_table["completed_installment"].ToString());

                        int total_installment = int.Parse(drChild_table["total_installment"].ToString());
                        int remaining_installments = total_installment - completed_installments;
                        loansno = int.Parse(drChild_table["loan_sl_no"].ToString());
                        //------------------------
                        if (Fradio == "Full Clearing")
                        {
                            completed_installments = total_installment;
                            remaining_installments = 0;
                        }
                        else
                        {
                            completed_installments = completed_installments + no_of_installments_cover;
                            remaining_installments = remaining_installments - no_of_installments_cover;
                        }



                        int emp_adv_loans_child_mid = int.Parse(drChild_table["id"].ToString());
                        emp_adv_loans_mid = int.Parse(drChild_table["emp_adv_loans_mid"].ToString());
                        int loan_amount = int.Parse(drChild_table["loan_amount"].ToString());

                        int principal_amount_recovered = int.Parse(drChild_table["principal_amount_recovered"].ToString());
                        int principal_open_amount = loan_amount - (principal_amount_recovered);
                        int principal_balance_amount = principal_open_amount - amountpaid;
                        int interest_accured = int.Parse(drChild_table["interest_accured"].ToString());

                        //interest related calculations
                        int interest_open_amount = int.Parse(drChild_table["os_interest_amount"].ToString());
                        interest_open_amount = paid_os_interest + interest_open_amount;

                        if (subloans.Rows.Count == 2)
                        {
                            int os_principal_amt1 = Convert.ToInt32(subloans.Rows[0]["os_principal_amount"]);
                            int os_principal_amt2 = Convert.ToInt32(subloans.Rows[1]["os_principal_amount"]);
                            int os_interest_amt1 = Convert.ToInt32(subloans.Rows[0]["os_interest_amount"]);
                            int os_interest_amt2 = Convert.ToInt32(subloans.Rows[1]["os_interest_amount"]);
                            total_os_amt = os_principal_amt1 + principal_balance_amount + os_interest_amt1 + interest_open_amount;
                        }
                        else if (subloans.Rows.Count == 1)
                        {
                            total_os_amt = os_total_amount; ;
                        }
                        //1. trans_id
                        sbqry.Append(GenNewTransactionString());

                        int NewNumIndex = 0;


                        string qry = "";
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments", NewNumIndex));
                        //qry = "Update pr_emp_adv_loans_adjustments set Active = 0 where id=" + id + ";";
                        //sbqry.Append(qry);

                        //update loan_adjustments table
                        qry = "update pr_emp_adv_loans_adjustments set active=0 where emp_adv_loans_mid=" + emp_adv_loans_mid + " and emp_adv_loans_child_mid=" + emp_adv_loans_child_mid + ";";
                        sbqry.Append(qry);


                        //3. qry---insert into pr_emp_adv_loans_adjustments
                        qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_accured],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[installments_paid],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id],fm,fy,loan_sl_no) values "
                           + "(@idnew" + NewNumIndex + "," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + amountpaid + "," + principal_balance_amount + ","+paid_os_interest+"," + interest_open_amount + "," + 0 + "," + interest_open_amount + "," + completed_installments + ",'" + Fradio + "','" + Sradio + "', '" + paidondate + "'," + no_of_installments_cover + "," + amountpaid + ",1, @transidnew, '" + fm + "'," + fy + "," + loansno + ");";
                        sbqry.Append(qry);

                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew" + NewNumIndex, ""));

                        int interest_amount_recovered = int.Parse(drChild_table["interest_amount_recovered"].ToString());
                        int total_amount_recovered = int.Parse(drChild_table["total_amount_recovered"].ToString());

                        principal_amount_recovered = principal_amount_recovered + amountpaid;
                        total_amount_recovered = interest_amount_recovered + principal_amount_recovered;
                      
                            if (total_os_amt < emi)
                            {
                               // interest calculation for out standing principal amount
                                qry = "Update pr_emp_adv_loans set installment_amount=" + total_os_amt + " where id=" + emp_adv_loans_mid + ";";
                                sbqry.Append(qry);
                                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));
                            }
                      
                        if (os_total_amount == 0)
                        {

                            qry = "update pr_emp_adv_loans_child set active=0, principal_recovered_flag=1, interest_recovered_flag=1  , principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + " where id=" + emp_adv_loans_child_mid + ";";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            qry = "Update pr_emp_adv_loans set active=0,completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id=" + emp_adv_loans_mid + ";";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));

                        }

                        else if (os_principal_amount == 0 && os_total_amount != 0)
                        {
                            qry = "update pr_emp_adv_loans_child set  principal_recovered_flag=1 , principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_total_amount= " + os_total_amount + " where emp_adv_loans_mid=" + emp_adv_loans_mid + " and id=" + emp_adv_loans_child_mid + ";";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id =" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));
                        }

                        else
                        {

                            qry = "update pr_emp_adv_loans_child set principal_amount_recovered=" + principal_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_interest_amount=" + interest_open_amount + ", os_total_amount= " + os_total_amount + " where id=" + emp_adv_loans_child_mid + " and active=1;";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id =" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));

                        }

                    }

                }
                else if (hidden_os_principal_amount != 0 && amountpaid > hidden_os_principal_amount)
                {

                    foreach (DataRow drChild_table in dtChildLoans.Rows)
                    {
                        //installments calculation
                        int emi = int.Parse(drChild_table["installment_amount"].ToString());
                        int no_of_installments_cover = 0;
                        if (emi != null && emi != 0)
                        {
                            no_of_installments_cover=int.Parse(Math.Round(Convert.ToDecimal(amountpaid / emi)).ToString());
                        }
                        int completed_installments = int.Parse(drChild_table["completed_installment"].ToString());
                        int remaining_installments = int.Parse(drChild_table["remaining_installment"].ToString());
                        int total_installment = int.Parse(drChild_table["total_installment"].ToString());

                        int firstsubloanintallments = int.Parse(drChild_table["total_principal_installments"].ToString());
                        //------------------------
                        if (Fradio == "Full Clearing")
                        {
                            //completed_installments = total_installment;
                            completed_installments = firstsubloanintallments;
                            remaining_installments = 0;
                        }
                        else
                        {
                            completed_installments = completed_installments + no_of_installments_cover;
                            remaining_installments = remaining_installments - no_of_installments_cover;
                        }

                        //------------------------

                        int emp_adv_loans_child_mid = int.Parse(drChild_table["id"].ToString());
                        emp_adv_loans_mid = int.Parse(drChild_table["emp_adv_loans_mid"].ToString());
                        int loan_amount = int.Parse(drChild_table["loan_amount"].ToString());

                        int principal_amount_recovered = int.Parse(drChild_table["principal_amount_recovered"].ToString());
                        int principal_open_amount = int.Parse(dtLoans.Rows[0]["os_principal_amount"].ToString()); 
                        int principal_paid_amount = principal_open_amount;
                        int x_amountpaid = amountpaid - principal_open_amount;
                        int interest_accured = int.Parse(drChild_table["interest_accured"].ToString());

                        //interest related calculations
                        int interest_open_amount = int.Parse(drChild_table["os_interest_amount"].ToString());
                        interest_open_amount = paid_os_interest + interest_open_amount;
                        int interest_paid_amount = x_amountpaid;
                        if(interest_paid_amount<0)
                        {
                            interest_paid_amount = System.Math.Abs(interest_paid_amount);
                            principal_paid_amount= principal_paid_amount- interest_paid_amount;


                        }
                        int interest_balance_amount = interest_open_amount - x_amountpaid;
                        if (interest_open_amount == 0 || interest_balance_amount <= 0)
                        {
                            interest_balance_amount = 0;
                            interest_open_amount = interest_paid_amount;
                        }

                        //1. trans_id
                        sbqry.Append(GenNewTransactionString());

                        int NewNumIndex = 0;

                        string qry = "";
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments", NewNumIndex));
                        //qry = "Update pr_emp_adv_loans_adjustments set Active = 0 where id=" + id + ";";
                        //sbqry.Append(qry);


                        //update loan_adjustments table
                        qry = "update pr_emp_adv_loans_adjustments set active=0 where emp_adv_loans_mid=" + emp_adv_loans_mid + " and emp_adv_loans_child_mid=" + emp_adv_loans_child_mid + ";";
                        sbqry.Append(qry);


                        //3. qry---insert into pr_emp_adv_loans_adjustments
                        qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[installments_paid],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id],fm,fy) values "
                           + "(@idnew" + NewNumIndex + "," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + principal_paid_amount + "," + 0 + "," + interest_open_amount + "," + interest_paid_amount + "," + interest_balance_amount + "," + completed_installments + ",'" + Fradio + "','" + Sradio + "', '" + paidondate + "'," + no_of_installments_cover + "," + amountpaid + ",1, @transidnew,'" + fm + "'," + fy + ");";
                        sbqry.Append(qry);



                        //qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principle_paid_amount],[principle_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id]) values "
                        //   + "(@idnew" + NewNumIndex + "," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + principal_open_amount + "," + amountpaid + "," + principal_balance_amount + ",'" + Fradio + ",'" + interest_accured + "','" + Sradio + "', '" + paidondate + "'," + Fradio + "," + "'," + Fradio + "," + "'," + Fradio + "," + Sradio + ",1, @transidnew);";


                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew" + NewNumIndex, ""));


                        // update loans_child table

                        int interest_amount_recovered = int.Parse(drChild_table["interest_amount_recovered"].ToString());
                        int total_amount_recovered = int.Parse(drChild_table["total_amount_recovered"].ToString());

                        interest_amount_recovered = interest_amount_recovered + x_amountpaid;
                        principal_amount_recovered = principal_amount_recovered + principal_open_amount;
                        total_amount_recovered = interest_amount_recovered + principal_amount_recovered;



                        if (os_principal_amount == 0 && os_interest_amount == 0)
                        {

                            qry = "update pr_emp_adv_loans_child set active=0, interest_recovered_flag=1, principal_recovered_flag=1, principal_amount_recovered=" + principal_amount_recovered + ",interest_amount_recovered=" + interest_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_interest_amount=" + os_interest_amount + ", os_total_amount= " + os_total_amount + " where id=" + id + " and active=1;";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            //todo

                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id=" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));


                        }
                        else
                        {
                            qry = "update pr_emp_adv_loans_child set principal_recovered_flag=1, principal_amount_recovered=" + principal_amount_recovered + ",interest_amount_recovered=" + interest_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_interest_amount=" + os_interest_amount + ", os_total_amount= " + os_total_amount + " where id=" + id + " and active=1;";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id=" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));

                        }


                    }

                }
                //updating only os_interest amount when debited
                else if (hidden_os_principal_amount == 0)
                {
                    foreach (DataRow drChild_table in dtChildLoans.Rows)
                    {
                        //installments calculation
                        int emi = int.Parse(drChild_table["installment_amount"].ToString());
                        int no_of_installments_cover = 0;
                        if (emi != null && emi != 0)
                        {
                            no_of_installments_cover = int.Parse(Math.Round(Convert.ToDecimal(amountpaid / emi)).ToString());
                        }
                        int completed_installments = int.Parse(drChild_table["completed_installment"].ToString());
                        int remaining_installments = int.Parse(drChild_table["remaining_installment"].ToString());
                        int total_installment = int.Parse(drChild_table["total_installment"].ToString());
                        int interest_installment_amount = int.Parse(drChild_table["interest_installment_amount"].ToString());
                        //------------------------
                        if (Fradio == "Full Clearing")
                        {
                            completed_installments = total_installment;
                            remaining_installments = 0;
                        }
                        else
                        {
                            completed_installments = completed_installments + no_of_installments_cover;
                            remaining_installments = remaining_installments - no_of_installments_cover;
                        }
                        //------------------------

                        int emp_adv_loans_child_mid = int.Parse(drChild_table["id"].ToString());
                        emp_adv_loans_mid = int.Parse(drChild_table["emp_adv_loans_mid"].ToString());
                        int loan_amount = int.Parse(drChild_table["loan_amount"].ToString());

                        int principal_amount_recovered = int.Parse(drChild_table["principal_amount_recovered"].ToString());
                        int principal_open_amount = loan_amount - (principal_amount_recovered);
                        int principal_paid_amount = principal_open_amount;
                        int interest_accured = int.Parse(drChild_table["interest_accured"].ToString());

                        //interest related calculations
                        int interest_open_amount = int.Parse(drChild_table["os_interest_amount"].ToString());
                        int interest_paid_amount = amountpaid;
                        int interest_balance_amount = interest_open_amount - amountpaid;
                        
                        if (interest_balance_amount <= interest_installment_amount)
                        {
                            interest_installment_amount = interest_balance_amount;
                        }
                       
                        //1. trans_id
                        sbqry.Append(GenNewTransactionString());

                        int NewNumIndex = 0;

                        string qry = "";
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_adv_loans_adjustments", NewNumIndex));
                        //qry = "Update pr_emp_adv_loans_adjustments set Active = 0 where id=" + id + ";";
                        //sbqry.Append(qry);


                        //update loan_adjustments table
                        qry = "update pr_emp_adv_loans_adjustments set active=0 where emp_adv_loans_mid=" + emp_adv_loans_mid + " and emp_adv_loans_child_mid=" + emp_adv_loans_child_mid + ";";
                        sbqry.Append(qry);


                        //3. qry---insert into pr_emp_adv_loans_adjustments
                        qry = "Insert into pr_emp_adv_loans_adjustments ([id],[emp_adv_loans_mid],[emp_adv_loans_child_mid],[principal_open_amount],[principal_paid_amount],[principal_balance_amount],[interest_open_amount],[interest_paid_amount],[interest_balance_amount],[installments_paid],[payment_type],[payment_mode],[cash_paid_on],[covered_installments],[amount_paid],[active],[trans_id],fm,fy) values "
                           + "(@idnew" + NewNumIndex + "," + emp_adv_loans_mid + "," + emp_adv_loans_child_mid + "," + 0 + "," + 0 + "," + 0 + "," + interest_open_amount + "," + interest_paid_amount + "," + interest_balance_amount + "," + completed_installments + ",'" + Fradio + "','" + Sradio + "', '" + paidondate + "'," + no_of_installments_cover + "," + amountpaid + ",1, @transidnew,'" + fm + "'," + fy + ");";
                        sbqry.Append(qry);


                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_adv_loans_adjustments", "@idnew" + NewNumIndex, ""));


                        // update loans_child table

                        int interest_amount_recovered = int.Parse(drChild_table["interest_amount_recovered"].ToString());
                        int total_amount_recovered = int.Parse(drChild_table["total_amount_recovered"].ToString());

                        interest_amount_recovered = interest_amount_recovered + amountpaid;
                        total_amount_recovered = interest_amount_recovered + principal_amount_recovered;


                        if (os_interest_amount == 0)
                        {
                            qry = "update pr_emp_adv_loans_child set interest_recovered_flag=1,active=0, interest_amount_recovered=" + interest_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_interest_amount=" + os_interest_amount + ", os_total_amount= " + os_total_amount + " where id=" + id + " and active=1;";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            //todo
                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id=" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));


                        }

                        else
                        {
                            qry = "update pr_emp_adv_loans_child set interest_amount_recovered=" + interest_amount_recovered + ",total_amount_recovered=" + total_amount_recovered + " , os_principal_amount=" + os_principal_amount + ", os_interest_amount=" + os_interest_amount + ", os_total_amount= " + os_total_amount + " where id=" + id + " and active=1;";
                            sbqry.Append(qry);

                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans_child", id.ToString(), amount.ToString()));

                            qry = "Update pr_emp_adv_loans set completed_installment =" + completed_installments + ", interest_installment_amount=" + interest_installment_amount + ",remaining_installment =" + remaining_installments + ", total_recovered_amount =" + total_amount_recovered + " where id=" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1; ";
                            sbqry.Append(qry);
                            sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_adv_loans", id.ToString(), amount.ToString()));
                        }
                    }


                }
            }

            //updating db if os_principal and os_interest both amounts debited at a time
            
            int loanPkId = 0;


            sbqry.Append("Select @idnew1;");
            int partPaymentId = await _sha.Run_INS_ExecuteScalar(sbqry.ToString());

            string qryLoan = "select id from pr_emp_adv_loans_child where emp_adv_loans_mid=" + emp_adv_loans_mid + " and active=1;";

            DataTable dtloans = await _sha.Get_Table_FromQry(qryLoan);
            if (dtloans.Rows.Count == 0)
            {
                await _sha.Run_UPDDEL_ExecuteNonQuery("Update pr_emp_adv_loans set active = 0 where id=" + emp_adv_loans_mid + "and emp_code=" + emp_id + " and active=1;");
            }

            LoansAdvancesBus lbus = new LoansAdvancesBus(_LoginCredential);

            await lbus.LoadLedgerReportNEW(loanPkId, partPaymentId);

            //if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            //{
            return "I#Loan Adjustments#Data Added Successfully";
            //}
            //else
            //{
            //    return "E#Error 123#Error 456";
            //}


        }

    }
}
