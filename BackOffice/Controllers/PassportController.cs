using BL;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class PassportController : BaseController
    {
        private readonly IPassportServices _passportServices;

        public PassportController(IPassportServices passportServices)

        {
            this._passportServices = passportServices;

        }
        public ActionResult ListConfig()
        {
            List<PassportConfig> passportList = _passportServices.GetAll();
            return View(passportList);
        }

        public ActionResult Detail(Guid id)
        {
            PassportConfig passport = _passportServices.GetByID(id);
            if (passport == null)
            {
                passport = new PassportConfig();
                passport.PassportOID = Guid.NewGuid();
            }
            return View(passport);
        }

        [HttpPost]
        public ActionResult Details(PassportConfig model)
        {
            PassportConfig passport = _passportServices.GetByID(model.PassportOID);
            model.IsDelete = false;
            _passportServices.SaveOrUpdate(model);
            return RedirectToAction("ListConfig", new { save = "t" });
        }

        public ActionResult PassportDelete(Guid id)
        {
            var passport = _passportServices.GetByID(id);
            passport.IsDelete = true;
            _passportServices.SaveOrUpdate(passport);
            return RedirectToAction("ListConfig", new { save = "t" });
        }

        //public ActionResult PassportDelete(Guid id)
        //{
        //    GogojiiPassport admin = _passportServices.GetByID(id);
        //    admin.IsDelete = true;
        //    _passportServices.SaveOrUpdate(admin);
        //    return RedirectToAction("ListConfig", new { save = "t" });
        //}
    }
}
