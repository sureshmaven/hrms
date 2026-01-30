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
    public class LeaveTypesController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        // GET: LeaveTypes/Create
        [NoDirectAccess]
        [HttpGet]
        [SessionTimeoutAttribute]
        public ActionResult Create()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items3 = Facade.EntitiesFacade.GetAllBanks().Select(x => new Banks
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Banks = new SelectList(items3, "Name", "Name");

            var items4 = Facade.EntitiesFacade.GetAllBranches().Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Branches = new SelectList(items4, "Name", "Name");
            ViewModel lmodel = new ViewModel();
            Employees lemployees = new Employees();
            LeaveTypes lleavetypes = new LeaveTypes();
            lleavetypes.LoginMode = lCredentials.LoginMode;
            ViewBag.Message = lCredentials.LoginMode;
            lmodel.lleavetypes = lleavetypes;
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

            var items6 = Facade.EntitiesFacade.GetAllDepartments().Select(x => new Departments
            {

                Name = x.Name
            });
            ViewBag.Department = new SelectList(GetDepartments(), "Name", "Name");

            var items7 = Facade.EntitiesFacade.GetAllRoles().Select(x => new Roles
            {

                Name = x.Name
            });
            ViewBag.Role = new SelectList(items4, "Name", "Name");
            var items5 = Facade.EntitiesFacade.GetAllBranches().Select(x => new Branches
            {
                Name = x.Name
            });
            ViewBag.Branch = new SelectList(items5, "Name", "Name");
            lmodel.lEmployess = data;
            Employees edata = (from u in db.Employes where u.Id == id select u).FirstOrDefault();
            return View("~/Views/LeaveTypes/_LeaveTypePartialView.cshtml", lmodel);
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

        public JsonResult CheckLT(string Type)
        {
            var lt = from ba in db.LeaveTypes where ba.Type.Equals(Type) select ba;
          
            {
                int count = lt.Count();
               

                if (count == 0 )
                {
                    return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public JsonResult CheckLTcode( string Code)
        {
            var lt = from ba in db.LeaveTypes where ba.Code.Equals(Code) select ba;

            {
                int count = lt.Count();


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
        public ActionResult Create(ViewModel LT)
        {
            if (ModelState.IsValid)
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                LT.lleavetypes.UpdatedBy = lCredentials.EmpId;
                LT.lleavetypes.UpdatedDate = DateTime.Now;
                db.LeaveTypes.Add(LT.lleavetypes);
                db.SaveChanges();
                TempData["AlertMessage"] = "LeaveTypes Created Successfully";
                return RedirectToAction("Create");
            }
            else
            {
                return View(LT);
            }
        }

        [NoDirectAccess]
        public ActionResult LeaveTypeView()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }
        [HttpGet]
        public JsonResult LeaveTypeViews(string Type)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var dbResult = db.LeaveTypes.ToList();
            if (string.IsNullOrEmpty(Type))
            {
                var LeaveType = (from LeaveTypelist in dbResult

                                 select new
                                 {
                                     LeaveTypelist.Id,
                                     LeaveTypelist.Type,
                                     LeaveTypelist.Code,
                                     LeaveTypelist.Description,


                                 });
                return Json(LeaveType, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var LeaveType = (from LeaveTypeslist in dbResult
                                 where LeaveTypeslist.Type.ToLower().Contains(Type.ToLower())
                                 select new
                                 {
                                     LeaveTypeslist.Id,
                                     LeaveTypeslist.Type,
                                     LeaveTypeslist.Code,
                                     LeaveTypeslist.Description,
                                 });
                return Json(LeaveType, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpGet]
        public ActionResult OtherdutyCreate()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }


        public JsonResult Typecheck(string type)
        {

            var odtype = from od in db.OD_Master where od.ODType.Equals(type) select od;
            int count = odtype.Count();


            if (count == 0)
            {
                return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult OtherdutyCreates(OD_Master type)
        {
            if (ModelState.IsValid)
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                type.UpdatedBy = lCredentials.EmpId;
                type.UpdatedDate = DateTime.Now;
                type.Status = true;
                db.OD_Master.Add(type);
                db.SaveChanges();
                TempData["AlertMessage"] = "OD Type Created Successfully";
                return RedirectToAction("OtherdutyCreate");
            }
            else
            {
                return View(type);
            }
        }

        [HttpGet]
        public JsonResult ODView(string type)
        {
           TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var ODType = db.OD_Master.ToList();
            if (string.IsNullOrEmpty(type))
            {
                var OtherDuty = (from odmaster in ODType

                             select new
                             {
                                 odmaster.Id,
                                 odmaster.ODType,
                             });
                return Json(OtherDuty, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var OtherDuty = (from odmaster in ODType

                             select new
                             {
                                 odmaster.Id,
                                 odmaster.ODType,
                             });
                return Json(OtherDuty, JsonRequestBehavior.AllowGet);
            }
               
            }
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        public ActionResult ODEdit(int? id)
        {
            LoginCredential lCredentails = LoginHelper.GetCurrentUser();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OD_Master otherduty = db.OD_Master.Find(id);
            if (otherduty == null)
            {
                return HttpNotFound();
            }
            return View(otherduty);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ODEdit(OD_Master type)
        {
            if (ModelState.IsValid)
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                type.UpdatedBy = lCredentials.EmpId;
                type.UpdatedDate = DateTime.Now;
                db.Entry(type).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "OD Type Updated Successfully";
                return RedirectToAction("OtherdutyCreate");
            }
            return View(type);
        }

        // GET: Banks/Delete/5
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        public ActionResult ODDelete(int? id)
        {
            OD_Master otherduty = db.OD_Master.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }    
            else
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                otherduty.UpdatedBy = lCredentials.EmpId;
                otherduty.UpdatedDate = DateTime.Now;
                db.OD_Master.Remove(otherduty);
                db.SaveChanges();
                TempData["AlertMessage"] = "OD Type Details deleted Successfully.";
                return RedirectToAction("OtherdutyCreate");
            }


        }
    }

    }    
        

    
