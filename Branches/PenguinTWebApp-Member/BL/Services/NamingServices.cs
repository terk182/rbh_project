using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using DataModel.UnitOfWork;

namespace BL
{
    public class NamingServices : INamingServices
    {
        private readonly UnitOfWork _unitOfWork;
        public NamingServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string GetAirlineName(string airlineCode)
        {
            return GetAirlineName(airlineCode, "en");
        }

        public string GetAirlineName(string airlineCode, string langaugeCode)
        {
            List<AirlineCode> airlines = this.GetAllAirline();
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

        public string GetAirportName(string airportCode, bool isFullName)
        {
            return GetAirportName(airportCode, isFullName, "en");
        }

        public string GetAirportName(string airportCode, bool isFullName, string langaugeCode)
        {
            List<AirportCode> airports = this.GetAllAirport();
            string name = "";
            var airport = airports.FirstOrDefault(x => x.Code.ToUpper() == airportCode.ToUpper() && x.Language.ToUpper() == langaugeCode.ToUpper());
            if (airport != null)
            {
                name = airport.Name;
            }
            else
            {
                airport = airports.FirstOrDefault(x => x.Code.ToUpper() == airportCode.ToUpper() && x.Language.ToUpper() == "EN");
                if (airport != null)
                {
                    name = airport.Name;
                }
                else
                {
                    string cityCode = airportCode;
                    var airportCities = this.GetAllAirportWithCity();
                    var ac = airportCities.FirstOrDefault(x => x.AirportCode.ToUpper() == airportCode.ToUpper());
                    if (ac != null)
                    {
                        cityCode = ac.CityCode;
                    }

                    name = this.GetCityName(cityCode, langaugeCode);
                }
            }

            if (isFullName)
            {
                string cityCode = airportCode;
                var airportCities = this.GetAllAirportWithCity();
                var ac = airportCities.FirstOrDefault(x => x.AirportCode.ToUpper() == airportCode.ToUpper());
                if (ac != null)
                {
                    cityCode = ac.CityCode;
                }

                string cityName = this.GetCityName(cityCode, langaugeCode);
                //if (cityName != name)
                {
                    name = name + ", " + cityName;
                }
            }

            return name;
        }

        public List<AirlineCode> GetAllAirline()
        {
            List<AirlineCode> airlines = new List<AirlineCode>();
            if (System.Web.HttpContext.Current.Cache["C_AIRLINE"] != null)
            {
                airlines = (List<AirlineCode>)System.Web.HttpContext.Current.Cache["C_AIRLINE"];
            }
            else
            {
                airlines = _unitOfWork.AirlineCodeRepository.GetAll().ToList();
                System.Web.HttpContext.Current.Cache.Insert("C_AIRLINE", airlines, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return airlines;
        }

        public List<AirportCode> GetAllAirport()
        {
            List<AirportCode> airports = new List<AirportCode>();
            if (System.Web.HttpContext.Current.Cache["C_AIRPORT"] != null)
            {
                airports = (List<AirportCode>)System.Web.HttpContext.Current.Cache["C_AIRPORT"];
            }
            else
            {
                airports = _unitOfWork.AirportCodeRepository.GetAll().ToList();
                System.Web.HttpContext.Current.Cache.Insert("C_AIRPORT", airports, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            return airports;
        }

        public List<AirportWithCity> GetAllAirportWithCity()
        {
            List<AirportWithCity> airports = new List<AirportWithCity>();
            if (System.Web.HttpContext.Current.Cache["C_AIRPORT_CITY"] != null)
            {
                airports = (List<AirportWithCity>)System.Web.HttpContext.Current.Cache["C_AIRPORT_CITY"];
            }
            else
            {
                airports = _unitOfWork.AirportWithCityRepository.GetAll().ToList();
                System.Web.HttpContext.Current.Cache.Insert("C_AIRPORT_CITY", airports, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            return airports;
        }

        public List<CityCode> GetAllCity()
        {
            List<CityCode> cities = new List<CityCode>();
            if (System.Web.HttpContext.Current.Cache["C_CITY"] != null)
            {
                cities = (List<CityCode>)System.Web.HttpContext.Current.Cache["C_CITY"];
            }
            else
            {
                cities = _unitOfWork.CityCodeRepository.GetAll().ToList();
                System.Web.HttpContext.Current.Cache.Insert("C_CITY", cities, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }

            return cities;
        }

        public List<CountryCode> GetAllCountry()
        {
            List<CountryCode> countries = new List<CountryCode>();
            if (System.Web.HttpContext.Current.Cache["C_COUNTRY"] != null)
            {
                countries = (List<CountryCode>)System.Web.HttpContext.Current.Cache["C_COUNTRY"];
            }
            else
            {
                countries = _unitOfWork.CountryCodeRepository.GetAll().ToList();
                System.Web.HttpContext.Current.Cache.Insert("C_COUNTRY", countries, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return countries;
        }

        public string GetCityName(string cityCode)
        {
            return GetCityName(cityCode, "en");
        }

        public string GetCityName(string cityCode, string langaugeCode)
        {
            List<CityCode> cities = this.GetAllCity();
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

        public string GetCountryCode(string cityCode)
        {
            List<CityCode> cities = this.GetAllCity();
            string code = "";
            var city = cities.FirstOrDefault(x => x.Code.ToUpper() == cityCode.ToUpper());
            if (city != null)
            {
                code = city.CountryCode;
            }
            else
            {
                List<AirportCode> airports = this.GetAllAirport();
                var airport = airports.FirstOrDefault(x => x.Code.ToUpper() == cityCode.ToUpper());
                if (city != null)
                {
                    code = airport.CountryCode;
                }
            }
            return code;
        }

        public string GetCountryName(string countryCode)
        {
            return GetCountryName(countryCode, "en");
        }

        public string GetCountryName(string countryCode, string langaugeCode)
        {
            List<CountryCode> countries = this.GetAllCountry();

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
    }
}
