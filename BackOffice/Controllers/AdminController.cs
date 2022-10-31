using BL;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class AdminController : BaseController
    {
        // GET: Admin
        private readonly IBackofficeAdminServices _adminServices;
        public AdminController(IBackofficeAdminServices adminServices)
        {
            this._adminServices = adminServices;

        }
        public ActionResult List()
        {
            List<BackOfficeAdmin> adminList = _adminServices.GetAll();
            return View(adminList);
        }

        public ActionResult Detail(Guid id)
        {
            BackOfficeAdmin admin = _adminServices.GetByID(id);
            if (admin == null)
            {
                admin = new BackOfficeAdmin();
                admin.BackOfficeAdminOID = Guid.NewGuid();
                admin.Password = "";
            }

            ViewBag.Password = admin.Password.Length > 10 ? admin.Password.Substring(0, 10) : "";
            return View(admin);
        }

        [HttpPost]
        public ActionResult Detail(BackOfficeAdmin model)
        {
            BackOfficeAdmin admin = _adminServices.GetByID(model.BackOfficeAdminOID);
            string password = model.Password;
            if (admin != null && admin.Password.Substring(0, 10) == model.Password)
            {
                model.Password = admin.Password;
                model.LastLogin = admin.LastLogin;
            }
            else
            {
                model.Password = BL.Utilities.Encryptor.MD5Hash(model.Password);
            }
            model.IsDelete = false;
            _adminServices.SaveOrUpdate(model);
            return RedirectToAction("List", new { save = "t" });
        }

        public JsonResult CheckUsername(string username)
        {
            BackOfficeAdmin admin = _adminServices.GetByUsermame(username);
            return Json(admin == null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(Guid id)
        {
            BackOfficeAdmin admin = _adminServices.GetByID(id);
            admin.IsDelete = true;
            _adminServices.SaveOrUpdate(admin);
            return RedirectToAction("List", new { save = "t" });
        }
    }
}