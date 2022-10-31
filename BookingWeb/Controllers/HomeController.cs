using BL;
using TGBookingWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace TGBookingWeb.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        public HomeController(IFlightSearchServices flightSearchServices,
            INamingServices namingServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            ViewBag.Menu = "1";
            
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            AllSearchModel searchModel = new AllSearchModel();
            searchModel.flightSearchM = new SearchModel();
            if (Session["ssSearch"] != null)
            {
                searchModel.flightSearchM = (SearchModel)Session["ssSearch"];
            }
            
            return View(searchModel);
        }

        public ActionResult SystemProblem(string msg)
        {
            ViewBag.Message = Localize.Show(msg);

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult PrivacyPolicy()
        {

            return View();
        }
        public ActionResult FAQs()
        {

            return View();
        }
        public ActionResult TermOfUse()
        {

            return View();
        }
        public ActionResult CookiePolicy()
        {

            return View();
        }
        public ActionResult ContactUs()
        {

            return View();
        }
        public ActionResult AboutUs()
        {

            return View();
        }

        public ActionResult GetThumbPrintCertificate()
        {
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                        X509FindType.FindByThumbprint,
                                        // Replace below with your certificate's thumbprint
                                        "841F0903325D389427A90073968D0D2AC20B55A0",
                                        false);
            // Get the first cert with the thumbprint
            if (certCollection.Count > 0)
            {
                X509Certificate2 cert = certCollection[0];
                // Use certificate
                Response.Write(cert.FriendlyName);
            }
            certStore.Close();

            return View();
        }

        public ActionResult Refresh()
        {
            foreach (System.Collections.DictionaryEntry entry in System.Web.HttpContext.Current.Cache)
            {
                System.Web.HttpContext.Current.Cache.Remove((string)entry.Key);
            }

            return View();
        }

        public JsonResult SearchCity(string keyword, string language)
        {
            /*
            if (keyword.Length < 3)
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
            var cityList = _flightSearchServices.SearchCities(keyword, language, 20);*/
            var cityList = _namingServices.SearchCities(keyword, language);
            return Json(cityList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExternalFooter()
        {
            return View();
        }

        public JsonResult ChangeLanguage(string lang)
        {
            Localize.SetLang(lang);
            return null;
        }
        public JsonResult ChangeCurrency(string ccy)
        {
            Currency.SetCurrency(ccy);
            return null;
        }
        
       
    }
}