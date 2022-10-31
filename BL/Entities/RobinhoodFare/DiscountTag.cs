using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodFare
{
    public class DiscountTag
    {
        public decimal discountAmount { get; set; }
        public bool discountIsPercent { get; set; }
        public string promotionGroupCode { get; set; }
        public string promotionText { get; set; }
    }
}
