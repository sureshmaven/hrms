using HRMSApplication.Helpers;
using HRMSBusiness.Business;
using Mavensoft.DAL.Db;
using Newtonsoft.Json;
using PayRollBusiness;
using PayRollBusiness.Masters;
using PayRollBusiness.Process;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using static PayRollBusiness.Process.PFNonReyPayableProcessBusiness;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class ProcessController : Controller
    {
        string URL = WebConfigurationManager.AppSettings.Get("URL").ToString();
        CommonBusiness commBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());
        PayslipBusiness PRBUS = new PayslipBusiness(LoginHelper.GetCurrentUserForPR());
        ProcessBusiness PBus = new ProcessBusiness(LoginHelper.GetCurrentUserForPR());
        EncashmentBusiness Ebus=new EncashmentBusiness(LoginHelper.GetCurrentUserForPR());
        PFPayableBusinessProcess payL = new PFPayableBusinessProcess(LoginHelper.GetCurrentUserForPR());
        PFNonReyPayableProcessBusiness pfp = new PFNonReyPayableProcessBusiness(LoginHelper.GetCurrentUserForPR());
        TdsProcessBusiness tds = new TdsProcessBusiness(LoginHelper.GetCurrentUserForPR());
        TDSUpdateBusiness tdu = new TDSUpdateBusiness(LoginHelper.GetCurrentUserForPR());

        CalculatePFforRetirementBusiness RetDate = new CalculatePFforRetirementBusiness(LoginHelper.GetCurrentUserForPR());

        LoginCredential lCredentials = null;
        
        // GET: Payroll/Process
        public ActionResult Index()
        {
            ViewBag.SectionName = "Process";
            return View();
        }


        public ActionResult AllowanceProcess()
        {
            ViewBag.SectionName = "Process";
            return View();
        }

        [HttpGet]
        public async Task<string> getAllowanceData()
        {
            var empDetails = await PBus.getAllowancesdata();
            return JsonConvert.SerializeObject(empDetails);
        }

        #region Day & Month End Process
        public ActionResult MonthEnd()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        [HttpGet]
        public string MonthEndProcess()
        {
            //// Create a Process Object here. 
            //System.Diagnostics.Process process1 = new System.Diagnostics.Process();
            ////Working Directory Of .exe File. 
            //process1.StartInfo.WorkingDirectory = Request.MapPath("~/Areas/Payroll/PayrollExe");
            ////exe File Name. 
            //process1.StartInfo.FileName = Request.MapPath("HrmsScheduler.exe");
            ////Argement Which you have tp pass. 
            //process1.StartInfo.Arguments = "PrMonthEndProcess";
            //process1.StartInfo.LoadUserProfile = true;
            ////Process Start on exe.
            //process1.Start();
            //process1.WaitForExit();
            //process1.Close();
            string Success = "";
            
            try
            { 
            Process process = new Process();
            process.StartInfo.FileName = @URL;
            process.StartInfo.Arguments = "PrMonthEndProcess";
            process.Start();
            process.WaitForExit();
            Success = "I#MonthEnd#Month End Process Completed Successfully.";
            }
            catch
            {
                Success = "E#Error#Error in running Month End process";
            }

            //FormsAuthentication.SignOut();
            //Session.Clear();
            //Session.Abandon();
            // Session["UserCredential"] = null;
            return JsonConvert.SerializeObject(Success);
        }

        //DayEndProcess
        [HttpGet]
        public string DayEndPorcess()
        {
            string Success = "";
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = @URL;
                process.StartInfo.Arguments = "PrDayEndProcess";
                process.Start();
                process.WaitForExit();
                Success = "I#MonthEnd#Day End Process Completed Successfully"; ;

            }
            catch
            {
                Success = "E#Error#Error in running Day End process";
            }
            return JsonConvert.SerializeObject(Success);
        }

        //HourEndPorcess
        [HttpGet]
        public string HourEndPorcess()
        {
            string Success = "";
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = @URL;
                process.StartInfo.Arguments = "PrHourProcess";
                process.Start();
                process.WaitForExit();
                Success = "I#HourEnd#Hour End Process Completed Successfully"; ;

            }
            catch
            {
                Success = "E#Error#Error in running Hour End process";
            }
            return JsonConvert.SerializeObject(Success);
        }

        //Reverese HourEndPorcess
        [HttpGet]
        public string ReverseHourEndPorcess()
        {
            string Success = "";
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = @URL;
                process.StartInfo.Arguments = "ReRunPrHourProcess";
                process.Start();
                process.WaitForExit();
                Success = "I#HourEnd#Reverse Hour End Process Completed Successfully"; ;

            }
            catch
            {
                Success = "E#Error#Error in running Hour End process";
            }
            return JsonConvert.SerializeObject(Success);
        }





        //year end process


        [HttpGet]
        public string yearendprocess()
        {
            string Success = "";
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = @URL;
                process.StartInfo.Arguments = "PrYearEndProcess";
                process.Start();
                process.WaitForExit();
                Success = "I#YearEnd#Yesr End Process Completed Successfully"; ;

            }
            catch
            {
                Success = "E#Error#Error in running Year End process";
            }
            return JsonConvert.SerializeObject(Success);
        }

        [HttpPost]
        public string Getpassword(CommonPostDTO data)
        {
            string pwd = data.StringData;
            lCredentials = LoginHelper.GetCurrentUserForPR();
            LoginBus lgbus = new LoginBus();
            ViewBag.EmpCode = lCredentials.EmpCode;
            string EmpId =Convert.ToString( ViewBag.EmpCode);
           
            LoginResult lResult = lgbus.getLoginInformation(EmpId, pwd);
             //var pwd =  PBus.Getpassword(data);
            return JsonConvert.SerializeObject(lResult);
        }



        #endregion

        #region PostIncrements
        public ActionResult PostIncrements()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }

        [HttpGet]
        public async Task<string> getProcessStagIncrementDateChange()
        {
            var empDetails = await PBus.getAuthStagIncrementDateChange();
            return JsonConvert.SerializeObject(empDetails);
        }
        [HttpPost]
        public async Task<string> UpprocessStagIncDateChange(List<string> data)
        {
            var empDetails = await PBus.UpAuthStagIncDateChange(data);
            TempData["retMessage"] = empDetails;
            return empDetails;
        }
        #endregion

        public ActionResult EncashmentProcess()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        [HttpGet]
        public async Task<string> getEncashmentDetails()
        {
            var encashDetails = await Ebus.getEncashDetails();
            return JsonConvert.SerializeObject(encashDetails);
        }
       
        public async Task<string> ProcessEncashment(List<string> Values)
        {
            try
            {
                var encashDetails = await Ebus.processEncashDetails(Values);
                TempData["retMessage"] = encashDetails;
                return encashDetails;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        #region Payslip

        public async Task<ActionResult> PayslipUI()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            var result = await commBus.RunorRevertbuttonenabledisable();
            ViewBag.RunorRevertbuttonenabledisable = result;
            return View();
        }
        [HttpGet]
        public async Task<string> getPayslipservice()
        {
            var empDetails = await PRBUS.getPayslipservice();
            return JsonConvert.SerializeObject(empDetails);
        }
        // check box for stop salary
        [HttpGet]
        public async Task<string> empcodeslist()
         {
            var codeslist = await PRBUS.empcodeslist();
            return JsonConvert.SerializeObject(codeslist);
        }
        //'General','Suspended'
        [HttpGet]
        public async Task<string> empcodeslistGS(string Pay_Type)
        {
            var codeslistGS = await PRBUS.empcodeslistGS(Pay_Type);
            //var ids = JsonConvert.SerializeObject(codeslistGS);
            return JsonConvert.SerializeObject(codeslistGS);
        }
        // emp codes for textbox(emp ids) validation
        [HttpGet]
        public async Task<string> empcodesvalidation(string EmpCode)
        {
            var codeslist = await PRBUS.empcodes(EmpCode);
            return JsonConvert.SerializeObject(codeslist);
        }
         
        [HttpGet]
        public async Task<string> getFinalProcess(string emp_code, string Pay_Type)
        {
            var codeslist = await PRBUS.getFinalProcess(emp_code, Pay_Type);
            return JsonConvert.SerializeObject(codeslist);
        }

        [HttpPost]
        public async Task<string> InsertPayslip(string date, string[] EmpId,string Pay_Type ,string Finalprocess )
        {
            try
            {
                //string EmpCode = Values.EmpId;
                string Result = await PRBUS.InsertPayslip(date, EmpId, Pay_Type, Finalprocess );
                //TempData["AlertMessage"] = message;
                return Result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpPost]
        public async Task<string> DeletePayslip(CommonPostDTO values) 
        {
            string emp_codes = values.StringData.ToString();
            var empDetails = await PRBUS.DeletePayslip(emp_codes.ToString());
            TempData["retMessage"] = empDetails;
            return empDetails;
        }
        #endregion

        #region PFRepayable
        public ActionResult PFRepayableProcess()
        {

            ViewBag.SectionName = "Process";
            //  string query1 = "SELECT id,name FROM Designations";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();


            return View();
        }
        public async Task<string> getPfPayableData(string emp_code)
        {
            var doc = await payL.getPayableEmpDetails(emp_code);
            return JsonConvert.SerializeObject(doc);

        }
        public async Task<string> getLoanPurposePayable()
        {
            var res = await payL.getLoansPurpose();
            return JsonConvert.SerializeObject(res);

        }
        public async Task<string> getLoanPFPayable()
        {
            var res = await payL.getLoansPFPayable();
            return JsonConvert.SerializeObject(res);

        }
        [HttpPost]
        public async Task<string> SavePFPayableData(PFPayable Values)
        {
            if (Values != null)
            {

                return await payL.savePFPayableData(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }

        }
        #endregion

        #region PFNonRepayable
        public ActionResult PFNonRepayableProcess()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        public async Task<string> getPfNonPayableData(string emp_code)
        {
            var doc = await pfp.getNonPayableEmpDetails(emp_code);
            return JsonConvert.SerializeObject(doc);
        }
        [HttpPost]
        public async Task<string> processLoan(PFNonPayable Values)
        {
            if (Values.EntityId != null)
            {

                return await pfp.getProcess(Values);
            }
            else
            {
                return "E#Error#Please Enter Employee Code";
            }
        }
        #endregion
        #region TaxOptions
        public ActionResult TaxProcess()
        {
            ViewBag.SectionName = "Process";
            ViewBag.ReportTitle = "Tax Options";
            ViewBag.PdfOrientation = "landscape";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();

            ViewBag.LoginUserName = commBus._LoginCredential.EmpShortName;
            ViewBag.LoginBranch = commBus._LoginCredential.BranchCode;
            ViewBag.LoginBranchName = commBus._LoginCredential.BranchName;

            ViewBag.PdfNoOfCols = 5;
            ViewBag.ExportColumns = "columns: [1, 2, 3, 4, 5]";
            ViewBag.PdfColumnsWidths = "150,75,80,70";
            ViewBag.DataUrl = "/Payroll/Process/getTaxOptionData?taxoption=^1";
            ViewBag.ReportColumns = "[{ 'title': 'RowID', 'data': 'RowId' , 'visible': false } ," +
               "{'title': 'Sl.No','data': 'SlNo' ,'sortable': false }," +
                 " { 'title': 'Emp Code','data': 'column1' ,'sortable': false}," +
                  " { 'title': 'Emp Name','data': 'column2' ,'sortable': false}," +
                " { 'title': 'Designation','data': 'column3' ,'sortable': false}," +
                " { 'title': 'Option','data': 'column4' ,'sortable': false}," +
                //" { 'title': 'Amount ','data': 'column6','sortable': false }," +
                "]";
            ViewBag.ReportColumnsCount = 5;
            ViewBag.ReportFooterColumnsCount = 5;
            return View();
        }
        public async Task<string> SearchEmployee(string EmpCode)
        {
            return await tds.SearchEmployee(EmpCode);
        }
        public async Task<string> getTaxOptionData(string taxoption)
        {
            //var doc = await tds.getTdsDetails(emp_code , final);
            var doc = await tds.getTDSTaxOption(taxoption);
            return JsonConvert.SerializeObject(doc);
        }
        public async Task<string> getTaxOptionDataSearch(string emp_code)
        {
            //var doc = await tds.getTdsDetails(emp_code , final);
            var doc = await tds.getTDSTaxOptionSearch(emp_code);
            return JsonConvert.SerializeObject(doc);
        }
        public async Task<string> UpdateEmpTaxOption(string empcodetaxoptions)
        {
            var message = await tds.UpdateTDSTaxOption(empcodetaxoptions);
            return JsonConvert.SerializeObject(message);
        }
        //public JsonResult getEmpselectedTaxOption(string empid)
        //{
        //    string str_qrygetempoption = tds.getEmpselectedTaxOption(empid);
        //    return Json(new { lMessage = str_qrygetempoption }, JsonRequestBehavior.AllowGet);
        //}
        #region TDS Process
        public async Task<ActionResult> TDSProcess()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            var result = await commBus.RunorRevertbuttonenabledisable();
            ViewBag.RunorRevertbuttonenabledisable = result;
            return View();
        }
        #endregion
        public async Task<string> getTdsData(string emp_code, bool final)
        {
            //var doc = await tds.getTdsDetails(emp_code , final);
            var doc = await tds.getTdsDetailsNew(emp_code, final);
            return JsonConvert.SerializeObject(doc);
        }
        //[HttpPost]
        //public async Task<string> processTDS(TDS values)
        //{
        //    var message = await tds.ProcessTDS(values);

        //    return message;
        //}
        [HttpPost]
        public async Task<string> processTDSForAll(TDSForAll values)
        {
            var message = await tds.tdsProcessForAllNew(values);
            return message;
        }
        

        #endregion

        #region TDS Update
        public ActionResult TDSUpdate()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        #endregion



        //pfconcardPorcess
        public async Task<string> pfconcardPorcess()
        {
            var pfconcoard = await pfp.pfconcardPorcess();
            return JsonConvert.SerializeObject(pfconcoard);
        }
      


        public ActionResult GratuityProcess()
        {
            ViewBag.SectionName = "Process";
            return View();
        }
        public async Task<string> getTDSDatatoUpdate(string emp_code)
        {
            var doc = await tdu.getDataToUpdate(emp_code);
            return JsonConvert.SerializeObject(doc);
        }
        public async Task<string> UpdateTDS(TDSUpdate data)
        {
            return await tdu.updateTDs(data);
            
        }
# region  PF interestcalculation
        public ActionResult PFInterestMonthwise()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            return View();
        }
        //newly added by chaitanya on 24/04/2020
        public async Task<string> PFInterestYearRate()
        {
            return await commBus.PFintyr();
        }
        //end
        public async Task<string> GetEmpOBFieldsData()
        {
            return  await pfp.GetEmpOBFieldsDetails();

        }
        [HttpPost]
        public async Task<string> saveFormPFInterestDetails(CommonPostDTO Values)
        {

            if (Values != null)
            {

                return await pfp.SavepfIntData(Values);
            }
            else
            {
                return "E#Error#Please Enter/Modify Data";
            }
           

        }
        #endregion


        #region calculate pf for retirement 
        public async Task<ActionResult> CalculatePFforRetirement()
        {
            ViewBag.SectionName = "Process";
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            var getinstrate = await RetDate.getinstrate();
            foreach (DataRow drPayroll1 in getinstrate.Rows)
            {
                ViewBag.interest = drPayroll1["interest"];
                ViewBag.fy = drPayroll1["fy"];
            }
            return View();
        }
        //retired emp search
        public async Task<string> SearchEmployeeforRetmt(string EmpCode)
        {
            return await RetDate.SearchEmployee(EmpCode);
        }
        public async Task<string> Retirement(string empid)
        {           
            var Retirement_date = await RetDate.getRetirementDate(empid);            
            return JsonConvert.SerializeObject(Retirement_date);
        }
        [HttpPost]
        public async Task<string> insertRetirementDate(retermaintemp values)
        {
            string RelivingDate = values.RelivingDate;
            string pfcaldate = values.pfcaldate;
            string empid = values.EmpId;
            decimal interest =values.inst_rates;
            return await RetDate.insertRetirementDate(empid, RelivingDate, interest , pfcaldate);
            ///return insertret;
        }
        
        #endregion
    }
}