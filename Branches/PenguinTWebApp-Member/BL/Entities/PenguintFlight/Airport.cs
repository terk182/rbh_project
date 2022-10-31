using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiFlight
{
    public class Airport
    {
        private NamingServices _namingServices;
        private bool _isFullName;
        private string _languageCode = "en";
        public Airport(NamingServices namingServices, bool isFullName, string languageCode)
        {
            _namingServices = namingServices;
            _isFullName = isFullName;
            _languageCode = languageCode;
        }
        public string code { get; set; }
        public string name {
            get
            {
                if (_namingServices == null)
                {
                    return "";
                }
                return _namingServices.GetAirportName(this.code, this._isFullName, this._languageCode);
            }
        }
    }
}
