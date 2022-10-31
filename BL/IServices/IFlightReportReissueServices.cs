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
    public interface IFlightReportReissueServices
    {
        
        List<AirFare.Reissue> GetAllReissue();
        AirFare GetByID(Guid id);
        void SaveOrUpdateReissue(AirFare airfare);
    }
}