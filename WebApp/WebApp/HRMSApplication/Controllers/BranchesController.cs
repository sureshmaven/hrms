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
    public class BranchesController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        // GET: Departments/Create
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        [HttpGet]
        public ActionResult Create()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items3 = Facade.EntitiesFacade.GetAllBanks().Select(x => new Banks
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Banks = new SelectList(items3, "Id", "Name");

            var items4 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Branches = new SelectList(items4, "Id", "Name");
            ViewModel lmodel = new ViewModel();
            Employees lemployees = new Employees();
            Branches lbranches = new Branches();
            lbranches.LoginMode = lCredentials.LoginMode;
            ViewBag.Message = lCredentials.LoginMode;
            lmodel.lbranches = lbranches;
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
                Id = x.Id,
                Name = x.Name
            });
            ViewBag.Role = new SelectList(items4, "Name", "Name");
            var items5 = Facade.EntitiesFacade.GetAllBranches().Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            });
            ViewBag.Branch = new SelectList(items5, "Id", "Name");
            lmodel.lEmployess = data;
            Employees edata = (from u in db.Employes where u.Id == id select u).FirstOrDefault();
            return View("~/Views/Branches/_BranchPartailView.cshtml", lmodel);
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
        public JsonResult CheckBranch(string Name)
        {

            var queryAllCustomers = from br in db.Branches where br.Name.Equals(Name) select br;
            int count = queryAllCustomers.Count();


            if (count == 0)
            {
                return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult branchcode(string Code)
        {             
            var queryAllCustomers = from br in db.Branches where br.BranchCode.Equals(Code) where br.BranchCode != "" select br;
            int count = queryAllCustomers.Count();


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
        public ActionResult Create(ViewModel br)
        {
            if (ModelState.IsValid)
            {

                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                br.lbranches.UpdatedBy = lCredentials.EmpId;
                br.lbranches.UpdatedDate = DateTime.Now;
                db.Branches.Add(br.lbranches);
                db.SaveChanges();
                TempData["AlertMessage"] = "Branch Created Successfully";
                return RedirectToAction("Create");
            }
            else
            {
                db.Branches.Add(br.lbranches);
                db.SaveChanges();
                return RedirectToAction("Create");
            }
        }
        [NoDirectAccess]
        public ActionResult BranchView()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }
        [HttpGet]
        public JsonResult BranchViews(string Name)
        {
            var Banks = db.Banks.ToList();
            var dbResult = db.Branches.ToList();
            if (string.IsNullOrEmpty(Name))
            {
                var branch = (from bank in Banks
                              join branchlist in dbResult on bank.Id equals branchlist.BankName
                              where (branchlist.IFSCCode != "TSHEADOFF" && branchlist.Name != "TGCAB-CTI")
                              select new
                              {

                                  branchlist.Id,
                                  bank.Name,
                                  branchN = branchlist.Name,
                                  branchlist.BankName,
                                  branchlist.BranchCode,
                                  branchlist.AddressLine1,
                                  branchlist.AddressLine2,
                                  branchlist.City,
                                  branchlist.PhoneNo1,
                                  branchlist.PhoneNo2,
                                  branchlist.PhoneNo3,

                              });
                return Json(branch, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var branch = (from branchlist in dbResult
                              where (branchlist.Name.ToLower().Contains(Name.ToLower()) && (branchlist.IFSCCode != "TSHEADOFF" && branchlist.Name != "TGCAB-CTI"))
                              select new
                              {
                                  branchlist.Id,
                                  branchlist.BankName,
                                  branchlist.Name,
                                  branchlist.BranchCode,
                                  branchlist.AddressLine1,
                                  branchlist.AddressLine2,
                                  branchlist.City,
                                  branchlist.PhoneNo1,
                                  branchlist.PhoneNo2,
                                  branchlist.PhoneNo3,

                              });
                return Json(branch, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: Branches/Edit/5
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        public ActionResult Edit(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items3 = Facade.EntitiesFacade.GetAllBanks().Select(x => new Banks
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Banks = new SelectList(items3, "Id", "Name");

            var items4 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();

            ViewBag.Branches = new SelectList(items4, "Name", "Name");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branches branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);

        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Branches branches)
        {
            try
            {


                if (ModelState.IsValid)
                {

                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    branches.UpdatedBy = lCredentials.EmpId;
                    branches.UpdatedDate = DateTime.Now;
                    db.Entry(branches).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Branch Updated Successfully";
                    return RedirectToAction("Create");
                }
                return View(branches);

            }
            catch (Exception e)
            {
                e.ToString();
            }
            return View(branches);
        }

        // GET: Branches/Delete/5
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        public ActionResult Delete(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            int lbrachesId = db.Employes.Where(a => a.Branch == id).Select(a => a.Branch).Distinct().FirstOrDefault();

            Branches br = db.Branches.Find(id);
            if (lbrachesId == id)
            {
                TempData["AlertMessage"] = "This Branch has Employees and cannot be deleted.";
                return RedirectToAction("Create");
            }

            else
            {
                br.UpdatedBy = lCredentials.EmpId;
                br.UpdatedDate = DateTime.Now;
                db.Branches.Remove(br);
                db.SaveChanges();
                TempData["AlertMessage"] = "Branch Details deleted Successfully.";
                return RedirectToAction("Create");
            }
        }



        //Branch Count Create
        [HttpGet]
        public ActionResult Createbranchcount()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();
            ViewBag.Name = new SelectList(items, "Id", "Name");
            return View("Createbranchcount");
        }

        //Branch Count Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Createbranchcount(BranchStaffCount branchcount)
        {


            int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
            branchcount.UpdatedBy = lCredentials.EmpId;
            branchcount.UpdatedDate = DateTime.Now;
            db.BranchStaffCount.Add(branchcount);
            db.SaveChanges();
            TempData["AlertMessage"] = "Branch Created Successfully";
            return RedirectToAction("Createbranchcount", "Branches");
        }




        //Branch Count Grid

        public ActionResult branchcountview()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }

        //Branch Count Grid
        [HttpGet]
        public JsonResult branchcountviews(string Name)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var branchcount = db.BranchStaffCount.ToList();
            var dbResult = db.Branches.ToList();

            if (string.IsNullOrEmpty(Name))
            {
                var branch = (from brlist in branchcount
                              join branchlist in dbResult on brlist.Branchid equals branchlist.Id
                              where (branchlist.Name != "TGCAB-CTI")                       
                              select new
                              {
                                  brlist.Id,
                                  branchlist.Name,
                                  brlist.AmountRange,
                                  brlist.Category,
                                  brlist.Staffcount

                              });
                return Json(branch, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var branch = (from brlist in branchcount
                              join branchlist in dbResult on brlist.Branchid equals branchlist.Id
                              //where (branchlist.IFSCCode != "TSHEADOFF")                       
                              select new
                              {
                                  brlist.Id,
                                  branchlist.Name,
                                  brlist.AmountRange,
                                  brlist.Category,
                                  brlist.Staffcount

                              });
                return Json(branch, JsonRequestBehavior.AllowGet);
            }

        }


        //Branch Count Edit
        [HttpGet]
        public ActionResult Editbranchcount(int? id)
        {

            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var items = Facade.EntitiesFacade.GetAllBranches().Select(x => new Branches
            {
                Id = x.Id,
                Name = x.Name
            }).Distinct();
            ViewBag.Name = new SelectList(items, "Id", "Name");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BranchStaffCount banks = db.BranchStaffCount.Find(id);
            if (banks == null)
            {
                return HttpNotFound();
            }
            return View(banks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Editbranchcount(BranchStaffCount branches)
        {
            try
            {


                if (ModelState.IsValid)
                {

                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    branches.UpdatedBy = lCredentials.EmpId;
                    branches.UpdatedDate = DateTime.Now;
                    db.Entry(branches).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Details Updated Successfully";
                    return RedirectToAction("Createbranchcount");
                }
                return View(branches);

            }
            catch (Exception e)
            {
                e.ToString();
            }
            return View(branches);
        }

        // Branch Count Delete
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        public ActionResult Deletebranchcount(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                BranchStaffCount br = db.BranchStaffCount.Find(id);
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                br.UpdatedDate = DateTime.Now;
                db.BranchStaffCount.Remove(br);
                db.SaveChanges();
                TempData["AlertMessage"] = "Details deleted Successfully.";
                return RedirectToAction("Createbranchcount");
            }
        }

        public JsonResult BranchName(int Name)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var name = from br in db.BranchStaffCount where br.Branchid.Equals(Name) select br;
            int count = name.Count();


            if (count == 0)
            {
                return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EditBranchName(int Name)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var name = from br in db.BranchStaffCount where br.Branchid.Equals(Name) select br;
            int count = name.Count();


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
}

