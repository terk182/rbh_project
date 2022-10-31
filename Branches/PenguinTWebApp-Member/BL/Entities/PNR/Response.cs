// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BL.Entities.PNR;
//
//    var response = Response.FromJson(jsonString);

namespace BL.Entities.PNR
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Response
    {
        [JsonProperty("PNR_Reply")]
        public PnrReply PnrReply { get; set; }

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

    public partial class PnrReply
    {
        [JsonProperty("xmlns")]
        public string Xmlns { get; set; }

        [JsonProperty("pnrHeader")]
        public PnrHeader PnrHeader { get; set; }

        [JsonProperty("securityInformation")]
        public SecurityInformation SecurityInformation { get; set; }

        [JsonProperty("sbrPOSDetails")]
        public SbrPosDetails SbrPosDetails { get; set; }

        [JsonProperty("sbrCreationPosDetails")]
        public SbrPosDetails SbrCreationPosDetails { get; set; }

        [JsonProperty("sbrUpdatorPosDetails")]
        public SbrUpdatorPosDetails SbrUpdatorPosDetails { get; set; }

        [JsonProperty("travellerInfo")]
        public TravellerInfoUnion TravellerInfo { get; set; }

        [JsonProperty("originDestinationDetails")]
        public OriginDestinationDetails OriginDestinationDetails { get; set; }

        [JsonProperty("dataElementsMaster")]
        public DataElementsMaster DataElementsMaster { get; set; }

        [JsonProperty("Session")]
        public Session Session { get; set; }
    }

    public partial class DataElementsMaster
    {
        [JsonProperty("marker2")]
        public string Marker2 { get; set; }

        [JsonProperty("dataElementsIndiv")]
        public DataElementsIndivUnion DataElementsIndiv { get; set; }
    }

    public partial class DataElementsIndiv
    {
        [JsonProperty("elementManagementData")]
        public ElementManagement ElementManagementData { get; set; }

        [JsonProperty("otherDataFreetext", NullValueHandling = NullValueHandling.Ignore)]
        public OtherDataFreetext OtherDataFreetext { get; set; }

        [JsonProperty("serviceRequest", NullValueHandling = NullValueHandling.Ignore)]
        public ServiceRequest ServiceRequest { get; set; }

        [JsonProperty("referenceForDataElement", NullValueHandling = NullValueHandling.Ignore)]
        public ReferenceForDataElement ReferenceForDataElement { get; set; }
    }

    public partial class ElementManagement
    {
        [JsonProperty("reference")]
        public ReferenceElement Reference { get; set; }

        [JsonProperty("segmentName")]
        public string SegmentName { get; set; }

        [JsonProperty("lineNumber")]
        public string LineNumber { get; set; }
    }

    public partial class ReferenceElement
    {
        [JsonProperty("qualifier")]
        public string Qualifier { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }
    }

    public partial class OtherDataFreetext
    {
        [JsonProperty("freetextDetail")]
        public OtherDataFreetextFreetextDetail FreetextDetail { get; set; }

        [JsonProperty("longFreetext")]
        public string LongFreetext { get; set; }
    }

    public partial class OtherDataFreetextFreetextDetail
    {
        [JsonProperty("subjectQualifier")]
        public string SubjectQualifier { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class ReferenceForDataElement
    {
        [JsonProperty("reference")]
        public ReferenceUnion Reference { get; set; }
    }

    public partial class ServiceRequest
    {
        [JsonProperty("ssr")]
        public Ssr Ssr { get; set; }
    }

    public partial class Ssr
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("companyId")]
        public string CompanyId { get; set; }

        [JsonProperty("freeText")]
        public string FreeText { get; set; }
    }

    public partial class OriginDestinationDetails
    {
        [JsonProperty("originDestination")]
        public string OriginDestination { get; set; }

        [JsonProperty("itineraryInfo")]
        public ItineraryInfoUnion ItineraryInfo { get; set; }
    }


    public partial struct ItineraryInfoUnion
    {
        public List<ItineraryInfo> ItineraryInfoElementArray;
        public ItineraryInfo PurpleItineraryInfo;

        public bool IsNull => ItineraryInfoElementArray == null && PurpleItineraryInfo == null;
    }

    public partial class ItineraryInfo
    {
        [JsonProperty("elementManagementItinerary")]
        public ElementManagement ElementManagementItinerary { get; set; }

        [JsonProperty("travelProduct")]
        public TravelProduct TravelProduct { get; set; }

        [JsonProperty("itineraryMessageAction")]
        public ItineraryMessageAction ItineraryMessageAction { get; set; }

        [JsonProperty("relatedProduct")]
        public RelatedProduct RelatedProduct { get; set; }

        [JsonProperty("flightDetail")]
        public FlightDetail FlightDetail { get; set; }

        [JsonProperty("selectionDetails")]
        public SelectionDetails SelectionDetails { get; set; }

        [JsonProperty("itineraryfreeFormText")]
        public ItineraryfreeFormText ItineraryfreeFormText { get; set; }

        [JsonProperty("markerRailTour")]
        public string MarkerRailTour { get; set; }
    }

    public partial class FlightDetail
    {
        [JsonProperty("productDetails")]
        public FlightDetailProductDetails ProductDetails { get; set; }

        [JsonProperty("arrivalStationInfo", NullValueHandling = NullValueHandling.Ignore)]
        public ArrivalStationInfo ArrivalStationInfo { get; set; }

        [JsonProperty("facilities")]
        public Facilities Facilities { get; set; }

        [JsonProperty("departureInformation", NullValueHandling = NullValueHandling.Ignore)]
        public DepartureInformation DepartureInformation { get; set; }
    }

    public partial class ArrivalStationInfo
    {
        [JsonProperty("terminal")]
        public string Terminal { get; set; }
    }

    public partial class DepartureInformation
    {
        [JsonProperty("departTerminal")]
        public string DepartTerminal { get; set; }
    }

    public partial class Facilities
    {
        [JsonProperty("entertainement")]
        public string Entertainement { get; set; }

        [JsonProperty("entertainementDescription")]
        public string EntertainementDescription { get; set; }
    }

    public partial class FlightDetailProductDetails
    {
        [JsonProperty("equipment")]
        public string Equipment { get; set; }

        [JsonProperty("numOfStops")]
        public string NumOfStops { get; set; }

        [JsonProperty("weekDay")]
        public string WeekDay { get; set; }
    }

    public partial class ItineraryMessageAction
    {
        [JsonProperty("business")]
        public Business Business { get; set; }
    }

    public partial class Business
    {
        [JsonProperty("function")]
        public string Function { get; set; }
    }

    public partial class ItineraryfreeFormText
    {
        [JsonProperty("freetextDetail")]
        public ItineraryfreeFormTextFreetextDetail FreetextDetail { get; set; }

        [JsonProperty("text")]
        public Text Text { get; set; }
    }

    public partial class ItineraryfreeFormTextFreetextDetail
    {
        [JsonProperty("subjectQualifier")]
        public string SubjectQualifier { get; set; }
    }

    public partial class RelatedProduct
    {
        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public partial class SelectionDetails
    {
        [JsonProperty("selection")]
        public Selection Selection { get; set; }
    }

    public partial class Selection
    {
        [JsonProperty("option")]
        public string Option { get; set; }
    }

    public partial class TravelProduct
    {
        [JsonProperty("product")]
        public Product Product { get; set; }

        [JsonProperty("boardpointDetail")]
        public PointDetail BoardpointDetail { get; set; }

        [JsonProperty("offpointDetail")]
        public PointDetail OffpointDetail { get; set; }

        [JsonProperty("companyDetail")]
        public CompanyDetail CompanyDetail { get; set; }

        [JsonProperty("productDetails")]
        public TravelProductProductDetails ProductDetails { get; set; }

        [JsonProperty("typeDetail")]
        public TypeDetail TypeDetail { get; set; }
    }

    public partial class PointDetail
    {
        [JsonProperty("cityCode")]
        public string CityCode { get; set; }
    }

    public partial class CompanyDetail
    {
        [JsonProperty("identification")]
        public string Identification { get; set; }
    }

    public partial class Product
    {
        [JsonProperty("depDate")]
        public string DepDate { get; set; }

        [JsonProperty("depTime")]
        public string DepTime { get; set; }

        [JsonProperty("arrDate")]
        public string ArrDate { get; set; }

        [JsonProperty("arrTime")]
        public string ArrTime { get; set; }
    }

    public partial class TravelProductProductDetails
    {
        [JsonProperty("identification")]
        public string Identification { get; set; }

        [JsonProperty("classOfService")]
        public string ClassOfService { get; set; }
    }

    public partial class TypeDetail
    {
        [JsonProperty("detail")]
        public string Detail { get; set; }
    }

    public partial class PnrHeader
    {
        [JsonProperty("reservationInfo")]
        public ReservationInfo ReservationInfo { get; set; }
    }

    public partial class ReservationInfo
    {
        [JsonProperty("reservation")]
        public Reservation Reservation { get; set; }
    }

    public partial class Reservation
    {
        [JsonProperty("companyId")]
        public string CompanyId { get; set; }

        [JsonProperty("controlNumber")]
        public string ControlNumber { get; set; }

        [JsonProperty("date")]
        public string date { get; set; }

        [JsonProperty("time")]
        public string time { get; set; }
    }

    public partial class SbrPosDetails
    {
        [JsonProperty("sbrUserIdentificationOwn")]
        public SbrCreationPosDetailsSbrUserIdentificationOwn SbrUserIdentificationOwn { get; set; }

        [JsonProperty("sbrSystemDetails")]
        public SbrCreationPosDetailsSbrSystemDetails SbrSystemDetails { get; set; }

        [JsonProperty("sbrPreferences")]
        public SbrPreferences SbrPreferences { get; set; }
    }

    public partial class SbrPreferences
    {
        [JsonProperty("userPreferences")]
        public UserPreferences UserPreferences { get; set; }
    }

    public partial class UserPreferences
    {
        [JsonProperty("codedCountry")]
        public string CodedCountry { get; set; }
    }

    public partial class SbrCreationPosDetailsSbrSystemDetails
    {
        [JsonProperty("deliveringSystem")]
        public Reservation DeliveringSystem { get; set; }
    }

    public partial class SbrCreationPosDetailsSbrUserIdentificationOwn
    {
        [JsonProperty("originIdentification")]
        public OriginIdentification OriginIdentification { get; set; }
    }

    public partial class OriginIdentification
    {
        [JsonProperty("inHouseIdentification1")]
        public string InHouseIdentification1 { get; set; }
    }

    public partial class SbrUpdatorPosDetails
    {
        [JsonProperty("sbrUserIdentificationOwn")]
        public SbrUpdatorPosDetailsSbrUserIdentificationOwn SbrUserIdentificationOwn { get; set; }

        [JsonProperty("sbrSystemDetails")]
        public SbrUpdatorPosDetailsSbrSystemDetails SbrSystemDetails { get; set; }

        [JsonProperty("sbrPreferences")]
        public SbrPreferences SbrPreferences { get; set; }
    }

    public partial class SbrUpdatorPosDetailsSbrSystemDetails
    {
        [JsonProperty("deliveringSystem")]
        public DeliveringSystem DeliveringSystem { get; set; }
    }

    public partial class DeliveringSystem
    {
        [JsonProperty("companyId")]
        public string CompanyId { get; set; }

        [JsonProperty("locationId")]
        public string LocationId { get; set; }
    }

    public partial class SbrUpdatorPosDetailsSbrUserIdentificationOwn
    {
        [JsonProperty("originIdentification")]
        public OriginIdentification OriginIdentification { get; set; }

        [JsonProperty("originatorTypeCode")]
        public string OriginatorTypeCode { get; set; }
    }

    public partial class SecurityInformation
    {
        [JsonProperty("responsibilityInformation")]
        public ResponsibilityInformation ResponsibilityInformation { get; set; }

        [JsonProperty("queueingInformation")]
        public QueueingInformation QueueingInformation { get; set; }

        [JsonProperty("cityCode")]
        public string CityCode { get; set; }
    }

    public partial class QueueingInformation
    {
        [JsonProperty("queueingOfficeId")]
        public string QueueingOfficeId { get; set; }
    }

    public partial class ResponsibilityInformation
    {
        [JsonProperty("typeOfPnrElement")]
        public string TypeOfPnrElement { get; set; }

        [JsonProperty("officeId")]
        public string OfficeId { get; set; }
    }

    public partial class Session
    {
        [JsonProperty("TransactionStatusCode")]
        public string TransactionStatusCode { get; set; }
        
    }

    public partial class TravellerInfo
    {
        [JsonProperty("elementManagementPassenger")]
        public ElementManagement ElementManagementPassenger { get; set; }

        [JsonProperty("passengerData")]
        public PassengerDataUnion PassengerData { get; set; }
        
    }
    
    public partial struct PassengerDataUnion
    {
        public List<PassengerData> PassengerDataElementArray;
        public PassengerData PurplePassengerData;

        public bool IsNull => PassengerDataElementArray == null && PurplePassengerData == null;
    }

    public partial class PassengerData
    {
        [JsonProperty("travellerInformation")]
        public TravellerInformation TravellerInformation { get; set; }

        [JsonProperty("dateOfBirth", NullValueHandling = NullValueHandling.Ignore)]
        public DateOfBirth DateOfBirth { get; set; }
    }

    public partial class DateOfBirth
    {
        [JsonProperty("dateAndTimeDetails")]
        public DateAndTimeDetails DateAndTimeDetails { get; set; }
    }

    public partial class DateAndTimeDetails
    {
        [JsonProperty("qualifier")]
        public string Qualifier { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }
    }

    public partial class TravellerInformation
    {
        [JsonProperty("traveller")]
        public Traveller Traveller { get; set; }

        [JsonProperty("passenger")]
        public PassengerUnion Passenger { get; set; }
    }

    public partial class PassengerElement
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("infantIndicator", NullValueHandling = NullValueHandling.Ignore)]
        public string InfantIndicator { get; set; }
    }

    public partial class PurplePassenger
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("infantIndicator", NullValueHandling = NullValueHandling.Ignore)]
        public string InfantIndicator { get; set; }
    }

    public partial class Traveller
    {
        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }
    }

    public partial struct ReferenceUnion
    {
        public ReferenceElement ReferenceElement;
        public List<ReferenceElement> ReferenceElementArray;

        public bool IsNull => ReferenceElementArray == null && ReferenceElement == null;
    }

    public partial struct Text
    {
        public string String;
        public List<string> StringArray;

        public bool IsNull => StringArray == null && String == null;
    }

    public partial struct PassengerUnion
    {
        public List<PassengerElement> PassengerElementArray;
        public PurplePassenger PurplePassenger;

        public bool IsNull => PassengerElementArray == null && PurplePassenger == null;
    }

    public partial struct TravellerInfoUnion
    {
        public List<TravellerInfo> TravellerInfoElementArray;
        public TravellerInfo PurpleTravellerInfo;

        public bool IsNull => TravellerInfoElementArray == null && PurpleTravellerInfo == null;
    }

    public partial struct DataElementsIndivUnion
    {
        public List<DataElementsIndiv> DataElementsIndivElementArray;
        public DataElementsIndiv PurpleDataElementsIndiv;

        public bool IsNull => DataElementsIndivElementArray == null && PurpleDataElementsIndiv == null;
    }


    public partial class Response
    {
        public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, BL.Entities.PNR.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Response self) => JsonConvert.SerializeObject(self, BL.Entities.PNR.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new TextConverter(),
                new PassengerUnionConverter(),
                new TravellerInfoUnionConverter(),
                new DataElementsIndivUnionConverter(),
                new ReferenceUnionConverter(),
                new ItineraryInfoUnionConverter(),
                new PassengerDataUnionConverter(),
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

    internal class PassengerUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(PassengerUnion) || t == typeof(PassengerUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<PurplePassenger>(reader);
                    return new PassengerUnion { PurplePassenger = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<PassengerElement>>(reader);
                    return new PassengerUnion { PassengerElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type PassengerUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (PassengerUnion)untypedValue;
            if (value.PassengerElementArray != null)
            {
                serializer.Serialize(writer, value.PassengerElementArray); return;
            }
            if (value.PurplePassenger != null)
            {
                serializer.Serialize(writer, value.PurplePassenger); return;
            }
            throw new Exception("Cannot marshal type PassengerUnion");
        }
    }

    internal class TravellerInfoUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TravellerInfoUnion) || t == typeof(TravellerInfoUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<TravellerInfo>(reader);
                    return new TravellerInfoUnion { PurpleTravellerInfo = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<TravellerInfo>>(reader);
                    return new TravellerInfoUnion { TravellerInfoElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type TravellerInfoUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (TravellerInfoUnion)untypedValue;
            if (value.TravellerInfoElementArray != null)
            {
                serializer.Serialize(writer, value.TravellerInfoElementArray); return;
            }
            if (value.PurpleTravellerInfo != null)
            {
                serializer.Serialize(writer, value.PurpleTravellerInfo); return;
            }
            throw new Exception("Cannot marshal type TravellerInfoUnion");
        }
    }

    internal class DataElementsIndivUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DataElementsIndivUnion) || t == typeof(DataElementsIndivUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<DataElementsIndiv>(reader);
                    return new DataElementsIndivUnion { PurpleDataElementsIndiv = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<DataElementsIndiv>>(reader);
                    return new DataElementsIndivUnion { DataElementsIndivElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type DataElementsIndivUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (DataElementsIndivUnion)untypedValue;
            if (value.DataElementsIndivElementArray != null)
            {
                serializer.Serialize(writer, value.DataElementsIndivElementArray); return;
            }
            if (value.PurpleDataElementsIndiv != null)
            {
                serializer.Serialize(writer, value.PurpleDataElementsIndiv); return;
            }
            throw new Exception("Cannot marshal type DataElementsIndivUnion");
        }
    }

    internal class ReferenceUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ReferenceUnion) || t == typeof(ReferenceUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<ReferenceElement>(reader);
                    return new ReferenceUnion { ReferenceElement = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<ReferenceElement>>(reader);
                    return new ReferenceUnion { ReferenceElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type ReferenceUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ReferenceUnion)untypedValue;
            if (value.ReferenceElementArray != null)
            {
                serializer.Serialize(writer, value.ReferenceElementArray); return;
            }
            if (value.ReferenceElement != null)
            {
                serializer.Serialize(writer, value.ReferenceElement); return;
            }
            throw new Exception("Cannot marshal type ReferenceUnion");
        }
    }

    internal class ItineraryInfoUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ItineraryInfoUnion) || t == typeof(ItineraryInfoUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<ItineraryInfo>(reader);
                    return new ItineraryInfoUnion { PurpleItineraryInfo = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<ItineraryInfo>>(reader);
                    return new ItineraryInfoUnion { ItineraryInfoElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type ItineraryInfoUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ItineraryInfoUnion)untypedValue;
            if (value.ItineraryInfoElementArray != null)
            {
                serializer.Serialize(writer, value.ItineraryInfoElementArray); return;
            }
            if (value.PurpleItineraryInfo != null)
            {
                serializer.Serialize(writer, value.PurpleItineraryInfo); return;
            }
            throw new Exception("Cannot marshal type ItineraryInfoUnion");
        }
    }

    internal class PassengerDataUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(PassengerDataUnion) || t == typeof(PassengerDataUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<PassengerData>(reader);
                    return new PassengerDataUnion { PurplePassengerData = objectValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<PassengerData>>(reader);
                    return new PassengerDataUnion { PassengerDataElementArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type PassengerDataUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (PassengerDataUnion)untypedValue;
            if (value.PassengerDataElementArray != null)
            {
                serializer.Serialize(writer, value.PassengerDataElementArray); return;
            }
            if (value.PurplePassengerData != null)
            {
                serializer.Serialize(writer, value.PurplePassengerData); return;
            }
            throw new Exception("Cannot marshal type PassengerDataUnion");
        }
    }

}
