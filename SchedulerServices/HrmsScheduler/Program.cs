using BusLogic;
using PayRollBusiness.PayrollService;
using PayrollModels;
using System;
using System.Data;
using System.Text;

namespace HrmsScheduler
{
    class Program
    {

         private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(Program));

        public static DateTime? Datevalue = null;

        static void Main(string[] args)
        {
            string mavenconn = System.Configuration.ConfigurationManager.AppSettings.Get("mavendb");
            string timesheetconn = System.Configuration.ConfigurationManager.AppSettings.Get("timesheetdb");
            DateTime startTime;
            DateTime endTime;
            TimeSpan duration;
            //  Payroll trns6 = new Payroll(mavenconn);
            //             trns6.ServiceStarting("Payroll");

            //             trns6.Payrolldata();
            //trns6.ServiceStoping("Payroll");
            //get bio metric logs from 3rd party database
            #region hrms, biometric
            if (args[0] == "getbiometriclogs")
            {
               
                
           
                string dt = "";
                string _LastInsertedDate = "";

                if (args.Length == 2)
                {
                    dt = args[1];
                }

                if (args.Length == 3)
                {
                    _LastInsertedDate = args[2];
                }

               DateTime Datevalue = DateTime.Today;
                //DateTime Datevalue =  new DateTime(2024, 11, 16);

                
                TimeSheet trns8 = new TimeSheet(mavenconn, timesheetconn, dt);
                startTime = DateTime.Now;
                trns8.ServiceStarting("timesheet");
                endTime = DateTime.Now;
                Console.WriteLine($"1-56: {endTime - startTime}");

                try
                {
                
                 
                    startTime = DateTime.Now;
                    trns8.GetBioMetricLogs(_LastInsertedDate);
                                      
                    endTime = DateTime.Now;
                    Console.WriteLine($"2-67: {endTime - startTime}");
                }
                catch (Exception e1)
                {
                    Console.Error.WriteLine((e1.Message));
                    Console.WriteLine(e1.Message);
                    Console.WriteLine(e1.StackTrace + "StackTrace");
                    Console.WriteLine(e1.InnerException + "Innerexception");
                    _logger.Error("getbiometric1"+ e1.Message);
                }
                if (!string.IsNullOrWhiteSpace(_LastInsertedDate))
                {

                    Datevalue = Convert.ToDateTime(_LastInsertedDate);
                }

                string strhdate = Datevalue.ToString("yyyy-MM-dd");
                string[] hsa1 = strhdate.Split('-');
                string _Year = hsa1[0];
                string _Month = hsa1[1];
                string _Day = hsa1[2];
                try
                {
                    // TimeSheet_ReRun Status --Month First Day insert

                    //if day equals to 1
                    if (_Day == "01")
                    {

                      //  Console.WriteLine("1.3 Get TimeSheet_ReRun Status Service started " + start1Time);
                      //// _logger.Info("1.3  Get TimeSheet_ReRun Status Service started "+ start1Time);

                        startTime = DateTime.Now;
                        trns8.InsertTimesheetReRunStatus(Datevalue);
                        endTime = DateTime.Now;

                        Console.WriteLine($"3-103: {endTime - startTime}");

                    }
                }
                catch (Exception e1)
                {
                   
                    Console.Error.WriteLine("InsertTimesheetReRunStatus");
                    Console.WriteLine(e1.Message);
                    Console.WriteLine(e1.StackTrace + "StackTrace");
                   _logger.Error("InsertTimesheetReRunStatus " + e1.Message);
                }
                try
                {
                     
                    DateTime Datevalue1 = DateTime.Today;
                   // DateTime Datevalue1 = new DateTime(2024, 11, 16);
                    if (!string.IsNullOrWhiteSpace(_LastInsertedDate))
                    {
                        Datevalue1 = Convert.ToDateTime(_LastInsertedDate);
                    }

                    //Time Sheet ReRun Daywise
                    startTime = DateTime.Now;
                    trns8.UpdateTimeSheetDayWiseBranchStatus(Datevalue1);
                    endTime = DateTime.Now;
                    Console.WriteLine($"4-128: {endTime - startTime}");
                }
                catch (Exception e1)
                {
                    _logger.Error("1.7 UpdateTimeSheetDayWiseBranchStatus "+e1.Message);
                    Console.Error.WriteLine("getbiometriclogs");
                    Console.WriteLine(e1.Message);
                    Console.WriteLine(e1.StackTrace + "StackTrace");
                    Console.WriteLine(e1.InnerException + "Innerexception");
                }
                startTime = DateTime.Now;
                trns8.ServiceStoping("timesheet");
                endTime = DateTime.Now;                
                Console.WriteLine($"5-142: {endTime - startTime}");


                while (true) { }

            }
            // generate timesheet logs and generate timesheet reports
            else if (args[0] == "gentimesheetreports")
            {
                
                
                //// _logger.Info("2.0 gentimesheetreports service start at "+ tstartTime);
                //Console.Write("2.0 second service started " + tstartTime);
                string dt = "";
                if (args.Length == 2)
                {
                    dt = args[1];
                }

                TimeSheet trns8 = new TimeSheet(mavenconn, null, dt);
                startTime = DateTime.Now;
                trns8.ServiceStarting("timesheet");
                endTime = DateTime.Now;
                Console.WriteLine($"50-166: {endTime - startTime}");
                //DateTime Datevalue = new DateTime(2024, 12, 13);
                try
                {
                    //// _logger.Info("2.1 Inserting All Employees List into Timesheet Employee Month started " + tstartTime);
                    //   Console.WriteLine("2.1 Inserting All Employees List into Timesheet Employee Month started " + tstartTime);
                    startTime = DateTime.Now;
                    trns8.InsertEmployeesIntoEmpMonthTable();
                    //tendTime = DateTime.Now;
                    //duration = tendTime - tstartTime;
                    //// _logger.Info("2.2 Inserting All Employees List into Timesheet Employee Month Ended " + tendTime+ " duration "+duration);
                    //Console.WriteLine("2.2 Inserting All Employees List into Timesheet Employee Month Ended " + tendTime + " duration " + duration);
                    endTime = DateTime.Now;
                    Console.WriteLine($"51-179: {endTime - startTime}");
                }
                catch (Exception e1)
                {
                   _logger.Error("InsertEmployeesIntoEmpMonthTable " + e1.Message);
                    Console.Error.WriteLine("InsertEmployeesIntoEmpMonthTable");
                    Console.WriteLine(e1.Message);
                    Console.WriteLine(e1.StackTrace + "StackTrace");
                    Console.WriteLine(e1.InnerException+"Innerexception");
                }

                try
                {
                    //    DateTime tstartcTime = DateTime.Now;
                    //    DateTime tendcTime;

                    //    Console.WriteLine("2.3 Checking Holiday Started " + tstartcTime);
                    //  // _logger.Info("2.3 Checking Holiday Started " + tstartcTime);
                    startTime = DateTime.Now;
                    trns8.CheckHolidayList();
                    
                    //  Console.WriteLine("2.4 Checking Holiday Ended " + tendcTime + " duration " + duration);
                    //// _logger.Info("2.4 Checking Holiday Ended " + tendcTime+ " duration "+duration);
                    endTime = DateTime.Now;
                    Console.WriteLine($"52-204: {endTime - startTime}");
                }
                catch (Exception e1)
                {
                    _logger.Error("CheckHolidayList "+e1.Message);
                    Console.Error.WriteLine(e1.Message);
                    Console.WriteLine("CheckHolidayList");
                    Console.WriteLine(e1.StackTrace + "StackTrace");
                    Console.WriteLine(e1.InnerException+ "Innerexception");
                }
                try
                {

                    startTime = DateTime.Now;
                    trns8.GenerateTimesheetReportsData(true, Datevalue, null, null, null);
                  endTime = DateTime.Now;
                    Console.WriteLine($"53-226: {endTime - startTime}");
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                    Console.WriteLine(e1.StackTrace + "StackTrace");
                    Console.WriteLine(e1.InnerException + "Innerexception");
                   _logger.Error(e1.Message);
                }

                try
                {
                    // Console.WriteLine("2.7 UpdateRequestFormUsers Started " + tstarturfTime);
                    //// _logger.Info("2.7 UpdateRequestFormUsers Started " + tstarturfTime);
                    startTime = DateTime.Now;
                    trns8.UpdateRequestFormUsers();
                    endTime = DateTime.Now;
                    Console.WriteLine($"54-241: {endTime - startTime}");
                }
                catch (Exception e1)
                {
                    //_logger.Error(e1.Message);
                    Console.WriteLine(e1.Message);
                    Console.WriteLine(e1.StackTrace + "StackTrace");
                    Console.WriteLine(e1.InnerException + "Innerexception");
                }
                try
                {
                    startTime = DateTime.Now;
                    trns8.UpdateRequestFormODUsers();
                    endTime = DateTime.Now;
                    Console.WriteLine($"55-255: {endTime - startTime}");
                }
                catch (Exception e1)
                {
                    //_logger.Error(e1.Message);
                    Console.WriteLine(e1.Message);
                    Console.WriteLine(e1.StackTrace + "StackTrace");
                    Console.WriteLine(e1.InnerException + "Innerexception");
                }
                // for last 15 days check for the entries
                try
                {
                    //DateTime Datevalue = new DateTime(2024, 12, 03);
                    startTime = DateTime.Now;
                    trns8.Check_Last_15days(Convert.ToDateTime(Datevalue));
                    endTime = DateTime.Now;
                    Console.WriteLine($"56-273: {endTime - startTime}");
                }
                catch (Exception e1)
                {
                    _logger.Error(e1.Message);
                    Console.WriteLine("End - Last 15 days check for the entries");
                    Console.WriteLine(e1.StackTrace);
                    Console.WriteLine(e1.InnerException);
                }
              
                try
                {
                    startTime = DateTime.Now;
                    trns8.RerunForGivenBranches();
                    endTime = DateTime.Now;
                    Console.WriteLine($"57-285: {endTime - startTime}");
                }
                catch (Exception e1)
                {
                  _logger.Error(e1.Message);
                    Console.WriteLine(e1.Message);
                    Console.WriteLine(e1.StackTrace + "StackTrace");
                    Console.WriteLine(e1.InnerException + "Innerexception");
                }
              try
                {
                    startTime = DateTime.Now;
                    trns8.LeaveAppliedButAttendedDuty();
                    endTime = DateTime.Now;
                    Console.WriteLine($"58-299: {endTime - startTime}");
                }
                catch (Exception e1)
                {
                  _logger.Error(e1.Message);
                    Console.WriteLine(e1.Message);
                    Console.WriteLine(e1.StackTrace + "StackTrace");
                    Console.WriteLine(e1.InnerException + "Innerexception");
                }
                startTime = DateTime.Now;
                trns8.ServiceStoping("timesheet");
                endTime = DateTime.Now;
                Console.WriteLine($"59-311: {endTime - startTime}");

                while (true) { }

            }
            else if (args[0] == "emptotalexp")
            {
                EmpTotalExp trns3 = new EmpTotalExp(mavenconn);
                trns3.ServiceStarting("emptotalexp");
                try
                {
                    Console.WriteLine("2.17 Employee Total Experience Service started");
                   //// _logger.Info("Employee Total Experience Service started");
                    trns3.Totalexperiences();
                    Console.WriteLine("2.18 Employee Total Experience Service Ended");
                   //// _logger.Info("Employee Total Experience Service Ended");
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                    //_logger.Error(e1.Message);
                }

                trns3.ServiceStoping("emptotalexp");
            }
            else if (args[0] == "revert")
            {
                RevertTransfers trsns1 = new RevertTransfers(mavenconn);
                trsns1.ServiceStarting("revert");
                try
                {
                    Console.WriteLine("2.19 Revert Temporary Transfer Service Started");
                   //// _logger.Info("Revert Temporary Transfer Service Started");
                    trsns1.RevertTempTransfers();
                    Console.WriteLine("2.20 Revert Temporary Transfer Service Ended");
                   //// _logger.Info("Revert Temporary Transfer Service Ended");
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                    //_logger.Error(e1.Message);
                }
                trsns1.ServiceStoping("revert");
            }
            else if (args[0] == "today")
            {
                TodayFutureTransfers trns2 = new TodayFutureTransfers(mavenconn);
                trns2.ServiceStarting("today");
                try
                {
                    Console.WriteLine("Today Temporary Transfer Service Started");
                   //// _logger.Info("Today Temporary Transfer Service Started");
                    trns2.TodayTempTransfers();
                    Console.WriteLine("Today Temporary Transfer Service Ended");
                   //// _logger.Info("Today Temporary Transfer Service Ended");
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                    //_logger.Error(e1.Message);
                }
                trns2.ServiceStoping("today");
            }
            else if (args[0] == "permanent")
            {
                PermanentTransfer trns4 = new PermanentTransfer(mavenconn);
                trns4.ServiceStarting("permanent");
                try
                {
                    Console.WriteLine("Permanent Transfer Service Started");
                   //// _logger.Info("Permanent Transfer Service Started");
                    trns4.PermanentTransfers();
                    Console.WriteLine("Permanent Transfer Service Ended");
                   //// _logger.Info("Permanent Transfer Service Ended");
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                    //_logger.Error(e1.Message);
                }
                trns4.ServiceStoping("permanent");
            }
            else if (args[0] == "promotion")
            {
                Promotion trns5 = new Promotion(mavenconn);
                trns5.ServiceStarting("promotion");
                try
                {
                    Console.WriteLine("Promotions Service Started");
                   //// _logger.Info("Promotions Service Started");
                    trns5.Promotions();
                    Console.WriteLine("Promotions Service Ended");
                   //// _logger.Info("Promotions Service Ended");
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                    //_logger.Error(e1.Message);
                }
                trns5.ServiceStoping("promotion");
            }
            else if (args[0] == "promotionTransfer")
            {
                PromotionTransfer trns6 = new PromotionTransfer(mavenconn);
                trns6.ServiceStarting("promotionTransfer");
                try
                {
                    Console.WriteLine("Promotions Transfer Service Started");
                   //// _logger.Info("Promotions Transfer Service Started");
                    trns6.PromotionTransfers();
                    Console.WriteLine("Promotions Transfer Service Endee");
                   //// _logger.Info("Promotions Transfer Service Endee");
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                    //_logger.Error(e1.Message);
                }

                trns6.ServiceStoping("promotionTransfer");
            }
            else if (args[0] == "autocreditcl")
            {
                AutoCredit trns7 = new AutoCredit(mavenconn);
                trns7.ServiceStarting("autocreditcl");
                try
                {
                    Console.WriteLine("AutoCredit Leaves Service Started");
                   //// _logger.Info("AutoCredit Leaves Service Started");
                    trns7.AutoCredits();
                    Console.WriteLine("AutoCredit Leaves Service Ended");
                   //// _logger.Info("AutoCredit Leaves Service Ended");
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                   //_logger.Error(e1.Message);

                }
                trns7.ServiceStoping("autocreditcl");
            }
            #endregion

            #region Payroll Services
            if (args[0] == "pr_allowence_calc")
            {
                EmpAllowences empall = new EmpAllowences(getLoginCredential());
                empall.ServiceStarting("Pr-Emp-Allowences").GetAwaiter().GetResult();
                try
                {
                    empall.EmpAllowenceCalculation().GetAwaiter().GetResult();
                }
                catch (Exception e1)
                {
                   //_logger.Error(e1.Message);
                }

                empall.ServiceStoping("Pr-Emp-Allowences").GetAwaiter().GetResult();
            }

            //Employee In Month Loans And Advances
            //if (args[0] == "PrMonthEndProcess")
            //{
            //    MonthEndProcess MEP = new MonthEndProcess(getLoginCredential(), //_logger);
            //    MEP.ServiceStarting("PrMonthEndProcess").GetAwaiter().GetResult();

            //    bool bCopy = false, bJaiib = false, bloans = false, ploans = false, OBshare = false, tds = false, psAll_Deduction = false, Adhoc = false, bAllowance = false;



            //    // 11. JAIIBCAIIBService

            //    DayEndProcess JCSer = new DayEndProcess(getLoginCredential(), //_logger);
            //    PaySlipSer payslip = new PaySlipSer(getLoginCredential(), //_logger);

            //    ////10.Change month
            //    //try
            //    //{
            //    //   //// _logger.Info("Change Month Process Started");
            //    //    MEP.ChangeMonthProcess().GetAwaiter().GetResult();
            //    //   //// _logger.Info("Change Month Process Ended");
            //    //}
            //    //catch (Exception e1)
            //    //{
            //    //    //_logger.Error(e1.Message);
            //    //}


            //    try
            //    {
            //        //Any Jaiib, Caiib, Annual incr >> add to allowences for that day
            //       //// _logger.Info("Update Increment Amount Started");
            //        JCSer.UpdateIncrementAmount().GetAwaiter().GetResult();
            //       //// _logger.Info("Update Increment Amount End");

            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }


            //    try
            //    {
            //        //updating promotion basic pay to employee master basic pay
            //        JCSer.UpdatePromotionPay().GetAwaiter().GetResult();
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }

            //    //try
            //    //{
            //    //      payslip.Gen_PaySlip(0).GetAwaiter().GetResult();
            //    //}
            //    //catch (Exception e1)
            //    //{
            //    //    //_logger.Error(e1.Message);
            //    //}

            //    ////1.Copy Emp Old Allw Deductions
            //    //try
            //    //{
            //    //    //bCopy = true;
            //    //   //// _logger.Info("Copy Employess OldAllwDed Started");
            //    //    bCopy = MEP.CopyEmpOldAllwDed().GetAwaiter().GetResult();
            //    //   //// _logger.Info("Copy Employess OldAllwDed Ended");
            //    //}
            //    //catch (Exception e1)
            //    //{
            //    //    //_logger.Error(e1.Message);
            //    //}

            //    //2.Jaiib , Caiib, Annual Incr added to basic, Promotions-Promotion change in employees table and new basic
            //    //updated in pay_fields table.
            //    try
            //    {
            //        //bJaiib = true;
            //       //// _logger.Info("Proc_JAIIB_CAIIB_AnnulIncr Started");
            //        bJaiib = MEP.Proc_JAIIB_CAIIB_AnnulIncr().GetAwaiter().GetResult();
            //       //// _logger.Info("Proc_JAIIB_CAIIB_AnnulIncr Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }

            //    //3. Increment Month End Process

            //    try
            //    {
            //        //bJaiib = true;
            //       //// _logger.Info("Procedure Annull_increments _month_end Started");
            //        bJaiib = MEP.ProcedureAnnull_increments_month_end().GetAwaiter().GetResult();
            //       //// _logger.Info("Procedure Annull_increments _month_end Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }

            //    //4. 
            //    //Updated Branch Allowances_General,Special,Amount
            //    try
            //    {
            //        //bJaiib = true;
            //       //// _logger.Info("Updated  Allowances_General_Special_Amount Started");
            //        bAllowance = MEP.UpdateBranchAllowances_Genearl_Special().GetAwaiter().GetResult();
            //       //// _logger.Info("Updated  Allowances_General_Special_Amount Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }


            //    //6.installment interest calculation And Partpayment
            //    //try
            //    //{
            //    //   //// _logger.Info("Update Interest And PartPayment Started");
            //    //     ploans =MEP.UpdateInterestAndPartPayment().GetAwaiter().GetResult();
            //    //   //// _logger.Info("Update Interest And PartPayment Ended");
            //    //}
            //    //catch (Exception e1)
            //    //{
            //    //    //_logger.Error(e1.Message);
            //    //}

            //    //Commeneted as per the new requirement Update OB_Share should be done in PrHourProcess
            //    ////7.update OB Share
            //    //try
            //    //{
            //    //   //// _logger.Info("Update OB_Share Started");
            //    //    OBshare = MEP.UpdateOB_Share().GetAwaiter().GetResult();
            //    //   //// _logger.Info("Update OB_Share Ended");
            //    //}
            //    //catch (Exception e1)
            //    //{
            //    //    //_logger.Error(e1.Message);
            //    //}


            //    //8.update TDS_Tax_Deducted
            //    try
            //    {
            //       //// _logger.Info("Update TDS_Tax_Deducted Started");
            //        tds = MEP.UpdateTDS_Tax_Deducted().GetAwaiter().GetResult();
            //       //// _logger.Info("Update TDS_Tax_Deducted Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }



            //    //9.Update Adhoc Deduction_Contribution_Det_Earn
            //    try
            //    {
            //       //// _logger.Info("Update Adhoc _Deductions_Contribution_Det_Earn Started");
            //        Adhoc = MEP.UpdateAdhoc_Deductions_Contribution_Det_Earn().GetAwaiter().GetResult();
            //       //// _logger.Info("Update Adhoc _Deductions_Contribution_Det_Earn  Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }




            //    //1.Copy Emp Old Allw Deductions
            //    try
            //    {
            //        //bCopy = true;
            //       //// _logger.Info("Copy Employess OldAllwDed Started");
            //        bCopy = MEP.CopyEmpOldAllwDed().GetAwaiter().GetResult();
            //       //// _logger.Info("Copy Employess OldAllwDed Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }


            //    //10.Change month
            //    try
            //    {
            //       //// _logger.Info("Change Month Process Started");
            //        MEP.ChangeMonthProcess().GetAwaiter().GetResult();
            //       //// _logger.Info("Change Month Process Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }

            //    //11.change attendance
            //    try
            //    {
            //       //// _logger.Info("Change Attendance Process Started");
            //        MEP.ChangeAttendanceProcess().GetAwaiter().GetResult();
            //       //// _logger.Info("Change Attendance Process Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }

            //    //12.Update Payslip_Deductions_Allowance
            //    try
            //    {
            //       //// _logger.Info("Update Payslip _Deductions_Allowance Started");
            //        psAll_Deduction = MEP.UpdatePayslip_Deductions_Allowance().GetAwaiter().GetResult();
            //       //// _logger.Info("Update Payslip _Deductions_Allowance Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }
            //    //13.Update Personal Earning and Deducation.
            //    try
            //    {
            //       //// _logger.Info("Update Personal _Earning _Deduction Started");
            //        MEP.UpdatePersonal_Earning_Deduction().GetAwaiter().GetResult();
            //       //// _logger.Info("Update Personal _Earning _Deduction Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }

            //    MEP.ServiceStoping("PrMonthEndProcess").GetAwaiter().GetResult();


            //}


            //JAIIBCAIIBService
            //if (args[0] == "PrDayEndProcess")
            //{
            //    DayEndProcess JCSer = new DayEndProcess(getLoginCredential(), //_logger);
            //    PaySlipSer payslip = new PaySlipSer(getLoginCredential(), //_logger);
            //    JCSer.ServiceStarting("PrDayEndProcess").GetAwaiter().GetResult();
            //    // commented 08/09/19 
            //    //try
            //    //{
            //    //    //Any Jaiib, Caiib, Annual incr >> add to allowences for that day
            //    //    JCSer.UpdateIncrementAmount().GetAwaiter().GetResult();
            //    //}
            //    //catch (Exception e1)
            //    //{
            //    //    //_logger.Error(e1.Message);
            //    //}


            //    try
            //    {
            //       //// _logger.Info("Update Promotion Start");
            //        //updating promotion basic pay to employee master basic pay
            //        JCSer.UpdatePromotionPay().GetAwaiter().GetResult();
            //       //// _logger.Info("Update promotion End");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }

            //    //try
            //    //{
            //    //    payslip.Gen_PaySlip(0).GetAwaiter().GetResult();
            //    //}
            //    //catch (Exception e1)
            //    //{
            //    //    //_logger.Error(e1.Message);
            //    //}

            //    JCSer.ServiceStoping("PrDayEndProcess").GetAwaiter().GetResult();
            //}

            // hour end service
            //if (args[0] == "PrHourProcess")
            //{
            //    HourEndProcess HrSer = new HourEndProcess(getLoginCredential(), //_logger);
            //    PaySlipSer payslip = new PaySlipSer(getLoginCredential(), //_logger);
            //    MonthEndProcess MEP = new MonthEndProcess(getLoginCredential(), //_logger); //new added on 25/05/2020
            //    HrSer.ServiceStarting("PrHourProcess").GetAwaiter().GetResult();

            //    bool OBshare = false; //new added on 25/05/2020

            //    try
            //    {
            //       //// _logger.Info("Before HourEnd Process for Loans Started");
            //       // MEP.BeforeHourEndProcessforLoans().GetAwaiter().GetResult();
            //       //// _logger.Info("Before HourEnd Process for Loans Ended");
            //    }
            //    catch (Exception ex)
            //    {
            //        //_logger.Error(ex.Message);
            //    }

            //    try
            //    {
            //        //HrSer.hourprocess().GetAwaiter().GetResult();
            //       //// _logger.Info("Hour End process Start");
            //        DataTable dtwd = HrSer.hourprocess().GetAwaiter().GetResult();


            //        foreach (DataRow drs in dtwd.Rows)
            //        {
            //            try
            //            {
            //               // payslip.Gen_PaySlip(0).GetAwaiter().GetResult();

            //            }
            //            catch (Exception e1)
            //            {
            //                //_logger.Error(e1.Message);
            //            }
            //        }

            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }
            //    //try
            //    //{
            //    //    //MonthEndProcess MEP = new MonthEndProcess(getLoginCredential(), //_logger);
            //    //   //// _logger.Info("Update Monthly Loan Installments Started");
            //    //    MEP.UpdateMonthlyLoanInstallmentsBeforeMonthend().GetAwaiter().GetResult();
            //    //   //// _logger.Info("Update Monthly Loan Installments Ended");

            //    //}
            //    //catch (Exception e1)
            //    //{
            //    //    //_logger.Error(e1.Message);
            //    //}
            //    // try
            //    // {
            //    ////// _logger.Info("Update Monthly Loan Installments Started");
            //    // MEP.UpdateMonthlyLoanInstallmentsReverse().GetAwaiter().GetResult();
            //    ////// _logger.Info("Update Monthly Loan Installments Ended");

            //    // }
            //    // catch (Exception e1)
            //    // {
            //    // //_logger.Error(e1.Message);
            //    // }
            //    //5.Loans Installments
            //    try
            //    {
            //       //// _logger.Info("Update Monthly Loan Installments Started");
            //        MEP.UpdateMonthlyLoanInstallments().GetAwaiter().GetResult();
            //        ////// _logger.Info("Update Monthly Loan Installments Ended");

            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }


            //    // new added on 25/05/2020
            //    try
            //    {
            //        ////// _logger.Info("Update OB_Share Started");
            //       // OBshare = MEP.UpdateOB_Share().GetAwaiter().GetResult();
            //        ////// _logger.Info("Update OB_Share Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }
            //    //end
            //    HrSer.ServiceStoping("PrHourProcess").GetAwaiter().GetResult();
            //    ////// _logger.Info("Hour End Process End");
            //}
            //if (args[0] == "ReRunPrHourProcess")
            //{
            //    HourEndProcess HrSer = new HourEndProcess(getLoginCredential(), //_logger);
            //    PaySlipSer payslip = new PaySlipSer(getLoginCredential(), //_logger);
            //    MonthEndProcess MEP = new MonthEndProcess(getLoginCredential(), //_logger); //new added on 25/05/2020
            //    HrSer.ServiceStarting("ReRunPrHourProcess").GetAwaiter().GetResult();

            //    try
            //    {
            //        ////// _logger.Info("Update Monthly Loan Installments Started");
            //        MEP.UpdateMonthlyLoanInstallmentsReverse().GetAwaiter().GetResult();
            //        ////// _logger.Info("Update Reverse Monthly Loan Installments Ended");

            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }
            //    try
            //    {
            //        ////// _logger.Info("Revert Payslip Service Started");
            //        MEP.RevertPayslipService().GetAwaiter().GetResult();
            //        ////// _logger.Info("Revert Payslip Service Ended");

            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }
            //    try
            //    {
            //        ////// _logger.Info("Revert OB Share Service Started");
            //        MEP.RevertOBShareService().GetAwaiter().GetResult();
            //        ////// _logger.Info("Revert OB Share Service Ended");

            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }

            //    //end
            //    HrSer.ServiceStoping("ReRunPrHourProcess").GetAwaiter().GetResult();
            //    ////// _logger.Info("Reverse Hour End Process End");
            //}
            //Year End Process
            //if (args[0] == "PrYearEndProcess")
            //{
            //    YearEndProcess YrSer = new YearEndProcess(getLoginCredential(), //_logger);
            //    PaySlipSer payslip = new PaySlipSer(getLoginCredential(), //_logger);
            //    YrSer.ServiceStarting("PrYearEndProcess").GetAwaiter().GetResult();

            //    //ob shar_process
            //    try
            //    {
            //        ////// _logger.Info("Update Obshar Process Started");
            //        YrSer.Yesr_End_obshar_process().GetAwaiter().GetResult();
            //        ////// _logger.Info("Update Obshar Process Started");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }
            //    // RENT DETAILS
            //    try
            //    {
            //        //bCopy = true;
            //        ////// _logger.Info("Update Rent Details Started");
            //        YrSer.Year_end_Rent_Details().GetAwaiter().GetResult();
            //        ////// _logger.Info("Update Rent Details Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }
            //    //personal earnings
            //    try
            //    {
            //        //bCopy = true;
            //        ////// _logger.Info("Update personal earnings Started");
            //        YrSer.Year_end_perearning().GetAwaiter().GetResult();
            //        ////// _logger.Info("Update personal earnings Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }
            //    // personal deductions
            //    try
            //    {
            //        //bCopy = true;
            //        ////// _logger.Info("Update personal deductions Started");
            //        YrSer.Year_end_perdeductions().GetAwaiter().GetResult();
            //        // ////// _logger.Info("Update personal deductions Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        // //_logger.Error(e1.Message);
            //    }

            //    //tds deductions
            //    try
            //    {
            //        //bCopy = true;
            //        //  ////// _logger.Info("Update TDS Deductions Started");
            //        YrSer.Year_end_tds_deductions().GetAwaiter().GetResult();
            //        // ////// _logger.Info("Update TDS Deductions Started");
            //    }
            //    catch (Exception e1)
            //    {
            //        // //_logger.Error(e1.Message);
            //    }
            //    //tds process
            //    try
            //    {
            //        //bCopy = true;
            //        // ////// _logger.Info("Update TDS Process Started");
            //        YrSer.Year_end_tds_process().GetAwaiter().GetResult();
            //        // ////// _logger.Info("Update TDS Process Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }
            //    //form 12ba
            //    try
            //    {
            //        //bCopy = true;
            //        // ////// _logger.Info("Update Form 12BA Started");
            //        YrSer.Year_end_incometax_12ba().GetAwaiter().GetResult();
            //        // ////// _logger.Info("update Form 12BA Ended");
            //    }
            //    catch (Exception e1)
            //    {
            //        //_logger.Error(e1.Message);
            //    }

            //    //try
            //    //{
            //    //    //bCopy = true;
            //    //    ////// _logger.Info("Copy Data to Old Tables");
            //    //    YrSer.CopyData().GetAwaiter().GetResult();
            //    //    ////// _logger.Info("Copy Data to Old Tables");
            //    //}
            //    //catch (Exception e1)
            //    //{
            //    //    //_logger.Error(e1.Message);
            //    //}

            //    YrSer.ServiceStoping("PrYearEndProcess").GetAwaiter().GetResult();
            //}
            #endregion

        }

        private static LoginCredential getLoginCredential()
        {
            //string qryfm = " select fm from pr_month_details where active=1";

            return new LoginCredential
            {
                AppName = "PR Service",
                AppVersion = "1.0.0",
                AppEnvironment = "Dev",
                FinancialMonthDate = DateTime.Now,
                FY = 2020,
                EmpCode = 0,
                EmpShortName = "Service"
            };
        }
    
    }
}
