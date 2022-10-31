using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodFlight
{
    public class Airport
    {
        private NamingServices _namingServices;
        private bool _isFullName;
        private string _languageCode = "en";
        private string _name = "";
        public Airport(NamingServices namingServices, bool isFullName, string languageCode)
        {
            _namingServices = namingServices;
            _isFullName = isFullName;
            _languageCode = languageCode;
        }
        public string code { get; set; }
        public string terminal { get; set; }
        public string name {
            get
            {
                if (_namingServices == null)
                {
                    return _name;
                }
                return _namingServices.GetAirportName(this.code, this._isFullName, this._languageCode);
            }
            set
            {
                _name = value;
            }
        }
    }
}
