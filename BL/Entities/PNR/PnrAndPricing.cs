using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.PNR
{
    public class PnrAndPricing
    {
        public Response pnr { get; set; }
        public Entities.RobinhoodFare.AirFare pricing { get; set; }
    }
}
