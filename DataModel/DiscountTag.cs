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
    
    public partial class DiscountTag
    {
        public System.Guid DiscountTagOID { get; set; }
        public Nullable<System.Guid> PromotionGroupCodeOID { get; set; }
        public string AirlineCodes { get; set; }
        public string RBD { get; set; }
        public string FareBasis { get; set; }
        public Nullable<bool> PaxTypeADT { get; set; }
        public Nullable<bool> PaxTypeCHD { get; set; }
        public Nullable<bool> PaxTypeINF { get; set; }
        public Nullable<System.DateTime> StartBookingDate { get; set; }
        public Nullable<System.DateTime> FinishBookingDate { get; set; }
        public string ZoneFrom { get; set; }
        public string ZoneTo { get; set; }
        public Nullable<decimal> DiscountAmt { get; set; }
        public Nullable<bool> DiscountIsPercent { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}
