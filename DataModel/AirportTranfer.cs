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
    
    public partial class AirportTranfer
    {
        public System.Guid AirportTranferOID { get; set; }
        public string Destination { get; set; }
        public Nullable<System.DateTime> StartBookingDate { get; set; }
        public Nullable<System.DateTime> FinishBookingDate { get; set; }
        public Nullable<System.DateTime> StartTravelDate { get; set; }
        public Nullable<System.DateTime> FinishTravelDate { get; set; }
        public string Discount { get; set; }
        public Nullable<bool> IsPercentDiscount { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string AirportCode { get; set; }
    }
}
