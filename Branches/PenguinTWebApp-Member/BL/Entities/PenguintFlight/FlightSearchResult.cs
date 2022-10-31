using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.GogojiiFlight
{
    public class FlightSearchResult
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<MasterFlight> flights { get; set; }
        public FlightFilter filter { get; set; }
        public Guid pgSearchOID { get; set; }

        public string toJson()
        {
            StringWriter sw = new StringWriter();
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
            foreach(Airline airline in filter.airlines)
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

            writer.WriteEndObject();
            // } filter
        
            //-------------------------
            // "flights": 
            writer.WritePropertyName("flights");
            // [
            writer.WriteStartArray();
            foreach(var flight in flights)
            {
                //flight{
                DateTime sDT = DateTime.Now;
                writer.WriteStartObject();
                writer.WritePropertyName("id");
                writer.WriteValue(flight.id);
                writer.WritePropertyName("maxDepTime");
                writer.WriteValue(flight.maxDepTime);
                writer.WritePropertyName("maxRetTime");
                writer.WriteValue(flight.maxRetTime);

                //fareRules
                writer.WritePropertyName("fareRules");
                writer.WriteStartArray();
                foreach(var fareRule in flight.fareRule)
                {
                    writer.WriteValue(fareRule);
                }
                writer.WriteEndArray();

                #region fare
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
                writer.WritePropertyName("fareBeforeMarkup");
                writer.WriteValue(flight.fare.adtFare.fareBeforeMarkup);
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
                foreach(Flight f in flight.departureFlight)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("bookingCode");
                    writer.WriteValue(f.bookingCode);
                    writer.WritePropertyName("totalTime");
                    writer.WriteValue(f.totalTime);

                    writer.WritePropertyName("flightDetails");
                    writer.WriteStartArray();
                    foreach(FlightDetail d in f.flightDetails)
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
                        writer.WritePropertyName("avail");
                        writer.WriteValue(d.avail);
                        writer.WritePropertyName("cabin");
                        writer.WriteValue(d.cabin);
                        writer.WritePropertyName("connectingTime");
                        writer.WriteValue(d.connectingTime);

                        #region depCity
                        writer.WritePropertyName("depCity");
                        writer.WriteStartObject();
                        writer.WritePropertyName("code");
                        writer.WriteValue(d.depCity.code);
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

                        writer.WritePropertyName("rbd");
                        writer.WriteValue(d.rbd);
                        writer.WritePropertyName("scaleConnection");
                        writer.WriteValue(d.scaleConnection);
                        writer.WritePropertyName("scaleFlight");
                        writer.WriteValue(d.scaleFlight);

                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                #endregion

                if (flight.returnFlight !=null)
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
                            writer.WritePropertyName("avail");
                            writer.WriteValue(d.avail);
                            writer.WritePropertyName("cabin");
                            writer.WriteValue(d.cabin);
                            writer.WritePropertyName("connectingTime");
                            writer.WriteValue(d.connectingTime);

                            #region depCity
                            writer.WritePropertyName("depCity");
                            writer.WriteStartObject();
                            writer.WritePropertyName("code");
                            writer.WriteValue(d.depCity.code);
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

                            writer.WritePropertyName("rbd");
                            writer.WriteValue(d.rbd);
                            writer.WritePropertyName("scaleConnection");
                            writer.WriteValue(d.scaleConnection);
                            writer.WritePropertyName("scaleFlight");
                            writer.WriteValue(d.scaleFlight);

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

            return sw.ToString();
        }
    }

    public class FlightFilter
    {
        public List<Airline> airlines { get; set; }
        public decimal maxPrice { get; set; }
        public int maxDepDuration { get; set; }
        public int maxRetDuration { get; set; }
        public int maxStop { get; set; }
    }
}
