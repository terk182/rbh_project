using BL;
using DataModel;
using DataModel.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GogojiiWeb.Utilities
{
    public class Naming
    {
        public INamingServices namingService = null;
        public Naming(INamingServices _namingService)
        {
            namingService = _namingService;
        }

        public string getAirlineName(string airlineCode)
        {
            return getAirlineName(airlineCode, "en");
        }
        public string getAirlineName(string airlineCode, string langaugeCode)
        {
            List<AirlineCode> airlines = new List<AirlineCode>();
            if (System.Web.HttpContext.Current.Cache["C_AIRLINE"] != null)
            {
                airlines = (List<AirlineCode>)System.Web.HttpContext.Current.Cache["C_AIRLINE"];
            }
            else
            {
                airlines = namingService.GetAllAirline();
                System.Web.HttpContext.Current.Cache.Insert("C_AIRLINE", airlines, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            string name = "";
            var airline = airlines.FirstOrDefault(x => x.Code.ToUpper() == airlineCode.ToUpper() && x.Language.ToUpper() == langaugeCode.ToUpper());
            if (airline != null)
            {
                name = airline.Name;
            }
            else
            {
                airline = airlines.FirstOrDefault(x => x.Code.ToUpper() == airlineCode.ToUpper() && x.Language.ToUpper() == "EN");
                if (airline != null)
                {
                    name = airline.Name;
                }
            }
            return name;
        }

        public string getCountryName(string countryCode)
        {
            return getCountryName(countryCode, "en");
        }
        public string getCountryName(string countryCode, string langaugeCode)
        {
            List<CountryCode> countries = new List<CountryCode>();
            if (System.Web.HttpContext.Current.Cache["C_COUNTRY"] != null)
            {
                countries = (List<CountryCode>)System.Web.HttpContext.Current.Cache["C_COUNTRY"];
            }
            else
            {
                countries = namingService.GetAllCountry();
                System.Web.HttpContext.Current.Cache.Insert("C_COUNTRY", countries, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            string name = "";
            var country = countries.FirstOrDefault(x => x.Code.ToUpper() == countryCode.ToUpper() && x.Language.ToUpper() == langaugeCode.ToUpper());
            if (country != null)
            {
                name = country.Name;
            }
            else
            {
                country = countries.FirstOrDefault(x => x.Code.ToUpper() == countryCode.ToUpper() && x.Language.ToUpper() == "EN");
                if (country != null)
                {
                    name = country.Name;
                }
            }
            return name;
        }


        public string getCityName(string cityCode)
        {
            return getCityName(cityCode, "en");
        }
        public string getCityName(string cityCode, string langaugeCode)
        {
            List<CityCode> cities = new List<CityCode>();
            if (System.Web.HttpContext.Current.Cache["C_CITY"] != null)
            {
                cities = (List<CityCode>)System.Web.HttpContext.Current.Cache["C_CITY"];
            }
            else
            {
                cities = namingService.GetAllCity();
                System.Web.HttpContext.Current.Cache.Insert("C_CITY", cities, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            string name = "";
            var city = cities.FirstOrDefault(x => x.Code.ToUpper() == cityCode.ToUpper() && x.Language.ToUpper() == langaugeCode.ToUpper());
            if (city != null)
            {
                name = city.Name;
            }
            else
            {
                city = cities.FirstOrDefault(x => x.Code.ToUpper() == cityCode.ToUpper() && x.Language.ToUpper() == "EN");
                if (city != null)
                {
                    name = city.Name;
                }
            }
            return name;
        }

    }
}
