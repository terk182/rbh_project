using BL.Entities;
using BL.Entities.RobinhoodFare;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface ISendEmailServices
    {
        AirFare GetByID(Guid id);
    }
}