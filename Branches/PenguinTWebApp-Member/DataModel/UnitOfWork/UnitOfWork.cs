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

        private GogojiiWebDBEntities context = null;
        private GenericRepository<AirlineCode> airlineCode;
        private GenericRepository<AirportCode> airportCode;
        private GenericRepository<AirPromotion> airPromotion;
        private GenericRepository<AirportWithCity> airportWithCity;
        private GenericRepository<CityCode> cityCode;
        private GenericRepository<CountryCode> countryCode;
        private GenericRepository<FlightSearchLog> flightSearchLog;
        private GenericRepository<ShortenURL> shortenURL;
        #endregion

        public UnitOfWork()
        {
            context = new GogojiiWebDBEntities();
        }

        #region Public Repository Creation properties...


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
        /// Get/Set Property for ShortenURL repository.
        /// </summary>
        public GenericRepository<ShortenURL> ShortenURLRepository
        {
            get
            {
                if (this.shortenURL == null)
                    this.shortenURL = new GenericRepository<ShortenURL>(context);
                return shortenURL;
            }
        }
        #endregion

        #region Public member methods...
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
