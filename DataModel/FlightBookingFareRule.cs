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
    
    public partial class FlightBookingFareRule
    {
        public System.Guid FlightBookingFareRuleOID { get; set; }
        public System.Guid BookingOID { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string FareBasis { get; set; }
    }
}
