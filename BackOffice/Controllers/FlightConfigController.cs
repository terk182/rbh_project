using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class FlightConfigController : BaseController
    {
        // GET: FlightConfig
        private readonly ISiteConfigServices _siteConfigServices;

        public FlightConfigController(ISiteConfigServices siteConfigServices)
        {
            this._siteConfigServices = siteConfigServices;

        }
        public ActionResult FlightConfigDetail()
        {
            List<DataModel.SiteConfig> airlineConfig = _siteConfigServices.GetAll();
            if (airlineConfig == null || (airlineConfig!=null && airlineConfig.Count==0))
            {
                airlineConfig = new List<DataModel.SiteConfig>();
                DataModel.SiteConfig siteConfig = new DataModel.SiteConfig();
                siteConfig.Seq = 1;
                siteConfig.ConfigKey = "OfficeID";//for search
                siteConfig.ConfigValue = "";
                airlineConfig.Add(siteConfig);
                siteConfig = new DataModel.SiteConfig();
                siteConfig.Seq = 2;
                siteConfig.ConfigKey = "InboundWeight";
                siteConfig.ConfigValue = "";
                airlineConfig.Add(siteConfig);
                siteConfig = new DataModel.SiteConfig();
                siteConfig.Seq = 3;
                siteConfig.ConfigKey = "OutboundWeight";
                siteConfig.ConfigValue = "";
                airlineConfig.Add(siteConfig);
                siteConfig = new DataModel.SiteConfig();
                siteConfig.Seq = 4;
                siteConfig.ConfigKey = "RoundtripWeight";
                siteConfig.ConfigValue = "";
                airlineConfig.Add(siteConfig);
                siteConfig = new DataModel.SiteConfig();
                siteConfig.Seq = 5;
                siteConfig.ConfigKey = "TicketIndicator";
                siteConfig.ConfigValue = "";
                airlineConfig.Add(siteConfig);
                siteConfig = new DataModel.SiteConfig();
                siteConfig.Seq = 6;
                siteConfig.ConfigKey = "TicketTimeLimitHour";
                siteConfig.ConfigValue = "";
                airlineConfig.Add(siteConfig);
                siteConfig = new DataModel.SiteConfig();
                siteConfig.Seq = 7;
                siteConfig.ConfigKey = "OfficeID_Save";
                siteConfig.ConfigValue = "";
                airlineConfig.Add(siteConfig);
                siteConfig = new DataModel.SiteConfig();
                siteConfig.Seq = 8;
                siteConfig.ConfigKey = "TICKET.QUEUE_OFFICEID";
                siteConfig.ConfigValue = "";
                airlineConfig.Add(siteConfig);
                siteConfig = new DataModel.SiteConfig();
                siteConfig.Seq = 9;
                siteConfig.ConfigKey = "TICKET.QUEUE_NUMBER";
                siteConfig.ConfigValue = "";
                airlineConfig.Add(siteConfig);
                siteConfig = new DataModel.SiteConfig();
                siteConfig.Seq = 10;
                siteConfig.ConfigKey = "TICKET.QUEUE_CAT";
                siteConfig.ConfigValue = "";
                airlineConfig.Add(siteConfig);
            }

            return View(airlineConfig);
        }
        [HttpPost]
        public ActionResult Details(List<DataModel.SiteConfig> model)
        {
            DataModel.SiteConfig siteConfig = new DataModel.SiteConfig();
            foreach (DataModel.SiteConfig config in model)
            {
                siteConfig = new DataModel.SiteConfig();
                siteConfig.ConfigOID = config.ConfigOID == Guid.Empty ? Guid.NewGuid() : config.ConfigOID;
                siteConfig.Seq = config.Seq;
                siteConfig.ConfigKey = config.ConfigKey;
                siteConfig.ConfigValue = config.ConfigValue;
                _siteConfigServices.SaveOrUpdate(siteConfig);
            }
            return RedirectToAction("FlightConfigDetail", new { save = "t" });
        }
    }
}