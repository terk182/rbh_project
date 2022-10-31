using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackOffice.Models
{
    public class NFTVoucherRequest
    {
        public string to { get; set; }
        public MetaData metaData { get; set; }
}
    public class MetaData
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