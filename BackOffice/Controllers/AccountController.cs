using BackOffice.Models;
using BL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class AccountController : Controller
    {
        private readonly IBackofficeAdminServices _adminServices;
        public AccountController(IBackofficeAdminServices adminServices)
        {
            this._adminServices = adminServices;
        }

        // GET: Account
        public ActionResult Index()
        {
            if (Session["admin"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            AccountModel model = new AccountModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(AccountModel model)
        {
            var admin = _adminServices.GetByLogin(model.username, model.password);
            if (admin == null)
            {
                ViewBag.ErrorMessage = "Invalid account!";
            }
            else
            {
                admin.LastLogin = DateTime.Now;
                _adminServices.SaveOrUpdate(admin);
                Session["admin"] = admin;
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Account");
        }

       
    }
}