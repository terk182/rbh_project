using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.FareRuleConfig
{
    public class Response
    {
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public RobinhoodFare.FareRule fareRule { get; set; }
    }
}
