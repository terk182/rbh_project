using BL;
using DataModel;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TGBookingAPI.Controllers
{
    public class FlightController : Controller
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFlightReportServices _flightReportServices;
        private readonly IFlightBookingServices _flightBookingServices;
        public FlightController(IFlightReportServices flightReportServices, IFlightBookingServices flightBookingServices)
        {
            this._flightReportServices = flightReportServices;
            this._flightBookingServices = flightBookingServices;
        }
        // GET: Flight
        public ActionResult Email(string robinhoodID)
        {
            List<FlightBooking> flightBookingList = _flightBookingServices.GetFlightBookings(robinhoodID);
            List<BL.Entities.RobinhoodFare.AirFare> airFares = new List<BL.Entities.RobinhoodFare.AirFare>();
            BL.Entities.RobinhoodFare.AirFare airfare  = null;
            if (flightBookingList != null && flightBookingList.Count>0)
            {
                foreach (var _booking in flightBookingList)
                {
                    airfare = _flightReportServices.GetByID(_booking.BookingOID);
                    airFares.Add(airfare);
                }
            }
            return View(airFares);
        }
    }
}