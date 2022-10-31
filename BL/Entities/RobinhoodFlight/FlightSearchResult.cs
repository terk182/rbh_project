using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodFlight
{
    public class FlightSearchResult
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<MasterFlight> flights { get; set; }
        public FlightFilter filter { get; set; }
        public Guid pgSearchOID { get; set; }
        public bool isError { get; set; }
        public string errorMessage { get; set; }

        public string toJson()
        {
            StringWriter sw = new StringWriter();
            try
            {
                JsonTextWriter writer = new JsonTextWriter(sw);
                Dictionary<string, string> airlineDic = new Dictionary<string, string>();
                Dictionary<string, string> airportDic = new Dictionary<string, string>();
                // {
                writer.WriteStartObject();

                // "pgSearchOID" : pgSearchOID
                writer.WritePropertyName("pgSearchOID");
                writer.WriteValue(pgSearchOID);

                // "filter": 
                writer.WritePropertyName("filter");
                // {
                writer.WriteStartObject();
                writer.WritePropertyName("maxDepDuration");
                writer.WriteValue(filter.maxDepDuration);
                writer.WritePropertyName("maxPrice");
                writer.WriteValue(filter.maxPrice);
                writer.WritePropertyName("maxRetDuration");
                writer.WriteValue(filter.maxRetDuration);
                writer.WritePropertyName("maxStop");
                writer.WriteValue(filter.maxStop);

                //airlines[]
                writer.WritePropertyName("airlines");
                writer.WriteStartArray();
                foreach (Airline airline in filter.airlines)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(airline.code);
                    writer.WritePropertyName("name");
                    if (airlineDic.ContainsKey(airline.code))
                    {
                        writer.WriteValue(airlineDic[airline.code]);
                    }
                    else
                    {
                        string name = airline.name;
                        writer.WriteValue(name);
                        airlineDic.Add(airline.code, name);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

                writer.WritePropertyName("departureAirport");
                writer.WriteStartArray();
                foreach (Airport airport in filter.departureAirport)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(airport.code);
                    writer.WritePropertyName("name");
                    if (airportDic.ContainsKey(airport.code))
                    {
                        writer.WriteValue(airlineDic[airport.code]);
                    }
                    else
                    {
                        string name = airport.name;
                        writer.WriteValue(name);
                        airportDic.Add(airport.code, name);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

                writer.WritePropertyName("arrivalAirport");
                writer.WriteStartArray();
                foreach (Airport airport in filter.arrivalAirport)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(airport.code);
                    writer.WritePropertyName("name");
                    if (airportDic.ContainsKey(airport.code))
                    {
                        writer.WriteValue(airlineDic[airport.code]);
                    }
                    else
                    {
                        string name = airport.name;
                        writer.WriteValue(name);
                        airportDic.Add(airport.code, name);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
                // } filter

                //-------------------------
                // "flights": 
                writer.WritePropertyName("flights");
                // [
                writer.WriteStartArray();
                foreach (var flight in flights)
                {
                    //flight{
                    DateTime sDT = DateTime.Now;
                    writer.WriteStartObject();
                    writer.WritePropertyName("id");
                    writer.WriteValue(flight.id);
                    writer.WritePropertyName("type");
                    writer.WriteValue(flight.type);
                    writer.WritePropertyName("maxDepTime");
                    writer.WriteValue(flight.maxDepTime);
                    writer.WritePropertyName("maxRetTime");
                    writer.WriteValue(flight.maxRetTime);

                    //fareRules
                    writer.WritePropertyName("fareRules");
                    writer.WriteStartArray();
                    foreach (var fareRule in flight.fareRule)
                    {
                        writer.WriteValue(fareRule);
                    }
                    writer.WriteEndArray();

                    #region fare
                    if (flight.fare != null)
                    {
                        //fare {
                        writer.WritePropertyName("fare");
                        writer.WriteStartObject();
                        writer.WritePropertyName("currencyCode");
                        writer.WriteValue(flight.fare.currencyCode);
                        writer.WritePropertyName("total");
                        writer.WriteValue(flight.fare.total);

                        //adt
                        writer.WritePropertyName("adtFare");
                        writer.WriteStartObject();
                        writer.WritePropertyName("fare");
                        writer.WriteValue(flight.fare.adtFare.fare);
                        //writer.WritePropertyName("fareBeforeMarkup");
                        //writer.WriteValue(flight.fare.adtFare.fareBeforeMarkup);
                        writer.WritePropertyName("net");
                        writer.WriteValue(flight.fare.adtFare.net);
                        writer.WritePropertyName("tax");
                        writer.WriteValue(flight.fare.adtFare.tax);
                        writer.WriteEndObject();
                        //adt

                        //chd
                        if (flight.fare.chdFare != null)
                        {
                            writer.WritePropertyName("chdFare");
                            writer.WriteStartObject();
                            writer.WritePropertyName("fare");
                            writer.WriteValue(flight.fare.chdFare.fare);
                            writer.WritePropertyName("net");
                            writer.WriteValue(flight.fare.chdFare.net);
                            writer.WritePropertyName("tax");
                            writer.WriteValue(flight.fare.chdFare.tax);
                            writer.WriteEndObject();
                        }
                        //chd

                        //inf
                        if (flight.fare.infFare != null)
                        {
                            writer.WritePropertyName("infFare");
                            writer.WriteStartObject();
                            writer.WritePropertyName("fare");
                            writer.WriteValue(flight.fare.infFare.fare);
                            writer.WritePropertyName("net");
                            writer.WriteValue(flight.fare.infFare.net);
                            writer.WritePropertyName("tax");
                            writer.WriteValue(flight.fare.infFare.tax);
                            writer.WriteEndObject();
                        }
                        //inf


                        writer.WriteEndObject();
                        //}fare
                    }
                    else
                    {
                        writer.WritePropertyName("addOnPrice");
                        writer.WriteValue(flight.addOnPrice);
                    }
                    #endregion

                    #region mainairline
                    writer.WritePropertyName("mainAirline");
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(flight.mainAirline.code);
                    writer.WritePropertyName("name");
                    if (airlineDic.ContainsKey(flight.mainAirline.code))
                    {
                        writer.WriteValue(airlineDic[flight.mainAirline.code]);
                    }
                    else
                    {
                        string name = flight.mainAirline.name;
                        writer.WriteValue(name);
                        airlineDic.Add(flight.mainAirline.code, name);
                    }
                    writer.WriteEndObject();
                    #endregion

                    #region DepFlight
                    writer.WritePropertyName("departureFlight");
                    writer.WriteStartArray();
                    foreach (Flight f in flight.departureFlight)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("bookingCode");
                        writer.WriteValue(f.bookingCode);
                        writer.WritePropertyName("totalTime");
                        writer.WriteValue(f.totalTime);

                        writer.WritePropertyName("flightDetails");
                        writer.WriteStartArray();
                        foreach (FlightDetail d in f.flightDetails)
                        {
                            writer.WriteStartObject();
                            #region airline
                            writer.WritePropertyName("airline");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.airline.code);
                            writer.WritePropertyName("name");
                            if (airlineDic.ContainsKey(d.airline.code))
                            {
                                writer.WriteValue(airlineDic[d.airline.code]);
                            }
                            else
                            {
                                string name = d.airline.name;
                                writer.WriteValue(name);
                                airlineDic.Add(d.airline.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region arrCity
                            writer.WritePropertyName("arrCity");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.arrCity.code);
                            writer.WritePropertyName("terminal");
                            writer.WriteValue(d.arrCity.terminal);
                            writer.WritePropertyName("name");
                            if (airportDic.ContainsKey(d.arrCity.code))
                            {
                                writer.WriteValue(airportDic[d.arrCity.code]);
                            }
                            else
                            {
                                string name = d.arrCity.name;
                                writer.WriteValue(name);
                                airportDic.Add(d.arrCity.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region arrDisplayDateTime
                            writer.WritePropertyName("arrDisplayDateTime");
                            writer.WriteStartObject();
                            writer.WritePropertyName("date");
                            writer.WriteValue(d.arrDisplayDateTime.date);
                            writer.WritePropertyName("displayTime");
                            writer.WriteValue(d.arrDisplayDateTime.displayTime);
                            writer.WritePropertyName("longDate");
                            writer.WriteValue(d.arrDisplayDateTime.longDate);
                            writer.WritePropertyName("longDateWithoutDay");
                            writer.WriteValue(d.arrDisplayDateTime.longDateWithoutDay);
                            writer.WritePropertyName("month");
                            writer.WriteValue(d.arrDisplayDateTime.month);
                            writer.WritePropertyName("shortDate");
                            writer.WriteValue(d.arrDisplayDateTime.shortDate);
                            writer.WritePropertyName("shortDateWithoutDay");
                            writer.WriteValue(d.arrDisplayDateTime.shortDateWithoutDay);
                            writer.WritePropertyName("time");
                            writer.WriteValue(d.arrDisplayDateTime.time);
                            writer.WritePropertyName("year");
                            writer.WriteValue(d.arrDisplayDateTime.year);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("arrivalDateTime");
                            writer.WriteValue(d.arrivalDateTime);
                            writer.WritePropertyName("availableSeat");
                            writer.WriteValue(d.availableSeat);
                            writer.WritePropertyName("cabin");
                            writer.WriteValue(d.cabin);
                            writer.WritePropertyName("connectingTime");
                            writer.WriteValue(d.connectingTime);

                            #region depCity
                            writer.WritePropertyName("depCity");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.depCity.code);
                            writer.WritePropertyName("terminal");
                            writer.WriteValue(d.depCity.terminal);
                            writer.WritePropertyName("name");
                            if (airportDic.ContainsKey(d.depCity.code))
                            {
                                writer.WriteValue(airportDic[d.depCity.code]);
                            }
                            else
                            {
                                string name = d.depCity.name;
                                writer.WriteValue(name);
                                airportDic.Add(d.depCity.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region depDisplayDateTime
                            writer.WritePropertyName("depDisplayDateTime");
                            writer.WriteStartObject();
                            writer.WritePropertyName("date");
                            writer.WriteValue(d.depDisplayDateTime.date);
                            writer.WritePropertyName("displayTime");
                            writer.WriteValue(d.depDisplayDateTime.displayTime);
                            writer.WritePropertyName("longDate");
                            writer.WriteValue(d.depDisplayDateTime.longDate);
                            writer.WritePropertyName("longDateWithoutDay");
                            writer.WriteValue(d.depDisplayDateTime.longDateWithoutDay);
                            writer.WritePropertyName("month");
                            writer.WriteValue(d.depDisplayDateTime.month);
                            writer.WritePropertyName("shortDate");
                            writer.WriteValue(d.depDisplayDateTime.shortDate);
                            writer.WritePropertyName("shortDateWithoutDay");
                            writer.WriteValue(d.depDisplayDateTime.shortDateWithoutDay);
                            writer.WritePropertyName("time");
                            writer.WriteValue(d.depDisplayDateTime.time);
                            writer.WritePropertyName("year");
                            writer.WriteValue(d.depDisplayDateTime.year);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("departureDateTime");
                            writer.WriteValue(d.departureDateTime);
                            writer.WritePropertyName("equipmentType");
                            writer.WriteValue(d.equipmentType);
                            writer.WritePropertyName("equipmentTypeName");
                            writer.WriteValue(d.equipmentTypeName);
                            writer.WritePropertyName("fareBasis");
                            writer.WriteValue(d.fareBasis);
                            writer.WritePropertyName("fareType");
                            writer.WriteValue(d.fareType);
                            writer.WritePropertyName("flightNumber");
                            writer.WriteValue(d.flightNumber);
                            writer.WritePropertyName("flightTime");
                            writer.WriteValue(d.flightTime);

                            #region operatedAirline
                            writer.WritePropertyName("operatedAirline");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.operatedAirline.code);
                            writer.WritePropertyName("name");
                            if (airlineDic.ContainsKey(d.operatedAirline.code))
                            {
                                writer.WriteValue(airlineDic[d.operatedAirline.code]);
                            }
                            else
                            {
                                string name = d.operatedAirline.name;
                                writer.WriteValue(name);
                                airlineDic.Add(d.operatedAirline.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion


                            #region inFlightServices
                            writer.WritePropertyName("inFlightServices");
                            writer.WriteStartObject();
                            writer.WritePropertyName("dutyFreeSale");
                            writer.WriteValue(d.inFlightServices.dutyFreeSale);
                            writer.WritePropertyName("inflightEntertain");
                            writer.WriteValue(d.inFlightServices.inflightEntertain);
                            writer.WritePropertyName("inSeatPower");
                            writer.WriteValue(d.inFlightServices.inSeatPower);
                            writer.WritePropertyName("wifiInternet");
                            writer.WriteValue(d.inFlightServices.wifiInternet);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("rbd");
                            writer.WriteValue(d.rbd);

                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    #endregion

                    if (flight.returnFlight != null)
                    {
                        #region returnFlight
                        writer.WritePropertyName("returnFlight");
                        writer.WriteStartArray();
                        foreach (Flight f in flight.returnFlight)
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName("bookingCode");
                            writer.WriteValue(f.bookingCode);
                            writer.WritePropertyName("totalTime");
                            writer.WriteValue(f.totalTime);

                            writer.WritePropertyName("flightDetails");
                            writer.WriteStartArray();
                            foreach (FlightDetail d in f.flightDetails)
                            {
                                writer.WriteStartObject();
                                #region airline
                                writer.WritePropertyName("airline");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.airline.code);
                                writer.WritePropertyName("name");
                                if (airlineDic.ContainsKey(d.airline.code))
                                {
                                    writer.WriteValue(airlineDic[d.airline.code]);
                                }
                                else
                                {
                                    string name = d.airline.name;
                                    writer.WriteValue(name);
                                    airlineDic.Add(d.airline.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region arrCity
                                writer.WritePropertyName("arrCity");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.arrCity.code);
                                writer.WritePropertyName("terminal");
                                writer.WriteValue(d.arrCity.terminal);
                                writer.WritePropertyName("name");
                                if (airportDic.ContainsKey(d.arrCity.code))
                                {
                                    writer.WriteValue(airportDic[d.arrCity.code]);
                                }
                                else
                                {
                                    string name = d.arrCity.name;
                                    writer.WriteValue(name);
                                    airportDic.Add(d.arrCity.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region arrDisplayDateTime
                                writer.WritePropertyName("arrDisplayDateTime");
                                writer.WriteStartObject();
                                writer.WritePropertyName("date");
                                writer.WriteValue(d.arrDisplayDateTime.date);
                                writer.WritePropertyName("displayTime");
                                writer.WriteValue(d.arrDisplayDateTime.displayTime);
                                writer.WritePropertyName("longDate");
                                writer.WriteValue(d.arrDisplayDateTime.longDate);
                                writer.WritePropertyName("longDateWithoutDay");
                                writer.WriteValue(d.arrDisplayDateTime.longDateWithoutDay);
                                writer.WritePropertyName("month");
                                writer.WriteValue(d.arrDisplayDateTime.month);
                                writer.WritePropertyName("shortDate");
                                writer.WriteValue(d.arrDisplayDateTime.shortDate);
                                writer.WritePropertyName("shortDateWithoutDay");
                                writer.WriteValue(d.arrDisplayDateTime.shortDateWithoutDay);
                                writer.WritePropertyName("time");
                                writer.WriteValue(d.arrDisplayDateTime.time);
                                writer.WritePropertyName("year");
                                writer.WriteValue(d.arrDisplayDateTime.year);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("arrivalDateTime");
                                writer.WriteValue(d.arrivalDateTime);
                                writer.WritePropertyName("availableSeat");
                                writer.WriteValue(d.availableSeat);
                                writer.WritePropertyName("cabin");
                                writer.WriteValue(d.cabin);
                                writer.WritePropertyName("connectingTime");
                                writer.WriteValue(d.connectingTime);

                                #region depCity
                                writer.WritePropertyName("depCity");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.depCity.code);
                                writer.WritePropertyName("terminal");
                                writer.WriteValue(d.depCity.terminal);
                                writer.WritePropertyName("name");
                                if (airportDic.ContainsKey(d.depCity.code))
                                {
                                    writer.WriteValue(airportDic[d.depCity.code]);
                                }
                                else
                                {
                                    string name = d.depCity.name;
                                    writer.WriteValue(name);
                                    airportDic.Add(d.depCity.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region depDisplayDateTime
                                writer.WritePropertyName("depDisplayDateTime");
                                writer.WriteStartObject();
                                writer.WritePropertyName("date");
                                writer.WriteValue(d.depDisplayDateTime.date);
                                writer.WritePropertyName("displayTime");
                                writer.WriteValue(d.depDisplayDateTime.displayTime);
                                writer.WritePropertyName("longDate");
                                writer.WriteValue(d.depDisplayDateTime.longDate);
                                writer.WritePropertyName("longDateWithoutDay");
                                writer.WriteValue(d.depDisplayDateTime.longDateWithoutDay);
                                writer.WritePropertyName("month");
                                writer.WriteValue(d.depDisplayDateTime.month);
                                writer.WritePropertyName("shortDate");
                                writer.WriteValue(d.depDisplayDateTime.shortDate);
                                writer.WritePropertyName("shortDateWithoutDay");
                                writer.WriteValue(d.depDisplayDateTime.shortDateWithoutDay);
                                writer.WritePropertyName("time");
                                writer.WriteValue(d.depDisplayDateTime.time);
                                writer.WritePropertyName("year");
                                writer.WriteValue(d.depDisplayDateTime.year);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("departureDateTime");
                                writer.WriteValue(d.departureDateTime);
                                writer.WritePropertyName("equipmentType");
                                writer.WriteValue(d.equipmentType);
                                writer.WritePropertyName("equipmentTypeName");
                                writer.WriteValue(d.equipmentTypeName);
                                writer.WritePropertyName("fareBasis");
                                writer.WriteValue(d.fareBasis);
                                writer.WritePropertyName("fareType");
                                writer.WriteValue(d.fareType);
                                writer.WritePropertyName("flightNumber");
                                writer.WriteValue(d.flightNumber);
                                writer.WritePropertyName("flightTime");
                                writer.WriteValue(d.flightTime);

                                #region operatedAirline
                                writer.WritePropertyName("operatedAirline");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.operatedAirline.code);
                                writer.WritePropertyName("name");
                                if (airlineDic.ContainsKey(d.operatedAirline.code))
                                {
                                    writer.WriteValue(airlineDic[d.operatedAirline.code]);
                                }
                                else
                                {
                                    string name = d.operatedAirline.name;
                                    writer.WriteValue(name);
                                    airlineDic.Add(d.operatedAirline.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region inFlightServices
                                writer.WritePropertyName("inFlightServices");
                                writer.WriteStartObject();
                                writer.WritePropertyName("dutyFreeSale");
                                writer.WriteValue(d.inFlightServices.dutyFreeSale);
                                writer.WritePropertyName("inflightEntertain");
                                writer.WriteValue(d.inFlightServices.inflightEntertain);
                                writer.WritePropertyName("inSeatPower");
                                writer.WriteValue(d.inFlightServices.inSeatPower);
                                writer.WritePropertyName("wifiInternet");
                                writer.WriteValue(d.inFlightServices.wifiInternet);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("rbd");
                                writer.WriteValue(d.rbd);

                                writer.WriteEndObject();
                            }
                            writer.WriteEndArray();
                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                        #endregion
                    }


                    writer.WriteEndObject();
                    //flight
                }
                writer.WriteEndArray();
                // ] flights
                writer.WriteEndObject();
                // }
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
            return sw.ToString();
        }
        public string toJsonV2()
        {
            StringWriter sw = new StringWriter();
            try
            {
                JsonTextWriter writer = new JsonTextWriter(sw);
                Dictionary<string, string> airlineDic = new Dictionary<string, string>();
                Dictionary<string, string> airportDic = new Dictionary<string, string>();
                // {
                writer.WriteStartObject();

                // "pgSearchOID" : pgSearchOID
                writer.WritePropertyName("pgSearchOID");
                writer.WriteValue(pgSearchOID);

                // "filter": 
                writer.WritePropertyName("filter");
                // {
                writer.WriteStartObject();
                writer.WritePropertyName("maxDepDuration");
                writer.WriteValue(filter.maxDepDuration);
                writer.WritePropertyName("maxPrice");
                writer.WriteValue(filter.maxPrice);
                writer.WritePropertyName("maxRetDuration");
                writer.WriteValue(filter.maxRetDuration);
                writer.WritePropertyName("maxStop");
                writer.WriteValue(filter.maxStop);

                //airlines[]
                writer.WritePropertyName("airlines");
                writer.WriteStartArray();
                foreach (Airline airline in filter.airlines)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(airline.code);
                    writer.WritePropertyName("name");
                    if (airlineDic.ContainsKey(airline.code))
                    {
                        writer.WriteValue(airlineDic[airline.code]);
                    }
                    else
                    {
                        string name = airline.name;
                        writer.WriteValue(name);
                        airlineDic.Add(airline.code, name);
                    }
                    writer.WritePropertyName("minPrice");
                    writer.WriteValue(flights.FirstOrDefault(x => x.mainAirline.code == airline.code).fare.adtFare.net);
                    writer.WritePropertyName("results");
                    writer.WriteValue(flights.Where(x => x.mainAirline.code == airline.code).ToList().Count);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();



                writer.WritePropertyName("departureAirport");
                writer.WriteStartArray();
                foreach (Airport airport in filter.departureAirport)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(airport.code);
                    writer.WritePropertyName("name");
                    if (airportDic.ContainsKey(airport.code))
                    {
                        writer.WriteValue(airlineDic[airport.code]);
                    }
                    else
                    {
                        string name = airport.name;
                        writer.WriteValue(name);
                        airportDic.Add(airport.code, name);
                    }
                    writer.WritePropertyName("results");
                    writer.WriteValue(flights.Where(x => x.departureFlight[0].flightDetails[0].depCity.code == airport.code).ToList().Count);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

                writer.WritePropertyName("arrivalAirport");
                writer.WriteStartArray();
                foreach (Airport airport in filter.arrivalAirport)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(airport.code);
                    writer.WritePropertyName("name");
                    if (airportDic.ContainsKey(airport.code))
                    {
                        writer.WriteValue(airlineDic[airport.code]);
                    }
                    else
                    {
                        string name = airport.name;
                        writer.WriteValue(name);
                        airportDic.Add(airport.code, name);
                    }
                    writer.WritePropertyName("results");
                    writer.WriteValue(flights.Where(x => x.departureFlight[0].flightDetails[x.departureFlight[0].flightDetails.Count - 1].arrCity.code == airport.code).ToList().Count);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
                // } filter

                //-------------------------
                // "flights": 
                writer.WritePropertyName("flights");
                // [
                writer.WriteStartArray();
                foreach (var flight in flights)
                {
                    //flight{
                    DateTime sDT = DateTime.Now;
                    writer.WriteStartObject();
                    writer.WritePropertyName("id");
                    writer.WriteValue(flight.id);
                    writer.WritePropertyName("type");
                    writer.WriteValue(flight.type);
                    writer.WritePropertyName("maxDepTime");
                    writer.WriteValue(flight.maxDepTime);
                    writer.WritePropertyName("maxRetTime");
                    writer.WriteValue(flight.maxRetTime);

                    //for sort
                    writer.WritePropertyName("price");
                    writer.WriteValue(flight.fare.adtFare.net);
                    int shortDuration = flight.departureFlight.Min(x => Convert.ToInt32(x.totalTime));
                    if (flight.returnFlight != null)
                    {
                        int returnShortDuration = flight.returnFlight.Min(x => Convert.ToInt32(x.totalTime));
                        if (returnShortDuration < shortDuration)
                        {
                            shortDuration = returnShortDuration;
                        }
                    }
                    writer.WritePropertyName("shortDuration");
                    writer.WriteValue(shortDuration);

                    int longDuration = flight.departureFlight.Max(x => Convert.ToInt32(x.totalTime));
                    if (flight.returnFlight != null)
                    {
                        int returnlongDuration = flight.returnFlight.Max(x => Convert.ToInt32(x.totalTime));
                        if (returnlongDuration > longDuration)
                        {
                            longDuration = returnlongDuration;
                        }
                    }
                    writer.WritePropertyName("longDuration");
                    writer.WriteValue(longDuration);
                    writer.WritePropertyName("depCity");
                    writer.WriteValue(flight.departureFlight[0].flightDetails[0].depCity.code);
                    writer.WritePropertyName("arrCity");
                    writer.WriteValue(flight.departureFlight[0].flightDetails[flight.departureFlight[0].flightDetails.Count - 1].arrCity.code);

                    //fareRules
                    writer.WritePropertyName("fareRules");
                    writer.WriteStartArray();
                    foreach (var fareRule in flight.fareRule)
                    {
                        writer.WriteValue(fareRule);
                    }
                    writer.WriteEndArray();

                    #region fare
                    if (flight.fare != null)
                    {
                        //fare {
                        writer.WritePropertyName("fare");
                        writer.WriteStartObject();
                        writer.WritePropertyName("currencyCode");
                        writer.WriteValue(flight.fare.currencyCode);
                        writer.WritePropertyName("total");
                        writer.WriteValue(flight.fare.total);

                        //adt
                        writer.WritePropertyName("adtFare");
                        writer.WriteStartObject();
                        writer.WritePropertyName("fare");
                        writer.WriteValue(flight.fare.adtFare.fare);
                        //writer.WritePropertyName("fareBeforeMarkup");
                        //writer.WriteValue(flight.fare.adtFare.fareBeforeMarkup);
                        writer.WritePropertyName("net");
                        writer.WriteValue(flight.fare.adtFare.net);
                        writer.WritePropertyName("tax");
                        writer.WriteValue(flight.fare.adtFare.tax);
                        writer.WriteEndObject();
                        //adt

                        //chd
                        if (flight.fare.chdFare != null)
                        {
                            writer.WritePropertyName("chdFare");
                            writer.WriteStartObject();
                            writer.WritePropertyName("fare");
                            writer.WriteValue(flight.fare.chdFare.fare);
                            writer.WritePropertyName("net");
                            writer.WriteValue(flight.fare.chdFare.net);
                            writer.WritePropertyName("tax");
                            writer.WriteValue(flight.fare.chdFare.tax);
                            writer.WriteEndObject();
                        }
                        //chd

                        //inf
                        if (flight.fare.infFare != null)
                        {
                            writer.WritePropertyName("infFare");
                            writer.WriteStartObject();
                            writer.WritePropertyName("fare");
                            writer.WriteValue(flight.fare.infFare.fare);
                            writer.WritePropertyName("net");
                            writer.WriteValue(flight.fare.infFare.net);
                            writer.WritePropertyName("tax");
                            writer.WriteValue(flight.fare.infFare.tax);
                            writer.WriteEndObject();
                        }
                        //inf


                        writer.WriteEndObject();
                        //}fare
                    }
                    else
                    {
                        writer.WritePropertyName("addOnPrice");
                        writer.WriteValue(flight.addOnPrice);
                    }
                    #endregion

                    #region mainairline
                    writer.WritePropertyName("mainAirline");
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(flight.mainAirline.code);
                    writer.WritePropertyName("name");
                    if (airlineDic.ContainsKey(flight.mainAirline.code))
                    {
                        writer.WriteValue(airlineDic[flight.mainAirline.code]);
                    }
                    else
                    {
                        string name = flight.mainAirline.name;
                        writer.WriteValue(name);
                        airlineDic.Add(flight.mainAirline.code, name);
                    }
                    writer.WriteEndObject();
                    #endregion

                    #region DepFlight
                    writer.WritePropertyName("departureFlight");
                    writer.WriteStartArray();
                    foreach (Flight f in flight.departureFlight)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("bookingCode");
                        writer.WriteValue(f.bookingCode);
                        writer.WritePropertyName("totalTime");
                        writer.WriteValue(f.totalTime);

                        writer.WritePropertyName("flightDetails");
                        writer.WriteStartArray();
                        foreach (FlightDetail d in f.flightDetails)
                        {
                            writer.WriteStartObject();
                            #region airline
                            writer.WritePropertyName("airline");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.airline.code);
                            writer.WritePropertyName("name");
                            if (airlineDic.ContainsKey(d.airline.code))
                            {
                                writer.WriteValue(airlineDic[d.airline.code]);
                            }
                            else
                            {
                                string name = d.airline.name;
                                writer.WriteValue(name);
                                airlineDic.Add(d.airline.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region arrCity
                            writer.WritePropertyName("arrCity");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.arrCity.code);
                            writer.WritePropertyName("terminal");
                            writer.WriteValue(d.arrCity.terminal);
                            writer.WritePropertyName("name");
                            if (airportDic.ContainsKey(d.arrCity.code))
                            {
                                writer.WriteValue(airportDic[d.arrCity.code]);
                            }
                            else
                            {
                                string name = d.arrCity.name;
                                writer.WriteValue(name);
                                airportDic.Add(d.arrCity.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region arrDisplayDateTime
                            writer.WritePropertyName("arrDisplayDateTime");
                            writer.WriteStartObject();
                            writer.WritePropertyName("date");
                            writer.WriteValue(d.arrDisplayDateTime.date);
                            writer.WritePropertyName("displayTime");
                            writer.WriteValue(d.arrDisplayDateTime.displayTime);
                            writer.WritePropertyName("longDate");
                            writer.WriteValue(d.arrDisplayDateTime.longDate);
                            writer.WritePropertyName("longDateWithoutDay");
                            writer.WriteValue(d.arrDisplayDateTime.longDateWithoutDay);
                            writer.WritePropertyName("month");
                            writer.WriteValue(d.arrDisplayDateTime.month);
                            writer.WritePropertyName("shortDate");
                            writer.WriteValue(d.arrDisplayDateTime.shortDate);
                            writer.WritePropertyName("shortDateWithoutDay");
                            writer.WriteValue(d.arrDisplayDateTime.shortDateWithoutDay);
                            writer.WritePropertyName("time");
                            writer.WriteValue(d.arrDisplayDateTime.time);
                            writer.WritePropertyName("year");
                            writer.WriteValue(d.arrDisplayDateTime.year);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("arrivalDateTime");
                            writer.WriteValue(d.arrivalDateTime);
                            writer.WritePropertyName("availableSeat");
                            writer.WriteValue(d.availableSeat);
                            writer.WritePropertyName("cabin");
                            writer.WriteValue(d.cabin);
                            writer.WritePropertyName("connectingTime");
                            writer.WriteValue(d.connectingTime);

                            #region depCity
                            writer.WritePropertyName("depCity");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.depCity.code);
                            writer.WritePropertyName("terminal");
                            writer.WriteValue(d.depCity.terminal);
                            writer.WritePropertyName("name");
                            if (airportDic.ContainsKey(d.depCity.code))
                            {
                                writer.WriteValue(airportDic[d.depCity.code]);
                            }
                            else
                            {
                                string name = d.depCity.name;
                                writer.WriteValue(name);
                                airportDic.Add(d.depCity.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region depDisplayDateTime
                            writer.WritePropertyName("depDisplayDateTime");
                            writer.WriteStartObject();
                            writer.WritePropertyName("date");
                            writer.WriteValue(d.depDisplayDateTime.date);
                            writer.WritePropertyName("displayTime");
                            writer.WriteValue(d.depDisplayDateTime.displayTime);
                            writer.WritePropertyName("longDate");
                            writer.WriteValue(d.depDisplayDateTime.longDate);
                            writer.WritePropertyName("longDateWithoutDay");
                            writer.WriteValue(d.depDisplayDateTime.longDateWithoutDay);
                            writer.WritePropertyName("month");
                            writer.WriteValue(d.depDisplayDateTime.month);
                            writer.WritePropertyName("shortDate");
                            writer.WriteValue(d.depDisplayDateTime.shortDate);
                            writer.WritePropertyName("shortDateWithoutDay");
                            writer.WriteValue(d.depDisplayDateTime.shortDateWithoutDay);
                            writer.WritePropertyName("time");
                            writer.WriteValue(d.depDisplayDateTime.time);
                            writer.WritePropertyName("year");
                            writer.WriteValue(d.depDisplayDateTime.year);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("departureDateTime");
                            writer.WriteValue(d.departureDateTime);
                            writer.WritePropertyName("equipmentType");
                            writer.WriteValue(d.equipmentType);
                            writer.WritePropertyName("equipmentTypeName");
                            writer.WriteValue(d.equipmentTypeName);
                            writer.WritePropertyName("fareBasis");
                            writer.WriteValue(d.fareBasis);
                            writer.WritePropertyName("fareType");
                            writer.WriteValue(d.fareType);
                            writer.WritePropertyName("flightNumber");
                            writer.WriteValue(d.flightNumber);
                            writer.WritePropertyName("flightTime");
                            writer.WriteValue(d.flightTime);

                            #region operatedAirline
                            writer.WritePropertyName("operatedAirline");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.operatedAirline.code);
                            writer.WritePropertyName("name");
                            if (airlineDic.ContainsKey(d.operatedAirline.code))
                            {
                                writer.WriteValue(airlineDic[d.operatedAirline.code]);
                            }
                            else
                            {
                                string name = d.operatedAirline.name;
                                writer.WriteValue(name);
                                airlineDic.Add(d.operatedAirline.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region inFlightServices
                            writer.WritePropertyName("inFlightServices");
                            writer.WriteStartObject();
                            writer.WritePropertyName("dutyFreeSale");
                            writer.WriteValue(d.inFlightServices.dutyFreeSale);
                            writer.WritePropertyName("inflightEntertain");
                            writer.WriteValue(d.inFlightServices.inflightEntertain);
                            writer.WritePropertyName("inSeatPower");
                            writer.WriteValue(d.inFlightServices.inSeatPower);
                            writer.WritePropertyName("wifiInternet");
                            writer.WriteValue(d.inFlightServices.wifiInternet);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("rbd");
                            writer.WriteValue(d.rbd);

                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    #endregion

                    if (flight.returnFlight != null)
                    {
                        #region returnFlight
                        writer.WritePropertyName("returnFlight");
                        writer.WriteStartArray();
                        foreach (Flight f in flight.returnFlight)
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName("bookingCode");
                            writer.WriteValue(f.bookingCode);
                            writer.WritePropertyName("totalTime");
                            writer.WriteValue(f.totalTime);

                            writer.WritePropertyName("flightDetails");
                            writer.WriteStartArray();
                            foreach (FlightDetail d in f.flightDetails)
                            {
                                writer.WriteStartObject();
                                #region airline
                                writer.WritePropertyName("airline");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.airline.code);
                                writer.WritePropertyName("name");
                                if (airlineDic.ContainsKey(d.airline.code))
                                {
                                    writer.WriteValue(airlineDic[d.airline.code]);
                                }
                                else
                                {
                                    string name = d.airline.name;
                                    writer.WriteValue(name);
                                    airlineDic.Add(d.airline.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region arrCity
                                writer.WritePropertyName("arrCity");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.arrCity.code);
                                writer.WritePropertyName("terminal");
                                writer.WriteValue(d.arrCity.terminal);
                                writer.WritePropertyName("name");
                                if (airportDic.ContainsKey(d.arrCity.code))
                                {
                                    writer.WriteValue(airportDic[d.arrCity.code]);
                                }
                                else
                                {
                                    string name = d.arrCity.name;
                                    writer.WriteValue(name);
                                    airportDic.Add(d.arrCity.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region arrDisplayDateTime
                                writer.WritePropertyName("arrDisplayDateTime");
                                writer.WriteStartObject();
                                writer.WritePropertyName("date");
                                writer.WriteValue(d.arrDisplayDateTime.date);
                                writer.WritePropertyName("displayTime");
                                writer.WriteValue(d.arrDisplayDateTime.displayTime);
                                writer.WritePropertyName("longDate");
                                writer.WriteValue(d.arrDisplayDateTime.longDate);
                                writer.WritePropertyName("longDateWithoutDay");
                                writer.WriteValue(d.arrDisplayDateTime.longDateWithoutDay);
                                writer.WritePropertyName("month");
                                writer.WriteValue(d.arrDisplayDateTime.month);
                                writer.WritePropertyName("shortDate");
                                writer.WriteValue(d.arrDisplayDateTime.shortDate);
                                writer.WritePropertyName("shortDateWithoutDay");
                                writer.WriteValue(d.arrDisplayDateTime.shortDateWithoutDay);
                                writer.WritePropertyName("time");
                                writer.WriteValue(d.arrDisplayDateTime.time);
                                writer.WritePropertyName("year");
                                writer.WriteValue(d.arrDisplayDateTime.year);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("arrivalDateTime");
                                writer.WriteValue(d.arrivalDateTime);
                                writer.WritePropertyName("availableSeat");
                                writer.WriteValue(d.availableSeat);
                                writer.WritePropertyName("cabin");
                                writer.WriteValue(d.cabin);
                                writer.WritePropertyName("connectingTime");
                                writer.WriteValue(d.connectingTime);

                                #region depCity
                                writer.WritePropertyName("depCity");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.depCity.code);
                                writer.WritePropertyName("terminal");
                                writer.WriteValue(d.depCity.terminal);
                                writer.WritePropertyName("name");
                                if (airportDic.ContainsKey(d.depCity.code))
                                {
                                    writer.WriteValue(airportDic[d.depCity.code]);
                                }
                                else
                                {
                                    string name = d.depCity.name;
                                    writer.WriteValue(name);
                                    airportDic.Add(d.depCity.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region depDisplayDateTime
                                writer.WritePropertyName("depDisplayDateTime");
                                writer.WriteStartObject();
                                writer.WritePropertyName("date");
                                writer.WriteValue(d.depDisplayDateTime.date);
                                writer.WritePropertyName("displayTime");
                                writer.WriteValue(d.depDisplayDateTime.displayTime);
                                writer.WritePropertyName("longDate");
                                writer.WriteValue(d.depDisplayDateTime.longDate);
                                writer.WritePropertyName("longDateWithoutDay");
                                writer.WriteValue(d.depDisplayDateTime.longDateWithoutDay);
                                writer.WritePropertyName("month");
                                writer.WriteValue(d.depDisplayDateTime.month);
                                writer.WritePropertyName("shortDate");
                                writer.WriteValue(d.depDisplayDateTime.shortDate);
                                writer.WritePropertyName("shortDateWithoutDay");
                                writer.WriteValue(d.depDisplayDateTime.shortDateWithoutDay);
                                writer.WritePropertyName("time");
                                writer.WriteValue(d.depDisplayDateTime.time);
                                writer.WritePropertyName("year");
                                writer.WriteValue(d.depDisplayDateTime.year);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("departureDateTime");
                                writer.WriteValue(d.departureDateTime);
                                writer.WritePropertyName("equipmentType");
                                writer.WriteValue(d.equipmentType);
                                writer.WritePropertyName("equipmentTypeName");
                                writer.WriteValue(d.equipmentTypeName);
                                writer.WritePropertyName("fareBasis");
                                writer.WriteValue(d.fareBasis);
                                writer.WritePropertyName("fareType");
                                writer.WriteValue(d.fareType);
                                writer.WritePropertyName("flightNumber");
                                writer.WriteValue(d.flightNumber);
                                writer.WritePropertyName("flightTime");
                                writer.WriteValue(d.flightTime);

                                #region operatedAirline
                                writer.WritePropertyName("operatedAirline");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.operatedAirline.code);
                                writer.WritePropertyName("name");
                                if (airlineDic.ContainsKey(d.operatedAirline.code))
                                {
                                    writer.WriteValue(airlineDic[d.operatedAirline.code]);
                                }
                                else
                                {
                                    string name = d.operatedAirline.name;
                                    writer.WriteValue(name);
                                    airlineDic.Add(d.operatedAirline.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region inFlightServices
                                writer.WritePropertyName("inFlightServices");
                                writer.WriteStartObject();
                                writer.WritePropertyName("dutyFreeSale");
                                writer.WriteValue(d.inFlightServices.dutyFreeSale);
                                writer.WritePropertyName("inflightEntertain");
                                writer.WriteValue(d.inFlightServices.inflightEntertain);
                                writer.WritePropertyName("inSeatPower");
                                writer.WriteValue(d.inFlightServices.inSeatPower);
                                writer.WritePropertyName("wifiInternet");
                                writer.WriteValue(d.inFlightServices.wifiInternet);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("rbd");
                                writer.WriteValue(d.rbd);

                                writer.WriteEndObject();
                            }
                            writer.WriteEndArray();
                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                        #endregion
                    }


                    writer.WriteEndObject();
                    //flight
                }
                writer.WriteEndArray();
                // ] flights
                writer.WriteEndObject();
                // }
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
            return sw.ToString();
        }
        public string toJsonMultiDestination()
        {
            StringWriter sw = new StringWriter();
            try
            {
                JsonTextWriter writer = new JsonTextWriter(sw);
                Dictionary<string, string> airlineDic = new Dictionary<string, string>();
                Dictionary<string, string> airportDic = new Dictionary<string, string>();
                // {
                writer.WriteStartObject();

                // "pgSearchOID" : pgSearchOID
                writer.WritePropertyName("pgSearchOID");
                writer.WriteValue(pgSearchOID);

                // "filter": 
                writer.WritePropertyName("filter");
                // {
                writer.WriteStartObject();
                writer.WritePropertyName("maxDepDuration");
                writer.WriteValue(filter.maxDepDuration);
                writer.WritePropertyName("maxPrice");
                writer.WriteValue(filter.maxPrice);
                writer.WritePropertyName("maxRetDuration");
                writer.WriteValue(filter.maxRetDuration);
                writer.WritePropertyName("maxStop");
                writer.WriteValue(filter.maxStop);

                //airlines[]
                writer.WritePropertyName("airlines");
                writer.WriteStartArray();
                foreach (Airline airline in filter.airlines)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(airline.code);
                    writer.WritePropertyName("name");
                    if (airlineDic.ContainsKey(airline.code))
                    {
                        writer.WriteValue(airlineDic[airline.code]);
                    }
                    else
                    {
                        string name = airline.name;
                        writer.WriteValue(name);
                        airlineDic.Add(airline.code, name);
                    }
                    writer.WritePropertyName("minPrice");
                    writer.WriteValue(flights.FirstOrDefault(x => x.mainAirline.code == airline.code).fare.adtFare.net);
                    writer.WritePropertyName("results");
                    writer.WriteValue(flights.Where(x => x.mainAirline.code == airline.code).ToList().Count);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();


                if (filter.departureAirport != null)
                {
                    writer.WritePropertyName("departureAirport");
                    writer.WriteStartArray();
                    foreach (Airport airport in filter.departureAirport)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("code");
                        writer.WriteValue(airport.code);
                        writer.WritePropertyName("name");
                        if (airportDic.ContainsKey(airport.code))
                        {
                            writer.WriteValue(airlineDic[airport.code]);
                        }
                        else
                        {
                            string name = airport.name;
                            writer.WriteValue(name);
                            airportDic.Add(airport.code, name);
                        }
                        writer.WritePropertyName("results");
                        writer.WriteValue(flights.Where(x => x.departureFlight[0].flightDetails[0].depCity.code == airport.code).ToList().Count);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }
                if (filter.arrivalAirport != null)
                {
                    writer.WritePropertyName("arrivalAirport");
                    writer.WriteStartArray();
                    foreach (Airport airport in filter.arrivalAirport)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("code");
                        writer.WriteValue(airport.code);
                        writer.WritePropertyName("name");
                        if (airportDic.ContainsKey(airport.code))
                        {
                            writer.WriteValue(airlineDic[airport.code]);
                        }
                        else
                        {
                            string name = airport.name;
                            writer.WriteValue(name);
                            airportDic.Add(airport.code, name);
                        }
                        writer.WritePropertyName("results");
                        writer.WriteValue(flights.Where(x => x.departureFlight[0].flightDetails[x.departureFlight[0].flightDetails.Count - 1].arrCity.code == airport.code).ToList().Count);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }

                writer.WriteEndObject();
                // } filter

                

                //-------------------------
                // "flights": 
                writer.WritePropertyName("flights");
                // [
                writer.WriteStartArray();
                foreach (var flight in flights)
                {
                    //flight{
                    DateTime sDT = DateTime.Now;
                    writer.WriteStartObject();
                    writer.WritePropertyName("id");
                    writer.WriteValue(flight.id);
                    writer.WritePropertyName("type");
                    writer.WriteValue(flight.type);
                    writer.WritePropertyName("maxSegRef1Time");
                    writer.WriteValue(flight.maxSegRef1Time);
                    writer.WritePropertyName("maxSegRef2Time");
                    writer.WriteValue(flight.maxSegRef2Time);
                    writer.WritePropertyName("maxSegRef3Time");
                    writer.WriteValue(flight.maxSegRef3Time);

                    //for sort
                    writer.WritePropertyName("price");
                    writer.WriteValue(flight.fare.adtFare.net);
                    int shortDuration = flight.Flight_SegRef1.Min(x => Convert.ToInt32(x.totalTime));
                    if (flight.Flight_SegRef2 != null)
                    {
                        int segRef2ShortDuration = flight.Flight_SegRef2.Min(x => Convert.ToInt32(x.totalTime));
                        if (segRef2ShortDuration < shortDuration)
                        {
                            shortDuration = segRef2ShortDuration;
                        }
                    }
                    if (flight.Flight_SegRef3 != null)
                    {
                        int segRef3ShortDuration = flight.Flight_SegRef3.Min(x => Convert.ToInt32(x.totalTime));
                        if (segRef3ShortDuration < shortDuration)
                        {
                            shortDuration = segRef3ShortDuration;
                        }
                    }
                    writer.WritePropertyName("shortDuration");
                    writer.WriteValue(shortDuration);

                    int longDuration = flight.Flight_SegRef1.Max(x => Convert.ToInt32(x.totalTime));
                    if (flight.Flight_SegRef2 != null)
                    {
                        int segRef2longDuration = flight.Flight_SegRef2.Max(x => Convert.ToInt32(x.totalTime));
                        if (segRef2longDuration > longDuration)
                        {
                            longDuration = segRef2longDuration;
                        }
                    }
                    if (flight.Flight_SegRef3 != null)
                    {
                        int segRef3longDuration = flight.Flight_SegRef3.Max(x => Convert.ToInt32(x.totalTime));
                        if (segRef3longDuration > longDuration)
                        {
                            longDuration = segRef3longDuration;
                        }
                    }
                    writer.WritePropertyName("longDuration");
                    writer.WriteValue(longDuration);

                    writer.WritePropertyName("earlyDepartTime");
                    writer.WriteValue(flight.Flight_SegRef1[0].flightDetails[0].departureDateTime);
                    writer.WritePropertyName("lastDepartTime");
                    writer.WriteValue(flight.Flight_SegRef1[0].flightDetails[0].departureDateTime);

                    writer.WritePropertyName("depCitySeg1");
                    writer.WriteValue(flight.Flight_SegRef1[0].flightDetails[0].depCity.code);
                    writer.WritePropertyName("arrCitySeg1");
                    writer.WriteValue(flight.Flight_SegRef1[0].flightDetails[flight.Flight_SegRef1[0].flightDetails.Count - 1].arrCity.code);

                    writer.WritePropertyName("depCitySeg2");
                    writer.WriteValue(flight.Flight_SegRef2[0].flightDetails[0].depCity.code);
                    writer.WritePropertyName("arrCitySeg2");
                    writer.WriteValue(flight.Flight_SegRef2[0].flightDetails[flight.Flight_SegRef2[0].flightDetails.Count - 1].arrCity.code);

                    if (flight.Flight_SegRef3 != null && flight.Flight_SegRef3.Count > 0)
                    {
                        writer.WritePropertyName("depCitySeg3");
                        writer.WriteValue(flight.Flight_SegRef3[0].flightDetails[0].depCity.code);
                        writer.WritePropertyName("arrCitySeg3");
                        writer.WriteValue(flight.Flight_SegRef3[0].flightDetails[flight.Flight_SegRef3[0].flightDetails.Count - 1].arrCity.code);
                    }
                    //fareRules
                    writer.WritePropertyName("fareRules");
                    writer.WriteStartArray();
                    foreach (var fareRule in flight.fareRule)
                    {
                        writer.WriteValue(fareRule);
                    }
                    writer.WriteEndArray();

                    #region fare
                    if (flight.fare != null)
                    {
                        //fare {
                        writer.WritePropertyName("fare");
                        writer.WriteStartObject();
                        writer.WritePropertyName("currencyCode");
                        writer.WriteValue(flight.fare.currencyCode);
                        writer.WritePropertyName("total");
                        writer.WriteValue(flight.fare.total);

                        //adt
                        writer.WritePropertyName("adtFare");
                        writer.WriteStartObject();
                        writer.WritePropertyName("fare");
                        writer.WriteValue(flight.fare.adtFare.fare);
                        //writer.WritePropertyName("fareBeforeMarkup");
                        //writer.WriteValue(flight.fare.adtFare.fareBeforeMarkup);
                        writer.WritePropertyName("net");
                        writer.WriteValue(flight.fare.adtFare.net);
                        writer.WritePropertyName("tax");
                        writer.WriteValue(flight.fare.adtFare.tax);
                        writer.WriteEndObject();
                        //adt

                        //chd
                        if (flight.fare.chdFare != null)
                        {
                            writer.WritePropertyName("chdFare");
                            writer.WriteStartObject();
                            writer.WritePropertyName("fare");
                            writer.WriteValue(flight.fare.chdFare.fare);
                            writer.WritePropertyName("net");
                            writer.WriteValue(flight.fare.chdFare.net);
                            writer.WritePropertyName("tax");
                            writer.WriteValue(flight.fare.chdFare.tax);
                            writer.WriteEndObject();
                        }
                        //chd

                        //inf
                        if (flight.fare.infFare != null)
                        {
                            writer.WritePropertyName("infFare");
                            writer.WriteStartObject();
                            writer.WritePropertyName("fare");
                            writer.WriteValue(flight.fare.infFare.fare);
                            writer.WritePropertyName("net");
                            writer.WriteValue(flight.fare.infFare.net);
                            writer.WritePropertyName("tax");
                            writer.WriteValue(flight.fare.infFare.tax);
                            writer.WriteEndObject();
                        }
                        //inf


                        writer.WriteEndObject();
                        //}fare
                    }
                    else
                    {
                        writer.WritePropertyName("addOnPrice");
                        writer.WriteValue(flight.addOnPrice);
                    }
                    #endregion

                    #region mainairline
                    writer.WritePropertyName("mainAirline");
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(flight.mainAirline.code);
                    writer.WritePropertyName("name");
                    if (airlineDic.ContainsKey(flight.mainAirline.code))
                    {
                        writer.WriteValue(airlineDic[flight.mainAirline.code]);
                    }
                    else
                    {
                        string name = flight.mainAirline.name;
                        writer.WriteValue(name);
                        airlineDic.Add(flight.mainAirline.code, name);
                    }
                    writer.WriteEndObject();
                    #endregion

                    #region Flight_SegRef1
                    writer.WritePropertyName("Flight_SegRef1");
                    writer.WriteStartArray();
                    foreach (Flight f in flight.Flight_SegRef1)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("bookingCode");
                        writer.WriteValue(f.bookingCode);
                        writer.WritePropertyName("totalTime");
                        writer.WriteValue(f.totalTime);

                        writer.WritePropertyName("flightDetails");
                        writer.WriteStartArray();
                        foreach (FlightDetail d in f.flightDetails)
                        {
                            writer.WriteStartObject();
                            #region airline
                            writer.WritePropertyName("airline");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.airline.code);
                            writer.WritePropertyName("name");
                            if (airlineDic.ContainsKey(d.airline.code))
                            {
                                writer.WriteValue(airlineDic[d.airline.code]);
                            }
                            else
                            {
                                string name = d.airline.name;
                                writer.WriteValue(name);
                                airlineDic.Add(d.airline.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region arrCity
                            writer.WritePropertyName("arrCity");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.arrCity.code);
                            writer.WritePropertyName("terminal");
                            writer.WriteValue(d.arrCity.terminal);
                            writer.WritePropertyName("name");
                            if (airportDic.ContainsKey(d.arrCity.code))
                            {
                                writer.WriteValue(airportDic[d.arrCity.code]);
                            }
                            else
                            {
                                string name = d.arrCity.name;
                                writer.WriteValue(name);
                                airportDic.Add(d.arrCity.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region arrDisplayDateTime
                            writer.WritePropertyName("arrDisplayDateTime");
                            writer.WriteStartObject();
                            writer.WritePropertyName("date");
                            writer.WriteValue(d.arrDisplayDateTime.date);
                            writer.WritePropertyName("displayTime");
                            writer.WriteValue(d.arrDisplayDateTime.displayTime);
                            writer.WritePropertyName("longDate");
                            writer.WriteValue(d.arrDisplayDateTime.longDate);
                            writer.WritePropertyName("longDateWithoutDay");
                            writer.WriteValue(d.arrDisplayDateTime.longDateWithoutDay);
                            writer.WritePropertyName("month");
                            writer.WriteValue(d.arrDisplayDateTime.month);
                            writer.WritePropertyName("shortDate");
                            writer.WriteValue(d.arrDisplayDateTime.shortDate);
                            writer.WritePropertyName("shortDateWithoutDay");
                            writer.WriteValue(d.arrDisplayDateTime.shortDateWithoutDay);
                            writer.WritePropertyName("time");
                            writer.WriteValue(d.arrDisplayDateTime.time);
                            writer.WritePropertyName("year");
                            writer.WriteValue(d.arrDisplayDateTime.year);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("arrivalDateTime");
                            writer.WriteValue(d.arrivalDateTime);
                            writer.WritePropertyName("availableSeat");
                            writer.WriteValue(d.availableSeat);
                            writer.WritePropertyName("cabin");
                            writer.WriteValue(d.cabin);
                            writer.WritePropertyName("connectingTime");
                            writer.WriteValue(d.connectingTime);

                            #region depCity
                            writer.WritePropertyName("depCity");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.depCity.code);
                            writer.WritePropertyName("terminal");
                            writer.WriteValue(d.depCity.terminal);
                            writer.WritePropertyName("name");
                            if (airportDic.ContainsKey(d.depCity.code))
                            {
                                writer.WriteValue(airportDic[d.depCity.code]);
                            }
                            else
                            {
                                string name = d.depCity.name;
                                writer.WriteValue(name);
                                airportDic.Add(d.depCity.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region depDisplayDateTime
                            writer.WritePropertyName("depDisplayDateTime");
                            writer.WriteStartObject();
                            writer.WritePropertyName("date");
                            writer.WriteValue(d.depDisplayDateTime.date);
                            writer.WritePropertyName("displayTime");
                            writer.WriteValue(d.depDisplayDateTime.displayTime);
                            writer.WritePropertyName("longDate");
                            writer.WriteValue(d.depDisplayDateTime.longDate);
                            writer.WritePropertyName("longDateWithoutDay");
                            writer.WriteValue(d.depDisplayDateTime.longDateWithoutDay);
                            writer.WritePropertyName("month");
                            writer.WriteValue(d.depDisplayDateTime.month);
                            writer.WritePropertyName("shortDate");
                            writer.WriteValue(d.depDisplayDateTime.shortDate);
                            writer.WritePropertyName("shortDateWithoutDay");
                            writer.WriteValue(d.depDisplayDateTime.shortDateWithoutDay);
                            writer.WritePropertyName("time");
                            writer.WriteValue(d.depDisplayDateTime.time);
                            writer.WritePropertyName("year");
                            writer.WriteValue(d.depDisplayDateTime.year);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("departureDateTime");
                            writer.WriteValue(d.departureDateTime);
                            writer.WritePropertyName("equipmentType");
                            writer.WriteValue(d.equipmentType);
                            writer.WritePropertyName("equipmentTypeName");
                            writer.WriteValue(d.equipmentTypeName);
                            writer.WritePropertyName("fareBasis");
                            writer.WriteValue(d.fareBasis);
                            writer.WritePropertyName("fareType");
                            writer.WriteValue(d.fareType);
                            writer.WritePropertyName("flightNumber");
                            writer.WriteValue(d.flightNumber);
                            writer.WritePropertyName("flightTime");
                            writer.WriteValue(d.flightTime);

                            #region operatedAirline
                            writer.WritePropertyName("operatedAirline");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.operatedAirline.code);
                            writer.WritePropertyName("name");
                            if (airlineDic.ContainsKey(d.operatedAirline.code))
                            {
                                writer.WriteValue(airlineDic[d.operatedAirline.code]);
                            }
                            else
                            {
                                string name = d.operatedAirline.name;
                                writer.WriteValue(name);
                                airlineDic.Add(d.operatedAirline.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region inFlightServices
                            writer.WritePropertyName("inFlightServices");
                            writer.WriteStartObject();
                            writer.WritePropertyName("dutyFreeSale");
                            writer.WriteValue(d.inFlightServices.dutyFreeSale);
                            writer.WritePropertyName("inflightEntertain");
                            writer.WriteValue(d.inFlightServices.inflightEntertain);
                            writer.WritePropertyName("inSeatPower");
                            writer.WriteValue(d.inFlightServices.inSeatPower);
                            writer.WritePropertyName("wifiInternet");
                            writer.WriteValue(d.inFlightServices.wifiInternet);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("rbd");
                            writer.WriteValue(d.rbd);

                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    #endregion

                    if (flight.Flight_SegRef2 != null)
                    {
                        #region Flight_SegRef2
                        writer.WritePropertyName("Flight_SegRef2");
                        writer.WriteStartArray();
                        foreach (Flight f in flight.Flight_SegRef2)
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName("bookingCode");
                            writer.WriteValue(f.bookingCode);
                            writer.WritePropertyName("totalTime");
                            writer.WriteValue(f.totalTime);

                            writer.WritePropertyName("flightDetails");
                            writer.WriteStartArray();
                            foreach (FlightDetail d in f.flightDetails)
                            {
                                writer.WriteStartObject();
                                #region airline
                                writer.WritePropertyName("airline");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.airline.code);
                                writer.WritePropertyName("name");
                                if (airlineDic.ContainsKey(d.airline.code))
                                {
                                    writer.WriteValue(airlineDic[d.airline.code]);
                                }
                                else
                                {
                                    string name = d.airline.name;
                                    writer.WriteValue(name);
                                    airlineDic.Add(d.airline.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region arrCity
                                writer.WritePropertyName("arrCity");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.arrCity.code);
                                writer.WritePropertyName("terminal");
                                writer.WriteValue(d.arrCity.terminal);
                                writer.WritePropertyName("name");
                                if (airportDic.ContainsKey(d.arrCity.code))
                                {
                                    writer.WriteValue(airportDic[d.arrCity.code]);
                                }
                                else
                                {
                                    string name = d.arrCity.name;
                                    writer.WriteValue(name);
                                    airportDic.Add(d.arrCity.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region arrDisplayDateTime
                                writer.WritePropertyName("arrDisplayDateTime");
                                writer.WriteStartObject();
                                writer.WritePropertyName("date");
                                writer.WriteValue(d.arrDisplayDateTime.date);
                                writer.WritePropertyName("displayTime");
                                writer.WriteValue(d.arrDisplayDateTime.displayTime);
                                writer.WritePropertyName("longDate");
                                writer.WriteValue(d.arrDisplayDateTime.longDate);
                                writer.WritePropertyName("longDateWithoutDay");
                                writer.WriteValue(d.arrDisplayDateTime.longDateWithoutDay);
                                writer.WritePropertyName("month");
                                writer.WriteValue(d.arrDisplayDateTime.month);
                                writer.WritePropertyName("shortDate");
                                writer.WriteValue(d.arrDisplayDateTime.shortDate);
                                writer.WritePropertyName("shortDateWithoutDay");
                                writer.WriteValue(d.arrDisplayDateTime.shortDateWithoutDay);
                                writer.WritePropertyName("time");
                                writer.WriteValue(d.arrDisplayDateTime.time);
                                writer.WritePropertyName("year");
                                writer.WriteValue(d.arrDisplayDateTime.year);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("arrivalDateTime");
                                writer.WriteValue(d.arrivalDateTime);
                                writer.WritePropertyName("availableSeat");
                                writer.WriteValue(d.availableSeat);
                                writer.WritePropertyName("cabin");
                                writer.WriteValue(d.cabin);
                                writer.WritePropertyName("connectingTime");
                                writer.WriteValue(d.connectingTime);

                                #region depCity
                                writer.WritePropertyName("depCity");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.depCity.code);
                                writer.WritePropertyName("terminal");
                                writer.WriteValue(d.depCity.terminal);
                                writer.WritePropertyName("name");
                                if (airportDic.ContainsKey(d.depCity.code))
                                {
                                    writer.WriteValue(airportDic[d.depCity.code]);
                                }
                                else
                                {
                                    string name = d.depCity.name;
                                    writer.WriteValue(name);
                                    airportDic.Add(d.depCity.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region depDisplayDateTime
                                writer.WritePropertyName("depDisplayDateTime");
                                writer.WriteStartObject();
                                writer.WritePropertyName("date");
                                writer.WriteValue(d.depDisplayDateTime.date);
                                writer.WritePropertyName("displayTime");
                                writer.WriteValue(d.depDisplayDateTime.displayTime);
                                writer.WritePropertyName("longDate");
                                writer.WriteValue(d.depDisplayDateTime.longDate);
                                writer.WritePropertyName("longDateWithoutDay");
                                writer.WriteValue(d.depDisplayDateTime.longDateWithoutDay);
                                writer.WritePropertyName("month");
                                writer.WriteValue(d.depDisplayDateTime.month);
                                writer.WritePropertyName("shortDate");
                                writer.WriteValue(d.depDisplayDateTime.shortDate);
                                writer.WritePropertyName("shortDateWithoutDay");
                                writer.WriteValue(d.depDisplayDateTime.shortDateWithoutDay);
                                writer.WritePropertyName("time");
                                writer.WriteValue(d.depDisplayDateTime.time);
                                writer.WritePropertyName("year");
                                writer.WriteValue(d.depDisplayDateTime.year);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("departureDateTime");
                                writer.WriteValue(d.departureDateTime);
                                writer.WritePropertyName("equipmentType");
                                writer.WriteValue(d.equipmentType);
                                writer.WritePropertyName("equipmentTypeName");
                                writer.WriteValue(d.equipmentTypeName);
                                writer.WritePropertyName("fareBasis");
                                writer.WriteValue(d.fareBasis);
                                writer.WritePropertyName("fareType");
                                writer.WriteValue(d.fareType);
                                writer.WritePropertyName("flightNumber");
                                writer.WriteValue(d.flightNumber);
                                writer.WritePropertyName("flightTime");
                                writer.WriteValue(d.flightTime);

                                #region operatedAirline
                                writer.WritePropertyName("operatedAirline");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.operatedAirline.code);
                                writer.WritePropertyName("name");
                                if (airlineDic.ContainsKey(d.operatedAirline.code))
                                {
                                    writer.WriteValue(airlineDic[d.operatedAirline.code]);
                                }
                                else
                                {
                                    string name = d.operatedAirline.name;
                                    writer.WriteValue(name);
                                    airlineDic.Add(d.operatedAirline.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region inFlightServices
                                writer.WritePropertyName("inFlightServices");
                                writer.WriteStartObject();
                                writer.WritePropertyName("dutyFreeSale");
                                writer.WriteValue(d.inFlightServices.dutyFreeSale);
                                writer.WritePropertyName("inflightEntertain");
                                writer.WriteValue(d.inFlightServices.inflightEntertain);
                                writer.WritePropertyName("inSeatPower");
                                writer.WriteValue(d.inFlightServices.inSeatPower);
                                writer.WritePropertyName("wifiInternet");
                                writer.WriteValue(d.inFlightServices.wifiInternet);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("rbd");
                                writer.WriteValue(d.rbd);

                                writer.WriteEndObject();
                            }
                            writer.WriteEndArray();
                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                        #endregion
                    }


                    #region Flight_SegRef3
                    writer.WritePropertyName("Flight_SegRef3");
                    writer.WriteStartArray();
                    if (flight.Flight_SegRef3 != null)
                    {
                        foreach (Flight f in flight.Flight_SegRef3)
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName("bookingCode");
                            writer.WriteValue(f.bookingCode);
                            writer.WritePropertyName("totalTime");
                            writer.WriteValue(f.totalTime);

                            writer.WritePropertyName("flightDetails");
                            writer.WriteStartArray();
                            foreach (FlightDetail d in f.flightDetails)
                            {
                                writer.WriteStartObject();
                                #region airline
                                writer.WritePropertyName("airline");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.airline.code);
                                writer.WritePropertyName("name");
                                if (airlineDic.ContainsKey(d.airline.code))
                                {
                                    writer.WriteValue(airlineDic[d.airline.code]);
                                }
                                else
                                {
                                    string name = d.airline.name;
                                    writer.WriteValue(name);
                                    airlineDic.Add(d.airline.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region arrCity
                                writer.WritePropertyName("arrCity");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.arrCity.code);
                                writer.WritePropertyName("terminal");
                                writer.WriteValue(d.arrCity.terminal);
                                writer.WritePropertyName("name");
                                if (airportDic.ContainsKey(d.arrCity.code))
                                {
                                    writer.WriteValue(airportDic[d.arrCity.code]);
                                }
                                else
                                {
                                    string name = d.arrCity.name;
                                    writer.WriteValue(name);
                                    airportDic.Add(d.arrCity.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region arrDisplayDateTime
                                writer.WritePropertyName("arrDisplayDateTime");
                                writer.WriteStartObject();
                                writer.WritePropertyName("date");
                                writer.WriteValue(d.arrDisplayDateTime.date);
                                writer.WritePropertyName("displayTime");
                                writer.WriteValue(d.arrDisplayDateTime.displayTime);
                                writer.WritePropertyName("longDate");
                                writer.WriteValue(d.arrDisplayDateTime.longDate);
                                writer.WritePropertyName("longDateWithoutDay");
                                writer.WriteValue(d.arrDisplayDateTime.longDateWithoutDay);
                                writer.WritePropertyName("month");
                                writer.WriteValue(d.arrDisplayDateTime.month);
                                writer.WritePropertyName("shortDate");
                                writer.WriteValue(d.arrDisplayDateTime.shortDate);
                                writer.WritePropertyName("shortDateWithoutDay");
                                writer.WriteValue(d.arrDisplayDateTime.shortDateWithoutDay);
                                writer.WritePropertyName("time");
                                writer.WriteValue(d.arrDisplayDateTime.time);
                                writer.WritePropertyName("year");
                                writer.WriteValue(d.arrDisplayDateTime.year);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("arrivalDateTime");
                                writer.WriteValue(d.arrivalDateTime);
                                writer.WritePropertyName("availableSeat");
                                writer.WriteValue(d.availableSeat);
                                writer.WritePropertyName("cabin");
                                writer.WriteValue(d.cabin);
                                writer.WritePropertyName("connectingTime");
                                writer.WriteValue(d.connectingTime);

                                #region depCity
                                writer.WritePropertyName("depCity");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.depCity.code);
                                writer.WritePropertyName("terminal");
                                writer.WriteValue(d.depCity.terminal);
                                writer.WritePropertyName("name");
                                if (airportDic.ContainsKey(d.depCity.code))
                                {
                                    writer.WriteValue(airportDic[d.depCity.code]);
                                }
                                else
                                {
                                    string name = d.depCity.name;
                                    writer.WriteValue(name);
                                    airportDic.Add(d.depCity.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region depDisplayDateTime
                                writer.WritePropertyName("depDisplayDateTime");
                                writer.WriteStartObject();
                                writer.WritePropertyName("date");
                                writer.WriteValue(d.depDisplayDateTime.date);
                                writer.WritePropertyName("displayTime");
                                writer.WriteValue(d.depDisplayDateTime.displayTime);
                                writer.WritePropertyName("longDate");
                                writer.WriteValue(d.depDisplayDateTime.longDate);
                                writer.WritePropertyName("longDateWithoutDay");
                                writer.WriteValue(d.depDisplayDateTime.longDateWithoutDay);
                                writer.WritePropertyName("month");
                                writer.WriteValue(d.depDisplayDateTime.month);
                                writer.WritePropertyName("shortDate");
                                writer.WriteValue(d.depDisplayDateTime.shortDate);
                                writer.WritePropertyName("shortDateWithoutDay");
                                writer.WriteValue(d.depDisplayDateTime.shortDateWithoutDay);
                                writer.WritePropertyName("time");
                                writer.WriteValue(d.depDisplayDateTime.time);
                                writer.WritePropertyName("year");
                                writer.WriteValue(d.depDisplayDateTime.year);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("departureDateTime");
                                writer.WriteValue(d.departureDateTime);
                                writer.WritePropertyName("equipmentType");
                                writer.WriteValue(d.equipmentType);
                                writer.WritePropertyName("equipmentTypeName");
                                writer.WriteValue(d.equipmentTypeName);
                                writer.WritePropertyName("fareBasis");
                                writer.WriteValue(d.fareBasis);
                                writer.WritePropertyName("fareType");
                                writer.WriteValue(d.fareType);
                                writer.WritePropertyName("flightNumber");
                                writer.WriteValue(d.flightNumber);
                                writer.WritePropertyName("flightTime");
                                writer.WriteValue(d.flightTime);

                                #region operatedAirline
                                writer.WritePropertyName("operatedAirline");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.operatedAirline.code);
                                writer.WritePropertyName("name");
                                if (airlineDic.ContainsKey(d.operatedAirline.code))
                                {
                                    writer.WriteValue(airlineDic[d.operatedAirline.code]);
                                }
                                else
                                {
                                    string name = d.operatedAirline.name;
                                    writer.WriteValue(name);
                                    airlineDic.Add(d.operatedAirline.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region inFlightServices
                                writer.WritePropertyName("inFlightServices");
                                writer.WriteStartObject();
                                writer.WritePropertyName("dutyFreeSale");
                                writer.WriteValue(d.inFlightServices.dutyFreeSale);
                                writer.WritePropertyName("inflightEntertain");
                                writer.WriteValue(d.inFlightServices.inflightEntertain);
                                writer.WritePropertyName("inSeatPower");
                                writer.WriteValue(d.inFlightServices.inSeatPower);
                                writer.WritePropertyName("wifiInternet");
                                writer.WriteValue(d.inFlightServices.wifiInternet);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("rbd");
                                writer.WriteValue(d.rbd);

                                writer.WriteEndObject();
                            }
                            writer.WriteEndArray();
                            writer.WriteEndObject();
                        }
                    }
                    writer.WriteEndArray();
                    #endregion



                    writer.WriteEndObject();
                    //flight
                }
                writer.WriteEndArray();
                // ] flights
                writer.WriteEndObject();
                // }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return sw.ToString();
        }
        public string toJsonBundle()
        {
            StringWriter sw = new StringWriter();
            try
            {
                JsonTextWriter writer = new JsonTextWriter(sw);
                Dictionary<string, string> airlineDic = new Dictionary<string, string>();
                Dictionary<string, string> airportDic = new Dictionary<string, string>();
                // {
                writer.WriteStartObject();

                // "pgSearchOID" : pgSearchOID
                writer.WritePropertyName("pgSearchOID");
                writer.WriteValue(pgSearchOID);

                // "filter": 
                writer.WritePropertyName("filter");
                // {
                writer.WriteStartObject();
                writer.WritePropertyName("maxDepDuration");
                writer.WriteValue(filter.maxDepDuration);
                writer.WritePropertyName("maxPrice");
                writer.WriteValue(filter.maxPrice);
                writer.WritePropertyName("maxRetDuration");
                writer.WriteValue(filter.maxRetDuration);
                writer.WritePropertyName("maxStop");
                writer.WriteValue(filter.maxStop);

                //airlines[]
                writer.WritePropertyName("airlines");
                writer.WriteStartArray();
                foreach (Airline airline in filter.airlines)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(airline.code);
                    writer.WritePropertyName("name");
                    if (airlineDic.ContainsKey(airline.code))
                    {
                        writer.WriteValue(airlineDic[airline.code]);
                    }
                    else
                    {
                        string name = airline.name;
                        writer.WriteValue(name);
                        airlineDic.Add(airline.code, name);
                    }
                    writer.WritePropertyName("minPrice");
                    writer.WriteValue(flights.FirstOrDefault(x => x.mainAirline.code == airline.code).addOnPrice);
                    writer.WritePropertyName("results");
                    writer.WriteValue(flights.Where(x => x.mainAirline.code == airline.code).ToList().Count);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();


                writer.WritePropertyName("departureAirport");
                writer.WriteStartArray();
                foreach (Airport airport in filter.departureAirport)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(airport.code);
                    writer.WritePropertyName("name");
                    if (airportDic.ContainsKey(airport.code))
                    {
                        writer.WriteValue(airlineDic[airport.code]);
                    }
                    else
                    {
                        string name = airport.name;
                        writer.WriteValue(name);
                        airportDic.Add(airport.code, name);
                    }
                    writer.WritePropertyName("results");
                    writer.WriteValue(flights.Where(x => x.departureFlight[0].flightDetails[0].depCity.code == airport.code).ToList().Count);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

                writer.WritePropertyName("arrivalAirport");
                writer.WriteStartArray();
                foreach (Airport airport in filter.arrivalAirport)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(airport.code);
                    writer.WritePropertyName("name");
                    if (airportDic.ContainsKey(airport.code))
                    {
                        writer.WriteValue(airlineDic[airport.code]);
                    }
                    else
                    {
                        string name = airport.name;
                        writer.WriteValue(name);
                        airportDic.Add(airport.code, name);
                    }
                    writer.WritePropertyName("results");
                    writer.WriteValue(flights.Where(x => x.departureFlight[0].flightDetails[x.departureFlight[0].flightDetails.Count - 1].arrCity.code == airport.code).ToList().Count);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
                // } filter

                //-------------------------
                // "flights": 
                writer.WritePropertyName("flights");
                // [
                writer.WriteStartArray();
                foreach (var flight in flights)
                {
                    //flight{
                    DateTime sDT = DateTime.Now;
                    writer.WriteStartObject();
                    writer.WritePropertyName("id");
                    writer.WriteValue(flight.id);
                    writer.WritePropertyName("type");
                    writer.WriteValue(flight.type);
                    writer.WritePropertyName("maxDepTime");
                    writer.WriteValue(flight.maxDepTime);
                    writer.WritePropertyName("maxRetTime");
                    writer.WriteValue(flight.maxRetTime);

                    //for sort
                    writer.WritePropertyName("price");
                    writer.WriteValue(flight.addOnPrice);
                    int shortDuration = flight.departureFlight.Min(x => Convert.ToInt32(x.totalTime));
                    if (flight.returnFlight != null)
                    {
                        int returnShortDuration = flight.returnFlight.Min(x => Convert.ToInt32(x.totalTime));
                        if (returnShortDuration < shortDuration)
                        {
                            shortDuration = returnShortDuration;
                        }
                    }
                    writer.WritePropertyName("shortDuration");
                    writer.WriteValue(shortDuration);

                    int longDuration = flight.departureFlight.Max(x => Convert.ToInt32(x.totalTime));
                    if (flight.returnFlight != null)
                    {
                        int returnlongDuration = flight.returnFlight.Max(x => Convert.ToInt32(x.totalTime));
                        if (returnlongDuration > longDuration)
                        {
                            longDuration = returnlongDuration;
                        }
                    }
                    writer.WritePropertyName("longDuration");
                    writer.WriteValue(longDuration);
                    writer.WritePropertyName("depCity");
                    writer.WriteValue(flight.departureFlight[0].flightDetails[0].depCity.code);
                    writer.WritePropertyName("arrCity");
                    writer.WriteValue(flight.departureFlight[0].flightDetails[flight.departureFlight[0].flightDetails.Count - 1].arrCity.code);

                    //fareRules
                    writer.WritePropertyName("fareRules");
                    writer.WriteStartArray();
                    foreach (var fareRule in flight.fareRule)
                    {
                        writer.WriteValue(fareRule);
                    }
                    writer.WriteEndArray();

                    #region fare
                    if (flight.fare != null)
                    {
                        //fare {
                        writer.WritePropertyName("fare");
                        writer.WriteStartObject();
                        writer.WritePropertyName("currencyCode");
                        writer.WriteValue(flight.fare.currencyCode);
                        writer.WritePropertyName("total");
                        writer.WriteValue(flight.fare.total);

                        //adt
                        writer.WritePropertyName("adtFare");
                        writer.WriteStartObject();
                        writer.WritePropertyName("fare");
                        writer.WriteValue(flight.fare.adtFare.fare);
                        //writer.WritePropertyName("fareBeforeMarkup");
                        //writer.WriteValue(flight.fare.adtFare.fareBeforeMarkup);
                        writer.WritePropertyName("net");
                        writer.WriteValue(flight.fare.adtFare.net);
                        writer.WritePropertyName("tax");
                        writer.WriteValue(flight.fare.adtFare.tax);
                        writer.WriteEndObject();
                        //adt

                        //chd
                        if (flight.fare.chdFare != null)
                        {
                            writer.WritePropertyName("chdFare");
                            writer.WriteStartObject();
                            writer.WritePropertyName("fare");
                            writer.WriteValue(flight.fare.chdFare.fare);
                            writer.WritePropertyName("net");
                            writer.WriteValue(flight.fare.chdFare.net);
                            writer.WritePropertyName("tax");
                            writer.WriteValue(flight.fare.chdFare.tax);
                            writer.WriteEndObject();
                        }
                        //chd

                        //inf
                        if (flight.fare.infFare != null)
                        {
                            writer.WritePropertyName("infFare");
                            writer.WriteStartObject();
                            writer.WritePropertyName("fare");
                            writer.WriteValue(flight.fare.infFare.fare);
                            writer.WritePropertyName("net");
                            writer.WriteValue(flight.fare.infFare.net);
                            writer.WritePropertyName("tax");
                            writer.WriteValue(flight.fare.infFare.tax);
                            writer.WriteEndObject();
                        }
                        //inf


                        writer.WriteEndObject();
                        //}fare
                    }
                    else
                    {
                        writer.WritePropertyName("addOnPrice");
                        writer.WriteValue(flight.addOnPrice);
                    }
                    #endregion

                    #region mainairline
                    writer.WritePropertyName("mainAirline");
                    writer.WriteStartObject();
                    writer.WritePropertyName("code");
                    writer.WriteValue(flight.mainAirline.code);
                    writer.WritePropertyName("name");
                    if (airlineDic.ContainsKey(flight.mainAirline.code))
                    {
                        writer.WriteValue(airlineDic[flight.mainAirline.code]);
                    }
                    else
                    {
                        string name = flight.mainAirline.name;
                        writer.WriteValue(name);
                        airlineDic.Add(flight.mainAirline.code, name);
                    }
                    writer.WriteEndObject();
                    #endregion

                    #region DepFlight
                    writer.WritePropertyName("departureFlight");
                    writer.WriteStartArray();
                    foreach (Flight f in flight.departureFlight)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("bookingCode");
                        writer.WriteValue(f.bookingCode);
                        writer.WritePropertyName("totalTime");
                        writer.WriteValue(f.totalTime);

                        writer.WritePropertyName("flightDetails");
                        writer.WriteStartArray();
                        foreach (FlightDetail d in f.flightDetails)
                        {
                            writer.WriteStartObject();
                            #region airline
                            writer.WritePropertyName("airline");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.airline.code);
                            writer.WritePropertyName("name");
                            if (airlineDic.ContainsKey(d.airline.code))
                            {
                                writer.WriteValue(airlineDic[d.airline.code]);
                            }
                            else
                            {
                                string name = d.airline.name;
                                writer.WriteValue(name);
                                airlineDic.Add(d.airline.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region arrCity
                            writer.WritePropertyName("arrCity");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.arrCity.code);
                            writer.WritePropertyName("terminal");
                            writer.WriteValue(d.arrCity.terminal);
                            writer.WritePropertyName("name");
                            if (airportDic.ContainsKey(d.arrCity.code))
                            {
                                writer.WriteValue(airportDic[d.arrCity.code]);
                            }
                            else
                            {
                                string name = d.arrCity.name;
                                writer.WriteValue(name);
                                airportDic.Add(d.arrCity.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region arrDisplayDateTime
                            writer.WritePropertyName("arrDisplayDateTime");
                            writer.WriteStartObject();
                            writer.WritePropertyName("date");
                            writer.WriteValue(d.arrDisplayDateTime.date);
                            writer.WritePropertyName("displayTime");
                            writer.WriteValue(d.arrDisplayDateTime.displayTime);
                            writer.WritePropertyName("longDate");
                            writer.WriteValue(d.arrDisplayDateTime.longDate);
                            writer.WritePropertyName("longDateWithoutDay");
                            writer.WriteValue(d.arrDisplayDateTime.longDateWithoutDay);
                            writer.WritePropertyName("month");
                            writer.WriteValue(d.arrDisplayDateTime.month);
                            writer.WritePropertyName("shortDate");
                            writer.WriteValue(d.arrDisplayDateTime.shortDate);
                            writer.WritePropertyName("shortDateWithoutDay");
                            writer.WriteValue(d.arrDisplayDateTime.shortDateWithoutDay);
                            writer.WritePropertyName("time");
                            writer.WriteValue(d.arrDisplayDateTime.time);
                            writer.WritePropertyName("year");
                            writer.WriteValue(d.arrDisplayDateTime.year);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("arrivalDateTime");
                            writer.WriteValue(d.arrivalDateTime);
                            writer.WritePropertyName("availableSeat");
                            writer.WriteValue(d.availableSeat);
                            writer.WritePropertyName("cabin");
                            writer.WriteValue(d.cabin);
                            writer.WritePropertyName("connectingTime");
                            writer.WriteValue(d.connectingTime);

                            #region depCity
                            writer.WritePropertyName("depCity");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.depCity.code);
                            writer.WritePropertyName("terminal");
                            writer.WriteValue(d.depCity.terminal);
                            writer.WritePropertyName("name");
                            if (airportDic.ContainsKey(d.depCity.code))
                            {
                                writer.WriteValue(airportDic[d.depCity.code]);
                            }
                            else
                            {
                                string name = d.depCity.name;
                                writer.WriteValue(name);
                                airportDic.Add(d.depCity.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion

                            #region depDisplayDateTime
                            writer.WritePropertyName("depDisplayDateTime");
                            writer.WriteStartObject();
                            writer.WritePropertyName("date");
                            writer.WriteValue(d.depDisplayDateTime.date);
                            writer.WritePropertyName("displayTime");
                            writer.WriteValue(d.depDisplayDateTime.displayTime);
                            writer.WritePropertyName("longDate");
                            writer.WriteValue(d.depDisplayDateTime.longDate);
                            writer.WritePropertyName("longDateWithoutDay");
                            writer.WriteValue(d.depDisplayDateTime.longDateWithoutDay);
                            writer.WritePropertyName("month");
                            writer.WriteValue(d.depDisplayDateTime.month);
                            writer.WritePropertyName("shortDate");
                            writer.WriteValue(d.depDisplayDateTime.shortDate);
                            writer.WritePropertyName("shortDateWithoutDay");
                            writer.WriteValue(d.depDisplayDateTime.shortDateWithoutDay);
                            writer.WritePropertyName("time");
                            writer.WriteValue(d.depDisplayDateTime.time);
                            writer.WritePropertyName("year");
                            writer.WriteValue(d.depDisplayDateTime.year);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("departureDateTime");
                            writer.WriteValue(d.departureDateTime);
                            writer.WritePropertyName("equipmentType");
                            writer.WriteValue(d.equipmentType);
                            writer.WritePropertyName("equipmentTypeName");
                            writer.WriteValue(d.equipmentTypeName);
                            writer.WritePropertyName("fareBasis");
                            writer.WriteValue(d.fareBasis);
                            writer.WritePropertyName("fareType");
                            writer.WriteValue(d.fareType);
                            writer.WritePropertyName("flightNumber");
                            writer.WriteValue(d.flightNumber);
                            writer.WritePropertyName("flightTime");
                            writer.WriteValue(d.flightTime);

                            #region operatedAirline
                            writer.WritePropertyName("operatedAirline");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.operatedAirline.code);
                            writer.WritePropertyName("name");
                            if (airlineDic.ContainsKey(d.operatedAirline.code))
                            {
                                writer.WriteValue(airlineDic[d.operatedAirline.code]);
                            }
                            else
                            {
                                string name = d.operatedAirline.name;
                                writer.WriteValue(name);
                                airlineDic.Add(d.operatedAirline.code, name);
                            }
                            writer.WriteEndObject();
                            #endregion


                            #region inFlightServices
                            writer.WritePropertyName("inFlightServices");
                            writer.WriteStartObject();
                            writer.WritePropertyName("dutyFreeSale");
                            writer.WriteValue(d.inFlightServices.dutyFreeSale);
                            writer.WritePropertyName("inflightEntertain");
                            writer.WriteValue(d.inFlightServices.inflightEntertain);
                            writer.WritePropertyName("inSeatPower");
                            writer.WriteValue(d.inFlightServices.inSeatPower);
                            writer.WritePropertyName("wifiInternet");
                            writer.WriteValue(d.inFlightServices.wifiInternet);
                            writer.WriteEndObject();
                            #endregion

                            writer.WritePropertyName("rbd");
                            writer.WriteValue(d.rbd);

                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    #endregion

                    if (flight.returnFlight != null)
                    {
                        #region returnFlight
                        writer.WritePropertyName("returnFlight");
                        writer.WriteStartArray();
                        foreach (Flight f in flight.returnFlight)
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName("bookingCode");
                            writer.WriteValue(f.bookingCode);
                            writer.WritePropertyName("totalTime");
                            writer.WriteValue(f.totalTime);

                            writer.WritePropertyName("flightDetails");
                            writer.WriteStartArray();
                            foreach (FlightDetail d in f.flightDetails)
                            {
                                writer.WriteStartObject();
                                #region airline
                                writer.WritePropertyName("airline");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.airline.code);
                                writer.WritePropertyName("name");
                                if (airlineDic.ContainsKey(d.airline.code))
                                {
                                    writer.WriteValue(airlineDic[d.airline.code]);
                                }
                                else
                                {
                                    string name = d.airline.name;
                                    writer.WriteValue(name);
                                    airlineDic.Add(d.airline.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region arrCity
                                writer.WritePropertyName("arrCity");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.arrCity.code);
                                writer.WritePropertyName("terminal");
                                writer.WriteValue(d.arrCity.terminal);
                                writer.WritePropertyName("name");
                                if (airportDic.ContainsKey(d.arrCity.code))
                                {
                                    writer.WriteValue(airportDic[d.arrCity.code]);
                                }
                                else
                                {
                                    string name = d.arrCity.name;
                                    writer.WriteValue(name);
                                    airportDic.Add(d.arrCity.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region arrDisplayDateTime
                                writer.WritePropertyName("arrDisplayDateTime");
                                writer.WriteStartObject();
                                writer.WritePropertyName("date");
                                writer.WriteValue(d.arrDisplayDateTime.date);
                                writer.WritePropertyName("displayTime");
                                writer.WriteValue(d.arrDisplayDateTime.displayTime);
                                writer.WritePropertyName("longDate");
                                writer.WriteValue(d.arrDisplayDateTime.longDate);
                                writer.WritePropertyName("longDateWithoutDay");
                                writer.WriteValue(d.arrDisplayDateTime.longDateWithoutDay);
                                writer.WritePropertyName("month");
                                writer.WriteValue(d.arrDisplayDateTime.month);
                                writer.WritePropertyName("shortDate");
                                writer.WriteValue(d.arrDisplayDateTime.shortDate);
                                writer.WritePropertyName("shortDateWithoutDay");
                                writer.WriteValue(d.arrDisplayDateTime.shortDateWithoutDay);
                                writer.WritePropertyName("time");
                                writer.WriteValue(d.arrDisplayDateTime.time);
                                writer.WritePropertyName("year");
                                writer.WriteValue(d.arrDisplayDateTime.year);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("arrivalDateTime");
                                writer.WriteValue(d.arrivalDateTime);
                                writer.WritePropertyName("availableSeat");
                                writer.WriteValue(d.availableSeat);
                                writer.WritePropertyName("cabin");
                                writer.WriteValue(d.cabin);
                                writer.WritePropertyName("connectingTime");
                                writer.WriteValue(d.connectingTime);

                                #region depCity
                                writer.WritePropertyName("depCity");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.depCity.code);
                                writer.WritePropertyName("terminal");
                                writer.WriteValue(d.depCity.terminal);
                                writer.WritePropertyName("name");
                                if (airportDic.ContainsKey(d.depCity.code))
                                {
                                    writer.WriteValue(airportDic[d.depCity.code]);
                                }
                                else
                                {
                                    string name = d.depCity.name;
                                    writer.WriteValue(name);
                                    airportDic.Add(d.depCity.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion

                                #region depDisplayDateTime
                                writer.WritePropertyName("depDisplayDateTime");
                                writer.WriteStartObject();
                                writer.WritePropertyName("date");
                                writer.WriteValue(d.depDisplayDateTime.date);
                                writer.WritePropertyName("displayTime");
                                writer.WriteValue(d.depDisplayDateTime.displayTime);
                                writer.WritePropertyName("longDate");
                                writer.WriteValue(d.depDisplayDateTime.longDate);
                                writer.WritePropertyName("longDateWithoutDay");
                                writer.WriteValue(d.depDisplayDateTime.longDateWithoutDay);
                                writer.WritePropertyName("month");
                                writer.WriteValue(d.depDisplayDateTime.month);
                                writer.WritePropertyName("shortDate");
                                writer.WriteValue(d.depDisplayDateTime.shortDate);
                                writer.WritePropertyName("shortDateWithoutDay");
                                writer.WriteValue(d.depDisplayDateTime.shortDateWithoutDay);
                                writer.WritePropertyName("time");
                                writer.WriteValue(d.depDisplayDateTime.time);
                                writer.WritePropertyName("year");
                                writer.WriteValue(d.depDisplayDateTime.year);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("departureDateTime");
                                writer.WriteValue(d.departureDateTime);
                                writer.WritePropertyName("equipmentType");
                                writer.WriteValue(d.equipmentType);
                                writer.WritePropertyName("equipmentTypeName");
                                writer.WriteValue(d.equipmentTypeName);
                                writer.WritePropertyName("fareBasis");
                                writer.WriteValue(d.fareBasis);
                                writer.WritePropertyName("fareType");
                                writer.WriteValue(d.fareType);
                                writer.WritePropertyName("flightNumber");
                                writer.WriteValue(d.flightNumber);
                                writer.WritePropertyName("flightTime");
                                writer.WriteValue(d.flightTime);

                                #region operatedAirline
                                writer.WritePropertyName("operatedAirline");
                                writer.WriteStartObject();
                                writer.WritePropertyName("code");
                                writer.WriteValue(d.operatedAirline.code);
                                writer.WritePropertyName("name");
                                if (airlineDic.ContainsKey(d.operatedAirline.code))
                                {
                                    writer.WriteValue(airlineDic[d.operatedAirline.code]);
                                }
                                else
                                {
                                    string name = d.operatedAirline.name;
                                    writer.WriteValue(name);
                                    airlineDic.Add(d.operatedAirline.code, name);
                                }
                                writer.WriteEndObject();
                                #endregion


                                #region inFlightServices
                                writer.WritePropertyName("inFlightServices");
                                writer.WriteStartObject();
                                writer.WritePropertyName("dutyFreeSale");
                                writer.WriteValue(d.inFlightServices.dutyFreeSale);
                                writer.WritePropertyName("inflightEntertain");
                                writer.WriteValue(d.inFlightServices.inflightEntertain);
                                writer.WritePropertyName("inSeatPower");
                                writer.WriteValue(d.inFlightServices.inSeatPower);
                                writer.WritePropertyName("wifiInternet");
                                writer.WriteValue(d.inFlightServices.wifiInternet);
                                writer.WriteEndObject();
                                #endregion

                                writer.WritePropertyName("rbd");
                                writer.WriteValue(d.rbd);

                                writer.WriteEndObject();
                            }
                            writer.WriteEndArray();
                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                        #endregion
                    }


                    writer.WriteEndObject();
                    //flight
                }
                writer.WriteEndArray();
                // ] flights
                writer.WriteEndObject();
                // }
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
            return sw.ToString();
        }
    }

    public class FlightFilter
    {
        public List<Airline> airlines { get; set; }
        public List<Airport> departureAirport { get; set; }
        public List<Airport> arrivalAirport { get; set; }
        public decimal maxPrice { get; set; }
        public int maxDepDuration { get; set; }
        public int maxRetDuration { get; set; }
        public int maxStop { get; set; }
        public decimal minPrice { get; set; }
        public decimal minPriceDepart { get; set; }
        public decimal minPriceReturn { get; set; }
    }
}
