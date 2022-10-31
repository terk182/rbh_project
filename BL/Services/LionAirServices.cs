using BL.Entities.LionAir_CreateSession;
using BL.Entities.LionAir_FlightMatrix;
using BL.Entities.RobinhoodFlight;
using BL.Utilities;
using DataModel.UnitOfWork;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BL
{
    public class LionAirServices : ILionAirServices
    {
        private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UnitOfWork _unitOfWork;
        private decimal paymentFee;
        private bool isPaymentFeePct;
        public LionAirServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            var payment = _unitOfWork.PaymentRepository.GetFirstOrDefault(x => 1 == 1);
            paymentFee = payment.KbankFullValue.GetValueOrDefault();
            isPaymentFeePct = payment.IspercentKbankFull.GetValueOrDefault();
        }
        public FlightSearchMultiTicketResult Search(Entities.MasterPricer.Request request)
        {
            FlightSearchMultiTicketResult result = new FlightSearchMultiTicketResult();
            MarkupServices markupSvc = new MarkupServices(_unitOfWork);
            FareMarkup markup = markupSvc.GetMarkup();
            NamingServices namingService = new NamingServices(_unitOfWork);
            CreateSessionReq sessionReq = new CreateSessionReq();
            string sMessage = sessionReq.GetCreateSessionRequest();
            Log.Debug("SessionCreate");
            Log.Debug(sMessage);
            LionAirAPI lionAPI = new LionAirAPI();
            lionAPI.serviceName = "SessionCreate.asmx";
            lionAPI.message = sMessage;
            string response = lionAPI.post();
            Log.Debug(response);
            Log.Debug("END SessionCreate");
            string BinarySecurityToken = "";
            List<MultiTicketFlight> multiTicketFlightList = null;
            if (response != null && response.Length > 0)
            {
                ///Envelope/Body/LogonResponse/LogonResult
                string sXmlResponse = response.Replace("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<Envelope>");
                sXmlResponse = sXmlResponse.Replace("</soap:Envelope>", "</Envelope>");
                sXmlResponse = sXmlResponse.Replace("soap:", "");
                sXmlResponse = sXmlResponse.Replace("<MessageHeader xmlns=\"http://www.ebxml.org/namespaces/messageHeader\">", "<MessageHeader>");
                sXmlResponse = sXmlResponse.Replace("<Security xmlns=\"http://schemas.xmlsoap.org/ws/2002/12/secext\">", "<Security>");
                sXmlResponse = sXmlResponse.Replace("<LogonResponse xmlns=\"http://www.vedaleon.com/webservices\">", "<LogonResponse>");
                Log.Debug(sXmlResponse);
                XmlDocument xmlMaster = new XmlDocument();
                xmlMaster.LoadXml(sXmlResponse);
                if (xmlMaster != null && xmlMaster.SelectSingleNode("//Envelope/Header/Security/BinarySecurityToken") != null)
                {
                    BinarySecurityToken = xmlMaster.SelectSingleNode("//Envelope/Header/Security/BinarySecurityToken").InnerText;
                    Log.Debug(BinarySecurityToken);
                    FlightMatrixReq flightMatrixReq = new FlightMatrixReq();
                    flightMatrixReq.BinarySecurityToken = BinarySecurityToken;
                    flightMatrixReq.TripType = request.flights.Count == 1 ? "OneWay" : "Return";
                    flightMatrixReq.noAdult = request.noOfAdults;
                    flightMatrixReq.noChild = request.noOfChildren;
                    flightMatrixReq.noInfant = request.noOfInfants;
                    flightMatrixReq.DepartureCode = request.flights[0].departureCity;
                    flightMatrixReq.DestinationCode = request.flights[0].arrivalCity;
                    flightMatrixReq.DepartureDateTime = request.flights[0].departureDateTime;
                    if (request.flights.Count > 1)
                    {
                        flightMatrixReq.ArrivalDateTime = request.flights[1].departureDateTime;
                    }
                    sMessage = flightMatrixReq.GetFlightMatrixRequest();
                    Log.Debug("FlightMatrixService");
                    Log.Debug(sMessage);

                    lionAPI = new LionAirAPI();
                    lionAPI.serviceName = "FlightMatrixService.asmx";
                    lionAPI.message = sMessage;
                    response = lionAPI.post();
                    Log.Debug(response);
                    Log.Debug("END FlightMatrixService");

                    sXmlResponse = response.Replace("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<Envelope>");
                    sXmlResponse = sXmlResponse.Replace("</soap:Envelope>", "</Envelope>");
                    sXmlResponse = sXmlResponse.Replace("soap:", "");
                    sXmlResponse = sXmlResponse.Replace("<MessageHeader xmlns=\"http://www.ebxml.org/namespaces/messageHeader\">", "<MessageHeader>");
                    sXmlResponse = sXmlResponse.Replace("<Security xmlns=\"http://schemas.xmlsoap.org/ws/2002/12/secext\">", "<Security>");
                    sXmlResponse = sXmlResponse.Replace("<FlightMatrixRequestResponse xmlns=\"http://www.vedaleon.com/webservices\">", "<FlightMatrixRequestResponse>");
                    sXmlResponse = sXmlResponse.Replace("<FlightSearchResult>Normal</FlightSearchResult>", "");
                    Log.Debug(sXmlResponse);
                    xmlMaster = new XmlDocument();
                    xmlMaster.LoadXml(sXmlResponse);
                    //Envelope/Body/FlightMatrixRequestResponse/FlightMatrixRS/Errors
                    if (xmlMaster != null && xmlMaster.SelectSingleNode("//Envelope/Body/FlightMatrixRequestResponse/FlightMatrixRS/Errors") != null && xmlMaster.SelectSingleNode("//Envelope/Body/FlightMatrixRequestResponse/FlightMatrixRS/Errors").InnerText.Length == 0)
                    {
                        XmlNodeList nodeList = xmlMaster.SelectNodes("//FlightMatrixRS/FlightMatrixOptions/FlightMatrixOption");//Fares
                        XmlNodeList nodeFlightMatrixRowList = xmlMaster.SelectNodes("//FlightMatrixRS/FlightMatrices/FlightMatrix");
                        XmlNodeList nodePTCList;
                        XmlNode nodeFlightMatrixList;
                        List<Entities.RobinhoodFlight.Flight> GetAllFlight = new List<Entities.RobinhoodFlight.Flight>();
                        string paxType = "";
                        if (nodeList != null && nodeList.Count > 0)
                        {
                            string sCurrencyCode= xmlMaster.SelectSingleNode("//Envelope/Body/FlightMatrixRequestResponse/FlightMatrixRS/Currency").InnerText;
                            int iRecommendation_number = 0;
                            MultiTicketFlight multiTicket = new MultiTicketFlight();
                            multiTicketFlightList = new List<MultiTicketFlight>();
                            foreach (XmlNode node in nodeList)
                            {
                                iRecommendation_number++;
                                multiTicket = new MultiTicketFlight();
                                multiTicket.type = "L";//A=Amadeus,L=LionAir
                                multiTicket.id = Guid.NewGuid();
                                multiTicket.recommendation_number = iRecommendation_number.ToString();
                                Log.Debug("multiTicket.recommendation_number=" + multiTicket.recommendation_number);
                                multiTicket.isMultiTicket = true;
                                nodeFlightMatrixList = node.SelectSingleNode("FlightMatrixIndices");
                                //Envelope/Body/FlightMatrixRequestResponse/FlightMatrixRS/FlightMatrixOptions/FlightMatrixOption[1]/PTC_FareBreakdown/PassengerTypeQuantity/@Code
                                XmlNode nodeFareBreakdownADT = node.SelectSingleNode("PTC_FareBreakdown[PassengerTypeQuantity/@Code='ADT']");
                                
                                for (int i = 0; i < request.flights.Count; i++)
                                {
                                    GetAllFlight = GetFlight(nodeFlightMatrixList,nodeFlightMatrixRowList[i],request.languageCode, request.flights[i].departureDateTime, i + 1, nodeFareBreakdownADT.SelectSingleNode("FareBasisCodes"), nodeFareBreakdownADT.SelectSingleNode("FareInfo"), BinarySecurityToken);
                                    if (GetAllFlight.Count > 0)
                                    {                                       
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
                                if (request.flights.Count == 2 && (multiTicket.Flight_SegRef1!=null && multiTicket.Flight_SegRef1.Count>0) && (multiTicket.Flight_SegRef2 != null && multiTicket.Flight_SegRef2.Count > 0))
                                {
                                    multiTicket.isMultiTicket = false;
                                }
                                nodePTCList = node.SelectNodes("PTC_FareBreakdown");//PaxFare
                                multiTicket.fare = new Pricing(request.noOfAdults, request.noOfChildren, request.noOfInfants);
                                multiTicket.fare.currencyCode = sCurrencyCode;
                                foreach (XmlNode nodePTC in nodePTCList)
                                {
                                    paxType = nodePTC.SelectSingleNode("PassengerTypeQuantity/@Code").InnerText;
                                    if (paxType == "ADT")
                                    {
                                        multiTicket.fare.adtFare = new PaxPricing();
                                        multiTicket.fare.adtFare = getPaxFare(nodePTC, "ADT", markup, multiTicket.Flight_SegRef1!=null ? multiTicket.Flight_SegRef1: multiTicket.Flight_SegRef2, namingService, request.userEmail,(!multiTicket.isMultiTicket && request.flights.Count == 2?"R":"O"));

                                    }
                                    else if(paxType == "CNN")
                                    {
                                        multiTicket.fare.chdFare = new PaxPricing();
                                        multiTicket.fare.chdFare = getPaxFare(nodePTC, "CHD", markup, multiTicket.Flight_SegRef1 != null ? multiTicket.Flight_SegRef1 : multiTicket.Flight_SegRef2, namingService, request.userEmail, (!multiTicket.isMultiTicket && request.flights.Count == 2 ? "R" : "O"));
                                    }
                                    else
                                    {
                                        multiTicket.fare.infFare = new PaxPricing();
                                        multiTicket.fare.infFare = getPaxFare(nodePTC, "INF", markup, multiTicket.Flight_SegRef1 != null ? multiTicket.Flight_SegRef1 : multiTicket.Flight_SegRef2, namingService, request.userEmail, (!multiTicket.isMultiTicket && request.flights.Count == 2 ? "R" : "O"));
                                    }
                                }
                                multiTicketFlightList.Add(multiTicket);
                            }
                        }
                    }
                }
            }

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

            return result;
        }
        private List<Entities.RobinhoodFlight.Flight> GetFlight(XmlNode nodeFlightMatrixIndex, XmlNode nodeFlightMatrixRow, string languageCode, DateTime depDate, int segmentNo, XmlNode nodefareBasis, XmlNode nodeRBD, string pgSearchOID)
        {
            NamingServices namingService = new NamingServices(_unitOfWork);
            List<Entities.RobinhoodFlight.Flight> flightList = new List<Entities.RobinhoodFlight.Flight>();
            XmlNodeList nodeFlightMatrixIndexList = segmentNo == 1 ? nodeFlightMatrixIndex.SelectNodes("FlightMatrixIndex/FlightOB") : nodeFlightMatrixIndex.SelectNodes("FlightMatrixIndex/FlightIB");
            List<string> refNo = new List<string>();
            string sRefNo = "";
            foreach (XmlNode node in nodeFlightMatrixIndexList)
            {
                if (int.Parse(node.InnerText) >= 0)
                {
                    sRefNo = node.InnerText;
                    if (!refNo.Contains(sRefNo))
                    {
                        refNo.Add(sRefNo);
                    }
                }
            }
            if (refNo.Count > 0)
            {
                Entities.RobinhoodFlight.Flight flight = new Entities.RobinhoodFlight.Flight();
                Entities.RobinhoodFlight.FlightDetail detail = new Entities.RobinhoodFlight.FlightDetail();
                int iSeq = 0;
                string sDuration = "";
                foreach (string rN in refNo)
                {
                    iSeq = 0;
                    sDuration = "";
                    flight = new Entities.RobinhoodFlight.Flight();
                    flight.bShow = true;
                    flight.refNumber = rN;
                    flight.pgSearchOID = pgSearchOID;
                    flight.flightFrom = "L";//A:Amadeus,L:LionAir
                    XmlNode nodeRow = nodeFlightMatrixRow.SelectSingleNode("FlightMatrixRows/FlightMatrixRow[RPH='"+rN+"']");
                    XmlNodeList nodeList = nodeRow.SelectNodes("OriginDestinationOptionType/FlightSegment");
                    XmlNode nodeRBDSeat;
                    flight.flightDetails = new List<Entities.RobinhoodFlight.FlightDetail>();
                    foreach (XmlNode node in nodeList)
                    {
                        iSeq++;
                        detail = new Entities.RobinhoodFlight.FlightDetail();
                        detail.fareType = "RP";
                        detail.airline = new Airline(namingService, languageCode);
                        detail.airline.code = node.SelectSingleNode("MarketingAirline/@Code").InnerText;
                        detail.operatedAirline = new Airline(namingService, languageCode);
                        detail.operatedAirline.code = node.SelectSingleNode("OperatingAirline/@Code").InnerText;
                        detail.flightNumber = node.SelectSingleNode("@FlightNumber").InnerText;
                        detail.depCity = new Airport(namingService, true, languageCode);
                        detail.depCity.code = node.SelectSingleNode("DepartureAirport/@LocationCode").InnerText;
                        detail.arrCity = new Airport(namingService, true, languageCode);
                        detail.arrCity.code = node.SelectSingleNode("ArrivalAirport/@LocationCode").InnerText;
                        detail.departureDateTime = Convert.ToDateTime(node.SelectSingleNode("@DepartureDateTime").InnerText);
                        detail.arrivalDateTime = Convert.ToDateTime(node.SelectSingleNode("@ArrivalDateTime").InnerText);
                        //detail.departureDateTime = DateTime.ParseExact(node.SelectSingleNode("@DepartureDateTime").InnerText
                        //    , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        //detail.arrivalDateTime = DateTime.ParseExact(node.SelectSingleNode("@ArrivalDateTime").InnerText
                        //    , "ddMMyyHHmm", System.Globalization.CultureInfo.InvariantCulture);

                        detail.equipmentTypeName = node.SelectSingleNode("Equipment/@AirEquipType").InnerText;
                        detail.equipmentType = detail.equipmentTypeName.Substring(0, 3);
                        detail.fareBasis = nodefareBasis.SelectSingleNode("FareBasisCode[" + iSeq + "]").InnerText;
                        detail.cabin = nodeRBD.SelectSingleNode("FareReference[" + iSeq + "]/text()").InnerText;
                        detail.rbd = nodeRBD.SelectSingleNode("FareReference[" + iSeq + "]/@ResBookDesigCode").InnerText;

                        nodeRBDSeat = node.SelectSingleNode("BookingClassAvails/BookingClassAvail[@ResBookDesigCode='" + detail.rbd + "']/@ResBookDesigQuantity");
                        detail.availableSeat = int.Parse(nodeRBDSeat.InnerText);


                        detail.Seq = iSeq;
                        detail.flightTime = node.SelectSingleNode("Duration").InnerText;
                        if (sDuration.Length > 0)
                        {
                            sDuration += "|";
                        }
                        sDuration += detail.flightTime;

                        detail.setDisplayDateTime(languageCode, depDate);
                        detail.inFlightServices = new InFlightServices();
                        flight.flightDetails.Add(detail);
                    }

                    if (flight.flightDetails.Count > 0)
                    {                        
                        flight.totalTime = GetTotalTime(sDuration);//total flightTime
                        flightList.Add(flight);
                    }
                }
            }
            return flightList;
        }
        private static Dictionary<string, string> cityDic = new Dictionary<string, string>();
        private PaxPricing getPaxFare(XmlNode node, string paxType, FareMarkup markup, List<BL.Entities.RobinhoodFlight.Flight> depFlights, NamingServices namingService, string userEmail, string tripType)
        {
            PaxPricing paxPricing = new PaxPricing();
            decimal dTotalFare = Convert.ToDecimal(node.SelectSingleNode("PassengerFare/TotalFare/@Amount").InnerText);           
            paxPricing.fareBeforeMarkup = Convert.ToDecimal(node.SelectSingleNode("PassengerFare/BaseFare/@Amount").InnerText);
            paxPricing.tax = dTotalFare - paxPricing.fareBeforeMarkup;
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
        
        private string GetTotalTime(string sDuration)
        {
            string result = "";
            string[] arr = sDuration.Split('|');//01:15|01:25
            if (arr.Length > 0)
            {
                int iHour = 0;
                int iMinute = 0;
                int iAllMinute = 0;
                int iTimeHour = 0;
                int iTimeMinute = 0;
                for (int i = 0; i < arr.Length; i++)
                {
                    string[] arrTime = arr[i].Split(':');
                    iHour += int.Parse(arrTime[0]);
                    iMinute+= int.Parse(arrTime[1]);
                }
                iAllMinute = (iHour * 60) + iMinute;
                iTimeHour = iAllMinute / 60;//Hour
                iTimeMinute = iAllMinute% 60;//Minute
                
                result = String.Format("{0}{1}", iTimeHour.ToString().PadLeft(2, '0'), iTimeMinute.ToString().PadLeft(2, '0'));
            }
            return result;
        }

        public Entities.RobinhoodFare.AirFare AirSellService(Entities.InformativePricing.Request request, Entities.InformativePricing.Request requestFor1A, string languageCode)
        {
            NamingServices namingServices = new NamingServices(_unitOfWork);
            Entities.RobinhoodFare.AirFare airFare = new Entities.RobinhoodFare.AirFare(namingServices, languageCode);
            airFare.type = "L";//A:Amadeus,L:Lion Air

            Entities.LionAir_AirSellService.AirSellRQ airReq = new Entities.LionAir_AirSellService.AirSellRQ();
            airReq.BinarySecurityToken = request.depFlightType == "L" ? request.dep_pgSearchOID : request.ret_pgSearchOID;
            airReq.TripType = (request.tripType.Equals("R") && request.depFlightType=="L" && request.retFlightType=="L") ? "Return" : "OneWay";
            airReq.noAdult = request.noOfAdults;
            airReq.noChild = request.noOfChildren;
            airReq.noInfant = request.noOfInfants;
            airReq.Flights = new List<Entities.LionAir_AirSellService.Flight>();
            Entities.LionAir_AirSellService.Flight flight = new Entities.LionAir_AirSellService.Flight();
            Entities.LionAir_AirSellService.FlightSegment segment = new Entities.LionAir_AirSellService.FlightSegment();
            int iSeq = 0;
            if (request.depFlightType == "L")
            {
                flight.Type = "D";
                flight.FlightSegments = new List<Entities.LionAir_AirSellService.FlightSegment>();
                foreach (var _flight in request.departureFlights)
                {
                    segment = new Entities.LionAir_AirSellService.FlightSegment();
                    segment.Seq = iSeq;
                    segment.DepartureDateTime = _flight.departureDateTime;
                    segment.ArrivalDateTime = _flight.arrivalDateTime;
                    segment.FlightNumber = _flight.flightNumber;
                    segment.ResBookDesigCode = _flight.RBDCode;
                    segment.DepartureCode = _flight.departureCity;
                    segment.DestinationCode = _flight.arrivalCity;
                    segment.MarketingAirline = _flight.companyCode;
                    segment.OperatingAirline = _flight.operatedBy;
                    segment.RPH = request.depRefNumber;
                    flight.FlightSegments.Add(segment);
                    iSeq++;
                }
                airReq.Flights.Add(flight);
            }
            if (request.tripType.Equals("R") && request.retFlightType == "L")
            {
                flight = new Entities.LionAir_AirSellService.Flight();
                flight.Type = "R";
                flight.FlightSegments = new List<Entities.LionAir_AirSellService.FlightSegment>();
                iSeq = 0;
                foreach (var _flight in request.returnFlights)
                {
                    segment = new Entities.LionAir_AirSellService.FlightSegment();
                    segment.Seq = iSeq;
                    segment.DepartureDateTime = _flight.departureDateTime;
                    segment.ArrivalDateTime = _flight.arrivalDateTime;
                    segment.FlightNumber = _flight.flightNumber;
                    segment.ResBookDesigCode = _flight.RBDCode;
                    segment.DepartureCode = _flight.departureCity;
                    segment.DestinationCode = _flight.arrivalCity;
                    segment.MarketingAirline = _flight.companyCode;
                    segment.OperatingAirline = _flight.operatedBy;
                    segment.RPH = request.retRefNumber;
                    flight.FlightSegments.Add(segment);
                    iSeq++;
                }
                airReq.Flights.Add(flight);
            }

            string sMessage = airReq.GetAirSellRequest();
            Log.Debug("AirSellRQ");
            Log.Debug(sMessage);
            LionAirAPI lionAPI = new LionAirAPI();
            lionAPI.serviceName = "OTA_AirSellService.asmx";
            lionAPI.message = sMessage;
            string response = lionAPI.post();
            Log.Debug(response);
            Log.Debug("END AirSellRQ");
            if (response != null && response.Length > 0)
            {
                ///Envelope/Body/LogonResponse/LogonResult
                string sXmlResponse = response.Replace("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<Envelope>");
                sXmlResponse = sXmlResponse.Replace("</soap:Envelope>", "</Envelope>");
                sXmlResponse = sXmlResponse.Replace("soap:", "");
                sXmlResponse = sXmlResponse.Replace("<MessageHeader xmlns=\"http://www.ebxml.org/namespaces/messageHeader\">", "<MessageHeader>");
                sXmlResponse = sXmlResponse.Replace("<Security xmlns=\"http://schemas.xmlsoap.org/ws/2002/12/secext\">", "<Security>");
                sXmlResponse = sXmlResponse.Replace("<AirSellRQResponse xmlns=\"http://www.vedaleon.com/webservices\">", "<AirSellRQResponse>");
                Log.Debug(sXmlResponse);
                XmlDocument xmlMaster = new XmlDocument();
                xmlMaster.LoadXml(sXmlResponse);
                //soap:Envelope/soap:Body/AirSellRQResponse/OTA_AirPriceRS/Errors
                if (xmlMaster != null && xmlMaster.SelectSingleNode("//Envelope/Body/AirSellRQResponse/OTA_AirPriceRS/Errors") != null && xmlMaster.SelectSingleNode("//Envelope/Body/AirSellRQResponse/OTA_AirPriceRS/Errors").InnerText.Length == 0)
                {
                    MarkupServices markupSvc = new MarkupServices(_unitOfWork);
                    FareMarkup markup = markupSvc.GetMarkup();
                    XmlNode node = xmlMaster.SelectSingleNode("AirSellRQResponse/OTA_AirPriceRS/PricedItineraries/PricedItinerary/AirItineraryPricingInfo/PTC_FareBreakdowns/PTC_FareBreakdown[PassengerTypeQuantity/@Code=='ADT']");
                    airFare.adtFare = GetFareFromAirSellService(node, "ADT", markup, request, paymentFee, isPaymentFeePct);
                    if (request.noOfChildren > 0)
                    {
                        node = xmlMaster.SelectSingleNode("AirSellRQResponse/OTA_AirPriceRS/PricedItineraries/PricedItinerary/AirItineraryPricingInfo/PTC_FareBreakdowns/PTC_FareBreakdown[PassengerTypeQuantity/@Code=='CNN']");
                        airFare.chdFare = GetFareFromAirSellService(node, "CHD", markup, request, paymentFee, isPaymentFeePct);
                    }
                    if (request.noOfInfants > 0)
                    {
                        node = xmlMaster.SelectSingleNode("AirSellRQResponse/OTA_AirPriceRS/PricedItineraries/PricedItinerary/AirItineraryPricingInfo/PTC_FareBreakdowns/PTC_FareBreakdown[PassengerTypeQuantity/@Code=='INF']");
                        airFare.infFare = GetFareFromAirSellService(node, "INF", markup, request, paymentFee, isPaymentFeePct);
                    }
                }
                else
                {
                    airFare.isError = true;
                    airFare.errorCode = "3007";
                    airFare.errorMessage = xmlMaster.SelectSingleNode("//Envelope/Body/AirSellRQResponse/OTA_AirPriceRS/Errors").InnerText;
                    return airFare;
                }
            }
            else
            {
                airFare.isError = true;
                airFare.errorCode = "3006";
                airFare.errorMessage = "LionAir - AirSellService not response.";
                return airFare;
            }
            return airFare;
        }

        private Entities.RobinhoodFare.Fare GetFareFromAirSellService(XmlNode node, string paxType, FareMarkup markup,Entities.InformativePricing.Request request, decimal paymentFee, bool isPaymentFeePct)
        {
            Log.Debug("paxType:" + paxType);
            NamingServices namingServices = new NamingServices(_unitOfWork);
            Entities.RobinhoodFare.Fare fare = new Entities.RobinhoodFare.Fare();
            decimal totalFare = Convert.ToDecimal(node.SelectSingleNode("PassengerFare/TotalFare/@Amount").InnerText);
            decimal baseFare = Convert.ToDecimal(node.SelectSingleNode("PassengerFare/BaseFare/@Amount").InnerText);
            decimal tax = totalFare - baseFare;
            Log.Debug("totalFare:" + totalFare);
            Log.Debug("baseFare:" + baseFare);
            Log.Debug("tax:" + tax);
            fare.baseFare = baseFare;
            fare.tax = tax;

            decimal lessFare = 0;
            //Markup
            string depCity = request.departureFlights[0].departureCity;
            string depCountry = namingServices.GetCountryCode(depCity);
            string arrCity = request.departureFlights[request.departureFlights.Count - 1].arrivalCity;
            string arrCountry = namingServices.GetCountryCode(arrCity);
            fare.sellingBaseFare = markup.getFareCharge(fare.baseFare, request.departureFlights[0].companyCode, depCity, depCountry, arrCity, arrCountry,
                    request.departureFlights[0]._ft, request.departureFlights[0].RBDCode,
                    request.departureFlights[0]._fb, paxType, ref lessFare, request.userEmail, paymentFee, isPaymentFeePct, fare.tax, request.tripType);
            return fare;
        }
    }
}
