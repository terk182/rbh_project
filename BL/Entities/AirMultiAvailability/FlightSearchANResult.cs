using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.AirMultiAvailability
{
    public class FlightSearchANResult
    {
        private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Guid pgSearchOID { get; set; }
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public List<FlightAN> departureFlight { get; set; }
        public List<FlightAN> returnFlight { get; set; }
    }
}
