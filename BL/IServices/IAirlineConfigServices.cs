using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IAirlineConfigServices
    {
        List<AirlineConfig> GetAll();
        AirlineConfig GetByID(Guid AirlineConfigOID);
        void SaveOrUpdate(AirlineConfig airline);
        AirlineConfig GetAirlineConfig(List<AirlineConfig> allAirlineConfig, string departureCode, string destinationCode);
    }
}
