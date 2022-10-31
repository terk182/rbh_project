using BL;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;
using TGBookingAPI.Models;

namespace TGBookingAPI.Controllers
{
    public class FlightMultiTicketPricingController : ApiController
    {
        // GET api/<controller>
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly ISiteConfigServices _siteConfigServices;
        private readonly ILionAirServices _lionAirServices;
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public FlightMultiTicketPricingController(IFlightSearchServices flightSearchServices, INamingServices namingServices, IAPIConfigServices apiConfigServices
            , ISiteConfigServices siteConfigServices, ILionAirServices lionAirServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            this._apiConfigServices = apiConfigServices;
            this._siteConfigServices = siteConfigServices;
            this._lionAirServices = lionAirServices;
        }
        [ResponseType(typeof(BL.Entities.RobinhoodFare.AirFare))]
        public HttpResponseMessage Post(SelectedFlightsMultiTicketModel model)
        {
            BL.Entities.RobinhoodFare.AirFare response = null;
            var header = Request.Headers;

            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                response = new BL.Entities.RobinhoodFare.AirFare();
                response.isError = true;
                response.errorCode = "1001";
                response.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.RobinhoodFare.AirFare>(response, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.OK//HttpStatusCode.Unauthorized
                };
                return unaut;
            }
            try
            {
                BL.Entities.RobinhoodFlight.FlightSearchMultiTicketResult result = null;
                var airSellReq = model.GetInformativePricingRequest();
                var airSellReq1A = model.GetInformativePricingRequestFor1A();
                var dataConfig = _siteConfigServices.GetByKey("OfficeID");
                Log.Debug("dataConfig.ConfigValue=" + dataConfig.ConfigValue);
                airSellReq.bookingOID = dataConfig.ConfigValue;
                airSellReq1A.bookingOID = dataConfig.ConfigValue;
                if (model.bMultiTicket_Fare && model.triptype.Equals("R"))
                {
                    //DataModel.FlightSearchMultiTicket flightSearchMultiTicket = _flightSearchServices.GetFlightSearchMulti(model.pgSearchOID);
                    //if (flightSearchMultiTicket != null)
                    //{
                    //result = JsonConvert.DeserializeObject<BL.Entities.RobinhoodFlight.FlightSearchMultiTicketResult>(flightSearchMultiTicket.FlightSearchResponse);
                    var airSellReqDepart1A = model.GetInformativePricingRequestFor1A(1);
                    Log.Debug("airSellReqDepart1A.depRefNumber=" + airSellReqDepart1A.depRefNumber);
                    Log.Debug("airSellReqDepart1A.depFlightType=" + airSellReqDepart1A.depFlightType);
                    airSellReqDepart1A.bookingOID = dataConfig.ConfigValue;
                    var airSellReqReturn1A = model.GetInformativePricingRequestFor1A(2);
                    Log.Debug("airSellReqReturn1A.depRefNumber=" + airSellReqReturn1A.depRefNumber);
                    Log.Debug("airSellReqReturn1A.depFlightType=" + airSellReqReturn1A.depFlightType);
                    airSellReqReturn1A.bookingOID = dataConfig.ConfigValue;
                    //BL.Entities.RobinhoodFlight.MultiTicketFlight flight1 = result.flights.Find(x => x.isMultiTicket == false && x.Flight_SegRef1.Find(s => s.refNumber == airSellReqDepart1A.depRefNumber) != null && x.Flight_SegRef2.Find(s => s.refNumber == airSellReqReturn1A.depRefNumber) != null);
                    //if (flight1 != null)
                    //{
                    //    Log.Debug("recommendation_number=" + flight1.recommendation_number);
                    //    response = _flightSearchServices.InformativePricing(airSellReq, airSellReq1A, model.languageCode);
                    //}
                    if (response == null)
                    {
                        Log.Debug("response == null");
                        if (airSellReq.depFlightType == airSellReq.retFlightType)
                        {
                            Log.Debug("airSellReq.depFlightType="+ airSellReq.depFlightType);
                            if (airSellReq.depFlightType == "A")
                            {
                                response = _flightSearchServices.InformativePricing(airSellReq, airSellReqDepart1A, airSellReqReturn1A, model.languageCode);
                            }
                            else if (airSellReq.depFlightType == "L")
                            {
                                response = _lionAirServices.AirSellService(airSellReq, airSellReq1A, model.languageCode);
                            }
                        }
                        else
                        {

                        }
                    }
                    foreach (var flight in response.depFlight)
                    {
                        flight.equipmentTypeName = Aircraft.Show(flight.equipmentType);
                    }
                    if (response.retFlight != null && response.retFlight.Count > 0)
                    {
                        foreach (var flight in response.retFlight)
                        {
                            flight.equipmentTypeName = Aircraft.Show(flight.equipmentType);
                        }
                    }
                    //}
                    //else
                    //{
                    //    response = new BL.Entities.RobinhoodFare.AirFare();
                    //    response.isError = true;
                    //    response.type = "A";//A:Amadeus
                    //    response.errorCode = "3002";
                    //    response.errorMessage = "Flight search result not found";
                    //}
                }
                else
                {
                    if (airSellReq.depFlightType == "A")
                    {
                        response = _flightSearchServices.InformativePricing(airSellReq, airSellReq1A, model.languageCode);
                    }
                    else
                    {
                        response = _lionAirServices.AirSellService(airSellReq, airSellReq1A, model.languageCode);
                    }

                    foreach (var flight in response.depFlight)
                    {
                        flight.equipmentTypeName = Aircraft.Show(flight.equipmentType);
                    }
                    if (response.retFlight != null && response.retFlight.Count > 0)
                    {
                        foreach (var flight in response.retFlight)
                        {
                            flight.equipmentTypeName = Aircraft.Show(flight.equipmentType);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(response.errorMessage))
                {
                    var resError = new HttpResponseMessage()
                    {
                        Content = new ObjectContent<BL.Entities.RobinhoodFare.AirFare>(response, DefaultJsonFormatter),
                        StatusCode = HttpStatusCode.OK//HttpStatusCode.InternalServerError
                    };
                    return resError;
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                response = new BL.Entities.RobinhoodFare.AirFare();
                response.isError = true;
                response.type = "A";//A:Amadeus
                response.errorCode = "3001";
                response.errorMessage = "Cannot pricing these flights";
            }

            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.RobinhoodFare.AirFare>(response, DefaultJsonFormatter),
                StatusCode = HttpStatusCode.OK//response.isError ? HttpStatusCode.BadRequest : HttpStatusCode.OK
            };
            return res;
        }
    }
}