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
    public class RolesController : Controller
    {
        private ContextBase db = new ContextBase();
        [NoDirectAccess]
        [HttpGet]
        [SessionTimeoutAttribute]
        public ActionResult Create()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var items3 = Facade.EntitiesFacade.GetAllBanks().Select(x => new Banks
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Banks = new SelectList(items3, "Name", "Name");

            var items4 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Branches = new SelectList(items4, "Name", "Name");
            ViewModel lmodel = new ViewModel();
            Employees lemployees = new Employees();
            Roles lroles = new Roles();
            lroles.LoginMode = lCredentials.LoginMode;
            ViewBag.Message = lCredentials.LoginMode;
            ViewBag.Message = lCredentials.LoginMode;
            lmodel.lroles = lroles;
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
            var items5 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF").Select(x => new Branches
            {
                Name = x.Name
            });
            ViewBag.Branch = new SelectList(items5, "Name", "Name");
            lmodel.lEmployess = data;
            Employees edata = (from u in db.Employes where u.Id == id select u).FirstOrDefault();
            return View("~/Views/Roles/_RolePartialView.cshtml", lmodel);
        }
        [NoDirectAccess]
        [HttpGet]
        [SessionTimeoutAttribute]
        public ActionResult Userrole()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var items3 = Facade.EntitiesFacade.GetAllBanks().Select(x => new Banks
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Banks = new SelectList(items3, "Name", "Name");

            var items4 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Branches = new SelectList(items4, "Name", "Name");
            ViewModel lmodel = new ViewModel();
            Employees lemployees = new Employees();
            Roles lroles = new Roles();
            lroles.LoginMode = lCredentials.LoginMode;
            ViewBag.Message = lCredentials.LoginMode;
            ViewBag.Message = lCredentials.LoginMode;
            lmodel.lroles = lroles;
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
            var items5 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF").Select(x => new Branches
            {
                Name = x.Name
            });
            ViewBag.Branch = new SelectList(items5, "Name", "Name");
            lmodel.lEmployess = data;
            Employees edata = (from u in db.Employes where u.Id == id select u).FirstOrDefault();
            return View("~/Views/Roles/UserRoles.cshtml", lmodel);
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


        public JsonResult Checkrole(string Name)
        {
            var role = from ba in db.Roles where ba.Name.Equals(Name) select ba;
            {
                int count = role.Count();


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
        public ActionResult Create(ViewModel r)
        {

            LogInformation.Info("Role Create Code started ");
            try
            {

                if (ModelState.IsValid)
                {
                    LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    r.lroles.UpdatedBy = lCredentials.EmpId;
                    r.lroles.UpdatedDate = DateTime.Now;
                    db.Roles.Add(r.lroles);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Role Created Successfully";
                    return RedirectToAction("Create");
                }
                else
                {
                    LogInformation.Info("Role Create Code ended ");
                    return View(r);
                }
              
            }
            catch(Exception ex)
            {
                LogInformation.Error("Roles, Create.Exception: " + ex);
            }
            return View(r);

        }

        [NoDirectAccess]
        public ActionResult RoleView()
        {
            return View();
        }
        [HttpGet]
        public JsonResult RoleViews(string Name)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var dbResult = db.Roles.ToList();
            if (string.IsNullOrEmpty(Name))
            {
                var role = (from rolelist in dbResult

                            select new
                            {
                                rolelist.Id,
                                rolelist.Name,
                                rolelist.Description,


                            });
                return Json(role, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var role = (from rolelist in dbResult
                            where rolelist.Name.ToLower().Contains(Name.ToLower())
                            select new
                            {
                                rolelist.Id,
                                rolelist.Name,
                                rolelist.Description,
                            });
                return Json(role, JsonRequestBehavior.AllowGet);
            }

        }
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Roles roles)
        {
            LogInformation.Info("Role Edit Code started ");
            try
            {
                if (ModelState.IsValid)
                {
                    LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    roles.UpdatedBy = lCredentials.EmpId;
                    roles.UpdatedDate = DateTime.Now;
                    db.Entry(roles).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Role Updated Successfully";
                    return RedirectToAction("Create");
                }
                LogInformation.Info("Role Edit Code ended ");
                return View(roles);
            }
            catch(Exception ex)
            {
                LogInformation.Error("Roles, Edit.Exception: " + ex);
            }
           
            return View(roles);
        }

        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            LogInformation.Info("Role Delete Code started ");
            try
            {


                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                int roleid = db.Employes.Where(a => a.Role == id).Select(a => a.Role).Distinct().FirstOrDefault();
                Roles role = db.Roles.Find(id);
                if (roleid == id)
                {
                    TempData["AlertMessage"] = "This Role has Employees and cannot be deleted.";
                    return RedirectToAction("Create");
                }
                else
                {
                    role.UpdatedBy = lCredentials.EmpId;
                    role.UpdatedDate = DateTime.Now;
                    db.Roles.Remove(role);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Role Details deleted Successfully.";
                    LogInformation.Info("Role Delete Code Ended ");
                    return RedirectToAction("Create");
                }

            }

            catch(Exception ex)
            {
                LogInformation.Error("Roles, Delete.Exception: " + ex);
            }
           
            return RedirectToAction("Create");
        }
    }
}





