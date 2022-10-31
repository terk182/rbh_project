using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.BookingTransaction
{
    public class BookingPassenger
    {
        public string SeatRequestId { get; set; }
        public bool HaveInfant { get; set; }
        public string SeatNumber { get; set; }
        public string FrequentFlyerAirline { get; set; }
        public string FoodRequestId { get; set; }
        public string InsureNo { get; set; }
        public bool WithInsure { get; set; }
        public string FirstName { get; set; }
        public string TicketID { get; set; }
        public string MiddleName { get; set; }
        public bool SeatRequest { get; set; }
        public int WithADT { get; set; }
        public string PassportNumber { get; set; }
        public string PaxType { get; set; }
        public string TitleName { get; set; }
        public int PTNo { get; set; }
        public string FrequentFlyerNumber { get; set; }
        public int PaxNo { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
    }

    public class BookingFlight
    {
        public string ToCityFullName { get; set; }
        public string CabinClass { get; set; }
        public string ArrivalDateTime { get; set; }
        public string FromAirportCode { get; set; }
        public string ToCity { get; set; }
        public string FromAirportFullName { get; set; }
        public string FareBasis { get; set; }
        public string FareType { get; set; }
        public string TripType { get; set; }
        public string FromCityFullName { get; set; }
        public string OperateAirline { get; set; }
        public string DepartureDateTime { get; set; }
        public string Duration { get; set; }
        public string ToAirportCode { get; set; }
        public string AircraftType { get; set; }
        public string MarketAirline { get; set; }
        public int routeno { get; set; }
        public int SegmentNo { get; set; }
        public string RBD { get; set; }
        public int TerminalId { get; set; }
        public string ToAirportFullName { get; set; }
        public string FromCity { get; set; }
        public string FlightNo { get; set; }
    }

    public class BookingPaxFare
    {
        public int NoOfPax { get; set; }
        public string Weight { get; set; }
        public string WeightUnits { get; set; }
        public string Currency { get; set; }
        public int FareWithTax { get; set; }
        public int FareWithoutTax { get; set; }
        public string PaxType { get; set; }
        public int Tax { get; set; }
    }

    public class Request
    {
        public List<BookingPassenger> BookingPassenger { get; set; }
        public string PNR { get; set; }
        public string Penalties1 { get; set; }
        public string ContactDestPhone { get; set; }
        public int TotalAmount { get; set; }
        public int PNRStatus { get; set; }
        public int NoOfPax { get; set; }
        public string FareFamily { get; set; }
        public string ShortFareCondition { get; set; }
        public string TripType { get; set; }
        public int Discount { get; set; }
        public int TotalCHD { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMiddleName { get; set; }
        public int TicketCounter { get; set; }
        public string Currency { get; set; }
        public int InsureQuoteTotal { get; set; }
        public string SelectedLanguage { get; set; }
        public string ContactLastName { get; set; }
        public string ContactAddress { get; set; }
        public string Penalties2 { get; set; }
        public int TotalADT { get; set; }
        public int PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string ContactFirstName { get; set; }
        public List<BookingFlight> BookingFlight { get; set; }
        public int PaymentChargePercent { get; set; }
        public int medId { get; set; }
        public int PaymentCharge { get; set; }
        public string ContactCountry { get; set; }
        public int InsureQuotePerPax { get; set; }
        public List<BookingPaxFare> BookingPaxFare { get; set; }
        public string PaymentDateTime { get; set; }
        public string ContactPhone { get; set; }
        public int TotalINF { get; set; }
        public int SendMailNotify { get; set; }
        public string IsHappyHour { get; set; }
        public int TicketStatus { get; set; }
        public string PaymentOrderID { get; set; }
        public string ContactPostCode { get; set; }
        public string SelectedFareFamily { get; set; }
        public string ContactCity { get; set; }
        public bool IsSubscribeEmail { get; set; }
        public string ContactTitleName { get; set; }
    }
}
