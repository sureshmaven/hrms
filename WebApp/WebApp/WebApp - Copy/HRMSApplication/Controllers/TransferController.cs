using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Entities;
using Repository;
using HRMSApplication.Models;
using HRMSApplication.Helpers;
using HRMSApplication.Filters;
using System.Web.Services;
using HRMSBusiness.Business;
using HRMSBusiness.Reports;
using Mavensoft.DAL.Db;
using System.Configuration;

namespace HRMSApplication.Controllers
{
    //[NoDirectAccess]
    [Authorize]
    [SessionTimeoutAttribute]
    public class TransferController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        // GET: Transfer
        public ActionResult Index()
        {

            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();

            var dt = new SqlHelper().Get_Table_FromQry("Select [Id],[category] from pr_emp_categories Where active=1");
            var items = dt.AsEnumerable().Select(r => new Branches
            {
                Id = (Int32)(r["Id"]),
                Name = (string)(r["category"] ?? "null")
            }).ToList();

           
            ViewBag.Categories = new SelectList(items, "Id", "Name");
            var lbranchdesig = db.Branch_Designation_Mapping.ToList();
            var ldesignation = db.Designations.ToList();
            int lHeadofficeValue = db.Branches.Where(a => a.Name == "HeadOffice").Select(a => a.Id).FirstOrDefault();
            var itemss = (from emplist in lbranchdesig
                          join desig in ldesignation on emplist.DesignationId equals desig.Id
                          where emplist.BranchId == lHeadofficeValue
                          select new
                          {
                              desig.Id,
                              desig.Name
                          });
            ViewBag.CurrentDesignationhead = new SelectList(itemss, "Id", "Name");
            var items2 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();
            ViewBag.Designation = new SelectList(items2, "Id", "Name");
            var items3 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TSCAB-CTI").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Branches = new SelectList(items3, "Id", "Name");
            var items6 = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
            {
                Id = x.Id,
                Name = x.Name
            });
            ViewBag.Department = new SelectList(items6, "Id", "Name");
            var items5 = Facade.EntitiesFacade.GetAll().Select(x => new Employees
            {
                Id = x.Id,
                LastName = x.FirstName.ToString() + " " + x.LastName.ToString()
            }).Distinct();
            ViewBag.EmpName = new SelectList(items5, "Id", "LastName");
            Session["Data"] = null;
            return View();
        }
        public string GetBranchDepartmentConcat(string branch, string Department)
        {
            string requireformate = "";
            if (branch != "OtherBranch" && branch != "TBD")
            {
                requireformate = branch;
            }
            if (Department != "OtherDepartment" && Department != "TBD")
            {
                requireformate = Department;
            }

            if (branch != null && Department != null)
            {

            }
            else
            {
                requireformate = requireformate.Replace('/', ' ');
            }
            return requireformate;
        }
        public JsonResult GetEmpidList(string criteria)
        {

            var query = (from c in db.Employes
                         where c.EmpId.Contains(criteria)
                         orderby c.EmpId ascending
                         select new { c.EmpId }).Distinct();
            return Json(query.ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Index1555(string criteria)
        {
            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            var employees = db.Employes.ToList();
            var lResult = (from userslist in employees
                           where userslist.RetirementDate >= lStartDate
                           select new
                           {
                               userslist.EmpId,
                               Name = userslist.FirstName + "" + userslist.LastName,
                           });
            var lresponseArray = lResult.ToArray();
            return Json(lresponseArray.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Basic(string prefix)
        {
            SqlHelper sh = new SqlHelper();
           
            string oldbasic = "";
            string query1 = "  select amount as old from pr_emp_pay_field pay join pr_earn_field_master pearn on pearn.id = pay.m_id and pearn.name = 'basic' and pearn.active = 1  where emp_code = "+ prefix + "";
            DataTable pdt1 = sh.Get_Table_FromQry(query1);
            foreach (DataRow dr in pdt1.Rows)
            {
               
                oldbasic = dr["old"].ToString();
            }

            return Json(oldbasic, JsonRequestBehavior.AllowGet);
        }
    [HttpGet]
        public JsonResult Index111(string prefix)
        {

            DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
            string lrdate = Convert.ToString("lStartDate");
            string status = "false";
            if (prefix == "")
            {
                string lempid = prefix;
                var employees = db.Employes.ToList();
                var lbranches = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var ldesignations = db.Designations.ToList();
                var lEmployee_Transfers = (from etrans in db.Employee_Transfer
                                               //where etrans. >= lStartDate
                                           orderby etrans.Id descending
                                           select new
                                           {
                                               etrans.Id,
                                               etrans.Type,
                                               etrans.EmpId,
                                               etrans.OldDesignation,
                                               etrans.NewDesignation,
                                               etrans.OldBranch,
                                               etrans.NewBranch,
                                               etrans.OldDepartment,
                                               etrans.NewDepartment,
                                               etrans.EffectiveFrom,
                                               etrans.EffectiveTo,
                                               etrans.Transfer_Type,

                                           }).ToList();
                var lResult = (from userslist in employees
                               join branchlist in lbranches on userslist.Branch equals branchlist.Id
                               join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                               join dept in ldepartments on userslist.Department equals dept.Id
                               join emptrnsfer in lEmployee_Transfers on userslist.Id equals emptrnsfer.EmpId
                               join branchlist1 in lbranches on emptrnsfer.NewBranch equals branchlist1.Id
                               join dept1 in ldepartments on emptrnsfer.NewDepartment equals dept1.Id
                               where userslist.RetirementDate >= lStartDate
                               orderby emptrnsfer.Id descending
                               select new
                               {
                                   userslist.EmpId,
                                   EmployeeName = userslist.FirstName + "" + userslist.LastName,
                                   Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                   desig.Name,
                                   emptrnsfer= GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                               }).Take(1);
                var lresponseArray = lResult.ToArray();
                string employeeId = lresponseArray[0].EmpId;
                string employeeName = lresponseArray[0].EmployeeName;
                string Deptbranchs = lresponseArray[0].Deptbranch;
                string ldesignation = lresponseArray[0].Name;
                string emptrnsfers = lresponseArray[0].emptrnsfer;
                return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation, Status = status , lemptrnsfer= emptrnsfers }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string lstatus = "True";
                string lempid = prefix;
                int count = db.Employes.Where(a => a.EmpId == lempid).Count();
                string RetirementDate = db.Employes.Where(a => a.EmpId == lempid).Select(a=>a.RetirementDate.ToString()).FirstOrDefault();
                DateTime lrdatee = Convert.ToDateTime(RetirementDate).Date;
                if (count == 0)
                {
                    string lstatus1 = "Notfound";
                    var employees = db.Employes.ToList();
                    var lbranches = db.Branches.ToList();
                    var ldepartments = db.Departments.ToList();
                    var ldesignations = db.Designations.ToList();
                    var lEmployee_Transfers = (from etrans in db.Employee_Transfer
                                                   //where etrans. >= lStartDate
                                               orderby etrans.Id descending
                                               select new
                                               {
                                                   etrans.Id,
                                                   etrans.Type,
                                                   etrans.EmpId,
                                                   etrans.OldDesignation,
                                                   etrans.NewDesignation,
                                                   etrans.OldBranch,
                                                   etrans.NewBranch,
                                                   etrans.OldDepartment,
                                                   etrans.NewDepartment,
                                                   etrans.EffectiveFrom,
                                                   etrans.EffectiveTo,
                                                   etrans.Transfer_Type,

                                               }).ToList();
                    var lResult = (from userslist in employees
                                   join branchlist in lbranches on userslist.Branch equals branchlist.Id
                                   join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                                   join dept in ldepartments on userslist.Department equals dept.Id
                                   join emptrnsfer in lEmployee_Transfers on userslist.Id equals emptrnsfer.EmpId
                                   join branchlist1 in lbranches on emptrnsfer.NewBranch equals branchlist1.Id
                                   join dept1 in ldepartments on emptrnsfer.NewDepartment equals dept1.Id
                                   orderby emptrnsfer.Id descending
                                   select new
                                   {
                                       userslist.EmpId,
                                       EmployeeName = userslist.FirstName + "" + userslist.LastName,
                                       Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                       desig.Name,
                                       emptrnsfer = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                   }).Take(1);
                    
                    var lresponseArray = lResult.ToArray();
                    //string employeeId = "";
                    //string employeeName = "";
                    //string Deptbranchs = "";
                    //string ldesignation = "";
                    //if (prefix != "")
                    //{
                    string employeeId = lresponseArray[0].EmpId;
                    string employeeName = lresponseArray[0].EmployeeName;
                    string Deptbranchs = lresponseArray[0].Deptbranch;
                    string ldesignation = lresponseArray[0].Name;
                    string emptrnsfers = lresponseArray[0].emptrnsfer;
                    return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation, Status = lstatus1, lemptrnsfer = emptrnsfers }, JsonRequestBehavior.AllowGet);
                }
                else
                if (lrdatee < lStartDate)
                {
                    string lstatus1 = "AlreadyRetired";
                    var employees = db.Employes.ToList();
                    var lbranches = db.Branches.ToList();
                    var ldepartments = db.Departments.ToList();
                    var ldesignations = db.Designations.ToList();
                    var lEmployee_Transfers = (from etrans in db.Employee_Transfer
                                                   //where etrans. >= lStartDate
                                               orderby etrans.Id descending
                                               select new
                                               {
                                                   etrans.Id,
                                                   etrans.Type,
                                                   etrans.EmpId,
                                                   etrans.OldDesignation,
                                                   etrans.NewDesignation,
                                                   etrans.OldBranch,
                                                   etrans.NewBranch,
                                                   etrans.OldDepartment,
                                                   etrans.NewDepartment,
                                                   etrans.EffectiveFrom,
                                                   etrans.EffectiveTo,
                                                   etrans.Transfer_Type,

                                               }).ToList();
                    var lResult = (from userslist in employees
                                   join branchlist in lbranches on userslist.Branch equals branchlist.Id
                                   join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                                   join dept in ldepartments on userslist.Department equals dept.Id
                                   join emptrnsfer in lEmployee_Transfers on userslist.Id equals emptrnsfer.EmpId
                                   join branchlist1 in lbranches on emptrnsfer.NewBranch equals branchlist1.Id
                                   join dept1 in ldepartments on emptrnsfer.NewDepartment equals dept1.Id
                                   orderby emptrnsfer.Id descending
                                   select new
                                   {
                                       userslist.EmpId,
                                       EmployeeName = userslist.FirstName + "" + userslist.LastName,
                                       Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                       desig.Name,
                                       emptrnsfer = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                   }).Take(1);
                    var lresponseArray = lResult.ToArray();
                    //string employeeId = "";
                    //string employeeName = "";
                    //string Deptbranchs = "";
                    //string ldesignation = "";
                    //if (prefix != "")
                    //{
                    string employeeId = lresponseArray[0].EmpId;
                    string employeeName = lresponseArray[0].EmployeeName;
                    string Deptbranchs = lresponseArray[0].Deptbranch;
                    string ldesignation = lresponseArray[0].Name;
                    string emptrnsfers = lresponseArray[0].emptrnsfer;
                    return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation, Status = lstatus1, lemptrnsfer = emptrnsfers }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //int emptrans = Convert.ToInt32(lempid);
                    string emptra = db.Employes.Where(a => a.EmpId == lempid).Select(a => a.Id.ToString()).FirstOrDefault();
                    int emptrans = Convert.ToInt32(emptra);

                    var employees = db.Employes.ToList();
                    var lbranches = db.Branches.ToList();
                    //var lbranches1 = db.Branches.ToList();
                    var ldepartments = db.Departments.ToList();
                    var ldesignations = db.Designations.ToList();// var lEmployee_Transfers = db.Employee_Transfer. ToList();
                    var lEmployee_Transfers = (from etrans in db.Employee_Transfer
                                  //where etrans.EmpId == emptrans
                                               orderby etrans.Id descending
                                   select new
                                   {
                                       etrans.Id ,
                                       etrans.Type ,
                                       etrans.EmpId  ,etrans.OldDesignation,
                                       etrans.NewDesignation ,
                                       etrans.OldBranch ,
                                       etrans.NewBranch,
                                       etrans.OldDepartment,
                                       etrans.NewDepartment  , etrans.EffectiveFrom,   etrans.EffectiveTo ,etrans.Transfer_Type,
                                       
                                   }).ToList();
                    var lEmployee_Transfers_data = (from etrans in db.Employee_Transfer
                                               where etrans.EmpId == emptrans
                                               orderby etrans.Id descending
                                               select new
                                               {
                                                   etrans.Id,
                                                   etrans.Type,
                                                   etrans.EmpId,
                                                  

                                               })
                                               .FirstOrDefault();

                    
                    //string employeeId = "";
                    //string employeeName = "";
                    //string Deptbranchs = "";
                    //string ldesignation = "";
                    //if (prefix != "")
                    //{
                   
                    if (lEmployee_Transfers_data == null)
                    {
                        var lResult = (from userslist in employees
                                       join branchlist in lbranches on userslist.Branch equals branchlist.Id
                                       join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                                       join dept in ldepartments on userslist.Department equals dept.Id
                                       //join emptrnsfer in lEmployee_Transfers on userslist.Id equals emptrnsfer.EmpId
                                      // join branchlist1 in lbranches on emptrnsfer.NewBranch equals branchlist1.Id
                                      // join dept1 in ldepartments on emptrnsfer.NewDepartment equals dept1.Id
                                       where userslist.EmpId == lempid
                                      // orderby emptrnsfer.Id descending
                                       select new
                                       {
                                           userslist.EmpId,
                                           EmployeeName = userslist.FirstName + "" + userslist.LastName,
                                           Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                           desig.Name,
                                           emptrnsfer = GetBranchDepartmentConcat(branchlist.Name, dept.Name),
                                       }).Take(1);
                        var lresponseArray = lResult.ToArray();
                        string employeeId = lresponseArray[0].EmpId;
                        string employeeName = lresponseArray[0].EmployeeName;
                        string Deptbranchs = lresponseArray[0].Deptbranch;
                        string ldesignation = lresponseArray[0].Name;
                        string emptrnsfers = lresponseArray[0].emptrnsfer;
                        return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation, Status = lstatus, lemptrnsfer = emptrnsfers }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //Transfers and Employee data
                        var lResult = (from userslist in employees
                                       join branchlist in lbranches on userslist.Branch equals branchlist.Id
                                       join desig in ldesignations on userslist.CurrentDesignation equals desig.Id
                                       join dept in ldepartments on userslist.Department equals dept.Id
                                       join emptrnsfer in lEmployee_Transfers on userslist.Id equals emptrnsfer.EmpId
                                       join branchlist1 in lbranches on emptrnsfer.NewBranch equals branchlist1.Id
                                       join dept1 in ldepartments on emptrnsfer.NewDepartment equals dept1.Id
                                       where userslist.EmpId == lempid
                                       orderby emptrnsfer.Id descending
                                       select new
                                       {
                                           userslist.EmpId,
                                           EmployeeName = userslist.FirstName + "" + userslist.LastName,
                                           Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept1.Name),
                                           desig.Name,
                                           emptrnsfer = GetBranchDepartmentConcat(branchlist1.Name, dept.Name),
                                       }).Take(1);
                        var lresponseArray = lResult.ToArray();

                        string employeeId = lresponseArray[0].EmpId;
                        string employeeName = lresponseArray[0].EmployeeName;
                        string Deptbranchs = lresponseArray[0].Deptbranch;
                        string ldesignation = lresponseArray[0].Name;
                        string emptrnsfers = lresponseArray[0].emptrnsfer;
                        return Json(new { lEmployeeId = employeeId, lEmployeeName = employeeName, ldeptbranch = Deptbranchs, ldesig = ldesignation, Status = lstatus, lemptrnsfer = emptrnsfers }, JsonRequestBehavior.AllowGet);

                    }
                }
            }

        }

        public static DateTime GetCurrentTime(DateTime ldate)
        {
            DateTime serverTime = DateTime.Now;
            DateTime utcTime = serverTime.ToUniversalTime();
            // convert it to Utc using timezone setting of server computer
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(EmpTransfer transfer, FormCollection from)
        {
            //string[] payrollaccess = ConfigurationManager.AppSettings["PayrollAccessId"].Split(',');
            string empstring = "";
            Mavensoft.DAL.Db.SqlHelper sha = new Mavensoft.DAL.Db.SqlHelper();
            string qry = "select constant from all_constants where functionality='LoginAccess' and active=1";
            DataTable constat = sha.Get_Table_FromQry(qry);
            foreach (DataRow dr in constat.Rows)
            {
                empstring = dr["constant"].ToString();
            }

            SqlHelper sh = new SqlHelper();
          
            string fms = "";
            int fys = 0001;

            string query = " select fm, fy from pr_month_details where active = 1";
            DataTable pdt = sh.Get_Table_FromQry(query);
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            string tem = TempData["RolePages"].ToString();
            foreach (DataRow dr in pdt.Rows)
            {
                fms = Convert.ToDateTime(dr["fm"]).ToString("yyyy-MM-dd");
                fys = Convert.ToInt32(dr["fy"]);
            }
         
            var emplist = db.Employes.ToList();
            string lEmpIdValue = from["fffff"];
            var res = new Employee_Transfer();
            string lid = lEmpIdValue.Replace(",", "");
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            var ldbresult = db.Employes.ToList();
            int ltansferId = db.Employes.Where(a => a.EmpId == lid).Select(a => a.Id).FirstOrDefault();
            var ldata = Facade.EntitiesFacade.GetEmpTabledata.GetById(ltansferId);

            WorkDiaryBus lbus = new WorkDiaryBus();
            int lcount = lbus.CheckTransfers(transfer.EffectiveFromP, transfer.EffectiveFromPT, transfer.EffectiveFromT, transfer.EffectiveToT, ltansferId.ToString(), transfer.Type, transfer.Transfer_Type);
            if (ltansferId == 0)
            {
                TempData["AlertMessage"] = "EmpCode doesnot exists";
                return RedirectToAction("Index");
            }
            if (lcount != 0)
            {
                if (transfer.Transfer_Type == "TemporaryTransfer")
                {
                    TempData["AlertMessage"] = "Employee is already Temporary Transfered";
                }

                if (transfer.Transfer_Type == "PermanentTransfer")
                {
                    TempData["AlertMessage"] = "Employee is already Permanent Transfered";
                }

                if (transfer.Type == "Promotion")
                {
                    TempData["AlertMessage"] = "Employee is already Promoted";
                }
                if (transfer.Type == "PromotionTransfer")
                {
                    TempData["AlertMessage"] = "Employee is already Promoted";
                }
                if (transfer.Type == "PromotionTransfer")
                {
                    TempData["AlertMessage"] = "Employee is already Promoted and Transfered";
                }

                // TempData["AlertMessage"] = "Employee is already"+" "+transfer.Type+"ed";
                return RedirectToAction("Index");
            }
            else
            {
                if (transfer.RadioValue1 == "42" && transfer.Type == "Deputation")
                {
                    res.EmpId = ldata.Id;
                    string lvalue = "OtherBranch";
                    int lId = db.Branches.Where(a => a.Name == lvalue).Select(a => a.Id).FirstOrDefault();
                    res.OldDepartment = ldata.Department;
                    res.NewDepartment = transfer.NewDepartment;
                    res.NewBranch = ldata.Branch;
                    res.OldBranch = ldata.Branch;
                    res.UpdatedDate = GetCurrentTime(DateTime.Now);
                    res.Type = transfer.Type;
                    res.Transfer_Type = transfer.Transfer_Type;
                    res.EffectiveFrom = transfer.EffectiveFrom;
                    res.EffectiveTo = transfer.EffectiveTo;
                    res.NewDesignation = ldata.CurrentDesignation;
                    res.OldDesignation = ldata.CurrentDesignation;
                    res.UpdatedBy = lCredentials.EmpId;
                    db.Employee_Transfer.Add(res);
                    db.SaveChanges();
                    DateTime lstartdate = GetCurrentTime(DateTime.Now.Date);
                    if (transfer.EffectiveFrom <= lstartdate.Date)
                    {
                        Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                        lemployee.Department = transfer.NewDepartment;
                        lemployee.CurrentDesignation = ldata.CurrentDesignation;
                        lemployee.Branch_Value1 = transfer.RadioValue1;
                        lemployee.Branch = lId;
                        lemployee.UpdatedBy = lCredentials.EmpId;
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    TempData["AlertMessage"] = "Submitted Successfully";
                }
                else if (transfer.RadioValue1 == "43" && transfer.Type == "Deputation")
                {
                    res.EmpId = ldata.Id;
                    string lvalue = "OtherDepartment";
                    int lId = db.Departments.Where(a => a.Code == lvalue).Select(a => a.Id).FirstOrDefault();
                    res.OldDepartment = ldata.Department;
                    res.NewDepartment = lId;
                    res.Type = transfer.Type;
                    res.UpdatedDate = GetCurrentTime(DateTime.Now);
                    res.Transfer_Type = transfer.Transfer_Type;
                    res.EffectiveFrom = transfer.EffectiveFrom;
                    res.EffectiveTo = transfer.EffectiveTo;
                    res.OldBranch = ldata.Branch;
                    res.NewBranch = transfer.NewBranch;
                    res.NewDesignation = ldata.CurrentDesignation;
                    res.OldDesignation = ldata.CurrentDesignation;
                    res.UpdatedBy = lCredentials.EmpId;
                    db.Employee_Transfer.Add(res);
                    db.SaveChanges();
                    DateTime lstartdate = GetCurrentTime(DateTime.Now.Date);
                    if (transfer.EffectiveFrom <= lstartdate.Date)
                    {
                        Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                        lemployee.Branch = transfer.NewBranch;
                        lemployee.Department = lId;
                        lemployee.CurrentDesignation = ldata.CurrentDesignation;
                        lemployee.Branch_Value1 = transfer.RadioValue1;
                        lemployee.UpdatedBy = lCredentials.EmpId;
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    TempData["AlertMessage"] = "Submitted Successfully";
                }
                else if (transfer.Type == "Promotion")
                {
                    string categoryname = "";
                    string querycat = " select category from pr_emp_categories where active = 1 and id="+transfer.Pcategory+"";
                    DataTable pcat = sh.Get_Table_FromQry(querycat);
                    foreach (DataRow dr in pcat.Rows)
                    {
                        categoryname = dr["category"].ToString();
                       
                    }
                    ReportBusiness Rbus = new ReportBusiness();
                    int employeeid = db.Employes.Where(a => a.Id == ldata.Id).Select(a => a.Id).FirstOrDefault();
                    var dt1 = Rbus.olddesig(employeeid, lCredentials.EmpPkId);
                    int olddesig = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldDesignation).FirstOrDefault();
                 
                    var dt2 = Rbus.NewBranch(employeeid, lCredentials.EmpPkId);
                    int Newbranch = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldBranch).FirstOrDefault();

                    var dt5 = Rbus.oldBranch(employeeid, lCredentials.EmpPkId);
                    int oldbranch = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldBranch).FirstOrDefault();

                    var dt3 = Rbus.NewDept(employeeid, lCredentials.EmpPkId);
                    int NewDept = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldDepartment).FirstOrDefault();

                    var dt4 = Rbus.oldDept(employeeid, lCredentials.EmpPkId);
                    int oldDept = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldDepartment).FirstOrDefault();


                    if (ldata.Id == 0)
                    {
                        TempData["AlertMessage"] = "EmpCode doesnot exists";
                        return RedirectToAction("Index");
                    }
                    int Employeedet = ldata.Id;
                    int empdesig = db.Employes.Where(a => a.Id == Employeedet).Select(a => a.CurrentDesignation).FirstOrDefault();
                    if (empdesig == transfer.NewDesignation)
                    {
                        TempData["AlertMessage"] = "Employee with same Designation cannot be Promoted.";
                       
                        if (empstring.Split(',').Contains(lCredentials.EmpId))
                        {
                            return RedirectToAction("Index/PayrollPromotion");
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        res.EmpId = ldata.Id;
                        // res.OldDepartment = ldata.Department;

                        if (oldDept == 0)
                        {
                            res.OldDepartment = ldata.Department;
                        }
                        else
                        {
                            if (dt4 != "")
                            {
                                res.OldDepartment = Convert.ToInt32(dt4);
                            }
                            else
                            {
                                res.OldDepartment = ldata.Department;
                            }
                        }
                        //res.NewDepartment = ldata.Department;
                        if (NewDept == 0)
                        {
                            res.NewDepartment = ldata.Department;
                        }
                        else
                        {
                            if (dt3 != "")
                            {
                                res.NewDepartment = Convert.ToInt32(dt3);
                            }
                            else
                            {
                                res.NewDepartment = ldata.Department;
                            }
                        }
                        //res.NewBranch = ldata.Branch;
                        if (NewDept == 0)
                        {
                            res.NewBranch = ldata.Branch;
                        }
                        else
                        {
                            if (dt2 != "")
                            {
                                res.NewBranch = Convert.ToInt32(dt2);
                            }
                            else
                            {
                                res.NewBranch = ldata.Branch;
                            }
                        }
                        res.EffectiveFrom = transfer.EffectiveFromP;
                        //res.OldBranch = ldata.Branch;
                        if (oldbranch == 0)
                        {
                            res.OldBranch = ldata.Branch;
                        }
                        else
                        {
                            if (dt5 != "")
                            {
                                res.OldBranch = Convert.ToInt32(dt5);
                            }
                            else
                            {
                                res.OldBranch = ldata.Branch;
                            }
                        }
                        res.UpdatedDate = GetCurrentTime(DateTime.Now);
                        res.Type = transfer.Type;
                        res.Transfer_Type = transfer.Type;
                        res.NewDesignation = transfer.NewDesignation;
                        res.fm = fms;
                        res.fy = fys;
                        res.incre_due_date = transfer.Pincrement;
                        res.active = 1;
                        res.authorisation = 0;
                        res.category = categoryname;
                        res.new_basic = transfer.Pnewbasic;
                        res.old_basic = transfer.Poldbasic;
                        res.senoirity_order = transfer.Pseniority;
                        //res.OldDesignation = ldata.CurrentDesignation;
                        if (olddesig == 0)
                        {
                            res.OldDesignation = ldata.CurrentDesignation;
                        }
                        else
                        {
                            res.OldDesignation = Convert.ToInt32(dt1);
                        }
                        res.UpdatedBy = lCredentials.EmpId;
                        db.Employee_Transfer.Add(res);
                        db.SaveChanges();
                        DateTime lstartdate = GetCurrentTime(DateTime.Now.Date);
                        if (transfer.EffectiveFromP <= lstartdate.Date)
                        {
                            
                            
                            
                            
                            Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                           // lemployee.Department = ldata.Department;
                            //lemployee.Branch = ldata.Branch;
                            lemployee.CurrentDesignation = transfer.NewDesignation;
                            lemployee.UpdatedBy = lCredentials.EmpId;
                            db.Entry(lemployee).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        TempData["AlertMessage"] = "Submitted Successfully";
                    }
                }
                else if (transfer.RadioValue2 == "42" && transfer.Type == "Transfer" && transfer.Transfer_Type == "TemporaryTransfer")
                {

                    ReportBusiness Rbus = new ReportBusiness();
                    int employeeid = db.Employes.Where(a => a.Id == ldata.Id).Select(a => a.Id).FirstOrDefault();
                    var dt1 = Rbus.olddesig(employeeid, lCredentials.EmpPkId);

                    int olddesig = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldDesignation).FirstOrDefault();
                    res.EmpId = ldata.Id;
                    res.OldDepartment = ldata.Department;
                    string lBvalue = "OtherBranch";
                    int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                    res.NewDepartment = transfer.NewDepartmentT;
                    res.EffectiveFrom = transfer.EffectiveFromT;
                    res.EffectiveTo = transfer.EffectiveToT;
                    res.NewBranch = lBranch;
                    res.UpdatedDate = GetCurrentTime(DateTime.Now);
                    DateTime update = DateTime.Now.Date;
                    res.OldBranch = ldata.Branch;
                   
                    res.NewDesignation = ldata.CurrentDesignation;
                    if (olddesig == 0)
                    {
                        res.OldDesignation = ldata.CurrentDesignation;
                    }
                    else
                    {
                        res.OldDesignation = Convert.ToInt32(dt1);
                    }
                    res.Transfer_Type = transfer.Transfer_Type;
                    res.Type = transfer.Transfer_Type;
                    res.UpdatedBy = lCredentials.EmpId;
                    db.Employee_Transfer.Add(res);
                    db.SaveChanges();
                    DateTime lstartdate = GetCurrentTime(DateTime.Now.Date);
                    if (res.UpdatedDate >= transfer.EffectiveFromT && update <= transfer.EffectiveToT)
                    {
                        Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                        lemployee.Department = transfer.NewDepartmentT;
                        lemployee.Branch = lBranch;
                        lemployee.Branch_Value1 = transfer.RadioValue2;
                        lemployee.CurrentDesignation = ldata.CurrentDesignation;
                        lemployee.UpdatedBy = lCredentials.EmpId;
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (transfer.EffectiveToT != null && res.UpdatedDate >= transfer.EffectiveToT)
                    {
                        Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                        lemployee.Branch = ldata.Branch;
                        lemployee.Department = ldata.Department;
                        lemployee.CurrentDesignation = ldata.CurrentDesignation;
                        lemployee.Branch_Value1 = ldata.Branch_Value1;
                        lemployee.UpdatedBy = lCredentials.EmpId;
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                  else  if (res.UpdatedDate >= transfer.EffectiveFromT)
                    {
                        Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                        lemployee.Department = transfer.NewDepartmentT;
                        lemployee.Branch = lBranch;
                        lemployee.Branch_Value1 = transfer.RadioValue2;
                        lemployee.CurrentDesignation = ldata.CurrentDesignation;
                        lemployee.UpdatedBy = lCredentials.EmpId;
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    TempData["AlertMessage"] = "Submitted Successfully";
                }
                else if (transfer.RadioValue2 == "43" && transfer.Type == "Transfer" && transfer.Transfer_Type == "TemporaryTransfer")
                {
                    ReportBusiness Rbus = new ReportBusiness();
                    int employeeid = db.Employes.Where(a => a.Id == ldata.Id).Select(a => a.Id).FirstOrDefault();
                    var dt1 = Rbus.olddesig(employeeid, lCredentials.EmpPkId);
                    int olddesig = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldDesignation).FirstOrDefault();

                    res.EmpId = ldata.Id;
                    string lvalue = "OtherDepartment";
                    int lId = db.Departments.Where(a => a.Code == lvalue).Select(a => a.Id).FirstOrDefault();
                    res.OldDepartment = ldata.Department;
                    res.EffectiveFrom = transfer.EffectiveFromT;
                    res.EffectiveTo = transfer.EffectiveToT;
                    res.NewBranch = transfer.NewBranchT;
                    res.NewDepartment = lId;
                    DateTime update = DateTime.Now.Date;
              
                    res.UpdatedDate = GetCurrentTime(DateTime.Now);
                    res.Transfer_Type = transfer.Transfer_Type;
                    res.Type = transfer.Transfer_Type;
                    res.OldBranch = ldata.Branch;
                    res.NewDesignation = ldata.CurrentDesignation;
                    if (olddesig == 0)
                    {
                        res.OldDesignation = ldata.CurrentDesignation;
                    }
                    else
                    {
                        res.OldDesignation = Convert.ToInt32(dt1);
                    }
                    res.UpdatedBy = lCredentials.EmpId;
                    db.Employee_Transfer.Add(res);
                    db.SaveChanges();
                    DateTime lstartdate = GetCurrentTime(DateTime.Now.Date);
                    if (res.UpdatedDate >= transfer.EffectiveFromT && update <= transfer.EffectiveToT)
                    {
                        Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                        lemployee.Branch = transfer.NewBranchT;
                        lemployee.Department = lId;
                        lemployee.CurrentDesignation = ldata.CurrentDesignation;
                        lemployee.Branch_Value1 = transfer.RadioValue2;
                        lemployee.UpdatedBy = lCredentials.EmpId;
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                   else if (transfer.EffectiveToT != null && res.UpdatedDate>=transfer.EffectiveToT)
                    {
                        Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                        lemployee.Branch = ldata.Branch;
                        lemployee.Department = ldata.Department;
                        lemployee.CurrentDesignation = ldata.CurrentDesignation;
                        lemployee.Branch_Value1 = ldata.Branch_Value1;
                        lemployee.UpdatedBy = lCredentials.EmpId;
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                   else if (res.UpdatedDate >= transfer.EffectiveFromT)
                    {
                        Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                        lemployee.Branch = transfer.NewBranchT;
                        lemployee.Department = lId;
                        lemployee.CurrentDesignation = ldata.CurrentDesignation;
                        lemployee.Branch_Value1 = transfer.RadioValue2;
                        lemployee.UpdatedBy = lCredentials.EmpId;
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    TempData["AlertMessage"] = "Submitted Successfully";
                }
                else if (transfer.RadioValue2 == "42" && transfer.Type == "Transfer" && transfer.Transfer_Type == "PermanentTransfer")
                {
                    ReportBusiness Rbus = new ReportBusiness();
                    int employeeid = db.Employes.Where(a => a.Id == ldata.Id).Select(a => a.Id).FirstOrDefault();
                    var dt1 = Rbus.olddesig(employeeid, lCredentials.EmpPkId);

                    int olddesig = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldDesignation).FirstOrDefault();
                    res.EmpId = ldata.Id;
                    res.OldDepartment = ldata.Department;
                    string lBvalue = "OtherBranch";
                    int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                    res.NewDepartment = transfer.NewDepartmentT;
                    res.EffectiveFrom = transfer.EffectiveFromT;
                    res.EffectiveTo = transfer.EffectiveToT;
                    res.NewBranch = lBranch;
                    res.UpdatedDate = GetCurrentTime(DateTime.Now);
                    res.OldBranch = ldata.Branch;
                    res.NewDesignation = ldata.CurrentDesignation;
                    if (olddesig == 0)
                    {
                        res.OldDesignation = ldata.CurrentDesignation;
                    }
                    else
                    {
                        res.OldDesignation = Convert.ToInt32(dt1);
                    }
                        res.Transfer_Type = transfer.Transfer_Type;
                        res.Type = transfer.Transfer_Type;
                        res.UpdatedBy = lCredentials.EmpId;
                        db.Employee_Transfer.Add(res);
                        db.SaveChanges();
                        DateTime lstartdate = GetCurrentTime(DateTime.Now.Date);
                        if (transfer.EffectiveFromT <= lstartdate.Date)

                        {
                            Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                            lemployee.Department = transfer.NewDepartmentT;
                            lemployee.Branch = lBranch;
                            lemployee.Branch_Value1 = transfer.RadioValue2;
                            lemployee.CurrentDesignation = ldata.CurrentDesignation;
                            lemployee.UpdatedBy = lCredentials.EmpId;
                            db.Entry(lemployee).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        TempData["AlertMessage"] = "Submitted Successfully";
                    }
                
                else if (transfer.RadioValue2 == "43" && transfer.Type == "Transfer" && transfer.Transfer_Type == "PermanentTransfer")
                {
                    ReportBusiness Rbus = new ReportBusiness();
                    int employeeid = db.Employes.Where(a => a.Id == ldata.Id).Select(a => a.Id).FirstOrDefault();
                    var dt1 = Rbus.olddesig(employeeid, lCredentials.EmpPkId);

                    int olddesig = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldDesignation).FirstOrDefault();
                    res.EmpId = ldata.Id;
                    string lvalue = "OtherDepartment";
                    int lId = db.Departments.Where(a => a.Code == lvalue).Select(a => a.Id).FirstOrDefault();
                    res.OldDepartment = ldata.Department;
                    res.EffectiveFrom = transfer.EffectiveFromT;
                    res.EffectiveTo = transfer.EffectiveToT;
                    res.NewBranch = transfer.NewBranchT;
                    res.NewDepartment = lId;
                    res.UpdatedDate = GetCurrentTime(DateTime.Now);
                    res.Transfer_Type = transfer.Transfer_Type;
                    res.Type = transfer.Transfer_Type;
                    res.OldBranch = ldata.Branch;
                    res.NewDesignation = ldata.CurrentDesignation;
                    if (olddesig == 0)
                    {
                        res.OldDesignation = ldata.CurrentDesignation;
                    }
                    else
                    {
                        res.OldDesignation = Convert.ToInt32(dt1);
                    }
                    res.UpdatedBy = lCredentials.EmpId;
                    db.Employee_Transfer.Add(res);
                    db.SaveChanges();
                    DateTime lstartdate = GetCurrentTime(DateTime.Now.Date);
                    if (transfer.EffectiveFromT <= lstartdate.Date)

                    {
                        Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                        lemployee.Branch = transfer.NewBranchT;
                        lemployee.Department = lId;
                        lemployee.CurrentDesignation = ldata.CurrentDesignation;
                        lemployee.Branch_Value1 = transfer.RadioValue2;
                        lemployee.UpdatedBy = lCredentials.EmpId;
                        db.Entry(lemployee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    TempData["AlertMessage"] = "Submitted Successfully";
                }
                else if (transfer.radiovalue == "42" && transfer.Type == "PromotionTransfer")
                {
                    ReportBusiness Rbus = new ReportBusiness();
                    int employeeid = db.Employes.Where(a => a.Id == ldata.Id).Select(a => a.Id).FirstOrDefault();
                    var dt1 = Rbus.olddesig(employeeid, lCredentials.EmpPkId);
                    int olddesig = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldDesignation).FirstOrDefault();
                    int Employeedet = ldata.Id;
                    int empdesig = db.Employes.Where(a => a.Id == Employeedet).Select(a => a.CurrentDesignation).FirstOrDefault();
                    if (empdesig == transfer.NewDesignationPT)
                    {
                        TempData["AlertMessage"] = "Employee with same Designation cannot be Promoted.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        res.EmpId = ldata.Id;
                        res.NewDepartment = transfer.NewDepartmentPT;
                        string lBvalue = "OtherBranch";
                        int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();
                        res.OldDepartment = ldata.Department;
                        res.EffectiveFrom = transfer.EffectiveFromPT;
                        res.NewBranch = lBranch;
                        res.OldBranch = ldata.Branch;
                        res.Type = transfer.Type;
                        res.UpdatedDate = GetCurrentTime(DateTime.Now);
                        res.Transfer_Type = transfer.Type;
                        res.NewDesignation = transfer.NewDesignationPT;
                        //res.OldDesignation = ldata.CurrentDesignation;
                        if (olddesig == 0)
                        {
                            res.OldDesignation = ldata.CurrentDesignation;
                        }
                        else
                        {
                            res.OldDesignation = Convert.ToInt32(dt1);
                        }
                        res.UpdatedBy = lCredentials.EmpId;
                        db.Employee_Transfer.Add(res);
                        db.SaveChanges();
                        DateTime lstartdate = GetCurrentTime(DateTime.Now.Date);
                        if (transfer.EffectiveFromPT <= lstartdate.Date)
                        {
                            Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                            lemployee.Department = transfer.NewDepartmentPT;
                            lemployee.Branch = lBranch;
                            lemployee.CurrentDesignation = transfer.NewDesignationPT;
                            lemployee.Branch_Value1 = transfer.radiovalue;
                            lemployee.UpdatedBy = lCredentials.EmpId;
                            db.Entry(lemployee).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        TempData["AlertMessage"] = "Submitted Successfully";
                    }
                }
                else
                {
                    int Employeedet = ldata.Id;
                    int empdesig = db.Employes.Where(a => a.Id == Employeedet).Select(a => a.CurrentDesignation).FirstOrDefault();
                    if (empdesig == transfer.NewDesignationPT)
                    {
                        TempData["AlertMessage"] = "Employee with same Designation canot be Promoted.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ReportBusiness Rbus = new ReportBusiness();
                        int employeeid = db.Employes.Where(a => a.Id == ldata.Id).Select(a => a.Id).FirstOrDefault();
                        var dt1 = Rbus.olddesig(employeeid, lCredentials.EmpPkId);
                        int olddesig = db.Employee_Transfer.Where(a => a.EmpId == ldata.Id).Select(a => a.OldDesignation).FirstOrDefault();
                        res.EmpId = ldata.Id;
                        string lvalue = "OtherDepartment";
                        int lId = db.Departments.Where(a => a.Code == lvalue).Select(a => a.Id).FirstOrDefault();
                        res.OldDepartment = ldata.Department;
                        res.NewDepartment = lId;
                        res.Type = transfer.Type;
                        res.Transfer_Type = transfer.Type;
                        res.EffectiveFrom = transfer.EffectiveFromPT;
                        res.NewBranch = transfer.NewBranchPT;
                        res.OldBranch = ldata.Branch;
                        res.UpdatedDate = GetCurrentTime(DateTime.Now);
                        res.NewDesignation = transfer.NewDesignationPT;
                        if (olddesig == 0)
                        {
                            res.OldDesignation = ldata.CurrentDesignation;
                        }
                        else
                        {
                            res.OldDesignation = Convert.ToInt32(dt1);
                        }
                        res.UpdatedBy = lCredentials.EmpId;
                        db.Employee_Transfer.Add(res);
                        db.SaveChanges();
                        DateTime lstartdate = GetCurrentTime(DateTime.Now.Date);
                        if (transfer.EffectiveFromPT <= lstartdate.Date)
                        {
                            Employees lemployee = emplist.Where(a => a.EmpId == lid).FirstOrDefault();
                            lemployee.Branch = transfer.NewBranchPT;
                            lemployee.Department = lId;
                            lemployee.CurrentDesignation = transfer.NewDesignationPT;
                            lemployee.Branch_Value1 = transfer.radiovalue;
                            lemployee.UpdatedBy = lCredentials.EmpId;
                            db.Entry(lemployee).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        TempData["AlertMessage"] = "Submitted Successfully";

                    }
                }

            }

            if (empstring.Split(',').Contains(lCredentials.EmpId) && tem.Contains("RPYLB"))
            {
                return RedirectToAction("Index");
            }
            else if (empstring.Split(',').Contains(lCredentials.EmpId) && !tem.Contains("RPYLB"))
            {
                return RedirectToAction("Index/PayrollPromotion");
            }
            else
            {
                return RedirectToAction("Index");
            }
           


        }

        public JsonResult HLoadByBranchId(string State)
        {

            int stateid = Convert.ToInt32(State);
            var query = (from b in db.Employes
                         join m in db.Designations on b.CurrentDesignation equals m.Id

                         where b.Department.Equals(stateid)

                         select new
                         {
                             m.Name,
                             m.Id
                         }).Distinct();



            //  List<Employees> lBranchs = db.Employes.Where(a => a.Department.ToString() == State).ToList();

            var stateData = query.Select(m => new SelectListItem()
            {
                Text = m.Name.ToString(),
                Value = m.Id.ToString(),
            });
            return Json(stateData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BLoadByBranchId(string State)
        {

            int stateid = Convert.ToInt32(State);
            var query = (from b in db.Employes
                         join m in db.Designations on b.CurrentDesignation equals m.Id

                         where b.Branch == stateid

                         select new
                         {
                             m.Name,
                             m.Id
                         }).Distinct();



            //List<Employees> lBranchs = db.Employes.Where(a => a.Department.ToString() == State).ToList();

            var stateData = query.Select(m => new SelectListItem()
            {
                Text = m.Name.ToString(),
                Value = m.Id.ToString(),
            });
            return Json(stateData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadByBranches1(string State)
        {

            string stateid = State;
            var lbranchdesig = db.Branch_Designation_Mapping.ToList();
            var ldesignation = db.Designations.ToList();
            int lHeadofficeValue = db.Branches.Where(a => a.Name == "HeadOffice").Select(a => a.Id).FirstOrDefault();
            int lid = db.Branches.Where(a => a.Name == stateid).Select(a => a.Id).FirstOrDefault();
            var query = (from emplist in lbranchdesig
                         join desig in ldesignation on emplist.DesignationId equals desig.Id
                         where emplist.BranchId == lHeadofficeValue

                         select new
                         {
                             desig.Name,
                             desig.Id
                         }).Distinct();
            var stateData = query.Select(m => new SelectListItem()
            {
                Text = m.Name.ToString(),
                Value = m.Id.ToString(),
            });

            ViewBag.DataStatus = stateData;

            string Names = "";
            foreach (var item in stateData)
            {
                Names = Names + "," + item.Text;

            }
            //  Session["Data"] = Names;
            return Json(stateData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LoadByBranches(string State)
        {

            string stateid = State;
            var lbranchdesig = db.Branch_Designation_Mapping.ToList();
            var ldesignation = db.Designations.ToList();
            int lid = db.Branches.Where(a => a.Name == stateid).Select(a => a.Id).FirstOrDefault();
            int lOtherBranchValue = db.Branches.Where(a => a.Name == "OtherBranch").Select(a => a.Id).FirstOrDefault();
            var query = (from emplist in lbranchdesig
                         join desig in ldesignation on emplist.DesignationId equals desig.Id
                         where emplist.BranchId == lOtherBranchValue

                         select new
                         {
                             desig.Name,
                             desig.Id
                         }).Distinct();
            var stateData = query.Select(m => new SelectListItem()
            {
                Text = m.Name.ToString(),
                Value = m.Id.ToString(),
            });

            ViewBag.DataStatus = stateData;

            string Names = "";
            foreach (var item in stateData)
            {
                Names = Names + "," + item.Text;

            }
            Session["Data"] = Names;
            return Json(stateData, JsonRequestBehavior.AllowGet);
        }
        //Reports:


        public string GetFirstandLastName(string FirstName, string LastName)
        {
            string lfirstname = "";
            lfirstname = FirstName.Length.ToString();
            int lfirst = Convert.ToInt32(lfirstname);
            if (lfirst >= 3)
            {
                lfirstname = FirstName.Substring(0, 1);
            }
            if (lfirst == 4)
            {
                lfirstname = FirstName.Substring(0, 2);
                if (lfirstname == "DR" || lfirstname == "Dr")
                {
                    lfirstname = lfirstname + "." + FirstName.Substring(2, 2);
                }
                else
                {
                    lfirstname = FirstName.Substring(0, 1);
                }
            }
            if (lfirst == 1)
            {
                lfirstname = FirstName.Substring(0, 1);
            }
            if (lfirst == 2)
            {
                lfirstname = FirstName.Substring(0, 2);
            }
            lfirstname = lfirstname + " " + LastName;
            return lfirstname;
        }

    }
}