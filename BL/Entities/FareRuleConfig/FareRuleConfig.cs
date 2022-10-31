using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.FareRuleConfig
{
    public class FareRuleConfig
    {
        public string airlineCode { get; set; }
        public string fareFamilyCode { get; set; }
        public List<FareRuleDetails> fareRuleDetails { get; set; }
        public List<ConditionConfig> conditionConfigs { get; set; }
    }
    public class FareRuleDetails
    {
        public string languageCode { get; set; }
        public string fareFamilyName { get; set; }
        public List<string> fareRuleText { get; set; }
    }
    public class ConditionConfig
    {
        public string zoneFrom { get; set; }
        public string zoneTo { get; set; }
        public string rbd { get; set; }
        public string fareBasis { get; set; }
    }
}
