using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodFare
{
    public class AirPax
    {
        public RobinhoodPax.ContactInfo contactInfo { get; set; }
        public List<RobinhoodPax.PaxInfo> adtPaxs { get; set; }
        public List<RobinhoodPax.PaxInfo> chdPaxs { get; set; }
        public List<RobinhoodPax.PaxInfo> infPaxs { get; set; }
    }
}
