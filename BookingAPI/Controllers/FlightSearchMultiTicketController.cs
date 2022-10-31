using BL;
using log4net;
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
using TGBookingAPI.Models;

namespace TGBookingAPI.Controllers
{
    public class FlightSearchMultiTicketController : ApiController
    {
        // GET api/<controller>
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly ISiteConfigServices _siteConfigServices;
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];
        private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public FlightSearchMultiTicketController(IFlightSearchServices flightSearchServices, INamingServices namingServices, IAPIConfigServices apiConfigServices, ISiteConfigServices siteConfigServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            this._apiConfigServices = apiConfigServices;
            this._siteConfigServices = siteConfigServices;
        }


        public HttpResponseMessage Post(SearchModel model)
        {
            Log.Debug("SEARCH SERVICE START");
            var statusCode = System.Net.HttpStatusCode.OK;
            BL.Entities.RobinhoodFlight.FlightSearchMultiTicketResult result = new BL.Entities.RobinhoodFlight.FlightSearchMultiTicketResult();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            string json = "";
            if (!isLogin)
            {
                json = "{\"isError\": true, \"type\":\"A\", \"errorCode\":\"1001\", \"errorMessage\": \"Invalid Account\"}";
                //statusCode = HttpStatusCode.Unauthorized;
            }
            else
            {
                BL.Entities.MasterPricer.Request request = model.GetMPSearchRequest();
                var dataConfig = _siteConfigServices.GetByKey("OfficeID");
                Log.Debug("dataConfig.ConfigValue=" + dataConfig.ConfigValue);
                request.bookingOID = dataConfig.ConfigValue;
                //request.bookingOID = officeID;
                result = this._flightSearchServices.SearchMultiTicket(request);

                if (result == null)
                {
                    result = new BL.Entities.RobinhoodFlight.FlightSearchMultiTicketResult();
                    //statusCode = HttpStatusCode.BadRequest;
                    json = "{\"isError\": true, \"type\":\"A\", \"errorCode\":\"2001\", \"errorMessage\": \"Flight not found\"}";
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
                                    detail.equipmentTypeName = Aircraft.Show(detail.equipmentType);
                                }
                            }
                        }

                        if (flight.Flight_SegRef2 != null)
                        {
                            foreach (var ret in flight.Flight_SegRef2)
                            {
                                foreach (var detail in ret.flightDetails)
                                {
                                    detail.equipmentTypeName = Aircraft.Show(detail.equipmentType);
                                }
                            }
                        }
                    }
                    json = result.toJsonV2(); //JsonConvert.SerializeObject(result);
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