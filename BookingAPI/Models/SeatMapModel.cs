using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGBookingAPI.Models
{
    public class SeatMapModel
    {
        public DateTime DepartureDateTime { get; set; }
        public string DepartureCity { get; set; }

        public string ArrivalCity { get; set; }

        public string CompanyCode { get; set; }

        public long FlightNumber { get; set; }

        public string RbdCode { get; set; }
    }
}