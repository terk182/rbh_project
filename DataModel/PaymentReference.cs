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
    
    public partial class PaymentReference
    {
        public System.Guid PaymentRefID { get; set; }
        public string PaymentRefOrderNo { get; set; }
        public string PaymentType { get; set; }
        public Nullable<System.DateTime> CreateDateTime { get; set; }
        public string Product { get; set; }
        public Nullable<System.Guid> BookingID { get; set; }
    }
}
