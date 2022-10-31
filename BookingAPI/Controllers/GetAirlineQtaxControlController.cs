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
    public class GetAirlineQtaxControlController : ApiController
    {
        private readonly IAirlineQtaxControlServices _airlineQtaxControlServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        // GET api/<controller>
        public GetAirlineQtaxControlController(IAirlineQtaxControlServices airlineQtaxControlServices,
             IAPIConfigServices apiConfigServices)
        {
            this._airlineQtaxControlServices = airlineQtaxControlServices;
            this._apiConfigServices = apiConfigServices;
        }
        [ResponseType(typeof(BL.Entities.RobinhoodAirlineQtaxControl.AirlineQtaxControlResponse))]
        public HttpResponseMessage Get()
        {
            BL.Entities.RobinhoodAirlineQtaxControl.AirlineQtaxControlResponse response = new BL.Entities.RobinhoodAirlineQtaxControl.AirlineQtaxControlResponse();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                response.isError = true;
                response.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.RobinhoodAirlineQtaxControl.AirlineQtaxControlResponse>(response, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return unaut;
            }

            List<DataModel.AirlineQtaxControl> airineList = _airlineQtaxControlServices.GetAll();
            response.isError = false;
            response.errorMessage = "";
            response.airlineQtaxControls = airineList;
            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.RobinhoodAirlineQtaxControl.AirlineQtaxControlResponse>(response, DefaultJsonFormatter),
                StatusCode = response.isError ? HttpStatusCode.BadRequest : HttpStatusCode.OK
            };
            return res;
        }
    }
}