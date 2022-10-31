using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BL.Entities.GogojiiFare;
using BL.Entities.GogojiiPNR;

namespace GogojiiWeb.Models
{
    public class CounterServiceModel
    {
        public string Url4Authorize = "";
        public string mid = "";
        public string inv = "";
        public string desp = "";
        public string amt = "";
        public string curr_type = "";
        public string serviceid = "";
        public string language = "";
        public string customer_name = "";
        public string customer_email = "";
        public string customer_phone = "";
        public string resp_url_back = "";
        public string resp_url_cancel = "";
        public string resp_url_confirm = "";
        public string expired_date = "";
        public string message_slip1 = "";
        public string message_slip2 = "";

        public CounterServiceModel(string frontURL, AirFare airfare, PNR pnr, string invoiceNo, bool isProd)
        {
            Url4Authorize = isProd ? "https://gateway.payatall.com/paynow" : "https://gateway.payatall.com/paynow";
            mid = "182";
            inv = invoiceNo;
            desp = "Air Ticket-" + pnr.recordLocator + "/" + airfare.origin.code + airfare.destination.code;
            amt = (airfare.counterServiceFee + airfare.totalFare).ToString("N2").Replace(",", "");
            curr_type = "THB";
            serviceid = "01";
            language = Localize.GetLang().ToUpper();
            customer_name = String.Format("{0} {1}", airfare.adtPaxs[0].firstname, airfare.adtPaxs[0].lastname);
            customer_email = airfare.adtPaxs[0].email;
            customer_phone = airfare.adtPaxs[0].telNo;
            resp_url_back = frontURL + "/CSConfirm";
            resp_url_cancel = frontURL + "/CSCancel";
            resp_url_confirm = frontURL + "/CSConfirm";
            expired_date = airfare.TKTL.ToString("yyyy-MM-dd HH:mm:ss");
            message_slip1 = "Invoice No:" + invoiceNo;
            message_slip2 = "PNR:" + pnr.recordLocator;

        }
    }
}