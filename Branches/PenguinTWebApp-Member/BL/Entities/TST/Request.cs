using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.TST
{
    public class Request
    {
        public List<string> uniqueReferences { get; set; }
        public bool useBookingOfficeID { get; set; }
        public string ClientIP { get; set; }
        public Session session { get; set; }
        public string bookingOID { get; set; }
    }

    public partial class Session
    {
        public bool isStateFull { get; set; }
        public bool InSeries { get; set; }
        public bool End { get; set; }
        public string SessionId { get; set; }
        public string SequenceNumber { get; set; }
        public string SecurityToken { get; set; }
    }
}
