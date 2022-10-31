using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGBookingAPI.Models
{
    public class SelectedFlightsModel
    {
        public string originCode { get; set; }
        public string destinationCode { get; set; }
        public int adult { get; set; }
        public int child { get; set; }
        public int infant { get; set; }
        public string svcClass { get; set; }
        public string tripType { get; set; }
        public string S1 { get; set; }
        public string S2 { get; set; }
        public string S3 { get; set; }
        public string S4 { get; set; }
        public string S5 { get; set; }
        public string S6 { get; set; }
        public string price { get; set; }
        public string languageCode { get; set; }

        public BL.Entities.InformativePricing.Request GetInformativePricingRequestFor1A()
        {
            BL.Entities.InformativePricing.Request request = new BL.Entities.InformativePricing.Request();
            request.origin = originCode;
            request.destination = destinationCode;
            request.svc_class = svcClass;
            request.noOfAdults = adult;
            request.noOfChildren = child;
            request.noOfInfants = infant;
            request.departureFlights = null;
            request.returnFlights = null;
            request.multiFlights = null;
            bool bHaveRU = false;
            bool bHaveRP = false;

            if (this.tripType != "M")
            {
                request.departureFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                string[] depFlights = S1.Split('|');
                for (int i = 0; i < depFlights.Length - 2; i++)
                {
                    BL.Entities.InformativePricing.AirFlight flight = new BL.Entities.InformativePricing.AirFlight();
                    flight.departureCity = depFlights[i].Substring(0, 3);
                    flight.arrivalCity = depFlights[i].Substring(3, 3);
                    flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.RBDCode = depFlights[i].Substring(30, 1);
                    flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                    flight._ft = depFlights[i].Substring(43, 2);
                    if (flight._ft == "RP")
                    {
                        bHaveRP = true;
                    }
                    else
                    {
                        bHaveRU = true;
                    }
                    flight.companyCode = depFlights[i].Substring(45, 2);
                    flight.operatedBy = depFlights[i].Substring(47, 2);
                    flight._equibment = depFlights[i].Substring(49, 3);
                    flight.flightNumber = depFlights[i].Substring(52);
                    if (i > 0 && flight.companyCode == request.departureFlights[i - 1].companyCode && flight.flightNumber == request.departureFlights[i - 1].flightNumber)
                    {
                        request.departureFlights[i - 1].arrivalCity = flight.arrivalCity;
                        request.departureFlights[i - 1].arrivalDateTime = flight.arrivalDateTime;
                    }
                    else
                    {
                        request.departureFlights.Add(flight);
                    }
                }
                request.depTotalTime = depFlights[depFlights.Length - 2];

                if (this.tripType == "R")
                {
                    request.returnFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                    string[] retFlights = S2.Split('|');
                    for (int i = 0; i < retFlights.Length - 2; i++)
                    {
                        BL.Entities.InformativePricing.AirFlight flight = new BL.Entities.InformativePricing.AirFlight();
                        flight.departureCity = retFlights[i].Substring(0, 3);
                        flight.arrivalCity = retFlights[i].Substring(3, 3);
                        flight.departureDateTime = DateTime.ParseExact(retFlights[i].Substring(6, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.arrivalDateTime = DateTime.ParseExact(retFlights[i].Substring(18, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.RBDCode = retFlights[i].Substring(30, 1);
                        flight._fb = retFlights[i].Substring(31, 12).Replace(".", "");
                        flight._ft = retFlights[i].Substring(43, 2);
                        if (flight._ft == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                        flight.companyCode = retFlights[i].Substring(45, 2);
                        flight.operatedBy = retFlights[i].Substring(47, 2);
                        flight._equibment = retFlights[i].Substring(49, 3);
                        flight.flightNumber = retFlights[i].Substring(52);
                        if (i > 0 && flight.companyCode == request.returnFlights[i - 1].companyCode && flight.flightNumber == request.returnFlights[i - 1].flightNumber)
                        {
                            request.returnFlights[i - 1].arrivalCity = flight.arrivalCity;
                            request.returnFlights[i - 1].arrivalDateTime = flight.arrivalDateTime;
                        }
                        else
                        {
                            request.returnFlights.Add(flight);
                        }
                    }
                    request.retTotalTime = retFlights[retFlights.Length - 2];
                }
            }
            else//Multi Destination
            {
                request.multiTotalTime = new List<string>();
                request.multiFlights = new List<List<BL.Entities.InformativePricing.AirFlight>>();
                string[] depFlights = S1.Split('|');
                List<BL.Entities.InformativePricing.AirFlight> mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                BL.Entities.InformativePricing.AirFlight flight = new BL.Entities.InformativePricing.AirFlight();
                for (int i = 0; i < depFlights.Length - 2; i++)
                {
                    flight = new BL.Entities.InformativePricing.AirFlight();
                    flight.departureCity = depFlights[i].Substring(0, 3);
                    flight.arrivalCity = depFlights[i].Substring(3, 3);
                    flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.RBDCode = depFlights[i].Substring(30, 1);
                    flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                    flight._ft = depFlights[i].Substring(43, 2);
                    if (flight._ft == "RP")
                    {
                        bHaveRP = true;
                    }
                    else
                    {
                        bHaveRU = true;
                    }
                    flight.companyCode = depFlights[i].Substring(45, 2);
                    flight.operatedBy = depFlights[i].Substring(47, 2);
                    flight._equibment = depFlights[i].Substring(49, 3);
                    flight.flightNumber = depFlights[i].Substring(52);
                    mFlights.Add(flight);
                }
                request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                request.multiFlights.Add(mFlights);

                mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                depFlights = S2.Split('|');
                for (int i = 0; i < depFlights.Length - 2; i++)
                {
                    flight = new BL.Entities.InformativePricing.AirFlight();
                    flight.departureCity = depFlights[i].Substring(0, 3);
                    flight.arrivalCity = depFlights[i].Substring(3, 3);
                    flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.RBDCode = depFlights[i].Substring(30, 1);
                    flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                    flight._ft = depFlights[i].Substring(43, 2);
                    if (flight._ft == "RP")
                    {
                        bHaveRP = true;
                    }
                    else
                    {
                        bHaveRU = true;
                    }
                    flight.companyCode = depFlights[i].Substring(45, 2);
                    flight.operatedBy = depFlights[i].Substring(47, 2);
                    flight._equibment = depFlights[i].Substring(49, 3);
                    flight.flightNumber = depFlights[i].Substring(52);
                    mFlights.Add(flight);
                }
                request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                request.multiFlights.Add(mFlights);

                if (S3 != null)
                {
                    mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                    depFlights = S3.Split('|');
                    for (int i = 0; i < depFlights.Length - 2; i++)
                    {
                        flight = new BL.Entities.InformativePricing.AirFlight();
                        flight.departureCity = depFlights[i].Substring(0, 3);
                        flight.arrivalCity = depFlights[i].Substring(3, 3);
                        flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.RBDCode = depFlights[i].Substring(30, 1);
                        flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                        flight._ft = depFlights[i].Substring(43, 2);
                        if (flight._ft == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                        flight.companyCode = depFlights[i].Substring(45, 2);
                        flight.operatedBy = depFlights[i].Substring(47, 2);
                        flight._equibment = depFlights[i].Substring(49, 3);
                        flight.flightNumber = depFlights[i].Substring(52);
                        mFlights.Add(flight);
                    }
                    request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                    request.multiFlights.Add(mFlights);
                }
                if (S4 != null)
                {
                    mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                    depFlights = S4.Split('|');
                    for (int i = 0; i < depFlights.Length - 2; i++)
                    {
                        flight = new BL.Entities.InformativePricing.AirFlight();
                        flight.departureCity = depFlights[i].Substring(0, 3);
                        flight.arrivalCity = depFlights[i].Substring(3, 3);
                        flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.RBDCode = depFlights[i].Substring(30, 1);
                        flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                        flight._ft = depFlights[i].Substring(43, 2);
                        if (flight._ft == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                        flight.companyCode = depFlights[i].Substring(45, 2);
                        flight.operatedBy = depFlights[i].Substring(47, 2);
                        flight._equibment = depFlights[i].Substring(49, 3);
                        flight.flightNumber = depFlights[i].Substring(52);
                        mFlights.Add(flight);
                    }
                    request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                    request.multiFlights.Add(mFlights);
                }
                if (S5 != null)
                {
                    mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                    depFlights = S5.Split('|');
                    for (int i = 0; i < depFlights.Length - 2; i++)
                    {
                        flight = new BL.Entities.InformativePricing.AirFlight();
                        flight.departureCity = depFlights[i].Substring(0, 3);
                        flight.arrivalCity = depFlights[i].Substring(3, 3);
                        flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.RBDCode = depFlights[i].Substring(30, 1);
                        flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                        flight._ft = depFlights[i].Substring(43, 2);
                        if (flight._ft == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                        flight.companyCode = depFlights[i].Substring(45, 2);
                        flight.operatedBy = depFlights[i].Substring(47, 2);
                        flight._equibment = depFlights[i].Substring(49, 3);
                        flight.flightNumber = depFlights[i].Substring(52);
                        mFlights.Add(flight);
                    }
                    request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                    request.multiFlights.Add(mFlights);
                }
                if (S6 != null)
                {
                    mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                    depFlights = S6.Split('|');
                    for (int i = 0; i < depFlights.Length - 2; i++)
                    {
                        flight = new BL.Entities.InformativePricing.AirFlight();
                        flight.departureCity = depFlights[i].Substring(0, 3);
                        flight.arrivalCity = depFlights[i].Substring(3, 3);
                        flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.RBDCode = depFlights[i].Substring(30, 1);
                        flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                        flight._ft = depFlights[i].Substring(43, 2);
                        if (flight._ft == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                        flight.companyCode = depFlights[i].Substring(45, 2);
                        flight.operatedBy = depFlights[i].Substring(47, 2);
                        flight._equibment = depFlights[i].Substring(49, 3);
                        flight.flightNumber = depFlights[i].Substring(52);
                        mFlights.Add(flight);
                    }
                    request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                    request.multiFlights.Add(mFlights);
                }
            }

            request.PricingTicketingIndicators = new List<string>();
            if (bHaveRU)
            {
                request.PricingTicketingIndicators.Add("RU");
            }
            if (bHaveRP)
            {
                request.PricingTicketingIndicators.Add("RP");
            }


            request.session = new BL.Entities.InformativePricing.Session();
            request.session.isStateFull = true;
            request.session.InSeries = false;
            request.session.End = false;
            request.session.SessionId = "";
            request.session.SequenceNumber = "";
            request.session.SecurityToken = "";
            request.useBookingOfficeID = false;
            request.bookingOID = "";
            request.nounce = null;
            request.signature = null;
            request.tripType = this.tripType;
            request.ClientIP = "";
            return request;
        }

        public BL.Entities.InformativePricing.Request GetInformativePricingRequest()
        {
            BL.Entities.InformativePricing.Request request = new BL.Entities.InformativePricing.Request();
            request.origin = originCode;
            request.destination = destinationCode;
            request.svc_class = svcClass;
            request.noOfAdults = adult;
            request.noOfChildren = child;
            request.noOfInfants = infant;
            request.multiFlights = null;
            bool bHaveRU = false;
            bool bHaveRP = false;

            if (this.tripType != "M")
            {

                request.departureFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                string[] depFlights = S1.Split('|');
                for (int i = 0; i < depFlights.Length - 2; i++)
                {
                    BL.Entities.InformativePricing.AirFlight flight = new BL.Entities.InformativePricing.AirFlight();
                    flight.departureCity = depFlights[i].Substring(0, 3);
                    flight.arrivalCity = depFlights[i].Substring(3, 3);
                    flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.RBDCode = depFlights[i].Substring(30, 1);
                    flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                    flight._ft = depFlights[i].Substring(43, 2);
                    if (flight._ft == "RP")
                    {
                        bHaveRP = true;
                    }
                    else
                    {
                        bHaveRU = true;
                    }
                    flight.companyCode = depFlights[i].Substring(45, 2);
                    flight.operatedBy = depFlights[i].Substring(47, 2);
                    flight._equibment = depFlights[i].Substring(49, 3);
                    flight.flightNumber = depFlights[i].Substring(52);
                    request.departureFlights.Add(flight);

                }
                request.depTotalTime = depFlights[depFlights.Length - 2];

                if (this.tripType == "R")
                {
                    request.returnFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                    string[] retFlights = S2.Split('|');
                    for (int i = 0; i < retFlights.Length - 2; i++)
                    {
                        BL.Entities.InformativePricing.AirFlight flight = new BL.Entities.InformativePricing.AirFlight();
                        flight.departureCity = retFlights[i].Substring(0, 3);
                        flight.arrivalCity = retFlights[i].Substring(3, 3);
                        flight.departureDateTime = DateTime.ParseExact(retFlights[i].Substring(6, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.arrivalDateTime = DateTime.ParseExact(retFlights[i].Substring(18, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.RBDCode = retFlights[i].Substring(30, 1);
                        flight._fb = retFlights[i].Substring(31, 12).Replace(".", "");
                        flight._ft = retFlights[i].Substring(43, 2);
                        if (flight._ft == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                        flight.companyCode = retFlights[i].Substring(45, 2);
                        flight.operatedBy = retFlights[i].Substring(47, 2);
                        flight._equibment = retFlights[i].Substring(49, 3);
                        flight.flightNumber = retFlights[i].Substring(52);
                        request.returnFlights.Add(flight);
                    }
                    request.retTotalTime = retFlights[retFlights.Length - 2];
                }
            }
            else//Multi Destination
            {
                request.multiTotalTime = new List<string>();
                request.multiFlights = new List<List<BL.Entities.InformativePricing.AirFlight>>();
                string[] depFlights = S1.Split('|');
                List<BL.Entities.InformativePricing.AirFlight> mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                BL.Entities.InformativePricing.AirFlight flight = new BL.Entities.InformativePricing.AirFlight();
                for (int i = 0; i < depFlights.Length - 2; i++)
                {
                    flight = new BL.Entities.InformativePricing.AirFlight();
                    flight.departureCity = depFlights[i].Substring(0, 3);
                    flight.arrivalCity = depFlights[i].Substring(3, 3);
                    flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.RBDCode = depFlights[i].Substring(30, 1);
                    flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                    flight._ft = depFlights[i].Substring(43, 2);
                    if (flight._ft == "RP")
                    {
                        bHaveRP = true;
                    }
                    else
                    {
                        bHaveRU = true;
                    }
                    flight.companyCode = depFlights[i].Substring(45, 2);
                    flight.operatedBy = depFlights[i].Substring(47, 2);
                    flight._equibment = depFlights[i].Substring(49, 3);
                    flight.flightNumber = depFlights[i].Substring(52);
                    mFlights.Add(flight);
                }
                request.multiTotalTime.Add(depFlights[depFlights.Length -2]);
                request.multiFlights.Add(mFlights);

                mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                depFlights = S2.Split('|');
                for (int i = 0; i < depFlights.Length - 2; i++)
                {
                    flight = new BL.Entities.InformativePricing.AirFlight();
                    flight.departureCity = depFlights[i].Substring(0, 3);
                    flight.arrivalCity = depFlights[i].Substring(3, 3);
                    flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                            , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                    flight.RBDCode = depFlights[i].Substring(30, 1);
                    flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                    flight._ft = depFlights[i].Substring(43, 2);
                    if (flight._ft == "RP")
                    {
                        bHaveRP = true;
                    }
                    else
                    {
                        bHaveRU = true;
                    }
                    flight.companyCode = depFlights[i].Substring(45, 2);
                    flight.operatedBy = depFlights[i].Substring(47, 2);
                    flight._equibment = depFlights[i].Substring(49, 3);
                    flight.flightNumber = depFlights[i].Substring(52);
                    mFlights.Add(flight);
                }
                request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                request.multiFlights.Add(mFlights);

                if (S3 != null)
                {
                    mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                    depFlights = S3.Split('|');
                    for (int i = 0; i < depFlights.Length - 2; i++)
                    {
                        flight = new BL.Entities.InformativePricing.AirFlight();
                        flight.departureCity = depFlights[i].Substring(0, 3);
                        flight.arrivalCity = depFlights[i].Substring(3, 3);
                        flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.RBDCode = depFlights[i].Substring(30, 1);
                        flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                        flight._ft = depFlights[i].Substring(43, 2);
                        if (flight._ft == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                        flight.companyCode = depFlights[i].Substring(45, 2);
                        flight.operatedBy = depFlights[i].Substring(47, 2);
                        flight._equibment = depFlights[i].Substring(49, 3);
                        flight.flightNumber = depFlights[i].Substring(52);
                        mFlights.Add(flight);
                    }
                    request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                    request.multiFlights.Add(mFlights);
                }
                if (S4 != null)
                {
                    mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                    depFlights = S4.Split('|');
                    for (int i = 0; i < depFlights.Length - 2; i++)
                    {
                        flight = new BL.Entities.InformativePricing.AirFlight();
                        flight.departureCity = depFlights[i].Substring(0, 3);
                        flight.arrivalCity = depFlights[i].Substring(3, 3);
                        flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.RBDCode = depFlights[i].Substring(30, 1);
                        flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                        flight._ft = depFlights[i].Substring(43, 2);
                        if (flight._ft == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                        flight.companyCode = depFlights[i].Substring(45, 2);
                        flight.operatedBy = depFlights[i].Substring(47, 2);
                        flight._equibment = depFlights[i].Substring(49, 3);
                        flight.flightNumber = depFlights[i].Substring(52);
                        mFlights.Add(flight);
                    }
                    request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                    request.multiFlights.Add(mFlights);
                }
                if (S5 != null)
                {
                    mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                    depFlights = S5.Split('|');
                    for (int i = 0; i < depFlights.Length - 2; i++)
                    {
                        flight = new BL.Entities.InformativePricing.AirFlight();
                        flight.departureCity = depFlights[i].Substring(0, 3);
                        flight.arrivalCity = depFlights[i].Substring(3, 3);
                        flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.RBDCode = depFlights[i].Substring(30, 1);
                        flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                        flight._ft = depFlights[i].Substring(43, 2);
                        if (flight._ft == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                        flight.companyCode = depFlights[i].Substring(45, 2);
                        flight.operatedBy = depFlights[i].Substring(47, 2);
                        flight._equibment = depFlights[i].Substring(49, 3);
                        flight.flightNumber = depFlights[i].Substring(52);
                        mFlights.Add(flight);
                    }
                    request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                    request.multiFlights.Add(mFlights);
                }
                if (S6 != null)
                {
                    mFlights = new List<BL.Entities.InformativePricing.AirFlight>();
                    depFlights = S6.Split('|');
                    for (int i = 0; i < depFlights.Length - 2; i++)
                    {
                        flight = new BL.Entities.InformativePricing.AirFlight();
                        flight.departureCity = depFlights[i].Substring(0, 3);
                        flight.arrivalCity = depFlights[i].Substring(3, 3);
                        flight.departureDateTime = DateTime.ParseExact(depFlights[i].Substring(6, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.arrivalDateTime = DateTime.ParseExact(depFlights[i].Substring(18, 12)
                                , "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                        flight.RBDCode = depFlights[i].Substring(30, 1);
                        flight._fb = depFlights[i].Substring(31, 12).Replace(".", "");
                        flight._ft = depFlights[i].Substring(43, 2);
                        if (flight._ft == "RP")
                        {
                            bHaveRP = true;
                        }
                        else
                        {
                            bHaveRU = true;
                        }
                        flight.companyCode = depFlights[i].Substring(45, 2);
                        flight.operatedBy = depFlights[i].Substring(47, 2);
                        flight._equibment = depFlights[i].Substring(49, 3);
                        flight.flightNumber = depFlights[i].Substring(52);
                        mFlights.Add(flight);
                    }
                    request.multiTotalTime.Add(depFlights[depFlights.Length - 2]);
                    request.multiFlights.Add(mFlights);
                }
            }
            request.PricingTicketingIndicators = new List<string>();
            if (bHaveRU)
            {
                request.PricingTicketingIndicators.Add("RU");
            }
            if (bHaveRP)
            {
                request.PricingTicketingIndicators.Add("RP");
            }
            request.session = new BL.Entities.InformativePricing.Session();
            request.session.isStateFull = true;
            request.session.InSeries = false;
            request.session.End = false;
            request.session.SessionId = "";
            request.session.SequenceNumber = "";
            request.session.SecurityToken = "";
            request.useBookingOfficeID = false;
            request.bookingOID = "";
            request.nounce = null;
            request.signature = null;
            request.tripType = this.tripType;
            request.ClientIP = "";
            return request;
        }
    }
}