using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.AirSell
{
    public class AirFlight
    {
        public DateTime departureDateTime { get; set; }
        public DateTime arrivalDateTime { get; set; }
        public string departureCity { get; set; }
        public string arrivalCity { get; set; }
        public string companyCode { get; set; }
        public string flightNumber { get; set; }
        public string RBDCode { get; set; }
        public string operatedBy { get; set; }
    }

    //public class ReturnFlight
    //{
    //    public DateTime departureDateTime { get; set; }
    //    public DateTime arrivalDateTime { get; set; }
    //    public string departureCity { get; set; }
    //    public string arrivalCity { get; set; }
    //    public string companyCode { get; set; }
    //    public string flightNumber { get; set; }
    //    public string RBDCode { get; set; }
    //    public string operatedBy { get; set; }
    //}
    

    public class Request
    {
        public string origin { get; set; }
        public string destination { get; set; }
        public string svc_class { get; set; }
        public int noOfAdults { get; set; }
        public int noOfChildren { get; set; }
        public int noOfInfants { get; set; }
        public string depTotalTime { get; set; }
        public string retTotalTime { get; set; }
        public List<string> multiTotalTime { get; set; }
        public List<AirFlight> departureFlights { get; set; }
        public List<AirFlight> returnFlights { get; set; }
        public List<List<AirFlight>> multiFlights { get; set; }
        public Session session { get; set; }
        public bool useBookingOfficeID { get; set; }
        public string bookingOID { get; set; }
        public object nounce { get; set; }
        public object signature { get; set; }
        public string tripType { get; set; }
        public string ClientIP { get; set; }
    }
}
