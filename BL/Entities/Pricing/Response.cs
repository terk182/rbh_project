// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BL.Entities.Pricing;
//
//    var response = Response.FromJson(jsonString);

namespace BL.Entities.Pricing
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Response
    {
        [JsonProperty("Fare_PricePNRWithBookingClassReply")]
        public FarePricePnrWithBookingClassReply FarePricePnrWithBookingClassReply { get; set; }

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

    public partial class FarePricePnrWithBookingClassReply
    {
        [JsonProperty("xmlns")]
        public string Xmlns { get; set; }

        [JsonProperty("fareList")]
        public FareListUnion FareList { get; set; }

        [JsonProperty("Session")]
        public Session Session { get; set; }
    }

    public partial struct FareListUnion
    {
        public List<FareList> FareListElementArray;
        public FareList PurpleFareList;

        public bool IsNull => FareListElementArray == null && PurpleFareList == null;
    }

    public partial class FareList
    {
        [JsonProperty("pricingInformation")]
        public PricingInformation PricingInformation { get; set; }

        [JsonProperty("fareReference")]
        public FareReference FareReference { get; set; }

        [JsonProperty("lastTktDate")]
        public LastTktDate LastTktDate { get; set; }

        [JsonProperty("validatingCarrier")]
        public ValidatingCarrier ValidatingCarrier { get; set; }

        [JsonProperty("paxSegReference")]
        public PaxSegReference PaxSegReference { get; set; }

        [JsonProperty("fareDataInformation")]
        public FareDataInformation FareDataInformation { get; set; }

        [JsonProperty("taxInformation")]
        public TaxInformations TaxInformations { get; set; }

        [JsonProperty("originDestination")]
        public OriginDestination OriginDestination { get; set; }

        [JsonProperty("segmentInformation")]
        public SegmentInformationUnion SegmentInformation { get; set; }

        [JsonProperty("otherPricingInfo")]
        public OtherPricingInfo OtherPricingInfo { get; set; }

        [JsonProperty("warningInformation")]
        public List<WarningInformation> WarningInformation { get; set; }
    }

    public partial struct SegmentInformationUnion
    {
        public List<SegmentInformation> SegmentInformationElementArray;
        public SegmentInformation PurpleSegmentInformation;

        public bool IsNull => SegmentInformationElementArray == null && PurpleSegmentInformation == null;
    }

    public partial struct TaxInformations
    {
        public TaxInformation TaxInformation;
        public List<TaxInformation> TaxInformationArray;

        public bool IsNull => TaxInformationArray == null && TaxInformation == null;
    }


    public partial class FareDataInformation
    {
        [JsonProperty("fareDataMainInformation")]
        public FareDataMainInformation FareDataMainInformation { get; set; }

        [JsonProperty("fareDataSupInformation")]
        public List<FareDataSupInformationElement> FareDataSupInformation { get; set; }
    }

    public partial class FareDataMainInformation
    {
        [JsonProperty("fareDataQualifier")]
        public string FareDataQualifier { get; set; }
    }

    public partial class FareDataSupInformationElement
    {
        [JsonProperty("fareDataQualifier")]
        public string FareDataQualifier { get; set; }

        [JsonProperty("fareAmount")]
        public string FareAmount { get; set; }

        [JsonProperty("fareCurrency")]
        public string FareCurrency { get; set; }
    }

    public partial class FareReference
    {
        [JsonProperty("referenceType")]
        public string ReferenceType { get; set; }

        [JsonProperty("uniqueReference")]
        public string UniqueReference { get; set; }
    }

    public partial class LastTktDate
    {
        [JsonProperty("businessSemantic")]
        public Text BusinessSemantic { get; set; }

        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }
    }

    public partial struct Text
    {
        public string String;
        public List<string> StringArray;

        public bool IsNull => StringArray == null && String == null;
    }

    public partial class DateTime
    {
        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("month")]
        public string Month { get; set; }

        [JsonProperty("day")]
        public string Day { get; set; }
    }

    public partial class OriginDestination
    {
        [JsonProperty("cityCode")]
        public List<string> CityCode { get; set; }
    }

    public partial class OtherPricingInfo
    {
        [JsonProperty("attributeDetails")]
        public AttributeDetailUnion AttributeDetails { get; set; }
    }
    public partial struct AttributeDetailUnion
    {
        public List<AttributeDetail> AttributeDetailElementArray;
        public AttributeDetail PurpleAttributeDetail;

        public bool IsNull => AttributeDetailElementArray == null && PurpleAttributeDetail == null;
    }

    public partial class AttributeDetail
    {
        [JsonProperty("attributeType")]
        public string AttributeType { get; set; }

        [JsonProperty("attributeDescription")]
        public string AttributeDescription { get; set; }
    }

    public partial class PaxSegReference
    {
        [JsonProperty("refDetails")]
        public RefDetails RefDetails { get; set; }
    }

    public partial class RefDetail
    {
        [JsonProperty("refQualifier")]
        public string RefQualifier { get; set; }

        [JsonProperty("refNumber")]
        public string RefNumber { get; set; }
    }

    public partial class PricingInformation
    {
        [JsonProperty("tstInformation")]
        public TstInformation TstInformation { get; set; }

        [JsonProperty("fcmi")]
        public string Fcmi { get; set; }
    }

    public partial class TstInformation
    {
        [JsonProperty("tstIndicator")]
        public string TstIndicator { get; set; }
    }

    public partial class SegmentInformation
    {
        [JsonProperty("connexInformation")]
        public ConnexInformation ConnexInformation { get; set; }

        [JsonProperty("segDetails")]
        public SegDetails SegDetails { get; set; }

        [JsonProperty("fareQualifier")]
        public FareQualifier FareQualifier { get; set; }

        [JsonProperty("validityInformation")]
        public LastTktDateUnion ValidityInformation { get; set; }

        [JsonProperty("bagAllowanceInformation")]
        public BagAllowanceInformation BagAllowanceInformation { get; set; }

        [JsonProperty("segmentReference")]
        public SegmentReference SegmentReference { get; set; }

        [JsonProperty("sequenceInformation")]
        public SequenceInformation SequenceInformation { get; set; }
    }


    public partial struct LastTktDateUnion
    {
        public List<LastTktDate> LastTktDateElementArray;
        public LastTktDate PurpleLastTktDate;

        public bool IsNull => LastTktDateElementArray == null && PurpleLastTktDate == null;
    }


    public partial class BagAllowanceInformation
    {
        [JsonProperty("bagAllowanceDetails")]
        public BagAllowanceDetails BagAllowanceDetails { get; set; }
    }

    public partial class BagAllowanceDetails
    {
        [JsonProperty("baggageWeight")]
        public string BaggageWeight { get; set; }

        [JsonProperty("baggageType")]
        public string BaggageType { get; set; }

        [JsonProperty("measureUnit")]
        public string MeasureUnit { get; set; }
    }

    public partial class ConnexInformation
    {
        [JsonProperty("connecDetails")]
        public ConnecDetails ConnecDetails { get; set; }
    }

    public partial class ConnecDetails
    {
        [JsonProperty("connexType")]
        public string ConnexType { get; set; }
    }

    public partial class FareQualifier
    {
        [JsonProperty("fareBasisDetails")]
        public FareBasisDetails FareBasisDetails { get; set; }
    }

    public partial class FareBasisDetails
    {
        [JsonProperty("primaryCode")]
        public string PrimaryCode { get; set; }

        [JsonProperty("fareBasisCode")]
        public string FareBasisCode { get; set; }

        [JsonProperty("discTktDesignator")]
        public string DiscTktDesignator { get; set; }
    }

    public partial class SegDetails
    {
        [JsonProperty("segmentDetail")]
        public SegmentDetail SegmentDetail { get; set; }
    }

    public partial class SegmentDetail
    {
        [JsonProperty("identification")]
        public string Identification { get; set; }

        [JsonProperty("classOfService")]
        public string ClassOfService { get; set; }
    }

    public partial class SegmentReference
    {
        [JsonProperty("refDetails")]
        public RefDetail RefDetails { get; set; }
    }

    public partial class SequenceInformation
    {
        [JsonProperty("sequenceSection")]
        public SequenceSection SequenceSection { get; set; }
    }

    public partial class SequenceSection
    {
        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }
    }

    public partial class TaxInformation
    {
        [JsonProperty("taxDetails")]
        public TaxDetails TaxDetails { get; set; }

        [JsonProperty("amountDetails")]
        public AmountDetails AmountDetails { get; set; }
    }

    public partial class AmountDetails
    {
        [JsonProperty("fareDataMainInformation")]
        public FareDataSupInformationElement FareDataMainInformation { get; set; }
    }

    public partial class TaxDetails
    {
        [JsonProperty("taxQualifier")]
        public string TaxQualifier { get; set; }

        [JsonProperty("taxIdentification")]
        public TaxIdentification TaxIdentification { get; set; }

        [JsonProperty("taxType")]
        public TaxType TaxType { get; set; }

        [JsonProperty("taxNature")]
        public string TaxNature { get; set; }
    }

    public partial class TaxIdentification
    {
        [JsonProperty("taxIdentifier")]
        public string TaxIdentifier { get; set; }
    }

    public partial class TaxType
    {
        [JsonProperty("isoCountry")]
        public string IsoCountry { get; set; }
    }

    public partial class ValidatingCarrier
    {
        [JsonProperty("carrierInformation")]
        public CarrierInformation CarrierInformation { get; set; }
    }

    public partial class CarrierInformation
    {
        [JsonProperty("carrierCode")]
        public string CarrierCode { get; set; }
    }

    public partial class WarningInformation
    {
        [JsonProperty("warningCode")]
        public WarningCode WarningCode { get; set; }

        [JsonProperty("warningText")]
        public WarningText WarningText { get; set; }
    }

    public partial class WarningCode
    {
        [JsonProperty("applicationErrorDetail")]
        public ApplicationErrorDetail ApplicationErrorDetail { get; set; }
    }

    public partial class ApplicationErrorDetail
    {
        [JsonProperty("applicationErrorCode")]
        public string ApplicationErrorCode { get; set; }

        [JsonProperty("codeListQualifier")]
        public string CodeListQualifier { get; set; }

        [JsonProperty("codeListResponsibleAgency")]
        public string CodeListResponsibleAgency { get; set; }
    }

    public partial class WarningText
    {
        [JsonProperty("errorFreeText")]
        public string ErrorFreeText { get; set; }
    }

    public partial class Session
    {
        [JsonProperty("TransactionStatusCode")]
        public string TransactionStatusCode { get; set; }
        
    }

    public partial struct RefDetails
    {
        public RefDetail RefDetail;
        public List<RefDetail> RefDetailArray;

        public bool IsNull => RefDetailArray == null && RefDetail == null;
    }

    public partial class Response
    {
        public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, BL.Entities.Pricing.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Response self) => JsonConvert.SerializeObject(self, BL.Entities.Pricing.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new TextConverter(),
                new RefDetailsConverter(),
                new FareListUnionConverter(),
                new SegmentInformationUnionConverter(),
                new TaxInformationsConverter(),
                new LastTktDateUnionConverter(),
                new AttributeDetailUnionConverter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class TextConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Text) || t == typeof(Text?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new Text { String = stringValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<string>>(reader);
                    return new Text { StringArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type Text");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Text)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String); return;
            }
            if (value.StringArray != null)
            {
                serializer.Serialize(writer, value.StringArray); return;
            }
            throw new Exception("Cannot marshal type Text");
        }
    }

    internal class RefDetailsConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(RefDetails) || t == typeof(RefDetails?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<RefDetail>(reader);
                    return new RefDetails { RefDetail = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<RefDetail>>(reader);
                    return new RefDetails { RefDetailArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type RefDetails");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (RefDetails)untypedValue;
            if (value.RefDetailArray != null)
            {
                serializer.Serialize(writer, value.RefDetailArray); return;
            }
            if (value.RefDetail != null)
            {
                serializer.Serialize(writer, value.RefDetail); return;
            }
            throw new Exception("Cannot marshal type RefDetails");
        }
    }

    internal class FareListUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FareListUnion) || t == typeof(FareListUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<FareList>(reader);
                    return new FareListUnion { PurpleFareList = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<FareList>>(reader);
                    return new FareListUnion { FareListElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type FareListUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (FareListUnion)untypedValue;
            if (value.FareListElementArray != null)
            {
                serializer.Serialize(writer, value.FareListElementArray); return;
            }
            if (value.PurpleFareList != null)
            {
                serializer.Serialize(writer, value.PurpleFareList); return;
            }
            throw new Exception("Cannot marshal type FareListUnion");
        }
    }


    internal class SegmentInformationUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SegmentInformationUnion) || t == typeof(SegmentInformationUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<SegmentInformation>(reader);
                    return new SegmentInformationUnion { PurpleSegmentInformation = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<SegmentInformation>>(reader);
                    return new SegmentInformationUnion { SegmentInformationElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type SegmentInformationUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (SegmentInformationUnion)untypedValue;
            if (value.SegmentInformationElementArray != null)
            {
                serializer.Serialize(writer, value.SegmentInformationElementArray); return;
            }
            if (value.PurpleSegmentInformation != null)
            {
                serializer.Serialize(writer, value.PurpleSegmentInformation); return;
            }
            throw new Exception("Cannot marshal type SegmentInformationUnion");
        }
    }

    internal class LastTktDateUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(LastTktDateUnion) || t == typeof(LastTktDateUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<LastTktDate>(reader);
                    return new LastTktDateUnion { PurpleLastTktDate = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<LastTktDate>>(reader);
                    return new LastTktDateUnion { LastTktDateElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type LastTktDateUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (LastTktDateUnion)untypedValue;
            if (value.LastTktDateElementArray != null)
            {
                serializer.Serialize(writer, value.LastTktDateElementArray); return;
            }
            if (value.PurpleLastTktDate != null)
            {
                serializer.Serialize(writer, value.PurpleLastTktDate); return;
            }
            throw new Exception("Cannot marshal type LastTktDateUnion");
        }
    }
    internal class AttributeDetailUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AttributeDetailUnion) || t == typeof(AttributeDetailUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<AttributeDetail>(reader);
                    return new AttributeDetailUnion { PurpleAttributeDetail = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<AttributeDetail>>(reader);
                    return new AttributeDetailUnion { AttributeDetailElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type AttributeDetailUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (LastTktDateUnion)untypedValue;
            if (value.LastTktDateElementArray != null)
            {
                serializer.Serialize(writer, value.LastTktDateElementArray); return;
            }
            if (value.PurpleLastTktDate != null)
            {
                serializer.Serialize(writer, value.PurpleLastTktDate); return;
            }
            throw new Exception("Cannot marshal type LastTktDateUnion");
        }
    }
    
    internal class TaxInformationsConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TaxInformations) || t == typeof(TaxInformations?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<TaxInformation>(reader);
                    return new TaxInformations { TaxInformation = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<TaxInformation>>(reader);
                    return new TaxInformations { TaxInformationArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type TaxInformations");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (TaxInformations)untypedValue;
            if (value.TaxInformationArray != null)
            {
                serializer.Serialize(writer, value.TaxInformationArray); return;
            }
            if (value.TaxInformation != null)
            {
                serializer.Serialize(writer, value.TaxInformation); return;
            }
            throw new Exception("Cannot marshal type TaxInformations");
        }
    }
}
