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
    
    public partial class FlightFareRuleConfig
    {
        public System.Guid FareRuleConfigOID { get; set; }
        public System.Guid FareRuleOID { get; set; }
        public string ZoneFrom { get; set; }
        public string ZoneTo { get; set; }
        public string RBD { get; set; }
        public string FareBasis { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}
