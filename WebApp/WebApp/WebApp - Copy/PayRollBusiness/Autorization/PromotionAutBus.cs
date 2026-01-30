using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using PayrollModels.Masters;
using System.Data;
using System.Web.Script.Serialization;
namespace PayRollBusiness.Autorization
{
    public class PromotionAutBus: BusinessBase
    {
        public PromotionAutBus(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        public class PrPromotion
        {
            // public int id { get; set; }
            public string Id { get; set; }
            public string display { get; set; }
            public string Value { get; set; }
            public string row_type { get; set; }
        }

        public async Task<string> GetEmpPRDesignations(int Empid)
        {
            string query1 = "SELECT id,code FROM Designations";
            DataTable dt1 = await _sha.Get_Table_FromQry(query1);
            var resultJson = JsonConvert.SerializeObject(new { designations = dt1 });
            return resultJson;
        }

        public async Task<string> GetEmpPRCategories(int Empid)
        {
            string query1 = "SELECT id,category FROM pr_emp_categories";
            DataTable dt1 = await _sha.Get_Table_FromQry(query1);
            var resultJson = JsonConvert.SerializeObject(new { categories = dt1 });
            return resultJson;
        }


        public async Task<IList<PrPromotion>> GetEmpPRDetails(int Empid)
        {
            int empidpr = 0;
            string qrypr = "Select id as Id from employees where empid=" + Empid + "";
            DataTable pdt1 = await _sha.Get_Table_FromQry(qrypr);
            foreach (DataRow dr in pdt1.Rows)
            {
                empidpr = Convert.ToInt32(dr["Id"]);

            }
            string qryGetPRfields = "SELECT top(1)  eprom.Id,eprom2.EmpId, FORMAT (eprom.EffectiveFrom, 'yyyy-MM-dd') as promotion_date,des.Name as desig,eprom.fy,eprom.fm,eprom.category,eprom.new_basic as basic_pay_fixed,FORMAT (eprom.incre_due_date, 'yyyy-MM-dd') as incre_due_date,eprom.senoirity_order,eprom.active,case when eprom.Id is null then 'N' else 'U' end as row_type FROM Employees eprom2 left join Employee_Transfer eprom on eprom2.Id = eprom.EmpId and eprom.active = 1 and eprom.authorisation=0 join Designations d on eprom2.CurrentDesignation= d.id join Designations des on des.id=eprom.newdesignation WHERE eprom.EmpId='" + empidpr + "' order by Id desc";

            DataSet dsGetPRfields = await _sha.Get_MultiTables_FromQry(qryGetPRfields);
            IList<PrPromotion> lstPRData = new List<PrPromotion>();
            DataTable dt = await _sha.Get_Table_FromQry(qryGetPRfields);
            if (dt.Rows.Count > 0)
            {

                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    display = "New Category :",
                    row_type = dt.Rows[0]["row_type"].ToString(),
                    Id = "category",
                    Value = dt.Rows[0]["category"].ToString()
                });
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "desig",
                    display = "New Designation:",
                    row_type = dt.Rows[0]["row_type"].ToString(),
                    Value = dt.Rows[0]["desig"].ToString()
                });
                //lstPRData.Add(new PrPromotion
                //{
                //    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                //    Id = "basic_pay",
                //    display = "New basic Pay:",
                //    row_type = dt.Rows[0]["row_type"].ToString(),
                //    Value = dt.Rows[0]["basic_pay"].ToString()
                //});
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "basic_pay_fixed",
                    display = "New Basic Pay:",
                    row_type = dt.Rows[0]["row_type"].ToString(),
                    Value = dt.Rows[0]["basic_pay_fixed"].ToString()
                });
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "promotion_date",
                    row_type = dt.Rows[0]["row_type"].ToString(),
                    display = "New Promotion Date:",
                    Value = dt.Rows[0]["promotion_date"].ToString()
                });

                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "incre_due_date",
                    row_type = dt.Rows[0]["row_type"].ToString(),
                    display = "New Inc Due Date :",
                    Value = dt.Rows[0]["incre_due_date"].ToString()

                });



                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "senoirity_order",
                    display = "Seniority Order:",
                    row_type = dt.Rows[0]["row_type"].ToString(),
                    Value = dt.Rows[0]["senoirity_order"].ToString()
                });

            }
            else
            {
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    display = "New Category :",
                    row_type = "",
                    Id = "category",
                    Value = "",
                });
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "desig",
                    display = "New Designation:",
                    row_type = "",
                    Value = "",

                });
                //lstPRData.Add(new PrPromotion
                //{
                //    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                //    Id = "basic_pay",
                //    display = "New basic Pay:",
                //    row_type = "",
                //    Value = "",
                //});
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "basic_pay_fixed",
                    display = "New Basic Pay:",
                    row_type = "",
                    Value = "",
                });
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "promotion_date",
                    row_type = "",
                    display = "New Promotion Date:",
                    Value = "",
                });

                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "incre_due_date",
                    row_type = "",
                    display = "New Inc Due Date :",
                    Value = "",

                });



                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "senoirity_order",
                    display = "Seniority Order:",
                    row_type = "",
                    Value = "",
                });
            }

            return lstPRData;
        }


        public async Task<IList<PrPromotion>> GetEmpPRDetailsactive1(int Empid)
        {
            int empidpr = 0;
            string qrypr = "Select id as Id from employees where empid=" + Empid + "";
            DataTable pdt1 = await _sha.Get_Table_FromQry(qrypr);
            foreach (DataRow dr in pdt1.Rows)
            {
                empidpr = Convert.ToInt32(dr["Id"]);

            }
            string qryGetPRfields = "SELECT top(1) et.Empid,et.Id,d.name as desig, FORMAT (EffectiveFrom, 'yyyy-MM-dd') as promotion_date,NewDesignation as desigs,fy,fm,category,new_basic as basic_pay_fixed,FORMAT (incre_due_date, 'yyyy-MM-dd') as incre_due_date,senoirity_order,active from Employee_transfer et join designations d on d.id=et.NewDesignation   WHERE EmpId='" + empidpr+ "' and active=1 and authorisation=0 order by id desc";

            DataSet dsGetPRfields = await _sha.Get_MultiTables_FromQry(qryGetPRfields);
            IList<PrPromotion> lstPRData = new List<PrPromotion>();
            DataTable dt = await _sha.Get_Table_FromQry(qryGetPRfields);
            if (dt.Rows.Count > 0)
            {

                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    display = "New Category :",
                  //  row_type = dt.Rows[0]["row_type"].ToString(),
                    Id = "category",
                    Value = dt.Rows[0]["category"].ToString()
                });
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "desig",
                    display = "New Designation:",
                   // row_type = dt.Rows[0]["row_type"].ToString(),
                    Value = dt.Rows[0]["desig"].ToString()
                });
                //lstPRData.Add(new PrPromotion
                //{
                //    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                //    Id = "basic_pay",
                //    display = "New basic Pay:",
                //   // row_type = dt.Rows[0]["row_type"].ToString(),
                //    Value = dt.Rows[0]["basic_pay"].ToString()
                //});
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "basic_pay_fixed",
                    display = "New Basic Pay:",
                   // row_type = dt.Rows[0]["row_type"].ToString(),
                    Value = dt.Rows[0]["basic_pay_fixed"].ToString()
                });
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "promotion_date",
                   // row_type = dt.Rows[0]["row_type"].ToString(),
                    display = "New Promotion Date:",
                    Value = dt.Rows[0]["promotion_date"].ToString()
                });

                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "incre_due_date",
                  //  row_type = dt.Rows[0]["row_type"].ToString(),
                    display = "New Inc Due Date :",
                    Value = dt.Rows[0]["incre_due_date"].ToString()

                });



                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "senoirity_order",
                    display = "Seniority Order:",
                 //   row_type = dt.Rows[0]["row_type"].ToString(),
                    Value = dt.Rows[0]["senoirity_order"].ToString()
                });

            }
            else
            {
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    display = "New Category :",
                   // row_type = "",
                    Id = "category",
                    Value = "",
                });
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "desig",
                    display = "New Designation:",
                   // row_type = "",
                    Value = "",

                });
                //lstPRData.Add(new PrPromotion
                //{
                //    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                //    Id = "basic_pay",
                //    display = "New basic Pay:",
                //  //  row_type = "",
                //    Value = "",
                //});
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "basic_pay_fixed",
                    display = "New Basic Pay:",
                   // row_type = "",
                    Value = "",
                });
                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "promotion_date",
                   // row_type = "",
                    display = "New Promotion Date:",
                    Value = "",
                });

                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "incre_due_date",
                  //  row_type = "",
                    display = "New Inc Due Date :",
                    Value = "",

                });



                lstPRData.Add(new PrPromotion
                {
                    // id = Convert.ToInt32(dt.Rows[0]["id"]),
                    Id = "senoirity_order",
                    display = "Seniority Order:",
                   // row_type = "",
                    Value = "",
                });
            }

            return lstPRData;
        }

        public async Task<string> AddPRPayDetails(CommonPostDTO Values)
        {
            int empidpr = 0;
            string qrypr = "Select id as Id from employees where empid=" + Values.EntityId + "";
            DataTable pdt1 = await _sha.Get_Table_FromQry(qrypr);
            foreach (DataRow dr in pdt1.Rows)
            {
                empidpr = Convert.ToInt32(dr["Id"]);

            }
            try
            {
                int FY = DateTime.Now.Year + 1;
                string FM = DateTime.Now.ToString("MM-dd-yyyy");
                int EfEmpid = Values.EntityId;
                string prdate = "";
                string incremdate = "";
                string bpay = "";
                string des = "";
                string cate = "";
                string seniority = "";
                int NewNumIndex = 0;
                int? emp_code = 0;
                string promotion_date = "";
                string bpayfixed = "";

                StringBuilder sbqry = new StringBuilder();
                //1. trans_id
                sbqry.Append(GenNewTransactionString());

                if (Values.object1 != null)
                {
                    var PromotionInfo = Values.object1;

                    foreach (var rdata in PromotionInfo)
                    {

                        if (rdata.Id == "promotion_date")
                        {
                            prdate = rdata.Value;
                        }
                        if (rdata.Id == "incre_due_date")
                        {
                            incremdate = rdata.Value;
                        }

                        if (rdata.Id == "desig")
                        {
                            des = rdata.Value;
                        }
                        if (rdata.Id == "category")
                        {
                            cate = rdata.Value;
                        }
                        if (rdata.Id == "senoirity_order")
                        {
                            seniority = rdata.Value;
                        }
                        //if (rdata.Id == "basic_pay")
                        //{
                        //    bpay = rdata.Value;
                        //}
                        if (rdata.Id == "basic_pay_fixed")
                        {
                            bpayfixed = rdata.Value;
                        }
                    }
                    NewNumIndex++;
                    sbqry.Append(GetNewNumStringArr("Employee_Transfer", NewNumIndex));
                    string qrey = "Select EmpId as emp_code ,fy,fm,category,NewDesignation as desig,new_basic as basic_pay_fixed,EffectiveFrom as promotion_date,incre_due_date,senoirity_order,authorisation,active from Employee_Transfer where Active=1 and Authorisation=0 and EmpId=" + empidpr + "; ";
                    DataTable dt1 = await _sha.Get_Table_FromQry(qrey);
                    string desig = "";
                    if (dt1.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt1.Rows)
                        {
                            string empcode = dt1.Rows[0]["emp_code"].ToString();
                            string fy = dt1.Rows[0]["fy"].ToString();
                            string fm = Convert.ToDateTime(dt1.Rows[0]["fm"]).ToString("yyyy/MM/dd");
                             desig = dt1.Rows[0]["desig"].ToString();
                          //  string basic_pay = dt1.Rows[0]["basic_pay"].ToString();
                            string basic_pay_fixed = dt1.Rows[0]["basic_pay_fixed"].ToString();
                            promotion_date = Convert.ToDateTime(dt1.Rows[0]["promotion_date"]).ToString("yyyy/MM/dd");
                            string incre_due_date = Convert.ToDateTime(dt1.Rows[0]["incre_due_date"]).ToString("yyyy/MM/dd");
                            string senoirity_order = dt1.Rows[0]["senoirity_order"].ToString();
                            string authorisation = dt1.Rows[0]["authorisation"].ToString();
                            string active = dt1.Rows[0]["active"].ToString();
                            //  string qrey3 = "Update pr_emp_promotion set authorisation=0  where emp_code=" + EfEmpid + " and active=1; ";
                            // sbqry.Append(qrey3);
                            string qrey2 = "Update Employee_Transfer set Active=0,Authorisation=1,EmpId='" + empidpr + "',fy='" + FY + "',fm='" + FM + "',category='" + cate + "',NewDesignation=" + desig + ",new_basic=" + bpayfixed + ",EffectiveFrom='" + prdate + "',incre_due_date='" + incremdate + "',senoirity_order='" + seniority + "'  where EmpId=" + empidpr + " and active=1; ";
                            sbqry.Append(qrey2);

                        }
                    }
                    else
                        {
                        string qrey2 = "Update Employee_Transfer set Active=1,Authorisation=0,EmpId='" + empidpr + "',fy='" + FY + "',fm='" + FM + "',category='" + cate + "',NewDesignation=" + desig + ",new_basic=" + bpayfixed + ",EffectiveFrom='" + prdate + "',incre_due_date='" + incremdate + "',senoirity_order='" + seniority + "'  where EmpId=" + empidpr + " and  authorisation=1; ";
                        sbqry.Append(qrey2);
                        string qry = "INSERT INTO Employee_Transfer (Id,EmpId,fy,fm,category,NewDesignation,new_basic,EffectiveFrom,incre_due_date,senoirity_order,active,trans_id,authorisation) VALUES(@idnew" + NewNumIndex + ",(select id from employees where empid=" + Values.EntityId + "),  " + FY + ",'" + FM + "','" + cate + "'," + desig +  ",'" + bpayfixed + "',' " + prdate + "','" + incremdate + "','" + seniority + "', 1, @transidnew,1); ";
                        sbqry.Append(qry);
                       
                    }
                    string qry2 = " select id from designations where Name='" + des + "'";
                    DataTable dt = await _sha.Get_Table_FromQry(qry2);
                    int curdes = 0;
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string desi = dr["id"].ToString();


                            curdes = Convert.ToInt32(desi);
                        }
                    }
                    if (incremdate != "")
                    {
                        if (Convert.ToDateTime(incremdate) <= DateTime.Now)
                        {
                            string qry3 = "Update Employees set CurrentDesignation=" + curdes + " where empid='" + Values.EntityId + "'";
                            sbqry.Append(qry3);
                        }
                    }

                    if (incremdate != "")
                    {
                        if (Convert.ToDateTime(incremdate) <= DateTime.Now)
                        {
                            string qry3 = "UPDATE PF SET AMOUNT =" + bpayfixed + " FROM pr_emp_pay_field PF " +
                                " JOIN pr_earn_field_master FM ON PF.m_id = FM.id WHERE FM.name like '%BASIC%' AND PF.emp_code =" + Values.EntityId + " and PF.active=1";
                            //string qry3 = "Update pr_emp_pay_field set amount='"+ bpayfixed + "' where emp_code='" + Values.EntityId + "' and active=1 ";
                            sbqry.Append(qry3);
                        }
                    }

                }                                 
                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                {
                    return "I#Promotion#Promotion Details Authorized Successfully..!!";
                }
                else
                {
                    return "E#Promotion#Error While Promotion Data Creation";
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
