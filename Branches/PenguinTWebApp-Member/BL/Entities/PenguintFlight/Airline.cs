using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiFlight
{
    public class Airline
    {
        private NamingServices _namingServices;
        private string _languageCode = "en";
        public Airline(NamingServices namingServices, string languageCode)
        {
            _namingServices = namingServices;
            _languageCode = languageCode;
        }
        public string code { get; set; }
        public string name
        {
            get
            {
                if (_namingServices == null)
                {
                    return "";
                }
                return _namingServices.GetAirlineName(this.code, this._languageCode);
            }
        }
    }
}
