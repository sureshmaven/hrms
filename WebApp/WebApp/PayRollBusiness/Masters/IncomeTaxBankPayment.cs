using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;

namespace PayRollBusiness.Masters
{
    public class IncomeTaxBankPayment : BusinessBase
    {
        
        public IncomeTaxBankPayment(LoginCredential loginCredential) : base(loginCredential)
        {

        }

        public async Task<IList<IncomTaxBankPaymentmodel>> getIncomeTaxBankPayment()
        {
            string IncomeTax = "SELECT id,fy,format(fm,'MMM') as fm,format(payment_date,'yyyy-MM-dd') as payment_date,bank_name,amount,challan_no FROM pr_incometax_bank_payment";
            DataTable dt = await _sha.Get_Table_FromQry(IncomeTax);
            IList<IncomTaxBankPaymentmodel> lstDept = new List<IncomTaxBankPaymentmodel>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstDept.Add(new IncomTaxBankPaymentmodel
                    {
                        id = dr["id"].ToString(),
                        fy = dr["fy"].ToString(),
                        fm = dr["fm"].ToString(),
                        payment_date = dr["payment_date"].ToString(),
                        bank_name = dr["bank_name"].ToString(),
                        amount = dr["amount"].ToString(),
                        challan_no = dr["challan_no"].ToString()

                    });
                }
            }
            catch (Exception ex)
            {

            }
            return lstDept;
        }

        //public async Task<string> getIncomeTaxBankPayment()
        //{

        //    string IncomeTaxdata = "SELECT id,fy,fm,payment_date,bank_name,amount,challan_no FROM pr_incometax_bank_payment";
        //    DataTable dt = await _sha.Get_Table_FromQry(IncomeTaxdata);
        //    var resultJson = JsonConvert.SerializeObject(dt);
        //    return resultJson;


        //}

        public async Task<string> updateIncomeTaxBankPayment(CommonPostDTO Values)
        {
            var multData = Values.IncomeTaxBank;
            string qry = "";
            //string temp = "";
            StringBuilder sbqry = new StringBuilder();
            
            sbqry.Append(GenNewTransactionString());

           foreach (var IncomeTaxBank in multData)
            {
                
                if (IncomeTaxBank.Action == "Update")
                {
                    
                    
                    qry = " Update pr_incometax_bank_payment SET Payment_Date='" + IncomeTaxBank.payment_date + "',Bank_Name='" + IncomeTaxBank.bank_name + "',Amount = " + IncomeTaxBank.amount + ",Challan_no = '" + IncomeTaxBank.challan_no + "' where id=" + IncomeTaxBank.id + " ";
                    sbqry.Append(qry);                 
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_incometax_bank_payment", IncomeTaxBank.id.ToString(), ""));
                   
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



    