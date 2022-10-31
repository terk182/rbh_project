using BL;
using log4net;
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
    public class LionAirFlightMatrixController : ApiController
    {
        private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET api/<controller>
        private readonly ILionAirServices _lionAirServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public LionAirFlightMatrixController(ILionAirServices lionAirServices, IAPIConfigServices apiConfigServices)
        {
            this._lionAirServices = lionAirServices;
            this._apiConfigServices = apiConfigServices;
        }
        public HttpResponseMessage Post(SearchModel model)
        {
            var statusCode = System.Net.HttpStatusCode.OK;
            BL.Entities.RobinhoodFlight.FlightSearchMultiTicketResult result = new BL.Entities.RobinhoodFlight.FlightSearchMultiTicketResult();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            string json = "";
            if (!isLogin)
            {
                json = "{\"isError\": true, \"type\":\"L\", \"errorCode\":\"1001\", \"errorMessage\": \"Invalid Account\"}";
                //statusCode = HttpStatusCode.Unauthorized;
            }
            else
            {
                BL.Entities.MasterPricer.Request request = model.GetMPSearchRequest();
                result = this._lionAirServices.Search(request);
                if (result == null)
                {
                    result = new BL.Entities.RobinhoodFlight.FlightSearchMultiTicketResult();
                    //statusCode = HttpStatusCode.BadRequest;
                    json = "{\"isError\": true, \"type\":\"L\", \"errorCode\":\"2001\", \"errorMessage\": \"Flight not found\"}";
                }
                else
                {
                    foreach (var flight in result.flights)
                    {
                        if (flight.Flight_SegRef1 != null)
                        {
                            foreach (var dep in flight.Flight_SegRef1)
                            {
                                foreach (var detail in dep.flightDetails)
                                {
                                    detail.equipmentType = Aircraft.GetCode(detail.equipmentTypeName);
                                }
                            }
                        }

                        if (flight.Flight_SegRef2 != null)
                        {
                            foreach (var ret in flight.Flight_SegRef2)
                            {
                                foreach (var detail in ret.flightDetails)
                                {
                                    detail.equipmentType = Aircraft.GetCode(detail.equipmentTypeName);
                                }
                            }
                        }
                    }
                    json = result.toJsonV2();
                }

            }
            HttpContext.Current.Response.Cache.VaryByHeaders["accept-encoding"] = true;

            var TheHTTPResponse = new HttpResponseMessage(statusCode);
            TheHTTPResponse.Content = new StringContent(json, Encoding.UTF8, "application/json");
            Log.Debug("SEARCH SERVICE END");
            return TheHTTPResponse;
        }
    }
}