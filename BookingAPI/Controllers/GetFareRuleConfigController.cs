using BL;
using BL.Entities.FareRuleConfig;
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
    public class GetFareRuleConfigController : ApiController
    {
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly IFlightFareRuleConfigServices _flightFareRuleConfigServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public GetFareRuleConfigController(IFlightFareRuleConfigServices flightFareRuleConfigServices,
            IAPIConfigServices apiConfigServices)
        {
            this._flightFareRuleConfigServices = flightFareRuleConfigServices;
            this._apiConfigServices = apiConfigServices;
        }

        [ResponseType(typeof(BL.Entities.FareRuleConfig.Response))]
        public HttpResponseMessage Post(BL.Entities.FareRuleConfig.Request request)
        {
            BL.Entities.FareRuleConfig.Response fareRule = new BL.Entities.FareRuleConfig.Response();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                fareRule.isError = true;
                fareRule.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.FareRuleConfig.Response>(fareRule, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.OK//HttpStatusCode.Unauthorized
                };
                return unaut;
            }
            SegmentForGetFareRule model = new SegmentForGetFareRule();
            model.boardPoint = request.origin;
            model.offPoint = request.destination;
            model.marketingCompany = request.airlineCode;
            model.fareBasis = request.fareBasis;
            model.rbd = request.rbd;

            BL.Entities.RobinhoodFare.FareRule _fareRule = _flightFareRuleConfigServices.GetFareRuleConfig(model, request.languageCode);
            if (_fareRule != null)
            {
                fareRule.fareRule = _fareRule;
            }
            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.FareRuleConfig.Response>(fareRule, DefaultJsonFormatter),
                StatusCode = HttpStatusCode.OK//fareRule.isError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
            };
            return res;
        }
    }
}