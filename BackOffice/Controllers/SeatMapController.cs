using BL;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class SeatMapController : BaseController
    {
        private readonly ISeatMapControlServices _seatMapServices;

        public SeatMapController(ISeatMapControlServices seatMapServices)

        {
            this._seatMapServices = seatMapServices;

        }

        public ActionResult SeatMapList()
        {
            List<SeatMapControl> seatMapList = _seatMapServices.GetAll();
            return View(seatMapList);
        }

        public ActionResult SeatMapDetail(Guid id)
        {
            SeatMapControl seatMap = _seatMapServices.GetByID(id);
            if (seatMap == null)
            {
                seatMap = new SeatMapControl();
                seatMap.SeatMapOID = Guid.NewGuid();

                //admin.Password = "";
            }

            return View(seatMap);
        }


        [HttpPost]
        public ActionResult Details(SeatMapControl model)
        {
            SeatMapControl fourDigit = _seatMapServices.GetByID(model.SeatMapOID);
            model.IsDelete = false;
            //model.LastUpdate = DateTime.Now;
            _seatMapServices.SaveOrUpdate(model);
            return RedirectToAction("SeatMapList", new { save = "t" });
        }

        public ActionResult SeatMapControlDelete(Guid id)
        {
            var fourDigit = _seatMapServices.GetByID(id);
            fourDigit.IsDelete = true;
            _seatMapServices.SaveOrUpdate(fourDigit);
            return RedirectToAction("SeatMapList", new { save = "t" });
        }
    }
}