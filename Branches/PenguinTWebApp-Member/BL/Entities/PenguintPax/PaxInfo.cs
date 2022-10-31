using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiPax
{
    public class PaxInfo
    {
        public string paxType { get; set; }
        public string title { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public DateTime birthday { get; set; }
        public string email { get; set; }
        public string telNo { get; set; }
    }
}
