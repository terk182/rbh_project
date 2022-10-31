using BL;
using DataModel;
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
    public class PaymentFeeController : ApiController
    {
        private readonly IPaymentServices _paymentServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public PaymentFeeController(IPaymentServices paymentServices,
             IAPIConfigServices apiConfigServices)
        {
            this._paymentServices = paymentServices;
            this._apiConfigServices = apiConfigServices;
        }

        [ResponseType(typeof(List<BL.Entities.PaymentFee.PaymentFeeResponse>))]
        public HttpResponseMessage Get()
        {
            BL.Entities.PaymentFee.PaymentFeeResponse response = new BL.Entities.PaymentFee.PaymentFeeResponse();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                response.isError = true;
                response.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.PaymentFee.PaymentFeeResponse>(response, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return unaut;
            }
            Payment productList = _paymentServices.GetAll();
            response.isError = false;
            response.errorMessage = "";
            response.paymentFeeList = productList;
            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.PaymentFee.PaymentFeeResponse>(response, DefaultJsonFormatter),
                StatusCode = HttpStatusCode.OK
            };
            return res;
        }
    }
}