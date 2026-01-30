using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Entities;
using HRMSApplication.Helpers;
using HRMSApplication.Models;
using Repository;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class BlockPeriodController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        //Create
        [HttpGet]
        public ActionResult Create()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/BlockPeriod/Create.cshtml");
        }
        [HttpPost]
        public ActionResult Create(BlockPeriod block)
        {
            if (ModelState.IsValid)
            {

               
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                block.UpdatedBy = lCredentials.EmpId;
                block.Block_Period = block.StartYear + "-" + block.EndYear;
                block.UpdatedDate = DateTime.Now;
                db.BlockPeriod.Add(block);
                db.SaveChanges();
                TempData["AlertMessage"] = "BlockPeriod Created Successfully";
                return RedirectToAction("Create");
            }
            return View(block);
        }
        [HttpGet]
        public JsonResult BlockPeriodView(string EmpId)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            var dbResult = db.BlockPeriod.ToList();
            if (string.IsNullOrEmpty(EmpId))
            {
                var block = (from blockperiod in dbResult
                             select new
                             {
                                 blockperiod.Id,
                                 blockperiod.StartYear,
                                 blockperiod.EndYear,
                                 blockperiod.Block_Period,
                                 blockperiod.UpdatedBy,
                                 blockperiod.UpdatedDate
                             });
                return Json(block, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var block = (from blockperiod in dbResult
                             select new
                             {
                                 blockperiod.Id,
                                 blockperiod.StartYear,
                                 blockperiod.EndYear,
                                 blockperiod.Block_Period,
                                 blockperiod.UpdatedBy,
                                 blockperiod.UpdatedDate
                             });
                return Json(block, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult checkYearExistOrNot(string year)
        {

            var empids = from u in db.BlockPeriod where u.StartYear.Equals(year) select u;
            int count = empids.Count();

            if (count == 0)
            {


                return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);


            }
            else
            {

                return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);
            }
        }
        //Edit
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            BlockPeriod block = db.BlockPeriod.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string blockid = db.Leaves_LTC.Where(a => a.Block_Period == id.ToString()).Select(a => a.Block_Period).Distinct().FirstOrDefault();
            int blockperiodid = Convert.ToInt32(blockid);

            if (blockperiodid == id)
            {
                TempData["AlertMessage"] = "This BlockPeriod has Records and cannot be Edited.";
                return RedirectToAction("Create");
            }
            return View(block);
        }
        [HttpPost]
        public ActionResult Edit(BlockPeriod block)
        {
            if(ModelState.IsValid)
            {
             
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                block.UpdatedBy = lCredentials.EmpId;
                block.UpdatedDate = DateTime.Now;
                block.Block_Period = block.StartYear + "-" + block.EndYear;
                db.Entry(block).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "BlockPeriod Updated Successfully";
                return RedirectToAction("Create");
            }
            return View(block);
        }
        //Delete
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            BlockPeriod block = db.BlockPeriod.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string blockid = db.Leaves_LTC.Where(a => a.Block_Period == id.ToString()).Select(a => a.Block_Period).Distinct().FirstOrDefault();
            int blockperiodid = Convert.ToInt32(blockid);

            if (blockperiodid == id)
            {
                TempData["AlertMessage"] = "This BlockPeriod has Records and cannot be deleted.";
                return RedirectToAction("Create");
            }

            else
            {
                LoginCredential lCredentials = LoginHelper.GetCurrentUser();
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                block.UpdatedBy = lCredentials.EmpId;
                block.UpdatedDate = DateTime.Now;
                block.Block_Period = block.StartYear + "-" + block.EndYear;
                db.BlockPeriod.Remove(block);
                db.SaveChanges();
                TempData["AlertMessage"] = "Block Period Details deleted Successfully.";
                return RedirectToAction("Create");
            }
        }
        public JsonResult block(string year1 , string year2)
        {
            var blockperiod = year1 + "-" + year2;
            var bp = from ba in db.BlockPeriod where ba.Block_Period.Equals(blockperiod) select ba;
            int count = bp.Count();
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
