using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRMSApplication.Models;
using HRMSApplication.Helpers;
using HRMSBusiness.Business;
using HRMSBusiness.Reports;
using Repository;
using Newtonsoft.Json;
using HRMSBusiness.Db;
using System.Data;
using Entities;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class ChangeBranchManagerController : Controller
    {
        private ContextBase db = new ContextBase();
        ReportBusiness Rbus = new ReportBusiness();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        //chaitanya
        [HttpGet]
        public ActionResult ChangeBranchManager()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.LoginUserName = lCredentials.EmpFullName;
            ViewBag.LoginBranch = lCredentials.Branch;
            ViewBag.LoginBranchName = lCredentials.BranchName;

            ViewBag.SearchBtn = "false";

            ViewBag.ReportFilters = "branchmanageddlOne";

            var dt = new SqlHelper().Get_Table_FromQry("select [Id],[Name] from Branches where Name in (select BranchName from v_BranchContactList)");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["Name"] ?? "null")
            }).ToList();

            //items.Insert(0, new Branches
            //{
            //    Id = 0,
            //    Name = "All"
            //});

            ViewBag.DdlOneData = new SelectList(items, "Id", "Name");

            ViewBag.ReportTitle = "Change Branch Manager";
            //ViewBag.DataUrl = "/AllReports/BranchManagerData";
            //ViewBag.ReportColumns = @"[{""title"": ""Emp Id"",""data"": ""EmpId"",  ""autoWidth"": true },{""title"": ""Emp Name"", ""data"": ""EmpName"", ""autoWidth"": true },{""title"": ""Branch"",""data"": ""BranchName"", ""autoWidth"": true },{""title"": ""Designation"",""data"": ""Code"",  ""autoWidth"": true },{""title"": ""Extensions"",""data"": ""PhoneNo1"", ""autoWidth"": true },{""title"": ""Mobile Number"",""data"": ""PhoneNo2"", ""autoWidth"": true },{""title"": ""Time"",""data"": ""Time"",  ""autoWidth"": true }]";
            //return View("~/Views/AllReports/AllReports.cshtml");
            return View();
        }


        //[HttpGet]
        //public string BranchManagerData()
        //{
        //    var dt = Rbus.getBranchContactsList();
        //    return JsonConvert.SerializeObject(dt);
        //}

        [HttpGet]
        public JsonResult GetBranchManagerData(string branch)
        {
            int empid; string empnam, brname, desig,errornobranch;
            string str_qry = "select [Name] from Branches where Id="+ branch + "";
            DataTable dt_brId= new SqlHelper().Get_Table_FromQry(str_qry);
            string str_qry1= "Select EmpId,EmpName,BranchName,name as Designation from v_BranchContactList where BranchName='" + dt_brId.Rows[0]["Name"].ToString() + "'";
            DataTable dt_brName = new SqlHelper().Get_Table_FromQry(str_qry1);
            if (dt_brName.Rows.Count == 0)
            {
                errornobranch = "No Record Found";
                return Json (new { lerrnobranch = errornobranch },JsonRequestBehavior.AllowGet);
            }

            empid = Convert.ToInt32(dt_brName.Rows[0]["EmpId"]);
            empnam = dt_brName.Rows[0]["EmpName"].ToString();
            //brname = dt.Rows[0]["BranchName"].ToString();
            desig = dt_brName.Rows[0]["Designation"].ToString();

            //var dt = Rbus.getBranchContactsList();
            //var empid= 
            return Json(new { lempidbranchAjax = empid, lbrempnameAjax = empnam, ldesignationAjax = desig }, JsonRequestBehavior.AllowGet);  // lbranchAjax = brname,
        }
        public JsonResult SearchNewEmpiddetails(string searchempcode)
        {
            int empid; string empname, brname, desig, strerrornoemp;
            string errornoempid, errorretiredemp;
            DataTable dt_getempid_new, dt_getretiredate_new, dt_getbranchid_new, dt_getnewbranchmanager;
            dt_getempid_new = new SqlHelper().Get_Table_FromQry("Select [EmpId] from Employees where EmpId=" + searchempcode + "");
            if (dt_getempid_new.Rows.Count == 0)
            {
                errornoempid = "Employee doesn't exists..";
                return Json(new { lerrnoempid = errornoempid }, JsonRequestBehavior.AllowGet);
            }
            dt_getretiredate_new = new SqlHelper().Get_Table_FromQry("Select [EmpId],[RetirementDate] from Employees where RetirementDate>=Getdate() and EmpId=" + searchempcode + "");
            if (dt_getretiredate_new.Rows.Count == 0)
            {
                errorretiredemp = "Employee already retired..";
                return Json(new { lerrretemp = errorretiredemp }, JsonRequestBehavior.AllowGet);
            }
            //string str_empsearch = "Select EmpId,EmpName,BranchName,name as Designation from v_BranchContactList where EmpId="+ searchempcode + "";
            string str_empsearch = "Select e.EmpId,e.ShortName as EmpName, case when b.Name = 'OtherBranch' then dep.name  else b.Name end as BranchName, d.name as Designation from Employees e join Branches b " +
                " on e.Branch=b.Id join Designations d on e.CurrentDesignation = d.id join Departments dep on e.Department = dep.Id where e.EmpId =" + searchempcode + "";
            DataTable dt_searchempid= new SqlHelper().Get_Table_FromQry(str_empsearch);
            //if(dt_searchempid.Rows.Count==0)
            //{
            //    strerrornoemp = "EmpId "+ searchempcode + " is not Allocated to any Branch";
            //    return Json(new {lerrornoemp= strerrornoemp }, JsonRequestBehavior.AllowGet);
            //}
            empid = Convert.ToInt32(dt_searchempid.Rows[0]["EmpId"]);
            empname = dt_searchempid.Rows[0]["EmpName"].ToString();
            brname = dt_searchempid.Rows[0]["BranchName"].ToString();
            desig = dt_searchempid.Rows[0]["Designation"].ToString();

            return Json(new { lempidsearchAjax = empid, lbrempnamesearchAjax = empname, ldesignationsearchAjax = desig, lbranchsearchAjax = brname }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult UpdatenewbranchManager(string empcode, string branchname,int branchid)
        //{
        //    int emp_code,branch_id;
        //    string strqueryupdatebranch = "", branch_name;
        //    bool newbrid_update=false;
        //    DateTime ret_date;
        //    DataTable dt_getempid, dt_getretiredate,dt_getbranchid;
        //    dt_getempid = new SqlHelper().Get_Table_FromQry("Select [EmpId] from Employees where EmpId=" + empcode + "");
        //    if (dt_getempid.Rows.Count > 0)
        //    {
        //        emp_code = Convert.ToInt32(dt_getempid.Rows[0]["EmpId"]);
        //    }
        //    else
        //    {
        //        TempData["empMess"] = "Employee Does not Exists";
        //    }
        //    dt_getretiredate = new SqlHelper().Get_Table_FromQry("Select [EmpId],[RetirementDate] from Employees where RetirementDate>=Getdate() and EmpId=" + empcode + "");
        //    if (dt_getretiredate.Rows.Count > 0)
        //    {
        //        ret_date = Convert.ToDateTime(dt_getretiredate.Rows[0]["RetirementDate"]);
        //    }
        //    else
        //    {
        //{
        //        TempData["empRetMess"] = "Employee Already Retired.";
        //    }
        //    dt_getbranchid = new SqlHelper().Get_Table_FromQry("Select [Id],[Name],[BM_Id] from Branches where [Id]="+ branchid);
        //    if (dt_getbranchid.Rows.Count>0)
        //    {
        //        strqueryupdatebranch = "Update Branches set BM_ID="+ empcode + " where Id="+ branchid;
        //        newbrid_update = new SqlHelper().Run_UPDDEL_ExecuteNonQuery("");
        //        if(newbrid_update==true)
        //        {
        //            TempData["NewBranchMessage"] = "New Branch Manager Sucessfully Updated....!";
        //        }
        //        else
        //        {
        //            TempData["NewBranchMessage"] = "Updation Failed.";
        //        }
        //    }
        //    return View();
        //}
        public JsonResult UpdatenewbranchManager(string empcodenew, string branchnamenew)
        {
            //try
            //{
            int emp_code, emp_code_new, branch_id_new;
            string strqueryupdatebranch_new = "", branch_name_new, strqueryupdatebranch_newbranch = "", empname_new, designation_new;
            string successmess,errornoempid,errorretiredemp;
            bool newbrid_update_new = false;
            DateTime ret_date_new;
            DataTable dt_getempid_new, dt_getretiredate_new, dt_getbranchid_new, dt_getnewbranchmanager;
            dt_getempid_new = new SqlHelper().Get_Table_FromQry("Select [EmpId] from Employees where EmpId=" + empcodenew + "");
            if (dt_getempid_new.Rows.Count == 0)
            {
                errornoempid = "Employee doesn't exists..";
                return Json(new { lerrnoempid = errornoempid }, JsonRequestBehavior.AllowGet);
            }
            dt_getretiredate_new = new SqlHelper().Get_Table_FromQry("Select [EmpId],[RetirementDate] from Employees where RetirementDate>=Getdate() and EmpId=" + empcodenew + "");
            if(dt_getretiredate_new.Rows.Count==0)
            {
                errorretiredemp = "Employee already retired..";
                return Json(new { lerrretemp = errorretiredemp }, JsonRequestBehavior.AllowGet);
            }
            //dt_getbranchid_new = new SqlHelper().Get_Table_FromQry("Select [Id],[Name],[BM_Id] from Branches where [Id]=" + branchnamenew);

            strqueryupdatebranch_new = "Update Branches set BM_ID=" + empcodenew + " where Id=" + branchnamenew;
            newbrid_update_new = new SqlHelper().Run_UPDDEL_ExecuteNonQuery(strqueryupdatebranch_new);

            dt_getnewbranchmanager = new SqlHelper().Get_Table_FromQry("Select [EmpId],[EmpName],[Name] from v_BranchContactList where EmpId=" + empcodenew + "");
            designation_new = dt_getnewbranchmanager.Rows[0]["Name"].ToString();
            empname_new = dt_getnewbranchmanager.Rows[0]["EmpName"].ToString();

            successmess = "New Branch Manager Sucessfully Updated....!";
            //}
            //catch (Exception ex)
            //{
            //    TempData["ErrorMessage"] = ex.Message;
            //}

            return Json(new { lempidbranchnewAjax = empcodenew, lbrempnamenewAjax = empname_new, ldesignationnewAjax = designation_new, lsuccessmesAjax = successmess }, JsonRequestBehavior.AllowGet);
        }
        //end
    }
}