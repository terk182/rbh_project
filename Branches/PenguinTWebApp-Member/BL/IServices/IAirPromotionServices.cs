using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IAirPromotionServices
    {
        List<AirPromotion> GetAll();
        AirPromotion GetById(Guid id);
        void SaveOrUpdate(AirPromotion airPromotion);
        void Delete(Guid id);
    }
}
