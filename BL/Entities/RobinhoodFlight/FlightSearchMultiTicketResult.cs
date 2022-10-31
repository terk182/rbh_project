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
    public class FlightSearchMultiTicketResult
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<MultiTicketFlight> flights { get; set; }
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
                        writer.WriteValue(airportDic[airport.code]);
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
                    writer.WriteValue(flight.maxSegRef1Time);
                    writer.WritePropertyName("maxRetTime");
                    writer.WriteValue(flight.maxSegRef2Time);

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
                        #region returnFlight
                        writer.WritePropertyName("Flight_SegRef2");//returnFlight
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
                    writer.WritePropertyName("recommendation_number");
                    writer.WriteValue(flight.recommendation_number);
                    writer.WritePropertyName("isMultiTicket");
                    writer.WriteValue(flight.isMultiTicket);
                    writer.WritePropertyName("type");
                    writer.WriteValue(flight.type);
                    writer.WritePropertyName("minSegRef1DepTime");
                    writer.WriteValue(flight.minSegRef1DepTime);
                    writer.WritePropertyName("minSegRef1Time");
                    writer.WriteValue(flight.minSegRef1Time);
                    writer.WritePropertyName("minSegRef2Time");
                    writer.WriteValue(flight.minSegRef2Time);

                    writer.WritePropertyName("maxSegRef1DepTime");
                    writer.WriteValue(flight.maxSegRef1DepTime);
                    writer.WritePropertyName("maxSegRef1Time");
                    writer.WriteValue(flight.maxSegRef1Time);
                    writer.WritePropertyName("maxSegRef2Time");
                    writer.WriteValue(flight.maxSegRef2Time);

                    //for sort
                    writer.WritePropertyName("price");
                    writer.WriteValue(flight.fare.adtFare.net);
                    int shortDuration = flight.Flight_SegRef1!=null?flight.Flight_SegRef1.Min(x => Convert.ToInt32(x.totalTime)): flight.Flight_SegRef2.Min(x => Convert.ToInt32(x.totalTime));
                    if (flight.Flight_SegRef2 != null)
                    {
                        int returnShortDuration = flight.Flight_SegRef2.Min(x => Convert.ToInt32(x.totalTime));
                        if (returnShortDuration < shortDuration)
                        {
                            shortDuration = returnShortDuration;
                        }
                    }
                    writer.WritePropertyName("shortDuration");
                    writer.WriteValue(shortDuration);

                    int longDuration = flight.Flight_SegRef1 != null ? flight.Flight_SegRef1.Max(x => Convert.ToInt32(x.totalTime)): flight.Flight_SegRef2.Max(x => Convert.ToInt32(x.totalTime));
                    if (flight.Flight_SegRef2 != null)
                    {
                        int returnlongDuration = flight.Flight_SegRef2.Max(x => Convert.ToInt32(x.totalTime));
                        if (returnlongDuration > longDuration)
                        {
                            longDuration = returnlongDuration;
                        }
                    }
                    writer.WritePropertyName("longDuration");
                    writer.WriteValue(longDuration);
                    writer.WritePropertyName("depCity");
                    writer.WriteValue(flight.Flight_SegRef1 != null ? flight.Flight_SegRef1[0].flightDetails[0].depCity.code: flight.Flight_SegRef2[0].flightDetails[0].depCity.code);
                    writer.WritePropertyName("arrCity");
                    if (flight.Flight_SegRef1 != null)
                    {
                        writer.WriteValue(flight.Flight_SegRef1[0].flightDetails[flight.Flight_SegRef1[0].flightDetails.Count - 1].arrCity.code);
                    }
                    else
                    {
                        writer.WriteValue(flight.Flight_SegRef2[0].flightDetails[flight.Flight_SegRef2[0].flightDetails.Count - 1].arrCity.code);
                    }

                    //fareRules
                    writer.WritePropertyName("fareRules");
                    if (flight.fareRule != null)
                    {
                        writer.WriteStartArray();
                        foreach (var fareRule in flight.fareRule)
                        {
                            writer.WriteValue(fareRule);
                        }
                        writer.WriteEndArray();
                    }
                    else
                    {
                        writer.WriteValue(flight.fareRule);
                    }
                    //fare
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
                        writer.WritePropertyName("strikethroughTotalPrice");
                        writer.WriteValue(flight.fare.strikethroughTotalPrice);

                        //adt
                        writer.WritePropertyName("adtFare");
                        writer.WriteStartObject();
                        writer.WritePropertyName("fare");
                        writer.WriteValue(flight.fare.adtFare.fare);
                        writer.WritePropertyName("fareBeforeMarkup");
                        writer.WriteValue(flight.fare.adtFare.fareBeforeMarkup);
                        writer.WritePropertyName("lessFare");
                        writer.WriteValue(flight.fare.adtFare.lessFare);
                        writer.WritePropertyName("net");
                        writer.WriteValue(flight.fare.adtFare.net);
                        writer.WritePropertyName("tax");
                        writer.WriteValue(flight.fare.adtFare.tax);
                        writer.WritePropertyName("strikethroughPrice");
                        writer.WriteValue(flight.fare.adtFare.strikethroughPrice);
                        writer.WriteEndObject();
                        //adt

                        //chd
                        if (flight.fare.chdFare != null)
                        {
                            writer.WritePropertyName("chdFare");
                            writer.WriteStartObject();
                            writer.WritePropertyName("fare");
                            writer.WriteValue(flight.fare.chdFare.fare);
                            writer.WritePropertyName("fareBeforeMarkup");
                            writer.WriteValue(flight.fare.chdFare.fareBeforeMarkup);
                            writer.WritePropertyName("lessFare");
                            writer.WriteValue(flight.fare.chdFare.lessFare);
                            writer.WritePropertyName("net");
                            writer.WriteValue(flight.fare.chdFare.net);
                            writer.WritePropertyName("tax");
                            writer.WriteValue(flight.fare.chdFare.tax);
                            writer.WritePropertyName("strikethroughPrice");
                            writer.WriteValue(flight.fare.chdFare.strikethroughPrice);
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
                            writer.WritePropertyName("fareBeforeMarkup");
                            writer.WriteValue(flight.fare.infFare.fareBeforeMarkup);
                            writer.WritePropertyName("lessFare");
                            writer.WriteValue(flight.fare.infFare.lessFare);
                            writer.WritePropertyName("net");
                            writer.WriteValue(flight.fare.infFare.net);
                            writer.WritePropertyName("tax");
                            writer.WriteValue(flight.fare.infFare.tax);
                            writer.WritePropertyName("strikethroughPrice");
                            writer.WriteValue(flight.fare.infFare.strikethroughPrice);
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
                    //discountTag 
                    #region discountTag
                    if (flight.discountTag != null)
                    {
                        writer.WritePropertyName("discountTag");
                        writer.WriteStartObject();
                        writer.WritePropertyName("discountAmount");
                        writer.WriteValue(flight.discountTag.discountAmount);
                        writer.WritePropertyName("discountIsPercent");
                        writer.WriteValue(flight.discountTag.discountIsPercent);
                        writer.WritePropertyName("promotionGroupCode");
                        writer.WriteValue(flight.discountTag.promotionGroupCode);
                        writer.WritePropertyName("promotionText");
                        writer.WriteValue(flight.discountTag.promotionText);
                        writer.WriteEndObject();
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

                    //upsell
                    if (flight.upsell != null)
                    {
                        writer.WritePropertyName("upsell");
                        writer.WriteStartArray();
                        foreach (var _upsell in flight.upsell)
                        {
                            writer.WriteValue(_upsell);
                        }
                        writer.WriteEndArray();
                    }
                    //upsell
                    if (flight.FamilyInformation != null)
                    {
                        writer.WritePropertyName("FamilyInformation");
                        writer.WriteStartArray();
                        foreach (var _family in flight.FamilyInformation)
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName("RefNumber");
                            writer.WriteValue(_family.RefNumber);
                            writer.WritePropertyName("FareFamilyname");
                            writer.WriteValue(_family.FareFamilyname);
                            writer.WritePropertyName("Description");
                            writer.WriteValue(_family.Description);
                            writer.WritePropertyName("Carrier");
                            writer.WriteValue(_family.Carrier);
                            writer.WritePropertyName("Services");
                            writer.WriteStartArray();
                            foreach (var _service in _family.Services)
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("Reference");
                                writer.WriteValue(_service.Reference);
                                writer.WritePropertyName("Status");
                                writer.WriteValue(_service.Status);
                                writer.WritePropertyName("ServiceGroup");
                                writer.WriteValue(_service.ServiceGroup);
                                if (_service.ServiceSubGroup != null)
                                {
                                    writer.WritePropertyName("ServiceSubGroup");
                                    writer.WriteValue(_service.ServiceSubGroup);
                                }
                                writer.WritePropertyName("CommercialName");
                                writer.WriteValue(_service.CommercialName);
                                writer.WriteEndObject();
                            }
                            
                            writer.WriteEndArray();
                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                    }

                    #region DepFlight
                    if (flight.Flight_SegRef1 != null && flight.Flight_SegRef1.Count > 0)
                    {
                        writer.WritePropertyName("Flight_SegRef1");
                        writer.WriteStartArray();

                        foreach (Flight f in flight.Flight_SegRef1)
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName("refNumber");
                            writer.WriteValue(f.refNumber);
                            writer.WritePropertyName("bookingCode");
                            writer.WriteValue(f.bookingCode);
                            writer.WritePropertyName("totalTime");
                            writer.WriteValue(f.totalTime);

                            if (f.baggageDetails != null) //&& f.baggageDetails.freeAllowance > 0
                            {
                                writer.WritePropertyName("baggageDetails");
                                writer.WriteStartObject();
                                writer.WritePropertyName("freeAllowance");
                                writer.WriteValue(f.baggageDetails.freeAllowance);
                                writer.WritePropertyName("quantityCode");
                                writer.WriteValue(f.baggageDetails.quantityCode);
                                writer.WritePropertyName("unitQualifier");
                                writer.WriteValue(f.baggageDetails.unitQualifier);
                                writer.WriteEndObject();
                            }

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
                                writer.WritePropertyName("Seq");
                                writer.WriteValue(d.Seq);

                                writer.WriteEndObject();
                            }
                            writer.WriteEndArray();
                            writer.WriteEndObject();
                        }
                        writer.WriteEndArray();
                    }
                    #endregion

                    if (flight.Flight_SegRef2 != null)
                    {
                        #region returnFlight
                        writer.WritePropertyName("Flight_SegRef2");//returnFlight
                        writer.WriteStartArray();
                        foreach (Flight f in flight.Flight_SegRef2)
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName("refNumber");
                            writer.WriteValue(f.refNumber);
                            writer.WritePropertyName("bookingCode");
                            writer.WriteValue(f.bookingCode);
                            writer.WritePropertyName("totalTime");
                            writer.WriteValue(f.totalTime);
                            if (f.baggageDetails != null) //&& f.baggageDetails.freeAllowance > 0
                            {
                                writer.WritePropertyName("baggageDetails");
                                writer.WriteStartObject();
                                writer.WritePropertyName("freeAllowance");
                                writer.WriteValue(f.baggageDetails.freeAllowance);
                                writer.WritePropertyName("quantityCode");
                                writer.WriteValue(f.baggageDetails.quantityCode);
                                writer.WritePropertyName("unitQualifier");
                                writer.WriteValue(f.baggageDetails.unitQualifier);
                                writer.WriteEndObject();
                            }

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
                                writer.WritePropertyName("Seq");
                                writer.WriteValue(d.Seq);

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
                writer.WritePropertyName("minPrice");
                writer.WriteValue(filter.minPrice);
                writer.WritePropertyName("minPriceDepart");
                writer.WriteValue(filter.minPriceDepart);
                writer.WritePropertyName("minPriceReturn");
                writer.WriteValue(filter.minPriceReturn);

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
                        writer.WriteValue(airportDic[airport.code]);
                    }
                    else
                    {
                        string name = airport.name;
                        writer.WriteValue(name);
                        airportDic.Add(airport.code, name);
                    }
                    writer.WritePropertyName("results");

                    writer.WriteValue(flights.Where(x => x.Flight_SegRef1 != null && x.Flight_SegRef1[0].flightDetails[0].depCity.code == airport.code).ToList().Count);
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
                        writer.WriteValue(airportDic[airport.code]);
                    }
                    else
                    {
                        string name = airport.name;
                        writer.WriteValue(name);
                        airportDic.Add(airport.code, name);
                    }
                    writer.WritePropertyName("results");
                    writer.WriteValue(flights.Where(x => x.Flight_SegRef1 != null && x.Flight_SegRef1[0].flightDetails[x.Flight_SegRef1[0].flightDetails.Count - 1].arrCity.code == airport.code).ToList().Count);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
                // } filter

                //-------------------------
                // "pgSearchOID" : pgSearchOID
                writer.WritePropertyName("pgSearchOID");
                writer.WriteValue(pgSearchOID);
                writer.WritePropertyName("isError");
                writer.WriteValue(isError);
                writer.WritePropertyName("errorMessage");
                writer.WriteValue(errorMessage);

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
                        writer.WriteValue(airportDic[airport.code]);
                    }
                    else
                    {
                        string name = airport.name;
                        writer.WriteValue(name);
                        airportDic.Add(airport.code, name);
                    }
                    writer.WritePropertyName("results");
                    writer.WriteValue(flights.Where(x => x.Flight_SegRef1[0].flightDetails[0].depCity.code == airport.code).ToList().Count);
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
                    writer.WriteValue(flights.Where(x => x.Flight_SegRef1[0].flightDetails[x.Flight_SegRef1[0].flightDetails.Count - 1].arrCity.code == airport.code).ToList().Count);
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
                    writer.WriteValue(flight.maxSegRef1Time);
                    writer.WritePropertyName("maxRetTime");
                    writer.WriteValue(flight.maxSegRef2Time);

                    //for sort
                    writer.WritePropertyName("price");
                    writer.WriteValue(flight.addOnPrice);
                    int shortDuration = flight.Flight_SegRef1.Min(x => Convert.ToInt32(x.totalTime));
                    if (flight.Flight_SegRef2 != null)
                    {
                        int returnShortDuration = flight.Flight_SegRef2.Min(x => Convert.ToInt32(x.totalTime));
                        if (returnShortDuration < shortDuration)
                        {
                            shortDuration = returnShortDuration;
                        }
                    }
                    writer.WritePropertyName("shortDuration");
                    writer.WriteValue(shortDuration);

                    int longDuration = flight.Flight_SegRef1.Max(x => Convert.ToInt32(x.totalTime));
                    if (flight.Flight_SegRef2 != null)
                    {
                        int returnlongDuration = flight.Flight_SegRef2.Max(x => Convert.ToInt32(x.totalTime));
                        if (returnlongDuration > longDuration)
                        {
                            longDuration = returnlongDuration;
                        }
                    }
                    writer.WritePropertyName("longDuration");
                    writer.WriteValue(longDuration);
                    writer.WritePropertyName("depCity");
                    writer.WriteValue(flight.Flight_SegRef1[0].flightDetails[0].depCity.code);
                    writer.WritePropertyName("arrCity");
                    writer.WriteValue(flight.Flight_SegRef1[0].flightDetails[flight.Flight_SegRef1[0].flightDetails.Count - 1].arrCity.code);

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
                    writer.WritePropertyName("Flight_SegRef1");//departureFlight
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
                        #region returnFlight
                        writer.WritePropertyName("Flight_SegRef2");//returnFlight
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
    }
 
}
