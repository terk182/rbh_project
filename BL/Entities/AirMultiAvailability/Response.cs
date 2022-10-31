namespace BL.Entities.AirMultiAvailability
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Response
    {
        [JsonProperty("Air_MultiAvailabilityReply")]
        public AirMultiAvailabilityReply AirMultiAvailabilityReply { get; set; }
    }

    public partial class AirMultiAvailabilityReply
    {
        [JsonProperty("@xmlns")]
        public string Xmlns { get; set; }

        [JsonProperty("messageActionDetails")]
        public MessageActionDetails MessageActionDetails { get; set; }

        [JsonProperty("singleCityPairInfo")]
        public SingleCityPairInfo SingleCityPairInfo { get; set; }//List<SingleCityPairInfo> 

        [JsonProperty("Session")]
        public SessionReply Session { get; set; }
    }

    public partial class MessageActionDetails
    {
        [JsonProperty("functionDetails")]
        public FunctionDetails FunctionDetails { get; set; }

        [JsonProperty("responseType")]
        public string ResponseType { get; set; }
    }

    public partial class FunctionDetails
    {
        [JsonProperty("businessFunction")]
        public int BusinessFunction { get; set; }

        [JsonProperty("actionCode")]
        public int ActionCode { get; set; }
    }

    public partial class SessionReply
    {
        [JsonProperty("@TransactionStatusCode")]
        public string TransactionStatusCode { get; set; }

        [JsonProperty("SessionId")]
        public string SessionId { get; set; }

        [JsonProperty("SequenceNumber")]
        public int SequenceNumber { get; set; }

        [JsonProperty("SecurityToken")]
        public string SecurityToken { get; set; }
    }

    public partial class SingleCityPairInfos
    {
        [JsonProperty("locationDetails")]
        public LocationDetails LocationDetails { get; set; }

        [JsonProperty("cityPairFreeFlowText")]
        public List<CityPairFreeFlowText> CityPairFreeFlowText { get; set; }

        [JsonProperty("flightInfo")]
        public FlightInfo FlightInfo { get; set; }//List<FlightInfo> 
    }

    public partial class CityPairFreeFlowText
    {
        [JsonProperty("freeTextQualification")]
        public FreeTextQualification FreeTextQualification { get; set; }

        [JsonProperty("freeText")]
        public string FreeText { get; set; }
    }

    public partial class FreeTextQualification
    {
        [JsonProperty("codedIndicator")]
        public int CodedIndicator { get; set; }

        [JsonProperty("typeOfInfo")]
        public string TypeOfInfo { get; set; }
    }

    public partial class FlightInfos
    {
        [JsonProperty("basicFlightInfo")]
        public BasicFlightInfo BasicFlightInfo { get; set; }

        [JsonProperty("infoOnClasses")]
        public InfoOnClasses InfoOnClasses { get; set; }

        [JsonProperty("additionalFlightInfo")]
        public AdditionalFlightInfo AdditionalFlightInfo { get; set; }
    }

    public partial class AdditionalFlightInfo
    {
        [JsonProperty("flightDetails")]
        public AdditionalFlightInfoFlightDetails FlightDetails { get; set; }

        [JsonProperty("arrivalStation")]
        public Station ArrivalStation { get; set; }

        [JsonProperty("productFacilities")]
        public ProductFacilities ProductFacilities { get; set; }

        [JsonProperty("departureStation", NullValueHandling = NullValueHandling.Ignore)]
        public Station DepartureStation { get; set; }
    }

    public partial class Station
    {
        [JsonProperty("terminal")]
        public string Terminal { get; set; }
    }

    public partial class AdditionalFlightInfoFlightDetails
    {
        [JsonProperty("typeOfAircraft")]
        public string TypeOfAircraft { get; set; }

        [JsonProperty("numberOfStops")]
        public int NumberOfStops { get; set; }

        [JsonProperty("legDuration")]
        public string LegDuration { get; set; }
    }

    public partial class ProductFacilit
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class BasicFlightInfo
    {
        [JsonProperty("flightDetails")]
        public BasicFlightInfoFlightDetails FlightDetails { get; set; }

        [JsonProperty("departureLocation")]
        public Location DepartureLocation { get; set; }

        [JsonProperty("arrivalLocation")]
        public Location ArrivalLocation { get; set; }

        [JsonProperty("marketingCompany")]
        public TingCompany MarketingCompany { get; set; }

        [JsonProperty("flightIdentification")]
        public FlightIdentification FlightIdentification { get; set; }

        [JsonProperty("productTypeDetail")]
        public ProductTypeDetail ProductTypeDetail { get; set; }

        [JsonProperty("lineItemNumber")]
        public int LineItemNumber { get; set; }

        [JsonProperty("operatingCompany", NullValueHandling = NullValueHandling.Ignore)]
        public TingCompany OperatingCompany { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("cityAirport")]
        public string CityAirport { get; set; }
    }

    public partial class BasicFlightInfoFlightDetails
    {
        [JsonProperty("departureDate")]
        public string DepartureDate { get; set; }

        [JsonProperty("departureTime")]
        public string DepartureTime { get; set; }

        [JsonProperty("arrivalDate")]
        public string ArrivalDate { get; set; }

        [JsonProperty("arrivalTime")]
        public string ArrivalTime { get; set; }
    }

    public partial class FlightIdentification
    {
        [JsonProperty("number")]
        public int Number { get; set; }
    }

    public partial class TingCompany
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
    }

    public partial class ProductTypeDetail
    {
        [JsonProperty("productIndicators")]
        public List<string> ProductIndicators { get; set; }
    }

    public partial class InfoOnClass
    {
        [JsonProperty("productClassDetail")]
        public ProductClassDetail ProductClassDetail { get; set; }
    }

    public partial class ProductClassDetail
    {
        [JsonProperty("serviceClass")]
        public string ServiceClass { get; set; }

        [JsonProperty("availabilityStatus")]
        public int AvailabilityStatus { get; set; }
    }

    public partial class LocationDetails
    {
        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }
    }

    public partial struct ProductFacilities
    {
        public ProductFacilit ProductFacilit;
        public List<ProductFacilit> ProductFacilitArray;

        public static implicit operator ProductFacilities(ProductFacilit ProductFacilit) => new ProductFacilities { ProductFacilit = ProductFacilit };
        public static implicit operator ProductFacilities(List<ProductFacilit> ProductFacilitArray) => new ProductFacilities { ProductFacilitArray = ProductFacilitArray };
    }

    public partial struct InfoOnClasses
    {
        public InfoOnClass InfoOnClass;
        public List<InfoOnClass> InfoOnClassArray;

        public static implicit operator InfoOnClasses(InfoOnClass InfoOnClass) => new InfoOnClasses { InfoOnClass = InfoOnClass };
        public static implicit operator InfoOnClasses(List<InfoOnClass> InfoOnClassArray) => new InfoOnClasses { InfoOnClassArray = InfoOnClassArray };
    }
    public partial struct FlightInfo
    {
        public FlightInfos FlightInfos;
        public List<FlightInfos> FlightInfosArray;

        public static implicit operator FlightInfo(FlightInfos FlightInfos) => new FlightInfo { FlightInfos = FlightInfos };
        public static implicit operator FlightInfo(List<FlightInfos> FlightInfosArray) => new FlightInfo { FlightInfosArray = FlightInfosArray };
    }

    public partial struct SingleCityPairInfo
    {
        public SingleCityPairInfos SingleCityPairInfos;
        public List<SingleCityPairInfos> SingleCityPairInfosArray;

        public static implicit operator SingleCityPairInfo(SingleCityPairInfos SingleCityPairInfos) => new SingleCityPairInfo { SingleCityPairInfos = SingleCityPairInfos };
        public static implicit operator SingleCityPairInfo(List<SingleCityPairInfos> SingleCityPairInfosArray) => new SingleCityPairInfo { SingleCityPairInfosArray = SingleCityPairInfosArray };
    }

    public partial class Response
    {
        public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, BL.Entities.AirMultiAvailability.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Response self) => JsonConvert.SerializeObject(self, BL.Entities.AirMultiAvailability.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ProductFacilitiesConverter.Singleton,
                InfoOnClassesConverter.Singleton,
                FlightInfoConverter.Singleton,
                SingleCityPairInfoConverter.Singleton,
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

    internal class ProductFacilitiesConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ProductFacilities) || t == typeof(ProductFacilities?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<ProductFacilit>(reader);
                    return new ProductFacilities { ProductFacilit = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<ProductFacilit>>(reader);
                    return new ProductFacilities { ProductFacilitArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type ProductFacilities");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ProductFacilities)untypedValue;
            if (value.ProductFacilitArray != null)
            {
                serializer.Serialize(writer, value.ProductFacilitArray);
                return;
            }
            if (value.ProductFacilit != null)
            {
                serializer.Serialize(writer, value.ProductFacilit);
                return;
            }
            throw new Exception("Cannot marshal type ProductFacilities");
        }

        public static readonly ProductFacilitiesConverter Singleton = new ProductFacilitiesConverter();
    }

    internal class InfoOnClassesConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(InfoOnClasses) || t == typeof(InfoOnClasses?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<InfoOnClass>(reader);
                    return new InfoOnClasses { InfoOnClass = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<InfoOnClass>>(reader);
                    return new InfoOnClasses { InfoOnClassArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type InfoOnClasses");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (InfoOnClasses)untypedValue;
            if (value.InfoOnClassArray != null)
            {
                serializer.Serialize(writer, value.InfoOnClassArray);
                return;
            }
            if (value.InfoOnClass != null)
            {
                serializer.Serialize(writer, value.InfoOnClass);
                return;
            }
            throw new Exception("Cannot marshal type InfoOnClasses");
        }

        public static readonly InfoOnClassesConverter Singleton = new InfoOnClassesConverter();
    }

    internal class FlightInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FlightInfo) || t == typeof(FlightInfo?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<FlightInfos>(reader);
                    return new FlightInfo { FlightInfos = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<FlightInfos>>(reader);
                    return new FlightInfo { FlightInfosArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type FlightInfos");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (FlightInfo)untypedValue;
            if (value.FlightInfosArray != null)
            {
                serializer.Serialize(writer, value.FlightInfosArray);
                return;
            }
            if (value.FlightInfos != null)
            {
                serializer.Serialize(writer, value.FlightInfos);
                return;
            }
            throw new Exception("Cannot marshal type FlightInfos");
        }

        public static readonly FlightInfoConverter Singleton = new FlightInfoConverter();
    }
    internal class SingleCityPairInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SingleCityPairInfo) || t == typeof(SingleCityPairInfo?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<SingleCityPairInfos>(reader);
                    return new SingleCityPairInfo { SingleCityPairInfos = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<SingleCityPairInfos>>(reader);
                    return new SingleCityPairInfo { SingleCityPairInfosArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type SingleCityPairInfos");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (SingleCityPairInfo)untypedValue;
            if (value.SingleCityPairInfosArray != null)
            {
                serializer.Serialize(writer, value.SingleCityPairInfosArray);
                return;
            }
            if (value.SingleCityPairInfos != null)
            {
                serializer.Serialize(writer, value.SingleCityPairInfos);
                return;
            }
            throw new Exception("Cannot marshal type SingleCityPairInfos");
        }

        public static readonly SingleCityPairInfoConverter Singleton = new SingleCityPairInfoConverter();
    }
}
