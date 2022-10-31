using BL.Entities.CitySearch;
using BL.Entities.GogojiiFlight;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IFlightSearchServices
    {
        CityList SearchCities(string keyword, string language, int numberOfList);
        void AddLog(FlightSearchLog log);
        FlightSearchResult Search(Entities.MasterPricer.Request request);
        Entities.GogojiiFare.AirFare InformativePricing(Entities.InformativePricing.Request request, Entities.InformativePricing.Request requestFor1A, string languageCode);
        Entities.GogojiiFare.AirFare GhostPricing(Entities.AirSell.Request request, string languageCode);
        Entities.GogojiiPNR.PNR Booking(Entities.GogojiiFare.AirFare request);
        void UpdatePaymentStatus(Entities.GogojiiPNR.PNR pnr, int paymentType);
        string GetInvoiceNo(Entities.Invoice.Request invoiceReq);
    }
}
