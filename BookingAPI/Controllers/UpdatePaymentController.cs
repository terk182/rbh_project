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
using DataModel;
using Newtonsoft.Json;
using log4net;

namespace TGBookingAPI.Controllers
{
    public class UpdatePaymentController : ApiController
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly IPaymentServices _paymentServices;
        private readonly IFlightReportServices _flightReportServices;
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public UpdatePaymentController(IFlightSearchServices flightSearchServices, INamingServices namingServices, IAPIConfigServices apiConfigServices
            , IPaymentServices paymentServices, IFlightReportServices flightReportServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            this._apiConfigServices = apiConfigServices;
            this._paymentServices = paymentServices;
            this._flightReportServices = flightReportServices;
        }

        [ResponseType(typeof(BL.Entities.UpdatePayment.Response))]
        public HttpResponseMessage Post(BL.Entities.UpdatePayment.Request request)
        {
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            BL.Entities.UpdatePayment.Response result = new BL.Entities.UpdatePayment.Response();
            if (!isLogin)
            {
                result.isError = true;
                result.errorCode = "1001";
                result.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.UpdatePayment.Response>(result, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.OK//HttpStatusCode.Unauthorized
                };
                return unaut;
            }
            List<string> PNR = new List<string>();
            string errMsg = "";
            bool bResult = _flightSearchServices.UpdateOutsidePaymentStatus(request,ref PNR, ref errMsg);
            if (bResult)
            {
                result.bookingKeyReference = request.bookingKeyReference;
                result.recordLocator = PNR;
                result.isError = false;
                result.type = "A";//A:Amadeus
                result.errorMessage = "";
            }
            else
            {
                result.isError = true;
                result.type = "A";//A:Amadeus
                result.errorCode = "5001";
                result.errorMessage = errMsg.Length > 0 ? errMsg : "Cannot update status on booking";
            }
            try
            {
                PaymentLog paymentLog = new PaymentLog();
                paymentLog.PaymentLogOID = Guid.NewGuid();
                paymentLog.RobinhoodID = request.bookingKeyReference;
                paymentLog.LogDateTime = DateTime.Now;
                paymentLog.LogRequest = JsonConvert.SerializeObject(request);
                paymentLog.LogResponse = JsonConvert.SerializeObject(result);
                _paymentServices.SavePaymentLog(paymentLog);

                if (bResult && request.paymentStatus == 1)//send email
                {
                    _flightReportServices.SendBookingEmail(request.bookingKeyReference, "Booking Confirmation - " + request.bookingKeyReference, "th");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.UpdatePayment.Response>(result, DefaultJsonFormatter),
                StatusCode = HttpStatusCode.OK//result.isError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
            };
            return res;
        }
    }
}
