using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodPax
{
    public class SpecialRequest
    {
        public int flightST { get; set; }
        public Seat seat { get; set; }
        public string mealCode { get; set; }
        public string freeText { get; set; }
    }
    public class Seat
    {
        public string seatType { get; set; }
        public string seatRefNo { get; set; }
    }
}
