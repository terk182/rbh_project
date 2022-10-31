using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.FareRuleConfig
{
    public class FareRuleEntities
    {
        public FlightFareRule fareRule { get; set; }
        public List<FlightFareRuleDetail> fareRuleDetails { get; set; }
        public List<FlightFareRuleConfig> fareRuleConfig { get; set; }
    }
}
