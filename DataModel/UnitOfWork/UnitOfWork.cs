#region Using Namespaces...

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data.Entity.Validation;
//
using DataModel.GenericRepository;

#endregion
namespace DataModel.UnitOfWork
{
    /// <summary>
    /// Unit of Work class responsible for DB transactions
    /// </summary>
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        #region Private member variables...

        private BookingDBEntities context = null;
        private GenericRepository<APIToken> apiToken;
        private GenericRepository<APIUser> apiUser;
        private GenericRepository<AirlineCode> airlineCode;
        private GenericRepository<AirlineControl> airline;
        private GenericRepository<AirlineControlSub> airlineSub;
        private GenericRepository<AirlineQtaxControl> airlineQtax;
        private GenericRepository<AirportCode> airportCode;
        private GenericRepository<AirportLocation> airportLocation;
        private GenericRepository<AirPromotion> airPromotion;
        private GenericRepository<AirportWithCity> airportWithCity;
        private GenericRepository<AirportTranfer> airportTranfer;
        private GenericRepository<BackOfficeAdmin> backOfficeAdmin;
        private GenericRepository<CityCode> cityCode;
        private GenericRepository<CountryCode> countryCode;
        private GenericRepository<CurrencyExchange> currencyExchange;
        private GenericRepository<FlightSearchLog> flightSearchLog;
        private GenericRepository<FlightBooking> flightBooking;
        private GenericRepository<FlightBookingContactInfo> flightBookingContactInfo;
        private GenericRepository<FlightBookingFare> flightBookingFare;
        private GenericRepository<FlightBookingBaggage> flightBookingBaggage;
        private GenericRepository<FlightBookingFlightDetail> flightBookingFlightDetail;
        private GenericRepository<FlightBookingPaxInfo> flightBookingPaxInfo;
        private GenericRepository<FlightBookingRefund> flightBookingRefund;
        private GenericRepository<FlightBookingReissue> flightBookingReissue;
        private GenericRepository<FlightBookingFareRule> flightBookingFareRule;
        private GenericRepository<FlightBookingFareRuleDatail> flightBookingFareRuleDetail;
        private GenericRepository<FourDigitControl> fourDigit;
        private GenericRepository<PassportConfig> passportConfig;
        private GenericRepository<KBankCharge> kbankCharge;
        private GenericRepository<MarkupFlight> markupFlight;
        private GenericRepository<Payment> payment;
        private GenericRepository<RunningNumber> runningNumber;
        private GenericRepository<SeatMapControl> seatMap;
        private GenericRepository<ChillPayBackground> chillPayBackgroundTransaction;
        private GenericRepository<ChillPayInquiry> chillPayInquiryTransaction;
        private GenericRepository<ChillPayResult> chillPayResultTransaction;
        private GenericRepository<PaymentReference> paymentReferenceTransaction;
        private GenericRepository<FlightSearchMultiTicket> flightSearchMultiTicket;

        private GenericRepository<FlightFareRule> flightFareRule;
        private GenericRepository<FlightFareRuleDetail> flightFareRuleDetail;
        private GenericRepository<FlightFareRuleConfig> flightFareRuleConfig;

        private GenericRepository<AirlineConfig> airlineConfig;
        private GenericRepository<SiteConfig> siteConfig;
        private GenericRepository<DiscountTag> discountTag;
        private GenericRepository<DiscountTagDetail> discountTagDetail;
        private GenericRepository<PromotionGroupCode> promotionGroupCode;
        private GenericRepository<PaymentLog> paymentLog;

        #endregion

        public UnitOfWork()
        {
            context = new BookingDBEntities();
        }

        #region Public Repository Creation properties...


        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<BackOfficeAdmin> BackOffceAdminRepository
        {
            get
            {
                if (this.backOfficeAdmin == null)
                    this.backOfficeAdmin = new GenericRepository<BackOfficeAdmin>(context);
                return backOfficeAdmin;
            }
        }

        /// <summary>
        /// Get/Set Property for APIToken repository.
        /// </summary>
        public GenericRepository<APIToken> APITokenRepository
        {
            get
            {
                if (this.apiToken == null)
                    this.apiToken = new GenericRepository<APIToken>(context);
                return apiToken;
            }
        }

        /// <summary>
        /// Get/Set Property for APIUser repository.
        /// </summary>
        public GenericRepository<APIUser> APIUserRepository
        {
            get
            {
                if (this.apiUser == null)
                    this.apiUser = new GenericRepository<APIUser>(context);
                return apiUser;
            }
        }

        /// <summary>
        /// Get/Set Property for AirlineCode repository.
        /// </summary>
        public GenericRepository<AirlineCode> AirlineCodeRepository
        {
            get
            {
                if (this.airlineCode == null)
                    this.airlineCode = new GenericRepository<AirlineCode>(context);
                return airlineCode;
            }
        }

        /// <summary>
        /// Get/Set Property for AirportCode repository.
        /// </summary>
        public GenericRepository<AirportCode> AirportCodeRepository
        {
            get
            {
                if (this.airportCode == null)
                    this.airportCode = new GenericRepository<AirportCode>(context);
                return airportCode;
            }
        }

        public GenericRepository<AirlineControl> AirlineControlRepository
        {
            get
            {
                if (this.airline == null)
                    this.airline = new GenericRepository<AirlineControl>(context);
                return airline;
            }
        }

        public GenericRepository<AirlineControlSub> AirlineControlSubRepository
        {
            get
            {
                if (this.airlineSub == null)
                    this.airlineSub = new GenericRepository<AirlineControlSub>(context);
                return airlineSub;
            }
        }

        public GenericRepository<AirlineQtaxControl> AirlineQtaxControlRepository
        {
            get
            {
                if (this.airlineQtax == null)
                    this.airlineQtax = new GenericRepository<AirlineQtaxControl>(context);
                return airlineQtax;
            }
        }

        /// <summary>
        /// Get/Set Property for AirportLocation repository.
        /// </summary>
        public GenericRepository<AirportLocation> AirportLocationRepository
        {
            get
            {
                if (this.airportLocation == null)
                    this.airportLocation = new GenericRepository<AirportLocation>(context);
                return airportLocation;
            }
        }
        /// <summary>
        /// Get/Set Property for AirPromotion repository.
        /// </summary>
        public GenericRepository<AirPromotion> AirPromotionRepository
        {
            get
            {
                if (this.airPromotion == null)
                    this.airPromotion = new GenericRepository<AirPromotion>(context);
                return airPromotion;
            }
        }

        /// <summary>
        /// Get/Set Property for AirportWithCity repository.
        /// </summary>
        public GenericRepository<AirportWithCity> AirportWithCityRepository
        {
            get
            {
                if (this.airportWithCity == null)
                    this.airportWithCity = new GenericRepository<AirportWithCity>(context);
                return airportWithCity;
            }
        }
        
        /// <summary>
         /// Get/Set Property for AirportWithCity repository.
         /// </summary>
        public GenericRepository<AirportTranfer> AirportTranferRepository
        {
            get
            {
                if (this.airportTranfer == null)
                    this.airportTranfer = new GenericRepository<AirportTranfer>(context);
                return airportTranfer;
            }
        }

        /// <summary>
        /// Get/Set Property for CityCode repository.
        /// </summary>
        public GenericRepository<CityCode> CityCodeRepository
        {
            get
            {
                if (this.cityCode == null)
                    this.cityCode = new GenericRepository<CityCode>(context);
                return cityCode;
            }
        }

        /// <summary>
        /// Get/Set Property for CountryCode repository.
        /// </summary>
        public GenericRepository<CountryCode> CountryCodeRepository
        {
            get
            {
                if (this.countryCode == null)
                    this.countryCode = new GenericRepository<CountryCode>(context);
                return countryCode;
            }
        }

        /// <summary>
        /// Get/Set Property for CurrencyExchange repository.
        /// </summary>
        public GenericRepository<CurrencyExchange> CurrencyExchangeRepository
        {
            get
            {
                if (this.currencyExchange == null)
                    this.currencyExchange = new GenericRepository<CurrencyExchange>(context);
                return currencyExchange;
            }
        }

        /// <summary>
        /// Get/Set Property for FlightSearchLog repository.
        /// </summary>
        public GenericRepository<FlightSearchLog> FlightSearchLogRepository
        {
            get
            {
                if (this.flightSearchLog == null)
                    this.flightSearchLog = new GenericRepository<FlightSearchLog>(context);
                return flightSearchLog;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FlightBooking> FlightBookingRepository
        {
            get
            {
                if (this.flightBooking == null)
                    this.flightBooking = new GenericRepository<FlightBooking>(context);
                return flightBooking;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FlightBookingContactInfo> FlightBookingContactInfoRepository
        {
            get
            {
                if (this.flightBookingContactInfo == null)
                    this.flightBookingContactInfo = new GenericRepository<FlightBookingContactInfo>(context);
                return flightBookingContactInfo;
            }
        }
        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FlightBookingFare> FlightBookingFareRepository
        {
            get
            {
                if (this.flightBookingFare == null)
                    this.flightBookingFare = new GenericRepository<FlightBookingFare>(context);
                return flightBookingFare;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FlightBookingBaggage> FlightBookingBaggageRepository
        {
            get
            {
                if (this.flightBookingBaggage == null)
                    this.flightBookingBaggage = new GenericRepository<FlightBookingBaggage>(context);
                return flightBookingBaggage;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FlightBookingFlightDetail> FlightBookingFlightDetailRepository
        {
            get
            {
                if (this.flightBookingFlightDetail == null)
                    this.flightBookingFlightDetail = new GenericRepository<FlightBookingFlightDetail>(context);
                return flightBookingFlightDetail;
            }
        }


        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FlightBookingPaxInfo> FlightBookingPaxInfoRepository
        {
            get
            {
                if (this.flightBookingPaxInfo == null)
                    this.flightBookingPaxInfo = new GenericRepository<FlightBookingPaxInfo>(context);
                return flightBookingPaxInfo;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FlightBookingRefund> FlightBookingRefundRepository
        {
            get
            {
                if (this.flightBookingRefund == null)
                    this.flightBookingRefund = new GenericRepository<FlightBookingRefund>(context);
                return flightBookingRefund;
            }
        }


        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FlightBookingReissue> FlightBookingReissueRepository
        {
            get
            {
                if (this.flightBookingReissue == null)
                    this.flightBookingReissue = new GenericRepository<FlightBookingReissue>(context);
                return flightBookingReissue;
            }
        }

        

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FlightBookingFareRule> FlightBookingFareRuleRepository
        {
            get
            {
                if (this.flightBookingFareRule == null)
                    this.flightBookingFareRule = new GenericRepository<FlightBookingFareRule>(context);
                return flightBookingFareRule;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FlightBookingFareRuleDatail> FlightBookingFareRuleDatailRepository
        {
            get
            {
                if (this.flightBookingFareRuleDetail == null)
                    this.flightBookingFareRuleDetail = new GenericRepository<FlightBookingFareRuleDatail>(context);
                return flightBookingFareRuleDetail;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<FourDigitControl> FourDigitRepository
        {
            get
            {
                if (this.fourDigit == null)
                    this.fourDigit = new GenericRepository<FourDigitControl>(context);
                return fourDigit;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<PassportConfig> PassportConfigRepository
        {
            get
            {
                if (this.passportConfig == null)
                    this.passportConfig = new GenericRepository<PassportConfig>(context);
                return passportConfig;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<MarkupFlight> MarkupFlightRepository
        {
            get
            {
                if (this.markupFlight == null)
                    this.markupFlight = new GenericRepository<MarkupFlight>(context);
                return markupFlight;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<SeatMapControl> SeatMapRepository
        {
            get
            {
                if (this.seatMap == null)
                    this.seatMap = new GenericRepository<SeatMapControl>(context);
                return seatMap;
            }
        }

        /// <summary>
        /// Get/Set Property for repository.
        /// </summary>
        public GenericRepository<KBankCharge> KBankChargeRepository
        {
            get
            {
                if (this.kbankCharge == null)
                    this.kbankCharge = new GenericRepository<KBankCharge>(context);
                return kbankCharge;
            }
        }

        public GenericRepository<ChillPayBackground> ChillPayBackgroundRepository
        {
            get
            {
                if (this.chillPayBackgroundTransaction == null)
                    this.chillPayBackgroundTransaction = new GenericRepository<ChillPayBackground>(context);
                return chillPayBackgroundTransaction;
            }
        }

        public GenericRepository<ChillPayInquiry> ChillPayInquiryRepository
        {
            get
            {
                if (this.chillPayInquiryTransaction == null)
                    this.chillPayInquiryTransaction = new GenericRepository<ChillPayInquiry>(context);
                return chillPayInquiryTransaction;
            }
        }

        public GenericRepository<ChillPayResult> ChillPayResultRepository
        {
            get
            {
                if (this.chillPayResultTransaction == null)
                    this.chillPayResultTransaction = new GenericRepository<ChillPayResult>(context);
                return chillPayResultTransaction;
            }
        }

        public GenericRepository<Payment> PaymentRepository
        {
            get
            {
                if (this.payment == null)
                    this.payment = new GenericRepository<Payment>(context);
                return payment;
            }
        }
        public GenericRepository<PaymentReference> PaymentReferenceRepository
        {
            get
            {
                if (this.paymentReferenceTransaction == null)
                    this.paymentReferenceTransaction = new GenericRepository<PaymentReference>(context);
                return paymentReferenceTransaction;
            }
        }

        public GenericRepository<RunningNumber> RunningNumberRepository
        {
            get
            {
                if (this.runningNumber == null)
                    this.runningNumber = new GenericRepository<RunningNumber>(context);
                return runningNumber;
            }
        }        

        public GenericRepository<FlightSearchMultiTicket> FlightSearchMultiTicketRepository
        {
            get
            {
                if (this.flightSearchMultiTicket == null)
                    this.flightSearchMultiTicket = new GenericRepository<FlightSearchMultiTicket>(context);
                return flightSearchMultiTicket;
            }
        }

        public GenericRepository<FlightFareRule> FlightFareRuleRepository
        {
            get
            {
                if (this.flightFareRule == null)
                    this.flightFareRule = new GenericRepository<FlightFareRule>(context);
                return flightFareRule;
            }
        }

        public GenericRepository<FlightFareRuleDetail> FlightFareRuleDetailRepository
        {
            get
            {
                if (this.flightFareRuleDetail == null)
                    this.flightFareRuleDetail = new GenericRepository<FlightFareRuleDetail>(context);
                return flightFareRuleDetail;
            }
        }

        public GenericRepository<FlightFareRuleConfig> FlightFareRuleConfigRepository
        {
            get
            {
                if (this.flightFareRuleConfig == null)
                    this.flightFareRuleConfig = new GenericRepository<FlightFareRuleConfig>(context);
                return flightFareRuleConfig;
            }
        }

        public GenericRepository<AirlineConfig> AirlineConfigRepository
        {
            get
            {
                if (this.airlineConfig == null)
                    this.airlineConfig = new GenericRepository<AirlineConfig>(context);
                return airlineConfig;
            }
        }

        public GenericRepository<SiteConfig> SiteConfigRepository
        {
            get
            {
                if (this.siteConfig == null)
                    this.siteConfig = new GenericRepository<SiteConfig>(context);
                return siteConfig;
            }
        }

        public GenericRepository<DiscountTag> DiscountTagRepository
        {
            get
            {
                if (this.discountTag == null)
                    this.discountTag = new GenericRepository<DiscountTag>(context);
                return discountTag;
            }
        }
        public GenericRepository<DiscountTagDetail> DiscountTagDetailRepository
        {
            get
            {
                if (this.discountTagDetail == null)
                    this.discountTagDetail = new GenericRepository<DiscountTagDetail>(context);
                return discountTagDetail;
            }
        }

        public GenericRepository<PromotionGroupCode> PromotionGroupCodeRepository
        {
            get
            {
                if (this.promotionGroupCode == null)
                    this.promotionGroupCode = new GenericRepository<PromotionGroupCode>(context);
                return promotionGroupCode;
            }
        }

        public GenericRepository<PaymentLog> PaymentLogRepository
        {
            get
            {
                if (this.paymentLog == null)
                    this.paymentLog = new GenericRepository<PaymentLog>(context);
                return paymentLog;
            }
        }

        #endregion

        #region Public member methods...
        public void ConfigFastRange()
        {
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;
        }

        /// <summary>
        /// Save method.
        /// </summary>
        public void Save()
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {

                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.Add(string.Format("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:", DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }
                //System.IO.File.AppendAllLines(@"C:\errors.txt", outputLines);

                throw e;
            }

        }

        public void SaveBulk()
        {
            try
            {
                context.BulkSaveChanges();
            }
            catch (DbEntityValidationException e)
            {

                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.Add(string.Format("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:", DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }
                //System.IO.File.AppendAllLines(@"C:\errors.txt", outputLines);

                throw e;
            }

        }
        #endregion

        #region Implementing IDiosposable...

        #region private dispose variable declaration...
        private bool disposed = false;
        #endregion

        /// <summary>
        /// Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Debug.WriteLine("UnitOfWork is being disposed");
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
