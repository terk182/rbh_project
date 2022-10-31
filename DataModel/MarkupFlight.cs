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
    
    public partial class MarkupFlight
    {
        public System.Guid RobinhoodMarkupOID { get; set; }
        public string AirlineCodes { get; set; }
        public string MixAirlineCodes { get; set; }
        public string RBD { get; set; }
        public string FareBasis { get; set; }
        public string FlightNo { get; set; }
        public Nullable<bool> PaxTypeADT { get; set; }
        public Nullable<bool> PaxTypeCHD { get; set; }
        public Nullable<bool> PaxTypeINF { get; set; }
        public Nullable<System.DateTime> StartBookingDate { get; set; }
        public Nullable<System.DateTime> FinishBookingDate { get; set; }
        public string ZoneFrom { get; set; }
        public string ZoneTo { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> MinPrice { get; set; }
        public Nullable<decimal> MaxPrice { get; set; }
        public string LV1Type { get; set; }
        public Nullable<decimal> LV1Value { get; set; }
        public string LV2Type { get; set; }
        public Nullable<decimal> LV2Value { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<bool> IsPercentLV1 { get; set; }
        public Nullable<bool> IsPercentLV2 { get; set; }
        public Nullable<System.DateTime> StartTravelDate { get; set; }
        public Nullable<System.DateTime> FinishTravelDate { get; set; }
        public string DomainName { get; set; }
        public string LV1TypeRT { get; set; }
        public Nullable<decimal> LV1ValueRT { get; set; }
        public string LV2TypeRT { get; set; }
        public Nullable<decimal> LV2ValueRT { get; set; }
        public Nullable<bool> IsPercentLV1RT { get; set; }
        public Nullable<bool> IsPercentLV2RT { get; set; }
    }
}
