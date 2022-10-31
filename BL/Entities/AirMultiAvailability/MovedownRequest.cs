using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.AirMultiAvailability
{
    public class MovedownRequest
    {
        public DateTime departureDateTime { get; set; }
        public DateTime returnDateTime { get; set; }
        public Session Session { get; set; }
    }
}
