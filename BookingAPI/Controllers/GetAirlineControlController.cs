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
    public class GetAirlineControlController : ApiController
    {
        private readonly IAirlineControlServices _airlineControlServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        // GET api/<controller>
        public GetAirlineControlController(IAirlineControlServices airlineControlServices,
             IAPIConfigServices apiConfigServices)
        {
            this._airlineControlServices = airlineControlServices;
            this._apiConfigServices = apiConfigServices;
        }
        [ResponseType(typeof(BL.Entities.RobinhoodAirlineControl.AirlineControlResponse))]
        public HttpResponseMessage Get()
        {
            BL.Entities.RobinhoodAirlineControl.AirlineControlResponse response = new BL.Entities.RobinhoodAirlineControl.AirlineControlResponse();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                response.isError = true;
                response.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.RobinhoodAirlineControl.AirlineControlResponse>(response, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return unaut;
            }
            List<BL.Entities.RobinhoodAirlineControl.AirlineControl> airlineControls = null;
            List<DataModel.AirlineControl> airineList = _airlineControlServices.GetAll();
            BL.Entities.RobinhoodAirlineControl.AirlineControl control = new BL.Entities.RobinhoodAirlineControl.AirlineControl();
            if (airineList != null && airineList.Count > 0) {
                airlineControls = new List<BL.Entities.RobinhoodAirlineControl.AirlineControl>();
                foreach (var li in airineList)
                {
                    control = new BL.Entities.RobinhoodAirlineControl.AirlineControl();
                    control.AirlineOID = li.AirlineOID;
                    control.DestinationCountryCode = li.DestinationCountryCode;
                    control.OriginalCountryCode = li.OriginalCountryCode;
                    control.IsActive = li.IsActive;
                    control.airlines = _airlineControlServices.GetAllAirlineSub(control.AirlineOID);
                    airlineControls.Add(control);
                }
            }

            response.isError = false;
            response.errorMessage = "";
            response.airlineControls = airlineControls;
            var res = new HttpResponseMessage()
            {
                Content = new ObjectContent<BL.Entities.RobinhoodAirlineControl.AirlineControlResponse>(response, DefaultJsonFormatter),
                StatusCode = response.isError ? HttpStatusCode.BadRequest : HttpStatusCode.OK
            };
            return res;
        }
    }
}