using BL;
using BL.Entities.AirlineControl;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class AirlineController : BaseController
    {
        private readonly IAirlineControlServices _airlineServices;

        public AirlineController(IAirlineControlServices airlineServices)

        {
            this._airlineServices = airlineServices;

        }

        public ActionResult AirlineList()
        {
            List<AirlineControl> airineList = _airlineServices.GetAll();
            return View(airineList);
        }

        public ActionResult AirlineSubList(Guid id)
        {
            List<AirlineControlSub> airineSubList = _airlineServices.GetAllAirlineSub(id);
            ViewBag.airline_id = id; 
            return View(airineSubList);
        }

        public ActionResult AirlineDetail(Guid id)
        {
            AirlineControl airline = _airlineServices.GetByID(id);
            if (airline == null )
            {
                airline = new AirlineControl();
                airline.AirlineOID = Guid.NewGuid();

                //admin.Password = "";
            }

            return View(airline);
        }

        public ActionResult AirlineSubDetail(Guid id , Guid AirlineOID)
        {
            AirlineControlSub airlineSub = _airlineServices.GetByIDAirlineSub(id, AirlineOID);
            if (airlineSub == null)
            {
                airlineSub = new AirlineControlSub();
                airlineSub.AirlineSubOID = id;
                airlineSub.AirlineOID = AirlineOID;

                //admin.Password = "";
            }

            return View(airlineSub);
        }


        [HttpPost]
        public ActionResult DetailsSub(AirlineControlSub model)
        {
            AirlineControlSub airline = _airlineServices.GetByIDAirlineSub(model.AirlineSubOID,model.AirlineOID);
            model.IsDelete = false;
            //model.LastUpdate = DateTime.Now;
            _airlineServices.SaveOrUpdateAirlinesub(model);
            return RedirectToAction("AirlineSubList" + "/" + model.AirlineOID, new { save = "t" });
        }

        [HttpPost]
        public ActionResult Details(AirlineControl model)
        {
            AirlineControl airline = _airlineServices.GetByID(model.AirlineOID);
            model.IsDelete = false;
            //model.LastUpdate = DateTime.Now;
            _airlineServices.SaveOrUpdate(model);
            return RedirectToAction("AirlineList", new { save = "t" });
        }

        public ActionResult AirlineSubControlDelete(Guid id, Guid AirlineOID)
        {
            var airline = _airlineServices.GetByIDAirlineSub(id, AirlineOID);
            airline.IsDelete = true;
            _airlineServices.SaveOrUpdateAirlinesub(airline);
            return RedirectToAction("AirlineSubList" + "/" + AirlineOID, new { save = "t" });
        }

        public ActionResult AirlineControlDelete(Guid id)
        {
            var airline = _airlineServices.GetByID(id);
            airline.IsDelete = true;
            _airlineServices.SaveOrUpdate(airline);
            return RedirectToAction("AirlineList", new { save = "t" });
        }


    }
}