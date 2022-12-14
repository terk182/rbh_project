// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using TGBookingWeb.Models;
//
//    var kBankNotifyResponse = KBankNotifyResponse.FromJson(jsonString);

namespace TGBookingWeb.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class KBankNotifyResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("transaction_state")]
        public string TransactionState { get; set; }

        [JsonProperty("source")]
        public Source Source { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("reference_order")]
        public string ReferenceOrder { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonProperty("approval_code")]
        public string ApprovalCode { get; set; }

        [JsonProperty("ref_1")]
        public string Ref1 { get; set; }

        [JsonProperty("ref_2")]
        public string Ref2 { get; set; }

        [JsonProperty("ref_3")]
        public string Ref3 { get; set; }

        [JsonProperty("livemode")]
        public bool Livemode { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("failure_code")]
        public string FailureCode { get; set; }

        [JsonProperty("failure_message")]
        public string FailureMessage { get; set; }

        [JsonProperty("checksum")]
        public string Checksum { get; set; }
    }

    public partial class Metadata
    {
    }

    public partial class Source
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("card_masking")]
        public string CardMasking { get; set; }

        [JsonProperty("issuer_bank")]
        public string IssuerBank { get; set; }
    }

    public partial class KBankNotifyResponse
    {
        public static KBankNotifyResponse FromJson(string json) => JsonConvert.DeserializeObject<KBankNotifyResponse>(json, TGBookingWeb.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this KBankNotifyResponse self) => JsonConvert.SerializeObject(self, TGBookingWeb.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

}
