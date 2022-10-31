using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.CitySearch
{
    public class CityList
    {
        public List<Destination> Destinations { get; set; }

        public List<CityAJAX> GetAjaxList()
        {
            List<CityAJAX> cities = new List<CityAJAX>();
            foreach(Destination d in Destinations)
            {
                CityAJAX city = new CityAJAX();
                city.code = d.Code;
                string name = "";
                if (d.Name != d.AirportName)
                {
                    name += d.AirportName;
                }
                name += (name == "" ? "" : ", ") + d.Name;
                name += " [" + d.Code + "] - ";
                name += d.CountryName;
                city.name = name;
                cities.Add(city);
            }
            return cities;
        }
    }

    public class Destination
    {
        public string Code { get; set; }
        public string CityCode { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string CityName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string AirportName { get; set; }
        public bool IsAllAirport { get; set; }
        public int Frequent { get; set; }
    }

    public class CityAJAX
    {
        public string code { get; set; }
        public string name { get; set; }
    }
}
