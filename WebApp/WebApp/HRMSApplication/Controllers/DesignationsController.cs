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
using HRMSBusiness.Business;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class DesignationsController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        [HttpGet]
        public ActionResult Create()
        {
           
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
            Designations ldesignations = new Designations();
            ldesignations.LoginMode = lCredentials.LoginMode;
            ViewBag.Message = lCredentials.LoginMode;
            lmodel.ldesignations = ldesignations;
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
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/Designations/_DesignationPartialView.cshtml", lmodel);
        }
        public JsonResult Checkdesig(string Name)
        {
            var desig = from ba in db.Designations where ba.Name.Equals(Name) select ba;
            {
                int count = desig.Count();


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

        public JsonResult Checkdesigcode(string Code)
        {
            var desig = from ba in db.Designations where ba.Code.Equals(Code) select ba;
            {
                int count = desig.Count();


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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ViewModel d,FormCollection form)
        {
            if (ModelState.IsValid)
            {
              
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                d.ldesignations.UpdatedBy = lCredentials.EmpId;
                d.ldesignations.UpdatedDate = DateTime.Now;
                db.Designations.Add(d.ldesignations);
                db.SaveChanges();
                Branch_Designation_Mapping brdesi = new Branch_Designation_Mapping();

                brdesi.DesignationId = d.ldesignations.Id;
                brdesi.BranchId = 43;
                db.Branch_Designation_Mapping.Add(brdesi);
                db.SaveChanges();

                brdesi.DesignationId = d.ldesignations.Id;
                brdesi.BranchId = 42;
                db.Branch_Designation_Mapping.Add(brdesi);
                db.SaveChanges();
                TempData["AlertMessage"] = "Designation Created Successfully";
                return RedirectToAction("Create");
            }
            else
            {
                return View(d);
            }
        }
        [NoDirectAccess]
        public ActionResult DesignationView()
        {
            return View();
        }
        [HttpGet]
        public JsonResult DesignationViews(string Name)
        {

            var dbResult = db.Designations.ToList();
            if (string.IsNullOrEmpty(Name))
            {
                var designation = (from designationlist in dbResult

                            select new
                            {
                                designationlist.Id,
                                designationlist.Code,
                                designationlist.Name,  
                                designationlist.Description,
                            });
                return Json(designation, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var designation = (from designationlist in dbResult
                            where designationlist.Name.ToLower().Contains(Name.ToLower())
                            select new
                            {
                                designationlist.Id,
                                designationlist.Code,
                                designationlist.Name,
                                designationlist.Description,
                            });
                return Json(designation, JsonRequestBehavior.AllowGet);
            }

        }

        [NoDirectAccess]
        [SessionTimeoutAttribute]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Designations designations = db.Designations.Find(id);
            if (designations == null)
            {
                return HttpNotFound();
            }
            return View(designations);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Designations designations)
        {
            if (ModelState.IsValid)
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                designations.UpdatedBy = lCredentials.EmpId;
                designations.UpdatedDate = DateTime.Now;
                db.Entry(designations).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Designation Updated Successfully";
                return RedirectToAction("Create");
            }
            return View(designations);
        }

        [NoDirectAccess]
        [SessionTimeoutAttribute]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int desigid = db.Employes.Where(a => a.CurrentDesignation == id).Select(a => a.CurrentDesignation).Distinct().FirstOrDefault();
            Designations desig = db.Designations.Find(id);
            if (desigid == id)
            {
                TempData["AlertMessage"] = "This Designation has Employees and cannot be deleted.";
                return RedirectToAction("Create");
            }
            else
            {


                WorkDiaryBus wdbus = new WorkDiaryBus();
                //desig.UpdatedBy = lCredentials.EmpId;
                //desig.UpdatedDate = DateTime.Now;
                //db.Designations.Remove(desig);
                //db.SaveChanges();
                var dtwd = wdbus.deletedesignation(lCredentials.EmpId, (int)id);
               
                TempData["AlertMessage"] = "Designation Details deleted Successfully.";
                return RedirectToAction("Create");
            }
        }       
    }
}