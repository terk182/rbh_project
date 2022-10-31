using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TGBookingAPI.Controllers
{
    public class RemoveFlightSearchMultiTicketController : ApiController
    {
        // GET api/<controller>
        private readonly IFlightSearchServices _flightSearchServices;
        public RemoveFlightSearchMultiTicketController(IFlightSearchServices flightSearchServices)
        {
            this._flightSearchServices = flightSearchServices;
        }
        public HttpResponseMessage Get()
        {
            bool bRemove = this._flightSearchServices.RemoveFlightSearchMultiTicket();
            var res = new HttpResponseMessage()
            {
                StatusCode = bRemove ? HttpStatusCode.OK: HttpStatusCode.InternalServerError
            };
            return res;
        }
    }
}