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
    public class CancelBookingController : ApiController
    {
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly IFlightBookingServices _flightBookingServices;
        private readonly IFlightReportServices _flightReportServices;
        private readonly ISiteConfigServices _siteConfigServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];
        private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public CancelBookingController(IFlightSearchServices flightSearchServices, IFlightBookingServices flightBookingServices, IFlightReportServices flightReportServices
            , IAPIConfigServices apiConfigServices, ISiteConfigServices siteConfigServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._flightBookingServices = flightBookingServices;
            this._flightReportServices = flightReportServices;
            this._apiConfigServices = apiConfigServices;
            this._siteConfigServices = siteConfigServices;
        }
        public HttpResponseMessage Post(Models.GetFlightBookingModel model)
        {
            BL.Entities.PNRCancel.Response response = new BL.Entities.PNRCancel.Response();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                response.isError = true;
                response.errorCode = "1001";
                response.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.PNRCancel.Response>(response, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.OK//HttpStatusCode.Unauthorized
                };
                return unaut;
            }

            List<DataModel.FlightBooking> flightBookings = _flightBookingServices.GetFlightBookings(model.bookingKeyReference);
            if (flightBookings != null && flightBookings.Count > 0)
            {
                var dataConfig = _siteConfigServices.GetByKey("OfficeID_Save");
                Log.Debug("dataConfig.ConfigValue=" + dataConfig.ConfigValue);
                officeID = dataConfig.ConfigValue;

                response.bookingKeyReference = model.bookingKeyReference;
                response.recordLocator = new List<string>();
                foreach (var _flightBooking in flightBookings)
                {
                    BL.Entities.RobinhoodFare.AirFare pnr = _flightReportServices.GetByID(_flightBooking.BookingOID);
                    if (pnr != null && pnr.PNR != null && pnr.PNR.Length > 0)
                    {
                        BL.Entities.PNRCancel.Request request = new BL.Entities.PNRCancel.Request();
                        request.session = new BL.Entities.PNRCancel.Session();
                        request.session.isStateFull = true;
                        request.session.InSeries = false;
                        request.session.End = false;
                        request.session.SessionId = "";
                        request.session.SequenceNumber = "0";
                        request.session.SecurityToken = "";
                        request.entryType = "I";
                        request.optionCode = "0";
                        request.bookingOID = officeID;
                        request.useBookingOfficeID = false;
                        request.pnrNumber = pnr.PNR;
                        request.ClientIP = BL.Utilities.HttpUtility.GetIPAddress();
                        bool bResult = _flightSearchServices.RetrieveAndCancel(request);
                        if (bResult)
                        {
                            _flightBookingServices.UpdateBookingStatus(_flightBooking.BookingOID, 3);
                            response.recordLocator.Add(pnr.PNR);
                        }

                    }
                }//foreach
            }
            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.PNRCancel.Response>(response, DefaultJsonFormatter),
                StatusCode = HttpStatusCode.OK//response.isError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
            };
            return res;
        }

    }
}