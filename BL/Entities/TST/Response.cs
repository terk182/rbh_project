// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BL.Entities.TST;
//
//    var response = Response.FromJson(jsonString);

namespace BL.Entities.TST
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Response
    {
        [JsonProperty("Ticket_CreateTSTFromPricingReply")]
        public TicketCreateTstFromPricingReply TicketCreateTstFromPricingReply { get; set; }

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

    public partial class TicketCreateTstFromPricingReply
    {
        [JsonProperty("xmlns")]
        public string Xmlns { get; set; }

        [JsonProperty("tstList")]
        public TstListUnion TstList { get; set; }

        [JsonProperty("Session")]
        public Session Session { get; set; }
    }

    public partial struct TstListUnion
    {
        public List<TstList> TstListElementArray;
        public TstList PurpleTstList;

        public bool IsNull => TstListElementArray == null && PurpleTstList == null;
    }

    public partial class Session
    {
        [JsonProperty("TransactionStatusCode")]
        public string TransactionStatusCode { get; set; }
        
    }

    public partial class TstList
    {
        [JsonProperty("tstReference")]
        public TstReference TstReference { get; set; }

        [JsonProperty("paxInformation")]
        public PaxInformation PaxInformation { get; set; }
    }

    public partial class PaxInformation
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

    public partial class TstReference
    {
        [JsonProperty("referenceType")]
        public string ReferenceType { get; set; }

        [JsonProperty("uniqueReference")]
        public string UniqueReference { get; set; }

        [JsonProperty("iDDescription")]
        public IDDescription IDDescription { get; set; }
    }

    public partial class IDDescription
    {
        [JsonProperty("iDSequenceNumber")]
        public string IDSequenceNumber { get; set; }
    }

    public partial struct RefDetails
    {
        public RefDetail RefDetail;
        public List<RefDetail> RefDetailArray;

        public bool IsNull => RefDetailArray == null && RefDetail == null;
    }
    

    public partial class Response
    {
        public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, BL.Entities.TST.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Response self) => JsonConvert.SerializeObject(self, BL.Entities.TST.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new RefDetailsConverter(),
                new TstListUnionConverter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
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

    internal class TstListUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TstListUnion) || t == typeof(TstListUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<TstList>(reader);
                    return new TstListUnion { PurpleTstList = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<TstList>>(reader);
                    return new TstListUnion { TstListElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type TstListUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (TstListUnion)untypedValue;
            if (value.TstListElementArray != null)
            {
                serializer.Serialize(writer, value.TstListElementArray); return;
            }
            if (value.PurpleTstList != null)
            {
                serializer.Serialize(writer, value.PurpleTstList); return;
            }
            throw new Exception("Cannot marshal type TstListUnion");
        }
    }
    
}
