using BL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using TGBookingAPI;
using TGBookingAPI.Models;

namespace TGBookingAPI.Controllers
{
    public class FlightSearchAvailabilityController : ApiController
    {
        // GET api/<controller>
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];

        public FlightSearchAvailabilityController(IFlightSearchServices flightSearchServices, INamingServices namingServices, IAPIConfigServices apiConfigServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            this._apiConfigServices = apiConfigServices;
        }


        public HttpResponseMessage Post(SearchModel model)
        {
            var statusCode = System.Net.HttpStatusCode.OK;
            BL.Entities.AirMultiAvailability.FlightSearchANResult result = new BL.Entities.AirMultiAvailability.FlightSearchANResult();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            string json = "";
            if (!isLogin)
            {
                json = "{\"isError\": true, \"errorCode\":\"1001\", \"errorMessage\": \"Invalid Account\"}";
                statusCode = HttpStatusCode.Unauthorized;
            }
            else
            {
                BL.Entities.AirMultiAvailability.Request request = model.GetANSearchRequest();
                request.bookingOID = officeID;
                result = this._flightSearchServices.SearchAN(request);

                if (result == null)
                {
                    result = new BL.Entities.AirMultiAvailability.FlightSearchANResult();
                    statusCode = HttpStatusCode.BadRequest;
                    json = "{\"isError\": true, \"type\":\"A\", \"errorCode\":\"2001\", \"errorMessage\": \"Flight not found\"}";
                }                
                else
                {
                    json = JsonConvert.SerializeObject(result);
                }
            }

            HttpContext.Current.Response.Cache.VaryByHeaders["accept-encoding"] = true;

            var TheHTTPResponse = new HttpResponseMessage(statusCode);
            TheHTTPResponse.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return TheHTTPResponse;
        }
    }
}