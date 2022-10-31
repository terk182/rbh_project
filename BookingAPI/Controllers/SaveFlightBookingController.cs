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
using System.Web.Http.Description;

namespace TGBookingAPI.Controllers
{
    public class SaveFlightBookingController : ApiController
    {
        private static readonly ILog Log =
           LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFlightBookingServices _flightBookingServices;
        private readonly INamingServices _namingServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly IFlightReportServices _flightReportServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        // GET api/<controller>
        public SaveFlightBookingController(IFlightBookingServices flightBookingServices, INamingServices namingServices, IFlightReportServices flightReportServices, IAPIConfigServices apiConfigServices)
        {
            this._flightBookingServices = flightBookingServices;
            this._namingServices = namingServices;
            this._apiConfigServices = apiConfigServices;
            this._flightReportServices = flightReportServices;
        }

        //[ResponseType(typeof(BL.Entities.RobinhoodPNR.PNR))]
        public HttpResponseMessage Post(BL.Entities.RobinhoodFare.AirFare airfare)
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

            if (airfare != null && airfare.PNR!=null && airfare.PNR.Length>0)
            {
                Log.Debug("have airfare");
               
                airfare.Platform = "App";
                airfare.sourceBy = 0;
                Guid guid = _flightBookingServices.SaveBooking(airfare);
                if (guid != null && guid !=Guid.Empty)
                {
                    pnr.isError = false;
                    pnr.errorMessage = "";
                    pnr.recordLocator = airfare.PNR;
                    pnr.bookingKeyReference = guid.ToString();

                    
                }
                else
                {
                    pnr.isError = true;
                    pnr.errorMessage = "Cannot save data";
                }
            }
            else
            {
                Log.Debug("airfare null");
                pnr.isError = true;
                pnr.errorMessage = "PNR is null.";
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