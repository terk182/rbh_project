using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodFlight
{
    public class City
    {
        private NamingServices _namingServices;
        private string _languageCode = "en";
        public City(NamingServices namingServices, string languageCode)
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
                string cityName = _namingServices.GetCityName(this.code, this._languageCode);
                if (cityName == "")
                {
                    cityName = _namingServices.GetAirportName(this.code, false, this._languageCode);
                }
                return cityName;
            }
        }
    }
}
