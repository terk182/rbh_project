using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodPNR
{
    public class MultiTicketPNR
    {
        public string RobinhoodID { get; set; }
        public bool isError { get; set; }
        public string type { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }       
        public List<AirSell.AirSellEntities> airSellEntities { get; set; }
        public List<Booking> Booking { get; set; }
    }
    public class Booking
    {
        public string recordLocator { get; set; }
        public string bookingKeyReference { get; set; }
    }
}
