using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGBookingWeb.Models
{
    public class MetaDataModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public List<attribute> attributes { get; set; }
    }
    public class attribute
    {
        public string trait_type { get; set; }
        public string value { get; set; }
    }
}