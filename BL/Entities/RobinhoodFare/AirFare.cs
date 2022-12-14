using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Entities.Pricing;
using System.Text.RegularExpressions;
using BL.Entities.RobinhoodFlight;
using BL.Entities.AirSell;
using BL.Entities.InformativePricing;
using Newtonsoft.Json;
using System.Xml.Serialization;
using log4net;

namespace BL.Entities.RobinhoodFare
{
    public class AirFare
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //public BL.Entities.AirSell.Request airSellRequest { get; set; }
        private NamingServices _namingServices { get; set; }
        private string _languageCode { get; set; }
        public Fare adtFare { get; set; }
        public Fare chdFare { get; set; }
        public Fare infFare { get; set; }
        public List<FlightDetail> depFlight { get; set; }
        public List<FlightDetail> retFlight { get; set; }
        public List<List<FlightDetail>> multiFlight { get; set; }
        public City origin { get; set; }
        public City destination { get; set; }
        public int noOfAdults { get; set; }
        public int noOfChildren { get; set; }
        public int noOfInfants { get; set; }
        public string svc_class { get; set; }
        public decimal grandTotal { get; set; }
        public Refund refund { get; set; }
        public Reissue reissue { get; set; }
        public class Refund
        {
            public Guid FlightBookingRefundOID { get; set; }
            public Guid BookingOID { get; set; }
            public int status { get; set; }
            public string newPNR { get; set; }
            public string oldPNR { get; set; }
            public string refundNo { get; set; }
            public System.DateTime refundCreateDate { get; set; }
            public System.DateTime refundGMDate { get; set; }
            public System.DateTime dueDateOfRefund { get; set; }
            public string remark { get; set; }
        }

        public class Reissue
        {
            public Guid FlightBookingReissueOID { get; set; }
            public Guid BookingOID { get; set; }
            public int status { get; set; }
            public string newPNR { get; set; }
            public System.DateTime reissueCreateDate { get; set; }
            public int typeChage { get; set; }
            public int detailChage { get; set; }
            public string remark { get; set; }
        }
        
        public bool isPassportRequired { get; set; }

        public string PNR { get; set; }

        public string RobinhoodID { get; set; }
        public string bookingURN { get; set; }
        public List<string> promotionGroupCode { get; set; }
        public int statusPayment { get; set; }
        public int paymentMethod { get; set; }
        public System.DateTime paymentDate { get; set; }
        public int statusBooking { get; set; }
        public string paymentType { get; set; }
        public string paymentValue { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public string Platform { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public string Wallet_Address { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public decimal NFTFee { get; set; }

        public string bookingOID { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public int medId { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public bool priceChange { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public decimal oldPrice { get; set; }

        public RobinhoodPax.ContactInfo contactInfo { get; set; }
        public List<RobinhoodPax.PaxInfo> adtPaxs { get; set; }
        public List<RobinhoodPax.PaxInfo> chdPaxs { get; set; }
        public List<RobinhoodPax.PaxInfo> infPaxs { get; set; }

        [XmlIgnore]
        public bool isPricingWithSegment { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public bool isBundle { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public System.DateTime TKTL { get; set; }

        public System.DateTime bookingDate { get; set; }
        public List<string> remarks { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public string note { get; set; }
        //public string gogojiiID { get; set; }
        //public int statusPayment { get; set; }
        //public int statusBooking { get; set; }
        //public int paymentMethod { get; set; }
        //public bool isBundle { get; set; }
        public int sourceBy { get; set; }
        public decimal totalFare
        {
            get
            {
                decimal fare = 0;
                if (noOfAdults > 0)
                {
                    fare += noOfAdults * adtFare.net;
                }
                if (noOfChildren > 0)
                {
                    fare += noOfChildren * chdFare.net;
                }
                if (noOfInfants > 0)
                {
                    fare += noOfInfants * infFare.net;
                }
                return fare;
            }
        }

        public List<FareRule> fareRules { get; set; }

        public bool isError { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }

        public string uuid { get; set; }
        public string userID { get; set; }
        public Guid memberOID { get; set; }

        public string type { get; set; }//A=Amadeus,K=Kiwi

        public decimal installmentMonthlyPaid { get; set; }
        public int installmentPlan { get; set; }
        public decimal finalPrice { get; set; }

        public string promotionCode { get; set; }
        public string promotionName { get; set; }
        public decimal promotionDiscount { get; set; }

        public string corporateCode { get; set; }
        public bool isDomestic
        {
            get
            {
                bool isDom = true;
                string depCountry = "";
                string arrCountry = "";

                if (this.depFlight != null && this.depFlight.Count > 0)
                {
                    foreach (var f in this.depFlight)
                    {
                        depCountry = Utilities.NamingUtility.GetCountryCode(f.depCity.code.ToUpper());
                        arrCountry = Utilities.NamingUtility.GetCountryCode(f.arrCity.code.ToUpper());
                        isDom = (depCountry == "TH" && arrCountry == "TH");
                        if (!isDom)
                        {
                            break;
                        }
                    }
                }
                if (this.retFlight != null && this.retFlight.Count > 0)
                {
                    foreach (var f in this.retFlight)
                    {
                        depCountry = Utilities.NamingUtility.GetCountryCode(f.depCity.code.ToUpper());
                        arrCountry = Utilities.NamingUtility.GetCountryCode(f.arrCity.code.ToUpper());
                        isDom = (depCountry == "TH" && arrCountry == "TH");
                        if (!isDom)
                        {
                            break;
                        }
                    }
                }
                return isDom;
            }
        }

        public AirFare()
        {

        }

        public AirFare(NamingServices namingServices, string languageCode)
        {
            _namingServices = namingServices;
            _languageCode = languageCode;
        }

        public Fare FindFare(FareList fareList)
        {
            Fare fare = new Fare();
            decimal allFare = Convert.ToDecimal(fareList.FareDataInformation.FareDataSupInformation.FirstOrDefault(x => x.FareDataQualifier == "712").FareAmount);
            decimal tax = 0;
            if (fareList.TaxInformations.TaxInformation != null)
            {
                tax += Convert.ToDecimal(fareList.TaxInformations.TaxInformation.AmountDetails.FareDataMainInformation.FareAmount);
            }
            else
            {
                foreach (var taxInfo in fareList.TaxInformations.TaxInformationArray)
                {
                    tax += Convert.ToDecimal(taxInfo.AmountDetails.FareDataMainInformation.FareAmount);
                }
            }
            fare.tax = tax;

            //QTAX
            decimal qtax = 0;
            List<AttributeDetail> qTaxInfo = new List<AttributeDetail>();
            if (fareList.OtherPricingInfo.AttributeDetails.PurpleAttributeDetail != null)
            {
                var qt = fareList.OtherPricingInfo.AttributeDetails.PurpleAttributeDetail.AttributeType == "FCA" ? fareList.OtherPricingInfo.AttributeDetails.PurpleAttributeDetail : null;
                if (qt != null)
                {
                    qTaxInfo.Add(qt);
                }
            }
            else
            {
                qTaxInfo = fareList.OtherPricingInfo.AttributeDetails.AttributeDetailElementArray.Where(x => x.AttributeType == "FCA").ToList();
            }
            if (qTaxInfo != null)
            {
                foreach (var q in qTaxInfo)
                {
                    qtax += QTaxCalculation(q.AttributeDescription);
                }
            }

            fare.qtax = qtax;

            fare.baseFare = allFare - tax - qtax;

            //Markup
            fare.sellingBaseFare = fare.baseFare;
            return fare;
        }

        private decimal QTaxCalculation(string FareCalculationText)
        {
            decimal l_dROE = 0, l_dQTax = 0, l_dTotalQTax = 0;
            string sROE_p = @"ROE\d+\.\d+";
            string sQTax_p = @" Q\d+\.\d+";
            MatchCollection mc;
            Regex regex;

            regex = new Regex(sROE_p);
            Match m = regex.Match(FareCalculationText);
            if (m.Success)
            {
                l_dROE = Convert.ToDecimal(m.Value.TrimStart(new char[] { 'R', 'O', 'E' }));
            }//if

            regex = new Regex(sQTax_p);
            mc = regex.Matches(FareCalculationText);
            
            if (mc.Count > 0)
            {
                for (int i = 0; i < mc.Count; i++)
                {
                    l_dQTax = Convert.ToDecimal(mc[i].Value.Trim().TrimStart('Q'));
                    l_dTotalQTax += l_dQTax * l_dROE;
                }
            }
            sQTax_p = @"Q [A-Z]{6}\d+\.\d+";
            regex = new Regex(sQTax_p);
            mc = regex.Matches(FareCalculationText);
            if (mc.Count > 0)
            {
                int iL = 0;
                for (int i = 0; i < mc.Count; i++)
                {
                    iL = mc[i].Value.Trim().Replace(" ", "").Length;
                    l_dQTax = Convert.ToDecimal(mc[i].Value.Trim().Replace(" ", "").Substring(7, (iL - 7)));
                    l_dTotalQTax += l_dQTax * l_dROE;
                }//for
            }
            bool bCeiling = true;
            if (bCeiling == true)
            {
                /* »Ñ´àÈÉ
				 * ¶éÒ ÁÒ¡¡ÇèÒ 0 áµè¹éÍÂ¡ÇèÒ 5 ãËé»Ñ´à»ç¹ 5 
				 * ¶éÒ ÁÒ¡¡ÇèÒ 5 áµè¹éÍÂ¡ÇèÒ 10 ãËé»Ñ´à»ç¹ 10
				 */
                decimal dRound;
                regex = new Regex(@"\d\.\d+");  //µÑ´àÍÒµÑé§áµèËÅÑ¡Ë¹èÇÂä»¨¹ËÅÑ§·È¹ÔÂÁ
                m = regex.Match(l_dTotalQTax.ToString());
                if (m.Success)
                {
                    dRound = Convert.ToDecimal(m.Value);
                }
                else
                {
                    dRound = 0;
                }//if
                l_dTotalQTax = l_dTotalQTax - dRound;
                if (dRound > 0 && dRound <= 5)
                {
                    dRound = 5;
                }
                else if (dRound > 5 && dRound < 10)
                {
                    dRound = 10;
                }
                l_dTotalQTax += dRound;
            }
            else
            {
                //»Ñ´àÈÉà»ç¹¨Ó¹Ç¹àµçÁ à¾ÃÒÐ¡Ã³Õ¡ÒÃ·Ó FP áÅéÇºÒ§ Currency àªè¹ HKD, THB ¨ÐäÁèÃÍ§ÃÑº¨Ó¹Ç¹à§Ô¹·ÕèÁÕ¨Ø´·È¹ÔÂÁ
                l_dTotalQTax = Math.Ceiling(l_dTotalQTax);
            }//if

            return l_dTotalQTax;
        }//method

        public void SetDisplayFlight(BL.Entities.InformativePricing.Request airSellRequest)
        {
            noOfAdults = airSellRequest.noOfAdults;
            noOfChildren = airSellRequest.noOfChildren;
            noOfInfants = airSellRequest.noOfInfants;
            svc_class = airSellRequest.svc_class;
            if (airSellRequest.tripType == "M")
            {
                multiFlight = new List<List<FlightDetail>>();
                List<FlightDetail> flightDetails = new List<FlightDetail>();
                for (int i = 0; i < airSellRequest.multiFlights.Count; i++)
                {
                    flightDetails = GetFlights(airSellRequest.multiFlights[i], airSellRequest.multiTotalTime[i]);
                    multiFlight.Add(flightDetails);
                }
            }
            else
            {
                origin = new City(_namingServices, _languageCode);
                origin.code = airSellRequest.origin;
                destination = new City(_namingServices, _languageCode);
                destination.code = airSellRequest.destination;
                depFlight = GetFlights(airSellRequest.departureFlights, airSellRequest.depTotalTime);


                if (airSellRequest.tripType == "R")
                {
                    retFlight = GetFlights(airSellRequest.returnFlights, airSellRequest.retTotalTime);

                }
            }
            grandTotal = 0;
            int paxid = 1;
            if (noOfAdults > 0)
            {
                grandTotal += (adtFare.net * noOfAdults);

                adtPaxs = new List<RobinhoodPax.PaxInfo>();
                for (int i = 0; i < noOfAdults; i++)
                {
                    adtPaxs.Add(new RobinhoodPax.PaxInfo()
                    {
                        paxType = "ADT",
                        id = paxid
                    });
                    paxid++;
                }
            }
            if (noOfChildren > 0)
            {
                grandTotal += (chdFare.net * noOfChildren);
                chdPaxs = new List<RobinhoodPax.PaxInfo>();
                for (int i = 0; i < noOfChildren; i++)
                {
                    chdPaxs.Add(new RobinhoodPax.PaxInfo()
                    {
                        paxType = "CHD",
                        id = paxid
                    });
                    paxid++;
                }
            }
            if (noOfInfants > 0)
            {
                grandTotal += (infFare.net * noOfInfants);
                infPaxs = new List<RobinhoodPax.PaxInfo>();
                for (int i = 0; i < noOfInfants; i++)
                {
                    infPaxs.Add(new RobinhoodPax.PaxInfo()
                    {
                        paxType = "INF",
                        id = paxid,
                        travelWithAdultID = i+1
                    });
                    paxid++;
                }
            }
            //string arrCountry = _namingServices.GetCountryCode(destination.code);
            //this.isPassportRequired = arrCountry == "US";
            this.contactInfo = new RobinhoodPax.ContactInfo();
        }
        public void SetDisplayFlight(BL.Entities.AirSell.Request airSellRequest)
        {
            noOfAdults = airSellRequest.noOfAdults;
            noOfChildren = airSellRequest.noOfChildren;
            noOfInfants = airSellRequest.noOfInfants;
            svc_class = airSellRequest.svc_class;
            if (airSellRequest.tripType == "M")
            {
                multiFlight = new List<List<FlightDetail>>();
                List<FlightDetail> flightDetails = new List<FlightDetail>();
                for (int i = 0; i < airSellRequest.multiFlights.Count; i++)
                {
                    flightDetails = GetFlights(airSellRequest.multiFlights[i], airSellRequest.multiTotalTime[i]);
                    multiFlight.Add(flightDetails);
                }
            }
            else
            {
                origin = new City(_namingServices, _languageCode);
                origin.code = airSellRequest.origin;
                destination = new City(_namingServices, _languageCode);
                destination.code = airSellRequest.destination;
                depFlight = GetFlights(airSellRequest.departureFlights, airSellRequest.depTotalTime);

                if (airSellRequest.tripType == "R")
                {
                    retFlight = GetFlights(airSellRequest.returnFlights, airSellRequest.retTotalTime);

                }
            }

            grandTotal = 0;
            int iPax = 1;
            if (noOfAdults > 0)
            {
                grandTotal += (adtFare.net * noOfAdults);

                adtPaxs = new List<RobinhoodPax.PaxInfo>();
                for (int i = 0; i < noOfAdults; i++)
                {
                    adtPaxs.Add(new RobinhoodPax.PaxInfo()
                    {
                        paxType = "ADT",
                        id = iPax
                    });
                    iPax++;
                }
            }
            if (noOfChildren > 0)
            {
                grandTotal += (chdFare.net * noOfChildren);
                chdPaxs = new List<RobinhoodPax.PaxInfo>();
                for (int i = 0; i < noOfChildren; i++)
                {
                    chdPaxs.Add(new RobinhoodPax.PaxInfo()
                    {
                        paxType = "CHD",
                        id = iPax
                    });
                    iPax++;
                }
            }
            if (noOfInfants > 0)
            {
                grandTotal += (infFare.net * noOfInfants);
                infPaxs = new List<RobinhoodPax.PaxInfo>();
                for (int i = 0; i < noOfInfants; i++)
                {
                    infPaxs.Add(new RobinhoodPax.PaxInfo()
                    {
                        paxType = "INF",
                        id = iPax
                    });
                    iPax++;
                }
            }
        }

        private List<FlightDetail> GetFlights(List<InformativePricing.AirFlight> flights, string totalTime)
        {
            List<FlightDetail> flightDetails = new List<FlightDetail>();
            foreach (var flight in flights)
            {
                FlightDetail d = new FlightDetail();
                d.depCity = new Airport(_namingServices, true, _languageCode);
                d.depCity.code = flight.departureCity;
                d.arrCity = new Airport(_namingServices, true, _languageCode);
                d.arrCity.code = flight.arrivalCity;
                d.departureDateTime = flight.departureDateTime;
                d.arrivalDateTime = flight.arrivalDateTime;
                d.setDisplayDateTime(_languageCode, flights[0].departureDateTime);
                d.airline = new Airline(_namingServices, _languageCode);
                d.airline.code = flight.companyCode;
                d.operatedAirline = new Airline(_namingServices, _languageCode);
                d.operatedAirline.code = flight.operatedBy;
                d.flightNumber = flight.flightNumber;
                d.rbd = flight.RBDCode;
                d.flightTime = totalTime;
                d.equipmentType = flight._equibment;
                d.fareBasis = flight._fb;
                d.fareType = flight._ft;

                flightDetails.Add(d);
            }

            for (int i = 0; i < flightDetails.Count - 1; i++)
            {
                TimeSpan ts = flightDetails[i + 1].departureDateTime - flightDetails[i].arrivalDateTime;
                int hour = (ts.Days * 24) + ts.Hours;
                flightDetails[i].connectingTime = hour.ToString().PadLeft(2, '0') + ts.Minutes.ToString().PadLeft(2, '0');
            }

            return flightDetails;
        }
        private List<FlightDetail> GetFlights(List<AirSell.AirFlight> flights, string totalTime)
        {
            List<FlightDetail> flightDetails = new List<FlightDetail>();
            foreach (var flight in flights)
            {
                FlightDetail d = new FlightDetail();
                d.depCity = new Airport(_namingServices, true, _languageCode);
                d.depCity.code = flight.departureCity;
                d.arrCity = new Airport(_namingServices, true, _languageCode);
                d.arrCity.code = flight.arrivalCity;
                d.departureDateTime = flight.departureDateTime;
                d.arrivalDateTime = flight.arrivalDateTime;
                d.setDisplayDateTime(_languageCode, flights[0].departureDateTime);
                d.airline = new Airline(_namingServices, _languageCode);
                d.airline.code = flight.companyCode;
                d.operatedAirline = new Airline(_namingServices, _languageCode);
                d.operatedAirline.code = flight.operatedBy;
                d.flightNumber = flight.flightNumber;
                d.rbd = flight.RBDCode;
                d.flightTime = totalTime;

                flightDetails.Add(d);
            }
            return flightDetails;
        }

        public Fare FindFareFromInformative(PricingGroupLevelGroup fareList, InformativePricing.Request request, string paxType, FareMarkup markup, decimal paymentFee, bool isPaymentFeePct,bool bCheckQTax)
        {
            Fare fare = new Fare();
            decimal baseFare = 0;
            //Convert.ToDecimal(fareList.FareInfoGroup.FareAmount.MonetaryDetails.Amount);
            if (fareList.FareInfoGroup.FareAmount.MonetaryDetails.PurpleMonetaryDetails.Currency == "THB")
            {
                baseFare = Convert.ToDecimal(fareList.FareInfoGroup.FareAmount.MonetaryDetails.PurpleMonetaryDetails.Amount);
            }
            else
            {
                var baseEx = fareList.FareInfoGroup.FareAmount.OtherMonetaryDetails.MonetaryDetailsElementArray.FirstOrDefault(x => x.TypeQualifier == "E");
                baseFare = Convert.ToDecimal(baseEx.Amount);

            }
            decimal tax = 0;
            if (fareList.FareInfoGroup.SurchargesGroup.TaxesAmount != null)
            {
                if (fareList.FareInfoGroup.SurchargesGroup.TaxesAmount.TaxDetails.TaxDetail != null)
                {
                    tax += Convert.ToDecimal(fareList.FareInfoGroup.SurchargesGroup.TaxesAmount.TaxDetails.TaxDetail.Rate);
                }
                else
                {
                    foreach (var taxInfo in fareList.FareInfoGroup.SurchargesGroup.TaxesAmount.TaxDetails.TaxDetailArray)
                    {
                        tax += Convert.ToDecimal(taxInfo.Rate);
                    }
                }
            }
            fare.tax = tax;

            //QTAX
            decimal qtax = 0;
            string _qtax = "";
            var qTaxInfo = fareList.FareInfoGroup.TextData.Where(x => x.FreeTextQualification.InformationType == "15").ToList();
            if (qTaxInfo != null)
            {
                foreach (var info in qTaxInfo)
                {
                    if (String.IsNullOrEmpty(info.FreeText.String))
                    {
                        foreach (var ssText in info.FreeText.StringArray)
                        {
                            _qtax += ssText;
                        }
                    }
                    else
                    {
                        _qtax+= info.FreeText.String;
                    }
                }
                qtax = QTaxCalculation(_qtax);
            }

            fare.qtax = qtax;

            string textWarning = "";
            var textWarningInfo = fareList.FareInfoGroup.TextData.Where(x => x.FreeTextQualification.InformationType == "710").ToList();
            if (textWarningInfo != null)
            {
                textWarning = "";
                foreach (var info in textWarningInfo)
                {
                    if (String.IsNullOrEmpty(info.FreeText.String))
                    {
                        foreach (var ssText in info.FreeText.StringArray)
                        {
                            textWarning += ssText;
                        }
                    }
                    else
                    {
                        textWarning += info.FreeText.String;
                    }
                }
                if (textWarning.Length > 0)
                {
                    fare.warningCode = "710";
                    fare.warningMessage = new List<string>();
                    fare.warningMessage.Add(textWarning);
                }
            }


            fare.baseFare = baseFare - qtax;
            decimal totalFare = 0;
            decimal totalTax = 0;
            if (!bCheckQTax)
            {
                totalFare = fare.baseFare + fare.qtax;
                totalTax = fare.tax;
            }
            else
            {
                totalFare = fare.baseFare;
                totalTax = fare.tax + fare.qtax;
            }
            Log.Debug("totalFare=" + totalFare);
            Log.Debug("totalTax=" + totalTax);
            //Markup
            decimal lessFare = 0;
            string depCity = "";
            string depCountry = "";
            string arrCity = "";
            string arrCountry = "";
            if (request.multiFlights != null && request.multiFlights.Count > 0)
            {
                depCity = request.multiFlights[0][0].departureCity;
                depCountry = _namingServices.GetCountryCode(depCity);

                arrCity = request.multiFlights[0][request.multiFlights[0].Count - 1].arrivalCity;
                arrCountry = _namingServices.GetCountryCode(arrCity);
                fare.sellingBaseFare = markup.getFareCharge(totalFare,
                    request.multiFlights[0][0].companyCode, depCity, depCountry, arrCity, arrCountry,
                    request.multiFlights[0][0]._ft, request.multiFlights[0][0].RBDCode,
                    request.multiFlights[0][0]._fb, paxType, ref lessFare, request.userEmail, paymentFee, isPaymentFeePct, totalTax,"R");

            }
            else
            {
                depCity = request.departureFlights[0].departureCity;
                depCountry = _namingServices.GetCountryCode(depCity);

                arrCity = request.departureFlights[request.departureFlights.Count - 1].arrivalCity;
                arrCountry = _namingServices.GetCountryCode(arrCity);
                lessFare = 0;
                fare.sellingBaseFare = markup.getFareCharge(totalFare,
                    request.departureFlights[0].companyCode, depCity, depCountry, arrCity, arrCountry,
                    request.departureFlights[0]._ft, request.departureFlights[0].RBDCode,
                    request.departureFlights[0]._fb, paxType, ref lessFare, request.userEmail, paymentFee, isPaymentFeePct, totalTax, (request.returnFlights!=null && request.returnFlights.Count>0?"R":"O"));               
            }
            //add on 2022-09-02 by ning for can check commision(incom)
            Log.Debug("1.lessFare="+ lessFare);
            if (lessFare == 0)
            {
                lessFare = totalFare;
            }
            Log.Debug("2.lessFare=" + lessFare);
            fare.lessFare = lessFare;
            if (!bCheckQTax)
            {
                fare.lessFare = lessFare - fare.qtax;
            }
            if (!bCheckQTax)
            {
                fare.sellingBaseFare = fare.sellingBaseFare - fare.qtax;
            }
            else
            {
                fare.sellingBaseFare = fare.sellingBaseFare;
            }


            //Set Baggage
            fare.baggages = new List<Baggage>();
            if (fareList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup != null)
            {
                if (fareList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.BaggageAllowance != null)
                {
                    fare.baggages.Add(new Baggage()
                    {
                        baggageNo = fareList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.BaggageAllowance.BaggageDetails.FreeAllowance ?? "",
                        baggageUnit = fareList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.BaggageAllowance.BaggageDetails.QuantityCode
                    });
                }
            }
            else
            {
                if (fareList.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray != null)
                {
                    foreach (var segment in fareList.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray)
                    {
                        if (segment.BaggageAllowance != null)
                        {

                            fare.baggages.Add(new Baggage()
                            {
                                baggageNo = segment.BaggageAllowance.BaggageDetails.FreeAllowance ?? "",
                                baggageUnit = segment.BaggageAllowance.BaggageDetails.QuantityCode
                            });
                        }
                    }
                }
            }
            //
            return fare;
        }
        public Fare FindFareFromInformative(PricingGroupLevelGroup fareDepartList, PricingGroupLevelGroup fareReturnList, InformativePricing.Request request, string paxType, FareMarkup markup, decimal paymentFee, bool isPaymentFeePct, bool bCheckQTax)
        {
            Log.Debug("paxType:" + paxType);
            Fare fare = new Fare();
            decimal baseFare = 0;
            decimal baseFareReturn = 0;
            if (fareDepartList.FareInfoGroup.FareAmount.OtherMonetaryDetails.MonetaryDetailsElementArray != null)
            {
                baseFare = Convert.ToDecimal(fareDepartList.FareInfoGroup.FareAmount.OtherMonetaryDetails.MonetaryDetailsElementArray.FirstOrDefault(x => x.TypeQualifier == "E").Amount);
            }
            else
            {
                baseFare = Convert.ToDecimal((fareDepartList.FareInfoGroup.FareAmount.MonetaryDetails.MonetaryDetailsElementArray != null && fareDepartList.FareInfoGroup.FareAmount.MonetaryDetails.MonetaryDetailsElementArray.Count > 0) ? fareDepartList.FareInfoGroup.FareAmount.MonetaryDetails.MonetaryDetailsElementArray[0].Amount : fareDepartList.FareInfoGroup.FareAmount.MonetaryDetails.PurpleMonetaryDetails.Amount);
            }
            Log.Debug("Fare:" + baseFare);
            if (fareReturnList.FareInfoGroup.FareAmount.OtherMonetaryDetails.MonetaryDetailsElementArray != null)
            {
                baseFareReturn = Convert.ToDecimal(fareReturnList.FareInfoGroup.FareAmount.OtherMonetaryDetails.MonetaryDetailsElementArray.FirstOrDefault(x => x.TypeQualifier == "E").Amount);
            }
            else
            {
                baseFareReturn = Convert.ToDecimal((fareReturnList.FareInfoGroup.FareAmount.MonetaryDetails.MonetaryDetailsElementArray != null && fareReturnList.FareInfoGroup.FareAmount.MonetaryDetails.MonetaryDetailsElementArray.Count > 0) ? fareReturnList.FareInfoGroup.FareAmount.MonetaryDetails.MonetaryDetailsElementArray[0].Amount : fareReturnList.FareInfoGroup.FareAmount.MonetaryDetails.PurpleMonetaryDetails.Amount);
            }
            Log.Debug("FareReturn:" + baseFareReturn);
            //decimal baseFare = Convert.ToDecimal((fareList.FareInfoGroup.FareAmount.MonetaryDetails.MonetaryDetailArray != null && fareList.FareInfoGroup.FareAmount.MonetaryDetails.MonetaryDetailArray.Count > 0) ? fareList.FareInfoGroup.FareAmount.MonetaryDetails.MonetaryDetailArray[0].Amount : fareList.FareInfoGroup.FareAmount.MonetaryDetails.MonetaryDetail.Amount);
            decimal tax = 0;
            decimal taxReturn = 0;
            if (fareDepartList.FareInfoGroup.SurchargesGroup.TaxesAmount != null)
            {
                if (fareDepartList.FareInfoGroup.SurchargesGroup.TaxesAmount.TaxDetails.TaxDetail != null)
                {
                    tax += Convert.ToDecimal(fareDepartList.FareInfoGroup.SurchargesGroup.TaxesAmount.TaxDetails.TaxDetail.Rate);
                }
                else
                {
                    foreach (var taxInfo in fareDepartList.FareInfoGroup.SurchargesGroup.TaxesAmount.TaxDetails.TaxDetailArray)
                    {
                        tax += Convert.ToDecimal(taxInfo.Rate);
                    }
                }
            }
            Log.Debug("tax:" + tax);
            if (fareReturnList.FareInfoGroup.SurchargesGroup.TaxesAmount != null)
            {
                if (fareReturnList.FareInfoGroup.SurchargesGroup.TaxesAmount.TaxDetails.TaxDetail != null)
                {
                    taxReturn += Convert.ToDecimal(fareReturnList.FareInfoGroup.SurchargesGroup.TaxesAmount.TaxDetails.TaxDetail.Rate);
                }
                else
                {
                    foreach (var taxInfo in fareReturnList.FareInfoGroup.SurchargesGroup.TaxesAmount.TaxDetails.TaxDetailArray)
                    {
                        taxReturn += Convert.ToDecimal(taxInfo.Rate);
                    }
                }
            }
            Log.Debug("taxReturn:" + taxReturn);

            //QTAX
            decimal qtax = 0;
            decimal qtaxReturn = 0;
            var qTaxInfo = fareDepartList.FareInfoGroup.TextData.Where(x => x.FreeTextQualification.InformationType == "15").ToList();
            if (qTaxInfo != null)
            {
                string textTaxInfo = "";
                foreach (var arr in qTaxInfo)
                {
                    if (String.IsNullOrEmpty(arr.FreeText.String))
                    {
                        foreach (var inArr in arr.FreeText.StringArray)
                        {
                            textTaxInfo += inArr;
                        }
                    }
                    else
                    {
                        textTaxInfo += arr.FreeText.String;
                    }
                }
                qtax = QTaxCalculation(textTaxInfo);
            }
            string textWarningInfo = "";
            var textWarning = fareDepartList.FareInfoGroup.TextData.Where(x => x.FreeTextQualification.InformationType == "710").ToList();
            if (textWarning != null)
            {
                textWarningInfo = "";
                foreach (var arr in textWarning)
                {
                    if (String.IsNullOrEmpty(arr.FreeText.String))
                    {
                        foreach (var inArr in arr.FreeText.StringArray)
                        {
                            textWarningInfo += inArr;
                        }
                    }
                    else
                    {
                        textWarningInfo += arr.FreeText.String;
                    }
                }
            }

            Log.Debug("qtax:" + qtax);
            qTaxInfo = fareReturnList.FareInfoGroup.TextData.Where(x => x.FreeTextQualification.InformationType == "15").ToList();
            if (qTaxInfo != null)
            {
                string textTaxInfo = "";
                foreach (var arr in qTaxInfo)
                {
                    if (String.IsNullOrEmpty(arr.FreeText.String))
                    {
                        foreach (var inArr in arr.FreeText.StringArray)
                        {
                            textTaxInfo += inArr;
                        }
                    }
                    else
                    {
                        textTaxInfo += arr.FreeText.String;
                    }
                }
                qtaxReturn = QTaxCalculation(textTaxInfo);
            }
            Log.Debug("qtaxReturn:" + qtaxReturn);

            string textWarningInfoReturn = "";
            var textWarningReturn = fareReturnList.FareInfoGroup.TextData.Where(x => x.FreeTextQualification.InformationType == "710").ToList();
            if (textWarningReturn != null)
            {
                textWarningInfoReturn = "";
                foreach (var arr in textWarningReturn)
                {
                    if (String.IsNullOrEmpty(arr.FreeText.String))
                    {
                        foreach (var inArr in arr.FreeText.StringArray)
                        {
                            textWarningInfoReturn += inArr;
                        }
                    }
                    else
                    {
                        textWarningInfoReturn += arr.FreeText.String;
                    }
                }
            }

            Log.Debug("bCheckQTax:" + bCheckQTax);
            decimal totalFare = 0, totalFareReturn = 0;
            decimal totalTax = 0, totalTaxReturn = 0;
            baseFare = baseFare - qtax;
            baseFareReturn = baseFareReturn - qtaxReturn;
            Log.Debug("baseFare:" + baseFare);
            Log.Debug("baseFareReturn:" + baseFareReturn);
            if (!bCheckQTax)
            {
                totalFare = baseFare + qtax;
                totalTax = tax;

                totalFareReturn = baseFareReturn + qtaxReturn;
                totalTaxReturn = taxReturn;
            }
            else
            {
                totalFare = baseFare;
                totalTax = tax + qtax;

                totalFareReturn = baseFareReturn;
                totalTaxReturn = taxReturn + qtaxReturn;
            }
            //Markup
            string depCity = request.departureFlights[0].departureCity;
            string depCountry = _namingServices.GetCountryCode(depCity);
            string arrCity =  request.departureFlights[request.departureFlights.Count - 1].arrivalCity;
            string arrCountry = _namingServices.GetCountryCode(arrCity);
            decimal lessFare = 0, lessFareReturn = 0;
            decimal sellingBaseFare = 0, sellingBaseFareReturn = 0;
            string FareType = request.departureFlights[0]._ft;
            if (depCountry == arrCountry && (request.departureFlights[0].companyCode == "TG") && FareType == "RV")//case Dom TG Type V
            {
                FareType = "A";
            }
            else
            {
                switch (FareType)
                {
                    case "RP": FareType = "P"; break;
                    case "RU": FareType = "N"; break;
                    case "RA": FareType = "A"; break;
                    default: FareType = "N"; break;
                }
            }
            
            sellingBaseFare = markup.getFareCharge(totalFare,
                    request.departureFlights[0].companyCode, depCity, depCountry, arrCity, arrCountry,
                    FareType, request.departureFlights[0].RBDCode,
                    request.departureFlights[0]._fb, paxType, ref lessFare, request.userEmail, paymentFee, isPaymentFeePct, totalTax,"O");
            Log.Debug("sellingBaseFare:" + sellingBaseFare);
            Log.Debug("lessFare:" + lessFare);
            fare.depPrice = new onewayPrice();
            if (textWarningInfo.Length > 0)
            {
                fare.depPrice.warningCode = "710";
                fare.depPrice.warningMessage = new List<string>();
                fare.depPrice.warningMessage.Add(textWarningInfo);
            }
            fare.depPrice.baseFare = baseFare;
            fare.depPrice.tax = tax;
            fare.depPrice.qtax = qtax;
            if (!bCheckQTax)
            {
                fare.depPrice.sellingBaseFare = (sellingBaseFare - qtax);
            }
            else
            {
                fare.depPrice.sellingBaseFare = sellingBaseFare;
            }
            //add on 2022-09-02 by ning for can check commision(incom)
            if (lessFare == 0)
            {
                lessFare = totalFare;
            }
            fare.depPrice.lessFare = lessFare;

            string FareTypeReturn = request.returnFlights[0]._ft;
            if (depCountry == arrCountry && (request.departureFlights[0].companyCode == "TG") && FareTypeReturn == "RV")//case Dom TG Type V
            {
                FareTypeReturn = "A";
            }
            else
            {
                switch (FareTypeReturn)
                {
                    case "RP": FareTypeReturn = "P"; break;
                    case "RU": FareTypeReturn = "N"; break;
                    case "RA": FareTypeReturn = "A"; break;
                    default: FareTypeReturn = "N"; break;
                }
            }

            depCity = request.returnFlights[0].departureCity;
            depCountry = _namingServices.GetCountryCode(depCity);
            arrCity = request.returnFlights[request.returnFlights.Count - 1].arrivalCity;
            arrCountry = _namingServices.GetCountryCode(arrCity);
            sellingBaseFareReturn = markup.getFareCharge(totalFareReturn,
                   request.returnFlights[0].companyCode, depCity, depCountry, arrCity, arrCountry,
                   FareTypeReturn, request.returnFlights[0].RBDCode,
                   request.departureFlights[0]._fb, paxType, ref lessFareReturn, request.userEmail, paymentFee, isPaymentFeePct, totalTaxReturn, "O");
            Log.Debug("sellingBaseFareReturn:" + sellingBaseFareReturn);
            Log.Debug("lessFareReturn:" + lessFareReturn);

            fare.retPrice = new onewayPrice();
            if (textWarningInfoReturn.Length > 0)
            {
                fare.retPrice.warningCode = "710";
                fare.retPrice.warningMessage = new List<string>();
                fare.retPrice.warningMessage.Add(textWarningInfoReturn);
            }
            fare.retPrice.baseFare = baseFareReturn;
            fare.retPrice.tax = taxReturn;
            fare.retPrice.qtax = qtaxReturn;
            if (!bCheckQTax)
            {
                fare.retPrice.sellingBaseFare = (sellingBaseFareReturn - qtaxReturn);
            }
            else
            {
                fare.retPrice.sellingBaseFare = sellingBaseFareReturn;
            }
            //add on 2022-09-02 by ning for can check commision(incom)
            if (lessFareReturn == 0)
            {
                lessFareReturn = totalFareReturn;
            }
            fare.retPrice.lessFare = lessFareReturn;

            fare.tax = tax + taxReturn;
            Log.Debug("fare.tax:" + fare.tax);

            fare.qtax = qtax + qtaxReturn;
            Log.Debug("fare.qtax:" + fare.qtax);

            fare.baseFare = fare.depPrice.baseFare + fare.retPrice.baseFare;
            Log.Debug("fare.baseFare:" + fare.baseFare);
            if (!bCheckQTax)
            {
                fare.sellingBaseFare = (sellingBaseFare - qtax) + (sellingBaseFareReturn - qtaxReturn);
            }
            else
            {
                fare.sellingBaseFare = sellingBaseFare + sellingBaseFareReturn;
            }
            Log.Debug("fare.sellingBaseFare:" + fare.sellingBaseFare);
            if (!bCheckQTax)            
            {
                lessFare = lessFare - qtax;
                lessFareReturn = lessFareReturn - qtaxReturn;
            }

            fare.lessFare = lessFare + lessFareReturn;
            Log.Debug("fare.lessFare:" + fare.lessFare);
            //Set Baggage
            fare.baggages = new List<Baggage>();
            int iSegment = 1;
            if (fareDepartList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup != null)
            {
                if (fareDepartList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.BaggageAllowance != null)
                {
                    fare.baggages.Add(new Baggage()
                    {
                        segment = iSegment,
                        baggageNo = fareDepartList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.BaggageAllowance.BaggageDetails.FreeAllowance ?? "",
                        baggageUnit = fareDepartList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.BaggageAllowance.BaggageDetails.QuantityCode
                    });

                }
            }
            else
            {
                if (fareDepartList.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray != null)
                {
                    foreach (var segment in fareDepartList.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray)
                    {
                        if (segment.BaggageAllowance != null)
                        {

                            fare.baggages.Add(new Baggage()
                            {
                                segment = iSegment,
                                baggageNo = segment.BaggageAllowance.BaggageDetails.FreeAllowance ?? "",
                                baggageUnit = segment.BaggageAllowance.BaggageDetails.QuantityCode
                            });
                            iSegment++;
                        }
                    }
                }
            }
            if (fareReturnList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup != null)
            {
                if (fareReturnList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.BaggageAllowance != null)
                {
                    fare.baggages.Add(new Baggage()
                    {
                        segment = iSegment,
                        baggageNo = fareReturnList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.BaggageAllowance.BaggageDetails.FreeAllowance ?? "",
                        baggageUnit = fareReturnList.FareInfoGroup.SegmentLevelGroup.PurpleSegmentLevelGroup.BaggageAllowance.BaggageDetails.QuantityCode
                    });
                }
            }
            else
            {
                if (fareReturnList.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray != null)
                {
                    foreach (var segment in fareReturnList.FareInfoGroup.SegmentLevelGroup.SegmentLevelGroupElementArray)
                    {
                        if (segment.BaggageAllowance != null)
                        {

                            fare.baggages.Add(new Baggage()
                            {
                                segment = iSegment,
                                baggageNo = segment.BaggageAllowance.BaggageDetails.FreeAllowance ?? "",
                                baggageUnit = segment.BaggageAllowance.BaggageDetails.QuantityCode
                            });
                            iSegment++;
                        }
                    }
                }
            }
            //
            return fare;
        }

        public AirSell.Request GetAirSellRequest()
        {
            AirSell.Request request = new AirSell.Request();
            request.bookingOID = this.bookingOID;
            request.noOfAdults = this.noOfAdults;
            request.noOfChildren = this.noOfChildren;
            request.noOfInfants = this.noOfInfants;
            if (this.multiFlight != null && this.multiFlight.Count > 0)
            {
                request.multiFlights = new List<List<AirSell.AirFlight>>();
                List<AirSell.AirFlight> airFlights = new List<AirSell.AirFlight>();
                AirSell.AirFlight flight = new AirSell.AirFlight();
                for (int i = 0; i < this.multiFlight.Count; i++)
                {
                    airFlights = new List<AirSell.AirFlight>();
                    for (int j = 0; j < this.multiFlight[i].Count; j++)
                    {
                        flight = new AirSell.AirFlight();
                        flight.departureDateTime = this.multiFlight[i][j].departureDateTime;
                        flight.arrivalDateTime = this.multiFlight[i][j].arrivalDateTime;
                        flight.departureCity = this.multiFlight[i][j].depCity.code;
                        flight.arrivalCity = this.multiFlight[i][j].arrCity.code;
                        flight.companyCode = this.multiFlight[i][j].airline.code;
                        flight.flightNumber = this.multiFlight[i][j].flightNumber;
                        flight.RBDCode = this.multiFlight[i][j].rbd;

                        airFlights.Add(flight);
                    }
                    request.multiFlights.Add(airFlights);
                }
            }
            else
            {
                request.departureFlights = new List<AirSell.AirFlight>();
                for (int i = 0; i < this.depFlight.Count; i++)
                {
                    var flight = new AirSell.AirFlight();

                    flight.departureDateTime = this.depFlight[i].departureDateTime;
                    flight.arrivalDateTime = this.depFlight[i].arrivalDateTime;
                    flight.departureCity = this.depFlight[i].depCity.code;
                    flight.arrivalCity = this.depFlight[i].arrCity.code;
                    flight.companyCode = this.depFlight[i].airline.code;
                    flight.flightNumber = this.depFlight[i].flightNumber;
                    flight.RBDCode = this.depFlight[i].rbd;

                    if (i > 0 && request.departureFlights[i - 1].companyCode == flight.companyCode && request.departureFlights[i - 1].flightNumber == flight.flightNumber)
                    {
                        request.departureFlights[i - 1].arrivalDateTime = flight.arrivalDateTime;
                        request.departureFlights[i - 1].arrivalCity = flight.arrivalCity;
                    }
                    else
                    {
                        request.departureFlights.Add(flight);
                    }
                }

                if (this.retFlight != null && this.retFlight.Count > 0)
                {
                    request.returnFlights = new List<AirSell.AirFlight>();

                    for (int i = 0; i < this.retFlight.Count; i++)
                    {
                        var flight = new AirSell.AirFlight();

                        flight.departureDateTime = this.retFlight[i].departureDateTime;
                        flight.arrivalDateTime = this.retFlight[i].arrivalDateTime;
                        flight.departureCity = this.retFlight[i].depCity.code;
                        flight.arrivalCity = this.retFlight[i].arrCity.code;
                        flight.companyCode = this.retFlight[i].airline.code;
                        flight.flightNumber = this.retFlight[i].flightNumber;
                        flight.RBDCode = this.retFlight[i].rbd;

                        if (i > 0 && request.returnFlights[i - 1].companyCode == flight.companyCode && request.returnFlights[i - 1].flightNumber == flight.flightNumber)
                        {
                            request.returnFlights[i - 1].arrivalDateTime = flight.arrivalDateTime;
                            request.returnFlights[i - 1].arrivalCity = flight.arrivalCity;
                        }
                        else
                        {
                            request.returnFlights.Add(flight);
                        }
                    }
                }
            }
            request.session = new AirSell.Session();
            request.session.isStateFull = true;
            request.session.InSeries = false;
            request.session.End = false;
            request.session.SessionId = "";
            request.session.SequenceNumber = "";
            request.session.SecurityToken = "";

            request.useBookingOfficeID = false;
            request.tripType = (this.multiFlight != null && this.multiFlight.Count > 0) ? "M" : ((this.retFlight != null && this.retFlight.Count > 0) ? "R" : "D");
            request.ClientIP = Utilities.HttpUtility.GetIPAddress();
            return request;
        }

        public AirSell.Request GetAirSellDepartRequest()
        {
            AirSell.Request request = new AirSell.Request();
            request.bookingOID = this.bookingOID;
            request.noOfAdults = this.noOfAdults;
            request.noOfChildren = this.noOfChildren;
            request.noOfInfants = this.noOfInfants;
            request.departureFlights = new List<AirSell.AirFlight>();
            for (int i = 0; i < this.depFlight.Count; i++)
            {
                var flight = new AirSell.AirFlight();

                flight.departureDateTime = this.depFlight[i].departureDateTime;
                flight.arrivalDateTime = this.depFlight[i].arrivalDateTime;
                flight.departureCity = this.depFlight[i].depCity.code;
                flight.arrivalCity = this.depFlight[i].arrCity.code;
                flight.companyCode = this.depFlight[i].airline.code;
                flight.flightNumber = this.depFlight[i].flightNumber;
                flight.RBDCode = this.depFlight[i].rbd;

                if (i > 0 && request.departureFlights[i - 1].companyCode == flight.companyCode && request.departureFlights[i - 1].flightNumber == flight.flightNumber)
                {
                    request.departureFlights[i - 1].arrivalDateTime = flight.arrivalDateTime;
                    request.departureFlights[i - 1].arrivalCity = flight.arrivalCity;
                }
                else
                {
                    request.departureFlights.Add(flight);
                }
            }//for
            request.session = new AirSell.Session();
            request.session.isStateFull = true;
            request.session.InSeries = false;
            request.session.End = false;
            request.session.SessionId = "";
            request.session.SequenceNumber = "";
            request.session.SecurityToken = "";

            request.useBookingOfficeID = false;
            request.tripType = "D";
            request.ClientIP = Utilities.HttpUtility.GetIPAddress();
            return request;

        }
        public AirSell.Request GetAirSellReturnRequest()
        {
            AirSell.Request request = new AirSell.Request();
            request.bookingOID = this.bookingOID;
            request.noOfAdults = this.noOfAdults;
            request.noOfChildren = this.noOfChildren;
            request.noOfInfants = this.noOfInfants;
            request.departureFlights = new List<AirSell.AirFlight>();
            for (int i = 0; i < this.retFlight.Count; i++)
            {
                var flight = new AirSell.AirFlight();

                flight.departureDateTime = this.retFlight[i].departureDateTime;
                flight.arrivalDateTime = this.retFlight[i].arrivalDateTime;
                flight.departureCity = this.retFlight[i].depCity.code;
                flight.arrivalCity = this.retFlight[i].arrCity.code;
                flight.companyCode = this.retFlight[i].airline.code;
                flight.flightNumber = this.retFlight[i].flightNumber;
                flight.RBDCode = this.retFlight[i].rbd;

                if (i > 0 && request.departureFlights[i - 1].companyCode == flight.companyCode && request.departureFlights[i - 1].flightNumber == flight.flightNumber)
                {
                    request.departureFlights[i - 1].arrivalDateTime = flight.arrivalDateTime;
                    request.departureFlights[i - 1].arrivalCity = flight.arrivalCity;
                }
                else
                {
                    request.departureFlights.Add(flight);
                }
            }
            request.session = new AirSell.Session();
            request.session.isStateFull = true;
            request.session.InSeries = false;
            request.session.End = false;
            request.session.SessionId = "";
            request.session.SequenceNumber = "";
            request.session.SecurityToken = "";

            request.useBookingOfficeID = false;
            request.tripType = "D";
            request.ClientIP = Utilities.HttpUtility.GetIPAddress();
            return request;

        }
    }
}
