using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BL.Entities.RobinhoodFare
{
    public class Fare
    {
        [JsonIgnore]
        [XmlIgnore]
        public Guid fareOID { get; set; }
        public decimal baseFare { get; set; }
        public decimal sellingBaseFare { get; set; }
        public decimal tax { get; set; }
        public decimal qtax { get; set; }
        public decimal lessFare { get; set; }
        public decimal net {
            get
            {
                return this.sellingBaseFare + this.tax + this.qtax;
            }
        }
        public List<Baggage> baggages { get; set; }
        public onewayPrice depPrice { get; set; }
        public onewayPrice retPrice { get; set; }
        public string warningCode { get; set; }
        public List<string> warningMessage { get; set; }

    }

    public class Baggage
    {
        public int segment { get; set; }
        public string baggageNo { get; set; }
        public string baggageUnit { get; set; }
    }
    public class onewayPrice
    {
        public decimal baseFare { get; set; }
        public decimal sellingBaseFare { get; set; }
        public decimal tax { get; set; }
        public decimal qtax { get; set; }
 
        public decimal lessFare { get; set; }
        public decimal net
        {
            get
            {
                return this.sellingBaseFare + this.tax + this.qtax;
            }
        }
        public string warningCode { get; set; }
        public List<string> warningMessage { get; set; }
    }
}
