using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayRollBusiness.Masters;
using PayrollModels;

namespace PayRollBusiness.Masters
{
    public class Form16ScreenBusiness : BusinessBase
    {
        public Form16ScreenBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }
        // get financial year
        public async Task<IList<LICReport>> GetForm16Data()
        {
            string qryfy = "select fy as fm_fy from pr_month_details where active=1;";
            int fm_fy = await _sha.Run_INS_ExecuteScalar(qryfy);

            IList<LICReport> fyear = new List<LICReport>();

            int fy = fm_fy - 1;
            int Id = 0;

            fyear.Add(new LICReport
            {
                Id = Id.ToString(),
                fY = "Select",

            });

            Id++;
            fyear.Add(new LICReport
            {

                Id = Id.ToString(),
                fY = (fy + "-" + (fm_fy)).ToString(),

            });

            for (int i = 1; i < 6; i++)
            {
                Id++;
                fm_fy--;
                fy--;
                fyear.Add(new LICReport
                {
                    Id = Id.ToString(),
                    fY = (fy + "-" + (fm_fy)).ToString(),

                });
            }

            return fyear;


        }
        // load payment details
        public async Task<string> getData(string mnth)
        {
            string fy = " ";
            string getqry = "";
            if (mnth != "Select")
            {
                fy = mnth;
                string[] arr = fy.Split('-');
                getqry = "select Convert(varchar,payment_date,105) as payment_date ,bsrcode_of_bank,challan_no from pr_form16_codes where fy= '" + arr[1] + "'";
            }
            else
            {
                fy = "0";
                getqry = "select Convert(varchar,payment_date,105) as payment_date ,bsrcode_of_bank,challan_no from pr_form16_codes where fy= '" + fy + "'";
            }
            DataTable dt = await _sha.Get_Table_FromQry(getqry);
            return JsonConvert.SerializeObject(dt);
        }

        //post pyment details
        public async Task<string> saveForm16Data(form16 Values)
        {
            IList<LICReport> dtfinancial = new List<LICReport>();
            DateTime gen_date = DateTime.Now;
            int FY = _LoginCredential.FY;
            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");

            StringBuilder sbqry = new StringBuilder();
            

            string Fstr = Values.fy;
            string[] arr = Fstr.Split('-');
            int fyear =Convert.ToInt32(arr[1]);
            string FQuery = "select fy as fy from pr_form16_codes";
            DataTable dt = await _sha.Get_Table_FromQry(FQuery);
            //int finYear = Convert.ToInt32(dt.Rows[0]["fy"]);
            bool isAvail = false;
            int finYear = 0;
            foreach (DataRow dr in dt.Rows)
            {
                finYear = Convert.ToInt32(dr["fy"].ToString());
                if(fyear == finYear)
                {
                    isAvail = true;
                }
                
            }
            //if(dtfinancial.Contains(fyear))
            if (!isAvail)
            {
                //1. trans_id
                sbqry.Append(GenNewTransactionString());

                // Inserting values
                string qry = "";

                int NewNumIndex = 0;
                DateTime str = Convert.ToDateTime(Values.payment_date);
                string paymentDate = str.ToString("yyyy-MM-dd");
                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_form16_codes", NewNumIndex));
                //3. qry
                qry = "Insert into pr_form16_codes ([id],[fy],[fm],[payment_date],[bsrcode_of_bank],[challan_no]) values "
                            + "(@idnew" + NewNumIndex + "," + fyear + ",'" + FM + "' ,'" + paymentDate + "', '" + Values.bsrcode + "','" + Values.challan_no + "');";
                sbqry.Append(qry);

                //4. transaction touch

                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_form16_codes", "@idnew" + NewNumIndex, ""));
            }

            if (sbqry.Length > 0)
            {

                if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))

                {
                    return "I#Form 16#Data Added Successfully";
                }
                else
                {
                    return "E#Error#Error While data Form 16 data Submission";
                }
            }
            else
            {
                return "E#Form 16# Alert!  Income Tax Payment is already Added for this Year";
            }
        }
    }
}
