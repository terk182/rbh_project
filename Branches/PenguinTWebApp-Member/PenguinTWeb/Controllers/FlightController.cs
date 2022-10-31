using BL;
using log4net;
using Newtonsoft.Json;
using GogojiiWeb.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace GogojiiWeb.Controllers
{
    public class FlightController : BaseController
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;

        private string officeID = ConfigurationManager.AppSettings["OfficeID"];

        public FlightController(IFlightSearchServices flightSearchServices, INamingServices namingServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            ViewBag.Menu = "2";
        }

        [OutputCache(NoStore = true, Duration = 1)]
        public ActionResult ResultList(SearchModel model)
        {
            Session["ssSearch"] = model;
            if (model.searchID == null || model.searchID == Guid.Empty)
            {
                model.searchID = Guid.NewGuid();
                return RedirectToAction("ResultList", model);
            }
            model.GetSearchText(this._namingServices);
            return View(model);
        }

        public ContentResult FlightResult(SearchModel model)
        {
            BL.Entities.MasterPricer.Request request = model.GetMPSearchRequest();
            request.bookingOID = officeID;
            BL.Entities.GogojiiFlight.FlightSearchResult result = null;
            if (Session["ssSearchResult"] != null)
            {
                Log.Info("Use result from session");
                BL.Entities.GogojiiFlight.FlightSearchResult ssResult = (BL.Entities.GogojiiFlight.FlightSearchResult)Session["ssSearchResult"];
                if (ssResult.pgSearchOID == model.searchID)
                {
                    var jsonS = ssResult.toJson();
                    return Content(jsonS, "application/json");
                }
            }
            result = this._flightSearchServices.Search(request);
            Session["ssSearchResult"] = result;
            var json = result.toJson();
            return Content(json, "application/json");
        }

        public ContentResult FlightResultMock(SearchModel model)
        {
            string text = System.IO.File.ReadAllText(@"D:\GogojiiWebApp\logs\test.json");

            return Content(text, "application/json");
        }


        public ActionResult SelectFlight(SelectedFlightsModel model)
        {
            Session.Remove("ssPNR");
            var airSellReq = model.GetInformativePricingRequest();
            var airSellReq1A = model.GetInformativePricingRequestFor1A();

            airSellReq.bookingOID = officeID;
            airSellReq1A.bookingOID = officeID;

            BL.Entities.GogojiiFare.AirFare response = _flightSearchServices.InformativePricing(airSellReq, airSellReq1A, Localize.GetLang());

            if (response == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "FLIGHTNOTFOUND" });
            }

            response.oldPrice = Convert.ToDecimal(model.price);
            if (response.totalFare != response.oldPrice)
            {
                response.priceChange = true;
            }
            Session["ssPricing"] = response;
            Session["ssDeepLink"] = Request.Url.AbsoluteUri;

            return RedirectToAction("PaxInfo");
        }

        public ActionResult FareRule(SelectedFlightsModel model)
        {
            var airSellReq = model.GetInformativePricingRequest();
            var airSellReq1A = model.GetInformativePricingRequestFor1A();
            airSellReq.bookingOID = officeID;
            airSellReq1A.bookingOID = officeID;
            BL.Entities.GogojiiFare.AirFare response = _flightSearchServices.InformativePricing(airSellReq, airSellReq1A, Localize.GetLang());

            if (response == null)
            {
                ViewBag.Error = "ไม่สามารถดึงเงื่อนไขบัตรโดยสารได้";
            }

            return View(response);
        }

        public ActionResult PaxInfo()
        {
            if (Session["ssPricing"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }

            if (Session["ssDeepLink"] != null)
            {
                string deeplink = (string)Session["ssDeepLink"];
                Response.Write("<!-- Deeplink: " + deeplink + " -->");
            }
            BL.Entities.GogojiiFare.AirFare response = (BL.Entities.GogojiiFare.AirFare)Session["ssPricing"];
            ViewBag.JSON = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            var cList = new List<object>();
            cList.Add(new
            {
                ID = "",
                Name = ""
            });
            cList.Add(new
            {
                ID = "MR",
                Name = "Mr. (นาย)"
            });
            cList.Add(new
            {
                ID = "MRS",
                Name = "Mrs. (นาง)"
            });
            cList.Add(new
            {
                ID = "MS",
                Name = "Ms. (นางสาว)"
            });
            cList.Add(new
            {
                ID = "MISS",
                Name = "Miss (นางสาว)"
            });
            var adtTitleList = new SelectList(cList, "ID", "Name", "");
            ViewData["adtTitleList"] = adtTitleList;

            cList = new List<object>();
            cList.Add(new
            {
                ID = "",
                Name = ""
            });
            cList.Add(new
            {
                ID = "MSTR",
                Name = "Mstr. (เด็กชาย)"
            });
            cList.Add(new
            {
                ID = "MISS",
                Name = "Miss (เด็กหญิง)"
            });
            var chdTitleList = new SelectList(cList, "ID", "Name", "");
            ViewData["chdTitleList"] = chdTitleList;
            Session["ssPricing"] = response;
            return View(response);
        }

        [HttpPost]
        public ActionResult Booking(BL.Entities.GogojiiFare.AirPax airPaxModel)
        {
            if (Session["ssPricing"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }
            BL.Entities.GogojiiFare.AirFare airfare = (BL.Entities.GogojiiFare.AirFare)Session["ssPricing"];
            airfare.adtPaxs = airPaxModel.adtPaxs;
            if (airfare.noOfChildren > 0)
            {
                airfare.chdPaxs = airPaxModel.chdPaxs;
                for (int i = 0; i < airfare.chdPaxs.Count; i++)
                {
                    airfare.chdPaxs[i].birthday = DateTime.ParseExact(Request["CHD_BD_" + i.ToString()]
                            , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            if (airfare.noOfInfants > 0)
            {
                airfare.infPaxs = airPaxModel.infPaxs;
                for (int i = 0; i < airfare.infPaxs.Count; i++)
                {
                    airfare.infPaxs[i].birthday = DateTime.ParseExact(Request["INF_BD_" + i.ToString()]
                            , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            airfare.TKTL = DateTime.Now.AddHours(24);
            ViewBag.JSON = Newtonsoft.Json.JsonConvert.SerializeObject(airfare);

            airfare.bookingOID = officeID;

            BL.Entities.GogojiiPNR.PNR pnr = _flightSearchServices.Booking(airfare);
            if (pnr.isError)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = pnr.errorMessage });
            }
            else
            {
                Session["ssPricing"] = airfare;
                Session["ssPNR"] = pnr;
                return RedirectToAction("Summary", "Flight");
            }

        }

        public ActionResult Summary()
        {
            if (Session["ssPricing"] == null || Session["ssPNR"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }
            BL.Entities.GogojiiFare.AirFare airfare = (BL.Entities.GogojiiFare.AirFare)Session["ssPricing"];
            BL.Entities.GogojiiPNR.PNR pnr = (BL.Entities.GogojiiPNR.PNR)Session["ssPNR"];
            ViewBag.PNR = pnr.recordLocator;
            return View(airfare);
        }

        [HttpPost]
        public ActionResult Pay(string payment_method)
        {
            if (Session["ssPricing"] == null || Session["ssPNR"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }
            BL.Entities.GogojiiFare.AirFare airfare = (BL.Entities.GogojiiFare.AirFare)Session["ssPricing"];
            BL.Entities.GogojiiPNR.PNR pnr = (BL.Entities.GogojiiPNR.PNR)Session["ssPNR"];

            if (payment_method == "BT")
            {
                return RedirectToAction("Thankyou");
            }
            else if (payment_method == "CC")
            {
                return RedirectToAction("InitialKTC");
            }
            else if (payment_method == "PP")
            {
                string invoiceNo = _flightSearchServices.GetInvoiceNo(new BL.Entities.Invoice.Request()
                {
                    Ref = pnr.recordLocator,
                    NameLastname = String.Format("{0} {1} {2}", airfare.adtPaxs[0].title.ToUpper(),
                                    airfare.adtPaxs[0].firstname.ToUpper(), 
                                    airfare.adtPaxs[0].lastname.ToUpper()),
                    Email = airfare.adtPaxs[0].email,
                    Telephonenumber = airfare.adtPaxs[0].telNo
                });
                PaypalModel paypal = new PaypalModel(Url.Action("Index", "Flight", null, Request.Url.Scheme), airfare.paypalFee + airfare.totalFare, pnr.recordLocator, invoiceNo, ConfigurationManager.AppSettings["WEBMODE"] != "PROD");
                Session.Add("ssPaypal", paypal);
                return Redirect(paypal.SUBMIT_URL);
            }
            else if (payment_method == "CS")
            {
                return RedirectToAction("InitialCS");
            }

            return RedirectToAction("Thankyou");
        }

        public ActionResult Thankyou()
        {
            ViewBag.Payment = Request["pm"] ?? "";
            return View();
        }

        public ActionResult InitialKTC()
        {
            if (Session["ssPricing"] == null || Session["ssPNR"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }
            BL.Entities.GogojiiFare.AirFare airfare = (BL.Entities.GogojiiFare.AirFare)Session["ssPricing"];
            BL.Entities.GogojiiPNR.PNR pnr = (BL.Entities.GogojiiPNR.PNR)Session["ssPNR"];

            if (ConfigurationManager.AppSettings["WEBMODE"] != "PROD")
            {
                ViewBag.WebMode = "test";
            }
            else
            {
                ViewBag.WebMode = "pro";
            }

            ViewBag.PNR = pnr.recordLocator;
            ViewBag.BookingKeyReference = pnr.BookingKeyReference;

            string _frontURL = Url.Action("Index", "Flight", null, Request.Url.Scheme);
            _frontURL =_frontURL + "/Thankyou?pm=op";
            ViewBag.RespURL = _frontURL;
            return View(airfare);
        }

        public ActionResult InitialCS()
        {
            if (Session["ssPricing"] == null || Session["ssPNR"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }
            BL.Entities.GogojiiFare.AirFare airfare = (BL.Entities.GogojiiFare.AirFare)Session["ssPricing"];
            BL.Entities.GogojiiPNR.PNR pnr = (BL.Entities.GogojiiPNR.PNR)Session["ssPNR"];

            string invoiceNo = _flightSearchServices.GetInvoiceNo(new BL.Entities.Invoice.Request()
            {
                Ref = pnr.recordLocator,
                NameLastname = String.Format("{0} {1} {2}", airfare.adtPaxs[0].title.ToUpper(),
                                airfare.adtPaxs[0].firstname.ToUpper(),
                                airfare.adtPaxs[0].lastname.ToUpper()),
                Email = airfare.adtPaxs[0].email,
                Telephonenumber = airfare.adtPaxs[0].telNo
            });

            CounterServiceModel csModel = new CounterServiceModel(Url.Action("Index", "Flight", null, Request.Url.Scheme), airfare, pnr, invoiceNo, ConfigurationManager.AppSettings["WEBMODE"] == "PROD");
            
            return View(csModel);
        }

        public ActionResult PaypalExpressStatusWithBNCode()
        {
            if (Session["ssPaypal"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }

            PaypalModel paypal = (PaypalModel)Session["ssPaypal"];
            bool bSuccess = false;
            string tlsResponse = Request["tlsres"] ?? "";
            NameValueCollection coll;
            coll = Request.QueryString;
            if (tlsResponse != "")
            {
                bSuccess = paypal.checkResponseFromTlsServer(tlsResponse, coll);
            }
            else
            {
                bSuccess = paypal.doPaypalPayment(coll);
            }

            if (bSuccess)
            {
                BL.Entities.GogojiiPNR.PNR pnr = (BL.Entities.GogojiiPNR.PNR)Session["ssPNR"];
                _flightSearchServices.UpdatePaymentStatus(pnr, 2);
                return RedirectToAction("Thankyou", new { pm = "op" });
            }
            else
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "Payment Fail" });
            }
        }

        public ActionResult PayPalCancel()
        {
            return RedirectToAction("SystemProblem", "Home", new { msg = "Payment Fail" });
        }

        public ActionResult CSConfirm()
        {
            return RedirectToAction("Thankyou", new { pm = "cs" });
        }

        public ActionResult CSCancel()
        {
            return RedirectToAction("SystemProblem", "Home", new { msg = "Payment Fail" });
        }
    }
}