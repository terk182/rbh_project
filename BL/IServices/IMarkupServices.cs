using BL.Entities;
using BL.Entities.RobinhoodFlight;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IMarkupServices
    {
        List<MarkupFlight> GetAll();
        MarkupFlight GetByID(Guid id);
        MarkupFlight GetDuplicate(MarkupFlight model);
        void SaveOrUpdate(MarkupFlight markup);
        FareMarkup GetMarkup();
        bool RemoveCache();
    }
}