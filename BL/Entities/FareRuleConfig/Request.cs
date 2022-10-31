using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.FareRuleConfig
{
    public class Request
    {
        public string origin { get; set; }//boardPoint
        public string destination { get; set; }//offPoint
        public string airlineCode { get; set; }//marketingCompany
        public string fareBasis { get; set; }
        public string rbd { get; set; }
        public string languageCode { get; set; }
    }
}
