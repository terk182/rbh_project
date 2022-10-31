using BL.Entities.CitySearch;
using BL.Entities.RobinhoodFlight;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IFlightSearchServices
    {
        CityList SearchCities(string keyword, string language, int numberOfList);
        void AddLog(FlightSearchLog log);
        FlightSearchResult Search(Entities.MasterPricer.Request request);
        FlightSearchMultiTicketResult SearchMultiTicket(Entities.MasterPricer.Request request);
        FlightSearchResult CalendarSearch(Entities.MasterCalendar.Request request);
        Entities.AirMultiAvailability.FlightSearchANResult SearchAN(Entities.AirMultiAvailability.Request request);
        Entities.RobinhoodFare.AirFare InformativePricing(Entities.InformativePricing.Request request, Entities.InformativePricing.Request requestFor1A, string languageCode);
        Entities.RobinhoodFare.AirFare InformativePricing(Entities.InformativePricing.Request request, Entities.InformativePricing.Request requestForDepart1A, Entities.InformativePricing.Request requestForReturn1A, string languageCode);
        Entities.RobinhoodFare.AirFare GhostPricing(Entities.AirSell.Request request, string languageCode);
        Entities.RobinhoodPNR.PNR Booking(Entities.RobinhoodFare.AirFare request);
        //Entities.RobinhoodPNR.PNR BookingBundle(Entities.RobinhoodFare.AirFare request);
        Entities.RobinhoodPNR.MultiTicketPNR BookingMultiTicket(Entities.RobinhoodFare.AirFare request);
        Entities.RobinhoodPNR.PNR BookingFromAN(Entities.AirMultiAvailability.SavePNRRequest request);
        void UpdatePaymentStatus(Entities.RobinhoodPNR.PNR pnr, int paymentType);
        void UpdateBookingStatus(string bookingKeyRef, int bookingStatus, string access_token);
        bool UpdateOutsidePaymentStatus(Entities.UpdatePayment.Request request, ref List<string> PNR, ref string errMsg);
        BL.Entities.PNR.Response Retrieve(string pnr, string officeID);
        bool RetrieveAndAddRemark(string pnr, string officeID, string AgencyName, List<string> remarks, bool isTKOK, ref string errMsg);
        bool RetrieveAndAddRemark(string pnr, Guid bookingOID, string transactionID, string officeID, string AgencyName, string bookingURN, List<string> remarks, bool isTKOK, ref string errMsg);
        //BL.Entities.PNR.PnrAndPricing RetrieveAndPricing(string pnr, string officeID, BL.Entities.MyTrip.BookingTransectionElement trip);
        void PNRCancel(Entities.PNRCancel.Request request);
        bool RetrieveAndCancel(Entities.PNRCancel.Request request);
        void MemberAutoCancel(string PNR, string bookingID, string userToken);
        FlightBooking GetByID(Guid id);
        void SaveOrUpdate(FlightBooking paxinfo);
        FlightSearchMultiTicket GetFlightSearchMulti(Guid pgSearchOID);
        bool RemoveFlightSearchMultiTicket();
        string AirSeatMap(Entities.SeatMap.Request request);

        Entities.RobinhoodPNR.PNR CheckDuplicateBooking(Entities.RobinhoodFare.AirFare request);
        Entities.RobinhoodPNR.MultiTicketPNR CheckDuplicateBookingMultiTicket(Entities.RobinhoodFare.AirFare request);
    }
}
