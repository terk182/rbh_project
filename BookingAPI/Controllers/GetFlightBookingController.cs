using BL;
using DataModel;
using TGBookingAPI.Models;
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
    public class GetFlightBookingController : ApiController
    {
        private readonly IFlightReportServices _flightReportServices;
        private readonly IAPIConfigServices _apiConfigServices;
        private readonly IFlightBookingServices _flightBookingServices;
        private string officeID = ConfigurationManager.AppSettings["OfficeID"];
        private static readonly MediaTypeFormatter DefaultJsonFormatter = new JsonMediaTypeFormatter();
        public GetFlightBookingController(IFlightReportServices flightReportServices, IFlightBookingServices flightBookingServices, 
            IAPIConfigServices apiConfigServices)
        {
            this._flightBookingServices = flightBookingServices;
            this._flightReportServices = flightReportServices;
            this._apiConfigServices = apiConfigServices;
        }

        [ResponseType(typeof(BL.Entities.RobinhoodFare.AirFare))]
        public HttpResponseMessage Post(Models.GetFlightBookingModel model)
        {
            BL.Entities.RobinhoodFare.AirFare pnr = new BL.Entities.RobinhoodFare.AirFare();
            var header = Request.Headers;
            string webmode = ConfigurationManager.AppSettings["WEBMODE"];
            bool isLogin = HeaderAuth.Authorize(header, webmode, _apiConfigServices);

            if (!isLogin)
            {
                pnr.isError = true;
                pnr.errorCode = "1001";
                pnr.errorMessage = "Invalid Account";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.RobinhoodFare.AirFare>(pnr, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.OK//HttpStatusCode.Unauthorized
                };
                return unaut;
            }
            List<DataModel.FlightBooking> flightBookings = _flightBookingServices.GetFlightBookings(model.bookingKeyReference);
            if (flightBookings != null)
            {
                if (flightBookings.Count > 1)
                {
                    List<BL.Entities.RobinhoodFare.AirFare> pnrList = new List<BL.Entities.RobinhoodFare.AirFare>();
                    foreach (var _flightBooking in flightBookings)
                    {
                        pnr = _flightReportServices.GetByID(_flightBooking.BookingOID);
                        foreach (var flight in pnr.depFlight)
                        {
                            flight.equipmentTypeName = Aircraft.Show(flight.equipmentType);
                        }
                        if (pnr.retFlight != null && pnr.retFlight.Count > 0)
                        {
                            foreach (var flight in pnr.retFlight)
                            {
                                flight.equipmentTypeName = Aircraft.Show(flight.equipmentType);
                            }
                        }
                        pnrList.Add(pnr);
                    }
                    var res = new HttpResponseMessage()
                    {
                        Content = new ObjectContent<List<BL.Entities.RobinhoodFare.AirFare>>(pnrList, DefaultJsonFormatter),
                        StatusCode = HttpStatusCode.OK//pnr.isError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
                    };
                    return res;
                }
                else
                {
                    List<BL.Entities.RobinhoodFare.AirFare> pnrList = new List<BL.Entities.RobinhoodFare.AirFare>();
                    pnr = _flightReportServices.GetByID(flightBookings[0].BookingOID);
                    foreach (var flight in pnr.depFlight)
                    {
                        flight.equipmentTypeName = Aircraft.Show(flight.equipmentType);
                    }
                    if (pnr.retFlight != null && pnr.retFlight.Count > 0)
                    {
                        foreach (var flight in pnr.retFlight)
                        {
                            flight.equipmentTypeName = Aircraft.Show(flight.equipmentType);
                        }
                    }
                    pnrList.Add(pnr);
                    var res = new HttpResponseMessage()
                    {
                        Content = new ObjectContent<List<BL.Entities.RobinhoodFare.AirFare>>(pnrList, DefaultJsonFormatter),
                        StatusCode = HttpStatusCode.OK//pnr.isError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK
                    };
                    return res;
                }
            }
            else
            {
                pnr.isError = true;
                pnr.type = "A";//A:Amadeus
                pnr.errorCode = "6001";
                pnr.errorMessage = "Invalid bookingKeyReference";
                var unaut = new HttpResponseMessage()
                {
                    Content = new ObjectContent<BL.Entities.RobinhoodFare.AirFare>(pnr, DefaultJsonFormatter),
                    StatusCode = HttpStatusCode.OK//HttpStatusCode.Unauthorized
                };
                return unaut;

            }
        }
    }
}
