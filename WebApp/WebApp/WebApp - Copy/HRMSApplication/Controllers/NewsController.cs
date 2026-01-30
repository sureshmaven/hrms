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
    public class NewsController : Controller
    {
        private ContextBase db = new ContextBase();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();
        public ActionResult Create()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/News/Create.cshtml");
        }
        [HttpPost]
        public ActionResult Create(News news)
        {
            if (ModelState.IsValid)
            {
              
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                news.UpdatedBy = lCredentials.EmpId;
                news.UpdatedDate = DateTime.Now;
                db.News.Add(news);
                db.SaveChanges();
                TempData["AlertMessage"] = "News Created Successfully";
                return RedirectToAction("Create");
            }
            return View(news);
        }
        public ActionResult NotesView(string EmpId)
        {
            var dbresult = db.News.ToList();
            if (string.IsNullOrEmpty(EmpId))
            {
                var data = (from lnews in dbresult
                            select new
                            {
                                lnews.Id,
                                lnews.Content,
                                lnews.Subject,     
                                lnews.UpdatedBy,
                                lnews.UpdatedDate,
                            });
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = (from lnews in dbresult
                            select new
                            {
                                lnews.Id,
                                lnews.Content,
                                lnews.Subject, 
                                lnews.UpdatedBy,
                                lnews.UpdatedDate,
                            });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Edit(int? id)
        {
            News news = db.News.Find(id);
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(news);
        }
        [HttpPost]
        public ActionResult Edit(News news)
        {
            if (ModelState.IsValid)
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                news.UpdatedBy = lCredentials.EmpId;
                news.UpdatedDate = DateTime.Now;
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = "News Updated Successfully";
                return RedirectToAction("Create");
            }
            return View(news);
        }
        public ActionResult Delete(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            News news = db.News.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                news.UpdatedBy = lCredentials.EmpId;
                news.UpdatedDate = DateTime.Now;
                db.News.Remove(news);
                db.SaveChanges();
                TempData["AlertMessage"] = "News Details deleted Successfully.";
                return RedirectToAction("Create");
            }
        }
    }
}
