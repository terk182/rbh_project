using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BL.Entities.RobinhoodFlight;
using DataModel;
using DataModel.UnitOfWork;
using log4net;

namespace BL
{
    public class MarkupServices : IMarkupServices
    {
        private readonly UnitOfWork _unitOfWork;
        private static readonly ILog Log =
             LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MarkupServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<MarkupFlight> GetAll()
        {
            return _unitOfWork.MarkupFlightRepository.GetMany(x => x.IsDelete == false).ToList();
        }

        public MarkupFlight GetByID(Guid RobinhoodMarkupOID)
        {
            return _unitOfWork.MarkupFlightRepository.GetFirstOrDefault(x => x.RobinhoodMarkupOID == RobinhoodMarkupOID && x.IsDelete == false);
        }

        public MarkupFlight GetDuplicate(MarkupFlight model)
        {
            //return _unitOfWork.MarkupFlightRepository.GetFirstOrDefault(x => x.AirlineCodes == model.AirlineCodes && x.PaxTypeADT == model.PaxTypeADT
            //&& x.PaxTypeCHD == model.PaxTypeCHD && x.PaxTypeINF == model.PaxTypeINF && x.FlightNo.IndexOf(model.FlightNo) >-1 && x.FareBasis.IndexOf(model.FareBasis) >-1
            //&& x.RBD.IndexOf(model.RBD) > - 1 && x.ZoneFrom.IndexOf(model.ZoneFrom) >-1 && x.ZoneTo.IndexOf(model.ZoneTo) > -1 && (x.MinPrice <= model.MinPrice && x.MaxPrice >= model.MinPrice) 
            //&& (x.StartBookingDate.Value.Date <= model.StartBookingDate.Value.Date && x.FinishBookingDate.Value.Date >= model.StartBookingDate.Value.Date) &&(x.StartTravelDate.Value.Date  <= model.StartTravelDate.Value.Date 
            //&& x.FinishTravelDate.Value.Date >= model.StartTravelDate.Value.Date) && x.RobinhoodMarkupOID != model.RobinhoodMarkupOID && x.IsDelete == false);

            //IndexOf
            //return _unitOfWork.MarkupFlightRepository.GetFirstOrDefault(x => x.AirlineCodes == model.AirlineCodes && x.PaxTypeADT == model.PaxTypeADT
            //&& x.PaxTypeCHD == model.PaxTypeCHD && x.PaxTypeINF == model.PaxTypeINF && x.FlightNo.IndexOf(model.FlightNo) > -1 && x.FareBasis.IndexOf(model.FareBasis) > -1
            //&& x.RBD.IndexOf(model.RBD) > -1 && x.ZoneFrom.IndexOf(model.ZoneFrom) > -1 && x.ZoneTo.IndexOf(model.ZoneTo) > -1 && (x.MinPrice <= model.MinPrice && x.MaxPrice >= model.MinPrice)
            //&& (x.StartBookingDate.Value.Date <= model.StartBookingDate.Value.Date && x.FinishBookingDate.Value.Date >= model.StartBookingDate.Value.Date) && x.RobinhoodMarkupOID != model.RobinhoodMarkupOID && x.Type == model.Type && x.IsDelete == false && x.DomainName == model.DomainName);

            return _unitOfWork.MarkupFlightRepository.GetFirstOrDefault(x => x.AirlineCodes == model.AirlineCodes && x.PaxTypeADT == model.PaxTypeADT
            && x.PaxTypeCHD == model.PaxTypeCHD && x.PaxTypeINF == model.PaxTypeINF && x.FlightNo == model.FlightNo && x.FareBasis == model.FareBasis
            && x.RBD == model.RBD && x.ZoneFrom == model.ZoneFrom && x.ZoneTo == model.ZoneTo && (x.MinPrice <= model.MinPrice && x.MaxPrice >= model.MinPrice)
            && (x.StartBookingDate.Value.Date <= model.StartBookingDate.Value.Date && x.FinishBookingDate.Value.Date >= model.StartBookingDate.Value.Date) && x.RobinhoodMarkupOID != model.RobinhoodMarkupOID && x.Type == model.Type && x.IsDelete == false && x.DomainName == model.DomainName);
        }

        public void SaveOrUpdate(MarkupFlight GogojiiMarkup)
        {
            using (var scope = new TransactionScope())
            {
                var check = _unitOfWork.MarkupFlightRepository.GetFirstOrDefault(x => x.RobinhoodMarkupOID == GogojiiMarkup.RobinhoodMarkupOID);
                if (check == null)
                {
                    _unitOfWork.MarkupFlightRepository.Insert(GogojiiMarkup);
                }
                else
                {
                    _unitOfWork.MarkupFlightRepository.Update(GogojiiMarkup);
                }
                _unitOfWork.Save();

                scope.Complete();
            }
        }

        public FareMarkup GetMarkup()
        {

            FareMarkup markup = new FareMarkup();
            if (System.Web.HttpContext.Current.Cache["C_FARENET"] != null)
            {
                Log.Debug("C_FARENET!=null");
                markup.MarkupList = (List<MarkupFlight>)System.Web.HttpContext.Current.Cache["C_FARENET"];
                
            }
            else
            {
                Log.Debug("C_FARENET==null");
                //Caching
                markup.MarkupList = this.GetAll();
                System.Web.HttpContext.Current.Cache.Insert("C_FARENET", markup.MarkupList, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);

            }
            return markup;
        }

        public bool RemoveCache()
        {
            if (System.Web.HttpContext.Current.Cache["C_FARENET"] != null)
            {
                System.Web.HttpContext.Current.Cache.Remove("C_FARENET");                
            }
            if (System.Web.HttpContext.Current.Cache["C_FARENET"] != null)
            {
                Log.Debug("C_FARENET!=null");
                return false;
            }
            else
            {
                Log.Debug("C_FARENET==null");
                return true;
            }
        }
    }
}
