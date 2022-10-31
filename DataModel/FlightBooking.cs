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
    
    public partial class FlightBooking
    {
        public System.Guid BookingOID { get; set; }
        public Nullable<int> NoOfAdults { get; set; }
        public Nullable<int> NoOfChildren { get; set; }
        public Nullable<int> NoOfInfants { get; set; }
        public string Svc_class { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public Nullable<bool> IsPassportRequired { get; set; }
        public string PNR { get; set; }
        public string Platform { get; set; }
        public Nullable<int> MedId { get; set; }
        public Nullable<bool> PriceChange { get; set; }
        public Nullable<decimal> OldPrice { get; set; }
        public Nullable<System.DateTime> TKTL { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> TotalFare { get; set; }
        public Nullable<decimal> CreditCardFee { get; set; }
        public Nullable<decimal> PaypalFee { get; set; }
        public Nullable<decimal> CounterServiceFee { get; set; }
        public Nullable<bool> IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public Nullable<System.DateTime> bookingDate { get; set; }
        public string Title { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string TelNo { get; set; }
        public string CountryOfResidence { get; set; }
        public string Note { get; set; }
        public string RobinhoodID { get; set; }
        public Nullable<int> StatusPayment { get; set; }
        public Nullable<int> PaymentMethod { get; set; }
        public Nullable<int> StatusBooking { get; set; }
        public Nullable<bool> IsBundle { get; set; }
        public Nullable<int> sourceBy { get; set; }
        public Nullable<decimal> PaymentFee { get; set; }
        public string PromoCode { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<int> InstallmentPlan { get; set; }
        public Nullable<decimal> InstallmentMonthly { get; set; }
        public Nullable<decimal> FinalPrice { get; set; }
        public string UUID { get; set; }
        public string UserID { get; set; }
        public Nullable<System.Guid> MemberOID { get; set; }
        public string type { get; set; }
        public Nullable<System.DateTime> paymentDate { get; set; }
        public string Wallet_Address { get; set; }
        public Nullable<decimal> NFTFee { get; set; }
        public string Transaction_Hash { get; set; }
        public string paymentReference { get; set; }
        public string PromotionGroupCode { get; set; }
        public string BookingURN { get; set; }
    }
}
