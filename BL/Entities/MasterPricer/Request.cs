using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.MasterPricer
{
    public class Flight
    {
        public string arrivalCity { get; set; }
        public DateTime arrivalDateTime { get; set; }
        public string departureCity { get; set; }
        public DateTime departureDateTime { get; set; }
    }

    public class Request
    {
        public Guid pgSearchOID { get; set; }
        public List<string> airlineCode { get; set; }
        public List<string> preferAirlineCode { get; set; }
        public List<string> blockAirlineCode { get; set; }
        public string cabinClass { get; set; }
        public string corporateContractNumber { get; set; }
        public string currencyCode { get; set; }
        public bool directFlight { get; set; }
        public int flexibleDate { get; set; }
        public List<Flight> flights { get; set; }
        public bool isPriceTypeRP { get; set; }
        public bool isPriceTypeRU { get; set; }
        public int noOfAdults { get; set; }
        public int noOfChildren { get; set; }
        public int noOfInfants { get; set; }
        public int noOfRecommendation { get; set; }
        public bool nonstopFlight { get; set; }
        public string sellingCity { get; set; }
        public string ticketingCity { get; set; }
        public bool useInterlineAgreement { get; set; }
        public string languageCode { get; set; }
        public string bookingOID { get; set; }

        public int inFlightServices { get; set; }

        public string userEmail { get; set; }
        public List<FareFamilyGroup> FareFamilyGroup { get; set; }
        public bool multiTicket { get; set; }
        public multiTicketOption multiTicketOption { get; set; }
        public fareFamilyInfoOption fareFamilyInfoOption { get; set; }
        public upsellOption upsellOption { get; set; }
    }
    public class FareFamilyGroup
    {
        public string FareFamilyname { get; set; }

        public int Hierarchy { get; set; }

        public string CarrierId { get; set; }

        public string FareFamilyQual { get; set; }
        public List<string> FareBasis { get; set; }

        public List<string> FareType { get; set; }
    }
    public class multiTicketOption
    {
        public bool enable { get; set; }
        public int outboundWeight { get; set; }
        public int inboundWeight { get; set; }
        public int roundtripWeight { get; set; }
    }
    public class fareFamilyInfoOption
    {
        public bool enable { get; set; }
        public string feeType { get; set; }//default เป็น "FFI"
        public string feeIdNumber { get; set; } //default เป็น "3"
    }
    public class upsellOption
    {
        public bool enable { get; set; }
        public string feeType { get; set; }//default เป็น "UP"
        public string feeIdNumber { get; set; } //default เป็น "6"
    }
}
