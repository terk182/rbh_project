using BL.Entities.GogojiiFlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiFare
{
    public class FareRule
    {
        public City origin { get; set; }
        public City destination { get; set; }
        public string fareBasis { get; set; }
        public List<FareRuleDatail> rules { get; set; }
    }

    public class FareRuleDatail
    {
        public string category {
            get
            {
                if (this.fareRuleText !=null && this.fareRuleText.Count >0)
                {
                    return this.fareRuleText[0].Substring(0, 2);
                }
                return "";
            }
        }
        public List<string> fareRuleText { get; set; }
    }
}
