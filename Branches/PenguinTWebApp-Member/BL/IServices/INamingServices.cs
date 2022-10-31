using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface INamingServices
    {
        List<CountryCode> GetAllCountry();
        List<CityCode> GetAllCity();
        List<AirportCode> GetAllAirport();
        List<AirportWithCity> GetAllAirportWithCity();
        List<AirlineCode> GetAllAirline();
        string GetAirlineName(string airlineCode);
        string GetAirlineName(string airlineCode, string langaugeCode);
        string GetCountryName(string countryCode);
        string GetCountryName(string countryCode, string langaugeCode);
        string GetCityName(string cityCode);
        string GetCityName(string cityCode, string langaugeCode);
        string GetAirportName(string airportCode, bool isFullName);
        string GetAirportName(string airportCode, bool isFullName, string langaugeCode);
        string GetCountryCode(string cityCode);
    }
}
