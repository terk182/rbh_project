using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.FareRuleConfig
{
    public class SegmentForGetFareRule
    {
        public string boardPoint { get; set; }//Depart
        public string offPoint { get; set; }//Destination
        public string marketingCompany { get; set; }//AirlineCode
        public string fareBasis { get; set; }
        public string rbd { get; set; }
    }
}
