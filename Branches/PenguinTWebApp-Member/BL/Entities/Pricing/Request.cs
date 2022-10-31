using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.Pricing
{
    public partial class Session
    {
        public bool isStateFull { get; set; }
        public bool InSeries { get; set; }
        public bool End { get; set; }
        public string SessionId { get; set; }
        public string SequenceNumber { get; set; }
        public string SecurityToken { get; set; }
    }

    public class Request
    {
        public List<string> pricingTicketingIndicators { get; set; }
        public bool useCityOverride { get; set; }
        public bool useCurrencyOverride { get; set; }
        public Session session { get; set; }
        public bool useBookingOfficeID { get; set; }
        public string validate_airlineCode { get; set; }
        public string sellingCity { get; set; }
        public string ticketingCity { get; set; }
        public string currencyCode { get; set; }
        public string ClientIP { get; set; }
        public string bookingOID { get; set; }
    }
}
