using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.SeatMap
{
    public partial class Response
    {
        [JsonProperty("Air_RetrieveSeatMapReply")]
        public AirRetrieveSeatMapReply AirRetrieveSeatMapReply { get; set; }
    }

    public partial class AirRetrieveSeatMapReply
    {
        [JsonProperty("segment")]
        public object segment { get; set; }
    }
}
