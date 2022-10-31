using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.DiscountTag
{
    public class DiscountTagEntities
    {
        public DataModel.DiscountTag discountTag { get; set; }
        public List<DataModel.DiscountTagDetail> discountTagDetail { get; set; }
        public DataModel.PromotionGroupCode promotionGroupCode { get; set; }
        
    }
}
