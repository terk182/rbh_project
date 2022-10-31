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
    public class TokenController : ApiController
    {
        private readonly IAPIConfigServices _apiConfigServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public TokenController(IAPIConfigServices apiConfigServices)
        {
            this._apiConfigServices = apiConfigServices;
        }

        [ResponseType(typeof(BL.Entities.APILogin.Response))]
        public HttpResponseMessage Post(BL.Entities.APILogin.Request model)
        {
            BL.Entities.APILogin.Response response = _apiConfigServices.Login(model,
                ConfigurationManager.AppSettings["WEBMODE"]);
            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.APILogin.Response>(response, DefaultJsonFormatter),
                StatusCode = response.isError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
            };
            return res;
        }
    }
}
