using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Entities.Pricing;
using System.Text.RegularExpressions;
using BL.Entities.GogojiiFlight;
using BL.Entities.AirSell;
using BL.Entities.InformativePricing;

namespace BL.Entities.GogojiiFare
{
    public class AirFare
    {
        //public BL.Entities.AirSell.Request airSellRequest { get; set; }
        private NamingServices _namingServices { get; set; }
        private string _languageCode { get; set; }
        public Fare adtFare { get; set; }
        public Fare chdFare { get; set; }
        public Fare infFare { get; set; }
        public List<FlightDetail> depFlight { get; set; }
        public List<FlightDetail> retFlight { get; set; }
        public City origin { get; set; }
        public City destination { get; set; }
        public int noOfAdults { get; set; }
        public int noOfChildren { get; set; }
        public int noOfInfants { get; set; }
        public string svc_class { get; set; }
        public decimal grandTotal { get; set; }

        public string bookingOID { get; set; }

        public bool priceChange { get; set; }
        public decimal oldPrice { get; set; }

        public List<GogojiiPax.PaxInfo> adtPaxs { get; set; }
        public List<GogojiiPax.PaxInfo> chdPaxs { get; set; }
        public List<GogojiiPax.PaxInfo> infPaxs { get; set; }

        public System.DateTime TKTL { get; set; }

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

        public decimal creditCardFee
        {
            get
            {
                return 0.0325M * this.totalFare;
            }
        }

        public decimal paypalFee
        {
            get
            {
                return 0.028M * this.totalFare;
            }
        }

        public decimal counterServiceFee
        {
            get
            {
                return 0.015M * this.totalFare;
            }
        }

        public List<FareRule> fareRules { get; set; }

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
            var qTaxInfo = fareList.OtherPricingInfo.AttributeDetails.FirstOrDefault(x => x.AttributeType == "FCA");
            if (qTaxInfo != null)
            {
                qtax = QTaxCalculation(qTaxInfo.AttributeDescription);
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
            for (int i = 0; i < mc.Count; i++)
            {
                l_dQTax = Convert.ToDecimal(mc[i].Value.Trim().TrimStart('Q'));
                l_dTotalQTax += l_dQTax * l_dROE;
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
            origin = new City(_namingServices, _languageCode);
            origin.code = airSellRequest.origin;
            destination = new City(_namingServices, _languageCode);
            destination.code = airSellRequest.destination;
            depFlight = GetFlights(airSellRequest.departureFlights, airSellRequest.depTotalTime);


            if (airSellRequest.tripType == "R")
            {
                retFlight = GetFlights(airSellRequest.returnFlights, airSellRequest.retTotalTime);

            }

            grandTotal = 0;
            if (noOfAdults > 0)
            {
                grandTotal += (adtFare.net * noOfAdults);

                adtPaxs = new List<GogojiiPax.PaxInfo>();
                for (int i = 0; i < noOfAdults; i++)
                {
                    adtPaxs.Add(new GogojiiPax.PaxInfo()
                    {
                        paxType = "ADT"
                    });
                }
            }
            if (noOfChildren > 0)
            {
                grandTotal += (chdFare.net * noOfChildren);
                chdPaxs = new List<GogojiiPax.PaxInfo>();
                for (int i = 0; i < noOfChildren; i++)
                {
                    chdPaxs.Add(new GogojiiPax.PaxInfo()
                    {
                        paxType = "CHD"
                    });
                }
            }
            if (noOfInfants > 0)
            {
                grandTotal += (infFare.net * noOfInfants);
                infPaxs = new List<GogojiiPax.PaxInfo>();
                for (int i = 0; i < noOfInfants; i++)
                {
                    infPaxs.Add(new GogojiiPax.PaxInfo()
                    {
                        paxType = "INF"
                    });
                }
            }
        }
        public void SetDisplayFlight(BL.Entities.AirSell.Request airSellRequest)
        {
            noOfAdults = airSellRequest.noOfAdults;
            noOfChildren = airSellRequest.noOfChildren;
            noOfInfants = airSellRequest.noOfInfants;
            svc_class = airSellRequest.svc_class;
            origin = new City(_namingServices, _languageCode);
            origin.code = airSellRequest.origin;
            destination = new City(_namingServices, _languageCode);
            destination.code = airSellRequest.destination;
            depFlight = GetFlights(airSellRequest.departureFlights, airSellRequest.depTotalTime);

            if (airSellRequest.tripType == "R")
            {
                retFlight = GetFlights(airSellRequest.returnFlights, airSellRequest.retTotalTime);

            }

            grandTotal = 0;
            if (noOfAdults > 0)
            {
                grandTotal += (adtFare.net * noOfAdults);

                adtPaxs = new List<GogojiiPax.PaxInfo>();
                for (int i = 0; i < noOfAdults; i++)
                {
                    adtPaxs.Add(new GogojiiPax.PaxInfo()
                    {
                        paxType = "ADT"
                    });
                }
            }
            if (noOfChildren > 0)
            {
                grandTotal += (chdFare.net * noOfChildren);
                chdPaxs = new List<GogojiiPax.PaxInfo>();
                for (int i = 0; i < noOfChildren; i++)
                {
                    chdPaxs.Add(new GogojiiPax.PaxInfo()
                    {
                        paxType = "CHD"
                    });
                }
            }
            if (noOfInfants > 0)
            {
                grandTotal += (infFare.net * noOfInfants);
                infPaxs = new List<GogojiiPax.PaxInfo>();
                for (int i = 0; i < noOfInfants; i++)
                {
                    infPaxs.Add(new GogojiiPax.PaxInfo()
                    {
                        paxType = "INF"
                    });
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

        public Fare FindFareFromInformative(PricingGroupLevelGroup fareList, InformativePricing.Request request, string paxType)
        {
            FareMarkup markup = new FareMarkup();
            Fare fare = new Fare();
            decimal baseFare = Convert.ToDecimal(fareList.FareInfoGroup.FareAmount.MonetaryDetails.Amount);
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
            var qTaxInfo = fareList.FareInfoGroup.TextData.FirstOrDefault(x => x.FreeTextQualification.InformationType == "15");
            if (qTaxInfo != null)
            {
                qtax = QTaxCalculation(String.IsNullOrEmpty(qTaxInfo.FreeText.String) ? qTaxInfo.FreeText.StringArray[0] : qTaxInfo.FreeText.String);
            }

            fare.qtax = qtax;

            fare.baseFare = baseFare - qtax;

            //Markup
            string depCity = request.departureFlights[0].departureCity;
            string depCountry = _namingServices.GetCountryCode(depCity);

            string arrCity = request.departureFlights[request.departureFlights.Count - 1].arrivalCity;
            string arrCountry = _namingServices.GetCountryCode(arrCity);
            decimal lessFare = 0;
            fare.sellingBaseFare = markup.getFareCharge(fare.baseFare,
                request.departureFlights[0].companyCode, depCity, depCountry, arrCity, arrCountry,
                request.departureFlights[0]._ft, request.departureFlights[0].RBDCode,
                request.departureFlights[0]._fb, paxType, ref lessFare);
            fare.lessFare = lessFare;

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

        public AirSell.Request GetAirSellRequest()
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

            request.session = new AirSell.Session();
            request.session.isStateFull = true;
            request.session.InSeries = false;
            request.session.End = false;
            request.session.SessionId = "";
            request.session.SequenceNumber = "";
            request.session.SecurityToken = "";

            request.useBookingOfficeID = false;
            request.tripType = (this.retFlight != null && this.retFlight.Count > 0) ? "R" : "D";
            request.ClientIP = Utilities.HttpUtility.GetIPAddress();
            return request;
        }

    }
}
