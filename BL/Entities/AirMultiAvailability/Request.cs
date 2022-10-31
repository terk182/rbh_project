using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.AirMultiAvailability
{
    public class Request
    {
        public Guid pgSearchOID { get; set; }
        public string tripType { get; set; }
        public List<string> airlineCode { get; set; }
        public string cabinClass { get; set; }
        public string anRoute { get; set; }//BOTH, DEP, RET
        public string actionCode { get; set; }
        public DateTime departureDateTime { get; set; }
        public DateTime returnDateTime { get; set; }
        public string originCity { get; set; }
        public string destinationCity { get; set; }
        public int noOfAdults { get; set; }
        public int noOfChildren { get; set; }
        public int noOfInfants { get; set; }
        public List<string> AN_optionInfo { get; set; }
        public string languageCode { get; set; }
        public string bookingOID { get; set; }
        public Session Session { get; set; }
    }
    public partial class Session
    {
        public bool IsStateFull { get; set; }

        public bool InSeries { get; set; }

        public bool End { get; set; }

        public string SessionId { get; set; }

        public string SequenceNumber { get; set; }

        public string SecurityToken { get; set; }
    }
}
