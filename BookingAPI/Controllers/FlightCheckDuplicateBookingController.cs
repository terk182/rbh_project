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

namespace TGBookingAPI.Controllers
{
    public class FlightCheckDuplicateBookingController : ApiController
    {
        // GET api/<controller>
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly ISiteConfigServices _siteConfigServices;
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];
        private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public FlightCheckDuplicateBookingController(IFlightSearchServices flightSearchServices, INamingServices namingServices, IAPIConfigServices apiConfigServices, ISiteConfigServices siteConfigServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            this._apiConfigServices = apiConfigServices;
            this._siteConfigServices = siteConfigServices;
        }
        public HttpResponseMessage Post(BL.Entities.RobinhoodFare.AirFare airfare)
        {
            if (airfare == null)
            {
                Log.Debug("airfare==null");
            }
            BL.Entities.RobinhoodPNR.PNR pnr = new BL.Entities.RobinhoodPNR.PNR();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                pnr.isError = true;
                pnr.type = "A";//Amadeus
                pnr.errorCode = "1001";
                pnr.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.RobinhoodPNR.PNR>(pnr, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.OK//HttpStatusCode.Unauthorized
                };
                return unaut;
            }
            var dataConfig = _siteConfigServices.GetByKey("OfficeID_Save");
            Log.Debug("dataConfig.ConfigValue=" + dataConfig.ConfigValue);
            airfare.bookingOID = dataConfig.ConfigValue;
            if (airfare.remarks == null)
            {
                airfare.remarks = new List<string>();
            }

            airfare.remarks.Add("ROBINHOOD");
            airfare.Platform = "API";

            airfare.medId = 0;
            airfare.TKTL = DateTime.Now.AddDays(2);
            airfare.bookingDate = DateTime.Now;
            if (airfare.isPricingWithSegment)
            {
                BL.Entities.RobinhoodPNR.MultiTicketPNR ticketPNR = _flightSearchServices.BookingMultiTicket(airfare);
                var res = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.RobinhoodPNR.MultiTicketPNR>(ticketPNR, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.OK//ticketPNR.isError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
                };
                return res;
            }
            else
            {
                pnr = _flightSearchServices.CheckDuplicateBooking(airfare);
                BL.Entities.RobinhoodPNR.MultiTicketPNR ticketPNR = new BL.Entities.RobinhoodPNR.MultiTicketPNR();
                ticketPNR.RobinhoodID = pnr.RobinhoodID;
                ticketPNR.isError = pnr.isError;
                ticketPNR.type = pnr.type;
                ticketPNR.errorCode = pnr.errorCode;
                ticketPNR.errorMessage = pnr.errorMessage;
                ticketPNR.airSellEntities = pnr.airSellEntities;
                if (pnr.recordLocator != null && pnr.recordLocator.Length > 0)
                {
                    ticketPNR.Booking = new List<BL.Entities.RobinhoodPNR.Booking>();
                    BL.Entities.RobinhoodPNR.Booking booking = new BL.Entities.RobinhoodPNR.Booking();
                    booking.recordLocator = pnr.recordLocator;
                    booking.bookingKeyReference = pnr.bookingKeyReference;
                    ticketPNR.Booking.Add(booking);
                }
                else
                {
                    ticketPNR.Booking = null;
                }
                var res = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.RobinhoodPNR.MultiTicketPNR>(ticketPNR, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.OK//ticketPNR.isError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
                };
                return res;
            }
        }
    }
}