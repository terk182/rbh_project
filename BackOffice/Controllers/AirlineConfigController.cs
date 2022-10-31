using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class AirlineConfigController : BaseController
    {
        // GET: AirlineConfig
        private readonly IAirlineConfigServices _airlineConfigServices;

        public AirlineConfigController(IAirlineConfigServices airlineConfigServices)

        {
            this._airlineConfigServices = airlineConfigServices;

        }
        public ActionResult AirlineConfigList()
        {
            List<DataModel.AirlineConfig> airineConfigList = _airlineConfigServices.GetAll();
            return View(airineConfigList);
        }

        public ActionResult AirlineConfigDetail(Guid id)
        {
            DataModel.AirlineConfig airlineConfig = _airlineConfigServices.GetByID(id);
            if (airlineConfig == null)
            {
                airlineConfig = new DataModel.AirlineConfig();
                airlineConfig.AirlineConfigOID = Guid.NewGuid();
            }

            return View(airlineConfig);
        }
        [HttpPost]
        public ActionResult Details(DataModel.AirlineConfig model)
        {
            model.IsDelete = false;
            _airlineConfigServices.SaveOrUpdate(model);
            return RedirectToAction("AirlineConfigList", new { save = "t" });
        }

        public ActionResult AirlineConfigDelete(Guid id)
        {
            var airlineConfig = _airlineConfigServices.GetByID(id);
            airlineConfig.IsDelete = true;
            _airlineConfigServices.SaveOrUpdate(airlineConfig);
            return RedirectToAction("AirlineConfigList", new { save = "t" });
        }
    }
}