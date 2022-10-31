using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodMarkup
{
    public class MarkupFlightResponse
    {
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public List<DataModel.MarkupFlight> markupFlights { get; set; }
    }
}
