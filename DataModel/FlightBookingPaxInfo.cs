//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class FlightBookingPaxInfo
    {
        public System.Guid FlightBookingPaxInfoOID { get; set; }
        public System.Guid BookingOID { get; set; }
        public string PaxType { get; set; }
        public string Title { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Email { get; set; }
        public string TelNo { get; set; }
        public string PassportNumber { get; set; }
        public Nullable<System.DateTime> PassportIssuingDate { get; set; }
        public Nullable<System.DateTime> PassportExpiryDate { get; set; }
        public string PassportIssuingCountry { get; set; }
        public string PassportNationality { get; set; }
        public string PessengerType { get; set; }
        public Nullable<decimal> NetRefund { get; set; }
        public Nullable<decimal> AgentRefund { get; set; }
        public Nullable<decimal> RefundAmount { get; set; }
        public string TickNoRefund { get; set; }
        public string RemarkRefund { get; set; }
        public Nullable<decimal> NetReissue { get; set; }
        public Nullable<decimal> AgentReissue { get; set; }
        public Nullable<decimal> ReissueSelling { get; set; }
        public string TickNoReissueOld { get; set; }
        public string TickNoReissueNew { get; set; }
        public string RemarkReissue { get; set; }
        public Nullable<bool> StatusRefund { get; set; }
        public Nullable<bool> StatusReissue { get; set; }
        public Nullable<int> KiwiBag { get; set; }
        public Nullable<decimal> KiwiBagPrice { get; set; }
        public Nullable<int> KiwiBagWeight { get; set; }
        public string FrequencyFlyerAirline { get; set; }
        public string FrequencyFlyerNumber { get; set; }
        public string MealRequest { get; set; }
        public string SeatRequest { get; set; }
    }
}