using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.FareRuleConfig
{
    public class AllResponse
    {
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public List<FareRuleConfig> fareRules { get; set; }
    }
}
