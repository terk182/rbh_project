using BL.Entities.RobinhoodFlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface ILionAirServices
    {
        FlightSearchMultiTicketResult Search(Entities.MasterPricer.Request request);
        Entities.RobinhoodFare.AirFare AirSellService(Entities.InformativePricing.Request request, Entities.InformativePricing.Request requestFor1L, string languageCode);
    }
}
