using BusLogic.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BusLogic
{
    public class Payroll : CommonService
    {
        private string _dbConnStr;
        public Payroll(string dbConnStr) : base(dbConnStr)
        {
            _dbConnStr = dbConnStr;
        }

        //Earned Wages Calculation based on Fixed Wages
        public void Payrolldata()
        {
            try
            {

                string qry1 = "";
                string Fwdata = getAllEmpIdsFW();
                string data = getAllworkdaysFromtimesheet_Emp_Month();
                string Fwagedata = Fwdata.TrimEnd(',');
                var Fwages = Fwagedata.Split(',');
                string Fwagedata1 = data.TrimEnd(',');
                var Fwages1 = Fwagedata1.Split(',');

                foreach (var wdays in Fwages1)
                {
                    foreach (var emp in Fwages)
                    {

                        var arremp2 = wdays.Split('#');
                        var Empkid = arremp2[0];
                        var absent = arremp2[1];
                        var arremp = emp.Split('#');
                        var empid = arremp[0];
                        var BDate = arremp[10];
                        DateTime date = Convert.ToDateTime(BDate);
                        var years = date.Year;
                        var months = date.Month;
                        string Dates =DateTime.Parse(arremp[10]).ToString("yyyy-MM-dd");
                        if (arremp2[0] == arremp[0] && arremp2[2]== years.ToString() && arremp2[3] == months.ToString())
                        {
                            var FixedGross = arremp[1];
                            var FixedBasic = arremp[2];
                            var FixedHRA = arremp[3];
                            var FixedConveyance = arremp[4];
                            var FixedMedical = arremp[5];
                            var FixedSA = arremp[6];
                            var PFEmployer = arremp[7];
                            var ESIEmployer = arremp[8];
                            float CTC = Convert.ToInt32(arremp[9]);
                            int year = DateTime.Now.Year;
                            int month = DateTime.Now.Month -1;
                            
                            int workingdays = DateTime.DaysInMonth(year, month);//To be done 
                            int workdays = workingdays - Convert.ToInt32(arremp2[1]);//To be done
                            float Fbasic = Convert.ToInt32(arremp[2]);
                            float fixedHR = Convert.ToInt32(arremp[3]);
                            float fixconv = Convert.ToInt32(arremp[4]);
                            float fixmed= Convert.ToInt32(arremp[5]);
                            float fixsa= Convert.ToInt32(arremp[6]);
                            float lopamt= Convert.ToInt32(arremp[1]);
                            //Earned Basic
                            float EarnedBasic = (float)Math.Round((Fbasic / workingdays) * workdays, 0);
                            //Earned HRA
                            float EarnedHRA = (float)Math.Round((fixedHR / workingdays) * workdays,0);
                            //EarnedConveyance
                            float EarnedConveyance = (float)Math.Round((fixconv / workingdays) * workdays, 0);
                            //EarnedMedical
                            float EarnedMedical = (float)Math.Round((fixmed / workingdays) * workdays, 0);
                            //EarnedSA
                            float EarnedSA = (float)Math.Round((fixsa / workingdays) * workdays, 0);
                            //EarnedGross
                            float EarnedGross = EarnedBasic + EarnedHRA + EarnedConveyance + EarnedMedical + EarnedSA;
                            //LOPDays 
                            int LOPDays = workingdays - workdays;
                            //LOPamount
                            float LOPamount = (float)Math.Round(lopamt - EarnedGross, 0); //to be done
                                                                                                              //PF
                            float PF;
                            if (Fbasic <= 15000)
                            {
                                PF = (float)Math.Round(EarnedBasic*0.12, 0);
                            }
                            else
                            {
                                PF = 1800;
                            }
                            //ESI
                            double ESI;
                            if (Convert.ToInt32(arremp[1]) <= 21000)
                            {
                                ESI = (double)Math.Round(((1.75 * EarnedGross) / 100), 0);
                            }
                            else
                            {
                                ESI = 0;
                            }
                            //PT
                            float PT;
                            if (Convert.ToInt32(arremp[1]) <= 15000)
                            {
                                PT = 0;
                            }
                            else if (Convert.ToInt32(arremp[1]) >= 15000 && Convert.ToInt32(arremp[1]) <= 20000)
                            {
                                PT = 150;
                            }
                            else if (Convert.ToInt32(arremp[1]) > 20000)
                            {
                                PT = 200;
                            }
                            else
                            {
                                PT = 0;
                            }

                            //TDS
                            float TDS;
                            if (CTC > 250001 && CTC<=500000)
                            {
                               
                                TDS = (float)Math.Round(((0.05 * CTC) / 12), 0);
                            }
                            else if (CTC > 500001 && CTC <= 1000000)
                            {
                                 TDS = (float)Math.Round((((12500+((0.2) )* CTC)) / 12), 0);
                            }
                            else if (CTC > 1000000)
                            {
                                 TDS = (float)Math.Round((((112500 + ((0.3)) * CTC)) / 12), 0);
                            }
                            else
                            {
                             TDS = 0;
                            }
                           
                            //Loan
                            float Loan = 0;
                            //Others
                            float Others = 0;
                            //Totaldeductions

                            float Totaldeductions = PF + Convert.ToInt32(ESI) + PT + TDS + Loan + Others;
                            //NetSalary
                            float NetSalary = EarnedGross - Totaldeductions;
                            qry1 += " Insert into Payroll_EW(EmpId,EmpCode,Date,Active,WorkingDays,WorkDays,EarnedBasic,EarnedHRA,EarnedConveyance,EarnedMedical,EarnedSA,EarnedGross,LOPDays,LOPamount,PF,ESI,PT,TDS,Loan,Others,Totaldeductions,NetSalary)values(" + arremp[12] + "," + arremp[0] + ",'" +Dates +  "', 1 , " + workingdays + ", " + workdays + ", " + EarnedBasic + "," + EarnedHRA + ", " + EarnedConveyance + ", " + EarnedMedical + " ," + EarnedSA + ", " + EarnedGross + "," + LOPDays + ", " + LOPamount + ", " + PF + "," + ESI + "," + PT + ", " + TDS + ", " + Loan + "," + Others + "," + Totaldeductions + "," + NetSalary + "); ";
                        }
                    }
                }
                SqlHelper sh = new SqlHelper(_dbConnStr);
                    sh.Run_INS_ExecuteScalar(qry1);
               
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }
        private string getAllEmpIdsFromtimesheet_Emp_Month()
        {
            string retStr = "";

            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            string selQry = "Select distinct Id,UserId from timesheet_Emp_Month where year=Year(getdate()) and month=Month(getdate())";

            SqlHelper sh = new SqlHelper(_dbConnStr);
            DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);
            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["UserId"].ToString() + ",";
            }
            return retStr;
        }
        //Workingdays for all employees
        private string getAllworkdaysFromtimesheet_Emp_Month()
        {
            string retStr = "";

            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            string selQry = " select t.userId as Empcode,t.Year as Year,t.month as Month ," +
 " sum((case when D1_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')    then 1  else 0 end) " +
" +(case when D2_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')  then 1  else 0 end) " +
" + (case when D3_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')  then 1  else 0 end) " +
" + (case when D4_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')     then 1  else 0 end) " +
" + (case when D5_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
" + (case when D6_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
" + (case when D7_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
" + (case when D8_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
" + (case when D9_Status  in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')    then 1  else 0 end) " +
" + (case when D10_Status  in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')    then 1  else 0 end) " +
" + (case when D11_Status  in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')  then 1  else 0 end) " +
" + (case when D12_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')    then 1  else 0 end) " +
" + (case when D13_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
" + (case when D14_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
" + (case when D15_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')    then 1  else 0 end) " +
" + (case when D16_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
" + (case when D17_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
" + (case when D18_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')  then 1  else 0 end) " +
" + (case when D19_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
" + (case when D20_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')  then 1  else 0 end) " +
" + (case when D21_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')     then 1  else 0 end) " +
" + (case when D22_Status  in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
" + (case when D23_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')  then 1  else 0 end) " +
" + (case when D24_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')  then 1  else 0 end) " +
" + (case when D25_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB') then 1  else 0 end) " +
" + (case when D26_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')  then 1  else 0 end) " +
" + (case when D27_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')  then 1  else 0 end) " +
" + (case when D28_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
 " + (case when D29_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
  " + (case when D30_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB')   then 1  else 0 end) " +
  " + (case when D31_Status   in ('CL','ML','PL','MTL','C-OFF','PTL','LOP','SCL','EOL','AB') then 1  else 0 end)  )" +
                " as Workdays from timesheet_Emp_Month t join Employees e on t.UserId = e.EmpId where t.UserId=e.EmpId and t.Year=year(getdate()) and t.month =DATEPART(mm, (SELECT DATEADD(mm,-1, GETDATE()))) GROUP BY t.UserId,t.Year,t.Month;";

            SqlHelper sh = new SqlHelper(_dbConnStr);
            DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);
            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["Empcode"].ToString() + "#" + dr["Workdays"].ToString() + "#" + dr["Year"].ToString() + "#" + dr["Month"].ToString() + ",";
            }
            return retStr;
        }
        //Getting Employee fixed wages 
        private string getAllEmpIdsFW()
        {
            string retStr = "";

            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            string selQry = " select EmpId,EmpCode,Date,Active,EmpBranch,EmpDesignation,EmpEmailId,EmpPANno,EmpESIno,EmpUAN_PFno,EmpBankAccountno,FixedGross,FixedBasic,FixedHRA,FixedConveyance,FixedMedical,FixedSA,PFEmployer,ESIEmployer,CTC from Payroll_FW where Active=1";

            SqlHelper sh = new SqlHelper(_dbConnStr);
            DataTable dtEmpIds = sh.Get_Table_FromQry(selQry);
            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["EmpCode"].ToString() + "#" + dr["FixedGross"].ToString()  +"#" + dr["FixedBasic"].ToString() +"#" + dr["FixedHRA"].ToString() + "#" + dr["FixedConveyance"].ToString()  +"#" + dr["FixedMedical"].ToString()  +"#" + dr["FixedSA"].ToString()  +"#" + dr["PFEmployer"].ToString()  +"#" + dr["ESIEmployer"].ToString() +"#" + dr["CTC"].ToString() + "#" + dr["Date"].ToString()  +"#" + dr["Active"].ToString() + "#" + dr["EmpId"].ToString()+ ",";
            }
            return retStr;
        }
    }
}
    
