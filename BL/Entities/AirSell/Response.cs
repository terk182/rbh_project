// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BL.Entities.AirSell;
//
//    var response = Response.FromJson(jsonString);

namespace BL.Entities.AirSell
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Response
    {
        [JsonProperty("Air_SellFromRecommendationReply")]
        public AirSellFromRecommendationReply AirSellFromRecommendationReply { get; set; }

        [JsonProperty("Error")]
        public Error Error { get; set; }
    }

    public partial class AirSellFromRecommendationReply
    {
        [JsonProperty("xmlns")]
        public string Xmlns { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("itineraryDetails")]
        public ItineraryDetailUnion ItineraryDetails { get; set; }

        [JsonProperty("Session")]
        public Session Session { get; set; }
    }
    public partial struct ItineraryDetailUnion
    {
        public List<ItineraryDetail> ItineraryDetailElementArray;
        public ItineraryDetail PurpleItineraryDetail;

        public bool IsNull => ItineraryDetailElementArray == null && PurpleItineraryDetail == null;
    }

    public partial class ItineraryDetail
    {
        [JsonProperty("originDestination")]
        public OriginDestination OriginDestination { get; set; }

        [JsonProperty("segmentInformation")]
        public SegmentInformationUnion SegmentInformation { get; set; }
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

    public partial class OriginDestination
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }
    }

    public partial class SegmentInformationElement
    {
        [JsonProperty("flightDetails")]
        public FlightDetails FlightDetails { get; set; }

        [JsonProperty("apdSegment")]
        public PurpleApdSegment ApdSegment { get; set; }

        [JsonProperty("actionDetails")]
        public ActionDetails ActionDetails { get; set; }
    }
    public partial class ActionDetails
    {
        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("statusCode")]
        public string StatusCode { get; set; }
    }

    public partial class PurpleApdSegment
    {
        [JsonProperty("legDetails")]
        public LegDetails LegDetails { get; set; }

        [JsonProperty("arrivalStationInfo")]
        public StationInfo ArrivalStationInfo { get; set; }

        [JsonProperty("departureStationInfo", NullValueHandling = NullValueHandling.Ignore)]
        public StationInfo DepartureStationInfo { get; set; }
    }


    public partial class StationInfo
    {
        [JsonProperty("terminal")]
        public string Terminal { get; set; }
    }

    public partial class LegDetails
    {
        [JsonProperty("equipment")]
        public string Equipment { get; set; }
    }

    public partial class FlightDetails
    {
        [JsonProperty("flightDate")]
        public FlightDate FlightDate { get; set; }

        [JsonProperty("boardPointDetails")]
        public PointDetails BoardPointDetails { get; set; }

        [JsonProperty("offpointDetails")]
        public PointDetails OffpointDetails { get; set; }

        [JsonProperty("companyDetails")]
        public CompanyDetails CompanyDetails { get; set; }

        [JsonProperty("flightIdentification")]
        public FlightIdentification FlightIdentification { get; set; }

        [JsonProperty("flightTypeDetails")]
        public FlightTypeDetails FlightTypeDetails { get; set; }

        [JsonProperty("specialSegment")]
        public string SpecialSegment { get; set; }
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

    public partial class FlightDate
    {
        [JsonProperty("departureDate")]
        public string DepartureDate { get; set; }

        [JsonProperty("departureTime")]
        public string DepartureTime { get; set; }

        [JsonProperty("arrivalTime")]
        public string ArrivalTime { get; set; }
    }

    public partial class FlightIdentification
    {
        [JsonProperty("flightNumber")]
        public string FlightNumber { get; set; }

        [JsonProperty("bookingClass")]
        public string BookingClass { get; set; }
    }

    public partial class FlightTypeDetails
    {
        [JsonProperty("flightIndicator")]
        public string FlightIndicator { get; set; }
    }

    public partial class PurpleSegmentInformation
    {
        [JsonProperty("flightDetails")]
        public FlightDetails FlightDetails { get; set; }

        [JsonProperty("apdSegment")]
        public FluffyApdSegment ApdSegment { get; set; }

        [JsonProperty("actionDetails")]
        public ActionDetails ActionDetails { get; set; }
    }

    public partial class FluffyApdSegment
    {
        [JsonProperty("legDetails")]
        public LegDetails LegDetails { get; set; }

        [JsonProperty("departureStationInfo")]
        public StationInfo DepartureStationInfo { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("messageFunctionDetails")]
        public MessageFunctionDetails MessageFunctionDetails { get; set; }
    }

    public partial class MessageFunctionDetails
    {
        [JsonProperty("messageFunction")]
        public string MessageFunction { get; set; }
    }

    public partial class Session
    {
        [JsonProperty("TransactionStatusCode")]
        public string TransactionStatusCode { get; set; }

        [JsonProperty("SessionId")]
        public string SessionId { get; set; }

        [JsonProperty("SequenceNumber")]
        public string SequenceNumber { get; set; }

        [JsonProperty("SecurityToken")]
        public string SecurityToken { get; set; }

        [JsonProperty("isStateFull")]
        public bool isStateFull { get; set; }

        [JsonProperty("InSeries")]
        public bool InSeries { get; set; }

        [JsonProperty("End")]
        public bool End { get; set; }
    }

    public partial struct SegmentInformationUnion
    {
        public PurpleSegmentInformation PurpleSegmentInformation;
        public List<SegmentInformationElement> SegmentInformationElementArray;

        public static implicit operator SegmentInformationUnion(PurpleSegmentInformation PurpleSegmentInformation) => new SegmentInformationUnion { PurpleSegmentInformation = PurpleSegmentInformation };
        public static implicit operator SegmentInformationUnion(List<SegmentInformationElement> SegmentInformationElementArray) => new SegmentInformationUnion { SegmentInformationElementArray = SegmentInformationElementArray };
    }

    public partial class Response
    {
        public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, BL.Entities.AirSell.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Response self) => JsonConvert.SerializeObject(self, BL.Entities.AirSell.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new ItineraryDetailUnionConverter(),
                new SegmentInformationUnionConverter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class ItineraryDetailUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ItineraryDetailUnion) || t == typeof(ItineraryDetailUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<ItineraryDetail>(reader);
                    return new ItineraryDetailUnion { PurpleItineraryDetail = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<ItineraryDetail>>(reader);
                    return new ItineraryDetailUnion { ItineraryDetailElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type ItineraryDetailUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ItineraryDetailUnion)untypedValue;
            if (value.ItineraryDetailElementArray != null)
            {
                serializer.Serialize(writer, value.ItineraryDetailElementArray); return;
            }
            if (value.PurpleItineraryDetail != null)
            {
                serializer.Serialize(writer, value.PurpleItineraryDetail); return;
            }
            throw new Exception("Cannot marshal type ItineraryDetailUnion");
        }
    }
    internal class SegmentInformationUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SegmentInformationUnion) || t == typeof(SegmentInformationUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<PurpleSegmentInformation>(reader);
                    return new SegmentInformationUnion { PurpleSegmentInformation = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<SegmentInformationElement>>(reader);
                    return new SegmentInformationUnion { SegmentInformationElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type SegmentInformationUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (SegmentInformationUnion)untypedValue;
            if (value.SegmentInformationElementArray != null)
            {
                serializer.Serialize(writer, value.SegmentInformationElementArray);
                return;
            }
            if (value.PurpleSegmentInformation != null)
            {
                serializer.Serialize(writer, value.PurpleSegmentInformation);
                return;
            }
            throw new Exception("Cannot marshal type SegmentInformationUnion");
        }

        public static readonly SegmentInformationUnionConverter Singleton = new SegmentInformationUnionConverter();
    }
}
