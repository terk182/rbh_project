using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BL.Entities.MasterCalendar
{
    public partial class Response
    {
        [JsonProperty("Fare_MasterPricerCalendarReply")]
        public FareMasterPricerCalendarReply FareMasterPricerCalendarReply { get; set; }
    }
    public partial class FareMasterPricerCalendarReply
    {
        [JsonProperty("@xmlns")]
        public string Xmlns { get; set; }

        [JsonProperty("replyStatus")]
        public ReplyStatus ReplyStatus { get; set; }

        [JsonProperty("conversionRate")]
        public ConversionRate ConversionRate { get; set; }

        [JsonProperty("flightIndex")]
        public FlightIndexUnion FlightIndex { get; set; }

        [JsonProperty("recommendation")]
        public RecommendationUnion Recommendation { get; set; }

        [JsonProperty("Session")]
        public Session Session { get; set; }
    }
    public partial struct RecommendationUnion
    {
        public List<Recommendation> RecommendationElementArray;
        public Recommendation PurpleRecommendation;
    }

    public partial struct RecommendationUnion
    {
        public RecommendationUnion(JsonReader reader, JsonSerializer serializer)
        {
            RecommendationElementArray = null;
            PurpleRecommendation = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    RecommendationElementArray = serializer.Deserialize<List<Recommendation>>(reader);
                    return;
                case JsonToken.StartObject:
                    PurpleRecommendation = serializer.Deserialize<Recommendation>(reader);
                    return;
            }
            throw new Exception("Cannot convert RecommendationUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (RecommendationElementArray != null)
            {
                serializer.Serialize(writer, RecommendationElementArray);
                return;
            }
            if (PurpleRecommendation != null)
            {
                serializer.Serialize(writer, PurpleRecommendation);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial class ConversionRate
    {
        [JsonProperty("conversionRateDetail")]
        public ConversionRateDetail ConversionRateDetail { get; set; }
    }

    public partial class ConversionRateDetail
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public partial class FlightIndexElement
    {
        [JsonProperty("requestedSegmentRef")]
        public SegmentRef RequestedSegmentRef { get; set; }

        [JsonProperty("groupOfFlights")]
        public GroupOfFlightUnion GroupOfFlights { get; set; }
    }

    public partial class PurpleFlightIndex
    {
        [JsonProperty("requestedSegmentRef")]
        public SegmentRef RequestedSegmentRef { get; set; }

        [JsonProperty("groupOfFlights")]
        public GroupOfFlightUnion GroupOfFlights { get; set; }
    }

    public partial struct GroupOfFlightUnion
    {
        public List<GroupOfFlight> GroupOfFlightElementArray;
        public GroupOfFlight PurpleGroupOfFlight;
    }


    public partial struct GroupOfFlightUnion
    {
        public GroupOfFlightUnion(JsonReader reader, JsonSerializer serializer)
        {
            GroupOfFlightElementArray = null;
            PurpleGroupOfFlight = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    GroupOfFlightElementArray = serializer.Deserialize<List<GroupOfFlight>>(reader);
                    return;
                case JsonToken.StartObject:
                    PurpleGroupOfFlight = serializer.Deserialize<GroupOfFlight>(reader);
                    return;
            }
            throw new Exception("Cannot convert GroupOfFlightUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (GroupOfFlightElementArray != null)
            {
                serializer.Serialize(writer, GroupOfFlightElementArray);
                return;
            }
            if (PurpleGroupOfFlight != null)
            {
                serializer.Serialize(writer, PurpleGroupOfFlight);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct FlightIndexUnion
    {
        public List<FlightIndexElement> FlightIndexElementArray;
        public PurpleFlightIndex PurpleFlightIndex;
    }


    public partial struct FlightIndexUnion
    {
        public FlightIndexUnion(JsonReader reader, JsonSerializer serializer)
        {
            FlightIndexElementArray = null;
            PurpleFlightIndex = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    FlightIndexElementArray = serializer.Deserialize<List<FlightIndexElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    PurpleFlightIndex = serializer.Deserialize<PurpleFlightIndex>(reader);
                    return;
            }
            throw new Exception("Cannot convert FlightIndexUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (FlightIndexElementArray != null)
            {
                serializer.Serialize(writer, FlightIndexElementArray);
                return;
            }
            if (PurpleFlightIndex != null)
            {
                serializer.Serialize(writer, PurpleFlightIndex);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }
    public partial class GroupOfFlight
    {
        [JsonProperty("propFlightGrDetail")]
        public PropFlightGrDetail PropFlightGrDetail { get; set; }

        [JsonProperty("flightDetails")]
        public FlightDetailsUnion FlightDetails { get; set; }

    }

    public partial class FlightCharacteristics
    {
        [JsonProperty("inFlightSrv")]
        public PriceType InFlightSrv { get; set; }
    }

    public partial class FlightDetail
    {
        [JsonProperty("flightInformation")]
        public FlightDetailFlightInformation FlightInformation { get; set; }

        [JsonProperty("technicalStop", NullValueHandling = NullValueHandling.Ignore)]
        public TechnicalStop TechnicalStop { get; set; }

        [JsonProperty("flightCharacteristics", NullValueHandling = NullValueHandling.Ignore)]
        public FlightCharacteristics FlightCharacteristics { get; set; }
    }

    public partial class FlightDetailFlightInformation
    {
        [JsonProperty("productDateTime")]
        public ProductDateTime ProductDateTime { get; set; }

        [JsonProperty("location")]
        public List<Location> Location { get; set; }

        [JsonProperty("companyId")]
        public CompanyId CompanyId { get; set; }

        [JsonProperty("flightOrtrainNumber")]
        public string FlightNumber { get; set; }

        [JsonProperty("productDetail")]
        public PurpleProductDetail ProductDetail { get; set; }

        [JsonProperty("addProductDetail")]
        public AddProductDetail AddProductDetail { get; set; }
    }

    public partial class AddProductDetail
    {
        [JsonProperty("electronicTicketing")]
        public string ElectronicTicketing { get; set; }

        [JsonProperty("productDetailQualifier")]
        public string ProductDetailQualifier { get; set; }
    }

    public partial class CompanyId
    {
        [JsonProperty("marketingCarrier")]
        public string MarketingCarrier { get; set; }

        [JsonProperty("operatingCarrier")]
        public string OperatingCarrier { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("locationId")]
        public string LocationId { get; set; }

        [JsonProperty("terminal", NullValueHandling = NullValueHandling.Ignore)]
        public string Terminal { get; set; }
    }

    public partial class ProductDateTime
    {
        [JsonProperty("dateOfDeparture")]
        public string DateOfDeparture { get; set; }

        [JsonProperty("timeOfDeparture")]
        public string TimeOfDeparture { get; set; }

        [JsonProperty("dateOfArrival")]
        public string DateOfArrival { get; set; }

        [JsonProperty("timeOfArrival")]
        public string TimeOfArrival { get; set; }

        [JsonProperty("dateVariation", NullValueHandling = NullValueHandling.Ignore)]
        public string DateVariation { get; set; }
    }

    public partial class PurpleProductDetail
    {
        [JsonProperty("equipmentType")]
        public string EquipmentType { get; set; }

        [JsonProperty("techStopNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string TechStopNumber { get; set; }
    }

    public partial class TechnicalStop
    {
        [JsonProperty("stopDetails")]
        public List<StopDetail> StopDetails { get; set; }
    }

    public partial class StopDetail
    {
        [JsonProperty("dateQualifier")]
        public string DateQualifier { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("firstTime")]
        public string FirstTime { get; set; }

        [JsonProperty("locationId", NullValueHandling = NullValueHandling.Ignore)]
        public string LocationId { get; set; }
    }

    public partial class FlightDetailsClass
    {
        [JsonProperty("flightInformation")]
        public FlightDetailsFlightInformation FlightInformation { get; set; }

        [JsonProperty("flightCharacteristics", NullValueHandling = NullValueHandling.Ignore)]
        public FlightCharacteristics FlightCharacteristics { get; set; }

        [JsonProperty("technicalStop", NullValueHandling = NullValueHandling.Ignore)]
        public TechnicalStop TechnicalStop { get; set; }
    }

    public partial class FlightDetailsFlightInformation
    {
        [JsonProperty("productDateTime")]
        public ProductDateTime ProductDateTime { get; set; }

        [JsonProperty("location")]
        public List<Location> Location { get; set; }

        [JsonProperty("companyId")]
        public CompanyId CompanyId { get; set; }

        [JsonProperty("flightOrtrainNumber")]
        public string FlightNumber { get; set; }

        [JsonProperty("productDetail")]
        public FluffyProductDetail ProductDetail { get; set; }

        [JsonProperty("addProductDetail")]
        public AddProductDetail AddProductDetail { get; set; }
    }

    public partial class FluffyProductDetail
    {
        [JsonProperty("equipmentType")]
        public string EquipmentType { get; set; }
    }

    public partial class PropFlightGrDetail
    {
        [JsonProperty("flightProposal")]
        public List<FlightProposal> FlightProposal { get; set; }
    }

    public partial class FlightProposal
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }

        [JsonProperty("unitQualifier", NullValueHandling = NullValueHandling.Ignore)]
        public string UnitQualifier { get; set; }
    }

    public partial class SegmentRef
    {
        [JsonProperty("segRef")]
        public string SegRef { get; set; }
    }

    public partial class Recommendation
    {
        [JsonProperty("itemNumber")]
        public ItemNumber ItemNumber { get; set; }

        [JsonProperty("recPriceInfo")]
        public RecPriceInfo RecPriceInfo { get; set; }

        [JsonProperty("segmentFlightRef")]
        public SegmentFlightRefUnion SegmentFlightRef { get; set; }

        [JsonProperty("paxFareProduct")]
        public PaxFareProductUnion PaxFareProduct { get; set; }
    }

    public partial class ItemNumber
    {
        [JsonProperty("itemNumberId")]
        public ItemNumberId ItemNumberId { get; set; }
    }

    public partial class ItemNumberId
    {
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("numberType", NullValueHandling = NullValueHandling.Ignore)]//ning add on 20200304 formulti ticket
        public string NumberType { get; set; }
    }

    public partial class PaxFareProductElement
    {
        [JsonProperty("paxFareDetail")]
        public PaxFareDetail PaxFareDetail { get; set; }

        [JsonProperty("paxReference")]
        public PaxReferenceUnion PaxReference { get; set; }

        [JsonProperty("fare")]
        public FareUnion Fare { get; set; }

        [JsonProperty("fareDetails")]
        public FareDetailUnion FareDetails { get; set; }
    }

    public partial class FareElement
    {
        [JsonProperty("pricingMessage")]
        public PurplePricingMessage PricingMessage { get; set; }

        [JsonProperty("monetaryInformation", NullValueHandling = NullValueHandling.Ignore)]
        public MonetaryInformation MonetaryInformation { get; set; }
    }

    public partial class MonetaryInformation
    {
        [JsonProperty("monetaryDetail")]
        public MonetaryInformationMonetaryDetail MonetaryDetail { get; set; }
    }

    public partial class MonetaryInformationMonetaryDetail
    {
        [JsonProperty("amountType")]
        public string AmountType { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public partial class PurplePricingMessage
    {
        [JsonProperty("freeTextQualification")]
        public FreeTextQualification FreeTextQualification { get; set; }

        [JsonProperty("description")]
        public PriceType Description { get; set; }
    }

    public partial class FreeTextQualification
    {
        [JsonProperty("textSubjectQualifier")]
        public string TextSubjectQualifier { get; set; }

        [JsonProperty("informationType")]
        public string InformationType { get; set; }
    }

    public partial class PurpleFare
    {
        [JsonProperty("pricingMessage")]
        public FluffyPricingMessage PricingMessage { get; set; }
    }

    public partial class FluffyPricingMessage
    {
        [JsonProperty("freeTextQualification")]
        public FreeTextQualification FreeTextQualification { get; set; }

        [JsonProperty("description")]
        public List<string> Description { get; set; }
    }

    public partial class FareDetailElement
    {
        [JsonProperty("segmentRef")]
        public SegmentRef SegmentRef { get; set; }

        [JsonProperty("groupOfFares")]
        public TentacledGroupOfFares GroupOfFares { get; set; }
    }

    public partial class PurpleFareDetail
    {
        [JsonProperty("segmentRef")]
        public SegmentRef SegmentRef { get; set; }

        [JsonProperty("groupOfFares")]
        public TentacledGroupOfFares GroupOfFares { get; set; }
    }

    public partial struct FareDetailUnion
    {
        public List<FareDetailElement> FareDetailElementArray;
        public PurpleFareDetail PurpleFareDetail;
    }


    public partial struct FareDetailUnion
    {
        public FareDetailUnion(JsonReader reader, JsonSerializer serializer)
        {
            FareDetailElementArray = null;
            PurpleFareDetail = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    FareDetailElementArray = serializer.Deserialize<List<FareDetailElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    PurpleFareDetail = serializer.Deserialize<PurpleFareDetail>(reader);
                    return;
            }
            throw new Exception("Cannot convert FareDetailUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (FareDetailElementArray != null)
            {
                serializer.Serialize(writer, FareDetailElementArray);
                return;
            }
            if (PurpleFareDetail != null)
            {
                serializer.Serialize(writer, PurpleFareDetail);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }



    public partial class PurpleGroupOfFare
    {
        [JsonProperty("productInformation")]
        public GroupOfFareProductInformation ProductInformation { get; set; }

        [JsonProperty("ticketInfos", NullValueHandling = NullValueHandling.Ignore)]
        public TicketInfos TicketInfos { get; set; }
    }

    public partial class GroupOfFareProductInformation
    {
        [JsonProperty("cabinProduct")]
        public CabinProduct CabinProduct { get; set; }

        [JsonProperty("fareProductDetail")]
        public PurpleFareProductDetail FareProductDetail { get; set; }

        [JsonProperty("breakPoint")]
        public string BreakPoint { get; set; }
    }

    public partial class CabinProduct
    {
        [JsonProperty("rbd")]
        public string Rbd { get; set; }

        [JsonProperty("cabin")]
        public string Cabin { get; set; }

        [JsonProperty("avlStatus")]
        public string AvlStatus { get; set; }
    }

    public partial class PurpleFareProductDetail
    {
        [JsonProperty("fareBasis")]
        public string FareBasis { get; set; }

        [JsonProperty("passengerType")]
        public string PassengerType { get; set; }

        [JsonProperty("fareType")]
        public PriceType FareType { get; set; }
    }

    public partial class TicketInfos
    {
        [JsonProperty("additionalFareDetails")]
        public AdditionalFareDetails AdditionalFareDetails { get; set; }
    }

    public partial class AdditionalFareDetails
    {
        [JsonProperty("ticketDesignator")]
        public string TicketDesignator { get; set; }
    }

    public partial class PurpleGroupOfFares
    {
        [JsonProperty("productInformation")]
        public GroupOfFaresProductInformation ProductInformation { get; set; }

        [JsonProperty("ticketInfos", NullValueHandling = NullValueHandling.Ignore)]
        public TicketInfos TicketInfos { get; set; }
    }

    public partial class GroupOfFaresProductInformation
    {
        [JsonProperty("cabinProduct")]
        public CabinProduct CabinProduct { get; set; }

        [JsonProperty("fareProductDetail")]
        public FluffyFareProductDetail FareProductDetail { get; set; }

        [JsonProperty("breakPoint")]
        public string BreakPoint { get; set; }
    }

    public partial class FluffyFareProductDetail
    {
        [JsonProperty("fareBasis")]
        public string FareBasis { get; set; }

        [JsonProperty("passengerType")]
        public string PassengerType { get; set; }

        [JsonProperty("fareType")]
        public PriceType FareType { get; set; }
    }

    public partial struct FareTypeUnion
    {
        public List<string> FareTypeElementArray;
        public string PurpleFareType;
    }


    public partial struct FareTypeUnion
    {
        public FareTypeUnion(JsonReader reader, JsonSerializer serializer)
        {
            FareTypeElementArray = null;
            PurpleFareType = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    FareTypeElementArray = serializer.Deserialize<List<string>>(reader);
                    return;
                case JsonToken.StartObject:
                    PurpleFareType = serializer.Deserialize<string>(reader);
                    return;
            }
            throw new Exception("Cannot convert FareTypeUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (FareTypeElementArray != null)
            {
                serializer.Serialize(writer, FareTypeElementArray);
                return;
            }
            if (PurpleFareType != null)
            {
                serializer.Serialize(writer, PurpleFareType);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }


    public partial class PaxFareDetail
    {
        [JsonProperty("paxFareNum")]
        public string PaxFareNum { get; set; }

        [JsonProperty("totalFareAmount")]
        public string TotalFareAmount { get; set; }

        [JsonProperty("totalTaxAmount")]
        public string TotalTaxAmount { get; set; }

        [JsonProperty("codeShareDetails")]
        public CodeShareDetails CodeShareDetails { get; set; }

        [JsonProperty("pricingTicketing", NullValueHandling = NullValueHandling.Ignore)]
        public PricingTicketing PricingTicketing { get; set; }
    }

    public partial class CodeShareDetail
    {
        [JsonProperty("transportStageQualifier", NullValueHandling = NullValueHandling.Ignore)]
        public string TransportStageQualifier { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }
    }

    public partial class PricingTicketing
    {
        [JsonProperty("priceType")]
        public PriceType PriceType { get; set; }
    }

    public partial class PurplePaxReference
    {
        [JsonProperty("ptc")]
        public string Ptc { get; set; }

        [JsonProperty("traveller")]
        public TravellerUnion Traveller { get; set; }
    }

    public partial class PaxReferenceElement
    {
        [JsonProperty("ptc")]
        public string Ptc { get; set; }

        [JsonProperty("traveller")]
        public TravellerUnion Traveller { get; set; }
    }

    public partial struct PaxReferenceUnion
    {
        public List<PaxReferenceElement> PaxReferenceElementArray;
        public PurplePaxReference PurplePaxReference;
    }


    public partial struct PaxReferenceUnion
    {
        public PaxReferenceUnion(JsonReader reader, JsonSerializer serializer)
        {
            PaxReferenceElementArray = null;
            PurplePaxReference = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    PaxReferenceElementArray = serializer.Deserialize<List<PaxReferenceElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    PurplePaxReference = serializer.Deserialize<PurplePaxReference>(reader);
                    return;
            }
            throw new Exception("Cannot convert PaxReferenceUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (PaxReferenceElementArray != null)
            {
                serializer.Serialize(writer, PaxReferenceElementArray);
                return;
            }
            if (PurplePaxReference != null)
            {
                serializer.Serialize(writer, PurplePaxReference);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }


    public partial class TravellerElement
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }
    }

    public partial class PurpleTraveller
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }

        [JsonProperty("infantIndicator", NullValueHandling = NullValueHandling.Ignore)]
        public string InfantIndicator { get; set; }
    }

    public partial class PurplePaxFareProduct
    {
        [JsonProperty("paxFareDetail")]
        public PaxFareDetail PaxFareDetail { get; set; }

        [JsonProperty("paxReference")]
        public PaxReferenceUnion PaxReference { get; set; }

        [JsonProperty("fare")]
        public FareUnion Fare { get; set; }

        [JsonProperty("fareDetails")]
        public FareDetails FareDetails { get; set; }
    }

    public partial class FareDetailsElement
    {
        [JsonProperty("segmentRef")]
        public SegmentRef SegmentRef { get; set; }

        [JsonProperty("groupOfFares")]
        public FareDetailsGroupOfFares GroupOfFares { get; set; }
    }

    public partial class FluffyGroupOfFare
    {
        [JsonProperty("productInformation")]
        public GroupOfFareProductInformation ProductInformation { get; set; }
    }

    public partial class FluffyGroupOfFares
    {
        [JsonProperty("productInformation")]
        public GroupOfFaresProductInformation ProductInformation { get; set; }
    }

    public partial class FluffyPaxReference
    {
        [JsonProperty("ptc")]
        public string Ptc { get; set; }

        [JsonProperty("traveller")]
        public TravellerElement Traveller { get; set; }
    }

    public partial class RecPriceInfo
    {
        [JsonProperty("monetaryDetail")]
        public List<MonetaryDetailElement> MonetaryDetail { get; set; }
    }

    public partial class MonetaryDetailElement
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }
    }

    public partial class SegmentFlightRefElement
    {
        [JsonProperty("referencingDetail")]
        public ReferencingDetailUnion ReferencingDetail { get; set; }
    }

    public partial class ReferencingDetailElement
    {
        [JsonProperty("refQualifier")]
        public string RefQualifier { get; set; }

        [JsonProperty("refNumber")]
        public string RefNumber { get; set; }
    }

    public partial class ReplyStatus
    {
        [JsonProperty("status")]
        public List<Status> Status { get; set; }
    }

    public partial class Status
    {
        [JsonProperty("advisoryTypeInfo")]
        public string AdvisoryTypeInfo { get; set; }

        [JsonProperty("notification", NullValueHandling = NullValueHandling.Ignore)]
        public string Notification { get; set; }
    }

    public partial class ServiceFeesGrp
    {
        [JsonProperty("serviceTypeInfo")]
        public ServiceTypeInfo ServiceTypeInfo { get; set; }

        [JsonProperty("serviceCoverageInfoGrp")]
        public ServiceCoverageInfoGrpUnion ServiceCoverageInfoGrp { get; set; }

        [JsonProperty("globalMessageMarker")]
        public string GlobalMessageMarker { get; set; }

        [JsonProperty("freeBagAllowanceGrp")]
        public FreeBagAllowanceGrpUnion FreeBagAllowanceGrp { get; set; }
    }

    public partial class FreeBagAllowanceGrpElement
    {
        [JsonProperty("freeBagAllownceInfo")]
        public FreeBagAllownceInfo FreeBagAllownceInfo { get; set; }

        [JsonProperty("itemNumberInfo")]
        public FreeBagAllowanceGrpItemNumberInfo ItemNumberInfo { get; set; }
    }

    public partial class FreeBagAllownceInfo
    {
        [JsonProperty("baggageDetails")]
        public BaggageDetails BaggageDetails { get; set; }
    }

    public partial class BaggageDetails
    {
        [JsonProperty("freeAllowance")]
        public long FreeAllowance { get; set; }

        [JsonProperty("quantityCode")]
        public string QuantityCode { get; set; }

        [JsonProperty("unitQualifier")]
        public string UnitQualifier { get; set; }
    }

    public partial class FreeBagAllowanceGrpItemNumberInfo
    {
        [JsonProperty("itemNumberDetails")]
        public ItemNumberDetailsClass ItemNumberDetails { get; set; }
    }

    public partial class ItemNumberDetailsClass
    {
        [JsonProperty("number")]
        public long Number { get; set; }
    }

    public partial class ServiceCoverageInfoGrpElement
    {
        [JsonProperty("itemNumberInfo")]
        public ServiceCoverageInfoGrpItemNumberInfo ItemNumberInfo { get; set; }

        [JsonProperty("serviceCovInfoGrp")]
        public ServiceCovInfoGrpUnion ServiceCovInfoGrp { get; set; }
    }

    public partial class ServiceCoverageInfoGrpItemNumberInfo
    {
        [JsonProperty("itemNumber")]
        public ItemNumberDetailsClass ItemNumber { get; set; }
    }

    public partial class ServiceCovInfoGrpElement
    {
        [JsonProperty("paxRefInfo")]
        public PaxRefInfo PaxRefInfo { get; set; }

        [JsonProperty("coveragePerFlightsInfo")]
        public CoveragePerFlightsInfoUnion CoveragePerFlightsInfo { get; set; }

        [JsonProperty("refInfo")]
        public RefInfo RefInfo { get; set; }
    }

    public partial class CoveragePerFlightsInfoElement
    {
        [JsonProperty("numberOfItemsDetails")]
        public NumberOfItemsDetails NumberOfItemsDetails { get; set; }

        [JsonProperty("lastItemsDetails")]
        public LastItemsDetailUnion LastItemsDetails { get; set; }
    }

    public partial class LastItemsDetailElement
    {
        [JsonProperty("refOfLeg")]
        public long RefOfLeg { get; set; }
    }

    public partial class NumberOfItemsDetails
    {
        [JsonProperty("referenceQualifier")]
        public string ReferenceQualifier { get; set; }

        [JsonProperty("refNum")]
        public long RefNum { get; set; }
    }

    public partial class PaxRefInfo
    {
        [JsonProperty("travellerDetails")]
        public TravellerDetails TravellerDetails { get; set; }
    }

    public partial class TravellerDetails
    {
        [JsonProperty("referenceNumber")]
        public long ReferenceNumber { get; set; }
    }

    public partial class RefInfo
    {
        [JsonProperty("referencingDetail")]
        public ReferencingDetail ReferencingDetail { get; set; }
    }
    public partial class ReferencingDetail
    {
        [JsonProperty("refQualifier")]
        public string RefQualifier { get; set; }

        [JsonProperty("refNumber")]
        public long RefNumber { get; set; }
    }

    public partial class ServiceTypeInfo
    {
        [JsonProperty("carrierFeeDetails")]
        public CarrierFeeDetails CarrierFeeDetails { get; set; }
    }

    public partial class CarrierFeeDetails
    {
        [JsonProperty("type")]
        public string Type { get; set; }
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
    }

    public partial struct FlightDetailsUnion
    {
        public List<FlightDetail> FlightDetailArray;
        public FlightDetailsClass FlightDetailsClass;
    }

    public partial struct PaxFareProductUnion
    {
        public List<PaxFareProductElement> PaxFareProductElementArray;
        public PurplePaxFareProduct PurplePaxFareProduct;
    }

    public partial struct FareUnion
    {
        public List<FareElement> FareElementArray;
        public PurpleFare PurpleFare;
    }

    public partial struct PriceType
    {
        public string String;
        public List<string> StringArray;
    }

    public partial struct TentacledGroupOfFares
    {
        public List<PurpleGroupOfFare> PurpleGroupOfFareArray;
        public PurpleGroupOfFares PurpleGroupOfFares;
    }

    public partial struct CodeShareDetails
    {
        public CodeShareDetail CodeShareDetail;
        public List<CodeShareDetail> CodeShareDetailArray;
    }

    public partial struct TravellerUnion
    {
        public PurpleTraveller PurpleTraveller;
        public List<TravellerElement> TravellerElementArray;
    }

    public partial struct FareDetails
    {
        public FareDetailsElement FareDetailsElement;
        public List<FareDetailsElement> FareDetailsElementArray;
    }

    public partial struct FareDetailsGroupOfFares
    {
        public List<FluffyGroupOfFare> FluffyGroupOfFareArray;
        public FluffyGroupOfFares FluffyGroupOfFares;
    }

    public partial struct SegmentFlightRefUnion
    {
        public SegmentFlightRefElement SegmentFlightRefElement;
        public List<SegmentFlightRefElement> SegmentFlightRefElementArray;
    }

    public partial struct ReferencingDetailUnion
    {
        public ReferencingDetailElement ReferencingDetailElement;
        public List<ReferencingDetailElement> ReferencingDetailElementArray;
    }

    public partial struct CoveragePerFlightsInfoUnion
    {
        public CoveragePerFlightsInfoElement CoveragePerFlightsInfoElement;
        public List<CoveragePerFlightsInfoElement> CoveragePerFlightsInfoElementArray;
    }
    public partial struct LastItemsDetailUnion
    {
        public LastItemsDetailElement LastItemsDetailElement;
        public List<LastItemsDetailElement> LastItemsDetailElementArray;
    }
    public partial struct ServiceCoverageInfoGrpUnion
    {
        public ServiceCoverageInfoGrpElement ServiceCoverageInfoGrpElement;
        public List<ServiceCoverageInfoGrpElement> ServiceCoverageInfoGrpElementArray;
    }

    public partial struct FreeBagAllowanceGrpUnion
    {
        public FreeBagAllowanceGrpElement FreeBagAllowanceGrpElement;
        public List<FreeBagAllowanceGrpElement> FreeBagAllowanceGrpElementArray;
    }
    public partial struct ServiceCovInfoGrpUnion
    {
        public ServiceCovInfoGrpElement ServiceCovInfoGrpElement;
        public List<ServiceCovInfoGrpElement> ServiceCovInfoGrpElementArray;
    }

    public partial class Response
    {
        public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, BL.Entities.MasterPricer.Converter.Settings);
    }

    public partial struct FlightDetailsUnion
    {
        public FlightDetailsUnion(JsonReader reader, JsonSerializer serializer)
        {
            FlightDetailArray = null;
            FlightDetailsClass = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    FlightDetailArray = serializer.Deserialize<List<FlightDetail>>(reader);
                    return;
                case JsonToken.StartObject:
                    FlightDetailsClass = serializer.Deserialize<FlightDetailsClass>(reader);
                    return;
            }
            throw new Exception("Cannot convert FlightDetailsUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (FlightDetailArray != null)
            {
                serializer.Serialize(writer, FlightDetailArray);
                return;
            }
            if (FlightDetailsClass != null)
            {
                serializer.Serialize(writer, FlightDetailsClass);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct PaxFareProductUnion
    {
        public PaxFareProductUnion(JsonReader reader, JsonSerializer serializer)
        {
            PaxFareProductElementArray = null;
            PurplePaxFareProduct = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    PaxFareProductElementArray = serializer.Deserialize<List<PaxFareProductElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    PurplePaxFareProduct = serializer.Deserialize<PurplePaxFareProduct>(reader);
                    return;
            }
            throw new Exception("Cannot convert PaxFareProductUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (PaxFareProductElementArray != null)
            {
                serializer.Serialize(writer, PaxFareProductElementArray);
                return;
            }
            if (PurplePaxFareProduct != null)
            {
                serializer.Serialize(writer, PurplePaxFareProduct);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct FareUnion
    {
        public FareUnion(JsonReader reader, JsonSerializer serializer)
        {
            FareElementArray = null;
            PurpleFare = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    FareElementArray = serializer.Deserialize<List<FareElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    PurpleFare = serializer.Deserialize<PurpleFare>(reader);
                    return;
            }
            throw new Exception("Cannot convert FareUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (FareElementArray != null)
            {
                serializer.Serialize(writer, FareElementArray);
                return;
            }
            if (PurpleFare != null)
            {
                serializer.Serialize(writer, PurpleFare);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct PriceType
    {
        public PriceType(JsonReader reader, JsonSerializer serializer)
        {
            String = null;
            StringArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    StringArray = serializer.Deserialize<List<string>>(reader);
                    return;
                case JsonToken.String:
                case JsonToken.Date:
                    String = serializer.Deserialize<string>(reader);
                    return;
            }
            throw new Exception("Cannot convert PriceType");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (String != null)
            {
                serializer.Serialize(writer, String);
                return;
            }
            if (StringArray != null)
            {
                serializer.Serialize(writer, StringArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct TentacledGroupOfFares
    {
        public TentacledGroupOfFares(JsonReader reader, JsonSerializer serializer)
        {
            PurpleGroupOfFareArray = null;
            PurpleGroupOfFares = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    PurpleGroupOfFareArray = serializer.Deserialize<List<PurpleGroupOfFare>>(reader);
                    return;
                case JsonToken.StartObject:
                    PurpleGroupOfFares = serializer.Deserialize<PurpleGroupOfFares>(reader);
                    return;
            }
            throw new Exception("Cannot convert TentacledGroupOfFares");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (PurpleGroupOfFareArray != null)
            {
                serializer.Serialize(writer, PurpleGroupOfFareArray);
                return;
            }
            if (PurpleGroupOfFares != null)
            {
                serializer.Serialize(writer, PurpleGroupOfFares);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct CodeShareDetails
    {
        public CodeShareDetails(JsonReader reader, JsonSerializer serializer)
        {
            CodeShareDetail = null;
            CodeShareDetailArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    CodeShareDetailArray = serializer.Deserialize<List<CodeShareDetail>>(reader);
                    return;
                case JsonToken.StartObject:
                    CodeShareDetail = serializer.Deserialize<CodeShareDetail>(reader);
                    return;
            }
            throw new Exception("Cannot convert CodeShareDetails");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (CodeShareDetail != null)
            {
                serializer.Serialize(writer, CodeShareDetail);
                return;
            }
            if (CodeShareDetailArray != null)
            {
                serializer.Serialize(writer, CodeShareDetailArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct TravellerUnion
    {
        public TravellerUnion(JsonReader reader, JsonSerializer serializer)
        {
            PurpleTraveller = null;
            TravellerElementArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    TravellerElementArray = serializer.Deserialize<List<TravellerElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    PurpleTraveller = serializer.Deserialize<PurpleTraveller>(reader);
                    return;
            }
            throw new Exception("Cannot convert TravellerUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (PurpleTraveller != null)
            {
                serializer.Serialize(writer, PurpleTraveller);
                return;
            }
            if (TravellerElementArray != null)
            {
                serializer.Serialize(writer, TravellerElementArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct FareDetails
    {
        public FareDetails(JsonReader reader, JsonSerializer serializer)
        {
            FareDetailsElement = null;
            FareDetailsElementArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    FareDetailsElementArray = serializer.Deserialize<List<FareDetailsElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    FareDetailsElement = serializer.Deserialize<FareDetailsElement>(reader);
                    return;
            }
            throw new Exception("Cannot convert FareDetails");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (FareDetailsElement != null)
            {
                serializer.Serialize(writer, FareDetailsElement);
                return;
            }
            if (FareDetailsElementArray != null)
            {
                serializer.Serialize(writer, FareDetailsElementArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct FareDetailsGroupOfFares
    {
        public FareDetailsGroupOfFares(JsonReader reader, JsonSerializer serializer)
        {
            FluffyGroupOfFareArray = null;
            FluffyGroupOfFares = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    FluffyGroupOfFareArray = serializer.Deserialize<List<FluffyGroupOfFare>>(reader);
                    return;
                case JsonToken.StartObject:
                    FluffyGroupOfFares = serializer.Deserialize<FluffyGroupOfFares>(reader);
                    return;
            }
            throw new Exception("Cannot convert FareDetailsGroupOfFares");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (FluffyGroupOfFareArray != null)
            {
                serializer.Serialize(writer, FluffyGroupOfFareArray);
                return;
            }
            if (FluffyGroupOfFares != null)
            {
                serializer.Serialize(writer, FluffyGroupOfFares);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct SegmentFlightRefUnion
    {
        public SegmentFlightRefUnion(JsonReader reader, JsonSerializer serializer)
        {
            SegmentFlightRefElement = null;
            SegmentFlightRefElementArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    SegmentFlightRefElementArray = serializer.Deserialize<List<SegmentFlightRefElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    SegmentFlightRefElement = serializer.Deserialize<SegmentFlightRefElement>(reader);
                    return;
            }
            throw new Exception("Cannot convert SegmentFlightRefUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (SegmentFlightRefElement != null)
            {
                serializer.Serialize(writer, SegmentFlightRefElement);
                return;
            }
            if (SegmentFlightRefElementArray != null)
            {
                serializer.Serialize(writer, SegmentFlightRefElementArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct ReferencingDetailUnion
    {
        public ReferencingDetailUnion(JsonReader reader, JsonSerializer serializer)
        {
            ReferencingDetailElement = null;
            ReferencingDetailElementArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    ReferencingDetailElementArray = serializer.Deserialize<List<ReferencingDetailElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    ReferencingDetailElement = serializer.Deserialize<ReferencingDetailElement>(reader);
                    return;
            }
            throw new Exception("Cannot convert ReferencingDetailUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (ReferencingDetailElement != null)
            {
                serializer.Serialize(writer, ReferencingDetailElement);
                return;
            }
            if (ReferencingDetailElementArray != null)
            {
                serializer.Serialize(writer, ReferencingDetailElementArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct CoveragePerFlightsInfoUnion
    {
        public CoveragePerFlightsInfoUnion(JsonReader reader, JsonSerializer serializer)
        {
            CoveragePerFlightsInfoElement = null;
            CoveragePerFlightsInfoElementArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    CoveragePerFlightsInfoElementArray = serializer.Deserialize<List<CoveragePerFlightsInfoElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    CoveragePerFlightsInfoElement = serializer.Deserialize<CoveragePerFlightsInfoElement>(reader);
                    return;
            }
            throw new Exception("Cannot convert CoveragePerFlightsInfoUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (CoveragePerFlightsInfoElement != null)
            {
                serializer.Serialize(writer, CoveragePerFlightsInfoElement);
                return;
            }
            if (CoveragePerFlightsInfoElementArray != null)
            {
                serializer.Serialize(writer, CoveragePerFlightsInfoElementArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }
    public partial struct LastItemsDetailUnion
    {
        public LastItemsDetailUnion(JsonReader reader, JsonSerializer serializer)
        {
            LastItemsDetailElement = null;
            LastItemsDetailElementArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    LastItemsDetailElementArray = serializer.Deserialize<List<LastItemsDetailElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    LastItemsDetailElement = serializer.Deserialize<LastItemsDetailElement>(reader);
                    return;
            }
            throw new Exception("Cannot convert LastItemsDetailUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (LastItemsDetailElement != null)
            {
                serializer.Serialize(writer, LastItemsDetailElement);
                return;
            }
            if (LastItemsDetailElementArray != null)
            {
                serializer.Serialize(writer, LastItemsDetailElementArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }
    public partial struct ServiceCoverageInfoGrpUnion
    {
        public ServiceCoverageInfoGrpUnion(JsonReader reader, JsonSerializer serializer)
        {
            ServiceCoverageInfoGrpElement = null;
            ServiceCoverageInfoGrpElementArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    ServiceCoverageInfoGrpElementArray = serializer.Deserialize<List<ServiceCoverageInfoGrpElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    ServiceCoverageInfoGrpElement = serializer.Deserialize<ServiceCoverageInfoGrpElement>(reader);
                    return;
            }
            throw new Exception("Cannot convert ServiceCoverageInfoGrpUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (ServiceCoverageInfoGrpElement != null)
            {
                serializer.Serialize(writer, ServiceCoverageInfoGrpElement);
                return;
            }
            if (ServiceCoverageInfoGrpElementArray != null)
            {
                serializer.Serialize(writer, ServiceCoverageInfoGrpElementArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct FreeBagAllowanceGrpUnion
    {
        public FreeBagAllowanceGrpUnion(JsonReader reader, JsonSerializer serializer)
        {
            FreeBagAllowanceGrpElement = null;
            FreeBagAllowanceGrpElementArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    FreeBagAllowanceGrpElementArray = serializer.Deserialize<List<FreeBagAllowanceGrpElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    FreeBagAllowanceGrpElement = serializer.Deserialize<FreeBagAllowanceGrpElement>(reader);
                    return;
            }
            throw new Exception("Cannot convert FreeBagAllowanceGrpUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (FreeBagAllowanceGrpElement != null)
            {
                serializer.Serialize(writer, FreeBagAllowanceGrpElement);
                return;
            }
            if (FreeBagAllowanceGrpElementArray != null)
            {
                serializer.Serialize(writer, FreeBagAllowanceGrpElementArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public partial struct ServiceCovInfoGrpUnion
    {
        public ServiceCovInfoGrpUnion(JsonReader reader, JsonSerializer serializer)
        {
            ServiceCovInfoGrpElement = null;
            ServiceCovInfoGrpElementArray = null;

            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    ServiceCovInfoGrpElementArray = serializer.Deserialize<List<ServiceCovInfoGrpElement>>(reader);
                    return;
                case JsonToken.StartObject:
                    ServiceCovInfoGrpElement = serializer.Deserialize<ServiceCovInfoGrpElement>(reader);
                    return;
            }
            throw new Exception("Cannot convert ServiceCovInfoGrpUnion");
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (ServiceCovInfoGrpElement != null)
            {
                serializer.Serialize(writer, ServiceCovInfoGrpElement);
                return;
            }
            if (ServiceCovInfoGrpElementArray != null)
            {
                serializer.Serialize(writer, ServiceCovInfoGrpElementArray);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public static class Serialize
    {
        public static string ToJson(this Response self) => JsonConvert.SerializeObject(self, BL.Entities.MasterPricer.Converter.Settings);
    }

    internal class Converter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FlightDetailsUnion) || t == typeof(PaxFareProductUnion) || t == typeof(FareUnion) || t == typeof(PriceType) || t == typeof(TentacledGroupOfFares) || t == typeof(CodeShareDetails) || t == typeof(TravellerUnion) || t == typeof(FareDetails) || t == typeof(FareDetailsGroupOfFares) || t == typeof(SegmentFlightRefUnion) || t == typeof(ReferencingDetailUnion) || t == typeof(FlightIndexUnion) || t == typeof(RecommendationUnion) || t == typeof(FareDetailUnion) || t == typeof(PaxReferenceUnion) || t == typeof(ReferencingDetailUnion) || t == typeof(GroupOfFlightUnion) || t == typeof(FlightDetailsUnion?) || t == typeof(PaxFareProductUnion?) || t == typeof(FareUnion?) || t == typeof(PriceType?) || t == typeof(TentacledGroupOfFares?) || t == typeof(CodeShareDetails?) || t == typeof(TravellerUnion?) || t == typeof(FareDetails?) || t == typeof(FareDetailsGroupOfFares?) || t == typeof(SegmentFlightRefUnion?) || t == typeof(ReferencingDetailUnion?) || t == typeof(FlightIndexUnion?) || t == typeof(FareDetailUnion?) || t == typeof(FareTypeUnion?) || t == typeof(PaxReferenceUnion?) || t == typeof(ReferencingDetailUnion?) || t == typeof(GroupOfFlightUnion?) || t == typeof(RecommendationUnion?) || t == typeof(CoveragePerFlightsInfoUnion) || t == typeof(CoveragePerFlightsInfoUnion?) || t == typeof(LastItemsDetailUnion) || t == typeof(LastItemsDetailUnion?) || t == typeof(ServiceCoverageInfoGrpUnion) || t == typeof(ServiceCoverageInfoGrpUnion?) || t == typeof(ServiceCovInfoGrpUnion) || t == typeof(ServiceCovInfoGrpUnion?) || t == typeof(FreeBagAllowanceGrpUnion) || t == typeof(FreeBagAllowanceGrpUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (t == typeof(FlightDetailsUnion) || t == typeof(FlightDetailsUnion?))
                return new FlightDetailsUnion(reader, serializer);
            if (t == typeof(PaxFareProductUnion) || t == typeof(PaxFareProductUnion?))
                return new PaxFareProductUnion(reader, serializer);
            if (t == typeof(FareUnion) || t == typeof(FareUnion?))
                return new FareUnion(reader, serializer);
            if (t == typeof(PriceType) || t == typeof(PriceType?))
                return new PriceType(reader, serializer);
            if (t == typeof(TentacledGroupOfFares) || t == typeof(TentacledGroupOfFares?))
                return new TentacledGroupOfFares(reader, serializer);
            if (t == typeof(CodeShareDetails) || t == typeof(CodeShareDetails?))
                return new CodeShareDetails(reader, serializer);
            if (t == typeof(TravellerUnion) || t == typeof(TravellerUnion?))
                return new TravellerUnion(reader, serializer);
            if (t == typeof(FareDetails) || t == typeof(FareDetails?))
                return new FareDetails(reader, serializer);
            if (t == typeof(FareDetailsGroupOfFares) || t == typeof(FareDetailsGroupOfFares?))
                return new FareDetailsGroupOfFares(reader, serializer);
            if (t == typeof(SegmentFlightRefUnion) || t == typeof(SegmentFlightRefUnion?))
                return new SegmentFlightRefUnion(reader, serializer);
            if (t == typeof(ReferencingDetailUnion) || t == typeof(ReferencingDetailUnion?))
                return new ReferencingDetailUnion(reader, serializer);
            if (t == typeof(FlightIndexUnion) || t == typeof(FlightIndexUnion?))
                return new FlightIndexUnion(reader, serializer);
            if (t == typeof(FareDetailUnion) || t == typeof(FareDetailUnion?))
                return new FareDetailUnion(reader, serializer);
            if (t == typeof(FareTypeUnion) || t == typeof(FareTypeUnion?))
                return new FareTypeUnion(reader, serializer);
            if (t == typeof(PaxReferenceUnion) || t == typeof(PaxReferenceUnion?))
                return new PaxReferenceUnion(reader, serializer);
            if (t == typeof(GroupOfFlightUnion) || t == typeof(GroupOfFlightUnion?))
                return new GroupOfFlightUnion(reader, serializer);
            if (t == typeof(RecommendationUnion) || t == typeof(RecommendationUnion?))
                return new RecommendationUnion(reader, serializer);
            if (t == typeof(CoveragePerFlightsInfoUnion) || t == typeof(CoveragePerFlightsInfoUnion?))
                return new CoveragePerFlightsInfoUnion(reader, serializer);
            if (t == typeof(LastItemsDetailUnion) || t == typeof(LastItemsDetailUnion?))
                return new LastItemsDetailUnion(reader, serializer);
            if (t == typeof(ServiceCoverageInfoGrpUnion) || t == typeof(ServiceCoverageInfoGrpUnion?))
                return new ServiceCoverageInfoGrpUnion(reader, serializer);
            if (t == typeof(ServiceCovInfoGrpUnion) || t == typeof(ServiceCovInfoGrpUnion?))
                return new ServiceCovInfoGrpUnion(reader, serializer);
            if (t == typeof(FreeBagAllowanceGrpUnion) || t == typeof(FreeBagAllowanceGrpUnion?))
                return new FreeBagAllowanceGrpUnion(reader, serializer);
            throw new Exception("Unknown type");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = value.GetType();
            if (t == typeof(FlightIndexUnion))
            {
                ((FlightIndexUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(FareDetailUnion))
            {
                ((FareDetailUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(FareTypeUnion))
            {
                ((FareTypeUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(PaxReferenceUnion))
            {
                ((PaxReferenceUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(FlightDetailsUnion))
            {
                ((FlightDetailsUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(PaxFareProductUnion))
            {
                ((PaxFareProductUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(FareUnion))
            {
                ((FareUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(PriceType))
            {
                ((PriceType)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(TentacledGroupOfFares))
            {
                ((TentacledGroupOfFares)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(CodeShareDetails))
            {
                ((CodeShareDetails)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(TravellerUnion))
            {
                ((TravellerUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(FareDetails))
            {
                ((FareDetails)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(FareDetailsGroupOfFares))
            {
                ((FareDetailsGroupOfFares)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(SegmentFlightRefUnion))
            {
                ((SegmentFlightRefUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(ReferencingDetailUnion))
            {
                ((ReferencingDetailUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(GroupOfFlightUnion))
            {
                ((GroupOfFlightUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(RecommendationUnion))
            {
                ((RecommendationUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(CoveragePerFlightsInfoUnion))
            {
                ((CoveragePerFlightsInfoUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(LastItemsDetailUnion))
            {
                ((LastItemsDetailUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(ServiceCoverageInfoGrpUnion))
            {
                ((ServiceCoverageInfoGrpUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(ServiceCovInfoGrpUnion))
            {
                ((ServiceCovInfoGrpUnion)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(FreeBagAllowanceGrpUnion))
            {
                ((FreeBagAllowanceGrpUnion)value).WriteJson(writer, serializer);
                return;
            }
            throw new Exception("Unknown type");
        }

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new Converter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
