using BL;
using BL.Entities.FareRuleConfig;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class FlightFareRuleConfigController : BaseController
    {
        // GET: FlightFareRuleConfig
        private readonly IFlightFareRuleConfigServices _fareRuleConfigServices;

        public FlightFareRuleConfigController(IFlightFareRuleConfigServices fareRuleConfigServices)
        {
            this._fareRuleConfigServices = fareRuleConfigServices;

        }
        public ActionResult FlightFareRuleList()
        {
            List<FareRuleEntities> fareRuleList = _fareRuleConfigServices.GetAll();
            if (fareRuleList == null)
            {
                fareRuleList = new List<FareRuleEntities>();
            }
            return View(fareRuleList);
        }

        public ActionResult FlightFareRuleDetail(Guid id)
        {
            FareRuleEntities fareRule = _fareRuleConfigServices.GetByID(id);
            if (fareRule == null)
            {
                fareRule = new FareRuleEntities();
                fareRule.fareRule = new DataModel.FlightFareRule();
                fareRule.fareRule.IsActive = true;
                fareRule.fareRule.IsDelete = false;
                fareRule.fareRuleDetails = new List<DataModel.FlightFareRuleDetail>();
                string[] arr = ConfigurationManager.AppSettings["webpages_LangCode"].Split('|');
                DataModel.FlightFareRuleDetail detail = new DataModel.FlightFareRuleDetail();
                for (int i = 0; i < arr.Length; i++)
                {
                    detail = new DataModel.FlightFareRuleDetail();
                    detail.LanguageCode = arr[i];
                    fareRule.fareRuleDetails.Add(detail);
                }
            }
            return View(fareRule);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult FlightFareRuleDetailSave(FareRuleEntities model)
        {
            if(model.fareRule.FareRuleOID == Guid.Empty)
            {
                model.fareRule.FareRuleOID = Guid.NewGuid();
            }
            _fareRuleConfigServices.SaveOrUpdate(model);
            return RedirectToAction("FlightFareRuleList", new { save = "t" });
        }
        
        public ActionResult FlightFareRuleDelete(Guid id)
        {
            FareRuleEntities fareRule = _fareRuleConfigServices.GetByID(id);
            fareRule.fareRule.IsDelete = true;
            _fareRuleConfigServices.SaveOrUpdate(fareRule);
            return RedirectToAction("FlightFareRuleList", new { save = "t" });
        }

        public ActionResult FareBasisFlightFareRule(Guid id)
        {
            FareRuleEntities fareRule = _fareRuleConfigServices.GetByID(id);
            if (fareRule.fareRuleConfig == null)
            {
                fareRule.fareRuleConfig = new List<DataModel.FlightFareRuleConfig>();
            }
            return View(fareRule);
        }

        public ActionResult FareBasisFlightFareRuleDetail(Guid id, Guid FareRuleOID)
        {
            FareRuleEntities fareRule = _fareRuleConfigServices.GetByID(FareRuleOID);
            DataModel.FlightFareRuleConfig model = null;
            if (fareRule.fareRuleConfig == null)
            {
                fareRule.fareRuleConfig = new List<DataModel.FlightFareRuleConfig>();
            }
            else
            {
                model = fareRule.fareRuleConfig.Find(x => x.FareRuleConfigOID == id);
                
            }
            if (model == null)
            {
                model = new DataModel.FlightFareRuleConfig();
                model.FareRuleOID = FareRuleOID;
                model.IsActive = true;
                model.IsDelete = false;
            }

            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult FareBasisFlightFareRuleDetailSave(DataModel.FlightFareRuleConfig model)
        {
            FareRuleEntities fareRule = _fareRuleConfigServices.GetByID(model.FareRuleOID);
            if (model.FareRuleConfigOID == Guid.Empty)
            {
                model.FareRuleConfigOID = Guid.NewGuid();
                fareRule.fareRuleConfig.Add(model);
            }
            else
            {
                DataModel.FlightFareRuleConfig config = fareRule.fareRuleConfig.Find(x => x.FareRuleConfigOID == model.FareRuleConfigOID);
                config.IsActive = model.IsActive;
                config.ZoneFrom = model.ZoneFrom;
                config.ZoneTo = model.ZoneTo;
                config.RBD = model.RBD;
                config.FareBasis = model.FareBasis;
            }
            
            _fareRuleConfigServices.SaveOrUpdate(fareRule);
            return RedirectToAction("FareBasisFlightFareRule", new { save = "t", id = model.FareRuleOID });
        }
        public ActionResult FareBasisFlightFareRuleDelete(Guid id, Guid FareRuleOID)
        {
            FareRuleEntities fareRule = _fareRuleConfigServices.GetByID(FareRuleOID);
            DataModel.FlightFareRuleConfig config = fareRule.fareRuleConfig.Find(x => x.FareRuleConfigOID == id);
            config.IsDelete = true;
            _fareRuleConfigServices.SaveOrUpdate(fareRule);
            return RedirectToAction("FareBasisFlightFareRule", new { save = "t", id= id });
        }
    }
}