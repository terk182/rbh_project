using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiPNR
{
    public class PNR
    {
        public string recordLocator { get; set; }
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public string BookingKeyReference { get; set; }
    }
}
