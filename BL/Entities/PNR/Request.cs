using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.PNR
{
    public class InfantPassenger
    {
        public int paxNo { get; set; }
        public bool infantFlag { get; set; }
        public bool haveDOB { get; set; }
        public DateTime DOB { get; set; }
        public int age { get; set; }
        public int withADT { get; set; }
        public string titleName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string paxType { get; set; }
        public string paxID { get; set; }
        public string passportNumber { get; set; }
    }

    public class PassengerList
    {
        public int paxNo { get; set; }
        public bool infantFlag { get; set; }
        public bool haveDOB { get; set; }
        public DateTime DOB { get; set; }
        public int age { get; set; }
        public InfantPassenger infantPassenger { get; set; }
        public int withADT { get; set; }
        public string ptNo { get; set; }
        public string titleName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string paxType { get; set; }
        public string paxID { get; set; }
        public bool? withInsurance { get; set; }
        public object seat { get; set; }
        public object ssrFreeTextList { get; set; }
        public object frequentFlyerAirline { get; set; }
        public object frequentFlyerNumber { get; set; }
        public List<RobinhoodPax.FrequentFlyList> frequentFlyList { get; set; }
        public object depSpecialRequest { get; set; }
        public object retSpecialRequest { get; set; }
        public object specialRequestList { get; set; }
        public string ticketID { get; set; }
        public string depMealsCode { get; set; }
        public string depMealsText { get; set; }
        public string retMealsCode { get; set; }
        public string retMealsText { get; set; }
        public string policyNumber { get; set; }
        public string passportNumber { get; set; }
    }

    public partial class Session
    {
        public bool isStateFull { get; set; }
        public bool InSeries { get; set; }
        public bool End { get; set; }
        public string SessionId { get; set; }
        public string SequenceNumber { get; set; }
        public string SecurityToken { get; set; }
    }

    public class Request
    {
        public List<PassengerList> passengerList { get; set; }
        public DateTime ticketTimeLimitDate { get; set; }
        public List<string> remarks { get; set; }
        public Session session { get; set; }
        public bool useBookingOfficeID { get; set; }
        public string bookingOID { get; set; }
        public string accessOID { get; set; }
        public object nounce { get; set; }
        public object signature { get; set; }
        public string ticketIndicator { get; set; }
        public string originCity { get; set; }
        public string destinationCity { get; set; }
        public string refContactNumber { get; set; }
        public string refMobileNumber { get; set; }
        public string refFaxNumber { get; set; }
        public string refBusinessNumber { get; set; }
        public string refEmail { get; set; }
        public string remarkType { get; set; }
        public string ClientIP { get; set; }
        public int departFlightSegmentCount { get; set; }
        public int returnFlightSegmentCount { get; set; }
        public string optionCode { get; set; }
        public string fop { get; set; }
        public string osCompanyID { get; set; }
        public string osLongFreeText { get; set; }
        public string rfLongFreeText { get; set; }
        public string skType { get; set; }
        public string skCompanyId { get; set; }
        public string skFreetext { get; set; }
        public List<PNRQueue> queues { get; set; }
        public FKOption fkOption { get; set; }
    }

    public class RTRequest
    {
        public Session session { get; set; }
        public bool useBookingOfficeID { get; set; }
        public string rtType { get; set; }
        public string bookingOID { get; set; }
        public string officeID { get; set; }
        public string option1 { get; set; }
        public string option2 { get; set; }
        public string pnrNumber { get; set; }
        public string firstName { get; set; }
        public string surName { get; set; }
        public string ClientIP { get; set; }
    }
    public class PNRQueue
    {
        public string queueNumber { get; set; }
        public string officeID { get; set; }
        public string categoryNumber { get; set; }
    }
    public class FKOption
    {
        public string generalIndicator { get; set; }
        public string officeId { get; set; }
    }
}