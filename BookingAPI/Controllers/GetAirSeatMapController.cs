using BL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Http;
using TGBookingAPI.Models;

namespace TGBookingAPI.Controllers
{
    public class GetAirSeatMapController : ApiController
    {
        // GET api/<controller>
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public GetAirSeatMapController(IFlightSearchServices flightSearchServices, IAPIConfigServices apiConfigServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._apiConfigServices = apiConfigServices;
        }
        public HttpResponseMessage Post(SeatMapModel model)
        {
            var statusCode = System.Net.HttpStatusCode.OK;
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            string json = "";
            if (!isLogin)
            {
                json = "{\"isError\": true, \"errorMessage\": \"Invalid Account\"}";
                statusCode = HttpStatusCode.Unauthorized;
            }
            else
            {
                BL.Entities.SeatMap.Request request = new BL.Entities.SeatMap.Request();
                request.Flight = new BL.Entities.SeatMap.Flight();
                request.Flight.DepartureDateTime = model.DepartureDateTime;
                request.Flight.DepartureCity = model.DepartureCity;
                request.Flight.ArrivalCity = model.ArrivalCity;
                request.Flight.CompanyCode = model.CompanyCode;
                request.Flight.FlightNumber = model.FlightNumber;
                request.Flight.RbdCode = model.RbdCode;
                request.Session = new BL.Entities.SeatMap.Session();
                request.Session.IsStateFull = false;
                json = _flightSearchServices.AirSeatMap(request);
                BL.Entities.SeatMap.Response response = JsonConvert.DeserializeObject<BL.Entities.SeatMap.Response>(json);
                json = JsonConvert.SerializeObject(response);
            }
            HttpContext.Current.Response.Cache.VaryByHeaders["accept-encoding"] = true;

            var TheHTTPResponse = new HttpResponseMessage(statusCode);
            TheHTTPResponse.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return TheHTTPResponse;
        }
        }
}