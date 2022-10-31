using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.SeatMap
{
    public partial class Request
    {
        public Flight Flight { get; set; }
        public Session Session { get; set; }
    }

    public partial class Flight
    {
        public DateTime DepartureDateTime { get; set; }

        public DateTime ArrivalDateTime { get; set; }

        public string DepartureCity { get; set; }

        public string ArrivalCity { get; set; }

        public string CompanyCode { get; set; }

        public long FlightNumber { get; set; }

        public string RbdCode { get; set; }
    }

    public partial class Session
    {
        public bool IsStateFull { get; set; }
    }
}
