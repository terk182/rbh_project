// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BL.Entities.InformativePricing;
//
//    var response = Response.FromJson(jsonString);

namespace BL.Entities.InformativePricing
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Response
    {
        [JsonProperty("Fare_InformativePricingWithoutPNRReply")]
        public FareInformativePricingWithoutPnrReply FareInformativePricingWithoutPnrReply { get; set; }

        [JsonProperty("Error")]
        public Error Error { get; set; }
    }

    public partial class FareInformativePricingWithoutPnrReply
    {
        [JsonProperty("xmlns")]
        public string Xmlns { get; set; }

        [JsonProperty("messageDetails")]
        public MessageDetails MessageDetails { get; set; }

        [JsonProperty("mainGroup")]
        public MainGroup MainGroup { get; set; }

        [JsonProperty("Session")]
        public Session Session { get; set; }
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

    public partial class MainGroup
    {
        [JsonProperty("dummySegment")]
        public string DummySegment { get; set; }

        [JsonProperty("convertionRate")]
        public ConvertionRate ConvertionRate { get; set; }

        [JsonProperty("generalIndicatorsGroup")]
        public GeneralIndicatorsGroup GeneralIndicatorsGroup { get; set; }

        [JsonProperty("pricingGroupLevelGroup")]
        public PricingGroupLevelGroupUnion PricingGroupLevelGroup { get; set; }
    }


    public partial struct PricingGroupLevelGroupUnion
    {
        public List<PricingGroupLevelGroup> PricingGroupLevelGroupElementArray;
        public PricingGroupLevelGroup PurplePricingGroupLevelGroup;

        public bool IsNull => PricingGroupLevelGroupElementArray == null && PurplePricingGroupLevelGroup == null;
    }


    public partial class ConvertionRate
    {
        [JsonProperty("conversionRateDetails")]
        public ConversionRateDetails ConversionRateDetails { get; set; }
    }

    public partial class ConversionRateDetails
    {
        [JsonProperty("rateType")]
        public string RateType { get; set; }

        [JsonProperty("pricingAmount")]
        public string PricingAmount { get; set; }

        [JsonProperty("dutyTaxFeeType")]
        public string DutyTaxFeeType { get; set; }
    }

    public partial class GeneralIndicatorsGroup
    {
        [JsonProperty("generalIndicators")]
        public GeneralIndicators GeneralIndicators { get; set; }
    }

    public partial class GeneralIndicators
    {
        [JsonProperty("priceTicketDetails")]
        public PriceTicketDetails PriceTicketDetails { get; set; }
    }

    public partial class PriceTicketDetails
    {
        [JsonProperty("indicators")]
        public string Indicators { get; set; }
    }

    public partial class PricingGroupLevelGroup
    {
        [JsonProperty("numberOfPax")]
        public NumberOfPax NumberOfPax { get; set; }

        [JsonProperty("passengersID")]
        public PassengersId PassengersId { get; set; }

        [JsonProperty("fareInfoGroup")]
        public FareInfoGroup FareInfoGroup { get; set; }
    }

    public partial class FareInfoGroup
    {
        [JsonProperty("emptySegment")]
        public string EmptySegment { get; set; }

        [JsonProperty("pricingIndicators")]
        public PricingIndicators PricingIndicators { get; set; }

        [JsonProperty("fareAmount")]
        public FareAmount FareAmount { get; set; }

        [JsonProperty("textData")]
        public List<TextDatum> TextData { get; set; }

        [JsonProperty("surchargesGroup")]
        public SurchargesGroup SurchargesGroup { get; set; }

        [JsonProperty("segmentLevelGroup")]
        public SegmentLevelGroupUnion SegmentLevelGroup { get; set; }
    }

    public partial struct SegmentLevelGroupUnion
    {
        public List<SegmentLevelGroup> SegmentLevelGroupElementArray;
        public SegmentLevelGroup PurpleSegmentLevelGroup;

        public bool IsNull => SegmentLevelGroupElementArray == null && PurpleSegmentLevelGroup == null;
    }

    public partial class FareAmount
    {
        [JsonProperty("monetaryDetails")]
        public MonetaryDetails MonetaryDetails { get; set; }

        [JsonProperty("otherMonetaryDetails")]
        public MonetaryDetails OtherMonetaryDetails { get; set; }
    }

    public partial class MonetaryDetails
    {
        [JsonProperty("typeQualifier")]
        public string TypeQualifier { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public partial class PricingIndicators
    {
        [JsonProperty("priceTariffType")]
        public string PriceTariffType { get; set; }

        [JsonProperty("productDateTimeDetails")]
        public ProductDateTimeDetails ProductDateTimeDetails { get; set; }
    }

    public partial class ProductDateTimeDetails
    {
        [JsonProperty("departureDate")]
        public string DepartureDate { get; set; }
    }

    public partial class SegmentLevelGroup
    {
        [JsonProperty("segmentInformation")]
        public SegmentInformation SegmentInformation { get; set; }

        [JsonProperty("additionalInformation")]
        public AdditionalInformation AdditionalInformation { get; set; }

        [JsonProperty("fareBasis")]
        public FareBasis FareBasis { get; set; }

        [JsonProperty("cabinGroup")]
        public CabinGroup CabinGroup { get; set; }

        [JsonProperty("baggageAllowance")]
        public BaggageAllowance BaggageAllowance { get; set; }

        [JsonProperty("ptcSegment")]
        public PtcSegment PtcSegment { get; set; }
    }

    public partial class AdditionalInformation
    {
        [JsonProperty("priceTicketDetails", NullValueHandling = NullValueHandling.Ignore)]
        public PriceTicketDetails PriceTicketDetails { get; set; }

        [JsonProperty("productDateTimeDetails")]
        public AdditionalInformationProductDateTimeDetails ProductDateTimeDetails { get; set; }

        [JsonProperty("idNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string IdNumber { get; set; }
    }

    public partial class AdditionalInformationProductDateTimeDetails
    {
        [JsonProperty("departureDate")]
        public string DepartureDate { get; set; }

        [JsonProperty("arrivalDate")]
        public string ArrivalDate { get; set; }
    }

    public partial class BaggageAllowance
    {
        [JsonProperty("baggageDetails")]
        public BaggageDetails BaggageDetails { get; set; }
    }

    public partial class BaggageDetails
    {
        [JsonProperty("freeAllowance")]
        public string FreeAllowance { get; set; }

        [JsonProperty("quantityCode")]
        public string QuantityCode { get; set; }
    }

    public partial class CabinGroup
    {
        [JsonProperty("cabinSegment")]
        public CabinSegment CabinSegment { get; set; }
    }

    public partial class CabinSegment
    {
        [JsonProperty("bookingClassDetails")]
        public BookingClassDetails BookingClassDetails { get; set; }
    }

    public partial class BookingClassDetails
    {
        [JsonProperty("designator")]
        public string Designator { get; set; }

        [JsonProperty("option")]
        public string Option { get; set; }
    }

    public partial class FareBasis
    {
        [JsonProperty("additionalFareDetails")]
        public AdditionalFareDetails AdditionalFareDetails { get; set; }
    }

    public partial class AdditionalFareDetails
    {
        [JsonProperty("rateClass")]
        public string RateClass { get; set; }

        [JsonProperty("secondRateClass")]
        public string SecondRateClass { get; set; }
    }

    public partial class PtcSegment
    {
        [JsonProperty("quantityDetails")]
        public QuantityDetails QuantityDetails { get; set; }
    }

    public partial class QuantityDetails
    {
        [JsonProperty("numberOfUnit")]
        public string NumberOfUnit { get; set; }

        [JsonProperty("unitQualifier")]
        public string UnitQualifier { get; set; }
    }

    public partial class SegmentInformation
    {
        [JsonProperty("flightDate")]
        public ProductDateTimeDetails FlightDate { get; set; }

        [JsonProperty("boardPointDetails")]
        public PointDetails BoardPointDetails { get; set; }

        [JsonProperty("offpointDetails")]
        public PointDetails OffpointDetails { get; set; }

        [JsonProperty("companyDetails")]
        public CompanyDetails CompanyDetails { get; set; }

        [JsonProperty("flightIdentification")]
        public FlightIdentification FlightIdentification { get; set; }

        [JsonProperty("flightTypeDetails", NullValueHandling = NullValueHandling.Ignore)]
        public FlightTypeDetails FlightTypeDetails { get; set; }

        [JsonProperty("itemNumber")]
        public string ItemNumber { get; set; }
    }

    public partial class PointDetails
    {
        [JsonProperty("trueLocationId")]
        public string TrueLocationId { get; set; }
    }

    public partial class CompanyDetails
    {
        [JsonProperty("marketingCompany")]
        public string MarketingCompany { get; set; }
    }

    public partial class FlightIdentification
    {
        [JsonProperty("flightNumber")]
        public string FlightNumber { get; set; }

        [JsonProperty("bookingClass")]
        public string BookingClass { get; set; }

        [JsonProperty("operationalSuffix")]
        public string OperationalSuffix { get; set; }
    }

    public partial class FlightTypeDetails
    {
        [JsonProperty("flightIndicator")]
        public string FlightIndicator { get; set; }
    }

    public partial class SurchargesGroup
    {
        [JsonProperty("taxesAmount")]
        public TaxesAmount TaxesAmount { get; set; }
    }

    public partial class TaxesAmount
    {
        [JsonProperty("taxDetails")]
        public TaxDetails TaxDetails { get; set; }
    }

    public partial struct TaxDetails
    {
        public TaxDetail TaxDetail;
        public List<TaxDetail> TaxDetailArray;

        public bool IsNull => TaxDetailArray == null && TaxDetail == null;
    }

    public partial class TaxDetail
    {
        [JsonProperty("rate")]
        public string Rate { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class TextDatum
    {
        [JsonProperty("freeTextQualification")]
        public FreeTextQualification FreeTextQualification { get; set; }

        [JsonProperty("freeText")]
        public FreeText FreeText { get; set; }
    }

    public partial class FreeTextQualification
    {
        [JsonProperty("textSubjectQualifier")]
        public string TextSubjectQualifier { get; set; }

        [JsonProperty("informationType")]
        public string InformationType { get; set; }
    }

    public partial class NumberOfPax
    {
        [JsonProperty("segmentControlDetails")]
        public SegmentControlDetails SegmentControlDetails { get; set; }
    }

    public partial class SegmentControlDetails
    {
        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("numberOfUnits")]
        public string NumberOfUnits { get; set; }
    }

    public partial class PassengersId
    {
        [JsonProperty("travellerDetails")]
        public TravellerDetails TravellerDetails { get; set; }
    }

    public partial class TravellerDetail
    {
        [JsonProperty("measurementValue")]
        public string MeasurementValue { get; set; }
    }

    public partial class MessageDetails
    {
        [JsonProperty("messageFunctionDetails")]
        public MessageFunctionDetails MessageFunctionDetails { get; set; }

        [JsonProperty("responseType")]
        public string ResponseType { get; set; }
    }

    public partial class MessageFunctionDetails
    {
        [JsonProperty("businessFunction")]
        public string BusinessFunction { get; set; }

        [JsonProperty("messageFunction")]
        public string MessageFunction { get; set; }

        [JsonProperty("responsibleAgency")]
        public string ResponsibleAgency { get; set; }
    }

    public partial class Session
    {
        [JsonProperty("TransactionStatusCode")]
        public string TransactionStatusCode { get; set; }
        
    }

    public partial struct FreeText
    {
        public string String;
        public List<string> StringArray;

        public bool IsNull => StringArray == null && String == null;
    }

    public partial struct TravellerDetails
    {
        public TravellerDetail TravellerDetail;
        public List<TravellerDetail> TravellerDetailArray;

        public bool IsNull => TravellerDetailArray == null && TravellerDetail == null;
    }

    public partial class Response
    {
        public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, BL.Entities.InformativePricing.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Response self) => JsonConvert.SerializeObject(self, BL.Entities.InformativePricing.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new FreeTextConverter(),
                new TravellerDetailsConverter(),
                new TaxDetailsConverter(),
                new PricingGroupLevelGroupUnionConverter(),
                new SegmentLevelGroupUnionConverter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class FreeTextConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FreeText) || t == typeof(FreeText?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new FreeText { String = stringValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<string>>(reader);
                    return new FreeText { StringArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type FreeText");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (FreeText)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String); return;
            }
            if (value.StringArray != null)
            {
                serializer.Serialize(writer, value.StringArray); return;
            }
            throw new Exception("Cannot marshal type FreeText");
        }
    }

    internal class TravellerDetailsConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TravellerDetails) || t == typeof(TravellerDetails?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<TravellerDetail>(reader);
                    return new TravellerDetails { TravellerDetail = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<TravellerDetail>>(reader);
                    return new TravellerDetails { TravellerDetailArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type TravellerDetails");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (TravellerDetails)untypedValue;
            if (value.TravellerDetailArray != null)
            {
                serializer.Serialize(writer, value.TravellerDetailArray); return;
            }
            if (value.TravellerDetail != null)
            {
                serializer.Serialize(writer, value.TravellerDetail); return;
            }
            throw new Exception("Cannot marshal type TravellerDetails");
        }
    }

    internal class PricingGroupLevelGroupUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(PricingGroupLevelGroupUnion) || t == typeof(PricingGroupLevelGroupUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<PricingGroupLevelGroup>(reader);
                    return new PricingGroupLevelGroupUnion { PurplePricingGroupLevelGroup = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<PricingGroupLevelGroup>>(reader);
                    return new PricingGroupLevelGroupUnion { PricingGroupLevelGroupElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type PricingGroupLevelGroupUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (PricingGroupLevelGroupUnion)untypedValue;
            if (value.PricingGroupLevelGroupElementArray != null)
            {
                serializer.Serialize(writer, value.PricingGroupLevelGroupElementArray); return;
            }
            if (value.PurplePricingGroupLevelGroup != null)
            {
                serializer.Serialize(writer, value.PurplePricingGroupLevelGroup); return;
            }
            throw new Exception("Cannot marshal type PricingGroupLevelGroupUnion");
        }
    }


    internal class SegmentLevelGroupUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SegmentLevelGroupUnion) || t == typeof(SegmentLevelGroupUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<SegmentLevelGroup>(reader);
                    return new SegmentLevelGroupUnion { PurpleSegmentLevelGroup = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<SegmentLevelGroup>>(reader);
                    return new SegmentLevelGroupUnion { SegmentLevelGroupElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type SegmentLevelGroupUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (SegmentLevelGroupUnion)untypedValue;
            if (value.SegmentLevelGroupElementArray != null)
            {
                serializer.Serialize(writer, value.SegmentLevelGroupElementArray); return;
            }
            if (value.PurpleSegmentLevelGroup != null)
            {
                serializer.Serialize(writer, value.PurpleSegmentLevelGroup); return;
            }
            throw new Exception("Cannot marshal type SegmentLevelGroupUnion");
        }
    }

    internal class TaxDetailsConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TaxDetails) || t == typeof(TaxDetails?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<TaxDetail>(reader);
                    return new TaxDetails { TaxDetail = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<TaxDetail>>(reader);
                    return new TaxDetails { TaxDetailArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type TaxDetails");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (TaxDetails)untypedValue;
            if (value.TaxDetailArray != null)
            {
                serializer.Serialize(writer, value.TaxDetailArray); return;
            }
            if (value.TaxDetail != null)
            {
                serializer.Serialize(writer, value.TaxDetail); return;
            }
            throw new Exception("Cannot marshal type TaxDetails");
        }
    }
}
