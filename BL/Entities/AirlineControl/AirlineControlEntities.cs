using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.AirlineControl
{
    public class AirlineControlEntities
    {
        public DataModel.AirlineControl airlineDetail { get; set; }
        public List<DataModel.AirlineControlSub> airlineSubDetail { get; set; }
    }
}
