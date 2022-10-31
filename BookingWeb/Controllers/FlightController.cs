using BL;
using log4net;
using Newtonsoft.Json;
using TGBookingWeb.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DataModel;

namespace TGBookingWeb.Controllers
{
    public class FlightController : BaseController
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        private readonly IFlightReportServices _flightReportServices;
        private readonly ICRMServices _crmServices;
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];

        public FlightController(IFlightSearchServices flightSearchServices,
            INamingServices namingServices,
            IFlightReportServices flightReportServices,
            ICRMServices crmServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            this._flightReportServices = flightReportServices;
            this._crmServices = crmServices;
            ViewBag.Menu = "2";
            if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request["lang"]))
            {
                Localize.SetLang(System.Web.HttpContext.Current.Request["lang"]);
            }
            if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request["currency"]))
            {
                Currency.SetCurrency(System.Web.HttpContext.Current.Request["currency"]);
            }
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            SearchModel searchModel = new SearchModel();
            if (Session["ssSearch"] != null)
            {
                searchModel = (SearchModel)Session["ssSearch"];
            }

            return View(searchModel);
        }

        [OutputCache(NoStore = true, Duration = 1)]
        public ActionResult ResultList(SearchModel model)
        {
            Session["ssSearch"] = model;
            if (String.IsNullOrEmpty(model.lang))
            {
                model.lang = "en";
            }

            if (model.origin.Length > 3 && (model.origin.IndexOf("[") <= 0 || model.origin.IndexOf("]") <= 0))
            {
                string code = _namingServices.FindBestMatchCityAirport(model.origin);
                if (code != "")
                {
                    model.origin = code;
                }
            }
            if (model.destination.Length > 3 && (model.destination.IndexOf("[") <= 0 || model.destination.IndexOf("]") <= 0))
            {
                string code = _namingServices.FindBestMatchCityAirport(model.destination);
                if (code != "")
                {
                    model.destination = code;
                }
            }

            if (model.origin.Length == 3)
            {
                model.origin = _namingServices.GetFullCityAirportName(model.origin, model.lang);
            }
            if (model.destination.Length == 3)
            {
                model.destination = _namingServices.GetFullCityAirportName(model.destination, model.lang);
            }
            if (model.searchID == null || model.searchID == Guid.Empty)
            {
                model.searchID = Guid.NewGuid();
                return RedirectToAction("ResultList", model);
            }
            model.GetSearchText(this._namingServices);
           

            if (!String.IsNullOrEmpty(model.platform))
            {
                Session.Add("ssPlatform", model.platform);
            }
            return View(model);
        }

        public ContentResult FlightResult(SearchModel model)
        {
            BL.Entities.MasterPricer.Request request = model.GetMPSearchRequest();
            request.bookingOID = officeID;
            BL.Entities.RobinhoodFlight.FlightSearchResult result = null;
            if (Session["ssSearchResult"] != null)
            {
                Log.Info("Use result from session");
                BL.Entities.RobinhoodFlight.FlightSearchResult ssResult = (BL.Entities.RobinhoodFlight.FlightSearchResult)Session["ssSearchResult"];
                if (ssResult.pgSearchOID == model.searchID)
                {
                    var jsonS = ssResult.toJsonV2();
                    return Content(jsonS, "application/json");
                }
            }
           
            result = this._flightSearchServices.Search(request);
            if (result != null && result.flights != null && result.flights.Count > 0)
            {
                foreach (var flight in result.flights)
                {
                    foreach (var dep in flight.departureFlight)
                    {
                        foreach (var detail in dep.flightDetails)
                        {
                            detail.equipmentTypeName = Aircraft.Show(detail.equipmentType);
                        }
                    }

                    if (flight.returnFlight != null)
                    {
                        foreach (var ret in flight.returnFlight)
                        {
                            foreach (var detail in ret.flightDetails)
                            {
                                detail.equipmentTypeName = Aircraft.Show(detail.equipmentType);
                            }
                        }
                    }
                }

                Session["ssSearchResult"] = result;

                var json = result.toJsonV2();
                Log.Debug(json);
                return Content(json, "application/json");
            }
            else
            {
                return null;
            }
        }

        public ContentResult FlightResultMock(SearchModel model)
        {
            string text = System.IO.File.ReadAllText(@"D:\TGBookingWebApp\logs\test.json");

            return Content(text, "application/json");
        }


        public ActionResult SelectFlight(SelectedFlightsModel model)
        {
            Session.Remove("ssPNR");
            Session.Remove("ssKiwi");
            Session.Remove("ssRequest_Booking");
            Session.Remove("ssResponse_KiwiBooking");

            var airSellReq = model.GetInformativePricingRequest();
            var airSellReq1A = model.GetInformativePricingRequestFor1A();

          
            airSellReq.bookingOID = officeID;
            airSellReq1A.bookingOID = officeID;

            BL.Entities.RobinhoodFare.AirFare response = _flightSearchServices.InformativePricing(airSellReq, airSellReq1A, Localize.GetLang());

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
            BL.Entities.RobinhoodFare.AirFare response = _flightSearchServices.InformativePricing(airSellReq, airSellReq1A, Localize.GetLang());

            if (response == null)
            {
                ViewBag.Error = "ไม่สามารถดึงเงื่อนไขบัตรโดยสารได้";
            }

            return View(response);
        }

        public ActionResult SelectFlightKiwi()
        {
            if (Session["ssSearch"] == null || Session["ssSearchResult"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }
            Session.Remove("ssPNR");
            BL.Entities.RobinhoodFlight.FlightSearchResult result = (BL.Entities.RobinhoodFlight.FlightSearchResult)Session["ssSearchResult"];
            string flights_id = Request["flights_id"];
           
            BL.Entities.RobinhoodFare.AirFare response = null;
    
            if (response == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "FLIGHTNOTFOUND" });
            }

            response.oldPrice = Convert.ToDecimal(Request["price"]);
            if (response.totalFare != response.oldPrice)
            {
                response.priceChange = true;
            }
            Session["ssPricing"] = response;
            return RedirectToAction("PaxInfo");
        }

        public ActionResult PaxInfo()
        {
            //FlightBooking paxinfo = _flightBookingServices.GetByID(id);
            //if (paxinfo == null)
            //{
            //    paxinfo = new FlightBooking();
            //    paxinfo.FlightBookingOID = Guid.NewGuid();
            //}

            if (Session["ssPricing"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }
            Session["ssURL_Redirect_Page"] = Request.Url.AbsoluteUri;
            if (Session["ssDeepLink"] != null)
            {
                string deeplink = (string)Session["ssDeepLink"];
                Response.Write("<!-- Deeplink: " + deeplink + " -->");
            }
            BL.Entities.RobinhoodFare.AirFare response = (BL.Entities.RobinhoodFare.AirFare)Session["ssPricing"];
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
                Name = "Mr."
            });
            cList.Add(new
            {
                ID = "MRS",
                Name = "Mrs."
            });
            cList.Add(new
            {
                ID = "MS",
                Name = "Ms."
            });
            cList.Add(new
            {
                ID = "MISS",
                Name = "Miss"
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
                Name = "Mstr."
            });
            cList.Add(new
            {
                ID = "MISS",
                Name = "Miss"
            });
            var chdTitleList = new SelectList(cList, "ID", "Name", "");
            ViewData["chdTitleList"] = chdTitleList;

            var allCountry = _namingServices.GetAllCountry();
            allCountry = allCountry.Where(x => x.Language == "en").ToList();
            cList = new List<object>();
            foreach (var country in allCountry)
            {
                cList.Add(new
                {
                    ID = country.Code,
                    Name = country.Name
                });
            }
            var countryList = new SelectList(cList, "ID", "Name", "TH");
            ViewData["countryList"] = countryList;

            var allAirline = _namingServices.GetAllAirline();
            allAirline = allAirline.Where(x => x.Language == "en").OrderBy(x => x.Name).ToList();
            cList = new List<object>();
            cList.Add(new
            {
                ID = "",
                Name = ""
            });
            foreach (var a in allAirline)
            {
                cList.Add(new
                {
                    ID = a.Code,
                    Name = a.Name + " (" + a.Code + ")"
                });
            }
            var airlineList = new SelectList(cList, "ID", "Name", "");
            ViewData["airlineList"] = airlineList;

            cList = new List<object>();
            cList.Add(new
            {
                ID = "",
                Name = ""
            });
            cList.Add(new
            {
                ID = "W",
                Name = Localize.Show("WINDOW_SEAT")
            });
            cList.Add(new
            {
                ID = "A",
                Name = Localize.Show("AISEL_SEAT")
            });
            var seatList = new SelectList(cList, "ID", "Name", "");
            ViewData["seatList"] = seatList;

            #region meal

            cList = new List<object>();
            cList.Add(new
            {
                ID = "",
                Name = ""
            });
            cList.Add(new
            {
                ID = "AVML",
                Name = "ASIAN VEGETARIAN MEAL"
            });
            cList.Add(new
            {
                ID = "BLML",
                Name = "BLAND MEAL"
            });
            cList.Add(new
            {
                ID = "DBML",
                Name = "DIABETIC MEAL"
            });
            cList.Add(new
            {
                ID = "FPML",
                Name = "FRUIT PLATTER"
            });
            cList.Add(new
            {
                ID = "GFML",
                Name = "GLUTEN-FREE MEAL"
            });
            cList.Add(new
            {
                ID = "HNML",
                Name = "HINDU (NON VEGETARIAN) MEAL"
            });
            cList.Add(new
            {
                ID = "KSML",
                Name = "KOSHER MEAL"
            });
            cList.Add(new
            {
                ID = "LCML",
                Name = "LOW CALORIE MEAL"
            });
            cList.Add(new
            {
                ID = "LFML",
                Name = "LOW CHOLESTEROL/LOW FAT MEAL "
            });
            cList.Add(new
            {
                ID = "LSML",
                Name = "LOW SODIUM, NO SALT ADDED"
            });
            cList.Add(new
            {
                ID = "MOML",
                Name = "MOSLEM MEAL"
            });
            cList.Add(new
            {
                ID = "NLML",
                Name = "NON LACTOSE MEAL"
            });
            cList.Add(new
            {
                ID = "ORML",
                Name = "ORIENTAL MEAL"
            });
            cList.Add(new
            {
                ID = "RVML",
                Name = "RAW VEGETARIAN MEAL"
            });
            cList.Add(new
            {
                ID = "SFML",
                Name = "SEA FOOD MEAL"
            });
            cList.Add(new
            {
                ID = "VGML",
                Name = "VEGETARIAN MEAL (NON-DAIRY)"
            });
            cList.Add(new
            {
                ID = "VLML",
                Name = "VEGETARIAN MEAL (LACTO-OVO)"
            });
            cList.Add(new
            {
                ID = "VOML",
                Name = "VEGETARIAN ORIENTAL MEAL"
            });
            var adtMealList = new SelectList(cList, "ID", "Name", "");
            ViewData["adtMealList"] = adtMealList;

            cList = new List<object>();
            cList.Add(new
            {
                ID = "",
                Name = ""
            });
            cList.Add(new
            {
                ID = "CHML",
                Name = "CHILD MEAL "
            });
            var chdMealList = new SelectList(cList, "ID", "Name", "");
            ViewData["chdMealList"] = chdMealList;

            cList = new List<object>();
            cList.Add(new
            {
                ID = "",
                Name = ""
            });
            cList.Add(new
            {
                ID = "BBML",
                Name = "INFANT/BABY FOOD"
            });
            var infMealList = new SelectList(cList, "ID", "Name", "");
            ViewData["infMealList"] = infMealList;
            #endregion

          

            Session["ssPricing"] = response;
            return View(response);
        }

        [HttpPost]
        public ActionResult Booking(BL.Entities.RobinhoodFare.AirPax airPaxModel/*, FlightBooking model*/)
        {
            //FlightBooking paxinfo = _flightBookingServices.GetByID(model.FlightBookingOID);
            //_flightBookingServices.SaveOrUpdate(model);
            if (Session["ssPricing"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }
            BL.Entities.RobinhoodPNR.PNR pnr = null;
            BL.Entities.RobinhoodFare.AirFare airfare = (BL.Entities.RobinhoodFare.AirFare)Session["ssPricing"];
            airfare.contactInfo = airPaxModel.contactInfo;
            airfare.contactInfo.telNo = Request["phoneCode"] + airfare.contactInfo.telNo;
            airfare.adtPaxs = airPaxModel.adtPaxs;
            for (int i = 0; i < airfare.adtPaxs.Count; i++)
            {
                if (Request["ADT_BD_" + i.ToString()] != null && Request["ADT_BD_" + i.ToString()].ToString().Length > 0)
                {
                    airfare.adtPaxs[i].birthday = DateTime.ParseExact(Request["ADT_BD_" + i.ToString()]
                            , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                if (Request["ADT_PI_" + i.ToString()] != null && Request["ADT_PI_" + i.ToString()].ToString().Length > 0)
                {
                    airfare.adtPaxs[i].passportIssuingDate = DateTime.ParseExact(Request["ADT_PI_" + i.ToString()], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                if (Request["ADT_PE_" + i.ToString()] != null && Request["ADT_PE_" + i.ToString()].ToString().Length > 0)
                {
                    airfare.adtPaxs[i].passportExpiryDate = DateTime.ParseExact(Request["ADT_PE_" + i.ToString()], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            if (airfare.noOfChildren > 0)
            {
                airfare.chdPaxs = airPaxModel.chdPaxs;
                for (int i = 0; i < airfare.chdPaxs.Count; i++)
                {
                    airfare.chdPaxs[i].birthday = DateTime.ParseExact(Request["CHD_BD_" + i.ToString()]
                            , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    if (Request["CHD_PI_" + i.ToString()] != null && Request["CHD_PI_" + i.ToString()].ToString().Length > 0)
                    {
                        airfare.chdPaxs[i].passportIssuingDate = DateTime.ParseExact(Request["CHD_PI_" + i.ToString()], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (Request["CHD_PE_" + i.ToString()] != null && Request["CHD_PE_" + i.ToString()].ToString().Length > 0)
                    {
                        airfare.chdPaxs[i].passportExpiryDate = DateTime.ParseExact(Request["CHD_PE_" + i.ToString()], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

            }
            if (airfare.noOfInfants > 0)
            {
                airfare.infPaxs = airPaxModel.infPaxs;
                for (int i = 0; i < airfare.infPaxs.Count; i++)
                {
                    airfare.infPaxs[i].birthday = DateTime.ParseExact(Request["INF_BD_" + i.ToString()]
                            , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    if (Request["INF_PI_" + i.ToString()] != null && Request["INF_PI_" + i.ToString()].ToString().Length > 0)
                    {
                        airfare.infPaxs[i].passportIssuingDate = DateTime.ParseExact(Request["INF_PI_" + i.ToString()], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (Request["INF_PE_" + i.ToString()] != null && Request["INF_PE_" + i.ToString()].ToString().Length > 0)
                    {
                        airfare.infPaxs[i].passportExpiryDate = DateTime.ParseExact(Request["INF_PE_" + i.ToString()], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }
            airfare.TKTL = DateTime.Now.AddHours(48);

            //Add New
            airfare.bookingDate = DateTime.Now;
            ViewBag.JSON = Newtonsoft.Json.JsonConvert.SerializeObject(airfare);

            airfare.bookingOID = officeID;
            airfare.remarks = new List<string>();
            airfare.remarks.Add("GOGOJII");
         
            if (Session["ssUUID"] != null)
            {
                airfare.uuid = (string)Session["ssUUID"];
            }

            if (Session["ssPlatform"] != null)
            {
                airfare.Platform = (string)Session["ssPlatform"];
            }
            else
            {
                airfare.Platform = "Web";
            }
            if(airfare.Platform == "Web")
            {
                airfare.sourceBy = 1;
            }
            else
            {
                airfare.sourceBy = 0;
            }
            airfare.type = "A";
            if(Request["needNFT"] !=null && Request["needNFT"] == "true" && Request["Wallet_Address"]!=null && Request["Wallet_Address"].Length>0)
            {
                airfare.Wallet_Address = Request["Wallet_Address"];
                airfare.NFTFee = Math.Ceiling((airfare.grandTotal * 3) / 100);
                airfare.grandTotal += airfare.NFTFee;
            }
            pnr = _flightSearchServices.Booking(airfare);
            if (pnr.isError)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = pnr.errorMessage });
            }
            else
            {
               
                Session["ssPricing"] = airfare;
                Session["ssPNR"] = pnr;
                //return RedirectToAction("Summary", "Flight");
                return RedirectToAction("Pay", "Payment", new { id = pnr.bookingKeyReference });
            }


        }

        public ActionResult Summary(BL.Entities.RobinhoodFare.AirFare booking)
        {
            if (Session["ssPricing"] == null || Session["ssPNR"] == null)
            {
                return RedirectToAction("SystemProblem", "Home", new { msg = "TIMEOUT" });
            }
            BL.Entities.RobinhoodFare.AirFare airfare = (BL.Entities.RobinhoodFare.AirFare)Session["ssPricing"];
            BL.Entities.RobinhoodPNR.PNR pnr = (BL.Entities.RobinhoodPNR.PNR)Session["ssPNR"];
            ViewBag.PNR = pnr.recordLocator;
            return View(airfare);
        }
        public ActionResult Voucher(Guid id)
        {
            BL.Entities.RobinhoodFare.AirFare airfare = _flightReportServices.GetByID(id);
            ViewBag.PNR = airfare.PNR;
            
            return View("Summary", airfare);
        }

        public ActionResult Email(Guid id)
        {
            if (!String.IsNullOrEmpty(Request["lang"]))
            {
                Localize.SetLang(Request["lang"].ToLower());
            }
            Log.Debug("lang="+ Localize.GetLang());
            BL.Entities.RobinhoodFare.AirFare airfare = _flightReportServices.GetByID(id);
            return View(airfare);
        }
        public JsonResult ResendEmail(string user_email, Guid id, string robinhoodID)
        {
            string response = "NOK";
            BL.Entities.RobinhoodFare.AirFare airfare = _flightReportServices.GetByID(id);
            bool bSend = _flightReportServices.ResendBookingEmail(id, "Booking Confirmation - " + robinhoodID, user_email);
            if (bSend)
            {
                response = "OK";
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Receipt(Guid id)
        {
            if (!String.IsNullOrEmpty(Request["lang"]))
            {
                Localize.SetLang(Request["lang"].ToLower());
            }
            
            BL.Entities.RobinhoodFare.AirFare airfare = _flightReportServices.GetByID(id);
            return PartialView(airfare);
        }
        public ActionResult ReceiptHeader(Guid id)
        {
            if (!String.IsNullOrEmpty(Request["lang"]))
            {
                Localize.SetLang(Request["lang"].ToLower());
            }
            BL.Entities.RobinhoodFare.AirFare airfare = _flightReportServices.GetByID(id);
            return PartialView(airfare);
        }
        public ActionResult ViewReceipt(Guid id)
        {
            if (!String.IsNullOrEmpty(Request["lang"]))
            {
                Localize.SetLang(Request["lang"].ToLower());
            }
            BL.Entities.RobinhoodFare.AirFare airfare = _flightReportServices.GetByID(id);
            //var pdfname = String.Format("Receipt_{0}.pdf", hotel.RobinhoodID);
            string customSwitches = string.Format("--header-html  \"{0}\" --footer-html \"{1}\" ", Url.Action("ReceiptHeader", "Flight", new { id = id }, Request.Url.Scheme), Url.Action("ReceiptFooter", "Hotel", null, Request.Url.Scheme));
            return new Rotativa.ViewAsPdf("Receipt", airfare)
            {
                //FileName = pdfname,
                CustomSwitches = customSwitches,
                PageMargins = new Rotativa.Options.Margins(100, 10, 20, 10),
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.A4
            };

        }
    

        
    }
}