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
using BL.Entities.GogojiiFlight;
using BL.Entities.MasterPricer;
using BL.Entities.AirSell;
using log4net;
using BL.Entities.InformativePricing;
using BL.Entities.GogojiiFare;

namespace BL
{
    public class FlightSearchServices : IFlightSearchServices
    {
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UnitOfWork _unitOfWork;
        public FlightSearchServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            FareMarkup markup = new FareMarkup();
            NamingServices namingService = new NamingServices(_unitOfWork);

            string url = ConfigurationManager.AppSettings["MASTERPRICER.URL"];
            string requestJson = JsonConvert.SerializeObject(request);

            List<MasterFlight> masterFlightList = new List<MasterFlight>();
            Entities.MasterPricer.Response response = new Entities.MasterPricer.Response();
            Log.Info("Search Start");
            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("Search End");

            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                response = BL.Entities.MasterPricer.Response.FromJson(json);
                if (response == null || response.FareMasterPricerTravelBoardSearchReply == null ||
                    response.FareMasterPricerTravelBoardSearchReply.Recommendation.Count <= 0)
                {
                    Log.Debug(requestJson);
                    Log.Debug(json);
                    return null;
                }
                //Log.Debug(requestJson);
                //Log.Debug(json);

                foreach (var recommendation in response.FareMasterPricerTravelBoardSearchReply.Recommendation)
                {
                    MasterFlight masterFlight = new MasterFlight();
                    masterFlight.id = Guid.NewGuid();
                    var depFlight = response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex == null ?
                        response.FareMasterPricerTravelBoardSearchReply.FlightIndex.FlightIndexElementArray[0].GroupOfFlights :
                     response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex.GroupOfFlights;

                    masterFlight.departureFlight = GetFlight(depFlight, recommendation.PaxFareProduct, "ADT", recommendation.SegmentFlightRef, request.languageCode, request.flights[0].departureDateTime, 1);

                    if (request.flights.Count > 1)
                    {
                        var retFlight = response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex == null ?
                            response.FareMasterPricerTravelBoardSearchReply.FlightIndex.FlightIndexElementArray[1].GroupOfFlights :
                     response.FareMasterPricerTravelBoardSearchReply.FlightIndex.PurpleFlightIndex.GroupOfFlights;

                        masterFlight.returnFlight = GetFlight(retFlight, recommendation.PaxFareProduct, "ADT", recommendation.SegmentFlightRef, request.languageCode, request.flights[1].departureDateTime, 2);

                    }

                    masterFlight.ComputeConnectionTime();

                    masterFlight.fare = new Pricing(request.noOfAdults, request.noOfChildren, request.noOfInfants);

                    masterFlight.fare.currencyCode = response.FareMasterPricerTravelBoardSearchReply.ConversionRate.ConversionRateDetail.Currency;

                    masterFlight.fare.adtFare = new PaxPricing();
                    masterFlight.fare.adtFare = getPaxFare(recommendation.PaxFareProduct, "ADT", markup, masterFlight.departureFlight, namingService);

                    if (request.noOfChildren > 0)
                    {
                        masterFlight.fare.chdFare = new PaxPricing();
                        masterFlight.fare.chdFare = getPaxFare(recommendation.PaxFareProduct, "CHD,CH", markup, masterFlight.departureFlight, namingService);
                    }

                    if (request.noOfInfants > 0)
                    {
                        masterFlight.fare.infFare = new PaxPricing();
                        masterFlight.fare.infFare = getPaxFare(recommendation.PaxFareProduct, "INF,IN", markup, masterFlight.departureFlight, namingService);
                    }
                    masterFlight.fareRule = getFareRule(recommendation.PaxFareProduct);

                    masterFlightList.Add(masterFlight);
                }

            }

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

            result.pgSearchOID = request.pgSearchOID;

            Log.Info("Convert to our object End");
            return result;
        }

        private List<string> getFareRule(PaxFareProductUnion paxFareProduct)
        {
            List<string> fareRule = new List<string>();
            if (paxFareProduct.PurplePaxFareProduct != null)
            {
                if(paxFareProduct.PurplePaxFareProduct.Fare.PurpleFare != null)
                {
                    fareRule.Add(String.Join(" ", paxFareProduct.PurplePaxFareProduct.Fare.PurpleFare.PricingMessage.Description.ToArray()));
                }
                else
                {
                    foreach(var fare in paxFareProduct.PurplePaxFareProduct.Fare.FareElementArray)
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
                foreach(var paxProduct in paxFareProduct.PaxFareProductElementArray)
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

        private List<Entities.GogojiiFlight.Flight> GetFlight(List<GroupOfFlight> groupFlight,
            PaxFareProductUnion paxFareProduct, string paxType, SegmentFlightRefUnion segmentRef,
            string languageCode, DateTime depDate, int segmentNo)
        {
            NamingServices namingService = new NamingServices(_unitOfWork);
            string[] pax = paxType.Split(',');
            List<string> rbd = new List<string>();
            List<string> fareBasis = new List<string>();
            List<string> avail = new List<string>();
            List<string> fareType = new List<string>();
            List<string> cabin = new List<string>();

            List<Entities.GogojiiFlight.Flight> flightList = new List<Entities.GogojiiFlight.Flight>();

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
                    avail.Add(product.CabinProduct.AvlStatus);
                    cabin.Add(product.CabinProduct.Cabin);
                    rbd.Add(product.CabinProduct.Rbd);
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
                        avail.Add(product.CabinProduct.AvlStatus);
                        cabin.Add(product.CabinProduct.Cabin);
                        rbd.Add(product.CabinProduct.Rbd);
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
                    avail.Add(product.CabinProduct.AvlStatus);
                    cabin.Add(product.CabinProduct.Cabin);
                    rbd.Add(product.CabinProduct.Rbd);
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
                        avail.Add(product.CabinProduct.AvlStatus);
                        cabin.Add(product.CabinProduct.Cabin);
                        rbd.Add(product.CabinProduct.Rbd);
                        fareBasis.Add(product.FareProductDetail.FareBasis);
                        fareType.Add(product.FareProductDetail.FareType.String == null ? String.Join(",", product.FareProductDetail.FareType.StringArray) : product.FareProductDetail.FareType.String);
                    }
                }
            }

            List<string> refNo = new List<string>();
            if (segmentRef.SegmentFlightRefElement != null)
            {
                if (segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElement != null)
                {
                    refNo.Add(segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElement.RefNumber);
                }
                else
                {
                    refNo.Add(segmentRef.SegmentFlightRefElement.ReferencingDetail.ReferencingDetailElementArray[segmentNo - 1].RefNumber);
                }
            }
            else
            {
                foreach (var refList in segmentRef.SegmentFlightRefElementArray)
                {
                    string sRefNo = "";
                    if (refList.ReferencingDetail.ReferencingDetailElement != null)
                    {
                        sRefNo = refList.ReferencingDetail.ReferencingDetailElement.RefNumber;
                    }
                    else
                    {
                        sRefNo = refList.ReferencingDetail.ReferencingDetailElementArray[segmentNo - 1].RefNumber;
                    }

                    if (!refNo.Contains(sRefNo))
                    {
                        refNo.Add(sRefNo);
                    }
                }
            }

            //Find flight
            foreach (string rN in refNo)
            {
                var flightDetail = groupFlight.FirstOrDefault(x => x.PropFlightGrDetail.FlightProposal[0].Ref == rN);
                Entities.GogojiiFlight.Flight flight = new Entities.GogojiiFlight.Flight();
                flight.totalTime = flightDetail.PropFlightGrDetail.FlightProposal.FirstOrDefault(x => x.UnitQualifier == "EFT").Ref;
                flight.flightDetails = new List<Entities.GogojiiFlight.FlightDetail>();
                if (flightDetail.FlightDetails.FlightDetailsClass != null)
                {
                    //TODO TECHNICAL STOP
                    var flightInfo = flightDetail.FlightDetails.FlightDetailsClass.FlightInformation;
                    var technicalStop = flightDetail.FlightDetails.FlightDetailsClass.TechnicalStop;
                    Entities.GogojiiFlight.FlightDetail detail = new Entities.GogojiiFlight.FlightDetail();
                    detail.airline = new Airline(namingService, languageCode);
                    detail.airline.code = flightInfo.CompanyId.MarketingCarrier;
                    if (flightInfo.CompanyId.OperatingCarrier != null)
                    {
                        detail.operatedAirline = new Airline(namingService, languageCode);
                        detail.operatedAirline.code = flightInfo.CompanyId.OperatingCarrier;
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
                    detail.avail = int.Parse(avail[0]);
                    detail.fareType = fareType[0];
                    detail.cabin = cabin[0];

                    detail.setDisplayDateTime(languageCode, depDate);

                    flight.flightDetails.Add(detail);


                    if (technicalStop != null && technicalStop.StopDetails != null && technicalStop.StopDetails.Count > 1)
                    {
                        detail = new Entities.GogojiiFlight.FlightDetail();
                        detail.airline = new Airline(namingService, languageCode);
                        detail.airline.code = flightInfo.CompanyId.MarketingCarrier;
                        if (flightInfo.CompanyId.OperatingCarrier != null)
                        {
                            detail.operatedAirline = new Airline(namingService, languageCode);
                            detail.operatedAirline.code = flightInfo.CompanyId.OperatingCarrier;
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
                        detail.avail = int.Parse(avail[0]);
                        detail.fareType = fareType[0];
                        detail.cabin = cabin[0];

                        detail.setDisplayDateTime(languageCode, depDate);
                        flight.flightDetails.Add(detail);
                    }
                }
                else
                {
                    for (int i = 0; i < flightDetail.FlightDetails.FlightDetailArray.Count; i++)
                    {
                        var flightInfo = flightDetail.FlightDetails.FlightDetailArray[i].FlightInformation;
                        Entities.GogojiiFlight.FlightDetail detail = new Entities.GogojiiFlight.FlightDetail();
                        detail.airline = new Airline(namingService, languageCode);
                        detail.airline.code = flightInfo.CompanyId.MarketingCarrier;
                        if (flightInfo.CompanyId.OperatingCarrier != null)
                        {
                            detail.operatedAirline = new Airline(namingService, languageCode);
                            detail.operatedAirline.code = flightInfo.CompanyId.OperatingCarrier;
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
                        detail.avail = int.Parse(avail[i]);
                        detail.fareType = fareType[i];
                        detail.cabin = cabin[i];

                        detail.setDisplayDateTime(languageCode, depDate);

                        flight.flightDetails.Add(detail);
                    }
                }
                flightList.Add(flight);
            }

            return flightList;
        }

        private static Dictionary<string, string> cityDic = new Dictionary<string, string>();
        private PaxPricing getPaxFare(PaxFareProductUnion paxFareProduct, string paxType, FareMarkup markup, List<BL.Entities.GogojiiFlight.Flight> depFlights, NamingServices namingService)
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
                depFlights[0].flightDetails[0].rbd, depFlights[0].flightDetails[0].fareBasis, paxType, ref lessFare);
            paxPricing.lessFare = lessFare;
            return paxPricing;
        }


        public Entities.GogojiiFare.AirFare InformativePricing(Entities.InformativePricing.Request request, Entities.InformativePricing.Request requestFor1A, string languageCode)
        {
            string url = ConfigurationManager.AppSettings["INFORMATIVE.URL"];
            string requestJson = JsonConvert.SerializeObject(requestFor1A);

            string json = HttpUtility.postJSON(url, requestJson);

            Log.Info("INFORMATIVE");
            Log.Debug(requestJson);
            Log.Debug(json);
            BL.Entities.InformativePricing.Response pricingResponse = new Entities.InformativePricing.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");
                pricingResponse = BL.Entities.InformativePricing.Response.FromJson(json);
            }

            if (pricingResponse.Error != null)
            {
                return null;
            }

            List<string> fareBasis = new List<string>();
            NamingServices namingServices = new NamingServices(_unitOfWork);
            Entities.GogojiiFare.AirFare airFare = new Entities.GogojiiFare.AirFare(namingServices, languageCode);
            if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup != null)
            {
                var fare = pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PurplePricingGroupLevelGroup;
                airFare.adtFare = airFare.FindFareFromInformative(fare, request, "ADT");

                fareBasis = GetFareBasis(fare.FareInfoGroup.SegmentLevelGroup);
            }
            else if (pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray.Count > 0)
            {
                foreach (var fare in pricingResponse.FareInformativePricingWithoutPnrReply.MainGroup.PricingGroupLevelGroup.PricingGroupLevelGroupElementArray)
                {
                    string paxType = "";
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

                    if (paxType == "ADT")
                    {
                        airFare.adtFare = airFare.FindFareFromInformative(fare, request, "ADT");
                        fareBasis = GetFareBasis(fare.FareInfoGroup.SegmentLevelGroup);
                    }
                    else if (paxType == "CHD" || paxType == "CH")
                    {
                        airFare.chdFare = airFare.FindFareFromInformative(fare, request, "CHD");
                    }
                    else if (paxType == "INF" || paxType == "IN")
                    {
                        airFare.infFare = airFare.FindFareFromInformative(fare, request, "INF");
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

            airFare.fareRules = new List<Entities.GogojiiFare.FareRule>();
            //Fare Rule
            for (int i = 0; i < fareBasis.Count; i++)
            {
                Entities.FareRule.Request fareRuleRequest = new Entities.FareRule.Request();
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
                    Entities.GogojiiFare.FareRule fareRule = new Entities.GogojiiFare.FareRule();
                    fareRule.origin = new City(namingServices, languageCode);
                    fareRule.origin.code = ruleResponse.FareCheckRulesReply.FlightDetails.OdiGrp.OriginDestination.Origin;
                    fareRule.destination = new City(namingServices, languageCode);
                    fareRule.destination.code = ruleResponse.FareCheckRulesReply.FlightDetails.OdiGrp.OriginDestination.Destination;
                    fareRule.fareBasis = ruleResponse.FareCheckRulesReply.FlightDetails.QualificationFareDetails.AdditionalFareDetails.FareClass;
                    fareRule.rules = new List<Entities.GogojiiFare.FareRuleDatail>();
                    if (ruleResponse.FareCheckRulesReply.TariffInfo.PurpleTariffInfo != null)
                    {
                        Entities.GogojiiFare.FareRuleDatail r = new Entities.GogojiiFare.FareRuleDatail();
                        r.fareRuleText = new List<string>();
                        foreach (var text in ruleResponse.FareCheckRulesReply.TariffInfo.PurpleTariffInfo.FareRuleText)
                        {
                            r.fareRuleText.Add(text.FreeText);
                        }
                        fareRule.rules.Add(r);
                    }
                    else
                    {
                        foreach (var info in ruleResponse.FareCheckRulesReply.TariffInfo.TariffInfoElementArray)
                        {
                            Entities.GogojiiFare.FareRuleDatail r = new Entities.GogojiiFare.FareRuleDatail();
                            r.fareRuleText = new List<string>();
                            foreach (var text in info.FareRuleText)
                            {
                                r.fareRuleText.Add(text.FreeText);
                            }
                            fareRule.rules.Add(r);
                        }
                    }
                    airFare.fareRules.Add(fareRule);
                }

            }

            airFare.SetDisplayFlight(request);
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

        public Entities.GogojiiFare.AirFare GhostPricing(Entities.AirSell.Request request, string languageCode)
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
            Entities.GogojiiFare.AirFare airFare = new Entities.GogojiiFare.AirFare(namingServices, languageCode);
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


        public Entities.GogojiiPNR.PNR Booking(Entities.GogojiiFare.AirFare request)
        {
            Entities.GogojiiPNR.PNR response = new Entities.GogojiiPNR.PNR();
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
                string status = airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.PurpleItineraryDetail.SegmentInformation.ActionDetails.StatusCode;
                if (status != "HK" && status != "OK")
                {
                    isNotConfirmStatus = true;
                }
            }
            else
            {
                foreach (var itin in airSellResponse.AirSellFromRecommendationReply.ItineraryDetails.ItineraryDetailElementArray)
                {
                    string status = itin.SegmentInformation.ActionDetails.StatusCode;
                    if (status != "HK" && status != "OK")
                    {
                        isNotConfirmStatus = true;
                    }
                }
            }

            if (isNotConfirmStatus)
            {
                response.isError = true;
                response.errorMessage = "FLIGHTS NOT CONFIRM";
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
            pnrReuest.bookingOID = request.bookingOID;

            pnrReuest.fop = "CA";
            pnrReuest.remarkType = "RM";
            pnrReuest.remarks = new List<string>();
            pnrReuest.remarks.Add("Gogojii WEB BOOKING");
            pnrReuest.remarks.Add("ADTSALE : " + (request.adtFare.net - request.adtFare.tax - request.adtFare.qtax).ToString());
            pnrReuest.remarks.Add("ADTNET : " + request.adtFare.lessFare);
            pnrReuest.remarks.Add("ADTTAX : " + (request.adtFare.tax + request.adtFare.qtax).ToString());
            if (request.noOfChildren > 0)
            {
                pnrReuest.remarks.Add("CHDSALE : " + (request.chdFare.net - request.chdFare.tax - request.chdFare.qtax).ToString());
                pnrReuest.remarks.Add("CHDNET : " + request.chdFare.lessFare);
                pnrReuest.remarks.Add("CHDTAX : " + (request.chdFare.tax + request.chdFare.qtax).ToString());
            }
            else
            {
                pnrReuest.remarks.Add("CHDSALE : 0.00");
                pnrReuest.remarks.Add("CHDNET :  0.00");
                pnrReuest.remarks.Add("CHDTAX : 0.00");
            }
            if (request.noOfInfants > 0)
            {
                pnrReuest.remarks.Add("INFSALE : " + (request.infFare.net - request.infFare.tax - request.infFare.qtax).ToString());
                pnrReuest.remarks.Add("INFNET : " + request.infFare.lessFare);
                pnrReuest.remarks.Add("INFTAX : " + (request.infFare.tax + request.infFare.qtax).ToString());
            }
            else
            {
                pnrReuest.remarks.Add("INFSALE : 0.00");
                pnrReuest.remarks.Add("INFNET :  0.00");
                pnrReuest.remarks.Add("INFTAX : 0.00");
            }
            string depFB = "";
            foreach (var depF in request.depFlight)
            {
                depFB += (depFB == "" ? "" : "/") + depF.fareBasis;
            }
            pnrReuest.remarks.Add("DEPARTFAREBASIS : " + depFB);
            if (request.retFlight != null && request.retFlight.Count > 0)
            {
                string retFB = "";
                foreach (var retF in request.retFlight)
                {
                    retFB += (retFB == "" ? "" : "/") + retF.fareBasis;
                }
                pnrReuest.remarks.Add("RETURNFAREBASIS : " + retFB);
            }
            pnrReuest.ticketIndicator = "TL";
            pnrReuest.ticketTimeLimitDate = request.TKTL;
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
                    pax.infantPassenger.paxNo = request.adtPaxs.Count + request.chdPaxs.Count + i + 1;
                    pax.infantPassenger.infantFlag = false;
                    pax.infantPassenger.haveDOB = true;
                    pax.infantPassenger.DOB = request.infPaxs[i].birthday;
                    pax.infantPassenger.age = 1;
                    pax.infantPassenger.withADT = 0;
                    pax.infantPassenger.titleName = "";
                    pax.infantPassenger.firstName = request.infPaxs[i].firstname;
                    pax.infantPassenger.lastName = request.infPaxs[i].lastname;
                    pax.infantPassenger.paxType = "INF";
                    pax.infantPassenger.paxID = (request.adtPaxs.Count + request.chdPaxs.Count + i + 1).ToString();
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
            pnrReuest.refEmail = request.adtPaxs[0].email;
            pnrReuest.refMobileNumber = request.adtPaxs[0].telNo;
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

            //Pricing
            Entities.Pricing.Request pricingReuest = new Entities.Pricing.Request();
            pricingReuest.session = new Entities.Pricing.Session();
            pricingReuest.session.isStateFull = true;
            pricingReuest.session.InSeries = true;
            pricingReuest.session.End = false;
            pricingReuest.session.SessionId = pnrResponse.PnrReply.Session.SessionId;
            pricingReuest.session.SequenceNumber = (int.Parse(pnrResponse.PnrReply.Session.SequenceNumber) + 1).ToString();
            pricingReuest.session.SecurityToken = pnrResponse.PnrReply.Session.SecurityToken;
            pricingReuest.bookingOID = request.bookingOID;

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

            Log.Info("TST");
            Log.Debug(requestJson);
            Log.Debug(json);

            BL.Entities.TST.Response tstResponse = new Entities.TST.Response();
            if (String.IsNullOrEmpty(json) == false)
            {
                json = json.Replace("@", "");

                tstResponse = BL.Entities.TST.Response.FromJson(json);
            }

            if (tstResponse.Error != null)
            {
                response.isError = true;
                response.errorMessage = tstResponse.Error.ErrorMessage + "(" + tstResponse.Error.ErrorCode + ")";
                return response;
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
            pnrSavedReuest.bookingOID = request.bookingOID;
            pnrSavedReuest.optionCode = "11";
            pnrSavedReuest.rfLongFreeText = "Gogojii";
            pnrSavedReuest.departFlightSegmentCount = request.depFlight.Count;
            pnrSavedReuest.returnFlightSegmentCount = request.retFlight == null ? 0 : request.retFlight.Count;
            pnrSavedReuest.accessOID = "BKKIW38EH";

            pnrSavedReuest.queues = new List<PNRQueue>();
            if (ConfigurationManager.AppSettings["WEBMODE"] == "PROD")
            {
                pnrSavedReuest.queues.Add(new PNRQueue()
                {
                    categoryNumber = "0",
                    officeID = "BKKIW38EH", // "BKKOK261I",
                    queueNumber = "60"
                });
            }
            else
            {
                pnrSavedReuest.queues.Add(new PNRQueue()
                {
                    categoryNumber = "0",
                    officeID = "BKKIW38EH",
                    queueNumber = "60"
                });
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
                response.isError = true;
                response.errorMessage = pnrSavedResponse.Error.ErrorMessage + "(" + pnrSavedResponse.Error.ErrorCode + ")";

                return response;
            }

            response.isError = false;
            response.recordLocator = pnrSavedResponse.PnrReply.PnrHeader.ReservationInfo.Reservation.ControlNumber;

            //Send message to Gogojii API
            Entities.BookingTransaction.Request transaction = new Entities.BookingTransaction.Request();
            transaction.BookingPassenger = new List<Entities.BookingTransaction.BookingPassenger>();
            transaction.BookingPaxFare = new List<Entities.BookingTransaction.BookingPaxFare>();
            int iPax = 1;
            foreach (var pax in request.adtPaxs)
            {
                Entities.BookingTransaction.BookingPassenger bookingPax = new Entities.BookingTransaction.BookingPassenger();
                bookingPax.SeatRequestId = "";
                bookingPax.HaveInfant = false;
                bookingPax.SeatNumber = "";
                bookingPax.FrequentFlyerAirline = "";
                bookingPax.FoodRequestId = "";
                bookingPax.InsureNo = "";
                bookingPax.WithInsure = false;
                bookingPax.FirstName = pax.firstname;
                bookingPax.TicketID = response.recordLocator;
                bookingPax.MiddleName = pax.middlename ?? "";
                bookingPax.LastName = pax.lastname;
                bookingPax.SeatRequest = false;
                bookingPax.WithADT = 0;
                bookingPax.PassportNumber = "";
                bookingPax.PaxType = "ADT";
                bookingPax.TitleName = pax.title;
                bookingPax.PTNo = iPax;
                bookingPax.FrequentFlyerNumber = "";
                bookingPax.PaxNo = iPax;
                
                transaction.BookingPassenger.Add(bookingPax);
                iPax++;

            }
            //Fare
            Entities.BookingTransaction.BookingPaxFare paxFare = new Entities.BookingTransaction.BookingPaxFare();
            paxFare.Currency = "THB";
            paxFare.FareWithoutTax = Convert.ToInt32(request.adtFare.sellingBaseFare);
            paxFare.FareWithTax = Convert.ToInt32(request.adtFare.net);
            paxFare.Tax = Convert.ToInt32(request.adtFare.tax);
            paxFare.PaxType = "ADT";
            paxFare.NoOfPax = 1;// request.adtPaxs.Count;
            if (request.adtFare.baggages != null && request.adtFare.baggages.Count > 0)
            {
                paxFare.Weight = request.adtFare.baggages[0].baggageNo;
                paxFare.WeightUnits = request.adtFare.baggages[0].baggageUnit;
            }
            transaction.BookingPaxFare.Add(paxFare);

            if (request.chdPaxs != null)
            {
                foreach (var pax in request.chdPaxs)
                {
                    Entities.BookingTransaction.BookingPassenger bookingPax = new Entities.BookingTransaction.BookingPassenger();
                    bookingPax.SeatRequestId = "";
                    bookingPax.HaveInfant = false;
                    bookingPax.SeatNumber = "";
                    bookingPax.FrequentFlyerAirline = "";
                    bookingPax.FoodRequestId = "";
                    bookingPax.InsureNo = "";
                    bookingPax.WithInsure = false;
                    bookingPax.FirstName = pax.firstname;
                    bookingPax.TicketID = response.recordLocator;
                    bookingPax.MiddleName = pax.middlename ?? "";
                    bookingPax.LastName = pax.lastname;
                    bookingPax.SeatRequest = false;
                    bookingPax.WithADT = 0;
                    bookingPax.PassportNumber = "";
                    bookingPax.PaxType = "CHD";
                    bookingPax.TitleName = pax.title;
                    bookingPax.PTNo = iPax;
                    bookingPax.FrequentFlyerNumber = "";
                    bookingPax.PaxNo = iPax;
                    bookingPax.DOB = pax.birthday;
                    transaction.BookingPassenger.Add(bookingPax);
                    iPax++;
                }
                //Fare
                paxFare = new Entities.BookingTransaction.BookingPaxFare();
                paxFare.Currency = "THB";
                paxFare.FareWithoutTax = Convert.ToInt32(request.chdFare.sellingBaseFare);
                paxFare.FareWithTax = Convert.ToInt32(request.chdFare.net);
                paxFare.Tax = Convert.ToInt32(request.chdFare.tax);
                paxFare.PaxType = "CHD";
                paxFare.NoOfPax = 2;// request.chdPaxs.Count;
                if (request.chdFare.baggages != null && request.chdFare.baggages.Count > 0)
                {
                    paxFare.Weight = request.chdFare.baggages[0].baggageNo;
                    paxFare.WeightUnits = request.chdFare.baggages[0].baggageUnit;
                }
                transaction.BookingPaxFare.Add(paxFare);
            }

            if (request.infPaxs != null)
            {
                int iADT = 1;
                foreach (var pax in request.infPaxs)
                {
                    Entities.BookingTransaction.BookingPassenger bookingPax = new Entities.BookingTransaction.BookingPassenger();
                    bookingPax.SeatRequestId = "";
                    bookingPax.HaveInfant = false;
                    bookingPax.SeatNumber = "";
                    bookingPax.FrequentFlyerAirline = "";
                    bookingPax.FoodRequestId = "";
                    bookingPax.InsureNo = "";
                    bookingPax.WithInsure = false;
                    bookingPax.FirstName = pax.firstname;
                    bookingPax.TicketID = response.recordLocator;
                    bookingPax.MiddleName = pax.middlename ?? "";
                    bookingPax.LastName = pax.lastname;
                    bookingPax.SeatRequest = false;
                    bookingPax.WithADT = iADT;
                    bookingPax.PassportNumber = "";
                    bookingPax.PaxType = "INF";
                    bookingPax.TitleName = pax.title;
                    bookingPax.PTNo = iPax;
                    bookingPax.FrequentFlyerNumber = "";
                    bookingPax.PaxNo = iPax;
                    bookingPax.DOB = pax.birthday;
                    transaction.BookingPassenger.Add(bookingPax);
                    iPax++;
                    iADT++;
                }
                //Fare
                paxFare = new Entities.BookingTransaction.BookingPaxFare();
                paxFare.Currency = "THB";
                paxFare.FareWithoutTax = Convert.ToInt32(request.infFare.sellingBaseFare);
                paxFare.FareWithTax = Convert.ToInt32(request.infFare.net);
                paxFare.Tax = Convert.ToInt32(request.infFare.tax);
                paxFare.PaxType = "INF";
                paxFare.NoOfPax = request.chdPaxs != null ? 3 : 2;// request.infPaxs.Count;
                if (request.infFare.baggages != null && request.infFare.baggages.Count > 0)
                {
                    paxFare.Weight = request.infFare.baggages[0].baggageNo;
                    paxFare.WeightUnits = request.infFare.baggages[0].baggageUnit;
                }
                transaction.BookingPaxFare.Add(paxFare);
            }
            transaction.PNR = response.recordLocator;

            if (request.fareRules.Count > 0)
            {
                transaction.Penalties1 = GetAllFareRuleString(request.fareRules[0].rules);
                //var rule = request.fareRules[0].rules.FirstOrDefault(x => x.category == "PE");
                //if (rule != null)
                //{
                //    transaction.Penalties1 = GetAllFareRuleString(request.fareRules[0].rules); //String.Join("\n", rule.fareRuleText.ToArray());
                //}
                //else
                //{
                //    transaction.Penalties1 = "";
                //}
            }
            else
            {
                transaction.Penalties1 = "";
            }

            if (request.fareRules.Count > 1)
            {
                transaction.Penalties2 = GetAllFareRuleString(request.fareRules[1].rules);
                //var rule = request.fareRules[1].rules.FirstOrDefault(x => x.category == "PE");
                //if (rule != null)
                //{
                //    transaction.Penalties2 = String.Join("\n", rule.fareRuleText.ToArray());
                //}
                //else
                //{
                //    transaction.Penalties2 = "";
                //}
            }
            else
            {
                transaction.Penalties2 = transaction.Penalties1;
            }

            transaction.ContactDestPhone = request.adtPaxs[0].telNo;
            transaction.ContactPhone = request.adtPaxs[0].telNo;
            transaction.TotalAmount = Convert.ToInt32(request.grandTotal);
            transaction.PNRStatus = 1;
            transaction.NoOfPax = iPax;
            transaction.FareFamily = "";
            transaction.ShortFareCondition = "";
            transaction.TripType = (request.retFlight != null && request.retFlight.Count > 0) ? "R" : "D";
            transaction.Discount = 0;
            transaction.TotalADT = request.noOfAdults;
            transaction.TotalCHD = request.noOfChildren;
            transaction.TotalINF = request.noOfInfants;
            transaction.ContactEmail = request.adtPaxs[0].email;
            transaction.ContactFirstName = request.adtPaxs[0].firstname;
            transaction.ContactLastName = request.adtPaxs[0].lastname;
            transaction.ContactMiddleName = request.adtPaxs[0].middlename ?? "";
            transaction.ContactTitleName = request.adtPaxs[0].title;
            transaction.TicketCounter = 0;
            transaction.Currency = "THB";
            transaction.InsureQuoteTotal = 0;
            transaction.SelectedLanguage = "TH";
            transaction.ContactAddress = "";
            transaction.PaymentStatus = 0;
            transaction.PaymentMethod = "";
            transaction.BookingFlight = new List<Entities.BookingTransaction.BookingFlight>();

            int iSegment = 1;
            foreach (var flight in request.depFlight)
            {
                Entities.BookingTransaction.BookingFlight bookingFlight = new Entities.BookingTransaction.BookingFlight();
                bookingFlight.ToCityFullName = flight.arrCity.name;
                bookingFlight.CabinClass = request.svc_class;
                bookingFlight.ArrivalDateTime = flight.arrivalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                bookingFlight.FromAirportCode = flight.depCity.code;
                bookingFlight.ToCity = flight.arrCity.code;
                bookingFlight.FromAirportFullName = flight.depCity.name;
                bookingFlight.FareBasis = flight.fareBasis;
                bookingFlight.FareType = flight.fareType;
                bookingFlight.TripType = "D";
                bookingFlight.FromCityFullName = flight.depCity.name;
                bookingFlight.OperateAirline = flight.operatedAirline.code;
                bookingFlight.DepartureDateTime = flight.departureDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                bookingFlight.Duration = flight.flightTime.Substring(0, 2) + ":" + flight.flightTime.Substring(2, 2);
                bookingFlight.ToAirportCode = flight.arrCity.code;
                bookingFlight.AircraftType = "";
                bookingFlight.MarketAirline = flight.airline.code;
                bookingFlight.routeno = 1;
                bookingFlight.SegmentNo = iSegment;
                bookingFlight.RBD = flight.rbd;
                bookingFlight.TerminalId = 0;
                bookingFlight.ToAirportFullName = flight.arrCity.name;
                bookingFlight.FromCity = flight.depCity.code;
                bookingFlight.FlightNo = flight.flightNumber;

                transaction.BookingFlight.Add(bookingFlight);
                iSegment++;
            }
            iSegment = 1;
            if (request.retFlight != null)
            {
                foreach (var flight in request.retFlight)
                {
                    Entities.BookingTransaction.BookingFlight bookingFlight = new Entities.BookingTransaction.BookingFlight();
                    bookingFlight.ToCityFullName = flight.arrCity.name;
                    bookingFlight.CabinClass = request.svc_class;
                    bookingFlight.ArrivalDateTime = flight.arrivalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    bookingFlight.FromAirportCode = flight.depCity.code;
                    bookingFlight.ToCity = flight.arrCity.code;
                    bookingFlight.FromAirportFullName = flight.depCity.name;
                    bookingFlight.FareBasis = flight.fareBasis;
                    bookingFlight.FareType = flight.fareType;
                    bookingFlight.TripType = "R";
                    bookingFlight.FromCityFullName = flight.depCity.name;
                    bookingFlight.OperateAirline = flight.operatedAirline.code;
                    bookingFlight.DepartureDateTime = flight.departureDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    bookingFlight.Duration = flight.flightTime.Substring(0, 2) + ":" + flight.flightTime.Substring(2, 2);
                    bookingFlight.ToAirportCode = flight.arrCity.code;
                    bookingFlight.AircraftType = "";
                    bookingFlight.MarketAirline = flight.airline.code;
                    bookingFlight.routeno = 2;
                    bookingFlight.SegmentNo = iSegment;
                    bookingFlight.RBD = flight.rbd;
                    bookingFlight.TerminalId = 0;
                    bookingFlight.ToAirportFullName = flight.arrCity.name;
                    bookingFlight.FromCity = flight.depCity.code;
                    bookingFlight.FlightNo = flight.flightNumber;

                    transaction.BookingFlight.Add(bookingFlight);
                    iSegment++;
                }
            }
            transaction.PaymentDateTime = request.TKTL.ToString("yyyy-MM-dd HH:mm:ss");
            transaction.SendMailNotify = 1;
            transaction.IsHappyHour = "0";
            transaction.TicketStatus = 0;
            transaction.PaymentOrderID = "";
            transaction.ContactPostCode = "";
            transaction.SelectedFareFamily = "";
            transaction.ContactCity = "";
            transaction.IsSubscribeEmail = true;

            try
            {
                url = ConfigurationManager.AppSettings["BOOKINGTRANSACTION.URL"];
                requestJson = JsonConvert.SerializeObject(transaction);
                Log.Info("BOOKINGTRANSACTION");
                Log.Debug(requestJson);

                json = HttpUtility.postJSON(url, requestJson);
                Log.Debug(json);
                if (String.IsNullOrEmpty(json) == false)
                {
                    var bookingRes = JsonConvert.DeserializeObject<Entities.BookingTransaction.Response>(json);
                    response.BookingKeyReference = bookingRes.BookingKeyReference;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            //LINE
            BL.Entities.LineNotify.Request lineReq = new Entities.LineNotify.Request();
            string lineMessage = "Booking Complete\nวันที่ทำรายการ {0}\n{1}\n{2}\n{3}\n{4} - {5}\nPNR: {6}\ndepart flight: {7}\nreturn flight : {8}\nWebsite";
            string depFlight = "";
            string retFlight = "";
            foreach (var flight in request.depFlight)
            {
                depFlight += (depFlight == "" ? "" : " - ") + flight.airline.code + flight.flightNumber;
            }
            if (request.retFlight != null)
            {
                foreach (var flight in request.retFlight)
                {
                    retFlight += (retFlight == "" ? "" : " - ") + flight.airline.code + flight.flightNumber;
                }
            }
            lineReq.Message = String.Format(lineMessage, DateTime.Now.ToString("dd MMM yyyy"),
            request.adtPaxs[0].title + " " + request.adtPaxs[0].firstname + " " + request.adtPaxs[0].middlename + " " + request.adtPaxs[0].lastname,
            request.adtPaxs[0].email,
            request.adtPaxs[0].telNo,
            request.origin.code,
            request.destination.code,
            response.recordLocator,
            depFlight,
            retFlight);
            try
            {
                url = ConfigurationManager.AppSettings["LINENOTIFY.URL"];
                requestJson = JsonConvert.SerializeObject(lineReq);
                Log.Info("LINENOTIFY");
                Log.Debug(requestJson);

                json = HttpUtility.postJSON(url, requestJson);
                Log.Debug(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return response;
        }

        private string GetAllFareRuleString(List<FareRuleDatail> rules)
        {
            string farerule = "";
            foreach(var rule in rules)
            {
                farerule += "<br />";
                string topic = rule.category;
                switch(rule.category)
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

        public string GetInvoiceNo(Entities.Invoice.Request invoiceReq)
        {
            string url = ConfigurationManager.AppSettings["INVOICE.URL"];
            string requestJson = JsonConvert.SerializeObject(invoiceReq);
            Log.Info("INVOICE");
            Log.Debug(requestJson);

            string json = HttpUtility.postJSON(url, requestJson);
            Log.Debug(json);
            if (String.IsNullOrEmpty(json) == false)
            {
                var invoiceRes = JsonConvert.DeserializeObject<Entities.Invoice.Response>(json);
                return invoiceRes.Invoice1;
            }
            return invoiceReq.Ref;
        }

        public void UpdatePaymentStatus(Entities.GogojiiPNR.PNR pnr, int paymentType)
        {
            /*
                1 = Cradit Card
                2 = PayPal
                3 = Tranfer
                4 = Counter Service
             
             */
            string requestJson = @"{
            ""BookingKeyReference"":""" + pnr.BookingKeyReference + @""" ,
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
    }
}
