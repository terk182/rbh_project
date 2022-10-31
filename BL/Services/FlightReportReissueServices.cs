using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BL.Entities.RobinhoodFare;
using BL.Entities.RobinhoodFlight;
using BL.Entities.RobinhoodPax;
using DataModel;
using DataModel.UnitOfWork;

namespace BL
{
    public class FlightReportReissueServices : IFlightReportReissueServices
    {
        private readonly UnitOfWork _unitOfWork;

        public FlightReportReissueServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       

        public List<AirFare.Reissue> GetAllReissue()
        {
            List<AirFare.Reissue> airfareEntity = new List<AirFare.Reissue>();
            var flightbookreissue = _unitOfWork.FlightBookingReissueRepository.GetAll().OrderByDescending(x => x.ReissueCreateDate).ToList();
            foreach (var flight in flightbookreissue)
            {
                AirFare.Reissue airfare = new AirFare.Reissue();
                airfare.FlightBookingReissueOID = flight.BookingOID;
                airfare.reissueCreateDate = flight.ReissueCreateDate ?? DateTime.MinValue;
                airfare.status = flight.Status ?? 0;
                airfare.typeChage = flight.TypeChage ?? 0;
                airfare.detailChage = flight.DetailChage ?? 0;
                airfare.newPNR = flight.NewPNR;
                airfareEntity.Add(airfare);
            }


            return airfareEntity;
            //return _unitOfWork.FlightBookingRepository.GetAll().OrderBy(x => x.TKTL).ToList();
        }

        
        public AirFare GetByID(Guid id)
        {

            AirFare airfareEntity = new AirFare();
            FlightBooking flightBooking = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == id);
            airfareEntity.bookingOID = flightBooking.BookingOID.ToString();
            airfareEntity.noOfAdults = flightBooking.NoOfAdults ?? 0;
            airfareEntity.noOfChildren = flightBooking.NoOfChildren ?? 0;
            airfareEntity.noOfInfants = flightBooking.NoOfInfants ?? 0;
            airfareEntity.svc_class = flightBooking.Svc_class;
            airfareEntity.grandTotal = flightBooking.GrandTotal ?? 0;
            airfareEntity.isPassportRequired = flightBooking.IsPassportRequired ?? false;
            airfareEntity.PNR = flightBooking.PNR;
            airfareEntity.Platform = flightBooking.Platform;
            airfareEntity.medId = flightBooking.MedId ?? 0;
            airfareEntity.priceChange = flightBooking.PriceChange ?? false;
            airfareEntity.oldPrice = flightBooking.OldPrice ?? 0;
            airfareEntity.TKTL = flightBooking.TKTL ?? DateTime.MinValue;
            airfareEntity.bookingDate = flightBooking.bookingDate ?? DateTime.MinValue;
            //airfareEntity.remarks = flightBooking.Remarks;
            //airfareEntity.totalFare = flightBooking.TotalFare;
            airfareEntity.isError = flightBooking.IsError ?? false;
            airfareEntity.errorMessage = flightBooking.ErrorMessage;
            airfareEntity.statusPayment = flightBooking.StatusPayment ?? 0;
            airfareEntity.RobinhoodID = flightBooking.RobinhoodID;
            airfareEntity.statusBooking = flightBooking.StatusBooking ?? 0;
            airfareEntity.paymentMethod = flightBooking.PaymentMethod ?? 0;

            airfareEntity.contactInfo = new ContactInfo();
            ContactInfo contactInfo = new ContactInfo();
            contactInfo = new ContactInfo();
            contactInfo.title = flightBooking.Title;
            contactInfo.firstname = flightBooking.Firstname;
            contactInfo.middlename = flightBooking.Middlename;
            contactInfo.lastname = flightBooking.Lastname;
            contactInfo.email = flightBooking.Email;
            contactInfo.telNo = flightBooking.TelNo;
            contactInfo.countryCode = flightBooking.CountryOfResidence;
            airfareEntity.contactInfo = contactInfo;

            //ContactInfo contactInfo = new ContactInfo();
            //var contactList = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID == id);
            //FlightBooking booking = new FlightBooking();
            //if (contactInfo != null)
            //{
            //    contactInfo.title = booking.Title;
            //    contactInfo.firstname = booking.Firstname;
            //    contactInfo.middlename = booking.Middlename;
            //    contactInfo.lastname = booking.Lastname;
            //    contactInfo.email = booking.Email;
            //    contactInfo.telNo = booking.TelNo;
            //    contactInfo.countryCode = booking.CountryOfResidence;
            //    airfareEntity.contactInfo = contactInfo;
            //}

            NamingServices _namingServices = null;
            City cityOrigin = new City(_namingServices, "en");
            cityOrigin.code = flightBooking.Origin;
            airfareEntity.origin = cityOrigin;
            City cityDestination = new City(_namingServices, "en");
            cityDestination.code = flightBooking.Destination;
            airfareEntity.destination = cityDestination;
            airfareEntity.note = flightBooking.Note;

            Fare fare = new Fare();
            List<FlightBookingFare> bookingfareList = _unitOfWork.FlightBookingFareRepository.GetMany(x => x.BookingOID == id).ToList();
            FlightBookingFare bookingfare = new FlightBookingFare();
            bookingfare = bookingfareList.Find(x => x.PessengerType == "adtFare");
            fare.baseFare = bookingfare.BaseFare ?? 0;
            fare.sellingBaseFare = bookingfare.SellingBaseFare ?? 0;
            fare.lessFare = bookingfare.LessFare ?? 0;
            fare.qtax = bookingfare.Qtax ?? 0;
            fare.tax = bookingfare.Tax ?? 0;
            airfareEntity.adtFare = fare;

            fare = new Fare();
            bookingfare = bookingfareList.Find(x => x.PessengerType == "chdFare");
            if (bookingfare != null)
            {
                fare.baseFare = bookingfare.BaseFare ?? 0;
                fare.sellingBaseFare = bookingfare.SellingBaseFare ?? 0;
                fare.lessFare = bookingfare.LessFare ?? 0;
                fare.qtax = bookingfare.Qtax ?? 0;
                fare.tax = bookingfare.Tax ?? 0;
                airfareEntity.chdFare = fare;
            }

            fare = new Fare();
            bookingfare = bookingfareList.Find(x => x.PessengerType == "intFare");
            if (bookingfare != null)
            {
                fare.baseFare = bookingfare.BaseFare ?? 0;
                fare.sellingBaseFare = bookingfare.SellingBaseFare ?? 0;
                fare.lessFare = bookingfare.LessFare ?? 0;
                fare.qtax = bookingfare.Qtax ?? 0;
                fare.tax = bookingfare.Tax ?? 0;
                airfareEntity.infFare = fare;
            }



            airfareEntity.depFlight = new List<FlightDetail>();
            airfareEntity.retFlight = new List<FlightDetail>();
            //NamingServices _namingServices=null;
            Airport airportdepart = new Airport(_namingServices, true, "en");
            Airline airlinedepart = new Airline(_namingServices, "en");
            Airport airportreturn = new Airport(_namingServices, true, "en");
            Airline airlinereturn = new Airline(_namingServices, "en");
            FlightDetail flightdepart = new FlightDetail();
            List<FlightBookingFlightDetail> flightdetailListdepart = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == id && x.TripType == "depart").OrderBy(x => x.Seq).ToList();
            FlightBookingFlightDetail flightdetaildepart = new FlightBookingFlightDetail();
            flightdetaildepart = flightdetailListdepart.Find(x => x.TripType == "depart");
            foreach (var flightdetail in flightdetailListdepart)
            {
                flightdepart = new FlightDetail();
                airportdepart = new Airport(_namingServices, true, "en");
                airportdepart.code = flightdetail.DepCity;
                flightdepart.depCity = airportdepart;
                airportreturn = new Airport(_namingServices, true, "en");
                airportreturn.code = flightdetail.ArrCity;
                flightdepart.arrCity = airportreturn;
                airlinedepart = new Airline(_namingServices, "en");
                airlinedepart.code = flightdetail.Airline;
                flightdepart.airline = airlinedepart;
                airlinereturn = new Airline(_namingServices, "en");
                airlinereturn.code = flightdetail.OperatedAirline;
                flightdepart.operatedAirline = airlinereturn;

                flightdepart.fareBasis = flightdetail.FareBasis;
                flightdepart.fareType = flightdetail.FareType;
                flightdepart.cabin = flightdetail.Cabin;
                airportdepart.code = flightdetail.DepCity;
                flightdepart.depCity = airportdepart;
                flightdepart.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                airportreturn.code = flightdetail.ArrCity;
                flightdepart.arrCity = airportreturn;
                flightdepart.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                flightdepart.Seq = flightdetail.Seq ?? 0;
                airfareEntity.depFlight.Add(flightdepart);
            }

            Airport airportdepart1 = new Airport(_namingServices, true, "en");
            Airport airportreturn1 = new Airport(_namingServices, true, "en");
            Airline airlinedepart1 = new Airline(_namingServices, "en");
            Airline airlinereturn1 = new Airline(_namingServices, "en");
            FlightDetail flightreturn = new FlightDetail();
            List<FlightBookingFlightDetail> flightdetailListreturn = _unitOfWork.FlightBookingFlightDetailRepository.GetMany(x => x.BookingOID == id && x.TripType == "return").OrderBy(x => x.Seq).ToList();
            FlightBookingFlightDetail flightdetailreturn = new FlightBookingFlightDetail();
            flightdetailreturn = flightdetailListreturn.Find(x => x.TripType == "return");
            if (flightdetailreturn != null)
            {

                foreach (var flightdetail in flightdetailListreturn)
                {
                    flightreturn = new FlightDetail();
                    airportdepart1 = new Airport(_namingServices, true, "en");
                    airportdepart1.code = flightdetail.DepCity;
                    flightreturn.depCity = airportdepart1;
                    airportreturn1 = new Airport(_namingServices, true, "en");
                    airportreturn1.code = flightdetail.ArrCity;
                    flightreturn.arrCity = airportreturn1;
                    airlinedepart1 = new Airline(_namingServices, "en");
                    airlinedepart.code = flightdetail.Airline;
                    flightreturn.airline = airlinedepart;
                    airlinereturn1 = new Airline(_namingServices, "en");
                    airlinereturn.code = flightdetail.OperatedAirline;
                    flightreturn.operatedAirline = airlinereturn;

                    flightreturn.fareBasis = flightdetail.FareBasis;
                    flightreturn.fareType = flightdetail.FareType;
                    flightreturn.cabin = flightdetail.Cabin;
                    airportdepart1.code = flightdetail.DepCity;
                    flightreturn.depCity = airportdepart1;
                    flightreturn.departureDateTime = flightdetail.DepartureDateTime ?? DateTime.MinValue;
                    airportreturn1.code = flightdetail.ArrCity;
                    flightreturn.arrCity = airportreturn1;
                    flightreturn.arrivalDateTime = flightdetail.ArrivalDateTime ?? DateTime.MinValue;
                    flightreturn.Seq = flightdetail.Seq ?? 0;
                    airfareEntity.retFlight.Add(flightreturn);
                }
            }

            airfareEntity.adtPaxs = new List<PaxInfo>();
            airfareEntity.chdPaxs = new List<PaxInfo>();
            airfareEntity.infPaxs = new List<PaxInfo>();
            //PaxInfo paxInfoadt = new PaxInfo();
            List<FlightBookingPaxInfo> paxinfoadtList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID == id).ToList();
            List<FlightBookingPaxInfo> paxinfoadtDetail = new List<FlightBookingPaxInfo>();
            paxinfoadtDetail = paxinfoadtList.Where(x => x.PaxType == "ADT").ToList();
            foreach (var adt in paxinfoadtDetail)
            {
                PaxInfo paxInfoadt = new PaxInfo();
                paxInfoadt.paxType = adt.PaxType;
                paxInfoadt.title = adt.Title;
                paxInfoadt.firstname = adt.Firstname;
                paxInfoadt.middlename = adt.Middlename;
                paxInfoadt.lastname = adt.Lastname;
                paxInfoadt.birthday = adt.Birthday ?? DateTime.MinValue;
                paxInfoadt.email = adt.Email;
                paxInfoadt.telNo = adt.TelNo;
                paxInfoadt.passportNumber = adt.PassportNumber;
                paxInfoadt.passportIssuingDate = adt.PassportIssuingDate ?? DateTime.MinValue;
                paxInfoadt.passportExpiryDate = adt.PassportExpiryDate ?? DateTime.MinValue;
                paxInfoadt.passportIssuingCountry = adt.PassportIssuingCountry;
                paxInfoadt.passportNationality = adt.PassportNationality;
                paxInfoadt.netRefund = adt.NetRefund ?? 0;
                paxInfoadt.agentRefund = adt.AgentRefund ?? 0;
                paxInfoadt.refundAmount = adt.RefundAmount ?? 0;
                paxInfoadt.tickNoRefund = adt.TickNoRefund;
                paxInfoadt.remarkRefund = adt.RemarkRefund;
                paxInfoadt.netReissue = adt.NetReissue ?? 0;
                paxInfoadt.agentReissue = adt.AgentReissue ?? 0;
                paxInfoadt.reissueSelling = adt.ReissueSelling ?? 0;
                paxInfoadt.tickNoReissueOld = adt.TickNoReissueOld;
                paxInfoadt.tickNoReissueNew = adt.TickNoReissueNew;
                paxInfoadt.remarkReissue = adt.RemarkReissue;
                paxInfoadt.StatusRefund = adt.StatusRefund ?? false;
                paxInfoadt.StatusReissue = adt.StatusReissue ?? false;
                airfareEntity.adtPaxs.Add(paxInfoadt);
            }

            //PaxInfo paxInfochd = new PaxInfo();
            List<FlightBookingPaxInfo> paxinfochdList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID == id).ToList();
            List<FlightBookingPaxInfo> paxinfochdDetail = new List<FlightBookingPaxInfo>();
            paxinfochdDetail = paxinfochdList.Where(x => x.PaxType == "CHD").ToList();
            if (paxinfochdDetail != null)
            {
                foreach (var chd in paxinfochdDetail)
                {
                    PaxInfo paxInfochd = new PaxInfo();
                    paxInfochd.paxType = chd.PaxType;
                    paxInfochd.title = chd.Title;
                    paxInfochd.firstname = chd.Firstname;
                    paxInfochd.middlename = chd.Middlename;
                    paxInfochd.lastname = chd.Lastname;
                    paxInfochd.birthday = chd.Birthday ?? DateTime.MinValue;
                    paxInfochd.email = chd.Email;
                    paxInfochd.telNo = chd.TelNo;
                    paxInfochd.passportNumber = chd.PassportNumber;
                    paxInfochd.passportIssuingDate = chd.PassportIssuingDate ?? DateTime.MinValue;
                    paxInfochd.passportExpiryDate = chd.PassportExpiryDate ?? DateTime.MinValue;
                    paxInfochd.passportIssuingCountry = chd.PassportIssuingCountry;
                    paxInfochd.passportNationality = chd.PassportNationality;
                    paxInfochd.netRefund = chd.NetRefund ?? 0;
                    paxInfochd.agentRefund = chd.AgentRefund ?? 0;
                    paxInfochd.refundAmount = chd.RefundAmount ?? 0;
                    paxInfochd.tickNoRefund = chd.TickNoRefund;
                    paxInfochd.remarkRefund = chd.RemarkRefund;
                    paxInfochd.netReissue = chd.NetReissue ?? 0;
                    paxInfochd.agentReissue = chd.AgentReissue ?? 0;
                    paxInfochd.reissueSelling = chd.ReissueSelling ?? 0;
                    paxInfochd.tickNoReissueOld = chd.TickNoReissueOld;
                    paxInfochd.tickNoReissueNew = chd.TickNoReissueNew;
                    paxInfochd.remarkReissue = chd.RemarkReissue;
                    paxInfochd.StatusRefund = chd.StatusRefund ?? false;
                    paxInfochd.StatusReissue = chd.StatusReissue ?? false;
                    airfareEntity.chdPaxs.Add(paxInfochd);
                }
            }

            //PaxInfo paxInfoinf = new PaxInfo();
            List<FlightBookingPaxInfo> paxinfoinfList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID == id).ToList();
            List<FlightBookingPaxInfo> paxinfoinfDetail = new List<FlightBookingPaxInfo>();
            paxinfoinfDetail = paxinfochdList.Where(x => x.PaxType == "INF").ToList();
            if (paxinfoinfDetail != null)
            {
                foreach (var inf in paxinfoinfDetail)
                {
                    PaxInfo paxInfoinf = new PaxInfo();
                    paxInfoinf.paxType = inf.PaxType;
                    paxInfoinf.title = inf.Title;
                    paxInfoinf.firstname = inf.Firstname;
                    paxInfoinf.middlename = inf.Middlename;
                    paxInfoinf.lastname = inf.Lastname;
                    paxInfoinf.birthday = inf.Birthday ?? DateTime.MinValue;
                    paxInfoinf.email = inf.Email;
                    paxInfoinf.telNo = inf.TelNo;
                    paxInfoinf.passportNumber = inf.PassportNumber;
                    paxInfoinf.passportIssuingDate = inf.PassportIssuingDate ?? DateTime.MinValue;
                    paxInfoinf.passportExpiryDate = inf.PassportExpiryDate ?? DateTime.MinValue;
                    paxInfoinf.passportIssuingCountry = inf.PassportIssuingCountry;
                    paxInfoinf.passportNationality = inf.PassportNationality;
                    paxInfoinf.netRefund = inf.NetRefund ?? 0;
                    paxInfoinf.agentRefund = inf.AgentRefund ?? 0;
                    paxInfoinf.refundAmount = inf.RefundAmount ?? 0;
                    paxInfoinf.tickNoRefund = inf.TickNoRefund;
                    paxInfoinf.remarkRefund = inf.RemarkRefund;
                    paxInfoinf.netReissue = inf.NetReissue ?? 0;
                    paxInfoinf.agentReissue = inf.AgentReissue ?? 0;
                    paxInfoinf.reissueSelling = inf.ReissueSelling ?? 0;
                    paxInfoinf.tickNoReissueOld = inf.TickNoReissueOld;
                    paxInfoinf.tickNoReissueNew = inf.TickNoReissueNew;
                    paxInfoinf.remarkReissue = inf.RemarkReissue;
                    paxInfoinf.StatusRefund = inf.StatusRefund ?? false;
                    paxInfoinf.StatusReissue = inf.StatusReissue ?? false;
                    airfareEntity.infPaxs.Add(paxInfoinf);
                }
            }

            BL.Entities.RobinhoodFare.AirFare.Refund refund = new BL.Entities.RobinhoodFare.AirFare.Refund();
            airfareEntity.refund = new BL.Entities.RobinhoodFare.AirFare.Refund();
            List<FlightBookingRefund> refundList = _unitOfWork.FlightBookingRefundRepository.GetMany(x => x.BookingOID == id).ToList();
            List<FlightBookingRefund> refundDetail = new List<FlightBookingRefund>();
            refundDetail = refundList.Where(x => x.Status == 1).ToList();
            if (refundDetail != null)
            {
                foreach (var refunddetail in refundList)
                {
                    refund = new BL.Entities.RobinhoodFare.AirFare.Refund();
                    refund.status = refunddetail.Status ?? 0;
                    refund.newPNR = refunddetail.NewPNR;
                    refund.refundNo = refunddetail.RefundNo;
                    refund.refundCreateDate = refunddetail.RefundCreateDate ?? DateTime.MinValue;
                    refund.refundGMDate = refunddetail.RefundGMDate ?? DateTime.MinValue;
                    refund.dueDateOfRefund = refunddetail.DueDateOfRefund ?? DateTime.MinValue;
                    refund.remark = refunddetail.Remark;
                    airfareEntity.refund = refund;
                }
            }

            BL.Entities.RobinhoodFare.AirFare.Reissue reissue = new BL.Entities.RobinhoodFare.AirFare.Reissue();
            airfareEntity.reissue = new BL.Entities.RobinhoodFare.AirFare.Reissue();
            List<FlightBookingReissue> reissueList = _unitOfWork.FlightBookingReissueRepository.GetMany(x => x.BookingOID == id).ToList();
            List<FlightBookingReissue> reissueDetail = new List<FlightBookingReissue>();
            reissueDetail = reissueList.Where(x => x.Status == 1).ToList();
            if (reissueDetail != null)
            {
                foreach (var reissuedetail in reissueList)
                {
                    reissue = new BL.Entities.RobinhoodFare.AirFare.Reissue();
                    reissue.status = reissuedetail.Status ?? 0;
                    reissue.newPNR = reissuedetail.NewPNR;
                    reissue.reissueCreateDate = reissuedetail.ReissueCreateDate ?? DateTime.MinValue;
                    reissue.typeChage = reissuedetail.TypeChage ?? 0;
                    reissue.detailChage = reissuedetail.DetailChage ?? 0;
                    reissue.remark = reissuedetail.Remark;
                    airfareEntity.reissue = reissue;
                }
            }




            airfareEntity.fareRules = new List<FareRule>();
            FareRule farerule = new FareRule();
            List<FlightBookingFareRule> fareruleList = _unitOfWork.FlightBookingFareRuleRepository.GetMany(x => x.BookingOID == id).ToList();
            FlightBookingFareRule fareruleDetail = new FlightBookingFareRule();
            farerule.fareBasis = fareruleDetail.FareBasis;
            City city1 = new City(_namingServices, "en");
            city1.code = fareruleDetail.Origin;
            farerule.origin = city1;
            city1.code = fareruleDetail.Destination;
            farerule.destination = city1;
            FareRuleDatail details = new FareRuleDatail();
            List<FlightBookingFareRuleDatail> detailList = _unitOfWork.FlightBookingFareRuleDatailRepository.GetMany(x => x.FlightBookingFareRuleDatailOID == id).ToList();
            FlightBookingFareRuleDatail detail = new FlightBookingFareRuleDatail();
            //details.fareRuleText = detail.FareRuleText;
            airfareEntity.fareRules.Add(farerule);

            return airfareEntity;
        }

        public void SaveOrUpdateReissue(AirFare airfare)
        {
            using (var scope = new TransactionScope())
            {
                AirFare airfareEntity = new AirFare();
                //FlightBooking flightBooking = _unitOfWork.FlightBookingRepository.GetFirstOrDefault(x => x.BookingOID.ToString() == airfare.bookingOID);
                //if (flightBooking == null)
                //{
                //    _unitOfWork.FlightBookingRepository.Insert(flightBooking);
                //}
                //else
                //{
                //    //flightBooking.GrandTotal = airfare.grandTotal;
                //    //flightBooking.TKTL = airfare.TKTL;
                //    flightBooking.Note = airfare.note;
                //    _unitOfWork.FlightBookingRepository.Update(flightBooking);
                //}

                //FlightBookingRefund refund = new FlightBookingRefund();
                FlightBookingReissue reissue = _unitOfWork.FlightBookingReissueRepository.GetFirstOrDefault(x => x.BookingOID.ToString() == airfare.bookingOID);
                if (reissue == null)
                {
                    FlightBookingReissue reissue1 = new FlightBookingReissue();
                    Guid reissueid = Guid.NewGuid();
                    reissue1.FlightBookingReissueOID = reissueid;
                    reissue1.BookingOID = new Guid(airfare.bookingOID);
                    reissue1.Status = 1;
                    reissue1.NewPNR = airfare.reissue.newPNR;
                    reissue1.ReissueCreateDate = airfare.reissue.reissueCreateDate;
                    reissue1.TypeChage = 0;
                    reissue1.DetailChage = 0;
                    reissue1.Remark = airfare.reissue.remark;
                    _unitOfWork.FlightBookingReissueRepository.Insert(reissue1);
                    _unitOfWork.Save();
                }
                else
                {
                    reissue.Status = airfare.refund.status;
                    reissue.NewPNR = airfare.reissue.newPNR;
                    reissue.ReissueCreateDate = airfare.reissue.reissueCreateDate;
                    reissue.TypeChage = airfare.reissue.typeChage;
                    reissue.DetailChage = airfare.reissue.detailChage;
                    reissue.Remark = airfare.reissue.remark;
                    _unitOfWork.FlightBookingRefundRepository.DetachAll();
                    _unitOfWork.FlightBookingReissueRepository.Update(reissue);
                    _unitOfWork.Save();
                }

                List<FlightBookingPaxInfo> adtpaxList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID.ToString() == airfare.bookingOID && x.PaxType == "ADT").ToList();
                if (adtpaxList != null && adtpaxList.Count > 0)
                {
                    foreach (var adtPax in adtpaxList)
                    {
                        adtPax.StatusReissue = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).StatusReissue;
                        adtPax.NetReissue = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).netReissue;
                        adtPax.AgentReissue = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).agentReissue;
                        adtPax.ReissueSelling = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).reissueSelling;
                        adtPax.TickNoReissueOld = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).tickNoReissueOld;
                        adtPax.TickNoReissueNew = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).tickNoReissueNew;
                        adtPax.RemarkReissue = airfare.adtPaxs.Find(x => x.firstname == adtPax.Firstname && x.lastname == adtPax.Lastname).remarkReissue;
                        _unitOfWork.FlightBookingPaxInfoRepository.Update(adtPax);
                        _unitOfWork.Save();
                    }
                }

                List<FlightBookingPaxInfo> chdpaxList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID.ToString() == airfare.bookingOID && x.PaxType == "CHD").ToList();
                if (chdpaxList != null && chdpaxList.Count > 0)
                {
                    foreach (var chdPax in chdpaxList)
                    {
                        chdPax.StatusReissue = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).StatusReissue;
                        chdPax.NetReissue = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).netReissue;
                        chdPax.AgentReissue = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).agentReissue;
                        chdPax.ReissueSelling = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).reissueSelling;
                        chdPax.TickNoReissueOld = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).tickNoReissueOld;
                        chdPax.TickNoReissueNew = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).tickNoReissueNew;
                        chdPax.RemarkReissue = airfare.chdPaxs.Find(x => x.firstname == chdPax.Firstname && x.lastname == chdPax.Lastname).remarkReissue;
                        _unitOfWork.FlightBookingPaxInfoRepository.Update(chdPax);
                        _unitOfWork.Save();
                    }
                }

                List<FlightBookingPaxInfo> infpaxList = _unitOfWork.FlightBookingPaxInfoRepository.GetMany(x => x.BookingOID.ToString() == airfare.bookingOID && x.PaxType == "INF").ToList();
                if (infpaxList != null && infpaxList.Count > 0)
                {
                    foreach (var infPax in infpaxList)
                    {
                        infPax.StatusReissue = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).StatusReissue;
                        infPax.NetReissue = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).netReissue;
                        infPax.AgentReissue = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).agentReissue;
                        infPax.ReissueSelling = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).reissueSelling;
                        infPax.TickNoReissueOld = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).tickNoReissueOld;
                        infPax.TickNoReissueNew = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).tickNoReissueNew;
                        infPax.RemarkReissue = airfare.infPaxs.Find(x => x.firstname == infPax.Firstname && x.lastname == infPax.Lastname).remarkReissue;
                        _unitOfWork.FlightBookingPaxInfoRepository.Update(infPax);
                        _unitOfWork.Save();
                    }
                }
                scope.Complete();
            }
        }
    }
}
