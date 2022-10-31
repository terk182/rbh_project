using BL;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class AirlineQtaxController : BaseController
    {
        private readonly IAirlineQtaxControlServices _airlineQtaxServices;

        public AirlineQtaxController(IAirlineQtaxControlServices airlineQtaxServices)

        {
            this._airlineQtaxServices = airlineQtaxServices;

        }

        public ActionResult AirlineQtaxList()
        {
            List<AirlineQtaxControl> airineQtaxList = _airlineQtaxServices.GetAll();
            return View(airineQtaxList);
        }

        public ActionResult AirlineQtaxDetail(Guid id)
        {
            AirlineQtaxControl airlineQtax = _airlineQtaxServices.GetByID(id);
            if (airlineQtax == null)
            {
                airlineQtax = new AirlineQtaxControl();
                airlineQtax.AirlineQtaxOID = Guid.NewGuid();

                //admin.Password = "";
            }

            return View(airlineQtax);
        }


        [HttpPost]
        public ActionResult Details(AirlineQtaxControl model)
        {
            AirlineQtaxControl airline = _airlineQtaxServices.GetByID(model.AirlineQtaxOID);
            model.IsDelete = false;
            //model.LastUpdate = DateTime.Now;
            _airlineQtaxServices.SaveOrUpdate(model);
            return RedirectToAction("AirlineQtaxList", new { save = "t" });
        }

        public ActionResult AirlineQtaxControlDelete(Guid id)
        {
            var airlineQtax = _airlineQtaxServices.GetByID(id);
            airlineQtax.IsDelete = true;
            _airlineQtaxServices.SaveOrUpdate(airlineQtax);
            return RedirectToAction("AirlineQtaxList", new { save = "t" });
        }
    }
}