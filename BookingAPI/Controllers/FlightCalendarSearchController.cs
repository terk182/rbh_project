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
    public class FlightCalendarSearchController : ApiController
    {
        // GET api/<controller>
        private readonly IFlightSearchServices _flightSearchServices;
        private readonly INamingServices _namingServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly ISiteConfigServices _siteConfigServices;
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];
        private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public FlightCalendarSearchController(IFlightSearchServices flightSearchServices, INamingServices namingServices, IAPIConfigServices apiConfigServices, ISiteConfigServices siteConfigServices)
        {
            this._flightSearchServices = flightSearchServices;
            this._namingServices = namingServices;
            this._apiConfigServices = apiConfigServices;
            this._siteConfigServices = siteConfigServices;
        }


        public HttpResponseMessage Post(CalendarSearchModel model)
        {
            var statusCode = System.Net.HttpStatusCode.OK;
            BL.Entities.RobinhoodFlight.FlightSearchResult result = new BL.Entities.RobinhoodFlight.FlightSearchResult();
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
                BL.Entities.MasterCalendar.Request request = model.GetMPSearchRequest();
                var dataConfig = _siteConfigServices.GetByKey("OfficeID");
                Log.Debug("dataConfig.ConfigValue=" + dataConfig.ConfigValue);
                request.bookingOID = dataConfig.ConfigValue;
                
                result = this._flightSearchServices.CalendarSearch(request);

                if (result == null)
                {
                    result = new BL.Entities.RobinhoodFlight.FlightSearchResult();
                    statusCode = HttpStatusCode.BadRequest;
                    json = "{\"isError\": true, \"errorMessage\": \"Flight not found\"}";
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