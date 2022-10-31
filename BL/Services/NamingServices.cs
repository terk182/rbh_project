using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Entities.CitySearch;
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
            /*
            if (System.Web.HttpContext.Current.Cache["C_AIRLINE"] != null)
            {
                airlines = (List<AirlineCode>)System.Web.HttpContext.Current.Cache["C_AIRLINE"];
            }
            else
            {
                airlines = _unitOfWork.AirlineCodeRepository.GetAll().ToList();
                System.Web.HttpContext.Current.Cache.Insert("C_AIRLINE", airlines, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }*/

            Utilities.MemoryCacher cacher = new Utilities.MemoryCacher();
            if (cacher.GetValue("C_AIRLINE") != null)
            {
                airlines = (List<AirlineCode>)cacher.GetValue("C_AIRLINE");
            }
            else
            {
                airlines = _unitOfWork.AirlineCodeRepository.GetAll().ToList();
                cacher.Add("C_AIRLINE", airlines, DateTime.Now.AddMonths(1));
            }
            return airlines;
        }

        public List<AirportCode> GetAllAirport()
        {
            List<AirportCode> airports = new List<AirportCode>();
            /*
            if (System.Web.HttpContext.Current.Cache["C_AIRPORT"] != null)
            {
                airports = (List<AirportCode>)System.Web.HttpContext.Current.Cache["C_AIRPORT"];
            }
            else
            {
                airports = _unitOfWork.AirportCodeRepository.GetAll().ToList();
                System.Web.HttpContext.Current.Cache.Insert("C_AIRPORT", airports, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            */
            Utilities.MemoryCacher cacher = new Utilities.MemoryCacher();
            if (cacher.GetValue("C_AIRPORT") != null)
            {
                airports = (List<AirportCode>)cacher.GetValue("C_AIRPORT");
            }
            else
            {
                airports = _unitOfWork.AirportCodeRepository.GetAll().ToList();
                cacher.Add("C_AIRPORT", airports, DateTime.Now.AddMonths(1));
            }
            return airports;
            
        }

        public string FindBestMatchCityAirport(string key)
        {
            key = key.ToLower();
            Dictionary<string, string> allName = this.GetAllAirportAndCityName(true);
            
            if (allName.ContainsValue(key))
            {
                return allName.FirstOrDefault(x => x.Value == key).Key.Split('_')[0];
            }
            else
            {
                var allMatch = allName.Where(x => x.Value.IndexOf(key) >= 0).ToList();
                if (allMatch != null && allMatch.Count > 0)
                {
                    allMatch = allMatch.OrderBy(x => x.Value.IndexOf(key)).ToList();
                    return allMatch[0].Key.Split('_')[0];
                }
            }

            return "";
        }

        public Dictionary<string, string> GetAllAirportAndCityName(bool toLower)
        {

            List<AirportCode> airport = this.GetAllAirport();
            List<CityCode> cities = this.GetAllCity();
            Dictionary<string, string> all = new Dictionary<string, string>();
            foreach(var a in airport)
            {
                if (!all.ContainsKey(a.Code + "_a_" + a.Language))
                {
                    all.Add(a.Code + "_a_" + a.Language, toLower ? a.Name.ToLower() : a.Name);
                }
            }
            foreach (var c in cities)
            {
                if (!all.ContainsKey(c.Code + "_c_" + c.Language))
                {
                    all.Add(c.Code + "_c_" + c.Language, toLower ? c.Name.ToLower() : c.Name);
                }
            }
            return all;
        }

        public List<AirportWithCity> GetAllAirportWithCity()
        {
            List<AirportWithCity> airports = new List<AirportWithCity>();
            /*
            if (System.Web.HttpContext.Current.Cache["C_AIRPORT_CITY"] != null)
            {
                airports = (List<AirportWithCity>)System.Web.HttpContext.Current.Cache["C_AIRPORT_CITY"];
            }
            else
            {
                airports = _unitOfWork.AirportWithCityRepository.GetAll().ToList();
                System.Web.HttpContext.Current.Cache.Insert("C_AIRPORT_CITY", airports, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            */

            Utilities.MemoryCacher cacher = new Utilities.MemoryCacher();
            if (cacher.GetValue("C_AIRPORT_CITY") != null)
            {
                airports = (List<AirportWithCity>)cacher.GetValue("C_AIRPORT_CITY");
            }
            else
            {
                airports = _unitOfWork.AirportWithCityRepository.GetAll().ToList();
                cacher.Add("C_AIRPORT_CITY", airports, DateTime.Now.AddMonths(1));
            }

            return airports;
        }

        public List<CityCode> GetAllCity()
        {
            List<CityCode> cities = new List<CityCode>();
            /*
            if (System.Web.HttpContext.Current.Cache["C_CITY"] != null)
            {
                cities = (List<CityCode>)System.Web.HttpContext.Current.Cache["C_CITY"];
            }
            else
            {
                cities = _unitOfWork.CityCodeRepository.GetAll().ToList();
                System.Web.HttpContext.Current.Cache.Insert("C_CITY", cities, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            */

            Utilities.MemoryCacher cacher = new Utilities.MemoryCacher();
            if (cacher.GetValue("C_CITY") != null)
            {
                cities = (List<CityCode>)cacher.GetValue("C_CITY");
            }
            else
            {
                cities = _unitOfWork.CityCodeRepository.GetAll().ToList();
                cacher.Add("C_CITY", cities, DateTime.Now.AddMonths(1));
            }

            return cities;
        }

        public List<CountryCode> GetAllCountry()
        {
            List<CountryCode> countries = new List<CountryCode>();
            /*if (System.Web.HttpContext.Current.Cache["C_COUNTRY"] != null)
            {
                countries = (List<CountryCode>)System.Web.HttpContext.Current.Cache["C_COUNTRY"];
            }
            else
            {
                countries = _unitOfWork.CountryCodeRepository.GetAll().ToList();
                System.Web.HttpContext.Current.Cache.Insert("C_COUNTRY", countries, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }*/

            Utilities.MemoryCacher cacher = new Utilities.MemoryCacher();
            if (cacher.GetValue("C_COUNTRY") != null)
            {
                countries = (List<CountryCode>)cacher.GetValue("C_COUNTRY");
            }
            else
            {
                countries = _unitOfWork.CountryCodeRepository.GetAll().ToList();
                cacher.Add("C_COUNTRY", countries, DateTime.Now.AddMonths(1));
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
                if (airport != null)
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

        public string GetFullCityAirportName(string code, string langauge)
        {
            var allCityAirport = this.GetAllAirportWithCity();
            string name = "";
            name = this.GetCityName(code, langauge);
            if (name == "")
            {
                name = this.GetAirportName(code, false, langauge);
                var city = allCityAirport.FirstOrDefault(x => x.AirportCode == code.ToUpper());
                name += " - " + this.GetCityName(city.CityCode, langauge);
            }

            name += " [" + code + "], ";
            var cc = this.GetCountryCode(code);
            name += this.GetCountryName(cc, langauge);
            return name;
        }

        public string GetNearestAirport(string latitude, string longitude, string countryCode)
        {
            var allCountryAirport = _unitOfWork.AirportLocationRepository.GetMany(x => x.CountryCode.ToUpper() == countryCode.ToUpper()).ToList();

            double distance = 0;
            string cityCode = "";
            double lat1 = Convert.ToDouble(latitude);
            double lng1 = Convert.ToDouble(longitude);
            foreach(var airport in allCountryAirport)
            {
                double lat2 = Convert.ToDouble(airport.Latitude);
                double lng2 = Convert.ToDouble(airport.Longitude);
                double d = this.DistanceCalc(lat1, lng1, lat2, lng2);
                if (distance == 0 || distance > d)
                {
                    distance = d;
                    cityCode = airport.CityCode;
                }
            }
            return cityCode;
        }
        public double DistanceCalc(double Lat1,
                  double Long1, double Lat2, double Long2)
        {
            
            double dDistance = Double.MinValue;
            double dLat1InRad = Lat1 * (Math.PI / 180.0);
            double dLong1InRad = Long1 * (Math.PI / 180.0);
            double dLat2InRad = Lat2 * (Math.PI / 180.0);
            double dLong2InRad = Long2 * (Math.PI / 180.0);

            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;

            // Intermediate result a.
            double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                       Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                       Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Intermediate result c (great circle distance in Radians).
            double c = 2.0 * Math.Asin(Math.Sqrt(a));

            // Distance.
            // const Double kEarthRadiusMiles = 3956.0;
            const Double kEarthRadiusKms = 6376.5;
            dDistance = kEarthRadiusKms * c;
            dDistance = Math.Abs(dDistance);
            return dDistance;
        }


        public void SaveAirportLocation(List<AirportLocation> location)
        {
            _unitOfWork.AirportLocationRepository.InsertMany(location);
            _unitOfWork.Save();
        }

        public List<CityAJAX> SearchCities(string keyword, string language)
        {
            if (keyword.Length < 3)
            {
                return new List<CityAJAX>();
            }
            var allCity = this.GetAllCity();
            var allAirport = this.GetAllAirport();
            var allCountry = this.GetAllCountry();
            var allCityAirport = this.GetAllAirportWithCity();

            List<string> codes = new List<string>();
            if (allCity.FirstOrDefault(x => x.Code.ToUpper() == keyword.ToUpper()) != null)
            {
                codes.Add(keyword.ToUpper());
            }
            if (allAirport.FirstOrDefault(x => x.Code.ToUpper() == keyword.ToUpper()) != null)
            {
                codes.Add(keyword.ToUpper());
            }

            var matchCityAirport = allCityAirport.Where(x => x.FacilityCode == "A" && codes.Contains(x.CityCode)).ToList();
            if (matchCityAirport != null && matchCityAirport.Count > 0)
            {
                foreach (var ca in matchCityAirport)
                {
                    codes.Add(ca.AirportCode);
                }
            }

            var matchCity = allCity.Where(x => x.Name.ToUpper().IndexOf(keyword.ToUpper()) >= 0).OrderBy(x => x.Name.ToUpper().IndexOf(keyword.ToUpper())).ToList();
            if (matchCity != null && matchCity.Count > 0)
            {
                foreach(var c in matchCity)
                {
                    codes.Add(c.Code);
                }
            }

            var matchAirport = allAirport.Where(x => x.Name.ToUpper().IndexOf(keyword.ToUpper()) >= 0).OrderBy(x => x.Name.ToUpper().IndexOf(keyword.ToUpper())).ToList();
            if (matchAirport != null && matchAirport.Count > 0)
            {
                foreach (var a in matchAirport)
                {
                    codes.Add(a.Code);
                }
            }

            matchCityAirport = allCityAirport.Where(x => x.FacilityCode == "A" && codes.Contains(x.CityCode)).ToList();
            if (matchCityAirport != null && matchCityAirport.Count > 0)
            {
                foreach (var ca in matchCityAirport)
                {
                    codes.Add(ca.AirportCode);
                }
            }


            var matchCountry = allCountry.Where(x => x.Name.ToUpper().IndexOf(keyword.ToUpper()) >= 0).OrderBy(x => x.Name.ToUpper().IndexOf(keyword.ToUpper())).ToList();
            if (matchCountry != null && matchCountry.Count > 0)
            {
                foreach (var cc in matchCountry)
                {
                    var allC = allCity.Where(x => x.CountryCode == cc.Code).ToList();
                    var allA = allAirport.Where(x => x.CountryCode == cc.Code).ToList();

                    foreach(var ac in allC)
                    {
                        codes.Add(ac.Code);
                    }
                    foreach (var aa in allA)
                    {
                        codes.Add(aa.Code);
                    }
                }
            }

            codes = codes.Distinct().ToList();

            List<CityAJAX> cityList = new List<CityAJAX>();
            foreach(var code in codes)
            {
                CityAJAX city = new CityAJAX();
                city.code = code;
                city.name = this.GetFullCityAirportName(code, language);
                cityList.Add(city);
            }
            return cityList;
        }
        public string GetCountryCodeFromAlpha3(string countryAlpha3Code)
        {
            List<CountryCode> countries = this.GetAllCountry();
            countries = countries.FindAll(x => x.Code != "+").ToList();
            string code = countryAlpha3Code;
            var country = countries.FirstOrDefault(x => x.Alpha3.ToUpper() == countryAlpha3Code.ToUpper() && x.Language.ToUpper() == "EN");
            if (country != null)
            {
                code = country.Code;
            }
            return code;
        }
    }
}
