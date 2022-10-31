using BL;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using TGBookingAPI;

namespace TGBookingAPI.Controllers
{
    public class SaveFlightBookingFromANController : ApiController
    {
        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public SaveFlightBookingFromANController(IFlightSearchServices flightSearchServices, INamingServices namingServices, IAPIConfigServices apiConfigServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            this._apiConfigServices = apiConfigServices;
        }
        public HttpResponseMessage Post(BL.Entities.AirMultiAvailability.SavePNRRequest request)
        {
            BL.Entities.RobinhoodPNR.PNR pnr = new BL.Entities.RobinhoodPNR.PNR();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                pnr.isError = true;
                pnr.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.RobinhoodPNR.PNR>(pnr, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return unaut;
            }

            if (request != null && (request.PNR == null || (request.PNR != null && request.PNR.Length == 0)))
            {
                pnr = _flightSearchServices.BookingFromAN(request);
            }
            else
            {
                pnr.isError = true;
                pnr.errorMessage = "PNR create alredy";
            }

            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.RobinhoodPNR.PNR>(pnr, DefaultJsonFormatter),
                StatusCode = pnr.isError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
            };
            return res;
        }
    }
}