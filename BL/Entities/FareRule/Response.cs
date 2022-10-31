// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BL.Entities.FareRule;
//
//    var response = Response.FromJson(jsonString);

namespace BL.Entities.FareRule
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Response
    {
        [JsonProperty("Fare_CheckRulesReply")]
        public FareCheckRulesReply FareCheckRulesReply { get; set; }
        
        [JsonProperty("Error")]
        public Error Error { get; set; }
    }

    public partial class Error
    {
        [JsonProperty("ErrorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("Session")]
        public Session Session { get; set; }
    }

    public partial class FareCheckRulesReply
    {
        [JsonProperty("xmlns")]
        public string Xmlns { get; set; }

        [JsonProperty("transactionType")]
        public TransactionType TransactionType { get; set; }

        [JsonProperty("tariffInfo")]
        public TariffInfoUnion TariffInfo { get; set; }

        [JsonProperty("flightDetails")]
        public FlightDetailsUnion FlightDetails { get; set; }

        [JsonProperty("Session")]
        public Session Session { get; set; }
    }
    public partial struct FlightDetailsUnion
    {
        public List<FlightDetails> FlightDetailsElementArray;
        public FlightDetails PurpleFlightDetails;

        public bool IsNull => FlightDetailsElementArray == null && PurpleFlightDetails == null;
    }
    public partial struct TariffInfoUnion
    {
        public List<TariffInfo> TariffInfoElementArray;
        public TariffInfo PurpleTariffInfo;

        public bool IsNull => TariffInfoElementArray == null && PurpleTariffInfo == null;
    }

    public partial class FlightDetails
    {
        [JsonProperty("nbOfSegments")]
        public string NbOfSegments { get; set; }

        [JsonProperty("qualificationFareDetails")]
        public QualificationFareDetails QualificationFareDetails { get; set; }

        [JsonProperty("transportService")]
        public TransportService TransportService { get; set; }

        [JsonProperty("flightErrorCode")]
        public FlightErrorCodeUnion FlightErrorCode { get; set; }

        [JsonProperty("productInfo")]
        public ProductInfo ProductInfo { get; set; }

        [JsonProperty("fareDetailInfo")]
        public FareDetailInfo FareDetailInfo { get; set; }

        [JsonProperty("odiGrp")]
        public OdiGrp OdiGrp { get; set; }

        [JsonProperty("travellerGrp")]
        public TravellerGrp TravellerGrp { get; set; }

        [JsonProperty("itemGrp")]
        public ItemGrp ItemGrp { get; set; }
    }
    public partial struct FlightErrorCodeUnion
    {
        public List<FlightErrorCode> FlightErrorCodeElementArray;
        public FlightErrorCode PurpleFlightErrorCode;

        public bool IsNull => FlightErrorCodeElementArray == null && PurpleFlightErrorCode == null;
    }

    public partial class FareDetailInfo
    {
        [JsonProperty("nbOfUnits")]
        public FareDetailInfoNbOfUnits NbOfUnits { get; set; }

        [JsonProperty("fareDeatilInfo")]
        public FareDeatilInfo FareDeatilInfo { get; set; }
    }

    public partial class FareDeatilInfo
    {
        [JsonProperty("fareTypeGrouping")]
        public FareTypeGrouping FareTypeGrouping { get; set; }
    }

    public partial class FareTypeGrouping
    {
        //[JsonProperty("pricingGroup")]
        //public string PricingGroup { get; set; }
    }

    public partial class FareDetailInfoNbOfUnits
    {
        [JsonProperty("quantityDetails")]
        public QuantityDetail QuantityDetails { get; set; }
    }

    public partial class QuantityDetail
    {
        [JsonProperty("numberOfUnit")]
        public string NumberOfUnit { get; set; }

        [JsonProperty("unitQualifier")]
        public string UnitQualifier { get; set; }
    }

    public partial class FlightErrorCode
    {
        [JsonProperty("freeTextQualification")]
        public FreeTextQualification FreeTextQualification { get; set; }

        [JsonProperty("freeText")]
        public string FreeText { get; set; }
    }

    public partial class FreeTextQualification
    {
        [JsonProperty("textSubjectQualifier")]
        public string TextSubjectQualifier { get; set; }

        [JsonProperty("informationType", NullValueHandling = NullValueHandling.Ignore)]
        public string InformationType { get; set; }
    }

    public partial class ItemGrp
    {
        [JsonProperty("itemNb")]
        public ItemNb ItemNb { get; set; }

        [JsonProperty("unitGrp")]
        public UnitGrp UnitGrp { get; set; }
    }

    public partial class ItemNb
    {
        [JsonProperty("itemNumberDetails")]
        public ItemNumberDetails ItemNumberDetails { get; set; }
    }

    public partial class ItemNumberDetails
    {
        [JsonProperty("number")]
        public string Number { get; set; }
    }

    public partial class UnitGrp
    {
        [JsonProperty("nbOfUnits")]
        public UnitGrpNbOfUnits NbOfUnits { get; set; }

        [JsonProperty("unitFareDetails")]
        public FareDeatilInfo UnitFareDetails { get; set; }
    }

    public partial class UnitGrpNbOfUnits
    {
        //[JsonProperty("quantityDetails")]
        //public List<QuantityDetail> QuantityDetails { get; set; }
    }

    public partial class OdiGrp
    {
        [JsonProperty("originDestination")]
        public OriginDestination OriginDestination { get; set; }
    }

    public partial class OriginDestination
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }
    }

    public partial class ProductInfo
    {
        [JsonProperty("productDetails")]
        public ProductDetails ProductDetails { get; set; }
    }

    public partial class ProductDetails
    {
        [JsonProperty("bookingClassDetails")]
        public BookingClassDetails BookingClassDetails { get; set; }
    }

    public partial class BookingClassDetails
    {
        [JsonProperty("designator")]
        public string Designator { get; set; }
    }

    public partial class QualificationFareDetails
    {
        [JsonProperty("fareDetails")]
        public FareDetails FareDetails { get; set; }

        [JsonProperty("additionalFareDetails")]
        public AdditionalFareDetails AdditionalFareDetails { get; set; }
    }

    public partial class AdditionalFareDetails
    {
        [JsonProperty("rateClass")]
        public string RateClass { get; set; }

        [JsonProperty("fareClass")]
        public string FareClass { get; set; }
    }

    public partial class FareDetails
    {
        [JsonProperty("qualifier")]
        public string Qualifier { get; set; }

        [JsonProperty("fareCategory")]
        public string FareCategory { get; set; }
    }

    public partial class TransportService
    {
        [JsonProperty("companyIdentification")]
        public CompanyIdentification CompanyIdentification { get; set; }
    }

    public partial class CompanyIdentification
    {
        [JsonProperty("marketingCompany")]
        public string MarketingCompany { get; set; }
    }

    public partial class TravellerGrp
    {
        [JsonProperty("travellerIdentRef")]
        public TravellerIdentRef TravellerIdentRef { get; set; }

        [JsonProperty("fareRulesDetails")]
        public FareRulesDetails FareRulesDetails { get; set; }
    }

    public partial class FareRulesDetails
    {
        [JsonProperty("tariffClassId")]
        public string TariffClassId { get; set; }

        //[JsonProperty("ruleSectionId")]
        //public List<string> RuleSectionId { get; set; }
    }

    public partial class TravellerIdentRef
    {
        [JsonProperty("referenceDetails")]
        public List<ReferenceDetail> ReferenceDetails { get; set; }
    }

    public partial class ReferenceDetail
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class Session
    {
        [JsonProperty("TransactionStatusCode")]
        public string TransactionStatusCode { get; set; }
    }

    public partial class TariffInfo
    {
        [JsonProperty("fareRuleInfo")]
        public FareRuleInfo FareRuleInfo { get; set; }

        [JsonProperty("fareRuleText")]
        public List<FlightErrorCode> FareRuleText { get; set; }
    }

    public partial class FareRuleInfo
    {
        [JsonProperty("ruleSectionLocalId")]
        public string RuleSectionLocalId { get; set; }

        [JsonProperty("ruleCategoryCode")]
        public string RuleCategoryCode { get; set; }
    }

    public partial class TransactionType
    {
        [JsonProperty("messageFunctionDetails")]
        public MessageFunctionDetails MessageFunctionDetails { get; set; }
    }

    public partial class MessageFunctionDetails
    {
        [JsonProperty("messageFunction")]
        public string MessageFunction { get; set; }
    }

    public partial class Response
    {
        public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, BL.Entities.FareRule.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Response self) => JsonConvert.SerializeObject(self, BL.Entities.FareRule.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new TariffInfoUnionConverter(),
                new FlightDetailsUnionConverter(),
                new FlightErrorCodeUnionConverter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class TariffInfoUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TariffInfoUnion) || t == typeof(TariffInfoUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<TariffInfo>(reader);
                    return new TariffInfoUnion { PurpleTariffInfo = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<TariffInfo>>(reader);
                    return new TariffInfoUnion { TariffInfoElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type TariffInfoUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (TariffInfoUnion)untypedValue;
            if (value.TariffInfoElementArray != null)
            {
                serializer.Serialize(writer, value.TariffInfoElementArray); return;
            }
            if (value.PurpleTariffInfo != null)
            {
                serializer.Serialize(writer, value.PurpleTariffInfo); return;
            }
            throw new Exception("Cannot marshal type TariffInfoUnion");
        }
    }
    internal class FlightDetailsUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FlightDetailsUnion) || t == typeof(FlightDetailsUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<FlightDetails>(reader);
                    return new FlightDetailsUnion { PurpleFlightDetails = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<FlightDetails>>(reader);
                    return new FlightDetailsUnion { FlightDetailsElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type TariffInfoUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (FlightDetailsUnion)untypedValue;
            if (value.FlightDetailsElementArray != null)
            {
                serializer.Serialize(writer, value.FlightDetailsElementArray); return;
            }
            if (value.PurpleFlightDetails != null)
            {
                serializer.Serialize(writer, value.PurpleFlightDetails); return;
            }
            throw new Exception("Cannot marshal type FlightDetailsUnion");
        }
    }
    internal class FlightErrorCodeUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FlightErrorCodeUnion) || t == typeof(FlightErrorCodeUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<FlightErrorCode>(reader);
                    return new FlightErrorCodeUnion { PurpleFlightErrorCode = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<FlightErrorCode>>(reader);
                    return new FlightErrorCodeUnion { FlightErrorCodeElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type FlightErrorCodeUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (FlightErrorCodeUnion)untypedValue;
            if (value.FlightErrorCodeElementArray != null)
            {
                serializer.Serialize(writer, value.FlightErrorCodeElementArray); return;
            }
            if (value.PurpleFlightErrorCode != null)
            {
                serializer.Serialize(writer, value.PurpleFlightErrorCode); return;
            }
            throw new Exception("Cannot marshal type FlightErrorCodeUnion");
        }
    }
}
