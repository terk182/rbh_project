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
    public class GetAllFareRuleConfigController : ApiController
    {
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly IFlightFareRuleConfigServices _flightFareRuleConfigServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public GetAllFareRuleConfigController(IFlightFareRuleConfigServices flightFareRuleConfigServices,
            IAPIConfigServices apiConfigServices)
        {
            this._flightFareRuleConfigServices = flightFareRuleConfigServices;
            this._apiConfigServices = apiConfigServices;
        }

        [ResponseType(typeof(BL.Entities.FareRuleConfig.AllResponse))]
        public HttpResponseMessage Get()
        {
            BL.Entities.FareRuleConfig.AllResponse fareRule = new BL.Entities.FareRuleConfig.AllResponse();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                fareRule.isError = true;
                fareRule.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.FareRuleConfig.AllResponse>(fareRule, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.OK//HttpStatusCode.Unauthorized
                };
                return unaut;
            }

            var allFareRule = _flightFareRuleConfigServices.GetAllConfig();
            if (allFareRule != null)
            {
                fareRule.fareRules = allFareRule;
            }
            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.FareRuleConfig.AllResponse>(fareRule, DefaultJsonFormatter),
                StatusCode = HttpStatusCode.OK//fareRule.isError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
            };
            return res;
        }
    }
}