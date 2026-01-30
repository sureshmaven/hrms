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

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        // GET: Departments/Create
        [NoDirectAccess]
        [HttpGet]
        [SessionTimeoutAttribute]
        public ActionResult Create()
        {
            
            var items3 = Facade.EntitiesFacade.GetAllBanks().Select(x => new Banks
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Banks = new SelectList(items3, "Name", "Name");

            var items4 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TSCAB-CTI").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Branches = new SelectList(items4, "Name", "Name");
            ViewModel lmodel = new ViewModel();
            Employees lemployees = new Employees();
            Departments ldepartments = new Departments();
            ldepartments.LoginMode = lCredentials.LoginMode;
            ViewBag.Message = lCredentials.LoginMode;
            lmodel.ldepartments = ldepartments;
            lmodel.lEmployess = lemployees;
            int id = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            var data = new Persistence.EmployeesRepository().GetIt(id);
            var items1 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
            {

                Name = x.Name
            });
            ViewBag.JoinedDesignation = new SelectList(items1, "Name", "Name");

            var items2 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
            {

                Name = x.Name
            });
            ViewBag.CurrentDesignation = new SelectList(items2, "Name", "Name");

            var items6 = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
            {

                Name = x.Name
            });
            ViewBag.Department = new SelectList(GetDepartments(), "Name", "Name");

            var items7 = Facade.EntitiesFacade.GetAllRoles().Select(x => new Roles
            {

                Name = x.Name
            });
            ViewBag.Role = new SelectList(items4, "Name", "Name");
            var items5 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TSCAB-CTI").Select(x => new Branches
            {
                Name = x.Name
            });
            ViewBag.Branch = new SelectList(items5, "Name", "Name");
            lmodel.lEmployess = data;
            Employees edata = (from u in db.Employes where u.Id == id select u).FirstOrDefault();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Departments/_DepartmentPartialView.cshtml", lmodel);
        }
        public IEnumerable<Departments> GetDepartments()
        {
            var ldepartment = db.Departments.ToList();
            return ldepartment;
        }
        public IEnumerable<Branches> GetAllBranches()
        {
            var lbranch = db.Branches.ToList();
            return lbranch;
        }
        public IEnumerable<Roles> GetAllRoles()
        {
            var lroles = db.Roles.ToList();
            return lroles;
        }
        public JsonResult Checkdept(string Name)
        {
            var dept = from ba in db.Departments where ba.Name.Equals(Name) select ba;
            {
                int count = dept.Count();


                if (count == 0)
                {
                    return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult Checkdeptcode(string Code)
        {
            var dept = from ba in db.Departments where ba.Code.Equals(Code) select ba;
            {
                int count = dept.Count();


                if (count == 0)
                {
                    return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ViewModel d)
        {
            LogInformation.Info("Department Create Code started ");
            try
            {
                if (ModelState.IsValid)
                {
                    LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    d.ldepartments.UpdatedBy = lCredentials.EmpId;
                    d.ldepartments.Active = 1;
                    d.ldepartments.UpdatedDate =GetCurrentTime(DateTime.Now);
                    db.Departments.Add(d.ldepartments);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Department Created Successfully";
                    return RedirectToAction("Create");
                }
                else
                {
                    LogInformation.Info("Department Create Code ended ");
                    return View(d);
                }
            }
            catch(Exception ex)
            {
                LogInformation.Error("Departments, Create.Exception: " + ex);
            }
            return View(d);
        }

        [NoDirectAccess]
        public ActionResult DepartmentView()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }
        [HttpGet]
        public JsonResult DepartmentViews(string Name)
        {

            var dbResult = db.Departments.ToList();
            if (string.IsNullOrEmpty(Name))
            {
                var department = (from departmentlist in dbResult
                                  where departmentlist.Code != "OtherDepartment" && departmentlist.Active == 1
                                  select new
                                  {
                                      departmentlist.Id,
                                      departmentlist.Code,
                                      departmentlist.Name,
                                      departmentlist.Description,


                                  });
                return Json(department, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var department = (from departmentlist in dbResult
                                  where (departmentlist.Name.ToLower().Contains(Name.ToLower())
                                  &&(departmentlist.Code!= "OtherDepartment")&& departmentlist.Active==1)
                                  select new
                                  {
                                      departmentlist.Id,
                                      departmentlist.Code,
                                      departmentlist.Name,
                                      departmentlist.Description,
                                  });
                return Json(department, JsonRequestBehavior.AllowGet);
            }

        }
        
        // GET: Departments/Edit/5
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        public ActionResult Edit(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            LogInformation.Info("Department Edit Code started ");
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Departments departments = db.Departments.Find(id);
                if (departments == null)
                {
                    return HttpNotFound();
                }
                LogInformation.Info("Department Edit Code ended ");
                return View(departments);
            }
            catch(Exception ex)
            {
                LogInformation.Error("Departments, Create.Exception: " + ex);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Description,UpdatedBy,UpdatedDate,Active")] Departments departments)
        {
            if (ModelState.IsValid)
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                departments.UpdatedBy = lCredentials.EmpId;
         
                departments.UpdatedDate = GetCurrentTime(DateTime.Now);
                departments.Active = 1;
                db.Entry(departments).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Department Updated Successfully";
                return RedirectToAction("Create");
            }
            return View(departments);
        }

        // GET: Departments/Delete/5
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        public ActionResult Delete(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            LogInformation.Info("Department Delete Code started ");
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                int deptid = db.Employes.Where(a => a.Department == id).Select(a => a.Department).Distinct().FirstOrDefault();

                Departments dept = db.Departments.Find(id);
                if (deptid == id)
                {
                    TempData["AlertMessage"] = "This Department has Employees and cannot be deleted.";
                    return RedirectToAction("Create");
                }

                else
                {
                    dept.UpdatedBy = lCredentials.EmpId;
                    dept.UpdatedDate = GetCurrentTime(DateTime.Now);
                    db.Departments.Remove(dept);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Department Details deleted Successfully.";

                    LogInformation.Info("Department Delete Code Ended ");
                    return RedirectToAction("Create");
                }
            }
            catch(Exception ex)
            {
                LogInformation.Error("Departments, Delete.Exception: " + ex);
            }
            return RedirectToAction("Create");
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
    }
}
   