using BL;
using TGBookingAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;
using log4net;

namespace TGBookingAPI.Controllers
{
    public class FlightPricingController : ApiController
    {
        private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly ISiteConfigServices _siteConfigServices;
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public FlightPricingController(IFlightSearchServices flightSearchServices, INamingServices namingServices, IAPIConfigServices apiConfigServices, ISiteConfigServices siteConfigServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            this._apiConfigServices = apiConfigServices;
            this._siteConfigServices = siteConfigServices;
        }

        [ResponseType(typeof(BL.Entities.RobinhoodFare.AirFare))]
        public HttpResponseMessage Post(SelectedFlightsModel model)
        {
            BL.Entities.RobinhoodFare.AirFare response = new BL.Entities.RobinhoodFare.AirFare();
            var header = Request.Headers;

            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                response.isError = true;
                response.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.RobinhoodFare.AirFare>(response, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return unaut;
            }
            try
            {
                var airSellReq = model.GetInformativePricingRequest();
                var airSellReq1A = model.GetInformativePricingRequestFor1A();
                var dataConfig = _siteConfigServices.GetByKey("OfficeID");
                Log.Debug("dataConfig.ConfigValue=" + dataConfig.ConfigValue);
                airSellReq.bookingOID = dataConfig.ConfigValue;
                airSellReq1A.bookingOID = dataConfig.ConfigValue;
                //airSellReq.bookingOID = officeID;
                //airSellReq1A.bookingOID = officeID;
                response = _flightSearchServices.InformativePricing(airSellReq, airSellReq1A, model.languageCode);


                if (!String.IsNullOrEmpty(response.errorMessage))
                {
                    var resError = new HttpResponseMessage()
                    {
                        Content = new ObjectContent<BL.Entities.RobinhoodFare.AirFare>(response, DefaultJsonFormatter),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                    return resError;
                }
            }
            catch
            {
                response.isError = true;
                response.errorMessage = "Cannot pricing these flights";
            }

            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.RobinhoodFare.AirFare>(response, DefaultJsonFormatter),
                StatusCode = response.isError ? HttpStatusCode.BadRequest : HttpStatusCode.OK
            };
            return res;
        }
    }
}
