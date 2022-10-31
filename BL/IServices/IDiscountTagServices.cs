using BL.Entities.DiscountTag;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IDiscountTagServices
    {
        List<DiscountTagEntities> GetAll();
        DiscountTagEntities GetByID(Guid DiscountTagOID);
        void SaveOrUpdate(DiscountTagEntities DiscountTag);
    }
}
