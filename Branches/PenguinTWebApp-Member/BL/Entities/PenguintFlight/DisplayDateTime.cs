using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiFlight
{
    public class DisplayDateTime
    {
        public int date { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string shortDate { get; set; }
        public string longDate { get; set; }
        public string shortDateWithoutDay { get; set; }
        public string longDateWithoutDay { get; set; }
        public int time { get; set; }
        public string displayTime { get; set; }
    }
}
