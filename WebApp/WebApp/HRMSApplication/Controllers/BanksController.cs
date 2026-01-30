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
    public class BanksController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        // GET: Banks/Create
        [NoDirectAccess]
        [SessionTimeoutAttribute]
     
        public ActionResult Create()
        {
          
            var items3 = Facade.EntitiesFacade.GetAllBanks().Select(x => new Banks
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Banks = new SelectList(items3, "Name", "Name");

            var items4 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            ViewBag.Branches = new SelectList(items4, "Name", "Name");
            ViewModel lmodel = new ViewModel();
            Employees lemployees = new Employees();
            Banks lbanks = new Banks();
            lbanks.LoginMode = lCredentials.LoginMode;
            ViewBag.Message = lCredentials.LoginMode;
            lmodel.lbanks = lbanks;
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
            var items5 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
            {
                Name = x.Name
            });
            ViewBag.Branch = new SelectList(items5, "Name", "Name");
            lmodel.lEmployess = data;
            Employees edata = (from u in db.Employes where u.Id == id select u).FirstOrDefault();
            return View("~/Views/Banks/_BankPartails.cshtml", lmodel);
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

        public JsonResult bankcheck(string Name)
        {

            var Bankname = from ba in db.Banks where ba.Name.Equals(Name) select ba;
            int count = Bankname.Count();


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
        public ActionResult Create(ViewModel Bank)
        {
            if (ModelState.IsValid)
            {
               
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                Bank.lbanks.UpdatedBy = lCredentials.EmpId;
                Bank.lbanks.UpdatedDate = DateTime.Now;
                db.Banks.Add(Bank.lbanks);
                db.SaveChanges();
                TempData["AlertMessage"] = "Bank Created Successfully";
                return RedirectToAction("Create");
            }
            else
            {
                return View(Bank);
            }
        }
        [NoDirectAccess]
        public ActionResult BankView()
        {
            return View();
        }
        [HttpGet]
        public JsonResult BankViews(string Name)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var dbResult = db.Banks.ToList();
            if (string.IsNullOrEmpty(Name))
            {
                var banks = (from bankslist in dbResult

                             select new
                             {
                                 bankslist.Id,
                                 bankslist.Name,
                                 bankslist.AddressLine1,
                                 bankslist.AddressLine2,
                                 bankslist.City,
                                 bankslist.PhoneNo1,
                                 bankslist.PhoneNo2,
                                 bankslist.PhoneNo3,


                             });
                return Json(banks, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var banks = (from bankslist in dbResult
                             where bankslist.Name.ToLower().Contains(Name.ToLower())
                             select new
                             {
                                 bankslist.Id,
                                 bankslist.Name,
                                 bankslist.AddressLine1,
                                 bankslist.AddressLine2,
                                 bankslist.City,
                                 bankslist.PhoneNo1,
                                 bankslist.PhoneNo2,
                                 bankslist.PhoneNo3,
                             });
                return Json(banks, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: Banks/Edit/5
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        public ActionResult Edit(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banks banks = db.Banks.Find(id);
            if (banks == null)
            {
                return HttpNotFound();
            }
            return View(banks);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,Name,AddressLine1,AddressLine2,City,PhoneNo1,PhoneNo2,PhoneNo3,UpdatedBy,UpdatedDate")] Banks banks)
        {
            if (ModelState.IsValid)
            {
                
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                banks.UpdatedBy = lCredentials.EmpId;
               banks.UpdatedDate = DateTime.Now;
                db.Entry(banks).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "Bank Updated Successfully";
                return RedirectToAction("Create");
            }
            return View(banks);
        }

        // GET: Banks/Delete/5
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        public ActionResult Delete(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            int bankid = db.Branches.Where(a => a.BankName == id).Select(a => a.BankName).Distinct().FirstOrDefault();
            Banks bank = db.Banks.Find(id);
            if (bankid == id)
            {
                TempData["AlertMessage"] = "This Bank has Branches and cannot be deleted.";
                return RedirectToAction("Create");
            }

            else
            {
               
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                bank.UpdatedBy = lCredentials.EmpId;
                bank.UpdatedDate = DateTime.Now;
                db.Banks.Remove(bank);
                db.SaveChanges();
                TempData["AlertMessage"] = "Bank Details deleted Successfully.";
                return RedirectToAction("Create");
            }


        }
    }
}
    
