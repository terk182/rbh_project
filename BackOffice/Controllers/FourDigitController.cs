using BL;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class FourDigitController : BaseController
    {
        private readonly IFourDigitControlServices _fourDigitServices;

        public FourDigitController(IFourDigitControlServices fourDigitServices)

        {
            this._fourDigitServices = fourDigitServices;

        }

        public ActionResult FourDigitList()
        {
            List<FourDigitControl> fourDigitList = _fourDigitServices.GetAll();
            return View(fourDigitList);
        }

        public ActionResult FourDigitDetail(Guid id)
        {
            FourDigitControl fourDigit = _fourDigitServices.GetByID(id);
            if (fourDigit == null)
            {
                fourDigit = new FourDigitControl();
                fourDigit.FourDigitOID = Guid.NewGuid();

                //admin.Password = "";
            }

            return View(fourDigit);
        }


        [HttpPost]
        public ActionResult Details(FourDigitControl model)
        {
            FourDigitControl fourDigit = _fourDigitServices.GetByID(model.FourDigitOID);
            model.IsDelete = false;
            //model.LastUpdate = DateTime.Now;
            _fourDigitServices.SaveOrUpdate(model);
            return RedirectToAction("FourDigitList", new { save = "t" });
        }

        public ActionResult FourDigitControlDelete(Guid id)
        {
            var fourDigit = _fourDigitServices.GetByID(id);
            fourDigit.IsDelete = true;
            _fourDigitServices.SaveOrUpdate(fourDigit);
            return RedirectToAction("FourDigitList", new { save = "t" });
        }
    }
}