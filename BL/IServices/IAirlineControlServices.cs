using BL.Entities;
using BL.Entities.AirlineControl;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IAirlineControlServices
    {
        List<AirlineControl> GetAll();
        //AirlineControlEntities GetByID(Guid id);
        AirlineControl GetByID(Guid AirlineOID);
        void SaveOrUpdate(AirlineControl airlire);
        List<AirlineControlSub> GetAllAirlineSub(Guid AirlineSubOID);
        AirlineControlSub GetByIDAirlineSub(Guid AirlineSubOID, Guid AirlineOID);
        void SaveOrUpdateAirlinesub(AirlineControlSub airlire);
        bool CheckAirlineControl(List<AirlineControlSub> airlinesub, string orginalCountryCode, string destinationCountryCode, string airlineCode, string rbd, string fareBasis);
    }
}