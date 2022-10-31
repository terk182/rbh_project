using BL.Entities;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IAirlineQtaxControlServices
    {
        List<AirlineQtaxControl> GetAll();
        AirlineQtaxControl GetByID(Guid id);
        void SaveOrUpdate(AirlineQtaxControl airlireQtax);

    }
}