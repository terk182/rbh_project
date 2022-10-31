using BL.Entities;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface ISeatMapControlServices
    {
        List<SeatMapControl> GetAll();
        SeatMapControl GetByID(Guid id);
        void SaveOrUpdate(SeatMapControl seatMap);
    }
}