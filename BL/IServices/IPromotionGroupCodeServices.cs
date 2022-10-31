using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IPromotionGroupCodeServices
    {
        List<PromotionGroupCode> GetAll();
        PromotionGroupCode GetByID(Guid PromotionGroupCodeOID);
        void SaveOrUpdate(PromotionGroupCode groupCode);
    }
}
