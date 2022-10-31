using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGBookingWeb.Models
{
    public class NFTResponse
    {
        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("errordetail")]
        public string Errordetail { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("tokenid")]
        public long Tokenid { get; set; }
    }
}