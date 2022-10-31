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
    }
}
