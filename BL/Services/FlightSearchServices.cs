using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Entities.CitySearch;
using DataModel;
using DataModel.UnitOfWork;
using System.Configuration;
using BL.Utilities;
using Newtonsoft.Json;
using BL.Entities;
using BL.Entities.RobinhoodFlight;
using BL.Entities.MasterPricer;
using BL.Entities.AirSell;
using log4net;
using BL.Entities.InformativePricing;
using BL.Entities.RobinhoodFare;
using BL.Entities.RobinhoodPNR;
using BL.Entities.PNR;
using System.Transactions;
using BL.Entities.FareRuleConfig;

namespace BL
{
    public class FlightSearchServices : IFlightSearchServices
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UnitOfWork _unitOfWork;
        private decimal paymentFee;
        private bool isPaymentFeePct;
        public FlightSearchServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            var payment = _unitOfWork.PaymentRepository.GetFirstOrDefault(x => 1 == 1);
            paymentFee = payment.KbankFullValue.GetValueOrDefault();
            isPaymentFeePct = payment.IspercentKbankFull.GetValueOrDefault();
        }

        public void AddLog(FlightSearchLog log)
        {
            _unitOfWork.FlightSearchLogRepository.Insert(log);
        }

        public CityList SearchCities(string keyword, string language, int numberOfList)
        {
            string url = ConfigurationManager.AppSettings["CITYSEARCH.URL"];
            url += String.Format("{0}/1/{1}/{2}", numberOfList.ToString(), language.ToLower(), keyword);

            CityList cities = new CityList();
            string json = HttpUtility.getJSON(url);
            if (String.IsNullOrEmpty(json) == false)
            {
                cities = JsonConvert.DeserializeObject<CityList>(json);
            }

            return cities;
        }

        public FlightSearchResult Search(Entities.MasterPricer.Request request)
        {
            FlightSearchResult result = new FlightSearchResult();
            MarkupServices markupSvc = new MarkupServices(_unitOfWork);
            FareMarkup markup = markupSvc.GetMarkup();
            NamingServices namingService = new NamingServices(_unitOfWork);
            AirlineControlServices airlineControlSvc = new AirlineControlServices(_unitOfWork);

            var controlByRoute = airlineControlSvc.GetAll();
            string depCountry = namingService.GetCountryCode(request.flights[0].departureCity.ToUpper());
            string arrCountry = namingService.GetCountryCode(request.flights[0].arrivalCity.ToUpper());
            bool isDomestic = (depCountry == "TH" && arrCountry == "TH");
            Log.Debug("isDomestic=" + isDomestic);

            string tripType = request.flights.Count > 2 ? "M" : "O";
            if (request.flights.Count == 2)
            {
                if (request.flights[0].arrivalCity == request.flights[1].departureCity)
                {
                    tripType = "R";
                }
                else
                {
                    tripType = "M";
                }
            }

            bool bAllowByAirlineControl = false;
            AirlineControl airlineControl = null;
            //keep search by airline from search box
            List<string> airlineFromSearh = request.airlineCode;
            if (!tripType.Equals("M"))
            {
                var chkAllowRoute = controlByRoute.FindAll(x => (x.OriginalCountryCode == "*" || x.OriginalCountryCode == depCountry) && (x.DestinationCountryCode == "*" || x.DestinationCountryCode == arrCountry));
                if (chkAllowRoute != null && chkAllowRoute.Count > 0)
                {
                    airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == depCountry && x.DestinationCountryCode == arrCountry);
                    if (airlineControl != null)
                    {
                        bAllowByAirlineControl = airlineControl.IsActive.Value;
                    }
                    else
                    {
                        airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == depCountry && x.DestinationCountryCode == "*");
                        if (airlineControl != null)
                        {
                            bAllowByAirlineControl = airlineControl.IsActive.Value;
                        }
                        else
                        {
                            airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == "*" && x.DestinationCountryCode == arrCountry);
                            if (airlineControl != null)
                            {
                                bAllowByAirlineControl = airlineControl.IsActive.Value;
                            }
                            else
                            {
                                airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == "*" && x.DestinationCountryCode == "*");
                                if (airlineControl != null)
                                {
                                    bAllowByAirlineControl = airlineControl.IsActive.Value;
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                bAllowByAirlineControl = true;
                int iRouteIsDomestic = 0;
                foreach (var route in request.flights)
                {
                    depCountry = namingService.GetCountryCode(route.departureCity.ToUpper());
                    arrCountry = namingService.GetCountryCode(route.arrivalCity.ToUpper());
                    if (depCountry == "TH" && arrCountry == "TH")
                    {
                        iRouteIsDomestic++;
                        Log.Debug("isDomestic=" + isDomestic);
                    }

                    var chkAllowRoute = controlByRoute.FindAll(x => (x.OriginalCountryCode == "*" || x.OriginalCountryCode == depCountry) && (x.DestinationCountryCode == "*" || x.DestinationCountryCode == arrCountry));
                    if (chkAllowRoute != null && chkAllowRoute.Count > 0)
                    {
                        airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == depCountry && x.DestinationCountryCode == arrCountry);
                        if (airlineControl != null)
                        {
                            bAllowByAirlineControl = airlineControl.IsActive.Value;
                        }
                        else
                        {
                            airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == depCountry && x.DestinationCountryCode == "*");
                            if (airlineControl != null)
                            {
                                bAllowByAirlineControl = airlineControl.IsActive.Value;
                            }
                            else
                            {
                                airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == "*" && x.DestinationCountryCode == arrCountry);
                                if (airlineControl != null)
                                {
                                    bAllowByAirlineControl = airlineControl.IsActive.Value;
                                }
                                else
                                {
                                    airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == "*" && x.DestinationCountryCode == "*");
                                    if (airlineControl != null)
                                    {
                                        bAllowByAirlineControl = airlineControl.IsActive.Value;
                                    }
                                }
                            }
                        }

                    }// if (chkAllowRoute != null && chkAllowRoute.Count > 0)
                    if (!bAllowByAirlineControl)
                    {
                        break;
                    }
                }
                if (iRouteIsDomestic == request.flights.Count)
                {
                    isDomestic = true;
                }
            }//if (!tripType.Equals("M"))
            Log.Debug("bAllowByAirlineControl=" + bAllowByAirlineControl);
            List<AirlineControlSub> airlineControlSub = null;
            List<MasterFlight> masterFlightList = new List<MasterFlight>();
            Entities.MasterPricer.Response response = new Entities.MasterPricer.Response();
            List<Recommendation> recommendations = null;
            string url = "";
            string requestJson = "";
            string json = "";
            if (bAllowByAirlineControl)
            {
                airlineControlSub = airlineControlSvc.GetAllAirlineSub(airlineControl.AirlineOID);
                if (airlineControl != null && request.airlineCode == null)
                {

                    if (airlineControlSub != null && airlineControlSub.Count > 0)
                    {
                        var airlineControlSubL = airlineControlSub.FindAll(x => x.AirlineCode != "YY" && x.IsActive == true && x.FareBasis == "*" && x.ClassOfService == "*");
                        if (airlineControlSubL != null && airlineControlSubL.Count > 0)
                        {
                            request.airlineCode = new List<string>();
                            foreach (var airlineL in airlineControlSubL)
                            {
                                foreach (var airline in airlineL.AirlineCode.Split(','))
                                {
                                    request.airlineCode.Add(airline);
                                }
                            }
                        }
                    }
                }
                request.inFlightServices = 1;
                request.noOfRecommendation = 250;
                request.FareFamilyGroup = null;

                url = ConfigurationManager.AppSettings["MASTERPRICER.URL"];
                requestJson = JsonConvert.SerializeObject(request);


                Log.Info("Search Start");
                json = HttpUtility.postJSON(url, requestJson);
                Log.Debug(requestJson);
                Log.Debug(json);
                Log.Info("Search End");
                //return airlineFromSearh to request
                request.airlineCode = airlineFromSearh;

                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");
                    response = BL.Entities.MasterPricer.Response.FromJson(json);

                    recommendations = new List<Recommendation>();
                    if (response == null || response.FareMasterPricerTravelBoardSearchReply == null)
                    {
                        Log.Debug(requestJson);
                        Log.Debug(json);
                        //return null; //comment for search flight from kiwi on 2021-06-29 by NING
                    }
                    else
                    {

                        if (response.FareMasterPricerTravelBoardSearchReply.Recommendation.PurpleRecommendation != null)
                        {
                            recommendations.Add(response.FareMasterPricerTravelBoardSearchReply.Recommendation.PurpleRecommendation);
                        }
                        else
                        {
                            recommendations = response.FareMasterPricerTravelBoardSearchReply.Recommendation.RecommendationElementArray;
                        }
                        if (recommendations.Count <= 0)
                        {
                            Log.Debug(requestJson);
                            Log.Debug(json);
                            //return null; //comment for search flight from kiwi on 2021-06-29 by NING
                        }
                        else
                        {
                            //Log.Debug(requestJson);
                            //Log.Debug(json);
                            int iCheck = 0;
                            string chkSameAirline = "";
                            bool ShowOnlySameAirline = false;
                            List<GroupOfFlight> gofFlight = new List<GroupOfFlight>();
                            List<Entities.RobinhoodFlight.Flight> GetAllFlight = new List<Entities.RobinhoodFlight.Flight>();
                            MasterFlight masterFlight = new MasterFlight();
                            foreach (var recommendation in recommendations)
                            {
                                masterFlight = new MasterFlight();
                                masterFlight.type = "A";//A=Amadeus,K=Kiwi
                                masterFlight.id = Guid.NewGuid();

                                if (!tripType.Equals("M"))
                                {
                                    var depFlight = response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex == null ?
                                    response.FareMasterPricerTravelBoardSearchReply.FlightIndex.FlightIndexElementArray[0].GroupOfFlights :
                                 response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex.GroupOfFlights;

                                    List<GroupOfFlight> gofDep = new List<GroupOfFlight>();
                                    if (depFlight.PurpleGroupOfFlight != null)
                                    {
                                        gofDep.Add(depFlight.PurpleGroupOfFlight);
                                    }
                                    else
                                    {
                                        gofDep = depFlight.GroupOfFlightElementArray;
                                    }
                                    masterFlight.departureFlight = GetFlight(gofDep, recommendation.PaxFareProduct, "ADT", recommendation.SegmentFlightRef, request.languageCode, request.flights[0].departureDateTime, 1, depCountry, arrCountry, airlineControlSub, response.FareMasterPricerTravelBoardSearchReply.ServiceFeesGrp, request.pgSearchOID);
                                    if (masterFlight.departureFlight.Count > 0)
                                    {
                                        if (request.flights.Count > 1)
                                        {
                                            var retFlight = response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex == null ?
                                                response.FareMasterPricerTravelBoardSearchReply.FlightIndex.FlightIndexElementArray[1].GroupOfFlights :
                                         response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex.GroupOfFlights;

                                            List<GroupOfFlight> gofRet = new List<GroupOfFlight>();
                                            if (retFlight.PurpleGroupOfFlight != null)
                                            {
                                                gofRet.Add(retFlight.PurpleGroupOfFlight);
                                            }
                                            else
                                            {
                                                gofRet = retFlight.GroupOfFlightElementArray;
                                            }
                                            masterFlight.returnFlight = GetFlight(gofRet, recommendation.PaxFareProduct, "ADT", recommendation.SegmentFlightRef, request.languageCode, request.flights[1].departureDateTime, 2, depCountry, arrCountry, airlineControlSub, response.FareMasterPricerTravelBoardSearchReply.ServiceFeesGrp, request.pgSearchOID);

                                        }
                                        if (request.flights.Count == 1 || (request.flights.Count > 1 && masterFlight.returnFlight.Count > 0))
                                        {
                                            masterFlight.ComputeConnectionTime();

                                            masterFlight.fare = new Pricing(request.noOfAdults, request.noOfChildren, request.noOfInfants);

                                            masterFlight.fare.currencyCode = response.FareMasterPricerTravelBoardSearchReply.ConversionRate.ConversionRateDetail.Currency;

                                            masterFlight.fare.adtFare = new PaxPricing();
                                            masterFlight.fare.adtFare = getPaxFare(recommendation.PaxFareProduct, "ADT", markup, masterFlight.departureFlight, namingService, request.userEmail, (request.flights.Count > 1 && masterFlight.returnFlight.Count > 0 ? "R" : "O"));

                                            if (request.noOfChildren > 0)
                                            {
                                                masterFlight.fare.chdFare = new PaxPricing();
                                                masterFlight.fare.chdFare = getPaxFare(recommendation.PaxFareProduct, "CHD,CH", markup, masterFlight.departureFlight, namingService, request.userEmail, (request.flights.Count > 1 && masterFlight.returnFlight.Count > 0 ? "R" : "O"));
                                            }

                                            if (request.noOfInfants > 0)
                                            {
                                                masterFlight.fare.infFare = new PaxPricing();
                                                masterFlight.fare.infFare = getPaxFare(recommendation.PaxFareProduct, "INF,IN", markup, masterFlight.departureFlight, namingService, request.userEmail, (request.flights.Count > 1 && masterFlight.returnFlight.Count > 0 ? "R" : "O"));
                                            }
                                            masterFlight.fareRule = getFareRule(recommendation.PaxFareProduct);

                                            masterFlightList.Add(masterFlight);
                                        }
                                    }
                                }
                                else
                                {
                                    iCheck = 0;
                                    #region                       
                                    for (int i = 0; i < request.flights.Count; i++)
                                    {
                                        var gFlight = response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex == null ?
                                        response.FareMasterPricerTravelBoardSearchReply.FlightIndex.FlightIndexElementArray[i].GroupOfFlights :
                                        response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex.GroupOfFlights;
                                        gofFlight = new List<GroupOfFlight>();
                                        GetAllFlight = new List<Entities.RobinhoodFlight.Flight>();
                                        if (gFlight.PurpleGroupOfFlight != null)
                                        {
                                            gofFlight.Add(gFlight.PurpleGroupOfFlight);
                                        }
                                        else
                                        {
                                            gofFlight = gFlight.GroupOfFlightElementArray;
                                        }
                                        if (tripType == "M")
                                        {
                                            depCountry = namingService.GetCountryCode(request.flights[i].departureCity);
                                            arrCountry = namingService.GetCountryCode(request.flights[i].arrivalCity);
                                        }
                                        GetAllFlight = GetFlight(gofFlight, recommendation.PaxFareProduct, "ADT", recommendation.SegmentFlightRef, request.languageCode, request.flights[i].departureDateTime, i + 1, depCountry, arrCountry, airlineControlSub, response.FareMasterPricerTravelBoardSearchReply.ServiceFeesGrp, request.pgSearchOID);
                                        Log.Debug("GetAllFlight.Count=" + GetAllFlight.Count);
                                        if (GetAllFlight.Count > 0)
                                        {
                                            if (!ShowOnlySameAirline)
                                            {
                                                iCheck++;
                                            }
                                            switch (i + 1)
                                            {
                                                case 1:
                                                    masterFlight.Flight_SegRef1 = GetAllFlight;
                                                    if (ShowOnlySameAirline)
                                                    {
                                                        chkSameAirline = GetAllFlight[0].flightDetails[0].airline.code;
                                                        iCheck++;
                                                    }
                                                    break;
                                                case 2:
                                                    masterFlight.Flight_SegRef2 = GetAllFlight;
                                                    if (ShowOnlySameAirline && chkSameAirline == GetAllFlight[0].flightDetails[0].airline.code)
                                                    {
                                                        iCheck++;
                                                    }
                                                    break;
                                                case 3:
                                                    masterFlight.Flight_SegRef3 = GetAllFlight;
                                                    if (ShowOnlySameAirline && chkSameAirline == GetAllFlight[0].flightDetails[0].airline.code)
                                                    {
                                                        iCheck++;
                                                    }
                                                    break;
                                                case 4:
                                                    masterFlight.Flight_SegRef4 = GetAllFlight;
                                                    if (ShowOnlySameAirline && chkSameAirline == GetAllFlight[0].flightDetails[0].airline.code)
                                                    {
                                                        iCheck++;
                                                    }
                                                    break;
                                                case 5:
                                                    masterFlight.Flight_SegRef5 = GetAllFlight;
                                                    if (ShowOnlySameAirline && chkSameAirline == GetAllFlight[0].flightDetails[0].airline.code)
                                                    {
                                                        iCheck++;
                                                    }
                                                    break;
                                                case 6:
                                                    masterFlight.Flight_SegRef6 = GetAllFlight;
                                                    if (ShowOnlySameAirline && chkSameAirline == GetAllFlight[0].flightDetails[0].airline.code)
                                                    {
                                                        iCheck++;
                                                    }
                                                    break;
                                            }

                                        }
                                    }//for
                                    Log.Debug("iCheck=" + iCheck + "/request.flights.Count=" + request.flights.Count);
                                    if (iCheck == request.flights.Count)
                                    {
                                        masterFlight.ComputeConnectionTime();

                                        masterFlight.fare = new Pricing(request.noOfAdults, request.noOfChildren, request.noOfInfants);

                                        masterFlight.fare.currencyCode = response.FareMasterPricerTravelBoardSearchReply.ConversionRate.ConversionRateDetail.Currency;

                                        masterFlight.fare.adtFare = new PaxPricing();
                                        masterFlight.fare.adtFare = getPaxFare(recommendation.PaxFareProduct, "ADT", markup, masterFlight.Flight_SegRef1, namingService, request.userEmail, "R");

                                        if (request.noOfChildren > 0)
                                        {
                                            masterFlight.fare.chdFare = new PaxPricing();
                                            masterFlight.fare.chdFare = getPaxFare(recommendation.PaxFareProduct, "CHD,CH", markup, masterFlight.Flight_SegRef1, namingService, request.userEmail, "R");
                                        }

                                        if (request.noOfInfants > 0)
                                        {
                                            masterFlight.fare.infFare = new PaxPricing();
                                            masterFlight.fare.infFare = getPaxFare(recommendation.PaxFareProduct, "INF,IN", markup, masterFlight.Flight_SegRef1, namingService, request.userEmail, "R");
                                        }
                                        masterFlight.fareRule = getFareRule(recommendation.PaxFareProduct);

                                        masterFlightList.Add(masterFlight);
                                    }
                                    #endregion
                                }//if (!tripType.Equals("M"))
                            }//foreach
                        }//if (recommendations.Count <= 0)
                    }// if (response == null || response.FareMasterPricerTravelBoardSearchReply == null)
                }//if (String.IsNullOrEmpty(json) == false)
            } //if bAllowByAirlineControl

            //SetupScale
            if (masterFlightList != null && masterFlightList.Count > 0)
            {
                masterFlightList = masterFlightList.OrderBy(x => x.fare.adtFare.net).ToList();
                result.flights = masterFlightList;

                //find filter
                result.filter = new FlightFilter();
                List<string> airlines = result.flights.Select(x => x.mainAirline.code).Distinct().ToList();
                result.filter.airlines = new List<Airline>();
                //NamingServices namingService = new NamingServices(_unitOfWork);
                foreach (var airline in airlines)
                {
                    Airline a = new Airline(namingService, request.languageCode);
                    a.code = airline;
                    result.filter.airlines.Add(a);
                }
                result.filter.airlines.Sort((x, y) => x.name.CompareTo(y.name));
                result.filter.maxPrice = result.flights.Max(x => x.fare.adtFare.net);
                result.filter.minPrice = result.flights.Min(x => x.fare.adtFare.net);

                if (!tripType.Equals("M"))
                {
                    int maxDepTime = masterFlightList.Max(x => x.maxDepTime);
                    int maxRetTime = masterFlightList.Max(x => x.maxRetTime);
                    foreach (var mp in masterFlightList)
                    {
                        mp.UIScale(maxDepTime, maxRetTime);
                    }

                    result.filter.maxDepDuration = maxDepTime;
                    result.filter.maxRetDuration = maxRetTime;

                    result.filter.maxStop = result.flights.Max(x => x.departureFlight.Max(y => y.flightDetails.Count - 1));

                    result.filter.departureAirport = new List<Airport>();
                    result.filter.arrivalAirport = new List<Airport>();
                    foreach (var filterFlight in result.flights)
                    {
                        string fDepCode = filterFlight.departureFlight[0].flightDetails[0].depCity.code;
                        string fArrCode = filterFlight.departureFlight[0].flightDetails[filterFlight.departureFlight[0].flightDetails.Count - 1].arrCity.code;
                        if (result.filter.departureAirport.FirstOrDefault(x => x.code == fDepCode) == null)
                        {
                            Airport a = new Airport(namingService, false, request.languageCode);
                            a.code = fDepCode;
                            result.filter.departureAirport.Add(a);
                        }
                        if (result.filter.arrivalAirport.FirstOrDefault(x => x.code == fArrCode) == null)
                        {
                            Airport a = new Airport(namingService, false, request.languageCode);
                            a.code = fArrCode;
                            result.filter.arrivalAirport.Add(a);
                        }
                    }
                }
                else
                {
                    int maxSegRef1Time = masterFlightList.Max(x => x.maxSegRef1Time);
                    int maxSegRef2Time = masterFlightList.Max(x => x.maxSegRef2Time);
                    int maxSegRef3Time = masterFlightList.Max(x => x.maxSegRef3Time);
                    int maxSegRef4Time = masterFlightList.Max(x => x.maxSegRef4Time);
                    int maxSegRef5Time = masterFlightList.Max(x => x.maxSegRef5Time);
                    int maxSegRef6Time = masterFlightList.Max(x => x.maxSegRef6Time);
                    foreach (var mp in masterFlightList)
                    {
                        mp.UIScale(maxSegRef1Time, maxSegRef2Time, maxSegRef3Time, maxSegRef4Time, maxSegRef5Time, maxSegRef6Time);
                    }
                }//if (!tripType.Equals("M"))
            }//if (masterFlightList != null && masterFlightList.Count > 0)
            result.pgSearchOID = request.pgSearchOID;

            Log.Info("Convert to our object End");
            return result;
        }

        public FlightSearchMultiTicketResult SearchMultiTicket(Entities.MasterPricer.Request request)
        {
            FlightSearchMultiTicketResult result = new FlightSearchMultiTicketResult();
            MarkupServices markupSvc = new MarkupServices(_unitOfWork);
            FareMarkup markup = markupSvc.GetMarkup();
            NamingServices namingService = new NamingServices(_unitOfWork);
            AirlineControlServices airlineControlSvc = new AirlineControlServices(_unitOfWork);
            AirlineConfigServices airlineConfigSvc = new AirlineConfigServices(_unitOfWork);
            var controlByRoute = airlineControlSvc.GetAll();
            var configByRoute = airlineConfigSvc.GetAll();
            string depCountry = namingService.GetCountryCode(request.flights[0].departureCity.ToUpper());
            string arrCountry = namingService.GetCountryCode(request.flights[0].arrivalCity.ToUpper());
            bool isDomestic = (depCountry == "TH" && arrCountry == "TH");
            Log.Debug("isDomestic=" + isDomestic);
            bool bAllowByAirlineControl = false;
            AirlineControl airlineControl = null;
            //keep search by airline from search box
            List<string> airlineFromSearh = request.airlineCode;

            var chkAllowRoute = controlByRoute.FindAll(x => (x.OriginalCountryCode == "*" || x.OriginalCountryCode == depCountry) && (x.DestinationCountryCode == "*" || x.DestinationCountryCode == arrCountry));
            if (chkAllowRoute != null && chkAllowRoute.Count > 0)
            {
                airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == depCountry && x.DestinationCountryCode == arrCountry);
                if (airlineControl != null)
                {
                    bAllowByAirlineControl = airlineControl.IsActive.Value;
                }
                else
                {
                    airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == depCountry && x.DestinationCountryCode == "*");
                    if (airlineControl != null)
                    {
                        bAllowByAirlineControl = airlineControl.IsActive.Value;
                    }
                    else
                    {
                        airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == "*" && x.DestinationCountryCode == arrCountry);
                        if (airlineControl != null)
                        {
                            bAllowByAirlineControl = airlineControl.IsActive.Value;
                        }
                        else
                        {
                            airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == "*" && x.DestinationCountryCode == "*");
                            if (airlineControl != null)
                            {
                                bAllowByAirlineControl = airlineControl.IsActive.Value;
                            }
                        }
                    }
                }

            }
            Log.Debug("bAllowByAirlineControl=" + bAllowByAirlineControl);
            List<AirlineControlSub> airlineControlSub = null;
            Entities.MasterPricer.Response response = new Entities.MasterPricer.Response();
            List<Recommendation> recommendations = null;
            List<MultiTicketFlight> multiTicketFlightList = new List<MultiTicketFlight>();
            string url = "";
            string requestJson = "";
            string json = "";
            if (bAllowByAirlineControl)
            {
                airlineControlSub = airlineControlSvc.GetAllAirlineSub(airlineControl.AirlineOID);
                if (configByRoute != null && configByRoute.Count > 0 && request.airlineCode == null)
                {
                    configByRoute = configByRoute.FindAll(x => x.IsActive == true);
                    var airlineConfig = airlineConfigSvc.GetAirlineConfig(configByRoute, request.flights[0].departureCity.ToUpper(), request.flights[0].arrivalCity.ToUpper());
                    if (airlineConfig != null)
                    {
                        if (airlineConfig.IsInclude.Value)
                        {
                            request.airlineCode = airlineConfig.AirlineCode.Split(',').ToList();
                        }
                        else
                        {
                            request.blockAirlineCode = airlineConfig.AirlineCode.Split(',').ToList();
                        }
                    }
                }
                else
                {

                    if (airlineControl != null && request.airlineCode == null)
                    {

                        if (airlineControlSub != null && airlineControlSub.Count > 0)
                        {
                            var airlineControlSubL = airlineControlSub.FindAll(x => x.AirlineCode != "YY" && x.IsActive == true && x.FareBasis == "*" && x.ClassOfService == "*");
                            if (airlineControlSubL != null && airlineControlSubL.Count > 0)
                            {
                                request.airlineCode = new List<string>();
                                foreach (var airlineL in airlineControlSubL)
                                {
                                    foreach (var airline in airlineL.AirlineCode.Split(','))
                                    {
                                        request.airlineCode.Add(airline);
                                    }
                                }
                            }
                        }
                    }
                }

                request.inFlightServices = 1;
                request.noOfRecommendation = 200;// 250;
                request.multiTicket = request.flights.Count == 1 ? false : true;
                if (request.multiTicket && request.flights.Count > 1)
                {
                    request.multiTicketOption = new multiTicketOption();
                    request.multiTicketOption.enable = true;
                    SiteConfigServices siteConfig = new SiteConfigServices(_unitOfWork);
                    var dataConfig = siteConfig.GetByKey("InboundWeight");
                    request.multiTicketOption.inboundWeight = int.Parse(dataConfig.ConfigValue);//40 ConfigurationManager.AppSettings["InboundWeight"]
                    dataConfig = siteConfig.GetByKey("OutboundWeight");
                    request.multiTicketOption.outboundWeight = int.Parse(dataConfig.ConfigValue);//40 ConfigurationManager.AppSettings["OutboundWeight"]
                    dataConfig = siteConfig.GetByKey("RoundtripWeight");
                    request.multiTicketOption.roundtripWeight = int.Parse(dataConfig.ConfigValue);//20 ConfigurationManager.AppSettings["RoundtripWeight"]
                }

                url = ConfigurationManager.AppSettings["MASTERPRICER.URL"];
                requestJson = JsonConvert.SerializeObject(request);


                Log.Info("Search Start");
                json = HttpUtility.postJSON(url, requestJson);
                Log.Debug(requestJson);
                Log.Debug(json);
                Log.Info("Search End");
                //return airlineFromSearh to request
                request.airlineCode = airlineFromSearh;

                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");
                    response = BL.Entities.MasterPricer.Response.FromJson(json);

                    recommendations = new List<Recommendation>();
                    if (response == null || response.FareMasterPricerTravelBoardSearchReply == null)
                    {
                        Log.Debug(requestJson);
                        Log.Debug(json);
                        //return null; //comment for search flight from kiwi on 2021-06-29 by NING
                    }
                    else
                    {
                        DiscountTagServices discountTagSvc = new DiscountTagServices(_unitOfWork);
                        var discountTagList = discountTagSvc.GetAll();
                        bool isAdult = true;
                        bool isChild = request.noOfChildren > 0;
                        Entities.DiscountTag.DiscountTagEntities discountTag = null;
                        if (discountTagList != null)
                        {
                            discountTagList = discountTagList.FindAll(x => x.discountTag.StartBookingDate <= DateTime.Now && x.discountTag.FinishBookingDate >= DateTime.Now
                            && (x.discountTag.ZoneFrom == "*" || x.discountTag.ZoneFrom.IndexOf(request.flights[0].departureCity) != -1 || x.discountTag.ZoneFrom.IndexOf(depCountry) != -1)
                            && (x.discountTag.ZoneTo == "*" || x.discountTag.ZoneTo.IndexOf(request.flights[0].arrivalCity) != -1 || x.discountTag.ZoneTo.IndexOf(arrCountry) != -1)
                            /*&& (isAdult ? x.discountTag.PaxTypeADT.GetValueOrDefault() : (isChild ? x.discountTag.PaxTypeCHD.GetValueOrDefault() : x.discountTag.PaxTypeINF.GetValueOrDefault()))*/);
                        }

                        if (response.FareMasterPricerTravelBoardSearchReply.Recommendation.PurpleRecommendation != null)
                        {
                            recommendations.Add(response.FareMasterPricerTravelBoardSearchReply.Recommendation.PurpleRecommendation);
                        }
                        else
                        {
                            recommendations = response.FareMasterPricerTravelBoardSearchReply.Recommendation.RecommendationElementArray;
                        }
                        if (recommendations.Count <= 0)
                        {
                            Log.Debug(requestJson);
                            Log.Debug(json);
                            //return null; //comment for search flight from kiwi on 2021-06-29 by NING
                        }
                        else
                        {
                            List<GroupOfFlight> gofFlight = new List<GroupOfFlight>();
                            List<Entities.RobinhoodFlight.Flight> GetAllFlight = new List<Entities.RobinhoodFlight.Flight>();
                            foreach (var recommendation in recommendations)
                            {
                                MultiTicketFlight multiTicket = new MultiTicketFlight();
                                multiTicket.type = "A";//A=Amadeus,L=LionAir
                                multiTicket.id = Guid.NewGuid();
                                multiTicket.recommendation_number = recommendation.ItemNumber.ItemNumberId.Number;
                                Log.Debug("multiTicket.recommendation_number=" + multiTicket.recommendation_number);
                                multiTicket.isMultiTicket = false;
                                if (recommendation.ItemNumber.ItemNumberId.NumberType == null)//normal
                                {
                                    for (int i = 0; i < request.flights.Count; i++)
                                    {
                                        var gFlight = response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex == null ?
                                        response.FareMasterPricerTravelBoardSearchReply.FlightIndex.FlightIndexElementArray[i].GroupOfFlights :
                                        response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex.GroupOfFlights;
                                        gofFlight = new List<GroupOfFlight>();
                                        GetAllFlight = new List<Entities.RobinhoodFlight.Flight>();
                                        if (gFlight.PurpleGroupOfFlight != null)
                                        {
                                            gofFlight.Add(gFlight.PurpleGroupOfFlight);
                                        }
                                        else
                                        {
                                            gofFlight = gFlight.GroupOfFlightElementArray;
                                        }
                                        GetAllFlight = GetFlight(gofFlight, recommendation.PaxFareProduct, "ADT", recommendation.SegmentFlightRef, request.languageCode, request.flights[i].departureDateTime, i + 1, depCountry, arrCountry, airlineControlSub, response.FareMasterPricerTravelBoardSearchReply.ServiceFeesGrp, request.pgSearchOID);
                                        if (GetAllFlight.Count > 0)
                                        {
                                            if (discountTagList != null && discountTagList.Count > 0)
                                            {
                                                discountTag = GetDiscountTag(discountTagList, GetAllFlight[0].flightDetails[0].airline.code, GetAllFlight[0].flightDetails[0].fareBasis, GetAllFlight[0].flightDetails[0].rbd);
                                            }
                                            switch (i + 1)
                                            {
                                                case 1:
                                                    multiTicket.Flight_SegRef1 = GetAllFlight;

                                                    break;
                                                case 2:
                                                    multiTicket.Flight_SegRef2 = GetAllFlight;
                                                    break;
                                            }

                                        }
                                    }
                                    if (multiTicket.Flight_SegRef1 != null)
                                    {
                                        List<string> upsell = GetUpsell(recommendation.SegmentFlightRef);
                                        if (upsell != null && upsell.Count > 0)
                                        {
                                            multiTicket.upsell = upsell;
                                        }

                                        multiTicket.ComputeConnectionTime();

                                        if (discountTag != null)
                                        {
                                            multiTicket.discountTag = new Entities.RobinhoodFare.DiscountTag();
                                            multiTicket.discountTag.discountAmount = discountTag.discountTag.DiscountAmt.Value;
                                            multiTicket.discountTag.discountIsPercent = discountTag.discountTag.DiscountIsPercent.Value;
                                            multiTicket.discountTag.promotionGroupCode = discountTag.promotionGroupCode.PromotionGroupCode1;
                                            multiTicket.discountTag.promotionText = discountTag.discountTagDetail.Find(x => x.LanguageCode == request.languageCode).PromotionTag;
                                        }

                                        multiTicket.fare = new Pricing(request.noOfAdults, request.noOfChildren, request.noOfInfants);

                                        multiTicket.fare.currencyCode = response.FareMasterPricerTravelBoardSearchReply.ConversionRate.ConversionRateDetail.Currency;

                                        multiTicket.fare.adtFare = new PaxPricing();
                                        multiTicket.fare.adtFare = getPaxFare(recommendation.PaxFareProduct, "ADT", markup, multiTicket.Flight_SegRef1, namingService, request.userEmail, (request.flights.Count == 1 ? "O" : "R"));
                                        if (multiTicket.discountTag != null)
                                        {
                                            if (multiTicket.discountTag.discountIsPercent)
                                            {
                                                multiTicket.fare.adtFare.strikethroughPrice = multiTicket.fare.adtFare.net + ceilingTo5Or10((multiTicket.fare.adtFare.net * multiTicket.discountTag.discountAmount) / 100);
                                            }
                                            else
                                            {
                                                multiTicket.fare.adtFare.strikethroughPrice = multiTicket.fare.adtFare.net + multiTicket.discountTag.discountAmount;
                                            }
                                        }
                                        if (request.noOfChildren > 0)
                                        {
                                            multiTicket.fare.chdFare = new PaxPricing();
                                            multiTicket.fare.chdFare = getPaxFare(recommendation.PaxFareProduct, "CHD,CH", markup, multiTicket.Flight_SegRef1, namingService, request.userEmail, (request.flights.Count == 1 ? "O" : "R"));
                                            if (multiTicket.discountTag != null)
                                            {
                                                if (multiTicket.discountTag.discountIsPercent)
                                                {
                                                    multiTicket.fare.chdFare.strikethroughPrice = multiTicket.fare.chdFare.net + ceilingTo5Or10((multiTicket.fare.chdFare.net * multiTicket.discountTag.discountAmount) / 100);
                                                }
                                                else
                                                {
                                                    multiTicket.fare.chdFare.strikethroughPrice = multiTicket.fare.chdFare.net + multiTicket.discountTag.discountAmount;
                                                }
                                            }
                                        }

                                        if (request.noOfInfants > 0)
                                        {
                                            multiTicket.fare.infFare = new PaxPricing();
                                            multiTicket.fare.infFare = getPaxFare(recommendation.PaxFareProduct, "INF,IN", markup, multiTicket.Flight_SegRef1, namingService, request.userEmail, (request.flights.Count == 1 ? "O" : "R"));
                                            if (multiTicket.discountTag != null)
                                            {
                                                if (multiTicket.discountTag.discountIsPercent)
                                                {
                                                    multiTicket.fare.infFare.strikethroughPrice = multiTicket.fare.infFare.net + ceilingTo5Or10((multiTicket.fare.infFare.net * multiTicket.discountTag.discountAmount) / 100);
                                                }
                                                else
                                                {
                                                    multiTicket.fare.infFare.strikethroughPrice = multiTicket.fare.infFare.net + multiTicket.discountTag.discountAmount;
                                                }
                                            }
                                        }
                                        if (multiTicket.discountTag != null)
                                        {
                                            if (multiTicket.discountTag.discountIsPercent)
                                            {
                                                multiTicket.fare.strikethroughTotalPrice = multiTicket.fare.total + ceilingTo5Or10((multiTicket.fare.total * multiTicket.discountTag.discountAmount) / 100);
                                            }
                                            else
                                            {
                                                multiTicket.fare.strikethroughTotalPrice = multiTicket.fare.total + multiTicket.discountTag.discountAmount;
                                            }
                                        }
                                        multiTicket.fareRule = getFareRule(recommendation.PaxFareProduct);
                                        if (recommendation.FareFamilyRef != null)
                                        {
                                            multiTicket.FamilyInformation = GetFamilyInformation(recommendation.FareFamilyRef.ReferencingDetail, response.FareMasterPricerTravelBoardSearchReply.FamilyInformation, response.FareMasterPricerTravelBoardSearchReply.ServiceFeesGrp);
                                        }

                                        multiTicketFlightList.Add(multiTicket);
                                    }
                                }
                                else//multi Ticket
                                {
                                    int segmentRef = 0;
                                    multiTicket.isMultiTicket = true;

                                    GetAllFlight = GetFlight(response.FareMasterPricerTravelBoardSearchReply.FlightIndex, recommendation.PaxFareProduct, "ADT", recommendation.SegmentFlightRef, request, ref segmentRef, depCountry, arrCountry, airlineControlSub, response.FareMasterPricerTravelBoardSearchReply.ServiceFeesGrp, request.pgSearchOID);
                                    if (GetAllFlight != null && GetAllFlight.Count > 0)
                                    {
                                        if (discountTagList != null && discountTagList.Count > 0)
                                        {
                                            discountTag = GetDiscountTag(discountTagList, GetAllFlight[0].flightDetails[0].airline.code, GetAllFlight[0].flightDetails[0].fareBasis, GetAllFlight[0].flightDetails[0].rbd);
                                        }
                                        switch (segmentRef)
                                        {
                                            case 1:
                                                multiTicket.Flight_SegRef1 = GetAllFlight;
                                                break;
                                            case 2:
                                                multiTicket.Flight_SegRef2 = GetAllFlight;
                                                break;
                                        }
                                        List<string> upsell = GetUpsell(recommendation.SegmentFlightRef);
                                        if (upsell != null && upsell.Count > 0)
                                        {
                                            multiTicket.upsell = upsell;
                                        }
                                        multiTicket.ComputeConnectionTime();
                                        if (discountTag != null)
                                        {
                                            multiTicket.discountTag = new Entities.RobinhoodFare.DiscountTag();
                                            multiTicket.discountTag.discountAmount = discountTag.discountTag.DiscountAmt.Value;
                                            multiTicket.discountTag.discountIsPercent = discountTag.discountTag.DiscountIsPercent.Value;
                                            multiTicket.discountTag.promotionGroupCode = discountTag.promotionGroupCode.PromotionGroupCode1;
                                            multiTicket.discountTag.promotionText = discountTag.discountTagDetail.Find(x => x.LanguageCode == request.languageCode).PromotionTag;
                                        }
                                        multiTicket.fare = new Pricing(request.noOfAdults, request.noOfChildren, request.noOfInfants);

                                        multiTicket.fare.currencyCode = response.FareMasterPricerTravelBoardSearchReply.ConversionRate.ConversionRateDetail.Currency;

                                        multiTicket.fare.adtFare = new PaxPricing();
                                        multiTicket.fare.adtFare = getPaxFare(recommendation.PaxFareProduct, "ADT", markup, segmentRef == 1 ? multiTicket.Flight_SegRef1 : multiTicket.Flight_SegRef2, namingService, request.userEmail, "O");
                                        if (multiTicket.discountTag != null)
                                        {
                                            if (multiTicket.discountTag.discountIsPercent)
                                            {
                                                multiTicket.fare.adtFare.strikethroughPrice = multiTicket.fare.adtFare.net + ceilingTo5Or10((multiTicket.fare.adtFare.net * multiTicket.discountTag.discountAmount) / 100);
                                            }
                                            else
                                            {
                                                multiTicket.fare.adtFare.strikethroughPrice = multiTicket.fare.adtFare.net + multiTicket.discountTag.discountAmount;
                                            }
                                        }
                                        if (request.noOfChildren > 0)
                                        {
                                            multiTicket.fare.chdFare = new PaxPricing();
                                            multiTicket.fare.chdFare = getPaxFare(recommendation.PaxFareProduct, "CHD,CH", markup, segmentRef == 1 ? multiTicket.Flight_SegRef1 : multiTicket.Flight_SegRef2, namingService, request.userEmail, "O");
                                            if (multiTicket.discountTag != null)
                                            {
                                                if (multiTicket.discountTag.discountIsPercent)
                                                {
                                                    multiTicket.fare.chdFare.strikethroughPrice = multiTicket.fare.chdFare.net + ceilingTo5Or10((multiTicket.fare.chdFare.net * multiTicket.discountTag.discountAmount) / 100);
                                                }
                                                else
                                                {
                                                    multiTicket.fare.chdFare.strikethroughPrice = multiTicket.fare.chdFare.net + multiTicket.discountTag.discountAmount;
                                                }
                                            }
                                        }

                                        if (request.noOfInfants > 0)
                                        {
                                            multiTicket.fare.infFare = new PaxPricing();
                                            multiTicket.fare.infFare = getPaxFare(recommendation.PaxFareProduct, "INF,IN", markup, segmentRef == 1 ? multiTicket.Flight_SegRef1 : multiTicket.Flight_SegRef2, namingService, request.userEmail, "O");
                                            if (multiTicket.discountTag != null)
                                            {
                                                if (multiTicket.discountTag.discountIsPercent)
                                                {
                                                    multiTicket.fare.infFare.strikethroughPrice = multiTicket.fare.infFare.net + ceilingTo5Or10((multiTicket.fare.infFare.net * multiTicket.discountTag.discountAmount) / 100);
                                                }
                                                else
                                                {
                                                    multiTicket.fare.infFare.strikethroughPrice = multiTicket.fare.infFare.net + multiTicket.discountTag.discountAmount;
                                                }
                                            }
                                        }
                                        if (multiTicket.discountTag != null)
                                        {
                                            if (multiTicket.discountTag.discountIsPercent)
                                            {
                                                multiTicket.fare.strikethroughTotalPrice = multiTicket.fare.total + ceilingTo5Or10((multiTicket.fare.total * multiTicket.discountTag.discountAmount) / 100);
                                            }
                                            else
                                            {
                                                multiTicket.fare.strikethroughTotalPrice = multiTicket.fare.total + multiTicket.discountTag.discountAmount;
                                            }
                                        }
                                        multiTicket.fareRule = getFareRule(recommendation.PaxFareProduct);
                                        if (recommendation.FareFamilyRef != null)
                                        {
                                            multiTicket.FamilyInformation = GetFamilyInformation(recommendation.FareFamilyRef.ReferencingDetail, response.FareMasterPricerTravelBoardSearchReply.FamilyInformation, response.FareMasterPricerTravelBoardSearchReply.ServiceFeesGrp);
                                        }
                                        multiTicketFlightList.Add(multiTicket);
                                    }//if (GetAllFlight != null && GetAllFlight.Count > 0)
                                }
                            }//foreach

                        }//if (recommendations.Count <= 0)
                    }// if (response == null || response.FareMasterPricerTravelBoardSearchReply == null)
                }//if (String.IsNullOrEmpty(json) == false)
            } //if bAllowByAirlineControl

            List<string> airlines;

            if (multiTicketFlightList != null && multiTicketFlightList.Count > 0)
            {
                Log.Debug("multiTicketFlightList.Count=" + multiTicketFlightList.Count);
                //SetupScale
                int maxSegRef1Time = multiTicketFlightList.Max(x => x.maxSegRef1Time);
                int maxSegRef2Time = multiTicketFlightList.Max(x => x.maxSegRef2Time);
                foreach (var mp in multiTicketFlightList)
                {

                    mp.UIScale(maxSegRef1Time, maxSegRef2Time);
                }
                multiTicketFlightList = multiTicketFlightList.OrderBy(x => x.fare.adtFare.net).ToList();
                result.flights = multiTicketFlightList;

                //find filter
                result.filter = new FlightFilter();

                airlines = result.flights.Select(x => x.mainAirline.code).Distinct().ToList();
                result.filter.airlines = new List<Airline>();
                //NamingServices namingService = new NamingServices(_unitOfWork);
                foreach (var airline in airlines)
                {
                    Airline a = new Airline(namingService, request.languageCode);
                    a.code = airline;
                    result.filter.airlines.Add(a);
                    Log.Debug(a.code + "-" + a.name);
                }
                result.filter.airlines.Sort((x, y) => x.name.CompareTo(y.name));


                result.filter.maxDepDuration = maxSegRef1Time;
                result.filter.maxRetDuration = maxSegRef2Time;
                result.filter.maxPrice = result.flights.Max(x => x.fare.adtFare.net);
                result.filter.minPrice = result.flights.FindAll(x => x.isMultiTicket == false).Min(x => x.fare.adtFare.net);
                result.filter.departureAirport = new List<Airport>();
                result.filter.arrivalAirport = new List<Airport>();
                if (request.flights.Count > 1)
                {
                    result.filter.minPriceDepart = result.flights.FindAll(x => x.isMultiTicket == true && x.Flight_SegRef1 != null).Min(x => x.fare.adtFare.net);
                    result.filter.minPriceReturn = result.flights.FindAll(x => x.isMultiTicket == true && x.Flight_SegRef2 != null).Min(x => x.fare.adtFare.net);
                    result.filter.maxStop = result.flights.FindAll(x => x.isMultiTicket == true && x.Flight_SegRef1 != null).Max(x => x.Flight_SegRef1.Max(y => y.flightDetails.Count - 1));
                    foreach (var filterFlight in result.flights.FindAll(x => x.isMultiTicket == true && x.Flight_SegRef1 != null))
                    {
                        string fDepCode = filterFlight.Flight_SegRef1[0].flightDetails[0].depCity.code;
                        string fArrCode = filterFlight.Flight_SegRef1[0].flightDetails[filterFlight.Flight_SegRef1[0].flightDetails.Count - 1].arrCity.code;
                        if (result.filter.departureAirport.FirstOrDefault(x => x.code == fDepCode) == null)
                        {
                            Airport a = new Airport(namingService, false, request.languageCode);
                            a.code = fDepCode;
                            result.filter.departureAirport.Add(a);
                        }
                        if (result.filter.arrivalAirport.FirstOrDefault(x => x.code == fArrCode) == null)
                        {
                            Airport a = new Airport(namingService, false, request.languageCode);
                            a.code = fArrCode;
                            result.filter.arrivalAirport.Add(a);
                        }
                    }
                }
                else
                {
                    result.filter.maxStop = result.flights.Max(x => x.Flight_SegRef1.Max(y => y.flightDetails.Count - 1));
                    foreach (var filterFlight in result.flights.FindAll(x => x.Flight_SegRef1 != null))
                    {
                        string fDepCode = filterFlight.Flight_SegRef1[0].flightDetails[0].depCity.code;
                        string fArrCode = filterFlight.Flight_SegRef1[0].flightDetails[filterFlight.Flight_SegRef1[0].flightDetails.Count - 1].arrCity.code;
                        if (result.filter.departureAirport.FirstOrDefault(x => x.code == fDepCode) == null)
                        {
                            Airport a = new Airport(namingService, false, request.languageCode);
                            a.code = fDepCode;
                            result.filter.departureAirport.Add(a);
                        }
                        if (result.filter.arrivalAirport.FirstOrDefault(x => x.code == fArrCode) == null)
                        {
                            Airport a = new Airport(namingService, false, request.languageCode);
                            a.code = fArrCode;
                            result.filter.arrivalAirport.Add(a);
                        }
                    }
                }


            }
            else
            {
                return null;
            }


            result.pgSearchOID = request.pgSearchOID;
            Log.Debug("FlightSearchMultiTicket");
            FlightSearchMultiTicket searchMultiTicket = new FlightSearchMultiTicket();
            searchMultiTicket.FlightSearchOID = result.pgSearchOID;
            searchMultiTicket.FlightSearchRequest = JsonConvert.SerializeObject(request);
            searchMultiTicket.FlightSearchResponse = result.toJsonV2();//JsonConvert.SerializeObject(result);
            searchMultiTicket.DateTimeRequest = DateTime.Now;
            Log.Debug("Insert FlightSearchMultiTicket");
            SaveToFlightSearchMultiTicket(searchMultiTicket);
            Log.Debug("END Insert FlightSearchMultiTicket");
            Log.Info("Convert to our object End");
            return result;
        }

        public FlightSearchResult CalendarSearch(Entities.MasterCalendar.Request request)
        {
            FlightSearchResult result = new FlightSearchResult();
            MarkupServices markupSvc = new MarkupServices(_unitOfWork);
            FareMarkup markup = markupSvc.GetMarkup();
            NamingServices namingService = new NamingServices(_unitOfWork);
            AirlineControlServices airlineControlSvc = new AirlineControlServices(_unitOfWork);

            var controlByRoute = airlineControlSvc.GetAll();
            string depCountry = namingService.GetCountryCode(request.flights[0].departureCity.ToUpper());
            string arrCountry = namingService.GetCountryCode(request.flights[0].arrivalCity.ToUpper());
            bool isDomestic = (depCountry == "TH" && arrCountry == "TH");
            Log.Debug("isDomestic=" + isDomestic);
            bool bAllowByAirlineControl = false;
            AirlineControl airlineControl = null;
            //keep search by airline from search box
            List<string> airlineFromSearh = request.airlineCode;

            var chkAllowRoute = controlByRoute.FindAll(x => (x.OriginalCountryCode == "*" || x.OriginalCountryCode == depCountry) && (x.DestinationCountryCode == "*" || x.DestinationCountryCode == arrCountry));
            if (chkAllowRoute != null && chkAllowRoute.Count > 0)
            {
                airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == depCountry && x.DestinationCountryCode == arrCountry);
                if (airlineControl != null)
                {
                    bAllowByAirlineControl = airlineControl.IsActive.Value;
                }
                else
                {
                    airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == depCountry && x.DestinationCountryCode == "*");
                    if (airlineControl != null)
                    {
                        bAllowByAirlineControl = airlineControl.IsActive.Value;
                    }
                    else
                    {
                        airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == "*" && x.DestinationCountryCode == arrCountry);
                        if (airlineControl != null)
                        {
                            bAllowByAirlineControl = airlineControl.IsActive.Value;
                        }
                        else
                        {
                            airlineControl = chkAllowRoute.Find(x => x.OriginalCountryCode == "*" && x.DestinationCountryCode == "*");
                            if (airlineControl != null)
                            {
                                bAllowByAirlineControl = airlineControl.IsActive.Value;
                            }
                        }
                    }
                }

            }
            Log.Debug("bAllowByAirlineControl=" + bAllowByAirlineControl);
            List<AirlineControlSub> airlineControlSub = null;
            List<MasterFlight> masterFlightList = new List<MasterFlight>();
            Entities.MasterPricer.Response response = new Entities.MasterPricer.Response();
            List<Recommendation> recommendations = null;
            string url = "";
            string requestJson = "";
            string json = "";
            if (bAllowByAirlineControl)
            {
                airlineControlSub = airlineControlSvc.GetAllAirlineSub(airlineControl.AirlineOID);
                if (airlineControl != null && request.airlineCode == null)
                {

                    if (airlineControlSub != null && airlineControlSub.Count > 0)
                    {
                        var airlineControlSubL = airlineControlSub.FindAll(x => x.AirlineCode != "YY" && x.IsActive == true && x.FareBasis == "*" && x.ClassOfService == "*");
                        if (airlineControlSubL != null && airlineControlSubL.Count > 0)
                        {
                            request.airlineCode = new List<string>();
                            foreach (var airlineL in airlineControlSubL)
                            {
                                foreach (var airline in airlineL.AirlineCode.Split(','))
                                {
                                    request.airlineCode.Add(airline);
                                }
                            }
                        }
                    }
                }
                request.inFlightServices = 1;
                request.noOfRecommendation = 250;

                url = ConfigurationManager.AppSettings["MASTERCALENDAR.URL"];
                requestJson = JsonConvert.SerializeObject(request);


                Log.Info("Search Start");
                json = HttpUtility.postJSON(url, requestJson);
                Log.Debug(requestJson);
                Log.Debug(json);
                Log.Info("Search End");
                //return airlineFromSearh to request
                request.airlineCode = airlineFromSearh;

                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");
                    response = BL.Entities.MasterPricer.Response.FromJson(json);

                    recommendations = new List<Recommendation>();
                    if (response == null || response.FareMasterPricerTravelBoardSearchReply == null)
                    {
                        Log.Debug(requestJson);
                        Log.Debug(json);
                        //return null; //comment for search flight from kiwi on 2021-06-29 by NING
                    }
                    else
                    {

                        if (response.FareMasterPricerTravelBoardSearchReply.Recommendation.PurpleRecommendation != null)
                        {
                            recommendations.Add(response.FareMasterPricerTravelBoardSearchReply.Recommendation.PurpleRecommendation);
                        }
                        else
                        {
                            recommendations = response.FareMasterPricerTravelBoardSearchReply.Recommendation.RecommendationElementArray;
                        }
                        if (recommendations.Count <= 0)
                        {
                            Log.Debug(requestJson);
                            Log.Debug(json);
                            //return null; //comment for search flight from kiwi on 2021-06-29 by NING
                        }
                        else
                        {
                            //Log.Debug(requestJson);
                            //Log.Debug(json);

                            foreach (var recommendation in recommendations)
                            {
                                MasterFlight masterFlight = new MasterFlight();
                                masterFlight.type = "A";//A=Amadeus,K=Kiwi
                                masterFlight.id = Guid.NewGuid();
                                var depFlight = response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex == null ?
                                    response.FareMasterPricerTravelBoardSearchReply.FlightIndex.FlightIndexElementArray[0].GroupOfFlights :
                                 response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex.GroupOfFlights;

                                List<GroupOfFlight> gofDep = new List<GroupOfFlight>();
                                if (depFlight.PurpleGroupOfFlight != null)
                                {
                                    gofDep.Add(depFlight.PurpleGroupOfFlight);
                                }
                                else
                                {
                                    gofDep = depFlight.GroupOfFlightElementArray;
                                }
                                masterFlight.departureFlight = GetFlight(gofDep, recommendation.PaxFareProduct, "ADT", recommendation.SegmentFlightRef, request.languageCode, request.flights[0].departureDateTime, 1, depCountry, arrCountry, airlineControlSub, response.FareMasterPricerTravelBoardSearchReply.ServiceFeesGrp, request.pgSearchOID);
                                if (masterFlight.departureFlight.Count > 0)
                                {
                                    if (request.flights.Count > 1)
                                    {
                                        var retFlight = response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex == null ?
                                            response.FareMasterPricerTravelBoardSearchReply.FlightIndex.FlightIndexElementArray[1].GroupOfFlights :
                                     response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex.GroupOfFlights;

                                        List<GroupOfFlight> gofRet = new List<GroupOfFlight>();
                                        if (retFlight.PurpleGroupOfFlight != null)
                                        {
                                            gofRet.Add(retFlight.PurpleGroupOfFlight);
                                        }
                                        else
                                        {
                                            gofRet = retFlight.GroupOfFlightElementArray;
                                        }
                                        masterFlight.returnFlight = GetFlight(gofRet, recommendation.PaxFareProduct, "ADT", recommendation.SegmentFlightRef, request.languageCode, request.flights[1].departureDateTime, 2, depCountry, arrCountry, airlineControlSub, response.FareMasterPricerTravelBoardSearchReply.ServiceFeesGrp, request.pgSearchOID);

                                    }
                                    if (request.flights.Count == 1 || (request.flights.Count > 1 && masterFlight.returnFlight.Count > 0))
                                    {
                                        masterFlight.ComputeConnectionTime();

                                        masterFlight.fare = new Pricing(request.noOfAdults, request.noOfChildren, request.noOfInfants);

                                        masterFlight.fare.currencyCode = response.FareMasterPricerTravelBoardSearchReply.ConversionRate.ConversionRateDetail.Currency;

                                        masterFlight.fare.adtFare = new PaxPricing();
                                        masterFlight.fare.adtFare = getPaxFare(recommendation.PaxFareProduct, "ADT", markup, masterFlight.departureFlight, namingService, request.userEmail, (request.flights.Count > 1 && masterFlight.returnFlight.Count > 0 ? "R" : "O"));

                                        if (request.noOfChildren > 0)
                                        {
                                            masterFlight.fare.chdFare = new PaxPricing();
                                            masterFlight.fare.chdFare = getPaxFare(recommendation.PaxFareProduct, "CHD,CH", markup, masterFlight.departureFlight, namingService, request.userEmail, (request.flights.Count > 1 && masterFlight.returnFlight.Count > 0 ? "R" : "O"));
                                        }

                                        if (request.noOfInfants > 0)
                                        {
                                            masterFlight.fare.infFare = new PaxPricing();
                                            masterFlight.fare.infFare = getPaxFare(recommendation.PaxFareProduct, "INF,IN", markup, masterFlight.departureFlight, namingService, request.userEmail, (request.flights.Count > 1 && masterFlight.returnFlight.Count > 0 ? "R" : "O"));
                                        }
                                        masterFlight.fareRule = getFareRule(recommendation.PaxFareProduct);

                                        masterFlightList.Add(masterFlight);
                                    }
                                }
                            }
                        }//if (recommendations.Count <= 0)
                    }// if (response == null || response.FareMasterPricerTravelBoardSearchReply == null)
                }//if (String.IsNullOrEmpty(json) == false)
            } //if bAllowByAirlineControl

            //SetupScale
            int maxDepTime = masterFlightList.Max(x => x.maxDepTime);
            int maxRetTime = masterFlightList.Max(x => x.maxRetTime);
            foreach (var mp in masterFlightList)
            {
                mp.UIScale(maxDepTime, maxRetTime);
            }
            masterFlightList = masterFlightList.OrderBy(x => x.fare.adtFare.net).ToList();
            result.flights = masterFlightList;

            //find filter
            result.filter = new FlightFilter();

            List<string> airlines = result.flights.Select(x => x.mainAirline.code).Distinct().ToList();
            result.filter.airlines = new List<Airline>();
            //NamingServices namingService = new NamingServices(_unitOfWork);
            foreach (var airline in airlines)
            {
                Airline a = new Airline(namingService, request.languageCode);
                a.code = airline;
                result.filter.airlines.Add(a);
            }
            result.filter.airlines.Sort((x, y) => x.name.CompareTo(y.name));

            result.filter.maxDepDuration = maxDepTime;
            result.filter.maxRetDuration = maxRetTime;
            result.filter.maxPrice = result.flights.Max(x => x.fare.adtFare.net);
            result.filter.maxStop = result.flights.Max(x => x.departureFlight.Max(y => y.flightDetails.Count - 1));

            result.filter.departureAirport = new List<Airport>();
            result.filter.arrivalAirport = new List<Airport>();
            foreach (var filterFlight in result.flights)
            {
                string fDepCode = filterFlight.departureFlight[0].flightDetails[0].depCity.code;
                string fArrCode = filterFlight.departureFlight[0].flightDetails[filterFlight.departureFlight[0].flightDetails.Count - 1].arrCity.code;
                if (result.filter.departureAirport.FirstOrDefault(x => x.code == fDepCode) == null)
                {
                    Airport a = new Airport(namingService, false, request.languageCode);
                    a.code = fDepCode;
                    result.filter.departureAirport.Add(a);
                }
                if (result.filter.arrivalAirport.FirstOrDefault(x => x.code == fArrCode) == null)
                {
                    Airport a = new Airport(namingService, false, request.languageCode);
                    a.code = fArrCode;
                    result.filter.arrivalAirport.Add(a);
                }
            }

            result.pgSearchOID = request.pgSearchOID;

            Log.Info("Convert to our object End");
            return result;
        }

        public Entities.AirMultiAvailability.FlightSearchANResult SearchAN(Entities.AirMultiAvailability.Request request)
        {
            int iMovedown = int.Parse(ConfigurationManager.AppSettings["NO_OF_MOVEDOWN_AN"]);
            Entities.AirMultiAvailability.FlightSearchANResult result = new Entities.AirMultiAvailability.FlightSearchANResult();
            Entities.AirMultiAvailability.Response response = null;
            string url = ConfigurationManager.AppSettings["AIRMULTIAVAILABILITY.URL"];
            request.anRoute = "DEP";
            Entities.AirMultiAvailability.Session depSession = new Entities.AirMultiAvailability.Session();
            depSession.IsStateFull = true;
            depSession.InSeries = false;
            depSession.End = false;
            depSession.SessionId = "";
            depSession.SequenceNumber = "";
            depSession.SecurityToken = "";
            request.Session = depSession;

            int iSequenceNumber = 0;
            string sSessionId = "";
            string sSecurityToken = "";
            string requestJson = JsonConvert.SerializeObject(request);
            Log.Info("Search DEP Start");
            string json = HttpUtility.postJSON(url, requestJson);
            Log.Debug(requestJson);
            Log.Debug(json);
            Log.Info("Search DEP End");

            if (String.IsNullOrEmpty(json) == false)
            {
                result = new Entities.AirMultiAvailability.FlightSearchANResult();
                List<Entities.AirMultiAvailability.FlightAN> departureFlight = null;
                List<Entities.AirMultiAvailability.FlightAN> returnFlight = null;
                json = json.Replace("@", "");
                response = Entities.AirMultiAvailability.Response.FromJson(json);
                Entities.AirMultiAvailability.MovedownRequest movedownRequest = new Entities.AirMultiAvailability.MovedownRequest();
                if (response != null && response.AirMultiAvailabilityReply != null
                && ((response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos != null
                && response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfos != null || response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfosArray != null)
                || response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfosArray != null))
                {
                    //departureFlight
                    departureFlight = GetFlightAN(response, request.departureDateTime, request.languageCode, departureFlight);
                    for (int i = 0; i < iMovedown; i++)
                    {
                        sSessionId = response.AirMultiAvailabilityReply.Session.SessionId;
                        iSequenceNumber = response.AirMultiAvailabilityReply.Session.SequenceNumber;
                        sSecurityToken = response.AirMultiAvailabilityReply.Session.SecurityToken;
                        Log.Debug("sSessionId=" + sSessionId);
                        Log.Debug("iSequenceNumber=" + iSequenceNumber);
                        Log.Debug("sSecurityToken=" + sSecurityToken);
                        url = ConfigurationManager.AppSettings["AIRMULTIAVAILABILITYMOVEDOWN.URL"];
                        movedownRequest = new Entities.AirMultiAvailability.MovedownRequest();
                        movedownRequest.departureDateTime = request.departureDateTime;
                        movedownRequest.returnDateTime = request.tripType.Equals("R") ? request.returnDateTime : request.departureDateTime;
                        movedownRequest.Session = new Entities.AirMultiAvailability.Session();
                        movedownRequest.Session.IsStateFull = true;
                        movedownRequest.Session.InSeries = true;
                        movedownRequest.Session.SessionId = sSessionId;
                        movedownRequest.Session.SequenceNumber = (iSequenceNumber + 1).ToString();
                        movedownRequest.Session.SecurityToken = sSecurityToken;
                        requestJson = JsonConvert.SerializeObject(movedownRequest);
                        Log.Info("Search DEP MOVEDOWN Start");
                        json = HttpUtility.postJSON(url, requestJson);
                        Log.Debug(requestJson);
                        Log.Debug(json);
                        Log.Info("Search DEP MOVEDOWN End");
                        if (String.IsNullOrEmpty(json) == false)
                        {
                            json = json.Replace("@", "");
                            response = Entities.AirMultiAvailability.Response.FromJson(json);
                            if (response != null && response.AirMultiAvailabilityReply != null
                            && ((response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos != null
                            && response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfos != null || response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfosArray != null)
                            || response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfosArray != null))
                            {
                                departureFlight = GetFlightAN(response, request.departureDateTime, request.languageCode, departureFlight);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }


                    //returnFlight
                    if (request.tripType.Equals("R"))
                    {
                        url = ConfigurationManager.AppSettings["AIRMULTIAVAILABILITY.URL"];
                        request.anRoute = "RET";
                        Entities.AirMultiAvailability.Session retSession = new Entities.AirMultiAvailability.Session();
                        retSession.IsStateFull = true;
                        retSession.InSeries = false;
                        retSession.End = false;
                        retSession.SessionId = "";
                        retSession.SequenceNumber = "";
                        retSession.SecurityToken = "";
                        request.Session = retSession;
                        requestJson = JsonConvert.SerializeObject(request);
                        Log.Info("Search RET Start");
                        json = HttpUtility.postJSON(url, requestJson);
                        Log.Debug(requestJson);
                        Log.Debug(json);
                        Log.Info("Search RET End");

                        if (String.IsNullOrEmpty(json) == false)
                        {
                            json = json.Replace("@", "");
                            response = Entities.AirMultiAvailability.Response.FromJson(json);
                            if (response != null && response.AirMultiAvailabilityReply != null
                            && ((response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos != null
                            && response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfos != null || response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfosArray != null)
                            || response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfosArray != null))
                            {
                                returnFlight = GetFlightAN(response, request.returnDateTime, request.languageCode, returnFlight);
                                for (int i = 0; i < iMovedown; i++)
                                {
                                    sSessionId = response.AirMultiAvailabilityReply.Session.SessionId;
                                    iSequenceNumber = response.AirMultiAvailabilityReply.Session.SequenceNumber;
                                    sSecurityToken = response.AirMultiAvailabilityReply.Session.SecurityToken;
                                    Log.Debug("sSessionId=" + sSessionId);
                                    Log.Debug("iSequenceNumber=" + iSequenceNumber);
                                    Log.Debug("sSecurityToken=" + sSecurityToken);
                                    url = ConfigurationManager.AppSettings["AIRMULTIAVAILABILITYMOVEDOWN.URL"];
                                    movedownRequest = new Entities.AirMultiAvailability.MovedownRequest();
                                    movedownRequest.departureDateTime = request.departureDateTime;
                                    movedownRequest.returnDateTime = request.returnDateTime;
                                    movedownRequest.Session = new Entities.AirMultiAvailability.Session();
                                    movedownRequest.Session.IsStateFull = true;
                                    movedownRequest.Session.InSeries = true;
                                    movedownRequest.Session.SessionId = sSessionId;
                                    movedownRequest.Session.SequenceNumber = (iSequenceNumber + 1).ToString();
                                    movedownRequest.Session.SecurityToken = sSecurityToken;
                                    requestJson = JsonConvert.SerializeObject(movedownRequest);
                                    Log.Info("Search RET MOVEDOWN Start");
                                    json = HttpUtility.postJSON(url, requestJson);
                                    Log.Debug(requestJson);
                                    Log.Debug(json);
                                    Log.Info("Search RET MOVEDOWN End");
                                    if (String.IsNullOrEmpty(json) == false)
                                    {
                                        json = json.Replace("@", "");
                                        response = Entities.AirMultiAvailability.Response.FromJson(json);
                                        if (response != null && response.AirMultiAvailabilityReply != null
                                        && ((response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos != null
                                        && response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfos != null || response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfosArray != null)
                                        || response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfosArray != null))
                                        {
                                            returnFlight = GetFlightAN(response, request.returnDateTime, request.languageCode, returnFlight);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                result.pgSearchOID = request.pgSearchOID;
                result.isError = false;
                if (departureFlight == null || departureFlight != null && departureFlight.Count == 0)
                {
                    result.isError = true;
                    result.errorMessage = "departureFlight not found.";
                }
                else
                {
                    result.departureFlight = departureFlight;
                    if (request.tripType.Equals("R"))
                    {
                        if (returnFlight != null && returnFlight.Count > 0)
                        {
                            result.returnFlight = returnFlight;
                        }
                        else
                        {
                            result.isError = true;
                            result.errorMessage = "returnFlight not found.";
                        }
                    }
                }
            }
            else
            {
                result = new Entities.AirMultiAvailability.FlightSearchANResult();
                result.isError = true;
                result.pgSearchOID = request.pgSearchOID;
                result.errorMessage = "";
            }

            return result;
        }

        private List<Entities.AirMultiAvailability.FlightAN> GetFlightAN(Entities.AirMultiAvailability.Response response, DateTime depDate, string languageCode, List<Entities.AirMultiAvailability.FlightAN> flightList)
        {
            NamingServices namingService = new NamingServices(_unitOfWork);

            int iSegment = 0;
            string localtionOrigin = "";
            string localtionDestination = "";
            Entities.AirMultiAvailability.FlightAN flight = new Entities.AirMultiAvailability.FlightAN();
            Entities.AirMultiAvailability.ProductClassInfo classInfo = new Entities.AirMultiAvailability.ProductClassInfo();
            if (response != null && response.AirMultiAvailabilityReply != null
            && (response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos != null || response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfosArray != null))
            {
                if (flightList == null)
                {
                    flightList = new List<Entities.AirMultiAvailability.FlightAN>();
                }
                if (response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos != null)
                {
                    Entities.AirMultiAvailability.FlightInfos infos = new Entities.AirMultiAvailability.FlightInfos();
                    Entities.AirMultiAvailability.FlightDetailAN details = new Entities.AirMultiAvailability.FlightDetailAN();
                    if (response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfos != null)
                    {
                        flight = new Entities.AirMultiAvailability.FlightAN();
                        infos = response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfos;
                        if (infos.BasicFlightInfo.DepartureLocation.CityAirport == localtionOrigin)
                        {
                            iSegment = 1;
                            flight = new Entities.AirMultiAvailability.FlightAN();
                            flight.flightDetails = new List<Entities.AirMultiAvailability.FlightDetailAN>();
                        }
                        details = new Entities.AirMultiAvailability.FlightDetailAN();
                        details.airline = new Airline(namingService, languageCode);
                        details.airline.code = infos.BasicFlightInfo.MarketingCompany.Identifier;
                        details.operatedAirline = new Airline(namingService, languageCode);
                        details.operatedAirline.code = infos.BasicFlightInfo.OperatingCompany != null ? infos.BasicFlightInfo.OperatingCompany.Identifier : infos.BasicFlightInfo.MarketingCompany.Identifier;
                        details.depCity = new Airport(namingService, true, languageCode);
                        details.depCity.code = infos.BasicFlightInfo.DepartureLocation.CityAirport;
                        details.arrCity = new Airport(namingService, true, languageCode);
                        details.arrCity.code = infos.BasicFlightInfo.ArrivalLocation.CityAirport;
                        details.flightNumber = infos.BasicFlightInfo.FlightIdentification.Number.ToString();
                        details.typeOfAircraft = infos.AdditionalFlightInfo.FlightDetails.TypeOfAircraft;
                        details.departureDateTime = DateTime.ParseExact(infos.BasicFlightInfo.FlightDetails.DepartureDate + infos.BasicFlightInfo.FlightDetails.DepartureTime
                , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        details.arrivalDateTime = DateTime.ParseExact(infos.BasicFlightInfo.FlightDetails.ArrivalDate + infos.BasicFlightInfo.FlightDetails.ArrivalTime
                , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        details.Seq = iSegment;
                        details.setDisplayDateTime(languageCode, depDate);
                        details.productInfo = new List<Entities.AirMultiAvailability.ProductClassInfo>();
                        if (infos.InfoOnClasses.InfoOnClassArray != null)
                        {
                            foreach (Entities.AirMultiAvailability.InfoOnClass infoClass in infos.InfoOnClasses.InfoOnClassArray)
                            {
                                classInfo = new Entities.AirMultiAvailability.ProductClassInfo();
                                classInfo.rbd = infoClass.ProductClassDetail.ServiceClass;
                                classInfo.availabilityStatus = infoClass.ProductClassDetail.AvailabilityStatus;
                                details.productInfo.Add(classInfo);
                            }
                        }
                        else
                        {
                            classInfo = new Entities.AirMultiAvailability.ProductClassInfo();
                            classInfo.rbd = infos.InfoOnClasses.InfoOnClass.ProductClassDetail.ServiceClass;
                            classInfo.availabilityStatus = infos.InfoOnClasses.InfoOnClass.ProductClassDetail.AvailabilityStatus;
                            details.productInfo.Add(classInfo);
                        }
                        flight.flightDetails.Add(details);
                        if (infos.BasicFlightInfo.ArrivalLocation.CityAirport == localtionDestination)
                        {
                            flight.totalTime = infos.AdditionalFlightInfo.FlightDetails.LegDuration;
                            flightList.Add(flight);
                        }
                    }
                    else//response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfosArray
                    {

                        localtionOrigin = response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.LocationDetails.Origin;
                        localtionDestination = response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.LocationDetails.Destination;
                        for (int i = 0; i < response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfosArray.Count; i++)
                        {
                            infos = response.AirMultiAvailabilityReply.SingleCityPairInfo.SingleCityPairInfos.FlightInfo.FlightInfosArray[i];

                            if (infos.BasicFlightInfo.DepartureLocation.CityAirport == localtionOrigin)
                            {
                                iSegment = 1;
                                flight = new Entities.AirMultiAvailability.FlightAN();
                                flight.flightDetails = new List<Entities.AirMultiAvailability.FlightDetailAN>();
                            }
                            details = new Entities.AirMultiAvailability.FlightDetailAN();
                            details.airline = new Airline(namingService, languageCode);
                            details.airline.code = infos.BasicFlightInfo.MarketingCompany.Identifier;
                            details.operatedAirline = new Airline(namingService, languageCode);
                            details.operatedAirline.code = infos.BasicFlightInfo.OperatingCompany != null ? infos.BasicFlightInfo.OperatingCompany.Identifier : infos.BasicFlightInfo.MarketingCompany.Identifier;
                            details.depCity = new Airport(namingService, true, languageCode);
                            details.depCity.code = infos.BasicFlightInfo.DepartureLocation.CityAirport;
                            details.arrCity = new Airport(namingService, true, languageCode);
                            details.arrCity.code = infos.BasicFlightInfo.ArrivalLocation.CityAirport;
                            details.flightNumber = infos.BasicFlightInfo.FlightIdentification.Number.ToString();
                            details.typeOfAircraft = infos.AdditionalFlightInfo.FlightDetails.TypeOfAircraft;
                            details.departureDateTime = DateTime.ParseExact(infos.BasicFlightInfo.FlightDetails.DepartureDate + infos.BasicFlightInfo.FlightDetails.DepartureTime
                    , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                            details.arrivalDateTime = DateTime.ParseExact(infos.BasicFlightInfo.FlightDetails.ArrivalDate + infos.BasicFlightInfo.FlightDetails.ArrivalTime
                    , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                            details.Seq = iSegment;
                            details.setDisplayDateTime(languageCode, depDate);
                            details.productInfo = new List<Entities.AirMultiAvailability.ProductClassInfo>();
                            if (infos.InfoOnClasses.InfoOnClassArray != null)
                            {
                                foreach (Entities.AirMultiAvailability.InfoOnClass infoClass in infos.InfoOnClasses.InfoOnClassArray)
                                {
                                    classInfo = new Entities.AirMultiAvailability.ProductClassInfo();
                                    classInfo.rbd = infoClass.ProductClassDetail.ServiceClass;
                                    classInfo.availabilityStatus = infoClass.ProductClassDetail.AvailabilityStatus;
                                    details.productInfo.Add(classInfo);
                                }
                            }
                            else
                            {
                                classInfo = new Entities.AirMultiAvailability.ProductClassInfo();
                                classInfo.rbd = infos.InfoOnClasses.InfoOnClass.ProductClassDetail.ServiceClass;
                                classInfo.availabilityStatus = infos.InfoOnClasses.InfoOnClass.ProductClassDetail.AvailabilityStatus;
                                details.productInfo.Add(classInfo);
                            }
                            flight.flightDetails.Add(details);
                            if (infos.BasicFlightInfo.ArrivalLocation.CityAirport == localtionDestination)
                            {
                                flight.totalTime = infos.AdditionalFlightInfo.FlightDetails.LegDuration;
                                flightList.Add(flight);
                            }
                            iSegment++;
                        }
                    }
                }
            }
            return flightList;
        }

        private List<string> getFareRule(PaxFareProductUnion paxFareProduct)
        {
            List<string> fareRule = new List<string>();
            if (paxFareProduct.PurplePaxFareProduct != null)
            {
                if (paxFareProduct.PurplePaxFareProduct.Fare.PurpleFare != null)
                {
                    fareRule.Add(String.Join(" ", paxFareProduct.PurplePaxFareProduct.Fare.PurpleFare.PricingMessage.Description.ToArray()));
                }
                else
                {
                    foreach (var fare in paxFareProduct.PurplePaxFareProduct.Fare.FareElementArray)
                    {
                        if (fare.PricingMessage.Description.String != null)
                        {
                            fareRule.Add(fare.PricingMessage.Description.String);
                        }
                        else
                        {
                            fareRule.Add(String.Join(" ", fare.PricingMessage.Description.StringArray.ToArray()));
                        }
                    }
                }
            }
            else
            {
                foreach (var paxProduct in paxFareProduct.PaxFareProductElementArray)
                {
                    if (paxProduct.Fare.PurpleFare != null)
                    {
                        fareRule.Add(String.Join(" ", paxProduct.Fare.PurpleFare.PricingMessage.Description.ToArray()));
                    }
                    else
                    {
                        foreach (var fare in paxProduct.Fare.FareElementArray)
                        {
                            if (fare.PricingMessage.Description.String != null)
                            {
                                fareRule.Add(fare.PricingMessage.Description.String);
                            }
                            else
                            {
                                fareRule.Add(String.Join(" ", fare.PricingMessage.Description.StringArray.ToArray()));
                            }
                        }
                    }
                }
            }
            return fareRule;
        }

        private List<Entities.RobinhoodFlight.Flight> GetFlight(List<GroupOfFlight> groupFlight,
            PaxFareProductUnion paxFareProduct, string paxType, SegmentFlightRefUnion segmentRef,
            string languageCode, DateTime depDate, int segmentNo, string OrginalCountryCode, string DestinationCountryCode, List<AirlineControlSub> airlineControls, ServiceFeesGrpUnion serviceFeesGrp
            , Guid pgSearchOID)
        {
            NamingServices namingService = new NamingServices(_unitOfWork);
            AirlineControlServices airlineControl = new AirlineControlServices(_unitOfWork);
            bool bAllow = false;
            string[] pax = paxType.Split(',');
            List<string> rbd = new List<string>();
            List<string> fareBasis = new List<string>();
            List<string> avail = new List<string>();
            List<string> fareType = new List<string>();
            List<string> cabin = new List<string>();

            List<Entities.RobinhoodFlight.Flight> flightList = new List<Entities.RobinhoodFlight.Flight>();

            if (paxFareProduct.PaxFareProductElementArray == null)
            {
                var fareDetails = paxFareProduct.PurplePaxFareProduct.FareDetails;
                FareDetailsGroupOfFares fdgf;
                if (fareDetails.FareDetailsElement != null)
                {
                    fdgf = fareDetails.FareDetailsElement.GroupOfFares;
                }
                else
                {
                    fdgf = fareDetails.FareDetailsElementArray.FirstOrDefault(x => x.SegmentRef.SegRef == segmentNo.ToString()).GroupOfFares;
                }

                if (fdgf.FluffyGroupOfFares != null)
                {
                    var product = fdgf.FluffyGroupOfFares.ProductInformation;
                    avail.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.AvlStatus : product.CabinProduct.CabinProductElementArray[1].AvlStatus);
                    cabin.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Cabin : product.CabinProduct.CabinProductElementArray[1].Cabin);
                    rbd.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Rbd : product.CabinProduct.CabinProductElementArray[1].Rbd);
                    fareBasis.Add(product.FareProductDetail.FareBasis);
                    if (product.FareProductDetail.FareType.StringArray != null &&
                        product.FareProductDetail.FareType.StringArray.Count > 0)
                    {
                        fareType.Add(product.FareProductDetail.FareType.StringArray[0]);
                    }
                    else
                    {
                        fareType.Add(product.FareProductDetail.FareType.String);
                    }
                }
                else
                {
                    foreach (var gf in fdgf.FluffyGroupOfFareArray)
                    {
                        var product = gf.ProductInformation;
                        avail.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.AvlStatus : product.CabinProduct.CabinProductElementArray[1].AvlStatus);
                        cabin.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Cabin : product.CabinProduct.CabinProductElementArray[1].Cabin);
                        rbd.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Rbd : product.CabinProduct.CabinProductElementArray[1].Rbd);
                        fareBasis.Add(product.FareProductDetail.FareBasis);
                        fareType.Add(product.FareProductDetail.FareType.String == null ? String.Join(",", product.FareProductDetail.FareType.StringArray) : product.FareProductDetail.FareType.String);
                    }
                }
            }
            else
            {
                var fareDetails = paxFareProduct.PaxFareProductElementArray.FirstOrDefault(x => x.PaxReference.PurplePaxReference != null ? pax.Contains(x.PaxReference.PurplePaxReference.Ptc.ToUpper()) : pax.Contains(x.PaxReference.PaxReferenceElementArray[0].Ptc.ToUpper())).FareDetails;
                TentacledGroupOfFares tgf;
                if (fareDetails.PurpleFareDetail != null)
                {
                    tgf = fareDetails.PurpleFareDetail.GroupOfFares;
                }
                else
                {
                    tgf = fareDetails.FareDetailElementArray.FirstOrDefault(x => x.SegmentRef.SegRef == segmentNo.ToString()).GroupOfFares;
                }

                if (tgf.PurpleGroupOfFares != null)
                {
                    var product = tgf.PurpleGroupOfFares.ProductInformation;
                    avail.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.AvlStatus : product.CabinProduct.CabinProductElementArray[1].AvlStatus);
                    cabin.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Cabin : product.CabinProduct.CabinProductElementArray[1].Cabin);
                    rbd.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Rbd : product.CabinProduct.CabinProductElementArray[1].Rbd);
                    fareBasis.Add(product.FareProductDetail.FareBasis);
                    if (product.FareProductDetail.FareType.StringArray != null &&
                        product.FareProductDetail.FareType.StringArray.Count > 0)
                    {
                        fareType.Add(product.FareProductDetail.FareType.StringArray[0]);
                    }
                    else
                    {
                        fareType.Add(product.FareProductDetail.FareType.String);
                    }
                }
                else
                {
                    foreach (var gf in tgf.PurpleGroupOfFareArray)
                    {
                        var product = gf.ProductInformation;
                        avail.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.AvlStatus : product.CabinProduct.CabinProductElementArray[1].AvlStatus);
                        cabin.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Cabin : product.CabinProduct.CabinProductElementArray[1].Cabin);
                        rbd.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Rbd : product.CabinProduct.CabinProductElementArray[1].Rbd);
                        fareBasis.Add(product.FareProductDetail.FareBasis);
                        fareType.Add(product.FareProductDetail.FareType.String == null ? String.Join(",", product.FareProductDetail.FareType.StringArray) : product.FareProductDetail.FareType.String);
                    }
                }
            }

            List<string> refNo = new List<string>();
            string freeBagAllownceInfoRef = "";
            if (segmentRef.SegmentFlightRefElement != null)
            {
                if (segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElement != null)
                {
                    refNo.Add(segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElement.RefNumber);
                }
                else
                {
                    refNo.Add(segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElementArray[segmentNo - 1].RefNumber);
                    foreach (var refReferencingDetail in segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElementArray)
                    {
                        if (refReferencingDetail.RefQualifier.Equals("B"))
                        {
                            freeBagAllownceInfoRef = refReferencingDetail.RefNumber;
                        }
                    }
                }
            }
            else
            {
                foreach (var refList in segmentRef.SegmentFlightRefElementArray)
                {
                    string sRefNo = "";
                    if (refList.ReferencingDetail.ReferencingDetailElement != null && refList.ReferencingDetail.ReferencingDetailElement.RefQualifier.Equals("S"))
                    {
                        sRefNo = refList.ReferencingDetail.ReferencingDetailElement.RefNumber;
                    }
                    else if (refList.ReferencingDetail.ReferencingDetailElement != null && refList.ReferencingDetail.ReferencingDetailElement.RefQualifier.Equals("B"))
                    {
                        freeBagAllownceInfoRef = refList.ReferencingDetail.ReferencingDetailElement.RefNumber;
                    }
                    else
                    {
                        if (refList.ReferencingDetail.ReferencingDetailElementArray[segmentNo - 1].RefQualifier.Equals("S"))
                        {
                            sRefNo = refList.ReferencingDetail.ReferencingDetailElementArray[segmentNo - 1].RefNumber;
                        }
                        var refDetailBag = refList.ReferencingDetail.ReferencingDetailElementArray.Find(x => x.RefQualifier == "B");
                        if (refDetailBag != null)
                        {
                            freeBagAllownceInfoRef = refDetailBag.RefNumber;
                        }
                    }

                    if (!refNo.Contains(sRefNo))
                    {
                        refNo.Add(sRefNo);
                    }
                }
            }

            //Find flight
            int iSeq = 0;
            foreach (string rN in refNo)
            {
                var flightDetail = groupFlight.FirstOrDefault(x => x.PropFlightGrDetail.FlightProposal[0].Ref == rN);
                Entities.RobinhoodFlight.Flight flight = new Entities.RobinhoodFlight.Flight();
                flight.bShow = true;
                flight.refNumber = rN;
                flight.totalTime = flightDetail.PropFlightGrDetail.FlightProposal.FirstOrDefault(x => x.UnitQualifier == "EFT").Ref;
                flight.pgSearchOID = pgSearchOID.ToString();
                flight.flightFrom = "A";//A:Amadeus,L:LionAir
                if (freeBagAllownceInfoRef.Length > 0)
                {
                    flight.baggageDetails = GetBaggageDetails(freeBagAllownceInfoRef, serviceFeesGrp, segmentNo);
                }
                flight.flightDetails = new List<Entities.RobinhoodFlight.FlightDetail>();
                iSeq = 0;
                if (flightDetail.FlightDetails.FlightDetailsClass != null)
                {
                    //TODO TECHNICAL STOP
                    var flightInfo = flightDetail.FlightDetails.FlightDetailsClass.FlightInformation;
                    var technicalStop = flightDetail.FlightDetails.FlightDetailsClass.TechnicalStop;
                    Entities.RobinhoodFlight.FlightDetail detail = new Entities.RobinhoodFlight.FlightDetail();

                    List<string> inFlightSvc = new List<string>();
                    if (flightDetail.FlightDetails.FlightDetailsClass.FlightCharacteristics != null)
                    {
                        if (flightDetail.FlightDetails.FlightDetailsClass.FlightCharacteristics.InFlightSrv.StringArray != null)
                        {
                            inFlightSvc = flightDetail.FlightDetails.FlightDetailsClass.FlightCharacteristics.InFlightSrv.StringArray;
                        }
                        else
                        {
                            inFlightSvc.Add(flightDetail.FlightDetails.FlightDetailsClass.FlightCharacteristics.InFlightSrv.String);
                        }
                    }
                    detail.inFlightServices = new InFlightServices();
                    if (inFlightSvc.Contains("7"))
                    {
                        detail.inFlightServices.dutyFreeSale = true;
                    }
                    if (inFlightSvc.Contains("1") || inFlightSvc.Contains("4") || inFlightSvc.Contains("5") || inFlightSvc.Contains("10") || inFlightSvc.Contains("15"))
                    {
                        detail.inFlightServices.inflightEntertain = true;
                    }
                    if (inFlightSvc.Contains("12"))
                    {
                        detail.inFlightServices.inSeatPower = true;
                    }
                    if (inFlightSvc.Contains("13") || inFlightSvc.Contains("18"))
                    {
                        detail.inFlightServices.wifiInternet = true;
                    }

                    detail.airline = new Airline(namingService, languageCode);
                    detail.airline.code = flightInfo.CompanyId.MarketingCarrier;
                    detail.operatedAirline = new Airline(namingService, languageCode);
                    if (flightInfo.CompanyId.OperatingCarrier != null)
                    {
                        detail.operatedAirline.code = flightInfo.CompanyId.OperatingCarrier;
                    }
                    else
                    {
                        detail.operatedAirline.code = detail.airline.code;
                    }
                    detail.flightNumber = flightInfo.FlightNumber;
                    detail.depCity = new Airport(namingService, true, languageCode);
                    detail.depCity.code = flightInfo.Location[0].LocationId;
                    detail.depCity.terminal = flightInfo.Location[0].Terminal;
                    detail.arrCity = new Airport(namingService, true, languageCode);
                    if (technicalStop != null && technicalStop.StopDetails != null && technicalStop.StopDetails.Count > 1)
                    {
                        detail.arrCity.code = technicalStop.StopDetails[0].LocationId;
                    }
                    else
                    {
                        detail.arrCity.code = flightInfo.Location[1].LocationId;
                    }
                    detail.arrCity.terminal = flightInfo.Location[1].Terminal;

                    detail.departureDateTime = DateTime.ParseExact(flightInfo.ProductDateTime.DateOfDeparture + flightInfo.ProductDateTime.TimeOfDeparture
                        , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    if (technicalStop != null && technicalStop.StopDetails != null && technicalStop.StopDetails.Count > 1)
                    {
                        detail.arrivalDateTime = DateTime.ParseExact(technicalStop.StopDetails[0].Date + technicalStop.StopDetails[0].FirstTime
                            , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        detail.arrivalDateTime = DateTime.ParseExact(flightInfo.ProductDateTime.DateOfArrival + flightInfo.ProductDateTime.TimeOfArrival
                            , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    }

                    detail.equipmentType = flightInfo.ProductDetail.EquipmentType;

                    detail.rbd = rbd[0];
                    detail.fareBasis = fareBasis[0];
                    detail.availableSeat = int.Parse(avail[0]);
                    detail.fareType = fareType[0];
                    detail.cabin = cabin[0];
                    detail.Seq = iSeq;
                    bAllow = airlineControl.CheckAirlineControl(airlineControls, OrginalCountryCode, DestinationCountryCode, detail.airline.code, detail.rbd, detail.fareBasis);
                    if (bAllow)
                    {
                        detail.setDisplayDateTime(languageCode, depDate);

                        flight.flightDetails.Add(detail);


                        if (technicalStop != null && technicalStop.StopDetails != null && technicalStop.StopDetails.Count > 1)
                        {
                            iSeq++;
                            detail = new Entities.RobinhoodFlight.FlightDetail();
                            detail.airline = new Airline(namingService, languageCode);
                            detail.airline.code = flightInfo.CompanyId.MarketingCarrier;
                            detail.operatedAirline = new Airline(namingService, languageCode);
                            if (flightInfo.CompanyId.OperatingCarrier != null)
                            {
                                detail.operatedAirline.code = flightInfo.CompanyId.OperatingCarrier;
                            }
                            else
                            {
                                detail.operatedAirline.code = detail.airline.code;
                            }
                            detail.flightNumber = flightInfo.FlightNumber;
                            detail.depCity = new Airport(namingService, true, languageCode);
                            detail.depCity.code = technicalStop.StopDetails[0].LocationId;
                            detail.arrCity = new Airport(namingService, true, languageCode);
                            detail.arrCity.code = flightInfo.Location[1].LocationId;
                            detail.arrCity.terminal = flightInfo.Location[1].Terminal;

                            detail.departureDateTime = DateTime.ParseExact(technicalStop.StopDetails[1].Date + technicalStop.StopDetails[1].FirstTime
                                , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                            detail.arrivalDateTime = DateTime.ParseExact(flightInfo.ProductDateTime.DateOfArrival + flightInfo.ProductDateTime.TimeOfArrival
                                    , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                            detail.equipmentType = flightInfo.ProductDetail.EquipmentType;

                            detail.rbd = rbd[0];
                            detail.fareBasis = fareBasis[0];
                            detail.availableSeat = int.Parse(avail[0]);
                            detail.fareType = fareType[0];
                            detail.cabin = cabin[0];
                            detail.Seq = iSeq;
                            detail.inFlightServices = new InFlightServices();

                            detail.setDisplayDateTime(languageCode, depDate);
                            flight.flightDetails.Add(detail);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < flightDetail.FlightDetails.FlightDetailArray.Count; i++)
                    {
                        var flightInfo = flightDetail.FlightDetails.FlightDetailArray[i].FlightInformation;
                        Entities.RobinhoodFlight.FlightDetail detail = new Entities.RobinhoodFlight.FlightDetail();

                        List<string> inFlightSvc = new List<string>();
                        if (flightDetail.FlightDetails.FlightDetailArray[i].FlightCharacteristics != null)
                        {
                            if (flightDetail.FlightDetails.FlightDetailArray[i].FlightCharacteristics.InFlightSrv.StringArray != null)
                            {
                                inFlightSvc = flightDetail.FlightDetails.FlightDetailArray[i].FlightCharacteristics.InFlightSrv.StringArray;
                            }
                            else
                            {
                                inFlightSvc.Add(flightDetail.FlightDetails.FlightDetailArray[i].FlightCharacteristics.InFlightSrv.String);
                            }
                        }
                        detail.inFlightServices = new InFlightServices();
                        if (inFlightSvc.Contains("7"))
                        {
                            detail.inFlightServices.dutyFreeSale = true;
                        }
                        if (inFlightSvc.Contains("1") || inFlightSvc.Contains("4") || inFlightSvc.Contains("5") || inFlightSvc.Contains("10") || inFlightSvc.Contains("15"))
                        {
                            detail.inFlightServices.inflightEntertain = true;
                        }
                        if (inFlightSvc.Contains("12"))
                        {
                            detail.inFlightServices.inSeatPower = true;
                        }
                        if (inFlightSvc.Contains("13") || inFlightSvc.Contains("18"))
                        {
                            detail.inFlightServices.wifiInternet = true;
                        }

                        detail.airline = new Airline(namingService, languageCode);
                        detail.airline.code = flightInfo.CompanyId.MarketingCarrier;
                        detail.operatedAirline = new Airline(namingService, languageCode);
                        if (flightInfo.CompanyId.OperatingCarrier != null)
                        {
                            detail.operatedAirline.code = flightInfo.CompanyId.OperatingCarrier;
                        }
                        else
                        {
                            detail.operatedAirline.code = detail.airline.code;
                        }
                        detail.flightNumber = flightInfo.FlightNumber;
                        detail.depCity = new Airport(namingService, true, languageCode);
                        detail.depCity.code = flightInfo.Location[0].LocationId;
                        detail.depCity.terminal = flightInfo.Location[0].Terminal;
                        detail.arrCity = new Airport(namingService, true, languageCode);
                        detail.arrCity.code = flightInfo.Location[1].LocationId;
                        detail.arrCity.terminal = flightInfo.Location[1].Terminal;

                        detail.departureDateTime = DateTime.ParseExact(flightInfo.ProductDateTime.DateOfDeparture + flightInfo.ProductDateTime.TimeOfDeparture
                            , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        detail.arrivalDateTime = DateTime.ParseExact(flightInfo.ProductDateTime.DateOfArrival + flightInfo.ProductDateTime.TimeOfArrival
                            , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);

                        detail.equipmentType = flightInfo.ProductDetail.EquipmentType;

                        detail.rbd = rbd[i];
                        detail.fareBasis = fareBasis[i];
                        detail.availableSeat = int.Parse(avail[i]);
                        detail.fareType = fareType[i];
                        detail.cabin = cabin[i];
                        detail.Seq = i;
                        bAllow = airlineControl.CheckAirlineControl(airlineControls, OrginalCountryCode, DestinationCountryCode, detail.airline.code, detail.rbd, detail.fareBasis);
                        if (bAllow)
                        {
                            detail.setDisplayDateTime(languageCode, depDate);

                            flight.flightDetails.Add(detail);
                        }
                    }
                }
                if (flight.flightDetails.Count > 0)
                {
                    flightList.Add(flight);
                }
            }

            return flightList;
        }

        private List<Entities.RobinhoodFlight.Flight> GetFlight(FlightIndexUnion flightIndex,
            PaxFareProductUnion paxFareProduct, string paxType, SegmentFlightRefUnion segmentRef,
           Entities.MasterPricer.Request request, ref int segmentNo, string OrginalCountryCodes, string DestinationCountryCodes, List<AirlineControlSub> airlineControls, ServiceFeesGrpUnion serviceFeesGrp, Guid pgSearchOID)
        {
            NamingServices namingService = new NamingServices(_unitOfWork);
            AirlineControlServices airlineControl = new AirlineControlServices(_unitOfWork);
            string[] pax = paxType.Split(',');
            List<string> rbd = new List<string>();
            List<string> fareBasis = new List<string>();
            List<string> avail = new List<string>();
            List<string> fareType = new List<string>();
            List<string> cabin = new List<string>();

            string languageCode = request.languageCode;

            List<Entities.RobinhoodFlight.Flight> flightList = new List<Entities.RobinhoodFlight.Flight>();

            if (paxFareProduct.PaxFareProductElementArray == null)
            {
                var fareDetails = paxFareProduct.PurplePaxFareProduct.FareDetails;
                FareDetailsGroupOfFares fdgf;
                if (fareDetails.FareDetailsElement != null)
                {
                    fdgf = fareDetails.FareDetailsElement.GroupOfFares;
                    segmentNo = int.Parse(fareDetails.FareDetailsElement.SegmentRef.SegRef);
                }
                else
                {
                    fdgf = fareDetails.FareDetailsElementArray.FirstOrDefault().GroupOfFares;
                    segmentNo = int.Parse(fareDetails.FareDetailsElementArray.FirstOrDefault().SegmentRef.SegRef);
                }

                if (fdgf.FluffyGroupOfFares != null)
                {
                    var product = fdgf.FluffyGroupOfFares.ProductInformation;
                    avail.Add(product.CabinProduct.PurpleCabinProduct.AvlStatus);
                    cabin.Add(product.CabinProduct.PurpleCabinProduct.Cabin);
                    rbd.Add(product.CabinProduct.PurpleCabinProduct.Rbd);
                    fareBasis.Add(product.FareProductDetail.FareBasis);
                    if (product.FareProductDetail.FareType.StringArray != null &&
                        product.FareProductDetail.FareType.StringArray.Count > 0)
                    {
                        fareType.Add(product.FareProductDetail.FareType.StringArray[0]);
                    }
                    else
                    {
                        fareType.Add(product.FareProductDetail.FareType.String);
                    }
                }
                else
                {
                    foreach (var gf in fdgf.FluffyGroupOfFareArray)
                    {
                        var product = gf.ProductInformation;
                        avail.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.AvlStatus : product.CabinProduct.CabinProductElementArray[1].AvlStatus);
                        cabin.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Cabin : product.CabinProduct.CabinProductElementArray[1].Cabin);
                        rbd.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Rbd : product.CabinProduct.CabinProductElementArray[1].Rbd);
                        fareBasis.Add(product.FareProductDetail.FareBasis);
                        fareType.Add(product.FareProductDetail.FareType.String == null ? String.Join(",", product.FareProductDetail.FareType.StringArray) : product.FareProductDetail.FareType.String);
                    }
                }
            }
            else
            {
                var fareDetails = paxFareProduct.PaxFareProductElementArray.FirstOrDefault(x => x.PaxReference.PurplePaxReference != null ? pax.Contains(x.PaxReference.PurplePaxReference.Ptc.ToUpper()) : pax.Contains(x.PaxReference.PaxReferenceElementArray[0].Ptc.ToUpper())).FareDetails;
                TentacledGroupOfFares tgf;
                if (fareDetails.PurpleFareDetail != null)
                {
                    tgf = fareDetails.PurpleFareDetail.GroupOfFares;
                    segmentNo = int.Parse(fareDetails.PurpleFareDetail.SegmentRef.SegRef);
                }
                else
                {
                    tgf = fareDetails.FareDetailElementArray.FirstOrDefault().GroupOfFares;
                    segmentNo = int.Parse(fareDetails.FareDetailElementArray.FirstOrDefault().SegmentRef.SegRef);
                }

                if (tgf.PurpleGroupOfFares != null)
                {
                    var product = tgf.PurpleGroupOfFares.ProductInformation;
                    avail.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.AvlStatus : product.CabinProduct.CabinProductElementArray[1].AvlStatus);
                    cabin.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Cabin : product.CabinProduct.CabinProductElementArray[1].Cabin);
                    rbd.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Rbd : product.CabinProduct.CabinProductElementArray[1].Rbd);
                    fareBasis.Add(product.FareProductDetail.FareBasis);
                    if (product.FareProductDetail.FareType.StringArray != null &&
                        product.FareProductDetail.FareType.StringArray.Count > 0)
                    {
                        fareType.Add(product.FareProductDetail.FareType.StringArray[0]);
                    }
                    else
                    {
                        fareType.Add(product.FareProductDetail.FareType.String);
                    }
                }
                else
                {
                    foreach (var gf in tgf.PurpleGroupOfFareArray)
                    {
                        var product = gf.ProductInformation;
                        avail.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.AvlStatus : product.CabinProduct.CabinProductElementArray[1].AvlStatus);
                        cabin.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Cabin : product.CabinProduct.CabinProductElementArray[1].Cabin);
                        rbd.Add(product.CabinProduct.PurpleCabinProduct != null ? product.CabinProduct.PurpleCabinProduct.Rbd : product.CabinProduct.CabinProductElementArray[1].Rbd);
                        fareBasis.Add(product.FareProductDetail.FareBasis);
                        fareType.Add(product.FareProductDetail.FareType.String == null ? String.Join(",", product.FareProductDetail.FareType.StringArray) : product.FareProductDetail.FareType.String);
                    }
                }
            }

            List<string> refNo = new List<string>();
            string freeBagAllownceInfoRef = "";
            if (segmentRef.SegmentFlightRefElement != null)
            {
                if (segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElement != null && segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElement.RefQualifier.Equals("S"))
                {
                    refNo.Add(segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElement.RefNumber);
                }
                else
                {
                    foreach (var refReferencingDetail in segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElementArray)
                    {
                        if (refReferencingDetail.RefQualifier.Equals("S"))
                        {
                            refNo.Add(refReferencingDetail.RefNumber);
                        }
                        else if (refReferencingDetail.RefQualifier.Equals("B"))
                        {
                            freeBagAllownceInfoRef = refReferencingDetail.RefNumber;
                        }
                    }
                    //if (segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElementArray[0].RefQualifier.Equals("S"))
                    //{
                    //    refNo.Add(segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElementArray[0].RefNumber);
                    //}

                }

            }
            else
            {
                string sRefNo = "";
                foreach (var refList in segmentRef.SegmentFlightRefElementArray)
                {
                    sRefNo = "";
                    if (refList.ReferencingDetail.ReferencingDetailElement != null && refList.ReferencingDetail.ReferencingDetailElement.RefQualifier.Equals("S"))
                    {
                        sRefNo = refList.ReferencingDetail.ReferencingDetailElement.RefNumber;
                    }
                    else if (refList.ReferencingDetail.ReferencingDetailElement != null && refList.ReferencingDetail.ReferencingDetailElement.RefQualifier.Equals("B"))
                    {
                        freeBagAllownceInfoRef = refList.ReferencingDetail.ReferencingDetailElement.RefNumber;
                    }
                    else
                    {
                        foreach (var refReferencingDetail in refList.ReferencingDetail.ReferencingDetailElementArray)
                        {
                            if (refReferencingDetail.RefQualifier.Equals("S"))
                            {
                                sRefNo = refReferencingDetail.RefNumber;
                            }
                            else if (refReferencingDetail.RefQualifier.Equals("B"))
                            {
                                freeBagAllownceInfoRef = refReferencingDetail.RefNumber;
                            }
                        }
                    }

                    if (!refNo.Contains(sRefNo))
                    {
                        refNo.Add(sRefNo);
                    }


                }
            }
            DateTime depDate = request.flights[segmentNo - 1].departureDateTime;
            var gFlight = flightIndex.PurpleFlightIndex == null ?
                            flightIndex.FlightIndexElementArray[segmentNo - 1].GroupOfFlights :
                         flightIndex.PurpleFlightIndex.GroupOfFlights;
            List<GroupOfFlight> groupFlight = new List<GroupOfFlight>();
            if (gFlight.PurpleGroupOfFlight != null)
            {
                groupFlight.Add(gFlight.PurpleGroupOfFlight);
            }
            else
            {
                groupFlight = gFlight.GroupOfFlightElementArray;
            }

            int iSeg = 1;
            bool bSave = true;
            string sAirlineCode = "";
            //Find flight
            int iDetailSeg = 0;
            foreach (string rN in refNo)
            {
                Log.Debug(rN);
                var flightDetail = groupFlight.FirstOrDefault(x => x.PropFlightGrDetail.FlightProposal[0].Ref == rN);
                Entities.RobinhoodFlight.Flight flight = new Entities.RobinhoodFlight.Flight();
                flight.bShow = true;
                flight.refNumber = rN;
                flight.totalTime = flightDetail.PropFlightGrDetail.FlightProposal.FirstOrDefault(x => x.UnitQualifier == "EFT").Ref;
                flight.pgSearchOID = pgSearchOID.ToString();
                flight.flightFrom = "A";//A=Amadeus,L=LionAir
                if (freeBagAllownceInfoRef.Length > 0)
                {
                    flight.baggageDetails = GetBaggageDetails(freeBagAllownceInfoRef, serviceFeesGrp, segmentNo);
                }
                flight.flightDetails = new List<Entities.RobinhoodFlight.FlightDetail>();
                if (flightDetail.FlightDetails.FlightDetailsClass != null)
                {
                    //TODO TECHNICAL STOP
                    var flightInfo = flightDetail.FlightDetails.FlightDetailsClass.FlightInformation;
                    var technicalStop = flightDetail.FlightDetails.FlightDetailsClass.TechnicalStop;
                    Entities.RobinhoodFlight.FlightDetail detail = new Entities.RobinhoodFlight.FlightDetail();

                    List<string> inFlightSvc = new List<string>();
                    if (flightDetail.FlightDetails.FlightDetailsClass.FlightCharacteristics != null)
                    {
                        if (flightDetail.FlightDetails.FlightDetailsClass.FlightCharacteristics.InFlightSrv.StringArray != null)
                        {
                            inFlightSvc = flightDetail.FlightDetails.FlightDetailsClass.FlightCharacteristics.InFlightSrv.StringArray;
                        }
                        else
                        {
                            inFlightSvc.Add(flightDetail.FlightDetails.FlightDetailsClass.FlightCharacteristics.InFlightSrv.String);
                        }
                    }
                    detail.inFlightServices = new InFlightServices();
                    if (inFlightSvc.Contains("7"))
                    {
                        detail.inFlightServices.dutyFreeSale = true;
                    }
                    if (inFlightSvc.Contains("1") || inFlightSvc.Contains("4") || inFlightSvc.Contains("5") || inFlightSvc.Contains("10") || inFlightSvc.Contains("15"))
                    {
                        detail.inFlightServices.inflightEntertain = true;
                    }
                    if (inFlightSvc.Contains("12"))
                    {
                        detail.inFlightServices.inSeatPower = true;
                    }
                    if (inFlightSvc.Contains("13") || inFlightSvc.Contains("18"))
                    {
                        detail.inFlightServices.wifiInternet = true;
                    }

                    detail.airline = new Airline(namingService, languageCode);
                    detail.airline.code = flightInfo.CompanyId.MarketingCarrier;
                    detail.operatedAirline = new Airline(namingService, languageCode);
                    if (flightInfo.CompanyId.OperatingCarrier != null)
                    {
                        detail.operatedAirline.code = flightInfo.CompanyId.OperatingCarrier;
                    }
                    else
                    {
                        detail.operatedAirline.code = detail.airline.code;
                    }
                    detail.flightNumber = flightInfo.FlightNumber;
                    detail.depCity = new Airport(namingService, true, languageCode);
                    detail.depCity.code = flightInfo.Location[0].LocationId;
                    detail.arrCity = new Airport(namingService, true, languageCode);
                    if (technicalStop != null && technicalStop.StopDetails != null && technicalStop.StopDetails.Count > 1)
                    {
                        detail.arrCity.code = technicalStop.StopDetails[0].LocationId;
                    }
                    else
                    {
                        detail.arrCity.code = flightInfo.Location[1].LocationId;
                    }

                    detail.departureDateTime = DateTime.ParseExact(flightInfo.ProductDateTime.DateOfDeparture + flightInfo.ProductDateTime.TimeOfDeparture
                        , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    if (technicalStop != null && technicalStop.StopDetails != null && technicalStop.StopDetails.Count > 1)
                    {
                        detail.arrivalDateTime = DateTime.ParseExact(technicalStop.StopDetails[0].Date + technicalStop.StopDetails[0].FirstTime
                            , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        detail.arrivalDateTime = DateTime.ParseExact(flightInfo.ProductDateTime.DateOfArrival + flightInfo.ProductDateTime.TimeOfArrival
                            , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    }

                    detail.equipmentType = flightInfo.ProductDetail.EquipmentType;

                    detail.rbd = rbd[0];
                    detail.fareBasis = fareBasis[0];
                    detail.availableSeat = int.Parse(avail[0]);
                    detail.fareType = fareType[0];
                    detail.cabin = cabin[0];
                    detail.Seq = iDetailSeg;
                    Log.Debug("OrginalCountryCodes=" + OrginalCountryCodes + "/DestinationCountryCodes=" + DestinationCountryCodes + "/detail.airline.code=" + detail.airline.code + "/detail.rbd=" + detail.rbd + "/detail.fareBasis=" + detail.fareBasis);
                    bSave = airlineControl.CheckAirlineControl(airlineControls, OrginalCountryCodes, DestinationCountryCodes, detail.airline.code, detail.rbd, detail.fareBasis);
                    Log.Debug("bSave=" + bSave);
                    if (bSave)
                    {
                        detail.setDisplayDateTime(languageCode, depDate);

                        flight.flightDetails.Add(detail);


                        if (technicalStop != null && technicalStop.StopDetails != null && technicalStop.StopDetails.Count > 1)
                        {
                            iDetailSeg++;
                            detail = new Entities.RobinhoodFlight.FlightDetail();
                            detail.airline = new Airline(namingService, languageCode);
                            detail.airline.code = flightInfo.CompanyId.MarketingCarrier;
                            detail.operatedAirline = new Airline(namingService, languageCode);
                            if (flightInfo.CompanyId.OperatingCarrier != null)
                            {
                                detail.operatedAirline.code = flightInfo.CompanyId.OperatingCarrier;
                            }
                            else
                            {
                                detail.operatedAirline.code = detail.airline.code;
                            }
                            detail.flightNumber = flightInfo.FlightNumber;
                            detail.depCity = new Airport(namingService, true, languageCode);
                            detail.depCity.code = technicalStop.StopDetails[0].LocationId;
                            detail.arrCity = new Airport(namingService, true, languageCode);
                            detail.arrCity.code = flightInfo.Location[1].LocationId;

                            detail.departureDateTime = DateTime.ParseExact(technicalStop.StopDetails[1].Date + technicalStop.StopDetails[1].FirstTime
                                , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                            detail.arrivalDateTime = DateTime.ParseExact(flightInfo.ProductDateTime.DateOfArrival + flightInfo.ProductDateTime.TimeOfArrival
                                    , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                            detail.equipmentType = flightInfo.ProductDetail.EquipmentType;

                            detail.rbd = rbd[0];
                            detail.fareBasis = fareBasis[0];
                            detail.availableSeat = int.Parse(avail[0]);
                            detail.fareType = fareType[0];
                            detail.cabin = cabin[0];
                            detail.Seq = iDetailSeg;
                            detail.setDisplayDateTime(languageCode, depDate);
                            flight.flightDetails.Add(detail);

                        }//if (technicalStop != null && technicalStop.StopDetails != null && technicalStop.StopDetails.Count > 1)
                    }// if (bSave)
                }
                else
                {
                    iSeg = flightDetail.FlightDetails.FlightDetailArray.Count;
                    for (int i = 0; i < flightDetail.FlightDetails.FlightDetailArray.Count; i++)
                    {
                        var flightInfo = flightDetail.FlightDetails.FlightDetailArray[i].FlightInformation;
                        Entities.RobinhoodFlight.FlightDetail detail = new Entities.RobinhoodFlight.FlightDetail();
                        List<string> inFlightSvc = new List<string>();
                        if (flightDetail.FlightDetails.FlightDetailArray[i].FlightCharacteristics != null)
                        {
                            if (flightDetail.FlightDetails.FlightDetailArray[i].FlightCharacteristics.InFlightSrv.StringArray != null)
                            {
                                inFlightSvc = flightDetail.FlightDetails.FlightDetailArray[i].FlightCharacteristics.InFlightSrv.StringArray;
                            }
                            else
                            {
                                inFlightSvc.Add(flightDetail.FlightDetails.FlightDetailArray[i].FlightCharacteristics.InFlightSrv.String);
                            }
                        }
                        detail.inFlightServices = new InFlightServices();
                        if (inFlightSvc.Contains("7"))
                        {
                            detail.inFlightServices.dutyFreeSale = true;
                        }
                        if (inFlightSvc.Contains("1") || inFlightSvc.Contains("4") || inFlightSvc.Contains("5") || inFlightSvc.Contains("10") || inFlightSvc.Contains("15"))
                        {
                            detail.inFlightServices.inflightEntertain = true;
                        }
                        if (inFlightSvc.Contains("12"))
                        {
                            detail.inFlightServices.inSeatPower = true;
                        }
                        if (inFlightSvc.Contains("13") || inFlightSvc.Contains("18"))
                        {
                            detail.inFlightServices.wifiInternet = true;
                        }
                        detail.airline = new Airline(namingService, languageCode);
                        detail.airline.code = flightInfo.CompanyId.MarketingCarrier;
                        detail.operatedAirline = new Airline(namingService, languageCode);
                        if (flightInfo.CompanyId.OperatingCarrier != null)
                        {
                            detail.operatedAirline.code = flightInfo.CompanyId.OperatingCarrier;
                        }
                        else
                        {
                            detail.operatedAirline.code = detail.airline.code;
                        }
                        detail.flightNumber = flightInfo.FlightNumber;
                        detail.depCity = new Airport(namingService, true, languageCode);
                        detail.depCity.code = flightInfo.Location[0].LocationId;
                        detail.arrCity = new Airport(namingService, true, languageCode);
                        detail.arrCity.code = flightInfo.Location[1].LocationId;

                        detail.departureDateTime = DateTime.ParseExact(flightInfo.ProductDateTime.DateOfDeparture + flightInfo.ProductDateTime.TimeOfDeparture
                            , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        detail.arrivalDateTime = DateTime.ParseExact(flightInfo.ProductDateTime.DateOfArrival + flightInfo.ProductDateTime.TimeOfArrival
                            , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);

                        detail.equipmentType = flightInfo.ProductDetail.EquipmentType;

                        detail.rbd = rbd[i];
                        detail.fareBasis = fareBasis[i];
                        detail.availableSeat = int.Parse(avail[i]);
                        detail.fareType = fareType[i];
                        detail.cabin = cabin[i];
                        detail.Seq = i;


                        if (bSave)
                        {
                            Log.Debug("OrginalCountryCodes=" + OrginalCountryCodes + "/DestinationCountryCodes=" + DestinationCountryCodes + "/detail.airline.code=" + detail.airline.code + "/detail.rbd=" + detail.rbd + "/detail.fareBasis=" + detail.fareBasis);
                            bSave = airlineControl.CheckAirlineControl(airlineControls, OrginalCountryCodes, DestinationCountryCodes, detail.airline.code, detail.rbd, detail.fareBasis);
                            Log.Debug("bSave=" + bSave);
                            if (bSave)
                            {
                                detail.setDisplayDateTime(languageCode, depDate);
                                flight.flightDetails.Add(detail);

                            }
                        }

                    }//for
                }

                if (flight.flightDetails.Count == iSeg)
                {
                    if (flight.flightDetails[0].departureDateTime.ToString("dd/MM/yyyy") != depDate.ToString("dd/MM/yyyy"))
                    {
                        flight.bShow = false;
                    }
                    flightList.Add(flight);
                }// if (flight.flightDetails.Count == iSeg)
            }//forech

            return flightList;
        }

        private Entities.RobinhoodFlight.BaggageDetails GetBaggageDetails(string freeBagAllownceInfoRef, ServiceFeesGrpUnion serviceFeesGrp, int segmentRef)
        {
            Entities.RobinhoodFlight.BaggageDetails baggage = new Entities.RobinhoodFlight.BaggageDetails();
            Entities.MasterPricer.BaggageDetails baggageDetails = new Entities.MasterPricer.BaggageDetails();
            string sRefNumber = "";
            if (serviceFeesGrp.PurpleServiceFeesGrp != null)
            {
                if (serviceFeesGrp.PurpleServiceFeesGrp.ServiceCoverageInfoGrp.ServiceCoverageInfoGrpElementArray != null)
                {

                    var _refNumber = serviceFeesGrp.PurpleServiceFeesGrp.ServiceCoverageInfoGrp.ServiceCoverageInfoGrpElementArray.Find(x => x.ServiceCovInfoGrp.ServiceCovInfoGrpElement != null && x.ItemNumberInfo.ItemNumber.Number.ToString() == freeBagAllownceInfoRef).ServiceCovInfoGrp.ServiceCovInfoGrpElement;
                    if (_refNumber == null)
                    {
                        var refCheck = serviceFeesGrp.PurpleServiceFeesGrp.ServiceCoverageInfoGrp.ServiceCoverageInfoGrpElementArray.Find(x => x.ServiceCovInfoGrp.ServiceCovInfoGrpElementArray != null).ServiceCovInfoGrp.ServiceCovInfoGrpElementArray;
                        if (refCheck != null)
                        {
                            _refNumber = refCheck.Find(x => x.RefInfo.ReferencingDetail.RefQualifier == "F");
                        }
                        else
                        {
                            _refNumber = serviceFeesGrp.PurpleServiceFeesGrp.ServiceCoverageInfoGrp.ServiceCoverageInfoGrpElementArray.Find(x => x.ServiceCovInfoGrp.ServiceCovInfoGrpElement != null).ServiceCovInfoGrp.ServiceCovInfoGrpElement;
                        }
                    }
                    sRefNumber = _refNumber.RefInfo.ReferencingDetail.RefNumber.ToString();

                }
                else
                {
                    var refCheck = serviceFeesGrp.PurpleServiceFeesGrp.ServiceCoverageInfoGrp.ServiceCoverageInfoGrpElement.ServiceCovInfoGrp.ServiceCovInfoGrpElementArray;
                    if (refCheck != null)
                    {
                        var _refNumber = refCheck.Find(x => x.RefInfo.ReferencingDetail.RefQualifier == "F");
                        sRefNumber = _refNumber.RefInfo.ReferencingDetail.RefNumber.ToString();
                    }
                    else
                    {
                        sRefNumber = serviceFeesGrp.PurpleServiceFeesGrp.ServiceCoverageInfoGrp.ServiceCoverageInfoGrpElement.ServiceCovInfoGrp.ServiceCovInfoGrpElement.RefInfo.ReferencingDetail.RefNumber.ToString();

                    }

                }

            }
            else
            {
                //segmentRef: 1=Depart, 2 Return
                var ServiceCoverageInfoGrp = serviceFeesGrp.ServiceFeesGrpElementArray.Find(x => x.ServiceCoverageInfoGrp.ServiceCoverageInfoGrpElementArray != null).ServiceCoverageInfoGrp.ServiceCoverageInfoGrpElementArray;
                if (ServiceCoverageInfoGrp != null)
                {
                    var ServiceCovInfoGrpElementArray = ServiceCoverageInfoGrp.Find(x => x.ItemNumberInfo.ItemNumber.Number.ToString() == freeBagAllownceInfoRef);
                    if (ServiceCovInfoGrpElementArray != null)
                    {
                        if (ServiceCovInfoGrpElementArray.ServiceCovInfoGrp.ServiceCovInfoGrpElementArray != null)
                        {
                            sRefNumber = ServiceCovInfoGrpElementArray.ServiceCovInfoGrp.ServiceCovInfoGrpElementArray[segmentRef - 1].RefInfo.ReferencingDetail.RefNumber.ToString();
                        }
                        else
                        {
                            sRefNumber = ServiceCovInfoGrpElementArray.ServiceCovInfoGrp.ServiceCovInfoGrpElement.RefInfo.ReferencingDetail.RefNumber.ToString();
                        }
                    }
                    else
                    {
                        var _refNumber = ServiceCoverageInfoGrp.Find(x => x.ServiceCovInfoGrp.ServiceCovInfoGrpElement != null && x.ItemNumberInfo.ItemNumber.Number.ToString() == freeBagAllownceInfoRef).ServiceCovInfoGrp.ServiceCovInfoGrpElement;
                        sRefNumber = _refNumber.RefInfo.ReferencingDetail.RefNumber.ToString();
                    }
                }
                else
                {
                    var _refNumber = serviceFeesGrp.ServiceFeesGrpElementArray.Find(x => x.ServiceCoverageInfoGrp.ServiceCoverageInfoGrpElement.ItemNumberInfo.ItemNumber.Number.ToString() == freeBagAllownceInfoRef).ServiceCoverageInfoGrp;
                    sRefNumber = _refNumber.ServiceCoverageInfoGrpElement.ServiceCovInfoGrp.ServiceCovInfoGrpElement.RefInfo.ReferencingDetail.RefNumber.ToString();
                }
            }

            if (sRefNumber.Length > 0)
            {
                if (serviceFeesGrp.ServiceFeesGrpElementArray != null)
                {
                    var FreeBagAllowanceGrpArray = serviceFeesGrp.ServiceFeesGrpElementArray.Find(x => x.FreeBagAllowanceGrp.FreeBagAllowanceGrpElementArray != null).FreeBagAllowanceGrp.FreeBagAllowanceGrpElementArray;
                    if (FreeBagAllowanceGrpArray != null)
                    {
                        baggageDetails = FreeBagAllowanceGrpArray.Find(x => x.ItemNumberInfo.ItemNumberDetails.Number.ToString() == sRefNumber).FreeBagAllownceInfo.BaggageDetails;
                    }
                    else
                    {
                        var FreeBagAllowanceGrp = serviceFeesGrp.ServiceFeesGrpElementArray.Find(x => x.FreeBagAllowanceGrp.FreeBagAllowanceGrpElement != null).FreeBagAllowanceGrp.FreeBagAllowanceGrpElement;
                        if (FreeBagAllowanceGrp != null)
                        {
                            baggageDetails = FreeBagAllowanceGrp.FreeBagAllownceInfo.BaggageDetails;
                        }
                    }

                }
                else
                {
                    if (serviceFeesGrp.PurpleServiceFeesGrp.FreeBagAllowanceGrp.FreeBagAllowanceGrpElement != null)
                    {
                        baggageDetails = serviceFeesGrp.PurpleServiceFeesGrp.FreeBagAllowanceGrp.FreeBagAllowanceGrpElement.FreeBagAllownceInfo.BaggageDetails;
                    }
                    else
                    {
                        baggageDetails = serviceFeesGrp.PurpleServiceFeesGrp.FreeBagAllowanceGrp.FreeBagAllowanceGrpElementArray.Find(x => x.ItemNumberInfo.ItemNumberDetails.Number.ToString() == sRefNumber).FreeBagAllownceInfo.BaggageDetails;
                    }
                }
                baggage.freeAllowance = int.Parse(baggageDetails.FreeAllowance.ToString());
                baggage.quantityCode = baggageDetails.QuantityCode;
                baggage.unitQualifier = baggageDetails.UnitQualifier;
            }
            return baggage;
        }

        private List<Entities.RobinhoodFlight.FamilyInformation> GetFamilyInformation(ReferencingDetailUnion referencingDetail, List<Entities.MasterPricer.FamilyInformation> familyInformation, ServiceFeesGrpUnion serviceFeesGrp)
        {
            List<Entities.RobinhoodFlight.FamilyInformation> familyInformationList = null;
            if (familyInformation == null)
                return null;
            familyInformationList = new List<Entities.RobinhoodFlight.FamilyInformation>();
            Entities.RobinhoodFlight.FamilyInformation information = new Entities.RobinhoodFlight.FamilyInformation();
            Entities.RobinhoodFlight.Service service = new Entities.RobinhoodFlight.Service();
            string sRefNumber = "";
            string sCommercialName = "";
            string sServiceGroup = "";
            string sServiceSubGroup = "";
            bool bGetService = false;
            if (referencingDetail.ReferencingDetailElement != null)
            {
                var _ref = referencingDetail.ReferencingDetailElement;
                information = new Entities.RobinhoodFlight.FamilyInformation();
                sRefNumber = _ref.RefNumber;
                var _info = familyInformation.Find(x => x.RefNumber == int.Parse(sRefNumber));
                information.RefNumber = _info.RefNumber;
                information.FareFamilyname = _info.FareFamilyname;
                information.Description = _info.Description;
                information.Carrier = _info.Carrier;
                information.Services = new List<Entities.RobinhoodFlight.Service>();
                foreach (var _ser in _info.Services)
                {
                    service = new Entities.RobinhoodFlight.Service();
                    service.Reference = _ser.Reference;
                    service.Status = _ser.Status;
                    sCommercialName = "";
                    sServiceGroup = "";
                    sServiceSubGroup = "";
                    bGetService = GetServiceDescriptionInfo(_ser.Reference, (serviceFeesGrp.PurpleServiceFeesGrp != null ? serviceFeesGrp.PurpleServiceFeesGrp.ServiceDetailsGrp : serviceFeesGrp.ServiceFeesGrpElementArray.Find(x => x.ServiceDetailsGrp != null).ServiceDetailsGrp), ref sCommercialName, ref sServiceGroup, ref sServiceSubGroup);
                    service.CommercialName = sCommercialName;
                    service.ServiceGroup = sServiceGroup;
                    if (sServiceSubGroup.Length > 0)
                    {
                        service.ServiceSubGroup = sServiceSubGroup;
                    }
                    information.Services.Add(service);
                }
                familyInformationList.Add(information);
            }
            else
            {

                foreach (var _ref in referencingDetail.ReferencingDetailElementArray)
                {
                    if (_ref.RefNumber != null && _ref.RefNumber != "0")
                    {
                        information = new Entities.RobinhoodFlight.FamilyInformation();
                        sRefNumber = _ref.RefNumber;
                        var _info = familyInformation.Find(x => x.RefNumber == int.Parse(sRefNumber));
                        information.RefNumber = _info.RefNumber;
                        information.FareFamilyname = _info.FareFamilyname;
                        information.Description = _info.Description;
                        information.Carrier = _info.Carrier;
                        information.Services = new List<Entities.RobinhoodFlight.Service>();
                        foreach (var _ser in _info.Services)
                        {
                            service = new Entities.RobinhoodFlight.Service();
                            service.Reference = _ser.Reference;
                            service.Status = _ser.Status;
                            sCommercialName = "";
                            sServiceGroup = "";
                            sServiceSubGroup = "";
                            bGetService = GetServiceDescriptionInfo(_ser.Reference, (serviceFeesGrp.PurpleServiceFeesGrp != null ? serviceFeesGrp.PurpleServiceFeesGrp.ServiceDetailsGrp : serviceFeesGrp.ServiceFeesGrpElementArray.Find(x => x.ServiceDetailsGrp != null).ServiceDetailsGrp), ref sCommercialName, ref sServiceGroup, ref sServiceSubGroup);
                            service.CommercialName = sCommercialName;
                            service.ServiceGroup = sServiceGroup;
                            if (sServiceSubGroup.Length > 0)
                            {
                                service.ServiceSubGroup = sServiceSubGroup;
                            }
                            information.Services.Add(service);
                        }
                        familyInformationList.Add(information);
                    }
                }
            }
            return familyInformationList;
        }

        private bool GetServiceDescriptionInfo(int number, List<ServiceDetailsGrp> serviceDetailsGrp, ref string CommercialName, ref string ServiceGroup, ref string ServiceSubGroup)
        {
            bool bResult = false;
            if (serviceDetailsGrp.Count > 0)
            {
                var service = serviceDetailsGrp.Find(x => x.FeeDescriptionGrp.ItemNumberInfo.ItemNumberDetails.Number == number);
                if (service != null)
                {
                    bResult = true;
                    ServiceGroup = service.FeeDescriptionGrp.ServiceDescriptionInfo.ServiceRequirementsInfo.ServiceGroup;
                    var subGroup = service.FeeDescriptionGrp.ServiceDescriptionInfo.ServiceRequirementsInfo.ServiceSubGroup;
                    if (subGroup != null)
                    {
                        ServiceSubGroup = subGroup;
                    }
                    else
                    {
                        ServiceSubGroup = "";
                    }
                    CommercialName = service.FeeDescriptionGrp.CommercialName.FreeText;
                }
            }

            return bResult;
        }

        private static Dictionary<string, string> cityDic = new Dictionary<string, string>();
        private PaxPricing getPaxFare(PaxFareProductUnion paxFareProduct, string paxType, FareMarkup markup, List<BL.Entities.RobinhoodFlight.Flight> depFlights, NamingServices namingService, string userEmail, string tripType)
        {
            string[] pax = paxType.Split(',');
            PaxPricing paxPricing = new PaxPricing();
            if (paxFareProduct.PaxFareProductElementArray == null)
            {
                paxPricing.tax = Convert.ToDecimal(paxFareProduct.PurplePaxFareProduct.PaxFareDetail.TotalTaxAmount);
                paxPricing.fareBeforeMarkup = Convert.ToDecimal(paxFareProduct.PurplePaxFareProduct.PaxFareDetail.TotalFareAmount) - paxPricing.tax;
            }
            else
            {
                paxPricing.tax = Convert.ToDecimal(paxFareProduct.PaxFareProductElementArray.FirstOrDefault(x => x.PaxReference.PurplePaxReference != null ? pax.Contains(x.PaxReference.PurplePaxReference.Ptc.ToUpper()) : pax.Contains(x.PaxReference.PaxReferenceElementArray[0].Ptc.ToUpper())).PaxFareDetail.TotalTaxAmount);
                paxPricing.fareBeforeMarkup = Convert.ToDecimal(paxFareProduct.PaxFareProductElementArray.FirstOrDefault(x => x.PaxReference.PurplePaxReference != null ? pax.Contains(x.PaxReference.PurplePaxReference.Ptc.ToUpper()) : pax.Contains(x.PaxReference.PaxReferenceElementArray[0].Ptc.ToUpper())).PaxFareDetail.TotalFareAmount)
                    - paxPricing.tax;
            }

            string depCity = depFlights[0].flightDetails[0].depCity.code;
            string depCountry = "";
            if (cityDic.ContainsKey(depCity))
            {
                depCountry = cityDic[depCity];
            }
            else
            {
                depCountry = namingService.GetCountryCode(depCity);
                cityDic.Add(depCity, depCountry);
            }
            string arrCity = depFlights[0].flightDetails[depFlights[0].flightDetails.Count - 1].arrCity.code;
            string arrCountry = "";
            if (cityDic.ContainsKey(arrCity))
            {
                arrCountry = cityDic[arrCity];
            }
            else
            {
                arrCountry = namingService.GetCountryCode(arrCity);
                cityDic.Add(arrCity, arrCountry);
            }
            decimal lessFare = 0;
            paxPricing.fare = markup.getFareCharge(paxPricing.fareBeforeMarkup,
                depFlights[0].flightDetails[0].airline.code, depCity, depCountry, arrCity,
                arrCountry, depFlights[0].flightDetails[0].fareType,
                depFlights[0].flightDetails[0].rbd, depFlights[0].flightDetails[0].fareBasis, paxType, ref lessFare, userEmail, paymentFee, isPaymentFeePct, paxPricing.tax, tripType);
            paxPricing.lessFare = lessFare;
            return paxPricing;
        }

        private List<string> GetUpsell(SegmentFlightRefUnion segmentRef)
        {
            List<string> upsellList = null;
            List<ReferencingDetailElement> referencingDetail = null;
            if (segmentRef.SegmentFlightRefElement != null)
            {
                referencingDetail = segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElementArray.FindAll(x => x.RefQualifier == "U");
            }
            else
            {
                referencingDetail = segmentRef.SegmentFlightRefElementArray[0].ReferencingDetail.ReferencingDetailElementArray.FindAll(x => x.RefQualifier == "U");
            }
            if (referencingDetail != null)
            {
                upsellList = new List<string>();
                foreach (var _ref in referencingDetail)
                {
                    upsellList.Add(_ref.RefNumber);
                }
            }
            return upsellList;
        }
        public Entities.RobinhoodFare.AirFare InformativePricing(Entities.InformativePricing.Request request, Entities.InformativePricing.Request requestFor1A, string languageCode)
        {
            string url = ConfigurationManager.AppSettings["INFORMATIVE.URL"];
            string requestJson = JsonConvert.SerializeObject(requestFor1A);

            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("INFORMATIVE");
            Log.Debug(requestJson);
            Log.Debug(json);
            NamingServices namingServices = new NamingServices(_unitOfWork);
            AirlineQtaxControlServices airlineQtaxControlSvc = new AirlineQtaxControlServices(_unitOfWork);
            bool bCheckQTax = false;
            string depCity = "";
            string depCountry = "";
            string arrCity = "";
            string arrCountry = "";
            if (request.departureFlights != null && request.departureFlights.Count > 0)
            {
                depCity = request.departureFlights[0].departureCity;
                depCountry = namingServices.GetCountryCode(depCity);

                arrCity = request.departureFlights[request.departureFlights.Count - 1].arrivalCity;
                arrCountry = namingServices.GetCountryCode(arrCity);
                bCheckQTax = airlineQtaxControlSvc.CheckAirlineQTaxConfig(depCountry, arrCountry, request.departureFlights[0].companyCode);
            }
            else//Multi Destination
            {
                foreach (var multi in request.multiFlights)
                {
                    depCity = multi[0].departureCity;
                    depCountry = namingServices.GetCountryCode(depCity);

                    arrCity = multi[multi.Count - 1].arrivalCity;
                    arrCountry = namingServices.GetCountryCode(arrCity);
                    bCheckQTax = airlineQtaxControlSvc.CheckAirlineQTaxConfig(depCountry, arrCountry, multi[0].companyCode);
                    if (bCheckQTax)
                    {
                        break;
                    }
                }
            }
            Log.Debug("bCheckQTax=" + bCheckQTax);
            Entities.RobinhoodFare.AirFare airFare = new Entities.RobinhoodFare.AirFare(namingServices, languageCode);
            airFare.type = "A";//A:Amadeus
            BL.Entities.InformativePricing.Response pricingResponse = new Entities.InformativePricing.Response();

            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");
                pricingResponse = BL.Entities.InformativePricing.Response.FromJson(json);
            }

            if (pricingResponse.Error != null)
            {
                airFare.isError = true;
                airFare.errorCode = "3003";
                airFare.errorMessage = pricingResponse.Error.ErrorMessage;
                return airFare;
            }


            MarkupServices markupSvc = new MarkupServices(_unitOfWork);
            FareMarkup markup = markupSvc.GetMarkup();
            List<string> fareBasis = new List<string>();
            if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup != null)
            {

                var fare = pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup;
                airFare.adtFare = airFare.FindFareFromInformative(fare, request, "ADT", markup, paymentFee, isPaymentFeePct, bCheckQTax);

                fareBasis = GetFareBasis(fare.FareInfoGroup.SegmentLevelGroup);

            }
            else if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray.Count > 0)
            {
                int iPaxType = 1;
                if (request.noOfChildren > 0)
                {
                    iPaxType += 1;
                }
                if (request.noOfInfants > 0)
                {
                    iPaxType += 1;
                }
                bool bCheckByPassengersID = false;
                if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray.Count == iPaxType)
                {
                    bCheckByPassengersID = true;
                    iPaxType = 0;
                }
                foreach (var fare in pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray)
                {
                    iPaxType++;
                    string paxType = "";
                    if (bCheckByPassengersID)
                    {
                        switch (iPaxType)
                        {
                            case 1:
                                paxType = "ADT"; break;
                            case 2:
                                paxType = (request.noOfChildren > 0) ? "CHD" : "INF"; break;
                            case 3:
                                paxType = "INF"; break;
                        }
                    }
                    else
                    {
                        if (fare.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup != null)
                        {
                            paxType = fare.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.PtcSegment.QuantityDetails.UnitQualifier.ToUpper();
                        }
                        else
                        {

                            foreach (var segment in fare.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray)
                            {
                                if (paxType == "" || paxType == "ADT")
                                {
                                    paxType = segment.PtcSegment.QuantityDetails.UnitQualifier.ToUpper();
                                }
                            }

                        }
                    }

                    if (paxType == "ADT" && airFare.adtFare == null)
                    {
                        airFare.adtFare = airFare.FindFareFromInformative(fare, request, "ADT", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                        fareBasis = GetFareBasis(fare.FareInfoGroup.SegmentLevelGroup);
                    }
                    else if ((paxType == "CHD" || paxType == "CH") && airFare.chdFare == null)
                    {
                        airFare.chdFare = airFare.FindFareFromInformative(fare, request, "CHD", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                    }
                    else if ((paxType == "INF" || paxType == "IN") && airFare.infFare == null)
                    {
                        airFare.infFare = airFare.FindFareFromInformative(fare, request, "INF", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                    }


                }
                if (request.noOfChildren > 0 && airFare.chdFare == null)
                {
                    airFare.chdFare = airFare.adtFare;
                }

                if (request.noOfInfants > 0 && airFare.infFare == null)
                {
                    airFare.infFare = airFare.adtFare;
                }
            }
            else
            {
                return null;
            }

            airFare.fareRules = new List<Entities.RobinhoodFare.FareRule>();
            //Fare Rule
            for (int i = 0; i < fareBasis.Count; i++)
            {
                Entities.FareRule.Request fareRuleRequest = new Entities.FareRule.Request();
                fareRuleRequest.bookingOID = requestFor1A.bookingOID;
                fareRuleRequest.session = new Entities.FareRule.Session();
                fareRuleRequest.session.isStateFull = true;
                fareRuleRequest.session.InSeries = true;
                fareRuleRequest.session.End = false;
                fareRuleRequest.session.SessionId = pricingResponse.FareInformativePricingWithoutPnrReply.Session.SessionId;
                fareRuleRequest.session.SequenceNumber = (int.Parse(pricingResponse.FareInformativePricingWithoutPnrReply.Session.SequenceNumber) + 1).ToString();
                fareRuleRequest.session.SecurityToken = pricingResponse.FareInformativePricingWithoutPnrReply.Session.SecurityToken;

                fareRuleRequest.categoryCodes = new List<string>();
                fareRuleRequest.categoryCodes.Add("PE");
                fareRuleRequest.categoryCodes.Add("SR");
                fareRuleRequest.categoryCodes.Add("AP");
                fareRuleRequest.categoryCodes.Add("MN");
                fareRuleRequest.categoryCodes.Add("MX");
                fareRuleRequest.componentNo = (i + 1).ToString();
                fareRuleRequest.ClientIP = Utilities.HttpUtility.GetIPAddress();
                url = ConfigurationManager.AppSettings["FARERULE.URL"];
                requestJson = JsonConvert.SerializeObject(fareRuleRequest);

                json = HttpUtility.postJSON(url, requestJson);

                //Log.Info("FARE_RULE");
                //Log.Debug(requestJson);
                //Log.Debug(json);

                BL.Entities.FareRule.Response ruleResponse = new Entities.FareRule.Response();
                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");
                    ruleResponse = BL.Entities.FareRule.Response.FromJson(json);
                }

                if (ruleResponse.Error == null)
                {
                    Entities.RobinhoodFare.FareRule fareRule = new Entities.RobinhoodFare.FareRule();
                    fareRule.origin = new City(namingServices, languageCode);
                    fareRule.origin.code = ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails != null ? ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails.OdiGrp.OriginDestination.Origin : ruleResponse.FareCheckRulesReply.FlightDetails.FlightDetailsElementArray[0].OdiGrp.OriginDestination.Origin;
                    fareRule.destination = new City(namingServices, languageCode);
                    fareRule.destination.code = ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails != null ? ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails.OdiGrp.OriginDestination.Destination : ruleResponse.FareCheckRulesReply.FlightDetails.FlightDetailsElementArray[0].OdiGrp.OriginDestination.Destination;
                    fareRule.fareBasis = ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails != null ? ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails.QualificationFareDetails.AdditionalFareDetails.FareClass : ruleResponse.FareCheckRulesReply.FlightDetails.FlightDetailsElementArray[0].QualificationFareDetails.AdditionalFareDetails.FareClass;
                    fareRule.rules = new List<Entities.RobinhoodFare.FareRuleDatail>();
                    if (ruleResponse.FareCheckRulesReply.TariffInfo.PurpleTariffInfo != null)
                    {
                        Entities.RobinhoodFare.FareRuleDatail r = new Entities.RobinhoodFare.FareRuleDatail();
                        r.fareRuleText = new List<string>();
                        foreach (var text in ruleResponse.FareCheckRulesReply.TariffInfo.PurpleTariffInfo.FareRuleText)
                        {
                            r.fareRuleText.Add(text.FreeText);
                        }
                        fareRule.rules.Add(r);
                    }
                    else
                    {
                        if (ruleResponse.FareCheckRulesReply.TariffInfo.TariffInfoElementArray != null)
                        {
                            foreach (var info in ruleResponse.FareCheckRulesReply.TariffInfo.TariffInfoElementArray)
                            {
                                Entities.RobinhoodFare.FareRuleDatail r = new Entities.RobinhoodFare.FareRuleDatail();
                                r.fareRuleText = new List<string>();
                                foreach (var text in info.FareRuleText)
                                {
                                    r.fareRuleText.Add(text.FreeText);
                                }
                                fareRule.rules.Add(r);
                            }
                        }
                    }
                    airFare.fareRules.Add(fareRule);
                }

            }

            airFare.SetDisplayFlight(request);
            PassportServices passportServices = new PassportServices(_unitOfWork);
            List<PassportConfig> passList = passportServices.GetAll();
            bool isRequirePassport = false;
            string sOrigin = "";
            string sDestination = "";
            PassportConfig pass = new PassportConfig();
            if (request.multiFlights != null && request.multiFlights.Count > 0)
            {
                foreach (var route in request.multiFlights)
                {
                    sOrigin = namingServices.GetCountryCode(route[0].departureCity);
                    sDestination = namingServices.GetCountryCode(route[route.Count - 1].arrivalCity);
                    Log.Debug("sOrigin=" + sOrigin + "/sDestination=" + sDestination);
                    pass = passList.Find(x => (x.AirlineCodes == route[0].companyCode || x.AirlineCodes == "YY") && (x.OriginCodes.IndexOf(sOrigin) != -1 || x.OriginCodes == "*") && (x.DestinationCodes.IndexOf(sDestination) != -1 || x.DestinationCodes == "*"));
                    if (pass != null)
                    {
                        isRequirePassport = pass.IsActive.Value;
                    }
                    if (isRequirePassport)
                    {
                        break;
                    }
                    Log.Debug("isRequirePassport=" + isRequirePassport);
                }


            }
            else
            {
                foreach (var segment in request.departureFlights)
                {
                    sOrigin = namingServices.GetCountryCode(request.origin);
                    sDestination = namingServices.GetCountryCode(request.destination);
                    pass = passList.Find(x => (x.AirlineCodes == segment.companyCode || x.AirlineCodes == "YY") && (x.OriginCodes.IndexOf(sOrigin) != -1 || x.OriginCodes == "*") && (x.DestinationCodes.IndexOf(sDestination) != -1 || x.DestinationCodes == "*"));
                    if (pass != null)
                    {
                        isRequirePassport = pass.IsActive.Value;
                    }
                    if (!isRequirePassport)
                    {
                        sOrigin = namingServices.GetCountryCode(segment.departureCity);
                        sDestination = namingServices.GetCountryCode(segment.arrivalCity);
                        pass = passList.Find(x => (x.AirlineCodes == segment.companyCode || x.AirlineCodes == "YY") && (x.OriginCodes.IndexOf(sOrigin) != -1 || x.OriginCodes == "*") && (x.DestinationCodes.IndexOf(sDestination) != -1 || x.DestinationCodes == "*"));
                        if (pass != null)
                        {
                            isRequirePassport = pass.IsActive.Value;
                        }
                    }
                    if (isRequirePassport)
                    {
                        break;
                    }
                }
                if (!isRequirePassport && request.returnFlights != null && request.returnFlights.Count > 0)
                {
                    foreach (var segment in request.returnFlights)
                    {
                        sOrigin = namingServices.GetCountryCode(request.origin);
                        sDestination = namingServices.GetCountryCode(request.destination);
                        pass = passList.Find(x => (x.AirlineCodes == segment.companyCode || x.AirlineCodes == "YY") && (x.OriginCodes.IndexOf(sOrigin) != -1 || x.OriginCodes == "*") && (x.DestinationCodes.IndexOf(sDestination) != -1 || x.DestinationCodes == "*"));
                        if (pass != null)
                        {
                            isRequirePassport = pass.IsActive.Value;
                        }
                        if (!isRequirePassport)
                        {
                            sOrigin = namingServices.GetCountryCode(segment.departureCity);
                            sDestination = namingServices.GetCountryCode(segment.arrivalCity);
                            pass = passList.Find(x => (x.AirlineCodes == segment.companyCode || x.AirlineCodes == "YY") && (x.OriginCodes.IndexOf(sOrigin) != -1 || x.OriginCodes == "*") && (x.DestinationCodes.IndexOf(sDestination) != -1 || x.DestinationCodes == "*"));
                            if (pass != null)
                            {
                                isRequirePassport = pass.IsActive.Value;
                            }
                        }
                        if (isRequirePassport)
                        {
                            break;
                        }
                    }
                }
            }
            airFare.isPassportRequired = isRequirePassport;
            airFare.isPricingWithSegment = false;
            return airFare;
        }

        public Entities.RobinhoodFare.AirFare InformativePricing(Entities.InformativePricing.Request request, Entities.InformativePricing.Request requestForDepart1A, Entities.InformativePricing.Request requestForReturn1A, string languageCode)
        {
            string url = ConfigurationManager.AppSettings["INFORMATIVE.URL"];
            string requestJson = JsonConvert.SerializeObject(requestForDepart1A);
            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("INFORMATIVE DEPART");
            Log.Debug(requestJson);
            Log.Debug(json);

            NamingServices namingServices = new NamingServices(_unitOfWork);
            AirlineQtaxControlServices airlineQtaxControlSvc = new AirlineQtaxControlServices(_unitOfWork);
            string depCity = request.departureFlights[0].departureCity;
            string depCountry = namingServices.GetCountryCode(depCity);

            string arrCity = request.departureFlights[request.departureFlights.Count - 1].arrivalCity;
            string arrCountry = namingServices.GetCountryCode(arrCity);
            bool bCheckQTax = airlineQtaxControlSvc.CheckAirlineQTaxConfig(depCountry, arrCountry, request.departureFlights[0].companyCode);
            Entities.RobinhoodFare.AirFare airFare = new Entities.RobinhoodFare.AirFare(namingServices, languageCode);
            airFare.type = "A";//A:Amadeus
            BL.Entities.InformativePricing.Response pricingResponse = new Entities.InformativePricing.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");
                pricingResponse = BL.Entities.InformativePricing.Response.FromJson(json);
            }

            if (pricingResponse.Error != null)
            {
                airFare.isError = true;
                airFare.errorCode = "3004";
                airFare.errorMessage = pricingResponse.Error.ErrorMessage;
                return airFare;
            }

            requestJson = JsonConvert.SerializeObject(requestForReturn1A);
            json = HttpUtility.postJSON(url, requestJson);
            Log.Info("INFORMATIVE RETURN");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.InformativePricing.Response pricingReturnResponse = new Entities.InformativePricing.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");
                pricingReturnResponse = BL.Entities.InformativePricing.Response.FromJson(json);
            }
            if (pricingReturnResponse.Error != null)
            {
                airFare.isError = true;
                airFare.errorCode = "3005";
                airFare.errorMessage = pricingReturnResponse.Error.ErrorMessage;
                return airFare;
            }

            MarkupServices markupSvc = new MarkupServices(_unitOfWork);
            FareMarkup markup = markupSvc.GetMarkup();
            List<string> fareBasis = new List<string>();
            List<string> fareBasis_Return = new List<string>();
            List<SegmentForGetFareRule> segment_Depart = new List<SegmentForGetFareRule>();
            List<SegmentForGetFareRule> segment_Return = new List<SegmentForGetFareRule>();
            if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup != null)
            {
                if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup.FareInfoGroup.PricingIndicators.ProductDateTimeDetails.DepartureDate != null)
                {
                    string sLastTicketDate = pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup.FareInfoGroup.PricingIndicators.ProductDateTimeDetails.DepartureDate;
                    int iday = int.Parse(sLastTicketDate.Substring(0, 2));
                    int imonth = int.Parse(sLastTicketDate.Substring(2, 2));
                    int iyear = int.Parse(sLastTicketDate.Substring(4, 2)) + int.Parse(DateTime.Now.Year.ToString().Substring(0, 2) + "00");
                    DateTime dtFromXML = new DateTime(iyear, imonth, iday);
                    DateTime dtNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    DateTime dtAllowTicketTimeLimitHour = dtNow;

                    if (dtFromXML == dtNow && DateTime.Now >= dtAllowTicketTimeLimitHour)// 
                    {
                        Log.Debug("error: Cannot save PNR with last ticket date.");
                        return null;
                    }
                }

                var fareDepart = pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup;
                Log.Debug(fareDepart);
                if (pricingReturnResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup != null)
                {
                    Log.Debug("1");
                    if (pricingReturnResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup.FareInfoGroup.PricingIndicators.ProductDateTimeDetails.DepartureDate != null)
                    {
                        string sLastTicketDate = pricingReturnResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup.FareInfoGroup.PricingIndicators.ProductDateTimeDetails.DepartureDate;
                        int iday = int.Parse(sLastTicketDate.Substring(0, 2));
                        int imonth = int.Parse(sLastTicketDate.Substring(2, 2));
                        int iyear = int.Parse(sLastTicketDate.Substring(4, 2)) + int.Parse(DateTime.Now.Year.ToString().Substring(0, 2) + "00");
                        DateTime dtFromXML = new DateTime(iyear, imonth, iday);
                        DateTime dtNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        DateTime dtAllowTicketTimeLimitHour = dtNow;

                        if (dtFromXML == dtNow && DateTime.Now >= dtAllowTicketTimeLimitHour)// 
                        {
                            Log.Debug("error: Cannot save PNR with last ticket date.");
                            return null;
                        }
                    }

                    var fareReturn = pricingReturnResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup;
                    Log.Debug(fareReturn);
                    airFare.adtFare = airFare.FindFareFromInformative(fareDepart, fareReturn, request, "ADT", markup, paymentFee, isPaymentFeePct, bCheckQTax);


                    fareBasis = GetFareBasis(fareDepart.FareInfoGroup.SegmentLevelGroup, ref segment_Depart);
                    fareBasis_Return = GetFareBasis(fareReturn.FareInfoGroup.SegmentLevelGroup, ref segment_Return);

                }
                else if (pricingReturnResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray.Count > 0)
                {
                    Log.Debug("2");
                    int iPaxType = 1;
                    if (request.noOfChildren > 0)
                    {
                        iPaxType += 1;
                    }
                    if (request.noOfInfants > 0)
                    {
                        iPaxType += 1;
                    }
                    bool bCheckByPassengersID = false;
                    if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray.Count == iPaxType)
                    {
                        bCheckByPassengersID = true;
                        iPaxType = 0;
                    }
                    foreach (var fare in pricingReturnResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray)
                    {
                        iPaxType++;
                        string paxType = "";
                        if (bCheckByPassengersID)
                        {
                            switch (iPaxType)
                            {
                                case 1:
                                    paxType = "ADT"; break;
                                case 2:
                                    paxType = (request.noOfChildren > 0) ? "CHD" : "INF"; break;
                                case 3:
                                    paxType = "INF"; break;
                            }
                        }
                        else
                        {
                            if (fare.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup != null)
                            {
                                paxType = fare.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.PtcSegment.QuantityDetails.UnitQualifier.ToUpper();
                            }
                            else
                            {
                                foreach (var segment in fare.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray)
                                {
                                    if (paxType == "" || paxType == "ADT")
                                    {
                                        paxType = segment.PtcSegment.QuantityDetails.UnitQualifier.ToUpper();
                                    }
                                }

                            }
                        }
                        Log.Debug(paxType);
                        Log.Debug(fare);
                        if (paxType == "ADT")
                        {
                            airFare.adtFare = airFare.FindFareFromInformative(fareDepart, fare, request, "ADT", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                            fareBasis = GetFareBasis(fareDepart.FareInfoGroup.SegmentLevelGroup, ref segment_Depart);
                            fareBasis_Return = GetFareBasis(fare.FareInfoGroup.SegmentLevelGroup, ref segment_Return);

                        }
                        else if (paxType == "CHD" || paxType == "CH")
                        {
                            airFare.chdFare = airFare.FindFareFromInformative(fareDepart, fare, request, "CHD", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                        }
                        else if (paxType == "INF" || paxType == "IN")
                        {
                            airFare.infFare = airFare.FindFareFromInformative(fareDepart, fare, request, "INF", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                        }


                    }
                    if (request.noOfChildren > 0 && airFare.chdFare == null)
                    {
                        airFare.chdFare = airFare.adtFare;
                    }

                    if (request.noOfInfants > 0 && airFare.infFare == null)
                    {
                        airFare.infFare = airFare.adtFare;
                    }
                }

            }
            else if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray.Count > 0)
            {

                if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray[0].FareInfoGroup.PricingIndicators.ProductDateTimeDetails.DepartureDate != null)
                {
                    string sLastTicketDate = pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray[0].FareInfoGroup.PricingIndicators.ProductDateTimeDetails.DepartureDate;
                    int iday = int.Parse(sLastTicketDate.Substring(0, 2));
                    int imonth = int.Parse(sLastTicketDate.Substring(2, 2));
                    int iyear = int.Parse(sLastTicketDate.Substring(4, 2)) + int.Parse(DateTime.Now.Year.ToString().Substring(0, 2) + "00");
                    DateTime dtFromXML = new DateTime(iyear, imonth, iday);
                    DateTime dtNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    DateTime dtAllowTicketTimeLimitHour = dtNow;

                    if (dtFromXML == dtNow && DateTime.Now >= dtAllowTicketTimeLimitHour)// 
                    {
                        Log.Debug("error: Cannot save PNR with last ticket date.");
                        return null;
                    }
                }

                if (pricingReturnResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup != null)
                {
                    Log.Debug("3");
                    var fareReturn = pricingReturnResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup;
                    Log.Debug(fareReturn);
                    int iPaxType = 1;
                    if (request.noOfChildren > 0)
                    {
                        iPaxType += 1;
                    }
                    if (request.noOfInfants > 0)
                    {
                        iPaxType += 1;
                    }
                    bool bCheckByPassengersID = false;
                    if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray.Count == iPaxType)
                    {
                        bCheckByPassengersID = true;
                        iPaxType = 0;
                    }
                    foreach (var fare in pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray)
                    {
                        iPaxType++;
                        string paxType = "";
                        if (bCheckByPassengersID)
                        {
                            switch (iPaxType)
                            {
                                case 1:
                                    paxType = "ADT"; break;
                                case 2:
                                    paxType = (request.noOfChildren > 0) ? "CHD" : "INF"; break;
                                case 3:
                                    paxType = "INF"; break;
                            }
                        }
                        else
                        {
                            if (fare.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup != null)
                            {
                                paxType = fare.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.PtcSegment.QuantityDetails.UnitQualifier.ToUpper();
                            }
                            else
                            {
                                foreach (var segment in fare.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray)
                                {
                                    if (paxType == "" || paxType == "ADT")
                                    {
                                        paxType = segment.PtcSegment.QuantityDetails.UnitQualifier.ToUpper();
                                    }
                                }

                            }
                        }
                        Log.Debug(paxType);
                        Log.Debug(fare);
                        if (paxType == "ADT")
                        {
                            airFare.adtFare = airFare.FindFareFromInformative(fare, fareReturn, request, "ADT", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                            fareBasis = GetFareBasis(fare.FareInfoGroup.SegmentLevelGroup, ref segment_Depart);
                            fareBasis_Return = GetFareBasis(fareReturn.FareInfoGroup.SegmentLevelGroup, ref segment_Return);

                        }
                        else if (paxType == "CHD" || paxType == "CH")
                        {
                            airFare.chdFare = airFare.FindFareFromInformative(fare, fareReturn, request, "CHD", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                        }
                        else if (paxType == "INF" || paxType == "IN")
                        {
                            airFare.infFare = airFare.FindFareFromInformative(fare, fareReturn, request, "INF", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                        }


                    }
                    if (request.noOfChildren > 0 && airFare.chdFare == null)
                    {
                        airFare.chdFare = airFare.adtFare;
                    }

                    if (request.noOfInfants > 0 && airFare.infFare == null)
                    {
                        airFare.infFare = airFare.adtFare;
                    }
                }
                else
                {
                    Log.Debug("4");
                    int iPaxType = 1;
                    int iPaxTypeReturn = 1;
                    if (request.noOfChildren > 0)
                    {
                        iPaxType += 1;
                        iPaxTypeReturn += 1;
                    }
                    if (request.noOfInfants > 0)
                    {
                        iPaxType += 1;
                        iPaxTypeReturn += 1;
                    }
                    bool bCheckByPassengersID = false;
                    if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray.Count == iPaxType)
                    {
                        bCheckByPassengersID = true;
                        iPaxType = 0;
                    }
                    bool bCheckByPassengersIDReturn = false;
                    if (pricingReturnResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray.Count == iPaxType)
                    {
                        bCheckByPassengersIDReturn = true;
                        iPaxTypeReturn = 0;
                    }
                    foreach (var fare in pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray)
                    {
                        iPaxType++;
                        string paxType = "";
                        if (bCheckByPassengersID)
                        {
                            switch (iPaxType)
                            {
                                case 1:
                                    paxType = "ADT"; break;
                                case 2:
                                    paxType = (request.noOfChildren > 0) ? "CHD" : "INF"; break;
                                case 3:
                                    paxType = "INF"; break;
                            }
                        }
                        else
                        {
                            if (fare.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup != null)
                            {
                                paxType = fare.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.PtcSegment.QuantityDetails.UnitQualifier.ToUpper();
                            }
                            else
                            {
                                foreach (var segment in fare.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray)
                                {
                                    if (paxType == "" || paxType == "ADT")
                                    {
                                        paxType = segment.PtcSegment.QuantityDetails.UnitQualifier.ToUpper();
                                    }
                                }

                            }
                        }
                        iPaxTypeReturn = 0;
                        foreach (var fareReturn in pricingReturnResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray)
                        {
                            iPaxTypeReturn++;
                            string paxTypeReturn = "";
                            if (bCheckByPassengersID)
                            {
                                switch (iPaxTypeReturn)
                                {
                                    case 1:
                                        paxTypeReturn = "ADT"; break;
                                    case 2:
                                        paxTypeReturn = (request.noOfChildren > 0) ? "CHD" : "INF"; break;
                                    case 3:
                                        paxTypeReturn = "INF"; break;
                                }
                            }
                            else
                            {
                                if (fareReturn.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup != null)
                                {
                                    paxTypeReturn = fareReturn.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.PtcSegment.QuantityDetails.UnitQualifier.ToUpper();
                                }
                                else
                                {
                                    foreach (var segment in fareReturn.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray)
                                    {
                                        if (paxTypeReturn == "" || paxTypeReturn == "ADT")
                                        {
                                            paxTypeReturn = segment.PtcSegment.QuantityDetails.UnitQualifier.ToUpper();
                                        }
                                    }

                                }
                            }
                            Log.Debug(paxType);
                            Log.Debug(fare);
                            Log.Debug(fareReturn);
                            if (paxType == "ADT" && paxTypeReturn == "ADT")
                            {
                                airFare.adtFare = airFare.FindFareFromInformative(fare, fareReturn, request, "ADT", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                                fareBasis = GetFareBasis(fare.FareInfoGroup.SegmentLevelGroup, ref segment_Depart);
                                fareBasis_Return = GetFareBasis(fareReturn.FareInfoGroup.SegmentLevelGroup, ref segment_Return);

                            }
                            else if ((paxType == "CHD" || paxType == "CH") && (paxTypeReturn == "CHD" || paxTypeReturn == "CH"))
                            {
                                airFare.chdFare = airFare.FindFareFromInformative(fare, fareReturn, request, "CHD", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                            }
                            else if ((paxType == "INF" || paxType == "IN") && (paxTypeReturn == "INF" || paxTypeReturn == "IN"))
                            {
                                airFare.infFare = airFare.FindFareFromInformative(fare, fareReturn, request, "INF", markup, paymentFee, isPaymentFeePct, bCheckQTax);
                            }


                        }

                    }
                    if (request.noOfChildren > 0 && airFare.chdFare == null)
                    {
                        airFare.chdFare = airFare.adtFare;
                    }

                    if (request.noOfInfants > 0 && airFare.infFare == null)
                    {
                        airFare.infFare = airFare.adtFare;
                    }
                }
            }
            else
            {
                return null;
            }

            airFare.fareRules = new List<FareRule>();
            //Fare Rule
            bool bHaveFareRuleDepartFromConfig = false;
            bool bHaveFareRuleReturnFromConfig = false;
            FlightFareRuleConfigServices fareRuleConfig = new FlightFareRuleConfigServices(_unitOfWork);
            List<Entities.FareRuleConfig.FareRuleEntities> fareRuleAll = fareRuleConfig.GetAll();
            List<FlightFareRuleConfig> ruleConfigs = new List<FlightFareRuleConfig>();
            if (fareRuleAll != null && fareRuleAll.Count > 0)
            {
                if (segment_Depart.Count == 1)
                {
                    depCountry = namingServices.GetCountryCode(segment_Depart[0].boardPoint);
                    arrCountry = namingServices.GetCountryCode(segment_Depart[0].offPoint);
                    fareRuleAll = fareRuleAll.FindAll(x => x.fareRule.AirlineCodes == segment_Depart[0].marketingCompany).ToList();
                    foreach (Entities.FareRuleConfig.FareRuleEntities ruleEntities in fareRuleAll)
                    {

                        ruleConfigs = fareRuleConfig.GetRuleConfigs(segment_Depart[0], ruleEntities, depCountry, arrCountry);
                        if (ruleConfigs != null && ruleConfigs.Count > 0)
                        {
                            var fareruleDetail = ruleConfigs.Find(x => x.RBD.IndexOf(segment_Depart[0].rbd) != -1 && x.FareBasis.IndexOf(segment_Depart[0].fareBasis) != -1);
                            if (fareruleDetail == null)
                            {
                                fareruleDetail = ruleConfigs.Find(x => x.RBD.IndexOf(segment_Depart[0].rbd) != -1 && x.FareBasis == "*");
                            }
                            if (fareruleDetail == null)
                            {
                                fareruleDetail = ruleConfigs.Find(x => x.RBD == "*" && x.FareBasis.IndexOf(segment_Depart[0].fareBasis) != -1);
                            }
                            if (fareruleDetail == null)
                            {
                                fareruleDetail = ruleConfigs.Find(x => x.RBD == "*" && x.FareBasis == "*");
                            }
                            if (fareruleDetail != null)
                            {
                                bHaveFareRuleDepartFromConfig = true;
                                FareRule fareRule = new FareRule();
                                fareRule.origin = new City(namingServices, languageCode);
                                fareRule.origin.code = segment_Depart[0].boardPoint;
                                fareRule.destination = new City(namingServices, languageCode);
                                fareRule.destination.code = segment_Depart[0].offPoint;
                                fareRule.fareBasis = segment_Depart[0].fareBasis;
                                fareRule.rules = new List<FareRuleDatail>();
                                FareRuleDatail r = new FareRuleDatail();
                                string[] arrRule = ruleEntities.fareRuleDetails.Find(x => x.LanguageCode == languageCode).FareRule.Replace("\r\n", "|").Split('|');
                                r.fareRuleText = new List<string>();
                                foreach (var _text in arrRule)
                                {
                                    r.fareRuleText.Add(_text);
                                }
                                fareRule.rules.Add(r);
                                airFare.fareRules.Add(fareRule);
                            }
                        }
                    }
                }
                else
                {
                    foreach (SegmentForGetFareRule segment in segment_Depart)
                    {
                        depCountry = namingServices.GetCountryCode(segment.boardPoint);
                        arrCountry = namingServices.GetCountryCode(segment.offPoint);
                        fareRuleAll = fareRuleAll.FindAll(x => x.fareRule.AirlineCodes == segment.marketingCompany).ToList();
                        foreach (Entities.FareRuleConfig.FareRuleEntities ruleEntities in fareRuleAll)
                        {

                            ruleConfigs = fareRuleConfig.GetRuleConfigs(segment, ruleEntities, depCountry, arrCountry);
                            if (ruleConfigs != null && ruleConfigs.Count > 0)
                            {
                                var fareruleDetail = ruleConfigs.Find(x => x.RBD.IndexOf(segment.rbd) != -1 && x.FareBasis.IndexOf(segment.fareBasis) != -1);
                                if (fareruleDetail == null)
                                {
                                    fareruleDetail = ruleConfigs.Find(x => x.RBD.IndexOf(segment.rbd) != -1 && x.FareBasis == "*");
                                }
                                if (fareruleDetail == null)
                                {
                                    fareruleDetail = ruleConfigs.Find(x => x.RBD == "*" && x.FareBasis.IndexOf(segment.fareBasis) != -1);
                                }
                                if (fareruleDetail == null)
                                {
                                    fareruleDetail = ruleConfigs.Find(x => x.RBD == "*" && x.FareBasis == "*");
                                }
                                if (fareruleDetail != null)
                                {
                                    bHaveFareRuleDepartFromConfig = true;
                                    FareRule fareRule = new FareRule();
                                    fareRule.origin = new City(namingServices, languageCode);
                                    fareRule.origin.code = segment.boardPoint;
                                    fareRule.destination = new City(namingServices, languageCode);
                                    fareRule.destination.code = segment.offPoint;
                                    fareRule.fareBasis = segment.fareBasis;
                                    fareRule.rules = new List<FareRuleDatail>();
                                    FareRuleDatail r = new FareRuleDatail();

                                    string[] arrRule = ruleEntities.fareRuleDetails.Find(x => x.LanguageCode == languageCode).FareRule.Replace("\r\n", "|").Split('|');
                                    r.fareRuleText = new List<string>();
                                    foreach (var _text in arrRule)
                                    {
                                        r.fareRuleText.Add(_text);
                                    }
                                    fareRule.rules.Add(r);
                                    airFare.fareRules.Add(fareRule);
                                }
                            }
                        }
                    }
                }

                if (segment_Return != null && segment_Return.Count > 0)
                {
                    fareRuleAll = fareRuleConfig.GetAll();
                    if (segment_Return.Count == 1)
                    {
                        depCountry = namingServices.GetCountryCode(segment_Return[0].boardPoint);
                        arrCountry = namingServices.GetCountryCode(segment_Return[0].offPoint);
                        fareRuleAll = fareRuleAll.FindAll(x => x.fareRule.AirlineCodes == segment_Return[0].marketingCompany).ToList();
                        foreach (Entities.FareRuleConfig.FareRuleEntities ruleEntities in fareRuleAll)
                        {

                            ruleConfigs = fareRuleConfig.GetRuleConfigs(segment_Return[0], ruleEntities, depCountry, arrCountry);
                            if (ruleConfigs != null && ruleConfigs.Count > 0)
                            {
                                var fareruleDetail = ruleConfigs.Find(x => x.RBD.IndexOf(segment_Return[0].rbd) != -1 && x.FareBasis.IndexOf(segment_Return[0].fareBasis) != -1);
                                if (fareruleDetail == null)
                                {
                                    fareruleDetail = ruleConfigs.Find(x => x.RBD.IndexOf(segment_Return[0].rbd) != -1 && x.FareBasis == "*");
                                }
                                if (fareruleDetail == null)
                                {
                                    fareruleDetail = ruleConfigs.Find(x => x.RBD == "*" && x.FareBasis.IndexOf(segment_Return[0].fareBasis) != -1);
                                }
                                if (fareruleDetail == null)
                                {
                                    fareruleDetail = ruleConfigs.Find(x => x.RBD == "*" && x.FareBasis == "*");
                                }
                                if (fareruleDetail != null)
                                {
                                    bHaveFareRuleReturnFromConfig = true;
                                    FareRule fareRule = new FareRule();
                                    fareRule.origin = new City(namingServices, languageCode);
                                    fareRule.origin.code = segment_Return[0].boardPoint;
                                    fareRule.destination = new City(namingServices, languageCode);
                                    fareRule.destination.code = segment_Return[0].offPoint;
                                    fareRule.fareBasis = segment_Return[0].fareBasis;
                                    fareRule.rules = new List<FareRuleDatail>();
                                    FareRuleDatail r = new FareRuleDatail();
                                    string[] arrRule = ruleEntities.fareRuleDetails.Find(x => x.LanguageCode == languageCode).FareRule.Replace("\r\n", "|").Split('|');
                                    r.fareRuleText = new List<string>();
                                    foreach (var _text in arrRule)
                                    {
                                        r.fareRuleText.Add(_text);
                                    }
                                    fareRule.rules.Add(r);
                                    airFare.fareRules.Add(fareRule);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (SegmentForGetFareRule segment in segment_Return)
                        {
                            depCountry = namingServices.GetCountryCode(segment.boardPoint);
                            arrCountry = namingServices.GetCountryCode(segment.offPoint);
                            fareRuleAll = fareRuleAll.FindAll(x => x.fareRule.AirlineCodes == segment.marketingCompany).ToList();
                            foreach (Entities.FareRuleConfig.FareRuleEntities ruleEntities in fareRuleAll)
                            {

                                ruleConfigs = fareRuleConfig.GetRuleConfigs(segment, ruleEntities, depCountry, arrCountry);
                                if (ruleConfigs != null && ruleConfigs.Count > 0)
                                {
                                    var fareruleDetail = ruleConfigs.Find(x => x.RBD.IndexOf(segment.rbd) != -1 && x.FareBasis.IndexOf(segment.fareBasis) != -1);
                                    if (fareruleDetail == null)
                                    {
                                        fareruleDetail = ruleConfigs.Find(x => x.RBD.IndexOf(segment.rbd) != -1 && x.FareBasis == "*");
                                    }
                                    if (fareruleDetail == null)
                                    {
                                        fareruleDetail = ruleConfigs.Find(x => x.RBD == "*" && x.FareBasis.IndexOf(segment.fareBasis) != -1);
                                    }
                                    if (fareruleDetail == null)
                                    {
                                        fareruleDetail = ruleConfigs.Find(x => x.RBD == "*" && x.FareBasis == "*");
                                    }
                                    if (fareruleDetail != null)
                                    {
                                        bHaveFareRuleReturnFromConfig = true;
                                        FareRule fareRule = new FareRule();
                                        fareRule.origin = new City(namingServices, languageCode);
                                        fareRule.origin.code = segment.boardPoint;
                                        fareRule.destination = new City(namingServices, languageCode);
                                        fareRule.destination.code = segment.offPoint;
                                        fareRule.fareBasis = segment.fareBasis;
                                        fareRule.rules = new List<FareRuleDatail>();
                                        FareRuleDatail r = new FareRuleDatail();
                                        string[] arrRule = ruleEntities.fareRuleDetails.Find(x => x.LanguageCode == languageCode).FareRule.Replace("\r\n", "|").Split('|');
                                        r.fareRuleText = new List<string>();
                                        foreach (var _text in arrRule)
                                        {
                                            r.fareRuleText.Add(_text);
                                        }
                                        fareRule.rules.Add(r);
                                        airFare.fareRules.Add(fareRule);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!bHaveFareRuleDepartFromConfig)
            {
                for (int i = 0; i < fareBasis.Count; i++)
                {
                    Entities.FareRule.Request fareRuleRequest = new Entities.FareRule.Request();
                    fareRuleRequest.bookingOID = requestForDepart1A.bookingOID;
                    fareRuleRequest.session = new Entities.FareRule.Session();
                    fareRuleRequest.session.isStateFull = true;
                    fareRuleRequest.session.InSeries = true;
                    fareRuleRequest.session.End = false;
                    fareRuleRequest.session.SessionId = pricingResponse.FareInformativePricingWithoutPnrReply.Session.SessionId;
                    fareRuleRequest.session.SequenceNumber = (int.Parse(pricingResponse.FareInformativePricingWithoutPnrReply.Session.SequenceNumber) + 1).ToString();
                    fareRuleRequest.session.SecurityToken = pricingResponse.FareInformativePricingWithoutPnrReply.Session.SecurityToken;

                    fareRuleRequest.categoryCodes = new List<string>();
                    fareRuleRequest.categoryCodes.Add("PE");
                    fareRuleRequest.categoryCodes.Add("SR");
                    fareRuleRequest.categoryCodes.Add("AP");
                    fareRuleRequest.categoryCodes.Add("MN");
                    fareRuleRequest.categoryCodes.Add("MX");
                    fareRuleRequest.componentNo = (i + 1).ToString();
                    fareRuleRequest.ClientIP = Utilities.HttpUtility.GetIPAddress();
                    url = ConfigurationManager.AppSettings["FARERULE.URL"];
                    requestJson = JsonConvert.SerializeObject(fareRuleRequest);

                    json = HttpUtility.postJSON(url, requestJson);

                    Log.Info("FARE_RULE");
                    Log.Debug(requestJson);
                    Log.Debug(json);

                    BL.Entities.FareRule.Response ruleResponse = new Entities.FareRule.Response();
                    if (String.IsNullOrEmpty(json) == false)
                    {
                        json = json.Replace("@", "");
                        ruleResponse = BL.Entities.FareRule.Response.FromJson(json);
                    }

                    if (ruleResponse.Error == null && (ruleResponse.FareCheckRulesReply.TariffInfo.PurpleTariffInfo != null || ruleResponse.FareCheckRulesReply.TariffInfo.TariffInfoElementArray != null))
                    {
                        Entities.RobinhoodFare.FareRule fareRule = new Entities.RobinhoodFare.FareRule();
                        fareRule.origin = new City(namingServices, languageCode);
                        fareRule.origin.code = ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails != null ? ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails.OdiGrp.OriginDestination.Origin : ruleResponse.FareCheckRulesReply.FlightDetails.FlightDetailsElementArray[0].OdiGrp.OriginDestination.Origin;
                        fareRule.destination = new City(namingServices, languageCode);
                        fareRule.destination.code = ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails != null ? ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails.OdiGrp.OriginDestination.Destination : ruleResponse.FareCheckRulesReply.FlightDetails.FlightDetailsElementArray[0].OdiGrp.OriginDestination.Destination;
                        fareRule.fareBasis = ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails != null ? ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails.QualificationFareDetails.AdditionalFareDetails.FareClass : ruleResponse.FareCheckRulesReply.FlightDetails.FlightDetailsElementArray[0].QualificationFareDetails.AdditionalFareDetails.FareClass;
                        fareRule.rules = new List<Entities.RobinhoodFare.FareRuleDatail>();
                        if (ruleResponse.FareCheckRulesReply.TariffInfo.PurpleTariffInfo != null)
                        {
                            Entities.RobinhoodFare.FareRuleDatail r = new Entities.RobinhoodFare.FareRuleDatail();
                            r.fareRuleText = new List<string>();
                            foreach (var text in ruleResponse.FareCheckRulesReply.TariffInfo.PurpleTariffInfo.FareRuleText)
                            {
                                r.fareRuleText.Add(text.FreeText);
                            }
                            fareRule.rules.Add(r);
                        }
                        else
                        {
                            if (ruleResponse.FareCheckRulesReply.TariffInfo.TariffInfoElementArray != null)
                            {
                                foreach (var info in ruleResponse.FareCheckRulesReply.TariffInfo.TariffInfoElementArray)
                                {
                                    Entities.RobinhoodFare.FareRuleDatail r = new Entities.RobinhoodFare.FareRuleDatail();
                                    r.fareRuleText = new List<string>();
                                    foreach (var text in info.FareRuleText)
                                    {
                                        r.fareRuleText.Add(text.FreeText);
                                    }
                                    fareRule.rules.Add(r);
                                }
                            }
                        }
                        airFare.fareRules.Add(fareRule);

                    }

                }
            }
            if (!bHaveFareRuleReturnFromConfig)
            {
                for (int i = 0; i < fareBasis_Return.Count; i++)
                {
                    Entities.FareRule.Request fareRuleRequest = new Entities.FareRule.Request();
                    fareRuleRequest.bookingOID = requestForReturn1A.bookingOID;
                    fareRuleRequest.session = new Entities.FareRule.Session();
                    fareRuleRequest.session.isStateFull = true;
                    fareRuleRequest.session.InSeries = true;
                    fareRuleRequest.session.End = false;
                    fareRuleRequest.session.SessionId = pricingReturnResponse.FareInformativePricingWithoutPnrReply.Session.SessionId;
                    fareRuleRequest.session.SequenceNumber = (int.Parse(pricingReturnResponse.FareInformativePricingWithoutPnrReply.Session.SequenceNumber) + 1).ToString();
                    fareRuleRequest.session.SecurityToken = pricingReturnResponse.FareInformativePricingWithoutPnrReply.Session.SecurityToken;

                    fareRuleRequest.categoryCodes = new List<string>();
                    fareRuleRequest.categoryCodes.Add("PE");
                    fareRuleRequest.categoryCodes.Add("SR");
                    fareRuleRequest.categoryCodes.Add("AP");
                    fareRuleRequest.categoryCodes.Add("MN");
                    fareRuleRequest.categoryCodes.Add("MX");
                    fareRuleRequest.componentNo = (i + 1).ToString();
                    fareRuleRequest.ClientIP = Utilities.HttpUtility.GetIPAddress();
                    url = ConfigurationManager.AppSettings["FARERULE.URL"];
                    requestJson = JsonConvert.SerializeObject(fareRuleRequest);

                    json = HttpUtility.postJSON(url, requestJson);

                    Log.Info("FARE_RULE_RETURN");
                    Log.Debug(requestJson);
                    Log.Debug(json);

                    BL.Entities.FareRule.Response ruleResponse = new Entities.FareRule.Response();
                    if (String.IsNullOrEmpty(json) == false)
                    {
                        json = json.Replace("@", "");
                        ruleResponse = BL.Entities.FareRule.Response.FromJson(json);
                    }

                    if (ruleResponse.Error == null && (ruleResponse.FareCheckRulesReply.TariffInfo.PurpleTariffInfo != null || ruleResponse.FareCheckRulesReply.TariffInfo.TariffInfoElementArray != null))
                    {
                        Entities.RobinhoodFare.FareRule fareRule = new Entities.RobinhoodFare.FareRule();
                        fareRule.origin = new City(namingServices, languageCode);
                        fareRule.origin.code = ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails != null ? ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails.OdiGrp.OriginDestination.Origin : ruleResponse.FareCheckRulesReply.FlightDetails.FlightDetailsElementArray[0].OdiGrp.OriginDestination.Origin;
                        fareRule.destination = new City(namingServices, languageCode);
                        fareRule.destination.code = ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails != null ? ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails.OdiGrp.OriginDestination.Destination : ruleResponse.FareCheckRulesReply.FlightDetails.FlightDetailsElementArray[0].OdiGrp.OriginDestination.Destination;
                        fareRule.fareBasis = ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails != null ? ruleResponse.FareCheckRulesReply.FlightDetails.PurpleFlightDetails.QualificationFareDetails.AdditionalFareDetails.FareClass : ruleResponse.FareCheckRulesReply.FlightDetails.FlightDetailsElementArray[0].QualificationFareDetails.AdditionalFareDetails.FareClass;
                        fareRule.rules = new List<Entities.RobinhoodFare.FareRuleDatail>();
                        if (ruleResponse.FareCheckRulesReply.TariffInfo.PurpleTariffInfo != null)
                        {
                            Entities.RobinhoodFare.FareRuleDatail r = new Entities.RobinhoodFare.FareRuleDatail();
                            r.fareRuleText = new List<string>();
                            foreach (var text in ruleResponse.FareCheckRulesReply.TariffInfo.PurpleTariffInfo.FareRuleText)
                            {
                                r.fareRuleText.Add(text.FreeText);
                            }
                            fareRule.rules.Add(r);
                        }
                        else
                        {
                            if (ruleResponse.FareCheckRulesReply.TariffInfo.TariffInfoElementArray != null)
                            {
                                foreach (var info in ruleResponse.FareCheckRulesReply.TariffInfo.TariffInfoElementArray)
                                {
                                    Entities.RobinhoodFare.FareRuleDatail r = new Entities.RobinhoodFare.FareRuleDatail();
                                    r.fareRuleText = new List<string>();
                                    foreach (var text in info.FareRuleText)
                                    {
                                        r.fareRuleText.Add(text.FreeText);
                                    }
                                    fareRule.rules.Add(r);
                                }
                            }
                        }
                        airFare.fareRules.Add(fareRule);

                    }

                }
            }
            airFare.SetDisplayFlight(request);

            if (request.tripType == "R")
            {
                TimeSpan ts = request.returnFlights[0].departureDateTime - request.departureFlights[0].departureDateTime;
                int day = ts.Days;
                NamingServices namingService = new NamingServices(_unitOfWork);
                string destinationCountry = namingService.GetCountryCode(request.destination);
                string destinationCountryName = namingService.GetCountryName(destinationCountry, languageCode);


            }

            PassportServices passportServices = new PassportServices(_unitOfWork);
            List<PassportConfig> passList = passportServices.GetAll();
            bool isRequirePassport = false;
            string sOrigin = "";
            string sDestination = "";
            PassportConfig pass = new PassportConfig();
            
            foreach (var segment in request.departureFlights)
            {
                sOrigin = namingServices.GetCountryCode(requestForDepart1A.origin);
                sDestination = namingServices.GetCountryCode(requestForDepart1A.destination);
                pass = passList.Find(x => (x.AirlineCodes == segment.companyCode || x.AirlineCodes == "YY") && (x.OriginCodes.IndexOf(sOrigin) != -1 || x.OriginCodes == "*") && (x.DestinationCodes.IndexOf(sDestination) != -1 || x.DestinationCodes == "*"));
                if (pass != null)
                {
                    isRequirePassport = pass.IsActive.Value;
                }
                if (!isRequirePassport)
                {
                    sOrigin = namingServices.GetCountryCode(segment.departureCity);
                    sDestination = namingServices.GetCountryCode(segment.arrivalCity);
                    pass = passList.Find(x => (x.AirlineCodes == segment.companyCode || x.AirlineCodes == "YY") && (x.OriginCodes.IndexOf(sOrigin) != -1 || x.OriginCodes == "*") && (x.DestinationCodes.IndexOf(sDestination) != -1 || x.DestinationCodes == "*"));
                    if (pass != null)
                    {
                        isRequirePassport = pass.IsActive.Value;
                    }
                }
                if (isRequirePassport)
                {
                    break;
                }
            }
            if (!isRequirePassport && request.returnFlights != null && request.returnFlights.Count > 0)
            {
                foreach (var segment in request.returnFlights)
                {
                    sOrigin = namingServices.GetCountryCode(requestForReturn1A.origin);
                    sDestination = namingServices.GetCountryCode(requestForReturn1A.destination);
                    pass = passList.Find(x => (x.AirlineCodes == segment.companyCode || x.AirlineCodes == "YY") && (x.OriginCodes.IndexOf(sOrigin) != -1 || x.OriginCodes == "*") && (x.DestinationCodes.IndexOf(sDestination) != -1 || x.DestinationCodes == "*"));
                    if (pass != null)
                    {
                        isRequirePassport = pass.IsActive.Value;
                    }
                    if (!isRequirePassport)
                    {
                        sOrigin = namingServices.GetCountryCode(segment.departureCity);
                        sDestination = namingServices.GetCountryCode(segment.arrivalCity);
                        pass = passList.Find(x => (x.AirlineCodes == segment.companyCode || x.AirlineCodes == "YY") && (x.OriginCodes.IndexOf(sOrigin) != -1 || x.OriginCodes == "*") && (x.DestinationCodes.IndexOf(sDestination) != -1 || x.DestinationCodes == "*"));
                        if (pass != null)
                        {
                            isRequirePassport = pass.IsActive.Value;
                        }
                    }
                    if (isRequirePassport)
                    {
                        break;
                    }
                }
            }
            airFare.isPassportRequired = isRequirePassport;
            airFare.isPricingWithSegment = true;
            return airFare;
        }
        private List<string> GetFareBasis(SegmentLevelGroupUnion segmentLevelGroup)
        {
            List<string> fareBasis = new List<string>();
            if (segmentLevelGroup.PurpleSegmentLevelGroup != null)
            {
                fareBasis.Add(segmentLevelGroup.PurpleSegmentLevelGroup.FareBasis.AdditionalFareDetails.RateClass);
            }
            else
            {
                foreach (var segment in segmentLevelGroup.SegmentLevelGroupElementArray)
                {
                    if (fareBasis.FirstOrDefault(x => x == segment.FareBasis.AdditionalFareDetails.RateClass) == null)
                    {
                        fareBasis.Add(segment.FareBasis.AdditionalFareDetails.RateClass);
                    }
                }
            }
            return fareBasis;
        }
        private List<string> GetFareBasis(SegmentLevelGroupUnion segmentLevelGroup, ref List<SegmentForGetFareRule> segment_Route)
        {
            List<string> fareBasis = new List<string>();
            if (segmentLevelGroup.PurpleSegmentLevelGroup != null)
            {
                SegmentForGetFareRule _seg = new SegmentForGetFareRule();
                _seg.boardPoint = segmentLevelGroup.PurpleSegmentLevelGroup.SegmentInformation.BoardPointDetails.TrueLocationId;
                _seg.offPoint = segmentLevelGroup.PurpleSegmentLevelGroup.SegmentInformation.OffpointDetails.TrueLocationId;
                _seg.marketingCompany = segmentLevelGroup.PurpleSegmentLevelGroup.SegmentInformation.CompanyDetails.MarketingCompany;
                _seg.fareBasis = segmentLevelGroup.PurpleSegmentLevelGroup.FareBasis.AdditionalFareDetails.RateClass;
                _seg.rbd = segmentLevelGroup.PurpleSegmentLevelGroup.SegmentInformation.FlightIdentification.BookingClass;
                segment_Route.Add(_seg);

                fareBasis.Add(segmentLevelGroup.PurpleSegmentLevelGroup.FareBasis.AdditionalFareDetails.RateClass);
            }
            else
            {
                foreach (var segment in segmentLevelGroup.SegmentLevelGroupElementArray)
                {
                    SegmentForGetFareRule _seg = new SegmentForGetFareRule();
                    _seg.boardPoint = segment.SegmentInformation.BoardPointDetails.TrueLocationId;
                    _seg.offPoint = segment.SegmentInformation.OffpointDetails.TrueLocationId;
                    _seg.marketingCompany = segment.SegmentInformation.CompanyDetails.MarketingCompany;
                    _seg.fareBasis = segment.FareBasis.AdditionalFareDetails.RateClass;
                    _seg.rbd = segment.SegmentInformation.FlightIdentification.BookingClass;
                    segment_Route.Add(_seg);
                    if (fareBasis.FirstOrDefault(x => x == segment.FareBasis.AdditionalFareDetails.RateClass) == null)
                    {
                        fareBasis.Add(segment.FareBasis.AdditionalFareDetails.RateClass);
                    }
                }
            }
            return fareBasis;
        }

        public Entities.RobinhoodFare.AirFare GhostPricing(Entities.AirSell.Request request, string languageCode)
        {
            string url = ConfigurationManager.AppSettings["AIRSELL.URL"];
            string requestJson = JsonConvert.SerializeObject(request);

            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("AIR_SELL");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.AirSell.Response airSellResponse = new Entities.AirSell.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");
                airSellResponse = BL.Entities.AirSell.Response.FromJson(json);
            }

            if (airSellResponse.Error != null)
            {
                return null;
            }

            //Ghost PNR
            Entities.PNR.Request pnrReuest = new Entities.PNR.Request();
            pnrReuest.session = new Entities.PNR.Session();
            pnrReuest.session.isStateFull = true;
            pnrReuest.session.InSeries = true;
            pnrReuest.session.End = false;
            pnrReuest.session.SessionId = airSellResponse.AirSellFromRecommendationReply.Session.SessionId;
            pnrReuest.session.SequenceNumber = (int.Parse(airSellResponse.AirSellFromRecommendationReply.Session.SequenceNumber) + 1).ToString();
            pnrReuest.session.SecurityToken = airSellResponse.AirSellFromRecommendationReply.Session.SecurityToken;

            pnrReuest.passengerList = new List<Entities.PNR.PassengerList>();
            for (int i = 0; i < request.noOfAdults; i++)
            {
                Entities.PNR.PassengerList pax = new Entities.PNR.PassengerList();
                pax.paxNo = i + 1;
                pax.infantFlag = request.noOfInfants >= (i + 1);
                pax.haveDOB = false;
                pax.DOB = new DateTime();
                pax.age = 25;
                pax.withADT = 0;
                pax.ptNo = "";
                pax.titleName = "MR";
                pax.firstName = "FIRSTNAME";
                pax.lastName = "LASTNAME";
                pax.middleName = "";
                pax.paxType = "ADT";
                pax.paxID = (i + 1).ToString();
                pax.withInsurance = false;

                if (request.noOfInfants >= (i + 1))
                {
                    pax.infantPassenger = new Entities.PNR.InfantPassenger();
                    pax.infantPassenger.paxNo = request.noOfAdults + request.noOfChildren + i + 1;
                    pax.infantPassenger.infantFlag = false;
                    pax.infantPassenger.haveDOB = true;
                    pax.infantPassenger.DOB = request.departureFlights[0].departureDateTime.AddYears(-1);
                    pax.infantPassenger.age = 1;
                    pax.infantPassenger.withADT = 0;
                    pax.infantPassenger.titleName = "";
                    pax.infantPassenger.firstName = "FIRSTNAME";
                    pax.infantPassenger.lastName = "LASTNAME";
                    pax.infantPassenger.paxType = "INF";
                    pax.infantPassenger.paxID = (request.noOfAdults + request.noOfChildren + i + 1).ToString();
                }

                pnrReuest.passengerList.Add(pax);
            }

            for (int i = 0; i < request.noOfChildren; i++)
            {
                Entities.PNR.PassengerList pax = new Entities.PNR.PassengerList();
                pax.paxNo = i + request.noOfAdults;
                pax.infantFlag = false;
                pax.haveDOB = true;
                pax.DOB = request.departureFlights[0].departureDateTime.AddYears(-8);
                pax.age = 8;
                pax.withADT = 0;
                pax.ptNo = "";
                pax.titleName = "MISS";
                pax.firstName = "FIRSTNAME";
                pax.lastName = "LASTNAME";
                pax.middleName = "";
                pax.paxType = "CHD";
                pax.withInsurance = false;
                pax.paxID = (i + request.noOfAdults).ToString();

                pnrReuest.passengerList.Add(pax);
            }
            url = ConfigurationManager.AppSettings["PNR.URL"];
            requestJson = JsonConvert.SerializeObject(pnrReuest);

            json = HttpUtility.postJSON(url, requestJson);

            Log.Info("GHOST_PNR");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrResponse = BL.Entities.PNR.Response.FromJson(json);
            }

            if (pnrResponse.Error != null)
            {
                return null;
            }

            //Pricing
            Entities.Pricing.Request pricingReuest = new Entities.Pricing.Request();
            pricingReuest.session = new Entities.Pricing.Session();
            pricingReuest.session.isStateFull = true;
            pricingReuest.session.InSeries = true;
            pricingReuest.session.End = true;
            pricingReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
            pricingReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
            pricingReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;

            pricingReuest.pricingTicketingIndicators = new List<string>();
            pricingReuest.pricingTicketingIndicators.Add("RU");
            pricingReuest.pricingTicketingIndicators.Add("RP");
            pricingReuest.pricingTicketingIndicators.Add("RLO");
            pricingReuest.useCityOverride = false;
            pricingReuest.useCurrencyOverride = true;
            pricingReuest.currencyCode = "THB";

            url = ConfigurationManager.AppSettings["PRICING.URL"];
            requestJson = JsonConvert.SerializeObject(pricingReuest);

            json = HttpUtility.postJSON(url, requestJson);

            Log.Info("GHOST_PRICING");
            Log.Debug(requestJson);
            Log.Debug(json);

            BL.Entities.Pricing.Response pricingResponse = new Entities.Pricing.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pricingResponse = BL.Entities.Pricing.Response.FromJson(json);
            }

            if (pricingResponse.Error != null)
            {
                return null;
            }
            NamingServices namingServices = new NamingServices(_unitOfWork);
            Entities.RobinhoodFare.AirFare airFare = new Entities.RobinhoodFare.AirFare(namingServices, languageCode);
            if (pricingResponse.FarePricePnrWithBookingClassReply.FareList.PurpleFareList != null)
            {
                airFare.adtFare = airFare.FindFare(pricingResponse.FarePricePnrWithBookingClassReply.FareList.PurpleFareList);
            }
            else if (pricingResponse.FarePricePnrWithBookingClassReply.FareList.FareListElementArray.Count > 0)
            {
                foreach (var fare in pricingResponse.FarePricePnrWithBookingClassReply.FareList.FareListElementArray)
                {
                    string paxType = "";
                    if (fare.SegmentInformation.PurpleSegmentInformation != null)
                    {
                        paxType = fare.SegmentInformation.PurpleSegmentInformation.FareQualifier.FareBasisDetails.DiscTktDesignator.ToUpper();
                    }
                    else
                    {
                        paxType = fare.SegmentInformation.SegmentInformationElementArray[0].FareQualifier.FareBasisDetails.DiscTktDesignator.ToUpper();

                    }

                    if (paxType == "ADT")
                    {
                        airFare.adtFare = airFare.FindFare(fare);
                    }
                    else if (paxType == "CHD" || paxType == "CH")
                    {
                        airFare.chdFare = airFare.FindFare(fare);
                    }
                    else if (paxType == "INF" || paxType == "IN")
                    {
                        airFare.infFare = airFare.FindFare(fare);
                    }
                }
            }
            else
            {
                return null;
            }
            airFare.SetDisplayFlight(request);
            return airFare;
        }

        public Entities.RobinhoodPNR.PNR BookingFromAN(Entities.AirMultiAvailability.SavePNRRequest request)
        {
            Entities.RobinhoodPNR.PNR response = new Entities.RobinhoodPNR.PNR();
            SiteConfigServices siteConfig = new SiteConfigServices(_unitOfWork);
            var dataConfig = siteConfig.GetByKey("OfficeID");
            request.bookingOID = dataConfig.ConfigValue;// ConfigurationManager.AppSettings["OfficeID"];



            #region AirSell
            string url = ConfigurationManager.AppSettings["AIRSELL.URL"];
            Entities.AirSell.Request airSellRequest = request.GetAirSellRequest();
            string requestJson = JsonConvert.SerializeObject(airSellRequest);

            string json = HttpUtility.postJSON(url, requestJson);
            Log.Info("AIR_SELL");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.AirSell.Response airSellResponse = new Entities.AirSell.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");
                airSellResponse = BL.Entities.AirSell.Response.FromJson(json);
            }

            if (airSellResponse.Error != null)
            {
                response.isError = true;
                response.errorMessage = airSellResponse.Error.ErrorMessage + "(" + airSellResponse.Error.ErrorCode + ")";
                return response;
            }
            bool isNotConfirmStatus = false;
            if (airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail != null)
            {
                if (airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation != null)
                {
                    string status = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.ActionDetails.StatusCode;
                    if (status != "HK" && status != "OK")
                    {
                        isNotConfirmStatus = true;
                    }
                }
                else
                {
                    foreach (var itin in airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.SegmentInformationElementArray)
                    {
                        string status = itin.ActionDetails.StatusCode;
                        if (status != "HK" && status != "OK")
                        {
                            isNotConfirmStatus = true;
                        }
                    }
                }
            }
            else
            {
                if (airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.ItineraryDetailElementArray[0].SegmentInformation.PurpleSegmentInformation != null)
                {
                    string status = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.ItineraryDetailElementArray[0].SegmentInformation.PurpleSegmentInformation.ActionDetails.StatusCode;
                    if (status != "HK" && status != "OK")
                    {
                        isNotConfirmStatus = true;
                    }
                }
                else
                {
                    foreach (var itin in airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.ItineraryDetailElementArray[0].SegmentInformation.SegmentInformationElementArray)
                    {
                        string status = itin.ActionDetails.StatusCode;
                        if (status != "HK" && status != "OK")
                        {
                            isNotConfirmStatus = true;
                        }
                    }
                }
            }

            if (isNotConfirmStatus)
            {
                response.isError = true;
                response.errorMessage = "FLIGHTS NOT CONFIRM";
                return response;
            }
            #endregion AirSell

            //PNR
            Entities.PNR.Request pnrReuest = new Entities.PNR.Request();
            pnrReuest.session = new Entities.PNR.Session();
            pnrReuest.session.isStateFull = true;
            pnrReuest.session.InSeries = true;
            pnrReuest.session.End = false;
            pnrReuest.session.SessionId = airSellResponse.AirSellFromRecommendationReply.Session.SessionId;
            pnrReuest.session.SequenceNumber = (int.Parse(airSellResponse.AirSellFromRecommendationReply.Session.SequenceNumber) + 1).ToString();
            pnrReuest.session.SecurityToken = airSellResponse.AirSellFromRecommendationReply.Session.SecurityToken;
            pnrReuest.bookingOID = request.bookingOID;

            pnrReuest.fop = "CA";
            pnrReuest.remarkType = "RM";
            pnrReuest.remarks = new List<string>();

            dataConfig = siteConfig.GetByKey("TicketIndicator");
            pnrReuest.ticketIndicator = dataConfig.ConfigValue;//ConfigurationManager.AppSettings["TicketIndicator"]; //"TL";

            dataConfig = siteConfig.GetByKey("TicketTimeLimitHour");
            pnrReuest.ticketTimeLimitDate = DateTime.Now.AddMinutes(int.Parse(dataConfig.ConfigValue)); //request.TKTL; ConfigurationManager.AppSettings["TicketTimeLimitHour"]
            pnrReuest.passengerList = new List<Entities.PNR.PassengerList>();

            for (int i = 0; i < request.adtPaxs.Count; i++)
            {
                Entities.PNR.PassengerList pax = new Entities.PNR.PassengerList();
                pax.paxNo = i + 1;
                pax.infantFlag = request.infPaxs == null ? false : (request.infPaxs.Count >= (i + 1));
                pax.haveDOB = false;
                pax.DOB = new DateTime();
                pax.age = 25;
                pax.withADT = 0;
                pax.ptNo = "";
                pax.titleName = request.adtPaxs[i].title;
                pax.firstName = request.adtPaxs[i].firstname;
                pax.lastName = request.adtPaxs[i].lastname;
                pax.middleName = request.adtPaxs[i].middlename;
                pax.paxType = "ADT";
                pax.paxID = (i + 1).ToString();
                pax.withInsurance = false;

                if (request.infPaxs != null && request.infPaxs.Count >= (i + 1))
                {
                    pax.infantPassenger = new Entities.PNR.InfantPassenger();
                    pax.infantPassenger.paxNo = (request.adtPaxs == null ? 0 : request.adtPaxs.Count) + (request.chdPaxs == null ? 0 : request.chdPaxs.Count) + i + 1;
                    pax.infantPassenger.infantFlag = false;
                    pax.infantPassenger.haveDOB = true;
                    pax.infantPassenger.DOB = request.infPaxs[i].birthday;
                    pax.infantPassenger.age = 1;
                    pax.infantPassenger.withADT = 0;
                    pax.infantPassenger.titleName = "";
                    pax.infantPassenger.firstName = request.infPaxs[i].firstname;
                    pax.infantPassenger.lastName = request.infPaxs[i].lastname;
                    pax.infantPassenger.paxType = "INF";
                    pax.infantPassenger.paxID = ((request.adtPaxs == null ? 0 : request.adtPaxs.Count) + (request.chdPaxs == null ? 0 : request.chdPaxs.Count) + i + 1).ToString();
                }

                pnrReuest.passengerList.Add(pax);
            }

            if (request.chdPaxs != null)
            {
                for (int i = 0; i < request.chdPaxs.Count; i++)
                {
                    Entities.PNR.PassengerList pax = new Entities.PNR.PassengerList();
                    pax.paxNo = i + request.adtPaxs.Count;
                    pax.infantFlag = false;
                    pax.haveDOB = true;
                    pax.DOB = request.chdPaxs[i].birthday;
                    pax.age = 8;
                    pax.withADT = 0;
                    pax.ptNo = "";
                    pax.titleName = request.chdPaxs[i].title;
                    pax.firstName = request.chdPaxs[i].firstname;
                    pax.lastName = request.chdPaxs[i].lastname;
                    pax.middleName = request.chdPaxs[i].middlename;
                    pax.paxType = "CHD";
                    pax.withInsurance = false;
                    pax.paxID = (i + request.adtPaxs.Count).ToString();

                    pnrReuest.passengerList.Add(pax);
                }
            }
            pnrReuest.refEmail = String.IsNullOrEmpty(request.contactInfo.email) ? request.adtPaxs[0].email : request.contactInfo.email;
            pnrReuest.refMobileNumber = String.IsNullOrEmpty(request.contactInfo.telNo) ? request.adtPaxs[0].telNo : request.contactInfo.telNo;

            url = ConfigurationManager.AppSettings["PNR.URL"];
            requestJson = JsonConvert.SerializeObject(pnrReuest);

            json = HttpUtility.postJSON(url, requestJson);

            Log.Info("CREATE_PNR");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrResponse = BL.Entities.PNR.Response.FromJson(json);
            }

            if (pnrResponse.Error != null)
            {
                response.isError = true;
                response.errorMessage = pnrResponse.Error.ErrorMessage + "(" + pnrResponse.Error.ErrorCode + ")";
                return response;
            }


            //PNR
            Entities.PNR.Request pnrSavedReuest = new Entities.PNR.Request();
            pnrSavedReuest.session = new Entities.PNR.Session();
            pnrSavedReuest.session.isStateFull = true;
            pnrSavedReuest.session.InSeries = true;
            pnrSavedReuest.session.End = true;
            pnrSavedReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
            pnrSavedReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
            pnrSavedReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
            pnrSavedReuest.bookingOID = request.bookingOID;
            pnrSavedReuest.optionCode = "11";
            pnrSavedReuest.rfLongFreeText = "AISOFT";
            pnrSavedReuest.departFlightSegmentCount = request.depFlight.Count;
            pnrSavedReuest.returnFlightSegmentCount = request.retFlight == null ? 0 : request.retFlight.Count;
            pnrSavedReuest.accessOID = "";// "BKKIW38FO";
            pnrSavedReuest.ClientIP = Utilities.HttpUtility.GetIPAddress();

            url = ConfigurationManager.AppSettings["SAVEPNR.URL"];
            requestJson = JsonConvert.SerializeObject(pnrSavedReuest);

            json = HttpUtility.postJSON(url, requestJson);

            Log.Info("SAVE_PNR");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrSavedResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrSavedResponse = BL.Entities.PNR.Response.FromJson(json);
            }

            if (pnrSavedResponse.Error != null)
            {
                response.isError = true;
                response.errorMessage = pnrSavedResponse.Error.ErrorMessage + "(" + pnrSavedResponse.Error.ErrorCode + ")";

                return response;
            }

            response.isError = false;
            if (pnrSavedResponse.PnrReply.PnrHeader.PurplePnrHeader != null)
            {
                response.recordLocator = pnrSavedResponse.PnrReply.PnrHeader.PurplePnrHeader.ReservationInfo.Reservation.ControlNumber;
            }
            else
            {
                response.recordLocator = pnrSavedResponse.PnrReply.PnrHeader.PnrHeaderElementArray[0].ReservationInfo.Reservation.ControlNumber;
            }
            //response.bookingKeyReference = Guid.NewGuid().ToString();

            request.PNR = response.recordLocator;
            Entities.RobinhoodFare.AirFare airFare = new Entities.RobinhoodFare.AirFare();
            airFare.bookingDate = DateTime.Now;
            airFare.type = "A";//A:Amadeus
            airFare.isBundle = false;
            airFare.bookingOID = dataConfig.ConfigValue;// ConfigurationManager.AppSettings["OfficeID"];
            airFare.noOfAdults = request.noOfAdults;
            airFare.noOfChildren = request.noOfChildren;
            airFare.noOfInfants = request.noOfInfants;
            airFare.svc_class = request.svc_class;
            NamingServices namingServices = new NamingServices(_unitOfWork);
            airFare.origin = new City(namingServices, request.languageCode);
            airFare.origin.code = request.depFlight[0].depCity.code;

            airFare.destination = new City(namingServices, request.languageCode);
            airFare.destination.code = request.depFlight[request.depFlight.Count - 1].arrCity.code;

            if (request.adtPaxs.Count > 0)
            {
                airFare.adtPaxs = request.adtPaxs;
                airFare.adtFare = new Fare();
            }
            if (request.chdPaxs.Count > 0)
            {
                airFare.chdPaxs = request.chdPaxs;
                airFare.chdFare = new Fare();
            }
            if (request.infPaxs.Count > 0)
            {
                airFare.infPaxs = request.infPaxs;
                airFare.infFare = new Fare();
            }
            airFare.PNR = response.recordLocator;
            airFare.contactInfo = request.contactInfo;
            airFare.depFlight = new List<Entities.RobinhoodFlight.FlightDetail>();
            Entities.RobinhoodFlight.FlightDetail flightDetail = new Entities.RobinhoodFlight.FlightDetail();
            foreach (var flight in request.depFlight)
            {
                flightDetail = new Entities.RobinhoodFlight.FlightDetail();
                flightDetail.airline = flight.airline;
                flightDetail.operatedAirline = flight.operatedAirline;
                flightDetail.depCity = flight.depCity;
                flightDetail.arrCity = flight.arrCity;
                flightDetail.flightNumber = flight.flightNumber;
                flightDetail.rbd = flight.rbd;
                flightDetail.Seq = flight.Seq;
                flightDetail.departureDateTime = flight.departureDateTime;
                flightDetail.arrivalDateTime = flight.arrivalDateTime;
                airFare.depFlight.Add(flightDetail);
            }

            if (request.retFlight != null && request.retFlight.Count > 0)
            {
                airFare.retFlight = new List<Entities.RobinhoodFlight.FlightDetail>();
                foreach (var flight in request.retFlight)
                {
                    flightDetail = new Entities.RobinhoodFlight.FlightDetail();
                    flightDetail.airline = flight.airline;
                    flightDetail.operatedAirline = flight.operatedAirline;
                    flightDetail.depCity = flight.depCity;
                    flightDetail.arrCity = flight.arrCity;
                    flightDetail.flightNumber = flight.flightNumber;
                    flightDetail.rbd = flight.rbd;
                    flightDetail.Seq = flight.Seq;
                    flightDetail.departureDateTime = flight.departureDateTime;
                    flightDetail.arrivalDateTime = flight.arrivalDateTime;
                    airFare.retFlight.Add(flightDetail);
                }
            }



            FlightBookingServices bookingSvc = new FlightBookingServices(_unitOfWork);
            var bookingRef = bookingSvc.SaveBooking(airFare);
            response.bookingKeyReference = bookingRef.ToString();
            Log.Debug("bookingRef=" + bookingRef);
            FlightReportServices reportSvc = new FlightReportServices(_unitOfWork);
            var bookingTest = reportSvc.GetByID(bookingRef);
            response.RobinhoodID = bookingTest.RobinhoodID;

            return response;
        }

        public Entities.RobinhoodPNR.MultiTicketPNR BookingMultiTicket(Entities.RobinhoodFare.AirFare request)
        {
            RunningNumberServices runningSvc = new RunningNumberServices(_unitOfWork);
            int rbhID = runningSvc.GetNumber("FBOOKING" + DateTime.Today.ToString("yyMMdd"));
            string robinhoodID = "F" + DateTime.Today.ToString("yyMMdd") + rbhID.ToString().PadLeft(4, '0');
            SiteConfigServices siteConfig = new SiteConfigServices(_unitOfWork);
            var dataConfig = siteConfig.GetByKey("OfficeID_Save");
            request.bookingOID = dataConfig.ConfigValue;// ConfigurationManager.AppSettings["OfficeID"];
            Entities.RobinhoodPNR.MultiTicketPNR response = new Entities.RobinhoodPNR.MultiTicketPNR();
            response.RobinhoodID = robinhoodID;
            response.Booking = new List<Booking>();
            response.airSellEntities = null;
            response.type = "A";//A:Amadeus
            Entities.RobinhoodPNR.Booking _onebooking = new Booking();
            List<AirSellEntities> airSellEntities = new List<AirSellEntities>();
            AirSellEntities _airSellEntities = new AirSellEntities();
            FlightBookingServices flightBookingServices = new FlightBookingServices(_unitOfWork);
            FlightBooking oneBooking = new FlightBooking();
            if (request.isPricingWithSegment)
            {
                for (int iR = 0; iR < 2; iR++)
                {
                    _onebooking = new Booking();
                    string url = ConfigurationManager.AppSettings["AIRSELL.URL"];
                    Entities.AirSell.Request airSellRequest = iR == 0 ? request.GetAirSellDepartRequest() : request.GetAirSellReturnRequest();
                    string requestJson = JsonConvert.SerializeObject(airSellRequest);

                    string json = HttpUtility.postJSON(url, requestJson);
                    Log.Info("AIR_SELL" + (iR == 0 ? " DEPART" : " RETURN"));
                    Log.Debug(requestJson);
                    Log.Debug(json);
                    BL.Entities.AirSell.Response airSellResponse = new Entities.AirSell.Response();
                    if (String.IsNullOrEmpty(json) == false)
                    {
                        json = json.Replace("@", "");
                        airSellResponse = BL.Entities.AirSell.Response.FromJson(json);
                    }

                    if (airSellResponse.Error != null)
                    {
                        response.isError = true;
                        response.errorCode = "4007";
                        response.errorMessage = airSellResponse.Error.ErrorMessage + "(" + airSellResponse.Error.ErrorCode + ")";//(288)
                        if (response.Booking != null && response.Booking.Count > 0 && response.Booking[0] != null && response.Booking[0].recordLocator.Length > 0)
                        {
                            Entities.PNRCancel.Request _cancel = new Entities.PNRCancel.Request();
                            _cancel.bookingOID = request.bookingOID;
                            _cancel.pnrNumber = response.Booking[0].recordLocator;
                            _cancel.useBookingOfficeID = true;
                            RetrieveAndCancel(_cancel);

                            oneBooking = flightBookingServices.GetFlightBooking(new Guid(response.Booking[0].bookingKeyReference));
                            oneBooking.StatusBooking = 3;
                            flightBookingServices.SaveFlightBooking(oneBooking);
                        }
                        response.Booking = null;
                        return response;
                    }

                    bool isNotConfirmStatus = false;
                    if (airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail != null)
                    {
                        if (airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation != null)
                        {
                            string status = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.ActionDetails.StatusCode;
                            _airSellEntities = new AirSellEntities();
                            _airSellEntities.tripType = (iR == 0 ? " DEPART" : " RETURN");
                            _airSellEntities.seq = 1;
                            _airSellEntities.boardPoint = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.BoardPointDetails.TrueLocationId;
                            _airSellEntities.offpoint = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.OffpointDetails.TrueLocationId;
                            _airSellEntities.airlineCode = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.CompanyDetails.MarketingCompany;
                            _airSellEntities.flightNumber = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.FlightIdentification.FlightNumber;
                            _airSellEntities.status = status;
                            airSellEntities.Add(_airSellEntities);

                            if (status != "HK" && status != "OK")
                            {
                                isNotConfirmStatus = true;
                            }
                        }
                        else
                        {
                            int iSeq = 0;
                            foreach (var itin in airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.SegmentInformationElementArray)
                            {
                                iSeq++;
                                string status = itin.ActionDetails.StatusCode;
                                _airSellEntities = new AirSellEntities();
                                _airSellEntities.tripType = (iR == 0 ? " DEPART" : " RETURN");
                                _airSellEntities.seq = iSeq;
                                _airSellEntities.boardPoint = itin.FlightDetails.BoardPointDetails.TrueLocationId;
                                _airSellEntities.offpoint = itin.FlightDetails.OffpointDetails.TrueLocationId;
                                _airSellEntities.airlineCode = itin.FlightDetails.CompanyDetails.MarketingCompany;
                                _airSellEntities.flightNumber = itin.FlightDetails.FlightIdentification.FlightNumber;
                                _airSellEntities.status = status;
                                airSellEntities.Add(_airSellEntities);
                                if (status != "HK" && status != "OK")
                                {
                                    isNotConfirmStatus = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        int iRoute = 0;
                        int iSeq = 0;
                        foreach (var itineraryDetail in airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.ItineraryDetailElementArray)
                        {
                            iSeq = 0;
                            iRoute++;

                            if (itineraryDetail.SegmentInformation.PurpleSegmentInformation != null)
                            {
                                iSeq++;
                                string status = itineraryDetail.SegmentInformation.PurpleSegmentInformation.ActionDetails.StatusCode;
                                _airSellEntities = new AirSellEntities();
                                _airSellEntities.tripType = iRoute == 1 ? "DEPART" : "RETURN";
                                _airSellEntities.seq = iSeq;
                                _airSellEntities.boardPoint = itineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.BoardPointDetails.TrueLocationId;
                                _airSellEntities.offpoint = itineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.OffpointDetails.TrueLocationId;
                                _airSellEntities.airlineCode = itineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.CompanyDetails.MarketingCompany;
                                _airSellEntities.flightNumber = itineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.FlightIdentification.FlightNumber;
                                _airSellEntities.status = status;
                                airSellEntities.Add(_airSellEntities);
                                if (status != "HK" && status != "OK")
                                {
                                    isNotConfirmStatus = true;
                                }
                            }
                            else
                            {

                                foreach (var itin in itineraryDetail.SegmentInformation.SegmentInformationElementArray)
                                {
                                    iSeq++;
                                    string status = itin.ActionDetails.StatusCode;
                                    _airSellEntities = new AirSellEntities();
                                    _airSellEntities.tripType = iRoute == 1 ? "DEPART" : "RETURN";
                                    _airSellEntities.seq = iSeq;
                                    _airSellEntities.boardPoint = itin.FlightDetails.BoardPointDetails.TrueLocationId;
                                    _airSellEntities.offpoint = itin.FlightDetails.OffpointDetails.TrueLocationId;
                                    _airSellEntities.airlineCode = itin.FlightDetails.CompanyDetails.MarketingCompany;
                                    _airSellEntities.flightNumber = itin.FlightDetails.FlightIdentification.FlightNumber;
                                    _airSellEntities.status = status;
                                    airSellEntities.Add(_airSellEntities);

                                    if (status != "HK" && status != "OK")
                                    {
                                        isNotConfirmStatus = true;
                                    }
                                }
                            }
                        }
                    }

                    if (isNotConfirmStatus)
                    {
                        response.isError = true;
                        response.errorCode = "4008";
                        response.errorMessage = "FLIGHTS NOT CONFIRM";
                        response.airSellEntities = airSellEntities;
                        if (response.Booking != null && response.Booking.Count > 0 && response.Booking[0] != null && response.Booking[0].recordLocator.Length > 0)
                        {
                            Entities.PNRCancel.Request _cancel = new Entities.PNRCancel.Request();
                            _cancel.bookingOID = request.bookingOID;
                            _cancel.pnrNumber = response.Booking[0].recordLocator;
                            _cancel.useBookingOfficeID = true;
                            RetrieveAndCancel(_cancel);

                            oneBooking = flightBookingServices.GetFlightBooking(new Guid(response.Booking[0].bookingKeyReference));
                            oneBooking.StatusBooking = 3;
                            flightBookingServices.SaveFlightBooking(oneBooking);
                        }
                        response.Booking = null;
                        return response;
                    }


                    //PNR
                    Entities.PNR.Request pnrReuest = new Entities.PNR.Request();
                    pnrReuest.session = new Entities.PNR.Session();
                    pnrReuest.session.isStateFull = true;
                    pnrReuest.session.InSeries = true;
                    pnrReuest.session.End = false;
                    pnrReuest.session.SessionId = airSellResponse.AirSellFromRecommendationReply.Session.SessionId;
                    pnrReuest.session.SequenceNumber = (int.Parse(airSellResponse.AirSellFromRecommendationReply.Session.SequenceNumber) + 1).ToString();
                    pnrReuest.session.SecurityToken = airSellResponse.AirSellFromRecommendationReply.Session.SecurityToken;
                    pnrReuest.bookingOID = request.bookingOID;//officeID

                    pnrReuest.fop = "CA";
                    pnrReuest.remarkType = "RM";
                    pnrReuest.remarks = new List<string>();
                    //pnrReuest.remarks.Add("Gogojii WEB BOOKING");
                    if (request.remarks != null && request.remarks.Count > 0)
                    {
                        foreach (string rm in request.remarks)
                        {
                            pnrReuest.remarks.Add(rm);
                        }
                    }
                    //pnrReuest.remarks.Add("ADTSALE : " + (request.adtFare.net - request.adtFare.tax - request.adtFare.qtax).ToString());//move qtax to fare
                    pnrReuest.remarks.Add("ADTSALE : " + (request.adtFare.net - request.adtFare.tax).ToString("N2"));
                    //pnrReuest.remarks.Add("ADTNET : " + request.adtFare.lessFare);//move qtax to fare
                    pnrReuest.remarks.Add("ADTNET : " + (request.adtFare.lessFare).ToString("N2"));
                    //pnrReuest.remarks.Add("ADTTAX : " + (request.adtFare.tax + request.adtFare.qtax).ToString());//move qtax to fare
                    pnrReuest.remarks.Add("ADTTAX : " + (request.adtFare.tax).ToString("N2"));
                    if (request.noOfChildren > 0)
                    {
                        pnrReuest.remarks.Add("CHDSALE : " + (request.chdFare.net - request.chdFare.tax).ToString("N2"));
                        pnrReuest.remarks.Add("CHDNET : " + (request.chdFare.lessFare).ToString("N2"));
                        pnrReuest.remarks.Add("CHDTAX : " + (request.chdFare.tax).ToString("N2"));
                    }
                    else
                    {
                        pnrReuest.remarks.Add("CHDSALE : 0.00");
                        pnrReuest.remarks.Add("CHDNET :  0.00");
                        pnrReuest.remarks.Add("CHDTAX : 0.00");
                    }
                    if (request.noOfInfants > 0)
                    {
                        pnrReuest.remarks.Add("INFSALE : " + (request.infFare.net - request.infFare.tax).ToString("N2"));
                        pnrReuest.remarks.Add("INFNET : " + (request.infFare.lessFare).ToString("N2"));
                        pnrReuest.remarks.Add("INFTAX : " + (request.infFare.tax).ToString("N2"));
                    }
                    else
                    {
                        pnrReuest.remarks.Add("INFSALE : 0.00");
                        pnrReuest.remarks.Add("INFNET :  0.00");
                        pnrReuest.remarks.Add("INFTAX : 0.00");
                    }
                    bool bHaveRP = false;
                    bool bHaveRU = false;
                    if (iR == 0)
                    {
                        string depFB = "";
                        foreach (var depF in request.depFlight)
                        {
                            depFB += (depFB == "" ? "" : "/") + depF.fareBasis;

                            if (depF.fareType == "RP")
                            {
                                bHaveRP = true;
                            }
                            else
                            {
                                bHaveRU = true;
                            }
                        }
                        pnrReuest.remarks.Add("DEPARTFAREBASIS : " + depFB);
                    }
                    else
                    {
                        if (request.retFlight != null && request.retFlight.Count > 0)
                        {
                            string retFB = "";
                            foreach (var retF in request.retFlight)
                            {
                                retFB += (retFB == "" ? "" : "/") + retF.fareBasis;

                                if (retF.fareType == "RP")
                                {
                                    bHaveRP = true;
                                }
                                else
                                {
                                    bHaveRU = true;
                                }
                            }
                            pnrReuest.remarks.Add("DEPARTFAREBASIS : " + retFB);
                        }
                    }
                    pnrReuest.remarks.Add("Pricing With FXP" + (bHaveRU ? (bHaveRP ? "/R,UP" : "/R,U") : ""));
                    pnrReuest.remarks.Add("TransactionID : " + response.RobinhoodID);
                    if (request.bookingURN != null && request.bookingURN.Length > 0)
                    {
                        pnrReuest.remarks.Add("BookingURN : " + request.bookingURN);
                    }
                    if (request.promotionGroupCode != null && request.promotionGroupCode.Count > 0)
                    {
                        if (iR == 0)
                        {
                            pnrReuest.remarks.Add("Promotion Group Code : " + request.promotionGroupCode[0]);
                        }
                        else
                        {

                            pnrReuest.remarks.Add("Promotion Group Code : " + (request.promotionGroupCode.Count > 0 ? request.promotionGroupCode[0] : request.promotionGroupCode[1]));
                        }
                    }
                    dataConfig = siteConfig.GetByKey("TicketIndicator");
                    pnrReuest.ticketIndicator = dataConfig.ConfigValue;//ConfigurationManager.AppSettings["TicketIndicator"]; //"TL";

                    dataConfig = siteConfig.GetByKey("TicketTimeLimitHour");
                    pnrReuest.ticketTimeLimitDate = DateTime.Now.AddMinutes(int.Parse(dataConfig.ConfigValue));
                    pnrReuest.passengerList = new List<Entities.PNR.PassengerList>();
                    List<Entities.RobinhoodPax.SpecialRequest> specialRequests = null;
                    Entities.RobinhoodPax.SpecialRequest SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                    Entities.RobinhoodPax.FrequentFlyList frequentFly = new Entities.RobinhoodPax.FrequentFlyList();
                    string gender = "";
                    for (int i = 0; i < request.adtPaxs.Count; i++)
                    {
                        Entities.PNR.PassengerList pax = new Entities.PNR.PassengerList();
                        pax.paxNo = i + 1;
                        pax.infantFlag = request.infPaxs == null ? false : (request.infPaxs.Count >= (i + 1));
                        pax.haveDOB = false;
                        pax.DOB = new DateTime();
                        pax.age = 25;
                        pax.withADT = 0;
                        pax.ptNo = "";
                        pax.titleName = request.adtPaxs[i].title.Trim();
                        pax.firstName = request.adtPaxs[i].firstname.Trim();
                        pax.lastName = request.adtPaxs[i].lastname.Trim();
                        pax.middleName = request.adtPaxs[i].middlename.Trim();
                        pax.paxType = "ADT";
                        pax.paxID = (i + 1).ToString();
                        pax.withInsurance = false;
                        if (request.adtPaxs[i].passportNumber != null && request.adtPaxs[i].passportNumber.Length > 0)
                        {
                            gender = (pax.titleName.ToUpper() == "MR" || pax.titleName.ToUpper() == "PHRA") ? "M" : "F";
                            pax.passportNumber = String.Format("P-{6}-{0}-{7}-{1}-{2}-{3}-{4}-{5}", request.adtPaxs[i].passportNumber, request.adtPaxs[i].birthday.ToString("ddMMMyy"), gender, request.adtPaxs[i].passportExpiryDate.ToString("ddMMMyy"), request.adtPaxs[i].lastname, request.adtPaxs[i].firstname, request.adtPaxs[i].passportIssuingCountry, request.adtPaxs[i].passportNationality);
                        }
                        if (request.adtPaxs[i].frequentFlyList != null && request.adtPaxs[i].frequentFlyList.Count > 0)
                        {
                            pax.frequentFlyList = new List<Entities.RobinhoodPax.FrequentFlyList>();
                            if (iR == 0)
                            {
                                frequentFly = new Entities.RobinhoodPax.FrequentFlyList();
                                frequentFly.Airline = request.adtPaxs[i].frequentFlyList[iR].Airline;
                                frequentFly.Number = request.adtPaxs[i].frequentFlyList[iR].Airline + request.adtPaxs[i].frequentFlyList[iR].Number;
                                pax.frequentFlyList.Add(frequentFly);
                            }
                            else
                            {
                                if (request.adtPaxs[i].frequentFlyList.Count > 1)
                                {
                                    frequentFly = new Entities.RobinhoodPax.FrequentFlyList();
                                    frequentFly.Airline = request.adtPaxs[i].frequentFlyList[iR].Airline;
                                    frequentFly.Number = request.adtPaxs[i].frequentFlyList[iR].Airline + request.adtPaxs[i].frequentFlyList[iR].Number;
                                    pax.frequentFlyList.Add(frequentFly);
                                }
                                else
                                {
                                    frequentFly = new Entities.RobinhoodPax.FrequentFlyList();
                                    frequentFly.Airline = request.adtPaxs[i].frequentFlyList[0].Airline;
                                    frequentFly.Number = request.adtPaxs[i].frequentFlyList[0].Airline + request.adtPaxs[i].frequentFlyList[0].Number;
                                    pax.frequentFlyList.Add(frequentFly);
                                }
                            }
                        }
                        else
                        {
                            pax.frequentFlyerAirline = request.adtPaxs[i].frequencyFlyerAirline != null ? request.adtPaxs[i].frequencyFlyerAirline : null;
                            pax.frequentFlyerNumber = request.adtPaxs[i].frequencyFlyerNumber != null ? request.adtPaxs[i].frequencyFlyerNumber : null;
                        }
                        if ((request.adtPaxs[i].seatsRequest != null && request.adtPaxs[i].seatsRequest.Count > 0)
                            || (request.adtPaxs[i].seatRequest != null && request.adtPaxs[i].seatRequest.Length > 0)
                            || (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0))
                        {
                            specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();
                            if (iR == 0)
                            {
                                for (int d = 0; d < request.depFlight.Count; d++)
                                {
                                    SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                    SpecialRequest.flightST = d + 1;
                                    if (request.adtPaxs[i].seatsRequest != null && request.adtPaxs[i].seatsRequest.Count > 0)
                                    {
                                        SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                        SpecialRequest.seat.seatType = request.adtPaxs[i].seatsRequest[d].seatType;
                                        SpecialRequest.seat.seatRefNo = request.adtPaxs[i].seatsRequest[d].seatRefNo;
                                    }
                                    else
                                    {
                                        if (request.adtPaxs[i].seatRequest != null && request.adtPaxs[i].seatRequest.Length > 0)
                                        {
                                            SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                            SpecialRequest.seat.seatType = request.adtPaxs[i].seatRequest == "W" ? "NSSW" : "NSSA";
                                            SpecialRequest.seat.seatRefNo = "";
                                        }
                                    }
                                    if (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0)
                                    {
                                        SpecialRequest.mealCode = request.adtPaxs[i].mealRequest;
                                    }
                                    else
                                    {
                                        SpecialRequest.mealCode = "NONE";
                                    }
                                    specialRequests.Add(SpecialRequest);
                                }
                                pax.depSpecialRequest = specialRequests;
                            }
                            else
                            {

                                if (request.retFlight != null && request.retFlight.Count > 0)
                                {
                                    specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();

                                    for (int d = request.depFlight.Count; d < request.retFlight.Count + request.depFlight.Count; d++)
                                    {
                                        SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                        SpecialRequest.flightST = d + 1;
                                        if (request.adtPaxs[i].seatsRequest != null && request.adtPaxs[i].seatsRequest.Count > 0)
                                        {
                                            SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                            SpecialRequest.seat.seatType = request.adtPaxs[i].seatsRequest[d].seatType;
                                            SpecialRequest.seat.seatRefNo = request.adtPaxs[i].seatsRequest[d].seatRefNo;
                                        }
                                        else
                                        {
                                            if (request.adtPaxs[i].seatRequest != null && request.adtPaxs[i].seatRequest.Length > 0)
                                            {
                                                SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                                SpecialRequest.seat.seatType = request.adtPaxs[i].seatRequest == "W" ? "NSSW" : "NSSA";
                                                SpecialRequest.seat.seatRefNo = "";
                                            }
                                        }
                                        if (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0)
                                        {
                                            SpecialRequest.mealCode = request.adtPaxs[i].mealRequest;
                                        }
                                        else
                                        {
                                            SpecialRequest.mealCode = "NONE";
                                        }
                                        specialRequests.Add(SpecialRequest);
                                    }
                                    //pax.retSpecialRequest = specialRequests;
                                    pax.depSpecialRequest = specialRequests;
                                }
                            }
                        }

                        if (request.infPaxs != null && request.infPaxs.Count >= (i + 1))
                        {
                            pax.infantPassenger = new Entities.PNR.InfantPassenger();
                            pax.infantPassenger.paxNo = (request.adtPaxs == null ? 0 : request.adtPaxs.Count) + (request.chdPaxs == null ? 0 : request.chdPaxs.Count) + i + 1;
                            pax.infantPassenger.infantFlag = false;
                            pax.infantPassenger.haveDOB = true;
                            pax.infantPassenger.DOB = request.infPaxs[i].birthday;
                            pax.infantPassenger.age = 1;
                            pax.infantPassenger.withADT = 0;
                            pax.infantPassenger.titleName = "";
                            pax.infantPassenger.firstName = request.infPaxs[i].firstname.Trim();
                            pax.infantPassenger.lastName = request.infPaxs[i].lastname.Trim();
                            pax.infantPassenger.paxType = "INF";
                            pax.infantPassenger.paxID = ((request.adtPaxs == null ? 0 : request.adtPaxs.Count) + (request.chdPaxs == null ? 0 : request.chdPaxs.Count) + i + 1).ToString();
                            if (request.infPaxs[i].passportNumber != null && request.infPaxs[i].passportNumber.Length > 0)
                            {
                                gender = pax.titleName.ToUpper() == "MSTR" ? "MI" : "FI";
                                pax.infantPassenger.passportNumber = String.Format("P-{6}-{0}-{7}-{1}-{2}-{3}-{4}-{5}", request.infPaxs[i].passportNumber, request.infPaxs[i].birthday.ToString("ddMMMyy"), gender, request.infPaxs[i].passportExpiryDate.ToString("ddMMMyy"), request.infPaxs[i].lastname, request.infPaxs[i].firstname, request.infPaxs[i].passportIssuingCountry, request.infPaxs[i].passportNationality);
                            }
                            if (request.infPaxs[i].mealRequest != null && request.infPaxs[i].mealRequest.Length > 0)
                            {
                                specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();
                                if (iR == 0)
                                {
                                    for (int d = 0; d < request.depFlight.Count; d++)
                                    {
                                        SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                        SpecialRequest.flightST = d + 1;
                                        if (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0)
                                        {
                                            SpecialRequest.mealCode = request.adtPaxs[i].mealRequest;
                                        }
                                        else
                                        {
                                            SpecialRequest.mealCode = "NONE";
                                        }
                                        specialRequests.Add(SpecialRequest);
                                    }
                                    pax.depSpecialRequest = specialRequests;
                                }
                                else
                                {
                                    if (request.retFlight != null && request.retFlight.Count > 0)
                                    {
                                        specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();

                                        for (int d = request.depFlight.Count; d < request.retFlight.Count + request.depFlight.Count; d++)
                                        {
                                            SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                            SpecialRequest.flightST = d + 1;
                                            if (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0)
                                            {
                                                SpecialRequest.mealCode = request.adtPaxs[i].mealRequest;
                                            }
                                            else
                                            {
                                                SpecialRequest.mealCode = "NONE";
                                            }
                                            specialRequests.Add(SpecialRequest);
                                        }
                                        //pax.retSpecialRequest = specialRequests;
                                        pax.depSpecialRequest = specialRequests;
                                    }
                                }
                            }
                        }

                        pnrReuest.passengerList.Add(pax);
                    }

                    if (request.chdPaxs != null)
                    {
                        for (int i = 0; i < request.chdPaxs.Count; i++)
                        {
                            Entities.PNR.PassengerList pax = new Entities.PNR.PassengerList();
                            pax.paxNo = i + request.adtPaxs.Count;
                            pax.infantFlag = false;
                            pax.haveDOB = true;
                            pax.DOB = request.chdPaxs[i].birthday;
                            pax.age = 8;
                            pax.withADT = 0;
                            pax.ptNo = "";
                            pax.titleName = request.chdPaxs[i].title.Trim();
                            pax.firstName = request.chdPaxs[i].firstname.Trim();
                            pax.lastName = request.chdPaxs[i].lastname.Trim();
                            pax.middleName = request.chdPaxs[i].middlename.Trim();
                            pax.paxType = "CHD";
                            pax.withInsurance = false;
                            pax.paxID = (i + request.adtPaxs.Count).ToString();
                            if (request.chdPaxs[i].passportNumber != null && request.chdPaxs[i].passportNumber.Length > 0)
                            {
                                gender = pax.titleName.ToUpper() == "MSTR" ? "M" : "F";
                                pax.passportNumber = String.Format("P-{6}-{0}-{7}-{1}-{2}-{3}-{4}-{5}", request.chdPaxs[i].passportNumber, request.chdPaxs[i].birthday.ToString("ddMMMyy"), gender, request.chdPaxs[i].passportExpiryDate.ToString("ddMMMyy"), request.chdPaxs[i].lastname, request.chdPaxs[i].firstname, request.chdPaxs[i].passportIssuingCountry, request.chdPaxs[i].passportNationality);
                            }
                            if (request.chdPaxs[i].frequentFlyList != null && request.chdPaxs[i].frequentFlyList.Count > 0)
                            {
                                pax.frequentFlyList = new List<Entities.RobinhoodPax.FrequentFlyList>();
                                if (iR == 0)
                                {
                                    frequentFly = new Entities.RobinhoodPax.FrequentFlyList();
                                    frequentFly.Airline = request.chdPaxs[i].frequentFlyList[iR].Airline;
                                    frequentFly.Number = request.chdPaxs[i].frequentFlyList[iR].Airline + request.chdPaxs[i].frequentFlyList[iR].Number;
                                    pax.frequentFlyList.Add(frequentFly);
                                }
                                else
                                {
                                    if (request.chdPaxs[i].frequentFlyList.Count > 1)
                                    {
                                        frequentFly = new Entities.RobinhoodPax.FrequentFlyList();
                                        frequentFly.Airline = request.chdPaxs[i].frequentFlyList[iR].Airline;
                                        frequentFly.Number = request.chdPaxs[i].frequentFlyList[iR].Airline + request.chdPaxs[i].frequentFlyList[iR].Number;
                                        pax.frequentFlyList.Add(frequentFly);
                                    }
                                    else
                                    {
                                        frequentFly = new Entities.RobinhoodPax.FrequentFlyList();
                                        frequentFly.Airline = request.chdPaxs[i].frequentFlyList[0].Airline;
                                        frequentFly.Number = request.chdPaxs[i].frequentFlyList[0].Airline + request.chdPaxs[i].frequentFlyList[0].Number;
                                        pax.frequentFlyList.Add(frequentFly);
                                    }
                                }
                            }
                            else
                            {
                                pax.frequentFlyerAirline = request.chdPaxs[i].frequencyFlyerAirline != null ? request.chdPaxs[i].frequencyFlyerAirline : null;
                                pax.frequentFlyerNumber = request.chdPaxs[i].frequencyFlyerNumber != null ? request.chdPaxs[i].frequencyFlyerNumber : null;
                            }

                            if ((request.chdPaxs[i].seatsRequest != null && request.chdPaxs[i].seatsRequest.Count > 0)
                                || (request.chdPaxs[i].seatRequest != null && request.chdPaxs[i].seatRequest.Length > 0)
                                || (request.chdPaxs[i].mealRequest != null && request.chdPaxs[i].mealRequest.Length > 0))
                            {
                                specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();
                                if (iR == 0)
                                {
                                    for (int d = 0; d < request.depFlight.Count; d++)
                                    {
                                        SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                        SpecialRequest.flightST = d + 1;
                                        if (request.chdPaxs[i].seatsRequest != null && request.chdPaxs[i].seatsRequest.Count > 0)
                                        {
                                            SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                            SpecialRequest.seat.seatType = request.chdPaxs[i].seatsRequest[d].seatType;
                                            SpecialRequest.seat.seatRefNo = request.chdPaxs[i].seatsRequest[d].seatRefNo;
                                        }
                                        else
                                        {
                                            if (request.chdPaxs[i].seatRequest != null && request.chdPaxs[i].seatRequest.Length > 0)
                                            {
                                                SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                                SpecialRequest.seat.seatType = request.chdPaxs[i].seatRequest == "W" ? "NSSW" : "NSSA";
                                                SpecialRequest.seat.seatRefNo = "";
                                            }
                                        }
                                        if (request.chdPaxs[i].mealRequest != null && request.chdPaxs[i].mealRequest.Length > 0)
                                        {
                                            SpecialRequest.mealCode = request.chdPaxs[i].mealRequest;
                                        }
                                        specialRequests.Add(SpecialRequest);
                                    }
                                    pax.depSpecialRequest = specialRequests;
                                }
                                else
                                {
                                    if (request.retFlight != null && request.retFlight.Count > 0)
                                    {
                                        specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();

                                        for (int d = request.depFlight.Count; d < request.retFlight.Count + request.depFlight.Count; d++)
                                        {
                                            SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                            SpecialRequest.flightST = d + 1;
                                            if (request.chdPaxs[i].seatsRequest != null && request.chdPaxs[i].seatsRequest.Count > 0)
                                            {
                                                SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                                SpecialRequest.seat.seatType = request.chdPaxs[i].seatsRequest[d].seatType;
                                                SpecialRequest.seat.seatRefNo = request.chdPaxs[i].seatsRequest[d].seatRefNo;
                                            }
                                            else
                                            {
                                                if (request.chdPaxs[i].seatRequest != null && request.chdPaxs[i].seatRequest.Length > 0)
                                                {
                                                    SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                                    SpecialRequest.seat.seatType = request.chdPaxs[i].seatRequest == "W" ? "NSSW" : "NSSA";
                                                    SpecialRequest.seat.seatRefNo = "";
                                                }
                                            }
                                            if (request.chdPaxs[i].mealRequest != null && request.chdPaxs[i].mealRequest.Length > 0)
                                            {
                                                SpecialRequest.mealCode = request.chdPaxs[i].mealRequest;
                                            }
                                            else
                                            {
                                                SpecialRequest.mealCode = "NONE";
                                            }
                                            specialRequests.Add(SpecialRequest);
                                        }
                                        //pax.retSpecialRequest = specialRequests;
                                        pax.depSpecialRequest = specialRequests;
                                    }
                                }
                            }

                            pnrReuest.passengerList.Add(pax);
                        }
                    }
                    pnrReuest.refEmail = String.IsNullOrEmpty(request.contactInfo.email) ? request.adtPaxs[0].email : request.contactInfo.email;
                    pnrReuest.refMobileNumber = String.IsNullOrEmpty(request.contactInfo.telNo) ? request.adtPaxs[0].telNo : request.contactInfo.telNo;

                    url = ConfigurationManager.AppSettings["PNR.URL"];
                    requestJson = JsonConvert.SerializeObject(pnrReuest);

                    json = HttpUtility.postJSON(url, requestJson);

                    Log.Info("CREATE_PNR");
                    Log.Debug(requestJson);
                    Log.Debug(json);
                    BL.Entities.PNR.Response pnrResponse = new Entities.PNR.Response();
                    if (String.IsNullOrEmpty(json) == false)
                    {
                        json = json.Replace("@", "");

                        pnrResponse = BL.Entities.PNR.Response.FromJson(json);
                    }

                    if (pnrResponse.Error != null)
                    {
                        response.isError = true;
                        response.errorCode = "4009";
                        response.errorMessage = pnrResponse.Error.ErrorMessage + "(" + pnrResponse.Error.ErrorCode + ")";
                        if (response.Booking != null && response.Booking.Count > 0 && response.Booking[0] != null && response.Booking[0].recordLocator.Length > 0)
                        {
                            Entities.PNRCancel.Request _cancel = new Entities.PNRCancel.Request();
                            _cancel.bookingOID = request.bookingOID;
                            _cancel.pnrNumber = response.Booking[0].recordLocator;
                            _cancel.useBookingOfficeID = true;
                            RetrieveAndCancel(_cancel);
                        }
                        response.Booking = null;
                        return response;
                    }

                    ElementManagement elePax = null;
                    Entities.PNR.PassengerList one_pax = null;
                    TravellerInfo traveller = null;
                    if (pnrResponse.PnrReply.TravellerInfo.TravellerInfoElementArray != null)
                    {
                        string paxType = "";
                        string pax_FirstName = "";
                        string pax_LastName = "";
                        for (int i = 0; i < pnrResponse.PnrReply.TravellerInfo.TravellerInfoElementArray.Count; i++)
                        {
                            traveller = pnrResponse.PnrReply.TravellerInfo.TravellerInfoElementArray[i];
                            if (traveller != null)
                            {
                                elePax = traveller.ElementManagementPassenger;
                                pax_LastName = traveller.PassengerData.PurplePassengerData != null ? traveller.PassengerData.PurplePassengerData.TravellerInformation.Traveller.Surname : "";
                                if (traveller.PassengerData.PassengerDataElementArray != null)
                                {
                                    if (traveller.PassengerData.PassengerDataElementArray.Find(x => x.TravellerInformation.Passenger.PassengerElementArray != null) != null)
                                    {

                                        var allPassengerElementArray = traveller.PassengerData.PassengerDataElementArray;
                                        foreach (var PassengerElementArray in allPassengerElementArray)
                                        {
                                            if (pax_LastName.Length == 0)
                                            {
                                                pax_LastName = PassengerElementArray.TravellerInformation.Traveller.Surname;
                                            }
                                            foreach (var allTravellerInformation in PassengerElementArray.TravellerInformation.Passenger.PassengerElementArray)
                                            {

                                                paxType = allTravellerInformation.Type;
                                                pax_FirstName = allTravellerInformation.FirstName;//"firstName": "NING TEST MR"

                                                one_pax = pnrReuest.passengerList.Find(x => x.paxType.ToUpper() == paxType && x.lastName.ToUpper() == pax_LastName && (String.Format("{0} {1}{2}", x.firstName, (x.middleName != null && x.middleName.Length > 0 ? (x.middleName + " ") : ""), x.titleName).ToUpper() == pax_FirstName));
                                                if (one_pax != null)
                                                {
                                                    one_pax.ptNo = elePax.Reference.Number;
                                                }
                                            }

                                        }

                                    }
                                    else//chd
                                    {
                                        foreach (var allPaxDataArray in traveller.PassengerData.PassengerDataElementArray)
                                        {
                                            pax_LastName = allPaxDataArray.TravellerInformation.Traveller.Surname;
                                            paxType = allPaxDataArray.TravellerInformation.Passenger.PurplePassenger.Type;
                                            pax_FirstName = allPaxDataArray.TravellerInformation.Passenger.PurplePassenger.FirstName;//"firstName": "NING TEST MR"
                                            one_pax = pnrReuest.passengerList.Find(x => x.paxType.ToUpper() == paxType && x.lastName.ToUpper() == pax_LastName && (String.Format("{0} {1}{2}", x.firstName, (x.middleName != null && x.middleName.Length > 0 ? (x.middleName + " ") : ""), x.titleName).ToUpper() == pax_FirstName));
                                            if (one_pax != null)
                                            {
                                                one_pax.ptNo = elePax.Reference.Number;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PassengerElementArray != null)
                                    {
                                        pax_LastName = traveller.PassengerData.PurplePassengerData.TravellerInformation.Traveller.Surname;
                                        paxType = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PassengerElementArray[0].Type;
                                        pax_FirstName = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PassengerElementArray[0].FirstName;
                                        one_pax = pnrReuest.passengerList.Find(x => x.paxType.ToUpper() == paxType && x.lastName.ToUpper() == pax_LastName && (String.Format("{0} {1}{2}", x.firstName, (x.middleName != null && x.middleName.Length > 0 ? (x.middleName + " ") : ""), x.titleName).ToUpper() == pax_FirstName));
                                        if (one_pax != null)
                                        {
                                            one_pax.ptNo = elePax.Reference.Number;
                                        }
                                        //inf
                                        paxType = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PassengerElementArray[1].Type;
                                        pax_FirstName = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PassengerElementArray[1].FirstName;
                                        one_pax = pnrReuest.passengerList.Find(x => x.paxType.ToUpper() == paxType && x.lastName.ToUpper() == pax_LastName && (String.Format("{0} {1}{2}", x.firstName, (x.middleName != null && x.middleName.Length > 0 ? (x.middleName + " ") : ""), x.titleName).ToUpper() == pax_FirstName));
                                        if (one_pax != null)
                                        {
                                            one_pax.ptNo = elePax.Reference.Number;
                                        }
                                    }
                                    else
                                    {
                                        paxType = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PurplePassenger.Type;
                                        pax_FirstName = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PurplePassenger.FirstName;
                                        one_pax = pnrReuest.passengerList.Find(x => x.paxType.ToUpper() == paxType && x.lastName.ToUpper() == pax_LastName && (String.Format("{0} {1}{2}", x.firstName, (x.middleName != null && x.middleName.Length > 0 ? (x.middleName + " ") : ""), x.titleName).ToUpper() == pax_FirstName));
                                        if (one_pax != null)
                                        {
                                            one_pax.ptNo = elePax.Reference.Number;
                                        }
                                    }

                                }
                            }

                        }

                    }
                    else
                    {
                        elePax = pnrResponse.PnrReply.TravellerInfo.PurpleTravellerInfo.ElementManagementPassenger;
                        if (elePax != null)
                        {
                            one_pax = pnrReuest.passengerList[0];
                            one_pax.ptNo = elePax.Reference.Number;
                        }
                    }

                    //Pricing
                    Entities.Pricing.Request pricingReuest = new Entities.Pricing.Request();
                    BL.Entities.TST.Response tstResponse = new Entities.TST.Response();
                    #region Pricing and TST
                    pricingReuest = new Entities.Pricing.Request();
                    pricingReuest.session = new Entities.Pricing.Session();
                    pricingReuest.session.isStateFull = true;
                    pricingReuest.session.InSeries = true;
                    pricingReuest.session.End = false;
                    pricingReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
                    pricingReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
                    pricingReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
                    pricingReuest.bookingOID = request.bookingOID;

                    pricingReuest.pricingTicketingIndicators = new List<string>();
                    //pricingReuest.pricingTicketingIndicators.Add("RU");
                    //pricingReuest.pricingTicketingIndicators.Add("RP");
                    //pricingReuest.pricingTicketingIndicators.Add("RLO");

                    bHaveRP = false;
                    bHaveRU = false;
                    if (iR == 0)
                    {
                        foreach (var depF in request.depFlight)
                        {
                            if (depF.fareType == "RP")
                            {
                                bHaveRP = true;
                            }
                            else
                            {
                                bHaveRU = true;
                            }
                        }

                    }
                    else
                    {
                        if (request.retFlight != null && request.retFlight.Count > 0)
                        {
                            foreach (var depF in request.retFlight)
                            {
                                if (depF.fareType == "RP")
                                {
                                    bHaveRP = true;
                                }
                                else
                                {
                                    bHaveRU = true;
                                }
                            }
                        }
                    }
                    if (bHaveRU)
                    {
                        pricingReuest.pricingTicketingIndicators.Add("RU");
                    }
                    if (bHaveRP)
                    {
                        pricingReuest.pricingTicketingIndicators.Add("RP");
                    }

                    pricingReuest.useCityOverride = false;
                    pricingReuest.useCurrencyOverride = true;
                    pricingReuest.currencyCode = "THB";

                    url = ConfigurationManager.AppSettings["PRICING.URL"];
                    requestJson = JsonConvert.SerializeObject(pricingReuest);

                    json = HttpUtility.postJSON(url, requestJson);

                    Log.Info("PRICING");
                    Log.Debug(requestJson);
                    Log.Debug(json);

                    BL.Entities.Pricing.Response pricingResponse = new Entities.Pricing.Response();
                    if (String.IsNullOrEmpty(json) == false)
                    {
                        json = json.Replace("@", "");

                        pricingResponse = BL.Entities.Pricing.Response.FromJson(json);
                    }

                    if (pricingResponse.Error != null)
                    {
                        response.isError = true;
                        response.errorCode = "4010";
                        response.errorMessage = pricingResponse.Error.ErrorMessage + "(" + pricingResponse.Error.ErrorCode + ")";
                        if (response.Booking != null && response.Booking.Count > 0 && response.Booking[0] != null && response.Booking[0].recordLocator.Length > 0)
                        {
                            Entities.PNRCancel.Request _cancel = new Entities.PNRCancel.Request();
                            _cancel.bookingOID = request.bookingOID;
                            _cancel.pnrNumber = response.Booking[0].recordLocator;
                            _cancel.useBookingOfficeID = true;
                            RetrieveAndCancel(_cancel);

                            oneBooking = flightBookingServices.GetFlightBooking(new Guid(response.Booking[0].bookingKeyReference));
                            oneBooking.StatusBooking = 3;
                            flightBookingServices.SaveFlightBooking(oneBooking);
                        }
                        response.Booking = null;
                        return response;
                    }

                    List<string> uniqueReferences = new List<string>();
                    if (pricingResponse.FarePricePnrWithBookingClassReply.FareList.PurpleFareList != null)
                    {
                        uniqueReferences.Add(pricingResponse.FarePricePnrWithBookingClassReply.FareList.PurpleFareList.FareReference.UniqueReference);
                    }
                    else
                    {
                        List<string> paxPTList = new List<string>();
                        bool bChkPT = false;
                        foreach (var f in pricingResponse.FarePricePnrWithBookingClassReply.FareList.FareListElementArray)
                        {
                            bChkPT = false;
                            if (f.PaxSegReference.RefDetails.RefDetailArray != null && f.PaxSegReference.RefDetails.RefDetailArray.Count > 0)
                            {
                                foreach (var pt in f.PaxSegReference.RefDetails.RefDetailArray)
                                {
                                    if (!paxPTList.Contains(pt.RefNumber))
                                    {
                                        paxPTList.Add(pt.RefNumber);
                                        bChkPT = true;
                                    }
                                }
                            }
                            else
                            {
                                if (!paxPTList.Contains(f.PaxSegReference.RefDetails.RefDetail.RefNumber))
                                {
                                    paxPTList.Add(f.PaxSegReference.RefDetails.RefDetail.RefNumber);
                                    bChkPT = true;
                                }

                            }
                            if (bChkPT)
                            {
                                uniqueReferences.Add(f.FareReference.UniqueReference);
                            }
                        }
                    }


                    Entities.TST.Request tstRequest = new Entities.TST.Request();
                    tstRequest.session = new Entities.TST.Session();
                    tstRequest.bookingOID = request.bookingOID;
                    tstRequest.session.isStateFull = true;
                    tstRequest.session.InSeries = true;
                    tstRequest.session.End = false;
                    tstRequest.session.SessionId = pricingResponse.FarePricePnrWithBookingClassReply.Session.SessionId;
                    tstRequest.session.SequenceNumber = (int.Parse(pricingResponse.FarePricePnrWithBookingClassReply.Session.SequenceNumber) + 1).ToString();
                    tstRequest.session.SecurityToken = pricingResponse.FarePricePnrWithBookingClassReply.Session.SecurityToken;
                    tstRequest.ClientIP = Utilities.HttpUtility.GetIPAddress();
                    tstRequest.uniqueReferences = uniqueReferences;
                    tstRequest.useBookingOfficeID = false;

                    url = ConfigurationManager.AppSettings["TST.URL"];
                    requestJson = JsonConvert.SerializeObject(tstRequest);

                    json = HttpUtility.postJSON(url, requestJson);

                    Log.Info("TST");
                    Log.Debug(requestJson);
                    Log.Debug(json);

                    tstResponse = new Entities.TST.Response();
                    if (String.IsNullOrEmpty(json) == false)
                    {
                        json = json.Replace("@", "");

                        tstResponse = BL.Entities.TST.Response.FromJson(json);
                    }

                    if (tstResponse.Error != null)
                    {
                        response.isError = true;
                        response.errorCode = "4011";
                        response.errorMessage = tstResponse.Error.ErrorMessage + "(" + tstResponse.Error.ErrorCode + ")";
                        if (response.Booking != null && response.Booking.Count > 0 && response.Booking[0] != null && response.Booking[0].recordLocator.Length > 0)
                        {
                            Entities.PNRCancel.Request _cancel = new Entities.PNRCancel.Request();
                            _cancel.bookingOID = request.bookingOID;
                            _cancel.pnrNumber = response.Booking[0].recordLocator;
                            _cancel.useBookingOfficeID = true;
                            RetrieveAndCancel(_cancel);

                            oneBooking = flightBookingServices.GetFlightBooking(new Guid(response.Booking[0].bookingKeyReference));
                            oneBooking.StatusBooking = 3;
                            flightBookingServices.SaveFlightBooking(oneBooking);
                        }
                        response.Booking = null;
                        return response;
                    }
                    #endregion Pricing and TST


                    //PNR
                    Entities.PNR.Request pnrSavedReuest = new Entities.PNR.Request();
                    pnrSavedReuest.session = new Entities.PNR.Session();
                    pnrSavedReuest.session.isStateFull = true;
                    pnrSavedReuest.session.InSeries = true;
                    pnrSavedReuest.session.End = true;
                    pnrSavedReuest.session.SessionId = tstResponse.TicketCreateTstFromPricingReply.Session.SessionId;
                    pnrSavedReuest.session.SequenceNumber = (int.Parse(tstResponse.TicketCreateTstFromPricingReply.Session.SequenceNumber) + 1).ToString();
                    pnrSavedReuest.session.SecurityToken = tstResponse.TicketCreateTstFromPricingReply.Session.SecurityToken;
                    //pnrSavedReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
                    //pnrSavedReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
                    //pnrSavedReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
                    pnrSavedReuest.bookingOID = request.bookingOID;//officeID
                    pnrSavedReuest.optionCode = "11";
                    pnrSavedReuest.rfLongFreeText = "ROBINHOOD API";

                    pnrSavedReuest.departFlightSegmentCount = request.depFlight == null ? 0 : request.depFlight.Count;
                    pnrSavedReuest.returnFlightSegmentCount = request.retFlight == null ? 0 : request.retFlight.Count;
                    pnrSavedReuest.passengerList = pnrReuest.passengerList;
                    pnrSavedReuest.accessOID = "";// "BKKIW38FO";

                    pnrSavedReuest.ClientIP = Utilities.HttpUtility.GetIPAddress();

                    pnrSavedReuest.fkOption = new FKOption();
                    pnrSavedReuest.fkOption.generalIndicator = "K";
                    pnrSavedReuest.fkOption.officeId = request.bookingOID;//officeID

                    url = ConfigurationManager.AppSettings["SAVEPNR.URL"];
                    requestJson = JsonConvert.SerializeObject(pnrSavedReuest);

                    json = HttpUtility.postJSON(url, requestJson);

                    Log.Info("SAVE_PNR");
                    Log.Debug(requestJson);
                    Log.Debug(json);
                    BL.Entities.PNR.Response pnrSavedResponse = new Entities.PNR.Response();
                    if (String.IsNullOrEmpty(json) == false)
                    {
                        json = json.Replace("@", "");

                        pnrSavedResponse = BL.Entities.PNR.Response.FromJson(json);
                    }

                    if (pnrSavedResponse.Error != null)
                    {
                        response.isError = true;
                        response.errorCode = "4012";
                        if (pnrSavedResponse.Error.ErrorCode == "313")
                        {
                            response.errorMessage = pnrSavedResponse.Error.ErrorMessage;
                        }
                        else
                        {
                            response.errorMessage = pnrSavedResponse.Error.ErrorMessage + "(" + pnrSavedResponse.Error.ErrorCode + ")";
                        }
                        if (response.Booking != null && response.Booking.Count > 0 && response.Booking[0] != null && response.Booking[0].recordLocator.Length > 0)
                        {
                            Entities.PNRCancel.Request _cancel = new Entities.PNRCancel.Request();
                            _cancel.bookingOID = request.bookingOID;
                            _cancel.pnrNumber = response.Booking[0].recordLocator;
                            _cancel.useBookingOfficeID = true;
                            RetrieveAndCancel(_cancel);

                            oneBooking = flightBookingServices.GetFlightBooking(new Guid(response.Booking[0].bookingKeyReference));
                            oneBooking.StatusBooking = 3;
                            flightBookingServices.SaveFlightBooking(oneBooking);
                        }
                        response.Booking = null;
                        return response;
                    }
                    if (pnrSavedResponse.PnrReply.PnrHeader.PurplePnrHeader != null)
                    {
                        _onebooking.recordLocator = pnrSavedResponse.PnrReply.PnrHeader.PurplePnrHeader.ReservationInfo.Reservation.ControlNumber;
                    }
                    else
                    {
                        _onebooking.recordLocator = pnrSavedResponse.PnrReply.PnrHeader.PnrHeaderElementArray[0].ReservationInfo.Reservation.ControlNumber;
                    }

                    //response.bookingKeyReference = Guid.NewGuid().ToString();

                    request.PNR = _onebooking.recordLocator;
                    request.RobinhoodID = response.RobinhoodID;
                    FlightBookingServices bookingSvc = new FlightBookingServices(_unitOfWork);
                    var bookingRef = bookingSvc.SaveBooking(request);
                    _onebooking.bookingKeyReference = bookingRef.ToString();

                    FlightReportServices reportSvc = new FlightReportServices(_unitOfWork);
                    var bookingTest = reportSvc.GetByID(bookingRef);
                    response.Booking.Add(_onebooking);
                }//for

            }//if (request.isPricingWithSegment)
            return response;
        }
        public Entities.RobinhoodPNR.PNR Booking(Entities.RobinhoodFare.AirFare request)
        {
            RunningNumberServices runningSvc = new RunningNumberServices(_unitOfWork);
            int rbhID = runningSvc.GetNumber("FBOOKING" + DateTime.Today.ToString("yyMMdd"));
            string robinhoodID = "F" + DateTime.Today.ToString("yyMMdd") + rbhID.ToString().PadLeft(4, '0');
            SiteConfigServices siteConfig = new SiteConfigServices(_unitOfWork);
            var dataConfig = siteConfig.GetByKey("OfficeID_Save");
            request.bookingOID = dataConfig.ConfigValue;// ConfigurationManager.AppSettings["OfficeID"];
            request.RobinhoodID = robinhoodID;
            Entities.RobinhoodPNR.PNR response = new Entities.RobinhoodPNR.PNR();
            response.RobinhoodID = robinhoodID;
            response.airSellEntities = null;
            response.type = "A";//A:Amadeus
            string url = ConfigurationManager.AppSettings["AIRSELL.URL"];
            Entities.AirSell.Request airSellRequest = request.GetAirSellRequest();
            string requestJson = JsonConvert.SerializeObject(airSellRequest);

            string json = HttpUtility.postJSON(url, requestJson);
            Log.Info("AIR_SELL");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.AirSell.Response airSellResponse = new Entities.AirSell.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");
                airSellResponse = BL.Entities.AirSell.Response.FromJson(json);
            }

            if (airSellResponse.Error != null)
            {
                response.isError = true;
                response.errorCode = "4001";
                response.errorMessage = airSellResponse.Error.ErrorMessage + "(" + airSellResponse.Error.ErrorCode + ")";//(288)
                return response;
            }

            bool isNotConfirmStatus = false;
            List<AirSellEntities> airSellEntities = new List<AirSellEntities>();
            AirSellEntities _airSellEntities = new AirSellEntities();
            if (airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail != null)
            {

                if (airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation != null)//oneway
                {
                    string status = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.ActionDetails.StatusCode;
                    _airSellEntities = new AirSellEntities();
                    _airSellEntities.tripType = "DEPART";
                    _airSellEntities.seq = 1;
                    _airSellEntities.boardPoint = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.BoardPointDetails.TrueLocationId;
                    _airSellEntities.offpoint = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.OffpointDetails.TrueLocationId;
                    _airSellEntities.airlineCode = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.CompanyDetails.MarketingCompany;
                    _airSellEntities.flightNumber = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.FlightIdentification.FlightNumber;
                    _airSellEntities.status = status;
                    airSellEntities.Add(_airSellEntities);

                    if (status != "HK" && status != "OK")
                    {
                        isNotConfirmStatus = true;
                    }
                }
                else
                {
                    int iSeq = 0;
                    foreach (var itin in airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.SegmentInformationElementArray)
                    {
                        iSeq++;
                        string status = itin.ActionDetails.StatusCode;
                        _airSellEntities = new AirSellEntities();
                        _airSellEntities.tripType = "DEPART";
                        _airSellEntities.seq = iSeq;
                        _airSellEntities.boardPoint = itin.FlightDetails.BoardPointDetails.TrueLocationId;
                        _airSellEntities.offpoint = itin.FlightDetails.OffpointDetails.TrueLocationId;
                        _airSellEntities.airlineCode = itin.FlightDetails.CompanyDetails.MarketingCompany;
                        _airSellEntities.flightNumber = itin.FlightDetails.FlightIdentification.FlightNumber;
                        _airSellEntities.status = status;
                        airSellEntities.Add(_airSellEntities);

                        if (status != "HK" && status != "OK")
                        {
                            isNotConfirmStatus = true;
                        }
                    }
                }
            }
            else//Roundtrip
            {
                int iRoute = 0;
                int iSeq = 0;
                foreach (var itineraryDetail in airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.ItineraryDetailElementArray)
                {
                    iSeq = 0;
                    iRoute++;

                    if (itineraryDetail.SegmentInformation.PurpleSegmentInformation != null)
                    {
                        iSeq++;
                        string status = itineraryDetail.SegmentInformation.PurpleSegmentInformation.ActionDetails.StatusCode;
                        _airSellEntities = new AirSellEntities();
                        _airSellEntities.tripType = iRoute == 1 ? "DEPART" : "RETURN";
                        _airSellEntities.seq = iSeq;
                        _airSellEntities.boardPoint = itineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.BoardPointDetails.TrueLocationId;
                        _airSellEntities.offpoint = itineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.OffpointDetails.TrueLocationId;
                        _airSellEntities.airlineCode = itineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.CompanyDetails.MarketingCompany;
                        _airSellEntities.flightNumber = itineraryDetail.SegmentInformation.PurpleSegmentInformation.FlightDetails.FlightIdentification.FlightNumber;
                        _airSellEntities.status = status;
                        airSellEntities.Add(_airSellEntities);
                        if (status != "HK" && status != "OK")
                        {
                            isNotConfirmStatus = true;
                        }
                    }
                    else
                    {

                        foreach (var itin in itineraryDetail.SegmentInformation.SegmentInformationElementArray)
                        {
                            iSeq++;
                            string status = itin.ActionDetails.StatusCode;
                            _airSellEntities = new AirSellEntities();
                            _airSellEntities.tripType = iRoute == 1 ? "DEPART" : "RETURN";
                            _airSellEntities.seq = iSeq;
                            _airSellEntities.boardPoint = itin.FlightDetails.BoardPointDetails.TrueLocationId;
                            _airSellEntities.offpoint = itin.FlightDetails.OffpointDetails.TrueLocationId;
                            _airSellEntities.airlineCode = itin.FlightDetails.CompanyDetails.MarketingCompany;
                            _airSellEntities.flightNumber = itin.FlightDetails.FlightIdentification.FlightNumber;
                            _airSellEntities.status = status;
                            airSellEntities.Add(_airSellEntities);

                            if (status != "HK" && status != "OK")
                            {
                                isNotConfirmStatus = true;
                            }
                        }
                    }
                }
            }

            if (isNotConfirmStatus)
            {
                response.isError = true;
                response.errorCode = "4002";
                response.errorMessage = "FLIGHTS NOT CONFIRM";
                response.airSellEntities = airSellEntities;
                return response;
            }

            //PNR
            Entities.PNR.Request pnrReuest = new Entities.PNR.Request();
            pnrReuest.session = new Entities.PNR.Session();
            pnrReuest.session.isStateFull = true;
            pnrReuest.session.InSeries = true;
            pnrReuest.session.End = false;
            pnrReuest.session.SessionId = airSellResponse.AirSellFromRecommendationReply.Session.SessionId;
            pnrReuest.session.SequenceNumber = (int.Parse(airSellResponse.AirSellFromRecommendationReply.Session.SequenceNumber) + 1).ToString();
            pnrReuest.session.SecurityToken = airSellResponse.AirSellFromRecommendationReply.Session.SecurityToken;
            pnrReuest.bookingOID = request.bookingOID;//officeID

            pnrReuest.fop = "CA";
            pnrReuest.remarkType = "RM";
            pnrReuest.remarks = new List<string>();
            //pnrReuest.remarks.Add("Gogojii WEB BOOKING");
            if (request.remarks != null && request.remarks.Count > 0)
            {
                foreach (string rm in request.remarks)
                {
                    pnrReuest.remarks.Add(rm);
                }
            }
            //pnrReuest.remarks.Add("ADTSALE : " + (request.adtFare.net - request.adtFare.tax - request.adtFare.qtax).ToString());//move qtax to fare
            pnrReuest.remarks.Add("ADTSALE : " + (request.adtFare.net - request.adtFare.tax).ToString("N2"));
            //pnrReuest.remarks.Add("ADTNET : " + request.adtFare.lessFare);//move qtax to fare
            pnrReuest.remarks.Add("ADTNET : " + (request.adtFare.lessFare).ToString("N2"));
            //pnrReuest.remarks.Add("ADTTAX : " + (request.adtFare.tax + request.adtFare.qtax).ToString());//move qtax to fare
            pnrReuest.remarks.Add("ADTTAX : " + (request.adtFare.tax).ToString("N2"));
            if (request.noOfChildren > 0)
            {
                pnrReuest.remarks.Add("CHDSALE : " + (request.chdFare.net - request.chdFare.tax).ToString("N2"));
                pnrReuest.remarks.Add("CHDNET : " + (request.chdFare.lessFare).ToString("N2"));
                pnrReuest.remarks.Add("CHDTAX : " + (request.chdFare.tax).ToString("N2"));
            }
            else
            {
                pnrReuest.remarks.Add("CHDSALE : 0.00");
                pnrReuest.remarks.Add("CHDNET :  0.00");
                pnrReuest.remarks.Add("CHDTAX : 0.00");
            }
            if (request.noOfInfants > 0)
            {
                pnrReuest.remarks.Add("INFSALE : " + (request.infFare.net - request.infFare.tax).ToString("N2"));
                pnrReuest.remarks.Add("INFNET : " + (request.infFare.lessFare).ToString("N2"));
                pnrReuest.remarks.Add("INFTAX : " + (request.infFare.tax).ToString("N2"));
            }
            else
            {
                pnrReuest.remarks.Add("INFSALE : 0.00");
                pnrReuest.remarks.Add("INFNET :  0.00");
                pnrReuest.remarks.Add("INFTAX : 0.00");
            }
            bool bHaveRP = false;
            bool bHaveRU = false;
            if (request.multiFlight != null && request.multiFlight.Count > 0)
            {
                string depFB = "";
                foreach (var depF in request.multiFlight)
                {
                    foreach (var depM in depF)
                    {
                        depFB += (depFB == "" ? "" : "/") + depM.fareBasis;

                        if (depM.fareType == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                    }
                }
                pnrReuest.remarks.Add("MULTIFAREBASIS : " + depFB);
                pnrReuest.remarks.Add("Pricing With FXP" + (bHaveRU ? (bHaveRP ? "/R,UP" : "/R,U") : ""));
            }
            else
            {
                string depFB = "";
                foreach (var depF in request.depFlight)
                {
                    depFB += (depFB == "" ? "" : "/") + depF.fareBasis;

                    if (depF.fareType == "RP")
                    {
                        bHaveRP = true;
                    }
                    else
                    {
                        bHaveRU = true;
                    }
                }
                pnrReuest.remarks.Add("DEPARTFAREBASIS : " + depFB);
                if (request.retFlight != null && request.retFlight.Count > 0)
                {
                    string retFB = "";
                    foreach (var retF in request.retFlight)
                    {
                        retFB += (retFB == "" ? "" : "/") + retF.fareBasis;

                        if (retF.fareType == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }

                    }
                    pnrReuest.remarks.Add("RETURNFAREBASIS : " + retFB);
                }
                pnrReuest.remarks.Add("Pricing With FXP" + (bHaveRU ? (bHaveRP ? "/R,UP" : "/R,U") : ""));
            }
            pnrReuest.remarks.Add("TransactionID : " + response.RobinhoodID);
            if (request.bookingURN != null && request.bookingURN.Length > 0)
            {
                pnrReuest.remarks.Add("BookingURN : " + request.bookingURN);
            }
            if (request.promotionGroupCode != null && request.promotionGroupCode.Count > 0)
            {
                pnrReuest.remarks.Add("Promotion Group Code : " + request.promotionGroupCode[0]);
            }
            //pnrReuest.ticketIndicator = "TL";
            //pnrReuest.ticketTimeLimitDate = DateTime.Now.AddHours(48); //request.TKTL;
            dataConfig = siteConfig.GetByKey("TicketIndicator");
            pnrReuest.ticketIndicator = dataConfig.ConfigValue;//ConfigurationManager.AppSettings["TicketIndicator"]; //"TL";

            dataConfig = siteConfig.GetByKey("TicketTimeLimitHour");
            pnrReuest.ticketTimeLimitDate = DateTime.Now.AddMinutes(int.Parse(dataConfig.ConfigValue));
            pnrReuest.passengerList = new List<Entities.PNR.PassengerList>();
            List<Entities.RobinhoodPax.SpecialRequest> specialRequests = null;
            Entities.RobinhoodPax.SpecialRequest SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
            string gender = "";
            for (int i = 0; i < request.adtPaxs.Count; i++)
            {
                Entities.PNR.PassengerList pax = new Entities.PNR.PassengerList();
                pax.paxNo = i + 1;
                pax.infantFlag = request.infPaxs == null ? false : (request.infPaxs.Count >= (i + 1));
                pax.haveDOB = false;
                pax.DOB = new DateTime();
                pax.age = 25;
                pax.withADT = 0;
                pax.ptNo = "";
                pax.titleName = request.adtPaxs[i].title.Trim();
                pax.firstName = request.adtPaxs[i].firstname.Trim();
                pax.lastName = request.adtPaxs[i].lastname.Trim();
                pax.middleName = request.adtPaxs[i].middlename.Trim();
                pax.paxType = "ADT";
                pax.paxID = (i + 1).ToString();
                pax.withInsurance = false;
                if (request.adtPaxs[i].passportNumber != null && request.adtPaxs[i].passportNumber.Length > 0)
                {
                    gender = (pax.titleName.ToUpper() == "MR" || pax.titleName.ToUpper() == "PHRA") ? "M" : "F";
                    pax.passportNumber = String.Format("P-{6}-{0}-{7}-{1}-{2}-{3}-{4}-{5}", request.adtPaxs[i].passportNumber, request.adtPaxs[i].birthday.ToString("ddMMMyy"), gender, request.adtPaxs[i].passportExpiryDate.ToString("ddMMMyy"), request.adtPaxs[i].lastname, request.adtPaxs[i].firstname, request.adtPaxs[i].passportIssuingCountry, request.adtPaxs[i].passportNationality);
                }

                if (request.adtPaxs[i].frequentFlyList != null && request.adtPaxs[i].frequentFlyList.Count > 0)
                {
                    pax.frequentFlyList = request.adtPaxs[i].frequentFlyList;
                }
                else
                {
                    pax.frequentFlyerAirline = request.adtPaxs[i].frequencyFlyerAirline != null ? request.adtPaxs[i].frequencyFlyerAirline : null;
                    pax.frequentFlyerNumber = request.adtPaxs[i].frequencyFlyerNumber != null ? request.adtPaxs[i].frequencyFlyerNumber : null;
                }
                if ((request.adtPaxs[i].seatsRequest != null && request.adtPaxs[i].seatsRequest.Count > 0)
                    || (request.adtPaxs[i].seatRequest != null && request.adtPaxs[i].seatRequest.Length > 0)
                    || (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0))
                {
                    specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();
                    if (request.multiFlight != null && request.multiFlight.Count > 0)
                    {
                        int iSeg = 0;
                        List<List<Entities.RobinhoodPax.SpecialRequest>> multi_specialRequests = new List<List<Entities.RobinhoodPax.SpecialRequest>>();
                        for (int iC = 0; iC < request.multiFlight.Count; iC++)
                        {

                            specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();
                            for (int iN = 0; iN < request.multiFlight[iC].Count; iN++)
                            {
                                SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                SpecialRequest.flightST = iSeg + 1;

                                if (request.adtPaxs[i].seatsRequest != null && request.adtPaxs[i].seatsRequest.Count > 0)
                                {
                                    SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                    SpecialRequest.seat.seatType = request.adtPaxs[i].seatsRequest[iSeg].seatType;
                                    SpecialRequest.seat.seatRefNo = request.adtPaxs[i].seatsRequest[iSeg].seatRefNo;
                                }
                                else
                                {
                                    if (request.adtPaxs[i].seatRequest != null && request.adtPaxs[i].seatRequest.Length > 0)
                                    {
                                        SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                        SpecialRequest.seat.seatType = request.adtPaxs[i].seatRequest == "W" ? "NSSW" : "NSSA";
                                        SpecialRequest.seat.seatRefNo = "";
                                    }
                                }
                                if (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0)
                                {
                                    SpecialRequest.mealCode = request.adtPaxs[i].mealRequest;
                                }
                                else
                                {
                                    SpecialRequest.mealCode = "NONE";
                                }
                                specialRequests.Add(SpecialRequest);
                                iSeg++;
                            }
                            multi_specialRequests.Add(specialRequests);
                        }
                        pax.specialRequestList = multi_specialRequests;
                    }
                    else
                    {
                        for (int d = 0; d < request.depFlight.Count; d++)
                        {
                            SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                            SpecialRequest.flightST = d + 1;
                            if (request.adtPaxs[i].seatsRequest != null && request.adtPaxs[i].seatsRequest.Count > 0)
                            {
                                SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                SpecialRequest.seat.seatType = request.adtPaxs[i].seatsRequest[d].seatType;
                                SpecialRequest.seat.seatRefNo = request.adtPaxs[i].seatsRequest[d].seatRefNo;
                            }
                            else
                            {
                                if (request.adtPaxs[i].seatRequest != null && request.adtPaxs[i].seatRequest.Length > 0)
                                {
                                    SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                    SpecialRequest.seat.seatType = request.adtPaxs[i].seatRequest == "W" ? "NSSW" : "NSSA";
                                    SpecialRequest.seat.seatRefNo = "";
                                }
                            }
                            if (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0)
                            {
                                SpecialRequest.mealCode = request.adtPaxs[i].mealRequest;
                            }
                            else
                            {
                                SpecialRequest.mealCode = "NONE";
                            }
                            specialRequests.Add(SpecialRequest);
                        }
                        pax.depSpecialRequest = specialRequests;
                        if (request.retFlight != null && request.retFlight.Count > 0)
                        {
                            specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();

                            for (int d = request.depFlight.Count; d < request.retFlight.Count + request.depFlight.Count; d++)
                            {
                                SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                SpecialRequest.flightST = d + 1;
                                if (request.adtPaxs[i].seatsRequest != null && request.adtPaxs[i].seatsRequest.Count > 0)
                                {
                                    SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                    SpecialRequest.seat.seatType = request.adtPaxs[i].seatsRequest[d].seatType;
                                    SpecialRequest.seat.seatRefNo = request.adtPaxs[i].seatsRequest[d].seatRefNo;
                                }
                                else
                                {
                                    if (request.adtPaxs[i].seatRequest != null && request.adtPaxs[i].seatRequest.Length > 0)
                                    {
                                        SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                        SpecialRequest.seat.seatType = request.adtPaxs[i].seatRequest == "W" ? "NSSW" : "NSSA";
                                        SpecialRequest.seat.seatRefNo = "";
                                    }
                                }
                                if (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0)
                                {
                                    SpecialRequest.mealCode = request.adtPaxs[i].mealRequest;
                                }
                                else
                                {
                                    SpecialRequest.mealCode = "NONE";
                                }
                                specialRequests.Add(SpecialRequest);
                            }
                            pax.retSpecialRequest = specialRequests;
                        }
                    }
                }

                if (request.infPaxs != null && request.infPaxs.Count >= (i + 1))
                {
                    pax.infantPassenger = new Entities.PNR.InfantPassenger();
                    pax.infantPassenger.paxNo = (request.adtPaxs == null ? 0 : request.adtPaxs.Count) + (request.chdPaxs == null ? 0 : request.chdPaxs.Count) + i + 1;
                    pax.infantPassenger.infantFlag = false;
                    pax.infantPassenger.haveDOB = true;
                    pax.infantPassenger.DOB = request.infPaxs[i].birthday;
                    pax.infantPassenger.age = 1;
                    pax.infantPassenger.withADT = 0;
                    pax.infantPassenger.titleName = "";
                    pax.infantPassenger.firstName = request.infPaxs[i].firstname.Trim();
                    pax.infantPassenger.lastName = request.infPaxs[i].lastname.Trim();
                    pax.infantPassenger.paxType = "INF";
                    pax.infantPassenger.paxID = ((request.adtPaxs == null ? 0 : request.adtPaxs.Count) + (request.chdPaxs == null ? 0 : request.chdPaxs.Count) + i + 1).ToString();
                    if (request.infPaxs[i].passportNumber != null && request.infPaxs[i].passportNumber.Length > 0)
                    {
                        gender = pax.titleName.ToUpper() == "MSTR" ? "MI" : "FI";
                        pax.infantPassenger.passportNumber = String.Format("P-{6}-{0}-{7}-{1}-{2}-{3}-{4}-{5}", request.infPaxs[i].passportNumber, request.infPaxs[i].birthday.ToString("ddMMMyy"), gender, request.infPaxs[i].passportExpiryDate.ToString("ddMMMyy"), request.infPaxs[i].lastname, request.infPaxs[i].firstname, request.infPaxs[i].passportIssuingCountry, request.infPaxs[i].passportNationality);
                    }
                    if (request.infPaxs[i].mealRequest != null && request.infPaxs[i].mealRequest.Length > 0)
                    {
                        specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();
                        if (request.multiFlight != null && request.multiFlight.Count > 0)
                        {
                            int iSeg = 0;
                            List<List<Entities.RobinhoodPax.SpecialRequest>> multi_specialRequests = new List<List<Entities.RobinhoodPax.SpecialRequest>>();
                            for (int iC = 0; iC < request.multiFlight.Count; iC++)
                            {
                                specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();
                                for (int iN = 0; iN < request.multiFlight[iC].Count; iN++)
                                {
                                    SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                    SpecialRequest.flightST = iSeg + 1;
                                    if (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0)
                                    {
                                        SpecialRequest.mealCode = request.adtPaxs[i].mealRequest;
                                    }
                                    else
                                    {
                                        SpecialRequest.mealCode = "NONE";
                                    }
                                    specialRequests.Add(SpecialRequest);
                                    iSeg++;
                                }
                                multi_specialRequests.Add(specialRequests);
                            }
                            pax.specialRequestList = multi_specialRequests;
                        }
                        else
                        {
                            for (int d = 0; d < request.depFlight.Count; d++)
                            {
                                SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                SpecialRequest.flightST = d + 1;
                                if (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0)
                                {
                                    SpecialRequest.mealCode = request.adtPaxs[i].mealRequest;
                                }
                                else
                                {
                                    SpecialRequest.mealCode = "NONE";
                                }
                                specialRequests.Add(SpecialRequest);
                            }
                            pax.depSpecialRequest = specialRequests;
                            if (request.retFlight != null && request.retFlight.Count > 0)
                            {
                                specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();

                                for (int d = request.depFlight.Count; d < request.retFlight.Count + request.depFlight.Count; d++)
                                {
                                    SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                    SpecialRequest.flightST = d + 1;
                                    if (request.adtPaxs[i].mealRequest != null && request.adtPaxs[i].mealRequest.Length > 0)
                                    {
                                        SpecialRequest.mealCode = request.adtPaxs[i].mealRequest;
                                    }
                                    else
                                    {
                                        SpecialRequest.mealCode = "NONE";
                                    }
                                    specialRequests.Add(SpecialRequest);
                                }
                                pax.retSpecialRequest = specialRequests;
                            }
                        }
                    }
                }

                pnrReuest.passengerList.Add(pax);
            }

            if (request.chdPaxs != null)
            {
                for (int i = 0; i < request.chdPaxs.Count; i++)
                {
                    Entities.PNR.PassengerList pax = new Entities.PNR.PassengerList();
                    pax.paxNo = i + request.adtPaxs.Count;
                    pax.infantFlag = false;
                    pax.haveDOB = true;
                    pax.DOB = request.chdPaxs[i].birthday;
                    pax.age = 8;
                    pax.withADT = 0;
                    pax.ptNo = "";
                    pax.titleName = request.chdPaxs[i].title.Trim();
                    pax.firstName = request.chdPaxs[i].firstname.Trim();
                    pax.lastName = request.chdPaxs[i].lastname.Trim();
                    pax.middleName = request.chdPaxs[i].middlename.Trim();
                    pax.paxType = "CHD";
                    pax.withInsurance = false;
                    pax.paxID = (i + request.adtPaxs.Count).ToString();
                    if (request.chdPaxs[i].passportNumber != null && request.chdPaxs[i].passportNumber.Length > 0)
                    {
                        gender = pax.titleName.ToUpper() == "MSTR" ? "M" : "F";
                        pax.passportNumber = String.Format("P-{6}-{0}-{7}-{1}-{2}-{3}-{4}-{5}", request.chdPaxs[i].passportNumber, request.chdPaxs[i].birthday.ToString("ddMMMyy"), gender, request.chdPaxs[i].passportExpiryDate.ToString("ddMMMyy"), request.chdPaxs[i].lastname, request.chdPaxs[i].firstname, request.chdPaxs[i].passportIssuingCountry, request.chdPaxs[i].passportNationality);
                    }
                    if (request.chdPaxs[i].frequentFlyList != null && request.chdPaxs[i].frequentFlyList.Count > 0)
                    {
                        pax.frequentFlyList = request.chdPaxs[i].frequentFlyList;
                    }
                    else
                    {
                        pax.frequentFlyerAirline = request.chdPaxs[i].frequencyFlyerAirline != null ? request.chdPaxs[i].frequencyFlyerAirline : null;
                        pax.frequentFlyerNumber = request.chdPaxs[i].frequencyFlyerNumber != null ? request.chdPaxs[i].frequencyFlyerNumber : null;
                    }

                    if ((request.chdPaxs[i].seatsRequest != null && request.chdPaxs[i].seatsRequest.Count > 0)
                        || (request.chdPaxs[i].seatRequest != null && request.chdPaxs[i].seatRequest.Length > 0)
                        || (request.chdPaxs[i].mealRequest != null && request.chdPaxs[i].mealRequest.Length > 0))
                    {
                        specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();
                        if (request.multiFlight != null && request.multiFlight.Count > 0)
                        {
                            int iSeg = 0;
                            List<List<Entities.RobinhoodPax.SpecialRequest>> multi_specialRequests = new List<List<Entities.RobinhoodPax.SpecialRequest>>();
                            for (int iC = 0; iC < request.multiFlight.Count; iC++)
                            {
                                specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();
                                for (int iN = 0; iN < request.multiFlight[iC].Count; iN++)
                                {
                                    SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                    SpecialRequest.flightST = iSeg + 1;
                                    if (request.chdPaxs[i].seatsRequest != null && request.chdPaxs[i].seatsRequest.Count > 0)
                                    {
                                        SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                        SpecialRequest.seat.seatType = request.chdPaxs[i].seatsRequest[iSeg].seatType;
                                        SpecialRequest.seat.seatRefNo = request.chdPaxs[i].seatsRequest[iSeg].seatRefNo;
                                    }
                                    else
                                    {
                                        if (request.chdPaxs[i].seatRequest != null && request.chdPaxs[i].seatRequest.Length > 0)
                                        {
                                            SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                            SpecialRequest.seat.seatType = request.chdPaxs[i].seatRequest == "W" ? "NSSW" : "NSSA";
                                            SpecialRequest.seat.seatRefNo = "";
                                        }
                                    }
                                    if (request.chdPaxs[i].mealRequest != null && request.chdPaxs[i].mealRequest.Length > 0)
                                    {
                                        SpecialRequest.mealCode = request.chdPaxs[i].mealRequest;
                                    }
                                    else
                                    {
                                        SpecialRequest.mealCode = "NONE";
                                    }
                                    specialRequests.Add(SpecialRequest);
                                    iSeg++;
                                }
                                multi_specialRequests.Add(specialRequests);
                            }
                            pax.specialRequestList = multi_specialRequests;
                        }
                        else
                        {
                            for (int d = 0; d < request.depFlight.Count; d++)
                            {
                                SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                SpecialRequest.flightST = d + 1;
                                if (request.chdPaxs[i].seatsRequest != null && request.chdPaxs[i].seatsRequest.Count > 0)
                                {
                                    SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                    SpecialRequest.seat.seatType = request.chdPaxs[i].seatsRequest[d].seatType;
                                    SpecialRequest.seat.seatRefNo = request.chdPaxs[i].seatsRequest[d].seatRefNo;
                                }
                                else
                                {
                                    if (request.chdPaxs[i].seatRequest != null && request.chdPaxs[i].seatRequest.Length > 0)
                                    {
                                        SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                        SpecialRequest.seat.seatType = request.chdPaxs[i].seatRequest == "W" ? "NSSW" : "NSSA";
                                        SpecialRequest.seat.seatRefNo = "";
                                    }
                                }
                                if (request.chdPaxs[i].mealRequest != null && request.chdPaxs[i].mealRequest.Length > 0)
                                {
                                    SpecialRequest.mealCode = request.chdPaxs[i].mealRequest;
                                }
                                specialRequests.Add(SpecialRequest);
                            }
                            pax.depSpecialRequest = specialRequests;
                            if (request.retFlight != null && request.retFlight.Count > 0)
                            {
                                specialRequests = new List<Entities.RobinhoodPax.SpecialRequest>();

                                for (int d = request.depFlight.Count; d < request.retFlight.Count + request.depFlight.Count; d++)
                                {
                                    SpecialRequest = new Entities.RobinhoodPax.SpecialRequest();
                                    SpecialRequest.flightST = d + 1;
                                    if (request.chdPaxs[i].seatsRequest != null && request.chdPaxs[i].seatsRequest.Count > 0)
                                    {
                                        SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                        SpecialRequest.seat.seatType = request.chdPaxs[i].seatsRequest[d].seatType;
                                        SpecialRequest.seat.seatRefNo = request.chdPaxs[i].seatsRequest[d].seatRefNo;
                                    }
                                    else
                                    {
                                        if (request.chdPaxs[i].seatRequest != null && request.chdPaxs[i].seatRequest.Length > 0)
                                        {
                                            SpecialRequest.seat = new Entities.RobinhoodPax.Seat();
                                            SpecialRequest.seat.seatType = request.chdPaxs[i].seatRequest == "W" ? "NSSW" : "NSSA";
                                            SpecialRequest.seat.seatRefNo = "";
                                        }
                                    }
                                    if (request.chdPaxs[i].mealRequest != null && request.chdPaxs[i].mealRequest.Length > 0)
                                    {
                                        SpecialRequest.mealCode = request.chdPaxs[i].mealRequest;
                                    }
                                    else
                                    {
                                        SpecialRequest.mealCode = "NONE";
                                    }
                                    specialRequests.Add(SpecialRequest);
                                }
                                pax.retSpecialRequest = specialRequests;
                            }
                        }
                    }
                    pnrReuest.passengerList.Add(pax);
                }
            }
            pnrReuest.refEmail = String.IsNullOrEmpty(request.contactInfo.email) ? request.adtPaxs[0].email : request.contactInfo.email;
            pnrReuest.refMobileNumber = String.IsNullOrEmpty(request.contactInfo.telNo) ? request.adtPaxs[0].telNo : request.contactInfo.telNo;

            url = ConfigurationManager.AppSettings["PNR.URL"];
            requestJson = JsonConvert.SerializeObject(pnrReuest);

            json = HttpUtility.postJSON(url, requestJson);

            Log.Info("CREATE_PNR");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrResponse = BL.Entities.PNR.Response.FromJson(json);
            }

            if (pnrResponse.Error != null)
            {
                response.isError = true;
                response.errorCode = "4003";
                response.errorMessage = pnrResponse.Error.ErrorMessage + "(" + pnrResponse.Error.ErrorCode + ")";
                return response;
            }

            ElementManagement elePax = null;
            Entities.PNR.PassengerList one_pax = null;
            TravellerInfo traveller = null;
            if (pnrResponse.PnrReply.TravellerInfo.TravellerInfoElementArray != null)
            {
                string paxType = "";
                string pax_FirstName = "";
                string pax_LastName = "";
                for (int i = 0; i < pnrResponse.PnrReply.TravellerInfo.TravellerInfoElementArray.Count; i++)
                {
                    traveller = pnrResponse.PnrReply.TravellerInfo.TravellerInfoElementArray[i];
                    if (traveller != null)
                    {
                        elePax = traveller.ElementManagementPassenger;
                        pax_LastName = traveller.PassengerData.PurplePassengerData != null ? traveller.PassengerData.PurplePassengerData.TravellerInformation.Traveller.Surname : "";
                        if (traveller.PassengerData.PassengerDataElementArray != null)
                        {
                            if (traveller.PassengerData.PassengerDataElementArray.Find(x => x.TravellerInformation.Passenger.PassengerElementArray != null) != null)
                            {

                                var allPassengerElementArray = traveller.PassengerData.PassengerDataElementArray;
                                foreach (var PassengerElementArray in allPassengerElementArray)
                                {
                                    if (pax_LastName.Length == 0)
                                    {
                                        pax_LastName = PassengerElementArray.TravellerInformation.Traveller.Surname;
                                    }
                                    foreach (var allTravellerInformation in PassengerElementArray.TravellerInformation.Passenger.PassengerElementArray)
                                    {

                                        paxType = allTravellerInformation.Type;
                                        pax_FirstName = allTravellerInformation.FirstName;//"firstName": "NING TEST MR"

                                        one_pax = pnrReuest.passengerList.Find(x => x.paxType.ToUpper() == paxType && x.lastName.ToUpper() == pax_LastName && (String.Format("{0} {1}{2}", x.firstName, (x.middleName != null && x.middleName.Length > 0 ? (x.middleName + " ") : ""), x.titleName).ToUpper() == pax_FirstName));
                                        if (one_pax != null)
                                        {
                                            one_pax.ptNo = elePax.Reference.Number;
                                        }
                                    }

                                }

                            }
                            else//chd
                            {
                                foreach (var allPaxDataArray in traveller.PassengerData.PassengerDataElementArray)
                                {
                                    pax_LastName = allPaxDataArray.TravellerInformation.Traveller.Surname;
                                    paxType = allPaxDataArray.TravellerInformation.Passenger.PurplePassenger.Type;
                                    pax_FirstName = allPaxDataArray.TravellerInformation.Passenger.PurplePassenger.FirstName;//"firstName": "NING TEST MR"

                                    one_pax = pnrReuest.passengerList.Find(x => x.paxType.ToUpper() == paxType && x.lastName.ToUpper() == pax_LastName && (String.Format("{0} {1}{2}", x.firstName, (x.middleName != null && x.middleName.Length > 0 ? (x.middleName + " ") : ""), x.titleName).ToUpper() == pax_FirstName));
                                    if (one_pax != null)
                                    {
                                        one_pax.ptNo = elePax.Reference.Number;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PassengerElementArray != null)
                            {
                                pax_LastName = traveller.PassengerData.PurplePassengerData.TravellerInformation.Traveller.Surname;
                                paxType = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PassengerElementArray[0].Type;
                                pax_FirstName = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PassengerElementArray[0].FirstName;
                                one_pax = pnrReuest.passengerList.Find(x => x.paxType.ToUpper() == paxType && x.lastName.ToUpper() == pax_LastName && (String.Format("{0} {1}{2}", x.firstName, (x.middleName != null && x.middleName.Length > 0 ? (x.middleName + " ") : ""), x.titleName).ToUpper() == pax_FirstName));
                                if (one_pax != null)
                                {
                                    one_pax.ptNo = elePax.Reference.Number;
                                }
                                //inf
                                paxType = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PassengerElementArray[1].Type;
                                pax_FirstName = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PassengerElementArray[1].FirstName;
                                one_pax = pnrReuest.passengerList.Find(x => x.paxType.ToUpper() == paxType && x.lastName.ToUpper() == pax_LastName && (String.Format("{0} {1}{2}", x.firstName, (x.middleName != null && x.middleName.Length > 0 ? (x.middleName + " ") : ""), x.titleName).ToUpper() == pax_FirstName));
                                if (one_pax != null)
                                {
                                    one_pax.ptNo = elePax.Reference.Number;
                                }
                            }
                            else
                            {
                                paxType = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PurplePassenger.Type;
                                pax_FirstName = traveller.PassengerData.PurplePassengerData.TravellerInformation.Passenger.PurplePassenger.FirstName;
                                one_pax = pnrReuest.passengerList.Find(x => x.paxType.ToUpper() == paxType && x.lastName.ToUpper() == pax_LastName && (String.Format("{0} {1}{2}", x.firstName, (x.middleName != null && x.middleName.Length > 0 ? (x.middleName + " ") : ""), x.titleName).ToUpper() == pax_FirstName));
                                if (one_pax != null)
                                {
                                    one_pax.ptNo = elePax.Reference.Number;
                                }
                            }

                        }
                    }

                }

            }
            else
            {
                elePax = pnrResponse.PnrReply.TravellerInfo.PurpleTravellerInfo.ElementManagementPassenger;
                if (elePax != null)
                {
                    one_pax = pnrReuest.passengerList[0];
                    one_pax.ptNo = elePax.Reference.Number;
                }
            }


            //Pricing
            Entities.Pricing.Request pricingReuest = new Entities.Pricing.Request();
            BL.Entities.TST.Response tstResponse = new Entities.TST.Response();
            if (request.isPricingWithSegment)//for multi ticket
            {
                #region Pricing AND TST for multi ticket
                OriginDestinationDetails details = pnrResponse.PnrReply.OriginDestinationDetails;
                List<string> passengersNumber = new List<string>();
                if (pnrResponse.PnrReply.TravellerInfo.TravellerInfoElementArray != null)
                {
                    foreach (var eleTraveller in pnrResponse.PnrReply.TravellerInfo.TravellerInfoElementArray)
                    {
                        passengersNumber.Add(eleTraveller.ElementManagementPassenger.Reference.Number);
                    }
                }
                else
                {
                    passengersNumber.Add(pnrResponse.PnrReply.TravellerInfo.PurpleTravellerInfo.ElementManagementPassenger.Reference.Number);
                }

                List<Entities.Pricing.PricingSegment> segments = new List<Entities.Pricing.PricingSegment>();
                Entities.Pricing.PricingSegment segment = new Entities.Pricing.PricingSegment();
                bHaveRP = false;
                bHaveRU = false;
                pricingReuest = new Entities.Pricing.Request();
                pricingReuest.session = new Entities.Pricing.Session();
                pricingReuest.session.isStateFull = true;
                pricingReuest.session.InSeries = true;
                pricingReuest.session.End = false;
                pricingReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
                pricingReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
                pricingReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
                pricingReuest.bookingOID = request.bookingOID;


                pricingReuest.pricingTicketingIndicators = new List<string>();
                foreach (var depF in request.depFlight)
                {
                    segment = new Entities.Pricing.PricingSegment();
                    if (depF.fareType == "RP")
                    {
                        bHaveRP = true;
                    }
                    else
                    {
                        bHaveRU = true;
                    }
                    segment.segmentNumber = details.ItineraryInfo.ItineraryInfoElementArray.Find(x => x.TravelProduct.BoardpointDetail.CityCode == depF.depCity.code && x.TravelProduct.OffpointDetail.CityCode == depF.arrCity.code).ElementManagementItinerary.Reference.Number;
                    segment.passengersNumber = passengersNumber;
                    segments.Add(segment);
                }
                if (bHaveRU)
                {
                    pricingReuest.pricingTicketingIndicators.Add("RU");
                }
                if (bHaveRP)
                {
                    pricingReuest.pricingTicketingIndicators.Add("RP");
                }
                pricingReuest.segments = segments;
                pricingReuest.useCityOverride = false;
                pricingReuest.useCurrencyOverride = true;
                pricingReuest.currencyCode = "THB";
                pricingReuest.isCorporateNegotiateFare = false;
                pricingReuest.corporateCode = request.corporateCode != null ? request.corporateCode : "";
                Log.Info("pricingReuest.corporateCode=" + pricingReuest.corporateCode);
                url = ConfigurationManager.AppSettings["PRICINGWITHSEGMENT.URL"];
                requestJson = JsonConvert.SerializeObject(pricingReuest);

                json = HttpUtility.postJSON(url, requestJson);

                Log.Info("PRICINGWITHSEGMENT_DEPART");
                Log.Debug(requestJson);
                Log.Debug(json);

                BL.Entities.Pricing.Response pricingResponse = new Entities.Pricing.Response();
                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");

                    pricingResponse = BL.Entities.Pricing.Response.FromJson(json);
                }

                if (pricingResponse.Error != null)
                {
                    response.isError = true;
                    response.errorCode = "4004";
                    response.errorMessage = pricingResponse.Error.ErrorMessage + "(" + pricingResponse.Error.ErrorCode + ")";
                    return response;
                }

                List<string> uniqueReferences = new List<string>();
                if (pricingResponse.FarePricePnrWithBookingClassReply.FareList.PurpleFareList != null)
                {
                    uniqueReferences.Add(pricingResponse.FarePricePnrWithBookingClassReply.FareList.PurpleFareList.FareReference.UniqueReference);
                }
                else
                {
                    foreach (var f in pricingResponse.FarePricePnrWithBookingClassReply.FareList.FareListElementArray)
                    {
                        uniqueReferences.Add(f.FareReference.UniqueReference);
                    }
                }


                Entities.TST.Request tstRequest = new Entities.TST.Request();
                tstRequest.session = new Entities.TST.Session();
                tstRequest.bookingOID = request.bookingOID;
                tstRequest.session.isStateFull = true;
                tstRequest.session.InSeries = true;
                tstRequest.session.End = false;
                tstRequest.session.SessionId = pricingResponse.FarePricePnrWithBookingClassReply.Session.SessionId;
                tstRequest.session.SequenceNumber = (int.Parse(pricingResponse.FarePricePnrWithBookingClassReply.Session.SequenceNumber) + 1).ToString();
                tstRequest.session.SecurityToken = pricingResponse.FarePricePnrWithBookingClassReply.Session.SecurityToken;
                tstRequest.ClientIP = Utilities.HttpUtility.GetIPAddress();
                tstRequest.uniqueReferences = uniqueReferences;
                tstRequest.useBookingOfficeID = false;

                url = ConfigurationManager.AppSettings["TST.URL"];
                requestJson = JsonConvert.SerializeObject(tstRequest);

                json = HttpUtility.postJSON(url, requestJson);

                Log.Info("TST_DEPART");
                Log.Debug(requestJson);
                Log.Debug(json);

                tstResponse = new Entities.TST.Response();
                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");

                    tstResponse = BL.Entities.TST.Response.FromJson(json);
                }

                if (tstResponse.Error != null)
                {
                    response.isError = true;
                    response.errorCode = "4005";
                    response.errorMessage = tstResponse.Error.ErrorMessage + "(" + tstResponse.Error.ErrorCode + ")";
                    return response;
                }//if

                //for return

                segments = new List<Entities.Pricing.PricingSegment>();
                segment = new Entities.Pricing.PricingSegment();
                bHaveRP = false;
                bHaveRU = false;
                pricingReuest = new Entities.Pricing.Request();
                pricingReuest.session = new Entities.Pricing.Session();
                pricingReuest.session.isStateFull = true;
                pricingReuest.session.InSeries = true;
                pricingReuest.session.End = false;
                pricingReuest.session.SessionId = tstResponse.TicketCreateTstFromPricingReply.Session.SessionId;
                pricingReuest.session.SequenceNumber = (int.Parse(tstResponse.TicketCreateTstFromPricingReply.Session.SequenceNumber) + 1).ToString();
                pricingReuest.session.SecurityToken = tstResponse.TicketCreateTstFromPricingReply.Session.SecurityToken;
                pricingReuest.bookingOID = request.bookingOID;


                pricingReuest.pricingTicketingIndicators = new List<string>();
                foreach (var depF in request.retFlight)
                {
                    segment = new Entities.Pricing.PricingSegment();
                    if (depF.fareType == "RP")
                    {
                        bHaveRP = true;
                    }
                    else
                    {
                        bHaveRU = true;
                    }
                    segment.segmentNumber = details.ItineraryInfo.ItineraryInfoElementArray.Find(x => x.TravelProduct.BoardpointDetail.CityCode == depF.depCity.code && x.TravelProduct.OffpointDetail.CityCode == depF.arrCity.code).ElementManagementItinerary.Reference.Number;
                    segment.passengersNumber = passengersNumber;
                    segments.Add(segment);
                }
                if (bHaveRU)
                {
                    pricingReuest.pricingTicketingIndicators.Add("RU");
                }
                if (bHaveRP)
                {
                    pricingReuest.pricingTicketingIndicators.Add("RP");
                }
                pricingReuest.segments = segments;
                pricingReuest.useCityOverride = false;
                pricingReuest.useCurrencyOverride = true;
                pricingReuest.currencyCode = "THB";
                pricingReuest.isCorporateNegotiateFare = false;
                pricingReuest.corporateCode = request.corporateCode != null ? request.corporateCode : "";
                Log.Info("pricingReuest.corporateCode=" + pricingReuest.corporateCode);
                url = ConfigurationManager.AppSettings["PRICINGWITHSEGMENT.URL"];
                requestJson = JsonConvert.SerializeObject(pricingReuest);

                json = HttpUtility.postJSON(url, requestJson);

                Log.Info("PRICINGWITHSEGMENT_RETURN");
                Log.Debug(requestJson);
                Log.Debug(json);

                pricingResponse = new Entities.Pricing.Response();
                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");

                    pricingResponse = BL.Entities.Pricing.Response.FromJson(json);
                }

                if (pricingResponse.Error != null)
                {
                    response.isError = true;
                    response.errorCode = "4006";
                    response.errorMessage = pricingResponse.Error.ErrorMessage + "(" + pricingResponse.Error.ErrorCode + ")";
                    return response;
                }

                uniqueReferences = new List<string>();
                if (pricingResponse.FarePricePnrWithBookingClassReply.FareList.PurpleFareList != null)
                {
                    uniqueReferences.Add(pricingResponse.FarePricePnrWithBookingClassReply.FareList.PurpleFareList.FareReference.UniqueReference);
                }
                else
                {
                    foreach (var f in pricingResponse.FarePricePnrWithBookingClassReply.FareList.FareListElementArray)
                    {
                        uniqueReferences.Add(f.FareReference.UniqueReference);
                    }
                }


                tstRequest = new Entities.TST.Request();
                tstRequest.session = new Entities.TST.Session();
                tstRequest.bookingOID = request.bookingOID;
                tstRequest.session.isStateFull = true;
                tstRequest.session.InSeries = true;
                tstRequest.session.End = false;
                tstRequest.session.SessionId = pricingResponse.FarePricePnrWithBookingClassReply.Session.SessionId;
                tstRequest.session.SequenceNumber = (int.Parse(pricingResponse.FarePricePnrWithBookingClassReply.Session.SequenceNumber) + 1).ToString();
                tstRequest.session.SecurityToken = pricingResponse.FarePricePnrWithBookingClassReply.Session.SecurityToken;
                tstRequest.ClientIP = Utilities.HttpUtility.GetIPAddress();
                tstRequest.uniqueReferences = uniqueReferences;
                tstRequest.useBookingOfficeID = false;

                url = ConfigurationManager.AppSettings["TST.URL"];
                requestJson = JsonConvert.SerializeObject(tstRequest);

                json = HttpUtility.postJSON(url, requestJson);

                Log.Info("TST_RETURN");
                Log.Debug(requestJson);
                Log.Debug(json);

                tstResponse = new Entities.TST.Response();
                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");

                    tstResponse = BL.Entities.TST.Response.FromJson(json);
                }

                if (tstResponse.Error != null)
                {
                    response.isError = true;
                    response.errorCode = "4007";
                    response.errorMessage = tstResponse.Error.ErrorMessage + "(" + tstResponse.Error.ErrorCode + ")";
                    return response;
                }//if
                #endregion
            }
            else
            {
                #region Pricing and TST
                pricingReuest = new Entities.Pricing.Request();
                pricingReuest.session = new Entities.Pricing.Session();
                pricingReuest.session.isStateFull = true;
                pricingReuest.session.InSeries = true;
                pricingReuest.session.End = false;
                pricingReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
                pricingReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
                pricingReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
                pricingReuest.bookingOID = request.bookingOID;

                pricingReuest.pricingTicketingIndicators = new List<string>();
                //pricingReuest.pricingTicketingIndicators.Add("RU");
                //pricingReuest.pricingTicketingIndicators.Add("RP");
                //pricingReuest.pricingTicketingIndicators.Add("RLO");

                bHaveRP = false;
                bHaveRU = false;
                if (request.multiFlight != null && request.multiFlight.Count > 0)
                {
                    foreach (var allF in request.multiFlight)
                    {
                        foreach (var depF in allF)
                        {
                            if (depF.fareType == "RP")
                            {
                                bHaveRP = true;
                            }
                            else
                            {
                                bHaveRU = true;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var depF in request.depFlight)
                    {
                        if (depF.fareType == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                    }
                    if (request.retFlight != null && request.retFlight.Count > 0)
                    {
                        foreach (var depF in request.retFlight)
                        {
                            if (depF.fareType == "RP")
                            {
                                bHaveRP = true;
                            }
                            else
                            {
                                bHaveRU = true;
                            }
                        }
                    }
                }
                if (bHaveRU)
                {
                    pricingReuest.pricingTicketingIndicators.Add("RU");
                }
                if (bHaveRP)
                {
                    pricingReuest.pricingTicketingIndicators.Add("RP");
                }

                pricingReuest.useCityOverride = false;
                pricingReuest.useCurrencyOverride = true;
                pricingReuest.currencyCode = "THB";

                url = ConfigurationManager.AppSettings["PRICING.URL"];
                requestJson = JsonConvert.SerializeObject(pricingReuest);

                json = HttpUtility.postJSON(url, requestJson);

                Log.Info("PRICING");
                Log.Debug(requestJson);
                Log.Debug(json);

                BL.Entities.Pricing.Response pricingResponse = new Entities.Pricing.Response();
                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");

                    pricingResponse = BL.Entities.Pricing.Response.FromJson(json);
                }

                if (pricingResponse.Error != null)
                {
                    response.isError = true;
                    response.errorCode = "4004";
                    response.errorMessage = pricingResponse.Error.ErrorMessage + "(" + pricingResponse.Error.ErrorCode + ")";
                    return response;
                }

                List<string> uniqueReferences = new List<string>();
                if (pricingResponse.FarePricePnrWithBookingClassReply.FareList.PurpleFareList != null)
                {
                    uniqueReferences.Add(pricingResponse.FarePricePnrWithBookingClassReply.FareList.PurpleFareList.FareReference.UniqueReference);
                }
                else
                {
                    List<string> paxPTList = new List<string>();
                    bool bChkPT = false;
                    foreach (var f in pricingResponse.FarePricePnrWithBookingClassReply.FareList.FareListElementArray)
                    {
                        bChkPT = false;
                        if (f.PaxSegReference.RefDetails.RefDetailArray != null && f.PaxSegReference.RefDetails.RefDetailArray.Count > 0)
                        {
                            foreach (var pt in f.PaxSegReference.RefDetails.RefDetailArray)
                            {
                                if (!paxPTList.Contains(pt.RefNumber))
                                {
                                    paxPTList.Add(pt.RefNumber);
                                    bChkPT = true;
                                }
                            }
                        }
                        else
                        {
                            if (!paxPTList.Contains(f.PaxSegReference.RefDetails.RefDetail.RefNumber))
                            {
                                paxPTList.Add(f.PaxSegReference.RefDetails.RefDetail.RefNumber);
                                bChkPT = true;
                            }

                        }
                        if (bChkPT)
                        {
                            uniqueReferences.Add(f.FareReference.UniqueReference);
                        }
                    }
                }


                Entities.TST.Request tstRequest = new Entities.TST.Request();
                tstRequest.session = new Entities.TST.Session();
                tstRequest.bookingOID = request.bookingOID;
                tstRequest.session.isStateFull = true;
                tstRequest.session.InSeries = true;
                tstRequest.session.End = false;
                tstRequest.session.SessionId = pricingResponse.FarePricePnrWithBookingClassReply.Session.SessionId;
                tstRequest.session.SequenceNumber = (int.Parse(pricingResponse.FarePricePnrWithBookingClassReply.Session.SequenceNumber) + 1).ToString();
                tstRequest.session.SecurityToken = pricingResponse.FarePricePnrWithBookingClassReply.Session.SecurityToken;
                tstRequest.ClientIP = Utilities.HttpUtility.GetIPAddress();
                tstRequest.uniqueReferences = uniqueReferences;
                tstRequest.useBookingOfficeID = false;

                url = ConfigurationManager.AppSettings["TST.URL"];
                requestJson = JsonConvert.SerializeObject(tstRequest);

                json = HttpUtility.postJSON(url, requestJson);

                Log.Info("TST");
                Log.Debug(requestJson);
                Log.Debug(json);

                tstResponse = new Entities.TST.Response();
                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");

                    tstResponse = BL.Entities.TST.Response.FromJson(json);
                }

                if (tstResponse.Error != null)
                {
                    response.isError = true;
                    response.errorCode = "4005";
                    response.errorMessage = tstResponse.Error.ErrorMessage + "(" + tstResponse.Error.ErrorCode + ")";
                    return response;
                }
                #endregion Pricing and TST
            }


            //PNR
            Entities.PNR.Request pnrSavedReuest = new Entities.PNR.Request();
            pnrSavedReuest.session = new Entities.PNR.Session();
            pnrSavedReuest.session.isStateFull = true;
            pnrSavedReuest.session.InSeries = true;
            pnrSavedReuest.session.End = true;
            pnrSavedReuest.session.SessionId = tstResponse.TicketCreateTstFromPricingReply.Session.SessionId;
            pnrSavedReuest.session.SequenceNumber = (int.Parse(tstResponse.TicketCreateTstFromPricingReply.Session.SequenceNumber) + 1).ToString();
            pnrSavedReuest.session.SecurityToken = tstResponse.TicketCreateTstFromPricingReply.Session.SecurityToken;
            //pnrSavedReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
            //pnrSavedReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
            //pnrSavedReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
            pnrSavedReuest.bookingOID = request.bookingOID;
            pnrSavedReuest.optionCode = "11";
            pnrSavedReuest.rfLongFreeText = "ROBINHOOD API";

            pnrSavedReuest.departFlightSegmentCount = request.depFlight == null ? 0 : request.depFlight.Count;
            pnrSavedReuest.returnFlightSegmentCount = request.retFlight == null ? 0 : request.retFlight.Count;
            pnrSavedReuest.passengerList = pnrReuest.passengerList;
            pnrSavedReuest.accessOID = "";// "BKKIW38FO";

            pnrSavedReuest.ClientIP = Utilities.HttpUtility.GetIPAddress();

            pnrSavedReuest.fkOption = new FKOption();
            pnrSavedReuest.fkOption.generalIndicator = "K";
            pnrSavedReuest.fkOption.officeId = request.bookingOID;//officeID

            url = ConfigurationManager.AppSettings["SAVEPNR.URL"];
            requestJson = JsonConvert.SerializeObject(pnrSavedReuest);

            json = HttpUtility.postJSON(url, requestJson);

            Log.Info("SAVE_PNR");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrSavedResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrSavedResponse = BL.Entities.PNR.Response.FromJson(json);
            }

            if (pnrSavedResponse.Error != null)
            {
                response.isError = true;
                response.errorCode = "4006";
                if (pnrSavedResponse.Error.ErrorCode == "313")
                {
                    response.errorMessage = pnrSavedResponse.Error.ErrorMessage;
                }
                else
                {
                    response.errorMessage = pnrSavedResponse.Error.ErrorMessage + "(" + pnrSavedResponse.Error.ErrorCode + ")";
                }
                return response;
            }

            response.isError = false;
            if (pnrSavedResponse.PnrReply.PnrHeader.PurplePnrHeader != null)
            {
                response.recordLocator = pnrSavedResponse.PnrReply.PnrHeader.PurplePnrHeader.ReservationInfo.Reservation.ControlNumber;
            }
            else
            {
                response.recordLocator = pnrSavedResponse.PnrReply.PnrHeader.PnrHeaderElementArray[0].ReservationInfo.Reservation.ControlNumber;
            }
            //response.bookingKeyReference = Guid.NewGuid().ToString();

            request.PNR = response.recordLocator;
            FlightBookingServices bookingSvc = new FlightBookingServices(_unitOfWork);
            var bookingRef = bookingSvc.SaveBooking(request);
            response.bookingKeyReference = bookingRef.ToString();

            FlightReportServices reportSvc = new FlightReportServices(_unitOfWork);
            var bookingTest = reportSvc.GetByID(bookingRef);
            response.RobinhoodID = bookingTest.RobinhoodID;
            return response;
        }


        private string GetAllFareRuleString(List<FareRuleDatail> rules)
        {
            string farerule = "";
            foreach (var rule in rules)
            {
                farerule += "<br />";
                string topic = rule.category;
                switch (rule.category)
                {
                    case "SR":
                        topic = "SALES RESTRICTIONS";
                        break;
                    case "AP":
                        topic = "ADVANCE PURCHASE / RESERVATIONS AND TICKETING";
                        break;
                    case "PE":
                        topic = "PENALTIES";
                        break;
                    case "MN":
                        topic = "MIN STAY";
                        break;
                    case "MX":
                        topic = "MAX STAY";
                        break;
                }

                farerule += "<b>" + topic + "</b> <br />";
                farerule += String.Join("<br />", rule.fareRuleText.ToArray());
                farerule += "<br />";
            }
            return farerule;
        }


        public void UpdatePaymentStatus(Entities.RobinhoodPNR.PNR pnr, int paymentType)
        {
            /*
                1 = Credit Card
                2 = PayPal
                3 = Transfer
                4 = Counter Service
                5 = Credit Term
             */
            string requestJson = @"{
            ""BookingKeyReference"":""" + pnr.bookingKeyReference + @""" ,
            ""PaymentStatus"": 1," + @"
            ""PaymentTypeId"": " + paymentType.ToString() + @",
            ""Penalties1"":"""",""Penalties2"":""""}";

            try
            {
                string url = ConfigurationManager.AppSettings["UPDATEPAYMENTSTATUS.URL"];
                Log.Info("UPDATEPAYMENT");
                Log.Debug(requestJson);

                string json = HttpUtility.postJSON(url, requestJson);
                Log.Debug(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public bool UpdateOutsidePaymentStatus(Entities.UpdatePayment.Request request, ref List<string> PNR, ref string errMsg)
        {
            /* Payment Status
                0 = Waiting for payment
                1 = Paid
                2 = Fail
                3 = Refunding
                4 = Refunded
             */
            /* Booking Status
               0 = New
               1 = Confirmed
               2 = Fail
               3 = Cancelled
               4 = Ticketed
               5 = Refunded Ticket
               6 = Reissue Ticket
            */
            bool bResult = true;

            //save to Database
            List<FlightBooking> flightBookingList = _unitOfWork.FlightBookingRepository.GetMany(x => x.RobinhoodID == request.bookingKeyReference).ToList();

            if (flightBookingList != null && flightBookingList.Count > 0)
            {
                //add remark on PNR
                bool isTKOK = false;
                if (request.paymentStatus == 1)
                {
                    isTKOK = true;
                }
                List<string> remaks = request.remark;// new List<string>();
                //remaks.Add(request.remark);
                string PromoCode = "";
                decimal DiscountAmount = 0;
                decimal TotalAmount = 0;
                foreach (var _remark in remaks)
                {
                    if (_remark.ToLower().IndexOf("totalAmount") != -1)
                    {
                        TotalAmount = Convert.ToDecimal(_remark.Split(':')[1]);
                    }
                    if (_remark.ToLower().IndexOf("promoCode") != -1)
                    {
                        PromoCode = _remark.Split(':')[1];
                    }
                    if (_remark.ToLower().IndexOf("discountAmount") != -1)
                    {
                        DiscountAmount = Convert.ToDecimal(_remark.Split(':')[1]);
                    }
                }
                SiteConfigServices siteConfig = new SiteConfigServices(_unitOfWork);
                var dataConfig = siteConfig.GetByKey("OfficeID_Save");
                string newBookingURN = "";
                if (request.bookingURN != null && request.bookingURN.Length > 0)
                {
                    newBookingURN = request.bookingURN;
                }
                foreach (var flightBooking in flightBookingList)
                {
                    Log.Debug("flightBooking.BookingURN=" + flightBooking.BookingURN);
                    Log.Debug("flightBooking.PNR=" + flightBooking.PNR);
                    Log.Debug("flightBooking.RobinhoodID=" + flightBooking.RobinhoodID);
                    bool bAddRemark = RetrieveAndAddRemark(flightBooking.PNR, flightBooking.BookingOID, flightBooking.RobinhoodID, dataConfig.ConfigValue, "ROBINHOOD API", newBookingURN, remaks, isTKOK, ref errMsg);
                    Log.Debug("bAddRemark=" + bAddRemark);
                    Log.Debug("errMsg=" + errMsg);
                    if (bAddRemark)
                    {
                        PNR.Add(flightBooking.PNR);
                    }
                    else
                    {
                        if (errMsg == "this pnr was cancelled.")
                        {
                            flightBooking.StatusBooking = 3;
                        }
                        bResult = false;
                    }
                    if (request.paymentStatus == 1)
                    {
                        flightBooking.StatusBooking = 1;
                    }
                    flightBooking.StatusPayment = request.paymentStatus;
                    flightBooking.paymentReference = request.paymentReference;
                    flightBooking.paymentDate = DateTime.Now;
                    flightBooking.PromoCode = PromoCode;
                    flightBooking.DiscountAmount = DiscountAmount;
                    flightBooking.FinalPrice = TotalAmount - DiscountAmount;
                    if (newBookingURN.Length > 0 && newBookingURN != flightBooking.BookingURN)
                    {
                        flightBooking.BookingURN = newBookingURN;
                    }

                    _unitOfWork.FlightBookingRepository.DetachAll();
                    _unitOfWork.FlightBookingRepository.Update(flightBooking);
                    _unitOfWork.Save();
                }

            }
            else
            {
                bResult = false;
            }



            return bResult;
        }

        public void UpdateBookingStatus(string bookingKeyRef, int bookingStatus, string access_token)
        {
            string requestJson = @"{
            ""BookingKeyReference"":""" + bookingKeyRef + @""" ,
            ""BookingStatus"": " + bookingStatus.ToString() + @" }";

            try
            {
                string url = ConfigurationManager.AppSettings["UPDATEBOOKINGSTATUS.URL"];
                Log.Info("UPDATEBOOKINGSTATUS");
                Log.Debug(requestJson);

                string json = HttpUtility.postJSON(url, requestJson, "Bearer " + access_token);
                Log.Debug(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public BL.Entities.PNR.Response Retrieve(string pnr, string officeID)
        {
            string url = ConfigurationManager.AppSettings["RETRIEVE.URL"];
            RTRequest request = new RTRequest();
            request.session = new Entities.PNR.Session();
            request.session.isStateFull = false;
            request.session.InSeries = false;
            request.session.End = true;
            request.session.SequenceNumber = "1";
            request.useBookingOfficeID = officeID != "";
            request.rtType = "2";
            request.officeID = officeID;
            request.option1 = "A";
            request.option2 = "";
            request.pnrNumber = pnr;
            request.firstName = "";
            request.surName = "";
            request.ClientIP = "";

            string requestJson = JsonConvert.SerializeObject(request);

            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("RETRIEVE");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrResponse = BL.Entities.PNR.Response.FromJson(json);
            }

            return pnrResponse;
        }

        public bool RetrieveAndAddRemark(string pnr, string officeID, string AgencyName, List<string> remarks, bool isTKOK, ref string errMsg)
        {
            string url = ConfigurationManager.AppSettings["RETRIEVE.URL"];
            RTRequest request = new RTRequest();
            request.session = new Entities.PNR.Session();
            request.session.isStateFull = true;
            request.session.InSeries = false;
            request.session.End = false;
            request.session.SequenceNumber = "1";
            request.useBookingOfficeID = officeID != "";
            request.rtType = "2";
            request.bookingOID = officeID;
            request.officeID = officeID;
            request.option1 = "A";
            request.option2 = "";
            request.pnrNumber = pnr;
            request.firstName = "";
            request.surName = "";
            request.ClientIP = "";

            string requestJson = JsonConvert.SerializeObject(request);

            string json = HttpUtility.postJSON(url, requestJson);
            Log.Info("officeID=" + officeID);
            Log.Info("RETRIEVE");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrResponse = BL.Entities.PNR.Response.FromJson(json);
            }
            if (pnrResponse.Error == null)
            {
                Entities.PNR.Request pnrSavedReuest = new Entities.PNR.Request();
                pnrSavedReuest.session = new Entities.PNR.Session();
                pnrSavedReuest.session.isStateFull = true;
                pnrSavedReuest.session.InSeries = false;
                pnrSavedReuest.session.End = true;
                pnrSavedReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
                pnrSavedReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
                pnrSavedReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
                pnrSavedReuest.optionCode = "11";
                pnrSavedReuest.rfLongFreeText = AgencyName;
                pnrSavedReuest.remarks = new List<string>();
                pnrSavedReuest.remarkType = "RM";

                if (remarks != null && remarks.Count > 0)
                {
                    foreach (string rm in remarks)
                    {
                        pnrSavedReuest.remarks.Add(rm);
                    }
                }

                pnrSavedReuest.accessOID = "";
                pnrSavedReuest.useBookingOfficeID = officeID != "";
                pnrSavedReuest.bookingOID = officeID;
                pnrSavedReuest.ClientIP = HttpUtility.GetIPAddress();

                url = ConfigurationManager.AppSettings["SAVEPNR.URL"];
                requestJson = JsonConvert.SerializeObject(pnrSavedReuest);

                json = HttpUtility.postJSON(url, requestJson);

                Log.Info("SAVE_PNR");
                Log.Debug(requestJson);
                Log.Debug(json);
                BL.Entities.PNR.Response pnrSavedResponse = new Entities.PNR.Response();
                if (String.IsNullOrEmpty(json) == false)
                {
                    json = json.Replace("@", "");

                    pnrSavedResponse = BL.Entities.PNR.Response.FromJson(json);
                }

                if (pnrSavedResponse.Error != null)
                {
                    Log.Info(pnrSavedResponse.Error.ErrorMessage + "(" + pnrSavedResponse.Error.ErrorCode + ")");

                    return false;
                }


            }
            return true;
        }
        public bool RetrieveAndAddRemark(string pnr, Guid bookingOID, string transactionID, string officeID, string AgencyName, string bookingURN, List<string> remarks, bool isTKOK, ref string errMsg)
        {
            string url = ConfigurationManager.AppSettings["RETRIEVE.URL"];
            RTRequest request = new RTRequest();
            request.session = new Entities.PNR.Session();
            request.session.isStateFull = true;
            request.session.InSeries = false;
            request.session.End = false;
            request.session.SequenceNumber = "1";
            request.useBookingOfficeID = officeID != "";
            request.rtType = "2";
            request.bookingOID = officeID;
            request.officeID = officeID;
            request.option1 = "A";
            request.option2 = "";
            request.pnrNumber = pnr;
            request.firstName = "";
            request.surName = "";
            request.ClientIP = "";

            string requestJson = JsonConvert.SerializeObject(request);

            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("RETRIEVE");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrResponse = BL.Entities.PNR.Response.FromJson(json);
            }
            if (pnrResponse.Error == null)
            {
                if (pnrResponse.PnrReply.OriginDestinationDetails != null)
                {
                    FlightBooking flightBooking = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.RobinhoodID == transactionID && x.BookingOID != bookingOID);
                    List<FlightBookingFlightDetail> _flightDetailList = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == bookingOID).ToList();
                    List<FlightBookingFlightDetail> _flightDetailListInSameBooking = null;
                    if (flightBooking != null)
                    {
                        _flightDetailListInSameBooking = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == flightBooking.BookingOID).ToList();
                    }

                    FlightBookingFlightDetail oneFlgiht = new FlightBookingFlightDetail();
                    //update airline controlNumber to database
                    if (pnrResponse.PnrReply.OriginDestinationDetails.ItineraryInfo.PurpleItineraryInfo != null)
                    {
                        if (pnrResponse.PnrReply.OriginDestinationDetails.ItineraryInfo.PurpleItineraryInfo.ItineraryReservationInfo != null)
                        {
                            string sControlNumber = pnrResponse.PnrReply.OriginDestinationDetails.ItineraryInfo.PurpleItineraryInfo.ItineraryReservationInfo.Reservation.ControlNumber;
                            Log.Debug("sControlNumber=" + sControlNumber);
                            if (_flightDetailList.Count == 1)
                            {
                                oneFlgiht = _flightDetailList[0];
                                oneFlgiht.ControlNumber = sControlNumber;
                                _unitOfWork.FlightBookingFlightDetailRepository.DetachAll();
                                _unitOfWork.FlightBookingFlightDetailRepository.Update(oneFlgiht);
                                _unitOfWork.Save();
                            }
                            if (_flightDetailListInSameBooking != null && _flightDetailListInSameBooking.Count == 1)
                            {
                                oneFlgiht = _flightDetailListInSameBooking[0];
                                oneFlgiht.ControlNumber = sControlNumber;
                                _unitOfWork.FlightBookingFlightDetailRepository.DetachAll();
                                _unitOfWork.FlightBookingFlightDetailRepository.Update(oneFlgiht);
                                _unitOfWork.Save();
                            }
                        }
                    }
                    else
                    {
                        string sFlightNumber = "";
                        string sControlNumber = "";
                        foreach (var segment in pnrResponse.PnrReply.OriginDestinationDetails.ItineraryInfo.ItineraryInfoElementArray)
                        {
                            sFlightNumber = segment.TravelProduct.ProductDetails.Identification;
                            Log.Debug("sFlightNumber=" + sFlightNumber);
                            sControlNumber = segment.ItineraryReservationInfo.Reservation.ControlNumber;
                            Log.Debug("sControlNumber=" + sControlNumber);
                            oneFlgiht = _flightDetailList.Find(x => x.FlightNumber == sFlightNumber);
                            if (oneFlgiht != null)
                            {
                                oneFlgiht.ControlNumber = sControlNumber;
                                _unitOfWork.FlightBookingFlightDetailRepository.DetachAll();
                                _unitOfWork.FlightBookingFlightDetailRepository.Update(oneFlgiht);
                                _unitOfWork.Save();
                            }
                            if (_flightDetailListInSameBooking != null)
                            {
                                oneFlgiht = _flightDetailListInSameBooking.Find(x => x.FlightNumber == sFlightNumber);
                                if (oneFlgiht != null)
                                {

                                    oneFlgiht.ControlNumber = sControlNumber;
                                    _unitOfWork.FlightBookingFlightDetailRepository.DetachAll();
                                    _unitOfWork.FlightBookingFlightDetailRepository.Update(oneFlgiht);
                                    _unitOfWork.Save();
                                }
                            }
                        }
                    }
                    Entities.PNRCancel.Request cancelRequest = new Entities.PNRCancel.Request();
                    if (isTKOK)
                    {
                        var ticketElement = pnrResponse.PnrReply.DataElementsMaster.DataElementsIndiv.DataElementsIndivElementArray.Find(x => x.ElementManagementData.SegmentName == "TK");
                        string ticketElementNumber = ticketElement.ElementManagementData.Reference.Number;
                        cancelRequest.entryType = "E";
                        cancelRequest.optionCode = "0";
                        cancelRequest.bookingOID = officeID;
                        cancelRequest.useBookingOfficeID = officeID != "";
                        cancelRequest.ClientIP = Utilities.HttpUtility.GetIPAddress();
                        cancelRequest.session = new Entities.PNRCancel.Session();
                        cancelRequest.session.isStateFull = true;
                        cancelRequest.session.InSeries = true;
                        cancelRequest.session.End = false;
                        cancelRequest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
                        cancelRequest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
                        cancelRequest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
                        cancelRequest.CancelElement = new Entities.PNRCancel.CancelElement();
                        cancelRequest.CancelElement.Identifier = "OT";
                        cancelRequest.CancelElement.Number = int.Parse(ticketElementNumber);
                    }
                    else
                    {
                        var tagFA = pnrResponse.PnrReply.DataElementsMaster.DataElementsIndiv.DataElementsIndivElementArray.Find(x => x.ElementManagementData.SegmentName == "FA");
                        if (tagFA == null)
                        {
                            cancelRequest.pnrNumber = pnr;
                            cancelRequest.entryType = "I";
                            cancelRequest.optionCode = "0";
                            cancelRequest.bookingOID = officeID;
                            cancelRequest.useBookingOfficeID = officeID != "";
                            cancelRequest.ClientIP = Utilities.HttpUtility.GetIPAddress();
                            cancelRequest.session = new Entities.PNRCancel.Session();
                            cancelRequest.session.isStateFull = true;
                            cancelRequest.session.InSeries = true;
                            cancelRequest.session.End = false;
                            cancelRequest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
                            cancelRequest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
                            cancelRequest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
                        }
                    }

                    url = ConfigurationManager.AppSettings["PNRCANCEL.URL"];
                    requestJson = JsonConvert.SerializeObject(cancelRequest);

                    json = HttpUtility.postJSON(url, requestJson);

                    Log.Info("PNRCANCEL");
                    Log.Debug(requestJson);
                    Log.Debug(json);
                    BL.Entities.PNR.Response pnrCanceledResponse = new Entities.PNR.Response();
                    if (String.IsNullOrEmpty(json) == false)
                    {
                        json = json.Replace("@", "");

                        pnrCanceledResponse = BL.Entities.PNR.Response.FromJson(json);
                    }
                    if (pnrCanceledResponse.Error == null)
                    {
                        Entities.PNR.Request pnrSavedReuest = new Entities.PNR.Request();
                        pnrSavedReuest.session = new Entities.PNR.Session();
                        pnrSavedReuest.session.isStateFull = true;
                        pnrSavedReuest.session.InSeries = false;
                        pnrSavedReuest.session.End = true;
                        pnrSavedReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
                        pnrSavedReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
                        pnrSavedReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
                        pnrSavedReuest.optionCode = "11";
                        pnrSavedReuest.rfLongFreeText = AgencyName;
                        pnrSavedReuest.remarks = new List<string>();
                        pnrSavedReuest.remarkType = "RM";
                        if (remarks != null && remarks.Count > 0)
                        {
                            foreach (string rm in remarks)
                            {
                                pnrSavedReuest.remarks.Add(rm);
                            }
                        }
                        if (bookingURN != null && bookingURN.Length > 0)
                        {
                            pnrSavedReuest.remarks.Add("BookingURN2 : " + bookingURN);
                        }

                        pnrSavedReuest.accessOID = "";
                        if (isTKOK)
                        {
                            pnrSavedReuest.ticketIndicator = "OK";
                            string QUEUE_OFFICEID = "", QUEUE_NUMBER = "", QUEUE_CAT = "";
                            SiteConfigServices siteConfig = new SiteConfigServices(_unitOfWork);
                            var dataConfig = siteConfig.GetByKey("TICKET.QUEUE_OFFICEID");
                            QUEUE_OFFICEID = dataConfig.ConfigValue;// ConfigurationManager.AppSettings["TICKET.QUEUE_OFFICEID"];
                            dataConfig = siteConfig.GetByKey("TICKET.QUEUE_NUMBER");
                            QUEUE_NUMBER = dataConfig.ConfigValue;//ConfigurationManager.AppSettings["TICKET.QUEUE_NUMBER"];
                            dataConfig = siteConfig.GetByKey("TICKET.QUEUE_CAT");
                            QUEUE_CAT = dataConfig.ConfigValue;//ConfigurationManager.AppSettings["TICKET.QUEUE_CAT"];
                            if (QUEUE_OFFICEID.Length > 0)
                            {
                                Log.Info("QUEUE_OFFICEID=" + QUEUE_OFFICEID);
                                Log.Info("QUEUE_NUMBER=" + QUEUE_NUMBER);
                                Log.Info("QUEUE_CAT=" + QUEUE_CAT);
                                pnrSavedReuest.queues = new List<PNRQueue>();
                                PNRQueue queue = new PNRQueue();
                                queue.officeID = QUEUE_OFFICEID;
                                queue.queueNumber = QUEUE_NUMBER;
                                queue.categoryNumber = QUEUE_CAT;
                                pnrSavedReuest.queues.Add(queue);
                            }
                        }

                        pnrSavedReuest.ClientIP = Utilities.HttpUtility.GetIPAddress();

                        url = ConfigurationManager.AppSettings["SAVEPNR.URL"];
                        requestJson = JsonConvert.SerializeObject(pnrSavedReuest);

                        json = HttpUtility.postJSON(url, requestJson);

                        Log.Info("SAVE_PNR");
                        Log.Debug(requestJson);
                        Log.Debug(json);
                        BL.Entities.PNR.Response pnrSavedResponse = new Entities.PNR.Response();
                        if (String.IsNullOrEmpty(json) == false)
                        {
                            json = json.Replace("@", "");

                            pnrSavedResponse = BL.Entities.PNR.Response.FromJson(json);
                        }

                        if (pnrSavedResponse.Error != null)
                        {
                            Log.Info(pnrSavedResponse.Error.ErrorMessage + "(" + pnrSavedResponse.Error.ErrorCode + ")");
                            errMsg = pnrSavedResponse.Error.ErrorMessage;
                            return false;
                        }

                    }
                    else
                    {
                        errMsg = pnrCanceledResponse.Error.ErrorMessage;
                    }
                }
                else
                {
                    errMsg = "this pnr was cancelled.";
                }
            }
            else
            {
                errMsg = pnrResponse.Error.ErrorMessage;
            }
            return errMsg.Length > 0 ? false : true;
        }

        public void PNRCancel(Entities.PNRCancel.Request request)
        {
            string url = ConfigurationManager.AppSettings["PNRCANCEL.URL"];
            string requestJson = JsonConvert.SerializeObject(request);

            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("PNRCancel");
            Log.Debug(requestJson);
            Log.Debug(json);
        }

        public bool RetrieveAndCancel(Entities.PNRCancel.Request request)
        {
            bool bCancel = false;
            string url = ConfigurationManager.AppSettings["RETRIEVE.URL"];
            RTRequest rtRequest = new RTRequest();
            rtRequest.session = new Entities.PNR.Session();
            rtRequest.session.isStateFull = true;
            rtRequest.session.InSeries = false;
            rtRequest.session.End = false;
            rtRequest.session.SequenceNumber = "1";
            rtRequest.useBookingOfficeID = request.useBookingOfficeID;
            rtRequest.rtType = "2";
            rtRequest.bookingOID = request.bookingOID;
            rtRequest.officeID = request.bookingOID;
            rtRequest.option1 = "A";
            rtRequest.option2 = "";
            rtRequest.pnrNumber = request.pnrNumber;
            rtRequest.firstName = "";
            rtRequest.surName = "";
            rtRequest.ClientIP = "";

            string requestJson = JsonConvert.SerializeObject(rtRequest);

            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("RETRIEVE");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrResponse = BL.Entities.PNR.Response.FromJson(json);
            }
            if (pnrResponse.Error == null)
            {
                if (pnrResponse.PnrReply.OriginDestinationDetails == null)//pnr cancelled
                {
                    bCancel = true;
                }
                else//check e-Ticket from tag FA
                {
                    //if have ticket number cannot cancel
                    //else can cancel pnr
                    var tagFA = pnrResponse.PnrReply.DataElementsMaster.DataElementsIndiv.DataElementsIndivElementArray.Find(x => x.ElementManagementData.SegmentName == "FA");
                    if (tagFA == null)
                    {
                        url = ConfigurationManager.AppSettings["PNRCANCEL.URL"];
                        request.session = new Entities.PNRCancel.Session();
                        request.session.isStateFull = true;
                        request.session.InSeries = false;
                        request.session.End = false;
                        request.session.SequenceNumber = "1";
                        request.entryType = "I";
                        request.optionCode = "0";
                        requestJson = JsonConvert.SerializeObject(request);

                        json = HttpUtility.postJSON(url, requestJson);

                        Log.Info("PNRCancel");
                        Log.Debug(requestJson);
                        Log.Debug(json);
                        pnrResponse = new Entities.PNR.Response();
                        if (String.IsNullOrEmpty(json) == false)
                        {
                            json = json.Replace("@", "");

                            pnrResponse = BL.Entities.PNR.Response.FromJson(json);
                        }
                        if (pnrResponse.Error == null)
                        {
                            Entities.PNR.Request pnrSavedReuest = new Entities.PNR.Request();
                            pnrSavedReuest.session = new Entities.PNR.Session();
                            pnrSavedReuest.session.isStateFull = true;
                            pnrSavedReuest.session.InSeries = false;
                            pnrSavedReuest.session.End = true;
                            pnrSavedReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
                            pnrSavedReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
                            pnrSavedReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
                            pnrSavedReuest.optionCode = "11";
                            pnrSavedReuest.rfLongFreeText = "BOOKING ENGINE";
                            pnrSavedReuest.remarks = new List<string>();
                            pnrSavedReuest.remarkType = "RM";
                            pnrSavedReuest.remarks.Add("CANCEL BY ROBINHOOD API");
                            pnrSavedReuest.accessOID = "";
                            pnrSavedReuest.ClientIP = Utilities.HttpUtility.GetIPAddress();

                            url = ConfigurationManager.AppSettings["SAVEPNR.URL"];
                            requestJson = JsonConvert.SerializeObject(pnrSavedReuest);

                            json = HttpUtility.postJSON(url, requestJson);

                            Log.Info("SAVE_PNR");
                            Log.Debug(requestJson);
                            Log.Debug(json);
                            BL.Entities.PNR.Response pnrSavedResponse = new Entities.PNR.Response();
                            if (String.IsNullOrEmpty(json) == false)
                            {
                                json = json.Replace("@", "");

                                pnrSavedResponse = BL.Entities.PNR.Response.FromJson(json);
                            }
                            if (pnrSavedResponse.Error == null && pnrSavedResponse.PnrReply.OriginDestinationDetails == null)//pnr cancelled
                            {
                                bCancel = true;
                            }
                        }
                    }//if (tagFA == null)

                }
            }

            return bCancel;
        }

        public void MemberAutoCancel(string PNR, string bookingID, string userToken)
        {
            BL.Entities.PNRCancel.Request request = new BL.Entities.PNRCancel.Request();
            request.session = new BL.Entities.PNRCancel.Session();
            request.session.isStateFull = true;
            request.session.InSeries = false;
            request.session.End = false;
            request.session.SessionId = "";
            request.session.SequenceNumber = "";
            request.session.SecurityToken = "";
            request.useBookingOfficeID = false;
            request.bookingOID = "";
            request.entryType = "I";
            request.optionCode = "0";
            request.pnrNumber = PNR;
            request.ClientIP = "";
            this.PNRCancel(request);


            this.UpdateBookingStatus(bookingID, 0, userToken);

        }

        public FlightBooking GetByID(Guid FlightBookingOID)
        {
            return _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == FlightBookingOID/* && x.IsDelete == false*/);
        }

        public void SaveOrUpdate(FlightBooking FlightBooking)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == FlightBooking.BookingOID);
                if (check == null)
                {
                    _unitOfWork.FlightBookingRepository.Insert(FlightBooking);
                }
                else
                {
                    _unitOfWork.FlightBookingRepository.Update(FlightBooking);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        private decimal ceilingTo5Or10(decimal d)
        {
            decimal result = d;
            decimal c = Math.Ceiling(d);
            decimal fragment = c % 10;
            if (fragment >= 1 && fragment <= 4)
            {
                result = c + (5 - fragment);
            }
            else if (fragment >= 6 && fragment <= 9)
            {
                result = c + (10 - fragment);
            }
            else
            {
                result = c;
            }
            return result;
        }

        public FlightSearchMultiTicket GetFlightSearchMulti(Guid pgSearchOID)
        {
            FlightSearchMultiTicket flightSearchMulti = null;
            var _query = _unitOfWork.FlightSearchMultiTicketRepository.GetFirstOrDefault(x => x.FlightSearchOID == pgSearchOID);
            if (_query != null)
            {
                flightSearchMulti = new FlightSearchMultiTicket();
                flightSearchMulti.FlightSearchOID = _query.FlightSearchOID;
                flightSearchMulti.FlightSearchRequest = BL.Utilities.StringCompression.unzip(_query.FlightSearchRequest);
                flightSearchMulti.FlightSearchResponse = BL.Utilities.StringCompression.unzip(_query.FlightSearchResponse);
                flightSearchMulti.DateTimeRequest = _query.DateTimeRequest;
            }
            return flightSearchMulti;
        }

        private void SaveToFlightSearchMultiTicket(FlightSearchMultiTicket searchMultiTicket)
        {
            string sql = @"INSERT INTO FlightSearchMultiTicket (FlightSearchOID,FlightSearchRequest,FlightSearchResponse,DateTimeRequest)
                        VALUES (@FlightSearchOID,@FlightSearchRequest,@FlightSearchResponse,@DateTimeRequest)";
            System.Data.SqlClient.SqlParameter[] parameters = new System.Data.SqlClient.SqlParameter[4];
            parameters[0] = new System.Data.SqlClient.SqlParameter("@FlightSearchOID", searchMultiTicket.FlightSearchOID);
            parameters[1] = new System.Data.SqlClient.SqlParameter("@FlightSearchRequest", BL.Utilities.StringCompression.zip(searchMultiTicket.FlightSearchRequest));
            parameters[2] = new System.Data.SqlClient.SqlParameter("@FlightSearchResponse", BL.Utilities.StringCompression.zip(searchMultiTicket.FlightSearchResponse));
            parameters[3] = new System.Data.SqlClient.SqlParameter("@DateTimeRequest", searchMultiTicket.DateTimeRequest);
            _unitOfWork.FlightSearchMultiTicketRepository.ExecuteSqlCommand(sql, parameters);
            //using (var scope = new TransactionScope())
            //{
            //    var check = _unitOfWork.FlightSearchMultiTicketRepository.GetFirstOrDefault(x => x.FlightSearchOID == searchMultiTicket.FlightSearchOID);
            //    if (check == null)
            //    {
            //        _unitOfWork.FlightSearchMultiTicketRepository.Insert(searchMultiTicket);
            //    }
            //    else
            //    {
            //        _unitOfWork.FlightSearchMultiTicketRepository.Update(searchMultiTicket);
            //    }
            //    _unitOfWork.Save();

            //    scope.Complete();
            //}
        }

        public bool RemoveFlightSearchMultiTicket()
        {
            bool bRemove = false;
            var allRemove = _unitOfWork.FlightSearchMultiTicketRepository.GetMany(x => x.DateTimeRequest <= DateTime.Now.AddHours(-48));
            if (allRemove != null)
            {
                using (var scope = new TransactionScope())
                {
                    foreach (var search in allRemove)
                    {
                        _unitOfWork.FlightSearchMultiTicketRepository.Delete(search);
                    }
                    _unitOfWork.Save();

                    scope.Complete();
                    bRemove = true;
                }
            }
            Log.Debug("bRemove=" + bRemove);
            return bRemove;
        }

        public string AirSeatMap(Entities.SeatMap.Request request)
        {
            string url = ConfigurationManager.AppSettings["SEATMAP.URL"];
            string requestJson = JsonConvert.SerializeObject(request);

            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("AirSeatMap");
            Log.Debug(requestJson);
            Log.Debug(json);
            return json;
        }

        private Entities.DiscountTag.DiscountTagEntities GetDiscountTag(List<Entities.DiscountTag.DiscountTagEntities> discountTagList, string _airlineCode, string _fareBasis, string _rbd)
        {
            Entities.DiscountTag.DiscountTagEntities discountTag = null;
            if (discountTagList != null && discountTagList.Count > 0)
            {
                discountTag = discountTagList.Find(x => x.discountTag.AirlineCodes.IndexOf(_airlineCode) != -1 && x.discountTag.RBD.IndexOf(_rbd) != -1 && x.discountTag.FareBasis.IndexOf(_fareBasis) != -1);
                if (discountTag == null)
                {
                    discountTag = discountTagList.Find(x => x.discountTag.AirlineCodes.IndexOf(_airlineCode) != -1 && x.discountTag.RBD.IndexOf(_rbd) != -1 && x.discountTag.FareBasis == "*");
                }
                if (discountTag == null)
                {
                    discountTag = discountTagList.Find(x => x.discountTag.AirlineCodes.IndexOf(_airlineCode) != -1 && x.discountTag.RBD == "*" && x.discountTag.FareBasis.IndexOf(_fareBasis) != -1);
                }
                if (discountTag == null)
                {
                    discountTag = discountTagList.Find(x => x.discountTag.AirlineCodes.IndexOf(_airlineCode) != -1 && x.discountTag.RBD == "*" && x.discountTag.FareBasis == "*");
                }
                if (discountTag == null)
                {
                    discountTag = discountTagList.Find(x => x.discountTag.AirlineCodes == "YY" && x.discountTag.RBD == "*" && x.discountTag.FareBasis == "*");
                }
            }
            return discountTag;
        }

        public Entities.RobinhoodPNR.PNR CheckDuplicateBooking(Entities.RobinhoodFare.AirFare request)
        {
            Entities.RobinhoodPNR.PNR response = new Entities.RobinhoodPNR.PNR();

            SiteConfigServices siteConfig = new SiteConfigServices(_unitOfWork);
            var dataConfig = siteConfig.GetByKey("OfficeID_Save");
            request.bookingOID = dataConfig.ConfigValue;// ConfigurationManager.AppSettings["OfficeID"];
            string url = ConfigurationManager.AppSettings["RETRIEVE.URL"];
            RTRequest retrieve_request = new RTRequest();
            retrieve_request.session = new Entities.PNR.Session();
            retrieve_request.session.isStateFull = false;
            retrieve_request.session.InSeries = false;
            retrieve_request.session.End = true;
            retrieve_request.session.SequenceNumber = "1";
            retrieve_request.useBookingOfficeID = request.bookingOID != "";
            retrieve_request.rtType = "3";
            retrieve_request.officeID = request.bookingOID;
            retrieve_request.option1 = "A";
            retrieve_request.option2 = "";
            retrieve_request.pnrNumber = "";
            retrieve_request.firstName = request.adtPaxs[0].firstname;
            retrieve_request.surName = request.adtPaxs[0].lastname;
            retrieve_request.ClientIP = "";

            string requestJson = JsonConvert.SerializeObject(request);

            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("RETRIEVE");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrResponse = BL.Entities.PNR.Response.FromJson(json);
            }
            if (pnrResponse.Error != null)
            {
                if (pnrResponse.Error.ErrorMessage.IndexOf("NO NAME") != -1)
                {
                    return Booking(request);
                }
                response.isError = true;
                response.errorCode = "8001";
                response.errorMessage = pnrResponse.Error.ErrorMessage + "(" + pnrResponse.Error.ErrorCode + ")";
                return response;
            }
            else
            {
                bool bHavePNR = false;
                //have pnr but canceled
                if (!bHavePNR)
                {
                    return Booking(request);
                }
                else//apply pnr to booking and save to DB
                {

                }
            }

            return response;
        }
        public Entities.RobinhoodPNR.MultiTicketPNR CheckDuplicateBookingMultiTicket(Entities.RobinhoodFare.AirFare request)
        {
            Entities.RobinhoodPNR.MultiTicketPNR response = new Entities.RobinhoodPNR.MultiTicketPNR();

            SiteConfigServices siteConfig = new SiteConfigServices(_unitOfWork);
            var dataConfig = siteConfig.GetByKey("OfficeID_Save");
            request.bookingOID = dataConfig.ConfigValue;// ConfigurationManager.AppSettings["OfficeID"];
            string url = ConfigurationManager.AppSettings["RETRIEVE.URL"];
            RTRequest retrieve_request = new RTRequest();
            retrieve_request.session = new Entities.PNR.Session();
            retrieve_request.session.isStateFull = false;
            retrieve_request.session.InSeries = false;
            retrieve_request.session.End = true;
            retrieve_request.session.SequenceNumber = "1";
            retrieve_request.useBookingOfficeID = request.bookingOID != "";
            retrieve_request.rtType = "3";
            retrieve_request.officeID = request.bookingOID;
            retrieve_request.option1 = "A";
            retrieve_request.option2 = "";
            retrieve_request.pnrNumber = "";
            retrieve_request.firstName = request.adtPaxs[0].firstname;
            retrieve_request.surName = request.adtPaxs[0].lastname;
            retrieve_request.ClientIP = "";

            string requestJson = JsonConvert.SerializeObject(request);

            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("RETRIEVE");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.PNR.Response pnrResponse = new Entities.PNR.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                pnrResponse = BL.Entities.PNR.Response.FromJson(json);
            }
            if (pnrResponse.Error != null)
            {
                if (pnrResponse.Error.ErrorMessage.IndexOf("NO NAME") != -1)
                {
                    return BookingMultiTicket(request);
                }
                response.isError = true;
                response.errorCode = "8001";
                response.errorMessage = pnrResponse.Error.ErrorMessage + "(" + pnrResponse.Error.ErrorCode + ")";
                return response;
            }

            return response;
        }

    }
}
