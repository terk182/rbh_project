using BL;
using DataModel;
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

namespace TGBookingAPI.Controllers
{
    public class ExchangeRateController : ApiController
    {
        private readonly ICurrencyExchange _fxServices;

        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public ExchangeRateController(ICurrencyExchange fxServices)
        {
            this._fxServices = fxServices;
        }

        [ResponseType(typeof(List<BL.Entities.CurrencyExchange.FXRate>))]
        public HttpResponseMessage Get()
        {
            List<BL.Entities.CurrencyExchange.FXRate> response = new List<BL.Entities.CurrencyExchange.FXRate>();
            response = _fxServices.FetchRate();

            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<List<BL.Entities.CurrencyExchange.FXRate>>(response, DefaultJsonFormatter),
                StatusCode = HttpStatusCode.OK
            };
            return res;
        }
    }
}
