using BL;
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
    public class FlightResendEmailController : ApiController
    {
        private readonly IFlightReportServices _flightReportServices;
        private readonly IAPIConfigServices _apiConfigServices;

        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public FlightResendEmailController(IFlightReportServices flightReportServices,
             IAPIConfigServices apiConfigServices)
        {
            this._flightReportServices = flightReportServices;
            this._apiConfigServices = apiConfigServices;
        }
        [ResponseType(typeof(BL.Entities.ResendEmail.Response))]
        public HttpResponseMessage Post(BL.Entities.ResendEmail.Request request)
        {
            BL.Entities.ResendEmail.Response response = new BL.Entities.ResendEmail.Response();
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
                    Content = new ObjectContent<BL.Entities.ResendEmail.Response>(response, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return unaut;
            }
            string sSubject = "Booking Confirmation - " + request.bookingKeyReference;
            bool bSend = _flightReportServices.ResendBookingEmail(request.bookingKeyReference, sSubject, request.Email);
            if (bSend)
            {
                response.isError = false;
                response.errorMessage = "";
            }
            else
            {
                response.isError = true;
                response.errorCode = "7001";
                response.errorMessage = "Email could not be sent";
            }
            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.ResendEmail.Response>(response, DefaultJsonFormatter),
                StatusCode = response.isError ? HttpStatusCode.BadRequest : HttpStatusCode.OK
            };
            return res;
        }
    }
}