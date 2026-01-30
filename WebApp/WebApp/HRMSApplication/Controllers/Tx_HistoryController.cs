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
    public class Tx_HistoryController : Controller
    {
        private ContextBase db = new ContextBase();

        // GET: Tx_History
        [NoDirectAccess]
        [SessionTimeoutAttribute]
        [HttpGet]
        public ActionResult Index()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return PartialView("~/Views/Tx_History/_TxHistoryPartial.cshtml");
        }

     
        public ActionResult HistoryView()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View();
        }
        [HttpGet]
        public JsonResult HistoryViews(string type)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            LoginCredential lCredentials = LoginHelper.GetCurrentUser();
            var dbResult = db.V_Tx_History.ToList();            
            if (string.IsNullOrEmpty(type))
                {
                    var history = (from historylist in dbResult

                                   select new
                                   {
                                       historylist.Id,
                                      
                                       historylist.Tx_type,
                                       historylist.Tx_subtype,
                                       historylist.Tx_by,
                                       historylist.Tx_on,                                  
                                       historylist.Tx_date,
                                       historylist.Notes,
                                       historylist.Comments,


                                   }).OrderByDescending( A => A.Tx_date);
                var jsonResult =  Json(history, JsonRequestBehavior.AllowGet);            
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
                else
                {
                    var history = (from historylist in dbResult
                                   where historylist.Tx_type.ToLower().Contains(type.ToLower())
                                   select new
                                   {
                                       historylist.Id,
                                  
                                       historylist.Tx_type,
                                       historylist.Tx_subtype,
                                       historylist.Tx_by,
                                       historylist.Tx_on,                                   
                                       historylist.Tx_date,
                                       historylist.Notes,
                                       historylist.Comments,
                                   }).OrderByDescending(A => A.Tx_date);
                var jsonResult = Json(history, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }

         

        }


    }
}