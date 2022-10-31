using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.BaseEntities
{
    public class RateBreakDown
    {
        public decimal net { get; set; }
        public List<PromotionBreakDown> promotions { get; set; }
        public decimal serviceCharge { get; set; }
        public decimal vat { get; set; }
        public decimal paymentFee { get; set; }
        public decimal roundUp { get; set; }
        public decimal total { get; set; }


        public RateBreakDown()
        {
            net = 0;
            serviceCharge = 0;
            vat = 0;
            paymentFee = 0;
            roundUp = 0;
            total = 0;
            promotions = new List<PromotionBreakDown>();
        }
    }
}
