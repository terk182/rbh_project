using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodAirlineControl
{
    public class AirlineControlResponse
    {
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public List<AirlineControl> airlineControls { get; set; }
    }
    public class AirlineControl
    {
        public System.Guid AirlineOID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string OriginalCountryCode { get; set; }
        public string DestinationCountryCode { get; set; }
        public List<DataModel.AirlineControlSub> airlines { get; set; }
    }

}
