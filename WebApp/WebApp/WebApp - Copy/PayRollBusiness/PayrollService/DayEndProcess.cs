using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollBusiness.PayrollService
{
    public class DayEndProcess : BusinessBase
    {

        log4net.ILog _logger = null;
        public DayEndProcess(LoginCredential loginCredential, log4net.ILog logger) : base(loginCredential)
        {
            _logger = logger;
        }

        public async Task ServiceStarting(string servicename)
        {
            string qryIns = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action]) "
                + "VALUES(getdate(),'" + servicename + "','Start');";
            qryIns += "SELECT CAST(SCOPE_IDENTITY() as int);";
            await _sha.Run_INS_ExecuteScalar(qryIns);
        }
        public async Task ServiceStoping(string servicename)
        {
            string qryIns = "INSERT INTO hrms_scheduler_log([dttime],[Type],[Action]) "
                + "VALUES(getdate(),'" + servicename + "','Stop');";
            qryIns += "SELECT CAST(SCOPE_IDENTITY() as int);";

            await _sha.Run_INS_ExecuteScalar(qryIns);
        }
        //*** End of common code ***





        //every day adding pr_emp_jaib_caib_general amount to pr_emp_pay_field amount on current date= incre_due_date
        public async Task UpdateIncrementAmount()
        {

            string qryfmfy = " select fm,fy from pr_month_details where active=1";

            DataTable dtfm = await _sha.Get_Table_FromQry(qryfmfy);

            DateTime dtFm = Convert.ToDateTime(dtfm.Rows[0]["fm"].ToString());
            int FY = Convert.ToInt32(dtfm.Rows[0]["fy"].ToString());

            string FM = dtFm.ToString("MM-dd-yyyy");

            string Incr_WEFdate = "";
            string Emp_code = "";
            string No_of_incr = "";
            string Incr_type = "";
            string incrementamt = "";
            string jcid = "";
            string Basicamount = "";

            string m_id = "";
            string m_type = "";
            string Id = "";
            string EmpId = "";
            string qry = "";

            //int NewNumIndex = 0;
            StringBuilder sbqry = null;

            //get all emp jaiib/caiib for today date
            string qrygetincramoutdetails = "select id,emp_id,emp_code, incr_incen_type, no_of_inc, incr_WEF_date,Incrementamt from pr_emp_jaib_caib_general general " +
                "where (Month(general.incr_WEF_date) =(select Month(fm) from pr_month_details where Active=1))  AND (Year(general.incr_WEF_date)=(select Year(fm) from pr_month_details where Active=1)) AND general.active = 1 AND general.authorisation = 1";
            DataTable dtincr = await _sha.Get_Table_FromQry(qrygetincramoutdetails);

            foreach (DataRow dr_incr in dtincr.Rows) //process every record one by one
            {
                sbqry = new StringBuilder();
                //trans_id
                sbqry.Append(GenNewTransactionString());
                sbqry.Append(GetNewNumString("pr_emp_pay_field"));

                EmpId = dr_incr["emp_id"].ToString();
                Emp_code = dr_incr["emp_code"].ToString();
                //Emp_code = "452";
                No_of_incr = dr_incr["no_of_inc"].ToString();
                Incr_type = dr_incr["incr_incen_type"].ToString();
                Incr_WEFdate = dr_incr["incr_WEF_date"].ToString();
                incrementamt = dr_incr["Incrementamt"].ToString();
                jcid = dr_incr["id"].ToString();
                
                ////Get basicamount from pr_emp_pay_field
                //string qrybasicamount = "select c.emp_id,c.id,c.m_id,c.m_type,c.amount from pr_earn_field_master m " +
                //    "join pr_emp_pay_field c on m.id=c.m_id where m.name='Basic' and c.emp_code=" + Emp_code + " and c.active=1";

                //DataTable dtbasic = await _sha.Get_Table_FromQry(qrybasicamount);

                //if (dtbasic.Rows.Count > 0)
                //{
                //    DataRow row = dtbasic.Rows[0];

                //    Basicamount = row["amount"].ToString();
                //    Id = row["id"].ToString();
                //    m_id = row["m_id"].ToString();
                //    m_type = row["m_type"].ToString();
                //    EmpId = dr_incr["emp_id"].ToString();
                //}

                int n_incr_amount = Convert.ToInt32(No_of_incr) * Convert.ToInt32(incrementamt);
                //int currentbasic = Convert.ToInt32(Basicamount) + n_incr_amount;

                //deactive existing emp pay fields 
                //qry = "Update pr_emp_pay_field set active=0, trans_id=@transidnew where emp_code=" + Emp_code + "" +
                //    " and m_id=" + m_id + ";";
                //sbqry.Append(qry);
                //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_pay_field", Id, ""));

                //inserting to pr_emp pay fields with new basic amount
                if (Incr_type == "CAIIB")
                {
                    string getqry = "select id,name,type from pr_earn_field_master where name like '%CAIIB%' and type='pay_fields';";
                    DataTable dt = await _sha.Get_Table_FromQry(getqry);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow efm = dt.Rows[0];
                        m_id = efm["id"].ToString();
                        m_type = efm["type"].ToString();
                    }
                    sbqry.Append("Update pr_emp_pay_field set active=0 where m_id=" + m_id + " and emp_code=" + Emp_code + " and m_type='" + m_type + "' and active=1;");
                    
                    qry = "Insert into pr_emp_pay_field (id,emp_id,emp_code,fy,fm,m_id,m_type,amount,active,trans_id) " +
                        "values(@idnew," + EmpId + "," +
                        "" + Emp_code + ",'" + FY + "','" + FM + "'," + m_id + ",'"
                        + m_type + "'," + incrementamt + ",1,@transidnew);";
                    sbqry.Append(qry);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_pay_field", "@idnew", ""));
                }
                else if (Incr_type == "JAIIB")
                {
                    string getqry = "select id,name,type from pr_earn_field_master where name like '%JAIIB%' and type='pay_fields';";
                    DataTable dt = await _sha.Get_Table_FromQry(getqry);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow efm = dt.Rows[0];
                        m_id = efm["id"].ToString();
                        m_type = efm["type"].ToString();

                    }
                    sbqry.Append("Update pr_emp_pay_field set active=0 where m_id=" + m_id + " and emp_code=" + Emp_code + " and m_type='"+ m_type + "' and active=1;");
                    
                    qry = "Insert into pr_emp_pay_field (id,emp_id,emp_code,fy,fm,m_id,m_type,amount,active,trans_id) " +
                        "values(@idnew," + EmpId + "," +
                        "" + Emp_code + ",'" + FY + "','" + FM + "'," + m_id + ",'"
                        + m_type + "'," + incrementamt + ",1,@transidnew);";
                    sbqry.Append(qry);
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_pay_field", "@idnew", ""));
                }


                //deactive JAIIB/CAIIB authorize record 
                qry = "Update pr_emp_jaib_caib_general set active=0, trans_id=@transidnew where emp_code=" + Emp_code + "" +
                    " and active = 1 and authorisation = 1 and incr_incen_type='"+ Incr_type + "';";
                sbqry.Append(qry);
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_jaib_caib_general", jcid, ""));

                try
                {
                    await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString());
                }
                catch (Exception ex)
                {
                    _logger.Info(sbqry.ToString());
                    _logger.Error(ex.Message);
                    _logger.Error(ex.StackTrace);
                }

            }
        }


        public async Task UpdatePromotionPay()
        {

            int emp_code;
            int id;
            string qryUpdatePromotion = "";
            try
            {
                string currDate = DateTime.Now.ToString("yyyy-MM-dd");
                //string date = currDate.ToString("yyyy-MM");
                string[] date = currDate.Split('-');
                string year = date[0];
                string month = date[1];
                string qryPromDetails = " SELECT  e.EmpId as emp_code, d.id, p.new_basic as basic_pay_fixed, incre_due_date FROM Employee_Transfer p join Designations d on p.NewDesignation=d.Id join pr_month_details m on month(p.incre_due_date) = (" + month +") and year(p.incre_due_date)= ("+ year +") join employees e on e.id=p.empid where p.authorisation=1 and m.active=1;";


                DataTable dtempPromDetails = await _sha.Get_Table_FromQry(qryPromDetails);

                if (dtempPromDetails.Rows.Count > 0)
                {
                    foreach (DataRow drempPromDetails in dtempPromDetails.Rows)
                    {
                        emp_code = int.Parse(drempPromDetails["emp_code"].ToString());
                        int basic_pay_fixed = int.Parse(drempPromDetails["basic_pay_fixed"].ToString());
                        id = int.Parse(drempPromDetails["id"].ToString());
                        qryUpdatePromotion += "UPDATE PF SET AMOUNT =" + basic_pay_fixed + " FROM pr_emp_pay_field PF JOIN pr_earn_field_master FM ON PF.m_id = FM.id WHERE FM.name like '%BASIC%' AND PF.emp_code =" + emp_code + " and PF.active=1;;";
                        qryUpdatePromotion += "update employees set CurrentDesignation=" + id + " where EmpId=" + emp_code + ";";
                        qryUpdatePromotion += "Update Employee_Transfer set active=0 where empid=(select id from employees where empid=" + emp_code + ") and month(incre_due_date) = (" + month + ") and year(incre_due_date)= (" + year + ");";
                        await _sha.Run_UPDDEL_ExecuteNonQuery(qryUpdatePromotion);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }


        }


    }
}
