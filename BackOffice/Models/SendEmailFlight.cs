using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackOffice.Models
{
    public class SendEmailFlight
    {
        public BL.Entities.RobinhoodFare.AirFare emailAirfare { get; set; }
        public Information emailInformation { get; set; }
    }
}