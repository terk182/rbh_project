using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodPNR
{
    public class PNR
    {
        public string recordLocator { get; set; }
        public bool isError { get; set; }
        public string type { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string bookingKeyReference { get; set; }
        public Guid BookingflightOID { get; set; }
        public string RobinhoodID { get; set; }
        public List<AirSell.AirSellEntities> airSellEntities { get; set; }
    }
}
