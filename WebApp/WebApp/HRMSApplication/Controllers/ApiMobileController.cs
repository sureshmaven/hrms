using HRMSApplication.Helpers;
using HRMSApplication.Models;
using HRMSBusiness.Business;
using HRMSBusiness.Reports;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using HRMSApplication.AuthHelpers;

namespace HRMSApplication.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/mobile")]
    public class ApiMobileController : ApiController
    {
        private ContextBase db = new ContextBase();
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ApiMobileController));

        [HttpGet]
        [Route("hello")]
        public IHttpActionResult GetHello()
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, "{hello....No Authorization}"));
        }

        [Authorize]
        [HttpGet]
        [Route("userinfotest")]
        public IHttpActionResult GetUserInfo()
        {
            UserInfo uinfo = AuthHelper.LoggedinUserInfo(User);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, uinfo));
        }

        [Authorize]
        [HttpGet]
        [Route("getuserinfo")]
        public IHttpActionResult GetUserInfoDetails()
        {
            UserInfo uinfo = AuthHelper.LoggedinUserInfo(User);
            LogInformation.Info("Mobile api getuserinfo Code started ");
            MobileDashboardDTO dto = new MobileDashboardDTO();
            //get employee data
            LoginBus lbus = new LoginBus();
            LoginResult userinfo = lbus.getLoginInformationmobileapi(uinfo.EmpId);
            if (userinfo.Success)
            {
                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings["websiteUrl"];
                dto.LeaveId = Convert.ToInt32(userinfo.EmpPkId);
                dto.EmpId = Convert.ToInt32(userinfo.EmpId);
                dto.FullName = userinfo.EmployeeFullName;
                dto.Designation = userinfo.desigcode;
                dto.EmployeeImage = websiteUrl + "/uploads/" + userinfo.EmployeeImage;
                dto.ControllingAuthority = userinfo.ControllingAuthority;
                dto.SanctioningAuthority = userinfo.SanctioningAuthority;
                dto.BrDept = userinfo.BranchName;
                ////get employee leave balance
                //IList<EmpLeavebalanceDTO> listbal = new List<EmpLeavebalanceDTO>();

                //DataTable dtbal = lbus.GetLeaveBalance(userinfo.EmpPkId);

                //listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Casual Leave", LeaveBalance = dtbal.Rows[0]["CasualLeave"].ToString() });
                //listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Medical/Sick Leave", LeaveBalance = dtbal.Rows[0]["MedicalSickLeave"].ToString() });
                //listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Privilege Leave", LeaveBalance = dtbal.Rows[0]["PrivilegeLeave"].ToString() });
                //listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Maternity Leave", LeaveBalance = dtbal.Rows[0]["MaternityLeave"].ToString() });
                //listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Paternity Leave", LeaveBalance = dtbal.Rows[0]["PaternityLeave"].ToString() });
                //listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Extraordinary Leave", LeaveBalance = dtbal.Rows[0]["ExtraordinaryLeave"].ToString() });
                //listbal.Add(new EmpLeavebalanceDTO { LeaveType = "SpecialCasual Leave", LeaveBalance = dtbal.Rows[0]["SpecialCasualLeave"].ToString() });
                //if (userinfo.desigcode == "Attender" || userinfo.desigcode == "Driver" || userinfo.desigcode == "SA" || userinfo.desigcode == "Attender-Watchman" || userinfo.desigcode == "Attender/J.C" || userinfo.desigcode == "Watchman" || userinfo.desigcode == "JR-SA" || userinfo.desigcode == "SA-Assistant Cashier")
                //{
                //    listbal.Add(new EmpLeavebalanceDTO { LeaveType = "CompensatoryOff Leave", LeaveBalance = dtbal.Rows[0]["CompensatoryOff"].ToString() });
                //}
                //listbal.Add(new EmpLeavebalanceDTO { LeaveType = NewLOP("LOP"), LeaveBalance = dtbal.Rows[0]["LOP"].ToString() });



                //dto.LeavesBalance = listbal;
                ////top five leavehistory
                //IList<EmpLeaveHistoryDTO> lhist = new List<EmpLeaveHistoryDTO>();
                //LeavesBusiness leavebus = new LeavesBusiness();
                //string empids = Convert.ToString(userinfo.EmpPkId);

                //int lempid = Convert.ToInt32(empids);
                //ReportBusiness rbus = new ReportBusiness();
                //DataTable dthis = (rbus.LeaveApplyControllerapi(lempid));
                //foreach (DataRow drow in dthis.Rows)
                //{
                //    lhist.Add(new EmpLeaveHistoryDTO
                //    {
                //        AppliedDate = Convert.ToDateTime(drow["UpdatedDate"]).Date.ToString("dd/MM/yy"),
                //        StartDate = Convert.ToDateTime(drow["StartDate"]).Date.ToString("dd/MM/yy"),

                //        EndDate = Convert.ToDateTime(drow["EndDate"]).Date.ToString("dd/MM/yy"),
                //        LeaveType = drow["LeaveType"].ToString(),
                //        LeaveDays = drow["LeaveDays"].ToString(),
                //        Status = NewLeaveStatus(drow["Status"].ToString())
                //    });

                //}
                //dto.LeavesHistory = lhist;
                LogInformation.Info("Mobile api GetUserLoginLeaveDetails Code Ended ");

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, dto));
            }
            else
            {
                LogInformation.Info("Invalid UserName and Password");
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid UserName and Password"));
            }
        }
        [Authorize]
        [HttpGet]
        [Route("userleavebalance")]
        public IHttpActionResult GetUserLeaveBalance()
        {
            LogInformation.Info("Mobile api Empbalance Code started ");
            UserInfo uinfo = AuthHelper.LoggedinUserInfo(User);

            IList<EmpLeavebalanceDTO> listbal = new List<EmpLeavebalanceDTO>();
            LoginBus lbus = new LoginBus();
            DataTable dtbal = lbus.GetLeaveBalance(uinfo.PkId);
            MobileDashboardDTO dto = new MobileDashboardDTO();

            listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Casual Leave", LeaveBalance = dtbal.Rows[0]["CasualLeave"].ToString() });
            listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Special Casual Leave", LeaveBalance = dtbal.Rows[0]["SpecialCasualLeave"].ToString() });
            listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Privilege Leave", LeaveBalance = dtbal.Rows[0]["PrivilegeLeave"].ToString() });
            listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Medical/Sick Leave", LeaveBalance = dtbal.Rows[0]["MedicalSickLeave"].ToString() });

            listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Maternity Leave", LeaveBalance = dtbal.Rows[0]["MaternityLeave"].ToString() });
            listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Paternity Leave", LeaveBalance = dtbal.Rows[0]["PaternityLeave"].ToString() });
            listbal.Add(new EmpLeavebalanceDTO { LeaveType = "Extraordinary Leave", LeaveBalance = dtbal.Rows[0]["ExtraordinaryLeave"].ToString() });

            //if (uinfo.desigcode == "Attender" || uinfo.desigcode == "Driver" || uinfo.desigcode == "SA" || uinfo.desigcode == "Attender-Watchman" || uinfo.desigcode == "Attender/J.C" || uinfo.desigcode == "Watchman" || uinfo.desigcode == "JR-SA" || uinfo.desigcode == "SA-Assistant Cashier")
            //{
            listbal.Add(new EmpLeavebalanceDTO { LeaveType = "C-OFF", LeaveBalance = dtbal.Rows[0]["CompensatoryOff"].ToString() });
            //}
            listbal.Add(new EmpLeavebalanceDTO { LeaveType = "LOP(Consumed)", LeaveBalance = dtbal.Rows[0]["LOP"].ToString() });



            dto.LeavesBalance = listbal;
            //top five leavehistory
            IList<EmpLeaveHistoryDTO> lhist = new List<EmpLeaveHistoryDTO>();
            LeavesBusiness leavebus = new LeavesBusiness();
            string empids = Convert.ToString(uinfo.PkId);

            int lempid = Convert.ToInt32(empids);
            ReportBusiness rbus = new ReportBusiness();
            DataTable dthis = (rbus.LeaveApplyControllerapi(lempid));
            foreach (DataRow drow in dthis.Rows)
            {
                lhist.Add(new EmpLeaveHistoryDTO
                {
                    AppliedDate = Convert.ToDateTime(drow["AppliedDate"]).Date.ToString("dd/MM/yy"),
                    StartDate = Convert.ToDateTime(drow["StartDate"]).Date.ToString("dd/MM/yy"),

                    EndDate = Convert.ToDateTime(drow["EndDate"]).Date.ToString("dd/MM/yy"),
                    LeaveType = drow["LeaveType"].ToString(),
                    LeaveDays = drow["LeaveDays"].ToString(),
                    Status = NewLeaveStatus(drow["Status"].ToString())
                });

            }
            dto.LeavesHistory = lhist;
            LogInformation.Info("Mobile api Empbalance Code Ended ");
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, dto));
        }
        [Authorize]
        [HttpPost]
        [Route("userleaveapply")]
        public IHttpActionResult UserLeaveApply(MobileApplyLeaveDTO leavedata)
        {
            LogInformation.Info("Mobile api Code started ");
            try
            {
                LeavesBusiness Lbus = new LeavesBusiness();
                UserInfo uinfo = AuthHelper.LoggedinUserInfo(User);
                LoginBus lbus = new LoginBus();
                LoginResult userinfo = lbus.getLoginInformationmobileapi(uinfo.EmpId);
                string empid = userinfo.EmpId;
                int userinfos = Convert.ToInt32(empid);
                string val = LeaveHelper.ValidateLeaveRequest(leavedata, userinfos);
                string valholiday = LeaveHelper.ValidateHolidayRequest(leavedata, userinfos);

                if (val != "")
                {
                    LogInformation.Info("ApiMobileController, Error. Info: " + val);
                    string msg = "{'status':'Error' ,'message':'Please Check the date range already applied in " + val + "'}";
                    LogInformation.Info("Mobile api Code ended ");
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, JObject.Parse(msg)));


                }
                //else if(dojml!="")
                // {
                //     LogInformation.Info("ApiMobileController, Error. Info: " + dojml);
                //     string msg = "{'status':'Error' ,'message':'Please Check the date range" + dojml + "'}";

                //     LogInformation.Info("Mobile api Code ended ");
                //     return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, JObject.Parse(msg)));
                // }
                else if (valholiday != "")
                {
                    LogInformation.Info("ApiMobileController, Error. Info: " + valholiday);
                    string msg = "{'status':'Error' ,'message':'Leave Cannot Start or End  on " + valholiday + "'}";
                    LogInformation.Info("Mobile api Code ended ");
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, JObject.Parse(msg)));
                }
                else
                {

                    string result = LeaveHelper.CreateLeave(leavedata);
                    string msg = "{'status':'Success' ,'message':'" + result + "'}";
                    LogInformation.Info("ApiMobileController, Success. Info: " + result);
                    LogInformation.Info("Mobile api Code ended ");
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, JObject.Parse(msg)));

                }

            }

            catch (Exception ex)
            {

                string msg = "{'status':'Error' ,'message':'Critical Exception:" + ex.Message + "'}";
                //return Request.CreateResponse(HttpStatusCode.OK, JObject.Parse(msg));
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, JObject.Parse(msg)));
            }

        }

        private string NewLeaveStatus(string st)
        {
            string ret = st;
            if (st == "PartialCancelled")
            {
                ret = "Partial Cancelled";
            }

            return ret;
        }
        private string NewLOP(string st)
        {
            string ret = st;
            if (st == "LOP")
            {
                ret = "Loss Of Pay";
            }

            return ret;
        }

    }
}
