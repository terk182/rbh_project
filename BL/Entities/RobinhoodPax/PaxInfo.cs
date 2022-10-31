using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BL.Entities.RobinhoodPax
{
    public class ContactInfo
    {
        public string title { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string telNo { get; set; }
        public string countryCode { get; set; }
    }
    public class PaxInfo
    {
        public int id { get; set; }
        public string paxType { get; set; }
        public string title { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public DateTime birthday { get; set; }
        public string email { get; set; }
        public string telNo { get; set; }

        public string passportNumber { get; set; }
        public DateTime passportIssuingDate { get; set; }
        public DateTime passportExpiryDate { get; set; }
        public string passportIssuingCountry { get; set; }
        public string passportNationality { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public string frequencyFlyerAirline { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public string frequencyFlyerNumber { get; set; }
        public List<FrequentFlyList> frequentFlyList { get; set; }
        public string seatRequest { get; set; }
        public List<SeatsRequest> seatsRequest { get; set; }

        public string mealRequest { get; set; }

        public int travelWithAdultID { get; set; }

        //Refund
        public decimal netRefund { get; set; }
        public decimal agentRefund { get; set; }
        public decimal refundAmount { get; set; }
        public string tickNoRefund { get; set; }
        public string remarkRefund { get; set; }
        public bool StatusRefund { get; set; }

        //Reissue
        public decimal netReissue { get; set; }
        public decimal agentReissue { get; set; }
        public decimal reissueSelling { get; set; }
        public string tickNoReissueOld { get; set; }
        public string tickNoReissueNew { get; set; }
        public string remarkReissue { get; set; }
        public bool StatusReissue { get; set; }

        //kiwiBag
        public int kiwiBag { get; set; }
        public decimal kiwiBagPrice { get; set; }
        public int kiwiBagWeight { get; set; }
    }
    public class FrequentFlyList
    {
        public string Airline { get; set; }
        public string Number { get; set; }
    }
    public class SeatsRequest
    {
        public string seatType { get; set; }
        public string seatRefNo { get; set; }
    }
}
