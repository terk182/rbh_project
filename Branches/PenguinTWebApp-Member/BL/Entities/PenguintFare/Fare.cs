using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiFare
{
    public class Fare
    {
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
    }

    public class Baggage
    {
        public string baggageNo { get; set; }
        public string baggageUnit { get; set; }
    }
}
