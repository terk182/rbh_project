using BL;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGBookingWeb.Models;

namespace TGBookingWeb.Controllers
{
    public class TransactionController : BaseController
    {
        // GET: Transaction
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly INamingServices _namingServices;
        private readonly IFlightReportServices _flightReportServices;
        public TransactionController(INamingServices namingServices,
           IFlightReportServices flightReportServices)
        {
            this._flightReportServices = flightReportServices;
            this._namingServices = namingServices;
            if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request["lang"]))
            {
                Localize.SetLang(System.Web.HttpContext.Current.Request["lang"]);
            }
        }
        public ActionResult Data(Guid transactionID)
        {
            BL.Entities.RobinhoodFare.AirFare airfare = _flightReportServices.GetByID(transactionID);
            if (airfare != null)
            {
                MetaDataModel model = new MetaDataModel();
                string _transactionID = airfare.RobinhoodID.Replace("F","");
                string transactionDate = _transactionID.Substring(0, 6);
                int transactionNo = int.Parse(_transactionID.Substring(6, 4));
                string transaction = String.Format("{0}{1}", transactionDate, transactionNo);
                Log.Debug("_transactionID="+ _transactionID);
                Log.Debug("transactionDate=" + transactionDate);
                Log.Debug("transactionNo=" + transactionNo);
                Log.Debug("transaction=" + transaction);
                model.id = Convert.ToInt32(transaction);
                model.name = String.Format("{0} - {1}", airfare.depFlight[0].depCity.name, airfare.depFlight[airfare.depFlight.Count - 1].arrCity.name);
                model.description = "";
                model.image = String.Format("{0}Transaction/Image?transactionID={1}", ConfigurationManager.AppSettings["Main.URL"].ToString(), transactionID);

                model.attributes = new List<attribute>();
                attribute att = new attribute();
                att.trait_type = "transactionID";
                att.value = transactionID.ToString();
                model.attributes.Add(att);
                att = new attribute();
                att.trait_type = "status";
                att.value = airfare.depFlight[0].departureDateTime >= DateTime.Now ? "true" : "false";
                model.attributes.Add(att);

                string json = JsonConvert.SerializeObject(model);
                Log.Debug(json);
                ViewBag.jsonRequest = json;
            }
            return View();
        }
        public ActionResult FileImage(Guid transactionID)
        {
            BL.Entities.RobinhoodFare.AirFare airfare = _flightReportServices.GetByID(transactionID);
            if (airfare != null)
            {
                string _transactionID = airfare.RobinhoodID.Replace("F", "");
                string transactionDate = _transactionID.Substring(0, 6);
                int transactionNo = int.Parse(_transactionID.Substring(6, 4));
                string transaction = String.Format("{0}{1}", transactionDate, transactionNo);
                ViewBag.iImage = Convert.ToInt32(transaction) % 5;
            }            
            return View(airfare);
        }
        public ActionResult Image(Guid transactionID)
        {
            string imgUrl = String.Format("{0}Transaction/FileImage?transactionID={1}", ConfigurationManager.AppSettings["Main.Url"].ToString(), transactionID);
            var urlAsImage = new Rotativa.UrlAsImage(imgUrl)
            {
                CropWidth = 800,
                CropHeight = 500,
                Format = Rotativa.Options.ImageFormat.jpeg
            };
            return urlAsImage;
        }
    }
}